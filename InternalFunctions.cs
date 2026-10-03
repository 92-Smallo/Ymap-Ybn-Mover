using SharpDX;
using System.Globalization;

namespace Internal_Functions
{
    public static class MathFunctions
    {
        public static Vector3 GetCentre(Vector3 extentMin, Vector3 extentMax) => (extentMin + extentMax) * 0.5f;

        public static Vector3 CreateVectorFromStrings(string x, string y, string z) => new(
            float.Parse(x, NumberStyles.Float, CultureInfo.InvariantCulture),
            float.Parse(y, NumberStyles.Float, CultureInfo.InvariantCulture),
            float.Parse(z, NumberStyles.Float, CultureInfo.InvariantCulture));
    }

    public static class OtherFunctions
    {
        public static void RemoveFilesOfType(ListView mainList, string groupName)
        {
            mainList.BeginUpdate();
            try
            {
                var group = mainList.Groups[groupName];
                if (group == null) return;
                foreach (var item in group.Items.Cast<ListViewItem>().ToArray()) item.Remove();
            }
            finally { mainList.EndUpdate(); }
        }
    }
}
