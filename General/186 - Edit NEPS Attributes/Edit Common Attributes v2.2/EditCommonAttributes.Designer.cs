namespace NEPS.GTechnology.EditCommonAttributes
{
    partial class EditCommonAttributes
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.button1 = new System.Windows.Forms.Button();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.dvFeat = new System.Windows.Forms.DataGridView();
            this.G3E_FID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.G3E_FNO = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.FEATURE = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.FEATURE_STATE = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.YEAR_INSTALL = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.OWNERSHIP = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.button2 = new System.Windows.Forms.Button();
            this.txtYear = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.cmbFeatState = new System.Windows.Forms.ComboBox();
            this.txtOwnership = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.txtJobState = new System.Windows.Forms.TextBox();
            this.textExcAbb = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.txtJobID = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.btn_Close = new System.Windows.Forms.Button();
            this.btn_Update = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dvFeat)).BeginInit();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.button1);
            this.groupBox1.Controls.Add(this.progressBar1);
            this.groupBox1.Controls.Add(this.dvFeat);
            this.groupBox1.Location = new System.Drawing.Point(13, 5);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(566, 223);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Feature to Edit";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(431, 197);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(109, 23);
            this.button1.TabIndex = 2;
            this.button1.Text = "Add Feature";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(15, 197);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(297, 20);
            this.progressBar1.TabIndex = 1;
            this.progressBar1.Visible = false;
            // 
            // dvFeat
            // 
            this.dvFeat.AllowUserToAddRows = false;
            this.dvFeat.AllowUserToDeleteRows = false;
            this.dvFeat.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dvFeat.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.G3E_FID,
            this.G3E_FNO,
            this.FEATURE,
            this.FEATURE_STATE,
            this.YEAR_INSTALL,
            this.OWNERSHIP});
            this.dvFeat.Location = new System.Drawing.Point(15, 19);
            this.dvFeat.Name = "dvFeat";
            this.dvFeat.ReadOnly = true;
            this.dvFeat.Size = new System.Drawing.Size(525, 170);
            this.dvFeat.TabIndex = 0;
            this.dvFeat.RowEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.dvFeat_RowEnter);
            this.dvFeat.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dvFeat_CellContentClick);
            // 
            // G3E_FID
            // 
            this.G3E_FID.HeaderText = "FID";
            this.G3E_FID.Name = "G3E_FID";
            this.G3E_FID.ReadOnly = true;
            this.G3E_FID.Width = 80;
            // 
            // G3E_FNO
            // 
            this.G3E_FNO.HeaderText = "FNO";
            this.G3E_FNO.Name = "G3E_FNO";
            this.G3E_FNO.ReadOnly = true;
            this.G3E_FNO.Width = 80;
            // 
            // FEATURE
            // 
            this.FEATURE.HeaderText = "FEATURE";
            this.FEATURE.Name = "FEATURE";
            this.FEATURE.ReadOnly = true;
            // 
            // FEATURE_STATE
            // 
            this.FEATURE_STATE.HeaderText = "FEATURE STATE";
            this.FEATURE_STATE.Name = "FEATURE_STATE";
            this.FEATURE_STATE.ReadOnly = true;
            // 
            // YEAR_INSTALL
            // 
            this.YEAR_INSTALL.HeaderText = "YEAR INSTALL";
            this.YEAR_INSTALL.Name = "YEAR_INSTALL";
            this.YEAR_INSTALL.ReadOnly = true;
            this.YEAR_INSTALL.ToolTipText = " ";
            // 
            // OWNERSHIP
            // 
            this.OWNERSHIP.HeaderText = "OWNERSHIP";
            this.OWNERSHIP.Name = "OWNERSHIP";
            this.OWNERSHIP.ReadOnly = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.button2);
            this.groupBox2.Controls.Add(this.txtYear);
            this.groupBox2.Controls.Add(this.label9);
            this.groupBox2.Controls.Add(this.cmbFeatState);
            this.groupBox2.Controls.Add(this.txtOwnership);
            this.groupBox2.Controls.Add(this.label8);
            this.groupBox2.Controls.Add(this.txtJobState);
            this.groupBox2.Controls.Add(this.textExcAbb);
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Controls.Add(this.txtJobID);
            this.groupBox2.Controls.Add(this.label6);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.btn_Close);
            this.groupBox2.Controls.Add(this.btn_Update);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Location = new System.Drawing.Point(12, 234);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(566, 204);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Attributes";
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(281, 172);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(108, 23);
            this.button2.TabIndex = 21;
            this.button2.Text = "Update Attributes";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // txtYear
            // 
            this.txtYear.BackColor = System.Drawing.Color.White;
            this.txtYear.ForeColor = System.Drawing.Color.Black;
            this.txtYear.Location = new System.Drawing.Point(208, 140);
            this.txtYear.Multiline = true;
            this.txtYear.Name = "txtYear";
            this.txtYear.Size = new System.Drawing.Size(219, 20);
            this.txtYear.TabIndex = 20;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(85, 143);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(82, 13);
            this.label9.TabIndex = 19;
            this.label9.Text = "Installation Year";
            // 
            // cmbFeatState
            // 
            this.cmbFeatState.Location = new System.Drawing.Point(208, 115);
            this.cmbFeatState.Name = "cmbFeatState";
            this.cmbFeatState.Size = new System.Drawing.Size(219, 21);
            this.cmbFeatState.TabIndex = 18;
            // 
            // txtOwnership
            // 
            this.txtOwnership.BackColor = System.Drawing.Color.White;
            this.txtOwnership.ForeColor = System.Drawing.Color.Black;
            this.txtOwnership.Location = new System.Drawing.Point(208, 91);
            this.txtOwnership.Multiline = true;
            this.txtOwnership.Name = "txtOwnership";
            this.txtOwnership.Size = new System.Drawing.Size(219, 20);
            this.txtOwnership.TabIndex = 17;
            this.txtOwnership.Text = "TM";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(85, 94);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(63, 13);
            this.label8.TabIndex = 16;
            this.label8.Text = "Ownership :";
            // 
            // txtJobState
            // 
            this.txtJobState.BackColor = System.Drawing.SystemColors.Control;
            this.txtJobState.Enabled = false;
            this.txtJobState.ForeColor = System.Drawing.Color.Black;
            this.txtJobState.Location = new System.Drawing.Point(208, 67);
            this.txtJobState.Multiline = true;
            this.txtJobState.Name = "txtJobState";
            this.txtJobState.ReadOnly = true;
            this.txtJobState.Size = new System.Drawing.Size(219, 20);
            this.txtJobState.TabIndex = 15;
            // 
            // textExcAbb
            // 
            this.textExcAbb.BackColor = System.Drawing.SystemColors.Control;
            this.textExcAbb.Enabled = false;
            this.textExcAbb.ForeColor = System.Drawing.Color.Black;
            this.textExcAbb.Location = new System.Drawing.Point(208, 43);
            this.textExcAbb.Multiline = true;
            this.textExcAbb.Name = "textExcAbb";
            this.textExcAbb.ReadOnly = true;
            this.textExcAbb.Size = new System.Drawing.Size(219, 20);
            this.textExcAbb.TabIndex = 14;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(85, 46);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(123, 13);
            this.label7.TabIndex = 13;
            this.label7.Text = "Exchange Abbreviation :";
            // 
            // txtJobID
            // 
            this.txtJobID.BackColor = System.Drawing.SystemColors.Control;
            this.txtJobID.Enabled = false;
            this.txtJobID.ForeColor = System.Drawing.Color.Black;
            this.txtJobID.Location = new System.Drawing.Point(208, 19);
            this.txtJobID.Name = "txtJobID";
            this.txtJobID.ReadOnly = true;
            this.txtJobID.Size = new System.Drawing.Size(219, 20);
            this.txtJobID.TabIndex = 12;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(84, 22);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(83, 13);
            this.label6.TabIndex = 9;
            this.label6.Text = "Scheme Name :";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(85, 118);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(77, 13);
            this.label5.TabIndex = 8;
            this.label5.Text = "Feature State :";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(164, 100);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(0, 13);
            this.label4.TabIndex = 7;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(164, 129);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(0, 13);
            this.label3.TabIndex = 6;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(164, 109);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(0, 13);
            this.label2.TabIndex = 5;
            // 
            // btn_Close
            // 
            this.btn_Close.Location = new System.Drawing.Point(395, 172);
            this.btn_Close.Name = "btn_Close";
            this.btn_Close.Size = new System.Drawing.Size(108, 24);
            this.btn_Close.TabIndex = 4;
            this.btn_Close.Text = "Close";
            this.btn_Close.UseVisualStyleBackColor = true;
            this.btn_Close.Click += new System.EventHandler(this.btn_Close_Click);
            // 
            // btn_Update
            // 
            this.btn_Update.Location = new System.Drawing.Point(167, 172);
            this.btn_Update.Name = "btn_Update";
            this.btn_Update.Size = new System.Drawing.Size(108, 24);
            this.btn_Update.TabIndex = 3;
            this.btn_Update.Text = "Assign Job";
            this.btn_Update.UseVisualStyleBackColor = true;
            this.btn_Update.Click += new System.EventHandler(this.btn_Update_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(85, 70);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(58, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Job State :";
            // 
            // EditCommonAttributes
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(591, 450);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "EditCommonAttributes";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Edit Common Attributes v2.2";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.GTWindowsForm_EditCommonAttributes_Load);
            this.Shown += new System.EventHandler(this.GTWindowsForm_EditCommonAttributes_Shown);
            this.Activated += new System.EventHandler(this.GTWindowsForm_EditCommonAttributes_Activated);
            this.groupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dvFeat)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.DataGridView dvFeat;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button btn_Close;
        private System.Windows.Forms.Button btn_Update;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtJobID;
        private System.Windows.Forms.TextBox textExcAbb;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox txtJobState;
        private System.Windows.Forms.ComboBox cmbFeatState;
        private System.Windows.Forms.TextBox txtOwnership;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox txtYear;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.DataGridViewTextBoxColumn G3E_FID;
        private System.Windows.Forms.DataGridViewTextBoxColumn G3E_FNO;
        private System.Windows.Forms.DataGridViewTextBoxColumn FEATURE;
        private System.Windows.Forms.DataGridViewTextBoxColumn FEATURE_STATE;
        private System.Windows.Forms.DataGridViewTextBoxColumn YEAR_INSTALL;
        private System.Windows.Forms.DataGridViewTextBoxColumn OWNERSHIP;



    }
}