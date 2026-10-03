using System.Security.Cryptography;
using CodeWalker.World;
using CodeWalker.GameFiles;
using SharpDX;
using Ymap_Ybn_Mover;

// Optional local fixtures. No game assets are included in the repository.
internal static class AssetAudit
{
    public static int Run(string[] args)
    {
        if (args.Length is < 2 or > 3)
        {
            Console.Error.WriteLine("Usage: RegressionTests <input-folder> <output-folder> [previous-90-degree-output-folder]");
            return 1;
        }
        var input = Path.GetFullPath(args[0]);
        var output = Path.GetFullPath(args[1]);
        var previous = args.Length == 3 ? Path.GetFullPath(args[2]) : null;
        if (Overlaps(output, input) || (previous != null && Overlaps(output, previous)))
            throw new ArgumentException("Test output must use a separate folder.");
        Directory.CreateDirectory(output);
        var context = Directory.GetFiles(input);
        var sharedCentre = MapBatch.GetCentreAsync(context, CancellationToken.None).GetAwaiter().GetResult() ?? throw new Exception("No map bounds found.");
        Console.WriteLine($"Shared map centre: {sharedCentre}");
        var failed = false;
        int files = 0, vertices = 0, rays = 0;
        foreach (var path in Directory.GetFiles(input).Where(p => Path.GetExtension(p).Equals(".ybn", StringComparison.OrdinalIgnoreCase)).Order())
        {
            var bytes = File.ReadAllBytes(path);
            var source = Load(bytes);
            var count = Meshes(source.Bounds, Matrix.Identity).Sum(m => m.Mesh.Vertices.Length);
            files++; vertices += count;
            Console.WriteLine($"{Path.GetFileName(path)}: {count} vertices");
            foreach (var transform in new[] { new MapTransform(Vector3.Zero, 0, sharedCentre), new MapTransform(Vector3.Zero, 37, sharedCentre),
                new MapTransform(Vector3.Zero, 90, sharedCentre), new MapTransform(Vector3.Zero, -90, sharedCentre), new MapTransform(new Vector3(20, -30, 7), 90, sharedCentre) })
            {
                var result = FileProcessor.ConvertFile(path, bytes, transform);
                var loaded = Load(result);
                var label = transform.Offset == Vector3.Zero ? transform.HasRotation ? Angle(transform) : "resave" : "90-plus-offset";
                var folder = Path.Combine(output, label); Directory.CreateDirectory(folder);
                File.WriteAllBytes(Path.Combine(folder, Path.GetFileName(path)), result);
                var check = Compare(source, loaded, transform);
                rays += check.Rays;
                bool bounds = EnclosesChildren(loaded.Bounds);
                Console.WriteLine($"  {label}: max vertex error={check.Error:F6} m; rays={check.Rays}, mismatches={check.Misses}; metadata={check.Metadata}; bounds={bounds}");
                failed |= check.Error > 0.01 || check.Misses != 0 || !check.Metadata || !bounds || check.Rays == 0;
            }
            if (previous != null && File.Exists(Path.Combine(previous, Path.GetFileName(path))))
            {
                var check = Compare(source, Load(File.ReadAllBytes(Path.Combine(previous, Path.GetFileName(path)))), new MapTransform(Vector3.Zero, 90));
                Console.WriteLine($"  previous output: max vertex error={check.Error:F6} m; ray mismatches={check.Misses}/{check.Rays}");
                var old = Load(File.ReadAllBytes(Path.Combine(previous, Path.GetFileName(path))));
                Console.WriteLine($"    original bounds={source.Bounds.BoxMin}..{source.Bounds.BoxMax}; previous bounds={old.Bounds.BoxMin}..{old.Bounds.BoxMax}; encloses={EnclosesChildren(old.Bounds)}");
            }
            if (!SHA256.HashData(bytes).SequenceEqual(SHA256.HashData(File.ReadAllBytes(path))))
                throw new Exception("Original file changed during the audit: " + path);
        }
        foreach (var path in Directory.GetFiles(input, "*.ymap").Order())
        {
            var bytes = File.ReadAllBytes(path);
            var source = new YmapFile(); source.Load(bytes);
            var transform = new MapTransform(Vector3.Zero, 90, sharedCentre);
            var converted = FileProcessor.ConvertFile(path, bytes, transform);
            var loaded = new YmapFile(); loaded.Load(converted);
            var original = source.AllEntities ?? [];
            var actual = loaded.AllEntities ?? [];
            bool valid = original.Length == actual.Length && original.Select((e, i) =>
                Vector3.Distance(transform.Position(e.Position), actual[i].Position) < 0.01f &&
                Vector3.Distance(transform.Direction(Vector3.Transform(Vector3.UnitX, e.Orientation)), Vector3.Transform(Vector3.UnitX, actual[i].Orientation)) < 0.001f).All(v => v);
            Directory.CreateDirectory(Path.Combine(output, "90"));
            File.WriteAllBytes(Path.Combine(output, "90", Path.GetFileName(path)), converted);
            Console.WriteLine($"{Path.GetFileName(path)}: 90 degrees, {actual.Length} entity positions/orientations valid={valid}");
            failed |= !valid;
            if (!bytes.SequenceEqual(File.ReadAllBytes(path))) throw new Exception("Original YMAP changed.");
        }
        Console.WriteLine($"{files} collision files, {vertices} vertices, {rays} ray comparisons; {(failed || files == 0 ? "FAILED" : "PASS")}");
        return failed || files == 0 ? 1 : 0;
    }

