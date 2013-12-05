using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace NEPS.GTechnology.NEPSCopyManhole
{
    public partial class Distance : Form
    {
        public Distance()
        {
            InitializeComponent();
        }

        private void rbNonPrec_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void rbPrec_CheckedChanged(object sender, EventArgs e)
        {
            if (rbPrec.Checked)
            {
                rbNonPrec.Checked = false;
                txtDistance.Enabled = true;
                GTCopyCivilManhole.Precision = true;
            }
            else 
            {
                rbNonPrec.Checked = true;
                txtDistance.Enabled = false;
                GTCopyCivilManhole.Precision = false;
            }
        }

        public void MessageHelpChange(int step)
        {
            if (step == 0)
            {
                lb1.Text = "Pnt> Select Source Manhole";
                lb2.Text = "Rst> Exit";
                gbMessage.Enabled = false;
            }
            if (step == 1)
            {
                lb1.Text = "Pnt> Confirm location";
                lb2.Text = "Rst> Exit";
                gbMessage.Enabled = true;
            }
            if (step == 2)
            {
                lb1.Text = "Pnt> Confirm rotation";
                lb2.Text = "Rst> Skip rotation";
                gbMessage.Enabled = false;
            }
            if (step == 3)
            {
                lb1.Text = "Double Click> Complete";
                lb2.Text = "Rst> New location";
                gbMessage.Enabled = false;
            }
           
        }

             
      
    }
}