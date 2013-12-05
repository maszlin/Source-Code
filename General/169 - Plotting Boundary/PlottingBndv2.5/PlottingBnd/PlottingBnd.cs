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
using Excel = Microsoft.Office.Interop.Excel;
using System.IO;
using Microsoft.Win32;

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

        int plotCount = 1;

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

                            string[] formData = new string[7];
                            formData[0] = m_PlotBoundaryForm.Desc1;
                            formData[1] = m_PlotBoundaryForm.Desc2;
                            formData[2] = m_PlotBoundaryForm.Desc3;
                            formData[3] = m_PlotBoundaryForm.Desc4;
                            formData[4] = m_PlotBoundaryForm.PrepareBy;
                            formData[5] = m_PlotBoundaryForm.RevisedBy;
                            formData[6] = m_PlotBoundaryForm.ApprovedBy;

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
                               
                                PPlottingLib.StartDrawingRedlines(oPlot, plotCount, Boundaries.Count, formData);
                                plotCount++;
                                plotnum++;

                                //Vinod Apr-2013
                                GenerateMIS(oGTPlotWindow);
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

        IGTPlotWindow oPlotWin;

        private void GenerateMIS(IGTPlotWindow oGTPlotWindow)
        {
            try
            {
                string genName = "";                

                if (m_PlotBoundaryForm.m_rbNewPlot == true)
                {
                    genName = "NewPlotPage1";
                    GTClassFactory.Create<IGTApplication>().Application.NamedPlots.Remove(genName);
                    IGTNamedPlot oNamedPlot = GTClassFactory.Create<IGTApplication>().Application.NewNamedPlot(genName);
                    oPlotWin = GTClassFactory.Create<IGTApplication>().Application.NewPlotWindow(oNamedPlot);
                }
                else if (m_PlotBoundaryForm.m_rbExtPlot == true)
                {
                    //IGTNamedPlots oNamedPlots = GTClassFactory.Create<IGTApplication>().Application.NamedPlots;
                    //foreach (IGTNamedPlot plotName in oNamedPlots)
                    //{
                    //    if (plotName.Name == cmbPlotWindow.Text)
                    //    {
                    //        IGTNamedPlot oNamedPlot = GTClassFactory.Create<IGTApplication>().Application.NamedPlots.Add(plotName);
                    //    }
                    //}

                    //IGTPlotWindows oExtPlot = GTClassFactory.Create<IGTApplication>().Application.GetPlotWindows();
                    oPlotWin = oGTPlotWindow; // FindPlotWindow(oExtPlot, cmbPlotWindow.Text);
                }

                PlotBoundary oPlotBnd = new PlotBoundary();
                oPlotBnd.Name = genName;
                oPlotBnd.Type = "Plot MIS";

                //_pnt = GetFirstPoint();
                //IGTPoint oTmpPoint = GTClassFactory.Create<IGTPoint>();
                //oTmpPoint.X = _pnt.X;
                //oTmpPoint.Y = _pnt.Y;

                //IGTPlotRedline oGTPlotRedline = null;
                //IGTPoint oTitlePoint = GTClassFactory.Create<IGTPoint>();
                //oTitlePoint = oTmpPoint;
                //oTitlePoint.X = oTitlePoint.X + 8000;
                //oTitlePoint.Y = oTitlePoint.Y - 4000;
                //IGTTextPointGeometry oTitleGeom = GTClassFactory.Create<IGTTextPointGeometry>();
                //oTitleGeom.Origin = oTitlePoint;
                //oTitleGeom.Rotation = 0;
                //oTitleGeom.Alignment = GTAlignmentConstants.gtalBottomCenter;
                //oTitleGeom.Text = lblPPWO.Text;
                //oGTPlotRedline = oPlotWin.NamedPlot.NewRedline(oTitleGeom, 5036);
                
                IGTPoint oPoint1 = GTClassFactory.Create<IGTPoint>();
                oPoint1.X = PPlottingLib.mMISX_Pnt + 500;
                oPoint1.Y = PPlottingLib.mMISY_Pnt - 10000;

                //Excel Generation
                //GenerateExcel();

                IGTPlotFrame _PlotFrame = null;

                _PlotFrame = oPlotWin.InsertObjectFromFile(GenerateExcel(), false, oPoint1);

                //IGTPlotRedline oRedline = null;
                //IGTSymbology oSym = null;
                //oRedline = _PlotFrame.BorderRedline;
                //oSym = oRedline.Symbology;
                //oSym.Color = GTStyleColorConstants.gtcolTransparent;
                //oRedline.Symbology = oSym;

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Plot MIS", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public string GenerateExcel()
        {
            string strFile = "";
            string FileName = "";
            string Exch = null;
            string sSql = null;
            ADODB.Recordset rs = new ADODB.Recordset();
            string JOBID = null;

            log.WriteLog("Plot MIS Log -- START");

            if (m_PlotBoundaryForm.m_PlotType == "Civil")
            {
                FileName = "NEPS_Civil.xlt";
            }
            else if (m_PlotBoundaryForm.m_PlotType == "Copper ESIDE")
            {
                FileName = "NEPS_E-Side.xlt";
            }
            else if (m_PlotBoundaryForm.m_PlotType == "HSBB DSIDE")
            {
                FileName = "NEPS_D-Side-HSBB.xlt";
            }
            else if (m_PlotBoundaryForm.m_PlotType == "Copper DSIDE")
            {
                FileName = "NEPS_D-Side.xlt";
            }
            else if (m_PlotBoundaryForm.m_PlotType == "Fibre")
            {
                FileName = "NEPS_Fiber.xlt";
            }

            log.WriteLog("Plot Type : " + m_PlotBoundaryForm.m_PlotType);

            Excel.Application _ExcelApp = new Excel.Application();
            Excel.Workbooks workbooks = _ExcelApp.Workbooks;
            Excel.Workbook newWorkbook;

            try
            {
                _ExcelApp.Visible = false;
                if (!File.Exists(GetPath() + "Program\\" + FileName))
                {
                    newWorkbook = _ExcelApp.Workbooks.Add(Excel.XlWBATemplate.xlWBATWorksheet);
                }
                else
                {
                    newWorkbook = _ExcelApp.Workbooks.Open(GetPath() + "Program\\" + FileName,
                        Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing,
                        Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
                }

                JOBID = m_PlotBoundaryForm.m_JobID;
                Exch = m_PlotBoundaryForm.m_JobID.Substring(0, 3);

                log.WriteLog("JOB ID : " + JOBID);
                log.WriteLog("");

                if (m_PlotBoundaryForm.m_PlotType == "Civil")
                {
                    string DUCT_KM = Get_Value("select sum(total_length)/1000 from gc_cond where g3e_fid in (select g3e_fid from gc_netelem where g3e_fno = 2200 and EXC_ABB = '" + Exch + "')");
                    string TOT_MH = Get_Value("select count(*) from gc_manhl where g3e_fid in (select g3e_fid from gc_netelem where g3e_fno = 2700 and EXC_ABB = '" + Exch + "')");
                    string POLE_D = Get_Value("select count(*) from gc_pole where g3e_fid in (select g3e_fid from gc_netelem where g3e_fno = 3000 and POLE_USAGE = 'DISTRIBUTION' and EXC_ABB = '" + Exch + "')");
                    string POLE_I = Get_Value("select count(*) from gc_pole where g3e_fid in (select g3e_fid from gc_netelem where g3e_fno = 3000 and POLE_USAGE = 'INTERMEDIATE' and EXC_ABB = '" + Exch + "')");

                    _ExcelApp.Cells[2, 4] = Exch;
                    _ExcelApp.Cells[3, 4] = DUCT_KM;
                    _ExcelApp.Cells[4, 4] = TOT_MH;
                    _ExcelApp.Cells[5, 4] = POLE_D;
                    _ExcelApp.Cells[6, 4] = POLE_I;

                    int j = 9;
                    sSql = "select rownum, DT_WAYS, (select dt_s_type from GC_CONDEXPST where g3e_fid = c.g3e_fid) as type, total_length, total_length/1000 as KM   from gc_cond C where c.g3e_fid in (select g3e_fid from gc_netelem where g3e_fno = 2200 and job_id = '" + JOBID + "')";
                    log.WriteLog(sSql);
                    log.WriteLog("");
                    rs = GTClassFactory.Create<IGTApplication>().DataContext.OpenRecordset(sSql, ADODB.CursorTypeEnum.adOpenStatic, ADODB.LockTypeEnum.adLockReadOnly, 1);
                    if (rs.RecordCount > 0)
                    {
                        rs.MoveFirst();
                        while (!rs.EOF)
                        {
                            _ExcelApp.Cells[j, 1] = rs.Fields["rownum"].Value.ToString();
                            if (rs.Fields["type"].Value.ToString() != "")
                                _ExcelApp.Cells[j, 2] = rs.Fields["DT_WAYS"].Value.ToString() + "/" + rs.Fields["type"].Value.ToString();
                            else
                                _ExcelApp.Cells[j, 2] = rs.Fields["DT_WAYS"].Value.ToString();
                            _ExcelApp.Cells[j, 3] = rs.Fields["total_length"].Value.ToString();
                            _ExcelApp.Cells[j, 4] = rs.Fields["KM"].Value.ToString();
                            rs.MoveNext();
                            j++;
                        }
                    }
                    rs.Close();
                }
                else if (m_PlotBoundaryForm.m_PlotType == "Copper ESIDE")
                {
                    string CABLE_CODE = Get_Value("select distinct CABLE_CODE from gc_cbl where CABLE_CLASS = 'E-CABLE' and g3e_fid in (select g3e_fid from gc_netelem where g3e_fno = 7000 and JOB_ID = '" + JOBID + "')");
                    string TOT_KM = Get_Value("select sum(total_length/1000) as TOT_SHE from gc_cbl where CABLE_CLASS = 'E-CABLE' and g3e_fid in (select g3e_fid from gc_netelem where g3e_fno = 7000 and JOB_ID = '" + JOBID + "')");
                    string TOT_SHE = Get_Value("select sum((copper_size*total_length)/1000) as TOT_KM from gc_cbl where CABLE_CLASS = 'E-CABLE' and g3e_fid in (select g3e_fid from gc_netelem where g3e_fno = 7000 and JOB_ID = '" + JOBID + "')");
                    string EFF_PAIR = Get_Value("select sum(EFFECTIVE_PAIRS) from gc_cbl where CABLE_CLASS = 'E-CABLE' and g3e_fid in (select g3e_fid from gc_netelem where g3e_fno = 7000 and JOB_ID = '" + JOBID + "')");
                    string TPE = Get_Value("SELECT Sum(DISTINCT TOTAL_SIZE) FROM GC_CBL A, GC_NETELEM B WHERE A.G3E_FID=B.G3E_FID AND A.CABLE_CODE='" + CABLE_CODE + "'  AND  B.JOB_ID = '" + JOBID + "'");
                    string SDF_DDP = Get_Value("SELECT (SELECT COUNT(*) FROM GC_ITFACE A, GC_NETELEM B WHERE A.G3E_FID=B.G3E_FID AND B.JOB_ID = '" + JOBID + "') + (SELECT COUNT(*) FROM GC_DDP A, GC_NETELEM B WHERE A.G3E_FID=B.G3E_FID AND B.JOB_ID = '" + JOBID + "') FROM DUAL");
                    string PAIR_CAB = Get_Value("SELECT SUM(E_CAPACITY+D_CAPACITY) FROM GC_ITFACE A, GC_NETELEM B WHERE A.G3E_FID=B.G3E_FID AND  B.JOB_ID = '" + JOBID + "'");
                    
                    _ExcelApp.Cells[2, 4] = CABLE_CODE;
                    _ExcelApp.Cells[3, 4] = Exch;
                    _ExcelApp.Cells[4, 4] = TPE;
                    _ExcelApp.Cells[5, 4] = EFF_PAIR;
                    _ExcelApp.Cells[6, 4] = "-";
                    _ExcelApp.Cells[7, 4] = TOT_KM;
                    _ExcelApp.Cells[8, 4] = TOT_SHE;
                    _ExcelApp.Cells[9, 4] = SDF_DDP;
                    _ExcelApp.Cells[10, 4] = PAIR_CAB;
                    _ExcelApp.Cells[11, 4] = "-";
                    _ExcelApp.Cells[12, 4] = "-";

                    int j = 15;
                    sSql = "select rownum, copper_size, ctype, total_length, (copper_size*total_length)/1000 as KM from gc_cbl where CABLE_CLASS = 'E-CABLE' and g3e_fid in (select g3e_fid from gc_netelem where g3e_fno = 7000 and JOB_ID = '" + JOBID + "')";
                    log.WriteLog(sSql);
                    log.WriteLog("");
                    rs = GTClassFactory.Create<IGTApplication>().DataContext.OpenRecordset(sSql, ADODB.CursorTypeEnum.adOpenStatic, ADODB.LockTypeEnum.adLockReadOnly, 1);
                    if (rs.RecordCount > 0)
                    {
                        rs.MoveFirst();
                        while (!rs.EOF)
                        {
                            _ExcelApp.Cells[j, 1] = rs.Fields["rownum"].Value.ToString();
                            _ExcelApp.Cells[j, 2] = rs.Fields["cable_size"].Value.ToString() + "/" + rs.Fields["ctype"].Value.ToString();
                            _ExcelApp.Cells[j, 3] = rs.Fields["total_length"].Value.ToString();
                            _ExcelApp.Cells[j, 4] = rs.Fields["KM"].Value.ToString();
                            rs.MoveNext();
                            j++;
                        }
                    }
                    rs.Close();
                }
                else if (m_PlotBoundaryForm.m_PlotType == "Copper DSIDE")
                {
                    string TOT_KM = Get_Value("select sum(total_length/1000) as TOT_SHE from gc_cbl where CABLE_CLASS = 'D-CABLE' and g3e_fid in (select g3e_fid from gc_netelem where g3e_fno = 7000 and JOB_ID = '" + JOBID + "')");
                    string TOT_SHE = Get_Value("select sum((copper_size*total_length)/1000) as TOT_KM from gc_cbl where CABLE_CLASS = 'D-CABLE' and g3e_fid in (select g3e_fid from gc_netelem where g3e_fno = 7000 and JOB_ID = '" + JOBID + "')");
                    string EPAIR_CAB = Get_Value("SELECT E_CAPACITY FROM GC_ITFACE A, GC_NETELEM B WHERE A.G3E_FID=B.G3E_FID AND  B.JOB_ID = '" + JOBID + "'");
                    string DPAIR_CAB = Get_Value("SELECT D_CAPACITY FROM GC_ITFACE A, GC_NETELEM B WHERE A.G3E_FID=B.G3E_FID AND  B.JOB_ID = '" + JOBID + "'");
                    string TOT_DP = Get_Value("SELECT COUNT(*) FROM GC_DP A, GC_NETELEM B WHERE A.G3E_FID=B.G3E_FID AND  B.JOB_ID ='" + JOBID + "'");
                    string TOT_DPOLE = Get_Value("SELECT COUNT(*) FROM GC_POLE  A, GC_NETELEM B WHERE A.G3E_FID=B.G3E_FID AND  B.JOB_ID = '" + JOBID + "' AND A.POLE_USAGE='DISTRIBUTION'");
                    string TOT_IPOLE = Get_Value("SELECT COUNT(*) FROM GC_POLE  A, GC_NETELEM B WHERE A.G3E_FID=B.G3E_FID AND  B.JOB_ID = '" + JOBID + "' AND A.POLE_USAGE='INTERMEDIATE'");

                    _ExcelApp.Cells[2, 4] = Exch;
                    _ExcelApp.Cells[3, 4] = EPAIR_CAB;
                    _ExcelApp.Cells[4, 4] = DPAIR_CAB;
                    _ExcelApp.Cells[5, 4] = TOT_KM;
                    _ExcelApp.Cells[6, 4] = TOT_SHE;
                    _ExcelApp.Cells[7, 4] = TOT_DP;
                    _ExcelApp.Cells[8, 4] = TOT_DPOLE;
                    _ExcelApp.Cells[9, 4] = TOT_IPOLE;
                    _ExcelApp.Cells[10, 4] = "-";

                    int j = 13;
                    sSql = "select rownum, copper_size, ctype, total_length, (copper_size*total_length)/1000 as KM from gc_cbl where CABLE_CLASS = 'D-CABLE' and g3e_fid in (select g3e_fid from gc_netelem where g3e_fno = 7000 and JOB_ID = '" + JOBID + "')";
                    log.WriteLog(sSql);
                    log.WriteLog("");
                    rs = GTClassFactory.Create<IGTApplication>().DataContext.OpenRecordset(sSql, ADODB.CursorTypeEnum.adOpenStatic, ADODB.LockTypeEnum.adLockReadOnly, 1);
                    if (rs.RecordCount > 0)
                    {
                        rs.MoveFirst();
                        while (!rs.EOF)
                        {
                            _ExcelApp.Cells[j, 1] = rs.Fields["rownum"].Value.ToString();
                            _ExcelApp.Cells[j, 2] = rs.Fields["copper_size"].Value.ToString() + "/" + rs.Fields["ctype"].Value.ToString();
                            _ExcelApp.Cells[j, 3] = rs.Fields["total_length"].Value.ToString();
                            _ExcelApp.Cells[j, 4] = rs.Fields["KM"].Value.ToString();
                            rs.MoveNext();
                            j++;
                        }
                    }
                    rs.Close();
                }
                else if (m_PlotBoundaryForm.m_PlotType == "HSBB DSIDE")
                {
                    string TOT_KM = Get_Value("select SUM((cable_size*cable_length/1000)) as TOT_KM from GC_FDCBL where  g3e_fid in (select g3e_fid from gc_netelem where g3e_fno = 7400 and JOB_ID = '" + JOBID + "')");
                    string TOT_SHE = Get_Value("select sum (cable_length/1000) as TOT_SHE from GC_FDCBL where  g3e_fid in (select g3e_fid from gc_netelem where g3e_fno = 7400 and JOB_ID = '" + JOBID + "')");
                    string TOT_FDP = Get_Value("select count(g3e_fid) from gc_netelem where g3e_fno = 5600 and JOB_ID = '" + JOBID + "'");
                    string TOT_FTB = Get_Value("select count(g3e_fid) from gc_netelem where g3e_fno = 5900 and JOB_ID = '" + JOBID + "'");
                    double TOT_DCORE_FDC = 0;
                    if (TOT_KM != "")
                        TOT_DCORE_FDC = Convert.ToDouble(TOT_KM) + Convert.ToDouble(TOT_FDP);
                    else
                        TOT_DCORE_FDC = Convert.ToInt32(TOT_FDP);


                    _ExcelApp.Cells[2, 4] = Exch;
                    _ExcelApp.Cells[3, 4] = "-";
                    _ExcelApp.Cells[4, 4] = "-";
                    _ExcelApp.Cells[5, 4] = TOT_DCORE_FDC;
                    _ExcelApp.Cells[6, 4] = TOT_KM;
                    _ExcelApp.Cells[7, 4] = TOT_SHE;
                    _ExcelApp.Cells[8, 4] = TOT_FDP;
                    _ExcelApp.Cells[9, 4] = TOT_FTB;

                    int j = 12;
                    sSql = "select rownum, cable_size, cable_type, cable_length, (cable_size*cable_length/1000) as KM from GC_FDCBL where  g3e_fid in (select g3e_fid from gc_netelem where g3e_fno = 7400 and JOB_ID = '" + JOBID + "')";
                    log.WriteLog(sSql);
                    log.WriteLog("");
                    rs = GTClassFactory.Create<IGTApplication>().DataContext.OpenRecordset(sSql, ADODB.CursorTypeEnum.adOpenStatic, ADODB.LockTypeEnum.adLockReadOnly, 1);
                    if (rs.RecordCount > 0)
                    {
                        rs.MoveFirst();
                        while (!rs.EOF)
                        {
                            _ExcelApp.Cells[j, 1] = rs.Fields["rownum"].Value.ToString();
                            _ExcelApp.Cells[j, 2] = rs.Fields["cable_size"].Value.ToString() + "/" + rs.Fields["cable_type"].Value.ToString();
                            _ExcelApp.Cells[j, 3] = rs.Fields["cable_length"].Value.ToString();
                            _ExcelApp.Cells[j, 4] = rs.Fields["KM"].Value.ToString();
                            rs.MoveNext();
                            j++;
                        }
                    }
                    rs.Close();
                }
                else if (m_PlotBoundaryForm.m_PlotType == "Fibre")
                {
                    string CABLE_CODE = Get_Value("select distinct CABLE_CODE from gc_fcbl where g3e_fid in (select g3e_fid from gc_netelem where JOB_ID = '" + JOBID + "')");
                    string TOT_FIB_KM = Get_Value("select SUM((cable_size * cable_length)/1000) as KM  from gc_fcbl where g3e_fid in (select g3e_fid from gc_netelem where g3e_fno = 7200 and JOB_ID = '" + JOBID + "')");
                    string TOT_SHE_KM = Get_Value("select sum(cable_length/1000) from gc_fcbl where g3e_fid in (select g3e_fid from gc_netelem where g3e_fno = 7200 and JOB_ID = '" + JOBID + "')");
                    string TOT_RT = Get_Value("select count(g3e_fid) from gc_netelem where g3e_fno = 9600 and JOB_ID = '" + JOBID + "'");
                    string TOT_2MB = Get_Value("SELECT A.CHL_QUANTUM FROM GC_RT A, GC_NETELEM B WHERE A.G3E_FID=B.G3E_FID AND B.JOB_ID= '" + JOBID + "' AND (B.FEATURE_STATE='PAD' OR B.FEATURE_STATE='ASB')");

                    _ExcelApp.Cells[2, 4] = CABLE_CODE;
                    _ExcelApp.Cells[3, 4] = Exch;
                    _ExcelApp.Cells[4, 4] = "-";
                    _ExcelApp.Cells[5, 4] = "-";
                    _ExcelApp.Cells[6, 4] = TOT_2MB;
                    _ExcelApp.Cells[7, 4] = TOT_FIB_KM;
                    _ExcelApp.Cells[8, 4] = TOT_SHE_KM;
                    _ExcelApp.Cells[9, 4] = TOT_RT;
                    _ExcelApp.Cells[10, 4] = "-";

                    int j = 13;
                    sSql = "select rownum, cable_size, cable_type, cable_length, (cable_size * cable_length)/1000 as KM  from gc_fcbl where g3e_fid in (select g3e_fid from gc_netelem where g3e_fno = 7200 and JOB_ID = '" + JOBID + "')";
                    log.WriteLog(sSql);
                    log.WriteLog("");
                    rs = GTClassFactory.Create<IGTApplication>().DataContext.OpenRecordset(sSql, ADODB.CursorTypeEnum.adOpenStatic, ADODB.LockTypeEnum.adLockReadOnly, 1);
                    if (rs.RecordCount > 0)
                    {
                        rs.MoveFirst();
                        while (!rs.EOF)
                        {
                            _ExcelApp.Cells[j, 1] = rs.Fields["rownum"].Value.ToString();
                            _ExcelApp.Cells[j, 2] = rs.Fields["cable_size"].Value.ToString() + "/" + rs.Fields["cable_type"].Value.ToString();
                            _ExcelApp.Cells[j, 3] = rs.Fields["cable_length"].Value.ToString();
                            _ExcelApp.Cells[j, 4] = rs.Fields["KM"].Value.ToString();
                            rs.MoveNext();
                            j++;
                        }
                    }
                    rs.Close();
                }

                for (int i = 0; i < 100; i++)
                {
                    strFile = Path.GetTempPath() + "Table" + String.Format("{0:00}", i) + ".xls";
                    try
                    {
                        if (File.Exists(strFile)) File.Delete(strFile);
                    }
                    catch { };
                    if (!File.Exists(strFile)) break;
                }

                _ExcelApp.ActiveWorkbook.SaveAs(strFile,
                    Excel.XlFileFormat.xlExcel7, Type.Missing,
                   Type.Missing, Type.Missing, Type.Missing,
                   Excel.XlSaveAsAccessMode.xlNoChange, Type.Missing,
                    Type.Missing, Type.Missing, Type.Missing, Type.Missing);

                log.WriteLog("Plot MIS Log -- END");

                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();
                GC.WaitForPendingFinalizers();
                newWorkbook.Close(false, Type.Missing, Type.Missing);
                System.Runtime.InteropServices.Marshal.FinalReleaseComObject(newWorkbook);
                newWorkbook = null;
                workbooks.Close();
                System.Runtime.InteropServices.Marshal.FinalReleaseComObject(workbooks);
                workbooks = null;
                _ExcelApp.DisplayAlerts = false;
                _ExcelApp.Quit();
                System.Runtime.InteropServices.Marshal.FinalReleaseComObject(_ExcelApp);
                _ExcelApp = null;

                return strFile;

            }
            catch (Exception ex)
            {
                _ExcelApp.Quit();
                return "";
            }
        }

        private static string GetPath()
        {
            RegistryKey pRegKey = Registry.LocalMachine;
            pRegKey = pRegKey.OpenSubKey("SOFTWARE\\Intergraph\\GFramme\\02.00\\Path");
            string val = pRegKey.GetValue("").ToString();

            return val;
        }

        //Get Value from Database
        private string Get_Value(string sSql)
        {
            try
            {
                log.WriteLog(sSql);
                log.WriteLog("");

                ADODB.Recordset rsPP = new ADODB.Recordset();
                rsPP = GTClassFactory.Create<IGTApplication>().DataContext.OpenRecordset(sSql, ADODB.CursorTypeEnum.adOpenStatic, ADODB.LockTypeEnum.adLockReadOnly, 1);
                if (rsPP.RecordCount > 0)
                {
                    rsPP.MoveFirst();
                    return (rsPP.Fields[0].Value.ToString());
                }
                return null;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Plot MIS", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
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
