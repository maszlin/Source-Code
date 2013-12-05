using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

using System.Drawing;
using Intergraph.GTechnology.Interfaces;
using Intergraph.GTechnology.API;
using ADODB;

namespace NEPS.OSP.COPPER.COLLECT.COUNT
{
    class clsCableCount
    {
        #region PROPERTIES
        protected int fid;
        public int FID
        {
            get { return fid; }
        }
        protected short fno;
        public short FNO
        {
            get { return fno; }
        }
        protected int out_fid;
        public int OUT_FID
        {
            get { return out_fid; }
        }
        protected short out_fno;
        public short OUT_FNO
        {
            get { return out_fno; }
        }
        protected string exc_abb;
        public string EXC_ABB
        {
            get { return exc_abb; }
        }
        protected string cable_code;
        public string CABLE_CODE
        {
            get { return cable_code; }
        }
        protected string cable_class;
        public string CABLE_CLASS
        {
            get { return cable_class; }
        }
        protected string feature_state;
        public string FEATURE_STATE
        {
            get { return feature_state; }
        }
        protected string itface_code;
        protected string rt_code;
        public string CABINET_CODE
        {
            get
            {
                return (itface_code.Length > 0 ? itface_code : rt_code);
            }
        }
        protected string count_annotation;
        public string COUNT_ANNO
        {
            get { return count_annotation; }
        }
        protected string count_ori;
        public string COUNT_ORI
        {
            get { return count_ori; }
        }
        protected int effective_pairs;
        public int EFFECTIVE_PAIRS
        {
            get { return effective_pairs; }
        }
        protected IGTPoint end_point;
        public IGTPoint END_POINT
        {
            get { return end_point; }
        }
        protected int detailID;
        public int DETAIL_ID
        {
            get { return detailID; }
        }
        #endregion

        #region CONSTRUCTOR

        public clsCableCount()
        {
            this.fid = -1;
            this.fno = -1;
            this.out_fid = -1;
            this.out_fno = -1;
            this.exc_abb = "";
            this.feature_state = "";
            this.cable_class = "";
            this.cable_code = "";
            this.effective_pairs = 0;
            this.itface_code = "";
            this.rt_code = "";
            this.count_annotation = "";
            this.count_ori = "";
            this.detailID = -1;
        }

        #endregion

        #region CABLE PROPERTIES
        short CBLFNO = 7000;
        public bool isCable(IGTSelectedObjects selFeature)
        {
            if (selFeature.FeatureCount == 1 && selFeature.GetObjects()[0].FNO == CBLFNO)
                return true;
            else
                return false;
        }

        #endregion

        #region PlaceLabel
        public void LabelPlacement()
        {
            string label = myUtil.GetFieldValue("GC_CBL", "COUNT_ANNOTATION", this.FID);
            if (label.Length > 0)
                GTCollectCount.tempLabel = new clsMoveLabel(FID, label, this.cable_class );
            else
            {
                GTCollectCount.tempLabel.FlagMove = false;
            }
        }
        protected void GetEndPoint(string geo, string det)
        {
            end_point = GTClassFactory.Create<IGTPoint>();
            try
            {
                string vector = (this.detailID > 0 ? det : geo);
                string[] points = vector.Split('|');
                end_point.X = double.Parse(points[points.Length - 2]);
                end_point.Y = double.Parse(points[points.Length - 1]);
            }
            catch
            {
                end_point.X = 0; end_point.Y = 0;
            }
        }
        #endregion
    }
}
