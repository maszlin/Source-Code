/*
 * Locate Feature - Modeless Custom Command
 * 
 * Author: Felipe Armoni - Credent
 * 
 * This modeless custom command gives the user a list of features and a input text box. The user can select one of those features
 * and input a value, then another list will open with all the placed features that contain that value. When the user selects
 * one of those features the map window will zoom on it.
 * 
 */
using ADODB;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Intergraph.GTechnology.Interfaces;
using Intergraph.GTechnology.API;
using System.Diagnostics;
using AG.GTechnology.Utilities;

namespace AG.GTechnology.PlottingBnd
{
    class GTPlottingBnd : Intergraph.GTechnology.Interfaces.IGTCustomCommandModeless
    {
        private Intergraph.GTechnology.API.IGTTransactionManager m_oGTTransactionManager;
        private Intergraph.GTechnology.API.IGTCustomCommandHelper m_oGTCustomCommandHelper;
        private static Intergraph.GTechnology.API.IGTApplication application = GTClassFactory.Create<IGTApplication>();
        private Logger log;
        private IGTDataContext oDataContext = application.DataContext;
        private static IGTPolylineGeometry oLineGeom = null;
        private static List<IGTGeometry> arrIntersection = new List<IGTGeometry>();
        private static List<IGTGeometry> arrIntersectionText = new List<IGTGeometry>();
        private string mstrFistPoint = "Please enter the attributes for plot windows.";
        private int lStyleId = 11508;

        private frmMain m_PlotBoundaryForm;

        private double Pi = 3.141592653589793;

        bool mblnVisible = false;
        int mintState;
        //IGTGeometryCreationService mobjCreationService;
        //IGTFeaturePlacementService mobjPlacementService;
        IGTGeometryEditService mobjEditService;
        IGTGeometryEditService mobjEditPointService;
        IGTFeatureExplorerService mobjExplorerService;
        //IGTKeyObject mobjContourFeature;
        IGTPoint m_pRotateAnchor;
        IGTPoint mptPoint1;
        IGTPoint mptPoint2;
        IGTKeyObject mobjSubDuctAttribute = null;

        private PlotBoundary TempBnd = new PlotBoundary();

        List<PlotBoundary> Boundaries = new List<PlotBoundary>();
        double m_dAngleDeg = 0;


        private IGTPolygonGeometry CreatePolygonGeometry(IGTPoint UserPoint, double dAngle)
        {
            double angle = (Math.PI
                        * (dAngle / 180));

            double angle90 = (Math.PI * (90.0 / 180.0));
            double angle180 = (Math.PI * (180.0 / 180.0));
            double sinAngle = Math.Sin(angle);
            double cosAngle = Math.Cos(angle);
            IGTPolygonGeometry oPolygonGeometry;
            IGTPoint oPoint0;
            IGTPoint oPoint1;
            IGTPoint oPoint2;
            IGTPoint oPoint3;
            //  Geometry construction process.
            oPolygonGeometry = GTClassFactory.Create<IGTPolygonGeometry>();
            oPoint0 = GTClassFactory.Create<IGTPoint>();
            oPoint0.X = (UserPoint.X
                        + (Math.Cos(angle - angle90) * m_PlotBoundaryForm.MapHeightScaled / 2));
            oPoint0.Y = (UserPoint.Y
                        + (Math.Sin(angle - angle90) * m_PlotBoundaryForm.MapHeightScaled / 2));
            oPolygonGeometry.Points.Add(oPoint0);
            oPoint1 = GTClassFactory.Create<IGTPoint>();
            oPoint1.X = (oPoint0.X
                        + (Math.Cos(angle) * m_PlotBoundaryForm.MapWidthScaled));
            oPoint1.Y = (oPoint0.Y
                        + (Math.Sin(angle) * m_PlotBoundaryForm.MapWidthScaled));
            oPolygonGeometry.Points.Add(oPoint1);
            oPoint2 = GTClassFactory.Create<IGTPoint>();
            oPoint2.X = (oPoint1.X
                        + (Math.Cos((angle + angle90)) * m_PlotBoundaryForm.MapHeightScaled));
            oPoint2.Y = (oPoint1.Y
                        + (Math.Sin((angle + angle90)) * m_PlotBoundaryForm.MapHeightScaled));
            oPolygonGeometry.Points.Add(oPoint2);
            oPoint3 = GTClassFactory.Create<IGTPoint>();
            oPoint3.X = (oPoint2.X
                        + (Math.Cos((angle + angle180)) * m_PlotBoundaryForm.MapWidthScaled));
            oPoint3.Y = (oPoint2.Y
                        + (Math.Sin((angle + angle180)) * m_PlotBoundaryForm.MapWidthScaled));
            oPolygonGeometry.Points.Add(oPoint3);
            oPolygonGeometry.Points.Add(oPoint0);
            return oPolygonGeometry;
        }

