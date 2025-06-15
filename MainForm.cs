using Internal_Functions;
using CodeWalker.GameFiles;
using Color = System.Drawing.Color;
using Vector3 = SharpDX.Vector3;
using Vector4 = SharpDX.Vector4;
using Newtonsoft.Json.Linq;
using System.Diagnostics;

namespace Ymap_Ybn_Mover
{
    public partial class MainForm : Form
    {
        public DateTime timerTime = DateTime.Now;
        public bool interrupt = false;
        public int totalFiles;
        public int currentFile;
        private Vector3 offsetVec = new();
        private readonly List<Control> aboutControls = new();
        private readonly List<Control> howToControls = new();
        private readonly List<Control> vecDiffControls = new();
        private CancellationTokenSource cts;

        public MainForm()
        {
            InitializeComponent();

            typeof(ListView).InvokeMember("DoubleBuffered",
        System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.SetProperty,
        null, mainList, new object[] { true });

            typeof(StatusStrip).InvokeMember("DoubleBuffered",
        System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.SetProperty,
        null, mainList, new object[] { true });

            CheckForIllegalCrossThreadCalls = false;
            int mainListWidth = mainList.Width / 100;
            mainList.Columns[0].Width = mainListWidth * 20;
            mainList.Columns[1].Width = mainListWidth * 68;
            mainList.Columns[2].Width = mainListWidth * 20;
            CheckForUpdate();

            aboutControls.AddRange(new List<Control>() { aboutGroupBox, aboutRichTextBox, closeAboutButton });
            howToControls.AddRange(new List<Control>() { howToUseCloseButton, howToUseGroupBox, howToUseRichTextBox });
            vecDiffControls.AddRange(new List<Control>() { vecDiffGroupBox, vecDiffCloseButton, CalculateButton, CentreButton, InputButton, InvertButton,
                                                         newOffset, vector1, vector2, CalculatedLabel, OGLocLabel, newLocLabel, InstructionsLabel });
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
                    string localVersion = "1.0.5";

                    if (latestVersion != localVersion)
                    {
                        DialogResult result = MessageBox.Show($"A newer version ({latestVersion}) is available. Do you want to download it?\n\nChanges:\n{release.body}", "Update Available", MessageBoxButtons.YesNo, MessageBoxIcon.Information);

                        if (result == DialogResult.Yes)
                        {
                            string url = release.assets[0].browser_download_url;
                            Process.Start("explorer.exe", url);
                            MessageBox.Show("New version downloaded, please close the program and overwrite your existing version with the new files.", "Update", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
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
            var fileTypes = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                { ".ymap", "YMAP Files" },
                { ".ybn", "YBN Files" },
                { ".ydr", "YDR Files" },
                { ".ydd", "YDD Files" },
                { ".yft", "YFT Files" }
            };

            var groupLookup = mainList.Groups.Cast<ListViewGroup>().ToDictionary(g => g.Header, g => g);

            var itemsToAdd = new List<ListViewItem>();

            mainList.BeginUpdate();
            foreach (string file in fileList)
            {
                string extension = Path.GetExtension(file);

                if (fileTypes.TryGetValue(extension, out string groupName))
                {
                    if (groupLookup.TryGetValue(groupName, out ListViewGroup group))
                    {
                        if (!StringFunctions.DoesEntryExist(file, mainList))
                        {
                            var item = CreateListEntry(extension, file, group);
                            itemsToAdd.Add(item);
                        }
                    }
                }
            }

            mainList.Items.AddRange(itemsToAdd.ToArray());
            mainList.EndUpdate();
        }


        private ListViewItem CreateListEntry(string fileType, string file, ListViewGroup group)
        {
            long fileSize = 0;
            try
            {
                fileSize = new FileInfo(file).Length;
            }
            catch (Exception) { }

            var tempViewItem = new ListViewItem(Path.GetFileNameWithoutExtension(file), 0, group)
            {
                SubItems = { file, $"{fileSize / 1000} KB", new ListViewItem.ListViewSubItem() { Text = "Waiting", ForeColor = Color.Blue, Font = new Font(Font, FontStyle.Bold) } },
                UseItemStyleForSubItems = false
            };

            return tempViewItem;
        }


        private void ExitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
            Application.Exit();
            Application.ExitThread();
        }

        private void InputButton_Click(object sender, EventArgs e)
        {
            xMoveNumeric.Value = (decimal)offsetVec.X;
            yMoveNumeric.Value = (decimal)offsetVec.Y;
            zMoveNumeric.Value = (decimal)offsetVec.Z;
        }

        private void CentreButton_Click(object sender, EventArgs e)
        {
            List<Vector3> vectors = new();
            ListView.SelectedListViewItemCollection selectedFiles = mainList.SelectedItems;
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

        private void AddFilesToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog(this) == DialogResult.OK)
                AddFiles(openFileDialog1.FileNames);
        }

