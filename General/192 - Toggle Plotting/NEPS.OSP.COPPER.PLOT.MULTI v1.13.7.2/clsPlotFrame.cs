using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

using System.Drawing;
using Intergraph.GTechnology.Interfaces;
using Intergraph.GTechnology.API;
using AG.GTechnology.Utilities;
using ADODB;

namespace NEPS.OSP.COPPER.PLOT.MULTI
{
    class clsPlotFrame
    {
        public List<PlotBoundary> Frames = new List<PlotBoundary>();
        public PlotBoundary MainFrame = new PlotBoundary();

        private List<double> YAxis = new List<double>();
        private List<double> XAxis = new List<double>();

        public void CreatePlotFrames(IGTPoint topleft, IGTPoint bottomright)
        {

            double frame_angle = 0;
            double angle = (Math.PI * (frame_angle / 180));
            double angle90 = (Math.PI * (90.0 / 180.0));
            double angle180 = (Math.PI * (180.0 / 180.0));
            double angle270 = (Math.PI * (270.0 / 180.0));
            double height = clsPlotProperties.MapHeightScaled;
            double width = clsPlotProperties.MapWidthScaled;

            char yLabel = 'A';

            Frames.Clear();
            XAxis.Clear();
            YAxis.Clear();

            IGTPoint frame_point = GTClassFactory.Create<IGTPoint>();

            frame_point.Y = topleft.Y; // start Y from top
            YAxis.Add(topleft.Y);
            XAxis.Add(topleft.X);

            int row = 0;
            int col = 0;

            while (frame_point.Y > bottomright.Y) // to bottom
            {
                double bottom = frame_point.Y + (Math.Sin(angle270) * height);
                col = 1;
                row++;

                YAxis.Add(bottom);
                frame_point.X = topleft.X; // start X from left 

                while (frame_point.X < bottomright.X) // to right
                {
                    PlotBoundary oPlot = CreatePlotBoundary(frame_point, frame_angle);

                    oPlot.BottomLeft = GTClassFactory.Create<IGTPoint>();
                    oPlot.BottomLeft.X = frame_point.X + (Math.Cos(angle270) * height);
                    oPlot.BottomLeft.Y = bottom;

                    oPlot.TopRight = GTClassFactory.Create<IGTPoint>();
                    oPlot.TopRight.X = (frame_point.X + (Math.Cos(angle) * width));
                    oPlot.TopRight.Y = (frame_point.Y + (Math.Sin(angle) * width));

                    oPlot.Geometry = CreatePolygonGeometry(frame_point, frame_angle);

                    Frames.Add(oPlot);
                    if (yLabel == 'A') XAxis.Add(oPlot.TopRight.X);

                    // get next point of X == frame.right
                    frame_point.X = oPlot.TopRight.X;
                    oPlot.labelName = Frames.Count.ToString(); //yLabel + col.ToString();
                    col++;
                }
                yLabel = NextChar(yLabel);

                // get next point of Y == frame.bottom
                frame_point.Y = bottom;
            }

            #region Main Frame - area overview
            int maxframe = Frames.Count - 1;
            MainFrame = CreatePlotBoundary(topleft, frame_angle);
            MainFrame.BottomLeft = GTClassFactory.Create<IGTPoint>();
            MainFrame.BottomLeft.X = Frames[0].BottomLeft.X;
            MainFrame.BottomLeft.Y = Frames[maxframe].BottomLeft.Y;

            MainFrame.TopRight = GTClassFactory.Create<IGTPoint>();
            MainFrame.TopRight.X = Frames[maxframe].TopRight.X;
            MainFrame.TopRight.Y = Frames[0].TopRight.Y;

            MainFrame.labelName = "Area Overview";
            col--;
            MainFrame.ScaleFactor = (col > row ? col : row);
            #endregion

        }

        private PlotBoundary CreatePlotBoundary(IGTPoint pRotateAnchor, double dAngleDeg)
        {
            PlotBoundary bnd = new PlotBoundary();
            bnd.Type = GTMultiPlot.m_gtapp.Properties["PlotBoundary.Type"].ToString();
            bnd.PaperSize = GTMultiPlot.m_gtapp.Properties["PlotBoundary.PaperSize"].ToString();
            bnd.PaperOrientation = GTMultiPlot.m_gtapp.Properties["PlotBoundary.SheetOrientation"].ToString();
            bnd.MapScale = GTMultiPlot.m_gtapp.Properties["PlotBoundary.MapScale"].ToString();

            return bnd;
        }

