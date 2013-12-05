using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Intergraph.GTechnology.Interfaces;
using Intergraph.GTechnology.API;
using ADODB;

namespace NEPS.GTechnology.InsertSplice
{
 
    class GTCustomCommandModeless : Intergraph.GTechnology.Interfaces.IGTCustomCommandModeless
    {
        #region variable
        public static Intergraph.GTechnology.API.IGTTransactionManager m_oGTTransactionManager = null;
        public static Intergraph.GTechnology.API.IGTCustomCommandHelper m_oGTCustomCommandHelper = null;

        public static IGTGeometryEditService mobjEditServiceTemp=null;
        public static Intergraph.GTechnology.API.IGTApplication m_gtapp =null;

        public IGTFeaturePlacementService placementService = null;
        public EventHandler<GTFinishedEventArgs> EV_placementService_Finished;

        public static IGTGeometry oPointPoint = null;
        public static IGTGeometry oTextPoint = null;

        private short LabelCNO = 0;
        private short MFNO = 0;//owner fno
        private int MFID = 0;//owner fid

        private short bFNO = 0;//cable fno
        private int bFID = 0;//cable fid

        public Logger log;

     
         int PlaceValue = 0;


        IGTKeyObject oCableToBreak;
        IGTKeyObject oCableNew;
        IGTKeyObject oSpliceFeature;

        public Intergraph.GTechnology.API.IGTTransactionManager TransactionManager
        {
            set
            {
                m_oGTTransactionManager = value;
            }
        }

        #endregion