        private PlotBoundary CreatePlotBoundary(IGTPoint pRotateAnchor, double dAngleDeg)
        {
            PlotBoundary bnd = new PlotBoundary();
            bnd.Type = application.Properties["PlotBoundary.Type"].ToString();
            bnd.PaperSize = application.Properties["PlotBoundary.PaperSize"].ToString();
            bnd.PaperOrientation = application.Properties["PlotBoundary.SheetOrientation"].ToString();
            bnd.MapScale = application.Properties["PlotBoundary.MapScale"].ToString();

            return bnd;
        }

        public void Activate(Intergraph.GTechnology.API.IGTCustomCommandHelper CustomCommandHelper)
        {
            try
            {
                application.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, mstrFistPoint);

                log = Logger.getInstance();
                log.WriteLog(this.GetType().FullName, "Plotting Boundary Placement started!");
                log.WriteLog("");

                m_oGTCustomCommandHelper = CustomCommandHelper;

                mobjEditService = GTClassFactory.Create<IGTGeometryEditService>();
                mobjEditService.TargetMapWindow = application.ActiveMapWindow;

                mobjEditPointService = GTClassFactory.Create<IGTGeometryEditService>();
                mobjEditPointService.TargetMapWindow = application.ActiveMapWindow;

                RegisterEvents();

                m_PlotBoundaryForm = new frmMain();
                m_PlotBoundaryForm.ShowDialog();

                if (m_PlotBoundaryForm.PlaceComponent)
                    mintState = 2;
                else
                    ExitCmd();
            }
            catch (Exception e)
            {
                log.WriteLog("");
                log.WriteLog(this.GetType().FullName, "ERROR: " + e.Message);
                log.WriteLog(e.StackTrace);
                MessageBox.Show(e.Message, "Error.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                ExitCmd();
            }
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
            //try
            //{



                if (m_oGTTransactionManager != null)
                {
                    if (m_oGTTransactionManager.TransactionInProgress)
                    {
                        m_oGTTransactionManager.Rollback();
                    }
                }

                if (!(mobjExplorerService == null))
                {
                    mobjExplorerService.Clear();
                }

                if ((!(mobjExplorerService == null)
                            && !mblnVisible))
                {
                    mobjExplorerService.Visible = false;
                }
                //mobjCreationService = null;
                //mobjPlacementService = null;
                m_pRotateAnchor = null;

            //}
            //catch (Exception e)
            //{
            //    throw e;
            //}
        }

        public Intergraph.GTechnology.API.IGTTransactionManager TransactionManager
        {
            set
            {
                m_oGTTransactionManager = value;
            }
        }

        private void ExitCmd()
        {
            try
            {
                log.CloseFile();
                application.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Exiting...");
                UnRegisterEvents();

                if (mobjEditService != null)
                {
                    mobjEditService.RemoveAllGeometries();
                    mobjEditService = null;
                }
                if (mobjEditPointService != null)
                {
                    mobjEditPointService.RemoveAllGeometries();
                    mobjEditPointService = null;
                }

                if (m_oGTCustomCommandHelper != null) m_oGTCustomCommandHelper.Complete();
                application.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Exited.");
            }
            catch (Exception ex)
            {
                //Logger log = Logger.GetInstance();
                log.WriteLog("Error", "ExitCmd", ex.Message);
                log.WriteLog(ex.StackTrace); MessageBox.Show(ex.Message, "Plotting Boundary", MessageBoxButtons.OK, MessageBoxIcon.Error);
                ExitCmd();//log.CloseFile();
            }
    }