    private static string Angle(MapTransform t) => MathF.Round(MathF.Atan2(t.Matrix.M12, t.Matrix.M11) * 180 / MathF.PI).ToString(System.Globalization.CultureInfo.InvariantCulture);
    private static bool Overlaps(string a, string b)
    {
        a = a.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar) + Path.DirectorySeparatorChar;
        b = b.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar) + Path.DirectorySeparatorChar;
        return a.StartsWith(b, StringComparison.OrdinalIgnoreCase) || b.StartsWith(a, StringComparison.OrdinalIgnoreCase);
    }
    private static YbnFile Load(byte[] bytes) { var ybn = new YbnFile(); ybn.Load(bytes); return ybn; }
    private static IEnumerable<(BoundGeometry Mesh, Matrix Placement)> Meshes(Bounds b, Matrix parent)
    {
        var placement = b.Transform * parent;
        if (b is BoundComposite c)
        {
            foreach (var child in c.Children.data_items)
                if (child != null) foreach (var m in Meshes(child, placement)) yield return m;
        }
        else if (b is BoundGeometry g) yield return (g, placement);
    }

    private static (float Error, int Rays, int Misses, bool Metadata) Compare(YbnFile source, YbnFile loaded, MapTransform transform)
    {
        var before = Meshes(source.Bounds, Matrix.Identity).ToArray();
        var after = Meshes(loaded.Bounds, Matrix.Identity).ToArray();
        if (before.Length != after.Length) return (float.PositiveInfinity, 0, 0, false);
        float error = 0; int rays = 0, misses = 0; bool metadata = true;
        for (int m = 0; m < before.Length; m++)
        {
            var (a, placementA) = before[m]; var (b, placementB) = after[m];
            if (a.Vertices.Length != b.Vertices.Length) return (float.PositiveInfinity, 0, 0, false);
            for (int i = 0; i < a.Vertices.Length; i++)
                error = MathF.Max(error, Vector3.Distance(transform.Position(Vector3.TransformCoordinate(a.Vertices[i] + a.CenterGeom, placementA)),
                    Vector3.TransformCoordinate(b.Vertices[i] + b.CenterGeom, placementB)));
            metadata &= a.CompositeFlags1.Equals(b.CompositeFlags1) && a.CompositeFlags2.Equals(b.CompositeFlags2) &&
                a.Polygons.Length == b.Polygons.Length && Signatures(a).SequenceEqual(Signatures(b));
            var triangles = a.Polygons.OfType<BoundPolygonTriangle>().ToArray();
            for (int i = 0; i < triangles.Length; i += Math.Max(1, triangles.Length / 256))
            {
                var tri = triangles[i];
                var p1 = a.Vertices[tri.vertIndex1] + a.CenterGeom;
                var p2 = a.Vertices[tri.vertIndex2] + a.CenterGeom;
                var p3 = a.Vertices[tri.vertIndex3] + a.CenterGeom;
                var normal = Vector3.Cross(p2 - p1, p3 - p1);
                if (normal.LengthSquared() < 1e-6) continue;
                normal.Normalize();
                var ray = new Ray((p1 + p2 + p3) / 3 + normal * 0.25f, -normal);
                var originalHit = LocalHit(a, ray);
                if (!originalHit.Hit) continue;
                var worldRay = new Ray(transform.Position(Vector3.TransformCoordinate(ray.Position, placementA)),
                    transform.Direction(Vector3.TransformNormal(ray.Direction, placementA)));
                var inverse = Matrix.Invert(placementB);
                var localRay = new Ray(Vector3.TransformCoordinate(worldRay.Position, inverse), Vector3.TransformNormal(worldRay.Direction, inverse));
                // Compare the same effective local ray on both meshes. World/local
                // float roundoff can otherwise change the nearest overlapping face
                // even when all serialized vertices and the local BVH are unchanged.
                var expectedHit = LocalHit(a, localRay);
                var movedHit = LocalHit(b, localRay);
                rays++;
                if (Vector3.Distance(ray.Position, localRay.Position) > 0.005f || Vector3.Distance(ray.Direction, localRay.Direction) > 0.0001f ||
                    movedHit.Hit != expectedHit.Hit || (expectedHit.Hit && MathF.Abs(expectedHit.HitDist - movedHit.HitDist) > 0.01f))
                {
                    misses++;
                    Console.WriteLine($"    ray {i}: {expectedHit.Hit}/{expectedHit.HitDist} -> {movedHit.Hit}/{movedHit.HitDist}, local drift={Vector3.Distance(ray.Position, localRay.Position)}");
                }
            }
        }
        return (error, rays, misses, metadata);
    }

    private static IEnumerable<string> Signatures(BoundGeometry mesh) => mesh.Polygons.Select(p =>
        p.Type + ":" + string.Join(",", p.VertexIndices) + ":" + Convert.ToHexString(MetaTypes.ConvertArrayToBytes(new[] { p.Material }))).Order(StringComparer.Ordinal);

    private static SpaceRayIntersectResult LocalHit(BoundGeometry mesh, Ray ray)
    {
        var placement = mesh.Transform;
        try { mesh.Transform = Matrix.Identity; return mesh.RayIntersect(ref ray); }
        finally { mesh.Transform = placement; }
    }

    private static bool EnclosesChildren(Bounds b)
    {
        if (b is not BoundComposite c) return true;
        foreach (var child in c.Children.data_items)
        {
            if (child == null) continue;
            var box = MapTransform.TransformBounds(child.BoxMin, child.BoxMax, child.Transform);
            var epsilon = new Vector3(0.005f);
            if (Vector3.Min(box.Min, b.BoxMin - epsilon) != b.BoxMin - epsilon || Vector3.Max(box.Max, b.BoxMax + epsilon) != b.BoxMax + epsilon || !EnclosesChildren(child)) return false;
        }
        return true;
    }
}
