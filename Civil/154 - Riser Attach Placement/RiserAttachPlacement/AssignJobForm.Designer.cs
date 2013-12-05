namespace NEPS.GTechnology.AssignJob
{
    partial class AssignJobForm
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
            this.gvFeatures = new System.Windows.Forms.DataGridView();
            this.Checked = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.FID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Username = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.btnSelectAll = new System.Windows.Forms.Button();
            this.btnClearAll = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.gbTitle = new System.Windows.Forms.GroupBox();
            this.btnAdmin = new System.Windows.Forms.Button();
            this.btnApply = new System.Windows.Forms.Button();
            this.txtRegenerateStatus = new System.Windows.Forms.TextBox();
            this.btnRegenerate = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.cbSIDs = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.txtUsername = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.txtPassword = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.cbRegenBoundaryFT = new System.Windows.Forms.ComboBox();
            this.cbRegenFeatureFT = new System.Windows.Forms.ComboBox();
            this.label8 = new System.Windows.Forms.Label();
            this.btnCancel = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.gvFeatures)).BeginInit();
            this.gbTitle.SuspendLayout();
            this.SuspendLayout();
            // 
            // gvFeatures
            // 
            this.gvFeatures.AllowUserToAddRows = false;
            this.gvFeatures.AllowUserToDeleteRows = false;
            this.gvFeatures.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.gvFeatures.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gvFeatures.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Checked,
            this.FID,
            this.Username});
            this.gvFeatures.Location = new System.Drawing.Point(39, 63);
            this.gvFeatures.Name = "gvFeatures";
            this.gvFeatures.ShowEditingIcon = false;
            this.gvFeatures.Size = new System.Drawing.Size(337, 431);
            this.gvFeatures.TabIndex = 1;
            this.gvFeatures.SelectionChanged += new System.EventHandler(this.gvFeatures_SelectionChanged);
            // 
            // Checked
            // 
            this.Checked.DataPropertyName = "IsChecked";
            this.Checked.HeaderText = "";
            this.Checked.Name = "Checked";
            this.Checked.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.Checked.Width = 5;
            // 
            // FID
            // 
            this.FID.DataPropertyName = "FID";
            this.FID.HeaderText = "ID";
            this.FID.Name = "FID";
            this.FID.Width = 43;
            // 
            // Username
            // 
            this.Username.DataPropertyName = "Username";
            this.Username.HeaderText = "Name";
            this.Username.Name = "Username";
            this.Username.ReadOnly = true;
            this.Username.Width = 60;
            // 
            // btnSelectAll
            // 
            this.btnSelectAll.Location = new System.Drawing.Point(24, 484);
            this.btnSelectAll.Name = "btnSelectAll";
            this.btnSelectAll.Size = new System.Drawing.Size(75, 23);
            this.btnSelectAll.TabIndex = 2;
            this.btnSelectAll.Text = "Select &All";
            this.btnSelectAll.UseVisualStyleBackColor = true;
            this.btnSelectAll.Click += new System.EventHandler(this.btnSelectAll_Click);
            // 
            // btnClearAll
            // 
            this.btnClearAll.Location = new System.Drawing.Point(105, 484);
            this.btnClearAll.Name = "btnClearAll";
            this.btnClearAll.Size = new System.Drawing.Size(75, 23);
            this.btnClearAll.TabIndex = 2;
            this.btnClearAll.Text = "&Clear All";
            this.btnClearAll.UseVisualStyleBackColor = true;
            this.btnClearAll.Click += new System.EventHandler(this.btnClearAll_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(21, 25);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(184, 13);
            this.label1.TabIndex = 6;
            this.label1.Text = "Unassigned Features in the Boundary";
            // 
            // gbTitle
            // 
            this.gbTitle.Controls.Add(this.btnCancel);
            this.gbTitle.Controls.Add(this.label1);
            this.gbTitle.Controls.Add(this.btnAdmin);
            this.gbTitle.Controls.Add(this.btnApply);
            this.gbTitle.Controls.Add(this.btnClearAll);
            this.gbTitle.Controls.Add(this.btnSelectAll);
            this.gbTitle.Location = new System.Drawing.Point(15, 16);
            this.gbTitle.Name = "gbTitle";
            this.gbTitle.Size = new System.Drawing.Size(379, 513);
            this.gbTitle.TabIndex = 11;
            this.gbTitle.TabStop = false;
            this.gbTitle.Text = "Specify Features";
            // 
            // btnAdmin
            // 
            this.btnAdmin.Location = new System.Drawing.Point(286, 18);
            this.btnAdmin.Name = "btnAdmin";
            this.btnAdmin.Size = new System.Drawing.Size(75, 23);
            this.btnAdmin.TabIndex = 19;
            this.btnAdmin.Text = "Admin";
            this.btnAdmin.UseVisualStyleBackColor = true;
            this.btnAdmin.Visible = false;
            this.btnAdmin.Click += new System.EventHandler(this.btnAdmin_Click);
            // 
            // btnApply
            // 
            this.btnApply.Location = new System.Drawing.Point(222, 484);
            this.btnApply.Name = "btnApply";
            this.btnApply.Size = new System.Drawing.Size(70, 23);
            this.btnApply.TabIndex = 0;
            this.btnApply.Text = "&Save";
            this.btnApply.UseVisualStyleBackColor = true;
            this.btnApply.Click += new System.EventHandler(this.btnApply_Click);
            // 
            // txtRegenerateStatus
            // 
            this.txtRegenerateStatus.Location = new System.Drawing.Point(418, 119);
            this.txtRegenerateStatus.Multiline = true;
            this.txtRegenerateStatus.Name = "txtRegenerateStatus";
            this.txtRegenerateStatus.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtRegenerateStatus.Size = new System.Drawing.Size(435, 407);
            this.txtRegenerateStatus.TabIndex = 13;
            // 
            // btnRegenerate
            // 
            this.btnRegenerate.Location = new System.Drawing.Point(763, 90);
            this.btnRegenerate.Name = "btnRegenerate";
            this.btnRegenerate.Size = new System.Drawing.Size(75, 23);
            this.btnRegenerate.TabIndex = 14;
            this.btnRegenerate.Text = "&Regenerate";
            this.btnRegenerate.UseVisualStyleBackColor = true;
            this.btnRegenerate.Click += new System.EventHandler(this.btnRegenerate_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(427, 25);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(25, 13);
            this.label4.TabIndex = 16;
            this.label4.Text = "SID";
            // 
            // cbSIDs
            // 
            this.cbSIDs.FormattingEnabled = true;
            this.cbSIDs.Items.AddRange(new object[] {
            "NOVA",
            "NEPSTRN"});
            this.cbSIDs.Location = new System.Drawing.Point(459, 25);
            this.cbSIDs.Name = "cbSIDs";
            this.cbSIDs.Size = new System.Drawing.Size(121, 21);
            this.cbSIDs.TabIndex = 17;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(598, 25);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(55, 13);
            this.label5.TabIndex = 16;
            this.label5.Text = "Username";
            // 
            // txtUsername
            // 
            this.txtUsername.Location = new System.Drawing.Point(660, 25);
            this.txtUsername.Name = "txtUsername";
            this.txtUsername.Size = new System.Drawing.Size(50, 20);
            this.txtUsername.TabIndex = 18;
            this.txtUsername.Text = "neps";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(726, 26);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(53, 13);
            this.label6.TabIndex = 16;
            this.label6.Text = "Password";
            // 
            // txtPassword
            // 
            this.txtPassword.Location = new System.Drawing.Point(788, 26);
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.Size = new System.Drawing.Size(50, 20);
            this.txtPassword.TabIndex = 18;
            this.txtPassword.Text = "neps";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(635, 65);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(68, 13);
            this.label7.TabIndex = 20;
            this.label7.Text = "Boundary FT";
            // 
            // cbRegenBoundaryFT
            // 
            this.cbRegenBoundaryFT.FormattingEnabled = true;
            this.cbRegenBoundaryFT.Location = new System.Drawing.Point(709, 57);
            this.cbRegenBoundaryFT.Name = "cbRegenBoundaryFT";
            this.cbRegenBoundaryFT.Size = new System.Drawing.Size(129, 21);
            this.cbRegenBoundaryFT.TabIndex = 21;
            // 
            // cbRegenFeatureFT
            // 
            this.cbRegenFeatureFT.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbRegenFeatureFT.Enabled = false;
            this.cbRegenFeatureFT.FormattingEnabled = true;
            this.cbRegenFeatureFT.Location = new System.Drawing.Point(496, 58);
            this.cbRegenFeatureFT.Name = "cbRegenFeatureFT";
            this.cbRegenFeatureFT.Size = new System.Drawing.Size(120, 21);
            this.cbRegenFeatureFT.TabIndex = 1;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(429, 65);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(59, 13);
            this.label8.TabIndex = 20;
            this.label8.Text = "Feature FT";
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(298, 484);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 20;
            this.btnCancel.Text = "C&ancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // AssignJobForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(875, 541);
            this.ControlBox = false;
            this.Controls.Add(this.cbRegenFeatureFT);
            this.Controls.Add(this.cbRegenBoundaryFT);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.txtPassword);
            this.Controls.Add(this.txtUsername);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.cbSIDs);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.btnRegenerate);
            this.Controls.Add(this.txtRegenerateStatus);
            this.Controls.Add(this.gvFeatures);
            this.Controls.Add(this.gbTitle);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MinimizeBox = false;
            this.Name = "AssignJobForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Assign Active Job";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.AssignJobForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.gvFeatures)).EndInit();
            this.gbTitle.ResumeLayout(false);
            this.gbTitle.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView gvFeatures;
        private System.Windows.Forms.DataGridViewCheckBoxColumn Checked;
        private System.Windows.Forms.DataGridViewTextBoxColumn FID;
        private System.Windows.Forms.DataGridViewTextBoxColumn Username;
        private System.Windows.Forms.Button btnSelectAll;
        private System.Windows.Forms.Button btnClearAll;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox gbTitle;
        private System.Windows.Forms.Button btnApply;
        private System.Windows.Forms.TextBox txtRegenerateStatus;
        private System.Windows.Forms.Button btnRegenerate;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox cbSIDs;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtUsername;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txtPassword;
        private System.Windows.Forms.Button btnAdmin;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.ComboBox cbRegenBoundaryFT;
        private System.Windows.Forms.ComboBox cbRegenFeatureFT;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Button btnCancel;
    }
}