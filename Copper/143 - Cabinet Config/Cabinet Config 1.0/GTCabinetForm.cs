/*
 * GTFRAC - FRAC configuration window
 * develop by : m.zam - azza solution (m.zam@azza4u.com)
 * start date : 01-MAR-2012
 * 
 * latest update : 10-MAY-2012
 * 1. change [BLOCK] to [Frame Unit]
 * 2. change [CABINET] to [Frame Container]
 * 3. re-arrange header information
 * 4. add menu on top of datagridview (complement the context menu)
 * 
 * latest update : 12-JUL-2012 (v1.0.3)
 * 1. add cabinet total size at header information 
 * 2. add validation condition -> total e-side + d-side <= cabinet total size
 * 3. g_cell = current edit cell 
 * -> before commit -> if g_cell in edit mode -> g_cell.commitedit 
 * -> this will make sure any value entered by user will be save to database
 * 4. add version at form title (current version 1.0.3)
 * -> easy to monitor which version used by user
 * 
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using ADODB;
using Intergraph.GTechnology.Interfaces;
using Intergraph.GTechnology.API;
using System.Diagnostics;
using System.Text.RegularExpressions;


namespace NEPS.GTechnology.Cabinet
{
    public partial class GTCabinetForm : Form
    {

        #region variables

        private IGTApplication m_gtapp = null;
        private IGTDataContext m_IGTDataContext = null;

        private clsCabinet FRAC = new clsCabinet();
        int FRAC_FID = -1;

        int D_Capacity = 0;
        int E_Capacity = 0;
        int DSL_IN_Cap = 0;
        int DSL_OT_Cap = 0;

        List<int> g_delrow = new List<int>(); // hold id fo deleted row
        List<object> g_copiedcell = new List<object>(); // hold values of copied cell
        int g_copiedcol = 0; // user copying a single col
        int g_copiedrow = 0; // user copying a single row


        private IGTDataContext m_GTDataContext = null;
        private bool isLoading = false;

        #endregion

        #region " Local Events "
        private void btnHeader_Click(object sender, EventArgs e)
        {
            if (btnHeader.ImageIndex == 0) // down arrow
            {
                btnHeader.ImageIndex = 1;
                pnlTop.Height = txtFRACLocation.Top + (int)(txtFRACLocation.Height * 1.5);
            }
            else
            {
                btnHeader.ImageIndex = 0;
                pnlTop.Height = btnHeader.Height;
            }
        }


        #endregion

        #region " General Events "

        private void entryForm_Enter(object sender, EventArgs e)
        {
            ((Control)sender).BackColor = Utilities.BackSelectedColor;
        }
        private void entryForm_Leave(object sender, EventArgs e)
        {
            ((Control)sender).BackColor = Utilities.BackNormalColor;
        }

        private void NumericTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            TextBox t = (TextBox)sender;
            bool dot = false;
            bool flag = false;
            if (e.KeyChar == '\b') return;
            if (e.KeyChar == '.' & t.Text.IndexOf('.') > 0) dot = true;
            if (e.KeyChar < '-' | e.KeyChar > '9' | dot == true) flag = true;
            if (t.Text.Length > 0 & e.KeyChar == '-') flag = true;
            if (e.KeyChar == '/') flag = true;
            e.Handled = flag;
        }

        private void cmbGT_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox cmb = (ComboBox)sender;
            try
            {
                if (cmb.SelectedIndex > -1)
                {
                    cmb.Tag = cmb.SelectedValue;
                }
                else
                {
                    cmb.Tag = -1;
                }
            }
            catch
            {
                cmb.Tag = -1;
            }
        }

        private int ParseInt(string val)
        {
            try
            {
                return int.Parse(val);
            }
            catch
            {
                return 0;
            }
        }


        #endregion

        #region " 2. Show FRAC dialog window "

        #region " 2.0 Initialize and Load Form "
        public GTCabinetForm()
        {
            try
            {
                InitializeComponent();

                m_IGTDataContext = GTClassFactory.Create<IGTApplication>().Application.DataContext;
                m_gtapp = GTClassFactory.Create<IGTApplication>().Application;
                m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Running XXX...");


                InitComboColumn(); // header              
                InitiateFRAC();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Test", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion

        #region " 2.1.1 Init Combobox Value - hardcoded "

        private void InitComboColumn()
        {
            this.colBlock.Items.Clear();
            this.colBlock.Items.Add("ESIDE");
            this.colBlock.Items.Add("DSIDE");
            this.colBlock.Items.Add("DSL-IN");
            this.colBlock.Items.Add("DSL-OUT");
            this.colBlock.Items.Add("DSL-IN-TIE");
            this.colBlock.Items.Add("DSL-OUT-TIE");

            this.colStatus.Items.Clear();
            this.colStatus.Items.Add("PLANNED");
            this.colStatus.Items.Add("IN SERVICE");
            this.colStatus.Items.Add("DECOMMISSIONED");
            InitComboItems(colStatus, "GC_ITFACE_BLK", "BLOCK_STATUS");
            this.colStatus.Items.Add("");

            this.colManufacturer.Items.Clear();
            InitComboItems(colManufacturer, "GC_ITFACE", "MODEL");
            InitComboItems(colManufacturer, "GC_ITFACE_BLK", "BLOCK_MANUFACTURER");
            this.colManufacturer.Items.Add("");

            this.colPosition.Items.Clear();
            this.colPosition.Items.Add("1");
            this.colPosition.Items.Add("2");
            this.colPosition.Items.Add("3"); 
            InitComboItems(colPosition, "GC_ITFACE_BLK", "BLOCK_POSITION");
            this.colPosition.Items.Add(""); 
            
        }

        private void InitComboItems(DataGridViewComboBoxColumn cmb, string tablename, string colname)
        {
            try
            {
                string ssql = "SELECT DISTINCT " + colname + " FROM " + tablename;
                this.m_GTDataContext = GTClassFactory.Create<IGTApplication>().DataContext;

                ADODB.Recordset rs = new ADODB.Recordset();
                rs = GTClassFactory.Create<IGTApplication>().DataContext.OpenRecordset(ssql,
                    ADODB.CursorTypeEnum.adOpenStatic, ADODB.LockTypeEnum.adLockOptimistic, 1);

                while (!rs.EOF)
                {
                    string val = CellValue(rs.Fields[0].Value).Trim();
                    if (val.Length > 0) 
                        if (!cmb.Items.Contains(val)) 
                            cmb.Items.Add(val);
                    rs.MoveNext();
                }
                rs.Close();
                rs = null;
            }
            catch { }
        }
        #endregion

        #region " 2.2 Initialize Application for FRAC Configuration "

        /// <summary>
        /// Initiate application to start FRAC configuration procedure. 
        /// Monitor mouse click for selected object. 
        /// Display FRAC information on the window if selected.
        /// 
        /// created : m.zam @ 01-04-2012
        /// </summary>
        private void InitiateFRAC()
        {
            GTClassFactory.Create<IGTApplication>().SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Initiate FRAC Configuration Procedure ....");

            // skip this step if FRAC is already selected
            if (!isFRACSelected())
            {
                // initialize G-Tech for convertion
                m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Select one cabinet to configure or Press Esc to cancel");
                m_gtapp.Application.ActiveMapWindow.MousePointer = GTMousePointerConstants.gtmwmpNWArrow;

            }
        }

        #endregion

        #endregion

        #region " 3. User Selected an RT Feauture by Mouse Click "
        public Boolean _mouseDown
        {
            set
            {
                if (isFRACSelected()) { }
            }
        }

        private bool isFRACSelected()
        {
            // make sure only one RT object is selected
//            GTClassFactory.Create<IGTApplication>().BeginWaitCursor();

            IGTSelectedObjects SelectedObjects = m_gtapp.SelectedObjects;
            switch (SelectedObjects.FeatureCount)
            {
                case 0:
                    return false;
                case 1:     // one feature selected
                    foreach (IGTDDCKeyObject oDDCKeyObject in SelectedObjects.GetObjects())
                    // notes for me : in one feature can have more than one object
                    {
                        if (oDDCKeyObject.FNO != clsCabinet.FRAC_FNO)
                        {
                            return false;
                        }

                        else
                        {
                            FRAC_FID = oDDCKeyObject.FID;
                            ShowFRAC_Object();
                            ShowFRAC_Elem(FRAC_FID);
                            ShowFRAC_Header(FRAC_FID);
                            Application.DoEvents();
                            ShowFRAC_FRAU();
                            return true;
                        }
                    }
                    return true;

                default: // more than one features selected
                    GTClassFactory.Create<IGTApplication>().SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "");
                    GTClassFactory.Create<IGTApplication>().Application.ActiveMapWindow.MousePointer = GTMousePointerConstants.gtmwmpNWArrow;

                    return false;
            }//switch

        }// isRTSelected

        #endregion

        #region " 5. Shows Details "

        private void ShowFRAC_Object()
        {
            //GTClassFactory.Create<IGTApplication>().BeginWaitCursor();
            IGTSelectedObjects SelectedObjects =
                GTClassFactory.Create<IGTApplication>().SelectedObjects;

            // get selected object properties
            foreach (IGTDDCKeyObject oDDCKeyObject in SelectedObjects.GetObjects())
            {
                ShowFRAC_Feature(oDDCKeyObject);

                ADODB.Recordset rs = new ADODB.Recordset();
                rs = oDDCKeyObject.Recordset;

                while (!rs.EOF) // notes for me : read all component under the RT features
                {
                    ShowFRAC_Component(rs);
                    rs.MoveNext();
                }
                rs.Close();
                rs = null;
            }
            GTClassFactory.Create<IGTApplication>().EndWaitCursor();
        }
        private void ShowFRAC_Feature(IGTDDCKeyObject oDDCKeyObject)
        {
        }
        private void ShowFRAC_Component(ADODB.Recordset rs)
        {
        }

        private void ShowFRAC_Header(int iFID)
        {
            try
            {
                ADODB.Recordset rs = new ADODB.Recordset();
                string ssql = "SELECT * FROM GC_ITFACE WHERE G3E_FID = " + iFID.ToString();

                rs = GTClassFactory.Create<IGTApplication>().DataContext.OpenRecordset(ssql, ADODB.CursorTypeEnum.adOpenStatic, ADODB.LockTypeEnum.adLockReadOnly, 1);
                this.txtFRACLocation.Text = CabinetAddress(CellValue(rs.Fields["ADDRESS1"].Value),
                    CellValue(rs.Fields["ADDRESS2"].Value), CellValue(rs.Fields["ADDRESS3"].Value));

                this.txtFRACType.Text = CellValue(rs.Fields["ITFACE_CLASS"].Value);
                this.txtFRACName.Text += 
                    (txtFRACType.Text == "SDF" ? " 999-" : " ") + CellValue(rs.Fields["ITFACE_CODE"].Value);
                this.txtFRACManufacturer.Text = CellValue(rs.Fields["ITFACE_TYPE"].Value);
                this.txtFRACDist.Text = CellValue(rs.Fields["DIST_FROM_EXC"].Value);
                this.txtFRACOffshore.Text = CellValue(rs.Fields["DIST_FROM_OFFSHORE"].Value);
                this.txtFRACStatus.Text = "PLANNED";
                this.txtFRACSize.Text = CellValue(rs.Fields["TOTAL_SIZE"].Value);

                E_Capacity = int.Parse(rs.Fields["E_CAPACITY"].ToString());
                D_Capacity = int.Parse(rs.Fields["D_CAPACITY"].ToString());
                DSL_IN_Cap = int.Parse(rs.Fields["DSL_IN_CAPACITY"].ToString());
                DSL_OT_Cap = int.Parse(rs.Fields["DSL_OUT_CAPACITY"].ToString());
            }
            catch (Exception ex)
            {
            }
        }

        private string CabinetAddress(string addr1, string addr2, string addr3)
        {
            addr1 = addr1.Trim();
            addr2 = addr2.Trim();
            addr3 = addr3.Trim();
            string addr = (addr1 != "***" && addr1.Length > 0 ? addr1 + " " : "");
            addr += (addr2 != "***" && addr2.Length > 0 ? addr2 + " " : "");
            addr += (addr3 != "***" && addr3.Length > 0 ? addr3 : "");
            return addr.Trim();
        }

        private void ShowFRAC_Elem(int iFID)
        {
            try
            {
                ADODB.Recordset rs = new ADODB.Recordset();
                string ssql = "SELECT * FROM GC_NETELEM WHERE G3E_FID = " + iFID.ToString();

                rs = GTClassFactory.Create<IGTApplication>().DataContext.OpenRecordset(ssql, ADODB.CursorTypeEnum.adOpenStatic, ADODB.LockTypeEnum.adLockReadOnly, 1);
                this.txtFRACName.Text = rs.Fields["EXC_ABB"].Value.ToString();
                this.txtFRACDate.Text = rs.Fields["YEAR_PLACED"].Value.ToString();
            }
            catch (Exception ex)
            {
                this.txtFRACName.Text = "-not define-";
            }
        }

        private void ShowFRAC_FRAU()
        {
            try
            {
                string ssql = "SELECT * FROM GC_ITFACE_BLK WHERE G3E_FID = " + FRAC_FID.ToString();
                this.m_GTDataContext = GTClassFactory.Create<IGTApplication>().DataContext;
                ADODB.Recordset rs = new ADODB.Recordset();
                rs = GTClassFactory.Create<IGTApplication>().DataContext.OpenRecordset(ssql,
                    ADODB.CursorTypeEnum.adOpenStatic, ADODB.LockTypeEnum.adLockOptimistic, 1);

                grdFRAC.Rows.Clear();
                int row = 0;
                while (!rs.EOF)
                {
                    row = grdFRAC.Rows.Add(); Application.DoEvents();
                    grdFRAC.Rows[row].Cells[0].Value = CellValue(rs.Fields["BLOCK_NAME"].Value);
                    grdFRAC.Rows[row].Cells[1].Value = CellValue(rs.Fields["BLOCK_POSITION"].Value);
                    grdFRAC.Rows[row].Cells[2].Value = ParseInt(CellValue(rs.Fields["BLOCK_SIZE"].Value));
                    grdFRAC.Rows[row].Cells[3].Value = CellValue(rs.Fields["BLOCK_STATUS"].Value);
                    grdFRAC.Rows[row].Cells[4].Value = CellValue(rs.Fields["BLOCK_MANUFACTURER"].Value);
                    grdFRAC.Rows[row].Tag = ParseInt(CellValue(rs.Fields["G3E_CID"].Value));

                    Debug.WriteLine(" ID : " + CellValue(rs.Fields["G3E_ID"].Value));
                    Debug.WriteLine("FID : " + CellValue(rs.Fields["G3E_FID"].Value));
                    Debug.WriteLine("FNO : " + CellValue(rs.Fields["G3E_FNO"].Value));
                    Debug.WriteLine("CID : " + CellValue(rs.Fields["G3E_CID"].Value));
                    Debug.WriteLine("CNO : " + CellValue(rs.Fields["G3E_CNO"].Value));

                    rs.MoveNext();
                }
                rs.Close();
                rs = null;
            }
            catch { }
        }

        private int GetTotalCapacity(string blk_name)
        {
            int capacity = 0;
            for (int r = 0; r < grdFRAC.Rows.Count; r++)
            {
                try
                {
                    if (grdFRAC.Rows[r].Cells[0].Value.ToString() == blk_name)
                    {
                        capacity += int.Parse(grdFRAC.Rows[r].Cells[2].Value.ToString());
                    }
                }
                catch { }
            }
            return capacity;
        }

        #endregion

        #region " 9. User edit FRAU "

        #region " 9.1 Context Menu - 18-MAY-2012 "

        private void ctxCopy_Click(object sender, EventArgs e)
        {
            try
            {
                int count = grdFRAC.SelectedCells.Count;
                g_copiedcell.Clear();
                if (count > 0)
                {
                    g_copiedcol = (grdFRAC.SelectedCells[0].ColumnIndex - grdFRAC.SelectedCells[count - 1].ColumnIndex) + 1;
                    g_copiedrow = (grdFRAC.SelectedCells[0].RowIndex - grdFRAC.SelectedCells[count - 1].RowIndex) + 1;

                    for (int i = 0; i < grdFRAC.SelectedCells.Count; i++)
                    {
                        g_copiedcell.Insert(i, grdFRAC.SelectedCells[i].Value);
                    }
                }
                else
                    MessageBox.Show("Please select cells to copy");
            }
            catch (Exception ex)
            {
                g_copiedcell.Clear();
                MessageBox.Show(ex.Message);
            }

        }

        private void ctxPaste_Click(object sender, EventArgs e)
        {
            try
            {
                int maxrow_index = grdFRAC.RowCount - 1;
                //int totalselect = grdFRAC.SelectedCells.Count;

                if (g_copiedcell.Count > 0)
                {
                    if (g_copiedcell.Count == 1)
                    {
                        // fill up selected column with copied value
                        if (grdFRAC.SelectedCells[0].RowIndex >= maxrow_index) grdFRAC.Rows.Add();
                        for (int i = 0; i < grdFRAC.SelectedCells.Count; i++)
                            grdFRAC.SelectedCells[i].Value = g_copiedcell[0];
                    }
                    else
                    {
                        int i = g_copiedcell.Count - 1;
                        int c = grdFRAC.SelectedCells[grdFRAC.SelectedCells.Count - 1].ColumnIndex;
                        int start_row = grdFRAC.SelectedCells[grdFRAC.SelectedCells.Count - 1].RowIndex;

                        if (start_row + g_copiedrow >= maxrow_index)
                        {

                            for (int rw = (start_row + g_copiedrow - maxrow_index);
                                rw > 0; rw--)
                            {
                                grdFRAC.Rows.Add();
                                maxrow_index++;
                            }
                        }
                        for (int col = 0; col < g_copiedcol; col++, c++)
                        {
                            int r = start_row;
                            for (int row = 0; row < g_copiedrow; row++, r++, i--)
                            {
                                grdFRAC.Rows[r].Cells[c].Value = g_copiedcell[i];
                            }
                        }
                    }
                }
                else
                    MessageBox.Show("Please select cells to copy");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void ctxClear_Click(object sender, EventArgs e)
        {
            try
            {
                this.TopMost = false;
                if (grdFRAC.SelectedCells.Count > 0)
                {
                    for (int i = 0; i < grdFRAC.SelectedCells.Count; i++)
                    {
                        if (grdFRAC.SelectedCells[i].ValueType == typeof(int))
                            grdFRAC.SelectedCells[i].Value = 0;

                        else
                            grdFRAC.SelectedCells[i].Value = "";
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error Filling Data to Cells\r\n" + ex.Message);
            }
            finally
            {
                this.TopMost = true;
            }
        }

        private void ctxFillCol_Click(object sender, EventArgs e)
        {
            try
            {
                this.TopMost = false;
                if (grdFRAC.SelectedCells.Count > 0)
                {
                    string val = InputBox.Show("Please enter value into selected column", "Fill Column", "", -1, -1).Trim();
                    if (val.Length > 0)
                    {
                        for (int i = 0; i < grdFRAC.SelectedCells.Count; i++)
                        {
                            if (grdFRAC.SelectedCells[i].ValueType == typeof(int))
                                grdFRAC.SelectedCells[i].Value = ParseInt(val);

                            else
                                grdFRAC.SelectedCells[i].Value = val;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error Filling Data to Cells\r\n" + ex.Message);
            }
            finally
            {
                this.TopMost = true;
            }
        }

        private void ctxAddRow_Click(object sender, EventArgs e)
        {
            try
            {
                this.TopMost = false;
                grdFRAC.Rows.Add();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Fail to add new record\r\n" + ex.Message);
            }
            finally
            {
                this.TopMost = true;
            }

        }

        private void ctxDelRow_Click(object sender, EventArgs e)
        {
            try
            {
                this.TopMost = false;
                if (grdFRAC.SelectedRows.Count == 0)
                {
                    MessageBox.Show("Please select rows to be deleted");
                }
                else if (MessageBox.Show("Please confirm to delete selected row",
                    "Delete Row", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    foreach (DataGridViewRow r in grdFRAC.SelectedRows)
                    {
                        int frauCID = (int)r.Tag;
                        grdFRAC.Rows.Remove(r);
                        if (frauCID > -1) g_delrow.Add(frauCID);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error while deleting record\r\n" + ex.Message);
            }
            finally
            {
                this.TopMost = true;
            }

        }

        private void ctxCommit_Click(object sender, EventArgs e)
        {
            try
            {
                this.TopMost = false;
                if (MessageBox.Show("Current changes will be save to database ?", "Commit", MessageBoxButtons.OKCancel) != DialogResult.OK)
                {
                    return;
                }

                if (gCell.IsInEditMode)
                {
                    DataGridViewDataErrorContexts c = new DataGridViewDataErrorContexts();
                    gCell.DataGridView.CommitEdit(c);
                }
                
                FRAC.DeleteFRAC(g_delrow, FRAC_FID);
                g_delrow.Clear();

                for (int i = 0; i < grdFRAC.Rows.Count - 1; i++)
                {
                    try
                    {
                        if ((int)grdFRAC.Rows[i].Tag == -1)
                        {
                            int cid = FRAC.InsertFRAU(grdFRAC.Rows[i], FRAC_FID);
                            grdFRAC.Rows[i].Tag = cid;
                        }
                        else
                            FRAC.UpdateFRAU(grdFRAC.Rows[i], FRAC_FID);
                    }
                    catch (Exception ex)
                    {
                        if (i < grdFRAC.Rows.Count - 1)
                            throw ex;
                    }
                }
                if (!UpdateTotalCapacity()) return; ;

                int iRecordsAffected;
                GTClassFactory.Create<IGTApplication>().DataContext.Execute(
                    "COMMIT", out iRecordsAffected, (int)ADODB.CommandTypeEnum.adCmdText);

                MessageBox.Show("Success : Changes has been saved to database");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error updating changes to database\r\n" + ex.Message, "FRAC COnfiguration", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                this.TopMost = true;
            }
        }

        private bool UpdateTotalCapacity()
        {
            int e_cap = GetTotalCapacity("ESIDE");
            int d_cap = GetTotalCapacity("DSIDE");
            int dsl_in = GetTotalCapacity("DSL-IN");
            int dsl_out = GetTotalCapacity("DSL-OUT");

            if ((e_cap + d_cap) > int.Parse(txtFRACSize.Text))
            {
                MessageBox.Show("ERROR : Total E-SIDE with D-SIDE capacity exceed Cabinet maximum capacity\r\n" +
                    "Please increase cabinet total size or reduce frame unit total capacity");
                return false;
            }
            else
            {
                string ssql = "UPDATE GC_ITFACE SET ";
                ssql += "E_CAPACITY = " + e_cap.ToString() + ", ";
                ssql += "D_CAPACITY = " + d_cap.ToString() + ", ";
                ssql += "DSL_IN_CAPACITY = " + dsl_in.ToString() + ", ";
                ssql += "DSL_OUT_CAPACITY = " + dsl_out.ToString() + " ";
                ssql += "WHERE G3E_FID = " + FRAC_FID.ToString();

                int iR;
                GTClassFactory.Create<IGTApplication>().DataContext.Execute(ssql, out iR, (int)ADODB.CommandTypeEnum.adCmdText);
                return true;
            }
        }

        private void ctxRollback_Click(object sender, EventArgs e)
        {
            this.TopMost = false;
            if (MessageBox.Show("Any changes after last commit will be cancel. Reload data from database", "Rollback", MessageBoxButtons.OKCancel) != DialogResult.OK)
            {
                return;
            }
            ShowFRAC_FRAU();
            this.TopMost = true;
        }
        #endregion

        #region " 9.2 new FRAU "

        private void grdFRAC_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            try
            {
                this.TopMost = false;
                int r = grdFRAC.Rows.Count - 2;
                grdFRAC.Rows[r].Tag = -1;
                grdFRAC.Rows[r].DefaultCellStyle.BackColor = (r % 2 == 0 ? Color.White : Color.LightGray);
                grdFRAC.Rows[r].Cells[3].Value = "PLANNED";
                grdFRAC.Rows[r].Cells[4].Value = this.txtFRACManufacturer.Text;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Adding new row : " + ex.Message);
            }
            this.TopMost = true;
        }

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
        #endregion

        #region "DataGrid Cell / ComboBox Event"
        //DataGridView grdEdit = new DataGridView();
        //Dictionary<int, DataGridViewRow> chgRow = new Dictionary<int, DataGridViewRow>();

        private void grdView_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                this.TopMost = false;
                if (e.RowIndex == -1) return;

                if (grdFRAC.Columns[e.ColumnIndex] is DataGridViewComboBoxColumn)
                {
                    string val = grdFRAC.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString();
                    DataGridViewComboBoxColumn cmbCol = (DataGridViewComboBoxColumn)grdFRAC.Columns[e.ColumnIndex];

                    if (!cmbCol.Items.Contains(val)) //check if item is already in drop down, if not, add it to all
                    {
                        {
                            cmbCol.Items.Add(val);
                            this.grdFRAC.CurrentCell.Value = val;
                        }
                    }
                }

            }
            catch (Exception ex) { /*MessageBox.Show(ex.Message);*/ }
            finally { this.TopMost = true; }
        }

        private void HandleEditShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            try
            {
                if (e.Control is ComboBox)
                {
                    ComboBox cbo = (ComboBox)e.Control;
                    if (cbo == null)
                    {
                        return;
                    }

                    cbo.DropDownStyle = ComboBoxStyle.DropDown;
                    cbo.Validating -= HandleComboBoxValidating;
                    cbo.Validating += HandleComboBoxValidating;
                }
            }
            catch (Exception ex)
            {
                this.TopMost = false;
                MessageBox.Show(ex.Message);
                this.TopMost = true;
            }
        }

        private void HandleComboBoxValidating(object sender, CancelEventArgs e)
        {
            DataGridViewComboBoxEditingControl combo = sender as DataGridViewComboBoxEditingControl;
            if (combo == null)
                return;

            if (!combo.Items.Contains(combo.Text)) //check if item is already in drop down, if not, add it to all
            {
                if (gCell.ColumnIndex == 0)
                    e.Cancel = true;
                else
                {
                    DataGridViewComboBoxColumn comboColumn = this.grdFRAC.Columns[this.grdFRAC.CurrentCell.ColumnIndex] as DataGridViewComboBoxColumn;
                    combo.Items.Add(combo.Text);
                    comboColumn.Items.Add(combo.Text);
                    this.grdFRAC.CurrentCell.Value = combo.Text;
                }
            }
        }

        DataGridViewCell gCell;
        private void grdFRAC_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            gCell = grdFRAC.Rows[e.RowIndex].Cells[e.ColumnIndex];
        }

        #endregion

        private void GTCabinetForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            GTClassFactory.Create<IGTApplication>().EndWaitCursor();
        }

        #endregion      

        private void pnlTop_Paint(object sender, PaintEventArgs e)
        {

        }


    }
}