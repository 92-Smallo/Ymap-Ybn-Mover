using CodeWalker.GameFiles;
using SharpDX;

namespace Ymap_Ybn_Mover;

internal static class CollisionVertexPacking
{
    public static void Repack(BoundGeometry geometry, Vector3 originalQuantum, IResourceBlock[] references)
    {
        // CodeWalker derives Quantum from half the box width. Raw vertices are relative
        // to CenterGeom, which need not be the box midpoint; unused vertices and shrunk
        // vertices can also extend beyond polygon bounds. Its short encoder clamps these
        // coordinates silently. Keep at least the original range and cover every vertex.
        var maximum = Vector3.Zero;
        foreach (var vertices in new[] { geometry.VerticesShrunk, geometry.Vertices })
            if (vertices != null)
                foreach (var vertex in vertices)
                    maximum = Vector3.Max(maximum, new Vector3(MathF.Abs(vertex.X), MathF.Abs(vertex.Y), MathF.Abs(vertex.Z)));
        var needed = maximum / 32767f;
        geometry.Quantum = new Vector3(
            SafeQuantum(geometry.Quantum.X, originalQuantum.X, needed.X),
            SafeQuantum(geometry.Quantum.Y, originalQuantum.Y, needed.Y),
            SafeQuantum(geometry.Quantum.Z, originalQuantum.Z, needed.Z));

        // These are the actual blocks referenced by CodeWalker's writer, in its
        // GetReferences order: shrunk vertices first (when present), then vertices.
        var arrays = new[] { geometry.VerticesShrunk, geometry.Vertices }.Where(v => v != null).ToArray();
        var blocks = references.OfType<ResourceSystemStructBlock<BoundVertex_s>>().ToArray();
        if (blocks.Length != arrays.Length)
            throw new InvalidDataException("Unexpected CodeWalker vertex blocks; the collision has not been saved.");
        for (int i = 0; i < blocks.Length; i++)
        {
            var vertices = arrays[i]!;
            if (blocks[i].Items.Length != vertices.Length)
                throw new InvalidDataException("Unexpected CodeWalker vertex count; the collision has not been saved.");
            for (int j = 0; j < vertices.Length; j++)
            {
                var packed = vertices[j] / geometry.Quantum;
                if (!float.IsFinite(packed.X) || !float.IsFinite(packed.Y) || !float.IsFinite(packed.Z))
                    throw new InvalidDataException("The collision contains non-finite vertex coordinates.");
                blocks[i].Items[j] = new BoundVertex_s(new Vector3(MathF.Round(packed.X), MathF.Round(packed.Y), MathF.Round(packed.Z)));
            }
        }
    }

    private static float SafeQuantum(float calculated, float original, float needed)
    {
        float value = MathF.Max(needed, MathF.Max(float.IsFinite(original) ? original : 0, float.IsFinite(calculated) ? calculated : 0));
        return value > 0 ? value : 1e-6f;
    }
}
