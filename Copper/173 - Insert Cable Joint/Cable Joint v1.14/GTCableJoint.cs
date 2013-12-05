using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

using System.Drawing;
using System.Runtime.InteropServices;

using Intergraph.GTechnology.Interfaces;
using Intergraph.GTechnology.API;
using ADODB;

namespace NEPS.GTechnology.Cable_Joint
{
    class GTCable_Joint : Intergraph.GTechnology.Interfaces.IGTCustomCommandModeless
    {

        #region Cable Joint : Variables declaration
        //public IGTPoint selGeom = null;
        public static Intergraph.GTechnology.API.IGTApplication m_gtapp = GTClassFactory.Create<IGTApplication>();
        public static Intergraph.GTechnology.API.IGTTransactionManager m_oIGTTransactionManager = null;
        public static Intergraph.GTechnology.API.IGTCustomCommandHelper m_oIGTCustomCommandHelper = null;
        public static Intergraph.GTechnology.API.IGTGeometryEditService mobjEditService = null;
        private bool addmode;
        #endregion

        #region Cable Joint : Event Handlers
        bool isClick = false;
        void m_oIGTCustomCommandHelper_Click(object sender, GTMouseEventArgs e)
        {
            try
            {
                if (addmode) return;
                InsertJoint(e);
            }
            catch  (Exception ex)
            {
                MessageBox.Show("Error placement\r\n" + ex.Message );
            }
        }

        void InsertJoint(GTMouseEventArgs e)
        {            
            if (clsCableJoint.LineSelected(m_gtapp.SelectedObjects))
            {
                isClick = false;

                if (MessageBox.Show("Click OK to confirm insert new joint. \r\nClick Cancel to cancel.",
                    "Insert Joint", MessageBoxButtons.OKCancel) == DialogResult.OK)
                {
                    addmode = true;
                    clsCableJoint.v_G3E_DETAILID = e.MapWindow.DetailID;

                    clsCableJoint.ReadJoints();

                    // get point of joint - snap point to the line
                    IGTPoint point = clsSnap.SnapPointer(clsCableJoint.m_SelectedLine.Geometry, m_gtapp.ActiveMapWindow.hWnd, e.WorldPoint);
                    // add joint to cable
                    m_oIGTCustomCommandHelper.MouseMove -= m_oIGTCustomCommandHelper_MouseMove;
                    DrawTempJoint(point);
                    Application.DoEvents();
                    Cursor.Current = Cursors.WaitCursor;

                    if (clsCableJoint.AddingJoint(m_gtapp, point))
                    {
                        ExitCmd();
                    }
                    else
                    {
                        AllPlacementFinished(true);
                        ExitCmd();
                    }

                }
            }

        }
        
        void m_oIGTCustomCommandHelper_MouseMove(object sender, GTMouseEventArgs e)
        {
            try
            {
                if (addmode)
                {
                    m_gtapp.BeginWaitCursor();
                    return;
                    //GTClassFactory.Create<IGTApplication>().Application.ActiveMapWindow.MousePointer = GTMousePointerConstants.
                }
                if (isClick)
                {
                    isClick = false;
                    InsertJoint(e);
                }
                else
                {

                    if ((GTClassFactory.Create<IGTApplication>().Application.ActiveMapWindow.MousePointer == GTMousePointerConstants.gtmwmpZoomIn) ||
                       (GTClassFactory.Create<IGTApplication>().Application.ActiveMapWindow.MousePointer == GTMousePointerConstants.gtmwmpZoomOut) ||
                       (GTClassFactory.Create<IGTApplication>().Application.ActiveMapWindow.MousePointer == GTMousePointerConstants.gtmwmpPan) ||
                       (GTClassFactory.Create<IGTApplication>().Application.ActiveMapWindow.MousePointer == GTMousePointerConstants.gtmwmpRotate))
                    {
                        if (mobjEditService.GeometryCount > 0)
                            mobjEditService.RemoveAllGeometries(); // clear the pointer 
                        return;
                    }
                    else
                    {
                        GTClassFactory.Create<IGTApplication>().Application.ActiveMapWindow.MousePointer = GTMousePointerConstants.gtmwmpCrossHair;
                    }

                    DrawTempJoint(e.WorldPoint);
                    GTClassFactory.Create<IGTApplication>().SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Click to insert joint.  ESC to exit custom modeless command.");
                }
            }
            catch (Exception ex)
            {
                //throw ex;
            }
        }

