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

namespace NEPS.GTechnology.CoreCount
{
    public partial class GTWindowsForm_CoreCount : Form
    {
        public static IGTApplication m_GeoApp;
        private Logger log;

        IGTDDCKeyObjects oGTKeyObjs;

        //GLobals

        private short FCBL_FNO = 7200;      // Fibre Cable
        private short FDCBL_FNO = 7400;     // FD Cable
        private short TRCBL_FNO = 4400;     // Trunk Cable
        private short JNCBL_FNO = 4500;     // Junction Cable
        
        private short FDC_FNO = 5100;       // FDC
        private short UPE_FNO = 5400;       // UPE
        
        private short FDP_FNO = 5600;       // FDP
        private short FTB_FNO = 5900;       // FTB
        private short TIE_FNO = 5800;       // TIE
        private short DB_FNO = 9900;        // DB

        private short MSAN_FNO = 9100;      // MSAN
        private short DDN_FNO = 9300;       // DDN
        private short NDH_FNO = 9400;       // NDH
        private short MUX_FNO = 9500;       // MUX
        private short EPE_FNO = 5200;       // EPE
        private short FAN_FNO = 9700;       // FAN
        private short RT_FNO = 9600;        // RT
        private short VDSL_FNO = 9800;      // VDSL2

        //Detail Window
        private short ODF_FNO = 5500;       // ODF
        private short FPATCH_FNO = 4900;    // FPATCH
        private short FPP_FNO = 12200;      // Fibre Patch Panel

        //SPLICE AND JOINTS
        private short FSE_FNO = 11800;      // Fibre Splice Enclosure
        private short FST_FNO = 4600;       // Fiber Splice Trunk
        private short FSJ_FNO = 4700;       // Fiber Splice Junction

        private short SHELF_FNO = 15800;    // Fiber Shelf
        private short SPLITTER_FNO = 12300; // Fiber Splitter
        private short CARD_FNO = 15900;     // Fiber Card

        private int edit_g3e_id = 0;
       
        public bool flag = false;
        private string ownership;

        private IGTDataContext m_IGTDataContext = null;

        public GTWindowsForm_CoreCount()
        {
            try
            {
                InitializeComponent();
                m_gtapp = GTClassFactory.Create<IGTApplication>();
                m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Running Core Count...");

                log = Logger.getInstance();

                this.m_IGTDataContext = GTClassFactory.Create<IGTApplication>().DataContext;

                LoadForm();
               
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "SpliceConnect", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btn_ChangeDevice_Click(object sender, EventArgs e)
        {
            LoadForm();
        }

        private void LoadForm()
        {
            try
            {                
                Select_Feature();

                if (flag != true)
                {
                    grp_Core.Visible = false;
                    grp_Port.Visible = false;
                    btn_Swap.Enabled = true;
                    btn_Connect.Enabled = true;
                    btn_Protection.Enabled = true;
                    btn_Foms.Enabled = false;
                    btn_Spare.Enabled = false;
                    lbl_CoreMSG.Visible = false;
                    lbl_CoreMSG.Text = "";

                    if (lbl_FID.Text == FSE_FNO.ToString())
                    {
                        button4.Image = button3.Image;
                        label2.Text = "Cable :";
                        label6.Text = "Low Core";
                        label5.Text = "High Core";
                        btn_Foms.Enabled = true;
                        btn_Spare.Enabled = true;
                        btn_Connect.Enabled = false;
                        btn_Protection.Enabled = false;
                    }

                    cboFrom.Items.Clear();
                    cboTo.Items.Clear();
                    txtLOW1.Text = "";
                    txtHIGH1.Text = "";
                    txtLOW2.Text = "";
                    txtHIGH2.Text = "";
                    lbl_FromCore.Text = "";
                    lbl_ToCore.Text = "";

                    Clear_Grid();

                    Load_Combo();
                    Select_Splice();

                    if (lbl_FID.Text != "" && lbl_fno.Text != "")
                    {
                        //Display the Available Core
                        Get_Available_Core(lbl_FID.Text, lbl_fno.Text);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "SpliceConnect", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private Intergraph.GTechnology.API.IGTApplication m_gtapp = null;

        private void GTWindowsForm_CoreCount_Shown(object sender, EventArgs e)
        {
            m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Running Core Count...");
            if (flag == true)
            {
                this.Close();
            }

        }

        private void GTWindowsForm_CoreCount_Activated(object sender, EventArgs e)
        {
            m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Running Core Count...");
        }

        private void GTWindowsForm_CoreCount_Load(object sender, EventArgs e)
        {
            this.m_IGTDataContext = GTClassFactory.Create<IGTApplication>().DataContext;

           // gv_Route.CellPainting += new DataGridViewCellPaintingEventHandler(gv_Route_CellPainting);
            txtLOW1.KeyPress += new KeyPressEventHandler(txtLOW1_KeyPress);
            txtLOW2.KeyPress += new KeyPressEventHandler(txtLOW2_KeyPress);
            txtHIGH1.KeyPress += new KeyPressEventHandler(txtHIGH1_KeyPress);
            txtHIGH2.KeyPress += new KeyPressEventHandler(txtHIGH2_KeyPress);                             

        }

        void txtHIGH2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsNumber(e.KeyChar))
            {
                if (Regex.IsMatch(txtHIGH2.Text, "\\D+"))
                {
                    e.Handled = true;
                }
            }
            else
            {
                e.Handled = e.KeyChar != (char)Keys.Back;
            }
        }

        void txtHIGH1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsNumber(e.KeyChar))
            {
                if (Regex.IsMatch(txtHIGH1.Text, "\\D+"))
                {
                    e.Handled = true;
                }
            }
            else
            {
                e.Handled = e.KeyChar != (char)Keys.Back;
            }
        }

        void txtLOW2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsNumber(e.KeyChar))
            {
                if (Regex.IsMatch(txtLOW2.Text, "\\D+"))
                {
                    e.Handled = true;
                }
            }
            else
            {
                e.Handled = e.KeyChar != (char)Keys.Back;
            }
        }

