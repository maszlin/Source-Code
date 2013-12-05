namespace NEPS.AssignPlantUnit
{
    partial class AssignmentForm
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
            this.cbMinMaterials = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnOK = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.txtOwnership = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.txtYearPlaced = new System.Windows.Forms.MaskedTextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.cbManufacturers = new System.Windows.Forms.ComboBox();
            this.cbContractors = new System.Windows.Forms.ComboBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.btn_Pick = new System.Windows.Forms.Button();
            this.lblMH_Id = new System.Windows.Forms.Label();
            this.txtManhlID = new System.Windows.Forms.TextBox();
            this.lbOwnerFID = new System.Windows.Forms.Label();
            this.txtOwnerFID = new System.Windows.Forms.TextBox();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // cbMinMaterials
            // 
            this.cbMinMaterials.BackColor = System.Drawing.Color.White;
            this.cbMinMaterials.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbMinMaterials.ForeColor = System.Drawing.Color.Black;
            this.cbMinMaterials.FormattingEnabled = true;
            this.cbMinMaterials.Location = new System.Drawing.Point(97, 78);
            this.cbMinMaterials.Name = "cbMinMaterials";
            this.cbMinMaterials.Size = new System.Drawing.Size(155, 21);
            this.cbMinMaterials.TabIndex = 3;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(21, 78);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Plant Unit";
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(24, 167);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 4;
            this.btnOK.Text = "&OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(21, 105);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(61, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "Ownership*";
            // 
            // txtOwnership
            // 
            this.txtOwnership.BackColor = System.Drawing.Color.White;
            this.txtOwnership.ForeColor = System.Drawing.Color.Black;
            this.txtOwnership.Location = new System.Drawing.Point(97, 105);
            this.txtOwnership.MaxLength = 30;
            this.txtOwnership.Name = "txtOwnership";
            this.txtOwnership.Size = new System.Drawing.Size(323, 20);
            this.txtOwnership.TabIndex = 6;
            this.txtOwnership.Text = "TM";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(21, 26);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(74, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "Manufacturer*";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(21, 52);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(60, 13);
            this.label4.TabIndex = 5;
            this.label4.Text = "Contractor*";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(21, 131);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(75, 13);
            this.label7.TabIndex = 5;
            this.label7.Text = "Year Installed*";
            // 
            // txtYearPlaced
            // 
            this.txtYearPlaced.BackColor = System.Drawing.Color.White;
            this.txtYearPlaced.ForeColor = System.Drawing.Color.Black;
            this.txtYearPlaced.Location = new System.Drawing.Point(97, 131);
            this.txtYearPlaced.Mask = "0000";
            this.txtYearPlaced.Name = "txtYearPlaced";
            this.txtYearPlaced.Size = new System.Drawing.Size(62, 20);
            this.txtYearPlaced.TabIndex = 7;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(186, 138);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(179, 13);
            this.label5.TabIndex = 5;
            this.label5.Text = "*Leave blank to use defaults values.";
            // 
            // cbManufacturers
            // 
            this.cbManufacturers.BackColor = System.Drawing.Color.White;
            this.cbManufacturers.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbManufacturers.ForeColor = System.Drawing.Color.Black;
            this.cbManufacturers.FormattingEnabled = true;
            this.cbManufacturers.Location = new System.Drawing.Point(97, 26);
            this.cbManufacturers.Name = "cbManufacturers";
            this.cbManufacturers.Size = new System.Drawing.Size(155, 21);
            this.cbManufacturers.TabIndex = 3;
            // 
            // cbContractors
            // 
            this.cbContractors.BackColor = System.Drawing.Color.White;
            this.cbContractors.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbContractors.ForeColor = System.Drawing.Color.Black;
            this.cbContractors.FormattingEnabled = true;
            this.cbContractors.Location = new System.Drawing.Point(97, 53);
            this.cbContractors.Name = "cbContractors";
            this.cbContractors.Size = new System.Drawing.Size(155, 21);
            this.cbContractors.TabIndex = 3;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.btn_Pick);
            this.groupBox2.Controls.Add(this.lblMH_Id);
            this.groupBox2.Controls.Add(this.txtManhlID);
            this.groupBox2.Controls.Add(this.lbOwnerFID);
            this.groupBox2.Controls.Add(this.txtOwnerFID);
            this.groupBox2.Location = new System.Drawing.Point(483, 18);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(405, 73);
            this.groupBox2.TabIndex = 30;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Owner (Pole/Manhole) :";
            // 
            // btn_Pick
            // 
            this.btn_Pick.Location = new System.Drawing.Point(238, 19);
            this.btn_Pick.Name = "btn_Pick";
            this.btn_Pick.Size = new System.Drawing.Size(75, 21);
            this.btn_Pick.TabIndex = 24;
            this.btn_Pick.Text = "Pick";
            this.btn_Pick.UseVisualStyleBackColor = true;
            this.btn_Pick.Click += new System.EventHandler(this.btn_Pick_Click);
            // 
            // lblMH_Id
            // 
            this.lblMH_Id.AutoSize = true;
            this.lblMH_Id.Location = new System.Drawing.Point(6, 48);
            this.lblMH_Id.Name = "lblMH_Id";
            this.lblMH_Id.Size = new System.Drawing.Size(62, 13);
            this.lblMH_Id.TabIndex = 27;
            this.lblMH_Id.Text = "Manhole ID";
            // 
            // txtManhlID
            // 
            this.txtManhlID.BackColor = System.Drawing.Color.White;
            this.txtManhlID.ForeColor = System.Drawing.Color.Black;
            this.txtManhlID.Location = new System.Drawing.Point(82, 45);
            this.txtManhlID.Name = "txtManhlID";
            this.txtManhlID.ReadOnly = true;
            this.txtManhlID.Size = new System.Drawing.Size(137, 20);
            this.txtManhlID.TabIndex = 28;
            // 
            // lbOwnerFID
            // 
            this.lbOwnerFID.AutoSize = true;
            this.lbOwnerFID.Location = new System.Drawing.Point(6, 22);
            this.lbOwnerFID.Name = "lbOwnerFID";
            this.lbOwnerFID.Size = new System.Drawing.Size(27, 13);
            this.lbOwnerFID.TabIndex = 25;
            this.lbOwnerFID.Text = " FID";
            // 
            // txtOwnerFID
            // 
            this.txtOwnerFID.BackColor = System.Drawing.Color.White;
            this.txtOwnerFID.ForeColor = System.Drawing.Color.Black;
            this.txtOwnerFID.Location = new System.Drawing.Point(82, 19);
            this.txtOwnerFID.Name = "txtOwnerFID";
            this.txtOwnerFID.ReadOnly = true;
            this.txtOwnerFID.Size = new System.Drawing.Size(137, 20);
            this.txtOwnerFID.TabIndex = 26;
            // 
            // AssignmentForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(428, 201);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.txtYearPlaced);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.txtOwnership);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.cbContractors);
            this.Controls.Add(this.cbManufacturers);
            this.Controls.Add(this.cbMinMaterials);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "AssignmentForm";
            this.ShowIcon = false;
            this.Text = "Splice Attributes v0.9.1";
            this.Load += new System.EventHandler(this.AssignmentForm_Load);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.AssignmentForm_FormClosing);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox cbMinMaterials;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtOwnership;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.MaskedTextBox txtYearPlaced;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox cbManufacturers;
        private System.Windows.Forms.ComboBox cbContractors;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button btn_Pick;
        private System.Windows.Forms.Label lblMH_Id;
        private System.Windows.Forms.TextBox txtManhlID;
        private System.Windows.Forms.Label lbOwnerFID;
        private System.Windows.Forms.TextBox txtOwnerFID;
    }
}