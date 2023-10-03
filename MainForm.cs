using Internal_Functions;
using CodeWalker.GameFiles;
using Color = System.Drawing.Color;
using Vector3 = SharpDX.Vector3;
using Vector4 = SharpDX.Vector4;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.Security.Policy;
using System.Xml.Linq;

namespace Ymap_Ybn_Mover
{
    public partial class MainForm : Form
    {
        public DateTime timerTime = DateTime.Now;
        public IDictionary<string, string> fileTypes = new Dictionary<string, string>() { { "YMAP Files", ".ymap" }, { "YBN Files", ".ybn" } };
        public bool interrupt = false;

        public ListView.SelectedListViewItemCollection SelectedMaps
        {
            get { return mainList.SelectedItems; }
        }

        public decimal XMove
        {
            get { return xMoveNumeric.Value; }
            set { xMoveNumeric.Value = value; }
        }

        public decimal YMove
        {
            get { return yMoveNumeric.Value; }
            set { yMoveNumeric.Value = value; }
        }

        public decimal ZMove
        {
            get { return zMoveNumeric.Value; }
            set { zMoveNumeric.Value = value; }
        }

        public MainForm()
        {
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;
            int mainListWidth = mainList.Width / 100;
            mainList.Columns[0].Width = mainListWidth * 20;
            mainList.Columns[1].Width = mainListWidth * 68;
            mainList.Columns[2].Width = mainListWidth * 10;
            mainList.Columns[3].Width = mainListWidth * 10;
            CheckForUpdate();
        }

        private static void CheckForUpdate(bool manualCheck = false)
        {
            string repoUrl = "https://api.github.com/repos/92-Smallo/Ymap-Ybn-Mover/releases/latest";

            using (HttpClient client = new())
            {
                client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (compatible; AcmeInc/1.0)");

                try
                {
                    HttpResponseMessage response = client.GetAsync(repoUrl).Result;
                    response.EnsureSuccessStatusCode();

                    string responseBody = response.Content.ReadAsStringAsync().Result;
                    dynamic release = JObject.Parse(responseBody);
                    string latestVersion = release.tag_name;
                    string localVersion = "1.0.1";

                    if (latestVersion != localVersion)
                    {
                        DialogResult result = MessageBox.Show(
                            $"A newer version ({latestVersion}) is available. Do you want to download it?\n\nChanges:\n{release.body}",
                            "Update Available",
                            MessageBoxButtons.YesNo,
                            MessageBoxIcon.Information);

                        if (result == DialogResult.Yes)
                        {
                            string url = release.assets[0].browser_download_url;
                            Process.Start("explorer.exe", url);
                        }
                    }
                    else
                    {
                        if (manualCheck)
                            MessageBox.Show("You have the latest version.", "Up to Date", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void AddFiles(string[] fileList)
        {
            mainList.BeginUpdate();
            foreach (string file in fileList)
            {
                foreach (ListViewGroup group in mainList.Groups)
                {
                    foreach (KeyValuePair<string, string> fileType in fileTypes)
                    {
                        if (group.Header == fileType.Key && file.EndsWith(fileType.Value))
                        {
                            CreateListEntry(fileType.Value, file, group);
                            break;
                        }
                    }
                }
            }
            mainList.EndUpdate();
        }

        private void CreateListEntry(string fileType, string file, ListViewGroup group)
        {
            long fileSize = new FileInfo(file).Length;
            ListViewItem tempViewItem = new(file, 0, group) { Text = Path.GetFileName(file).Replace(fileType, "") };
            tempViewItem.SubItems.Add(file);
            tempViewItem.SubItems.Add((fileSize / 1000).ToString() + " KB");
            tempViewItem.SubItems.Add(new ListViewItem.ListViewSubItem(tempViewItem, "Waiting", Color.Blue, mainList.BackColor, new Font(Font, FontStyle.Bold)));
            tempViewItem.UseItemStyleForSubItems = false;
            mainList.Items.Add(tempViewItem);
        }

        private void ExitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
            Application.Exit();
            Application.ExitThread();
        }

        private void AddFilesToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog(this) == DialogResult.OK)
                AddFiles(openFileDialog1.FileNames);
        }
        private void CalculateVectorDifferenceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CalcVect calculateVectorForm = new(this);
            calculateVectorForm.ShowDialog();
        }

        private void AddFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (mainFolderDialog.ShowDialog(this) == DialogResult.OK)
            {
                var options = new EnumerationOptions() { IgnoreInaccessible = true, RecurseSubdirectories = true };
                string[] ymapFiles = Directory.GetFiles(mainFolderDialog.SelectedPath, "*.ymap", options);
                string[] ybnFiles = Directory.GetFiles(mainFolderDialog.SelectedPath, "*.ybn", options);

                AddFiles(ymapFiles);
                AddFiles(ybnFiles);
            }
        }

