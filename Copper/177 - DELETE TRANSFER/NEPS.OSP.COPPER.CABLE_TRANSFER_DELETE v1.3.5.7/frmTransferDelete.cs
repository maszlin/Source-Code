using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace NEPS.OSP.COPPER.CABLE_TRANSFER_DELETE
{    
    public partial class frmTransferDelete : Form
    {
        clsTransferDelete cTransfer = new clsTransferDelete();
        public frmTransferDelete()
        {
            InitializeComponent();
        }

        private void frmTransferDelete_Load(object sender, EventArgs e)
        {
            string filename = Application.StartupPath + "/NEPS.OSP.COPPER.CABLE_TRANSFER_DELETE.dll";
            this.Text = "Cancel Transfer [" + System.Diagnostics.FileVersionInfo.GetVersionInfo(filename).FileVersion + "]";
        }

        private void btnTransfer_Click(object sender, EventArgs e)
        {
            if (cTransfer.isTransferSelected(GTTransferDelete.m_gtapp.SelectedObjects))
            {
                try
                {
                    this.TopMost = false;
                    if (MessageBox.Show("Are you sure to cancel selected transfer\r\nCancel will delete transfer and\r\nrollback related feature to original state", 
                        "Cancel Transfer", MessageBoxButtons.YesNo) == DialogResult.Yes)
                        cTransfer.PerformDeleteTransfer();
                }
                catch (Exception exception)
                { }
                finally
                {
                    this.TopMost = true;
                }
            }
            

        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

    }
}