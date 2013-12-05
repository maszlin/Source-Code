using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ADODB;
using Intergraph.GTechnology.Interfaces;
using Intergraph.GTechnology.API;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace NEPS.GTechnology.NEPSDuctPathExpansion
{
    public partial class DPExpansionForm : Form
    {
        #region var

        private int CloseStatus = 0;

        #endregion
       
        #region init and load form
        public DPExpansionForm()
        {
            try
            {
                InitializeComponent();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Duct Path Expansion", MessageBoxButtons.OK, MessageBoxIcon.Error);
                CloseStatus = 1;
                this.Close();
            }
        }
        private void DPExpansionFrom_Load(object sender, EventArgs e)
        {
            FillingOriginalAttr();
        }
        #region filling first page
        private void FillingOriginalAttr()
        {
            txtDuctPathFID.Text = GTDuctPathExpansion.DuctPathOrigin.FID.ToString();
            txtNumDuctWays.Text = GTDuctPathExpansion.DuctPathOrigin.DuctWay.ToString();
            txtPPNumDuctWays.Enabled = true;
            txtPPNumDuctWays.Text = "0";
            if (GTDuctPathExpansion.DuctPathOrigin.Feature_state == "MOD" && GTDuctPathExpansion.DuctPathOrigin.PPDuctWay != 0)
            {
                btnExpand.Text = "Cancel Expand";
                txtPPNumDuctWays.Enabled = false;
                txtPPNumDuctWays.Text = GTDuctPathExpansion.DuctPathOrigin.PPDuctWay.ToString();
            }
           
        }
        #endregion
        #endregion

        #region Closing Form
        private void DPExpansionFrom_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (CloseStatus == 0)
            {
                DialogResult retVal = MessageBox.Show("Are you sure that you want to exit?", "Duct Path Expansion", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (retVal == DialogResult.Yes)
                {
                    e.Cancel = false;
                }
                else { e.Cancel = true; }
            }
            else
            {
                e.Cancel = false;
            }
        }
        #endregion
        #region Button close2 click
        private void btnClose2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        #endregion 

        #region Expand Duct Path
        private void btnExpand_Click(object sender, EventArgs e)
        {

            txtPPNumDuctWays.Enabled = false;
            if (btnExpand.Text == "Expand")
            {
                if (!(GTDuctPathExpansion.DuctPathOrigin.Feature_state == "ASB"))
                {
                    MessageBox.Show("Expansion is only avaliable for ASB Duct Path", "Duct Path Expansion", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                  {
                    int sectLen=0;
                    if (!(int.TryParse(txtPPNumDuctWays.Text, out sectLen)))
                    {
                        MessageBox.Show("Addition Number of Duct Ways should be integer number!", "Duct Path Expansion", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                    else
                    {
                        if (sectLen<=0)
                        {
                            MessageBox.Show("Addition Number of Duct Ways should be greater then 0!", "Duct Path Expansion", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                        }
                        else
                        {
                            progressBar1.Visible = true;
                            progressBar1.Value = 10;
                            if (ExpandDuctPath())
                            {
                                GTDuctPathExpansion.m_gtapp.RefreshWindows();
                                progressBar1.Value = 100;
                                MessageBox.Show("Expansion is successed", "Duct Path Expansion", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                btnExpand.Text = "Cancel Expand";
                                txtPPNumDuctWays.Enabled = false;
                                progressBar1.Visible = false;
                            }
                            else
                            {
                                CloseStatus = 1;
                                this.Close();
                            }
                        }

                    }
                }
            }
            else if (btnExpand.Text == "Cancel Expand")
            {
                progressBar1.Visible = true;
                progressBar1.Value = 10;
                if (ExpandDuctPath())
                {
                    GTDuctPathExpansion.m_gtapp.RefreshWindows();
                    progressBar1.Value = 100;
                    MessageBox.Show("Cancellation of Expansion is successed", "Duct Path Expansion", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    btnExpand.Text = "Expand";
                    txtPPNumDuctWays.Text = "0";
                    txtPPNumDuctWays.Enabled = true;
                    progressBar1.Visible = false;
                }
                else
                {
                    CloseStatus = 1;
                    this.Close();
                }
            }
        }
        #region Get Value from Database
        private string Get_Value(string sSql)
        {
            try
            {
                ADODB.Recordset rsPP = new ADODB.Recordset();
                rsPP = GTDuctPathExpansion.m_IGTDataContext.OpenRecordset(sSql, ADODB.CursorTypeEnum.adOpenStatic, ADODB.LockTypeEnum.adLockReadOnly, 1);
                if (rsPP.RecordCount > 0)
                {
                    rsPP.MoveFirst();
                    return (rsPP.Fields[0].Value.ToString());
                }
                return "";
            }
            catch (Exception ex)
            {
                return "";
            }

        }
        #endregion
        private bool ExpandDuctPath()
        {
            if (btnExpand.Text == "Expand")
            {
                try
                {
                    int ExpandCount = int.Parse(txtPPNumDuctWays.Text);
                    GTDuctPathExpansion.m_oIGTTransactionManager.Begin("Expand Duct Path");
                    IGTKeyObject oDuctPathFeature = GTDuctPathExpansion.m_IGTDataContext.OpenFeature(GTDuctPathExpansion.DuctPathOrigin.FNO, GTDuctPathExpansion.DuctPathOrigin.FID);
                    progressBar1.Value = 30;
                    if (!oDuctPathFeature.Components.GetComponent(51).Recordset.EOF)
                    {
                        oDuctPathFeature.Components.GetComponent(51).Recordset.MoveLast();
                        oDuctPathFeature.Components.GetComponent(51).Recordset.Update("FEATURE_STATE", "MOD");
                        oDuctPathFeature.Components.GetComponent(51).Recordset.Update("JOB_ID", GTDuctPathExpansion.m_IGTDataContext.ActiveJob);
                        oDuctPathFeature.Components.GetComponent(51).Recordset.Update("JOB_STATE", Get_Value("SELECT JOB_STATE FROM G3E_JOB WHERE G3E_IDENTIFIER ='"+GTDuctPathExpansion.m_IGTDataContext.ActiveJob+"'" ));
                    }
                    progressBar1.Value = 40;
                    if (!oDuctPathFeature.Components.GetComponent(2202).Recordset.EOF)
                    {
                        oDuctPathFeature.Components.GetComponent(2202).Recordset.MoveFirst();
                        for (int j = 0; j < oDuctPathFeature.Components.GetComponent(2202).Recordset.RecordCount; j++)
                        {
                            oDuctPathFeature.Components.GetComponent(2202).Recordset.Update("DT_S_PP_WAYS", ExpandCount);
                            oDuctPathFeature.Components.GetComponent(2202).Recordset.Update("YEAR_EXPANDED", new DateTime(DateTime.Now.Year, 1, 1));
                            oDuctPathFeature.Components.GetComponent(2202).Recordset.MoveNext();
                        }
                    }
                    progressBar1.Value = 50;
                    GTDuctPathExpansion.m_oIGTTransactionManager.Commit();
                    progressBar1.Value = 70;
                    GTDuctPathExpansion.m_oIGTTransactionManager.RefreshDatabaseChanges();
                    progressBar1.Value = 80;
                    return true;
                }
                catch (Exception ex)
                {
                    if (GTDuctPathExpansion.m_oIGTTransactionManager.TransactionInProgress)
                        GTDuctPathExpansion.m_oIGTTransactionManager.Rollback();
                    MessageBox.Show(ex.Message, "Duct Path Expansion", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    CloseStatus = 1;
                    this.Close();
                }
            }
            else if (btnExpand.Text == "Cancel Expand")
            {
                try
                {
                    
                    int ExpandCount = int.Parse(txtNumDuctWays.Text);
                    GTDuctPathExpansion.m_oIGTTransactionManager.Begin("Cancel Expand Duct Path");
                    IGTKeyObject oDuctPathFeature = GTDuctPathExpansion.m_IGTDataContext.OpenFeature(GTDuctPathExpansion.DuctPathOrigin.FNO, GTDuctPathExpansion.DuctPathOrigin.FID);
                    progressBar1.Value = 30;
                    if (!oDuctPathFeature.Components.GetComponent(51).Recordset.EOF)
                    {
                        oDuctPathFeature.Components.GetComponent(51).Recordset.MoveLast();
                        oDuctPathFeature.Components.GetComponent(51).Recordset.Update("FEATURE_STATE", "ASB");
                    }
                    progressBar1.Value = 40;
                    if (!oDuctPathFeature.Components.GetComponent(2202).Recordset.EOF)
                    {
                        oDuctPathFeature.Components.GetComponent(2202).Recordset.MoveFirst();
                        for (int j = 0; j < oDuctPathFeature.Components.GetComponent(2202).Recordset.RecordCount; j++)
                        {
                            oDuctPathFeature.Components.GetComponent(2202).Recordset.Update("DT_S_PP_WAYS", 0);
                            oDuctPathFeature.Components.GetComponent(2202).Recordset.Update("YEAR_EXPANDED", DBNull.Value);
                            oDuctPathFeature.Components.GetComponent(2202).Recordset.MoveNext();
                        } 
                    }
                    progressBar1.Value = 50;
                    GTDuctPathExpansion.m_oIGTTransactionManager.Commit();
                    progressBar1.Value = 70;
                    GTDuctPathExpansion.m_oIGTTransactionManager.RefreshDatabaseChanges();
                    progressBar1.Value = 80;
                    return true;
                }
                catch (Exception ex)
                {
                    if (GTDuctPathExpansion.m_oIGTTransactionManager.TransactionInProgress)
                        GTDuctPathExpansion.m_oIGTTransactionManager.Rollback();
                    MessageBox.Show(ex.Message, "Duct Path Expansion", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    CloseStatus = 1;
                    this.Close();
                }
            }
            return false;
        }
        #endregion

        
        
    }
}