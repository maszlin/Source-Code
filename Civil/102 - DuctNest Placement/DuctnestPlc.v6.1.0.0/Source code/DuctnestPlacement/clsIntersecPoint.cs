using System;
using System.Collections.Generic;
using System.Text;

namespace NEPS.GTechnology.NEPSDuctNestPlc
{
    /// <summary>
    /// This struct has all the columns of the table IW_LOCATEFEATURE and can hold an entire
    /// line of this table.
    /// </summary>
    public class clsIntersecPoint
    {
        #region Getters/Setters

        /// <summary>
        /// FNO of the feature.
        /// </summary>
        private int fid = 0;
        public int G3E_FID
        {
            set
            { fid = value; }

            get
            { return fid; }
        }

        private double _x = 0;
        public double X
        {
            set
            { _x = value; }

            get
            { return _x; }
        }

        private double _y = 0;
        public double Y
        {
            set
            { _y = value; }

            get
            { return _y; }
        }

        private int _order = 0;
        public int Order
        {
            set
            { _order = value; }

            get
            { return _order; }
        }

        private double _Elevation = 0;
        public double Elevation
        {
            set
            { _Elevation = value; }

            get
            { return _Elevation; }
        }
        #endregion

    }
}
