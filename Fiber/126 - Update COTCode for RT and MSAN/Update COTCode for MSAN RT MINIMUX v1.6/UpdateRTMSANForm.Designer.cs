namespace NEPS.GTechnology.UpdateRTMSAN
{
    partial class GTWindowsForm_UpdateRTMSAN
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
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.gbFeatureInfo = new System.Windows.Forms.GroupBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.txtDeviceCode = new System.Windows.Forms.TextBox();
            this.txtDevice = new System.Windows.Forms.TextBox();
            this.btn_Pick = new System.Windows.Forms.Button();
            this.lblFID = new System.Windows.Forms.Label();
            this.txt_Location = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.btn_Update = new System.Windows.Forms.Button();
            this.btn_Close = new System.Windows.Forms.Button();
            this.cboType = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.groupBox2.SuspendLayout();
            this.gbFeatureInfo.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.gbFeatureInfo);
            this.groupBox2.Controls.Add(this.lblFID);
            this.groupBox2.Controls.Add(this.txt_Location);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.btn_Update);
            this.groupBox2.Controls.Add(this.btn_Close);
            this.groupBox2.Controls.Add(this.cboType);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Location = new System.Drawing.Point(12, 12);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(314, 188);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            // 
            // gbFeatureInfo
            // 
            this.gbFeatureInfo.Controls.Add(this.label4);
            this.gbFeatureInfo.Controls.Add(this.label1);
            this.gbFeatureInfo.Controls.Add(this.txtDeviceCode);
            this.gbFeatureInfo.Controls.Add(this.txtDevice);
            this.gbFeatureInfo.Controls.Add(this.btn_Pick);
            this.gbFeatureInfo.Location = new System.Drawing.Point(13, 16);
            this.gbFeatureInfo.Name = "gbFeatureInfo";
            this.gbFeatureInfo.Size = new System.Drawing.Size(289, 69);
            this.gbFeatureInfo.TabIndex = 8;
            this.gbFeatureInfo.TabStop = false;
            this.gbFeatureInfo.Text = "MSAN/MUX";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 49);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(32, 13);
            this.label4.TabIndex = 5;
            this.label4.Text = "Code";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 23);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(24, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "FID";
            // 
            // txtDeviceCode
            // 
            this.txtDeviceCode.BackColor = System.Drawing.Color.White;
            this.txtDeviceCode.ForeColor = System.Drawing.Color.Black;
            this.txtDeviceCode.Location = new System.Drawing.Point(81, 42);
            this.txtDeviceCode.Name = "txtDeviceCode";
            this.txtDeviceCode.ReadOnly = true;
            this.txtDeviceCode.Size = new System.Drawing.Size(137, 20);
            this.txtDeviceCode.TabIndex = 3;
            // 
            // txtDevice
            // 
            this.txtDevice.BackColor = System.Drawing.Color.White;
            this.txtDevice.ForeColor = System.Drawing.Color.Black;
            this.txtDevice.Location = new System.Drawing.Point(81, 16);
            this.txtDevice.Name = "txtDevice";
            this.txtDevice.ReadOnly = true;
            this.txtDevice.Size = new System.Drawing.Size(137, 20);
            this.txtDevice.TabIndex = 2;
            // 
            // btn_Pick
            // 
            this.btn_Pick.Location = new System.Drawing.Point(233, 16);
            this.btn_Pick.Name = "btn_Pick";
            this.btn_Pick.Size = new System.Drawing.Size(44, 23);
            this.btn_Pick.TabIndex = 0;
            this.btn_Pick.Text = "Pick";
            this.btn_Pick.UseVisualStyleBackColor = true;
            this.btn_Pick.Click += new System.EventHandler(this.btn_Pick_Click);
            // 
            // lblFID
            // 
            this.lblFID.AutoSize = true;
            this.lblFID.Location = new System.Drawing.Point(268, 102);
            this.lblFID.Name = "lblFID";
            this.lblFID.Size = new System.Drawing.Size(34, 13);
            this.lblFID.TabIndex = 7;
            this.lblFID.Text = "lblFID";
            this.lblFID.Visible = false;
            // 
            // txt_Location
            // 
            this.txt_Location.BackColor = System.Drawing.Color.White;
            this.txt_Location.ForeColor = System.Drawing.Color.Black;
            this.txt_Location.Location = new System.Drawing.Point(94, 95);
            this.txt_Location.Name = "txt_Location";
            this.txt_Location.ReadOnly = true;
            this.txt_Location.Size = new System.Drawing.Size(137, 20);
            this.txt_Location.TabIndex = 6;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(19, 102);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(55, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "Exchange";
            // 
            // btn_Update
            // 
            this.btn_Update.Location = new System.Drawing.Point(79, 152);
            this.btn_Update.Name = "btn_Update";
            this.btn_Update.Size = new System.Drawing.Size(96, 28);
            this.btn_Update.TabIndex = 4;
            this.btn_Update.Text = "Update";
            this.btn_Update.UseVisualStyleBackColor = true;
            this.btn_Update.Click += new System.EventHandler(this.btn_Update_Click);
            // 
            // btn_Close
            // 
            this.btn_Close.Location = new System.Drawing.Point(194, 152);
            this.btn_Close.Name = "btn_Close";
            this.btn_Close.Size = new System.Drawing.Size(96, 28);
            this.btn_Close.TabIndex = 2;
            this.btn_Close.Text = "Close";
            this.btn_Close.UseVisualStyleBackColor = true;
            this.btn_Close.Click += new System.EventHandler(this.button1_Click);
            // 
            // cboType
            // 
            this.cboType.BackColor = System.Drawing.Color.White;
            this.cboType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboType.ForeColor = System.Drawing.Color.Black;
            this.cboType.FormattingEnabled = true;
            this.cboType.Location = new System.Drawing.Point(94, 120);
            this.cboType.Name = "cboType";
            this.cboType.Size = new System.Drawing.Size(137, 21);
            this.cboType.TabIndex = 4;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(19, 128);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(57, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "COT Code";
            // 
            // GTWindowsForm_UpdateRTMSAN
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(351, 211);
            this.Controls.Add(this.groupBox2);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "GTWindowsForm_UpdateRTMSAN";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Update COT Code v1.6";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.GTWindowsForm_InitRoute_Load);
            this.Shown += new System.EventHandler(this.GTWindowsForm_InitRoute_Shown);
            this.Activated += new System.EventHandler(this.GTWindowsForm_InitRoute_Activated);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.gbFeatureInfo.ResumeLayout(false);
            this.gbFeatureInfo.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox gbFeatureInfo;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtDeviceCode;
        private System.Windows.Forms.TextBox txtDevice;
        private System.Windows.Forms.Button btn_Pick;
        private System.Windows.Forms.Label lblFID;
        private System.Windows.Forms.TextBox txt_Location;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btn_Update;
        private System.Windows.Forms.Button btn_Close;
        private System.Windows.Forms.ComboBox cboType;
        private System.Windows.Forms.Label label2;


    }
}