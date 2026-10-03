using CodeWalker.GameFiles;
using SharpDX;

namespace Ymap_Ybn_Mover;

public static class GameFileTransformer
{
    // Matches CodeWalker's packed grass position conversion.
    private const float GrassPositionUnit = 0.00001525878f;

    public static void TransformYmap(YmapFile ymap, MapTransform transform)
    {
        if (ymap._CMapData.containerLods.Count1 != 0 || ymap._CMapData.instancedData.PropInstanceList.Count1 != 0)
            throw new NotSupportedException("CodeWalker cannot preserve container LODs or prop instance batches in this YMAP. The original has not been changed.");

        PrepareLodLights(ymap.LODLights, transform);
        if (transform.IsIdentity) return;

        if (ymap.AllEntities != null)
            foreach (var entity in ymap.AllEntities)
            {
                // MLO contents are local to their instance; moving the parent updates them.
                if (entity.MloParent != null) continue;
                var position = transform.Position(entity.Position);
                if (transform.HasRotation) entity.SetOrientation(transform.Orientation(entity.Orientation));
                entity.SetPosition(position);
            }

        if (ymap.CarGenerators != null)
            foreach (var car in ymap.CarGenerators)
            {
                car.SetPosition(transform.Position(car.Position));
                if (transform.HasRotation)
                {
                    // Rotate the stored heading vector without changing its length.
                    var heading = transform.Direction(new Vector3(car._CCarGen.orientX, car._CCarGen.orientY, 0));
                    car._CCarGen.orientX = heading.X;
                    car._CCarGen.orientY = heading.Y;
                    car.CalcOrientation();
                }
            }

        if (ymap.DistantLODLights?.positions != null)
            for (int i = 0; i < ymap.DistantLODLights.positions.Length; i++)
                ymap.DistantLODLights.positions[i] = new MetaVECTOR3(transform.Position(ymap.DistantLODLights.positions[i].ToVector3()));

        if (ymap.GrassInstanceBatches != null)
            foreach (var batch in ymap.GrassInstanceBatches) TransformGrass(batch, transform);

        if (ymap.CTimeCycleModifiers != null)
            for (int i = 0; i < ymap.CTimeCycleModifiers.Length; i++)
            {
                var modifier = ymap.CTimeCycleModifiers[i];
                (modifier.minExtents, modifier.maxExtents) = transform.Bounds(modifier.minExtents, modifier.maxExtents);
                ymap.CTimeCycleModifiers[i] = modifier;
                if (ymap.TimeCycleModifiers != null && i < ymap.TimeCycleModifiers.Length)
                {
                    ymap.TimeCycleModifiers[i].CTimeCycleModifier = modifier;
                    ymap.TimeCycleModifiers[i].BBMin = modifier.minExtents;
                    ymap.TimeCycleModifiers[i].BBMax = modifier.maxExtents;
                }
            }

        if (ymap.BoxOccluders != null)
            foreach (var box in ymap.BoxOccluders)
            {
                box.Position = transform.Position(box.Position);
                // The file encodes centers as signed shorts in quarter-meter units.
                if (!FitsOccluderCoordinate(box.Position.X) || !FitsOccluderCoordinate(box.Position.Y) || !FitsOccluderCoordinate(box.Position.Z))
                    throw new NotSupportedException("A moved box occluder exceeds the format's coordinate range (about +/-8192 m).");
                if (transform.HasRotation) box.Orientation = transform.Orientation(box.Orientation);
            }

        if (ymap.OccludeModels != null)
            foreach (var model in ymap.OccludeModels)
                if (model.Triangles != null)
                    foreach (var triangle in model.Triangles)
                    {
                        triangle.Corner1 = transform.Position(triangle.Corner1);
                        triangle.Corner2 = transform.Position(triangle.Corner2);
                        triangle.Corner3 = transform.Position(triangle.Corner3);
                    }

        (ymap._CMapData.entitiesExtentsMin, ymap._CMapData.entitiesExtentsMax) =
            transform.Bounds(ymap._CMapData.entitiesExtentsMin, ymap._CMapData.entitiesExtentsMax);
        (ymap._CMapData.streamingExtentsMin, ymap._CMapData.streamingExtentsMax) =
            transform.Bounds(ymap._CMapData.streamingExtentsMin, ymap._CMapData.streamingExtentsMax);
    }

