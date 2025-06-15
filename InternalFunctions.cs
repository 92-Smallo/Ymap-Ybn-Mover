using SharpDX;
using System.Globalization;
using Color = System.Drawing.Color;

namespace Internal_Functions
{
    public class MathFunctions
    {
        public static Vector3 ConvertToVec3(Vector4 vec4)
        {
            return new Vector3(vec4.X, vec4.Y, vec4.Z);
        }

        public static Vector3 GetCentre(Vector3 extentMin, Vector3 extentMax)
        {
            return new Vector3((extentMin.X + extentMax.X) / 2, (extentMin.Y + extentMax.Y) / 2, (extentMin.Z + extentMax.Z) / 2);
        }

        public static Vector3 CreateVectorFromStrings(string X, string Y, string Z)
        {
            return new Vector3(float.Parse(X, CultureInfo.InvariantCulture.NumberFormat), float.Parse(Y, CultureInfo.InvariantCulture.NumberFormat), float.Parse(Z, CultureInfo.InvariantCulture.NumberFormat));
        }

        public static (Vector3, Quaternion) RotateAndMove(Vector3 pivot, string zRotation, Quaternion orientation, Vector3 position)
        {
            Vector3 eulerAngle = CreateVectorFromStrings("0.0", "0.0", zRotation);
            Quaternion rotationQuaternion = Quaternion.RotationYawPitchRoll(MathUtil.DegreesToRadians(eulerAngle.Y), MathUtil.DegreesToRadians(eulerAngle.X), MathUtil.DegreesToRadians(eulerAngle.Z));
            Quaternion newRotation = rotationQuaternion * orientation;
            Vector3 newPosition = Vector3.TransformCoordinate(position - pivot, Matrix.RotationQuaternion(rotationQuaternion)) + pivot;

            return (newPosition, newRotation);
        }

        public static Matrix RotateMatrixByEulerAngles(Matrix initialMatrix, float pitch, float yaw, float roll)
        {
            Matrix rotationYaw = Matrix.RotationYawPitchRoll(MathUtil.DegreesToRadians(yaw), 0, 0);
            Matrix rotationPitch = Matrix.RotationYawPitchRoll(0, MathUtil.DegreesToRadians(pitch), 0);
            Matrix rotationRoll = Matrix.RotationYawPitchRoll(0, 0, MathUtil.DegreesToRadians(roll));
            Matrix combinedRotation = rotationYaw * rotationPitch * rotationRoll;

            return Matrix.Multiply(initialMatrix, combinedRotation);
        }

        public static Vector3 GetCentreFromList(List<Vector3> vectors)
        {
            if (vectors.Count == 0)
            {
                throw new ArgumentException("The list of vectors is empty.");
            }

            Vector3 sum = Vector3.Zero;

            foreach (Vector3 vector in vectors)
            {
                sum += vector;
            }

            return sum / vectors.Count;
        }
    }

    public class StringFunctions
    {
        public static bool DoesEntryExist(string filename, ListView mainList)
        {
            foreach (ListViewItem item in mainList.Items.OfType<ListViewItem>())
            {
                if (item.SubItems[1].Text == filename)
                    return true;
            }

            return false;
        }
    }

    public class OtherFunctions
    {
        public static void RemoveFilesOfType(ListView mainList, string groupName)
        {
            mainList.BeginUpdate();

            var targetGroup = mainList.Groups[groupName];
            if (targetGroup != null)
            {
                var itemsToRemove = new List<ListViewItem>();
                foreach (ListViewItem item in targetGroup.Items)
                {
                    itemsToRemove.Add(item);
                }

                foreach (var item in itemsToRemove)
                {
                    mainList.Items.Remove(item);
                }
            }

            mainList.EndUpdate();
        }
    }
}
