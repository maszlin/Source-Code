namespace NEPS.OSP.COPPER.COPY.DP
{
    partial class frmCopyDP
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
            this.tabManage = new System.Windows.Forms.TabControl();
            this.tabIdentifyParent = new System.Windows.Forms.TabPage();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.lblStatus = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.chkCopyBoundary = new System.Windows.Forms.CheckBox();
            this.btnApply = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.label9 = new System.Windows.Forms.Label();
            this.txtDPNumNew = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtDPNumSelected = new System.Windows.Forms.TextBox();
            this.txtRT_CODE = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.getSelectedBtn = new System.Windows.Forms.Button();
            this.txtITFACE_CODE = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.txtEXC_ABB = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.cmdGetParent = new System.Windows.Forms.Button();
            this.tabDPNum = new System.Windows.Forms.TabPage();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.textBox4 = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.textBox3 = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            this.tabManage.SuspendLayout();
            this.tabIdentifyParent.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.tabDPNum.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.SystemColors.GradientActiveCaption;
            this.pictureBox1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureBox1.Location = new System.Drawing.Point(2, 2);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(590, 56);
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // pictureBox2
            // 
            this.pictureBox2.BackColor = System.Drawing.SystemColors.GradientActiveCaption;
            this.pictureBox2.Image = global::NEPS.OSP.COPPER.COPY.DP.Properties.Resources.tnav_tmlogo;
            this.pictureBox2.Location = new System.Drawing.Point(6, 5);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(100, 50);
            this.pictureBox2.TabIndex = 1;
            this.pictureBox2.TabStop = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.SystemColors.GradientActiveCaption;
            this.label1.Font = new System.Drawing.Font("Verdana", 21.75F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.SystemColors.Desktop;
            this.label1.Location = new System.Drawing.Point(112, 14);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(183, 35);
            this.label1.TabIndex = 2;
            this.label1.Text = "NEPS-OSP";
            // 
            // tabManage
            // 
            this.tabManage.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tabManage.Appearance = System.Windows.Forms.TabAppearance.FlatButtons;
            this.tabManage.Controls.Add(this.tabIdentifyParent);
            this.tabManage.Controls.Add(this.tabDPNum);
            this.tabManage.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tabManage.Location = new System.Drawing.Point(2, 61);
            this.tabManage.Name = "tabManage";
            this.tabManage.SelectedIndex = 0;
            this.tabManage.Size = new System.Drawing.Size(296, 302);
            this.tabManage.TabIndex = 11;
            // 
            // tabIdentifyParent
            // 
            this.tabIdentifyParent.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.tabIdentifyParent.Controls.Add(this.groupBox1);
            this.tabIdentifyParent.Controls.Add(this.groupBox2);
            this.tabIdentifyParent.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tabIdentifyParent.Location = new System.Drawing.Point(4, 25);
            this.tabIdentifyParent.Name = "tabIdentifyParent";
            this.tabIdentifyParent.Padding = new System.Windows.Forms.Padding(3);
            this.tabIdentifyParent.Size = new System.Drawing.Size(288, 273);
            this.tabIdentifyParent.TabIndex = 1;
            this.tabIdentifyParent.Text = "Copy DP";
            this.tabIdentifyParent.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.lblStatus);
            this.groupBox1.Location = new System.Drawing.Point(3, 211);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(278, 54);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Status";
            // 
            // lblStatus
            // 
            this.lblStatus.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.lblStatus.Location = new System.Drawing.Point(3, 14);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(272, 34);
            this.lblStatus.TabIndex = 14;
            this.lblStatus.Text = "Select DP to copy";
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.chkCopyBoundary);
            this.groupBox2.Controls.Add(this.btnApply);
            this.groupBox2.Controls.Add(this.btnClose);
            this.groupBox2.Controls.Add(this.label9);
            this.groupBox2.Controls.Add(this.txtDPNumNew);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.txtDPNumSelected);
            this.groupBox2.Controls.Add(this.txtRT_CODE);
            this.groupBox2.Controls.Add(this.label8);
            this.groupBox2.Controls.Add(this.getSelectedBtn);
            this.groupBox2.Controls.Add(this.txtITFACE_CODE);
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Controls.Add(this.txtEXC_ABB);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.cmdGetParent);
            this.groupBox2.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox2.Location = new System.Drawing.Point(3, 3);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(278, 208);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Properties";
            // 
            // chkCopyBoundary
            // 
            this.chkCopyBoundary.AutoSize = true;
            this.chkCopyBoundary.Location = new System.Drawing.Point(14, 179);
            this.chkCopyBoundary.Name = "chkCopyBoundary";
            this.chkCopyBoundary.Size = new System.Drawing.Size(138, 17);
            this.chkCopyBoundary.TabIndex = 13;
            this.chkCopyBoundary.Text = "Copy Service Boundary";
            this.chkCopyBoundary.UseVisualStyleBackColor = true;
            this.chkCopyBoundary.Visible = false;
            // 
            // btnApply
            // 
            this.btnApply.Enabled = false;
            this.btnApply.Location = new System.Drawing.Point(188, 108);
            this.btnApply.Name = "btnApply";
            this.btnApply.Size = new System.Drawing.Size(84, 42);
            this.btnApply.TabIndex = 12;
            this.btnApply.Text = "Update DP Attribute";
            this.btnApply.UseVisualStyleBackColor = true;
            this.btnApply.Click += new System.EventHandler(this.btnApply_Click);
            // 
            // btnClose
            // 
            this.btnClose.Location = new System.Drawing.Point(188, 155);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(84, 42);
            this.btnClose.TabIndex = 11;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(11, 148);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(86, 13);
            this.label9.TabIndex = 9;
            this.label9.Text = "Next DP Number";
            // 
            // txtDPNumNew
            // 
            this.txtDPNumNew.BackColor = System.Drawing.Color.White;
            this.txtDPNumNew.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtDPNumNew.ForeColor = System.Drawing.Color.Black;
            this.txtDPNumNew.Location = new System.Drawing.Point(106, 143);
            this.txtDPNumNew.Name = "txtDPNumNew";
            this.txtDPNumNew.Size = new System.Drawing.Size(67, 23);
            this.txtDPNumNew.TabIndex = 10;
            this.txtDPNumNew.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.txtDPNumNew.Leave += new System.EventHandler(this.txtDPNumNew_Leave);
            this.txtDPNumNew.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtDPNumNew_KeyPress);
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(11, 114);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(62, 32);
            this.label3.TabIndex = 3;
            this.label3.Text = "Selected DP Number";
            // 
            // txtDPNumSelected
            // 
            this.txtDPNumSelected.BackColor = System.Drawing.Color.WhiteSmoke;
            this.txtDPNumSelected.Enabled = false;
            this.txtDPNumSelected.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtDPNumSelected.ForeColor = System.Drawing.Color.Black;
            this.txtDPNumSelected.Location = new System.Drawing.Point(106, 114);
            this.txtDPNumSelected.Name = "txtDPNumSelected";
            this.txtDPNumSelected.Size = new System.Drawing.Size(67, 23);
            this.txtDPNumSelected.TabIndex = 4;
            this.txtDPNumSelected.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.txtDPNumSelected.TextChanged += new System.EventHandler(this.txtProp_TextChanged);
            // 
            // txtRT_CODE
            // 
            this.txtRT_CODE.BackColor = System.Drawing.Color.WhiteSmoke;
            this.txtRT_CODE.Enabled = false;
            this.txtRT_CODE.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtRT_CODE.ForeColor = System.Drawing.Color.Black;
            this.txtRT_CODE.Location = new System.Drawing.Point(106, 73);
            this.txtRT_CODE.Name = "txtRT_CODE";
            this.txtRT_CODE.ReadOnly = true;
            this.txtRT_CODE.Size = new System.Drawing.Size(67, 23);
            this.txtRT_CODE.TabIndex = 8;
            this.txtRT_CODE.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.txtRT_CODE.TextChanged += new System.EventHandler(this.txtProp_TextChanged);
            this.txtRT_CODE.Enter += new System.EventHandler(this.txtProp_Enter);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(11, 78);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(48, 13);
            this.label8.TabIndex = 7;
            this.label8.Text = "RT Code";
            // 
            // getSelectedBtn
            // 
            this.getSelectedBtn.Location = new System.Drawing.Point(188, 15);
            this.getSelectedBtn.Name = "getSelectedBtn";
            this.getSelectedBtn.Size = new System.Drawing.Size(84, 42);
            this.getSelectedBtn.TabIndex = 1;
            this.getSelectedBtn.Text = "Get DP";
            this.getSelectedBtn.UseVisualStyleBackColor = true;
            this.getSelectedBtn.Click += new System.EventHandler(this.cmdChgGetSelected_Click);
            // 
            // txtITFACE_CODE
            // 
            this.txtITFACE_CODE.BackColor = System.Drawing.Color.WhiteSmoke;
            this.txtITFACE_CODE.Enabled = false;
            this.txtITFACE_CODE.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtITFACE_CODE.ForeColor = System.Drawing.Color.Black;
            this.txtITFACE_CODE.Location = new System.Drawing.Point(106, 44);
            this.txtITFACE_CODE.Name = "txtITFACE_CODE";
            this.txtITFACE_CODE.ReadOnly = true;
            this.txtITFACE_CODE.Size = new System.Drawing.Size(67, 23);
            this.txtITFACE_CODE.TabIndex = 6;
            this.txtITFACE_CODE.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.txtITFACE_CODE.TextChanged += new System.EventHandler(this.txtProp_TextChanged);
            this.txtITFACE_CODE.Enter += new System.EventHandler(this.txtProp_Enter);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(11, 49);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(72, 13);
            this.label7.TabIndex = 5;
            this.label7.Text = "Cabinet Code";
            // 
            // txtEXC_ABB
            // 
            this.txtEXC_ABB.BackColor = System.Drawing.Color.WhiteSmoke;
            this.txtEXC_ABB.Enabled = false;
            this.txtEXC_ABB.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtEXC_ABB.ForeColor = System.Drawing.Color.Black;
            this.txtEXC_ABB.Location = new System.Drawing.Point(106, 15);
            this.txtEXC_ABB.Name = "txtEXC_ABB";
            this.txtEXC_ABB.ReadOnly = true;
            this.txtEXC_ABB.Size = new System.Drawing.Size(67, 23);
            this.txtEXC_ABB.TabIndex = 4;
            this.txtEXC_ABB.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.txtEXC_ABB.TextChanged += new System.EventHandler(this.txtProp_TextChanged);
            this.txtEXC_ABB.Enter += new System.EventHandler(this.txtProp_Enter);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(11, 20);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(80, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Exchange Abb.";
            // 
            // cmdGetParent
            // 
            this.cmdGetParent.Enabled = false;
            this.cmdGetParent.Location = new System.Drawing.Point(188, 61);
            this.cmdGetParent.Name = "cmdGetParent";
            this.cmdGetParent.Size = new System.Drawing.Size(84, 42);
            this.cmdGetParent.TabIndex = 1;
            this.cmdGetParent.Text = "Get Parent";
            this.cmdGetParent.UseVisualStyleBackColor = true;
            this.cmdGetParent.Click += new System.EventHandler(this.cmdGetParent_Click);
            // 
            // tabDPNum
            // 
            this.tabDPNum.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.tabDPNum.Controls.Add(this.groupBox4);
            this.tabDPNum.Controls.Add(this.groupBox3);
            this.tabDPNum.Location = new System.Drawing.Point(4, 25);
            this.tabDPNum.Name = "tabDPNum";
            this.tabDPNum.Size = new System.Drawing.Size(288, 273);
            this.tabDPNum.TabIndex = 2;
            this.tabDPNum.Text = "Copy DP (Insert DP Number)";
            this.tabDPNum.UseVisualStyleBackColor = true;
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.textBox4);
            this.groupBox4.Controls.Add(this.label6);
            this.groupBox4.Controls.Add(this.textBox3);
            this.groupBox4.Controls.Add(this.label5);
            this.groupBox4.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox4.Location = new System.Drawing.Point(207, 1);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(365, 62);
            this.groupBox4.TabIndex = 2;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "[ DP Number ]";
            // 
            // textBox4
            // 
            this.textBox4.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.textBox4.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox4.Location = new System.Drawing.Point(287, 24);
            this.textBox4.Name = "textBox4";
            this.textBox4.Size = new System.Drawing.Size(65, 23);
            this.textBox4.TabIndex = 6;
            this.textBox4.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(219, 29);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(67, 13);
            this.label6.TabIndex = 5;
            this.label6.Text = "Set DP Num:";
            // 
            // textBox3
            // 
            this.textBox3.BackColor = System.Drawing.SystemColors.InactiveCaptionText;
            this.textBox3.Enabled = false;
            this.textBox3.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox3.Location = new System.Drawing.Point(146, 24);
            this.textBox3.Name = "textBox3";
            this.textBox3.Size = new System.Drawing.Size(62, 23);
            this.textBox3.TabIndex = 4;
            this.textBox3.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(10, 29);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(135, 13);
            this.label5.TabIndex = 3;
            this.label5.Text = "Next DP Num (by System):";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.textBox2);
            this.groupBox3.Controls.Add(this.label4);
            this.groupBox3.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox3.Location = new System.Drawing.Point(6, 1);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(195, 62);
            this.groupBox3.TabIndex = 1;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "[ DP Information ]";
            // 
            // textBox2
            // 
            this.textBox2.BackColor = System.Drawing.SystemColors.InactiveCaptionText;
            this.textBox2.Enabled = false;
            this.textBox2.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox2.Location = new System.Drawing.Point(117, 24);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(67, 23);
            this.textBox2.TabIndex = 4;
            this.textBox2.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(9, 29);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(108, 13);
            this.label4.TabIndex = 3;
            this.label4.Text = "Source Copy DP FID:";
            // 
            // frmCopyDP
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(299, 365);
            this.Controls.Add(this.tabManage);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.pictureBox2);
            this.Controls.Add(this.pictureBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.Name = "frmCopyDP";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Copy DP 1.2";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.frmFrame_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            this.tabManage.ResumeLayout(false);
            this.tabIdentifyParent.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.tabDPNum.ResumeLayout(false);
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TabControl tabManage;
        private System.Windows.Forms.TabPage tabIdentifyParent;
        private System.Windows.Forms.TextBox txtDPNumSelected;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button getSelectedBtn;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TextBox txtEXC_ABB;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button cmdGetParent;
        private System.Windows.Forms.TabPage tabDPNum;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.TextBox textBox4;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox textBox3;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtITFACE_CODE;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox txtRT_CODE;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox txtDPNumNew;
        private System.Windows.Forms.Button btnApply;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.CheckBox chkCopyBoundary;



    }
}