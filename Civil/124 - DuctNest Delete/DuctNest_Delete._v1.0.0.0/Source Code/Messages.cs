using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace NEPS.GTechnology.NEPSDuctNestDel
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
                lblMessage.Text = "Wait, deleting in process...";
            }
            if (step == 2)
            {
                lblMessage.Text = "Completed succefully!";
            }
            
        }
    }
}