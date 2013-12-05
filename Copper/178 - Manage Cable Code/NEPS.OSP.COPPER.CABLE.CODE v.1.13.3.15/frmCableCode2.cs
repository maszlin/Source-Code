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

namespace NEPS.OSP.COPPER.CABLE.CODE
{
    public partial class frmCableCode : Form
    {
        private IGTApplication m_gtapp = null;
        private IGTDataContext m_IGTDataContext = null;

        //Variables to store values from selected E-Side/D-Side Cable
        int s_FID = 0, s_outFID = 0;
        string s_ExcAbb = "", s_CableClass = "", s_curCableCode, s_ItfaceCode, s_RTCode, s_textValue = "",
               d_CableClass = "";

        #region Form Cable Code
        public frmCableCode()
        {
            try
            {
                InitializeComponent();
                this.FormClosing += new FormClosingEventHandler(this.frmFrame_FormClosing);
                m_gtapp = GTClassFactory.Create<IGTApplication>().Application;
                m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Running Manage Cable Code...");
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "Running Manage Cable Code", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void frmFrame_Shown(object sender, EventArgs e)
        {
            m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Running Manage Cable Code...");
        }

        private void frmFrame_Activated(object sender, EventArgs e)
        {
            m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Running Manage Cable Code...");
        }

        private void frmFrame_Load(object sender, EventArgs e)
        {
            this.m_IGTDataContext = GTClassFactory.Create<IGTApplication>().Application.DataContext;
        }

        private void frmFrame_FormClosing(object sender, FormClosingEventArgs e)
        {
            //DialogResult retVal = MessageBox.Show(this,"Are you sure to exit?", "Manage Cable Code", MessageBoxButtons.YesNo);
            //if (retVal == DialogResult.Yes)
            //{
            //    e.Cancel = false;
            //}
            //else
            //{
            //    e.Cancel = true;
            //}
        }
        #endregion

        #region General Function

        private void resetButtonsAndTextBox()
        {
            this.Cursor = Cursors.Default;
            currCableCodeTb.Text = "";
            newCableCodeTb.Enabled = false;
            newCableCodeTb.Text = "";
            updateCableCode_ChangeBtn.Enabled = false;
            progressBar1.Value = 0;
            rtbNote.Text = "   [Note]" + Environment.NewLine +
                "\t- please select cable to change " + Environment.NewLine +
                "\t- and click on [Get Cable Code] button";
        }

        private void AssignValues(string sRExc, string sRItFc, string sRrtc, string sRCC, string sRCurCode, string sTextVal, int sOutFID)
        {
            s_ExcAbb = sRExc;
            s_ItfaceCode = (sRItFc == "***" ? "" : sRItFc);
            s_RTCode = (sRrtc == "***" ? "" : sRrtc);
            s_CableClass = sRCC;
            s_curCableCode = sRCurCode;
            s_textValue = sTextVal;
            s_outFID = sOutFID;
            currCableCodeTb.Text = s_curCableCode.ToString();

            rtbNote.Text = "Exchange : " + s_ExcAbb + Environment.NewLine;
            if (s_CableClass.IndexOf("D-") > -1)
            {
                rtbNote.Text += "ITFACE_CODE : " + s_ItfaceCode;
                rtbNote.Text += "\tRT_CODE : " + s_RTCode + Environment.NewLine;
            }
            rtbNote.Text += "CABLE_CLASS : " + s_CableClass;
            rtbNote.Text += "\tCABLE_CODE : " + s_curCableCode + Environment.NewLine;


            Application.DoEvents();

        }

        private string sqlSelectedCable(int sourceSel_FID)
        {
            return "SELECT A.EXC_ABB, B.ITFACE_CODE, B.RT_CODE, B.CABLE_CLASS, B.CABLE_CODE, B.TEXT_VALUE, C.IN_FNO, C.OUT_FID " +
                   "FROM GC_NETELEM A, GC_CBL B, GC_NR_CONNECT C " +
                   "WHERE A.G3E_FID = B.G3E_FID AND B.G3E_FID = C.G3E_FID AND C.G3E_FID =" + sourceSel_FID;
        }

        #endregion

        #region Manage Cable Code : Change Cable Code Tab

        #region Manage Cable Code : Variables declaration
        bool valid = false;
        string getSelectedSQL;

        ADODB.Recordset sRS = new ADODB.Recordset(); //sRS = selected Recordset
        ADODB.Recordset compareRS = new ADODB.Recordset(); // compareRS = compareCode RecordSet

        #endregion

        private void getSelected_ChangeBtn_Click(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;
            short sInFno = 0;

            int selected_count = GTClassFactory.Create<IGTApplication>().SelectedObjects.GetObjects().Count;
            try
            {
                foreach (IGTDDCKeyObject oDDCKeyObject in GTClassFactory.Create<IGTApplication>().SelectedObjects.GetObjects())
                {
                    string cbl_class = oDDCKeyObject.Recordset.Fields["CABLE_CLASS"].Value.ToString();
                    if (oDDCKeyObject.FNO == 7000)
                    {
                        s_FID = oDDCKeyObject.FID;
                        getSelectedSQL = sqlSelectedCable(s_FID);

                        sRS = GTClassFactory.Create<IGTApplication>().DataContext.OpenRecordset(getSelectedSQL,
                            ADODB.CursorTypeEnum.adOpenStatic, ADODB.LockTypeEnum.adLockReadOnly, 1);
                        if (sRS.RecordCount > 0)
                        {
                            sRS.MoveFirst();
                            sInFno = myUtil.ParseShort(sRS.Fields["IN_FNO"].Value.ToString());
                            {
                                AssignValues(sRS.Fields["EXC_ABB"].Value.ToString(),
                                    sRS.Fields["ITFACE_CODE"].Value.ToString(),
                                    sRS.Fields["RT_CODE"].Value.ToString(),
                                    sRS.Fields["CABLE_CLASS"].Value.ToString(),
                                    sRS.Fields["CABLE_CODE"].Value.ToString(),
                                    sRS.Fields["TEXT_VALUE"].Value.ToString(),
                                    myUtil.ParseInt(sRS.Fields["OUT_FID"].Value.ToString()));
                                valid = true;
                            }
                            break;
                        }
                    }
                }

                this.Cursor = Cursors.Default;
                if (valid)
                {
                    newCableCodeTb.Enabled = true;
                    updateCableCode_ChangeBtn.Enabled = true;
                    rtbNote.Text += Environment.NewLine + "Enter new cable code" + Environment.NewLine;
                    rtbNote.Text += "and click the [Change Code] button";
                }
                else
                {
                    resetButtonsAndTextBox();
                    MessageBox.Show(this, "Please select ONLY ONE E-Side or D-Side Cable", "Manage Cable Code",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "Manage Cable Code", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion

        private void UpdateCableCode_ChangeBtn_Click(object sender, EventArgs e)
        {
            try
            {
                rtbNote.Text = "Changing cable code from " + currCableCodeTb.Text + " to " + newCableCodeTb.Text;
                rtbNote.Text += "\r\n\nValidating the new cable code ...";
                this.Cursor = Cursors.WaitCursor;
                GTManageCableCode.m_gtapp.BeginWaitCursor();
                Application.DoEvents();

                if (!isValidCode())
                {
                    rtbNote.Text += "\r\n\nValidation fail. Please enter a valid cable code";
                    return;
                }

                rtbNote.Text += "\r\nPerforming the changes, please wait.";

                Application.DoEvents();


                Application.DoEvents();

                int[] start_cable;
                if (s_CableClass.IndexOf("E-") != -1)
                    start_cable = clsTrace.TraceUp_EGetStartPoint(s_FID);
                else
                    start_cable = clsTrace.TraceUp_DGetStartPoint(s_FID);


                GTManageCableCode.m_oIGTTransactionManager.Begin("ManageCableCode");
                clsUpdateCode updatecode = new clsUpdateCode(newCableCodeTb.Text, currCableCodeTb.Text, start_cable[2]);
                updatecode.UpdateCableCode();
                this.progressBar1.Value = this.progressBar1.Maximum;
                rtbNote.Text += Environment.NewLine + "Please wait while updating changes to database...";
                Application.DoEvents();

                GTManageCableCode.m_oIGTTransactionManager.Commit();
                GTManageCableCode.m_oIGTTransactionManager.RefreshDatabaseChanges();
                MessageBox.Show(this, "Completed : Cables code already change", "Manage Cable Code", MessageBoxButtons.OK);
                resetButtonsAndTextBox();
            }
            catch (Exception ex)
            {
                GTManageCableCode.m_oIGTTransactionManager.Rollback();
                MessageBox.Show(this, ex.Message, "Manage Cable Code", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                this.Cursor = Cursors.Default;
                GTManageCableCode.m_gtapp.EndWaitCursor();
            }
        }

        #region Edited by m.zam 2013-03-13

        #region Code Validation
        private bool isValidCode()
        {
            if (!Test_CableCodeFormat())
                return false;

            if (FindCableCode(newCableCodeTb.Text))
            {
                if (MessageBox.Show(this, "Cable Code already in use.\r\nContinue with this cable code",
                    "Manage Cable Code", MessageBoxButtons.OKCancel, MessageBoxIcon.Information) == DialogResult.Cancel)
                    return false;
            }

            return true;

        }

        private bool Test_CableCodeFormat()
        {
            newCableCodeTb.Text = newCableCodeTb.Text.Trim();
            string cbl_code = newCableCodeTb.Text;
            if (cbl_code == "")
            {
                MessageBox.Show(this, "Cable Code cannot be NULL.\nPlease enter the code", "Manage Cable Code", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return false;
            }
            else if (char.IsDigit(cbl_code[0]))
            {
                MessageBox.Show(this, "Cable code must begin with an alphabet", "Manage Cable Code", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return false;
            }
            else
            {
                bool need_digit = false;
                for (int i = 1; i < cbl_code.Length; i++)
                {
                    if (char.IsDigit(cbl_code[i]))
                        need_digit = true;
                    else if (need_digit)
                    {
                        MessageBox.Show(this, "Cable code not in correct format", "Manage Cable Code", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return false;
                    }
                }
                return true;
            }
        }
        private bool FindCableCode(string newCableCode)
        {
            string checkSQL;
            checkSQL = "SELECT B.CABLE_CODE FROM GC_NETELEM A, GC_CBL B " +
                    "WHERE A.G3E_FID = B.G3E_FID AND A.EXC_ABB = '" + s_ExcAbb + "' " +
                    "AND B.CABLE_CODE = '" + newCableCode + "' AND ROWNUM = 1";
            if (s_CableClass.IndexOf("D-") > -1) // D-SIDE cable
            {
                checkSQL += (s_ItfaceCode.Length > 0 ?
                 " AND B.ITFACE_CODE = '" + s_ItfaceCode + "'" :
                 " AND B.RT_CODE = '" + s_RTCode + "'");
            }

            ADODB.Recordset compareRs = new ADODB.Recordset();
            compareRS = GTClassFactory.Create<IGTApplication>().DataContext.OpenRecordset(checkSQL,
                ADODB.CursorTypeEnum.adOpenStatic, ADODB.LockTypeEnum.adLockReadOnly, 1);

            return (compareRS.RecordCount > 0); // return true if found
        }

        #endregion

        private void newCableCodeTb_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsLetterOrDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
                e.Handled = true;
        }

        public void UpdateProgressBar()
        {
            int val = this.progressBar1.Value + 1;
            this.progressBar1.Value = (val < this.progressBar1.Maximum ? val : this.progressBar1.Maximum / 2);
            Application.DoEvents();
        }
        #endregion

    }
}