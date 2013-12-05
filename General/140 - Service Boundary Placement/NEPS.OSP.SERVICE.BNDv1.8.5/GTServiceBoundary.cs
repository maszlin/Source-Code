using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Intergraph.GTechnology.Interfaces;
using Intergraph.GTechnology.API;
using ADODB;

namespace NEPS.OSP.SERVICE.BND
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


    class GTServiceBoundary : Intergraph.GTechnology.Interfaces.IGTCustomCommandModeless
    {
        public static Intergraph.GTechnology.API.IGTTransactionManager m_oGTTransactionManager = null;
        public static Intergraph.GTechnology.API.IGTCustomCommandHelper m_oGTCustomCommandHelper = null;

        public static WinWrapper objWinWrapper = new WinWrapper();

        //public GisLib objGisLib = null;
        public IGTPoint oPoint = null;
        public bool bBeginSelect;
       // private string sFNOFilter;

        GTWindowsForm_Service_Boundary m_CustomForm = null;
        
        #region Event Handlers
        
        private static Intergraph.GTechnology.API.IGTApplication application = GTClassFactory.Create<IGTApplication>();
        private IGTDataContext oDataContext = application.DataContext;
        private static IGTPolylineGeometry oLineGeom = null;
        private static List<IGTGeometry> arrIntersection = new List<IGTGeometry>();
        private static List<IGTGeometry> arrIntersectionText = new List<IGTGeometry>();
        private int lStyleId = 24613162;
        double dRotation;

        IGTPoint mptSelected;
        IGTPoint mptPoint1;
        IGTPoint mptPoint2;
        double _distance = 0;

        double xPoint = 0;
        double yPoint = 0;

        double xPrePoint = 0;
        double yPrePoint = 0;

        double xLinePoint = 0;
        double yLinePoint = 0;

        public static IGTGeometryEditService mobjEditServiceTemp;
        public static IGTGeometryEditService mobjEditServiceLine;
        public static IGTGeometryEditService mobjEditService;
        public static IGTGeometryEditService mobjEditServiceBound;

        private static List<IGTPoint> arrPoint = new List<IGTPoint>();
        bool editmode; // set flag to false identifies user selected Zoom, Pan or Rotate


        /// <summary>
        /// As mouse move we draw dynamic Boundary geometry to guide user the area they are covering.
        /// If single points, only draw a line to mark between the first line and current mouse position
        /// when we have more points, draw a poligon for all marked point including current mouse position as last point
        /// 
        /// * originally coded by others -> draw a dynamic line from last point to current mouse position
        /// - edited by : m.zam on 14-march-2012 *** added the above mention functions
        /// 
        /// </summary>
        /// <param name="sender">object sender</param>
        /// <param name="e">GTMouseEventArgs</param>
        void m_oGTCustomCommandHelper_MouseMove(object sender, GTMouseEventArgs e)
        {
                
            // create dynamic temporary line as mouse move
            IGTPoint WorldPoint = e.WorldPoint;

            if ((GTClassFactory.Create<IGTApplication>().Application.ActiveMapWindow.MousePointer == GTMousePointerConstants.gtmwmpZoomIn) ||
               (GTClassFactory.Create<IGTApplication>().Application.ActiveMapWindow.MousePointer == GTMousePointerConstants.gtmwmpZoomOut) ||
               (GTClassFactory.Create<IGTApplication>().Application.ActiveMapWindow.MousePointer == GTMousePointerConstants.gtmwmpPan) ||
               (GTClassFactory.Create<IGTApplication>().Application.ActiveMapWindow.MousePointer == GTMousePointerConstants.gtmwmpRotate))
            {
                editmode = false;
            }
            else if (m_CustomForm.EditFlag || m_CustomForm.CopyFlag)
            {
                editmode = true;
                GTClassFactory.Create<IGTApplication>().Application.ActiveMapWindow.MousePointer = GTMousePointerConstants.gtmwmpCrossHair;
            }


            if (mobjEditService.GeometryCount > 0 && m_CustomForm.EditFlag == true)
            {
                GTClassFactory.Create<IGTApplication>().SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Click to Draw Boundary. Press F10 or Double Click to Place Boundary.");

                List<IGTPoint> arrTempPoint = new List<IGTPoint>();
                double X = WorldPoint.X;
                double Y = WorldPoint.Y;

                xPrePoint = xPoint;
                yPrePoint = yPoint;
                xLinePoint = X;
                yLinePoint = Y;                

                // create polygon on temporary edit.service as mouse move
                if ((mobjEditService.GeometryCount > 2))
                {
                    if (mobjEditService.GeometryCount > 0) mobjEditServiceTemp.RemoveAllGeometries();

                    for (int i = 0; i < arrPoint.Count; i++)
                    {
                        arrTempPoint.Add(arrPoint[i]);
                    }
                    arrTempPoint.Add(WorldPoint);

                    IGTPolygonGeometry oPolygon = PGeoLib.CreatePolyGeom(arrTempPoint);
                    mobjEditServiceTemp.AddGeometry(oPolygon, 351001);
                }
                else if (mobjEditService.GeometryCount > 0)
                {
                    IGTLineGeometry oLine = PGeoLib.CreateLineGeom(xPoint, yPoint, X, Y);//(xPrePoint, yPrePoint, xLinePoint, yLinePoint);
                    if (mobjEditServiceTemp.GeometryCount > 0) mobjEditServiceTemp.RemoveAllGeometries();
                    mobjEditServiceTemp.AddGeometry(oLine, 351001); //351001);
                }
            }

            else if (m_CustomForm.CopyFlag == true)
            {
                GTClassFactory.Create<IGTApplication>().SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Move the mouse to move boundary. Click to Place Boundary.");

                List<IGTPoint> PointCol = new List<IGTPoint>();

                for (int i = 0; i < m_CustomForm.oCopyBoundary.KeypointCount; i++)
                    PointCol.Add(m_CustomForm.oCopyBoundary.GetKeypointPosition(i));

                IGTPolygonGeometry oPoly = PGeoLib.CreatePolygonGeom(PointCol);
                if (mobjEditServiceBound.GeometryCount > 0) mobjEditServiceBound.RemoveAllGeometries();
                mobjEditServiceBound.AddGeometry(oPoly, 30391709);

                IGTPoint objPointTmp = GTClassFactory.Create<IGTPoint>();
                objPointTmp.X = WorldPoint.X;
                objPointTmp.Y = WorldPoint.Y;

                mobjEditServiceBound.BeginMove(m_CustomForm.oCopyBoundary.GetKeypointPosition(0));
                mobjEditServiceBound.EndMove(objPointTmp); 
            }            
        }

        /// <summary>
        /// KeyUp event - monitor if user pressed ESC and Backspace key
        /// ESC : cancel Boundary operation, delete everything from memory
        /// Backspace : only delete last Boundary point, redraw geometry after deletion.
        /// 
        /// edited by : m.zam on 14-mac-2012
        /// </summary>
        /// <param name="sender">sender object</param>
        /// <param name="e">GTKeyEventArgs</param>
        void m_oGTCustomCommandHelper_MouseUp(object sender, GTMouseEventArgs e)
        {
            GTClassFactory.Create<IGTApplication>().SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Click to Draw Boundary. Press F10 to Place Boundary.");

            if (e.Button == 1 && (m_CustomForm != null) && editmode) // right button click
            {
                // To Get Points  
                oPoint = e.WorldPoint;
                if ( m_CustomForm.EditFlag)
                {

                    IGTOrientedPointGeometry oOrPointGeom = null;
                    IGTPoint objPoint;
                    IGTVector objVector;

                    //****** create a point geometry
                    objPoint = GTClassFactory.Create<IGTPoint>();
                    objPoint.X = oPoint.X;
                    objPoint.Y = oPoint.Y;
                    oOrPointGeom = GTClassFactory.Create<IGTOrientedPointGeometry>();
                    //  Offset
                    oOrPointGeom.Origin = objPoint;
                    //  Radians
                    dRotation = 0;
                    objVector = GTClassFactory.Create<IGTVector>();
                    objVector.I = Math.Cos(dRotation);
                    objVector.J = Math.Sin(dRotation);
                    objVector.K = 0.0;
                    oOrPointGeom.Orientation = objVector;

                    xPoint = oOrPointGeom.FirstPoint.X;
                    yPoint = oOrPointGeom.FirstPoint.Y;
                    //****** Add geometry into service to show
                    mobjEditService.AddGeometry(oOrPointGeom, lStyleId);

                    // ******* added by : m.zam 13-mac-201 **********
                    // arrPoint hold Boundary point of the Boundary - use to draw polygon as the Boundary marked

                    arrPoint.Add(oPoint);
                    if ((mobjEditService.GeometryCount > 2))
                    {
                        // after 2 Boundary points no need to draw any more lines as polygon is draw as mouse move
                        if (mobjEditServiceLine.GeometryCount > 0) mobjEditServiceLine.RemoveAllGeometries();
                    }
                    else if (mobjEditService.GeometryCount > 1)
                    {
                        // draw lines on temp.edit.service to show mark between 2 point
                        IGTLineGeometry oLine = PGeoLib.CreateLineGeom(xPrePoint, yPrePoint, xLinePoint, yLinePoint);
                        mobjEditServiceLine.AddGeometry(oLine, 351001); // 1410001
                    }
                    // ******* completed by : m.zam 14-mac-201 **********
                }
                else if (m_CustomForm.CopyFlag )
                {
                    m_CustomForm.CopyFlag = false;
                    List<IGTPoint> PointCol = new List<IGTPoint>();
                    IGTPoint objPoint = GTClassFactory.Create<IGTPoint>();
                    if (mobjEditServiceBound.GeometryCount >0)
                    {
                        for (int i = 0; i <= mobjEditServiceBound.GetGeometry(1).KeypointCount-1 ; i++)
                        {
                            PointCol.Add(mobjEditServiceBound.GetGeometry(1).GetKeypointPosition(i));
                        }
                        //To Start and End Point are same.
                        //PointCol.Add((IGTPoint)mobjEditServiceBound.GetGeometry(1).FirstPoint);
                    }

                    m_CustomForm.DrawBND = false;
                    m_CustomForm.CopyBND = true;
                    GTClassFactory.Create<IGTApplication>().BeginWaitCursor();
                    m_CustomForm.DrawAreaBoundary(24000, PointCol);
                    GTClassFactory.Create<IGTApplication>().EndWaitCursor();
                    //ExitCmd();
                }

            }
            else if (e.Button == 2)            
            {
                
            }            

        }              

        void m_oGTCustomCommandHelper_KeyUp(object sender, GTKeyEventArgs e)
        {
            GTClassFactory.Create<IGTApplication>().SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Click to Draw Boundary. Press F10 or Double Click to Place Boundary.");

            if (e.KeyCode == 27) // ESC key - cancel add Boundary
            {
                m_CustomForm.CopyFlag = false;
                m_CustomForm.EditFlag = false;
                if (mobjEditService.GeometryCount > 0) mobjEditService.RemoveAllGeometries();
                if (mobjEditServiceLine.GeometryCount > 0) mobjEditServiceLine.RemoveAllGeometries();
                if (mobjEditServiceTemp.GeometryCount > 0) mobjEditServiceTemp.RemoveAllGeometries();
                if (mobjEditServiceBound.GeometryCount > 0) mobjEditServiceBound.RemoveAllGeometries();
                
                arrPoint.Clear();

                xPoint = 0;
                yPoint = 0;
                xPrePoint = 0;
                yPrePoint = 0;
                xLinePoint = 0;
                yLinePoint = 0;
            }
            else if (e.KeyCode == 8) // Backspace key - delete a point
            {
                if (mobjEditService.GeometryCount > 0)
                {
                    //only remove lastes geometry point
                    mobjEditService.RemoveGeometry(mobjEditService.GeometryCount);
                    // clear geometry drawing as we going to redraw it again later
                    if (mobjEditServiceLine.GeometryCount > 0) mobjEditServiceLine.RemoveGeometry(mobjEditServiceLine.GeometryCount);
                    if (mobjEditServiceTemp.GeometryCount > 0) mobjEditServiceTemp.RemoveGeometry(mobjEditServiceTemp.GeometryCount);

                    if (mobjEditService.GeometryCount > 0)
                    {
                        xPoint = mobjEditService.GetGeometry(mobjEditService.GeometryCount).FirstPoint.X;
                        yPoint = mobjEditService.GetGeometry(mobjEditService.GeometryCount).FirstPoint.Y;
                        arrPoint.RemoveAt(arrPoint.Count - 1); // remove last point as user press backspace

                        RedrawGeometry(); // redraw geometry
                    }
                    else
                    {
                        arrPoint.Clear(); // clear arrPoint as user already deleted all points.

                        xPoint = 0;
                        yPoint = 0;
                        xPrePoint = 0;
                        yPrePoint = 0;
                        xLinePoint = 0;
                        yLinePoint = 0;
                    }

                }
            }

            
            
        }

        void m_oGTCustomCommandHelper_KeyDown(object sender, GTKeyEventArgs e)
        {
            //GTClassFactory.Create<IGTApplication>().SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "KeyDown.");
            if (e.KeyCode == 121)// F10 was pressed
            {
                PlaceGeometry();
            }             
        }

        void m_oGTCustomCommandHelper_Click(object sender, GTMouseEventArgs e)
        {
            try
            {
                GTClassFactory.Create<IGTApplication>().SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Click to Draw Boundary. Press F10 to Place Boundary.");
            }
            catch (Exception e1)
            {
                throw e1;
            }
        }

        void m_oGTCustomCommandHelper_DblClick(object sender, GTMouseEventArgs e)
        {
            //GTClassFactory.Create<IGTApplication>().SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Click to Draw Boundary. Press F10 to Place Boundary.");
            PlaceGeometry();
        }
        void m_oGTCustomCommandHelper_WheelRotate(object sender, GTWheelRotateEventArgs e)
        {
            GTClassFactory.Create<IGTApplication>().SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "WheelRotate.");
        }

        void m_oGTCustomCommandHelper_KeyPress(object sender, GTKeyPressEventArgs e)
        {
            //GTClassFactory.Create<IGTApplication>().SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "KeyPress.");            
        }

        void m_oGTCustomCommandHelper_MouseDown(object sender, GTMouseEventArgs e)
        {
            GTClassFactory.Create<IGTApplication>().SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Click to Draw Boundary. Press F10 to Place Boundary.");

        }

        void m_oGTCustomCommandHelper_LostFocus(object sender, GTLostFocusEventArgs e)
        {
            GTClassFactory.Create<IGTApplication>().SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Click to Draw Boundary. Press F10 to Place Boundary.");
        }

        void m_oGTCustomCommandHelper_GainedFocus(object sender, GTGainedFocusEventArgs e)
        {
            GTClassFactory.Create<IGTApplication>().SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Click to Draw Boundary. Press F10 to Place Boundary.");
        }

        void m_oGTCustomCommandHelper_Deactivate(object sender, GTDeactivateEventArgs e)
        {
            GTClassFactory.Create<IGTApplication>().SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Click to Draw Boundary. Press F10 to Place Boundary.");
        }

        void m_oGTCustomCommandHelper_Activate(object sender, GTActivateEventArgs e)
        {
            GTClassFactory.Create<IGTApplication>().SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Click to Draw Boundary. Press F10 to Place Boundary.");
        }       
        

        #endregion

        #region IGTServiceBoundary Members

        public void Activate(Intergraph.GTechnology.API.IGTCustomCommandHelper CustomCommandHelper)
        {
            GTClassFactory.Create<IGTApplication>().SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Running Service Boundary...");

            m_oGTCustomCommandHelper = CustomCommandHelper;
            mobjEditService = GTClassFactory.Create<IGTGeometryEditService>();
            mobjEditService.TargetMapWindow = application.ActiveMapWindow;

            mobjEditServiceTemp = GTClassFactory.Create<IGTGeometryEditService>();
            mobjEditServiceTemp.TargetMapWindow = application.ActiveMapWindow;

            mobjEditServiceLine = GTClassFactory.Create<IGTGeometryEditService>();
            mobjEditServiceLine.TargetMapWindow = application.ActiveMapWindow;

            mobjEditServiceBound = GTClassFactory.Create<IGTGeometryEditService>();
            mobjEditServiceBound.TargetMapWindow = application.ActiveMapWindow;

            // added : m.zam 14-mar-2012 
            arrPoint.Clear();

            SubscribeEvents();
            m_CustomForm = new GTWindowsForm_Service_Boundary();
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

        #region "MouseEvent Un/Subscribe"
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
        #endregion

        public void ExitCmd()
        {
            GTClassFactory.Create<IGTApplication>().SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Exiting....");            
            UnsubscribeEvents();
            if (m_oGTCustomCommandHelper != null)  m_oGTCustomCommandHelper.Complete();
            GTClassFactory.Create<IGTApplication>().SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Exited.");

        }

        #region "added/edited by m.zam 14-mac-2012"
        /// <summary>
        /// RedrawGeometry will redraw the Boundary geometry on temporary edit services after 
        /// user delete last Boundary point by pressing the backspace button.
        /// The geometry can be a polygon if there a more than two points
        /// or just a line if there were only two points left.
        /// 
        /// created by : m.zam on 14-mac-2012
        /// </summary>
        void RedrawGeometry()
        {
            if (mobjEditServiceTemp.GeometryCount > 0) mobjEditServiceTemp.RemoveAllGeometries();

            if ((mobjEditService.GeometryCount > 2))
            // create polygon if we have more than two Boundary point
            {
                IGTPolygonGeometry oPolygon = PGeoLib.CreatePolyGeom(arrPoint);
                mobjEditServiceTemp.AddGeometry(oPolygon, 351001);
            }
            else if (mobjEditService.GeometryCount == 2)
            // draw line if we have only two Boundary point
            {
                IGTLineGeometry oLine = PGeoLib.CreateLineGeom(arrPoint[0].X, arrPoint[0].Y, arrPoint[1].X, arrPoint[1].Y);//(xPrePoint, yPrePoint, xLinePoint, yLinePoint);
                mobjEditServiceLine.AddGeometry(oLine, 351001);
                xPoint = arrPoint[1].X;
                yPoint = arrPoint[1].Y;
            }
        }

        void PlaceGeometry()
        {
            if (m_CustomForm.EditFlag && editmode)
            {
                List<IGTPoint> PointCol = new List<IGTPoint>();

                if (mobjEditService.GeometryCount < 3)
                {
                    MessageBox.Show("Less than 3 Points Placed, Please Left Click and Place Points to Draw Boundary.");
                    return;
                }
                else
                {
                    for (int i = 1; i <= mobjEditService.GeometryCount; i++)
                    {
                        PointCol.Add((IGTPoint)mobjEditService.GetGeometry(i).FirstPoint);
                    }

                    m_CustomForm.DrawBND = true;
                    m_CustomForm.CopyBND = false;
                    GTClassFactory.Create<IGTApplication>().BeginWaitCursor();
                    m_CustomForm.DrawAreaBoundary(24000, PointCol);

                    arrPoint.Clear();
                    GTClassFactory.Create<IGTApplication>().EndWaitCursor();
                }
                //ExitCmd();
            }
        }
        #endregion

        #region "added by m.zam 18-apr-2012"
        public static void InitBoundry() 
        {
            mobjEditService.RemoveAllGeometries();            
            mobjEditServiceLine.RemoveAllGeometries();            
            mobjEditServiceTemp.RemoveAllGeometries();
            arrPoint.Clear();
        }

        #endregion
    }
}
