using CodeWalker.GameFiles;
using SharpDX;
using Ymap_Ybn_Mover;

internal static class Program
{
    private static readonly MapTransform Rotation = new(new Vector3(20, -30, 7), 37);
    private static int failures;

    [STAThread]
    private static int Main(string[] args)
    {
        if (args.Length != 0) return AssetAudit.Run(args);
        Test("Z rotation around the world origin preserves height and precedes the offset", () =>
        {
            var transform = new MapTransform(new Vector3(10, 20, 30), 90);
            Near(transform.Position(new Vector3(2, 0, 7)), new Vector3(10, 22, 37));
            Near(transform.Direction(Vector3.UnitX), Vector3.UnitY);
            Near(transform.Position(Vector3.Zero), transform.Offset);
            Near(new MapTransform(Vector3.Zero, -37).Position(new Vector3(4, -5, 9)).Z, 9);
        });
        Test("quarter turns are exact and reversible at map-sized coordinates", () =>
        {
            foreach (float angle in new[] { -270f, -180f, -90f, 90f, 180f, 270f })
            {
                var transform = new MapTransform(Vector3.Zero, angle);
                var point = new Vector3(-2251.4854f, -623.3439f, 2.8722615f);
                var restored = Vector3.TransformCoordinate(transform.Position(point), Matrix.Invert(transform.Matrix));
                Check(restored == point, "quarter turn introduces coordinate drift");
                Near(transform.Matrix.Determinant(), 1, 0);
            }
        });
        Test("orientation composition with a tilted entity", () =>
        {
            var old = Quaternion.RotationYawPitchRoll(0.4f, -0.7f, 0.2f);
            var actual = Vector3.Transform(Vector3.UnitX, Rotation.Orientation(old));
            Near(actual, Rotation.Direction(Vector3.Transform(Vector3.UnitX, old)));
        });
        Test("rotated bounds enclose every corner", () =>
        {
            var min = new Vector3(-2, -5, -1);
            var max = new Vector3(10, 3, 6);
            var bounds = Rotation.Bounds(min, max);
            foreach (var corner in new BoundingBox(min, max).GetCorners())
            {
                var p = Rotation.Position(corner);
                Check(p.X >= bounds.Min.X - 0.001 && p.Y >= bounds.Min.Y - 0.001 && p.Z >= bounds.Min.Z - 0.001 &&
                    p.X <= bounds.Max.X + 0.001 && p.Y <= bounds.Max.Y + 0.001 && p.Z <= bounds.Max.Z + 0.001, "corner outside transformed bounds");
            }
        });
        Test("YMAP translation is applied once", () =>
        {
            var map = MapFixture();
            var original = map.AllEntities[0].Position;
            var move = new MapTransform(new Vector3(4, 5, 6), 0);
            var loaded = ConvertMap(map, move);
            Near(loaded.AllEntities[0].Position, original + move.Offset);
        });
        Test("normal and MLO entity orientations survive rotation", () =>
        {
            var map = MapFixture();
            var def = new CMloInstanceDef { CEntityDef = EntityDefinition(new Vector3(3, 4, 5)) };
            var mlo = new YmapEntityDef(map, 1, ref def);
            map.AddEntity(mlo);
            var positions = map.AllEntities.Select(e => Rotation.Position(e.Position)).ToArray();
            var directions = map.AllEntities.Select(e => Rotation.Direction(Vector3.Transform(Vector3.UnitX, e.Orientation))).ToArray();
            var loaded = ConvertMap(map, Rotation);
            for (int i = 0; i < positions.Length; i++)
            {
                Near(loaded.AllEntities[i].Position, positions[i]);
                Near(Vector3.Transform(Vector3.UnitX, loaded.AllEntities[i].Orientation), directions[i]);
            }
            Check(loaded.AllEntities.Any(e => e.IsMlo), "MLO instance lost");
        });
        Test("car generators preserve heading magnitude", () =>
        {
            var map = MapFixture();
            map.CarGenerators = new[] { new YmapCarGen(map, new CCarGen { position = new Vector3(1, 2, 3), orientX = 2, orientY = -3, perpendicularLength = 4 }) };
            var loaded = ConvertMap(map, Rotation);
            Near(loaded.CarGenerators[0].Position, Rotation.Position(new Vector3(1, 2, 3)));
            Near(new Vector3(loaded.CarGenerators[0].CCarGen.orientX, loaded.CarGenerators[0].CCarGen.orientY, 0), Rotation.Direction(new Vector3(2, -3, 0)));
        });
        Test("LOD light arrays survive standalone resaves and rotation", () =>
        {
            var map = MapFixture();
            map.LODLights = LightFixture(map);
            // Supply editor objects for the initial fixture save, then reload raw arrays.
            GameFileTransformer.TransformYmap(map, new MapTransform(Vector3.Zero, 0));
            var input = map.Save();
            var loaded = new YmapFile();
            loaded.Load(FileProcessor.ConvertFile("lights.YMAP", input, Rotation));
            Check(loaded.LODLights.direction.Length == 1, "light lost");
            Near(loaded.LODLights.direction[0].ToVector3(), Rotation.Direction(Vector3.UnitX));
            Check(loaded.LODLights.hash[0] == 123 && loaded.LODLights.falloff[0] == 15 && loaded.LODLights.coronaIntensity[0] == 20, "light metadata changed");
            loaded.Load(FileProcessor.ConvertFile("lights.ymap", input, new MapTransform(Vector3.Zero, 0)));
            Near(loaded.LODLights.direction[0].ToVector3(), Vector3.UnitX);
        });
        Test("grass packed positions and normals survive rotation", () =>
        {
            var map = MapFixture();
            var min = new Vector3(1, 2, 3);
            var max = new Vector3(12, 8, 6);
            var instance = new rage__fwGrassInstanceListDef__InstanceData
            {
                Position = new ArrayOfUshorts3 { u0 = 12000, u1 = 46000, u2 = 33000 }, NormalX = 190, NormalY = 80,
                Ao = 45, Scale = 30, Color = new ArrayOfBytes3 { b0 = 10, b1 = 20, b2 = 30 }
            };
            map.GrassInstanceBatches = new[] { new YmapGrassInstanceBatch { Ymap = map, AABBMin = min, AABBMax = max,
                Batch = new rage__fwGrassInstanceListDef { archetypeName = 333 }, Instances = new[] { instance } } };
            var position = min + (max - min) * new Vector3(12000, 46000, 33000) * 0.00001525878f;
            var loaded = ConvertMap(map, Rotation);
            var batch = loaded.GrassInstanceBatches[0];
            var packed = batch.Instances[0].Position;
            Near(batch.AABBMin + (batch.AABBMax - batch.AABBMin) * new Vector3(packed.u0, packed.u1, packed.u2) * 0.00001525878f, Rotation.Position(position), 0.002f);
            var normal = Rotation.Direction(new Vector3(190 / 255f * 2 - 1, 80 / 255f * 2 - 1, 0));
            Near(new Vector3(batch.Instances[0].NormalX / 255f * 2 - 1, batch.Instances[0].NormalY / 255f * 2 - 1, 0), normal, 0.012f);
            Check(batch.Instances[0].Ao == 45 && batch.Instances[0].Scale == 30, "grass attributes changed");
        });
        Test("distant lights, timecycle volumes and occlusion meshes move", () =>
        {
            var map = MapFixture();
            map.DistantLODLights = new YmapDistantLODLights { Ymap = map, positions = new[] { new MetaVECTOR3(new Vector3(3, 4, 5)) }, colours = new uint[] { 123 } };
            map.CTimeCycleModifiers = new[] { new CTimeCycleModifier { minExtents = Vector3.Zero, maxExtents = Vector3.One, percentage = 33, range = 3 } };
            var model = new YmapOccludeModel(map, new OccludeModel()) { Vertices = new[] { Vector3.Zero, Vector3.UnitX, Vector3.UnitY }, Indices = new byte[] { 0, 1, 2 } };
            model.BuildTriangles();
            map.OccludeModels = new[] { model };
            var loaded = ConvertMap(map, Rotation);
            Near(loaded.DistantLODLights.positions[0].ToVector3(), Rotation.Position(new Vector3(3, 4, 5)));
            var bounds = Rotation.Bounds(Vector3.Zero, Vector3.One);
            Near(loaded.CTimeCycleModifiers[0].minExtents, bounds.Min);
            Near(loaded.CTimeCycleModifiers[0].maxExtents, bounds.Max);
            Near(loaded.OccludeModels[0].Triangles[0].Corner1, Rotation.Position(Vector3.Zero));
        });
        Test("unsupported YMAP sections are refused", () =>
        {
            var map = MapFixture();
            map._CMapData.containerLods = new Array_Structure { Count1 = 1 };
            Throws<NotSupportedException>(() => GameFileTransformer.TransformYmap(map, Rotation));
        });
        Test("occluders beyond packed coordinate range are refused", () =>
        {
            var map = MapFixture();
            map.BoxOccluders = new[] { new YmapBoxOccluder(map, new BoxOccluder { iLength = 4, iWidth = 4, iHeight = 4 }) };
            Throws<NotSupportedException>(() => GameFileTransformer.TransformYmap(map, new MapTransform(new Vector3(9000, 0, 0), 0)));
        });
        Test("incomplete cloth collision format is refused", () =>
        {
            var ybn = new YbnFile { Bounds = Primitive(BoundsType.Cloth) };
            Throws<NotSupportedException>(() => GameFileTransformer.TransformYbn(ybn, Rotation));
        });
        foreach (var type in new[] { BoundsType.Sphere, BoundsType.Capsule, BoundsType.Box, BoundsType.Disc, BoundsType.Cylinder })
            Test($"standalone {type} collision survives save/reload", () =>
            {
                var bounds = Primitive(type);
                var input = RoundTrip(new YbnFile { Bounds = bounds });
                var loaded = ConvertBounds(input, Rotation);
                var child = ((BoundComposite)loaded.Bounds).Children.data_items[0];
                Check(child.Type == type, "shape type changed");
                Near(Vector3.TransformCoordinate(child.SphereCenter, child.Transform), Rotation.Position(input.Bounds.SphereCenter));
                Near(child.BoxMin, input.Bounds.BoxMin);
                Near(child.SphereRadius, input.Bounds.SphereRadius);
            });
        Test("geometry and GeometryBVH preserve local vertices and collision queries", () =>
        {
            foreach (var type in new[] { "Geometry", "GeometryBVH" })
            {
                var input = RoundTrip(GeometryFixture(type));
                var original = (BoundGeometry)input.Bounds;
                var loaded = ConvertBounds(input, Rotation);
                var child = (BoundGeometry)((BoundComposite)loaded.Bounds).Children.data_items[0];
                Near(child.CenterGeom, original.CenterGeom);
                for (int i = 0; i < original.Vertices.Length; i++)
                {
                    Near(child.Vertices[i], original.Vertices[i], 0.001f);
                    Near(Vector3.TransformCoordinate(child.Vertices[i] + child.CenterGeom, child.Transform), Rotation.Position(original.Vertices[i] + original.CenterGeom), 0.002f);
                }
                Check(child.Polygons.Select(p => p.Type).Order().SequenceEqual(original.Polygons.Select(p => p.Type).Order()), "polygon types changed");
                Near(child.BoxMin, original.BoxMin, 0.002f);
                Near(child.BoxMax, original.BoxMax, 0.002f);
                if (type == "GeometryBVH")
                {
                    // Sphere primitive inside the mesh; a translated ray must still hit after save.
                    var ray = new Ray(new Vector3(4, -2, 12), -Vector3.UnitZ);
                    var hit = input.Bounds.RayIntersect(ref ray);
                    Check(hit.Hit, "fixture ray missed");
                    var transformedRay = new Ray(Rotation.Position(ray.Position), Rotation.Direction(ray.Direction));
                    // CodeWalker's editor queries use Transform in GetVertexPos even though
                    // a composite already inverse-transforms the ray. Query the persisted
                    // local mesh explicitly, as the game's collision hierarchy does.
                    var localRay = new Ray(Vector3.TransformCoordinate(transformedRay.Position, child.TransformInv),
                        Vector3.TransformNormal(transformedRay.Direction, child.TransformInv));
                    var placement = child.Transform;
                    child.Transform = Matrix.Identity;
                    var movedHit = child.RayIntersect(ref localRay);
                    child.Transform = placement;
                    Check(movedHit.Hit, "rotated serialized collision ray missed");
                    Near(movedHit.HitDist, hit.HitDist, 0.01f);
                }
            }
        });
        Test("composite BVH with null slots and nested transforms", () =>
        {
            var root = Composite(Enumerable.Range(0, 7).Select(i => i == 3 ? null! : Primitive(BoundsType.Box)).ToArray());
            for (int i = 0; i < root.Children.data_items.Length; i++)
                if (root.Children.data_items[i] is { } child) SetTransform(child, Matrix.RotationZ(0.2f) * Matrix.Translation(i * 7, i * -2, 0));
            var nested = Composite(new[] { Primitive(BoundsType.Sphere) });
            SetTransform(nested.Children.data_items[0], Matrix.Translation(3, 4, 5));
            SetTransform(nested, Matrix.RotationZ(-0.3f) * Matrix.Translation(50, 10, 3));
            root.Children.data_items[6] = nested;
            var input = RoundTrip(new YbnFile { Bounds = root });
            var original = (BoundComposite)input.Bounds;
            var loaded = ConvertBounds(input, Rotation);
            var moved = (BoundComposite)loaded.Bounds;
            Check(moved.BVH != null && moved.Children.data_items[3] == null, "composite BVH or null slot lost");
            var oldNested = (BoundComposite)original.Children.data_items[6];
            var newNested = (BoundComposite)moved.Children.data_items[6];
            var point = new Vector3(1, 2, 3);
            var expected = Rotation.Position(Vector3.TransformCoordinate(point, oldNested.Children.data_items[0].Transform * oldNested.Transform));
            Near(Vector3.TransformCoordinate(point, newNested.Children.data_items[0].Transform * newNested.Transform), expected);
            for (int i = 0; i < moved.Children.data_items.Length; i++)
                if (moved.Children.data_items[i] is { } child)
                {
                    if (child is not BoundComposite) Near(child.BoxMin, original.Children.data_items[i].BoxMin);
                    var world = MapTransform.TransformBounds(child.BoxMin, child.BoxMax, child.Transform);
                    Check(world.Min.X >= moved.BoxMin.X - 0.01 && world.Max.X <= moved.BoxMax.X + 0.01, "root bounds do not enclose child");
                }
        });
        Test("mesh collision keeps local BVH under an existing scaled placement", () =>
        {
            var mesh = GeometryFixture("GeometryBVH").Bounds;
            var placement = Matrix.Scaling(1.2f, 0.8f, 1.1f) * Matrix.RotationZ(-0.6f) * Matrix.Translation(35, -14, 3);
            SetTransform(mesh, placement);
            mesh.CompositeFlags1 = new BoundCompositeChildrenFlags { Flags1 = EBoundCompositeFlags.MAP_VEHICLE | EBoundCompositeFlags.PED, Flags2 = EBoundCompositeFlags.OBJECT };
            mesh.CompositeFlags2 = new BoundCompositeChildrenFlags { Flags1 = EBoundCompositeFlags.TEST_CAMERA, Flags2 = EBoundCompositeFlags.TEST_AI };
            var source = new YbnFile { Bounds = Composite(new[] { mesh }) };
            GameFileTransformer.TransformYbn(source, new MapTransform(Vector3.Zero, 0));
            var input = RoundTrip(source);
            // RoundTrip directly through CodeWalker would rebuild using the placement;
            // feed its existing bytes directly into the application's serializer.
            var stableBytes = source.Save();
            var loaded = new YbnFile();
            loaded.Load(FileProcessor.ConvertFile("mesh.ybn", stableBytes, Rotation));
            var actual = (BoundGeometry)((BoundComposite)loaded.Bounds).Children.data_items[0];
            var original = (BoundGeometry)((BoundComposite)input.Bounds).Children.data_items[0];
            Near(actual.BoxMin, original.BoxMin, 0.001f);
            Near(actual.BoxMax, original.BoxMax, 0.001f);
            for (int i = 0; i < actual.Vertices.Length; i++)
                Near(Vector3.TransformCoordinate(actual.Vertices[i] + actual.CenterGeom, actual.Transform),
                    Rotation.Position(Vector3.TransformCoordinate(original.Vertices[i] + original.CenterGeom, original.Transform)), 0.003f);
            Check(actual.CompositeFlags1.Equals(original.CompositeFlags1) && actual.CompositeFlags2.Equals(original.CompositeFlags2), "collision filters changed");
            Near(Vector3.TransformCoordinate(Vector3.TransformCoordinate(Vector3.One, actual.Transform), actual.TransformInv), Vector3.One, 0.001f);
        });
        Test("off-center and unused mesh vertices survive packing, including shrunk vertices", () =>
        {
            foreach (var type in new[] { "Geometry", "GeometryBVH" })
            {
                var input = RoundTrip(GeometryFixture(type));
                var mesh = (BoundGeometry)input.Bounds;
                // Preserve polygon positions while moving the encoding origin.
                // Half the polygon box width now cannot encode the raw vertices.
                var shift = new Vector3(500, -250, 80);
                mesh.CenterGeom -= shift;
                mesh.Vertices = mesh.Vertices.Select(v => v + shift).Concat(new[] { new Vector3(900, -600, 300) }).ToArray();
                if (type == "Geometry") mesh.VerticesShrunk = mesh.Vertices.Select(v => v - new Vector3(0.01f)).ToArray();
                var expected = mesh.Vertices.Select(v => v + mesh.CenterGeom).ToArray();
                var shrunk = mesh.VerticesShrunk?.ToArray();
                var transform = new MapTransform(Vector3.Zero, 90);
                GameFileTransformer.TransformYbn(input, transform);
                var actual = (BoundGeometry)((BoundComposite)RoundTrip(input).Bounds).Children.data_items[0];
                for (int i = 0; i < expected.Length; i++)
                    Near(Vector3.TransformCoordinate(actual.Vertices[i] + actual.CenterGeom, actual.Transform), transform.Position(expected[i]), 0.04f);
                if (shrunk != null)
                    for (int i = 0; i < shrunk.Length; i++) Near(actual.VerticesShrunk[i], shrunk[i], 0.04f);
            }
        });
        Test("resaving repairs stale outer bounds throughout a collision hierarchy", () =>
        {
            var nested = Composite(new[] { Primitive(BoundsType.Box) });
            SetTransform(nested.Children.data_items[0], Matrix.Translation(20, 30, 40));
            SetTransform(nested, Matrix.Translation(-5, 8, 2));
            var input = new YbnFile { Bounds = Composite(new[] { nested }) };
            GameFileTransformer.TransformYbn(input, new MapTransform(Vector3.Zero, 0));
            var loaded = (BoundComposite)RoundTrip(input).Bounds;
            Near(loaded.BoxMin, new Vector3(14, 36, 39));
            Near(loaded.BoxMax, new Vector3(16, 40, 45));
            Near(loaded.SphereCenter, new Vector3(15, 38, 42));
            Check(loaded.SphereRadius >= Vector3.Distance(loaded.BoxMin, loaded.SphereCenter), "sphere does not enclose repaired box");
        });
        Test("atomic replacement retains originals and creates distinct backups", () => WithTemporaryDirectory(async directory =>
        {
            string file = Path.Combine(directory, "map.ymap");
            byte[] original = { 1, 2, 3 };
            await File.WriteAllBytesAsync(file, original);
            await FileProcessor.ReplaceFileAsync(file, new byte[] { 4, 5 }, true, CancellationToken.None);
            Check(File.ReadAllBytes(file + ".bak").SequenceEqual(original), "original backup incorrect");
            await FileProcessor.ReplaceFileAsync(file, new byte[] { 6 }, true, CancellationToken.None);
            Check(Directory.GetFiles(directory, "*.bak").Length == 2, "backup overwritten");
            Check(Directory.GetFiles(directory, "*.tmp").Length == 0, "temporary file leaked");
        }));
        Test("YDR resaves without moving local drawable bounds", () =>
        {
            var drawable = DrawableFixture();
            var source = new YdrFile { Drawable = drawable };
            var loaded = new YdrFile();
            loaded.Load(FileProcessor.ConvertFile("model.YDR", source.Save(), Rotation));
            Near(loaded.Drawable.BoundingBoxMin, drawable.BoundingBoxMin);
            Near(loaded.Drawable.BoundingBoxMax, drawable.BoundingBoxMax);
        });
        Test("YDD resaves without moving local drawable bounds", () =>
        {
            var drawable = DrawableFixture();
            var source = new YddFile { DrawableDict = new DrawableDictionary { Hashes = new uint[] { 123 },
                Drawables = new ResourcePointerArray64<Drawable> { data_items = new[] { drawable } } } };
            var loaded = new YddFile();
            loaded.Load(FileProcessor.ConvertFile("dictionary.YDD", source.Save(), Rotation));
            Near(loaded.DrawableDict.Drawables.data_items[0].BoundingBoxMin, drawable.BoundingBoxMin);
            Check(loaded.DrawableDict.Hashes[0] == 123, "drawable hash changed");
        });
        Test("YFT resaves without moving local fragment bounds", () =>
        {
            var source = new YftFile { Fragment = new FragType { Name = "fragment", BoundingSphereCenter = new Vector3(3, 4, 5), BoundingSphereRadius = 10,
                Cloths = new ResourcePointerList64<EnvironmentCloth>(), LightAttributes = new ResourceSimpleList64<LightAttributes>() } };
            var loaded = new YftFile();
            loaded.Load(FileProcessor.ConvertFile("fragment.YFT", source.Save(), Rotation));
            Near(loaded.Fragment.BoundingSphereCenter, source.Fragment.BoundingSphereCenter);
            Near(loaded.Fragment.BoundingSphereRadius, source.Fragment.BoundingSphereRadius);
        });
        Test("cancellation and corrupt input leave the original untouched", () => WithTemporaryDirectory(async directory =>
        {
            var filename = Path.Combine(directory, "bad.YBN");
            byte[] original = { 1, 2, 3, 4 };
            await File.WriteAllBytesAsync(filename, original);
            using var cancellation = new CancellationTokenSource();
            cancellation.Cancel();
            try { await FileProcessor.ReplaceFileAsync(filename, new byte[] { 5 }, true, cancellation.Token); throw new Exception("cancel ignored"); }
            catch (OperationCanceledException) { }
            try { await FileProcessor.ProcessAsync(filename, Rotation, true, CancellationToken.None); throw new Exception("corrupt input accepted"); }
            catch (Exception ex) when (ex.Message != "corrupt input accepted") { }
            Check(File.ReadAllBytes(filename).SequenceEqual(original), "original changed");
            Check(Directory.GetFiles(directory).Length == 1, "failed conversion created extra files");
        }));
        Test("footer layout fits default, narrow and large-text windows", () =>
        {
            System.Windows.Forms.Control.CheckForIllegalCrossThreadCalls = true;
            using var form = new MainForm(checkForUpdates: false);
            Check(System.Windows.Forms.Control.CheckForIllegalCrossThreadCalls, "cross-thread checks disabled");
            // Create visible control handles off-screen without displaying a user-facing
            // window or running the startup update request. DrawToBitmap renders the form.
            form.ShowInTaskbar = false;
            form.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            form.Location = new System.Drawing.Point(-20000, -20000);
            form.Opacity = 0;
            form.Show();
            CheckUiLayout(form);
            string? path = Environment.GetEnvironmentVariable("YMAP_MOVER_RENDER_TEST");
            if (!string.IsNullOrEmpty(path)) RenderForm(form, path);
            form.Size = form.MinimumSize;
            CheckUiLayout(form);
            if (!string.IsNullOrEmpty(path)) RenderForm(form, Path.ChangeExtension(path, "minimum.png"));
            form.Font = new System.Drawing.Font(form.Font.FontFamily, 13.5f);
            form.ClientSize = new System.Drawing.Size(1560, 930);
            CheckUiLayout(form);
            if (!string.IsNullOrEmpty(path)) RenderForm(form, Path.ChangeExtension(path, "large-text.png"));
        });
        Console.WriteLine(failures == 0 ? "All regression checks passed." : $"{failures} regression checks failed.");
        return failures == 0 ? 0 : 1;
    }

