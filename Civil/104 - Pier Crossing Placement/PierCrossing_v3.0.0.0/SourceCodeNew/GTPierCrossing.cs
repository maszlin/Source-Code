using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Intergraph.GTechnology.Interfaces;
using Intergraph.GTechnology.API;
using ADODB;

namespace NEPS.GTechnology.PierCrossing
{
    class GTPierCrossing : Intergraph.GTechnology.Interfaces.IGTCustomCommandModeless
    {
        #region members declaration
        public static Intergraph.GTechnology.API.IGTTransactionManager m_oIGTTransactionManager = null;
        public static Intergraph.GTechnology.API.IGTCustomCommandHelper m_oIGTCustomCommandHelper = null;
        public static Intergraph.GTechnology.API.IGTApplication application = null;
        public static Intergraph.GTechnology.API.IGTFeatureExplorerService mobjExplorerService = null;
        IGTLocateService mobjLocateService = null;
        public IGTPoint SelGeom = null;
        public Boolean placePX = false;
        public Boolean rotatePX = false;
        public Boolean tempRotatePX = false;
        public Boolean confirmRotationPX = false;
        public Boolean tempMovePX = false;
        public Boolean confirmMovePX = false;
        public Boolean initMovePX = false;
        public Boolean initDeletePX = false;

        frmPierCrossing PierCrossCustComFrm = null;

        //private Logger log;
        //private short iPierCrossFNO = 3600;

        #endregion

