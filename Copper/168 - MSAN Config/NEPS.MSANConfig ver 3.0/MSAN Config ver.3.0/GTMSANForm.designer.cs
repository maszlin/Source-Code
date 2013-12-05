namespace NEPS.GTechnology.MSANConfig
{
    partial class GTMSANForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GTMSANForm));
            System.Windows.Forms.TreeNode treeNode7 = new System.Windows.Forms.TreeNode("Slot 01");
            System.Windows.Forms.TreeNode treeNode8 = new System.Windows.Forms.TreeNode("Slot 02");
            System.Windows.Forms.TreeNode treeNode9 = new System.Windows.Forms.TreeNode("Slot 03");
            System.Windows.Forms.TreeNode treeNode10 = new System.Windows.Forms.TreeNode("Shelf 01", new System.Windows.Forms.TreeNode[] {
            treeNode7,
            treeNode8,
            treeNode9});
            System.Windows.Forms.TreeNode treeNode11 = new System.Windows.Forms.TreeNode("Shelf 02");
            System.Windows.Forms.TreeNode treeNode12 = new System.Windows.Forms.TreeNode("", new System.Windows.Forms.TreeNode[] {
            treeNode10,
            treeNode11});
            this.pnlTop = new System.Windows.Forms.Panel();
            this.txtStatus = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.txtType = new System.Windows.Forms.TextBox();
            this.txtModel = new System.Windows.Forms.TextBox();
            this.txtManufacturer = new System.Windows.Forms.TextBox();
            this.label29 = new System.Windows.Forms.Label();
            this.label28 = new System.Windows.Forms.Label();
            this.lblType = new System.Windows.Forms.Label();
            this.txtCode = new System.Windows.Forms.TextBox();
            this.lblCode = new System.Windows.Forms.Label();
            this.txtExchange = new System.Windows.Forms.TextBox();
            this.label20 = new System.Windows.Forms.Label();
            this.btnHeader = new System.Windows.Forms.Button();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.trvCard = new System.Windows.Forms.TreeView();
            this.grdMSAN = new System.Windows.Forms.DataGridView();
            this.colSlotNo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colSlotNIS = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colPlantUnit = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.colCardTypeCombo = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.colCardModelCombo = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.colCardStatus = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.colPortLo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colPortHi = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colGEMS = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colBOM = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ctxMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.ctxCopy = new System.Windows.Forms.ToolStripMenuItem();
            this.ctxPaste = new System.Windows.Forms.ToolStripMenuItem();
            this.ctxClear = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
            this.ctxFillCol = new System.Windows.Forms.ToolStripMenuItem();
            this.ctxAddRow = new System.Windows.Forms.ToolStripMenuItem();
            this.ctxDelRow = new System.Windows.Forms.ToolStripMenuItem();
            this.btnShelfLoad = new System.Windows.Forms.Button();
            this.txtMSANRack = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnShelfDel = new System.Windows.Forms.Button();
            this.btnShelfAdd = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.cmbShelf = new System.Windows.Forms.ComboBox();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.shelfToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuShelfAdd = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuShelfDel = new System.Windows.Forms.ToolStripMenuItem();
            this.slotToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuSlotAdd = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuSlotDel = new System.Windows.Forms.ToolStripMenuItem();
            this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuCopy = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuPaste = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuFill = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuClear = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuRollback = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuReload = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuCommit = new System.Windows.Forms.ToolStripMenuItem();
            this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader3 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader4 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader5 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader6 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader7 = new System.Windows.Forms.ColumnHeader();
            this.pnlTemplate = new System.Windows.Forms.Panel();
            this.btnApplySelected = new System.Windows.Forms.Button();
            this.cmbMSANTemplate = new System.Windows.Forms.ComboBox();
            this.btnApply = new System.Windows.Forms.Button();
            this.lsvTemplate = new System.Windows.Forms.ListView();
            this.columnHeader15 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader16 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader17 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader18 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader19 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader21 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader20 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader22 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader8 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader9 = new System.Windows.Forms.ColumnHeader();
            this.label26 = new System.Windows.Forms.Label();
            this.btnTemplate = new System.Windows.Forms.Button();
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.btnDetail = new System.Windows.Forms.Button();
            this.pnlTop.SuspendLayout();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grdMSAN)).BeginInit();
            this.ctxMenu.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.pnlTemplate.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlTop
            // 
            this.pnlTop.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.pnlTop.Controls.Add(this.txtStatus);
            this.pnlTop.Controls.Add(this.label4);
            this.pnlTop.Controls.Add(this.txtType);
            this.pnlTop.Controls.Add(this.txtModel);
            this.pnlTop.Controls.Add(this.txtManufacturer);
            this.pnlTop.Controls.Add(this.label29);
            this.pnlTop.Controls.Add(this.label28);
            this.pnlTop.Controls.Add(this.lblType);
            this.pnlTop.Controls.Add(this.txtCode);
            this.pnlTop.Controls.Add(this.lblCode);
            this.pnlTop.Controls.Add(this.txtExchange);
            this.pnlTop.Controls.Add(this.label20);
            this.pnlTop.Controls.Add(this.btnHeader);
            this.pnlTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlTop.Location = new System.Drawing.Point(0, 0);
            this.pnlTop.Name = "pnlTop";
            this.pnlTop.Size = new System.Drawing.Size(935, 89);
            this.pnlTop.TabIndex = 0;
            // 
            // txtStatus
            // 
            this.txtStatus.Location = new System.Drawing.Point(102, 59);
            this.txtStatus.Name = "txtStatus";
            this.txtStatus.ReadOnly = true;
            this.txtStatus.Size = new System.Drawing.Size(109, 21);
            this.txtStatus.TabIndex = 61;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.ForeColor = System.Drawing.SystemColors.ActiveCaption;
            this.label4.Location = new System.Drawing.Point(10, 62);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(44, 13);
            this.label4.TabIndex = 60;
            this.label4.Text = "Status";
            // 
            // txtType
            // 
            this.txtType.Location = new System.Drawing.Point(352, 59);
            this.txtType.Name = "txtType";
            this.txtType.ReadOnly = true;
            this.txtType.Size = new System.Drawing.Size(122, 21);
            this.txtType.TabIndex = 59;
            // 
            // txtModel
            // 
            this.txtModel.Location = new System.Drawing.Point(599, 59);
            this.txtModel.Name = "txtModel";
            this.txtModel.ReadOnly = true;
            this.txtModel.Size = new System.Drawing.Size(149, 21);
            this.txtModel.TabIndex = 55;
            // 
            // txtManufacturer
            // 
            this.txtManufacturer.Location = new System.Drawing.Point(599, 32);
            this.txtManufacturer.Name = "txtManufacturer";
            this.txtManufacturer.ReadOnly = true;
            this.txtManufacturer.Size = new System.Drawing.Size(149, 21);
            this.txtManufacturer.TabIndex = 54;
            // 
            // label29
            // 
            this.label29.AutoSize = true;
            this.label29.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label29.ForeColor = System.Drawing.SystemColors.ActiveCaption;
            this.label29.Location = new System.Drawing.Point(509, 62);
            this.label29.Name = "label29";
            this.label29.Size = new System.Drawing.Size(41, 13);
            this.label29.TabIndex = 38;
            this.label29.Text = "Model";
            // 
            // label28
            // 
            this.label28.AutoSize = true;
            this.label28.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label28.ForeColor = System.Drawing.SystemColors.ActiveCaption;
            this.label28.Location = new System.Drawing.Point(509, 36);
            this.label28.Name = "label28";
            this.label28.Size = new System.Drawing.Size(84, 13);
            this.label28.TabIndex = 37;
            this.label28.Text = "Manufacturer";
            // 
            // lblType
            // 
            this.lblType.AutoSize = true;
            this.lblType.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblType.ForeColor = System.Drawing.SystemColors.ActiveCaption;
            this.lblType.Location = new System.Drawing.Point(244, 62);
            this.lblType.Name = "lblType";
            this.lblType.Size = new System.Drawing.Size(70, 13);
            this.lblType.TabIndex = 36;
            this.lblType.Text = "MSAN Type";
            // 
            // txtCode
            // 
            this.txtCode.Location = new System.Drawing.Point(352, 32);
            this.txtCode.Name = "txtCode";
            this.txtCode.ReadOnly = true;
            this.txtCode.Size = new System.Drawing.Size(122, 21);
            this.txtCode.TabIndex = 34;
            // 
            // lblCode
            // 
            this.lblCode.AutoSize = true;
            this.lblCode.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCode.ForeColor = System.Drawing.SystemColors.ActiveCaption;
            this.lblCode.Location = new System.Drawing.Point(244, 36);
            this.lblCode.Name = "lblCode";
            this.lblCode.Size = new System.Drawing.Size(70, 13);
            this.lblCode.TabIndex = 33;
            this.lblCode.Text = "MSAN Code";
            // 
            // txtExchange
            // 
            this.txtExchange.Location = new System.Drawing.Point(102, 32);
            this.txtExchange.Name = "txtExchange";
            this.txtExchange.ReadOnly = true;
            this.txtExchange.Size = new System.Drawing.Size(109, 21);
            this.txtExchange.TabIndex = 32;
            // 
            // label20
            // 
            this.label20.AutoSize = true;
            this.label20.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label20.ForeColor = System.Drawing.SystemColors.ActiveCaption;
            this.label20.Location = new System.Drawing.Point(10, 36);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(61, 13);
            this.label20.TabIndex = 31;
            this.label20.Text = "Exchange";
            // 
            // btnHeader
            // 
            this.btnHeader.BackColor = System.Drawing.Color.White;
            this.btnHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnHeader.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnHeader.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnHeader.ImageIndex = 1;
            this.btnHeader.ImageList = this.imageList1;
            this.btnHeader.Location = new System.Drawing.Point(0, 0);
            this.btnHeader.Name = "btnHeader";
            this.btnHeader.Size = new System.Drawing.Size(935, 22);
            this.btnHeader.TabIndex = 2;
            this.btnHeader.Text = "General Information";
            this.btnHeader.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnHeader.UseVisualStyleBackColor = false;
            this.btnHeader.Click += new System.EventHandler(this.btnHeader_Click);
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "Button Down.png");
            this.imageList1.Images.SetKeyName(1, "Button Up.png");
            this.imageList1.Images.SetKeyName(2, "Button Down-01.ico");
            this.imageList1.Images.SetKeyName(3, "Button Up-01.ico");
            this.imageList1.Images.SetKeyName(4, "Folder Search-01.ico");
            this.imageList1.Images.SetKeyName(5, "Folder-01.ico");
            // 
            // statusStrip1
            // 
            this.statusStrip1.Location = new System.Drawing.Point(0, 565);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(935, 22);
            this.statusStrip1.TabIndex = 1;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 239);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.trvCard);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.grdMSAN);
            this.splitContainer1.Size = new System.Drawing.Size(935, 326);
            this.splitContainer1.SplitterDistance = 141;
            this.splitContainer1.TabIndex = 63;
            // 
            // trvCard
            // 
            this.trvCard.Dock = System.Windows.Forms.DockStyle.Fill;
            this.trvCard.Location = new System.Drawing.Point(0, 0);
            this.trvCard.Name = "trvCard";
            treeNode7.Name = "Node3";
            treeNode7.Text = "Slot 01";
            treeNode8.Name = "Node4";
            treeNode8.Text = "Slot 02";
            treeNode9.Name = "Node5";
            treeNode9.Text = "Slot 03";
            treeNode10.Name = "Node1";
            treeNode10.Text = "Shelf 01";
            treeNode11.Name = "Node2";
            treeNode11.Text = "Shelf 02";
            treeNode12.Name = "Node0";
            treeNode12.Text = "";
            this.trvCard.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode12});
            this.trvCard.ShowRootLines = false;
            this.trvCard.Size = new System.Drawing.Size(141, 326);
            this.trvCard.TabIndex = 0;
            this.trvCard.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.trvCard_AfterSelect);
            // 
            // grdMSAN
            // 
            this.grdMSAN.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.grdMSAN.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colSlotNo,
            this.colSlotNIS,
            this.colPlantUnit,
            this.colCardTypeCombo,
            this.colCardModelCombo,
            this.colCardStatus,
            this.colPortLo,
            this.colPortHi,
            this.colGEMS,
            this.colBOM});
            this.grdMSAN.ContextMenuStrip = this.ctxMenu;
            this.grdMSAN.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grdMSAN.Location = new System.Drawing.Point(0, 0);
            this.grdMSAN.Name = "grdMSAN";
            this.grdMSAN.Size = new System.Drawing.Size(790, 326);
            this.grdMSAN.TabIndex = 28;
            this.grdMSAN.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.grdMSAN_CellValueChanged);
            this.grdMSAN.RowsAdded += new System.Windows.Forms.DataGridViewRowsAddedEventHandler(this.grdMSAN_RowsAdded);
            this.grdMSAN.EditingControlShowing += new System.Windows.Forms.DataGridViewEditingControlShowingEventHandler(this.HandleEditShowing);
            this.grdMSAN.KeyDown += new System.Windows.Forms.KeyEventHandler(this.grdMSAN_KeyDown);
            this.grdMSAN.CellEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.grdMSAN_CellEnter);
            // 
            // colSlotNo
            // 
            this.colSlotNo.HeaderText = "Slot No";
            this.colSlotNo.Name = "colSlotNo";
            this.colSlotNo.Width = 50;
            // 
            // colSlotNIS
            // 
            this.colSlotNIS.HeaderText = "Slot NIS";
            this.colSlotNIS.Name = "colSlotNIS";
            this.colSlotNIS.Width = 50;
            // 
            // colPlantUnit
            // 
            this.colPlantUnit.HeaderText = "Plant Unit";
            this.colPlantUnit.Name = "colPlantUnit";
            this.colPlantUnit.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.colPlantUnit.Sorted = true;
            this.colPlantUnit.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.colPlantUnit.Width = 230;
            // 
            // colCardTypeCombo
            // 
            this.colCardTypeCombo.HeaderText = "Card Type";
            this.colCardTypeCombo.Name = "colCardTypeCombo";
            this.colCardTypeCombo.ReadOnly = true;
            this.colCardTypeCombo.Width = 130;
            // 
            // colCardModelCombo
            // 
            this.colCardModelCombo.HeaderText = "Card Model";
            this.colCardModelCombo.Name = "colCardModelCombo";
            this.colCardModelCombo.ReadOnly = true;
            // 
            // colCardStatus
            // 
            this.colCardStatus.HeaderText = "Card Status";
            this.colCardStatus.Name = "colCardStatus";
            this.colCardStatus.Sorted = true;
            this.colCardStatus.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.colCardStatus.Width = 80;
            // 
            // colPortLo
            // 
            this.colPortLo.HeaderText = "Port Lo";
            this.colPortLo.Name = "colPortLo";
            this.colPortLo.Width = 50;
            // 
            // colPortHi
            // 
            this.colPortHi.HeaderText = "Port Hi";
            this.colPortHi.Name = "colPortHi";
            this.colPortHi.Width = 50;
            // 
            // colGEMS
            // 
            this.colGEMS.HeaderText = "GEMS Item No";
            this.colGEMS.Name = "colGEMS";
            this.colGEMS.ReadOnly = true;
            this.colGEMS.Visible = false;
            // 
            // colBOM
            // 
            this.colBOM.HeaderText = "Component ID";
            this.colBOM.Name = "colBOM";
            this.colBOM.ReadOnly = true;
            this.colBOM.Visible = false;
            // 
            // ctxMenu
            // 
            this.ctxMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ctxCopy,
            this.ctxPaste,
            this.ctxClear,
            this.toolStripMenuItem2,
            this.ctxFillCol,
            this.ctxAddRow,
            this.ctxDelRow});
            this.ctxMenu.Name = "ctxMenu";
            this.ctxMenu.Size = new System.Drawing.Size(146, 142);
            // 
            // ctxCopy
            // 
            this.ctxCopy.Name = "ctxCopy";
            this.ctxCopy.Size = new System.Drawing.Size(145, 22);
            this.ctxCopy.Text = "Copy";
            this.ctxCopy.Click += new System.EventHandler(this.ctxCopy_Click);
            // 
            // ctxPaste
            // 
            this.ctxPaste.Name = "ctxPaste";
            this.ctxPaste.Size = new System.Drawing.Size(145, 22);
            this.ctxPaste.Text = "Paste";
            this.ctxPaste.Click += new System.EventHandler(this.ctxPaste_Click);
            // 
            // ctxClear
            // 
            this.ctxClear.Name = "ctxClear";
            this.ctxClear.Size = new System.Drawing.Size(145, 22);
            this.ctxClear.Text = "Clear";
            this.ctxClear.Click += new System.EventHandler(this.ctxClear_Click);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(142, 6);
            // 
            // ctxFillCol
            // 
            this.ctxFillCol.Name = "ctxFillCol";
            this.ctxFillCol.Size = new System.Drawing.Size(145, 22);
            this.ctxFillCol.Text = "Fill Column";
            this.ctxFillCol.Click += new System.EventHandler(this.ctxFillCol_Click);
            // 
            // ctxAddRow
            // 
            this.ctxAddRow.Name = "ctxAddRow";
            this.ctxAddRow.Size = new System.Drawing.Size(145, 22);
            this.ctxAddRow.Text = "Add Rows";
            this.ctxAddRow.Click += new System.EventHandler(this.ctxAddRow_Click);
            // 
            // ctxDelRow
            // 
            this.ctxDelRow.Name = "ctxDelRow";
            this.ctxDelRow.Size = new System.Drawing.Size(145, 22);
            this.ctxDelRow.Text = "Delete Rows";
            this.ctxDelRow.Click += new System.EventHandler(this.ctxDelRow_Click);
            // 
            // btnShelfLoad
            // 
            this.btnShelfLoad.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnShelfLoad.Location = new System.Drawing.Point(630, 270);
            this.btnShelfLoad.Name = "btnShelfLoad";
            this.btnShelfLoad.Size = new System.Drawing.Size(49, 23);
            this.btnShelfLoad.TabIndex = 61;
            this.btnShelfLoad.Text = "Load";
            this.btnShelfLoad.UseVisualStyleBackColor = true;
            this.btnShelfLoad.Visible = false;
            this.btnShelfLoad.Click += new System.EventHandler(this.cmbShelf_SelectedIndexChanged);
            // 
            // txtMSANRack
            // 
            this.txtMSANRack.Location = new System.Drawing.Point(385, 269);
            this.txtMSANRack.Name = "txtMSANRack";
            this.txtMSANRack.Size = new System.Drawing.Size(30, 21);
            this.txtMSANRack.TabIndex = 60;
            this.txtMSANRack.Visible = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(349, 270);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(30, 13);
            this.label1.TabIndex = 23;
            this.label1.Text = "Rack";
            this.label1.Visible = false;
            // 
            // btnShelfDel
            // 
            this.btnShelfDel.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnShelfDel.Location = new System.Drawing.Point(590, 270);
            this.btnShelfDel.Name = "btnShelfDel";
            this.btnShelfDel.Size = new System.Drawing.Size(40, 23);
            this.btnShelfDel.TabIndex = 16;
            this.btnShelfDel.Text = "Del";
            this.btnShelfDel.UseVisualStyleBackColor = true;
            this.btnShelfDel.Visible = false;
            this.btnShelfDel.Click += new System.EventHandler(this.btnShelfDel_Click);
            // 
            // btnShelfAdd
            // 
            this.btnShelfAdd.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnShelfAdd.Location = new System.Drawing.Point(550, 270);
            this.btnShelfAdd.Name = "btnShelfAdd";
            this.btnShelfAdd.Size = new System.Drawing.Size(40, 23);
            this.btnShelfAdd.TabIndex = 15;
            this.btnShelfAdd.Text = "Add";
            this.btnShelfAdd.UseVisualStyleBackColor = true;
            this.btnShelfAdd.Visible = false;
            this.btnShelfAdd.Click += new System.EventHandler(this.btnShelfAdd_Click);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(421, 270);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(47, 13);
            this.label6.TabIndex = 14;
            this.label6.Text = "Shelf No";
            this.label6.Visible = false;
            // 
            // cmbShelf
            // 
            this.cmbShelf.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbShelf.FormattingEnabled = true;
            this.cmbShelf.Location = new System.Drawing.Point(474, 269);
            this.cmbShelf.Name = "cmbShelf";
            this.cmbShelf.Size = new System.Drawing.Size(50, 21);
            this.cmbShelf.Sorted = true;
            this.cmbShelf.TabIndex = 13;
            this.cmbShelf.Visible = false;
            this.cmbShelf.SelectedIndexChanged += new System.EventHandler(this.cmbShelf_SelectedIndexChanged);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.shelfToolStripMenuItem,
            this.slotToolStripMenuItem,
            this.editToolStripMenuItem,
            this.mnuRollback,
            this.mnuReload,
            this.mnuCommit});
            this.menuStrip1.Location = new System.Drawing.Point(0, 215);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(935, 24);
            this.menuStrip1.TabIndex = 62;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // shelfToolStripMenuItem
            // 
            this.shelfToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuShelfAdd,
            this.mnuShelfDel});
            this.shelfToolStripMenuItem.Name = "shelfToolStripMenuItem";
            this.shelfToolStripMenuItem.Size = new System.Drawing.Size(43, 20);
            this.shelfToolStripMenuItem.Text = "Shelf";
            // 
            // mnuShelfAdd
            // 
            this.mnuShelfAdd.Name = "mnuShelfAdd";
            this.mnuShelfAdd.Size = new System.Drawing.Size(116, 22);
            this.mnuShelfAdd.Text = "Add";
            this.mnuShelfAdd.Click += new System.EventHandler(this.btnShelfAdd_Click);
            // 
            // mnuShelfDel
            // 
            this.mnuShelfDel.Name = "mnuShelfDel";
            this.mnuShelfDel.Size = new System.Drawing.Size(116, 22);
            this.mnuShelfDel.Text = "Delete";
            this.mnuShelfDel.Click += new System.EventHandler(this.btnShelfDel_Click);
            // 
            // slotToolStripMenuItem
            // 
            this.slotToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuSlotAdd,
            this.mnuSlotDel});
            this.slotToolStripMenuItem.Name = "slotToolStripMenuItem";
            this.slotToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.slotToolStripMenuItem.Text = "Slot";
            // 
            // mnuSlotAdd
            // 
            this.mnuSlotAdd.Name = "mnuSlotAdd";
            this.mnuSlotAdd.Size = new System.Drawing.Size(116, 22);
            this.mnuSlotAdd.Text = "Add";
            this.mnuSlotAdd.Click += new System.EventHandler(this.ctxAddRow_Click);
            // 
            // mnuSlotDel
            // 
            this.mnuSlotDel.Name = "mnuSlotDel";
            this.mnuSlotDel.Size = new System.Drawing.Size(116, 22);
            this.mnuSlotDel.Text = "Delete";
            this.mnuSlotDel.Click += new System.EventHandler(this.ctxDelRow_Click);
            // 
            // editToolStripMenuItem
            // 
            this.editToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuCopy,
            this.mnuPaste,
            this.mnuFill,
            this.mnuClear});
            this.editToolStripMenuItem.Name = "editToolStripMenuItem";
            this.editToolStripMenuItem.Size = new System.Drawing.Size(42, 20);
            this.editToolStripMenuItem.Text = "Card";
            // 
            // mnuCopy
            // 
            this.mnuCopy.Name = "mnuCopy";
            this.mnuCopy.Size = new System.Drawing.Size(112, 22);
            this.mnuCopy.Text = "Copy";
            this.mnuCopy.Click += new System.EventHandler(this.ctxCopy_Click);
            // 
            // mnuPaste
            // 
            this.mnuPaste.Name = "mnuPaste";
            this.mnuPaste.Size = new System.Drawing.Size(112, 22);
            this.mnuPaste.Text = "Paste";
            this.mnuPaste.Click += new System.EventHandler(this.ctxPaste_Click);
            // 
            // mnuFill
            // 
            this.mnuFill.Name = "mnuFill";
            this.mnuFill.Size = new System.Drawing.Size(112, 22);
            this.mnuFill.Text = "Fill";
            this.mnuFill.Click += new System.EventHandler(this.ctxFillCol_Click);
            // 
            // mnuClear
            // 
            this.mnuClear.Name = "mnuClear";
            this.mnuClear.Size = new System.Drawing.Size(112, 22);
            this.mnuClear.Text = "Clear";
            this.mnuClear.Click += new System.EventHandler(this.ctxClear_Click);
            // 
            // mnuRollback
            // 
            this.mnuRollback.Name = "mnuRollback";
            this.mnuRollback.Size = new System.Drawing.Size(58, 20);
            this.mnuRollback.Text = "Rollback";
            this.mnuRollback.Click += new System.EventHandler(this.btnRollback_Click);
            // 
            // mnuReload
            // 
            this.mnuReload.Name = "mnuReload";
            this.mnuReload.Size = new System.Drawing.Size(52, 20);
            this.mnuReload.Text = "Reload";
            this.mnuReload.Click += new System.EventHandler(this.mnuReload_Click);
            // 
            // mnuCommit
            // 
            this.mnuCommit.Name = "mnuCommit";
            this.mnuCommit.Size = new System.Drawing.Size(54, 20);
            this.mnuCommit.Text = "Commit";
            this.mnuCommit.Click += new System.EventHandler(this.btnCommit_Click);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Slot";
            this.columnHeader1.Width = 45;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Card Type";
            this.columnHeader2.Width = 70;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "Card Model";
            this.columnHeader3.Width = 70;
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "Start Port";
            // 
            // columnHeader5
            // 
            this.columnHeader5.Text = "End Port";
            // 
            // columnHeader6
            // 
            this.columnHeader6.Text = "Card Status";
            this.columnHeader6.Width = 70;
            // 
            // columnHeader7
            // 
            this.columnHeader7.Text = "Card Mode";
            this.columnHeader7.Width = 70;
            // 
            // pnlTemplate
            // 
            this.pnlTemplate.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.pnlTemplate.Controls.Add(this.btnApplySelected);
            this.pnlTemplate.Controls.Add(this.cmbMSANTemplate);
            this.pnlTemplate.Controls.Add(this.btnApply);
            this.pnlTemplate.Controls.Add(this.lsvTemplate);
            this.pnlTemplate.Controls.Add(this.label26);
            this.pnlTemplate.Controls.Add(this.btnTemplate);
            this.pnlTemplate.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlTemplate.Location = new System.Drawing.Point(0, 89);
            this.pnlTemplate.Name = "pnlTemplate";
            this.pnlTemplate.Size = new System.Drawing.Size(935, 101);
            this.pnlTemplate.TabIndex = 3;
            // 
            // btnApplySelected
            // 
            this.btnApplySelected.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnApplySelected.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnApplySelected.Location = new System.Drawing.Point(803, 27);
            this.btnApplySelected.Name = "btnApplySelected";
            this.btnApplySelected.Size = new System.Drawing.Size(106, 23);
            this.btnApplySelected.TabIndex = 6;
            this.btnApplySelected.Text = "Apply Selected";
            this.btnApplySelected.UseVisualStyleBackColor = true;
            this.btnApplySelected.Click += new System.EventHandler(this.btnApplySelected_Click);
            // 
            // cmbMSANTemplate
            // 
            this.cmbMSANTemplate.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbMSANTemplate.FormattingEnabled = true;
            this.cmbMSANTemplate.Location = new System.Drawing.Point(82, 29);
            this.cmbMSANTemplate.Name = "cmbMSANTemplate";
            this.cmbMSANTemplate.Size = new System.Drawing.Size(636, 21);
            this.cmbMSANTemplate.TabIndex = 5;
            this.cmbMSANTemplate.SelectedIndexChanged += new System.EventHandler(this.cmbMSANTemplate_SelectedIndexChanged);
            // 
            // btnApply
            // 
            this.btnApply.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnApply.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnApply.Location = new System.Drawing.Point(724, 27);
            this.btnApply.Name = "btnApply";
            this.btnApply.Size = new System.Drawing.Size(75, 23);
            this.btnApply.TabIndex = 4;
            this.btnApply.Text = "Apply All";
            this.btnApply.UseVisualStyleBackColor = true;
            this.btnApply.Click += new System.EventHandler(this.btnApply_Click);
            // 
            // lsvTemplate
            // 
            this.lsvTemplate.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lsvTemplate.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader15,
            this.columnHeader16,
            this.columnHeader17,
            this.columnHeader18,
            this.columnHeader19,
            this.columnHeader21,
            this.columnHeader20,
            this.columnHeader22,
            this.columnHeader8,
            this.columnHeader9});
            this.lsvTemplate.FullRowSelect = true;
            this.lsvTemplate.GridLines = true;
            this.lsvTemplate.Location = new System.Drawing.Point(3, 57);
            this.lsvTemplate.Name = "lsvTemplate";
            this.lsvTemplate.Size = new System.Drawing.Size(927, 38);
            this.lsvTemplate.TabIndex = 3;
            this.lsvTemplate.UseCompatibleStateImageBehavior = false;
            this.lsvTemplate.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader15
            // 
            this.columnHeader15.Text = "RACK NO";
            this.columnHeader15.Width = 61;
            // 
            // columnHeader16
            // 
            this.columnHeader16.Text = "FRAME NO";
            this.columnHeader16.Width = 67;
            // 
            // columnHeader17
            // 
            this.columnHeader17.Text = "SLOT NO";
            this.columnHeader17.Width = 61;
            // 
            // columnHeader18
            // 
            this.columnHeader18.Text = " SLOT NIS";
            this.columnHeader18.Width = 63;
            // 
            // columnHeader19
            // 
            this.columnHeader19.Text = "CARD TYPE";
            this.columnHeader19.Width = 75;
            // 
            // columnHeader21
            // 
            this.columnHeader21.Text = "CARD MODEL";
            this.columnHeader21.Width = 80;
            // 
            // columnHeader20
            // 
            this.columnHeader20.Text = "CARD STATUS";
            this.columnHeader20.Width = 85;
            // 
            // columnHeader22
            // 
            this.columnHeader22.Text = "PORT LO";
            // 
            // columnHeader8
            // 
            this.columnHeader8.Text = "PORT HI";
            // 
            // columnHeader9
            // 
            this.columnHeader9.Text = "PLANT UNIT";
            this.columnHeader9.Width = 90;
            // 
            // label26
            // 
            this.label26.AutoSize = true;
            this.label26.Location = new System.Drawing.Point(6, 32);
            this.label26.Name = "label26";
            this.label26.Size = new System.Drawing.Size(70, 13);
            this.label26.TabIndex = 1;
            this.label26.Text = "Template File";
            // 
            // btnTemplate
            // 
            this.btnTemplate.BackColor = System.Drawing.Color.White;
            this.btnTemplate.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnTemplate.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnTemplate.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnTemplate.ImageIndex = 1;
            this.btnTemplate.ImageList = this.imageList1;
            this.btnTemplate.Location = new System.Drawing.Point(0, 0);
            this.btnTemplate.Name = "btnTemplate";
            this.btnTemplate.Size = new System.Drawing.Size(935, 22);
            this.btnTemplate.TabIndex = 0;
            this.btnTemplate.Text = "Template";
            this.btnTemplate.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnTemplate.UseVisualStyleBackColor = false;
            this.btnTemplate.Click += new System.EventHandler(this.btnTemplate_Click);
            // 
            // splitter1
            // 
            this.splitter1.Dock = System.Windows.Forms.DockStyle.Top;
            this.splitter1.Location = new System.Drawing.Point(0, 190);
            this.splitter1.Name = "splitter1";
            this.splitter1.Size = new System.Drawing.Size(935, 3);
            this.splitter1.TabIndex = 4;
            this.splitter1.TabStop = false;
            // 
            // btnDetail
            // 
            this.btnDetail.BackColor = System.Drawing.Color.White;
            this.btnDetail.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnDetail.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDetail.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnDetail.ImageIndex = 1;
            this.btnDetail.Location = new System.Drawing.Point(0, 193);
            this.btnDetail.Name = "btnDetail";
            this.btnDetail.Size = new System.Drawing.Size(935, 22);
            this.btnDetail.TabIndex = 5;
            this.btnDetail.Text = "Detail Configuration";
            this.btnDetail.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnDetail.UseVisualStyleBackColor = false;
            // 
            // GTMSANForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.ClientSize = new System.Drawing.Size(935, 587);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.txtMSANRack);
            this.Controls.Add(this.btnShelfLoad);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.btnShelfDel);
            this.Controls.Add(this.btnShelfAdd);
            this.Controls.Add(this.cmbShelf);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.menuStrip1);
            this.Controls.Add(this.btnDetail);
            this.Controls.Add(this.splitter1);
            this.Controls.Add(this.pnlTemplate);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.pnlTop);
            this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Name = "GTMSANForm";
            this.Text = "Equipment Configuration Module v3.0";
            this.TopMost = true;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.GTMSANForm_FormClosing);
            this.pnlTop.ResumeLayout(false);
            this.pnlTop.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.grdMSAN)).EndInit();
            this.ctxMenu.ResumeLayout(false);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.pnlTemplate.ResumeLayout(false);
            this.pnlTemplate.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel pnlTop;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.ColumnHeader columnHeader5;
        private System.Windows.Forms.ColumnHeader columnHeader6;
        private System.Windows.Forms.ColumnHeader columnHeader7;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.Panel pnlTemplate;
        private System.Windows.Forms.Button btnTemplate;
        private System.Windows.Forms.Label label26;
        private System.Windows.Forms.Button btnApply;
        private System.Windows.Forms.ListView lsvTemplate;
        private System.Windows.Forms.ColumnHeader columnHeader15;
        private System.Windows.Forms.ColumnHeader columnHeader16;
        private System.Windows.Forms.ColumnHeader columnHeader17;
        private System.Windows.Forms.ColumnHeader columnHeader18;
        private System.Windows.Forms.ColumnHeader columnHeader19;
        private System.Windows.Forms.ColumnHeader columnHeader20;
        private System.Windows.Forms.ColumnHeader columnHeader21;
        private System.Windows.Forms.ColumnHeader columnHeader22;
        private System.Windows.Forms.Splitter splitter1;
        private System.Windows.Forms.Button btnHeader;
        private System.Windows.Forms.Label label29;
        private System.Windows.Forms.Label label28;
        private System.Windows.Forms.Label lblType;
        private System.Windows.Forms.TextBox txtCode;
        private System.Windows.Forms.Label lblCode;
        private System.Windows.Forms.TextBox txtExchange;
        private System.Windows.Forms.Label label20;
        private System.Windows.Forms.ComboBox cmbMSANTemplate;
        private System.Windows.Forms.Button btnApplySelected;
        private System.Windows.Forms.Button btnShelfDel;
        private System.Windows.Forms.Button btnShelfAdd;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ComboBox cmbShelf;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnDetail;
        private System.Windows.Forms.DataGridView grdMSAN;
        private System.Windows.Forms.ColumnHeader columnHeader8;
        private System.Windows.Forms.TextBox txtModel;
        private System.Windows.Forms.TextBox txtManufacturer;
        private System.Windows.Forms.TextBox txtType;
        private System.Windows.Forms.ContextMenuStrip ctxMenu;
        private System.Windows.Forms.ToolStripMenuItem ctxCopy;
        private System.Windows.Forms.ToolStripMenuItem ctxPaste;
        private System.Windows.Forms.ToolStripMenuItem ctxClear;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem ctxFillCol;
        private System.Windows.Forms.ToolStripMenuItem ctxAddRow;
        private System.Windows.Forms.ToolStripMenuItem ctxDelRow;
        private System.Windows.Forms.TextBox txtMSANRack;
        private System.Windows.Forms.TextBox txtStatus;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button btnShelfLoad;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mnuCopy;
        private System.Windows.Forms.ToolStripMenuItem mnuPaste;
        private System.Windows.Forms.ToolStripMenuItem mnuFill;
        private System.Windows.Forms.ToolStripMenuItem mnuClear;
        private System.Windows.Forms.ToolStripMenuItem mnuCommit;
        private System.Windows.Forms.ToolStripMenuItem mnuRollback;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.TreeView trvCard;
        private System.Windows.Forms.ToolStripMenuItem shelfToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mnuShelfAdd;
        private System.Windows.Forms.ToolStripMenuItem mnuShelfDel;
        private System.Windows.Forms.ToolStripMenuItem slotToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mnuSlotAdd;
        private System.Windows.Forms.ToolStripMenuItem mnuSlotDel;
        private System.Windows.Forms.ToolStripMenuItem mnuReload;
        private System.Windows.Forms.ColumnHeader columnHeader9;
        private System.Windows.Forms.DataGridViewTextBoxColumn colSlotNo;
        private System.Windows.Forms.DataGridViewTextBoxColumn colSlotNIS;
        private System.Windows.Forms.DataGridViewComboBoxColumn colPlantUnit;
        private System.Windows.Forms.DataGridViewComboBoxColumn colCardTypeCombo;
        private System.Windows.Forms.DataGridViewComboBoxColumn colCardModelCombo;
        private System.Windows.Forms.DataGridViewComboBoxColumn colCardStatus;
        private System.Windows.Forms.DataGridViewTextBoxColumn colPortLo;
        private System.Windows.Forms.DataGridViewTextBoxColumn colPortHi;
        private System.Windows.Forms.DataGridViewTextBoxColumn colGEMS;
        private System.Windows.Forms.DataGridViewTextBoxColumn colBOM;

    }
}

