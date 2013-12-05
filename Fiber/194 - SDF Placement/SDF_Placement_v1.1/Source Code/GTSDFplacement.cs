using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Intergraph.GTechnology.Interfaces;
using Intergraph.GTechnology.API;
using ADODB;

namespace NEPS.GTechnology.SDFplacement
{
    class GTSDFplacement : Intergraph.GTechnology.Interfaces.IGTCustomCommandModeless
    {
        #region variables
        public static Intergraph.GTechnology.API.IGTTransactionManager m_oIGTTransactionManager = null;
        public static Intergraph.GTechnology.API.IGTCustomCommandHelper m_oIGTCustomCommandHelper = null;
        public static Intergraph.GTechnology.API.IGTApplication application = null;
        public static Intergraph.GTechnology.API.IGTGeometryEditService mobjEditService = null;


        SDF_Plc_Form m_CustomForm = null;
        bool closestatus = false;

        IGTLocateService mobjLocateService = null;
        //IGTKeyObject mobjAttribute = null;
        #endregion

        #region Event Handlers
        void m_oIGTCustomCommandHelper_Click(object sender, GTMouseEventArgs e)
        {
            try
            {
               
            }
            catch (Exception e1)
            {
                throw e1;
            }
        }
        void m_oIGTCustomCommandHelper_MouseMove(object sender, GTMouseEventArgs e)
        {
            if (m_CustomForm.PlaceValue == 100)
            {
                application.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Point to select VDSL2. Right click to cancel selection");

            }
        }

       

        void m_oIGTCustomCommandHelper_DblClick(object sender, GTMouseEventArgs e)
        {
            
        }

        void m_oIGTCustomCommandHelper_MouseUp(object sender, GTMouseEventArgs e)
        {
            #region left click
            if (e.Button == 1)
            {    

                #region select parent device
                if (m_CustomForm.PlaceValue == 100)
                {
                    IGTDDCKeyObjects feat = mobjLocateService.Locate(e.WorldPoint, 20, 1, GTSelectionTypeConstants.gtmwstSelectSingle);
                    for (int K = 0; K < feat.Count; K++)
                        application.SelectedObjects.Add(GTSelectModeConstants.gtsosmAllDisplayedComponentsInActiveLegend, feat[K]);

                    if (application.SelectedObjects.FeatureCount == 1)
                    {
                        m_CustomForm.PickVDSL2();
                    }
                    return;
                }
                #endregion
              
            }
            #endregion

            #region right click
            else
            {
                if (m_CustomForm.PlaceValue == 100 || m_CustomForm.PlaceValue == 1 || m_CustomForm.PlaceValue == 200)
                {
                    application.SelectedObjects.Clear();
                    //if (mobjEditServicePoint.GeometryCount > 0)
                    //    mobjEditServicePoint.RemoveAllGeometries();
                    //if (mobjEditServiceText.GeometryCount > 0)
                    //    mobjEditServiceText.RemoveAllGeometries();
                    //if (mobjEditServiceTemp.GeometryCount > 0)
                    //    mobjEditServiceTemp.RemoveAllGeometries();
                    m_CustomForm.Show();
                    m_CustomForm.PlaceValue = 0;
                    application.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "");
                    return;
                }
            }
            #endregion

        }
        
        #endregion