        #region Event Handlers
        void m_oIGTCustomCommandHelper_Click(object sender, GTMouseEventArgs e)
        {
            try
            {
                IGTPolylineGeometry oPolyline = GTClassFactory.Create<IGTPolylineGeometry>();
                if (e.Button != 2)
                {
                    if (PierCrossCustComFrm != null)
                    {
                        /***************Check Add Mode******************/

                        switch (frmPierCrossing.addMode)
                        {
                            case 1:
                                frmPierCrossing.RefPoints.Add(e.WorldPoint);
                                oPolyline.Points.Add(e.WorldPoint);
                                frmPierCrossing.gGeomCS.TargetMapWindow = application.ActiveMapWindow;
                                frmPierCrossing.gGeomCS.RemoveAllGeometries();
                                frmPierCrossing.gPXIdx = frmPierCrossing.gGeomCS.CreateGeometry(GTGeometryTypeConstants.gtgtPolylineGeometry, (int)GTStyleIDConstants.gtstyleLineSelectSolid2);
                                frmPierCrossing.gPXIdx = frmPierCrossing.gGeomCS.AddGeometry(oPolyline, (int)GTStyleIDConstants.gtstyleLineSelectSolid2);
                                frmPierCrossing.addMode = 2; // activate the second point selection
                                application.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Left Click to place second reference point for Pier Crossing. Right Click to delete first reference point.");
                                application.Application.ActiveMapWindow.MousePointer = GTMousePointerConstants.gtmwmpCrossHair;
                                break;

                            case 2:
                                frmPierCrossing.RefPoints.Add(e.WorldPoint);
                                frmPierCrossing.addMode = 3;
                                application.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Wait, creating in process ...");
                                application.Application.ActiveMapWindow.MousePointer = GTMousePointerConstants.gtmwmpCrossHair;
                                PierCrossCustComFrm._placePX = placePX;
                                break;

                            default:
                                break;
                        }

                        /***************Check Rotate Mode******************/

                        switch (frmPierCrossing.rotateMode)
                        {
                            case 1:
                                application.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Select one Pier Crossing feature for rotation. Right Click to cancel rotation");
                                application.Application.ActiveMapWindow.MousePointer = GTMousePointerConstants.gtmwmpCrossHair;
                                IGTDDCKeyObjects feat = mobjLocateService.Locate(e.WorldPoint, 20, 1, GTSelectionTypeConstants.gtmwstSelectSingle);
                                IGTDDCKeyObjects feat2 = GTClassFactory.Create<IGTDDCKeyObjects>();
                                for (int K = 0; K < feat.Count; K++)
                                {
                                    feat2 = application.DataContext.GetDDCKeyObjects(feat[K].FNO, feat[K].FID, GTComponentGeometryConstants.gtddcgAllGeographic);
                                    break;
                                }
                                for (int K = 0; K < feat2.Count; K++)
                                    application.SelectedObjects.Add(GTSelectModeConstants.gtsosmAllDisplayedComponentsInActiveLegend, feat2[K]);

                                if (application.SelectedObjects.FeatureCount == 1)
                                {
                                    PierCrossCustComFrm._initRotatePX = rotatePX; // initiate the pier cross for rotation.
                                }
                                else if (application.SelectedObjects.FeatureCount > 1)
                                {
                                    MessageBox.Show("Please select only one Pier Crossing feature to rotate", "Pier Crossing", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                    application.SelectedObjects.Clear();
                                }
                                break;

                            case 3:
                                frmPierCrossing.gGeomES.Rotate(e.WorldPoint);
                                PierCrossCustComFrm._SelGeom = e.WorldPoint;
                                application.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Updating database ...");
                                application.Application.ActiveMapWindow.MousePointer = GTMousePointerConstants.gtmwmpCrossHair;
                                PierCrossCustComFrm._tempRotatePX = tempRotatePX; //confirm Rotation and set the image
                                break;

                            case 5:
                                application.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Double Left Click to accept the rotation angel. Right Click to cancel rotation");
                                application.Application.ActiveMapWindow.MousePointer = GTMousePointerConstants.gtmwmpCrossHair;
                                PierCrossCustComFrm._tempRotatePX = tempRotatePX; // initiate the pier cross for rotation.
                                break;

                            default: break;
                        }

                        /***************Check Move Mode******************/

                        switch (frmPierCrossing.moveMode)
                        {
                            case 1:

                                application.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Select one Pier Crossing feature to move. Right Click to cancel moving");
                                application.Application.ActiveMapWindow.MousePointer = GTMousePointerConstants.gtmwmpCrossHair;
                                IGTDDCKeyObjects feat = mobjLocateService.Locate(e.WorldPoint, 20, 1, GTSelectionTypeConstants.gtmwstSelectSingle);
                                IGTDDCKeyObjects feat2 = GTClassFactory.Create<IGTDDCKeyObjects>();
                                for (int K = 0; K < feat.Count; K++)
                                {
                                    feat2 = application.DataContext.GetDDCKeyObjects(feat[K].FNO, feat[K].FID, GTComponentGeometryConstants.gtddcgAllGeographic);
                                    break;
                                }
                                for (int K = 0; K < feat2.Count; K++)
                                    application.SelectedObjects.Add(GTSelectModeConstants.gtsosmAllDisplayedComponentsInActiveLegend, feat2[K]);
                                if (application.SelectedObjects.FeatureCount == 1)
                                {
                                    PierCrossCustComFrm._initMovePX = initMovePX;
                                }
                                else if (application.SelectedObjects.FeatureCount > 1)
                                {
                                    MessageBox.Show("Please select only one Pier Crossing feature to move", "Pier Crossing", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                    application.SelectedObjects.Clear();
                                }
                                break;

                            case 3:

                                frmPierCrossing.gGeomES.Move(e.WorldPoint);
                                PierCrossCustComFrm._SelGeom = e.WorldPoint;
                                //  frmPierCrossing.moveMode = 4;
                                application.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Updating database ...");
                                application.Application.ActiveMapWindow.MousePointer = GTMousePointerConstants.gtmwmpCrossHair;
                                PierCrossCustComFrm._tempMovePX = tempMovePX;
                                break;

                            case 5:
                                application.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Double Left Click to accept the new position. Right Click to cancel moving");
                                application.Application.ActiveMapWindow.MousePointer = GTMousePointerConstants.gtmwmpCrossHair;
                                PierCrossCustComFrm._tempMovePX = tempMovePX;// initiate the pier cross for moving.
                                break;
                            default: break;
                        }

                        /***************Check Delete Mode******************/
                        if (frmPierCrossing.deleteMode == 1)
                        {
                            application.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Select one Pier Crossing feature to delete. Right Click to cancel deletion");
                            application.Application.ActiveMapWindow.MousePointer = GTMousePointerConstants.gtmwmpCrossHair;
                            IGTDDCKeyObjects feat = mobjLocateService.Locate(e.WorldPoint, 20, 1, GTSelectionTypeConstants.gtmwstSelectSingle);
                            IGTDDCKeyObjects feat2 = GTClassFactory.Create<IGTDDCKeyObjects>();
                            for (int K = 0; K < feat.Count; K++)
                            {
                                feat2 = application.DataContext.GetDDCKeyObjects(feat[K].FNO, feat[K].FID, GTComponentGeometryConstants.gtddcgAllGeographic);
                                break;
                            }
                            for (int K = 0; K < feat2.Count; K++)
                                application.SelectedObjects.Add(GTSelectModeConstants.gtsosmAllDisplayedComponentsInActiveLegend, feat2[K]);
                            if (application.SelectedObjects.FeatureCount == 1)
                            {
                                PierCrossCustComFrm._initDeletePX = initDeletePX;
                            }
                            else if (application.SelectedObjects.FeatureCount > 1)
                            {
                                MessageBox.Show("Please select only one Pier Crossing feature to delete", "Pier Crossing", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                application.SelectedObjects.Clear();
                            }
                        }
                    }
                }
                else if (e.Button == 2)//right Left Click
                {
                    if (frmPierCrossing.addMode == 1)
                    {
                        PierCrossCustComFrm.Cancel_Placement();
                    }

                    if (frmPierCrossing.addMode == 2)
                    {
                        if (frmPierCrossing.RefPoints.Count == 1)
                        {
                            frmPierCrossing.RefPoints.Clear();
                            frmPierCrossing.gGeomCS.RemoveAllGeometries();
                            frmPierCrossing.addMode = 1;
                            application.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Left Click to place first reference point for Pier Crossing Geo Line. Right Click to cancel placement");
                            application.Application.ActiveMapWindow.MousePointer = GTMousePointerConstants.gtmwmpCrossHair;
                            application.ActiveMapWindow.Activate();
                        }
                    }

                    if ((frmPierCrossing.rotateMode == 1) || (frmPierCrossing.rotateMode == 3))
                    {
                        PierCrossCustComFrm.Cancel_Rotation();
                    }

                    if ((frmPierCrossing.moveMode == 1) || (frmPierCrossing.moveMode == 3) || (frmPierCrossing.moveMode == 5))
                    {
                        PierCrossCustComFrm.Cancel_Move();
                    }

                    if (frmPierCrossing.deleteMode == 1)
                    {
                        PierCrossCustComFrm.Cancel_Deletion();
                    }

                    if (frmPierCrossing.rotateMode == 6)
                    {
                        IGTDDCKeyObjects oGTKeyObjs;

                        GTPierCrossing.m_oIGTTransactionManager.Rollback();
                        application.SelectedObjects.Clear();
                        oGTKeyObjs = (IGTDDCKeyObjects)application.DataContext.GetDDCKeyObjects(frmPierCrossing.selectedFNO, frmPierCrossing.selectedFID, GTComponentGeometryConstants.gtddcgAllGeographic);
                        for (int K = 0; K < oGTKeyObjs.Count; K++)
                            application.SelectedObjects.Add(GTSelectModeConstants.gtsosmAllDisplayedComponentsInActiveLegend, oGTKeyObjs[K]);

                        application.RefreshWindows();
                        frmPierCrossing.rotateMode = 4;
                        PierCrossCustComFrm._initRotatePX = rotatePX;
                    }
                    if (frmPierCrossing.moveMode == 6)
                    {
                        IGTDDCKeyObjects oGTKeyObjs;

                        GTPierCrossing.m_oIGTTransactionManager.Rollback();
                        application.SelectedObjects.Clear();
                        oGTKeyObjs = (IGTDDCKeyObjects)application.DataContext.GetDDCKeyObjects(frmPierCrossing.selectedFNO, frmPierCrossing.selectedFID, GTComponentGeometryConstants.gtddcgAllGeographic);
                        for (int K = 0; K < oGTKeyObjs.Count; K++)
                            application.SelectedObjects.Add(GTSelectModeConstants.gtsosmAllDisplayedComponentsInActiveLegend, oGTKeyObjs[K]);

                        application.RefreshWindows();
                        frmPierCrossing.moveMode = 4;
                        PierCrossCustComFrm._initMovePX = initMovePX;
                    }
                }
            }
            catch (Exception e1)
            {
                throw e1;
            }
        }

        void m_oIGTCustomCommandHelper_MouseMove(object sender, GTMouseEventArgs e)
        {
            try
            {
                if (PierCrossCustComFrm != null)
                {
                    /***************Check Add Mode******************/
                    switch (frmPierCrossing.addMode)
                    {
                        case 1:
                            application.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Left Click to place first reference point for Pier Crossing. Right Click to cancel placement");
                            application.Application.ActiveMapWindow.MousePointer = GTMousePointerConstants.gtmwmpCrossHair;
                            break;

                        case 2:
                            application.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Left Click to place second reference point for Pier Crossing. Right Click  to delete first reference point.");
                            application.Application.ActiveMapWindow.MousePointer = GTMousePointerConstants.gtmwmpCrossHair;
                            frmPierCrossing.gGeomCS.SetDynamicPoint(frmPierCrossing.gPXIdx, e.WorldPoint);
                            break;

                        case 3:
                            application.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Wait, creating in process ...");
                            application.Application.ActiveMapWindow.MousePointer = GTMousePointerConstants.gtmwmpCrossHair;
                            break;

                        default: break;
                    }

                    /***************Check Rotate Mode******************/
                    switch (frmPierCrossing.rotateMode)
                    {
                        case 1:
                            application.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Select one Pier Crossing feature for rotation. Right Click to cancel rotation");
                            application.Application.ActiveMapWindow.MousePointer = GTMousePointerConstants.gtmwmpCrossHair;
                            break;

                        case 2:
                            application.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Checking selected feature ...");
                            break;

                        case 3:
                            application.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Left Click to accept the new rotation angle. Press Space to toggle rotation origin. Right Click to cancel rotation");
                            application.Application.ActiveMapWindow.MousePointer = GTMousePointerConstants.gtmwmpCrossHair;
                            frmPierCrossing.gGeomES.Rotate(e.WorldPoint);
                            break;

                        case 4:

                            application.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Move the pointer to indicate the rotation angle. Right Click to cancel rotation");
                            application.Application.ActiveMapWindow.MousePointer = GTMousePointerConstants.gtmwmpNWArrow;
                            frmPierCrossing.rotateMode = 5; // temporarily accept the new angle upon Left Click. 
                            PierCrossCustComFrm._initRotatePX = rotatePX;
                            break;

                        default:
                            break;
                    }

                    /***************Check Move Mode******************/

                    switch (frmPierCrossing.moveMode)
                    {
                        case 1:
                            application.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Select one Pier Crossing feature to move. Right Click to cancel moving");
                            application.Application.ActiveMapWindow.MousePointer = GTMousePointerConstants.gtmwmpCrossHair;
                            break;

                        case 2:
                            application.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Checking selected feature ...");
                            break;

                        case 3:
                            application.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Left Click to accept the new position. Right Click to cancel moving");
                            application.Application.ActiveMapWindow.MousePointer = GTMousePointerConstants.gtmwmpCrossHair;
                            frmPierCrossing.gGeomES.Move(e.WorldPoint);
                            break;
                        
                        case 4:

                            application.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Move the pointer to indicate the position. Right Click to cancel moving");
                            application.Application.ActiveMapWindow.MousePointer = GTMousePointerConstants.gtmwmpNWArrow;
                            frmPierCrossing.moveMode = 5; // temporarily accept the new angle upon Left Click. 
                            PierCrossCustComFrm._initMovePX = initMovePX;
                            break;
                       

                        default:
                            break;
                    }

                    /***************Check Delete Mode******************/

                    switch (frmPierCrossing.deleteMode)
                    {
                        case 1:
                            application.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Select one Pier Crossing feature to delete. Right Click to cancel deletion");
                            application.Application.ActiveMapWindow.MousePointer = GTMousePointerConstants.gtmwmpCrossHair;
                            break;

                        case 4:
                            application.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Updating database ...");
                            break;

                        default:
                            break;

                    }

                    // TO BE REMOVED <<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<
                    //application.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, frmPierCrossing.rotateMode.ToString());
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        void m_oIGTCustomCommandHelper_DblClick(object sender, GTMouseEventArgs e)
        {
            //application.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "DblLeft Click.");
            if (frmPierCrossing.rotateMode == 6)
                PierCrossCustComFrm._confRotatePX = confirmRotationPX;
            if (frmPierCrossing.moveMode == 6)
                PierCrossCustComFrm._confMovePX = confirmMovePX;
        }

        void m_oIGTCustomCommandHelper_KeyUp(object sender, GTKeyEventArgs e)
        {
            if (e.KeyCode == (short)Keys.Space)
            {
                if (frmPierCrossing.rotateMode == 3)
                {
                    switch (frmPierCrossing.rotateOrigin)
                    {

                        case 0:
                            frmPierCrossing.gGeomES.EndRotate(frmPierCrossing.RotationPoints[2]);
                            frmPierCrossing.gGeomES.BeginRotate(frmPierCrossing.RotationPoints[0], frmPierCrossing.RotationPoints[1]);
                            frmPierCrossing.rotateOrigin = 1;
                            application.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Left Click to accept the new rotation angle. Press Space to toggle rotation origin. Right Click to cancel rotation");
                            application.Application.ActiveMapWindow.MousePointer = GTMousePointerConstants.gtmwmpCrossHair;
                            break;

                        case 1:
                            frmPierCrossing.gGeomES.EndRotate(frmPierCrossing.RotationPoints[0]);
                            frmPierCrossing.gGeomES.BeginRotate(frmPierCrossing.RotationPoints[0], frmPierCrossing.RotationPoints[2]);
                            frmPierCrossing.rotateOrigin = 2;
                            application.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Left Click to accept the new rotation angle. Press Space to toggle rotation origin. Right Click to cancel rotation");
                            application.Application.ActiveMapWindow.MousePointer = GTMousePointerConstants.gtmwmpCrossHair;
                            break;

                        case 2:
                            frmPierCrossing.gGeomES.EndRotate(frmPierCrossing.RotationPoints[0]);
                            frmPierCrossing.gGeomES.BeginRotate(frmPierCrossing.RotationPoints[2], frmPierCrossing.RotationPoints[0]);
                            frmPierCrossing.rotateOrigin = 0;
                            application.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Left Click to accept the new rotation angle. Press Space to toggle rotation origin. Right Click to cancel rotation");
                            application.Application.ActiveMapWindow.MousePointer = GTMousePointerConstants.gtmwmpCrossHair;
                            break;

                        default: break;

                    }
                }
            }

        }

        void m_oIGTCustomCommandHelper_KeyDown(object sender, GTKeyEventArgs e)
        {
            //application.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "KeyDown.");
           

           
        }

        #endregion
      
#region IGTCustomCommandModeless Members

        public void Activate(Intergraph.GTechnology.API.IGTCustomCommandHelper CustomCommandHelper)
        {
            try
            {
                application = GTClassFactory.Create<IGTApplication>();
                application.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Running Pier Crossing ...");
                m_oIGTCustomCommandHelper = CustomCommandHelper;
                PierCrossCustComFrm = new frmPierCrossing();
                PierCrossCustComFrm.FormClosed += new FormClosedEventHandler(m_CustomForm_FormClosed);
                frmPierCrossing.gGeomCS = GTClassFactory.Create<IGTGeometryCreationService>();
                mobjLocateService = application.ActiveMapWindow.LocateService;

                SubscribeEvents();
                // Feature Explorer Service
                mobjExplorerService = GTClassFactory.Create<IGTFeatureExplorerService>();
                mobjExplorerService.CancelClick += new EventHandler(mobjExplorerService_CancelClick);
                mobjExplorerService.SaveAndContinueClick += new EventHandler(mobjExplorerService_SaveAndContinueClick);
                mobjExplorerService.SaveClick += new EventHandler(mobjExplorerService_SaveClick);
                PierCrossCustComFrm.ShowPierForm();
            }
            catch (InvalidCastException er)
            {
                MessageBox.Show(er.Message, "Pier Crossing", MessageBoxButtons.OK
                                , MessageBoxIcon.Error);
                ExitCmd();
            }
        }

        #region FormClosed
        void m_CustomForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            PierCrossCustComFrm.cleanup();
            ExitCmd();
        }
        #endregion

        #region Term/Pause
        public bool CanTerminate
        {
            get
            {
                DialogResult retVal = MessageBox.Show("Do you want to discard your current changes and exit?", "Pier Crossing", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (retVal == DialogResult.Yes)
                {
                    ExitCmd();
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
            frmPierCrossing.addMode += 1000;
            frmPierCrossing.moveMode += 1000;
            frmPierCrossing.rotateMode += 1000;
            frmPierCrossing.deleteMode += 1000;
        }

        public void Resume()
        {
            frmPierCrossing.addMode -= 1000;
            frmPierCrossing.moveMode -= 1000;
            frmPierCrossing.rotateMode -= 1000;
            frmPierCrossing.deleteMode -= 1000;
        }

        public void Terminate()
        {
            try
            {

                if (m_oIGTTransactionManager != null)
                {
                    m_oIGTTransactionManager = null;
                }


            }
            catch (Exception e)
            {
                throw e;
            }

        }
        #endregion

        public Intergraph.GTechnology.API.IGTTransactionManager TransactionManager
        {
            set
            {
                m_oIGTTransactionManager = value;
            }
        }

        #endregion

        #region Feature Explorer
        private void CloseFeatureExplorer()
        {
            // Clears feature explorer, if it was set.
            if (mobjExplorerService != null)
            {
                mobjExplorerService.Clear();
                mobjExplorerService.Visible = false;
            }

           // frmPierCrossing.addMode = 1;

        }

        private void saveAttribute()
        {
           
                m_oIGTTransactionManager.Rollback();
                CloseFeatureExplorer();
            
        }

        public void mobjExplorerService_CancelClick(object sender, EventArgs e)
        {
            try
            {

                application.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Placement is cancelled");
                m_oIGTTransactionManager.Rollback();
                CloseFeatureExplorer();
                PierCrossCustComFrm.Cancel_Placement();
              //  frmPierCrossing.addMode = 1;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Pier Crossing", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void mobjExplorerService_SaveAndContinueClick(object sender, EventArgs e)
        {
            try
            {
                saveAttribute();
                frmPierCrossing.addMode = 1;

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Pier Crossing", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

           
        }

        public void mobjExplorerService_SaveClick(object sender, EventArgs e)
        {
            try
            {
                saveAttribute();
                frmPierCrossing.addMode = 1;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Pier Crossing", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }
        #endregion

        #region Events subs/unsub
        public void SubscribeEvents() // register events. Use + sign
        {
            // Subscribe to m_oIIGTCustomCommandHelper events using C# 1.0 syntax

            m_oIGTCustomCommandHelper.KeyUp += new EventHandler<GTKeyEventArgs>(m_oIGTCustomCommandHelper_KeyUp);
            m_oIGTCustomCommandHelper.KeyDown += new EventHandler<GTKeyEventArgs>(m_oIGTCustomCommandHelper_KeyDown);
            m_oIGTCustomCommandHelper.Click += new EventHandler<GTMouseEventArgs>(m_oIGTCustomCommandHelper_Click);
            m_oIGTCustomCommandHelper.MouseMove += new EventHandler<GTMouseEventArgs>(m_oIGTCustomCommandHelper_MouseMove);
            m_oIGTCustomCommandHelper.DblClick += new EventHandler<GTMouseEventArgs>(m_oIGTCustomCommandHelper_DblClick);

        }

        private void UnsubscribeEvents() // Unregister an event. Use - sign
        {
            // UnSubscribe to m_oIIGTCustomCommandHelper events using C# 1.0 syntax
            m_oIGTCustomCommandHelper.KeyUp -= m_oIGTCustomCommandHelper_KeyUp;
            m_oIGTCustomCommandHelper.KeyDown -= m_oIGTCustomCommandHelper_KeyDown;
            m_oIGTCustomCommandHelper.Click -= m_oIGTCustomCommandHelper_Click;
            m_oIGTCustomCommandHelper.MouseMove -= m_oIGTCustomCommandHelper_MouseMove;
            m_oIGTCustomCommandHelper.DblClick -= m_oIGTCustomCommandHelper_DblClick;
        }
        #endregion

        #region exit customCmd
        public void ExitCmd()
        {
            application.SelectedObjects.Clear();
            application.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Exiting Pier Crossing ...");
            if (frmPierCrossing.gGeomCS != null)
            {
                if(frmPierCrossing.gGeomCS.GeometryCount!=0)
                    frmPierCrossing.gGeomCS.RemoveAllGeometries();
                frmPierCrossing.gGeomCS = null;
            }
           
            if (m_oIGTTransactionManager != null)
            {
                if (m_oIGTTransactionManager.TransactionInProgress)
                    m_oIGTTransactionManager.Rollback();
                m_oIGTTransactionManager = null;
            }
            if (mobjExplorerService != null)
            {
                mobjExplorerService.Clear();
                mobjExplorerService.Visible = false;
                mobjExplorerService.CancelClick -= new EventHandler(mobjExplorerService_CancelClick);
                mobjExplorerService.SaveAndContinueClick -= new EventHandler(mobjExplorerService_SaveAndContinueClick);
                mobjExplorerService.SaveClick -= new EventHandler(mobjExplorerService_SaveClick);
                mobjExplorerService = null;
            }
          //  m_oIGTCustomCommandHelper = null;
            if(PierCrossCustComFrm != null)
                PierCrossCustComFrm = null;
            
            application.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Exited.");
            application.EndProgressBar();
            application.EndWaitCursor();
            application.ActiveMapWindow.MousePointer = GTMousePointerConstants.gtmwmpNWArrow;

            if (m_oIGTCustomCommandHelper != null)
            {
                UnsubscribeEvents();
                m_oIGTCustomCommandHelper.Complete();
                m_oIGTCustomCommandHelper = null;
            }
           // application.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Pier Crossing Exited.");
            //Application.Exit();
        }
        #endregion
      

    }
}
