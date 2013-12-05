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
//using AG.GTechnology.WallNumberPlacement.Forms;
using System.Diagnostics;
using AG.GTechnology.Utilities;

namespace AG.GTechnology.WallNumberPlacement
{
    class GTWallNumberPlacement : Intergraph.GTechnology.Interfaces.IGTCustomCommandModeless
    {
        private Intergraph.GTechnology.API.IGTTransactionManager m_oGTTransactionManager;
        private Intergraph.GTechnology.API.IGTCustomCommandHelper m_oGTCustomCommandHelper;
        private static Intergraph.GTechnology.API.IGTApplication application = GTClassFactory.Create<IGTApplication>();
      //  private ManholeForm _ManholeForm;
        private Logger log;
        private IGTDataContext oDataContext = application.DataContext;
        private static IGTPolylineGeometry oLineGeom = null;
        private static List<IGTGeometry> arrIntersection = new List<IGTGeometry>();
        private static List<IGTGeometry> arrIntersectionText = new List<IGTGeometry>();
        private string mstrFistPoint = "Please enter the attributes for Manhole.";
        private int lStyleId = 11508;

        private double Pi = 3.141592653589793;

        bool mblnVisible = false;
        int mintState;
        //IGTGeometryCreationService mobjCreationService;
        //IGTFeaturePlacementService mobjPlacementService;
        IGTGeometryEditService mobjEditService;
        IGTGeometryEditService mobjEditPointService;
        IGTGeometryEditService mobjEditServiceTemp;
        IGTLocateService mobjLocateService;
        IGTFeatureExplorerService mobjExplorerService;
        //IGTKeyObject mobjContourFeature;
        IGTPoint mptSelected;
        IGTPoint mptPoint1;
        IGTPoint mptPoint2;
        IGTKeyObject mobjManholeAttribute = null;

        private short iManholeFNO = 2700;
        private int iManholeFID = 0;
        private IGTKeyObject oManhole = null;
        private IGTGeometry iManholeGeom = null;
        private ManholeCtrl cManhole = null;
        private int numberofWall = 0;
        private string ManholeType = string.Empty;

        private int StyleManhole = 2720001;

        Dictionary<string, string> Parameters = new Dictionary<string, string>();

        double _distance = 0;

        private void LoadManholeType(string _type)
        {
            cManhole = new ManholeCtrl();
            cManhole.ManholeType = _type;
            cManhole.LoadManholeType();
            StyleManhole = cManhole.ManholeStyle;
        }

        private void LoadBoundary(IGTKeyObject feature)
        {
            if (feature.FNO == 2700)
            {
                if (!feature.Components.GetComponent(2701).Recordset.EOF)
                {
                    ManholeType = feature.Components.GetComponent(2701).Recordset.Fields["FEATURE_TYPE"].Value.ToString();
                }
                if (string.IsNullOrEmpty(ManholeType)) ManholeType = "JC9";
                ManholeType = "JC9";
                LoadManholeType(ManholeType);

                if (!feature.Components.GetComponent(2720).Recordset.EOF)
                {
                    IGTOrientedPointGeometry ManholeOrigin = (IGTOrientedPointGeometry)feature.Components.GetComponent(2720).Geometry;
                    iManholeGeom = ManholeOrigin;
                    cManhole.Origin = ManholeOrigin.Origin;
                    cManhole.RotationPnt = GTClassFactory.Create<IGTPoint>();

                    //IGTVector vector = GTClassFactory.Create<IGTVector>();
                    //vector.I = Math.Sin(3.14159265 / 3);
                    //vector.J = Math.Cos(3.14159265 / 3);

                    //ManholeOrigin.Orientation = vector;

                    cManhole.RotationPnt.X = cManhole.Origin.X + ManholeOrigin.Orientation.I;
                    cManhole.RotationPnt.Y = cManhole.Origin.Y + ManholeOrigin.Orientation.J;

                    //ManholeOrigin.Orientation.Magnitude = 0.5;

                    if (cManhole.BoundaryGeom.Points.Count > 0)
                    {
                        mobjEditService.AddGeometry(cManhole.BoundaryGeom, 12016);
                        IGTPoint objPoint1 = GTClassFactory.Create<IGTPoint>();
                        objPoint1.X = 0;
                        objPoint1.Y = 0;
                        IGTPoint objPoint2 = GTClassFactory.Create<IGTPoint>();
                        objPoint2.X = ManholeOrigin.Origin.X;
                        objPoint2.Y = ManholeOrigin.Origin.Y;
                        mobjEditService.BeginMove(objPoint1);
                        mobjEditService.Move(objPoint2);
                        mobjEditService.EndMove(objPoint2);

                        objPoint2 = GTClassFactory.Create<IGTPoint>();
                        objPoint2.X = ManholeOrigin.Origin.X + ManholeOrigin.Orientation.Magnitude;
                        objPoint2.Y = ManholeOrigin.Origin.Y;

                        mobjEditService.BeginRotate(objPoint2, cManhole.Origin);
                        mobjEditService.Rotate(cManhole.RotationPnt);
                        mobjEditService.EndRotate(cManhole.RotationPnt);
                        
                        cManhole.BoundaryGeom = (IGTPolylineGeometry)mobjEditService.GetGeometry(1);
                    }
                }

                if (!feature.Components.GetComponent(2732).Recordset.EOF)
                {
                    feature.Components.GetComponent(2732).Recordset.MoveLast();
                    numberofWall = feature.Components.GetComponent(2732).Recordset.RecordCount;
                }
                numberofWall++;
            }
        }

