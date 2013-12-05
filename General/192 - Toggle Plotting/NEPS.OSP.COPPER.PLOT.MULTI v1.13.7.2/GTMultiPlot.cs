using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Intergraph.GTechnology.Interfaces;
using Intergraph.GTechnology.API;
using ADODB;

using AG.GTechnology.Utilities;

namespace NEPS.OSP.COPPER.PLOT.MULTI
{
    class GTMultiPlot : Intergraph.GTechnology.Interfaces.IGTCustomCommandModeless
    {
        public static Intergraph.GTechnology.API.IGTTransactionManager m_oIGTTransactionManager = null;
        public static Intergraph.GTechnology.API.IGTCustomCommandHelper m_oIGTCustomCommandHelper = null;

        // public IGTPoint SelGeom = null;
        public static frmMultiPlot m_CustomForm = null;
        public static IGTApplication m_gtapp = GTClassFactory.Create<IGTApplication>();
        public static clsPlotArea plotArea = new clsPlotArea();
        public static clsPlotFrame plotFrame = new clsPlotFrame();
        public static IGTMapWindow activeMapWindow = GTMultiPlot.m_gtapp.ActiveMapWindow;

        IGTGeometryEditService mobjEditService;
        public static IGTGeometryEditService mobjPlotFrame;

        #region Plot Task
        public static PlotTask m_iPlotTask = PlotTask.init;
        public enum PlotTask
        {
            init = -1,
            set_plot_prop = 0,
            set_plot_area = 1,
            set_plot_frame = 2,
            set_plot_index = 3,
            set_plot_done = 4,
            start_ploting = 9
        };

        #endregion

        #region Plot : Event Handlers

        void m_oIGTCustomCommandHelper_Click(object sender, GTMouseEventArgs e)
        {
            if ((GTClassFactory.Create<IGTApplication>().Application.ActiveMapWindow.MousePointer == GTMousePointerConstants.gtmwmpZoomIn) ||
               (GTClassFactory.Create<IGTApplication>().Application.ActiveMapWindow.MousePointer == GTMousePointerConstants.gtmwmpZoomOut) ||
               (GTClassFactory.Create<IGTApplication>().Application.ActiveMapWindow.MousePointer == GTMousePointerConstants.gtmwmpPan) ||
               (GTClassFactory.Create<IGTApplication>().Application.ActiveMapWindow.MousePointer == GTMousePointerConstants.gtmwmpRotate))
            {
                return;
            }

            if (m_iPlotTask == PlotTask.set_plot_area)
            {
                activeMapWindow = GTMultiPlot.m_gtapp.ActiveMapWindow;

                if (e.Button == 2) // (right click == 2 )(left click == 1)
                {
                    plotArea.ResetPlotPoint();
                    return;
                }
                plotArea.ApplyPlotPoint();
                if (plotArea.isBottomRight == false && plotArea.isTopLeft == false)
                    m_iPlotTask = PlotTask.set_plot_frame;
            }
            if (m_iPlotTask == PlotTask.set_plot_frame)
            {
                plotArea.ClearPlotArea();
                // generate plot frames base on selected area
                plotFrame.CreatePlotFrames(plotArea.topleft_point, plotArea.bottomright_point);
                // draw plot frames in GTECH
                plotFrame.DrawFrame(mobjPlotFrame);
                m_iPlotTask = PlotTask.set_plot_index;
            }
            if (m_iPlotTask == PlotTask.set_plot_index)
            {
                m_CustomForm.Show();
                //m_iPlotTask = PlotTask.start_ploting;
            }
        }

