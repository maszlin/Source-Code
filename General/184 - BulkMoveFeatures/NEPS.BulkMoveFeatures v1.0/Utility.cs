using System;
using System.Collections.Generic;
using System.Text;
using Intergraph.GTechnology.API;
using ADODB;
using System.Drawing;

namespace NEPS.GTechnology.BulkMoveFeatures
{
    public class Utility
    {
        public const int FNO_Cable = 7000;
        public const int FNO_FiberESideCable = 7400;
        public const int FNO_FiberDSideCable = 7200;
        public const int FNO_DP = 13000;
        public const int FNO_FiberDP = 5600;
        public const int FNO_Joint = 10800;
        public const short FNO_DuctNest = 2400;
        public const short FNO_Duct =2300;
        public const short FNO_Pole = 3000;
        public const short FNO_NonDuctPath = 3500;
        public const short FNO_StrutStayAttach = 2000;
        public const short FNO_Cabinet = 10300;
        public const short FNO_FiberSplice = 11800;
        public const short FNO_Ductpath = 2200;
        public const short FNO_Manhole = 2700;
        public const short FNO_FiberDistributionCabinet = 5100;
        public const short FNO_PatchCable = 13400;

        public const short RNO_Connects = 1;
        public const short RNO_Owns = 2;
        public const short RNO_Contains = 7;

        public const short CNO_DuctPathGeoLabel = 2230;
        public const short CNO_DPLabel = 13030;
        public const short CNO_FiberDPLabel = 5630;
        public const short CNO_CableGeoline = 7010;
        public const short CNO_CableGeoLabel = 7030;
        public const short CNO_FiberDSideGeoline = 7210;
        public const short CNO_FiberDSideGeoLabel = 7230;
        public const short CNO_FiberESideGeoline = 7410;
        public const short CNO_FiberESideGeoLabel = 7430;
        public const short CNO_CableAnnotationGeoLabel = 7034;
        public const short CNO_NE_Connect = 54;
        private const int CNO_NETELEM = 51;
        public const int StyleIDFromArea = 14008; //refer to G3E_STYLE if you want to change this to whatever.
        public const int StyleIDToArea = 12511;

        // FNOs of features that should be excluded from movement. These are typically property admin features, etc.
        public static short[] StaticFNOs = { 8000, 24000, 30000, 30100, 30200, 30300, 30400, 30500, 30600, 30700, 30800, 30900, 
            31000, 31100, 31200, 31300, 31400, 31500 };
        //public static short[] StaticFNOs ={ 0 };

        public class FeatureItem
        {
            public int FID;
            public short FNO;
        }

        public class FeatureType
        {
            public short FNO;
            public string Username;
        }

        public class MoveableFeature
        {
            public short FNO;
            public List<short> AssociatedFNOs = new List<short>();
            public void Associate(short FNO)
            {
                AssociatedFNOs.Add(FNO);
            }
            public MoveableFeature(short _FNO)
            {
                FNO = _FNO;
            }
        }

       
        /// <summary>
        /// Kamal's Instructions
        //1. Manhole: User selects a manhole, and clicks on the new location for the manhole. The vertex of duct path connected to the manhole will move along, together with any labels and duct nest associated to the manhole and the duct path.
        //2. Pole: User selects a pole, and clicks on the new location for the pole. The vertex of the non-duct path connected to the pole will move along, together with any strut, stay, labels and duct nest associated to the pole and the non-duct path.
        //3. Copper DP: User selects a DP, and clicks on the new location for the DP. The vertex of the cable connected to the DP will move along, together with any labels associated to the DP and the connecting cable.
        //4. Cabinet: User selects a cabinet, and clicks on the new location for the cabinet. All the vertices of the various copper cables which are connected to the cabinet will move along, together with any labels associated to the cabinet and cables.
        //5. Copper Joint: User selects a copper joint, and clicks on the new location for the copper joint. The vertices of the copper cables connected to the copper joint with move along, together with any labels associated to the joint and copper cables.
        //6. Fiber DP: Same as Copper DP
        //7. Fiber Joint: Same as Copper Joint
        /// </summary>
        /// <returns></returns>
        public static Dictionary<short, MoveableFeature> GetMoveableFeatures()
        {
            Dictionary<short, MoveableFeature> output = new Dictionary<short, MoveableFeature>();
            MoveableFeature obj = null;

            // Manholes
            obj = new MoveableFeature(FNO_Manhole);
            obj.Associate(FNO_DuctNest);
            obj.Associate(FNO_Ductpath);       
            obj.Associate(FNO_Duct);
            output.Add(obj.FNO, obj);

            // Poles
            obj = new MoveableFeature(FNO_Pole);
            obj.Associate(FNO_NonDuctPath);
            obj.Associate(FNO_StrutStayAttach);
            obj.Associate(FNO_DuctNest);

            // additionally, move joint close to the pole
            obj.Associate(FNO_Joint);

            output.Add(obj.FNO, obj);

            // Copper DP
            obj = new MoveableFeature(FNO_DP);
            obj.Associate(FNO_Cable);
            output.Add(obj.FNO, obj);

            // Cabinet
            obj = new MoveableFeature(FNO_Cabinet);
            obj.Associate(FNO_Cable);
            obj.Associate(FNO_PatchCable);
            output.Add(obj.FNO, obj);

            // FDC
            obj = new MoveableFeature(FNO_FiberDistributionCabinet);
            obj.Associate(FNO_FiberDSideCable);
            obj.Associate(FNO_FiberESideCable);
            output.Add(obj.FNO, obj);

            // Copper Joint
            obj = new MoveableFeature(FNO_Joint);
            obj.Associate(FNO_Cable);
            output.Add(obj.FNO, obj);

            // Fiber DP
            obj = new MoveableFeature(FNO_FiberDP);
            obj.Associate(FNO_FiberDSideCable);
            obj.Associate(FNO_FiberESideCable);
            output.Add(obj.FNO, obj);

            // Fiber Joint
            obj = new MoveableFeature(FNO_FiberSplice);
            obj.Associate(FNO_FiberDSideCable);
            obj.Associate(FNO_FiberESideCable);
            output.Add(obj.FNO, obj);

            return output;
        }

