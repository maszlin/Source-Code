namespace AG.GTechnology.InsertFiberSplitter
{
    partial class GTWindowsForm_InsertSplitter
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
            this.txt_NoSplitter = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btn_Generate = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.cmbSplitterType = new System.Windows.Forms.ComboBox();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.comboBox2 = new System.Windows.Forms.ComboBox();
            this.gbServiceSp = new System.Windows.Forms.GroupBox();
            this.gbFOMSSp = new System.Windows.Forms.GroupBox();
            this.cmbSplitterTypeFOMS = new System.Windows.Forms.ComboBox();
            this.txt_NoSplitterFOMS = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.gbOwner = new System.Windows.Forms.GroupBox();
            this.txtOLTID = new System.Windows.Forms.TextBox();
            this.lbOLTID = new System.Windows.Forms.Label();
            this.btnSelectOwner = new System.Windows.Forms.Button();
            this.txtOwnerCode = new System.Windows.Forms.TextBox();
            this.txtOwnerFID = new System.Windows.Forms.TextBox();
            this.lbOwnerCode = new System.Windows.Forms.Label();
            this.lbOwnerFID = new System.Windows.Forms.Label();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.gbServiceSp.SuspendLayout();
            this.gbFOMSSp.SuspendLayout();
            this.gbOwner.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.SuspendLayout();
            // 
            // txt_NoSplitter
            // 
            this.txt_NoSplitter.BackColor = System.Drawing.Color.White;
            this.txt_NoSplitter.ForeColor = System.Drawing.Color.Black;
            this.txt_NoSplitter.Location = new System.Drawing.Point(114, 43);
            this.txt_NoSplitter.MaxLength = 2;
            this.txt_NoSplitter.Name = "txt_NoSplitter";
            this.txt_NoSplitter.Size = new System.Drawing.Size(137, 20);
            this.txt_NoSplitter.TabIndex = 20;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(14, 50);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(74, 13);
            this.label1.TabIndex = 19;
            this.label1.Text = "No of Splitter :";
            // 
            // btn_Generate
            // 
            this.btn_Generate.Location = new System.Drawing.Point(6, 189);
            this.btn_Generate.Name = "btn_Generate";
            this.btn_Generate.Size = new System.Drawing.Size(115, 28);
            this.btn_Generate.TabIndex = 4;
            this.btn_Generate.Text = "Add Splitter(s)";
            this.btn_Generate.UseVisualStyleBackColor = true;
            this.btn_Generate.Click += new System.EventHandler(this.btn_Generate_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(14, 23);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(72, 13);
            this.label4.TabIndex = 17;
            this.label4.Text = "Splitter Type :";
            // 
            // cmbSplitterType
            // 
            this.cmbSplitterType.BackColor = System.Drawing.Color.White;
            this.cmbSplitterType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbSplitterType.ForeColor = System.Drawing.Color.Black;
            this.cmbSplitterType.FormattingEnabled = true;
            this.cmbSplitterType.Location = new System.Drawing.Point(114, 20);
            this.cmbSplitterType.Name = "cmbSplitterType";
            this.cmbSplitterType.Size = new System.Drawing.Size(137, 21);
            this.cmbSplitterType.TabIndex = 18;
            // 
            // comboBox1
            // 
            this.comboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Location = new System.Drawing.Point(118, 76);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(137, 21);
            this.comboBox1.TabIndex = 22;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(18, 80);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(48, 13);
            this.label3.TabIndex = 21;
            this.label3.Text = "OLT ID :";
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(118, 48);
            this.textBox1.MaxLength = 2;
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(137, 20);
            this.textBox1.TabIndex = 20;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(18, 50);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(74, 13);
            this.label5.TabIndex = 19;
            this.label5.Text = "No of Splitter :";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(194, 121);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(78, 28);
            this.button1.TabIndex = 2;
            this.button1.Text = "Close";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(10, 121);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(78, 28);
            this.button2.TabIndex = 4;
            this.button2.Text = "Add Splitter(s)";
            this.button2.UseVisualStyleBackColor = true;
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(102, 121);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(78, 28);
            this.button3.TabIndex = 3;
            this.button3.Text = "Clear";
            this.button3.UseVisualStyleBackColor = true;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(18, 20);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(72, 13);
            this.label6.TabIndex = 17;
            this.label6.Text = "Splitter Type :";
            // 
            // comboBox2
            // 
            this.comboBox2.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox2.FormattingEnabled = true;
            this.comboBox2.Location = new System.Drawing.Point(118, 17);
            this.comboBox2.Name = "comboBox2";
            this.comboBox2.Size = new System.Drawing.Size(137, 21);
            this.comboBox2.TabIndex = 18;
            // 
            // gbServiceSp
            // 
            this.gbServiceSp.Controls.Add(this.cmbSplitterType);
            this.gbServiceSp.Controls.Add(this.label4);
            this.gbServiceSp.Controls.Add(this.txt_NoSplitter);
            this.gbServiceSp.Controls.Add(this.label1);
            this.gbServiceSp.Location = new System.Drawing.Point(6, 19);
            this.gbServiceSp.Name = "gbServiceSp";
            this.gbServiceSp.Size = new System.Drawing.Size(261, 79);
            this.gbServiceSp.TabIndex = 2;
            this.gbServiceSp.TabStop = false;
            this.gbServiceSp.Text = "Service Splitters";
            // 
            // gbFOMSSp
            // 
            this.gbFOMSSp.Controls.Add(this.cmbSplitterTypeFOMS);
            this.gbFOMSSp.Controls.Add(this.txt_NoSplitterFOMS);
            this.gbFOMSSp.Controls.Add(this.label7);
            this.gbFOMSSp.Controls.Add(this.label8);
            this.gbFOMSSp.Location = new System.Drawing.Point(6, 104);
            this.gbFOMSSp.Name = "gbFOMSSp";
            this.gbFOMSSp.Size = new System.Drawing.Size(261, 79);
            this.gbFOMSSp.TabIndex = 3;
            this.gbFOMSSp.TabStop = false;
            this.gbFOMSSp.Text = "FOMS Splitters";
            // 
            // cmbSplitterTypeFOMS
            // 
            this.cmbSplitterTypeFOMS.BackColor = System.Drawing.Color.White;
            this.cmbSplitterTypeFOMS.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbSplitterTypeFOMS.ForeColor = System.Drawing.Color.Black;
            this.cmbSplitterTypeFOMS.FormattingEnabled = true;
            this.cmbSplitterTypeFOMS.Location = new System.Drawing.Point(114, 22);
            this.cmbSplitterTypeFOMS.Name = "cmbSplitterTypeFOMS";
            this.cmbSplitterTypeFOMS.Size = new System.Drawing.Size(137, 21);
            this.cmbSplitterTypeFOMS.TabIndex = 22;
            // 
            // txt_NoSplitterFOMS
            // 
            this.txt_NoSplitterFOMS.BackColor = System.Drawing.Color.White;
            this.txt_NoSplitterFOMS.ForeColor = System.Drawing.Color.Black;
            this.txt_NoSplitterFOMS.Location = new System.Drawing.Point(114, 45);
            this.txt_NoSplitterFOMS.MaxLength = 2;
            this.txt_NoSplitterFOMS.Name = "txt_NoSplitterFOMS";
            this.txt_NoSplitterFOMS.Size = new System.Drawing.Size(137, 20);
            this.txt_NoSplitterFOMS.TabIndex = 24;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(14, 25);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(72, 13);
            this.label7.TabIndex = 21;
            this.label7.Text = "Splitter Type :";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(14, 52);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(74, 13);
            this.label8.TabIndex = 23;
            this.label8.Text = "No of Splitter :";
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(126, 194);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(141, 14);
            this.progressBar1.TabIndex = 5;
            this.progressBar1.Visible = false;
            // 
            // gbOwner
            // 
            this.gbOwner.Controls.Add(this.txtOLTID);
            this.gbOwner.Controls.Add(this.lbOLTID);
            this.gbOwner.Controls.Add(this.btnSelectOwner);
            this.gbOwner.Controls.Add(this.txtOwnerCode);
            this.gbOwner.Controls.Add(this.txtOwnerFID);
            this.gbOwner.Controls.Add(this.lbOwnerCode);
            this.gbOwner.Controls.Add(this.lbOwnerFID);
            this.gbOwner.Location = new System.Drawing.Point(12, 12);
            this.gbOwner.Name = "gbOwner";
            this.gbOwner.Size = new System.Drawing.Size(276, 110);
            this.gbOwner.TabIndex = 6;
            this.gbOwner.TabStop = false;
            this.gbOwner.Text = "SELECTED OWNER (FDC/FDP/DB)";
            // 
            // txtOLTID
            // 
            this.txtOLTID.Location = new System.Drawing.Point(167, 73);
            this.txtOLTID.Name = "txtOLTID";
            this.txtOLTID.ReadOnly = true;
            this.txtOLTID.Size = new System.Drawing.Size(90, 20);
            this.txtOLTID.TabIndex = 6;
            this.txtOLTID.Visible = false;
            // 
            // lbOLTID
            // 
            this.lbOLTID.AutoSize = true;
            this.lbOLTID.Location = new System.Drawing.Point(117, 76);
            this.lbOLTID.Name = "lbOLTID";
            this.lbOLTID.Size = new System.Drawing.Size(45, 13);
            this.lbOLTID.TabIndex = 5;
            this.lbOLTID.Text = "OLT ID:";
            this.lbOLTID.Visible = false;
            // 
            // btnSelectOwner
            // 
            this.btnSelectOwner.Location = new System.Drawing.Point(19, 75);
            this.btnSelectOwner.Name = "btnSelectOwner";
            this.btnSelectOwner.Size = new System.Drawing.Size(75, 23);
            this.btnSelectOwner.TabIndex = 4;
            this.btnSelectOwner.Text = "Select";
            this.btnSelectOwner.UseVisualStyleBackColor = true;
            this.btnSelectOwner.Click += new System.EventHandler(this.btnSelectOwner_Click);
            // 
            // txtOwnerCode
            // 
            this.txtOwnerCode.BackColor = System.Drawing.Color.White;
            this.txtOwnerCode.ForeColor = System.Drawing.Color.Black;
            this.txtOwnerCode.Location = new System.Drawing.Point(120, 44);
            this.txtOwnerCode.Name = "txtOwnerCode";
            this.txtOwnerCode.Size = new System.Drawing.Size(137, 20);
            this.txtOwnerCode.TabIndex = 3;
            // 
            // txtOwnerFID
            // 
            this.txtOwnerFID.BackColor = System.Drawing.Color.White;
            this.txtOwnerFID.ForeColor = System.Drawing.Color.Black;
            this.txtOwnerFID.Location = new System.Drawing.Point(120, 22);
            this.txtOwnerFID.Name = "txtOwnerFID";
            this.txtOwnerFID.Size = new System.Drawing.Size(137, 20);
            this.txtOwnerFID.TabIndex = 2;
            // 
            // lbOwnerCode
            // 
            this.lbOwnerCode.AutoSize = true;
            this.lbOwnerCode.Location = new System.Drawing.Point(18, 51);
            this.lbOwnerCode.Name = "lbOwnerCode";
            this.lbOwnerCode.Size = new System.Drawing.Size(66, 13);
            this.lbOwnerCode.TabIndex = 1;
            this.lbOwnerCode.Text = "Owner Code";
            // 
            // lbOwnerFID
            // 
            this.lbOwnerFID.AutoSize = true;
            this.lbOwnerFID.Location = new System.Drawing.Point(18, 25);
            this.lbOwnerFID.Name = "lbOwnerFID";
            this.lbOwnerFID.Size = new System.Drawing.Size(58, 13);
            this.lbOwnerFID.TabIndex = 0;
            this.lbOwnerFID.Text = "Owner FID";
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.gbServiceSp);
            this.groupBox4.Controls.Add(this.gbFOMSSp);
            this.groupBox4.Controls.Add(this.progressBar1);
            this.groupBox4.Controls.Add(this.btn_Generate);
            this.groupBox4.Location = new System.Drawing.Point(12, 128);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(276, 225);
            this.groupBox4.TabIndex = 7;
            this.groupBox4.TabStop = false;
            // 
            // GTWindowsForm_InsertSplitter
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(299, 366);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.gbOwner);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "GTWindowsForm_InsertSplitter";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Add Splitter(s) v1.0";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.GTWindowsForm_PlaceFDC_Load);
            this.Shown += new System.EventHandler(this.GTWindowsForm_PlaceFDC_Shown);
            this.gbServiceSp.ResumeLayout(false);
            this.gbServiceSp.PerformLayout();
            this.gbFOMSSp.ResumeLayout(false);
            this.gbFOMSSp.PerformLayout();
            this.gbOwner.ResumeLayout(false);
            this.gbOwner.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btn_Generate;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox cmbSplitterType;
        private System.Windows.Forms.TextBox txt_NoSplitter;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ComboBox comboBox2;
        private System.Windows.Forms.GroupBox gbServiceSp;
        private System.Windows.Forms.GroupBox gbFOMSSp;
        private System.Windows.Forms.ComboBox cmbSplitterTypeFOMS;
        private System.Windows.Forms.TextBox txt_NoSplitterFOMS;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.GroupBox gbOwner;
        private System.Windows.Forms.TextBox txtOwnerFID;
        private System.Windows.Forms.Label lbOwnerCode;
        private System.Windows.Forms.Label lbOwnerFID;
        private System.Windows.Forms.TextBox txtOwnerCode;
        private System.Windows.Forms.Button btnSelectOwner;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Label lbOLTID;
        private System.Windows.Forms.TextBox txtOLTID;


    }
}