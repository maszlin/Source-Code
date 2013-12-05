using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Intergraph.GTechnology.Interfaces;
using Intergraph.GTechnology.API;
using ADODB;

namespace NEPS.GTechnology.MSANConfig
{
    class GTMSAN : Intergraph.GTechnology.Interfaces.IGTCustomCommandModeless
    {
        #region variables member
        public static Intergraph.GTechnology.API.IGTTransactionManager m_oIGTTransactionManager = null;
        public static Intergraph.GTechnology.API.IGTCustomCommandHelper m_oIGTCustomCommandHelper = null;
        private static Intergraph.GTechnology.API.IGTApplication application = GTClassFactory.Create<IGTApplication>();
        GTMSANForm m_CustomForm = null;
        public static Intergraph.GTechnology.API.IGTGeometryEditService mobjEditService = null;
        #endregion      

        #region Event Handlers

        public Boolean objSelected = false;
        void m_oIGTCustomCommandHelper_Click(object sender, GTMouseEventArgs e)
        {
        //
            if (GTClassFactory.Create<IGTApplication>().SelectedObjects.FeatureCount == 1)
            {
                m_CustomForm._mouseDown = objSelected;
            }
        }

        void m_oIGTCustomCommandHelper_MouseMove(object sender, GTMouseEventArgs e)
        {
            //
        }
             
        void m_oIGTCustomCommandHelper_DblClick(object sender, GTMouseEventArgs e)
        {
            //
        }
        void m_oIGTCustomCommandHelper_WheelRotate(object sender, GTWheelRotateEventArgs e)
        {
            //GTClassFactory.Create<IGTApplication>().SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "WheelRotate.");
        }

        void m_oIGTCustomCommandHelper_MouseUp(object sender, GTMouseEventArgs e)
        {
            //GTClassFactory.Create<IGTApplication>().SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "MouseUp.");            
        }

        void m_oIGTCustomCommandHelper_MouseDown(object sender, GTMouseEventArgs e)
        {
          //  GTClassFactory.Create<IGTApplication>().SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "MouseDown.");            
        }

        void m_oIGTCustomCommandHelper_LostFocus(object sender, GTLostFocusEventArgs e)
        {
          //  GTClassFactory.Create<IGTApplication>().SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "LostFocus.");
        }

        void m_oIGTCustomCommandHelper_KeyUp(object sender, GTKeyEventArgs e)
        {
          //  GTClassFactory.Create<IGTApplication>().SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "KeyUp.");            
        }

        void m_oIGTCustomCommandHelper_GainedFocus(object sender, GTGainedFocusEventArgs e)
        {
          //  GTClassFactory.Create<IGTApplication>().SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "GainedFocus.");
        }

        void m_oIGTCustomCommandHelper_Deactivate(object sender, GTDeactivateEventArgs e)
        {
          //  GTClassFactory.Create<IGTApplication>().SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Deactivate.");
        }

        void m_oIGTCustomCommandHelper_Activate(object sender, GTActivateEventArgs e)
        {
          //  GTClassFactory.Create<IGTApplication>().SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Activate.");
        }

        void m_oIGTCustomCommandHelper_KeyPress(object sender, GTKeyPressEventArgs e)
        {
           // GTClassFactory.Create<IGTApplication>().SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "KeyPress.");
        }

        void m_oIGTCustomCommandHelper_KeyDown(object sender, GTKeyEventArgs e)
        {
          // GTClassFactory.Create<IGTApplication>().SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "KeyDown.");
        }

        #endregion
       
        #region Members
      
        public void Activate(Intergraph.GTechnology.API.IGTCustomCommandHelper CustomCommandHelper)
        {
            application.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Running MSAN Config...");            
            m_oIGTCustomCommandHelper = CustomCommandHelper;
            
            mobjEditService = GTClassFactory.Create<IGTGeometryEditService>();
            mobjEditService.TargetMapWindow = application.ActiveMapWindow;
            SubscribeEvents();
            m_CustomForm = new GTMSANForm();
            m_CustomForm.FormClosed += new FormClosedEventHandler(m_CustomForm_FormClosed);
            if (GTMSANForm.appsmode != 1)
                m_CustomForm.Show();
            else
            {
                m_CustomForm = null; 
                ExitCmd();
            }
        }
                
        void m_CustomForm_FormClosed(object sender, FormClosedEventArgs e)
        {
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

        #endregion

        #region subscribe events for windows form
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
        #endregion

        #region close application
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
            GTClassFactory.Create<IGTApplication>().SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "DblClicked! Exiting...");
            if (mobjEditService != null)
            {
                mobjEditService.RemoveAllGeometries();
                mobjEditService = null;
            }                       
            UnsubscribeEvents();
            m_oIGTCustomCommandHelper.Complete();
            GTClassFactory.Create<IGTApplication>().SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "DblClicked! Exited.");

        }
        #endregion
    }
}
