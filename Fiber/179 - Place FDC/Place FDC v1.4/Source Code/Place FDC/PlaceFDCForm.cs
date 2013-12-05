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

namespace NEPS.GTechnology.PlaceFDC
{
    public partial class GTWindowsForm_PlaceFDC : Form
    {
       
      //  public static IGTm_gtapp m_GeoApp;
        //IGTm_gtapp m_GeoApp = m_gtapp;
        public Logger log;
        public  IGTFeaturePlacementService placementService = null;
        public IGTFeatureExplorerService mobjExplorerService = null;
        public IGTKeyObject mobjFDCAttribute = null;
        public short componentBeingPlaced=0;

        public bool flag = false;
        public string vPARENT = null;
        public string vCOMP = null;
        public string vFNO = null;

        public bool PlaceFlag = false;
        public int PlaceValue = 0;
        public bool CopyFlag = false;

        public int PlacedFID = 0;

        string ParentXY = "";
        string[] ParantBND;
        int ParentFID = 0;
        
        public string FEATURE = null;
        public short FNO = 0;
        public short A_CNO = 0;
        public short G_CNO = 0;
        public short L_CNO = 0;
        private Intergraph.GTechnology.API.IGTApplication m_gtapp = null;
        private IGTDataContext m_GTDataContext = null;
        private string Exch = null;

