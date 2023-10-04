using SharpDX;
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
        public static void UpdateStatus(string status, ListView mainList, ListViewItem item, Color colour)
        {
            mainList.BeginUpdate();
            item.SubItems[3].Text = status;
            item.SubItems[3].ForeColor = colour;
            mainList.EndUpdate();
        }

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
        public static void RemoveFilesOfType(IEnumerable<ListViewItem> items, string fileType)
        {
            foreach (ListViewItem item in items)
            {
                if (item.SubItems[1].Text.EndsWith(fileType))
                    item.Remove();
            }
        }
    }
}
