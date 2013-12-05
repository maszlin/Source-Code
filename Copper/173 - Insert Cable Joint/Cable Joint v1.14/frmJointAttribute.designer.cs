namespace NEPS.GTechnology.Cable_Joint
{
    partial class frmJointAttribute
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
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.btnOK = new System.Windows.Forms.Button();
            this.cmbJoint = new System.Windows.Forms.ComboBox();
            this.cmbClosure = new System.Windows.Forms.ComboBox();
            this.lblPlantUnit = new System.Windows.Forms.Label();
            this.lblCableCode = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 31);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Plant Unit";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(9, 78);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(57, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Joint Type";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(9, 104);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(70, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "Closure Type";
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(229, 128);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 7;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // cmbJoint
            // 
            this.cmbJoint.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbJoint.FormattingEnabled = true;
            this.cmbJoint.Items.AddRange(new object[] {
            "MULTIPLE 2B",
            "MULTIPLE 3B",
            "NUMBERED",
            "STRAIGHT",
            "T-JOINT",
            "UNDERGROUND DP",
            "VCJ"});
            this.cmbJoint.Location = new System.Drawing.Point(101, 77);
            this.cmbJoint.Name = "cmbJoint";
            this.cmbJoint.Size = new System.Drawing.Size(203, 21);
            this.cmbJoint.TabIndex = 8;
            this.cmbJoint.SelectedIndexChanged += new System.EventHandler(this.cmbJoint_SelectedIndexChanged);
            // 
            // cmbClosure
            // 
            this.cmbClosure.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbClosure.FormattingEnabled = true;
            this.cmbClosure.Items.AddRange(new object[] {
            "HEAT SHRINK TUBING",
            "LEAD",
            "MECHANICAL",
            "UNIVERSAL",
            "VAULT",
            "XSAGA"});
            this.cmbClosure.Location = new System.Drawing.Point(101, 101);
            this.cmbClosure.Name = "cmbClosure";
            this.cmbClosure.Size = new System.Drawing.Size(203, 21);
            this.cmbClosure.TabIndex = 9;
            this.cmbClosure.SelectedIndexChanged += new System.EventHandler(this.cmbJoint_SelectedIndexChanged);
            // 
            // lblPlantUnit
            // 
            this.lblPlantUnit.AutoSize = true;
            this.lblPlantUnit.Location = new System.Drawing.Point(9, 50);
            this.lblPlantUnit.Name = "lblPlantUnit";
            this.lblPlantUnit.Size = new System.Drawing.Size(23, 13);
            this.lblPlantUnit.TabIndex = 10;
            this.lblPlantUnit.Text = "><";
            // 
            // lblCableCode
            // 
            this.lblCableCode.AutoSize = true;
            this.lblCableCode.Location = new System.Drawing.Point(9, 9);
            this.lblCableCode.Name = "lblCableCode";
            this.lblCableCode.Size = new System.Drawing.Size(69, 13);
            this.lblCableCode.TabIndex = 11;
            this.lblCableCode.Text = "Cable Code :";
            // 
            // frmJointAttribute
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(316, 159);
            this.Controls.Add(this.lblCableCode);
            this.Controls.Add(this.lblPlantUnit);
            this.Controls.Add(this.cmbClosure);
            this.Controls.Add(this.cmbJoint);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "frmJointAttribute";
            this.Text = "Joint Attribute";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.frmJointAttribute_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.ComboBox cmbJoint;
        private System.Windows.Forms.ComboBox cmbClosure;
        private System.Windows.Forms.Label lblPlantUnit;
        private System.Windows.Forms.Label lblCableCode;

    }
}