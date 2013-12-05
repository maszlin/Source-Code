/*
 * Manhole placement - Modeless Custom Command
 * 
 * Author: La Viet Phuong - AG
 * 
 * This modeless custom command gives the user a list of features and a input text box. The user can select one of those features
 * and input a value, then another list will open with all the placed features that contain that value. When the user selects
 * one of those features the map window will zoom on it.
 * 
 * edited : m.zam @ 12-10-2012
 * issues : stub/stump can starts from GC_ITFACE
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

namespace AG.GTechnology.StubStumpPlacement
{
    class GTStubStumpPlacement : Intergraph.GTechnology.Interfaces.IGTCustomCommandModeless
    {
        private Intergraph.GTechnology.API.IGTTransactionManager m_oGTTransactionManager;
        private Intergraph.GTechnology.API.IGTCustomCommandHelper m_oGTCustomCommandHelper;
        private static Intergraph.GTechnology.API.IGTApplication application = GTClassFactory.Create<IGTApplication>();
        private Logger log;
        private IGTDataContext oDataContext = application.DataContext;
        private static IGTPolylineGeometry oLineGeom = null;
        private static List<IGTGeometry> arrIntersection = new List<IGTGeometry>();
        private static List<IGTGeometry> arrIntersectionText = new List<IGTGeometry>();
        private string mstrFistPoint = "Please enter the attributes for Stub/Stump.";
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

        IGTPoint mptSelected;
        IGTPoint mptPoint1;
        IGTPoint mptPoint2;
        IGTKeyObject mobjAttribute = null;

        private int iDetailID = 0;

        private short iSourceCblFNO = 7000;
        private int iSourceCblFID = 0;
        private IGTKeyObject iSourceCbl = null;
        private string CableSide = string.Empty;

        private short iTermFNO = 7000;
        private int iTermFID = 0;
        private IGTGeometry iTermGeom = null;

        private clsStumpStub cStubStump = null;

        Dictionary<string, string> Parameters = new Dictionary<string, string>();

        double _distance = 0;

        private IGTPoint LoadTerminationPoint(short iFNO, int iFID)
        {
            Recordset rsComp = null;
            int recordsAffected = 0;

            IGTPoint pnt = null;
            if (iFNO != 10800) // its not a joint
            {
                return null;
            }

            string sSql = "select b.SPLICE_CLASS from GC_SPLICE b where b.G3E_FNO=" + iFNO.ToString() + " and G3E_FID=" + iFID.ToString();
            rsComp = application.DataContext.Execute(sSql, out recordsAffected, (int)CommandTypeEnum.adCmdText, null);

            if (rsComp != null)
            {
                if (!rsComp.EOF)
                {
                    if (rsComp.Fields["SPLICE_CLASS"].Value != DBNull.Value) CableSide = rsComp.Fields["SPLICE_CLASS"].Value.ToString();
                }
            }
            else
            {
                return null;
            }
            rsComp = null;

            if (CableSide.Contains("D-SIDE")) CableSide = "D-SIDE";
            else if (CableSide.Contains("E-SIDE")) CableSide = "E-SIDE";
            else CableSide = string.Empty;

            sSql = "select b.G3E_FNO, b.G3E_FID from GC_NR_CONNECT b where b.OUT_FNO=" + iFNO.ToString() + " and b.OUT_FID=" + iFID.ToString();
            rsComp = application.DataContext.Execute(sSql, out recordsAffected, (int)CommandTypeEnum.adCmdText, null);

            if (rsComp != null)
            {
                if (!rsComp.EOF)
                {
                    if (rsComp.Fields["G3E_FNO"].Value != DBNull.Value) iSourceCblFNO = Convert.ToInt16(rsComp.Fields["G3E_FNO"].Value);
                    if (rsComp.Fields["G3E_FID"].Value != DBNull.Value) iSourceCblFID = Convert.ToInt32(rsComp.Fields["G3E_FID"].Value);
                }
            }
            rsComp = null;

            //PGeoLib.LocateFeature(iTermFNO, iTermFID, application.ActiveMapWindow);

            short iCNO = (short)(application.ActiveMapWindow.DetailID == 0 ? 10820 : 10821); // gc_splice_s or dgc_splice_s
            iSourceCbl = application.DataContext.OpenFeature(iFNO, iFID);

            IGTGeometry iSourceCblGeom = iSourceCbl.Components.GetComponent(iCNO).Geometry;
            if (iSourceCblGeom != null)
            {
                pnt = iSourceCblGeom.FirstPoint;
                IGTPoint pntTemp = GTClassFactory.Create<IGTPoint>();

                IGTWorldRange range = GTClassFactory.Create<IGTWorldRange>();
                pntTemp.X = pnt.X - 10;
                pntTemp.Y = pnt.Y + 10;
                range.BottomLeft = pntTemp;

                pntTemp = GTClassFactory.Create<IGTPoint>();
                pntTemp.X = pnt.X + 10;
                pntTemp.Y = pnt.Y - 10;
                range.TopRight = pntTemp;
                application.ActiveMapWindow.ZoomArea(range);
                application.RefreshWindows();
            }

            return pnt;
        }


        private IGTKeyObject CreateStump(clsStumpStub stump)
        {
            short iFNO;
            short iCNO;
            long lFID;
            IGTPoint oPoint = stump.StartPoint;
            double _angle = stump.Angle;

            IGTKeyObject oNewFeature;
            IGTKeyObject oNewFeaturePoint;

            IGTTextPointGeometry oOrTextGeom;
            IGTOrientedPointGeometry oOrPointGeom = null;

            IGTRelationshipService mobjRelationshipService;
            short mintNRRelationshipNumber = 8;

            if (mobjEditService == null) return null;

            iFNO = 7000;
            oNewFeature = application.DataContext.NewFeature(iFNO);

            // FID generation.
            lFID = oNewFeature.FID;

            if (mobjAttribute != null)
                PGeoLib.CopyComponentAttribute(mobjAttribute, oNewFeature);
            else
            {
                // Every feature is imported in the Existing state.
                iCNO = 51; //GC_NETELEM
                if (oNewFeature.Components.GetComponent(iCNO).Recordset.EOF)
                {
                    oNewFeature.Components.GetComponent(iCNO).Recordset.AddNew("G3E_FID", lFID);
                    //oNewFeature.Components.GetComponent(iCNO).Recordset.Update("JOB_STATE", "CLD");
                    //oNewFeature.Components.GetComponent(iCNO).Recordset.Update("FEATURE_STATE", "PPA");
                    //oNewFeature.Components.GetComponent(iCNO).Recordset.Update("SWITCH_CENTRE_CLLI", "PPJ");
                }
                else
                {
                    //oNewFeature.Components.GetComponent(iCNO).Recordset.Update("JOB_STATE", "CLD");
                    //oNewFeature.Components.GetComponent(iCNO).Recordset.Update("FEATURE_STATE", "PPA");
                    //oNewFeature.Components.GetComponent(iCNO).Recordset.Update("SWITCH_CENTRE_CLLI", "PPJ");
                }

                // Attribute
                iCNO = 7001;
                oNewFeature.Components.GetComponent(iCNO).Recordset.Update("CABLE_CLASS", stump.Name);
                oNewFeature.Components.GetComponent(iCNO).Recordset.Update("TEXT_FORMAT", 5);
            }

            //Line
            if (iDetailID > 0) iCNO = 7011; else iCNO = 7010;
            if (oNewFeature.Components.GetComponent(iCNO).Recordset.EOF)
            {
                oNewFeature.Components.GetComponent(iCNO).Recordset.AddNew("G3E_FID", lFID);
                oNewFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_FNO", iFNO);
                if (iDetailID > 0)
                    oNewFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_DETAILID", iDetailID);
            }
            else
            {
                oNewFeature.Components.GetComponent(iCNO).Recordset.MoveLast();
            }

            _distance = 2;
            double xStubStump = oPoint.X + Math.Cos(_angle) * _distance;
            double yStubStump = oPoint.Y - Math.Sin(_angle) * _distance;

            IGTPoint objPoint1;
            IGTPoint objPoint2;
            IGTCompositePolylineGeometry oOrLineGeom = null;

            objPoint1 = GTClassFactory.Create<IGTPoint>();
            objPoint1.X = oPoint.X;
            objPoint1.Y = oPoint.Y;
            objPoint2 = GTClassFactory.Create<IGTPoint>();
            objPoint2.X = xStubStump;
            objPoint2.Y = yStubStump;
            oOrLineGeom = GTClassFactory.Create<IGTCompositePolylineGeometry>();
            IGTPolylineGeometry oLine = GTClassFactory.Create<IGTPolylineGeometry>();
            oLine.Points.Add(objPoint1);
            oLine.Points.Add(objPoint2);
            oOrLineGeom.Add(oLine);

            oNewFeature.Components.GetComponent(iCNO).Geometry = oOrLineGeom;

            //Label
            if (iDetailID > 0) iCNO = 7031; else iCNO = 7030;
            IGTTextPointGeometry txtLabel = PGeoLib.CreateTextGeom(
                PGeoLib.GetPoint(oLine, PGeoLib.GetLength(oLine) / 2).X,
                PGeoLib.GetPoint(oLine, PGeoLib.GetLength(oLine) / 2).Y,
                "[Stump label]",
                0, GTAlignmentConstants.gtalTopLeft);

            if (oNewFeature.Components.GetComponent(iCNO).Recordset.EOF)
            {
                oNewFeature.Components.GetComponent(iCNO).Recordset.AddNew("G3E_FID", lFID);
                oNewFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_FNO", iFNO);
                if (iDetailID > 0)
                    oNewFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_DETAILID", iDetailID);
            }
            else
            {
                oNewFeature.Components.GetComponent(iCNO).Recordset.MoveLast();
            }

            oNewFeature.Components.GetComponent(iCNO).Geometry = txtLabel;



            iFNO = 10800;
            oNewFeaturePoint = application.DataContext.NewFeature(iFNO);

            // FID generation.
            lFID = oNewFeaturePoint.FID;

            // Every feature is imported in the Existing state.
            iCNO = 51; //GC_NETELEM
            if (oNewFeaturePoint.Components.GetComponent(iCNO).Recordset.EOF)
            {
                oNewFeaturePoint.Components.GetComponent(iCNO).Recordset.AddNew("G3E_FID", lFID);
                //oNewFeaturePoint.Components.GetComponent(iCNO).Recordset.Update("JOB_STATE", "CLD");
                //oNewFeaturePoint.Components.GetComponent(iCNO).Recordset.Update("FEATURE_STATE", "EXT");
                //oNewFeaturePoint.Components.GetComponent(iCNO).Recordset.Update("SWITCH_CENTRE_CLLI", "PPJ");
            }
            else
            {
                //oNewFeaturePoint.Components.GetComponent(iCNO).Recordset.Update("JOB_STATE", "CLD");
                //oNewFeaturePoint.Components.GetComponent(iCNO).Recordset.Update("FEATURE_STATE", "EXT");
                //oNewFeaturePoint.Components.GetComponent(iCNO).Recordset.Update("SWITCH_CENTRE_CLLI", "PPJ");
            }
            oNewFeaturePoint.Components.GetComponent(iCNO).Recordset.Update("MIN_MATERIAL", "0");

            // Attribute
            iCNO = 10801;
            if (oNewFeaturePoint.Components.GetComponent(iCNO).Recordset.EOF)
            {
                oNewFeaturePoint.Components.GetComponent(iCNO).Recordset.AddNew("G3E_FID", lFID);
                oNewFeaturePoint.Components.GetComponent(iCNO).Recordset.Update("G3E_FNO", iFNO);
            }
            else
                oNewFeaturePoint.Components.GetComponent(iCNO).Recordset.MoveLast();

            oNewFeaturePoint.Components.GetComponent(iCNO).Recordset.Update("SPLICE_CLASS", stump.Name);
            oNewFeaturePoint.Components.GetComponent(iCNO).Recordset.Update("CLOSURE_TYPE", "***");
            oNewFeaturePoint.Components.GetComponent(iCNO).Recordset.Update("JOINT_TYPE", "***");

            iCNO = 64;
            if (oNewFeaturePoint.Components.GetComponent(iCNO).Recordset.EOF)
            {
                oNewFeaturePoint.Components.GetComponent(iCNO).Recordset.AddNew("G3E_FID", lFID);
                oNewFeaturePoint.Components.GetComponent(iCNO).Recordset.Update("G3E_FNO", iFNO);
            }

            //iCNO = 66;
            //if (oNewFeaturePoint.Components.GetComponent(iCNO).Recordset.EOF)
            //{
            //    oNewFeaturePoint.Components.GetComponent(iCNO).Recordset.AddNew("G3E_FID", lFID);
            //    oNewFeaturePoint.Components.GetComponent(iCNO).Recordset.Update("G3E_FNO", iFNO);
            //}

            //Symbol
            if (iDetailID > 0) iCNO = 10821; else iCNO = 10820;
            if (oNewFeaturePoint.Components.GetComponent(iCNO).Recordset.EOF)
            {
                oNewFeaturePoint.Components.GetComponent(iCNO).Recordset.AddNew("G3E_FID", lFID);
                oNewFeaturePoint.Components.GetComponent(iCNO).Recordset.Update("G3E_FNO", iFNO);
                if (iDetailID > 0)
                    oNewFeaturePoint.Components.GetComponent(iCNO).Recordset.Update("G3E_DETAILID", iDetailID);
            }
            else
            {
                oNewFeaturePoint.Components.GetComponent(iCNO).Recordset.MoveLast();
            }

            // GetCompInfo iFNO, iCNO, bMainGr, grType
            oOrPointGeom = PGeoLib.CreatePointGeom(xStubStump, yStubStump);
            oNewFeaturePoint.Components.GetComponent(iCNO).Geometry = oOrPointGeom;


            mobjRelationshipService = GTClassFactory.Create<IGTRelationshipService>(m_oGTCustomCommandHelper);
            mobjRelationshipService.DataContext = application.DataContext;
            //Connect
            mobjRelationshipService.ActiveFeature = oNewFeature;
            if (mobjRelationshipService.AllowSilentEstablish(oNewFeaturePoint))
            {
                mobjRelationshipService.SilentEstablish(mintNRRelationshipNumber, oNewFeaturePoint);
            }

            IGTKeyObject oTermFeature = application.DataContext.OpenFeature(iTermFNO, iTermFID);
            //Connect
            mobjRelationshipService.ActiveFeature = oNewFeature;
            if (mobjRelationshipService.AllowSilentEstablish(oTermFeature))
            {
                mobjRelationshipService.SilentEstablish(13, oTermFeature);
            }

            return oNewFeature;
        }

        private IGTKeyObject CreateStub(clsStumpStub stump)
        {
            short iFNO;
            short iCNO;
            long lFID;
            IGTPoint oPoint = stump.StartPoint;
            double _angle = stump.Angle;

            IGTKeyObject oNewFeature;
            IGTKeyObject oNewFeaturePoint;

            IGTTextPointGeometry oOrTextGeom;
            IGTOrientedPointGeometry oOrPointGeom = null;

            IGTRelationshipService mobjRelationshipService;
            short mintNRRelationshipNumber = 8;

            if (mobjEditService == null) return null;

            iFNO = 7000;
            oNewFeature = application.DataContext.NewFeature(iFNO);

            // FID generation.
            lFID = oNewFeature.FID;

            if (mobjAttribute != null)
                PGeoLib.CopyComponentAttribute(mobjAttribute, oNewFeature);
            else
            {
                // Every feature is imported in the Existing state.
                iCNO = 51; //GC_NETELEM
                if (oNewFeature.Components.GetComponent(iCNO).Recordset.EOF)
                {
                    oNewFeature.Components.GetComponent(iCNO).Recordset.AddNew("G3E_FID", lFID);
                    //oNewFeature.Components.GetComponent(iCNO).Recordset.Update("JOB_STATE", "CLD");
                    //oNewFeature.Components.GetComponent(iCNO).Recordset.Update("FEATURE_STATE", "PPA");
                    //oNewFeature.Components.GetComponent(iCNO).Recordset.Update("SWITCH_CENTRE_CLLI", "PPJ");
                }
                else
                {
                    //oNewFeature.Components.GetComponent(iCNO).Recordset.Update("JOB_STATE", "CLD");
                    //oNewFeature.Components.GetComponent(iCNO).Recordset.Update("FEATURE_STATE", "PPA");
                    //oNewFeature.Components.GetComponent(iCNO).Recordset.Update("SWITCH_CENTRE_CLLI", "PPJ");
                }

                // Attribute
                iCNO = 7001;
                oNewFeature.Components.GetComponent(iCNO).Recordset.Update("CABLE_CLASS", stump.Name);
                oNewFeature.Components.GetComponent(iCNO).Recordset.Update("TEXT_FORMAT", 5);
            }

            //Line
            iCNO = (short)(iDetailID > 0 ? 7011 : 7010);
            if (oNewFeature.Components.GetComponent(iCNO).Recordset.EOF)
            {
                oNewFeature.Components.GetComponent(iCNO).Recordset.AddNew("G3E_FID", lFID);
                oNewFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_FNO", iFNO);
                if (iDetailID > 0)
                    oNewFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_DETAILID", iDetailID);
            }
            else
            {
                oNewFeature.Components.GetComponent(iCNO).Recordset.MoveLast();
            }

            double xStubStump = cStubStump.Line.LastPoint.X;
            double yStubStump = cStubStump.Line.LastPoint.Y;

            IGTCompositePolylineGeometry oOrLineGeom = GTClassFactory.Create<IGTCompositePolylineGeometry>();
            oOrLineGeom.Add(cStubStump.Line);

            oNewFeature.Components.GetComponent(iCNO).Geometry = oOrLineGeom;

            //Label
            iCNO = (short)(iDetailID > 0 ? 7031 : 7030);
            IGTTextPointGeometry txtLabel = PGeoLib.CreateTextGeom(
                PGeoLib.GetPoint(cStubStump.Line, PGeoLib.GetLength(cStubStump.Line) / 2).X,
                PGeoLib.GetPoint(cStubStump.Line, PGeoLib.GetLength(cStubStump.Line) / 2).Y,
                "[Stub label]", 0, GTAlignmentConstants.gtalTopLeft);

            if (oNewFeature.Components.GetComponent(iCNO).Recordset.EOF)
            {
                oNewFeature.Components.GetComponent(iCNO).Recordset.AddNew("G3E_FID", lFID);
                oNewFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_FNO", iFNO);
                if (iDetailID > 0)
                    oNewFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_DETAILID", iDetailID);
            }
            else
            {
                oNewFeature.Components.GetComponent(iCNO).Recordset.MoveLast();
            }

            oNewFeature.Components.GetComponent(iCNO).Geometry = txtLabel;



            iFNO = 10800;
            oNewFeaturePoint = application.DataContext.NewFeature(iFNO);

            // FID generation.
            lFID = oNewFeaturePoint.FID;

            // Every feature is imported in the Existing state.
            iCNO = 51; //GC_NETELEM
            if (oNewFeaturePoint.Components.GetComponent(iCNO).Recordset.EOF)
            {
                oNewFeaturePoint.Components.GetComponent(iCNO).Recordset.AddNew("G3E_FID", lFID);
                //oNewFeaturePoint.Components.GetComponent(iCNO).Recordset.Update("JOB_STATE", "CLD");
                //oNewFeaturePoint.Components.GetComponent(iCNO).Recordset.Update("FEATURE_STATE", "EXT");
                //oNewFeaturePoint.Components.GetComponent(iCNO).Recordset.Update("SWITCH_CENTRE_CLLI", "PPJ");
            }
            else
            {
                //oNewFeaturePoint.Components.GetComponent(iCNO).Recordset.Update("JOB_STATE", "CLD");
                //oNewFeaturePoint.Components.GetComponent(iCNO).Recordset.Update("FEATURE_STATE", "EXT");
                //oNewFeaturePoint.Components.GetComponent(iCNO).Recordset.Update("SWITCH_CENTRE_CLLI", "PPJ");
            }
            oNewFeaturePoint.Components.GetComponent(iCNO).Recordset.Update("MIN_MATERIAL", "0");

            // Attribute
            iCNO = 10801; // GC_SPLICE
            if (oNewFeaturePoint.Components.GetComponent(iCNO).Recordset.EOF)
            {
                oNewFeaturePoint.Components.GetComponent(iCNO).Recordset.AddNew("G3E_FID", lFID);
                oNewFeaturePoint.Components.GetComponent(iCNO).Recordset.Update("G3E_FNO", iFNO);
            }
            else
                oNewFeaturePoint.Components.GetComponent(iCNO).Recordset.MoveLast();

            oNewFeaturePoint.Components.GetComponent(iCNO).Recordset.Update("SPLICE_CLASS", stump.Name);
            oNewFeaturePoint.Components.GetComponent(iCNO).Recordset.Update("CLOSURE_TYPE", "***");
            oNewFeaturePoint.Components.GetComponent(iCNO).Recordset.Update("JOINT_TYPE", "***");

            iCNO = 64;
            if (oNewFeaturePoint.Components.GetComponent(iCNO).Recordset.EOF)
            {
                oNewFeaturePoint.Components.GetComponent(iCNO).Recordset.AddNew("G3E_FID", lFID);
                oNewFeaturePoint.Components.GetComponent(iCNO).Recordset.Update("G3E_FNO", iFNO);
            }

            //iCNO = 66;
            //if (oNewFeaturePoint.Components.GetComponent(iCNO).Recordset.EOF)
            //{
            //    oNewFeaturePoint.Components.GetComponent(iCNO).Recordset.AddNew("G3E_FID", lFID);
            //    oNewFeaturePoint.Components.GetComponent(iCNO).Recordset.Update("G3E_FNO", iFNO);
            //}

            //Symbol
            iCNO = (short)(iDetailID > 0 ? 10821 : 10820);
            if (oNewFeaturePoint.Components.GetComponent(iCNO).Recordset.EOF)
            {
                oNewFeaturePoint.Components.GetComponent(iCNO).Recordset.AddNew("G3E_FID", lFID);
                oNewFeaturePoint.Components.GetComponent(iCNO).Recordset.Update("G3E_FNO", iFNO);
                if (iDetailID > 0)
                    oNewFeaturePoint.Components.GetComponent(iCNO).Recordset.Update("G3E_DETAILID", iDetailID);
            }
            else
            {
                oNewFeaturePoint.Components.GetComponent(iCNO).Recordset.MoveLast();
            }

            // GetCompInfo iFNO, iCNO, bMainGr, grType
            oOrPointGeom = PGeoLib.CreatePointGeom(xStubStump, yStubStump);
            oNewFeaturePoint.Components.GetComponent(iCNO).Geometry = oOrPointGeom;


            mobjRelationshipService = GTClassFactory.Create<IGTRelationshipService>(m_oGTCustomCommandHelper);
            mobjRelationshipService.DataContext = application.DataContext;
            //Connect
            mobjRelationshipService.ActiveFeature = oNewFeature;
            if (mobjRelationshipService.AllowSilentEstablish(oNewFeaturePoint))
            {
                mobjRelationshipService.SilentEstablish(mintNRRelationshipNumber, oNewFeaturePoint);
            }

            IGTKeyObject oTermFeature = application.DataContext.OpenFeature(iTermFNO, iTermFID);
            //Connect
            mobjRelationshipService.ActiveFeature = oNewFeature;
            if (mobjRelationshipService.AllowSilentEstablish(oTermFeature))
            {
                mobjRelationshipService.SilentEstablish(13, oTermFeature);
            }

            return oNewFeature;
        }

        public void Activate(Intergraph.GTechnology.API.IGTCustomCommandHelper CustomCommandHelper)
        {
            try
            {
                application.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, mstrFistPoint);

                log = Logger.getInstance();
                log.WriteLog(this.GetType().FullName, "Stub/Stump Placement started!");
                log.WriteLog("");

                m_oGTCustomCommandHelper = CustomCommandHelper;

                cStubStump = new clsStumpStub();

                if (application.SelectedObjects.FeatureCount > 0)
                {

                    application.SelectedObjects.Clear();

                    //foreach (IGTDDCKeyObject oDDCKeyObject in application.SelectedObjects.GetObjects())
                    //{
                    //    switch (oDDCKeyObject.ComponentViewName)
                    //    {
                    //        case "VGC_CBL_L":
                    //        case "VGC_CBL_T":
                    //        case "VGC_DCBL_L":
                    //        case "VGC_DCBL_T":
                    //            {
                    //                iSourceCblFNO = oDDCKeyObject.FNO;
                    //                iSourceCblFID = oDDCKeyObject.FID;
                    //                break;
                    //            }
                    //    }
                    //}

                    //if (iSourceCblFID > 0)
                    //{
                    //    mptSelected = LoadTerminationPoint(iSourceCblFNO, iSourceCblFID);
                    //    cStubStump.SourceCblFID = iSourceCblFID;
                    //    cStubStump.Side = CableSide;
                    //    cStubStump.StartPoint = mptSelected;
                    //}
                }

                //  Assigns the private member variables their default values.
                mintState = 1;

                iDetailID = application.ActiveMapWindow.DetailID;

                mobjEditService = GTClassFactory.Create<IGTGeometryEditService>();
                mobjEditService.TargetMapWindow = application.ActiveMapWindow;

                mobjEditPointService = GTClassFactory.Create<IGTGeometryEditService>();
                mobjEditPointService.TargetMapWindow = application.ActiveMapWindow;

                mobjEditServiceTemp = GTClassFactory.Create<IGTGeometryEditService>();
                mobjEditServiceTemp.TargetMapWindow = application.ActiveMapWindow;

                mobjLocateService = application.ActiveMapWindow.LocateService;

                mobjExplorerService = GTClassFactory.Create<IGTFeatureExplorerService>();

                RegisterEvents();
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

            UnRegisterEvents();

            if (m_oGTTransactionManager != null)
            {
                if (m_oGTTransactionManager.TransactionInProgress)
                {
                    m_oGTTransactionManager.Rollback();
                }
            }

            log.CloseFile();
            application.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Exiting...");
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

            CloseFeatureExplorer();

            mptSelected = null;

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
                //UnsubscribeEvents();

                application.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Exited.");


            }
            catch (Exception ex)
            {
            }
        }

        private void m_oCustomCommandHelper_MouseUp(object sender, GTMouseEventArgs MouseEventArgs)
        {

            try
            {
                IGTPoint WorldPoint = MouseEventArgs.WorldPoint;

                if (MouseEventArgs.Button == 2)
                {
                    if (mintState == 1)
                    {
                        mintState = -1;

                        cStubStump.Type = "Stub";

                        if ((cStubStump.SourceCblFID > 0) && (!string.IsNullOrEmpty(cStubStump.Side)))
                            LoadAttributeDialog();
                        else
                            mintState = 2;
                    }
                    else if (mintState == 3)
                    {
                        // wait for attribute
                    }
                    else if (mintState == 5)
                    {
                        mintState = -1;

                        if (mobjEditServiceTemp.GeometryCount > 0) mobjEditServiceTemp.RemoveAllGeometries();

                        m_oGTTransactionManager.Begin("Place new stub");
                        CreateStub(cStubStump);
                        m_oGTTransactionManager.Commit();
                        m_oGTTransactionManager.RefreshDatabaseChanges();

                        ExitCmd();

                        //application.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Define next point of stub or right-click to accept");
                        mintState = 8;
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
                        mintState = -1;

                        cStubStump.Type = "Stump";

                        if ((cStubStump.SourceCblFID > 0) && (!string.IsNullOrEmpty(cStubStump.Side)))
                            LoadAttributeDialog();
                        else
                            mintState = 2;
                    }
                    else if ((mintState == 2))
                    {
                        double X = WorldPoint.X;
                        double Y = WorldPoint.Y;

                        mintState = -1;
                        IGTDDCKeyObjects objs = mobjLocateService.Locate(WorldPoint, 10, 5, GTSelectionTypeConstants.gtmwstSelectSingle);

                        iTermFID = 0;
                        foreach (IGTDDCKeyObject oDDCKeyObject in objs)
                        {
                            // edited : m.zam @ 12-10-2012 - include CAB as start point
                            if (oDDCKeyObject.FNO == 10800 || oDDCKeyObject.FNO == 10300 || oDDCKeyObject.FNO == 10300 || oDDCKeyObject.FNO == 9300) // get joint properties
                            {
                                iTermFNO = oDDCKeyObject.FNO;
                                iTermFID = oDDCKeyObject.FID;
                            } 
                        }
                        if (iTermFID > 0)
                        {
                            mptSelected = LoadTerminationPoint(iTermFNO, iTermFID); // get geometry
                            if (mptSelected == null)
                                mintState = 2;
                            else
                            {
                                cStubStump.SourceCblFID = iSourceCblFID;
                                cStubStump.Side = CableSide;
                                cStubStump.StartPoint = mptSelected;
                                cStubStump.SourceDevFNO = iTermFNO;
                                cStubStump.SourceDevFID = iTermFID;
                                LoadAttributeDialog();
                                mintState = 3;
                            }
                            return;
                        }
                        mintState = 2;
                    }
                    else if (mintState == 3)
                    {
                        // wait for attribute
                    }
                    else if ((mintState == 4))
                    {
                        mintState = -1;
                        double X = WorldPoint.X;
                        double Y = WorldPoint.Y;

                        _distance = 2;
                        if (mptSelected == null) mptSelected = WorldPoint;
                        double _angle = PGeoLib.GetAngle(mptSelected.X, mptSelected.Y, X, Y);
                        double xStubStump = mptSelected.X + Math.Cos(_angle) * _distance;
                        double yStubStump = mptSelected.Y - Math.Sin(_angle) * _distance;

                        if (mobjEditPointService.GeometryCount > 0) mobjEditPointService.RemoveAllGeometries();
                        if (mobjEditServiceTemp.GeometryCount > 0) mobjEditServiceTemp.RemoveAllGeometries();
                        if (mobjEditService.GeometryCount > 0) mobjEditService.RemoveAllGeometries();

                        cStubStump.StartPoint = mptSelected;
                        cStubStump.Angle = _angle;

                        m_oGTTransactionManager.Begin("Place new stump");
                        CreateStump(cStubStump);
                        m_oGTTransactionManager.Commit();
                        m_oGTTransactionManager.RefreshDatabaseChanges();

                        if (mobjEditPointService.GeometryCount > 0) mobjEditPointService.RemoveAllGeometries();
                        if (mobjEditServiceTemp.GeometryCount > 0) mobjEditServiceTemp.RemoveAllGeometries();
                        if (mobjEditService.GeometryCount > 0) mobjEditService.RemoveAllGeometries();

                        ExitCmd();

                        mintState = 8;
                    }
                    else if (mintState == 5)
                    {
                        mintState = -1;
                        mptPoint2 = WorldPoint;

                        if (cStubStump.Line == null)
                        {
                            cStubStump.Line = GTClassFactory.Create<IGTPolylineGeometry>();
                            cStubStump.Line.Points.Add(mptSelected);
                        }
                        cStubStump.Line.Points.Add(mptPoint2);

                        if (mobjEditPointService.GeometryCount > 0) mobjEditPointService.RemoveAllGeometries();
                        if (mobjEditServiceTemp.GeometryCount > 0) mobjEditServiceTemp.RemoveAllGeometries();
                        if (mobjEditService.GeometryCount > 0) mobjEditService.RemoveAllGeometries();

                        mintState = 5;
                    }
                    else if (mintState == 6)
                    {
                        mintState = -1;

                        if (mobjEditPointService.GeometryCount > 0) mobjEditPointService.RemoveAllGeometries();
                        if (mobjEditServiceTemp.GeometryCount > 0) mobjEditServiceTemp.RemoveAllGeometries();
                        if (mobjEditService.GeometryCount > 0) mobjEditService.RemoveAllGeometries();

                        mintState = 7;
                    }
                    else if (mintState == 7)
                    {
                        mintState = -1;

                        m_oGTTransactionManager.Begin("Place new stub");
                        //CreateStub(cManhole, mobjEditService);
                        m_oGTTransactionManager.Commit();
                        m_oGTTransactionManager.RefreshDatabaseChanges();

                        if (mobjEditPointService.GeometryCount > 0) mobjEditPointService.RemoveAllGeometries();
                        if (mobjEditServiceTemp.GeometryCount > 0) mobjEditServiceTemp.RemoveAllGeometries();
                        if (mobjEditService.GeometryCount > 0) mobjEditService.RemoveAllGeometries();

                        ExitCmd();
                        mintState = 8;
                    }
                    else
                        ExitCmd();
                }
            }
            catch (Exception ex)
            {
                //Logger log = Logger.GetInstance();
                log.WriteLog("Error", "m_oCustomCommandHelper_MouseUp", ex.Message);
                log.WriteLog(ex.StackTrace);
                MessageBox.Show(ex.Message, "Error.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                if (m_oGTTransactionManager != null) m_oGTTransactionManager.Rollback();
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
                    application.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Left-click for Stump or right-click for Stub");
                }
                else if ((mintState == 2))
                {
                    application.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Identify e-side/d-side joint");
                    double X = WorldPoint.X;
                    double Y = WorldPoint.Y;

                    mptSelected = null;
                    IGTDDCKeyObjects objs = mobjLocateService.Locate(WorldPoint, 10, 5, GTSelectionTypeConstants.gtmwstSelectAll);

                    application.ActiveMapWindow.HighlightedObjects.Clear();
                    application.ActiveMapWindow.HighlightedObjects.AddMultiple(objs);
                }
                else if ((mintState == 3))
                {
                    application.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Edit and save attributes in Feature Explorer");
                    if (!mobjExplorerService.Visible) mobjExplorerService.Visible = true;
                    mobjExplorerService.Slide(true);
    
                }
                else if (mintState == 4)
                {
                    double X = WorldPoint.X;
                    double Y = WorldPoint.Y;

                    _distance = 2;
                    if (mptSelected == null) mptSelected = WorldPoint;
                    double _angle = PGeoLib.GetAngle(mptSelected.X, mptSelected.Y, X, Y);
                    double xStubStump = mptSelected.X + Math.Cos(_angle) * _distance;
                    double yStubStump = mptSelected.Y - Math.Sin(_angle) * _distance;

                    IGTLineGeometry oLine = PGeoLib.CreateLineGeom(mptSelected.X, mptSelected.Y, xStubStump, yStubStump);
                    if (mobjEditServiceTemp.GeometryCount > 0) mobjEditServiceTemp.RemoveAllGeometries();
                    mobjEditServiceTemp.AddGeometry(oLine, 11516);

                    if (mobjEditPointService.GeometryCount > 0) mobjEditPointService.RemoveAllGeometries();
                    IGTOrientedPointGeometry oStumpPoint;
                    oStumpPoint = PGeoLib.CreatePointGeom(xStubStump, yStubStump);
                    mobjEditPointService.AddGeometry(oStumpPoint, 10820003);

                    application.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Point to accept angle or right-click to exit");
                }
                else if (mintState == 5)
                {
                    mptPoint2 = MouseEventArgs.WorldPoint;
                    if (mptPoint1 == null) mptPoint1 = mptSelected;

                    IGTPolylineGeometry oLine = GTClassFactory.Create<IGTPolylineGeometry>();

                    if (cStubStump.Line == null)
                        oLine.Points.Add(mptSelected);
                    else
                    {
                        foreach (IGTPoint pnt in cStubStump.Line.Points)
                            oLine.Points.Add(pnt);
                    }

                    oLine.Points.Add(mptPoint2);

                    //IGTOrientedPointGeometry oManhole;
                    //IGTPoint pnttmp = PGeoLib.GetLength(oLine, PGeoLib.GetLength(oLine)/2);

                    //oManhole = PGeoLib.CreatePointGeom(pnttmp.X, pnttmp.Y);

                    if (mobjEditServiceTemp.GeometryCount > 0) mobjEditServiceTemp.RemoveAllGeometries();
                    mobjEditServiceTemp.AddGeometry(oLine, 11516);
                    //mobjEditServiceTemp.AddGeometry(oManhole, StyleTempManhole);
                    application.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Define next point of stub or right-click to accept");
                }
                else if (mintState == 6)
                {
                    application.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Identify [+] to be wall 1");
                }
                else if (mintState == 7)
                {
                    IGTPoint mptPoint2 = MouseEventArgs.WorldPoint;

                    IGTTextPointGeometry oText = PGeoLib.CreateTextGeom(mptPoint2.X, mptPoint2.Y, "[Manhole label]", 0, GTAlignmentConstants.gtalTopLeft);
                    if (mobjEditServiceTemp.GeometryCount > 0) mobjEditServiceTemp.RemoveAllGeometries();
                    mobjEditServiceTemp.AddGeometry(oText, 30700);
                    application.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Enter new location for text");
                }
                //System.Windows.Forms.Application.DoEvents();
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

        private void LoadAttributeDialog()
        {
            if (cStubStump == null) return;

            if ((cStubStump.SourceCblFID > 0) && (!string.IsNullOrEmpty(cStubStump.Side)))
            {
                application.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Enter attribute for Stub/Stump");

                m_oGTTransactionManager.Begin("Place Stub/Stump Attribute");
                mobjAttribute = application.DataContext.NewFeature(7000, true);
                if (iSourceCbl != null) // CNO : 7001 - GC_CBL
                {
                    PGeoLib.CopyComponentAttribute(iSourceCbl, mobjAttribute);
                    mobjAttribute.Components.GetComponent(7001).Recordset.Update("CUSAGE", cStubStump.Type.ToUpper());
                    mobjAttribute.Components.GetComponent(7001).Recordset.Update("TEXT_FORMAT", 5);
                }
                else
                {
                    mobjAttribute.Components.GetComponent(7001).Recordset.Update("CUSAGE", cStubStump.Type.ToUpper());
                    mobjAttribute.Components.GetComponent(7001).Recordset.Update("TEXT_FORMAT", 5);
                }

                mobjExplorerService.ExploreFeature(mobjAttribute, cStubStump.Type + " " + cStubStump.Side);
                mblnVisible = mobjExplorerService.Visible;

                mobjExplorerService.Visible = true;
                mobjExplorerService.Slide(true);

                mintState = 3;
            }
        }

        private void SaveAttribute()
        {
            if (cStubStump == null) return;

            if (mintState == 3)
            {
                if (cStubStump.Type == "Stump")
                    mintState = 4;
                else
                    mintState = 5;

                m_oGTTransactionManager.Rollback();

                if ((!(mobjExplorerService == null)
                        && mblnVisible))
                {
                    mobjExplorerService.Visible = false;
                }
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
                //m_oCustomCommandHelper.MouseMove += new EventHandler(this.m_oCustomCommandHelper_MouseMove);

                mobjExplorerService.CancelClick -= new EventHandler(mobjExplorerService_CancelClick);
                mobjExplorerService.SaveAndContinueClick -= new EventHandler(mobjExplorerService_SaveAndContinueClick);
                mobjExplorerService.SaveClick -= new EventHandler(mobjExplorerService_SaveClick);
            }
            catch (Exception ex)
            {
                //Logger log = Logger.GetInstance();
                log.WriteLog("Error", "UnRegisterEvents", ex.Message);
                log.WriteLog(ex.StackTrace);
                //MessageBox.Show(ex.Message, "Error.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //log.CloseFile();
            }
        }
        #endregion

    }
}