        private void CreateManholeGeom(IGTPoint objPoint, IGTPoint objPointRotate, IGTGeometryEditService mobjEditService)
        {
            IGTOrientedPointGeometry oOrPointGeom = PGeoLib.CreatePointGeom(objPoint.X, objPoint.Y);

            //if (mobjEditService.GeometryCount > 0) mobjEditService.RemoveAllGeometries();
            mobjEditService.AddGeometry(oOrPointGeom, StyleManhole);

            if (objPointRotate != null)
            {
                IGTPoint objPointTmp = GTClassFactory.Create<IGTPoint>();
                objPointTmp.X = objPoint.X + 100;
                objPointTmp.Y = objPoint.Y;

                mobjEditService.BeginRotate(objPointTmp, objPoint);
                mobjEditService.Rotate(objPointRotate);
                mobjEditService.EndRotate(objPointRotate);
                //oOrPointGeom = (IGTOrientedPointGeometry)mobjEditService.GetGeometry(1);
            }

            //return oOrPointGeom;
        }

        private IGTKeyObject CreateManhole(ManholeCtrl ctlManhole, IGTGeometryEditService mobjEditService)
        {
            short iFNO;
            short iCNO;
            int lFID;
            double dRotation;
            IGTKeyObject oNewFeature;

            IGTTextPointGeometry oOrTextGeom;
            IGTOrientedPointGeometry oOrPointGeom = null;

            if (ctlManhole == null) return null;
            if (mobjEditService == null) return null;
            if (mobjManholeAttribute == null) return null;

            IGTPoint oPoint = ctlManhole.Origin;
            IGTPoint oPointRotate = ctlManhole.RotationPnt;

            iFNO = mobjManholeAttribute.FNO;
            oNewFeature = mobjManholeAttribute;

            // FID generation.
            lFID = oNewFeature.FID;

            oNewFeature = application.DataContext.OpenFeature(iFNO, lFID);
            //Wall number
            iCNO = 2732;
            int fNbr = numberofWall;

            double angle = Math.Atan2(((IGTOrientedPointGeometry)iManholeGeom).Orientation.J, ((IGTOrientedPointGeometry)iManholeGeom).Orientation.I) * (180 / Math.PI);
            oOrTextGeom = PGeoLib.CreateTextGeom(cManhole.LabelOrigin.X, cManhole.LabelOrigin.Y, numberofWall.ToString(), angle, GTAlignmentConstants.gtalCenterCenter);

            //oOrTextGeom = CreateTextGeom(cManhole.LabelOrigin.X, cManhole.LabelOrigin.Y, numberofWall.ToString());

            oNewFeature.Components.GetComponent(iCNO).Recordset.AddNew("G3E_FID", lFID);
            oNewFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_FNO", iFNO);
            oNewFeature.Components.GetComponent(iCNO).Geometry = oOrTextGeom;

            return oNewFeature;
        }

        private void LoadManholeConfig()
        {
            if (!mobjManholeAttribute.Components.GetComponent(2701).Recordset.EOF)
            {
                ManholeType = mobjManholeAttribute.Components.GetComponent(2701).Recordset.Fields["FEATURE_TYPE"].Value.ToString();
            }
            if (string.IsNullOrEmpty(ManholeType)) ManholeType = "JC9";
            LoadManholeType(ManholeType);

            IGTMapWindow mobjMapWindow = application.ActiveMapWindow;

          //  _ManholeForm = new ManholeForm(m_oGTCustomCommandHelper);
          //  _ManholeForm.FormClosed += new FormClosedEventHandler(ManholeForm_FormClosed);
          //  _ManholeForm.PlaceClick += new EventHandler(ManholeForm_PlaceClick);
         //  _ManholeForm.Show();
        }