        #region Activate
        public void Activate(Intergraph.GTechnology.API.IGTCustomCommandHelper CustomCommandHelper)
        {
            if (m_gtapp == null) m_gtapp = GTClassFactory.Create<IGTApplication>();
            m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Running Insert Splice ...");
            
            m_oGTCustomCommandHelper = CustomCommandHelper;

            foreach (IGTDDCKeyObject oDDCKeyObject in GTClassFactory.Create<IGTApplication>().SelectedObjects.GetObjects())
            {
                m_gtapp.SelectedObjects.Add(GTSelectModeConstants.gtsosmAllDisplayedComponentsInActiveLegend, oDDCKeyObject);
            }

            SubscribeEvents();

            mobjEditServiceTemp = GTClassFactory.Create<IGTGeometryEditService>();
            mobjEditServiceTemp.TargetMapWindow = m_gtapp.ActiveMapWindow;

            log = Logger.getInstance();
            
            if (m_gtapp.SelectedObjects.FeatureCount > 0)
            {
                foreach (IGTDDCKeyObject oDDCKeyObject in m_gtapp.SelectedObjects.GetObjects())
                {
                    if (oDDCKeyObject.FNO != 2700 && oDDCKeyObject.FNO != 3000 && oDDCKeyObject.FNO != 2800 && oDDCKeyObject.FNO != 3300 && oDDCKeyObject.FNO != 3800)
                    {
                        m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Please Select a Manhole/Pole/Civil Node/Tunnel/Trench/Chamber to Place Splice...");
                        MessageBox.Show("Please Select a Manhole/Pole/Civil Node/Tunnel/Trench/Chamber to Place Splice", "Insert Splice", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        break;
                    }
                    else
                    {
                        MFNO = oDDCKeyObject.FNO;
                        MFID = oDDCKeyObject.FID;
                        m_gtapp.SelectedObjects.Clear();
                        PlaceValue = 2;
                        MessageBox.Show("Please Select a Fiber Cable to Insert Splice/Break Feature.", "Insert Splice", MessageBoxButtons.OK, MessageBoxIcon.Information);
                               
                        break;
                    }
                }
            }
            if(PlaceValue!=2)
                PlaceValue = 1;

           // MessageBox.Show("Please Select a Manhole/Pole/Civil Node/Tunnel to Place Splice", "Insert Splice", MessageBoxButtons.OK, MessageBoxIcon.Information);                        
        }
        #endregion

        #region not used event
        void m_oGTCustomCommandHelper_Click(object sender, GTMouseEventArgs e)
        {

        }
        void m_oGTCustomCommandHelper_DblClick(object sender, GTMouseEventArgs e)
        {

        }
        void m_oGTCustomCommandHelper_WheelRotate(object sender, GTWheelRotateEventArgs e)
        {

        }
        void m_oGTCustomCommandHelper_MouseDown(object sender, GTMouseEventArgs e)
        {

        }

        void m_oGTCustomCommandHelper_LostFocus(object sender, GTLostFocusEventArgs e)
        {
        }
        void m_oGTCustomCommandHelper_GainedFocus(object sender, GTGainedFocusEventArgs e)
        {
            //     m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "GainedFocus.");
        }

        void m_oGTCustomCommandHelper_Deactivate(object sender, GTDeactivateEventArgs e)
        {
            //   m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Deactivate.");
        }

        void m_oGTCustomCommandHelper_Activate(object sender, GTActivateEventArgs e)
        {
            //   m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Activate.");
        }

        void m_oGTCustomCommandHelper_KeyPress(object sender, GTKeyPressEventArgs e)
        {
            //  m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "KeyPress.");
        }

        void m_oGTCustomCommandHelper_KeyDown(object sender, GTKeyEventArgs e)
        {
            //  m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "KeyDown.");
        }
        #endregion

        #region Mouse up/ Mouse Move
        void m_oGTCustomCommandHelper_MouseMove(object sender, GTMouseEventArgs e)
        {
            IGTPoint WorldPoint = e.WorldPoint;
            if (PlaceValue == 1)
            {
                m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Please Select a Manhole/Pole/Civil Node/Tunnel/Trench/Chamber to Place Splice...");
                //MessageBox.Show("Please Select a Manhole/Pole/Civil Node/Tunnel to Place Splice", "Insert Splice", MessageBoxButtons.OK, MessageBoxIcon.Information);                        
            } else
            if (PlaceValue == 2)
            {
                m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Please Select a Cable to Place Splice...");
                //MessageBox.Show("Please Select a Cable to Place Splice", "Insert Splice", MessageBoxButtons.OK, MessageBoxIcon.Information);                        
                IGTPointGeometry oPoint = PGeoLib.CreatePointGeom(WorldPoint.X, WorldPoint.Y);
                oPointPoint = oPoint;
                if (mobjEditServiceTemp.GeometryCount > 0) mobjEditServiceTemp.RemoveAllGeometries();
                mobjEditServiceTemp.AddGeometry(oPointPoint, 11820099);
            }
            else
            if (PlaceValue == 11)
            {

                //Pick Manhole or Pole MH 2700, Pole 3000
                if (m_gtapp.SelectedObjects.FeatureCount > 0)
                {
                    foreach (IGTDDCKeyObject oDDCKeyObject in m_gtapp.SelectedObjects.GetObjects())
                    {
                        if (oDDCKeyObject.FNO != 2700 && oDDCKeyObject.FNO != 3000 && oDDCKeyObject.FNO != 2800 && oDDCKeyObject.FNO != 3300 && oDDCKeyObject.FNO != 3800)
                        {
                            m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Please Select a Manhole/Pole/Civil Node/Tunnel/Trench/Chamber to Place Splice...");
                            MessageBox.Show("Please Select a Manhole/Pole/Civil Node/Tunnel/Trench/Chamber to Place Splice", "Insert Splice", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            PlaceValue = 1;
                            return;
                        }
                        else
                        {
                            MFNO = oDDCKeyObject.FNO;
                            MFID = oDDCKeyObject.FID;
                            m_gtapp.SelectedObjects.Clear();
                            PlaceValue = 2;
                        }
                    }
                }
                else
                {
                    m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Please Select a Manhole/Pole/Civil Node/Tunnel/Trench/Chamber to Place Splice...");
                    MessageBox.Show("Please Select a Manhole/Pole/Civil Node/Tunnel/Trench/Chamber to Place Splice", "Insert Splice", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    PlaceValue = 1;
                    return;
                }
            }
            else
            if (PlaceValue == 21)
            {
                //Get Selcted Cable Details
                if (m_gtapp.SelectedObjects.FeatureCount > 0)
                {
                    foreach (IGTDDCKeyObject oDDCKeyObject in m_gtapp.SelectedObjects.GetObjects())
                    {
                        if (oDDCKeyObject.FNO == 7200 || oDDCKeyObject.FNO == 7400 || oDDCKeyObject.FNO == 4400 || oDDCKeyObject.FNO == 4500)
                        {
                            bFNO = oDDCKeyObject.FNO;
                            bFID = oDDCKeyObject.FID;

                            if (mobjEditServiceTemp.GeometryCount > 0) mobjEditServiceTemp.RemoveAllGeometries();
                            mobjEditServiceTemp.AddGeometry(oPointPoint, 11820099);

                            PlaceValue = 3;
                            BreakCable();
                            return;
                        }
                        else
                        {
                            m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Cable Not selected, Please Select a Cable to Insert Splcie/Break Feature.");
                            MessageBox.Show("Please Select a Fiber Cable to Insert Splice/Break Feature.", "Insert Splice", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            PlaceValue = 2;
                            return;
                        }
                    }
                }
                else
                {
                    m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Please Select a Cable to Insert Splcie/Break Feature.");
                    MessageBox.Show("Please Select a Cable to Insert Splcie/Break Feature.", "Insert Splice", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    PlaceValue = 2;
                    return;
                }
            }

        }
        
        void m_oGTCustomCommandHelper_MouseUp(object sender, GTMouseEventArgs e)
        {
            try
            {
                if (PlaceValue == 1)
                {
                    PlaceValue = 11;
                }
                else if (PlaceValue == 2)
                {
                    PlaceValue = 21;
                }
            }
            catch (Exception e1)
            {
                throw e1;
            }
        }
        #endregion        

        #region placement service events

        private void placementService_Finished(object sender, GTFinishedEventArgs e)
        {
            EndCommand(false);
        }

        protected void EndCommand(bool canceled)
        {
            if (LabelCNO == 7230)
            {
                LabelCNO = 7234;
                placementService.StartComponent(oCableNew, LabelCNO);
                
            }
            else
            {
               
              //  placementService = null;

                placementService.Dispose();
                placementService.Finished -= EV_placementService_Finished;
                m_oGTTransactionManager.Commit();

                ////Update Cable Length
                //m_oGTTransactionManager.Begin("UpdateLength");
                //string FLength = Get_Value("select SDO_GEOM.SDO_LENGTH(G3E_GEOMETRY, 0.0005) from GC_FCBL_L where g3e_fid = " + oNewFeature.FID);
                //oNewFeature.Components.GetComponent(7201).Recordset.Update("CABLE_LENGTH", FLength);

                //string SLength = Get_Value("select SDO_GEOM.SDO_LENGTH(G3E_GEOMETRY, 0.0005) from GC_FCBL_L where g3e_fid = " + oFeatureToBreak.FID);
                //oFeatureToBreak.Components.GetComponent(7201).Recordset.Update("CABLE_LENGTH", SLength);
                //m_oGTTransactionManager.Commit();

                //Update GC_SPLICE_CONNECT Table   
              //  string HIGH = Get_Value("SELECT CABLE_SIZE as CORE FROM REF_FCBL WHERE MIN_MATERIAL IN (SELECT MIN_MATERIAL FROM GC_NETELEM WHERE G3E_FID = '" + bFID + "')");
                //  if (HIGH != "" && HIGH != null)
                //    FiberConnect(oFeatureToBreak, oSpliceFeature, oNewFeature, 1, int.Parse(HIGH), 1, int.Parse(HIGH), null);

                m_oGTTransactionManager.RefreshDatabaseChanges();
                m_gtapp.RefreshWindows();
                m_gtapp.EndWaitCursor();
              
                ExitCmd();
            }
        }
        #endregion
        
        #region button ESC

        void m_oGTCustomCommandHelper_KeyUp(object sender, GTKeyEventArgs e)
        {
            m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Press ESC to cancel.");
            
            if (e.KeyCode == 27)
            {
                ExitCmd();
            }

        }

        #endregion

        #region Termination cmd
        public bool CanTerminate
        {
            get
            {
                return true;
            }
        }

        public void Pause()
        {
        }

        public void Resume()
        {
        }

        public void Terminate()
        {
            try
            {

                if (m_oGTTransactionManager != null)
                {
                    if (m_oGTTransactionManager.TransactionInProgress)
                    {
                        m_oGTTransactionManager.Rollback();
                    }
                }

            }
            catch (Exception e)
            {
                throw e;
            }
        }
        #endregion       

        #region subscribe/ ussubs event
        public void SubscribeEvents()
        {
            // Subscribe to m_oIGTCustomCommandHelper events using C# 1.0 syntax
            m_oGTCustomCommandHelper.Activate += new EventHandler<GTActivateEventArgs>(m_oGTCustomCommandHelper_Activate);
            m_oGTCustomCommandHelper.Deactivate += new EventHandler<GTDeactivateEventArgs>(m_oGTCustomCommandHelper_Deactivate);
            m_oGTCustomCommandHelper.GainedFocus += new EventHandler<GTGainedFocusEventArgs>(m_oGTCustomCommandHelper_GainedFocus);
            m_oGTCustomCommandHelper.LostFocus += new EventHandler<GTLostFocusEventArgs>(m_oGTCustomCommandHelper_LostFocus);
            m_oGTCustomCommandHelper.KeyUp += new EventHandler<GTKeyEventArgs>(m_oGTCustomCommandHelper_KeyUp);
            m_oGTCustomCommandHelper.KeyDown += new EventHandler<GTKeyEventArgs>(m_oGTCustomCommandHelper_KeyDown);
            m_oGTCustomCommandHelper.KeyPress += new EventHandler<GTKeyPressEventArgs>(m_oGTCustomCommandHelper_KeyPress);
            m_oGTCustomCommandHelper.Click += new EventHandler<GTMouseEventArgs>(m_oGTCustomCommandHelper_Click);
            m_oGTCustomCommandHelper.DblClick += new EventHandler<GTMouseEventArgs>(m_oGTCustomCommandHelper_DblClick);
            m_oGTCustomCommandHelper.MouseMove += new EventHandler<GTMouseEventArgs>(m_oGTCustomCommandHelper_MouseMove);
            m_oGTCustomCommandHelper.MouseDown += new EventHandler<GTMouseEventArgs>(m_oGTCustomCommandHelper_MouseDown);
            m_oGTCustomCommandHelper.MouseUp += new EventHandler<GTMouseEventArgs>(m_oGTCustomCommandHelper_MouseUp);
            m_oGTCustomCommandHelper.WheelRotate += new EventHandler<GTWheelRotateEventArgs>(m_oGTCustomCommandHelper_WheelRotate);
        }

        private void UnsubscribeEvents()
        {
            // UnSubscribe to m_oIGTCustomCommandHelper events using C# 1.0 syntax
            m_oGTCustomCommandHelper.Activate -= m_oGTCustomCommandHelper_Activate;
            m_oGTCustomCommandHelper.Deactivate -= m_oGTCustomCommandHelper_Deactivate;
            m_oGTCustomCommandHelper.GainedFocus -= m_oGTCustomCommandHelper_GainedFocus;
            m_oGTCustomCommandHelper.LostFocus -= m_oGTCustomCommandHelper_LostFocus;
            m_oGTCustomCommandHelper.KeyUp -= m_oGTCustomCommandHelper_KeyUp;
            m_oGTCustomCommandHelper.KeyDown -= m_oGTCustomCommandHelper_KeyDown;
            m_oGTCustomCommandHelper.KeyPress -= m_oGTCustomCommandHelper_KeyPress;
            m_oGTCustomCommandHelper.Click -= m_oGTCustomCommandHelper_Click;
            m_oGTCustomCommandHelper.DblClick -= m_oGTCustomCommandHelper_DblClick;
            m_oGTCustomCommandHelper.MouseMove -= m_oGTCustomCommandHelper_MouseMove;
            m_oGTCustomCommandHelper.MouseDown -= m_oGTCustomCommandHelper_MouseDown;
            m_oGTCustomCommandHelper.MouseUp -= m_oGTCustomCommandHelper_MouseUp;
            m_oGTCustomCommandHelper.WheelRotate -= m_oGTCustomCommandHelper_WheelRotate;
        }
        #endregion

        #region Exit Cmd
        public void ExitCmd()
        {
            m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, " Exiting...");
            PlaceValue = 0;
            if (m_oGTTransactionManager != null)
            {
                if (m_oGTTransactionManager.TransactionInProgress)
                    m_oGTTransactionManager.Rollback();
                m_oGTTransactionManager = null;
            }

            if (mobjEditServiceTemp != null)
            {
                if(mobjEditServiceTemp.GeometryCount>0)
                    mobjEditServiceTemp.RemoveAllGeometries();
                mobjEditServiceTemp = null;
            }
            if (placementService != null)
            {
                placementService = null;
            }
            m_gtapp.EndWaitCursor();
           // m_gtapp.e
            if (m_gtapp != null)
            {
                m_gtapp = null;
            }
           
            UnsubscribeEvents();
            m_oGTCustomCommandHelper.Complete();
        }
        #endregion         
       
        #region Get Value from Database
        private string Get_Value(string sSql)
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
                return "";
            }

        }
#endregion

        #region Draw Splice

        public bool DrawSplice(short iFNO)
        {
            try
            {               
                short iCNO;
                int iFID = 0;
                IGTPointGeometry oPointGeom;
                short iCNOA = 0;
                short iCNOG = 0;
             // short iCNOL = 0;
               

             // m_oGTTransactionManager.Begin("DrawSplice");

                oSpliceFeature = m_gtapp.DataContext.NewFeature(iFNO);
                iFID = oSpliceFeature.FID;

                switch (iFNO)
                {
                    case 11800:
                        iCNOA = 11801;
                        iCNOG = 11820;
                       // iCNOL = 11830;
                        break;
                    case 4600:
                        iCNOA = 4601;
                        iCNOG = 4620;
                      //  iCNOL = 4630;
                        break;
                    case 4700:
                        iCNOA = 4701;
                        iCNOG = 4720;
                      //  iCNOL = 4730;
                        break;
                }                

                // NETELEM 51
                oSpliceFeature.Components.GetComponent(51).Recordset.Update("JOB_STATE", "PROPOSED");
                oSpliceFeature.Components.GetComponent(51).Recordset.Update("FEATURE_STATE", "PPF");

                //Attribute
                if (oSpliceFeature.Components.GetComponent(iCNOA).Recordset.EOF)
                {
                    oSpliceFeature.Components.GetComponent(iCNOA).Recordset.AddNew("G3E_FID", iFID);
                    oSpliceFeature.Components.GetComponent(iCNOA).Recordset.Update("G3E_FNO", iFNO);                    
                }
                else
                {
                    oSpliceFeature.Components.GetComponent(iCNOA).Recordset.Update("G3E_FID", iFID);
                    oSpliceFeature.Components.GetComponent(iCNOA).Recordset.Update("G3E_FNO", iFNO);
                }

                //Geometry                
                oPointGeom = GTClassFactory.Create<IGTPointGeometry>();
                oPointGeom.Origin = oPointPoint.FirstPoint;

                if (oSpliceFeature.Components.GetComponent(iCNOG).Recordset.EOF)
                {
                    oSpliceFeature.Components.GetComponent(iCNOG).Recordset.AddNew("G3E_FID", iFID);
                    oSpliceFeature.Components.GetComponent(iCNOG).Recordset.Update("G3E_FNO", iFNO);
                }
                else
                {
                    oSpliceFeature.Components.GetComponent(iCNOG).Recordset.MoveLast();
                }
                oSpliceFeature.Components.GetComponent(iCNOG).Geometry = oPointGeom;

                #region relationship with owner feature

                IGTRelationshipService mobjRelationshipService = GTClassFactory.Create<IGTRelationshipService>();
                mobjRelationshipService.DataContext = m_gtapp.DataContext;

                IGTKeyObject oOwnerFeature = m_gtapp.DataContext.OpenFeature(MFNO, MFID);
                mobjRelationshipService.ActiveFeature = oOwnerFeature;
                if (mobjRelationshipService.AllowSilentEstablish(oSpliceFeature))
                {
                    mobjRelationshipService.SilentEstablish(2, oSpliceFeature);
                }

                mobjRelationshipService.ActiveFeature = oSpliceFeature;
                if (mobjRelationshipService.AllowSilentEstablish(oOwnerFeature))
                {
                    mobjRelationshipService.SilentEstablish(3, oOwnerFeature);
                }
                
                #endregion

              //  m_oGTTransactionManager.Commit();
              //  m_oGTTransactionManager.RefreshDatabaseChanges();

                //MessageBox.Show("Completed", "Insert Splice", MessageBoxButtons.OK, MessageBoxIcon.Information);

                PlaceValue = 0;
                return true;
            }
            catch (Exception ex)
            {
                return false;
                log.WriteLog("");
                log.WriteLog(this.GetType().FullName, "ERROR: " + ex.Message);
                log.WriteLog(ex.StackTrace);
               // m_oGTTransactionManager.Rollback();
                MessageBox.Show(ex.Message, "Insert Splice", MessageBoxButtons.OK, MessageBoxIcon.Error);
                
            }
        }
        #endregion

        #region Fibre Connection

        private void FiberConnect(IGTKeyObject objFrom, IGTKeyObject objNode, IGTKeyObject objTo, int LOW1, int HIGH1, int LOW2, int HIGH2, string CORE_STATUS)
        {
            try
            {
                string CABLE_CODE = string.Empty;
                string EXC_ABB = null;
                string FEATURE_STATE = null;
                int recordsAffected = 0;
                int lastSEQ = 0;
                string SEQ = null;
                short G_CNO = 0;
                short L_CNO = 0;
                short S_CNO = 0;
                string SRC_FID = string.Empty;
                string SRC_FNO = string.Empty;
                string SRC_LOW = string.Empty;
                string SRC_HIGH = string.Empty;
                string sSql = null;
                ADODB.Recordset rs = new ADODB.Recordset();

                sSql = "SELECT SRC_FID, SRC_FNO, SRC_LOW, SRC_HIGH FROM GC_SPLICE_CONNECT WHERE G3E_FID IN (SELECT OUT_FID FROM GC_NR_CONNECT WHERE G3E_FID = " + objTo.FID + " AND G3E_FNO = " + objTo.FNO + ")";
                rs = m_gtapp.DataContext.OpenRecordset(sSql, ADODB.CursorTypeEnum.adOpenStatic, ADODB.LockTypeEnum.adLockReadOnly, 1);

                if (rs.RecordCount > 0)
                {
                    if (rs.Fields[0].Value.ToString() != "") SRC_FID = rs.Fields[0].Value.ToString();
                    if (rs.Fields[1].Value.ToString() != "") SRC_FNO = rs.Fields[1].Value.ToString();
                    if (rs.Fields[2].Value.ToString() != "") SRC_LOW = rs.Fields[2].Value.ToString();
                    if (rs.Fields[3].Value.ToString() != "") SRC_HIGH = rs.Fields[3].Value.ToString();
                }

                m_oGTTransactionManager.Begin("SpliceConnect");
                IGTKeyObject OFet = m_gtapp.DataContext.OpenFeature(objNode.FNO, objNode.FID);

                if (OFet.Components.GetComponent(77).Recordset.EOF)
                {
                    SEQ = "1";
                    OFet.Components.GetComponent(77).Recordset.AddNew("G3E_FNO", objNode.FNO);
                    OFet.Components.GetComponent(77).Recordset.Fields["SEQ"].Value = SEQ;
                    OFet.Components.GetComponent(77).Recordset.Fields["G3E_FNO"].Value = objNode.FNO;
                    OFet.Components.GetComponent(77).Recordset.Fields["G3E_FID"].Value = objNode.FID;
                    OFet.Components.GetComponent(77).Recordset.Fields["G3E_CNO"].Value = 77;
                    OFet.Components.GetComponent(77).Recordset.Fields["G3E_CID"].Value = 1;
                    OFet.Components.GetComponent(77).Recordset.Fields["FNO1"].Value = objFrom.FNO;
                    OFet.Components.GetComponent(77).Recordset.Fields["FID1"].Value = objFrom.FID;
                    OFet.Components.GetComponent(77).Recordset.Fields["LOW1"].Value = LOW1;
                    OFet.Components.GetComponent(77).Recordset.Fields["HIGH1"].Value = HIGH1;
                    OFet.Components.GetComponent(77).Recordset.Fields["FNO2"].Value = objTo.FNO;
                    OFet.Components.GetComponent(77).Recordset.Fields["FID2"].Value = objTo.FID;
                    OFet.Components.GetComponent(77).Recordset.Fields["LOW2"].Value = LOW2;
                    OFet.Components.GetComponent(77).Recordset.Fields["HIGH2"].Value = HIGH2;

                    OFet.Components.GetComponent(77).Recordset.Fields["NODE_FNO1"].Value = objNode.FNO;
                    OFet.Components.GetComponent(77).Recordset.Fields["NODE_FID1"].Value = objNode.FID;

                    OFet.Components.GetComponent(77).Recordset.Update("CONNECTION_TYPE", "Continuous");
                    OFet.Components.GetComponent(77).Recordset.Update("CORE_STATUS", CORE_STATUS);
                    OFet.Components.GetComponent(77).Recordset.Update("LOSS_MEASURED", "0");
                    OFet.Components.GetComponent(77).Recordset.Update("LOSS_TYPICAL", "0");

                    EXC_ABB = Get_Value("SELECT EXC_ABB FROM GC_NETELEM WHERE G3E_FID = " + objNode.FID);
                    FEATURE_STATE = Get_Value("SELECT FEATURE_STATE FROM GC_NETELEM WHERE G3E_FID = " + objNode.FID);
                    OFet.Components.GetComponent(77).Recordset.Update("EXC_ABB", EXC_ABB);
                    OFet.Components.GetComponent(77).Recordset.Update("FEATURE_STATE", FEATURE_STATE);

                    if (SRC_FID != "") OFet.Components.GetComponent(77).Recordset.Update("SRC_FID", SRC_FID);
                    if (SRC_FNO != "") OFet.Components.GetComponent(77).Recordset.Update("SRC_FNO", SRC_FNO);
                    if (SRC_LOW != "") OFet.Components.GetComponent(77).Recordset.Update("SRC_LOW", SRC_LOW);
                    if (SRC_HIGH != "") OFet.Components.GetComponent(77).Recordset.Update("SRC_HIGH", SRC_HIGH);
                    if (CABLE_CODE != "") OFet.Components.GetComponent(77).Recordset.Update("CABLE_CODE", CABLE_CODE);
                }
                else
                {
                    OFet.Components.GetComponent(77).Recordset.Fields["NODE_FNO1"].Value = objNode.FNO;
                    OFet.Components.GetComponent(77).Recordset.Fields["NODE_FID1"].Value = objNode.FID;

                    OFet.Components.GetComponent(77).Recordset.Update("CONNECTION_TYPE", "Continuous");
                    OFet.Components.GetComponent(77).Recordset.Update("CORE_STATUS", CORE_STATUS);
                    OFet.Components.GetComponent(77).Recordset.Update("LOSS_MEASURED", "0");
                    OFet.Components.GetComponent(77).Recordset.Update("LOSS_TYPICAL", "0");

                    EXC_ABB = Get_Value("SELECT EXC_ABB FROM GC_NETELEM WHERE G3E_FID = " + objNode.FID);
                    FEATURE_STATE = Get_Value("SELECT FEATURE_STATE FROM GC_NETELEM WHERE G3E_FID = " + objNode.FID);
                    OFet.Components.GetComponent(77).Recordset.Update("EXC_ABB", EXC_ABB);
                    OFet.Components.GetComponent(77).Recordset.Update("FEATURE_STATE", FEATURE_STATE);

                    if (SRC_FID != "") OFet.Components.GetComponent(77).Recordset.Update("SRC_FID", SRC_FID);
                    if (SRC_FNO != "") OFet.Components.GetComponent(77).Recordset.Update("SRC_FNO", SRC_FNO);
                    if (SRC_LOW != "") OFet.Components.GetComponent(77).Recordset.Update("SRC_LOW", SRC_LOW);
                    if (SRC_HIGH != "") OFet.Components.GetComponent(77).Recordset.Update("SRC_HIGH", SRC_HIGH);
                    if (CABLE_CODE != "") OFet.Components.GetComponent(77).Recordset.Update("CABLE_CODE", CABLE_CODE);
                }

                m_oGTTransactionManager.Commit();
            }
            catch (Exception ex)
            {
                m_oGTTransactionManager.Rollback();
                MessageBox.Show(ex.Message, "Insert Splice", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion

        #region break Cable
        private void BreakCable()
        {
            try
            {
                m_gtapp.BeginWaitCursor();
                m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Placing Splice Symbol ");
                m_oGTTransactionManager.Begin("BreakFeature");
                
                if (bFNO == 7200 || bFNO == 7400)
                {
                    if(!DrawSplice(11800))
                    { 
                        m_oGTTransactionManager.Rollback();
                        ExitCmd();
                        return;
                    }
                    //  AssociateMH(PlacedFNO, PlacedFID);
                }
                else if (bFNO == 4400)
                {
                    if(!DrawSplice(4600))
                        { 
                        m_oGTTransactionManager.Rollback();
                        ExitCmd();
                        return;
                    }
                    //  AssociateMH(PlacedFNO, PlacedFID);
                }
                else if (bFNO == 4500)
                {
                    if(!DrawSplice(4700))
                    {
                        m_oGTTransactionManager.Rollback();
                        ExitCmd();
                        return;
                    }
                    //  AssociateMH(PlacedFNO, PlacedFID);
                }
                m_gtapp.EndWaitCursor();

                if (bFNO > 0 && bFID > 0)
                {
                
                    IGTPoint oBreakPoint = null;
                    short CCNOG = 0;
                    short SCNOG = 0;

                   
                    m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Breaking feature...");
                    IGTBreakLinearService GTBLS = GTClassFactory.Create<IGTBreakLinearService>();

                    if (bFNO == 7200)
                    {
                        CCNOG = 7210;
                        SCNOG = 11820;
                        LabelCNO = 7230;
                    }
                    else if (bFNO == 7400)
                    {
                        CCNOG = 7410;
                        SCNOG = 11820;
                        LabelCNO = 7430;
                    }
                    else if (bFNO == 4400)
                    {
                        CCNOG = 4410;
                        SCNOG = 4620;
                        LabelCNO = 4430;
                    }
                    else if (bFNO == 4500)
                    {
                        CCNOG = 4510;
                        SCNOG = 4720;
                        LabelCNO = 4530;
                    }
                   
                    oCableToBreak = m_gtapp.DataContext.OpenFeature(bFNO, bFID);
                    oCableToBreak.CNO = CCNOG;
                    oCableToBreak.CID = 1;
                    oBreakPoint = GTClassFactory.Create<IGTPoint>();
                    oBreakPoint.X = oPointPoint.FirstPoint.X;
                    oBreakPoint.Y = oPointPoint.FirstPoint.Y;
                    oBreakPoint.Z = oPointPoint.FirstPoint.Z;

                    GTBLS.DataContext = m_gtapp.DataContext;
                    GTBLS.FeatureToBreak = oCableToBreak;
                    GTBLS.BreakPoint = oBreakPoint;
                    GTBLS.MapWindow = m_gtapp.Application.ActiveMapWindow;

                   // IGTGeometry test3 = oCableToBreak.Components.GetComponent(7410).Geometry;
                  //  IGTKeyObject oSpliceFeature1 = m_gtapp.DataContext.OpenFeature(oSpliceFeature.FNO, oSpliceFeature.FID);
                    oSpliceFeature.CNO = SCNOG;
                    oSpliceFeature.CID = 1;
                    oCableNew = GTBLS.Execute(oSpliceFeature, true, GTConnectOptionsConstants.gtcoInline);
                   // IGTGeometry test1 = oCableNew.Components.GetComponent(7410).Geometry;
                  //  IGTGeometry test2 = oCableToBreak.Components.GetComponent(7410).Geometry;
                    m_oGTTransactionManager.Commit();
                    m_oGTTransactionManager.RefreshDatabaseChanges();
                    m_gtapp.RefreshWindows();
                    m_gtapp.EndWaitCursor();
                    ExitCmd();
                    return;

                    //oCableNew = m_gtapp.DataContext.OpenFeature(oCableNew.FNO, oCableNew.FID);
                    //if (!oCableNew.Components.GetComponent(LabelCNO).Recordset.EOF)
                    //{

                    //    IGTGeometry test = oCableNew.Components.GetComponent(LabelCNO).Geometry;

                    //    if (oCableNew.Components.GetComponent(LabelCNO).Geometry == null)
                    //    {
                    //      //  m_oGTTransactionManager.Begin("Place Label For cable");

                    //        //text format for D side
                    //        if (bFNO == 7400)
                    //        {
                    //            if (oCableNew.Components.GetComponent(7401).Recordset.EOF)
                    //            {
                    //                oCableNew.Components.GetComponent(7401).Recordset.AddNew("G3E_FID", oCableNew.FID);
                    //                oCableNew.Components.GetComponent(7401).Recordset.Update("G3E_FNO", oCableNew.FNO);
                    //                oCableNew.Components.GetComponent(7401).Recordset.Update("TEXT_FORMAT", "FULL TEXT");
                    //            }
                    //            else
                    //            {
                    //                oCableNew.Components.GetComponent(7401).Recordset.Update("TEXT_FORMAT", "FULL TEXT");
                    //            }
                    //        }

                    //        placementService = GTClassFactory.Create<IGTFeaturePlacementService>(m_oGTCustomCommandHelper);
                    //        EV_placementService_Finished = new EventHandler<GTFinishedEventArgs>(placementService_Finished);
                    //        placementService.Finished += EV_placementService_Finished;
                    //        placementService.StartComponent(oCableNew, LabelCNO);
                    //    }
                    //    else
                    //    {
                    //        m_gtapp.RefreshWindows();
                    //        m_gtapp.EndWaitCursor();
                    //        ExitCmd();
                    //    }
                    //}
                    //else
                    //{

                       
                    //       // m_oGTTransactionManager.Begin("Place Label For cable");

                    //        //text format for D side
                    //        if (bFNO == 7400)
                    //        {
                    //            if (oCableNew.Components.GetComponent(7401).Recordset.EOF)
                    //            {
                    //                oCableNew.Components.GetComponent(7401).Recordset.AddNew("G3E_FID", oCableNew.FID);
                    //                oCableNew.Components.GetComponent(7401).Recordset.Update("G3E_FNO", oCableNew.FNO);
                    //                oCableNew.Components.GetComponent(7401).Recordset.Update("TEXT_FORMAT", "FULL TEXT");
                    //            }
                    //            else
                    //            {
                    //                oCableNew.Components.GetComponent(7401).Recordset.Update("TEXT_FORMAT", "FULL TEXT");
                    //            }


                    //        }

                    //        placementService = GTClassFactory.Create<IGTFeaturePlacementService>(m_oGTCustomCommandHelper);
                    //        EV_placementService_Finished = new EventHandler<GTFinishedEventArgs>(placementService_Finished);
                    //        placementService.Finished += EV_placementService_Finished;
                    //        placementService.StartComponent(oCableNew, LabelCNO);
                       
                      
                    //}

                }
     
                // ExitCmd();
            }
            catch (Exception ex)
            {
                if(m_oGTTransactionManager.TransactionInProgress)
                    m_oGTTransactionManager.Rollback();
                MessageBox.Show(ex.Message, "Insert Splice", MessageBoxButtons.OK, MessageBoxIcon.Error);
                ExitCmd();
            }
        }
        #endregion
    }

}
