using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Intergraph.GTechnology.Interfaces;
using Intergraph.GTechnology.API;
using ADODB;

namespace NEPS.GTechnology.Property_Count
{

    public class WinWrapper : System.Windows.Forms.IWin32Window
    {        

        #region IWin32Window Members

        IntPtr IWin32Window.Handle
        {
            get 
            {
                System.IntPtr iptr = new System.IntPtr();
                iptr = (System.IntPtr)GTClassFactory.Create<IGTApplication>().hWnd;
                //iptr = CType(GTClassFactory.Create<IGTApplication>().hWnd, System.IntPtr);
                return iptr;
            }
        }

        #endregion
    }
       

    class GTCustomCommandModeless : Intergraph.GTechnology.Interfaces.IGTCustomCommandModeless
    {
        public static Intergraph.GTechnology.API.IGTTransactionManager m_oGTTransactionManager = null;
        public static Intergraph.GTechnology.API.IGTCustomCommandHelper m_oGTCustomCommandHelper = null;

        public static WinWrapper objWinWrapper = new WinWrapper();

        //public GisLib objGisLib = null;
        public IGTPoint oPoint = null;
        public bool bBeginSelect;
       // private string sFNOFilter;

        GTWindowsForm_Property_Count m_CustomForm = null;
        
        #region Event Handlers
        void m_oGTCustomCommandHelper_Click(object sender, GTMouseEventArgs e)
        {
            try
            {
                GTClassFactory.Create<IGTApplication>().SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Clicked!: " + e.MapWindow.Caption + ". Double Click to exit custom modeless command.");
            }
            catch (Exception e1)
            {
                throw e1;
            }
        }
        void m_oGTCustomCommandHelper_MouseMove(object sender, GTMouseEventArgs e)
        {
            GTClassFactory.Create<IGTApplication>().SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Click to continue.  Double Click to exit custom modeless command.");
            
        }

        void m_oGTCustomCommandHelper_DblClick(object sender, GTMouseEventArgs e)
        {
            // auto pick the double-clicked boundary
            IGTApplication app = GTClassFactory.Create<IGTApplication>();
            IGTDDCKeyObjects selectedObjects = app.ActiveMapWindow.LocateService.Locate(e.WorldPoint, 10, 5, GTSelectionTypeConstants.gtmwstSelectSingle);
            m_CustomForm.SetActiveBoundary(selectedObjects);
        }
        void m_oGTCustomCommandHelper_WheelRotate(object sender, GTWheelRotateEventArgs e)
        {
            GTClassFactory.Create<IGTApplication>().SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "WheelRotate.");
        }