        public static List<FeatureItem> GetFeaturesContained(IGTPolygonGeometry area, IGTApplication app)
        {
            List<FeatureItem> output = new List<FeatureItem>();

            // initialize spatial service
            IGTSpatialService oSS = GTClassFactory.Create<IGTSpatialService>();
            IGTKeyObjects oKO = GTClassFactory.Create<IGTKeyObjects>();
            IGTKeyObjects oFG = GTClassFactory.Create<IGTKeyObjects>();
            ADODB.Recordset rsAOI = new ADODB.Recordset();
            oSS.DataContext = app.DataContext;
            oSS.FilterGeometry = area;
            oSS.Operator = GTSpatialOperatorConstants.gtsoTouches;

            // get all the feature FNOs and add them to the list
            List<short> lstFNO = new List<short>();
            List<FeatureType> featureTypes = GetFeatureTypes();
            foreach (FeatureType featureType in featureTypes)
            {
                lstFNO.Add(featureType.FNO);
            }

            // use the list of FNOs to get features which are inside the boundary
            rsAOI = oSS.GetResultsByFNO(lstFNO.ToArray());
            if (!rsAOI.EOF)
            {
                rsAOI.MoveFirst();
                while (!rsAOI.EOF)
                {
                    FeatureItem item = new FeatureItem();
                    item.FNO = Convert.ToInt16(rsAOI.Fields["G3E_FNO"].Value);
                    item.FID = Convert.ToInt32(rsAOI.Fields["G3E_FID"].Value);

                    // should the FNO be excluded?
                    if (IsStaticFNO(item.FNO))
                    {
                        rsAOI.MoveNext();
                        continue;
                    }


                    output.Add(item);
                    rsAOI.MoveNext();
                }
            }

            return output;
        }

        private static bool IsStaticFNO(short FNO)
        {
            for (int i = 0; i < StaticFNOs.Length; i++)
            {
                if (FNO == StaticFNOs[i])
                    return true;
            }
            return false;
        }

        private static List<FeatureType> GetFeatureTypes()
        {
            List<FeatureType> output = new List<FeatureType>();
            IGTApplication app = GTClassFactory.Create<IGTApplication>();
            string sql = "SELECT * FROM G3E_FEATURE ORDER BY G3E_USERNAME";

            int count = 0;
            Recordset rs = app.DataContext.Execute(sql, out count, 0, null);
            if (!rs.EOF)
            {
                rs.MoveFirst();


                while (!rs.EOF)
                {
                    FeatureType item = new FeatureType();
                    item.FNO = Convert.ToInt16(rs.Fields["G3E_FNO"].Value);
                    item.Username = rs.Fields["G3E_USERNAME"].Value.ToString();
                    rs.MoveNext();
                    output.Add(item);
                }
            }
            return output;
        }

        public static bool ShouldMove(IGTPoint testPoint, IGTPoints testBoundary)
        {
            bool isInside = IsPointInPolygon(testPoint, testBoundary);
            return isInside;
        }

        /// <summary>
        /// Got from here http://dominoc925.blogspot.com/2012/02/c-code-snippet-to-determine-if-point-is.html
        /// </summary>
        /// <param name="point"></param>
        /// <param name="polygon"></param>
        /// <returns></returns>
        public static bool IsPointInPolygon(IGTPoint point, IGTPoints polygon)
        {
            // if no polygon is specified, assume it is inside
            if (polygon == null)
                return true;

            bool isInside = false;
           
            for (int i = 0, j = polygon.Count - 1; i < polygon.Count; j = i++)
            {
                if (((polygon[i].Y > point.Y) != (polygon[j].Y > point.Y)) &&
                (point.X < (polygon[j].X - polygon[i].X) * (point.Y - polygon[i].Y) / (polygon[j].Y - polygon[i].Y) + polygon[i].X))
                {
                    isInside = !isInside;
                }
            }
            return isInside;
        }

