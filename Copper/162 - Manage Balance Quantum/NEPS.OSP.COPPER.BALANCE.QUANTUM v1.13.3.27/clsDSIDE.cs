/*
 * edited : m.zam @ 2012-10-13
 * issues : multiple count on single cable
 * - change lo & hi from int to List<int>
 * - add CID property represent number of count available
 * 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using Intergraph.GTechnology.Interfaces;
using Intergraph.GTechnology.API;
using ADODB;

namespace NEPS.OSP.COPPER.BALANCE.QUANTUM
{
    class clsDSIDE
    {
        #region Declaration

        public string cablecode;
        public static int max_lo = 0;
        public static int max_hi = 0;

        private struct Overlapped
        {
            public int row1;
            public int row2;
            public Overlapped(int row1, int row2)
            {
                if (row1 < row2)
                {
                    this.row1 = row1;
                    this.row2 = row2;
                }
                else
                {
                    this.row1 = row2;
                    this.row2 = row1;
                }
            }
        }
        private List<Overlapped> CountOverlap = new List<Overlapped>();

        public struct PairCount
        {
            public string termNo;
            public int termFID;
            public short termFNO;
            public string termType;
            public int detailID;
            public int cblFID;
            public int efctvPair;
            public int cableSize;
            public string featureState;
            public bool overlapped;
            public bool unbalanced;

            List<int> lo;
            List<int> hi;
            public int CID;

            public PairCount(string termno, int termfid, short termfno, string termtype, int cblfid, int efctv, int cblsize, string featstate,
                int detailid)
            {
                termNo = termno;
                termFID = termfid;
                termFNO = termfno;
                termType = termtype;
                cblFID = cblfid;
                efctvPair = efctv;
                cableSize = cblsize;
                featureState = featstate;

                detailID = detailid;

                lo = new List<int>();
                hi = new List<int>();
                CID = 0;

                overlapped = false;
                unbalanced = false;
            }

            public int Lo(int cid)
            {
                return lo[cid];
            }
            public int Hi(int cid)
            {
                return hi[cid];
            }
            public bool SetCount(int cid, int lo, int hi)
            {
                if (cid < CID)
                {
                    this.hi[cid] = hi;
                    this.lo[cid] = lo;
                }
                else
                {
                    this.hi.Add(hi);
                    this.lo.Add(lo);
                    CID = this.hi.Count;
                }

                //if (efctvPair == 0 && hi == 0 && lo == 0)
                if (hi == 0 && lo == 0)
                    unbalanced = false;
                else if (lo < max_lo || hi > max_hi)
                    unbalanced = true;
                else
                {
                    int effective = efctvPair;
                    for (int i = 0; i < this.hi.Count; i++)
                    {
                        if (this.hi[i] != 0 || this.lo[i] != 0)
                            effective -= (this.hi[i] - this.lo[i] + 1);
                    }

                    unbalanced = (effective < 0) || (hi < lo);
                }
                return !unbalanced;
            }
            public bool SetCount(string count_annotation)
            {
                string anno = count_annotation + Environment.NewLine;
                if (count_annotation.Length == 0)
                {
                    SetCount(1, 0, 0);
                    return true;
                }

                int cid = 0;
                bool flag = true;
                for (int i = anno.IndexOf(Environment.NewLine); i > -1; i = anno.IndexOf(Environment.NewLine, cid++))
                {
                    string[] c = anno.Remove(i).Split('-');
                    int j = c[0].IndexOf(',');
                    if (j > -1) c[0] = c[0].Substring(j + 1);
                    if (!SetCount(cid, myUtil.ParseInt(c[0]), myUtil.ParseInt(c[1]))) flag = false;
                    anno = anno.Substring(i + 2);
                    if (anno.Length == 0) break;
                }
                return flag;
            }
            public bool RemoveCount(int cid)
            {
                if (cid != CID || CID == 1)
                    return false;
                else
                {
                    CID--;
                    this.hi.RemoveAt(cid - 1);
                    this.lo.RemoveAt(cid - 1);
                }
                return true;
            }
            public bool isOverlapped(int cid, int lo, int hi)
            {
                if (lo == 0 && hi == 0)
                    return false;
                else
                {
                    //for (int i = 0; i < this.hi.Count; i++)
                    if (!(hi < this.lo[cid] || lo > this.hi[cid]))
                    {
                        overlapped = true;
                        return true;
                    }
                    return false;
                }
            }
        }
        public SortedDictionary<int, PairCount> TermPoints = new SortedDictionary<int, PairCount>();

        public List<Point> CountGeo = new List<Point>();


        public void SetCableCode(string code)
        {
            try
            {
                int c = int.Parse(code.Trim().Substring(1)); // remove D from cable code and convert to integer
                cablecode = code;
                max_hi = (c * 200);
                max_lo = max_hi - 200 + 1;
            }
            catch { }
        }
        #endregion

        #region Trace Downstream

        public void GetTermPoints(int cabFID, string cableCode, Logger log)
        {
            TermPoints.Clear();
            CountOverlap.Clear();

            SetCableCode(cableCode);

            string ssql = "SELECT A.G3E_FID FROM GC_NR_CONNECT A, GC_CBL B WHERE A.IN_FID = " + cabFID.ToString(); // CABINET FID
            ssql += " AND A.G3E_FID = B.G3E_FID AND B.CABLE_CODE = '" + cableCode + "'";

            List<string> cblsFID = myUtil.GetMultipleValue(ssql, "G3E_FID");

            string DPs = "";
            string PDPs = "";
            string CBLs = "";

            foreach (string sFID in cblsFID) // read all cable with the same cable_code
            {
                Dictionary<int, int> endsFID = clsTrace.TraceDown_GetEndPoints(int.Parse(sFID), "G3E_FID");
                foreach (int key in endsFID.Keys)
                {
                    int endsFNO = endsFID[key];
                    switch (endsFNO) // obj[0] : term_FNO; obj[1] : term_FID; obj[2] : end_cbl_FID
                    {
                        case 13000: //DP
                            DPs += (DPs.Length > 0 ? "," : "") + key.ToString();
                            break;
                        case 13100: // PDP
                            PDPs += (PDPs.Length > 0 ? "," : "") + key.ToString();
                            break;
                        default: // CBL or SPLICE
                            CBLs += (CBLs.Length > 0 ? "," : "") + key.ToString();
                            break;
                    }
                }
            }

            GetTermPoints(DPs, PDPs, CBLs, log);
        }

        private void GetTermPoints(string DPCables, string PDPCables, string endCables, Logger log)
        {
            TermPoints.Clear();
            CountOverlap.Clear();
            // FNO : 13000 = DP; 
            if (DPCables.Length > 0)
            {
                string ssql = "SELECT A.G3E_FID, A.EFFECTIVE_PAIRS, A.TOTAL_SIZE, B.G3E_FID TERM_FID, B.G3E_FNO TERM_FNO, B.DP_NUM TERM_NO, 'DP' TERM_TYPE, D.FEATURE_STATE, ";
                //                ssql += "(SELECT CURRENT_LOW||'-'||CURRENT_HIGH FROM GC_COUNT WHERE G3E_FID = A.G3E_FID AND ROWNUM = 1) ANNOTATION, ";
                ssql += "A.COUNT_ANNOTATION, ";
                ssql += "(SELECT G3E_DETAILID FROM DGC_CBL_L WHERE G3E_FID = A.G3E_FID) G3E_DETAILID ";
                ssql += "FROM GC_CBL A, GC_DP B, GC_NR_CONNECT C, GC_NETELEM D ";
                ssql += "WHERE A.G3E_FID = C.G3E_FID AND A.G3E_FID = D.G3E_FID AND C.OUT_FID = B.G3E_FID ";
                ssql += "AND A.CABLE_CODE = '" + cablecode + "' AND B.G3E_FID IN (" + DPCables + ") ORDER BY TERM_NO";
                GetTermPoint(ssql, log);
            }

            if (endCables.Length > 0)
            {
                string ssql = "SELECT A.G3E_FID, A.EFFECTIVE_PAIRS, A.TOTAL_SIZE, B.G3E_FID TERM_FID, B.G3E_FNO TERM_FNO, '0' TERM_NO, B.SPLICE_CLASS TERM_TYPE, D.FEATURE_STATE, ";
                ssql += "A.COUNT_ANNOTATION, ";
                ssql += "(SELECT G3E_DETAILID FROM DGC_CBL_L WHERE G3E_FID = A.G3E_FID) G3E_DETAILID ";
                ssql += "FROM GC_CBL A, GC_SPLICE B, GC_NR_CONNECT C, GC_NETELEM D ";
                ssql += "WHERE A.G3E_FID = C.G3E_FID AND A.G3E_FID = D.G3E_FID AND C.OUT_FID = B.G3E_FID ";
                ssql += "AND A.CABLE_CODE = '" + cablecode + "' AND B.G3E_FID IN (" + endCables + ") ORDER BY TERM_NO";
                GetTermPoint(ssql, log);
            }
        }

        private void GetTermPoint(string ssql, Logger log)
        {
            log.WriteLog("Reading Database : " + ssql);
            ADODB.Recordset rs = new ADODB.Recordset();
            rs = GTBalanceQuantum.m_gtapp.DataContext.OpenRecordset(ssql,
                     ADODB.CursorTypeEnum.adOpenStatic, ADODB.LockTypeEnum.adLockOptimistic, 1);

            if (rs.RecordCount > 0)
            {
                List<int> termFIDs = new List<int>(); // avoid duplicate termination point
                rs.MoveFirst();
                while (!rs.EOF)
                {
                    try
                    {
                        string term_type = myUtil.rsField(rs, "TERM_TYPE").Trim();
                        if (term_type.IndexOf("D-SIDE") > -1) term_type = term_type.Remove(term_type.Length - 6).Trim();

                        log.WriteLog("Reading Recordset : " + term_type + " " + myUtil.rsField(rs, "TERM_NO") + " [FID:" + myUtil.rsField(rs, "TERM_FID") + "]");
                        PairCount term = new PairCount(
                            myUtil.rsField(rs, "TERM_NO"),
                            myUtil.ParseInt(myUtil.rsField(rs, "TERM_FID")),
                            short.Parse(myUtil.rsField(rs, "TERM_FNO")),
                            term_type,
                            myUtil.ParseInt(myUtil.rsField(rs, "G3E_FID")),
                            myUtil.ParseInt(myUtil.rsField(rs, "EFFECTIVE_PAIRS")),
                            myUtil.ParseInt(myUtil.rsField(rs, "TOTAL_SIZE")),
                            myUtil.rsField(rs, "FEATURE_STATE"),
                            myUtil.ParseInt(myUtil.rsField(rs, "G3E_DETAILID"))
                        );

                        // set cable pair count - edited : m.zam 2012-10-13
                        term.SetCount(myUtil.rsField(rs, "COUNT_ANNOTATION"));

                        if (!termFIDs.Contains(term.cblFID))
                        {
                            log.WriteLog("Insert TO Grid");
                            termFIDs.Add(term.cblFID);
                        }

                        rs.MoveNext();
                        Application.DoEvents();
                    }
                    catch (Exception ex)
                    {
                        log.WriteErr(ex);
                    }
                }
            }
            rs.Close();
        }


        #endregion

    
        #region macam-macam

        private string CellValue(object val)
        {
            try
            {
                if (val == null)
                    return "";
                else
                    return val.ToString().Trim();
            }
            catch { return ""; }
        }

        #region GetValue
        private string GetValue(string ssql, string keyname)
        {
            ADODB.Recordset rs = new ADODB.Recordset();
            rs = GTBalanceQuantum.m_gtapp.DataContext.OpenRecordset
                (ssql, ADODB.CursorTypeEnum.adOpenStatic, ADODB.LockTypeEnum.adLockOptimistic, 1);
            return (myUtil.rsField(rs, keyname));
        }

        private Dictionary<string, string> GetValues(string ssql, string[] keyname)
        {
            ADODB.Recordset rs = new ADODB.Recordset();
            rs = GTBalanceQuantum.m_gtapp.DataContext.OpenRecordset
                (ssql, ADODB.CursorTypeEnum.adOpenStatic, ADODB.LockTypeEnum.adLockOptimistic, 1);

            if (rs.EOF) return null;

            Dictionary<string, string> val = new Dictionary<string, string>();
            for (int i = 0; i < keyname.Length; i++)
            {
                val.Add(keyname[i], myUtil.rsField(rs, keyname[i]));
            }
            return val;
        }
        #endregion



        #endregion

        #region Update 2012-11-21 : Unassigned Count

        private SortedDictionary<int, int> Unassigned = new SortedDictionary<int, int>();
        
        public string AvailableCount()
        {
            GetAvailableCount();

            if (Unassigned.Count == 0)
                return "No available count";
            else
            {
                string count = "Low-High";
                foreach (int l in Unassigned.Keys)
                {
                    count += Environment.NewLine + l.ToString() + "-" + Unassigned[l].ToString();
                }
                return count;
            }

        }
        
        private void GetAvailableCount()
        {
            Unassigned = new SortedDictionary<int, int>();
            Unassigned.Add(max_lo, max_hi);
            foreach (PairCount t in TermPoints.Values)
            {
                if (!t.overlapped && !t.unbalanced)
                    for (int i = 0; i < t.CID; i++)
                        AssignCount(t.Lo(i), t.Hi(i));
            }
        }

        private void AssignCount(int lo, int hi)
        {
            bool flag = false;
            foreach (int l in Unassigned.Keys)
            {
                int h = Unassigned[l];
                if (l <= lo && hi <= h)
                {
                    if (hi < h)
                        Unassigned.Add(hi + 1, h);
                    if (l == lo)
                        Unassigned.Remove(l);
                    else //if (l < lo)
                        Unassigned[l] = lo - 1;

                    flag = true;
                    break;
                }
            }
        }
        #endregion
    }
}
