using ADODB;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Intergraph.GTechnology.API;
using System.Windows.Forms.DataVisualization.Charting;

namespace AG.GTechnology.DuctSpaceReport.Forms
{
    public partial class SubDuctForm : Form
    {
        private Intergraph.GTechnology.API.IGTApplication application;
        private Logger log;

        public SubDuctForm(IGTCustomCommandHelper customCommandHelper)
        {
            InitializeComponent();
            application = GTClassFactory.Create<IGTApplication>();

            log = Logger.getInstance();

            Shown += new EventHandler(SubDuctForm_Shown);
        }

        private int _nducts = 3;
        public int NumberOfSubduct
        {
            set { _nducts = value; }
            get { return _nducts; }
        }

        /// <summary>
        /// Sets the focus to the valueTextBox when this form is shown.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SubDuctForm_Shown(object sender, EventArgs e)
        {
            application.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Select a number of subduct/innerduct.");
        }

        #region Getters/Setters

        public Logger Logger
        {
            set
            { log = value; }
        }

        #endregion

        public new event EventHandler PlaceClick
        {
            add
            {
                this.btnPlace.Click += value;
            }
            remove
            {
                this.btnPlace.Click -= value;
            }
        }

        private void nbrDucts_ValueChanged(object sender, EventArgs e)
        {
            _nducts = (int)nbrDucts.Value;
        }

    }
}