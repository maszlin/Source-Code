/*
 * Manhole placement - Modeless Custom Command
 * 
 * Author: La Viet Phuong - AG
 * 
 * This modeless custom command gives the user a list of features and a input text box. The user can select one of those features
 * and input a value, then another list will open with all the placed features that contain that value. When the user selects
 * one of those features the map window will zoom on it.
 * 
 */
using ADODB;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Intergraph.GTechnology.Interfaces;
using Intergraph.GTechnology.API;
using System.Diagnostics;
using AG.GTechnology.Utilities;

namespace AG.GTechnology.StubStumpDelete
{
    class GTStubStumpDelete : Intergraph.GTechnology.Interfaces.IGTCustomCommandModeless
    {
        private Intergraph.GTechnology.API.IGTTransactionManager m_oGTTransactionManager;
        private Intergraph.GTechnology.API.IGTCustomCommandHelper m_oGTCustomCommandHelper;
        private static Intergraph.GTechnology.API.IGTApplication application = GTClassFactory.Create<IGTApplication>();
        private Logger log;
        private IGTDataContext oDataContext = application.DataContext;
        private static IGTPolylineGeometry oLineGeom = null;
        private static List<IGTGeometry> arrIntersection = new List<IGTGeometry>();
        private static List<IGTGeometry> arrIntersectionText = new List<IGTGeometry>();
        private string mstrFistPoint = "Please enter the attributes for Stub/Stump.";
        private int lStyleId = 11508;

        bool mblnVisible = false;
        int mintState;
        //IGTGeometryCreationService mobjCreationService;
        //IGTFeaturePlacementService mobjPlacementService;
        IGTGeometryEditService mobjEditService;
        IGTGeometryEditService mobjEditPointService;
        IGTGeometryEditService mobjEditServiceTemp;
        IGTLocateService mobjLocateService;
        IGTFeatureExplorerService mobjExplorerService;

        IGTPoint mptSelected;
        IGTPoint mptPoint1;
        IGTPoint mptPoint2;
        IGTKeyObject mobjAttribute = null;

        private int iDetailID = 0;

        private short iSourceCblFNO = 7000;
        private int iSourceCblFID = 0;
        private IGTKeyObject iSourceCbl = null;
        private string CableSide = string.Empty;

        private short iTermFNO = 7000;
        private int iTermFID = 0;
        private IGTGeometry iTermGeom = null;

        private clsStumpStub cStubStump = null;

        private int StyleManhole = 2730001;
        private int StyleTempManhole = 2730001;

        Dictionary<string, string> Parameters = new Dictionary<string, string>();

        private void DeleteStumpStub(short iFNO, int iFID)
        {
            IGTKeyObject oNewFeature;

            Recordset rsComp = null;
            int recordsAffected = 0;

            string CableClass = string.Empty;

            string sSql = "select b.CABLE_CLASS from GC_CBL b where b.G3E_FNO=" + iFNO.ToString() + " and G3E_FID=" + iFID.ToString();
            rsComp = application.DataContext.Execute(sSql, out recordsAffected, (int)CommandTypeEnum.adCmdText, null);

            if (rsComp != null)
            {
                if (!rsComp.EOF)
                {
                    if (rsComp.Fields["CABLE_CLASS"].Value != DBNull.Value) CableClass = rsComp.Fields["CABLE_CLASS"].Value.ToString();
                }
            }
            rsComp = null;

            if ((CableClass.Contains("STUMP")) || (CableClass.Contains("STUB")))
            {
                DialogResult ret = MessageBox.Show("Are you sure to delete stub/stump " + iFID.ToString(), "Message", MessageBoxButtons.YesNo);

                if (ret == DialogResult.Yes)
                {
                    iTermFNO = 0;
                    sSql = "select b.OUT_FNO, b.OUT_FID from GC_NR_CONNECT b where b.G3E_FNO=" + iFNO.ToString() + " and G3E_FID=" + iFID.ToString();
                    rsComp = application.DataContext.Execute(sSql, out recordsAffected, (int)CommandTypeEnum.adCmdText, null);

                    if (rsComp != null)
                    {
                        if (!rsComp.EOF)
                        {
                            if (rsComp.Fields["OUT_FNO"].Value != DBNull.Value) iTermFNO = Convert.ToInt16(rsComp.Fields["OUT_FNO"].Value);
                            if (rsComp.Fields["OUT_FID"].Value != DBNull.Value) iTermFID = Convert.ToInt32(rsComp.Fields["OUT_FID"].Value);
                        }
                    }
                    rsComp = null;

                    m_oGTTransactionManager.Begin("Delete stub/stump");
                    //oNewFeature = application.DataContext.OpenFeature(iFNO, iFID);
                    PGeoLib.DeleteComponents(iFNO, iFID);

                    if (iTermFNO > 0)
                    {
                        //oNewFeature = application.DataContext.OpenFeature(iTermFNO, iTermFID);
                        PGeoLib.DeleteComponents(iTermFNO, iTermFID);
                    }
                    m_oGTTransactionManager.Commit();
                    m_oGTTransactionManager.RefreshDatabaseChanges();

                }
            }
            mintState = 2;
        }

