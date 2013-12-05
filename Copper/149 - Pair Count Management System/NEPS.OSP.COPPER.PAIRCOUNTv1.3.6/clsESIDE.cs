using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using Intergraph.GTechnology.Interfaces;
using Intergraph.GTechnology.API;
using ADODB;

namespace NEPS.OSP.COPPER.PAIRCOUNT
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
                                SetRowStatus(r, e);
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
                                SetRowStatus(r, e);
                            }
                        }
                        #endregion
                    }
                }
                #endregion
            }
            SetRowStatus(row, c);
            eCounts[index] = c;
            tails[key] = tl;
            termpoints[cbl_fid] = tp;
        }

        private void SetRowStatus(DataGridViewRow row, ESIDECount val)
        {
            try
            {
                if (val.unbalanced)
                    SetRowColor(row, Color.Red, Color.Yellow);
                else if (val.overlapped)
                    SetRowColor(row, Color.Yellow, Color.Red);
                else
                    SetRowColor(row, Color.White, Color.Black);
            }
            catch { }
        }

        private void SetRowColor(DataGridViewRow row, Color bg, Color fg)
        {
            row.Cells[2].Style.BackColor = bg;
            row.Cells[2].Style.ForeColor = fg;
            row.Cells[3].Style.BackColor = bg;
            row.Cells[3].Style.ForeColor = fg;

            row.Cells[4].Style.BackColor = bg;
            row.Cells[4].Style.ForeColor = fg;
            row.Cells[5].Style.BackColor = bg;
            row.Cells[5].Style.ForeColor = fg;

            row.Cells[6].Style.BackColor = bg;
            row.Cells[6].Style.ForeColor = fg;
        }
        #endregion

        #region Save And Placement Pair Count Annotation
        Dictionary<int, string> annotations_lbl = new Dictionary<int, string>();
        Dictionary<int, int> annotations_cid = new Dictionary<int, int>();

        public bool SavePairCount(DataGridView dgv, string excAbb, string cblCode, bool placement, bool all)
        {
            bool flag = true;
            GTPairCount.m_gtapp.BeginWaitCursor();
            GTPairCount.m_oIGTTransactionManager.Begin("Count");
            annotations_lbl.Clear();
            annotations_cid.Clear();
            try
            {
                GTPairCount.m_CustomForm.InitEProgressBar(dgv.Rows.Count * 2);
                ClearDeleteCount();

                if (all)
                    foreach (DataGridViewRow row in dgv.Rows)
                        SaveCount(row, excAbb, cblCode, placement);
                else
                    foreach (DataGridViewRow row in dgv.SelectedRows)
                        SaveCount(row, excAbb, cblCode, placement);

                GTPairCount.m_oIGTTransactionManager.Commit();
                flag = true;
            }
            catch (Exception ex)
            {
                flag = false;
                GTPairCount.m_oIGTTransactionManager.Rollback();
                MessageBox.Show("Error saving record to database\r\n" + ex.Message);
            }
            finally
            {
                GTPairCount.m_oIGTTransactionManager.RefreshDatabaseChanges();
                GTPairCount.m_CustomForm.IncreaseEProgressBar(-1);
                GTPairCount.m_gtapp.EndWaitCursor();
            }
            return flag;

        }

        private void SaveCount(DataGridViewRow row, string excAbb, string cblCode, bool placement)
        {
            ADODB.Recordset rs;
            if (row.Tag != null)
            {
                IGTKeyObject oFeature;
                ESIDECount cnt = eCounts[(int)row.Tag];
                TERMPOINT tp = termpoints[cnt.cbl_FID];

                oFeature = GTClassFactory.Create<IGTApplication>().DataContext.OpenFeature(7000, cnt.cbl_FID);

                #region Open Recordset - GC_COUNT (CNO : 55)
                short iCNO = 55;
                rs = oFeature.Components.GetComponent(iCNO).Recordset;

                if (rs.EOF)
                {
                    rs.AddNew("G3E_FID", cnt.cbl_FID);
                    rs.Update("G3E_FNO", 7000);
                    rs.Update("G3E_CID", cnt.cbl_CID);
                }
                else
                {
                    rs.MoveFirst();
                    while (myUtil.rsField(rs, "G3E_CID") != cnt.cbl_CID.ToString())
                        rs.MoveNext();

                    if (myUtil.rsField(rs, "G3E_CID") != cnt.cbl_CID.ToString())
                    {
                        rs.AddNew("G3E_FID", cnt.cbl_FID);
                        rs.Update("G3E_FNO", 7000);
                        rs.Update("G3E_CID", cnt.cbl_CID);
                    }
                }
                #endregion

                #region Annotations
                int psg = (cnt.pair_hi == 0 ? 0 : (cnt.pair_hi - cnt.pair_lo) + 1);

                string anno = excAbb + "/" + myUtil.CellValue(row.Cells[1].Value) + "/"
                    + (cnt.mdf_lo == 0 ? "" : cnt.mdf_lo.ToString()) + "-"
                    + (cnt.mdf_hi == 0 ? "" : cnt.mdf_hi.ToString()) + "/"
                    + (cnt.pair_lo == 0 ? "" : cnt.pair_lo.ToString()) + "-"
                    + (cnt.pair_hi == 0 ? "" : cnt.pair_hi.ToString()) + "/"
                    + (psg == 0 ? "" : psg.ToString() + " psg");

                if (annotations_lbl.ContainsKey(cnt.cbl_FID))
                {
                    annotations_lbl[cnt.cbl_FID] += Environment.NewLine + anno;
                    annotations_cid[cnt.cbl_FID] += 1;
                }
                else
                {
                    annotations_lbl.Add(cnt.cbl_FID, anno);
                    annotations_cid.Add(cnt.cbl_FID, 0);
                }
                #endregion

                #region Update Recordset - GC_COUNT
                rs.Update("CLASS", "Pair Count");
                rs.Update("COUNT_ANNOTATION", anno);
                rs.Update("CURRENT_DESIGNATION", cblCode);
                rs.Update("PROPOSED_DESIGNATION", cblCode);
                rs.Update("PROPOSED_FEED_DIR", "F");
                rs.Update("PROPOSED_HIGH", cnt.pair_hi.ToString());
                rs.Update("PROPOSED_LOW", cnt.pair_lo.ToString());
                rs.Update("CURRENT_FEED_DIR", "F");
                rs.Update("CURRENT_HIGH", cnt.pair_hi.ToString());
                rs.Update("CURRENT_LOW", cnt.pair_lo.ToString());

                rs.Update("EHI_PR", cnt.pair_hi.ToString());
                rs.Update("ELO_PR", cnt.pair_lo.ToString());
                try
                {
                    rs.Update("HIGH_PORT", cnt.mdf_hi.ToString());
                    rs.Update("LOW_PORT", cnt.mdf_lo.ToString());
                }
                catch { }
                rs.Update("MDF_UNIT", cnt.mdf_no.ToString());
                rs.Update("VERTICAL_UNIT", cnt.vert_no.ToString());
                rs.Update("SEQ", cnt.cbl_CID);
                #endregion

                GTPairCount.m_CustomForm.IncreaseEProgressBar(1);

                #region Save to GC_CBL.COUNT_ANNOTATION
                iCNO = 7001;
                rs = oFeature.Components.GetComponent(iCNO).Recordset;
                rs.Update("COUNT_ANNOTATION", annotations_lbl[cnt.cbl_FID]);
                rs.Update("EFFECTIVE_PAIRS", termpoints[cnt.cbl_FID].efctvPair.ToString());
                #endregion

                #region Auto Placement
                if (placement)
                {
                    IGTTextPointGeometry oCableTextTL;
                    oCableTextTL = GTClassFactory.Create<IGTTextPointGeometry>();

                    IGTPoint oPntTextTL = GTClassFactory.Create<IGTPoint>();
                    int y = annotations_cid[cnt.cbl_FID] * 3;
                    if (tp.termLoc.X == tp.cblLoc.X)
                        oPntTextTL.X = tp.cblLoc.X + 1;
                    else if (tp.termLoc.X > tp.cblLoc.X)
                        oPntTextTL.X = tp.cblLoc.X - 30;
                    else if (tp.termLoc.X < tp.cblLoc.X)
                        oPntTextTL.X = tp.cblLoc.X + 5;

                    if (Math.Round(tp.termLoc.Y) == Math.Round(tp.cblLoc.Y))
                        oPntTextTL.Y = tp.cblLoc.Y + y;
                    else if (tp.termLoc.Y > tp.cblLoc.Y)
                        oPntTextTL.Y = tp.cblLoc.Y - 7 - y;
                    else if (tp.termLoc.Y < tp.cblLoc.Y)
                        oPntTextTL.Y = tp.cblLoc.Y + 7 + y;

                    oCableTextTL.Origin = oPntTextTL;
                    oCableTextTL.Rotation = 0;

                    iCNO = (short)(tp.detailID > 0 ? 7035 : 7034); // gc_cblcnt_tl_t  7032 : gc_cblcnt_bl_t 
                    rs = oFeature.Components.GetComponent(iCNO).Recordset;
                    if (rs.EOF)
                    {
                        rs.AddNew("G3E_FID", cnt.cbl_FID);
                        rs.Update("G3E_FNO", 7000);
                    }
                    else
                        rs.MoveLast();

                    rs.Update("G3E_CID", 1);
                    oFeature.Components.GetComponent(iCNO).Geometry = oCableTextTL;
                    if (tp.detailID > 0)
                        oFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_DETAILID", tp.detailID);
                }
                #endregion

                GTPairCount.m_CustomForm.IncreaseEProgressBar(1);


            }
        }
        #endregion

        #region Delete Count
        private List<int> DeletedCount = new List<int>();

        public void DeleteRow(DataGridViewRow row)
        {
            try
            {
                int ind = (int)row.Tag;

                if (eCounts.ContainsKey(ind))
                {
                    if (!DeletedCount.Contains(eCounts[ind].cbl_FID))
                        DeletedCount.Add(eCounts[ind].cbl_FID);

                    eCounts.Remove(ind);
                }
                for (int r = CountOverlap.Count - 1; r >= 0; r--)
                {
                    Overlapped o = CountOverlap[r];
                    if ((o.row1 == ind) || (o.row2 == ind))
                        CountOverlap.Remove(o);
                }
            }
            catch { }
        }

        private void ClearDeleteCount()
        {
            foreach (int FID in DeletedCount)
            {
                try
                {
                    IGTKeyObject oFeature = GTClassFactory.Create<IGTApplication>().DataContext.OpenFeature(7000, FID);

                    // clear count annotation in GC_CBL
                    ADODB.Recordset rs = oFeature.Components.GetComponent(7001).Recordset;
                    rs.Update("COUNT_ANNOTATION", "-");

                }
                catch { }
            }
        }

        #endregion

    } // clsESIDE
}
