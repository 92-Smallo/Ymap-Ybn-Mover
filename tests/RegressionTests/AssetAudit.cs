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
        if (args.Length == 3 && args[0] == "--diagnose") return Diagnose(args[1], args[2]);
        if (args.Length == 3 && args[0] == "--repair") return Repair(args[1], args[2]);
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
                bool renderable = Meshes(loaded.Bounds, Matrix.Identity).All(m => m.Mesh.Transform == Matrix.Identity);
                Console.WriteLine($"  {label}: max vertex error={check.Error:F6} m; contacts={check.Rays}, missing/mismatched={check.Misses}, nearest-face changes={check.NearestChanges}; metadata={check.Metadata}; bounds={bounds}; renderer={renderable}");
                failed |= check.Error > 0.01 || check.Misses != 0 || !check.Metadata || !bounds || !renderable || check.Rays == 0;
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

    private static int Repair(string input, string output)
    {
        input = Path.GetFullPath(input); output = Path.GetFullPath(output);
        if (Overlaps(input, output)) throw new ArgumentException("Repair output must use a separate folder.");
        Directory.CreateDirectory(output);
        int files = 0, rays = 0; bool failed = false;
        foreach (var path in Directory.GetFiles(input, "*.ybn").Order())
        {
            var bytes = File.ReadAllBytes(path);
            var transform = new MapTransform(Vector3.Zero, 0);
            var data = FileProcessor.ConvertFile(path, bytes, transform);
            var result = Load(data);
            var check = Compare(Load(bytes), result, transform);
            bool renderer = Meshes(result.Bounds, Matrix.Identity).All(m => m.Mesh.Transform == Matrix.Identity);
            bool valid = check.Error <= 0.01 && check.Misses == 0 && check.Metadata && EnclosesChildren(result.Bounds) && renderer;
            Console.WriteLine($"{Path.GetFileName(path)}: world error={check.Error:F6} m; contacts={check.Rays}; missing/mismatched={check.Misses}; nearest-face changes={check.NearestChanges}; renderer={renderer}; valid={valid}");
            failed |= !valid; files++; rays += check.Rays;
            File.WriteAllBytes(Path.Combine(output, Path.GetFileName(path)), data);
            if (!bytes.SequenceEqual(File.ReadAllBytes(path))) throw new Exception("Repair changed an input file.");
        }
        Console.WriteLine($"Repaired {files} YBNs without further rotation; {rays} contact probes; {(failed || files == 0 ? "FAILED" : "PASS")}");
        return failed || files == 0 ? 1 : 0;
    }

    private static int Diagnose(string input, string converted)
    {
        var centre = MapBatch.GetCentreAsync(Directory.GetFiles(input), CancellationToken.None).GetAwaiter().GetResult() ?? Vector3.Zero;
        var rotation = new MapTransform(Vector3.Zero, 90, centre);
        foreach (var path in Directory.GetFiles(input, "*.ybn").Order())
        {
            var other = Path.Combine(converted, Path.GetFileName(path));
            if (!File.Exists(other)) continue;
            var a = Load(File.ReadAllBytes(path)); var b = Load(File.ReadAllBytes(other));
            var source = Meshes(a.Bounds, Matrix.Identity).ToArray(); var output = Meshes(b.Bounds, Matrix.Identity).ToArray();
            Console.WriteLine($"{Path.GetFileName(path)}: meshes={source.Length}->{output.Length}; verts={source.Sum(m => m.Mesh.Vertices.Length)}->{output.Sum(m => m.Mesh.Vertices.Length)}; polys={source.Sum(m => m.Mesh.Polygons.Length)}->{output.Sum(m => m.Mesh.Polygons.Length)}");
            Console.WriteLine($"  root VFT={a.Bounds.FileVFT}->{b.Bounds.FileVFT}; bounds={b.Bounds.BoxMin}..{b.Bounds.BoxMax}; sphere={b.Bounds.SphereCenter}/{b.Bounds.SphereRadius}; encloses={EnclosesChildren(b.Bounds)}");
            for (int m = 0; m < Math.Min(source.Length, output.Length); m++)
            {
                var (oldMesh, oldPlacement) = source[m]; var (newMesh, newPlacement) = output[m];
                Console.WriteLine($"  mesh {m} placement={newPlacement}; renderer scale={newPlacement.ScaleVector}; center={newMesh.CenterGeom}; local bounds={newMesh.BoxMin}..{newMesh.BoxMax}; quantum={newMesh.Quantum}; flags={Convert.ToHexString(MetaTypes.ConvertArrayToBytes(new[] {newMesh.CompositeFlags1,newMesh.CompositeFlags2}))}");
                var polys = oldMesh.Polygons.OfType<BoundPolygonTriangle>().ToArray();
                int tests = 0, localHits = 0, rootHits = 0;
                foreach (var tri in polys.Where((p, i) => i % Math.Max(1, polys.Length / 256) == 0))
                {
                    var v1 = oldMesh.Vertices[tri.vertIndex1] + oldMesh.CenterGeom;
                    var v2 = oldMesh.Vertices[tri.vertIndex2] + oldMesh.CenterGeom;
                    var v3 = oldMesh.Vertices[tri.vertIndex3] + oldMesh.CenterGeom;
                    var n = Vector3.Cross(v2-v1, v3-v1); if (n.LengthSquared() < 1e-6f) continue; n.Normalize();
                    var ray = new Ray((v1+v2+v3)/3+n*0.25f, -n);
                    if (!LocalHit(oldMesh, ray).Hit) continue;
                    var world = new Ray(rotation.Position(Vector3.TransformCoordinate(ray.Position, oldPlacement)), rotation.Direction(Vector3.TransformNormal(ray.Direction, oldPlacement)));
                    var inverse = Matrix.Invert(newPlacement);
                    var local = new Ray(Vector3.TransformCoordinate(world.Position, inverse), Vector3.TransformNormal(world.Direction, inverse));
                    tests++;
                    if (LocalHit(newMesh, local).Hit) localHits++;
                    if (b.Bounds.RayIntersect(ref world).Hit) rootHits++;
                }
                Console.WriteLine($"  probes={tests}; persisted local hits={localHits}; unmodified loaded hierarchy hits={rootHits}");
            }
        }
        return 0;
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

    private static (float Error, int Rays, int Misses, bool Metadata, int NearestChanges) Compare(YbnFile source, YbnFile loaded, MapTransform transform)
    {
        var before = Meshes(source.Bounds, Matrix.Identity).ToArray();
        var after = Meshes(loaded.Bounds, Matrix.Identity).ToArray();
        if (before.Length != after.Length) return (float.PositiveInfinity, 0, 0, false, 0);
        float error = 0; int rays = 0, misses = 0, nearestChanges = 0; bool metadata = true;
        for (int m = 0; m < before.Length; m++)
        {
            var (a, placementA) = before[m]; var (b, placementB) = after[m];
            if (a.Vertices.Length != b.Vertices.Length) return (float.PositiveInfinity, 0, 0, false, 0);
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
                var ray = new Ray((p1 + p2 + p3) / 3 + normal * 0.173205f, -normal);
                var originalHit = LocalHit(a, ray);
                if (!originalHit.Hit) continue;
                var worldRay = new Ray(transform.Position(Vector3.TransformCoordinate(ray.Position, placementA)),
                    transform.Direction(Vector3.TransformNormal(ray.Direction, placementA)));
                var inverse = Matrix.Invert(placementB);
                var localRay = new Ray(Vector3.TransformCoordinate(worldRay.Position, inverse), Vector3.TransformNormal(worldRay.Direction, inverse));
                var sourceInverse = Matrix.Invert(placementA * transform.Matrix);
                var sourceRay = new Ray(Vector3.TransformCoordinate(worldRay.Position, sourceInverse), Vector3.TransformNormal(worldRay.Direction, sourceInverse));
                var expectedHit = LocalHit(a, sourceRay);
                var movedHit = LocalHit(b, localRay);
                // This is the ordinary loaded CodeWalker hierarchy. Do not clear any
                // transforms for this check: that hid the viewer/query failure before.
                var hierarchyHit = loaded.Bounds.RayIntersect(ref worldRay);
                rays++;
                // Baking introduces the file's normal short-vertex quantization. A
                // grazing ray may select another overlapping face after a millimetre
                // vertex change; report this separately from missing collision or a
                // hierarchy lookup failure. Every vertex is checked above as well.
                if (expectedHit.Hit && movedHit.Hit && MathF.Abs(expectedHit.HitDist - movedHit.HitDist) > 0.01f) nearestChanges++;
                if (Vector3.Distance(ray.Position, sourceRay.Position) > 0.005f || Vector3.Distance(ray.Direction, sourceRay.Direction) > 0.0001f ||
                    movedHit.Hit != expectedHit.Hit ||
                    (movedHit.Hit && (!hierarchyHit.Hit || hierarchyHit.HitDist > movedHit.HitDist + 0.01f)))
                {
                    misses++;
                    Console.WriteLine($"    ray {i}: {expectedHit.Hit}/{expectedHit.HitDist} -> {movedHit.Hit}/{movedHit.HitDist}, hierarchy={hierarchyHit.Hit}/{hierarchyHit.HitDist}, source drift={Vector3.Distance(ray.Position, sourceRay.Position)}");
                }
            }
        }
        return (error, rays, misses, metadata, nearestChanges);
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
