using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace AG.GTechnology.ManholeInsert
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
                lblMessage.Text = "Wait, loading Duct Path information...";
            }           
            if (step == 2)
            {
                lblMessage.Text = "Wait, placing new Manhole in process...";
            }
            if (step == 3)
            {
                lblMessage.Text = "Wait, breaking Duct Path in process...";
            }
        }
    }
}