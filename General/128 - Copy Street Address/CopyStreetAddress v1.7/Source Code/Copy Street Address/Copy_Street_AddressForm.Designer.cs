namespace NEPS.GTechnology.CopyStreetAddress
{
    partial class CopyStreetAddress
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
            this.button1 = new System.Windows.Forms.Button();
            this.dvFeat = new System.Windows.Forms.DataGridView();
            this.G3E_FNO = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Feat_Name = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.G3E_FID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Code = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.StreetName1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.StreetType1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.PostalNumber = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.SectionName1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.CityName1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.StateCode1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.STREET_NAME = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.STREET_TYPE = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.POSTAL_NUM = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.SECTION_NAME = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.CITY_NAME = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.STATE_CODE = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label11 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.button4 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.txtStreet = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.label14 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dvFeat)).BeginInit();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.button1);
            this.groupBox1.Controls.Add(this.dvFeat);
            this.groupBox1.Location = new System.Drawing.Point(13, 5);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(566, 226);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Select The Feature to do Copy Address";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(20, 15);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(136, 23);
            this.button1.TabIndex = 1;
            this.button1.Text = "Add Feature";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // dvFeat
            // 
            this.dvFeat.AllowUserToAddRows = false;
            this.dvFeat.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dvFeat.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.G3E_FNO,
            this.Feat_Name,
            this.G3E_FID,
            this.Code,
            this.StreetName1,
            this.StreetType1,
            this.PostalNumber,
            this.SectionName1,
            this.CityName1,
            this.StateCode1,
            this.STREET_NAME,
            this.STREET_TYPE,
            this.POSTAL_NUM,
            this.SECTION_NAME,
            this.CITY_NAME,
            this.STATE_CODE});
            this.dvFeat.Location = new System.Drawing.Point(20, 44);
            this.dvFeat.Name = "dvFeat";
            this.dvFeat.Size = new System.Drawing.Size(525, 170);
            this.dvFeat.TabIndex = 0;
            this.dvFeat.RowEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.dvFeat_RowEnter);
            // 
            // G3E_FNO
            // 
            this.G3E_FNO.HeaderText = "G3E FNO";
            this.G3E_FNO.Name = "G3E_FNO";
            // 
            // Feat_Name
            // 
            this.Feat_Name.HeaderText = "Feature Name";
            this.Feat_Name.Name = "Feat_Name";
            // 
            // G3E_FID
            // 
            this.G3E_FID.HeaderText = "G3E FID";
            this.G3E_FID.Name = "G3E_FID";
            // 
            // Code
            // 
            this.Code.HeaderText = "Code";
            this.Code.Name = "Code";
            // 
            // StreetName1
            // 
            this.StreetName1.HeaderText = "Street Name";
            this.StreetName1.Name = "StreetName1";
            // 
            // StreetType1
            // 
            this.StreetType1.HeaderText = "Street Type";
            this.StreetType1.Name = "StreetType1";
            // 
            // PostalNumber
            // 
            this.PostalNumber.HeaderText = "Postal Code";
            this.PostalNumber.Name = "PostalNumber";
            // 
            // SectionName1
            // 
            this.SectionName1.HeaderText = "Section Name";
            this.SectionName1.Name = "SectionName1";
            // 
            // CityName1
            // 
            this.CityName1.HeaderText = "City Name";
            this.CityName1.Name = "CityName1";
            // 
            // StateCode1
            // 
            this.StateCode1.HeaderText = "State Code";
            this.StateCode1.Name = "StateCode1";
            // 
            // STREET_NAME
            // 
            this.STREET_NAME.HeaderText = "Street Name";
            this.STREET_NAME.Name = "STREET_NAME";
            this.STREET_NAME.Visible = false;
            // 
            // STREET_TYPE
            // 
            this.STREET_TYPE.HeaderText = "Street Type";
            this.STREET_TYPE.Name = "STREET_TYPE";
            this.STREET_TYPE.Visible = false;
            // 
            // POSTAL_NUM
            // 
            this.POSTAL_NUM.HeaderText = "Postal Num";
            this.POSTAL_NUM.Name = "POSTAL_NUM";
            this.POSTAL_NUM.Visible = false;
            // 
            // SECTION_NAME
            // 
            this.SECTION_NAME.HeaderText = "Section Name";
            this.SECTION_NAME.Name = "SECTION_NAME";
            this.SECTION_NAME.Visible = false;
            // 
            // CITY_NAME
            // 
            this.CITY_NAME.HeaderText = "City Name";
            this.CITY_NAME.Name = "CITY_NAME";
            this.CITY_NAME.Visible = false;
            // 
            // STATE_CODE
            // 
            this.STATE_CODE.HeaderText = "State Code";
            this.STATE_CODE.Name = "STATE_CODE";
            this.STATE_CODE.Visible = false;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label11);
            this.groupBox2.Controls.Add(this.label10);
            this.groupBox2.Controls.Add(this.label9);
            this.groupBox2.Controls.Add(this.label8);
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Controls.Add(this.label6);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.button4);
            this.groupBox2.Controls.Add(this.button3);
            this.groupBox2.Controls.Add(this.button2);
            this.groupBox2.Controls.Add(this.txtStreet);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Location = new System.Drawing.Point(13, 234);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(566, 123);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Select the Street";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(385, 64);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(0, 13);
            this.label11.TabIndex = 14;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(315, 64);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(60, 13);
            this.label10.TabIndex = 13;
            this.label10.Text = "State Code";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(385, 44);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(0, 13);
            this.label9.TabIndex = 12;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(315, 44);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(55, 13);
            this.label8.TabIndex = 11;
            this.label8.Text = "City Name";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(84, 83);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(53, 13);
            this.label7.TabIndex = 10;
            this.label7.Text = "Postal No";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(84, 64);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(74, 13);
            this.label6.TabIndex = 9;
            this.label6.Text = "Section Name";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(84, 44);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(62, 13);
            this.label5.TabIndex = 8;
            this.label5.Text = "Street Type";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(164, 83);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(0, 13);
            this.label4.TabIndex = 7;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(164, 64);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(0, 13);
            this.label3.TabIndex = 6;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(164, 44);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(0, 13);
            this.label2.TabIndex = 5;
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(311, 92);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(75, 23);
            this.button4.TabIndex = 4;
            this.button4.Text = "Close";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(220, 92);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(75, 23);
            this.button3.TabIndex = 3;
            this.button3.Text = "Update";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(371, 13);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(114, 23);
            this.button2.TabIndex = 2;
            this.button2.Text = "Select Admin Street";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // txtStreet
            // 
            this.txtStreet.BackColor = System.Drawing.Color.White;
            this.txtStreet.ForeColor = System.Drawing.Color.Black;
            this.txtStreet.Location = new System.Drawing.Point(161, 16);
            this.txtStreet.Name = "txtStreet";
            this.txtStreet.ReadOnly = true;
            this.txtStreet.Size = new System.Drawing.Size(201, 20);
            this.txtStreet.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(84, 23);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(66, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Street Name";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.label14);
            this.groupBox3.Controls.Add(this.label13);
            this.groupBox3.Controls.Add(this.label12);
            this.groupBox3.Location = new System.Drawing.Point(13, 359);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(566, 85);
            this.groupBox3.TabIndex = 2;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "To Do";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(20, 66);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(93, 13);
            this.label14.TabIndex = 2;
            this.label14.Text = "3. Click \"Update\" ";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(20, 43);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(321, 13);
            this.label13.TabIndex = 1;
            this.label13.Text = "2. Select the Admin Street on Map and click \"Select Admin Street\"";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(20, 20);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(300, 13);
            this.label12.TabIndex = 0;
            this.label12.Text = "1. Select the feature to Copy Address and click \"Add Feature\"";
            // 
            // CopyStreetAddress
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(591, 456);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "CopyStreetAddress";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Copy Street Address v1.7";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.GTWindowsForm_InitRoute_Load);
            this.Shown += new System.EventHandler(this.GTWindowsForm_InitRoute_Shown);
            this.Activated += new System.EventHandler(this.GTWindowsForm_InitRoute_Activated);
            this.groupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dvFeat)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.DataGridView dvFeat;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.TextBox txtStreet;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DataGridViewTextBoxColumn G3E_FNO;
        private System.Windows.Forms.DataGridViewTextBoxColumn Feat_Name;
        private System.Windows.Forms.DataGridViewTextBoxColumn G3E_FID;
        private System.Windows.Forms.DataGridViewTextBoxColumn Code;
        private System.Windows.Forms.DataGridViewTextBoxColumn StreetName1;
        private System.Windows.Forms.DataGridViewTextBoxColumn StreetType1;
        private System.Windows.Forms.DataGridViewTextBoxColumn PostalNumber;
        private System.Windows.Forms.DataGridViewTextBoxColumn SectionName1;
        private System.Windows.Forms.DataGridViewTextBoxColumn CityName1;
        private System.Windows.Forms.DataGridViewTextBoxColumn StateCode1;
        private System.Windows.Forms.DataGridViewTextBoxColumn STREET_NAME;
        private System.Windows.Forms.DataGridViewTextBoxColumn STREET_TYPE;
        private System.Windows.Forms.DataGridViewTextBoxColumn POSTAL_NUM;
        private System.Windows.Forms.DataGridViewTextBoxColumn SECTION_NAME;
        private System.Windows.Forms.DataGridViewTextBoxColumn CITY_NAME;
        private System.Windows.Forms.DataGridViewTextBoxColumn STATE_CODE;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Label label13;



    }
}