        #region Not used event

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
            GTClassFactory.Create<IGTApplication>().SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "KeyUp.");            
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
            GTClassFactory.Create<IGTApplication>().SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "KeyDown.");
        }

        #endregion

        #region IGTCustomCommandModeless Members
      
        public void Activate(Intergraph.GTechnology.API.IGTCustomCommandHelper CustomCommandHelper)
        {
            application = GTClassFactory.Create<IGTApplication>();
            application.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Running SDF Placement...");            
            m_oIGTCustomCommandHelper = CustomCommandHelper;
            
            mobjEditService = GTClassFactory.Create<IGTGeometryEditService>();
            mobjEditService.TargetMapWindow = application.ActiveMapWindow;

            mobjLocateService = application.ActiveMapWindow.LocateService;

            SubscribeEvents();
            m_CustomForm = new SDF_Plc_Form();
            m_CustomForm.FormClosed += new FormClosedEventHandler(m_CustomForm_FormClosed);  

            if (application.SelectedObjects.FeatureCount == 1)
            {
                m_CustomForm.PickVDSL2();
                m_CustomForm.Show();
            }
            else  m_CustomForm.Show();
        }

        public Intergraph.GTechnology.API.IGTTransactionManager TransactionManager
        {
            set
            {
                m_oIGTTransactionManager = value;
            }
        }


        #endregion 

        #region Subscr/Unsubs Event
        public void SubscribeEvents()
        {
            // Subscribe to m_oIIGTCustomCommandHelper events using C# 1.0 syntax
            // m_oIGTCustomCommandHelper.Activate += new EventHandler<GTActivateEventArgs>(m_oIGTCustomCommandHelper_Activate);
            // m_oIGTCustomCommandHelper.Deactivate += new EventHandler<GTDeactivateEventArgs>(m_oIGTCustomCommandHelper_Deactivate);
            // m_oIGTCustomCommandHelper.GainedFocus += new EventHandler<GTGainedFocusEventArgs>(m_oIGTCustomCommandHelper_GainedFocus);
            // m_oIGTCustomCommandHelper.LostFocus += new EventHandler<GTLostFocusEventArgs>(m_oIGTCustomCommandHelper_LostFocus);
            // m_oIGTCustomCommandHelper.KeyUp += new EventHandler<GTKeyEventArgs>(m_oIGTCustomCommandHelper_KeyUp);
            // m_oIGTCustomCommandHelper.KeyDown += new EventHandler<GTKeyEventArgs>(m_oIGTCustomCommandHelper_KeyDown);
            // m_oIGTCustomCommandHelper.KeyPress += new EventHandler<GTKeyPressEventArgs>(m_oIGTCustomCommandHelper_KeyPress);
            m_oIGTCustomCommandHelper.Click += new EventHandler<GTMouseEventArgs>(m_oIGTCustomCommandHelper_Click);
            m_oIGTCustomCommandHelper.DblClick += new EventHandler<GTMouseEventArgs>(m_oIGTCustomCommandHelper_DblClick);
            m_oIGTCustomCommandHelper.MouseMove += new EventHandler<GTMouseEventArgs>(m_oIGTCustomCommandHelper_MouseMove);
            //  m_oIGTCustomCommandHelper.MouseDown += new EventHandler<GTMouseEventArgs>(m_oIGTCustomCommandHelper_MouseDown);
            m_oIGTCustomCommandHelper.MouseUp += new EventHandler<GTMouseEventArgs>(m_oIGTCustomCommandHelper_MouseUp);
            //  m_oIGTCustomCommandHelper.WheelRotate += new EventHandler<GTWheelRotateEventArgs>(m_oIGTCustomCommandHelper_WheelRotate);
        }

        private void UnsubscribeEvents()
        {
            // UnSubscribe to m_oIIGTCustomCommandHelper events using C# 1.0 syntax
            //  m_oIGTCustomCommandHelper.Activate -= m_oIGTCustomCommandHelper_Activate;
            //  m_oIGTCustomCommandHelper.Deactivate -= m_oIGTCustomCommandHelper_Deactivate;
            //  m_oIGTCustomCommandHelper.GainedFocus -= m_oIGTCustomCommandHelper_GainedFocus;
            // m_oIGTCustomCommandHelper.LostFocus -= m_oIGTCustomCommandHelper_LostFocus;
            // m_oIGTCustomCommandHelper.KeyUp -= m_oIGTCustomCommandHelper_KeyUp;
            // m_oIGTCustomCommandHelper.KeyDown -= m_oIGTCustomCommandHelper_KeyDown;
            // m_oIGTCustomCommandHelper.KeyPress -= m_oIGTCustomCommandHelper_KeyPress;
            m_oIGTCustomCommandHelper.Click -= m_oIGTCustomCommandHelper_Click;
            m_oIGTCustomCommandHelper.DblClick -= m_oIGTCustomCommandHelper_DblClick;
            m_oIGTCustomCommandHelper.MouseMove -= m_oIGTCustomCommandHelper_MouseMove;
            //  m_oIGTCustomCommandHelper.MouseDown -= m_oIGTCustomCommandHelper_MouseDown;
            m_oIGTCustomCommandHelper.MouseUp -= m_oIGTCustomCommandHelper_MouseUp;
            //   m_oIGTCustomCommandHelper.WheelRotate -= m_oIGTCustomCommandHelper_WheelRotate;
        }
        #endregion

        #region closed CmdForm
        void m_CustomForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (!closestatus)
                ExitCmd();
        }
        #endregion

        #region Termination
        public bool CanTerminate
        {
            get
            {
                DialogResult retVal = MessageBox.Show("Do you want to discard your current changes and exit?", "SDF Placement", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (retVal == DialogResult.Yes)
                {
                    ExitCmd();
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


            }
            catch (Exception e)
            {
                throw e;
            }
        }

        

        #endregion              

        #region Exit CustomCmd
        public void ExitCmd()
        {
            application.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, " Exiting...");

            if (m_oIGTTransactionManager != null)
            {
                if (m_oIGTTransactionManager.TransactionInProgress)
                    m_oIGTTransactionManager.Rollback();
                m_oIGTTransactionManager = null;
            }

            if (application != null)
            {
                application = null;
            }

            if (mobjEditService != null)
            {

                if (mobjEditService.GeometryCount > 0)
                    mobjEditService.RemoveAllGeometries();
                mobjEditService = null;
            }

            if (m_CustomForm != null)
            {
                closestatus = true;
                m_CustomForm.Close();
                m_CustomForm = null;
            }

            mobjLocateService = null;
            
            UnsubscribeEvents();
            m_oIGTCustomCommandHelper.Complete();

        }

        #endregion

    }
}
