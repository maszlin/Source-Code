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

namespace NEPS.OSP.COPPER.SERVICE.ASSIGN
{
    public partial class frmServiceAssign : Form
    {
        private IGTApplication m_gtapp = null;
        private IGTDataContext m_IGTDataContext = null;
        private double initX;
        private double initY;

        private const int isBldgFNO = 30000;
        private Int32 isBldgFID = 0;
        private Boolean isCallData = false;

        #region Form Service Assignment
        public frmServiceAssign()
        {
            try
            {
                InitializeComponent();
                this.FormClosing += new FormClosingEventHandler(this.frmFrame_FormClosing);
                m_gtapp = GTClassFactory.Create<IGTApplication>().Application;
                m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Running Service Assignment Management System...");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Running Service Assignment Management System", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void frmFrame_Shown(object sender, EventArgs e)
        {
            m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Running Service Assignment Management System...");
        }

        private void frmFrame_Activated(object sender, EventArgs e)
        {
            m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Running Service Assignment Management System...");
        }

        private void frmFrame_Load(object sender, EventArgs e)
        {
            this.m_IGTDataContext = GTClassFactory.Create<IGTApplication>().Application.DataContext;
            string filename = Application.StartupPath + "/NEPS.OSP.COPPER.SERVICE.ASSIGN.dll";
            this.Text = "Service Assignment [" + FileVersionInfo.GetVersionInfo(filename).FileVersion + "]";
            cmdServed.Enabled = false;
            cmdUnServed.Enabled = false;
        }

        private void frmFrame_FormClosing(object sender, FormClosingEventArgs e)
        {
            DialogResult retVal = MessageBox.Show("Are you sure to exit?", "Service Assignment Management System", MessageBoxButtons.YesNo);
            if (retVal == DialogResult.Yes)
            {
                e.Cancel = false;
            }
            else
            {
                e.Cancel = true;
            }
        }
        #endregion

        #region General Function
        private bool isNumeric(string val, System.Globalization.NumberStyles NumberStyle)
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

        #region Reset
        private void cmdNew_Click(object sender, EventArgs e)
        {
            resetGDS();
            unLoadData();
            cmdGDS.Enabled = true;
            this.tabControl1.SelectedTab = tpgInit;
        }
        #endregion

        #region GDS
        private void resetGDS()
        {
            txtGDSSegment.Text = "";
            txtGDSObjectID.Text = "";
            txtGDSBuildingCode.Text = "";
            txtGDSBuildingName.Text = "";
            txtGDSNoFloor.Text = "";
            isBldgFID = 0;
            cmdService.Enabled = false;
            grpService.Enabled = false;
            lblStep2.Enabled = false;
            lblStep3.Enabled = false;
            btnStep3.Enabled = false;


            cmdServed.Enabled = false;
            cmdUnServed.Enabled = false;
            resetService();
        }

        private void cmdGDS_Click(object sender, EventArgs e)
        {
            try
            {

                IGTSelectedObjects selFeature;  // selected Feature (any object on the map)

                if (GTClassFactory.Create<IGTApplication>().SelectedObjects.FeatureCount == 0)
                {
                    MessageBox.Show("No feature is selected. Please select one Building GDS feature", "Service Assignment Management System", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                selFeature = GTClassFactory.Create<IGTApplication>().SelectedObjects;

                if (selFeature.FeatureCount == 1)
                {
                    foreach (IGTDDCKeyObject oDDCKeyObject in selFeature.GetObjects())
                    {
                        if (oDDCKeyObject.FNO != isBldgFNO) //isBldgFNO = 30000. Check whether the selected object is Building GDS
                        {
                            MessageBox.Show("Please select only Building GDS feature", "Service Assignment Management System", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            resetGDS();
                            return;
                        }
                        else
                        {
                            string strSQL = "SELECT G3E_FID, OBJECTID, BUILDING_CODE, BUILDING_NAME, NUM_FLOOR, SEGMENT FROM GC_ADM_BLDG WHERE G3E_FID = " + oDDCKeyObject.FID;
                            ADODB.Recordset rsBldg = new ADODB.Recordset();
                            rsBldg = GTClassFactory.Create<IGTApplication>().DataContext.OpenRecordset(strSQL, ADODB.CursorTypeEnum.adOpenStatic, ADODB.LockTypeEnum.adLockReadOnly, 1);
                            if (rsBldg.RecordCount > 0)
                            {
                                isBldgFID = oDDCKeyObject.FID;
                                txtGDSObjectID.Text = rsBldg.Fields[1].Value.ToString();
                                txtGDSBuildingCode.Text = rsBldg.Fields[2].Value.ToString();
                                txtGDSBuildingName.Text = rsBldg.Fields[3].Value.ToString();
                                txtGDSNoFloor.Text = rsBldg.Fields[4].Value.ToString();
                                txtGDSSegment.Text = rsBldg.Fields[5].Value.ToString();
                                cmdService.Enabled = true;
                                grpService.Enabled = true;
                                lblStep2.Enabled = true;
                                cmdGDS.Enabled = false;
                                // LoadAptAll(txtGDSObjectID.Text);

                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Service Assignment Management System", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
        }
        #endregion

        #region Service - edited on 2-JUL-2013
        private void resetService()
        {
            txtServiceFID.Text = "";
            txtServiceFNO.Text = "";
            txtServiceType.Text = "";
            txtServiceExcAbb.Text = "";
            txtServiceCode.Text = "";
        }

        private void cmdService_Click(object sender, EventArgs e)
        {
            try
            {
                IGTSelectedObjects selFeature;  // selected Feature (any object on the map)
                clsServicePoint srvPoint = GTServiceAssign.m_Service;
                if (GTClassFactory.Create<IGTApplication>().SelectedObjects.FeatureCount == 0)
                {
                    MessageBox.Show(this, "No feature is selected. Please select a Service Point", "Service Assignment Management System", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                selFeature = GTClassFactory.Create<IGTApplication>().SelectedObjects;

                if (selFeature.FeatureCount == 1)
                {
                    isCallData = false;
                    foreach (IGTDDCKeyObject oDDCKeyObject in selFeature.GetObjects())
                    {
                        if (srvPoint.ValidServicePoint(oDDCKeyObject.FNO, oDDCKeyObject.FID))
                        {
                            if (srvPoint.GetParentCode())
                            {
                                if (srvPoint.PRT_TYPE == "CABINET")
                                    throw new System.Exception("Cabinet not a valid service point");

                                txtServiceFID.Text = oDDCKeyObject.FID.ToString();
                                txtServiceFNO.Text = oDDCKeyObject.FNO.ToString();
                                txtServiceType.Text = srvPoint.PRT_TYPE;
                                txtServiceCode.Text = srvPoint.PRT_CODE;
                                txtServiceExcAbb.Text = srvPoint.EXC_ABB;

                                if (isCallData == false)
                                {
                                    isCallData = true;
                                    unLoadData();

                                    LoadServices(srvPoint);
                                    LoadAptStatus(srvPoint);

                                }
                                cmdService.Enabled = false;
                                cmdServed.Enabled = true;
                                cmdUnServed.Enabled = true;
                                lblStep3.Enabled = true;
                                btnStep3.Enabled = true;
                                return;
                            }
                        }
                        else
                            throw new System.Exception("Please select a valid service point");

                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "Service Assignment Management System", MessageBoxButtons.OK, MessageBoxIcon.Error);
                resetService();
            }

        }

        #endregion

        #region Un-Load Data
        private void unLoadData()
        {
            grdSP.Rows.Clear();
            grdUnServed.Rows.Clear();
            grdServed.Rows.Clear();
        }


        #endregion

        #region EDITED by M.ZAM @ 20-05-2013 / 2-JUL-2013

        #region Edit DP Grid - edited
        bool srvLoading = false;

        bool cellEditing;
        private void grdServices_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            cellEditing = true;
        }

        private void grdServices_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (!srvLoading && cellEditing && grdSP.Columns[e.ColumnIndex].Name == "colSPFloor")
            {
                grdSP.Rows[e.RowIndex].Cells["colSPUpdate"].Value = 0;
                cellEditing = false;
            }
        }



        #endregion

        #region Apartment Listing - edited on 2-JUL-2013

        private void LoadAptStatus(clsServicePoint sp)
        {
            try
            {
                string strSQL = "";
                Int32 i = 0;

                // Load Apartment List
                grdUnServed.Rows.Clear();
                grdServed.Rows.Clear();
                i = 0;

                strSQL = "SELECT A.OBJECTID, A.FLOOR_NUM, A.APT_NUM, A.SEGMENT, " +
                    " (SELECT B.G3E_FID FROM GC_DP_ASSIGNMENT B WHERE A.OBJECTID = B.OBJECTID AND A.SEGMENT = B.SEGMENT_GDS AND A.G3E_FID=B.G3E_FID) SERV_FID" +
                    " FROM GC_ADM_ADDR A" +
                    " WHERE A.BUILDING_ID = " + this.txtGDSObjectID.Text +
                    " AND A.SEGMENT = '" + txtGDSSegment.Text + "'" +
                    " ORDER BY A.OBJECTID";
                System.Diagnostics.Debug.WriteLine(strSQL);
                ADODB.Recordset rs = new ADODB.Recordset();
                rs = GTClassFactory.Create<IGTApplication>().DataContext.OpenRecordset(strSQL, ADODB.CursorTypeEnum.adOpenStatic, ADODB.LockTypeEnum.adLockReadOnly, 1);
                if (rs.RecordCount > 0)
                {
                    do
                    {
                        int SERV_FID = this.ReadDBInt(rs, "SERV_FID");
                        if (SERV_FID == -1) // unserve apartment
                        {
                            i = grdUnServed.Rows.Count;
                            grdUnServed.Rows.Add();
                            grdUnServed.Rows[i].Cells["colAptID"].Value = rs.Fields["OBJECTID"].Value.ToString();
                            grdUnServed.Rows[i].Cells["colAptFloor"].Value = rs.Fields["FLOOR_NUM"].Value.ToString();
                            grdUnServed.Rows[i].Cells["colAptNum"].Value = rs.Fields["APT_NUM"].Value.ToString();
                        }
                        else // served apartment
                        {
                            ADODB.Recordset rsSrv = new ADODB.Recordset();
                            string ssql = "SELECT '{0} '||{1} SRV_CODE FROM {2} WHERE G3E_FID = " + SERV_FID;
                            rsSrv = myUtil.ADODB_ExecuteQuery(string.Format(ssql, sp.SERV_TYPE, sp.SERV_CODE_LBL, sp.SERV_TABLE));

                            i = grdServed.Rows.Count;
                            grdServed.Rows.Add();
                            grdServed.Rows[i].Cells["colSvrAptID"].Value = rs.Fields["OBJECTID"].Value.ToString();
                            grdServed.Rows[i].Cells["colSvrFloor"].Value = rs.Fields["FLOOR_NUM"].Value.ToString();
                            grdServed.Rows[i].Cells["colSvrAptNum"].Value = rs.Fields["APT_NUM"].Value.ToString();
                            grdServed.Rows[i].Cells["colSvrFID"].Value = SERV_FID.ToString();

                            if (!rsSrv.EOF)
                                grdServed.Rows[i].Cells["colSvrCode"].Value = rsSrv.Fields["SRV_CODE"].Value.ToString();

                            else
                                grdServed.Rows[i].Cells["colSvrCode"].Value = "ERROR";

                            grdServed.Rows[i].Cells["colSvrUpdate"].Value = "1"; //updated is TRUE


                        }
                        rs.MoveNext();
                    }
                    while (!rs.EOF);
                }
                if (grdServed.Rows.Count > 0) grdServed.Rows[0].Selected = true;
                if (grdUnServed.Rows.Count > 0) grdUnServed.Rows[0].Selected = true;
                rs = null;
                strSQL = "";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Service Assignment Management System", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
        }

        #endregion

        #region Helper Functions / Events

        private void grdView_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            try
            {
                DataGridView dg = (DataGridView)sender;
                int i = e.ColumnIndex;
                if (dg.Columns[i].HeaderCell.SortGlyphDirection == SortOrder.None ||
                    dg.Columns[i].HeaderCell.SortGlyphDirection == SortOrder.Descending)
                {
                    dg.Sort(new DataGridColumnSorter(i, SortOrder.Ascending));
                    dg.Columns[i].HeaderCell.SortGlyphDirection = SortOrder.Ascending;
                }
                else
                {
                    dg.Sort(new DataGridColumnSorter(i, SortOrder.Descending));
                    dg.Columns[i].HeaderCell.SortGlyphDirection = SortOrder.Descending;
                }
            }
            catch { }

            Application.DoEvents();
        }

        private int ReadDBInt(ADODB.Recordset rs, string fieldname)
        {
            try
            {
                return int.Parse(rs.Fields[fieldname].Value.ToString());
            }
            catch
            {
                return -1;
            }
        }

        #endregion

        #region Button Events (Served / UnServed) - edited
        private void cmdServed_Click(object sender, EventArgs e)
        {
            if (grdSP.SelectedRows.Count == 0)
                MessageBox.Show(this, "Please select a service");

            else if (grdUnServed.SelectedRows.Count == 0)
                MessageBox.Show(this, "Please select an apartment unit");

            else
            {
                DataGridViewRow r2 = grdSP.SelectedRows[0];
                int count = myUtil.ParseInt(r2.Cells["colSPUnitCnt"].Value.ToString());
                for (int r = grdUnServed.SelectedRows.Count; r > 0; )
                {
                    DataGridViewRow r1 = grdUnServed.SelectedRows[--r];

                    grdServed.Rows.Add();
                    DataGridViewRow r3 = grdServed.Rows[grdServed.Rows.Count - 1];
                    r3.Cells["colSvrAptID"].Value = r1.Cells["colAptID"].Value.ToString(); // APT ID (OBJECT ID)
                    r3.Cells["colSvrFloor"].Value = r1.Cells["colAptFloor"].Value.ToString(); // FLOOR NUM
                    r3.Cells["colSvrAptNum"].Value = r1.Cells["colAptNum"].Value.ToString(); // APT NUM

                    r3.Cells["colSvrFID"].Value = r2.Cells["colSPFID"].Value.ToString(); // DP FID / SDF FID
                    r3.Cells["colSvrCode"].Value = r2.Cells["colSPCode"].Value.ToString(); // DP NUMBER / SDF CODE
                    r3.Cells["colSvrBndFID"].Value = r2.Cells["colSPBndFID"].Value.ToString();
                    r3.Cells["colSvrUpdate"].Value = "0"; //updates

                    count++;
                    grdUnServed.Rows.Remove(r1);

                }

                r2.Cells["colSPUnitCnt"].Value = count.ToString();
                foreach (DataGridViewRow r in grdServed.SelectedRows)
                    r.Selected = false;

            }
        }

        private void cmdUnServed_Click(object sender, EventArgs e)
        {
            try
            {
                if (grdServed.Rows.Count > 0)
                {
                    for (int r = grdServed.SelectedRows.Count; r > 0; )
                    {
                        DataGridViewRow r1 = grdServed.SelectedRows[--r];

                        grdUnServed.Rows.Add();
                        DataGridViewRow r2 = grdUnServed.Rows[grdUnServed.Rows.Count - 1];
                        r2.Cells["colAptID"].Value = r1.Cells["colSvrAptID"].Value.ToString(); // APT ID (OBJECT ID)
                        r2.Cells["colAptFloor"].Value = r1.Cells["colSvrFloor"].Value.ToString(); // FLOOR NUM
                        r2.Cells["colAptNum"].Value = r1.Cells["colSvrAptNum"].Value.ToString(); // APT NUM

                        DecreaseUnitCount(r1.Cells["colSvrFID"].Value.ToString());
                        DeleteServAssignment(r1);
                    }

                    foreach (DataGridViewRow r in grdServed.SelectedRows)
                        r.Selected = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "Service Assignment - unserve error");
                unLoadData();
                LoadServices(GTServiceAssign.m_Service);
                LoadAptStatus(GTServiceAssign.m_Service);
            }
        }

        private void DecreaseUnitCount(string dpFID)
        {
            foreach (DataGridViewRow r in grdSP.Rows)
            {
                if (r.Cells["colSPFID"].Value.ToString() == dpFID)
                {
                    int count = myUtil.ParseInt(r.Cells["colSPUnitCnt"].Value.ToString());
                    count--;
                    r.Cells["colSPUnitCnt"].Value = count.ToString();
                    return;
                }
            }
        }

        #endregion

        #region Button Events (Save / Report / Cancel)
        private void btnSave_Click(object sender, EventArgs e)
        {
            int countsave = 0;
            clsServicePoint sp = GTServiceAssign.m_Service;
            clsServAssignment sa;
            sa = new clsServAssignment(txtGDSObjectID.Text, txtGDSSegment.Text, sp.SERV_FNO, sp.SERV_CNO);
            
            clsServBoundary Bnd = new clsServBoundary(sp);
            clsAddress Addr = new clsAddress(txtGDSObjectID.Text, txtGDSSegment.Text);
            
            
            Cursor.Current = Cursors.WaitCursor;
            this.Cursor = Cursors.WaitCursor;
            Application.DoEvents();
           
            try
            {
                GTServiceAssign.m_oIGTTransactionManager.Begin("SavingRecords");
                m_gtapp.BeginWaitCursor();

                foreach (DataGridViewRow r in grdSP.Rows)
                {
                    System.Diagnostics.Debug.WriteLine(r.Cells["colSPUpdate"].Value.ToString());
                  //  if (r.Cells["colSPUpdate"].Value.ToString() == "0")
                  //  {
                        int FID = myUtil.ParseInt(r.Cells["colSPFID"].Value.ToString());
                        Addr.FloorNum = r.Cells["colSPFloor"].Value.ToString();
                        Addr.Save_GC_Address(sp.SERV_FNO, FID);
                        r.Cells["colSPUpdate"].Value = "1";
                  //  }
                }
                System.Diagnostics.Debug.WriteLine("Debug 5");
            
                for (int i = 0; i < grdServed.Rows.Count; i++)
                {
                    DataGridViewRow r = grdServed.Rows[i];
                    System.Diagnostics.Debug.WriteLine("Debug 6");
            
                    if (r.Cells["colSvrUpdate"].Value.ToString() == "0")
                    {
                       
                        int dpFID = myUtil.ParseInt(r.Cells["colSvrFID"].Value.ToString());

                        int bndFID = myUtil.ParseInt(r.Cells["colSvrBndFID"].Value.ToString());
                        System.Diagnostics.Debug.WriteLine("Debug 9");
                        if (bndFID == 0)
                        {
                            System.Diagnostics.Debug.WriteLine("Debug 10 : " + dpFID);
                            bndFID = Bnd.GetServBoundary(dpFID);
                            System.Diagnostics.Debug.WriteLine("Debug 11");
                            CopyBoundryFID(dpFID, bndFID);

                        }
                        
            
                        sa.SaveServiceAssignment(r);
                        Addr.FloorNum = r.Cells["colSvrFloor"].Value.ToString();
                        Addr.AptNum = r.Cells["colSvrAptNum"].Value.ToString();
                        Addr.AptID = r.Cells["colSvrAptID"].Value.ToString();
                        Addr.Save_GC_ServiceAddress(bndFID);

                        r.Cells["colSvrUpdate"].Value = "1";
                        countsave++;
                    }
                }
                System.Diagnostics.Debug.WriteLine("Debug 8");
            
                GTServiceAssign.m_oIGTTransactionManager.Commit();
                GTServiceAssign.m_oIGTTransactionManager.RefreshDatabaseChanges();

                MessageBox.Show(this, "Record saved to database", "DP Service Assignment");
            }
            catch (Exception ex)
            {
                GTServiceAssign.m_oIGTTransactionManager.Rollback();
                MessageBox.Show(this, ex.Message, "Service Assignment - Save Error");
            }
            m_gtapp.EndWaitCursor();
            Cursor.Current = Cursors.Default;
            this.Cursor = Cursors.Default;
        }

        private void btnReport_Click(object sender, EventArgs e)
        {
            try
            {
                clsAddress cAddress = new clsAddress(txtGDSObjectID.Text, txtGDSSegment.Text);

                clsReport.buildingid = this.txtGDSObjectID.Text;
                clsReport.buildingname = this.txtGDSBuildingName.Text;
                clsReport.buildingfloor = this.txtGDSNoFloor.Text;
                clsReport.segment = this.txtGDSSegment.Text;
                clsReport.address = cAddress.PrintBuildingAddress();

                clsReport.service_type = this.txtServiceType.Text;
                clsReport.service_code = this.txtServiceCode.Text;
                clsReport.exc_abb = this.txtServiceExcAbb.Text;

                string filename = clsReport.CreateReport(grdSP);
                Process.Start(filename);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, "Error creating the report\r\n" + ex.Message, "DP Service Assignment");
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            unLoadData();
            LoadServices(GTServiceAssign.m_Service);
            LoadAptStatus((GTServiceAssign.m_Service));
        }
        #endregion

        #region Tab Events
        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        #endregion

        #region Database Related - Save / Delete Assignment

        private void DeleteServAssignment(DataGridViewRow r)
        {
            int srvFID = Convert.ToInt32(r.Cells["colSvrFID"].Value.ToString());
            int aptID = Convert.ToInt32(r.Cells["colSvrAptID"].Value.ToString());

            #region remove from GC_DP_ASSIGMENT
            string ssql = "DELETE FROM GC_DP_ASSIGNMENT WHERE G3E_FID = " + srvFID + " AND OBJECTID = " + aptID;
            myUtil.ADODB_ExecuteNonQuery(ssql);
            #endregion

            #region remove from GC_SERVICEADDRESS
            ADODB.Recordset rs = myUtil.FindFID("GC_BNDTERM", srvFID);
            int bndFID = myUtil.ParseInt(myUtil.rsField(rs, "G3E_OWNERFID"));
            if (bndFID > 0)
                clsAddress.Delete_GC_ServiceAddress(bndFID, aptID);
            #endregion

            grdServed.Rows.Remove(r);
        }

        #endregion

        #region Steps
        private void lblStep1_EnabledChanged(object sender, EventArgs e)
        {
            lblStep1.ForeColor = (lblStep1.Enabled ? Color.Black : Color.Gray);
        }
        private void lblStep2_EnabledChanged(object sender, EventArgs e)
        {
            lblStep2.ForeColor = (lblStep2.Enabled ? Color.Black : Color.Gray);
            if (lblStep2.Enabled) lblStep1.ForeColor = Color.Gray;
        }
        private void lblStep3_EnabledChanged(object sender, EventArgs e)
        {
            lblStep3.ForeColor = (lblStep2.Enabled ? Color.Black : Color.Gray);
            if (lblStep3.Enabled) lblStep2.ForeColor = Color.Gray;
        }
        private void btnStep3_Click(object sender, EventArgs e)
        {
            //tpgApt.BringToFront();
            this.tabControl1.SelectedTab = tpgAssign;
        }
        #endregion

        #region Form Closing
        private void frmServiceAssign_FormClosing(object sender, FormClosingEventArgs e)
        {
            foreach (DataGridViewRow r in grdSP.Rows)
                if (r.Cells["colSPUpdate"].Value.ToString() == "0") // not updated assignment
                {
                    if (MessageBox.Show(this, "You have unsave records" + Environment.NewLine +
                        "Do you want to continue closing without saving", "DP Service Assignment",
                        MessageBoxButtons.OKCancel) == DialogResult.Cancel)
                    {
                        e.Cancel = true;
                    }
                    return; // exit foreach
                }

            foreach (DataGridViewRow r in grdServed.Rows)
                if (r.Cells["colSvrUpdate"].Value.ToString() == "0") // not updated assignment
                {
                    if (MessageBox.Show(this, "You have unsave records" + Environment.NewLine +
                        "Do you want to continue closing without saving", "DP Service Assignment",
                        MessageBoxButtons.OKCancel) == DialogResult.Cancel)
                    {
                        e.Cancel = true;
                    }
                    return; // exit foreach
                }
        }

        #endregion

        #endregion

        #region edited on 03-JUL-2013 by m.zam to handle DB configuration
        private void LoadServices(clsServicePoint sp)
        {
            ChangeLabel(sp);

            srvLoading = true;
            try
            {
                sp.GetServices(this.grdSP);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "Loading Services");
            }
            srvLoading = false;
            cellEditing = false;

        }

        private void ChangeLabel(clsServicePoint sp)
        {
            lblDP.Text = sp.SERV_NAME_LBL;
            string lbl = sp.SERV_CODE_LBL.Replace("_", " ");
            grdSP.Columns["colSPCode"].HeaderText = lbl;
            grdServed.Columns["colSvrCode"].HeaderText = lbl;
        }

        private void CopyBoundryFID(int spFID, int bndFID)
        {
            foreach (DataGridViewRow r in this.grdSP.Rows)
            {
                if (r.Cells["colSPFID"].Value.ToString() == spFID.ToString())
                    r.Cells["colSPBndFID"].Value = bndFID;
            }
            foreach (DataGridViewRow r in this.grdServed.Rows)
            {
                if (r.Cells["colSvrFID"].Value.ToString() == spFID.ToString())
                    r.Cells["colSvrBndFID"].Value = bndFID;
            }

        }
        #endregion

    }
}