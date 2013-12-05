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

namespace NEPS.OSP.COPPER.PAIRCOUNT
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
            public bool isreadonly;

            public IGTPoint cblLoc;
            public IGTPoint termLoc;

            List<int> lo;
            List<int> hi;
            public int CID;

            public PairCount(string termno, int termfid, short termfno, string termtype, int cblfid, int efctv, int cblsize, string featstate,
                IGTPoint cblloc, IGTPoint termloc, int detailid)
            {
                termNo = termno;
                termFID = termfid;
                termFNO = termfno;
                termType = termtype;
                cblFID = cblfid;
                efctvPair = efctv;
                cableSize = cblsize;
                featureState = featstate;

                cblLoc = cblloc;
                termLoc = termloc;
                detailID = detailid;

                lo = new List<int>();
                hi = new List<int>();
                CID = 0;

                overlapped = false;
                unbalanced = false;
                isreadonly = false;
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
                if (anno.IndexOf("ORIG") > -1 || anno.IndexOf("CC") > -1)
                {
                    isreadonly = true;
                }

                int cid = 0;
                bool flag = true;
                for (int i = anno.IndexOf(Environment.NewLine); i > -1; i = anno.IndexOf(Environment.NewLine, cid++))
                {
                    string[] c = anno.Remove(i).Split('-');

                    #region Collect Count Annotation 2013-01-11 : handle collect count label (ORIG + CC)
                    int ori = c[0].IndexOf("(ORIG)");
                    int cc = c[0].IndexOf("(CC)");    
                    if (ori > -1) break; // original count before collect count was performed
                    if (cc > -1) c[0] = c[0].Substring(5); // remove CC label from annotation
                    #endregion

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

        public DataGridView grdPairCount = new DataGridView();

        public clsDSIDE(DataGridView grd)
        {
            grdPairCount = grd;
        }

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
            grdPairCount.Rows.Clear();

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
            grdPairCount.Rows.Clear();
            // FNO : 13000 = DP; 
            if (DPCables.Length > 0)
            {
                string ssql = "SELECT A.G3E_FID, A.EFFECTIVE_PAIRS, A.TOTAL_SIZE, B.G3E_FID TERM_FID, B.G3E_FNO TERM_FNO, B.DP_NUM TERM_NO, 'DP' TERM_TYPE, D.FEATURE_STATE, ";
                //                ssql += "(SELECT CURRENT_LOW||'-'||CURRENT_HIGH FROM GC_COUNT WHERE G3E_FID = A.G3E_FID AND ROWNUM = 1) ANNOTATION, ";
                ssql += "A.COUNT_ANNOTATION, ";
                ssql += "(SELECT GC_OSP_COP_VAL.EXTRACT_LINE_TO_VECTOR(G3E_GEOMETRY) FROM GC_DP_S WHERE G3E_FID = B.G3E_FID) TERM_GEO_XY, ";
                ssql += "(SELECT GC_OSP_COP_VAL.EXTRACT_LINE_TO_VECTOR(G3E_GEOMETRY) FROM DGC_DP_S WHERE G3E_FID = B.G3E_FID) TERM_DET_XY, ";
                ssql += "(SELECT GC_OSP_COP_VAL.EXTRACT_LINE_TO_VECTOR(G3E_GEOMETRY) FROM GC_CBL_L WHERE G3E_FID = A.G3E_FID) CBL_GEO_XY, ";
                ssql += "(SELECT GC_OSP_COP_VAL.EXTRACT_LINE_TO_VECTOR(G3E_GEOMETRY) FROM DGC_CBL_L WHERE G3E_FID = A.G3E_FID) CBL_DET_XY, ";
                ssql += "(SELECT G3E_DETAILID FROM DGC_CBL_L WHERE G3E_FID = A.G3E_FID) G3E_DETAILID ";
                ssql += "FROM GC_CBL A, GC_DP B, GC_NR_CONNECT C, GC_NETELEM D ";
                ssql += "WHERE A.G3E_FID = C.G3E_FID AND A.G3E_FID = D.G3E_FID AND C.OUT_FID = B.G3E_FID ";
                ssql += "AND A.CABLE_CODE = '" + cablecode + "' AND B.G3E_FID IN (" + DPCables + ") ORDER BY TERM_NO";
                GetTermPoint(ssql, log);
            }

            if (endCables.Length > 0)
            {
                string ssql = "SELECT A.G3E_FID, A.EFFECTIVE_PAIRS, A.TOTAL_SIZE, B.G3E_FID TERM_FID, B.G3E_FNO TERM_FNO, '0' TERM_NO, B.SPLICE_CLASS TERM_TYPE, D.FEATURE_STATE, ";
                //                ssql += "(SELECT CURRENT_LOW||'-'||CURRENT_HIGH FROM GC_COUNT WHERE G3E_FID = A.G3E_FID AND ROWNUM = 1) ANNOTATION, ";
                ssql += "A.COUNT_ANNOTATION, ";
                ssql += "(SELECT GC_OSP_COP_VAL.EXTRACT_LINE_TO_VECTOR(G3E_GEOMETRY) FROM GC_SPLICE_S WHERE G3E_FID = B.G3E_FID) TERM_GEO_XY, ";
                ssql += "(SELECT GC_OSP_COP_VAL.EXTRACT_LINE_TO_VECTOR(G3E_GEOMETRY) FROM DGC_SPLICE_S WHERE G3E_FID = B.G3E_FID) TERM_DET_XY, ";
                ssql += "(SELECT GC_OSP_COP_VAL.EXTRACT_LINE_TO_VECTOR(G3E_GEOMETRY) FROM GC_CBL_L WHERE G3E_FID = A.G3E_FID) CBL_GEO_XY, ";
                ssql += "(SELECT GC_OSP_COP_VAL.EXTRACT_LINE_TO_VECTOR(G3E_GEOMETRY) FROM DGC_CBL_L WHERE G3E_FID = A.G3E_FID) CBL_DET_XY, ";
                ssql += "(SELECT G3E_DETAILID FROM DGC_CBL_L WHERE G3E_FID = A.G3E_FID) G3E_DETAILID ";
                ssql += "FROM GC_CBL A, GC_SPLICE B, GC_NR_CONNECT C, GC_NETELEM D ";
                ssql += "WHERE A.G3E_FID = C.G3E_FID AND A.G3E_FID = D.G3E_FID AND C.OUT_FID = B.G3E_FID ";
                ssql += "AND A.CABLE_CODE = '" + cablecode + "' AND B.G3E_FID IN (" + endCables + ") ORDER BY TERM_NO";
                System.Diagnostics.Debug.WriteLine("ssql : " + ssql);
                System.Diagnostics.Debug.WriteLine("Next");
                GetTermPoint(ssql, log);
            }
        }

        private void GetTermPoint(string ssql, Logger log)
        {
            int i = 0;
            log.WriteLog("Reading Database : " + ssql);
            ADODB.Recordset rs = new ADODB.Recordset();
            rs = GTPairCount.m_gtapp.DataContext.OpenRecordset(ssql,
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

                        //IGTPoint cblLoc;
                        //IGTPoint termLoc;

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
                            GetGeoPoint(myUtil.rsField(rs, "TERM_GEO_XY"), myUtil.rsField(rs, "TERM_DET_XY"), true),
                            GetGeoPoint(myUtil.rsField(rs, "CBL_GEO_XY"), myUtil.rsField(rs, "CBL_DET_XY"), false),
                            myUtil.ParseInt(myUtil.rsField(rs, "G3E_DETAILID"))
                        );

                        // set cable pair count - edited : m.zam 2012-10-13
                        term.SetCount(myUtil.rsField(rs, "COUNT_ANNOTATION"));

                        if (!termFIDs.Contains(term.cblFID))
                        {
                            log.WriteLog("Insert TO Grid");
                            InsertToGrid(term);
                            termFIDs.Add(term.cblFID);
                        }

                        
                        Application.DoEvents();
                    }
                    catch (Exception ex)
                    {
                        log.WriteErr(ex);
                    }

                    rs.MoveNext();
                }
            }
            rs.Close();
        }

        private IGTPoint GetGeoPoint(string geo, string det, bool firstpoint)
        {
            IGTPoint geopoint = GTClassFactory.Create<IGTPoint>();
            try
            {
                string vector;
                if (GTClassFactory.Create<IGTApplication>().ActiveMapWindow.DetailID > 0)
                    vector = det;
                else
                    vector = geo;

                string[] points = vector.Split('|');
                if (firstpoint)
                {
                    geopoint.X = double.Parse(points[0]);
                    geopoint.Y = double.Parse(points[1]);
                }
                else
                {
                    geopoint.X = double.Parse(points[points.Length - 2]);
                    geopoint.Y = double.Parse(points[points.Length - 1]);
                }
            }
            catch
            {
                geopoint.X = 0; geopoint.Y = 0;
            }
            return geopoint;
        }

        private void InsertToGrid(PairCount t)
        {
            int i = grdPairCount.Rows.Count - 1;
            for (int CID = 0; CID < t.CID; CID++, i++)
            {
                grdPairCount.Rows.Add();
                grdPairCount.Rows[i].Cells[0].Value = t.termType;
                grdPairCount.Rows[i].Cells[1].Value = t.termNo;
                grdPairCount.Rows[i].Cells[2].Value = t.Lo(CID);
                grdPairCount.Rows[i].Cells[3].Value = t.Hi(CID);
                grdPairCount.Rows[i].Cells[4].Value = t.efctvPair;
                grdPairCount.Rows[i].Cells[5].Value = t.cableSize;
                grdPairCount.Rows[i].Cells[6].Value = t.featureState;
                grdPairCount.Rows[i].Cells[7].Value = t.cblFID;
                grdPairCount.Rows[i].Cells[8].Value = t.termFID;
                grdPairCount.Rows[i].Cells[9].Value = CID + 1;
                grdPairCount.Rows[i].Tag = i;
                grdPairCount.Rows[i].ReadOnly = t.isreadonly;

                SetRowStatus(grdPairCount.Rows[i], t);
                if (CID == 0) TermPoints.Add(t.cblFID, t);
            }
        }

        #endregion

        #region Editing
        public void ApplyDPNumber(int key, string val)
        {
            PairCount edit = new PairCount();
            if (TermPoints.ContainsKey(key))
            {
                edit = TermPoints[key];
                edit.termNo = val;
                TermPoints[key] = edit;
            }
        }
        public void ApplyEffectivePair(int key, string val)
        {
            PairCount edit = new PairCount();
            if (TermPoints.ContainsKey(key))
            {
                edit = TermPoints[key];
                edit.efctvPair = myUtil.ParseInt(val);
                TermPoints[key] = edit;
            }
        }

        public void ApplyCount(DataGridView dgv, int cFID, DataGridViewRow r)
        {
            PairCount edit = new PairCount();
            if (TermPoints.ContainsKey(cFID))
            {
                // add lo,hi to termination point & check for count balance
                edit = TermPoints[cFID];
                int edit_lo = int.Parse(r.Cells[2].Value.ToString());
                int edit_hi = int.Parse(r.Cells[3].Value.ToString());
                int cid = int.Parse(r.Cells[9].Value.ToString()) - 1;
                if (edit.SetCount(cid, edit_lo, edit_hi))
                {
                    #region Test Overlapping Count
                    bool overlap_flag = false;
                    for (int row = 0; row < dgv.Rows.Count - 1; row++)
                    {
                        int i = (int)dgv.Rows[row].Tag;
                        int k = (int)r.Tag;
                        int iFID = int.Parse(dgv.Rows[row].Cells[7].Value.ToString()); // read cable FID of row[i]
                        if (k != i)
                        {
                            PairCount test = TermPoints[iFID];
                            if (!test.unbalanced)
                            {
                                Overlapped o = new Overlapped(k, i);

                                #region If Count Is Overlapped
                                int cell_lo = int.Parse(dgv.Rows[row].Cells["colDLo"].Value.ToString());
                                int cell_hi = int.Parse(dgv.Rows[row].Cells["colDHi"].Value.ToString());
                                if (edit.isOverlapped(cid, cell_lo, cell_hi))
                                {
                                    overlap_flag = true;
                                    if (!CountOverlap.Contains(o))
                                    {
                                        test.overlapped = true;
                                        TermPoints[iFID] = test;
                                        SetRowStatus(dgv.Rows[row], test);
                                        CountOverlap.Add(o);
                                    }
                                }
                                #endregion

                                #region If Count Not Overlapped
                                // test previously overlap with edit - clear test overlap status
                                else if (CountOverlap.Contains(o))
                                {
                                    edit.overlapped = overlap_flag;
                                    CountOverlap.Remove(o);

                                    bool testoverlap = false;
                                    for (int t = 0; t < CountOverlap.Count; t++)
                                    {
                                        if (CountOverlap[t].row1 == i ||
                                            CountOverlap[t].row2 == i)
                                        {
                                            testoverlap = true;
                                            break;
                                        }
                                    }
                                    if (!testoverlap)
                                    {
                                        test.overlapped = testoverlap;
                                        TermPoints[iFID] = test;
                                        SetRowStatus(dgv.Rows[row], test);
                                    }
                                }
                                #endregion
                            }
                        }
                    }
                    #endregion
                }
                TermPoints[cFID] = edit;
                SetRowStatus(r, edit);

            }
        }

        private void SetRowStatus(DataGridViewRow row, PairCount val)
        {
            if (val.isreadonly)
            {
                //SetColor(row, Color.LightGray, Color.White);
                for (int c = 0; c < row.Cells.Count; c++)
                {
                    row.Cells[c].Style.BackColor = Color.White;
                    row.Cells[c].Style.ForeColor = Color.Gray;
                }
            }
                
            else if (val.unbalanced)
                SetColor(row, Color.Red, Color.Yellow);
            else if (val.overlapped)
                SetColor(row, Color.Yellow, Color.Red);
            else
                SetColor(row, Color.White, Color.Black);
        }
        private void SetColor(DataGridViewRow row, Color bc, Color fc)
        {
            row.Cells["colDLo"].Style.BackColor = bc;
            row.Cells["colDLo"].Style.ForeColor = fc;
            row.Cells["colDHi"].Style.BackColor = bc;
            row.Cells["colDHi"].Style.ForeColor = fc;
        }
        #endregion

        #region Apply Setting
        public bool ApplySetting(DataGridView grdD, string cablecode, bool placement, bool all)
        {
            GTPairCount.m_CustomForm.InitDProgressBar(TermPoints.Count * 2);
            GTPairCount.m_oIGTTransactionManager.Begin("Count");
            GTPairCount.m_gtapp.BeginWaitCursor();
            bool flag = false;
            try
            {
                if (all)
                    foreach (DataGridViewRow row in grdD.Rows)
                    {
                        if (!row.ReadOnly && row.Tag != null && (int)row.Cells[9].Value == 1)
                        {
                            PairCount term = TermPoints[(int)row.Cells[7].Value]; // key is cable FID

                            SavePairCount(term, cablecode, placement);
                            GTPairCount.m_CustomForm.IncreaseDProgressBar(1);

                            SaveDPNumber(term.termFID, term.termNo);
                            GTPairCount.m_CustomForm.IncreaseDProgressBar(1);
                        }
                    }
                else
                    foreach (DataGridViewRow row in grdD.SelectedRows)
                    {
                        if (!row.ReadOnly && row.Tag != null && (int)row.Cells[9].Value == 1)
                        {
                            PairCount term = TermPoints[(int)row.Cells[7].Value];
                            SavePairCount(term, cablecode, placement);
                            GTPairCount.m_CustomForm.IncreaseDProgressBar(1);

                            SaveDPNumber(term.termFID, term.termNo);
                            GTPairCount.m_CustomForm.IncreaseDProgressBar(1);
                        }
                    }

                GTPairCount.m_oIGTTransactionManager.Commit();
                flag = true;
            }
            catch (Exception ex)
            {
                GTPairCount.m_oIGTTransactionManager.Rollback();
                GTPairCount.m_CustomForm.TopMost = false;
                MessageBox.Show("Fail saving record to NEPS\r\n" + ex.Message);
            }
            finally
            {
                GTPairCount.m_oIGTTransactionManager.RefreshDatabaseChanges();
                GTPairCount.m_gtapp.EndWaitCursor();
                GTPairCount.m_CustomForm.IncreaseDProgressBar(-1);
            }

            return flag;

        }

        private void SavePairCount(PairCount term, string cablecode, bool placement)
        {

            if (term.CID > 0)
            {
                string anno = "";
                ADODB.Recordset rs;
                IGTKeyObject oFeature = null;

                for (int cid = 0; cid < term.CID; cid++)
                {
                    oFeature = GTClassFactory.Create<IGTApplication>().DataContext.OpenFeature(7000, term.cblFID);
                    rs = oFeature.Components.GetComponent(55).Recordset; // 55 is CNO for GC_COUNT

                    if (rs.EOF)
                    {
                        rs.AddNew("G3E_FID", term.cblFID);
                        rs.Update("G3E_FNO", 7000);
                        rs.Update("G3E_CID", 1);
                    }
                    else
                    {
                        rs.MoveLast();
                    }
                    rs.Update("CLASS", "Pair Count");
                    rs.Update("CURRENT_DESIGNATION", cablecode);
                    rs.Update("PROPOSED_DESIGNATION", cablecode);
                    rs.Update("PROPOSED_FEED_DIR", "F");
                    rs.Update("PROPOSED_HIGH", term.Hi(cid));
                    rs.Update("PROPOSED_LOW", term.Lo(cid));
                    rs.Update("CURRENT_FEED_DIR", "F");
                    rs.Update("CURRENT_HIGH", term.Hi(cid));
                    rs.Update("CURRENT_LOW", term.Lo(cid));
                    rs.Update("SEQ", 1);

                    if (term.Lo(cid) != 0 && term.Hi(cid) != 0)
                    {
                        if (anno.Length > 0) anno += Environment.NewLine;
                        anno += term.Lo(cid).ToString() + "-" + term.Hi(cid).ToString();
                        rs.Update("COUNT_ANNOTATION", term.Lo(cid).ToString() + "-" + term.Hi(cid).ToString());
                    }
                    else
                        rs.Update("COUNT_ANNOTATION", "");

                }

                if (placement)
                {
                    #region AUTO PLACEMENT OF COUNT ANNOTATION
                    IGTTextPointGeometry oCableTextTL;
                    oCableTextTL = GTClassFactory.Create<IGTTextPointGeometry>();

                    IGTPoint oPntTextTL = GTClassFactory.Create<IGTPoint>();

                    if (Math.Round(term.termLoc.Y) == Math.Round(term.cblLoc.Y))
                    {
                        oPntTextTL.Y = term.cblLoc.Y;
                        if (term.termLoc.X == term.cblLoc.X)
                            oPntTextTL.X = term.cblLoc.X + 1;
                        else if (term.termLoc.X > term.cblLoc.X)
                            oPntTextTL.X = term.cblLoc.X - 5; // notes : 5 if without cable code
                        else if (term.termLoc.X < term.cblLoc.X)
                            oPntTextTL.X = term.cblLoc.X + 3;
                    }
                    else
                    {
                        oPntTextTL.X = term.cblLoc.X - 2;
                        if (term.termLoc.Y > term.cblLoc.Y)
                            oPntTextTL.Y = term.cblLoc.Y - 3;
                        else if (term.termLoc.Y < term.cblLoc.Y)
                            oPntTextTL.Y = term.cblLoc.Y + 5;
                    }


                    oCableTextTL.Origin = oPntTextTL;
                    oCableTextTL.Rotation = 0;

                    // notes : gc_cblcnt_tl_t = 7034 : dgc_cblcnt_tl_t = 7035 : gc_cblcnt_bl_t = 7032
                    short CNO = (short)(term.detailID > 0 ? 7035 : 7034);

                    rs = oFeature.Components.GetComponent(CNO).Recordset;
                    if (rs.EOF)
                    {
                        rs.AddNew("G3E_FID", term.cblFID);
                        rs.Update("G3E_FNO", 7000);
                        rs.Update("G3E_CID", 1);
                    }
                    else
                        rs.MoveLast();

                    oFeature.Components.GetComponent(CNO).Geometry = oCableTextTL;
                    if (term.detailID > 0)
                        oFeature.Components.GetComponent(CNO).Recordset.Update("G3E_DETAILID", term.detailID);
                    #endregion
                }

                rs = oFeature.Components.GetComponent(7001).Recordset;
                rs.Update("COUNT_ANNOTATION", anno);
                rs.Update("EFFECTIVE_PAIRS", term.efctvPair.ToString());
            }
            else
            {
                IGTKeyObject oFeature;
                oFeature = GTClassFactory.Create<IGTApplication>().DataContext.OpenFeature(7000, term.cblFID);
                ADODB.Recordset rs = oFeature.Components.GetComponent(7001).Recordset;
                rs.Update("COUNT_ANNOTATION", "");
                rs.Update("EFFECTIVE_PAIRS", term.efctvPair.ToString());
            }


        }

        private void SaveDPNumber(int dpFID, string DPNumber)
        {
            string ssql = "UPDATE GC_DP SET DP_NUM = '" + DPNumber + "' WHERE G3E_FID = " + dpFID.ToString();
            int iR;
            GTClassFactory.Create<IGTApplication>().DataContext.Execute(ssql, out iR, (int)ADODB.CommandTypeEnum.adCmdText);
            GTClassFactory.Create<IGTApplication>().DataContext.Execute("COMMIT", out iR, (int)ADODB.CommandTypeEnum.adCmdText);
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
            rs = GTPairCount.m_gtapp.DataContext.OpenRecordset
                (ssql, ADODB.CursorTypeEnum.adOpenStatic, ADODB.LockTypeEnum.adLockOptimistic, 1);
            return (myUtil.rsField(rs, keyname));
        }

        private Dictionary<string, string> GetValues(string ssql, string[] keyname)
        {
            ADODB.Recordset rs = new ADODB.Recordset();
            rs = GTPairCount.m_gtapp.DataContext.OpenRecordset
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
