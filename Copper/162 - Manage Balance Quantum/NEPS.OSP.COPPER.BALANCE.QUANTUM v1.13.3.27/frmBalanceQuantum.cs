using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ADODB;
using System.IO;
using Intergraph.GTechnology.Interfaces;
using Intergraph.GTechnology.API;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace NEPS.OSP.COPPER.BALANCE.QUANTUM
{
    public partial class frmBalanceQuantum : Form
    {
        private IGTApplication m_gtapp = null;
        private IGTDataContext m_IGTDataContext = null;
        private clsBalance Cable;
        private bool isBalance = false;

        #region Form Cable Code
        public frmBalanceQuantum()
        {
            try
            {
                InitializeComponent();
                this.FormClosing += new FormClosingEventHandler(this.frmFrame_FormClosing);
                m_gtapp = GTClassFactory.Create<IGTApplication>().Application;
                m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Running Exchange Frame Management System...");
                Cable = new clsBalance();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Running Exchange Frame Management System", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void frmFrame_Shown(object sender, EventArgs e)
        {
            m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Running Exchange Frame Management System...");
        }

        private void frmFrame_Activated(object sender, EventArgs e)
        {
            m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Running Exchange Frame Management System...");
        }

        private void frmFrame_Load(object sender, EventArgs e)
        {
            this.m_IGTDataContext = GTClassFactory.Create<IGTApplication>().Application.DataContext;
        }

        private void frmFrame_FormClosing(object sender, FormClosingEventArgs e)
        {
            //DialogResult retVal = MessageBox.Show("Are you sure to exit?", "Exchange Frame Management System", MessageBoxButtons.YesNo);
            //if (retVal == DialogResult.Yes)
            //{
            //    e.Cancel = false;
            //}
            //else
            //{
            //    e.Cancel = true;
            //}
        }
        #endregion

        #region Trace Balance

        private void btnPick_Click(object sender, EventArgs e)
        {
            try
            {
                if (Cable.isCableSelected(m_gtapp.SelectedObjects))
                {
                    lblEXC.Text = "EXC : " + Cable.EXC_ABB;
                    lblITFACE.Text = "CAB : " +
                        (Cable.ITFACE_CODE.Length > 0 ? Cable.ITFACE_CODE : Cable.RT_CODE);
                    lblCableCode.Text = "CABLE CODE : " + Cable.CABLE_CODE;
                    lblCableType.Text = "CABLE TYPE : " + Cable.CABLE_CLASS;
                    lblMsg.Text = "";

                    TraceBalanceQuantum(true);

                    isBalance = Cable.EffectiveTree(this.trvNetwork);
                    if (isBalance)
                        MessageBox.Show(this,"This cable network is already balance","Balance Quantum");
                    else
                        MessageBox.Show(this, "This cable network is not balance\r\n" +
                            "Open propose page to see propose setting", "Balance Quantum");
    
                }
                else
                {
                    lblMsg.Text = ">>> select cable and click [Read Effective Pairs]";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, "Error reading balance quantum\r\n" + ex.Message, "Balance Quantum");
            }
        }

        private void TraceBalanceQuantum(bool trace_new)
        {
            InitProgressBar(100);

            if (Cable.CABLE_CLASS.IndexOf("D") > -1)
            {
                lblITFACE.Text = "CAB : " + Cable.ITFACE_CODE;
                Application.DoEvents();
                Cable.BalanceQuantum_DSIDE();

            }
            else
            {
                lblITFACE.Text = "";
                Application.DoEvents();
                Cable.BalanceQuantum_ESIDE();
            }

            IncreaseProgressBar(100);           
        }

        #endregion       

        #region Close
        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        #endregion

        #region Hilite Feature
       
        private void hiliteFeature(int iFID, short iFNO)
        {
            try
            {
                IGTDDCKeyObjects oGTKeyObjs;
                GTBalanceQuantum.m_gtapp.SelectedObjects.Clear();

                oGTKeyObjs = (IGTDDCKeyObjects)GTBalanceQuantum.m_gtapp.DataContext.GetDDCKeyObjects(iFNO, iFID, GTComponentGeometryConstants.gtddcgAllGeographic);
                for (int i = 0; i < oGTKeyObjs.Count; i++)
                {
                    GTBalanceQuantum.m_gtapp.SelectedObjects.Add(GTSelectModeConstants.gtsosmAllDisplayedComponentsInActiveLegend, oGTKeyObjs[i]);
                }
                GTBalanceQuantum.m_gtapp.ActiveMapWindow.CenterSelectedObjects();
                GTBalanceQuantum.m_gtapp.ActiveMapWindow.DisplayScale = 400;
                GTBalanceQuantum.m_gtapp.RefreshWindows();
                oGTKeyObjs = null;

            }
            catch (Exception ex)
            {
            }
        }
        #endregion

        #region Progress Bar
        public void InitProgressBar(int val)
        {
            progressBar1.Maximum = val;
            progressBar1.Value = 0;
        }
        public void IncreaseProgressBar(int val)
        {
            if (val == progressBar1.Maximum)
                progressBar1.Value = progressBar1.Maximum;
            else if (progressBar1.Value + val > progressBar1.Maximum)
                progressBar1.Value = (progressBar1.Maximum / 2);
            else
                progressBar1.Value += val;
            progressBar1.Visible = true;
            Application.DoEvents();
        }
        #endregion

  
        private void trvNetwork_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node != null)
            {
                TreeNode nd = e.Node;
                try
                {
                    if (nd.Nodes.Count > 0)
                    {
                        hiliteFeature(int.Parse(nd.Name), 7000);
                        if ((TreeView)sender == trvNew)
                            CableDetail_Propose(nd);
                    }
                    else
                    {
                        string sFNO = nd.ToString();
                        sFNO = sFNO.Substring(sFNO.IndexOf('[') + 1);
                        sFNO = sFNO.Remove(sFNO.IndexOf(']'));
                        hiliteFeature(int.Parse(nd.Name), short.Parse(sFNO));
                    }
                }
                catch { }
            }
        }

        private void CableDetail_Propose(TreeNode nd)
        {
            if (nd.Nodes.Count > 0)
            {
                int iFID = int.Parse(nd.Name);
                clsBalance.CablePairs c = Cable.cable(iFID);
                txtFID2.Text = iFID.ToString();
                txtCblSize2.Text = c.iTotalPairs.ToString();
                txtEffective2.Text = c.iEffectivePairs.ToString();
                txtNew2.Text = c.newEffectivePairs.ToString();
                txtDownstream2.Text = c.childEffectivePairs.ToString();
            }
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabControl1.SelectedIndex == 1)
            {
                bool cancel = true;
                if (trvNetwork.Nodes.Count == 0)
                    MessageBox.Show(this, "You have to select a cable and click [Read Effective Pairs]\r\nbefore opening the propose page", "Balance Quantum");
                else if (isBalance)
                    MessageBox.Show(this, "Selected network already balance\r\nAuto balancing not required", "Balance Quantum");
                else
                    cancel = false;

                if (cancel)
                    tabControl1.SelectedIndex = 0;
                else
                    AutoBalancing();
            }
        }

        #region "Auto Balancing"
        private void AutoBalancing()
        {
            clsAutoBalance a = new clsAutoBalance();
            btnApply.Enabled = a.AutoBalanceTree(trvNetwork, trvNew, Cable);
            if (!btnApply.Enabled)
                MessageBox.Show(this,"Conflict detected, unable to perform auto balancing\r\n" + 
                    "Propose effective pairs bigger than cable size","Balance Quantum");
            Application.DoEvents();
        }

        private void btnApply_Click(object sender, EventArgs e)
        {
            GTBalanceQuantum.m_gtapp.BeginWaitCursor();
            GTBalanceQuantum.m_oIGTTransactionManager.Begin("Update Effective Pairs");
            try
            {
                clsSaveBalance.SaveBalance(trvNew, Cable);

                GTBalanceQuantum.m_oIGTTransactionManager.Commit();
                GTBalanceQuantum.m_oIGTTransactionManager.RefreshDatabaseChanges();
                MessageBox.Show(this, "Setting already applied", "Balance Quantum");
            }
            catch (Exception ex)
            {
                GTBalanceQuantum.m_oIGTTransactionManager.Rollback();
                MessageBox.Show(this,"Fail to apply setting\r\n" + ex.Message,"Balance Quantum");
            }
            finally
            {
                GTBalanceQuantum.m_gtapp.EndWaitCursor();
            }
        }
        #endregion

        private void txtBalance_Enter(object sender, EventArgs e)
        {
            this.tabControl1.Focus();
        }
    }
}