        private async void AddFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (mainFolderDialog.ShowDialog(this) == DialogResult.OK)
            {
                timeElapsedLabel.Text = "";
                HashSet<string> fileExtensions = new(StringComparer.OrdinalIgnoreCase) { ".ymap", ".ybn", ".ydr", ".ydd", ".yft" };
                var options = new EnumerationOptions() { IgnoreInaccessible = true, RecurseSubdirectories = true };
                List<string> files = new();

                await Task.Run(() =>
                {
                    Parallel.ForEach(Directory.EnumerateFiles(mainFolderDialog.SelectedPath, "*", options), file =>
                    {
                        if (fileExtensions.Contains(Path.GetExtension(file)))
                        {
                            lock (files)
                            {
                                files.Add(file);
                            }
                        }
                    });
                });

                if (files.Count == 0)
                {
                    MessageBox.Show("No files found.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                AddFiles(files.ToArray());
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
            List<string> fileTypes = new() { ".ymap", ".ybn", ".ydr", ".ydd", ".yft" };
            string[]? files = e.Data?.GetData(DataFormats.FileDrop) as string[];
            List<string> allFiles = new();

            for (int i = 0; i < files?.Length; i++)
            {
                string file = files[i];
                foreach (string fileType in fileTypes)
                {
                    if (file.EndsWith(fileType))
                        allFiles.Add(file);
                }

            }

            AddFiles(allFiles.ToArray());
        }

        private void ClearFilesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mainList.BeginUpdate();
            timeElapsedLabel.Text = "";
            mainList.Items.Clear();
            mainList.EndUpdate();
        }

        private void ClearSelectedFilesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mainList.BeginUpdate();
            foreach (ListViewItem item in mainList.Items)
            {
                if (item.Selected)
                    item.Remove();
            }
            mainList.EndUpdate();
        }

        private static void ToggleControlVisibility(List<Control> controls, bool visible)
        {
            foreach (Control control in controls)
                control.Visible = visible;
        }

        public void TimerTick(object info)
        {
            DateTime currentTime = DateTime.Now;
            TimeSpan t = TimeSpan.FromMilliseconds((currentTime - timerTime).TotalMilliseconds);

            if (interrupt)
            {
                timeElapsedLabel.Text = string.Format("Stopped | Time Elapsed: {0:D2}m:{1:D2}s:{2:D3}ms | File {3} of {4}", t.Minutes, t.Seconds, t.Milliseconds, currentFile, totalFiles);
            }
            else
            {
                timeElapsedLabel.Text = string.Format("Time Elapsed: {0:D2}m:{1:D2}s:{2:D3}ms | File {3} of {4}", t.Minutes, t.Seconds, t.Milliseconds, currentFile, totalFiles);
            }
        }

