using CodeWalker.GameFiles;
using SharpDX;

namespace Ymap_Ybn_Mover;

internal static class CollisionGeometryPlacement
{
    public static void Bake(BoundGeometry geometry)
    {
        var placement = geometry.Transform;
        if (placement == Matrix.Identity) return;
        var x = new Vector3(placement.M11, placement.M12, placement.M13);
        var y = new Vector3(placement.M21, placement.M22, placement.M23);
        var z = new Vector3(placement.M31, placement.M32, placement.M33);
        float scale = x.Length();
        float tolerance = MathF.Max(1, scale * scale) * 1e-5f;
        bool uniform = scale > 0 && MathF.Abs(x.LengthSquared() - y.LengthSquared()) <= tolerance &&
            MathF.Abs(x.LengthSquared() - z.LengthSquared()) <= tolerance && MathF.Abs(Vector3.Dot(x, y)) <= tolerance &&
            MathF.Abs(Vector3.Dot(x, z)) <= tolerance && MathF.Abs(Vector3.Dot(y, z)) <= tolerance;
        if (!uniform && geometry.Polygons?.Any(p => p.Type != BoundPolygonType.Triangle) == true)
            throw new NotSupportedException("A collision mesh with non-uniform scale/shear and primitive polygons cannot be baked without changing its shapes. The original has not been changed.");

        // CodeWalker's renderer reads Matrix.ScaleVector (the diagonal), so a 90°
        // placement matrix becomes scale (0,0,1). Its polygon queries also apply
        // Transform after the composite has already inverse-transformed the ray.
        // Store placed geometry in the parent frame and leave the child matrix at identity.
        var box = MapTransform.TransformBounds(geometry.BoxMin, geometry.BoxMax, placement);
        geometry.BoxMin = box.Min; geometry.BoxMax = box.Max;
        geometry.BoxCenter = Vector3.TransformCoordinate(geometry.BoxCenter, placement);
        geometry.SphereCenter = Vector3.TransformCoordinate(geometry.SphereCenter, placement);
        geometry.CenterGeom = Vector3.TransformCoordinate(geometry.CenterGeom, placement);
        foreach (var vertices in new[] { geometry.Vertices, geometry.VerticesShrunk })
            if (vertices != null)
                for (int i = 0; i < vertices.Length; i++) vertices[i] = Vector3.TransformNormal(vertices[i], placement);
        var q = geometry.Quantum;
        geometry.Quantum = new Vector3(
            MathF.Abs(placement.M11) * q.X + MathF.Abs(placement.M21) * q.Y + MathF.Abs(placement.M31) * q.Z,
            MathF.Abs(placement.M12) * q.X + MathF.Abs(placement.M22) * q.Y + MathF.Abs(placement.M32) * q.Z,
            MathF.Abs(placement.M13) * q.X + MathF.Abs(placement.M23) * q.Y + MathF.Abs(placement.M33) * q.Z);
        if (uniform)
        {
            geometry.Margin *= scale;
            geometry.SphereRadius *= scale;
            if (geometry.Polygons != null)
                foreach (var polygon in geometry.Polygons)
                    switch (polygon)
                    {
                        case BoundPolygonSphere sphere: sphere.sphereRadius *= scale; break;
                        case BoundPolygonCapsule capsule: capsule.capsuleRadius *= scale; break;
                        case BoundPolygonCylinder cylinder: cylinder.cylinderRadius *= scale; break;
                    }
        }
        geometry.Transform = Matrix.Identity;
        geometry.TransformInv = Matrix.Identity;
        geometry.CalculateOctants();
        if (geometry is BoundBVH bvh) bvh.BuildBVH(false);
        else
        {
            geometry.BoxCenter = (box.Min + box.Max) * 0.5f;
            geometry.SphereCenter = geometry.BoxCenter;
            geometry.SphereRadius = (box.Max - geometry.BoxCenter).Length();
        }
    }
}
