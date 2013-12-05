namespace NEPS.GTechnology.NEPSDuctPathExpansion
{
    partial class DPExpansionForm
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.gbDuctPathAttrValues = new System.Windows.Forms.GroupBox();
            this.txtPPNumDuctWays = new System.Windows.Forms.TextBox();
            this.txtDuctPathFID = new System.Windows.Forms.TextBox();
            this.txtNumDuctWays = new System.Windows.Forms.TextBox();
            this.btnExpand = new System.Windows.Forms.Button();
            this.btnClose2 = new System.Windows.Forms.Button();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.groupBox1.SuspendLayout();
            this.gbDuctPathAttrValues.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.label9);
            this.groupBox1.Location = new System.Drawing.Point(12, 9);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(211, 109);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(8, 81);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(169, 13);
            this.label2.TabIndex = 14;
            this.label2.Text = "Expansion OF DUCTS (Ductways)";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(8, 27);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(89, 13);
            this.label1.TabIndex = 13;
            this.label1.Text = "DUCT PATH FID";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(8, 55);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(167, 13);
            this.label9.TabIndex = 0;
            this.label9.Text = "NUMBER OF DUCTS (Ductways)";
            // 
            // gbDuctPathAttrValues
            // 
            this.gbDuctPathAttrValues.Controls.Add(this.txtPPNumDuctWays);
            this.gbDuctPathAttrValues.Controls.Add(this.txtDuctPathFID);
            this.gbDuctPathAttrValues.Controls.Add(this.txtNumDuctWays);
            this.gbDuctPathAttrValues.Location = new System.Drawing.Point(228, 10);
            this.gbDuctPathAttrValues.Name = "gbDuctPathAttrValues";
            this.gbDuctPathAttrValues.Size = new System.Drawing.Size(139, 108);
            this.gbDuctPathAttrValues.TabIndex = 2;
            this.gbDuctPathAttrValues.TabStop = false;
            // 
            // txtPPNumDuctWays
            // 
            this.txtPPNumDuctWays.Enabled = false;
            this.txtPPNumDuctWays.Location = new System.Drawing.Point(6, 73);
            this.txtPPNumDuctWays.Name = "txtPPNumDuctWays";
            this.txtPPNumDuctWays.Size = new System.Drawing.Size(120, 20);
            this.txtPPNumDuctWays.TabIndex = 16;
            // 
            // txtDuctPathFID
            // 
            this.txtDuctPathFID.Location = new System.Drawing.Point(6, 19);
            this.txtDuctPathFID.Name = "txtDuctPathFID";
            this.txtDuctPathFID.ReadOnly = true;
            this.txtDuctPathFID.Size = new System.Drawing.Size(120, 20);
            this.txtDuctPathFID.TabIndex = 15;
            // 
            // txtNumDuctWays
            // 
            this.txtNumDuctWays.Enabled = false;
            this.txtNumDuctWays.Location = new System.Drawing.Point(6, 47);
            this.txtNumDuctWays.Name = "txtNumDuctWays";
            this.txtNumDuctWays.Size = new System.Drawing.Size(120, 20);
            this.txtNumDuctWays.TabIndex = 0;
            // 
            // btnExpand
            // 
            this.btnExpand.Location = new System.Drawing.Point(12, 124);
            this.btnExpand.Name = "btnExpand";
            this.btnExpand.Size = new System.Drawing.Size(117, 23);
            this.btnExpand.TabIndex = 65;
            this.btnExpand.Text = "Expand";
            this.btnExpand.UseVisualStyleBackColor = true;
            this.btnExpand.Click += new System.EventHandler(this.btnExpand_Click);
            // 
            // btnClose2
            // 
            this.btnClose2.Location = new System.Drawing.Point(135, 124);
            this.btnClose2.Name = "btnClose2";
            this.btnClose2.Size = new System.Drawing.Size(88, 23);
            this.btnClose2.TabIndex = 66;
            this.btnClose2.Text = "Close";
            this.btnClose2.UseVisualStyleBackColor = true;
            this.btnClose2.Click += new System.EventHandler(this.btnClose2_Click);
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(228, 127);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(139, 17);
            this.progressBar1.TabIndex = 67;
            this.progressBar1.Visible = false;
            // 
            // DPExpansionForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(381, 162);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.btnClose2);
            this.Controls.Add(this.btnExpand);
            this.Controls.Add(this.gbDuctPathAttrValues);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "DPExpansionForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Duct Path Expansion v1.0";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.DPExpansionFrom_Load);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.DPExpansionFrom_FormClosing);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.gbDuctPathAttrValues.ResumeLayout(false);
            this.gbDuctPathAttrValues.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.GroupBox gbDuctPathAttrValues;
        private System.Windows.Forms.TextBox txtPPNumDuctWays;
        private System.Windows.Forms.TextBox txtDuctPathFID;
        private System.Windows.Forms.TextBox txtNumDuctWays;
        private System.Windows.Forms.Button btnExpand;
        private System.Windows.Forms.Button btnClose2;
        private System.Windows.Forms.ProgressBar progressBar1;
    }
}