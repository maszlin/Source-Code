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

namespace NEPS.GTechnology.PlaceFDP
{
    public partial class GTWindowsForm_PlaceFDP : Form
    {
       
      //  public static IGTm_gtapp m_GeoApp;
        //IGTm_gtapp m_GeoApp = m_gtapp;
        public Logger log;

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
        public int StyleId = 0;
        public int StyleTextId = 0;
        public GTAlignmentConstants StyleTextAlignment = 0;
        public string TextContent = "";
        IGTFeatureExplorerService mobjExplorerService = null;
        public IGTKeyObject mobjFDPAttribute = null;
        private string _XPoint;
        public string XPointGeom
        {
            get
            {
                return _XPoint;
            }
            set
            {
                                
            }
        }

        private string _YPoint;
        public string YPointGeom
        {
            get
            {
                return _YPoint;
            }
            set
            {
               // listBox2.Items.Add(value);              
            }
        }



        public GTWindowsForm_PlaceFDP()
        {
            try
            {
                InitializeComponent();
                //m_gtapp = GTm_gtapp.m_gtapp;
                m_gtapp = GTCustomCommandModeless.application;
                m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Running Place FDP...");

                log = Logger.getInstance();
              
            }
            catch (Exception ex)
            {
                log.WriteLog("");
                log.WriteLog(this.GetType().FullName, "ERROR: " + ex.Message);
                log.WriteLog(ex.StackTrace);            
                MessageBox.Show(ex.Message, "Place FDP", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }        
        

        private Intergraph.GTechnology.API.IGTApplication m_gtapp = null; 
        //IGTm_gtapp m_gtapp = m_gtapp;


        private void GTWindowsForm_InitRoute_Shown(object sender, EventArgs e)
        {
            m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Running Place FDP...");            
        }

        private void GTWindowsForm_InitRoute_Activated(object sender, EventArgs e)
        {
            m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Running Place FDP...");            
        }


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
                MessageBox.Show(ex.Message, "Place FDP", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }

        }
        private IGTDataContext m_GTDataContext = null;

        private void GTWindowsForm_InitRoute_Load(object sender, EventArgs e)
        {
            PlaceFlag = false;
            PlaceValue = 0;
            cmbFeature.Items.Clear();
            cmbType.Items.Clear();
            //cmbSplitter1.Items.Clear();
            //cmbSplitter2.Items.Clear();
            Load_Combo(cmbFeature, "SELECT DISTINCT FEATURE FROM FDP_TYPE");

            //lblSplitter1.Visible = false;
            //lblSplitter2.Visible = false;
            //cmbSplitter1.Visible = false;
            //cmbSplitter2.Visible = false;
            cmbFeature.Text = "FDP";

            ChangeSelectedFeatureType();
        }

        private void Load_Combo(ComboBox cmb, string sSql)
        {
            int recordsAffected = 0;           
            this.m_GTDataContext = m_gtapp.DataContext;
            Recordset rsComp = m_GTDataContext.Execute(sSql, out recordsAffected, (int)CommandTypeEnum.adCmdText, null);
            cmb.Items.Clear();
           
            if (rsComp != null &&  rsComp.RecordCount > 0 )
            {
                rsComp.MoveFirst();
                do
                {
                    cmb.Items.Add(rsComp.Fields[0].Value.ToString());
                    rsComp.MoveNext();
                }
                while (!rsComp.EOF);
            }
            rsComp = null;
            
        }

        private void cmbFeature_SelectedIndexChanged(object sender, EventArgs e)
        {
           
                ChangeSelectedFeatureType();
           
        }
        private void ChangeSelectedFeatureType()
        {
            try{
           
            cmbSplitter1.Items.Clear();
            cmbSplitter2.Items.Clear();
            txtParant.Text = "";
            txtParantCode.Text = "";
            Load_Combo(cmbType, "SELECT DISTINCT FDP_TYPE FROM FDP_TYPE WHERE FEATURE = '" + cmbFeature.Text + "'");

            lblSplitter1.Visible = false;
            lblSplitter2.Visible = false;
            cmbSplitter1.Visible = false;
            cmbSplitter2.Visible = false;

            btn_Generate.Text = "Generate " + cmbFeature.Text;

            IGTHelperService helper = GTClassFactory.Create<IGTHelperService>();
            helper.DataContext = m_GTDataContext;

            if (cmbFeature.Text == "FDP" || cmbFeature.Text == "FOMS")           //1 Splitter
            {
                if(cmbFeature.Text == "FOMS")
                    FEATURE = "FOMS";
                else
                    FEATURE = "FDP";
                FNO = 5600;
                A_CNO = 5601;
                G_CNO = 5620;
                L_CNO = 5630;
                groupBox2.Text = "Parent Device: FDC";
                lblParentDevice.Text = "FDC";
                vCOMP = "VGC_FDC_S";
                vFNO = "5100";

                GTCustomCommandModeless.m_oGTTransactionManager.Begin("Feature to take styles");
                IGTKeyObject oNewFeature = m_gtapp.DataContext.NewFeature(FNO);
                oNewFeature.Components.GetComponent(51).Recordset.Update("FEATURE_STATE", "PPF");
                StyleId = helper.GetComponentStyleID(oNewFeature, G_CNO);
                helper.GetComponentLabel(oNewFeature, L_CNO, ref TextContent, ref StyleTextAlignment);
                StyleTextId = helper.GetComponentStyleID(oNewFeature, L_CNO);
                GTCustomCommandModeless.m_oGTTransactionManager.Rollback();
            }
            else 
            if (cmbFeature.Text == "PFDP")           //1 Splitter
            {
                FEATURE = "PFDP";
                FNO = 6800;
                A_CNO = 6801;
                G_CNO = 6820;
                L_CNO = 6830;
                groupBox2.Text = "Parent Device: FDC";
                lblParentDevice.Text = "FDC";
                vCOMP = "VGC_FDC_S";
                vFNO = "5100";

                GTCustomCommandModeless.m_oGTTransactionManager.Begin("Feature to take styles");
                IGTKeyObject oNewFeature = m_gtapp.DataContext.NewFeature(FNO);
                oNewFeature.Components.GetComponent(51).Recordset.Update("FEATURE_STATE", "PPF");
                StyleId = helper.GetComponentStyleID(oNewFeature, G_CNO);
                helper.GetComponentLabel(oNewFeature, L_CNO, ref TextContent, ref StyleTextAlignment);
                StyleTextId = helper.GetComponentStyleID(oNewFeature, L_CNO);
                GTCustomCommandModeless.m_oGTTransactionManager.Rollback();
            }
            else if (cmbFeature.Text == "TIE")  //No Splitter
            {
                FEATURE = "TIE";
                FNO = 5800;
                A_CNO = 5801;
                G_CNO = 5820;
                L_CNO = 5830;
                groupBox2.Text = "Parent Device: FDP";
                lblParentDevice.Text = "FDP";
                vCOMP = "VGC_FDP_S";
                vFNO = "5600";
                GTCustomCommandModeless.m_oGTTransactionManager.Begin("Feature to take styles");
                IGTKeyObject oNewFeature = m_gtapp.DataContext.NewFeature(FNO);
                oNewFeature.Components.GetComponent(51).Recordset.Update("FEATURE_STATE", "PPF");
                StyleId = helper.GetComponentStyleID(oNewFeature, G_CNO);
                helper.GetComponentLabel(oNewFeature, L_CNO, ref TextContent, ref StyleTextAlignment);
                StyleTextId = helper.GetComponentStyleID(oNewFeature, L_CNO);
                GTCustomCommandModeless.m_oGTTransactionManager.Rollback();
            }
            else if (cmbFeature.Text == "FTB")      //No Splitter
            {
                FEATURE = "FTB";
                FNO = 5900;
                A_CNO = 5901;
                G_CNO = 5920;
                L_CNO = 5930;

                groupBox2.Text = "Parent Device: PFDP";
                lblParentDevice.Text = "PFDP";
                vCOMP = "VGC_PFDP_S";
                vFNO = "6800";
                GTCustomCommandModeless.m_oGTTransactionManager.Begin("Feature to take styles");
                IGTKeyObject oNewFeature = m_gtapp.DataContext.NewFeature(FNO);
                oNewFeature.Components.GetComponent(51).Recordset.Update("FEATURE_STATE", "PPF");
                StyleId = helper.GetComponentStyleID(oNewFeature, G_CNO);
                helper.GetComponentLabel(oNewFeature, L_CNO, ref TextContent, ref StyleTextAlignment);
                StyleTextId = helper.GetComponentStyleID(oNewFeature, L_CNO);
                GTCustomCommandModeless.m_oGTTransactionManager.Rollback();
            }
            else if (cmbFeature.Text == "DB")       //2 Splitter
            {
                FEATURE = "DB";
                FNO = 9900;
                A_CNO = 9901;
                G_CNO = 9920;
                L_CNO = 9930;

                groupBox2.Text = "Parent Device: FDC";
                lblParentDevice.Text = "FDC";
                vCOMP = "VGC_FDC_S";
                vFNO = "5100";
                GTCustomCommandModeless.m_oGTTransactionManager.Begin("Feature to take styles");
                IGTKeyObject oNewFeature = m_gtapp.DataContext.NewFeature(FNO);
                oNewFeature.Components.GetComponent(51).Recordset.Update("FEATURE_STATE", "PPF");
                StyleId = helper.GetComponentStyleID(oNewFeature, G_CNO);
                helper.GetComponentLabel(oNewFeature, L_CNO, ref TextContent, ref StyleTextAlignment);
                StyleTextId = helper.GetComponentStyleID(oNewFeature, L_CNO);
                GTCustomCommandModeless.m_oGTTransactionManager.Rollback();
            }
    }
    catch (Exception ex)
    {
        if (GTCustomCommandModeless.m_oGTTransactionManager.TransactionInProgress)
            GTCustomCommandModeless.m_oGTTransactionManager.Rollback();
        StyleId = 5820001;
        StyleTextId = 5830001;
    }
        }

        private void cmbType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbFeature.Text == "FDP" || cmbFeature.Text == "FOMS" || cmbFeature.Text == "PFDP")           //1 Splitter
            {
                cmbSplitter1.Items.Clear();
                cmbSplitter2.Items.Clear();
                Load_Combo(cmbSplitter1, "SELECT DISTINCT SPLITTER1 FROM FDP_TYPE WHERE FEATURE = '" + cmbFeature.Text + "' and FDP_TYPE = '" + cmbType.Text + "'  and SPLITTER1 is not null");
                lblSplitter1.Visible = true;
                cmbSplitter1.Visible = true;                
                
            }