    private static void CheckUiLayout(MainForm form)
    {
        form.PerformLayout();
        var inputsPanel = form.Controls.Find("transformPanel", true).Single();
        var actionsPanel = form.Controls.Find("actionsPanel", true).Single();
        var list = form.Controls.Find("mainList", true).Single();
        Check(list.Bottom <= inputsPanel.Top, "inputs overlap the file list");
        Check(inputsPanel.Bottom <= actionsPanel.Top, "inputs overlap the action row");
        foreach (System.Windows.Forms.Control group in inputsPanel.Controls)
        {
            var inputs = (System.Windows.Forms.TableLayoutPanel)group.Controls[0];
            inputs.PerformLayout();
            foreach (System.Windows.Forms.Control control in inputs.Controls)
            {
                Check(inputs.ClientRectangle.Contains(control.Bounds), "coordinate control clipped by its layout");
                if (control is System.Windows.Forms.NumericUpDown number)
                {
                    int requiredWidth = System.Windows.Forms.TextRenderer.MeasureText("-100000.000", number.Font).Width + 24;
                    Check(number.Width >= requiredWidth, "coordinate input too narrow for its allowed range");
                }
                foreach (System.Windows.Forms.Control other in inputs.Controls)
                    if (control != other) Check(!control.Bounds.IntersectsWith(other.Bounds), "coordinate label/input overlap");
            }
        }
        foreach (System.Windows.Forms.Control control in actionsPanel.Controls)
            Check(actionsPanel.ClientRectangle.Contains(control.Bounds), "action control clipped by its layout");
        Check(form.Controls.Find("pivotXNumeric", true).Length == 0, "pivot control still present");
    }

