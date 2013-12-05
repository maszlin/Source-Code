/*
 * Manhole placement - Modeless Custom Command
 * 
 * Author: La Viet Phuong - AG
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

namespace AG.GTechnology.ManholeDemolish
{
    class GTManholeDemolish : Intergraph.GTechnology.Interfaces.IGTCustomCommandModeless
    {
        private Intergraph.GTechnology.API.IGTTransactionManager m_oGTTransactionManager;
        private Intergraph.GTechnology.API.IGTCustomCommandHelper m_oGTCustomCommandHelper;
        private static Intergraph.GTechnology.API.IGTApplication application = GTClassFactory.Create<IGTApplication>();
        private Logger log;
        private IGTDataContext oDataContext = application.DataContext;
        private static IGTPolylineGeometry oLineGeom = null;
        private static List<IGTGeometry> arrIntersection = new List<IGTGeometry>();
        private static List<IGTGeometry> arrIntersectionText = new List<IGTGeometry>();
        private string mstrFistPoint = "Please enter the attributes for Manhole.";
        private int lStyleId = 11508;

        bool mblnVisible = false;
        int mintState;
        //IGTGeometryCreationService mobjCreationService;
        //IGTFeaturePlacementService mobjPlacementService;
        IGTGeometryEditService mobjEditService;
        IGTGeometryEditService mobjEditPointService;
        IGTGeometryEditService mobjEditServiceTemp;
        IGTLocateService mobjLocateService;
        IGTFeatureExplorerService mobjExplorerService;
        //IGTKeyObject mobjContourFeature;
        IGTPoint mptSelected;
        IGTPoint mptPoint1;
        IGTPoint mptPoint2;
        IGTKeyObject mobjManholeAttribute = null;

        List<DuctpathCtrl> lstDuctpathIPID = new List<DuctpathCtrl>();

        private short iManholeFNO = 2700;
        private int iManholeFID = 0;
        private IGTKeyObject oManhole = null;
        private IGTGeometry iManholeGeom = null;
        private ManholeCtrl cManhole = null;
        private int numberofWall = 0;
        private int currentPath = 0;
        private int currentNest = 0;
        private string ManholeType = string.Empty;

        private int StyleManhole = 2730001;
        private int StyleTempManhole = 2730001;

        Dictionary<string, string> Parameters = new Dictionary<string, string>();

        double _distance = 0;

        private void LoadManholeType(IGTKeyObject _attr)
        {
            string ManholeState = "ASB";
            string ManholeType = "JC9";
            if (!_attr.Components.GetComponent(51).Recordset.EOF)
            {
                ManholeState = _attr.Components.GetComponent(51).Recordset.Fields["FEATURE_STATE"].Value.ToString();
            }
            if (!_attr.Components.GetComponent(2701).Recordset.EOF)
            {
                ManholeType = _attr.Components.GetComponent(2701).Recordset.Fields["FEATURE_TYPE"].Value.ToString();
            }

            //if (ManholeState != "ASB")
            //{
            //    iManholeFID = 0;
            //    return;
            //}

            cManhole = new ManholeCtrl();
            cManhole.ManholeType = ManholeType;
            cManhole.FID = _attr.FID;
            cManhole.Manhole = _attr;
            cManhole.LoadManholeType();
        }

        private void LoadDuctPath()
        {
            Recordset rsComp = null;
            int recordsAffected = 0;
            int iFID = 0;

            lstDuctpathIPID.Clear();
            string sSql = "select b.G3E_FID, b.DT_ND_FRM_ID, b.DT_ND_TO_ID from GC_COND b where b.DT_ND_FRM_ID=" + iManholeFID + " OR DT_ND_TO_ID=" + iManholeFID;
            rsComp = application.DataContext.Execute(sSql, out recordsAffected, (int)CommandTypeEnum.adCmdText, null);

            if (rsComp != null)
            {
                while (!rsComp.EOF)
                {
                    DuctpathCtrl cDuctpath = new DuctpathCtrl();
                    if (rsComp.Fields["G3E_FID"].Value != DBNull.Value) cDuctpath.FID = Convert.ToInt32(rsComp.Fields["G3E_FID"].Value);
                    if (rsComp.Fields["DT_ND_FRM_ID"].Value != DBNull.Value) cDuctpath.FromFID = Convert.ToInt32(rsComp.Fields["DT_ND_FRM_ID"].Value);
                    if (rsComp.Fields["DT_ND_TO_ID"].Value != DBNull.Value) cDuctpath.ToFID = Convert.ToInt32(rsComp.Fields["DT_ND_TO_ID"].Value);

                    cDuctpath.LoadPoint(iManholeFID);
                    lstDuctpathIPID.Add(cDuctpath);

                    rsComp.MoveNext();
                }
            }
            rsComp = null;
        }

        private void CreateManholeGeom(IGTPoint objPoint, IGTPoint objPointRotate, IGTGeometryEditService mobjEditService)
        {
            CreateManholeGeom(objPoint, objPointRotate, mobjEditService, StyleManhole);

            //return oOrPointGeom;
        }

        private void CreateManholeGeom(IGTPoint objPoint, IGTPoint objPointRotate, IGTGeometryEditService mobjEditService, int StyleId)
        {
            IGTOrientedPointGeometry oOrPointGeom = PGeoLib.CreatePointGeom(objPoint.X, objPoint.Y);

            //if (mobjEditService.GeometryCount > 0) mobjEditService.RemoveAllGeometries();
            if (StyleId == 0) StyleId = StyleManhole;
            mobjEditService.AddGeometry(oOrPointGeom, StyleId);

            if (objPointRotate != null)
            {
                IGTPoint objPointTmp = GTClassFactory.Create<IGTPoint>();
                objPointTmp.X = objPoint.X + 100;
                objPointTmp.Y = objPoint.Y;

                mobjEditService.BeginRotate(objPointTmp, objPoint);
                mobjEditService.Rotate(objPointRotate);
                mobjEditService.EndRotate(objPointRotate);
                //oOrPointGeom = (IGTOrientedPointGeometry)mobjEditService.GetGeometry(1);
            }

            //return oOrPointGeom;
        }

        private IGTKeyObject UpdateDuctpath(DuctpathCtrl Ductpath, double x, double y, string manholewall)
        {
            short iFNO;
            short iCNO;
            long lFID;
            double dRotation;
            IGTKeyObject oNewFeature;

            IGTPoint oPoint = GTClassFactory.Create<IGTPoint>();
            oPoint.X = x;
            oPoint.Y = y;

            oNewFeature = application.DataContext.OpenFeature(2200, Ductpath.FID);
            oManhole = application.DataContext.OpenFeature(2700, cManhole.FID);

            IGTRelationshipService mobjRelationshipService;
            mobjRelationshipService = GTClassFactory.Create<IGTRelationshipService>(m_oGTCustomCommandHelper);
            mobjRelationshipService.DataContext = application.DataContext;

            //mobjRelationshipService.ActiveFeature = oNewFeature;
            //mobjRelationshipService.SilentDelete(1);

            iCNO = 2210;
            IGTGeometry oOldLine = (IGTGeometry)oNewFeature.Components.GetComponent(iCNO).Geometry;
            IGTPolylineGeometry oNewLine = GTClassFactory.Create<IGTPolylineGeometry>();
            for (int i = 0; i < oOldLine.KeypointCount; i++)
            {
                if (Ductpath.ConnectType == 0)
                {
                    if (i == 0)
                    {
                        oNewLine.Points.Add(oPoint);
                    }
                    else
                    {
                        oNewLine.Points.Add(oOldLine.GetKeypointPosition(i));
                    }
                }
                else
                {
                    if (i == oOldLine.KeypointCount - 1)
                    {
                        oNewLine.Points.Add(oPoint);
                    }
                    else
                    {
                        oNewLine.Points.Add(oOldLine.GetKeypointPosition(i));
                    }
                }
            }
            oNewFeature.Components.GetComponent(iCNO).Geometry = oNewLine;

            double newLen = PGeoLib.GetLength(oNewLine);
            double oldLen = 0;
            iCNO = 2201;
            if (oNewFeature.Components.GetComponent(iCNO).Recordset.Fields["TOTAL_LENGTH"].Value != DBNull.Value) oldLen = Convert.ToDouble(oNewFeature.Components.GetComponent(iCNO).Recordset.Fields["TOTAL_LENGTH"].Value);
            if (Ductpath.ConnectType == 0)
            {
                oNewFeature.Components.GetComponent(iCNO).Recordset.Update("DT_ND_FRM_ID", cManhole.FID);
                oNewFeature.Components.GetComponent(iCNO).Recordset.Update("DT_MH_FRM_WALL", manholewall);

                if (!oNewFeature.Components.GetComponent(2202).Recordset.EOF)
                {
                    oNewFeature.Components.GetComponent(2202).Recordset.Sort = "G3E_CID ASC";
                    double dLen = 0;
                    if (oNewFeature.Components.GetComponent(2202).Recordset.Fields["DT_S_LENGTH"].Value != DBNull.Value) dLen = Convert.ToDouble(oNewFeature.Components.GetComponent(2202).Recordset.Fields["DT_S_LENGTH"].Value);
                    dLen = dLen + (newLen - oldLen);
                    if (dLen < 0) dLen = 0;
                    oNewFeature.Components.GetComponent(2202).Recordset.Update("DT_S_LENGTH", Math.Round(dLen));
                }

                mobjRelationshipService.ActiveFeature = oNewFeature;
                if (mobjRelationshipService.AllowSilentEstablish(oManhole))
                {
                    mobjRelationshipService.SilentEstablish(1, oManhole, GTRelationshipOrdinalConstants.gtrelRelationshipOrdinal1);
                }
            }
            else
            {
                oNewFeature.Components.GetComponent(iCNO).Recordset.Update("DT_ND_TO_ID", cManhole.FID);
                oNewFeature.Components.GetComponent(iCNO).Recordset.Update("DT_MH_TO_WALL", manholewall);

                if (!oNewFeature.Components.GetComponent(2202).Recordset.EOF)
                {
                    oNewFeature.Components.GetComponent(2202).Recordset.Sort = "G3E_CID DESC";
                    double dLen = 0;
                    if (oNewFeature.Components.GetComponent(2202).Recordset.Fields["DT_S_LENGTH"].Value != DBNull.Value) dLen = Convert.ToDouble(oNewFeature.Components.GetComponent(2202).Recordset.Fields["DT_S_LENGTH"].Value);
                    dLen = dLen + (newLen - oldLen);
                    if (dLen < 0) dLen = 0;
                    oNewFeature.Components.GetComponent(2202).Recordset.Update("DT_S_LENGTH", Math.Round(dLen));
                }

                mobjRelationshipService.ActiveFeature = oNewFeature;
                if (mobjRelationshipService.AllowSilentEstablish(oManhole))
                {
                    mobjRelationshipService.SilentEstablish(1, oManhole, GTRelationshipOrdinalConstants.gtrelRelationshipOrdinal2);
                }
            }
            oNewFeature.Components.GetComponent(iCNO).Recordset.Update("TOTAL_LENGTH", newLen);

            return oNewFeature;
        }

        private IGTKeyObject MoveFormation(DuctnestCtrl ductnest, IGTPoint newLocation)
        {
            short iFNO;
            short iCNO;
            int lFID;
            IGTKeyObject oNewFeature;

            IGTPoint oPointTmp = null;

            iFNO = 2400;
            lFID = ductnest.FID;
            oNewFeature = application.DataContext.OpenFeature(iFNO, lFID);

            //Line
            if (ductnest.ConnectType == 0)
                iCNO = 2410;
            else
                iCNO = 2412;

            IGTPoints nestPoints = ((IGTPolylineGeometry) oNewFeature.Components.GetComponent(iCNO).Geometry).Points;
            IGTPoint oPoint = oNewFeature.Components.GetComponent(iCNO).Geometry.FirstPoint;
            double len = PGeoLib.GetLength(oPoint, newLocation);
            double angle = PGeoLib.GetAngle(oPoint, newLocation);

            IGTVector vector = GTClassFactory.Create<IGTVector>();
            vector.I = len * Math.Cos(angle);
            vector.J = len * Math.Sin(angle);
            
            PGeoLib.MoveComponent(oNewFeature, iCNO, vector);

            //Label
            if (ductnest.ConnectType == 0)
                iCNO = 2430;
            else
                iCNO = 2432;
            PGeoLib.MoveComponent(oNewFeature, iCNO, vector);

            IGTKeyObject oDuctFeature;

            Recordset rsComp = null;
            int recordsAffected = 0;
            int iFID = 0;

            string sSql = "select b.G3E_FID from GC_CONTAIN b where b.G3E_OWNERFID=" + lFID.ToString() + " AND b.G3E_FNO=2300";
            rsComp = application.DataContext.Execute(sSql, out recordsAffected, (int)CommandTypeEnum.adCmdText, null);

            if (rsComp != null)
            {
                while (!rsComp.EOF)
                {
                    DuctnestCtrl cDuctnest = new DuctnestCtrl();
                    if (rsComp.Fields["G3E_FID"].Value != DBNull.Value) iFID = Convert.ToInt32(rsComp.Fields["G3E_FID"].Value);

                    iFNO = 2300;
                    oDuctFeature = application.DataContext.OpenFeature(iFNO, iFID);

                    //Symbol
                    if (ductnest.ConnectType == 0)
                        iCNO = 2320;
                    else
                        iCNO = 2322;
                    PGeoLib.MoveComponent(oDuctFeature, iCNO, vector);

                    rsComp.MoveNext();
                }
            }
            rsComp = null;

            sSql = "select a.G3E_FID from GC_CONTAIN b, GC_CONTAIN a where a.G3E_OWNERFID=b.G3E_FID AND a.G3E_FNO=16100 AND b.G3E_OWNERFID=" + lFID.ToString() + " AND b.G3E_FNO=2300";
            rsComp = application.DataContext.Execute(sSql, out recordsAffected, (int)CommandTypeEnum.adCmdText, null);

            if (rsComp != null)
            {
                while (!rsComp.EOF)
                {
                    DuctnestCtrl cDuctnest = new DuctnestCtrl();
                    if (rsComp.Fields["G3E_FID"].Value != DBNull.Value) iFID = Convert.ToInt32(rsComp.Fields["G3E_FID"].Value);

                    iFNO = 16100;
                    oDuctFeature = application.DataContext.OpenFeature(iFNO, iFID);

                    //Symbol
                    if (ductnest.ConnectType == 0)
                        iCNO = 16120;
                    else
                        iCNO = 16122;
                    PGeoLib.MoveComponent(oDuctFeature, iCNO, vector);

                    rsComp.MoveNext();
                }
            }
            rsComp = null;

            //Cable symbol
            List<int> Cable = new List<int>();
            sSql = "select a.G3E_FID from GC_CONTAIN b, GC_CONTAIN a where a.G3E_OWNERFID=b.G3E_FID AND a.G3E_FNO=7000 AND b.G3E_OWNERFID=" + lFID.ToString() + " AND b.G3E_FNO=2300";
            rsComp = application.DataContext.Execute(sSql, out recordsAffected, (int)CommandTypeEnum.adCmdText, null);

            if (rsComp != null)
            {
                while (!rsComp.EOF)
                {
                    DuctnestCtrl cDuctnest = new DuctnestCtrl();
                    if (rsComp.Fields["G3E_FID"].Value != DBNull.Value)
                    {
                        iFID = Convert.ToInt32(rsComp.Fields["G3E_FID"].Value);

                        iFNO = 7000;
                        Cable.Add(iFID);
                    }

                    rsComp.MoveNext();
                }
            }
            rsComp = null;

            //nestPoints.Add(nestPoints[0]);
            for (int i = 0; i < Cable.Count; i++)
            {
                iFNO = 7000;
                iFID = Cable[i];
                oDuctFeature = application.DataContext.OpenFeature(iFNO, iFID);
                while(!oDuctFeature.Components.GetComponent(7020).Recordset.EOF)
                {
                    oPointTmp = oDuctFeature.Components.GetComponent(7020).Geometry.FirstPoint;
                    if (oPointTmp.IsContainedBy(nestPoints) == GTInPolygonType.gtiptInside)
                    {
                        IGTGeometry geom = oDuctFeature.Components.GetComponent(7020).Geometry;
                        geom.Move(vector);

                        oDuctFeature.Components.GetComponent(7020).Geometry = geom;
                    }
                    oDuctFeature.Components.GetComponent(7020).Recordset.MoveNext();
                }

                while (!oDuctFeature.Components.GetComponent(7036).Recordset.EOF)
                {
                    oPointTmp = oDuctFeature.Components.GetComponent(7036).Geometry.FirstPoint;
                    if (oPointTmp.IsContainedBy(nestPoints) == GTInPolygonType.gtiptInside)
                    {
                        IGTGeometry geom = oDuctFeature.Components.GetComponent(7036).Geometry;
                        geom.Move(vector);

                        oDuctFeature.Components.GetComponent(7036).Geometry = geom;
                    }
                    oDuctFeature.Components.GetComponent(7036).Recordset.MoveNext();
                }
            }
            return oNewFeature;
        }

        private IGTKeyObject MoveManhole(int lFID, IGTPoint newLocation)
        {
            short iFNO;
            short iCNO;
            double dRotation;
            IGTKeyObject oNewFeature;

            IGTTextPointGeometry oOrTextGeom;
            IGTOrientedPointGeometry oOrPointGeom = null;
            IGTPoint oPointTmp = null;

            iFNO = iManholeFNO;
            oNewFeature = application.DataContext.OpenFeature(iFNO, lFID);

            //Symbol
            iCNO = 2720;

            IGTPoint oPoint = oNewFeature.Components.GetComponent(iCNO).Geometry.FirstPoint;
            double len = PGeoLib.GetLength(oPoint, newLocation);
            double angle = PGeoLib.GetAngle(oPoint, newLocation);

            IGTVector vector = GTClassFactory.Create<IGTVector>();
            vector.I = len * Math.Cos(angle);
            vector.J = len * Math.Sin(angle);

            PGeoLib.MoveFeature(oNewFeature, vector);
            
            //if (oNewFeature.Components.GetComponent(iCNO).Recordset.EOF)
            //{
            //    oNewFeature.Components.GetComponent(iCNO).Recordset.AddNew("G3E_FID", lFID);
            //    oNewFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_FNO", iFNO);
            //}
            //else
            //{
            //    oNewFeature.Components.GetComponent(iCNO).Recordset.MoveLast();
            //}

            //// GetCompInfo iFNO, iCNO, bMainGr, grType
            //IGTOrientedPointGeometry fromSymbol = (IGTOrientedPointGeometry)oNewFeature.Components.GetComponent(iCNO).Geometry;
            //IGTPoint oPoint = fromSymbol.Origin;
            //oOrPointGeom = PGeoLib.CreatePointGeom(newLocation.X, newLocation.Y);
            //oOrPointGeom.Orientation = fromSymbol.Orientation;

            //oNewFeature.Components.GetComponent(iCNO).Geometry = oOrPointGeom;

            ////Leader line
            //iCNO = 2712;

            //while (!oNewFeature.Components.GetComponent(iCNO).Recordset.EOF)
            //{
            //    // GetCompInfo iFNO, iCNO, bMainGr, grType
            //    IGTGeometry fromLeader = oNewFeature.Components.GetComponent(iCNO).Geometry;
            //    IGTPolylineGeometry oLine = GTClassFactory.Create<IGTPolylineGeometry>();

            //    for (int i = 0; i < fromLeader.KeypointCount; i++)
            //    {
            //        IGTPoint oPointLine = fromLeader.GetKeypointPosition(i);
            //        oPointLine.X = newLocation.X + (oPointLine.X - oPoint.X);
            //        oPointLine.Y = newLocation.Y + (oPointLine.Y - oPoint.Y);
            //        oLine.Points.Add(oPointLine);
            //    }

            //    oNewFeature.Components.GetComponent(iCNO).Geometry = oLine;
            //    oNewFeature.Components.GetComponent(iCNO).Recordset.MoveNext();
            //}

            ////Wall number
            //iCNO = 2732;
            //while (!oNewFeature.Components.GetComponent(iCNO).Recordset.EOF)
            //{
            //    IGTOrientedPointGeometry txtFrom = (IGTOrientedPointGeometry)oNewFeature.Components.GetComponent(iCNO).Geometry;
            //    oPointTmp = GTClassFactory.Create<IGTPoint>();
            //    oPointTmp.X = newLocation.X + (txtFrom.FirstPoint.X - oPoint.X);
            //    oPointTmp.Y = newLocation.Y + (txtFrom.FirstPoint.Y - oPoint.Y);
            //    IGTOrientedPointGeometry txtNumber = PGeoLib.CreatePointGeom(oPointTmp.X, oPointTmp.Y);
            //    txtNumber.Orientation = txtFrom.Orientation;

            //    oNewFeature.Components.GetComponent(iCNO).Geometry = txtNumber;

            //    oNewFeature.Components.GetComponent(iCNO).Recordset.MoveNext();
            //}

            ////Label
            //iCNO = 2730;

            //while (!oNewFeature.Components.GetComponent(iCNO).Recordset.EOF)
            //{
            //    IGTOrientedPointGeometry txtFromLabel = (IGTOrientedPointGeometry)oNewFeature.Components.GetComponent(iCNO).Geometry;
            //    oPointTmp = GTClassFactory.Create<IGTPoint>();
            //    oPointTmp.X = newLocation.X + (txtFromLabel.FirstPoint.X - oPoint.X);
            //    oPointTmp.Y = newLocation.Y + (txtFromLabel.FirstPoint.Y - oPoint.Y);
            //    IGTOrientedPointGeometry txtLabel = PGeoLib.CreatePointGeom(oPointTmp.X, oPointTmp.Y);
            //    txtLabel.Orientation = txtFromLabel.Orientation;

            //    oNewFeature.Components.GetComponent(iCNO).Geometry = txtLabel;
            //    oNewFeature.Components.GetComponent(iCNO).Recordset.MoveNext();
            //}

            return oNewFeature;
        }

        private IGTKeyObject CopyManhole(IGTKeyObject fromManhole, IGTPoint newLocation)
        {
            short iFNO;
            short iCNO;
            long lFID;
            double dRotation;
            IGTKeyObject oNewFeature;

            IGTTextPointGeometry oOrTextGeom;
            IGTOrientedPointGeometry oOrPointGeom = null;
            IGTPoint oPointTmp = null;

            if (fromManhole == null) return null;

            iFNO = iManholeFNO;
            oNewFeature = application.DataContext.NewFeature(iFNO);

            // FID generation.
            lFID = oNewFeature.FID;

            if (fromManhole != null)
                PGeoLib.CopyComponentAttribute(fromManhole, oNewFeature);
            else
            {
                // Every feature is imported in the Existing state.
                iCNO = 51; //GC_NETELEM
                if (oNewFeature.Components.GetComponent(iCNO).Recordset.EOF)
                {
                    oNewFeature.Components.GetComponent(iCNO).Recordset.AddNew("G3E_FID", lFID);
                    oNewFeature.Components.GetComponent(iCNO).Recordset.Update("JOB_STATE", "CLD");
                    //oNewFeature.Components.GetComponent(iCNO).Recordset.Update("FEATURE_STATE", "EXT");
                    //oNewFeature.Components.GetComponent(iCNO).Recordset.Update("SWITCH_CENTRE_CLLI", "PPJ");
                }
                else
                {
                    //oNewFeature.Components.GetComponent(iCNO).Recordset.Update("JOB_STATE", "CLD");
                    //oNewFeature.Components.GetComponent(iCNO).Recordset.Update("FEATURE_STATE", "EXT");
                    //oNewFeature.Components.GetComponent(iCNO).Recordset.Update("SWITCH_CENTRE_CLLI", "PPJ");
                }

                // Attribute
                iCNO = 2701;
                //oNewFeature.Components.GetComponent(iCNO).Recordset.Update("FEATURE_TYPE", "JC9");
            }

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
            IGTOrientedPointGeometry fromSymbol = (IGTOrientedPointGeometry)fromManhole.Components.GetComponent(iCNO).Geometry;
            IGTPoint oPoint = fromSymbol.Origin;
            oOrPointGeom = PGeoLib.CreatePointGeom(newLocation.X, newLocation.Y);
            oOrPointGeom.Orientation = fromSymbol.Orientation;

            oNewFeature.Components.GetComponent(iCNO).Geometry = oOrPointGeom;

            //Leader line
            iCNO = 2712;

            if (!fromManhole.Components.GetComponent(iCNO).Recordset.EOF)
            {
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
                IGTGeometry fromLeader = fromManhole.Components.GetComponent(iCNO).Geometry;
                IGTPolylineGeometry oLine = GTClassFactory.Create<IGTPolylineGeometry>();

                for (int i = 0; i < fromLeader.KeypointCount; i++)
                {
                    IGTPoint oPointLine = fromLeader.GetKeypointPosition(i);
                    oPointLine.X = newLocation.X + (oPointLine.X - oPoint.X);
                    oPointLine.Y = newLocation.Y + (oPointLine.Y - oPoint.Y);
                    oLine.Points.Add(oPointLine);
                }

                oNewFeature.Components.GetComponent(iCNO).Geometry = oLine;
            }

            //Wall number
            iCNO = 2732;
            while (!fromManhole.Components.GetComponent(iCNO).Recordset.EOF)
            {
                IGTOrientedPointGeometry txtFrom = (IGTOrientedPointGeometry)fromManhole.Components.GetComponent(iCNO).Geometry;
                oPointTmp = GTClassFactory.Create<IGTPoint>();
                oPointTmp.X = newLocation.X + (txtFrom.FirstPoint.X - oPoint.X);
                oPointTmp.Y = newLocation.Y + (txtFrom.FirstPoint.Y - oPoint.Y);
                IGTOrientedPointGeometry txtNumber = PGeoLib.CreatePointGeom(oPointTmp.X, oPointTmp.Y);
                txtNumber.Orientation = txtFrom.Orientation;

                oNewFeature.Components.GetComponent(iCNO).Recordset.AddNew("G3E_FID", lFID);
                oNewFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_FNO", iFNO);
                oNewFeature.Components.GetComponent(iCNO).Recordset.Update("WALL_NUM", fromManhole.Components.GetComponent(iCNO).Recordset.Fields["WALL_NUM"].Value);
                oNewFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_ALIGNMENT", fromManhole.Components.GetComponent(iCNO).Recordset.Fields["G3E_ALIGNMENT"].Value);
                oNewFeature.Components.GetComponent(iCNO).Geometry = txtNumber;

                fromManhole.Components.GetComponent(iCNO).Recordset.MoveNext();
            }

            //Label
            iCNO = 2730;

            if (!fromManhole.Components.GetComponent(iCNO).Recordset.EOF)
            {
                IGTOrientedPointGeometry txtFromLabel = (IGTOrientedPointGeometry)fromManhole.Components.GetComponent(iCNO).Geometry;
                oPointTmp = GTClassFactory.Create<IGTPoint>();
                oPointTmp.X = newLocation.X + (txtFromLabel.FirstPoint.X - oPoint.X);
                oPointTmp.Y = newLocation.Y + (txtFromLabel.FirstPoint.Y - oPoint.Y);
                IGTOrientedPointGeometry txtLabel = PGeoLib.CreatePointGeom(oPointTmp.X, oPointTmp.Y);
                txtLabel.Orientation = txtFromLabel.Orientation;

                if (oNewFeature.Components.GetComponent(iCNO).Recordset.EOF)
                {
                    oNewFeature.Components.GetComponent(iCNO).Recordset.AddNew("G3E_FID", lFID);
                    oNewFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_FNO", iFNO);
                }
                else
                {
                    oNewFeature.Components.GetComponent(iCNO).Recordset.MoveLast();
                }
                oNewFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_ALIGNMENT", fromManhole.Components.GetComponent(iCNO).Recordset.Fields["G3E_ALIGNMENT"].Value);

                oNewFeature.Components.GetComponent(iCNO).Geometry = txtLabel;
            }

            return oNewFeature;
        }

        private IGTKeyObject CreateManhole(ManholeCtrl ctlManhole, IGTGeometryEditService mobjEditService)
        {
            short iFNO;
            short iCNO;
            long lFID;
            double dRotation;
            IGTKeyObject oNewFeature;

            IGTTextPointGeometry oOrTextGeom;
            IGTOrientedPointGeometry oOrPointGeom = null;

            if (ctlManhole == null) return null;
            if (mobjEditService == null) return null;

            IGTPoint oPoint = ctlManhole.Origin;
            IGTPoint oPointRotate = ctlManhole.RotationPnt;

            iFNO = iManholeFNO;
            oNewFeature = application.DataContext.NewFeature(iFNO);

            // FID generation.
            lFID = oNewFeature.FID;

            if (mobjManholeAttribute != null)
                PGeoLib.CopyComponentAttribute(mobjManholeAttribute, oNewFeature);
            else
            {
                // Every feature is imported in the Existing state.
                iCNO = 51; //GC_NETELEM
                if (oNewFeature.Components.GetComponent(iCNO).Recordset.EOF)
                {
                    oNewFeature.Components.GetComponent(iCNO).Recordset.AddNew("G3E_FID", lFID);
                    oNewFeature.Components.GetComponent(iCNO).Recordset.Update("JOB_STATE", "CLD");
                    //oNewFeature.Components.GetComponent(iCNO).Recordset.Update("FEATURE_STATE", "EXT");
                    //oNewFeature.Components.GetComponent(iCNO).Recordset.Update("SWITCH_CENTRE_CLLI", "PPJ");
                }
                else
                {
                    oNewFeature.Components.GetComponent(iCNO).Recordset.Update("JOB_STATE", "CLD");
                    //oNewFeature.Components.GetComponent(iCNO).Recordset.Update("FEATURE_STATE", "EXT");
                    //oNewFeature.Components.GetComponent(iCNO).Recordset.Update("SWITCH_CENTRE_CLLI", "PPJ");
                }

                // Attribute
                iCNO = 2701;
                //oNewFeature.Components.GetComponent(iCNO).Recordset.Update("FEATURE_TYPE", "JC9");
            }

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
            int fNbr = ctlManhole.FirstNumber;
            if (ctlManhole.ManholeWall != null)
            {
                for (int i = 0; i < ctlManhole.ManholeWall.Count; i++)
                {
                    if (fNbr >= ctlManhole.ManholeWall.Count) fNbr = 0;
                    ManholePoint _pnt = ctlManhole.ManholeWall[fNbr];
                    IGTTextPointGeometry txtNumber = PGeoLib.CreateTextGeom(ctlManhole.Origin.X + _pnt.X, ctlManhole.Origin.Y + _pnt.Y, Convert.ToString(i + 1));

                    IGTPoint objPointTmp = GTClassFactory.Create<IGTPoint>();
                    objPointTmp.X = oPoint.X + 100;
                    objPointTmp.Y = oPoint.Y;

                    if (mobjEditService.GeometryCount > 0) mobjEditService.RemoveAllGeometries();
                    mobjEditService.AddGeometry(txtNumber, 30700);
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
            IGTTextPointGeometry txtLabel = PGeoLib.CreateTextGeom(ctlManhole.LabelOrigin.X, ctlManhole.LabelOrigin.Y, "[Manhole label]", 0, GTAlignmentConstants.gtalTopLeft);

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

            return oNewFeature;
        }

        public void Activate(Intergraph.GTechnology.API.IGTCustomCommandHelper CustomCommandHelper)
        {
            try
            {
                application.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, mstrFistPoint);

                log = Logger.getInstance();
                log.WriteLog(this.GetType().FullName, "Manhole Demolish/Reconstruction started!");
                log.WriteLog("");

                m_oGTCustomCommandHelper = CustomCommandHelper;

                mobjEditService = GTClassFactory.Create<IGTGeometryEditService>();
                mobjEditService.TargetMapWindow = application.ActiveMapWindow;

                mobjEditPointService = GTClassFactory.Create<IGTGeometryEditService>();
                mobjEditPointService.TargetMapWindow = application.ActiveMapWindow;

                mobjEditServiceTemp = GTClassFactory.Create<IGTGeometryEditService>();
                mobjEditServiceTemp.TargetMapWindow = application.ActiveMapWindow;

                mobjLocateService = application.ActiveMapWindow.LocateService;

                mobjExplorerService = GTClassFactory.Create<IGTFeatureExplorerService>();

                RegisterEvents();

                mintState = 1;
                //  Assigns the private member variables their default values.
                if (application.SelectedObjects.FeatureCount > 0)
                {
                    foreach (IGTDDCKeyObject oDDCKeyObject in application.SelectedObjects.GetObjects())
                    {
                        if ((oDDCKeyObject.ComponentViewName == "VGC_MANHL_S"))
                        {
                            if (oDDCKeyObject.Recordset.Fields["FEATURE_STATE"].Value.ToString() == "ASB")
                            {
                                iManholeGeom = oDDCKeyObject.Geometry;
                                iManholeFNO = oDDCKeyObject.FNO;
                                iManholeFID = oDDCKeyObject.FID;
                            }
                            else
                            {
                                mintState = 1;
                                MessageBox.Show("Selected manhole must be in 'ASB' state", "Manhole demolish/recontruction", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                                return;
                            }
                        }
                    }

                    if (iManholeFID > 0)
                    {
                        mintState = -1;
                        application.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Loading manhole information, please wait....");
                        oManhole = application.DataContext.OpenFeature(iManholeFNO, iManholeFID);

                        LoadManholeType(oManhole);
                        LoadDuctPath();
                        application.ActiveMapWindow.HighlightedObjects.Clear();
                        application.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Point to enter new location or right-click to accept current location");
                        mintState = 2;
                    }
                }
                else
                    mintState = 1;
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
                DialogResult msg = MessageBox.Show("Do you want to to discard your current changes?", "Message", MessageBoxButtons.YesNo);
                if (msg == DialogResult.Yes)
                    return true;
                else
                    return false;
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

                application.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Exiting...");

                UnRegisterEvents();

                log.CloseFile();
                if (m_oGTTransactionManager != null)
                {
                    if (m_oGTTransactionManager.TransactionInProgress)
                    {
                        m_oGTTransactionManager.Rollback();
                    }
                }

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
                if (mobjEditServiceTemp != null)
                {
                    mobjEditServiceTemp.RemoveAllGeometries();
                    mobjEditServiceTemp = null;
                }
                if (cManhole != null)
                {
                    cManhole.Dispose();
                    cManhole = null;
                }

                CloseFeatureExplorer();

                //if (!(mobjExplorerService == null))
                //{
                //    mobjExplorerService.Clear();
                //}

                //if ((!(mobjExplorerService == null)
                //            && !mblnVisible))
                //{
                //    mobjExplorerService.Visible = false;
                //}
                //mobjCreationService = null;
                //mobjPlacementService = null;
                mptSelected = null;

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
                application.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Exiting...");
                if (m_oGTCustomCommandHelper != null) m_oGTCustomCommandHelper.Complete();
                application.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Exited.");
            }
            catch (Exception ex)
            {
                //Logger log = Logger.GetInstance();
                log.WriteLog("Error", "ExitCmd", ex.Message);
                log.WriteLog(ex.StackTrace);
                //MessageBox.Show(ex.Message, "Error.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //log.CloseFile();
            }
        }

        private void m_oCustomCommandHelper_MouseUp(object sender, GTMouseEventArgs MouseEventArgs)
        {

            try
            {
                IGTPoint WorldPoint = MouseEventArgs.WorldPoint;

                if (MouseEventArgs.Button == 2)
                {
                    if (mintState == 31)
                    {
                        application.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Enter new manhole attribute");

                        string mstrTransactionName = "Enter Manhole Attribute";
                        m_oGTTransactionManager.Begin(mstrTransactionName);

                        mobjManholeAttribute = application.DataContext.NewFeature(iManholeFNO, true);
                        PGeoLib.CopyComponentAttribute(oManhole, mobjManholeAttribute);

                        mobjExplorerService.ExploreFeature(mobjManholeAttribute, "Manhole");

                        mblnVisible = mobjExplorerService.Visible;

                        mobjExplorerService.Visible = true;
                        mobjExplorerService.Slide(true);
                        mintState = 32;
                    }
                    else if (mintState == 32)
                    {
                        return;
                    }
                    else if (mintState == 4)
                    {
                        mintState = -1;
                        double xManhole = WorldPoint.X;
                        double yManhole = WorldPoint.Y;


                        if (mobjEditPointService.GeometryCount > 0) mobjEditPointService.RemoveAllGeometries();
                        if (mobjEditServiceTemp.GeometryCount > 0) mobjEditServiceTemp.RemoveAllGeometries();

                        string mstrTransactionName = "Update Manhole Graphic";
                        m_oGTTransactionManager.Begin(mstrTransactionName);

                        oManhole = application.DataContext.OpenFeature(iManholeFNO, iManholeFID);

                        IGTPoint pnt = GTClassFactory.Create<IGTPoint>();

                        short iCNO = 2720;
                        if (mptSelected != null)
                        {
                            xManhole = mptSelected.X;
                            yManhole = mptSelected.Y;

                            IGTOrientedPointGeometry oldGeom = (IGTOrientedPointGeometry)oManhole.Components.GetComponent(iCNO).Geometry;
                            double oldX = oldGeom.Origin.X;
                            double oldY = oldGeom.Origin.Y;

                            pnt.X = xManhole;
                            pnt.Y = yManhole;
                            oldGeom.Origin = pnt;

                            oManhole.Components.GetComponent(iCNO).Geometry = oldGeom;

                            cManhole.X = xManhole;
                            cManhole.Y = yManhole;
                            cManhole.Origin = pnt;
                        }
                        else
                        {
                            mptSelected = cManhole.Origin;
                            pnt = cManhole.Origin;
                            xManhole = cManhole.X;
                            yManhole = cManhole.Y;
                        }

                        //Wall number
                        iCNO = 2732;
                        PGeoLib.DeleteComponent(iManholeFNO, iManholeFID, 2712);
                        PGeoLib.DeleteComponent(iManholeFNO, iManholeFID, 2730);
                        PGeoLib.DeleteComponent(iManholeFNO, iManholeFID, 2732);

                        m_oGTTransactionManager.Commit();
                        m_oGTTransactionManager.RefreshDatabaseChanges();
                        application.RefreshWindows();

                        if (mobjEditServiceTemp.GeometryCount > 0) mobjEditServiceTemp.RemoveAllGeometries();

                        IGTTextPointGeometry txtNumber = null;
                        //txtNumber = PGeoLib.CreateTextGeom(pnt.X, pnt.Y, "c");
                        //mobjEditServiceTemp.AddGeometry(txtNumber, 30700);

                        if (cManhole.ManholeWall != null)
                        {
                            int i = 0;
                            foreach (ManholePoint _pnt in cManhole.ManholeWall)
                            {
                                if (cManhole.Angle == 0)
                                {
                                    txtNumber = PGeoLib.CreateTextGeom(pnt.X + _pnt.X, pnt.Y + _pnt.Y, "+");
                                }
                                else
                                {
                                    IGTPoint objPointTmp = GTClassFactory.Create<IGTPoint>();
                                    double angle = cManhole.Angle;
                                    objPointTmp.X = pnt.X + _pnt.X;
                                    objPointTmp.Y = pnt.Y + _pnt.Y;

                                    //txtNumber = PGeoLib.CreateTextGeom(objPointTmp.X, objPointTmp.Y, "o");
                                    //mobjEditServiceTemp.AddGeometry(txtNumber, 30700);

                                    double txtX = PGeoLib.RotatePoint(pnt, angle, objPointTmp).X;
                                    double txtY = PGeoLib.RotatePoint(pnt, angle, objPointTmp).Y;

                                    Debug.Print("{0}: old {1}-{2} new {3}-{4}", i, objPointTmp.X, objPointTmp.Y, txtX, txtY);

                                    txtNumber = PGeoLib.CreateTextGeom(txtX, txtY, "+");
                                }
                                mobjEditServiceTemp.AddGeometry(txtNumber, 30700);
                                i++;
                            }
                            mintState = 5;
                        }
                        else
                            mintState = 6;

                        application.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Identify [+] to be wall 1");
                    }
                    else if (mintState == 5)
                    {
                        //mintState = -1;
                        //cManhole.X = mptPoint1.X;
                        //cManhole.Y = mptPoint1.Y;
                        //cManhole.Origin = mptPoint1;

                        ////if (mptPoint2 != null)
                        ////{
                        ////mobjEditService.Rotate(mptPoint2);
                        ////mobjEditService.EndRotate(mptPoint2);

                        //if (mobjEditPointService.GeometryCount > 0) mobjEditPointService.RemoveAllGeometries();
                        //if (mobjEditServiceTemp.GeometryCount > 0) mobjEditServiceTemp.RemoveAllGeometries();
                        //if (mobjEditService.GeometryCount > 0) mobjEditService.RemoveAllGeometries();

                        //if (cManhole.ManholeWall != null)
                        //{
                        //    foreach (ManholePoint _pnt in cManhole.ManholeWall)
                        //    {
                        //        IGTTextPointGeometry txtNumber = PGeoLib.CreateTextGeom(mptPoint1.X + _pnt.X, mptPoint1.Y + _pnt.Y, "+");
                        //        mobjEditService.AddGeometry(txtNumber, 30700);
                        //    }

                        //}
                        ////}

                        //CreateManholeGeom(mptPoint1, mptPoint2, mobjEditService);
                        //mintState = 6;
                        //if ((cManhole.ManholeWall == null) || (cManhole.ManholeWall.Count <= 0)) mintState = 7;
                    }
                    else if (mintState == 6)
                    {
                        mintState = -1;
                        mintState = 7;
                    }
                    else if (mintState == 7)
                    {
                        mintState = -1;

                        if (mptPoint2 == null) mptPoint2 = WorldPoint;
                        //Set text position
                        if (mobjEditServiceTemp.GeometryCount > 0) mobjEditServiceTemp.RemoveAllGeometries();
                        if (mobjEditPointService.GeometryCount > 0) mobjEditPointService.RemoveAllGeometries();

                        string mstrTransactionName = "Update Manhole Label";
                        m_oGTTransactionManager.Begin(mstrTransactionName);

                        oManhole = application.DataContext.OpenFeature(iManholeFNO, iManholeFID);

                        //Manhole label
                        short iCNO = 2730;
                        IGTTextPointGeometry txtNumber = PGeoLib.CreateTextGeom(mptPoint2.X, mptPoint2.Y, "[Manhole label]");

                        oManhole.Components.GetComponent(iCNO).Recordset.AddNew("G3E_FID", oManhole.FID);
                        oManhole.Components.GetComponent(iCNO).Recordset.Update("G3E_FNO", oManhole.FNO);
                        oManhole.Components.GetComponent(iCNO).Recordset.Update("G3E_ALIGNMENT", (int)GTAlignmentConstants.gtalBottomCenter);
                        oManhole.Components.GetComponent(iCNO).Geometry = txtNumber;

                        if (cManhole.Arrow != null)
                        {
                            iCNO = 2712;
                            IGTPolylineGeometry oLine = PGeoLib.CreateHeaderLine(cManhole.Arrow, mptPoint2);

                            oManhole.Components.GetComponent(iCNO).Recordset.AddNew("G3E_FID", oManhole.FID);
                            oManhole.Components.GetComponent(iCNO).Recordset.Update("G3E_FNO", oManhole.FNO);
                            oManhole.Components.GetComponent(iCNO).Geometry = oLine;
                        }

                        m_oGTTransactionManager.Commit();
                        m_oGTTransactionManager.RefreshDatabaseChanges();
                        application.RefreshWindows();

                        mintState = 8;
                        if (lstDuctpathIPID.Count == 0) mintState = 1;
                    }
                    else if (mintState == 8)
                    {
                    }
                    else if (mintState == 9)
                    {
                        DuctpathCtrl cDuctpath = lstDuctpathIPID[currentPath];
                        if (mobjEditServiceTemp.GeometryCount > 0) mobjEditServiceTemp.RemoveAllGeometries();

                        if ((cDuctpath.DuctNest == null) || (cDuctpath.DuctNest.Count == 0))
                        {
                            currentPath++;
                            if ((currentPath >= lstDuctpathIPID.Count) && (lstDuctpathIPID.Count > 0))
                                mintState = 1;
                            else
                                mintState = 8;
                        }
                        else
                        {
                            currentNest = 0;
                            mintState = 10;
                        }
                    }
                    else if (mintState == 10)
                    {
                    }
                    else
                    {
                        ExitCmd();
                        return;
                    }
                }
                else
                {
                    //  If the current step in the command is the third step then get the selected point.
                    if (mintState == 1)
                    {
                        double X = WorldPoint.X;
                        double Y = WorldPoint.Y;

                        iManholeFID = 0;
                        mptSelected = null;
                        IGTDDCKeyObjects objs = mobjLocateService.Locate(WorldPoint, 50, 10, GTSelectionTypeConstants.gtmwstSelectAll);

                        foreach (IGTDDCKeyObject oDDCKeyObject in objs)
                        {
                            if ((oDDCKeyObject.ComponentViewName == "VGC_MANHL_S"))
                            {
                                if (oDDCKeyObject.Recordset.Fields["FEATURE_STATE"].Value.ToString() == "ASB")
                                {
                                    iManholeGeom = oDDCKeyObject.Geometry;
                                    iManholeFNO = oDDCKeyObject.FNO;
                                    iManholeFID = oDDCKeyObject.FID;
                                }
                                else
                                {
                                    MessageBox.Show("Selected manhole must be in 'ASB' state", "Manhole demolish/recontruction", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                                    return;
                                }
                            }
                        }

                        if (iManholeFID > 0)
                        {
                            mintState = -1;
                            application.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Loading manhole information, please wait....");
                            oManhole = application.DataContext.OpenFeature(iManholeFNO, iManholeFID);

                            LoadManholeType(oManhole);
                            LoadDuctPath();
                            application.ActiveMapWindow.HighlightedObjects.Clear();
                            application.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Point to enter new location or right-click to accept current location");
                            mintState = 2;
                        }
                    }
                    else if (mintState == 2)
                    {
                        mintState = -1;

                        double xManhole = WorldPoint.X;
                        double yManhole = WorldPoint.Y;

                        if (mobjEditPointService.GeometryCount > 0) mobjEditPointService.RemoveAllGeometries();
                        if (mobjEditServiceTemp.GeometryCount > 0) mobjEditServiceTemp.RemoveAllGeometries();
                        if (mobjEditService.GeometryCount > 0) mobjEditService.RemoveAllGeometries();

                        application.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Updating demolised manhole, please wait....");
                        string mstrTransactionName = "Move demolished manhole";
                        m_oGTTransactionManager.Begin(mstrTransactionName);

                        IGTPoint pnt = GTClassFactory.Create<IGTPoint>();
                        pnt.X = xManhole;
                        pnt.Y = yManhole;

                        MoveManhole(cManhole.FID, pnt);

                        oManhole = application.DataContext.OpenFeature(2700, cManhole.FID);
                        oManhole.Components.GetComponent(51).Recordset.Update("FEATURE_STATE", "PDR");
                        //Anna's add
                        oManhole.Components.GetComponent(51).Recordset.Update("JOB_ID", application.DataContext.ActiveJob);
                        oManhole.Components.GetComponent(51).Recordset.Update("JOB_STATE", Get_Value("SELECT JOB_STATE FROM G3E_JOB WHERE G3E_IDENTIFIER ='" + application.DataContext.ActiveJob + "'"));
                        /////
                        short iCNO = 2710;
                        if (oManhole.Components.GetComponent(iCNO).Recordset.EOF)
                        {
                            oManhole.Components.GetComponent(iCNO).Recordset.AddNew("G3E_FID", cManhole.FID);
                            oManhole.Components.GetComponent(iCNO).Recordset.Update("G3E_FNO", 2700);
                        }
                        else
                        {
                            oManhole.Components.GetComponent(iCNO).Recordset.MoveLast();
                        }

                        IGTLineGeometry oLine = PGeoLib.CreateLineGeom((xManhole + 1) - 2, yManhole - 2, (xManhole + 1) + 2, yManhole + 2);
                        oManhole.Components.GetComponent(iCNO).Geometry = oLine;

                        oLine = PGeoLib.CreateLineGeom((xManhole - 1) - 2, yManhole - 2, (xManhole - 1) + 2, yManhole + 2);
                        oManhole.Components.GetComponent(iCNO).Recordset.AddNew("G3E_FID", cManhole.FID);
                        oManhole.Components.GetComponent(iCNO).Recordset.Update("G3E_FNO", 2700);
                        oManhole.Components.GetComponent(iCNO).Geometry = oLine;

                        oLine = PGeoLib.CreateLineGeom(xManhole - 0.3, yManhole - 0.3, xManhole + 0.3, yManhole + 0.3);
                        oManhole.Components.GetComponent(iCNO).Recordset.AddNew("G3E_FID", cManhole.FID);
                        oManhole.Components.GetComponent(iCNO).Recordset.Update("G3E_FNO", 2700);
                        oManhole.Components.GetComponent(iCNO).Geometry = oLine;

                        oLine = PGeoLib.CreateLineGeom(xManhole - 0.3, yManhole + 0.3, xManhole + 0.3, yManhole - 0.3);
                        oManhole.Components.GetComponent(iCNO).Recordset.AddNew("G3E_FID", cManhole.FID);
                        oManhole.Components.GetComponent(iCNO).Recordset.Update("G3E_FNO", 2700);
                        oManhole.Components.GetComponent(iCNO).Geometry = oLine;
                        
                        //Remove connection
                        //IGTRelationshipService mobjRelationshipService;
                        //mobjRelationshipService = GTClassFactory.Create<IGTRelationshipService>(m_oGTCustomCommandHelper);
                        //mobjRelationshipService.DataContext = application.DataContext;

                        //mobjRelationshipService.ActiveFeature = oManhole;
                        //if (mobjRelationshipService.AllowDelete(1))
                        //{
                        //    mobjRelationshipService.SilentDelete(1);
                        //}

                        //if (!oManhole.Components.GetComponent(54).Recordset.EOF)
                        //{
                        //    oManhole.Components.GetComponent(54).Recordset.Update("NODE1_ID", 0);
                        //    oManhole.Components.GetComponent(54).Recordset.Update("NODE2_ID", 0);
                        //}

                        m_oGTTransactionManager.Commit();
                        m_oGTTransactionManager.RefreshDatabaseChanges();
                        application.RefreshWindows();

                        application.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Identify PPF manhole or right-click to place new manhole");
                        mintState = 31;
                    }
                    else if (mintState == 31)
                    {
                        iManholeFID = 0;
                        cManhole = null;

                        double X = WorldPoint.X;
                        double Y = WorldPoint.Y;
                        mintState = -1;
                        mptSelected = null;
                        IGTDDCKeyObjects objs = mobjLocateService.Locate(WorldPoint, 50, 10, GTSelectionTypeConstants.gtmwstSelectAll);

                        foreach (IGTDDCKeyObject oDDCKeyObject in objs)
                        {
                            if ((oDDCKeyObject.ComponentViewName == "VGC_MANHL_S"))
                            {
                                if (oDDCKeyObject.Recordset.Fields["FEATURE_STATE"].Value.ToString() == "PPF")
                                {
                                    iManholeGeom = oDDCKeyObject.Geometry;
                                    iManholeFNO = oDDCKeyObject.FNO;
                                    iManholeFID = oDDCKeyObject.FID;
                                }
                                else
                                {
                                    MessageBox.Show("Selected manhole must be in 'PPF' state", "Manhole demolish/recontruction", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                                    mintState = 31;
                                    return;
                                }
                            }
                        }

                        if (iManholeFID > 0)
                        {
                            bool NoConnection = true;
                            oManhole = application.DataContext.OpenFeature(iManholeFNO, iManholeFID);
                            if (!oManhole.Components.GetComponent(54).Recordset.EOF)
                            {
                                int node_id = 0;
                                if (oManhole.Components.GetComponent(54).Recordset.Fields["NODE1_ID"].Value != DBNull.Value) node_id = Convert.ToInt32(oManhole.Components.GetComponent(54).Recordset.Fields["NODE1_ID"].Value);
                                if (node_id > 0) NoConnection = false;
                                if (oManhole.Components.GetComponent(54).Recordset.Fields["NODE2_ID"].Value != DBNull.Value) node_id = Convert.ToInt32(oManhole.Components.GetComponent(54).Recordset.Fields["NODE2_ID"].Value);
                                if (node_id > 0) NoConnection = false;
                            }
                            if (NoConnection)
                            {
                                LoadManholeType(oManhole);

                                application.ActiveMapWindow.HighlightedObjects.Clear();
                                application.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Point to define new location or right-click to accept current location");
                                mintState = 4;
                                return;
                            }
                            else
                            {
                                MessageBox.Show("Selected manhole must have no connectivity", "Manhole demolish/recontruction", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                                mintState = 31;
                                return;
                            }
                        }
                        mintState = 31;
                    }
                    else if (mintState == 4)
                    {
                        mintState = -1;
                        double xManhole = WorldPoint.X;
                        double yManhole = WorldPoint.Y;

                        mptSelected = WorldPoint;

                        if (mobjEditPointService.GeometryCount > 0) mobjEditPointService.RemoveAllGeometries();
                        if (mobjEditServiceTemp.GeometryCount > 0) mobjEditServiceTemp.RemoveAllGeometries();
                        //if (mobjEditService.GeometryCount > 0) mobjEditService.RemoveAllGeometries();

                        IGTOrientedPointGeometry oManholeGeom;
                        oManholeGeom = PGeoLib.CreatePointGeom(xManhole, yManhole);
                        if (cManhole.Angle != 0)
                        {
                            IGTVector vector = GTClassFactory.Create<IGTVector>();
                            vector.I = Math.Cos(cManhole.Angle);
                            vector.J = Math.Sin(cManhole.Angle);
                            oManholeGeom.Orientation = vector;
                        }
                        mobjEditPointService.AddGeometry(oManholeGeom, cManhole.ManholeStyle);

                        mintState = 4;
                    }
                    else if (mintState == 5)
                    {
                        mintState = -1;
                        // Set wall number

                        ManholePoint _pnt1 = cManhole.ManholeWall[0];
                        int fNbr = 0;

                        if (cManhole.Angle == 0)
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
                            double angle = cManhole.Angle;
                            objPointTmp.X = cManhole.X + _pnt1.X;
                            objPointTmp.Y = cManhole.Y + _pnt1.Y;
                            double txtX = PGeoLib.RotatePoint(cManhole.Origin, angle, objPointTmp).X;
                            double txtY = PGeoLib.RotatePoint(cManhole.Origin, angle, objPointTmp).Y;
                            double len1 = PGeoLib.GetLength(WorldPoint.X, WorldPoint.Y, txtX, txtY);
                            for (int i = 0; i < cManhole.ManholeWall.Count; i++)
                            {
                                ManholePoint _pnt = cManhole.ManholeWall[i];
                                objPointTmp.X = cManhole.X + _pnt.X;
                                objPointTmp.Y = cManhole.Y + _pnt.Y;
                                txtX = PGeoLib.RotatePoint(cManhole.Origin, angle, objPointTmp).X;
                                txtY = PGeoLib.RotatePoint(cManhole.Origin, angle, objPointTmp).Y;
                                //IGTTextPointGeometry txtNumber2 = PGeoLib.CreateTextGeom(txtX, txtY, "o" + Convert.ToString(i));
                                //mobjEditServiceTemp.AddGeometry(txtNumber2, 30700);

                                double len2 = PGeoLib.GetLength(WorldPoint.X, WorldPoint.Y, txtX, txtY);
                                if (len2 < len1)
                                {
                                    fNbr = i;
                                    len1 = len2;
                                }
                            }
                        }

                        cManhole.FirstNumber = fNbr;

                        if (mobjEditServiceTemp.GeometryCount > 0) mobjEditServiceTemp.RemoveAllGeometries();

                        string mstrTransactionName = "Update Manhole Wallnumber";
                        m_oGTTransactionManager.Begin(mstrTransactionName);

                        oManhole = application.DataContext.OpenFeature(iManholeFNO, iManholeFID);

                        //Wall number
                        short iCNO = 2732;
                        for (int i = 0; i < cManhole.ManholeWall.Count; i++)
                        {
                            if (fNbr >= cManhole.ManholeWall.Count) fNbr = 0;
                            ManholePoint _pnt = cManhole.ManholeWall[fNbr];
                            IGTTextPointGeometry txtNumber ;
                            if (cManhole.Angle == 0)
                            {
                                txtNumber = PGeoLib.CreateTextGeom(cManhole.X + _pnt.X, cManhole.Y + _pnt.Y, Convert.ToString(i + 1));
                            }
                            else
                            {
                                IGTPoint objPointTmp = GTClassFactory.Create<IGTPoint>();
                                double angle = cManhole.Angle;
                                objPointTmp.X = cManhole.X + _pnt.X;
                                objPointTmp.Y = cManhole.Y + _pnt.Y;
                                double txtX = PGeoLib.RotatePoint(cManhole.Origin, angle, objPointTmp).X;
                                double txtY = PGeoLib.RotatePoint(cManhole.Origin, angle, objPointTmp).Y;

                                txtNumber = PGeoLib.CreateTextGeom(txtX, txtY, Convert.ToString(i + 1), PGeoLib.Rad2Deg(angle), GTAlignmentConstants.gtalCenterCenter);
                            }

                            oManhole.Components.GetComponent(iCNO).Recordset.AddNew("G3E_FID", oManhole.FID);
                            oManhole.Components.GetComponent(iCNO).Recordset.Update("G3E_FNO", oManhole.FNO);
                            oManhole.Components.GetComponent(iCNO).Recordset.Update("WALL_NUM", Convert.ToString(i + 1));
                            oManhole.Components.GetComponent(iCNO).Recordset.Update("G3E_ALIGNMENT", 0);
                            oManhole.Components.GetComponent(iCNO).Geometry = txtNumber;

                            fNbr++;
                        }
                        m_oGTTransactionManager.Commit();
                        m_oGTTransactionManager.RefreshDatabaseChanges();
                        application.RefreshWindows();


                        mintState = 6;
                    }
                    else if (mintState == 6)
                    {
                        mintState = -1;
                        // Add text arrow
                        cManhole.Arrow = WorldPoint;

                        mintState = 7;
                    }
                    else if (mintState == 7)
                    {
                        mintState = -1;
                        mptPoint2 = WorldPoint;

                        // Add text
                        IGTTextPointGeometry oText = PGeoLib.CreateTextGeom(mptPoint2.X, mptPoint2.Y, cManhole.ManholeLabel, 0, cManhole.ManholeLabelAlignment);
                        if (mobjEditPointService.GeometryCount > 0) mobjEditPointService.RemoveAllGeometries();
                        mobjEditPointService.AddGeometry(oText, 30700);

                        if (cManhole.Arrow != null)
                        {
                            IGTPolylineGeometry oLine = PGeoLib.CreateHeaderLine(cManhole.Arrow, WorldPoint);
                            mobjEditPointService.AddGeometry(oLine, 11501);
                        }


                        mintState = 7;
                    }
                    else if (mintState == 8)
                    {
                        mintState = -1;

                        if (currentPath >= lstDuctpathIPID.Count)
                        {
                            mintState = 10;
                            return;
                        }
                        DuctpathCtrl cDuctpath = lstDuctpathIPID[currentPath];
                        IGTDDCKeyObjects objs = mobjLocateService.Locate(WorldPoint, 10, 1, GTSelectionTypeConstants.gtmwstSelectAll);

                        application.ActiveMapWindow.HighlightedObjects.Clear();
                        foreach (IGTDDCKeyObject oDDCKeyObject in objs)
                        {
                            if ((oDDCKeyObject.ComponentViewName == "VGC_MANHLW_T"))
                            {
                                if (oDDCKeyObject.FID == cManhole.FID)
                                {
                                    string sWall = oDDCKeyObject.Recordset.Fields["WALL_NUM"].Value.ToString();
                                    double X = oDDCKeyObject.Geometry.FirstPoint.X;
                                    double Y = oDDCKeyObject.Geometry.FirstPoint.Y;

                                    string mstrTransactionName = "Update Ductpath";
                                    m_oGTTransactionManager.Begin(mstrTransactionName);

                                    IGTKeyObject cond = UpdateDuctpath(lstDuctpathIPID[currentPath], X, Y, sWall);
                                    lstDuctpathIPID[currentPath].Geometry = cond.Components.GetComponent(2210).Geometry;

                                    m_oGTTransactionManager.Commit();
                                    m_oGTTransactionManager.RefreshDatabaseChanges();
                                    application.RefreshWindows();

                                    mintState = 9;
                                    return;
                                }
                            }
                        }
                        mintState = 8;
                    }
                    else if (mintState == 9)
                    {
                        // Set ductpath text
                        if (currentPath >= lstDuctpathIPID.Count) return;
                        if (mobjEditServiceTemp.GeometryCount > 0) mobjEditServiceTemp.RemoveAllGeometries();

                        string mstrTransactionName = "Update Ductpath Label";
                        m_oGTTransactionManager.Begin(mstrTransactionName);

                        DuctpathCtrl cDuctpath = lstDuctpathIPID[currentPath];
                        IGTKeyObject oDuctpath = application.DataContext.OpenFeature(2200, cDuctpath.FID);

                        //Manhole label
                        short iCNO = 2230;

                        IGTPoint pnt = cDuctpath.Geometry.FirstPoint;
                        pnt.Y = pnt.Y + 10;
                        mptPoint2 = PGeoLib.GetProjectedPoint(cDuctpath.Geometry.CopyGeometryParallel(pnt, 1), WorldPoint);

                        IGTTextPointGeometry oText = PGeoLib.CreateTextGeom(mptPoint2.X, mptPoint2.Y, cDuctpath.Label, 0, GTAlignmentConstants.gtalCenterCenter);
                        double angle = PGeoLib.GetProjectedAngle(cDuctpath.Geometry.CopyGeometryParallel(pnt, 1), WorldPoint);
                        if (angle > Math.PI / 2) angle = angle - Math.PI;
                        if (angle > Math.PI / 2) angle = angle - Math.PI;
                        oText.Rotation = PGeoLib.Rad2Deg(angle);

                        if (oDuctpath.Components.GetComponent(iCNO).Recordset.EOF)
                        {
                            oDuctpath.Components.GetComponent(iCNO).Recordset.AddNew("G3E_FID", cDuctpath.FID);
                            oDuctpath.Components.GetComponent(iCNO).Recordset.Update("G3E_FNO", 2200);
                        }
                        oDuctpath.Components.GetComponent(iCNO).Geometry = oText;

                        iCNO = 2212;
                        if (!oDuctpath.Components.GetComponent(iCNO).Recordset.EOF)
                        {
                            PGeoLib.DeleteComponent(2200, cDuctpath.FID, iCNO);
                        }

                        m_oGTTransactionManager.Commit();
                        m_oGTTransactionManager.RefreshDatabaseChanges();
                        application.RefreshWindows();

                        if ((cDuctpath.DuctNest == null) || (cDuctpath.DuctNest.Count == 0))
                        {
                            currentPath++;
                            if ((currentPath >= lstDuctpathIPID.Count) && (lstDuctpathIPID.Count > 0))
                                mintState = 1;
                            else
                                mintState = 8;
                        }
                        else
                        {
                            currentNest = 0;
                            mintState = 10;
                        }
                    }
                    else if (mintState == 10)
                    {
                        // Set ductnest location
                        if (currentPath >= lstDuctpathIPID.Count) return;
                        if (mobjEditServiceTemp.GeometryCount > 0) mobjEditServiceTemp.RemoveAllGeometries();

                        application.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Moving ductnest, please wait...");
                        string mstrTransactionName = "Update Ductnest";
                        m_oGTTransactionManager.Begin(mstrTransactionName);

                        DuctpathCtrl cDuctpath = lstDuctpathIPID[currentPath];
                        DuctnestCtrl cDuctnest = cDuctpath.DuctNest[currentNest];

                        MoveFormation(cDuctnest, WorldPoint);

                        m_oGTTransactionManager.Commit();
                        m_oGTTransactionManager.RefreshDatabaseChanges();
                        application.RefreshWindows();

                        currentNest++;
                        if (currentNest >= cDuctpath.DuctNest.Count)
                        {
                            currentPath++;
                            if ((currentPath >= lstDuctpathIPID.Count) && (lstDuctpathIPID.Count > 0))
                                mintState = 1;
                            else
                                mintState = 8;
                        }
                        else
                        {
                            mintState = 10;
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                //Logger log = Logger.GetInstance();
                log.WriteLog("Error", "m_oCustomCommandHelper_MouseUp", ex.Message);
                log.WriteLog(ex.StackTrace);
                MessageBox.Show(ex.Message, "Error.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                if (m_oGTTransactionManager != null) m_oGTTransactionManager.Rollback();
                if (mintState == -1)
                    if (m_oGTCustomCommandHelper != null) m_oGTCustomCommandHelper.Complete();
                //log.CloseFile();
            }
        }

        private void m_oCustomCommandHelper_MouseMove(object sender, GTMouseEventArgs MouseEventArgs)
        {
            try
            {
                IGTPoint WorldPoint = MouseEventArgs.WorldPoint;

                if ((mintState == 1))
                {
                    application.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Identify manhole to demolish or right-click to exit");
                    double X = WorldPoint.X;
                    double Y = WorldPoint.Y;

                    mptSelected = null;
                    IGTDDCKeyObjects objs = mobjLocateService.Locate(WorldPoint, 50, 10, GTSelectionTypeConstants.gtmwstSelectAll);

                    application.ActiveMapWindow.HighlightedObjects.Clear();
                    application.ActiveMapWindow.HighlightedObjects.AddMultiple(objs);
                }
                else if ((mintState == 2))
                {
                    double xManhole = WorldPoint.X;
                    double yManhole = WorldPoint.Y;

                    if (mobjEditServiceTemp.GeometryCount > 0) mobjEditServiceTemp.RemoveAllGeometries();
                    IGTLineGeometry oLine = PGeoLib.CreateLineGeom((xManhole + 1) - 2, yManhole - 2, (xManhole + 1) + 2, yManhole + 2);
                    mobjEditServiceTemp.AddGeometry(oLine, 11516);
                    oLine = PGeoLib.CreateLineGeom((xManhole - 1) - 2, yManhole - 2, (xManhole - 1) + 2, yManhole + 2);
                    mobjEditServiceTemp.AddGeometry(oLine, 11516);

                    oLine = PGeoLib.CreateLineGeom(xManhole - 0.3, yManhole - 0.3, xManhole + 0.3, yManhole + 0.3);
                    mobjEditServiceTemp.AddGeometry(oLine, 11516);

                    oLine = PGeoLib.CreateLineGeom(xManhole - 0.3, yManhole + 0.3, xManhole + 0.3, yManhole - 0.3);
                    mobjEditServiceTemp.AddGeometry(oLine, 11516);

                    IGTOrientedPointGeometry oManhole;
                    oManhole = PGeoLib.CreatePointGeom(xManhole, yManhole);
                    if (cManhole.Angle != 0)
                    {
                        IGTVector vector = GTClassFactory.Create<IGTVector>();
                        vector.I = Math.Cos(cManhole.Angle);
                        vector.J = Math.Sin(cManhole.Angle);
                        oManhole.Orientation = vector;
                    }
                    mobjEditServiceTemp.AddGeometry(oManhole, cManhole.ManholeStyle);

                    application.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Enter location of demolished manhole");
                }
                else if ((mintState == 3))
                {
                    double X = WorldPoint.X;
                    double Y = WorldPoint.Y;

                    if (mptSelected == null) mptSelected = WorldPoint;
                    double _angle = PGeoLib.GetAngle(mptSelected.X, mptSelected.Y, X, Y);
                    double xManhole = mptSelected.X + Math.Cos(_angle) * _distance;
                    double yManhole = mptSelected.Y - Math.Sin(_angle) * _distance;

                    IGTLineGeometry oLine = PGeoLib.CreateLineGeom(mptSelected.X, mptSelected.Y, xManhole, yManhole);
                    if (mobjEditServiceTemp.GeometryCount > 0) mobjEditServiceTemp.RemoveAllGeometries();
                    mobjEditServiceTemp.TargetMapWindow = application.ActiveMapWindow;
                    mobjEditServiceTemp.AddGeometry(oLine, 11516);

                    //if (mobjEditPointService.GeometryCount > 0) mobjEditPointService.RemoveAllGeometries();
                    IGTOrientedPointGeometry oManhole;
                    oManhole = PGeoLib.CreatePointGeom(xManhole, yManhole);
                    mobjEditServiceTemp.AddGeometry(oManhole, StyleTempManhole);

                    application.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Point to accept location or right-click to exit");
                }
                else if (mintState == 31)
                {
                    application.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Identify PPF manhole or right-click to place new manhole");
                    double X = WorldPoint.X;
                    double Y = WorldPoint.Y;

                    mptSelected = null;
                    IGTDDCKeyObjects objs = mobjLocateService.Locate(WorldPoint, 50, 10, GTSelectionTypeConstants.gtmwstSelectAll);

                    application.ActiveMapWindow.HighlightedObjects.Clear();
                    application.ActiveMapWindow.HighlightedObjects.AddMultiple(objs);
                }
                else if (mintState == 4)
                {
                    double xManhole = WorldPoint.X;
                    double yManhole = WorldPoint.Y;

                    if (mobjEditServiceTemp.GeometryCount > 0) mobjEditServiceTemp.RemoveAllGeometries();

                    IGTOrientedPointGeometry oManhole;
                    oManhole = PGeoLib.CreatePointGeom(xManhole, yManhole);
                    if (cManhole.Angle != 0)
                    {
                        IGTVector vector = GTClassFactory.Create<IGTVector>();
                        vector.I = Math.Cos(cManhole.Angle);
                        vector.J = Math.Sin(cManhole.Angle);
                        oManhole.Orientation = vector;
                    }
                    mobjEditServiceTemp.AddGeometry(oManhole, cManhole.ManholeStyle);

                    application.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Point to enter new location or right-click to accept current location");
                }
                else if (mintState == 5)
                {
                    application.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Identify [+] to be wall 1");
                }
                else if (mintState == 6)
                {
                    application.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Point to define arrow head text or right-click to define text location");
                }
                else if (mintState == 7)
                {
                    IGTPoint mptPoint2 = MouseEventArgs.WorldPoint;

                    IGTTextPointGeometry oText = PGeoLib.CreateTextGeom(mptPoint2.X, mptPoint2.Y, cManhole.ManholeLabel, 0, cManhole.ManholeLabelAlignment);
                    if (mobjEditServiceTemp.GeometryCount > 0) mobjEditServiceTemp.RemoveAllGeometries();
                    mobjEditServiceTemp.AddGeometry(oText, 30700);

                    if (cManhole.Arrow != null)
                    {
                        IGTPolylineGeometry oLine = PGeoLib.CreateHeaderLine(cManhole.Arrow, WorldPoint);
                        mobjEditServiceTemp.AddGeometry(oLine, 11501);
                    }
                    
                    application.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Enter new location for text or right-click to accept current location");
                }
                else if (mintState == 8)
                {
                    application.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Identify the manhole wall.");

                    double X = WorldPoint.X;
                    double Y = WorldPoint.Y;

                    if (currentPath >= lstDuctpathIPID.Count) return;
                    DuctpathCtrl cDuctpath = lstDuctpathIPID[currentPath];

                    IGTLineGeometry oLine = PGeoLib.CreateLineGeom(cDuctpath.Point.X, cDuctpath.Point.Y, X, Y);
                    if (mobjEditServiceTemp.GeometryCount > 0) mobjEditServiceTemp.RemoveAllGeometries();
                    mobjEditServiceTemp.AddGeometry(oLine, 11508);

                    IGTDDCKeyObjects objs = mobjLocateService.Locate(WorldPoint, 10, 1, GTSelectionTypeConstants.gtmwstSelectAll);

                    application.ActiveMapWindow.HighlightedObjects.Clear();
                    foreach (IGTDDCKeyObject oDDCKeyObject in objs)
                    {
                        if ((oDDCKeyObject.ComponentViewName == "VGC_MANHLW_T"))
                        {
                            application.ActiveMapWindow.HighlightedObjects.AddMultiple(objs);
                        }
                    }
                }
                else if (mintState == 9)
                {
                    application.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Move text to place along the line");
                    if (currentPath >= lstDuctpathIPID.Count) return;
                    DuctpathCtrl cDuctpath = lstDuctpathIPID[currentPath];

                    IGTPoint pnt = cDuctpath.Geometry.FirstPoint;
                    pnt.Y = pnt.Y + 10;
                    mptPoint2 = PGeoLib.GetProjectedPoint(cDuctpath.Geometry.CopyGeometryParallel(pnt, 1), WorldPoint);

                    IGTTextPointGeometry oText = PGeoLib.CreateTextGeom(mptPoint2.X, mptPoint2.Y, cDuctpath.Label, 0, cDuctpath.Alignment);
                    double angle = PGeoLib.GetProjectedAngle(cDuctpath.Geometry.CopyGeometryParallel(pnt, 1), WorldPoint);
                    if (angle > Math.PI / 2) angle = angle - Math.PI;
                    if (angle > Math.PI / 2) angle = angle - Math.PI;
                    oText.Rotation = PGeoLib.Rad2Deg(angle);
                    
                    if (mobjEditServiceTemp.GeometryCount > 0) mobjEditServiceTemp.RemoveAllGeometries();
                    mobjEditServiceTemp.AddGeometry(oText, 30700);
                }
                else if (mintState == 10)
                {
                    application.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Define ductnest location");
                    if (currentPath >= lstDuctpathIPID.Count) return;
                    DuctpathCtrl cDuctpath = lstDuctpathIPID[currentPath];
                    DuctnestCtrl cDuctnest = cDuctpath.DuctNest[currentNest];

                    IGTGeometry geom = cDuctnest.Geometry;
                    IGTPoint oPoint = geom.FirstPoint;
                    double len = PGeoLib.GetLength(oPoint, WorldPoint);
                    double angle = PGeoLib.GetAngle(oPoint, WorldPoint);

                    IGTVector vector = GTClassFactory.Create<IGTVector>();
                    vector.I = len * Math.Cos(angle);
                    vector.J = len * Math.Sin(angle);

                    geom.Move(vector);

                    if (mobjEditServiceTemp.GeometryCount > 0) mobjEditServiceTemp.RemoveAllGeometries();
                    mobjEditServiceTemp.AddGeometry(geom, 11500);
                }
                System.Windows.Forms.Application.DoEvents();
            }
            catch (Exception ex)
            {
                log.WriteLog("Error", "m_oCustomCommandHelper_MouseMove", ex.Message);
                log.WriteLog(ex.StackTrace);
                MessageBox.Show(ex.Message, "Error.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //log.CloseFile();
            }
        }

        //******************************************************************************
        //********************** GTFeatureExplorerService Events. **********************
        //******************************************************************************
        private void CloseFeatureExplorer()
        {
            // Clears feature explorer, if it was set.
            if (!(mobjExplorerService == null))
            {
                mobjExplorerService.Clear();
            }


            // If feature explorer was not originally displayed before this command was
            // started then ensure it is not displayed after. The only reason why it was
            // displayed is because this command displayed it and the command should reset
            // the state back to the original conditions.
            if ((!(mobjExplorerService == null)
                        && !mblnVisible))
            {
                mobjExplorerService.Visible = false;
            }

        }

        private void SaveAttribute()
        {
            if (mintState == 32)
            {
                mintState = -1;
                m_oGTTransactionManager.Rollback();

                string mstrTransactionName = "Create new Manhole";
                m_oGTTransactionManager.Begin(mstrTransactionName);

                oManhole = CopyManhole(cManhole.Manhole, cManhole.Origin);

                //oManhole = application.DataContext.OpenFeature(iManholeFNO, iManholeFID);
                PGeoLib.CopyComponentAttribute(mobjManholeAttribute, oManhole);

                iManholeFID = oManhole.FID;

                m_oGTTransactionManager.Commit();
                m_oGTTransactionManager.RefreshDatabaseChanges();
                application.RefreshWindows();

                LoadManholeType(oManhole);

                if (!(mobjExplorerService == null))
                {
                    mobjExplorerService.Visible = false;
                }

                mintState = 4;
            }
        }

        //******************************************************************************
        // Purpose:  Represents the method that will handle the CancelClick event.
        //
        //******************************************************************************
        private void mobjExplorerService_CancelClick(object sender, EventArgs e)
        {
            // Throws any errors raised to the error handler.
            try
            {
                //service with exitcmd
                ExitCmd();

            }
            catch (Exception ex)
            {
                log.WriteLog("Error", "m_oCustomCommandHelper_MouseMove", ex.Message);
                log.WriteLog(ex.StackTrace);
                MessageBox.Show(ex.Message, "Error.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //log.CloseFile();
            }
        }

        //******************************************************************************
        // Purpose:  Represents the method that will handle the SaveAndContinueClick
        //           event.
        //
        //******************************************************************************
        private void mobjExplorerService_SaveAndContinueClick(object sender, EventArgs e)
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
                log.WriteLog("Error", "m_oCustomCommandHelper_MouseMove", ex.Message);
                log.WriteLog(ex.StackTrace);
                MessageBox.Show(ex.Message, "Error.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //log.CloseFile();
            }

        }


        //******************************************************************************
        // Purpose:  Represents the method that will handle the SaveClick event.
        //
        //******************************************************************************
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
                log.WriteLog("Error", "m_oCustomCommandHelper_MouseMove", ex.Message);
                log.WriteLog(ex.StackTrace);
                MessageBox.Show(ex.Message, "Error.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //log.CloseFile();
            }

        }
        //Anna's add
        #region Get Value from Database
        private string Get_Value(string sSql)
        {
            try
            {
                ADODB.Recordset rsPP = new ADODB.Recordset();
                rsPP = application.DataContext.OpenRecordset(sSql, ADODB.CursorTypeEnum.adOpenStatic, ADODB.LockTypeEnum.adLockReadOnly, 1);
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
        //////
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

                mobjExplorerService.CancelClick += new EventHandler(mobjExplorerService_CancelClick);
                mobjExplorerService.SaveAndContinueClick += new EventHandler(mobjExplorerService_SaveAndContinueClick);
                mobjExplorerService.SaveClick += new EventHandler(mobjExplorerService_SaveClick);
            }
            catch (Exception ex)
            {
                //Logger log = Logger.GetInstance();
                log.WriteLog("Error", "RegisterEvents", ex.Message);
                log.WriteLog(ex.StackTrace);
                MessageBox.Show(ex.Message, "Error.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //log.CloseFile();
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
                //m_oCustomCommandHelper.MouseMove -= new EventHandler(this.m_oCustomCommandHelper_MouseMove);

                mobjExplorerService.CancelClick -= new EventHandler(mobjExplorerService_CancelClick);
                mobjExplorerService.SaveAndContinueClick -= new EventHandler(mobjExplorerService_SaveAndContinueClick);
                mobjExplorerService.SaveClick -= new EventHandler(mobjExplorerService_SaveClick);
            }
            catch (Exception ex)
            {
                //Logger log = Logger.GetInstance();
                log.WriteLog("Error", "UnRegisterEvents", ex.Message);
                log.WriteLog(ex.StackTrace);
                MessageBox.Show(ex.Message, "Error.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //log.CloseFile();
            }
        }
        #endregion

    }
}
