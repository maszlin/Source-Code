using System;
using System.Collections.Generic;
using System.Text;
using Intergraph.GTechnology.API;

namespace AG.GTechnology.StubStumpPlacement
{
    public class clsStumpStub
    {
        private int _SourceCblfid = 0;
        public int SourceCblFID
        {
            set { _SourceCblfid = value; }
            get { return _SourceCblfid; }
        }

        private int _SourceDevfid = 0;
        public int SourceDevFID
        {
            set { _SourceDevfid = value; }
            get { return _SourceDevfid; }
        }

        private short _SourceDevfno = 0;
        public short SourceDevFNO
        {
            set { _SourceDevfno = value; }
            get { return _SourceDevfno; }
        }

        private int _fid = 0;
        public int FID
        {
            set { _fid = value; }
            get { return _fid; }
        }

        private double _angle = 0;
        public double Angle
        {
            set { _angle = value; }
            get { return _angle; }
        }

        private string _side = string.Empty;
        public string Side
        {
            set { _side = value; }
            get { return _side; }
        }

        private string _type = string.Empty;
        public string Type
        {
            set { _type = value; }
            get { return _type; }
        }

        private string _Name = string.Empty;
        public string Name
        {
            get { return _type.ToUpper() + " " + _side.ToUpper(); }
        }

        private string _cablecode = string.Empty;
        public string CableCode
        {
            set { _cablecode = value; }
            get { return _cablecode; }
        }

        private IGTPoint _startPoint = null;
        public IGTPoint StartPoint
        {
            set { _startPoint = value; }
            get { return _startPoint; }
        }

        private IGTPoint _endPoint = null;
        public IGTPoint EndPoint
        {
            set { _endPoint = value; }
            get { return _endPoint; }
        }

        private IGTPolylineGeometry _line = null;
        public IGTPolylineGeometry Line
        {
            set { _line = value; }
            get { return _line; }
        }
    }
}
