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
        private double initX;
        private double initY;
        private int initDetail_ID;

        //Variables to store values from selected E-Side/D-Side Cable
        int s_FID = 0, s_outFID = 0;
        string s_ExcAbb = "", s_CableClass = "", s_curCableCode, s_ItfaceCode, s_textValue = "",
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
                MessageBox.Show(this,ex.Message, "Running Manage Cable Code", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

        public int detail_ID
        {
            set
            {
                initDetail_ID = value;
            }
        }

        private void resetButtonsAndTextBox()
        {
            currCableCodeTb.Text = "";
            checkCodeBtn.Enabled = false;
            newCableCodeTb.Enabled = false;
            newCableCodeTb.Text = "";
            updateCableCode_ChangeBtn.Enabled = false;
            //updateStatusTb.Text = "";
            progressBar1.Value = 0;
            progressBar2.Value = 0;
        }

        private void assignValues(string sRExc, string sRItFc, string sRCC, string sRCurCode, string sTextVal, int sOutFID)
        {
            s_ExcAbb = sRExc;
            s_ItfaceCode = sRItFc;
            s_CableClass = sRCC;
            s_curCableCode = sRCurCode;
            s_textValue = sTextVal;
            s_outFID = sOutFID;
            currCableCodeTb.Text = s_curCableCode.ToString();
        }

        private string TraceDownstream(int selectedCableFID)
        {
            int iRecordsetAffected;
            ADODB.Recordset rsTrace = new ADODB.Recordset();
            string traceName = "update_CableCode" + DateTime.Now.ToString("yyMMdd_HHmmss");
            try
            {
                object[] objs = { traceName, selectedCableFID };
                rsTrace = m_gtapp.DataContext.Execute("GC_OSP_COP_VAL.TRACE_DOWNSTREAM", out iRecordsetAffected, (int)ADODB.CommandTypeEnum.adCmdStoredProc, objs);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return traceName;
        }

        private string traceResultFields(ADODB.Recordset rs, string fieldname)
        {
            try
            {
                object val = rs.Fields[fieldname].Value.ToString();
                if (val == null)
                    return "";
                else
                    return val.ToString().Trim();
            }
            catch { return ""; }
        }

        private string tracingDownstreamSQL(string traceRslt)
        {
            return "SELECT DISTINCT G3E_FID, G3E_FNO FROM " +
            "( SELECT A.G3E_FID, A.G3E_FNO  FROM TRACERESULT A, TRACEID B WHERE B.G3E_ID = A.G3E_TNO AND B.G3E_NAME = '" + traceRslt + "' " +
            "UNION ALL SELECT A.G3E_NODE1, A.G3E_FNO1  FROM TRACERESULT A, TRACEID B WHERE B.G3E_ID = A.G3E_TNO AND B.G3E_NAME = '" + traceRslt + "' " +
            "UNION ALL SELECT A.G3E_NODE2, A.G3E_FNO2 FROM TRACERESULT A, TRACEID B WHERE B.G3E_ID = A.G3E_TNO AND B.G3E_NAME = '" + traceRslt + "' " +
            ") A "; 
        }

        private void deleteTrace(string traceName)
        {
            ADODB.Recordset rs = new ADODB.Recordset();
            string strSQL = "TRACE.DELETE " + traceName + ")";

            try
            {
                int iRecordsetAffected;
                rs = m_gtapp.DataContext.Execute("strSQL", out iRecordsetAffected, (int)ADODB.CommandTypeEnum.adCmdStoredProc, 1);
                rs = null;

            }
            catch { }
        }

        private string sqlSelectedCable(int sourceSel_FID)
        {
            return "SELECT A.EXC_ABB, B.ITFACE_CODE, B.CABLE_CLASS, B.CABLE_CODE, B.TEXT_VALUE, C.IN_FNO, C.OUT_FID " +
                   "FROM GC_NETELEM A, GC_CBL B, GC_NR_CONNECT C " +
                   "WHERE A.G3E_FID = B.G3E_FID AND B.G3E_FID = C.G3E_FID AND C.G3E_FID =" + sourceSel_FID;
        }
        /*
         * VALIDATION PURPOSE : To check whether the new cable code entered by user is already existed.
         */
        private bool checkCableCodeAvailability(string newCableCode, string source_ItCd)
        {
            string checkSQL = "";
            ADODB.Recordset compareRs = new ADODB.Recordset();

            checkSQL = (source_ItCd == "") ?
                "SELECT B.CABLE_CODE FROM GC_NETELEM A, GC_CBL B " +
                    "WHERE A.G3E_FID = B.G3E_FID AND " +
                        "A.EXC_ABB = '" + s_ExcAbb + "' AND " +
                        "B.CABLE_CODE = '" + newCableCode + "' AND ROWNUM <= 1" :
                "SELECT B.CABLE_CODE FROM GC_NETELEM A, GC_CBL B " +
                   "WHERE A.G3E_FID = B.G3E_FID AND " +
                       "A.EXC_ABB = '" + s_ExcAbb + "' AND " +
                       "B.CABLE_CODE = '" + newCableCode + "' AND " +
                       "B.ITFACE_CODE = '" + source_ItCd + "' AND ROWNUM <= 1";

            compareRS = GTClassFactory.Create<IGTApplication>().DataContext.OpenRecordset(checkSQL, ADODB.CursorTypeEnum.adOpenStatic, ADODB.LockTypeEnum.adLockReadOnly, 1);
            return (compareRS.RecordCount > 0);
        }

        private bool checkDSideCableCodeAvailability(string newCableCode)
        {
            string checkSQL = "";
            ADODB.Recordset compareRs = new ADODB.Recordset();

            checkSQL =
                "SELECT B.CABLE_CODE FROM GC_NETELEM A, GC_CBL B " +
                "WHERE A.G3E_FID = B.G3E_FID AND " +
                    "A.EXC_ABB = '" + s_ExcAbb + "' AND " +
                    "B.CABLE_CODE = '" + newCableCode.ToString().ToUpper() + "' AND ROWNUM <= 1";

            compareRS = GTClassFactory.Create<IGTApplication>().DataContext.OpenRecordset(checkSQL, ADODB.CursorTypeEnum.adOpenStatic, ADODB.LockTypeEnum.adLockReadOnly, 1);
            return (compareRS.RecordCount > 0);
        }

        private void changeCableCodeOnClick(int sourceFID, string updateType)
        {
            int dFID, count = 0, iRecordsAffected; // dFID = destination FID
            short dFNO; // dFNO = destination FNO

            string traceResult = "", selectSQL = "", update_TailSQL = "";
            ADODB.Recordset rsSync = new ADODB.Recordset();
            ADODB.Recordset rsTail = new ADODB.Recordset();

            traceResult = TraceDownstream(sourceFID);
            selectSQL = tracingDownstreamSQL(traceResult);
            rsSync = GTClassFactory.Create<IGTApplication>().DataContext.OpenRecordset(selectSQL, ADODB.CursorTypeEnum.adOpenStatic, ADODB.LockTypeEnum.adLockReadOnly, 1);
            if(rsSync.RecordCount > 0)
                rsSync.MoveFirst();

            if (updateType == "Update")
            {
                progressBar1.Maximum = rsSync.RecordCount;
                progressBar1.Value = count; 
                
                update_TailSQL = "UPDATE GC_CBL SET CABLE_CODE = '" + newCableCodeTb.Text + "' WHERE G3E_FID IN (SELECT G3E_FID FROM GC_NR_CONNECT WHERE OUT_FID = " + s_outFID + ")";
                rsTail = GTClassFactory.Create<IGTApplication>().DataContext.Execute(update_TailSQL, out iRecordsAffected, (int)ADODB.CommandTypeEnum.adCmdText);
                update_TailSQL = "COMMIT";
                count += iRecordsAffected;
            }
            else
            {
                progressBar2.Maximum = rsSync.RecordCount;
                progressBar2.Value = count;
            }

            while (!rsSync.EOF)
            {
                dFID = int.Parse(rsSync.Fields[0].Value.ToString());
                dFNO = short.Parse(rsSync.Fields[1].Value.ToString());

                if (dFNO != 0)
                {
                    switch (dFNO)
                    {
                        case 6200: updateAndSyncCableCode("GC_PDDP", s_ExcAbb, s_curCableCode, dFID, updateType); break;
                        case 6300: updateAndSyncCableCode("GC_DDP", s_ExcAbb, s_curCableCode, dFID, updateType); break;
                        case 6400: updateAndSyncCableCode("GC_CONTGAUGE", s_ExcAbb, s_curCableCode, dFID, updateType); break;
                        case 6500: updateAndSyncCableCode("GC_CONTALARM", s_ExcAbb, s_curCableCode, dFID, updateType); break;
                        case 6600: updateAndSyncCableCode("GC_GASSEAL", s_ExcAbb, s_curCableCode, dFID, updateType); break;
                        case 6700: updateAndSyncCableCode("GC_TESTPNT", s_ExcAbb, s_curCableCode, dFID, updateType); break;
                        case 6900: updateAndSyncCableCode("GC_TRNSDCR", s_ExcAbb, s_curCableCode, dFID, updateType); break;
                        case 7000: updateAndSyncCableCode("GC_CBL", s_ExcAbb, s_curCableCode, dFID, updateType); break;
                        case 10300: updateAndSyncCableCode("GC_ITFACE", s_ExcAbb, s_curCableCode, dFID, updateType); break;
                        case 10401: updateAndSyncCableCode("GC_LDCOIL", s_ExcAbb, s_curCableCode, dFID, updateType); break;
                        case 10800: updateAndSyncCableCode("GC_SPLICE", s_ExcAbb, s_curCableCode, dFID, updateType); break;
                        case 13000: updateAndSyncCableCode("GC_DP", s_ExcAbb, s_curCableCode, dFID, updateType); break;
                        case 13100: updateAndSyncCableCode("GC_PDP", s_ExcAbb, s_curCableCode, dFID, updateType); break;
                        case 13200: updateAndSyncCableCode("GC_IDF", s_ExcAbb, s_curCableCode, dFID, updateType); break;
                    }
                    rsSync.MoveNext();
                    count++;

                    if (updateType == "Update")
                        progressBar1.Value = count;
                    else
                        progressBar2.Value = count;
                }
                else
                    rsSync.MoveNext(); 
            }

            MessageBox.Show(this," Updates are completed!");

            update_TailSQL = "";
            selectSQL = "";
            rsSync = null;
            rsTail = null;
            deleteTrace(traceResult);
        }

        private void updateAndSyncCableCode(string tableName, string sel_excAbb, string sel_CableCode, int sel_FID, string updateType)
        {
            int iRecordsAffected;
            string chgSQL = "", newReplacedCode = "";

            ADODB.Recordset chgRS = new ADODB.Recordset();
            ADODB.Recordset chgCodeRS = new ADODB.Recordset();
            
            if (updateType == "Update")
            {
                //update text_value
                if (s_CableClass == "TAIL" && s_textValue.IndexOf(currCableCodeTb.Text) != -1)
                {
                    newReplacedCode = s_textValue.Replace(currCableCodeTb.Text, newCableCodeTb.Text);
                    if (newReplacedCode != "")
                    {
                        chgSQL = "UPDATE GC_CBL SET TEXT_VALUE = '" + newReplacedCode + "' WHERE G3E_FID = " + sel_FID;
                        chgCodeRS = GTClassFactory.Create<IGTApplication>().DataContext.Execute(chgSQL, out iRecordsAffected, (int)ADODB.CommandTypeEnum.adCmdText);
                        chgSQL = "COMMIT";
                        if (iRecordsAffected >= 1)
                        {
                            Application.DoEvents();
                        }
                        chgSQL = "";
                        chgCodeRS = null;
                    }
                }
                //updateStatusTb.Text = "Updating " + sel_FID;
                //Application.DoEvents();
            }
            /*else
            {
                syncStatusTb.Text = "Updating " + sel_FID ;
                Application.DoEvents();
            }*/


            //update cable_code
            chgSQL = "UPDATE " + tableName + " SET CABLE_CODE = '" + newCableCodeTb.Text+ "' WHERE G3E_FID = " + sel_FID;
            chgRS = GTClassFactory.Create<IGTApplication>().DataContext.Execute(chgSQL, out iRecordsAffected, (int)ADODB.CommandTypeEnum.adCmdText);
            chgSQL = "COMMIT";
            //if (iRecordsAffected >= 1)
            //{
            //    if (updateType == "Update")
            //    {
            //        updateStatusTb.Text = sel_FID + " is updated!";
            //        Application.DoEvents();
            //    }
            //    else
            //    {
            //        syncStatusTb.Text = sel_FID + " is updated!";
            //        Application.DoEvents();
            //    }
            //}

            chgSQL = "";
            chgRS = null; 
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
            checkCodeBtn.Enabled = true;
            updateCableCode_ChangeBtn.Enabled = false;
            short sInFno = 0;

            try
            {
                if (GTClassFactory.Create<IGTApplication>().SelectedObjects.FeatureCount > 0)
                {
                    foreach (IGTDDCKeyObject oDDCKeyObject in GTClassFactory.Create<IGTApplication>().SelectedObjects.GetObjects())
                    {
                        if ( oDDCKeyObject.FNO == 7000 &&
                           ( oDDCKeyObject.Recordset.Fields["G3E_CNO"].Value.ToString() == "7010" ||
                             oDDCKeyObject.Recordset.Fields["G3E_CNO"].Value.ToString() == "7011" ||
                             oDDCKeyObject.Recordset.Fields["G3E_CNO"].Value.ToString() == "7020" ||
                             oDDCKeyObject.Recordset.Fields["G3E_CNO"].Value.ToString() == "7021" ||
                             oDDCKeyObject.Recordset.Fields["G3E_CNO"].Value.ToString() == "7030" ||
                             oDDCKeyObject.Recordset.Fields["G3E_CNO"].Value.ToString() == "7031" ||
                             oDDCKeyObject.Recordset.Fields["G3E_CNO"].Value.ToString() == "7032" ||
                             oDDCKeyObject.Recordset.Fields["G3E_CNO"].Value.ToString() == "7033" ||
                             oDDCKeyObject.Recordset.Fields["G3E_CNO"].Value.ToString() == "7034" ||
                             oDDCKeyObject.Recordset.Fields["G3E_CNO"].Value.ToString() == "7035" ||
                             oDDCKeyObject.Recordset.Fields["G3E_CNO"].Value.ToString() == "7036" ||
                             oDDCKeyObject.Recordset.Fields["G3E_CNO"].Value.ToString() == "7037" ||
                             oDDCKeyObject.Recordset.Fields["G3E_CNO"].Value.ToString() == "7040" ||
                             oDDCKeyObject.Recordset.Fields["G3E_CNO"].Value.ToString() == "7041" ||
                             oDDCKeyObject.Recordset.Fields["G3E_CNO"].Value.ToString() == "7042" ||
                             oDDCKeyObject.Recordset.Fields["G3E_CNO"].Value.ToString() == "7043" ||
                             oDDCKeyObject.Recordset.Fields["G3E_CNO"].Value.ToString() == "7044" ||
                             oDDCKeyObject.Recordset.Fields["G3E_CNO"].Value.ToString() == "7045" )&&
                           (( oDDCKeyObject.Recordset.Fields["CABLE_CLASS"].Value.ToString() == "TAIL" ) ||
                            ( oDDCKeyObject.Recordset.Fields["CABLE_CLASS"].Value.ToString().IndexOf("E-") != -1 )))
                        {
                            s_FID = oDDCKeyObject.FID;
                            getSelectedSQL = sqlSelectedCable(s_FID);
 
                            sRS = GTClassFactory.Create<IGTApplication>().DataContext.OpenRecordset(getSelectedSQL, ADODB.CursorTypeEnum.adOpenStatic, ADODB.LockTypeEnum.adLockReadOnly, 1);
                            if (sRS.RecordCount > 0)
                            {
                                sRS.MoveFirst();
                                sInFno = short.Parse(sRS.Fields[5].Value.ToString());
                                if (sInFno == 10800)
                                {
                                    MessageBox.Show(this,"Please select TAIL to edit the cable code.", " Manage Cable Code", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                    return;
                                }
                                else
                                {
                                    //exc_abb[0], itface_code[1], cable_class[2], cable_code[3], text_value[4], out_fid[6]
                                    assignValues(sRS.Fields[0].Value.ToString(), sRS.Fields[1].Value.ToString(), sRS.Fields[2].Value.ToString(), sRS.Fields[3].Value.ToString(), sRS.Fields[4].Value.ToString(), int.Parse(sRS.Fields[6].Value.ToString()));
                                    valid = true;
                                }
                            }
                            sRS = null;
                            getSelectedSQL = "";
                            break;
                        }
                        else
                        {
                            if ( oDDCKeyObject.FNO == 7000 &&
                               ( oDDCKeyObject.Recordset.Fields["G3E_CNO"].Value.ToString() == "7010" ||
                                 oDDCKeyObject.Recordset.Fields["G3E_CNO"].Value.ToString() == "7011" ||
                                 oDDCKeyObject.Recordset.Fields["G3E_CNO"].Value.ToString() == "7020" ||
                                 oDDCKeyObject.Recordset.Fields["G3E_CNO"].Value.ToString() == "7021" ||
                                 oDDCKeyObject.Recordset.Fields["G3E_CNO"].Value.ToString() == "7030" ||
                                 oDDCKeyObject.Recordset.Fields["G3E_CNO"].Value.ToString() == "7031" ||
                                 oDDCKeyObject.Recordset.Fields["G3E_CNO"].Value.ToString() == "7032" ||
                                 oDDCKeyObject.Recordset.Fields["G3E_CNO"].Value.ToString() == "7033" ||
                                 oDDCKeyObject.Recordset.Fields["G3E_CNO"].Value.ToString() == "7034" ||
                                 oDDCKeyObject.Recordset.Fields["G3E_CNO"].Value.ToString() == "7035" ||
                                 oDDCKeyObject.Recordset.Fields["G3E_CNO"].Value.ToString() == "7036" ||
                                 oDDCKeyObject.Recordset.Fields["G3E_CNO"].Value.ToString() == "7037" ||
                                 oDDCKeyObject.Recordset.Fields["G3E_CNO"].Value.ToString() == "7040" ||
                                 oDDCKeyObject.Recordset.Fields["G3E_CNO"].Value.ToString() == "7041" ||
                                 oDDCKeyObject.Recordset.Fields["G3E_CNO"].Value.ToString() == "7042" ||
                                 oDDCKeyObject.Recordset.Fields["G3E_CNO"].Value.ToString() == "7043" ||
                                 oDDCKeyObject.Recordset.Fields["G3E_CNO"].Value.ToString() == "7044" ||
                                 oDDCKeyObject.Recordset.Fields["G3E_CNO"].Value.ToString() == "7045" ) &&
                                 oDDCKeyObject.Recordset.Fields["CABLE_CLASS"].Value.ToString().IndexOf("D-") != -1)
                            {
                                s_FID = oDDCKeyObject.FID;
                                getSelectedSQL = sqlSelectedCable(s_FID);
                                sRS = GTClassFactory.Create<IGTApplication>().DataContext.OpenRecordset(getSelectedSQL, ADODB.CursorTypeEnum.adOpenStatic, ADODB.LockTypeEnum.adLockReadOnly, 1);
                                if (sRS.RecordCount > 0)
                                {
                                    sRS.MoveFirst();
                                    sInFno = short.Parse(sRS.Fields[6].Value.ToString());
                                    if (sInFno == 10800)
                                    {
                                        MessageBox.Show(this,"Please select first cable from D-Side\ntermination point to edit the cable code.", " Manage Cable Code", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                        return;
                                    }
                                    else
                                    {
                                        assignValues(sRS.Fields[0].Value.ToString(), sRS.Fields[1].Value.ToString(), sRS.Fields[2].Value.ToString(), sRS.Fields[3].Value.ToString(), sRS.Fields[4].Value.ToString(), int.Parse(sRS.Fields[6].Value.ToString()));
                                        valid = true;
                                    }
                                }
                                sRS = null;
                                getSelectedSQL = "";
                                break;
                            }
                            else
                            {
                                MessageBox.Show(this,"Selected feature is not E-Side or D-Side Cable.\nPlease select ONLY E-Side or D-Side Cable", "Manage Cable Code", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                break;
                            }
                        }
                    }
                }
                else
                {
                    if (GTClassFactory.Create<IGTApplication>().SelectedObjects.FeatureCount > 1)
                    {
                        MessageBox.Show(this,"Please select ONLY ONE E-Side or D-Side Cable", "Manage Cable Code", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show(this,"No cable selected", "Manage Cable Code", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }

                if (valid == true)
                {
                    checkCodeBtn.Enabled = true;
                    newCableCodeTb.Enabled = true;
                }
                else
                    resetButtonsAndTextBox();
                
            }
            catch (Exception ex)
            {
                MessageBox.Show(this,ex.Message, "Manage Cable Code", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void checkCodeBtn_Click(object sender, EventArgs e)
        {
            bool assignCode = true;
            //add checking for non-Capital letters, I, O and Z.

            if (newCableCodeTb.Text[0] <= 64  || newCableCodeTb.Text[0] == 73 || newCableCodeTb.Text[0] == 79 || newCableCodeTb.Text >= 91) 
            {
                MessageBox.Show(this, "Please use ONLY Capital Letters.", "Manage Cable Code", MessageBoxButtons.OK, MessageBoxIcon.Information);
                newCableCodeTb.Text = "";
                return;
            }
            else if (s_curCableCode == newCableCodeTb.Text)
            {
                MessageBox.Show(this,"Selected cable has the same cable code with the New Cable Code.\nPlease re-enter", "Manage Cable Code", MessageBoxButtons.OK, MessageBoxIcon.Information);
                newCableCodeTb.Text = "";
                return;
            }
            else
            {
                // Check if New Cable Code is NULL.
                if (newCableCodeTb.Text == "")
                {
                    MessageBox.Show(this,"Cable Code cannot be NULL.\nPlease enter the code", "Manage Cable Code", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                else
                {
                    // If cable code is NOT NULL, check the new cable code against the selected cable code
                    if (s_curCableCode == "")
                    {
                        DialogResult retVal = MessageBox.Show(this,"Selected cable has NO Cable Code.\nDo you want to assign the cable code?", "Manage Cable Code", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                        //assignCode is set to 1 to enable edit for cable without code if user choose to assign
                        assignCode = (retVal == DialogResult.No) ? false : true;
                    }

                    if (assignCode)
                    {
                        // Begin checking the entered cable code against the existing cable code to check for the available code                
                        if (s_ItfaceCode == "")
                        {
                            if (checkCableCodeAvailability(newCableCodeTb.Text.ToString(), ""))
                            {
                                MessageBox.Show(this,"Cable Code entered is already existed\nfor " + s_ExcAbb + " Exchange. Please re-enter", "Manage Cable Code", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                newCableCodeTb.Text = "";
                                return;
                            }
                            else
                            {
                                updateCableCode_ChangeBtn.Enabled = true;
                                //getReportChgCableCmd.Enabled = true;
                            }
                        }
                        else
                        {
                            if (checkCableCodeAvailability(newCableCodeTb.Text.ToString(), s_ItfaceCode))
                            {
                                MessageBox.Show(this,"Cable Code entered is already existed for\nItface Code no " + s_ItfaceCode + ". Please re-enter", "Manage Cable Code", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                newCableCodeTb.Text = "";
                                return;
                            }
                            else
                            {
                                updateCableCode_ChangeBtn.Enabled = true;
                                //getReportChgCableCmd.Enabled = true;
                            } 
                        }
                    }
                    else
                    {
                        newCableCodeTb.Text = "";
                        return;
                    }
                }
            }
        }
        #endregion

        private void UpdateCableCode_ChangeBtn_Click(object sender, EventArgs e)
        {
            try
            {
                GTManageCableCode.m_oIGTTransactionManager.Begin("ManageCableCode");
                if (s_CableClass.IndexOf("E-") != 1 || s_CableClass.IndexOf("D-") != 1)
                {
                    changeCableCodeOnClick(s_FID, "Update");
                    resetButtonsAndTextBox();
                }
                GTManageCableCode.m_oIGTTransactionManager.Commit();
                GTManageCableCode.m_oIGTTransactionManager.RefreshDatabaseChanges();
            }
            catch(Exception ex)
            {
                MessageBox.Show(this,ex.Message, "Manage Cable Code", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #region Synchronize Cable Code
        //other variables for various purpose
        bool found = false;

        //related variables for source cable
        int srcFID = 0, srcOutFID = 0;
        short srcOutFno;
        string srcExcAbb = "", strSrcSQL = "";

        //related variables for destination cable
        int dstFID = 0, dstInFID = 0;
        string dstExcAbb = "", strDestSQL = "";

        //all ADODB recordset for source, destination and synchronization purpose
        ADODB.Recordset rsSrc = new ADODB.Recordset();
        ADODB.Recordset rsDest = new ADODB.Recordset();

        private void cmdSynGetSrc_Click(object sender, EventArgs e)
        {
            try
            {
                if (GTClassFactory.Create<IGTApplication>().SelectedObjects.FeatureCount == 1)
                {
                    foreach (IGTDDCKeyObject oDDCKeyObject in GTClassFactory.Create<IGTApplication>().SelectedObjects.GetObjects())
                    {
                        if ( oDDCKeyObject.FNO == 7000 &&
                           ( oDDCKeyObject.Recordset.Fields["G3E_CNO"].Value.ToString() == "7010" ||
                             oDDCKeyObject.Recordset.Fields["G3E_CNO"].Value.ToString() == "7011" ||
                             oDDCKeyObject.Recordset.Fields["G3E_CNO"].Value.ToString() == "7020" ||
                             oDDCKeyObject.Recordset.Fields["G3E_CNO"].Value.ToString() == "7021" ||
                             oDDCKeyObject.Recordset.Fields["G3E_CNO"].Value.ToString() == "7030" ||
                             oDDCKeyObject.Recordset.Fields["G3E_CNO"].Value.ToString() == "7031" ||
                             oDDCKeyObject.Recordset.Fields["G3E_CNO"].Value.ToString() == "7032" ||
                             oDDCKeyObject.Recordset.Fields["G3E_CNO"].Value.ToString() == "7033" ||
                             oDDCKeyObject.Recordset.Fields["G3E_CNO"].Value.ToString() == "7034" ||
                             oDDCKeyObject.Recordset.Fields["G3E_CNO"].Value.ToString() == "7035" ||
                             oDDCKeyObject.Recordset.Fields["G3E_CNO"].Value.ToString() == "7036" ||
                             oDDCKeyObject.Recordset.Fields["G3E_CNO"].Value.ToString() == "7037" ||
                             oDDCKeyObject.Recordset.Fields["G3E_CNO"].Value.ToString() == "7040" ||
                             oDDCKeyObject.Recordset.Fields["G3E_CNO"].Value.ToString() == "7041" ||
                             oDDCKeyObject.Recordset.Fields["G3E_CNO"].Value.ToString() == "7042" ||
                             oDDCKeyObject.Recordset.Fields["G3E_CNO"].Value.ToString() == "7043" ||
                             oDDCKeyObject.Recordset.Fields["G3E_CNO"].Value.ToString() == "7044" ||
                             oDDCKeyObject.Recordset.Fields["G3E_CNO"].Value.ToString() == "7045"))
                        {
                            srcFID = oDDCKeyObject.FID;
                            strSrcSQL =
                                "SELECT A.EXC_ABB, B.CABLE_CODE, C.OUT_FNO, C.OUT_FID " +
                                "FROM GC_NETELEM A,GC_CBL B, GC_NR_CONNECT C " +
                                "WHERE A.G3E_FID = B.G3E_FID AND B.G3E_FID = C.G3E_FID AND C.G3E_FID =  " + srcFID;
                            rsSrc = GTClassFactory.Create<IGTApplication>().DataContext.OpenRecordset(strSrcSQL, ADODB.CursorTypeEnum.adOpenStatic, ADODB.LockTypeEnum.adLockReadOnly, 1);

                            if (rsSrc.RecordCount > 0)
                            {
                                rsSrc.MoveFirst();
                                srcExcAbb = rsSrc.Fields[0].Value.ToString();
                                txtSynSrcCblCode.Text = rsSrc.Fields[1].Value.ToString();
                                srcOutFno = short.Parse(rsSrc.Fields[2].Value.ToString());
                                srcOutFID = int.Parse(rsSrc.Fields[3].Value.ToString());

                                /*
                                 * VALIDATION PURPOSE : To check whether user select cable that has 'JOINT' as its' termination point 
                                 */
                                if (srcOutFno == 10800)
                                {
                                    MessageBox.Show(this,"Please select cable that is TERMINATED at\nCAB, PCAB, SDF, PDDP, PDP or IDF.", " Manage Cable Code", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                    txtSynSrcCblCode.Text = "";
                                    return;
                                }
                            }
                            rsSrc = null;
                            strSrcSQL = "";
                            break;
                        }
                    }
                }
                else
                    if (GTClassFactory.Create<IGTApplication>().SelectedObjects.FeatureCount > 1)
                        MessageBox.Show(this,"Please select ONLY ONE cable.", "Manage Cable Code", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    else
                        MessageBox.Show(this,"No cable selected", "Manage Cable Code", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this,ex.Message, "Manage Cable Code", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void cmdSynGetDst_Click(object sender, EventArgs e)
        {
            try
            { 
                if (GTClassFactory.Create<IGTApplication>().SelectedObjects.FeatureCount == 1)
                {
                    foreach (IGTDDCKeyObject oDDCKeyObject in GTClassFactory.Create<IGTApplication>().SelectedObjects.GetObjects())
                    {
                        if (oDDCKeyObject.FNO == 7000 &&
                           (oDDCKeyObject.Recordset.Fields["G3E_CNO"].Value.ToString() == "7010" ||
                             oDDCKeyObject.Recordset.Fields["G3E_CNO"].Value.ToString() == "7011" ||
                             oDDCKeyObject.Recordset.Fields["G3E_CNO"].Value.ToString() == "7020" ||
                             oDDCKeyObject.Recordset.Fields["G3E_CNO"].Value.ToString() == "7021" ||
                             oDDCKeyObject.Recordset.Fields["G3E_CNO"].Value.ToString() == "7030" ||
                             oDDCKeyObject.Recordset.Fields["G3E_CNO"].Value.ToString() == "7031" ||
                             oDDCKeyObject.Recordset.Fields["G3E_CNO"].Value.ToString() == "7032" ||
                             oDDCKeyObject.Recordset.Fields["G3E_CNO"].Value.ToString() == "7033" ||
                             oDDCKeyObject.Recordset.Fields["G3E_CNO"].Value.ToString() == "7034" ||
                             oDDCKeyObject.Recordset.Fields["G3E_CNO"].Value.ToString() == "7035" ||
                             oDDCKeyObject.Recordset.Fields["G3E_CNO"].Value.ToString() == "7036" ||
                             oDDCKeyObject.Recordset.Fields["G3E_CNO"].Value.ToString() == "7037" ||
                             oDDCKeyObject.Recordset.Fields["G3E_CNO"].Value.ToString() == "7040" ||
                             oDDCKeyObject.Recordset.Fields["G3E_CNO"].Value.ToString() == "7041" ||
                             oDDCKeyObject.Recordset.Fields["G3E_CNO"].Value.ToString() == "7042" ||
                             oDDCKeyObject.Recordset.Fields["G3E_CNO"].Value.ToString() == "7043" ||
                             oDDCKeyObject.Recordset.Fields["G3E_CNO"].Value.ToString() == "7044" ||
                             oDDCKeyObject.Recordset.Fields["G3E_CNO"].Value.ToString() == "7045"))
                        {
                            dstFID = oDDCKeyObject.FID;
                            strDestSQL =
                                   "SELECT A.EXC_ABB, B.CABLE_CODE, B.CABLE_CLASS, C.IN_FID " +
                                   "FROM GC_NETELEM A, GC_CBL B, GC_NR_CONNECT C " +
                                   "WHERE A.G3E_FID = B.G3E_FID AND B.G3E_FID = C.G3E_FID AND C.G3E_FID =" + dstFID;

                            rsDest = GTClassFactory.Create<IGTApplication>().DataContext.OpenRecordset(strDestSQL, ADODB.CursorTypeEnum.adOpenStatic, ADODB.LockTypeEnum.adLockReadOnly, 1);
                            if (rsDest.RecordCount > 0)
                            {
                                rsDest.MoveFirst();
                                dstExcAbb = rsDest.Fields[0].Value.ToString();
                                txtSynDstCableCode.Text = rsDest.Fields[1].Value.ToString();
                                d_CableClass = rsDest.Fields[2].Value.ToString();
                                dstInFID = int.Parse(rsDest.Fields[3].Value.ToString());
                            }
                            rsDest = null;
                            strDestSQL = "";
                            found = true;

                            if (srcOutFID != dstInFID)
                            {
                                MessageBox.Show(this, "Selected cable does not come from the same SOURCE.\nPlease re-select.", "Manage Cable Code", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                txtSynDstCableCode.Text = "";
                                return;
                            }
                            else
                            {
                                if (txtSynDstCableCode.Text == "")
                                {
                                    DialogResult retVal = MessageBox.Show(this, "Destination cable has NO cable code.\nDo you want to synchronize this cable?", "Manage Cable Code", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                                    //cmdSynchronize.Enabled = (retVal == DialogResult.Yes) ? true : false;

                                    if (retVal == DialogResult.Yes)
                                        cmdSynchronize.Enabled = true;
                                }
                                else
                                {
                                    if (txtSynSrcCblCode.Text == txtSynDstCableCode.Text)
                                    {
                                        DialogResult retVal = MessageBox.Show(this, "Destination Cable and Source Cable have same cable code.\nDo you want to synchronize this cable?", "Manage Cable Code", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                                        cmdSynchronize.Enabled = (retVal == DialogResult.Yes) ? true : false;
                                    }
                                    else
                                        cmdSynchronize.Enabled = true;
                                }
                            }
                            break;
                        }
                    }
                }
                else
                {
                    if (GTClassFactory.Create<IGTApplication>().SelectedObjects.FeatureCount > 1)
                        MessageBox.Show(this,"Please select ONLY ONE cable.", "Manage Cable Code", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    else
                        MessageBox.Show(this,"No cable selected", "Manage Cable Code", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(this,ex.Message, "Manage Cable Code", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void cmdSynchronize_Click(object sender, EventArgs e)
        {
            try
            {
                GTManageCableCode.m_oIGTTransactionManager.Begin("ManageCableCode");
 
                if( d_CableClass.IndexOf("E-") != -1 || d_CableClass.IndexOf("D-") != 1)
                {
                    changeCableCodeOnClick(dstFID, "Synchronize");
                    resetButtonsAndTextBox();
                }
                GTManageCableCode.m_oIGTTransactionManager.Commit();
                GTManageCableCode.m_oIGTTransactionManager.RefreshDatabaseChanges();
            }
            catch (Exception ex)
            {
                MessageBox.Show(this,ex.Message, "Manage Cable Code", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion
    }
}