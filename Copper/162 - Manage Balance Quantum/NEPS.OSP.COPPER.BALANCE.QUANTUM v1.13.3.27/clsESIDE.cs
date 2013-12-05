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
    class clsESIDE
    {
        #region Declaration
        public struct TAIL
        {
            public int tailFID;
            public int vertFID;
            public string vertNo;
            public int vertHi;
            public int vertLo;
            public string mdfNo;

            public TAIL(int tail_fid, int vert_fid, string mdf_no, string vert_no, int vert_hi, int vert_lo)
            {
                tailFID = tail_fid;
                vertFID = vert_fid;
                mdfNo = mdf_no;
                vertNo = vert_no;
                vertHi = vert_hi;
                vertLo = vert_lo;
            }

            public bool setMDF(int mdf_lo, int mdf_hi)
            {
                if ((mdf_lo == 0) && (mdf_hi == 0))
                    return true;
                else if (mdf_lo > mdf_hi)
                    return false;
                else
                    return ((mdf_lo >= vertLo) && (mdf_hi <= vertHi));
            }
        }
        public Dictionary<string, TAIL> tails = new Dictionary<string, TAIL>();

        public struct TERMPOINT
        {
            public string termNo;
            public int termFID;
            public short termFNO;
            public string termType;
            public int cblFID;
            public int cblCID;
            public string featureState;
            public int cableSize;
            public int efctvPair;

            public int detailID;
            public IGTPoint cblLoc;
            public IGTPoint termLoc;


            public TERMPOINT(string termno, int termfid, short termfno, string termtype, int cblfid, int efctv, int cblsize, string featstate,
                IGTPoint cblloc, IGTPoint termloc, int detailid)
            {
                termNo = termno;
                termFID = termfid;
                termFNO = termfno;
                termType = termtype;
                cblFID = cblfid;
                cblCID = 0;
                efctvPair = efctv;
                cableSize = cblsize;
                featureState = featstate;

                detailID = detailid;
                cblLoc = cblloc;
                termLoc = termloc;
            }

            public bool setPair(int pairlo, int pairhi)
            {
                if ((pairlo == 0) && (pairhi == 0))
                    return true;
                else
                    return ((pairhi - pairlo) < efctvPair);
            }
        }
        public SortedDictionary<int, TERMPOINT> termpoints = new SortedDictionary<int, TERMPOINT>();

        public struct ESIDECount
        {
            public int cbl_FID;
            public int cbl_CID;
            public int term_FID;
            public int pair_lo;
            public int pair_hi;
            public int mdf_lo;
            public int mdf_hi;
            public string mdf_no;
            public string vert_no;
            public string key;
            public bool overlapped;
            public bool unbalanced;

            public ESIDECount(int tp_fid, int cbl_fid, int cid, string mdf, string vert)
            {
                term_FID = tp_fid;
                cbl_FID = cbl_fid;
                cbl_CID = cid;
                mdf_no = mdf;
                vert_no = vert;
                key = mdf_no + "/" + vert_no;
                mdf_lo = 0;
                mdf_hi = 0;
                pair_lo = 0;
                pair_hi = 0;
                overlapped = false;
                unbalanced = false;
            }

            public bool SetCount(string mdf_no, string vert_no, int mdf_lo, int mdf_hi, int pair_lo, int pair_hi)
            {
                this.mdf_no = mdf_no;
                this.vert_no = vert_no;
                this.key = mdf_no + "/" + vert_no;
                this.mdf_hi = mdf_hi;
                this.mdf_lo = mdf_lo;
                this.pair_hi = pair_hi;
                this.pair_lo = pair_lo;

                if ((mdf_lo > mdf_hi) || (pair_lo > pair_hi) || (mdf_hi - mdf_lo != pair_hi - pair_lo))
                    this.unbalanced = true;
                else
                    this.unbalanced = false;

                return !this.unbalanced;
            }

            public bool isMDFOverlapped(ESIDECount c)
            {
                if (c.key != this.key)
                    return false;
                else if (c.mdf_lo == 0 && c.mdf_hi == 0)
                    return false;
                else if ((c.mdf_lo < this.mdf_lo && c.mdf_hi < this.mdf_lo) || (c.mdf_lo > this.mdf_hi && c.mdf_hi > this.mdf_hi))
                    return false;
                else
                {
                    overlapped = true;
                    return true;
                }
            }

            public bool isPairOverlapped(ESIDECount c)
            {
                if (c.term_FID != this.term_FID)
                    return false;
                else if (c.pair_lo == 0 && c.pair_hi == 0)
                    return false;
                else if (c.key != this.key)
                    return false;
                else if ((c.pair_lo < this.pair_lo && c.pair_hi < this.pair_lo) || (c.pair_lo > this.pair_hi && c.pair_hi > this.pair_hi))
                    return false;
                else
                    overlapped = true;

                return true;
            }

            public bool isUnbalance(int balance_mdfcount, int balance_effectivepair)
            {
                if (balance_mdfcount < 0)
                    unbalanced = true;
                else if (balance_effectivepair < 0)
                    unbalanced = true;
                else
                    unbalanced = false;

                return unbalanced;
            }
        }
        public Dictionary<int, ESIDECount> eCounts = new Dictionary<int, ESIDECount>();

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

        public DataGridView grdPairCount = new DataGridView();

        private string mEXC_ABB;
        private string mCABLE_CODE;

        public clsESIDE(DataGridView grd, string excabb, string cablecode)
        {
            grdPairCount = grd;
            mEXC_ABB = excabb;
            mCABLE_CODE = cablecode;
        }
        #endregion

        #region Editing
        public void ApplyCount(DataGridView dgv, DataGridViewRow row, int term_fid, int cbl_fid, string mdf_no, string vert_no, int mdf_lo, int mdf_hi, int pair_lo, int pair_hi)
        {
            string key = mdf_no + "/" + vert_no;
            int index = (int)row.Tag;
            TAIL tl = tails[key];
            TERMPOINT tp = termpoints[cbl_fid];
            ESIDECount c;

            #region Apply Count and Test Unbalance
            if (eCounts.ContainsKey(index))
            {
                c = eCounts[index];
            }
            else
            {
                tp.cblCID += 1;
                c = new ESIDECount(term_fid, cbl_fid, tp.cblCID, mdf_no, vert_no);
                eCounts.Add(index, c);
            }

            c.SetCount(mdf_no, vert_no, mdf_lo, mdf_hi, pair_lo, pair_hi);
            if (!tl.setMDF(mdf_lo, mdf_hi)) c.unbalanced = true;
            if (!tp.setPair(pair_lo, pair_hi)) c.unbalanced = true;
            #endregion

            if (!c.unbalanced)
            {
                #region Test Count Overlapping
                bool overlap_flag = false;
                //for (int r = 0; r < dgv.Rows.Count - 1; r++)
                foreach (DataGridViewRow r in dgv.Rows)
                {
                    if (r.Tag == null) continue;
                    int ind = (int)r.Tag;
                    if (ind != index)
                    {
                        ESIDECount e = eCounts[ind];
                        Overlapped o = new Overlapped(ind, index);
                        #region If Count Is Overlapping
                        if (c.isPairOverlapped(e) || c.isMDFOverlapped(e))
                        {
                            overlap_flag = true;
                            if (!CountOverlap.Contains(o))
                            {
                                e.overlapped = true;
                                eCounts[ind] = e;
                                CountOverlap.Add(o);
                            }
                        }
                        #endregion

                        #region If Count Not Overlapping
                        else if (CountOverlap.Contains(o))
                        {
                            // test previously overlap with edit - clear test overlap status
                            c.overlapped = overlap_flag;
                            CountOverlap.Remove(o);

                            bool testoverlap = false;
                            for (int j = 0; j < CountOverlap.Count; j++)
                            {
                                if (CountOverlap[j].row1 == ind ||
                                    CountOverlap[j].row2 == ind)
                                {
                                    testoverlap = true;
                                    break;
                                }
                            }
                            if (!testoverlap)
                            {
                                e.overlapped = false;
                                eCounts[ind] = e;
                            }
                        }
                        #endregion
                    }
                }
                #endregion
            }
            eCounts[index] = c;
            tails[key] = tl;
            termpoints[cbl_fid] = tp;
        }


        #endregion

    } // clsESIDE
}
