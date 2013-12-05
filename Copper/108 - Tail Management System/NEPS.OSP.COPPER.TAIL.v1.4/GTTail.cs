using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Intergraph.GTechnology.Interfaces;
using Intergraph.GTechnology.API;
using ADODB;

namespace NEPS.OSP.COPPER.TAIL
{
    class GTTail : Intergraph.GTechnology.Interfaces.IGTCustomCommandModeless
    {
        public static Intergraph.GTechnology.API.IGTTransactionManager m_oIGTTransactionManager = null;
        public static Intergraph.GTechnology.API.IGTCustomCommandHelper m_oIGTCustomCommandHelper = null;

        // for Temporary Drawing
        private static Intergraph.GTechnology.API.IGTApplication m_gtapp = GTClassFactory.Create<IGTApplication>();
        private static Intergraph.GTechnology.API.IGTGeometryEditService mobjEditService = null;
        private static Intergraph.GTechnology.API.IGTGeometryEditService mobjEditServicePoly = null;

        private static Intergraph.GTechnology.API.IGTPolylineGeometry oTempCableLine = GTClassFactory.Create<IGTPolylineGeometry>();
        private static Intergraph.GTechnology.API.IGTPolygonGeometry oTempCablePoly = GTClassFactory.Create<IGTPolygonGeometry>();
        private static Intergraph.GTechnology.API.IGTTextPointGeometry oTempCableText = GTClassFactory.Create<IGTTextPointGeometry>();

        private List<IGTPoint> v_PointLine = new List<IGTPoint>();
        private List<IGTPoint> v_PointPoly = new List<IGTPoint>();
        private List<IGTPoint> v_PointText = new List<IGTPoint>();

        IGTPoint oPntLineStartGeom = GTClassFactory.Create<IGTPoint>();

        private double isStep = 0;

        // for General
        frmTail m_CustomForm = null;
        IGTPoint SelGeom = null;

        #region General Procedure And Function

        private void addGeomToTempGeom(double isX, double isY, string isType)
        {
            IGTPoint oPntGeom = GTClassFactory.Create<IGTPoint>();
            oPntGeom.X = isX;
            oPntGeom.Y = isY;
            if (isType == "LINE")
            {
                v_PointLine.Add(oPntGeom);
            }
            else if (isType == "POLY")
            {
                v_PointPoly.Add(oPntGeom);
            }
            else
            {
                v_PointText.Add(oPntGeom);
            }
        }
        
        #endregion

        #region Event Handlers

        void m_oIGTCustomCommandHelper_Click(object sender, GTMouseEventArgs e)
        {
            GTClassFactory.Create<IGTApplication>().SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Click.");
        }
        
        void m_oIGTCustomCommandHelper_MouseMove(object sender, GTMouseEventArgs e)
        {
            SelGeom = e.WorldPoint;

            if (m_CustomForm != null)
            {
                if (e.MapWindow.DetailID.ToString() != "0")
                {
                    if (isStep != 7) isStep = m_CustomForm.getInitStep;
                    if (isStep == 7)
                    {
                        GTClassFactory.Create<IGTApplication>().SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Click to continue.  Click in Main Joint to Save and Finish Define Geometry. Press Esc to Re-Drawing.");
                        IGTPoint oPntGeom = GTClassFactory.Create<IGTPoint>();
                        oPntGeom.X = SelGeom.X;
                        oPntGeom.Y = SelGeom.Y;

                        if (v_PointPoly.Count == 0)
                        {
                            if (mobjEditServicePoly.GeometryCount > 0) mobjEditServicePoly.RemoveAllGeometries();
                            v_PointPoly = m_CustomForm.isPointPoly;
                            for (int i = 0; i < v_PointPoly.Count; i++)
                            {
                                oTempCablePoly.Points.Add(v_PointPoly[i]);
                            }
                            oTempCablePoly.Points.Add(v_PointPoly[0]);
                            oPntLineStartGeom.X = v_PointPoly[0].X + ((v_PointPoly[3].X - v_PointPoly[0].X) / 2);
                            oPntLineStartGeom.Y = v_PointPoly[0].Y - ((v_PointPoly[0].Y - v_PointPoly[1].Y) / 2);
                            addGeomToTempGeom(oPntLineStartGeom.X, oPntLineStartGeom.Y, "LINE");
                            mobjEditServicePoly.AddGeometry(oTempCablePoly, 7015001);
                        }

                        if (mobjEditService.GeometryCount > 0) mobjEditService.RemoveAllGeometries();

                        oTempCableLine.Points.Clear();

                        for (int i = 0; i < v_PointLine.Count; i++)
                        {
                            oTempCableLine.Points.Add(v_PointLine[i]);
                        }
                        oTempCableLine.Points.Add(oPntGeom);

                        mobjEditService.AddGeometry(oTempCableLine, 10002);

                        System.Windows.Forms.Application.DoEvents();
                    }
                }
            }
        }