        void txtLOW1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsNumber(e.KeyChar))
            {
                if (Regex.IsMatch(txtLOW1.Text, "\\D+"))
                {
                    e.Handled = true;
                }
            }
            else
            {
                e.Handled = e.KeyChar != (char)Keys.Back;
            }
        }
               
        //Create Grid Columns
        private void Create_Grid()
        {
            try
            {

            }
            catch (Exception ex)
            {
                log.WriteLog("");
                log.WriteLog(this.GetType().FullName, "ERROR: " + ex.Message);
                log.WriteLog(ex.StackTrace);
                MessageBox.Show(ex.Message, "Core Count", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        //Get the Splice details for selected Feature
        private void Select_Splice()
        {
            string gFNO1 = null;
            string gFID1= null;
            string gFNO1_Desc = null;
            string gFID1_Desc = null;
            string gLOW1= null; 
            string gHIGH1= null; 
            string gFNO2= null; 
            string gFID2= null;
            string gFNO2_Desc = null;
            string gFID2_Desc = null; 
            string gLOW2= null; 
            string gHIGH2= null;
            string gCID = null;
            string gG3E_ID = null;
            string ppwo = null;
            string sSql = null;
            string Condition = null;
            string Conn_Type = null;
            string PanelNo = null;
            string EXC_ABB = null;
            string CABLE_CODE = null;
            string FEATURE_STATE = null;
            string TERM_FNO= null;
            string TERM_FID= null;
            string TERM_LOW= null;
            string TERM_HIGH = null;
            string COLOR = null;

            try
            {
                if (lbl_FID.Text != "")
                {
                    //GTClassFactory.Create<IGTApplication>().BeginWaitCursor();
                    ADODB.Recordset rs = new ADODB.Recordset();
                    Clear_Grid();
                    Condition = " WHERE g3e_fid = " + lbl_FID.Text;

                    //if (cboFrom.Text.Trim() != "")
                    //    Condition = Condition + " and  ( SC.FID1 = " + GetComboValue(cboFrom, cboFrom.SelectedItem.ToString()) + " OR SC.FID2 = " + GetComboValue(cboFrom, cboFrom.SelectedItem.ToString()) + ")";
                    //if (cboTo.Text.Trim() != "")
                    //    Condition = Condition + " and  ( SC.FID2 = " + GetComboValue(cboTo, cboTo.SelectedItem.ToString()) + " OR SC.FID1 = " + GetComboValue(cboTo, cboTo.SelectedItem.ToString()) + ")";


                    sSql = "SELECT SC.G3E_FNO, SC.G3E_FID, SC.FNO1, SC.FID1,SC.LOW1, SC.HIGH1, SC.FNO2, SC.FID2, SC.LOW2, SC.HIGH2, SC.G3E_CID, sc.g3e_id, SC.WORKORDER, SC.CORE_STATUS, " +
                           " SC.EXC_ABB, SC.CABLE_CODE, SC.FEATURE_STATE, SC.TERM_FNO, SC.TERM_FID, SC.TERM_LOW, SC.TERM_HIGH " +
                           " FROM gc_splice_connect SC " + Condition + " ORDER BY FID1, LOW1";

                    rs = m_IGTDataContext.OpenRecordset(sSql, ADODB.CursorTypeEnum.adOpenStatic, ADODB.LockTypeEnum.adLockReadOnly, 1);
                    if (rs.RecordCount == 0)
                    {
                        //MessageBox.Show("No Records Found.", "Core Count", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;

                    }

                    rs.MoveFirst();
                    do
                    {
                        gFNO1 = rs.Fields[2].Value.ToString();
                        gFID1 = rs.Fields[3].Value.ToString();

                        gFNO1_Desc = Get_Value("Select g3e_username from g3e_feature where g3e_fno = " + gFNO1);
                        if (gFNO1 == "7200")
                            gFID1_Desc = Get_Value("select CABLE_CODE ||' (' || g3e_fid || ')' from GC_fcbl where g3e_fid = " + gFID1);
                        else if (gFNO1 == "7400")
                            gFID1_Desc = Get_Value("select FCABLE_CODE ||' (' || g3e_fid || ')' from GC_FDCBL where g3e_fid = " + gFID1);
                        else if (gFNO1 == "4400")
                            gFID1_Desc = Get_Value("select CABLE_CODE ||' (' || g3e_fid || ')' from GC_FCBL_TRUNK where g3e_fid = " + gFID1);
                        else if (gFNO1 == "4500")
                            gFID1_Desc = Get_Value("select CABLE_CODE ||' (' || g3e_fid || ')' from GC_FCBL_JNT where g3e_fid = " + gFID1);
                        else
                            gFID1_Desc = gFID1;
                                               
                        gLOW1 = rs.Fields[4].Value.ToString();
                        gHIGH1 = rs.Fields[5].Value.ToString();

                        gFNO2 = rs.Fields[6].Value.ToString();
                        gFID2 = rs.Fields[7].Value.ToString();

                        gFNO2_Desc = Get_Value("Select g3e_username from g3e_feature where g3e_fno = " + gFNO2);
                        
                        if (gFNO2 == FDC_FNO.ToString())
                        {
                            gFID2_Desc = Get_Value("select FDC_CODE ||' (' || g3e_fid || ')' from GC_FDC where g3e_fid = " + gFID2);
                        }
                        else if (gFNO2 == UPE_FNO.ToString())
                        {
                            gFID2_Desc = Get_Value("select UPE_CODE ||' (' || g3e_fid || ')' from GC_UPE where g3e_fid = " + gFID2);
                        }
                        else if (gFNO2 == FDP_FNO.ToString())
                        {
                            gFID2_Desc = Get_Value("select FDP_CODE ||' (' || g3e_fid || ')' from GC_FDP where g3e_fid = " + gFID2);
                        }
                        else if (gFNO2 == FTB_FNO.ToString())
                        {
                            gFID2_Desc = Get_Value("select FDP_CODE ||' (' || g3e_fid || ')' from GC_FTB where g3e_fid = " + gFID2);
                        }
                        else if (gFNO2 == TIE_FNO.ToString())
                        {
                            gFID2_Desc = Get_Value("select FDP_CODE ||' (' || g3e_fid || ')' from GC_TIEFDP where g3e_fid = " + gFID2);
                        }
                        else if (gFNO2 == DB_FNO.ToString())
                        {
                            gFID2_Desc = Get_Value("select DB_CODE ||' (' || g3e_fid || ')' from GC_DB where g3e_fid = " + gFID2);
                        }
                        else if (gFNO2 == MSAN_FNO.ToString())
                        {
                            gFID2_Desc = Get_Value("select RT_CODE ||' (' || g3e_fid || ')' from GC_MSAN where g3e_fid = " + gFID2);
                        }
                        else if (gFNO2 == DDN_FNO.ToString())
                        {
                            gFID2_Desc = Get_Value("select DDN_CODE ||' (' || g3e_fid || ')' from GC_DDN where g3e_fid = " + gFID2);
                        }
                        else if (gFNO2 == NDH_FNO.ToString())
                        {
                            gFID2_Desc = Get_Value("select NDH_CODE ||' (' || g3e_fid || ')' from GC_NDH where g3e_fid = " + gFID2);
                        }
                        else if (gFNO2 == MUX_FNO.ToString())
                        {
                            gFID2_Desc = Get_Value("select MUX_CODE ||' (' || g3e_fid || ')' from GC_MINIMUX where g3e_fid = " + gFID2);
                        }
                        else if (gFNO2 == EPE_FNO.ToString())
                        {
                            gFID2_Desc = Get_Value("select EPE_CODE ||' (' || g3e_fid || ')' from GC_EPE where g3e_fid = " + gFID2);
                        }
                        else if (gFNO2 == FAN_FNO.ToString())
                        {
                            gFID2_Desc = Get_Value("select FAN_CODE ||' (' || g3e_fid || ')' from GC_FAN where g3e_fid = " + gFID2);
                        }
                        else if (gFNO2 == RT_FNO.ToString())
                        {
                            gFID2_Desc = Get_Value("select RT_CODE ||' (' || g3e_fid || ')' from GC_RT where g3e_fid = " + gFID2);
                        }
                        else if (gFNO2 == ODF_FNO.ToString())
                        {
                            gFID2_Desc = Get_Value("select ODF_NUM ||' (' || g3e_fid || ')' from GC_ODF where g3e_fid = " + gFID2);
                        }
                        else if (gFNO2 == FPATCH_FNO.ToString())
                        {
                            gFID2_Desc = Get_Value("select PATCH_CODE ||' (' || g3e_fid || ')' from GC_FPATCH where g3e_fid = " + gFID2);
                        }
                        else if (gFNO2 == FPP_FNO.ToString())
                        {
                            gFID2_Desc = Get_Value("select PATCH_CODE ||' (' || g3e_fid || ')' from GC_FPATCHPANEL where g3e_fid = " + gFID2);
                        }
                        else if (gFNO2 == FSE_FNO.ToString())
                        {
                            gFID2_Desc = Get_Value("select SPLICE_CODE ||' (' || g3e_fid || ')' from GC_FSPLICE where g3e_fid = " + gFID2);
                        }
                        else if (gFNO2 == SPLITTER_FNO.ToString())
                        {
                            //gFID2_Desc = Get_Value("select SPLITTER_CODE from GC_FSPLITTER where g3e_fid = " + gFID2);
                            //Requst from Mike on 11-Aug-2012
                            gFID2_Desc = Get_Value("select NO_OF_SPLITTER ||' (' || g3e_fid || ')' from GC_FSPLITTER where g3e_fid = " + gFID2);
                        }
                        else if (gFNO2 == "7200")
                        {
                            gFID2_Desc = Get_Value("select CABLE_CODE ||' (' || g3e_fid || ')' from GC_fcbl where g3e_fid = " + gFID2);
                        }
                        else if (gFNO2 == "7400")
                        {
                            gFID2_Desc = Get_Value("select FCABLE_CODE ||' (' || g3e_fid || ')' from GC_FDCBL where g3e_fid = " + gFID2);
                        }
                        else if (gFNO2 == "4400")
                        {
                            gFID2_Desc = Get_Value("select CABLE_CODE ||' (' || g3e_fid || ')' from GC_FCBL_TRUNK where g3e_fid = " + gFID2);
                        }
                        else if (gFNO2 == "4500")
                        {
                            gFID2_Desc = Get_Value("select CABLE_CODE ||' (' || g3e_fid || ')' from GC_FCBL_JNT where g3e_fid = " + gFID2);
                        }
                        else
                            gFID2_Desc = gFID2;

                        gLOW2 = rs.Fields[8].Value.ToString();
                        gHIGH2 = rs.Fields[9].Value.ToString();
                        gCID = rs.Fields[10].Value.ToString();
                        gG3E_ID = rs.Fields[11].Value.ToString();
                        ppwo = rs.Fields[12].Value.ToString();
                        Conn_Type = rs.Fields[13].Value.ToString();
                        EXC_ABB = rs.Fields[14].Value.ToString();
                        CABLE_CODE= rs.Fields[15].Value.ToString();
                        FEATURE_STATE= rs.Fields[16].Value.ToString();
                        TERM_FNO= rs.Fields[17].Value.ToString();
                        TERM_FID= rs.Fields[18].Value.ToString();
                        TERM_LOW= rs.Fields[19].Value.ToString();
                        TERM_HIGH = rs.Fields[20].Value.ToString();
                        
                        COLOR = "";
                        if (cboFrom.Text.Trim() != "" && (gFID1 == GetComboValue(cboFrom, cboFrom.SelectedItem.ToString()) || gFID2 == GetComboValue(cboFrom, cboFrom.SelectedItem.ToString())))
                            COLOR = "YES";
                        if (cboTo.Text.Trim() != "" && (gFID2 == GetComboValue(cboTo, cboTo.SelectedItem.ToString()) || gFID1 == GetComboValue(cboTo, cboTo.SelectedItem.ToString())))
                            COLOR = "YES";

                        AddtoGrid(gFNO1, gFID1, gFNO1_Desc, gFID1_Desc, gLOW1, gHIGH1, gFNO2, gFID2, gFNO2_Desc, gFID2_Desc, gLOW2, gHIGH2, gCID, gG3E_ID, ppwo, Conn_Type, EXC_ABB, CABLE_CODE, FEATURE_STATE, TERM_FNO, TERM_FID, TERM_LOW, TERM_HIGH, COLOR);

                        rs.MoveNext();
                    }
                    while (!rs.EOF);

                    //GTClassFactory.Create<IGTApplication>().EndWaitCursor();                   
                    
                }
            }
            catch (Exception ex)
            {
                //GTClassFactory.Create<IGTApplication>().EndWaitCursor();
                log.WriteLog("");
                log.WriteLog(this.GetType().FullName, "ERROR: " + ex.Message);
                log.WriteLog(ex.StackTrace);
                MessageBox.Show(ex.Message, "Core Count", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
               

        //Get the Splice details for selected Feature
        private void Load_Combo()
        {
            string gFNO1 = null;
            string gFID1 = null;
            string gFNO1_Desc = null;
            string gFID1_Desc = null;
            string gFNOName = string.Empty;
            string Desc = string.Empty;
            
            string gFNO2 = null;
            string gFID2 = null;
            string gFNO2_Desc = null;
            string gFID2_Desc = null;
            
            string gG3E_ID = null;
            string sSqlFrom = null;
            string sSqlTo = null;


            switch (Convert.ToInt32(lbl_fno.Text))
            {                
                case 5100:  //FDC
                case 5600:  //FDP
                case 5200:  //EPE
                case 9300:  //DDN
                case 9100:  //IPMSAN
                case 9500:  //MUX
                case 9400:  //NDH
                case 9600:  //RT
                case 5400:  //UPE
                case 5900:  //FTB
                case 9900:  //DB
                    sSqlFrom = "SELECT DISTINCT g3e_fid, g3e_fno FROM gc_nr_connect WHERE out_fid = " + lbl_FID.Text + " UNION SELECT DISTINCT g3e_fid, g3e_fno FROM gc_nr_connect WHERE IN_fid = " + lbl_FID.Text + "   ORDER BY g3e_fid";
                    sSqlTo = "select G3E_FID, g3e_fno from gc_ownership where owner1_id = (select G3E_ID from gc_ownership where g3e_fid =" + lbl_FID.Text + " )";
                    break;

                default:
                    sSqlFrom = "SELECT DISTINCT g3e_fid, g3e_fno FROM gc_nr_connect WHERE out_fid = " + lbl_FID.Text + " UNION SELECT DISTINCT g3e_fid, g3e_fno FROM gc_nr_connect WHERE IN_fid = " + lbl_FID.Text + "   ORDER BY g3e_fid";
                    //sSqlTo = "SELECT DISTINCT in_fid, in_fno FROM gc_nr_connect WHERE in_fid = " + lbl_FID.Text + " union SELECT DISTINCT out_fid, out_fno  FROM gc_nr_connect WHERE out_fid = " + lbl_FID.Text + " ORDER BY in_fid";
                    sSqlTo = "SELECT DISTINCT g3e_fid, g3e_fno FROM gc_nr_connect WHERE out_fid = " + lbl_FID.Text + " UNION SELECT DISTINCT g3e_fid, g3e_fno FROM gc_nr_connect WHERE IN_fid = " + lbl_FID.Text + "   ORDER BY g3e_fid";
                    break;
            }
                    
            try
            {
                if (lbl_FID.Text != "")
                {
                    cboFrom.Items.Clear();
                    cboFrom.Items.Add(new cboItem(" ", " "));
                    cboTo.Items.Clear();
                    cboTo.Items.Add(new cboItem(" ", " "));

                    ADODB.Recordset rs1 = new ADODB.Recordset();
                    rs1 = m_IGTDataContext.OpenRecordset(sSqlFrom, ADODB.CursorTypeEnum.adOpenStatic, ADODB.LockTypeEnum.adLockReadOnly, 1);

                    if (rs1.RecordCount > 0)
                    {
                        rs1.MoveFirst();
                        do
                        {
                            gFNO1 = rs1.Fields[1].Value.ToString();
                            gFID1 = rs1.Fields[0].Value.ToString();

                            string MIN_MAT = Get_Value("select MIN_MATERIAL from GC_NETELEM where G3E_FID = " + gFID1);
                            if (MIN_MAT == "")
                            {
                                MessageBox.Show("No Plant Unit selected for the Cable connected to the Device", "Core Count", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                return;
                            }

                            gFNO1_Desc = Get_Value("Select g3e_username from g3e_feature where g3e_fno = " + gFNO1);

                            if (gFNO1 == "7200")
                                gFID1_Desc = Get_Value("select CABLE_CODE ||' (' || EFF_CORE || ')' from GC_fcbl where g3e_fid = " + gFID1);
                            else if (gFNO1 == "7400")
                                gFID1_Desc = Get_Value("select FCABLE_CODE ||' (' || CORE_NUM || ')' from GC_FDCBL where g3e_fid = " + gFID1);
                            else if (gFNO1 == "4400")
                                gFID1_Desc = Get_Value("select CABLE_CODE ||' (' || EFF_CORE || ')' from GC_FCBL_TRUNK where g3e_fid = " + gFID1);
                            else if (gFNO1 == "4500")
                                gFID1_Desc = Get_Value("select CABLE_CODE ||' (' || EFF_CORE || ')' from GC_FCBL_JNT where g3e_fid = " + gFID1);
                            else
                                gFID1_Desc = gFID1;

                            //Desc = gFNO1_Desc + " - " + gFID1_Desc;
                            Desc = gFID1_Desc + " - " + gFID1;
                            cboFrom.Items.Add(new cboItem(Desc, gFID1));
                            //cboTo.Items.Add(new cboItem(Desc, gFID1));
                            rs1.MoveNext();
                        }
                        while (!rs1.EOF);

                    }
                    else
                    {
                        MessageBox.Show("There are no related feature.", "Core Count", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    rs1.Close();                    

                    //Vinod 09-Aug-2012 for the below 3 devices the out is always the Device.
                    if (lbl_fno.Text == "5500" || lbl_fno.Text == "4900" || lbl_fno.Text == "12200" || lbl_fno.Text == "9100" || lbl_fno.Text == "9600" || lbl_fno.Text == "5800")
                    {
                        gFNO2 = lbl_fno.Text;
                        gFID2 = lbl_FID.Text;
                        gFNO2_Desc = Get_Value("Select g3e_username from g3e_feature where g3e_fno = " + gFNO2);

                        if (gFNO2 == "5500")
                            gFID2_Desc = Get_Value("select ODF_NUM from GC_ODF where g3e_fid = " + gFID2);
                        else if (gFNO2 == "4900")
                            gFID2_Desc = Get_Value("select PATCH_CODE from GC_FPATCHPANEL where g3e_fid = " + gFID2);
                        else if (gFNO2 == "12200")
                            gFID2_Desc = Get_Value("select PATCH_CODE from GC_FPATCH where g3e_fid = " + gFID2);
                        else if (gFNO2 == "9100")
                            gFID2_Desc = Get_Value("select RT_CODE from GC_MSAN where g3e_fid = " + gFID2);
                        else if (gFNO2 == "9600")
                            gFID2_Desc = Get_Value("select RT_CODE from GC_RT where g3e_fid = " + gFID2);
                        else if (gFNO2 == "5800")
                            gFID2_Desc = Get_Value("select FDP_CODE from GC_TIEFDP where g3e_fid = " + gFID2);
                        else
                            gFID2_Desc = gFID2;

                        //Desc = gFNO2_Desc + " - " + gFID2_Desc;
                        Desc = gFID2_Desc + " - " + gFID2;
                        //cboFrom.Items.Add(new cboItem(Desc, gFID2));
                        cboTo.Items.Add(new cboItem(Desc, gFID2));
                    }
                    else
                    {
                        ADODB.Recordset rs2 = new ADODB.Recordset();
                        rs2 = m_IGTDataContext.OpenRecordset(sSqlTo, ADODB.CursorTypeEnum.adOpenStatic, ADODB.LockTypeEnum.adLockReadOnly, 1);

                        if (rs2.RecordCount > 0)
                        {
                            rs2.MoveFirst();
                            do
                            {
                                gFNO2 = rs2.Fields[1].Value.ToString();
                                gFID2 = rs2.Fields[0].Value.ToString();

                                string MIN_MAT = Get_Value("select MIN_MATERIAL from GC_NETELEM where G3E_FID = " + gFID2);
                                if (MIN_MAT == "")
                                {
                                    MessageBox.Show("No Plant Unit selected for the Device", "Core Count", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                    return;
                                }

                                gFNO2_Desc = Get_Value("Select g3e_username from g3e_feature where g3e_fno = " + gFNO2);

                                if (gFNO2 == FDC_FNO.ToString())
                                {
                                    gFID2_Desc = Get_Value("select FDC_CODE from GC_FDC where g3e_fid = " + gFID2);
                                }
                                else if (gFNO2 == UPE_FNO.ToString())
                                {
                                    gFID2_Desc = Get_Value("select UPE_CODE from GC_UPE where g3e_fid = " + gFID2);
                                }
                                else if (gFNO2 == FDP_FNO.ToString())
                                {
                                    gFID2_Desc = Get_Value("select FDP_CODE from GC_FDP where g3e_fid = " + gFID2);
                                }
                                else if (gFNO2 == FTB_FNO.ToString())
                                {
                                    gFID2_Desc = Get_Value("select FDP_CODE from GC_FTB where g3e_fid = " + gFID2);
                                }
                                else if (gFNO2 == TIE_FNO.ToString())
                                {
                                    gFID2_Desc = Get_Value("select FDP_CODE from GC_TIEFDP where g3e_fid = " + gFID2);
                                }
                                else if (gFNO2 == DB_FNO.ToString())
                                {
                                    gFID2_Desc = Get_Value("select DB_CODE from GC_DB where g3e_fid = " + gFID2);
                                }
                                else if (gFNO2 == MSAN_FNO.ToString())
                                {
                                    gFID2_Desc = Get_Value("select RT_CODE from GC_MSAN where g3e_fid = " + gFID2);
                                }
                                else if (gFNO2 == DDN_FNO.ToString())
                                {
                                    gFID2_Desc = Get_Value("select DDN_CODE from GC_DDN where g3e_fid = " + gFID2);
                                }
                                else if (gFNO2 == NDH_FNO.ToString())
                                {
                                    gFID2_Desc = Get_Value("select NDH_CODE from GC_NDH where g3e_fid = " + gFID2);
                                }
                                else if (gFNO2 == MUX_FNO.ToString())
                                {
                                    gFID2_Desc = Get_Value("select MUX_CODE from GC_MINIMUX where g3e_fid = " + gFID2);
                                }
                                else if (gFNO2 == EPE_FNO.ToString())
                                {
                                    gFID2_Desc = Get_Value("select EPE_CODE from GC_EPE where g3e_fid = " + gFID2);
                                }
                                else if (gFNO2 == FAN_FNO.ToString())
                                {
                                    gFID2_Desc = Get_Value("select FAN_CODE from GC_FAN where g3e_fid = " + gFID2);
                                }
                                else if (gFNO2 == RT_FNO.ToString())
                                {
                                    gFID2_Desc = Get_Value("select RT_CODE from GC_RT where g3e_fid = " + gFID2);
                                }
                                else if (gFNO2 == ODF_FNO.ToString())
                                {
                                    gFID2_Desc = Get_Value("select ODF_NUM from GC_ODF where g3e_fid = " + gFID2);
                                }
                                else if (gFNO2 == FPATCH_FNO.ToString())
                                {
                                    gFID2_Desc = Get_Value("select PATCH_CODE from GC_FPATCH where g3e_fid = " + gFID2);
                                }
                                else if (gFNO2 == FPP_FNO.ToString())
                                {
                                    gFID2_Desc = Get_Value("select PATCH_CODE from GC_FPATCHPANEL where g3e_fid = " + gFID2);
                                }
                                else if (gFNO2 == FSE_FNO.ToString())
                                {
                                    gFID2_Desc = Get_Value("select SPLICE_CODE from GC_FSPLICE where g3e_fid = " + gFID2);
                                }
                                else if (gFNO2 == SPLITTER_FNO.ToString())
                                {
                                    //gFID2_Desc = Get_Value("select SPLITTER_CODE from GC_FSPLITTER where g3e_fid = " + gFID2);
                                    //Requst from Mike on 11-Aug-2012
                                    gFID2_Desc = Get_Value("select NO_OF_SPLITTER from GC_FSPLITTER where g3e_fid = " + gFID2);
                                }
                                else if (gFNO2 == "7200")
                                {
                                    gFID2_Desc = Get_Value("select CABLE_CODE ||' (' || EFF_CORE || ')' from GC_fcbl where g3e_fid = " + gFID2);
                                }
                                else if (gFNO2 == "7400")
                                {
                                    gFID2_Desc = Get_Value("select FCABLE_CODE ||' (' || CORE_NUM || ')' from GC_FDCBL where g3e_fid = " + gFID2);
                                }
                                else if (gFNO2 == "4400")
                                {
                                    gFID2_Desc = Get_Value("select CABLE_CODE ||' (' || EFF_CORE || ')' from GC_FCBL_TRUNK where g3e_fid = " + gFID2);
                                }
                                else if (gFNO2 == "4500")
                                {
                                    gFID2_Desc = Get_Value("select CABLE_CODE ||' (' || EFF_CORE || ')' from GC_FCBL_JNT where g3e_fid = " + gFID2);
                                }
                                else
                                    gFID2_Desc = gFID2;

                                //Desc = gFNO2_Desc + " - " + gFID2_Desc;
                                Desc = gFID2_Desc + " - " + gFID2;
                                //cboFrom.Items.Add(new cboItem(Desc, gFID2));
                                cboTo.Items.Add(new cboItem(Desc, gFID2));

                                rs2.MoveNext();
                            }
                            while (!rs2.EOF);

                        }
                        else
                        {
                            MessageBox.Show("There are no related feature.", "Core Count", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        rs2.Close();
                    }
                }

            }
            catch (Exception ex)
            {
                log.WriteLog("");
                log.WriteLog(this.GetType().FullName, "ERROR: " + ex.Message);
                log.WriteLog(ex.StackTrace);
                MessageBox.Show(ex.Message, "Core Count", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
               

        //Add to GRID
        private void AddtoGrid(string gFNO1, string gFID1, string gFNO1D, string gFID1D, string gLOW1, string gHIGH1, string gFNO2, string gFID2, string gFNO2D, string gFID2D, string gLOW2, string gHIGH2, string gCID, string gG3E_ID, string ppwo, string ConnType, string EXC_ABB, string CABLE_CODE, string FEATURE_STATE, string TERM_FNO, string TERM_FID, string TERM_LOW, string TERM_HIGH, string COLOR)
        {
            try
            {
                int n = 0,i,j;

                /*
                DataGridViewCheckBoxColumn cbox = new DataGridViewCheckBoxColumn();
                cbox.Name = "chkbox";
                cbox.HeaderText = "Select";
                gv_Route.Columns.Add(cbox);
                gv_Route.AutoSize = true;
                gv_Route.AllowUserToAddRows = false;
                */                              
                
                for (i=0; i < gv_Route.RowCount; i++)
                {
                    gv_Route.Rows[i].Cells[2].Value = false; 
                }
                               
                n = gv_Route.Rows.Add();

                for (j = 3; j < gv_Route.ColumnCount ; j++)
                {
                    if (COLOR == "YES")                        
                        gv_Route.Rows[n].Cells[j].Style.BackColor = System.Drawing.Color.LemonChiffon;
                }
                                
                gv_Route.Rows[n].Cells[3].Value = gFNO1D;
                gv_Route.Rows[n].Cells[4].Value = gFID1D;
                gv_Route.Rows[n].Cells[5].Value = gLOW1;
                gv_Route.Rows[n].Cells[6].Value = gHIGH1;
                gv_Route.Rows[n].Cells[7].Value = gFNO2D;
                gv_Route.Rows[n].Cells[8].Value = gFID2D;
                gv_Route.Rows[n].Cells[9].Value = gLOW2;
                gv_Route.Rows[n].Cells[10].Value = gHIGH2;
                gv_Route.Rows[n].Cells[11].Value = gFNO1;
                gv_Route.Rows[n].Cells[12].Value = gFID1;
                gv_Route.Rows[n].Cells[13].Value = gFNO2;
                gv_Route.Rows[n].Cells[14].Value = gFID2;
                gv_Route.Rows[n].Cells[15].Value = gCID;
                gv_Route.Rows[n].Cells[16].Value = gG3E_ID;
                gv_Route.Rows[n].Cells[17].Value = ppwo;
                gv_Route.Rows[n].Cells[18].Value = ConnType;

                gv_Route.Rows[n].Cells[19].Value = EXC_ABB;
                gv_Route.Rows[n].Cells[20].Value = CABLE_CODE;
                gv_Route.Rows[n].Cells[21].Value = FEATURE_STATE;
                gv_Route.Rows[n].Cells[22].Value = TERM_FNO;
                gv_Route.Rows[n].Cells[23].Value = TERM_FID;
                gv_Route.Rows[n].Cells[24].Value = TERM_LOW;
                gv_Route.Rows[n].Cells[25].Value = TERM_HIGH;

                gv_Route.ClearSelection();

            }
            catch (Exception ex)
            {
                log.WriteLog("");
                log.WriteLog(this.GetType().FullName, "ERROR: " + ex.Message);
                log.WriteLog(ex.StackTrace);
                MessageBox.Show(ex.Message, "Core Count", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btn_Connect_Click(object sender, EventArgs e)
        {
            Connect("MAIN");
        }

        private void btn_Protection_Click(object sender, EventArgs e)
        {
            Connect("PROTECTION");
        }

        private void btn_Spare_Click(object sender, EventArgs e)
        {
            string sSql = "Select * from GC_SPLICE_CONNECT where G3E_FNO = " + lbl_fno.Text + " and G3E_FID = " + lbl_FID.Text + " and CORE_STATUS = 'SPARE'";
            ADODB.Recordset rs = m_IGTDataContext.OpenRecordset(sSql, ADODB.CursorTypeEnum.adOpenStatic, ADODB.LockTypeEnum.adLockReadOnly, 1);
            if (rs.RecordCount == 0)
            {
                Connect("SPARE");
                //if (lbl_fno.Text == "11800" || lbl_fno.Text == "5100" || lbl_fno.Text == "5200" || lbl_fno.Text == "5400" || lbl_fno.Text == "5500" || lbl_fno.Text == "5600" || lbl_fno.Text == "5800" || lbl_fno.Text == "5900" || lbl_fno.Text == "9100" || lbl_fno.Text == "9300" || lbl_fno.Text == "9400" || lbl_fno.Text == "9500" || lbl_fno.Text == "9600" || lbl_fno.Text == "9700" || lbl_fno.Text == "9800" || lbl_fno.Text == "9900")
                //if (lbl_fno.Text == "11800")
                //{
                //    DrawStump(Convert.ToInt16(lbl_fno.Text), Convert.ToInt32(lbl_FID.Text), "SPARE");
                //}
            }
            else
            {
                MessageBox.Show("SPARE Record already Exists for the Fiber Splice.", "Core Count", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
        }

        private void btn_Foms_Click(object sender, EventArgs e)
        {
            string sSql = "Select * from GC_SPLICE_CONNECT where G3E_FNO = " + lbl_fno.Text + " and G3E_FID = " + lbl_FID.Text + " and CORE_STATUS = 'FOMS'";
            ADODB.Recordset rs = m_IGTDataContext.OpenRecordset(sSql, ADODB.CursorTypeEnum.adOpenStatic, ADODB.LockTypeEnum.adLockReadOnly, 1);
            if (rs.RecordCount == 0)
            {
                Connect("FOMS");
                //DrawStump(Convert.ToInt16(lbl_fno.Text), Convert.ToInt32(lbl_FID.Text), "FOMS");
            }
            else
            {
                MessageBox.Show("FOMS/STUMP Record already Exists for the Fiber Splice.", "Core Count", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
        }

        private void btn_Stump_Click(object sender, EventArgs e)
        {
            string sSql = "Select * from GC_SPLICE_CONNECT where G3E_FNO = " + lbl_fno.Text + " and G3E_FID = " + lbl_FID.Text + " and CORE_STATUS = 'STUMP'";
            ADODB.Recordset rs = m_IGTDataContext.OpenRecordset(sSql, ADODB.CursorTypeEnum.adOpenStatic, ADODB.LockTypeEnum.adLockReadOnly, 1);
            if (rs.RecordCount == 0)
            {
                Connect("STUMP");
                //if (lbl_fno.Text == "11800" || lbl_fno.Text == "5100" || lbl_fno.Text == "5200" || lbl_fno.Text == "5400" || lbl_fno.Text == "5500" || lbl_fno.Text == "5600" || lbl_fno.Text == "5800" || lbl_fno.Text == "5900" || lbl_fno.Text == "9100" || lbl_fno.Text == "9300" || lbl_fno.Text == "9400" || lbl_fno.Text == "9500" || lbl_fno.Text == "9600" || lbl_fno.Text == "9700" || lbl_fno.Text == "9800" || lbl_fno.Text == "9900")
                //if (lbl_fno.Text == "11800")
                //{
                //    DrawStump(Convert.ToInt16(lbl_fno.Text), Convert.ToInt32(lbl_FID.Text), "STUMP");
                //}
            }
            else
            {
                MessageBox.Show("STUMP Record already Exists for the Fiber Splice.", "Core Count", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
        }

        private void btn_Assign_Click(object sender, EventArgs e)
        {
            Connect(""); 
        }

        private void btn_Swap_Click(object sender, EventArgs e)
        {
            try
            {
                int Splice_Cnt = 0;
                string G3E_ID = null;
                string CORE_STATUS = null;
                Recordset rsDel = null;
                string sSql = "";
                int iRecordNum = 0;
                object[] obj = null;

                for (int i = 0; i <= gv_Route.Rows.Count - 1; i++)
                {
                    if (Convert.ToBoolean(gv_Route.Rows[i].Cells[2].Value) == true)
                    {
                        G3E_ID = gv_Route.Rows[i].Cells[16].Value.ToString();
                        CORE_STATUS = gv_Route.Rows[i].Cells[18].Value.ToString();
                        Splice_Cnt++;
                    }
                }

                if (Splice_Cnt == 0)
                {
                    MessageBox.Show("Please Select a Row by Clicking the CheckBox to Swap MAIN/PROTECTION.", "Core Count", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                //else if (Splice_Cnt > 0)
                //{
                //    MessageBox.Show("Swap MAIN/PROTECTION can be done only for the first splice.", "Core Count", MessageBoxButtons.OK, MessageBoxIcon.Information);
                //    return;
                //}

                if (CORE_STATUS == "MAIN" || CORE_STATUS == "PROTECTION")
                {
                    DialogResult retVal = MessageBox.Show("Are you sure to Swap the selected Rows to MAIN/PROTECTION?", "SpliceConnect", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (retVal == DialogResult.Yes)
                    {
                        GTCustomCommandModeless.m_oGTTransactionManager.Begin("SpliceSwap");
                        if (CORE_STATUS == "MAIN")
                            sSql = "Update GC_SPLICE_CONNECT SET TERM_FID = " + lbl_CoreP_FID.Text + ", CORE_STATUS = 'PROTECTION' where g3e_id =" + G3E_ID + "";
                        else if (CORE_STATUS == "PROTECTION")
                            sSql = "Update GC_SPLICE_CONNECT SET TERM_FID = " + lbl_Core_FID.Text + ", CORE_STATUS = 'MAIN' where g3e_id =" + G3E_ID + "";
                        rsDel = m_IGTDataContext.Execute(sSql, out iRecordNum, (int)ADODB.CommandTypeEnum.adCmdText, obj);
                        GTCustomCommandModeless.m_oGTTransactionManager.Commit();
                        btn_Swap.Enabled = false;
                        lbl_MSG.Text = "Splice Swap Done.";
                        Select_Splice();
                    }
                }
            }
            catch (Exception ex)
            {
                //GTClassFactory.Create<IGTApplication>().EndWaitCursor();
                log.WriteLog("");
                log.WriteLog(this.GetType().FullName, "ERROR: " + ex.Message);
                log.WriteLog(ex.StackTrace);
                MessageBox.Show(ex.Message, "Core Count", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
               

        private void Connect(string CORE_STATUS)
        {
            try
            {
                if (cboFrom.Text.Trim() != "" && cboTo.Text.Trim() != "")
                {
                    if (GetComboValue(cboFrom, cboFrom.SelectedItem.ToString()) == GetComboValue(cboTo, cboTo.SelectedItem.ToString()))
                    {
                        MessageBox.Show("Loopback Connection Can't be established ", "Core Count", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }
                }
                if (cboFrom.Text.Trim() == "")
                {
                    MessageBox.Show("Please select the Cable", "Core Count", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                if (cboTo.Text.Trim() == "")
                {
                    MessageBox.Show("Please select the Device", "Core Count", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                if (txtLOW1.Text.Trim() == "")
                {
                    MessageBox.Show("Low Core Cannot be Empty.", "Core Count", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                if (txtHIGH1.Text.Trim() == "")
                {
                    MessageBox.Show("High Core Cannot be Empty.", "Core Count", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                if (Convert.ToInt32(txtLOW1.Text) > Convert.ToInt32(txtHIGH1.Text))
                {
                    MessageBox.Show("Low Core is Grater than High Core.", "Core Count", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                                
                if (txtLOW2.Text == "")
                {
                    MessageBox.Show("Low Port Cannot be Empty.", "Core Count", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                if (txtHIGH2.Text == "")
                {
                    MessageBox.Show("High Port Cannot be Empty.", "Core Count", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                if (Convert.ToInt32(txtLOW2.Text) > Convert.ToInt32(txtHIGH2.Text))
                {
                    MessageBox.Show("Low Port is Grater than High Port.", "Core Count", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                //if (Convert.ToInt32(txtHIGH1.Text) > Convert.ToInt32(label3.Text))
                //{
                //    MessageBox.Show("Low1 & High1 within this Range : 1 - "+ label3.Text +".", "Core Count", MessageBoxButtons.OK, MessageBoxIcon.Information);
                //    return;
                //}
                //if (Convert.ToInt32(txtHIGH2.Text) > Convert.ToInt32(label4.Text))
                //{
                //    MessageBox.Show("Low2 & High2 within this Range : 1 - "+ label4.Text + ".", "Core Count", MessageBoxButtons.OK, MessageBoxIcon.Information);
                //    return;
                //}

                int count = Convert.ToInt32(txtHIGH2.Text) - Convert.ToInt32(txtLOW2.Text) + 1;
                //if (count > Convert.ToInt32(label4.Text))
                //{
                //    MessageBox.Show("Pair's Limit Exceed...", "Core Count", MessageBoxButtons.OK, MessageBoxIcon.Information);
                //    return;
                //}
                
                //Cable
                
                int count1 = Convert.ToInt32(txtHIGH1.Text) - Convert.ToInt32(txtLOW1.Text) + 1;
                if (lbl_FromCore.Text != "")
                {
                    if (count1 > Convert.ToInt32(lbl_FromCore.Text))
                    {
                        MessageBox.Show("Effective Core (" + lbl_FromCore.Text + ") Exceeds the Pair Count...", "Core Count", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }
                }

                int count2 = Convert.ToInt32(txtHIGH2.Text) - Convert.ToInt32(txtLOW2.Text) + 1;
                if (lbl_ToCore.Text != "")
                {
                    if (count2 > Convert.ToInt32(lbl_ToCore.Text))
                    {
                        MessageBox.Show("Port/Strand (" + lbl_ToCore.Text + ") Exceeds the Pair Count...", "Core Count", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }
                }

                if (count != count1)
                {
                    MessageBox.Show("Core Count and Port Count Don't match. Pair's Count Exceed...", "Core Count", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                //if (btn_Connect.Text == "Connect")
                //{
                //For SPARE don't do Validation, Request from Mike on 26-Sep-2012
                if (CORE_STATUS != "SPARE")
                {
                    if (Validation_Device_Splice_1(lbl_fno.Text.ToString(), Convert.ToInt32(txtLOW1.Text), Convert.ToInt32(txtHIGH1.Text), cboFrom, cboTo) == false)
                    {
                        MessageBox.Show("Low Core or High Core Splice Connection Already Exists...", "Core Count", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }
                    else if (Validation_Device_Splice_1(lbl_fno.Text.ToString(), Convert.ToInt32(txtLOW2.Text), Convert.ToInt32(txtHIGH2.Text), cboTo, cboFrom) == false)
                    {
                        MessageBox.Show("Low Port or High Port Splice Connection Already Exists...", "Core Count", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;

                    }
                    if (validation_rack_splicedevice(lbl_fno.Text.ToString(), Convert.ToInt32(txtLOW1.Text), Convert.ToInt32(txtHIGH1.Text), cboFrom) == false)
                    {
                        MessageBox.Show("Low Core & High Core Splice Connection Already Exists...", "Core Count", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }
                    //Commented as Error for FDC D-Side later Fix it.
                    //else if (validation_rack_splicedevice(lbl_fno.Text.ToString(), Convert.ToInt32(txtLOW2.Text), Convert.ToInt32(txtHIGH2.Text), cboTo) == false)
                    //{
                    //    MessageBox.Show(" Low Port & High Port Splice Connection Already Exists...", "Core Count", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    //    return;
                    //}

                    if (Validation_Cable_Rack(Convert.ToInt32(txtLOW1.Text), Convert.ToInt32(txtHIGH1.Text), cboFrom, cboTo) == false)
                    {
                        MessageBox.Show(" Given Low Core & High Core Pair  Spliced", "Core Count", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }
                    else if (Validation_Cable_Rack(Convert.ToInt32(txtLOW2.Text), Convert.ToInt32(txtHIGH2.Text), cboTo, cboFrom) == false)
                    {
                        MessageBox.Show("Given Low Port & High Port Pair  Spliced", "Core Count", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }
                }

                    //if (lbl_fno.Text == "5500")
                    //{
                    //    if (cboPP.Text == "")
                    //    {
                    //        MessageBox.Show("Please Select a Patch Panel", "Core Count", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    //        return;
                    //    }
                    //}

                    //GTClassFactory.Create<IGTApplication>().BeginWaitCursor();

                    IGTKeyObject cblSFeat = null;
                    IGTKeyObject cblDFeat = null;
                    IGTKeyObject devFeat = null;
                    string cblFID = null;

                    cblFID = Get_Value("select g3e_fno from gc_netelem where g3e_fid = " + GetComboValue(cboFrom, cboFrom.SelectedItem.ToString()));
                    cblSFeat = GTClassFactory.Create<IGTApplication>().DataContext.OpenFeature(short.Parse(cblFID), int.Parse(GetComboValue(cboFrom, cboFrom.SelectedItem.ToString())));

                    cblFID = Get_Value("select g3e_fno from gc_netelem where g3e_fid = " + GetComboValue(cboTo, cboTo.SelectedItem.ToString()));
                    cblDFeat = GTClassFactory.Create<IGTApplication>().DataContext.OpenFeature(short.Parse(cblFID), int.Parse(GetComboValue(cboTo, cboTo.SelectedItem.ToString())));

                    cblFID = Get_Value("select g3e_fno from gc_netelem where g3e_fid = " + lbl_FID.Text);
                    devFeat = GTClassFactory.Create<IGTApplication>().DataContext.OpenFeature(short.Parse(cblFID), int.Parse(lbl_FID.Text));

                    int port = 0;
                    
                    //For FDC D-Side
                    string CFNO = Get_Value("Select g3e_fno from gc_netelem where g3e_fid = " +  GetComboValue(cboFrom, cboFrom.SelectedItem.ToString()));
                    
                    if (lbl_fno.Text == "5100" && CFNO == "7400")
                        FiberConnect(cblDFeat, devFeat, cblSFeat, port + int.Parse(txtLOW2.Text), port + int.Parse(txtHIGH2.Text), int.Parse(txtLOW1.Text), int.Parse(txtHIGH1.Text), CORE_STATUS);
                    else
                        FiberConnect(cblSFeat, devFeat, cblDFeat, port + int.Parse(txtLOW1.Text), port + int.Parse(txtHIGH1.Text), int.Parse(txtLOW2.Text), int.Parse(txtHIGH2.Text), CORE_STATUS);

                    All_Clear();
                    
                    Select_Splice();
                    //Get_Port_Size(GetComboValue(cboFrom, cboFrom.SelectedItem.ToString()));
                    
                    //Display the Available Core
                    Get_Available_Core(lbl_FID.Text, lbl_fno.Text);

                    //GTClassFactory.Create<IGTApplication>().EndWaitCursor();
                    lbl_MSG.Text = "Splice Connection Done.";
                    //MessageBox.Show("Splice Connection Done.", "SpliceConnect", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    GTClassFactory.Create<IGTApplication>().SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Core Count.");

                //}
                //else
                //{

                //    {
                //        Recordset rsDel = null;
                //        string sSql = "";
                //        int iRecordNum = 0;
                //        object[] obj = null;

                //        GTCustomCommandModeless.m_oGTTransactionManager.Begin("SpliceConnect");

                //        sSql = "Update GC_SPLICE_CONNECT SET LOW1=" + int.Parse(txtLOW1.Text) + ", HIGH1 =" + int.Parse(txtHIGH1.Text) + ", LOW2 =" + int.Parse(txtLOW2.Text) + ", HIGH2=" + int.Parse(txtHIGH2.Text) + "  where g3e_id =" + edit_g3e_id + "";
                //        rsDel = m_IGTDataContext.Execute(sSql, out iRecordNum, (int)ADODB.CommandTypeEnum.adCmdText, obj);

                //        GTCustomCommandModeless.m_oGTTransactionManager.Commit();

                //        MessageBox.Show("Splice Connection Updated.", "SpliceConnect", MessageBoxButtons.OK, MessageBoxIcon.Information);
                //        GTClassFactory.Create<IGTApplication>().SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Core Count.");

                //        All_Clear();

                //        Select_Splice();

                //        btn_Connect.Text = "Connect";
                //    }

                //}

            }
            catch (Exception ex)
            {
                //GTClassFactory.Create<IGTApplication>().EndWaitCursor();
                log.WriteLog("");
                log.WriteLog(this.GetType().FullName, "ERROR: " + ex.Message);
                log.WriteLog(ex.StackTrace);
                MessageBox.Show(ex.Message, "Core Count", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        ////Draw Stump Symbol
        //public void DrawStump(short iFNO, int iFID, string CORE_TYPE)
        //{
        //    try
        //    {         
        //        IGTKeyObject oExtFeature;
        //        IGTTextPointGeometry oTextGeom;
        //        IGTPointGeometry oPointGeom;
        //        IGTPoint oPoint;
        //        short G_CNO = 0;    
        //        short L_CNO = 0;    
        //        short S_CNO = 0;    
        //        int lastSEQ = 0;
        //        string DFID = null;
        //        string DLOW2 = null;
        //        string DHIGH2 = null;
        //        string EXCH = null;
        //        string CABLE_CODE = null;
        //        string CORE = null;
        //        string sSql = null;
        //        int recordsAffected = 0;

        //        switch (iFNO)
        //        {
        //            case 11800: //Stump
        //                G_CNO = 11822;    
        //                L_CNO = 11834;    
        //                S_CNO = 11820;    
        //                break;
        //            case 5100: //Fiber Distribution Cabinet 
        //                G_CNO = 5122;
        //                L_CNO = 5134;
        //                S_CNO = 5120;
        //                break;
        //            case 5200: //Edge Provider Edge
        //                G_CNO = 5222;
        //                L_CNO = 5234;
        //                S_CNO = 5220;
        //                break;
        //            case 5400: //User Provider Edge
        //                G_CNO = 5422;
        //                L_CNO = 5434;
        //                S_CNO = 5420;
        //                break;
        //            case 5500: //Optical Distribution Frame
        //                G_CNO = 5522;
        //                L_CNO = 5534;
        //                S_CNO = 5520;
        //                break;
        //            case 5600: //Fiber Distribution Point
        //                G_CNO = 5622;
        //                L_CNO = 5634;
        //                S_CNO = 5620;
        //                break;
        //            case 5800: //Tie FDP
        //                G_CNO = 5822;
        //                L_CNO = 5834;
        //                S_CNO = 5820;
        //                break;
        //            case 5900: //FTB
        //                G_CNO = 5922;
        //                L_CNO = 5934;
        //                S_CNO = 5920;
        //                break;
        //            case 9100: //MSAN
        //                G_CNO = 9122;
        //                L_CNO = 9134;
        //                S_CNO = 9120;
        //                break;
        //            case 9300: //DDN
        //                G_CNO = 9322;
        //                L_CNO = 9334;
        //                S_CNO = 9320;
        //                break;
        //            case 9400: //NDH
        //                G_CNO = 9422;
        //                L_CNO = 9434;
        //                S_CNO = 9420;
        //                break;
        //            case 9500: //MINMUX
        //                G_CNO = 9522;
        //                L_CNO = 9534;
        //                S_CNO = 9520;
        //                break;
        //            case 9600: //Remote Terminal
        //                G_CNO = 9622;
        //                L_CNO = 9634;
        //                S_CNO = 9620;
        //                break;
        //            case 9700: //Fiber Access Node
        //                G_CNO = 9722;
        //                L_CNO = 9734;
        //                S_CNO = 9720;
        //                break;
        //            case 9800: //VDSL2
        //                G_CNO = 9822;
        //                L_CNO = 9834;
        //                S_CNO = 9820;
        //                break;
        //            case 9900: //DB
        //                G_CNO = 9922;
        //                L_CNO = 9934;
        //                S_CNO = 9920;
        //                break;
        //        }

        //        oPoint = GTClassFactory.Create<IGTPoint>();

        //        GTCustomCommandModeless.m_oGTTransactionManager.Begin("DrawStump");
        //        oExtFeature = GTClassFactory.Create<IGTApplication>().DataContext.OpenFeature(iFNO, iFID);

        //        while (!oExtFeature.Components.GetComponent(G_CNO).Recordset.EOF)
        //        {
        //            if (lastSEQ < Convert.ToInt32(oExtFeature.Components.GetComponent(G_CNO).Recordset.Fields["G3E_CID"].Value))
        //                lastSEQ = Convert.ToInt32(oExtFeature.Components.GetComponent(G_CNO).Recordset.Fields["G3E_CID"].Value);
        //            oExtFeature.Components.GetComponent(G_CNO).Recordset.MoveNext();
        //        }
        //        lastSEQ++;
                
        //        //Stump Symbol
        //        oPointGeom = GTClassFactory.Create<IGTPointGeometry>();
        //        oPoint.X = oExtFeature.Components.GetComponent(S_CNO).Geometry.FirstPoint.X;
        //        oPoint.Y = oExtFeature.Components.GetComponent(S_CNO).Geometry.FirstPoint.Y + 1;
        //        oPoint.Z = oExtFeature.Components.GetComponent(S_CNO).Geometry.FirstPoint.Z;
        //        oPointGeom.Origin = oPoint;

        //        //oExtFeature.Components.GetComponent(G_CNO).Recordset.Filter = "CORE_TYPE = '" + CORE_TYPE + "'";
        //        if (oExtFeature.Components.GetComponent(G_CNO).Recordset.EOF)
        //        {
        //            oExtFeature.Components.GetComponent(G_CNO).Recordset.AddNew("G3E_FID", iFID);
        //            oExtFeature.Components.GetComponent(G_CNO).Recordset.Update("G3E_FNO", iFNO);
        //            oExtFeature.Components.GetComponent(G_CNO).Recordset.Update("G3E_CID", lastSEQ);
        //            oExtFeature.Components.GetComponent(G_CNO).Recordset.Update("CORE_TYPE", CORE_TYPE);                                   

        //            oExtFeature.Components.GetComponent(G_CNO).Geometry = oPointGeom;
        //        }

        //        //Stump Text
        //        oTextGeom = GTClassFactory.Create<IGTTextPointGeometry>();
        //        oPoint.X = oExtFeature.Components.GetComponent(S_CNO).Geometry.FirstPoint.X;
        //        if (lastSEQ == 1)
        //            oPoint.Y = oExtFeature.Components.GetComponent(S_CNO).Geometry.FirstPoint.Y - 1;
        //        else
        //            oPoint.Y = oExtFeature.Components.GetComponent(S_CNO).Geometry.FirstPoint.Y - 1.6;
        //        oPoint.Z = oExtFeature.Components.GetComponent(S_CNO).Geometry.FirstPoint.Z;
        //        oTextGeom.Origin = oPoint;
        //        oTextGeom.Rotation = 0;

        //        try
        //        {
        //            sSql = "select G3E_FID, LOW2, HIGH2, EXC_ABB from GC_SPLICE_CONNECT where CORE_STATUS = '" + CORE_TYPE + "' and G3E_FID = " + iFID;
        //            Recordset rsC = GTClassFactory.Create<IGTApplication>().DataContext.Execute(sSql, out recordsAffected, (int)CommandTypeEnum.adCmdText, null);
        //            if (rsC.RecordCount > 0)
        //            {
        //                rsC.MoveFirst();
        //                DFID = rsC.Fields["G3E_FID"].Value.ToString();
        //                DLOW2 = rsC.Fields["LOW2"].Value.ToString();
        //                DHIGH2 = rsC.Fields["HIGH2"].Value.ToString();
        //                EXCH = rsC.Fields["EXC_ABB"].Value.ToString();
        //            }

        //            sSql = "Select A.FNO2, A.FID2, A.LOW2, A.HIGH2 from table (select AG_GET_CORECOUNT (11800, " + DFID + ", " + DLOW2 + ", " + DHIGH2 + ") from dual ) A";
        //            Recordset rs = GTClassFactory.Create<IGTApplication>().DataContext.Execute(sSql, out recordsAffected, (int)CommandTypeEnum.adCmdText, null);
        //            if (rs.RecordCount > 0)
        //            {
        //                rs.MoveFirst();
                        
        //                if (rs.Fields["FNO2"].Value.ToString() == "7200")
        //                    CABLE_CODE = Get_Value("select CABLE_CODE from GC_fcbl where G3E_FID = " + rs.Fields["FID2"].Value.ToString());
        //                else if (rs.Fields["FNO2"].Value.ToString() == "7400")
        //                    CABLE_CODE = Get_Value("select FCABLE_CODE from GC_FDCBL where G3E_FID = " + rs.Fields["FID2"].Value.ToString());
        //                else if (rs.Fields["FNO2"].Value.ToString() == "4400")
        //                    CABLE_CODE = Get_Value("select CABLE_CODE from GC_FCBL_TRUNK where G3E_FID = " + rs.Fields["FID2"].Value.ToString());
        //                else if (rs.Fields["FNO2"].Value.ToString() == "4500")
        //                    CABLE_CODE = Get_Value("select CABLE_CODE from GC_FCBL_JNT where G3E_FID = " + rs.Fields["FID2"].Value.ToString());

        //                CORE = rs.Fields["LOW2"].Value.ToString() + "-" + rs.Fields["HIGH2"].Value.ToString();
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            MessageBox.Show(ex.Message + "\n" + sSql, "AG_GET_CORECOUNT", MessageBoxButtons.OK, MessageBoxIcon.Error);
        //        }

        //        //if (oExtFeature.Components.GetComponent(L_CNO).Recordset.EOF)
        //        //{
        //            oExtFeature.Components.GetComponent(L_CNO).Recordset.AddNew("G3E_FID", iFID);
        //            oExtFeature.Components.GetComponent(L_CNO).Recordset.Update("G3E_FNO", iFNO);
        //            oExtFeature.Components.GetComponent(L_CNO).Recordset.Update("G3E_CID", lastSEQ);
        //            oExtFeature.Components.GetComponent(L_CNO).Recordset.Update("CORE_TEXT", CORE_TYPE + " : " + EXCH + " " + CABLE_CODE + " " + CORE);

        //            oExtFeature.Components.GetComponent(L_CNO).Geometry = oTextGeom;
        //        //}
        //        //else
        //        //{
        //        //    oExtFeature.Components.GetComponent(L_CNO).Recordset.Update("CORE_TEXT", CORE_TYPE + " : " + EXCH + " " + CABLE_CODE + " " + CORE);
        //        //    oExtFeature.Components.GetComponent(L_CNO).Geometry = oTextGeom;
        //        //}

        //        GTCustomCommandModeless.m_oGTTransactionManager.Commit();                
        //        GTCustomCommandModeless.m_oGTTransactionManager.RefreshDatabaseChanges();               

        //    }
        //    catch (Exception ex)
        //    {
        //        log.WriteLog("");
        //        log.WriteLog(this.GetType().FullName, "ERROR: " + ex.Message);
        //        log.WriteLog(ex.StackTrace);
        //        GTCustomCommandModeless.m_oGTTransactionManager.Rollback();
        //        MessageBox.Show(ex.Message, "Core Count", MessageBoxButtons.OK, MessageBoxIcon.Error);
        //    }
        //}

        //Draw Stump Symbol


        public void DrawStump(short iFNO, int iFID, string CORE_TYPE)
        {
            try
            {
                IGTKeyObject oExtFeature;
                IGTTextPointGeometry oTextGeom;
                IGTPointGeometry oPointGeom;
                IGTPoint oPoint;
                short G_CNO = 11822;    //Stump Symbol
                short L_CNO = 11836;    //Stump Text
                short S_CNO = 11820;    //Splice Symbol
                int lastSEQ = 0;
                string DFID = null;
                string DLOW2 = null;
                string DHIGH2 = null;
                string EXCH = null;
                string CABLE_CODE = null;
                string CORE = null;
                string sSql = null;
                int recordsAffected = 0;

                oPoint = GTClassFactory.Create<IGTPoint>();

                GTCustomCommandModeless.m_oGTTransactionManager.Begin("DrawStump");
                oExtFeature = GTClassFactory.Create<IGTApplication>().DataContext.OpenFeature(iFNO, iFID);

                while (!oExtFeature.Components.GetComponent(G_CNO).Recordset.EOF)
                {
                    if (lastSEQ < Convert.ToInt32(oExtFeature.Components.GetComponent(G_CNO).Recordset.Fields["G3E_CID"].Value))
                        lastSEQ = Convert.ToInt32(oExtFeature.Components.GetComponent(G_CNO).Recordset.Fields["G3E_CID"].Value);
                    oExtFeature.Components.GetComponent(G_CNO).Recordset.MoveNext();
                }
                lastSEQ++;

                //Stump Symbol
                oPointGeom = GTClassFactory.Create<IGTPointGeometry>();
                oPoint.X = oExtFeature.Components.GetComponent(S_CNO).Geometry.FirstPoint.X;
                oPoint.Y = oExtFeature.Components.GetComponent(S_CNO).Geometry.FirstPoint.Y + 1;
                oPoint.Z = oExtFeature.Components.GetComponent(S_CNO).Geometry.FirstPoint.Z;
                oPointGeom.Origin = oPoint;

                oExtFeature.Components.GetComponent(G_CNO).Recordset.Filter = "CORE_TYPE = '" + CORE_TYPE + "'";
                if (oExtFeature.Components.GetComponent(G_CNO).Recordset.EOF)
                {
                    oExtFeature.Components.GetComponent(G_CNO).Recordset.AddNew("G3E_FID", iFID);
                    oExtFeature.Components.GetComponent(G_CNO).Recordset.Update("G3E_FNO", iFNO);
                    oExtFeature.Components.GetComponent(G_CNO).Recordset.Update("G3E_CID", lastSEQ);
                    oExtFeature.Components.GetComponent(G_CNO).Recordset.Update("CORE_TYPE", CORE_TYPE);

                    oExtFeature.Components.GetComponent(G_CNO).Geometry = oPointGeom;
                }

                //Stump Text
                oTextGeom = GTClassFactory.Create<IGTTextPointGeometry>();
                oPoint.X = oExtFeature.Components.GetComponent(S_CNO).Geometry.FirstPoint.X;
                if (lastSEQ == 1)
                    oPoint.Y = oExtFeature.Components.GetComponent(S_CNO).Geometry.FirstPoint.Y - 1;
                else
                    oPoint.Y = oExtFeature.Components.GetComponent(S_CNO).Geometry.FirstPoint.Y - 1.6;
                oPoint.Z = oExtFeature.Components.GetComponent(S_CNO).Geometry.FirstPoint.Z;
                oTextGeom.Origin = oPoint;
                oTextGeom.Rotation = 0;

                try
                {
                    sSql = "select G3E_FID, LOW2, HIGH2, EXC_ABB from GC_SPLICE_CONNECT where CORE_STATUS = '" + CORE_TYPE + "' and G3E_FID = " + iFID;
                    Recordset rsC = GTClassFactory.Create<IGTApplication>().DataContext.Execute(sSql, out recordsAffected, (int)CommandTypeEnum.adCmdText, null);
                    if (rsC.RecordCount > 0)
                    {
                        rsC.MoveFirst();
                        DFID = rsC.Fields["G3E_FID"].Value.ToString();
                        DLOW2 = rsC.Fields["LOW2"].Value.ToString();
                        DHIGH2 = rsC.Fields["HIGH2"].Value.ToString();
                        EXCH = rsC.Fields["EXC_ABB"].Value.ToString();
                    }

                    sSql = "Select A.FNO2, A.FID2, A.LOW2, A.HIGH2 from table (select AG_GET_CORECOUNT (11800, " + DFID + ", " + DLOW2 + ", " + DHIGH2 + ") from dual ) A";
                    Recordset rs = GTClassFactory.Create<IGTApplication>().DataContext.Execute(sSql, out recordsAffected, (int)CommandTypeEnum.adCmdText, null);
                    if (rs.RecordCount > 0)
                    {
                        rs.MoveFirst();

                        if (rs.Fields["FNO2"].Value.ToString() == "7200")
                            CABLE_CODE = Get_Value("select CABLE_CODE from GC_fcbl where G3E_FID = " + rs.Fields["FID2"].Value.ToString());
                        else if (rs.Fields["FNO2"].Value.ToString() == "7400")
                            CABLE_CODE = Get_Value("select FCABLE_CODE from GC_FDCBL where G3E_FID = " + rs.Fields["FID2"].Value.ToString());
                        else if (rs.Fields["FNO2"].Value.ToString() == "4400")
                            CABLE_CODE = Get_Value("select CABLE_CODE from GC_FCBL_TRUNK where G3E_FID = " + rs.Fields["FID2"].Value.ToString());
                        else if (rs.Fields["FNO2"].Value.ToString() == "4500")
                            CABLE_CODE = Get_Value("select CABLE_CODE from GC_FCBL_JNT where G3E_FID = " + rs.Fields["FID2"].Value.ToString());

                        CORE = rs.Fields["LOW2"].Value.ToString() + "-" + rs.Fields["HIGH2"].Value.ToString();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message + "\n" + sSql, "AG_GET_CORECOUNT", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                //if (oExtFeature.Components.GetComponent(L_CNO).Recordset.EOF)
                //{
                oExtFeature.Components.GetComponent(L_CNO).Recordset.AddNew("G3E_FID", iFID);
                oExtFeature.Components.GetComponent(L_CNO).Recordset.Update("G3E_FNO", iFNO);
                oExtFeature.Components.GetComponent(L_CNO).Recordset.Update("G3E_CID", lastSEQ);
                oExtFeature.Components.GetComponent(L_CNO).Recordset.Update("CORE_TEXT", CORE_TYPE + " : " + EXCH + " " + CABLE_CODE + " " + CORE);

                oExtFeature.Components.GetComponent(L_CNO).Geometry = oTextGeom;
                //}
                //else
                //{
                //    oExtFeature.Components.GetComponent(L_CNO).Recordset.Update("CORE_TEXT", CORE_TYPE + " : " + EXCH + " " + CABLE_CODE + " " + CORE);
                //    oExtFeature.Components.GetComponent(L_CNO).Geometry = oTextGeom;
                //}

                GTCustomCommandModeless.m_oGTTransactionManager.Commit();
                GTCustomCommandModeless.m_oGTTransactionManager.RefreshDatabaseChanges();

            }
            catch (Exception ex)
            {
                log.WriteLog("");
                log.WriteLog(this.GetType().FullName, "ERROR: " + ex.Message);
                log.WriteLog(ex.StackTrace);
                GTCustomCommandModeless.m_oGTTransactionManager.Rollback();
                MessageBox.Show(ex.Message, "Core Count", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void All_Clear()
        {
            txtHIGH1.Text = "";
            txtHIGH2.Text = "";
            txtLOW1.Text = "";
            txtLOW2.Text = "";          
            lbl_MSG.Text = "";          
           
        }


        public void All_Clear_Clear()
        {
            txtHIGH1.Text = "";
            txtHIGH2.Text = "";
            txtLOW1.Text = "";
            txtLOW2.Text = "";
            cboFrom.SelectedIndex = -1;
            cboTo.SelectedIndex = -1;           
            lbl_FromCore.Text = "";
            lbl_ToCore.Text = "";
            lbl_MSG.Text = "";


        }
        private bool validation_rack_splicedevice(string fno, int low1, int high1, ComboBox FID1)
        {
            //if (fno == "15700" || fno == "11800")
            //{
                ADODB.Recordset rs1 = new ADODB.Recordset();
                string sSql = "";

                sSql = "SELECT * FROM gc_splice_connect WHERE g3e_fid=" + lbl_FID.Text.ToString() + " AND  fid1= " + GetComboValue(FID1 , FID1.SelectedItem.ToString()) + "  AND ((low1 <=" + low1 + " AND High1 >=" + high1 + " ) OR ( low1 >=" + low1 + "  And high1 <=" + high1 + ")) ORDER BY FID1";
                rs1 = m_IGTDataContext.OpenRecordset(sSql, ADODB.CursorTypeEnum.adOpenStatic, ADODB.LockTypeEnum.adLockReadOnly, 1);
                if (rs1.RecordCount > 0)
                {
                    return false;
                }
                else
                {

                    ADODB.Recordset rs2 = new ADODB.Recordset();
                    rs1.Close();
                    string sSql2 = "";

                    sSql2 = "SELECT * FROM gc_splice_connect WHERE g3e_fid=" + lbl_FID.Text.ToString() + " AND  fid2= " + GetComboValue(FID1, FID1.SelectedItem.ToString()) + "  AND ((low2 <=" + low1 + " AND High2 >=" + high1 + " ) OR ( low2 >=" + low1 + "  And high2 <=" + high1 + ")) ORDER BY FID1";
                    rs2= m_IGTDataContext.OpenRecordset(sSql2, ADODB.CursorTypeEnum.adOpenStatic, ADODB.LockTypeEnum.adLockReadOnly, 1);
                    if (rs2.RecordCount > 0)
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                    rs2.Close();
                    
                }
                rs1.Close();

            //}
            //else
            //    return true;

            

        }

        private void Highlight(short mFNO, int mFID)
        {
            try
            {
                GTClassFactory.Create<IGTApplication>().SelectedObjects.Clear();
                oGTKeyObjs = (IGTDDCKeyObjects)GTClassFactory.Create<IGTApplication>().DataContext.GetDDCKeyObjects(mFNO, mFID, GTComponentGeometryConstants.gtddcgAllGeographic);
                for (int K = 0; K < oGTKeyObjs.Count; K++)
                    GTClassFactory.Create<IGTApplication>().SelectedObjects.Add(GTSelectModeConstants.gtsosmAllDisplayedComponentsInActiveLegend, oGTKeyObjs[K]);
            

                //if (opt_FitZoom.Checked == true)
                //{
                //GTClassFactory.Create<IGTApplication>().ActiveMapWindow.CenterSelectedObjects();
                //GTClassFactory.Create<IGTApplication>().ActiveMapWindow.DisplayScale = 300;
                //}
                GTClassFactory.Create<IGTApplication>().RefreshWindows();
                oGTKeyObjs = null;
               
            }
            catch (Exception ex)
            {
                log.WriteLog("");
                log.WriteLog(this.GetType().FullName, "ERROR: " + ex.Message);
                log.WriteLog(ex.StackTrace);
                //MessageBox.Show(ex.Message, "Core Count", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool Validation_Device_Splice_1(string fno, int low1, int high1, ComboBox FID1, ComboBox FromTo)
        {
            //if (fno == "11900" || fno == "12400" || fno == "12100")
            string cmbFNO = Get_Value("select g3e_fno  from gc_netelem where g3e_fid = " + GetComboValue(FID1, FID1.SelectedItem.ToString()));
            string cmbFromTo = Get_Value("select g3e_fno  from gc_netelem where g3e_fid = " + GetComboValue(FromTo, FromTo.SelectedItem.ToString()));

            if (cmbFNO != "12300") 
            {
                ADODB.Recordset rs1 = new ADODB.Recordset();
                string sSql = "";

                sSql = "SELECT * FROM gc_splice_connect WHERE g3e_fid=" + lbl_FID.Text.ToString() + " AND  FID1 = " + GetComboValue(FID1, FID1.SelectedItem.ToString()) + " AND ((low1 <=" + low1 + " and High1 >=" + high1 + " ) OR ( low1 >=" + low1 + "  and high1 <=" + high1 + ")) ORDER BY FID1";
                rs1 = m_IGTDataContext.OpenRecordset(sSql, ADODB.CursorTypeEnum.adOpenStatic, ADODB.LockTypeEnum.adLockReadOnly, 1);
                if (rs1.RecordCount > 0)
                {
                    return false;
                }
                else
                {
                    ADODB.Recordset rs2 = new ADODB.Recordset();
                    string sSql2 = "";

                    sSql2 = "SELECT * FROM gc_splice_connect WHERE g3e_fid = " + GetComboValue(FID1, FID1.SelectedItem.ToString()) + " AND FID2 = " + GetComboValue(FID1, FID1.SelectedItem.ToString()) + " AND ((low2 <=" + low1 + " and High2 >=" + high1 + " ) OR ( low2 >=" + low1 + "  and high2 <=" + high1 + ")) ORDER BY FID1";
                    rs2 = m_IGTDataContext.OpenRecordset(sSql2, ADODB.CursorTypeEnum.adOpenStatic, ADODB.LockTypeEnum.adLockReadOnly, 1);
                    if (rs2.RecordCount > 0)
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                    rs2.Close();
                }
                rs1.Close();
            }
            else
            {
                ADODB.Recordset rs1 = new ADODB.Recordset();
                string sSql = "";

                sSql = "SELECT * FROM gc_splice_connect WHERE g3e_fid=" + lbl_FID.Text.ToString() + " and FID2 = " +  cmbFromTo + " AND  FID1 = " + GetComboValue(FID1, FID1.SelectedItem.ToString()) + " AND ((low1 <=" + low1 + " and High1 >=" + high1 + " ) OR ( low1 >=" + low1 + "  and high1 <=" + high1 + ")) ORDER BY FID1";
                rs1 = m_IGTDataContext.OpenRecordset(sSql, ADODB.CursorTypeEnum.adOpenStatic, ADODB.LockTypeEnum.adLockReadOnly, 1);
                if (rs1.RecordCount > 0)
                {
                    return false;
                }
                else
                {
                    ADODB.Recordset rs2 = new ADODB.Recordset();
                    string sSql2 = "";

                    sSql2 = "SELECT * FROM gc_splice_connect WHERE g3e_fid = " + GetComboValue(FID1, FID1.SelectedItem.ToString()) + " and FID1 = " + cmbFromTo + "  AND FID2 = " + GetComboValue(FID1, FID1.SelectedItem.ToString()) + " AND ((low2 <=" + low1 + " and High2 >=" + high1 + " ) OR ( low2 >=" + low1 + "  and high2 <=" + high1 + ")) ORDER BY FID1";
                    rs2 = m_IGTDataContext.OpenRecordset(sSql2, ADODB.CursorTypeEnum.adOpenStatic, ADODB.LockTypeEnum.adLockReadOnly, 1);
                    if (rs2.RecordCount > 0)
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                    rs2.Close();
                }
                rs1.Close();
            }

        }
                
        private bool Validation_Device_Splice_New(string fno,int low1,int high1,ComboBox FID1)
        {
            //if (fno == "11900" || fno == "12400" || fno == "12100")
            //{
                
                ADODB.Recordset rs1 = new ADODB.Recordset();
                string sSql = "";

                sSql = "SELECT * FROM gc_splice_connect WHERE g3e_fid = " + GetComboValue(FID1 , FID1.SelectedItem.ToString()) + " AND FID1 = " + GetComboValue(FID1, FID1.SelectedItem.ToString()) + " AND ((low1 <=" + low1  + " AND High1 >=" + high1  + " ) OR ( low1 >=" + low1 + "  And high1 <=" + high1 + ")) ORDER BY FID1";
                rs1 = m_IGTDataContext.OpenRecordset(sSql, ADODB.CursorTypeEnum.adOpenStatic, ADODB.LockTypeEnum.adLockReadOnly, 1);
                if (rs1.RecordCount > 0)
                {
                    return false;
                }
                else
                {
                    ADODB.Recordset rs2 = new ADODB.Recordset();
                    string sSql2 = "";

                    sSql2 = "SELECT * FROM gc_splice_connect WHERE g3e_fid = " + GetComboValue(FID1 , FID1.SelectedItem.ToString()) + " AND FID2 = " + GetComboValue(FID1, FID1.SelectedItem.ToString()) + " AND ((low2 <=" + low1  + " AND High2 >=" + high1 + " ) OR ( low2 >=" + low1 + "  And high2 <=" + low1 + ")) ORDER BY FID1";
                    rs2 = m_IGTDataContext.OpenRecordset(sSql2, ADODB.CursorTypeEnum.adOpenStatic, ADODB.LockTypeEnum.adLockReadOnly, 1);
                    if (rs2.RecordCount > 0)
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                    rs2.Close();
                }
                rs1.Close();

            //}
            //else
            //    return true;           

        }



        private bool Validation_Cable_Rack(int low1,int high1, ComboBox FID1, ComboBox FID2)
        {
            ADODB.Recordset rs1 = new ADODB.Recordset();
            string sSql = "";

            //int low1 = Convert.ToInt32(txtLOW2.Text);
            //int high1 = Convert.ToInt32(txtHIGH2.Text);

            sSql = "SELECT * FROM gc_splice_connect WHERE g3e_fid = " + lbl_FID.Text.ToString() + " AND FID1 = " + GetComboValue(FID1, FID1.SelectedItem.ToString()) + " AND FID2 = " + GetComboValue(FID2 , FID2.SelectedItem.ToString()) + " AND ((low1 <=" + low1 + " AND High1 >=" + high1 + " ) OR ( low1 >=" + low1 + "  And high1 <=" + high1 + ")) ORDER BY FID1";
            rs1 = m_IGTDataContext.OpenRecordset(sSql, ADODB.CursorTypeEnum.adOpenStatic, ADODB.LockTypeEnum.adLockReadOnly, 1);
            if (rs1.RecordCount > 0)
            {
                return false;
            }
            else
            {
                ADODB.Recordset rs2 = new ADODB.Recordset();
                string sSql1 = "";

               // int low1 = Convert.ToInt32(txtLOW2.Text);
               // int high1 = Convert.ToInt32(txtHIGH2.Text);

                sSql1 = "SELECT * FROM gc_splice_connect WHERE g3e_fid = " + lbl_FID.Text .ToString() + " AND FID2 = " + GetComboValue(FID1 , FID1 .SelectedItem.ToString()) + " AND FID1 = " + GetComboValue(FID2 , FID2 .SelectedItem.ToString()) + " AND ((low2 <=" + low1 + " AND High2 >=" + high1 + " ) OR ( low2 >=" + low1 + "  And high2 <=" + high1 + ")) ORDER BY FID1";
                rs2 = m_IGTDataContext.OpenRecordset(sSql1, ADODB.CursorTypeEnum.adOpenStatic, ADODB.LockTypeEnum.adLockReadOnly, 1);
                if (rs2.RecordCount > 0)
                {
                    return false;
                }
                else
                {
                    return true;
                }
                rs2.Close();
                
            }
            rs1.Close();
                 
        }        


        // Fibre Connection
        private void FiberConnect(IGTKeyObject objFrom, IGTKeyObject objNode, IGTKeyObject objTo, int LOW1, int HIGH1, int LOW2, int HIGH2, string CORE_STATUS)
        {
            try
            {
                string sSql = null;
                string CABLE_CODE = string.Empty;
                string EXC_ABB = null;
                string FEATURE_STATE = null;
                int recordsAffected = 0;
                int lastSEQ = 0;
                string SEQ = null;
                short G_CNO = 0;
                short L_CNO = 0;
                short S_CNO = 0;
                IGTTextPointGeometry oTextGeom;
                IGTPointGeometry oPointGeom;
                IGTPoint oPointS;
                IGTPoint oPointL;
                string TERM_FID= string.Empty;
                string TERM_FNO = string.Empty;
                string TERM_LOW = string.Empty;
                string TERM_HIGH = string.Empty;
                
                switch (objNode.FNO)
                {
                    case 11800: //Stump
                        G_CNO = 11822;
                        L_CNO = 11834;
                        S_CNO = 11820;
                        break;
                    case 5100: //Fiber Distribution Cabinet 
                        G_CNO = 5122;
                        L_CNO = 5134;
                        S_CNO = 5130; //GC_FDC_T
                        break;
                    case 5200: //Edge Provider Edge
                        G_CNO = 5222;
                        L_CNO = 5234;
                        S_CNO = 5220;
                        break;
                    case 5400: //User Provider Edge
                        G_CNO = 5422;
                        L_CNO = 5434;
                        S_CNO = 5420;
                        break;
                    case 5500: //Optical Distribution Frame
                        G_CNO = 5522;
                        L_CNO = 5534;
                        S_CNO = 5520;
                        break;
                    case 5600: //Fiber Distribution Point
                        G_CNO = 5622;
                        L_CNO = 5634;
                        S_CNO = 5620;
                        break;
                    case 5800: //Tie FDP
                        G_CNO = 5822;
                        L_CNO = 5834;
                        S_CNO = 5820;
                        break;
                    case 5900: //FTB
                        G_CNO = 5922;
                        L_CNO = 5934;
                        S_CNO = 5920;
                        break;
                    case 9100: //MSAN
                        G_CNO = 9122;
                        L_CNO = 9134;
                        S_CNO = 9120;
                        break;
                    case 9300: //DDN
                        G_CNO = 9322;
                        L_CNO = 9334;
                        S_CNO = 9320;
                        break;
                    case 9400: //NDH
                        G_CNO = 9422;
                        L_CNO = 9434;
                        S_CNO = 9420;
                        break;
                    case 9500: //MINMUX
                        G_CNO = 9522;
                        L_CNO = 9534;
                        S_CNO = 9520;
                        break;
                    case 9600: //Remote Terminal
                        G_CNO = 9622;
                        L_CNO = 9634;
                        S_CNO = 9620;
                        break;
                    case 9700: //Fiber Access Node
                        G_CNO = 9722;
                        L_CNO = 9734;
                        S_CNO = 9720;
                        break;
                    case 9800: //VDSL2
                        G_CNO = 9822;
                        L_CNO = 9834;
                        S_CNO = 9820;
                        break;
                    case 9900: //DB
                        G_CNO = 9922;
                        L_CNO = 9934;
                        S_CNO = 9920;
                        break;
                }

                //save the TERM_xxx on EVERY Joint/FSplice and Device/Enclosure 17-Sep-2012 
                //objNode.FNO != 11800 &&
                //&& !(objFrom.FNO == 12300 && objTo.FNO == 7400)
                if (objNode.FNO != 5500 && objNode.FNO != 4900 && objNode.FNO != 12200 )
                {
                    try
                    {
                        sSql = "Select A.FNO2, A.FID2, A.LOW2, A.HIGH2 from table (select AG_GET_CORECOUNT (" + objNode.FNO + ", " + objNode.FID + ", " + objFrom.FNO + ", " + objFrom.FID + ", " + LOW1 + ", " + HIGH1 + ") from dual ) A";
                        Recordset rs = GTClassFactory.Create<IGTApplication>().DataContext.Execute(sSql, out recordsAffected, (int)CommandTypeEnum.adCmdText, null);
                        if (rs.RecordCount > 0)
                        {
                            rs.MoveFirst();
                            if ((lbl_Core_FID.Text != rs.Fields["FID2"].Value.ToString()) && (objNode.FNO != 11800))
                            {
                                MessageBox.Show("Possible Trace/Splice Error due to Missing Data", "Core Count", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                return;
                            }
                            if (rs.Fields["FID2"].Value.ToString() == "-1")
                            {
                                MessageBox.Show("CABLE or SPLICE connection Error, Possibly at <G3E_FID> returned in another field" + "\n" + sSql, "Core Count", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                return;
                            }

                            TERM_FID= rs.Fields["FID2"].Value.ToString();
                            TERM_FNO= rs.Fields["FNO2"].Value.ToString();
                            TERM_LOW= rs.Fields["LOW2"].Value.ToString();
                            TERM_HIGH= rs.Fields["HIGH2"].Value.ToString();

                            if (rs.Fields["FNO2"].Value.ToString() == "7200")
                                CABLE_CODE = Get_Value("select CABLE_CODE from GC_fcbl where g3e_fid = " + rs.Fields["FID2"].Value.ToString());
                            else if (rs.Fields["FNO2"].Value.ToString() == "7400")
                                CABLE_CODE = Get_Value("select FCABLE_CODE from GC_FDCBL where g3e_fid = " + rs.Fields["FID2"].Value.ToString());
                            else if (rs.Fields["FNO2"].Value.ToString() == "4400")
                                CABLE_CODE = Get_Value("select CABLE_CODE from GC_FCBL_TRUNK where g3e_fid = " + rs.Fields["FID2"].Value.ToString());
                            else if (rs.Fields["FNO2"].Value.ToString() == "4500")
                                CABLE_CODE = Get_Value("select CABLE_CODE from GC_FCBL_JNT where g3e_fid = " + rs.Fields["FID2"].Value.ToString());

                            //Validation
                            //          where TERM_FID=xx and CORE_STATUS='MAIN' and TERM_LO and TERM_HIGH
                            //          where TERM_FID=xx and CORE_STATUS='PROTECTION' and TERM_LO and TERM_HIGH
                            string Term_Cnt = Get_Value("SELECT count(*) as cnt FROM gc_splice_connect WHERE  TERM_FID =" + TERM_FID + " AND TERM_FNO = " + TERM_FNO  + " AND TERM_LOW = " + TERM_LOW + " AND TERM_HIGH = " + TERM_HIGH + " AND CORE_STATUS =" + CORE_STATUS + " AND CORE_STATUS IS NOT NULL");
                            if ( Convert.ToInt32(Term_Cnt) > 0)
                            {
                                MessageBox.Show("Core Count strands already Connected" + "\n" + "TERM_FID = " + TERM_FID + " TERM_FNO = " + TERM_FNO + " TERM_LOW = " + TERM_LOW + " TERM_HIGH = " + TERM_HIGH + " CORE_STATUS = " + CORE_STATUS + "\n" + sSql, "Core Count", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                return;
                            }    

                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message + "\n" + sSql, "AG_GET_CORECOUNT", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }

                GTCustomCommandModeless.m_oGTTransactionManager.Begin("SpliceConnect");
                oPointS = GTClassFactory.Create<IGTPoint>();
                oPointL = GTClassFactory.Create<IGTPoint>();
                IGTKeyObject OFet = GTClassFactory.Create<IGTApplication>().DataContext.OpenFeature(objNode.FNO, objNode.FID);

                //Symbol
                if (objNode.FNO == 11800)
                {
                    oPointS.X = OFet.Components.GetComponent(S_CNO).Geometry.FirstPoint.X - 7;
                    oPointS.Y = OFet.Components.GetComponent(S_CNO).Geometry.FirstPoint.Y + 1;
                    oPointS.Z = OFet.Components.GetComponent(S_CNO).Geometry.FirstPoint.Z;
                }
                else if (objNode.FNO == 5100)
                {
                    oPointS.X = OFet.Components.GetComponent(S_CNO).Geometry.FirstPoint.X;
                    oPointS.Y = OFet.Components.GetComponent(S_CNO).Geometry.FirstPoint.Y + 1;
                    oPointS.Z = OFet.Components.GetComponent(S_CNO).Geometry.FirstPoint.Z;
                }
                else
                {
                    oPointS.X = OFet.Components.GetComponent(S_CNO).Geometry.FirstPoint.X;
                    oPointS.Y = OFet.Components.GetComponent(S_CNO).Geometry.FirstPoint.Y + 1;
                    oPointS.Z = OFet.Components.GetComponent(S_CNO).Geometry.FirstPoint.Z;
                }

                //Label
                if (objNode.FNO == 11800)
                {
                    oPointL.X = OFet.Components.GetComponent(S_CNO).Geometry.FirstPoint.X - 7;
                    oPointL.Y = OFet.Components.GetComponent(S_CNO).Geometry.FirstPoint.Y - 1;
                    oPointL.Z = OFet.Components.GetComponent(S_CNO).Geometry.FirstPoint.Z;
                }
                else if (objNode.FNO == 5100)
                {
                    oPointL.X = OFet.Components.GetComponent(S_CNO).Geometry.FirstPoint.X;
                    oPointL.Y = OFet.Components.GetComponent(S_CNO).Geometry.FirstPoint.Y - 1;
                    oPointL.Z = OFet.Components.GetComponent(S_CNO).Geometry.FirstPoint.Z;
                }
                else
                {
                    oPointL.X = OFet.Components.GetComponent(S_CNO).Geometry.FirstPoint.X;
                    oPointL.Y = OFet.Components.GetComponent(S_CNO).Geometry.FirstPoint.Y - 1;
                    oPointL.Z = OFet.Components.GetComponent(S_CNO).Geometry.FirstPoint.Z;
                }


                if (OFet.Components.GetComponent(77).Recordset.EOF)
                {
                    SEQ = "1";

                    OFet.Components.GetComponent(77).Recordset.AddNew("G3E_FNO", objNode.FNO);
                    OFet.Components.GetComponent(77).Recordset.Fields["SEQ"].Value = SEQ;
                    OFet.Components.GetComponent(77).Recordset.Fields["G3E_FNO"].Value = objNode.FNO;
                    OFet.Components.GetComponent(77).Recordset.Fields["G3E_FID"].Value = objNode.FID;
                    OFet.Components.GetComponent(77).Recordset.Fields["G3E_CNO"].Value = 77;
                    OFet.Components.GetComponent(77).Recordset.Fields["G3E_CID"].Value = 1;
                    OFet.Components.GetComponent(77).Recordset.Fields["FNO1"].Value = objFrom.FNO;
                    OFet.Components.GetComponent(77).Recordset.Fields["FID1"].Value = objFrom.FID;
                    OFet.Components.GetComponent(77).Recordset.Fields["LOW1"].Value = LOW1;
                    OFet.Components.GetComponent(77).Recordset.Fields["HIGH1"].Value = HIGH1;
                    OFet.Components.GetComponent(77).Recordset.Fields["FNO2"].Value = objTo.FNO;
                    OFet.Components.GetComponent(77).Recordset.Fields["FID2"].Value = objTo.FID;
                    OFet.Components.GetComponent(77).Recordset.Fields["LOW2"].Value = LOW2;
                    OFet.Components.GetComponent(77).Recordset.Fields["HIGH2"].Value = HIGH2;

                    if (objFrom.FNO == 12200 || objFrom.FNO == 5500 || objFrom.FNO == 4900 || objFrom.FNO == 9100 || objFrom.FNO == 9600 || objFrom.FNO == 5800)
                    {
                        OFet.Components.GetComponent(77).Recordset.Fields["NODE_FNO1"].Value = objFrom.FNO;
                        OFet.Components.GetComponent(77).Recordset.Fields["NODE_FID1"].Value = objFrom.FID;
                    }
                    else if (objTo.FNO == 12200 || objTo.FNO == 5500 || objTo.FNO == 4900 || objTo.FNO == 9100 || objTo.FNO == 9600 || objTo.FNO == 5800)
                    {
                        OFet.Components.GetComponent(77).Recordset.Fields["NODE_FNO1"].Value = objTo.FNO;
                        OFet.Components.GetComponent(77).Recordset.Fields["NODE_FID1"].Value = objTo.FID;
                    }
                    else
                    {
                        OFet.Components.GetComponent(77).Recordset.Fields["NODE_FNO1"].Value = objNode.FNO;
                        OFet.Components.GetComponent(77).Recordset.Fields["NODE_FID1"].Value = objNode.FID;
                    }
                    OFet.Components.GetComponent(77).Recordset.Update("CONNECTION_TYPE", "Continuous");
                    OFet.Components.GetComponent(77).Recordset.Update("CORE_STATUS", CORE_STATUS);
                    OFet.Components.GetComponent(77).Recordset.Update("LOSS_MEASURED", "0");
                    OFet.Components.GetComponent(77).Recordset.Update("LOSS_TYPICAL", "0");

                    //New Columns
                    EXC_ABB = Get_Value("SELECT EXC_ABB FROM GC_NETELEM WHERE G3E_FID = " + objNode.FID);
                    FEATURE_STATE = Get_Value("SELECT FEATURE_STATE FROM GC_NETELEM WHERE G3E_FID = " + objNode.FID);
                    OFet.Components.GetComponent(77).Recordset.Update("EXC_ABB", EXC_ABB);
                    OFet.Components.GetComponent(77).Recordset.Update("FEATURE_STATE", FEATURE_STATE);

                    if (TERM_FID != "") OFet.Components.GetComponent(77).Recordset.Update("TERM_FID", TERM_FID);
                    if (TERM_FNO != "") OFet.Components.GetComponent(77).Recordset.Update("TERM_FNO", TERM_FNO);
                    if (TERM_LOW != "") OFet.Components.GetComponent(77).Recordset.Update("TERM_LOW", TERM_LOW);
                    if (TERM_HIGH != "") OFet.Components.GetComponent(77).Recordset.Update("TERM_HIGH", TERM_HIGH);
                    if (CABLE_CODE != "") OFet.Components.GetComponent(77).Recordset.Update("CABLE_CODE", CABLE_CODE);
                                       
                    //Core Count Symbol 
                    //Update CORE_STATUS to GC_XXX_S2 Component
                    if ((G_CNO == 11822 && CORE_STATUS == "SPARE") || (G_CNO == 11822 && CORE_STATUS == "FOMS") || (G_CNO == 11822 && CORE_STATUS == "STUMP") || G_CNO == 5622 || G_CNO == 5122)
                    {
                        oPointGeom = GTClassFactory.Create<IGTPointGeometry>();
                        oPointGeom.Origin = oPointS;

                        if (OFet.Components.GetComponent(G_CNO).Recordset.EOF)
                        {
                            OFet.Components.GetComponent(G_CNO).Recordset.AddNew("G3E_FID", objNode.FID);
                            OFet.Components.GetComponent(G_CNO).Recordset.Update("G3E_FNO", objNode.FNO);
                            OFet.Components.GetComponent(G_CNO).Recordset.Update("G3E_CID", SEQ);                            
                            OFet.Components.GetComponent(G_CNO).Recordset.Update("CORE_STATUS", CORE_STATUS);
                            OFet.Components.GetComponent(G_CNO).Geometry = oPointGeom;
                        }
                    }

                    //Core Count Text
                    //FSPLICE, not needed unless there is SPARE or STUMP
                    if ((L_CNO != 11834 || (L_CNO == 11834 && CORE_STATUS == "SPARE") || (L_CNO == 11834 && CORE_STATUS == "STUMP")))
                    {
                        oTextGeom = GTClassFactory.Create<IGTTextPointGeometry>();
                        oTextGeom.Origin = oPointL;
                        oTextGeom.Rotation = 0;

                        if (OFet.Components.GetComponent(L_CNO).Recordset.EOF)
                        {
                            OFet.Components.GetComponent(L_CNO).Recordset.AddNew("G3E_FID", objNode.FID);
                            OFet.Components.GetComponent(L_CNO).Recordset.Update("G3E_FNO", objNode.FNO);
                            OFet.Components.GetComponent(L_CNO).Recordset.Update("G3E_CID", SEQ);
                            OFet.Components.GetComponent(L_CNO).Geometry = oTextGeom;
                        }
                    }
                    
                    
                }
                else
                {                    
                    while (!OFet.Components.GetComponent(77).Recordset.EOF)
                    {
                        if (lastSEQ < Convert.ToInt32(OFet.Components.GetComponent(77).Recordset.Fields["G3E_CID"].Value))
                            lastSEQ = Convert.ToInt32(OFet.Components.GetComponent(77).Recordset.Fields["G3E_CID"].Value);
                        OFet.Components.GetComponent(77).Recordset.MoveNext();
                    }
                    lastSEQ++;
                    SEQ = lastSEQ.ToString();

                    OFet.Components.GetComponent(77).Recordset.AddNew("G3E_FNO", objNode.FNO);
                    OFet.Components.GetComponent(77).Recordset.Fields["SEQ"].Value = 1;
                    //OFet.Components.GetComponent(77).Recordset.Fields["SEQ"].Value = lastSEQ;
                    OFet.Components.GetComponent(77).Recordset.Fields["G3E_FNO"].Value = objNode.FNO;
                    OFet.Components.GetComponent(77).Recordset.Fields["G3E_FID"].Value = objNode.FID;
                    OFet.Components.GetComponent(77).Recordset.Fields["G3E_CNO"].Value = 77;
                    OFet.Components.GetComponent(77).Recordset.Fields["G3E_CID"].Value = SEQ;
                    OFet.Components.GetComponent(77).Recordset.Fields["FNO1"].Value = objFrom.FNO;
                    OFet.Components.GetComponent(77).Recordset.Fields["FID1"].Value = objFrom.FID;
                    OFet.Components.GetComponent(77).Recordset.Fields["LOW1"].Value = LOW1;
                    OFet.Components.GetComponent(77).Recordset.Fields["HIGH1"].Value = HIGH1;
                    OFet.Components.GetComponent(77).Recordset.Fields["FNO2"].Value = objTo.FNO;
                    OFet.Components.GetComponent(77).Recordset.Fields["FID2"].Value = objTo.FID;
                    OFet.Components.GetComponent(77).Recordset.Fields["LOW2"].Value = LOW2;
                    OFet.Components.GetComponent(77).Recordset.Fields["HIGH2"].Value = HIGH2;

                    if (objFrom.FNO == 12200 || objFrom.FNO == 5500 || objFrom.FNO == 4900 || objFrom.FNO == 9100 || objFrom.FNO == 9600 || objFrom.FNO == 5800)
                    {
                        OFet.Components.GetComponent(77).Recordset.Fields["NODE_FNO1"].Value = objFrom.FNO;
                        OFet.Components.GetComponent(77).Recordset.Fields["NODE_FID1"].Value = objFrom.FID;
                    }
                    else if (objTo.FNO == 12200 || objTo.FNO == 5500 || objTo.FNO == 4900 || objTo.FNO == 9100 || objTo.FNO == 9600 || objTo.FNO == 5800)
                    {
                        OFet.Components.GetComponent(77).Recordset.Fields["NODE_FNO1"].Value = objTo.FNO;
                        OFet.Components.GetComponent(77).Recordset.Fields["NODE_FID1"].Value = objTo.FID;
                    }
                    else
                    {
                        OFet.Components.GetComponent(77).Recordset.Fields["NODE_FNO1"].Value = objNode.FNO;
                        OFet.Components.GetComponent(77).Recordset.Fields["NODE_FID1"].Value = objNode.FID;
                    }                   
                    
                    OFet.Components.GetComponent(77).Recordset.Update("G3E_FNO", objNode.FNO);
                    OFet.Components.GetComponent(77).Recordset.Update("CONNECTION_TYPE", "Continuous");
                    OFet.Components.GetComponent(77).Recordset.Update("CORE_STATUS", CORE_STATUS);
                    OFet.Components.GetComponent(77).Recordset.Update("LOSS_MEASURED", "0");
                    OFet.Components.GetComponent(77).Recordset.Update("LOSS_TYPICAL", "0");

                    //New Columns
                    EXC_ABB = Get_Value("SELECT EXC_ABB FROM GC_NETELEM WHERE G3E_FID = " + objNode.FID);
                    FEATURE_STATE = Get_Value("SELECT FEATURE_STATE FROM GC_NETELEM WHERE G3E_FID = " + objNode.FID);
                    OFet.Components.GetComponent(77).Recordset.Update("EXC_ABB", EXC_ABB);
                    OFet.Components.GetComponent(77).Recordset.Update("FEATURE_STATE", FEATURE_STATE);

                    if (TERM_FID != "") OFet.Components.GetComponent(77).Recordset.Update("TERM_FID", TERM_FID);
                    if (TERM_FNO != "") OFet.Components.GetComponent(77).Recordset.Update("TERM_FNO", TERM_FNO);
                    if (TERM_LOW != "") OFet.Components.GetComponent(77).Recordset.Update("TERM_LOW", TERM_LOW);
                    if (TERM_HIGH != "") OFet.Components.GetComponent(77).Recordset.Update("TERM_HIGH", TERM_HIGH);
                    if (CABLE_CODE != "") OFet.Components.GetComponent(77).Recordset.Update("CABLE_CODE", CABLE_CODE);
                    
                    //Core Count Symbol
                    //Update CORE_STATUS to GC_XXX_S2 Component
                    if ((G_CNO == 11822 && CORE_STATUS == "SPARE") || (G_CNO == 11822 && CORE_STATUS == "FOMS") || (G_CNO == 11822 && CORE_STATUS == "STUMP") || G_CNO == 5622 || G_CNO == 5122)
                    {
                        oPointGeom = GTClassFactory.Create<IGTPointGeometry>();
                        oPointGeom.Origin = oPointS;

                        if (OFet.Components.GetComponent(G_CNO).Recordset.EOF)
                        {
                            OFet.Components.GetComponent(G_CNO).Recordset.AddNew("G3E_FID", objNode.FID);
                            OFet.Components.GetComponent(G_CNO).Recordset.Update("G3E_FNO", objNode.FNO);
                            OFet.Components.GetComponent(G_CNO).Recordset.Update("G3E_CID", SEQ);
                            OFet.Components.GetComponent(G_CNO).Recordset.Update("CORE_STATUS", CORE_STATUS);
                            OFet.Components.GetComponent(G_CNO).Geometry = oPointGeom;
                        }
                        else
                        {
                            OFet.Components.GetComponent(G_CNO).Recordset.MoveLast();
                            OFet.Components.GetComponent(G_CNO).Geometry = oPointGeom;
                        }
                    }

                    //Core Count Text
                    //FSPLICE, not needed unless there is SPARE or STUMP
                    if ((L_CNO != 11834 || (L_CNO == 11834 && CORE_STATUS == "SPARE") || (L_CNO == 11834 && CORE_STATUS == "STUMP")))
                    {
                        oTextGeom = GTClassFactory.Create<IGTTextPointGeometry>();
                        oTextGeom.Origin = oPointL;
                        oTextGeom.Rotation = 0;

                        if (OFet.Components.GetComponent(L_CNO).Recordset.EOF)
                        {
                            OFet.Components.GetComponent(L_CNO).Recordset.AddNew("G3E_FID", objNode.FID);
                            OFet.Components.GetComponent(L_CNO).Recordset.Update("G3E_FNO", objNode.FNO);
                            OFet.Components.GetComponent(L_CNO).Recordset.Update("G3E_CID", SEQ);
                            OFet.Components.GetComponent(L_CNO).Geometry = oTextGeom;
                        }
                        else
                        {
                            OFet.Components.GetComponent(L_CNO).Recordset.MoveLast();
                            OFet.Components.GetComponent(L_CNO).Geometry = oTextGeom;
                        }
                    }
                    
                }
                GTCustomCommandModeless.m_oGTTransactionManager.Commit();                
            }
            catch (Exception ex)
            {
                GTCustomCommandModeless.m_oGTTransactionManager.Rollback();
                MessageBox.Show(ex.Message, "Core Count", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        //Get the Selected Feature
        private void Select_Feature()
        {
            try
            {
                IGTGeometry geom = null;
                short iFNO = 0;
                int iFID = 0;
                string MIN_MAT = null;

                if (GTClassFactory.Create<IGTApplication>().SelectedObjects.FeatureCount == 0)
                {
                    flag = true;
                    MessageBox.Show("Please select a Device", "Core Count", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }                             

                foreach (IGTDDCKeyObject oDDCKeyObject in GTClassFactory.Create<IGTApplication>().SelectedObjects.GetObjects())
                {
                    geom = oDDCKeyObject.Geometry;
                    iFNO = oDDCKeyObject.FNO;
                    iFID = oDDCKeyObject.FID;

                    if (iFNO == FDC_FNO || iFNO == UPE_FNO || iFNO == FDP_FNO || iFNO == FTB_FNO || iFNO == TIE_FNO || iFNO == DB_FNO || iFNO == MSAN_FNO || iFNO == DDN_FNO || iFNO == NDH_FNO || iFNO == MUX_FNO || iFNO == EPE_FNO || iFNO == FAN_FNO || iFNO == RT_FNO || iFNO == VDSL_FNO || iFNO == ODF_FNO || iFNO == FPATCH_FNO || iFNO == FPP_FNO || iFNO == FSE_FNO || iFNO == FST_FNO || iFNO == FSJ_FNO)
                    {
                        lbl_FID.Text = iFID.ToString();
                        lbl_fno.Text = iFNO.ToString();

                        MIN_MAT = Get_Value("select MIN_MATERIAL from GC_NETELEM where G3E_FID = " + iFID.ToString());
                        if (MIN_MAT == "")
                        {
                            flag = true;
                            MessageBox.Show("No Plant Unit selected for the Device", "Core Count", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            return;
                        }

                        //Get the Font Name, Letter and Size
                        Font font;
                        ADODB.Recordset rs = new ADODB.Recordset();
                        string iCNO = Get_Value("SELECT G3E_PRIMARYGEOGRAPHICCNO from G3E_FEATURE WHERE G3E_FNO = " + iFNO);
                        string sSql = "SELECT G3E_FONTNAME, G3E_SYMBOL, G3E_SIZE FROM G3E_POINTSTYLE WHERE G3E_SNO IN ( SELECT G3E_SNO FROM G3E_STYLERULE WHERE G3E_SRNO IN ( SELECT L.G3E_SRNO FROM G3E_COMPONENTVIEWDEFINITION CVD, G3E_LEGENDENTRY L WHERE G3E_FNO = " + iFNO + " AND G3E_CNO = " + iCNO + " AND L.G3E_LENO = CVD.G3E_LENO )AND g3e_stylerule.g3e_filter is null )";
                        rs = m_IGTDataContext.OpenRecordset(sSql, ADODB.CursorTypeEnum.adOpenStatic, ADODB.LockTypeEnum.adLockReadOnly, 1);
                        if (rs.RecordCount > 0)
                        {
                            rs.MoveFirst();                            
                            if (iCNO == "9520")
                            {
                                font = new Font(rs.Fields[0].Value.ToString(), float.Parse(rs.Fields[2].Value.ToString())-100);
                            }
                            else
                            {
                                font = new Font(rs.Fields[0].Value.ToString(), float.Parse(rs.Fields[2].Value.ToString()));
                            }

                            lbl_Symbol.Font = font;
                            lbl_Symbol.Text = rs.Fields[1].Value.ToString();
                        }
                        rs.Close();

                        lbl_Exch.Text = "Exchange : " + Get_Value("select EXC_ABB from gc_netelem where g3e_fid = " + iFID);
                        lbl_DeviceName.Text = Get_Value("select g3e_username from g3e_feature where g3e_fno = " + iFNO);
                        
                        if (iFNO == FDC_FNO ) 
                        {
                            lbl_Device.Text = "Code : " + Get_Value("select FDC_CODE from GC_FDC where g3e_fid = " + iFID.ToString());
                        }
                        else if (iFNO == UPE_FNO)
                        {
                            lbl_Device.Text = "Code : " + Get_Value("select UPE_CODE from GC_UPE where g3e_fid = " + iFID.ToString());
                        }
                        else if (iFNO == FDP_FNO)
                        {
                            lbl_Device.Text = "Code : " + Get_Value("select FDP_CODE from GC_FDP where g3e_fid = " + iFID.ToString());
                        }
                        else if (iFNO == FTB_FNO)
                        {
                            lbl_Device.Text = "Code : " + Get_Value("select FDP_CODE from GC_FTB where g3e_fid = " + iFID.ToString());
                        }
                        else if (iFNO == TIE_FNO)
                        {
                            lbl_Device.Text = "Code : " + Get_Value("select FDP_CODE from GC_TIEFDP where g3e_fid = " + iFID.ToString());
                        }
                        else if (iFNO == DB_FNO)
                        {
                            lbl_Device.Text = "Code : " + Get_Value("select DB_CODE from GC_DB where g3e_fid = " + iFID.ToString());
                        }
                        else if (iFNO == MSAN_FNO) 
                        {
                            lbl_Device.Text = "Code : " + Get_Value("select RT_CODE from GC_MSAN where g3e_fid = " + iFID.ToString());
                        } 
                        else if ( iFNO == DDN_FNO) 
                        {
                            lbl_Device.Text = "Code : " + Get_Value("select DDN_CODE from GC_DDN where g3e_fid = " + iFID.ToString());
                        } 
                        else if ( iFNO == NDH_FNO) 
                        {
                            lbl_Device.Text = "Code : " + Get_Value("select NDH_CODE from GC_NDH where g3e_fid = " + iFID.ToString());
                        } 
                        else if ( iFNO == MUX_FNO) 
                        {
                            lbl_Device.Text = "Code : " + Get_Value("select MUX_CODE from GC_MINIMUX where g3e_fid = " + iFID.ToString());
                        } 
                        else if ( iFNO == EPE_FNO) 
                        {
                            lbl_Device.Text = "Code : " + Get_Value("select EPE_CODE from GC_EPE where g3e_fid = " + iFID.ToString());
                        } 
                        else if ( iFNO == FAN_FNO) 
                        {
                            lbl_Device.Text = "Code : " + Get_Value("select FAN_CODE from GC_FAN where g3e_fid = " + iFID.ToString());
                        } 
                        else if ( iFNO == RT_FNO) 
                        {
                            lbl_Device.Text = "Code : " + Get_Value("select RT_CODE from GC_RT where g3e_fid = " + iFID.ToString());
                        }                         
                        else if (iFNO == ODF_FNO)
                        {
                            lbl_Device.Text = "Code : " + Get_Value("select ODF_NUM from GC_ODF where g3e_fid = " + iFID.ToString());
                        }
                        else if (iFNO == FPATCH_FNO)
                        {
                            lbl_Device.Text = "Code : " + Get_Value("select PATCH_CODE from GC_FPATCH where g3e_fid = " + iFID.ToString());
                        }
                        else if (iFNO == FPP_FNO)
                        {
                            lbl_Device.Text = "Code : " + Get_Value("select PATCH_CODE from GC_FPATCHPANEL where g3e_fid = " + iFID.ToString());
                        }
                        else if ( iFNO == FSE_FNO) 
                        {
                            lbl_Device.Text = "Code : " + Get_Value("select SPLICE_CODE from GC_FSPLICE where g3e_fid = " + iFID.ToString());                            
                        }                        
                    }
                    else
                    {
                        flag = true;
                        MessageBox.Show("This selected feature is not a valid to edit splice connection with.", "Core Count", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }
                }                
                
            }
            catch (Exception ex)
            {
                log.WriteLog("");
                log.WriteLog(this.GetType().FullName, "ERROR: " + ex.Message);
                log.WriteLog(ex.StackTrace);
                MessageBox.Show(ex.Message, "Core Count", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void btn_Disconnect_Click(object sender, EventArgs e)
        {
            DialogResult retVal = MessageBox.Show("Are you sure to Disconnect the selected Rows?", "SpliceConnect", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (retVal == DialogResult.Yes)
            {
                Update_Splice();
            }
        }

        private void gv_Route_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.ColumnIndex == 1 && e.RowIndex > -1)
            {
                Image img = Image.FromFile("\\Image\\delete.gif");
                e.Graphics.DrawImage(img, e.CellBounds.Location);
                e.PaintContent(e.CellBounds);
                e.Handled = true;
            }
        }


        private void gv_Route_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                switch (e.ColumnIndex)
                {
                    case 1:
                        if (Convert.ToBoolean(gv_Route[2, e.RowIndex].Value) == true)
                        {
                            gv_Route[2, e.RowIndex].Value = false;
                        }
                        else
                        {
                            gv_Route[2, e.RowIndex].Value = true;
                        }
                        DialogResult retVal = MessageBox.Show("Are you sure to Disconnect?", "SpliceConnect", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (retVal == DialogResult.Yes)
                        {
                            Update_Splice();
                        }
                        else
                            gv_Route[2, e.RowIndex].Value = false;

                        break;

                    case 0:
                        //gv_Route[5, e.RowIndex].ReadOnly = false;
                        //gv_Route[6, e.RowIndex].ReadOnly = false;

                        //gv_Route[9, e.RowIndex].ReadOnly = false;
                        //gv_Route[10, e.RowIndex].ReadOnly = false; 


                        SetComboValue(cboFrom, gv_Route[3, e.RowIndex].Value.ToString()+ " - " + gv_Route[4, e.RowIndex].Value.ToString() );
                        
                        txtLOW1.Text = gv_Route[5, e.RowIndex].Value.ToString();
                        txtHIGH1.Text = gv_Route[6, e.RowIndex].Value.ToString();

                        txtLOW2.Text = gv_Route[9, e.RowIndex].Value.ToString();
                        txtHIGH2.Text = gv_Route[10, e.RowIndex].Value.ToString();

                        edit_g3e_id = Convert.ToInt32(gv_Route[16, e.RowIndex].Value);

                        string Name = gv_Route[7, e.RowIndex].Value.ToString() + " - " + gv_Route[8, e.RowIndex].Value.ToString();
                        SetComboValue(cboTo , Name );                   

                        cboFrom.Enabled = false;
                        cboTo.Enabled = false;                        

                        btn_Connect.Text = "Update";
                                             
                        break;
                }
            }
            catch (Exception ex)
            {
                log.WriteLog("");
                log.WriteLog(this.GetType().FullName, "ERROR: " + ex.Message);
                log.WriteLog(ex.StackTrace);
                MessageBox.Show(ex.Message, "Error.", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

      
        private void Update_Splice()
        {
            try
            {
                short iCNO;
                IGTKeyObject oNewFeature;
                Recordset rsDel = null;
                string sSql = "";
                int recordsAffected = 0;
                Recordset rsComp = null;
               
                int iSession = 1;
                int iCable_FID = 0;
                int iRecordNum = 0;
                object[] obj = null;

                bool flag = false ;
                string G3E_ID = null;
                int Stump_Cnt = 0;
                int Spare_Cnt = 0;
                int Splice_Cnt = 0;
                string Stump = null;
                string Spare = null;
                string Splice = null;
                string G_CNO =null;
                string L_CNO =null;
                                
                string strActiveJob = GTClassFactory.Create<IGTApplication>().DataContext.ActiveJob;

                string PP_WO = strActiveJob;

                for (int i = 0; i <= gv_Route.Rows.Count - 1; i++)
                {
                    if (Convert.ToBoolean(gv_Route.Rows[i].Cells[2].Value) == true)
                    {
                        G3E_ID = G3E_ID + gv_Route.Rows[i].Cells[16].Value.ToString() + ",";
                        //if (gv_Route.Rows[i].Cells[18].Value.ToString() == "FOMS") //Stump
                        //    Stump_Cnt++;
                        //if (gv_Route.Rows[i].Cells[18].Value.ToString() == "SPARE") //Spare
                        //    Spare_Cnt++;
                        Splice_Cnt++;
                    }
                }

                if (G3E_ID != null)
                        G3E_ID = G3E_ID.Substring(0, G3E_ID.Length - 1);

                if (G3E_ID == null)
                {
                    MessageBox.Show("Please Select a Row to Disconnect.", "Core Count", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                GTCustomCommandModeless.m_oGTTransactionManager.Begin("SpliceConnect");
                sSql = "Delete from GC_SPLICE_CONNECT where g3e_id IN (" + G3E_ID + ")";
                rsDel = m_IGTDataContext.Execute(sSql, out iRecordNum, (int)ADODB.CommandTypeEnum.adCmdText, obj); 
                GTCustomCommandModeless.m_oGTTransactionManager.Commit();

                //Delete Symbol and Label Component                
                switch (lbl_fno.Text)
                {
                    case "11800": //Splice
                        G_CNO = "GC_FSPLICE_S2";
                        L_CNO = "GC_FSPLICE_T3";                        
                        break;
                    case "5100": //Fiber Distribution Cabinet 
                        G_CNO = "GC_FDC_S2";
                        L_CNO = "GC_FDC_T3";                        
                        break;
                    case "5200": //Edge Provider Edge
                        G_CNO = "GC_EPE_S2";
                        L_CNO = "GC_EPE_T3";                        
                        break;
                    case "5400": //User Provider Edge
                        G_CNO = "GC_UPE_S2";
                        L_CNO = "GC_UPE_T3";                        
                        break;
                    case "5500": //Optical Distribution Frame
                        G_CNO = "GC_ODF_S2";
                        L_CNO = "GC_ODF_T3";                        
                        break;
                    case "5600": //Fiber Distribution Point
                        G_CNO = "GC_FDP_S2";
                        L_CNO = "GC_FDP_T3";                        
                        break;
                    case "5800": //Tie FDP
                        G_CNO = "GC_TIEFDP_S2";
                        L_CNO = "GC_TIEFDP_T3";                        
                        break;
                    case "5900": //FTB
                        G_CNO = "GC_FTB_S2";
                        L_CNO = "GC_FTB_T3";                        
                        break;
                    case "9100": //MSAN
                        G_CNO = "GC_MSAN_S2";
                        L_CNO = "GC_MSAN_T3";                        
                        break;
                    case "9300": //DDN
                        G_CNO = "GC_DDN_S2";
                        L_CNO = "GC_DDN_T3";                        
                        break;
                    case "9400": //NDH
                        G_CNO = "GC_NDH _S2";
                        L_CNO = "GC_NDH_T3";                        
                        break;
                    case "9500": //MINMUX
                        G_CNO = "GC_MINIMUX_S2";
                        L_CNO = "GC_MINIMUX_T3";                        
                        break;
                    case "9600": //Remote Terminal
                        G_CNO = "GC_RT_S2";
                        L_CNO = "GC_RT_T3";                        
                        break;
                    case "9700": //Fiber Access Node
                        G_CNO = "GC_FAN_S2";
                        L_CNO = "GC_FAN_T3";                        
                        break;
                    case "9800": //VDSL2
                        G_CNO = "GC_VDSL2_S2";
                        L_CNO = "GC_VDSL2_T3";                        
                        break;
                    case "9900": //DB
                        G_CNO = "GC_DB_S2";
                        L_CNO = "GC_DB_T3";                        
                        break;
                }

                if (Splice_Cnt > 0)
                {
                    GTCustomCommandModeless.m_oGTTransactionManager.Begin("SpliceDelete");

                    Splice = Get_Value("Select count(*) as cnt from GC_SPLICE_CONNECT where G3E_FNO = " + lbl_fno.Text + " and G3E_FID = " + lbl_FID.Text );
                    if (Splice == "0")
                    {                        
                        sSql = "Delete from " + G_CNO + " where g3e_fid = " + lbl_FID.Text;
                        rsDel = m_IGTDataContext.Execute(sSql, out iRecordNum, (int)ADODB.CommandTypeEnum.adCmdText, obj);
                        sSql = "Delete from " + L_CNO + " where g3e_fid = " + lbl_FID.Text;
                        rsDel = m_IGTDataContext.Execute(sSql, out iRecordNum, (int)ADODB.CommandTypeEnum.adCmdText, obj);
                    }
                    GTCustomCommandModeless.m_oGTTransactionManager.Commit();
                }

                ////Delete Stump Component                
                //if ((lbl_fno.Text == "11800") && (Stump_Cnt > 0 || Spare_Cnt > 0))
                //{
                //    GTCustomCommandModeless.m_oGTTransactionManager.Begin("SpliceDelete");
                //    if (Stump_Cnt > 0)
                //    {
                //        Stump = Get_Value("Select count(*) as cnt from GC_SPLICE_CONNECT where G3E_FNO = " + lbl_fno.Text + " and G3E_FID = " + lbl_FID.Text + " and CORE_STATUS = 'FOMS'");
                //        if (Stump == "0")
                //        {
                //            string ID = Get_Value("SELECT G3E_ID from GC_FSPLSTP_S where g3e_fid IN (" + lbl_FID.Text + ") and CORE_TYPE = 'FOMS' ");
                //            sSql = "Delete from GC_FSPLSTP_S where g3e_id = " + ID;
                //            rsDel = m_IGTDataContext.Execute(sSql, out iRecordNum, (int)ADODB.CommandTypeEnum.adCmdText, obj);
                //            sSql = "Delete from GC_FSPLSTP_T where g3e_id = " + ID;
                //            rsDel = m_IGTDataContext.Execute(sSql, out iRecordNum, (int)ADODB.CommandTypeEnum.adCmdText, obj);
                //        }
                //    }

                //    if (Spare_Cnt > 0)
                //    {
                //        Spare = Get_Value("Select count(*) as cnt from GC_SPLICE_CONNECT where G3E_FNO = " + lbl_fno.Text + " and G3E_FID = " + lbl_FID.Text + " and CORE_STATUS = 'SPARE'");
                //        if (Spare == "0")
                //        {
                //            string ID = Get_Value("SELECT G3E_ID from GC_FSPLSTP_S where g3e_fid IN (" + lbl_FID.Text + ") and CORE_TYPE = 'SPARE' ");
                //            sSql = "Delete from GC_FSPLSTP_S where g3e_id = " + ID;
                //            rsDel = m_IGTDataContext.Execute(sSql, out iRecordNum, (int)ADODB.CommandTypeEnum.adCmdText, obj);
                //            sSql = "Delete from GC_FSPLSTP_T where g3e_id = " + ID;
                //            rsDel = m_IGTDataContext.Execute(sSql, out iRecordNum, (int)ADODB.CommandTypeEnum.adCmdText, obj);
                //        }
                //    }
                //    GTCustomCommandModeless.m_oGTTransactionManager.Commit();
                //}                

                // MessageBox.Show("Core Count Disconnected.", "SpliceConnect", MessageBoxButtons.OK, MessageBoxIcon.Information);
                lbl_MSG.Text = "Splice Connection Disconnected !";
                GTClassFactory.Create<IGTApplication>().SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Core Count.");
                
                //Get_Port_Size(GetComboValue(cboFrom, cboFrom.SelectedItem.ToString()));
                Select_Splice();
                //Display the Available Core
                Get_Available_Core(lbl_FID.Text, lbl_fno.Text);

            }            
            catch (Exception e)
            {
                GTCustomCommandModeless.m_oGTTransactionManager.Rollback();
                MessageBox.Show(e.Message);
            }
        }

        private void Clear_Grid()
        {
            gv_Route.Rows.Clear();            
        }

        //Get Value from Database
        private string Get_Value(string sSql)
        {
            try
            {
                ADODB.Recordset rsPP = new ADODB.Recordset();
                rsPP = m_IGTDataContext.OpenRecordset(sSql, ADODB.CursorTypeEnum.adOpenStatic, ADODB.LockTypeEnum.adLockReadOnly, 1);
                if (rsPP.RecordCount > 0)
                {
                    rsPP.MoveFirst();
                    return (rsPP.Fields[0].Value.ToString());
                }
                return null;
            }
            catch (Exception ex)
            {
                log.WriteLog("");
                log.WriteLog(this.GetType().FullName, "ERROR: " + ex.Message);
                log.WriteLog(ex.StackTrace);
                MessageBox.Show(ex.Message, "Core Count", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }

        }

        public static List<IGTPoint> GetXY(short FNO, int FID, short iComp)
        {
            IGTKeyObject oFeature;
            IGTComponent oComp;
            Object oGeom;

            List<IGTPoint> PointCol = new List<IGTPoint>();
            oFeature = GTClassFactory.Create<IGTApplication>().DataContext.OpenFeature(FNO, FID);

            oComp = oFeature.Components.GetComponent(iComp);
            oGeom = oComp.Geometry;

            if (oGeom != null)
            {
                switch (oGeom.ToString())
                {
                    case "Intergraph.GTechnology.API.GTOrientedPointGeometry":
                        PointCol.Add(((IGTOrientedPointGeometry)oGeom).Origin);
                        break;
                    case "Intergraph.GTechnology.API.GTCompositePolylineGeometry":
                        PointCol.Add(((IGTCompositePolylineGeometry)oGeom).LastPoint);
                        break;
                    case "Intergraph.GTechnology.API.GTPolylineGeometry":
                        PointCol.Add(((IGTPolylineGeometry)oGeom).LastPoint);
                        break;
                }
            }
            return PointCol;
        }


        private void btn_Close_Click(object sender, EventArgs e)
        {
            //DialogResult retVal = MessageBox.Show("Are you sure to exit?","SpliceConnect", MessageBoxButtons.YesNo);
            //if (retVal == DialogResult.Yes)
            //{
            //GTClassFactory.Create<IGTApplication>().EndWaitCursor();
            Close();
            Dispose();
            //}
        }
        private string BtnSelectVal = "Check All";
        private void BtnSelect_Click(object sender, EventArgs e)
        {
            if (BtnSelectVal == "Check All")
            {
                BtnSelectVal = "Uncheck All";
                for (int i = 0; i < gv_Route.RowCount; i++)
                {
                    gv_Route.Rows[i].Cells[2].Value = true;
                }
            }
            else
            {
                BtnSelectVal = "Check All";
                for (int i = 0; i < gv_Route.RowCount; i++)
                {
                    gv_Route.Rows[i].Cells[2].Value = false;
                }
            }
        }

        private void LoadCMB()
        {
            try
            {
                string MINAttr = "MIN_MATERIAL";
                string MINAttrDesc = "ENGRG_PART_NO";
                string RefTab = "";
                String sSql = null;
                int recordsAffected = 0;

                RefTab = "REF_FCBL";
                sSql = "select distinct " + MINAttr + "," + MINAttrDesc + " from " + RefTab + " ORDER BY " + MINAttr + " ASC";
                Recordset rsComp = GTClassFactory.Create<IGTApplication>().DataContext.Execute(sSql, out recordsAffected, (int)CommandTypeEnum.adCmdText, null);
                if (rsComp != null)
                {
                    if (!rsComp.EOF)
                    {
                        //cboCblMIN.Items.Clear();
                        //cboCblMIN.DropDownWidth = 400;
                        while (!rsComp.EOF)
                        {
                            //cboCblMIN.Items.Add(new cboItem(rsComp.Fields[MINAttrDesc].Value.ToString(), rsComp.Fields[MINAttr].Value.ToString()));
                            rsComp.MoveNext();
                        }
                    }
                }
                rsComp = null;
            }
            catch (Exception ex)
            {
                log.WriteLog("");
                log.WriteLog(this.GetType().FullName, "ERROR: " + ex.Message);
                log.WriteLog(ex.StackTrace);
                MessageBox.Show(ex.Message, "Core Count", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private string GetComboValue(ComboBox cboList, string SelectValue)
        {
            for (int i = 0; i < cboList.Items.Count; i++)
            {
                if (SelectValue == ((cboItem)cboList.Items[i]).Name)
                {
                    return ((cboItem)cboList.Items[i]).Value;
                }
            }
            return "";
        }

        private void SetComboValue(ComboBox cboList, string SelectValue)
        {
            for (int i = 0; i < cboList.Items.Count; i++)
            {
                if (SelectValue == ((cboItem)cboList.Items[i]).Name )
                {
                    cboList.SelectedIndex = i;
                }
                if (SelectValue == DBNull.Value.ToString())
                {
                    cboList.SelectedIndex = -1;
                    
                }
            }

        }
        private void Get_Port_Size(string fid, Label port)
        {
            int j = 0;
            int recordsAffected = 0;
            string[] arCore = new string[2];
            char[] splitter = { ':' };
            string CFNO = ""; 

            string sSql = "SELECT MIN_MATERIAL, G3E_FNO FROM GC_NETELEM WHERE G3E_FID =" + fid.ToString() + "";
            Recordset rsComp = GTClassFactory.Create<IGTApplication>().DataContext.Execute(sSql, out recordsAffected, (int)CommandTypeEnum.adCmdText, null);

            if (rsComp.Fields[1].Value.ToString() == "12300")
            {
                //string sSql1 = "SELECT MAX(IW_ORDINAL) as Port FROM IW_FIBERPORT WHERE IW_MATERIAL IN ('" + rsComp.Fields[0].Value.ToString() + "')";
                string sSql1 = "SELECT SPLITTER_TYPE as Port FROM GC_FSPLITTER WHERE g3e_fid  IN ('" + fid + "')";
                Recordset rsComp1 = GTClassFactory.Create<IGTApplication>().DataContext.Execute(sSql1, out recordsAffected, (int)CommandTypeEnum.adCmdText, null);
                if (rsComp1.RecordCount > 0)
                {
                    if (rsComp1.Fields[0].Value.ToString() != "")
                    {
                        if (cboFrom.Text.Trim() != "")
                         CFNO = Get_Value("Select g3e_fno from gc_netelem where g3e_fid = " +  GetComboValue(cboFrom, cboFrom.SelectedItem.ToString()));

                        if (CFNO == "7200")
                        {
                            arCore = rsComp1.Fields[0].Value.ToString().Split(splitter);
                            port.Text = arCore[0].ToString();
                        }
                        else if (CFNO == "7400")
                        {
                            arCore = rsComp1.Fields[0].Value.ToString().Split(splitter);
                            port.Text = arCore[1].ToString();
                        }
                        else
                        {
                            arCore = rsComp1.Fields[0].Value.ToString().Split(splitter);
                            port.Text = arCore[0].ToString();
                        }

                    }
                }
                rsComp1 = null;
            }
            else if (rsComp.Fields[1].Value.ToString() == "5500" || rsComp.Fields[1].Value.ToString() == "4900" || rsComp.Fields[1].Value.ToString() == "12200" || rsComp.Fields[1].Value.ToString() == "9100" || rsComp.Fields[1].Value.ToString() == "9600" || rsComp.Fields[1].Value.ToString() == "5800")
            {
                //string sSql1 = "SELECT MAX(IW_ORDINAL) as Port FROM IW_FIBERPORT WHERE IW_MATERIAL IN ('" + rsComp.Fields[0].Value.ToString() + "')";
                //Recordset rsComp1 = GTClassFactory.Create<IGTApplication>().DataContext.Execute(sSql1, out recordsAffected, (int)CommandTypeEnum.adCmdText, null);
                //if (rsComp1.RecordCount > 0)
                //{
                //  if (rsComp1.Fields[0].Value.ToString() != "")
                //  {
                //    port.Text = rsComp1.Fields[0].Value.ToString();
                //  }
                //}
                //rsComp1 = null;
            }
            else if (rsComp.Fields[1].Value.ToString() == "7200"  )
            {
                port.Text = Get_Value("Select EFF_CORE from GC_FCBL where g3e_fid = " + fid.ToString());

                //string sSql1 = null;                
                //sSql1 = "SELECT CABLE_SIZE as Port FROM REF_FCBL WHERE MIN_MATERIAL IN ('" + rsComp.Fields[0].Value.ToString() + "')";
               
                //Recordset rsComp1 = GTClassFactory.Create<IGTApplication>().DataContext.Execute(sSql1, out recordsAffected, (int)CommandTypeEnum.adCmdText, null);
                //if (rsComp1.RecordCount > 0)
                //{
                //    if (rsComp1.Fields[0].Value.ToString() != "")
                //    {
                //        port.Text = rsComp1.Fields[0].Value.ToString();
                //    }
                //}
                //rsComp1 = null;

            }
            else if (rsComp.Fields[1].Value.ToString() == "7400" )
            {
                port.Text = Get_Value("Select CORE_NUM from GC_FDCBL where g3e_fid = " + fid.ToString());
            }
            else if (rsComp.Fields[1].Value.ToString() == "4400" )
            {
                port.Text = Get_Value("Select EFF_CORE from GC_FCBL_TRUNK where g3e_fid = " + fid.ToString());
            }
            else if (rsComp.Fields[1].Value.ToString() == "4500")
            {
                port.Text = Get_Value("Select EFF_CORE from GC_FCBL_JNT where g3e_fid = " + fid.ToString());
            }

                     
        }

        private void Get_Available_Core(string fid, string fno)
        {
            int j = 0;
            string core = "";
            int recordsAffected = 0;
                        
            if (fno != "11800" && fno != "5500" && fno != "4900" && fno != "12200")
            {
                bool CoreExists = false;
                bool WentInside = false;
                string sSql1 = null;
                string CABLE_FID = null;
                string CABLE_FNO = null;
                string CABLE_CODE = null;
                string PCABLE_CODE = null;
                string PCABLE_FID = null;
                int maxNum = 0;
                string tCode = null;

                lst_Core.Items.Clear();
                string C_FNO = Get_Value("select g3e_fno from gc_nr_connect where OUT_FID = " + fid);

                if (C_FNO == "7400")
                {
                    string SSQL = "select IN_FNO, IN_FID, G3E_FNO, G3E_FID, OUT_FNO, OUT_FID  from GC_NR_CONNECT  start with out_fid = " + fid + " connect by nocycle prior in_fid = out_fid and G3E_FNO in (7200,7400, 4400,4500)";
                    Recordset rsC = GTClassFactory.Create<IGTApplication>().DataContext.Execute(SSQL, out recordsAffected, (int)CommandTypeEnum.adCmdText, null);
                    if (rsC.RecordCount > 0)
                    {
                        do
                        {
                            if (rsC.Fields["IN_FNO"].Value.ToString() == "5100") //Stop at first FDC
                            {
                                CABLE_FID = rsC.Fields["G3E_FID"].Value.ToString();
                                CABLE_FNO = rsC.Fields["G3E_FNO"].Value.ToString();
                                break;
                            }
                            rsC.MoveNext();
                        }
                        while (!rsC.EOF);
                    }
                    rsC.Close();
                }
                else
                {
                    string SSQL = "select IN_FNO, IN_FID, G3E_FNO, G3E_FID, OUT_FNO, OUT_FID  from GC_NR_CONNECT  start with out_fid = " + fid + " connect by nocycle prior in_fid = out_fid and G3E_FNO in (7200,7400, 4400,4500)";
                    Recordset rsC = GTClassFactory.Create<IGTApplication>().DataContext.Execute(SSQL, out recordsAffected, (int)CommandTypeEnum.adCmdText, null);
                    if (rsC.RecordCount > 0)
                    {                     
                        do
                        {
                            if (rsC.Fields["IN_FNO"].Value.ToString() == "5500") //Stop at first ODF
                            {
                                CABLE_FID = rsC.Fields["G3E_FID"].Value.ToString();
                                CABLE_FNO = rsC.Fields["G3E_FNO"].Value.ToString();
                                break;
                            }
                            rsC.MoveNext();
                        }
                        while (!rsC.EOF);
                    }
                    rsC.Close();
                }
                if (CABLE_FNO != null || CABLE_FID != null)
                {
                    if (CABLE_FNO == "7200")
                    {
                        CABLE_CODE = Get_Value("select CABLE_CODE from GC_FCBL where g3e_fid = " + CABLE_FID);                        
                    }
                    else if (CABLE_FNO == "7400")
                    {
                        CABLE_CODE = Get_Value("select FCABLE_CODE from GC_FDCBL where g3e_fid = " + CABLE_FID);
                    }
                    else if (CABLE_FNO == "4400")
                    {
                        CABLE_CODE = Get_Value("select CABLE_CODE from GC_FCBL_TRUNK where g3e_fid = " + CABLE_FID);
                    }
                    else if (CABLE_FNO == "4500")
                    {
                        CABLE_CODE = Get_Value("select CABLE_CODE from GC_FCBL_JNT where g3e_fid = " + CABLE_FID);
                    }

                    sSql1 = "SELECT CABLE_SIZE as CORE FROM REF_FCBL WHERE MIN_MATERIAL IN (SELECT MIN_MATERIAL FROM GC_NETELEM WHERE G3E_FID = '" + CABLE_FID + "')";                                       

                    lbl_CoreFID.Text = "M: " + CABLE_CODE + " - " + CABLE_FID;
                    lbl_Core_FID.Text = CABLE_FID;

                    //Protection only for 7200
                    if (CABLE_FNO == "7200")
                    {
                        maxNum = Convert.ToInt32(CABLE_CODE.Substring(1,2)) + 1;
                        if (maxNum.ToString().Length == 1) tCode = "0" + maxNum.ToString();
                        else if (maxNum.ToString().Length == 2) tCode = maxNum.ToString();

                        PCABLE_CODE =  "F" + tCode;
                        PCABLE_FID = Get_Value("SELECT G3E_FID FROM GC_FCBL WHERE CABLE_CODE = '" + PCABLE_CODE + "'");
                        lbl_CorePFID.Text = "P: " + PCABLE_CODE + " - " + PCABLE_FID;
                        lbl_CoreP_FID.Text = PCABLE_FID;
                    }
                    else
                    {
                        lbl_CorePFID.Visible = false;
                    }

                    Recordset rsComp1 = GTClassFactory.Create<IGTApplication>().DataContext.Execute(sSql1, out recordsAffected, (int)CommandTypeEnum.adCmdText, null);
                    if (rsComp1.RecordCount > 0)
                    {
                        if (rsComp1.Fields[0].Value.ToString() != "")
                        {
                            for (int i = 1; i <= Convert.ToInt32(rsComp1.Fields[0].Value.ToString()); i++)
                            {
                                CoreExists = false;
                                string sLow = "SELECT TERM_LOW, TERM_HIGH FROM gc_splice_connect WHERE  TERM_FID =" + CABLE_FID + " AND TERM_FNO = " + CABLE_FNO + "  and CORE_STATUS is NOT NULL order by TERM_LOW";
                                Recordset rsLow = GTClassFactory.Create<IGTApplication>().DataContext.Execute(sLow, out recordsAffected, (int)CommandTypeEnum.adCmdText, null);
                                if (rsLow.RecordCount > 0)
                                {
                                    WentInside = true;
                                    do
                                    {
                                        if (rsLow.Fields[0].Value.ToString() != "" && rsLow.Fields[1].Value.ToString() != "")
                                        {
                                            int Low = Convert.ToInt32(rsLow.Fields[0].Value.ToString());
                                            int High = Convert.ToInt32(rsLow.Fields[1].Value.ToString());
                                            if (i >= Low && i <= High)
                                            {
                                                CoreExists = true;
                                            }
                                        }
                                        rsLow.MoveNext();
                                    }
                                    while (!rsLow.EOF);
                                    rsLow.Close();
                                }

                                if (CoreExists == false)
                                {
                                    if (i.ToString().Length == 2)
                                        core = core + i.ToString() + "   ";
                                    else
                                        core = core + i.ToString() + "     ";
                                    j = j + 1;
                                    if (j == 5)
                                    {
                                        j = 0;
                                        lst_Core.Items.Add(core);
                                        core = "";
                                    }
                                }
                            }
                            if (lst_Core.Items.Count == 0 && WentInside == false)
                            {
                                j = 0;
                                for (int i = 1; i <= Convert.ToInt32(rsComp1.Fields[0].Value.ToString()); i++)
                                {
                                    if (i.ToString().Length == 2)
                                        core = core + i.ToString() + "   ";
                                    else
                                        core = core + i.ToString() + "     ";
                                    j = j + 1;
                                    if (j == 5)
                                    {
                                        j = 0;
                                        lst_Core.Items.Add(core);
                                        core = "";
                                    }
                                }
                                lst_Core.Items.Add(core);
                                grp_Core.Visible = true;
                            }
                            else
                            {
                                lst_Core.Items.Add(core);
                                grp_Core.Visible = true;
                            }
                        }
                    }
                    else
                    {
                        //MessageBox.Show("No Plant Unit for Core Cable. Unable to display the Maximum Available Core. \nCore Cable : " + lbl_CoreFID.Text, "Core Count", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        lbl_CoreMSG.Visible = true;
                        lbl_CoreMSG.Text = "No Plant Unit for Core Cable. \nUnable to display the Maximum Available Core. \nCore Cable : " + lbl_CoreFID.Text;
                        return;
                    }
                    rsComp1 = null;
                }
                else
                {
                    lbl_CoreMSG.Visible = true;
                    if (C_FNO == "7400")                    
                        lbl_CoreMSG.Text = "Unable to find the Core Cable connected to FDC. \nConnectivity Missing.";
                    else
                        lbl_CoreMSG.Text = "Unable to find the Core Cable connected to ODF. \nConnectivity Missing.";

                    return;
                }
            }
        }

        private void cboFrom_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboFrom.Text.Trim() != "")
            {
                Get_Port_Size(GetComboValue(cboFrom, cboFrom.SelectedItem.ToString()), lbl_FromCore);
                if (cboTo.Text.Trim() != "")
                    Get_Port_Size(GetComboValue(cboTo, cboTo.SelectedItem.ToString()), lbl_ToCore);
                Select_Splice();
                string FID = GetComboValue(cboFrom, cboFrom.SelectedItem.ToString());
                string FNO = Get_Value("Select g3e_fno from gc_netelem where g3e_fid = " + FID);
                Highlight(Convert.ToInt16(FNO),Convert.ToInt32(FID));
            }
            else
            {
                Select_Splice();
                //Clear_Grid();
                //grp_Core.Visible = false;
            }
        }

        private void cboTo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboTo.Text.Trim() != "")
            {
                Get_Port_Size(GetComboValue(cboTo, cboTo.SelectedItem.ToString()), lbl_ToCore);
                Select_Splice();
                string FID = GetComboValue(cboTo, cboTo.SelectedItem.ToString());
                string FNO = Get_Value("Select g3e_fno from gc_netelem where g3e_fid = " + FID);
                Highlight(Convert.ToInt16(FNO), Convert.ToInt32(FID));
            }
            else
            {
                Select_Splice();
                //Clear_Grid();
                //grp_Port.Visible = false;
            }

            //string FNO = null;
            //if (GetComboValue(cboTo, cboTo.Text) != " ")
            //    FNO = Get_Value("SELECT G3E_FNO from GC_NETELEM where G3E_FID = " + GetComboValue(cboTo, cboTo.Text));

            //if (FNO == "5500")
            //{
            //    lblPP.Visible = true;
            //    cboPP.Visible = true;

            //    cboPP.Items.Clear();
            //    //cboPP.Items.Add(new cboItem(" ", " "));

            //    string HORZ_NUM = Get_Value("SELECT HORZ_NUM from GC_ODF where G3E_FID = " + lbl_FID.Text);

            //    if (HORZ_NUM != "")
            //    {
            //        for (int i = 1; i <= Convert.ToInt32(HORZ_NUM); i++)
            //            cboPP.Items.Add(new cboItem(i.ToString(), i.ToString()));
            //    }
            //}
            //else
            //{
            //    cboPP.Items.Clear();
            //    lblPP.Visible = false;
            //    cboPP.Visible = false;                
            //}
        }

        private void btn_Close_Click_1(object sender, EventArgs e)
        {
            Close();
            Dispose();
        }

        private void btn_Clear_Click(object sender, EventArgs e)
        {
            All_Clear_Clear();
            Select_Splice();
            btn_Connect.Text = "Connect";
            lstPPWO.Items.Clear();
            
        }

        
    }
        
      
}