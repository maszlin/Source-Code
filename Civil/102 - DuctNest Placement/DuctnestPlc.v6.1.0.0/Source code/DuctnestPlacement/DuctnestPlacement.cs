using ADODB;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Intergraph.GTechnology.Interfaces;
using Intergraph.GTechnology.API;
using System.Diagnostics;
using NEPS.GTechnology.Utilities;

namespace NEPS.GTechnology.NEPSDuctNestPlc
{
    class GTDuctnestPlacement : Intergraph.GTechnology.Interfaces.IGTCustomCommandModeless
    {
        private Intergraph.GTechnology.API.IGTTransactionManager m_oGTTransactionManager;
        private Intergraph.GTechnology.API.IGTCustomCommandHelper m_oGTCustomCommandHelper;
        private static Intergraph.GTechnology.API.IGTApplication application = null;//GTClassFactory.Create<IGTApplication>();
        private DuctnestConfigForm _DuctnestConfigForm;
        private Logger log;
        private IGTDataContext oDataContext = null;//application.DataContext;
        private static IGTPolylineGeometry oLineGeom = null;
        private static List<IGTGeometry> arrIntersection = null;//new List<IGTGeometry>();
        private static List<IGTGeometry> arrIntersectionText = null;//new List<IGTGeometry>();
        private static List<IGTGeometry> arrIntersectionText1 = null;//new List<IGTGeometry>();
        private string mstrFistPoint = "Duct Nest Placement is running...";
        private int lStyleId = 11508;
        private bool OneSidePlacement = false;


        private class DuctPosition
        {
           
            public string PosFrom;
            public string PosTo;
            public int ColFrom;
            public int RowFrom;
            public int ColTo;
            public int RowTo;
            private int _fid = 0;
            public int FID
            {
                set { _fid = value; }
                get { return _fid; }
            }
        };
        private List<DuctPosition> DuctPositions = null;

        int mintState;
        //IGTGeometryCreationService mobjCreationService;
        //IGTFeaturePlacementService mobjPlacementService;
        IGTGeometryEditService mobjEditServiceFrom;
        IGTGeometryEditService mobjEditServiceTo;
        IGTGeometryEditService mobjEditServiceTemp;
        //IGTKeyObject mobjContourFeature;
        IGTPoint mptSelected;
        IGTPoint mptPoint1;
        IGTPoint mptPoint2;

        private short iPathFNO = 2200;
        private int iPathFID = 0;
        private string PathState="PPF";
        private string DT_MH_FRM_WALL = "";
        private string DT_MH_TO_WALL = "";

        private string from_sourcename = "";
        private string to_sourcename = "";

        private IGTKeyObject oDuctpath = null;
        private IGTGeometry iPathGeom = null;
        private static DuctNest _DuctNest = null;
        private int numberofDucts = 0;
        private int numberofDuctsInNest = 0;

        private int StyleDuct = 2320067;
        private int StyleDuctnest = 2410001;
        private int StyleDuctnestText = 2430001;

        double DuctHSpace = 15;
        double DuctVSpace = 15;

        double DuctHOffset = 15;
        double DuctVOffset = 15;

        short fromFNO = 0;
        int fromFID = 0;
        short fromWall;

        short toFNO = 0;
        int toFID = 0;
        short toWall;

        IGTMapWindow FromWindow = null;
        IGTMapWindow ToWindow = null;

        string WindowCaption = "MapWindow 1";

        double Pi = 3.141592653589793;

        Dictionary<string, string> Parameters = new Dictionary<string, string>();
        Messages frmMsg = new Messages();

        private void ShowMessage(string str)
        {
            frmMsg.lblMessage.Text = str;
            frmMsg.Show();
        }

        private void HideMessage()
        {
            frmMsg.Hide();
        }

        private void LoadParameters()
        {
            Recordset rsComp = null;
            int recordsAffected = 0;

            string sSql = "select b.G3E_TYPE, b.G3E_PARAMETER1 from G3E_DUCTIFACE b";
            rsComp = application.DataContext.Execute(sSql, out recordsAffected, (int)CommandTypeEnum.adCmdText, null);

            if (rsComp != null)
            {
                while (!rsComp.EOF)
                {
                    if (rsComp.Fields["G3E_TYPE"].Value != DBNull.Value) Parameters.Add(rsComp.Fields["G3E_TYPE"].Value.ToString(), rsComp.Fields["G3E_PARAMETER1"].Value.ToString());

                    rsComp.MoveNext();
                }
            }
            rsComp = null;
//            DuctHSpace =  0.250;
//            DuctVSpace=0.250;
//            DuctHOffset = 0.1;
//            DuctVOffset=0.0;
////.235
//.1
//0
            if (Parameters["G3E_DHSPACING"] != null) DuctHSpace = Convert.ToDouble(Parameters["G3E_DHSPACING"]);
            if (Parameters["G3E_DVSPACING"] != null) DuctVSpace = Convert.ToDouble(Parameters["G3E_DVSPACING"]);

            if (Parameters["G3E_DHOFFSET"] != null) DuctHOffset = Convert.ToDouble(Parameters["G3E_DHOFFSET"]);
            if (Parameters["G3E_DVOFFSET"] != null) DuctVOffset = Convert.ToDouble(Parameters["G3E_DVOFFSET"]);
        }

        private void LocateFeature(short iFNO, int lFID, IGTMapWindow window, string wallnum) //wallnum TO ZOOM IN WINDOW ON SOURCE/TERM WALL OF DUCT PATH,AN_MOD
        {
            if (window == null) return;

            IGTDDCKeyObjects feat = application.DataContext.GetDDCKeyObjects(iFNO, lFID, GTComponentGeometryConstants.gtddcgAllGeographic);
            if (wallnum == "0")
            {
                for (int K = 0; K < feat.Count; K++)
                {
                    string t = feat[K].ComponentViewName.ToString();
                    if (feat[K].ComponentViewName == "VGC_PSGCON_S")
                    {
                        IGTWorldRange range = GTClassFactory.Create<IGTWorldRange>();
                        IGTPoint point1 = GTClassFactory.Create<IGTPoint>();
                        IGTPoint point2 = GTClassFactory.Create<IGTPoint>();
                        point1.X = feat[K].Geometry.FirstPoint.X - 2;
                        point1.Y = feat[K].Geometry.FirstPoint.Y - 2;
                        range.BottomLeft = point1;
                        point2.X = feat[K].Geometry.FirstPoint.X + 2;
                        point2.Y = feat[K].Geometry.FirstPoint.Y + 2;
                        range.TopRight = point2;
                        window.ZoomArea(range);
                        application.RefreshWindows();
                        return;
                    }
                }
                return;
            }          
            for (int K = 0; K < feat.Count; K++)
            {
                if (feat[K].ComponentViewName == "VGC_MANHLW_T" ||
                    feat[K].ComponentViewName == "VGC_CHAMBERWALL_T" ||
                    feat[K].ComponentViewName == "VGC_TUNNELWALL_T") //AN_MOD
                {
                    for (int i = 0; i < feat[K].Recordset.Fields.Count; i++)//AN_MOD
                    {
                        if ((feat[K].Recordset.Fields[i].Name == "WALL_NUM") && (feat[K].Recordset.Fields[i].Value.ToString() == wallnum))//AN_MOD
                        {
                            application.SelectedObjects.Add(GTSelectModeConstants.gtsosmSelectedComponentsOnly, feat[K]);
                            IGTWorldRange range = GTClassFactory.Create<IGTWorldRange>();
                            IGTPoint point1 = GTClassFactory.Create<IGTPoint>();
                            IGTPoint point2 = GTClassFactory.Create<IGTPoint>();
                            point1.X = feat[K].Geometry.FirstPoint.X - 2;
                            point1.Y = feat[K].Geometry.FirstPoint.Y - 2;
                            range.BottomLeft = point1;
                            point2.X = feat[K].Geometry.FirstPoint.X + 2;
                            point2.Y = feat[K].Geometry.FirstPoint.Y + 2;
                            range.TopRight = point2;
                            window.ZoomArea(range);
                            application.RefreshWindows();
                            return;
                        }

                    }
                    
                    
                }
            }
            application.RefreshWindows();
        }

