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

namespace NEPS.OSP.COPPER.COLLECT.COUNT
{
    public partial class frmCollectCount : Form
    {
        #region Variables

        clsESIDE cblESrc;
        clsESIDE cblERcv;
        clsDSIDE cblDSrc;
        clsDSIDE cblDRcv;

        #endregion

        public frmCollectCount()
        {
            InitializeComponent();
            StatusMsg(appstatus.init);
            InitD();
            InitE();
        }

        #region D-SIDE COLLECT COUNT
        private void InitD()
        {
            btnDSrc.Enabled = true;
            btnDRcv.Enabled = false;
            btnDCollect.Enabled = false;
            btnDCancel.Enabled = false;

            txtDSrcDesc.Text = "";
            txtDSrcCount.Text = "";
            txtDHi.Text = "";
            txtDLo.Text = "";

            txtDRcvDesc.Text = "";
            txtDRcvCount.Text = "";
            txtDCount.Text = "";

            grpDCollect.Enabled = false;

            StatusMsg(appstatus.select_source);
        }

        #region Button Events
        private void btnDSrc_Click(object sender, EventArgs e)
        {
            try
            {
                InitD();
                cblDSrc = new clsDSIDE();

                GTClassFactory.Create<IGTApplication>().SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Checking selected feature");
                IGTSelectedObjects selFeature = GTClassFactory.Create<IGTApplication>().SelectedObjects;

                if (cblDSrc.isCable(selFeature))
                {
                    cblDSrc.ReadCable(selFeature.GetObjects()[0].FID);
                    if (cblDSrc.CABLE_CLASS == "E-CABLE")
                        throw new System.Exception("Please selected a D-SIDE cable");
                    else if (cblDSrc.FEATURE_STATE != "ASB" && cblDSrc.FEATURE_STATE != "MOD")
                        throw new System.Exception("Source state must be either AS-BUILT or MOD");

                    txtDSrcDesc.Text = cblDSrc.ToString();
                    txtDSrcCount.Text = cblDSrc.COUNT_ANNO;
                    btnDRcv.Enabled = true;
                    btnDCancel.Enabled = true;
                }
                else
                    throw new System.Exception("Cable must end with either DP, STUB or STUMP");

                StatusMsg(appstatus.select_receiver);

            }
            catch (Exception ex)
            {
                MessageBox.Show(this, "Fail to select source cable\r\n" + ex.Message, "Pick Source");
            }//end try catch
        }

        private void btnDRcv_Click(object sender, EventArgs e)
        {
            try
            {
                cblDRcv = new clsDSIDE();

                GTClassFactory.Create<IGTApplication>().SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Checking selected feature");
                IGTSelectedObjects selFeature = GTClassFactory.Create<IGTApplication>().SelectedObjects;

                if (cblDRcv.isCable(selFeature))
                {
                    cblDRcv.ReadCable(selFeature.GetObjects()[0].FID);
                    if (!ValidateCableProperties(cblDSrc, cblDRcv)) return;
                    if (cblDRcv.CABLE_CLASS == "E-CABLE")
                        throw new System.Exception("Please select a D-SIDE cable");

                    txtDRcvDesc.Text = cblDRcv.ToString();
                    txtDRcvCount.Text = cblDRcv.COUNT_ANNO;
                    btnDCollect.Enabled = true;
                    grpDCollect.Enabled = true;
                }
                else
                {
                    MessageBox.Show(this, "Cable must end with either DP, STUB or STUMP", "Pick Receiver");
                    txtDRcvDesc.Text = "";
                    txtDRcvCount.Text = "";
                    btnDCollect.Enabled = false;
                    grpDCollect.Enabled = false;
                }

                StatusMsg(appstatus.set_count);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, "Fail to select receiver cable\r\n" + ex.Message, "Pick Receiver");
            }//end try catch
        }

