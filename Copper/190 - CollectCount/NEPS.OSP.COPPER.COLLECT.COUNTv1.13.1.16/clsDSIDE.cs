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
    class clsDSIDE : clsCableCount
    {
        #region PAIR COUNT

        #region PROPERTIES
        List<int> lo;
        List<int> hi;
        private int cid;
        public int CID
        {
            get { return hi.Count; }
        }
        public int Lo(int cid)
        {
            return lo[cid];
        }
        public int Hi(int cid)
        {
            return hi[cid];
        }
        private string dpnum;
        public string DP_NUM
        {
            get { return dpnum; }
        }
        #endregion

        #region D-SIDE PAIR COUNT
        private void SetCount_DSide(int cid, int lo, int hi)
        {
            if (cid == -1)
            {
                this.hi.Clear();
                this.lo.Clear();
            }
            else if (cid < this.cid)
            {
                this.hi[cid] = hi;
                this.lo[cid] = lo;
            }
            else
            {
                this.hi.Add(hi);
                this.lo.Add(lo);
                this.cid = this.hi.Count;
            }
        }

        private void ParseCountAnnotation_DSide(string anno)
        {
            count_annotation = anno;
            if (count_annotation.Length == 0)
            {
                SetCount_DSide(-1, 0, 0);
                return;
            }

            anno += Environment.NewLine;
            int cid = 0;
            for (int i = anno.IndexOf(Environment.NewLine); i > -1; i = anno.IndexOf(Environment.NewLine), cid++)
            {
                string[] c = anno.Remove(i).Split('-');

                #region Collect Count Annotation 2013-01-11 : handle collect count label (ORIG + CC)
                int ori = c[0].IndexOf("(ORIG)");
                int cc = c[0].IndexOf("(CC)");
                if (ori > -1)
                {
                    count_ori = anno.Substring(ori + 7);
                    break; // original count before collect count was performed
                }
                if (cc > -1)
                    throw new System.Exception("Cable is a COLLECT COUNT cable");
                #endregion

                int j = c[0].IndexOf(',');
                if (j > -1) c[0] = c[0].Substring(j + 1);
                SetCount_DSide(cid, myUtil.ParseInt(c[0]), myUtil.ParseInt(c[1]));
                anno = anno.Substring(i + 2);
                if (anno.Length == 0) break;
            }
        }
        #endregion

        #endregion

        #region CONSTRUCTOR

        public clsDSIDE()
        {
            this.hi = new List<int>();
            this.lo = new List<int>();
        }

        public string ToString()
        {
            string txt = "EXC_ABB : " + this.exc_abb + Environment.NewLine;
            txt += "CABINET CODE : " + this.CABINET_CODE + Environment.NewLine;
            txt += "CABLE CODE : " + this.CABLE_CODE + Environment.NewLine;
            if (dpnum.Length > 0) txt += "DP NUMBER : " + dpnum;
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

                if (this.cable_class != "D-CABLE") return false;
                try
                {
                    switch (short.Parse(rs.Fields["OUT_FNO"].Value.ToString()))
                    {
                        case 13100: //PDP_FNO
                            dpnum = myUtil.GetFieldValue("GC_PDP", "PDP_NUM",
                                int.Parse(rs.Fields["OUT_FID"].Value.ToString()));
                            break;
                        case 13000: //DP_FNO:
                            dpnum = myUtil.GetFieldValue("GC_DP", "DP_NUM",
                                int.Parse(rs.Fields["OUT_FID"].Value.ToString()));
                            break;
                        case 10800: //JNT_FNO:                            
                            if (this.cable_class.IndexOf("STU") == -1)
                                return false;
                            break;

                        default: return false;
                    }
                }
                catch { dpnum = ""; }

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
                ParseCountAnnotation_DSide(this.count_annotation);

                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion

        #region COLLECT COUNT D-SIDE
        public int isDSideCountAvailable(int lo, int hi)
        {
            for (int cid = 0; cid < this.CID; cid++)
            {
                if (this.lo[cid] <= lo && hi <= this.hi[cid])
                {
                    return cid;
                }
            }
            return -1;
        }

        public bool TransferCountDSide(int lo, int hi, int cid)
        {
            if (cid > -1 && cid < this.CID)
            {
                if (this.lo[cid] == lo && this.hi[cid] == hi)
                {
                    this.lo.RemoveAt(cid);
                    this.hi.RemoveAt(cid);
                }
                else if (this.lo[cid] == lo)
                {
                    this.lo[cid] = hi + 1;
                }
                else if (this.hi[cid] == hi)
                {
                    this.hi[cid] = lo - 1;
                }
                else
                {
                    this.hi.Insert(cid, lo - 1);
                    this.lo.Insert(cid + 1, hi + 1);
                }
                this.effective_pairs -= (hi - lo + 1);
                return true;
            }
            else
                return false;
        }

        public void ReceiveCount(int lo, int hi)
        {
            SetCount_DSide(this.cid + 1, lo, hi);
        }
        #endregion


    }
}
