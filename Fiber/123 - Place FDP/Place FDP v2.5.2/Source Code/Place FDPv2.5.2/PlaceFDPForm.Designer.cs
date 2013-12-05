namespace NEPS.GTechnology.PlaceFDP
{
    partial class GTWindowsForm_PlaceFDP
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
            this.tbFDP = new System.Windows.Forms.TabControl();
            this.tPlaceFDP = new System.Windows.Forms.TabPage();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.btn_Pick = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.txtParantCode = new System.Windows.Forms.TextBox();
            this.lblParentDevice = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.txtParant = new System.Windows.Forms.TextBox();
            this.lblSplitter1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.cmbFeature = new System.Windows.Forms.ComboBox();
            this.btn_Close = new System.Windows.Forms.Button();
            this.lblSplitter2 = new System.Windows.Forms.Label();
            this.btn_Generate = new System.Windows.Forms.Button();
            this.cmbSplitter2 = new System.Windows.Forms.ComboBox();
            this.btn_Clear = new System.Windows.Forms.Button();
            this.cmbSplitter1 = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.cmbType = new System.Windows.Forms.ComboBox();
            this.tCopyFDP = new System.Windows.Forms.TabPage();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label6 = new System.Windows.Forms.Label();
            this.txtCopyFDPFID = new System.Windows.Forms.TextBox();
            this.txtCopyFDPType = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.txtCopyFDPCode = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.btn_PickCopy = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.btn_Clear1 = new System.Windows.Forms.Button();
            this.btn_CopyBND = new System.Windows.Forms.Button();
            this.tbFDP.SuspendLayout();
            this.tPlaceFDP.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.tCopyFDP.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tbFDP
            // 
            this.tbFDP.Controls.Add(this.tPlaceFDP);
            this.tbFDP.Controls.Add(this.tCopyFDP);
            this.tbFDP.Location = new System.Drawing.Point(12, 12);
            this.tbFDP.Name = "tbFDP";
            this.tbFDP.SelectedIndex = 0;
            this.tbFDP.Size = new System.Drawing.Size(378, 289);
            this.tbFDP.TabIndex = 1;
            // 
            // tPlaceFDP
            // 
            this.tPlaceFDP.Controls.Add(this.groupBox2);
            this.tPlaceFDP.Controls.Add(this.lblSplitter1);
            this.tPlaceFDP.Controls.Add(this.label2);
            this.tPlaceFDP.Controls.Add(this.cmbFeature);
            this.tPlaceFDP.Controls.Add(this.btn_Close);
            this.tPlaceFDP.Controls.Add(this.lblSplitter2);
            this.tPlaceFDP.Controls.Add(this.btn_Generate);
            this.tPlaceFDP.Controls.Add(this.cmbSplitter2);
            this.tPlaceFDP.Controls.Add(this.btn_Clear);
            this.tPlaceFDP.Controls.Add(this.cmbSplitter1);
            this.tPlaceFDP.Controls.Add(this.label4);
            this.tPlaceFDP.Controls.Add(this.cmbType);
            this.tPlaceFDP.Location = new System.Drawing.Point(4, 22);
            this.tPlaceFDP.Name = "tPlaceFDP";
            this.tPlaceFDP.Padding = new System.Windows.Forms.Padding(3);
            this.tPlaceFDP.Size = new System.Drawing.Size(370, 263);
            this.tPlaceFDP.TabIndex = 0;
            this.tPlaceFDP.Text = "Place FDP";
            this.tPlaceFDP.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.btn_Pick);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.txtParantCode);
            this.groupBox2.Controls.Add(this.lblParentDevice);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.txtParant);
            this.groupBox2.Location = new System.Drawing.Point(22, 43);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(327, 83);
            this.groupBox2.TabIndex = 29;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Parent Device :";
            // 
            // btn_Pick
            // 
            this.btn_Pick.Location = new System.Drawing.Point(243, 29);
            this.btn_Pick.Name = "btn_Pick";
            this.btn_Pick.Size = new System.Drawing.Size(75, 21);
            this.btn_Pick.TabIndex = 24;
            this.btn_Pick.Text = "Pick";
            this.btn_Pick.UseVisualStyleBackColor = true;
            this.btn_Pick.Click += new System.EventHandler(this.btn_Pick_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(6, 55);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(32, 13);
            this.label5.TabIndex = 27;
            this.label5.Text = "Code";
            // 
            // txtParantCode
            // 
            this.txtParantCode.BackColor = System.Drawing.Color.White;
            this.txtParantCode.ForeColor = System.Drawing.Color.Black;
            this.txtParantCode.Location = new System.Drawing.Point(100, 57);
            this.txtParantCode.Name = "txtParantCode";
            this.txtParantCode.ReadOnly = true;
            this.txtParantCode.Size = new System.Drawing.Size(137, 20);
            this.txtParantCode.TabIndex = 28;
            // 
            // lblParentDevice
            // 
            this.lblParentDevice.AutoSize = true;
            this.lblParentDevice.Location = new System.Drawing.Point(102, 14);
            this.lblParentDevice.Name = "lblParentDevice";
            this.lblParentDevice.Size = new System.Drawing.Size(37, 13);
            this.lblParentDevice.TabIndex = 23;
            this.lblParentDevice.Text = "parent";
            this.lblParentDevice.Visible = false;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 30);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(27, 13);
            this.label3.TabIndex = 25;
            this.label3.Text = " FID";
            // 
            // txtParant
            // 
            this.txtParant.BackColor = System.Drawing.Color.White;
            this.txtParant.ForeColor = System.Drawing.Color.Black;
            this.txtParant.Location = new System.Drawing.Point(100, 30);
            this.txtParant.Name = "txtParant";
            this.txtParant.ReadOnly = true;
            this.txtParant.Size = new System.Drawing.Size(137, 20);
            this.txtParant.TabIndex = 26;
            // 
            // lblSplitter1
            // 
            this.lblSplitter1.AutoSize = true;
            this.lblSplitter1.Location = new System.Drawing.Point(19, 168);
            this.lblSplitter1.Name = "lblSplitter1";
            this.lblSplitter1.Size = new System.Drawing.Size(54, 13);
            this.lblSplitter1.TabIndex = 1;
            this.lblSplitter1.Text = "Splitter 1 :";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(19, 20);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(76, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Feature Type :";
            // 
            // cmbFeature
            // 
            this.cmbFeature.BackColor = System.Drawing.Color.White;
            this.cmbFeature.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbFeature.ForeColor = System.Drawing.Color.Black;
            this.cmbFeature.FormattingEnabled = true;
            this.cmbFeature.Location = new System.Drawing.Point(122, 16);
            this.cmbFeature.Name = "cmbFeature";
            this.cmbFeature.Size = new System.Drawing.Size(137, 21);
            this.cmbFeature.TabIndex = 4;
            this.cmbFeature.SelectedIndexChanged += new System.EventHandler(this.cmbFeature_SelectedIndexChanged);
            // 
            // btn_Close
            // 
            this.btn_Close.Location = new System.Drawing.Point(265, 225);
            this.btn_Close.Name = "btn_Close";
            this.btn_Close.Size = new System.Drawing.Size(75, 21);
            this.btn_Close.TabIndex = 2;
            this.btn_Close.Text = "Close";
            this.btn_Close.UseVisualStyleBackColor = true;
            this.btn_Close.Click += new System.EventHandler(this.button1_Click);
            // 
            // lblSplitter2
            // 
            this.lblSplitter2.AutoSize = true;
            this.lblSplitter2.Location = new System.Drawing.Point(19, 195);
            this.lblSplitter2.Name = "lblSplitter2";
            this.lblSplitter2.Size = new System.Drawing.Size(54, 13);
            this.lblSplitter2.TabIndex = 21;
            this.lblSplitter2.Text = "Splitter 2 :";
            // 
            // btn_Generate
            // 
            this.btn_Generate.Location = new System.Drawing.Point(122, 218);
            this.btn_Generate.Name = "btn_Generate";
            this.btn_Generate.Size = new System.Drawing.Size(86, 28);
            this.btn_Generate.TabIndex = 4;
            this.btn_Generate.Text = "Generate FDP";
            this.btn_Generate.UseVisualStyleBackColor = true;
            this.btn_Generate.Click += new System.EventHandler(this.button3_Click);
            // 
            // cmbSplitter2
            // 
            this.cmbSplitter2.BackColor = System.Drawing.Color.White;
            this.cmbSplitter2.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbSplitter2.ForeColor = System.Drawing.Color.Black;
            this.cmbSplitter2.FormattingEnabled = true;
            this.cmbSplitter2.Location = new System.Drawing.Point(122, 192);
            this.cmbSplitter2.Name = "cmbSplitter2";
            this.cmbSplitter2.Size = new System.Drawing.Size(137, 21);
            this.cmbSplitter2.TabIndex = 20;
            // 
            // btn_Clear
            // 
            this.btn_Clear.Location = new System.Drawing.Point(265, 198);
            this.btn_Clear.Name = "btn_Clear";
            this.btn_Clear.Size = new System.Drawing.Size(75, 21);
            this.btn_Clear.TabIndex = 3;
            this.btn_Clear.Text = "Clear";
            this.btn_Clear.UseVisualStyleBackColor = true;
            this.btn_Clear.Click += new System.EventHandler(this.btn_Clear_Click);
            // 
            // cmbSplitter1
            // 
            this.cmbSplitter1.BackColor = System.Drawing.Color.White;
            this.cmbSplitter1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbSplitter1.ForeColor = System.Drawing.Color.Black;
            this.cmbSplitter1.FormattingEnabled = true;
            this.cmbSplitter1.Location = new System.Drawing.Point(122, 165);
            this.cmbSplitter1.Name = "cmbSplitter1";
            this.cmbSplitter1.Size = new System.Drawing.Size(137, 21);
            this.cmbSplitter1.TabIndex = 19;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(19, 141);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(61, 13);
            this.label4.TabIndex = 17;
            this.label4.Text = "FDP Type :";
            // 
            // cmbType
            // 
            this.cmbType.BackColor = System.Drawing.Color.White;
            this.cmbType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbType.ForeColor = System.Drawing.Color.Black;
            this.cmbType.FormattingEnabled = true;
            this.cmbType.Location = new System.Drawing.Point(122, 138);
            this.cmbType.Name = "cmbType";
            this.cmbType.Size = new System.Drawing.Size(137, 21);
            this.cmbType.TabIndex = 18;
            this.cmbType.SelectedIndexChanged += new System.EventHandler(this.cmbType_SelectedIndexChanged);
            // 
            // tCopyFDP
            // 
            this.tCopyFDP.Controls.Add(this.groupBox1);
            this.tCopyFDP.Controls.Add(this.button1);
            this.tCopyFDP.Controls.Add(this.btn_Clear1);
            this.tCopyFDP.Controls.Add(this.btn_CopyBND);
            this.tCopyFDP.Location = new System.Drawing.Point(4, 22);
            this.tCopyFDP.Name = "tCopyFDP";
            this.tCopyFDP.Padding = new System.Windows.Forms.Padding(3);
            this.tCopyFDP.Size = new System.Drawing.Size(370, 263);
            this.tCopyFDP.TabIndex = 1;
            this.tCopyFDP.Text = "Copy FDP";
            this.tCopyFDP.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.txtCopyFDPFID);
            this.groupBox1.Controls.Add(this.txtCopyFDPType);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.txtCopyFDPCode);
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Controls.Add(this.btn_PickCopy);
            this.groupBox1.Location = new System.Drawing.Point(25, 35);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(327, 107);
            this.groupBox1.TabIndex = 40;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Copying feature:";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(6, 80);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(27, 13);
            this.label6.TabIndex = 40;
            this.label6.Text = "FID:";
            // 
            // txtCopyFDPFID
            // 
            this.txtCopyFDPFID.BackColor = System.Drawing.Color.White;
            this.txtCopyFDPFID.ForeColor = System.Drawing.Color.Black;
            this.txtCopyFDPFID.Location = new System.Drawing.Point(85, 73);
            this.txtCopyFDPFID.Name = "txtCopyFDPFID";
            this.txtCopyFDPFID.Size = new System.Drawing.Size(137, 20);
            this.txtCopyFDPFID.TabIndex = 39;
            // 
            // txtCopyFDPType
            // 
            this.txtCopyFDPType.BackColor = System.Drawing.Color.White;
            this.txtCopyFDPType.ForeColor = System.Drawing.Color.Black;
            this.txtCopyFDPType.Location = new System.Drawing.Point(85, 22);
            this.txtCopyFDPType.Name = "txtCopyFDPType";
            this.txtCopyFDPType.Size = new System.Drawing.Size(137, 20);
            this.txtCopyFDPType.TabIndex = 38;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 54);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 13);
            this.label1.TabIndex = 37;
            this.label1.Text = "Code:";
            // 
            // txtCopyFDPCode
            // 
            this.txtCopyFDPCode.BackColor = System.Drawing.Color.White;
            this.txtCopyFDPCode.ForeColor = System.Drawing.Color.Black;
            this.txtCopyFDPCode.Location = new System.Drawing.Point(85, 47);
            this.txtCopyFDPCode.Name = "txtCopyFDPCode";
            this.txtCopyFDPCode.Size = new System.Drawing.Size(137, 20);
            this.txtCopyFDPCode.TabIndex = 36;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(6, 26);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(37, 13);
            this.label8.TabIndex = 35;
            this.label8.Text = "Type :";
            // 
            // btn_PickCopy
            // 
            this.btn_PickCopy.Location = new System.Drawing.Point(246, 19);
            this.btn_PickCopy.Name = "btn_PickCopy";
            this.btn_PickCopy.Size = new System.Drawing.Size(75, 23);
            this.btn_PickCopy.TabIndex = 34;
            this.btn_PickCopy.Text = "Pick";
            this.btn_PickCopy.UseVisualStyleBackColor = true;
            this.btn_PickCopy.Click += new System.EventHandler(this.btn_PickCopy_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(242, 173);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 28);
            this.button1.TabIndex = 37;
            this.button1.Text = "Close";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click_1);
            // 
            // btn_Clear1
            // 
            this.btn_Clear1.Location = new System.Drawing.Point(156, 173);
            this.btn_Clear1.Name = "btn_Clear1";
            this.btn_Clear1.Size = new System.Drawing.Size(75, 28);
            this.btn_Clear1.TabIndex = 38;
            this.btn_Clear1.Text = "Clear";
            this.btn_Clear1.UseVisualStyleBackColor = true;
            this.btn_Clear1.Click += new System.EventHandler(this.btn_Clear1_Click);
            // 
            // btn_CopyBND
            // 
            this.btn_CopyBND.Location = new System.Drawing.Point(46, 173);
            this.btn_CopyBND.Name = "btn_CopyBND";
            this.btn_CopyBND.Size = new System.Drawing.Size(99, 29);
            this.btn_CopyBND.TabIndex = 33;
            this.btn_CopyBND.Text = "Copy ";
            this.btn_CopyBND.UseVisualStyleBackColor = true;
            this.btn_CopyBND.Click += new System.EventHandler(this.btn_CopyBND_Click);
            // 
            // GTWindowsForm_PlaceFDP
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(400, 313);
            this.Controls.Add(this.tbFDP);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "GTWindowsForm_PlaceFDP";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Place FDP v2.5.2";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.GTWindowsForm_InitRoute_Load);
            this.Shown += new System.EventHandler(this.GTWindowsForm_InitRoute_Shown);
            this.Activated += new System.EventHandler(this.GTWindowsForm_InitRoute_Activated);
            this.tbFDP.ResumeLayout(false);
            this.tPlaceFDP.ResumeLayout(false);
            this.tPlaceFDP.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.tCopyFDP.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tbFDP;
        private System.Windows.Forms.TabPage tPlaceFDP;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button btn_Pick;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtParantCode;
        private System.Windows.Forms.Label lblParentDevice;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtParant;
        private System.Windows.Forms.Label lblSplitter1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cmbFeature;
        private System.Windows.Forms.Button btn_Close;
        private System.Windows.Forms.Label lblSplitter2;
        private System.Windows.Forms.Button btn_Generate;
        private System.Windows.Forms.ComboBox cmbSplitter2;
        private System.Windows.Forms.Button btn_Clear;
        private System.Windows.Forms.ComboBox cmbSplitter1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox cmbType;
        private System.Windows.Forms.TabPage tCopyFDP;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button btn_Clear1;
        private System.Windows.Forms.TextBox txtCopyFDPCode;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Button btn_PickCopy;
        private System.Windows.Forms.Button btn_CopyBND;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox txtCopyFDPType;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtCopyFDPFID;
        private System.Windows.Forms.Label label6;


    }
}