        private void btnDCancel_Click(object sender, EventArgs e)
        {
            StatusMsg(appstatus.closing);
            if (btnDCancel.Text == "Close")
                this.Close();
            else if (MessageBox.Show("Press [CANCEL] to cancel\r\nor press [OK] to close Collect Count",
                "Collect Count", MessageBoxButtons.OKCancel) == DialogResult.Cancel)

                InitD();
            else
                this.Close();
        }

        private void btnDCollect_Click(object sender, EventArgs e)
        {
            try
            {
                if (!ValidateCollectCount()) return;

                int lo = int.Parse(txtDLo.Text);
                int hi = int.Parse(txtDHi.Text);
                int cid = cblDSrc.isDSideCountAvailable(lo, hi);

                if (cid > -1)
                {
                    StatusMsg(appstatus.collect_count);

                    DoCollectCount_DSide(lo, hi, cid);
                    if (txtDRcvCount.Text.Length > 0)
                        txtDRcvCount.Text += Environment.NewLine + txtDLo.Text + "-" + txtDHi.Text;
                    else
                        txtDRcvCount.Text = txtDLo.Text + "-" + txtDHi.Text;
                    txtDHi.Text = "";
                    txtDLo.Text = "";
                    grpDCollect.Enabled = false;
                    btnDCollect.Enabled = false;
                    btnDRcv.Enabled = false;
                    btnDSrc.Enabled = true;
                    StatusMsg(appstatus.complete_count);
                }
                else
                    MessageBox.Show(this, "Collect count fail\r\nCount high and low not available in the source",
                        "Collect Count");
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, "Collect count fail\r\n" + ex.Message, "Collect Count");
            }
        }

        private void DoCollectCount_DSide(int lo, int hi, int cid)
        {
            try
            {
                GTCollectCount.m_oIGTTransactionManager.Begin("CollectCount");

                cblDSrc.TransferCountDSide(lo, hi, cid);
                cblDRcv.ReceiveCount(lo, hi);

                clsSaveCount.DeletePairCount(cblDSrc);
                clsSaveCount.SavePairCountDSide(cblDSrc, "ORIG");

                clsSaveCount.DeletePairCount(cblDRcv);
                clsSaveCount.SavePairCountDSide(cblDRcv, "CC");

                clsSaveCount.PlacePairCountLabel(cblDRcv);

                GTCollectCount.m_oIGTTransactionManager.Commit();
                GTCollectCount.m_oIGTTransactionManager.RefreshDatabaseChanges();
                MessageBox.Show(this, "Collect count success", "Collect Count");
            }
            catch (Exception ex)
            {
                GTCollectCount.m_oIGTTransactionManager.Rollback();
                MessageBox.Show(this, "Collect count fail\r\n" + ex.Message, "Collect Count");
            }
        }

        #endregion

        #region TextBox Events
        private void txtNumeric_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Tab || e.KeyChar == (char)Keys.Enter)
                this.SelectNextControl((Control)sender, true, true, true, true);

            //else if (e.KeyChar == (char)Keys.Back)
            //   this.SelectNextControl((Control)sender, false, true, true, true);

            else if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
                e.Handled = true;

        }

        private void txtDCount_Enter(object sender, EventArgs e)
        {
            btnDCollect.Focus();
        }

        private void txtDCount_TextChanged(object sender, EventArgs e)
        {
            try
            {
                int lo = int.Parse(txtDLo.Text);
                int hi = int.Parse(txtDHi.Text);
                btnDCollect.Enabled = (lo <= hi);
                int count = (hi - lo) + 1;
                if (hi == 0) count = 0;
                this.txtDCount.Text = count.ToString();
            }
            catch
            {
                btnDCollect.Enabled = false;
                this.txtDCount.Text = "0";
            }
        }
        #endregion

        #region Validation

        private bool ValidateCollectCount()
        {
            try
            {
                int lo = int.Parse(txtDLo.Text);
                int hi = int.Parse(txtDHi.Text);

                if (hi < lo)
                    MessageBox.Show(this, "High count must not be lower than low count", "Error : Collect Count");
                else if (cblDRcv.EFFECTIVE_PAIRS < hi - lo + 1)
                    MessageBox.Show(this, "Total count is bigger than receiver effective pairs", "Error : Collect Count");
                else
                    return true;

                return false;
            }
            catch (Exception ex)
            {
                if (ex.Message == "Input string was not in a correct format.")
                {
                    MessageBox.Show(this, "Please enter pair low and high properly\r\n" + ex.Message, "Error : Collect Count");
                }
                return false;
            }

        }
        #endregion

        #endregion

        #region E-SIDE COLLECT COUNT
        private void InitE()
        {
            btnESrc.Enabled = true;
            btnERcv.Enabled = false;
            btnECollect.Enabled = false;
            btnECancel.Enabled = false;

            txtESrcDesc.Text = "";
            txtERcvDesc.Text = "";
            txtERcvCount.Text = "";
            txtESrcCount.Text = "";

            cmbE_MDF.Items.Clear();
            cmbE_Vert.Items.Clear();

            txtE_VHi.Text = "";
            txtE_VLo.Text = "";
            txtEHiSrc.Text = "";
            txtELoSrc.Text = "";
            txtETotalSrc.Text = "";
            txtEHiRcv.Text = "";
            txtELoRcv.Text = "";
            txtETotalRcv.Text = "";

            grpECollect.Enabled = false;

            Application.DoEvents();
        }

        #region Button Events
        private void btnESrc_Click(object sender, EventArgs e)
        {
            try
            {
                InitE();
                cblESrc = new clsESIDE();

                GTClassFactory.Create<IGTApplication>().SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Checking selected feature");
                IGTSelectedObjects selFeature = GTClassFactory.Create<IGTApplication>().SelectedObjects;

                if (cblESrc.isCable(selFeature))
                {
                    cblESrc.ReadCable(selFeature.GetObjects()[0].FID);
                    if (cblESrc.CABLE_CLASS == "D-CABLE")
                        throw new System.Exception("Please select an E-SIDE cable");

                    txtESrcDesc.Text = cblESrc.ToString();
                    txtESrcCount.Text = cblESrc.COUNT_ANNO;
                    for (int cid = 0; cid < cblESrc.CID; cid++)
                    {
                        if (cmbE_MDF.FindStringExact(cblESrc.MDF(cid).ToString()) == -1)
                            cmbE_MDF.Items.Add(cblESrc.MDF(cid).ToString());
                        if (cmbE_Vert.FindStringExact(cblESrc.Vertical(cid).ToString()) == -1)
                            cmbE_Vert.Items.Add(cblESrc.Vertical(cid).ToString());
                    }
                    if (cmbE_MDF.Items.Count > 0) cmbE_MDF.SelectedIndex = 0;
                    if (cmbE_Vert.Items.Count > 0) cmbE_Vert.SelectedIndex = 0;
                    btnERcv.Enabled = true;
                    btnECancel.Enabled = true;
                }
                else
                    throw new System.Exception("Cable must end with E-SIDE termination point");

            }
            catch (Exception ex)
            {
                MessageBox.Show(this, "Fail to select source cable\r\n" + ex.Message, "Pick Source");
            }//end try catch
        }

        private void btnERcv_Click(object sender, EventArgs e)
        {
            try
            {
                cblERcv = new clsESIDE();

                GTClassFactory.Create<IGTApplication>().SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Checking selected feature");
                IGTSelectedObjects selFeature = GTClassFactory.Create<IGTApplication>().SelectedObjects;

                if (cblERcv.isCable(selFeature))
                {
                    cblERcv.ReadCable(selFeature.GetObjects()[0].FID);
                    if (cblERcv.CABLE_CLASS == "D-CABLE")
                        throw new System.Exception("Please selected an E-SIDE cable");

                    txtERcvDesc.Text = cblERcv.ToString();
                    txtERcvCount.Text = cblERcv.COUNT_ANNO;
                    btnECollect.Enabled = true;
                    grpECollect.Enabled = true;
                }
                else
                {
                    MessageBox.Show(this, "Cable must end with E-SIDE termination point", "Pick Receiver");
                    txtERcvDesc.Text = "";
                    txtERcvCount.Text = "";
                    btnECollect.Enabled = false;
                    grpECollect.Enabled = false;
                    btnECancel.Text = "Close";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, "Fail to select receiver cable\r\n" + ex.Message, "Pick Receiver");
            }//end try catch
        }

        private void btnECancel_Click(object sender, EventArgs e)
        {
            if (btnECancel.Text == "Close")
                this.Close();
            else if (MessageBox.Show("Press [CANCEL] to cancel\r\nor press [OK] to close Collect Count",
                "Collect Count", MessageBoxButtons.OKCancel) == DialogResult.Cancel)

                InitE();
            else
                this.Close();
        }

        private void btnECollect_Click(object sender, EventArgs e)
        {
            try
            {
                if (!ValidateCableProperties(cblESrc, cblERcv)) return;
                if (!ValidateESideCount()) return;

                int vlo = int.Parse(txtE_VLo.Text);
                int vhi = int.Parse(txtE_VHi.Text);
                int cid = cblESrc.isESideCountAvailable(int.Parse(cmbE_MDF.Text), int.Parse(cmbE_Vert.Text), vlo, vhi);

                if (cid > -1)
                {
                    DoCollectCount_ESide(cid);

                    if (txtERcvCount.Text.Length > 0)
                        txtERcvCount.Text += Environment.NewLine + txtELoSrc.Text + "-" + txtEHiSrc.Text;
                    else
                        txtERcvCount.Text = txtELoSrc.Text + "-" + txtEHiSrc.Text;
                    txtEHiSrc.Text = "";
                    txtELoSrc.Text = "";
                    grpECollect.Enabled = false;
                    btnECollect.Enabled = false;
                }
                else
                    MessageBox.Show(this, "Collect count fail\r\nPlease make sure combination of MDF, Vertical, High and Low is correct",
                        "Collect Count");
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, "Collect count fail\r\n" + ex.Message, "Collect Count");
            }
        }

        private void DoCollectCount_ESide(int cid)
        {
            try
            {
                int rlo = int.Parse(txtELoRcv.Text);
                int rhi = int.Parse(txtEHiRcv.Text);
                int slo = int.Parse(txtELoSrc.Text);
                int shi = int.Parse(txtEHiSrc.Text);
                int vlo = int.Parse(txtE_VLo.Text);
                int vhi = int.Parse(txtE_VHi.Text);
                int mdf = int.Parse(cmbE_MDF.Text);
                int vert = int.Parse(cmbE_Vert.Text);

                GTCollectCount.m_oIGTTransactionManager.Begin("CollectCount");

                cblESrc.TransferCountESide(cid, mdf, vert, vlo, vhi, slo, shi);
                cblERcv.ReceiveCountESide(mdf, vert, vlo, vhi, rlo, rhi);

                clsSaveCount.DeletePairCount(cblESrc);
                clsSaveCount.SavePairCountESide(cblESrc, "ORIG");

                clsSaveCount.DeletePairCount(cblERcv);
                clsSaveCount.SavePairCountESide(cblERcv, "CC");

                clsSaveCount.PlacePairCountLabel(cblERcv);

                GTCollectCount.m_oIGTTransactionManager.Commit();
                GTCollectCount.m_oIGTTransactionManager.RefreshDatabaseChanges();
                MessageBox.Show(this, "Collect count success", "Collect Count");
            }
            catch (Exception ex)
            {
                GTCollectCount.m_oIGTTransactionManager.Rollback();
                MessageBox.Show(this, "Collect count fail\r\n" + ex.Message, "Collect Count");
            }
        }

        private void txtE_VHiLo_Leave(object sender, EventArgs e)
        {
            try
            {
                int vlo = int.Parse(txtE_VLo.Text);
                int vhi = int.Parse(txtE_VHi.Text);
                if (vlo > vhi) return;

                int cid = cblESrc.isESideCountAvailable(int.Parse(cmbE_MDF.Text), int.Parse(cmbE_Vert.Text), vlo, vhi);
                if (cid > -1)
                {
                    int total = int.Parse(txtE_VHi.Text) - int.Parse(txtE_VLo.Text) + 1;
                    int lo = cblESrc.Lo(cid) + (vlo - cblESrc.Vert_Lo(cid));
                    int hi = lo + total - 1;
                    this.txtELoSrc.Text = lo.ToString();
                    this.txtEHiSrc.Text = hi.ToString();
                    this.txtETotalSrc.Text = total.ToString();
                }
                else
                {
                    this.txtELoSrc.Text = "";
                    this.txtEHiSrc.Text = "";
                    this.txtETotalSrc.Text = "";
                }
            }
            catch (Exception ex)
            { }
        }

        #endregion

        #region TextBox Events
        private void txtECount_Enter(object sender, EventArgs e)
        {
            btnECollect.Focus();
        }

        private void txtECount_TextChanged(object sender, EventArgs e)
        {
            try
            {
                int lo = int.Parse(txtELoRcv.Text);
                int hi = int.Parse(txtEHiRcv.Text);
                btnECollect.Enabled = (lo <= hi);
                int count = (hi - lo) + 1;
                if (hi == 0) count = 0;
                this.txtERcvCount.Text = count.ToString();
            }
            catch
            {
                btnECollect.Enabled = false;
                this.txtERcvCount.Text = "0";
            }
        }
        #endregion

        #region Validation

        private bool ValidateESideCount()
        {
            int lo = int.Parse(txtELoSrc.Text);
            int hi = int.Parse(txtEHiSrc.Text);

            if (hi < lo)
                MessageBox.Show(this, "High count must not be lower than low count", "Error : Collect Count");
            else if (cblERcv.EFFECTIVE_PAIRS < hi - lo + 1)
                MessageBox.Show(this, "Total count is bigger than receiver effective pairs", "Error : Collect Count");
            else
                return true;

            return false;

        }
        #endregion

        #endregion

        #region Highlight End Point
        private void btnSrcHiLite_Click(object sender, EventArgs e)
        {
            if (sender == this.btnDSrcHiLite)
                hiliteFeature(cblDSrc.OUT_FID, cblDSrc.OUT_FNO, cblDSrc.DETAIL_ID);
            else
                hiliteFeature(cblESrc.OUT_FID, cblESrc.OUT_FNO, cblESrc.DETAIL_ID);
        }

        private void btnRcvHiLite_Click(object sender, EventArgs e)
        {
            if (sender == this.btnDRcvHiLite)
                hiliteFeature(cblDRcv.OUT_FID, cblDRcv.OUT_FNO, cblDRcv.DETAIL_ID);
            else
                hiliteFeature(cblERcv.OUT_FID, cblERcv.OUT_FNO, cblERcv.DETAIL_ID);
        }

        private void hiliteFeature(int isFID, short isFNO, int detailID)
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
                GTClassFactory.Create<IGTApplication>().ActiveMapWindow.DisplayScale = 200;
                GTClassFactory.Create<IGTApplication>().RefreshWindows();
                oGTKeyObjs = null;

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Pair Count Management System", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion

        #region Move Label
        private void btnSrcMove_Click(object sender, EventArgs e)
        {
            clsCableCount cblSrc;
            if (sender == btnDSrcMove)
                cblSrc = cblDSrc;
            else
                cblSrc = cblESrc;
            hiliteFeature(cblSrc.OUT_FID, cblSrc.OUT_FNO, cblSrc.DETAIL_ID);
            cblSrc.LabelPlacement();
            StatusMsg(appstatus.move_label);
        }

        private void btnRcvMove_Click(object sender, EventArgs e)
        {
            clsCableCount cblRcv;
            if (sender == btnDRcvMove)
                cblRcv = cblDRcv;
            else
                cblRcv = cblERcv;
            hiliteFeature(cblRcv.OUT_FID, cblRcv.OUT_FNO, cblRcv.DETAIL_ID);
            cblRcv.LabelPlacement();
            StatusMsg(appstatus.move_label);
        }
        #endregion

        #region Validation
        private bool ValidateCableProperties(clsCableCount cblSrc, clsCableCount cblRcv)
        {
            if (cblSrc.FID == cblRcv.FID)
                MessageBox.Show(this, "Source and Receiver cannot be the same cable", "Error : Collect Count");
            else if (cblSrc.CABINET_CODE != cblRcv.CABINET_CODE)
                MessageBox.Show(this, "Source and Receiver not from the same source", "Error : Collect Count");
            else if (cblSrc.CABLE_CLASS != cblRcv.CABLE_CLASS)
                MessageBox.Show(this, "Source and Receiver not in the same type of cable", "Error : Collect Count");
            else if (cblSrc.CABLE_CODE != cblRcv.CABLE_CODE)
                MessageBox.Show(this, "Source and Receiver has different cable code", "Error : Collect Count");
            else if (cblSrc.FEATURE_STATE != "ASB" && cblSrc.FEATURE_STATE != "MOD")
                MessageBox.Show(this, "Source state must be either AS-BUILT or MOD", "Error : Collect Count");
            else if (cblRcv.FEATURE_STATE != "PPF" && cblRcv.FEATURE_STATE != "MOD" && cblRcv.FEATURE_STATE != "PAD")
                MessageBox.Show(this, "Receiver state must be either PPF, PAD or MOD", "Error : Collect Count");
            else if (cblSrc.EXC_ABB != cblRcv.EXC_ABB)
                MessageBox.Show(this, "Source and Receiver not from the same exchange", "Error : Collect Count");
            else
                return true;

            return false;

        }
        #endregion

        #region Status
        enum appstatus
        {
            init,
            select_source,
            select_receiver,
            set_count,
            collect_count,
            complete_count,
            move_label,
            click_label,
            closing
        }
        private void StatusMsg(appstatus level)
        {
            tabCollect.Enabled = (level != appstatus.collect_count);
            this.Cursor = (level == appstatus.collect_count ? Cursors.WaitCursor : Cursors.Default);
            switch (level)
            {
                case appstatus.init:
                    txtStatus.Text = "Initialize..."; break;
                case appstatus.select_source:
                    txtStatus.Text = "Please select source cable with count before click on [Select Cable] button"; break;
                case appstatus.select_receiver:
                    txtStatus.Text = "Source cable selected. \r\nSelect receiver cable with count and click [Select Cable]"; break;
                case appstatus.set_count:
                    txtStatus.Text = "Receiver cable selected. \r\nKey-in pair low and high value then click [Collect Count] to continue"; break;
                case appstatus.collect_count:
                    txtStatus.Text = "Performing collect count, please wait..."; break;
                case appstatus.complete_count:
                    txtStatus.Text = "Collect count completed. \r\nClick [Move Pair Count] to move pair count label"; break;
                case appstatus.move_label:
                    txtStatus.Text = "Double click to place label"; break;
                case appstatus.click_label:
                    txtStatus.Text = "Label placed."; break;
                case appstatus.closing:
                    txtStatus.Text = "Closing collect count..."; break;
            }
            Application.DoEvents();
        }

        public void placeLabelMsg()
        {
            StatusMsg(appstatus.click_label);
        }
        private void txtStatus_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;
        }
        #endregion

        private void btnCollect_Click(object sender, EventArgs e)
        {

            string index = ((Button)sender).Name.Substring(3);
            tabCollect.SelectedIndex = int.Parse(index) * 2 - 1;
            //tabCollect.TabPages[int.Parse(index) * 2 - 1].Select();
        }

    }
}