        private void MainListDragEnter(object sender, DragEventArgs e)
        {
            if (e.Data == null)
                return;

            if (!e.Data.GetDataPresent(DataFormats.FileDrop))
                return;

            e.Effect = DragDropEffects.Copy;
        }

        private void MainListDragDrop(object sender, DragEventArgs e)
        {
            string[]? files = e.Data?.GetData(DataFormats.FileDrop) as string[];
            List<string> allFiles = new();

            for (int i = 0; i < files?.Length; i++)
            {
                string file = files[i];
                if (file.EndsWith(".ybn") || file.EndsWith(".ymap"))
                    allFiles.Add(file);
            }

            AddFiles(allFiles.ToArray());
        }

        public void TimerTick(object info)
        {
            DateTime currentTime = DateTime.Now;
            TimeSpan t = TimeSpan.FromMilliseconds((currentTime - timerTime).TotalMilliseconds);

            if (interrupt)
                timeElapsedLabel.Text = "Cancelling";
            else
                timeElapsedLabel.Text = string.Format("Time Elapsed: {0:D2}m:{1:D2}s:{2:D3}ms", t.Minutes, t.Seconds, t.Milliseconds);
        }

        private void ProcessFiles(IEnumerable<ListViewItem> items)
        {
            processAllButton.Enabled = false;
            processSelectedButton.Enabled = false;
            stopButton.Enabled = true;
            interrupt = false;
            timerTime = DateTime.Now;
            System.Threading.Timer watch = new(TimerTick, null, 0, 10);

            new Thread(() =>
            {
                Thread.CurrentThread.IsBackground = true;
                Vector3 moveVector = new((float)xMoveNumeric.Value, (float)yMoveNumeric.Value, (float)zMoveNumeric.Value);

                foreach (ListViewItem item in items)
                {
                    mainList.EnsureVisible(item.Index);
                    string filename = item.SubItems[1].Text;
                    StringFunctions.UpdateStatus("Processing", mainList, item, Color.Orange);

                    if (filename.EndsWith(".ymap"))
                    {
                        YmapFile ymap = new();
                        byte[] oldData;
                        oldData = File.ReadAllBytes(filename);

                        try
                        {
                            ymap.Load(oldData);
                            if (ymap.AllEntities != null)
                            {
                                foreach (YmapEntityDef yEnts in ymap.AllEntities)
                                    yEnts.SetPosition(yEnts.Position + moveVector);
                            }

                            if (ymap.CarGenerators != null)
                            {
                                foreach (YmapCarGen yEnts in ymap.CarGenerators)
                                    yEnts.SetPosition(yEnts.Position + moveVector);
                            }
                            if (ymap.DistantLODLights != null)
                            {
                                int lightCount = ymap._CMapData.DistantLODLightsSOA.position.Count1;
                                for (int i = 0; i < lightCount; i++)
                                {
                                    Vector3 vector3 = ymap.DistantLODLights.positions[i].ToVector3() + moveVector;
                                    MetaVECTOR3 metaVec = new() { x = vector3.X, y = vector3.Y, z = vector3.Z };
                                    ymap.DistantLODLights.positions[i] = metaVec;
                                }
                            }
                            if (ymap.GrassInstanceBatches != null)
                            {
                                foreach (YmapGrassInstanceBatch yEnts in ymap.GrassInstanceBatches)
                                {
                                    yEnts.Position += moveVector;
                                    yEnts.AABBMin += moveVector;
                                    yEnts.AABBMax += moveVector;
                                }
                            }

                            ymap._CMapData.streamingExtentsMax += moveVector;
                            ymap._CMapData.streamingExtentsMin += moveVector;
                            ymap._CMapData.entitiesExtentsMax += moveVector;
                            ymap._CMapData.entitiesExtentsMin += moveVector;

                            byte[] newData = ymap.Save();
                            File.WriteAllBytes(filename, newData);
                        }
                        catch (Exception)
                        {
                            StringFunctions.UpdateStatus("Error", mainList, item, Color.Red);
                        }

                        StringFunctions.UpdateStatus("Complete", mainList, item, Color.Green);
                    }

                    if (filename.EndsWith(".ybn"))
                    {
                        StringFunctions.UpdateStatus("File Loaded", mainList, item, Color.Red);

                        YbnFile ybn = new();
                        byte[] oldData;
                        oldData = File.ReadAllBytes(filename);

                        try
                        {
                            ybn.Load(oldData);

                            if (ybn.Bounds != null)
                            {
                                ybn.Bounds.BoxCenter += moveVector;
                                ybn.Bounds.BoxMax += moveVector;
                                ybn.Bounds.BoxMin += moveVector;
                                ybn.Bounds.SphereCenter += moveVector;

                                BoundComposite? boundcomp = ybn.Bounds as BoundComposite;
                                var compchilds = boundcomp?.Children?.data_items;
                                if (boundcomp?.BVH != null)
                                {
                                    Vector3 boundcompBBC = MathFunctions.ConvertToVec3(boundcomp.BVH.BoundingBoxCenter);
                                    Vector3 boundcompBBMax = MathFunctions.ConvertToVec3(boundcomp.BVH.BoundingBoxMax);
                                    Vector3 boundcompBBMin = MathFunctions.ConvertToVec3(boundcomp.BVH.BoundingBoxMin);
                                    boundcomp.BVH.BoundingBoxCenter = new Vector4(boundcompBBC + moveVector, boundcomp.BVH.BoundingBoxCenter.W);
                                    boundcomp.BVH.BoundingBoxMax = new Vector4(boundcompBBMax + moveVector, boundcomp.BVH.BoundingBoxMax.W);
                                    boundcomp.BVH.BoundingBoxMin = new Vector4(boundcompBBMin + moveVector, boundcomp.BVH.BoundingBoxMin.W);
                                }
                                if (compchilds != null)
                                {
                                    for (int i = 0; i < compchilds.Length; i++)
                                    {
                                        compchilds[i].BoxCenter += moveVector;
                                        compchilds[i].BoxMax += moveVector;
                                        compchilds[i].BoxMin += moveVector;
                                        compchilds[i].SphereCenter += moveVector;
                                        if (compchilds[i] is BoundBVH bgeom)
                                        {
                                            if (bgeom.BVH != null)
                                            {
                                                Vector3 bgeomBBC = MathFunctions.ConvertToVec3(bgeom.BVH.BoundingBoxCenter);
                                                Vector3 bgeomBBMax = MathFunctions.ConvertToVec3(bgeom.BVH.BoundingBoxMax);
                                                Vector3 bgeomBBMin = MathFunctions.ConvertToVec3(bgeom.BVH.BoundingBoxMin);
                                                bgeom.BVH.BoundingBoxCenter = new Vector4(bgeomBBC + moveVector, bgeom.BVH.BoundingBoxCenter.W);
                                                bgeom.BVH.BoundingBoxMax = new Vector4(bgeomBBMax + moveVector, bgeom.BVH.BoundingBoxMax.W);
                                                bgeom.BVH.BoundingBoxMin = new Vector4(bgeomBBMin + moveVector, bgeom.BVH.BoundingBoxMin.W);
                                            }
                                            bgeom.CenterGeom += moveVector;
                                        }
                                    }
                                }
                            }
                            byte[] newData = ybn.Save();
                            File.WriteAllBytes(filename, newData);
                        }
                        catch (Exception)
                        {
                            StringFunctions.UpdateStatus("Error", mainList, item, Color.Red);
                        }

                        StringFunctions.UpdateStatus("Complete", mainList, item, Color.Green);
                    }

                    if (interrupt)
                    {
                        StringFunctions.UpdateStatus("Cancelled", mainList, item, Color.Red);
                        processAllButton.Enabled = true;
                        processSelectedButton.Enabled = true;
                        stopButton.Enabled = false;
                        timeElapsedLabel.Text = "";
                        break;
                    }
                }
                watch.Dispose();
            }).Start();

            stopButton.Click += (s, e) => { interrupt = true; };
        }

