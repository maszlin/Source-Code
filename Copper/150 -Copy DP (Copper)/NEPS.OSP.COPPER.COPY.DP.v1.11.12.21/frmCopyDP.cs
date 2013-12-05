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

namespace NEPS.OSP.COPPER.COPY.DP
{
    public partial class frmCopyDP : Form
    {
        #region Copy DP : Variables declaration

        private IGTApplication m_gtapp = null;
        private IGTDataContext m_IGTDataContext = null;

        //temporary geometry
        private List<IGTPoint> v_PointText = new List<IGTPoint>();

        private short iCNO = 0;
        private string strSQL = null;
        private static int sFeatureFID = 0;
        private static short sFeatureFNo = 0;       // source Feature FNO
        private const int DPFNo = 13000;            // DPFNo stands for Distribution Point Feature Number
        public bool isCopyMode = false;
        public string isCableCode = "";

        // Variables from selected DP
        short v_G3E_FNO = 0, v_G3E_CNO = 0;
        string v_EXC_ABB, v_BILLING_RATE, v_ID, v_IMAP_FEATURE_ID, v_MIC, v_MIN_MATERIAL, v_OWNERSHIP, v_PLAN_ID, v_SAP_WRK_ID,
                v_SEGMENT, v_SERVICE_CODE, v_SWITCH_CENTRE_CLLI, v_COMPOSITION, v_CON_TYPE, v_DP_CLASS, v_DP_OWNER, v_DP_NUM,
                v_DP_TYPE, v_D_CAPACITY, v_INSTALL_LOC, v_ITFACE_CODE, v_RT_CODE, v_ORIGINAL_USER, v_TERM_TYPE, v_TOOL_TYPE;
        double v_G3E_CID, v_BND_FID, v_CASS_DATE, v_D_WAITER, v_EARTH_ROD, v_E_WAITER, v_FAULT_PAIR,
                v_MAINDP_IPID, v_NONUM_WAITER, v_RAD_DIST, v_RES_PAIR, v_SPARE_PAIR, v_WORK_PAIR;

        #endregion

