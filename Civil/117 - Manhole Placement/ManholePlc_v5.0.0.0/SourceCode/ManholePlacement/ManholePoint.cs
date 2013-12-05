using System;
using System.Collections.Generic;
using System.Text;

namespace AG.GTechnology.ManholePlacement
{
    public class ManholePoint
    {
        private int _order = 1;
        public int ORD
        {
            set { _order = value; }
            get { return _order; }
        }

        private double _x = 0;
        public double X
        {
            set { _x = value; }
            get { return _x; }
        }

        private double _y = 0;
        public double Y
        {
            set { _y = value; }
            get { return _y; }
        }
    }
}