        // create temporary plot area while mouse move
        void m_oIGTCustomCommandHelper_MouseMove(object sender, GTMouseEventArgs e)
        {
            try
            {
                if ((GTClassFactory.Create<IGTApplication>().Application.ActiveMapWindow.MousePointer == GTMousePointerConstants.gtmwmpZoomIn) ||
                   (GTClassFactory.Create<IGTApplication>().Application.ActiveMapWindow.MousePointer == GTMousePointerConstants.gtmwmpZoomOut) ||
                   (GTClassFactory.Create<IGTApplication>().Application.ActiveMapWindow.MousePointer == GTMousePointerConstants.gtmwmpPan) ||
                   (GTClassFactory.Create<IGTApplication>().Application.ActiveMapWindow.MousePointer == GTMousePointerConstants.gtmwmpRotate))
                {
                    //do nothing
                }
                else if (m_iPlotTask == PlotTask.set_plot_area)
                {
                    if (!plotArea.isBottomRight) plotArea.isTopLeft = true;

                    plotArea.DrawPlotArea(e.WorldPoint);

                    GTClassFactory.Create<IGTApplication>().Application.ActiveMapWindow.MousePointer = GTMousePointerConstants.gtmwmpCrossHair;
                    return;
                }
                System.Windows.Forms.Application.DoEvents();
            }
            catch (Exception ex)
            {
                //log.WriteLog("Error", "m_oCustomCommandHelper_MouseMove", ex.Message);
                //log.WriteLog(ex.StackTrace); MessageBox.Show(ex.Message, "Plotting Boundary", MessageBoxButtons.OK, MessageBoxIcon.Error);
                ExitCmd();
                //log.CloseFile();
            }

        }

        void m_oIGTCustomCommandHelper_DblClick(object sender, GTMouseEventArgs e)
        {
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
            if (e.KeyCode == 27 && m_iPlotTask == PlotTask.set_plot_area) // ESC key - cancel add Boundary
            {
                GTClassFactory.Create<IGTApplication>().SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Cancel Place Boundry");
                plotArea.ResetPlotPoint();
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
            try
            {
                GTClassFactory.Create<IGTApplication>().SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Activate.");
                mobjEditService = GTClassFactory.Create<IGTGeometryEditService>();
                mobjEditService.TargetMapWindow = m_gtapp.ActiveMapWindow;
                if (mobjPlotFrame == null)
                {
                    mobjPlotFrame = GTClassFactory.Create<IGTGeometryEditService>();
                    mobjPlotFrame.TargetMapWindow = m_gtapp.ActiveMapWindow;
                }
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
                DialogResult retVal = MessageBox.Show("Are you sure to finish Plotting?", "Plotting", MessageBoxButtons.YesNo);
                if (retVal == DialogResult.Yes)
                {
                    //m_CustomForm.finishCopy();
                }
            }
        }

        #endregion

        #region MultiPlot : IGTCustomCommandModeless Members
        public void Activate(Intergraph.GTechnology.API.IGTCustomCommandHelper CustomCommandHelper)
        {
            GTClassFactory.Create<IGTApplication>().SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Running .NET custom modeless command...");
            m_oIGTCustomCommandHelper = CustomCommandHelper;

            SubscribeEvents();

            m_iPlotTask = PlotTask.init;
            plotFrame.Frames.Clear();
            plotArea.bottomright_point = GTClassFactory.Create<IGTPoint>();
            plotArea.topleft_point = GTClassFactory.Create<IGTPoint>();

            m_CustomForm = new frmMultiPlot();
            m_CustomForm.FormClosed += new FormClosedEventHandler(m_CustomForm_FormClosed);
            m_CustomForm.Show();
        }

        void m_CustomForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            m_iPlotTask = PlotTask.init;
            plotFrame.Frames.Clear();
            plotArea.ClearPlotArea();

            //plotArea.bottomright_point = GTClassFactory.Create<IGTPoint>();
            //plotArea.topleft_point = GTClassFactory.Create<IGTPoint>();
            IGTNamedPlots oGTNamePlots = GTMultiPlot.m_gtapp.NamedPlots;
            for (int c = oGTNamePlots.Count; c > 0; )
            {
                oGTNamePlots.Remove(--c);
            }
            IGTPlotWindows oGTPlotWindows = GTMultiPlot.m_gtapp.GetPlotWindows();
            for (int i = oGTPlotWindows.Count; i > 0; )
            {
                oGTPlotWindows[--i].Close();
            }
            if (mobjEditService != null)
            {
                mobjEditService.RemoveAllGeometries();
                mobjEditService = null;
                System.Windows.Forms.Application.DoEvents();
            }
            if (mobjPlotFrame != null)
            {
                mobjPlotFrame.RemoveAllGeometries();
                mobjPlotFrame = null;
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
            GTClassFactory.Create<IGTApplication>().SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "DblClicked! Exiting...");            
            Terminate(); 
            UnsubscribeEvents();
            m_oIGTCustomCommandHelper.Complete();
            GTClassFactory.Create<IGTApplication>().SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "DblClicked! Exited.");
        }
        #endregion

    }
}