        private void m_oCustomCommandHelper_MouseUp(object sender, GTMouseEventArgs MouseEventArgs)
        {

            try
            {
                IGTPoint WorldPoint = MouseEventArgs.WorldPoint;

                if (MouseEventArgs.Button == 2)
                {
                    
                    if (Boundaries.Count > 0)
                    {
                        if (mobjEditPointService.GeometryCount > 0) mobjEditPointService.RemoveAllGeometries();

                        application.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Create/Place plot or Cancel to exit"); 
                        m_PlotBoundaryForm.CreatePlot = true;
                        m_PlotBoundaryForm.ShowDialog();

                        if (m_PlotBoundaryForm.CreatePlot)
                        {
                            IGTNamedPlot oGTNamedPlot;
                            IGTPlotWindow oGTPlotWindow;
                            
                            string sStatusBarTextTemp = string.Empty;
                            IGTMapWindow oMapWindow = application.ActiveMapWindow;
                        //   oGTPlotWindow.InsertMap(
                            int plotnum = application.NamedPlots.Count +1;
                            for (int i =0;i<Boundaries.Count;i++)
                            {
                                PlotBoundary oPlot = Boundaries[i];
                                oPlot.Name = "Plotting " + plotnum;
                                oPlot.Legend =  oMapWindow.LegendName;
                                oPlot.activemap = oMapWindow;
                               // oMapWindow.
                                oPlot.ActiveLegendNodes = oMapWindow.DisplayService.GetDisplayControlNodes();

                                //for (int j = 0; j < application.NamedPlots.Count;j++ )
                                //{
                                //    if (application.NamedPlots[j].Name == oPlot.Name)
                                //        application.NamedPlots.Remove(j);
                                //}

                                while (0 == 0)
                                {

                                    try
                                    { 
                                        //  Create a new plot window named the same as the PlanID
                                        oGTNamedPlot = application.NewNamedPlot(oPlot.Name);
                                        //  Auto open the new named plot - need to only do this if single named plot created otherwise ask user at the end using dialog FUTURE -Currently need to have PlotWindow open to Insert a Map Window
                                        sStatusBarTextTemp = application.GetStatusBarText(GTStatusPanelConstants.gtaspcMessage);                               
                                        oGTPlotWindow = application.NewPlotWindow(oGTNamedPlot);
                                        break;
                                    }
                                    catch (Exception ex)
                                    {
                                        plotnum++;
                                        oPlot.Name = "Plotting " + plotnum;
                                    }
                                }
                               // oGTPlotWindow.
                                application.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, sStatusBarTextTemp); 
                               
                                PPlottingLib.StartDrawingRedlines(oPlot);
                                plotnum++;
                            }
                            ExitCmd();
                        }
                        else if (!m_PlotBoundaryForm.PlaceComponent)
                            ExitCmd();
                        else
                        {
                            if (mobjEditPointService.GeometryCount > 0) mobjEditPointService.RemoveAllGeometries();
                            if (mobjEditService.GeometryCount > 0) mobjEditService.RemoveAllGeometries();

                            Boundaries.Clear();
                            mintState = 2;
                            return;
                        }
                        return;
                    }
                    else
                    {
                        if (mintState == 2 || mintState == 3)
                        {
                            if (mobjEditPointService.GeometryCount > 0) mobjEditPointService.RemoveAllGeometries();
                            mintState = 0;
                            m_PlotBoundaryForm.CreatePlot = false;
                            application.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Place plot or Cancel to exit"); 
                            m_PlotBoundaryForm.ShowDialog();
                            if (m_PlotBoundaryForm.PlaceComponent)
                                mintState = 2;
                            else
                                ExitCmd();
                            return;
                        }

                       // ExitCmd();
                        return;
                    }
                }

                //  If the current step in the command is the third step then get the selected point.
                if ((mintState == 2))
                {
                    double X = WorldPoint.X;
                    double Y = WorldPoint.Y;

                    m_pRotateAnchor = WorldPoint;

                    application.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Click to accept plot boundary or right-click to create plot"); 
                    mintState = 3;
                }
                else if ((mintState == 3))
                {
                    if (m_pRotateAnchor != null)
                    {
                        //if (mobjEditService.GeometryCount > 0) mobjEditService.RemoveAllGeometries();

                        PlotBoundary oPlot = CreatePlotBoundary(m_pRotateAnchor, m_dAngleDeg);

                        double angle = (Math.PI * (m_dAngleDeg / 180));
                        double angle90 = (Math.PI * (90.0 / 180.0));
                        double angle180 = (Math.PI * (180.0 / 180.0));

                        oPlot.BottomLeft = GTClassFactory.Create<IGTPoint>();
                        oPlot.BottomLeft.X = (m_pRotateAnchor.X
                                    + (Math.Cos(angle - angle90) * m_PlotBoundaryForm.MapHeightScaled / 2));
                        oPlot.BottomLeft.Y = (m_pRotateAnchor.Y
                                    + (Math.Sin(angle - angle90) * m_PlotBoundaryForm.MapHeightScaled / 2));

                        oPlot.TopRight = GTClassFactory.Create<IGTPoint>();
                        oPlot.TopRight.X = (m_pRotateAnchor.X
                                    + (Math.Cos(angle + angle90) * m_PlotBoundaryForm.MapHeightScaled / 2));
                        oPlot.TopRight.Y = (m_pRotateAnchor.Y
                                    + (Math.Sin(angle + angle90) * m_PlotBoundaryForm.MapHeightScaled / 2));

                        oPlot.TopRight.X = (oPlot.TopRight.X
                                    + (Math.Cos(angle) * m_PlotBoundaryForm.MapWidthScaled));
                        oPlot.TopRight.Y = (oPlot.TopRight.Y
                                    + (Math.Sin(angle) * m_PlotBoundaryForm.MapWidthScaled));

                        oPlot.Geometry = CreatePolygonGeometry(m_pRotateAnchor, m_dAngleDeg);

                        mobjEditService.AddGeometry(oPlot.Geometry, 41500);

                        Boundaries.Add(oPlot);

                        m_pRotateAnchor.X = (m_pRotateAnchor.X
                                        + (Math.Cos((Math.PI * (m_dAngleDeg / 180))) * m_PlotBoundaryForm.MapWidthScaled));
                        m_pRotateAnchor.Y = (m_pRotateAnchor.Y
                                        + (Math.Sin((Math.PI * (m_dAngleDeg / 180))) * m_PlotBoundaryForm.MapWidthScaled));

                        application.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Click to accept plot boundary or right-click to create plot"); 
                    }

                }

            }
            catch (Exception ex)
            {
                //Logger log = Logger.GetInstance();
                log.WriteLog("Error", "m_oCustomCommandHelper_MouseUp", ex.Message);
                log.WriteLog(ex.StackTrace);
                MessageBox.Show(ex.Message, "Plotting Boundary", MessageBoxButtons.OK, MessageBoxIcon.Error);
               
                if (m_oGTTransactionManager != null) m_oGTTransactionManager.Rollback();
                ExitCmd();
                //log.CloseFile();
            }
        }

