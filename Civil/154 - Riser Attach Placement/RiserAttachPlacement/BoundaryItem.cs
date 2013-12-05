using System;
using System.Collections.Generic;
using System.Text;
using Intergraph.GTechnology.API;

namespace NEPS.GTechnology.AssigJob
{
    public class BoundaryItem
    {
        private int _FID;

        public int FID
        {
            get { return _FID; }
            set { _FID = value; }
        }
        private string extendedName;

        public short FNO;

        public string ExtendedName
        {
            get { return extendedName; }
            set { extendedName = value; }
        }



    }
}
