namespace Ymap_Ybn_Mover
{
    partial class CalcVect
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CalcVect));
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
            SuspendLayout();
            // 
            // CentreButton
            // 
            CentreButton.Location = new Point(102, 122);
            CentreButton.Margin = new Padding(4);
            CentreButton.Name = "CentreButton";
            CentreButton.Size = new Size(88, 28);
            CentreButton.TabIndex = 21;
            CentreButton.Text = "Get Centre";
            CentreButton.UseVisualStyleBackColor = true;
            CentreButton.Click += CentreButton_Click;
            // 
            // InvertButton
            // 
            InvertButton.Location = new Point(292, 122);
            InvertButton.Margin = new Padding(4);
            InvertButton.Name = "InvertButton";
            InvertButton.Size = new Size(88, 28);
            InvertButton.TabIndex = 20;
            InvertButton.Text = "Invert Inputs";
            InvertButton.UseVisualStyleBackColor = true;
            InvertButton.Click += InvertButton_Click;
            // 
            // InstructionsLabel
            // 
            InstructionsLabel.AutoSize = true;
            InstructionsLabel.Location = new Point(285, 6);
            InstructionsLabel.Margin = new Padding(4, 0, 4, 0);
            InstructionsLabel.Name = "InstructionsLabel";
            InstructionsLabel.Size = new Size(85, 16);
            InstructionsLabel.TabIndex = 19;
            InstructionsLabel.Text = "Input as X, Y, Z";
            // 
            // InputButton
            // 
            InputButton.Location = new Point(198, 122);
            InputButton.Margin = new Padding(4);
            InputButton.Name = "InputButton";
            InputButton.Size = new Size(88, 28);
            InputButton.TabIndex = 18;
            InputButton.Text = "Input Offset";
            InputButton.UseVisualStyleBackColor = true;
            InputButton.Click += InputButton_Click;
            // 
            // CalculatedLabel
            // 
            CalculatedLabel.AutoSize = true;
            CalculatedLabel.Location = new Point(4, 94);
            CalculatedLabel.Margin = new Padding(4, 0, 4, 0);
            CalculatedLabel.Name = "CalculatedLabel";
            CalculatedLabel.Size = new Size(100, 16);
            CalculatedLabel.TabIndex = 17;
            CalculatedLabel.Text = "Calculated Offset:";
            // 
            // newLocLabel
            // 
            newLocLabel.AutoSize = true;
            newLocLabel.Location = new Point(22, 62);
            newLocLabel.Margin = new Padding(4, 0, 4, 0);
            newLocLabel.Name = "newLocLabel";
            newLocLabel.Size = new Size(83, 16);
            newLocLabel.TabIndex = 16;
            newLocLabel.Text = "New Location:";
            // 
            // OGLocLabel
            // 
            OGLocLabel.AutoSize = true;
            OGLocLabel.Location = new Point(10, 30);
            OGLocLabel.Margin = new Padding(4, 0, 4, 0);
            OGLocLabel.Name = "OGLocLabel";
            OGLocLabel.Size = new Size(101, 16);
            OGLocLabel.TabIndex = 15;
            OGLocLabel.Text = "Original Location:";
            // 
            // newOffset
            // 
            newOffset.Location = new Point(117, 90);
            newOffset.Margin = new Padding(4);
            newOffset.Name = "newOffset";
            newOffset.ReadOnly = true;
            newOffset.Size = new Size(262, 23);
            newOffset.TabIndex = 14;
            // 
            // vector2
            // 
            vector2.Location = new Point(117, 58);
            vector2.Margin = new Padding(4);
            vector2.Name = "vector2";
            vector2.Size = new Size(262, 23);
            vector2.TabIndex = 13;
            vector2.Text = "0.0, 0.0, 0.0";
            // 
            // vector1
            // 
            vector1.Location = new Point(117, 26);
            vector1.Margin = new Padding(4);
            vector1.Name = "vector1";
            vector1.Size = new Size(262, 23);
            vector1.TabIndex = 12;
            vector1.Text = "0.0, 0.0, 0.0";
            // 
            // CalculateButton
            // 
            CalculateButton.Location = new Point(8, 122);
            CalculateButton.Margin = new Padding(4);
            CalculateButton.Name = "CalculateButton";
            CalculateButton.Size = new Size(88, 28);
            CalculateButton.TabIndex = 11;
            CalculateButton.Text = "Calculate";
            CalculateButton.UseVisualStyleBackColor = true;
            CalculateButton.Click += CalculateButton_Click;
            // 
            // CalcVect
            // 
            AutoScaleDimensions = new SizeF(7F, 16F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(388, 159);
            Controls.Add(CentreButton);
            Controls.Add(InvertButton);
            Controls.Add(InstructionsLabel);
            Controls.Add(InputButton);
            Controls.Add(CalculatedLabel);
            Controls.Add(newLocLabel);
            Controls.Add(OGLocLabel);
            Controls.Add(newOffset);
            Controls.Add(vector2);
            Controls.Add(vector1);
            Controls.Add(CalculateButton);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Icon = (Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            Name = "CalcVect";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Calculate Vector Difference";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

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
    }
}