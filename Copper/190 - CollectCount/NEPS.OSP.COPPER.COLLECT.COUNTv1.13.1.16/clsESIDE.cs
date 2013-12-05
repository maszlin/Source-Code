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
    class clsESIDE : clsCableCount
    {

        #region PAIR COUNT

        #region PROPERTIES
        List<int> lo;
        List<int> hi;
        List<int> mdf;
        List<int> vert;
        List<int> vhi;
        List<int> vlo;
        private int cid;
        public int CID
        {
            get { return hi.Count; }
        }
        public int MDF(int cid)
        {
            return mdf[cid];
        }
        public int Vertical(int cid)
        {
            return vert[cid];
        }
        public int Vert_Lo(int cid)
        {
            return vlo[cid];
        }
        public int Vert_Hi(int cid)
        {
            return vhi[cid];
        }
        public int Lo(int cid)
        {
            return lo[cid];
        }
        public int Hi(int cid)
        {
            return hi[cid];
        }
        private string endText;
        public string END_LABEL
        {
            get { return endText; }
        }

        #endregion

        #region E-SIDE Pair Count
        public void SetCount_ESide(int cid, int mdf, int vert, int vert_lo, int vert_hi, int pair_lo, int pair_hi)
        {
            if (cid == -1)
            {
                this.mdf.Clear();
                this.vert.Clear();
                this.vhi.Clear();
                this.vlo.Clear();
                this.hi.Clear();
                this.lo.Clear();
            }
            else if (cid < this.cid)
            {
                this.mdf[cid] = mdf;
                this.vert[cid] = vert;
                this.vhi[cid] = vert_hi;
                this.vlo[cid] = vert_lo;
                this.hi[cid] = pair_hi;
                this.lo[cid] = pair_lo;
            }
            else
            {
                this.mdf.Add(mdf);
                this.vert.Add(vert);
                this.vhi.Add(vert_hi);
                this.vlo.Add(vert_lo);
                this.hi.Add(pair_hi);
                this.lo.Add(pair_lo);
                //this.cid = this.hi.Count;
            }
        }

        private void ParseCountAnnotation_ESide(string annotation)
        {
            count_annotation = annotation;
            if (count_annotation.Length == 0)
            {
                SetCount_ESide(-1, 0, 0, 0, 0, 0, 0);
                return;
            }

            annotation += Environment.NewLine;
            int cid = 0;
            int i = annotation.IndexOf(Environment.NewLine);
            for (; i > -1; i = annotation.IndexOf(Environment.NewLine), cid++)
            {
                string[] anno = (i == -1 ? annotation.Split('/') : annotation.Remove(i).Split('/'));
                string[] vert = anno[3].Split('-');
                string[] pair = anno[4].Split('-');

                #region Collect Count Annotation 2013-01-11 : handle collect count label (ORIG + CC)
                int ori = anno[0].IndexOf("(ORIG)");
                int cc = anno[0].IndexOf("(CC)");
                if (ori > -1) break;
                if (cc > -1) throw new System.Exception("Cable is a COLLECT COUNT cable");
                #endregion

                SetCount_ESide(cid, int.Parse(anno[1]), int.Parse(anno[2]),
                    int.Parse(vert[0]), int.Parse(vert[1]),
                    int.Parse(pair[0]), int.Parse(pair[1]));

                annotation = annotation.Substring(i + 2);
                if (annotation.Length == 0) break;
            }
        }

        #endregion

        #endregion

        #region CONSTRUCTOR

        public clsESIDE()
        {
            this.mdf = new List<int>();
            this.vert = new List<int>();
            this.vhi = new List<int>();
            this.vlo = new List<int>();
            this.hi = new List<int>();
            this.lo = new List<int>();
            this.endText = "";
        }
        public string ToString()
        {
            string txt = "EXC_ABB : " + this.exc_abb + Environment.NewLine;
            txt += "CABLE CODE : " + this.CABLE_CODE + Environment.NewLine;
            if (endText.Length > 0) txt += "END POINT : " + endText;
            return txt;
        }

        #endregion

        #region CABLE PROPERTIES

        public bool ReadCable(int cableFID)
        {
            string ssql = "SELECT A.*, B.OUT_FID, B.OUT_FNO, C.EXC_ABB, C.FEATURE_STATE, ";
            ssql += "(SELECT GC_OSP_COP_VAL.EXTRACT_LINE_TO_VECTOR(G3E_GEOMETRY) FROM GC_CBL_L WHERE G3E_FID = A.G3E_FID) CBL_GEO_XY, ";
            ssql += "(SELECT GC_OSP_COP_VAL.EXTRACT_LINE_TO_VECTOR(G3E_GEOMETRY) FROM DGC_CBL_L WHERE G3E_FID = A.G3E_FID) CBL_DET_XY, ";
            ssql += "(SELECT G3E_DETAILID FROM DGC_CBL_L WHERE G3E_FID = A.G3E_FID) G3E_DETAILID ";
            ssql += "FROM GC_CBL A, GC_NR_CONNECT B, GC_NETELEM C ";
            ssql += "WHERE B.G3E_FID = A.G3E_FID AND C.G3E_FID = A.G3E_FID AND A.G3E_FID = " + cableFID.ToString();

            ADODB.Recordset rs = GTClassFactory.Create<IGTApplication>().DataContext.OpenRecordset(ssql,
                ADODB.CursorTypeEnum.adOpenStatic, ADODB.LockTypeEnum.adLockReadOnly, 1);

            if (rs.RecordCount == 1)
            {
                this.cable_class =
                    (rs.Fields["CABLE_CLASS"].Value.ToString().IndexOf("D") > -1 ? "D-CABLE" : "E-CABLE");
                if (this.cable_class != "E-CABLE") return false;

                switch (short.Parse(rs.Fields["OUT_FNO"].Value.ToString()))
                {
                    case 10300: //CABINET
                        endText = myUtil.GetFieldValue("GC_DP", "DP_NUM",
                            int.Parse(rs.Fields["OUT_FID"].Value.ToString()));
                        break;
                    case 6300: // DDP
                        endText = myUtil.GetFieldValue("GC_DDP", "DDP_NUM",
                            int.Parse(rs.Fields["OUT_FID"].Value.ToString()));
                        break;
                    case 6200: // PDDP
                        endText = myUtil.GetFieldValue("GC_PDDP", "PDDP_CODE",
                            int.Parse(rs.Fields["OUT_FID"].Value.ToString()));
                        break;
                    case 10800: //JNT_FNO:
                        if (this.cable_class.IndexOf("STU") == -1)
                            return false;
                        break;

                    default: return false;
                }

                this.fid = cableFID;
                this.fno = short.Parse(rs.Fields["G3E_FNO"].Value.ToString());
                this.out_fid = int.Parse(rs.Fields["OUT_FID"].Value.ToString());
                this.out_fno = short.Parse(rs.Fields["OUT_FNO"].Value.ToString());
                this.exc_abb = rs.Fields["EXC_ABB"].Value.ToString();
                this.feature_state = rs.Fields["FEATURE_STATE"].Value.ToString();
                this.cable_class =
                    (rs.Fields["CABLE_CLASS"].Value.ToString().IndexOf("D") > -1 ? "D-CABLE" : "E-CABLE");
                this.cable_code = rs.Fields["CABLE_CODE"].Value.ToString();
                this.effective_pairs = int.Parse(rs.Fields["EFFECTIVE_PAIRS"].Value.ToString());
                this.itface_code = rs.Fields["ITFACE_CODE"].Value.ToString();
                this.rt_code = rs.Fields["RT_CODE"].Value.ToString();
                this.count_annotation = rs.Fields["COUNT_ANNOTATION"].Value.ToString();
                this.detailID = myUtil.ParseInt(myUtil.rsField(rs, "G3E_DETAILID"));

                GetEndPoint(myUtil.rsField(rs, "CBL_GEO_XY"), myUtil.rsField(rs, "CBL_DET_XY"));
                ParseCountAnnotation_ESide(this.count_annotation);

                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion

        #region COLLECT COUNT E-SIDE 2013-01-15
        public int isESideCountAvailable(int mdf, int vert, int vlo, int vhi)
        {
            for (int cid = 0; cid < this.CID; cid++)
            {
                if (this.vlo[cid] <= vlo && vhi <= this.vhi[cid])
                {
                    if (this.mdf[cid] == mdf && this.vert[cid] == vert)
                        return cid;
                    else
                        throw new System.Exception("The selected MDF, VERTICAL, VERTICAL LOW and HIGH not available in the source cable");
                }
            }
            return -1;
        }

        public bool TransferCountESide(int cid, int mdf, int vert, int vlo, int vhi, int lo, int hi)
        {
            if (cid > -1 && cid < this.CID)
            {
                if (this.vlo[cid] == vlo && this.vhi[cid] == vhi)
                {
                    this.mdf.RemoveAt(cid);
                    this.vert.RemoveAt(cid);
                    this.vlo.RemoveAt(cid);
                    this.vhi.RemoveAt(cid);
                    this.lo.RemoveAt(cid);
                    this.hi.RemoveAt(cid);
                }
                else
                {
                    if (this.vlo[cid] == vlo)
                    {
                        this.vlo[cid] = vhi + 1;
                        this.lo[cid] = hi + 1;
                    }
                    else if (this.vhi[cid] == vhi)
                    {
                        this.vhi[cid] = vlo - 1;
                        this.hi[cid] = lo - 1;
                    }
                    else
                    {
                        this.mdf.Insert(cid, mdf);
                        this.vert.Insert(cid, vert);
                        this.vhi.Insert(cid, vlo - 1);
                        this.vlo.Insert(cid + 1, vhi + 1);
                        this.hi.Insert(cid, lo - 1);
                        this.lo.Insert(cid + 1, hi + 1);
                    }

                }
                this.effective_pairs -= (hi - lo + 1);
                return true;
            }
            else
                return false;
        }

        public void ReceiveCountESide(int mdf, int vert, int vlo, int vhi, int lo, int hi)
        {
            SetCount_ESide(this.cid + 1, mdf, vert, vlo, vhi, lo, hi);
        }

        #endregion

    }
}