        private async void ProcessFiles(IEnumerable<ListViewItem> items)
        {
            cts = new CancellationTokenSource();
            int fileCount = 0;
            processAllButton.Enabled = false;
            processSelectedButton.Enabled = false;
            stopButton.Enabled = true;
            timerTime = DateTime.Now;
            System.Threading.Timer watch = new(TimerTick, null, 0, 10);
            Vector3 moveVector = new((float)xMoveNumeric.Value, (float)yMoveNumeric.Value, (float)zMoveNumeric.Value);
            var itemsList = items.ToList();
            var tasks = new List<Task>();
            totalFiles = itemsList.Count;

            try
            {
                tasks = itemsList.Select(item => Task.Run(async () =>
                {
                    fileCount++;
                    currentFile = fileCount;

                    if (cts.Token.IsCancellationRequested)
                    {   
                        watch.Dispose();
                        return;
                    }

                    mainList.Invoke(() => mainList.EnsureVisible(item.Index));

                    string filename = item.SubItems[1].Text;

                    try
                    {
                        if (filename.EndsWith(".ymap"))
                        {
                            await ProcessYmapFileAsync(filename, moveVector, item);
                        }
                        else if (filename.EndsWith(".ybn"))
                        {
                            await ProcessYbnFileAsync(filename, moveVector, item);
                        }
                        else if (filename.EndsWith(".ydr") || filename.EndsWith(".ydd") || filename.EndsWith(".yft"))
                        {
                            await ProcessGenericFileAsync(filename, item);
                        }

                        item.Selected = false;
                    }
                    catch (Exception ex)
                    {
                        UpdateListViewItem(item, Color.Red, $"Error: {ex.Message}");
                        return;
                    }
                })).ToList();
            }
            finally
            {
                await Task.WhenAll(tasks);

                watch.Dispose();
                processAllButton.Enabled = true;
                processSelectedButton.Enabled = true;
                stopButton.Enabled = false;
            }
        }

        private async Task ProcessYmapFileAsync(string filename, Vector3 moveVector, ListViewItem item)
        {
            try
            {
                byte[] oldData = await File.ReadAllBytesAsync(filename);
                var ymap = new YmapFile();
                ymap.Load(oldData);

                if (ymap.AllEntities != null)
                {
                    foreach (YmapEntityDef yEnts in ymap.AllEntities)
                        yEnts.SetPosition(yEnts.Position + moveVector);
                }

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
                await File.WriteAllBytesAsync(filename, newData);
                UpdateListViewItem(item, Color.Green);
            }
            catch (Exception ex)
            {
                UpdateListViewItem(item, Color.Red, $"Error: {ex.Message}");
            }
        }

