using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Intergraph.GTechnology.Interfaces;
using Intergraph.GTechnology.API;
using ADODB;

namespace NEPS.GTechnology.NEPSDuctPathPlc
{
    class GTDuctPathPlc : Intergraph.GTechnology.Interfaces.IGTCustomCommandModeless
    {
        #region variables member
        public static Intergraph.GTechnology.API.IGTTransactionManager m_oIGTTransactionManager = null;
        public static Intergraph.GTechnology.API.IGTCustomCommandHelper m_oIGTCustomCommandHelper = null;
        public static Intergraph.GTechnology.API.IGTApplication m_gtapp = null;
        public static Intergraph.GTechnology.API.IGTDataContext m_IGTDataContext = null;
        public IGTPoint SelGeom = null;
        GTWindowsForm_DuctPathPlc m_CustomForm = null;
        IGTLocateService mobjLocateService = null;
        public static IGTTextPointGeometry LabelAlongLine = null;
        public static IGTTextPointGeometry oTextGeomLabel = null;
        public static int startdraw = 7;
        private int middledraw = 0;
        public static IGTPoint PointForLeaderLine = null;
        public static IGTPoint StartDrawPoint = null;
        public static IGTPoint EndDrawPoint = null;
        private int styleid = 14500;

        public static Intergraph.GTechnology.API.IGTGeometryEditService mobjEditService = null;
        public static Intergraph.GTechnology.API.IGTRelationshipService mobjRelationshipService = null;
        
        #endregion

        #region Event Handlers
      
        #region Mouse Moving
        void m_oIGTCustomCommandHelper_MouseMove(object sender, GTMouseEventArgs e)
        {
            try{

               // startdraw = 100;
                #region Get Selected Source and Termineted Devices
                if (startdraw == 100)
                {
                    #region Exit application while choosing for 2 start features
                   
                     m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Select source and terminated features (2 at once). Right Click to exit application!");

                  
                    #endregion
                    if (m_gtapp.SelectedObjects.FeatureCount == 2)                    
                    {
                        IGTGeometry geom = null;
                        short iFNO = 0;
                        int iFID = 0;

                        foreach (IGTDDCKeyObject oDDCKeyObject in m_gtapp.SelectedObjects.GetObjects())
                        {
                            if (iFID != oDDCKeyObject.FID)
                            {
                                if (iFID == 0)
                                {
                                    if (!m_CustomForm.GetSourceDevice(oDDCKeyObject.FNO, oDDCKeyObject.FID, oDDCKeyObject.Geometry))
                                    {
                                       // m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Select Source and Terminated Devices Again!");
                                        m_gtapp.SelectedObjects.Clear();
                                        return;
                                    }
                                }
                                else
                                {
                                    if (m_CustomForm.GetTermDevice(oDDCKeyObject.FNO, oDDCKeyObject.FID, oDDCKeyObject.Geometry))
                                    {
                                        startdraw = 0;
                                        m_CustomForm.LocateFeature(2, m_gtapp.ActiveMapWindow);
                                        m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "");m_CustomForm.Show();
                                        return;
                                    }
                                    else
                                    {
                                      //  m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Select Source and Terminated Devices Again!");
                                        m_gtapp.SelectedObjects.Clear();
                                        return;
                                    }

                                }
                            }
                            geom = oDDCKeyObject.Geometry;
                            iFNO = oDDCKeyObject.FNO;
                            iFID = oDDCKeyObject.FID;
                        }
                    }
                    else if (m_gtapp.SelectedObjects.FeatureCount > 2)
                    {
                        MessageBox.Show("Selected more than two features!\nSelect Source and Terminated Devices Again!", "Duct Path Placement", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    
                       // m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Selected more than two features! Select Source and Terminated Devices Again!");
                        m_gtapp.SelectedObjects.Clear();
                        return;
                    }
                    return;
                }
                #endregion 

                #region Re Select Source OR Termineted Devices
                if (startdraw == 500 || startdraw == 600)
                {
                    m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Point to select civil feature! Right Click to cancel re-selection.");

                    if (m_gtapp.SelectedObjects.FeatureCount == 1)
                    {

                        IGTGeometry geom = null;
                        short iFNO = 0;
                        int iFID = 0;
                        foreach (IGTDDCKeyObject oDDCKeyObject in m_gtapp.SelectedObjects.GetObjects())
                        {
                            geom = oDDCKeyObject.Geometry;
                            iFNO = oDDCKeyObject.FNO;
                            iFID = oDDCKeyObject.FID;
                            break;
                        }
                        //check if selected allowed feature and if succes show detail
                        
                        if ((startdraw == 500 && m_CustomForm.GetSourceDevice(iFNO, iFID, geom))//source
                            || (startdraw == 600 && m_CustomForm.GetTermDevice(iFNO, iFID, geom)))//term
                        {
                            mobjEditService.RemoveAllGeometries();
                            startdraw = 0;
                            m_CustomForm.LocateFeature(2, m_gtapp.ActiveMapWindow);
                            m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "");m_CustomForm.Show();
                        }
                        
                    }
                    return;
                }
                #endregion 
               
                #region Get Selected Source Wall OR Term Wall of Device
                if (startdraw == 300 || startdraw == 400)
                {

                    m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Point to select wall of civil feature! Right Click to cancel re-selection.");

                    if (m_gtapp.SelectedObjects.FeatureCount == 1)
                    {
                        if ((startdraw == 300 && m_CustomForm.GetDeviceWithWall(true))//source
                            || (startdraw == 400 && m_CustomForm.GetDeviceWithWall(false)))//term
                        {
                            startdraw = 0;
                            m_CustomForm.LocateFeature(2, m_gtapp.ActiveMapWindow);
                            m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "");m_CustomForm.Show();
                            return;
                        }
                        else return;
                    }

                }
                   #endregion
              
                #region For Conduit Line Placement
                if (startdraw == 1 && StartDrawPoint != null)
                {

                   m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Point to confirm point of turn! Double Click to complete drawing.");

                 //   m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "LEFT MOUSE CLICK TO CONFIRM POINT ON MAP, DOUBLE MOUSE CLICK TO FINISHED DRAWING");
                    IGTPoint Point1 = GTClassFactory.Create<IGTPoint>();
                    IGTPolylineGeometry oLineGeom = GTClassFactory.Create<IGTPolylineGeometry>();
                    Point1.X = e.WorldPoint.X;
                    Point1.Y = e.WorldPoint.Y;
                    Point1.Z = 0.0; 
                    oLineGeom.Points.Add(StartDrawPoint);
                    oLineGeom.Points.Add(Point1);
                    if (middledraw == 0)
                    {
                        if (mobjEditService.GeometryCount > 0)
                            mobjEditService.RemoveGeometry(mobjEditService.GeometryCount);// .RemoveAllGeometries();
                    }

                    mobjEditService.AddGeometry(oLineGeom, styleid);

                    if (middledraw == 1) middledraw = 0;
                    return;
                }
                if (startdraw == 1)
                {
                    m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Point to confirm point of turn! Double Click to complete drawing.");//"LEFT MOUSE CLICK TO CONFIRM POINT ON MAP, DOUBLE MOUSE CLICK TO FINISHED DRAWING");
                    return;
                }
                if (startdraw == 101)
                {
                    m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Double Click to complete drawing! Right Click to re-draw.");//"LEFT MOUSE CLICK TO CONFIRM POINT ON MAP, DOUBLE MOUSE CLICK TO FINISHED DRAWING");
                    return;
                }
                #endregion
                #region For Section Slash plc
                
                if (startdraw == 2)
                {

                    m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Point to move along line! Double Click to confirm location! Right Click to cancel slash placement.");

                    IGTOrientedPointGeometry temp = GTClassFactory.Create<IGTOrientedPointGeometry>();
                    IGTPoint tempp = GTClassFactory.Create<IGTPoint>();
                    tempp.X = e.WorldPoint.X;
                    tempp.Y = e.WorldPoint.Y;
                    tempp.Z = 0.0;
                    temp.Origin = tempp;
                    if (mobjEditService.GeometryCount > 0)
                        {
                            if (mobjEditService.GetGeometry(mobjEditService.GeometryCount).Type == "OrientedPointGeometry")
                                mobjEditService.RemoveGeometry(mobjEditService.GeometryCount);
                        }
                        mobjEditService.AddGeometry(temp, 2220004);
                        return;
                }
                 if (startdraw == 20)
                {

                    m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Point to move along line! Double Click to confirm location! Right Click to start moving follow cursor.");
                    return;
                }
                #endregion

