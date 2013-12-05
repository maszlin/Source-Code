using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Intergraph.GTechnology.Interfaces;
using Intergraph.GTechnology.API;
using ADODB;

namespace NEPS.OSP.COPPER.CABLE_TRANSFER
{
    class GTTransfer : Intergraph.GTechnology.Interfaces.IGTCustomCommandModeless
    {
        public static IGTTransactionManager m_oIGTTransactionManager = null;
        public static IGTCustomCommandHelper m_oIGTCustomCommandHelper = null;


        public static Intergraph.GTechnology.API.IGTApplication m_gtapp = GTClassFactory.Create<IGTApplication>();

        public int isStep = 0;

        // for General
        public static frmTransfer m_CustomForm = null;


        // IGTPoint SelGeom = null;

        #region Event Handlers
        private bool isClick;
        void m_oIGTCustomCommandHelper_MouseMove(object sender, GTMouseEventArgs e)
        {
            if (m_CustomForm == null) return;
            if ((m_gtapp.ActiveMapWindow.MousePointer == GTMousePointerConstants.gtmwmpZoomIn) ||
               (m_gtapp.ActiveMapWindow.MousePointer == GTMousePointerConstants.gtmwmpZoomOut) ||
               (m_gtapp.ActiveMapWindow.MousePointer == GTMousePointerConstants.gtmwmpPan) ||
               (m_gtapp.ActiveMapWindow.MousePointer == GTMousePointerConstants.gtmwmpRotate))
            {
                return;
            }

            if (e.MapWindow.DetailID.ToString() == "0")
            {
                if (m_CustomForm != null)
                {
                    if (!m_CustomForm.Visible)
                    {
                        m_CustomForm.UserMouseMove(e.WorldPoint);
                    }
                }
            }
            else
            {
                GTClassFactory.Create<IGTApplication>().SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Cable transfer cannot be done in Detail Window");
            }
        }

        void m_oIGTCustomCommandHelper_DblClick(object sender, GTMouseEventArgs e)
        {
            //GTClassFactory.Create<IGTApplication>().SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "DblClick.");
            if (!m_CustomForm.Visible)
            {
                m_CustomForm.Visible = true;
                m_CustomForm.UserMouseClick(e.WorldPoint);
                m_CustomForm.UpdateTransferFeature();
            }
        }

        void m_oIGTCustomCommandHelper_WheelRotate(object sender, GTWheelRotateEventArgs e)
        {
            //GTClassFactory.Create<IGTApplication>().SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "WheelRotate.");
        }

        void m_oIGTCustomCommandHelper_MouseUp(object sender, GTMouseEventArgs e)
        {
            if (m_CustomForm == null) return;

            GTClassFactory.Create<IGTApplication>().SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "MouseUp.");
            Application.DoEvents();
            Application.DoEvents();
            try
            {

                if ((m_gtapp.ActiveMapWindow.MousePointer == GTMousePointerConstants.gtmwmpZoomIn) ||
                   (m_gtapp.ActiveMapWindow.MousePointer == GTMousePointerConstants.gtmwmpZoomOut) ||
                   (m_gtapp.ActiveMapWindow.MousePointer == GTMousePointerConstants.gtmwmpPan) ||
                   (m_gtapp.ActiveMapWindow.MousePointer == GTMousePointerConstants.gtmwmpRotate))
                {
                    return;
                }

                if (e.MapWindow.DetailID.ToString() == "0")
                {
                    if (!m_CustomForm.Visible)
                    {
                        m_CustomForm.UserMouseClick(e.WorldPoint);
                    }
                }
                else
                {
                    GTClassFactory.Create<IGTApplication>().SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Cable transfer cannot be done in Detail Window");
                }
            }
            catch (Exception ex)
            {
                m_gtapp.EndWaitCursor();
                m_CustomForm.ProgressMessage("Cable transfer cancel\r\nError : " + ex.Message);
                //m_CustomForm.ProsesSteps(-1);
            }
            // m_gtapp.SelectedObjects.Clear();
        }

        void m_oIGTCustomCommandHelper_MouseDown(object sender, GTMouseEventArgs e)
        {
            //GTClassFactory.Create<IGTApplication>().SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "MouseDown.");
        }

        void m_oIGTCustomCommandHelper_LostFocus(object sender, GTLostFocusEventArgs e)
        {
            //GTClassFactory.Create<IGTApplication>().SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "LostFocus.");
        }

