namespace NEPS.GTechnology.SDFplacement
{
    partial class SDF_Plc_Form
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
            this.gbVDSL2 = new System.Windows.Forms.GroupBox();
            this.btn_Pick = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.txtParantCode = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtParant = new System.Windows.Forms.TextBox();
            this.gbSDFNum = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.txtSDFNum = new System.Windows.Forms.TextBox();
            this.gbSDFCodes = new System.Windows.Forms.GroupBox();
            this.lbl10 = new System.Windows.Forms.Label();
            this.txt10 = new System.Windows.Forms.TextBox();
            this.lbl9 = new System.Windows.Forms.Label();
            this.lbl8 = new System.Windows.Forms.Label();
            this.lbl7 = new System.Windows.Forms.Label();
            this.txt9 = new System.Windows.Forms.TextBox();
            this.txt8 = new System.Windows.Forms.TextBox();
            this.txt7 = new System.Windows.Forms.TextBox();
            this.txt6 = new System.Windows.Forms.TextBox();
            this.lbl6 = new System.Windows.Forms.Label();
            this.lbl5 = new System.Windows.Forms.Label();
            this.lbl4 = new System.Windows.Forms.Label();
            this.lbl3 = new System.Windows.Forms.Label();
            this.lbl2 = new System.Windows.Forms.Label();
            this.txt5 = new System.Windows.Forms.TextBox();
            this.txt4 = new System.Windows.Forms.TextBox();
            this.txt3 = new System.Windows.Forms.TextBox();
            this.txt2 = new System.Windows.Forms.TextBox();
            this.txt1 = new System.Windows.Forms.TextBox();
            this.lbl1 = new System.Windows.Forms.Label();
            this.btnGenerateSDF = new System.Windows.Forms.Button();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.label16 = new System.Windows.Forms.Label();
            this.label17 = new System.Windows.Forms.Label();
            this.label18 = new System.Windows.Forms.Label();
            this.label21 = new System.Windows.Forms.Label();
            this.label22 = new System.Windows.Forms.Label();
            this.label23 = new System.Windows.Forms.Label();
            this.tbcSDFplc = new System.Windows.Forms.TabControl();
            this.tabSDFAttr = new System.Windows.Forms.TabPage();
            this.tabGranite = new System.Windows.Forms.TabPage();
            this.cbGrTemplate = new System.Windows.Forms.ComboBox();
            this.cbContractor = new System.Windows.Forms.ComboBox();
            this.cbManufacturer = new System.Windows.Forms.ComboBox();
            this.cbDeveloper = new System.Windows.Forms.ComboBox();
            this.cbAccessRestr = new System.Windows.Forms.ComboBox();
            this.cbCableType = new System.Windows.Forms.ComboBox();
            this.txtProjectID = new System.Windows.Forms.TextBox();
            this.txtComment = new System.Windows.Forms.TextBox();
            this.txtContact = new System.Windows.Forms.TextBox();
            this.cbFiberToPremise = new System.Windows.Forms.ComboBox();
            this.cbCopperOwnTM = new System.Windows.Forms.ComboBox();
            this.cbEquipLoc = new System.Windows.Forms.ComboBox();
            this.cbServiceDate = new System.Windows.Forms.DateTimePicker();
            this.gbVDSL2.SuspendLayout();
            this.gbSDFNum.SuspendLayout();
            this.gbSDFCodes.SuspendLayout();
            this.tbcSDFplc.SuspendLayout();
            this.tabSDFAttr.SuspendLayout();
            this.tabGranite.SuspendLayout();
            this.SuspendLayout();
            // 
            // gbVDSL2
            // 
            this.gbVDSL2.Controls.Add(this.btn_Pick);
            this.gbVDSL2.Controls.Add(this.label5);
            this.gbVDSL2.Controls.Add(this.txtParantCode);
            this.gbVDSL2.Controls.Add(this.label3);
            this.gbVDSL2.Controls.Add(this.txtParant);
            this.gbVDSL2.Location = new System.Drawing.Point(15, 18);
            this.gbVDSL2.Name = "gbVDSL2";
            this.gbVDSL2.Size = new System.Drawing.Size(363, 86);
            this.gbVDSL2.TabIndex = 30;
            this.gbVDSL2.TabStop = false;
            this.gbVDSL2.Text = "VDSL2 :";
            // 
            // btn_Pick
            // 
            this.btn_Pick.Location = new System.Drawing.Point(260, 29);
            this.btn_Pick.Name = "btn_Pick";
            this.btn_Pick.Size = new System.Drawing.Size(75, 25);
            this.btn_Pick.TabIndex = 24;
            this.btn_Pick.Text = "Pick";
            this.btn_Pick.UseVisualStyleBackColor = true;
            this.btn_Pick.Click += new System.EventHandler(this.btn_Pick_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(6, 55);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(32, 13);
            this.label5.TabIndex = 27;
            this.label5.Text = "Code";
            // 
            // txtParantCode
            // 
            this.txtParantCode.BackColor = System.Drawing.Color.White;
            this.txtParantCode.ForeColor = System.Drawing.Color.Black;
            this.txtParantCode.Location = new System.Drawing.Point(92, 55);
            this.txtParantCode.Name = "txtParantCode";
            this.txtParantCode.ReadOnly = true;
            this.txtParantCode.Size = new System.Drawing.Size(137, 20);
            this.txtParantCode.TabIndex = 28;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 30);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(27, 13);
            this.label3.TabIndex = 25;
            this.label3.Text = " FID";
            // 
            // txtParant
            // 
            this.txtParant.BackColor = System.Drawing.Color.White;
            this.txtParant.ForeColor = System.Drawing.Color.Black;
            this.txtParant.Location = new System.Drawing.Point(92, 30);
            this.txtParant.Name = "txtParant";
            this.txtParant.ReadOnly = true;
            this.txtParant.Size = new System.Drawing.Size(137, 20);
            this.txtParant.TabIndex = 26;
            // 
            // gbSDFNum
            // 
            this.gbSDFNum.Controls.Add(this.label1);
            this.gbSDFNum.Controls.Add(this.txtSDFNum);
            this.gbSDFNum.Location = new System.Drawing.Point(15, 110);
            this.gbSDFNum.Name = "gbSDFNum";
            this.gbSDFNum.Size = new System.Drawing.Size(363, 60);
            this.gbSDFNum.TabIndex = 31;
            this.gbSDFNum.TabStop = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(6, 26);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(93, 13);
            this.label1.TabIndex = 33;
            this.label1.Text = "Number of SDF";
            // 
            // txtSDFNum
            // 
            this.txtSDFNum.BackColor = System.Drawing.Color.White;
            this.txtSDFNum.ForeColor = System.Drawing.Color.Black;
            this.txtSDFNum.Location = new System.Drawing.Point(105, 23);
            this.txtSDFNum.Name = "txtSDFNum";
            this.txtSDFNum.Size = new System.Drawing.Size(124, 20);
            this.txtSDFNum.TabIndex = 27;
            // 
            // gbSDFCodes
            // 
            this.gbSDFCodes.Controls.Add(this.lbl10);
            this.gbSDFCodes.Controls.Add(this.txt10);
            this.gbSDFCodes.Controls.Add(this.lbl9);
            this.gbSDFCodes.Controls.Add(this.lbl8);
            this.gbSDFCodes.Controls.Add(this.lbl7);
            this.gbSDFCodes.Controls.Add(this.txt9);
            this.gbSDFCodes.Controls.Add(this.txt8);
            this.gbSDFCodes.Controls.Add(this.txt7);
            this.gbSDFCodes.Controls.Add(this.txt6);
            this.gbSDFCodes.Controls.Add(this.lbl6);
            this.gbSDFCodes.Controls.Add(this.lbl5);
            this.gbSDFCodes.Controls.Add(this.lbl4);
            this.gbSDFCodes.Controls.Add(this.lbl3);
            this.gbSDFCodes.Controls.Add(this.lbl2);
            this.gbSDFCodes.Controls.Add(this.txt5);
            this.gbSDFCodes.Controls.Add(this.txt4);
            this.gbSDFCodes.Controls.Add(this.txt3);
            this.gbSDFCodes.Controls.Add(this.txt2);
            this.gbSDFCodes.Controls.Add(this.txt1);
            this.gbSDFCodes.Controls.Add(this.lbl1);
            this.gbSDFCodes.Location = new System.Drawing.Point(15, 176);
            this.gbSDFCodes.Name = "gbSDFCodes";
            this.gbSDFCodes.Size = new System.Drawing.Size(363, 145);
            this.gbSDFCodes.TabIndex = 32;
            this.gbSDFCodes.TabStop = false;
            this.gbSDFCodes.Text = "SDF Codes [eg: 0001]";
            // 
            // lbl10
            // 
            this.lbl10.AutoSize = true;
            this.lbl10.Location = new System.Drawing.Point(176, 114);
            this.lbl10.Name = "lbl10";
            this.lbl10.Size = new System.Drawing.Size(22, 13);
            this.lbl10.TabIndex = 54;
            this.lbl10.Text = "10.";
            // 
            // txt10
            // 
            this.txt10.BackColor = System.Drawing.Color.White;
            this.txt10.ForeColor = System.Drawing.Color.Black;
            this.txt10.Location = new System.Drawing.Point(211, 111);
            this.txt10.Name = "txt10";
            this.txt10.Size = new System.Drawing.Size(124, 20);
            this.txt10.TabIndex = 53;
            // 
            // lbl9
            // 
            this.lbl9.AutoSize = true;
            this.lbl9.Location = new System.Drawing.Point(176, 94);
            this.lbl9.Name = "lbl9";
            this.lbl9.Size = new System.Drawing.Size(16, 13);
            this.lbl9.TabIndex = 51;
            this.lbl9.Text = "9.";
            // 
            // lbl8
            // 
            this.lbl8.AutoSize = true;
            this.lbl8.Location = new System.Drawing.Point(176, 74);
            this.lbl8.Name = "lbl8";
            this.lbl8.Size = new System.Drawing.Size(16, 13);
            this.lbl8.TabIndex = 50;
            this.lbl8.Text = "8.";
            // 
            // lbl7
            // 
            this.lbl7.AutoSize = true;
            this.lbl7.Location = new System.Drawing.Point(176, 54);
            this.lbl7.Name = "lbl7";
            this.lbl7.Size = new System.Drawing.Size(16, 13);
            this.lbl7.TabIndex = 49;
            this.lbl7.Text = "7.";
            // 
            // txt9
            // 
            this.txt9.BackColor = System.Drawing.Color.White;
            this.txt9.ForeColor = System.Drawing.Color.Black;
            this.txt9.Location = new System.Drawing.Point(211, 91);
            this.txt9.Name = "txt9";
            this.txt9.Size = new System.Drawing.Size(124, 20);
            this.txt9.TabIndex = 47;
            // 
            // txt8
            // 
            this.txt8.BackColor = System.Drawing.Color.White;
            this.txt8.ForeColor = System.Drawing.Color.Black;
            this.txt8.Location = new System.Drawing.Point(211, 71);
            this.txt8.Name = "txt8";
            this.txt8.Size = new System.Drawing.Size(124, 20);
            this.txt8.TabIndex = 46;
            // 
            // txt7
            // 
            this.txt7.BackColor = System.Drawing.Color.White;
            this.txt7.ForeColor = System.Drawing.Color.Black;
            this.txt7.Location = new System.Drawing.Point(211, 51);
            this.txt7.Name = "txt7";
            this.txt7.Size = new System.Drawing.Size(124, 20);
            this.txt7.TabIndex = 45;
            // 
            // txt6
            // 
            this.txt6.BackColor = System.Drawing.Color.White;
            this.txt6.ForeColor = System.Drawing.Color.Black;
            this.txt6.Location = new System.Drawing.Point(211, 31);
            this.txt6.Name = "txt6";
            this.txt6.Size = new System.Drawing.Size(124, 20);
            this.txt6.TabIndex = 44;
            // 
            // lbl6
            // 
            this.lbl6.AutoSize = true;
            this.lbl6.Location = new System.Drawing.Point(176, 34);
            this.lbl6.Name = "lbl6";
            this.lbl6.Size = new System.Drawing.Size(16, 13);
            this.lbl6.TabIndex = 43;
            this.lbl6.Text = "6.";
            // 
            // lbl5
            // 
            this.lbl5.AutoSize = true;
            this.lbl5.Location = new System.Drawing.Point(6, 114);
            this.lbl5.Name = "lbl5";
            this.lbl5.Size = new System.Drawing.Size(16, 13);
            this.lbl5.TabIndex = 42;
            this.lbl5.Text = "5.";
            // 
            // lbl4
            // 
            this.lbl4.AutoSize = true;
            this.lbl4.Location = new System.Drawing.Point(6, 94);
            this.lbl4.Name = "lbl4";
            this.lbl4.Size = new System.Drawing.Size(16, 13);
            this.lbl4.TabIndex = 41;
            this.lbl4.Text = "4.";
            // 
            // lbl3
            // 
            this.lbl3.AutoSize = true;
            this.lbl3.Location = new System.Drawing.Point(6, 74);
            this.lbl3.Name = "lbl3";
            this.lbl3.Size = new System.Drawing.Size(16, 13);
            this.lbl3.TabIndex = 40;
            this.lbl3.Text = "3.";
            // 
            // lbl2
            // 
            this.lbl2.AutoSize = true;
            this.lbl2.Location = new System.Drawing.Point(6, 54);
            this.lbl2.Name = "lbl2";
            this.lbl2.Size = new System.Drawing.Size(16, 13);
            this.lbl2.TabIndex = 39;
            this.lbl2.Text = "2.";
            // 
            // txt5
            // 
            this.txt5.BackColor = System.Drawing.Color.White;
            this.txt5.ForeColor = System.Drawing.Color.Black;
            this.txt5.Location = new System.Drawing.Point(41, 111);
            this.txt5.Name = "txt5";
            this.txt5.Size = new System.Drawing.Size(124, 20);
            this.txt5.TabIndex = 38;
            // 
            // txt4
            // 
            this.txt4.BackColor = System.Drawing.Color.White;
            this.txt4.ForeColor = System.Drawing.Color.Black;
            this.txt4.Location = new System.Drawing.Point(41, 91);
            this.txt4.Name = "txt4";
            this.txt4.Size = new System.Drawing.Size(124, 20);
            this.txt4.TabIndex = 37;
            // 
            // txt3
            // 
            this.txt3.BackColor = System.Drawing.Color.White;
            this.txt3.ForeColor = System.Drawing.Color.Black;
            this.txt3.Location = new System.Drawing.Point(41, 71);
            this.txt3.Name = "txt3";
            this.txt3.Size = new System.Drawing.Size(124, 20);
            this.txt3.TabIndex = 36;
            // 
            // txt2
            // 
            this.txt2.BackColor = System.Drawing.Color.White;
            this.txt2.ForeColor = System.Drawing.Color.Black;
            this.txt2.Location = new System.Drawing.Point(41, 51);
            this.txt2.Name = "txt2";
            this.txt2.Size = new System.Drawing.Size(124, 20);
            this.txt2.TabIndex = 35;
            // 
            // txt1
            // 
            this.txt1.BackColor = System.Drawing.Color.White;
            this.txt1.ForeColor = System.Drawing.Color.Black;
            this.txt1.Location = new System.Drawing.Point(41, 31);
            this.txt1.Name = "txt1";
            this.txt1.Size = new System.Drawing.Size(124, 20);
            this.txt1.TabIndex = 34;
            // 
            // lbl1
            // 
            this.lbl1.AutoSize = true;
            this.lbl1.Location = new System.Drawing.Point(6, 34);
            this.lbl1.Name = "lbl1";
            this.lbl1.Size = new System.Drawing.Size(16, 13);
            this.lbl1.TabIndex = 0;
            this.lbl1.Text = "1.";
            // 
            // btnGenerateSDF
            // 
            this.btnGenerateSDF.Location = new System.Drawing.Point(31, 378);
            this.btnGenerateSDF.Name = "btnGenerateSDF";
            this.btnGenerateSDF.Size = new System.Drawing.Size(124, 31);
            this.btnGenerateSDF.TabIndex = 33;
            this.btnGenerateSDF.Text = "Generate SDF";
            this.btnGenerateSDF.UseVisualStyleBackColor = true;
            this.btnGenerateSDF.Click += new System.EventHandler(this.btnGenerateSDF_Click);
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(31, 361);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(363, 17);
            this.progressBar1.TabIndex = 29;
            this.progressBar1.Visible = false;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label9.Location = new System.Drawing.Point(16, 118);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(92, 13);
            this.label9.TabIndex = 45;
            this.label9.Text = "Equip Location";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label10.Location = new System.Drawing.Point(16, 96);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(95, 13);
            this.label10.TabIndex = 44;
            this.label10.Text = "Access Restriction";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label11.Location = new System.Drawing.Point(16, 74);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(82, 13);
            this.label11.TabIndex = 43;
            this.label11.Text = "Manufacturer";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label12.Location = new System.Drawing.Point(16, 52);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(66, 13);
            this.label12.TabIndex = 42;
            this.label12.Text = "Contractor";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label13.Location = new System.Drawing.Point(16, 30);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(104, 13);
            this.label13.TabIndex = 41;
            this.label13.Text = "Granite Template";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label14.Location = new System.Drawing.Point(16, 227);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(88, 13);
            this.label14.TabIndex = 50;
            this.label14.Text = "Granite Comment";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label15.Location = new System.Drawing.Point(16, 206);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(81, 13);
            this.label15.TabIndex = 49;
            this.label15.Text = "Granite Contact";
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label16.Location = new System.Drawing.Point(16, 184);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(98, 13);
            this.label16.TabIndex = 48;
            this.label16.Text = "Granite Cable Type";
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label17.Location = new System.Drawing.Point(16, 162);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(133, 13);
            this.label17.TabIndex = 47;
            this.label17.Text = "Fiber To Premise Exist";
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label18.Location = new System.Drawing.Point(16, 140);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(116, 13);
            this.label18.TabIndex = 46;
            this.label18.Text = "Copper Own By TM";
            // 
            // label21
            // 
            this.label21.AutoSize = true;
            this.label21.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label21.Location = new System.Drawing.Point(16, 292);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(81, 13);
            this.label21.TabIndex = 53;
            this.label21.Text = "In Service Date";
            // 
            // label22
            // 
            this.label22.AutoSize = true;
            this.label22.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label22.Location = new System.Drawing.Point(16, 270);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(95, 13);
            this.label22.TabIndex = 52;
            this.label22.Text = "SMART Project ID";
            // 
            // label23
            // 
            this.label23.AutoSize = true;
            this.label23.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label23.Location = new System.Drawing.Point(16, 248);
            this.label23.Name = "label23";
            this.label23.Size = new System.Drawing.Size(97, 13);
            this.label23.TabIndex = 51;
            this.label23.Text = "SMART Developer";
            // 
            // tbcSDFplc
            // 
            this.tbcSDFplc.Controls.Add(this.tabSDFAttr);
            this.tbcSDFplc.Controls.Add(this.tabGranite);
            this.tbcSDFplc.Location = new System.Drawing.Point(12, 12);
            this.tbcSDFplc.Name = "tbcSDFplc";
            this.tbcSDFplc.SelectedIndex = 0;
            this.tbcSDFplc.Size = new System.Drawing.Size(400, 360);
            this.tbcSDFplc.TabIndex = 54;
            // 
            // tabSDFAttr
            // 
            this.tabSDFAttr.Controls.Add(this.gbVDSL2);
            this.tabSDFAttr.Controls.Add(this.gbSDFCodes);
            this.tabSDFAttr.Controls.Add(this.gbSDFNum);
            this.tabSDFAttr.Location = new System.Drawing.Point(4, 22);
            this.tabSDFAttr.Name = "tabSDFAttr";
            this.tabSDFAttr.Padding = new System.Windows.Forms.Padding(3);
            this.tabSDFAttr.Size = new System.Drawing.Size(392, 334);
            this.tabSDFAttr.TabIndex = 0;
            this.tabSDFAttr.Text = "SDF";
            this.tabSDFAttr.UseVisualStyleBackColor = true;
            // 
            // tabGranite
            // 
            this.tabGranite.Controls.Add(this.cbGrTemplate);
            this.tabGranite.Controls.Add(this.cbContractor);
            this.tabGranite.Controls.Add(this.cbManufacturer);
            this.tabGranite.Controls.Add(this.cbDeveloper);
            this.tabGranite.Controls.Add(this.cbAccessRestr);
            this.tabGranite.Controls.Add(this.cbCableType);
            this.tabGranite.Controls.Add(this.txtProjectID);
            this.tabGranite.Controls.Add(this.txtComment);
            this.tabGranite.Controls.Add(this.txtContact);
            this.tabGranite.Controls.Add(this.cbFiberToPremise);
            this.tabGranite.Controls.Add(this.cbCopperOwnTM);
            this.tabGranite.Controls.Add(this.cbEquipLoc);
            this.tabGranite.Controls.Add(this.cbServiceDate);
            this.tabGranite.Controls.Add(this.label10);
            this.tabGranite.Controls.Add(this.label21);
            this.tabGranite.Controls.Add(this.label13);
            this.tabGranite.Controls.Add(this.label22);
            this.tabGranite.Controls.Add(this.label12);
            this.tabGranite.Controls.Add(this.label23);
            this.tabGranite.Controls.Add(this.label11);
            this.tabGranite.Controls.Add(this.label14);
            this.tabGranite.Controls.Add(this.label9);
            this.tabGranite.Controls.Add(this.label15);
            this.tabGranite.Controls.Add(this.label18);
            this.tabGranite.Controls.Add(this.label16);
            this.tabGranite.Controls.Add(this.label17);
            this.tabGranite.Location = new System.Drawing.Point(4, 22);
            this.tabGranite.Name = "tabGranite";
            this.tabGranite.Padding = new System.Windows.Forms.Padding(3);
            this.tabGranite.Size = new System.Drawing.Size(392, 334);
            this.tabGranite.TabIndex = 1;
            this.tabGranite.Text = "SDF Granite";
            this.tabGranite.UseVisualStyleBackColor = true;
            // 
            // cbGrTemplate
            // 
            this.cbGrTemplate.BackColor = System.Drawing.Color.White;
            this.cbGrTemplate.ForeColor = System.Drawing.Color.Black;
            this.cbGrTemplate.FormattingEnabled = true;
            this.cbGrTemplate.Location = new System.Drawing.Point(169, 27);
            this.cbGrTemplate.Name = "cbGrTemplate";
            this.cbGrTemplate.Size = new System.Drawing.Size(200, 21);
            this.cbGrTemplate.TabIndex = 69;
            this.cbGrTemplate.SelectedValueChanged += new System.EventHandler(this.cbGrTemplate_SelectedValueChanged);
            // 
            // cbContractor
            // 
            this.cbContractor.BackColor = System.Drawing.Color.White;
            this.cbContractor.ForeColor = System.Drawing.Color.Black;
            this.cbContractor.FormattingEnabled = true;
            this.cbContractor.Location = new System.Drawing.Point(169, 49);
            this.cbContractor.Name = "cbContractor";
            this.cbContractor.Size = new System.Drawing.Size(200, 21);
            this.cbContractor.TabIndex = 70;
            this.cbContractor.SelectedValueChanged += new System.EventHandler(this.cbContractor_SelectedValueChanged);
            // 
            // cbManufacturer
            // 
            this.cbManufacturer.BackColor = System.Drawing.Color.White;
            this.cbManufacturer.ForeColor = System.Drawing.Color.Black;
            this.cbManufacturer.FormattingEnabled = true;
            this.cbManufacturer.Location = new System.Drawing.Point(169, 71);
            this.cbManufacturer.Name = "cbManufacturer";
            this.cbManufacturer.Size = new System.Drawing.Size(200, 21);
            this.cbManufacturer.TabIndex = 71;
            this.cbManufacturer.SelectedValueChanged += new System.EventHandler(this.cbManufacturer_SelectedValueChanged);
            // 
            // cbDeveloper
            // 
            this.cbDeveloper.BackColor = System.Drawing.Color.White;
            this.cbDeveloper.ForeColor = System.Drawing.Color.Black;
            this.cbDeveloper.FormattingEnabled = true;
            this.cbDeveloper.Location = new System.Drawing.Point(169, 245);
            this.cbDeveloper.Name = "cbDeveloper";
            this.cbDeveloper.Size = new System.Drawing.Size(200, 21);
            this.cbDeveloper.TabIndex = 72;
            // 
            // cbAccessRestr
            // 
            this.cbAccessRestr.BackColor = System.Drawing.Color.White;
            this.cbAccessRestr.ForeColor = System.Drawing.Color.Black;
            this.cbAccessRestr.FormattingEnabled = true;
            this.cbAccessRestr.Location = new System.Drawing.Point(169, 93);
            this.cbAccessRestr.Name = "cbAccessRestr";
            this.cbAccessRestr.Size = new System.Drawing.Size(200, 21);
            this.cbAccessRestr.TabIndex = 68;
            // 
            // cbCableType
            // 
            this.cbCableType.BackColor = System.Drawing.Color.White;
            this.cbCableType.ForeColor = System.Drawing.Color.Black;
            this.cbCableType.FormattingEnabled = true;
            this.cbCableType.Location = new System.Drawing.Point(169, 181);
            this.cbCableType.Name = "cbCableType";
            this.cbCableType.Size = new System.Drawing.Size(200, 21);
            this.cbCableType.TabIndex = 67;
            // 
            // txtProjectID
            // 
            this.txtProjectID.BackColor = System.Drawing.Color.White;
            this.txtProjectID.ForeColor = System.Drawing.Color.Black;
            this.txtProjectID.Location = new System.Drawing.Point(169, 267);
            this.txtProjectID.Name = "txtProjectID";
            this.txtProjectID.Size = new System.Drawing.Size(200, 20);
            this.txtProjectID.TabIndex = 66;
            // 
            // txtComment
            // 
            this.txtComment.BackColor = System.Drawing.Color.White;
            this.txtComment.ForeColor = System.Drawing.Color.Black;
            this.txtComment.Location = new System.Drawing.Point(169, 224);
            this.txtComment.Name = "txtComment";
            this.txtComment.Size = new System.Drawing.Size(200, 20);
            this.txtComment.TabIndex = 64;
            // 
            // txtContact
            // 
            this.txtContact.BackColor = System.Drawing.Color.White;
            this.txtContact.ForeColor = System.Drawing.Color.Black;
            this.txtContact.Location = new System.Drawing.Point(169, 203);
            this.txtContact.Name = "txtContact";
            this.txtContact.Size = new System.Drawing.Size(200, 20);
            this.txtContact.TabIndex = 63;
            // 
            // cbFiberToPremise
            // 
            this.cbFiberToPremise.BackColor = System.Drawing.Color.White;
            this.cbFiberToPremise.ForeColor = System.Drawing.Color.Black;
            this.cbFiberToPremise.FormattingEnabled = true;
            this.cbFiberToPremise.Location = new System.Drawing.Point(169, 159);
            this.cbFiberToPremise.Name = "cbFiberToPremise";
            this.cbFiberToPremise.Size = new System.Drawing.Size(200, 21);
            this.cbFiberToPremise.TabIndex = 62;
            // 
            // cbCopperOwnTM
            // 
            this.cbCopperOwnTM.BackColor = System.Drawing.Color.White;
            this.cbCopperOwnTM.ForeColor = System.Drawing.Color.Black;
            this.cbCopperOwnTM.FormattingEnabled = true;
            this.cbCopperOwnTM.Location = new System.Drawing.Point(169, 137);
            this.cbCopperOwnTM.Name = "cbCopperOwnTM";
            this.cbCopperOwnTM.Size = new System.Drawing.Size(200, 21);
            this.cbCopperOwnTM.TabIndex = 61;
            // 
            // cbEquipLoc
            // 
            this.cbEquipLoc.BackColor = System.Drawing.Color.White;
            this.cbEquipLoc.ForeColor = System.Drawing.Color.Black;
            this.cbEquipLoc.FormattingEnabled = true;
            this.cbEquipLoc.Location = new System.Drawing.Point(169, 115);
            this.cbEquipLoc.Name = "cbEquipLoc";
            this.cbEquipLoc.Size = new System.Drawing.Size(200, 21);
            this.cbEquipLoc.TabIndex = 55;
            // 
            // cbServiceDate
            // 
            this.cbServiceDate.CalendarForeColor = System.Drawing.Color.Black;
            this.cbServiceDate.CalendarMonthBackground = System.Drawing.Color.White;
            this.cbServiceDate.CalendarTitleBackColor = System.Drawing.Color.Blue;
            this.cbServiceDate.CalendarTitleForeColor = System.Drawing.Color.White;
            this.cbServiceDate.CalendarTrailingForeColor = System.Drawing.Color.Gray;
            this.cbServiceDate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.cbServiceDate.Location = new System.Drawing.Point(169, 288);
            this.cbServiceDate.Name = "cbServiceDate";
            this.cbServiceDate.ShowCheckBox = true;
            this.cbServiceDate.Size = new System.Drawing.Size(200, 20);
            this.cbServiceDate.TabIndex = 54;
            this.cbServiceDate.Value = new System.DateTime(2013, 8, 7, 0, 0, 0, 0);
            this.cbServiceDate.ValueChanged += new System.EventHandler(this.cbServiceDate_ValueChanged);
            // 
            // SDF_Plc_Form
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(426, 418);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.tbcSDFplc);
            this.Controls.Add(this.btnGenerateSDF);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "SDF_Plc_Form";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "SDF Placement v1.1";
            this.TopMost = true;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.SDF_Plc_Form_FormClosing);
            this.gbVDSL2.ResumeLayout(false);
            this.gbVDSL2.PerformLayout();
            this.gbSDFNum.ResumeLayout(false);
            this.gbSDFNum.PerformLayout();
            this.gbSDFCodes.ResumeLayout(false);
            this.gbSDFCodes.PerformLayout();
            this.tbcSDFplc.ResumeLayout(false);
            this.tabSDFAttr.ResumeLayout(false);
            this.tabGranite.ResumeLayout(false);
            this.tabGranite.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox gbVDSL2;
        private System.Windows.Forms.Button btn_Pick;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtParantCode;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtParant;
        private System.Windows.Forms.GroupBox gbSDFNum;
        private System.Windows.Forms.GroupBox gbSDFCodes;
        private System.Windows.Forms.TextBox txtSDFNum;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txt1;
        private System.Windows.Forms.Label lbl1;
        private System.Windows.Forms.TextBox txt3;
        private System.Windows.Forms.TextBox txt2;
        private System.Windows.Forms.Label lbl9;
        private System.Windows.Forms.Label lbl8;
        private System.Windows.Forms.Label lbl7;
        private System.Windows.Forms.TextBox txt9;
        private System.Windows.Forms.TextBox txt8;
        private System.Windows.Forms.TextBox txt7;
        private System.Windows.Forms.TextBox txt6;
        private System.Windows.Forms.Label lbl6;
        private System.Windows.Forms.Label lbl5;
        private System.Windows.Forms.Label lbl4;
        private System.Windows.Forms.Label lbl3;
        private System.Windows.Forms.Label lbl2;
        private System.Windows.Forms.TextBox txt5;
        private System.Windows.Forms.TextBox txt4;
        private System.Windows.Forms.Label lbl10;
        private System.Windows.Forms.TextBox txt10;
        private System.Windows.Forms.Button btnGenerateSDF;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.Label label21;
        private System.Windows.Forms.Label label22;
        private System.Windows.Forms.Label label23;
        private System.Windows.Forms.TabControl tbcSDFplc;
        private System.Windows.Forms.TabPage tabSDFAttr;
        private System.Windows.Forms.TabPage tabGranite;
        private System.Windows.Forms.ComboBox cbEquipLoc;
        private System.Windows.Forms.DateTimePicker cbServiceDate;
        private System.Windows.Forms.TextBox txtProjectID;
        private System.Windows.Forms.TextBox txtComment;
        private System.Windows.Forms.TextBox txtContact;
        private System.Windows.Forms.ComboBox cbFiberToPremise;
        private System.Windows.Forms.ComboBox cbCopperOwnTM;
        private System.Windows.Forms.ComboBox cbCableType;
        private System.Windows.Forms.ComboBox cbAccessRestr;
        private System.Windows.Forms.ComboBox cbDeveloper;
        private System.Windows.Forms.ComboBox cbGrTemplate;
        private System.Windows.Forms.ComboBox cbContractor;
        private System.Windows.Forms.ComboBox cbManufacturer;
    }
}