        void m_oIGTCustomCommandHelper_DblClick(object sender, GTMouseEventArgs e)
        {
            GTClassFactory.Create<IGTApplication>().SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "DblClick.");
        }

        void m_oIGTCustomCommandHelper_WheelRotate(object sender, GTWheelRotateEventArgs e)
        {
            GTClassFactory.Create<IGTApplication>().SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "WheelRotate.");
        }

        void m_oIGTCustomCommandHelper_MouseUp(object sender, GTMouseEventArgs e)
        {
            GTClassFactory.Create<IGTApplication>().SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "MouseUp.");
            try
            {
                SelGeom = e.WorldPoint;
                if (m_CustomForm != null)
                {
                    if (e.MapWindow.DetailID.ToString() != "0")
                    {

                        isStep = m_CustomForm.getInitStep;

                        if (isStep == 7)
                        {
                            GTClassFactory.Create<IGTApplication>().SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Click to continue.  Click in Main Joint to Save and Finish Define Geometry. Press Esc to Re-Drawing.");
                            if (mobjEditServicePoly.GeometryCount > 0) mobjEditServicePoly.RemoveAllGeometries();
                            for (int i = 0; i < v_PointPoly.Count; i++)
                            {
                                oTempCablePoly.Points.Add(v_PointPoly[i]);
                            }
                            oTempCablePoly.Points.Add(v_PointPoly[0]);
                            mobjEditServicePoly.AddGeometry(oTempCablePoly, 7015001);

                            if (mobjEditService.GeometryCount > 0) mobjEditService.RemoveAllGeometries();

                            addGeomToTempGeom(SelGeom.X, SelGeom.Y, "LINE");

                            oTempCableLine.Points.Clear();
                            for (int i = 0; i < v_PointLine.Count; i++)
                            {
                                oTempCableLine.Points.Add(v_PointLine[i]);
                            }
                            mobjEditService.AddGeometry(oTempCableLine, 10002);

                            if (GTClassFactory.Create<IGTApplication>().SelectedObjects.FeatureCount > 0)
                            {
                                int initFNO = 0;
                                foreach (IGTDDCKeyObject oDDCKeyObject in GTClassFactory.Create<IGTApplication>().SelectedObjects.GetObjects())
                                {
                                    initFNO = oDDCKeyObject.FNO;
                                }
                                if (initFNO == 10800)
                                {
                                    m_CustomForm.getInitStep = 1;
                                    m_CustomForm.isPointLine = v_PointLine;
                                    isStep = 8;
                                }
                            }
                            System.Windows.Forms.Application.DoEvents();
                        }

                        m_CustomForm.XPointGeom = SelGeom.X;
                        m_CustomForm.YPointGeom = SelGeom.Y;
                        m_CustomForm.manageTail = e.MapWindow.DetailID.ToString();
                    }
                }
            }
            catch (Exception e1)
            {
                throw e1;
            }
        }

        void m_oIGTCustomCommandHelper_MouseDown(object sender, GTMouseEventArgs e)
        {
            GTClassFactory.Create<IGTApplication>().SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "MouseDown.");            
        }

        void m_oIGTCustomCommandHelper_LostFocus(object sender, GTLostFocusEventArgs e)
        {
            GTClassFactory.Create<IGTApplication>().SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "LostFocus.");
        }

        void m_oIGTCustomCommandHelper_KeyUp(object sender, GTKeyEventArgs e)
        {
            GTClassFactory.Create<IGTApplication>().SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "KeyUp.");
            if (m_CustomForm != null)
            {
                if (e.MapWindow.DetailID.ToString() != "0")
                {
                    if (isStep == 7)
                    {
                        if (e.KeyCode == 27)
                        {
                            v_PointLine.Clear();
                            oTempCableLine.Points.Clear();
                            mobjEditService.RemoveAllGeometries();
                        }
                    }
                }
            }
        }

        void m_oIGTCustomCommandHelper_GainedFocus(object sender, GTGainedFocusEventArgs e)
        {
            GTClassFactory.Create<IGTApplication>().SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "GainedFocus.");
        }

        void m_oIGTCustomCommandHelper_Deactivate(object sender, GTDeactivateEventArgs e)
        {
            GTClassFactory.Create<IGTApplication>().SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Deactivate.");
        }

        void m_oIGTCustomCommandHelper_Activate(object sender, GTActivateEventArgs e)
        {
            GTClassFactory.Create<IGTApplication>().SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Activate.");
            mobjEditService = GTClassFactory.Create<IGTGeometryEditService>();
            mobjEditServicePoly = GTClassFactory.Create<IGTGeometryEditService>();
            mobjEditService.TargetMapWindow = m_gtapp.ActiveMapWindow;
            mobjEditServicePoly.TargetMapWindow = m_gtapp.ActiveMapWindow;
        }

        void m_oIGTCustomCommandHelper_KeyPress(object sender, GTKeyPressEventArgs e)
        {
            GTClassFactory.Create<IGTApplication>().SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "KeyPress.");
        }

        void m_oIGTCustomCommandHelper_KeyDown(object sender, GTKeyEventArgs e)
        {
            GTClassFactory.Create<IGTApplication>().SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "KeyDown.");
        }
        
        #endregion

        #region IGTCustomCommandModeless Members

        public void Activate(Intergraph.GTechnology.API.IGTCustomCommandHelper CustomCommandHelper)
        {
            GTClassFactory.Create<IGTApplication>().SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Running .NET custom modeless command...");            
            m_oIGTCustomCommandHelper = CustomCommandHelper;

            ActivateExplorer(); // added for feature explorer - 25-08-2012
            SubscribeEvents();
            m_CustomForm = new frmTail();
            m_CustomForm.FormClosed += new FormClosedEventHandler(m_CustomForm_FormClosed);            
            m_CustomForm.Show();
        }
                
        void m_CustomForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (mobjEditService != null)
            {
                v_PointLine.Clear();
                oTempCableLine.Points.Clear();
                oTempCablePoly.Points.Clear();
                mobjEditService.RemoveAllGeometries();
                mobjEditService = null;
                mobjEditServicePoly.RemoveAllGeometries();
                mobjEditServicePoly = null;
                System.Windows.Forms.Application.DoEvents();
            }
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

        public Intergraph.GTechnology.API.IGTTransactionManager TransactionManager
        {
            set
            {
                m_oIGTTransactionManager = value;
            }
        }

        public void SubscribeEvents()
        {
            // Subscribe to m_oIIGTCustomCommandHelper events using C# 1.0 syntax
            m_oIGTCustomCommandHelper.Activate += new EventHandler<GTActivateEventArgs>(m_oIGTCustomCommandHelper_Activate);
            m_oIGTCustomCommandHelper.Deactivate += new EventHandler<GTDeactivateEventArgs>(m_oIGTCustomCommandHelper_Deactivate);
            m_oIGTCustomCommandHelper.GainedFocus += new EventHandler<GTGainedFocusEventArgs>(m_oIGTCustomCommandHelper_GainedFocus);
            m_oIGTCustomCommandHelper.LostFocus += new EventHandler<GTLostFocusEventArgs>(m_oIGTCustomCommandHelper_LostFocus);
            m_oIGTCustomCommandHelper.KeyUp += new EventHandler<GTKeyEventArgs>(m_oIGTCustomCommandHelper_KeyUp);
            m_oIGTCustomCommandHelper.KeyDown += new EventHandler<GTKeyEventArgs>(m_oIGTCustomCommandHelper_KeyDown);
            m_oIGTCustomCommandHelper.KeyPress += new EventHandler<GTKeyPressEventArgs>(m_oIGTCustomCommandHelper_KeyPress);
            m_oIGTCustomCommandHelper.Click += new EventHandler<GTMouseEventArgs>(m_oIGTCustomCommandHelper_Click);
            m_oIGTCustomCommandHelper.DblClick += new EventHandler<GTMouseEventArgs>(m_oIGTCustomCommandHelper_DblClick);
            m_oIGTCustomCommandHelper.MouseMove += new EventHandler<GTMouseEventArgs>(m_oIGTCustomCommandHelper_MouseMove);
            m_oIGTCustomCommandHelper.MouseDown += new EventHandler<GTMouseEventArgs>(m_oIGTCustomCommandHelper_MouseDown);
            m_oIGTCustomCommandHelper.MouseUp += new EventHandler<GTMouseEventArgs>(m_oIGTCustomCommandHelper_MouseUp);
            m_oIGTCustomCommandHelper.WheelRotate += new EventHandler<GTWheelRotateEventArgs>(m_oIGTCustomCommandHelper_WheelRotate);
        }

        private void UnsubscribeEvents()
        {
            // UnSubscribe to m_oIIGTCustomCommandHelper events using C# 1.0 syntax
            m_oIGTCustomCommandHelper.Activate -= m_oIGTCustomCommandHelper_Activate;
            m_oIGTCustomCommandHelper.Deactivate -= m_oIGTCustomCommandHelper_Deactivate;
            m_oIGTCustomCommandHelper.GainedFocus -= m_oIGTCustomCommandHelper_GainedFocus;
            m_oIGTCustomCommandHelper.LostFocus -= m_oIGTCustomCommandHelper_LostFocus;
            m_oIGTCustomCommandHelper.KeyUp -= m_oIGTCustomCommandHelper_KeyUp;
            m_oIGTCustomCommandHelper.KeyDown -= m_oIGTCustomCommandHelper_KeyDown;
            m_oIGTCustomCommandHelper.KeyPress -= m_oIGTCustomCommandHelper_KeyPress;
            m_oIGTCustomCommandHelper.Click -= m_oIGTCustomCommandHelper_Click;
            m_oIGTCustomCommandHelper.DblClick -= m_oIGTCustomCommandHelper_DblClick;
            m_oIGTCustomCommandHelper.MouseMove -= m_oIGTCustomCommandHelper_MouseMove;
            m_oIGTCustomCommandHelper.MouseDown -= m_oIGTCustomCommandHelper_MouseDown;
            m_oIGTCustomCommandHelper.MouseUp -= m_oIGTCustomCommandHelper_MouseUp;
            m_oIGTCustomCommandHelper.WheelRotate -= m_oIGTCustomCommandHelper_WheelRotate;
        }

        public void ExitCmd()
        {
            GTClassFactory.Create<IGTApplication>().SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Tail Management  Exiting...");
            UnsubscribeEvents();
            CleanUpExplorer(); 
            m_oIGTCustomCommandHelper.Complete();
            GTClassFactory.Create<IGTApplication>().SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Tail Management Exited.");
        }

        #endregion

        #region Feature Explorer

        private static IGTFeatureExplorerService featureExplorerService = null;
        private EventHandler EV_Explorer_SaveClick;
        private EventHandler EV_Explorer_CancelClick;

        private void ActivateExplorer()
        {
            // feature explorer events
            featureExplorerService = GTClassFactory.Create<IGTFeatureExplorerService>();
            EV_Explorer_CancelClick = new EventHandler(Explorer_CancelClick);
            EV_Explorer_SaveClick = new EventHandler(Explorer_SaveClick);
            featureExplorerService.SaveClick += EV_Explorer_SaveClick;
            featureExplorerService.CancelClick += EV_Explorer_CancelClick;

        }

        public static void StartFeatureExplorer(IGTKeyObject newFeature)
        {
            featureExplorerService.ExploreFeature(newFeature, "Edit"); // Replace or Placement
            featureExplorerService.Visible = true;
            featureExplorerService.Slide(true);
        }

        void Explorer_CancelClick(object sender, EventArgs e)
        {
            OnFinishedExplorer(true);
            m_CustomForm.WindowState = FormWindowState.Normal;
            m_CustomForm.Close();
        }

        void Explorer_SaveClick(object sender, EventArgs e)
        {
            OnFinishedExplorer(false);
            m_CustomForm.WindowState = FormWindowState.Normal;
            m_CustomForm.Close();
        }

        private void OnFinishedExplorer(bool canceled)
        {
            AllPlacementFinished(canceled);
            featureExplorerService.Slide(false);
        }

        protected void AllPlacementFinished(bool canceled)
        {
            if (canceled)
            {
                m_oIGTTransactionManager.Rollback();
            }
            else
            {
                m_oIGTTransactionManager.Commit(true);
            }
            m_oIGTTransactionManager.RefreshDatabaseChanges();
        }

        private void CleanUpExplorer()
        {
            featureExplorerService.SaveClick -= EV_Explorer_SaveClick;
            featureExplorerService.CancelClick -= EV_Explorer_CancelClick;
        }

        #endregion
    }
}