        void m_oIGTCustomCommandHelper_KeyUp(object sender, GTKeyEventArgs e)
        {
            GTClassFactory.Create<IGTApplication>().SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "KeyUp.");
            if (e.KeyCode == 27) //ESC
            {
                if (MessageBox.Show("Do you want to cancel cable transfer", "Cable Transfer", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    ExitCmd();
                else
                    m_CustomForm.Visible = true;
            }
            else if (e.KeyCode == 8) // Backspace
            {
                m_CustomForm.Backspace();
            }
            else if (e.KeyCode == 13) // ENTER
            {
                m_CustomForm.UpdateTransferFeature();
            }
        }

        void m_oIGTCustomCommandHelper_GainedFocus(object sender, GTGainedFocusEventArgs e)
        {
            //GTClassFactory.Create<IGTApplication>().SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "GainedFocus.");
        }

        void m_oIGTCustomCommandHelper_Deactivate(object sender, GTDeactivateEventArgs e)
        {
            //GTClassFactory.Create<IGTApplication>().SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Deactivate.");
        }

        void m_oIGTCustomCommandHelper_Activate(object sender, GTActivateEventArgs e)
        {
            //GTClassFactory.Create<IGTApplication>().SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Activate.");
        }

        void m_oIGTCustomCommandHelper_KeyPress(object sender, GTKeyPressEventArgs e)
        {
            //GTClassFactory.Create<IGTApplication>().SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "KeyPress.");
        }

        void m_oIGTCustomCommandHelper_KeyDown(object sender, GTKeyEventArgs e)
        {
            //GTClassFactory.Create<IGTApplication>().SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "KeyDown.");
        }

        #endregion

        #region IGTCustomCommandModeless Members

        public void Activate(Intergraph.GTechnology.API.IGTCustomCommandHelper CustomCommandHelper)
        {
            GTClassFactory.Create<IGTApplication>().SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Running .NET custom command...");
            m_oIGTCustomCommandHelper = CustomCommandHelper;
            m_gtapp.SelectedObjects.Clear();

            SubscribeEvents();
            m_CustomForm = new frmTransfer();
            m_CustomForm.FormClosed += new FormClosedEventHandler(m_CustomForm_FormClosed);
            m_CustomForm.Show();

        }

        public void DeActivate()
        {
            m_CustomForm.CancelDrawing();
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
            catch (Exception ex)
            {
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
            m_oIGTCustomCommandHelper.KeyDown += new EventHandler<GTKeyEventArgs>(m_oIGTCustomCommandHelper_KeyDown);
            m_oIGTCustomCommandHelper.KeyPress += new EventHandler<GTKeyPressEventArgs>(m_oIGTCustomCommandHelper_KeyPress);
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
            m_oIGTCustomCommandHelper.KeyDown -= m_oIGTCustomCommandHelper_KeyDown;
            m_oIGTCustomCommandHelper.KeyPress -= m_oIGTCustomCommandHelper_KeyPress;
            m_oIGTCustomCommandHelper.DblClick -= m_oIGTCustomCommandHelper_DblClick;
            m_oIGTCustomCommandHelper.MouseMove -= m_oIGTCustomCommandHelper_MouseMove;
            m_oIGTCustomCommandHelper.MouseDown -= m_oIGTCustomCommandHelper_MouseDown;
            m_oIGTCustomCommandHelper.MouseUp -= m_oIGTCustomCommandHelper_MouseUp;
            //m_oIGTCustomCommandHelper.WheelRotate -= m_oIGTCustomCommandHelper_WheelRotate;
        }

        void m_CustomForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            ExitCmd();
        }

        public void ExitCmd()
        {
            try
            {
                GTClassFactory.Create<IGTApplication>().SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Exiting...");
                DeActivate();
                Terminate();
                UnsubscribeEvents();
                GTClassFactory.Create<IGTApplication>().EndWaitCursor();
                m_oIGTCustomCommandHelper.Complete();

            }
            catch { }
            finally
            {
                GTClassFactory.Create<IGTApplication>().SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Exited.");
            }
        }


        #endregion

        #region Method

        public static string JOB_ID()
        {
            return m_gtapp.DataContext.ActiveJob.ToString();
        }

        #endregion

    }
}
