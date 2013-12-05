using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Intergraph.GTechnology.Interfaces;
using Intergraph.GTechnology.API;
using ADODB;

namespace NEPS.GTechnology.PUTrigger
{
    class GTPUTrigger : Intergraph.GTechnology.Interfaces.IGTCustomCommandModeless
    {
       #region variables
        public static Intergraph.GTechnology.API.IGTApplication application = null;
        public static Intergraph.GTechnology.API.IGTTransactionManager m_oGTTransactionManager = null;
        public static Intergraph.GTechnology.API.IGTCustomCommandHelper m_oGTCustomCommandHelper = null;

        public static IGTGeometryEditService mobjEditServiceTemp;
        public static IGTGeometryEditService mobjEditServicePoint;
        public static IGTGeometry oPointPoint = null;
        IGTLocateService mobjLocateService = null;
        bool closestatus = false;

        public IGTPoint SelGeom = null;
        public Boolean placePX = false;
        public Boolean chkFeature1 = false;
        public Boolean chkFeature2 = false;
        public Boolean cfmFeature1 = false;
        public Boolean cfmFeature2 = false;
        public Boolean addFeature = false;
        public Boolean rotatePX = false;
        public Boolean initRotatePX = false;
        public Boolean movePX = false;
        public Boolean initMovePX = false;
        public Boolean initDeletePX = false;
        frmPUTrigger m_CustomForm = null;
       #endregion

        #region Event Handlers

        #region Mouse Move
        void m_oIGTCustomCommandHelper_MouseMove(object sender, GTMouseEventArgs e)
        {
            try 
            {
                if (m_CustomForm != null)
                {
                    if (m_CustomForm.PlaceValue == 10)
                    {
                        application.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Point to select " + m_CustomForm.vFeature1+ "!Right click to cancel selection");

                    }
                    else
                        if (m_CustomForm.PlaceValue == 20)
                        {
                            application.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Point to select " + m_CustomForm.vFeature2 + "!Right click to cancel selection");

                        }
                    else
                        if (m_CustomForm.PlaceValue == 30)
                        {
                            application.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Point to place PU Trigger!Right click to cancel placement");
                            IGTPoint WorldPoint = e.WorldPoint;
                            IGTPointGeometry oPoint = PGeoLib.CreatePointGeom(WorldPoint.X, WorldPoint.Y);
                            if (mobjEditServiceTemp.GeometryCount > 0) mobjEditServiceTemp.RemoveAllGeometries();
                            mobjEditServiceTemp.AddGeometry(oPoint, m_CustomForm.StyleId);
                            oPointPoint = oPoint;
                        }
                }
            }
            catch (Exception e1)
            {
                throw e1;
            }
        }

        #endregion

        #region Mouse Up
        void m_oIGTCustomCommandHelper_MouseUp(object sender, GTMouseEventArgs e)
        {

            try
            {
                #region left click
                if (e.Button == 1)
                {
                  

                    #region select parent device1
                    if (m_CustomForm.PlaceValue == 10)
                    {
                        IGTDDCKeyObjects feat = mobjLocateService.Locate(e.WorldPoint, 20, 1, GTSelectionTypeConstants.gtmwstSelectSingle);
                        for (int K = 0; K < feat.Count; K++)
                            application.SelectedObjects.Add(GTSelectModeConstants.gtsosmAllDisplayedComponentsInActiveLegend, feat[K]);

                        if (application.SelectedObjects.FeatureCount == 1)
                        {
                            application.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "");
                            m_CustomForm.PickParent(1);
                        }
                        return;
                    }
                    #endregion

                    #region select parent device1
                    if (m_CustomForm.PlaceValue == 20)
                    {
                        IGTDDCKeyObjects feat = mobjLocateService.Locate(e.WorldPoint, 20, 1, GTSelectionTypeConstants.gtmwstSelectSingle);
                        for (int K = 0; K < feat.Count; K++)
                            application.SelectedObjects.Add(GTSelectModeConstants.gtsosmAllDisplayedComponentsInActiveLegend, feat[K]);

                        if (application.SelectedObjects.FeatureCount == 1)
                        {
                            application.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "");
                            m_CustomForm.PickParent(2);
                        }
                        return;
                    }
                    #endregion

                    #region place PU Trigger
                    if (m_CustomForm.PlaceValue == 30)
                    {
                        m_CustomForm.PlaceValue = 0;
                        if (mobjEditServicePoint.GeometryCount > 0) mobjEditServicePoint.RemoveAllGeometries();
                        mobjEditServicePoint.AddGeometry(oPointPoint, m_CustomForm.StyleId);

                        application.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Wait, database is updating...");
                        application.BeginWaitCursor();
                        m_CustomForm.CommitToDBnewPUTr(oPointPoint.FirstPoint);

                        if (mobjEditServicePoint.GeometryCount > 0)
                            mobjEditServicePoint.RemoveAllGeometries();
                        if (mobjEditServiceTemp.GeometryCount > 0)
                            mobjEditServiceTemp.RemoveAllGeometries();

                        application.EndWaitCursor();
                        m_CustomForm.Show();
                        application.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "");                            
                        
                        return;
                    }
                    #endregion
                   
                }
                #endregion