        private void DrawTempJoint(IGTPoint point)
        {
            //GTClassFactory.Create<IGTApplication>().SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Move mouse to the indicate location of new joint. Click to confirm.");

            IGTPointGeometry oPointDPGeom;
            oPointDPGeom = GTClassFactory.Create<IGTPointGeometry>();
            IGTPoint oPntGeom = GTClassFactory.Create<IGTPoint>();
            oPntGeom.X = point.X;
            oPntGeom.Y = point.Y;

            int isStyleID = 10820001;
            if (mobjEditService.GeometryCount > 0)
                mobjEditService.RemoveAllGeometries();

            oPointDPGeom.Origin = oPntGeom;
            // draw the temp joint symbol
            mobjEditService.AddGeometry(oPointDPGeom, isStyleID);
            Application.DoEvents();

        }
        
        void m_oIGTCustomCommandHelper_DblClick(object sender, GTMouseEventArgs e)
        {
            try
            {
            }
            catch (Exception ex)
            {
            }

        }

        void m_oIGTCustomCommandHelper_WheelRotate(object sender, GTWheelRotateEventArgs e)
        {
//            GTClassFactory.Create<IGTApplication>().SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "WheelRotate.");
        }

        void m_oIGTCustomCommandHelper_MouseUp(object sender, GTMouseEventArgs e)
        {
//            GTClassFactory.Create<IGTApplication>().SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "MouseUp.");
        }

        void m_oIGTCustomCommandHelper_MouseDown(object sender, GTMouseEventArgs e)
        {
            if (clsCableJoint.LineSelected(m_gtapp.SelectedObjects))
                m_oIGTCustomCommandHelper_Click(sender, e);

        }

        void m_oIGTCustomCommandHelper_KeyUp(object sender, GTKeyEventArgs e)
        {
            if (e.KeyCode == 27 ) // ESC key - cancel add Boundary
            {
                GTClassFactory.Create<IGTApplication>().SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "User pressed ESC button.");
                AllPlacementFinished(true);
                ExitCmd();
            }
        }

        void m_oIGTCustomCommandHelper_KeyPress(object sender, GTKeyPressEventArgs e)
        {
//            GTClassFactory.Create<IGTApplication>().SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "KeyPress.");
        }

