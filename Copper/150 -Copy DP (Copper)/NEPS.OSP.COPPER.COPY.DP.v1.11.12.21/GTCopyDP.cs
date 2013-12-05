using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Intergraph.GTechnology.Interfaces;
using Intergraph.GTechnology.API;
using ADODB;

namespace NEPS.OSP.COPPER.COPY.DP
{
    class GTCopyDP : Intergraph.GTechnology.Interfaces.IGTCustomCommandModeless
    {
        #region Copy DP : Variables declaration
        public IGTPoint selGeom = null;
        frmCopyDP m_CustomForm = null;
        private static Intergraph.GTechnology.API.IGTApplication m_gtapp = GTClassFactory.Create<IGTApplication>();
        public static Intergraph.GTechnology.API.IGTTransactionManager m_oIGTTransactionManager = null;
        public static Intergraph.GTechnology.API.IGTCustomCommandHelper m_oIGTCustomCommandHelper = null;
        public static Intergraph.GTechnology.API.IGTGeometryEditService mobjEditService = null;

        private static double detail_ID;
        #endregion

        #region Copy DP : Event Handlers
        bool do_placing = false;
        void m_oIGTCustomCommandHelper_Click(object sender, GTMouseEventArgs e)
        {
            GTClassFactory.Create<IGTApplication>().SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Click.");
            if (mobjEditService.GeometryCount > 0)
                mobjEditService.RemoveAllGeometries();


        }

        void m_oIGTCustomCommandHelper_MouseMove(object sender, GTMouseEventArgs e)
        {
            try
            {
                selGeom = e.WorldPoint;         // to retrieve the coordinate of the pointer once it is click/move

                if (m_CustomForm.getTextValue) // only perform the 
                {
                    if (m_CustomForm != null)
                    {
                        do_placing = true;

                        IGTPointGeometry oPointDPGeom;
                        oPointDPGeom = GTClassFactory.Create<IGTPointGeometry>();
                        IGTPoint oPntGeom = GTClassFactory.Create<IGTPoint>();
                        oPntGeom.X = selGeom.X;
                        oPntGeom.Y = selGeom.Y;
                        int isStyleID = 13020001;               //G3E_SNO for DP.
                        if (mobjEditService.GeometryCount > 0)
                            mobjEditService.RemoveAllGeometries();

                        oPointDPGeom.Origin = oPntGeom;
                        // draw the temp DP symbol
                        mobjEditService.AddGeometry(oPointDPGeom, isStyleID);

                        System.Windows.Forms.Application.DoEvents();
                    }
                    GTClassFactory.Create<IGTApplication>().SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Move mouse to the indicate the new location of the new DP. Double Click to confirm.");
                }
                else
                {
                    do_placing = false;
                }
            }
            catch (Exception ex)
            {
                //throw ex;
            }
        }

        void m_oIGTCustomCommandHelper_DblClick(object sender, GTMouseEventArgs e)
        {
            if (!do_placing) return;

            DialogResult retVal = MessageBox.Show(m_CustomForm, "Click OK to confirm with the New DP Placement. \r\nClick Cancel to cancel placement.", "Copy DP", MessageBoxButtons.OKCancel);
            if (retVal == DialogResult.OK)
            {
                selGeom = e.WorldPoint;
                detail_ID = Convert.ToDouble(e.MapWindow.DetailID.ToString());
                m_CustomForm.createNewDP(selGeom.X, selGeom.Y, detail_ID);
            }

        }

        void m_oIGTCustomCommandHelper_WheelRotate(object sender, GTWheelRotateEventArgs e)
        {
            GTClassFactory.Create<IGTApplication>().SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "WheelRotate.");
        }

        void m_oIGTCustomCommandHelper_MouseUp(object sender, GTMouseEventArgs e)
        {
            GTClassFactory.Create<IGTApplication>().SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "MouseUp.");
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
            try
            {
                GTClassFactory.Create<IGTApplication>().SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Activate.");
                mobjEditService = GTClassFactory.Create<IGTGeometryEditService>();
                mobjEditService.TargetMapWindow = m_gtapp.ActiveMapWindow;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        void m_oIGTCustomCommandHelper_KeyPress(object sender, GTKeyPressEventArgs e)
        {
            GTClassFactory.Create<IGTApplication>().SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "KeyPress.");
        }

        void m_oIGTCustomCommandHelper_KeyDown(object sender, GTKeyEventArgs e)
        {
            //GTClassFactory.Create<IGTApplication>().SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "KeyDown.");
            if (e.KeyCode == 121)
            {
                DialogResult retVal = MessageBox.Show("Are you sure to finish copy DP?", "Copy DP", MessageBoxButtons.YesNo);
                if (retVal == DialogResult.Yes)
                {
                    m_CustomForm.finishCopy();
                }
            }
        }

        #endregion

        #region copy DP : IGTCustomCommandModeless Members
        public void Activate(Intergraph.GTechnology.API.IGTCustomCommandHelper CustomCommandHelper)
        {
            GTClassFactory.Create<IGTApplication>().SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Running .NET custom modeless command...");
            m_oIGTCustomCommandHelper = CustomCommandHelper;

            SubscribeEvents();
            m_CustomForm = new frmCopyDP();
            m_CustomForm.FormClosed += new FormClosedEventHandler(m_CustomForm_FormClosed);
            m_CustomForm.Show();
        }

        void m_CustomForm_FormClosed(object sender, FormClosedEventArgs e)
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
            m_oIGTCustomCommandHelper.Activate += new EventHandler<GTActivateEventArgs>(m_oIGTCustomCommandHelper_Activate);
            m_oIGTCustomCommandHelper.Deactivate += new EventHandler<GTDeactivateEventArgs>(m_oIGTCustomCommandHelper_Deactivate);
            //m_oIGTCustomCommandHelper.GainedFocus += new EventHandler<GTGainedFocusEventArgs>(m_oIGTCustomCommandHelper_GainedFocus);
            //m_oIGTCustomCommandHelper.LostFocus += new EventHandler<GTLostFocusEventArgs>(m_oIGTCustomCommandHelper_LostFocus);
            //m_oIGTCustomCommandHelper.KeyUp += new EventHandler<GTKeyEventArgs>(m_oIGTCustomCommandHelper_KeyUp);
            m_oIGTCustomCommandHelper.KeyDown += new EventHandler<GTKeyEventArgs>(m_oIGTCustomCommandHelper_KeyDown);
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
            m_oIGTCustomCommandHelper.Activate -= m_oIGTCustomCommandHelper_Activate;
            m_oIGTCustomCommandHelper.Deactivate -= m_oIGTCustomCommandHelper_Deactivate;
            //m_oIGTCustomCommandHelper.GainedFocus -= m_oIGTCustomCommandHelper_GainedFocus;
            //m_oIGTCustomCommandHelper.LostFocus -= m_oIGTCustomCommandHelper_LostFocus;
            //m_oIGTCustomCommandHelper.KeyUp -= m_oIGTCustomCommandHelper_KeyUp;
            m_oIGTCustomCommandHelper.KeyDown -= m_oIGTCustomCommandHelper_KeyDown;
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
            GTClassFactory.Create<IGTApplication>().SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "DblClicked! Exiting...");
            UnsubscribeEvents();
            m_oIGTCustomCommandHelper.Complete();
            GTClassFactory.Create<IGTApplication>().SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "DblClicked! Exited.");
        }
        #endregion
    }
}
