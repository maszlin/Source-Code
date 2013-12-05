namespace NEPS.OSP.COPPER.CABLE.CODE
{
    partial class frmCableCode
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
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.label1 = new System.Windows.Forms.Label();
            this.tabChangeCableCode = new System.Windows.Forms.TabPage();
            this.rtbNote = new System.Windows.Forms.RichTextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.updateCableCode_ChangeBtn = new System.Windows.Forms.Button();
            this.newCableCodeTb = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.currCableCodeTb = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.getSelected_ChangeBtn = new System.Windows.Forms.Button();
            this.tabManage = new System.Windows.Forms.TabControl();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            this.tabChangeCableCode.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.tabManage.SuspendLayout();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.SystemColors.GradientActiveCaption;
            this.pictureBox1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureBox1.Location = new System.Drawing.Point(4, 2);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(399, 56);
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // pictureBox2
            // 
            this.pictureBox2.BackColor = System.Drawing.SystemColors.GradientActiveCaption;
            this.pictureBox2.Image = global::NEPS.OSP.COPPER.CABLE.CODE.Properties.Resources.tnav_tmlogo;
            this.pictureBox2.Location = new System.Drawing.Point(8, 5);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(99, 53);
            this.pictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox2.TabIndex = 1;
            this.pictureBox2.TabStop = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.SystemColors.GradientActiveCaption;
            this.label1.Font = new System.Drawing.Font("Verdana", 20.25F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.SystemColors.Desktop;
            this.label1.Location = new System.Drawing.Point(103, 14);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(299, 32);
            this.label1.TabIndex = 2;
            this.label1.Text = "NEPS-OSP SYSTEM";
            // 
            // tabChangeCableCode
            // 
            this.tabChangeCableCode.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.tabChangeCableCode.Controls.Add(this.rtbNote);
            this.tabChangeCableCode.Controls.Add(this.label8);
            this.tabChangeCableCode.Controls.Add(this.groupBox5);
            this.tabChangeCableCode.Controls.Add(this.groupBox2);
            this.tabChangeCableCode.Controls.Add(this.groupBox1);
            this.tabChangeCableCode.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tabChangeCableCode.Location = new System.Drawing.Point(4, 25);
            this.tabChangeCableCode.Name = "tabChangeCableCode";
            this.tabChangeCableCode.Padding = new System.Windows.Forms.Padding(3);
            this.tabChangeCableCode.Size = new System.Drawing.Size(387, 282);
            this.tabChangeCableCode.TabIndex = 1;
            this.tabChangeCableCode.Text = "Change Cable Code";
            this.tabChangeCableCode.UseVisualStyleBackColor = true;
            // 
            // rtbNote
            // 
            this.rtbNote.BackColor = System.Drawing.Color.White;
            this.rtbNote.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.rtbNote.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(192)))));
            this.rtbNote.Location = new System.Drawing.Point(6, 176);
            this.rtbNote.Name = "rtbNote";
            this.rtbNote.Size = new System.Drawing.Size(371, 96);
            this.rtbNote.TabIndex = 6;
            this.rtbNote.Text = "   [Note]\n   - please select cable to change \n   - and click on [Get Cable Code] " +
                "button";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.ForeColor = System.Drawing.Color.Blue;
            this.label8.Location = new System.Drawing.Point(8, 194);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(7, 13);
            this.label8.TabIndex = 5;
            this.label8.Text = "\r\n";
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.progressBar1);
            this.groupBox5.Location = new System.Drawing.Point(7, 127);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(370, 43);
            this.groupBox5.TabIndex = 2;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "[Update Progress]";
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(11, 20);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(350, 15);
            this.progressBar1.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.progressBar1.TabIndex = 9;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.updateCableCode_ChangeBtn);
            this.groupBox2.Controls.Add(this.newCableCodeTb);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Location = new System.Drawing.Point(7, 62);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(370, 66);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "[ New Information ]";
            // 
            // updateCableCode_ChangeBtn
            // 
            this.updateCableCode_ChangeBtn.Enabled = false;
            this.updateCableCode_ChangeBtn.Location = new System.Drawing.Point(12, 25);
            this.updateCableCode_ChangeBtn.Name = "updateCableCode_ChangeBtn";
            this.updateCableCode_ChangeBtn.Size = new System.Drawing.Size(122, 30);
            this.updateCableCode_ChangeBtn.TabIndex = 7;
            this.updateCableCode_ChangeBtn.Text = "Change Code";
            this.updateCableCode_ChangeBtn.UseVisualStyleBackColor = true;
            this.updateCableCode_ChangeBtn.Click += new System.EventHandler(this.UpdateCableCode_ChangeBtn_Click);
            // 
            // newCableCodeTb
            // 
            this.newCableCodeTb.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.newCableCodeTb.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.newCableCodeTb.Enabled = false;
            this.newCableCodeTb.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.newCableCodeTb.Location = new System.Drawing.Point(210, 29);
            this.newCableCodeTb.MaxLength = 4;
            this.newCableCodeTb.Name = "newCableCodeTb";
            this.newCableCodeTb.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.newCableCodeTb.Size = new System.Drawing.Size(58, 23);
            this.newCableCodeTb.TabIndex = 6;
            this.newCableCodeTb.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.newCableCodeTb.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.newCableCodeTb_KeyPress);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(149, 34);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(35, 13);
            this.label4.TabIndex = 5;
            this.label4.Text = "New :";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.currCableCodeTb);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.getSelected_ChangeBtn);
            this.groupBox1.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox1.Location = new System.Drawing.Point(7, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(370, 58);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "[ Identify Current Information ]";
            // 
            // currCableCodeTb
            // 
            this.currCableCodeTb.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.currCableCodeTb.Enabled = false;
            this.currCableCodeTb.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.currCableCodeTb.Location = new System.Drawing.Point(210, 24);
            this.currCableCodeTb.Name = "currCableCodeTb";
            this.currCableCodeTb.ReadOnly = true;
            this.currCableCodeTb.Size = new System.Drawing.Size(58, 23);
            this.currCableCodeTb.TabIndex = 4;
            this.currCableCodeTb.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(149, 29);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(51, 13);
            this.label3.TabIndex = 3;
            this.label3.Text = "Current :";
            // 
            // getSelected_ChangeBtn
            // 
            this.getSelected_ChangeBtn.Location = new System.Drawing.Point(12, 20);
            this.getSelected_ChangeBtn.Name = "getSelected_ChangeBtn";
            this.getSelected_ChangeBtn.Size = new System.Drawing.Size(122, 30);
            this.getSelected_ChangeBtn.TabIndex = 1;
            this.getSelected_ChangeBtn.Text = "Get Cable Code";
            this.getSelected_ChangeBtn.UseVisualStyleBackColor = true;
            this.getSelected_ChangeBtn.Click += new System.EventHandler(this.getSelected_ChangeBtn_Click);
            // 
            // tabManage
            // 
            this.tabManage.Appearance = System.Windows.Forms.TabAppearance.FlatButtons;
            this.tabManage.Controls.Add(this.tabChangeCableCode);
            this.tabManage.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tabManage.Location = new System.Drawing.Point(7, 64);
            this.tabManage.Name = "tabManage";
            this.tabManage.SelectedIndex = 0;
            this.tabManage.Size = new System.Drawing.Size(395, 311);
            this.tabManage.TabIndex = 10;
            // 
            // frmCableCode
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(407, 394);
            this.Controls.Add(this.pictureBox2);
            this.Controls.Add(this.tabManage);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.pictureBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "frmCableCode";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Manage Cable Code";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.frmFrame_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            this.tabChangeCableCode.ResumeLayout(false);
            this.tabChangeCableCode.PerformLayout();
            this.groupBox5.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.tabManage.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TabPage tabChangeCableCode;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Button updateCableCode_ChangeBtn;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TextBox newCableCodeTb;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox currCableCodeTb;
        private System.Windows.Forms.Button getSelected_ChangeBtn;
        private System.Windows.Forms.TabControl tabManage;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.RichTextBox rtbNote;
        private System.Windows.Forms.Label label3;

    }
}