        public bool CheckDuctpath()
        {
            Recordset rsComp = null;
            int recordsAffected = 0;

            if (iPathFID == 0) return false;
            string sSql = "select nvl(b.DT_WAYS,0) DT_WAYS, nvl(b.DT_NEST_WAYS,0) DT_NEST_WAYS from GC_COND b where b.G3E_FID=" + iPathFID.ToString();
            rsComp = application.DataContext.Execute(sSql, out recordsAffected, (int)CommandTypeEnum.adCmdText, null);

            if (rsComp != null)
            {
                if (!rsComp.EOF)
                {
                    if (rsComp.Fields["DT_WAYS"].Value != DBNull.Value) numberofDucts = Convert.ToInt32(rsComp.Fields["DT_WAYS"].Value);
                    if (rsComp.Fields["DT_NEST_WAYS"].Value != DBNull.Value) numberofDuctsInNest = Convert.ToInt32(rsComp.Fields["DT_NEST_WAYS"].Value);

                    if (numberofDucts == 0) return false;
                    if (numberofDucts <= numberofDuctsInNest) return false;
                }
            }
            rsComp = null;

            sSql = "select * from GC_NE_CONNECT a where a.g3e_fno in (2700,2800,3300,3800) and a.node1_id in (select node1_id from GC_NE_CONNECT b where b.G3E_FID=" + iPathFID.ToString() + ")";
            rsComp = application.DataContext.Execute(sSql, out recordsAffected, (int)CommandTypeEnum.adCmdText, null);

            if (rsComp != null)
            {
                if (!rsComp.EOF)
                {
                    if (rsComp.Fields["G3E_FNO"].Value != DBNull.Value) fromFNO = Convert.ToInt16(rsComp.Fields["G3E_FNO"].Value);
                    if (rsComp.Fields["G3E_FID"].Value != DBNull.Value) fromFID = Convert.ToInt32(rsComp.Fields["G3E_FID"].Value);
                }
            }
            rsComp = null;

            sSql = "select * from GC_NE_CONNECT a where a.g3e_fno in (2700,2800,3300,3800) and a.node1_id in (select node2_id from GC_NE_CONNECT b where b.G3E_FID=" + iPathFID.ToString() + ")";
            rsComp = application.DataContext.Execute(sSql, out recordsAffected, (int)CommandTypeEnum.adCmdText, null);

            if (rsComp != null)
            {
                if (!rsComp.EOF)
                {
                    if (rsComp.Fields["G3E_FNO"].Value != DBNull.Value) toFNO = Convert.ToInt16(rsComp.Fields["G3E_FNO"].Value);
                    if (rsComp.Fields["G3E_FID"].Value != DBNull.Value) toFID = Convert.ToInt32(rsComp.Fields["G3E_FID"].Value);
                }
            }
            rsComp = null;
             //=====================================   
            //TO FIT SOURCE AND TERM WALL OF DUCT PATH, AN_MOD
            IGTKeyObject oDuctpath1 = application.DataContext.OpenFeature(iPathFNO, iPathFID);
            for (int i = 0; i < oDuctpath1.Components.GetComponent(2201).Recordset.Fields.Count; i++)
            {
                if (oDuctpath1.Components.GetComponent(2201).Recordset.Fields[i].Name == "DT_MH_FRM_WALL")
                    DT_MH_FRM_WALL = oDuctpath1.Components.GetComponent(2201).Recordset.Fields[i].Value.ToString();
                if (oDuctpath1.Components.GetComponent(2201).Recordset.Fields[i].Name == "DT_MH_TO_WALL")
                    DT_MH_TO_WALL = oDuctpath1.Components.GetComponent(2201).Recordset.Fields[i].Value.ToString();
            }
            for (int i = 0; i < oDuctpath1.Components.GetComponent(51).Recordset.Fields.Count; i++)
            {
                if (oDuctpath1.Components.GetComponent(51).Recordset.Fields[i].Name == "FEATURE_STATE")
                    PathState = oDuctpath1.Components.GetComponent(51).Recordset.Fields[i].Value.ToString();
                
            }
            //=========================================
            
                 from_sourcename = FeatureName(fromFNO, fromFID);

                 if (from_sourcename == "Manhole")
                     from_sourcename += " ID=" + Get_Value("select MANHOLE_ID from GC_MANHL where g3e_fid = " + fromFID.ToString());
                 else from_sourcename += " FID=" + fromFID.ToString();

                 

                 to_sourcename = FeatureName(toFNO, toFID);
                 if (to_sourcename == "Manhole")
                     to_sourcename += " ID=" + Get_Value("select MANHOLE_ID from GC_MANHL where g3e_fid = " + toFID.ToString());
                 else to_sourcename += " FID=" + toFID.ToString();

            return true;
        }

        private IGTPolylineGeometry CreateNestGeom(double X, double Y, DuctNest _ductnest, ConnectType type)
        {
            IGTPoint objPoint;
            IGTPolylineGeometry oOrLineGeom = null;
            double dRotation = 0;

            int Cols = 0;
            int Rows = 0;

            if (type == ConnectType.from)
            {
                Cols = _ductnest.From_Cols;
                Rows = _ductnest.From_Rows;
            }
            else
            {
                Cols = _ductnest.To_Cols;
                Rows = _ductnest.To_Rows;
            }

            oOrLineGeom = GTClassFactory.Create<IGTPolylineGeometry>();

            objPoint = GTClassFactory.Create<IGTPoint>();
            objPoint.X = X;
            objPoint.Y = Y;
            oOrLineGeom.Points.Add(objPoint);
            objPoint = GTClassFactory.Create<IGTPoint>();
            objPoint.X = X + Cols * DuctHSpace;
            objPoint.Y = Y;
            oOrLineGeom.Points.Add(objPoint);
            objPoint = GTClassFactory.Create<IGTPoint>();
            objPoint.X = X + Cols * DuctHSpace;
            objPoint.Y = Y + Rows * DuctVSpace;
            oOrLineGeom.Points.Add(objPoint);
            objPoint = GTClassFactory.Create<IGTPoint>();
            objPoint.X = X;
            objPoint.Y = Y + Rows * DuctVSpace; ;
            oOrLineGeom.Points.Add(objPoint);
            objPoint = GTClassFactory.Create<IGTPoint>();
            objPoint.X = X;
            objPoint.Y = Y;
            oOrLineGeom.Points.Add(objPoint);

            return oOrLineGeom;
        }