                #region right click
                else
                {
                    if (m_CustomForm.PlaceValue == 10 || m_CustomForm.PlaceValue == 20 )
                    {
                        application.SelectedObjects.Clear();
                        if (mobjEditServicePoint.GeometryCount > 0)
                            mobjEditServicePoint.RemoveAllGeometries();
                        if (mobjEditServiceTemp.GeometryCount > 0)
                            mobjEditServiceTemp.RemoveAllGeometries();
                        m_CustomForm.Show();
                        m_CustomForm.PlaceValue = 0;
                        application.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "");
                        return;
                    }

                    if (m_CustomForm.PlaceValue == 30)
                    {
                        
                        if (mobjEditServicePoint.GeometryCount > 0)
                            mobjEditServicePoint.RemoveAllGeometries();
                        if (mobjEditServiceTemp.GeometryCount > 0)
                            mobjEditServiceTemp.RemoveAllGeometries();
                        m_CustomForm.CancelPlacement();
                        m_CustomForm.Show();
                        m_CustomForm.PlaceValue = 0;
                        application.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "");
                        return;
                    }

                }
                #endregion 
            }
            catch (Exception e1)
            {
                if (mobjEditServicePoint.GeometryCount > 0) mobjEditServicePoint.RemoveAllGeometries();

                if (mobjEditServiceTemp.GeometryCount > 0)
                    mobjEditServiceTemp.RemoveAllGeometries();
                application.EndWaitCursor();
                m_CustomForm.Show();
                application.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, ""); 
            }
        }
        #endregion

        #region event not use
        void m_oIGTCustomCommandHelper_Click(object sender, GTMouseEventArgs e)
        {
        }

        void m_oIGTCustomCommandHelper_DblClick(object sender, GTMouseEventArgs e)
        {
            GTClassFactory.Create<IGTApplication>().SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "DblClick.");
        }

        void m_oIGTCustomCommandHelper_WheelRotate(object sender, GTWheelRotateEventArgs e)
        {
            GTClassFactory.Create<IGTApplication>().SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "WheelRotate.");
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
        }

        void m_oIGTCustomCommandHelper_KeyPress(object sender, GTKeyPressEventArgs e)
        {
            GTClassFactory.Create<IGTApplication>().SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "KeyPress.");
        }

        void m_oIGTCustomCommandHelper_KeyDown(object sender, GTKeyEventArgs e)
        {
           
        }
        #endregion
        #endregion

        #region IGTCustomCommandModeless Members

        public void Activate(Intergraph.GTechnology.API.IGTCustomCommandHelper CustomCommandHelper)
        {
            try
            {
                if (application == null) application = GTClassFactory.Create<IGTApplication>();
                application.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Running PU Trigger Placement....");

                m_oGTCustomCommandHelper = CustomCommandHelper;
                mobjEditServiceTemp = GTClassFactory.Create<IGTGeometryEditService>();
                mobjEditServiceTemp.TargetMapWindow = application.ActiveMapWindow;

                mobjEditServicePoint = GTClassFactory.Create<IGTGeometryEditService>();
                mobjEditServicePoint.TargetMapWindow = application.ActiveMapWindow;


                mobjLocateService = application.ActiveMapWindow.LocateService;
                SubscribeEvents();
                m_CustomForm = new frmPUTrigger();
                m_CustomForm.FormClosed += new FormClosedEventHandler(m_CustomForm_FormClosed);
              //  frmPUTrigger.gGeomCS = GTClassFactory.Create<IGTGeometryCreationService>();
                m_CustomForm.Show();                
            }
            catch (Exception ex) 
            { MessageBox.Show(ex.Message, "Error.", MessageBoxButtons.OK, MessageBoxIcon.Error); ExitCmd(); }

           
        }
                
        void m_CustomForm_FormClosed(object sender, FormClosedEventArgs e)
        {

            if (!closestatus)
                ExitCmd();
        }

        public bool CanTerminate
        {
            get
            {
                DialogResult retVal = MessageBox.Show("Do you want to exit?", "PU Trigger Placement", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (retVal == DialogResult.Yes)
                {
                    ExitCmd();
                }

                return false;

            }
        }

        public void Pause()
        {
            if (m_CustomForm.PlaceValue != 0)
                m_CustomForm.PlaceValue += 50000;
        }

        public void Resume()
        {
            if (m_CustomForm.PlaceValue != 0)
                m_CustomForm.PlaceValue -= 50000;
        }

        public void Terminate()
        {
            try
            {
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


        
        #endregion

        #region Subs/Unsubs Event
        public void SubscribeEvents()
        {
            // Subscribe to m_oIIGTCustomCommandHelper events using C# 1.0 syntax
            //m_oIGTCustomCommandHelper.Activate += new EventHandler<GTActivateEventArgs>(m_oIGTCustomCommandHelper_Activate);
            //m_oIGTCustomCommandHelper.Deactivate += new EventHandler<GTDeactivateEventArgs>(m_oIGTCustomCommandHelper_Deactivate);
            //m_oIGTCustomCommandHelper.GainedFocus += new EventHandler<GTGainedFocusEventArgs>(m_oIGTCustomCommandHelper_GainedFocus);
            //m_oIGTCustomCommandHelper.LostFocus += new EventHandler<GTLostFocusEventArgs>(m_oIGTCustomCommandHelper_LostFocus);
           // m_oIGTCustomCommandHelper.KeyUp += new EventHandler<GTKeyEventArgs>(m_oIGTCustomCommandHelper_KeyUp);
           // m_oIGTCustomCommandHelper.KeyDown += new EventHandler<GTKeyEventArgs>(m_oIGTCustomCommandHelper_KeyDown);
            //m_oIGTCustomCommandHelper.KeyPress += new EventHandler<GTKeyPressEventArgs>(m_oIGTCustomCommandHelper_KeyPress);
           // m_oIGTCustomCommandHelper.Click += new EventHandler<GTMouseEventArgs>(m_oIGTCustomCommandHelper_Click);
            //m_oIGTCustomCommandHelper.DblClick += new EventHandler<GTMouseEventArgs>(m_oIGTCustomCommandHelper_DblClick);
            m_oGTCustomCommandHelper.MouseMove += new EventHandler<GTMouseEventArgs>(m_oIGTCustomCommandHelper_MouseMove);
            //m_oIGTCustomCommandHelper.MouseDown += new EventHandler<GTMouseEventArgs>(m_oIGTCustomCommandHelper_MouseDown);
            m_oGTCustomCommandHelper.MouseUp += new EventHandler<GTMouseEventArgs>(m_oIGTCustomCommandHelper_MouseUp);
            //m_oIGTCustomCommandHelper.WheelRotate += new EventHandler<GTWheelRotateEventArgs>(m_oIGTCustomCommandHelper_WheelRotate);
        }

        private void UnsubscribeEvents()
        {
            // UnSubscribe to m_oIIGTCustomCommandHelper events using C# 1.0 syntax
            //m_oIGTCustomCommandHelper.Activate -= m_oIGTCustomCommandHelper_Activate;
            //m_oIGTCustomCommandHelper.Deactivate -= m_oIGTCustomCommandHelper_Deactivate;
            //m_oIGTCustomCommandHelper.GainedFocus -= m_oIGTCustomCommandHelper_GainedFocus;
            //m_oIGTCustomCommandHelper.LostFocus -= m_oIGTCustomCommandHelper_LostFocus;
            //m_oIGTCustomCommandHelper.KeyUp -= m_oIGTCustomCommandHelper_KeyUp;
            //m_oIGTCustomCommandHelper.KeyDown -= m_oIGTCustomCommandHelper_KeyDown;
            //m_oIGTCustomCommandHelper.KeyPress -= m_oIGTCustomCommandHelper_KeyPress;
            //m_oIGTCustomCommandHelper.Click -= m_oIGTCustomCommandHelper_Click;
            //m_oIGTCustomCommandHelper.DblClick -= m_oIGTCustomCommandHelper_DblClick;
            m_oGTCustomCommandHelper.MouseMove -= m_oIGTCustomCommandHelper_MouseMove;
            //m_oIGTCustomCommandHelper.MouseDown -= m_oIGTCustomCommandHelper_MouseDown;
            m_oGTCustomCommandHelper.MouseUp -= m_oIGTCustomCommandHelper_MouseUp;
            //m_oIGTCustomCommandHelper.WheelRotate -= m_oIGTCustomCommandHelper_WheelRotate;
        }
        #endregion

        #region exit custom command
        public void ExitCmd()
        {
            application.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, " Exiting PU Trigger Placement....");

            if (m_oGTTransactionManager != null)
            {
                if (m_oGTTransactionManager.TransactionInProgress)
                    m_oGTTransactionManager.Rollback();
                m_oGTTransactionManager = null;
            }
            if (oPointPoint != null)
            {
                oPointPoint = null;
            }
            if (application != null)
            {
                application = null;
            }
            if (mobjEditServiceTemp != null)
            {
                if (mobjEditServiceTemp.GeometryCount > 0)
                    mobjEditServiceTemp.RemoveAllGeometries();
                mobjEditServiceTemp = null;
            }
            if (mobjEditServicePoint != null)
            {
                if (mobjEditServicePoint.GeometryCount > 0)
                    mobjEditServicePoint.RemoveAllGeometries();
                mobjEditServicePoint = null;
            }
           
            
            if (m_CustomForm != null)
            {
                closestatus = true;
                m_CustomForm.Close();
                m_CustomForm = null;
            }
            mobjLocateService = null;
            UnsubscribeEvents();
            m_oGTCustomCommandHelper.Complete();
        }
        #endregion

    }
}
