using SharpDX;

namespace Ymap_Ybn_Mover;

/// <summary>Rotate around the world's Z axis, then translate.</summary>
public sealed class MapTransform
{
    private readonly Matrix rotationMatrix;
    public Vector3 Offset { get; }
    public Quaternion Rotation { get; }
    public Matrix Matrix { get; }
    public bool HasRotation { get; }
    public bool IsIdentity => !HasRotation && Offset == Vector3.Zero;

    public MapTransform(Vector3 offset, float degrees)
    {
        if (!IsFinite(offset) || !float.IsFinite(degrees))
            throw new ArgumentException("Offset and rotation must be finite numbers.");

        Offset = offset;
        degrees %= 360;
        HasRotation = degrees != 0;
        Rotation = HasRotation ? Quaternion.RotationAxis(Vector3.UnitZ, MathUtil.DegreesToRadians(degrees)) : Quaternion.Identity;
        // SharpDX uses row vectors: local * existingTransform * worldTransform.
        rotationMatrix = SharpDX.Matrix.RotationQuaternion(Rotation);
        // Exact quarter turns avoid introducing tiny shear/scale errors at map-sized
        // coordinates when a world ray or vertex is transformed back to local space.
        if (degrees % 90 == 0)
        {
            rotationMatrix.M11 = MathF.Round(rotationMatrix.M11);
            rotationMatrix.M12 = MathF.Round(rotationMatrix.M12);
            rotationMatrix.M21 = MathF.Round(rotationMatrix.M21);
            rotationMatrix.M22 = MathF.Round(rotationMatrix.M22);
        }
        Matrix = rotationMatrix * SharpDX.Matrix.Translation(offset);
    }

    public Vector3 Position(Vector3 position) => HasRotation
        ? Vector3.TransformCoordinate(position, rotationMatrix) + Offset
        : position + Offset;

    public Vector3 Direction(Vector3 direction) => HasRotation
        ? Vector3.TransformNormal(direction, rotationMatrix)
        : direction;

    public Quaternion Orientation(Quaternion orientation) => HasRotation
        ? Quaternion.Normalize(Rotation * orientation)
        : orientation;

    public (Vector3 Min, Vector3 Max) Bounds(Vector3 min, Vector3 max) => HasRotation
        ? TransformBounds(min, max, Matrix)
        : (min + Offset, max + Offset);

    public static (Vector3 Min, Vector3 Max) TransformBounds(Vector3 min, Vector3 max, Matrix matrix)
    {
        var resultMin = new Vector3(float.MaxValue);
        var resultMax = new Vector3(float.MinValue);
        // Transform all eight corners; transforming only min/max fails for rotations.
        for (int i = 0; i < 8; i++)
        {
            var corner = new Vector3((i & 1) == 0 ? min.X : max.X,
                (i & 2) == 0 ? min.Y : max.Y, (i & 4) == 0 ? min.Z : max.Z);
            corner = Vector3.TransformCoordinate(corner, matrix);
            resultMin = Vector3.Min(resultMin, corner);
            resultMax = Vector3.Max(resultMax, corner);
        }
        return (resultMin, resultMax);
    }

    private static bool IsFinite(Vector3 value) => float.IsFinite(value.X) && float.IsFinite(value.Y) && float.IsFinite(value.Z);
}