        public void Activate(Intergraph.GTechnology.API.IGTCustomCommandHelper CustomCommandHelper)
        {
            try
            {
                application.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, mstrFistPoint);

                log = Logger.getInstance();
                log.WriteLog(this.GetType().FullName, "Wall number Placement started!");
                log.WriteLog("");

                m_oGTCustomCommandHelper = CustomCommandHelper;

                if (application.SelectedObjects.FeatureCount == 0)
                {
                    MessageBox.Show("Please identify manhole/chamber/tunnel", "Wall number placement", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    ExitCmd();
                    return;
                }

                if (application.SelectedObjects.FeatureCount > 1)
                {
                    MessageBox.Show("Please select single feature at once", "Wall number placement", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    ExitCmd();
                    return;
                }

                foreach (IGTDDCKeyObject oDDCKeyObject in application.SelectedObjects.GetObjects())
                {
                    if ((oDDCKeyObject.ComponentViewName == "VGC_MANHL_S") ||
                        (oDDCKeyObject.ComponentViewName == "VGC_CHAMBER_P") ||
                        (oDDCKeyObject.ComponentViewName == "VGC_TUNNEL_P"))
                    {
                        iManholeGeom = oDDCKeyObject.Geometry;
                        iManholeFNO = oDDCKeyObject.FNO;
                        iManholeFID = oDDCKeyObject.FID;
                    }
                }

                //  Assigns the private member variables their default values.
                mintState = 1;

                mobjEditService = GTClassFactory.Create<IGTGeometryEditService>();
                mobjEditService.TargetMapWindow = application.ActiveMapWindow;

                mobjEditPointService = GTClassFactory.Create<IGTGeometryEditService>();
                mobjEditPointService.TargetMapWindow = application.ActiveMapWindow;

                mobjEditServiceTemp = GTClassFactory.Create<IGTGeometryEditService>();
                mobjEditServiceTemp.TargetMapWindow = application.ActiveMapWindow;

                mobjLocateService = application.ActiveMapWindow.LocateService;

                RegisterEvents();

                mobjManholeAttribute = application.DataContext.OpenFeature(iManholeFNO, iManholeFID);
                LoadBoundary(mobjManholeAttribute);
            }
                
            catch (Exception e)
            {
                log.WriteLog("");
                log.WriteLog(this.GetType().FullName, "ERROR: " + e.Message);
                log.WriteLog(e.StackTrace);
                MessageBox.Show(e.Message, "Error.", MessageBoxButtons.OK, MessageBoxIcon.Error);
              //  if (!_ManholeForm.IsDisposed)
              //      _ManholeForm.Close();
             //   else
                    ExitCmd();
            }
        }

        //private void ManholeForm_PlaceClick(object sender, EventArgs e)
        //{
        //    bool PlaceType = _ManholeForm.PlaceType;
        //    _ManholeForm.Hide();

        //    if (PlaceType)
        //    {
        //        _distance = _ManholeForm.Distance;
        //        mintState = 2;
        //    }
        //    else
        //    {
        //        mintState = 4;
        //    }

        //    //_ManholeForm.Close();
        //}

        //void ManholeForm_FormClosed(object sender, FormClosedEventArgs e)
        //{
        //    _ManholeForm.Dispose();
        //    ExitCmd();
        //}

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
            if (mobjEditServiceTemp != null)
            {
                mobjEditServiceTemp.RemoveAllGeometries();
                mobjEditServiceTemp = null;
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
                    if (mintState == 4)
                    {
                        mintState = -1;
                    }
                    else
                    {
                        ExitCmd();
                        return;
                    }
                }
                else
                {
                    //  If the current step in the command is the third step then get the selected point.
                    if ((mintState == 1))
                    {
                        double X = WorldPoint.X;
                        double Y = WorldPoint.Y;

                        //cManhole.LabelOrigin = GTClassFactory.Create<IGTPoint>();
                        //cManhole.LabelOrigin.X = X;
                        //cManhole.LabelOrigin.Y = Y;
                        cManhole.LabelOrigin = PGeoLib.GetProjectedPoint(cManhole.BoundaryGeom, WorldPoint);
                        mintState = -1;
                        m_oGTTransactionManager.Begin("Add the wall number");
                        CreateManhole(cManhole, mobjEditService);
                        m_oGTTransactionManager.Commit();
                        m_oGTTransactionManager.RefreshDatabaseChanges();

                        if (mobjEditPointService.GeometryCount > 0) mobjEditPointService.RemoveAllGeometries();
                        if (mobjEditServiceTemp.GeometryCount > 0) mobjEditServiceTemp.RemoveAllGeometries();
                        if (mobjEditService.GeometryCount > 0) mobjEditService.RemoveAllGeometries();

                        ExitCmd();
                        mintState = 8;
                    }
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
                    if (numberofWall <= 0) return;
                    application.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Enter location of wall number");

                    if (mobjEditServiceTemp.GeometryCount > 0) mobjEditServiceTemp.RemoveAllGeometries();

                    IGTPoint prjPoint = PGeoLib.GetProjectedPoint(cManhole.BoundaryGeom, WorldPoint);
                    double angle = Math.Atan2(((IGTOrientedPointGeometry)iManholeGeom).Orientation.J, ((IGTOrientedPointGeometry)iManholeGeom).Orientation.I) * (180 / Math.PI);
                    IGTTextPointGeometry txtNumber = PGeoLib.CreateTextGeom(prjPoint.X, prjPoint.Y, numberofWall.ToString(), angle, GTAlignmentConstants.gtalCenterCenter);
                    mobjEditServiceTemp.AddGeometry(txtNumber, 30700);
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
                LoadManholeConfig();
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
