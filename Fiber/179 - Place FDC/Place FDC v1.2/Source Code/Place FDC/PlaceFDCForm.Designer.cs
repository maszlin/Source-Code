namespace NEPS.GTechnology.PlaceFDC
{
    partial class GTWindowsForm_PlaceFDC
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
            this.tPlaceFDC = new System.Windows.Forms.TabPage();
            this.cmbOLTID = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txt_NoSplitter = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btn_Close = new System.Windows.Forms.Button();
            this.btn_Generate = new System.Windows.Forms.Button();
            this.btn_Clear = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.cmbSplitterType = new System.Windows.Forms.ComboBox();
            this.tbFDP.SuspendLayout();
            this.tPlaceFDC.SuspendLayout();
            this.SuspendLayout();
            // 
            // tbFDP
            // 
            this.tbFDP.Controls.Add(this.tPlaceFDC);
            this.tbFDP.Location = new System.Drawing.Point(12, 12);
            this.tbFDP.Name = "tbFDP";
            this.tbFDP.SelectedIndex = 0;
            this.tbFDP.Size = new System.Drawing.Size(295, 188);
            this.tbFDP.TabIndex = 1;
            // 
            // tPlaceFDC
            // 
            this.tPlaceFDC.Controls.Add(this.cmbOLTID);
            this.tPlaceFDC.Controls.Add(this.label2);
            this.tPlaceFDC.Controls.Add(this.txt_NoSplitter);
            this.tPlaceFDC.Controls.Add(this.label1);
            this.tPlaceFDC.Controls.Add(this.btn_Close);
            this.tPlaceFDC.Controls.Add(this.btn_Generate);
            this.tPlaceFDC.Controls.Add(this.btn_Clear);
            this.tPlaceFDC.Controls.Add(this.label4);
            this.tPlaceFDC.Controls.Add(this.cmbSplitterType);
            this.tPlaceFDC.Location = new System.Drawing.Point(4, 22);
            this.tPlaceFDC.Name = "tPlaceFDC";
            this.tPlaceFDC.Padding = new System.Windows.Forms.Padding(3);
            this.tPlaceFDC.Size = new System.Drawing.Size(287, 162);
            this.tPlaceFDC.TabIndex = 0;
            this.tPlaceFDC.Text = "Place FDC";
            this.tPlaceFDC.UseVisualStyleBackColor = true;
            // 
            // cmbOLTID
            // 
            this.cmbOLTID.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbOLTID.FormattingEnabled = true;
            this.cmbOLTID.Location = new System.Drawing.Point(118, 76);
            this.cmbOLTID.Name = "cmbOLTID";
            this.cmbOLTID.Size = new System.Drawing.Size(137, 21);
            this.cmbOLTID.TabIndex = 22;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(18, 80);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(48, 13);
            this.label2.TabIndex = 21;
            this.label2.Text = "OLT ID :";
            // 
            // txt_NoSplitter
            // 
            this.txt_NoSplitter.Location = new System.Drawing.Point(118, 48);
            this.txt_NoSplitter.MaxLength = 2;
            this.txt_NoSplitter.Name = "txt_NoSplitter";
            this.txt_NoSplitter.Size = new System.Drawing.Size(137, 20);
            this.txt_NoSplitter.TabIndex = 20;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(18, 50);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(74, 13);
            this.label1.TabIndex = 19;
            this.label1.Text = "No of Splitter :";
            // 
            // btn_Close
            // 
            this.btn_Close.Location = new System.Drawing.Point(194, 121);
            this.btn_Close.Name = "btn_Close";
            this.btn_Close.Size = new System.Drawing.Size(78, 28);
            this.btn_Close.TabIndex = 2;
            this.btn_Close.Text = "Close";
            this.btn_Close.UseVisualStyleBackColor = true;
            this.btn_Close.Click += new System.EventHandler(this.btn_Close_Click);
            // 
            // btn_Generate
            // 
            this.btn_Generate.Location = new System.Drawing.Point(10, 121);
            this.btn_Generate.Name = "btn_Generate";
            this.btn_Generate.Size = new System.Drawing.Size(78, 28);
            this.btn_Generate.TabIndex = 4;
            this.btn_Generate.Text = "Place FDC";
            this.btn_Generate.UseVisualStyleBackColor = true;
            this.btn_Generate.Click += new System.EventHandler(this.btn_Generate_Click);
            // 
            // btn_Clear
            // 
            this.btn_Clear.Location = new System.Drawing.Point(102, 121);
            this.btn_Clear.Name = "btn_Clear";
            this.btn_Clear.Size = new System.Drawing.Size(78, 28);
            this.btn_Clear.TabIndex = 3;
            this.btn_Clear.Text = "Clear";
            this.btn_Clear.UseVisualStyleBackColor = true;
            this.btn_Clear.Click += new System.EventHandler(this.btn_Clear_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(18, 20);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(72, 13);
            this.label4.TabIndex = 17;
            this.label4.Text = "Splitter Type :";
            // 
            // cmbSplitterType
            // 
            this.cmbSplitterType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbSplitterType.FormattingEnabled = true;
            this.cmbSplitterType.Location = new System.Drawing.Point(118, 17);
            this.cmbSplitterType.Name = "cmbSplitterType";
            this.cmbSplitterType.Size = new System.Drawing.Size(137, 21);
            this.cmbSplitterType.TabIndex = 18;
            // 
            // GTWindowsForm_PlaceFDC
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(316, 212);
            this.Controls.Add(this.tbFDP);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "GTWindowsForm_PlaceFDC";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Place FDC v1.2";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.GTWindowsForm_PlaceFDC_Load);
            this.Shown += new System.EventHandler(this.GTWindowsForm_PlaceFDC_Shown);
            this.tbFDP.ResumeLayout(false);
            this.tPlaceFDC.ResumeLayout(false);
            this.tPlaceFDC.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tbFDP;
        private System.Windows.Forms.TabPage tPlaceFDC;
        private System.Windows.Forms.Button btn_Close;
        private System.Windows.Forms.Button btn_Generate;
        private System.Windows.Forms.Button btn_Clear;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox cmbSplitterType;
        private System.Windows.Forms.TextBox txt_NoSplitter;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cmbOLTID;
        private System.Windows.Forms.Label label2;


    }
}