        private async Task ProcessYbnFileAsync(string filename, Vector3 moveVector, ListViewItem item)
        {
            try
            {
                byte[] oldData = await File.ReadAllBytesAsync(filename);
                var ybn = new YbnFile();
                ybn.Load(oldData);

                if (ybn.Bounds == null)
                {
                    UpdateListViewItem(item, Color.Red, "Error: Bounds or BoundsData is null.");
                    return;
                }

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

                for (int i = 0; i < boundcomp?.ChildrenBoundingBoxes.Length; i++)
                {
                    Vector3 boundChildMin = MathFunctions.ConvertToVec3(boundcomp.ChildrenBoundingBoxes[i].Min);
                    Vector3 boundChildMax = MathFunctions.ConvertToVec3(boundcomp.ChildrenBoundingBoxes[i].Max);
                    boundcomp.ChildrenBoundingBoxes[i].Min = new Vector4(boundChildMin + moveVector, boundcomp.ChildrenBoundingBoxes[i].Min.W);
                    boundcomp.ChildrenBoundingBoxes[i].Max = new Vector4(boundChildMax + moveVector, boundcomp.ChildrenBoundingBoxes[i].Max.W);
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

                byte[] newData = ybn.Save();
                await File.WriteAllBytesAsync(filename, newData);
                UpdateListViewItem(item, Color.Green);
            }
            catch (Exception ex)
            {
                UpdateListViewItem(item, Color.Red, $"Error: {ex.Message}");
            }
        }

        private async Task ProcessGenericFileAsync(string filename, ListViewItem item)
        {
            try
            {
                byte[] oldData = await File.ReadAllBytesAsync(filename);
                byte[] newData;

                if (filename.EndsWith(".ydr"))
                {
                    var ydr = new YdrFile();
                    RpfFile.LoadResourceFile(ydr, oldData, 165);
                    newData = ydr.Save();

                }
                else if (filename.EndsWith(".ydd"))
                {
                    var ydd = new YddFile();
                    RpfFile.LoadResourceFile(ydd, oldData, 165);
                    newData = ydd.Save();
                }
                else
                {
                    var yft = new YftFile();
                    RpfFile.LoadResourceFile(yft, oldData, 162);
                    newData = yft.Save();
                }

                await File.WriteAllBytesAsync(filename, newData);

                UpdateListViewItem(item, Color.Green);
            }
            catch (Exception ex)
            {
                UpdateListViewItem(item, Color.Red, $"Error: {ex.Message}");
            }
        }

        private void UpdateListViewItem(ListViewItem item, Color color, string toolTipText = "")
        {
            mainList.BeginUpdate();
            try
            {
                item.ForeColor = color;
                item.ToolTipText = toolTipText;
            }
            finally
            {
                mainList.EndUpdate();
            }
        }

        private void stopButton_Click(object sender, EventArgs e)
        {
            interrupt = true;
            cts?.Cancel();
        }

        private void ProcessAllButton_Click(object sender, EventArgs e) => ProcessFiles(mainList.Items.OfType<ListViewItem>());
        private void ProcessSelectedButton_Click(object sender, EventArgs e) => ProcessFiles(mainList.SelectedItems.OfType<ListViewItem>());
        private void AddFilesToolStripMenuItem_Click(object sender, EventArgs e) => mainFileDialog.ShowDialog();
        private void OpenFileDialog1_FileOk(object sender, EventArgs e) => AddFiles(mainFileDialog.FileNames);
        private void InvertButton_Click(object sender, EventArgs e) => (vector2.Text, vector1.Text) = (vector1.Text, vector2.Text);
        private void ClearAllYMAPsToolStripMenuItem_Click(object sender, EventArgs e) => OtherFunctions.RemoveFilesOfType(mainList, "ymapGroup");
        private void ClearAllYBNsToolStripMenuItem_Click(object sender, EventArgs e) => OtherFunctions.RemoveFilesOfType(mainList, "ybnGroup");
        private void ClearAllYDRsToolStripMenuItem_Click(object sender, EventArgs e) => OtherFunctions.RemoveFilesOfType(mainList, "ydrGroup");
        private void ClearAllYDDsToolStripMenuItem_Click(object sender, EventArgs e) => OtherFunctions.RemoveFilesOfType(mainList, "yddGroup");
        private void ClearAllYFTsToolStripMenuItem_Click(object sender, EventArgs e) => OtherFunctions.RemoveFilesOfType(mainList, "yftGroup");
        private void CheckForUpdateToolStripMenuItem_Click(object sender, EventArgs e) => CheckForUpdate(true);
        private void AboutToolStripMenuItem_Click(object sender, EventArgs e) => ToggleControlVisibility(aboutControls, true);
        private void CloseAboutButton_Click(object sender, EventArgs e) => ToggleControlVisibility(aboutControls, false);
        private void HowToUseToolStripMenuItem_Click(object sender, EventArgs e) => ToggleControlVisibility(howToControls, true);
        private void HowToUseCloseButton_Click(object sender, EventArgs e) => ToggleControlVisibility(howToControls, false);
        private void CalcVecDiffStripMenuItem_Click(object sender, EventArgs e) => ToggleControlVisibility(vecDiffControls, true);
        private void VecDiffCloseButton_Click(object sender, EventArgs e) => ToggleControlVisibility(vecDiffControls, false);
    }
}