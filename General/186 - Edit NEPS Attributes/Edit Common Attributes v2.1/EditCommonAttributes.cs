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

namespace NEPS.GTechnology.EditCommonAttributes
{
    public partial class EditCommonAttributes : Form
    {       
        public static IGTApplication m_GeoApp;        
        private Logger log;
        IGTDDCKeyObjects oGTKeyObjs;
        //private IGTDataContext m_IGTDataContext = null;
        
        private Intergraph.GTechnology.API.IGTApplication m_gtapp = null; 

        public EditCommonAttributes()
        {
            try
            {
                InitializeComponent();               
                m_gtapp =  GTClassFactory.Create<IGTApplication>().Application;
                m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Running Edit Common Attributes...");

                log = Logger.getInstance();
               
            }
            catch (Exception ex)
            {
                log.WriteLog("");
                log.WriteLog(this.GetType().FullName, "ERROR: " + ex.Message);
                log.WriteLog(ex.StackTrace);
                MessageBox.Show(ex.Message, "Running Edit Common Attributes...", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }                      
        
        private void GTWindowsForm_EditCommonAttributes_Shown(object sender, EventArgs e)
        {
            m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Running Edit Common Attributes...");
        }

        private void GTWindowsForm_EditCommonAttributes_Activated(object sender, EventArgs e)
        {
            m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Running Edit Common Attributes...");
        }
        
        //Get Value from Database
        private string Get_Value(string sSql)
        {
            try
            {
                ADODB.Recordset rsPP = new ADODB.Recordset();
                rsPP = GTClassFactory.Create<IGTApplication>().DataContext.OpenRecordset(sSql, ADODB.CursorTypeEnum.adOpenStatic, ADODB.LockTypeEnum.adLockReadOnly, 1);
                if (rsPP.RecordCount > 0)
                {
                    rsPP.MoveFirst();
                    return (rsPP.Fields[0].Value.ToString());
                }
                return null;
            }
            catch (Exception ex)
            {
                log.WriteLog("");
                log.WriteLog(this.GetType().FullName, "ERROR: " + ex.Message);
                log.WriteLog(ex.StackTrace);
                MessageBox.Show(ex.Message, "Running Edit Common Attributes...", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }

        }
        
        private void GTWindowsForm_EditCommonAttributes_Load(object sender, EventArgs e)
        {
            LoadFeatureState_Year();
            txtJobID.Text = GTClassFactory.Create<IGTApplication>().DataContext.ActiveJob;
            textExcAbb.Text = Get_Value("SELECT EXC_ABB FROM G3E_JOB WHERE SCHEME_NAME = '" + GTClassFactory.Create<IGTApplication>().DataContext.ActiveJob + "'");
            txtJobState.Text = Get_Value("SELECT JOB_STATE FROM G3E_JOB WHERE SCHEME_NAME = '" + GTClassFactory.Create<IGTApplication>().DataContext.ActiveJob + "'");
            LoadSelected();
            
        }
      
        private void LoadFeatureState_Year()
        {
            try
            {
                int featCount = GTClassFactory.Create<IGTApplication>().SelectedObjects.FeatureCount;
                string FID = null;
                string featState = null;
                string yearInstalled = null;
                if (featCount == 1)
                {
                     foreach (IGTDDCKeyObject oDDCKeyObject in GTClassFactory.Create<IGTApplication>().SelectedObjects.GetObjects())
                    {                
                        FID = oDDCKeyObject.FID.ToString();
                    }
                    featState = Get_Value("SELECT FEATURE_STATE FROM GC_NETELEM WHERE G3E_FID = " + FID);
                    yearInstalled = Get_Value("SELECT YEAR_PLACED FROM GC_NETELEM WHERE G3E_FID = " + FID);
                    
                }
                if (yearInstalled == null || yearInstalled == "")
                {
                    yearInstalled = DateTime.Today.Year.ToString();
                    txtYear.Text = yearInstalled;
                }
                if (featCount == 0 || featState == null || featState == "")
                {
                    string jobState = Get_Value("SELECT TRIM(JOB_STATE) FROM G3E_JOB WHERE SCHEME_NAME = '" + GTClassFactory.Create<IGTApplication>().DataContext.ActiveJob + "'");
                    if (jobState == "PROPOSED")
                    {
                        cmbFeatState.Items.Add(new cboItem("PPF", "PPF"));
                        cmbFeatState.Items.Add(new cboItem("PAD", "PAD"));
                        cmbFeatState.Items.Add(new cboItem("PDR", "PDR"));
                    }
                    else if (jobState == "UN_CONSTRUCT")
                    {
                        cmbFeatState.Items.Add(new cboItem("PAD", "PAD"));
                        cmbFeatState.Items.Add(new cboItem("UC", "UC"));
                    }
                    else if (jobState == "COMPLETED")
                    {
                        cmbFeatState.Items.Add(new cboItem("UC", "UC"));
                        cmbFeatState.Items.Add(new cboItem("ASB", "ASB"));
                        cmbFeatState.Items.Add(new cboItem("MOD", "MOD"));
                    }
                    else if (jobState == "DEMOLISHED")
                    {
                        cmbFeatState.Items.Add(new cboItem("ASB", "ASB"));
                        cmbFeatState.Items.Add(new cboItem("PDR", "PDR"));
                    }
                    else if (jobState == "ABANDONED")
                    {
                        cmbFeatState.Items.Add(new cboItem("ASB", "ASB"));
                        cmbFeatState.Items.Add(new cboItem("FAB", "FAB"));
                    }
                    else if (jobState == "AMENDMENT")
                    {
                        cmbFeatState.Items.Add(new cboItem("ASB", "ASB"));
                        cmbFeatState.Items.Add(new cboItem("MOD", "MOD"));
                    }
                    else if (jobState == "RECOVERED")
                    {
                        cmbFeatState.Items.Add(new cboItem("ASB", "ASB"));
                        cmbFeatState.Items.Add(new cboItem("PRC", "PRC"));
                        cmbFeatState.Items.Add(new cboItem("DECOMM", "DECOMM"));
                    }
                }
                else
                {
                    cmbFeatState.Text = featState;
                    cmbFeatState.Enabled = false;
                    cmbFeatState.DroppedDown = false;

                    txtYear.Text = yearInstalled;
                    txtYear.Enabled = false;

                }
               
            }
            catch (Exception ex)
            {
                log.WriteLog("");
                log.WriteLog(this.GetType().FullName, "ERROR: " + ex.Message);
                log.WriteLog(ex.StackTrace);
                MessageBox.Show(ex.Message, "Edit Common Attributes", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadSelected()
        {
            //IGTGeometry geom = null;            
            string ViewName = string.Empty;
            string FIDs = null;

            if (GTClassFactory.Create<IGTApplication>().SelectedObjects.FeatureCount == 0)
            {
                MessageBox.Show("Please select a Feature/Features...", "Edit Common Attributes", MessageBoxButtons.OK, MessageBoxIcon.Information);
                //this.Close();
                btn_Update.Enabled = true;
                return;
            }

            foreach (IGTDDCKeyObject oDDCKeyObject in GTClassFactory.Create<IGTApplication>().SelectedObjects.GetObjects())
            {

                if (FIDs == null)
                    FIDs = oDDCKeyObject.FID.ToString();
                 else
                     FIDs = FIDs + "," + oDDCKeyObject.FID.ToString();

                 if (oDDCKeyObject.FNO == 5100 || oDDCKeyObject.FNO == 5600 || oDDCKeyObject.FNO == 6800 || oDDCKeyObject.FNO == 9900)//5100,5600,6800,9900
                 {
                     try
                     {

                         string owner_fid = Get_Value("select g3e_id from gc_ownership where g3e_fno = " + oDDCKeyObject.FNO + " and g3e_fid = " + oDDCKeyObject.FID);
                         IGTApplication app = GTClassFactory.Create<IGTApplication>();

                         List<int> output = new List<int>();
                         string sql = string.Format("SELECT * FROM gc_ownership where g3e_fno = 12300 and owner1_id = " + owner_fid);

                         int count = 0;
                         Recordset rs = app.DataContext.Execute(sql, out count, 0, null);
                         if (!rs.EOF)
                         {
                             rs.MoveFirst();
                             while (!rs.EOF)
                             {
                                 int fid = Convert.ToInt32(rs.Fields["G3E_FID"].Value);
                                 FIDs = FIDs + "," + fid.ToString();
                                 rs.MoveNext();
                             }
                         }
                     }
                     catch
                     {
                     }
                 }
                 else if (oDDCKeyObject.FNO == 9100 || oDDCKeyObject.FNO == 9600 || oDDCKeyObject.FNO == 9500 || oDDCKeyObject.FNO == 9800 || oDDCKeyObject.FNO == 5400 || oDDCKeyObject.FNO == 5200)//Card, Shelf, Slot
                 {
                     try
                     {
                         string owner_id = Get_Value("select g3e_id from gc_ownership where g3e_fno = " + oDDCKeyObject.FNO + " and g3e_fid = " + oDDCKeyObject.FID);
                         IGTApplication app = GTClassFactory.Create<IGTApplication>();
                         List<int> output = new List<int>();
                         string sql = string.Format("select * from gc_ownership where g3e_fno=15900 and owner1_id in (select g3e_id from gc_ownership where g3e_fno=12500 and owner1_id in (select g3e_id from gc_ownership where g3e_fno=15800 and owner1_id in(select g3e_id from gc_ownership where g3e_fno= " + oDDCKeyObject.FNO + " and g3e_id= " + owner_id + ")))UNION ALL select * from gc_ownership where g3e_fno=12500 and owner1_id in (select g3e_id from gc_ownership where g3e_fno=15800 and owner1_id in(select g3e_id from gc_ownership where g3e_fno=" + oDDCKeyObject.FNO + " and g3e_id=" + owner_id + "))UNION ALL select * from gc_ownership where g3e_fno=15800 and owner1_id in(select g3e_id from gc_ownership where g3e_fno=" + oDDCKeyObject.FNO + " and g3e_id=" + owner_id + ")");

                         int count = 0;
                         Recordset rs = app.DataContext.Execute(sql, out count, 0, null);
                         if (!rs.EOF)
                         {
                             rs.MoveFirst();
                             while (!rs.EOF)
                             {
                                 int fid = Convert.ToInt32(rs.Fields["G3E_FID"].Value);
                                 FIDs = FIDs + "," + fid.ToString();
                                 rs.MoveNext();
                             }
                         }
                     }
                     catch
                     {
                     }
                 }
            }
                Add_Device_Grid(FIDs);
            
        }

        private void Add_Device_Grid(string FIDs)
        {                            
            try
            {
                int iRow = 0;   
                ADODB.Recordset rsPP = new ADODB.Recordset();
                string sSql = "SELECT * from GC_NETELEM WHERE G3E_FID IN (" + FIDs + ")";
                rsPP = GTClassFactory.Create<IGTApplication>().DataContext.OpenRecordset(sSql, ADODB.CursorTypeEnum.adOpenStatic, ADODB.LockTypeEnum.adLockReadOnly, 1);
                if (rsPP.RecordCount > 0)
                {
                    rsPP.MoveFirst();
                    while (!rsPP.EOF)
                    {
                        iRow = dvFeat.Rows.Add();
                        dvFeat["G3E_FNO", iRow].Value = rsPP.Fields["G3E_FNO"].Value.ToString().TrimEnd();
                        dvFeat["Feat_Name", iRow].Value = Get_Value("SELECT G3E_USERNAME FROM G3E_FEATURE WHERE G3E_FNO = " + rsPP.Fields["G3E_FNO"].Value.ToString());
                        dvFeat["G3E_FID", iRow].Value = rsPP.Fields["G3E_FID"].Value.ToString();
                        dvFeat["JobState", iRow].Value = rsPP.Fields["JOB_STATE"].Value.ToString();
                        dvFeat["FeatureState", iRow].Value = rsPP.Fields["FEATURE_STATE"].Value.ToString();
                        rsPP.MoveNext();
                    }
                }

                dvFeat.ClearSelection();
            }
            catch (Exception ex)
            {
                log.WriteLog("");
                log.WriteLog(this.GetType().FullName, "ERROR: " + ex.Message);
                log.WriteLog(ex.StackTrace);
                MessageBox.Show(ex.Message, "Edit Common Attributes", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }            
        }

        private void dvFeat_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (dvFeat.SelectedRows.Count > 0)
            {
                Highlight();
            }
        }

        private void Highlight()
        {
            try
            {
                short mFNO = 0;
                int mFID = 0;

                if (dvFeat.Rows.Count > 0)
                {
                    GTClassFactory.Create<IGTApplication>().SelectedObjects.Clear();
                    foreach (DataGridViewRow row in dvFeat.Rows)
                    {
                        if (row.Selected == true)
                        {
                            mFID = int.Parse(row.Cells[2].Value.ToString());
                            mFNO = short.Parse(row.Cells[0].Value.ToString());

                            oGTKeyObjs = (IGTDDCKeyObjects)GTClassFactory.Create<IGTApplication>().DataContext.GetDDCKeyObjects(mFNO, mFID, GTComponentGeometryConstants.gtddcgAllGeographic);
                            for (int K = 0; K < oGTKeyObjs.Count; K++)
                                GTClassFactory.Create<IGTApplication>().SelectedObjects.Add(GTSelectModeConstants.gtsosmAllDisplayedComponentsInActiveLegend, oGTKeyObjs[K]);
                        }
                    }
                                       
                    GTClassFactory.Create<IGTApplication>().ActiveMapWindow.CenterSelectedObjects();
                    GTClassFactory.Create<IGTApplication>().ActiveMapWindow.DisplayScale = 500;
                    
                    GTClassFactory.Create<IGTApplication>().RefreshWindows();
                    oGTKeyObjs = null;
                }
            }
            catch (Exception ex)
            {
                log.WriteLog("");
                log.WriteLog(this.GetType().FullName, "ERROR: " + ex.Message);
                log.WriteLog(ex.StackTrace);
                MessageBox.Show(ex.Message, "Edit Common Attributes", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void Clear()
        {
            
        }

        private void btn_Close_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btn_Update_Click(object sender, EventArgs e)
        {


            progressBar1.Value = 0;
            progressBar1.Visible = true;
            for (int i = 0; i < 5; i++)
            {
                progressBar1.Value++;
            }

            int FID;
            short FNO;
            try
            {
                if (txtJobState.Text == "")
                {
                    MessageBox.Show("Please select a Job State", "Edit Common Attributes", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    progressBar1.Visible = false;
                    return;
                }

                /*if (cmbFeatState.Text == "")
                {
                    MessageBox.Show("Please select a Feature State", "Edit Common Attributes", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    progressBar1.Visible = false;
                    return;
                }*/

                if (txtJobID.Text == "")
                {
                    MessageBox.Show("Please enter the Job ID", "Edit Common Attributes", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    progressBar1.Visible = false;
                    return;
                }
                btn_Update.Enabled = false;
                btn_Close.Enabled = false;
                

                IGTKeyObject oEditFeature;
                GTCustomCommandModeless.m_oGTTransactionManager.Begin("EditAttributes");

                int iRow = 0;
                for (iRow = 0; iRow <= dvFeat.Rows.Count - 1; iRow++)
                {
                    FID = Convert.ToInt32(dvFeat["G3E_FID", iRow].Value);
                    FNO = Convert.ToInt16(dvFeat["G3E_FNO", iRow].Value);

                    string featState = Get_Value("SELECT FEATURE_STATE FROM GC_NETELEM WHERE G3E_FID = " + FID);
                    string yearInstalled = Get_Value("SELECT YEAR_PLACED FROM GC_NETELEM WHERE G3E_FID = " + FID);

                    oEditFeature = GTClassFactory.Create<IGTApplication>().DataContext.OpenFeature(FNO, FID);
                    // NETELEM 51
                    oEditFeature.Components.GetComponent(51).Recordset.Update("SCHEME_NAME", txtJobID.Text);
                    oEditFeature.Components.GetComponent(51).Recordset.Update("JOB_ID", txtJobID.Text);
                    oEditFeature.Components.GetComponent(51).Recordset.Update("EXC_ABB", textExcAbb.Text);
                    oEditFeature.Components.GetComponent(51).Recordset.Update("JOB_STATE", txtJobState.Text);
                    oEditFeature.Components.GetComponent(51).Recordset.Update("OWNERSHIP", txtOwnership.Text);
                    if (featState == null || featState == "")
                    {
                        oEditFeature.Components.GetComponent(51).Recordset.Update("FEATURE_STATE", cmbFeatState.Text);
                    }
                    if (yearInstalled == null || yearInstalled == "")
                    {
                        oEditFeature.Components.GetComponent(51).Recordset.Update("YEAR_PLACED", txtYear.Text);
                    }
                    if (progressBar1.Value != 70)
                    {
                        progressBar1.Value++;
                    }
                }
               
                GTCustomCommandModeless.m_oGTTransactionManager.Commit();
                GTCustomCommandModeless.m_oGTTransactionManager.RefreshDatabaseChanges();

                for (int j = progressBar1.Value; j < 100; j++)
                {
                    progressBar1.Value++;
                }
                //progressBar1.Visible = false;
                MessageBox.Show("Attributes Updated.", "EditCommonAttributes", MessageBoxButtons.OK, MessageBoxIcon.Information );
                this.Close();
            }
            catch (Exception ex)
            {
                GTCustomCommandModeless.m_oGTTransactionManager.Rollback();
                log.WriteLog("");
                log.WriteLog(this.GetType().FullName, "ERROR: " + ex.Message);
                log.WriteLog(ex.StackTrace);
                MessageBox.Show(ex.Message, "Edit Common Attributes", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            LoadSelected();
        }

        private void button2_Click(object sender, EventArgs e)
        {


            progressBar1.Value = 0;
            progressBar1.Visible = true;
            for (int i = 0; i < 5; i++)
            {
                progressBar1.Value++;
            }

            int FID;
            short FNO;
            try
            {
                if (txtJobState.Text == "")
                {
                    MessageBox.Show("Please select a Job State", "Edit Common Attributes", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    progressBar1.Visible = false;
                    return;
                }

                /*if (cmbFeatState.Text == "")
                {
                    MessageBox.Show("Please select a Feature State", "Edit Common Attributes", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    progressBar1.Visible = false;
                    return;
                }*/

                if (txtJobID.Text == "")
                {
                    MessageBox.Show("Please enter the Job ID", "Edit Common Attributes", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    progressBar1.Visible = false;
                    return;
                }
                btn_Update.Enabled = false;
                btn_Close.Enabled = false;


                IGTKeyObject oEditFeature;
                GTCustomCommandModeless.m_oGTTransactionManager.Begin("EditAttributes");

                int iRow = 0;
                for (iRow = 0; iRow <= dvFeat.Rows.Count - 1; iRow++)
                {
                    FID = Convert.ToInt32(dvFeat["G3E_FID", iRow].Value);
                    FNO = Convert.ToInt16(dvFeat["G3E_FNO", iRow].Value);

                    string featState = Get_Value("SELECT FEATURE_STATE FROM GC_NETELEM WHERE G3E_FID = " + FID);
                    string yearInstalled = Get_Value("SELECT YEAR_PLACED FROM GC_NETELEM WHERE G3E_FID = " + FID);

                    oEditFeature = GTClassFactory.Create<IGTApplication>().DataContext.OpenFeature(FNO, FID);
                    // NETELEM 51
                    //oEditFeature.Components.GetComponent(51).Recordset.Update("SCHEME_NAME", txtJobID.Text);
                    //oEditFeature.Components.GetComponent(51).Recordset.Update("JOB_ID", txtJobID.Text);
                    oEditFeature.Components.GetComponent(51).Recordset.Update("EXC_ABB", textExcAbb.Text);
                    //oEditFeature.Components.GetComponent(51).Recordset.Update("JOB_STATE", txtJobState.Text);
                    oEditFeature.Components.GetComponent(51).Recordset.Update("OWNERSHIP", txtOwnership.Text);
                    if (featState == null || featState == "")
                    {
                        oEditFeature.Components.GetComponent(51).Recordset.Update("FEATURE_STATE", cmbFeatState.Text);
                    }
                    if (yearInstalled == null || yearInstalled == "")
                    {
                        oEditFeature.Components.GetComponent(51).Recordset.Update("YEAR_PLACED", txtYear.Text);
                    }
                    if (progressBar1.Value != 70)
                    {
                        progressBar1.Value++;
                    }
                }

                GTCustomCommandModeless.m_oGTTransactionManager.Commit();
                GTCustomCommandModeless.m_oGTTransactionManager.RefreshDatabaseChanges();

                for (int j = progressBar1.Value; j < 100; j++)
                {
                    progressBar1.Value++;
                }
                //progressBar1.Visible = false;
                MessageBox.Show("Attributes Updated.", "EditCommonAttributes", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close();
            }
            catch (Exception ex)
            {
                GTCustomCommandModeless.m_oGTTransactionManager.Rollback();
                log.WriteLog("");
                log.WriteLog(this.GetType().FullName, "ERROR: " + ex.Message);
                log.WriteLog(ex.StackTrace);
                MessageBox.Show(ex.Message, "Edit Common Attributes", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            
        }

        

    }
}