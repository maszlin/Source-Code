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
using AG.GTechnology.ManholePlacement.Forms;
using System.Diagnostics;
using AG.GTechnology.Utilities;

namespace AG.GTechnology.ManholePlacement
{
    class GTManholePlacement : Intergraph.GTechnology.Interfaces.IGTCustomCommandModeless
    {
        private Intergraph.GTechnology.API.IGTTransactionManager m_oGTTransactionManager;
        private Intergraph.GTechnology.API.IGTCustomCommandHelper m_oGTCustomCommandHelper;
        private static Intergraph.GTechnology.API.IGTApplication application = GTClassFactory.Create<IGTApplication>();
        private ManholeForm _ManholeForm;
        private Logger log;
        private IGTDataContext oDataContext = application.DataContext;
        private static IGTPolylineGeometry oLineGeom = null;
        private static List<IGTGeometry> arrIntersection = new List<IGTGeometry>();
        private static List<IGTGeometry> arrIntersectionText = new List<IGTGeometry>();
        private string mstrFistPoint = "Please enter the attributes for Manhole.";
        private int lStyleId = 11508;

        bool mblnVisible = false;
        int mintState;
        //IGTGeometryCreationService mobjCreationService;
        //IGTFeaturePlacementService mobjPlacementService;
        IGTGeometryEditService mobjEditService;
        IGTGeometryEditService mobjEditPointService;
        IGTGeometryEditService mobjEditServiceTemp;

        IGTGeometryEditService mobjEditServiceNewTemp;
        IGTGeometryEditService mobjEditServiceNew;
        IGTGeometryEditService mobjEditPointServiceNew;

        IGTLocateService mobjLocateService;
        IGTFeatureExplorerService mobjExplorerService;
        //IGTKeyObject mobjContourFeature;
        IGTPoint mptSelected;
        IGTPoint mptPoint1;
        IGTPoint mptPoint2;
        IGTKeyObject mobjManholeAttribute = null;

        IGTMapWindow FromWindow = null;
        IGTMapWindow ToWindow = null;
        string WindowCaption = "MapWindow 1";

        private short iManholeFNO = 2700;
        private IGTKeyObject oManhole = null;
        private IGTGeometry iManholeGeom = null;
        private ManholeCtrl cManhole = null;
        private int numberofWall = 0;
        private string ManholeType = string.Empty;

        private int StyleManhole = 2730001;
        private int StyleTempManhole = 2730001;

        bool ContinueNext = false;
        Dictionary<string, string> Parameters = new Dictionary<string, string>();

        double _distance = 0;

        //private void LoadManholeType(string _type)
        //{
        //    cManhole = new ManholeCtrl();
        //    cManhole.ManholeType = _type;
        //    cManhole.LoadManholeType();
        //    StyleManhole = cManhole.ManholeStyle;
        //    StyleTempManhole = cManhole.ManholeTempStyle;
        //}


        private void LocateFeature(short iFNO, int lFID, IGTMapWindow window)
        {
            if (window == null) return;

            IGTDDCKeyObjects feat = application.DataContext.GetDDCKeyObjects(iFNO, lFID, GTComponentGeometryConstants.gtddcgAllGeographic);
            //application.SelectedObjects.Clear();            
            for (int K = 0; K < feat.Count; K++)
            {
                if (feat[K].ComponentViewName == "VGC_MANHL_S")
                {
                    application.SelectedObjects.Add(GTSelectModeConstants.gtsosmSelectedComponentsOnly, feat[K]);
                    IGTWorldRange range = GTClassFactory.Create<IGTWorldRange>();
                    IGTPoint point1 = GTClassFactory.Create<IGTPoint>();
                    IGTPoint point2 = GTClassFactory.Create<IGTPoint>();
                    point1.X = feat[K].Geometry.FirstPoint.X - 5;
                    point1.Y = feat[K].Geometry.FirstPoint.Y - 5;
                    range.BottomLeft = point1;
                    point2.X = feat[K].Geometry.FirstPoint.X + 5;
                    point2.Y = feat[K].Geometry.FirstPoint.Y + 5;
                    range.TopRight = point2;
                    window.ZoomArea(range);
                }
            }
            //window.CenterSelectedObjects();
            //window.DisplayScale = 100;
            application.RefreshWindows();
        }

        private void LoadManholeType(IGTKeyObject _attr)
        {
            string ManholeType = "JC9";
            string MH_MIX = "";
            if (!_attr.Components.GetComponent(2701).Recordset.EOF)
            {
                ManholeType = _attr.Components.GetComponent(2701).Recordset.Fields["FEATURE_TYPE"].Value.ToString();            
                MH_MIX = _attr.Components.GetComponent(2701).Recordset.Fields["MH_MIX"].Value.ToString();
            }
            cManhole = new ManholeCtrl();
            cManhole.ManholeType = ManholeType;
            cManhole.LoadManholeType();
            StyleManhole = cManhole.ManholeStyle;
            StyleTempManhole = cManhole.ManholeTempStyle;

            IGTHelperService helper = GTClassFactory.Create<IGTHelperService>();
            helper.DataContext = application.DataContext;

            StyleManhole = helper.GetComponentStyleID(_attr, 2720);
            StyleTempManhole = StyleManhole;
            
            cManhole.ManholeStyle = StyleManhole;
            cManhole.ManholeTempStyle = StyleTempManhole;

            string manholeLabel = "[ManholeLabel]";
            GTAlignmentConstants ManholeLabelAlignment = GTAlignmentConstants.gtalBottomLeft;

            cManhole.ManholeTextStyle = helper.GetComponentStyleID(_attr, 2730);
            helper.GetComponentLabel(_attr, 2730, ref manholeLabel, ref ManholeLabelAlignment);

            string decription="";
            if (MH_MIX.ToUpper() == "PRE-FAB" )
                decription=ManholeType + "/(P)";
            else if (MH_MIX.ToUpper() == "PRE-FAB (SHUTTERING)")
                decription= ManholeType + "/(P/S)";
            else if (MH_MIX.ToUpper() =="SITE")
                decription= ManholeType;
            else  decription= ManholeType +MH_MIX;   

    // REPLACE  "NEW LINE" MORE THAN 1 WITH 1 "NEW LINE"
         //  decription=decription.Replace("\s{2,}","\n");
    
            cManhole.ManholeLabel = "???\n" + decription;
            cManhole.ManholeLabelDesription = decription;
            cManhole.ManholeLabelAlignment = ManholeLabelAlignment;
            cManhole.ManholeWallStyle = helper.GetComponentStyleID(_attr, 2732);

        }

