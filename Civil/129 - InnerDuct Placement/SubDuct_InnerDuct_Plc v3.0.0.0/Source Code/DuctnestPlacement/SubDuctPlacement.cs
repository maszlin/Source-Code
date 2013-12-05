/*
 * Locate Feature - Modeless Custom Command
 * 
 * Author: Felipe Armoni - Credent
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
using AG.GTechnology.SubDuctPlacement.Forms;
using System.Diagnostics;
using AG.GTechnology.Utilities;

namespace AG.GTechnology.SubDuctPlacement
{
    class GTSubDuctPlacement : Intergraph.GTechnology.Interfaces.IGTCustomCommandModeless
    {
        private Intergraph.GTechnology.API.IGTTransactionManager m_oGTTransactionManager;
        private Intergraph.GTechnology.API.IGTCustomCommandHelper m_oGTCustomCommandHelper;
        private static Intergraph.GTechnology.API.IGTApplication application = GTClassFactory.Create<IGTApplication>();
        private SubDuctForm _SubDuctForm;
        private Logger log;
        private IGTDataContext oDataContext = application.DataContext;
        private static IGTPolylineGeometry oLineGeom = null;
        private static List<IGTGeometry> arrIntersection = new List<IGTGeometry>();
        private static List<IGTGeometry> arrIntersectionText = new List<IGTGeometry>();
        private string mstrFistPoint = "Please enter the attributes for subduct.";
        private int lStyleId = 11508;

        private double Pi = 3.141592653589793;

        bool mblnVisible = false;
        int mintState;
        //IGTGeometryCreationService mobjCreationService;
        //IGTFeaturePlacementService mobjPlacementService;
        IGTGeometryEditService mobjEditService;
        IGTGeometryEditService mobjEditPointService;
        IGTFeatureExplorerService mobjExplorerService;
        //IGTKeyObject mobjContourFeature;
        IGTPoint mptSelected;
        IGTPoint mptPoint1;
        IGTPoint mptPoint2;
        IGTKeyObject mobjSubDuctAttribute = null;

        private short iSubDuctFNO = 16100;
        private short iDuctFNO = 2300;
        private int iDuctFID = 0;
        private int _nDucts = 0;
        private IGTKeyObject oDuct = null;
        private IGTGeometry iDuctGeom = null;
        private int numberofSDucts = 0;

        private int StyleDuct = 2320031;
        private int StyleSDuct = 2120002;
        private int StyleDuctnest = 10621;

        double SDuctHSpace = 0.05;
        double SDuctVSpace = 0.05;

        double SDuctHOffset = 0.10;
        double SDuctVOffset = 0.10;

        Dictionary<string, string> Parameters = new Dictionary<string, string>();

        private void LoadParameters()
        {
            Recordset rsComp = null;
            int recordsAffected = 0;

            string sSql = "select b.G3E_TYPE, b.G3E_PARAMETER1 from G3E_DUCTIFACE b";
            rsComp = application.DataContext.Execute(sSql, out recordsAffected, (int)CommandTypeEnum.adCmdText, null);

            if (rsComp != null)
            {
                while (!rsComp.EOF)
                {
                    if (rsComp.Fields["G3E_TYPE"].Value != DBNull.Value) Parameters.Add(rsComp.Fields["G3E_TYPE"].Value.ToString(), rsComp.Fields["G3E_PARAMETER1"].Value.ToString());

                    rsComp.MoveNext();
                }
            }
            rsComp = null;

            if (Parameters["G3E_DHSPACING"] != null) SDuctHSpace = Convert.ToDouble(Parameters["G3E_IDUCT_DHSPACING"]);
            if (Parameters["G3E_DVSPACING"] != null) SDuctVSpace = Convert.ToDouble(Parameters["G3E_IDUCT_DVSPACING"]);

            if (Parameters["G3E_DHOFFSET"] != null) SDuctHOffset = Convert.ToDouble(Parameters["G3E_IDUCT_DHOFFSET"]);
            if (Parameters["G3E_DVOFFSET"] != null) SDuctVOffset = Convert.ToDouble(Parameters["G3E_IDUCT_DVOFFSET"]);
        }

        public bool CheckDuctpath()
        {
            Recordset rsComp = null;
            int recordsAffected = 0;

            return true;
            if (iDuctFID == 0) return false;
            string sSql = "select count(*) DT_WAYS from GC_CONTAIN b where b.G3E_FNO=16100 and b.G3E_OWNERFID=" + iDuctFID.ToString();
            rsComp = application.DataContext.Execute(sSql, out recordsAffected, (int)CommandTypeEnum.adCmdText, null);

            if (rsComp != null)
            {
                if (!rsComp.EOF)
                {
                    if (rsComp.Fields["DT_WAYS"].Value != DBNull.Value) numberofSDucts = Convert.ToInt32(rsComp.Fields["DT_WAYS"].Value);

                    if (numberofSDucts >= 3) return false;
                }
            }
            rsComp = null;

            return true;
        }

        private IGTKeyObject CreateSubDuct(double X, double Y, ConnectType type)
        {
            short iFNO;
            short iCNO;
            long lFID;
            double dRotation;
            IGTKeyObject oNewFeature;

            IGTPoint oPointGeom;
            IGTTextPointGeometry oOrTextGeom;
            IGTOrientedPointGeometry oOrPointGeom = null;

            iFNO = iSubDuctFNO;
            oNewFeature = application.DataContext.NewFeature(iFNO);

            // FID generation.
            lFID = oNewFeature.FID;

            if (mobjSubDuctAttribute != null)
                PGeoLib.CopyComponentAttribute(mobjSubDuctAttribute, oNewFeature);
            else
            {
                // Every feature is imported in the Existing state.
                iCNO = 51; //GC_NETELEM
                if (oNewFeature.Components.GetComponent(iCNO).Recordset.EOF)
                {
                    oNewFeature.Components.GetComponent(iCNO).Recordset.AddNew("G3E_FID", lFID);
                    oNewFeature.Components.GetComponent(iCNO).Recordset.Update("JOB_STATE", "CLD");
                    oNewFeature.Components.GetComponent(iCNO).Recordset.Update("FEATURE_STATE", "EXT");
                    oNewFeature.Components.GetComponent(iCNO).Recordset.Update("SWITCH_CENTRE_CLLI", "PPJ");
                }
                else
                {
                    oNewFeature.Components.GetComponent(iCNO).Recordset.Update("JOB_STATE", "CLD");
                    oNewFeature.Components.GetComponent(iCNO).Recordset.Update("FEATURE_STATE", "EXT");
                    oNewFeature.Components.GetComponent(iCNO).Recordset.Update("SWITCH_CENTRE_CLLI", "PPJ");
                }

                // Attribute
                iCNO = 16101;
                oNewFeature.Components.GetComponent(iCNO).Recordset.Update("FEATURE_TYPE", "THROUGH");
            }

            // Duct Point
            if (type== ConnectType.from)
                iCNO = 16120;
            else
                iCNO = 16122;
            if (oNewFeature.Components.GetComponent(iCNO).Recordset.EOF)
            {
                oNewFeature.Components.GetComponent(iCNO).Recordset.AddNew("G3E_FID", lFID);
                oNewFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_FNO", iFNO);
            }
            else
            {
                oNewFeature.Components.GetComponent(iCNO).Recordset.MoveLast();
            }

            // GetCompInfo iFNO, iCNO, bMainGr, grType
            oOrPointGeom = PGeoLib.CreatePointGeom(X, Y);

            oNewFeature.Components.GetComponent(iCNO).Geometry = oOrPointGeom;
            return oNewFeature;
        }

        private void PlaceDuct(double X, double Y, int nDucts, ConnectType type)
        {
            IGTKeyObject oDuctnest;
            IGTKeyObject oDuct;
            IGTRelationshipService mobjRelationshipService;

            short mintContainRelationshipNumber = 7;

            string mstrTransactionName = "Place SubDuct";
            m_oGTTransactionManager.Begin(mstrTransactionName);
            mobjRelationshipService = GTClassFactory.Create<IGTRelationshipService>(m_oGTCustomCommandHelper);
            mobjRelationshipService.DataContext = application.DataContext;

            oDuct = application.DataContext.OpenFeature(iDuctFNO, iDuctFID);

            IGTKeyObject oSubDuct;
            for (int i = 0; i < nDucts; i++)
            {
                double xSubDuct = X + Math.Sin(i * 2 * Pi / nDucts) * SDuctHSpace;
                double ySubDuct = Y + Math.Cos(i * 2 * Pi / nDucts) * SDuctVSpace;
                oSubDuct = CreateSubDuct(xSubDuct, ySubDuct, type);
                //Ownership        
                mobjRelationshipService.ActiveFeature = oDuct;
                if (mobjRelationshipService.AllowSilentEstablish(oSubDuct))
                {
                    mobjRelationshipService.SilentEstablish(mintContainRelationshipNumber, oSubDuct);
                }
            }
            m_oGTTransactionManager.Commit();
            m_oGTTransactionManager.RefreshDatabaseChanges();
        }

        private void LoadDuctConfig(int DuctFid)
        {
            IGTMapWindow mobjMapWindow = application.ActiveMapWindow;

            _SubDuctForm = new SubDuctForm(m_oGTCustomCommandHelper);
            _SubDuctForm.FormClosed += new FormClosedEventHandler(SubDuctForm_FormClosed);
            _SubDuctForm.PlaceClick += new EventHandler(SubDuctForm_PlaceClick);
            _SubDuctForm.NumberOfSubduct = 3;
            _SubDuctForm.Show();
        }

        public void Activate(Intergraph.GTechnology.API.IGTCustomCommandHelper CustomCommandHelper)
        {
            try
            {
                application.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, mstrFistPoint);

                log = Logger.getInstance();
                log.WriteLog(this.GetType().FullName, "Duct Placement started!");
                log.WriteLog("");

                m_oGTCustomCommandHelper = CustomCommandHelper;

                if (application.SelectedObjects.FeatureCount == 0)
                {
                    MessageBox.Show("Please select an Existing Duct", "Duct Config", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    ExitCmd();
                    return;
                }

                if (application.SelectedObjects.FeatureCount > 1)
                {
                    MessageBox.Show("Please select single feature at once", "Duct Config", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    ExitCmd();
                    return;
                }

                foreach (IGTDDCKeyObject oDDCKeyObject in application.SelectedObjects.GetObjects())
                {
                    if ((oDDCKeyObject.ComponentViewName == "VGC_DUCTF_S") || (oDDCKeyObject.ComponentViewName == "VGC_DUCTT_S"))
                    {
                        iDuctGeom = oDDCKeyObject.Geometry;
                        iDuctFNO = oDDCKeyObject.FNO;
                        iDuctFID = oDDCKeyObject.FID;
                    }
                }

                if (iDuctFNO != 2300)
                {
                    MessageBox.Show("Please select an Existing Duct", "SubDuct Config", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    ExitCmd();
                    return;
                }

                if (!CheckDuctpath())
                {
                    MessageBox.Show("All spare space in the duct has been allocated", "SubDuct Config", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    ExitCmd();
                    return;
                }                

                //  Assigns the private member variables their default values.
                mintState = 1;

                mobjEditService = GTClassFactory.Create<IGTGeometryEditService>();
                mobjEditService.TargetMapWindow = application.ActiveMapWindow;

                mobjEditPointService = GTClassFactory.Create<IGTGeometryEditService>();
                mobjEditPointService.TargetMapWindow = application.ActiveMapWindow;

                mobjExplorerService = GTClassFactory.Create<IGTFeatureExplorerService>();
                mobjExplorerService.CancelClick += new EventHandler(mobjExplorerService_CancelClick);
                mobjExplorerService.SaveAndContinueClick += new EventHandler(mobjExplorerService_SaveAndContinueClick);
                mobjExplorerService.SaveClick += new EventHandler(mobjExplorerService_SaveClick);

                RegisterEvents();

                LoadParameters();

                //LoadDuctConfig(iDuctFID);

                string mstrTransactionName = "Place SubDuct Attribute";
                m_oGTTransactionManager.Begin(mstrTransactionName);

                mobjSubDuctAttribute = application.DataContext.NewFeature(iSubDuctFNO, true);
                mobjExplorerService.ExploreFeature(mobjSubDuctAttribute, "Placement");

                mblnVisible = mobjExplorerService.Visible;

                mobjExplorerService.Visible = true;
                mobjExplorerService.Slide(true);
            }
                
            catch (Exception e)
            {
                log.WriteLog("");
                log.WriteLog(this.GetType().FullName, "ERROR: " + e.Message);
                log.WriteLog(e.StackTrace);
                MessageBox.Show(e.Message, "Error.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                if (!_SubDuctForm.IsDisposed)
                    _SubDuctForm.Close();
                else
                    ExitCmd();
            }
        }

        private void SubDuctForm_PlaceClick(object sender, EventArgs e)
        {
            _nDucts = _SubDuctForm.NumberOfSubduct;
            _SubDuctForm.Hide();
            mintState = 2;

            oDuct = application.DataContext.OpenFeature(iDuctFNO, iDuctFID);

            if (oDuct != null)
            {
                if (!oDuct.Components.GetComponent(2320).Recordset.EOF)
                {
                    iDuctGeom = oDuct.Components.GetComponent(2320).Geometry;
                    PlaceDuct(iDuctGeom.FirstPoint.X, iDuctGeom.FirstPoint.Y, _nDucts, ConnectType.from);
                }

                if (!oDuct.Components.GetComponent(2322).Recordset.EOF)
                {
                    iDuctGeom = oDuct.Components.GetComponent(2322).Geometry;
                    PlaceDuct(iDuctGeom.FirstPoint.X, iDuctGeom.FirstPoint.Y, _nDucts, ConnectType.to);
                }
            }
            _SubDuctForm.Close();
        }

        void SubDuctForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            _SubDuctForm.Dispose();
            ExitCmd();
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

                if (!(mobjExplorerService == null))
                {
                    mobjExplorerService.Clear();
                }

                if ((!(mobjExplorerService == null)
                            && !mblnVisible))
                {
                    mobjExplorerService.Visible = false;
                }
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

            CloseFeatureExplorer();
            if (m_oGTCustomCommandHelper != null) m_oGTCustomCommandHelper.Complete();
            application.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Exited.");
        }
        catch (Exception ex)
        {
            //Logger log = Logger.GetInstance();
            log.WriteLog("Error", "ExitCmd", ex.Message);
            log.WriteLog(ex.StackTrace);
            MessageBox.Show(ex.Message, "Error.", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

                //  If the current step in the command is the third step then get the selected point.
                if ((mintState == 2))
                {
                    double X = WorldPoint.X;
                    double Y = WorldPoint.Y;

                    //if (mobjEditPointService.GeometryCount > 0) mobjEditPointService.RemoveAllGeometries();
                    //IGTOrientedPointGeometry oSubDuct;
                    //for (int i = 0; i < _nDucts; i++)
                    //{
                    //    double xSubDuct = X + Math.Sin(i * 2 * Pi / _nDucts) * SDuctHSpace;
                    //    double ySubDuct = Y + Math.Cos(i * 2 * Pi / _nDucts) * SDuctVSpace;
                    //    oSubDuct = CreatePointGeom(xSubDuct, ySubDuct);
                    //    mobjEditPointService.AddGeometry(oSubDuct, StyleSDuct);
                    //}

                //    mptSelected = null;
                //    mptSelected = WorldPoint;
                //    mintState = 3;
                //    if (mobjEditPointService.GeometryCount > 0) mobjEditPointService.RemoveAllGeometries();
                //    PlaceDuct(mptSelected.X, mptSelected.Y, _nDucts, ConnectType.from);
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
                if ((mintState == 1))
                {
                    application.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Right-click to end.");
                }
                else if ((mintState == 2))
                {
                    //IGTPoint WorldPoint = MouseEventArgs.WorldPoint;

                    //double X = WorldPoint.X;
                    //double Y = WorldPoint.Y;

                    //if (mobjEditPointService.GeometryCount > 0) mobjEditPointService.RemoveAllGeometries();
                    //IGTOrientedPointGeometry oSubDuct;
                    //for (int i = 0; i < _nDucts; i++)
                    //{
                    //    double xSubDuct = X + Math.Sin(i * 2 * Pi / _nDucts) * DuctHSpace;
                    //    double ySubDuct = Y + Math.Cos(i * 2 * Pi / _nDucts) * DuctVSpace;
                    //    oSubDuct = CreatePointGeom(xSubDuct, ySubDuct);
                    //    mobjEditPointService.AddGeometry(oSubDuct, StyleDuct);
                    //}
                }
                System.Windows.Forms.Application.DoEvents();
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
            if (mintState == 1)
            {
                mintState = 2;
                m_oGTTransactionManager.Rollback();
                LoadDuctConfig(iDuctFID);
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
