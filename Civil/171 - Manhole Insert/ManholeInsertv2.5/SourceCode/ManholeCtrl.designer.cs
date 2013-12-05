namespace AG.GTechnology.ManholeInsert
{
    partial class ManholeCtrl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.btnDuct = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnDuct
            // 
            this.btnDuct.BackColor = System.Drawing.Color.ForestGreen;
            this.btnDuct.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnDuct.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.btnDuct.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDuct.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnDuct.ForeColor = System.Drawing.SystemColors.ControlText;
            this.btnDuct.Location = new System.Drawing.Point(0, 0);
            this.btnDuct.Name = "btnDuct";
            this.btnDuct.Size = new System.Drawing.Size(36, 36);
            this.btnDuct.TabIndex = 0;
            this.btnDuct.Text = "A1";
            this.btnDuct.UseVisualStyleBackColor = false;
            // 
            // DuctCtrl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.btnDuct);
            this.Name = "DuctCtrl";
            this.Size = new System.Drawing.Size(36, 36);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnDuct;
    }
}