    private static void RenderForm(MainForm form, string path)
    {
        using var bitmap = new System.Drawing.Bitmap(form.Width, form.Height);
        form.DrawToBitmap(bitmap, new System.Drawing.Rectangle(System.Drawing.Point.Empty, form.Size));
        bitmap.Save(path);
    }

    private static void Test(string name, Action test)
    {
        try { test(); Console.WriteLine("PASS " + name); }
        catch (Exception ex) { failures++; Console.WriteLine("FAIL " + name + ": " + ex); }
    }
    private static void Check(bool condition, string message) { if (!condition) throw new Exception(message); }
    private static void Near(Vector3 actual, Vector3 expected, float tolerance = 0.001f) => Check(Vector3.Distance(actual, expected) <= tolerance, $"expected {expected}, got {actual}");
    private static void Near(float actual, float expected, float tolerance = 0.001f) => Check(MathF.Abs(actual - expected) <= tolerance, $"expected {expected}, got {actual}");
    private static void Throws<T>(Action action) where T : Exception
    {
        try { action(); } catch (T) { return; }
        throw new Exception($"Expected {typeof(T).Name}");
    }
    private static void WithTemporaryDirectory(Func<string, Task> action)
    {
        var directory = Path.Combine(Path.GetTempPath(), "YmapMoverTests-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(directory);
        try { action(directory).GetAwaiter().GetResult(); }
        finally { Directory.Delete(directory, true); }
    }
    private static CEntityDef EntityDefinition(Vector3 position) => new()
    {
        archetypeName = 123, position = position, rotation = new Vector4(0, 0, 0, 1), scaleXY = 1, scaleZ = 1,
        parentIndex = -1, lodLevel = rage__eLodType.LODTYPES_DEPTH_ORPHANHD
    };
    private static YmapFile MapFixture()
    {
        var map = new YmapFile { Name = "regression.ymap" };
        var def = EntityDefinition(new Vector3(1, 2, 3));
        map.AddEntity(new YmapEntityDef(map, 0, ref def));
        map._CMapData.entitiesExtentsMin = new Vector3(-10);
        map._CMapData.entitiesExtentsMax = new Vector3(10);
        map._CMapData.streamingExtentsMin = new Vector3(-20);
        map._CMapData.streamingExtentsMax = new Vector3(20);
        return map;
    }
    private static YmapLODLights LightFixture(YmapFile map) => new()
    {
        Ymap = map, direction = new[] { new MetaVECTOR3(Vector3.UnitX) }, falloff = new float[] { 15 }, falloffExponent = new float[] { 2 },
        timeAndStateFlags = new uint[] { 5 }, hash = new uint[] { 123 }, coneInnerAngle = new byte[] { 10 }, coneOuterAngleOrCapExt = new byte[] { 15 }, coronaIntensity = new byte[] { 20 }
    };
    private static Drawable DrawableFixture() => new() { Name = "model", BoundingBoxMin = new Vector3(-1, -2, -3), BoundingBoxMax = new Vector3(4, 5, 6),
        LightAttributes = new ResourceSimpleList64<LightAttributes>() };
    private static YmapFile ConvertMap(YmapFile map, MapTransform transform)
    {
        var loaded = new YmapFile();
        loaded.Load(FileProcessor.ConvertFile("regression.YMAP", map.Save(), transform));
        return loaded;
    }
    private static YbnFile ConvertBounds(YbnFile input, MapTransform transform)
    {
        var loaded = new YbnFile();
        loaded.Load(FileProcessor.ConvertFile("regression.YBN", input.Save(), transform));
        return loaded;
    }
    private static YbnFile RoundTrip(YbnFile input)
    {
        var loaded = new YbnFile(); loaded.Load(input.Save()); return loaded;
    }
    private static Bounds Primitive(BoundsType type)
    {
        var bounds = Bounds.Create(type);
        bounds.Type = type;
        bounds.BoxMin = new Vector3(-1, -2, -3); bounds.BoxMax = new Vector3(1, 2, 3);
        bounds.SphereCenter = Vector3.Zero; bounds.SphereRadius = 3; bounds.Margin = 0.05f;
        bounds.MaterialIndex = 5;
        return bounds;
    }
    private static BoundComposite Composite(Bounds[] children)
    {
        var result = new BoundComposite { Type = BoundsType.Composite, Children = new ResourcePointerArray64<Bounds> { data_items = children },
            BoxMin = new Vector3(-100), BoxMax = new Vector3(100), SphereRadius = 200 };
        foreach (var child in children) if (child != null) child.Parent = result;
        return result;
    }
    private static void SetTransform(Bounds bounds, Matrix transform) { bounds.Transform = transform; bounds.TransformInv = Matrix.Invert(transform); }
    private static YbnFile GeometryFixture(string type) => XmlYbn.GetYbn($"""
        <BoundsFile><Bounds type="{type}">
        <BoxMin x="-3" y="-5" z="-2"/><BoxMax x="10" y="5" z="8"/>
        <SphereCenter x="4" y="-2" z="3"/><SphereRadius value="20"/><Margin value="0.04"/>
        <GeometryCenter x="4" y="-2" z="3"/>
        <Vertices>
        0, 0, 0
        3, 0, 0
        0, 2, 0
        5, 0, 0
        5, 0, 3
        -3, -2, -1
        -1, -2, -1
        -1, 2, 1
        -3, 2, 1
        -5, 0, -3
        </Vertices>
        <Polygons>
        <Triangle m="0" v1="0" v2="1" v3="2" f1="0" f2="0" f3="0"/>
        <Sphere m="0" v="0" radius="1"/>
        <Capsule m="0" v1="3" v2="4" radius="0.5"/>
        <Cylinder m="0" v1="3" v2="4" radius="0.3"/>
        <Box m="0" v1="5" v2="6" v3="7" v4="8"/>
        <Sphere m="0" v="9" radius="1"/>
        </Polygons></Bounds></BoundsFile>
        """);
}