    public static void TransformYbn(YbnFile ybn, MapTransform transform)
    {
        var root = ybn.Bounds ?? throw new InvalidDataException("The YBN contains no bounds.");
        root = LocalSpaceBoundBvh.Prepare(root);
        ybn.Bounds = root;
        RefreshCompositeBounds(root);
        if (transform.IsIdentity) return;

        if (root is not BoundComposite composite)
        {
            // Root Bounds.Transform is not serialized. A composite supplies a persisted
            // child transform, even for standalone sphere/capsule/geometry roots.
            composite = new BoundComposite
            {
                Type = BoundsType.Composite,
                FileVFT = 1080212136,
                Children = new ResourcePointerArray64<Bounds> { data_items = new[] { root } },
                OwnerYbn = ybn,
                OwnerName = root.OwnerName,
                Unknown_3Ch = 1
            };
            root.Parent = composite;
            root.Transform = Matrix.Identity;
            root.TransformInv = Matrix.Identity;
            // Standalone roots have no composite filter. Permit all collision queries.
            var flags = new BoundCompositeChildrenFlags { Flags1 = (EBoundCompositeFlags)uint.MaxValue };
            root.CompositeFlags1 = flags;
            root.CompositeFlags2 = flags;
            ybn.Bounds = composite;
        }

        var children = composite.Children?.data_items;
        if (children == null || !children.Any(child => child != null))
            throw new InvalidDataException("The collision composite contains no shapes.");

        foreach (var child in children)
        {
            if (child == null) continue;
            // Do not recurse: nested geometry and shapes stay in their local frame.
            child.Transform *= transform.Matrix;
            child.TransformInv = Matrix.Invert(child.Transform);
        }
        RefreshCompositeBounds(composite);
        // CodeWalker Save rebuilds child transforms, bounding boxes and composite BVH.
    }

    private static void RefreshCompositeBounds(Bounds bounds)
    {
        if (bounds is not BoundComposite composite) return;
        var min = new Vector3(float.MaxValue);
        var max = new Vector3(float.MinValue);
        foreach (var child in composite.Children?.data_items ?? [])
        {
            if (child == null) continue;
            RefreshCompositeBounds(child);
            var box = MapTransform.TransformBounds(child.BoxMin, child.BoxMax, child.Transform);
            min = Vector3.Min(min, box.Min);
            max = Vector3.Max(max, box.Max);
        }
        if (min.X == float.MaxValue) return;
        composite.BoxMin = min;
        composite.BoxMax = max;
        composite.BoxCenter = (min + max) * 0.5f;
        composite.SphereCenter = composite.BoxCenter;
        composite.SphereRadius = (max - composite.BoxCenter).Length();
    }

    private static void PrepareLodLights(YmapLODLights? lights, MapTransform transform)
    {
        if (lights?.direction == null) return;
        int count = lights.direction.Length;
        // CodeWalker.Save clears raw light arrays unless the editor light objects exist.
        // These directions do not need the parent YMAP's distant-light positions.
        if (lights.falloff?.Length != count || lights.falloffExponent?.Length != count ||
            lights.timeAndStateFlags?.Length != count || lights.hash?.Length != count ||
            lights.coneInnerAngle?.Length != count || lights.coneOuterAngleOrCapExt?.Length != count || lights.coronaIntensity?.Length != count)
            throw new InvalidDataException("The LOD light arrays have inconsistent lengths.");

        lights.LodLights = Enumerable.Range(0, count).Select(i => new YmapLODLight
        {
            Direction = transform.Direction(lights.direction[i].ToVector3()),
            Falloff = lights.falloff[i], FalloffExponent = lights.falloffExponent[i],
            TimeAndStateFlags = lights.timeAndStateFlags[i], Hash = lights.hash[i],
            ConeInnerAngle = lights.coneInnerAngle[i], ConeOuterAngleOrCapExt = lights.coneOuterAngleOrCapExt[i],
            CoronaIntensity = lights.coronaIntensity[i], LodLights = lights, Index = i
        }).ToArray();
    }

    private static void TransformGrass(YmapGrassInstanceBatch batch, MapTransform transform)
    {
        var oldMin = batch.AABBMin;
        var oldSize = batch.AABBMax - oldMin;
        var bounds = transform.Bounds(batch.AABBMin, batch.AABBMax);
        var newSize = bounds.Max - bounds.Min;
        if (transform.HasRotation && batch.Instances != null)
            for (int i = 0; i < batch.Instances.Length; i++)
            {
                var instance = batch.Instances[i];
                var packed = instance.Position;
                var position = oldMin + oldSize * new Vector3(packed.u0, packed.u1, packed.u2) * GrassPositionUnit;
                var relative = transform.Position(position) - bounds.Min;
                instance.Position = new ArrayOfUshorts3
                {
                    u0 = PackGrassPosition(relative.X, newSize.X),
                    u1 = PackGrassPosition(relative.Y, newSize.Y),
                    u2 = PackGrassPosition(relative.Z, newSize.Z)
                };
                var normal = transform.Direction(new Vector3(instance.NormalX / 255f * 2 - 1, instance.NormalY / 255f * 2 - 1, 0));
                instance.NormalX = (byte)Math.Clamp(MathF.Round((normal.X + 1) * 0.5f * 255), 0, 255);
                instance.NormalY = (byte)Math.Clamp(MathF.Round((normal.Y + 1) * 0.5f * 255), 0, 255);
                batch.Instances[i] = instance;
            }
        batch.AABBMin = bounds.Min;
        batch.AABBMax = bounds.Max;
        batch.Position = (bounds.Min + bounds.Max) * 0.5f;
        batch.Radius = newSize.Length() * 0.5f;
    }

    private static ushort PackGrassPosition(float offset, float size) => size == 0 ? (ushort)0
        : (ushort)Math.Clamp(MathF.Round(offset / size / GrassPositionUnit), 0, ushort.MaxValue);

    private static bool FitsOccluderCoordinate(float coordinate) => MathF.Round(coordinate * 4) >= short.MinValue && MathF.Round(coordinate * 4) <= short.MaxValue;
}
