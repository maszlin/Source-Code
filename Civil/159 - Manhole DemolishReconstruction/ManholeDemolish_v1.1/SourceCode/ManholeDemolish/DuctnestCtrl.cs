using System;
using System.Collections.Generic;
using System.Text;
using Intergraph.GTechnology.API;
using ADODB;

namespace AG.GTechnology.ManholeDemolish
{
    public class DuctnestCtrl
    {
        private static Intergraph.GTechnology.API.IGTApplication application = GTClassFactory.Create<IGTApplication>();

        private IGTKeyObject _oDuctnest = null;
        public IGTKeyObject Ductnest
        {
            set { _oDuctnest = value; }
            get { return _oDuctnest; }
        }

        private IGTPoint _Point = null;
        public IGTPoint Point
        {
            set { _Point = value; }
            get { return _Point; }
        }

        private int _fid = 0;
        public int FID
        {
            set { _fid = value; }
            get { return _fid; }
        }

        private IGTGeometry _geometry = null;
        public IGTGeometry Geometry
        {
            set { _geometry = value; }
            get { return _geometry; }
        }

        private string _label = "[Ductnest label]";
        public string Label
        {
            set { _label = value; }
            get { return _label; }
        }

        private int _linestyle = 0;
        public int LineStyle
        {
            set { _linestyle = value; }
            get { return _linestyle; }
        }

        private int _textstyle = 0;
        public int TextStyle
        {
            set { _textstyle = value; }
            get { return _textstyle; }
        }

        private GTAlignmentConstants _Alignment = GTAlignmentConstants.gtalCenterCenter;
        public GTAlignmentConstants Alignment
        {
            set { _Alignment = value; }
            get { return _Alignment; }
        }

        private int _connecttype = 0;
        public int ConnectType
        {
            set { _connecttype = value; }
            get { return _connecttype; }
        }

        public void Init(int FID)
        {
            _fid = FID;
            _oDuctnest = application.DataContext.OpenFeature(2400, _fid);
            short iCNO = 2410;
            if (_connecttype==0)
                iCNO = 2410;
            else
                iCNO = 2412;
            _geometry = _oDuctnest.Components.GetComponent(iCNO).Geometry;

            IGTHelperService helper = GTClassFactory.Create<IGTHelperService>();
            helper.DataContext = application.DataContext;

            _linestyle = helper.GetComponentStyleID(_oDuctnest, iCNO);

            _textstyle = helper.GetComponentStyleID(_oDuctnest, 2430);
            helper.GetComponentLabel(_oDuctnest, 2430, ref _label, ref _Alignment);
        }
    }
}