        private IGTPolygonGeometry CreatePolygonGeometry(IGTPoint UserPoint, double dAngle)
        {
            double angle = (Math.PI * (dAngle / 180));
            double angle180 = (Math.PI * ((dAngle + 180.0) / 180.0));
            double angle270 = (Math.PI * ((dAngle + 270.0) / 180.0));
            double w = clsPlotProperties.MapWidthScaled;
            double h = clsPlotProperties.MapHeightScaled;

            IGTPolygonGeometry oPolygonGeometry;
            IGTPoint oPoint0;
            IGTPoint oPoint1;
            IGTPoint oPoint2;
            IGTPoint oPoint3;
            //  Geometry construction process.
            oPolygonGeometry = GTClassFactory.Create<IGTPolygonGeometry>();
            oPoint0 = GTClassFactory.Create<IGTPoint>(); // bottom left
            oPoint0.X = UserPoint.X;
            oPoint0.Y = UserPoint.Y;
            oPolygonGeometry.Points.Add(oPoint0);

            oPoint1 = GTClassFactory.Create<IGTPoint>();
            oPoint1.X = oPoint0.X + Math.Cos(angle) * w;
            oPoint1.Y = oPoint0.Y + Math.Sin(angle) * h;
            oPolygonGeometry.Points.Add(oPoint1);

            oPoint2 = GTClassFactory.Create<IGTPoint>();
            oPoint2.X = oPoint1.X + Math.Cos(angle270) * h;
            oPoint2.Y = oPoint1.Y + Math.Sin(angle270) * h;
            oPolygonGeometry.Points.Add(oPoint2);

            oPoint3 = GTClassFactory.Create<IGTPoint>();
            oPoint3.X = oPoint2.X + Math.Cos(angle180) * w;
            oPoint3.Y = oPoint2.Y + Math.Sin(angle180) * w;
            oPolygonGeometry.Points.Add(oPoint3);
            oPolygonGeometry.Points.Add(oPoint0);
            return oPolygonGeometry;
        }

