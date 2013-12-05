namespace AG.GTechnology.PlottingBnd
{
    partial class frmMain
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMain));
            this.btnPlace = new System.Windows.Forms.Button();
            this.tabPlotTemplate = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.gbType = new System.Windows.Forms.GroupBox();
            this.cbType = new System.Windows.Forms.CheckBox();
            this.lbType = new System.Windows.Forms.ListBox();
            this.gbMapScale = new System.Windows.Forms.GroupBox();
            this.lbMapScalePreDefined = new System.Windows.Forms.ListBox();
            this.tbMapScaleCustom = new System.Windows.Forms.TextBox();
            this.optMapScaleCustom = new System.Windows.Forms.RadioButton();
            this.optMapScalePreDefined = new System.Windows.Forms.RadioButton();
            this.gbPaper = new System.Windows.Forms.GroupBox();
            this.gbPaperOrientation = new System.Windows.Forms.GroupBox();
            this.optPaperOrientationLandbase = new System.Windows.Forms.RadioButton();
            this.optPaperOrientationPortrait = new System.Windows.Forms.RadioButton();
            this.lbPaperSize = new System.Windows.Forms.ListBox();
            this.lblPaperSize = new System.Windows.Forms.Label();
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
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.cmdRemoveAll = new System.Windows.Forms.Button();
            this.cmdSelectAll = new System.Windows.Forms.Button();
            this.cmdRemoveSingle = new System.Windows.Forms.Button();
            this.cmdSelectSingle = new System.Windows.Forms.Button();
            this.gbActiveSessionInfo = new System.Windows.Forms.GroupBox();
            this.lblActiveSessionInfoCapitalWorkOrderNumberValue = new System.Windows.Forms.Label();
            this.lblActiveSessionInfoJobNumberValue = new System.Windows.Forms.Label();
            this.lblActiveSessionInfoUserValue = new System.Windows.Forms.Label();
            this.lblActiveSessionInfoCapitalWorkOrderNumber = new System.Windows.Forms.Label();
            this.lblActiveSessionInfoJobNumber = new System.Windows.Forms.Label();
            this.lblActiveSessionInfoUser = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.tvAvailablePlotBoundaries = new System.Windows.Forms.TreeView();
            this.gbSelectedPlotBoundaries = new System.Windows.Forms.GroupBox();
            this.treeView1 = new System.Windows.Forms.TreeView();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.btnCancel = new System.Windows.Forms.Button();
            this.ImageList1 = new System.Windows.Forms.ImageList(this.components);
            this.btnCreatePlotWindows = new System.Windows.Forms.Button();
            this.tvSelectedPlotBoundaries = new System.Windows.Forms.TreeView();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.txtExchange = new System.Windows.Forms.TextBox();
            this.txtScale = new System.Windows.Forms.TextBox();
            this.txtPreparedBy = new System.Windows.Forms.TextBox();
            this.txtRevisedBy = new System.Windows.Forms.TextBox();
            this.txtPoject1 = new System.Windows.Forms.TextBox();
            this.txtPoject2 = new System.Windows.Forms.TextBox();
            this.txtPoject3 = new System.Windows.Forms.TextBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.txtNoPainting = new System.Windows.Forms.TextBox();
            this.txtNameDesignation = new System.Windows.Forms.TextBox();
            this.txtApproved = new System.Windows.Forms.TextBox();
            this.txtPoject4 = new System.Windows.Forms.TextBox();
            this.tabPlotTemplate.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.gbType.SuspendLayout();
            this.gbMapScale.SuspendLayout();
            this.gbPaper.SuspendLayout();
            this.gbPaperOrientation.SuspendLayout();
            this.GroupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.imgPageContainer)).BeginInit();
            this.tabPage2.SuspendLayout();
            this.gbActiveSessionInfo.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.gbSelectedPlotBoundaries.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnPlace
            // 
            this.btnPlace.Location = new System.Drawing.Point(425, 408);
            this.btnPlace.Name = "btnPlace";
            this.btnPlace.Size = new System.Drawing.Size(75, 23);
            this.btnPlace.TabIndex = 1;
            this.btnPlace.Text = "Place";
            this.btnPlace.UseVisualStyleBackColor = true;
            this.btnPlace.Click += new System.EventHandler(this.btnPlace_Click);
            // 
            // tabPlotTemplate
            // 
            this.tabPlotTemplate.Controls.Add(this.tabPage1);
            this.tabPlotTemplate.Controls.Add(this.tabPage2);
            this.tabPlotTemplate.Controls.Add(this.tabPage3);
            this.tabPlotTemplate.Location = new System.Drawing.Point(3, 1);
            this.tabPlotTemplate.Name = "tabPlotTemplate";
            this.tabPlotTemplate.SelectedIndex = 0;
            this.tabPlotTemplate.Size = new System.Drawing.Size(610, 474);
            this.tabPlotTemplate.TabIndex = 2;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.gbType);
            this.tabPage1.Controls.Add(this.gbMapScale);
            this.tabPage1.Controls.Add(this.gbPaper);
            this.tabPage1.Controls.Add(this.btnCancel);
            this.tabPage1.Controls.Add(this.GroupBox1);
            this.tabPage1.Controls.Add(this.btnPlace);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(602, 448);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "User Defined Plot";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // gbType
            // 
            this.gbType.Controls.Add(this.cbType);
            this.gbType.Controls.Add(this.lbType);
            this.gbType.Location = new System.Drawing.Point(425, 5);
            this.gbType.Name = "gbType";
            this.gbType.Size = new System.Drawing.Size(170, 100);
            this.gbType.TabIndex = 9;
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
            this.gbMapScale.Location = new System.Drawing.Point(425, 243);
            this.gbMapScale.Name = "gbMapScale";
            this.gbMapScale.Size = new System.Drawing.Size(170, 152);
            this.gbMapScale.TabIndex = 8;
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
            this.tbMapScaleCustom.Size = new System.Drawing.Size(148, 20);
            this.tbMapScaleCustom.TabIndex = 1;
            this.tbMapScaleCustom.TextChanged += new System.EventHandler(this.tbMapScaleCustom_TextChanged);
            // 
            // optMapScaleCustom
            // 
            this.optMapScaleCustom.AutoSize = true;
            this.optMapScaleCustom.Location = new System.Drawing.Point(10, 101);
            this.optMapScaleCustom.Name = "optMapScaleCustom";
            this.optMapScaleCustom.Size = new System.Drawing.Size(60, 17);
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
            this.optMapScalePreDefined.Size = new System.Drawing.Size(82, 17);
            this.optMapScalePreDefined.TabIndex = 0;
            this.optMapScalePreDefined.TabStop = true;
            this.optMapScalePreDefined.Text = "Pre-defined:";
            this.optMapScalePreDefined.UseVisualStyleBackColor = true;
            this.optMapScalePreDefined.CheckedChanged += new System.EventHandler(this.optMapScalePreDefined_CheckedChanged);
            // 
            // gbPaper
            // 
            this.gbPaper.Controls.Add(this.lbPaperSize);
            this.gbPaper.Controls.Add(this.lblPaperSize);
            this.gbPaper.Location = new System.Drawing.Point(425, 111);
            this.gbPaper.Name = "gbPaper";
            this.gbPaper.Size = new System.Drawing.Size(170, 117);
            this.gbPaper.TabIndex = 7;
            this.gbPaper.TabStop = false;
            this.gbPaper.Text = "Paper:";
            // 
            // gbPaperOrientation
            // 
            this.gbPaperOrientation.Controls.Add(this.optPaperOrientationLandbase);
            this.gbPaperOrientation.Controls.Add(this.optPaperOrientationPortrait);
            this.gbPaperOrientation.Location = new System.Drawing.Point(751, 241);
            this.gbPaperOrientation.Name = "gbPaperOrientation";
            this.gbPaperOrientation.Size = new System.Drawing.Size(148, 60);
            this.gbPaperOrientation.TabIndex = 2;
            this.gbPaperOrientation.TabStop = false;
            this.gbPaperOrientation.Text = "Orientation:";
            this.gbPaperOrientation.Visible = false;
            // 
            // optPaperOrientationLandbase
            // 
            this.optPaperOrientationLandbase.AutoSize = true;
            this.optPaperOrientationLandbase.Checked = true;
            this.optPaperOrientationLandbase.Location = new System.Drawing.Point(7, 37);
            this.optPaperOrientationLandbase.Name = "optPaperOrientationLandbase";
            this.optPaperOrientationLandbase.Size = new System.Drawing.Size(78, 17);
            this.optPaperOrientationLandbase.TabIndex = 0;
            this.optPaperOrientationLandbase.TabStop = true;
            this.optPaperOrientationLandbase.Text = "Landscape";
            this.optPaperOrientationLandbase.UseVisualStyleBackColor = true;
            this.optPaperOrientationLandbase.CheckedChanged += new System.EventHandler(this.optPaperOrientationLandbase_CheckedChanged);
            // 
            // optPaperOrientationPortrait
            // 
            this.optPaperOrientationPortrait.AutoSize = true;
            this.optPaperOrientationPortrait.Enabled = false;
            this.optPaperOrientationPortrait.Location = new System.Drawing.Point(7, 20);
            this.optPaperOrientationPortrait.Name = "optPaperOrientationPortrait";
            this.optPaperOrientationPortrait.Size = new System.Drawing.Size(58, 17);
            this.optPaperOrientationPortrait.TabIndex = 0;
            this.optPaperOrientationPortrait.Text = "Portrait";
            this.optPaperOrientationPortrait.UseVisualStyleBackColor = true;
            this.optPaperOrientationPortrait.CheckedChanged += new System.EventHandler(this.optPaperOrientationPortrait_CheckedChanged);
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
            this.lbPaperSize.Location = new System.Drawing.Point(10, 37);
            this.lbPaperSize.Name = "lbPaperSize";
            this.lbPaperSize.Size = new System.Drawing.Size(148, 69);
            this.lbPaperSize.TabIndex = 1;
            this.lbPaperSize.SelectedIndexChanged += new System.EventHandler(this.lbPaperSize_SelectedIndexChanged);
            // 
            // lblPaperSize
            // 
            this.lblPaperSize.AutoSize = true;
            this.lblPaperSize.Location = new System.Drawing.Point(7, 20);
            this.lblPaperSize.Name = "lblPaperSize";
            this.lblPaperSize.Size = new System.Drawing.Size(30, 13);
            this.lblPaperSize.TabIndex = 0;
            this.lblPaperSize.Text = "Size:";
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
            this.GroupBox1.Location = new System.Drawing.Point(6, 5);
            this.GroupBox1.Name = "GroupBox1";
            this.GroupBox1.Size = new System.Drawing.Size(413, 437);
            this.GroupBox1.TabIndex = 6;
            this.GroupBox1.TabStop = false;
            // 
            // lblSetMapSizeValue
            // 
            this.lblSetMapSizeValue.AutoSize = true;
            this.lblSetMapSizeValue.Location = new System.Drawing.Point(162, 416);
            this.lblSetMapSizeValue.Name = "lblSetMapSizeValue";
            this.lblSetMapSizeValue.Size = new System.Drawing.Size(92, 13);
            this.lblSetMapSizeValue.TabIndex = 8;
            this.lblSetMapSizeValue.Text = "61.4mm x 39.1mm";
            // 
            // lblSetMapScaleValue
            // 
            this.lblSetMapScaleValue.AutoSize = true;
            this.lblSetMapScaleValue.Location = new System.Drawing.Point(162, 403);
            this.lblSetMapScaleValue.Name = "lblSetMapScaleValue";
            this.lblSetMapScaleValue.Size = new System.Drawing.Size(34, 13);
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
            this.lblSetPaperValue.Size = new System.Drawing.Size(107, 13);
            this.lblSetPaperValue.TabIndex = 5;
            this.lblSetPaperValue.Text = "8.5 x 11 (Landscape)";
            // 
            // lblSetMapSize
            // 
            this.lblSetMapSize.AutoSize = true;
            this.lblSetMapSize.Location = new System.Drawing.Point(6, 416);
            this.lblSetMapSize.Name = "lblSetMapSize";
            this.lblSetMapSize.Size = new System.Drawing.Size(51, 13);
            this.lblSetMapSize.TabIndex = 4;
            this.lblSetMapSize.Text = "Map Size";
            // 
            // lblSetMapScale
            // 
            this.lblSetMapScale.AutoSize = true;
            this.lblSetMapScale.Location = new System.Drawing.Point(6, 403);
            this.lblSetMapScale.Name = "lblSetMapScale";
            this.lblSetMapScale.Size = new System.Drawing.Size(58, 13);
            this.lblSetMapScale.TabIndex = 3;
            this.lblSetMapScale.Text = "Map Scale";
            // 
            // lblBorderInset
            // 
            this.lblBorderInset.AutoSize = true;
            this.lblBorderInset.Location = new System.Drawing.Point(6, 390);
            this.lblBorderInset.Name = "lblBorderInset";
            this.lblBorderInset.Size = new System.Drawing.Size(67, 13);
            this.lblBorderInset.TabIndex = 2;
            this.lblBorderInset.Text = "Border Inset:";
            // 
            // lblSetPaper
            // 
            this.lblSetPaper.AutoSize = true;
            this.lblSetPaper.Location = new System.Drawing.Point(6, 377);
            this.lblSetPaper.Name = "lblSetPaper";
            this.lblSetPaper.Size = new System.Drawing.Size(38, 13);
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
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.cmdRemoveAll);
            this.tabPage2.Controls.Add(this.cmdSelectAll);
            this.tabPage2.Controls.Add(this.cmdRemoveSingle);
            this.tabPage2.Controls.Add(this.cmdSelectSingle);
            this.tabPage2.Controls.Add(this.gbActiveSessionInfo);
            this.tabPage2.Controls.Add(this.groupBox2);
            this.tabPage2.Controls.Add(this.gbSelectedPlotBoundaries);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(602, 448);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Plot Boundary";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // cmdRemoveAll
            // 
            this.cmdRemoveAll.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmdRemoveAll.Location = new System.Drawing.Point(163, 270);
            this.cmdRemoveAll.Name = "cmdRemoveAll";
            this.cmdRemoveAll.Size = new System.Drawing.Size(34, 24);
            this.cmdRemoveAll.TabIndex = 21;
            this.cmdRemoveAll.Text = "<<";
            this.cmdRemoveAll.UseVisualStyleBackColor = true;
            // 
            // cmdSelectAll
            // 
            this.cmdSelectAll.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmdSelectAll.Location = new System.Drawing.Point(163, 180);
            this.cmdSelectAll.Name = "cmdSelectAll";
            this.cmdSelectAll.Size = new System.Drawing.Size(34, 24);
            this.cmdSelectAll.TabIndex = 20;
            this.cmdSelectAll.Text = ">>";
            this.cmdSelectAll.UseVisualStyleBackColor = true;
            // 
            // cmdRemoveSingle
            // 
            this.cmdRemoveSingle.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmdRemoveSingle.Location = new System.Drawing.Point(163, 240);
            this.cmdRemoveSingle.Name = "cmdRemoveSingle";
            this.cmdRemoveSingle.Size = new System.Drawing.Size(34, 24);
            this.cmdRemoveSingle.TabIndex = 19;
            this.cmdRemoveSingle.Text = "<";
            this.cmdRemoveSingle.UseVisualStyleBackColor = true;
            // 
            // cmdSelectSingle
            // 
            this.cmdSelectSingle.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmdSelectSingle.Location = new System.Drawing.Point(163, 210);
            this.cmdSelectSingle.Name = "cmdSelectSingle";
            this.cmdSelectSingle.Size = new System.Drawing.Size(34, 24);
            this.cmdSelectSingle.TabIndex = 18;
            this.cmdSelectSingle.Text = ">";
            this.cmdSelectSingle.UseVisualStyleBackColor = true;
            // 
            // gbActiveSessionInfo
            // 
            this.gbActiveSessionInfo.Controls.Add(this.lblActiveSessionInfoCapitalWorkOrderNumberValue);
            this.gbActiveSessionInfo.Controls.Add(this.lblActiveSessionInfoJobNumberValue);
            this.gbActiveSessionInfo.Controls.Add(this.lblActiveSessionInfoUserValue);
            this.gbActiveSessionInfo.Controls.Add(this.lblActiveSessionInfoCapitalWorkOrderNumber);
            this.gbActiveSessionInfo.Controls.Add(this.lblActiveSessionInfoJobNumber);
            this.gbActiveSessionInfo.Controls.Add(this.lblActiveSessionInfoUser);
            this.gbActiveSessionInfo.Location = new System.Drawing.Point(6, 6);
            this.gbActiveSessionInfo.Name = "gbActiveSessionInfo";
            this.gbActiveSessionInfo.Size = new System.Drawing.Size(310, 80);
            this.gbActiveSessionInfo.TabIndex = 15;
            this.gbActiveSessionInfo.TabStop = false;
            this.gbActiveSessionInfo.Text = "Active Session Information:";
            // 
            // lblActiveSessionInfoCapitalWorkOrderNumberValue
            // 
            this.lblActiveSessionInfoCapitalWorkOrderNumberValue.AutoSize = true;
            this.lblActiveSessionInfoCapitalWorkOrderNumberValue.ForeColor = System.Drawing.SystemColors.Highlight;
            this.lblActiveSessionInfoCapitalWorkOrderNumberValue.Location = new System.Drawing.Point(122, 60);
            this.lblActiveSessionInfoCapitalWorkOrderNumberValue.Name = "lblActiveSessionInfoCapitalWorkOrderNumberValue";
            this.lblActiveSessionInfoCapitalWorkOrderNumberValue.Size = new System.Drawing.Size(123, 13);
            this.lblActiveSessionInfoCapitalWorkOrderNumberValue.TabIndex = 8;
            this.lblActiveSessionInfoCapitalWorkOrderNumberValue.Text = "WorkOrderNumberValue";
            // 
            // lblActiveSessionInfoJobNumberValue
            // 
            this.lblActiveSessionInfoJobNumberValue.AutoSize = true;
            this.lblActiveSessionInfoJobNumberValue.ForeColor = System.Drawing.SystemColors.Highlight;
            this.lblActiveSessionInfoJobNumberValue.Location = new System.Drawing.Point(122, 41);
            this.lblActiveSessionInfoJobNumberValue.Name = "lblActiveSessionInfoJobNumberValue";
            this.lblActiveSessionInfoJobNumberValue.Size = new System.Drawing.Size(88, 13);
            this.lblActiveSessionInfoJobNumberValue.TabIndex = 7;
            this.lblActiveSessionInfoJobNumberValue.Text = "JobNumberValue";
            // 
            // lblActiveSessionInfoUserValue
            // 
            this.lblActiveSessionInfoUserValue.AutoSize = true;
            this.lblActiveSessionInfoUserValue.ForeColor = System.Drawing.SystemColors.Highlight;
            this.lblActiveSessionInfoUserValue.Location = new System.Drawing.Point(122, 23);
            this.lblActiveSessionInfoUserValue.Name = "lblActiveSessionInfoUserValue";
            this.lblActiveSessionInfoUserValue.Size = new System.Drawing.Size(56, 13);
            this.lblActiveSessionInfoUserValue.TabIndex = 6;
            this.lblActiveSessionInfoUserValue.Text = "UserValue";
            // 
            // lblActiveSessionInfoCapitalWorkOrderNumber
            // 
            this.lblActiveSessionInfoCapitalWorkOrderNumber.AutoSize = true;
            this.lblActiveSessionInfoCapitalWorkOrderNumber.Location = new System.Drawing.Point(6, 60);
            this.lblActiveSessionInfoCapitalWorkOrderNumber.Name = "lblActiveSessionInfoCapitalWorkOrderNumber";
            this.lblActiveSessionInfoCapitalWorkOrderNumber.Size = new System.Drawing.Size(105, 13);
            this.lblActiveSessionInfoCapitalWorkOrderNumber.TabIndex = 2;
            this.lblActiveSessionInfoCapitalWorkOrderNumber.Text = "Work Order Number:";
            // 
            // lblActiveSessionInfoJobNumber
            // 
            this.lblActiveSessionInfoJobNumber.AutoSize = true;
            this.lblActiveSessionInfoJobNumber.Location = new System.Drawing.Point(6, 41);
            this.lblActiveSessionInfoJobNumber.Name = "lblActiveSessionInfoJobNumber";
            this.lblActiveSessionInfoJobNumber.Size = new System.Drawing.Size(67, 13);
            this.lblActiveSessionInfoJobNumber.TabIndex = 1;
            this.lblActiveSessionInfoJobNumber.Text = "Job Number:";
            // 
            // lblActiveSessionInfoUser
            // 
            this.lblActiveSessionInfoUser.AutoSize = true;
            this.lblActiveSessionInfoUser.Location = new System.Drawing.Point(6, 22);
            this.lblActiveSessionInfoUser.Name = "lblActiveSessionInfoUser";
            this.lblActiveSessionInfoUser.Size = new System.Drawing.Size(32, 13);
            this.lblActiveSessionInfoUser.TabIndex = 0;
            this.lblActiveSessionInfoUser.Text = "User:";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.tvAvailablePlotBoundaries);
            this.groupBox2.Location = new System.Drawing.Point(6, 90);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(154, 352);
            this.groupBox2.TabIndex = 16;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Available Plot Boundaries";
            // 
            // tvAvailablePlotBoundaries
            // 
            this.tvAvailablePlotBoundaries.Dock = System.Windows.Forms.DockStyle.Left;
            this.tvAvailablePlotBoundaries.FullRowSelect = true;
            this.tvAvailablePlotBoundaries.HideSelection = false;
            this.tvAvailablePlotBoundaries.Indent = 15;
            this.tvAvailablePlotBoundaries.Location = new System.Drawing.Point(3, 16);
            this.tvAvailablePlotBoundaries.Name = "tvAvailablePlotBoundaries";
            this.tvAvailablePlotBoundaries.ShowLines = false;
            this.tvAvailablePlotBoundaries.ShowNodeToolTips = true;
            this.tvAvailablePlotBoundaries.ShowPlusMinus = false;
            this.tvAvailablePlotBoundaries.ShowRootLines = false;
            this.tvAvailablePlotBoundaries.Size = new System.Drawing.Size(148, 333);
            this.tvAvailablePlotBoundaries.TabIndex = 3;
            // 
            // gbSelectedPlotBoundaries
            // 
            this.gbSelectedPlotBoundaries.Controls.Add(this.treeView1);
            this.gbSelectedPlotBoundaries.Location = new System.Drawing.Point(200, 88);
            this.gbSelectedPlotBoundaries.Name = "gbSelectedPlotBoundaries";
            this.gbSelectedPlotBoundaries.Size = new System.Drawing.Size(168, 354);
            this.gbSelectedPlotBoundaries.TabIndex = 17;
            this.gbSelectedPlotBoundaries.TabStop = false;
            this.gbSelectedPlotBoundaries.Text = "Selected Plot Boundaries";
            // 
            // treeView1
            // 
            this.treeView1.Dock = System.Windows.Forms.DockStyle.Left;
            this.treeView1.FullRowSelect = true;
            this.treeView1.HideSelection = false;
            this.treeView1.Indent = 15;
            this.treeView1.Location = new System.Drawing.Point(3, 16);
            this.treeView1.Name = "treeView1";
            this.treeView1.ShowLines = false;
            this.treeView1.ShowPlusMinus = false;
            this.treeView1.ShowRootLines = false;
            this.treeView1.Size = new System.Drawing.Size(158, 335);
            this.treeView1.TabIndex = 14;
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.groupBox3);
            this.tabPage3.Controls.Add(this.btnCreatePlotWindows);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage3.Size = new System.Drawing.Size(602, 448);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "Plot Template";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(516, 408);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 1;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // ImageList1
            // 
            this.ImageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("ImageList1.ImageStream")));
            this.ImageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.ImageList1.Images.SetKeyName(0, "Map.jpg");
            // 
            // btnCreatePlotWindows
            // 
            this.btnCreatePlotWindows.Enabled = false;
            this.btnCreatePlotWindows.Location = new System.Drawing.Point(42, 406);
            this.btnCreatePlotWindows.Name = "btnCreatePlotWindows";
            this.btnCreatePlotWindows.Size = new System.Drawing.Size(75, 23);
            this.btnCreatePlotWindows.TabIndex = 3;
            this.btnCreatePlotWindows.Text = "Create Plot";
            this.btnCreatePlotWindows.UseVisualStyleBackColor = true;
            this.btnCreatePlotWindows.Click += new System.EventHandler(this.btnCreatePlotWindows_Click);
            // 
            // tvSelectedPlotBoundaries
            // 
            this.tvSelectedPlotBoundaries.Dock = System.Windows.Forms.DockStyle.Left;
            this.tvSelectedPlotBoundaries.FullRowSelect = true;
            this.tvSelectedPlotBoundaries.HideSelection = false;
            this.tvSelectedPlotBoundaries.Indent = 15;
            this.tvSelectedPlotBoundaries.LineColor = System.Drawing.Color.Empty;
            this.tvSelectedPlotBoundaries.Location = new System.Drawing.Point(3, 16);
            this.tvSelectedPlotBoundaries.Name = "tvSelectedPlotBoundaries";
            this.tvSelectedPlotBoundaries.ShowLines = false;
            this.tvSelectedPlotBoundaries.ShowPlusMinus = false;
            this.tvSelectedPlotBoundaries.ShowRootLines = false;
            this.tvSelectedPlotBoundaries.Size = new System.Drawing.Size(158, 335);
            this.tvSelectedPlotBoundaries.TabIndex = 14;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(15, 36);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(80, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "IBUSAWAT: ";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(6, 17);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(102, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "TAJUK PROJEK:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(15, 198);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(56, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "Ukuran: ";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(15, 223);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(119, 13);
            this.label4.TabIndex = 3;
            this.label4.Text = "DISEDIAKAN OLEH";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(15, 249);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(99, 13);
            this.label5.TabIndex = 4;
            this.label5.Text = "DISEMAK OLEH";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(15, 275);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(85, 13);
            this.label6.TabIndex = 5;
            this.label6.Text = "DILULUSKAN";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(15, 300);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(159, 13);
            this.label7.TabIndex = 6;
            this.label7.Text = "Nama dan Jawatan Tarikh:";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.Location = new System.Drawing.Point(15, 326);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(79, 13);
            this.label8.TabIndex = 7;
            this.label8.Text = "No Lukisan :";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.txtNoPainting);
            this.groupBox3.Controls.Add(this.txtNameDesignation);
            this.groupBox3.Controls.Add(this.txtApproved);
            this.groupBox3.Controls.Add(this.groupBox4);
            this.groupBox3.Controls.Add(this.txtRevisedBy);
            this.groupBox3.Controls.Add(this.txtPreparedBy);
            this.groupBox3.Controls.Add(this.txtScale);
            this.groupBox3.Controls.Add(this.txtExchange);
            this.groupBox3.Controls.Add(this.label1);
            this.groupBox3.Controls.Add(this.label8);
            this.groupBox3.Controls.Add(this.label7);
            this.groupBox3.Controls.Add(this.label3);
            this.groupBox3.Controls.Add(this.label6);
            this.groupBox3.Controls.Add(this.label4);
            this.groupBox3.Controls.Add(this.label5);
            this.groupBox3.Location = new System.Drawing.Point(33, 21);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(476, 358);
            this.groupBox3.TabIndex = 8;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "ACCESS NETWORK DEVELOPMENT (AND) KAWASAN A";
            // 
            // txtExchange
            // 
            this.txtExchange.Location = new System.Drawing.Point(184, 33);
            this.txtExchange.Name = "txtExchange";
            this.txtExchange.Size = new System.Drawing.Size(250, 20);
            this.txtExchange.TabIndex = 8;
            // 
            // txtScale
            // 
            this.txtScale.Location = new System.Drawing.Point(184, 195);
            this.txtScale.Name = "txtScale";
            this.txtScale.Size = new System.Drawing.Size(250, 20);
            this.txtScale.TabIndex = 9;
            // 
            // txtPreparedBy
            // 
            this.txtPreparedBy.Location = new System.Drawing.Point(184, 220);
            this.txtPreparedBy.Name = "txtPreparedBy";
            this.txtPreparedBy.Size = new System.Drawing.Size(250, 20);
            this.txtPreparedBy.TabIndex = 10;
            // 
            // txtRevisedBy
            // 
            this.txtRevisedBy.Location = new System.Drawing.Point(184, 246);
            this.txtRevisedBy.Name = "txtRevisedBy";
            this.txtRevisedBy.Size = new System.Drawing.Size(250, 20);
            this.txtRevisedBy.TabIndex = 11;
            // 
            // txtPoject1
            // 
            this.txtPoject1.Location = new System.Drawing.Point(175, 14);
            this.txtPoject1.Name = "txtPoject1";
            this.txtPoject1.Size = new System.Drawing.Size(250, 20);
            this.txtPoject1.TabIndex = 12;
            // 
            // txtPoject2
            // 
            this.txtPoject2.Location = new System.Drawing.Point(175, 40);
            this.txtPoject2.Name = "txtPoject2";
            this.txtPoject2.Size = new System.Drawing.Size(250, 20);
            this.txtPoject2.TabIndex = 13;
            // 
            // txtPoject3
            // 
            this.txtPoject3.Location = new System.Drawing.Point(175, 66);
            this.txtPoject3.Name = "txtPoject3";
            this.txtPoject3.Size = new System.Drawing.Size(250, 20);
            this.txtPoject3.TabIndex = 14;
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.txtPoject4);
            this.groupBox4.Controls.Add(this.label2);
            this.groupBox4.Controls.Add(this.txtPoject3);
            this.groupBox4.Controls.Add(this.txtPoject1);
            this.groupBox4.Controls.Add(this.txtPoject2);
            this.groupBox4.Location = new System.Drawing.Point(9, 59);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(442, 122);
            this.groupBox4.TabIndex = 15;
            this.groupBox4.TabStop = false;
            // 
            // txtNoPainting
            // 
            this.txtNoPainting.Location = new System.Drawing.Point(184, 323);
            this.txtNoPainting.Name = "txtNoPainting";
            this.txtNoPainting.Size = new System.Drawing.Size(250, 20);
            this.txtNoPainting.TabIndex = 18;
            // 
            // txtNameDesignation
            // 
            this.txtNameDesignation.Location = new System.Drawing.Point(184, 297);
            this.txtNameDesignation.Name = "txtNameDesignation";
            this.txtNameDesignation.Size = new System.Drawing.Size(250, 20);
            this.txtNameDesignation.TabIndex = 17;
            // 
            // txtApproved
            // 
            this.txtApproved.Location = new System.Drawing.Point(184, 272);
            this.txtApproved.Name = "txtApproved";
            this.txtApproved.Size = new System.Drawing.Size(250, 20);
            this.txtApproved.TabIndex = 16;
            // 
            // txtPoject4
            // 
            this.txtPoject4.Location = new System.Drawing.Point(175, 92);
            this.txtPoject4.Name = "txtPoject4";
            this.txtPoject4.Size = new System.Drawing.Size(250, 20);
            this.txtPoject4.TabIndex = 15;
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(616, 479);
            this.Controls.Add(this.gbPaperOrientation);
            this.Controls.Add(this.tabPlotTemplate);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "frmMain";
            this.Text = "Plotting ver 2.3";
            this.Load += new System.EventHandler(this.frmMain_Load);
            this.tabPlotTemplate.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.gbType.ResumeLayout(false);
            this.gbType.PerformLayout();
            this.gbMapScale.ResumeLayout(false);
            this.gbMapScale.PerformLayout();
            this.gbPaper.ResumeLayout(false);
            this.gbPaper.PerformLayout();
            this.gbPaperOrientation.ResumeLayout(false);
            this.gbPaperOrientation.PerformLayout();
            this.GroupBox1.ResumeLayout(false);
            this.GroupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.imgPageContainer)).EndInit();
            this.tabPage2.ResumeLayout(false);
            this.gbActiveSessionInfo.ResumeLayout(false);
            this.gbActiveSessionInfo.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.gbSelectedPlotBoundaries.ResumeLayout(false);
            this.tabPage3.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnPlace;
        private System.Windows.Forms.TabControl tabPlotTemplate;
        private System.Windows.Forms.TabPage tabPage1;
        internal System.Windows.Forms.GroupBox gbType;
        internal System.Windows.Forms.CheckBox cbType;
        internal System.Windows.Forms.ListBox lbType;
        internal System.Windows.Forms.GroupBox gbMapScale;
        internal System.Windows.Forms.ListBox lbMapScalePreDefined;
        internal System.Windows.Forms.TextBox tbMapScaleCustom;
        internal System.Windows.Forms.RadioButton optMapScaleCustom;
        internal System.Windows.Forms.RadioButton optMapScalePreDefined;
        internal System.Windows.Forms.GroupBox gbPaper;
        internal System.Windows.Forms.GroupBox gbPaperOrientation;
        internal System.Windows.Forms.RadioButton optPaperOrientationLandbase;
        internal System.Windows.Forms.RadioButton optPaperOrientationPortrait;
        internal System.Windows.Forms.ListBox lbPaperSize;
        internal System.Windows.Forms.Label lblPaperSize;
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
        private System.Windows.Forms.Button btnCancel;
        internal System.Windows.Forms.ImageList ImageList1;
        private System.Windows.Forms.TabPage tabPage2;
        internal System.Windows.Forms.Button btnCreatePlotWindows;
        internal System.Windows.Forms.TreeView tvSelectedPlotBoundaries;
        internal System.Windows.Forms.Button cmdRemoveAll;
        internal System.Windows.Forms.Button cmdSelectAll;
        internal System.Windows.Forms.Button cmdRemoveSingle;
        internal System.Windows.Forms.Button cmdSelectSingle;
        internal System.Windows.Forms.GroupBox gbActiveSessionInfo;
        internal System.Windows.Forms.Label lblActiveSessionInfoCapitalWorkOrderNumberValue;
        internal System.Windows.Forms.Label lblActiveSessionInfoJobNumberValue;
        internal System.Windows.Forms.Label lblActiveSessionInfoUserValue;
        internal System.Windows.Forms.Label lblActiveSessionInfoCapitalWorkOrderNumber;
        internal System.Windows.Forms.Label lblActiveSessionInfoJobNumber;
        internal System.Windows.Forms.Label lblActiveSessionInfoUser;
        internal System.Windows.Forms.GroupBox groupBox2;
        internal System.Windows.Forms.TreeView tvAvailablePlotBoundaries;
        internal System.Windows.Forms.GroupBox gbSelectedPlotBoundaries;
        internal System.Windows.Forms.TreeView treeView1;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox txtExchange;
        private System.Windows.Forms.TextBox txtNoPainting;
        private System.Windows.Forms.TextBox txtNameDesignation;
        private System.Windows.Forms.TextBox txtApproved;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.TextBox txtPoject4;
        private System.Windows.Forms.TextBox txtPoject3;
        private System.Windows.Forms.TextBox txtPoject1;
        private System.Windows.Forms.TextBox txtPoject2;
        private System.Windows.Forms.TextBox txtRevisedBy;
        private System.Windows.Forms.TextBox txtPreparedBy;
        private System.Windows.Forms.TextBox txtScale;
    }
}