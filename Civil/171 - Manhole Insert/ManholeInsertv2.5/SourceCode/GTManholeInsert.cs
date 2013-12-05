using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Intergraph.GTechnology.Interfaces;
using Intergraph.GTechnology.API;
using AG.GTechnology.Utilities;
using ADODB;

namespace AG.GTechnology.ManholeInsert
{
    class GTManholeInsert : Intergraph.GTechnology.Interfaces.IGTCustomCommandModeless
    {
        #region variables member
        public static Intergraph.GTechnology.API.IGTTransactionManager m_oIGTTransactionManager = null;
        public static Intergraph.GTechnology.API.IGTCustomCommandHelper m_oIGTCustomCommandHelper = null;
        private static Intergraph.GTechnology.API.IGTApplication m_gtapp = null;
        public static Intergraph.GTechnology.API.IGTDataContext m_IGTDataContext = null;

        IGTRelationshipService mobjRelationshipService = null;

        IGTGeometryEditService mobjEditServiceSlash = null;
        IGTGeometryEditService mobjEditService = null;
        IGTGeometryEditService mobjEditServiceRotate = null;

        IGTGeometryEditService mobjEditService1 = null;
        IGTGeometryEditService mobjEditServiceRotate1 = null;
        IGTGeometryEditService mobjEditService2 = null;
        IGTGeometryEditService mobjEditServiceRotate2 = null;
        IGTGeometryEditService mobjEditService3 = null;
        IGTGeometryEditService mobjEditServiceRotate3 = null;

        IGTGeometryEditService mobjEditServiceDuctPathFrom = null;
        IGTGeometryEditService mobjEditServiceDuctPathTo = null;
        IGTLocateService mobjLocateService = null;
      
        #region define Duct Path class
        public class TextPointGeom
        {
            public IGTTextPointGeometry geom;
            public int CID;
            public short CNO;
        };
        public class OrientedPointGeom
        {
            public IGTOrientedPointGeometry geom;
            public int CID;
            public short CNO;
        };
        public class PolylineGeom
        {
            public IGTPolylineGeometry geom;
            public int CID;
            public short CNO;
        };
        public class Duct
        {
            public int FID;
            public short FNO;
            public int styleID;
            public int OwnerFID;
            public short OwnerFNO;
            // public string Feature_state;
            public List<OrientedPointGeom> Form;
        };
        public class DuctNest
        {
            public int FID;
            public short FNO;
            //public string Feature_state;
            public int styleIDlabel;
            public int styleIDform;
            public List<TextPointGeom> Labels;
            public List<PolylineGeom> Form;
            public List<Duct> Ducts;

        };

        public class DuctPathSect
        {
            public string NumDuctWaysSect;
            public string SectionLength;
            public int SectGraphicLength;
            public string SectDiam;
            public string YearExpanded;
            public string YearExtended;
            public string SectOwner;
            public string SectType;
            public string SectPlc;
            public string SectBillingRate;
            public string Encasement;
            public string SectBackFill;
            public string PUSect;
            public short CID;
        };

        public class SectSlash
        {
            public int length;
            public IGTOrientedPointGeometry Slash;
            public short CID;
        };

        public class SectLabelLeaderLine
        {
            public IGTTextPointGeometry Label;
            public string LabelText;
            public int LabelAlight;
            public IGTPolylineGeometry LeaderLine;
            public short CID;
        };
        public class DuctPath
        {
            public int FID;
            public short FNO;
            public string Feature_state;
            public int sourceFID;
            public short sourceFNO;
            public int sourceWall;
            public string sourceType;
            public int termFID;
            public short termFNO;
            public int termWall;
            public string termType;
            public string EXC_ABB;
            public int Length;
            public int DuctWay;
            public string ConstructBy;
            public string InstallYear;
            public string BillingRate;
            public string DBFlag;
            public string Description;
            public string Feature_type;
            public int ASb_SubDuct;
            public int Prop_SubDuct;
            public int Prop_InnDuct;
            public int NestAssignDuctWay;
            public int DuctBalance;
            public string ExpandFlag;
            public IGTPolylineGeometry DuctPathLineGeom;
            public List<SectSlash> SectSlashes = null;
            public List<SectLabelLeaderLine> SectLabels = null;
            public List<DuctPathSect> Sections = null;
            public List<DuctNest> DuctNestFrom;
            public List<DuctNest> DuctNestTo;
        };
        #endregion

        int step = 0;
         DuctPath DuctPathOrigin = null;
        DuctPath DuctPathFrom = null;
        DuctPath DuctPathTo = null;
         SectSlash BreakSlash=new SectSlash();


        IGTGeometryEditService mobjEditPointService = null;
        IGTGeometryEditService mobjEditServiceTemp = null;

        IGTGeometryEditService mobjEditServiceNewTemp = null;
        IGTGeometryEditService mobjEditServiceNew = null;
        IGTGeometryEditService mobjEditPointServiceNew = null;

        IGTFeatureExplorerService mobjExplorerService = null;

        int CountGeomDuctNestFrom = 0;
        int CountGeomDuctNestTo = 0;

        #region manhl plac var
        int mintState = 0;
        IGTPoint mptSelected;
        IGTPoint mptPoint1;
        IGTPoint mptPoint2;
        IGTKeyObject mobjManholeAttribute = null;
        private short iManholeFNO = 2700;
        private ManholeCtrl cManhole = null;
        private string ManholeType = string.Empty;
        int iManholeFID = 0;
        private int StyleManhole = 2730001;
        private int StyleTempManhole = 2730001;

        Dictionary<string, string> Parameters = new Dictionary<string, string>();

        double _distance = 0;
        #endregion
        #endregion

