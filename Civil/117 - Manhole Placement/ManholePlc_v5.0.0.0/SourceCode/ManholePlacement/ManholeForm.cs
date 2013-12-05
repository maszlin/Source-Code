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

namespace AG.GTechnology.ManholePlacement.Forms
{
    public partial class ManholeForm : Form
    {
        private Intergraph.GTechnology.API.IGTApplication application;
        private Logger log;

        public ManholeForm(IGTCustomCommandHelper customCommandHelper)
        {
            InitializeComponent();
            application = GTClassFactory.Create<IGTApplication>();

            log = Logger.getInstance();

            Shown += new EventHandler(SubDuctForm_Shown);
        }

        private double _distance = 1;
        public double Distance
        {
            set { _distance = value; }
            get {
                try
                {
                    _distance = Convert.ToDouble(txtDistance.Text);
                }
                catch { }
                return _distance; 
            }
        }

        public bool PlaceType
        {
            get
            {
                return radioPlaceType1.Checked;
            }
        }

        /// <summary>
        /// Sets the focus to the valueTextBox when this form is shown.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SubDuctForm_Shown(object sender, EventArgs e)
        {
            application.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Select a manhole placement type.");
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

        private void radioPlaceType1_CheckedChanged(object sender, EventArgs e)
        {
            txtDistance.Enabled = radioPlaceType1.Checked;
            label1.Enabled = radioPlaceType1.Checked;
        }

        
    }
}