        #region loading form
        public GTWindowsForm_PlaceFDC()
        {
            try
            {
                InitializeComponent();
                //m_gtapp = GTm_gtapp.m_gtapp;
                m_gtapp = GTCustomCommandModeless.application;
                m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Running Place FDC...");

                log = Logger.getInstance();
              
            }
            catch (Exception ex)
            {
                log.WriteLog("");
                log.WriteLog(this.GetType().FullName, "ERROR: " + ex.Message);
                log.WriteLog(ex.StackTrace);            
                MessageBox.Show(ex.Message, "Place FDC", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }        
        
        private void GTWindowsForm_PlaceFDC_Shown(object sender, EventArgs e)
        {
            m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Running Place FDC...");            
        }
     
        private void GTWindowsForm_PlaceFDC_Load(object sender, EventArgs e)
        {
            PlaceFlag = false;
            PlaceValue = 0;
          //  cmbSplitterType.Items.Clear();
           // cmbSplitterTypeFOMS.Items.Clear();
            Load_Splitter("SELECT SPLITTER_TYPE, MIN_MATERIAL FROM REF_FSPLITTER  WHERE SPLITTER_CLASS='NORMAL'  order by SPLITTER_TYPE",false);
            Load_Splitter("SELECT SPLITTER_TYPE, MIN_MATERIAL FROM REF_FSPLITTER  WHERE SPLITTER_CLASS='FOMS'  order by SPLITTER_TYPE", true);
            txt_NoSplitter.Text = "";
            txt_NoSplitterFOMS.Text = "1";
            Exch = Get_Value("Select EXC_ABB from G3E_JOB where G3E_IDENTIFIER = '" + m_gtapp.DataContext.ActiveJob + "'");                
            Load_OLT();
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
                        if(!foms)
                        cmbSplitterType.Items.Clear();
                        else
                        cmbSplitterTypeFOMS.Items.Clear();
                        //cmbType.DropDownWidth = 400;
                        while (!rsComp.EOF)
                        {
                            //cboCblMIN.Items.Add(new cboItem(rsComp.Fields["MIN_MATERIAL"].Value.ToString() + " - " + rsComp.Fields["CABLE_TYPE"].Value.ToString() + " - " + rsComp.Fields["CABLE_SIZE"].Value.ToString() + " - " + rsComp.Fields["CORE_CONSTYPE"].Value.ToString() + " - " + rsComp.Fields["INSTALL_TYPE"].Value.ToString(), rsComp.Fields["MIN_MATERIAL"].Value.ToString()));
                            if(!foms)
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

        #region button generate click

        private void btn_Generate_Click(object sender, EventArgs e)
        {
            if (cmbSplitterType.Text == "")
            {
                MessageBox.Show("Please Select a Service Splitter Type", "Place FDC", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (cmbSplitterTypeFOMS.Text == "")
            {
                MessageBox.Show("Please Select a FOMS Splitter Type", "Place FDC", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            
            if (txt_NoSplitter.Text == "" )
            {
                MessageBox.Show("Please Enter correct No of SERVICE Splitter", "Place FDC", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (txt_NoSplitterFOMS.Text == "")
            {
                MessageBox.Show("Please Enter correct No of FOMS Splitter", "Place FDC", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            int try_no = 0;
            if (!int.TryParse(txt_NoSplitter.Text,out try_no))
            {
                MessageBox.Show("Please Enter correct No of SERVICE Splitter", "Place FDC", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (!int.TryParse(txt_NoSplitterFOMS.Text, out try_no))
            {
                MessageBox.Show("Please Enter correct No of FOMS Splitter", "Place FDC", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (cmbOLTID.Text == "")
            {
                MessageBox.Show("Please Select an OLT ID", "Place FDC", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            m_gtapp.SetProgressBarRange(1, 100);
            m_gtapp.BeginProgressBar();
            m_gtapp.SetProgressBarPosition(20);
           this.Hide();
            
           NewFDCobj();
       }
        #endregion

        #region New FDC
       private void NewFDCobj()
        {
            try
            {
                mobjFDCAttribute = null;
                GTCustomCommandModeless.m_oGTTransactionManager.Begin("PlaceFDC");
                mobjFDCAttribute = m_gtapp.DataContext.NewFeature(5100);
               

                int iFID = mobjFDCAttribute.FID;
                //  mobjFDCAttribute.Components.GetComponent(16001).Recordset.Update("MANHOLE_WALL_NUM", Wall_Selected);
                #region Attributes

                // NETELEM 51
                if (mobjFDCAttribute.Components.GetComponent(51).Recordset.EOF)
                {
                    mobjFDCAttribute.Components.GetComponent(51).Recordset.AddNew("G3E_FID", iFID);
                    mobjFDCAttribute.Components.GetComponent(51).Recordset.Update("JOB_STATE", "PROPOSED");
                    mobjFDCAttribute.Components.GetComponent(51).Recordset.Update("FEATURE_STATE", "PPF");
                }
                else
                {
                    mobjFDCAttribute.Components.GetComponent(51).Recordset.Update("G3E_FID", iFID);
                    mobjFDCAttribute.Components.GetComponent(51).Recordset.Update("JOB_STATE", "PROPOSED");
                    mobjFDCAttribute.Components.GetComponent(51).Recordset.Update("FEATURE_STATE", "PPF");
                }
               
                A_CNO = 5101;

                if (mobjFDCAttribute.Components.GetComponent(A_CNO).Recordset.EOF)
                {
                    mobjFDCAttribute.Components.GetComponent(A_CNO).Recordset.AddNew("G3E_FID", iFID);
                    mobjFDCAttribute.Components.GetComponent(A_CNO).Recordset.Update("OLT_ID", cmbOLTID.Text);
                    mobjFDCAttribute.Components.GetComponent(A_CNO).Recordset.Update("FDC_CODE", Gen_Code());
                }
                else
                {
                    mobjFDCAttribute.Components.GetComponent(A_CNO).Recordset.Update("G3E_FID", iFID);
                    mobjFDCAttribute.Components.GetComponent(A_CNO).Recordset.Update("OLT_ID", cmbOLTID.Text);
                    mobjFDCAttribute.Components.GetComponent(A_CNO).Recordset.Update("FDC_CODE", Gen_Code());
                }

                
                #endregion
               
                PlacedFID = mobjFDCAttribute.FID;
              
                //Open Feature Explorer
                m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, " Enter Attributes for FDC");
                mobjExplorerService = GTClassFactory.Create<IGTFeatureExplorerService>();
                mobjExplorerService.CancelClick += new EventHandler(mobjExplorerService_CancelClick);
                mobjExplorerService.SaveAndContinueClick += new EventHandler(mobjExplorerService_SaveAndContinueClick);
                mobjExplorerService.SaveClick += new EventHandler(mobjExplorerService_SaveClick);

                mobjExplorerService.ExploreFeature(mobjFDCAttribute, "Placement");
                mobjExplorerService.Visible = true;
                mobjExplorerService.Slide(true);               
               
            }
            catch (Exception ex) { }
        }
        #endregion

        # region Generate Code
         private string Gen_Code()
        {
           
                string code = "";
                try
                {
                int maxNum = 0;
                string sSql = "";
                ADODB.Recordset rsPP = new ADODB.Recordset();
                string Exch = Get_Value("Select EXC_ABB from G3E_JOB where G3E_IDENTIFIER = '" + m_gtapp.DataContext.ActiveJob.ToString() + "'");
                
                    sSql = "select substr(FDC_CODE,2,length(FDC_CODE)) from GC_FDC A, GC_NETELEM B where A.G3E_FID = B.G3E_FID and B.EXC_ABB = '" + Exch + "' and FDC_CODE <> '***'";
                    rsPP = m_gtapp.DataContext.OpenRecordset(sSql, ADODB.CursorTypeEnum.adOpenDynamic, ADODB.LockTypeEnum.adLockOptimistic, (int)ADODB.CommandTypeEnum.adCmdText);
                    if (rsPP.RecordCount > 0)
                    {
                        rsPP.MoveFirst();
                        for (int i = 0; i < rsPP.RecordCount; i++)
                        {
                            int tempint = 0;
                            if (int.TryParse(rsPP.Fields[0].Value.ToString(), out tempint))
                            {
                                if (maxNum < tempint)
                                    maxNum = tempint;
                            }
                            rsPP.MoveNext();


                        }
                        maxNum = maxNum + 1;
                        if (maxNum.ToString().Length == 1) code = "00" + maxNum.ToString();
                        else if (maxNum.ToString().Length == 2) code = "0" + maxNum.ToString();
                        else if (maxNum.ToString().Length == 3) code = maxNum.ToString();
                        return "C" +code;
                    }
                    else return "C001";
                
               
                    
            }
            catch (Exception ex) { }
            return code;
        }
          
        #endregion

        #region Validation
        private bool ValidationCode()
        {
            string code_to_check = "";
            short A_CNO = 5101;
            short FNO = 5100;

            for (int j = 0; j < mobjFDCAttribute.Components.GetComponent(A_CNO).Recordset.Fields.Count; j++)
            {
                if (mobjFDCAttribute.Components.GetComponent(A_CNO).Recordset.Fields[j].Name == "FDC_CODE")
                    code_to_check = mobjFDCAttribute.Components.GetComponent(A_CNO).Recordset.Fields[j].Value.ToString();
            }


            if (code_to_check == "")
            {
                MessageBox.Show("FDC Code can not be empty", "Place FDC", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            string sSql = "";
            ADODB.Recordset rsPP = new ADODB.Recordset();
            string Exch = Get_Value("Select EXC_ABB from G3E_JOB where G3E_IDENTIFIER = '" + m_gtapp.DataContext.ActiveJob.ToString() + "'");
            sSql = "select FDC_CODE from GC_FDC A, GC_NETELEM B where A.G3E_FID = B.G3E_FID and B.EXC_ABB = '" + Exch + "' and FDC_CODE = '" + code_to_check + "'";
                rsPP = m_GTDataContext.OpenRecordset(sSql, ADODB.CursorTypeEnum.adOpenDynamic, ADODB.LockTypeEnum.adLockOptimistic, (int)ADODB.CommandTypeEnum.adCmdText);
                if (rsPP.RecordCount > 0)
                {
                    MessageBox.Show("FDC Code already exists", "Place FDC", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;

                }
            

           
            return true;
        }

        public bool validation()
        { //required fields
            short iCNO = 5101;
            mobjFDCAttribute.Components.GetComponent(iCNO).Recordset.MoveFirst();
            for (int j = 0; j < mobjFDCAttribute.Components.GetComponent(iCNO).Recordset.RecordCount; j++)
            {
                for (int f = 0; f < mobjFDCAttribute.Components.GetComponent(iCNO).Recordset.Fields.Count; f++)
                {

                    if (mobjFDCAttribute.Components.GetComponent(iCNO).Recordset.Fields[f].Name == "CONTRACTOR")
                        if (mobjFDCAttribute.Components.GetComponent(iCNO).Recordset.Fields[f].Value.ToString() == "")
                        {
                            MessageBox.Show("CONTRACTOR field is required!", "Place FDC", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            return false;
                        }

                    if (mobjFDCAttribute.Components.GetComponent(iCNO).Recordset.Fields[f].Name == "MANUFACTURER")
                        if (mobjFDCAttribute.Components.GetComponent(iCNO).Recordset.Fields[f].Value.ToString() == "")
                        {
                            MessageBox.Show("MANUFACTURER field is required!", "Place FDC", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            return false;
                        }

                }
            }

            iCNO = 51;
            mobjFDCAttribute.Components.GetComponent(iCNO).Recordset.MoveFirst();
            for (int j = 0; j < mobjFDCAttribute.Components.GetComponent(iCNO).Recordset.RecordCount; j++)
            {
                for (int f = 0; f < mobjFDCAttribute.Components.GetComponent(iCNO).Recordset.Fields.Count; f++)
                {
                    if (mobjFDCAttribute.Components.GetComponent(iCNO).Recordset.Fields[f].Name == "MIN_MATERIAL")
                        if (mobjFDCAttribute.Components.GetComponent(iCNO).Recordset.Fields[f].Value.ToString() == "")
                        {
                            MessageBox.Show("Plant Unit field is required!", "Place FDC", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            return false;
                        }
                    

                }
            }
            ////
            if (!ValidationCode())
            {
                return false;
            }

            return true;
        }
        #endregion

        #region feature explorer events
        private void SaveAttribute()
        {
            if (validation())
            {
                m_gtapp.SetProgressBarPosition(70);
                m_gtapp.EndWaitCursor();

                if (!(mobjExplorerService == null))
                {
                    mobjExplorerService.Visible = false;
                    mobjExplorerService.Clear();
                }
                mobjExplorerService.CancelClick -= new EventHandler(mobjExplorerService_CancelClick);
                mobjExplorerService.SaveAndContinueClick -= new EventHandler(mobjExplorerService_SaveAndContinueClick);
                mobjExplorerService.SaveClick -= new EventHandler(mobjExplorerService_SaveClick);
                mobjExplorerService = null;
                m_gtapp.SetProgressBarPosition(80);
                placementService.StartComponent(mobjFDCAttribute, 5120); 
                componentBeingPlaced = 5120;
                return;
            }
            m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, " Enter Attributes for FDC");
        }

        private void mobjExplorerService_CancelClick(object sender, EventArgs e)
        {
            // Throws any errors raised to the error handler.
            try
            {
                GTCustomCommandModeless.m_oGTTransactionManager.Rollback();
                if (!(mobjExplorerService == null))
                {
                    mobjExplorerService.Visible = false;
                    mobjExplorerService.Clear();
                }
                mobjExplorerService.CancelClick -= new EventHandler(mobjExplorerService_CancelClick);
                mobjExplorerService.SaveAndContinueClick -= new EventHandler(mobjExplorerService_SaveAndContinueClick);
                mobjExplorerService.SaveClick -= new EventHandler(mobjExplorerService_SaveClick);
                mobjExplorerService = null;
                mobjFDCAttribute = null;
                MessageBox.Show("Placement is canceled!", "Place FDC", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Show();

            }
            catch (Exception ex)
            {
                if (this != null)
                {

                }
                MessageBox.Show(ex.Message, "Error.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //log.CloseFile();
            }
        }

        private void mobjExplorerService_SaveAndContinueClick(object sender, EventArgs e)
        {
            // Throws any errors raised to the error handler.
            try
            {
                SaveAttribute();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //log.CloseFile();
            }

        }

        private void mobjExplorerService_SaveClick(object sender, EventArgs e)
        {
            // Throws any errors raised to the error handler.
            try
            {

                SaveAttribute();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //log.CloseFile();
            }

        }

        #endregion

        #region submit FDC to data base DRAW FDC
        public void DrawFDC(short iFNO)
        {
            try
            {
                m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, " Wait, updating database changes...");
                int iFID = 0;
                int iSplitFID = 0;
                IGTKeyObject oNewSplitter1;
               

                A_CNO = 5101;
                G_CNO = 5120;
                L_CNO = 5130;

                iFID = PlacedFID;
                
                #region Attributes

               
                //64 GC_OWNERSHIP
                if (cmbSplitterType.Text != "")
                {
                    if (mobjFDCAttribute.Components.GetComponent(64).Recordset.EOF)
                    {
                        mobjFDCAttribute.Components.GetComponent(64).Recordset.AddNew("G3E_FID", iFID);
                        mobjFDCAttribute.Components.GetComponent(64).Recordset.Update("G3E_FNO", iFNO);
                        mobjFDCAttribute.Components.GetComponent(64).Recordset.Update("G3E_CID", "1");
                    }
                }                
                #endregion




                m_gtapp.SetProgressBarPosition(89);
               
                
                #region SPLITTER/OWNERSHIP
                if (cmbSplitterType.Text != "")
                {
                  //  GTCustomCommandModeless.m_oGTTransactionManager.Begin("DrawSplitter");

                    string MIN_MATERIAL = null;
                    string SPLITTER_TYPE = null;
                    string SPLITTER_CLASS = null;
                    string tCode = null;
                    int SPlit_num = 0;
                    string SPLITTER_CODE = null;

                    #region SPLITTER SERVICE
                    for (int i = 1; i <= Convert.ToInt32(txt_NoSplitter.Text); i++)
                    {
                        int recordsAffected = 0;
                        this.m_GTDataContext = m_gtapp.DataContext;
                        string get_splitter = "select * from REF_FSPLITTER  WHERE SPLITTER_CLASS='NORMAL' and SPLITTER_TYPE = '" + cmbSplitterType.Text + "'";
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
                        if (i.ToString().Length == 1) 
                            tCode = "0" + i.ToString();
                        else
                            tCode = i.ToString();

                        SPLITTER_CODE = cmbOLTID.Text+"-" + tCode;
                        SPlit_num = i;

                        if (oNewSplitter1.Components.GetComponent(12301).Recordset.EOF)
                        {
                            oNewSplitter1.Components.GetComponent(12301).Recordset.AddNew("G3E_FID", iSplitFID);
                            oNewSplitter1.Components.GetComponent(12301).Recordset.Update("SPLITTER_TYPE", SPLITTER_TYPE);
                            oNewSplitter1.Components.GetComponent(12301).Recordset.Update("SPLITTER_CLASS", SPLITTER_CLASS);
                            oNewSplitter1.Components.GetComponent(12301).Recordset.Update("NO_OF_SPLITTER", i);
                            oNewSplitter1.Components.GetComponent(12301).Recordset.Update("SPLITTER_CODE", SPLITTER_CODE);
                        }
                        else
                        {
                            oNewSplitter1.Components.GetComponent(12301).Recordset.Update("G3E_FID", iSplitFID);
                            oNewSplitter1.Components.GetComponent(12301).Recordset.Update("SPLITTER_TYPE", SPLITTER_TYPE);
                            oNewSplitter1.Components.GetComponent(12301).Recordset.Update("SPLITTER_CLASS", SPLITTER_CLASS);
                            oNewSplitter1.Components.GetComponent(12301).Recordset.Update("NO_OF_SPLITTER", i);
                            oNewSplitter1.Components.GetComponent(12301).Recordset.Update("SPLITTER_CODE", SPLITTER_CODE);
                        }

                        IGTRelationshipService mobjRelationshipService;
                        mobjRelationshipService = GTClassFactory.Create<IGTRelationshipService>(GTCustomCommandModeless.m_oGTCustomCommandHelper);
                        mobjRelationshipService.DataContext = m_gtapp.DataContext;

                        short mRelationshipNumber = 2;
                        //Ownership    
                       mobjRelationshipService.ActiveFeature = mobjFDCAttribute;

                        if (mobjRelationshipService.AllowSilentEstablish(oNewSplitter1))
                        {
                            mobjRelationshipService.SilentEstablish(mRelationshipNumber, oNewSplitter1);
                        }
                        mRelationshipNumber = 3;
                        //Ownership        
                        mobjRelationshipService.ActiveFeature = oNewSplitter1;

                        if (mobjRelationshipService.AllowSilentEstablish(mobjFDCAttribute))
                        {
                            mobjRelationshipService.SilentEstablish(mRelationshipNumber, mobjFDCAttribute);
                        }


                    }

                    #endregion
                    
                    #region SPLITTER FOMS
                    SPlit_num++;
                    for (int j = 1; j <= Convert.ToInt32(txt_NoSplitterFOMS.Text); j++)
                    {
                        int recordsAffected = 0;
                        this.m_GTDataContext = m_gtapp.DataContext;
                        string get_splitter = "select * from REF_FSPLITTER  WHERE SPLITTER_CLASS='FOMS' and SPLITTER_TYPE = '" + cmbSplitterTypeFOMS.Text + "'";
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
                        if (SPlit_num.ToString().Length == 1)
                            tCode = "0" + SPlit_num.ToString();
                        else
                            tCode = SPlit_num.ToString();

                        SPLITTER_CODE = cmbOLTID.Text + "-" + tCode;

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
                        mobjRelationshipService = GTClassFactory.Create<IGTRelationshipService>(GTCustomCommandModeless.m_oGTCustomCommandHelper);
                        mobjRelationshipService.DataContext = m_gtapp.DataContext;

                        short mRelationshipNumber = 2;
                        //Ownership    
                        mobjRelationshipService.ActiveFeature = mobjFDCAttribute;

                        if (mobjRelationshipService.AllowSilentEstablish(oNewSplitter1))
                        {
                            mobjRelationshipService.SilentEstablish(mRelationshipNumber, oNewSplitter1);
                        }
                        mRelationshipNumber = 3;
                        //Ownership        
                        mobjRelationshipService.ActiveFeature = oNewSplitter1;

                        if (mobjRelationshipService.AllowSilentEstablish(mobjFDCAttribute))
                        {
                            mobjRelationshipService.SilentEstablish(mRelationshipNumber, mobjFDCAttribute);
                        }

                        SPlit_num++;
                    }

                     #endregion


                }
                #endregion
              
                m_gtapp.SetProgressBarPosition(90);
                //Update ISP_CUSTOM
                //string strSQL = null;
                //int iRecordNum = 0;
                //object[] obj = null;
                //GTCustomCommandModeless.m_oGTTransactionManager.Begin("UpdateOLTID");
                //strSQL = "UPDATE ISP_CUSTOM.ISP_PLACEMENT SET FID = " + iFID + " where TYPE = 'OLT' and LOCATION = '" + Exch + "' and NAME = '" + cmbOLTID.Text + "'";
                //m_gtapp.DataContext.Execute(strSQL, out iRecordNum, (int)ADODB.CommandTypeEnum.adCmdText, obj);
                //GTCustomCommandModeless.m_oGTTransactionManager.Commit();

                //MessageBox.Show("Completed", "Place FDC", MessageBoxButtons.OK, MessageBoxIcon.Information);
                GTCustomCommandModeless.m_oGTTransactionManager.Commit();
                GTCustomCommandModeless.m_oGTTransactionManager.RefreshDatabaseChanges();
                PlacedFID = iFID;

                m_gtapp.SetProgressBarPosition(100);
                m_gtapp.EndProgressBar();
                m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, " Succesfully commit..");

            }
            catch (Exception ex)
            {
                log.WriteLog("");
                log.WriteLog(this.GetType().FullName, "ERROR: " + ex.Message);
                log.WriteLog(ex.StackTrace);
                GTCustomCommandModeless.m_oGTTransactionManager.Rollback();
                MessageBox.Show(ex.Message, "Place FDC", MessageBoxButtons.OK, MessageBoxIcon.Error);                
            }
        }
        #endregion

        #region load OLT
        private void Load_OLT()
        {
            try
            {
                string sSql;
                int recordsAffected = 0;
                this.m_GTDataContext = m_gtapp.DataContext;
                sSql = "select NAME  from ISP_CUSTOM.ISP_PLACEMENT where TYPE = 'OLT' and LOCATION = '" + Exch + "'";
                Recordset rsComp = m_GTDataContext.Execute(sSql, out recordsAffected, (int)CommandTypeEnum.adCmdText, null);
                cmbOLTID.Items.Clear();

                if (rsComp.RecordCount > 0)
                {
                    rsComp.MoveFirst();
                    do
                    {
                        cmbOLTID.Items.Add(rsComp.Fields[0].Value.ToString());
                        rsComp.MoveNext();
                    }
                    while (!rsComp.EOF);
                }
                rsComp = null;
            }
            catch (Exception ex)
            {
                log.WriteLog("");
                log.WriteLog(this.GetType().FullName, "ERROR: " + ex.Message);
                log.WriteLog(ex.StackTrace);
                MessageBox.Show(ex.Message, "Place FDC", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region Copy attributes
        private void CopyAttributes(Recordset FromRec, Recordset ToRec)
        {
            if (!FromRec.EOF)
            {
                for (int i = 0; i < FromRec.Fields.Count; i++)
                {
                    if ((FromRec.Fields[i].Name != "G3E_FID") && (FromRec.Fields[i].Name != "G3E_ID") && (FromRec.Fields[i].Name != "G3E_CNO") && (FromRec.Fields[i].Name != "G3E_FNO") && (FromRec.Fields[i].Name != "G3E_CID") && (FromRec.Fields[i].Name != "FDP_CODE"))
                    {
                        if (FromRec.Fields[i].Value != DBNull.Value)
                            ToRec.Update(FromRec.Fields[i].Name, FromRec.Fields[i].Value);
                    }
                }
            }
        }
        #endregion

        #region button cancel/close
               
        private void clear()
        {
            PlaceFlag = false;
            PlaceValue = 0;
           // cmbSplitterType.Items.Clear();
            Load_Splitter("SELECT SPLITTER_TYPE, MIN_MATERIAL  FROM REF_FSPLITTER  WHERE SPLITTER_CLASS='NORMAL'  order by SPLITTER_TYPE", false);
            Load_Splitter("SELECT SPLITTER_TYPE, MIN_MATERIAL FROM REF_FSPLITTER  WHERE SPLITTER_CLASS='FOMS'  order by SPLITTER_TYPE", true);
            txt_NoSplitter.Text = "";
            txt_NoSplitterFOMS.Text = "1";
            Load_OLT();
        }
        
        private void btn_Clear_Click(object sender, EventArgs e)
        {
            clear();
        }


        private void btn_Close_Click(object sender, EventArgs e)
        {
            this.Close();
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
                return null;
            }
            catch (Exception ex)
            {
                log.WriteLog("");
                log.WriteLog(this.GetType().FullName, "ERROR: " + ex.Message);
                log.WriteLog(ex.StackTrace);
                MessageBox.Show(ex.Message, "Place FDC", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }

        }
        #endregion

       
    }
}