using System;
using System.Collections.Generic;
using System.Text;

namespace NEPS.GTechnology.BulkMoveFeatures
{
    public class Point
    {
        public double X;
        public double Y;

        public Point(double _X, double _Y)
        {
            X = _X;
            Y = _Y;
        }
    }

    class Rectangle
    {
        public double Left;
        public double Top;
        public double Right;
        public double Bottom;
        public Point TopLeft = null;
        public Point BottomRight = null;

        public Rectangle(double left, double top, double right, double bottom)
        {
            Left = left;
            Top = top;
            Right = right;
            Bottom = bottom;
            TopLeft = new Point(left, top);
            BottomRight = new Point(right, bottom);
        }



        public Point Center
        {
            get
            {
                Point output = new Point(
              (Right - Left) / 2 + Left,
              (Top - Bottom) / 2 + Bottom);

                return output;
            }

        }

        public double Height
        {
            get
            {
                double output = Top - Bottom;
                output = Math.Abs(output);
                return output;
            }

        }


        public double Width
        {
            get
            {
                double output = Right - Left;
                output = Math.Abs(output);
                return output;
            }

        }




    }
}
