namespace NEPS.GTechnology.NEPSDuctNestPlc
{
    partial class DuctnestConfigForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DuctnestConfigForm));
            this.panel1 = new System.Windows.Forms.Panel();
            this.panelTop = new System.Windows.Forms.Panel();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.lblMsg = new System.Windows.Forms.Label();
            this.txtDuctPair = new System.Windows.Forms.TextBox();
            this.lblToManhole = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.btnAddPair = new System.Windows.Forms.Button();
            this.nmrToCol = new System.Windows.Forms.NumericUpDown();
            this.btnClearList = new System.Windows.Forms.Button();
            this.lblFromManhole = new System.Windows.Forms.Label();
            this.nmrFromRow = new System.Windows.Forms.NumericUpDown();
            this.nmrFromCol = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.btnDefaultList = new System.Windows.Forms.Button();
            this.nmrToRow = new System.Windows.Forms.NumericUpDown();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.btnPlaceDuctnest = new System.Windows.Forms.ToolStripButton();
            this.panelLeft = new System.Windows.Forms.Panel();
            this.panelLayout = new System.Windows.Forms.Panel();
            this.panelRight = new System.Windows.Forms.Panel();
            this.lvwMapping = new System.Windows.Forms.ListView();
            this.From = new System.Windows.Forms.ColumnHeader();
            this.To = new System.Windows.Forms.ColumnHeader();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.panelTop.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nmrToCol)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nmrFromRow)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nmrFromCol)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nmrToRow)).BeginInit();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 482);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(765, 10);
            this.panel1.TabIndex = 4;
            // 
            // panelTop
            // 
            this.panelTop.Controls.Add(this.groupBox1);
            this.panelTop.Controls.Add(this.toolStrip1);
            this.panelTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelTop.Location = new System.Drawing.Point(0, 0);
            this.panelTop.Name = "panelTop";
            this.panelTop.Size = new System.Drawing.Size(765, 121);
            this.panelTop.TabIndex = 7;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.lblMsg);
            this.groupBox1.Controls.Add(this.txtDuctPair);
            this.groupBox1.Controls.Add(this.lblToManhole);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.btnAddPair);
            this.groupBox1.Controls.Add(this.nmrToCol);
            this.groupBox1.Controls.Add(this.btnClearList);
            this.groupBox1.Controls.Add(this.lblFromManhole);
            this.groupBox1.Controls.Add(this.nmrFromRow);
            this.groupBox1.Controls.Add(this.nmrFromCol);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.btnDefaultList);
            this.groupBox1.Controls.Add(this.nmrToRow);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(0, 25);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(765, 96);
            this.groupBox1.TabIndex = 8;
            this.groupBox1.TabStop = false;
            // 
            // lblMsg
            // 
            this.lblMsg.AutoSize = true;
            this.lblMsg.Location = new System.Drawing.Point(225, 71);
            this.lblMsg.Name = "lblMsg";
            this.lblMsg.Size = new System.Drawing.Size(10, 13);
            this.lblMsg.TabIndex = 8;
            this.lblMsg.Text = "-";
            // 
            // txtDuctPair
            // 
            this.txtDuctPair.Location = new System.Drawing.Point(174, 18);
            this.txtDuctPair.Name = "txtDuctPair";
            this.txtDuctPair.Size = new System.Drawing.Size(168, 20);
            this.txtDuctPair.TabIndex = 2;
            // 
            // lblToManhole
            // 
            this.lblToManhole.AutoSize = true;
            this.lblToManhole.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblToManhole.Location = new System.Drawing.Point(459, 53);
            this.lblToManhole.Name = "lblToManhole";
            this.lblToManhole.Size = new System.Drawing.Size(52, 13);
            this.lblToManhole.TabIndex = 7;
            this.lblToManhole.Text = "To Nest";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(8, 21);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(160, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Define ductway pair (eg. A1 C2):";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(106, 71);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(12, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "x";
            // 
            // btnAddPair
            // 
            this.btnAddPair.Enabled = false;
            this.btnAddPair.Location = new System.Drawing.Point(355, 16);
            this.btnAddPair.Name = "btnAddPair";
            this.btnAddPair.Size = new System.Drawing.Size(75, 23);
            this.btnAddPair.TabIndex = 1;
            this.btnAddPair.Text = "Add";
            this.btnAddPair.UseVisualStyleBackColor = true;
            this.btnAddPair.Click += new System.EventHandler(this.btnAddPair_Click);
            // 
            // nmrToCol
            // 
            this.nmrToCol.Location = new System.Drawing.Point(555, 69);
            this.nmrToCol.Maximum = new decimal(new int[] {
            36,
            0,
            0,
            0});
            this.nmrToCol.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nmrToCol.Name = "nmrToCol";
            this.nmrToCol.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.nmrToCol.Size = new System.Drawing.Size(71, 20);
            this.nmrToCol.TabIndex = 5;
            this.nmrToCol.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nmrToCol.ValueChanged += new System.EventHandler(this.nmrToCol_ValueChanged);
            // 
            // btnClearList
            // 
            this.btnClearList.Enabled = false;
            this.btnClearList.Location = new System.Drawing.Point(517, 16);
            this.btnClearList.Name = "btnClearList";
            this.btnClearList.Size = new System.Drawing.Size(75, 23);
            this.btnClearList.TabIndex = 1;
            this.btnClearList.Text = "Clear List";
            this.btnClearList.UseVisualStyleBackColor = true;
            this.btnClearList.Click += new System.EventHandler(this.btnClearList_Click);
            // 
            // lblFromManhole
            // 
            this.lblFromManhole.AutoSize = true;
            this.lblFromManhole.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblFromManhole.Location = new System.Drawing.Point(30, 53);
            this.lblFromManhole.Name = "lblFromManhole";
            this.lblFromManhole.Size = new System.Drawing.Size(64, 13);
            this.lblFromManhole.TabIndex = 7;
            this.lblFromManhole.Text = "From Nest";
            // 
            // nmrFromRow
            // 
            this.nmrFromRow.Location = new System.Drawing.Point(29, 69);
            this.nmrFromRow.Maximum = new decimal(new int[] {
            36,
            0,
            0,
            0});
            this.nmrFromRow.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nmrFromRow.Name = "nmrFromRow";
            this.nmrFromRow.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.nmrFromRow.Size = new System.Drawing.Size(71, 20);
            this.nmrFromRow.TabIndex = 5;
            this.nmrFromRow.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nmrFromRow.ValueChanged += new System.EventHandler(this.nmrFromRow_ValueChanged);
            // 
            // nmrFromCol
            // 
            this.nmrFromCol.Location = new System.Drawing.Point(125, 69);
            this.nmrFromCol.Maximum = new decimal(new int[] {
            36,
            0,
            0,
            0});
            this.nmrFromCol.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nmrFromCol.Name = "nmrFromCol";
            this.nmrFromCol.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.nmrFromCol.Size = new System.Drawing.Size(71, 20);
            this.nmrFromCol.TabIndex = 5;
            this.nmrFromCol.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nmrFromCol.ValueChanged += new System.EventHandler(this.nmrFromCol_ValueChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(537, 71);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(12, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "x";
            // 
            // btnDefaultList
            // 
            this.btnDefaultList.Enabled = false;
            this.btnDefaultList.Location = new System.Drawing.Point(436, 16);
            this.btnDefaultList.Name = "btnDefaultList";
            this.btnDefaultList.Size = new System.Drawing.Size(75, 23);
            this.btnDefaultList.TabIndex = 1;
            this.btnDefaultList.Text = "Default List";
            this.btnDefaultList.UseVisualStyleBackColor = true;
            this.btnDefaultList.Click += new System.EventHandler(this.btnDefaultList_Click);
            // 
            // nmrToRow
            // 
            this.nmrToRow.Location = new System.Drawing.Point(460, 69);
            this.nmrToRow.Maximum = new decimal(new int[] {
            36,
            0,
            0,
            0});
            this.nmrToRow.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nmrToRow.Name = "nmrToRow";
            this.nmrToRow.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.nmrToRow.Size = new System.Drawing.Size(71, 20);
            this.nmrToRow.TabIndex = 5;
            this.nmrToRow.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nmrToRow.ValueChanged += new System.EventHandler(this.nmrToRow_ValueChanged);
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnPlaceDuctnest});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(765, 25);
            this.toolStrip1.TabIndex = 0;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // btnPlaceDuctnest
            // 
            this.btnPlaceDuctnest.Enabled = false;
            this.btnPlaceDuctnest.Image = ((System.Drawing.Image)(resources.GetObject("btnPlaceDuctnest.Image")));
            this.btnPlaceDuctnest.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnPlaceDuctnest.Name = "btnPlaceDuctnest";
            this.btnPlaceDuctnest.Size = new System.Drawing.Size(98, 22);
            this.btnPlaceDuctnest.Text = "Place Ductnest";
            // 
            // panelLeft
            // 
            this.panelLeft.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panelLeft.Dock = System.Windows.Forms.DockStyle.Left;
            this.panelLeft.Location = new System.Drawing.Point(0, 121);
            this.panelLeft.Name = "panelLeft";
            this.panelLeft.Size = new System.Drawing.Size(246, 361);
            this.panelLeft.TabIndex = 9;
            // 
            // panelLayout
            // 
            this.panelLayout.Dock = System.Windows.Forms.DockStyle.Right;
            this.panelLayout.Location = new System.Drawing.Point(658, 121);
            this.panelLayout.Name = "panelLayout";
            this.panelLayout.Size = new System.Drawing.Size(107, 361);
            this.panelLayout.TabIndex = 11;
            // 
            // panelRight
            // 
            this.panelRight.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panelRight.Dock = System.Windows.Forms.DockStyle.Right;
            this.panelRight.Location = new System.Drawing.Point(408, 121);
            this.panelRight.Name = "panelRight";
            this.panelRight.Size = new System.Drawing.Size(250, 361);
            this.panelRight.TabIndex = 12;
            // 
            // lvwMapping
            // 
            this.lvwMapping.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.From,
            this.To});
            this.lvwMapping.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lvwMapping.FullRowSelect = true;
            this.lvwMapping.GridLines = true;
            this.lvwMapping.Location = new System.Drawing.Point(246, 121);
            this.lvwMapping.MultiSelect = false;
            this.lvwMapping.Name = "lvwMapping";
            this.lvwMapping.Size = new System.Drawing.Size(162, 361);
            this.lvwMapping.SmallImageList = this.imageList1;
            this.lvwMapping.StateImageList = this.imageList1;
            this.lvwMapping.TabIndex = 13;
            this.lvwMapping.UseCompatibleStateImageBehavior = false;
            this.lvwMapping.View = System.Windows.Forms.View.Details;
            // 
            // From
            // 
            this.From.Text = "From";
            this.From.Width = 82;
            // 
            // To
            // 
            this.To.Text = "To";
            this.To.Width = 72;
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "mapping-right");
            this.imageList1.Images.SetKeyName(1, "mapping-left");
            this.imageList1.Images.SetKeyName(2, "mapping");
            // 
            // DuctnestConfigForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(765, 492);
            this.Controls.Add(this.lvwMapping);
            this.Controls.Add(this.panelRight);
            this.Controls.Add(this.panelLayout);
            this.Controls.Add(this.panelLeft);
            this.Controls.Add(this.panelTop);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "DuctnestConfigForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Ductnest Configuration v6.1";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.DuctnestConfigForm_Load);
            this.panelTop.ResumeLayout(false);
            this.panelTop.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nmrToCol)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nmrFromRow)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nmrFromCol)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nmrToRow)).EndInit();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panelTop;
        private System.Windows.Forms.Panel panelLeft;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.Panel panelLayout;
        private System.Windows.Forms.Panel panelRight;
        private System.Windows.Forms.ListView lvwMapping;
        private System.Windows.Forms.ColumnHeader From;
        private System.Windows.Forms.ColumnHeader To;
        private System.Windows.Forms.ToolStripButton btnPlaceDuctnest;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtDuctPair;
        private System.Windows.Forms.Button btnAddPair;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown nmrToCol;
        private System.Windows.Forms.NumericUpDown nmrToRow;
        private System.Windows.Forms.NumericUpDown nmrFromCol;
        private System.Windows.Forms.NumericUpDown nmrFromRow;
        private System.Windows.Forms.Button btnClearList;
        private System.Windows.Forms.Button btnDefaultList;
        private System.Windows.Forms.Label lblToManhole;
        private System.Windows.Forms.Label lblFromManhole;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label lblMsg;


    }
}