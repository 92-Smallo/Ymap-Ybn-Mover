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
            ListViewGroup listViewGroup6 = new ListViewGroup("YMAP Files", HorizontalAlignment.Left);
            ListViewGroup listViewGroup7 = new ListViewGroup("YBN Files", HorizontalAlignment.Left);
            ListViewGroup listViewGroup8 = new ListViewGroup("YFT Files", HorizontalAlignment.Left);
            ListViewGroup listViewGroup9 = new ListViewGroup("YDD Files", HorizontalAlignment.Left);
            ListViewGroup listViewGroup10 = new ListViewGroup("YDR Files", HorizontalAlignment.Left);
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
            clearAllYDRsToolStripMenuItem = new ToolStripMenuItem();
            clearAllYDDsToolStripMenuItem = new ToolStripMenuItem();
            clearAllYFTsToolStripMenuItem = new ToolStripMenuItem();
            toolStripSeparator4 = new ToolStripSeparator();
            calcVecDiffStripMenuItem = new ToolStripMenuItem();
            helpToolStripMenuItem = new ToolStripMenuItem();
            checkForUpdateToolStripMenuItem = new ToolStripMenuItem();
            toolStripSeparator2 = new ToolStripSeparator();
            howToUseToolStripMenuItem = new ToolStripMenuItem();
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
            aboutGroupBox = new GroupBox();
            aboutRichTextBox = new RichTextBox();
            closeAboutButton = new Button();
            howToUseRichTextBox = new RichTextBox();
            howToUseCloseButton = new Button();
            howToUseGroupBox = new GroupBox();
            vecDiffGroupBox = new GroupBox();
            CentreButton = new Button();
            InvertButton = new Button();
            InstructionsLabel = new Label();
            InputButton = new Button();
            CalculatedLabel = new Label();
            newLocLabel = new Label();
            OGLocLabel = new Label();
            newOffset = new TextBox();
            vector2 = new TextBox();
            vector1 = new TextBox();
            CalculateButton = new Button();
            vecDiffCloseButton = new Button();
            mainMenuStrip.SuspendLayout();
            mainStatusStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)zMoveNumeric).BeginInit();
            ((System.ComponentModel.ISupportInitialize)yMoveNumeric).BeginInit();
            ((System.ComponentModel.ISupportInitialize)xMoveNumeric).BeginInit();
            aboutGroupBox.SuspendLayout();
            howToUseGroupBox.SuspendLayout();
            vecDiffGroupBox.SuspendLayout();
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
            editToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { clearFilesToolStripMenuItem, clearSelectedFilesToolStripMenuItem, toolStripSeparator3, clearAllYMAPsToolStripMenuItem, clearAllYBNsToolStripMenuItem, clearAllYDRsToolStripMenuItem, clearAllYDDsToolStripMenuItem, clearAllYFTsToolStripMenuItem, toolStripSeparator4, calcVecDiffStripMenuItem });
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
            // clearAllYDRsToolStripMenuItem
            // 
            clearAllYDRsToolStripMenuItem.Name = "clearAllYDRsToolStripMenuItem";
            clearAllYDRsToolStripMenuItem.Size = new Size(215, 22);
            clearAllYDRsToolStripMenuItem.Text = "Clear all YDRs";
            clearAllYDRsToolStripMenuItem.Click += ClearAllYDRsToolStripMenuItem_Click;
            // 
            // clearAllYDDsToolStripMenuItem
            // 
            clearAllYDDsToolStripMenuItem.Name = "clearAllYDDsToolStripMenuItem";
            clearAllYDDsToolStripMenuItem.Size = new Size(215, 22);
            clearAllYDDsToolStripMenuItem.Text = "Clear all YDDs";
            clearAllYDDsToolStripMenuItem.Click += ClearAllYDDsToolStripMenuItem_Click;
            // 
            // clearAllYFTsToolStripMenuItem
            // 
            clearAllYFTsToolStripMenuItem.Name = "clearAllYFTsToolStripMenuItem";
            clearAllYFTsToolStripMenuItem.Size = new Size(215, 22);
            clearAllYFTsToolStripMenuItem.Text = "Clear all YFTs";
            clearAllYFTsToolStripMenuItem.Click += ClearAllYFTsToolStripMenuItem_Click;
            // 
            // toolStripSeparator4
            // 
            toolStripSeparator4.Name = "toolStripSeparator4";
            toolStripSeparator4.Size = new Size(212, 6);
            // 
            // calcVecDiffStripMenuItem
            // 
            calcVecDiffStripMenuItem.Name = "calcVecDiffStripMenuItem";
            calcVecDiffStripMenuItem.Size = new Size(215, 22);
            calcVecDiffStripMenuItem.Text = "Calculate Vector Difference";
            calcVecDiffStripMenuItem.Click += CalcVecDiffStripMenuItem_Click;
            // 
            // helpToolStripMenuItem
            // 
            helpToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { checkForUpdateToolStripMenuItem, toolStripSeparator2, howToUseToolStripMenuItem, aboutToolStripMenuItem });
            helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            helpToolStripMenuItem.Size = new Size(44, 20);
            helpToolStripMenuItem.Text = "Help";
            // 
            // checkForUpdateToolStripMenuItem
            // 
            checkForUpdateToolStripMenuItem.Name = "checkForUpdateToolStripMenuItem";
            checkForUpdateToolStripMenuItem.Size = new Size(165, 22);
            checkForUpdateToolStripMenuItem.Text = "Check for Update";
            checkForUpdateToolStripMenuItem.Click += CheckForUpdateToolStripMenuItem_Click;
            // 
            // toolStripSeparator2
            // 
            toolStripSeparator2.Name = "toolStripSeparator2";
            toolStripSeparator2.Size = new Size(162, 6);
            // 
            // howToUseToolStripMenuItem
            // 
            howToUseToolStripMenuItem.Name = "howToUseToolStripMenuItem";
            howToUseToolStripMenuItem.Size = new Size(165, 22);
            howToUseToolStripMenuItem.Text = "How to Use";
            howToUseToolStripMenuItem.Click += HowToUseToolStripMenuItem_Click;
            // 
            // aboutToolStripMenuItem
            // 
            aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            aboutToolStripMenuItem.Size = new Size(165, 22);
            aboutToolStripMenuItem.Text = "About";
            aboutToolStripMenuItem.Click += AboutToolStripMenuItem_Click;
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
            listViewGroup6.Header = "YMAP Files";
            listViewGroup6.Name = "ymapGroup";
            listViewGroup7.Header = "YBN Files";
            listViewGroup7.Name = "ybnGroup";
            listViewGroup8.Header = "YFT Files";
            listViewGroup8.Name = "yftGroup";
            listViewGroup9.Header = "YDD Files";
            listViewGroup9.Name = "yddGroup";
            listViewGroup10.Header = "YDR Files";
            listViewGroup10.Name = "ydrGroup";
            mainList.Groups.AddRange(new ListViewGroup[] { listViewGroup6, listViewGroup7, listViewGroup8, listViewGroup9, listViewGroup10 });
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
            stopButton.Location = new Point(236, 470);
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
            zMoveNumeric.Maximum = new decimal(new int[] { 500000, 0, 0, 0 });
            zMoveNumeric.Minimum = new decimal(new int[] { 500000, 0, 0, int.MinValue });
            zMoveNumeric.Name = "zMoveNumeric";
            zMoveNumeric.Size = new Size(120, 23);
            zMoveNumeric.TabIndex = 20;
            // 
            // yMoveNumeric
            // 
            yMoveNumeric.DecimalPlaces = 4;
            yMoveNumeric.Location = new Point(738, 470);
            yMoveNumeric.Maximum = new decimal(new int[] { 500000, 0, 0, 0 });
            yMoveNumeric.Minimum = new decimal(new int[] { 500000, 0, 0, int.MinValue });
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
            xMoveNumeric.Maximum = new decimal(new int[] { 500000, 0, 0, 0 });
            xMoveNumeric.Minimum = new decimal(new int[] { 500000, 0, 0, int.MinValue });
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
            xMoveLabel.Size = new Size(118, 21);
            xMoveLabel.TabIndex = 23;
            xMoveLabel.Text = "Move Offset X:";
            // 
            // openFileDialog1
            // 
            openFileDialog1.FileName = "openFileDialog1";
            openFileDialog1.Filter = "All Types|*.ybn;*.ymap;*.ydr;*.ydd;*.yft;\" + \"|YBN Files|*.ybn|YMAP Files|*.ymap|YDR Files|*.ydr|YDD Files|*.ydd|YFT Files|*.yft";
            openFileDialog1.Multiselect = true;
            // 
            // aboutGroupBox
            // 
            aboutGroupBox.Controls.Add(aboutRichTextBox);
            aboutGroupBox.Controls.Add(closeAboutButton);
            aboutGroupBox.Location = new Point(340, 71);
            aboutGroupBox.Name = "aboutGroupBox";
            aboutGroupBox.Size = new Size(337, 327);
            aboutGroupBox.TabIndex = 25;
            aboutGroupBox.TabStop = false;
            aboutGroupBox.Text = "About";
            aboutGroupBox.Visible = false;
            // 
            // aboutRichTextBox
            // 
            aboutRichTextBox.BackColor = SystemColors.Control;
            aboutRichTextBox.BorderStyle = BorderStyle.None;
            aboutRichTextBox.Font = new Font("Segoe UI Variable Text", 11.25F, FontStyle.Regular, GraphicsUnit.Point);
            aboutRichTextBox.Location = new Point(6, 22);
            aboutRichTextBox.Name = "aboutRichTextBox";
            aboutRichTextBox.ReadOnly = true;
            aboutRichTextBox.Size = new Size(325, 270);
            aboutRichTextBox.TabIndex = 1;
            aboutRichTextBox.Text = resources.GetString("aboutRichTextBox.Text");
            aboutRichTextBox.Visible = false;
            // 
            // closeAboutButton
            // 
            closeAboutButton.Location = new Point(131, 298);
            closeAboutButton.Name = "closeAboutButton";
            closeAboutButton.Size = new Size(75, 23);
            closeAboutButton.TabIndex = 0;
            closeAboutButton.Text = "Close";
            closeAboutButton.UseVisualStyleBackColor = true;
            closeAboutButton.Visible = false;
            closeAboutButton.Click += CloseAboutButton_Click;
            // 
            // howToUseRichTextBox
            // 
            howToUseRichTextBox.BackColor = SystemColors.Control;
            howToUseRichTextBox.BorderStyle = BorderStyle.None;
            howToUseRichTextBox.Font = new Font("Segoe UI Variable Text", 12F, FontStyle.Regular, GraphicsUnit.Point);
            howToUseRichTextBox.Location = new Point(6, 22);
            howToUseRichTextBox.Name = "howToUseRichTextBox";
            howToUseRichTextBox.ReadOnly = true;
            howToUseRichTextBox.Size = new Size(954, 270);
            howToUseRichTextBox.TabIndex = 1;
            howToUseRichTextBox.Text = resources.GetString("howToUseRichTextBox.Text");
            howToUseRichTextBox.Visible = false;
            // 
            // howToUseCloseButton
            // 
            howToUseCloseButton.Location = new Point(446, 298);
            howToUseCloseButton.Name = "howToUseCloseButton";
            howToUseCloseButton.Size = new Size(75, 23);
            howToUseCloseButton.TabIndex = 0;
            howToUseCloseButton.Text = "Close";
            howToUseCloseButton.UseVisualStyleBackColor = true;
            howToUseCloseButton.Visible = false;
            howToUseCloseButton.Click += HowToUseCloseButton_Click;
            // 
            // howToUseGroupBox
            // 
            howToUseGroupBox.Controls.Add(howToUseRichTextBox);
            howToUseGroupBox.Controls.Add(howToUseCloseButton);
            howToUseGroupBox.Location = new Point(25, 71);
            howToUseGroupBox.Name = "howToUseGroupBox";
            howToUseGroupBox.Size = new Size(966, 327);
            howToUseGroupBox.TabIndex = 26;
            howToUseGroupBox.TabStop = false;
            howToUseGroupBox.Text = "How to Use";
            howToUseGroupBox.Visible = false;
            // 
            // vecDiffGroupBox
            // 
            vecDiffGroupBox.Controls.Add(CentreButton);
            vecDiffGroupBox.Controls.Add(InvertButton);
            vecDiffGroupBox.Controls.Add(InstructionsLabel);
            vecDiffGroupBox.Controls.Add(InputButton);
            vecDiffGroupBox.Controls.Add(CalculatedLabel);
            vecDiffGroupBox.Controls.Add(newLocLabel);
            vecDiffGroupBox.Controls.Add(OGLocLabel);
            vecDiffGroupBox.Controls.Add(newOffset);
            vecDiffGroupBox.Controls.Add(vector2);
            vecDiffGroupBox.Controls.Add(vector1);
            vecDiffGroupBox.Controls.Add(CalculateButton);
            vecDiffGroupBox.Controls.Add(vecDiffCloseButton);
            vecDiffGroupBox.Location = new Point(305, 126);
            vecDiffGroupBox.Name = "vecDiffGroupBox";
            vecDiffGroupBox.Size = new Size(407, 200);
            vecDiffGroupBox.TabIndex = 27;
            vecDiffGroupBox.TabStop = false;
            vecDiffGroupBox.Text = "Calculate Vector Difference";
            vecDiffGroupBox.Visible = false;
            // 
            // CentreButton
            // 
            CentreButton.Location = new Point(112, 134);
            CentreButton.Margin = new Padding(4);
            CentreButton.Name = "CentreButton";
            CentreButton.Size = new Size(88, 28);
            CentreButton.TabIndex = 32;
            CentreButton.Text = "Get Centre";
            CentreButton.UseVisualStyleBackColor = true;
            CentreButton.Visible = false;
            CentreButton.Click += CentreButton_Click;
            // 
            // InvertButton
            // 
            InvertButton.Location = new Point(302, 134);
            InvertButton.Margin = new Padding(4);
            InvertButton.Name = "InvertButton";
            InvertButton.Size = new Size(88, 28);
            InvertButton.TabIndex = 31;
            InvertButton.Text = "Invert Inputs";
            InvertButton.UseVisualStyleBackColor = true;
            InvertButton.Visible = false;
            InvertButton.Click += InvertButton_Click;
            // 
            // InstructionsLabel
            // 
            InstructionsLabel.AutoSize = true;
            InstructionsLabel.Location = new Point(295, 18);
            InstructionsLabel.Margin = new Padding(4, 0, 4, 0);
            InstructionsLabel.Name = "InstructionsLabel";
            InstructionsLabel.Size = new Size(85, 16);
            InstructionsLabel.TabIndex = 30;
            InstructionsLabel.Text = "Input as X, Y, Z";
            InstructionsLabel.Visible = false;
            // 
            // InputButton
            // 
            InputButton.Location = new Point(208, 134);
            InputButton.Margin = new Padding(4);
            InputButton.Name = "InputButton";
            InputButton.Size = new Size(88, 28);
            InputButton.TabIndex = 29;
            InputButton.Text = "Input Offset";
            InputButton.UseVisualStyleBackColor = true;
            InputButton.Visible = false;
            InputButton.Click += InputButton_Click;
            // 
            // CalculatedLabel
            // 
            CalculatedLabel.AutoSize = true;
            CalculatedLabel.Location = new Point(14, 106);
            CalculatedLabel.Margin = new Padding(4, 0, 4, 0);
            CalculatedLabel.Name = "CalculatedLabel";
            CalculatedLabel.Size = new Size(100, 16);
            CalculatedLabel.TabIndex = 28;
            CalculatedLabel.Text = "Calculated Offset:";
            CalculatedLabel.Visible = false;
            // 
            // newLocLabel
            // 
            newLocLabel.AutoSize = true;
            newLocLabel.Location = new Point(32, 74);
            newLocLabel.Margin = new Padding(4, 0, 4, 0);
            newLocLabel.Name = "newLocLabel";
            newLocLabel.Size = new Size(83, 16);
            newLocLabel.TabIndex = 27;
            newLocLabel.Text = "New Location:";
            newLocLabel.Visible = false;
            // 
            // OGLocLabel
            // 
            OGLocLabel.AutoSize = true;
            OGLocLabel.Location = new Point(20, 42);
            OGLocLabel.Margin = new Padding(4, 0, 4, 0);
            OGLocLabel.Name = "OGLocLabel";
            OGLocLabel.Size = new Size(101, 16);
            OGLocLabel.TabIndex = 26;
            OGLocLabel.Text = "Original Location:";
            OGLocLabel.Visible = false;
            // 
            // newOffset
            // 
            newOffset.Location = new Point(127, 102);
            newOffset.Margin = new Padding(4);
            newOffset.Name = "newOffset";
            newOffset.ReadOnly = true;
            newOffset.Size = new Size(262, 23);
            newOffset.TabIndex = 25;
            newOffset.Visible = false;
            // 
            // vector2
            // 
            vector2.Location = new Point(127, 70);
            vector2.Margin = new Padding(4);
            vector2.Name = "vector2";
            vector2.Size = new Size(262, 23);
            vector2.TabIndex = 24;
            vector2.Text = "0.0, 0.0, 0.0";
            vector2.Visible = false;
            // 
            // vector1
            // 
            vector1.Location = new Point(127, 38);
            vector1.Margin = new Padding(4);
            vector1.Name = "vector1";
            vector1.Size = new Size(262, 23);
            vector1.TabIndex = 23;
            vector1.Text = "0.0, 0.0, 0.0";
            vector1.Visible = false;
            // 
            // CalculateButton
            // 
            CalculateButton.Location = new Point(18, 134);
            CalculateButton.Margin = new Padding(4);
            CalculateButton.Name = "CalculateButton";
            CalculateButton.Size = new Size(88, 28);
            CalculateButton.TabIndex = 22;
            CalculateButton.Text = "Calculate";
            CalculateButton.UseVisualStyleBackColor = true;
            CalculateButton.Visible = false;
            CalculateButton.Click += CalculateButton_Click;
            // 
            // vecDiffCloseButton
            // 
            vecDiffCloseButton.Location = new Point(167, 169);
            vecDiffCloseButton.Name = "vecDiffCloseButton";
            vecDiffCloseButton.Size = new Size(75, 23);
            vecDiffCloseButton.TabIndex = 0;
            vecDiffCloseButton.Text = "Close";
            vecDiffCloseButton.UseVisualStyleBackColor = true;
            vecDiffCloseButton.Visible = false;
            vecDiffCloseButton.Click += VecDiffCloseButton_Click;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 16F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1016, 522);
            Controls.Add(vecDiffGroupBox);
            Controls.Add(howToUseGroupBox);
            Controls.Add(aboutGroupBox);
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
            aboutGroupBox.ResumeLayout(false);
            howToUseGroupBox.ResumeLayout(false);
            vecDiffGroupBox.ResumeLayout(false);
            vecDiffGroupBox.PerformLayout();
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
        private OpenFileDialog openFileDialog1;
        private GroupBox aboutGroupBox;
        private Button closeAboutButton;
        private RichTextBox aboutRichTextBox;
        private ToolStripMenuItem howToUseToolStripMenuItem;
        private RichTextBox howToUseRichTextBox;
        private Button howToUseCloseButton;
        private GroupBox howToUseGroupBox;
        private ToolStripMenuItem clearAllYDRsToolStripMenuItem;
        private ToolStripMenuItem clearAllYDDsToolStripMenuItem;
        private ToolStripMenuItem clearAllYFTsToolStripMenuItem;
        private GroupBox vecDiffGroupBox;
        private Button vecDiffCloseButton;
        private Button CentreButton;
        private Button InvertButton;
        private Label InstructionsLabel;
        private Button InputButton;
        private Label CalculatedLabel;
        private Label newLocLabel;
        private Label OGLocLabel;
        private TextBox newOffset;
        private TextBox vector2;
        private TextBox vector1;
        private Button CalculateButton;
        private ToolStripMenuItem calcVecDiffStripMenuItem;
    }
}