        private void CreateManholeGeom(IGTPoint objPoint, IGTPoint objPointRotate, IGTGeometryEditService mobjEditServiceIn)
        {
            CreateManholeGeom(objPoint, objPointRotate, mobjEditServiceIn, StyleManhole);

            //return oOrPointGeom;
        }

        private void CreateManholeGeom(IGTPoint objPoint, IGTPoint objPointRotate, IGTGeometryEditService mobjEditServiceIn, int StyleId)
        {
            IGTOrientedPointGeometry oOrPointGeom = PGeoLib.CreatePointGeom(objPoint.X, objPoint.Y);

            //if (mobjEditService.GeometryCount > 0) mobjEditService.RemoveAllGeometries();
            if (StyleId == 0) StyleId = StyleManhole;
            mobjEditServiceIn.AddGeometry(oOrPointGeom, StyleId);

            if (objPointRotate != null)
            {
                IGTPoint objPointTmp = GTClassFactory.Create<IGTPoint>();
                objPointTmp.X = objPoint.X + 100;
                objPointTmp.Y = objPoint.Y;

                mobjEditServiceIn.BeginRotate(objPointTmp, objPoint);
                mobjEditServiceIn.Rotate(objPointRotate);
                mobjEditServiceIn.EndRotate(objPointRotate);
                //oOrPointGeom = (IGTOrientedPointGeometry)mobjEditService.GetGeometry(1);
            }

            //return oOrPointGeom;
        }

        private IGTKeyObject  CreateManhole(ManholeCtrl ctlManhole, IGTGeometryEditService mobjEditService)
        {
            short iFNO;
            short iCNO;
            long lFID;
            double dRotation;
            IGTKeyObject oNewFeature;

            IGTTextPointGeometry oOrTextGeom;
            IGTOrientedPointGeometry oOrPointGeom = null;

            if (ctlManhole == null) return null;
            if (mobjEditService == null) return null;

            IGTPoint oPoint = ctlManhole.Origin;
            IGTPoint oPointRotate = ctlManhole.RotationPnt;

            iFNO = iManholeFNO;
            oNewFeature = application.DataContext.NewFeature(iFNO);

            // FID generation.
            lFID = oNewFeature.FID;

            if (mobjManholeAttribute != null)
                PGeoLib.CopyComponentAttribute(mobjManholeAttribute, oNewFeature);
            else
            {
                // Every feature is imported in the Existing state.
                iCNO = 51; //GC_NETELEM
                if (oNewFeature.Components.GetComponent(iCNO).Recordset.EOF)
                {
                    oNewFeature.Components.GetComponent(iCNO).Recordset.AddNew("G3E_FID", lFID);
                    oNewFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_FNO", iFNO);
                    oNewFeature.Components.GetComponent(iCNO).Recordset.Update("FEATURE_STATE", "PPF");
                }
                else
                {

                    oNewFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_FNO", iFNO);
                    oNewFeature.Components.GetComponent(iCNO).Recordset.Update("FEATURE_STATE", "PPF");
                }

                // Attribute
                iCNO = 2701;
                //oNewFeature.Components.GetComponent(iCNO).Recordset.Update("FEATURE_TYPE", "JC9");
            }

            //Symbol
            iCNO = 2720;
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
            oOrPointGeom = PGeoLib.CreatePointGeom(oPoint.X, oPoint.Y);

            if (mobjEditService.GeometryCount > 0) mobjEditService.RemoveAllGeometries();
            mobjEditService.AddGeometry(oOrPointGeom, StyleManhole);

            if (oPointRotate != null)
            {
                IGTPoint objPointTmp = GTClassFactory.Create<IGTPoint>();
                objPointTmp.X = oPoint.X + 100;
                objPointTmp.Y = oPoint.Y;

                mobjEditService.BeginRotate(objPointTmp, oPoint);
                mobjEditService.Rotate(oPointRotate);
                mobjEditService.EndRotate(oPointRotate);
                oOrPointGeom = (IGTOrientedPointGeometry)mobjEditService.GetGeometry(1);
            }
            oNewFeature.Components.GetComponent(iCNO).Geometry = oOrPointGeom;

            //Wall number
            iCNO = 2732;
            int fNbr = cManhole.FirstNumber;
            if (cManhole.ManholeWall != null)
            {
                for (int i = 0; i < cManhole.ManholeWall.Count; i++)
                {
                    if (fNbr >= cManhole.ManholeWall.Count) fNbr = 0;
                    ManholePoint _pnt = cManhole.ManholeWall[fNbr];
                    IGTTextPointGeometry txtNumber = PGeoLib.CreateTextGeom(cManhole.Origin.X + _pnt.X, cManhole.Origin.Y + _pnt.Y, Convert.ToString(i + 1));

                    IGTPoint objPointTmp = GTClassFactory.Create<IGTPoint>();
                    objPointTmp.X = oPoint.X + 100;
                    objPointTmp.Y = oPoint.Y;

                    if (mobjEditService.GeometryCount > 0) mobjEditService.RemoveAllGeometries();
                    mobjEditService.AddGeometry(txtNumber, cManhole.ManholeWallStyle);
                    if (oPointRotate != null)
                    {

                        mobjEditService.BeginRotate(objPointTmp, oPoint);
                        mobjEditService.Rotate(oPointRotate);
                        mobjEditService.EndRotate(oPointRotate);
                        txtNumber = (IGTTextPointGeometry)mobjEditService.GetGeometry(1);
                    }

                    oNewFeature.Components.GetComponent(iCNO).Recordset.AddNew("G3E_FID", lFID);
                    oNewFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_FNO", iFNO);
                    oNewFeature.Components.GetComponent(iCNO).Recordset.Update("WALL_NUM", Convert.ToString(i + 1));
                    oNewFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_ALIGNMENT", 0);
                    oNewFeature.Components.GetComponent(iCNO).Geometry = txtNumber;

                    fNbr++;
                }
            }

            //Label
            iCNO = 2730;
            IGTTextPointGeometry txtLabel = PGeoLib.CreateTextGeom(cManhole.LabelOrigin.X, cManhole.LabelOrigin.Y, "[Manhole label]", 0, GTAlignmentConstants.gtalTopLeft);

            if (oNewFeature.Components.GetComponent(iCNO).Recordset.EOF)
            {
                oNewFeature.Components.GetComponent(iCNO).Recordset.AddNew("G3E_FID", lFID);
                oNewFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_FNO", iFNO);
            }
            else
            {
                oNewFeature.Components.GetComponent(iCNO).Recordset.MoveLast();
            }

            oNewFeature.Components.GetComponent(iCNO).Geometry = txtLabel;

            //Label Description
            iCNO = 2734;
             txtLabel = PGeoLib.CreateTextGeom(cManhole.LabelOrigin.X, cManhole.LabelOrigin.Y-2.5, "[Manhole label description]", 0, GTAlignmentConstants.gtalTopLeft);

            if (oNewFeature.Components.GetComponent(iCNO).Recordset.EOF)
            {
                oNewFeature.Components.GetComponent(iCNO).Recordset.AddNew("G3E_FID", lFID);
                oNewFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_FNO", iFNO);
            }
            else
            {
                oNewFeature.Components.GetComponent(iCNO).Recordset.MoveLast();
            }

            oNewFeature.Components.GetComponent(iCNO).Geometry = txtLabel;

            return oNewFeature;
        }