        private void CreateDuctnestGeom(double X, double Y, DuctNest _ductnest, ConnectType type, IGTGeometryEditService mobjEditPointService)
        {
            if (_ductnest == null) return;
            if (mobjEditPointService == null) return;

            double ductX = 0;
            double ductY = 0;
            IGTOrientedPointGeometry oOrPointGeom = null;
            IGTPolylineGeometry oOrLineGeom = null;
            if (mobjEditPointService.GeometryCount > 0) mobjEditPointService.RemoveAllGeometries();

            oOrLineGeom = CreateNestGeom(X, Y, _ductnest, type);
            mobjEditPointService.AddGeometry(oOrLineGeom, StyleDuctnest);

            if (type == ConnectType.from)
            {
                foreach (DuctCtrl _duct in _ductnest.FromDucts)
                {
                    ductX = X + DuctHOffset + (_duct.Col) * DuctHSpace;
                    ductY = Y + DuctVOffset + (_duct.Row + 1) * DuctVSpace;//TO FIT INTO FORMATION IN TEMPORARY GEOMETRY, AN_MOD
                    oOrPointGeom = PGeoLib.CreatePointGeom(ductX, ductY);
                    mobjEditPointService.AddGeometry(oOrPointGeom, StyleDuct);
                }
            }
            else
            {
                foreach (DuctCtrl _duct in _ductnest.ToDucts)
                {
                    ductX = X + DuctHOffset + (_duct.Col) * DuctHSpace;
                    ductY = Y + DuctVOffset + (_duct.Row + 1) * DuctVSpace;//TO FIT INTO FORMATION IN TEMPORARY GEOMETRY, AN_MOD
                    oOrPointGeom = PGeoLib.CreatePointGeom(ductX, ductY);
                    mobjEditPointService.AddGeometry(oOrPointGeom, StyleDuct);
                }
            }
            for (int i = 1; i <= mobjEditPointService.GeometryCount; i++)
                mobjEditPointService.UnselectAllKeyPoints(i);
        }

        private IGTKeyObject CreateDuct(double X, double Y,  IGTPoint point1, IGTPoint point2)
        {
            short iFNO;
            short iCNO;
            long lFID;
            double dRotation;
            IGTKeyObject oNewFeature;

            IGTPoint oPointGeom;
            IGTTextPointGeometry oOrTextGeom;
            IGTOrientedPointGeometry oOrPointGeom = null;

            iFNO = 2300;
         //   if(type==ConnectType.from)
            oNewFeature = application.DataContext.NewFeature(iFNO);

            // FID generation.
            lFID = oNewFeature.FID;

            // Every feature is imported in the Existing state.
            iCNO = 51; //GC_NETELEM
            // oNewFeature.Components.GetComponent(iCNO).Recordset.Update("JOB_STATE", "CLD");//AN_MOD, SHOULD SET BY TRIGGER
            oNewFeature.Components.GetComponent(iCNO).Recordset.Update("FEATURE_STATE", PathState);//AN_MOD, SHOULD BE ppf FOR JUST PLACED FEATURE
            oNewFeature.Components.GetComponent(iCNO).Recordset.Update("SWITCH_CENTRE_CLLI", "0");
            oNewFeature.Components.GetComponent(iCNO).Recordset.Update("BILLING_RATE", "DAY");
            oNewFeature.Components.GetComponent(iCNO).Recordset.Update("MIN_MATERIAL", "0");
            // Attribute
            iCNO = 2301;
            oNewFeature.Components.GetComponent(iCNO).Recordset.Update("FEATURE_TYPE", "THROUGH");

            // Duct Point
            
                iCNO = 2320;
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
            oOrPointGeom = PGeoLib.CreatePointGeom(X, Y);

            if (point2 != null)
            {
                double x = oOrPointGeom.FirstPoint.X;

                IGTPoint pntTmp = GTClassFactory.Create<IGTPoint>();
                pntTmp.X = point1.X + 100;
                pntTmp.Y = point1.Y;
                if (mobjEditServiceTemp.GeometryCount > 0) mobjEditServiceTemp.RemoveAllGeometries();
                mobjEditServiceTemp.TargetMapWindow = application.ActiveMapWindow;
                mobjEditServiceTemp.AddGeometry(oOrPointGeom, StyleDuct);
                mobjEditServiceTemp.BeginRotate(pntTmp, point1);
                mobjEditServiceTemp.Rotate(point2);
                mobjEditServiceTemp.EndRotate(point2);
                oNewFeature.Components.GetComponent(iCNO).Geometry = mobjEditServiceTemp.GetGeometry(1);
            }
            else
                oNewFeature.Components.GetComponent(iCNO).Geometry = oOrPointGeom;
            return oNewFeature;
        }

        private IGTKeyObject UpdateToDuct(double X, double Y, IGTPoint point1, IGTPoint point2, int FID)
        {
            short iFNO;
            short iCNO;
            int lFID = FID;
            double dRotation;
            IGTKeyObject oFeature;

            IGTPoint oPointGeom;
            IGTTextPointGeometry oOrTextGeom;
            IGTOrientedPointGeometry oOrPointGeom = null;

            iFNO = 2300;
            //   if(type==ConnectType.from)
            oFeature = application.DataContext.OpenFeature(iFNO,lFID);

            // Duct Point
             iCNO = 2322;
             if (oFeature.Components.GetComponent(iCNO).Recordset.EOF)
            {
                oFeature.Components.GetComponent(iCNO).Recordset.AddNew("G3E_FID", lFID);
                oFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_FNO", iFNO);
            }
            else
            {
                oFeature.Components.GetComponent(iCNO).Recordset.MoveLast();
            }

            // GetCompInfo iFNO, iCNO, bMainGr, grType
            oOrPointGeom = PGeoLib.CreatePointGeom(X, Y);

            if (point2 != null)
            {
                IGTPoint pntTmp = GTClassFactory.Create<IGTPoint>();
                pntTmp.X = point1.X + 100;
                pntTmp.Y = point1.Y;
                if (mobjEditServiceTemp.GeometryCount > 0) mobjEditServiceTemp.RemoveAllGeometries();
                mobjEditServiceTemp.TargetMapWindow = application.ActiveMapWindow;
                mobjEditServiceTemp.AddGeometry(oOrPointGeom, StyleDuct);
                mobjEditServiceTemp.BeginRotate(pntTmp, point1);
                mobjEditServiceTemp.Rotate(point2);
                mobjEditServiceTemp.EndRotate(point2);
                oFeature.Components.GetComponent(iCNO).Geometry = mobjEditServiceTemp.GetGeometry(1);
            }
            else
                oFeature.Components.GetComponent(iCNO).Geometry = oOrPointGeom;
            return oFeature;
        }

