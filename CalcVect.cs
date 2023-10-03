using CodeWalker.GameFiles;
using Internal_Functions;
using SharpDX;

namespace Ymap_Ybn_Mover
{
    public partial class CalcVect : Form
    {
        private readonly MainForm mainForm;
        private Vector3 offsetVec = new();

        public CalcVect(MainForm ParentForm)
        {
            InitializeComponent();
            mainForm = ParentForm;
        }

        private void CentreButton_Click(object sender, EventArgs e)
        {
            List<Vector3> vectors = new();
            ListView.SelectedListViewItemCollection selectedFiles = mainForm.SelectedMaps;
            if (selectedFiles.Count == 0)
            {
                MessageBox.Show("You don't have any maps selected.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            foreach (ListViewItem item in selectedFiles)
            {
                YmapFile ymap = new();
                ymap.Load(File.ReadAllBytes(item.SubItems[1].Text));
                vectors.Add(MathFunctions.GetCentre(ymap.CMapData.entitiesExtentsMin, ymap.CMapData.entitiesExtentsMax));
            }

            Vector3 centerPoint = MathFunctions.GetCentreFromList(vectors);
            vector1.Text = centerPoint.X + ", " + centerPoint.Y + ", " + centerPoint.Z;
        }

        private void InputButton_Click(object sender, EventArgs e)
        {
            mainForm.XMove = (decimal)offsetVec.X;
            mainForm.YMove = (decimal)offsetVec.Y;
            mainForm.ZMove = (decimal)offsetVec.Z;
            Close();
        }

        private void CalculateButton_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(vector1.Text) || !string.IsNullOrEmpty(vector2.Text))
            {
                try
                {
                    string[] OldLocSplit = vector1.Text.Split(',');
                    string[] NewLocSplit = vector2.Text.Split(',');

                    offsetVec = new Vector3(float.Parse(NewLocSplit[0]), float.Parse(NewLocSplit[1]), float.Parse(NewLocSplit[2])) - new Vector3(float.Parse(OldLocSplit[0]), float.Parse(OldLocSplit[1]), float.Parse(OldLocSplit[2]));
                    newOffset.Text = offsetVec.X.ToString() + ", " + offsetVec.Y.ToString() + ", " + offsetVec.Z.ToString();
                }
                catch (Exception)
                {
                    MessageBox.Show("The vectors don't appear to be in the correct format.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void InvertButton_Click(object sender, EventArgs e) => (vector2.Text, vector1.Text) = (vector1.Text, vector2.Text);
    }
}
