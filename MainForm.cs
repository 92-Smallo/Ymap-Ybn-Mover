using Internal_Functions;
using CodeWalker.GameFiles;
using System.Diagnostics;
using System.Globalization;
using System.Text.Json;
using Vector3 = SharpDX.Vector3;
using Color = System.Drawing.Color;

namespace Ymap_Ybn_Mover
{
    public partial class MainForm : Form
    {
        private Vector3 offsetVec;
        private readonly List<Control> aboutControls = new();
        private readonly List<Control> howToControls = new();
        private readonly List<Control> vecDiffControls = new();
        private CancellationTokenSource? cts;
        private bool closeAfterProcessing;
        private readonly NumericUpDown rotationNumeric = CoordinateInput(360, -360);
        private readonly TableLayoutPanel transformPanel = new() { Name = "transformPanel" };
        private readonly CheckBox backupCheckBox = new() { Text = "Keep backups", Checked = true, AutoSize = true };
        private readonly Dictionary<GroupBox, Size> overlaySizes = new();

        public MainForm() : this(true) { }

        public MainForm(bool checkForUpdates)
        {
            Font = new Font("Segoe UI", 9f);
            InitializeComponent();
            foreach (var group in new[] { aboutGroupBox, howToUseGroupBox, vecDiffGroupBox }) overlaySizes.Add(group, group.Size);
            typeof(ListView).InvokeMember("DoubleBuffered",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.SetProperty,
                null, mainList, new object[] { true });
            mainList.Columns.Add("Status", 190);
            mainList.ShowItemToolTips = true;
            aboutControls.AddRange(new Control[] { aboutGroupBox, aboutRichTextBox, closeAboutButton });
            howToControls.AddRange(new Control[] { howToUseCloseButton, howToUseGroupBox, howToUseRichTextBox });
            vecDiffControls.AddRange(new Control[] { vecDiffGroupBox, vecDiffCloseButton, CalculateButton, CentreButton, InputButton, InvertButton,
                newOffset, vector1, vector2, CalculatedLabel, OGLocLabel, newLocLabel, InstructionsLabel });
            ConfigureTransformControls();
            Resize += (_, _) => LayoutOverlays();
            if (checkForUpdates) Shown += async (_, _) => await CheckForUpdateAsync();
            FormClosing += (_, e) =>
            {
                if (cts == null) return;
                closeAfterProcessing = true;
                cts.Cancel();
                e.Cancel = true;
                timeElapsedLabel.Text = "Stopping before closing...";
            };
        }

        private static NumericUpDown CoordinateInput(decimal maximum = 100000, decimal minimum = -100000) => new()
        {
            DecimalPlaces = 3, Maximum = maximum, Minimum = minimum, Size = new Size(120, 23)
        };