        private IGTKeyObject CreateDuctnest(double X, double Y, DuctNest _ductnest, ConnectType type, IGTPoint point2)
        {
            short iFNO;
            short iCNO;
            long lFID;
            double dRotation;
            string labelID = "";//AN_MOD to complete label with from/to manhole ID
            IGTKeyObject oNewFeature;

            IGTPoint oPointGeom;
            IGTTextPointGeometry oOrTextGeom;
            IGTPolylineGeometry oOrLineGeom = null;

            iFNO = 2400;
            if (_ductnest.FID > 0)
                oNewFeature = application.DataContext.OpenFeature(iFNO, _ductnest.FID);
            else
                oNewFeature = application.DataContext.NewFeature(iFNO);

            // FID generation.
            lFID = oNewFeature.FID;

            // Every feature is imported in the Existing state.
            iCNO = 51; //GC_NETELEM
            // oNewFeature.Components.GetComponent(iCNO).Recordset.Update("JOB_STATE", "CLD");//AN_MOD, SHOULD SET BY TRIGGER
            oNewFeature.Components.GetComponent(iCNO).Recordset.Update("FEATURE_STATE", PathState);//AN_MOD, SHOULD BE ppf FOR JUST PLACED FEATURE
            oNewFeature.Components.GetComponent(iCNO).Recordset.Update("SWITCH_CENTRE_CLLI", "PPJ");
            oNewFeature.Components.GetComponent(iCNO).Recordset.Update("BILLING_RATE", "DAY");
            oNewFeature.Components.GetComponent(iCNO).Recordset.Update("MIN_MATERIAL", "0");

            // Attribute
            iCNO = 2401;
            oNewFeature.Components.GetComponent(iCNO).Recordset.Update("FEATURE_TYPE", "STRAIGHT");
            
            // Ductnest Line  AND LABEL CONTENT
            labelID = Get_Value("select count(g3e_fid) from GC_CONTAIN where g3e_fno=2400 and g3e_ownerfid= " + iPathFID);
          
            if (type == ConnectType.from)
            {
                int count = int.Parse(labelID) + 1;
                labelID = "[" + count.ToString() + "] ";
                if (toFNO == 2700)
                    labelID += Get_Value("select MANHOLE_ID from GC_MANHL where g3e_fid = " + toFID.ToString());
                else labelID += toFID.ToString();
                iCNO = 2410;
            }
            else
            {
                labelID = "[" + labelID + "] ";

                if (fromFNO == 2700)
                    labelID += Get_Value("select MANHOLE_ID from GC_MANHL where g3e_fid = " + fromFID.ToString());
                else labelID += fromFID.ToString(); 
                iCNO = 2412;
            }

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
            oOrLineGeom = CreateNestGeom(X, Y, _ductnest, type);

            if (point2 != null)
            {
                IGTPoint point1 = GTClassFactory.Create<IGTPoint>();
                point1.X = X;
                point1.Y = Y;

                IGTPoint pntTmp = GTClassFactory.Create<IGTPoint>();
                pntTmp.X = X + 100;
                pntTmp.Y = Y;
                if (mobjEditServiceTemp.GeometryCount > 0) mobjEditServiceTemp.RemoveAllGeometries();
                mobjEditServiceTemp.TargetMapWindow = application.ActiveMapWindow;
                mobjEditServiceTemp.AddGeometry(oOrLineGeom, StyleDuctnest);
                mobjEditServiceTemp.BeginRotate(pntTmp, point1);
                mobjEditServiceTemp.Rotate(point2);
                mobjEditServiceTemp.EndRotate(point2);
                oNewFeature.Components.GetComponent(iCNO).Geometry = mobjEditServiceTemp.GetGeometry(1);
            }
            else
                oNewFeature.Components.GetComponent(iCNO).Geometry = oOrLineGeom;

            // Ductnest Label
            int Cols = 0;
            int Rows = 0;

            if (type == ConnectType.from)
            {
                iCNO = 2430;
                Cols = _ductnest.From_Cols;
                Rows = _ductnest.From_Rows;
            }
            else
            {
                iCNO = 2432;
                Cols = _ductnest.To_Cols;
                Rows = _ductnest.To_Rows;
            }

            for (int col =0;col<Cols;col++)
            {
                //if (oNewFeature.Components.GetComponent(iCNO).Recordset.EOF)
                //{
                    oNewFeature.Components.GetComponent(iCNO).Recordset.AddNew("G3E_FID", lFID);
                    oNewFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_FNO", iFNO);
                    oNewFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_TEXT", (col + 1).ToString());
                    oNewFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_ALIGNMENT", 4); 
                //}
                //else
                //{
                //    oNewFeature.Components.GetComponent(iCNO).Recordset.MoveLast();
                //}

                // GetCompInfo iFNO, iCNO, bMainGr, grType
                    oOrTextGeom = PGeoLib.CreateTextGeom(X + DuctHSpace / 2 + col * DuctHSpace, Y, (col + 1).ToString());

                if (point2 != null)
                {
                    IGTPoint point1 = GTClassFactory.Create<IGTPoint>();
                    point1.X = X;
                    point1.Y = Y;

                    IGTPoint pntTmp = GTClassFactory.Create<IGTPoint>();
                    pntTmp.X = X + 100;
                    pntTmp.Y = Y;
                    if (mobjEditServiceTemp.GeometryCount > 0) mobjEditServiceTemp.RemoveAllGeometries();
                    mobjEditServiceTemp.TargetMapWindow = application.ActiveMapWindow;
                    mobjEditServiceTemp.AddGeometry(oOrTextGeom, StyleDuctnestText);
                    mobjEditServiceTemp.BeginRotate(pntTmp, point1);
                    mobjEditServiceTemp.Rotate(point2);
                    mobjEditServiceTemp.EndRotate(point2);
                    oNewFeature.Components.GetComponent(iCNO).Geometry = mobjEditServiceTemp.GetGeometry(1);
                }
                else
                    oNewFeature.Components.GetComponent(iCNO).Geometry = oOrTextGeom;
            }

            for (int row = 0; row < Rows; row++)
            {
                //if (oNewFeature.Components.GetComponent(iCNO).Recordset.EOF)
                //{
                    oNewFeature.Components.GetComponent(iCNO).Recordset.AddNew("G3E_FID", lFID);
                    oNewFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_FNO", iFNO);
                    oNewFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_TEXT", char.ConvertFromUtf32(65 + row));
                    oNewFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_ALIGNMENT", 2); 
                //}
                //else
                //{
                //    oNewFeature.Components.GetComponent(iCNO).Recordset.MoveLast();
                //}

                    oOrTextGeom = PGeoLib.CreateTextGeom(X, Y + DuctVSpace / 2 + row * DuctVSpace, char.ConvertFromUtf32(65 + row));

                    if (point2 != null)
                    {
                        IGTPoint point1 = GTClassFactory.Create<IGTPoint>();
                        point1.X = X;
                        point1.Y = Y;

                        IGTPoint pntTmp = GTClassFactory.Create<IGTPoint>();
                        pntTmp.X = X + 100;
                        pntTmp.Y = Y;
                        if (mobjEditServiceTemp.GeometryCount > 0) mobjEditServiceTemp.RemoveAllGeometries();
                        mobjEditServiceTemp.TargetMapWindow = application.ActiveMapWindow;
                        mobjEditServiceTemp.AddGeometry(oOrTextGeom, StyleDuctnestText);
                        mobjEditServiceTemp.BeginRotate(pntTmp, point1);
                        mobjEditServiceTemp.Rotate(point2);
                        mobjEditServiceTemp.EndRotate(point2);
                        oNewFeature.Components.GetComponent(iCNO).Geometry = mobjEditServiceTemp.GetGeometry(1);
                    }
                    else
                        oNewFeature.Components.GetComponent(iCNO).Geometry = oOrTextGeom;
            }

            oNewFeature.Components.GetComponent(iCNO).Recordset.AddNew("G3E_FID", lFID);
            oNewFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_FNO", iFNO); // "[" + (type == ConnectType.from ? "1" : "2") + "] " + 
            oNewFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_TEXT",labelID);//AN_MOD to complete label with from/to manhole ID
                

            oOrTextGeom = PGeoLib.CreateTextGeom(X, Y + DuctVSpace / 2 + Rows * DuctVSpace, labelID);//"[" + (type == ConnectType.from ? "1" : "2") + "]");

            if (point2 != null)
            {
                IGTPoint point1 = GTClassFactory.Create<IGTPoint>();
                point1.X = X;
                point1.Y = Y;

                IGTPoint pntTmp = GTClassFactory.Create<IGTPoint>();
                pntTmp.X = X + 100;
                pntTmp.Y = Y;
                if (mobjEditServiceTemp.GeometryCount > 0) mobjEditServiceTemp.RemoveAllGeometries();
                mobjEditServiceTemp.TargetMapWindow = application.ActiveMapWindow;
                mobjEditServiceTemp.AddGeometry(oOrTextGeom, StyleDuctnestText);
                mobjEditServiceTemp.BeginRotate(pntTmp, point1);
                mobjEditServiceTemp.Rotate(point2);
                mobjEditServiceTemp.EndRotate(point2);
                oNewFeature.Components.GetComponent(iCNO).Geometry = mobjEditServiceTemp.GetGeometry(1);
            }
            else
                oNewFeature.Components.GetComponent(iCNO).Geometry = oOrTextGeom;

            return oNewFeature;
        }

