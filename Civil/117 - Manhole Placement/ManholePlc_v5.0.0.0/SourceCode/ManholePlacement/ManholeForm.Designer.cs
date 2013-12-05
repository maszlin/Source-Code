namespace AG.GTechnology.ManholePlacement.Forms
{
    partial class ManholeForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ManholeForm));
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.btnPlace = new System.Windows.Forms.Button();
            this.radioPlaceType1 = new System.Windows.Forms.RadioButton();
            this.radioPlaceType2 = new System.Windows.Forms.RadioButton();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.txtDistance = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtDistance)).BeginInit();
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
            // btnPlace
            // 
            this.btnPlace.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnPlace.Location = new System.Drawing.Point(111, 77);
            this.btnPlace.Name = "btnPlace";
            this.btnPlace.Size = new System.Drawing.Size(130, 23);
            this.btnPlace.TabIndex = 1;
            this.btnPlace.Text = "Placement";
            this.btnPlace.UseVisualStyleBackColor = true;
            // 
            // radioPlaceType1
            // 
            this.radioPlaceType1.AutoSize = true;
            this.radioPlaceType1.Location = new System.Drawing.Point(11, 44);
            this.radioPlaceType1.Name = "radioPlaceType1";
            this.radioPlaceType1.Size = new System.Drawing.Size(68, 17);
            this.radioPlaceType1.TabIndex = 2;
            this.radioPlaceType1.Text = "Precision";
            this.radioPlaceType1.UseVisualStyleBackColor = true;
            this.radioPlaceType1.CheckedChanged += new System.EventHandler(this.radioPlaceType1_CheckedChanged);
            // 
            // radioPlaceType2
            // 
            this.radioPlaceType2.AutoSize = true;
            this.radioPlaceType2.Checked = true;
            this.radioPlaceType2.Location = new System.Drawing.Point(115, 44);
            this.radioPlaceType2.Name = "radioPlaceType2";
            this.radioPlaceType2.Size = new System.Drawing.Size(90, 17);
            this.radioPlaceType2.TabIndex = 2;
            this.radioPlaceType2.TabStop = true;
            this.radioPlaceType2.Text = "Non precision";
            this.radioPlaceType2.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.txtDistance);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.radioPlaceType2);
            this.groupBox1.Controls.Add(this.radioPlaceType1);
            this.groupBox1.Location = new System.Drawing.Point(1, 1);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(240, 70);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            // 
            // txtDistance
            // 
            this.txtDistance.DecimalPlaces = 2;
            this.txtDistance.Location = new System.Drawing.Point(111, 12);
            this.txtDistance.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.txtDistance.Name = "txtDistance";
            this.txtDistance.Size = new System.Drawing.Size(120, 20);
            this.txtDistance.TabIndex = 5;
            this.txtDistance.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 19);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(107, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Distance to previous:";
            // 
            // ManholeForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(253, 104);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btnPlace);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ManholeForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Manhole Placement v5.0";
            this.TopMost = true;
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtDistance)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.Button btnPlace;
        private System.Windows.Forms.RadioButton radioPlaceType1;
        private System.Windows.Forms.RadioButton radioPlaceType2;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown txtDistance;


    }
}