        public void DrawFrame(IGTGeometryEditService plotArea)
        {
            if (plotArea.GeometryCount > 0) plotArea.RemoveAllGeometries();
            double centerX = clsPlotProperties.MapWidthScaled / 2;
            double centerY = clsPlotProperties.MapHeightScaled / 2;

            #region Draw fram border

            IGTPolygonGeometry frameBorder = GTClassFactory.Create<IGTPolygonGeometry>();
            IGTPoint oPoint0;
            IGTPoint oPoint1;
            IGTPoint oPoint2;
            IGTPoint oPoint3;

            int maxY = YAxis.Count - 1;
            int maxX = XAxis.Count - 1;

            #region Draw Y Border
            try
            {
                for (int y = 0; y < YAxis.Count; y += 2)
                {
                    oPoint0 = GTClassFactory.Create<IGTPoint>();
                    oPoint0.X = XAxis[0];
                    oPoint0.Y = YAxis[y];
                    frameBorder.Points.Add(oPoint0);

                    oPoint1 = GTClassFactory.Create<IGTPoint>();
                    oPoint1.X = XAxis[maxX];
                    oPoint1.Y = YAxis[y];
                    frameBorder.Points.Add(oPoint1);

                    oPoint2 = GTClassFactory.Create<IGTPoint>();
                    oPoint2.X = XAxis[maxX];
                    oPoint2.Y = YAxis[y + 1];
                    frameBorder.Points.Add(oPoint2);

                    oPoint3 = GTClassFactory.Create<IGTPoint>();
                    oPoint3.X = XAxis[0];
                    oPoint3.Y = YAxis[y + 1];
                    frameBorder.Points.Add(oPoint3);
                }
            }
            catch (Exception ex) { }
            #endregion

            #region Draw X Border
            try
            {
                for (int x = maxX; x > -1; x -= 2)
                {
                    oPoint0 = GTClassFactory.Create<IGTPoint>();
                    oPoint0.X = XAxis[x];
                    oPoint0.Y = YAxis[maxY];
                    frameBorder.Points.Add(oPoint0);

                    oPoint1 = GTClassFactory.Create<IGTPoint>();
                    oPoint1.X = XAxis[x];
                    oPoint1.Y = YAxis[0];
                    frameBorder.Points.Add(oPoint1);

                    oPoint2 = GTClassFactory.Create<IGTPoint>();
                    oPoint2.X = XAxis[x - 1];
                    oPoint2.Y = YAxis[0];
                    frameBorder.Points.Add(oPoint2);

                    oPoint3 = GTClassFactory.Create<IGTPoint>();
                    oPoint3.X = XAxis[x - 1];
                    oPoint3.Y = YAxis[maxY];
                    frameBorder.Points.Add(oPoint3);
                }
            }
            catch (Exception ex)
            {
            }
            #endregion

            plotArea.AddGeometry(frameBorder, 41500);

            #endregion

            #region Draw frame label
            int framelabel = 0;
            for (int i = 0; i < Frames.Count; i++)
            {
                //if (Frames[i].isHidden) Frames[i].isHidden = false;

                IGTTextPointGeometry oText = GTClassFactory.Create<IGTTextPointGeometry>();
                oText.Text = (++framelabel).ToString(); //Frames[i].labelName; 

                IGTPoint objPoint = GTClassFactory.Create<IGTPoint>();
                objPoint.X = Frames[i].BottomLeft.X + centerX;
                objPoint.Y = Frames[i].BottomLeft.Y + centerY;

                oText.Origin = objPoint;
                oText.Alignment = GTAlignmentConstants.gtalCenterCenter;

                if (Frames[i].isHidden)
                    plotArea.AddGeometry(oText, 80300706);
                else
                    plotArea.AddGeometry(oText, 80300702); // + (i % 5)); //80300700

            }
            #endregion

        }

        public void RemoveFrameLabel(IGTGeometryEditService plotArea, int framenumber, int frameindex)
        {
            try
            {
                Frames[frameindex].isHidden = true;
                System.Diagnostics.Debug.WriteLine("Hidden : " + Frames[frameindex].labelName);

                plotArea.RemoveGeometry(framenumber);
            }
            catch (Exception ex)
            {
            }
        }

        public void UpdateFrameIndex(IGTGeometryEditService plotArea)
        {
            try
            {
                int j = 0;
                for (int i = 0; i < Frames.Count; i++)
                {
                    if (Frames[i].isHidden) continue; // hidden frame already remove earlier - go next frame

                    IGTTextPointGeometry oText = GTClassFactory.Create<IGTTextPointGeometry>();
                    oText = (IGTTextPointGeometry)plotArea.GetGeometry(2);
                    oText.Text = (++j).ToString();

                    Frames[i].labelName = oText.Text;
                    plotArea.AddGeometry(oText, 80300702); //700

                    plotArea.RemoveGeometry(2);
                }
            }
            catch (Exception ex)
            {
            }
        }

        private char NextChar(char letter)
        {
            if (letter == 'Z')
                return 'A';
            else
                return (char)(((int)letter) + 1);
        }

        #region edit 22-07-2013 plotting redline

