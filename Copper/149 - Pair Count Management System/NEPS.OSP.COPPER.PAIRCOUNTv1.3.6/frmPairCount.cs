/*
 *  
 * 
 * 
 * 
 * edited  : m.zam @ 19-09-2012
 * issues  : include pair count for copper cable connected to FIBER MSAN/RT
 *           perform tracing up & down to include STUB & STUMP without itface_code
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Intergraph.GTechnology.API;
using Intergraph.GTechnology.Interfaces;
using System.Diagnostics;
using System.Text.RegularExpressions;
using ADODB;

namespace NEPS.OSP.COPPER.PAIRCOUNT
{
    public partial class frmPairCount : Form
    {
        private IGTApplication m_gtapp = null;
        private IGTDataContext m_IGTDataContext = null;
        private double initX;
        private double initY;

        private int isActiveFID_TERMINATE = 0;
        private int isActiveFID_CBL = 0;
        private int isRowIndex = 0;

        private bool isOnChange = false;
        private bool isOnLoadMode = false;


        private Logger log = Logger.getInstance();

        #region Form Pair Event
        public frmPairCount()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("a");
                InitializeComponent();
                sendFormToBottomLeft();

                string filename = "PAIRCOUNT";
                log = Logger.getInstance();
                log.OpenFile(filename);

                //this.FormClosing += new FormClosingEventHandler(this.frmPairCount_FormClosing);
                m_gtapp = GTClassFactory.Create<IGTApplication>().Application;
                m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Running Pair Count Management System...");

                // Check E-Side or D-Side
                short initFNO = 0;
                int initFID = 0;
                string itfaceCode = "";
                string cableCode = "";
                string cableClass = "";
                string excAbb = "";

                foreach (IGTDDCKeyObject oDDCKeyObject in GTClassFactory.Create<IGTApplication>().SelectedObjects.GetObjects())
                {
                    initFNO = oDDCKeyObject.FNO;
                    initFID = oDDCKeyObject.FID;
                    if (initFNO == 7000)
                    {
                        ADODB.Recordset rs = oDDCKeyObject.Recordset;

                        cableClass = rs.Fields["CABLE_CLASS"].Value.ToString();
                        itfaceCode = rs.Fields["ITFACE_CODE"].Value.ToString();
                        cableCode = rs.Fields["CABLE_CODE"].Value.ToString();
                        excAbb = rs.Fields["EXC_ABB"].Value.ToString();
                        break;
                    }
                }

                this.Visible = true;
                Cursor.Current = Cursors.WaitCursor;
                System.Windows.Forms.Application.DoEvents();

                if (cableClass.IndexOf("D-") == -1) // not a D-SIDE cable
                {
                    tabDSide.Dispose();
                    this.menuStrip2.Visible = true;
                    loadE(initFID, initFNO, cableCode, excAbb);
                }
                else
                {
                    tabESide.Dispose();
                    this.menuStrip1.Visible = true;
                    loadD(initFID, cableCode, excAbb);
                    this.grpDHeader.Height = 1;
                }
            }
            catch (Exception ex)
            {
                log.WriteErr(ex);
                MessageBox.Show(ex.Message, "Running Pair Count Management System", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            Cursor.Current = Cursors.Default; ;
        }

        private void sendFormToBottomLeft()
        {
            Rectangle r = Screen.PrimaryScreen.WorkingArea;
            this.StartPosition = FormStartPosition.Manual;
            this.Location = new Point(Screen.PrimaryScreen.WorkingArea.Width - this.Width - 50,
                Screen.PrimaryScreen.WorkingArea.Height - this.Height - 50);
        }

        private void frmPairCount_Shown(object sender, EventArgs e)
        {
            m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Running Pair Count Management System...");
        }

        private void frmPairCount_Activated(object sender, EventArgs e)
        {
            m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Running Pair Count Management System...");
        }

        private void frmPairCount_Load(object sender, EventArgs e)
        {
            this.m_IGTDataContext = GTClassFactory.Create<IGTApplication>().Application.DataContext;
            string filename = Application.StartupPath + "/NEPS.OSP.COPPER.PAIRCOUNT.dll";
            this.Text = "Manage Pair Count [" + System.Diagnostics.FileVersionInfo.GetVersionInfo(filename).FileVersion + "]";
        }

        private void hiliteFeature(int isFID, short isFNO, int detailID, int displayscale)
        {
            try
            {
                IGTDDCKeyObjects oGTKeyObjs;
                GTClassFactory.Create<IGTApplication>().SelectedObjects.Clear();
                if (detailID > 0)
                {
                    oGTKeyObjs = (IGTDDCKeyObjects)GTClassFactory.Create<IGTApplication>().DataContext.GetDDCKeyObjects(isFNO, isFID, GTComponentGeometryConstants.gtddcgAllDetail);
                }
                else
                {
                    oGTKeyObjs = (IGTDDCKeyObjects)GTClassFactory.Create<IGTApplication>().DataContext.GetDDCKeyObjects(isFNO, isFID, GTComponentGeometryConstants.gtddcgAllGeographic);
                }
                for (int i = 0; i < oGTKeyObjs.Count; i++)
                {
                    GTClassFactory.Create<IGTApplication>().SelectedObjects.Add(GTSelectModeConstants.gtsosmAllDisplayedComponentsInActiveLegend, oGTKeyObjs[i]);
                }

                GTClassFactory.Create<IGTApplication>().ActiveMapWindow.CenterSelectedObjects();
                GTClassFactory.Create<IGTApplication>().ActiveMapWindow.DisplayScale = displayscale;
                GTClassFactory.Create<IGTApplication>().RefreshWindows();
                oGTKeyObjs = null;

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Pair Count Management System", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region General
        public bool isNumeric(string val, System.Globalization.NumberStyles NumberStyle)
        {
            int result;
            return int.TryParse(val, NumberStyle, System.Globalization.CultureInfo.CurrentCulture, out result);
        }

        public double XPointGeom
        {
            set
            {
                initX = value;
            }
        }

        public double YPointGeom
        {
            set
            {
                initY = value;
            }
        }
        #endregion

        #region D-Side

        private void loadD(int cblFID, string cableCode, string excAbb)
        {
            lblDExc.Text = excAbb;
            lblDCableCode.Text = cableCode;

            try
            {
                Cursor.Current = Cursors.WaitCursor;

                D_SIDE = new clsDSIDE(grdD);
                isOnLoadMode = true;

                System.Windows.Forms.Application.DoEvents();


                #region Load Network
                int[] val = clsTrace.TraceUp_DGetStartPoint(cblFID);
                if (val == null)
                {
                    MessageBox.Show(this, "Unable to trace termination point for selected cable", "Pair Count : Error");
                    return;
                }
                // Load Network for D-Side and Cabinet FID
                ADODB.Recordset rs = new ADODB.Recordset();
                string strSQL = "SELECT CABLE_CODE||' : Effective Pairs '||EFFECTIVE_PAIRS AS CABLE_CODE FROM GC_CBL WHERE G3E_FID IN ";
                strSQL += "(SELECT B.G3E_FID FROM {0} A, GC_NR_CONNECT B WHERE A.G3E_FID = {1} AND B.IN_FID = A.G3E_FID)";
                strSQL += " ORDER BY CABLE_CODE";

                string cabSQL = "SELECT {1} FROM {0} WHERE G3E_FID = {2}";
                switch (val[0]) // obj[0] : term_FNO; obj[1] : term_FID; obj[2] : end_cbl_FID
                {
                    case 9600: // RT
                        lblDCabinet.Text = "RT : " + excAbb + " ";
                        lblDCabinet.Tag = "GC_RT";
                        strSQL = string.Format(strSQL, "GC_RT", val[1].ToString());
                        cabSQL = string.Format(cabSQL, "GC_RT", "RT_CODE", val[1].ToString());
                        break;
                    case 9100: // MSAN
                        lblDCabinet.Text = "MSAN : " + excAbb + " ";
                        lblDCabinet.Tag = "GC_MSAN";
                        strSQL = string.Format(strSQL, "GC_MSAN", val[1].ToString());
                        cabSQL = string.Format(cabSQL, "GC_MSAN", "RT_CODE", val[1].ToString());
                        break;
                    case 9800: // VDSL2
                        lblDCabinet.Text = "VDSL2 : " + excAbb + " ";
                        lblDCabinet.Tag = "GC_VDSL2";
                        strSQL = string.Format(strSQL, "GC_VDSL2", val[1].ToString());
                        cabSQL = string.Format(cabSQL, "GC_VDSL2", "RT_CODE", val[1].ToString());
                        break;
                    default: // 10300 : ITFACE
                        lblDCabinet.Text = "CAB : " + excAbb + " ";
                        lblDCabinet.Tag = "GC_ITFACE";
                        strSQL = string.Format(strSQL, "GC_ITFACE", val[1].ToString());
                        cabSQL = string.Format(cabSQL, "GC_ITFACE", "ITFACE_CODE, ITFACE_CLASS", val[1].ToString());
                        break;
                }
                log.WriteLog("Reading Database : " + strSQL);
                lsvDNetwork.Items.Clear();
                lsvDNetwork.Tag = val[1].ToString();
                lblDFID.Text = val[1].ToString();

                rs = GTClassFactory.Create<IGTApplication>().DataContext.OpenRecordset(strSQL, ADODB.CursorTypeEnum.adOpenStatic, ADODB.LockTypeEnum.adLockReadOnly, 1);
                if (rs.RecordCount > 0)
                {
                    while (!rs.EOF)
                    {
                        log.WriteLog("Read Recordset : " + rs.Fields[0].Value.ToString());
                        ListViewItem l = lsvDNetwork.Items.Add("#" + rs.Fields[0].Value.ToString());
                        l.Tag = l.Text.Substring(1, l.Text.IndexOf(":") - 2);
                        rs.MoveNext();
                    }
                }
                #endregion

                #region Get Cabinet Code
                if (val[0] == 10300)
                {
                    Dictionary<string, string> cab = myUtil.GetValues(cabSQL, new string[] { "ITFACE_CODE", "ITFACE_CLASS" });
                    lblDCabinet.Text = GetCabClass(cab["ITFACE_CLASS"]) + " : " + excAbb + " " + cab["ITFACE_CODE"];
                }
                else
                {
                    string cab = myUtil.GetSingleValue(cabSQL, "RT_CODE");
                    lblDCabinet.Text += cab;
                }
                #endregion

                #region Load Termination Points
                D_SIDE.GetTermPoints(val[1], cableCode, log);
                CheckDPRedundancy();

                btnDPList.Text = "[" + lblDCabinet.Text + "] [Cable Code : " + cableCode + "][" + clsDSIDE.max_lo + "-" + clsDSIDE.max_hi + "]";
                grdD.Sort(new DataGridColumnSorter(1, SortOrder.Ascending));
                grdD.Columns[1].HeaderCell.SortGlyphDirection = SortOrder.Ascending;
                #endregion

                Application.DoEvents();

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                isOnLoadMode = false;
                Cursor.Current = Cursors.Default;
            }
        }



        #region GUI gimmics
        private void btnNetworkList_Click(object sender, EventArgs e)
        {
            if (btnDNetworkList.ImageIndex == 0) // arrow UP
            {
                pnlNetwork.Height = btnDNetworkList.Height + 1;
                grpDHeader.Height = (btnDPList.ImageIndex == 1 ? 1 : 110);
                btnDNetworkList.ImageIndex = 1;
            }
            else
            {
                pnlNetwork.Height = 110;
                grpDHeader.Height = 1;
                btnDNetworkList.ImageIndex = 0;
            }
        }

        private void btnDPList_Click(object sender, EventArgs e)
        {
            if (btnDPList.ImageIndex == 0) // arrow UP
            {
                pnlNetwork.Height = btnDNetworkList.Height + 1;
                grpDHeader.Height = 1;
                btnDPList.ImageIndex = 1;
            }
            else
            {
                if (btnDNetworkList.ImageIndex == 0)
                    pnlNetwork.Height = 110;
                else
                    grpDHeader.Height = 110;
                btnDPList.ImageIndex = 0;
            }
        }

        private void General_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                //    if (gDCell != null)
                //    {
                //        if (gDCell.ColumnIndex < 3)
                //            grdD.Rows[gDCell.RowIndex].Cells[gDCell.ColumnIndex + 1].Selected = true;
                //        else
                //            grdD.Rows[gDCell.RowIndex+1].Cells[1].Selected = true;
                //}
                //if (this.GetNextControl(ActiveControl, true) != null)
                //{
                //    e.Handled = true;
                //    this.GetNextControl(ActiveControl, true).Focus();
                //}
            }
        }

        #endregion

        #region Datagrid D-Side
        clsDSIDE D_SIDE;

        #region "DataGrid Cell"

        DataGridViewCell gDCell;
        DataGridViewRow gDRow;

        private void grdD_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            gDCell = grdD.Rows[e.RowIndex].Cells[e.ColumnIndex];
            if (grdD.Rows[e.RowIndex] != gDRow)
            {
                gDRow = grdD.Rows[e.RowIndex];
                if (mnuDHiliteAuto.Checked && !isOnLoadMode)
                {
                    try
                    {
                        int key = (int)(gDRow.Cells[7].Value);
                        hiliteFeature(key, 7000, D_SIDE.TermPoints[key].detailID, 400);
                    }
                    catch (Exception ex)
                    {
                        log.WriteErr(ex);
                    }
                }

            }
        }

        private void grdD_RowChange(DataGridViewRow r)
        {
            //lblDTransfer.Text = "0";
            //txtDTermCode.Text = myUtil.CellValue(r.Cells[1].Value);
            //lblDPair.Text = myUtil.CellValue(r.Cells[4].Value);
            //lblDCableSize.Text = myUtil.CellValue(r.Cells[5].Value);
            //lblDState.Text = myUtil.CellValue(r.Cells[6].Value);
        }

        private void grdD_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.RowIndex == -1) return;
                isOnChange = !isOnLoadMode;
                string val = myUtil.CellValue(grdD.Rows[e.RowIndex].Cells[e.ColumnIndex].Value);
                int ind = (int)grdD.Rows[e.RowIndex].Cells[7].Value; // (int)grdD.Rows[e.RowIndex].Tag;

                #region Edit DP Number
                if (e.ColumnIndex == 1 && !isOnLoadMode)
                {
                    if (val == "" || val == "0")
                        grdD.Rows[e.RowIndex].Cells[1].Value = "0";

                    else
                    {
                        CheckDPRedundancy();
                        D_SIDE.ApplyDPNumber(ind, val.ToUpper());
                        grdD.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = val.ToUpper();
                    }
                }
                #endregion

                #region Edit Pair Count
                else if ((e.ColumnIndex == 2) || (e.ColumnIndex == 3))
                {
                    grdD.CellValueChanged -= grdD_CellValueChanged;
                    if (val == "" || val == "0")
                    {
                        grdD.Rows[e.RowIndex].Cells[2].Value = "0";
                        grdD.Rows[e.RowIndex].Cells[3].Value = "0";
                    }
                    else if (e.ColumnIndex == 2)
                    {
                        grdD.Rows[e.RowIndex].Cells[3].Value = GetDHiCount(e.RowIndex);
                    }
                    grdD.CellValueChanged += grdD_CellValueChanged;
                    D_SIDE.ApplyCount(grdD, ind, grdD.Rows[e.RowIndex]);
                }
                #endregion

                #region Edit Effective Pair
                else if (e.ColumnIndex == 4 && !isOnLoadMode)
                {
                    grdD.Rows[e.RowIndex].Cells[4].Style.BackColor = Color.Red;
                    if (val != "" && val != "0")
                    {
                        if (ValidEffectivePairs(e.RowIndex))
                        {
                            grdD.Rows[e.RowIndex].Cells[4].Style.BackColor = Color.White;
                            D_SIDE.ApplyEffectivePair(ind, val);
                        }
                    }
                }
                #endregion

            }
            catch (Exception ex)
            { log.WriteErr(ex); }
        }

        private int GetDHiCount(int r)
        {
            try
            {
                int lo = int.Parse(grdD.Rows[r].Cells[2].Value.ToString());
                int effective = int.Parse(grdD.Rows[r].Cells[4].Value.ToString());
                int cFID = (int)(grdD.Rows[r].Cells[7].Value);
                int cid = (int)(grdD.Rows[r].Cells[9].Value) - 1;

                clsDSIDE.PairCount d = D_SIDE.TermPoints[cFID];
                for (int i = 0; i < d.CID; i++)
                    if (i != cid) effective -= (d.Hi(i) - d.Lo(i) + 1);

                if (effective > 0)
                    return (lo + effective - 1);
                else
                    return lo;
            }
            catch
            {
                return 0;
            }
        }

        private bool ValidEffectivePairs(int row)
        {
            try
            {
                if (myUtil.ParseInt(grdD.Rows[row].Cells[4].Value.ToString()) > myUtil.ParseInt(grdD.Rows[row].Cells[5].Value.ToString()))
                    return false;
                else
                    return true;
            }
            catch (Exception ex)
            {
                log.WriteErr(ex);
                return false;
            }
        }

        private void CheckDPRedundancy()
        {
            for (int i = 0; i < grdD.Rows.Count - 1; i++)
                grdD.Rows[i].Cells[1].Style.BackColor = Color.White;

            for (int i = 0; i < grdD.Rows.Count - 1; i++)
            {
                for (int j = (i + 1); j < grdD.Rows.Count - 1; j++)
                {
                    string dp1 = myUtil.CellValue(grdD.Rows[i].Cells[1].Value);
                    string dp2 = myUtil.CellValue(grdD.Rows[j].Cells[1].Value);
                    if ((dp1 != "0") && (dp1.Length != 0) && (dp2 != "0") && (dp1.Length != 0))
                    {
                        if (i != j && dp1.Equals(dp2))
                        {
                            if (grdD.Rows[i].Cells[8].Value.ToString() != grdD.Rows[j].Cells[8].Value.ToString())
                            {
                                grdD.Rows[i].Cells[1].Style.BackColor = Color.Red;
                                grdD.Rows[j].Cells[1].Style.BackColor = Color.Red;
                                break;
                            }
                        }
                    }
                }
            }
        }

        private void grdD_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                int r = grdD.CurrentCell.RowIndex;
                int c = grdD.CurrentCell.ColumnIndex;

                if (c < 4)
                    grdD.CurrentCell = grdD.Rows[r].Cells[c + 1];
                else if (r == grdD.Rows.Count - 2)
                    grdD.CurrentCell = grdD.Rows[0].Cells[1];
                else
                    grdD.CurrentCell = grdD.Rows[r + 1].Cells[1];

            }
        }

        #endregion



        private void grdD_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            try
            {
                if (e.ColumnIndex == 1 || e.ColumnIndex == 2 || e.ColumnIndex == 3)
                {
                    try
                    {
                        int i = e.ColumnIndex;
                        if (grdD.Columns[i].HeaderCell.SortGlyphDirection == SortOrder.None ||
                            grdD.Columns[i].HeaderCell.SortGlyphDirection == SortOrder.Descending)
                        {
                            grdD.Sort(new DataGridColumnSorter(i, SortOrder.Ascending));
                            grdD.Columns[i].HeaderCell.SortGlyphDirection = SortOrder.Ascending;
                        }
                        else
                        {
                            grdD.Sort(new DataGridColumnSorter(i, SortOrder.Descending));
                            grdD.Columns[i].HeaderCell.SortGlyphDirection = SortOrder.Descending;
                        }
                    }
                    catch { }

                    Application.DoEvents();
                }
            }
            catch (Exception ex)
            {
                log.WriteErr(ex);
            }
        }

        private void grdD_CellStyleContentChanged(object sender, DataGridViewCellStyleContentChangedEventArgs e)
        {
            bool flag = true;
            if ((e.CellStyle.BackColor == Color.Red) || (e.CellStyle.ForeColor == Color.Red))
                flag = false;

            mnuDSavePlace.Enabled = flag;
            mnuDSave.Enabled = flag;
        }

        #endregion

        #region Menu Item - 21-Aug-2012 (3 Syawal)

        private void mnuClearAll_Click(object sender, EventArgs e)
        {
            try
            {
                foreach (DataGridViewRow r in grdD.Rows)
                    r.Cells[2].Value = "";
            }
            catch { }
        }

        private void mnuClearSelection_Click(object sender, EventArgs e)
        {
            try
            {
                foreach (DataGridViewRow r in grdD.SelectedRows)
                    r.Cells[2].Value = "";
            }
            catch { }
        }

        private void mnuDSave_Click(object sender, EventArgs e)
        {
            bool all = false;
            string available = D_SIDE.AvailableCount();
            //if (available != "No available count")
            //{
            //    if (MessageBox.Show(this, "There are unassigned count\r\n" + available, 
            //        "Unassigned Count", MessageBoxButtons.OKCancel) == DialogResult.Cancel)
            //        return;
            //}
            if (sender.Equals(mnuDSaveAll) || sender.Equals(mnuDSavePlaceAll))
                all = true;
            else if (grdD.SelectedRows.Count == 0)
            {
                MessageBox.Show(this, "Please select rows to save");
                return;
            }
            bool place = false;
            if (sender.Equals(mnuDSavePlaceSelection) || sender.Equals(mnuDSavePlaceAll)) place = true;

            Cursor.Current = Cursors.WaitCursor;
            if (D_SIDE.ApplySetting(grdD, lblDCableCode.Text, place, all))
            {
                isOnChange = false;
                MessageBox.Show(this, "D-SIDE Pair Count already saved to NEPS");
            }
            Cursor.Current = Cursors.Default;
            GTPairCount.m_CustomForm.TopMost = true;
            prgD.Visible = false;
        }


        private void mnuAutoDPNo_Click(object sender, EventArgs e)
        {
            try
            {
                string DP_NUM = GetNextDPNumber(lblDExc.Text, lblDCabinet.Text, (string)lblDCabinet.Tag);
                for (int i = 0; i < grdD.Rows.Count - 1; i++)
                {
                    DataGridViewRow r = grdD.Rows[i];
                    int dp = myUtil.ParseInt(r.Cells[3].Value.ToString()) / 10;
                    if (r.Cells[1].Value.ToString().Trim().Length == 0)
                    {
                        r.Cells[1].Value = DP_NUM;
                        r.Cells[1].Style.BackColor = Color.LightBlue;
                        DP_NUM = Convert.ToString(int.Parse(DP_NUM) + 1);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, "ERROR : " + ex.Source + "\r\n" + ex.Message);
            }
        }

        private string GetNextDPNumber(string excabb, string cabcode, string cabtable)
        {
            try
            {
                string ssql = "SELECT MAX (LPAD(REGEXP_REPLACE (A.DP_NUM,'[A-Z]|[a-z]','') ,7,'0'))  " +
                "FROM GC_DP A, GC_NETELEM B WHERE A.G3E_FID = B.G3E_FID AND A.{0} = '{1}' AND B.EXC_ABB = '{2}' ";

                if (cabtable == "GC_ITFACE")
                    ssql = string.Format(ssql, "ITFACE_CODE", cabcode, excabb);
                else
                    ssql = string.Format(ssql, "RT_CODE", cabcode, excabb);

                ADODB.Recordset rs = new ADODB.Recordset();
                rs = GTClassFactory.Create<IGTApplication>().DataContext.OpenRecordset(ssql,
                    ADODB.CursorTypeEnum.adOpenStatic, ADODB.LockTypeEnum.adLockOptimistic, 1);

                if (!rs.EOF)
                {
                    return Convert.ToString(Convert.ToInt32(rs.Fields[0].Value) + 1);
                }
                else
                {
                    return "1";
                }
            }
            catch (Exception ex)
            {
                return "1";
            }
        }


        private void mnuAutoDPCount_Click(object sender, EventArgs e)
        {
            try
            {
                for (int i = 0; i < grdD.Rows.Count - 1; i++)
                {
                    Debug.Write("i = " + i.ToString());
                    DataGridViewRow r = grdD.Rows[i];
                    string dpnum = r.Cells[1].Value.ToString();
                    if (!isNumeric(dpnum, System.Globalization.NumberStyles.Integer))
                    {
                        for (int c = dpnum.Length - 1; c > -1; c--)
                            if (!char.IsDigit(dpnum[c])) dpnum = dpnum.Remove(c);

                    }
                    int dp = int.Parse(dpnum) * 10 + 1;
                    dp -= int.Parse(r.Cells[4].Value.ToString());
                    Debug.WriteLine(dp.ToString());
                    r.Cells[2].Value = dp.ToString();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, "ERROR : " + ex.Source + "\r\n" + ex.Message);
            }
        }

        private void mnuHiliteCable_Click(object sender, EventArgs e)
        {
            try
            {
                int key = (int)(gDRow.Cells[7].Value);
                hiliteFeature(key, 7000, D_SIDE.TermPoints[key].detailID, 400);
            }
            catch { }
        }

        private void mnuHiliteTerm_Click(object sender, EventArgs e)
        {
            try
            {
                int key = (int)(gDRow.Cells[7].Value);
                if (D_SIDE.TermPoints[key].termFNO == 10800) //its a splice -> stump or stub
                    hiliteFeature(key, 7000, D_SIDE.TermPoints[key].detailID, 200);
                else
                    hiliteFeature(D_SIDE.TermPoints[key].termFID, D_SIDE.TermPoints[key].termFNO, D_SIDE.TermPoints[key].detailID, 200);
            }
            catch { }
        }

        #endregion

        #region Progress Bar
        public void InitDProgressBar(int val)
        {
            prgD.Maximum = val;
            prgD.Value = 0;
        }
        public void IncreaseDProgressBar(int val)
        {
            try
            {
                if (val == prgD.Maximum || val == -1)
                    prgD.Value = prgD.Maximum;
                else if (prgD.Value + val > prgD.Maximum)
                    prgD.Value = prgD.Maximum - 5;
                else if (val == 0)
                    prgD.Value = 0;
                else
                    prgD.Value += val;
                prgD.Visible = (prgD.Value != 0);

            }
            catch (Exception ex)
            {
                log.WriteErr(ex);
            }
            finally { Application.DoEvents(); }
        }
        #endregion

        private void lsvDNetwork_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (lsvDNetwork.SelectedItems.Count > 0)
                {
                    // Load Network
                    string cblcode = lsvDNetwork.SelectedItems[0].Text.Substring(1);
                    cblcode = cblcode.Remove(cblcode.IndexOf(':')).Trim();
                    if (isOnChange)
                    {
                        if (MessageBox.Show(this,
                            "Current changes not yet save.\r\nDo you want to proceed without saving",
                            "D-SIDE Pair Count", MessageBoxButtons.OKCancel) == DialogResult.Cancel)
                            return;
                    }

                    if (cblcode != lblDCableCode.Text)
                    {
                        Cursor.Current = Cursors.WaitCursor;
                        isOnLoadMode = true;
                        D_SIDE.GetTermPoints(int.Parse(lblDFID.Text), cblcode, log);
                        CheckDPRedundancy();

                        grdD.Sort(new DataGridColumnSorter(1, SortOrder.Ascending));
                        grdD.Columns[1].HeaderCell.SortGlyphDirection = SortOrder.Ascending;

                        btnDPList.Text = "[" + lblDCabinet.Text + "] [Cable Code : " + cblcode + "][" + clsDSIDE.max_lo + "-" + clsDSIDE.max_hi + "]";
                        lblDCableCode.Text = cblcode;
                        isOnLoadMode = false;
                        isOnChange = false;
                    }
                }
            }
            catch (Exception ex)
            {
                log.WriteErr(ex);
            }
            finally
            {
                Cursor.Current = Cursors.Default;
            }
        }

        #endregion

        #region E-Side
        clsESIDE E_SIDE;
        string isExcAbbE = "";

        #region Loading E-SIDE
        private void loadE(int isFID, short isFNO, string cableCode, string excAbb)
        {
            string strSQL = "";

            isOnLoadMode = true;
            isOnChange = false;

            isExcAbbE = excAbb;
            lblEExc.Text = excAbb;
            lblECableCode.Text = cableCode;

            E_SIDE = new clsESIDE(grdE, excAbb, cableCode);

            Cursor.Current = Cursors.WaitCursor;

            // Load Available Count for E-Side
            int mjFID = LoadETail(cableCode);

            // Load Network
            #region Load Cable Code
            lsvENetwork.Items.Clear();

            strSQL = "SELECT B.CABLE_CODE, SUM(F.CURRENT_HIGH - F.CURRENT_LOW + 1) TOTAL_COUNT "
            + "FROM GC_CBL A, GC_SPLICE B, GC_NR_CONNECT C, GC_VERTICAL D, GC_NETELEM E, GC_COUNT F "
            + "WHERE A.CABLE_CLASS = 'TAIL' AND E.EXC_ABB = '" + excAbb + "' AND A.G3E_FID = C.G3E_FID AND A.G3E_FID = E.G3E_FID "
            + "AND B.G3E_FID = C.OUT_FID AND D.G3E_FID = C.IN_FID AND A.G3E_FID = F.G3E_FID GROUP BY B.CABLE_CODE";

            ADODB.Recordset rsE = GTClassFactory.Create<IGTApplication>().DataContext.OpenRecordset(strSQL, ADODB.CursorTypeEnum.adOpenStatic, ADODB.LockTypeEnum.adLockReadOnly, 1);
            if (rsE.RecordCount > 0)
            {
                while (!rsE.EOF)
                {
                    string cblcode = myUtil.rsField(rsE, "CABLE_CODE");
                    log.WriteLog("Read Recordset : " + cblcode);
                    ListViewItem l = lsvENetwork.Items.Add("#" + cblcode + " [total MDF count : " + myUtil.rsField(rsE, "TOTAL_COUNT") + "]");
                    l.Tag = cblcode;
                    rsE.MoveNext();
                }

            }
            #endregion

            LoadETermination(mjFID, cableCode);
            isOnLoadMode = false;
            isOnChange = false;

            pnlENetwork.Height = btnENetworkList.Height + 1;
            btnENetworkList.ImageIndex = 1;

            Cursor.Current = Cursors.Default;
        }

        private int LoadETail(string cableCode)
        {
            int mainJointFID = -1;

            #region Load Tail
            ADODB.Recordset rsE = new ADODB.Recordset();
            string strSQL = "SELECT A.G3E_FID TAIL_FID, D.G3E_FID VERT_FID, C.OUT_FID MAINJOINT_FID, B.CABLE_CODE, D.MDF_NUM, D.VERT_NUM, F.COUNT_ANNOTATION, F.CURRENT_HIGH, F.CURRENT_LOW "
                 + "FROM GC_CBL A, GC_SPLICE B, GC_NR_CONNECT C, GC_VERTICAL D, GC_NETELEM E, GC_COUNT F "
                 + "WHERE A.CABLE_CLASS = 'TAIL' AND B.CABLE_CODE = '" + cableCode + "' AND E.EXC_ABB = '" + isExcAbbE + "' "
                 + "AND A.G3E_FID = C.G3E_FID AND A.G3E_FID = E.G3E_FID AND A.G3E_FID = F.G3E_FID "
                 + "AND B.G3E_FID = C.OUT_FID AND D.G3E_FID = C.IN_FID";


            rsE = GTClassFactory.Create<IGTApplication>().DataContext.OpenRecordset(strSQL, ADODB.CursorTypeEnum.adOpenStatic, ADODB.LockTypeEnum.adLockReadOnly, 1);

            E_SIDE.tails.Clear();
            cmbMDFVert.Items.Clear();
            if (rsE.RecordCount > 0)
            {
                int i = 0;
                grdE.Rows.Clear();
                mainJointFID = myUtil.ParseInt(myUtil.rsField(rsE, "MAINJOINT_FID"));
                do
                {
                    string mdf = myUtil.rsField(rsE, "MDF_NUM").Trim().ToUpper();
                    if (mdf.IndexOf("MDF") > -1) mdf = mdf.Substring(mdf.IndexOf("MDF") + 3);
                    string ver = myUtil.rsField(rsE, "VERT_NUM").Trim().ToUpper();
                    string tail = mdf + "/" + ver;

                    clsESIDE.TAIL t = new clsESIDE.TAIL(
                        myUtil.ParseInt(myUtil.rsField(rsE, "TAIL_FID")),
                        myUtil.ParseInt(myUtil.rsField(rsE, "VERT_FID")),
                        mdf, ver,
                        myUtil.ParseInt(myUtil.rsField(rsE, "CURRENT_HIGH")),
                        myUtil.ParseInt(myUtil.rsField(rsE, "CURRENT_LOW")));

                    if (!cmbMDFVert.Items.Contains(tail)) //check if item is already in drop down, if not, add it to all
                    {
                        E_SIDE.tails.Add(tail, t);
                        cmbMDFVert.Items.Add(tail);
                    }
                    else
                    {
                        clsESIDE.TAIL t1 = E_SIDE.tails[tail];
                        if (t1.vertLo > t.vertLo) t1.vertLo = t.vertLo;
                        if (t1.vertHi < t.vertHi) t1.vertHi = t.vertHi;
                        E_SIDE.tails[tail] = t1;
                    }
                    rsE.MoveNext();
                }
                while (!rsE.EOF);
                if (cmbMDFVert.Items.Count > 0) cmbMDFVert.SelectedIndex = 0;
            }
            rsE = null;
            strSQL = "";
            #endregion

            return mainJointFID;
        }

        private void LoadETermination(int mjFID, string cableCode)
        {
            //int mjFID = clsTrace.TraceUp_EGetStartPoint(FID);

            Dictionary<int, int> endsFID = clsTrace.TraceDown_GetEndPoints(mjFID, "IN_FID");
            string CABs = "";
            string CBLs = "";
            string DDPs = "";
            foreach (int key in endsFID.Keys)
            {
                int endsFNO = endsFID[key];
                switch (endsFNO)
                {
                    case 10300: //CABs
                        CABs += (CABs.Length > 0 ? "," : "") + key.ToString();
                        break;
                    case 6300: // DDP (6200 : PDDP)
                        DDPs += (DDPs.Length > 0 ? "," : "") + key.ToString();
                        break;
                    default: // CBL or SPLICE
                        CBLs += (CBLs.Length > 0 ? "," : "") + key.ToString();
                        break;
                }
            }

            LoadEITFace(CABs, cableCode);
            LoadEDDP(DDPs, cableCode);
            LoadEStubStump(CBLs, cableCode);

        }
        // FNO : 10300 = ITFACE; 6200= PDDP
        private void LoadEITFace(string CAB_FID, string cableCode)
        {
            E_SIDE.termpoints.Clear();
            E_SIDE.eCounts.Clear();
            grdE.Rows.Clear();

            if (CAB_FID.Length == 0) return;

            string strSQL = "SELECT A.G3E_FID, A.EFFECTIVE_PAIRS, A.TOTAL_SIZE, A.COUNT_ANNOTATION, B.G3E_FID TERM_FID, B.G3E_FNO TERM_FNO, B.ITFACE_CLASS TERM_CLASS, B.ITFACE_CODE TERM_CODE, D.FEATURE_STATE, ";
            strSQL += "(SELECT CURRENT_LOW||'-'||CURRENT_HIGH FROM GC_COUNT WHERE G3E_FID = A.G3E_FID AND ROWNUM = 1) ANNOTATION, ";
            strSQL += "(SELECT GC_OSP_COP_VAL.EXTRACT_LINE_TO_VECTOR(G3E_GEOMETRY) FROM GC_ITFACE_S WHERE G3E_FID = B.G3E_FID) TERM_GEO_XY, ";
            strSQL += "(SELECT GC_OSP_COP_VAL.EXTRACT_LINE_TO_VECTOR(G3E_GEOMETRY) FROM DGC_ITFACE_S WHERE G3E_FID = B.G3E_FID) TERM_DET_XY, ";
            strSQL += "(SELECT GC_OSP_COP_VAL.EXTRACT_LINE_TO_VECTOR(G3E_GEOMETRY) FROM GC_CBL_L WHERE G3E_FID = A.G3E_FID) CBL_GEO_XY, ";
            strSQL += "(SELECT GC_OSP_COP_VAL.EXTRACT_LINE_TO_VECTOR(G3E_GEOMETRY) FROM DGC_CBL_L WHERE G3E_FID = A.G3E_FID) CBL_DET_XY, ";
            strSQL += "(SELECT G3E_DETAILID FROM DGC_CBL_L WHERE G3E_FID = A.G3E_FID) G3E_DETAILID ";
            strSQL += "FROM GC_CBL A, GC_ITFACE B, GC_NR_CONNECT C, GC_NETELEM D ";
            strSQL += "WHERE C.OUT_FNO = 10300 AND A.G3E_FID = C.G3E_FID AND A.G3E_FID = D.G3E_FID AND C.OUT_FID = B.G3E_FID ";
            strSQL += "AND B.G3E_FID IN (" + CAB_FID + ") AND A.CABLE_CODE = '" + cableCode + "' ORDER BY TERM_CODE ASC";

            LoadETerminationPoints(strSQL);
        }

        private void LoadEDDP(string DDP_FID, string cableCode)
        {
            if (DDP_FID.Length == 0) return;

            string strSQL = "SELECT A.G3E_FID, A.EFFECTIVE_PAIRS, A.TOTAL_SIZE, A.COUNT_ANNOTATION, B.G3E_FID TERM_FID, B.G3E_FNO TERM_FNO, 'DDP' TERM_CLASS, B.DDP_NUM TERM_CODE, D.FEATURE_STATE, ";
            strSQL += "(SELECT CURRENT_LOW||'-'||CURRENT_HIGH FROM GC_COUNT WHERE G3E_FID = A.G3E_FID AND ROWNUM = 1) ANNOTATION, ";
            strSQL += "(SELECT GC_OSP_COP_VAL.EXTRACT_LINE_TO_VECTOR(G3E_GEOMETRY) FROM GC_DDP_S WHERE G3E_FID = B.G3E_FID) TERM_GEO_XY, ";
            strSQL += "(SELECT GC_OSP_COP_VAL.EXTRACT_LINE_TO_VECTOR(G3E_GEOMETRY) FROM DGC_DDP_S WHERE G3E_FID = B.G3E_FID) TERM_DET_XY, ";
            strSQL += "(SELECT GC_OSP_COP_VAL.EXTRACT_LINE_TO_VECTOR(G3E_GEOMETRY) FROM GC_CBL_L WHERE G3E_FID = A.G3E_FID) CBL_GEO_XY, ";
            strSQL += "(SELECT GC_OSP_COP_VAL.EXTRACT_LINE_TO_VECTOR(G3E_GEOMETRY) FROM DGC_CBL_L WHERE G3E_FID = A.G3E_FID) CBL_DET_XY, ";
            strSQL += "(SELECT G3E_DETAILID FROM DGC_CBL_L WHERE G3E_FID = A.G3E_FID) G3E_DETAILID ";
            strSQL += "FROM GC_CBL A, GC_DDP B, GC_NR_CONNECT C, GC_NETELEM D ";
            strSQL += "WHERE C.OUT_FNO = 6300 AND A.G3E_FID = C.G3E_FID AND A.G3E_FID = D.G3E_FID AND C.OUT_FID = B.G3E_FID ";
            strSQL += "AND B.G3E_FID IN (" + DDP_FID + ") AND A.CABLE_CODE = '" + cableCode + "' ORDER BY TERM_CODE ASC";

            LoadETerminationPoints(strSQL);

        }

        private void LoadEStubStump(string endFID, string cableCode)
        {
            if (endFID.Length == 0) return;

            string strSQL = "SELECT A.G3E_FID, A.EFFECTIVE_PAIRS, A.TOTAL_SIZE, A.COUNT_ANNOTATION, B.G3E_FID TERM_FID, B.G3E_FNO TERM_FNO, B.SPLICE_CLASS TERM_CLASS, '' TERM_CODE, D.FEATURE_STATE, ";
            strSQL += "(SELECT CURRENT_LOW||'-'||CURRENT_HIGH FROM GC_COUNT WHERE G3E_FID = A.G3E_FID AND ROWNUM = 1) ANNOTATION, ";
            strSQL += "(SELECT GC_OSP_COP_VAL.EXTRACT_LINE_TO_VECTOR(G3E_GEOMETRY) FROM GC_SPLICE_S WHERE G3E_FID = B.G3E_FID) TERM_GEO_XY, ";
            strSQL += "(SELECT GC_OSP_COP_VAL.EXTRACT_LINE_TO_VECTOR(G3E_GEOMETRY) FROM DGC_SPLICE_S WHERE G3E_FID = B.G3E_FID) TERM_DET_XY, ";
            strSQL += "(SELECT GC_OSP_COP_VAL.EXTRACT_LINE_TO_VECTOR(G3E_GEOMETRY) FROM GC_CBL_L WHERE G3E_FID = A.G3E_FID) CBL_GEO_XY, ";
            strSQL += "(SELECT GC_OSP_COP_VAL.EXTRACT_LINE_TO_VECTOR(G3E_GEOMETRY) FROM DGC_CBL_L WHERE G3E_FID = A.G3E_FID) CBL_DET_XY, ";
            strSQL += "(SELECT G3E_DETAILID FROM DGC_CBL_L WHERE G3E_FID = A.G3E_FID) G3E_DETAILID ";
            strSQL += "FROM GC_CBL A, GC_SPLICE B, GC_NR_CONNECT C, GC_NETELEM D ";
            strSQL += "WHERE A.G3E_FID = C.G3E_FID AND A.G3E_FID = D.G3E_FID AND C.OUT_FID = B.G3E_FID ";
            strSQL += "AND B.G3E_FID IN (" + endFID + ") AND A.CABLE_CODE = '" + cableCode + "' ORDER BY TERM_CODE ASC";

            LoadETerminationPoints(strSQL);
        }

        private void LoadETerminationPoints(string strSQL)
        {
            try
            {
                ADODB.Recordset rsE = new ADODB.Recordset();
                rsE = GTClassFactory.Create<IGTApplication>().DataContext.OpenRecordset(strSQL, ADODB.CursorTypeEnum.adOpenStatic, ADODB.LockTypeEnum.adLockReadOnly, 1);

                while (!rsE.EOF)
                {
                    string term_class = myUtil.rsField(rsE, "TERM_CLASS").Trim();
                    if (term_class.IndexOf("E-SIDE") > -1)
                        term_class = term_class.Remove(term_class.Length - 6).Trim();
                    else
                        term_class = GetCabClass(term_class);

                    clsESIDE.TERMPOINT t = new clsESIDE.TERMPOINT(
                        myUtil.rsField(rsE, "TERM_CODE"), myUtil.ParseInt(myUtil.rsField(rsE, "TERM_FID")), short.Parse(myUtil.rsField(rsE, "TERM_FNO")), term_class,
                        myUtil.ParseInt(myUtil.rsField(rsE, "G3E_FID")), myUtil.ParseInt(myUtil.rsField(rsE, "EFFECTIVE_PAIRS")),
                        myUtil.ParseInt(myUtil.rsField(rsE, "TOTAL_SIZE")), myUtil.rsField(rsE, "FEATURE_STATE"),
                        GetGeoPoint(myUtil.rsField(rsE, "TERM_GEO_XY"), myUtil.rsField(rsE, "TERM_DET_XY"), true),
                        GetGeoPoint(myUtil.rsField(rsE, "CBL_GEO_XY"), myUtil.rsField(rsE, "CBL_DET_XY"), false),
                        myUtil.ParseInt(myUtil.rsField(rsE, "G3E_DETAILID"))
                    );

                    if (!E_SIDE.termpoints.ContainsKey(t.cblFID))
                    {
                        E_SIDE.termpoints.Add(t.cblFID, t);
                        t.cblCID = GetTermCount(t, myUtil.rsField(rsE, "COUNT_ANNOTATION"));
                        E_SIDE.termpoints[t.cblFID] = t;
                    }
                    rsE.MoveNext();
                }

            }
            catch (Exception ex)
            {
                log.WriteErr(ex);
            }
        }

        private string GetCabClass(string val)
        {
            switch (val.ToUpper())
            {
                case "PHANTOM CABINET": return "PCAB";
                case "SDF": return "SDF";
                case "DDP": return "DDP";
                default: return "CAB";
            }
        }

        private int GetTermCount(clsESIDE.TERMPOINT t, string annotation)
        {
            try
            {
                if (annotation.Length > 0)
                {
                    while (annotation.Length > 5)
                    {
                        int i = annotation.IndexOf(Environment.NewLine);
                        try
                        {
                            string[] anno = (i == -1 ? annotation.Split('/') : annotation.Remove(i).Split('/'));
                            string[] mdf = anno[3].Split('-');
                            string[] cab = anno[4].Split('-');
                            clsESIDE.ESIDECount cnt = new clsESIDE.ESIDECount(t.termFID, t.cblFID, ++t.cblCID, anno[1], anno[2]);
                            cnt.SetCount(anno[1].Trim(), anno[2].Trim(),
                                myUtil.ParseInt(mdf[0]), myUtil.ParseInt(mdf[1]), myUtil.ParseInt(cab[0]), myUtil.ParseInt(cab[1]));

                            int r = grdE.Rows.Count - 1;

                            grdE.Rows.Add();
                            gERow = grdE.Rows[r];

                            grdE.Rows[r].Cells[0].Value = t.termType + " " + t.termNo;
                            grdE.Rows[r].Cells[1].Value = cnt.key; //.mdf_no + "/" + cnt.vert_no;
                            grdE.Rows[r].Cells[2].Value = mdf[0];
                            grdE.Rows[r].Cells[3].Value = mdf[1];
                            grdE.Rows[r].Cells[4].Value = cab[0];
                            grdE.Rows[r].Cells[5].Value = cab[1];
                            grdE.Rows[r].Cells[6].Value = t.efctvPair;
                            grdE.Rows[r].Cells[7].Value = t.cableSize;
                            grdE.Rows[r].Cells[8].Value = t.featureState;
                            grdE.Rows[r].Cells[9].Value = t.cblFID;
                            grdE.Rows[r].Cells[10].Value = t.termFID;
                            grdE.Rows[r].Tag = r;

                            E_SIDE.eCounts.Add(r, cnt);
                            if (!cmbMDFVert.Items.Contains(cnt.key))
                            {
                                cnt.unbalanced = true;
                                grdE.Rows[r].Cells[1].Style.BackColor = Color.Red;
                                //grdE.Rows[r].Cells[1].Style.ForeColor = Color.White;
                            }
                            else
                            {
                                E_SIDE.ApplyCount(grdE, grdE.Rows[r], t.termFID, t.cblFID, cnt.mdf_no, cnt.vert_no,
                                    myUtil.ParseInt(mdf[0]), myUtil.ParseInt(mdf[1]),
                                    myUtil.ParseInt(cab[0]), myUtil.ParseInt(cab[1]));
                            }
                        }
                        catch (Exception ex)
                        {
                            log.WriteErr(ex);
                        }
                        if (i == -1 || i + 1 > annotation.Length) break;
                        annotation = annotation.Substring(i + 2);
                    }
                }
                else
                {
                    clsESIDE.ESIDECount cnt = new clsESIDE.ESIDECount(t.termFID, t.cblFID, ++t.cblCID, "", "");
                    int r = grdE.Rows.Count - 1;
                    grdE.Rows.Add();
                    gERow = grdE.Rows[r];

                    grdE.Rows[r].Cells[0].Value = t.termType + " " + t.termNo;
                    grdE.Rows[r].Cells[1].Value = cmbMDFVert.Text;
                    grdE.Rows[r].Cells[2].Value = "0";
                    grdE.Rows[r].Cells[3].Value = "0";
                    grdE.Rows[r].Cells[4].Value = "0";
                    grdE.Rows[r].Cells[5].Value = "0";
                    grdE.Rows[r].Cells[6].Value = t.efctvPair;
                    grdE.Rows[r].Cells[7].Value = t.cableSize;
                    grdE.Rows[r].Cells[8].Value = t.featureState;
                    grdE.Rows[r].Cells[9].Value = t.cblFID;
                    grdE.Rows[r].Cells[10].Value = t.termFID;
                    grdE.Rows[r].Tag = r;

                    E_SIDE.eCounts.Add(r, cnt);
                    Application.DoEvents();
                }


            }
            catch (Exception ex)
            {
                log.WriteErr(ex);
            }
            return t.cblCID;

        }

        private IGTPoint GetGeoPoint(string geo, string det, bool firstpoint)
        {
            IGTPoint geopoint = GTClassFactory.Create<IGTPoint>();
            try
            {
                string vector = (geo.Length > 0 ? geo : det);
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
        #endregion

        #region DataGridView Events
        private DataGridViewCell gECell;
        private DataGridViewRow gERow;
        private void grdE_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.RowIndex == -1 || isOnLoadMode) return;
                grdE.Rows[e.RowIndex].Cells[e.ColumnIndex].Style.BackColor = Color.White;
                isOnChange = !isOnLoadMode;
                string val = myUtil.CellValue(grdE.Rows[e.RowIndex].Cells[e.ColumnIndex].Value);

                if (e.ColumnIndex > 6) return;
                isOnLoadMode = true;

                if (e.ColumnIndex == 6)
                    EditEffectivePair(gERow);

                else if ((e.ColumnIndex > 0) && (e.ColumnIndex < 6))
                #region Edit Pair Count

                {
                    if (e.ColumnIndex > 1)
                    {
                        if (val == "" || val == "0")
                        {
                            if (e.ColumnIndex == 2) grdE.Rows[e.RowIndex].Cells[3].Value = "0";
                            else if (e.ColumnIndex == 3) grdE.Rows[e.RowIndex].Cells[2].Value = "0";
                            else if (e.ColumnIndex == 4) grdE.Rows[e.RowIndex].Cells[5].Value = "0";
                            else if (e.ColumnIndex == 5) grdE.Rows[e.RowIndex].Cells[4].Value = "0";
                        }
                        else if (e.ColumnIndex == 2) // mdf_lo
                        {
                            grdE.Rows[e.RowIndex].Cells[3].Value =
                                myUtil.ParseInt(myUtil.CellValue(grdE.Rows[e.RowIndex].Cells[2].Value)) +
                                myUtil.ParseInt(myUtil.CellValue(grdE.Rows[e.RowIndex].Cells[6].Value)) - 1;
                        }
                        else if (e.ColumnIndex == 4) // pair_lo
                        {
                            int diff = myUtil.ParseInt(myUtil.CellValue(grdE.Rows[e.RowIndex].Cells[3].Value))
                                - myUtil.ParseInt(myUtil.CellValue(grdE.Rows[e.RowIndex].Cells[2].Value));
                            if (diff > 0 && diff <= myUtil.ParseInt(myUtil.CellValue(grdE.Rows[e.RowIndex].Cells[6].Value)))
                                grdE.Rows[e.RowIndex].Cells[5].Value =
                                    myUtil.ParseInt(myUtil.CellValue(grdE.Rows[e.RowIndex].Cells[4].Value)) + diff;
                            else
                                grdE.Rows[e.RowIndex].Cells[5].Value =
                                    myUtil.ParseInt(myUtil.CellValue(grdE.Rows[e.RowIndex].Cells[4].Value)) +
                                    myUtil.ParseInt(myUtil.CellValue(grdE.Rows[e.RowIndex].Cells[6].Value)) - 1;
                        }
                        isOnLoadMode = false;
                    }

                    E_SIDE.ApplyCount(grdE, gERow, (int)gERow.Cells[10].Value,
                        (int)gERow.Cells[9].Value, lblMDFunit.Text, lblVERTunit.Text,
                        myUtil.ParseInt(gERow.Cells[2].Value.ToString()),
                        myUtil.ParseInt(gERow.Cells[3].Value.ToString()),
                        myUtil.ParseInt(gERow.Cells[4].Value.ToString()),
                        myUtil.ParseInt(gERow.Cells[5].Value.ToString()));

                }
                #endregion
            }
            catch (Exception ex)
            { log.WriteErr(ex); }
            finally
            {
                isOnLoadMode = false;
            }
        }

        private void EditEffectivePair(DataGridViewRow row)
        {
            int effective = myUtil.ParseInt(row.Cells[6].Value.ToString());
            int cablesize = myUtil.ParseInt(row.Cells[7].Value.ToString());
            int cable_FID = myUtil.ParseInt(row.Cells[9].Value.ToString());

            Color bg = Color.White;
            Color fg = Color.Black;

            if ((effective == 0 || effective > cablesize)
            || (myUtil.ParseInt(row.Cells[3].Value.ToString())
                - myUtil.ParseInt(row.Cells[2].Value.ToString()) > effective)
            || (myUtil.ParseInt(row.Cells[5].Value.ToString())
                - myUtil.ParseInt(row.Cells[4].Value.ToString()) > effective))
            {
                bg = Color.Red;
                fg = Color.Yellow;
            }
            else
            {
                clsESIDE.TERMPOINT t = E_SIDE.termpoints[cable_FID];
                t.efctvPair = effective;
                E_SIDE.termpoints[cable_FID] = t;
            }

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

        private void grdE_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            gECell = grdE.Rows[e.RowIndex].Cells[e.ColumnIndex];
            if (gERow == null)
                gERow = grdE.Rows[e.RowIndex];
            else if (gERow.Index != e.RowIndex)
                gERow = grdE.Rows[e.RowIndex];
            else
                return;


            string mdf = myUtil.CellValue(grdE.Rows[e.RowIndex].Cells[1].Value);
            if (gERow.Tag == null)
            {
                gERow.Cells[1].Value = "";
                gERow.Cells[2].Style.BackColor = Color.White;
                gERow.Cells[3].Style.BackColor = Color.White;
                gERow.Cells[4].Style.BackColor = Color.White;
                gERow.Cells[5].Style.BackColor = Color.White;
            }
            else if (mdf.Length == 0)
            {
                gERow.Cells[1].Value = cmbMDFVert.Text;
                gERow.Cells[2].Style.BackColor = Color.LightGray;
                gERow.Cells[3].Style.BackColor = Color.LightGray;
                gERow.Cells[4].Style.BackColor = Color.LightGray;
                gERow.Cells[5].Style.BackColor = Color.LightGray;
            }
            else
                cmbMDFVert.Text = mdf;

            lblECableSize.Text = myUtil.CellValue(gERow.Cells[7].Value);
            lblEPair.Text = myUtil.CellValue(gERow.Cells[6].Value);
            lblEState.Text = myUtil.CellValue(gERow.Cells[8].Value);

            if (mnuEHiliteAuto.Checked && !isOnLoadMode)
            {
                try
                {
                    int key = myUtil.ParseInt(gERow.Cells[9].Value.ToString());
                    if (E_SIDE.termpoints.ContainsKey(key))
                    {
                        hiliteFeature(E_SIDE.termpoints[key].cblFID, 7000, E_SIDE.termpoints[key].detailID, 400);
                    }
                }
                catch (Exception ex)
                {
                    log.WriteErr(ex);
                }
            }

        }

        bool ESIDECountOK = true;
        private void grdE_CellStyleContentChanged(object sender, DataGridViewCellStyleContentChangedEventArgs e)
        {
            if ((e.CellStyle.BackColor == Color.Red) || (e.CellStyle.ForeColor == Color.Red))
                ESIDECountOK = false;
            else
                ESIDECountOK = true;

            mnuESavePlace.Enabled = ESIDECountOK;
            mnuESave.Enabled = ESIDECountOK;
        }
        #endregion

        private void cmbMDFVert_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbMDFVert.SelectedItem != null)
            {
                if (E_SIDE.tails.ContainsKey(cmbMDFVert.Text))
                {
                    if (E_SIDE.tails.ContainsKey(cmbMDFVert.Text))
                    {
                        lblMDFHi.Text = E_SIDE.tails[cmbMDFVert.Text].vertHi.ToString();
                        lblMDFLo.Text = E_SIDE.tails[cmbMDFVert.Text].vertLo.ToString();
                        lblMDFunit.Text = E_SIDE.tails[cmbMDFVert.Text].mdfNo;
                        lblVERTunit.Text = E_SIDE.tails[cmbMDFVert.Text].vertNo;
                    }

                    if (gERow != null)
                    {
                        if (gERow.Tag != null)
                        {
                            if ((gERow.Cells[1].Value == null) || gERow.Cells[1].Value.ToString() != cmbMDFVert.Text)
                            {
                                gERow.Cells[1].Value = cmbMDFVert.Text;
                            }
                        }
                    }
                }
            }
        }

        #region Gimmics
        private void btnENetworkList_Click(object sender, EventArgs e)
        {
            if (btnENetworkList.ImageIndex == 0) // arrow UP
            {
                pnlENetwork.Height = btnENetworkList.Height + 1;
                btnENetworkList.ImageIndex = 1;
            }
            else
            {
                pnlENetwork.Height = 90;
                btnENetworkList.ImageIndex = 0;
            }
        }

        private void btnETerm_Click(object sender, EventArgs e)
        {
            if (btnETerm.ImageIndex == 0) // arrow UP
            {
                grpEInfo.Height = 1;
                btnETerm.ImageIndex = 1;
            }
            else
            {
                grpEInfo.Height = 110;
                btnETerm.ImageIndex = 0;
            }

        }
        #endregion

        private void lsvENetwork_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (lsvENetwork.SelectedItems.Count > 0)
                {
                    // Load Network
                    string cblcode = lsvENetwork.SelectedItems[0].Text.Substring(1);
                    cblcode = cblcode.Remove(cblcode.IndexOf('[')).Trim();
                    if (isOnChange)
                    {
                        if (MessageBox.Show(this,
                            "Current changes not yet save.\r\nDo you want to proceed without saving",
                            "E-SIDE Pair Count", MessageBoxButtons.OKCancel) == DialogResult.Cancel)
                            return;
                    }
                    if (cblcode != lblECableCode.Text)
                    {
                        Cursor.Current = Cursors.WaitCursor;
                        btnETerm.Text = "Termination Points List [ Exc : " + isExcAbbE + "] [Cable Code : " + cblcode + "]";
                        isOnLoadMode = true;

                        //LoadETermination(

                        int mjFID = LoadETail(cblcode);
                        LoadETermination(mjFID, cblcode);

                        lblECableCode.Text = cblcode;
                        isOnLoadMode = false;
                    }
                }
            }
            catch (Exception ex)
            {
                log.WriteErr(ex);
            }
            finally
            {
                Cursor.Current = Cursors.Default;
            }

        }


        #region Menu Events
        private void mnuENew_Click(object sender, EventArgs e)
        {
            try
            {
                if (gERow == null)
                    MessageBox.Show(this, "Please select a row to copy");
                else
                {
                    isOnLoadMode = true;

                    int r = grdE.Rows.Count - 1;
                    grdE.Rows.Add();
                    grdE.Rows[r].Cells[0].Value = gERow.Cells[0].Value;
                    grdE.Rows[r].Cells[1].Value = gERow.Cells[1].Value;
                    grdE.Rows[r].Cells[2].Value = 0;
                    grdE.Rows[r].Cells[3].Value = 0;
                    grdE.Rows[r].Cells[4].Value = 0;
                    grdE.Rows[r].Cells[5].Value = 0;
                    grdE.Rows[r].Cells[6].Value = gERow.Cells[6].Value;
                    grdE.Rows[r].Cells[7].Value = gERow.Cells[7].Value;
                    grdE.Rows[r].Cells[8].Value = gERow.Cells[8].Value;
                    grdE.Rows[r].Cells[9].Value = gERow.Cells[9].Value;
                    grdE.Rows[r].Cells[10].Value = gERow.Cells[10].Value;
                    grdE.Rows[r].Tag = r;
                    gERow = grdE.Rows[r];


                    int cFID = myUtil.ParseInt(myUtil.CellValue(gERow.Cells[9].Value));
                    int tFID = myUtil.ParseInt(myUtil.CellValue(gERow.Cells[10].Value));

                    clsESIDE.TERMPOINT t = E_SIDE.termpoints[cFID];
                    string[] mdf = myUtil.CellValue(gERow.Cells[1].Value).Split('/');
                    clsESIDE.ESIDECount cnt = new clsESIDE.ESIDECount(tFID, cFID, ++t.cblCID, mdf[0], mdf[1]);
                    E_SIDE.termpoints[cFID] = t;

                    E_SIDE.eCounts.Add(r, cnt);
                    E_SIDE.ApplyCount(grdE, gERow,
                        myUtil.ParseInt(myUtil.CellValue(gERow.Cells[10].Value)),
                        myUtil.ParseInt(myUtil.CellValue(gERow.Cells[9].Value)),
                        mdf[0], mdf[1], 0, 0, 0, 0);

                    isOnChange = true;

                }
            }
            catch (Exception ex) { log.WriteErr(ex); }
            isOnLoadMode = false;

        }

        private void mnuEDel_Click(object sender, EventArgs e)
        {
            try
            {
                if (gERow == null)
                    MessageBox.Show(this, "Please select row to delete");
                else
                {
                    E_SIDE.DeleteRow(gERow);
                    grdE.Rows.Remove(gERow);
                }
            }
            catch (Exception ex) { log.WriteErr(ex); }
        }

        private void mnuEClearAll_Click(object sender, EventArgs e)
        {
            try
            {
                isOnLoadMode = true;
                foreach (DataGridViewRow r in grdE.Rows)
                {
                    if (r.Tag == null) continue;
                    string[] mdf = myUtil.CellValue(r.Cells[1].Value).Split('/');
                    E_SIDE.ApplyCount(grdE, r,
                        myUtil.ParseInt(myUtil.CellValue(r.Cells[10].Value)),
                        myUtil.ParseInt(myUtil.CellValue(r.Cells[9].Value)),
                        mdf[0], mdf[1], 0, 0, 0, 0);

                    r.Cells[2].Value = 0;
                    r.Cells[3].Value = 0;
                    r.Cells[4].Value = 0;
                    r.Cells[5].Value = 0;

                }
            }
            catch (Exception ex) { log.WriteErr(ex); }
            isOnLoadMode = false;
        }

        private void mnuEClearSelect_Click(object sender, EventArgs e)
        {
            try
            {
                isOnLoadMode = true;
                foreach (DataGridViewRow r in grdE.SelectedRows)
                {
                    if (r.Tag == null) continue;
                    string[] mdf = myUtil.CellValue(r.Cells[1].Value).Split('/');
                    E_SIDE.ApplyCount(grdE, r,
                        myUtil.ParseInt(myUtil.CellValue(r.Cells[10].Value)),
                        myUtil.ParseInt(myUtil.CellValue(r.Cells[9].Value)),
                        mdf[0], mdf[1], 0, 0, 0, 0);

                    r.Cells[2].Value = 0;
                    r.Cells[3].Value = 0;
                    r.Cells[4].Value = 0;
                    r.Cells[5].Value = 0;
                }
            }
            catch (Exception ex) { log.WriteErr(ex); }
            isOnLoadMode = false;
        }

        private void mnuESave_Click(object sender, EventArgs e)
        {
            try
            {
                bool all = false;
                if (sender.Equals(mnuESaveAll) || sender.Equals(mnuESavePlaceAll))
                    all = true;
                else if (grdE.SelectedRows.Count == 0)
                {
                    MessageBox.Show(this, "Please select rows to save");
                    return;
                }

                bool place = false;
                if (sender.Equals(mnuESavePlaceSelection) || sender.Equals(mnuESavePlaceAll)) place = true;

                Cursor.Current = Cursors.WaitCursor;
                if (E_SIDE.SavePairCount(grdE, lblEExc.Text, lblECableCode.Text, place, all))
                {
                    MessageBox.Show(this, "E-SIDE Pair Count already saved to NEPS");
                    isOnChange = false;
                }
            }
            catch (Exception ex) { log.WriteErr(ex); }
            Cursor.Current = Cursors.Default;
            prgE.Visible = false;
        }

        private void mnuEExit_Click(object sender, EventArgs e)
        {
            if (isOnChange)
            {
                if (MessageBox.Show(this, "Current setting not yet apply.\r\nAre you sure to exit now", "Pair Count", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    this.Close();
            }
            else
                this.Close();
        }


        private void mnuEHiliteCable_Click(object sender, EventArgs e)
        {
            try
            {
                int key = myUtil.ParseInt(gERow.Cells[9].Value.ToString());
                if (E_SIDE.termpoints.ContainsKey(key))
                {
                    hiliteFeature(E_SIDE.termpoints[key].cblFID, 7000, E_SIDE.termpoints[key].detailID, 400);
                }
            }
            catch (Exception ex)
            {
                log.WriteErr(ex);
            }
        }

        private void mnuEHiliteTerm_Click(object sender, EventArgs e)
        {
            try
            {
                int key = myUtil.ParseInt(gERow.Cells[9].Value.ToString());
                if (E_SIDE.termpoints.ContainsKey(key))
                {
                    if (E_SIDE.termpoints[key].termFNO == 10800) //its a splice -> stump or stub
                        hiliteFeature(E_SIDE.termpoints[key].cblFID, 7000, E_SIDE.termpoints[key].detailID, 200);
                    else
                        hiliteFeature(E_SIDE.termpoints[key].termFID, E_SIDE.termpoints[key].termFNO, E_SIDE.termpoints[key].detailID, 200);
                }
            }
            catch (Exception ex)
            {
                log.WriteErr(ex);
            }
        }

        #endregion

        #region Progress Bar
        public void InitEProgressBar(int val)
        {
            prgE.Maximum = val;
            prgE.Value = 0;
            prgE.Visible = true;
        }
        public void IncreaseEProgressBar(int val)
        {
            try
            {
                if (val == prgE.Maximum || val == -1)
                    prgE.Value = prgE.Maximum;
                else if (prgE.Value + val > prgE.Maximum)
                    prgE.Value = prgE.Maximum - 5;
                else if (val == 0)
                    prgE.Value = 0;
                else
                    prgE.Value += val;
                prgE.Visible = (prgE.Value != 0);

            }
            catch (Exception ex)
            {
                log.WriteErr(ex);
            }
            finally { Application.DoEvents(); }
        }
        #endregion

        private void grdE_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                int r = grdE.CurrentCell.RowIndex;
                int c = grdE.CurrentCell.ColumnIndex;

                if (c < 5)
                    grdE.CurrentCell = grdE.Rows[r].Cells[c + 1];
                else if (r == grdE.Rows.Count - 2)
                    grdE.CurrentCell = grdE.Rows[0].Cells[2];
                else
                    grdE.CurrentCell = grdE.Rows[r + 1].Cells[2];

            }
        }


        private void grdE_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            try
            {
                if (e.ColumnIndex < 6)
                {
                    try
                    {
                        int i = e.ColumnIndex;
                        if (grdE.Columns[i].HeaderCell.SortGlyphDirection == SortOrder.None ||
                            grdE.Columns[i].HeaderCell.SortGlyphDirection == SortOrder.Descending)
                        {
                            grdE.Sort(new DataGridColumnSorter(i, SortOrder.Ascending));
                            grdE.Columns[i].HeaderCell.SortGlyphDirection = SortOrder.Ascending;
                        }
                        else
                        {
                            grdE.Sort(new DataGridColumnSorter(i, SortOrder.Descending));
                            grdE.Columns[i].HeaderCell.SortGlyphDirection = SortOrder.Descending;
                        }
                    }
                    catch { }

                    Application.DoEvents();
                }
            }
            catch (Exception ex)
            {
                log.WriteErr(ex);
            }

        }
        #endregion

        #region DDP

        private void loadDDP(int isFID, short isFNO, string cableCode, string excAbb)
        {
            string strSQL = "";

            isOnLoadMode = true;
            isOnChange = false;

            isExcAbbE = excAbb;
            lblEExc.Text = excAbb;
            lblECableCode.Text = cableCode;

            E_SIDE = new clsESIDE(grdE, excAbb, cableCode);

            Cursor.Current = Cursors.WaitCursor;

            // Load Available Count for E-Side
            //int mjFID = LoadETail(cableCode);

            // Load Network
            #region Load Cable Code
            lsvENetwork.Items.Clear();

            int ddpFID = clsTrace.TraceUp_DPP(isFID);

            strSQL = "SELECT B.CABLE_CODE, SUM(F.CURRENT_HIGH - F.CURRENT_LOW + 1) TOTAL_COUNT "
            + "FROM GC_CBL A, GC_SPLICE B, GC_NR_CONNECT C, GC_VERTICAL D, GC_NETELEM E, GC_COUNT F "
            + "WHERE A.CABLE_CLASS = 'TAIL' AND E.EXC_ABB = '" + excAbb + "' AND A.G3E_FID = C.G3E_FID AND A.G3E_FID = E.G3E_FID "
            + "AND B.G3E_FID = C.OUT_FID AND D.G3E_FID = C.IN_FID AND A.G3E_FID = F.G3E_FID "
            + "AND B.CABLE_CODE IN (SELECT A.CABLE_CODE FROM GC_CBL A, GC_NR_CONNECT B WHERE A.G3E_FID = B.G3E_FID AND B.IN_FID = " + ddpFID.ToString() + ") "
            + "GROUP BY B.CABLE_CODE";

            ADODB.Recordset rsE = GTClassFactory.Create<IGTApplication>().DataContext.OpenRecordset(strSQL, ADODB.CursorTypeEnum.adOpenStatic, ADODB.LockTypeEnum.adLockReadOnly, 1);
            if (rsE.RecordCount > 0)
            {
                while (!rsE.EOF)
                {
                    string cblcode = myUtil.rsField(rsE, "CABLE_CODE");
                    log.WriteLog("Read Recordset : " + cblcode);
                    ListViewItem l = lsvENetwork.Items.Add("#" + cblcode + " [total MDF count : " + myUtil.rsField(rsE, "TOTAL_COUNT") + "]");
                    l.Tag = cblcode;
                    rsE.MoveNext();
                }

            }
            #endregion

            #region

            LoadETermination(ddpFID, cableCode);

            #endregion
            isOnLoadMode = false;
            isOnChange = false;

            pnlENetwork.Height = btnENetworkList.Height + 1;
            btnENetworkList.ImageIndex = 1;

            Cursor.Current = Cursors.Default;
        }

        #endregion

        #region Closing
        private void mnuClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void frmPairCount_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (isOnChange)
            {
                if (MessageBox.Show(this, "Current setting not yet apply.\r\nAre you sure to exit now", "Pair Count",
                    MessageBoxButtons.YesNo) == DialogResult.No)
                    e.Cancel = true;
            }
        }
        #endregion

        private void mnuDNew_Click(object sender, EventArgs e)
        {
            try
            {
                if (gDRow == null)
                    MessageBox.Show(this, "Please select a row to copy");
                else
                {
                    isOnLoadMode = true;

                    int cFID = myUtil.ParseInt(myUtil.CellValue(gDRow.Cells[7].Value));

                    clsDSIDE.PairCount d = D_SIDE.TermPoints[cFID];
                    d.SetCount(d.CID, 0, 0);
                    D_SIDE.TermPoints[cFID] = d;

                    int r = grdD.Rows.Count - 1;
                    grdD.Rows.Add();
                    grdD.Rows[r].Cells[0].Value = gDRow.Cells[0].Value;
                    grdD.Rows[r].Cells[1].Value = gDRow.Cells[1].Value;
                    grdD.Rows[r].Cells[2].Value = 0;
                    grdD.Rows[r].Cells[3].Value = 0;
                    grdD.Rows[r].Cells[4].Value = gDRow.Cells[4].Value;
                    grdD.Rows[r].Cells[5].Value = gDRow.Cells[5].Value;
                    grdD.Rows[r].Cells[6].Value = gDRow.Cells[6].Value;
                    grdD.Rows[r].Cells[7].Value = gDRow.Cells[7].Value;
                    grdD.Rows[r].Cells[8].Value = gDRow.Cells[8].Value;
                    grdD.Rows[r].Cells[9].Value = d.CID;
                    grdD.Rows[r].Tag = r;

                    isOnChange = true;
                }
            }
            catch (Exception ex) { log.WriteErr(ex); }
            isOnLoadMode = false;
        }

        private void mnuDDel_Click(object sender, EventArgs e)
        {
            try
            {
                if (gDRow == null)
                    MessageBox.Show(this, "Please select row to delete", "Fail to Delete Row");
                else if (gDRow.ReadOnly)
                    MessageBox.Show(this, "Selected row is readonly", "Fail to Delete Row");

                else
                {
                    isOnLoadMode = true;

                    int cFID = myUtil.ParseInt(myUtil.CellValue(gDRow.Cells[7].Value));
                    int CID = myUtil.ParseInt(myUtil.CellValue(gDRow.Cells[9].Value));

                    clsDSIDE.PairCount d = D_SIDE.TermPoints[cFID];

                    if (d.RemoveCount(CID))
                    {
                        grdD.Rows.Remove(gDRow);
                        D_SIDE.TermPoints[cFID] = d;
                    }
                    else
                        MessageBox.Show(this, "Cannot delete current row\r\nMake sure it is the last row in collection\r\nAnd it is not the only one");
                }
            }
            catch (Exception ex) { log.WriteErr(ex); }
        }

        #region Move Pair Count Label
        private void mnuEMoveCount_Click(object sender, EventArgs e)
        {
            if (gERow == null)
                MessageBox.Show(this, "Please select row to move count label", "Pair Count [Move Pair Count]");
            else
            {
                int FID = int.Parse(gERow.Cells["colECblFID"].Value.ToString());
                string label = myUtil.GetFieldValue("GC_CBL", "COUNT_ANNOTATION", FID);
                if (label.Length > 0)
                    GTPairCount.tempLabel = new clsMoveLabel(FID, label, "E-CABLE");
                else
                {
                    MessageBox.Show(this, "Selected cable does not have pair count", "Pair Count [Move Pair Count]");
                    GTPairCount.tempLabel.FlagMove = false;
                }
            }
        }

        private void mnuEMoveTransfer_Click(object sender, EventArgs e)
        {
            if (gERow == null)
                MessageBox.Show(this, "Please select row to move count transfer label", "Pair Count [Move Count Transfer]");
            else
            {
                int FID = int.Parse(gERow.Cells["colECblFID"].Value.ToString());
                string label = myUtil.GetFieldValue("GC_CBL", "COUNT_TRANSFER", FID);
                if (label.Length > 0)
                    GTPairCount.tempLabel = new clsMoveLabel(FID, label, "E-CABLE");
                else
                {
                    MessageBox.Show(this, "Selected cable does not have count transfer", "Pair Count [Move Count Transfer]");
                    GTPairCount.tempLabel.FlagMove = false;
                }
            }
        }

        private void mnuMoveCount_Click(object sender, EventArgs e)
        {
            if (gDRow == null)
                MessageBox.Show(this, "Please select row to move count label", "Pair Count [Move Pair Count]");
            else
            {
                int FID = int.Parse(gDRow.Cells["colDCblFID"].Value.ToString());
                string label = myUtil.GetFieldValue("GC_CBL", "COUNT_ANNOTATION", FID);
                if (label.Length > 0)
                    GTPairCount.tempLabel = new clsMoveLabel(FID, label, "D-CABLE");
                else
                {
                    MessageBox.Show(this, "Selected cable does not have pair count", "Pair Count [Move Pair Count]");
                    GTPairCount.tempLabel.FlagMove = false;
                }
            }
        }

        private void mnuMoveTransfer_Click(object sender, EventArgs e)
        {
            if (gDRow == null)
                MessageBox.Show(this, "Please select row to move count transfer label", "Pair Count [Move Count Transfer]");
            else
            {
                int FID = int.Parse(gDRow.Cells["colDCblFID"].Value.ToString());
                string label = myUtil.GetFieldValue("GC_CBL", "COUNT_TRANSFER", FID);
                if (label.Length > 0)
                    GTPairCount.tempLabel = new clsMoveLabel(FID, label, "D-CABLE");
                else
                {
                    GTPairCount.tempLabel.FlagMove = false;
                    MessageBox.Show(this, "Selected cable does not have count transfer", "Pair Count [Move Count Transfer]");
                }
            }

        }
        #endregion

        private void mnuUnassigned_Click(object sender, EventArgs e)
        {
            MessageBox.Show(this, D_SIDE.AvailableCount(), "Available Count");
        }

    }

}