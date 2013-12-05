using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace NEPS.GTechnology.NEPSCopyManhole
{
    public partial class Messages : Form
    {
        public Messages()
        {
            InitializeComponent();
        }
        public void Message(int step)
        {
            if (step == 1)
            {
                lblMessage.Text = "Wait, copying in process...";
            }           
            if (step == 3)
            {
                lblMessage.Text = "Completed succefully!";
            }
            
        }
    }
}