        public static void AddPoint(IGTPolygonGeometry output, double x, double y)
        {
            IGTPoint topLeft = GTClassFactory.Create<IGTPoint>();
            topLeft.X = x;
            topLeft.Y = y;
            output.Points.Add(topLeft);
        }

        internal static IGTPolygonGeometry CreateBoundingRect(IGTPolygonGeometry source, out Rectangle outputRect)
        {
            // define the corners of the rect
            double left = double.MaxValue, top = double.MinValue, right = double.MinValue, bottom = double.MaxValue;
            foreach (IGTPoint pt in source.Points)
            {
                if (pt.X < left)
                    left = pt.X;
                if (pt.X > right)
                    right = pt.X;
                if (pt.Y > top)
                    top = pt.Y;
                if (pt.Y < bottom)
                    bottom = pt.Y;
            }

            // create a rect and return it
            IGTPolygonGeometry output = GTClassFactory.Create<IGTPolygonGeometry>();
            output.Points.Add(GTClassFactory.Create<IGTPoint>(left, top, 0));
            output.Points.Add(GTClassFactory.Create<IGTPoint>(right, top, 0));
            output.Points.Add(GTClassFactory.Create<IGTPoint>(right, bottom, 0));
            output.Points.Add(GTClassFactory.Create<IGTPoint>(left, bottom, 0));

            outputRect = new Rectangle(left, top, right, bottom);

            return output;

        }

        public static IGTVector CalculateDelta(IGTPoint from, IGTPoint to)
        {
            IGTVector delta = GTClassFactory.Create<IGTVector>();
            delta.I = to.X - from.X;
            delta.J = to.Y - from.Y;
            return delta;
        }

        public static IGTVector CalculateDelta(IGTOrientedPointGeometry from, IGTPoint to)
        {
            IGTVector delta = GTClassFactory.Create<IGTVector>();
            delta.I = to.X - from.FirstPoint.X;
            delta.J = to.Y - from.FirstPoint.Y;
            return delta;
        }

        internal static IGTPoint ToIGTPoint(Point source)
        {
            IGTPoint output = GTClassFactory.Create<IGTPoint>();
            output.X = source.X;
            output.Y = source.Y;
            output.Z = 0;
            return output;
        }

        public static IGTKeyObject FindByFID(List<IGTKeyObject> source, int FID)
        {
            foreach (IGTKeyObject obj in source)
            {
                if (obj.FID == FID)
                    return obj;
            }
            return null;
        }

        public static FeatureItem FindByFID(List<FeatureItem> source, int FID)
        {
            foreach (FeatureItem obj in source)
            {
                if (obj.FID == FID)
                    return obj;
            }
            return null;
        }

        public static IGTKeyObject FindByFID(IGTKeyObjects source, int FID)
        {
            foreach (IGTKeyObject obj in source)
            {
                if (obj.FID == FID)
                    return obj;
            }
            return null;
        }

        internal static IGTVector GetLineAngle(IGTPolylineGeometry line, bool useLastPointPair)
        {
            IGTPoint pt1 = line.Points[0];
            IGTPoint pt2 = line.Points[1];
            if (useLastPointPair)
            {
                pt1 = line.Points[line.Points.Count - 2];
                pt2 = line.Points[line.Points.Count - 1];
            }

            bool swap = pt1.X > pt2.X;
            if (swap)
            {
                IGTPoint temp = pt1;
                pt1 = pt2;
                pt2 = temp;
            }

            IGTVector output = CalculateDelta(pt1, pt2);
            return output;
        }

        internal static IGTPoint GetMiddlePosition(IGTPolylineGeometry line, float offsetPercentage, bool useLastPointPair)
        {
            IGTPoint pt1 = line.Points[0];
            IGTPoint pt2 = line.Points[1];

            if (useLastPointPair)
            {
                pt1 = line.Points[line.Points.Count - 2];
                pt2 = line.Points[line.Points.Count - 1];
            }


            bool swap = pt1.X > pt2.X;
            if (swap)
            {
                IGTPoint temp = pt1;
                pt1 = pt2;
                pt2 = temp;
            }

            IGTPoint output = GTClassFactory.Create<IGTPoint>();
            output.X = pt1.X + (pt2.X - pt1.X) * offsetPercentage;
            output.Y = pt1.Y + (pt2.Y - pt1.Y) * offsetPercentage;
            return output;
        }

        public static double degreeBetweenPoints(IGTPoint pt1, IGTPoint pt2)
        {
            double xDiff = pt2.X - pt1.X;
            double yDiff = pt2.Y - pt1.Y;
            return Math.Atan2(yDiff, xDiff) * 180.0 / Math.PI;
        }

        public static IGTPoint GetPointOnCircle(double theta, double radius, IGTPoint center)
        {
            double x = center.X + radius * Math.Cos(theta * Math.PI / 180);
            double y = center.Y + radius * Math.Sin(theta * Math.PI / 180);
            IGTPoint output = GTClassFactory.Create<IGTPoint>();
            output.X = x;
            output.Y = y;
            return output;
        }

    }


}
