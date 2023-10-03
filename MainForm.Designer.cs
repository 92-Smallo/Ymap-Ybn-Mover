namespace Ymap_Ybn_Mover
{
    partial class MainForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            ListViewGroup listViewGroup1 = new ListViewGroup("YMAP Files", HorizontalAlignment.Left);
            ListViewGroup listViewGroup2 = new ListViewGroup("YBN Files", HorizontalAlignment.Left);
            ListViewGroup listViewGroup3 = new ListViewGroup("RPF Files", HorizontalAlignment.Left);
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            mainMenuStrip = new MenuStrip();
            fileToolStripMenuItem = new ToolStripMenuItem();
            addFilesToolStripMenuItem = new ToolStripMenuItem();
            addFolderToolStripMenuItem = new ToolStripMenuItem();
            toolStripSeparator1 = new ToolStripSeparator();
            exitToolStripMenuItem = new ToolStripMenuItem();
            editToolStripMenuItem = new ToolStripMenuItem();
            clearFilesToolStripMenuItem = new ToolStripMenuItem();
            clearSelectedFilesToolStripMenuItem = new ToolStripMenuItem();
            toolStripSeparator3 = new ToolStripSeparator();
            clearAllYMAPsToolStripMenuItem = new ToolStripMenuItem();
            clearAllYBNsToolStripMenuItem = new ToolStripMenuItem();
            toolStripSeparator4 = new ToolStripSeparator();
            calculateVectorDifferenceToolStripMenuItem = new ToolStripMenuItem();
            helpToolStripMenuItem = new ToolStripMenuItem();
            checkForUpdateToolStripMenuItem = new ToolStripMenuItem();
            toolStripSeparator2 = new ToolStripSeparator();
            aboutToolStripMenuItem = new ToolStripMenuItem();
            mainStatusStrip = new StatusStrip();
            timeElapsedLabel = new ToolStripStatusLabel();
            mainList = new ListView();
            filenameHeader = new ColumnHeader();
            fileLocationHeader = new ColumnHeader();
            filesizeHeader = new ColumnHeader();
            processedHeader = new ColumnHeader();
            mainFileDialog = new OpenFileDialog();
            button2 = new Button();
            processAllButton = new Button();
            processSelectedButton = new Button();
            zMoveLabel = new Label();
            stopButton = new Button();
            mainFolderDialog = new FolderBrowserDialog();
            zMoveNumeric = new NumericUpDown();
            yMoveNumeric = new NumericUpDown();
            yMoveLabel = new Label();
            xMoveNumeric = new NumericUpDown();
            xMoveLabel = new Label();
            openFileDialog1 = new OpenFileDialog();
            mainMenuStrip.SuspendLayout();
            mainStatusStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)zMoveNumeric).BeginInit();
            ((System.ComponentModel.ISupportInitialize)yMoveNumeric).BeginInit();
            ((System.ComponentModel.ISupportInitialize)xMoveNumeric).BeginInit();
            SuspendLayout();
            // 
            // mainMenuStrip
            // 
            mainMenuStrip.Items.AddRange(new ToolStripItem[] { fileToolStripMenuItem, editToolStripMenuItem, helpToolStripMenuItem });
            mainMenuStrip.Location = new Point(0, 0);
            mainMenuStrip.Name = "mainMenuStrip";
            mainMenuStrip.Size = new Size(1016, 24);
            mainMenuStrip.TabIndex = 1;
            mainMenuStrip.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            fileToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { addFilesToolStripMenuItem, addFolderToolStripMenuItem, toolStripSeparator1, exitToolStripMenuItem });
            fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            fileToolStripMenuItem.Size = new Size(37, 20);
            fileToolStripMenuItem.Text = "File";
            // 
            // addFilesToolStripMenuItem
            // 
            addFilesToolStripMenuItem.Name = "addFilesToolStripMenuItem";
            addFilesToolStripMenuItem.Size = new Size(132, 22);
            addFilesToolStripMenuItem.Text = "Add Files";
            addFilesToolStripMenuItem.Click += AddFilesToolStripMenuItem_Click_1;
            // 
            // addFolderToolStripMenuItem
            // 
            addFolderToolStripMenuItem.Name = "addFolderToolStripMenuItem";
            addFolderToolStripMenuItem.Size = new Size(132, 22);
            addFolderToolStripMenuItem.Text = "Add Folder";
            addFolderToolStripMenuItem.Click += AddFolderToolStripMenuItem_Click;
            // 
            // toolStripSeparator1
            // 
            toolStripSeparator1.Name = "toolStripSeparator1";
            toolStripSeparator1.Size = new Size(129, 6);
            // 
            // exitToolStripMenuItem
            // 
            exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            exitToolStripMenuItem.Size = new Size(132, 22);
            exitToolStripMenuItem.Text = "Exit";
            exitToolStripMenuItem.Click += ExitToolStripMenuItem_Click;
            // 
            // editToolStripMenuItem
            // 
            editToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { clearFilesToolStripMenuItem, clearSelectedFilesToolStripMenuItem, toolStripSeparator3, clearAllYMAPsToolStripMenuItem, clearAllYBNsToolStripMenuItem, toolStripSeparator4, calculateVectorDifferenceToolStripMenuItem });
            editToolStripMenuItem.Name = "editToolStripMenuItem";
            editToolStripMenuItem.Size = new Size(39, 20);
            editToolStripMenuItem.Text = "Edit";
            // 
            // clearFilesToolStripMenuItem
            // 
            clearFilesToolStripMenuItem.Name = "clearFilesToolStripMenuItem";
            clearFilesToolStripMenuItem.Size = new Size(215, 22);
            clearFilesToolStripMenuItem.Text = "Clear File(s)";
            clearFilesToolStripMenuItem.Click += ClearFilesToolStripMenuItem_Click;
            // 
            // clearSelectedFilesToolStripMenuItem
            // 
            clearSelectedFilesToolStripMenuItem.Name = "clearSelectedFilesToolStripMenuItem";
            clearSelectedFilesToolStripMenuItem.Size = new Size(215, 22);
            clearSelectedFilesToolStripMenuItem.Text = "Clear Selected File(s)";
            clearSelectedFilesToolStripMenuItem.Click += ClearSelectedFilesToolStripMenuItem_Click;
            // 
            // toolStripSeparator3
            // 
            toolStripSeparator3.Name = "toolStripSeparator3";
            toolStripSeparator3.Size = new Size(212, 6);
            // 
            // clearAllYMAPsToolStripMenuItem
            // 
            clearAllYMAPsToolStripMenuItem.Name = "clearAllYMAPsToolStripMenuItem";
            clearAllYMAPsToolStripMenuItem.Size = new Size(215, 22);
            clearAllYMAPsToolStripMenuItem.Text = "Clear all YMAPs";
            clearAllYMAPsToolStripMenuItem.Click += ClearAllYMAPsToolStripMenuItem_Click;
            // 
            // clearAllYBNsToolStripMenuItem
            // 
            clearAllYBNsToolStripMenuItem.Name = "clearAllYBNsToolStripMenuItem";
            clearAllYBNsToolStripMenuItem.Size = new Size(215, 22);
            clearAllYBNsToolStripMenuItem.Text = "Clear all YBNs";
            clearAllYBNsToolStripMenuItem.Click += ClearAllYBNsToolStripMenuItem_Click;
            // 
            // toolStripSeparator4
            // 
            toolStripSeparator4.Name = "toolStripSeparator4";
            toolStripSeparator4.Size = new Size(212, 6);
            // 
            // calculateVectorDifferenceToolStripMenuItem
            // 
            calculateVectorDifferenceToolStripMenuItem.Name = "calculateVectorDifferenceToolStripMenuItem";
            calculateVectorDifferenceToolStripMenuItem.Size = new Size(215, 22);
            calculateVectorDifferenceToolStripMenuItem.Text = "Calculate Vector Difference";
            // 
            // helpToolStripMenuItem
            // 
            helpToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { checkForUpdateToolStripMenuItem, toolStripSeparator2, aboutToolStripMenuItem });
            helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            helpToolStripMenuItem.Size = new Size(44, 20);
            helpToolStripMenuItem.Text = "Help";
            // 
            // checkForUpdateToolStripMenuItem
            // 
            checkForUpdateToolStripMenuItem.Name = "checkForUpdateToolStripMenuItem";
            checkForUpdateToolStripMenuItem.Size = new Size(180, 22);
            checkForUpdateToolStripMenuItem.Text = "Check for Update";
            checkForUpdateToolStripMenuItem.Click += CheckForUpdateToolStripMenuItem_Click;
            // 
            // toolStripSeparator2
            // 
            toolStripSeparator2.Name = "toolStripSeparator2";
            toolStripSeparator2.Size = new Size(177, 6);
            // 
            // aboutToolStripMenuItem
            // 
            aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            aboutToolStripMenuItem.Size = new Size(180, 22);
            aboutToolStripMenuItem.Text = "About";
            // 
            // mainStatusStrip
            // 
            mainStatusStrip.Items.AddRange(new ToolStripItem[] { timeElapsedLabel });
            mainStatusStrip.Location = new Point(0, 500);
            mainStatusStrip.Name = "mainStatusStrip";
            mainStatusStrip.Size = new Size(1016, 22);
            mainStatusStrip.TabIndex = 2;
            mainStatusStrip.Text = "statusStrip1";
            // 
            // timeElapsedLabel
            // 
            timeElapsedLabel.Name = "timeElapsedLabel";
            timeElapsedLabel.Size = new Size(0, 17);
            // 
            // mainList
            // 
            mainList.AllowDrop = true;
            mainList.Columns.AddRange(new ColumnHeader[] { filenameHeader, fileLocationHeader, filesizeHeader, processedHeader });
            listViewGroup1.Header = "YMAP Files";
            listViewGroup1.Name = "ymapGroup";
            listViewGroup2.Header = "YBN Files";
            listViewGroup2.Name = "ybnGroup";
            listViewGroup3.Header = "RPF Files";
            listViewGroup3.Name = "rpfGroup";
            mainList.Groups.AddRange(new ListViewGroup[] { listViewGroup1, listViewGroup2, listViewGroup3 });
            mainList.HeaderStyle = ColumnHeaderStyle.Nonclickable;
            mainList.Location = new Point(12, 27);
            mainList.Name = "mainList";
            mainList.Size = new Size(992, 437);
            mainList.Sorting = SortOrder.Ascending;
            mainList.TabIndex = 3;
            mainList.UseCompatibleStateImageBehavior = false;
            mainList.View = View.Details;
            mainList.DragDrop += MainListDragDrop;
            mainList.DragEnter += MainListDragEnter;
            // 
            // filenameHeader
            // 
            filenameHeader.Text = "Filename";
            filenameHeader.Width = 59;
            // 
            // fileLocationHeader
            // 
            fileLocationHeader.DisplayIndex = 2;
            fileLocationHeader.Text = "File Location";
            fileLocationHeader.Width = 876;
            // 
            // filesizeHeader
            // 
            filesizeHeader.DisplayIndex = 1;
            filesizeHeader.Text = "File Size";
            filesizeHeader.Width = 53;
            // 
            // processedHeader
            // 
            processedHeader.Text = "Status";
            processedHeader.Width = 70;
            // 
            // mainFileDialog
            // 
            mainFileDialog.FileName = "openFileDialog1";
            mainFileDialog.Filter = "GTAV Map Files|*.ymap;*.ybn";
            mainFileDialog.Multiselect = true;
            mainFileDialog.FileOk += OpenFileDialog1_FileOk;
            // 
            // button2
            // 
            button2.Location = new Point(0, 0);
            button2.Name = "button2";
            button2.Size = new Size(75, 23);
            button2.TabIndex = 0;
            // 
            // processAllButton
            // 
            processAllButton.Location = new Point(12, 470);
            processAllButton.Name = "processAllButton";
            processAllButton.Size = new Size(106, 23);
            processAllButton.TabIndex = 5;
            processAllButton.Text = "Process All";
            processAllButton.UseVisualStyleBackColor = true;
            processAllButton.Click += ProcessAllButton_Click;
            // 
            // processSelectedButton
            // 
            processSelectedButton.Location = new Point(124, 470);
            processSelectedButton.Name = "processSelectedButton";
            processSelectedButton.Size = new Size(106, 23);
            processSelectedButton.TabIndex = 6;
            processSelectedButton.Text = "Process Selected";
            processSelectedButton.UseVisualStyleBackColor = true;
            processSelectedButton.Click += ProcessSelectedButton_Click;
            // 
            // zMoveLabel
            // 
            zMoveLabel.AutoSize = true;
            zMoveLabel.Font = new Font("Segoe UI Variable Display Semib", 12F, FontStyle.Bold, GraphicsUnit.Point);
            zMoveLabel.Location = new Point(864, 470);
            zMoveLabel.Name = "zMoveLabel";
            zMoveLabel.Size = new Size(23, 21);
            zMoveLabel.TabIndex = 8;
            zMoveLabel.Text = "Z:";
            // 
            // stopButton
            // 
            stopButton.Enabled = false;
            stopButton.Location = new Point(236, 469);
            stopButton.Name = "stopButton";
            stopButton.Size = new Size(106, 23);
            stopButton.TabIndex = 19;
            stopButton.Text = "Stop";
            stopButton.UseVisualStyleBackColor = true;
            // 
            // zMoveNumeric
            // 
            zMoveNumeric.DecimalPlaces = 4;
            zMoveNumeric.Location = new Point(884, 470);
            zMoveNumeric.Maximum = new decimal(new int[] { 50000, 0, 0, 0 });
            zMoveNumeric.Minimum = new decimal(new int[] { 50000, 0, 0, int.MinValue });
            zMoveNumeric.Name = "zMoveNumeric";
            zMoveNumeric.Size = new Size(120, 23);
            zMoveNumeric.TabIndex = 20;
            // 
            // yMoveNumeric
            // 
            yMoveNumeric.DecimalPlaces = 4;
            yMoveNumeric.Location = new Point(738, 470);
            yMoveNumeric.Maximum = new decimal(new int[] { 50000, 0, 0, 0 });
            yMoveNumeric.Minimum = new decimal(new int[] { 50000, 0, 0, int.MinValue });
            yMoveNumeric.Name = "yMoveNumeric";
            yMoveNumeric.Size = new Size(120, 23);
            yMoveNumeric.TabIndex = 22;
            // 
            // yMoveLabel
            // 
            yMoveLabel.AutoSize = true;
            yMoveLabel.Font = new Font("Segoe UI Variable Display Semib", 12F, FontStyle.Bold, GraphicsUnit.Point);
            yMoveLabel.Location = new Point(718, 470);
            yMoveLabel.Name = "yMoveLabel";
            yMoveLabel.Size = new Size(23, 21);
            yMoveLabel.TabIndex = 21;
            yMoveLabel.Text = "Y:";
            // 
            // xMoveNumeric
            // 
            xMoveNumeric.DecimalPlaces = 4;
            xMoveNumeric.Location = new Point(592, 470);
            xMoveNumeric.Maximum = new decimal(new int[] { 50000, 0, 0, 0 });
            xMoveNumeric.Minimum = new decimal(new int[] { 50000, 0, 0, int.MinValue });
            xMoveNumeric.Name = "xMoveNumeric";
            xMoveNumeric.Size = new Size(120, 23);
            xMoveNumeric.TabIndex = 24;
            // 
            // xMoveLabel
            // 
            xMoveLabel.AutoSize = true;
            xMoveLabel.Font = new Font("Segoe UI Variable Display Semib", 12F, FontStyle.Bold, GraphicsUnit.Point);
            xMoveLabel.Location = new Point(478, 470);
            xMoveLabel.Name = "xMoveLabel";
            xMoveLabel.Size = new Size(117, 21);
            xMoveLabel.TabIndex = 23;
            xMoveLabel.Text = "Move Offset Z:";
            // 
            // openFileDialog1
            // 
            openFileDialog1.FileName = "openFileDialog1";
            openFileDialog1.Filter = "All Types|*.ybn;*.ymap;\" + \"|YBN Files|*.ybn|YMAP Files|*.ymap";
            openFileDialog1.Multiselect = true;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 16F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1016, 522);
            Controls.Add(xMoveNumeric);
            Controls.Add(xMoveLabel);
            Controls.Add(yMoveNumeric);
            Controls.Add(yMoveLabel);
            Controls.Add(zMoveNumeric);
            Controls.Add(stopButton);
            Controls.Add(processSelectedButton);
            Controls.Add(processAllButton);
            Controls.Add(mainList);
            Controls.Add(mainStatusStrip);
            Controls.Add(mainMenuStrip);
            Controls.Add(zMoveLabel);
            DoubleBuffered = true;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Icon = (Icon)resources.GetObject("$this.Icon");
            MainMenuStrip = mainMenuStrip;
            MaximizeBox = false;
            Name = "MainForm";
            Text = "YMAP YBN Mover";
            mainMenuStrip.ResumeLayout(false);
            mainMenuStrip.PerformLayout();
            mainStatusStrip.ResumeLayout(false);
            mainStatusStrip.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)zMoveNumeric).EndInit();
            ((System.ComponentModel.ISupportInitialize)yMoveNumeric).EndInit();
            ((System.ComponentModel.ISupportInitialize)xMoveNumeric).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private MenuStrip mainMenuStrip;
        private ToolStripMenuItem fileToolStripMenuItem;
        private ToolStripMenuItem editToolStripMenuItem;
        private ToolStripMenuItem helpToolStripMenuItem;
        private StatusStrip mainStatusStrip;
        private ListView mainList;
        private OpenFileDialog mainFileDialog;
        private ColumnHeader filenameHeader;
        private ColumnHeader filesizeHeader;
        private ColumnHeader fileLocationHeader;
        private ColumnHeader processedHeader;
        private Button button2;
        private Button processAllButton;
        private Button processSelectedButton;
        private Label zMoveLabel;
        private Button stopButton;
        private ToolStripMenuItem addFilesToolStripMenuItem;
        private ToolStripMenuItem addFolderToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripMenuItem exitToolStripMenuItem;
        private FolderBrowserDialog mainFolderDialog;
        private NumericUpDown zMoveNumeric;
        private NumericUpDown yMoveNumeric;
        private Label yMoveLabel;
        private NumericUpDown xMoveNumeric;
        private Label xMoveLabel;
        private ToolStripStatusLabel timeElapsedLabel;
        private ToolStripMenuItem checkForUpdateToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator2;
        private ToolStripMenuItem aboutToolStripMenuItem;
        private ToolStripMenuItem clearFilesToolStripMenuItem;
        private ToolStripMenuItem clearSelectedFilesToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator3;
        private ToolStripMenuItem clearAllYMAPsToolStripMenuItem;
        private ToolStripMenuItem clearAllYBNsToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator4;
        private ToolStripMenuItem calculateVectorDifferenceToolStripMenuItem;
        private OpenFileDialog openFileDialog1;
    }
}