        public void Activate(Intergraph.GTechnology.API.IGTCustomCommandHelper CustomCommandHelper)
        {
            try
            {
                application.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, mstrFistPoint);

                log = Logger.getInstance();
                log.WriteLog(this.GetType().FullName, "Stub/Stump Delete started!");
                log.WriteLog("");

                m_oGTCustomCommandHelper = CustomCommandHelper;

                cStubStump = new clsStumpStub();

                if (application.SelectedObjects.FeatureCount == 1)
                {
                    foreach (IGTDDCKeyObject oDDCKeyObject in application.SelectedObjects.GetObjects())
                    {
                        if ((oDDCKeyObject.ComponentViewName == "VGC_CBL_L") || (oDDCKeyObject.ComponentViewName == "VGC_CBL_T"))
                        {
                            iSourceCblFNO = oDDCKeyObject.FNO;
                            iSourceCblFID = oDDCKeyObject.FID;
                        }
                        else if ((oDDCKeyObject.ComponentViewName == "VGC_DCBL_L") || (oDDCKeyObject.ComponentViewName == "VGC_DCBL_T"))
                        {
                            iSourceCblFNO = oDDCKeyObject.FNO;
                            iSourceCblFID = oDDCKeyObject.FID;
                        }
                    }
                }

                mintState = 1;
                //  Assigns the private member variables their default values.
                if (iSourceCblFID > 0)
                {
                    DeleteStumpStub(iSourceCblFNO, iSourceCblFID);
                }
                else
                    mintState = 2;

                iDetailID = application.ActiveMapWindow.DetailID;

                mobjEditService = GTClassFactory.Create<IGTGeometryEditService>();
                mobjEditService.TargetMapWindow = application.ActiveMapWindow;

                mobjEditPointService = GTClassFactory.Create<IGTGeometryEditService>();
                mobjEditPointService.TargetMapWindow = application.ActiveMapWindow;

                mobjEditServiceTemp = GTClassFactory.Create<IGTGeometryEditService>();
                mobjEditServiceTemp.TargetMapWindow = application.ActiveMapWindow;

                mobjLocateService = application.ActiveMapWindow.LocateService;

                mobjExplorerService = GTClassFactory.Create<IGTFeatureExplorerService>();

                RegisterEvents();
            }
                
            catch (Exception e)
            {
                log.WriteLog("");
                log.WriteLog(this.GetType().FullName, "ERROR: " + e.Message);
                log.WriteLog(e.StackTrace);
                MessageBox.Show(e.Message, "Error.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                ExitCmd();
            }
        }

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
            //try
            //{

                if (m_oGTTransactionManager != null)
                {
                    if (m_oGTTransactionManager.TransactionInProgress)
                    {
                        m_oGTTransactionManager.Rollback();
                    }
                }

                log.CloseFile();
                application.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Exiting...");
                if (mobjEditService != null)
                {
                    mobjEditService.RemoveAllGeometries();
                    mobjEditService = null;
                }
                if (mobjEditPointService != null)
                {
                    mobjEditPointService.RemoveAllGeometries();
                    mobjEditPointService = null;
                }
                if (mobjEditServiceTemp != null)
                {
                    mobjEditServiceTemp.RemoveAllGeometries();
                    mobjEditServiceTemp = null;
                }

                CloseFeatureExplorer();

                //if (!(mobjExplorerService == null))
                //{
                //    mobjExplorerService.Clear();
                //}

                //if ((!(mobjExplorerService == null)
                //            && !mblnVisible))
                //{
                //    mobjExplorerService.Visible = false;
                //}
                //mobjCreationService = null;
                //mobjPlacementService = null;
                mptSelected = null;

            //}
            //catch (Exception e)
            //{
            //    throw e;
            //}
        }

        public Intergraph.GTechnology.API.IGTTransactionManager TransactionManager
        {
            set
            {
                m_oGTTransactionManager = value;
            }
        }