        private void PlaceDuctnest(double X, double Y, DuctNest _ductnest, ConnectType type, IGTPoint point2)
        {
            IGTKeyObject oDuctnest;
            IGTKeyObject oDuct;
            IGTRelationshipService mobjRelationshipService;

            short mintContainRelationshipNumber = 7;

            string mstrTransactionName = "Place Ductnest";
            m_oGTTransactionManager.Begin(mstrTransactionName);
            mobjRelationshipService = GTClassFactory.Create<IGTRelationshipService>(m_oGTCustomCommandHelper);
            mobjRelationshipService.DataContext = application.DataContext;

            IGTPoint pntTmp = GTClassFactory.Create<IGTPoint>();
            pntTmp.X = X;
            pntTmp.Y = Y;

            oDuctnest = CreateDuctnest(X, Y, _ductnest, type, point2);
            _ductnest.FID = oDuctnest.FID;

            oDuctpath = application.DataContext.OpenFeature(iPathFNO, iPathFID);
            //Ownership        
            mobjRelationshipService.ActiveFeature = oDuctpath;
            if (mobjRelationshipService.AllowSilentEstablish(oDuctnest))
            {
                mobjRelationshipService.SilentEstablish(mintContainRelationshipNumber, oDuctnest);
            }

            if (type == ConnectType.from)
            {
                foreach (DuctCtrl _duct in _ductnest.FromDucts)
                {
                    _duct.X = X + DuctHOffset + (_duct.Col+0.1) * DuctHSpace;
                    _duct.Y = Y + DuctVOffset + (_duct.Row+0.5) * DuctVSpace; //TO FIT INTO FORMATION, AN_MOD
                     oDuct = CreateDuct(_duct.X, _duct.Y, pntTmp, point2);
                    _duct.FID = oDuct.FID;
                 //  DuctPositions
                    for (int k = 0; k < DuctPositions.Count; k++)
                    {
                        if (DuctPositions[k].RowFrom == _duct.Row && DuctPositions[k].ColFrom == _duct.Col)
                        {
                            DuctPositions[k].FID = _duct.FID;
                            break;
                        }
                    }
                    //Ownership        
                    mobjRelationshipService.ActiveFeature = oDuctnest;
                    if (mobjRelationshipService.AllowSilentEstablish(oDuct))
                    {
                        mobjRelationshipService.SilentEstablish(mintContainRelationshipNumber, oDuct);
                    }
                }
            }
            else
            {
                foreach (DuctCtrl _duct in _ductnest.ToDucts)
                {
                    _duct.X = X + DuctHOffset + (_duct.Col + 0.1) * DuctHSpace;
                    _duct.Y = Y + DuctVOffset + (_duct.Row + 0.5) * DuctVSpace;//TO FIT INTO FORMATION, AN_MOD

                    if (DT_MH_FRM_WALL == "" || DT_MH_FRM_WALL == "0")
                    {
                        oDuct = CreateDuct(_duct.X, _duct.Y, pntTmp, point2);
                        _duct.FID = oDuct.FID;
                        //  DuctPositions
                        for (int k = 0; k < DuctPositions.Count; k++)
                        {
                            if (DuctPositions[k].RowTo == _duct.Row && DuctPositions[k].ColTo == _duct.Col)
                            {
                                DuctPositions[k].FID = _duct.FID;
                                break;
                            }
                        }
                        //Ownership        
                        mobjRelationshipService.ActiveFeature = oDuctnest;
                        if (mobjRelationshipService.AllowSilentEstablish(oDuct))
                        {
                            mobjRelationshipService.SilentEstablish(mintContainRelationshipNumber, oDuct);
                        }
                    }
                    else
                    {
                        for (int k = 0; k < DuctPositions.Count; k++)
                        {
                            if (DuctPositions[k].RowTo == _duct.Row && DuctPositions[k].ColTo == _duct.Col)
                            {
                                oDuct = UpdateToDuct(_duct.X, _duct.Y, pntTmp, point2, DuctPositions[k].FID);
                                break;
                            }
                        }
                    }

                    // for (int k = 0; k < DuctPositions.Count; k++)
                   

                   // _duct.FID = oDuct.FID;
                    //Ownership        
                  //  mobjRelationshipService.ActiveFeature = oDuctnest;
                  //  if (mobjRelationshipService.AllowSilentEstablish(oDuct))
                  //  {
                  //      mobjRelationshipService.SilentEstablish(mintContainRelationshipNumber, oDuct);
                  //  }
                }
            }

            oDuctpath.Components.GetComponent(2201).Recordset.Update("DT_NEST_WAYS", numberofDuctsInNest + _ductnest.Total_duct);
            
            m_oGTTransactionManager.Commit();
            m_oGTTransactionManager.RefreshDatabaseChanges();
        }
        #region Get Value from Database //AN_MOD
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
                MessageBox.Show("No data found!", "DuctNest Placement", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return "";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "DuctNest Placement", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return "";
            }

        }
        #endregion

        private void LoadDuctnestConfig(int PathFid)
        {
            IGTMapWindow mobjMapWindow = application.ActiveMapWindow;
            if (_DuctnestConfigForm != null) _DuctnestConfigForm = null;
            _DuctnestConfigForm = new DuctnestConfigForm(m_oGTCustomCommandHelper);
            _DuctnestConfigForm.FormClosed += new FormClosedEventHandler(DuctnestConfigForm_FormClosed);
            _DuctnestConfigForm.PlaceClick += new EventHandler(DuctnestConfigForm_PlaceClick);
            _DuctnestConfigForm.DUCTPATH_FID = PathFid;
            _DuctnestConfigForm.LoadDuctnestConfig(from_sourcename, to_sourcename);
            _DuctnestConfigForm.Msg = "Ductpath contains " + (numberofDucts - numberofDuctsInNest) + " spare ductways";
            _DuctnestConfigForm.Show();
        }
    
    private void DuctnestConfigForm_PlaceClick(object sender, EventArgs e)
        {
            application.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Wait, two windows for placement are opening...");

            _DuctNest = _DuctnestConfigForm.DUCT_NEST;

            /* AN_MOD start*/
            if (DuctPositions == null)
                DuctPositions = new List<DuctPosition>();
            else DuctPositions.Clear();

            for (int i = 0; i < _DuctNest.Total_duct; i++)
            {
                DuctPosition temp = new DuctPosition();
                temp.ColFrom = _DuctNest.FromDucts[i].Col;
                temp.RowFrom= _DuctNest.FromDucts[i].Row;
                temp.PosFrom = _DuctNest.FromDucts[i].Text;
                temp.PosTo = _DuctNest.DuctMapping[temp.PosFrom];
                for (int j = 0; j < _DuctNest.Total_duct; j++)
                {
                    if(temp.PosTo ==_DuctNest.ToDucts[j].Text)
                    {
                        temp.ColTo = _DuctNest.ToDucts[j].Col;
                        temp.RowTo = _DuctNest.ToDucts[j].Row;
                        break;
                    }
                }
                DuctPositions.Add(temp);
            }
          
            _DuctnestConfigForm.Hide();
            WindowCaption = application.ActiveMapWindow.Caption;
           
    {
                 OneSidePlacement = false;
                 ToWindow = application.ActiveMapWindow;
                 FromWindow = application.NewMapWindow(application.ActiveMapWindow.LegendName);
                 application.ArrangeWindows(GTWindowActionConstants.gtapwaTileHorizontal);
                 FromWindow.Activate();

                 mobjEditServiceFrom.TargetMapWindow = FromWindow;
                 mobjEditServiceTo.TargetMapWindow = ToWindow;
                 FromWindow.Caption = "From " + from_sourcename;
                 ToWindow.Caption = "To " + to_sourcename;
                 PGeoLib.CopyWindowSetting(ToWindow, FromWindow);
                 application.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Define new FromDuctnest location. Point to accept, Right click to exit.");

                 LocateFeature(fromFNO, fromFID, FromWindow, DT_MH_FRM_WALL);
                 LocateFeature(toFNO, toFID, ToWindow, DT_MH_TO_WALL);
                 mintState = 2;
                 if (DT_MH_FRM_WALL == "" || DT_MH_FRM_WALL == "0")
                 {
                     ToWindow.Activate();
                     MessageBox.Show("Place only To Duct Nest", "DuctNest Placement", MessageBoxButtons.OK, MessageBoxIcon.Information);
                     mintState = 5;
                 }
             }
       

            /* AN_MOD end*/

          
        }