                #region For Section Label Placement
                if (startdraw == 3)
                {

                    m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Point to confirm location! Right Click to place along line.");

                    IGTPoint Point1 = GTClassFactory.Create<IGTPoint>();
                    IGTTextPointGeometry oTextGeom = GTClassFactory.Create<IGTTextPointGeometry>();
                    Point1.X = e.WorldPoint.X;
                    Point1.Y = e.WorldPoint.Y;
                    Point1.Z = 0.0;
                    oTextGeom.Origin = Point1;
                    oTextGeom.Alignment = 0;
                    oTextGeom.Text = m_CustomForm.SectLabel;
                    if (mobjEditService.GeometryCount > 0)
                    {
                        if (mobjEditService.GetGeometry(mobjEditService.GeometryCount - 1).Type == "PolylineGeometry"
                            && mobjEditService.GeometryCount != 2)
                            mobjEditService.RemoveGeometry(mobjEditService.GeometryCount);
                        mobjEditService.RemoveGeometry(mobjEditService.GeometryCount);
                    }
                    //leader line first
                    IGTPolylineGeometry oLineGeom = GTClassFactory.Create<IGTPolylineGeometry>();
                    oLineGeom.Points.Add(LabelAlongLine.FirstPoint);
                    oLineGeom.Points.Add(Point1);
                    mobjEditService.AddGeometry(oLineGeom, 11509);//14510 14509
                    //label
                    mobjEditService.AddGeometry(oTextGeom, 32400);
                    return;
                }
                if (startdraw == 30)
                {
                    m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Double Click to complete!Right Click to start moving along section line.");
                    return;
                }
                if (startdraw == 38)
                {
                    m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Double Click to complete!Right Click to start moving follow cursor.");
                    return;
                }
                #endregion
                #region For Moving label along line plc
                if (startdraw == 39)
                {
                    m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Point to choose new position! Right Click to place in middle of section.");
                    
                    IGTTextPointGeometry temp = GTClassFactory.Create<IGTTextPointGeometry>();
                    IGTPoint tempp = GTClassFactory.Create<IGTPoint>();
                    tempp.X = e.WorldPoint.X;
                    tempp.Y = e.WorldPoint.Y;
                    tempp.Z = 0.0;
                    temp.Origin = tempp;
                    IGTPoint projectPoint = m_CustomForm.PointOnConduit(tempp.X, tempp.Y, true);

                    if (projectPoint.X == 0 && projectPoint.Y == 0)
                    {
                        //MessageBox.Show("Please, place Label on creating Conduit Feature!", "Duct Path Placement", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }
                    else
                    {
                        temp.Origin = projectPoint;
                        //IGTVector Orientation = GTClassFactory.Create<IGTVector>();
                        //temp.Orientation = Orientation.BuildVector( projectPoint,tempp);
                        int length = int.Parse(projectPoint.Z.ToString());
                        if (m_CustomForm.SectSlashes.Count > 0)
                        {
                            if (m_CustomForm.LastSection != 1)
                                if (length >= m_CustomForm.SectSlashes[m_CustomForm.SectSlashes.Count - 1].length)
                                {
                                   // MessageBox.Show("Not allowed to place Label for new section on existing one!", "Duct Path Placement", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                    return;
                                }
                            if (m_CustomForm.SectSlashes.Count > 1)
                            {
                                if (m_CustomForm.LastSection == 1)
                                {
                                    if (length <= m_CustomForm.SectSlashes[m_CustomForm.SectSlashes.Count - 1].length)
                                    {
                                     //   MessageBox.Show("Not allowed to place Label for new section on existing one!", "Duct Path Placement", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                        return;
                                    }
                                }
                                else
                                    if (length <= m_CustomForm.SectSlashes[m_CustomForm.SectSlashes.Count - 2].length)
                                    {
                                      //  MessageBox.Show("Not allowed to place Label for new section on existing one!", "Duct Path Placement", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                        return;
                                    }
                            }
                        }

                        temp.Rotation = m_CustomForm.TakeRotationOfSegmentPolyline(length);// OrientationForPointOnConduit(projectPoint.X, projectPoint.Y, length);
                        temp.Text = LabelAlongLine.Text;
                        temp.Alignment = 0;
                        if (mobjEditService.GeometryCount > 0)
                        {
                            if (mobjEditService.GetGeometry(mobjEditService.GeometryCount).Type == "TextPointGeometry")
                                mobjEditService.RemoveGeometry(mobjEditService.GeometryCount);
                        }
                        mobjEditService.AddGeometry(temp, 32400);
                       // oTextGeomLabel.Origin = temp.Origin;
                       // oTextGeomLabel.Text = temp.Text;
                       // oTextGeomLabel.Rotation = temp.Rotation;
                       // oTextGeomLabel.Alignment = 0;
                    }
                    return;
                }
                #endregion
                #region Rotate Label
                if (startdraw == 5)
                {

                    m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Point to confirm rotation! Right Click to skip rotation.");

                    IGTPoint Point1 = GTClassFactory.Create<IGTPoint>();
                    IGTPoint Point2 = GTClassFactory.Create<IGTPoint>();
                    IGTTextPointGeometry oTextGeom = GTClassFactory.Create<IGTTextPointGeometry>();
                    Point1.X = e.WorldPoint.X;
                    Point1.Y = e.WorldPoint.Y;
                    Point1.Z = 0.0;
                    Point2.X = mobjEditService.GetGeometry(mobjEditService.GeometryCount).FirstPoint.X;
                    Point2.Y = mobjEditService.GetGeometry(mobjEditService.GeometryCount).FirstPoint.Y;
                    Point2.Z = 0.0;
                    if (mobjEditService.GeometryCount > 0)
                        while (mobjEditService.GetGeometry(mobjEditService.GeometryCount).Type == "TextPointGeometry")
                            mobjEditService.RemoveGeometry(mobjEditService.GeometryCount);
                    oTextGeom.Origin = Point2;
                    oTextGeom.Text = m_CustomForm.SectLabel;
                    oTextGeom.Alignment = 0;
                    mobjEditService.AddGeometry(oTextGeom, 32400);
                    oTextGeom.Rotation = m_CustomForm.AngleBtwPoint(Point2.X, Point2.Y, Point1.X, Point1.Y);
                    mobjEditService.AddGeometry(oTextGeom, 32400);
                    oTextGeomLabel.Origin = Point2;
                    oTextGeomLabel.Text = m_CustomForm.SectLabel;
                    oTextGeomLabel.Rotation = oTextGeom.Rotation;

                }
                #endregion
                #region For Leader Line Section Label Placement
                if (startdraw == 35)
                {

                    m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Point to confirm point of turn! Double Click to complete drawing! Right Click to re-draw.");

                    IGTPolylineGeometry oLineGeom = GTClassFactory.Create<IGTPolylineGeometry>();
                    IGTPoint Point1 = GTClassFactory.Create<IGTPoint>();
                    Point1.X = e.WorldPoint.X;
                    Point1.Y = e.WorldPoint.Y;
                    Point1.Z = 0.0;

                    if (PointForLeaderLine == null)
                    {
                        PointForLeaderLine = GTClassFactory.Create<IGTPoint>();
                        PointForLeaderLine.X = LabelAlongLine.FirstPoint.X;
                        PointForLeaderLine.Y = LabelAlongLine.FirstPoint.Y;
                        PointForLeaderLine.Z = LabelAlongLine.FirstPoint.Z;
                    }

                    oLineGeom.Points.Add(PointForLeaderLine);
                    oLineGeom.Points.Add(Point1);
                    if (middledraw == 0)
                    {
                        if (mobjEditService.GeometryCount > 0)
                        {
                            if (mobjEditService.GetGeometry(mobjEditService.GeometryCount).Type == "PolylineGeometry")
                                mobjEditService.RemoveGeometry(mobjEditService.GeometryCount);
                        }
                    }

                    mobjEditService.AddGeometry(oLineGeom, 12501);

                    if (middledraw == 1) middledraw = 0;
                    return;
                }
                if (startdraw == 31)
                {
                    m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Double Click to complete! Right Click to delete leader line.");
                    return;
                }
                if (startdraw == 32)
                {
                    m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Double Click to complete! Right Click to place along line.");
                    return;
                }
                #endregion
                #region Status text commented
                //#region Slash section placing
                //if (startdraw == 2)
                //{
                //    m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "LEFT MOUSE CLICK TO PLACE TEMPORARY SLASH POINT ON MAP, DOUBLE MOUSE CLICK TO CONFIRM LOCATION OF SLASH POINT");
                //}
                //#endregion
                //#region Label section placing
                //if (startdraw == 3)
                //{
                //    m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "LEFT MOUSE CLICK TO PLACE TEMPORARY LABEL ON MAP, DOUBLE MOUSE CLICK TO CONFIRM LOCATION OF LABEL");
                //}
                //#endregion
                //#region Label section ROTATION
                //if (startdraw == 5)
                //{
                //    m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "LEFT MOUSE CLICK TO ROTATATE TEMPORARY LABEL ON MAP, DOUBLE MOUSE CLICK TO CONFIRM ROTATION OF LABEL");
                //}
                //#endregion
                //#region get SOURCE device
                //if (startdraw == 7)
                //{
                //    m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "SELECT SOURCE CIVIL FEATURE ON A MAP AND PRESS GET SELECTED BUTTON");
                //}
                //if (startdraw == 6)
                //{
                //    m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "PRESS 'Start Drawing' BUTTON FOR BEGINNING PLACEMENT FOR GRAPHIC LINE OF CONDUIT OR 'Get Selected' BUTTON TO RESELECT SOURCE DEVICE");
                //}
                //#endregion
            //    #region get TERM device
            //    if (startdraw == 8)
            //    {
            //        m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "PRESS 'Confirm' BUTTON FOR CONFIRMATION OF GRAPHIC OR 'Get Selected' BUTTON TO RESELECT TERMINATED DEVICE");
            //    }

            //    #endregion
            //    #region confirm GRAPHIC for conduit
            //    if (startdraw == 9)
            //    {
            //        m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage,"PRESS 'Confirm' BUTTON FOR CONFIRMATION OF ENTERED ATTRIBUTES FOR CONDUIT");
            //     }
            //     #endregion 
            //     #region plc only one section for conduit
            //     if (startdraw == 10)
            //     {
            //         m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "FILL ATTRIBUTES FOR SECTION, PRESS 'Confirm' FOR CONFIRMATION ENTERED VALUES");
            //     }
            //     #endregion 
            //     #region confirm ATTR for conduit
            //     if (startdraw == 11)
            //     {
            //         m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "PRESS 'Confirm' BUTTON FOR CONFIRMATION OF ENTERED ATTRIBUTES FOR CONDUIT");
            //     }
            //     #endregion 
            //     #region reconfirmation graphic for confuit
            //     if (startdraw == 12)
            //     {
            //         m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "PRESS 'Start Drawing' BUTTON TO REDRAW GRAPHIC LINE OF CONDUIT OR 'Get Selected' BUTTON TO RESELECT TERMINATED DEVICE");
            //     }
            //    #endregion
            //     #region reconfirmation TERM device for confuit
            //     if (startdraw == 13)
            //     {
            //         m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "SELECT TERMINATED CIVIL FEATURE ON A MAP AND PRESS GET SELECTED BUTTON");
            //     }
            //    #endregion
           
                // GTDuctPathPlc.startdraw = 10;
                // m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, 
                #endregion
 }
            catch (Exception ex)
            {
                
                MessageBox.Show(ex.Message, "Duct Path Placement", MessageBoxButtons.OK, MessageBoxIcon.Error);
                ExitCmd();
            }
        }
        #endregion

