namespace AG.GTechnology.DuctSpaceReport.Forms
{
    partial class SubDuctForm
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SubDuctForm));
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.nbrDucts = new System.Windows.Forms.NumericUpDown();
            this.btnPlace = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.nbrDucts)).BeginInit();
            this.SuspendLayout();
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "mapping-right");
            this.imageList1.Images.SetKeyName(1, "mapping-left");
            this.imageList1.Images.SetKeyName(2, "mapping");
            // 
            // nbrDucts
            // 
            this.nbrDucts.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.nbrDucts.Location = new System.Drawing.Point(2, 2);
            this.nbrDucts.Maximum = new decimal(new int[] {
            3,
            0,
            0,
            0});
            this.nbrDucts.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nbrDucts.Name = "nbrDucts";
            this.nbrDucts.Size = new System.Drawing.Size(120, 20);
            this.nbrDucts.TabIndex = 0;
            this.nbrDucts.Value = new decimal(new int[] {
            3,
            0,
            0,
            0});
            this.nbrDucts.ValueChanged += new System.EventHandler(this.nbrDucts_ValueChanged);
            // 
            // btnPlace
            // 
            this.btnPlace.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnPlace.Location = new System.Drawing.Point(128, 1);
            this.btnPlace.Name = "btnPlace";
            this.btnPlace.Size = new System.Drawing.Size(71, 23);
            this.btnPlace.TabIndex = 1;
            this.btnPlace.Text = "Place";
            this.btnPlace.UseVisualStyleBackColor = true;
            // 
            // SubDuctForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(215, 24);
            this.Controls.Add(this.btnPlace);
            this.Controls.Add(this.nbrDucts);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SubDuctForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Enter number of subduct/innerduct";
            this.TopMost = true;
            ((System.ComponentModel.ISupportInitialize)(this.nbrDucts)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.NumericUpDown nbrDucts;
        private System.Windows.Forms.Button btnPlace;


    }
}