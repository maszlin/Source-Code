using System;
using System.Collections.Generic;
using System.Text;

using System.Drawing;
using Intergraph.GTechnology.Interfaces;
using Intergraph.GTechnology.API;
using AG.GTechnology.Utilities;
using ADODB;

namespace NEPS.OSP.COPPER.PLOT.MULTI
{
    class clsPlotArea
    {
        #region Declaration
        public static IGTGeometryEditService objPlotArea;

        public bool isTopLeft = false;
        public bool isBottomRight = false;

        public IGTPoint topleft_point;
        public IGTPoint bottomright_point;
        #endregion

        public clsPlotArea()
        {
            objPlotArea = GTClassFactory.Create<IGTGeometryEditService>();
            objPlotArea.TargetMapWindow = GTClassFactory.Create<IGTApplication>().ActiveMapWindow;

            bottomright_point = GTClassFactory.Create<IGTPoint>();
            topleft_point = GTClassFactory.Create<IGTPoint>();
        }

        public bool DrawPlotArea(IGTPoint ePoint)
        {
            if (isTopLeft)
            {
                topleft_point = ePoint;
            }
            else if (isBottomRight)
            {
                bottomright_point = ePoint;
            }
            else
                return false;

            if (topleft_point.Y == 0 && topleft_point.X == 0)
                return true;
            if (bottomright_point.Y == 0 && bottomright_point.X == 0)
                return true;

//            VerifiedPoints();

            IGTPoint topright_point = GTClassFactory.Create<IGTPoint>();
            topright_point.X = bottomright_point.X;
            topright_point.Y = topleft_point.Y;

            IGTPoint bottomleft_point = GTClassFactory.Create<IGTPoint>();
            bottomleft_point.X = topleft_point.X;
            bottomleft_point.Y = bottomright_point.Y;

            GTClassFactory.Create<IGTApplication>().SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Click to set point. Press ESC or right click to cancel.");
            List<IGTPoint> arrTempPoint = new List<IGTPoint>();
            arrTempPoint.Add(topleft_point);
            arrTempPoint.Add(topright_point);
            arrTempPoint.Add(bottomright_point);
            arrTempPoint.Add(bottomleft_point);

            // create polygon on temporary edit.service as mouse move
            if (objPlotArea.GeometryCount > 0) objPlotArea.RemoveAllGeometries();
            IGTPolygonGeometry oPolygon = PGeoLib.CreatePolyGeom(arrTempPoint);
            objPlotArea.AddGeometry(oPolygon, 351001);

            return true;
        }

        private void VerifiedPoints()
        {
            if (bottomright_point.X < topleft_point.X)
            {
                double x = bottomright_point.X;
                bottomright_point.X = topleft_point.X;
                topleft_point.X = x;
            }
            if (bottomright_point.Y > topleft_point.Y)
            {
                double y = bottomright_point.Y;
                bottomright_point.Y = topleft_point.Y;
                topleft_point.Y = y;
            }
        }

        public void ApplyPlotPoint()
        {
            if (isTopLeft)
            {
                isTopLeft = false;
                isBottomRight = true;
            }
            else if (isBottomRight)
            {
                VerifiedPoints();
                isBottomRight = false;
            }
            else
                return;
        }

        public void ResetPlotPoint()
        {
            isTopLeft = true;
            isBottomRight = false;
            bottomright_point = GTClassFactory.Create<IGTPoint>();
            topleft_point = GTClassFactory.Create<IGTPoint>();
            if (objPlotArea.GeometryCount > 0) objPlotArea.RemoveAllGeometries();
        }

        public void ClearPlotArea()
        {
            if (objPlotArea.GeometryCount > 0) objPlotArea.RemoveAllGeometries();
        }

    }
}