        private void ConfigureTransformControls()
        {
            SuspendLayout();
            FormBorderStyle = FormBorderStyle.Sizable;
            MaximizeBox = true;
            MinimumSize = new Size(820, 540);
            ClientSize = new Size(1040, 620);
            var layout = new TableLayoutPanel
            {
                Name = "mainLayout", Dock = DockStyle.Fill, Padding = new Padding(12),
                ColumnCount = 1, RowCount = 4
            };
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            layout.Controls.Add(new Label
            {
                Text = "Load one map's files. YMAP / YBN files rotate about its centre and move; models are resaved.",
                AutoSize = true, ForeColor = SystemColors.GrayText, Margin = new Padding(0, 0, 0, 8)
            }, 0, 0);
            mainList.Dock = DockStyle.Fill;
            mainList.Margin = new Padding(0, 0, 0, 12);
            layout.Controls.Add(mainList, 0, 1);

            // Layout containers measure each label/input instead of using designer pixel
            // coordinates. This keeps the footer aligned at different fonts and DPI.
            transformPanel.Dock = DockStyle.Fill;
            transformPanel.AutoSize = true;
            transformPanel.ColumnCount = 2;
            transformPanel.RowCount = 1;
            transformPanel.Margin = new Padding(0, 0, 0, 10);
            transformPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 70));
            transformPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30));
            var moveGroup = InputGroup("Move offset", 3);
            var moveInputs = (TableLayoutPanel)moveGroup.Controls[0];
            AddCoordinate(moveInputs, "X", xMoveNumeric, 0);
            AddCoordinate(moveInputs, "Y", yMoveNumeric, 1);
            AddCoordinate(moveInputs, "Z", zMoveNumeric, 2);
            moveGroup.Margin = new Padding(0, 0, 10, 0);
            transformPanel.Controls.Add(moveGroup, 0, 0);
            var rotationGroup = InputGroup("Rotation about map centre", 1);
            rotationGroup.Margin = Padding.Empty;
            AddCoordinate((TableLayoutPanel)rotationGroup.Controls[0], "Z (degrees)", rotationNumeric, 0);
            transformPanel.Controls.Add(rotationGroup, 1, 0);
            layout.Controls.Add(transformPanel, 0, 2);

            var actions = new TableLayoutPanel
            {
                Name = "actionsPanel", Dock = DockStyle.Fill, AutoSize = true,
                ColumnCount = 5, RowCount = 1, Margin = Padding.Empty
            };
            for (int i = 0; i < 3; i++) actions.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            actions.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            actions.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            var buttons = new[] { processAllButton, processSelectedButton, stopButton };
            for (int i = 0; i < buttons.Length; i++)
            {
                buttons[i].AutoSize = true;
                buttons[i].MinimumSize = new Size(112, 32);
                buttons[i].Margin = new Padding(0, 0, 8, 0);
                buttons[i].Anchor = AnchorStyles.Left;
                actions.Controls.Add(buttons[i], i, 0);
            }
            backupCheckBox.Anchor = AnchorStyles.Right;
            backupCheckBox.Margin = new Padding(8, 0, 0, 0);
            actions.Controls.Add(backupCheckBox, 4, 0);
            layout.Controls.Add(actions, 0, 3);
            xMoveLabel.Visible = yMoveLabel.Visible = zMoveLabel.Visible = false;
            Controls.Add(layout);
            layout.BringToFront();
            aboutGroupBox.BringToFront();
            howToUseGroupBox.BringToFront();
            vecDiffGroupBox.BringToFront();
            aboutRichTextBox.Dock = howToUseRichTextBox.Dock = DockStyle.Fill;
            closeAboutButton.Dock = howToUseCloseButton.Dock = DockStyle.Bottom;
            mainList.SizeChanged += (_, _) => UpdateListColumns();
            howToUseRichTextBox.Text = "Add all files belonging to one map, then enter a move offset and Z rotation in degrees. YMAP and YBN files rotate around the map centre first, then move by the offset. The centre is calculated automatically from the combined entity bounds of all loaded YMAPs. If no usable YMAP bounds are available, it uses the placed collision shapes. Process Selected uses the same loaded map context. Positive angles rotate counterclockwise viewed from above (+X toward +Y). Rotation does not change height.\n\nYDR, YDD and YFT files are resaved through CodeWalker without moving or rotating their local model geometry. Leave the offset and rotation at zero to resave all formats.\n\nProcessing replaces each input file. 'Keep backups' is enabled by default and saves the previous file beside it as .bak (additional backups get unique names). Stop cancels the current file before replacement where possible and skips the remaining files. Completed files remain processed.\n\nUnsupported YMAP sections and save errors leave the original file unchanged. Hover over an error row to see its full message.";
            ResumeLayout(true);
            UpdateListColumns();
            LayoutOverlays();
        }

        private static GroupBox InputGroup(string title, int coordinateCount)
        {
            var group = new GroupBox
            {
                Text = title, Dock = DockStyle.Fill, AutoSize = true,
                Padding = new Padding(12, 8, 12, 12)
            };
            var inputs = new TableLayoutPanel
            {
                Dock = DockStyle.Fill, AutoSize = true, ColumnCount = coordinateCount * 2,
                RowCount = 1, Margin = Padding.Empty
            };
            for (int i = 0; i < coordinateCount; i++)
            {
                inputs.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
                inputs.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f / coordinateCount));
            }
            group.Controls.Add(inputs);
            return group;
        }

        private void AddCoordinate(TableLayoutPanel inputs, string label, NumericUpDown input, int index)
        {
            inputs.Controls.Add(new Label
            {
                Text = label, AutoSize = true, Anchor = AnchorStyles.Left,
                Margin = new Padding(index == 0 ? 0 : 12, 0, 8, 0)
            }, index * 2, 0);
            input.ResetFont();
            input.Dock = DockStyle.Fill;
            input.TextAlign = HorizontalAlignment.Right;
            input.Margin = Padding.Empty;
            inputs.Controls.Add(input, index * 2 + 1, 0);
        }

        private void UpdateListColumns()
        {
            float scale = mainList.DeviceDpi / 96f;
            mainList.Columns[0].Width = (int)(160 * scale);
            mainList.Columns[2].Width = (int)(80 * scale);
            mainList.Columns[3].Width = (int)(190 * scale);
            mainList.Columns[1].Width = Math.Max((int)(120 * scale), mainList.ClientSize.Width -
                mainList.Columns[0].Width - mainList.Columns[2].Width - mainList.Columns[3].Width - SystemInformation.VerticalScrollBarWidth - 4);
        }

        private async Task CheckForUpdateAsync(bool manualCheck = false)
        {
            checkForUpdateToolStripMenuItem.Enabled = false;
            try
            {
                using var client = new HttpClient { Timeout = TimeSpan.FromSeconds(15) };
                client.DefaultRequestHeaders.UserAgent.ParseAdd("Ymap-Ybn-Mover/1.1.0");
                string body = await client.GetStringAsync("https://api.github.com/repos/92-Smallo/Ymap-Ybn-Mover/releases/latest");
                if (IsDisposed || Disposing || closeAfterProcessing) return;
                using var document = JsonDocument.Parse(body);
                var release = document.RootElement;
                string tag = release.GetProperty("tag_name").GetString() ?? "";
                var versionText = tag.TrimStart('v', 'V').Split('-')[0];
                var localVersion = typeof(MainForm).Assembly.GetName().Version ?? new Version(1, 1, 0);
                if (Version.TryParse(versionText, out var latestVersion) && latestVersion > new Version(localVersion.Major, localVersion.Minor, Math.Max(localVersion.Build, 0)))
                {
                    string changes = release.TryGetProperty("body", out var notes) ? notes.GetString() ?? "" : "";
                    if (MessageBox.Show(this, $"Version {tag} is available. Open the release page?\n\n{changes}", "Update available", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                        Process.Start(new ProcessStartInfo("https://github.com/92-Smallo/Ymap-Ybn-Mover/releases/latest") { UseShellExecute = true });
                }
                else if (manualCheck)
                    MessageBox.Show(this, "No newer release is available.", "Update check", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                if (manualCheck && !IsDisposed)
                    MessageBox.Show(this, $"Could not check for updates: {ex.Message}", "Update check", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            finally { if (!IsDisposed) checkForUpdateToolStripMenuItem.Enabled = true; }
        }

        private void AddFiles(string[] fileList)
        {
            if (cts != null) return;
            var fileTypes = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                { ".ymap", "YMAP Files" }, { ".ybn", "YBN Files" }, { ".ydr", "YDR Files" },
                { ".ydd", "YDD Files" }, { ".yft", "YFT Files" }
            };
            var groups = mainList.Groups.Cast<ListViewGroup>().ToDictionary(g => g.Header, g => g);
            var existing = mainList.Items.Cast<ListViewItem>().Select(item => item.SubItems[1].Text).ToHashSet(StringComparer.OrdinalIgnoreCase);
            mainList.BeginUpdate();
            try
            {
                foreach (string input in fileList)
                {
                    string file = Path.GetFullPath(input);
                    if (!fileTypes.TryGetValue(Path.GetExtension(file), out var groupName) || !existing.Add(file)) continue;
                    long size = 0;
                    try { size = new FileInfo(file).Length; } catch (IOException) { } catch (UnauthorizedAccessException) { }
                    var item = new ListViewItem(Path.GetFileNameWithoutExtension(file), 0, groups[groupName]) { UseItemStyleForSubItems = false };
                    item.SubItems.AddRange(new[] { file, $"{size / 1000} KB", "Waiting" });
                    mainList.Items.Add(item);
                }
            }
            finally { mainList.EndUpdate(); }
        }

        private Vector3 GetSelectedMapCentre()
        {
            var selected = mainList.SelectedItems.Cast<ListViewItem>()
                .Where(item => Path.GetExtension(item.SubItems[1].Text).Equals(".ymap", StringComparison.OrdinalIgnoreCase)).ToList();
            if (selected.Count == 0) throw new InvalidOperationException("Select at least one YMAP file to calculate the map centre.");
            var min = new Vector3(float.MaxValue);
            var max = new Vector3(float.MinValue);
            foreach (var item in selected)
            {
                var ymap = new YmapFile();
                ymap.Load(File.ReadAllBytes(item.SubItems[1].Text));
                min = Vector3.Min(min, ymap.CMapData.entitiesExtentsMin);
                max = Vector3.Max(max, ymap.CMapData.entitiesExtentsMax);
            }
            return MathFunctions.GetCentre(min, max);
        }

        private void CentreButton_Click(object sender, EventArgs e)
        {
            try { vector1.Text = FormatVector(GetSelectedMapCentre()); }
            catch (Exception ex) { MessageBox.Show(this, ex.Message, "Map centre", MessageBoxButtons.OK, MessageBoxIcon.Warning); }
        }

        private static string FormatVector(Vector3 vector) => string.Create(CultureInfo.InvariantCulture, $"{vector.X}, {vector.Y}, {vector.Z}");

        private static Vector3 ParseVector(string text)
        {
            var parts = text.Split(',');
            if (parts.Length != 3) throw new FormatException("Enter three coordinates as X, Y, Z, using a dot for decimals.");
            var vector = MathFunctions.CreateVectorFromStrings(parts[0], parts[1], parts[2]);
            if (!float.IsFinite(vector.X) || !float.IsFinite(vector.Y) || !float.IsFinite(vector.Z)) throw new FormatException("Coordinates must be finite numbers.");
            return vector;
        }

        private void CalculateButton_Click(object sender, EventArgs e)
        {
            try
            {
                offsetVec = ParseVector(vector2.Text) - ParseVector(vector1.Text);
                newOffset.Text = FormatVector(offsetVec);
            }
            catch (Exception ex) { MessageBox.Show(this, ex.Message, "Invalid vector", MessageBoxButtons.OK, MessageBoxIcon.Warning); }
        }

        private void InputButton_Click(object sender, EventArgs e)
        {
            if (offsetVec.X < (float)xMoveNumeric.Minimum || offsetVec.X > (float)xMoveNumeric.Maximum ||
                offsetVec.Y < (float)yMoveNumeric.Minimum || offsetVec.Y > (float)yMoveNumeric.Maximum ||
                offsetVec.Z < (float)zMoveNumeric.Minimum || offsetVec.Z > (float)zMoveNumeric.Maximum)
            {
                MessageBox.Show(this, "The calculated offset exceeds the input range.", "Offset", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            xMoveNumeric.Value = (decimal)offsetVec.X;
            yMoveNumeric.Value = (decimal)offsetVec.Y;
            zMoveNumeric.Value = (decimal)offsetVec.Z;
        }

        private async void AddFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (mainFolderDialog.ShowDialog(this) != DialogResult.OK) return;
            string folder = mainFolderDialog.SelectedPath;
            fileToolStripMenuItem.Enabled = processAllButton.Enabled = processSelectedButton.Enabled = false;
            try
            {
                var extensions = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { ".ymap", ".ybn", ".ydr", ".ydd", ".yft" };
                var files = await Task.Run(() => Directory.EnumerateFiles(folder, "*",
                    new EnumerationOptions { IgnoreInaccessible = true, RecurseSubdirectories = true })
                    .Where(file => extensions.Contains(Path.GetExtension(file))).ToArray());
                if (IsDisposed) return;
                if (files.Length == 0) MessageBox.Show(this, "No supported files found.", "Add folder", MessageBoxButtons.OK, MessageBoxIcon.Information);
                AddFiles(files);
            }
            catch (Exception ex) { if (!IsDisposed) MessageBox.Show(this, ex.Message, "Add folder", MessageBoxButtons.OK, MessageBoxIcon.Error); }
            finally { if (!IsDisposed) fileToolStripMenuItem.Enabled = processAllButton.Enabled = processSelectedButton.Enabled = true; }
        }

        private void MainListDragEnter(object sender, DragEventArgs e)
        {
            if (cts == null && e.Data?.GetDataPresent(DataFormats.FileDrop) == true) e.Effect = DragDropEffects.Copy;
        }

        private void MainListDragDrop(object sender, DragEventArgs e)
        {
            if (e.Data?.GetData(DataFormats.FileDrop) is string[] files) AddFiles(files);
        }

        private void ClearFilesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mainList.Items.Clear();
            timeElapsedLabel.Text = "";
        }

        private void ClearSelectedFilesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (var item in mainList.SelectedItems.Cast<ListViewItem>().ToArray()) item.Remove();
        }

        private void ToggleControlVisibility(List<Control> controls, bool visible)
        {
            foreach (var control in controls) control.Visible = visible;
            LayoutOverlays();
        }

        private void LayoutOverlays()
        {
            int availableHeight = ClientSize.Height - mainMenuStrip.Height - mainStatusStrip.Height;
            foreach (var (group, preferred) in overlaySizes)
            {
                int width = Math.Min(preferred.Width, ClientSize.Width - 32);
                int height = Math.Min(preferred.Height, availableHeight - 32);
                group.Bounds = new Rectangle((ClientSize.Width - width) / 2,
                    mainMenuStrip.Height + (availableHeight - height) / 2, width, height);
            }
        }

        private async Task ProcessFilesAsync(IEnumerable<ListViewItem> items)
        {
            if (cts != null) return;
            var selected = items.ToList();
            if (selected.Count == 0) return;
            var offset = new Vector3((float)xMoveNumeric.Value, (float)yMoveNumeric.Value, (float)zMoveNumeric.Value);
            float degrees = (float)rotationNumeric.Value;
            var contextFiles = mainList.Items.Cast<ListViewItem>().Select(item => item.SubItems[1].Text).ToArray();
            bool createBackup = backupCheckBox.Checked;
            cts = new CancellationTokenSource();
            var token = cts.Token;
            processAllButton.Enabled = processSelectedButton.Enabled = fileToolStripMenuItem.Enabled = editToolStripMenuItem.Enabled = false;
            SetTransformControlsEnabled(false);
            stopButton.Enabled = true;
            var watch = Stopwatch.StartNew();
            int completed = 0, errors = 0;
            bool preparing = true;
            foreach (var item in selected) UpdateListViewItem(item, Color.Blue, "Waiting");
            using var timer = new System.Windows.Forms.Timer { Interval = 100 };
            timer.Tick += (_, _) => timeElapsedLabel.Text = $"{(token.IsCancellationRequested ? "Stopping" : preparing ? "Calculating map centre" : "Processing")} | {completed} of {selected.Count} | {watch.Elapsed:mm\\:ss}";
            timer.Start();
            try
            {
                var transform = await MapBatch.CreateTransformAsync(contextFiles, offset, degrees, token);
                preparing = false;
                // CodeWalker has shared caches. One active conversion also bounds memory use
                // and makes cancellation predictable when processing large directories.
                foreach (var item in selected)
                {
                    if (token.IsCancellationRequested) break;
                    mainList.EnsureVisible(item.Index);
                    UpdateListViewItem(item, Color.Blue, "Processing");
                    try
                    {
                        await FileProcessor.ProcessAsync(item.SubItems[1].Text, transform, createBackup, token);
                        UpdateListViewItem(item, Color.Green, "Done");
                        item.Selected = false;
                    }
                    catch (OperationCanceledException) when (token.IsCancellationRequested)
                    {
                        UpdateListViewItem(item, Color.DarkOrange, "Cancelled");
                        break;
                    }
                    catch (Exception ex)
                    {
                        errors++;
                        UpdateListViewItem(item, Color.Red, "Error", ex.Message);
                    }
                    completed++;
                }
                if (token.IsCancellationRequested)
                    foreach (var item in selected.Where(item => item.SubItems[3].Text == "Waiting"))
                        UpdateListViewItem(item, Color.DarkOrange, "Skipped");
            }
            catch (OperationCanceledException) when (token.IsCancellationRequested)
            {
                foreach (var item in selected.Where(item => item.SubItems[3].Text == "Waiting"))
                    UpdateListViewItem(item, Color.DarkOrange, "Skipped");
            }
            catch (Exception ex)
            {
                foreach (var item in selected.Where(item => item.SubItems[3].Text == "Waiting"))
                {
                    errors++;
                    UpdateListViewItem(item, Color.Red, "Error", "Could not calculate the shared map centre: " + ex.Message);
                }
            }
            finally
            {
                timer.Stop();
                timeElapsedLabel.Text = $"{(token.IsCancellationRequested ? "Stopped" : "Finished")} | {completed} of {selected.Count} | {errors} errors | {watch.Elapsed:mm\\:ss}";
                cts.Dispose();
                cts = null;
                processAllButton.Enabled = processSelectedButton.Enabled = fileToolStripMenuItem.Enabled = editToolStripMenuItem.Enabled = true;
                SetTransformControlsEnabled(true);
                stopButton.Enabled = false;
                if (closeAfterProcessing) Close();
            }
        }

        private void SetTransformControlsEnabled(bool enabled)
        {
            xMoveNumeric.Enabled = yMoveNumeric.Enabled = zMoveNumeric.Enabled = backupCheckBox.Enabled = enabled;
            transformPanel.Enabled = enabled;
            vecDiffGroupBox.Enabled = enabled;
        }

        private static void UpdateListViewItem(ListViewItem item, Color color, string status, string details = "")
        {
            item.ForeColor = color;
            item.SubItems[3].Text = status;
            item.ToolTipText = details;
        }

        private void stopButton_Click(object sender, EventArgs e) => cts?.Cancel();
        private async void ProcessAllButton_Click(object sender, EventArgs e) => await ProcessFilesAsync(mainList.Items.Cast<ListViewItem>());
        private async void ProcessSelectedButton_Click(object sender, EventArgs e) => await ProcessFilesAsync(mainList.SelectedItems.Cast<ListViewItem>());
        private void ExitToolStripMenuItem_Click(object sender, EventArgs e) => Close();
        private void AddFilesToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog(this) == DialogResult.OK) AddFiles(openFileDialog1.FileNames);
        }
        private void AddFilesToolStripMenuItem_Click(object sender, EventArgs e) => AddFilesToolStripMenuItem_Click_1(sender, e);
        private void OpenFileDialog1_FileOk(object sender, EventArgs e) => AddFiles(mainFileDialog.FileNames);
        private void InvertButton_Click(object sender, EventArgs e) => (vector2.Text, vector1.Text) = (vector1.Text, vector2.Text);
        private void ClearAllYMAPsToolStripMenuItem_Click(object sender, EventArgs e) => OtherFunctions.RemoveFilesOfType(mainList, "ymapGroup");
        private void ClearAllYBNsToolStripMenuItem_Click(object sender, EventArgs e) => OtherFunctions.RemoveFilesOfType(mainList, "ybnGroup");
        private void ClearAllYDRsToolStripMenuItem_Click(object sender, EventArgs e) => OtherFunctions.RemoveFilesOfType(mainList, "ydrGroup");
        private void ClearAllYDDsToolStripMenuItem_Click(object sender, EventArgs e) => OtherFunctions.RemoveFilesOfType(mainList, "yddGroup");
        private void ClearAllYFTsToolStripMenuItem_Click(object sender, EventArgs e) => OtherFunctions.RemoveFilesOfType(mainList, "yftGroup");
        private async void CheckForUpdateToolStripMenuItem_Click(object sender, EventArgs e) => await CheckForUpdateAsync(true);
        private void AboutToolStripMenuItem_Click(object sender, EventArgs e) => ToggleControlVisibility(aboutControls, true);
        private void CloseAboutButton_Click(object sender, EventArgs e) => ToggleControlVisibility(aboutControls, false);
        private void HowToUseToolStripMenuItem_Click(object sender, EventArgs e) => ToggleControlVisibility(howToControls, true);
        private void HowToUseCloseButton_Click(object sender, EventArgs e) => ToggleControlVisibility(howToControls, false);
        private void CalcVecDiffStripMenuItem_Click(object sender, EventArgs e) => ToggleControlVisibility(vecDiffControls, true);
        private void VecDiffCloseButton_Click(object sender, EventArgs e) => ToggleControlVisibility(vecDiffControls, false);
    }
}
