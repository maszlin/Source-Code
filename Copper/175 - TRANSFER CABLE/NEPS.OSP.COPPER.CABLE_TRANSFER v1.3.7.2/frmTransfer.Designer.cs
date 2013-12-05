namespace NEPS.OSP.COPPER.CABLE_TRANSFER
{
    partial class frmTransfer
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
            this.label1 = new System.Windows.Forms.Label();
            this.btnReport = new System.Windows.Forms.Button();
            this.txtMessage = new System.Windows.Forms.TextBox();
            this.btnExit = new System.Windows.Forms.Button();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.btnTransfer = new System.Windows.Forms.Button();
            this.dtpTransfer = new System.Windows.Forms.DateTimePicker();
            this.lblDate = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.optCountNow = new System.Windows.Forms.RadioButton();
            this.optCountNew = new System.Windows.Forms.RadioButton();
            this.prgProcess = new System.Windows.Forms.ProgressBar();
            this.chkRecipient = new System.Windows.Forms.CheckBox();
            this.btnRecipient = new System.Windows.Forms.Button();
            this.btnDonor = new System.Windows.Forms.Button();
            this.chkDonor = new System.Windows.Forms.CheckBox();
            this.lblRecipient = new System.Windows.Forms.Label();
            this.lblDonor = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.SystemColors.GradientActiveCaption;
            this.label1.Font = new System.Drawing.Font("Verdana", 21.75F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.SystemColors.Desktop;
            this.label1.Location = new System.Drawing.Point(112, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(323, 35);
            this.label1.TabIndex = 2;
            this.label1.Text = "NEPS-OSP SYSTEM";
            // 
            // btnReport
            // 
            this.btnReport.Enabled = false;
            this.btnReport.Location = new System.Drawing.Point(6, 165);
            this.btnReport.Name = "btnReport";
            this.btnReport.Size = new System.Drawing.Size(120, 29);
            this.btnReport.TabIndex = 6;
            this.btnReport.Text = "4. REPORT";
            this.btnReport.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnReport.UseVisualStyleBackColor = true;
            this.btnReport.Click += new System.EventHandler(this.btnReport_Click);
            // 
            // txtMessage
            // 
            this.txtMessage.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtMessage.BackColor = System.Drawing.Color.WhiteSmoke;
            this.txtMessage.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtMessage.Location = new System.Drawing.Point(132, 199);
            this.txtMessage.Multiline = true;
            this.txtMessage.Name = "txtMessage";
            this.txtMessage.ReadOnly = true;
            this.txtMessage.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtMessage.Size = new System.Drawing.Size(314, 28);
            this.txtMessage.TabIndex = 8;
            this.txtMessage.Text = "Welcome to transfer module";
            // 
            // btnExit
            // 
            this.btnExit.Location = new System.Drawing.Point(6, 199);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(120, 29);
            this.btnExit.TabIndex = 9;
            this.btnExit.Text = "5. CANCEL";
            this.btnExit.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnExit.UseVisualStyleBackColor = true;
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // pictureBox2
            // 
            this.pictureBox2.BackColor = System.Drawing.SystemColors.GradientActiveCaption;
            this.pictureBox2.Image = global::NEPS.OSP.COPPER.CABLE_TRANSFER.Properties.Resources.tnav_tmlogo;
            this.pictureBox2.Location = new System.Drawing.Point(6, 6);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(100, 54);
            this.pictureBox2.TabIndex = 1;
            this.pictureBox2.TabStop = false;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBox1.BackColor = System.Drawing.SystemColors.GradientActiveCaption;
            this.pictureBox1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureBox1.Location = new System.Drawing.Point(2, 2);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(444, 60);
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // btnTransfer
            // 
            this.btnTransfer.Enabled = false;
            this.btnTransfer.Location = new System.Drawing.Point(6, 132);
            this.btnTransfer.Name = "btnTransfer";
            this.btnTransfer.Size = new System.Drawing.Size(120, 29);
            this.btnTransfer.TabIndex = 11;
            this.btnTransfer.Text = "3. TRANSFER";
            this.btnTransfer.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnTransfer.UseVisualStyleBackColor = true;
            this.btnTransfer.Click += new System.EventHandler(this.btnTransfer_Click);
            this.btnTransfer.EnabledChanged += new System.EventHandler(this.btnTransfer_EnabledChanged);
            // 
            // dtpTransfer
            // 
            this.dtpTransfer.CustomFormat = "dd MMM yyyy";
            this.dtpTransfer.Enabled = false;
            this.dtpTransfer.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpTransfer.Location = new System.Drawing.Point(239, 136);
            this.dtpTransfer.Name = "dtpTransfer";
            this.dtpTransfer.Size = new System.Drawing.Size(115, 20);
            this.dtpTransfer.TabIndex = 12;
            // 
            // lblDate
            // 
            this.lblDate.AutoSize = true;
            this.lblDate.Enabled = false;
            this.lblDate.Location = new System.Drawing.Point(133, 141);
            this.lblDate.Name = "lblDate";
            this.lblDate.Size = new System.Drawing.Size(91, 14);
            this.lblDate.TabIndex = 13;
            this.lblDate.Text = "TRANSFER DATE";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(133, 171);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(68, 14);
            this.label3.TabIndex = 15;
            this.label3.Text = "PAIR COUNT";
            this.label3.Visible = false;
            // 
            // optCountNow
            // 
            this.optCountNow.AutoSize = true;
            this.optCountNow.Checked = true;
            this.optCountNow.Location = new System.Drawing.Point(302, 170);
            this.optCountNow.Name = "optCountNow";
            this.optCountNow.Size = new System.Drawing.Size(104, 18);
            this.optCountNow.TabIndex = 16;
            this.optCountNow.TabStop = true;
            this.optCountNow.Text = "Maintain Existing";
            this.optCountNow.UseVisualStyleBackColor = true;
            this.optCountNow.Visible = false;
            // 
            // optCountNew
            // 
            this.optCountNew.AutoSize = true;
            this.optCountNew.Location = new System.Drawing.Point(239, 170);
            this.optCountNew.Name = "optCountNew";
            this.optCountNew.Size = new System.Drawing.Size(48, 18);
            this.optCountNew.TabIndex = 17;
            this.optCountNew.TabStop = true;
            this.optCountNew.Text = "New";
            this.optCountNew.UseVisualStyleBackColor = true;
            this.optCountNew.Visible = false;
            // 
            // prgProcess
            // 
            this.prgProcess.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.prgProcess.Location = new System.Drawing.Point(2, 233);
            this.prgProcess.Name = "prgProcess";
            this.prgProcess.Size = new System.Drawing.Size(440, 29);
            this.prgProcess.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.prgProcess.TabIndex = 18;
            this.prgProcess.Visible = false;
            // 
            // chkRecipient
            // 
            this.chkRecipient.AutoSize = true;
            this.chkRecipient.Location = new System.Drawing.Point(132, 74);
            this.chkRecipient.Name = "chkRecipient";
            this.chkRecipient.Size = new System.Drawing.Size(75, 18);
            this.chkRecipient.TabIndex = 19;
            this.chkRecipient.Text = "RECIPIENT";
            this.chkRecipient.UseVisualStyleBackColor = true;
            this.chkRecipient.CheckedChanged += new System.EventHandler(this.chkTransfer_CheckedChanged);
            // 
            // btnRecipient
            // 
            this.btnRecipient.Location = new System.Drawing.Point(6, 68);
            this.btnRecipient.Name = "btnRecipient";
            this.btnRecipient.Size = new System.Drawing.Size(120, 29);
            this.btnRecipient.TabIndex = 20;
            this.btnRecipient.Text = "1. PICK RECIPIENT";
            this.btnRecipient.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnRecipient.UseVisualStyleBackColor = true;
            this.btnRecipient.Click += new System.EventHandler(this.btnRecipient_Click);
            // 
            // btnDonor
            // 
            this.btnDonor.Enabled = false;
            this.btnDonor.Location = new System.Drawing.Point(6, 100);
            this.btnDonor.Name = "btnDonor";
            this.btnDonor.Size = new System.Drawing.Size(120, 29);
            this.btnDonor.TabIndex = 21;
            this.btnDonor.Text = "2. PICK DONOR";
            this.btnDonor.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnDonor.UseVisualStyleBackColor = true;
            this.btnDonor.Click += new System.EventHandler(this.btnDonor_Click);
            // 
            // chkDonor
            // 
            this.chkDonor.AutoSize = true;
            this.chkDonor.Location = new System.Drawing.Point(132, 106);
            this.chkDonor.Name = "chkDonor";
            this.chkDonor.Size = new System.Drawing.Size(63, 18);
            this.chkDonor.TabIndex = 22;
            this.chkDonor.Text = "DONOR";
            this.chkDonor.UseVisualStyleBackColor = true;
            this.chkDonor.CheckedChanged += new System.EventHandler(this.chkTransfer_CheckedChanged);
            // 
            // lblRecipient
            // 
            this.lblRecipient.AutoSize = true;
            this.lblRecipient.Location = new System.Drawing.Point(213, 75);
            this.lblRecipient.Name = "lblRecipient";
            this.lblRecipient.Size = new System.Drawing.Size(182, 14);
            this.lblRecipient.TabIndex = 24;
            this.lblRecipient.Text = "[ Recipient must be a stub or stump ]";
            // 
            // lblDonor
            // 
            this.lblDonor.AutoSize = true;
            this.lblDonor.Location = new System.Drawing.Point(213, 107);
            this.lblDonor.Name = "lblDonor";
            this.lblDonor.Size = new System.Drawing.Size(120, 14);
            this.lblDonor.TabIndex = 25;
            this.lblDonor.Text = "[ Donor must be a joint ]";
            // 
            // frmTransfer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(450, 263);
            this.Controls.Add(this.lblDonor);
            this.Controls.Add(this.lblRecipient);
            this.Controls.Add(this.chkDonor);
            this.Controls.Add(this.btnDonor);
            this.Controls.Add(this.btnRecipient);
            this.Controls.Add(this.chkRecipient);
            this.Controls.Add(this.prgProcess);
            this.Controls.Add(this.optCountNew);
            this.Controls.Add(this.optCountNow);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.lblDate);
            this.Controls.Add(this.dtpTransfer);
            this.Controls.Add(this.btnTransfer);
            this.Controls.Add(this.btnExit);
            this.Controls.Add(this.txtMessage);
            this.Controls.Add(this.btnReport);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.pictureBox2);
            this.Controls.Add(this.pictureBox1);
            this.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.MaximizeBox = false;
            this.MinimumSize = new System.Drawing.Size(445, 207);
            this.Name = "frmTransfer";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "TRANSFER Management 1.";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.frmTransfer_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnReport;
        private System.Windows.Forms.TextBox txtMessage;
        private System.Windows.Forms.Button btnExit;
        private System.Windows.Forms.Button btnTransfer;
        private System.Windows.Forms.DateTimePicker dtpTransfer;
        private System.Windows.Forms.Label lblDate;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.RadioButton optCountNow;
        private System.Windows.Forms.RadioButton optCountNew;
        private System.Windows.Forms.ProgressBar prgProcess;
        private System.Windows.Forms.CheckBox chkRecipient;
        private System.Windows.Forms.Button btnRecipient;
        private System.Windows.Forms.Button btnDonor;
        private System.Windows.Forms.CheckBox chkDonor;
        private System.Windows.Forms.Label lblRecipient;
        private System.Windows.Forms.Label lblDonor;



    }
}