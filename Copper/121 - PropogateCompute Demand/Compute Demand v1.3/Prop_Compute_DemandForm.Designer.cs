namespace NEPS.GTechnology.Prop_Compute_Demand
{
    partial class GTWindowsForm_Demand
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
            this.FeatType = new System.Windows.Forms.Label();
            this.ServBndyFID = new System.Windows.Forms.Label();
            this.button4 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.txtServiceBndy = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.FeatType);
            this.groupBox1.Controls.Add(this.ServBndyFID);
            this.groupBox1.Controls.Add(this.button4);
            this.groupBox1.Controls.Add(this.button3);
            this.groupBox1.Controls.Add(this.button2);
            this.groupBox1.Controls.Add(this.txtServiceBndy);
            this.groupBox1.Controls.Add(this.button1);
            this.groupBox1.Location = new System.Drawing.Point(15, 10);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(422, 141);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            // 
            // FeatType
            // 
            this.FeatType.AutoSize = true;
            this.FeatType.Location = new System.Drawing.Point(250, 55);
            this.FeatType.Name = "FeatType";
            this.FeatType.Size = new System.Drawing.Size(35, 13);
            this.FeatType.TabIndex = 10;
            this.FeatType.Text = "label1";
            this.FeatType.Visible = false;
            // 
            // ServBndyFID
            // 
            this.ServBndyFID.AutoSize = true;
            this.ServBndyFID.Location = new System.Drawing.Point(187, 55);
            this.ServBndyFID.Name = "ServBndyFID";
            this.ServBndyFID.Size = new System.Drawing.Size(35, 13);
            this.ServBndyFID.TabIndex = 9;
            this.ServBndyFID.Text = "label1";
            this.ServBndyFID.Visible = false;
            // 
            // button4
            // 
            this.button4.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button4.Location = new System.Drawing.Point(64, 74);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(86, 45);
            this.button4.TabIndex = 8;
            this.button4.Text = "Close";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // button3
            // 
            this.button3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button3.Location = new System.Drawing.Point(250, 74);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(86, 45);
            this.button3.TabIndex = 7;
            this.button3.Text = "Compute Demand";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // button2
            // 
            this.button2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button2.Location = new System.Drawing.Point(158, 74);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(86, 45);
            this.button2.TabIndex = 6;
            this.button2.Text = "Propagate Demand";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click_1);
            // 
            // txtServiceBndy
            // 
            this.txtServiceBndy.BackColor = System.Drawing.Color.White;
            this.txtServiceBndy.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtServiceBndy.ForeColor = System.Drawing.Color.Black;
            this.txtServiceBndy.Location = new System.Drawing.Point(42, 28);
            this.txtServiceBndy.Name = "txtServiceBndy";
            this.txtServiceBndy.ReadOnly = true;
            this.txtServiceBndy.Size = new System.Drawing.Size(220, 21);
            this.txtServiceBndy.TabIndex = 5;
            // 
            // button1
            // 
            this.button1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button1.Location = new System.Drawing.Point(268, 27);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(112, 25);
            this.button1.TabIndex = 4;
            this.button1.Text = "Pick Boundry";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // GTWindowsForm_Demand
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(453, 170);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "GTWindowsForm_Demand";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Propagate & Compute  Demand v1.3";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.GTWindowsForm_InitRoute_Load);
            this.Shown += new System.EventHandler(this.GTWindowsForm_InitRoute_Shown);
            this.Activated += new System.EventHandler(this.GTWindowsForm_InitRoute_Activated);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.TextBox txtServiceBndy;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label ServBndyFID;
        private System.Windows.Forms.Label FeatType;





    }
}