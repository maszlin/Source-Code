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

namespace NEPS.OSP.COPPER.CABLE_TRANSFER
{
    public partial class frmTransfer : Form
    {
        private IGTApplication m_gtapp = null;
        private IGTDataContext m_IGTDataContext = null;


        #region Form Event

        public frmTransfer()
        {
            try
            {
                InitializeComponent();
                this.txtMessage.Text = "Please select source cable";

                m_gtapp = GTClassFactory.Create<IGTApplication>().Application;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }


        private void frmTransfer_Load(object sender, EventArgs e)
        {
            
            this.m_IGTDataContext = GTClassFactory.Create<IGTApplication>().Application.DataContext;
            string filename = Application.StartupPath + "/NEPS.OSP.COPPER.CABLE_TRANSFER.dll";
            this.Text = "Cable Transfer [" + FileVersionInfo.GetVersionInfo(filename).FileVersion + "]";

        }
        #endregion

        #region Transfer Message
        public void ProgressMessage(string msg)
        {
            this.txtMessage.Text = msg;
            Application.DoEvents();
        }
        public void AddReport(string msg)
        {
            this.txtMessage.Text = msg + "\r\n" + this.txtMessage.Text;
            Application.DoEvents();
        }
        public void InitProgressBar(int val)
        {
            prgProcess.Maximum = val;
            prgProcess.Value = 0;
        }
        public void IncreaseProgressBar(int val)
        {
            if (val == prgProcess.Maximum)
                prgProcess.Value = prgProcess.Maximum;
            else if (prgProcess.Value + val > prgProcess.Maximum)
                prgProcess.Value = prgProcess.Maximum - 5;
            else
                prgProcess.Value += val;
            prgProcess.Visible = true;
            Application.DoEvents();
        }
        
        #endregion

        #region Button Event
        private void btnExit_Click(object sender, EventArgs e)
        {
            this.TopMost = false;
            if (btnExit.Text == "5. CANCEL")
                if (MessageBox.Show(this, "Area you sure to close and cancel cable transfer",
                "Cable Transfer", MessageBoxButtons.YesNo) == DialogResult.No)
                {
                    this.TopMost = true;
                    return;
                }
            this.Close();
        }

        #endregion

        #region Test User Selection
        //public Intergraph.GTechnology.API.IGTApplication m_gtapp = GTClassFactory.Create<IGTApplication>();

        clsTransfer cTransfer = new clsTransfer();
        private int isStep = 0;
        private bool isClick = false;

        public void UserMouseMove(IGTPoint ePoint)
        {
            if (!this.Visible)
            {
                DrawLeaderLines(ePoint, mobjEditService);
                DrawTempTransferFeature(ePoint);
            }
            return;

            //if (isClick) UserMouseClick(ePoint);
            //isClick = false;
            switch (isStep)
            {
                case 0: // user select source cable
                    GTClassFactory.Create<IGTApplication>().SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Click on the recipient stub");
                    m_gtapp.ActiveMapWindow.MousePointer = GTMousePointerConstants.gtmwmpCrossHair;
                    break;
                case 1: // user to select destination cable
                    GTClassFactory.Create<IGTApplication>().SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Click on the donor joint or cable");
                    m_gtapp.ActiveMapWindow.MousePointer = GTMousePointerConstants.gtmwmpCrossHair;
                    break;
                case 2:
                    GTClassFactory.Create<IGTApplication>().SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Cable transfer in progress...");
                    m_gtapp.ActiveMapWindow.MousePointer = GTMousePointerConstants.gtmwmpRotate;
                    break;
                case 3:
                    GTClassFactory.Create<IGTApplication>().SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Cable transfer completed");
                    isStep = 4;
                    break;
                case 4:
                    this.Close();
                    break;
                case -1: // error
                    //CancelTransfer();
                    break;
            }
            System.Windows.Forms.Application.DoEvents();
        }

        public void UserMouseClick(IGTPoint ePoint)
        {
            isClick = true;
            if (!this.Visible)
            {
                isClick = false;
                int i = arrPoint.Count - 1;
                if (arrPoint[i].X != ePoint.X || arrPoint[i].Y != ePoint.Y)
                {
                    DrawLeaderLines(ePoint, mobjEditServiceLine);
                    arrPoint.Add(ePoint);
                }
                //createTransferFeature(ePoint);
            }
            else
            {

                //DrawTempTransferFeature(ePoint);
            }

        }
        #endregion

        #region Transfer ListView - update on 07-07-2012

        private void ShowSelectedObject(IGTDDCKeyObject obj)
        {
            m_gtapp.SelectedObjects.Clear();
            m_gtapp.SelectedObjects.Add(GTSelectModeConstants.gtsosmAllComponentsOfFeature, obj);
            m_gtapp.ActiveMapWindow.CenterSelectedObjects();
            m_gtapp.RefreshWindows();
        }


        #endregion


        private void btnRecipient_Click(object sender, EventArgs e)
        {
            if (cTransfer.isSrceSelected(m_gtapp.SelectedObjects))
            {
                chkRecipient.Checked = true;
                lblRecipient.Text = "[" + clsReport.rcpnt.EXC_ABB + "][" +
                    clsReport.rcpnt.CAB_CODE + "][" + clsReport.rcpnt.CABLE_CODE + "][" +
                    clsReport.rcpnt.m_CABLE_SIZE + "/" + clsReport.rcpnt.m_EFCT_PAIRS + "]";
            }
            else
                this.txtMessage.Text = "Please select the recipient stub";
        }

        private void btnDonor_Click(object sender, EventArgs e)
        {
            if (cTransfer.isDestSelected(m_gtapp.SelectedObjects))
            {
                chkDonor.Checked = true;
                lblDonor.Text = "[" + clsReport.donor.EXC_ABB + "][" +
                    clsReport.donor.CAB_CODE + "][" + clsReport.donor.CABLE_CODE + "][" +
                    clsReport.donor.m_CABLE_SIZE + "/" + clsReport.donor.m_EFCT_PAIRS + "]";

                btnTransfer.Enabled = true;
            }
            else
            {
                lblDonor.Text = "Donor must be a valid joint";
                btnTransfer.Enabled = false;
            }
        }

        private void btnTransfer_Click(object sender, EventArgs e)
        {
            try
            {
                InitProgressBar(50);
                if (chkRecipient.Checked && chkDonor.Checked)
                {
                    if (lblRecipient.Text != lblDonor.Text)
                        optCountNew.Checked = true;

                    btnExit.Enabled = false;
                    this.Cursor = Cursors.WaitCursor;
                    int TR_FID = cTransfer.PerformCableTransfer(dtpTransfer.Value);
                    if (TR_FID > -1)
                    {
                        IncreaseProgressBar(prgProcess.Maximum);
                        InitDrawTransferFeature(TR_FID);
                    }
                }
            }
            catch (Exception ex)
            {
                this.TopMost = false;
                MessageBox.Show(this, "ERROR performing cable transfer\r\n" + ex.Message, "Cable Transfer");
                this.TopMost = true;
            }
            finally
            {
                this.Cursor = Cursors.Default;
                this.TopMost = true;
            }
        }

        private void chkTransfer_CheckedChanged(object sender, EventArgs e)
        {
            btnDonor.Enabled = chkRecipient.Checked;
            if (btnDonor.Enabled)
                this.txtMessage.Text = "Please select a joint as donor" + Environment.NewLine +
                    "Effective pairs and cable size must equal to recipient's";

            else
                chkDonor.Checked = false;

            if (!chkRecipient.Checked)
                this.txtMessage.Text = "Please select a stub as recipient";
            else if (chkDonor.Checked)
            {
                btnTransfer.Enabled = true;
                this.txtMessage.Text = "You may change the transfer date";
            }
            else
                btnTransfer.Enabled = false;

        }

        #region Transfer Feature
        IGTGeometryEditService mobjEditService = null;
        IGTGeometryEditService mobjEditServiceLine = null;
        List<IGTPoint> arrPoint = new List<IGTPoint>();
        int m_TransferFID = -1;

        private void InitDrawTransferFeature(int TR_FID)
        {
            m_TransferFID = TR_FID;
            mobjEditService = GTClassFactory.Create<IGTGeometryEditService>();
            mobjEditService.TargetMapWindow = m_gtapp.ActiveMapWindow;
            mobjEditServiceLine = GTClassFactory.Create<IGTGeometryEditService>();
            mobjEditServiceLine.TargetMapWindow = m_gtapp.ActiveMapWindow;
            arrPoint.Clear();
            arrPoint.Add(cTransfer.dstSelectedJoint.Geometry.FirstPoint);
            arrPoint.Add(cTransfer.dstSelectedJoint.Geometry.FirstPoint);
            this.Hide();
            btnReport.Enabled = true;
            btnExit.Enabled = true;
            btnExit.Text = "5. DONE";
        }

        private void DrawTempTransferFeature(IGTPoint point)
        {
            //GTClassFactory.Create<IGTApplication>().SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Move mouse to the indicate location of new joint. Click to confirm.");

            IGTPointGeometry oPointDPGeom;
            oPointDPGeom = GTClassFactory.Create<IGTPointGeometry>();
            IGTPoint oPntGeom = GTClassFactory.Create<IGTPoint>();
            oPntGeom.X = point.X;
            oPntGeom.Y = point.Y;

            int isStyleID = 6120001;               //G3E_SNO for TRANSFER FEATURE.

            oPointDPGeom.Origin = oPntGeom;
            // draw the temp joint symbol
            mobjEditService.AddGeometry(oPointDPGeom, isStyleID);

            GTClassFactory.Create<IGTApplication>().SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Click to place leade line. Double-click to place transfer feature");
            System.Windows.Forms.Application.DoEvents();

        }

        private void DrawLeaderLines(IGTPoint worldPoint, IGTGeometryEditService tempMap)
        {
            IGTPoint lastPoint = arrPoint[arrPoint.Count - 1];
            IGTLineGeometry oLine = PGeoLib.CreateLineGeom(lastPoint.X, lastPoint.Y, worldPoint.X, worldPoint.Y);
            if (mobjEditService.GeometryCount > 0) mobjEditService.RemoveAllGeometries();
            tempMap.AddGeometry(oLine, 351001); //351001);
            //            arrPoint.Add(worldPoint);
        }

        public void UpdateTransferFeature()
        {
            cTransfer.UpdateTransferObject(m_TransferFID, arrPoint);
            clsReport.TransferFID = m_TransferFID;
            // ClearTemp();
            this.Show();
        }

        private void ClearTemp()
        {
            arrPoint.Clear();
            if (mobjEditService != null)
                mobjEditService.RemoveAllGeometries();
            if (mobjEditServiceLine != null)
                mobjEditServiceLine.RemoveAllGeometries();
        }

        public void Backspace()
        {
            if (arrPoint.Count > 1)
                arrPoint.RemoveAt(arrPoint.Count - 1);
        }

        public void CancelDrawing()
        {
            arrPoint.Clear();
            if (mobjEditService != null)
            {
                mobjEditService.RemoveAllGeometries();
                mobjEditService = null;
            }
            if (mobjEditServiceLine != null)
            {
                mobjEditServiceLine.RemoveAllGeometries();
                mobjEditServiceLine = null;
            }
            System.Windows.Forms.Application.DoEvents();
            //ExitCmd();
        }

        #endregion

        private void btnReport_Click(object sender, EventArgs e)
        {
            try
            {
                string filename = clsReport.CreateReport();
                Process.Start(filename);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, "Error creating the report\r\n" + ex.Message, "Cable Transfer");
            }
        }

        #region Edited : 28-03-2013
        private void btnTransfer_EnabledChanged(object sender, EventArgs e)
        {
            lblDate.Enabled = btnTransfer.Enabled;
            dtpTransfer.Enabled = btnTransfer.Enabled;
        }
        #endregion

    }
}