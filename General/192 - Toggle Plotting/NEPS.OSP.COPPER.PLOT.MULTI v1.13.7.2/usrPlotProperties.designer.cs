namespace NEPS.OSP.COPPER.PLOT.MULTI
{
    partial class usrPlotProperties
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(usrPlotProperties));
            this.gbType = new System.Windows.Forms.GroupBox();
            this.cbType = new System.Windows.Forms.CheckBox();
            this.lbType = new System.Windows.Forms.ListBox();
            this.gbMapScale = new System.Windows.Forms.GroupBox();
            this.lbMapScalePreDefined = new System.Windows.Forms.ListBox();
            this.tbMapScaleCustom = new System.Windows.Forms.TextBox();
            this.optMapScaleCustom = new System.Windows.Forms.RadioButton();
            this.optMapScalePreDefined = new System.Windows.Forms.RadioButton();
            this.gbPaper = new System.Windows.Forms.GroupBox();
            this.optPaperOrientationPortrait = new System.Windows.Forms.RadioButton();
            this.optPaperOrientationLandbase = new System.Windows.Forms.RadioButton();
            this.lbPaperSize = new System.Windows.Forms.ListBox();
            this.lblPaperSize = new System.Windows.Forms.Label();
            this.btnCancel = new System.Windows.Forms.Button();
            this.GroupBox1 = new System.Windows.Forms.GroupBox();
            this.lblSetMapSizeValue = new System.Windows.Forms.Label();
            this.lblSetMapScaleValue = new System.Windows.Forms.Label();
            this.lblSetBorderInsetValue = new System.Windows.Forms.Label();
            this.lblSetPaperValue = new System.Windows.Forms.Label();
            this.lblSetMapSize = new System.Windows.Forms.Label();
            this.lblSetMapScale = new System.Windows.Forms.Label();
            this.lblBorderInset = new System.Windows.Forms.Label();
            this.lblSetPaper = new System.Windows.Forms.Label();
            this.imgPageContainer = new System.Windows.Forms.PictureBox();
            this.btnPlace = new System.Windows.Forms.Button();
            this.ImageList1 = new System.Windows.Forms.ImageList(this.components);
            this.gbType.SuspendLayout();
            this.gbMapScale.SuspendLayout();
            this.gbPaper.SuspendLayout();
            this.GroupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.imgPageContainer)).BeginInit();
            this.SuspendLayout();
            // 
            // gbType
            // 
            this.gbType.Controls.Add(this.cbType);
            this.gbType.Controls.Add(this.lbType);
            this.gbType.Location = new System.Drawing.Point(422, 0);
            this.gbType.Name = "gbType";
            this.gbType.Size = new System.Drawing.Size(170, 100);
            this.gbType.TabIndex = 15;
            this.gbType.TabStop = false;
            this.gbType.Text = "Type:";
            // 
            // cbType
            // 
            this.cbType.AutoSize = true;
            this.cbType.Checked = true;
            this.cbType.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbType.Location = new System.Drawing.Point(10, 0);
            this.cbType.Name = "cbType";
            this.cbType.Size = new System.Drawing.Size(50, 17);
            this.cbType.TabIndex = 6;
            this.cbType.Text = "Type";
            this.cbType.UseVisualStyleBackColor = true;
            this.cbType.CheckedChanged += new System.EventHandler(this.cbType_CheckedChanged);
            // 
            // lbType
            // 
            this.lbType.FormattingEnabled = true;
            this.lbType.Items.AddRange(new object[] {
            "Construction",
            "Construction SLD",
            "UG Construction Print",
            "Pipe Line Crossing",
            "RCM Trace Map"});
            this.lbType.Location = new System.Drawing.Point(10, 19);
            this.lbType.Name = "lbType";
            this.lbType.Size = new System.Drawing.Size(148, 69);
            this.lbType.TabIndex = 0;
            this.lbType.SelectedIndexChanged += new System.EventHandler(this.lbType_SelectedIndexChanged);
            // 
            // gbMapScale
            // 
            this.gbMapScale.Controls.Add(this.lbMapScalePreDefined);
            this.gbMapScale.Controls.Add(this.tbMapScaleCustom);
            this.gbMapScale.Controls.Add(this.optMapScaleCustom);
            this.gbMapScale.Controls.Add(this.optMapScalePreDefined);
            this.gbMapScale.Location = new System.Drawing.Point(422, 245);
            this.gbMapScale.Name = "gbMapScale";
            this.gbMapScale.Size = new System.Drawing.Size(170, 152);
            this.gbMapScale.TabIndex = 14;
            this.gbMapScale.TabStop = false;
            this.gbMapScale.Text = "Map Scale:";
            // 
            // lbMapScalePreDefined
            // 
            this.lbMapScalePreDefined.FormattingEnabled = true;
            this.lbMapScalePreDefined.Items.AddRange(new object[] {
            "1:250",
            "1:350",
            "1:500",
            "1:800"});
            this.lbMapScalePreDefined.Location = new System.Drawing.Point(10, 39);
            this.lbMapScalePreDefined.Name = "lbMapScalePreDefined";
            this.lbMapScalePreDefined.Size = new System.Drawing.Size(148, 56);
            this.lbMapScalePreDefined.TabIndex = 2;
            this.lbMapScalePreDefined.SelectedIndexChanged += new System.EventHandler(this.lbMapScalePreDefined_SelectedIndexChanged);
            // 
            // tbMapScaleCustom
            // 
            this.tbMapScaleCustom.Location = new System.Drawing.Point(10, 124);
            this.tbMapScaleCustom.Name = "tbMapScaleCustom";
            this.tbMapScaleCustom.Size = new System.Drawing.Size(148, 21);
            this.tbMapScaleCustom.TabIndex = 1;
            this.tbMapScaleCustom.TextChanged += new System.EventHandler(this.tbMapScaleCustom_TextChanged);
            // 
            // optMapScaleCustom
            // 
            this.optMapScaleCustom.AutoSize = true;
            this.optMapScaleCustom.Location = new System.Drawing.Point(10, 101);
            this.optMapScaleCustom.Name = "optMapScaleCustom";
            this.optMapScaleCustom.Size = new System.Drawing.Size(61, 17);
            this.optMapScaleCustom.TabIndex = 0;
            this.optMapScaleCustom.Text = "Custom";
            this.optMapScaleCustom.UseVisualStyleBackColor = true;
            this.optMapScaleCustom.CheckedChanged += new System.EventHandler(this.optMapScaleCustom_CheckedChanged);
            // 
            // optMapScalePreDefined
            // 
            this.optMapScalePreDefined.AutoSize = true;
            this.optMapScalePreDefined.Checked = true;
            this.optMapScalePreDefined.Location = new System.Drawing.Point(10, 21);
            this.optMapScalePreDefined.Name = "optMapScalePreDefined";
            this.optMapScalePreDefined.Size = new System.Drawing.Size(85, 17);
            this.optMapScalePreDefined.TabIndex = 0;
            this.optMapScalePreDefined.TabStop = true;
            this.optMapScalePreDefined.Text = "Pre-defined:";
            this.optMapScalePreDefined.UseVisualStyleBackColor = true;
            this.optMapScalePreDefined.CheckedChanged += new System.EventHandler(this.optMapScalePreDefined_CheckedChanged);
            // 
            // gbPaper
            // 
            this.gbPaper.Controls.Add(this.optPaperOrientationPortrait);
            this.gbPaper.Controls.Add(this.optPaperOrientationLandbase);
            this.gbPaper.Controls.Add(this.lbPaperSize);
            this.gbPaper.Controls.Add(this.lblPaperSize);
            this.gbPaper.Location = new System.Drawing.Point(422, 106);
            this.gbPaper.Name = "gbPaper";
            this.gbPaper.Size = new System.Drawing.Size(170, 133);
            this.gbPaper.TabIndex = 13;
            this.gbPaper.TabStop = false;
            this.gbPaper.Text = "Paper:";
            // 
            // optPaperOrientationPortrait
            // 
            this.optPaperOrientationPortrait.AutoSize = true;
            this.optPaperOrientationPortrait.Enabled = false;
            this.optPaperOrientationPortrait.Location = new System.Drawing.Point(91, 109);
            this.optPaperOrientationPortrait.Name = "optPaperOrientationPortrait";
            this.optPaperOrientationPortrait.Size = new System.Drawing.Size(61, 17);
            this.optPaperOrientationPortrait.TabIndex = 3;
            this.optPaperOrientationPortrait.Text = "Portrait";
            this.optPaperOrientationPortrait.UseVisualStyleBackColor = true;
            this.optPaperOrientationPortrait.CheckedChanged += new System.EventHandler(this.optPaperOrientationPortrait_CheckedChanged);
            // 
            // optPaperOrientationLandbase
            // 
            this.optPaperOrientationLandbase.AutoSize = true;
            this.optPaperOrientationLandbase.Checked = true;
            this.optPaperOrientationLandbase.Location = new System.Drawing.Point(10, 109);
            this.optPaperOrientationLandbase.Name = "optPaperOrientationLandbase";
            this.optPaperOrientationLandbase.Size = new System.Drawing.Size(76, 17);
            this.optPaperOrientationLandbase.TabIndex = 2;
            this.optPaperOrientationLandbase.TabStop = true;
            this.optPaperOrientationLandbase.Text = "Landscape";
            this.optPaperOrientationLandbase.UseVisualStyleBackColor = true;
            this.optPaperOrientationLandbase.CheckedChanged += new System.EventHandler(this.optPaperOrientationLandbase_CheckedChanged);
            // 
            // lbPaperSize
            // 
            this.lbPaperSize.FormattingEnabled = true;
            this.lbPaperSize.Items.AddRange(new object[] {
            "A-Size 8.5 x 11",
            "B-Size 11 x 17",
            "C-Size 17 x 22",
            "D-Size 22 x 34",
            "E-Size 34 x 44"});
            this.lbPaperSize.Location = new System.Drawing.Point(10, 33);
            this.lbPaperSize.Name = "lbPaperSize";
            this.lbPaperSize.Size = new System.Drawing.Size(148, 69);
            this.lbPaperSize.TabIndex = 1;
            this.lbPaperSize.SelectedIndexChanged += new System.EventHandler(this.lbPaperSize_SelectedIndexChanged);
            // 
            // lblPaperSize
            // 
            this.lblPaperSize.AutoSize = true;
            this.lblPaperSize.Location = new System.Drawing.Point(7, 17);
            this.lblPaperSize.Name = "lblPaperSize";
            this.lblPaperSize.Size = new System.Drawing.Size(30, 13);
            this.lblPaperSize.TabIndex = 0;
            this.lblPaperSize.Text = "Size:";
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(513, 403);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 34);
            this.btnCancel.TabIndex = 10;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // GroupBox1
            // 
            this.GroupBox1.Controls.Add(this.lblSetMapSizeValue);
            this.GroupBox1.Controls.Add(this.lblSetMapScaleValue);
            this.GroupBox1.Controls.Add(this.lblSetBorderInsetValue);
            this.GroupBox1.Controls.Add(this.lblSetPaperValue);
            this.GroupBox1.Controls.Add(this.lblSetMapSize);
            this.GroupBox1.Controls.Add(this.lblSetMapScale);
            this.GroupBox1.Controls.Add(this.lblBorderInset);
            this.GroupBox1.Controls.Add(this.lblSetPaper);
            this.GroupBox1.Controls.Add(this.imgPageContainer);
            this.GroupBox1.Location = new System.Drawing.Point(3, 0);
            this.GroupBox1.Name = "GroupBox1";
            this.GroupBox1.Size = new System.Drawing.Size(413, 437);
            this.GroupBox1.TabIndex = 12;
            this.GroupBox1.TabStop = false;
            // 
            // lblSetMapSizeValue
            // 
            this.lblSetMapSizeValue.AutoSize = true;
            this.lblSetMapSizeValue.Location = new System.Drawing.Point(162, 416);
            this.lblSetMapSizeValue.Name = "lblSetMapSizeValue";
            this.lblSetMapSizeValue.Size = new System.Drawing.Size(95, 13);
            this.lblSetMapSizeValue.TabIndex = 8;
            this.lblSetMapSizeValue.Text = "61.4mm x 39.1mm";
            // 
            // lblSetMapScaleValue
            // 
            this.lblSetMapScaleValue.AutoSize = true;
            this.lblSetMapScaleValue.Location = new System.Drawing.Point(162, 403);
            this.lblSetMapScaleValue.Name = "lblSetMapScaleValue";
            this.lblSetMapScaleValue.Size = new System.Drawing.Size(35, 13);
            this.lblSetMapScaleValue.TabIndex = 7;
            this.lblSetMapScaleValue.Text = "1:250";
            // 
            // lblSetBorderInsetValue
            // 
            this.lblSetBorderInsetValue.AutoSize = true;
            this.lblSetBorderInsetValue.Location = new System.Drawing.Point(162, 390);
            this.lblSetBorderInsetValue.Name = "lblSetBorderInsetValue";
            this.lblSetBorderInsetValue.Size = new System.Drawing.Size(35, 13);
            this.lblSetBorderInsetValue.TabIndex = 6;
            this.lblSetBorderInsetValue.Text = "17mm";
            // 
            // lblSetPaperValue
            // 
            this.lblSetPaperValue.AutoSize = true;
            this.lblSetPaperValue.Location = new System.Drawing.Point(162, 377);
            this.lblSetPaperValue.Name = "lblSetPaperValue";
            this.lblSetPaperValue.Size = new System.Drawing.Size(109, 13);
            this.lblSetPaperValue.TabIndex = 5;
            this.lblSetPaperValue.Text = "8.5 x 11 (Landscape)";
            // 
            // lblSetMapSize
            // 
            this.lblSetMapSize.AutoSize = true;
            this.lblSetMapSize.Location = new System.Drawing.Point(6, 416);
            this.lblSetMapSize.Name = "lblSetMapSize";
            this.lblSetMapSize.Size = new System.Drawing.Size(49, 13);
            this.lblSetMapSize.TabIndex = 4;
            this.lblSetMapSize.Text = "Map Size";
            // 
            // lblSetMapScale
            // 
            this.lblSetMapScale.AutoSize = true;
            this.lblSetMapScale.Location = new System.Drawing.Point(6, 403);
            this.lblSetMapScale.Name = "lblSetMapScale";
            this.lblSetMapScale.Size = new System.Drawing.Size(55, 13);
            this.lblSetMapScale.TabIndex = 3;
            this.lblSetMapScale.Text = "Map Scale";
            // 
            // lblBorderInset
            // 
            this.lblBorderInset.AutoSize = true;
            this.lblBorderInset.Location = new System.Drawing.Point(6, 390);
            this.lblBorderInset.Name = "lblBorderInset";
            this.lblBorderInset.Size = new System.Drawing.Size(71, 13);
            this.lblBorderInset.TabIndex = 2;
            this.lblBorderInset.Text = "Border Inset:";
            // 
            // lblSetPaper
            // 
            this.lblSetPaper.AutoSize = true;
            this.lblSetPaper.Location = new System.Drawing.Point(6, 377);
            this.lblSetPaper.Name = "lblSetPaper";
            this.lblSetPaper.Size = new System.Drawing.Size(39, 13);
            this.lblSetPaper.TabIndex = 1;
            this.lblSetPaper.Text = "Paper:";
            // 
            // imgPageContainer
            // 
            this.imgPageContainer.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.imgPageContainer.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.imgPageContainer.Location = new System.Drawing.Point(9, 16);
            this.imgPageContainer.Name = "imgPageContainer";
            this.imgPageContainer.Size = new System.Drawing.Size(396, 358);
            this.imgPageContainer.TabIndex = 0;
            this.imgPageContainer.TabStop = false;
            this.imgPageContainer.Paint += new System.Windows.Forms.PaintEventHandler(this.imgPageContainer_Paint);
            // 
            // btnPlace
            // 
            this.btnPlace.Location = new System.Drawing.Point(432, 403);
            this.btnPlace.Name = "btnPlace";
            this.btnPlace.Size = new System.Drawing.Size(75, 34);
            this.btnPlace.TabIndex = 11;
            this.btnPlace.Text = "Place";
            this.btnPlace.UseVisualStyleBackColor = true;
            this.btnPlace.Click += new System.EventHandler(this.btnPlace_Click);
            // 
            // ImageList1
            // 
            this.ImageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("ImageList1.ImageStream")));
            this.ImageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.ImageList1.Images.SetKeyName(0, "Map.jpg");
            // 
            // usrPlotProperties
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.gbType);
            this.Controls.Add(this.gbMapScale);
            this.Controls.Add(this.gbPaper);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.GroupBox1);
            this.Controls.Add(this.btnPlace);
            this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "usrPlotProperties";
            this.Size = new System.Drawing.Size(597, 441);
            this.gbType.ResumeLayout(false);
            this.gbType.PerformLayout();
            this.gbMapScale.ResumeLayout(false);
            this.gbMapScale.PerformLayout();
            this.gbPaper.ResumeLayout(false);
            this.gbPaper.PerformLayout();
            this.GroupBox1.ResumeLayout(false);
            this.GroupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.imgPageContainer)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        internal System.Windows.Forms.GroupBox gbType;
        internal System.Windows.Forms.CheckBox cbType;
        internal System.Windows.Forms.ListBox lbType;
        internal System.Windows.Forms.GroupBox gbMapScale;
        internal System.Windows.Forms.ListBox lbMapScalePreDefined;
        internal System.Windows.Forms.TextBox tbMapScaleCustom;
        internal System.Windows.Forms.RadioButton optMapScaleCustom;
        internal System.Windows.Forms.RadioButton optMapScalePreDefined;
        internal System.Windows.Forms.GroupBox gbPaper;
        internal System.Windows.Forms.ListBox lbPaperSize;
        internal System.Windows.Forms.Label lblPaperSize;
        private System.Windows.Forms.Button btnCancel;
        internal System.Windows.Forms.GroupBox GroupBox1;
        internal System.Windows.Forms.Label lblSetMapSizeValue;
        internal System.Windows.Forms.Label lblSetMapScaleValue;
        internal System.Windows.Forms.Label lblSetBorderInsetValue;
        internal System.Windows.Forms.Label lblSetPaperValue;
        internal System.Windows.Forms.Label lblSetMapSize;
        internal System.Windows.Forms.Label lblSetMapScale;
        internal System.Windows.Forms.Label lblBorderInset;
        internal System.Windows.Forms.Label lblSetPaper;
        internal System.Windows.Forms.PictureBox imgPageContainer;
        private System.Windows.Forms.Button btnPlace;
        internal System.Windows.Forms.RadioButton optPaperOrientationLandbase;
        internal System.Windows.Forms.RadioButton optPaperOrientationPortrait;
        internal System.Windows.Forms.ImageList ImageList1;
    }
}
