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
            this.tabSyncronize = new System.Windows.Forms.TabPage();
            this.richTextBox2 = new System.Windows.Forms.RichTextBox();
            this.synStatus = new System.Windows.Forms.GroupBox();
            this.progressBar2 = new System.Windows.Forms.ProgressBar();
            this.cmdSynchronize = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.txtSynDstCableCode = new System.Windows.Forms.TextBox();
            this.cmdSynGetDst = new System.Windows.Forms.Button();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.txtSynSrcCblCode = new System.Windows.Forms.TextBox();
            this.cmdSynGetSrc = new System.Windows.Forms.Button();
            this.tabChangeCableCode = new System.Windows.Forms.TabPage();
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.checkCodeBtn = new System.Windows.Forms.Button();
            this.updateCableCode_ChangeBtn = new System.Windows.Forms.Button();
            this.newCableCodeTb = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.currCableCodeTb = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.getSelected_ChangeBtn = new System.Windows.Forms.Button();
            this.tabManage = new System.Windows.Forms.TabControl();
            this.label2 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            this.tabSyncronize.SuspendLayout();
            this.synStatus.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
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
            // tabSyncronize
            // 
            this.tabSyncronize.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.tabSyncronize.Controls.Add(this.richTextBox2);
            this.tabSyncronize.Controls.Add(this.synStatus);
            this.tabSyncronize.Controls.Add(this.cmdSynchronize);
            this.tabSyncronize.Controls.Add(this.label6);
            this.tabSyncronize.Controls.Add(this.groupBox3);
            this.tabSyncronize.Controls.Add(this.groupBox4);
            this.tabSyncronize.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tabSyncronize.Location = new System.Drawing.Point(4, 25);
            this.tabSyncronize.Name = "tabSyncronize";
            this.tabSyncronize.Size = new System.Drawing.Size(387, 252);
            this.tabSyncronize.TabIndex = 2;
            this.tabSyncronize.Text = "Synchronize Cable Code";
            this.tabSyncronize.UseVisualStyleBackColor = true;
            // 
            // richTextBox2
            // 
            this.richTextBox2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.richTextBox2.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.richTextBox2.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.richTextBox2.Location = new System.Drawing.Point(7, 182);
            this.richTextBox2.Name = "richTextBox2";
            this.richTextBox2.Size = new System.Drawing.Size(367, 58);
            this.richTextBox2.TabIndex = 13;
            this.richTextBox2.Text = "    [Note]\n   - Source Cable and Destination Cable MUST come from\n     the same t" +
                "ermination point";
            // 
            // synStatus
            // 
            this.synStatus.Controls.Add(this.progressBar2);
            this.synStatus.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.synStatus.Location = new System.Drawing.Point(7, 128);
            this.synStatus.Name = "synStatus";
            this.synStatus.Size = new System.Drawing.Size(367, 48);
            this.synStatus.TabIndex = 12;
            this.synStatus.TabStop = false;
            this.synStatus.Text = "[ Synchronization Progress ]";
            // 
            // progressBar2
            // 
            this.progressBar2.Location = new System.Drawing.Point(11, 21);
            this.progressBar2.Name = "progressBar2";
            this.progressBar2.Size = new System.Drawing.Size(350, 15);
            this.progressBar2.TabIndex = 10;
            // 
            // cmdSynchronize
            // 
            this.cmdSynchronize.Enabled = false;
            this.cmdSynchronize.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmdSynchronize.Location = new System.Drawing.Point(269, 54);
            this.cmdSynchronize.Name = "cmdSynchronize";
            this.cmdSynchronize.Size = new System.Drawing.Size(93, 30);
            this.cmdSynchronize.TabIndex = 6;
            this.cmdSynchronize.Text = "Synchronize";
            this.cmdSynchronize.UseVisualStyleBackColor = true;
            this.cmdSynchronize.Click += new System.EventHandler(this.cmdSynchronize_Click);
            // 
            // label6
            // 
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(249, 50);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(24, 37);
            this.label6.TabIndex = 7;
            this.label6.Text = "}";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.txtSynDstCableCode);
            this.groupBox3.Controls.Add(this.cmdSynGetDst);
            this.groupBox3.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox3.Location = new System.Drawing.Point(7, 65);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(258, 62);
            this.groupBox3.TabIndex = 3;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "[ Destination ]";
            // 
            // txtSynDstCableCode
            // 
            this.txtSynDstCableCode.BackColor = System.Drawing.SystemColors.InactiveCaptionText;
            this.txtSynDstCableCode.Enabled = false;
            this.txtSynDstCableCode.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtSynDstCableCode.Location = new System.Drawing.Point(194, 24);
            this.txtSynDstCableCode.Name = "txtSynDstCableCode";
            this.txtSynDstCableCode.Size = new System.Drawing.Size(53, 23);
            this.txtSynDstCableCode.TabIndex = 7;
            this.txtSynDstCableCode.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // cmdSynGetDst
            // 
            this.cmdSynGetDst.Location = new System.Drawing.Point(12, 20);
            this.cmdSynGetDst.Name = "cmdSynGetDst";
            this.cmdSynGetDst.Size = new System.Drawing.Size(150, 30);
            this.cmdSynGetDst.TabIndex = 5;
            this.cmdSynGetDst.Text = "Get Destination Cable Code";
            this.cmdSynGetDst.UseVisualStyleBackColor = true;
            this.cmdSynGetDst.Click += new System.EventHandler(this.cmdSynGetDst_Click);
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.txtSynSrcCblCode);
            this.groupBox4.Controls.Add(this.cmdSynGetSrc);
            this.groupBox4.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox4.Location = new System.Drawing.Point(7, 3);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(258, 62);
            this.groupBox4.TabIndex = 2;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "[ Source ]";
            // 
            // txtSynSrcCblCode
            // 
            this.txtSynSrcCblCode.BackColor = System.Drawing.SystemColors.InactiveCaptionText;
            this.txtSynSrcCblCode.Enabled = false;
            this.txtSynSrcCblCode.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtSynSrcCblCode.Location = new System.Drawing.Point(194, 26);
            this.txtSynSrcCblCode.Name = "txtSynSrcCblCode";
            this.txtSynSrcCblCode.Size = new System.Drawing.Size(53, 23);
            this.txtSynSrcCblCode.TabIndex = 4;
            this.txtSynSrcCblCode.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // cmdSynGetSrc
            // 
            this.cmdSynGetSrc.Location = new System.Drawing.Point(12, 20);
            this.cmdSynGetSrc.Name = "cmdSynGetSrc";
            this.cmdSynGetSrc.Size = new System.Drawing.Size(150, 30);
            this.cmdSynGetSrc.TabIndex = 1;
            this.cmdSynGetSrc.Text = "Get Source Cable Code";
            this.cmdSynGetSrc.UseVisualStyleBackColor = true;
            this.cmdSynGetSrc.Click += new System.EventHandler(this.cmdSynGetSrc_Click);
            // 
            // tabChangeCableCode
            // 
            this.tabChangeCableCode.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.tabChangeCableCode.Controls.Add(this.richTextBox1);
            this.tabChangeCableCode.Controls.Add(this.label8);
            this.tabChangeCableCode.Controls.Add(this.groupBox5);
            this.tabChangeCableCode.Controls.Add(this.groupBox2);
            this.tabChangeCableCode.Controls.Add(this.groupBox1);
            this.tabChangeCableCode.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tabChangeCableCode.Location = new System.Drawing.Point(4, 25);
            this.tabChangeCableCode.Name = "tabChangeCableCode";
            this.tabChangeCableCode.Padding = new System.Windows.Forms.Padding(3);
            this.tabChangeCableCode.Size = new System.Drawing.Size(387, 252);
            this.tabChangeCableCode.TabIndex = 1;
            this.tabChangeCableCode.Text = "Change Cable Code";
            this.tabChangeCableCode.UseVisualStyleBackColor = true;
            // 
            // richTextBox1
            // 
            this.richTextBox1.BackColor = System.Drawing.Color.White;
            this.richTextBox1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.richTextBox1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(192)))));
            this.richTextBox1.Location = new System.Drawing.Point(6, 176);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.Size = new System.Drawing.Size(371, 66);
            this.richTextBox1.TabIndex = 6;
            this.richTextBox1.Text = "   [Note]\n   - To edit E-Side Cable Code select TAIL. \n   - To edit D-Side Cable " +
                "Code, please select the first cable \n      from the termination point.";
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
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.checkCodeBtn);
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
            // checkCodeBtn
            // 
            this.checkCodeBtn.Enabled = false;
            this.checkCodeBtn.Location = new System.Drawing.Point(12, 18);
            this.checkCodeBtn.Name = "checkCodeBtn";
            this.checkCodeBtn.Size = new System.Drawing.Size(122, 31);
            this.checkCodeBtn.TabIndex = 11;
            this.checkCodeBtn.Text = "Check Available Code";
            this.checkCodeBtn.UseVisualStyleBackColor = true;
            this.checkCodeBtn.Click += new System.EventHandler(this.checkCodeBtn_Click);
            // 
            // updateCableCode_ChangeBtn
            // 
            this.updateCableCode_ChangeBtn.Enabled = false;
            this.updateCableCode_ChangeBtn.Location = new System.Drawing.Point(284, 16);
            this.updateCableCode_ChangeBtn.Name = "updateCableCode_ChangeBtn";
            this.updateCableCode_ChangeBtn.Size = new System.Drawing.Size(80, 30);
            this.updateCableCode_ChangeBtn.TabIndex = 7;
            this.updateCableCode_ChangeBtn.Text = "Update";
            this.updateCableCode_ChangeBtn.UseVisualStyleBackColor = true;
            this.updateCableCode_ChangeBtn.Click += new System.EventHandler(this.UpdateCableCode_ChangeBtn_Click);
            // 
            // newCableCodeTb
            // 
            this.newCableCodeTb.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.newCableCodeTb.Enabled = false;
            this.newCableCodeTb.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.newCableCodeTb.Location = new System.Drawing.Point(210, 20);
            this.newCableCodeTb.MaxLength = 4;
            this.newCableCodeTb.Name = "newCableCodeTb";
            this.newCableCodeTb.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.newCableCodeTb.Size = new System.Drawing.Size(58, 23);
            this.newCableCodeTb.TabIndex = 6;
            this.newCableCodeTb.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(149, 25);
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
            this.currCableCodeTb.BackColor = System.Drawing.SystemColors.InactiveCaptionText;
            this.currCableCodeTb.Enabled = false;
            this.currCableCodeTb.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.currCableCodeTb.Location = new System.Drawing.Point(210, 24);
            this.currCableCodeTb.Name = "currCableCodeTb";
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
            this.tabManage.Controls.Add(this.tabSyncronize);
            this.tabManage.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tabManage.Location = new System.Drawing.Point(7, 64);
            this.tabManage.Name = "tabManage";
            this.tabManage.SelectedIndex = 0;
            this.tabManage.Size = new System.Drawing.Size(395, 281);
            this.tabManage.TabIndex = 10;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.ForeColor = System.Drawing.Color.Red;
            this.label2.Location = new System.Drawing.Point(149, 49);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(111, 13);
            this.label2.TabIndex = 12;
            this.label2.Text = "[Capital letters ONLY]";
            // 
            // frmCableCode
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(407, 347);
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
            this.tabSyncronize.ResumeLayout(false);
            this.synStatus.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
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
        private System.Windows.Forms.TabPage tabSyncronize;
        private System.Windows.Forms.ProgressBar progressBar2;
        private System.Windows.Forms.Button cmdSynchronize;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.TextBox txtSynDstCableCode;
        private System.Windows.Forms.Button cmdSynGetDst;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.TextBox txtSynSrcCblCode;
        private System.Windows.Forms.Button cmdSynGetSrc;
        private System.Windows.Forms.TabPage tabChangeCableCode;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Button updateCableCode_ChangeBtn;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button checkCodeBtn;
        private System.Windows.Forms.TextBox newCableCodeTb;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox currCableCodeTb;
        private System.Windows.Forms.Button getSelected_ChangeBtn;
        private System.Windows.Forms.TabControl tabManage;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.GroupBox synStatus;
        private System.Windows.Forms.RichTextBox richTextBox1;
        private System.Windows.Forms.RichTextBox richTextBox2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;

    }
}