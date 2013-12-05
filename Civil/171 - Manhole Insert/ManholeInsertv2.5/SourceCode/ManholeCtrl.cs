using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using Intergraph.GTechnology.API;

using ADODB;

namespace AG.GTechnology.ManholeInsert
{
    public partial class ManholeCtrl : UserControl
    {
        public ManholeCtrl()
        {
            InitializeComponent();
        }

        private IGTKeyObject _oManhole = null;
        public IGTKeyObject Manhole
        {
            set { _oManhole = value; }
            get { return _oManhole; }
        }

        private int _fid = 0;
        public int FID
        {
            set { _fid = value; }
            get { return _fid; }
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

        private IGTPoint _Origin = null;
        public IGTPoint Origin
        {
            set { _Origin = value; }
            get { return _Origin; }
        }

        private IGTPoint _RotationPnt = null;
        public IGTPoint RotationPnt
        {
            set { _RotationPnt = value; }
            get { return _RotationPnt; }
        }

        private IGTPoint _LabelOrigin = null;
        public IGTPoint LabelOrigin
        {
            set { _LabelOrigin = value; }
            get { return _LabelOrigin; }
        }

        private string _type = string.Empty;
        public string ManholeType
        {
            set 
            {
                _type = value;  
            }
            get 
            {
                return _type; 
            }
        }

        private int _fNbr = 0;
        public int FirstNumber
        {
            set
            {
                _fNbr = value;
            }
            get
            {
                return _fNbr;
            }
        }

        private string _label = "[Manhole Label]";
        public string ManholeLabel
        {
            set
            {
                _label = value;
            }
            get
            {
                return _label;
            }
        }
        private string _label_des = "[Manhole Label Desc]";
        public string ManholeLabelDesription
        {
            set
            {
                _label_des = value;
            }
            get
            {
                return _label_des;
            }
        }
        private GTAlignmentConstants _labelAlignment = GTAlignmentConstants.gtalBottomLeft;
        public GTAlignmentConstants ManholeLabelAlignment
        {
            set
            {
                _labelAlignment = value;
            }
            get
            {
                return _labelAlignment;
            }
        }

        private int _textstyle = 2730001;
        public int ManholeTextStyle
        {
            set
            {
                _textstyle = value;
            }
            get
            {
                return _textstyle;
            }
        }
        private int _wallstyle = 30600;
        public int ManholeWallStyle
        {
            set
            {
                _wallstyle = value;
            }
            get
            {
                return _wallstyle;
            }
        }

        private int _style = 2720001;
        public int ManholeStyle
        {
            set
            {
                _style = value;
            }
            get
            {
                return _style;
            }
        }

        private int _tempstyle = 2720001;
        public int ManholeTempStyle
        {
            set
            {
                _tempstyle = value;
            }
            get
            {
                return _tempstyle;
            }
        }

        public void LoadManholeType()
        {
           
           if (string.IsNullOrEmpty(_type)) return;

            Intergraph.GTechnology.API.IGTApplication application;
            application = GTClassFactory.Create<IGTApplication>();
            LoadManholeWall();
            LoadManholeBoundary();
        }

        public void LoadManholeWall()
        {
            Recordset rsComp = null;
            int recordsAffected = 0;

            if (string.IsNullOrEmpty(_type)) return;
            if (ManholeWall == null) _ManholeWall = new List<ManholePoint>();
            _ManholeWall.Clear();

            Intergraph.GTechnology.API.IGTApplication application;
            application = GTClassFactory.Create<IGTApplication>();

            string sSql = "select b.X, b.Y, b.ORD from AG_MANHL_WALL b where MANHOLE_TYPE='" + _type + "' order by b.ORD";
            rsComp = application.DataContext.Execute(sSql, out recordsAffected, (int)CommandTypeEnum.adCmdText, null);

            if (rsComp != null)
            {
                while (!rsComp.EOF)
                {
                    ManholePoint _pnt = new ManholePoint();
                    if (rsComp.Fields["X"].Value != DBNull.Value) _pnt.X = Convert.ToDouble(rsComp.Fields["X"].Value);
                    if (rsComp.Fields["Y"].Value != DBNull.Value) _pnt.Y = Convert.ToDouble(rsComp.Fields["Y"].Value);
                    if (rsComp.Fields["ORD"].Value != DBNull.Value) _pnt.ORD = Convert.ToInt32(rsComp.Fields["ORD"].Value);

                    _ManholeWall.Add(_pnt);

                    rsComp.MoveNext();
                }
            }
            rsComp = null;
        }

        public void LoadManholeBoundary()
        {
            Recordset rsComp = null;
            int recordsAffected = 0;

            if (string.IsNullOrEmpty(_type)) return;
            if (_Boundary == null) _Boundary = new List<ManholePoint>();
            _Boundary.Clear();

            Intergraph.GTechnology.API.IGTApplication application;
            application = GTClassFactory.Create<IGTApplication>();

            string sSql = "select b.X1, b.Y1, b.X2, b.Y2, b.ORD from AG_MANHL_BND b where MANHOLE_TYPE='" + _type + "' order by b.ORD";
            rsComp = application.DataContext.Execute(sSql, out recordsAffected, (int)CommandTypeEnum.adCmdText, null);

            if (rsComp != null)
            {
                while (!rsComp.EOF)
                {
                    ManholePoint _pnt = new ManholePoint();
                    if (rsComp.Fields["X1"].Value != DBNull.Value) _pnt.X = Convert.ToDouble(rsComp.Fields["X1"].Value);
                    if (rsComp.Fields["Y1"].Value != DBNull.Value) _pnt.Y = Convert.ToDouble(rsComp.Fields["Y1"].Value);
                    if (rsComp.Fields["ORD"].Value != DBNull.Value) _pnt.ORD = Convert.ToInt32(rsComp.Fields["ORD"].Value);

                    _Boundary.Add(_pnt);

                    _pnt = new ManholePoint();
                    if (rsComp.Fields["X2"].Value != DBNull.Value) _pnt.X = Convert.ToDouble(rsComp.Fields["X2"].Value);
                    if (rsComp.Fields["Y2"].Value != DBNull.Value) _pnt.Y = Convert.ToDouble(rsComp.Fields["Y2"].Value);
                    if (rsComp.Fields["ORD"].Value != DBNull.Value) _pnt.ORD = Convert.ToInt32(rsComp.Fields["ORD"].Value);

                    _Boundary.Add(_pnt);

                    rsComp.MoveNext();
                }
            }
            rsComp = null;
        }

        private List<ManholePoint> _ManholeWall = null;
        public List<ManholePoint> ManholeWall
        {
            set
            {
                _ManholeWall = value;
            }
            get
            {
                return _ManholeWall;
            }
        }

        private List<ManholePoint> _Boundary = null;
        public List<ManholePoint> Boundary
        {
            set
            {
                _Boundary = value;
            }
            get
            {
                return _Boundary;
            }
        }
    }
}
