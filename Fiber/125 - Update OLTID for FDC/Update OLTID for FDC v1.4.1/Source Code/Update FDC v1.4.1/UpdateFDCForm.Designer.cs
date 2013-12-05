namespace NEPS.GTechnology.UpdateFDC
{
    partial class GTWindowsForm_UpdateFDC
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
            this.txtFDCcode = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.lblFID = new System.Windows.Forms.Label();
            this.txt_Location = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.btn_Update = new System.Windows.Forms.Button();
            this.btn_Close = new System.Windows.Forms.Button();
            this.cboType = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtFDC = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btn_Pick = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // txtFDCcode
            // 
            this.txtFDCcode.BackColor = System.Drawing.Color.White;
            this.txtFDCcode.ForeColor = System.Drawing.Color.Black;
            this.txtFDCcode.Location = new System.Drawing.Point(77, 45);
            this.txtFDCcode.Name = "txtFDCcode";
            this.txtFDCcode.ReadOnly = true;
            this.txtFDCcode.Size = new System.Drawing.Size(137, 20);
            this.txtFDCcode.TabIndex = 9;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(3, 49);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(32, 13);
            this.label4.TabIndex = 8;
            this.label4.Text = "Code";
            // 
            // lblFID
            // 
            this.lblFID.AutoSize = true;
            this.lblFID.Location = new System.Drawing.Point(242, 99);
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
            this.txt_Location.Location = new System.Drawing.Point(89, 94);
            this.txt_Location.Name = "txt_Location";
            this.txt_Location.ReadOnly = true;
            this.txt_Location.Size = new System.Drawing.Size(137, 20);
            this.txt_Location.TabIndex = 6;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(15, 99);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(55, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "Exchange";
            // 
            // btn_Update
            // 
            this.btn_Update.Location = new System.Drawing.Point(89, 150);
            this.btn_Update.Name = "btn_Update";
            this.btn_Update.Size = new System.Drawing.Size(96, 28);
            this.btn_Update.TabIndex = 4;
            this.btn_Update.Text = "Update";
            this.btn_Update.UseVisualStyleBackColor = true;
            this.btn_Update.Click += new System.EventHandler(this.btn_Update_Click);
            // 
            // btn_Close
            // 
            this.btn_Close.Location = new System.Drawing.Point(210, 150);
            this.btn_Close.Name = "btn_Close";
            this.btn_Close.Size = new System.Drawing.Size(96, 28);
            this.btn_Close.TabIndex = 2;
            this.btn_Close.Text = "Close";
            this.btn_Close.UseVisualStyleBackColor = true;
            this.btn_Close.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // cboType
            // 
            this.cboType.BackColor = System.Drawing.Color.White;
            this.cboType.ForeColor = System.Drawing.Color.Black;
            this.cboType.FormattingEnabled = true;
            this.cboType.Location = new System.Drawing.Point(89, 120);
            this.cboType.Name = "cboType";
            this.cboType.Size = new System.Drawing.Size(137, 21);
            this.cboType.TabIndex = 4;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(15, 127);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(42, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "OLT ID";
            // 
            // txtFDC
            // 
            this.txtFDC.BackColor = System.Drawing.Color.White;
            this.txtFDC.ForeColor = System.Drawing.Color.Black;
            this.txtFDC.Location = new System.Drawing.Point(77, 19);
            this.txtFDC.Name = "txtFDC";
            this.txtFDC.ReadOnly = true;
            this.txtFDC.Size = new System.Drawing.Size(137, 20);
            this.txtFDC.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 23);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(24, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "FID";
            // 
            // btn_Pick
            // 
            this.btn_Pick.Location = new System.Drawing.Point(220, 18);
            this.btn_Pick.Name = "btn_Pick";
            this.btn_Pick.Size = new System.Drawing.Size(64, 23);
            this.btn_Pick.TabIndex = 0;
            this.btn_Pick.Text = "Pick";
            this.btn_Pick.UseVisualStyleBackColor = true;
            this.btn_Pick.Click += new System.EventHandler(this.btn_Pick_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.txtFDCcode);
            this.groupBox1.Controls.Add(this.txtFDC);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.btn_Pick);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(294, 76);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "FDC";
            // 
            // GTWindowsForm_UpdateFDC
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(366, 190);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.lblFID);
            this.Controls.Add(this.txt_Location);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.cboType);
            this.Controls.Add(this.btn_Update);
            this.Controls.Add(this.btn_Close);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "GTWindowsForm_UpdateFDC";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Update OLT ID for FDC v1.4.1";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.GTWindowsForm_InitRoute_Load);
            this.Shown += new System.EventHandler(this.GTWindowsForm_InitRoute_Shown);
            this.Activated += new System.EventHandler(this.GTWindowsForm_InitRoute_Activated);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btn_Update;
        private System.Windows.Forms.Button btn_Close;
        private System.Windows.Forms.TextBox txtFDC;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btn_Pick;
        private System.Windows.Forms.ComboBox cboType;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txt_Location;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label lblFID;
        private System.Windows.Forms.TextBox txtFDCcode;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.GroupBox groupBox1;

    }
}