        void m_oIGTCustomCommandHelper_KeyDown(object sender, GTKeyEventArgs e)
        {
            if (e.KeyCode == 27) // ESC key - cancel add Boundary
            {
                GTClassFactory.Create<IGTApplication>().SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "User pressed ESC button.");
                AllPlacementFinished(true);
                ExitCmd();
            }

        }

        #endregion

        #region Cable Joint : IGTCustomCommandModeless Members
        public void Activate(Intergraph.GTechnology.API.IGTCustomCommandHelper CustomCommandHelper)
        {
            GTClassFactory.Create<IGTApplication>().SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Running .NET custom modeless command...");
            m_oIGTCustomCommandHelper = CustomCommandHelper;
            
            m_gtapp.SelectedObjects.Clear();

            mobjEditService = GTClassFactory.Create<IGTGeometryEditService>();
            mobjEditService.TargetMapWindow = m_gtapp.ActiveMapWindow;

            ActivateExplorer();
            SubscribeEvents();

           // m_oIGTTransactionManager.Begin("Feature Placement");
        }

        public void DeActivate()
        {
            if (mobjEditService != null)
            {
                mobjEditService.RemoveAllGeometries();
                mobjEditService = null;
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
            //m_oIGTCustomCommandHelper.Activate += new EventHandler<GTActivateEventArgs>(m_oIGTCustomCommandHelper_Activate);
            //m_oIGTCustomCommandHelper.Deactivate += new EventHandler<GTDeactivateEventArgs>(m_oIGTCustomCommandHelper_Deactivate);
            //m_oIGTCustomCommandHelper.GainedFocus += new EventHandler<GTGainedFocusEventArgs>(m_oIGTCustomCommandHelper_GainedFocus);
            //m_oIGTCustomCommandHelper.LostFocus += new EventHandler<GTLostFocusEventArgs>(m_oIGTCustomCommandHelper_LostFocus);
            m_oIGTCustomCommandHelper.KeyUp += new EventHandler<GTKeyEventArgs>(m_oIGTCustomCommandHelper_KeyUp);
            //m_oIGTCustomCommandHelper.KeyDown += new EventHandler<GTKeyEventArgs>(m_oIGTCustomCommandHelper_KeyDown);
            //m_oIGTCustomCommandHelper.KeyPress += new EventHandler<GTKeyPressEventArgs>(m_oIGTCustomCommandHelper_KeyPress);
            m_oIGTCustomCommandHelper.Click += new EventHandler<GTMouseEventArgs>(m_oIGTCustomCommandHelper_Click);
            m_oIGTCustomCommandHelper.DblClick += new EventHandler<GTMouseEventArgs>(m_oIGTCustomCommandHelper_DblClick);
            m_oIGTCustomCommandHelper.MouseMove += new EventHandler<GTMouseEventArgs>(m_oIGTCustomCommandHelper_MouseMove);
            m_oIGTCustomCommandHelper.MouseDown += new EventHandler<GTMouseEventArgs>(m_oIGTCustomCommandHelper_MouseDown);
            m_oIGTCustomCommandHelper.MouseUp += new EventHandler<GTMouseEventArgs>(m_oIGTCustomCommandHelper_MouseUp);
            //m_oIGTCustomCommandHelper.WheelRotate += new EventHandler<GTWheelRotateEventArgs>(m_oIGTCustomCommandHelper_WheelRotate);
        }
        
        private void UnsubscribeEvents()
        {
            // UnSubscribe to m_oIIGTCustomCommandHelper events using C# 1.0 syntax
            //m_oIGTCustomCommandHelper.Activate -= m_oIGTCustomCommandHelper_Activate;
            //m_oIGTCustomCommandHelper.Deactivate -= m_oIGTCustomCommandHelper_Deactivate;
            //m_oIGTCustomCommandHelper.GainedFocus -= m_oIGTCustomCommandHelper_GainedFocus;
            //m_oIGTCustomCommandHelper.LostFocus -= m_oIGTCustomCommandHelper_LostFocus;
            m_oIGTCustomCommandHelper.KeyUp -= m_oIGTCustomCommandHelper_KeyUp;
            //m_oIGTCustomCommandHelper.KeyDown -= m_oIGTCustomCommandHelper_KeyDown;
            //m_oIGTCustomCommandHelper.KeyPress -= m_oIGTCustomCommandHelper_KeyPress;
            m_oIGTCustomCommandHelper.Click -= m_oIGTCustomCommandHelper_Click;
            m_oIGTCustomCommandHelper.DblClick -= m_oIGTCustomCommandHelper_DblClick;
            m_oIGTCustomCommandHelper.MouseMove -= m_oIGTCustomCommandHelper_MouseMove;
            m_oIGTCustomCommandHelper.MouseDown -= m_oIGTCustomCommandHelper_MouseDown;
            m_oIGTCustomCommandHelper.MouseUp -= m_oIGTCustomCommandHelper_MouseUp;
            //m_oIGTCustomCommandHelper.WheelRotate -= m_oIGTCustomCommandHelper_WheelRotate;
        }

        public void ExitCmd()
        {
            GTClassFactory.Create<IGTApplication>().SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Exiting...");
            if (mobjEditService.GeometryCount > 0)
                mobjEditService.RemoveAllGeometries();

            UnsubscribeEvents();
            CleanUpExplorer();
            m_oIGTCustomCommandHelper.Complete();
            GTClassFactory.Create<IGTApplication>().SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Exited.");
            addmode = false;
            Cursor.Current = Cursors.Default;
        }
        #endregion

   
 
        #region Feature Explorer

        private IGTFeatureExplorerService featureExplorerService = null;
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

        public void StartFeatureExplorer(IGTKeyObject newFeature)
        {
            m_gtapp.BeginWaitCursor();
            featureExplorerService.ExploreFeature(newFeature, "Edit"); // Replace or Placement
            featureExplorerService.Visible = true;
            featureExplorerService.Slide(true);
            m_gtapp.EndWaitCursor();
        }

        void Explorer_CancelClick(object sender, EventArgs e)
        {
            OnFinishedExplorer(true);
            ExitCmd();
        }

        void Explorer_SaveClick(object sender, EventArgs e)
        {
            OnFinishedExplorer(false);
            ExitCmd();
        }

        private void OnFinishedExplorer(bool canceled)
        {
            AllPlacementFinished(canceled);

            featureExplorerService.Slide(false);
            featureExplorerService.Visible = false;          
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
