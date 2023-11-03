using SharpDX;
using System.Xml;
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
            return new Vector3((extentMin.X + extentMax.X) / 2, (extentMin.Y + extentMax.Y) / 2,
                (extentMin.Z + extentMax.Z) / 2);
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

        public static void SendToNodeMover(string offset)
        {
            {
                var folderPath = "./Files"; // Change this to the folder containing your XML files

                var files = Directory.GetFiles(folderPath, "*.xml");

                foreach (var file in files)
                    try
                    {
                        var doc = new XmlDocument();
                        doc.Load(file);

                        // Select all elements with 'Position' attribute
                        var positions = doc.SelectNodes("//Position");

                        foreach (XmlNode position in positions) 
                        {
                            // Extract the 'x', 'y', and 'z' attributes
                            var xAttr = position.Attributes["x"]?.Value;
                            var yAttr = position.Attributes["y"]?.Value;
                            var zAttr = position.Attributes["z"]?.Value;

                            if (xAttr != null && yAttr != null && zAttr != null)
                            {
                                // Parse the values to float
                                var x = float.Parse(xAttr) + float.Parse(offset.Split(',')[0]);
                                var y = float.Parse(yAttr) + float.Parse(offset.Split(',')[1]);
                                var z = float.Parse(zAttr) + float.Parse(offset.Split(',')[2]);

                                // Update the attribute values
                                position.Attributes["x"].Value = x.ToString();
                                position.Attributes["y"].Value = y.ToString();
                                position.Attributes["z"].Value = z.ToString();
                            }
                        }

                        // Save the modified XML back to the file
                        doc.Save(file);

                        Console.WriteLine($"Processed: {file}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error processing file: {file} - {ex.Message}");
                    }

                // Parse the offset string
                var offsetParts = offset.Split(',');
                var offsetX = float.Parse(offsetParts[0]);
                var offsetY = float.Parse(offsetParts[1]);
                var offsetZ = float.Parse(offsetParts[2]);

                // Get a list of text files in the folder
                files = Directory.GetFiles(folderPath, "*.txt");

                foreach (var filePath in files)
                {
                    var lines = File.ReadAllLines(filePath);

                    for (var i = 1; i < lines.Length; i++)
                    {
                        var parts = lines[i].Split(' ');

                        if (parts.Length >= 3)
                        {
                            var x = float.Parse(parts[0]) + offsetX;
                            var y = float.Parse(parts[1]) + offsetY;
                            var z = float.Parse(parts[2]) + offsetZ;

                            // Add a '0' to the end and format the offset
                            lines[i] = $"{x} {y} {z} 0";
                        }
                    }

                    // Save the modified lines back to the file
                    File.WriteAllLines(filePath, lines);
                }

                MessageBox.Show("Car/Train Nodes Move Completed :)", "Node Move Completed", MessageBoxButtons.OK, MessageBoxIcon.Information);

            }
        }
    }
}