        void m_oGTCustomCommandHelper_MouseUp(object sender, GTMouseEventArgs e)
        {
            GTClassFactory.Create<IGTApplication>().SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "MouseUp.");
        }

        void m_oGTCustomCommandHelper_MouseDown(object sender, GTMouseEventArgs e)
        {            
           // GTClassFactory.Create<IGTApplication>().SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "MouseDown.");
            oPoint = e.WorldPoint;
            if (m_CustomForm != null)
            {
                m_CustomForm.XPointGeom = oPoint.X.ToString();
                m_CustomForm.YPointGeom = oPoint.Y.ToString();
            }
        }

        void m_oGTCustomCommandHelper_LostFocus(object sender, GTLostFocusEventArgs e)
        {
            GTClassFactory.Create<IGTApplication>().SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "LostFocus.");
        }

        void m_oGTCustomCommandHelper_KeyUp(object sender, GTKeyEventArgs e)
        {
            GTClassFactory.Create<IGTApplication>().SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "KeyUp.");
            
        }

        void m_oGTCustomCommandHelper_GainedFocus(object sender, GTGainedFocusEventArgs e)
        {
            GTClassFactory.Create<IGTApplication>().SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "GainedFocus.");
        }

        void m_oGTCustomCommandHelper_Deactivate(object sender, GTDeactivateEventArgs e)
        {
            GTClassFactory.Create<IGTApplication>().SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Deactivate.");
        }

        void m_oGTCustomCommandHelper_Activate(object sender, GTActivateEventArgs e)
        {
            GTClassFactory.Create<IGTApplication>().SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Activate.");
        }

        void m_oGTCustomCommandHelper_KeyPress(object sender, GTKeyPressEventArgs e)
        {
            GTClassFactory.Create<IGTApplication>().SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "KeyPress.");
        }

        void m_oGTCustomCommandHelper_KeyDown(object sender, GTKeyEventArgs e)
        {
            GTClassFactory.Create<IGTApplication>().SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "KeyDown.");
        }

        #endregion
        #region IGTCustomCommandModeless Members

        public void Activate(Intergraph.GTechnology.API.IGTCustomCommandHelper CustomCommandHelper)
        {
            GTClassFactory.Create<IGTApplication>().SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Running .NET custom modeless command...");

            m_oGTCustomCommandHelper = CustomCommandHelper;
                       
            SubscribeEvents();
            m_CustomForm = new GTWindowsForm_Property_Count();
            m_CustomForm.FormClosed += new FormClosedEventHandler(m_CustomForm_FormClosed);
            this.bBeginSelect = true;
            m_CustomForm.Show(objWinWrapper);

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

                if (m_oGTTransactionManager != null)
                {
                    m_oGTTransactionManager = null;
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
                m_oGTTransactionManager = value;
            }
        }

        #endregion
        public void SubscribeEvents()
        {
            // Subscribe to m_oIGTCustomCommandHelper events using C# 1.0 syntax
            m_oGTCustomCommandHelper.Activate += new EventHandler<GTActivateEventArgs>(m_oGTCustomCommandHelper_Activate);
            m_oGTCustomCommandHelper.Deactivate += new EventHandler<GTDeactivateEventArgs>(m_oGTCustomCommandHelper_Deactivate);
            m_oGTCustomCommandHelper.GainedFocus += new EventHandler<GTGainedFocusEventArgs>(m_oGTCustomCommandHelper_GainedFocus);
            m_oGTCustomCommandHelper.LostFocus += new EventHandler<GTLostFocusEventArgs>(m_oGTCustomCommandHelper_LostFocus);
            m_oGTCustomCommandHelper.KeyUp += new EventHandler<GTKeyEventArgs>(m_oGTCustomCommandHelper_KeyUp);
            m_oGTCustomCommandHelper.KeyDown += new EventHandler<GTKeyEventArgs>(m_oGTCustomCommandHelper_KeyDown);
            m_oGTCustomCommandHelper.KeyPress += new EventHandler<GTKeyPressEventArgs>(m_oGTCustomCommandHelper_KeyPress);
            m_oGTCustomCommandHelper.Click += new EventHandler<GTMouseEventArgs>(m_oGTCustomCommandHelper_Click);
            m_oGTCustomCommandHelper.DblClick += new EventHandler<GTMouseEventArgs>(m_oGTCustomCommandHelper_DblClick);
            m_oGTCustomCommandHelper.MouseMove += new EventHandler<GTMouseEventArgs>(m_oGTCustomCommandHelper_MouseMove);
            m_oGTCustomCommandHelper.MouseDown += new EventHandler<GTMouseEventArgs>(m_oGTCustomCommandHelper_MouseDown);
            m_oGTCustomCommandHelper.MouseUp += new EventHandler<GTMouseEventArgs>(m_oGTCustomCommandHelper_MouseUp);
            m_oGTCustomCommandHelper.WheelRotate += new EventHandler<GTWheelRotateEventArgs>(m_oGTCustomCommandHelper_WheelRotate);
        }

        private void UnsubscribeEvents()
        {
            // UnSubscribe to m_oIGTCustomCommandHelper events using C# 1.0 syntax
            m_oGTCustomCommandHelper.Activate -= m_oGTCustomCommandHelper_Activate;
            m_oGTCustomCommandHelper.Deactivate -= m_oGTCustomCommandHelper_Deactivate;
            m_oGTCustomCommandHelper.GainedFocus -= m_oGTCustomCommandHelper_GainedFocus;
            m_oGTCustomCommandHelper.LostFocus -= m_oGTCustomCommandHelper_LostFocus;
            m_oGTCustomCommandHelper.KeyUp -= m_oGTCustomCommandHelper_KeyUp;
            m_oGTCustomCommandHelper.KeyDown -= m_oGTCustomCommandHelper_KeyDown;
            m_oGTCustomCommandHelper.KeyPress -= m_oGTCustomCommandHelper_KeyPress;
            m_oGTCustomCommandHelper.Click -= m_oGTCustomCommandHelper_Click;
            m_oGTCustomCommandHelper.DblClick -= m_oGTCustomCommandHelper_DblClick;
            m_oGTCustomCommandHelper.MouseMove -= m_oGTCustomCommandHelper_MouseMove;
            m_oGTCustomCommandHelper.MouseDown -= m_oGTCustomCommandHelper_MouseDown;
            m_oGTCustomCommandHelper.MouseUp -= m_oGTCustomCommandHelper_MouseUp;
            m_oGTCustomCommandHelper.WheelRotate -= m_oGTCustomCommandHelper_WheelRotate;
        }

        public void ExitCmd()
        {
            GTClassFactory.Create<IGTApplication>().SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "DblClicked! Exiting...");
            UnsubscribeEvents();
            m_oGTCustomCommandHelper.Complete();
            GTClassFactory.Create<IGTApplication>().SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "DblClicked! Exited.");

        }

    }
}
