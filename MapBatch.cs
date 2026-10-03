using CodeWalker.GameFiles;
using SharpDX;

namespace Ymap_Ybn_Mover;

/// <summary>Resolve one shared centre before writing any files in a batch.</summary>
public static class MapBatch
{
    public static async Task<MapTransform> CreateTransformAsync(IEnumerable<string> files, Vector3 offset, float degrees, CancellationToken cancellationToken)
    {
        var transform = new MapTransform(offset, degrees);
        if (!transform.HasRotation) return transform;
        var centre = await GetCentreAsync(files, cancellationToken);
        return new MapTransform(offset, degrees, centre ?? Vector3.Zero);
    }

    public static async Task<Vector3?> GetCentreAsync(IEnumerable<string> files, CancellationToken cancellationToken)
    {
        var paths = files.Select(Path.GetFullPath).Distinct(StringComparer.OrdinalIgnoreCase).ToArray();
        var min = new Vector3(float.MaxValue);
        var max = new Vector3(float.MinValue);
        foreach (var path in paths.Where(p => Path.GetExtension(p).Equals(".ymap", StringComparison.OrdinalIgnoreCase)))
        {
            var bytes = await File.ReadAllBytesAsync(path, cancellationToken);
            var box = await Task.Run(() =>
            {
                var map = new YmapFile(); map.Load(bytes);
                if (map.Meta == null) throw new InvalidDataException($"Cannot calculate the map centre from {Path.GetFileName(path)}.");
                var a = map.CMapData.entitiesExtentsMin; var b = map.CMapData.entitiesExtentsMax;
                // An empty metadata map's zero defaults must not pull a distant map towards origin.
                if (a == Vector3.Zero && b == Vector3.Zero && (map.AllEntities?.Length ?? 0) == 0 &&
                    (map.GrassInstanceBatches?.Length ?? 0) == 0 && (map.DistantLODLights?.positions?.Length ?? 0) == 0)
                    return ((Vector3 Min, Vector3 Max)?)null;
                ValidateBounds(a, b, path);
                return ((Vector3 Min, Vector3 Max)?)(a, b);
            }, cancellationToken);
            if (box.HasValue) { min = Vector3.Min(min, box.Value.Min); max = Vector3.Max(max, box.Value.Max); }
        }
        // Prefer map extents. Collision roots can contain stale outer bounds, so the
        // fallback uses the placed leaf shapes throughout the hierarchy instead.
        if (min.X == float.MaxValue)
            foreach (var path in paths.Where(p => Path.GetExtension(p).Equals(".ybn", StringComparison.OrdinalIgnoreCase)))
            {
                var bytes = await File.ReadAllBytesAsync(path, cancellationToken);
                var boxes = await Task.Run(() =>
                {
                    var ybn = new YbnFile(); ybn.Load(bytes);
                    if (ybn.Bounds == null) throw new InvalidDataException($"No collision bounds in {Path.GetFileName(path)}.");
                    return LeafBounds(ybn.Bounds, Matrix.Identity, path).ToArray();
                }, cancellationToken);
                foreach (var box in boxes) { min = Vector3.Min(min, box.Min); max = Vector3.Max(max, box.Max); }
            }
        cancellationToken.ThrowIfCancellationRequested();
        return min.X == float.MaxValue ? null : min * 0.5f + max * 0.5f;
    }

    private static IEnumerable<(Vector3 Min, Vector3 Max)> LeafBounds(Bounds bounds, Matrix parent, string path)
    {
        var placement = bounds.Transform * parent;
        if (bounds is BoundComposite composite)
        {
            foreach (var child in composite.Children?.data_items ?? [])
                if (child != null) foreach (var box in LeafBounds(child, placement, path)) yield return box;
        }
        else
        {
            ValidateBounds(bounds.BoxMin, bounds.BoxMax, path);
            var box = MapTransform.TransformBounds(bounds.BoxMin, bounds.BoxMax, placement);
            ValidateBounds(box.Min, box.Max, path);
            yield return box;
        }
    }

    private static void ValidateBounds(Vector3 min, Vector3 max, string path)
    {
        if (!float.IsFinite(min.X) || !float.IsFinite(min.Y) || !float.IsFinite(min.Z) ||
            !float.IsFinite(max.X) || !float.IsFinite(max.Y) || !float.IsFinite(max.Z) || min.X > max.X || min.Y > max.Y || min.Z > max.Z)
            throw new InvalidDataException($"Invalid map bounds in {Path.GetFileName(path)}; the rotation centre cannot be calculated.");
    }
}