        private void ExitCmd()
        {
            try
            {
                application.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Exiting...");
                UnRegisterEvents();
                m_oGTCustomCommandHelper.Complete();
                application.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Exited.");
            }
            catch (Exception ex)
            {
                //Logger log = Logger.GetInstance();
                //log.WriteLog("Error", "ExitCmd", ex.Message);
                //log.WriteLog(ex.StackTrace);
                //MessageBox.Show(ex.Message, "Error.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //log.CloseFile();
            }
        }

        private void m_oCustomCommandHelper_MouseUp(object sender, GTMouseEventArgs MouseEventArgs)
        {

            try
            {
                IGTPoint WorldPoint = MouseEventArgs.WorldPoint;

                if (MouseEventArgs.Button == 2)
                {
                    ExitCmd();
                    return;
                }
                else
                {
                    //  If the current step in the command is the third step then get the selected point.
                    if (mintState == 2)
                    {
                        double X = WorldPoint.X;
                        double Y = WorldPoint.Y;

                        IGTDDCKeyObjects objs = mobjLocateService.Locate(WorldPoint, 20, 1, GTSelectionTypeConstants.gtmwstSelectSingle);

                        iSourceCblFID = 0;
                        foreach (IGTDDCKeyObject oDDCKeyObject in objs)
                        {
                            if (oDDCKeyObject.FNO == 7000)
                            {
                                iSourceCblFNO = oDDCKeyObject.FNO;
                                iSourceCblFID = oDDCKeyObject.FID;
                            }
                        }
                        if (iSourceCblFID > 0)
                        {
                            DeleteStumpStub(iSourceCblFNO, iSourceCblFID);
                        }
                    }
                    else 
                        ExitCmd();
                }
            }
            catch (Exception ex)
            {
                //Logger log = Logger.GetInstance();
                log.WriteLog("Error", "m_oCustomCommandHelper_MouseUp", ex.Message);
                log.WriteLog(ex.StackTrace);
                MessageBox.Show(ex.Message, "Error.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                if (m_oGTTransactionManager != null) m_oGTTransactionManager.Rollback();
                //log.CloseFile();
            }
        }

        private void m_oCustomCommandHelper_MouseMove(object sender, GTMouseEventArgs MouseEventArgs)
        {
            try
            {
                IGTPoint WorldPoint = MouseEventArgs.WorldPoint;

                if ((mintState == 1))
                {
                    application.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Ready.");
                }
                else if ((mintState == 2))
                {
                    application.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Identify stub/stump to delete or right-click to exit.");
                }
                //System.Windows.Forms.Application.DoEvents();
            }
            catch (Exception ex)
            {
                log.WriteLog("Error", "m_oCustomCommandHelper_MouseMove", ex.Message);
                log.WriteLog(ex.StackTrace);
                MessageBox.Show(ex.Message, "Error.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //log.CloseFile();
            }
        }

        //******************************************************************************
        //********************** GTFeatureExplorerService Events. **********************
        //******************************************************************************
        private void CloseFeatureExplorer()
        {
            // Clears feature explorer, if it was set.
            if (!(mobjExplorerService == null))
            {
                mobjExplorerService.Clear();
            }


            // If feature explorer was not originally displayed before this command was
            // started then ensure it is not displayed after. The only reason why it was
            // displayed is because this command displayed it and the command should reset
            // the state back to the original conditions.
            if ((!(mobjExplorerService == null)
                        && !mblnVisible))
            {
                mobjExplorerService.Visible = false;
            }

        }

        private void SaveAttribute()
        {
            if (cStubStump == null) return;

            if (mintState == 3)
            {
                if (cStubStump.Type == "Stump")
                    mintState = 4;
                else
                    mintState = 5;

                m_oGTTransactionManager.Rollback();

                if ((!(mobjExplorerService == null)
                        && mblnVisible))
                {
                    mobjExplorerService.Visible = false;
                }
            }
            else
            {
                CloseFeatureExplorer();
            }
        }

        //******************************************************************************
        // Purpose:  Represents the method that will handle the CancelClick event.
        //
        //******************************************************************************
        private void mobjExplorerService_CancelClick(object sender, EventArgs e)
        {
            // Throws any errors raised to the error handler.
            try
            {

                ExitCmd();

            }
            catch (Exception ex)
            {
                log.WriteLog("Error", "m_oCustomCommandHelper_MouseMove", ex.Message);
                log.WriteLog(ex.StackTrace);
                MessageBox.Show(ex.Message, "Error.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //log.CloseFile();
            }
        }

        //******************************************************************************
        // Purpose:  Represents the method that will handle the SaveAndContinueClick
        //           event.
        //
        //******************************************************************************
        private void mobjExplorerService_SaveAndContinueClick(object sender, EventArgs e)
        {
            // Throws any errors raised to the error handler.
            try
            {

                // Transitions the state of the command to the next step in the placement work flow.
                SaveAttribute();

                // Catches any errors thrown to the error handler and displays a message box with the description.
            }
            catch (Exception ex)
            {
                log.WriteLog("Error", "m_oCustomCommandHelper_MouseMove", ex.Message);
                log.WriteLog(ex.StackTrace);
                MessageBox.Show(ex.Message, "Error.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //log.CloseFile();
            }

        }


        //******************************************************************************
        // Purpose:  Represents the method that will handle the SaveClick event.
        //
        //******************************************************************************
        private void mobjExplorerService_SaveClick(object sender, EventArgs e)
        {
            // Throws any errors raised to the error handler.
            try
            {

                // Transitions the state of the command to the next step in the placement work flow.
                SaveAttribute();

                // Catches any errors thrown to the error handler and displays a message box with the description.
            }
            catch (Exception ex)
            {
                log.WriteLog("Error", "m_oCustomCommandHelper_MouseMove", ex.Message);
                log.WriteLog(ex.StackTrace);
                MessageBox.Show(ex.Message, "Error.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //log.CloseFile();
            }

        }


        /// <summary>
        /// Register EventHandlers on CommandHelper
        /// </summary>
        /// <param name=""></param>
        #region private void RegisterEvents()
        private void RegisterEvents()
        {
            try
            {
                //m_oGTCustomCommandHelper.KeyUp += new EventHandler<GTKeyEventArgs>(m_oCustomCommandHelper_KeyUp);
                //m_oGTCustomCommandHelper.Click += new EventHandler<GTMouseEventArgs>(m_oCustomCommandHelper_Click);
                //m_oGTCustomCommandHelper.MouseDown += new EventHandler<GTMouseEventArgs>(m_oCustomCommandHelper_MouseDown);
                m_oGTCustomCommandHelper.MouseUp += new EventHandler<GTMouseEventArgs>(m_oCustomCommandHelper_MouseUp);
                m_oGTCustomCommandHelper.MouseMove += new EventHandler<GTMouseEventArgs>(m_oCustomCommandHelper_MouseMove);
                //m_oCustomCommandHelper.MouseMove += new EventHandler(this.m_oCustomCommandHelper_MouseMove);

                mobjExplorerService.CancelClick += new EventHandler(mobjExplorerService_CancelClick);
                mobjExplorerService.SaveAndContinueClick += new EventHandler(mobjExplorerService_SaveAndContinueClick);
                mobjExplorerService.SaveClick += new EventHandler(mobjExplorerService_SaveClick);
            }
            catch (Exception ex)
            {
                //Logger log = Logger.GetInstance();
                log.WriteLog("Error", "RegisterEvents", ex.Message);
                log.WriteLog(ex.StackTrace);
                MessageBox.Show(ex.Message, "Error.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //log.CloseFile();
            }
        }
        #endregion

        /// <summary>
        /// UnRegister EventHandlers on CommandHelper
        /// </summary>
        /// <param name=""></param>
        #region private void UnRegisterEvents()
        private void UnRegisterEvents()
        {
            try
            {
                //m_oGTCustomCommandHelper.KeyUp -= m_oCustomCommandHelper_KeyUp;
                //m_oGTCustomCommandHelper.Click -= m_oCustomCommandHelper_Click;
                //m_oGTCustomCommandHelper.MouseDown -= m_oCustomCommandHelper_MouseDown;
                m_oGTCustomCommandHelper.MouseUp -= m_oCustomCommandHelper_MouseUp;
                m_oGTCustomCommandHelper.MouseMove -= m_oCustomCommandHelper_MouseMove;
                //m_oCustomCommandHelper.MouseMove += new EventHandler(this.m_oCustomCommandHelper_MouseMove);

                mobjExplorerService.CancelClick -= new EventHandler(mobjExplorerService_CancelClick);
                mobjExplorerService.SaveAndContinueClick -= new EventHandler(mobjExplorerService_SaveAndContinueClick);
                mobjExplorerService.SaveClick -= new EventHandler(mobjExplorerService_SaveClick);
            }
            catch (Exception ex)
            {
                //Logger log = Logger.GetInstance();
                log.WriteLog("Error", "UnRegisterEvents", ex.Message);
                log.WriteLog(ex.StackTrace);
                MessageBox.Show(ex.Message, "Error.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //log.CloseFile();
            }
        }
        #endregion

    }
}
