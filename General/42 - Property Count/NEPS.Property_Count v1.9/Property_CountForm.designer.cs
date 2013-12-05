namespace NEPS.GTechnology.Property_Count
{
    partial class GTWindowsForm_Property_Count
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
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.btnPickSelected = new System.Windows.Forms.Button();
            this.lblDP_FNO = new System.Windows.Forms.Label();
            this.button2 = new System.Windows.Forms.Button();
            this.ServBndyFID = new System.Windows.Forms.Label();
            this.txtServiceBndy = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.button4 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.rbSingleBoundary = new System.Windows.Forms.RadioButton();
            this.rbAllBoundaries = new System.Windows.Forms.RadioButton();
            this.label2 = new System.Windows.Forms.Label();
            this.lblStatus = new System.Windows.Forms.Label();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.btnPickSelected);
            this.groupBox2.Controls.Add(this.lblDP_FNO);
            this.groupBox2.Controls.Add(this.button2);
            this.groupBox2.Controls.Add(this.ServBndyFID);
            this.groupBox2.Controls.Add(this.txtServiceBndy);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Location = new System.Drawing.Point(43, 56);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(348, 108);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            // 
            // btnPickSelected
            // 
            this.btnPickSelected.Location = new System.Drawing.Point(216, 69);
            this.btnPickSelected.Name = "btnPickSelected";
            this.btnPickSelected.Size = new System.Drawing.Size(107, 25);
            this.btnPickSelected.TabIndex = 40;
            this.btnPickSelected.Text = "&Pick Selected";
            this.btnPickSelected.UseVisualStyleBackColor = true;
            this.btnPickSelected.Click += new System.EventHandler(this.btnPickSelected_Click);
            // 
            // lblDP_FNO
            // 
            this.lblDP_FNO.AutoSize = true;
            this.lblDP_FNO.Location = new System.Drawing.Point(307, 16);
            this.lblDP_FNO.Name = "lblDP_FNO";
            this.lblDP_FNO.Size = new System.Drawing.Size(35, 13);
            this.lblDP_FNO.TabIndex = 6;
            this.lblDP_FNO.Text = "label2";
            this.lblDP_FNO.Visible = false;
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(521, 197);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(115, 41);
            this.button2.TabIndex = 4;
            this.button2.Text = "Place Property";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // ServBndyFID
            // 
            this.ServBndyFID.AutoSize = true;
            this.ServBndyFID.Location = new System.Drawing.Point(354, 40);
            this.ServBndyFID.Name = "ServBndyFID";
            this.ServBndyFID.Size = new System.Drawing.Size(13, 13);
            this.ServBndyFID.TabIndex = 3;
            this.ServBndyFID.Text = "L";
            this.ServBndyFID.Visible = false;
            // 
            // txtServiceBndy
            // 
            this.txtServiceBndy.BackColor = System.Drawing.SystemColors.Window;
            this.txtServiceBndy.Location = new System.Drawing.Point(23, 43);
            this.txtServiceBndy.Name = "txtServiceBndy";
            this.txtServiceBndy.ReadOnly = true;
            this.txtServiceBndy.Size = new System.Drawing.Size(300, 20);
            this.txtServiceBndy.TabIndex = 30;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(20, 24);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(216, 15);
            this.label1.TabIndex = 0;
            this.label1.Text = "Double-click on a boundary to select it:";
            // 
            // button4
            // 
            this.button4.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button4.Location = new System.Drawing.Point(316, 176);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(75, 36);
            this.button4.TabIndex = 60;
            this.button4.Text = "&Close";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // button3
            // 
            this.button3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button3.Location = new System.Drawing.Point(204, 176);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(106, 36);
            this.button3.TabIndex = 50;
            this.button3.Text = "&Update Property";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // rbSingleBoundary
            // 
            this.rbSingleBoundary.AutoSize = true;
            this.rbSingleBoundary.Checked = true;
            this.rbSingleBoundary.Location = new System.Drawing.Point(82, 24);
            this.rbSingleBoundary.Name = "rbSingleBoundary";
            this.rbSingleBoundary.Size = new System.Drawing.Size(102, 17);
            this.rbSingleBoundary.TabIndex = 10;
            this.rbSingleBoundary.TabStop = true;
            this.rbSingleBoundary.Text = "Single Boundary";
            this.rbSingleBoundary.UseVisualStyleBackColor = true;
            this.rbSingleBoundary.CheckedChanged += new System.EventHandler(this.rbSingleBoundary_CheckedChanged);
            // 
            // rbAllBoundaries
            // 
            this.rbAllBoundaries.AutoSize = true;
            this.rbAllBoundaries.Location = new System.Drawing.Point(190, 24);
            this.rbAllBoundaries.Name = "rbAllBoundaries";
            this.rbAllBoundaries.Size = new System.Drawing.Size(92, 17);
            this.rbAllBoundaries.TabIndex = 20;
            this.rbAllBoundaries.Text = "All Boundaries";
            this.rbAllBoundaries.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(22, 24);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(54, 15);
            this.label2.TabIndex = 0;
            this.label2.Text = "Process:";
            // 
            // lblStatus
            // 
            this.lblStatus.AutoSize = true;
            this.lblStatus.Location = new System.Drawing.Point(40, 176);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(0, 13);
            this.lblStatus.TabIndex = 61;
            // 
            // GTWindowsForm_Property_Count
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(444, 239);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.rbAllBoundaries);
            this.Controls.Add(this.rbSingleBoundary);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.label2);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "GTWindowsForm_Property_Count";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Property Count v1.9";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.GTWindowsForm_InitRoute_Load);
            this.Shown += new System.EventHandler(this.GTWindowsForm_InitRoute_Shown);
            this.Activated += new System.EventHandler(this.GTWindowsForm_InitRoute_Activated);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TextBox txtServiceBndy;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label ServBndyFID;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Label lblDP_FNO;
        private System.Windows.Forms.Button btnPickSelected;
        private System.Windows.Forms.RadioButton rbSingleBoundary;
        private System.Windows.Forms.RadioButton rbAllBoundaries;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label lblStatus;


    }
}