        #region MouseMove
        void m_oIGTCustomCommandHelper_MouseMove(object sender, GTMouseEventArgs e)
        {

            IGTPoint WorldPoint = e.WorldPoint;

            #region Get Selected Duct Path to break
            if (step == 1)
            {
                m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Select Duct Path feature to break! Right Click to exit.");

                if (m_gtapp.SelectedObjects.FeatureCount == 1)
                {
                    foreach (IGTDDCKeyObject oDDCKeyObject in m_gtapp.SelectedObjects.GetObjects())
                    {
                        if (oDDCKeyObject.FNO != 2200)
                        {
                            MessageBox.Show("Please select a Duct Path!", "Manhole Insert", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            m_gtapp.ActiveMapWindow.Activate();
                            m_gtapp.SelectedObjects.Clear();
                            m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Select Duct Path feature to break Right Click to exit.");
                            return;
                        }
                        if (DuctPathOrigin == null)
                        {
                            Messages msg = new Messages();
                            msg.Message(1);
                            msg.Show();
                            m_gtapp.BeginProgressBar();
                            m_gtapp.SetProgressBarRange(0, 100);
                            m_gtapp.SetProgressBarPosition(25);
                            m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Wait, loading Duct Path information. Running Insert Manhole ver 2.5...");
                            if (GetDuctPathOrigin(oDDCKeyObject.FNO, oDDCKeyObject.FID))
                            {
                                if (DuctPathOrigin == null)
                                {
                                    if(msg!=null) msg.Close();
                                    MessageBox.Show("Only ASB and PPF features are allowed to be modified!", "Manhole Insert", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                    m_gtapp.ActiveMapWindow.Activate();
                                    m_gtapp.SelectedObjects.Clear();
                                    m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Select Duct Path feature to break! Right Click to exit.");
                                    return;
                                }
                                step = 2;

                                m_gtapp.SetProgressBarPosition(100);
                                m_gtapp.RefreshWindows();
                                m_gtapp.EndProgressBar();
                                if(msg!=null) msg.Close();
                                m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Place break slash! Right Click to exit.");
                                return;
                            }
                            else
                            {
                                if(msg!=null) msg.Close();
                                m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "");
                                if (DuctPathOrigin != null)
                                    DuctPathOrigin = null;
                                MessageBox.Show("Please select one more time!", "Manhole Insert", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                m_gtapp.ActiveMapWindow.Activate();
                                m_gtapp.SelectedObjects.Clear();

                                m_gtapp.SetProgressBarPosition(100);
                                m_gtapp.RefreshWindows();
                                m_gtapp.EndProgressBar();
                                m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Select Duct Path feature for editing! Right Click to exit.");
                                return;
                            }
                        }


                    }
                    m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Select Duct Path feature for editing! Right Click to exit.");

                }
                else if (m_gtapp.SelectedObjects.FeatureCount > 1)
                {
                    MessageBox.Show("Please select only one Duct Path at once!", "Manhole Insert", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    m_gtapp.SelectedObjects.Clear();
                    m_gtapp.ActiveMapWindow.Activate();
                    m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Select Duct Path feature for editing! Right Click to exit.");
                    return;
                }
                return;
            }
            #endregion

            #region break slash moving along line
            if (step == 2)
            {
                m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Point to accept new position of break-slash! Right Click to exit");

                IGTOrientedPointGeometry temp = GTClassFactory.Create<IGTOrientedPointGeometry>();
                IGTPoint tempp = GTClassFactory.Create<IGTPoint>();
                tempp.X = e.WorldPoint.X;
                tempp.Y = e.WorldPoint.Y;
                tempp.Z = 0.0;
                temp.Origin = tempp;
                IGTPoint projectPoint = PointOnConduit(tempp.X, tempp.Y, DuctPathOrigin.DuctPathLineGeom);

                if (projectPoint.X == 0 && projectPoint.Y == 0)
                {
                    return;
                }
                else
                {
                    temp.Origin = projectPoint;
                    IGTVector Orientation = GTClassFactory.Create<IGTVector>();
                    temp.Orientation = Orientation.BuildVector(projectPoint, tempp);
                    int length = int.Parse(projectPoint.Z.ToString());

                    if (length < 0 || length >= DuctPathOrigin.Length)
                            return;

                        if (mobjEditServiceSlash.GeometryCount > 0)
                    {
                        mobjEditServiceSlash.RemoveGeometry(mobjEditServiceSlash.GeometryCount);
                    }
                    mobjEditServiceSlash.AddGeometry(temp, 24500);
                    return;
                }
            }
            #endregion

            #region mh plc
            if (mintState == 4)
            {
                double X = WorldPoint.X;
                double Y = WorldPoint.Y;

                if (mptSelected == null) mptSelected = WorldPoint;

                double xManhole = WorldPoint.X;
                double yManhole = WorldPoint.Y;

                if (mobjEditServiceTemp.GeometryCount > 0) mobjEditServiceTemp.RemoveAllGeometries();

                //if (mobjEditPointService.GeometryCount > 0) mobjEditPointService.RemoveAllGeometries();
                IGTOrientedPointGeometry oManhole;
                oManhole = PGeoLib.CreatePointGeom(xManhole, yManhole);
                mobjEditServiceTemp.AddGeometry(oManhole, StyleTempManhole);

                m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Point to enter new location or right-click to accept current location");
            }
            else if (mintState == 41)
            {
                double X = WorldPoint.X;
                double Y = WorldPoint.Y;

                if (mptSelected == null) mptSelected = WorldPoint;
                double _angle = PGeoLib.GetAngle(mptSelected.X, mptSelected.Y, X, Y);
                double _len = PGeoLib.GetLength(mptSelected.X, mptSelected.Y, X, Y);

                double xManhole = WorldPoint.X;
                double yManhole = WorldPoint.Y;

                if (_len > _distance + 3)
                {
                    xManhole = mptSelected.X + Math.Cos(_angle) * (_distance + 3);
                    yManhole = mptSelected.Y + Math.Sin(_angle) * (_distance + 3);
                }

                if ((_len < _distance - 3) && (_distance > 3))
                {
                    xManhole = mptSelected.X + Math.Cos(_angle) * (_distance - 3);
                    yManhole = mptSelected.Y + Math.Sin(_angle) * (_distance - 3);
                }

                IGTLineGeometry oLine = PGeoLib.CreateLineGeom(mptSelected.X, mptSelected.Y, xManhole, yManhole);
                IGTTextPointGeometry oText = PGeoLib.CreateTextGeom((mptSelected.X + xManhole) / 2, (mptSelected.Y + yManhole) / 2,
                                        string.Format("{0:0.0}m", PGeoLib.GetLength(mptSelected.X, mptSelected.Y, xManhole, yManhole)),
                                        PGeoLib.Rad2Deg(_angle), GTAlignmentConstants.gtalCenterCenter);
                if (mobjEditServiceTemp.GeometryCount > 0) mobjEditServiceTemp.RemoveAllGeometries();
                mobjEditServiceTemp.AddGeometry(oLine, 11516);
                mobjEditServiceTemp.AddGeometry(oText, cManhole.ManholeWallStyle);

                //if (mobjEditPointService.GeometryCount > 0) mobjEditPointService.RemoveAllGeometries();
                IGTOrientedPointGeometry oManhole;
                oManhole = PGeoLib.CreatePointGeom(xManhole, yManhole);
                mobjEditServiceTemp.AddGeometry(oManhole, StyleTempManhole);

                m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Point to enter new location or right-click to accept current location");
            }
            else if (mintState == 5)
            {
                IGTPoint mptPoint2 = WorldPoint;
                mobjEditPointService.Rotate(mptPoint2);
                //CreateManholeGeom(mptPoint1, mptPoint2, mobjEditPointService);

                IGTLineGeometry oLine = PGeoLib.CreateLineGeom(mptPoint1.X, mptPoint1.Y, mptPoint2.X, mptPoint2.Y);
                if (mobjEditServiceTemp.GeometryCount > 0) mobjEditServiceTemp.RemoveAllGeometries();
                mobjEditServiceTemp.AddGeometry(oLine, 11516);

               
                m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Point to define new angle of facility or right-click to accept");
            }
            else if (mintState == 6)
            {
                m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Identify [+] to be wall 1");
            }
            else if (mintState == 7)
            {
                IGTPoint mptPoint2 = WorldPoint;

                IGTTextPointGeometry oText = PGeoLib.CreateTextGeom(mptPoint2.X, mptPoint2.Y, cManhole.ManholeLabel, 0, cManhole.ManholeLabelAlignment);
                if (mobjEditServiceTemp.GeometryCount > 0) mobjEditServiceTemp.RemoveAllGeometries();
                mobjEditServiceTemp.AddGeometry(oText, cManhole.ManholeTextStyle);

                m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Enter new location for text");
            }
            #endregion

            #region Get Selected Source Wall OR Term Wall of Device
            if (step == 5 || step == 50 || step == 501 || step == 51)
            {
                m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Select Wall to connect Duct Path!");
                m_gtapp.SelectedObjects.Clear();
                IGTPolylineGeometry DuctPathLineGeomNew = GTClassFactory.Create<IGTPolylineGeometry>();

                if (step == 5 || step == 51)
                {

                    {
                        double length = 0;
                        DuctPathLineGeomNew.Points.Clear();
                        DuctPathLineGeomNew.Points.Add(DuctPathOrigin.DuctPathLineGeom.Points[0]);
                        for (int i = 0; i < DuctPathOrigin.DuctPathLineGeom.Points.Count - 1; i++)
                        {
                            length += LegthBtwTwoPoints(DuctPathOrigin.DuctPathLineGeom.Points[i].X, DuctPathOrigin.DuctPathLineGeom.Points[i].Y,
                            DuctPathOrigin.DuctPathLineGeom.Points[i + 1].X, DuctPathOrigin.DuctPathLineGeom.Points[i + 1].Y);

                            if (length <= BreakSlash.length)
                                DuctPathLineGeomNew.Points.Add(DuctPathOrigin.DuctPathLineGeom.Points[i + 1]);
                            else break;
                        }
                        DuctPathLineGeomNew.Points.Add(WorldPoint);

                    }
                    if (mobjEditServiceDuctPathFrom.GeometryCount > 0)
                    {
                        mobjEditServiceDuctPathFrom.RemoveGeometry(mobjEditServiceDuctPathFrom.GeometryCount);
                    }
                    mobjEditServiceDuctPathFrom.AddGeometry(DuctPathLineGeomNew, 12000);
                   
                }
                else
                {
                    DuctPathLineGeomNew.Points.Clear();
                    DuctPathLineGeomNew.Points.Add(WorldPoint);
                    double length = 0;
                    for (int i = 1; i < DuctPathOrigin.DuctPathLineGeom.Points.Count; i++)
                    {
                        length += LegthBtwTwoPoints(DuctPathOrigin.DuctPathLineGeom.Points[i - 1].X, DuctPathOrigin.DuctPathLineGeom.Points[i - 1].Y,
                            DuctPathOrigin.DuctPathLineGeom.Points[i].X, DuctPathOrigin.DuctPathLineGeom.Points[i].Y);
                        if (length >= BreakSlash.length)
                            DuctPathLineGeomNew.Points.Add(DuctPathOrigin.DuctPathLineGeom.Points[i]);
                    }

                    if (mobjEditServiceDuctPathTo.GeometryCount > 0)
                    {
                        mobjEditServiceDuctPathTo.RemoveGeometry(mobjEditServiceDuctPathTo.GeometryCount);
                    }
                    mobjEditServiceDuctPathTo.AddGeometry(DuctPathLineGeomNew, 12000);

                }
                return;
            }
            #endregion

            #region label moving along line
            if (step == 6 || step == 60)
            {
                m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Point to accept new position of label! ");

                IGTTextPointGeometry temp = GTClassFactory.Create<IGTTextPointGeometry>();
                IGTPoint tempp = GTClassFactory.Create<IGTPoint>();
                tempp.X = e.WorldPoint.X;
                tempp.Y = e.WorldPoint.Y;
                tempp.Z = 0.0;
                temp.Origin = tempp;
                IGTPoint projectPoint =GTClassFactory.Create<IGTPoint>();
                if(step==6)
                    projectPoint = PointOnConduit(tempp.X, tempp.Y, DuctPathFrom.DuctPathLineGeom);
                if(step==60)
                    projectPoint = PointOnConduit(tempp.X, tempp.Y, DuctPathTo.DuctPathLineGeom);

                if (projectPoint.X == 0 && projectPoint.Y == 0)
                {
                    return;
                }
                else
                {
                    temp.Origin = projectPoint;
                   int length = int.Parse(projectPoint.Z.ToString());

                    if (step == 6)
                    {
                        if (length < 0 || length >= DuctPathFrom.Length)
                            return;
                        if (DuctPathFrom.SectSlashes != null)
                            if (DuctPathFrom.SectSlashes.Count > 0)
                                if (length < DuctPathFrom.SectSlashes[DuctPathFrom.SectSlashes.Count - 1].length)
                                    return;


                        temp.Rotation = TakeRotationOfSegmentPolyline(length, DuctPathFrom.DuctPathLineGeom);// OrientationForPointOnConduit(projectPoint.X, projectPoint.Y, length);
                        temp.Text = DuctPathFrom.SectLabels[DuctPathFrom.SectLabels.Count - 1].LabelText;
                        temp.Alignment =(GTAlignmentConstants)DuctPathFrom.SectLabels[DuctPathFrom.SectLabels.Count - 1].LabelAlight;

                        if (mobjEditServiceDuctPathFrom.GeometryCount > 1)
                        {
                            if (mobjEditServiceDuctPathFrom.GetGeometry(mobjEditServiceDuctPathFrom.GeometryCount).Type == "TextPointGeometry")
                                mobjEditServiceDuctPathFrom.RemoveGeometry(mobjEditServiceDuctPathFrom.GeometryCount);
                        }
                        mobjEditServiceDuctPathFrom.AddGeometry(temp, 32400);
                        return;
                    }
                    if (step == 60)
                    {
                        if (length < 0 || length >= DuctPathTo.Length)
                            return;
                        if (DuctPathTo.SectSlashes != null)
                            if (DuctPathTo.SectSlashes.Count > 0)
                                if (length > DuctPathTo.SectSlashes[0].length)
                                    return;


                        temp.Rotation = TakeRotationOfSegmentPolyline(length, DuctPathTo.DuctPathLineGeom);// OrientationForPointOnConduit(projectPoint.X, projectPoint.Y, length);
                        temp.Text = DuctPathTo.SectLabels[0].LabelText;
                        temp.Alignment = (GTAlignmentConstants)DuctPathTo.SectLabels[0].LabelAlight;

                        if (mobjEditServiceDuctPathTo.GeometryCount > 1)
                        {
                            if (mobjEditServiceDuctPathTo.GetGeometry(mobjEditServiceDuctPathTo.GeometryCount).Type == "TextPointGeometry")
                                mobjEditServiceDuctPathTo.RemoveGeometry(mobjEditServiceDuctPathTo.GeometryCount);
                        }
                        mobjEditServiceDuctPathTo.AddGeometry(temp, 32400);
                        return;
                    }
                   
                }
            }
            #endregion


            #region move duct nest
            if (step == 7 || step==70)
            {

                m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Point to confirm location!");
                m_gtapp.SelectedObjects.Clear();
                IGTDDCKeyObjects feat = null;


                if (step == 70)
                {
                    mobjEditService3.RemoveAllGeometries();
                    mobjEditServiceRotate3.RemoveAllGeometries();
                    int i = CountGeomDuctNestFrom;


                    feat = m_gtapp.DataContext.GetDDCKeyObjects(DuctPathTo.DuctNestFrom[i].FNO, DuctPathTo.DuctNestFrom[i].FID, GTComponentGeometryConstants.gtddcgAllGeographic);
                    for (int K2 = 0; K2 < feat.Count; K2++)
                        m_gtapp.SelectedObjects.Add(GTSelectModeConstants.gtsosmAllDisplayedComponentsInActiveLegend, feat[K2]);
                    //  m_gtapp.RefreshWindows();

                    for (int j = 0; j < DuctPathTo.DuctNestFrom[i].Form.Count; j++)
                    {
                        mobjEditService3.AddGeometry(DuctPathTo.DuctNestFrom[i].Form[j].geom, DuctPathTo.DuctNestFrom[i].styleIDform);
                        //   mobjEditServiceRotate.AddGeometry(DuctPathOrigin.DuctNestFrom[i].Form[j], DuctPathOrigin.DuctNestFrom[i].styleIDform);

                    }
                    for (int j = 0; j < DuctPathTo.DuctNestFrom[i].Labels.Count; j++)
                    {
                        mobjEditService3.AddGeometry(DuctPathTo.DuctNestFrom[i].Labels[j].geom, DuctPathTo.DuctNestFrom[i].styleIDlabel);
                        //  mobjEditServiceRotate.AddGeometry(DuctPathOrigin.DuctNestFrom[i].Labels[j], DuctPathOrigin.DuctNestFrom[i].styleIDlabel);

                    }

                    for (int j = 0; j < DuctPathTo.DuctNestFrom[i].Ducts.Count; j++)
                    {
                        for (int k = 0; k < DuctPathTo.DuctNestFrom[i].Ducts[j].Form.Count; k++)
                        {
                            mobjEditService3.AddGeometry(DuctPathTo.DuctNestFrom[i].Ducts[j].Form[k].geom, DuctPathTo.DuctNestFrom[i].Ducts[j].styleID);
                            //   mobjEditServiceRotate.AddGeometry(DuctPathOrigin.DuctNestFrom[i].Ducts[j].Form[k], DuctPathOrigin.DuctNestFrom[i].Ducts[j].styleID);
                        }
                    }
                    mobjEditService3.BeginMove(mobjEditService3.GetGeometry(1).FirstPoint);
                    step = 701;
                    return;
                }
                else
                {
                    int i = CountGeomDuctNestTo; 
                    feat = m_gtapp.DataContext.GetDDCKeyObjects(DuctPathFrom.DuctNestTo[i].FNO, DuctPathFrom.DuctNestTo[i].FID, GTComponentGeometryConstants.gtddcgAllGeographic);
                    for (int K2 = 0; K2 < feat.Count; K2++)
                        m_gtapp.SelectedObjects.Add(GTSelectModeConstants.gtsosmAllDisplayedComponentsInActiveLegend, feat[K2]);
                    //  m_gtapp.RefreshWindows();
                    for (int j = 0; j < DuctPathFrom.DuctNestTo[i].Form.Count; j++)
                    {
                        mobjEditService3.AddGeometry(DuctPathFrom.DuctNestTo[i].Form[j].geom, DuctPathFrom.DuctNestTo[i].styleIDform);
                        //   mobjEditServiceRotate.AddGeometry(DuctPathOrigin.DuctNestTo[i].Form[j], DuctPathOrigin.DuctNestTo[i].styleIDform);
                    }
                    for (int j = 0; j < DuctPathFrom.DuctNestTo[i].Labels.Count; j++)
                    {
                        mobjEditService3.AddGeometry(DuctPathFrom.DuctNestTo[i].Labels[j].geom, DuctPathFrom.DuctNestTo[i].styleIDlabel);
                        //   mobjEditServiceRotate.AddGeometry(DuctPathOrigin.DuctNestTo[i].Labels[j], DuctPathOrigin.DuctNestTo[i].styleIDlabel);
                    }

                    for (int j = 0; j < DuctPathFrom.DuctNestTo[i].Ducts.Count; j++)
                    {
                        for (int k = 0; k < DuctPathFrom.DuctNestTo[i].Ducts[j].Form.Count; k++)
                        {
                            mobjEditService3.AddGeometry(DuctPathFrom.DuctNestTo[i].Ducts[j].Form[k].geom, DuctPathFrom.DuctNestTo[i].Ducts[j].styleID);
                            //  mobjEditServiceRotate.AddGeometry(DuctPathOrigin.DuctNestTo[i].Ducts[j].Form[k], DuctPathOrigin.DuctNestTo[i].Ducts[j].styleID);

                        }
                    }
                    mobjEditService3.BeginMove(mobjEditService3.GetGeometry(1).FirstPoint);
                    step = 71;
                    return;
                }
                
            }
            if (step == 71 || step == 701)
            {

                m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Point to confirm location!");
                mobjEditService3.Move(WorldPoint);
                return;
            }
            //if (step == 701)
            //{

            //    m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Point to confirm location!");
            //    mobjEditService3.Move(WorldPoint);
            //    return;
            //}
            #endregion
            #region rotating nest
            if (step == 8 || step == 80)
            {

                m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Point to confirm rotation! Right Click  to skip rotation.");
                if (step == 80)
                {

                    int countgeom = 0; //mobjEditService.GeometryCount;
                    int i = CountGeomDuctNestFrom;
                    for (int j = 0; j < DuctPathOrigin.DuctNestFrom[i].Form.Count; j++)
                    {
                        countgeom++;
                        mobjEditServiceRotate3.AddGeometry(mobjEditService3.GetGeometry(countgeom), DuctPathOrigin.DuctNestFrom[i].styleIDform);

                    }
                    for (int j = 0; j < DuctPathOrigin.DuctNestFrom[i].Labels.Count; j++)
                    {
                        countgeom++;
                        mobjEditServiceRotate3.AddGeometry(mobjEditService3.GetGeometry(countgeom), DuctPathOrigin.DuctNestFrom[i].styleIDlabel);

                    }

                    for (int j = 0; j < DuctPathOrigin.DuctNestFrom[i].Ducts.Count; j++)
                    {
                        for (int k = 0; k < DuctPathOrigin.DuctNestFrom[i].Ducts[j].Form.Count; k++)
                        {
                            countgeom++;
                            mobjEditServiceRotate3.AddGeometry(mobjEditService3.GetGeometry(countgeom), DuctPathOrigin.DuctNestFrom[i].Ducts[j].styleID);
                        }
                    }

                    mobjEditServiceRotate3.BeginRotate(mobjEditServiceRotate3.GetGeometry(1).FirstPoint, mobjEditServiceRotate3.GetGeometry(1).FirstPoint);
                    step = 801;
                    return;
                }
                else
                {
                    int countgeom = 0; //mobjEditService.GeometryCount;
                    int i = CountGeomDuctNestTo;
                    for (int j = 0; j < DuctPathOrigin.DuctNestTo[i].Form.Count; j++)
                    {
                        countgeom++;
                        mobjEditServiceRotate3.AddGeometry(mobjEditService3.GetGeometry(countgeom), DuctPathOrigin.DuctNestTo[i].styleIDform);

                    }
                    for (int j = 0; j < DuctPathOrigin.DuctNestTo[i].Labels.Count; j++)
                    {
                        countgeom++;
                        mobjEditServiceRotate3.AddGeometry(mobjEditService3.GetGeometry(countgeom), DuctPathOrigin.DuctNestTo[i].styleIDlabel);

                    }

                    for (int j = 0; j < DuctPathOrigin.DuctNestTo[i].Ducts.Count; j++)
                    {
                        for (int k = 0; k < DuctPathOrigin.DuctNestTo[i].Ducts[j].Form.Count; k++)
                        {
                            countgeom++;
                            mobjEditServiceRotate3.AddGeometry(mobjEditService3.GetGeometry(countgeom), DuctPathOrigin.DuctNestTo[i].Ducts[j].styleID);
                        }
                    }
                    mobjEditServiceRotate3.BeginRotate(mobjEditServiceRotate3.GetGeometry(1).FirstPoint, mobjEditServiceRotate3.GetGeometry(1).FirstPoint);
                    step = 81;
                    return;
                }
              
            }
            if (step == 81 || step == 801)
            {

                m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Point to confirm rotation! Right Click  to skip rotation.");
                mobjEditServiceRotate3.Rotate(WorldPoint);
                return;
            }
            //if (step == 801)
            //{

            //    m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Point to confirm rotation! Right Click  to skip rotation.");
            //    mobjEditServiceRotate3.Rotate(WorldPoint);
            //    return;
            //}
            #endregion
        }
        #endregion

        #region DblClick
        void m_oIGTCustomCommandHelper_DblClick(object sender, GTMouseEventArgs e)
        {
            #region break slash accept
            if (step == 2 || step == 3)
            {
                if (BreakSlash == null)
                    BreakSlash = new SectSlash();
                BreakSlash.Slash = (IGTOrientedPointGeometry)mobjEditServiceSlash.GetGeometry(1);
                BreakSlash.length = (int)BreakSlash.Slash.Origin.Z;
                m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Please enter attributes for inserting Manhole! ");
                step = 0;
                StartMHplc();
                return;
            }
            #endregion 

            #region label accept
            if (step == 6 || step == 61)
            {
                DuctPathFrom.SectLabels[DuctPathFrom.SectLabels.Count - 1].LeaderLine = null;
                DuctPathFrom.SectLabels[DuctPathFrom.SectLabels.Count - 1].Label = (IGTTextPointGeometry)mobjEditServiceDuctPathFrom.GetGeometry(2);

                if (DuctPathOrigin.DuctNestTo != null)
                    if (DuctPathOrigin.DuctNestTo.Count > 0)
                    {
                        LocateFeature(3, m_gtapp.ActiveMapWindow, DuctPathOrigin.sourceFNO, DuctPathOrigin.sourceFID, iManholeFNO, iManholeFID);
                        
                        step = 7;//duct nest plc
                        return ;
                    }
                LocateFeature(3, m_gtapp.ActiveMapWindow, DuctPathOrigin.sourceFNO, DuctPathOrigin.sourceFID, iManholeFNO, iManholeFID);
                        
                        
                step = 50;
                return;
            }
            if (step == 60 || step == 601)
            {
                DuctPathTo.SectLabels[0].LeaderLine = null;
                DuctPathTo.SectLabels[0].Label = (IGTTextPointGeometry)mobjEditServiceDuctPathTo.GetGeometry(2);

                if (DuctPathOrigin.DuctNestFrom != null)

                    if (DuctPathOrigin.DuctNestFrom.Count > 0)
                    {
                       LocateFeature(1, m_gtapp.ActiveMapWindow, DuctPathTo.sourceFNO, DuctPathTo.sourceFID, DuctPathTo.termFNO, DuctPathTo.termFID);
                       step = 70;//duct nest plc
                        return ;
                    }
                BreakDuctPath();
               
                return;
            }
            #endregion 
        }
        #endregion
               

        #region MouseUp
        void m_oIGTCustomCommandHelper_MouseUp(object sender, GTMouseEventArgs e)
        {
             IGTPoint WorldPoint = e.WorldPoint;

             if (e.Button == 1)//left button
             {
                 #region Get Selected Duct Path to break
                 if (step == 1)
                 {
                     m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Select Duct Path feature to break! Right Click to exit.");

                     IGTDDCKeyObjects feat = mobjLocateService.Locate(WorldPoint, 20, 1, GTSelectionTypeConstants.gtmwstSelectSingle);
                     for (int K = 0; K < feat.Count; K++)
                         m_gtapp.SelectedObjects.Add(GTSelectModeConstants.gtsosmAllDisplayedComponentsInActiveLegend, feat[K]);

                     if (m_gtapp.SelectedObjects.FeatureCount == 1)
                     {
                         foreach (IGTDDCKeyObject oDDCKeyObject in m_gtapp.SelectedObjects.GetObjects())
                         {
                             if (oDDCKeyObject.FNO != 2200)
                             {
                                 MessageBox.Show("Please select a Duct Path!", "Manhole Insert", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                 m_gtapp.ActiveMapWindow.Activate();
                                 m_gtapp.SelectedObjects.Clear();
                                 m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Select Duct Path feature to break! Right Click to exit.");
                                 return;
                             }

                             if (DuctPathOrigin == null)
                             {
                                 Messages msg = new Messages();
                                 msg.Message(1);
                                 msg.Show();
                                 m_gtapp.BeginProgressBar();
                                 m_gtapp.SetProgressBarRange(0, 100);
                                 m_gtapp.SetProgressBarPosition(25);
                                 m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Wait, loading Duct Path information. Running Insert Manhole ver 2.5...");
                                 if (GetDuctPathOrigin(oDDCKeyObject.FNO, oDDCKeyObject.FID))
                                 {
                                     if (DuctPathOrigin == null)
                                     {
                                         if(msg!=null) msg.Close();
                                         MessageBox.Show("Only ASB and PPF features are allowed to be modified!", "Manhole Insert", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                         m_gtapp.ActiveMapWindow.Activate();
                                         m_gtapp.SelectedObjects.Clear();
                                         m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Select Duct Path feature to break! Right Click to exit.");
                                         return;
                                     }
                                     step = 2;

                                     m_gtapp.SetProgressBarPosition(100);
                                     m_gtapp.RefreshWindows();
                                     m_gtapp.EndProgressBar();
                                     if(msg!=null) msg.Close();
                                     m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Place break slash! Right Click to exit.");

                                     return;
                                 }
                                 else
                                 {
                                     if(msg!=null) msg.Close();
                                     if (DuctPathOrigin != null)
                                         DuctPathOrigin = null;
                                     MessageBox.Show("Please select one more time!", "Manhole Insert", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                     m_gtapp.ActiveMapWindow.Activate();
                                     m_gtapp.SelectedObjects.Clear();
                                     m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Select Duct Path feature to break! Right Click to exit.");
                                     return;
                                 }
                             }


                         }
                         m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Select Duct Path feature to break! Right Click to exit.");

                     }
                     else if (m_gtapp.SelectedObjects.FeatureCount > 1)
                     {
                         MessageBox.Show("Please select only one Duct Path at once!", "Manhole Insert", MessageBoxButtons.OK, MessageBoxIcon.Information);
                         m_gtapp.SelectedObjects.Clear();
                         m_gtapp.ActiveMapWindow.Activate();
                         m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Select Duct Path feature to break! Right Click to exit.");
                         return;
                     }
                     return;
                 }
                 #endregion

                 #region break slash temporary accept
                 if (step == 2 || step == 3)
                 {
                     step = 3;
                     m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Double click to accept position of break-slash! Right click to start moving along line.");
                    return;
                 }
                 #endregion

                 #region mh plc
                 if (mintState == 4)
                 {
                     mintState = -1;

                     m_gtapp.ActiveMapWindow.HighlightedObjects.Clear();

                     double X = WorldPoint.X;
                     double Y = WorldPoint.Y;

                     double xManhole = WorldPoint.X;
                     double yManhole = WorldPoint.Y;

                     mptPoint1 = GTClassFactory.Create<IGTPoint>();
                     mptPoint1.X = xManhole;
                     mptPoint1.Y = yManhole;

                     if (mobjEditPointService.GeometryCount > 0) mobjEditPointService.RemoveAllGeometries();
                     if (mobjEditServiceTemp.GeometryCount > 0) mobjEditServiceTemp.RemoveAllGeometries();

                     //if (mobjEditService.GeometryCount > 0) mobjEditService.RemoveAllGeometries();
                     //IGTOrientedPointGeometry oManhole;
                     //oManhole = CreatePointGeom(xManhole, yManhole);
                     //mobjEditService.AddGeometry(oManhole, StyleManhole);
                     if (mobjEditService.GeometryCount > 0) mobjEditService.RemoveAllGeometries();
                     CreateManholeGeom(mptPoint1, null, mobjEditService);
                     mintState = 4;
                 }
                 else if (mintState == 41)
                 {
                     mintState = -1;

                     m_gtapp.ActiveMapWindow.HighlightedObjects.Clear();

                     double X = WorldPoint.X;
                     double Y = WorldPoint.Y;

                     double _angle = PGeoLib.GetAngle(mptSelected.X, mptSelected.Y, X, Y);
                     double _len = PGeoLib.GetLength(mptSelected.X, mptSelected.Y, X, Y);

                     double xManhole = WorldPoint.X;
                     double yManhole = WorldPoint.Y;

                     if (_len > _distance + 3)
                     {
                         xManhole = mptSelected.X + Math.Cos(_angle) * (_distance + 3);
                         yManhole = mptSelected.Y + Math.Sin(_angle) * (_distance + 3);
                     }

                     if ((_len < _distance - 3) && (_distance > 3))
                     {
                         xManhole = mptSelected.X + Math.Cos(_angle) * (_distance - 3);
                         yManhole = mptSelected.Y + Math.Sin(_angle) * (_distance - 3);
                     }

                     mptPoint1 = GTClassFactory.Create<IGTPoint>();
                     mptPoint1.X = xManhole;
                     mptPoint1.Y = yManhole;

                     if (mobjEditPointService.GeometryCount > 0) mobjEditPointService.RemoveAllGeometries();
                     if (mobjEditServiceTemp.GeometryCount > 0) mobjEditServiceTemp.RemoveAllGeometries();

                     //if (mobjEditService.GeometryCount > 0) mobjEditService.RemoveAllGeometries();
                     //IGTOrientedPointGeometry oManhole;
                     //oManhole = CreatePointGeom(xManhole, yManhole);
                     //mobjEditService.AddGeometry(oManhole, StyleManhole);
                     if (mobjEditService.GeometryCount > 0) mobjEditService.RemoveAllGeometries();
                     CreateManholeGeom(mptPoint1, null, mobjEditService);


                     mintState = 41;
                 }
                 else if (mintState == 5)
                 {
                     mintState = -1;
                     mptPoint2 = WorldPoint;
                     cManhole.RotationPnt = mptPoint2;

                     //mobjEditService.Rotate(mptPoint2);

                     //if (mobjEditPointService.GeometryCount > 0) mobjEditPointService.RemoveAllGeometries();
                     //if (mobjEditService.GeometryCount > 0) mobjEditPointService.AddGeometry(mobjEditService.GetGeometry(1), StyleManhole);
                     if (mobjEditService.GeometryCount > 0) mobjEditService.RemoveAllGeometries();
                     CreateManholeGeom(mptPoint1, mptPoint2, mobjEditService);


                     mintState = 5;
                 }
                 else if (mintState == 6)
                 {
                     mintState = -1;
                     ManholePoint _pnt1 = cManhole.ManholeWall[0];
                     int fNbr = 0;

                     if (mptPoint2 == null)
                     {
                         double len1 = PGeoLib.GetLength(WorldPoint.X, WorldPoint.Y, cManhole.X + _pnt1.X, cManhole.Y + _pnt1.Y);
                         for (int i = 0; i < cManhole.ManholeWall.Count; i++)
                         {
                             ManholePoint _pnt = cManhole.ManholeWall[i];
                             double len2 = PGeoLib.GetLength(WorldPoint.X, WorldPoint.Y, cManhole.X + _pnt.X, cManhole.Y + _pnt.Y);
                             if (len2 < len1)
                             {
                                 fNbr = i;
                                 len1 = len2;
                             }
                         }
                     }
                     else
                     {
                         IGTPoint objPointTmp = GTClassFactory.Create<IGTPoint>();
                         double angle = PGeoLib.GetAngle(mptPoint1.X, mptPoint1.Y, mptPoint2.X, mptPoint2.Y);
                         objPointTmp.X = cManhole.X + _pnt1.X;
                         objPointTmp.Y = cManhole.Y + _pnt1.Y;
                         double txtX = PGeoLib.RotatePoint(mptPoint1, angle, objPointTmp).X;
                         double txtY = PGeoLib.RotatePoint(mptPoint1, angle, objPointTmp).Y;
                         double len1 = PGeoLib.GetLength(WorldPoint.X, WorldPoint.Y, txtX, txtY);
                         for (int i = 0; i < cManhole.ManholeWall.Count; i++)
                         {
                             ManholePoint _pnt = cManhole.ManholeWall[i];
                             objPointTmp.X = cManhole.X + _pnt.X;
                             objPointTmp.Y = cManhole.Y + _pnt.Y;
                             txtX = PGeoLib.RotatePoint(mptPoint1, angle, objPointTmp).X;
                             txtY = PGeoLib.RotatePoint(mptPoint1, angle, objPointTmp).Y;
                             //IGTTextPointGeometry txtNumber2 = PGeoLib.CreateTextGeom(txtX, txtY, "o" + Convert.ToString(i));
                             //mobjEditServiceTemp.AddGeometry(txtNumber2, cManhole.ManholeWallStyle);

                             double len2 = PGeoLib.GetLength(WorldPoint.X, WorldPoint.Y, txtX, txtY);
                             if (len2 < len1)
                             {
                                 fNbr = i;
                                 len1 = len2;
                             }
                         }
                     }

                     cManhole.FirstNumber = fNbr;

                     if (mobjEditPointService.GeometryCount > 0) mobjEditPointService.RemoveAllGeometries();
                     if (mobjEditServiceTemp.GeometryCount > 0) mobjEditServiceTemp.RemoveAllGeometries();
                     if (mobjEditService.GeometryCount > 0) mobjEditService.RemoveAllGeometries();

                     if (mobjEditPointServiceNew.GeometryCount > 0) mobjEditPointServiceNew.RemoveAllGeometries();
                     if (mobjEditServiceNewTemp.GeometryCount > 0) mobjEditServiceNewTemp.RemoveAllGeometries();
                     if (mobjEditServiceNew.GeometryCount > 0) mobjEditServiceNew.RemoveAllGeometries();

                     for (int i = 0; i < cManhole.ManholeWall.Count; i++)
                     {
                         if (fNbr >= cManhole.ManholeWall.Count) fNbr = 0;
                         ManholePoint _pnt = cManhole.ManholeWall[fNbr];
                         IGTTextPointGeometry txtNumber = PGeoLib.CreateTextGeom(cManhole.X + _pnt.X, cManhole.Y + _pnt.Y, Convert.ToString(i + 1));
                         mobjEditService.AddGeometry(txtNumber, cManhole.ManholeWallStyle);


                         fNbr++;
                     }

                     //IGTTextPointGeometry txtNumber1 = PGeoLib.CreateTextGeom(WorldPoint.X, WorldPoint.Y, "x");
                     //mobjEditService.AddGeometry(txtNumber1, cManhole.ManholeWallStyle);

                     CreateManholeGeom(mptPoint1, mptPoint2, mobjEditService);

                     mintState = 7;
                 }
                 else if (mintState == 7)
                 {
                     mintState = -1;

                     cManhole.LabelOrigin = WorldPoint;
                     CreateManhole(cManhole);
                     m_gtapp.SelectedObjects.Clear();
                 }
                 #endregion

                 #region Get Selected Source Wall OR Term Wall of Device
                 if (step == 51 || step == 501)
                 {
                     if (step == 51)
                         step = 5;
                     if (step == 501)
                         step = 50;
                     m_gtapp.SelectedObjects.Clear();
                     m_gtapp.RefreshWindows();
                     return;
                 }
                 if (step == 5 || step == 50)
                 {

                     m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Select Wall to connect Duct Path!");
                     IGTDDCKeyObjects feat = null;
                     feat = mobjLocateService.Locate(WorldPoint, 20, 1, GTSelectionTypeConstants.gtmwstSelectSingle);
                     for (int K = 0; K < feat.Count; K++)
                         m_gtapp.SelectedObjects.Add(GTSelectModeConstants.gtsosmAllDisplayedComponentsInActiveLegend, feat[K]);

                     if (m_gtapp.SelectedObjects.FeatureCount == 1)
                     {
                         if ((step == 5 && GetDeviceWithWall(true))//source
                             || (step == 50 && GetDeviceWithWall(false)))//term
                         {
                            return;
                         }
                         else
                         {
                             if (step == 5)
                                 step = 51;
                             if (step == 50)
                                 step = 501;
                             m_gtapp.SelectedObjects.Clear();
                             m_gtapp.RefreshWindows();
                             return;
                         }
                     }

                 }
                 #endregion

                 #region label temporary accept
                 if (step == 6)
                 {
                     step = 61;
                     m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Double click to accept position of label! Right click to start moving along line.");
                     return;
                 }
                 if (step == 60)
                 {
                     step = 601;
                     m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Double click to accept position of label! Right click to start moving along line.");
                     return;
                 }
                 #endregion

                 #region moving nest
                 if (step == 71)
                 {
                     mobjEditService3.EndMove(WorldPoint);
                     step = 8;
                     return;
                 }
                 if (step == 701)
                 {
                     mobjEditService3.EndMove(WorldPoint);
                     step = 80;
                     return;
                 }
                 #endregion

                 #region rotating nest
                   
                     if (step == 801)
                     {

                         mobjEditServiceRotate3.EndRotate(WorldPoint);
                         mobjEditService3.RemoveAllGeometries();
                         int i = CountGeomDuctNestFrom;
                         int y = 1;

                         for (int j = 0; j < DuctPathTo.DuctNestFrom[i].Form.Count; j++)
                         {
                             mobjEditServiceRotate1.AddGeometry(mobjEditServiceRotate3.GetGeometry(y).Copy(), DuctPathTo.DuctNestFrom[i].styleIDform);
                             y++;
                         }
                         for (int j = 0; j < DuctPathTo.DuctNestFrom[i].Labels.Count; j++)
                         {
                             mobjEditServiceRotate1.AddGeometry(mobjEditServiceRotate3.GetGeometry(y).Copy(), DuctPathTo.DuctNestFrom[i].styleIDlabel);
                             y++;
                         }

                         for (int j = 0; j < DuctPathTo.DuctNestFrom[i].Ducts.Count; j++)
                         {
                             for (int k = 0; k < DuctPathTo.DuctNestFrom[i].Ducts[j].Form.Count; k++)
                             {
                                 mobjEditServiceRotate1.AddGeometry(mobjEditServiceRotate3.GetGeometry(y).Copy(), DuctPathTo.DuctNestFrom[i].Ducts[j].styleID);
                                 y++;
                             }
                         }

                         DuctPathTo.DuctNestFrom[CountGeomDuctNestFrom] = UpdateGeomDuctNest(DuctPathTo.DuctNestFrom[CountGeomDuctNestFrom], false);
                         mobjEditServiceRotate3.RemoveAllGeometries();


                         if (CountGeomDuctNestFrom < DuctPathTo.DuctNestFrom.Count - 1)
                         {
                             CountGeomDuctNestFrom++;
                             step = 70;
                         }
                         else
                         {
                             step = 0;
                             //CountGeomDuctNestTo = 0;
                           BreakDuctPath();
                //             LocateFeature(3, m_gtapp.ActiveMapWindow, DuctPathFrom.sourceFNO, DuctPathFrom.sourceFID, DuctPathFrom.termFNO, DuctPathFrom.termFID);
                //LocateFeature(1, m_gtapp.ActiveMapWindow, DuctPathTo.sourceFNO, DuctPathTo.sourceFID, DuctPathTo.termFNO, DuctPathTo.termFID);
                //LocateFeature(2, m_gtapp.ActiveMapWindow, DuctPathFrom.sourceFNO, DuctPathFrom.sourceFID, DuctPathFrom.termFNO, DuctPathFrom.termFID);
                //LocateFeature(2, m_gtapp.ActiveMapWindow, DuctPathTo.sourceFNO, DuctPathTo.sourceFID, DuctPathTo.termFNO, DuctPathTo.termFID);
                
                             return;
                         }
                         
                     }

                     if (step == 81)
                     {
                   //      mobjEditServiceRotate2.EndRotate(WorldPoint);
                      //   mobjEditService2.RemoveAllGeometries();
                         mobjEditServiceRotate3.EndRotate(WorldPoint);
                         mobjEditService3.RemoveAllGeometries();

                         int i = CountGeomDuctNestTo;
                         int y = 1;
                         for (int j = 0; j < DuctPathFrom.DuctNestTo[i].Form.Count; j++)
                         {
                             mobjEditServiceRotate2.AddGeometry(mobjEditServiceRotate3.GetGeometry(y).Copy(), DuctPathFrom.DuctNestTo[i].styleIDform);
                             y++;
                         }
                         for (int j = 0; j < DuctPathFrom.DuctNestTo[i].Labels.Count; j++)
                         {
                             mobjEditServiceRotate2.AddGeometry(mobjEditServiceRotate3.GetGeometry(y).Copy(), DuctPathFrom.DuctNestTo[i].styleIDlabel);
                             y++;

                         }

                         for (int j = 0; j < DuctPathFrom.DuctNestTo[i].Ducts.Count; j++)
                         {
                             for (int k = 0; k < DuctPathFrom.DuctNestTo[i].Ducts[j].Form.Count; k++)
                             {
                                 mobjEditServiceRotate2.AddGeometry(mobjEditServiceRotate3.GetGeometry(y).Copy(), DuctPathFrom.DuctNestTo[i].Ducts[j].styleID);
                                 y++;
                             }
                         }

                         DuctPathFrom.DuctNestTo[CountGeomDuctNestTo] = UpdateGeomDuctNest(DuctPathFrom.DuctNestTo[CountGeomDuctNestTo], true);

                         mobjEditServiceRotate3.RemoveAllGeometries();


                         if (CountGeomDuctNestTo < DuctPathFrom.DuctNestTo.Count - 1)
                         {
                             CountGeomDuctNestTo++;
                             step = 7;
                         }
                         else
                         {
                             // step = 0;
                             // CountGeomDuctNestFrom = 0;
                          //   LocateFeature(1, m_gtapp.ActiveMapWindow, DuctPathTo.sourceFNO, DuctPathTo.sourceFID, DuctPathTo.termFNO, DuctPathTo.termFID);
                        
                             step = 50;

                             return;
                         }
                     }

                 #endregion
             }
             else if (e.Button == 2)//right click
             {
                 #region exiting from application
                 if (step == 1 || step == 2)//exiting from application
                 {
                     DialogResult retVal = MessageBox.Show("Are you sure that you want to exit?", "Manhole Insert", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                     if (retVal == DialogResult.Yes)
                     {
                         ExitCmd();
                         return;
                     }
                     m_gtapp.ActiveMapWindow.Activate();
                     return;
                 }
                 #endregion

                 #region break slash temporary accept
                 if (step == 3)
                 {
                     step = 2;
                     m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Point to accept new position of break-slash! Right Click to exit");
                     return;
                 }
                 #endregion

                 #region mh plc
                 if ((mintState == 4) || (mintState == 41))
                 {
                     mintState = -1;
                     double xManhole = WorldPoint.X;
                     double yManhole = WorldPoint.Y;

                     if (mptPoint1 == null)
                     {
                         mptPoint1 = WorldPoint;

                         //if (mobjEditService.GeometryCount > 0) mobjEditService.RemoveAllGeometries();
                         //IGTOrientedPointGeometry oManhole;
                         //oManhole = CreatePointGeom(xManhole, yManhole);
                         //mobjEditService.AddGeometry(oManhole, StyleManhole);
                     }

                     if (mobjEditService.GeometryCount > 0) mobjEditService.RemoveAllGeometries();
                     CreateManholeGeom(mptPoint1, null, mobjEditService);

                     if (mobjEditPointService.GeometryCount > 0) mobjEditPointService.RemoveAllGeometries();
                     CreateManholeGeom(mptPoint1, null, mobjEditPointService, StyleTempManhole);
                     IGTPoint objPoint = GTClassFactory.Create<IGTPoint>();
                     objPoint.X = mptPoint1.X + 100;
                     objPoint.Y = mptPoint1.Y;
                     mobjEditPointService.BeginRotate(objPoint, mptPoint1);
                     cManhole.RotationPnt = null;



                     m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Point to define angle of facility or right-click to complete");
                     mintState = 5;
                 }
                 else if (mintState == 5)
                 {
                     mintState = -1;
                     cManhole.X = mptPoint1.X;
                     cManhole.Y = mptPoint1.Y;
                     cManhole.Origin = mptPoint1;

                     //if (mptPoint2 != null)
                     //{
                     //mobjEditService.Rotate(mptPoint2);
                     //mobjEditService.EndRotate(mptPoint2);

                     if (mobjEditPointService.GeometryCount > 0) mobjEditPointService.RemoveAllGeometries();
                     if (mobjEditServiceTemp.GeometryCount > 0) mobjEditServiceTemp.RemoveAllGeometries();
                     if (mobjEditService.GeometryCount > 0) mobjEditService.RemoveAllGeometries();

                     if (mobjEditPointServiceNew.GeometryCount > 0) mobjEditPointServiceNew.RemoveAllGeometries();
                     if (mobjEditServiceNewTemp.GeometryCount > 0) mobjEditServiceNewTemp.RemoveAllGeometries();
                     if (mobjEditServiceNew.GeometryCount > 0) mobjEditServiceNew.RemoveAllGeometries();

                     if (cManhole.ManholeWall != null)
                     {
                         foreach (ManholePoint _pnt in cManhole.ManholeWall)
                         {
                             IGTTextPointGeometry txtNumber = PGeoLib.CreateTextGeom(mptPoint1.X + _pnt.X, mptPoint1.Y + _pnt.Y, "+");
                             mobjEditService.AddGeometry(txtNumber, cManhole.ManholeWallStyle);


                         }

                     }
                     //}

                     CreateManholeGeom(mptPoint1, mptPoint2, mobjEditService);


                     mintState = 6;
                     if ((cManhole.ManholeWall == null) || (cManhole.ManholeWall.Count <= 0)) mintState = 7;
                 }
                 else if (mintState == 6)
                 {
                     mintState = 7;
                 }

                 #endregion

                 #region label temporary accept
                 if (step == 61)
                 {
                     step = 6;
                     m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Point to accept new position of label! Right Click to exit");
                     return;
                 }
                 if (step == 601)
                 {
                     step = 60;
                     m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Point to accept new position of label! Right Click to exit");
                     return;
                 }
                 #endregion

                 #region cancel rotating nest use default
                  
                     if (step == 81)
                     {
                        // mobjEditServiceRotate2.EndRotate(mobjEditServiceRotate2.GetGeometry(1).FirstPoint);
                       //  mobjEditService2.RemoveAllGeometries();
                         mobjEditServiceRotate3.EndRotate(mobjEditServiceRotate3.GetGeometry(1).FirstPoint);
                         mobjEditService3.RemoveAllGeometries();

                         int i = CountGeomDuctNestTo;
                         int y = 1;
 
                         for (int j = 0; j < DuctPathFrom.DuctNestTo[i].Form.Count; j++)
                         {
                             mobjEditServiceRotate2.AddGeometry(mobjEditServiceRotate3.GetGeometry(y).Copy(), DuctPathFrom.DuctNestTo[i].styleIDform);
                             y++;
                         }
                         for (int j = 0; j < DuctPathFrom.DuctNestTo[i].Labels.Count; j++)
                         {
                             mobjEditServiceRotate2.AddGeometry(mobjEditServiceRotate3.GetGeometry(y).Copy(), DuctPathFrom.DuctNestTo[i].styleIDlabel);
                             y++;
                         }

                         for (int j = 0; j < DuctPathFrom.DuctNestTo[i].Ducts.Count; j++)
                         {
                             for (int k = 0; k < DuctPathFrom.DuctNestTo[i].Ducts[j].Form.Count; k++)
                             {
                                 mobjEditServiceRotate2.AddGeometry(mobjEditServiceRotate3.GetGeometry(y).Copy(), DuctPathFrom.DuctNestTo[i].Ducts[j].styleID);
                                 y++;
                             }
                         }

                         DuctPathFrom.DuctNestTo[CountGeomDuctNestTo] = UpdateGeomDuctNest(DuctPathFrom.DuctNestTo[CountGeomDuctNestTo], true);
                         mobjEditServiceRotate3.RemoveAllGeometries();

                         if (CountGeomDuctNestTo < DuctPathFrom.DuctNestTo.Count - 1)
                         {
                             CountGeomDuctNestTo++;
                             step = 7;
                         }
                         else
                         {
                             // step = 0;
                             // CountGeomDuctNestFrom = 0;
                             LocateFeature(3, m_gtapp.ActiveMapWindow, DuctPathOrigin.sourceFNO, DuctPathOrigin.sourceFID, iManholeFNO, iManholeFID);
                        
                        
                             step = 50;

                             return;
                         }
                     }
                     if (step == 801)
                     {
                       //  mobjEditServiceRotate1.EndRotate(mobjEditServiceRotate1.GetGeometry(1).FirstPoint);
                       //  mobjEditService1.RemoveAllGeometries();
                         mobjEditServiceRotate3.EndRotate(mobjEditServiceRotate3.GetGeometry(1).FirstPoint);
                         mobjEditService3.RemoveAllGeometries();

                         int i = CountGeomDuctNestFrom;
                         int y = 1;
                         for (int j = 0; j < DuctPathTo.DuctNestFrom[i].Form.Count; j++)
                         {
                             mobjEditServiceRotate1.AddGeometry(mobjEditServiceRotate3.GetGeometry(y).Copy(), DuctPathTo.DuctNestFrom[i].styleIDform);
                             y++;
                         }
                         for (int j = 0; j < DuctPathTo.DuctNestFrom[i].Labels.Count; j++)
                         {
                             mobjEditServiceRotate1.AddGeometry(mobjEditServiceRotate3.GetGeometry(y).Copy(), DuctPathTo.DuctNestFrom[i].styleIDlabel);
                             y++;
                         }

                         for (int j = 0; j < DuctPathTo.DuctNestFrom[i].Ducts.Count; j++)
                         {
                             for (int k = 0; k < DuctPathTo.DuctNestFrom[i].Ducts[j].Form.Count; k++)
                             {
                                 mobjEditServiceRotate1.AddGeometry(mobjEditServiceRotate3.GetGeometry(y).Copy(), DuctPathTo.DuctNestFrom[i].Ducts[j].styleID);
                                 y++;
                             }
                         }

                         DuctPathTo.DuctNestFrom[CountGeomDuctNestFrom] = UpdateGeomDuctNest(DuctPathTo.DuctNestFrom[CountGeomDuctNestFrom], false);
                         mobjEditServiceRotate3.RemoveAllGeometries();

                         if (CountGeomDuctNestFrom < DuctPathTo.DuctNestFrom.Count - 1)
                         {
                             CountGeomDuctNestFrom++;
                             step = 70;
                         }
                         else
                         {
                             step = 0;
                             //CountGeomDuctNestTo = 0;
                             BreakDuctPath();
                            
                             return;
                         }
                     }
                     
                 #endregion
             }
        }
        #endregion

        #region Event Handlers
        void m_oIGTCustomCommandHelper_Click(object sender, GTMouseEventArgs e)
        {
        //
        }

       
        void m_oIGTCustomCommandHelper_WheelRotate(object sender, GTWheelRotateEventArgs e)
        {
            //GTClassFactory.Create<IGTApplication>().SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "WheelRotate.");
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
          try{ 
              m_gtapp = GTClassFactory.Create<IGTApplication>();
                m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Running Insert Manhole ver 2.5. . . ");
                m_oIGTCustomCommandHelper = CustomCommandHelper;
                m_IGTDataContext = m_gtapp.DataContext;
                mobjLocateService = m_gtapp.ActiveMapWindow.LocateService;
                mobjEditService = GTClassFactory.Create<IGTGeometryEditService>();
                mobjEditService.TargetMapWindow = m_gtapp.ActiveMapWindow;
                mobjEditServiceRotate = GTClassFactory.Create<IGTGeometryEditService>();
                mobjEditServiceRotate.TargetMapWindow = m_gtapp.ActiveMapWindow;
              
                mobjEditService1 = GTClassFactory.Create<IGTGeometryEditService>();
                mobjEditService1.TargetMapWindow = m_gtapp.ActiveMapWindow;
                mobjEditServiceRotate1 = GTClassFactory.Create<IGTGeometryEditService>();
                mobjEditServiceRotate1.TargetMapWindow = m_gtapp.ActiveMapWindow;
              
                mobjEditService2 = GTClassFactory.Create<IGTGeometryEditService>();
                mobjEditService2.TargetMapWindow = m_gtapp.ActiveMapWindow;
                mobjEditServiceRotate2 = GTClassFactory.Create<IGTGeometryEditService>();
                mobjEditServiceRotate2.TargetMapWindow = m_gtapp.ActiveMapWindow;

                mobjEditService3 = GTClassFactory.Create<IGTGeometryEditService>();
                mobjEditService3.TargetMapWindow = m_gtapp.ActiveMapWindow;
                mobjEditServiceRotate3 = GTClassFactory.Create<IGTGeometryEditService>();
                mobjEditServiceRotate3.TargetMapWindow = m_gtapp.ActiveMapWindow;

                mobjEditServiceSlash = GTClassFactory.Create<IGTGeometryEditService>();
                mobjEditServiceSlash.TargetMapWindow = m_gtapp.ActiveMapWindow;

                mobjEditServiceDuctPathFrom = GTClassFactory.Create<IGTGeometryEditService>();
                mobjEditServiceDuctPathFrom.TargetMapWindow = m_gtapp.ActiveMapWindow;

                mobjEditServiceDuctPathTo = GTClassFactory.Create<IGTGeometryEditService>();
                mobjEditServiceDuctPathTo.TargetMapWindow = m_gtapp.ActiveMapWindow;

                mobjRelationshipService = GTClassFactory.Create<IGTRelationshipService>();
                mobjRelationshipService.DataContext = m_IGTDataContext;

                foreach (IGTDDCKeyObject oDDCKeyObject in GTClassFactory.Create<IGTApplication>().SelectedObjects.GetObjects())
                {
                    m_gtapp.SelectedObjects.Add(GTSelectModeConstants.gtsosmAllDisplayedComponentsInActiveLegend, oDDCKeyObject);
                }

                SubscribeEvents();
                              
                step = 1;
                m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Select Duct Path feature to break! Right Click to exit.");
             }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Manhole Insert", MessageBoxButtons.OK, MessageBoxIcon.Error);
                ExitCmd();                
            }
          
        }
                
     

        public bool CanTerminate
        {
            get
            {
                DialogResult retVal = MessageBox.Show("Do you want to discard your current changes and exit?", "Manhole Insert", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (retVal == DialogResult.Yes)
                {
                    ExitCmd();
                }
                else
                {
                    return false;
                }

                return false;
            }
        }

        public void Pause()
        {
            step += 50000;
        }

        public void Resume()
        {
            step -= 50000;
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

        #region function for feature explorer

        //******************************************************************************
        //********************** GTFeatureExplorerService Events. **********************
        //******************************************************************************
  
        private void SaveAttribute()
        {
            //validate Manhole
            short iCNO=2701;
             mobjManholeAttribute.Components.GetComponent(iCNO).Recordset.MoveFirst();
             for (int j = 0; j < mobjManholeAttribute.Components.GetComponent(iCNO).Recordset.RecordCount; j++)
             {
                 for (int f = 0; f < mobjManholeAttribute.Components.GetComponent(iCNO).Recordset.Fields.Count; f++)
                 {
                     if (mobjManholeAttribute.Components.GetComponent(iCNO).Recordset.Fields[f].Name == "MH_PLACEMENT")
                         if (mobjManholeAttribute.Components.GetComponent(iCNO).Recordset.Fields[f].Value.ToString() == "")
                         {
                             MessageBox.Show("Placement field is required!", "Manhole Insert", MessageBoxButtons.OK, MessageBoxIcon.Information);

                             return;
                         }
                     if (mobjManholeAttribute.Components.GetComponent(iCNO).Recordset.Fields[f].Name == "MH_ATTCHMNT")
                         if (mobjManholeAttribute.Components.GetComponent(iCNO).Recordset.Fields[f].Value.ToString() == "")
                         {
                             MessageBox.Show("Loading Coil Pit field is required!", "Manhole Insert", MessageBoxButtons.OK, MessageBoxIcon.Information);

                             return;
                         }

                     if (mobjManholeAttribute.Components.GetComponent(iCNO).Recordset.Fields[f].Name == "MH_CONSTRUCTION")
                         if (mobjManholeAttribute.Components.GetComponent(iCNO).Recordset.Fields[f].Value.ToString() == "")
                         {
                             MessageBox.Show("Construction field is required!", "Manhole Insert", MessageBoxButtons.OK, MessageBoxIcon.Information);

                             return;
                         }

                     if (mobjManholeAttribute.Components.GetComponent(iCNO).Recordset.Fields[f].Name == "MH_MIX")
                         if (mobjManholeAttribute.Components.GetComponent(iCNO).Recordset.Fields[f].Value.ToString() == "")
                         {
                             MessageBox.Show("Mix field is required!", "Manhole Insert", MessageBoxButtons.OK, MessageBoxIcon.Information);

                             return;
                         }

                     if (mobjManholeAttribute.Components.GetComponent(iCNO).Recordset.Fields[f].Name == "FEATURE_TYPE")
                         if (mobjManholeAttribute.Components.GetComponent(iCNO).Recordset.Fields[f].Value.ToString() == "")
                         {
                             MessageBox.Show("Manhole Type field is required!", "Manhole Insert", MessageBoxButtons.OK, MessageBoxIcon.Information);

                             return;
                         }
                        
                 }
             }

             iCNO = 51;
             mobjManholeAttribute.Components.GetComponent(iCNO).Recordset.MoveFirst();
             for (int j = 0; j < mobjManholeAttribute.Components.GetComponent(iCNO).Recordset.RecordCount; j++)
             {
                 for (int f = 0; f < mobjManholeAttribute.Components.GetComponent(iCNO).Recordset.Fields.Count; f++)
                 {
                     if (mobjManholeAttribute.Components.GetComponent(iCNO).Recordset.Fields[f].Name == "MIN_MATERIAL")
                         if (mobjManholeAttribute.Components.GetComponent(iCNO).Recordset.Fields[f].Value.ToString() == "")
                         {
                             MessageBox.Show("Plant Unit field is required!", "Manhole Insert", MessageBoxButtons.OK, MessageBoxIcon.Information);

                             return;
                         }
                     if (mobjManholeAttribute.Components.GetComponent(iCNO).Recordset.Fields[f].Name == "BILLING_RATE")
                         if (mobjManholeAttribute.Components.GetComponent(iCNO).Recordset.Fields[f].Value.ToString() == "")
                         {
                             MessageBox.Show("Billing Rate field is required!", "Manhole Insert", MessageBoxButtons.OK, MessageBoxIcon.Information);

                             return;
                         }
                    
                 }
             }
             ////
                            {
                m_oIGTTransactionManager.Rollback();
                if (!(mobjExplorerService == null))
                {
                    mobjExplorerService.Visible = false;
                    mobjExplorerService.Clear();
                }

                mobjExplorerService.CancelClick -= new EventHandler(mobjExplorerService_CancelClick);
                mobjExplorerService.SaveAndContinueClick -= new EventHandler(mobjExplorerService_SaveAndContinueClick);
                mobjExplorerService.SaveClick -= new EventHandler(mobjExplorerService_SaveClick);
                mobjExplorerService = null;
                LoadManholeType(mobjManholeAttribute);
                mintState = 4;
                            }

        }
      
        private void mobjExplorerService_CancelClick(object sender, EventArgs e)
        {
            // Throws any errors raised to the error handler.
            try
            {
                //Service without exitcmd
                if (!(mobjExplorerService == null))
                {
                    mobjExplorerService.Visible = false;
                    mobjExplorerService.Clear();
                }
                mobjExplorerService.CancelClick -= new EventHandler(mobjExplorerService_CancelClick);
                mobjExplorerService.SaveAndContinueClick -= new EventHandler(mobjExplorerService_SaveAndContinueClick);
                mobjExplorerService.SaveClick -= new EventHandler(mobjExplorerService_SaveClick);
                mobjExplorerService = null;
                MessageBox.Show("Insert of Manhole is canceled!", "Manhole Insert", MessageBoxButtons.OK, MessageBoxIcon.Information);
                
                ExitCmd();

            }
            catch (Exception ex)
            {}
        }

        private void mobjExplorerService_SaveAndContinueClick(object sender, EventArgs e)
        {
            // Throws any errors raised to the error handler.
            try
            {
                SaveAttribute();
}
            catch (Exception ex)
            {
             }

        }

        private void mobjExplorerService_SaveClick(object sender, EventArgs e)
        {
            // Throws any errors raised to the error handler.
            try
            {

                // Transitions the state of the command to the next step in the placement work flow.
                SaveAttribute();

                // Catches any errors thrown to the error handler and displays a message box with the description.
            }
            catch (Exception ex)
            {
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

        #region unsubscribe events
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
        #endregion

        #region GetDuctPathOrigin
        public bool GetDuctPathOrigin(short FNO, int FID)
        {
            try
            {
                if (DuctPathOrigin == null)
                    DuctPathOrigin = new DuctPath();
                IGTKeyObject oDuctPathFeature = m_IGTDataContext.OpenFeature(FNO, FID);
                #region Netelem
                if (!oDuctPathFeature.Components.GetComponent(51).Recordset.EOF)
                {
                    oDuctPathFeature.Components.GetComponent(51).Recordset.MoveLast();

                    for (int i = 0; i < oDuctPathFeature.Components.GetComponent(51).Recordset.Fields.Count; i++)
                    {
                        if (oDuctPathFeature.Components.GetComponent(51).Recordset.Fields[i].Name == "EXC_ABB")
                        {
                            DuctPathOrigin.EXC_ABB = oDuctPathFeature.Components.GetComponent(51).Recordset.Fields[i].Value.ToString();
                        }
                        else if (oDuctPathFeature.Components.GetComponent(51).Recordset.Fields[i].Name == "BILLING_RATE")
                        {
                            DuctPathOrigin.BillingRate = oDuctPathFeature.Components.GetComponent(51).Recordset.Fields[i].Value.ToString();
                        }
                        else if (oDuctPathFeature.Components.GetComponent(51).Recordset.Fields[i].Name == "YEAR_PLACED")
                        {
                            DuctPathOrigin.InstallYear = oDuctPathFeature.Components.GetComponent(51).Recordset.Fields[i].Value.ToString();
                        }
                        else if (oDuctPathFeature.Components.GetComponent(51).Recordset.Fields[i].Name == "FEATURE_STATE")
                        {
                            DuctPathOrigin.Feature_state = oDuctPathFeature.Components.GetComponent(51).Recordset.Fields[i].Value.ToString();
                        }


                    }
                }
                if (DuctPathOrigin.Feature_state != "ASB" && DuctPathOrigin.Feature_state != "PPF")
                {
                    DuctPathOrigin = null;
                    return true;
                }
                #endregion
                #region Attr
                if (!oDuctPathFeature.Components.GetComponent(2201).Recordset.EOF)
                {
                    oDuctPathFeature.Components.GetComponent(2201).Recordset.MoveLast();
                    DuctPathOrigin.FNO = 2200;

                    for (int i = 0; i < oDuctPathFeature.Components.GetComponent(2201).Recordset.Fields.Count; i++)
                    {
                        if (oDuctPathFeature.Components.GetComponent(2201).Recordset.Fields[i].Name == "G3E_FID")
                        {
                            DuctPathOrigin.FID = int.Parse(oDuctPathFeature.Components.GetComponent(2201).Recordset.Fields[i].Value.ToString());
                        }
                        else if (oDuctPathFeature.Components.GetComponent(2201).Recordset.Fields[i].Name == "DT_ND_FRM_ID")
                        {
                            DuctPathOrigin.sourceFID = int.Parse(oDuctPathFeature.Components.GetComponent(2201).Recordset.Fields[i].Value.ToString());
                        }
                        else if (oDuctPathFeature.Components.GetComponent(2201).Recordset.Fields[i].Name == "DT_ND_FRM_TY")
                        {
                            DuctPathOrigin.sourceType = oDuctPathFeature.Components.GetComponent(2201).Recordset.Fields[i].Value.ToString().Trim();
                            DuctPathOrigin.sourceFNO = GetFNObyFeatureType(DuctPathOrigin.sourceType);
                        }
                        else if (oDuctPathFeature.Components.GetComponent(2201).Recordset.Fields[i].Name == "DT_MH_FRM_WALL")
                        {
                            DuctPathOrigin.sourceWall = int.Parse(oDuctPathFeature.Components.GetComponent(2201).Recordset.Fields[i].Value.ToString());
                        }
                        else if (oDuctPathFeature.Components.GetComponent(2201).Recordset.Fields[i].Name == "DT_ND_TO_ID")
                        {
                            DuctPathOrigin.termFID = int.Parse(oDuctPathFeature.Components.GetComponent(2201).Recordset.Fields[i].Value.ToString());
                        }
                        else if (oDuctPathFeature.Components.GetComponent(2201).Recordset.Fields[i].Name == "DT_ND_TO_TY")
                        {
                            DuctPathOrigin.termType = oDuctPathFeature.Components.GetComponent(2201).Recordset.Fields[i].Value.ToString().Trim();
                            DuctPathOrigin.termFNO = GetFNObyFeatureType(DuctPathOrigin.termType);
                        }
                        else if (oDuctPathFeature.Components.GetComponent(2201).Recordset.Fields[i].Name == "DT_MH_TO_WALL")
                        {
                            DuctPathOrigin.termWall = int.Parse(oDuctPathFeature.Components.GetComponent(2201).Recordset.Fields[i].Value.ToString());
                        }
                        else if (oDuctPathFeature.Components.GetComponent(2201).Recordset.Fields[i].Name == "TOTAL_LENGTH")
                        {
                            DuctPathOrigin.Length = int.Parse(oDuctPathFeature.Components.GetComponent(2201).Recordset.Fields[i].Value.ToString());
                        }
                        else if (oDuctPathFeature.Components.GetComponent(2201).Recordset.Fields[i].Name == "DT_WAYS")
                        {
                            DuctPathOrigin.DuctWay = int.Parse(oDuctPathFeature.Components.GetComponent(2201).Recordset.Fields[i].Value.ToString());
                        }
                         else if (oDuctPathFeature.Components.GetComponent(2201).Recordset.Fields[i].Name == "DT_ASB_SUBDUCT")
                        {
                            DuctPathOrigin.ASb_SubDuct = int.Parse(oDuctPathFeature.Components.GetComponent(2201).Recordset.Fields[i].Value.ToString());
                        }
                         else if (oDuctPathFeature.Components.GetComponent(2201).Recordset.Fields[i].Name == "DT_NEST_WAYS")
                        {
                            DuctPathOrigin.NestAssignDuctWay = int.Parse(oDuctPathFeature.Components.GetComponent(2201).Recordset.Fields[i].Value.ToString());
                        }
                         else if (oDuctPathFeature.Components.GetComponent(2201).Recordset.Fields[i].Name == "DT_PP_INNDUCT")
                        {
                            DuctPathOrigin.Prop_InnDuct = int.Parse(oDuctPathFeature.Components.GetComponent(2201).Recordset.Fields[i].Value.ToString());
                        }
                         else if (oDuctPathFeature.Components.GetComponent(2201).Recordset.Fields[i].Name == "DT_PP_SUBDUCT")
                        {
                            DuctPathOrigin.Prop_SubDuct = int.Parse(oDuctPathFeature.Components.GetComponent(2201).Recordset.Fields[i].Value.ToString());
                        }
                         else if (oDuctPathFeature.Components.GetComponent(2201).Recordset.Fields[i].Name == "DUCT_BALANCE")
                        {
                            DuctPathOrigin.DuctBalance = 0;// int.Parse(oDuctPathFeature.Components.GetComponent(2201).Recordset.Fields[i].Value.ToString());
                        }
                        else if (oDuctPathFeature.Components.GetComponent(2201).Recordset.Fields[i].Name == "DT_CONSTRUCTION")
                        {
                            DuctPathOrigin.ConstructBy = oDuctPathFeature.Components.GetComponent(2201).Recordset.Fields[i].Value.ToString();
                        }
                        else if (oDuctPathFeature.Components.GetComponent(2201).Recordset.Fields[i].Name == "EXPAND_FLAG")
                        {
                            DuctPathOrigin.ExpandFlag = oDuctPathFeature.Components.GetComponent(2201).Recordset.Fields[i].Value.ToString();
                        }
                        else if (oDuctPathFeature.Components.GetComponent(2201).Recordset.Fields[i].Name == "CENTRALDB_FLAG")
                        {
                            DuctPathOrigin.DBFlag = oDuctPathFeature.Components.GetComponent(2201).Recordset.Fields[i].Value.ToString();
                        }
                        else if (oDuctPathFeature.Components.GetComponent(2201).Recordset.Fields[i].Name == "DESCRIPTION")
                        {
                            DuctPathOrigin.Description = oDuctPathFeature.Components.GetComponent(2201).Recordset.Fields[i].Value.ToString();
                        }
                        else if (oDuctPathFeature.Components.GetComponent(2201).Recordset.Fields[i].Name == "FEATURE_TYPE")
                        {
                            DuctPathOrigin.Feature_type = oDuctPathFeature.Components.GetComponent(2201).Recordset.Fields[i].Value.ToString();
                        }


                                                                                                                                                                                                                    
  
                    }
                    DuctPathOrigin.DuctBalance = DuctPathOrigin.DuctWay - DuctPathOrigin.NestAssignDuctWay;
                }
                #endregion
                #region Geom
                if (!oDuctPathFeature.Components.GetComponent(2210).Recordset.EOF)
                {

                    IGTCompositePolylineGeometry tempcomp = (IGTCompositePolylineGeometry)oDuctPathFeature.Components.GetComponent(2210).Geometry;
                    DuctPathOrigin.DuctPathLineGeom = (IGTPolylineGeometry)(tempcomp.ExtractGeometry(tempcomp.FirstPoint, tempcomp.LastPoint, false));

                    //IGTGeometry temp = (IGTGeometry)oDuctPathFeature.Components.GetComponent(2210).Geometry;                  
                    //DuctPathOrigin.DuctPathLineGeom = GTClassFactory.Create<IGTPolylineGeometry>();
                    //for (int i = 0; i < temp.KeypointCount; i++)
                    //{
                    //    DuctPathOrigin.DuctPathLineGeom.Points.Add(temp.GetKeypointPosition(i));
                    //}
                }
                #endregion
                List<string> templabelcont = new List<string>();
                #region sections attr
                if (!oDuctPathFeature.Components.GetComponent(2202).Recordset.EOF)
                {
                    DuctPathOrigin.Sections = new List<DuctPathSect>();
                    
                    oDuctPathFeature.Components.GetComponent(2202).Recordset.MoveFirst();
                    for (int j = 0; j < oDuctPathFeature.Components.GetComponent(2202).Recordset.RecordCount; j++)
                    {

                        DuctPathSect secttemp = new DuctPathSect();
                        for (int k = 0; k < oDuctPathFeature.Components.GetComponent(2202).Recordset.Fields.Count; k++)
                        {
                            if (oDuctPathFeature.Components.GetComponent(2202).Recordset.Fields[k].Name == "G3E_CID")
                            {
                                secttemp.CID = short.Parse(oDuctPathFeature.Components.GetComponent(2202).Recordset.Fields[k].Value.ToString());
                            }
                            if (oDuctPathFeature.Components.GetComponent(2202).Recordset.Fields[k].Name == "DT_S_WAYS")
                            {
                                secttemp.NumDuctWaysSect = oDuctPathFeature.Components.GetComponent(2202).Recordset.Fields[k].Value.ToString();
                            }
                            if (oDuctPathFeature.Components.GetComponent(2202).Recordset.Fields[k].Name == "DT_S_LENGTH")
                            {
                                secttemp.SectionLength = oDuctPathFeature.Components.GetComponent(2202).Recordset.Fields[k].Value.ToString();
                            }
                            if (oDuctPathFeature.Components.GetComponent(2202).Recordset.Fields[k].Name == "DT_S_DIAMETER")
                            {
                                secttemp.SectDiam = oDuctPathFeature.Components.GetComponent(2202).Recordset.Fields[k].Value.ToString();
                            }
                            if (oDuctPathFeature.Components.GetComponent(2202).Recordset.Fields[k].Name == "COMMON_TRENCH")
                            {
                                secttemp.SectOwner = oDuctPathFeature.Components.GetComponent(2202).Recordset.Fields[k].Value.ToString();
                            }
                            if (oDuctPathFeature.Components.GetComponent(2202).Recordset.Fields[k].Name == "DT_S_PLACMNT")
                            {
                                secttemp.SectPlc = oDuctPathFeature.Components.GetComponent(2202).Recordset.Fields[k].Value.ToString();
                            }
                            if (oDuctPathFeature.Components.GetComponent(2202).Recordset.Fields[k].Name == "DT_S_TYPE")
                            {
                                secttemp.SectType = oDuctPathFeature.Components.GetComponent(2202).Recordset.Fields[k].Value.ToString();
                            }
                            if (oDuctPathFeature.Components.GetComponent(2202).Recordset.Fields[k].Name == "BILLING_RATE")
                            {
                                secttemp.SectBillingRate = oDuctPathFeature.Components.GetComponent(2202).Recordset.Fields[k].Value.ToString();
                            }
                            if (oDuctPathFeature.Components.GetComponent(2202).Recordset.Fields[k].Name == "DT_S_ENCASE")
                            {
                                secttemp.Encasement = oDuctPathFeature.Components.GetComponent(2202).Recordset.Fields[k].Value.ToString();
                            }
                            if (oDuctPathFeature.Components.GetComponent(2202).Recordset.Fields[k].Name == "DT_S_BACKFILL")
                            {
                                secttemp.SectBackFill = oDuctPathFeature.Components.GetComponent(2202).Recordset.Fields[k].Value.ToString();
                            }
                            if (oDuctPathFeature.Components.GetComponent(2202).Recordset.Fields[k].Name == "MIN_MATERIAL")
                            {
                                secttemp.PUSect = oDuctPathFeature.Components.GetComponent(2202).Recordset.Fields[k].Value.ToString();
                            }
                            if (oDuctPathFeature.Components.GetComponent(2202).Recordset.Fields[k].Name == "YEAR_EXTENDED")
                            {

                                string tt = oDuctPathFeature.Components.GetComponent(2202).Recordset.Fields[k].Value.ToString();
                                if (tt == "")
                                    secttemp.YearExtended = "0";
                                else
                                    secttemp.YearExtended = tt.Substring(tt.Length - 4);

                            }
                            if (oDuctPathFeature.Components.GetComponent(2202).Recordset.Fields[k].Name == "YEAR_EXPANDED")
                            {

                                string tt = oDuctPathFeature.Components.GetComponent(2202).Recordset.Fields[k].Value.ToString();
                                if (tt == "")
                                    secttemp.YearExpanded = "0";
                                else
                                    secttemp.YearExpanded = tt.Substring(tt.Length - 4);

                            }
                            if (oDuctPathFeature.Components.GetComponent(2202).Recordset.Fields[k].Name == "G3E_TEXT")
                            {
                                templabelcont.Add(oDuctPathFeature.Components.GetComponent(2202).Recordset.Fields[k].Value.ToString());
                            }
                        }

                        // secttemp.PUSect = "0";
                        DuctPathOrigin.Sections.Add(secttemp);
                        oDuctPathFeature.Components.GetComponent(2202).Recordset.MoveNext();
                    }

                }
                #endregion
                #region sections labels
                if (!oDuctPathFeature.Components.GetComponent(2230).Recordset.EOF)
                {
                    DuctPathOrigin.SectLabels = new List<SectLabelLeaderLine>();
                    oDuctPathFeature.Components.GetComponent(2230).Recordset.MoveFirst();
                    for (int j = 0; j < oDuctPathFeature.Components.GetComponent(2230).Recordset.RecordCount; j++)
                    {
                        SectLabelLeaderLine labelsect = new SectLabelLeaderLine();
                        IGTOrientedPointGeometry tempcomp = (IGTOrientedPointGeometry)oDuctPathFeature.Components.GetComponent(2230).Geometry;
                        TextPointGeom ttemp = new TextPointGeom();
                        ttemp.CNO = 2230;

                        IGTTextPointGeometry templabel = GTClassFactory.Create<IGTTextPointGeometry>();
                        templabel.Origin = tempcomp.Origin;
                        templabel.Rotation = AngleBtwPoint(tempcomp.Orientation.I, tempcomp.Orientation.J);
                        for (int k = 0; k < oDuctPathFeature.Components.GetComponent(2230).Recordset.Fields.Count; k++)
                        {
                           
                            if (oDuctPathFeature.Components.GetComponent(2230).Recordset.Fields[k].Name == "G3E_ALIGNMENT")
                            {//(GTAlignmentConstants)
                                labelsect.LabelAlight = int.Parse(oDuctPathFeature.Components.GetComponent(2230).Recordset.Fields[k].Value.ToString());
                            }
                            if (oDuctPathFeature.Components.GetComponent(2230).Recordset.Fields[k].Name == "G3E_CID")
                            {
                                labelsect.CID = short.Parse(oDuctPathFeature.Components.GetComponent(2230).Recordset.Fields[k].Value.ToString());
                            }
                        }
                        labelsect.LabelText = templabelcont[j];
                        labelsect.Label = templabel;
                        DuctPathOrigin.SectLabels.Add(labelsect);
                        oDuctPathFeature.Components.GetComponent(2230).Recordset.MoveNext();
                    }
                    if (!oDuctPathFeature.Components.GetComponent(2212).Recordset.EOF)
                    {
                        oDuctPathFeature.Components.GetComponent(2212).Recordset.MoveFirst();
                        for (int j = 0; j < oDuctPathFeature.Components.GetComponent(2212).Recordset.RecordCount; j++)
                        {
                            int cid = 0;
                            for (int k = 0; k < oDuctPathFeature.Components.GetComponent(2212).Recordset.Fields.Count; k++)
                            {
                                if (oDuctPathFeature.Components.GetComponent(2212).Recordset.Fields[k].Name == "G3E_CID")
                                {
                                    cid = short.Parse(oDuctPathFeature.Components.GetComponent(2212).Recordset.Fields[k].Value.ToString());
                                }
                            }
                            for (int k = 0; k < DuctPathOrigin.SectLabels.Count; k++)
                            {
                                if (DuctPathOrigin.SectLabels[k].CID == cid)
                                {
                                    if (oDuctPathFeature.Components.GetComponent(2212).Geometry != null)
                                    {
                                        IGTCompositePolylineGeometry tempcomp = (IGTCompositePolylineGeometry)oDuctPathFeature.Components.GetComponent(2212).Geometry;
                                        DuctPathOrigin.SectLabels[k].LeaderLine = (IGTPolylineGeometry)(tempcomp.ExtractGeometry(tempcomp.FirstPoint, tempcomp.LastPoint, false));

                                    }
                                    break;
                                }
                            }
                            oDuctPathFeature.Components.GetComponent(2212).Recordset.MoveNext();
                        }
                    }
                }
                #endregion
                #region section slashes
                if (!oDuctPathFeature.Components.GetComponent(2220).Recordset.EOF)
                {
                    DuctPathOrigin.SectSlashes = new List<SectSlash>();
                    oDuctPathFeature.Components.GetComponent(2220).Recordset.MoveFirst();
                    for (int j = 0; j < oDuctPathFeature.Components.GetComponent(2220).Recordset.RecordCount; j++)
                    {
                        SectSlash tempslash = new SectSlash();
                        for (int k = 0; k < oDuctPathFeature.Components.GetComponent(2220).Recordset.Fields.Count; k++)
                        {
                            if (oDuctPathFeature.Components.GetComponent(2220).Recordset.Fields[k].Name == "G3E_CID")
                            {
                                tempslash.CID = short.Parse(oDuctPathFeature.Components.GetComponent(2220).Recordset.Fields[k].Value.ToString());
                            }
                        }
                        tempslash.Slash = (IGTOrientedPointGeometry)(oDuctPathFeature.Components.GetComponent(2220).Geometry);
                        DuctPathOrigin.SectSlashes.Add(tempslash);
                        oDuctPathFeature.Components.GetComponent(2220).Recordset.MoveNext();
                    }

                    for (int k = 0; k < DuctPathOrigin.SectSlashes.Count; k++)
                    {
                        DuctPathOrigin.SectSlashes[k].length = 0;
                        for (int j = 0; j < DuctPathOrigin.Sections.Count; j++)
                        {
                            if (DuctPathOrigin.SectSlashes[k].CID >= DuctPathOrigin.Sections[j].CID)
                            {
                                DuctPathOrigin.SectSlashes[k].length += int.Parse(DuctPathOrigin.Sections[j].SectionLength);
                            }
                        }
                    }

                }

                //add to duct path slash points
                if (DuctPathOrigin.SectSlashes != null)
                    if (DuctPathOrigin.SectSlashes.Count > 0)
                    {
                        double len = 0;
                        int s = 0;
                        IGTPolylineGeometry tempgeom=GTClassFactory.Create<IGTPolylineGeometry>();
                        for (int i = 0; i < DuctPathOrigin.DuctPathLineGeom.Points.Count-1; i++)
                        {
                            len += LegthBtwTwoPoints(DuctPathOrigin.DuctPathLineGeom.Points[i].X, DuctPathOrigin.DuctPathLineGeom.Points[i].Y,
                                                   DuctPathOrigin.DuctPathLineGeom.Points[i + 1].X, DuctPathOrigin.DuctPathLineGeom.Points[i + 1].Y);
                            if (s < DuctPathOrigin.SectSlashes.Count)
                            {
                                if (DuctPathOrigin.SectSlashes[s].length < len)
                                {
                                    tempgeom.Points.Add(DuctPathOrigin.SectSlashes[s].Slash.Origin);
                                    s++;
                                }
                            }

                            tempgeom.Points.Add(DuctPathOrigin.DuctPathLineGeom.Points[i]);

                        }

                        DuctPathOrigin.DuctPathLineGeom = null;
                        DuctPathOrigin.DuctPathLineGeom = GTClassFactory.Create<IGTPolylineGeometry>();
                        DuctPathOrigin.DuctPathLineGeom = tempgeom;
                    }
                #endregion
                #region DuctNest

                int CountDuctNest = int.Parse(Get_Value("select count(g3e_fid) from GC_CONTAIN where G3E_FNO=2400 and g3e_ownerfno=2200 and g3e_ownerfid =" + DuctPathOrigin.FID.ToString()));
                if (CountDuctNest > 0)
                {
                    string sSql = "select g3e_fid from GC_CONTAIN where G3E_FNO=2400 and g3e_ownerfno=2200 and g3e_ownerfid =" + DuctPathOrigin.FID.ToString();
                    ADODB.Recordset rsPP = new ADODB.Recordset();
                    rsPP = m_IGTDataContext.OpenRecordset(sSql, ADODB.CursorTypeEnum.adOpenStatic, ADODB.LockTypeEnum.adLockReadOnly, 1);
                    if (rsPP.RecordCount > 0)
                    {
                        rsPP.MoveFirst();
                        for (int i = 0; i < rsPP.RecordCount; i++)
                        {

                            DuctNest tempFrom = new DuctNest();
                            tempFrom.FID = int.Parse(rsPP.Fields[0].Value.ToString());
                            tempFrom.FNO = 2400;
                            if (tempFrom.Ducts == null)
                                tempFrom.Ducts = new List<Duct>();
                            tempFrom.Ducts = GetAllDuct(tempFrom.FID, true);
                            tempFrom.styleIDform = 2410002;
                            tempFrom.styleIDlabel = 2430002;

                            DuctNest tempTo = new DuctNest();
                            tempTo.FID = tempFrom.FID;
                            tempTo.FNO = 2400;
                            if (tempTo.Ducts == null)
                                tempTo.Ducts = new List<Duct>();
                            tempTo.Ducts = GetAllDuct(tempTo.FID, false);
                            tempTo.styleIDform = 2410002;
                            tempTo.styleIDlabel = 2430002;
                            IGTKeyObject oDuctNestFeature = m_IGTDataContext.OpenFeature(2400, tempFrom.FID);

                            short sCNO = 2410;
                            if (!oDuctNestFeature.Components.GetComponent(sCNO).Recordset.EOF)
                            {
                                oDuctNestFeature.Components.GetComponent(sCNO).Recordset.MoveFirst();
                                for (int j = 0; j < oDuctNestFeature.Components.GetComponent(sCNO).Recordset.RecordCount; j++)
                                {
                                    //string t = oDuctNestFeature.Components.GetComponent(sCNO).Geometry.Type;
                                    // t = "";
                                    //  IGTCompositePolylineGeometry tempcomp = (IGTCompositePolylineGeometry)oDuctNestFeature.Components.GetComponent(sCNO).Geometry;
                                    IGTPolylineGeometry tempform = (IGTPolylineGeometry)oDuctNestFeature.Components.GetComponent(sCNO).Geometry;
                                    PolylineGeom ttemp = new PolylineGeom();
                                    ttemp.geom = tempform;
                                    ttemp.CNO = sCNO;
                                    for (int k = 0; k < oDuctNestFeature.Components.GetComponent(sCNO).Recordset.Fields.Count; k++)
                                    {
                                        if (oDuctNestFeature.Components.GetComponent(sCNO).Recordset.Fields[k].Name == "G3E_CID")
                                        {
                                            ttemp.CID = int.Parse(oDuctNestFeature.Components.GetComponent(sCNO).Recordset.Fields[k].Value.ToString());
                                        }
                                    }
                                    if (tempFrom.Form == null)
                                        tempFrom.Form = new List<PolylineGeom>();
                                    tempFrom.Form.Add(ttemp);
                                    oDuctNestFeature.Components.GetComponent(sCNO).Recordset.MoveNext();
                                }
                            }
                            sCNO = 2430;
                            if (!oDuctNestFeature.Components.GetComponent(sCNO).Recordset.EOF)
                            {
                                oDuctNestFeature.Components.GetComponent(sCNO).Recordset.MoveFirst();
                                for (int j = 0; j < oDuctNestFeature.Components.GetComponent(sCNO).Recordset.RecordCount; j++)
                                {
                                    IGTOrientedPointGeometry tempcomp = (IGTOrientedPointGeometry)oDuctNestFeature.Components.GetComponent(sCNO).Geometry;
                                    TextPointGeom ttemp = new TextPointGeom();
                                    ttemp.CNO = sCNO;

                                    IGTTextPointGeometry templabel = GTClassFactory.Create<IGTTextPointGeometry>();
                                    templabel.Origin = tempcomp.Origin;
                                    templabel.Rotation = AngleBtwPoint(tempcomp.Orientation.I, tempcomp.Orientation.J);
                                    for (int k = 0; k < oDuctNestFeature.Components.GetComponent(sCNO).Recordset.Fields.Count; k++)
                                    {
                                        if (oDuctNestFeature.Components.GetComponent(sCNO).Recordset.Fields[k].Name == "G3E_TEXT")
                                        {
                                            templabel.Text = oDuctNestFeature.Components.GetComponent(sCNO).Recordset.Fields[k].Value.ToString();
                                        }
                                        if (oDuctNestFeature.Components.GetComponent(sCNO).Recordset.Fields[k].Name == "G3E_ALIGNMENT")
                                        {
                                            templabel.Alignment = (GTAlignmentConstants)(int.Parse(oDuctNestFeature.Components.GetComponent(sCNO).Recordset.Fields[k].Value.ToString()));
                                        }
                                        if (oDuctNestFeature.Components.GetComponent(sCNO).Recordset.Fields[k].Name == "G3E_CID")
                                        {
                                            ttemp.CID = int.Parse(oDuctNestFeature.Components.GetComponent(sCNO).Recordset.Fields[k].Value.ToString());
                                        }
                                    }
                                    ttemp.geom = templabel;
                                    //(IGTTextPointGeometry)(tempcomp.ExtractGeometry(tempcomp.FirstPoint, tempcomp.LastPoint, false)); 
                                    if (tempFrom.Labels == null)
                                        tempFrom.Labels = new List<TextPointGeom>();
                                    tempFrom.Labels.Add(ttemp);
                                    oDuctNestFeature.Components.GetComponent(sCNO).Recordset.MoveNext();
                                }
                            }
                            sCNO = 2412;
                            if (!oDuctNestFeature.Components.GetComponent(sCNO).Recordset.EOF)
                            {
                                oDuctNestFeature.Components.GetComponent(sCNO).Recordset.MoveFirst();
                                for (int j = 0; j < oDuctNestFeature.Components.GetComponent(sCNO).Recordset.RecordCount; j++)
                                {
                                    // IGTCompositePolylineGeometry tempcomp = (IGTCompositePolylineGeometry)oDuctNestFeature.Components.GetComponent(sCNO).Geometry;
                                    IGTPolylineGeometry tempform = (IGTPolylineGeometry)oDuctNestFeature.Components.GetComponent(sCNO).Geometry;
                                    PolylineGeom ttemp = new PolylineGeom();
                                    ttemp.geom = tempform;
                                    ttemp.CNO = sCNO;
                                    for (int k = 0; k < oDuctNestFeature.Components.GetComponent(sCNO).Recordset.Fields.Count; k++)
                                    {
                                        if (oDuctNestFeature.Components.GetComponent(sCNO).Recordset.Fields[k].Name == "G3E_CID")
                                        {
                                            ttemp.CID = int.Parse(oDuctNestFeature.Components.GetComponent(sCNO).Recordset.Fields[k].Value.ToString());
                                        }
                                    }
                                    if (tempTo.Form == null)
                                        tempTo.Form = new List<PolylineGeom>();
                                    tempTo.Form.Add(ttemp);
                                    oDuctNestFeature.Components.GetComponent(sCNO).Recordset.MoveNext();
                                }
                            }
                            sCNO = 2432;
                            if (!oDuctNestFeature.Components.GetComponent(sCNO).Recordset.EOF)
                            {
                                oDuctNestFeature.Components.GetComponent(sCNO).Recordset.MoveFirst();
                                for (int j = 0; j < oDuctNestFeature.Components.GetComponent(sCNO).Recordset.RecordCount; j++)
                                {
                                    IGTOrientedPointGeometry tempcomp = (IGTOrientedPointGeometry)oDuctNestFeature.Components.GetComponent(sCNO).Geometry;
                                    TextPointGeom ttemp = new TextPointGeom();
                                    ttemp.CNO = sCNO;

                                    IGTTextPointGeometry templabel = GTClassFactory.Create<IGTTextPointGeometry>();
                                    templabel.Origin = tempcomp.Origin;
                                    templabel.Rotation = AngleBtwPoint(tempcomp.Orientation.I, tempcomp.Orientation.J);
                                    for (int k = 0; k < oDuctNestFeature.Components.GetComponent(sCNO).Recordset.Fields.Count; k++)
                                    {
                                        if (oDuctNestFeature.Components.GetComponent(sCNO).Recordset.Fields[k].Name == "G3E_TEXT")
                                        {
                                            templabel.Text = oDuctNestFeature.Components.GetComponent(sCNO).Recordset.Fields[k].Value.ToString();
                                        }
                                        if (oDuctNestFeature.Components.GetComponent(sCNO).Recordset.Fields[k].Name == "G3E_ALIGNMENT")
                                        {
                                            templabel.Alignment = (GTAlignmentConstants)(int.Parse(oDuctNestFeature.Components.GetComponent(sCNO).Recordset.Fields[k].Value.ToString()));
                                        }
                                        if (oDuctNestFeature.Components.GetComponent(sCNO).Recordset.Fields[k].Name == "G3E_CID")
                                        {
                                            ttemp.CID = int.Parse(oDuctNestFeature.Components.GetComponent(sCNO).Recordset.Fields[k].Value.ToString());
                                        }
                                    }

                                    ttemp.geom = templabel;
                                    if (tempTo.Labels == null)
                                        tempTo.Labels = new List<TextPointGeom>();
                                    tempTo.Labels.Add(ttemp);
                                    oDuctNestFeature.Components.GetComponent(sCNO).Recordset.MoveNext();
                                }
                            }
                            if (DuctPathOrigin.DuctNestFrom == null)
                                DuctPathOrigin.DuctNestFrom = new List<DuctNest>();
                            if (DuctPathOrigin.DuctNestTo == null)
                                DuctPathOrigin.DuctNestTo = new List<DuctNest>();

                            DuctPathOrigin.DuctNestFrom.Add(tempFrom);
                            DuctPathOrigin.DuctNestTo.Add(tempTo);
                            rsPP.MoveNext();
                        }

                    }
                    rsPP = null;

                }

                #endregion
                LocateFeature(2, m_gtapp.ActiveMapWindow, DuctPathOrigin.sourceFNO, DuctPathOrigin.sourceFID, DuctPathOrigin.termFNO, DuctPathOrigin.termFID);

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        #region get all duct/subduct/innerduct/cable
        public List<Duct> GetAllDuct(int iFID, bool from)
        {
            List<Duct> ductsnew = new List<Duct>();

            string sSql = 
                "  select g3e_fno, g3e_fid from GC_CONTAIN where (G3E_FNO =4400 or G3E_FNO =4500 or " +
                " G3E_FNO =7000 or  G3E_FNO =7200 or  G3E_FNO =7400 ) and g3e_ownerfid in (" +
" ( select g3e_fid from GC_CONTAIN where G3E_FNO in (16100,2100,2300) and g3e_ownerfno=2400 and g3e_ownerfid = " + iFID.ToString() + " ) " +
" union " +
 " ( select g3e_fid from GC_CONTAIN where  G3E_FNO in (2100,16100) and g3e_ownerfno=2300 and g3e_ownerfid in " +
 " ( select g3e_fid from GC_CONTAIN where G3E_FNO=2300 and g3e_ownerfno=2400 and g3e_ownerfid = " + iFID.ToString() + " )) " +
" union  " +
"( select g3e_fid from GC_CONTAIN where   G3E_FNO = 2100 and g3e_ownerfno=16100 and g3e_ownerfid in  " +
 "( select g3e_fid from GC_CONTAIN where G3E_FNO =16100 and g3e_ownerfno=2300 and g3e_ownerfid in " +
 " ( select g3e_fid from GC_CONTAIN where G3E_FNO=2300 and g3e_ownerfno=2400 and g3e_ownerfid = " + iFID.ToString() + " ))))" +
 " union " +
                "select g3e_fno, g3e_fid from GC_CONTAIN where   G3E_FNO = 2100 and g3e_ownerfno=16100 and g3e_ownerfid in  " +
 "( select g3e_fid from GC_CONTAIN where G3E_FNO =16100 and g3e_ownerfno=2300 and g3e_ownerfid in " +
 "( select g3e_fid from GC_CONTAIN where G3E_FNO=2300 and g3e_ownerfno=2400 and g3e_ownerfid = " + iFID.ToString() + " ))  " +
 " union " +
 "select g3e_fno, g3e_fid from GC_CONTAIN where  G3E_FNO in (2100,16100) and g3e_ownerfno=2300 and g3e_ownerfid in " +
 "( select g3e_fid from GC_CONTAIN where G3E_FNO=2300 and g3e_ownerfno=2400 and g3e_ownerfid = " + iFID.ToString() + " ) " +
" union  " +
"( select g3e_fno, g3e_fid from GC_CONTAIN where G3E_FNO in (16100,2100,2300) and g3e_ownerfno=2400 and g3e_ownerfid = " + iFID.ToString() + " ) ";

            ADODB.Recordset rsPP = new ADODB.Recordset();
            rsPP = m_IGTDataContext.OpenRecordset(sSql, ADODB.CursorTypeEnum.adOpenStatic, ADODB.LockTypeEnum.adLockReadOnly, 1);

            if (rsPP.RecordCount > 0)
            {
                rsPP.MoveFirst();
                for (int i = 0; i < rsPP.RecordCount; i++)
                {
                    Duct temp = new Duct();
                    temp.FNO = short.Parse(rsPP.Fields[0].Value.ToString());
                    temp.FID = int.Parse(rsPP.Fields[1].Value.ToString());
                    IGTKeyObject oDuct = m_IGTDataContext.OpenFeature(temp.FNO, temp.FID);

                    mobjRelationshipService.ActiveFeature = oDuct;
                    IGTKeyObjects gtKOs = mobjRelationshipService.GetRelatedFeatures(6);
                    foreach (IGTKeyObject obj in gtKOs)
                    {
                        temp.OwnerFID = obj.FID;
                        temp.OwnerFNO = obj.FNO;

                    }
                    short sCNO = GetCno(temp.FNO, from);
                    temp.styleID = GetStyleId(temp.FNO);
                    if (!oDuct.Components.GetComponent(sCNO).Recordset.EOF)
                    {
                        oDuct.Components.GetComponent(sCNO).Recordset.MoveFirst();
                        for (int j = 0; j < oDuct.Components.GetComponent(sCNO).Recordset.RecordCount; j++)
                        {
                            IGTOrientedPointGeometry temppoint = (IGTOrientedPointGeometry)(oDuct.Components.GetComponent(sCNO).Geometry);//.ExtractGeometry(((IGTCompositePolylineGeometry)oDuctPathFeature.Components.GetComponent(2210).Geometry).FirstPoint, ((IGTCompositePolylineGeometry)oDuctPathFeature.Components.GetComponent(2210).Geometry).LastPoint, false);
                            OrientedPointGeom ttemp = new OrientedPointGeom();
                            ttemp.geom = temppoint;
                            ttemp.CNO = sCNO;
                            for (int k = 0; k < oDuct.Components.GetComponent(sCNO).Recordset.Fields.Count; k++)
                            {
                                if (oDuct.Components.GetComponent(sCNO).Recordset.Fields[k].Name == "G3E_CID")
                                {
                                    ttemp.CID = int.Parse(oDuct.Components.GetComponent(sCNO).Recordset.Fields[k].Value.ToString());
                                }
                            }
                            if (temp.Form == null)
                                temp.Form = new List<OrientedPointGeom>();
                            temp.Form.Add(ttemp);
                            oDuct.Components.GetComponent(sCNO).Recordset.MoveNext();
                        }
                    }
                    ductsnew.Add(temp);
                    rsPP.MoveNext();
                }

            }
            rsPP = null;
            return ductsnew;
        }

        public short GetCno(short FNO, bool from)
        {
            if (FNO == 16100 && from)
                return 16120;
            if (FNO == 16100 && !from)
                return 16122;

            if (FNO == 2100 && from)
                return 2120;
            if (FNO == 2100 && !from)
                return 2122;

            if (FNO == 2300 && from)
                return 2320;
            if (FNO == 2300 && !from)
                return 2322;

            if (FNO == 4400)// && from)
                return 4422;
            //if (FNO == 4400 && !from)
            //   return 4400;

            if (FNO == 4500)//&& from)
                return 4522;
            //if (FNO == 4500 && !from)
            //   return 4500;

            if (FNO == 7000)// && from)
                return 7020;
            //if (FNO == 7000 && !from)
            //   return 7000;

            if (FNO == 7200)//&& from)
                return 7222;
            //if (FNO == 7200 && !from)
            //   return 7200;

            if (FNO == 7400)//&& from)
                return 7422;
            //if (FNO == 7400 && !from)
            //   return 7400;
            return 0;
        }

        public int GetStyleId(short FNO)
        {
            if (FNO == 16100)
                return 1612002;
            if (FNO == 2100)
                return 2120002;
            if (FNO == 2300)
                return 2320002;

            if (FNO == 4400)
                return 7020002;
            if (FNO == 4500)
                return 7020002;
            if (FNO == 7000)
                return 7020002;
            if (FNO == 7200)
                return 7020002;
            if (FNO == 7400)
                return 7020002;
            return 0;
        }
        #endregion
        #endregion

        #region GetFNObyFeatureType
        public short GetFNObyFeatureType(string type)
        {
            if (type.Trim().ToUpper() == "MANHOLE")
                return 2700;
            if (type.TrimEnd().TrimStart().ToUpper() == "CIVIL NODE" || type.TrimEnd().TrimStart().ToUpper() == "NODE")
                return 2800;
            if (type.Trim().ToUpper() == "CHAMBER")
                return 3800;
            if (type.Trim().ToUpper() == "TUNNEL" || type.Trim().ToUpper() == "TRENCH")
                return 3300;
            return 0;
        }
        #endregion

        #region angle between points
        public double AngleBtwPoint(double diffX, double diffY)
        {
            double t1 = diffY;
            double t2 = diffX;

            if (t1 == 0 && t2 == 0) return 0;
            if (t2 == 0)
            {
                if (t1 > 0)
                    return 90;
                if (t1 < 0)
                    return -90;
                if (t1 == 0)
                    return 0;
            }

            double grad = Math.Atan(Math.Abs(t1 / t2)) * 180 / Math.PI;

            if (t2 > 0)
            {
                if (t1 > 0)
                    return grad;
                if (t1 < 0)
                    return -grad;
                if (t1 == 0)
                    return 0;
            }
            if (t2 < 0)
            {
                if (t1 > 0)
                    return 180 - grad;
                if (t1 < 0)
                    return 180 + grad;
                if (t1 == 0)
                    return 180;
            }
            return 0;

        }
        #endregion

        #region Get Value from Database
        private string Get_Value(string sSql)
        {
            try
            {
                ADODB.Recordset rsPP = new ADODB.Recordset();
                rsPP = m_IGTDataContext.OpenRecordset(sSql, ADODB.CursorTypeEnum.adOpenStatic, ADODB.LockTypeEnum.adLockReadOnly, 1);
                if (rsPP.RecordCount > 0)
                {
                    rsPP.MoveFirst();
                    return (rsPP.Fields[0].Value.ToString());
                }
                return "";
            }
            catch (Exception ex)
            {
                return "";
            }

        }
        #endregion

        #region Exit CustomCommand
        void m_CustomForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            ExitCmd();
        }

        public void ExitCmd()
        {
            //    m_gtapp.SetProgressBarRange(0, 0);
            m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Exiting...");
            m_gtapp.RefreshWindows();
            m_gtapp.EndProgressBar();
            m_gtapp.SelectedObjects.Clear();
            UnsubscribeEvents();
            step = 0;
             CountGeomDuctNestFrom = 0;
             CountGeomDuctNestTo = 0;
            if (mobjEditService != null)
            {
                mobjEditService.RemoveAllGeometries();
                mobjEditService = null;
            }
            if (mobjEditService1 != null)
            {
                mobjEditService1.RemoveAllGeometries();
                mobjEditService1 = null;
            }
            if (mobjEditService2 != null)
            {
                mobjEditService2.RemoveAllGeometries();
                mobjEditService2 = null;
            }
            if (mobjEditService3 != null)
            {
                mobjEditService3.RemoveAllGeometries();
                mobjEditService3 = null;
            }
             if (mobjEditServiceRotate != null)
            {
                mobjEditServiceRotate.RemoveAllGeometries();
                mobjEditServiceRotate = null;
            }
            if (mobjEditServiceRotate1 != null)
            {
                mobjEditServiceRotate1.RemoveAllGeometries();
                mobjEditServiceRotate1 = null;
            }
            if (mobjEditServiceRotate2 != null)
            {
                mobjEditServiceRotate2.RemoveAllGeometries();
                mobjEditServiceRotate2 = null;
            }
            if (mobjEditServiceRotate3 != null)
            {
                mobjEditServiceRotate3.RemoveAllGeometries();
                mobjEditServiceRotate3 = null;
            }
            if (mobjEditServiceSlash != null)
            {
                mobjEditServiceSlash.RemoveAllGeometries();
                mobjEditServiceSlash = null;
            }

            if (mobjEditServiceDuctPathFrom != null)
            {
                mobjEditServiceDuctPathFrom.RemoveAllGeometries();
                mobjEditServiceDuctPathFrom = null;
            }
            if (mobjEditServiceDuctPathTo != null)
            {
                mobjEditServiceDuctPathTo.RemoveAllGeometries();
                mobjEditServiceDuctPathTo = null;
            }
            if (mobjEditPointService != null) mobjEditPointService.RemoveAllGeometries();
            if (mobjEditServiceTemp != null) mobjEditServiceTemp.RemoveAllGeometries();
            if (mobjEditService != null) mobjEditService.RemoveAllGeometries();

            if (mobjEditPointServiceNew != null) mobjEditPointServiceNew.RemoveAllGeometries();
            if (mobjEditServiceNewTemp != null) mobjEditServiceNewTemp.RemoveAllGeometries();
            if (mobjEditServiceNew != null) mobjEditServiceNew.RemoveAllGeometries();

            if (mobjLocateService != null)
                mobjLocateService = null;
            if (mobjRelationshipService != null)
            {
                mobjRelationshipService = null;
            }

            if (DuctPathOrigin != null)
            {
                LocateFeature(2, m_gtapp.ActiveMapWindow, DuctPathOrigin.sourceFNO, DuctPathOrigin.sourceFID, DuctPathOrigin.termFNO, DuctPathOrigin.termFID);

                if (DuctPathOrigin.Sections != null)
                    DuctPathOrigin.Sections.Clear();
                if (DuctPathOrigin.SectLabels != null)
                    DuctPathOrigin.SectLabels.Clear();
                if (DuctPathOrigin.SectSlashes != null)
                    DuctPathOrigin.SectSlashes.Clear();
                DuctPathOrigin = null;
            }

            if (DuctPathFrom != null)
            {
                if (DuctPathFrom.Sections != null)
                    DuctPathFrom.Sections.Clear();
                if (DuctPathFrom.SectLabels != null)
                    DuctPathFrom.SectLabels.Clear();
                if (DuctPathFrom.SectSlashes != null)
                    DuctPathFrom.SectSlashes.Clear();
                DuctPathFrom = null;
            }

            if (DuctPathTo != null)
            {
                if (DuctPathTo.Sections != null)
                    DuctPathTo.Sections.Clear();
                if (DuctPathTo.SectLabels != null)
                    DuctPathTo.SectLabels.Clear();
                if (DuctPathTo.SectSlashes != null)
                    DuctPathTo.SectSlashes.Clear();
                DuctPathTo = null;
            }
            m_oIGTCustomCommandHelper.Complete();

        }
        #endregion

        #region Coordinate for Projected Point on Conduit line
        public IGTPoint PointOnConduit(double Xslash, double Yslash, IGTPolylineGeometry DuctPathLineGeom)
        {

            IGTPoint projectPoint = GTClassFactory.Create<IGTPoint>();
            projectPoint.X = 0;
            projectPoint.Y = 0;
            projectPoint.Z = 0;
            List<IGTPoint> ProjectedPoints = new List<IGTPoint>();
            for (int i = 0; i < DuctPathLineGeom.Points.Count - 1; i++)
            {
                ProjectedPoints.Add(ProjectedPointOnConduit(DuctPathLineGeom.Points[i].X,
                DuctPathLineGeom.Points[i].Y,
                DuctPathLineGeom.Points[i + 1].X,
                DuctPathLineGeom.Points[i + 1].Y,
                Xslash, Yslash));
            }
            double min = 0;
            int sectnum = 0;
            for (int i = 0; i < ProjectedPoints.Count; i++)
            {
                double difX = ProjectedPoints[i].X - Xslash;
                double difY = ProjectedPoints[i].Y - Yslash;

                if (difX < 0) difX *= -1;
                if (difY < 0) difY *= -1;
                double disMin = difY + difX;
                if (min == 0) min = disMin + 1;
                if (min > disMin)
                {
                    projectPoint.X = ProjectedPoints[i].X;
                    projectPoint.Y = ProjectedPoints[i].Y;
                    projectPoint.Z = 0;
                    min = disMin;
                    sectnum = i + 1;
                }

            }
            //checking if mouse click too far from conduit
            if (min > 3)
            {
                projectPoint.X = 0;
                projectPoint.Y = 0;
                projectPoint.Z = 0;
                sectnum = 0;
            }
            if (sectnum > 0)
            {

                int length = 0;
                for (int i = 0; i < DuctPathLineGeom.Points.Count - 1; i++)
                {
                    if (sectnum == i + 1)
                    {
                        length += LegthBtwTwoPoints(DuctPathLineGeom.Points[i].X,
                        DuctPathLineGeom.Points[i].Y, Xslash, Yslash);
                        break;
                    }
                    else
                        length += LegthBtwTwoPoints(DuctPathLineGeom.Points[i].X,
                            DuctPathLineGeom.Points[i].Y,
                            DuctPathLineGeom.Points[i + 1].X,
                            DuctPathLineGeom.Points[i + 1].Y);
                }
                projectPoint.Z = double.Parse(length.ToString());
            }
            return projectPoint;

        }
        #endregion
        #region Coordinate for Projected Point on SEGMENT of Conduit line
        public IGTPoint ProjectedPointOnConduit(double stX, double stY, double endX, double endY, double slashX, double slashY)
        {
            IGTSegment sectT = GTClassFactory.Create<IGTSegment>();
            IGTSegmentPoint slashT = GTClassFactory.Create<IGTSegmentPoint>();
            IGTPoint clickT = GTClassFactory.Create<IGTPoint>();
            IGTPoint projectPoint = GTClassFactory.Create<IGTPoint>();
            sectT.Point1.X = stX;
            sectT.Point1.Y = stY;
            sectT.Point1.Z = 0;
            sectT.Point2.X = endX;
            sectT.Point2.Y = endY;
            sectT.Point2.Z = 0;

            clickT.X = slashX;
            clickT.Y = slashY;
            clickT.Z = 0;

            slashT = clickT.ProjectTo(sectT);

            projectPoint.X = slashT.Point.X;
            projectPoint.Y = slashT.Point.Y;
            projectPoint.Z = 0;

            return projectPoint;
        }
        #endregion
        #region Calculate Rotation for Along plc lable
        public double TakeRotationOfSegmentPolyline(int GraphicLength, IGTPolylineGeometry DuctPathLineGeom)
        {
            int lengthTemp = 0;
            double Rotat = 0.0;
            for (int i = 0; i < DuctPathLineGeom.Points.Count - 1; i++)
            {

                int temp = LegthBtwTwoPoints(DuctPathLineGeom.Points[i].X,
                    DuctPathLineGeom.Points[i].Y,
                    DuctPathLineGeom.Points[i + 1].X,
                    DuctPathLineGeom.Points[i + 1].Y);
                lengthTemp += temp;
                if (lengthTemp >= GraphicLength)
                {
                    if (DuctPathLineGeom.Points[i + 1].Y < DuctPathLineGeom.Points[i].Y &&
                        DuctPathLineGeom.Points[i + 1].X <= DuctPathLineGeom.Points[i].X
                        || DuctPathLineGeom.Points[i + 1].Y > DuctPathLineGeom.Points[i].Y &&
                        DuctPathLineGeom.Points[i + 1].X <= DuctPathLineGeom.Points[i].X)
                        Rotat = AngleBtwPoint(DuctPathLineGeom.Points[i + 1].X, DuctPathLineGeom.Points[i + 1].Y,
                            DuctPathLineGeom.Points[i].X, DuctPathLineGeom.Points[i].Y);
                    else
                        Rotat = AngleBtwPoint(DuctPathLineGeom.Points[i].X, DuctPathLineGeom.Points[i].Y,
                                          DuctPathLineGeom.Points[i + 1].X, DuctPathLineGeom.Points[i + 1].Y);
                    break;
                }
            }
            return Rotat;
        }
        #endregion
        #region Angle between segment and OX by start and end's points on segment
        public double AngleBtwPoint(double stX, double stY, double endX, double endY)
        {
            double t1 = endY - stY;
            double t2 = endX - stX;

            if (t1 == 0 && t2 == 0) return 0;
            if (t2 == 0)
            {
                if (t1 > 0)
                    return 90;
                if (t1 < 0)
                    return -90;
                if (t1 == 0)
                    return 0;
            }

            double grad = Math.Atan(Math.Abs(t1 / t2)) * 180 / Math.PI;

            if (t2 > 0)
            {
                if (t1 > 0)
                    return grad;
                if (t1 < 0)
                    return -grad;
                if (t1 == 0)
                    return 0;
            }
            if (t2 < 0)
            {
                if (t1 > 0)
                    return 180 - grad;
                if (t1 < 0)
                    return 180 + grad;
                if (t1 == 0)
                    return 180;
            }
            return 0;

        }
        #endregion
        #region Calculate Length's

        #region Between Two points on simple line
        private int LegthBtwTwoPoints(double startPointX, double startPointY, double endPointX, double endPointY)
        {
            return Convert.ToInt32(Math.Round(Math.Sqrt(Math.Pow((endPointX - startPointX), 2) + Math.Pow((endPointY - startPointY), 2)), 0));
        }
        #endregion

        #region For whole conduit
        private int LengthConduit(IGTPolylineGeometry DuctPathLineGeom)
        {
            int length = 0;
            for (int i = 0; i < DuctPathLineGeom.Points.Count - 1; i++)
            {
                length += LegthBtwTwoPoints(DuctPathLineGeom.Points[i].X,
                        DuctPathLineGeom.Points[i].Y,
                        DuctPathLineGeom.Points[i + 1].X,
                        DuctPathLineGeom.Points[i + 1].Y);
            }

            return length;
        }
        #endregion
        #endregion

        #region Load manhole
        void StartMHplc()
        {
            mobjEditService = GTClassFactory.Create<IGTGeometryEditService>();
            mobjEditService.TargetMapWindow = m_gtapp.ActiveMapWindow;

            mobjEditPointService = GTClassFactory.Create<IGTGeometryEditService>();
            mobjEditPointService.TargetMapWindow = m_gtapp.ActiveMapWindow;

            mobjEditServiceTemp = GTClassFactory.Create<IGTGeometryEditService>();
            mobjEditServiceTemp.TargetMapWindow = m_gtapp.ActiveMapWindow;


            mobjEditServiceNew = GTClassFactory.Create<IGTGeometryEditService>();
            mobjEditServiceNew.TargetMapWindow = m_gtapp.ActiveMapWindow;

            mobjEditPointServiceNew = GTClassFactory.Create<IGTGeometryEditService>();
            mobjEditPointServiceNew.TargetMapWindow = m_gtapp.ActiveMapWindow;

            mobjEditServiceNewTemp = GTClassFactory.Create<IGTGeometryEditService>();
            mobjEditServiceNewTemp.TargetMapWindow = m_gtapp.ActiveMapWindow;

            mobjExplorerService = GTClassFactory.Create<IGTFeatureExplorerService>();
            mobjExplorerService.CancelClick += new EventHandler(mobjExplorerService_CancelClick);
            mobjExplorerService.SaveAndContinueClick += new EventHandler(mobjExplorerService_SaveAndContinueClick);
            mobjExplorerService.SaveClick += new EventHandler(mobjExplorerService_SaveClick);

            m_oIGTTransactionManager.Begin("new manhole plc");

            mobjManholeAttribute = m_gtapp.DataContext.NewFeature(iManholeFNO, true);
            mobjExplorerService.ExploreFeature(mobjManholeAttribute, "Manhole");
            mobjExplorerService.Visible = true;
            mobjExplorerService.Slide(true);
        }

        private void LoadManholeType(IGTKeyObject _attr)
        {
            string ManholeType = "JC9";
            string MH_MIX = "";
            if (!_attr.Components.GetComponent(2701).Recordset.EOF)
            {
                ManholeType = _attr.Components.GetComponent(2701).Recordset.Fields["FEATURE_TYPE"].Value.ToString();
                MH_MIX = _attr.Components.GetComponent(2701).Recordset.Fields["MH_MIX"].Value.ToString();
            }
            cManhole = new ManholeCtrl();
            cManhole.ManholeType = ManholeType;
            cManhole.LoadManholeType();
            StyleManhole = cManhole.ManholeStyle;
            StyleTempManhole = cManhole.ManholeTempStyle;

            IGTHelperService helper = GTClassFactory.Create<IGTHelperService>();
            helper.DataContext = m_gtapp.DataContext;

            StyleManhole = helper.GetComponentStyleID(_attr, 2720);
            StyleTempManhole = StyleManhole;

            cManhole.ManholeStyle = StyleManhole;
            cManhole.ManholeTempStyle = StyleTempManhole;

            string manholeLabel = "[ManholeLabel]";
            GTAlignmentConstants ManholeLabelAlignment = GTAlignmentConstants.gtalBottomLeft;

            cManhole.ManholeTextStyle = helper.GetComponentStyleID(_attr, 2730);
            helper.GetComponentLabel(_attr, 2730, ref manholeLabel, ref ManholeLabelAlignment);

            string decription = "";
            if (MH_MIX.ToUpper() == "PRE-FAB")
                decription = ManholeType + "/(P)";
            else if (MH_MIX.ToUpper() == "PRE-FAB (SHUTTERING)")
                decription = ManholeType + "/(P/S)";
            else if (MH_MIX.ToUpper() == "SITE")
                decription = ManholeType;
            else decription = ManholeType + MH_MIX;

            // REPLACE  "NEW LINE" MORE THAN 1 WITH 1 "NEW LINE"
            //  decription=decription.Replace("\s{2,}","\n");

            cManhole.ManholeLabel = "???\n" + decription;
            cManhole.ManholeLabelDesription = decription;
            cManhole.ManholeLabelAlignment = ManholeLabelAlignment;
            cManhole.ManholeWallStyle = helper.GetComponentStyleID(_attr, 2732);

        }
        #endregion

        #region create mh geometry

        private void CreateManholeGeom(IGTPoint objPoint, IGTPoint objPointRotate, IGTGeometryEditService mobjEditServiceIn)
        {
            CreateManholeGeom(objPoint, objPointRotate, mobjEditServiceIn, StyleManhole);

            //return oOrPointGeom;
        }

        private void CreateManholeGeom(IGTPoint objPoint, IGTPoint objPointRotate, IGTGeometryEditService mobjEditServiceIn, int StyleId)
        {
            IGTOrientedPointGeometry oOrPointGeom = PGeoLib.CreatePointGeom(objPoint.X, objPoint.Y);

            //if (mobjEditService.GeometryCount > 0) mobjEditService.RemoveAllGeometries();
            if (StyleId == 0) StyleId = StyleManhole;
            mobjEditServiceIn.AddGeometry(oOrPointGeom, StyleId);

            if (objPointRotate != null)
            {
                IGTPoint objPointTmp = GTClassFactory.Create<IGTPoint>();
                objPointTmp.X = objPoint.X + 100;
                objPointTmp.Y = objPoint.Y;

                mobjEditServiceIn.BeginRotate(objPointTmp, objPoint);
                mobjEditServiceIn.Rotate(objPointRotate);
                mobjEditServiceIn.EndRotate(objPointRotate);
                //oOrPointGeom = (IGTOrientedPointGeometry)mobjEditService.GetGeometry(1);
            }

            //return oOrPointGeom;
        }
        #endregion

        #region create new iserting manhl
        private void CreateManhole(ManholeCtrl ctlManhole)
        {
            short iFNO;
            short iCNO;
            long lFID;          
            IGTKeyObject oNewFeature;
           
            IGTOrientedPointGeometry oOrPointGeom = null;

            if (ctlManhole == null || mobjEditService == null)
            {
                MessageBox.Show("Error during placement of Manhole! Exiting custom command", "Manhole Insert", MessageBoxButtons.OK, MessageBoxIcon.Error);
                m_gtapp.EndProgressBar();
                m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "");
                ExitCmd();
                return;
            }
            Messages msg = new Messages();
            msg.Message(2);
            msg.Show();
            try
            {
                m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Wait, Placing Manhole in process ... ");

                m_gtapp.BeginProgressBar();
                m_gtapp.SetProgressBarRange(0, 100);
                m_gtapp.SetProgressBarPosition(15);
                m_oIGTTransactionManager.Begin("Place new manhole");

                m_gtapp.SetProgressBarPosition(35);
                IGTPoint oPoint = ctlManhole.Origin;
                IGTPoint oPointRotate = ctlManhole.RotationPnt;

                iFNO = iManholeFNO;
                oNewFeature = m_gtapp.DataContext.NewFeature(iFNO);

                // FID generation.
                lFID = oNewFeature.FID;

                if (mobjManholeAttribute != null)
                    PGeoLib.CopyComponentAttribute(mobjManholeAttribute, oNewFeature);
               

                //Symbol
                iCNO = 2720;
                if (oNewFeature.Components.GetComponent(iCNO).Recordset.EOF)
                {
                    oNewFeature.Components.GetComponent(iCNO).Recordset.AddNew("G3E_FID", lFID);
                    oNewFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_FNO", iFNO);
                }
                else
                {
                    oNewFeature.Components.GetComponent(iCNO).Recordset.MoveLast();
                }

                // GetCompInfo iFNO, iCNO, bMainGr, grType
                oOrPointGeom = PGeoLib.CreatePointGeom(oPoint.X, oPoint.Y);

                if (mobjEditService.GeometryCount > 0) mobjEditService.RemoveAllGeometries();
                mobjEditService.AddGeometry(oOrPointGeom, StyleManhole);

                if (oPointRotate != null)
                {
                    IGTPoint objPointTmp = GTClassFactory.Create<IGTPoint>();
                    objPointTmp.X = oPoint.X + 100;
                    objPointTmp.Y = oPoint.Y;

                    mobjEditService.BeginRotate(objPointTmp, oPoint);
                    mobjEditService.Rotate(oPointRotate);
                    mobjEditService.EndRotate(oPointRotate);
                    oOrPointGeom = (IGTOrientedPointGeometry)mobjEditService.GetGeometry(1);
                }
                oNewFeature.Components.GetComponent(iCNO).Geometry = oOrPointGeom;

                //Wall number
                iCNO = 2732;
                int fNbr = cManhole.FirstNumber;
                if (cManhole.ManholeWall != null)
                {
                    for (int i = 0; i < cManhole.ManholeWall.Count; i++)
                    {
                        if (fNbr >= cManhole.ManholeWall.Count) fNbr = 0;
                        ManholePoint _pnt = cManhole.ManholeWall[fNbr];
                        IGTTextPointGeometry txtNumber = PGeoLib.CreateTextGeom(cManhole.Origin.X + _pnt.X, cManhole.Origin.Y + _pnt.Y, Convert.ToString(i + 1));

                        IGTPoint objPointTmp = GTClassFactory.Create<IGTPoint>();
                        objPointTmp.X = oPoint.X + 100;
                        objPointTmp.Y = oPoint.Y;

                        if (mobjEditService.GeometryCount > 0) mobjEditService.RemoveAllGeometries();
                        mobjEditService.AddGeometry(txtNumber, cManhole.ManholeWallStyle);
                        if (oPointRotate != null)
                        {

                            mobjEditService.BeginRotate(objPointTmp, oPoint);
                            mobjEditService.Rotate(oPointRotate);
                            mobjEditService.EndRotate(oPointRotate);
                            txtNumber = (IGTTextPointGeometry)mobjEditService.GetGeometry(1);
                        }

                        oNewFeature.Components.GetComponent(iCNO).Recordset.AddNew("G3E_FID", lFID);
                        oNewFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_FNO", iFNO);
                        oNewFeature.Components.GetComponent(iCNO).Recordset.Update("WALL_NUM", Convert.ToString(i + 1));
                        oNewFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_ALIGNMENT", 0);
                        oNewFeature.Components.GetComponent(iCNO).Geometry = txtNumber;

                        fNbr++;
                    }
                }

                //Label
                iCNO = 2730;
                IGTTextPointGeometry txtLabel = PGeoLib.CreateTextGeom(cManhole.LabelOrigin.X, cManhole.LabelOrigin.Y, "[Manhole label]", 0, GTAlignmentConstants.gtalTopLeft);

                if (oNewFeature.Components.GetComponent(iCNO).Recordset.EOF)
                {
                    oNewFeature.Components.GetComponent(iCNO).Recordset.AddNew("G3E_FID", lFID);
                    oNewFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_FNO", iFNO);
                }
                else
                {
                    oNewFeature.Components.GetComponent(iCNO).Recordset.MoveLast();
                }

                oNewFeature.Components.GetComponent(iCNO).Geometry = txtLabel;

                //Label Description
                iCNO = 2734;
                txtLabel = PGeoLib.CreateTextGeom(cManhole.LabelOrigin.X, cManhole.LabelOrigin.Y - 2.5, "[Manhole label description]", 0, GTAlignmentConstants.gtalTopLeft);

                if (oNewFeature.Components.GetComponent(iCNO).Recordset.EOF)
                {
                    oNewFeature.Components.GetComponent(iCNO).Recordset.AddNew("G3E_FID", lFID);
                    oNewFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_FNO", iFNO);
                }
                else
                {
                    oNewFeature.Components.GetComponent(iCNO).Recordset.MoveLast();
                }

                oNewFeature.Components.GetComponent(iCNO).Geometry = txtLabel;
                iManholeFID = int.Parse(lFID.ToString());
                m_gtapp.SetProgressBarPosition(55);
                         m_oIGTTransactionManager.Commit();

                         m_gtapp.SetProgressBarPosition(75);
                         m_oIGTTransactionManager.RefreshDatabaseChanges();

                         m_gtapp.SetProgressBarPosition(100);
                         m_gtapp.RefreshWindows();
                         m_gtapp.EndProgressBar();
                         m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "");
                         if (mobjEditPointService != null) mobjEditPointService.RemoveAllGeometries();
                         if (mobjEditServiceTemp != null) mobjEditServiceTemp.RemoveAllGeometries();
                         if (mobjEditService != null) mobjEditService.RemoveAllGeometries();

                         if (mobjEditPointServiceNew != null) mobjEditPointServiceNew.RemoveAllGeometries();
                         if (mobjEditServiceNewTemp != null) mobjEditServiceNewTemp.RemoveAllGeometries();
                         if (mobjEditServiceNew != null) mobjEditServiceNew.RemoveAllGeometries();
                         LocateFeature(3, m_gtapp.ActiveMapWindow, DuctPathOrigin.sourceFNO, DuctPathOrigin.sourceFID, iManholeFNO, iManholeFID);
                         if(msg!=null) msg.Close();         
                step = 51;
                
                     }
                     catch (Exception ex)
                     {
                         if(msg!=null) msg.Close();
                         m_oIGTTransactionManager.Rollback();
                         MessageBox.Show(ex.Message, "Manhole Insert", MessageBoxButtons.OK, MessageBoxIcon.Error);
                         m_gtapp.EndProgressBar();
                         m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "");
                         ExitCmd(); 
                         //iManholeFID = 1000277303;
                         //m_gtapp.SetProgressBarPosition(100);
                         //m_gtapp.RefreshWindows();
                         //m_gtapp.EndProgressBar();
                         //m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "");
                         //if (mobjEditPointService != null) mobjEditPointService.RemoveAllGeometries();
                         //if (mobjEditServiceTemp != null) mobjEditServiceTemp.RemoveAllGeometries();
                         //if (mobjEditService != null) mobjEditService.RemoveAllGeometries();

                         //if (mobjEditPointServiceNew != null) mobjEditPointServiceNew.RemoveAllGeometries();
                         //if (mobjEditServiceNewTemp != null) mobjEditServiceNewTemp.RemoveAllGeometries();
                         //if (mobjEditServiceNew != null) mobjEditServiceNew.RemoveAllGeometries();

                         //step = 51;
                     }
                    
        }
        #endregion

        #region Get Wall Number
        public bool GetDeviceWithWall(bool flag)
        {
            IGTGeometry geom = null;
            short iFNO = 0;
            int iFID = 0;
            string WallNum = "";
            short iFNOSelected = iManholeFNO;
            int iFIDSelected = iManholeFID;
            string FeatureType = "MANHOLE";

            

            #region check if selected allowed feature and if successshow detail
            if ( m_gtapp.SelectedObjects.FeatureCount == 0)
            {
                MessageBox.Show("Please select Wall of inserted " + FeatureType + " with FID = " + iFIDSelected.ToString() + " \n to connect Duct Path!", "Manhole Insert", MessageBoxButtons.OK, MessageBoxIcon.Information);
                 m_gtapp.SelectedObjects.Clear();
                return false;
            }

            if ( m_gtapp.SelectedObjects.FeatureCount > 1)
            {
                MessageBox.Show("Please select Wall of inserted " + FeatureType + " with FID = " + iFIDSelected.ToString() + " \n to connect Duct Path!", "Manhole Insert", MessageBoxButtons.OK, MessageBoxIcon.Information);
                 m_gtapp.SelectedObjects.Clear();
                return false;
            }


            foreach (IGTDDCKeyObject oDDCKeyObject in  m_gtapp.SelectedObjects.GetObjects())
            {
                geom = oDDCKeyObject.Geometry;
                iFNO = oDDCKeyObject.FNO;
                iFID = oDDCKeyObject.FID;

                if (iFNOSelected != iFNO)
                {
                    MessageBox.Show("Please select Wall of inserted " + FeatureType + " with FID = " + iFIDSelected.ToString() + " \n to connect Duct Path!", "Manhole Insert", MessageBoxButtons.OK, MessageBoxIcon.Information);
                     m_gtapp.SelectedObjects.Clear();
                    return false;
                }
                if (iFIDSelected != iFID)
                {
                    MessageBox.Show("Please select Wall of inserted " + FeatureType + " with FID = " + iFIDSelected.ToString() + " \n to connect Duct Path!", "Manhole Insert", MessageBoxButtons.OK, MessageBoxIcon.Information);
                     m_gtapp.SelectedObjects.Clear();
                    return false;
                }

                //manhole
                if (iFNO == 2700)
                {
                    if (oDDCKeyObject.ComponentViewName == "VGC_MANHLW_T")
                    {
                        for (int i = 0; i < oDDCKeyObject.Recordset.Fields.Count; i++)
                        {
                            if (oDDCKeyObject.Recordset.Fields[i].Name == "WALL_NUM")
                            {
                                WallNum = oDDCKeyObject.Recordset.Fields[i].Value.ToString();
                                break;
                            }
                        }
                    }
                }
                if (WallNum == "")
                {
                    MessageBox.Show("Please select Wall of  inserted " + FeatureType + " with FID = " + iFIDSelected.ToString() + " \n to connect Duct Path!", "Manhole Insert", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    m_gtapp.SelectedObjects.Clear();
                    return false;
                }
                break;

            }
            #endregion

            

            IGTPolylineGeometry DuctPathLineGeomNew = GTClassFactory.Create<IGTPolylineGeometry>();

            if (flag)
            {
                
                {
                    double length = 0;
                    DuctPathLineGeomNew.Points.Clear();
                    DuctPathLineGeomNew.Points.Add(DuctPathOrigin.DuctPathLineGeom.Points[0]);  
                    for (int i = 0; i < DuctPathOrigin.DuctPathLineGeom.Points.Count - 1; i++)
                    {
                        length += LegthBtwTwoPoints(DuctPathOrigin.DuctPathLineGeom.Points[i].X, DuctPathOrigin.DuctPathLineGeom.Points[i].Y,
                        DuctPathOrigin.DuctPathLineGeom.Points[i+1].X, DuctPathOrigin.DuctPathLineGeom.Points[i+1].Y);
                       
                        if (length <= BreakSlash.length)
                            DuctPathLineGeomNew.Points.Add(DuctPathOrigin.DuctPathLineGeom.Points[i+1]);
                        else break;
                    }
                    DuctPathLineGeomNew.Points.Add(geom.FirstPoint);                   

                }
            }
            else
            {
                DuctPathLineGeomNew.Points.Clear();
                DuctPathLineGeomNew.Points.Add(geom.FirstPoint);
                double length = 0;
                for (int i = 1; i < DuctPathOrigin.DuctPathLineGeom.Points.Count; i++)
                {
                    length += LegthBtwTwoPoints(DuctPathOrigin.DuctPathLineGeom.Points[i - 1].X, DuctPathOrigin.DuctPathLineGeom.Points[i - 1].Y,
                        DuctPathOrigin.DuctPathLineGeom.Points[i].X, DuctPathOrigin.DuctPathLineGeom.Points[i].Y);
                    if (length >= BreakSlash.length)
                        DuctPathLineGeomNew.Points.Add(DuctPathOrigin.DuctPathLineGeom.Points[i]);
                }
                
            }

            if (!ReOwnWall(DuctPathLineGeomNew, flag, WallNum))
            {
                 m_gtapp.SelectedObjects.Clear();
                return false;
            }
            return true;
        }
        #endregion

        #region Re-OwnWall
        public bool ReOwnWall(IGTPolylineGeometry geomNew, bool from, string wallnum)
        {
            try
            {
                if (from)
                    return ReOwnWallFrom(geomNew,  wallnum);
                else return ReOwnWallTo(geomNew,  wallnum);
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }
      
       #region ReOwnWallFrom
        public bool ReOwnWallFrom(IGTPolylineGeometry geomNew, string wallnum)
        {
            try
            {
                DuctPath DuctpathNew = new DuctPath();
                #region copy attr
                DuctpathNew.BillingRate = DuctPathOrigin.BillingRate;
                DuctpathNew.ConstructBy = DuctPathOrigin.ConstructBy;
                DuctpathNew.DBFlag = DuctPathOrigin.DBFlag;
                DuctpathNew.DuctWay = DuctPathOrigin.DuctWay;
                DuctpathNew.EXC_ABB = DuctPathOrigin.EXC_ABB;
                DuctpathNew.Feature_state = DuctPathOrigin.Feature_state;
                DuctpathNew.FID = DuctPathOrigin.FID;
                DuctpathNew.FNO = DuctPathOrigin.FNO;
                DuctpathNew.InstallYear = DuctPathOrigin.InstallYear;
                DuctpathNew.sourceFID = DuctPathOrigin.sourceFID;
                DuctpathNew.sourceFNO = DuctPathOrigin.sourceFNO;
                DuctpathNew.sourceType = DuctPathOrigin.sourceType;
                DuctpathNew.sourceWall = DuctPathOrigin.sourceWall;
                DuctpathNew.termFID = DuctPathOrigin.termFID;
                DuctpathNew.termFNO = DuctPathOrigin.termFNO;
                DuctpathNew.termType = DuctPathOrigin.termType;
                DuctpathNew.termWall = DuctPathOrigin.termWall;
                DuctpathNew.ASb_SubDuct = DuctPathOrigin.ASb_SubDuct;
                DuctpathNew.Prop_InnDuct = DuctPathOrigin.Prop_InnDuct;
                DuctpathNew.Prop_SubDuct = DuctPathOrigin.Prop_SubDuct;
                DuctpathNew.NestAssignDuctWay = DuctPathOrigin.NestAssignDuctWay;
                DuctpathNew.Description = DuctPathOrigin.Description;
                DuctpathNew.DuctBalance = DuctPathOrigin.DuctBalance;
                DuctpathNew.Feature_type = DuctPathOrigin.Feature_type;
                DuctpathNew.ExpandFlag = DuctPathOrigin.ExpandFlag;

                //duct nest from  
                #region duct nest from
                if (DuctPathOrigin.DuctNestFrom != null)
                {
                    DuctpathNew.DuctNestFrom = new List<DuctNest>();
                    for (int n = 0; n < DuctPathOrigin.DuctNestFrom.Count; n++)
                    {

                        DuctNest temp = new DuctNest();
                        temp.FID = DuctPathOrigin.DuctNestFrom[n].FID;
                        temp.FNO = DuctPathOrigin.DuctNestFrom[n].FNO;
                        temp.styleIDform = DuctPathOrigin.DuctNestFrom[n].styleIDform;
                        temp.styleIDlabel = DuctPathOrigin.DuctNestFrom[n].styleIDlabel;
                        temp.Form = new List<PolylineGeom>();
                        //formation line
                        for (int i = 0; i < DuctPathOrigin.DuctNestFrom[n].Form.Count; i++)
                        {
                            PolylineGeom tt = new PolylineGeom();
                            tt.CID = DuctPathOrigin.DuctNestFrom[n].Form[i].CID;
                            tt.CNO = DuctPathOrigin.DuctNestFrom[n].Form[i].CNO;
                            IGTPolylineGeometry templine = GTClassFactory.Create<IGTPolylineGeometry>();
                            for (int ll = 0; ll < DuctPathOrigin.DuctNestFrom[n].Form[i].geom.Points.Count; ll++)
                            {
                                IGTPoint newpoint = GTClassFactory.Create<IGTPoint>();
                                newpoint.X = DuctPathOrigin.DuctNestFrom[n].Form[i].geom.Points[ll].X;
                                newpoint.Y = DuctPathOrigin.DuctNestFrom[n].Form[i].geom.Points[ll].Y;
                                newpoint.Z = DuctPathOrigin.DuctNestFrom[n].Form[i].geom.Points[ll].Z;
                                templine.Points.Add(newpoint);

                            }
                            tt.geom = templine;

                            temp.Form.Add(tt);
                        }
                        temp.Labels = new List<TextPointGeom>();
                        // labels
                        for (int i = 0; i < DuctPathOrigin.DuctNestFrom[n].Labels.Count; i++)
                        {
                            TextPointGeom tt = new TextPointGeom();
                            tt.CID = DuctPathOrigin.DuctNestFrom[n].Labels[i].CID;
                            tt.CNO = DuctPathOrigin.DuctNestFrom[n].Labels[i].CNO;
                            IGTTextPointGeometry templab = GTClassFactory.Create<IGTTextPointGeometry>();
                            IGTPoint newpoint = GTClassFactory.Create<IGTPoint>();
                            newpoint.X = DuctPathOrigin.DuctNestFrom[n].Labels[i].geom.Origin.X;
                            newpoint.Y = DuctPathOrigin.DuctNestFrom[n].Labels[i].geom.Origin.Y;
                            newpoint.Z = DuctPathOrigin.DuctNestFrom[n].Labels[i].geom.Origin.Z;
                            templab.Origin = newpoint;
                            templab.Rotation = DuctPathOrigin.DuctNestFrom[n].Labels[i].geom.Rotation;
                            templab.Text = DuctPathOrigin.DuctNestFrom[n].Labels[i].geom.Text;
                            templab.Alignment = DuctPathOrigin.DuctNestFrom[n].Labels[i].geom.Alignment;
                            tt.geom = templab;

                            temp.Labels.Add(tt);
                        }
                        if (DuctPathOrigin.DuctNestFrom[n].Ducts != null)
                        {
                            temp.Ducts = new List<Duct>();
                            #region ducts
                            for (int i = 0; i < DuctPathOrigin.DuctNestFrom[n].Ducts.Count; i++)
                            {
                                Duct tt = new Duct();
                                tt.FID = DuctPathOrigin.DuctNestFrom[n].Ducts[i].FID;
                                tt.FNO = DuctPathOrigin.DuctNestFrom[n].Ducts[i].FNO;
                                tt.OwnerFNO = DuctPathOrigin.DuctNestFrom[n].Ducts[i].OwnerFNO;
                                tt.OwnerFID = DuctPathOrigin.DuctNestFrom[n].Ducts[i].OwnerFID;
                                tt.styleID = DuctPathOrigin.DuctNestFrom[n].Ducts[i].styleID;
                                tt.Form = new List<OrientedPointGeom>();
                                for (int j = 0; j < DuctPathOrigin.DuctNestFrom[n].Ducts[i].Form.Count; j++)
                                {
                                    OrientedPointGeom ttt = new OrientedPointGeom();
                                    ttt.CID = DuctPathOrigin.DuctNestFrom[n].Ducts[i].Form[j].CID;
                                    ttt.CNO = DuctPathOrigin.DuctNestFrom[n].Ducts[i].Form[j].CNO;

                                    IGTOrientedPointGeometry tempduct = GTClassFactory.Create<IGTOrientedPointGeometry>();
                                    IGTPoint newpoint = GTClassFactory.Create<IGTPoint>();
                                    newpoint.X = DuctPathOrigin.DuctNestFrom[n].Ducts[i].Form[j].geom.Origin.X;
                                    newpoint.Y = DuctPathOrigin.DuctNestFrom[n].Ducts[i].Form[j].geom.Origin.Y;
                                    newpoint.Z = DuctPathOrigin.DuctNestFrom[n].Ducts[i].Form[j].geom.Origin.Z;
                                    tempduct.Origin = newpoint;

                                    ttt.geom = tempduct;// DuctPathOrigin.DuctNestFrom[n].Ducts[i].Form[j].geom;
                                    tt.Form.Add(ttt);
                                }
                                temp.Ducts.Add(tt);
                            }
                            #endregion
                        }
                        DuctpathNew.DuctNestFrom.Add(temp);
                    }
                }
                #endregion
                
                //duct nest to    
                #region duct nest to
                if (DuctPathOrigin.DuctNestTo != null)
                {
                    DuctpathNew.DuctNestTo = new List<DuctNest>();
                    for (int n = 0; n < DuctPathOrigin.DuctNestTo.Count; n++)
                    {


                        DuctNest temp = new DuctNest();
                        temp.FID = DuctPathOrigin.DuctNestTo[n].FID;
                        temp.FNO = DuctPathOrigin.DuctNestTo[n].FNO;
                        temp.styleIDform = DuctPathOrigin.DuctNestTo[n].styleIDform;
                        temp.styleIDlabel = DuctPathOrigin.DuctNestTo[n].styleIDlabel;
                        temp.Form = new List<PolylineGeom>();
                        //formation line
                        for (int i = 0; i < DuctPathOrigin.DuctNestTo[n].Form.Count; i++)
                        {
                            PolylineGeom tt = new PolylineGeom();
                            tt.CID = DuctPathOrigin.DuctNestTo[n].Form[i].CID;
                            tt.CNO = DuctPathOrigin.DuctNestTo[n].Form[i].CNO;
                            IGTPolylineGeometry templine = GTClassFactory.Create<IGTPolylineGeometry>();
                            for (int ll = 0; ll < DuctPathOrigin.DuctNestTo[n].Form[i].geom.Points.Count; ll++)
                            {
                                IGTPoint newpoint = GTClassFactory.Create<IGTPoint>();
                                newpoint.X = DuctPathOrigin.DuctNestTo[n].Form[i].geom.Points[ll].X;
                                newpoint.Y = DuctPathOrigin.DuctNestTo[n].Form[i].geom.Points[ll].Y;
                                newpoint.Z = DuctPathOrigin.DuctNestTo[n].Form[i].geom.Points[ll].Z;
                                templine.Points.Add(newpoint);

                            }
                            tt.geom = templine;// DuctPathOrigin.DuctNestTo[n].Form[i].geom;

                            // tt.geom = DuctPathOrigin.DuctNestTo[n].Form[i].geom;

                            temp.Form.Add(tt);
                        }
                        temp.Labels = new List<TextPointGeom>();
                        // labels
                        for (int i = 0; i < DuctPathOrigin.DuctNestTo[n].Labels.Count; i++)
                        {
                            TextPointGeom tt = new TextPointGeom();
                            tt.CID = DuctPathOrigin.DuctNestTo[n].Labels[i].CID;
                            tt.CNO = DuctPathOrigin.DuctNestTo[n].Labels[i].CNO;
                            IGTTextPointGeometry templab = GTClassFactory.Create<IGTTextPointGeometry>();
                            IGTPoint newpoint = GTClassFactory.Create<IGTPoint>();
                            newpoint.X = DuctPathOrigin.DuctNestTo[n].Labels[i].geom.Origin.X;
                            newpoint.Y = DuctPathOrigin.DuctNestTo[n].Labels[i].geom.Origin.Y;
                            newpoint.Z = DuctPathOrigin.DuctNestTo[n].Labels[i].geom.Origin.Z;
                            templab.Origin = newpoint;
                            templab.Rotation = DuctPathOrigin.DuctNestTo[n].Labels[i].geom.Rotation;
                            templab.Text = DuctPathOrigin.DuctNestTo[n].Labels[i].geom.Text;
                            templab.Alignment = DuctPathOrigin.DuctNestTo[n].Labels[i].geom.Alignment;
                            tt.geom = templab;
                            temp.Labels.Add(tt);
                        }
                        if (DuctPathOrigin.DuctNestTo[n].Ducts != null)
                        {
                            temp.Ducts = new List<Duct>();
                            #region ducts
                            for (int i = 0; i < DuctPathOrigin.DuctNestTo[n].Ducts.Count; i++)
                            {
                                Duct tt = new Duct();
                                tt.FID = DuctPathOrigin.DuctNestTo[n].Ducts[i].FID;
                                tt.FNO = DuctPathOrigin.DuctNestTo[n].Ducts[i].FNO;
                                tt.OwnerFNO = DuctPathOrigin.DuctNestTo[n].Ducts[i].OwnerFNO;
                                tt.OwnerFID = DuctPathOrigin.DuctNestTo[n].Ducts[i].OwnerFID;
                                tt.styleID = DuctPathOrigin.DuctNestTo[n].Ducts[i].styleID;
                                tt.Form = new List<OrientedPointGeom>();
                                for (int j = 0; j < DuctPathOrigin.DuctNestTo[n].Ducts[i].Form.Count; j++)
                                {
                                    OrientedPointGeom ttt = new OrientedPointGeom();
                                    ttt.CID = DuctPathOrigin.DuctNestTo[n].Ducts[i].Form[j].CID;
                                    ttt.CNO = DuctPathOrigin.DuctNestTo[n].Ducts[i].Form[j].CNO;
                                    IGTOrientedPointGeometry tempduct = GTClassFactory.Create<IGTOrientedPointGeometry>();
                                    IGTPoint newpoint = GTClassFactory.Create<IGTPoint>();
                                    newpoint.X = DuctPathOrigin.DuctNestTo[n].Ducts[i].Form[j].geom.Origin.X;
                                    newpoint.Y = DuctPathOrigin.DuctNestTo[n].Ducts[i].Form[j].geom.Origin.Y;
                                    newpoint.Z = DuctPathOrigin.DuctNestTo[n].Ducts[i].Form[j].geom.Origin.Z;
                                    tempduct.Origin = newpoint;

                                    ttt.geom = tempduct;
                                    tt.Form.Add(ttt);
                                }
                                temp.Ducts.Add(tt);
                            }
                            #endregion
                        }
                        DuctpathNew.DuctNestTo.Add(temp);
                    }
                }
                #endregion

                #endregion

                DuctpathNew.DuctPathLineGeom = geomNew;
                DuctpathNew.Sections=new List<DuctPathSect>();
                int length = 0;

               
                for (int i = 0; i < DuctPathOrigin.Sections.Count; i++)
                {
                    length += int.Parse(DuctPathOrigin.Sections[i].SectionLength);
                    if (length <= BreakSlash.length)
                    {
                        DuctPathSect newnewsec = new DuctPathSect();
                        newnewsec.CID = DuctPathOrigin.Sections[i].CID;
                        newnewsec.Encasement = DuctPathOrigin.Sections[i].Encasement;
                        newnewsec.NumDuctWaysSect = DuctPathOrigin.Sections[i].NumDuctWaysSect;
                        newnewsec.PUSect = DuctPathOrigin.Sections[i].PUSect;
                        newnewsec.SectBackFill = DuctPathOrigin.Sections[i].SectBackFill;
                        newnewsec.SectBillingRate = DuctPathOrigin.Sections[i].SectBillingRate;
                        newnewsec.SectDiam = DuctPathOrigin.Sections[i].SectDiam;
                        newnewsec.SectGraphicLength = DuctPathOrigin.Sections[i].SectGraphicLength;
                        newnewsec.SectionLength = DuctPathOrigin.Sections[i].SectionLength;
                        newnewsec.SectOwner = DuctPathOrigin.Sections[i].SectOwner;
                        newnewsec.SectPlc = DuctPathOrigin.Sections[i].SectPlc;
                        newnewsec.SectType = DuctPathOrigin.Sections[i].SectType;
                        newnewsec.YearExpanded = DuctPathOrigin.Sections[i].YearExpanded;
                        newnewsec.YearExtended = DuctPathOrigin.Sections[i].YearExtended;
                        DuctpathNew.Sections.Add(newnewsec);
                    }

                    else if (length == BreakSlash.length)
                        break;
                    else
                    {
                        DuctPathSect newnewsec = new DuctPathSect();
                        newnewsec.CID = DuctPathOrigin.Sections[i].CID;
                        newnewsec.Encasement = DuctPathOrigin.Sections[i].Encasement;
                        newnewsec.NumDuctWaysSect = DuctPathOrigin.Sections[i].NumDuctWaysSect;
                        newnewsec.PUSect = DuctPathOrigin.Sections[i].PUSect;
                        newnewsec.SectBackFill = DuctPathOrigin.Sections[i].SectBackFill;
                        newnewsec.SectBillingRate = DuctPathOrigin.Sections[i].SectBillingRate;
                        newnewsec.SectDiam = DuctPathOrigin.Sections[i].SectDiam;
                        newnewsec.SectGraphicLength = DuctPathOrigin.Sections[i].SectGraphicLength;
                        newnewsec.SectionLength = DuctPathOrigin.Sections[i].SectionLength;
                        newnewsec.SectOwner = DuctPathOrigin.Sections[i].SectOwner;
                        newnewsec.SectPlc = DuctPathOrigin.Sections[i].SectPlc;
                        newnewsec.SectType = DuctPathOrigin.Sections[i].SectType;
                        newnewsec.YearExpanded = DuctPathOrigin.Sections[i].YearExpanded;
                        newnewsec.YearExtended = DuctPathOrigin.Sections[i].YearExtended;
                        DuctpathNew.Sections.Add(newnewsec);
                        break;
                    }               

                }
                length-=int.Parse(DuctpathNew.Sections[DuctpathNew.Sections.Count-1].SectionLength);
                int lengthnew = LengthConduit(geomNew);
                DuctpathNew.Length = lengthnew;
                lengthnew-=length;
                DuctpathNew.Sections[DuctpathNew.Sections.Count - 1].SectionLength = lengthnew.ToString();
                
                if (DuctPathOrigin.SectLabels != null)
                    if (DuctPathOrigin.SectLabels.Count > 0)
                    {

                        DuctpathNew.SectLabels = new List<SectLabelLeaderLine>();
                        for (int i = 0; i < DuctpathNew.Sections.Count; i++)
                        {
                            SectLabelLeaderLine newnewsec = new SectLabelLeaderLine();
                            newnewsec.CID = DuctPathOrigin.SectLabels[i].CID;
                            newnewsec.Label = DuctPathOrigin.SectLabels[i].Label;
                            newnewsec.LabelAlight = DuctPathOrigin.SectLabels[i].LabelAlight;
                            newnewsec.LabelText = DuctPathOrigin.SectLabels[i].LabelText;
                            newnewsec.LeaderLine = DuctPathOrigin.SectLabels[i].LeaderLine;
                            DuctpathNew.SectLabels.Add(newnewsec);
                        }
                    }
               if (DuctPathOrigin.SectSlashes != null)
                    if (DuctPathOrigin.SectSlashes.Count > 0)
                    {
                        DuctpathNew.SectSlashes = new List<SectSlash>();                
                        for (int i = 0; i < DuctpathNew.Sections.Count - 1; i++)
                        {
                            SectSlash newnewsec = new SectSlash();
                            newnewsec.CID = DuctPathOrigin.SectSlashes[i].CID;
                            newnewsec.length = DuctPathOrigin.SectSlashes[i].length;
                            newnewsec.Slash = DuctPathOrigin.SectSlashes[i].Slash;
                            DuctpathNew.SectSlashes.Add(newnewsec);
                        }
                    }
                DuctPathFrom = DuctpathNew;
                if (mobjEditServiceDuctPathFrom.GeometryCount > 0)
                {
                    mobjEditServiceDuctPathFrom.RemoveGeometry(mobjEditServiceDuctPathFrom.GeometryCount);
                }
                mobjEditServiceDuctPathFrom.AddGeometry(DuctPathFrom.DuctPathLineGeom, 12000);
                string content = DuctPathFrom.SectLabels[DuctPathFrom.SectLabels.Count - 1].LabelText.Trim();
                int l = content.IndexOf("\n");
                if (l > 0)
                {
                    DuctPathFrom.SectLabels[DuctPathFrom.SectLabels.Count - 1].LabelText=content.Substring(0, l);
                    DuctPathFrom.SectLabels[DuctPathFrom.SectLabels.Count - 1].LabelText += "\n" +
                        DuctPathFrom.Sections[DuctPathFrom.Sections.Count - 1].SectionLength + " m";
                }
                DuctPathFrom.SectLabels[DuctPathFrom.SectLabels.Count - 1].Label.Text =
                DuctPathFrom.SectLabels[DuctPathFrom.SectLabels.Count - 1].LabelText;
                //DuctPathFrom.SectLabels[DuctPathFrom.SectLabels.Count - 1].Label.Origin.X = DuctPathFrom.DuctPathLineGeom.Points[0].X;
                //DuctPathFrom.SectLabels[DuctPathFrom.SectLabels.Count - 1].Label.Origin.Y = DuctPathFrom.DuctPathLineGeom.Points[0].Y;
                mobjEditServiceDuctPathFrom.AddGeometry(DuctPathFrom.SectLabels[DuctPathFrom.SectLabels.Count - 1].Label, 32400);
                DuctPathFrom.termFID = iManholeFID;
                DuctPathFrom.termFNO = iManholeFNO;
                DuctPathFrom.termWall = int.Parse(wallnum);
                DuctPathFrom.termType = "MANHOLE";
                
               LocateFeature(2, m_gtapp.ActiveMapWindow, DuctPathFrom.sourceFNO, DuctPathFrom.sourceFID, DuctPathFrom.termFNO, DuctPathFrom.termFID);
                                
                    step = 6;//label placement
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }
        #endregion

         #region ReOwnWallTo
        public bool ReOwnWallTo(IGTPolylineGeometry geomNew, string wallnum)
        {
            try
            {
                DuctPath DuctpathNew = new DuctPath();
                #region copy attr
                DuctpathNew.BillingRate = DuctPathOrigin.BillingRate;
                DuctpathNew.ConstructBy = DuctPathOrigin.ConstructBy;
                DuctpathNew.DBFlag = DuctPathOrigin.DBFlag;
                DuctpathNew.DuctWay = DuctPathOrigin.DuctWay;
                DuctpathNew.EXC_ABB = DuctPathOrigin.EXC_ABB;
                DuctpathNew.Feature_state = DuctPathOrigin.Feature_state;
                DuctpathNew.FID = DuctPathOrigin.FID;
                DuctpathNew.FNO = DuctPathOrigin.FNO;
                DuctpathNew.InstallYear = DuctPathOrigin.InstallYear;
                DuctpathNew.sourceFID = DuctPathOrigin.sourceFID;
                DuctpathNew.sourceFNO = DuctPathOrigin.sourceFNO;
                DuctpathNew.sourceType = DuctPathOrigin.sourceType;
                DuctpathNew.sourceWall = DuctPathOrigin.sourceWall;
                DuctpathNew.termFID = DuctPathOrigin.termFID;
                DuctpathNew.termFNO = DuctPathOrigin.termFNO;
                DuctpathNew.termType = DuctPathOrigin.termType;
                DuctpathNew.termWall = DuctPathOrigin.termWall;
                DuctpathNew.ASb_SubDuct = DuctPathOrigin.ASb_SubDuct;
                DuctpathNew.Prop_InnDuct = DuctPathOrigin.Prop_InnDuct;
                DuctpathNew.Prop_SubDuct = DuctPathOrigin.Prop_SubDuct;
                DuctpathNew.NestAssignDuctWay = DuctPathOrigin.NestAssignDuctWay;
                DuctpathNew.Description = DuctPathOrigin.Description;
                DuctpathNew.DuctBalance = DuctPathOrigin.DuctBalance;
                DuctpathNew.Feature_type = DuctPathOrigin.Feature_type;
                DuctpathNew.ExpandFlag = DuctPathOrigin.ExpandFlag;
                //duct nest from  
                #region duct nest from
                if (DuctPathOrigin.DuctNestFrom != null)
                {
                    DuctpathNew.DuctNestFrom = new List<DuctNest>();
                    for (int n = 0; n < DuctPathOrigin.DuctNestFrom.Count; n++)
                    {

                        DuctNest temp = new DuctNest();
                        temp.FID = DuctPathOrigin.DuctNestFrom[n].FID;
                        temp.FNO = DuctPathOrigin.DuctNestFrom[n].FNO;
                        temp.styleIDform = DuctPathOrigin.DuctNestFrom[n].styleIDform;
                        temp.styleIDlabel = DuctPathOrigin.DuctNestFrom[n].styleIDlabel;
                        temp.Form = new List<PolylineGeom>();
                        //formation line
                        for (int i = 0; i < DuctPathOrigin.DuctNestFrom[n].Form.Count; i++)
                        {
                            PolylineGeom tt = new PolylineGeom();
                            tt.CID = DuctPathOrigin.DuctNestFrom[n].Form[i].CID;
                            tt.CNO = DuctPathOrigin.DuctNestFrom[n].Form[i].CNO;
                            IGTPolylineGeometry templine = GTClassFactory.Create<IGTPolylineGeometry>();
                            for (int ll = 0; ll < DuctPathOrigin.DuctNestFrom[n].Form[i].geom.Points.Count; ll++)
                            {
                                IGTPoint newpoint = GTClassFactory.Create<IGTPoint>();
                                newpoint.X = DuctPathOrigin.DuctNestFrom[n].Form[i].geom.Points[ll].X;
                                newpoint.Y = DuctPathOrigin.DuctNestFrom[n].Form[i].geom.Points[ll].Y;
                                newpoint.Z = DuctPathOrigin.DuctNestFrom[n].Form[i].geom.Points[ll].Z;
                                templine.Points.Add(newpoint);

                            }
                            tt.geom = templine;

                            temp.Form.Add(tt);
                        }
                        temp.Labels = new List<TextPointGeom>();
                        // labels
                        for (int i = 0; i < DuctPathOrigin.DuctNestFrom[n].Labels.Count; i++)
                        {
                            TextPointGeom tt = new TextPointGeom();
                            tt.CID = DuctPathOrigin.DuctNestFrom[n].Labels[i].CID;
                            tt.CNO = DuctPathOrigin.DuctNestFrom[n].Labels[i].CNO;
                            IGTTextPointGeometry templab = GTClassFactory.Create<IGTTextPointGeometry>();
                            IGTPoint newpoint = GTClassFactory.Create<IGTPoint>();
                            newpoint.X = DuctPathOrigin.DuctNestFrom[n].Labels[i].geom.Origin.X;
                            newpoint.Y = DuctPathOrigin.DuctNestFrom[n].Labels[i].geom.Origin.Y;
                            newpoint.Z = DuctPathOrigin.DuctNestFrom[n].Labels[i].geom.Origin.Z;
                            templab.Origin = newpoint;
                            templab.Rotation = DuctPathOrigin.DuctNestFrom[n].Labels[i].geom.Rotation;
                            templab.Text = DuctPathOrigin.DuctNestFrom[n].Labels[i].geom.Text;
                            templab.Alignment = DuctPathOrigin.DuctNestFrom[n].Labels[i].geom.Alignment;
                            tt.geom = templab;

                            temp.Labels.Add(tt);
                        }
                        if (DuctPathOrigin.DuctNestFrom[n].Ducts != null)
                        {
                            temp.Ducts = new List<Duct>();
                            #region ducts
                            for (int i = 0; i < DuctPathOrigin.DuctNestFrom[n].Ducts.Count; i++)
                            {
                                Duct tt = new Duct();
                                tt.FID = DuctPathOrigin.DuctNestFrom[n].Ducts[i].FID;
                                tt.FNO = DuctPathOrigin.DuctNestFrom[n].Ducts[i].FNO; 
                                tt.OwnerFNO = DuctPathOrigin.DuctNestFrom[n].Ducts[i].OwnerFNO;
                                tt.OwnerFID = DuctPathOrigin.DuctNestFrom[n].Ducts[i].OwnerFID;
                               
                                tt.styleID = DuctPathOrigin.DuctNestFrom[n].Ducts[i].styleID;
                                tt.Form = new List<OrientedPointGeom>();
                                for (int j = 0; j < DuctPathOrigin.DuctNestFrom[n].Ducts[i].Form.Count; j++)
                                {
                                    OrientedPointGeom ttt = new OrientedPointGeom();
                                    ttt.CID = DuctPathOrigin.DuctNestFrom[n].Ducts[i].Form[j].CID;
                                    ttt.CNO = DuctPathOrigin.DuctNestFrom[n].Ducts[i].Form[j].CNO;

                                    IGTOrientedPointGeometry tempduct = GTClassFactory.Create<IGTOrientedPointGeometry>();
                                    IGTPoint newpoint = GTClassFactory.Create<IGTPoint>();
                                    newpoint.X = DuctPathOrigin.DuctNestFrom[n].Ducts[i].Form[j].geom.Origin.X;
                                    newpoint.Y = DuctPathOrigin.DuctNestFrom[n].Ducts[i].Form[j].geom.Origin.Y;
                                    newpoint.Z = DuctPathOrigin.DuctNestFrom[n].Ducts[i].Form[j].geom.Origin.Z;
                                    tempduct.Origin = newpoint;

                                    ttt.geom = tempduct;// DuctPathOrigin.DuctNestFrom[n].Ducts[i].Form[j].geom;
                                    tt.Form.Add(ttt);
                                }
                                temp.Ducts.Add(tt);
                            }
                            #endregion
                        }
                        DuctpathNew.DuctNestFrom.Add(temp);
                    }
                }
                #endregion

                //duct nest to    
                #region duct nest to
                if (DuctPathOrigin.DuctNestTo != null)
                {
                    DuctpathNew.DuctNestTo = new List<DuctNest>();
                    for (int n = 0; n < DuctPathOrigin.DuctNestTo.Count; n++)
                    {


                        DuctNest temp = new DuctNest();
                        temp.FID = DuctPathOrigin.DuctNestTo[n].FID;
                        temp.FNO = DuctPathOrigin.DuctNestTo[n].FNO;
                        temp.styleIDform = DuctPathOrigin.DuctNestTo[n].styleIDform;
                        temp.styleIDlabel = DuctPathOrigin.DuctNestTo[n].styleIDlabel;
                        temp.Form = new List<PolylineGeom>();
                        //formation line
                        for (int i = 0; i < DuctPathOrigin.DuctNestTo[n].Form.Count; i++)
                        {
                            PolylineGeom tt = new PolylineGeom();
                            tt.CID = DuctPathOrigin.DuctNestTo[n].Form[i].CID;
                            tt.CNO = DuctPathOrigin.DuctNestTo[n].Form[i].CNO;
                            IGTPolylineGeometry templine = GTClassFactory.Create<IGTPolylineGeometry>();
                            for (int ll = 0; ll < DuctPathOrigin.DuctNestTo[n].Form[i].geom.Points.Count; ll++)
                            {
                                IGTPoint newpoint = GTClassFactory.Create<IGTPoint>();
                                newpoint.X = DuctPathOrigin.DuctNestTo[n].Form[i].geom.Points[ll].X;
                                newpoint.Y = DuctPathOrigin.DuctNestTo[n].Form[i].geom.Points[ll].Y;
                                newpoint.Z = DuctPathOrigin.DuctNestTo[n].Form[i].geom.Points[ll].Z;
                                templine.Points.Add(newpoint);

                            }
                            tt.geom = templine;// DuctPathOrigin.DuctNestTo[n].Form[i].geom;

                            // tt.geom = DuctPathOrigin.DuctNestTo[n].Form[i].geom;

                            temp.Form.Add(tt);
                        }
                        temp.Labels = new List<TextPointGeom>();
                        // labels
                        for (int i = 0; i < DuctPathOrigin.DuctNestTo[n].Labels.Count; i++)
                        {
                            TextPointGeom tt = new TextPointGeom();
                            tt.CID = DuctPathOrigin.DuctNestTo[n].Labels[i].CID;
                            tt.CNO = DuctPathOrigin.DuctNestTo[n].Labels[i].CNO;
                            IGTTextPointGeometry templab = GTClassFactory.Create<IGTTextPointGeometry>();
                            IGTPoint newpoint = GTClassFactory.Create<IGTPoint>();
                            newpoint.X = DuctPathOrigin.DuctNestTo[n].Labels[i].geom.Origin.X;
                            newpoint.Y = DuctPathOrigin.DuctNestTo[n].Labels[i].geom.Origin.Y;
                            newpoint.Z = DuctPathOrigin.DuctNestTo[n].Labels[i].geom.Origin.Z;
                            templab.Origin = newpoint;
                            templab.Rotation = DuctPathOrigin.DuctNestTo[n].Labels[i].geom.Rotation;
                            templab.Text = DuctPathOrigin.DuctNestTo[n].Labels[i].geom.Text;
                            templab.Alignment = DuctPathOrigin.DuctNestTo[n].Labels[i].geom.Alignment;
                            tt.geom = templab;
                            temp.Labels.Add(tt);
                        }
                        if (DuctPathOrigin.DuctNestTo[n].Ducts != null)
                        {
                            temp.Ducts = new List<Duct>();
                            #region ducts
                            for (int i = 0; i < DuctPathOrigin.DuctNestTo[n].Ducts.Count; i++)
                            {
                                Duct tt = new Duct();
                                tt.FID = DuctPathOrigin.DuctNestTo[n].Ducts[i].FID;
                                tt.FNO = DuctPathOrigin.DuctNestTo[n].Ducts[i].FNO;
                                tt.OwnerFNO = DuctPathOrigin.DuctNestTo[n].Ducts[i].OwnerFNO;
                                tt.OwnerFID = DuctPathOrigin.DuctNestTo[n].Ducts[i].OwnerFID;
                               
                                tt.styleID = DuctPathOrigin.DuctNestTo[n].Ducts[i].styleID;
                                tt.Form = new List<OrientedPointGeom>();
                                for (int j = 0; j < DuctPathOrigin.DuctNestTo[n].Ducts[i].Form.Count; j++)
                                {
                                    OrientedPointGeom ttt = new OrientedPointGeom();
                                    ttt.CID = DuctPathOrigin.DuctNestTo[n].Ducts[i].Form[j].CID;
                                    ttt.CNO = DuctPathOrigin.DuctNestTo[n].Ducts[i].Form[j].CNO;
                                    IGTOrientedPointGeometry tempduct = GTClassFactory.Create<IGTOrientedPointGeometry>();
                                    IGTPoint newpoint = GTClassFactory.Create<IGTPoint>();
                                    newpoint.X = DuctPathOrigin.DuctNestTo[n].Ducts[i].Form[j].geom.Origin.X;
                                    newpoint.Y = DuctPathOrigin.DuctNestTo[n].Ducts[i].Form[j].geom.Origin.Y;
                                    newpoint.Z = DuctPathOrigin.DuctNestTo[n].Ducts[i].Form[j].geom.Origin.Z;
                                    tempduct.Origin = newpoint;

                                    ttt.geom = tempduct;
                                    tt.Form.Add(ttt);
                                }
                                temp.Ducts.Add(tt);
                            }
                            #endregion
                        }
                        DuctpathNew.DuctNestTo.Add(temp);
                    }
                }
                #endregion

                #endregion
                DuctpathNew.DuctPathLineGeom = geomNew;
                DuctpathNew.Sections=new List<DuctPathSect>();
                int length = 0;
                int sectstartnum = 0;

                for (int i = 0; i < DuctPathOrigin.Sections.Count; i++)
                {
                    length += int.Parse(DuctPathOrigin.Sections[i].SectionLength);
                    if (length == BreakSlash.length)
                    {
                        sectstartnum = i+1;
                        break;
                    }
                    else if (length > BreakSlash.length)
                    {
                        sectstartnum = i;
                        break;
                    }
                }

                length = 0;
                for (int i = sectstartnum; i < DuctPathOrigin.Sections.Count; i++)
                {
                    DuctPathSect newnewsec = new DuctPathSect();
                    newnewsec.CID = DuctPathOrigin.Sections[i].CID;
                    newnewsec.Encasement = DuctPathOrigin.Sections[i].Encasement;
                    newnewsec.NumDuctWaysSect = DuctPathOrigin.Sections[i].NumDuctWaysSect;
                    newnewsec.PUSect = DuctPathOrigin.Sections[i].PUSect;
                    newnewsec.SectBackFill = DuctPathOrigin.Sections[i].SectBackFill;
                    newnewsec.SectBillingRate = DuctPathOrigin.Sections[i].SectBillingRate;
                    newnewsec.SectDiam = DuctPathOrigin.Sections[i].SectDiam;
                    newnewsec.SectGraphicLength = DuctPathOrigin.Sections[i].SectGraphicLength;
                    newnewsec.SectionLength = DuctPathOrigin.Sections[i].SectionLength;
                    newnewsec.SectOwner = DuctPathOrigin.Sections[i].SectOwner;
                    newnewsec.SectPlc = DuctPathOrigin.Sections[i].SectPlc;
                    newnewsec.SectType = DuctPathOrigin.Sections[i].SectType;
                    newnewsec.YearExpanded = DuctPathOrigin.Sections[i].YearExpanded;
                    newnewsec.YearExtended = DuctPathOrigin.Sections[i].YearExtended;
                    DuctpathNew.Sections.Add(newnewsec);
                    length += int.Parse(DuctPathOrigin.Sections[i].SectionLength);
                }

                length -= int.Parse(DuctpathNew.Sections[0].SectionLength);
                int lengthnew = LengthConduit(geomNew);
                DuctpathNew.Length = lengthnew;
                lengthnew -= length;
                DuctpathNew.Sections[0].SectionLength = lengthnew.ToString();
                if (DuctPathOrigin.SectLabels != null)
                    if (DuctPathOrigin.SectLabels.Count > 0)
                    {

                        DuctpathNew.SectLabels = new List<SectLabelLeaderLine>();
                        for (int i = sectstartnum; i < DuctpathNew.Sections.Count; i++)
                        {
                            SectLabelLeaderLine newnewsec = new SectLabelLeaderLine();
                            newnewsec.CID = DuctPathOrigin.SectLabels[i].CID;
                            newnewsec.Label = DuctPathOrigin.SectLabels[i].Label;
                            newnewsec.LabelAlight = DuctPathOrigin.SectLabels[i].LabelAlight;
                            newnewsec.LabelText = DuctPathOrigin.SectLabels[i].LabelText;
                            newnewsec.LeaderLine = DuctPathOrigin.SectLabels[i].LeaderLine;
                            DuctpathNew.SectLabels.Add(newnewsec);
                        }
                    }
                
                if (DuctPathOrigin.SectSlashes != null)
                    if (DuctPathOrigin.SectSlashes.Count > 0)
                    {
                        DuctpathNew.SectSlashes = new List<SectSlash>();
                        for (int i = sectstartnum; i < DuctpathNew.Sections.Count - 1; i++)
                        {
                            SectSlash newnewsec = new SectSlash();
                            newnewsec.CID = DuctPathOrigin.SectSlashes[i].CID;
                            newnewsec.length = DuctPathOrigin.SectSlashes[i].length;
                            newnewsec.Slash = DuctPathOrigin.SectSlashes[i].Slash;
                            DuctpathNew.SectSlashes.Add(newnewsec);
                        }
                    }
                DuctPathTo = DuctpathNew;
                
                if (mobjEditServiceDuctPathTo.GeometryCount > 0)
                {
                    mobjEditServiceDuctPathTo.RemoveGeometry(mobjEditServiceDuctPathTo.GeometryCount);
                }
                mobjEditServiceDuctPathTo.AddGeometry(DuctPathTo.DuctPathLineGeom, 12000);
                string content = DuctPathTo.SectLabels[0].LabelText.Trim();
                int l = content.IndexOf("\n");
                if (l > 0)
                {
                    DuctPathTo.SectLabels[0].LabelText = content.Substring(0, l);
                    DuctPathTo.SectLabels[0].LabelText += "\n" + DuctPathTo.Sections[0].SectionLength + " m";
                }
                DuctPathTo.SectLabels[0].Label.Text = DuctPathTo.SectLabels[0].LabelText;

                //DuctPathTo.SectLabels[0].Label.Origin.X = DuctPathTo.DuctPathLineGeom.Points[0].X;
                //DuctPathTo.SectLabels[0].Label.Origin.Y = DuctPathTo.DuctPathLineGeom.Points[0].Y;
                mobjEditServiceDuctPathTo.AddGeometry(DuctPathTo.SectLabels[0].Label, 32400);
                DuctPathTo.sourceFID = iManholeFID;
                DuctPathTo.sourceFNO = iManholeFNO;
                DuctPathTo.sourceWall = int.Parse(wallnum);
                DuctPathTo.sourceType="MANHOLE";
               LocateFeature(2, m_gtapp.ActiveMapWindow, DuctPathTo.sourceFNO, DuctPathTo.sourceFID, DuctPathTo.termFNO, DuctPathTo.termFID);
                
                step = 60;//label placement
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }
        #endregion
        #endregion

        #region Update Geometry for Duct Nest
        public DuctNest UpdateGeomDuctNest(DuctNest dnest, bool from)
        {
            try
            {
               
                int countgeom = 0;
               
                #region formation line
                for (int i = 0; i < dnest.Form.Count; i++)
                {
                    countgeom++;
                    if(!from)
                    dnest.Form[i].geom = (IGTPolylineGeometry)mobjEditServiceRotate3.GetGeometry(countgeom);
                    else dnest.Form[i].geom = (IGTPolylineGeometry)mobjEditServiceRotate3.GetGeometry(countgeom);
                }
                #endregion
                #region labels
                for (int i = 0; i < dnest.Labels.Count; i++)
                {
                
                    countgeom++;
                    if (!from)
                    dnest.Labels[i].geom = (IGTTextPointGeometry)mobjEditServiceRotate3.GetGeometry(countgeom);
                else dnest.Labels[i].geom = (IGTTextPointGeometry)mobjEditServiceRotate3.GetGeometry(countgeom);
                }
                #endregion
               
                #region ducts
                for (int i = 0; i < dnest.Ducts.Count; i++)
                {
                    for (int j = 0; j < dnest.Ducts[i].Form.Count; j++)
                    {
                       

                        countgeom++;
                        if (!from)
                        dnest.Ducts[i].Form[j].geom = (IGTOrientedPointGeometry)mobjEditServiceRotate3.GetGeometry(countgeom);
                    else
                        dnest.Ducts[i].Form[j].geom = (IGTOrientedPointGeometry)mobjEditServiceRotate3.GetGeometry(countgeom);
                      
                    }
                }
                #endregion

            }
            catch (Exception ex)
            {
                return null;
            }
            return dnest;
        }

        public bool UpdateGeomDuctNest(DuctNest dnest)
        {
            try
            {
                short iFNO = dnest.FNO;
                int iFID = dnest.FID;
                short iCNO = 0;// GetCno(iFNO, from);
                int iCID = 0;
                IGTKeyObject oNewFeature=  m_IGTDataContext.OpenFeature(iFNO, iFID);
                #region update exc_abb
                iCNO = 51;
                if (!oNewFeature.Components.GetComponent(iCNO).Recordset.EOF)
                {
                    oNewFeature.Components.GetComponent(iCNO).Recordset.Update("EXC_ABB", Get_Value("select EXC_ABB from g3e_job where g3e_identifier ='" + m_IGTDataContext.ActiveJob + "'"));
                }
                #endregion
                #region formation line
                for (int i = 0; i < dnest.Form.Count; i++)
                {

                    iCNO = dnest.Form[i].CNO;
                    iCID = dnest.Form[i].CID;
                    oNewFeature.Components.GetComponent(iCNO).Recordset.MoveFirst();
                    for (int j = 0; j < oNewFeature.Components.GetComponent(iCNO).Recordset.RecordCount; j++)
                    {

                        for (int f = 0; f < oNewFeature.Components.GetComponent(iCNO).Recordset.Fields.Count; f++)
                        {
                            if (oNewFeature.Components.GetComponent(iCNO).Recordset.Fields[f].Name == "G3E_CID")
                                if (oNewFeature.Components.GetComponent(iCNO).Recordset.Fields[f].Value.ToString() == iCID.ToString())
                                {
                                    oNewFeature.Components.GetComponent(iCNO).Geometry = dnest.Form[i].geom;
                                    break;
                                }
                        }

                        oNewFeature.Components.GetComponent(iCNO).Recordset.MoveNext();
                    }
                }

                #endregion
                #region labels
                for (int i = 0; i < dnest.Labels.Count; i++)
                {
                    iCNO = dnest.Labels[i].CNO;
                    iCID = dnest.Labels[i].CID;
                    oNewFeature.Components.GetComponent(iCNO).Recordset.MoveFirst();
                    for (int j = 0; j < oNewFeature.Components.GetComponent(iCNO).Recordset.RecordCount; j++)
                    {

                        for (int f = 0; f < oNewFeature.Components.GetComponent(iCNO).Recordset.Fields.Count; f++)
                        {
                            if (oNewFeature.Components.GetComponent(iCNO).Recordset.Fields[f].Name == "G3E_CID")
                                if (oNewFeature.Components.GetComponent(iCNO).Recordset.Fields[f].Value.ToString() == iCID.ToString())
                                {
                                    oNewFeature.Components.GetComponent(iCNO).Geometry = dnest.Labels[i].geom;
                                    break;
                                }
                        }

                        oNewFeature.Components.GetComponent(iCNO).Recordset.MoveNext();
                    }
                   
                }

                //update label for from label with new manhole id
                iCNO = 2430;
                
                oNewFeature.Components.GetComponent(iCNO).Recordset.MoveFirst();
                for (int j = 0; j < oNewFeature.Components.GetComponent(iCNO).Recordset.RecordCount; j++)
                {
                    string cid = "1";
                    for (int f = 0; f < oNewFeature.Components.GetComponent(iCNO).Recordset.Fields.Count; f++)
                    {
                        if (oNewFeature.Components.GetComponent(iCNO).Recordset.Fields[f].Name == "G3E_CID")
                        {
                            cid = oNewFeature.Components.GetComponent(iCNO).Recordset.Fields[f].Value.ToString();
                            break;
                        }
                    }

                    string textto = Get_Value("select g3e_text from GC_FORMFROM_T where g3e_cid=" + cid.ToString() + " and g3e_fno=2400 and g3e_fid=" + dnest.FID);
                    int ind = textto.IndexOf(']');
                    if (ind > 0)
                    {
                        textto = textto.Substring(0, ind + 1);
                        textto += " " + Get_Value("select MANHOLE_ID from GC_MANHL where g3e_fid = " + DuctPathFrom.termFID);
                        oNewFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_TEXT", textto);
                        ind = 0;
                        break;
                    }
                    oNewFeature.Components.GetComponent(iCNO).Recordset.MoveNext();
                }
                #endregion
                #region ducts


                for (int i = 0; i < dnest.Ducts.Count; i++)
                {
                    iFNO = dnest.Ducts[i].FNO;
                    iFID = dnest.Ducts[i].FID;
                    IGTKeyObject oDuct = m_IGTDataContext.OpenFeature(iFNO, iFID);
                    
                    for (int k = 0; k < dnest.Ducts[i].Form.Count; k++)
                    {
                        #region update exc_abb
                        iCNO = 51;
                        if (!oDuct.Components.GetComponent(iCNO).Recordset.EOF)
                        {
                            oDuct.Components.GetComponent(iCNO).Recordset.Update("EXC_ABB", Get_Value("select EXC_ABB from g3e_job where g3e_identifier ='" + m_IGTDataContext.ActiveJob + "'"));
                        }
                        #endregion

                        iCNO = dnest.Ducts[i].Form[k].CNO;
                        iCID = dnest.Ducts[i].Form[k].CID;
                        oDuct.Components.GetComponent(iCNO).Recordset.MoveFirst();
                        for (int j = 0; j < oDuct.Components.GetComponent(iCNO).Recordset.RecordCount; j++)
                        {

                            for (int f = 0; f < oDuct.Components.GetComponent(iCNO).Recordset.Fields.Count; f++)
                            {
                                if (oDuct.Components.GetComponent(iCNO).Recordset.Fields[f].Name == "G3E_CID")
                                    if (oDuct.Components.GetComponent(iCNO).Recordset.Fields[f].Value.ToString() == iCID.ToString())
                                    {
                                        oDuct.Components.GetComponent(iCNO).Geometry = dnest.Ducts[i].Form[k].geom;
                                        break;
                                    }
                            }

                          

                            oDuct.Components.GetComponent(iCNO).Recordset.MoveNext();
                        }
                        
                    }

                    //string Sql1 = "delete from GC_CONTAIN where g3e_fno=" + oDuct.FNO + " and g3e_fid=" + oDuct.FID +
                    //"and g3e_ownerfno=" + dnest.Ducts[i].OwnerFNO + " and g3e_ownerfid=" + dnest.Ducts[i].OwnerFID + "";
                    //int roweff1 = 0;
                    //m_IGTDataContext.Execute(Sql1, out roweff1, -1);

                    ////Ownership        
                    //mobjRelationshipService.ActiveFeature = m_IGTDataContext.OpenFeature(dnest.Ducts[i].OwnerFNO, dnest.Ducts[i].OwnerFID); ;
                    //if (mobjRelationshipService.AllowSilentEstablish(oDuct))
                    //{
                    //    mobjRelationshipService.SilentEstablish(7, oDuct);
                    //}
                }
                #endregion
                #region relationship with duct path
               
                //Ownership    
                //IGTKeyObject DuctPathFromFeat = m_IGTDataContext.OpenFeature(DuctPathFrom.FNO, DuctPathFrom.FID);

                //string Sql = "delete from GC_CONTAIN where g3e_fno=" + oNewFeature.FNO + " and g3e_fid=" + oNewFeature.FID +
                //"and g3e_ownerfno=" + DuctPathFromFeat.FNO + " and g3e_ownerfid=" + DuctPathFromFeat.FID + "";
                //int roweff = 0;
                //m_IGTDataContext.Execute(Sql, out roweff, -1);

                //mobjRelationshipService.ActiveFeature = DuctPathFromFeat;
                //if (mobjRelationshipService.AllowSilentEstablish(oNewFeature))
                //{
                //    mobjRelationshipService.SilentEstablish(7, oNewFeature);
                //}

               
                #endregion

            }
            catch (Exception ex)
            {
                
                return false;
            }
            return true;
        }
        #endregion

        #region BreakDuctPath
        public bool BreakDuctPath()
        {
            Messages msg = new Messages();
           
            try
            {
               
                msg.Message(3);
                msg.Show();
                m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Wait, Breaking Duct Path in process ... ");

                m_gtapp.BeginProgressBar();
                m_gtapp.SetProgressBarRange(0, 100);
                m_gtapp.SetProgressBarPosition(5);
                m_oIGTTransactionManager.Begin("BreakDuctPath");
                

                short iFNO = 0;
                int iFID = 0;
                short iCNO = 0;// GetCno(iFNO, from);
                int iCID = 0;
                int countgeom = 0;
                
                #region update exisitng duct path
                IGTKeyObject DPFROM = m_IGTDataContext.OpenFeature(DuctPathFrom.FNO, DuctPathFrom.FID);
                iFNO = DuctPathFrom.FNO;
                iFID = DuctPathFrom.FID;
                #region Attributes
                iCNO = 2201;
                if (DPFROM.Components.GetComponent(iCNO).Recordset.EOF)
                {
                    DPFROM.Components.GetComponent(iCNO).Recordset.AddNew("G3E_FID", iFID);
                    DPFROM.Components.GetComponent(iCNO).Recordset.Update("G3E_FNO", iFNO);
                    DPFROM.Components.GetComponent(iCNO).Recordset.Update("G3E_CID", 1);
                    DPFROM.Components.GetComponent(iCNO).Recordset.Update("DT_ND_TO_ID", DuctPathFrom.termFID);
                    DPFROM.Components.GetComponent(iCNO).Recordset.Update("DT_ND_TO_TY", DuctPathFrom.termType);
                    DPFROM.Components.GetComponent(iCNO).Recordset.Update("DT_MH_TO_WALL", DuctPathFrom.sourceWall);
                    DPFROM.Components.GetComponent(iCNO).Recordset.Update("TOTAL_LENGTH", DuctPathFrom.Length);
                   
                }
                else
                {
                    DPFROM.Components.GetComponent(iCNO).Recordset.MoveLast();
                    DPFROM.Components.GetComponent(iCNO).Recordset.Update("G3E_FNO", iFNO);
                    DPFROM.Components.GetComponent(iCNO).Recordset.Update("G3E_CID", 1);
                    DPFROM.Components.GetComponent(iCNO).Recordset.Update("DT_ND_TO_ID", DuctPathFrom.termFID);
                    DPFROM.Components.GetComponent(iCNO).Recordset.Update("DT_ND_TO_TY", DuctPathFrom.termType);
                    DPFROM.Components.GetComponent(iCNO).Recordset.Update("DT_MH_TO_WALL", DuctPathFrom.sourceWall);
                    DPFROM.Components.GetComponent(iCNO).Recordset.Update("TOTAL_LENGTH", DuctPathFrom.Length);
                }
                #endregion
                m_gtapp.SetProgressBarPosition(15);
                #region Line Graphic
                iCNO = 2210;
                if (DPFROM.Components.GetComponent(iCNO).Recordset.EOF)
                {
                    DPFROM.Components.GetComponent(iCNO).Recordset.AddNew("G3E_FID", iFID);
                    DPFROM.Components.GetComponent(iCNO).Recordset.Update("G3E_FNO", iFNO);
                    DPFROM.Components.GetComponent(iCNO).Recordset.Update("G3E_CID", 1);

                }
                else
                {
                    DPFROM.Components.GetComponent(iCNO).Recordset.MoveLast();
                    DPFROM.Components.GetComponent(iCNO).Recordset.Update("G3E_FNO", iFNO);
                    DPFROM.Components.GetComponent(iCNO).Recordset.Update("G3E_CID", 1);
                }
                DPFROM.Components.GetComponent(iCNO).Geometry = DuctPathFrom.DuctPathLineGeom;
                #endregion
                m_gtapp.SetProgressBarPosition(25);
                #region NE Connection to Source or Term devices
                
                IGTKeyObject oTerm = m_IGTDataContext.OpenFeature(DuctPathFrom.termFNO, DuctPathFrom.termFID);
              
                string Sql1 = "update GC_NE_CONNECT set node2_id=0 where g3e_fno=" + DuctPathFrom.FNO + " and g3e_fid=" + DuctPathFrom.FID + " ";
                int roweff1 = 0;
                m_IGTDataContext.Execute(Sql1, out roweff1, -1);

                mobjRelationshipService.ActiveFeature = DPFROM;
                if (mobjRelationshipService.AllowSilentEstablish(oTerm))
                    mobjRelationshipService.SilentEstablish(1, oTerm, GTRelationshipOrdinalConstants.gtrelRelationshipOrdinal2);
                
                #endregion
                m_gtapp.SetProgressBarPosition(35);
                #region Sects
             

                #region Section Attributes
                                      // Section Attributes
                        iCNO = 2202;

                        int roweff = 0;
                
                        string Sql = "delete from gc_condst where g3e_fno=" + iFNO.ToString()+
                             " and g3e_fid=" + iFID.ToString()+" and g3e_cid >" +DuctPathFrom.Sections.Count.ToString();
                                        m_IGTDataContext.Execute(Sql, out roweff, -1);
                        Sql = "delete from gc_condexpst where g3e_fno=" + iFNO.ToString()+
                             " and g3e_fid=" + iFID.ToString()+" and g3e_cid >=" +DuctPathFrom.Sections.Count.ToString();
                                        m_IGTDataContext.Execute(Sql, out roweff, -1);

                
                        DPFROM.Components.GetComponent(iCNO).Recordset.MoveLast();
                        DPFROM.Components.GetComponent(iCNO).Recordset.Update("DT_S_LENGTH", DuctPathFrom.Sections[DuctPathFrom.Sections.Count-1].SectionLength);

                #endregion

                #region Lables for Sections and Leader Lines
                 iCNO = 2230;
                 if (DuctPathFrom.SectLabels != null)
                 {
                     Sql = "delete from GC_COND_T where g3e_fno=" + iFNO.ToString() +
                             " and g3e_fid=" + iFID.ToString() + " and g3e_cid >=" + DuctPathFrom.SectLabels.Count.ToString();
                     m_IGTDataContext.Execute(Sql, out roweff, -1);
                 }
                if (DPFROM.Components.GetComponent(iCNO).Recordset.EOF)
                {
                    DPFROM.Components.GetComponent(iCNO).Recordset.AddNew("G3E_FID", iFID);
                    DPFROM.Components.GetComponent(iCNO).Recordset.Update("G3E_FNO", iFNO);
                    DPFROM.Components.GetComponent(iCNO).Recordset.Update("G3E_CID", DuctPathFrom.SectLabels.Count);
                    DPFROM.Components.GetComponent(iCNO).Recordset.Update("G3E_ALIGNMENT", 0);
                }
                else
                {
                    DPFROM.Components.GetComponent(iCNO).Recordset.MoveLast();
                    DPFROM.Components.GetComponent(iCNO).Recordset.Update("G3E_FNO", iFNO);
                    DPFROM.Components.GetComponent(iCNO).Recordset.Update("G3E_CID", DuctPathFrom.SectLabels.Count);
                    DPFROM.Components.GetComponent(iCNO).Recordset.Update("G3E_ALIGNMENT", 0);
                }
                DPFROM.Components.GetComponent(iCNO).Geometry = DuctPathFrom.SectLabels[DuctPathFrom.SectLabels.Count - 1].Label;

                iCNO = 2212;
                if (DuctPathFrom.SectLabels != null)
                {
                    Sql = "delete from GC_CONDLDR_L where g3e_fno=" + iFNO.ToString() +
                                 " and g3e_fid=" + iFID.ToString() + " and g3e_cid >=" + DuctPathFrom.SectLabels.Count.ToString();
                    m_IGTDataContext.Execute(Sql, out roweff, -1);
                }
                #endregion

                #region Slashes for sections
                iCNO = 2220;
                if (DuctPathFrom.SectSlashes != null)
                {
                    Sql = "delete from GC_CONDSLASH_S where g3e_fno=" + iFNO.ToString() +
                                 " and g3e_fid=" + iFID.ToString() + " and g3e_cid >=" + DuctPathFrom.SectSlashes.Count.ToString();
                    m_IGTDataContext.Execute(Sql, out roweff, -1);
                }
                #endregion
                #endregion
                m_gtapp.SetProgressBarPosition(40);
                //update nests
               // DuctPathFrom.DuctNestFrom
                if (DuctPathFrom.DuctNestTo != null)
                {
                    for (int i = 0; i < DuctPathFrom.DuctNestTo.Count; i++)
                    {
                        UpdateGeomDuctNest(DuctPathFrom.DuctNestTo[i]);
                    }
                }

                #endregion
                 m_gtapp.SetProgressBarPosition(55);

                #region add new duct path
                 IGTKeyObject oNewFeature = m_IGTDataContext.NewFeature(iFNO);
                 iFID = oNewFeature.FID;
                 #region Attributes
                 iCNO = 2201;
                 if (oNewFeature.Components.GetComponent(iCNO).Recordset.EOF)
                 {
                     oNewFeature.Components.GetComponent(iCNO).Recordset.AddNew("G3E_FID", iFID);
                     oNewFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_FNO", iFNO);
                     oNewFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_CID", 1);
                     int flg = 0;
                     if( int.TryParse(DuctPathTo.DBFlag,out flg))
                         oNewFeature.Components.GetComponent(iCNO).Recordset.Update("CENTRALDB_FLAG", flg);
                     if (int.TryParse(DuctPathTo.ExpandFlag, out flg))
                         oNewFeature.Components.GetComponent(iCNO).Recordset.Update("EXPAND_FLAG", flg);
                     oNewFeature.Components.GetComponent(iCNO).Recordset.Update("DT_WAYS", DuctPathTo.DuctWay);
                     oNewFeature.Components.GetComponent(iCNO).Recordset.Update("DT_ND_FRM_ID", DuctPathTo.sourceFID);
                     oNewFeature.Components.GetComponent(iCNO).Recordset.Update("DT_ND_FRM_TY", DuctPathTo.sourceType);
                     oNewFeature.Components.GetComponent(iCNO).Recordset.Update("DT_MH_FRM_WALL", DuctPathTo.sourceWall);
                     oNewFeature.Components.GetComponent(iCNO).Recordset.Update("DT_ND_TO_ID", DuctPathTo.termFID);
                     oNewFeature.Components.GetComponent(iCNO).Recordset.Update("DT_ND_TO_TY", DuctPathTo.termType);
                     oNewFeature.Components.GetComponent(iCNO).Recordset.Update("DT_MH_TO_WALL", DuctPathTo.termWall);
                     oNewFeature.Components.GetComponent(iCNO).Recordset.Update("DT_CONSTRUCTION", DuctPathTo.ConstructBy);
                     oNewFeature.Components.GetComponent(iCNO).Recordset.Update("TOTAL_LENGTH", DuctPathTo.Length);
                     oNewFeature.Components.GetComponent(iCNO).Recordset.Update("DT_NEST_WAYS", DuctPathTo.DuctWay);
                     oNewFeature.Components.GetComponent(iCNO).Recordset.Update("DT_ASB_SUBDUCT", DuctPathTo.ASb_SubDuct);
                     oNewFeature.Components.GetComponent(iCNO).Recordset.Update("DT_PP_SUBDUCT", DuctPathTo.Prop_SubDuct);
                     oNewFeature.Components.GetComponent(iCNO).Recordset.Update("DT_PP_INNDUCT", DuctPathTo.Prop_InnDuct);
                     oNewFeature.Components.GetComponent(iCNO).Recordset.Update("DESCRIPTION", DuctPathTo.Description);
                     oNewFeature.Components.GetComponent(iCNO).Recordset.Update("DT_NEST_WAYS", DuctPathTo.NestAssignDuctWay);
                     oNewFeature.Components.GetComponent(iCNO).Recordset.Update("DUCT_BALANCE", DuctPathTo.DuctBalance);
                     oNewFeature.Components.GetComponent(iCNO).Recordset.Update("FEATURE_TYPE", DuctPathTo.Feature_type);
                 }
                 else
                 {
                     oNewFeature.Components.GetComponent(iCNO).Recordset.MoveLast();
                     oNewFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_FNO", iFNO);
                     oNewFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_CID", 1);
                     int flg = 0;
                     if (int.TryParse(DuctPathTo.DBFlag, out flg))
                         oNewFeature.Components.GetComponent(iCNO).Recordset.Update("CENTRALDB_FLAG", flg);
                     if (int.TryParse(DuctPathTo.ExpandFlag, out flg))
                         oNewFeature.Components.GetComponent(iCNO).Recordset.Update("EXPAND_FLAG", flg);
                     oNewFeature.Components.GetComponent(iCNO).Recordset.Update("DT_WAYS", DuctPathTo.DuctWay);
                     oNewFeature.Components.GetComponent(iCNO).Recordset.Update("DT_ND_FRM_ID", DuctPathTo.sourceFID);
                     oNewFeature.Components.GetComponent(iCNO).Recordset.Update("DT_ND_FRM_TY", DuctPathTo.sourceType);
                     oNewFeature.Components.GetComponent(iCNO).Recordset.Update("DT_MH_FRM_WALL", DuctPathTo.sourceWall);
                     oNewFeature.Components.GetComponent(iCNO).Recordset.Update("DT_ND_TO_ID", DuctPathTo.termFID);
                     oNewFeature.Components.GetComponent(iCNO).Recordset.Update("DT_ND_TO_TY", DuctPathTo.termType);
                     oNewFeature.Components.GetComponent(iCNO).Recordset.Update("DT_MH_TO_WALL", DuctPathTo.termWall);
                     oNewFeature.Components.GetComponent(iCNO).Recordset.Update("DT_CONSTRUCTION", DuctPathTo.ConstructBy);
                     oNewFeature.Components.GetComponent(iCNO).Recordset.Update("TOTAL_LENGTH", DuctPathTo.Length);
                     oNewFeature.Components.GetComponent(iCNO).Recordset.Update("DT_NEST_WAYS", DuctPathTo.DuctWay);
                     oNewFeature.Components.GetComponent(iCNO).Recordset.Update("DT_ASB_SUBDUCT", DuctPathTo.ASb_SubDuct);
                     oNewFeature.Components.GetComponent(iCNO).Recordset.Update("DT_PP_SUBDUCT", DuctPathTo.Prop_SubDuct);
                     oNewFeature.Components.GetComponent(iCNO).Recordset.Update("DT_PP_INNDUCT", DuctPathTo.Prop_InnDuct);
                     oNewFeature.Components.GetComponent(iCNO).Recordset.Update("DESCRIPTION", DuctPathTo.Description);
                     oNewFeature.Components.GetComponent(iCNO).Recordset.Update("DT_NEST_WAYS", DuctPathTo.NestAssignDuctWay);
                     oNewFeature.Components.GetComponent(iCNO).Recordset.Update("DUCT_BALANCE", DuctPathTo.DuctBalance);
                     oNewFeature.Components.GetComponent(iCNO).Recordset.Update("FEATURE_TYPE", DuctPathTo.Feature_type);
                 }

                 #endregion
                 m_gtapp.SetProgressBarPosition(65);
                 #region Netelem
                 iCNO = 51;
                 if (oNewFeature.Components.GetComponent(iCNO).Recordset.EOF)
                 {
                     oNewFeature.Components.GetComponent(iCNO).Recordset.AddNew("G3E_FID", iFID);
                     oNewFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_FNO", iFNO);
                     oNewFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_CID", 1);
                     oNewFeature.Components.GetComponent(iCNO).Recordset.Update("YEAR_PLACED", DuctPathTo.InstallYear);
                     oNewFeature.Components.GetComponent(iCNO).Recordset.Update("SWITCH_CENTRE_CLLI", "0");
                     oNewFeature.Components.GetComponent(iCNO).Recordset.Update("EXC_ABB", DuctPathTo.EXC_ABB);
                     oNewFeature.Components.GetComponent(iCNO).Recordset.Update("FEATURE_STATE", DuctPathTo.Feature_state);
                     oNewFeature.Components.GetComponent(iCNO).Recordset.Update("BILLING_RATE", DuctPathTo.BillingRate);
                     oNewFeature.Components.GetComponent(iCNO).Recordset.Update("MIN_MATERIAL", "0");
                 }
                 else
                 {
                     oNewFeature.Components.GetComponent(iCNO).Recordset.MoveLast();
                     oNewFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_FNO", iFNO);
                     oNewFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_CID", 1);
                     oNewFeature.Components.GetComponent(iCNO).Recordset.Update("YEAR_PLACED", DuctPathTo.InstallYear);
                     oNewFeature.Components.GetComponent(iCNO).Recordset.Update("SWITCH_CENTRE_CLLI", "0");
                     oNewFeature.Components.GetComponent(iCNO).Recordset.Update("EXC_ABB", DuctPathTo.EXC_ABB);
                     oNewFeature.Components.GetComponent(iCNO).Recordset.Update("FEATURE_STATE", DuctPathTo.Feature_state);
                     oNewFeature.Components.GetComponent(iCNO).Recordset.Update("BILLING_RATE", DuctPathTo.BillingRate);
                     oNewFeature.Components.GetComponent(iCNO).Recordset.Update("MIN_MATERIAL", "0");
                 }
                 #endregion
                 m_gtapp.SetProgressBarPosition(70);
                 #region Line Graphic
                 iCNO = 2210;
                 if (oNewFeature.Components.GetComponent(iCNO).Recordset.EOF)
                 {
                     oNewFeature.Components.GetComponent(iCNO).Recordset.AddNew("G3E_FID", iFID);
                     oNewFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_FNO", iFNO);
                     oNewFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_CID", 1);

                 }
                 else
                 {
                     oNewFeature.Components.GetComponent(iCNO).Recordset.MoveLast();
                     oNewFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_FNO", iFNO);
                     oNewFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_CID", 1);
                 }
                 oNewFeature.Components.GetComponent(iCNO).Geometry = DuctPathTo.DuctPathLineGeom;
                 
                 #endregion
                 m_gtapp.SetProgressBarPosition(75);
                 #region NE Connection to Source or Term devices

                 IGTKeyObject oSource = m_IGTDataContext.OpenFeature(DuctPathTo.sourceFNO, DuctPathTo.sourceFID);
                 
                 IGTKeyObject oTerm2 = m_IGTDataContext.OpenFeature(DuctPathTo.termFNO, DuctPathTo.termFID);
                 //   IGTKeyObject oDuctPath = m_IGTDataContext.OpenFeature(2200, iFID);

                 mobjRelationshipService.ActiveFeature = oSource;

                 if (mobjRelationshipService.AllowSilentEstablish(oNewFeature))
                     mobjRelationshipService.SilentEstablish(1, oNewFeature, GTRelationshipOrdinalConstants.gtrelRelationshipOrdinal1);
                 
                 mobjRelationshipService.ActiveFeature = oNewFeature;
                 if (mobjRelationshipService.AllowSilentEstablish(oTerm2))
                     mobjRelationshipService.SilentEstablish(1, oTerm2, GTRelationshipOrdinalConstants.gtrelRelationshipOrdinal2);
                
                 #endregion
                 m_gtapp.SetProgressBarPosition(80);
                 #region Sects
                
                 #region Section Attributes
                 if (DuctPathTo.Sections != null)
                 {
                     for (int i = 0; i < DuctPathTo.Sections.Count; i++)
                     {
                         if (i != 0)
                         {

                             // Section Attributes
                             iCNO = 2202;
                             oNewFeature.Components.GetComponent(iCNO).Recordset.AddNew("G3E_FID", iFID);
                             oNewFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_FNO", iFNO);
                             oNewFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_CID", i + 1);
                             oNewFeature.Components.GetComponent(iCNO).Recordset.Update("DT_S_WAYS", DuctPathTo.Sections[i].NumDuctWaysSect);
                             oNewFeature.Components.GetComponent(iCNO).Recordset.Update("DT_S_LENGTH", DuctPathTo.Sections[i].SectionLength);
                             oNewFeature.Components.GetComponent(iCNO).Recordset.Update("DT_S_TYPE", DuctPathTo.Sections[i].SectType);
                             oNewFeature.Components.GetComponent(iCNO).Recordset.Update("DT_S_PLACMNT", DuctPathTo.Sections[i].SectPlc);
                             oNewFeature.Components.GetComponent(iCNO).Recordset.Update("DT_S_ENCASE", DuctPathTo.Sections[i].Encasement);
                             oNewFeature.Components.GetComponent(iCNO).Recordset.Update("DT_S_DIAMETER", DuctPathTo.Sections[i].SectDiam);
                             oNewFeature.Components.GetComponent(iCNO).Recordset.Update("DT_S_BACKFILL", DuctPathTo.Sections[i].SectBackFill);
                             oNewFeature.Components.GetComponent(iCNO).Recordset.Update("COMMON_TRENCH", DuctPathTo.Sections[i].SectOwner);
                             oNewFeature.Components.GetComponent(iCNO).Recordset.Update("BILLING_RATE", DuctPathTo.Sections[i].SectBillingRate);
                             oNewFeature.Components.GetComponent(iCNO).Recordset.Update("MIN_MATERIAL", DuctPathTo.Sections[i].PUSect);
                             if (DuctPathTo.Sections[i].YearExpanded != null)
                                 if (DuctPathTo.Sections[i].YearExpanded != "0" && DuctPathTo.Sections[i].YearExpanded != "")
                                     oNewFeature.Components.GetComponent(iCNO).Recordset.Update("YEAR_EXPANDED", DateTime.Parse(DuctPathTo.Sections[i].YearExpanded));
                             if (DuctPathTo.Sections[i].YearExtended != null)
                                 if (DuctPathTo.Sections[i].YearExtended != "0" && DuctPathTo.Sections[i].YearExtended != "")
                                     oNewFeature.Components.GetComponent(iCNO).Recordset.Update("YEAR_EXPANDED", DateTime.Parse(DuctPathTo.Sections[i].YearExtended));



                         }
                         else
                         {

                             // Section Attributes
                             iCNO = 2202;
                             if (oNewFeature.Components.GetComponent(iCNO).Recordset.EOF)
                             {
                                 oNewFeature.Components.GetComponent(iCNO).Recordset.AddNew("G3E_FID", iFID);
                                 oNewFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_FNO", iFNO);
                                 oNewFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_CID", i + 1);
                                 oNewFeature.Components.GetComponent(iCNO).Recordset.Update("DT_S_WAYS", DuctPathTo.Sections[i].NumDuctWaysSect);
                                 oNewFeature.Components.GetComponent(iCNO).Recordset.Update("DT_S_LENGTH", DuctPathTo.Sections[i].SectionLength);
                                 oNewFeature.Components.GetComponent(iCNO).Recordset.Update("DT_S_TYPE", DuctPathTo.Sections[i].SectType);
                                 oNewFeature.Components.GetComponent(iCNO).Recordset.Update("DT_S_PLACMNT", DuctPathTo.Sections[i].SectPlc);
                                 oNewFeature.Components.GetComponent(iCNO).Recordset.Update("DT_S_ENCASE", DuctPathTo.Sections[i].Encasement);
                                 oNewFeature.Components.GetComponent(iCNO).Recordset.Update("DT_S_DIAMETER", DuctPathTo.Sections[i].SectDiam);
                                 oNewFeature.Components.GetComponent(iCNO).Recordset.Update("DT_S_BACKFILL", DuctPathTo.Sections[i].SectBackFill);
                                 oNewFeature.Components.GetComponent(iCNO).Recordset.Update("COMMON_TRENCH", DuctPathTo.Sections[i].SectOwner);
                                 oNewFeature.Components.GetComponent(iCNO).Recordset.Update("BILLING_RATE", DuctPathTo.Sections[i].SectBillingRate);
                                 oNewFeature.Components.GetComponent(iCNO).Recordset.Update("MIN_MATERIAL", DuctPathTo.Sections[i].PUSect);

                                 if (DuctPathTo.Sections[i].YearExpanded != null)
                                     if (DuctPathTo.Sections[i].YearExpanded != "0" && DuctPathTo.Sections[i].YearExpanded != "")
                                         oNewFeature.Components.GetComponent(iCNO).Recordset.Update("YEAR_EXPANDED", DateTime.Parse(DuctPathTo.Sections[i].YearExpanded));
                                 if (DuctPathTo.Sections[i].YearExtended != null)
                                     if (DuctPathTo.Sections[i].YearExtended != "0" && DuctPathTo.Sections[i].YearExtended != "")
                                         oNewFeature.Components.GetComponent(iCNO).Recordset.Update("YEAR_EXPANDED", DateTime.Parse(DuctPathTo.Sections[i].YearExtended));

                             }
                             else
                             {
                                 oNewFeature.Components.GetComponent(iCNO).Recordset.MoveLast();
                                 oNewFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_FNO", iFNO);
                                 oNewFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_CID", i + 1);
                                 oNewFeature.Components.GetComponent(iCNO).Recordset.Update("DT_S_WAYS", DuctPathTo.Sections[i].NumDuctWaysSect);
                                 oNewFeature.Components.GetComponent(iCNO).Recordset.Update("DT_S_LENGTH", DuctPathTo.Sections[i].SectionLength);
                                 oNewFeature.Components.GetComponent(iCNO).Recordset.Update("DT_S_TYPE", DuctPathTo.Sections[i].SectType);
                                 oNewFeature.Components.GetComponent(iCNO).Recordset.Update("DT_S_PLACMNT", DuctPathTo.Sections[i].SectPlc);
                                 oNewFeature.Components.GetComponent(iCNO).Recordset.Update("DT_S_ENCASE", DuctPathTo.Sections[i].Encasement);
                                 oNewFeature.Components.GetComponent(iCNO).Recordset.Update("DT_S_DIAMETER", DuctPathTo.Sections[i].SectDiam);
                                 oNewFeature.Components.GetComponent(iCNO).Recordset.Update("DT_S_BACKFILL", DuctPathTo.Sections[i].SectBackFill);
                                 oNewFeature.Components.GetComponent(iCNO).Recordset.Update("COMMON_TRENCH", DuctPathTo.Sections[i].SectOwner);
                                 oNewFeature.Components.GetComponent(iCNO).Recordset.Update("BILLING_RATE", DuctPathTo.Sections[i].SectBillingRate);
                                 oNewFeature.Components.GetComponent(iCNO).Recordset.Update("MIN_MATERIAL", DuctPathTo.Sections[i].PUSect);
                                 if (DuctPathTo.Sections[i].YearExpanded != null)
                                     if (DuctPathTo.Sections[i].YearExpanded != "0" && DuctPathTo.Sections[i].YearExpanded != "")
                                         oNewFeature.Components.GetComponent(iCNO).Recordset.Update("YEAR_EXPANDED", DateTime.Parse(DuctPathTo.Sections[i].YearExpanded));
                                 if (DuctPathTo.Sections[i].YearExtended != null)
                                     if (DuctPathTo.Sections[i].YearExtended != "0" && DuctPathTo.Sections[i].YearExtended != "")
                                         oNewFeature.Components.GetComponent(iCNO).Recordset.Update("YEAR_EXPANDED", DateTime.Parse(DuctPathTo.Sections[i].YearExtended));

                             }


                         }
                     }
                 }
                 #endregion

                 #region Lables for Sections and Leader Lines
                 if (DuctPathTo.SectLabels != null)
                 {
                     if (DuctPathTo.SectLabels.Count > 0)
                         for (int i = 0; i < DuctPathTo.SectLabels.Count; i++)
                         {
                             if (i != 0)
                             {
                                 iCNO = 2230;
                                 oNewFeature.Components.GetComponent(iCNO).Recordset.AddNew("G3E_FID", iFID);
                                 oNewFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_FNO", iFNO);
                                 oNewFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_CID", i + 1);
                                 oNewFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_ALIGNMENT", 0);
                                 oNewFeature.Components.GetComponent(iCNO).Geometry = DuctPathTo.SectLabels[i].Label;

                                 if (DuctPathTo.SectLabels[i].LeaderLine != null)
                                 {
                                     iCNO = 2212;
                                     oNewFeature.Components.GetComponent(iCNO).Recordset.AddNew("G3E_FID", iFID);
                                     oNewFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_FNO", iFNO);
                                     oNewFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_CID", i + 1);
                                     oNewFeature.Components.GetComponent(iCNO).Geometry = DuctPathTo.SectLabels[i].LeaderLine;

                                 }
                             }
                             else
                             {
                                 iCNO = 2230;

                                 if (oNewFeature.Components.GetComponent(iCNO).Recordset.EOF)
                                 {
                                     oNewFeature.Components.GetComponent(iCNO).Recordset.AddNew("G3E_FID", iFID);
                                     oNewFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_FNO", iFNO);
                                     oNewFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_CID", i + 1);
                                     oNewFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_ALIGNMENT", 0);
                                 }
                                 else
                                 {
                                     oNewFeature.Components.GetComponent(iCNO).Recordset.MoveLast();
                                     oNewFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_FNO", iFNO);
                                     oNewFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_CID", i + 1);
                                     oNewFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_ALIGNMENT", 0);
                                 }
                                 oNewFeature.Components.GetComponent(iCNO).Geometry = DuctPathTo.SectLabels[i].Label;

                                 if (DuctPathTo.SectLabels[i].LeaderLine != null)
                                 {
                                     iCNO = 2212;
                                     if (oNewFeature.Components.GetComponent(iCNO).Recordset.EOF)
                                     {
                                         oNewFeature.Components.GetComponent(iCNO).Recordset.AddNew("G3E_FID", iFID);
                                         oNewFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_FNO", iFNO);
                                         oNewFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_CID", i + 1);
                                     }
                                     else
                                     {
                                         oNewFeature.Components.GetComponent(iCNO).Recordset.MoveLast();
                                         oNewFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_FNO", iFNO);
                                         oNewFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_CID", i + 1);
                                     }
                                     oNewFeature.Components.GetComponent(iCNO).Geometry = DuctPathTo.SectLabels[i].LeaderLine;

                                 }
                             }

                         }
                 }
                 #endregion

                 #region Slashes for sections
                 if (DuctPathTo.SectSlashes != null)
                 {
                     if (DuctPathTo.Sections.Count == DuctPathTo.SectSlashes.Count && DuctPathTo.Sections.Count != 0)
                         DuctPathTo.SectSlashes.RemoveAt(DuctPathTo.SectSlashes.Count - 1);

                     if (DuctPathTo.SectSlashes.Count > 0)
                         for (int i = 0; i < DuctPathTo.SectSlashes.Count; i++)
                         {

                             iCNO = 2220;
                             if (i != 0)
                             {

                                 oNewFeature.Components.GetComponent(iCNO).Recordset.AddNew("G3E_FID", iFID);
                                 oNewFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_FNO", iFNO);
                                 oNewFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_CID", i + 1);
                             }
                             else
                             {
                                 if (oNewFeature.Components.GetComponent(iCNO).Recordset.EOF)
                                 {
                                     oNewFeature.Components.GetComponent(iCNO).Recordset.AddNew("G3E_FID", iFID);
                                     oNewFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_FNO", iFNO);
                                     oNewFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_CID", i + 1);
                                 }
                                 else
                                 {
                                     oNewFeature.Components.GetComponent(iCNO).Recordset.MoveLast();
                                     oNewFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_FNO", iFNO);
                                     oNewFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_CID", i + 1);
                                 }
                             }
                             oNewFeature.Components.GetComponent(iCNO).Geometry = DuctPathTo.SectSlashes[i].Slash;
                         }
                 }
                 #endregion
                 #endregion
                 m_gtapp.SetProgressBarPosition(81);

                 #region create ductnest
                 if (DuctPathTo.DuctNestTo != null && DuctPathTo.DuctNestFrom != null)
                 {
                     for (int j = 0; j < DuctPathTo.DuctNestFrom.Count; j++)
                     {
                         for (int i = 0; i < DuctPathTo.DuctNestTo.Count; i++)
                         {
                             if (DuctPathTo.DuctNestFrom[j].FID == DuctPathTo.DuctNestTo[i].FID)
                             {
                                 IGTKeyObject oDuctNest = CreateDuctNest(DuctPathTo.DuctNestFrom[j], DuctPathTo.DuctNestTo[i]);
                                 if (oDuctNest != null)
                                 {
                                     //Ownership        
                                     mobjRelationshipService.ActiveFeature = oNewFeature;
                                     if (mobjRelationshipService.AllowSilentEstablish(oDuctNest))
                                     {
                                         mobjRelationshipService.SilentEstablish(7, oDuctNest);
                                     }
                                 }
                                 break;
                             }
                         }

                     }
                 }
                 #endregion
                
                 m_gtapp.SetProgressBarPosition(90);
                #endregion
              
                #region finishing
                m_oIGTTransactionManager.Commit();
                m_gtapp.SetProgressBarPosition(95);
                m_oIGTTransactionManager.RefreshDatabaseChanges();

                m_gtapp.SetProgressBarPosition(100);
               
                if (mobjEditService != null)
                {
                    mobjEditService.RemoveAllGeometries();
                    mobjEditService = null;
                }
                if (mobjEditService1 != null)
                {
                    mobjEditService1.RemoveAllGeometries();
                    mobjEditService1 = null;
                }
                if (mobjEditService2 != null)
                {
                    mobjEditService2.RemoveAllGeometries();
                    mobjEditService2 = null;
                }
                if (mobjEditServiceRotate != null)
                {
                    mobjEditServiceRotate.RemoveAllGeometries();
                    mobjEditServiceRotate = null;
                }
                if (mobjEditServiceRotate1 != null)
                {
                    mobjEditServiceRotate1.RemoveAllGeometries();
                    mobjEditServiceRotate1 = null;
                }
                if (mobjEditServiceRotate2 != null)
                {
                    mobjEditServiceRotate2.RemoveAllGeometries();
                    mobjEditServiceRotate2 = null;
                }
                if (mobjEditServiceSlash != null)
                {
                    mobjEditServiceSlash.RemoveAllGeometries();
                    mobjEditServiceSlash = null;
                }

                if (mobjLocateService != null)
                    mobjLocateService = null;
                if (mobjRelationshipService != null)
                {
                    mobjRelationshipService = null;
                }
                m_gtapp.RefreshWindows();
                m_gtapp.EndProgressBar();
                m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "");
                
                if(msg!=null) msg.Close();
                MessageBox.Show("Breaking Duct Path is successfully completed!", "Manhole Insert", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ExitCmd();
                #endregion
            }
            catch (Exception ex)
            {
                m_oIGTTransactionManager.Rollback();                
                if(msg!=null) msg.Close();
                MessageBox.Show(ex.Message, "Manhole Insert", MessageBoxButtons.OK, MessageBoxIcon.Error);
                m_gtapp.EndProgressBar();
                m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "");
                ExitCmd(); 
            }
            return true;
        }
        #endregion

        #region CreateDuctNest
        public IGTKeyObject CreateDuctNest(DuctNest dnfrom, DuctNest dnto)
        {
            try
            {
                IGTKeyObject newDuctNest = m_IGTDataContext.NewFeature(dnfrom.FNO);
                int newFID = newDuctNest.FID;
                if (dnto.FNO != 2400 || dnfrom.FNO != 2400) return null;

               IGTKeyObject existNest = m_IGTDataContext.OpenFeature(dnfrom.FNO, dnfrom.FID);
                short A_CNO = 2401;

                #region attr
                
                if (newDuctNest.Components.GetComponent(A_CNO).Recordset.EOF)
                {
                    newDuctNest.Components.GetComponent(A_CNO).Recordset.AddNew("G3E_FID", newFID);
                    newDuctNest.Components.GetComponent(A_CNO).Recordset.Update("G3E_FNO", dnfrom.FNO);
                    newDuctNest.Components.GetComponent(A_CNO).Recordset.Update("G3E_CID", 1);
                }

                for (int j = 0; j < existNest.Components.GetComponent(A_CNO).Recordset.Fields.Count; j++)
                {
                    if ((existNest.Components.GetComponent(A_CNO).Recordset.Fields[j].Name != "G3E_FID") &&
                        (existNest.Components.GetComponent(A_CNO).Recordset.Fields[j].Name != "G3E_FNO") &&
                        (existNest.Components.GetComponent(A_CNO).Recordset.Fields[j].Name != "G3E_CNO") &&
                        (existNest.Components.GetComponent(A_CNO).Recordset.Fields[j].Name != "G3E_CID") &&
                        (existNest.Components.GetComponent(A_CNO).Recordset.Fields[j].Name != "G3E_ID")
                    )newDuctNest.Components.GetComponent(A_CNO).Recordset.Update(existNest.Components.GetComponent(A_CNO).Recordset.Fields[j].Name, existNest.Components.GetComponent(A_CNO).Recordset.Fields[j].Value);
                }
                #endregion

                #region netelem
                A_CNO = 51;
                if (newDuctNest.Components.GetComponent(A_CNO).Recordset.EOF)
                {
                    newDuctNest.Components.GetComponent(A_CNO).Recordset.AddNew("G3E_FID", newFID);
                    newDuctNest.Components.GetComponent(A_CNO).Recordset.Update("G3E_FNO", dnfrom.FNO);
                    newDuctNest.Components.GetComponent(A_CNO).Recordset.Update("G3E_CID", 1);
                }

                for (int j = 0; j < existNest.Components.GetComponent(A_CNO).Recordset.Fields.Count; j++)
                {
                    if ((existNest.Components.GetComponent(A_CNO).Recordset.Fields[j].Name != "G3E_FID") &&
                        (existNest.Components.GetComponent(A_CNO).Recordset.Fields[j].Name != "G3E_FNO") &&
                        (existNest.Components.GetComponent(A_CNO).Recordset.Fields[j].Name != "G3E_CNO") &&
                        (existNest.Components.GetComponent(A_CNO).Recordset.Fields[j].Name != "G3E_CID") &&
                        (existNest.Components.GetComponent(A_CNO).Recordset.Fields[j].Name != "G3E_ID")
                    )
                    newDuctNest.Components.GetComponent(A_CNO).Recordset.Update(existNest.Components.GetComponent(A_CNO).Recordset.Fields[j].Name, existNest.Components.GetComponent(A_CNO).Recordset.Fields[j].Value);
                }
                #endregion

                #region geomfrom
                A_CNO = 2410;

                existNest.Components.GetComponent(A_CNO).Recordset.MoveFirst();
                for (int i = 0; i < dnfrom.Form.Count; i++)
                {
                    if (newDuctNest.Components.GetComponent(A_CNO).Recordset.EOF)
                    {
                        newDuctNest.Components.GetComponent(A_CNO).Recordset.AddNew("G3E_FID", newFID);
                        newDuctNest.Components.GetComponent(A_CNO).Recordset.Update("G3E_FNO", dnfrom.FNO);
                        newDuctNest.Components.GetComponent(A_CNO).Recordset.Update("G3E_CID", 1);
                    }
                      for (int j = 0; j < existNest.Components.GetComponent(A_CNO).Recordset.Fields.Count; j++)
                    {
                        if ((existNest.Components.GetComponent(A_CNO).Recordset.Fields[j].Name != "G3E_FID") &&
                            (existNest.Components.GetComponent(A_CNO).Recordset.Fields[j].Name != "G3E_FNO") &&
                            (existNest.Components.GetComponent(A_CNO).Recordset.Fields[j].Name != "G3E_CNO") &&
                            (existNest.Components.GetComponent(A_CNO).Recordset.Fields[j].Name != "G3E_CID") &&
                            (existNest.Components.GetComponent(A_CNO).Recordset.Fields[j].Name != "G3E_ID") &&
                            (existNest.Components.GetComponent(A_CNO).Recordset.Fields[j].Name != "G3E_GEOMETRY")
                        )
                            newDuctNest.Components.GetComponent(A_CNO).Recordset.Update(existNest.Components.GetComponent(A_CNO).Recordset.Fields[j].Name, existNest.Components.GetComponent(A_CNO).Recordset.Fields[j].Value);
                    }
                    newDuctNest.Components.GetComponent(A_CNO).Geometry = dnfrom.Form[i].geom;
                   existNest.Components.GetComponent(A_CNO).Recordset.MoveNext();
                }

                A_CNO = 2430;
              existNest.Components.GetComponent(A_CNO).Recordset.MoveFirst();
                for (int i = 0; i < dnfrom.Labels.Count; i++)
                {
                    int cid = i + 1;
                    newDuctNest.Components.GetComponent(A_CNO).Recordset.AddNew("G3E_FID", newFID);
                    newDuctNest.Components.GetComponent(A_CNO).Recordset.Update("G3E_FNO", dnfrom.FNO);
                    newDuctNest.Components.GetComponent(A_CNO).Recordset.Update("G3E_CID", cid);

                    for (int j = 0; j < existNest.Components.GetComponent(A_CNO).Recordset.Fields.Count; j++)
                    {
                        if ((existNest.Components.GetComponent(A_CNO).Recordset.Fields[j].Name != "G3E_FID") &&
                            (existNest.Components.GetComponent(A_CNO).Recordset.Fields[j].Name != "G3E_FNO") &&
                            (existNest.Components.GetComponent(A_CNO).Recordset.Fields[j].Name != "G3E_CNO") &&
                           // (existNest.Components.GetComponent(A_CNO).Recordset.Fields[j].Name != "G3E_CID") &&
                            (existNest.Components.GetComponent(A_CNO).Recordset.Fields[j].Name != "G3E_ID") &&
                            (existNest.Components.GetComponent(A_CNO).Recordset.Fields[j].Name != "G3E_GEOMETRY")
                        )
                            newDuctNest.Components.GetComponent(A_CNO).Recordset.Update(existNest.Components.GetComponent(A_CNO).Recordset.Fields[j].Name, existNest.Components.GetComponent(A_CNO).Recordset.Fields[j].Value);
                        
                    }
                    newDuctNest.Components.GetComponent(A_CNO).Geometry = dnfrom.Labels[i].geom;   
                     
                            existNest.Components.GetComponent(A_CNO).Recordset.MoveNext();
                }

                //update label for from label with new manhole id


                newDuctNest.Components.GetComponent(A_CNO).Recordset.MoveFirst();
                for (int j = 0; j < newDuctNest.Components.GetComponent(A_CNO).Recordset.RecordCount; j++)
                {
                    string cid = "1";
                    for (int f = 0; f < newDuctNest.Components.GetComponent(A_CNO).Recordset.Fields.Count; f++)
                    {
                        if (newDuctNest.Components.GetComponent(A_CNO).Recordset.Fields[f].Name == "G3E_CID")
                        {
                            cid = newDuctNest.Components.GetComponent(A_CNO).Recordset.Fields[f].Value.ToString();
                            break;
                        }
                    }

                    string textto = Get_Value("select g3e_text from GC_FORMFROM_T where g3e_cid=" + cid.ToString() + " and g3e_fno=2400 and g3e_fid=" + dnfrom.FID);
                    int ind = textto.IndexOf(']');
                    if (ind > 0)
                    {
                        textto = textto.Substring(0, ind + 1);
                        textto += " " + Get_Value("select MANHOLE_ID from GC_MANHL where g3e_fid = " + DuctPathTo.termFID);
                        newDuctNest.Components.GetComponent(A_CNO).Recordset.Update("G3E_TEXT", textto);
                        ind = 0;
                        break;
                    }
                    newDuctNest.Components.GetComponent(A_CNO).Recordset.MoveNext();
                }
                #endregion

                #region geomto
                A_CNO = 2412;
              existNest.Components.GetComponent(A_CNO).Recordset.MoveFirst();
                for (int i = 0; i < dnto.Form.Count; i++)
                {
                    if (newDuctNest.Components.GetComponent(A_CNO).Recordset.EOF)
                    {
                        newDuctNest.Components.GetComponent(A_CNO).Recordset.AddNew("G3E_FID", newFID);
                        newDuctNest.Components.GetComponent(A_CNO).Recordset.Update("G3E_FNO", dnfrom.FNO);
                        newDuctNest.Components.GetComponent(A_CNO).Recordset.Update("G3E_CID", 1);
                    }

                    for (int j = 0; j < existNest.Components.GetComponent(A_CNO).Recordset.Fields.Count; j++)
                    {
                        if ((existNest.Components.GetComponent(A_CNO).Recordset.Fields[j].Name != "G3E_FID") &&
                            (existNest.Components.GetComponent(A_CNO).Recordset.Fields[j].Name != "G3E_FNO") &&
                            (existNest.Components.GetComponent(A_CNO).Recordset.Fields[j].Name != "G3E_CNO") &&
                            (existNest.Components.GetComponent(A_CNO).Recordset.Fields[j].Name != "G3E_CID") &&
                            (existNest.Components.GetComponent(A_CNO).Recordset.Fields[j].Name != "G3E_ID") &&
                            (existNest.Components.GetComponent(A_CNO).Recordset.Fields[j].Name != "G3E_GEOMETRY")
                        )
                        newDuctNest.Components.GetComponent(A_CNO).Recordset.Update(existNest.Components.GetComponent(A_CNO).Recordset.Fields[j].Name, existNest.Components.GetComponent(A_CNO).Recordset.Fields[j].Value);
                    } 
                    newDuctNest.Components.GetComponent(A_CNO).Geometry = dnto.Form[i].geom;
        
                    existNest.Components.GetComponent(A_CNO).Recordset.MoveNext();
                }

                A_CNO = 2432;
                existNest.Components.GetComponent(A_CNO).Recordset.MoveFirst();
                for (int i = 0; i < dnto.Labels.Count; i++)
                {
                    int cid=i+1;
                    newDuctNest.Components.GetComponent(A_CNO).Recordset.AddNew("G3E_FID", newFID);
                    newDuctNest.Components.GetComponent(A_CNO).Recordset.Update("G3E_FNO", dnfrom.FNO);
                    newDuctNest.Components.GetComponent(A_CNO).Recordset.Update("G3E_CID", cid);

                    for (int j = 0; j < existNest.Components.GetComponent(A_CNO).Recordset.Fields.Count; j++)
                    {
                        if ((existNest.Components.GetComponent(A_CNO).Recordset.Fields[j].Name != "G3E_FID") &&
                            (existNest.Components.GetComponent(A_CNO).Recordset.Fields[j].Name != "G3E_FNO") &&
                            (existNest.Components.GetComponent(A_CNO).Recordset.Fields[j].Name != "G3E_CNO") &&
                            //(existNest.Components.GetComponent(A_CNO).Recordset.Fields[j].Name != "G3E_CID") &&
                            (existNest.Components.GetComponent(A_CNO).Recordset.Fields[j].Name != "G3E_ID") &&
                            (existNest.Components.GetComponent(A_CNO).Recordset.Fields[j].Name != "G3E_GEOMETRY")
                        )
                            newDuctNest.Components.GetComponent(A_CNO).Recordset.Update(existNest.Components.GetComponent(A_CNO).Recordset.Fields[j].Name, existNest.Components.GetComponent(A_CNO).Recordset.Fields[j].Value);
                       
                    }
                   
                    
                    newDuctNest.Components.GetComponent(A_CNO).Geometry = dnto.Labels[i].geom;
                    existNest.Components.GetComponent(A_CNO).Recordset.MoveNext();
                }

                newDuctNest.Components.GetComponent(A_CNO).Recordset.MoveFirst();
                for (int j = 0; j < newDuctNest.Components.GetComponent(A_CNO).Recordset.RecordCount; j++)
                {
                    string cid = "1";
                    for (int f = 0; f < newDuctNest.Components.GetComponent(A_CNO).Recordset.Fields.Count; f++)
                    {
                        if (newDuctNest.Components.GetComponent(A_CNO).Recordset.Fields[f].Name == "G3E_CID")
                        {
                            cid = newDuctNest.Components.GetComponent(A_CNO).Recordset.Fields[f].Value.ToString();
                            break;
                        }
                    }

                    string textto = Get_Value("select g3e_text from GC_FORMTO_T where g3e_cid=" + cid.ToString() + " and g3e_fno=2400 and g3e_fid=" + dnfrom.FID);
                    int ind = textto.IndexOf(']');
                    if (ind > 0)
                    {
                        textto = textto.Substring(0, ind + 1);
                        textto += " " + Get_Value("select MANHOLE_ID from GC_MANHL where g3e_fid = " + DuctPathTo.sourceFID);
                        newDuctNest.Components.GetComponent(A_CNO).Recordset.Update("G3E_TEXT", textto);
                        ind = 0;
                        break;
                    }
                    newDuctNest.Components.GetComponent(A_CNO).Recordset.MoveNext();
                }
                #endregion

             

                #region createduct
                //update fid for relationship
                for (int ow = 0; ow < dnfrom.Ducts.Count; ow++)
                    if (dnfrom.Ducts[ow].OwnerFID == dnfrom.FID)
                        dnfrom.Ducts[ow].OwnerFID = newDuctNest.FID;
                for (int ow = 0; ow < dnto.Ducts.Count; ow++)
                    if (dnto.Ducts[ow].OwnerFID == dnto.FID)
                        dnto.Ducts[ow].OwnerFID = newDuctNest.FID;

                for (int j = 0; j < dnfrom.Ducts.Count; j++)
                {
                    for (int i = 0; i < dnto.Ducts.Count; i++)
                    {
                        if (dnfrom.Ducts[j].FID == dnto.Ducts[i].FID)
                        {
                            IGTKeyObject oDuct = CreateDuct(dnfrom.Ducts[j], dnto.Ducts[i]);
                            //update fid for relationship
                            for (int ow = 0; ow < dnfrom.Ducts.Count; ow++)
                                if (dnfrom.Ducts[ow].OwnerFID == dnfrom.Ducts[j].FID)
                                    dnfrom.Ducts[ow].OwnerFID = oDuct.FID;
                            for (int ow = 0; ow < dnto.Ducts.Count; ow++)
                                if (dnto.Ducts[ow].OwnerFID == dnto.Ducts[i].FID)
                                    dnto.Ducts[ow].OwnerFID = oDuct.FID;
                            dnfrom.Ducts[j].FID = oDuct.FID;
                            dnto.Ducts[i].FID = oDuct.FID;
                            break;
                        }
                    }

                }
                //create relationship
                for (int j = 0; j < dnfrom.Ducts.Count; j++)
                {
                    IGTKeyObject newDuct = m_IGTDataContext.OpenFeature(dnfrom.Ducts[j].FNO, dnfrom.Ducts[j].FID);
                    //Ownership        
                    mobjRelationshipService.ActiveFeature = m_IGTDataContext.OpenFeature(dnfrom.Ducts[j].OwnerFNO, dnfrom.Ducts[j].OwnerFID); ;
                    if (mobjRelationshipService.AllowSilentEstablish(newDuct))
                    {
                        mobjRelationshipService.SilentEstablish(7, newDuct);
                    }
                }
                #endregion

                return newDuctNest;
            }
            catch (Exception ex) { return null; }
         }

        public IGTKeyObject CreateDuct(Duct dfrom, Duct dto)
        {
            IGTKeyObject newDuct = m_IGTDataContext.NewFeature(dfrom.FNO);
            int newFID = newDuct.FID;

          //  if (dto.FNO != 2300 || dfrom.FNO!=2300) return null;
            IGTKeyObject existDuct = m_IGTDataContext.OpenFeature(dfrom.FNO, dfrom.FID);
            short A_CNO = dfrom.FNO;//dfrom.Form[0].CNO;
            A_CNO++;
            #region attr
            if (newDuct.Components.GetComponent(A_CNO).Recordset.EOF)
            {
                newDuct.Components.GetComponent(A_CNO).Recordset.AddNew("G3E_FID", newFID);
                newDuct.Components.GetComponent(A_CNO).Recordset.Update("G3E_FNO", dfrom.FNO);
                newDuct.Components.GetComponent(A_CNO).Recordset.Update("G3E_CID", 1);
            }
            for (int j = 0; j < existDuct.Components.GetComponent(A_CNO).Recordset.Fields.Count; j++)
            {
                if ((existDuct.Components.GetComponent(A_CNO).Recordset.Fields[j].Name != "G3E_FID") &&
                    (existDuct.Components.GetComponent(A_CNO).Recordset.Fields[j].Name != "G3E_FNO") &&
                    (existDuct.Components.GetComponent(A_CNO).Recordset.Fields[j].Name != "G3E_CNO") &&
                    (existDuct.Components.GetComponent(A_CNO).Recordset.Fields[j].Name != "G3E_CID") &&
                    (existDuct.Components.GetComponent(A_CNO).Recordset.Fields[j].Name != "G3E_ID")
                )
                   
                newDuct.Components.GetComponent(A_CNO).Recordset.Update(existDuct.Components.GetComponent(A_CNO).Recordset.Fields[j].Name, existDuct.Components.GetComponent(A_CNO).Recordset.Fields[j].Value);
            }
            #endregion

            #region netelem
            A_CNO = 51;
            if (newDuct.Components.GetComponent(A_CNO).Recordset.EOF)
            {
                newDuct.Components.GetComponent(A_CNO).Recordset.AddNew("G3E_FID", newFID);
                newDuct.Components.GetComponent(A_CNO).Recordset.Update("G3E_FNO", dfrom.FNO);
                newDuct.Components.GetComponent(A_CNO).Recordset.Update("G3E_CID", 1);
            }
            for (int j = 0; j < existDuct.Components.GetComponent(A_CNO).Recordset.Fields.Count; j++)
            {
                if ((existDuct.Components.GetComponent(A_CNO).Recordset.Fields[j].Name != "G3E_FID") &&
                    (existDuct.Components.GetComponent(A_CNO).Recordset.Fields[j].Name != "G3E_FNO") &&
                    (existDuct.Components.GetComponent(A_CNO).Recordset.Fields[j].Name != "G3E_CNO") &&
                    (existDuct.Components.GetComponent(A_CNO).Recordset.Fields[j].Name != "G3E_CID") &&
                    (existDuct.Components.GetComponent(A_CNO).Recordset.Fields[j].Name != "G3E_ID")
                )
                   
                newDuct.Components.GetComponent(A_CNO).Recordset.Update(existDuct.Components.GetComponent(A_CNO).Recordset.Fields[j].Name, existDuct.Components.GetComponent(A_CNO).Recordset.Fields[j].Value);
            }
            #endregion

            #region geomfrom
           // A_CNO = 2320;
            A_CNO = dfrom.Form[0].CNO;
            existDuct.Components.GetComponent(A_CNO).Recordset.MoveFirst();
            for (int i = 0; i < dfrom.Form.Count; i++)
            {
                A_CNO = dfrom.Form[i].CNO;
                if (newDuct.Components.GetComponent(A_CNO).Recordset.EOF)
                {
                    newDuct.Components.GetComponent(A_CNO).Recordset.AddNew("G3E_FID", newFID);
                    newDuct.Components.GetComponent(A_CNO).Recordset.Update("G3E_FNO", dfrom.FNO);
                    newDuct.Components.GetComponent(A_CNO).Recordset.Update("G3E_CID", i + 1);
                }
                for (int j = 0; j < existDuct.Components.GetComponent(A_CNO).Recordset.Fields.Count; j++)
                {
                    if ((existDuct.Components.GetComponent(A_CNO).Recordset.Fields[j].Name != "G3E_FID") &&
                        (existDuct.Components.GetComponent(A_CNO).Recordset.Fields[j].Name != "G3E_FNO") &&
                        (existDuct.Components.GetComponent(A_CNO).Recordset.Fields[j].Name != "G3E_CNO") &&
                        (existDuct.Components.GetComponent(A_CNO).Recordset.Fields[j].Name != "G3E_CID") &&
                        (existDuct.Components.GetComponent(A_CNO).Recordset.Fields[j].Name != "G3E_ID") &&
                        (existDuct.Components.GetComponent(A_CNO).Recordset.Fields[j].Name != "G3E_GEOMETRY")
                    )
                        
                    newDuct.Components.GetComponent(A_CNO).Recordset.Update(existDuct.Components.GetComponent(A_CNO).Recordset.Fields[j].Name, existDuct.Components.GetComponent(A_CNO).Recordset.Fields[j].Value);
                }
                newDuct.Components.GetComponent(A_CNO).Geometry = dfrom.Form[i].geom;
                existDuct.Components.GetComponent(A_CNO).Recordset.MoveNext();
            }

           
            #endregion

            #region geomto
         //   A_CNO = 2322;
            A_CNO = dto.Form[0].CNO;
            existDuct.Components.GetComponent(A_CNO).Recordset.MoveFirst();
            for (int i = 0; i < dto.Form.Count; i++)
            {
                A_CNO = dto.Form[i].CNO;
                if (newDuct.Components.GetComponent(A_CNO).Recordset.EOF)
                {
                    newDuct.Components.GetComponent(A_CNO).Recordset.AddNew("G3E_FID", newFID);
                    newDuct.Components.GetComponent(A_CNO).Recordset.Update("G3E_FNO", dto.FNO);
                    newDuct.Components.GetComponent(A_CNO).Recordset.Update("G3E_CID", i + 1);
                }
                for (int j = 0; j < existDuct.Components.GetComponent(A_CNO).Recordset.Fields.Count; j++)
                {
                    if ((existDuct.Components.GetComponent(A_CNO).Recordset.Fields[j].Name != "G3E_FID") &&
                        (existDuct.Components.GetComponent(A_CNO).Recordset.Fields[j].Name != "G3E_FNO") &&
                        (existDuct.Components.GetComponent(A_CNO).Recordset.Fields[j].Name != "G3E_CNO") &&
                        (existDuct.Components.GetComponent(A_CNO).Recordset.Fields[j].Name != "G3E_CID") &&
                        (existDuct.Components.GetComponent(A_CNO).Recordset.Fields[j].Name != "G3E_ID") &&
                        (existDuct.Components.GetComponent(A_CNO).Recordset.Fields[j].Name != "G3E_GEOMETRY")
                    )
                    newDuct.Components.GetComponent(A_CNO).Recordset.Update(existDuct.Components.GetComponent(A_CNO).Recordset.Fields[j].Name, existDuct.Components.GetComponent(A_CNO).Recordset.Fields[j].Value);
                }
                newDuct.Components.GetComponent(A_CNO).Geometry = dto.Form[i].geom;
                existDuct.Components.GetComponent(A_CNO).Recordset.MoveNext();
            }

           
            #endregion

            
            return newDuct;
        }
        #endregion

        #region ZoomIn/ZoomOut

        public void LocateFeature(int flag, IGTMapWindow window, short SourceFNO, int SourceFID, short TermFNO, int TermFID)
        {
            if (window == null) return;
            IGTDDCKeyObjects feat = null;
            short iFNO = SourceFNO;
            int lFID = SourceFID;

             m_gtapp.SelectedObjects.Clear();

            if (flag == 1)//fit for source feature
            {
                feat =  m_gtapp.DataContext.GetDDCKeyObjects(iFNO, lFID, GTComponentGeometryConstants.gtddcgAllGeographic);


                for (int K = 0; K < feat.Count; K++)
                {
                    string t = feat[K].ComponentViewName.ToString();
                    if (feat[K].ComponentViewName == "VGC_MANHL_S" ||
                        feat[K].ComponentViewName == "VGC_PSGCON_S")
                    {
                        //2700 VGC_MANHL_S  
                        //2800 VGC_PSGCON_S
                        IGTWorldRange range = GTClassFactory.Create<IGTWorldRange>();
                        IGTPoint point1 = GTClassFactory.Create<IGTPoint>();
                        IGTPoint point2 = GTClassFactory.Create<IGTPoint>();
                        point1.X = feat[K].Geometry.FirstPoint.X - 3;
                        point1.Y = feat[K].Geometry.FirstPoint.Y - 3;
                        range.BottomLeft = point1;
                        point2.X = feat[K].Geometry.FirstPoint.X + 3;
                        point2.Y = feat[K].Geometry.FirstPoint.Y + 3;
                        range.TopRight = point2;
                        window.ZoomArea(range);
                         m_gtapp.RefreshWindows();
                         m_gtapp.SelectedObjects.Clear();
                        return;
                    }

                    if (feat[K].ComponentViewName == "VGC_CHAMBER_P" ||
                        feat[K].ComponentViewName == "VGC_TUNNEL_P")
                    {
                        //3800 VGC_CHAMBER_P
                        //3300 VGC_TUNNEL_P
                        for (int K2 = 0; K2 < feat.Count; K2++)
                             m_gtapp.SelectedObjects.Add(GTSelectModeConstants.gtsosmAllDisplayedComponentsInActiveLegend, feat[K2]);
                        window.FitSelectedObjects();
                        window.CenterSelectedObjects();
                         m_gtapp.RefreshWindows();
                         m_gtapp.SelectedObjects.Clear();
                        return;
                    }
                }

            }
            if (flag == 2)//copy source feature to selected obj to fit both source and term
            {
                feat =  m_gtapp.DataContext.GetDDCKeyObjects(iFNO, lFID, GTComponentGeometryConstants.gtddcgAllGeographic);
                for (int K = 0; K < feat.Count; K++)
                     m_gtapp.SelectedObjects.Add(GTSelectModeConstants.gtsosmAllDisplayedComponentsInActiveLegend, feat[K]);
            }
            iFNO = TermFNO;
            lFID = TermFID;// int.Parse(txtFIDTerm.Text);

            if (flag == 3)//fit for term feature
            {
                 m_gtapp.SelectedObjects.Clear();
                feat =  m_gtapp.DataContext.GetDDCKeyObjects(iFNO, lFID, GTComponentGeometryConstants.gtddcgAllGeographic);

                for (int K = 0; K < feat.Count; K++)
                {
                    string t = feat[K].ComponentViewName.ToString();
                    if (feat[K].ComponentViewName == "VGC_MANHL_S" ||
                        feat[K].ComponentViewName == "VGC_PSGCON_S")
                    {
                        //2700 VGC_MANHL_S  
                        //2800 VGC_PSGCON_S
                        IGTWorldRange range = GTClassFactory.Create<IGTWorldRange>();
                        IGTPoint point1 = GTClassFactory.Create<IGTPoint>();
                        IGTPoint point2 = GTClassFactory.Create<IGTPoint>();
                        point1.X = feat[K].Geometry.FirstPoint.X - 3;
                        point1.Y = feat[K].Geometry.FirstPoint.Y - 3;
                        range.BottomLeft = point1;
                        point2.X = feat[K].Geometry.FirstPoint.X + 3;
                        point2.Y = feat[K].Geometry.FirstPoint.Y + 3;
                        range.TopRight = point2;
                        window.ZoomArea(range);
                         m_gtapp.RefreshWindows();
                         m_gtapp.SelectedObjects.Clear();
                        return;
                    }

                    if (feat[K].ComponentViewName == "VGC_CHAMBER_P" ||
                        feat[K].ComponentViewName == "VGC_TUNNEL_P")
                    {
                        //3800 VGC_CHAMBER_P
                        //3300 VGC_TUNNEL_P
                        for (int K2 = 0; K2 < feat.Count; K2++)
                             m_gtapp.SelectedObjects.Add(GTSelectModeConstants.gtsosmAllDisplayedComponentsInActiveLegend, feat[K2]);
                        window.FitSelectedObjects();
                        window.CenterSelectedObjects();
                         m_gtapp.RefreshWindows();
                         m_gtapp.SelectedObjects.Clear();
                        return;
                    }
                }

            }
            //copy term feature to selected obj to fit both source and term
            feat =  m_gtapp.DataContext.GetDDCKeyObjects(iFNO, lFID, GTComponentGeometryConstants.gtddcgAllGeographic);
            for (int K = 0; K < feat.Count; K++)
                 m_gtapp.SelectedObjects.Add(GTSelectModeConstants.gtsosmAllDisplayedComponentsInActiveLegend, feat[K]);

            window.CenterSelectedObjects();
            window.FitSelectedObjects();
             m_gtapp.RefreshWindows();
              m_gtapp.SelectedObjects.Clear();
        }

        #endregion
    }


}
