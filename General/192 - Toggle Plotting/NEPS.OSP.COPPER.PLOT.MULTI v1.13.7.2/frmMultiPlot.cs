using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Intergraph.GTechnology.Interfaces;
using Intergraph.GTechnology.API;
using System.Diagnostics;
using System.Text.RegularExpressions;


namespace NEPS.OSP.COPPER.PLOT.MULTI
{
    public partial class frmMultiPlot : Form
    {
        public frmMultiPlot()
        {
            InitializeComponent();
        }

        private void frmMultiPlot_Load(object sender, EventArgs e)
        {
            IGTDataContext m_IGTDataContext = GTClassFactory.Create<IGTApplication>().Application.DataContext;
            string filename = Application.StartupPath + "/NEPS.OSP.COPPER.PLOT.MULTI.dll";
            this.Text = "Plotting Index [" + FileVersionInfo.GetVersionInfo(filename).FileVersion + "]";

            if (GTMultiPlot.m_iPlotTask == GTMultiPlot.PlotTask.init)
                LoadPlotControl(new usrPlotProperties());
        }


        private void frmMultiPlot_VisibleChanged(object sender, EventArgs e)
        {
            if (this.Visible)
            {
                if (GTMultiPlot.m_iPlotTask == GTMultiPlot.PlotTask.set_plot_index)
                    LoadPlotControl(new usrPlotList());
            }

        }

        private void LoadPlotControl(UserControl usrCtrl)
        {
            usrCtrl.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            usrCtrl.Dock = DockStyle.Fill;
            usrCtrl.BorderStyle = BorderStyle.None;
            this.Controls.Clear();
            this.Controls.Add(usrCtrl);
        }


    }
}