        private void m_oCustomCommandHelper_MouseMove(object sender, GTMouseEventArgs MouseEventArgs)
        {
            try
            {
                if ((mintState == 2))
                {
                     IGTPoint UserPoint = MouseEventArgs.WorldPoint;

                    double X = UserPoint.X;
                    double Y = UserPoint.Y;


                    m_pRotateAnchor = UserPoint;

                    application.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Click to accept position of plot or right-click to cancel"); 
                   
                    if (m_pRotateAnchor != null)
                    {
                        if (mobjEditPointService.GeometryCount > 0) mobjEditPointService.RemoveAllGeometries();

                        mobjEditPointService.AddGeometry(CreatePolygonGeometry(m_pRotateAnchor, 0), 41500);
                    }
                    return;
                }

                if (mintState == 3)
                {
                    IGTPoint UserPoint = MouseEventArgs.WorldPoint;

                    double X = UserPoint.X;
                    double Y = UserPoint.Y;

                    double dSlope = 0;
                    double m_dAngle = 0;

                    application.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Click to accept position of plot or right-click to cancel"); 

                    if (m_pRotateAnchor != null)
                    {
                        if (mobjEditPointService.GeometryCount > 0) mobjEditPointService.RemoveAllGeometries();

                        dSlope = ((m_pRotateAnchor.Y - UserPoint.Y) / (m_pRotateAnchor.X - UserPoint.X));
                        if (!Double.IsNaN(Math.Atan(dSlope)))
                        {
                            m_dAngle = Math.Atan(dSlope);
                            // m_dAngle = Atan2(m_pRotateAnchor.Y - UserPoint.Y, m_pRotateAnchor.X - UserPoint.X) ' Using the dSlope and Atan() instead of Atan2() keeps the polygon right reading.
                            m_dAngleDeg = (m_dAngle * (180 / Math.PI));
                            int ShiftState = 2;
                            switch (ShiftState)
                            {
                                case 2:
                                    //  Round to the nearest degree
                                    m_dAngleDeg = Math.Round(m_dAngleDeg, 0);
                                    break;
                                case 3:
                                    //  Snap to nearest 45deg
                                    if (((m_dAngleDeg > (0 - 22.5))
                                                && (m_dAngleDeg <= (0 + 22.5))))
                                    {
                                        m_dAngleDeg = 0;
                                    }
                                    if (((m_dAngleDeg > (45 - 22.5))
                                                && (m_dAngleDeg <= (45 + 22.5))))
                                    {
                                        m_dAngleDeg = 45;
                                    }
                                    if (((m_dAngleDeg > (90 - 22.5))
                                                && (m_dAngleDeg <= (90 + 22.5))))
                                    {
                                        m_dAngleDeg = 90;
                                    }
                                    if (((m_dAngleDeg
                                                > ((45 - 22.5)
                                                * -1))
                                                && (m_dAngleDeg <= (-45 + 22.5))))
                                    {
                                        m_dAngleDeg = -45;
                                    }
                                    if (((m_dAngleDeg
                                                > ((90 - 22.5)
                                                * -1))
                                                && (m_dAngleDeg <= (-90 + 22.5))))
                                    {
                                        m_dAngleDeg = -90;
                                    }
                                    break;
                            }
                        }
                        mobjEditPointService.AddGeometry(CreatePolygonGeometry(m_pRotateAnchor, m_dAngleDeg), 41500);
                        //IGTPoint pnt1 = GTClassFactory.Create<IGTPoint>();
                        //IGTPoint pnt2 = GTClassFactory.Create<IGTPoint>();
                        //IGTPoint pnt3 = GTClassFactory.Create<IGTPoint>();
                        //IGTPoint pnt4 = GTClassFactory.Create<IGTPoint>();
                        //IGTPolygonGeometry plotBnd = GTClassFactory.Create<IGTPolygonGeometry>();
                        //plotBnd.Points.Add(pnt1);
                        //plotBnd.Points.Add(pnt2);
                        //plotBnd.Points.Add(pnt3);
                        //plotBnd.Points.Add(pnt4);
                        //mobjEditPointService.AddGeometry(plotBnd, 1000);
                    }
                    return;
                }
                System.Windows.Forms.Application.DoEvents();
            }
            catch (Exception ex)
            {
                log.WriteLog("Error", "m_oCustomCommandHelper_MouseMove", ex.Message);
                log.WriteLog(ex.StackTrace); MessageBox.Show(ex.Message, "Plotting Boundary", MessageBoxButtons.OK, MessageBoxIcon.Error);
                ExitCmd();
                //log.CloseFile();
            }
        }