        void DuctnestConfigForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            _DuctnestConfigForm.Dispose();
            ExitCmd();
        }

        private string FeatureName(short FNO, int FID)
        {
            if (FNO == 2700) return "Manhole";
            else if (FNO == 2800) return "Civil Node";
            else if (FNO == 3800) return "Chamber";
            else if (FNO == 3300) 
            {
                string type = Get_Value("select TRENCH from GC_TUNNEL where g3e_fid = " + FID.ToString());
                if (type == "N")
                    return "Tunnel";
                else return "Trench";
            }
            return "";        

        }

        public void Activate(Intergraph.GTechnology.API.IGTCustomCommandHelper CustomCommandHelper)
        {
            try
            {
                if(application==null) application = GTClassFactory.Create<IGTApplication>();
                if(oDataContext==null)oDataContext = application.DataContext;
                oLineGeom = null;
         if(arrIntersection ==null)arrIntersection = new List<IGTGeometry>();
         if (arrIntersectionText == null) arrIntersectionText = new List<IGTGeometry>();
         if (arrIntersectionText1 == null) arrIntersectionText1 = new List<IGTGeometry>();
                application.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, mstrFistPoint);

                log = Logger.getInstance();
                log.WriteLog(this.GetType().FullName, "Ductnest Placement started!");
                log.WriteLog("");

                m_oGTCustomCommandHelper = CustomCommandHelper;

                if (application.SelectedObjects.FeatureCount == 0)
                {
                    MessageBox.Show("Please identify the ductpath to place nest", "DuctNest Placement", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    ExitCmd();
                    return;
                }

                if (application.SelectedObjects.FeatureCount > 1)
                {
                    MessageBox.Show("Please select single feature at once", "DuctNest Placement", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    ExitCmd();
                    return;
                }
               
                foreach (IGTDDCKeyObject oDDCKeyObject in application.SelectedObjects.GetObjects())
                {
                    
                    if (oDDCKeyObject.ComponentViewName == "VGC_COND_L")
                    {
                        iPathGeom = oDDCKeyObject.Geometry;
                        iPathFNO = oDDCKeyObject.FNO;
                        iPathFID = oDDCKeyObject.FID;
                        break;
                    }
                    if (oDDCKeyObject.FNO != 2200)
                    {
                        MessageBox.Show("Select Duct Path Feature!", "DuctNest Placement", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        ExitCmd();
                        return;
                    }
                }

                

                if (!CheckDuctpath())
                {
                    MessageBox.Show("All " + numberofDucts.ToString() + " ductways in this ductpath have already been assigned to nests", "DuctNest Placement", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    ExitCmd();
                    return;
                }

                //  Assigns the private member variables their default values.
                mintState = 1;

                mobjEditServiceFrom = GTClassFactory.Create<IGTGeometryEditService>();
                mobjEditServiceFrom.TargetMapWindow = application.ActiveMapWindow;

                mobjEditServiceTo = GTClassFactory.Create<IGTGeometryEditService>();
                mobjEditServiceTo.TargetMapWindow = application.ActiveMapWindow;

                mobjEditServiceTemp = GTClassFactory.Create<IGTGeometryEditService>();
                mobjEditServiceTemp.TargetMapWindow = application.ActiveMapWindow;

                RegisterEvents();

                LoadParameters();
                LoadDuctnestConfig(iPathFID);
            }
                
            catch (Exception e)
            {
                log.WriteLog("");
                log.WriteLog(this.GetType().FullName, "ERROR: " + e.Message);
                log.WriteLog(e.StackTrace);
                MessageBox.Show(e.Message, "DuctNest Placement", MessageBoxButtons.OK, MessageBoxIcon.Error);
                if (!_DuctnestConfigForm.IsDisposed)
                    _DuctnestConfigForm.Close();
                else
                    ExitCmd();
            }
        }

        public bool CanTerminate
        {
            get
            {
             //   if (mintState == 0)
              //      m_CustomForm.Hide();
                DialogResult retVal = MessageBox.Show("Do you want to discard your current changes and exit?", "DuctNest Placement", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (retVal == DialogResult.Yes)
                {
                    ExitCmd();
                    //  return true;
                }
                else
                {
                   // if (startdraw == 0)
                   //     m_CustomForm.Show();
                    return false;
                }

                return false;
            }
        }

        public void Pause()
        {
            mintState += 10000;
        }

        public void Resume()
        {
            mintState -= 10000;
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

        private void ExitCmd()
        {
            try
            {
                log.CloseFile();
                application.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Exiting...");
                if (mobjEditServiceFrom != null)
                {
                    mobjEditServiceFrom.RemoveAllGeometries();
                    mobjEditServiceFrom = null;
                }
                if (mobjEditServiceTo != null)
                {
                    mobjEditServiceTo.RemoveAllGeometries();
                    mobjEditServiceTo = null;
                }
                if (mobjEditServiceTemp != null)
                {
                    mobjEditServiceTemp.RemoveAllGeometries();
                    mobjEditServiceTemp = null;
                }
                if (FromWindow != null)
                {
                    FromWindow.Close();
                    FromWindow = null;
                }
                if (ToWindow != null)
                {
                    ToWindow.Caption = WindowCaption;
                    ToWindow.WindowState = GTWindowStateConstants.gtmwwsMaximize;
                    ToWindow.Activate();
                }
                application.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Exited.");
                if (m_oGTCustomCommandHelper != null) m_oGTCustomCommandHelper.Complete();
                if (m_oGTTransactionManager != null)
                {
                    if (m_oGTTransactionManager.TransactionInProgress)
                        m_oGTTransactionManager.Rollback();
                    m_oGTTransactionManager = null;
                }

        application = null;
        
        oLineGeom = null;
        if (arrIntersection != null)
        {
            arrIntersection.Clear();
            arrIntersection = null; ;
        }
        if (arrIntersectionText != null)
        {
            arrIntersectionText.Clear();
            arrIntersectionText = null; ;
        }
        if (arrIntersectionText1 != null)
        {
            arrIntersectionText1.Clear();
            arrIntersectionText1 = null; ;
        }
      
        DuctPositions = null;
        

        iPathFNO = 2200;
        iPathFID = 0;
        oDuctpath = null;
        iPathGeom = null;
        _DuctNest = null;
        numberofDucts = 0;
        numberofDuctsInNest = 0;

            }
            catch (Exception ex)
            {
                //Logger log = Logger.GetInstance();
                log.WriteLog("Error", "ExitCmd", ex.Message);
                log.WriteLog(ex.StackTrace);
                MessageBox.Show(ex.Message, "DuctNest Placement", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //log.CloseFile();
            }
        }

        private void m_oCustomCommandHelper_MouseUp(object sender, GTMouseEventArgs MouseEventArgs)
        {

            try
            {
                IGTPoint WorldPoint = MouseEventArgs.WorldPoint;

                double angle = 0;
                if (MouseEventArgs.Button == 2)
                {
                    if (mintState == 2)
                    {
                        ExitCmd();
                        return;
                    }
                    else if (mintState == 3)
                    {
                        angle = 0;
                        mptPoint2 = null;
                        mintState = 4;
                    }
                    else if (mintState == 6)
                    {
                        angle = 0;
                        mptPoint2 = null;
                        mintState = 7;
                    }
                    //else
                    //{
                    //    ExitCmd();
                    //    return;
                    //}
                }
                else
                {
                    //  If the current step in the command is the third step then get the selected point.
                    if (mintState == 2)
                    {
                        if (MouseEventArgs.MapWindow.Caption == ToWindow.Caption) return;


                        if (LengthBtwTwoPoints(WorldPoint.X,WorldPoint.Y, 0)>5)
                        {
                                MessageBox.Show("Not allowed to place DuctNest far than 5 m!", "DuctNest Placement", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                return;
                        }
                        mintState = -1;
                        mptSelected = null;
                        mptSelected = WorldPoint;
                        if (mobjEditServiceFrom.GeometryCount > 0) mobjEditServiceFrom.RemoveAllGeometries();
                        CreateDuctnestGeom(mptSelected.X, mptSelected.Y, _DuctNest, ConnectType.from, mobjEditServiceFrom);

                        IGTPoint objPoint = GTClassFactory.Create<IGTPoint>();
                        objPoint.X = mptSelected.X + 100;
                        objPoint.Y = mptSelected.Y;

                        mobjEditServiceFrom.BeginRotate(objPoint, mptSelected);
                        application.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Click a point to accept current angle or Right-click to skip rotate");
                        mintState = 3;
                    }
                    else if ((mintState == 3))
                    {
                        mintState = -1;
                        mptPoint2 = null;
                        mptPoint2 = WorldPoint;

                        angle = PGeoLib.GetAngle(mptSelected.X, mptSelected.Y, mptPoint2.X, mptPoint2.Y);
                        mintState = 4;
                    }
                    else if (mintState == 5)
                    {
                        if (MouseEventArgs.MapWindow.Caption == FromWindow.Caption) return;
                       
                        if (LengthBtwTwoPoints(WorldPoint.X, WorldPoint.Y, 1) > 5)
                        {
                            MessageBox.Show("Not allowed to place DuctNest far than 5 m!", "DuctNest Placement", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            return;
                        }

                        mintState = -1;
                        mptSelected = null;
                        mptSelected = WorldPoint;
                        if (mobjEditServiceTo.GeometryCount > 0) mobjEditServiceTo.RemoveAllGeometries();
                        CreateDuctnestGeom(mptSelected.X, mptSelected.Y, _DuctNest, ConnectType.to, mobjEditServiceTo);

                        IGTPoint objPoint = GTClassFactory.Create<IGTPoint>();
                        objPoint.X = mptSelected.X + 100;
                        objPoint.Y = mptSelected.Y;

                        mobjEditServiceTo.BeginRotate(objPoint, mptSelected);
                        application.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Click to ToWindow to accept or Right-click to skip rotate");
                        mintState = 6;
                    }
                    else if (mintState == 6)
                    {
                        mintState = -1;
                        mptPoint2 = null;
                        mptPoint2 = WorldPoint;

                        angle = PGeoLib.GetAngle(mptSelected.X, mptSelected.Y, mptPoint2.X, mptPoint2.Y);
                        mintState = 7;
                    }
                }

                if (mintState == 4)
                {
                    if (MouseEventArgs.MapWindow.Caption == ToWindow.Caption) return;

                    mintState = -1;
                    application.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Creating FromDuctnest. Please wait...");
                    ShowMessage("Creating FromDuctnest. Please wait...");
                    if (mobjEditServiceFrom.GeometryCount > 0) mobjEditServiceFrom.RemoveAllGeometries();
                    if (mobjEditServiceTemp.GeometryCount > 0) mobjEditServiceTemp.RemoveAllGeometries();
                    PlaceDuctnest(mptSelected.X, mptSelected.Y, _DuctNest, ConnectType.from, mptPoint2);
                    HideMessage();
                    application.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Define new ToDuctnest location in ToWindow.");
                    mintState = 5;
                    if (DT_MH_TO_WALL == "" || DT_MH_TO_WALL == "0")
                    {
                        mintState = 0;
                        if (!CheckDuctpath())
                        {
                            ExitCmd();
                            return;
                        }
                        if (FromWindow != null)
                        {
                            FromWindow.Close();
                            FromWindow = null;
                        }
                        if (ToWindow != null)
                        {
                            ToWindow.Caption = WindowCaption;
                            ToWindow.WindowState = GTWindowStateConstants.gtmwwsMaximize;
                            LocateFeature(fromFNO, fromFID, ToWindow, DT_MH_FRM_WALL);
                          //  LocateFeature(toFNO, toFID, ToWindow, DT_MH_TO_WALL);

                            ToWindow.Activate();
                        }
                        LoadDuctnestConfig(iPathFID);
                    }
                    //  ExitCmd();
                    //  mintState = 8;
                    return;
                }

                if (mintState == 7)
                {
                    if (MouseEventArgs.MapWindow.Caption == FromWindow.Caption) return;

                    mintState = -1;
                    application.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Creating ToDuctnest. Please wait...");
                    ShowMessage("Creating ToDuctnest. Please wait...");
                    if (mobjEditServiceTo.GeometryCount > 0) mobjEditServiceTo.RemoveAllGeometries();
                    if (mobjEditServiceTemp.GeometryCount > 0) mobjEditServiceTemp.RemoveAllGeometries();
                    PlaceDuctnest(mptSelected.X, mptSelected.Y, _DuctNest, ConnectType.to, mptPoint2);
                    HideMessage();
                    if (!CheckDuctpath())
                    {
                        ExitCmd();
                        return;
                    }
                    if (FromWindow != null)
                    {
                        FromWindow.Close();
                        FromWindow = null;
                    }
                    if (ToWindow != null)
                    {
                        ToWindow.Caption = WindowCaption;
                        ToWindow.WindowState = GTWindowStateConstants.gtmwwsMaximize;
                        ToWindow.Activate();
                    }
                    LoadDuctnestConfig(iPathFID);
                  //  ExitCmd();
                  //  mintState = 8;
                    return;
                }
                //if (mintState==8)
                //{
                //    ExitCmd();
                //}
            }
            catch (Exception ex)
            {
                HideMessage();
                //Logger log = Logger.GetInstance();
                log.WriteLog("Error", "m_oCustomCommandHelper_MouseUp", ex.Message);
                log.WriteLog(ex.StackTrace);
                MessageBox.Show(ex.Message, "DuctNest Placement", MessageBoxButtons.OK, MessageBoxIcon.Error);
                if (m_oGTTransactionManager != null) m_oGTTransactionManager.Rollback();
                //log.CloseFile();
            }
        }

        private void m_oCustomCommandHelper_MouseMove(object sender, GTMouseEventArgs MouseEventArgs)
        {
            try
            {
               

                if ((mintState == 1))
                {
                    application.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Wait, two windows for placement are opening...");
                }
                else if (mintState == 2)
                {
                    IGTPoint mptPoint2 = MouseEventArgs.WorldPoint;
                    CreateDuctnestGeom(mptPoint2.X, mptPoint2.Y, _DuctNest, ConnectType.from, mobjEditServiceFrom);
                    application.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Define new FromDuctnest location. Click a point to accept, right-click to exit.");
                }
                else if (mintState == 3)
                {
                    IGTPoint mptPoint2 = MouseEventArgs.WorldPoint;
                    mobjEditServiceFrom.Rotate(mptPoint2);
                    IGTLineGeometry oLine = PGeoLib.CreateLineGeom(mptSelected.X, mptSelected.Y, mptPoint2.X, mptPoint2.Y);
                    if (mobjEditServiceTemp.GeometryCount > 0) mobjEditServiceTemp.RemoveAllGeometries();
                    mobjEditServiceTemp.TargetMapWindow = FromWindow;
                    mobjEditServiceTemp.AddGeometry(oLine, 11516);
                    application.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Click a point to accept or right-click to cancel rotate.");
                }
                else if ((mintState == 5))
                {
                    IGTPoint mptPoint2 = MouseEventArgs.WorldPoint;
                    CreateDuctnestGeom(mptPoint2.X, mptPoint2.Y, _DuctNest, ConnectType.to, mobjEditServiceTo);
                    application.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Define new ToDuctnest location. Click a point to accept.");
                }
                else if (mintState == 6)
                {
                    IGTPoint mptPoint2 = MouseEventArgs.WorldPoint;
                    mobjEditServiceTo.Rotate(mptPoint2);
                    IGTLineGeometry oLine = PGeoLib.CreateLineGeom(mptSelected.X, mptSelected.Y, mptPoint2.X, mptPoint2.Y);
                    if (mobjEditServiceTemp.GeometryCount > 0) mobjEditServiceTemp.RemoveAllGeometries();
                    mobjEditServiceTemp.TargetMapWindow = ToWindow;
                    mobjEditServiceTemp.AddGeometry(oLine, 11516);
                    application.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Click a point to accept or right-click to cancel rotate.");
                }
               // System.Windows.Forms.Application.DoEvents();
                //ExitCmd();
            }
            catch (Exception ex)
            {
                log.WriteLog("Error", "m_oCustomCommandHelper_MouseMove", ex.Message);
                log.WriteLog(ex.StackTrace);
                MessageBox.Show(ex.Message, "DuctNest Placement", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //log.CloseFile();
            }
        }

        private void m_oCustomCommandHelper_KeyUp(object sender, GTKeyEventArgs KeyEventArgs)
        {
            try
            {
                if (KeyEventArgs.KeyCode == 27)
                {
                    ExitCmd();
                    return;
                }
            }
            catch (Exception ex)
            {
                log.WriteLog("Error", "m_oCustomCommandHelper_KeyUp", ex.Message);
                log.WriteLog(ex.StackTrace);
                MessageBox.Show(ex.Message, "DuctNest Placement", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //log.CloseFile();
            }
        }

        public string ManholeName(bool from)
        {
            string name = "";
            if (from)
            {
                //name = FeatureName(fromFNO, fromFID);
                //if (name == "Manhole")
                //    name += " ID=" + Get_Value("select MANHOLE_ID from GC_MANHL where g3e_fid = " + fromFID.ToString());
                //else name += " FID=" + fromFID.ToString();

                name = "From " + from_sourcename;
            }
            else
            {
                //name = FeatureName(toFNO, toFID);

                //if (name == "Manhole")
                //    name += " ID=" + Get_Value("select MANHOLE_ID from GC_MANHL where g3e_fid = " + fromFID.ToString());
                //else name += " FID=" + fromFID.ToString();

                name= "To " + to_sourcename;
            }
            return name;
        }


        #region LengthBetween Two points on sumple line
        private int LengthBtwTwoPoints(double startPointX, double startPointY, int flag)
        {
            double endPointX = 0;
            double endPointY = 0;
            
            IGTKeyObject oDuctpath1 = application.DataContext.OpenFeature(iPathFNO, iPathFID);
            //string DT_MH_FRM_WALL = "";
            //string DT_MH_TO_WALL = "";
            //for (int i = 0; i < oDuctpath1.Components.GetComponent(2201).Recordset.Fields.Count; i++)
            //{
            //    if (oDuctpath1.Components.GetComponent(2201).Recordset.Fields[i].Name == "DT_MH_FRM_WALL")
            //        DT_MH_FRM_WALL = oDuctpath1.Components.GetComponent(2201).Recordset.Fields[i].Value.ToString();
            //    if (oDuctpath1.Components.GetComponent(2201).Recordset.Fields[i].Name == "DT_MH_TO_WALL")
            //        DT_MH_TO_WALL = oDuctpath1.Components.GetComponent(2201).Recordset.Fields[i].Value.ToString();
            //}

            string wallnum="";
            short iFNO=0;
            int lFID=0;
            if(flag==0)
            {
                iFNO=fromFNO;
                lFID=fromFID;
                wallnum=DT_MH_FRM_WALL;
            }
            else
            { 
                iFNO=toFNO;
                lFID=toFID;
                wallnum=DT_MH_TO_WALL;
            }
             IGTDDCKeyObjects feat = application.DataContext.GetDDCKeyObjects(iFNO, lFID, GTComponentGeometryConstants.gtddcgAllGeographic);
             if (wallnum == "0")
             {
                 for (int K = 0; K < feat.Count; K++)
                 {
                     if (feat[K].ComponentViewName == "VGC_PSGCON_S")
                     {

                         endPointX = feat[K].Geometry.FirstPoint.X;
                         endPointY = feat[K].Geometry.FirstPoint.Y;
                         break;
                     }
                 }
             }
             else
             {
                 for (int K = 0; K < feat.Count; K++)
                 {
                     if (feat[K].ComponentViewName == "VGC_MANHLW_T" ||
                         feat[K].ComponentViewName == "VGC_CHAMBERWALL_T" ||
                         feat[K].ComponentViewName == "VGC_TUNNELWALL_T") //AN_MOD
                     {
                         for (int i = 0; i < feat[K].Recordset.Fields.Count; i++)//AN_MOD
                         {
                             if ((feat[K].Recordset.Fields[i].Name == "G3E_CID") && (feat[K].Recordset.Fields[i].Value.ToString() == wallnum))//AN_MOD
                             {

                                 endPointX = feat[K].Geometry.FirstPoint.X;
                                 endPointY = feat[K].Geometry.FirstPoint.Y;
                                 break;
                             }

                         }

                     }
                 }
             }
            return Convert.ToInt32(Math.Round(Math.Sqrt(Math.Pow((endPointX - startPointX), 2) + Math.Pow((endPointY - startPointY), 2)), 0));
        }
        #endregion

        /// <summary>
        /// Register EventHandlers on CommandHelper
        /// </summary>
        /// <param name=""></param>
        #region private void RegisterEvents()
        private void RegisterEvents()
        {
            try
            {
                m_oGTCustomCommandHelper.KeyUp += new EventHandler<GTKeyEventArgs>(m_oCustomCommandHelper_KeyUp);
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
                log.WriteLog(ex.StackTrace);
                MessageBox.Show(ex.Message, "DuctNest Placement", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                m_oGTCustomCommandHelper.KeyUp -= m_oCustomCommandHelper_KeyUp;
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
                log.WriteLog(ex.StackTrace);
                MessageBox.Show(ex.Message, "DuctNest Placement", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //log.CloseFile();
            }
        }
        #endregion

    }
}
