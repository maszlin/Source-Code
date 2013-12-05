namespace AG.GTechnology.OffsetPointSPT_Cbl
{
    partial class frmODF
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
            this.lstODF = new System.Windows.Forms.ListView();
            this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.chkbox_nr_conn = new System.Windows.Forms.CheckBox();
            this.chkbox_nr_conn2 = new System.Windows.Forms.CheckBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(2, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(185, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Specify the source ODF in Exchange:";
            // 
            // lstODF
            // 
            this.lstODF.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2});
            this.lstODF.FullRowSelect = true;
            this.lstODF.Location = new System.Drawing.Point(5, 35);
            this.lstODF.MultiSelect = false;
            this.lstODF.Name = "lstODF";
            this.lstODF.Size = new System.Drawing.Size(287, 181);
            this.lstODF.TabIndex = 1;
            this.lstODF.UseCompatibleStateImageBehavior = false;
            this.lstODF.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "ODF";
            this.columnHeader1.Width = 201;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Horz Num.";
            this.columnHeader2.Width = 70;
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(12, 291);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 2;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(93, 291);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 2;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // chkbox_nr_conn
            // 
            this.chkbox_nr_conn.AutoSize = true;
            this.chkbox_nr_conn.Checked = true;
            this.chkbox_nr_conn.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkbox_nr_conn.Location = new System.Drawing.Point(7, 11);
            this.chkbox_nr_conn.Name = "chkbox_nr_conn";
            this.chkbox_nr_conn.Size = new System.Drawing.Size(137, 17);
            this.chkbox_nr_conn.TabIndex = 3;
            this.chkbox_nr_conn.Text = "ODF connects to Cable";
            this.chkbox_nr_conn.UseVisualStyleBackColor = true;
            this.chkbox_nr_conn.CheckedChanged += new System.EventHandler(this.chkbox_nr_conn_CheckedChanged);
            // 
            // chkbox_nr_conn2
            // 
            this.chkbox_nr_conn2.AutoSize = true;
            this.chkbox_nr_conn2.Location = new System.Drawing.Point(7, 34);
            this.chkbox_nr_conn2.Name = "chkbox_nr_conn2";
            this.chkbox_nr_conn2.Size = new System.Drawing.Size(137, 17);
            this.chkbox_nr_conn2.TabIndex = 4;
            this.chkbox_nr_conn2.Text = "Cable connects to ODF";
            this.chkbox_nr_conn2.UseVisualStyleBackColor = true;
            this.chkbox_nr_conn2.CheckedChanged += new System.EventHandler(this.chkbox_nr_conn2_CheckedChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.chkbox_nr_conn);
            this.groupBox1.Controls.Add(this.chkbox_nr_conn2);
            this.groupBox1.Location = new System.Drawing.Point(5, 222);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(171, 55);
            this.groupBox1.TabIndex = 5;
            this.groupBox1.TabStop = false;
            // 
            // frmODF
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(297, 326);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.lstODF);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "frmODF";
            this.Text = "ODF";
            this.Load += new System.EventHandler(this.frmODF_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ListView lstODF;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.CheckBox chkbox_nr_conn;
        private System.Windows.Forms.CheckBox chkbox_nr_conn2;
        private System.Windows.Forms.GroupBox groupBox1;
    }
}