        /// <summary>
        /// Register EventHandlers on CommandHelper
        /// </summary>
        /// <param name=""></param>
        #region private void RegisterEvents()
        private void RegisterEvents()
        {
            try
            {
                //m_oGTCustomCommandHelper.KeyUp += new EventHandler<GTKeyEventArgs>(m_oCustomCommandHelper_KeyUp);
                //m_oGTCustomCommandHelper.Click += new EventHandler<GTMouseEventArgs>(m_oCustomCommandHelper_Click);
                //m_oGTCustomCommandHelper.MouseDown += new EventHandler<GTMouseEventArgs>(m_oCustomCommandHelper_MouseDown);
                m_oGTCustomCommandHelper.MouseUp += new EventHandler<GTMouseEventArgs>(m_oCustomCommandHelper_MouseUp);
                m_oGTCustomCommandHelper.MouseMove += new EventHandler<GTMouseEventArgs>(m_oCustomCommandHelper_MouseMove);
                //m_oCustomCommandHelper.MouseMove += new EventHandler(this.m_oCustomCommandHelper_MouseMove);
            }
            catch (Exception ex)
            {
                //Logger log = Logger.GetInstance();
                log.WriteLog("Error", "RegisterEvents", ex.Message);
                log.WriteLog(ex.StackTrace); MessageBox.Show(ex.Message, "Plotting Boundary", MessageBoxButtons.OK, MessageBoxIcon.Error);
                ExitCmd();//log.CloseFile();
            }
        }
        #endregion

        /// <summary>
        /// UnRegister EventHandlers on CommandHelper
        /// </summary>
        /// <param name=""></param>
        #region private void UnRegisterEvents()
        private void UnRegisterEvents()
        {
            try
            {
                //m_oGTCustomCommandHelper.KeyUp -= m_oCustomCommandHelper_KeyUp;
                //m_oGTCustomCommandHelper.Click -= m_oCustomCommandHelper_Click;
                //m_oGTCustomCommandHelper.MouseDown -= m_oCustomCommandHelper_MouseDown;
                m_oGTCustomCommandHelper.MouseUp -= m_oCustomCommandHelper_MouseUp;
                m_oGTCustomCommandHelper.MouseMove -= m_oCustomCommandHelper_MouseMove;
                //m_oCustomCommandHelper.MouseMove += new EventHandler(this.m_oCustomCommandHelper_MouseMove);
            }
            catch (Exception ex)
            {
                //Logger log = Logger.GetInstance();
                log.WriteLog("Error", "UnRegisterEvents", ex.Message);
                log.WriteLog(ex.StackTrace); MessageBox.Show(ex.Message, "Plotting Boundary", MessageBoxButtons.OK, MessageBoxIcon.Error);
                ExitCmd();//log.CloseFile();
            }
        }
        #endregion

    }
}