        private void LoadManholeConfig()
        {
            //if (!mobjManholeAttribute.Components.GetComponent(2701).Recordset.EOF)
            //{
            //    ManholeType = mobjManholeAttribute.Components.GetComponent(2701).Recordset.Fields["FEATURE_TYPE"].Value.ToString();
            //}
            //if (string.IsNullOrEmpty(ManholeType)) ManholeType = "JC9";
            //LoadManholeType(ManholeType);
            LoadManholeType(mobjManholeAttribute);

            IGTMapWindow mobjMapWindow = application.ActiveMapWindow;

            _ManholeForm = new ManholeForm(m_oGTCustomCommandHelper);
            _ManholeForm.FormClosing += new FormClosingEventHandler(ManholeForm_FormClosing);
            _ManholeForm.FormClosed += new FormClosedEventHandler(ManholeForm_FormClosed);
            _ManholeForm.PlaceClick += new EventHandler(ManholeForm_PlaceClick);
            _ManholeForm.Show();
        }

        public void Activate(Intergraph.GTechnology.API.IGTCustomCommandHelper CustomCommandHelper)
        {
            try
            {
                application.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, mstrFistPoint);

                log = Logger.getInstance();
                log.WriteLog(this.GetType().FullName, "Manhole Placement started!");
                log.WriteLog("");

                m_oGTCustomCommandHelper = CustomCommandHelper;

                //  Assigns the private member variables their default values.
                mintState = 1;

                _distance = 0;
                FromWindow = application.ActiveMapWindow;

                mobjEditService = GTClassFactory.Create<IGTGeometryEditService>();
                mobjEditService.TargetMapWindow = application.ActiveMapWindow;

                mobjEditPointService = GTClassFactory.Create<IGTGeometryEditService>();
                mobjEditPointService.TargetMapWindow = application.ActiveMapWindow;

                mobjEditServiceTemp = GTClassFactory.Create<IGTGeometryEditService>();
                mobjEditServiceTemp.TargetMapWindow = application.ActiveMapWindow;


                mobjEditServiceNew = GTClassFactory.Create<IGTGeometryEditService>();
                mobjEditServiceNew.TargetMapWindow = application.ActiveMapWindow;

                mobjEditPointServiceNew = GTClassFactory.Create<IGTGeometryEditService>();
                mobjEditPointServiceNew.TargetMapWindow = application.ActiveMapWindow;

                mobjEditServiceNewTemp = GTClassFactory.Create<IGTGeometryEditService>();
                mobjEditServiceNewTemp.TargetMapWindow = application.ActiveMapWindow;

                mobjLocateService = application.ActiveMapWindow.LocateService;

                mobjExplorerService = GTClassFactory.Create<IGTFeatureExplorerService>();

                RegisterEvents();

                string mstrTransactionName = "Place Manhole Attribute";
                m_oGTTransactionManager.Begin(mstrTransactionName);

                mobjManholeAttribute = application.DataContext.NewFeature(iManholeFNO, true);
                mobjExplorerService.ExploreFeature(mobjManholeAttribute, "Manhole");

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
                if (!_ManholeForm.IsDisposed)
                    _ManholeForm.Close();
                else
                    if (m_oGTCustomCommandHelper != null) m_oGTCustomCommandHelper.Complete();
            }
        }

