using Intergraph.GTechnology.API;
using System.Collections.Generic;
using System.Text;
using System;

/////////////////////////////
// La Viet Phuong - Jan 2012
////////////////////////////
namespace NEPS.GTechnology.Cable_Joint
{
    public class PGeoLib
    {
        private static double Pi = 3.141592653589793;
        private static Intergraph.GTechnology.API.IGTApplication application = GTClassFactory.Create<IGTApplication>();

        #region Math - calculation functions
        public static double Rad2Deg(double rad)
        {
            return rad * (180 / Math.PI);
        }

        public static double Deg2Rad(double deg)
        {
            return (deg * Math.PI) /180;
        }

        public static double GetAngle(double X1, double Y1, double X2, double Y2)
        {
            double XDiff;
            double YDiff;
            double TempAngle;

            YDiff = Math.Abs(Y2 - Y1);

            if ((X1 == X2) && (Y1 == Y2)) return 0;

            if ((YDiff == 0) && (X1 < X2))
                return 0;
            else if ((YDiff == 0) && (X1 > X2))
                return Pi;

            XDiff = Math.Abs(X2 - X1);

            TempAngle = Math.Atan(XDiff / YDiff);

            if (Y2 > Y1) TempAngle = Pi - TempAngle;
            if (X2 < X1) TempAngle = -TempAngle;
            TempAngle = 1.5707963267949 - TempAngle;
            if (TempAngle < 0) TempAngle = 6.28318530717959 + TempAngle;

            return TempAngle;
        }

        public static double GetLength(double X1, double Y1, double X2, double Y2)
        {
            double lenght;

            lenght = Math.Sqrt(Math.Pow(X1 - X2, 2) + Math.Pow(Y1 - Y2, 2));

            return lenght;
        }

        public static double GetLength(IGTPolylineGeometry pline)
        {
            double lenght = 0;

            for (int i = 0; i < pline.Points.Count - 1; i++)
            {
                lenght = lenght + GetLength(pline.Points[i].X, pline.Points[i].Y, pline.Points[i + 1].X, pline.Points[i + 1].Y);
            }

            return lenght;
        }

        public static IGTPoint GetPoint(IGTPolylineGeometry pline, double dist)
        {
            double lenght = 0;
            IGTPoint pnt = GTClassFactory.Create<IGTPoint>();

            for (int i = 0; i < pline.Points.Count - 1; i++)
            {
                double dLine = GetLength(pline.Points[i].X, pline.Points[i].Y, pline.Points[i + 1].X, pline.Points[i + 1].Y);
                if ((lenght + dLine) == dist) return pline.Points[i + 1];
                if ((lenght + dLine) > dist)
                {
                    pnt.X = pline.Points[i].X + ((dist - lenght) / dLine) * (pline.Points[i + 1].X - pline.Points[i].X);
                    pnt.Y = pline.Points[i].Y + ((dist - lenght) / dLine) * (pline.Points[i + 1].Y - pline.Points[i].Y);
                    break;
                }

                lenght = lenght + dLine;
            }

            return pnt;
        }

        public static IGTPoint GetProjectedPoint(IGTPolylineGeometry pline, IGTPoint pnt)
        {
            List<IGTPoint> _list = new List<IGTPoint>();
            double minLen = 0;
            IGTPoint prjPoint = null;
            for (int i = 0; i < pline.Points.Count - 1; i++)
            {
                IGTSegment segment = GTClassFactory.Create<IGTSegment>();
                segment.Point1 = pline.Points[i];
                segment.Point2 = pline.Points[i + 1];

                IGTSegmentPoint pnt2 = segment.ProjectPoint(pnt);

                _list.Add(pnt2.Point);
                minLen = GetLength(pnt2.Point.X, pnt2.Point.Y, pnt.X, pnt.Y);
                prjPoint = pnt2.Point;
            }

            foreach (IGTPoint _pnt in _list)
            {
                if (minLen > GetLength(_pnt.X, _pnt.Y, pnt.X, pnt.Y))
                {
                    minLen = GetLength(_pnt.X, _pnt.Y, pnt.X, pnt.Y);
                    prjPoint = _pnt;
                }
            }
            return prjPoint;
        }
        #endregion

        #region Create geometry functions
        public static IGTLineGeometry CreateLineGeom(double X1, double Y1, double X2, double Y2)
        {
            IGTPoint objPoint1;
            IGTPoint objPoint2;
            IGTLineGeometry oOrLineGeom = null;

            objPoint1 = GTClassFactory.Create<IGTPoint>();
            objPoint1.X = X1;
            objPoint1.Y = Y1;
            objPoint2 = GTClassFactory.Create<IGTPoint>();
            objPoint2.X = X2;
            objPoint2.Y = Y2;
            oOrLineGeom = GTClassFactory.Create<IGTLineGeometry>();
            oOrLineGeom.Start = objPoint1;
            oOrLineGeom.End = objPoint2;

            return oOrLineGeom;
        }

        public static IGTOrientedPointGeometry CreatePointGeom(double X, double Y)
        {
            IGTPoint objPoint;
            IGTVector objVector;
            IGTOrientedPointGeometry oOrPointGeom = null;
            double dRotation = 0;

            objPoint = GTClassFactory.Create<IGTPoint>();
            objPoint.X = X;
            objPoint.Y = Y;
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

            return oOrPointGeom;
        }

        public static IGTTextPointGeometry CreateTextGeom(double X, double Y, string text)
        {
            IGTTextPointGeometry oOrPointGeom = null;

            oOrPointGeom = CreateTextGeom(X, Y, text, 0, GTAlignmentConstants.gtalCenterCenter);

            return oOrPointGeom;
        }

        public static IGTTextPointGeometry CreateTextGeom(double X, double Y, string text, double angle, GTAlignmentConstants alignment)
        {
            IGTPoint objPoint;
            IGTTextPointGeometry oOrPointGeom = null;
            double dRotation = 0;

            dRotation = angle;

            objPoint = GTClassFactory.Create<IGTPoint>();
            objPoint.X = X;
            objPoint.Y = Y;
            oOrPointGeom = GTClassFactory.Create<IGTTextPointGeometry>();
            //  Offset
            oOrPointGeom.Origin = objPoint;
            //  Degrees
            oOrPointGeom.Text = text;
            oOrPointGeom.Rotation = dRotation;
            oOrPointGeom.Alignment = alignment;

            return oOrPointGeom;
        }
        #endregion


        public static void DeleteComponents(IGTKeyObject fromFeature)
        {
            for (int i = 0; i < fromFeature.Components.Count; i++)
            {
                short iCNO = fromFeature.Components[i].CNO;
                if (!fromFeature.Components[i].Recordset.EOF)
                {
                    while (!fromFeature.Components[i].Recordset.EOF)
                    {
                        fromFeature.Components[i].Recordset.Delete(ADODB.AffectEnum.adAffectAll);
                    }
                }
            }
        }

        public static void LocateFeature(short iFNO, int lFID, IGTMapWindow window)
        {
            if (window == null) return;

            IGTDDCKeyObjects feat = application.DataContext.GetDDCKeyObjects(iFNO, lFID, GTComponentGeometryConstants.gtddcgAllGeographic);

            application.SelectedObjects.Clear();            
            for (int K = 0; K < feat.Count; K++)
            {
                application.SelectedObjects.Add(GTSelectModeConstants.gtsosmSelectedComponentsOnly, feat[K]);
            }

            window.CenterSelectedObjects();
            window.DisplayScale = 100;
            application.RefreshWindows();
        }

    }
}