        #region Copy DP : Form
        public frmCopyDP()
        {
            try
            {
                InitializeComponent();
                sendFormToBottomLeft();
                this.FormClosing += new FormClosingEventHandler(this.frmFrame_FormClosing);
                m_gtapp = GTClassFactory.Create<IGTApplication>().Application;
                m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Running Copy DP (Copper)...");

            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "Running Copy DP (Copper)", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void frmFrame_Shown(object sender, EventArgs e)
        {
            m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Running Copy DP (Copper)...");
        }

        private void frmFrame_Activated(object sender, EventArgs e)
        {
            m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Running Copy DP (Copper)...");
        }

        private void frmFrame_Load(object sender, EventArgs e)
        {
            this.m_IGTDataContext = GTClassFactory.Create<IGTApplication>().Application.DataContext;
            tabManage.TabPages.Remove(tabDPNum);
        }

        private void frmFrame_FormClosing(object sender, FormClosingEventArgs e)
        {
            //DialogResult retVal = MessageBox.Show(this,"Are you sure you want to exit?", "Copy DP (Copper)", MessageBoxButtons.YesNo);
            //if (retVal == DialogResult.Yes)
            //{
            //    e.Cancel = false;
            //}
            //else
            //{
            //    e.Cancel = true;
            //}
        }

        private void sendFormToBottomLeft()
        {
            Rectangle r = Screen.PrimaryScreen.WorkingArea;
            this.StartPosition = FormStartPosition.Manual;
            this.Location = new Point(Screen.PrimaryScreen.WorkingArea.Width - (int)(this.Width * 1.2),
                Screen.PrimaryScreen.WorkingArea.Height - (int)(this.Height * 1.2));
        }

        #endregion

        #region Copy DP : General Functions
        private bool isNumeric(string val, System.Globalization.NumberStyles NumberStyle)
        {
            int result;
            return int.TryParse(val, NumberStyle, System.Globalization.CultureInfo.CurrentCulture, out result);
        }

        public bool getTextValue
        {
            get
            {
                if (txtITFACE_CODE.Text.Trim().Length == 0 && txtRT_CODE.Text.Trim().Length == 0)
                {
                    this.lblStatus.Text = "Get Parent for DP";
                    return false;
                }
                else if (txtDPNumSelected.Text.Trim().Length == 0)
                {
                    this.lblStatus.Text = "Select DP to be copied\r\nOr update parent attribute of the DP";
                    return false;
                }
                else if (txtDPNumNew.Text.Trim().Length == 0)
                {
                    this.lblStatus.Text = "Enter next DP Number";
                    return false;
                }
                else
                    return true;
            }
        }

        #endregion

        #region Copy DP : Get Selected Feature Info
        private void cmdChgGetSelected_Click(object sender, EventArgs e)
        {
            try
            {
                #region Copy DP : Initialize

                IGTGeometry selGeom = null;     // selected Geometry
                IGTSelectedObjects selFeature;  // selected Feature (any object on the map)

                lblStatus.Text = "Reading DP properties ...";
                cmdGetParent.Enabled = false;
                btnApply.Enabled = false;

                txtEXC_ABB.Text = "";
                txtITFACE_CODE.Text = "";
                txtRT_CODE.Text = "";
                txtDPNumSelected.Tag = 0;
                txtDPNumSelected.Text = "";
                txtDPNumNew.Text = "";

                v_EXC_ABB = "";
                v_ITFACE_CODE = "";
                v_RT_CODE = "";
                v_DP_NUM = "";

                Application.DoEvents();

                #endregion

                #region Copy DP : Check selected feature

                if (GTClassFactory.Create<IGTApplication>().SelectedObjects.FeatureCount == 0)
                {
                    throw new System.Exception("No feature is selected. Please select a DP");
                }

                GTClassFactory.Create<IGTApplication>().SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Checking selected feature");

                selFeature = GTClassFactory.Create<IGTApplication>().SelectedObjects;
                if (selFeature.FeatureCount == 1)
                {
                    IGTDDCKeyObject oDDCKeyObject = selFeature.GetObjects()[0];
                    if (oDDCKeyObject.FNO != DPFNo) //DPFNo = 13000. Check whether the selected object is DP
                    {
                        throw new System.Exception("Please select only one DP");
                    }
                    else
                    {
                        selGeom = oDDCKeyObject.Geometry;
                        sFeatureFNo = oDDCKeyObject.FNO;
                        sFeatureFID = oDDCKeyObject.FID;
                        txtDPNumSelected.Tag = sFeatureFID;

                        lblStatus.Text = "Reading DP properties ...";

                        #region Copy DP - Retrieve all information for DP

                        /*
                             GC_DP
                            --------------------
                            Name                  Null?    Type         
                            --------------------- -------- ------------ 
                        
                            v_G3E_FNO               NOT NULL NUMBER(5)    
                            v_G3E_CNO               NOT NULL NUMBER(5)    
                            v_G3E_CID               NOT NULL NUMBER(10)   
                            v_BND_FID                        NUMBER(10)   
                            v_CASS_DATE                      NUMBER(22)   
                            v_COMPOSITION                    VARCHAR2(20) 
                            v_CON_TYPE                       VARCHAR2(50) 
                            v_DP_CLASS                       VARCHAR2(25) 
                            v_DP_OWNER                       VARCHAR2(20) 
                            v_DP_TYPE                        VARCHAR2(50) 
                            v_D_CAPACITY                     VARCHAR2(50) 
                            v_D_WAITER                       NUMBER(22)   
                            v_EARTH_ROD                      NUMBER(22)   
                            v_E_WAITER                       NUMBER(22)   
                            v_FAULT_PAIR                     NUMBER(22)   
                            v_INSTALL_LOC                    VARCHAR2(50) 
                            v_ITFACE_CODE                    VARCHAR2(4)  
                            v_MAINDP_IPID                    NUMBER(22)   
                            v_NONUM_WAITER                   NUMBER(22)   
                            v_ORIGINAL_USER                  VARCHAR2(20) 
                            v_RAD_DIST                       NUMBER(22)   
                            v_RES_PAIR                       NUMBER(22)   
                            v_SPARE_PAIR                     NUMBER(22)   
                            v_TERM_TYPE                      VARCHAR2(30) 
                            v_TOOL_TYPE                      VARCHAR2(50) 
                            v_WORK_PAIR                      NUMBER(22)   
                             */

                        /* 
                        * GC_NETELEM
                        --------------------
                        Name                  Null?    Type           
                        --------------------- -------- -------------- 
                            
                        G3E_FNO               NOT NULL   NUMBER(5)      
                        G3E_CNO               NOT NULL   NUMBER(5)      
                        G3E_CID               NOT NULL   NUMBER(10)    
                        v_EXC_ABB                        VARCHAR2(50)   
                        v_BILLING_RATE                   VARCHAR2(50)   
                        v_ID                             VARCHAR2(30)   
                        v_IMAP_FEATURE_ID                VARCHAR2(25)   
                        v_MIC                            VARCHAR2(15)   
                        v_MIN_MATERIAL                   VARCHAR2(50)   
                        v_OWNERSHIP                      VARCHAR2(30)   
                        v_PLAN_ID                        VARCHAR2(25)   
                        v_SAP_WRK_ID                     VARCHAR2(30)   
                        v_SEGMENT                        VARCHAR2(80)   
                        v_SERVICE_CODE                   VARCHAR2(4)    
                        v_SWITCH_CENTRE_CLLI             VARCHAR2(25)   
                            
                        */

                        // Perform query for DP and NETELEM
                        ADODB.Recordset rsInfo = new ADODB.Recordset();
                        strSQL = "SELECT A.BILLING_RATE, A.ID, A.EXC_ABB, A.IMAP_FEATURE_ID, A.MIC, A.MIN_MATERIAL, A.OWNERSHIP, ";
                        strSQL = strSQL + "A.PLAN_ID, A.SAP_WRK_ID, A.SEGMENT, A.SERVICE_CODE, A.SWITCH_CENTRE_CLLI, ";
                        strSQL = strSQL + "B.G3E_FNO, B.G3E_CNO, B.G3E_CID, B.BND_FID, B.CASS_DATE, B.COMPOSITION, ";
                        strSQL = strSQL + "B.CON_TYPE, B.DP_CLASS, B.DP_NUM, B.DP_OWNER, B.DP_TYPE, B.D_CAPACITY, B.D_WAITER, ";
                        strSQL = strSQL + "B.EARTH_ROD, B.E_WAITER, B.FAULT_PAIR, B.INSTALL_LOC, B.ITFACE_CODE, B.RT_CODE, B.MAINDP_IPID, ";
                        strSQL = strSQL + "B.NONUM_WAITER, B.ORIGINAL_USER, B.RAD_DIST, B.RES_PAIR, B.SPARE_PAIR, ";
                        strSQL = strSQL + "B.TERM_TYPE, B.TOOL_TYPE, B.WORK_PAIR ";
                        strSQL = strSQL + "FROM GC_NETELEM A, GC_DP B WHERE A.G3E_FID = B.G3E_FID AND A.G3E_FID = " + sFeatureFID;
                        rsInfo = GTClassFactory.Create<IGTApplication>().DataContext.OpenRecordset(strSQL, ADODB.CursorTypeEnum.adOpenStatic, ADODB.LockTypeEnum.adLockReadOnly, 1);

                        if (rsInfo.RecordCount > 0)
                        {
                            rsInfo.MoveFirst();
                            v_EXC_ABB = myUtil.rsField(rsInfo, "EXC_ABB");
                            v_BILLING_RATE = myUtil.rsField(rsInfo, "BILLING_RATE");
                            v_ID = myUtil.rsField(rsInfo, "ID");
                            v_IMAP_FEATURE_ID = myUtil.rsField(rsInfo, "IMAP_FEATURE_ID");
                            v_MIC = myUtil.rsField(rsInfo, "MIC");
                            v_MIN_MATERIAL = myUtil.rsField(rsInfo, "MIN_MATERIAL");
                            v_OWNERSHIP = myUtil.rsField(rsInfo, "OWNERSHIP");
                            v_PLAN_ID = myUtil.rsField(rsInfo, "PLAN_ID");
                            v_SAP_WRK_ID = myUtil.rsField(rsInfo, "SAP_WRK_ID");
                            v_SEGMENT = myUtil.rsField(rsInfo, "SEGMENT");
                            v_SERVICE_CODE = myUtil.rsField(rsInfo, "SERVICE_CODE");
                            v_SWITCH_CENTRE_CLLI = myUtil.rsField(rsInfo, "SWITCH_CENTRE_CLLI");
                            v_G3E_FNO = Convert.ToInt16(rsInfo.Fields["G3E_FNO"].Value.ToString());
                            v_G3E_CNO = Convert.ToInt16(rsInfo.Fields["G3E_CNO"].Value.ToString());
                            v_G3E_CID = Convert.ToDouble(rsInfo.Fields["G3E_CID"].Value.ToString());
                            if (myUtil.rsField(rsInfo, "BND_FID").Length > 0)
                                v_BND_FID = Convert.ToDouble(rsInfo.Fields["BND_FID"].Value.ToString());
                            if (myUtil.rsField(rsInfo, "CASS_DATE").Length > 0)
                                v_CASS_DATE = Convert.ToDouble(rsInfo.Fields["CASS_DATE"].Value.ToString());
                            v_COMPOSITION = myUtil.rsField(rsInfo, "COMPOSITION");
                            v_CON_TYPE = myUtil.rsField(rsInfo, "CON_TYPE");
                            v_DP_NUM = myUtil.rsField(rsInfo, "DP_NUM");
                            v_DP_CLASS = myUtil.rsField(rsInfo, "DP_CLASS ");
                            v_DP_OWNER = myUtil.rsField(rsInfo, "DP_OWNER");
                            v_DP_TYPE = myUtil.rsField(rsInfo, "DP_TYPE");
                            v_D_CAPACITY = myUtil.rsField(rsInfo, "D_CAPACITY");
                            if (myUtil.rsField(rsInfo, "D_WAITER").Length > 0)
                                v_D_WAITER = Convert.ToDouble(rsInfo.Fields["D_WAITER"].Value.ToString());
                            if (myUtil.rsField(rsInfo, "EARTH_ROD").Length > 0)
                                v_EARTH_ROD = Convert.ToDouble(rsInfo.Fields["EARTH_ROD"].Value.ToString());
                            if (myUtil.rsField(rsInfo, "E_WAITER").Length > 0)
                                v_E_WAITER = Convert.ToDouble(rsInfo.Fields["E_WAITER"].Value.ToString());
                            if (myUtil.rsField(rsInfo, "FAULT_PAIR").Length > 0)
                                v_FAULT_PAIR = Convert.ToDouble(rsInfo.Fields["FAULT_PAIR"].Value.ToString());
                            v_INSTALL_LOC = myUtil.rsField(rsInfo, "INSTALL_LOC");
                            v_ITFACE_CODE = myUtil.rsField(rsInfo, "ITFACE_CODE");
                            v_RT_CODE = myUtil.rsField(rsInfo, "RT_CODE");
                            if (myUtil.rsField(rsInfo, "MAINDP_IPID").Length > 0)
                                v_MAINDP_IPID = Convert.ToDouble(rsInfo.Fields["MAINDP_IPID"].Value.ToString());
                            if (myUtil.rsField(rsInfo, "NONUM_WAITER").Length > 0)
                                v_NONUM_WAITER = Convert.ToDouble(rsInfo.Fields["NONUM_WAITER"].Value.ToString());
                            v_ORIGINAL_USER = myUtil.rsField(rsInfo, "ORIGINAL_USER");
                            if (myUtil.rsField(rsInfo, "RAD_DIST").Length > 0)
                                v_RAD_DIST = Convert.ToDouble(rsInfo.Fields["RAD_DIST"].Value.ToString());
                            if (myUtil.rsField(rsInfo, "RES_PAIR").Length > 0)
                                v_RES_PAIR = Convert.ToDouble(rsInfo.Fields["RES_PAIR"].Value.ToString());
                            if (myUtil.rsField(rsInfo, "SPARE_PAIR").Length > 0)
                                v_SPARE_PAIR = Convert.ToDouble(rsInfo.Fields["SPARE_PAIR"].Value.ToString());
                            v_TERM_TYPE = myUtil.rsField(rsInfo, "TERM_TYPE");
                            v_TOOL_TYPE = myUtil.rsField(rsInfo, "TOOL_TYPE");
                            if (myUtil.rsField(rsInfo, "WORK_PAIR").Length > 0)
                                v_WORK_PAIR = Convert.ToDouble(rsInfo.Fields["WORK_PAIR"].Value.ToString());
                        }
                        rsInfo = null;
                        #endregion

                        txtEXC_ABB.Text = v_EXC_ABB;
                        txtITFACE_CODE.Text = v_ITFACE_CODE;
                        txtRT_CODE.Text = v_RT_CODE;
                        txtDPNumSelected.Text = v_DP_NUM;
                        txtDPNumSelected.Tag = sFeatureFID;

                        txtDPNumNew.Text = GetNextDPNumber(v_EXC_ABB, v_ITFACE_CODE, v_RT_CODE);

                    } //end if else
                }//end if (selFeature.FeatureCount == 1)

                #endregion

                cmdGetParent.Enabled = true;
                if (txtEXC_ABB.Text.Length == 0 || (txtITFACE_CODE.Text.Length == 0 && txtRT_CODE.Text.Length == 0))
                    lblStatus.Text = "Parent information not found\r\nPlease Get Parent before DP placement";
                else
                    lblStatus.Text = "Double click on map to place new DP or\r\nGet Parent to change DP parent";
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "Copy DP", MessageBoxButtons.OK, MessageBoxIcon.Error);
                lblStatus.Text = "Select DP to be copied";
            }//end try catch

        }// end function

        #endregion

        #region Copy DP : Create New DP on Double Click Event.
        public void finishCopy()
        {
            this.Show();
            tabManage.TabPages.Remove(tabDPNum);
            tabManage.TabPages.Add(tabIdentifyParent);
        }

        public void createNewDP(double XCoord, double YCoord, double detail_ID)
        {
            try
            {
                #region Copy DP : Validation
                if (txtEXC_ABB.Text.Length == 0 || (txtITFACE_CODE.Text.Length == 0 && txtRT_CODE.Text.Length == 0))
                    throw new System.Exception("Please Get Parent before continue with DP placement");
                if (DuplicateCode(txtEXC_ABB.Text.Trim(), txtITFACE_CODE.Text.Trim(), txtRT_CODE.Text.Trim(), txtDPNumNew.Text.Trim()))
                    return;
                #endregion

                GTClassFactory.Create<IGTApplication>().BeginWaitCursor();

                int nFeatureFID = 0;
                lblStatus.Text = "Saving new DP properties to database...";

                #region Copy DP : Preparing DP Point Symbol and DP Point Text according to X-coordinate, Y-coordinate

                #region Copy DP - DP Point symbol
                IGTPointGeometry oDP_Point;
                oDP_Point = GTClassFactory.Create<IGTPointGeometry>();
                IGTPoint oPntGeom = GTClassFactory.Create<IGTPoint>();
                oPntGeom.X = XCoord;
                oPntGeom.Y = YCoord;

                oDP_Point.Origin = oPntGeom;
                #endregion

                #region Copy DP - DP Point text
                IGTTextPointGeometry oDP_Text;
                oDP_Text = GTClassFactory.Create<IGTTextPointGeometry>();
                IGTPoint oTxtGeom = GTClassFactory.Create<IGTPoint>();
                oTxtGeom.X = XCoord;
                oTxtGeom.Y = YCoord;

                oDP_Text.Origin = oTxtGeom;
                oDP_Text.Rotation = 0;
                #endregion

                #endregion

                #region Copy DP : Initiate placement/drawing process

                GTCopyDP.m_oIGTTransactionManager.Begin("DrawDP");
                IGTKeyObject oNewFeature;
                oNewFeature = GTClassFactory.Create<IGTApplication>().DataContext.NewFeature(sFeatureFNo);
                nFeatureFID = oNewFeature.FID;

                #endregion

                #region Copy DP : Registering DP Netelem into the database
                iCNO = 51;
                /* 
                 * GC_NETELEM
                --------------------
                Name                  Null?    Type           Need To Copy
                --------------------- -------- -------------- ------------
            
                G3E_FNO               NOT NULL NUMBER(5)      Y
                G3E_CNO               NOT NULL NUMBER(5)      Y
                G3E_CID               NOT NULL NUMBER(10)     Y
                BILLING_RATE                   VARCHAR2(50)   Y
                ID                             VARCHAR2(30)   Y
                IMAP_FEATURE_ID                VARCHAR2(25)   Y
                MIC                            VARCHAR2(15)   Y
                MIN_MATERIAL                   VARCHAR2(50)   Y
                OWNERSHIP                      VARCHAR2(30)   Y
                PLAN_ID                        VARCHAR2(25)   Y
                SAP_WRK_ID                     VARCHAR2(30)   Y
                SEGMENT                        VARCHAR2(80)   Y
                SERVICE_CODE                   VARCHAR2(4)    Y
                SWITCH_CENTRE_CLLI             VARCHAR2(25)   Y
            
                */

                if (oNewFeature.Components.GetComponent(iCNO).Recordset.EOF)
                {
                    oNewFeature.Components.GetComponent(iCNO).Recordset.AddNew("G3E_FID", nFeatureFID);
                    oNewFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_FNO", sFeatureFNo);
                    oNewFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_CID", 1);
                }
                else
                {
                    oNewFeature.Components.GetComponent(iCNO).Recordset.MoveLast();
                }
                oNewFeature.Components.GetComponent(iCNO).Recordset.Update("BILLING_RATE", v_BILLING_RATE);
                oNewFeature.Components.GetComponent(iCNO).Recordset.Update("ID", v_ID);
                oNewFeature.Components.GetComponent(iCNO).Recordset.Update("IMAP_FEATURE_ID", v_IMAP_FEATURE_ID);
                oNewFeature.Components.GetComponent(iCNO).Recordset.Update("MIC", v_MIC);
                oNewFeature.Components.GetComponent(iCNO).Recordset.Update("MIN_MATERIAL", v_MIN_MATERIAL);
                oNewFeature.Components.GetComponent(iCNO).Recordset.Update("OWNERSHIP", v_OWNERSHIP);
                oNewFeature.Components.GetComponent(iCNO).Recordset.Update("PLAN_ID", v_PLAN_ID);
                oNewFeature.Components.GetComponent(iCNO).Recordset.Update("SAP_WRK_ID", v_SAP_WRK_ID);
                oNewFeature.Components.GetComponent(iCNO).Recordset.Update("SEGMENT", v_SEGMENT);
                oNewFeature.Components.GetComponent(iCNO).Recordset.Update("SERVICE_CODE", v_SERVICE_CODE);
                oNewFeature.Components.GetComponent(iCNO).Recordset.Update("SWITCH_CENTRE_CLLI", v_SWITCH_CENTRE_CLLI);
                oNewFeature.Components.GetComponent(iCNO).Recordset.Update("FEATURE_STATE", "PPF");
                oNewFeature.Components.GetComponent(iCNO).Recordset.Update("JOB_STATE", "PROPOSED");
                oNewFeature.Components.GetComponent(iCNO).Recordset.Update("EXC_ABB", txtEXC_ABB.Text);

                #endregion

                #region Copy DP - Registering New DP Attributes into the database
                iCNO = 13001;

                /*
                 GC_DP
                --------------------
                Name                  Null?    Type         
                --------------------- -------- ------------ 
        
                G3E_FNO               NOT NULL NUMBER(5)    
                G3E_CNO               NOT NULL NUMBER(5)    
                G3E_CID               NOT NULL NUMBER(10)   
                BND_FID                        NUMBER(10)   
                CASS_DATE                      NUMBER(22)   
                COMPOSITION                    VARCHAR2(20) 
                CON_TYPE                       VARCHAR2(50) 
                DP_CLASS                       VARCHAR2(25) 
                DP_OWNER                       VARCHAR2(20) 
                DP_TYPE                        VARCHAR2(50) 
                D_CAPACITY                     VARCHAR2(50) 
                D_WAITER                       NUMBER(22)   
                EARTH_ROD                      NUMBER(22)   
                E_WAITER                       NUMBER(22)   
                FAULT_PAIR                     NUMBER(22)   
                INSTALL_LOC                    VARCHAR2(50) 
                ITFACE_CODE                    VARCHAR2(4)  
                MAINDP_IPID                    NUMBER(22)   
                NONUM_WAITER                   NUMBER(22)   
                ORIGINAL_USER                  VARCHAR2(20) 
                RAD_DIST                       NUMBER(22)   
                RES_PAIR                       NUMBER(22)   
                SPARE_PAIR                     NUMBER(22)   
                TERM_TYPE                      VARCHAR2(30) 
                TOOL_TYPE                      VARCHAR2(50) 
                WORK_PAIR                      NUMBER(22)   
                */

                if (oNewFeature.Components.GetComponent(iCNO).Recordset.EOF)
                {
                    oNewFeature.Components.GetComponent(iCNO).Recordset.AddNew("G3E_FID", nFeatureFID);
                    oNewFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_FNO", v_G3E_FNO);
                    oNewFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_CNO", v_G3E_CNO);
                    oNewFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_CID", v_G3E_CID);
                }
                else
                    oNewFeature.Components.GetComponent(iCNO).Recordset.MoveLast();

                oNewFeature.Components.GetComponent(iCNO).Recordset.Update("CASS_DATE", v_CASS_DATE);
                oNewFeature.Components.GetComponent(iCNO).Recordset.Update("COMPOSITION", v_COMPOSITION);
                oNewFeature.Components.GetComponent(iCNO).Recordset.Update("CON_TYPE", v_CON_TYPE);
                oNewFeature.Components.GetComponent(iCNO).Recordset.Update("DP_NUM", txtDPNumNew.Text);
                oNewFeature.Components.GetComponent(iCNO).Recordset.Update("DP_CLASS", v_DP_CLASS);
                oNewFeature.Components.GetComponent(iCNO).Recordset.Update("DP_OWNER", v_DP_OWNER);
                oNewFeature.Components.GetComponent(iCNO).Recordset.Update("DP_TYPE", v_DP_TYPE);
                oNewFeature.Components.GetComponent(iCNO).Recordset.Update("D_CAPACITY", v_D_CAPACITY);
                oNewFeature.Components.GetComponent(iCNO).Recordset.Update("D_WAITER", v_D_WAITER);
                oNewFeature.Components.GetComponent(iCNO).Recordset.Update("EARTH_ROD", v_EARTH_ROD);
                oNewFeature.Components.GetComponent(iCNO).Recordset.Update("E_WAITER", v_E_WAITER);
                oNewFeature.Components.GetComponent(iCNO).Recordset.Update("FAULT_PAIR", v_FAULT_PAIR);
                oNewFeature.Components.GetComponent(iCNO).Recordset.Update("INSTALL_LOC", v_INSTALL_LOC);
                oNewFeature.Components.GetComponent(iCNO).Recordset.Update("ITFACE_CODE", txtITFACE_CODE.Text);
                oNewFeature.Components.GetComponent(iCNO).Recordset.Update("RT_CODE", txtRT_CODE.Text);
                oNewFeature.Components.GetComponent(iCNO).Recordset.Update("MAINDP_IPID", v_MAINDP_IPID);
                oNewFeature.Components.GetComponent(iCNO).Recordset.Update("NONUM_WAITER", v_NONUM_WAITER);
                oNewFeature.Components.GetComponent(iCNO).Recordset.Update("ORIGINAL_USER", v_ORIGINAL_USER);
                oNewFeature.Components.GetComponent(iCNO).Recordset.Update("RAD_DIST", v_RAD_DIST);
                oNewFeature.Components.GetComponent(iCNO).Recordset.Update("RES_PAIR", v_RES_PAIR);
                oNewFeature.Components.GetComponent(iCNO).Recordset.Update("SPARE_PAIR", v_SPARE_PAIR);
                oNewFeature.Components.GetComponent(iCNO).Recordset.Update("TERM_TYPE", v_TERM_TYPE);
                oNewFeature.Components.GetComponent(iCNO).Recordset.Update("TOOL_TYPE", v_TOOL_TYPE);
                oNewFeature.Components.GetComponent(iCNO).Recordset.Update("WORK_PAIR", v_WORK_PAIR);
                if (detail_ID > 0)
                    oNewFeature.Components.GetComponent(iCNO).Recordset.Update("BND_FID", 0);
                else if (chkCopyBoundary.Checked)
                    oNewFeature.Components.GetComponent(iCNO).Recordset.Update("BND_FID", v_BND_FID);


                #endregion

                CopyDPAddress(oNewFeature);

                #region Copy DP : Create DP Symbol

                // detail_ID is used by Detail Window 
                if (detail_ID > 0)
                    iCNO = 13021;
                else
                    iCNO = 13020;

                if (oNewFeature.Components.GetComponent(iCNO).Recordset.EOF)
                {
                    oNewFeature.Components.GetComponent(iCNO).Recordset.AddNew("G3E_FID", nFeatureFID);
                    oNewFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_FNO", sFeatureFNo);
                    oNewFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_CID", 1);
                    if (detail_ID > 0)
                        oNewFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_DETAILID", detail_ID);
                }
                else
                {
                    oNewFeature.Components.GetComponent(iCNO).Recordset.MoveLast();
                }

                oNewFeature.Components.GetComponent(iCNO).Geometry = oDP_Point;
                #endregion

                #region Copy DP : Create DP Text

                if (detail_ID > 0)
                    iCNO = 13031;
                else
                    iCNO = 13030;

                // select G3E_CNO FROM G3E_COMPONENT WHERE G3E_TABLE = GC_DP_T; 
                if (oNewFeature.Components.GetComponent(iCNO).Recordset.EOF)
                {
                    oNewFeature.Components.GetComponent(iCNO).Recordset.AddNew("G3E_FID", nFeatureFID);
                    oNewFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_FNO", sFeatureFNo);
                    oNewFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_CID", 1);
                    if (detail_ID > 0)
                        oNewFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_DETAILID", detail_ID);
                }
                else
                {
                    oNewFeature.Components.GetComponent(iCNO).Recordset.MoveLast();
                }

                oNewFeature.Components.GetComponent(iCNO).Geometry = oDP_Text;
                #endregion

                #region Copy DP - Initial End Drawing & Refresh Metadata Changing
                GTCopyDP.m_oIGTTransactionManager.Commit();
                GTCopyDP.m_oIGTTransactionManager.RefreshDatabaseChanges();
                #endregion

                txtDPNumNew.Text = GetNextDPNumber(txtEXC_ABB.Text, txtITFACE_CODE.Text, txtRT_CODE.Text);
                lblStatus.Text = "New DP already saved to NEPS\r\nDouble Click on map to place next DP";
                btnClose.Enabled = true;
            }
            catch (Exception ex)
            {
                lblStatus.Text = "Error saving new DP";
                MessageBox.Show(this, ex.Message, "Copy DP Cancel");
                GTCopyDP.m_oIGTTransactionManager.Rollback();
                this.Close();
            }
            finally
            {
                GTClassFactory.Create<IGTApplication>().EndWaitCursor();
            }
        }

        #endregion

        private void CopyDPAddress(IGTKeyObject dest)
        {
            string ssql = "SELECT * FROM GC_ADDRESS WHERE G3E_FID = " + sFeatureFID.ToString();
            ADODB.Recordset rs = new ADODB.Recordset();
            rs = GTClassFactory.Create<IGTApplication>().DataContext.OpenRecordset(ssql,
                ADODB.CursorTypeEnum.adOpenStatic, ADODB.LockTypeEnum.adLockOptimistic, 1);

            if (rs.EOF) return;

            iCNO = 52;
            if (dest.Components.GetComponent(iCNO).Recordset.EOF)
            {
                dest.Components.GetComponent(iCNO).Recordset.AddNew("G3E_FID", dest.FID);
                dest.Components.GetComponent(iCNO).Recordset.Update("G3E_FNO", 13000);
                dest.Components.GetComponent(iCNO).Recordset.Update("G3E_CNO", 52);
                dest.Components.GetComponent(iCNO).Recordset.Update("G3E_CID", 1);
            }
            else
                dest.Components.GetComponent(iCNO).Recordset.MoveLast();

            myUtil.CopyFields(dest, rs, iCNO, "BUILDING_NAME");
            myUtil.CopyFields(dest, rs, iCNO, "CITY");
            myUtil.CopyFields(dest, rs, iCNO, "COUNTRY");
            myUtil.CopyFields(dest, rs, iCNO, "OLD_ADDRESS");
            myUtil.CopyFields(dest, rs, iCNO, "OTHER_LOCATION");
            myUtil.CopyFields(dest, rs, iCNO, "POSTAL_CODE");
            myUtil.CopyFields(dest, rs, iCNO, "SEC_NAME");
            myUtil.CopyFields(dest, rs, iCNO, "STATE_CODE");
            myUtil.CopyFields(dest, rs, iCNO, "STREET_DIRECTION");
            myUtil.CopyFields(dest, rs, iCNO, "STREET_NAME");
            myUtil.CopyFields(dest, rs, iCNO, "STREET_TYPE");
        }

        #region Parent

        private void cmdGetParent_Click(object sender, EventArgs e)
        {
            try
            {
                #region Copy DP : Variables declaration

                IGTSelectedObjects selFeature;  // selected Feature (any object on the map)

                #endregion

                #region Copy DP : Check selected feature
                if (GTClassFactory.Create<IGTApplication>().SelectedObjects.FeatureCount == 0)
                {
                    MessageBox.Show(this, "No feature is selected. Please select a cabinet", "Copy DP (Copper)", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                GTClassFactory.Create<IGTApplication>().SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Checking selected feature");

                selFeature = GTClassFactory.Create<IGTApplication>().SelectedObjects;

                if (selFeature.FeatureCount == 1)
                {
                    IGTDDCKeyObject oDDCKeyObject = selFeature.GetObjects()[0];

                    string ssql = "SELECT {0} FROM {1} WHERE G3E_FID = " + oDDCKeyObject.FID;
                    switch (oDDCKeyObject.FNO)
                    {
                        case 10300:
                            txtITFACE_CODE.Text = ReadFieldValue(string.Format(ssql, "ITFACE_CODE", "GC_ITFACE"));
                            txtRT_CODE.Text = "";
                            break;
                        case 9100:
                            txtRT_CODE.Text = ReadFieldValue(string.Format(ssql, "RT_CODE", "GC_MSAN"));
                            txtITFACE_CODE.Text = "";
                            break;
                        case 9600:
                            txtRT_CODE.Text = ReadFieldValue(string.Format(ssql, "RT_CODE", "GC_RT"));
                            txtITFACE_CODE.Text = "";
                            break;
                        default:
                            throw new System.Exception("Please select either a COPPER, RT or MSAN cabinet");
                    }

                    ssql = "SELECT EXC_ABB FROM GC_NETELEM WHERE G3E_FID = " + oDDCKeyObject.FID;
                    txtEXC_ABB.Text = ReadFieldValue(ssql);

                    txtDPNumSelected.Text = "";
                    txtDPNumNew.Text = GetNextDPNumber(txtEXC_ABB.Text, txtITFACE_CODE.Text, txtRT_CODE.Text);
                    cmdGetParent.Enabled = false;
                    btnApply.Enabled = true;
                    lblStatus.Text = "Click 'Update DP attribute' to set attribute of selected DP";
                }
                else
                {
                    throw new System.Exception("Get parent fail. Multiple selection detected. ");
                }//end if (selFeature.FeatureCount == 1

                #endregion
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "Copy DP (Copper)", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }//end try catch
        }

        #endregion

        #region Update 02-07-2012

        private string ReadFieldValue(string ssql)
        {
            ADODB.Recordset rs = new ADODB.Recordset();
            rs = GTClassFactory.Create<IGTApplication>().DataContext.OpenRecordset(ssql,
                ADODB.CursorTypeEnum.adOpenStatic, ADODB.LockTypeEnum.adLockOptimistic, 1);

            if (!rs.EOF)
            {
                return rs.Fields[0].Value.ToString();
            }
            else
            {
                return "";
            }
        }

        private string GetDPNumber(int i)
        {
            this.TopMost = false;

            try
            {
                string ssql = "SELECT GC_OSP_COP_VAL.CNO_13001_GET_DP_NUMBER ('"
                    + txtEXC_ABB.Text + "','" + txtITFACE_CODE.Text + "', NULL) FROM DUAL";

                string dpnum = ReadFieldValue(ssql);
                string msg = (i == 0 ? "Please enter new DP number" : "DP Number already in used.\r\nPlease enter a new number");
                msg += "\r\n\nor click [OK] to accept suggested DP number";
                dpnum = InputBox.Show(this, msg, "Copy DP", dpnum, -1, -1);
                if (dpnum.Length == 0)
                {
                    throw new System.Exception("User cancel Copy DP");
                }
                else
                {
                    ssql = "SELECT A.DP_NUM FROM GC_DP A, GC_NETELEM B WHERE A.G3E_FID = B.G3E_FID AND A.ITFACE_CODE = '" + txtITFACE_CODE.Text
                        + "' AND B.EXC_ABB = '" + txtEXC_ABB.Text + "' AND A.DP_NUM = '" + dpnum + "'";

                    if (ReadFieldValue(ssql) != "")
                    {
                        if (i < 3)
                            dpnum = GetDPNumber(++i);
                        else
                            throw new System.Exception("Too many failed try. Copy DP canceled");
                    }
                }
                return dpnum;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                this.TopMost = true;
                this.Visible = true;
            }
        }

        private string GetNextDPNumber(string excabb, string itface, string rtcode)
        {
            try
            {
                string ssql = "SELECT MAX (LPAD(REGEXP_REPLACE (A.DP_NUM,'[A-Z]|[a-z]','') ,7,'0'))  " +
                "FROM GC_DP A, GC_NETELEM B WHERE A.G3E_FID = B.G3E_FID AND A.{0} = '{1}' AND B.EXC_ABB = '{2}' ";

                if (rtcode.Length > 0)
                    ssql = string.Format(ssql, "RT_CODE", rtcode, excabb);
                else
                    ssql = string.Format(ssql, "ITFACE_CODE", itface, excabb);

                ADODB.Recordset rs = new ADODB.Recordset();
                rs = GTClassFactory.Create<IGTApplication>().DataContext.OpenRecordset(ssql,
                    ADODB.CursorTypeEnum.adOpenStatic, ADODB.LockTypeEnum.adLockOptimistic, 1);

                if (!rs.EOF)
                {
                    return Convert.ToString(Convert.ToInt32(rs.Fields[0].Value) + 1);
                }
                else
                {
                    return "1";
                }
            }
            catch (Exception ex)
            {
                return "1";
            }
        }

        private bool DuplicateCode(string excabb, string itface, string rtcode, string dpnum)
        {
            try
            {
                string ssql = "SELECT LPAD(A.DP_NUM,7,'0') FROM GC_DP A, GC_NETELEM B " +
                    "WHERE LPAD(A.DP_NUM,7,'0') = '{1}' AND A.{2} = '{3}' AND B.EXC_ABB = '{0}' AND A.G3E_FID = B.G3E_FID";

                ADODB.Recordset rs = new ADODB.Recordset();
                if (itface.Length > 0)
                    ssql = string.Format(ssql, excabb, dpnum.PadLeft(7, '0'), "ITFACE_CODE", itface);
                else
                    ssql = string.Format(ssql, excabb, dpnum.PadLeft(7, '0'), "RT_CODE", rtcode);

                rs = GTClassFactory.Create<IGTApplication>().DataContext.OpenRecordset(ssql,
                    ADODB.CursorTypeEnum.adOpenStatic, ADODB.LockTypeEnum.adLockOptimistic, 1);

                if (rs.RecordCount > 0)
                {
                    MessageBox.Show(this, "Duplicate DP Code\r\nCurrent DP Number already use by others", "Copy DP");
                    return true;
                }
                else
                    return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        #endregion

        private void txtProp_TextChanged(object sender, EventArgs e)
        {
            GetNextDPNumber(txtEXC_ABB.Text, txtITFACE_CODE.Text, txtRT_CODE.Text);

            if ((txtEXC_ABB.Text.Length > 0) &&
                (txtITFACE_CODE.Text.Length > 0 ||
                txtRT_CODE.Text.Length > 0))
            {
                cmdGetParent.Enabled = false;
            }
            else
            {
                cmdGetParent.Enabled = true;
            }
        }

        private void txtProp_Enter(object sender, EventArgs e)
        {
            txtDPNumSelected.Focus();
        }

        private void btnApply_Click(object sender, EventArgs e)
        {
            if (DuplicateCode(txtEXC_ABB.Text.Trim(), txtITFACE_CODE.Text.Trim(), txtITFACE_CODE.Text.Trim(), txtDPNumNew.Text.Trim()))
            {
                lblStatus.Text = "Operation Fail : Duplicate DP Number";
                return;
            }
            else if (MessageBox.Show(this,
                "Set parent attribute and DP Number of selected DP\r\nSelected DP Number will be set to " + txtDPNumNew.Text,
                "Update DP Attribute", MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                string ssql = "UPDATE GC_DP SET ITFACE_CODE = '" + txtITFACE_CODE.Text + "', RT_CODE = '" + txtRT_CODE.Text + "', ";
                ssql += "DP_NUM = '" + txtDPNumNew.Text + "' WHERE G3E_FID = " + sFeatureFID.ToString();

                int iR;
                GTClassFactory.Create<IGTApplication>().DataContext.Execute(ssql, out iR, (int)ADODB.CommandTypeEnum.adCmdText);

                ssql = "UPDATE GC_NETELEM SET EXC_ABB = '" + txtEXC_ABB.Text + "' WHERE G3E_FID = " + sFeatureFID.ToString();
                GTClassFactory.Create<IGTApplication>().DataContext.Execute(ssql, out iR, (int)ADODB.CommandTypeEnum.adCmdText);

                GTClassFactory.Create<IGTApplication>().DataContext.Execute("COMMIT", out iR, (int)ADODB.CommandTypeEnum.adCmdText);
                GTClassFactory.Create<IGTApplication>().DataContext.OpenRecordset(ssql,
                    ADODB.CursorTypeEnum.adOpenDynamic, ADODB.LockTypeEnum.adLockOptimistic, (int)ADODB.CommandTypeEnum.adCmdText);

                v_EXC_ABB = txtEXC_ABB.Text;
                v_ITFACE_CODE = txtITFACE_CODE.Text;
                v_RT_CODE = txtRT_CODE.Text;
                txtDPNumSelected.Text = txtDPNumNew.Text;
                v_DP_NUM = txtDPNumSelected.Text;
                txtDPNumNew.Text = GetNextDPNumber(txtEXC_ABB.Text, txtITFACE_CODE.Text, txtRT_CODE.Text);

                btnApply.Enabled = false;
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void txtDPNumNew_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!Char.IsLetterOrDigit(e.KeyChar) && !Char.IsControl(e.KeyChar))
                e.Handled = true;
            else if (Char.IsLower(e.KeyChar))
                e.KeyChar = Char.ToUpper(e.KeyChar);
        }

        private void txtDPNumNew_Leave(object sender, EventArgs e)
        {
            try
            {
                string ssql = "SELECT A.G3E_FID FROM GC_DP A, GC_NETELEM B WHERE A.DP_NUM = '" + txtDPNumNew.Text + "' ";
                ssql += "AND A.G3E_FID = B.G3E_FID AND B.EXC_ABB = '" + txtEXC_ABB.Text + "' ";
                if (txtRT_CODE.Text.Length > 0)
                    ssql += " AND A.RT_CODE = '" + txtRT_CODE.Text + "'";
                else
                    ssql += " AND A.ITFACE_CODE = '" + txtITFACE_CODE.Text + "'";

                string fid = ReadFieldValue(ssql);
                if (fid.Length > 0)
                { }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }

        }

    }//end partial class
}//end namespace