        private void ManholeForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (mintState != 2 && mintState != 4)
            {
                DialogResult msg = MessageBox.Show("Do you want to discard your current changes and exit?", "Manhole Placement", MessageBoxButtons.YesNo);
                if (msg == DialogResult.Yes)
                    e.Cancel = false;
                else
                    e.Cancel = true;
            }
        }

        private void ManholeForm_PlaceClick(object sender, EventArgs e)
        {
            bool PlaceType = _ManholeForm.PlaceType;
            _ManholeForm.Hide();
            System.Windows.Forms.Application.DoEvents();

            if (PlaceType)
            {
                _distance = _ManholeForm.Distance;
                mintState = 2;
            }
            else
            {
                mintState = 4;
            }

            _ManholeForm.Close();
        }

        void ManholeForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            //_ManholeForm.Dispose();
            //ExitCmd();
        }

        public bool CanTerminate
        {
            get
            {
                DialogResult msg = MessageBox.Show("Do you want to discard your current changes and exit?", "Manhole Placement", MessageBoxButtons.YesNo);
                if (msg == DialogResult.Yes)
                {
                    ExitCmd();
                    //  return true;
                }
                else
                {
                    return false;
                }

                return false;
           
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

               // if (m_oIGTTransactionManager != null)
               // {
               //     m_oIGTTransactionManager = null;
               // }

            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public Intergraph.GTechnology.API.IGTTransactionManager TransactionManager
        {
            set
            {
                m_oGTTransactionManager = value;
            }
        }
        public void ExternalCAllExit()
        {
            ExitCmd();
        }

        private void ExitCmd()
        {
            try
            {
                UnRegisterEvents();
                mintState = 1;
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
                if (mobjEditServiceNew != null)
                {
                    mobjEditServiceNew.RemoveAllGeometries();
                    mobjEditServiceNew = null;
                }
                if (mobjEditPointServiceNew != null)
                {
                    mobjEditPointServiceNew.RemoveAllGeometries();
                    mobjEditPointServiceNew = null;
                }
                if (mobjEditServiceNewTemp != null)
                {
                    mobjEditServiceNewTemp.RemoveAllGeometries();
                    mobjEditServiceNewTemp = null;
                }

                mobjLocateService = null;

                if (m_oGTTransactionManager != null)
                {
                    if (m_oGTTransactionManager.TransactionInProgress)
                    {
                        m_oGTTransactionManager.Rollback();
                    }
                }

                //if (cManhole != null)
                //{
                //    cManhole.Dispose();
                //    cManhole = null;
                //}

                CloseFeatureExplorer();
                if (m_oGTCustomCommandHelper != null) m_oGTCustomCommandHelper.Complete();
                application.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Exited.");
            }
            catch (Exception ex)
            {
                //Logger log = Logger.GetInstance();
                log.WriteLog("Error", "ExitCmd", ex.Message);
                log.WriteLog(ex.StackTrace);
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
                    if ((mintState == 4)||(mintState == 41))
                    {
                        mintState = -1;
                        double xManhole = WorldPoint.X;
                        double yManhole = WorldPoint.Y;

                        if (mptPoint1 == null)
                        {
                            mptPoint1 = WorldPoint;

                            //if (mobjEditService.GeometryCount > 0) mobjEditService.RemoveAllGeometries();
                            //IGTOrientedPointGeometry oManhole;
                            //oManhole = CreatePointGeom(xManhole, yManhole);
                            //mobjEditService.AddGeometry(oManhole, StyleManhole);
                        }

                        if (mobjEditService.GeometryCount > 0) mobjEditService.RemoveAllGeometries();
                        CreateManholeGeom(mptPoint1, null, mobjEditService);

                        if (mobjEditPointService.GeometryCount > 0) mobjEditPointService.RemoveAllGeometries();
                        CreateManholeGeom(mptPoint1, null, mobjEditPointService, StyleTempManhole);
                        IGTPoint objPoint = GTClassFactory.Create<IGTPoint>();
                        objPoint.X = mptPoint1.X + 100;
                        objPoint.Y = mptPoint1.Y;
                        mobjEditPointService.BeginRotate(objPoint, mptPoint1);
                        cManhole.RotationPnt = null;

                        if ((_distance >= 200) && (PGeoLib.CheckMapWindow(WindowCaption)))
                        {
                            if (mobjEditServiceNew.GeometryCount > 0) mobjEditServiceNew.RemoveAllGeometries();
                            mobjEditServiceNew.TargetMapWindow = ToWindow;
                            CreateManholeGeom(mptPoint1, null, mobjEditServiceNew);

                            if (mobjEditPointServiceNew.GeometryCount > 0) mobjEditPointServiceNew.RemoveAllGeometries();
                            mobjEditPointServiceNew.TargetMapWindow = ToWindow;
                            CreateManholeGeom(mptPoint1, null, mobjEditPointServiceNew, StyleTempManhole);
                            mobjEditPointServiceNew.BeginRotate(objPoint, mptPoint1);
                        }

                        application.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Point to define angle of facility or right-click to complete");
                        mintState = 5;
                    }
                    else if (mintState == 5)
                    {
                        mintState = -1;
                        cManhole.X = mptPoint1.X;
                        cManhole.Y = mptPoint1.Y;
                        cManhole.Origin = mptPoint1;

                        //if (mptPoint2 != null)
                        //{
                            //mobjEditService.Rotate(mptPoint2);
                            //mobjEditService.EndRotate(mptPoint2);

                            if (mobjEditPointService.GeometryCount > 0) mobjEditPointService.RemoveAllGeometries();
                            if (mobjEditServiceTemp.GeometryCount > 0) mobjEditServiceTemp.RemoveAllGeometries();
                            if (mobjEditService.GeometryCount > 0) mobjEditService.RemoveAllGeometries();

                            if (mobjEditPointServiceNew.GeometryCount > 0) mobjEditPointServiceNew.RemoveAllGeometries();
                            if (mobjEditServiceNewTemp.GeometryCount > 0) mobjEditServiceNewTemp.RemoveAllGeometries();
                            if (mobjEditServiceNew.GeometryCount > 0) mobjEditServiceNew.RemoveAllGeometries();

                            if (cManhole.ManholeWall != null)
                            {
                                foreach (ManholePoint _pnt in cManhole.ManholeWall)
                                {
                                    IGTTextPointGeometry txtNumber = PGeoLib.CreateTextGeom(mptPoint1.X + _pnt.X, mptPoint1.Y + _pnt.Y, "+");
                                    mobjEditService.AddGeometry(txtNumber, cManhole.ManholeWallStyle);

                                    if ((_distance >= 200) && (PGeoLib.CheckMapWindow(WindowCaption)))
                                    {
                                        txtNumber = PGeoLib.CreateTextGeom(mptPoint1.X + _pnt.X, mptPoint1.Y + _pnt.Y, "+");
                                        mobjEditServiceNew.AddGeometry(txtNumber, cManhole.ManholeWallStyle);
                                    }
                                }

                            }
                        //}

                        CreateManholeGeom(mptPoint1, mptPoint2, mobjEditService);

                        if ((_distance >= 200) && (PGeoLib.CheckMapWindow(WindowCaption)))
                        {
                            CreateManholeGeom(mptPoint1, mptPoint2, mobjEditServiceNew);
                        }
                        mintState = 6;
                        if ((cManhole.ManholeWall == null) || (cManhole.ManholeWall.Count <= 0)) mintState = 7;
                    }
                    else if (mintState == 6)
                    {
                        mintState = -1;
                        mintState = 7;
                    }
                    else
                    {
                        if (m_oGTCustomCommandHelper != null) m_oGTCustomCommandHelper.Complete();
                        return;
                    }
                }
                else
                {
                    //  If the current step in the command is the third step then get the selected point.
                    if ((mintState == 2))
                    {
                        double X = WorldPoint.X;
                        double Y = WorldPoint.Y;

                        mptSelected = null;
                        mobjLocateService = application.ActiveMapWindow.LocateService;
                        IGTDDCKeyObjects objs = mobjLocateService.Locate(WorldPoint, 50, 10, GTSelectionTypeConstants.gtmwstSelectAll);
                        int fromFID = 0;
                        short fromFNO = 0;
                        foreach (IGTDDCKeyObject oDDCKeyObject in objs)
                        {
                            if ((oDDCKeyObject.ComponentViewName == "VGC_MANHL_S") || 
                                (oDDCKeyObject.ComponentViewName == "VGC_NODE_S") ||
                                (oDDCKeyObject.ComponentViewName == "VGC_POLE_S"))
                            {
                                mptSelected = oDDCKeyObject.Geometry.FirstPoint;
                                fromFID = oDDCKeyObject.FID;
                                fromFNO = oDDCKeyObject.FNO;
                            }
                        }

                        if (mptSelected != null)
                        {
                            mintState = 3;

                            if (_distance >= 200)
                            {
                                FromWindow = application.ActiveMapWindow;
                                ToWindow = application.NewMapWindow(application.ActiveMapWindow.LegendName);
                                application.ArrangeWindows(GTWindowActionConstants.gtapwaTileHorizontal);

                                WindowCaption = "Manhole Placement from " + fromFID.ToString();
                                PGeoLib.CloseMapWindow(WindowCaption);
                                ToWindow.Caption = WindowCaption;

                                LocateFeature(fromFNO, fromFID, FromWindow);
                                LocateFeature(fromFNO, fromFID, ToWindow);

                                PGeoLib.CopyWindowSetting(FromWindow, ToWindow);

                                ToWindow.DisplayScale = ToWindow.DisplayScale * 3;
                            }
                        }
                    }
                    else if ((mintState == 3))
                    {
                        mintState = -1;

                        application.ActiveMapWindow.HighlightedObjects.Clear();

                        double X = WorldPoint.X;
                        double Y = WorldPoint.Y;

                        double _angle = PGeoLib.GetAngle(mptSelected.X, mptSelected.Y, X, Y);
                        double _len = PGeoLib.GetLength(mptSelected.X, mptSelected.Y, X, Y);

                        double xManhole = WorldPoint.X;
                        double yManhole = WorldPoint.Y;

                        xManhole = mptSelected.X + Math.Cos(_angle) * (_distance);
                        yManhole = mptSelected.Y + Math.Sin(_angle) * (_distance);

                        mptPoint1 = GTClassFactory.Create<IGTPoint>();
                        mptPoint1.X = xManhole;
                        mptPoint1.Y = yManhole;

                        if (mobjEditPointService.GeometryCount > 0) mobjEditPointService.RemoveAllGeometries();
                        if (mobjEditServiceTemp.GeometryCount > 0) mobjEditServiceTemp.RemoveAllGeometries();

                        //if (mobjEditService.GeometryCount > 0) mobjEditService.RemoveAllGeometries();
                        //IGTOrientedPointGeometry oManhole;
                        //oManhole = CreatePointGeom(xManhole, yManhole);
                        //mobjEditService.AddGeometry(oManhole, StyleManhole);
                        if (mobjEditService.GeometryCount > 0) mobjEditService.RemoveAllGeometries();
                        CreateManholeGeom(mptPoint1, null, mobjEditService);

                        if ((_distance >= 200) && (PGeoLib.CheckMapWindow(WindowCaption)))
                        {
                            if (mobjEditServiceNew.GeometryCount > 0) mobjEditServiceNew.RemoveAllGeometries();
                            mobjEditServiceNew.TargetMapWindow = ToWindow;
                            CreateManholeGeom(mptPoint1, null, mobjEditServiceNew);

                            PGeoLib.LocatePoint(mptPoint1.X, mptPoint1.Y, 5, ToWindow);
                        }

                        application.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Point to enter new location or right-click to accept current location");
                        mintState = 41;
                    }
                    else if (mintState == 4)
                    {
                        mintState = -1;

                        application.ActiveMapWindow.HighlightedObjects.Clear();

                        double X = WorldPoint.X;
                        double Y = WorldPoint.Y;

                        double xManhole = WorldPoint.X;
                        double yManhole = WorldPoint.Y;

                        mptPoint1 = GTClassFactory.Create<IGTPoint>();
                        mptPoint1.X = xManhole;
                        mptPoint1.Y = yManhole;

                        if (mobjEditPointService.GeometryCount > 0) mobjEditPointService.RemoveAllGeometries();
                        if (mobjEditServiceTemp.GeometryCount > 0) mobjEditServiceTemp.RemoveAllGeometries();

                        //if (mobjEditService.GeometryCount > 0) mobjEditService.RemoveAllGeometries();
                        //IGTOrientedPointGeometry oManhole;
                        //oManhole = CreatePointGeom(xManhole, yManhole);
                        //mobjEditService.AddGeometry(oManhole, StyleManhole);
                        if (mobjEditService.GeometryCount > 0) mobjEditService.RemoveAllGeometries();
                        CreateManholeGeom(mptPoint1, null, mobjEditService);
                        mintState = 4;
                    }
                    else if (mintState == 41)
                    {
                        mintState = -1;

                        application.ActiveMapWindow.HighlightedObjects.Clear();

                        double X = WorldPoint.X;
                        double Y = WorldPoint.Y;

                        double _angle = PGeoLib.GetAngle(mptSelected.X, mptSelected.Y, X, Y);
                        double _len = PGeoLib.GetLength(mptSelected.X, mptSelected.Y, X, Y);

                        double xManhole = WorldPoint.X;
                        double yManhole = WorldPoint.Y;

                        if (_len > _distance + 3)
                        {
                            xManhole = mptSelected.X + Math.Cos(_angle) * (_distance + 3);
                            yManhole = mptSelected.Y + Math.Sin(_angle) * (_distance + 3);
                        }

                        if ((_len < _distance - 3) && (_distance > 3))
                        {
                            xManhole = mptSelected.X + Math.Cos(_angle) * (_distance - 3);
                            yManhole = mptSelected.Y + Math.Sin(_angle) * (_distance - 3);
                        }

                        mptPoint1 = GTClassFactory.Create<IGTPoint>();
                        mptPoint1.X = xManhole;
                        mptPoint1.Y = yManhole;

                        if (mobjEditPointService.GeometryCount > 0) mobjEditPointService.RemoveAllGeometries();
                        if (mobjEditServiceTemp.GeometryCount > 0) mobjEditServiceTemp.RemoveAllGeometries();

                        //if (mobjEditService.GeometryCount > 0) mobjEditService.RemoveAllGeometries();
                        //IGTOrientedPointGeometry oManhole;
                        //oManhole = CreatePointGeom(xManhole, yManhole);
                        //mobjEditService.AddGeometry(oManhole, StyleManhole);
                        if (mobjEditService.GeometryCount > 0) mobjEditService.RemoveAllGeometries();
                        CreateManholeGeom(mptPoint1, null, mobjEditService);

                        if ((_distance >= 200) && (PGeoLib.CheckMapWindow(WindowCaption)))
                        {
                            if (mobjEditServiceNew.GeometryCount > 0) mobjEditServiceNew.RemoveAllGeometries();
                            mobjEditServiceNew.TargetMapWindow = ToWindow;
                            CreateManholeGeom(mptPoint1, null, mobjEditServiceNew);
                        }

                        mintState = 41;
                    }
                    else if (mintState == 5)
                    {
                        mintState = -1;
                        mptPoint2 = WorldPoint;
                        cManhole.RotationPnt = mptPoint2;

                        //mobjEditService.Rotate(mptPoint2);

                        //if (mobjEditPointService.GeometryCount > 0) mobjEditPointService.RemoveAllGeometries();
                        //if (mobjEditService.GeometryCount > 0) mobjEditPointService.AddGeometry(mobjEditService.GetGeometry(1), StyleManhole);
                        if (mobjEditService.GeometryCount > 0) mobjEditService.RemoveAllGeometries();
                        CreateManholeGeom(mptPoint1, mptPoint2, mobjEditService);

                        if ((_distance >= 200) && (PGeoLib.CheckMapWindow(WindowCaption)))
                        {
                            if (mobjEditServiceNew.GeometryCount > 0) mobjEditServiceNew.RemoveAllGeometries();
                            CreateManholeGeom(mptPoint1, mptPoint2, mobjEditServiceNew);
                        }
                        mintState = 5;
                    }
                    else if (mintState == 6)
                    {
                        mintState = -1;
                        ManholePoint _pnt1 = cManhole.ManholeWall[0];
                        int fNbr = 0;

                        if (mptPoint2 == null)     
                        {
                            double len1 = PGeoLib.GetLength(WorldPoint.X, WorldPoint.Y, cManhole.X + _pnt1.X, cManhole.Y + _pnt1.Y);
                            for (int i = 0; i < cManhole.ManholeWall.Count; i++)
                            {
                                ManholePoint _pnt = cManhole.ManholeWall[i];
                                double len2 = PGeoLib.GetLength(WorldPoint.X, WorldPoint.Y, cManhole.X + _pnt.X, cManhole.Y + _pnt.Y);
                                if (len2 < len1)
                                {
                                    fNbr = i;
                                    len1 = len2;
                                }
                            }
                        }
                        else
                        {
                            IGTPoint objPointTmp = GTClassFactory.Create<IGTPoint>();
                            double angle = PGeoLib.GetAngle(mptPoint1.X, mptPoint1.Y, mptPoint2.X, mptPoint2.Y);
                            objPointTmp.X = cManhole.X + _pnt1.X;
                            objPointTmp.Y = cManhole.Y + _pnt1.Y;
                            double txtX = PGeoLib.RotatePoint(mptPoint1, angle, objPointTmp).X;
                            double txtY = PGeoLib.RotatePoint(mptPoint1, angle, objPointTmp).Y;
                            double len1 = PGeoLib.GetLength(WorldPoint.X, WorldPoint.Y, txtX, txtY);
                            for (int i = 0; i < cManhole.ManholeWall.Count; i++)
                            {
                                ManholePoint _pnt = cManhole.ManholeWall[i];
                                objPointTmp.X = cManhole.X + _pnt.X;
                                objPointTmp.Y = cManhole.Y + _pnt.Y;
                                txtX = PGeoLib.RotatePoint(mptPoint1, angle, objPointTmp).X;
                                txtY = PGeoLib.RotatePoint(mptPoint1, angle, objPointTmp).Y;
                                //IGTTextPointGeometry txtNumber2 = PGeoLib.CreateTextGeom(txtX, txtY, "o" + Convert.ToString(i));
                                //mobjEditServiceTemp.AddGeometry(txtNumber2, cManhole.ManholeWallStyle);

                                double len2 = PGeoLib.GetLength(WorldPoint.X, WorldPoint.Y, txtX, txtY);
                                if (len2 < len1)
                                {
                                    fNbr = i;
                                    len1 = len2;
                                }
                            }
                        }

                        cManhole.FirstNumber = fNbr;

                        if (mobjEditPointService.GeometryCount > 0) mobjEditPointService.RemoveAllGeometries();
                        if (mobjEditServiceTemp.GeometryCount > 0) mobjEditServiceTemp.RemoveAllGeometries();
                        if (mobjEditService.GeometryCount > 0) mobjEditService.RemoveAllGeometries();

                        if (mobjEditPointServiceNew.GeometryCount > 0) mobjEditPointServiceNew.RemoveAllGeometries();
                        if (mobjEditServiceNewTemp.GeometryCount > 0) mobjEditServiceNewTemp.RemoveAllGeometries();
                        if (mobjEditServiceNew.GeometryCount > 0) mobjEditServiceNew.RemoveAllGeometries();

                        for (int i = 0; i < cManhole.ManholeWall.Count; i++)
                        {
                            if (fNbr >= cManhole.ManholeWall.Count) fNbr = 0;
                            ManholePoint _pnt = cManhole.ManholeWall[fNbr];
                            IGTTextPointGeometry txtNumber = PGeoLib.CreateTextGeom(cManhole.X + _pnt.X, cManhole.Y + _pnt.Y, Convert.ToString(i + 1));
                            mobjEditService.AddGeometry(txtNumber, cManhole.ManholeWallStyle);

                            if ((_distance >= 200) && (PGeoLib.CheckMapWindow(WindowCaption)))
                            {
                                txtNumber = PGeoLib.CreateTextGeom(cManhole.X + _pnt.X, cManhole.Y + _pnt.Y, Convert.ToString(i + 1));
                                mobjEditServiceNew.AddGeometry(txtNumber, cManhole.ManholeWallStyle);
                            }

                            fNbr++;
                        }

                        //IGTTextPointGeometry txtNumber1 = PGeoLib.CreateTextGeom(WorldPoint.X, WorldPoint.Y, "x");
                        //mobjEditService.AddGeometry(txtNumber1, cManhole.ManholeWallStyle);

                        CreateManholeGeom(mptPoint1, mptPoint2, mobjEditService);
                        if ((_distance >= 200) && (PGeoLib.CheckMapWindow(WindowCaption)))
                        {
                            CreateManholeGeom(mptPoint1, mptPoint2, mobjEditServiceNew);
                        }

                        mintState = 7;
                    }
                    else if (mintState == 7)
                    {
                        mintState = -1;

                        cManhole.LabelOrigin = WorldPoint;

                        m_oGTTransactionManager.Begin("Place new manhole");
                        CreateManhole(cManhole, mobjEditService);
                        m_oGTTransactionManager.Commit();
                        m_oGTTransactionManager.RefreshDatabaseChanges();

                        if (mobjEditPointService.GeometryCount > 0) mobjEditPointService.RemoveAllGeometries();
                        if (mobjEditServiceTemp.GeometryCount > 0) mobjEditServiceTemp.RemoveAllGeometries();
                        if (mobjEditService.GeometryCount > 0) mobjEditService.RemoveAllGeometries();

                        if (mobjEditPointServiceNew.GeometryCount > 0) mobjEditPointServiceNew.RemoveAllGeometries();
                        if (mobjEditServiceNewTemp.GeometryCount > 0) mobjEditServiceNewTemp.RemoveAllGeometries();
                        if (mobjEditServiceNew.GeometryCount > 0) mobjEditServiceNew.RemoveAllGeometries();

                        if (ContinueNext)
                            mintState = 8;
                        else
                        {
                            if (m_oGTCustomCommandHelper != null) m_oGTCustomCommandHelper.Complete();
                            return;
                        }
                    }
                    else if (mintState == 8)
                    {
                        mintState = 1;

                        mobjEditPointService.TargetMapWindow = application.ActiveMapWindow;
                        mobjEditServiceTemp.TargetMapWindow = application.ActiveMapWindow;
                        mobjEditService.TargetMapWindow = application.ActiveMapWindow;

                        ContinueNext = false;

                        string mstrTransactionName = "Place Manhole Attribute";
                        m_oGTTransactionManager.Begin(mstrTransactionName);

                        mobjManholeAttribute = application.DataContext.NewFeature(iManholeFNO, true);
                        mobjExplorerService.ExploreFeature(mobjManholeAttribute, "Manhole");

                        mblnVisible = mobjExplorerService.Visible;

                        mobjExplorerService.Visible = true;
                        mobjExplorerService.Slide(true);
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
                    application.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Enter manhole attributes");
                }
                else if ((mintState == 2))
                {
                    application.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Identify existing manhole/node/pole");

                    double X = WorldPoint.X;
                    double Y = WorldPoint.Y;

                    mptSelected = null;
                    mobjLocateService = application.ActiveMapWindow.LocateService;
                    IGTDDCKeyObjects objs = mobjLocateService.Locate(WorldPoint, 50, 10, GTSelectionTypeConstants.gtmwstSelectAll);

                    application.ActiveMapWindow.HighlightedObjects.Clear();
                    application.ActiveMapWindow.HighlightedObjects.AddMultiple(objs);
                    //foreach (IGTDDCKeyObject oDDCKeyObject in objs)
                    //{
                    //    if (oDDCKeyObject.ComponentViewName == "VGC_MANHL_S")
                    //    {
                    //        I
                    //    }
                    //}
                }
                else if ((mintState == 3))
                {
                    double X = WorldPoint.X;
                    double Y = WorldPoint.Y;

                    if (mptSelected == null) mptSelected = WorldPoint;
                    double _angle = PGeoLib.GetAngle(mptSelected.X, mptSelected.Y, X, Y);
                    double _len = PGeoLib.GetLength(mptSelected.X, mptSelected.Y, X, Y);

                    double xManhole = WorldPoint.X;
                    double yManhole = WorldPoint.Y;

                    xManhole = mptSelected.X + Math.Cos(_angle) * (_distance);
                    yManhole = mptSelected.Y + Math.Sin(_angle) * (_distance);

                    IGTLineGeometry oLine = PGeoLib.CreateLineGeom(mptSelected.X, mptSelected.Y, xManhole, yManhole);
                    IGTTextPointGeometry oText = PGeoLib.CreateTextGeom((mptSelected.X + xManhole) / 2, (mptSelected.Y + yManhole) / 2,
                                            string.Format("{0:0.0}m", PGeoLib.GetLength(mptSelected.X, mptSelected.Y, xManhole, yManhole)),
                                            _angle, GTAlignmentConstants.gtalCenterCenter);
                    if (mobjEditServiceTemp.GeometryCount > 0) mobjEditServiceTemp.RemoveAllGeometries();
                    mobjEditServiceTemp.AddGeometry(oLine, 11516);
                    mobjEditServiceTemp.AddGeometry(oText, cManhole.ManholeWallStyle);

                    //if (mobjEditPointService.GeometryCount > 0) mobjEditPointService.RemoveAllGeometries();
                    IGTOrientedPointGeometry oManhole;
                    oManhole = PGeoLib.CreatePointGeom(xManhole, yManhole);
                    mobjEditServiceTemp.AddGeometry(oManhole, StyleTempManhole);

                    if ((_distance >= 200) && (PGeoLib.CheckMapWindow(WindowCaption)))
                    {
                        if (mobjEditServiceNewTemp.GeometryCount > 0) mobjEditServiceNewTemp.RemoveAllGeometries();
                        mobjEditServiceNewTemp.TargetMapWindow = ToWindow;
                        mobjEditServiceNewTemp.AddGeometry(oLine, 11516);
                        mobjEditServiceNewTemp.AddGeometry(oText, cManhole.ManholeWallStyle);
                        mobjEditServiceNewTemp.AddGeometry(oManhole, StyleTempManhole);
                    }

                    application.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Point to accept location or right-click to exit");
                }
                else if (mintState == 4)
                {
                    double X = WorldPoint.X;
                    double Y = WorldPoint.Y;

                    if (mptSelected == null) mptSelected = WorldPoint;

                    double xManhole = WorldPoint.X;
                    double yManhole = WorldPoint.Y;

                    if (mobjEditServiceTemp.GeometryCount > 0) mobjEditServiceTemp.RemoveAllGeometries();

                    //if (mobjEditPointService.GeometryCount > 0) mobjEditPointService.RemoveAllGeometries();
                    IGTOrientedPointGeometry oManhole;
                    oManhole = PGeoLib.CreatePointGeom(xManhole, yManhole);
                    mobjEditServiceTemp.AddGeometry(oManhole, StyleTempManhole);

                    application.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Point to enter new location or right-click to accept current location");
                }
                else if (mintState == 41)
                {
                    double X = WorldPoint.X;
                    double Y = WorldPoint.Y;

                    if (mptSelected == null) mptSelected = WorldPoint;
                    double _angle = PGeoLib.GetAngle(mptSelected.X, mptSelected.Y, X, Y);
                    double _len = PGeoLib.GetLength(mptSelected.X, mptSelected.Y, X, Y);

                    double xManhole = WorldPoint.X;
                    double yManhole = WorldPoint.Y;

                    if (_len > _distance + 3)
                    {
                        xManhole = mptSelected.X + Math.Cos(_angle) * (_distance + 3);
                        yManhole = mptSelected.Y + Math.Sin(_angle) * (_distance + 3);
                    }

                    if ((_len < _distance - 3) && (_distance > 3))
                    {
                        xManhole = mptSelected.X + Math.Cos(_angle) * (_distance - 3);
                        yManhole = mptSelected.Y + Math.Sin(_angle) * (_distance - 3);
                    }

                    IGTLineGeometry oLine = PGeoLib.CreateLineGeom(mptSelected.X, mptSelected.Y, xManhole, yManhole);
                    IGTTextPointGeometry oText = PGeoLib.CreateTextGeom((mptSelected.X + xManhole) / 2, (mptSelected.Y + yManhole) / 2,
                                            string.Format("{0:0.0}m", PGeoLib.GetLength(mptSelected.X, mptSelected.Y, xManhole, yManhole)),
                                            PGeoLib.Rad2Deg(_angle), GTAlignmentConstants.gtalCenterCenter);
                    if (mobjEditServiceTemp.GeometryCount > 0) mobjEditServiceTemp.RemoveAllGeometries();
                    mobjEditServiceTemp.AddGeometry(oLine, 11516);
                    mobjEditServiceTemp.AddGeometry(oText, cManhole.ManholeWallStyle);

                    //if (mobjEditPointService.GeometryCount > 0) mobjEditPointService.RemoveAllGeometries();
                    IGTOrientedPointGeometry oManhole;
                    oManhole = PGeoLib.CreatePointGeom(xManhole, yManhole);
                    mobjEditServiceTemp.AddGeometry(oManhole, StyleTempManhole);

                    if ((_distance >= 200) && (PGeoLib.CheckMapWindow(WindowCaption)))
                    {
                        if (mobjEditServiceNewTemp.GeometryCount > 0) mobjEditServiceNewTemp.RemoveAllGeometries();
                        mobjEditServiceNewTemp.TargetMapWindow = ToWindow;
                        mobjEditServiceNewTemp.AddGeometry(oLine, 11516);

                        oText = PGeoLib.CreateTextGeom((xManhole - Math.Cos(_angle) * 5), 
                                                       (yManhole - Math.Sin(_angle) * 5),
                                            string.Format("{0:0.0}m", PGeoLib.GetLength(mptSelected.X, mptSelected.Y, xManhole, yManhole)),
                                            PGeoLib.Rad2Deg(_angle), GTAlignmentConstants.gtalCenterCenter);
                        mobjEditServiceNewTemp.AddGeometry(oText, cManhole.ManholeWallStyle);
                        mobjEditServiceNewTemp.AddGeometry(oManhole, StyleTempManhole);
                    }
                    application.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Point to enter new location or right-click to accept current location");
                }
                else if (mintState == 5)
                {
                    IGTPoint mptPoint2 = MouseEventArgs.WorldPoint;
                    mobjEditPointService.Rotate(mptPoint2);
                    //CreateManholeGeom(mptPoint1, mptPoint2, mobjEditPointService);

                    IGTLineGeometry oLine = PGeoLib.CreateLineGeom(mptPoint1.X, mptPoint1.Y, mptPoint2.X, mptPoint2.Y);
                    if (mobjEditServiceTemp.GeometryCount > 0) mobjEditServiceTemp.RemoveAllGeometries();
                    mobjEditServiceTemp.AddGeometry(oLine, 11516);

                    if ((_distance >= 200) && (PGeoLib.CheckMapWindow(WindowCaption)))
                    {
                        mobjEditPointServiceNew.Rotate(mptPoint2);

                        if (mobjEditServiceNewTemp.GeometryCount > 0) mobjEditServiceNewTemp.RemoveAllGeometries();
                        mobjEditServiceNewTemp.TargetMapWindow = ToWindow;
                        mobjEditServiceNewTemp.AddGeometry(oLine, 11516);
                    }
                    application.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Point to define new angle of facility or right-click to accept");
                }
                else if (mintState == 6)
                {
                    application.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Identify [+] to be wall 1");
                }
                else if (mintState == 7)
                {
                    IGTPoint mptPoint2 = MouseEventArgs.WorldPoint;

                    IGTTextPointGeometry oText = PGeoLib.CreateTextGeom(mptPoint2.X, mptPoint2.Y, cManhole.ManholeLabel, 0, cManhole.ManholeLabelAlignment);
                    if (mobjEditServiceTemp.GeometryCount > 0) mobjEditServiceTemp.RemoveAllGeometries();
                    mobjEditServiceTemp.AddGeometry(oText, cManhole.ManholeTextStyle);

                    if ((_distance >= 200) && (PGeoLib.CheckMapWindow(WindowCaption)))
                    {
                        if (mobjEditServiceNewTemp.GeometryCount > 0) mobjEditServiceNewTemp.RemoveAllGeometries();
                        mobjEditServiceNewTemp.AddGeometry(oText, cManhole.ManholeTextStyle);
                    }
                    application.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Enter new location for text");
                }
                else if (mintState == 8)
                {
                    application.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Click to place other manhole or right-click to exit.");
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
                mintState = 21;
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
                //Service without exitcmd
                if (m_oGTCustomCommandHelper != null) m_oGTCustomCommandHelper.Complete();

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
                ContinueNext = true;
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
