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

namespace AG.GTechnology.InsertFiberSplitter
{
    public partial class GTWindowsForm_InsertSplitter : Form
    {
        private Intergraph.GTechnology.API.IGTApplication m_gtapp = null;
        private IGTDataContext m_GTDataContext = null;
        public short OwnerFNO=0;
        public int OwnerFID=0;


        #region loading form
        public GTWindowsForm_InsertSplitter()
        {
            try
            {
                InitializeComponent();
                m_gtapp = GTInsertFiberSplitter.application;
                m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Running Insert Splitter(s)..");

              
            }
            catch (Exception ex)
            {        
                MessageBox.Show(ex.Message, "Add Splitter(s)", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }        
        
        private void GTWindowsForm_PlaceFDC_Shown(object sender, EventArgs e)
        {
            m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Running Place FDC...");            
        }
     
        private void GTWindowsForm_PlaceFDC_Load(object sender, EventArgs e)
        {

           
        }

        private string GetComboValue(ComboBox cboList, string SelectValue)
        {
            for (int i = 0; i < cboList.Items.Count; i++)
            {
                if (SelectValue == ((cboItem)cboList.Items[i]).Name)
                {
                    return ((cboItem)cboList.Items[i]).Value;
                }
            }
            return "";
        }
        

        #endregion

        #region button generate click

        private void btn_Generate_Click(object sender, EventArgs e)
        {
            if (cmbSplitterType.Text == "")
            {
                MessageBox.Show("Please Select a Service Splitter Type", "Add Splitter(s)", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if(gbFOMSSp.Enabled)
                if (cmbSplitterTypeFOMS.Text == "")
                {
                MessageBox.Show("Please Select a FOMS Splitter Type", "Add Splitter(s)", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
                }
            
            if (txt_NoSplitter.Text == "" )
            {
                MessageBox.Show("Please Enter correct No of SERVICE Splitter", "Add Splitter(s)", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (txt_NoSplitterFOMS.Text == "")
            {
                MessageBox.Show("Please Enter correct No of FOMS Splitter", "Add Splitter(s)", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            int try_no = 0;
            if (!int.TryParse(txt_NoSplitter.Text,out try_no))
            {
                MessageBox.Show("Please Enter correct No of SERVICE Splitter", "Add Splitter(s)", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (!int.TryParse(txt_NoSplitterFOMS.Text, out try_no))
            {
                MessageBox.Show("Please Enter correct No of FOMS Splitter", "Add Splitter(s)", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            AddSplitter();
       }
        #endregion
      
        #region Add Splitters
        public void AddSplitter()
        {
            try
            {
                m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, " Wait, updating database changes...");
                progressBar1.Visible = true;
                progressBar1.Value = 10;
                btn_Generate.Enabled = false;
                btnSelectOwner.Enabled = false;
                int count = 75 / (Convert.ToInt32(txt_NoSplitter.Text) + Convert.ToInt32(txt_NoSplitterFOMS.Text));
                
                int iSplitFID = 0;
                GTInsertFiberSplitter.m_oGTTransactionManager.Begin("Add Splitters");
                IGTKeyObject oNewSplitter1;
                IGTKeyObject mobjOwnerAttribute = m_gtapp.DataContext.OpenFeature(OwnerFNO, OwnerFID);

                #region SPLITTER/OWNERSHIP
                if (cmbSplitterType.Text != "")
                {
                  //  GTCustomCommandModeless.m_oGTTransactionManager.Begin("DrawSplitter");

                    string MIN_MATERIAL = null;
                    string SPLITTER_TYPE = null;
                    string SPLITTER_CLASS = null;
                    string tCode = null;
                    int SPlit_num = int.Parse(Get_Value("select count(*) from gc_ownership where g3e_fno=12300 and owner1_id in " +
                                  "(select g3e_id from gc_ownership where g3e_fid="+ OwnerFID.ToString()+")"));
                    
                    string SPLITTER_CODE = "";

                    #region SPLITTER SERVICE
                    for (int i = 1; i <= Convert.ToInt32(txt_NoSplitter.Text); i++)
                    {
                        int recordsAffected = 0;
                        this.m_GTDataContext = m_gtapp.DataContext;
                        string get_splitter = "select * from REF_FSPLITTER where SPLITTER_TYPE = '" + cmbSplitterType.Text + "'";
                        Recordset rsComp = m_GTDataContext.Execute(get_splitter, out recordsAffected, (int)CommandTypeEnum.adCmdText, null);
                        if (rsComp != null && rsComp.RecordCount > 0)
                        {
                            rsComp.MoveFirst();
                            MIN_MATERIAL = rsComp.Fields["MIN_MATERIAL"].Value.ToString();
                            SPLITTER_TYPE = rsComp.Fields["SPLITTER_TYPE"].Value.ToString();
                            SPLITTER_CLASS = "NORMAL";
                        }
                        rsComp = null;

                        oNewSplitter1 = m_gtapp.DataContext.NewFeature(12300);
                        iSplitFID = oNewSplitter1.FID;

                        // NETELEM 51
                        oNewSplitter1.Components.GetComponent(51).Recordset.Update("JOB_STATE", "PROPOSED");
                        oNewSplitter1.Components.GetComponent(51).Recordset.Update("FEATURE_STATE", "PPF");
                        oNewSplitter1.Components.GetComponent(51).Recordset.Update("MIN_MATERIAL", MIN_MATERIAL);

                        //ATTRIBUTE

                        SPlit_num++;
                        if (SPlit_num.ToString().Length == 1)
                            tCode = "0" + SPlit_num.ToString();
                        else
                            tCode = SPlit_num.ToString();

                        if (txtOLTID.Visible)
                            SPLITTER_CODE = txtOLTID.Text + "-" + tCode;
                        else SPLITTER_CODE = "SP" + tCode;

                        if (oNewSplitter1.Components.GetComponent(12301).Recordset.EOF)
                        {
                            oNewSplitter1.Components.GetComponent(12301).Recordset.AddNew("G3E_FID", iSplitFID);
                            oNewSplitter1.Components.GetComponent(12301).Recordset.Update("SPLITTER_TYPE", SPLITTER_TYPE);
                            oNewSplitter1.Components.GetComponent(12301).Recordset.Update("SPLITTER_CLASS", SPLITTER_CLASS);
                            oNewSplitter1.Components.GetComponent(12301).Recordset.Update("NO_OF_SPLITTER", SPlit_num);
                            oNewSplitter1.Components.GetComponent(12301).Recordset.Update("SPLITTER_CODE", SPLITTER_CODE);
                        }
                        else
                        {
                            oNewSplitter1.Components.GetComponent(12301).Recordset.Update("G3E_FID", iSplitFID);
                            oNewSplitter1.Components.GetComponent(12301).Recordset.Update("SPLITTER_TYPE", SPLITTER_TYPE);
                            oNewSplitter1.Components.GetComponent(12301).Recordset.Update("SPLITTER_CLASS", SPLITTER_CLASS);
                            oNewSplitter1.Components.GetComponent(12301).Recordset.Update("NO_OF_SPLITTER", SPlit_num);
                            oNewSplitter1.Components.GetComponent(12301).Recordset.Update("SPLITTER_CODE", SPLITTER_CODE);
                        }

                        IGTRelationshipService mobjRelationshipService;
                        mobjRelationshipService = GTClassFactory.Create<IGTRelationshipService>(GTInsertFiberSplitter.m_oGTCustomCommandHelper);
                        mobjRelationshipService.DataContext = m_gtapp.DataContext;

                        short mRelationshipNumber = 2;
                        //Ownership    
                        mobjRelationshipService.ActiveFeature = mobjOwnerAttribute;

                        if (mobjRelationshipService.AllowSilentEstablish(oNewSplitter1))
                        {
                            mobjRelationshipService.SilentEstablish(mRelationshipNumber, oNewSplitter1);
                        }
                        mRelationshipNumber = 3;
                        //Ownership        
                        mobjRelationshipService.ActiveFeature = oNewSplitter1;

                        if (mobjRelationshipService.AllowSilentEstablish(mobjOwnerAttribute))
                        {
                            mobjRelationshipService.SilentEstablish(mRelationshipNumber, mobjOwnerAttribute);
                        }

                        progressBar1.Value += count;

                    }

                    #endregion
                    
                    #region SPLITTER FOMS
                    
                    for (int j = 1; j <= Convert.ToInt32(txt_NoSplitterFOMS.Text); j++)
                    {
                        int recordsAffected = 0;
                        this.m_GTDataContext = m_gtapp.DataContext;
                        string get_splitter = "select * from REF_FSPLITTER where SPLITTER_TYPE = '" + cmbSplitterTypeFOMS.Text + "'";
                        Recordset rsComp = m_GTDataContext.Execute(get_splitter, out recordsAffected, (int)CommandTypeEnum.adCmdText, null);
                        if (rsComp != null && rsComp.RecordCount > 0)
                        {
                            rsComp.MoveFirst();
                            MIN_MATERIAL = rsComp.Fields["MIN_MATERIAL"].Value.ToString();
                            SPLITTER_TYPE = rsComp.Fields["SPLITTER_TYPE"].Value.ToString();
                            SPLITTER_CLASS = "FOMS";
                        }
                        rsComp = null;

                        oNewSplitter1 = m_gtapp.DataContext.NewFeature(12300);
                        iSplitFID = oNewSplitter1.FID;

                        // NETELEM 51
                        oNewSplitter1.Components.GetComponent(51).Recordset.Update("JOB_STATE", "PROPOSED");
                        oNewSplitter1.Components.GetComponent(51).Recordset.Update("FEATURE_STATE", "PPF");
                        oNewSplitter1.Components.GetComponent(51).Recordset.Update("MIN_MATERIAL", MIN_MATERIAL);
                       
                        //ATTRIBUTE
                        SPlit_num++;
                        if (SPlit_num.ToString().Length == 1)
                            tCode = "0" + SPlit_num.ToString();
                        else
                            tCode = SPlit_num.ToString();

                        if (txtOLTID.Visible)
                            SPLITTER_CODE = txtOLTID.Text + "-" + tCode;
                        else SPLITTER_CODE = "SP" + tCode;

                        if (oNewSplitter1.Components.GetComponent(12301).Recordset.EOF)
                        {
                            oNewSplitter1.Components.GetComponent(12301).Recordset.AddNew("G3E_FID", iSplitFID);
                            oNewSplitter1.Components.GetComponent(12301).Recordset.Update("SPLITTER_TYPE", SPLITTER_TYPE);
                            oNewSplitter1.Components.GetComponent(12301).Recordset.Update("SPLITTER_CLASS", SPLITTER_CLASS);
                            oNewSplitter1.Components.GetComponent(12301).Recordset.Update("NO_OF_SPLITTER", SPlit_num);
                            oNewSplitter1.Components.GetComponent(12301).Recordset.Update("SPLITTER_CODE", SPLITTER_CODE);
                        }
                        else
                        {
                            oNewSplitter1.Components.GetComponent(12301).Recordset.Update("G3E_FID", iSplitFID);
                            oNewSplitter1.Components.GetComponent(12301).Recordset.Update("SPLITTER_TYPE", SPLITTER_TYPE);
                            oNewSplitter1.Components.GetComponent(12301).Recordset.Update("SPLITTER_CLASS", SPLITTER_CLASS);
                            oNewSplitter1.Components.GetComponent(12301).Recordset.Update("NO_OF_SPLITTER", SPlit_num);
                            oNewSplitter1.Components.GetComponent(12301).Recordset.Update("SPLITTER_CODE", SPLITTER_CODE);
                        }

                        IGTRelationshipService mobjRelationshipService;
                        mobjRelationshipService = GTClassFactory.Create<IGTRelationshipService>(GTInsertFiberSplitter.m_oGTCustomCommandHelper);
                        mobjRelationshipService.DataContext = m_gtapp.DataContext;

                        short mRelationshipNumber = 2;
                        //Ownership    
                        mobjRelationshipService.ActiveFeature = mobjOwnerAttribute;

                        if (mobjRelationshipService.AllowSilentEstablish(oNewSplitter1))
                        {
                            mobjRelationshipService.SilentEstablish(mRelationshipNumber, oNewSplitter1);
                        }
                        mRelationshipNumber = 3;
                        //Ownership        
                        mobjRelationshipService.ActiveFeature = oNewSplitter1;

                        if (mobjRelationshipService.AllowSilentEstablish(mobjOwnerAttribute))
                        {
                            mobjRelationshipService.SilentEstablish(mRelationshipNumber, mobjOwnerAttribute);
                        }

                        progressBar1.Value+=count;
                    }

                     #endregion


                }
                #endregion


                GTInsertFiberSplitter.m_oGTTransactionManager.Commit();
                progressBar1.Value=90;
                GTInsertFiberSplitter.m_oGTTransactionManager.RefreshDatabaseChanges();
                progressBar1.Value=100;
                
                m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, " Succesfully commit..");

                btn_Generate.Enabled = true;
                btnSelectOwner.Enabled = true;
                progressBar1.Visible = false;
                progressBar1.Value=0;
            }
            catch (Exception ex)
            {
                GTInsertFiberSplitter.m_oGTTransactionManager.Rollback();
                MessageBox.Show(ex.Message, "Add Splitter(s)", MessageBoxButtons.OK, MessageBoxIcon.Error);  
                progressBar1.Visible=false;
                progressBar1.Value=0;
            }
        }
        #endregion

        #region Get value from database
        //Get Value from Database
        public string Get_Value(string sSql)
        {
            try
            {
                ADODB.Recordset rsPP = new ADODB.Recordset();
                rsPP = m_gtapp.DataContext.OpenRecordset(sSql, ADODB.CursorTypeEnum.adOpenStatic, ADODB.LockTypeEnum.adLockReadOnly, 1);
                if (rsPP.RecordCount > 0)
                {
                    rsPP.MoveFirst();
                    return (rsPP.Fields[0].Value.ToString());
                }
                return "";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Add Splitter(s)", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return "";
            }

        }
        #endregion
       
       #region SELECT  OWNER  -FDC/FDP
        private void btnSelectOwner_Click(object sender, EventArgs e)
        {
            if (m_gtapp.SelectedObjects.FeatureCount == 1)
            {
                foreach (IGTDDCKeyObject oDDCKeyObject in m_gtapp.SelectedObjects.GetObjects())
                {
                    if (!SelectOwner(oDDCKeyObject.FID, oDDCKeyObject.FNO))
                        m_gtapp.SelectedObjects.Clear();
                    return;
                }
            } 
            else 
                MessageBox.Show("Please Select SPlitter(s) Owner (FDC/FDP/DB)!", "Add Splitter(s)", MessageBoxButtons.OK, MessageBoxIcon.Information);
            
            return;
        }

        public bool SelectOwner(int iFID, short iFNO)
        {
            if (iFNO == 5100)
            {
                string OLTID = Get_Value("select olt_id from gc_fdc where g3e_fid=" + iFID.ToString());
                if (OLTID == "")
                {
                    MessageBox.Show("OLT ID of selected FDC is empty!\nPlease update OLT ID first!", "Add Splitter(s)", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return false;
                }
                lbOLTID.Visible = true;
                txtOLTID.Visible = true;
                txtOLTID.Text = OLTID;
                txtOwnerFID.Text = iFID.ToString();
                txtOwnerCode.Text = Get_Value("Select FDC_CODE from gc_fdc where g3e_fid = " + iFID.ToString());
                gbOwner.Text = "SELECTED OWNER - FDC";
                lbOwnerFID.Text = "FDC FID";
                lbOwnerCode.Text = "FDC Code";
                gbFOMSSp.Enabled = true;
                Load_Splitter("SELECT SPLITTER_TYPE, MIN_MATERIAL FROM REF_FSPLITTER  WHERE SPLITTER_CLASS='NORMAL'  order by SPLITTER_TYPE", false);
                Load_Splitter("SELECT SPLITTER_TYPE, MIN_MATERIAL FROM REF_FSPLITTER  WHERE SPLITTER_CLASS='FOMS'  order by SPLITTER_TYPE", true);
                txt_NoSplitter.Text = "";
                txt_NoSplitter.ReadOnly = false;
                txt_NoSplitterFOMS.Text = "1";
                OwnerFNO = iFNO;
                OwnerFID = iFID;
            }
            else
                if (iFNO == 5600)
                {
                    int number_sp = int.Parse(Get_Value("select count(*) from gc_ownership where g3e_fno=12300 and owner1_id in " +
                                  "(select g3e_id from gc_ownership where g3e_fid=" + iFID.ToString() + ")"));
                    if (number_sp >= 1)
                    {
                        MessageBox.Show("Selected FDP already has splitter!", "Add Splitter(s)", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return false;
                    }
                    string capacity = Get_Value("select fdp_capacity from gc_fdp where g3e_fid=" + iFID.ToString());
                    if (capacity == "")
                    {
                        MessageBox.Show("Capacity of selected FDP is empty!\nPlease update capacity first!", "Add Splitter(s)", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return false;
                    }
                    txtOwnerFID.Text = iFID.ToString();
                    txtOwnerCode.Text = Get_Value("Select FDP_CODE from gc_fdp where g3e_fid = " + iFID.ToString());
                    gbOwner.Text = "SELECTED OWNER - FDP";
                    lbOwnerFID.Text = "FDP FID";
                    lbOwnerCode.Text = "FDP Code";
                    gbFOMSSp.Enabled = false;
                    Load_Splitter("SELECT SPLITTER_TYPE, MIN_MATERIAL FROM REF_FSPLITTER  WHERE SPLITTER_CLASS='NORMAL' AND SPLITTER_TYPE='"+capacity+"'", false);
                    txt_NoSplitter.Text = "1";
                    txt_NoSplitter.ReadOnly = true;
                    txt_NoSplitterFOMS.Text = "0";
                    lbOLTID.Visible = false;
                    txtOLTID.Visible = false;
                    OwnerFNO = iFNO;
                    OwnerFID = iFID;
                }
                else
                    if (iFNO == 9900)
                    {
                        int number_sp = int.Parse(Get_Value("select count(*) from gc_ownership where g3e_fno=12300 and owner1_id in " +
                                  "(select g3e_id from gc_ownership where g3e_fid=" + iFID.ToString() + ")"));
                        if (number_sp >= 2)
                        {
                            MessageBox.Show("Selected DB already has splitters!", "Add Splitter(s)", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            return false;
                        }
                        
                        txtOwnerFID.Text = iFID.ToString();
                        txtOwnerCode.Text = Get_Value("Select DB_CODE from gc_db where g3e_fid = " + iFID.ToString());
                        gbOwner.Text = "SELECTED OWNER - DB";
                        lbOwnerFID.Text = "DB FID";
                        lbOwnerCode.Text = "DB Code";
                        gbFOMSSp.Enabled = false;
                        Load_Splitter("SELECT SPLITTER_TYPE, MIN_MATERIAL FROM REF_FSPLITTER  WHERE SPLITTER_CLASS='NORMAL' "+
                        "AND SPLITTER_TYPE IN ("+
                            "SELECT distinct splitter1 as SPLITTER_TYPE FROM FDP_TYPE where feature='DB' "+
                            "union SELECT distinct splitter2 as SPLITTER_TYPE FROM FDP_TYPE where feature='DB')",false);
                        txt_NoSplitter.Text = "1";
                        txt_NoSplitter.ReadOnly = true;
                        txt_NoSplitterFOMS.Text = "0";
                        lbOLTID.Visible = false;
                        txtOLTID.Visible = false;
                        OwnerFNO = iFNO;
                        OwnerFID = iFID;
                    }
                    else
                    {
                        MessageBox.Show("Please Select SPlitter(s) Owner (FDC/FDP/DB)!", "Add Splitter(s)", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return false;
                    }

                              

                return true;
        }



        private void Load_Splitter(string sSql, bool foms)
        {
            try
            {
                int recordsAffected = 0;
                Recordset rsComp = m_gtapp.DataContext.Execute(sSql, out recordsAffected, (int)CommandTypeEnum.adCmdText, null);
                if (rsComp != null)
                {
                    if (!rsComp.EOF)
                    {
                        if (!foms)
                            cmbSplitterType.Items.Clear();
                        else
                            cmbSplitterTypeFOMS.Items.Clear();
                        //cmbType.DropDownWidth = 400;
                        while (!rsComp.EOF)
                        {
                            //cboCblMIN.Items.Add(new cboItem(rsComp.Fields["MIN_MATERIAL"].Value.ToString() + " - " + rsComp.Fields["CABLE_TYPE"].Value.ToString() + " - " + rsComp.Fields["CABLE_SIZE"].Value.ToString() + " - " + rsComp.Fields["CORE_CONSTYPE"].Value.ToString() + " - " + rsComp.Fields["INSTALL_TYPE"].Value.ToString(), rsComp.Fields["MIN_MATERIAL"].Value.ToString()));
                            if (!foms)
                                cmbSplitterType.Items.Add(new cboItem(rsComp.Fields["SPLITTER_TYPE"].Value.ToString(), rsComp.Fields["MIN_MATERIAL"].Value.ToString()));
                            else
                                cmbSplitterTypeFOMS.Items.Add(new cboItem(rsComp.Fields["SPLITTER_TYPE"].Value.ToString(), rsComp.Fields["MIN_MATERIAL"].Value.ToString()));
                            rsComp.MoveNext();
                        }
                    }
                }
                rsComp = null;

                int i = 0;
                if (!foms)
                {
                    for (i = 0; i < cmbSplitterType.Items.Count; i++)
                    {
                        cmbSplitterType.SelectedItem = cmbSplitterType.Items[i];
                        if (cmbSplitterType.SelectedItem.ToString().ToUpper().Contains("2:4"))
                            break;
                    }
                    if (i == cmbSplitterType.Items.Count)
                        cmbSplitterType.SelectedIndex = 0;
                }
                else
                {
                    for (i = 0; i < cmbSplitterTypeFOMS.Items.Count; i++)
                    {
                        cmbSplitterTypeFOMS.SelectedItem = cmbSplitterTypeFOMS.Items[i];
                        if (cmbSplitterTypeFOMS.SelectedItem.ToString().ToUpper().Contains("2:16"))
                            break;
                    }
                    if (i == cmbSplitterTypeFOMS.Items.Count)
                        cmbSplitterTypeFOMS.SelectedIndex = 0;
                }
            }
            catch (Exception ex) { }

        }





#endregion
       
    }
}