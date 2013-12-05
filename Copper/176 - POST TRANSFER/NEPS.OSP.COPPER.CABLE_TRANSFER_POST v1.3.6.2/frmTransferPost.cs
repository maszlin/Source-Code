using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace NEPS.OSP.COPPER.CABLE_TRANSFER_POST
{
    public partial class frmTransferPost : Form
    {
        clsTransferPost cTransfer = new clsTransferPost();
        private Logger log = Logger.getInstance();

        public frmTransferPost()
        {
            InitializeComponent();

            string filename = "TRANSFER"; 
            log = Logger.getInstance();
            log.OpenFile(filename);

        }
      
        private void btnTransfer_Click(object sender, EventArgs e)
        {
            if (cTransfer.isTransferSelected(GTTransferPost.m_gtapp.SelectedObjects))
            {
                try
                {
                    if (MessageBox.Show(this, "Are you sure to post selected transfer", "Post Transfer", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        InitProgressBar(50);
                        if (cTransfer.PerformPostTransfer(log))
                            MessageBox.Show(this, "Transfer already posted");
                        else
                            MessageBox.Show(this, "Transfer canceled with error");

                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, "Transfer canceled with error\r\n" + ex.Message);

                }
                finally
                {
                }
            }


        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        #region Progress Bar
        public void InitProgressBar(int val)
        {
            prgProcess.Maximum = val;
            prgProcess.Value = 0;
        }
        public void IncreaseProgressBar(int val)
        {
            if (val == prgProcess.Maximum)
                prgProcess.Value = prgProcess.Maximum;
            else if (prgProcess.Value + val > prgProcess.Maximum)
                prgProcess.Value = prgProcess.Maximum - 5;
            else
                prgProcess.Value += val;
            prgProcess.Visible = true;
            Application.DoEvents();
        }
        #endregion

        private void frmTransferPost_Load(object sender, EventArgs e)
        {
            string filename = Application.StartupPath + "/NEPS.OSP.COPPER.CABLE_TRANSFER_POST.dll";
            this.Text = "Complete Transfer [" + System.Diagnostics.FileVersionInfo.GetVersionInfo(filename).FileVersion + "]";

        }
    }
}