        private void ClearFilesToolStripMenuItem_Click(object sender, EventArgs e) => mainList.Clear();
        private void ClearSelectedFilesToolStripMenuItem_Click(object sender, EventArgs e) => mainList.SelectedItems.Clear();
        private void ProcessAllButton_Click(object sender, EventArgs e) => ProcessFiles(mainList.Items.OfType<ListViewItem>());
        private void ProcessSelectedButton_Click(object sender, EventArgs e) => ProcessFiles(mainList.SelectedItems.OfType<ListViewItem>());
        private void AddFilesToolStripMenuItem_Click(object sender, EventArgs e) => mainFileDialog.ShowDialog();
        private void OpenFileDialog1_FileOk(object sender, EventArgs e) => AddFiles(mainFileDialog.FileNames);
        private void ClearAllYMAPsToolStripMenuItem_Click(object sender, EventArgs e) => OtherFunctions.RemoveFilesOfType(mainList.Items.OfType<ListViewItem>(), ".ymap");
        private void ClearAllYBNsToolStripMenuItem_Click(object sender, EventArgs e) => OtherFunctions.RemoveFilesOfType(mainList.Items.OfType<ListViewItem>(), ".ybn");
        private void CheckForUpdateToolStripMenuItem_Click(object sender, EventArgs e) => CheckForUpdate(true);
    }
}