using System;
using System.Collections.Generic;
using System.Text;
using Intergraph.GTechnology.API;
using ADODB;

namespace AG.GTechnology.ManholeDemolish
{
    public class DuctpathCtrl
    {
        private static Intergraph.GTechnology.API.IGTApplication application = GTClassFactory.Create<IGTApplication>();

        private IGTKeyObject _oConduit = null;
        public IGTKeyObject Conduit
        {
            set { _oConduit = value; }
            get { return _oConduit; }
        }

        private IGTPoint _Point = null;
        public IGTPoint Point
        {
            set { _Point = value; }
            get { return _Point; }
        }

        private int _fromfid = 0;
        public int FromFID
        {
            set { _fromfid = value; }
            get { return _fromfid; }
        }

        private int _tofid = 0;
        public int ToFID
        {
            set { _tofid = value; }
            get { return _tofid; }
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

        private string _label = "[Ductpath label]";
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

        private List<DuctnestCtrl> _ductnest = null;
        public List<DuctnestCtrl> DuctNest
        {
            set { _ductnest = value; }
            get { return _ductnest; }
        }

        public void LoadPoint(int ManholeFID)
        {
            _oConduit = application.DataContext.OpenFeature(2200, _fid);
            _geometry = _oConduit.Components.GetComponent(2210).Geometry;

            if (ManholeFID == _fromfid)
            {
                _connecttype = 0;
                _Point = _geometry.GetKeypointPosition(1);
            }
            else
            {
                _connecttype = 1;
                _Point = _geometry.GetKeypointPosition(_geometry.KeypointCount - 2);
            }

            IGTHelperService helper = GTClassFactory.Create<IGTHelperService>();
            helper.DataContext = application.DataContext;

            _linestyle = helper.GetComponentStyleID(_oConduit, 2210);

            _textstyle = helper.GetComponentStyleID(_oConduit, 2230);
            helper.GetComponentLabel(_oConduit, 2230, ref _label, ref _Alignment);

            if ((!_oConduit.Components.GetComponent(2202).Recordset.EOF) && (_label == "[G3E_TEXT]"))
            {
                _label = _oConduit.Components.GetComponent(2202).Recordset.Fields["G3E_TEXT"].Value.ToString();
            }

            Recordset rsComp = null;
            int recordsAffected = 0;
            int iFID = 0;

            string sSql = "select b.G3E_FID from GC_CONTAIN b where b.G3E_OWNERFID=" + _fid.ToString() + " AND b.G3E_FNO=2400";
            rsComp = application.DataContext.Execute(sSql, out recordsAffected, (int)CommandTypeEnum.adCmdText, null);

            if (rsComp != null)
            {
                _ductnest = new List<DuctnestCtrl>();
                while (!rsComp.EOF)
                {
                    DuctnestCtrl cDuctnest = new DuctnestCtrl();
                    if (rsComp.Fields["G3E_FID"].Value != DBNull.Value) iFID = Convert.ToInt32(rsComp.Fields["G3E_FID"].Value);
                    cDuctnest.ConnectType = _connecttype;
                    cDuctnest.Init(iFID);
                    _ductnest.Add(cDuctnest);

                    rsComp.MoveNext();
                }
            }
            rsComp = null;
        }
    }
}
