namespace NEPS.GTechnology.NEPSCopyPole
{
    partial class Distance
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
            this.gbMessage = new System.Windows.Forms.GroupBox();
            this.txtDistance = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.rbNonPrec = new System.Windows.Forms.RadioButton();
            this.rbPrec = new System.Windows.Forms.RadioButton();
            this.lb1 = new System.Windows.Forms.Label();
            this.lb2 = new System.Windows.Forms.Label();
            this.gbHelp = new System.Windows.Forms.GroupBox();
            this.gbMessage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtDistance)).BeginInit();
            this.gbHelp.SuspendLayout();
            this.SuspendLayout();
            // 
            // gbMessage
            // 
            this.gbMessage.Controls.Add(this.txtDistance);
            this.gbMessage.Controls.Add(this.label1);
            this.gbMessage.Controls.Add(this.rbNonPrec);
            this.gbMessage.Controls.Add(this.rbPrec);
            this.gbMessage.Location = new System.Drawing.Point(6, 6);
            this.gbMessage.Name = "gbMessage";
            this.gbMessage.Size = new System.Drawing.Size(259, 70);
            this.gbMessage.TabIndex = 5;
            this.gbMessage.TabStop = false;
            // 
            // txtDistance
            // 
            this.txtDistance.DecimalPlaces = 2;
            this.txtDistance.Enabled = false;
            this.txtDistance.Location = new System.Drawing.Point(130, 12);
            this.txtDistance.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.txtDistance.Name = "txtDistance";
            this.txtDistance.Size = new System.Drawing.Size(120, 20);
            this.txtDistance.TabIndex = 5;
            this.txtDistance.Value = new decimal(new int[] {
            20,
            0,
            0,
            0});
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 19);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(118, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Distance from previous:";
            // 
            // rbNonPrec
            // 
            this.rbNonPrec.AutoSize = true;
            this.rbNonPrec.Checked = true;
            this.rbNonPrec.Location = new System.Drawing.Point(130, 44);
            this.rbNonPrec.Name = "rbNonPrec";
            this.rbNonPrec.Size = new System.Drawing.Size(90, 17);
            this.rbNonPrec.TabIndex = 2;
            this.rbNonPrec.TabStop = true;
            this.rbNonPrec.Text = "Non precision";
            this.rbNonPrec.UseVisualStyleBackColor = true;
            this.rbNonPrec.CheckedChanged += new System.EventHandler(this.rbNonPrec_CheckedChanged);
            // 
            // rbPrec
            // 
            this.rbPrec.AutoSize = true;
            this.rbPrec.Location = new System.Drawing.Point(11, 44);
            this.rbPrec.Name = "rbPrec";
            this.rbPrec.Size = new System.Drawing.Size(68, 17);
            this.rbPrec.TabIndex = 2;
            this.rbPrec.Text = "Precision";
            this.rbPrec.UseVisualStyleBackColor = true;
            this.rbPrec.CheckedChanged += new System.EventHandler(this.rbPrec_CheckedChanged);
            // 
            // lb1
            // 
            this.lb1.AutoSize = true;
            this.lb1.Location = new System.Drawing.Point(8, 12);
            this.lb1.Name = "lb1";
            this.lb1.Size = new System.Drawing.Size(123, 13);
            this.lb1.TabIndex = 6;
            this.lb1.Text = "Pnt> Select Source Pole";
            // 
            // lb2
            // 
            this.lb2.AutoSize = true;
            this.lb2.Location = new System.Drawing.Point(8, 31);
            this.lb2.Name = "lb2";
            this.lb2.Size = new System.Drawing.Size(49, 13);
            this.lb2.TabIndex = 7;
            this.lb2.Text = "Rst> Exit";
            // 
            // gbHelp
            // 
            this.gbHelp.Controls.Add(this.lb1);
            this.gbHelp.Controls.Add(this.lb2);
            this.gbHelp.Location = new System.Drawing.Point(6, 80);
            this.gbHelp.Name = "gbHelp";
            this.gbHelp.Size = new System.Drawing.Size(259, 51);
            this.gbHelp.TabIndex = 8;
            this.gbHelp.TabStop = false;
            // 
            // Distance
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(275, 139);
            this.Controls.Add(this.gbHelp);
            this.Controls.Add(this.gbMessage);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.KeyPreview = true;
            this.Name = "Distance";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Copy Pole";
            this.TopMost = true;
            this.gbMessage.ResumeLayout(false);
            this.gbMessage.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtDistance)).EndInit();
            this.gbHelp.ResumeLayout(false);
            this.gbHelp.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label1;
        public System.Windows.Forms.NumericUpDown txtDistance;
        public System.Windows.Forms.RadioButton rbNonPrec;
        public System.Windows.Forms.RadioButton rbPrec;
        private System.Windows.Forms.Label lb1;
        private System.Windows.Forms.Label lb2;
        private System.Windows.Forms.GroupBox gbHelp;
        private System.Windows.Forms.GroupBox gbMessage;
    }
}