        public void DrawRedLineFrame(IGTNamedPlot plot, IGTPlotRedline oGTPlotRedline)
        {
            int iGroupNumber = oGTPlotRedline.GroupNumber;

            IGTPoint bottomright = plot.Frames[0].Range.BottomRight;
            IGTPoint topleft = plot.Frames[0].Range.TopLeft;

            bottomright.X -= 100;
            bottomright.Y -= 100;
            topleft.X += 100;
            topleft.Y += 100;

            double width = (bottomright.X - topleft.X) / (XAxis.Count - 1);
            double height = (bottomright.Y - topleft.Y) / (YAxis.Count - 1);
            double centerX = width / 2;
            double centerY = height / 2;

            #region Draw fram border

            IGTPolygonGeometry frameBorder = GTClassFactory.Create<IGTPolygonGeometry>();
            IGTPoint oPoint;
            oPoint = GTClassFactory.Create<IGTPoint>();

            #region Draw Y Border
            try
            {
                double currY = topleft.Y;

                oPoint.X = topleft.X;
                oPoint.Y = topleft.Y;
                frameBorder.Points.Add(oPoint);

                oPoint.Y = bottomright.Y;
                frameBorder.Points.Add(oPoint);

                oPoint.X = bottomright.X;
                frameBorder.Points.Add(oPoint);

                oPoint.Y = topleft.Y;
                frameBorder.Points.Add(oPoint);

                oPoint.X = topleft.X;
                while (currY <= bottomright.Y)
                {
                    oPoint.Y = currY;
                    Debug.WriteLine("X1 : " + oPoint.X + " - Y1 : " + oPoint.Y);
                    frameBorder.Points.Add(oPoint);

                    oPoint.X = bottomright.X;
                    Debug.WriteLine("X2 : " + oPoint.X + " - Y2 : " + oPoint.Y);
                    frameBorder.Points.Add(oPoint);

                    currY += height;
                    if (currY > bottomright.Y) break;

                    oPoint.Y = currY;
                    Debug.WriteLine("X3 : " + oPoint.X + " - Y3 : " + oPoint.Y);
                    frameBorder.Points.Add(oPoint);

                    oPoint.X = topleft.X;
                    Debug.WriteLine("X4 : " + oPoint.X + " - Y4 : " + oPoint.Y);
                    frameBorder.Points.Add(oPoint);

                    currY += height;
                }
            }
            catch (Exception ex) { }
            #endregion

            #region Draw X Border
            try
            {
                double currX = bottomright.X;                

                if (oPoint.X != bottomright.X && oPoint.Y == bottomright.Y)
                {
                    oPoint.X = bottomright.X;
                    frameBorder.Points.Add(oPoint);
                }
                else if (oPoint.X == bottomright.X && oPoint.Y != bottomright.Y)
                {
                    oPoint.Y = bottomright.Y;
                    frameBorder.Points.Add(oPoint);
                }

                while (currX >= topleft.X)
                {
                    oPoint.X = currX;
                    oPoint.Y = bottomright.Y;
                    frameBorder.Points.Add(oPoint);

                    oPoint.Y = topleft.Y;
                    frameBorder.Points.Add(oPoint);

                    currX -= width;
                    if (currX < topleft.X) break;
                    oPoint.X = currX;
                    frameBorder.Points.Add(oPoint);

                    oPoint.Y = bottomright.Y;
                    frameBorder.Points.Add(oPoint);

                    currX -= width;
                }
     
            }
            catch (Exception ex)
            {
            }
            #endregion

            plot.NewRedline(frameBorder, 5010, iGroupNumber);
            iGroupNumber = oGTPlotRedline.GroupNumber;
            PPlottingLib.arrPlotGeom.Add(frameBorder);
            PPlottingLib.arrPlotStyle.Add(5010);

            #endregion

            #region Draw frame label

            int framelabel = 0;
            double lblX = topleft.X + centerX;
            double lblY = topleft.Y + centerY;
            
            for (int i = 0; i < Frames.Count; i++)
            {               
                if (!Frames[i].isHidden)
                {
                    IGTTextPointGeometry oText = GTClassFactory.Create<IGTTextPointGeometry>();
                    oText.Text = (++framelabel).ToString(); //Frames[i].labelName; 

                    IGTPoint objPoint = GTClassFactory.Create<IGTPoint>();
                    objPoint.X = lblX;
                    objPoint.Y = lblY;
                    
                    oText.Origin = objPoint;
                    oText.Alignment = GTAlignmentConstants.gtalCenterCenter;

                    oGTPlotRedline = plot.NewRedline(oText, 80300702, iGroupNumber);
                    iGroupNumber = oGTPlotRedline.GroupNumber;

                    PPlottingLib.arrPlotGeom.Add(oText);
                    PPlottingLib.arrPlotStyle.Add(80300702);
                }

                lblX += width;
                if (lblX > bottomright.X)
                {
                    lblX = topleft.X + centerX;
                    lblY += height;
                }
            }

            #endregion
        }
        #endregion

    }

}