            else if (cmbFeature.Text == "TIE" || cmbFeature.Text == "FTB")  //No Splitter
            {
                
            }
           
            else if (cmbFeature.Text == "DB")       //2 Splitter
            {
                cmbSplitter1.Items.Clear();
                cmbSplitter2.Items.Clear();
                Load_Combo(cmbSplitter1, "SELECT DISTINCT SPLITTER1 FROM FDP_TYPE WHERE FEATURE = '" + cmbFeature.Text + "' and FDP_TYPE = '" + cmbType.Text + "' and SPLITTER1 is not null");
                Load_Combo(cmbSplitter2, "SELECT DISTINCT SPLITTER2 FROM FDP_TYPE WHERE FEATURE = '" + cmbFeature.Text + "' and FDP_TYPE = '" + cmbType.Text + "' and SPLITTER2 is not null");
                lblSplitter1.Visible = true;
                cmbSplitter1.Visible = true;
                lblSplitter2.Visible = true;
                cmbSplitter2.Visible = true;

            }

        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (cmbFeature.Text == "")
            {
                MessageBox.Show("Please Select a Feature", "Place FDP", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (cmbType.Text == "")
            {
                MessageBox.Show("Please Select a FDP Type", "Place FDP", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (cmbFeature.Text == "FDP" || cmbFeature.Text == "FOMS" || cmbFeature.Text == "DB" || cmbFeature.Text == "PFDP")
            {
                if (cmbSplitter1.Text == "")
                {
                    MessageBox.Show("Please Select a Splitter 1", "Place FDP", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            }

            if (cmbSplitter2.Text == "" && cmbFeature.Text == "DB")
            {
                MessageBox.Show("Please Select a Splitter 2", "Place FDP", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            
            PlaceFlag = true;
            CopyFlag = false;
            this.Hide();
            m_gtapp.SelectedObjects.Clear();
           // m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Fill up attributes for "+cmbFeature.Text +"...");//"Point to place new "+cmbFeature.Text+"!Right click to cancel placement");
            //DrawFDP(5600);
            NewFDPobj();
         }

         #region feature explorer events
        private void NewFDPobj()
        {
            try
            {
                mobjFDPAttribute = null;
                mobjExplorerService = GTClassFactory.Create<IGTFeatureExplorerService>();
                mobjExplorerService.CancelClick += new EventHandler(mobjExplorerService_CancelClick);
                mobjExplorerService.SaveAndContinueClick += new EventHandler(mobjExplorerService_SaveAndContinueClick);
                mobjExplorerService.SaveClick += new EventHandler(mobjExplorerService_SaveClick);

                GTCustomCommandModeless.m_oGTTransactionManager.Begin("Place FDP/PFDP/Tie/FTB/DB Attribute");

                mobjFDPAttribute = m_gtapp.DataContext.NewFeature(FNO);
                int iFID = mobjFDPAttribute.FID;
                //  mobjFDPAttribute.Components.GetComponent(16001).Recordset.Update("MANHOLE_WALL_NUM", Wall_Selected);
                #region Attributes

                // NETELEM 51
                mobjFDPAttribute.Components.GetComponent(51).Recordset.Update("JOB_STATE",Get_Value("Select JOB_STATE from G3E_JOB where G3E_IDENTIFIER = '" + m_gtapp.DataContext.ActiveJob.ToString() + "'"));
                mobjFDPAttribute.Components.GetComponent(51).Recordset.Update("EXC_ABB",Get_Value("Select EXC_ABB from G3E_JOB where G3E_IDENTIFIER = '" + m_gtapp.DataContext.ActiveJob.ToString() + "'"));
                mobjFDPAttribute.Components.GetComponent(51).Recordset.Update("SCHEME_NAME",Get_Value("Select SCHEME_NAME from G3E_JOB where G3E_IDENTIFIER = '" + m_gtapp.DataContext.ActiveJob.ToString() + "'"));
                mobjFDPAttribute.Components.GetComponent(51).Recordset.Update("JOB_ID",Get_Value("Select SCHEME_NAME from G3E_JOB where G3E_IDENTIFIER = '" + m_gtapp.DataContext.ActiveJob.ToString() + "'"));
                mobjFDPAttribute.Components.GetComponent(51).Recordset.Update("FEATURE_STATE", "PPF");
                string sql_get_material = "SELECT MIN_MATERIAL FROM FDP_TYPE WHERE FEATURE = '" + cmbFeature.Text + "' and FDP_TYPE = '" + cmbType.Text + "'";
                if (cmbSplitter1.Text.Trim() != "")
                    sql_get_material += " and SPLITTER1='" + cmbSplitter1.Text + "'";
                if (cmbSplitter2.Text.Trim() != "")
                    sql_get_material += " and SPLITTER2='" + cmbSplitter2.Text + "'";
                string MATERIAL = Get_Value(sql_get_material);
                mobjFDPAttribute.Components.GetComponent(51).Recordset.Update("MIN_MATERIAL", MATERIAL);

                if (mobjFDPAttribute.Components.GetComponent(A_CNO).Recordset.EOF)
                {
                    mobjFDPAttribute.Components.GetComponent(A_CNO).Recordset.AddNew("G3E_FID", iFID);
                }

                if (cmbFeature.Text != "DB" && cmbFeature.Text != "PFDP")
                {
                    mobjFDPAttribute.Components.GetComponent(A_CNO).Recordset.Update("FDP_CLASS", cmbFeature.Text);
                    mobjFDPAttribute.Components.GetComponent(A_CNO).Recordset.Update("FDP_TYPE", cmbType.Text);
                    mobjFDPAttribute.Components.GetComponent(A_CNO).Recordset.Update("FDP_CAPACITY", cmbSplitter1.Text);
                }
                else if (cmbFeature.Text == "PFDP")
                {
                    mobjFDPAttribute.Components.GetComponent(A_CNO).Recordset.Update("FDP_TYPE", cmbType.Text);
                    mobjFDPAttribute.Components.GetComponent(A_CNO).Recordset.Update("FDP_CAPACITY", cmbSplitter1.Text);
                }
                else
                {
                    mobjFDPAttribute.Components.GetComponent(A_CNO).Recordset.Update("DB_TYPE", cmbType.Text);
                    mobjFDPAttribute.Components.GetComponent(A_CNO).Recordset.Update("DB_CAPACITY",cmbSplitter2.Text);
                    //int totalcapacity = 0;
                    //int spnum = 0;
                    //string sp = cmbSplitter1.Text.Trim();
                    //int l = sp.IndexOf(":");
                    //if (l > 0)
                    //{
                    //    string temp = sp.Substring(l + 1);
                    //    if (int.TryParse(temp, out spnum))
                    //        totalcapacity += spnum;
                    //}
                    //sp = cmbSplitter2.Text.Trim();
                    //l = sp.IndexOf(":");
                    //if (l > 0)
                    //{
                    //    string temp = sp.Substring(l + 1);
                    //    if (int.TryParse(temp, out spnum))
                    //        totalcapacity += spnum;
                    //}

                    //if (totalcapacity > 0)
                    //    mobjFDPAttribute.Components.GetComponent(A_CNO).Recordset.Update("TOTAL_CAPACITY", totalcapacity);

                }
                if (cmbFeature.Text == "FDP")
                {
                    mobjFDPAttribute.Components.GetComponent(A_CNO).Recordset.Update("FDP_CLASS", "Normal");
                }
                else if (cmbFeature.Text == "FOMS")
                {
                    mobjFDPAttribute.Components.GetComponent(A_CNO).Recordset.Update("FDP_CLASS", "FOMS");
                   // mobjFDPAttribute.Components.GetComponent(A_CNO).Recordset.Update("BND_FID", 0);
                }
                else if (cmbFeature.Text == "TIE")
                {
                    mobjFDPAttribute.Components.GetComponent(A_CNO).Recordset.Update("FDP_CLASS", "TIE");
                }
                else if (cmbFeature.Text == "FTB")
                {
                    mobjFDPAttribute.Components.GetComponent(A_CNO).Recordset.Update("FDP_CLASS", "FTB");
                }
                else if (cmbFeature.Text == "DB")
                {
                    //  oNewFeature.Components.GetComponent(A_CNO).Recordset.Update("FDP_CLASS", "DB");
                }

                if (cmbFeature.Text == "FDP" || cmbFeature.Text == "FOMS" || cmbFeature.Text == "PFDP" || cmbFeature.Text == "DB")
                {
                    string FDC_CODE = Get_Value("select FDC_CODE from GC_FDC where g3e_fid = " + txtParant.Text);
                    mobjFDPAttribute.Components.GetComponent(A_CNO).Recordset.Update("FDC_FID", txtParant.Text);
                    mobjFDPAttribute.Components.GetComponent(A_CNO).Recordset.Update("FDC_CODE", FDC_CODE.Trim());
                    if (cmbFeature.Text == "FDP" || cmbFeature.Text == "FOMS" || cmbFeature.Text == "PFDP")
                    mobjFDPAttribute.Components.GetComponent(A_CNO).Recordset.Update("FDP_CODE", Gen_Code("", FDC_CODE.Trim()));
                else mobjFDPAttribute.Components.GetComponent(A_CNO).Recordset.Update("DB_CODE", Gen_Code("", FDC_CODE.Trim()));

                }
                else if (cmbFeature.Text == "TIE" )
                {
                    string FDC_FID = Get_Value("select FDC_FID from GC_FDP where g3e_fid = " + txtParant.Text);
                    string FDC_CODE = Get_Value("select FDC_CODE from GC_FDP where g3e_fid = " + txtParant.Text);
                    if (FDC_FID == "")
                        FDC_FID = Get_Value("select fdc.g3e_fid from GC_FDC fdc, GC_NETELEM net where "
                    + " net.g3e_fid=fdc.g3e_fid and net.exc_abb='"
                    + Get_Value("Select EXC_ABB from G3E_JOB where G3E_IDENTIFIER = '" + m_gtapp.DataContext.ActiveJob.ToString() + "'")
                    +"' and fdc.FDC_CODE='" + FDC_CODE+"'");
                    string MAINFDP_CODE = Get_Value("select FDP_CODE from GC_FDP where g3e_fid = " + txtParant.Text);
                    mobjFDPAttribute.Components.GetComponent(A_CNO).Recordset.Update("MAINFDP_FID", txtParant.Text);
                    mobjFDPAttribute.Components.GetComponent(A_CNO).Recordset.Update("MAINFDP_CODE", MAINFDP_CODE.Trim());
                    mobjFDPAttribute.Components.GetComponent(A_CNO).Recordset.Update("FDC_FID", FDC_FID);
                    mobjFDPAttribute.Components.GetComponent(A_CNO).Recordset.Update("FDC_CODE", FDC_CODE.Trim());
                    mobjFDPAttribute.Components.GetComponent(A_CNO).Recordset.Update("FDP_CODE", Gen_Code(MAINFDP_CODE.Trim(), FDC_CODE.Trim()));
                } else
                if (cmbFeature.Text == "FTB")
                {
                    string FDC_FID = Get_Value("select FDC_FID from GC_PFDP where g3e_fid = " + txtParant.Text);
                    string FDC_CODE = Get_Value("select FDC_CODE from GC_PFDP where g3e_fid = " + txtParant.Text);
                    if (FDC_FID == "")
                        FDC_FID = Get_Value("select fdc.g3e_fid from GC_FDC fdc, GC_NETELEM net where "
                    + " net.g3e_fid=fdc.g3e_fid and net.exc_abb='"
                    + Get_Value("Select EXC_ABB from G3E_JOB where G3E_IDENTIFIER = '" + m_gtapp.DataContext.ActiveJob.ToString() + "'")
                    + "' and fdc.FDC_CODE='" + FDC_CODE + "'");
                    string MAINFDP_CODE = Get_Value("select FDP_CODE from GC_PFDP where g3e_fid = " + txtParant.Text);
                    mobjFDPAttribute.Components.GetComponent(A_CNO).Recordset.Update("MAINFDP_FID", txtParant.Text);
                    mobjFDPAttribute.Components.GetComponent(A_CNO).Recordset.Update("MAINFDP_CODE", MAINFDP_CODE.Trim());
                    mobjFDPAttribute.Components.GetComponent(A_CNO).Recordset.Update("FDC_FID", FDC_FID);
                    mobjFDPAttribute.Components.GetComponent(A_CNO).Recordset.Update("FDC_CODE", FDC_CODE.Trim());
                    mobjFDPAttribute.Components.GetComponent(A_CNO).Recordset.Update("FDP_CODE", Gen_Code(MAINFDP_CODE.Trim(), FDC_CODE.Trim()));
                }

                mobjFDPAttribute.Components.GetComponent(A_CNO).Recordset.Update("CONTRACTOR", "3OPP");
                mobjFDPAttribute.Components.GetComponent(A_CNO).Recordset.Update("MANUFACTURER", "FIBERHOME");
                mobjFDPAttribute.Components.GetComponent(A_CNO).Recordset.Update("PLINTH", "N");
                mobjFDPAttribute.Components.GetComponent(A_CNO).Recordset.Update("DIST_FROM_EXC", "0");
                mobjFDPAttribute.Components.GetComponent(A_CNO).Recordset.Update("RAD_DIST", "0");
             //   mobjFDPAttribute.Components.GetComponent(A_CNO).Recordset.Update("INSTALL_LOC", "INTERNAL");
                mobjFDPAttribute.Components.GetComponent(A_CNO).Recordset.Update("DB_LOSS", "0");
                #endregion

                mobjExplorerService.ExploreFeature(mobjFDPAttribute, "Placement");
                mobjExplorerService.Visible = true;
                mobjExplorerService.Slide(true);
                //PlaceFlag = true;
                //PlaceValue = 1;
                //GTCustomCommandModeless.m_oGTTransactionManager.Rollback();
            }
            catch (Exception ex) { }
        }
        private string Gen_Code(string MAINFDP_CODE, string FDC_CODE)
        {
           
                string code = "";
                try
                {
                //GC_FDP -- 5600 FDP
                int maxNum = 0;
                string sSql = "";
                ADODB.Recordset rsPP = new ADODB.Recordset();
                string Exch = Get_Value("Select EXC_ABB from G3E_JOB where G3E_IDENTIFIER = '" + m_gtapp.DataContext.ActiveJob.ToString() + "'");
                if (FNO == 5600 || FNO == 9900 || FNO == 6800)
                {
                    sSql = "select FDP_CODE as code from GC_FDP A, GC_NETELEM B where A.G3E_FID = B.G3E_FID and B.EXC_ABB = '" + Exch + "' and A.fdc_code= '" + FDC_CODE + "'"
                         + " union all "
                         + "select  DB_CODE as code from GC_DB A, GC_NETELEM B where A.G3E_FID = B.G3E_FID and B.EXC_ABB = '" + Exch + "' and A.fdc_code='" + FDC_CODE + "'"
                    +" union all "
                        + "select  FDP_CODE as code from GC_PFDP A, GC_NETELEM B where A.G3E_FID = B.G3E_FID and B.EXC_ABB = '" + Exch + "' and A.fdc_code='" + FDC_CODE + "'";
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
                        if (maxNum.ToString().Length == 1) code = "000" + maxNum.ToString();
                        else if (maxNum.ToString().Length == 2) code = "00" + maxNum.ToString();
                        else if (maxNum.ToString().Length == 3) code = "0" + maxNum.ToString();
                        else if (maxNum.ToString().Length == 4) code = maxNum.ToString();
                        return code;
                    }
                    else return "0001";
                }
                else  //GC_TIEFDP -- 5800 TIE FDP
                    if (FNO == 5800 )
                    {
                        if (MAINFDP_CODE != "")
                        {
                            sSql = "select lett from "
                                + "(select  substr(FDP_CODE,-1,length(FDP_CODE)) as lett from GC_TIEFDP A, GC_NETELEM B where A.G3E_FID = B.G3E_FID and B.EXC_ABB = '" + Exch + "'  and A.fdc_code= '" + FDC_CODE + "' and MAINFDP_CODE = '" + MAINFDP_CODE + "' "
                                + ") order by lett asc";
                            rsPP = m_GTDataContext.OpenRecordset(sSql, ADODB.CursorTypeEnum.adOpenDynamic, ADODB.LockTypeEnum.adLockOptimistic, (int)ADODB.CommandTypeEnum.adCmdText);
                            if (rsPP.RecordCount == 0)
                            {
                                code = MAINFDP_CODE + "A";
                                return code;
                            }
                            else
                            {
                                rsPP.MoveLast();
                                for (int i = rsPP.RecordCount-1; i >=0; i++)
                                {
                                    char tempchar = 'A';
                                    if (char.TryParse(rsPP.Fields[0].Value.ToString(), out tempchar))
                                    {
                                        char letter = tempchar;
                                        letter++;
                                        code = MAINFDP_CODE + letter;
                                        return code;
                                    }
                                    rsPP.MovePrevious();
                                }   
                                return code;
                            }
                        }
                    }
                    else  //FTB
                        if (FNO == 5900)
                        {
                            if (MAINFDP_CODE != "")
                            {
                                sSql = "select lett from "
                                    + "( "
                                    + "select  substr(FDP_CODE,-1,length(FDP_CODE)) as lett from GC_FTB A, GC_NETELEM B where A.G3E_FID = B.G3E_FID and B.EXC_ABB = '" + Exch + "'  and A.fdc_code= '" + FDC_CODE + "' and MAINFDP_CODE = '" + MAINFDP_CODE + "' "
                                    + ") order by lett asc";
                                rsPP = m_GTDataContext.OpenRecordset(sSql, ADODB.CursorTypeEnum.adOpenDynamic, ADODB.LockTypeEnum.adLockOptimistic, (int)ADODB.CommandTypeEnum.adCmdText);
                                if (rsPP.RecordCount == 0)
                                {
                                    code = MAINFDP_CODE + "A";
                                    return code;
                                }
                                else
                                {
                                    rsPP.MoveLast();
                                    for (int i = rsPP.RecordCount - 1; i >= 0; i++)
                                    {
                                        char tempchar = 'A';
                                        if (char.TryParse(rsPP.Fields[0].Value.ToString(), out tempchar))
                                        {
                                            char letter = tempchar;
                                            letter++;
                                            code = MAINFDP_CODE + letter;
                                            return code;
                                        }
                                        rsPP.MovePrevious();
                                    }
                                    return code;
                                }
                            }
                        }
            }
            catch (Exception ex) { }
            return code;
        }

        private void SaveAttribute()
        {
            if (ValidationCode())
            {
               // GTCustomCommandModeless.m_oGTTransactionManager.Rollback();
                if (!(mobjExplorerService == null))
                {
                    mobjExplorerService.Visible = false;
                    mobjExplorerService.Clear();
                }
                mobjExplorerService.CancelClick -= new EventHandler(mobjExplorerService_CancelClick);
                mobjExplorerService.SaveAndContinueClick -= new EventHandler(mobjExplorerService_SaveAndContinueClick);
                mobjExplorerService.SaveClick -= new EventHandler(mobjExplorerService_SaveClick);
                mobjExplorerService = null;
              //  PlaceFlag = true;
              //  PlaceValue = 1;
                PlaceFlag = true;
                PlaceValue = 1;
                GTCustomCommandModeless.m_oGTTransactionManager.Rollback();
            }
        }

         private void mobjExplorerService_CancelClick(object sender, EventArgs e)
        {
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
                mobjFDPAttribute = null;
                MessageBox.Show("Placement is canceled!", "Place FDP", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Show();

            }
            catch (Exception ex)
            {
                
            }
        }

        private void mobjExplorerService_SaveAndContinueClick(object sender, EventArgs e)
        {
            
            try
            {
                SaveAttribute();
            }
            catch (Exception ex)
            {
                
            }

        }
        

        private void mobjExplorerService_SaveClick(object sender, EventArgs e)
        {
            try
            {
                SaveAttribute();
            }
            catch (Exception ex)
            {
               
            }

        }
#endregion


        public void DrawFDP(short iFNO, int CabelFID, short CabelFNO, bool copy)
        {
            try
            {
                int iFID = 0;
                int iSplitFID = 0;
                IGTKeyObject oNewFeature;
                IGTKeyObject oNewSplitter1;
                IGTKeyObject oNewSplitter2;
                IGTTextPointGeometry oTextGeom;
                IGTPointGeometry oPointGeom;
                string SPLITTER_TYPE = "";

                GTCustomCommandModeless.m_oGTTransactionManager.Begin("DrawFDP");

                oNewFeature = m_gtapp.DataContext.NewFeature(iFNO);
                iFID = oNewFeature.FID;
                if (copy)
                {
                    try
                    {
                        mobjFDPAttribute = null;
                        mobjFDPAttribute = m_gtapp.DataContext.OpenFeature(CopyFNO, CopyFID);
                    }
                    catch (Exception ex) { }
                }
                #region Attributes

                
                if (!mobjFDPAttribute.Components.GetComponent(51).Recordset.EOF)
                        {
                            if (oNewFeature.Components.GetComponent(51).Recordset.EOF)
                            {
                                oNewFeature.Components.GetComponent(51).Recordset.AddNew("G3E_FID", iFID);
                                oNewFeature.Components.GetComponent(51).Recordset.Update("G3E_FNO", iFNO);
                            }
                            else
                            {
                                oNewFeature.Components.GetComponent(51).Recordset.MoveLast();
                            }

                            for (int j = 0; j < mobjFDPAttribute.Components.GetComponent(51).Recordset.Fields.Count; j++)
                            {
                                if ((mobjFDPAttribute.Components.GetComponent(51).Recordset.Fields[j].Name != "G3E_FID") &&
                                    (mobjFDPAttribute.Components.GetComponent(51).Recordset.Fields[j].Name != "G3E_FNO") &&
                                    (mobjFDPAttribute.Components.GetComponent(51).Recordset.Fields[j].Name != "G3E_CNO") &&
                                    (mobjFDPAttribute.Components.GetComponent(51).Recordset.Fields[j].Name != "G3E_CID") &&
                                    (mobjFDPAttribute.Components.GetComponent(51).Recordset.Fields[j].Name != "G3E_ID")
                                )
                                    oNewFeature.Components.GetComponent(51).Recordset.Update(mobjFDPAttribute.Components.GetComponent(51).Recordset.Fields[j].Name, mobjFDPAttribute.Components.GetComponent(51).Recordset.Fields[j].Value);
                            }
                        }
                
                        oNewFeature.Components.GetComponent(51).Recordset.Update("G3E_FNO", iFNO);
                        oNewFeature.Components.GetComponent(51).Recordset.Update("JOB_STATE", Get_Value("Select JOB_STATE from G3E_JOB where G3E_IDENTIFIER = '" + m_gtapp.DataContext.ActiveJob.ToString() + "'"));
                        oNewFeature.Components.GetComponent(51).Recordset.Update("EXC_ABB", Get_Value("Select EXC_ABB from G3E_JOB where G3E_IDENTIFIER = '" + m_gtapp.DataContext.ActiveJob.ToString() + "'"));
                        oNewFeature.Components.GetComponent(51).Recordset.Update("SCHEME_NAME", Get_Value("Select SCHEME_NAME from G3E_JOB where G3E_IDENTIFIER = '" + m_gtapp.DataContext.ActiveJob.ToString() + "'"));
                        oNewFeature.Components.GetComponent(51).Recordset.Update("JOB_ID", Get_Value("Select SCHEME_NAME from G3E_JOB where G3E_IDENTIFIER = '" + m_gtapp.DataContext.ActiveJob.ToString() + "'"));
                        oNewFeature.Components.GetComponent(51).Recordset.Update("FEATURE_STATE", "PPF");

                        if (!mobjFDPAttribute.Components.GetComponent(A_CNO).Recordset.EOF)
                        {
                            if (oNewFeature.Components.GetComponent(A_CNO).Recordset.EOF)
                            {
                                oNewFeature.Components.GetComponent(A_CNO).Recordset.AddNew("G3E_FID", iFID);
                                oNewFeature.Components.GetComponent(A_CNO).Recordset.Update("G3E_FNO", iFNO);
                            }
                            else
                            {
                                oNewFeature.Components.GetComponent(A_CNO).Recordset.MoveLast();
                            }
                            string FDC_Code_copy="";
                            string MAINFDP_Code_copy="";
                            for (int j = 0; j < mobjFDPAttribute.Components.GetComponent(A_CNO).Recordset.Fields.Count; j++)
                            {
                                if ((mobjFDPAttribute.Components.GetComponent(A_CNO).Recordset.Fields[j].Name != "G3E_FID") &&
                                    (mobjFDPAttribute.Components.GetComponent(A_CNO).Recordset.Fields[j].Name != "G3E_FNO") &&
                                    (mobjFDPAttribute.Components.GetComponent(A_CNO).Recordset.Fields[j].Name != "G3E_CNO") &&
                                    (mobjFDPAttribute.Components.GetComponent(A_CNO).Recordset.Fields[j].Name != "G3E_CID") &&
                                    (mobjFDPAttribute.Components.GetComponent(A_CNO).Recordset.Fields[j].Name != "G3E_ID")  &&
                                    (mobjFDPAttribute.Components.GetComponent(A_CNO).Recordset.Fields[j].Name != "BND_FID")
                                )
                                    oNewFeature.Components.GetComponent(A_CNO).Recordset.Update(mobjFDPAttribute.Components.GetComponent(A_CNO).Recordset.Fields[j].Name, mobjFDPAttribute.Components.GetComponent(A_CNO).Recordset.Fields[j].Value);
                                if (copy)
                                {
                                    if (txtCopyFDPType.Text == "FDP" || txtCopyFDPType.Text == "PFDP" || txtCopyFDPType.Text == "TIE" || txtCopyFDPType.Text == "FTB")
                                    {
                                        if(mobjFDPAttribute.Components.GetComponent(A_CNO).Recordset.Fields[j].Name == "FDC_CODE")
                                            FDC_Code_copy=mobjFDPAttribute.Components.GetComponent(A_CNO).Recordset.Fields[j].Value.ToString();
                                        
                                        if(mobjFDPAttribute.Components.GetComponent(A_CNO).Recordset.Fields[j].Name == "FDP_CAPACITY")
                                            SPLITTER_TYPE=mobjFDPAttribute.Components.GetComponent(A_CNO).Recordset.Fields[j].Value.ToString();
                                    }
                                    if ( txtCopyFDPType.Text == "DB")
                                    {
                                        if (mobjFDPAttribute.Components.GetComponent(A_CNO).Recordset.Fields[j].Name == "FDC_CODE")
                                            FDC_Code_copy = mobjFDPAttribute.Components.GetComponent(A_CNO).Recordset.Fields[j].Value.ToString();
                                        if (mobjFDPAttribute.Components.GetComponent(A_CNO).Recordset.Fields[j].Name == "DB_CAPACITY")
                                            SPLITTER_TYPE = mobjFDPAttribute.Components.GetComponent(A_CNO).Recordset.Fields[j].Value.ToString();
                                       
                                    }
                                    if (txtCopyFDPType.Text == "TIE" || txtCopyFDPType.Text == "FTB")
                                    {
                                        if (mobjFDPAttribute.Components.GetComponent(A_CNO).Recordset.Fields[j].Name == "MAINFDP_CODE")
                                            MAINFDP_Code_copy = mobjFDPAttribute.Components.GetComponent(A_CNO).Recordset.Fields[j].Value.ToString();
                                    }
                                }

                              
                            }
                            if (copy)
                            {
                                if (txtCopyFDPType.Text == "FDP" || txtCopyFDPType.Text == "FOMS" || txtCopyFDPType.Text == "PFDP")
                                    oNewFeature.Components.GetComponent(A_CNO).Recordset.Update("FDP_CODE", Gen_Code("", FDC_Code_copy));
                                else if(txtCopyFDPType.Text == "DB")
                                    oNewFeature.Components.GetComponent(A_CNO).Recordset.Update("DB_CODE", Gen_Code("", FDC_Code_copy));                                
                                else if (txtCopyFDPType.Text == "TIE" || txtCopyFDPType.Text == "FTB")
                                    oNewFeature.Components.GetComponent(A_CNO).Recordset.Update("FDP_CODE", Gen_Code(MAINFDP_Code_copy, FDC_Code_copy));
                               
                            }
                        }
                    
                //64 GC_OWNERSHIP
                        if (cmbFeature.Text == "FDP" || cmbFeature.Text == "FOMS" || cmbFeature.Text == "DB" || cmbFeature.Text == "PFDP")
                {
                    if (oNewFeature.Components.GetComponent(64).Recordset.EOF)
                    {
                        oNewFeature.Components.GetComponent(64).Recordset.AddNew("G3E_FID", iFID);
                        oNewFeature.Components.GetComponent(64).Recordset.Update("G3E_FNO", iFNO);
                        oNewFeature.Components.GetComponent(64).Recordset.Update("G3E_CID", "1");
                    }
                }
                if (CabelFID != 0)//output for d-side cabel
                {
                    IGTRelationshipService mobjRelationshipService1;
                    mobjRelationshipService1 = GTClassFactory.Create<IGTRelationshipService>(GTCustomCommandModeless.m_oGTCustomCommandHelper);
                    mobjRelationshipService1.DataContext = m_gtapp.DataContext;


                    IGTKeyObject oCabelFeature = m_gtapp.DataContext.OpenFeature(CabelFNO, CabelFID);
                    mobjRelationshipService1.ActiveFeature = oNewFeature;

                    if (mobjRelationshipService1.AllowSilentEstablish(oCabelFeature))
                    {
                        mobjRelationshipService1.SilentEstablish(13, oCabelFeature);
                    }
                    mobjRelationshipService1.ActiveFeature = oCabelFeature;

                    if (mobjRelationshipService1.AllowSilentEstablish(oNewFeature))
                    {
                        mobjRelationshipService1.SilentEstablish(8, oNewFeature);
                    }
                    //if (cmbFeature.Text == "FDP")
                    //{
                    //    ADODB.Recordset test = new ADODB.Recordset();
                    //    test = oNewFeature.Components.GetComponent(A_CNO).Recordset;
                    //    string FCABLE_CODE = Get_Value("select FCABLE_CODE from GC_FDCBL where g3e_fid = " + CabelFID.ToString());
                    //    oNewFeature.Components.GetComponent(A_CNO).Recordset.Update("FIB_DCABLE", FCABLE_CODE);
                    //}

                   
                    
                }
                #endregion

                #region Geometry

                oPointGeom = GTClassFactory.Create<IGTPointGeometry>();
                oPointGeom.Origin = GTCustomCommandModeless.oPointPoint.FirstPoint;

                if (oNewFeature.Components.GetComponent(G_CNO).Recordset.EOF)
                {
                    oNewFeature.Components.GetComponent(G_CNO).Recordset.AddNew("G3E_FID", iFID);
                    oNewFeature.Components.GetComponent(G_CNO).Recordset.Update("G3E_FNO", iFNO);
                }
                else
                {
                    oNewFeature.Components.GetComponent(G_CNO).Recordset.MoveLast();
                }
                oNewFeature.Components.GetComponent(G_CNO).Geometry = oPointGeom;

                #endregion

                #region TextGeometry

                oTextGeom = GTClassFactory.Create<IGTTextPointGeometry>();
                oTextGeom.Origin = GTCustomCommandModeless.oTextPoint.FirstPoint;
               // L_CNO = 5630;
                if (oNewFeature.Components.GetComponent(L_CNO).Recordset.EOF)
                {
                    oNewFeature.Components.GetComponent(L_CNO).Recordset.AddNew("G3E_FID", iFID);
                    oNewFeature.Components.GetComponent(L_CNO).Recordset.Update("G3E_FNO", iFNO);
                }
                else
                {
                    oNewFeature.Components.GetComponent(L_CNO).Recordset.MoveLast();
                }
                oNewFeature.Components.GetComponent(L_CNO).Geometry = oTextGeom;

                #endregion
                

                    #region SPLITTER/OWNERSHIP

                    string MIN_MATERIAL = null;
                    string SPLITTER_CLASS = null;

                    //SPLITTER1
                    if (cmbFeature.Text == "FDP" || cmbFeature.Text == "FOMS" || cmbFeature.Text == "PFDP" || cmbFeature.Text == "DB")
                    {
                        int recordsAffected = 0;
                        this.m_GTDataContext = m_gtapp.DataContext;
                        string tempforDB = "";
                        if (!copy)
                            SPLITTER_TYPE = cmbSplitter1.Text;
                        if( cmbFeature.Text == "DB" && copy)
                        {
                            int l = SPLITTER_TYPE.Trim().IndexOf(",");
                            if (l > 0)
                            {
                                tempforDB = SPLITTER_TYPE;
                                SPLITTER_TYPE = SPLITTER_TYPE.Substring(0, l).Trim();
                               
                            }
                        }
                        string get_splitter = "select * from REF_FSPLITTER where SPLITTER_CLASS='NORMAL' and  SPLITTER_TYPE = '" + SPLITTER_TYPE + "'";

                        Recordset rsComp = m_GTDataContext.Execute(get_splitter, out recordsAffected, (int)CommandTypeEnum.adCmdText, null);
                        if (rsComp != null && rsComp.RecordCount > 0)
                        {
                            rsComp.MoveFirst();
                            MIN_MATERIAL = rsComp.Fields["MIN_MATERIAL"].Value.ToString();
                            SPLITTER_TYPE = rsComp.Fields["SPLITTER_TYPE"].Value.ToString();
                            SPLITTER_CLASS = rsComp.Fields["SPLITTER_CLASS"].Value.ToString();
                        }
                        rsComp = null;

                        oNewSplitter1 = m_gtapp.DataContext.NewFeature(12300);
                        iSplitFID = oNewSplitter1.FID;

                        // NETELEM 51
                        oNewSplitter1.Components.GetComponent(51).Recordset.Update("JOB_STATE", "PROPOSED");
                        oNewSplitter1.Components.GetComponent(51).Recordset.Update("FEATURE_STATE", "PPF");
                        oNewSplitter1.Components.GetComponent(51).Recordset.Update("MIN_MATERIAL", MIN_MATERIAL);

                        //ATTRIBUTE
                        if (oNewSplitter1.Components.GetComponent(12301).Recordset.EOF)
                        {
                            oNewSplitter1.Components.GetComponent(12301).Recordset.AddNew("G3E_FID", iSplitFID);
                            oNewSplitter1.Components.GetComponent(12301).Recordset.Update("SPLITTER_TYPE", SPLITTER_TYPE);
                            oNewSplitter1.Components.GetComponent(12301).Recordset.Update("SPLITTER_CLASS", SPLITTER_CLASS);
                            oNewSplitter1.Components.GetComponent(12301).Recordset.Update("NO_OF_SPLITTER", "1");
                            oNewSplitter1.Components.GetComponent(12301).Recordset.Update("CAPACITY", "0");
                            oNewSplitter1.Components.GetComponent(12301).Recordset.Update("PORT_NUM", "0");
                            oNewSplitter1.Components.GetComponent(12301).Recordset.Update("SPLITTER_CODE", "SP01");
                        }
                        else
                        {
                            oNewSplitter1.Components.GetComponent(12301).Recordset.Update("G3E_FID", iSplitFID);
                            oNewSplitter1.Components.GetComponent(12301).Recordset.Update("SPLITTER_TYPE", SPLITTER_TYPE);
                            oNewSplitter1.Components.GetComponent(12301).Recordset.Update("SPLITTER_CLASS", SPLITTER_CLASS);
                            oNewSplitter1.Components.GetComponent(12301).Recordset.Update("NO_OF_SPLITTER", "1");
                            oNewSplitter1.Components.GetComponent(12301).Recordset.Update("CAPACITY", "0");
                            oNewSplitter1.Components.GetComponent(12301).Recordset.Update("PORT_NUM", "0");
                            oNewSplitter1.Components.GetComponent(12301).Recordset.Update("SPLITTER_CODE", "SP01");

                            
                        }
                        //realtionship btw fdp and splitter1
                        IGTRelationshipService mobjRelationshipService;
                        mobjRelationshipService = GTClassFactory.Create<IGTRelationshipService>(GTCustomCommandModeless.m_oGTCustomCommandHelper);
                        mobjRelationshipService.DataContext = m_gtapp.DataContext;

                        short mRelationshipNumber = 2;
                        //Ownership    
                        oNewFeature = m_gtapp.DataContext.OpenFeature(iFNO, iFID);
                        mobjRelationshipService.ActiveFeature = oNewFeature;

                        if (mobjRelationshipService.AllowSilentEstablish(oNewSplitter1))
                        {
                            mobjRelationshipService.SilentEstablish(mRelationshipNumber, oNewSplitter1);
                        }
                        mRelationshipNumber = 3;
                        //Ownership        
                        mobjRelationshipService.ActiveFeature = oNewSplitter1;

                        if (mobjRelationshipService.AllowSilentEstablish(oNewFeature))
                        {
                            mobjRelationshipService.SilentEstablish(mRelationshipNumber, oNewFeature);
                        }
                        if(tempforDB !="")
                            SPLITTER_TYPE = tempforDB;
                        
                    }

                    //SPLITTER2
                    if (cmbFeature.Text == "DB")
                    {
                        int recordsAffected = 0;
                        this.m_GTDataContext = m_gtapp.DataContext;
                        if (!copy)
                            SPLITTER_TYPE = cmbSplitter2.Text;
                        else 
                        {
                            int l = SPLITTER_TYPE.Trim().IndexOf(",");
                            if (l > 0)
                            {
                                SPLITTER_TYPE = SPLITTER_TYPE.Substring(l + 1).Trim();

                            }
                        }

                        Recordset rsComp = m_GTDataContext.Execute("select * from REF_FSPLITTER where SPLITTER_CLASS='NORMAL' and SPLITTER_TYPE	= '" + SPLITTER_TYPE + "'", out recordsAffected, (int)CommandTypeEnum.adCmdText, null);
                        if (rsComp != null && rsComp.RecordCount > 0)
                        {
                            rsComp.MoveFirst();
                            MIN_MATERIAL = rsComp.Fields["MIN_MATERIAL"].Value.ToString();
                            SPLITTER_TYPE = rsComp.Fields["SPLITTER_TYPE"].Value.ToString();
                            SPLITTER_CLASS = rsComp.Fields["SPLITTER_CLASS"].Value.ToString();
                        }
                        rsComp = null;

                        oNewSplitter2 = m_gtapp.DataContext.NewFeature(12300);
                        iSplitFID = oNewSplitter2.FID;

                        // NETELEM 51
                        oNewSplitter2.Components.GetComponent(51).Recordset.Update("JOB_STATE", "PROPOSED");
                        oNewSplitter2.Components.GetComponent(51).Recordset.Update("FEATURE_STATE", "PPF");
                        oNewSplitter2.Components.GetComponent(51).Recordset.Update("MIN_MATERIAL", MIN_MATERIAL);

                        //ATTRIBUTE
                        if (oNewSplitter2.Components.GetComponent(12301).Recordset.EOF)
                        {
                            oNewSplitter2.Components.GetComponent(12301).Recordset.AddNew("G3E_FID", iSplitFID);
                            oNewSplitter2.Components.GetComponent(12301).Recordset.Update("SPLITTER_TYPE", SPLITTER_TYPE);
                            oNewSplitter2.Components.GetComponent(12301).Recordset.Update("SPLITTER_CLASS", SPLITTER_CLASS);
                            oNewSplitter2.Components.GetComponent(12301).Recordset.Update("NO_OF_SPLITTER", "2");
                            oNewSplitter2.Components.GetComponent(12301).Recordset.Update("CAPACITY", "0");
                            oNewSplitter2.Components.GetComponent(12301).Recordset.Update("PORT_NUM", "0");
                            oNewSplitter2.Components.GetComponent(12301).Recordset.Update("SPLITTER_CODE", "SP02");
                        }
                        else
                        {
                            oNewSplitter2.Components.GetComponent(12301).Recordset.Update("G3E_FID", iSplitFID);
                            oNewSplitter2.Components.GetComponent(12301).Recordset.Update("SPLITTER_TYPE", SPLITTER_TYPE);
                            oNewSplitter2.Components.GetComponent(12301).Recordset.Update("SPLITTER_CLASS", SPLITTER_CLASS);
                            oNewSplitter2.Components.GetComponent(12301).Recordset.Update("NO_OF_SPLITTER", "2");
                            oNewSplitter2.Components.GetComponent(12301).Recordset.Update("CAPACITY", "0");
                            oNewSplitter2.Components.GetComponent(12301).Recordset.Update("PORT_NUM", "0");
                            oNewSplitter2.Components.GetComponent(12301).Recordset.Update("SPLITTER_CODE", "SP02");
                        }


                        //realtionship btw fdp and splitter2
                        IGTRelationshipService mobjRelationshipService;
                        mobjRelationshipService = GTClassFactory.Create<IGTRelationshipService>(GTCustomCommandModeless.m_oGTCustomCommandHelper);
                        mobjRelationshipService.DataContext = m_gtapp.DataContext;
                        short mRelationshipNumber = 2;
                        //Ownership    
                        oNewFeature = m_gtapp.DataContext.OpenFeature(iFNO, iFID);
                        mobjRelationshipService.ActiveFeature = oNewFeature;

                        if (mobjRelationshipService.AllowSilentEstablish(oNewSplitter2))
                        {
                            mobjRelationshipService.SilentEstablish(mRelationshipNumber, oNewSplitter2);
                        }
                        mRelationshipNumber = 3;
                        //Ownership        
                        mobjRelationshipService.ActiveFeature = oNewSplitter2;

                        if (mobjRelationshipService.AllowSilentEstablish(oNewFeature))
                        {
                            mobjRelationshipService.SilentEstablish(mRelationshipNumber, oNewFeature);
                        }
                        ////OWNERSHIP
                        //string G3E_ID = Get_Value("select G3E_ID from GC_OWNERSHIP where g3e_fno = " + FNO + " and g3e_fid =" + iFID.ToString());
                        //if (oNewSplitter2.Components.GetComponent(64).Recordset.EOF)
                        //{
                        //    oNewSplitter2.Components.GetComponent(64).Recordset.AddNew("G3E_FID", iSplitFID);
                        //    oNewSplitter2.Components.GetComponent(64).Recordset.Update("OWNER1_ID", G3E_ID);
                        //}
                        //else
                        //{
                        //    oNewSplitter2.Components.GetComponent(64).Recordset.Update("G3E_FID", iSplitFID);
                        //    oNewSplitter2.Components.GetComponent(64).Recordset.Update("OWNER1_ID", G3E_ID);
                        //}
                    }

                    #endregion


                    //MessageBox.Show("Completed", "Place FDP", MessageBoxButtons.OK, MessageBoxIcon.Information);
                

                GTCustomCommandModeless.m_oGTTransactionManager.Commit();
                GTCustomCommandModeless.m_oGTTransactionManager.RefreshDatabaseChanges();
                if (GTCustomCommandModeless.mobjEditServiceTemp != null)
                {
                    GTCustomCommandModeless.mobjEditServiceTemp.RemoveAllGeometries();
                    //GTCustomCommandModeless.mobjEditServiceTemp = null;
                }
                if (GTCustomCommandModeless.mobjEditServicePoint != null)
                {
                    GTCustomCommandModeless.mobjEditServicePoint.RemoveAllGeometries();
                    //GTCustomCommandModeless.mobjEditServicePoint = null;
                }
                if (GTCustomCommandModeless.mobjEditServiceText != null)
                {
                    GTCustomCommandModeless.mobjEditServiceText.RemoveAllGeometries();
                    //GTCustomCommandModeless.mobjEditServiceText = null;
                }

                PlacedFID = iFID;
                //this.Hide();

                MessageBox.Show("New FDP is successfully placed!", "Place FDP", MessageBoxButtons.OK, MessageBoxIcon.Information);

            }
            catch (Exception ex)
            {
                log.WriteLog("");
                log.WriteLog(this.GetType().FullName, "ERROR: " + ex.Message);
                log.WriteLog(ex.StackTrace);
                GTCustomCommandModeless.m_oGTTransactionManager.Rollback();
                MessageBox.Show(ex.Message, "Place FDP", MessageBoxButtons.OK, MessageBoxIcon.Error);

                if (GTCustomCommandModeless.mobjEditServiceTemp != null)
                {
                    GTCustomCommandModeless.mobjEditServiceTemp.RemoveAllGeometries();
                   //GTCustomCommandModeless.mobjEditServiceTemp = null;
                }
                if (GTCustomCommandModeless.mobjEditServicePoint != null)
                {
                    GTCustomCommandModeless.mobjEditServicePoint.RemoveAllGeometries();
                    //GTCustomCommandModeless.mobjEditServicePoint = null;
                }
                if (GTCustomCommandModeless.mobjEditServiceText != null)
                {
                    GTCustomCommandModeless.mobjEditServiceText.RemoveAllGeometries();
                    //GTCustomCommandModeless.mobjEditServiceText = null;
                }
            }
        }
        //----//
        public void CopyFDP()
        {
            try
            {
                mobjFDPAttribute = null;
                mobjFDPAttribute = m_gtapp.DataContext.OpenFeature(CopyFNO, CopyFID);
                mobjFDPAttribute.Components.GetComponent(51).Recordset.Update("JOB_STATE",Get_Value("Select JOB_STATE from G3E_JOB where G3E_IDENTIFIER = '" + m_gtapp.DataContext.ActiveJob.ToString() + "'"));
                mobjFDPAttribute.Components.GetComponent(51).Recordset.Update("EXC_ABB",Get_Value("Select EXC_ABB from G3E_JOB where G3E_IDENTIFIER = '" + m_gtapp.DataContext.ActiveJob.ToString() + "'"));
                mobjFDPAttribute.Components.GetComponent(51).Recordset.Update("SCHEME_NAME",Get_Value("Select SCHEME_NAME from G3E_JOB where G3E_IDENTIFIER = '" + m_gtapp.DataContext.ActiveJob.ToString() + "'"));
                mobjFDPAttribute.Components.GetComponent(51).Recordset.Update("JOB_ID",Get_Value("Select SCHEME_NAME from G3E_JOB where G3E_IDENTIFIER = '" + m_gtapp.DataContext.ActiveJob.ToString() + "'"));
                mobjFDPAttribute.Components.GetComponent(51).Recordset.Update("FEATURE_STATE", "PPF");

            }
            catch (Exception ex) { }
        }
        
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

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
                
        private void Get_X_Y_Feature(int FID)
        {
            string sSql = "SELECT T.X, T.Y FROM GC_BND_P A, TABLE(SDO_UTIL.GETVERTICES(A.G3E_GEOMETRY)) T WHERE A.G3E_FID ="+ FID;
            ADODB.Recordset rs = new ADODB.Recordset();           
            rs = m_gtapp.DataContext.OpenRecordset(sSql, ADODB.CursorTypeEnum.adOpenStatic, ADODB.LockTypeEnum.adLockReadOnly, 1);
            if (rs.RecordCount > 0)
            {
                rs.MoveFirst();

                do
                {
                    if (ParentXY == "")
                    {
                        ParentXY = rs.Fields[0].Value.ToString() + "," + rs.Fields[1].Value.ToString() + ",0";
                    }
                    else
                    {
                        ParentXY = ParentXY + "," + rs.Fields[0].Value.ToString() + "," + rs.Fields[1].Value.ToString() + ",0";
                    }
                    rs.MoveNext();
                } 
                while (!rs.EOF);
            }
            rs = null;
        }

        
        private void clear()
        {
            PlaceFlag = false;
            PlaceValue = 0;
            cmbFeature.Items.Clear();
            cmbType.Items.Clear();
            cmbSplitter1.Items.Clear();
            cmbSplitter2.Items.Clear();
            Load_Combo(cmbFeature, "SELECT DISTINCT FEATURE FROM FDP_TYPE");

            lblSplitter1.Visible = false;
            lblSplitter2.Visible = false;
            cmbSplitter1.Visible = false;
            cmbSplitter2.Visible = false;

            txtCopyFDPCode.Text = "";

            txtParant.Text = "";
            txtParantCode.Text = "";
            groupBox2.Text = "Parent Device:";
            lblParentDevice.Text = "";
            txtCopyFDPType.Text = "";
            txtCopyFDPFID.Text = "";
            txtCopyFDPCode.Text = "";
        }
                
        int CopyFID = 0;
        short CopyFNO = 0;
        public IGTGeometry oCopyBoundry;

        private void btn_Clear_Click(object sender, EventArgs e)
        {
            clear();
        }

        private void btn_Pick_Click(object sender, EventArgs e)
        {
            this.Hide();
            m_gtapp.SelectedObjects.Clear();
            m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Point to select " + lblParentDevice.Text + "!Right click to cancel selection");
            vPARENT = lblParentDevice.Text;
            PlaceValue = 100;

            txtParantCode.Text = "";
            txtParant.Text = "";  
           // PickFDC();
        }

        public void PickFDC()
        {
            short iFNO = 0;
            int iFID = 0;

            if (cmbFeature.Text == "")
            {
                MessageBox.Show("Please select a Feature Type", "Place FDP", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (m_gtapp.SelectedObjects.FeatureCount == 0)
            {
                MessageBox.Show("Please select a Feature", "Place FDP", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            foreach (IGTDDCKeyObject oDDCKeyObject in m_gtapp.SelectedObjects.GetObjects())
            {
               
                iFNO = oDDCKeyObject.FNO;
                iFID = oDDCKeyObject.FID;

                if (cmbFeature.Text == "FDP" || cmbFeature.Text == "FOMS" || cmbFeature.Text == "PFDP" || cmbFeature.Text == "DB")         
            {
                if (iFNO == 5100)
                    txtParantCode.Text = Get_Value("Select FDC_CODE from gc_fdc where g3e_fid = " + iFID.ToString());
                else break;
            }
            else if (cmbFeature.Text == "TIE") 
            {
                if (iFNO == 5600)
                    txtParantCode.Text = Get_Value("Select FDP_CODE from gc_fdp where g3e_fid = " + iFID.ToString());
                else break;
            }
            else if (cmbFeature.Text == "FTB")
            {
                if (iFNO == 6800)
                    txtParantCode.Text = Get_Value("Select FDP_CODE from gc_pfdp where g3e_fid = " + iFID.ToString());
                else break;
            }
          
            txtParant.Text = iFID.ToString();          
                  
            }

            if (txtParant.Text == "")
            {
                MessageBox.Show("Please select a Parent Device : " + lblParentDevice.Text, "Place FDP", MessageBoxButtons.OK, MessageBoxIcon.Information);
                m_gtapp.SelectedObjects.Clear();
                return;
            }
            this.Show();
            PlaceValue = 0;
        }
//-------------------//

        private void button1_Click_1(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btn_Clear1_Click(object sender, EventArgs e)
        {
            clear();
        }

        private void btn_PickCopy_Click(object sender, EventArgs e)
        {
            this.Hide();
            m_gtapp.SelectedObjects.Clear();
            m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Point to select FDP for copying!Right click to cancel selection");
            PlaceValue = 200;            
        }
        public void PickCopyFDP()
        {
            short iFNO = 0;
            int iFID = 0;

            if (m_gtapp.SelectedObjects.FeatureCount == 0)
            {
                MessageBox.Show("Please select a Feature for copying", "Copy FDP", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            foreach (IGTDDCKeyObject oDDCKeyObject in m_gtapp.SelectedObjects.GetObjects())
            {
                iFNO = oDDCKeyObject.FNO;
                iFID = oDDCKeyObject.FID;

                if (iFNO == 5600 || iFNO == 5800 || iFNO == 5900 || iFNO == 9900 || iFNO == 6800)
                {
                    if (iFNO == 5600)
                    {
                        txtCopyFDPCode.Text = Get_Value("SELECT FDP_CODE FROM GC_FDP WHERE G3E_FID =" + iFID);
                        txtCopyFDPFID.Text = iFID.ToString();
                        txtCopyFDPType.Text = "FDP";
                        CopyFID = iFID;
                        CopyFNO = 5600;
                        btn_CopyBND.Text = "Copy FDP";

                        A_CNO = 5601;
                        G_CNO = 5620;
                        L_CNO = 5630;
                    }
                    else if (iFNO == 6800)
                    {
                        txtCopyFDPCode.Text = Get_Value("SELECT FDP_CODE FROM GC_PFDP WHERE G3E_FID =" + iFID);
                        txtCopyFDPFID.Text = iFID.ToString();
                        txtCopyFDPType.Text = "PFDP";
                        CopyFID = iFID;
                        CopyFNO = 6800;
                        btn_CopyBND.Text = "Copy PFDP";

                        A_CNO = 6801;
                        G_CNO = 6820;
                        L_CNO = 6830;
                    }
                    else if (iFNO == 5800)
                    {
                        txtCopyFDPCode.Text = Get_Value("SELECT FDP_CODE FROM GC_TIEFDP WHERE G3E_FID =" + iFID);
                        CopyFID = iFID;
                        CopyFNO = 5800;
                        btn_CopyBND.Text = "Copy TIE FDP";
                        txtCopyFDPFID.Text = iFID.ToString();
                        txtCopyFDPType.Text = "TIE";
                        A_CNO = 5801;
                        G_CNO = 5820;
                        L_CNO = 5830;
                    }
                    else if (iFNO == 5900)
                    {
                        txtCopyFDPCode.Text = Get_Value("SELECT FDP_CODE FROM GC_FTB WHERE G3E_FID =" + iFID);
                        CopyFID = iFID;
                        CopyFNO = 5900;
                        txtCopyFDPFID.Text = iFID.ToString();
                        txtCopyFDPType.Text = "FTB";
                        btn_CopyBND.Text = "Copy FTB";

                        A_CNO = 5901;
                        G_CNO = 5920;
                        L_CNO = 5930;
                    }
                    else if (iFNO == 9900)
                    {
                        txtCopyFDPCode.Text = Get_Value("SELECT DB_CODE FROM GC_DB WHERE G3E_FID =" + iFID);
                        CopyFID = iFID;
                        CopyFNO = 9900;
                        btn_CopyBND.Text = "Copy DB";
                        txtCopyFDPFID.Text = iFID.ToString();
                        txtCopyFDPType.Text = "DB";

                        A_CNO = 9901;
                        G_CNO = 9920;
                        L_CNO = 9930;
                    }
                }
                else
                {
                    MessageBox.Show("Please select for copying only FDP features:\nFDP, PFDP, TIE FDP, DB, FTB", "Copy FDP", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    m_gtapp.SelectedObjects.Clear();
                    return;
                }
            }

            this.Show();
            PlaceValue = 0;
        }

        private void btn_CopyBND_Click(object sender, EventArgs e)
        {
            if (CopyFID == 0)
            {
                MessageBox.Show("Please Select a FDP / PFDP / TIE FDP / FTB / DB to Copy", "Copy FDP", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (txtCopyFDPCode.Text == "")
            {
                MessageBox.Show("FDP Code of selected feature is empty!\nPlease, update FDP Code before copying!", "Copy FDP", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            CopyFlag = true;
            PlaceFlag = false;
            PlaceValue = 1;
            this.Hide();
        }


        private bool ValidationCode()
        {
           string code_to_check="";
           string FDC_Code="";
           string main_fdp_code="";

           for (int j = 0; j < mobjFDPAttribute.Components.GetComponent(A_CNO).Recordset.Fields.Count; j++)
           {
               if (mobjFDPAttribute.Components.GetComponent(A_CNO).Recordset.Fields[j].Name == "FDC_CODE")
                   FDC_Code = mobjFDPAttribute.Components.GetComponent(A_CNO).Recordset.Fields[j].Value.ToString();

               if (FNO == 9900)
               {
                   if (mobjFDPAttribute.Components.GetComponent(A_CNO).Recordset.Fields[j].Name == "DB_CODE")
                       code_to_check = mobjFDPAttribute.Components.GetComponent(A_CNO).Recordset.Fields[j].Value.ToString();
               }
               else
               {
                   if (mobjFDPAttribute.Components.GetComponent(A_CNO).Recordset.Fields[j].Name == "FDP_CODE")
                       code_to_check = mobjFDPAttribute.Components.GetComponent(A_CNO).Recordset.Fields[j].Value.ToString();

               }
               if (FNO == 5800 || FNO == 5900)
               {
                   if (mobjFDPAttribute.Components.GetComponent(A_CNO).Recordset.Fields[j].Name == "MAINFDP_CODE")
                       main_fdp_code = mobjFDPAttribute.Components.GetComponent(A_CNO).Recordset.Fields[j].Value.ToString();
               }
           }


            if (code_to_check == "")
            {
                MessageBox.Show("FDP Code can not be empty", "Place FDP", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            
            string sSql = "";
            ADODB.Recordset rsPP = new ADODB.Recordset();
            string Exch = Get_Value("Select EXC_ABB from G3E_JOB where G3E_IDENTIFIER = '" + m_gtapp.DataContext.ActiveJob.ToString() + "'");
            if (FNO == 5600 || FNO == 6800 || FNO == 9900)
            {
                sSql = "select FDP_CODE from GC_FDP A, GC_NETELEM B where A.G3E_FID = B.G3E_FID and B.EXC_ABB = '" + Exch + "' and FDP_CODE = '" + code_to_check + "' and FDC_CODE = '" + FDC_Code + "'"
                 + " union all " +
                       " select DB_CODE from GC_DB A, GC_NETELEM B where A.G3E_FID = B.G3E_FID and B.EXC_ABB = '" + Exch + "' and DB_CODE = '" + code_to_check + "' and FDC_CODE = '" + FDC_Code + "'"
                +" union all " +
                       " select FDP_CODE from GC_PFDP A, GC_NETELEM B where A.G3E_FID = B.G3E_FID and B.EXC_ABB = '" + Exch + "' and FDP_CODE = '" + code_to_check + "' and FDC_CODE = '" + FDC_Code + "'";
                
                rsPP = m_GTDataContext.OpenRecordset(sSql, ADODB.CursorTypeEnum.adOpenDynamic, ADODB.LockTypeEnum.adLockOptimistic, (int)ADODB.CommandTypeEnum.adCmdText);
                if (rsPP.RecordCount > 0)
                {
                    MessageBox.Show("FDP Code already exists", "Place FDP", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;

                }
            }


            if (FNO == 5800 )
            {
                sSql = "select FDP_CODE from GC_TIEFDP A, GC_NETELEM B where A.G3E_FID = B.G3E_FID and B.EXC_ABB = '" + Exch + "' and FDP_CODE = '" + code_to_check + "' and FDC_CODE = '" + FDC_Code + "' " +
                    " and MAINFDP_CODE = '" + main_fdp_code + "'";
                rsPP = m_GTDataContext.OpenRecordset(sSql, ADODB.CursorTypeEnum.adOpenDynamic, ADODB.LockTypeEnum.adLockOptimistic, (int)ADODB.CommandTypeEnum.adCmdText);
                if (rsPP.RecordCount > 0)
                {
                    MessageBox.Show("FDP Code already exists", "Place FDP", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;

                }
            }

            if (FNO == 5900)
            {
                sSql = " select FDP_CODE from GC_FTB A, GC_NETELEM B where A.G3E_FID = B.G3E_FID and B.EXC_ABB = '" + Exch + "' and FDP_CODE = '" + code_to_check + "' and FDC_CODE = '" + FDC_Code + "' " +
                            " and MAINFDP_CODE = '" + main_fdp_code + "'";
                rsPP = m_GTDataContext.OpenRecordset(sSql, ADODB.CursorTypeEnum.adOpenDynamic, ADODB.LockTypeEnum.adLockOptimistic, (int)ADODB.CommandTypeEnum.adCmdText);
                if (rsPP.RecordCount > 0)
                {
                    MessageBox.Show("FDP Code already exists", "Place FDP", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;

                }
            }
            return true;
        }
    }
}