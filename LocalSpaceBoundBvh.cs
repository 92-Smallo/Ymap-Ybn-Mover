using CodeWalker.GameFiles;
using SharpDX;

namespace Ymap_Ybn_Mover;

/// <summary>
/// CodeWalker rebuilds BoundBVH using polygon positions, which include the editor's
/// Transform. Serialization must instead build that acceleration structure in local
/// space; the containing composite already persists the placement matrix.
/// </summary>
internal sealed class LocalSpaceBoundBvh : BoundBVH
{
    public static Bounds Prepare(Bounds bounds)
    {
        if (bounds.Type == BoundsType.Cloth)
            throw new NotSupportedException("CodeWalker's cloth-bound serializer is incomplete. This YBN has not been changed.");
        if (bounds is BoundComposite composite && composite.Children?.data_items != null)
        {
            var children = composite.Children.data_items;
            for (int i = 0; i < children.Length; i++)
                if (children[i] != null) children[i] = Prepare(children[i]);
        }
        if (bounds is not BoundBVH geometry) return bounds;
        if (geometry is LocalSpaceBoundBvh) return geometry;
        var result = new LocalSpaceBoundBvh();
        result.CopyFrom(geometry);
        result.Type = geometry.Type;
        result.Unknown_11h = geometry.Unknown_11h;
        result.Unknown_12h = geometry.Unknown_12h;
        result.Unknown_18h = geometry.Unknown_18h;
        result.Unknown_1Ch = geometry.Unknown_1Ch;
        result.Unknown_5Eh = geometry.Unknown_5Eh;
        result.Transform = geometry.Transform;
        result.TransformInv = geometry.TransformInv;
        result.CompositeFlags1 = geometry.CompositeFlags1;
        result.CompositeFlags2 = geometry.CompositeFlags2;
        result.Parent = geometry.Parent;
        result.Owner = geometry.Owner;
        result.OwnerYbn = geometry.OwnerYbn;
        result.OwnerName = geometry.OwnerName;
        result.FileVFT = geometry.FileVFT;
        result.FileUnknown = geometry.FileUnknown;
        result.FilePagesInfo = geometry.FilePagesInfo;
        result.Unknown_70h = geometry.Unknown_70h;
        result.Unknown_74h = geometry.Unknown_74h;
        result.VerticesShrunkPointer = geometry.VerticesShrunkPointer;
        result.Unknown_80h = geometry.Unknown_80h;
        result.Unknown_82h = geometry.Unknown_82h;
        result.VerticesShrunkCount = geometry.VerticesShrunkCount;
        result.PolygonsPointer = geometry.PolygonsPointer;
        result.Quantum = geometry.Quantum;
        result.Unknown_9Ch = geometry.Unknown_9Ch;
        result.CenterGeom = geometry.CenterGeom;
        result.Unknown_ACh = geometry.Unknown_ACh;
        result.VerticesPointer = geometry.VerticesPointer;
        result.VertexColoursPointer = geometry.VertexColoursPointer;
        result.OctantsPointer = geometry.OctantsPointer;
        result.OctantItemsPointer = geometry.OctantItemsPointer;
        result.VerticesCount = geometry.VerticesCount;
        result.PolygonsCount = geometry.PolygonsCount;
        result.Unknown_D8h = geometry.Unknown_D8h;
        result.Unknown_DCh = geometry.Unknown_DCh;
        result.Unknown_E0h = geometry.Unknown_E0h;
        result.Unknown_E4h = geometry.Unknown_E4h;
        result.Unknown_E8h = geometry.Unknown_E8h;
        result.Unknown_ECh = geometry.Unknown_ECh;
        result.MaterialsPointer = geometry.MaterialsPointer;
        result.MaterialColoursPointer = geometry.MaterialColoursPointer;
        result.Unknown_100h = geometry.Unknown_100h;
        result.Unknown_104h = geometry.Unknown_104h;
        result.Unknown_108h = geometry.Unknown_108h;
        result.Unknown_10Ch = geometry.Unknown_10Ch;
        result.Unknown_110h = geometry.Unknown_110h;
        result.Unknown_114h = geometry.Unknown_114h;
        result.PolygonMaterialIndicesPointer = geometry.PolygonMaterialIndicesPointer;
        result.MaterialsCount = geometry.MaterialsCount;
        result.MaterialColoursCount = geometry.MaterialColoursCount;
        result.Unknown_122h = geometry.Unknown_122h;
        result.Unknown_124h = geometry.Unknown_124h;
        result.Unknown_128h = geometry.Unknown_128h;
        result.Unknown_12Ch = geometry.Unknown_12Ch;
        result.VerticesShrunk = geometry.VerticesShrunk;
        result.Polygons = geometry.Polygons;
        result.Vertices = geometry.Vertices;
        result.VertexColours = geometry.VertexColours;
        result.Octants = geometry.Octants;
        result.Materials = geometry.Materials;
        result.MaterialColours = geometry.MaterialColours;
        result.PolygonMaterialIndices = geometry.PolygonMaterialIndices;
        result.BvhPointer = geometry.BvhPointer;
        result.Unknown_138h = geometry.Unknown_138h;
        result.Unknown_13Ch = geometry.Unknown_13Ch;
        result.Unknown_140h = geometry.Unknown_140h;
        result.Unknown_142h = geometry.Unknown_142h;
        result.Unknown_144h = geometry.Unknown_144h;
        result.Unknown_148h = geometry.Unknown_148h;
        result.Unknown_14Ch = geometry.Unknown_14Ch;
        result.BVH = geometry.BVH;
        if (result.Polygons != null)
            foreach (var polygon in result.Polygons)
                if (polygon != null) polygon.Owner = result;
        result.BuildLocalBvh();
        return result;
    }

    private void BuildLocalBvh()
    {
        var placement = Transform;
        try
        {
            Transform = Matrix.Identity;
            BuildBVH(false);
        }
        finally { Transform = placement; }
    }

    public override IResourceBlock[] GetReferences()
    {
        var placement = Transform;
        try
        {
            Transform = Matrix.Identity;
            return base.GetReferences();
        }
        finally { Transform = placement; }
    }
}