        #region Double Click
        void m_oIGTCustomCommandHelper_DblClick(object sender, GTMouseEventArgs e)
        {
            try
            {
                #region LEFT CLICK
                if (e.Button != 2)
                {
                    #region For Conduit Line plc
                    if ((startdraw == 1 || startdraw == 101) && StartDrawPoint != null)
                    {
                        IGTPolylineGeometry oLineGeom = GTClassFactory.Create<IGTPolylineGeometry>();
                        oLineGeom.Points.Add(StartDrawPoint);

                        if (startdraw == 1)
                        {
                            IGTPoint Point1 = GTClassFactory.Create<IGTPoint>();
                            Point1.X = e.WorldPoint.X;
                            Point1.Y = e.WorldPoint.Y;
                            Point1.Z = 0.0;
                            oLineGeom.Points.Add(Point1);
                        }

                        oLineGeom.Points.Add(EndDrawPoint);
                        if (middledraw == 0)
                        {
                            if (mobjEditService.GeometryCount > 0)
                                mobjEditService.RemoveGeometry(mobjEditService.GeometryCount);
                        }
                        mobjEditService.AddGeometry(oLineGeom, styleid);
                        StartDrawPoint.X = e.WorldPoint.X;
                        StartDrawPoint.Y = e.WorldPoint.Y;
                        StartDrawPoint.Z = 0.0;
                        if (middledraw == 1) middledraw = 0;
                        startdraw = 0;
                        //  m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "PRESS 'Start Drawing' BUTTON TO REDRAW GRAPHIC LINE OF CONDUIT OR 'Get Selected' BUTTON TO RESELECT TERMINATED DEVICE");
                        m_CustomForm.LocateFeature(2, m_gtapp.ActiveMapWindow);
                        m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, ""); m_CustomForm.Show();
                        m_CustomForm.DrawBtnIsEnable = true;
                    }
                    #endregion

                    #region first sttep of confirmation Leader Line Section Label Placement
                    if (startdraw == 35)
                    {
                        IGTPolylineGeometry oLineGeom = GTClassFactory.Create<IGTPolylineGeometry>();
                        IGTPoint Point1 = GTClassFactory.Create<IGTPoint>();
                        Point1.X = e.WorldPoint.X;
                        Point1.Y = e.WorldPoint.Y;
                        Point1.Z = 0.0;

                        if (PointForLeaderLine == null)
                        {
                            PointForLeaderLine = GTClassFactory.Create<IGTPoint>();
                            PointForLeaderLine.X = LabelAlongLine.FirstPoint.X;
                            PointForLeaderLine.Y = LabelAlongLine.FirstPoint.Y;
                            PointForLeaderLine.Z = LabelAlongLine.FirstPoint.Z;
                        }

                        oLineGeom.Points.Add(PointForLeaderLine);
                        oLineGeom.Points.Add(Point1);
                        if (middledraw == 0)
                        {
                            if (mobjEditService.GeometryCount > 0)
                            {
                                if (mobjEditService.GetGeometry(mobjEditService.GeometryCount).Type == "PolylineGeometry")
                                    mobjEditService.RemoveGeometry(mobjEditService.GeometryCount);
                            }
                        }

                        mobjEditService.AddGeometry(oLineGeom, 12501);

                        if (middledraw == 1) middledraw = 0;
                        startdraw = 31;
                        return;
                    }
                    #endregion

                    #region For Section Slash plc

                    if (startdraw == 20)
                    {

                        //IGTOrientedPointGeometry temp = GTClassFactory.Create<IGTOrientedPointGeometry>();
                        //IGTPoint tempp = GTClassFactory.Create<IGTPoint>();
                        //tempp.X = e.WorldPoint.X;// mobjEditService.GetGeometry(mobjEditService.GeometryCount).FirstPoint.X;
                        //tempp.Y = e.WorldPoint.Y; //mobjEditService.GetGeometry(mobjEditService.GeometryCount).FirstPoint.Y;
                        //tempp.Z = 0.0;
                        //temp.Origin = tempp;

                        //IGTPoint projectPoint = m_CustomForm.PointOnConduit(tempp.X, tempp.Y, true);
                        //int length = int.Parse(projectPoint.Z.ToString());
                        //temp.Orientation = m_CustomForm.OrientationForPointOnConduit(projectPoint.X, projectPoint.Y, length); 
                        //mobjEditService.RemoveGeometry(mobjEditService.GeometryCount);
                        //            mobjEditService.AddGeometry(temp, 2220006);
                        //            m_CustomForm.AddSectSlash(projectPoint.X, projectPoint.Y, length, temp.Orientation);
                        //            m_gtapp.SelectedObjects.Clear();
                        //            startdraw = 0;
                        //            m_CustomForm.ConfSelBtnIsEnable = true;
                        //            m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "");m_CustomForm.Show();

                        IGTOrientedPointGeometry temp = GTClassFactory.Create<IGTOrientedPointGeometry>();
                        IGTPoint tempp = GTClassFactory.Create<IGTPoint>();
                        tempp.X = e.WorldPoint.X;
                        tempp.Y = e.WorldPoint.Y;
                        tempp.Z = 0.0;
                        temp.Origin = tempp;
                        IGTPoint projectPoint = m_CustomForm.PointOnConduit(tempp.X, tempp.Y, true);

                        if (projectPoint.X == 0 && projectPoint.Y == 0)
                        {
                            MessageBox.Show("Please, place Slash Point on creating Conduit Feature!", "Duct Path Placement", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            return;
                        }
                        else
                        {
                            temp.Origin = projectPoint;
                            //IGTVector Orientation = GTClassFactory.Create<IGTVector>();
                            //temp.Orientation = Orientation.BuildVector( projectPoint,tempp);
                            int length = int.Parse(projectPoint.Z.ToString());
                            if (m_CustomForm.SectSlashes.Count > 0)
                            {
                                if (length <= m_CustomForm.SectSlashes[m_CustomForm.SectSlashes.Count - 1].length)
                                {
                                    MessageBox.Show("Not allowed place Slash for new section on existing one!", "Duct Path Placement", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                    return;
                                }
                            }

                            temp.Orientation = m_CustomForm.OrientationForPointOnConduit(projectPoint.X, projectPoint.Y, length);

                            if (mobjEditService.GeometryCount > 0)
                            {
                                if (mobjEditService.GetGeometry(mobjEditService.GeometryCount).Type == "OrientedPointGeometry")
                                    mobjEditService.RemoveGeometry(mobjEditService.GeometryCount);
                            }
                            mobjEditService.AddGeometry(temp, 2220006);
                            m_CustomForm.AddSectSlash(projectPoint.X, projectPoint.Y, length, temp.Orientation);
                            m_gtapp.SelectedObjects.Clear();
                            startdraw = 0;
                            m_CustomForm.ConfSelBtnIsEnable = true;
                            m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, ""); m_CustomForm.Show();
                            return;
                        }


                    }
                    #endregion

                    #region For Section Label Placement
                    if (startdraw == 30 || startdraw == 32 || startdraw == 38)
                    {
                        // IGTGeometry geom = GTClassFactory.Create<IGTGeometry>();
                        if (mobjEditService.GeometryCount > 0)
                        {
                            mobjEditService.RemoveGeometry(mobjEditService.GeometryCount);
                        }
                        mobjEditService.AddGeometry(oTextGeomLabel, 32500);
                        m_CustomForm.SectionLabelAdd(oTextGeomLabel.Origin.X, oTextGeomLabel.Origin.Y, oTextGeomLabel.Rotation, null);
                        oTextGeomLabel = null;
                        m_gtapp.SelectedObjects.Clear();

                        if (m_CustomForm.LastSection == 0)
                        {
                            m_gtapp.SelectedObjects.Clear();
                            startdraw = 2;
                        }
                        else
                        {
                            startdraw = 0;
                            m_gtapp.SelectedObjects.Clear();
                            m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, ""); m_CustomForm.Show();
                            if (m_CustomForm.LastSection == 2)
                            {
                                m_CustomForm.ConfSelBtnIsEnable = true;
                                m_CustomForm.LastSection = 1;
                            }
                        }
                        return;

                    }
                   
                    //label with leader line
                    if (startdraw == 31)
                    {

                        IGTPolylineGeometry LeaderLine = GTClassFactory.Create<IGTPolylineGeometry>();
                        IGTGeometry geom = GTClassFactory.Create<IGTGeometry>();
                        if (mobjEditService.GetGeometry(mobjEditService.GeometryCount).Type != "TextPointGeometry")
                        {
                            geom = mobjEditService.GetGeometry(mobjEditService.GeometryCount);
                            LeaderLine.Points.Add(geom.FirstPoint);
                            for (int i = mobjEditService.GeometryCount; i >= 1; i--)
                            {
                                geom = mobjEditService.GetGeometry(i);
                                if (geom.Type != "TextPointGeometry")
                                {
                                    LeaderLine.Points.Add(geom.FirstPoint);
                                    mobjEditService.RemoveGeometry(i);
                                }
                                else break;
                            }
                        }

                        if (mobjEditService.GeometryCount > 0)
                        {
                            mobjEditService.RemoveGeometry(mobjEditService.GeometryCount);
                        }

                        mobjEditService.AddGeometry(LeaderLine, 13002);
                        mobjEditService.AddGeometry(oTextGeomLabel, 32500);

                        m_CustomForm.SectionLabelAdd(oTextGeomLabel.Origin.X, oTextGeomLabel.Origin.Y, oTextGeomLabel.Rotation, LeaderLine);
                        oTextGeomLabel = null;
                        LeaderLine = null;
                        PointForLeaderLine = null;
                        m_gtapp.SelectedObjects.Clear();

                        if (m_CustomForm.LastSection == 0)
                        {
                            m_gtapp.SelectedObjects.Clear();
                            startdraw = 2;
                        }
                        else
                        {
                            startdraw = 0;
                            m_gtapp.SelectedObjects.Clear();
                            m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, ""); m_CustomForm.Show();
                            if (m_CustomForm.LastSection == 2)
                            {
                                m_CustomForm.ConfSelBtnIsEnable = true;
                                m_CustomForm.LastSection = 1;
                            }
                        }
                        return;
                    }
                    #endregion

                }
                #endregion
            }
            catch (Exception ex)
            {
                
                MessageBox.Show(ex.Message, "Duct Path Placement", MessageBoxButtons.OK, MessageBoxIcon.Error);
                ExitCmd();
            }

        }

        
        #endregion

        #region MouseUp
        void m_oIGTCustomCommandHelper_MouseUp(object sender, GTMouseEventArgs e)
        {
            try
            {
                IGTPoint WorldPoint = e.WorldPoint;

                #region LEFT CLICK
                if (e.Button != 2)
                {


                    #region For Conduit Line Placement
                    if (startdraw == 1 && StartDrawPoint != null)
                    {

                        //   m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "LEFT MOUSE CLICK TO CONFIRM POINT ON MAP, DOUBLE MOUSE CLICK TO FINISHED DRAWING");
                        IGTPoint Point1 = GTClassFactory.Create<IGTPoint>();
                        IGTPolylineGeometry oLineGeom = GTClassFactory.Create<IGTPolylineGeometry>();
                        Point1.X = e.WorldPoint.X;
                        Point1.Y = e.WorldPoint.Y;
                        Point1.Z = 0.0;
                        oLineGeom.Points.Add(StartDrawPoint);
                        oLineGeom.Points.Add(Point1);
                        if (middledraw == 0)
                        {
                            if (mobjEditService.GeometryCount > 0)
                                mobjEditService.RemoveGeometry(mobjEditService.GeometryCount);// .RemoveAllGeometries();
                        }

                        mobjEditService.AddGeometry(oLineGeom, styleid);

                        StartDrawPoint.X = e.WorldPoint.X;
                        StartDrawPoint.Y = e.WorldPoint.Y;
                        StartDrawPoint.Z = 0.0;
                        middledraw = 1;
                        //  m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "LEFT MOUSE CLICK TO CONFIRM POINT ON MAP, DOUBLE MOUSE CLICK TO FINISHED DRAWING");

                    }
                    #endregion

                    #region For Leader Line Section Label Placement
                    if (startdraw == 35)
                    {
                        if (PointForLeaderLine == null)
                        {
                            PointForLeaderLine = GTClassFactory.Create<IGTPoint>();
                        }
                        PointForLeaderLine.X = e.WorldPoint.X;
                        PointForLeaderLine.Y = e.WorldPoint.Y;
                        PointForLeaderLine.Z = 0.0;
                        middledraw = 1;
                    }
                    #endregion

                    #region For Section Label Placement

                    //fix position after rotation
                    if (startdraw == 5)
                    {
                        IGTPoint Point1 = GTClassFactory.Create<IGTPoint>();
                        IGTPoint Point2 = GTClassFactory.Create<IGTPoint>();
                        IGTTextPointGeometry oTextGeom = GTClassFactory.Create<IGTTextPointGeometry>();
                        Point1.X = e.WorldPoint.X;
                        Point1.Y = e.WorldPoint.Y;
                        Point1.Z = 0.0;
                        Point2.X = mobjEditService.GetGeometry(mobjEditService.GeometryCount).FirstPoint.X;
                        Point2.Y = mobjEditService.GetGeometry(mobjEditService.GeometryCount).FirstPoint.Y;
                        Point2.Z = 0.0;
                        oTextGeom.Origin = Point2;
                        oTextGeom.Rotation = m_CustomForm.AngleBtwPoint(Point2.X, Point2.Y, Point1.X, Point1.Y);
                        oTextGeom.Text = m_CustomForm.SectLabel;
                        oTextGeom.Alignment = 0;
                        mobjEditService.RemoveGeometry(mobjEditService.GeometryCount);
                        mobjEditService.RemoveGeometry(mobjEditService.GeometryCount);
                        mobjEditService.RemoveGeometry(mobjEditService.GeometryCount);
                        mobjEditService.AddGeometry(oTextGeom, 32400);
                        oTextGeomLabel.Origin = Point2;
                        startdraw = 35;
                        return;
                    }

                    if (startdraw == 3)
                    {
                        startdraw = 5;
                        return;
                    }
                    #endregion

                    #region For Section Slash plc
                    if (startdraw == 2 || startdraw == 20)
                    {
                        IGTOrientedPointGeometry temp = GTClassFactory.Create<IGTOrientedPointGeometry>();
                        IGTPoint tempp = GTClassFactory.Create<IGTPoint>();
                        tempp.X = e.WorldPoint.X;
                        tempp.Y = e.WorldPoint.Y;
                        tempp.Z = 0.0;
                        temp.Origin = tempp;
                        IGTPoint projectPoint = m_CustomForm.PointOnConduit(tempp.X, tempp.Y, true);

                        if (projectPoint.X == 0 && projectPoint.Y == 0)
                        {
                            MessageBox.Show("Please, place Slash Point on creating Conduit Feature!", "Duct Path Placement", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            return;
                        }
                        else
                        {
                            temp.Origin = projectPoint;
                            //IGTVector Orientation = GTClassFactory.Create<IGTVector>();
                            //temp.Orientation = Orientation.BuildVector( projectPoint,tempp);
                            int length = int.Parse(projectPoint.Z.ToString());
                            if (m_CustomForm.SectSlashes.Count > 0)
                            {
                                if (length <= m_CustomForm.SectSlashes[m_CustomForm.SectSlashes.Count - 1].length)
                                {
                                    MessageBox.Show("Not allowed to place Slash for new section on existing one!", "Duct Path Placement", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                    return;
                                }
                            }

                            temp.Orientation = m_CustomForm.OrientationForPointOnConduit(projectPoint.X, projectPoint.Y, length);

                            if (mobjEditService.GeometryCount > 0)
                            {
                                if (mobjEditService.GetGeometry(mobjEditService.GeometryCount).Type == "OrientedPointGeometry")
                                    mobjEditService.RemoveGeometry(mobjEditService.GeometryCount);
                            }
                            mobjEditService.AddGeometry(temp, 2220004);
                        }
                        startdraw = 20;
                        return;
                    }
                    #endregion
                   
                    #region For Moving label along line plc
                    

                    if (startdraw == 39)
                    {
                        IGTTextPointGeometry temp = GTClassFactory.Create<IGTTextPointGeometry>();
                        IGTPoint tempp = GTClassFactory.Create<IGTPoint>();
                        tempp.X = e.WorldPoint.X;
                        tempp.Y = e.WorldPoint.Y;
                        tempp.Z = 0.0;
                        temp.Origin = tempp;
                        IGTPoint projectPoint = m_CustomForm.PointOnConduit(tempp.X, tempp.Y, true);

                        if (projectPoint.X == 0 && projectPoint.Y == 0)
                        {
                            MessageBox.Show("Please, place Label on creating Conduit Feature!", "Duct Path Placement", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            return;
                        }
                        else
                        {
                            temp.Origin = projectPoint;
                            //IGTVector Orientation = GTClassFactory.Create<IGTVector>();
                            //temp.Orientation = Orientation.BuildVector( projectPoint,tempp);
                            int length = int.Parse(projectPoint.Z.ToString());
                            if (m_CustomForm.SectSlashes.Count > 0)
                            {
                                if (m_CustomForm.LastSection != 1)
                                    if (length >= m_CustomForm.SectSlashes[m_CustomForm.SectSlashes.Count - 1].length)
                                    {
                                        MessageBox.Show("Not allowed to place Label for new section on existing one!", "Duct Path Placement", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                        return;
                                    }
                                if (m_CustomForm.SectSlashes.Count > 1)
                                {
                                    if (m_CustomForm.LastSection == 1)
                                    {
                                        if (length <= m_CustomForm.SectSlashes[m_CustomForm.SectSlashes.Count - 1].length)
                                        {
                                            MessageBox.Show("Not allowed to place Label for new section on existing one!", "Duct Path Placement", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                            return;
                                        }
                                    }
                                    else
                                        if (length <= m_CustomForm.SectSlashes[m_CustomForm.SectSlashes.Count - 2].length)
                                        {
                                            MessageBox.Show("Not allowed to place Label for new section on existing one!", "Duct Path Placement", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                            return;
                                        }
                                }
                            }

                            temp.Rotation = m_CustomForm.TakeRotationOfSegmentPolyline(length);// OrientationForPointOnConduit(projectPoint.X, projectPoint.Y, length);
                            temp.Text = LabelAlongLine.Text;
                            temp.Alignment = 0;
                            if (mobjEditService.GeometryCount > 0)
                            {
                                if (mobjEditService.GetGeometry(mobjEditService.GeometryCount).Type == "TextPointGeometry")
                                    mobjEditService.RemoveGeometry(mobjEditService.GeometryCount);
                            }
                            mobjEditService.AddGeometry(temp, 32400);
                            oTextGeomLabel.Origin = temp.Origin;
                            oTextGeomLabel.Text = temp.Text;
                            oTextGeomLabel.Rotation = temp.Rotation;
                            oTextGeomLabel.Alignment = 0;
                        }
                        startdraw = 38;
                        return;
                    }
                    #endregion

                    #region Get Selected Source Wall OR Term Wall of Device
                    if (startdraw == 300 || startdraw == 400)
                    {
                        IGTDDCKeyObjects feat = mobjLocateService.Locate(WorldPoint, 20, 1, GTSelectionTypeConstants.gtmwstSelectSingle);
                        for (int K = 0; K < feat.Count; K++)
                            m_gtapp.SelectedObjects.Add(GTSelectModeConstants.gtsosmAllDisplayedComponentsInActiveLegend, feat[K]);

                        if (m_gtapp.SelectedObjects.FeatureCount == 1)
                        {
                            if ((startdraw == 300 && m_CustomForm.GetDeviceWithWall(true))//source
                                || (startdraw == 400 && m_CustomForm.GetDeviceWithWall(false)))//term
                            {
                                startdraw = 0;
                                m_CustomForm.LocateFeature(2, m_gtapp.ActiveMapWindow);
                                m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "");m_CustomForm.Show();
                                return;
                            }
                            else return;
                        }

                    }
                    #endregion

                    #region Get Selected Source and Termineted Devices
                    if (startdraw == 100)
                    {

                        IGTDDCKeyObjects feat = mobjLocateService.Locate(WorldPoint, 20, 1, GTSelectionTypeConstants.gtmwstSelectMultiple);
                        for (int K = 0; K < feat.Count; K++)
                            m_gtapp.SelectedObjects.Add(GTSelectModeConstants.gtsosmAllDisplayedComponentsInActiveLegend, feat[K]);

                        if (m_gtapp.SelectedObjects.FeatureCount == 2)
                        {
                            IGTGeometry geom = null;
                            short iFNO = 0;
                            int iFID = 0;

                            foreach (IGTDDCKeyObject oDDCKeyObject in m_gtapp.SelectedObjects.GetObjects())
                            {
                                if (iFID != oDDCKeyObject.FID)
                                {
                                    if (iFID == 0)
                                    {
                                        if (!m_CustomForm.GetSourceDevice(oDDCKeyObject.FNO, oDDCKeyObject.FID, oDDCKeyObject.Geometry))
                                        {
                                            // m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Select Source and Terminated Devices Again!");
                                            m_gtapp.SelectedObjects.Clear();
                                            return;
                                        }
                                    }
                                    else
                                    {
                                        if (m_CustomForm.GetTermDevice(oDDCKeyObject.FNO, oDDCKeyObject.FID, oDDCKeyObject.Geometry))
                                        {
                                            startdraw = 0;
                                            m_CustomForm.LocateFeature(2, m_gtapp.ActiveMapWindow);
                                            m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "");m_CustomForm.Show();
                                            return;
                                        }
                                        else
                                        {
                                            //  m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Select Source and Terminated Devices Again!");
                                            m_gtapp.SelectedObjects.Clear();
                                            return;
                                        }

                                    }
                                }
                                geom = oDDCKeyObject.Geometry;
                                iFNO = oDDCKeyObject.FNO;
                                iFID = oDDCKeyObject.FID;
                            }
                        }
                        else if (m_gtapp.SelectedObjects.FeatureCount > 2)
                        {
                            MessageBox.Show("Selected more than two features!\nSelect Source and Terminated Devices Again!", "Duct Path Placement", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            // m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Selected more than two features! Select Source and Terminated Devices Again!");
                            m_gtapp.SelectedObjects.Clear();
                            return;
                        }
                        return;
                    }
                    #endregion

                    #region Re Select Source OR Termineted Devices
                    if (startdraw == 500 || startdraw == 600)
                    {
                        IGTDDCKeyObjects feat = mobjLocateService.Locate(WorldPoint, 20, 1, GTSelectionTypeConstants.gtmwstSelectSingle);
                        for (int K = 0; K < feat.Count; K++)
                            m_gtapp.SelectedObjects.Add(GTSelectModeConstants.gtsosmAllDisplayedComponentsInActiveLegend, feat[K]);

                        if (m_gtapp.SelectedObjects.FeatureCount == 1)
                        {

                            IGTGeometry geom = null;
                            short iFNO = 0;
                            int iFID = 0;
                            foreach (IGTDDCKeyObject oDDCKeyObject in m_gtapp.SelectedObjects.GetObjects())
                            {
                                geom = oDDCKeyObject.Geometry;
                                iFNO = oDDCKeyObject.FNO;
                                iFID = oDDCKeyObject.FID;
                                break;
                            }
                            //check if selected allowed feature and if succes show detail

                            if ((startdraw == 500 && m_CustomForm.GetSourceDevice(iFNO, iFID, geom))//source
                                || (startdraw == 600 && m_CustomForm.GetTermDevice(iFNO, iFID, geom)))//term
                            {
                                mobjEditService.RemoveAllGeometries();
                                startdraw = 0;
                                m_CustomForm.LocateFeature(2, m_gtapp.ActiveMapWindow);
                                m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "");m_CustomForm.Show();
                            }

                        }
                        return;
                    }
                    #endregion 
                }
                #endregion

                #region RIGHT CLICK

                if (e.Button == 2)
                {
                    #region start drawing with mouse moving Conduit Line Placement
                    if (startdraw == 101)
                    {
                        if (mobjEditService.GeometryCount > 0)
                            mobjEditService.RemoveAllGeometries();
                        m_CustomForm.LocateFeature(2, m_gtapp.ActiveMapWindow);
                        startdraw = 1;
                        //  m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "LEFT MOUSE CLICK TO CONFIRM POINT ON MAP, DOUBLE MOUSE CLICK TO FINISHED DRAWING");

                    }
                    #endregion

                    #region Cancelation for Wall or Device Selection
                    if (startdraw == 300 || startdraw == 400 || startdraw == 500 || startdraw == 600)
                    {
                        m_CustomForm.LocateFeature(2, m_gtapp.ActiveMapWindow);
                        m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "");m_CustomForm.Show();
                        startdraw = 0;
                        // m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "LEFT MOUSE CLICK TO CONFIRM POINT ON MAP, DOUBLE MOUSE CLICK TO FINISHED DRAWING");

                    }
                    #endregion

                    #region Cancellation of placement for section's slash
                    if (startdraw == 2)
                    {
                        if (m_CustomForm.LastSection == 0)
                        {
                            m_gtapp.SelectedObjects.Clear();
                            if (mobjEditService.GeometryCount > 0)
                                if (mobjEditService.GetGeometry(mobjEditService.GeometryCount).Type == "OrientedPointGeometry")
                                    mobjEditService.RemoveGeometry(mobjEditService.GeometryCount);
                            startdraw = 0;
                            m_CustomForm.LastSection = 1;
                            // m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Cancellation of placement for section's slash! Next section will be the last");
                            m_CustomForm.LocateFeature(2, m_gtapp.ActiveMapWindow);
                            m_CustomForm.ConfSelBtnIsEnable = true;
                            m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "");m_CustomForm.Show();
                            return;
                        }

                    }

                    if (startdraw == 20)
                    {
                        startdraw = 2;
                        m_CustomForm.LocateFeature(2, m_gtapp.ActiveMapWindow);
                        return;
                    }
                    #endregion

                    #region Placement for section's label along the line
                    if (startdraw == 3 || startdraw == 32 || startdraw == 39 || (startdraw == 30 && oTextGeomLabel.Rotation != LabelAlongLine.Rotation))
                    {
                        if (mobjEditService.GeometryCount > 0)
                        {
                            while (mobjEditService.GetGeometry(mobjEditService.GeometryCount).Type == "TextPointGeometry")
                                mobjEditService.RemoveGeometry(mobjEditService.GeometryCount);
                            if (mobjEditService.GetGeometry(mobjEditService.GeometryCount).Type == "PolylineGeometry" &&
                                mobjEditService.GeometryCount != 1)
                                mobjEditService.RemoveGeometry(mobjEditService.GeometryCount);
                        }
                        LabelAlongLine.Alignment = 0;
                        mobjEditService.AddGeometry(LabelAlongLine, 32400);

                        oTextGeomLabel.Origin = LabelAlongLine.Origin;
                        oTextGeomLabel.Text = m_CustomForm.SectLabel;
                        oTextGeomLabel.Rotation = LabelAlongLine.Rotation;
                        oTextGeomLabel.Alignment = 0;
                        startdraw = 30;
                        return;
                    }
                    //start moving along line
                    if (startdraw == 30 && oTextGeomLabel.Rotation== LabelAlongLine.Rotation)
                    {
                        startdraw = 39;
                        m_CustomForm.LocateFeature(2, m_gtapp.ActiveMapWindow);
                        return;
                    }
                    //start moving again
                    if (startdraw == 38)
                    {
                        startdraw = 3;
                        m_CustomForm.LocateFeature(2, m_gtapp.ActiveMapWindow);
                        return;
                    }
                    #endregion
                    #region Skip rotation
                    if (startdraw == 5)
                    {
                        IGTPoint Point1 = GTClassFactory.Create<IGTPoint>();
                        IGTPoint Point2 = GTClassFactory.Create<IGTPoint>();
                        IGTTextPointGeometry oTextGeom = GTClassFactory.Create<IGTTextPointGeometry>();
                        Point1.X = e.WorldPoint.X;
                        Point1.Y = e.WorldPoint.Y;
                        Point1.Z = 0.0;
                        Point2.X = mobjEditService.GetGeometry(mobjEditService.GeometryCount).FirstPoint.X;
                        Point2.Y = mobjEditService.GetGeometry(mobjEditService.GeometryCount).FirstPoint.Y;
                        Point2.Z = 0.0;
                        oTextGeom.Origin = Point2;
                        oTextGeom.Rotation = LabelAlongLine.Rotation;// m_CustomForm.AngleBtwPoint(Point2.X, Point2.Y, Point1.X, Point1.Y);
                        oTextGeom.Text = m_CustomForm.SectLabel;
                        oTextGeom.Alignment = 0;
                        mobjEditService.RemoveGeometry(mobjEditService.GeometryCount);
                        mobjEditService.RemoveGeometry(mobjEditService.GeometryCount);
                        mobjEditService.RemoveGeometry(mobjEditService.GeometryCount);
                        mobjEditService.AddGeometry(oTextGeom, 32400);
                        oTextGeomLabel.Origin = Point2;
                        oTextGeomLabel.Text = m_CustomForm.SectLabel;
                        oTextGeomLabel.Rotation = LabelAlongLine.Rotation;
                        oTextGeomLabel.Alignment = 0;
                        startdraw = 35;
                        return;
                    }
                    #endregion
                    #region Start drawing leader line from beginning
                    if (startdraw == 35)
                    {
                        //  IGTGeometry geom = GTClassFactory.Create<IGTGeometry>();
                        if (mobjEditService.GeometryCount > 0)
                        {
                            while (mobjEditService.GetGeometry(mobjEditService.GeometryCount).Type != "TextPointGeometry")
                                mobjEditService.RemoveGeometry(mobjEditService.GeometryCount);
                        }
                        PointForLeaderLine = null;
                        middledraw = 0;
                        return;
                    }
                    //after stopping drawing before final acception
                    if (startdraw == 31)
                    {
                        //  IGTGeometry geom = GTClassFactory.Create<IGTGeometry>();
                        if (mobjEditService.GeometryCount > 0)
                        {
                            while (mobjEditService.GetGeometry(mobjEditService.GeometryCount).Type != "TextPointGeometry")
                                mobjEditService.RemoveGeometry(mobjEditService.GeometryCount);
                        }
                        PointForLeaderLine = null;
                        middledraw = 0;
                        startdraw = 32;
                        return;
                    }
                    //start  drawing again
                    //if (startdraw == 32)
                    //{
                    //    startdraw = 35;
                    //    return;
                    //}
                    #endregion

                    #region Exit application while choosing for 2 start features
                    if (startdraw == 100)
                    {
                        DialogResult retVal = MessageBox.Show("Are you sure that you want to exit?", "Duct Path Placement", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (retVal == DialogResult.Yes)
                        {
                            m_CustomForm.Close();
                            ExitCmd();
                        }
                    }
                    #endregion
                }
                #endregion

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Duct Path Placement", MessageBoxButtons.OK, MessageBoxIcon.Error);
                ExitCmd();
            }

        }
        #endregion

        #region press button (ESC)
        void m_oIGTCustomCommandHelper_KeyUp(object sender, GTKeyEventArgs e)
        {
            try{
            if (e.KeyCode == 27)
            {
                #region start drawing with mouse moving Conduit Line Placement
                if (startdraw == 101)
                {
                    if (mobjEditService.GeometryCount > 0)
                        mobjEditService.RemoveAllGeometries();
                    m_CustomForm.LocateFeature(2, m_gtapp.ActiveMapWindow);
                    startdraw = 1;
                  //  m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "LEFT MOUSE CLICK TO CONFIRM POINT ON MAP, DOUBLE MOUSE CLICK TO FINISHED DRAWING");

                }
                #endregion
               
                #region Cancelation for Wall or Device Selection
                if (startdraw == 300 || startdraw == 400 || startdraw == 500 || startdraw == 600)
                {
                    m_CustomForm.LocateFeature(2, m_gtapp.ActiveMapWindow);
                    m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "");m_CustomForm.Show();
                    startdraw = 0;
                   // m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "LEFT MOUSE CLICK TO CONFIRM POINT ON MAP, DOUBLE MOUSE CLICK TO FINISHED DRAWING");

                }
                #endregion

                #region Cancellation of placement for section's slash
                if (startdraw == 2)
                {
                    if (m_CustomForm.LastSection == 0)
                    {
                        m_gtapp.SelectedObjects.Clear();
                        if (mobjEditService.GeometryCount > 0)
                            if (mobjEditService.GetGeometry(mobjEditService.GeometryCount).Type == "OrientedPointGeometry")
                                mobjEditService.RemoveGeometry(mobjEditService.GeometryCount);
                        startdraw = 0;
                        m_CustomForm.LastSection = 1;
                       // m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Cancellation of placement for section's slash! Next section will be the last");
                        m_CustomForm.LocateFeature(2, m_gtapp.ActiveMapWindow);
                        m_CustomForm.ConfSelBtnIsEnable = true;
                        m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "");m_CustomForm.Show();
                        return;
                    }

                }

                if (startdraw == 20)
                {
                    startdraw = 2;
                    m_CustomForm.LocateFeature(2, m_gtapp.ActiveMapWindow);
                    return;
                }
                #endregion

                #region Placement for section's label along the line
                if (startdraw == 3 || startdraw == 5 || startdraw == 32 || (startdraw == 30 && oTextGeomLabel.Rotation != LabelAlongLine.Rotation))
                {
                    if (mobjEditService.GeometryCount > 0)
                    {
                        while (mobjEditService.GetGeometry(mobjEditService.GeometryCount).Type == "TextPointGeometry")
                            mobjEditService.RemoveGeometry(mobjEditService.GeometryCount);
                        if (mobjEditService.GetGeometry(mobjEditService.GeometryCount).Type == "PolylineGeometry" &&
                            mobjEditService.GeometryCount != 1)
                            mobjEditService.RemoveGeometry(mobjEditService.GeometryCount);
                    }
                    mobjEditService.AddGeometry(LabelAlongLine, 32400);

                    oTextGeomLabel.Origin = LabelAlongLine.Origin;
                    oTextGeomLabel.Text = m_CustomForm.SectLabel;
                    oTextGeomLabel.Rotation = LabelAlongLine.Rotation;
                    startdraw = 30;
                    return;
                }
                //start moving again
                if (startdraw == 30 && oTextGeomLabel.Rotation == LabelAlongLine.Rotation)
                {
                    startdraw = 3;
                    m_CustomForm.LocateFeature(2, m_gtapp.ActiveMapWindow);
                    return;
                }
                #endregion

                #region Start drawing leader line from beginning
                if (startdraw == 35)
                {
                    //  IGTGeometry geom = GTClassFactory.Create<IGTGeometry>();
                    if (mobjEditService.GeometryCount > 0)
                    {
                        while (mobjEditService.GetGeometry(mobjEditService.GeometryCount).Type != "TextPointGeometry")
                            mobjEditService.RemoveGeometry(mobjEditService.GeometryCount);
                    }
                    PointForLeaderLine = null;
                    middledraw = 0;
                    return;
                }
                //after stopping drawing before final acception
                if (startdraw == 31)
                {
                    //  IGTGeometry geom = GTClassFactory.Create<IGTGeometry>();
                    if (mobjEditService.GeometryCount > 0)
                    {
                        while (mobjEditService.GetGeometry(mobjEditService.GeometryCount).Type != "TextPointGeometry")
                            mobjEditService.RemoveGeometry(mobjEditService.GeometryCount);
                    }
                    PointForLeaderLine = null;
                    middledraw = 0;
                    startdraw = 32;
                    return;
                }
                //start  drawing again
                //if (startdraw == 32)
                //{
                //    startdraw = 35;
                //    return;
                //}
                #endregion

                #region Exit application while choosing for 2 start features
                if (startdraw == 100 )
                {
                    DialogResult retVal = MessageBox.Show("Are you sure that you want to exit?", "Duct Path Placement", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (retVal == DialogResult.Yes)
                    {
                        m_CustomForm.Close();
                        ExitCmd();
                    }
                }
                #endregion
            }
        }
        catch (Exception ex)
        {
           
            MessageBox.Show(ex.Message, "Duct Path Placement", MessageBoxButtons.OK, MessageBoxIcon.Error);
            ExitCmd();
        }
        }
        #endregion

        #region events only with status bar
        #region Mouse click
        void m_oIGTCustomCommandHelper_Click(object sender, GTMouseEventArgs e)
        {
           
        }
          #endregion

        
        void m_oIGTCustomCommandHelper_WheelRotate(object sender, GTWheelRotateEventArgs e)
        {
           // m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "WheelRotate.");
        }  

        void m_oIGTCustomCommandHelper_MouseDown(object sender, GTMouseEventArgs e)
        {
          //  m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "MouseDown.");
        }

        void m_oIGTCustomCommandHelper_LostFocus(object sender, GTLostFocusEventArgs e)
        {
            
          //  m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "LostFocus.");
        }
       

        void m_oIGTCustomCommandHelper_GainedFocus(object sender, GTGainedFocusEventArgs e)
        {
         //   m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "GainedFocus.");

        }

        void m_oIGTCustomCommandHelper_Deactivate(object sender, GTDeactivateEventArgs e)
        {
           // m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Deactivate.");
        }

        void m_oIGTCustomCommandHelper_Activate(object sender, GTActivateEventArgs e)
        {
           // m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Activate.");
        }

        void m_oIGTCustomCommandHelper_KeyPress(object sender, GTKeyPressEventArgs e)
        {
           // m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "KeyPress.");
        }

        void m_oIGTCustomCommandHelper_KeyDown(object sender, GTKeyEventArgs e)
        {
          //  m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "KeyDown.");
        }
        #endregion
        #endregion

        #region IGTCustomCommandModeless Members

        public void Activate(Intergraph.GTechnology.API.IGTCustomCommandHelper CustomCommandHelper)
        {
            m_gtapp = GTClassFactory.Create<IGTApplication>();
            m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Running Duct Path Placement ...");            
            m_oIGTCustomCommandHelper = CustomCommandHelper;
            mobjEditService = GTClassFactory.Create<IGTGeometryEditService>();
            mobjEditService.TargetMapWindow = m_gtapp.ActiveMapWindow;
            m_IGTDataContext = m_gtapp.DataContext;
            mobjRelationshipService = GTClassFactory.Create<IGTRelationshipService>();
            mobjRelationshipService.DataContext = m_IGTDataContext;
            mobjLocateService = m_gtapp.ActiveMapWindow.LocateService;
            
            foreach (IGTDDCKeyObject oDDCKeyObject in GTClassFactory.Create<IGTApplication>().SelectedObjects.GetObjects())
            {
                m_gtapp.SelectedObjects.Add(GTSelectModeConstants.gtsosmAllDisplayedComponentsInActiveLegend, oDDCKeyObject);
            }
            SubscribeEvents();
            
             m_CustomForm = new GTWindowsForm_DuctPathPlc();
             m_CustomForm.FormClosed += new FormClosedEventHandler(m_CustomForm_FormClosed);
             startdraw = 100;
          //  m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Select Source and Terminated Devices (together)!");
             //i = m_gtapp.SelectedObjects.FeatureCount;                          
        }
                
        void m_CustomForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            ExitCmd();
        }

        public bool CanTerminate
        {
            get
            {
                if (startdraw == 0)
                    m_CustomForm.Hide();
                DialogResult retVal = MessageBox.Show("Do you want to discard your current changes and exit?", "Duct Path Placement", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (retVal == DialogResult.Yes)
                {
                    ExitCmd();
                    //  return true;
                }
                else
                {
                    if (startdraw == 0)
                        m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "");m_CustomForm.Show();
                    return false;
                }

                return false;
            }
        }

        public void Pause()
        {

            startdraw += 50000;
        }

        public void Resume()
        {

            startdraw -= 50000;
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

        #region subscribe events
        public void SubscribeEvents()
        {
            // Subscribe to m_oIIGTCustomCommandHelper events using C# 1.0 syntax
           // m_oIGTCustomCommandHelper.Activate += new EventHandler<GTActivateEventArgs>(m_oIGTCustomCommandHelper_Activate);
           // m_oIGTCustomCommandHelper.Deactivate += new EventHandler<GTDeactivateEventArgs>(m_oIGTCustomCommandHelper_Deactivate);
          //  m_oIGTCustomCommandHelper.GainedFocus += new EventHandler<GTGainedFocusEventArgs>(m_oIGTCustomCommandHelper_GainedFocus);
           // m_oIGTCustomCommandHelper.LostFocus += new EventHandler<GTLostFocusEventArgs>(m_oIGTCustomCommandHelper_LostFocus);
         //   m_oIGTCustomCommandHelper.KeyUp += new EventHandler<GTKeyEventArgs>(m_oIGTCustomCommandHelper_KeyUp);
           // m_oIGTCustomCommandHelper.KeyDown += new EventHandler<GTKeyEventArgs>(m_oIGTCustomCommandHelper_KeyDown);
          //  m_oIGTCustomCommandHelper.KeyPress += new EventHandler<GTKeyPressEventArgs>(m_oIGTCustomCommandHelper_KeyPress);
            m_oIGTCustomCommandHelper.Click += new EventHandler<GTMouseEventArgs>(m_oIGTCustomCommandHelper_Click);
            m_oIGTCustomCommandHelper.DblClick += new EventHandler<GTMouseEventArgs>(m_oIGTCustomCommandHelper_DblClick);
            m_oIGTCustomCommandHelper.MouseMove += new EventHandler<GTMouseEventArgs>(m_oIGTCustomCommandHelper_MouseMove);
          //  m_oIGTCustomCommandHelper.MouseDown += new EventHandler<GTMouseEventArgs>(m_oIGTCustomCommandHelper_MouseDown);
            m_oIGTCustomCommandHelper.MouseUp += new EventHandler<GTMouseEventArgs>(m_oIGTCustomCommandHelper_MouseUp);
          //  m_oIGTCustomCommandHelper.WheelRotate += new EventHandler<GTWheelRotateEventArgs>(m_oIGTCustomCommandHelper_WheelRotate);
        
        }
        #endregion

        #region close application
        private void UnsubscribeEvents()
        {
            // UnSubscribe to m_oIIGTCustomCommandHelper events using C# 1.0 syntax
            //m_oIGTCustomCommandHelper.Activate -= m_oIGTCustomCommandHelper_Activate;
            //m_oIGTCustomCommandHelper.Deactivate -= m_oIGTCustomCommandHelper_Deactivate;
           // m_oIGTCustomCommandHelper.GainedFocus -= m_oIGTCustomCommandHelper_GainedFocus;
            //m_oIGTCustomCommandHelper.LostFocus -= m_oIGTCustomCommandHelper_LostFocus;
          //  m_oIGTCustomCommandHelper.KeyUp -= m_oIGTCustomCommandHelper_KeyUp;
            //m_oIGTCustomCommandHelper.KeyDown -= m_oIGTCustomCommandHelper_KeyDown;
            //m_oIGTCustomCommandHelper.KeyPress -= m_oIGTCustomCommandHelper_KeyPress;
            m_oIGTCustomCommandHelper.Click -= m_oIGTCustomCommandHelper_Click;
            m_oIGTCustomCommandHelper.DblClick -= m_oIGTCustomCommandHelper_DblClick;
            m_oIGTCustomCommandHelper.MouseMove -= m_oIGTCustomCommandHelper_MouseMove;
            //m_oIGTCustomCommandHelper.MouseDown -= m_oIGTCustomCommandHelper_MouseDown;
            m_oIGTCustomCommandHelper.MouseUp -= m_oIGTCustomCommandHelper_MouseUp;
            //m_oIGTCustomCommandHelper.WheelRotate -= m_oIGTCustomCommandHelper_WheelRotate;
        }

        public void ExitCmd()
        {
            if (mobjEditService != null)
            {
                mobjEditService.RemoveAllGeometries();
                mobjEditService = null;
            }

            if (mobjLocateService != null)
                mobjLocateService = null;
            m_gtapp.SelectedObjects.Clear();
            startdraw = 0;
            UnsubscribeEvents();
            if (mobjRelationshipService != null)
            {
                mobjRelationshipService = null;
            }
            
            m_oIGTCustomCommandHelper.Complete();
        }
        #endregion

     
       
    }
}
