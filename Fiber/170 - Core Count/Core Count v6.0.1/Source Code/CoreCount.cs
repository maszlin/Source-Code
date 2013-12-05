using System;
using System.Collections;
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
        public const string C_PROG_NAME = "Core Count";
        public const string C_KEY_FR_ODF = "From ODF";
        public const string C_KEY_TO_ODF = "To ODF";
        public const string C_SPARE = "SPARE";
        public const string C_STUMP = "STUMP";
        public const string C_MAIN = "MAIN";
        public const string C_PROCTECTION = "PROTECTION";
        public const string C_FOMS = "FOMS";
        public const string C_DISTRIBUTE = "DISTRIBUTE";
        //2nd March 2013 - Catherine Add FOMS MAIN, FOMS PROTECTION;
        public const string C_FOMS_MAIN = "FOMS M";
        public const string C_FOMS_PROTECTION = "FOMS P";

        public const string C_TAB_ESIDE = "TabESide";
        public const string C_TAB_DSIDE = "TabDSide";
        public const string C_TAB_ODF = "TabODF";
        public const string C_TAB_OVERALL_CORE = "TabOverallCore";
        public const string C_TAB_OVERALL_PORT = "TabOverallPort";
        public const string C_TAB_OVERALL_ODF = "TabOverallODF";

        public const string C_FOMS_FDP_TYPE = "FOMS";
        public string gbl_CURR_TAB = C_TAB_ESIDE;

        string E_ACTION_MODE = C_DISTRIBUTE;
        string D_ACTION_MODE = C_DISTRIBUTE;
        static IGTApplication m_GeoApp;

        public TabPage[] MyTabPg = null;

        private Logger log;
        private Intergraph.GTechnology.API.IGTApplication m_gtapp = null;

        IGTDDCKeyObjects oGTKeyObjs;

        //GLobals

        private const short FECBL_FNO = 7200;      // Fibre E-Side Cable
        private const short FDCBL_FNO = 7400;     // Fibre D-Side Cable
        private const short TRCBL_FNO = 4400;     // Trunk Cable
        private const short JNCBL_FNO = 4500;     // Junction Cable

        private const short FDC_FNO = 5100;       // FDC
        private const short UPE_FNO = 5400;       // UPE

        private const short FDP_FNO = 5600;       // FDP
        private const short FTB_FNO = 5900;       // FTB
        private const short TIE_FNO = 5800;       // TIE
        private const short DB_FNO = 9900;        // DB

        private const short MSAN_FNO = 9100;      // MSAN
        private const short DDN_FNO = 9300;       // DDN
        private const short NDH_FNO = 9400;       // NDH
        private const short MUX_FNO = 9500;       // MUX
        private const short EPE_FNO = 5200;       // EPE
        private const short FAN_FNO = 9700;       // FAN
        private const short RT_FNO = 9600;        // RT
        private const short VDSL_FNO = 9800;      // VDSL2

        //Detail Window
        private const short ODF_FNO = 5500;       // ODF
        private const short FPATCH_FNO = 4900;    // FPATCH
        private const short FPP_FNO = 12200;      // Fibre Patch Panel or FTB AC

        //SPLICE AND JOINTS
        private const short FSLICE_FNO = 11800;      // Fibre Splice Enclosure
        private const short FST_FNO = 4600;       // Fiber Splice Trunk
        private const short FSJ_FNO = 4700;       // Fiber Splice Junction

        private const short SHELF_FNO = 15800;    // Fiber Shelf
        private const short SPLITTER_FNO = 12300; // Fiber Splitter
        private const short CARD_FNO = 15900;     // Fiber Card

        private int edit_g3e_id = 0;

        bool bSelectedFeatureValid = false;

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

                //Backup the Current Tab Page First
                

                LoadForm();
                

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "SpliceConnect", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        

        private void btn_ChangeDevice_Click(object sender, EventArgs e)
        {
            ClearFormSelection();
            LoadForm();
            EmptyForm();
        }
        private void EmptyForm()
        {
            if (gbl_CURR_TAB == C_TAB_ESIDE)
            {
                //cboFrom.Items.Clear();
                //cboTo.Items.Clear();
                if (cboFrom.Items.Count > 0) cboFrom.SelectedText = "";
                if (cboTo.Items.Count > 0) cboTo.SelectedText = "";
                LstInUnAssRngCore.Items.Clear();
                LstOutUnAssRngCore.Items.Clear();
                txtLOW1.Text = "";
                txtHIGH1.Text = "";
                txtLOW2.Text = "";
                txtHIGH2.Text = "";
                lbl_FromCore.Text = "";
                lbl_ToCore.Text = "";
                grpMainProtection.Enabled = true;
                grpEAssTo.Enabled = true;
                rBtnDistributeE.Checked = true;
                rBtnMain.Checked = true;
                btn_Assign.Enabled = false;
            }
            else if (gbl_CURR_TAB == C_TAB_DSIDE)
            {
                //cboDFrom.Items.Clear();
                //cboDTo.Items.Clear();
                if (cboDFrom.Items.Count > 0) cboDFrom.SelectedText = "";
                if (cboDTo.Items.Count > 0) cboDTo.SelectedText = "";
                LstInUnAssRngPort.Items.Clear();
                LstOutUnAssRngPort.Items.Clear();
                txtDLOW1.Text = "";
                txtDHIGH1.Text = "";
                txtDLOW2.Text = "";
                txtDHIGH2.Text = "";
                lbl_FromPort.Text = "";
                lbl_ToPort.Text = "";
                rBtnDistributeD.Checked = true;
                grpDAssTo.Enabled = true;
                btn_DAssign.Enabled = false;
                
                //rBtnSetSpareD.Enabled = true;
                //rBtnSetStumpD.Enabled = true;
                //grpMainFOMS.Enabled = false;
                //grpMainFOMS.Visible = false;
            }
          

        }

        private void ClearFormSelection()
        {
            Font font;
            
            Clear_Grid();
            E_ACTION_MODE = C_DISTRIBUTE;
            D_ACTION_MODE = C_DISTRIBUTE;
            font = new Font("Arial Black", 35);
            lbl_Symbol.Font = font;
            lbl_Symbol.Text = "?";
            lbl_Exch.Text = "";
            lbl_DeviceName.Text = "";
            lbl_Device.Text = "";
            lbl_FID.Text = "";
            lbl_fno.Text = "";
        }

        private void LoadAllTabPage()
        {
            try
            {
                if (MyTabPg == null) // if MyTabPg is empty then reload the tab
                {
                    MyTabPg = new TabPage[tabCoreCount.TabPages.Count];
                    for (int i = 0; i < tabCoreCount.TabPages.Count; i++)
                    {                        
                        //ShowTabPage(tabCoreCount.TabPages[i]);
                        MyTabPg[i] = tabCoreCount.TabPages[i];
                    }
                }
                else
                {
                    //Remove Tabpage  
                    //tabCoreCount.TabPages.Clear();
                    //Hide all
                    //foreach (TabPage tb_tmp in tabCoreCount.TabPages)
                    //{
                    //    HideTabPage(tb_tmp);
                    //}
                    for (int i = 0; i < MyTabPg.Length; i++)
                    {
                        if (MyTabPg[i] != null) ShowTabPage(MyTabPg[i], i+1);
                    }
                }
            } catch (Exception Ex){
                MessageBox.Show(Ex.Message);
            }
        }
        private void LoadForm()
        {
            try
            {
                
                bSelectedFeatureValid = Select_Feature();
                if (bSelectedFeatureValid)
                {

                    if (lbl_fno.Text == FSLICE_FNO.ToString())
                    {
                        button4.Image = button3.Image;
                        //label2.Text = "Cable :";
                        //label6.Text = "Low Core";
                        //label5.Text = "High Core";
                    }

                    DisplayDefaultTabLayout();
                    Load_Combo();
                    //Select_Splice();
                    FillDataGrid();

                }
                else
                {
                    //if not valid clear the Form
                    ClearFormSelection();
                    EmptyForm();
                    if (gbl_CURR_TAB == C_TAB_DSIDE)
                    {
                        cboDFrom.Items.Clear();
                        cboDTo.Items.Clear();
                    }
                    else if (gbl_CURR_TAB == C_TAB_ESIDE)
                    {
                        cboFrom.Items.Clear();
                        cboTo.Items.Clear();
                    }
                    else if (gbl_CURR_TAB == C_TAB_ODF)
                    {
                        EnableOrDisableODFAssignButton();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "SpliceConnect", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DisplayDefaultTabLayout()
        {
            LoadAllTabPage();
            //Set the TAB
            //Hide the grpMainProtect if select feature is not FDC
            switch (Convert.ToInt32(lbl_fno.Text.Trim()))
            {
                case FDC_FNO:

                    //E-Side & D-Side Assignment Tab
                    grpMainProtection.Visible = true;
                    grpMainProtection.Enabled = true;
                    rBtnMain.Checked = true;
                    tabCoreCount.SelectTab(C_TAB_ESIDE);
                    HideTabPage(tabCoreCount.TabPages[C_TAB_OVERALL_ODF]);
                    HideTabPage(tabCoreCount.TabPages[C_TAB_ODF]);
                    gbl_CURR_TAB = C_TAB_ESIDE;
                    //Disable Stump Option
                    rBtnSetStumpE.Enabled = false;

                    //Enable Spare and Stump & Disable FOMS Option
                    rBtnSetSpareD.Enabled = true;
                    rBtnSetStumpD.Enabled = true;
                    rBtnDistributeD.Enabled = true;
                    rBtnDistributeD.Checked = true;
                    //rBtnFOMSD.Enabled = false;
                    
                    break;
                case MSAN_FNO:
                case VDSL_FNO:
                case MUX_FNO:
                case RT_FNO:                
                    grpMainProtection.Visible = true;
                    grpMainProtection.Enabled = true;
                    rBtnMain.Checked = true;
                    tabCoreCount.SelectTab(C_TAB_ESIDE);
                    //Only E-Side Assignment Tab
                    HideTabPage(tabCoreCount.TabPages[C_TAB_DSIDE]);
                    HideTabPage(tabCoreCount.TabPages[C_TAB_OVERALL_PORT]);
                    HideTabPage(tabCoreCount.TabPages[C_TAB_OVERALL_ODF]);
                    HideTabPage(tabCoreCount.TabPages[C_TAB_ODF]);
                    gbl_CURR_TAB = C_TAB_ESIDE;
                    //Disable Stump Option
                    rBtnSetStumpE.Enabled = false;
                    
                    break;
                case FDP_FNO:
                    //Check if is FOMS FDP type
                    //grpMainFOMS.Visible = IsFOMSFDP(lbl_fno.Text.Trim(), lbl_FID.Text.Trim());
                    //grpMainFOMS.Enabled = IsFOMSFDP(lbl_fno.Text.Trim(), lbl_FID.Text.Trim());

                    //Only D-Side Assignment Tab
                    HideTabPage(tabCoreCount.TabPages[C_TAB_ESIDE]);
                    HideTabPage(tabCoreCount.TabPages[C_TAB_OVERALL_CORE]);
                    HideTabPage(tabCoreCount.TabPages[C_TAB_OVERALL_ODF]);
                    HideTabPage(tabCoreCount.TabPages[C_TAB_ODF]);
                    tabCoreCount.SelectTab(C_TAB_DSIDE);
                    gbl_CURR_TAB = C_TAB_DSIDE;

                    //Disable the radio button for spare/stump
                    rBtnDistributeD.Enabled = true;
                    rBtnDistributeD.Checked = true;
                    rBtnSetSpareD.Enabled = false;
                    rBtnSetStumpD.Enabled = false;
                    //rBtnFOMSD.Enabled = false;
                    //if (IsFOMSFDP(lbl_fno.Text.Trim(), lbl_FID.Text.Trim()))
                    //{
                    //    rBtnDistributeD.Enabled = false;
                    //    rBtnDistributeD.Checked = false;
                    //    rBtnFOMSD.Enabled = true;
                    //    rBtnFOMSD.Checked = true;
                    //}
                    break;
                case FTB_FNO:
                case TIE_FNO:
                case DB_FNO:
                    //Only D-Side Assignment Tab
                    //grpMainFOMS.Visible = false;
                    //grpMainFOMS.Visible = false;
                    HideTabPage(tabCoreCount.TabPages[C_TAB_ESIDE]);
                    HideTabPage(tabCoreCount.TabPages[C_TAB_OVERALL_CORE]);
                    HideTabPage(tabCoreCount.TabPages[C_TAB_OVERALL_ODF]);
                    HideTabPage(tabCoreCount.TabPages[C_TAB_ODF]);
                    tabCoreCount.SelectTab(C_TAB_DSIDE);
                    gbl_CURR_TAB = C_TAB_DSIDE;

                    //Disable the radio button for spare/stump
                    rBtnSetSpareD.Enabled = false;
                    rBtnSetStumpD.Enabled = false;
                    //rBtnFOMSD.Enabled = false;
                    break;
                case FSLICE_FNO:
                    // Determine if E-Side Network or D-Side Network
                    switch (getConnectivityType(lbl_FID.Text.Trim()))
                    {
                        case FECBL_FNO:
                            //Only E-Side Assignment Tab
                            HideTabPage(tabCoreCount.TabPages[C_TAB_DSIDE]);
                            HideTabPage(tabCoreCount.TabPages[C_TAB_OVERALL_PORT]);
                            HideTabPage(tabCoreCount.TabPages[C_TAB_OVERALL_ODF]);
                            HideTabPage(tabCoreCount.TabPages[C_TAB_ODF]);
                            tabCoreCount.SelectTab(C_TAB_ESIDE);
                            grpMainProtection.Enabled = false;
                            grpMainProtection.Visible = false;
                            rBtnSetStumpE.Enabled = true;
                            rBtnSetSpareE.Enabled = true;
                            gbl_CURR_TAB = C_TAB_ESIDE;
                            break;
                        case FDCBL_FNO:
                            //Only D-Side Assignment Tab
                            HideTabPage(tabCoreCount.TabPages[C_TAB_ESIDE]);
                            HideTabPage(tabCoreCount.TabPages[C_TAB_OVERALL_CORE]);
                            HideTabPage(tabCoreCount.TabPages[C_TAB_OVERALL_ODF]);
                            HideTabPage(tabCoreCount.TabPages[C_TAB_ODF]);
                            //grpMainFOMS.Visible = false;
                            //grpMainFOMS.Enabled = false;
                            rBtnSetStumpD.Enabled = true;
                            rBtnSetSpareD.Enabled = true;
                            //rBtnFOMSD.Enabled = false;
                            tabCoreCount.SelectTab(C_TAB_DSIDE);
                            gbl_CURR_TAB = C_TAB_DSIDE;
                            break;
                    }

                    grpEAssTo.Visible = true;
                    grpEAssTo.Enabled = true;
                    break;
                case ODF_FNO:
                    //Only D-Side Assignment Tab
                    HideTabPage(tabCoreCount.TabPages[C_TAB_ESIDE]);
                    HideTabPage(tabCoreCount.TabPages[C_TAB_OVERALL_CORE]);
                    HideTabPage(tabCoreCount.TabPages[C_TAB_DSIDE]);
                    HideTabPage(tabCoreCount.TabPages[C_TAB_OVERALL_PORT]);
                    tabCoreCount.SelectTab(C_TAB_ODF);
                    gbl_CURR_TAB = C_TAB_ODF;

                    break;
                case FPATCH_FNO:
                case FPP_FNO:
                    //do something on these
                    break;
            }

        }


        private void HideTabPage(TabPage tp)
        {
            if (tabCoreCount.TabPages.Contains(tp))
                tabCoreCount.TabPages.Remove(tp);
        }


        private void ShowTabPage(TabPage tp)
        {
            ShowTabPage(tp, tabCoreCount.TabPages.Count);
        }


        private void ShowTabPage(TabPage tp, int index)
        {
            bool bFound = false;

            //Determine if specific tab name appear
            //for (int i = 0; i < tabCoreCount.TabPages.Count; i++)
            //{
            //    //if (tabCoreCount.TabPages.Contains(tp)) return;
            //    if (tabCoreCount.TabPages[i].Name.ToString().Trim() == tp.Name.ToString().Trim()) return;
            //}
            if (tabCoreCount.TabCount > 0)
            {
                foreach (TabPage TP_tmp in tabCoreCount.TabPages)
                {
                    if (TP_tmp.Name.ToString().Trim() == tp.Name.ToString().Trim()) return;
                }
            }
            if (!bFound)  InsertTabPage(tp, index);
        }


        private void InsertTabPage(TabPage tabpage, int index)
        {
           //if (index < 0 || index > tabCoreCount.TabCount)
            if (index < 0 || index > tabCoreCount.TabCount+1)
                throw new ArgumentException("Index out of Range.");
            tabCoreCount.TabPages.Add(tabpage);
            if (index-1 < tabCoreCount.TabCount - 1)
                do
                {
                    SwapTabPages(tabpage,
                        (tabCoreCount.TabPages[tabCoreCount.TabPages.IndexOf(tabpage) - 1]));
                }
                while (tabCoreCount.TabPages.IndexOf(tabpage) != index-1);
            tabCoreCount.SelectedTab = tabpage;
        }

        private void SwapTabPages(TabPage tp1, TabPage tp2)
        {
            if (tabCoreCount.TabPages.Contains(tp1) == false
                                           || tabCoreCount.TabPages.Contains(tp2) == false)
                throw new ArgumentException(
                                "TabPages must be in the TabControls TabPageCollection.");

            int Index1 = tabCoreCount.TabPages.IndexOf(tp1);
            int Index2 = tabCoreCount.TabPages.IndexOf(tp2);
            tabCoreCount.TabPages[Index1] = tp2;
            tabCoreCount.TabPages[Index2] = tp1;
        }

        private int getConnectivityType(string m_FID)
        {

            try
            {
                int iReturn = -1;
                ADODB.Recordset rs1 = new ADODB.Recordset();
                string sSql = "";
                sSql = "select distinct g3e_fno from gc_nr_connect where in_fid =" + m_FID + " or out_fid = " + m_FID;
                rs1 = m_IGTDataContext.OpenRecordset(sSql, ADODB.CursorTypeEnum.adOpenStatic, ADODB.LockTypeEnum.adLockReadOnly, 1);
                if (rs1.RecordCount > 0)
                {
                    rs1.MoveFirst();
                    while (!rs1.EOF)
                    {
                        iReturn = Convert.ToInt16(rs1.Fields[0].Value.ToString());
                        rs1.MoveNext();
                    }
                }

                rs1.Close();

                return iReturn;

            }
            catch (Exception ex)
            {
                return -1;
            }
        }

       
        private void GTWindowsForm_CoreCount_Shown(object sender, EventArgs e)
        {
            m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Running Core Count...");
            if (!bSelectedFeatureValid)
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

       
        //Fill the Datagrid
        private void FillDataGrid()
        {
            string gFNO1 = null;
            string gFID1 = null;
            string gFNO1_Desc = null;
            string gFID1_Desc = null;
            string gLOW1 = null;
            string gHIGH1 = null;
            string gFNO2 = null;
            string gFID2 = null;
            string gFNO2_Desc = null;
            string gFID2_Desc = null;
            string gLOW2 = null;
            string gHIGH2 = null;
            string gCID = null;
            string gG3E_ID = null;
            string ppwo = null;
            string sSql = null;
            string Condition = null;
            string Conn_Type = null;
            string EXC_ABB = null;
            string CABLE_CODE = null;
            string FEATURE_STATE = null;
            string TERM_FNO = null;
            string TERM_FID = null;
            string TERM_LOW = null;
            string TERM_HIGH = null;

            try
            {
                if (lbl_FID.Text != "")
                {
                    //GTClassFactory.Create<IGTApplication>().BeginWaitCursor();
                    ADODB.Recordset rs = new ADODB.Recordset();
                    Clear_Grid();
                    ////Temporarily do not load grid if select ODF
                    //if ((gbl_CURR_TAB == C_TAB_OVERALL_ODF)||(gbl_CURR_TAB == C_TAB_ODF) ) return;
                    
                    //Check if Current grid is on what tab
                    Condition = " WHERE g3e_fid = " + lbl_FID.Text;
                    if (gbl_CURR_TAB == C_TAB_DSIDE)
                    {
                        sSql = "SELECT SC.G3E_FNO, SC.G3E_FID, SC.FNO1, SC.FID1,SC.LOW1, SC.HIGH1, SC.FNO2, SC.FID2, SC.LOW2, SC.HIGH2, SC.G3E_CID, sc.g3e_id, SC.WORKORDER, SC.CORE_STATUS, " +
                               " SC.EXC_ABB, SC.CABLE_CODE, SC.FEATURE_STATE, SC.TERM_FNO, SC.TERM_FID, SC.TERM_LOW, SC.TERM_HIGH " +
                               " FROM gc_splice_connect SC " + Condition + " and  TERM_FNO in ( " + SPLITTER_FNO.ToString() +", " + FDC_FNO.ToString() + ") ORDER BY FID2, LOW2";


                    }
                    else if (gbl_CURR_TAB == C_TAB_ESIDE)
                    {
                        //sSql = "SELECT SC.G3E_FNO, SC.G3E_FID, SC.FNO1, SC.FID1,SC.LOW1, SC.HIGH1, SC.FNO2, SC.FID2, SC.LOW2, SC.HIGH2, SC.G3E_CID, sc.g3e_id, SC.WORKORDER, SC.CORE_STATUS, " +
                        //       " SC.EXC_ABB, SC.CABLE_CODE, SC.FEATURE_STATE, SC.TERM_FNO, SC.TERM_FID, SC.TERM_LOW, SC.TERM_HIGH " +
                        //       " FROM gc_splice_connect SC " + Condition + " and TERM_FNO = " + FECBL_FNO.ToString() + " ORDER BY FID2, LOW2";

                        sSql = "SELECT SC.G3E_FNO, SC.G3E_FID, SC.FNO1, SC.FID1,SC.LOW1, SC.HIGH1, SC.FNO2, SC.FID2, SC.LOW2, SC.HIGH2, SC.G3E_CID, sc.g3e_id, SC.WORKORDER, SC.CORE_STATUS, " +
                               " SC.EXC_ABB, SC.CABLE_CODE, SC.FEATURE_STATE, SC.TERM_FNO, SC.TERM_FID, SC.TERM_LOW, SC.TERM_HIGH " +
                               " FROM gc_splice_connect SC " + Condition + " and ( SC.FNO1 in ( " + FECBL_FNO.ToString() + ", " + TRCBL_FNO.ToString() + ", " + JNCBL_FNO.ToString() + ") or " +
                               " SC.FNO2 in ( " + FECBL_FNO.ToString() + ", " + TRCBL_FNO.ToString() + ", " + JNCBL_FNO.ToString() + "))" +
                               " ORDER BY FID2, LOW2";
                    }
                    else if (gbl_CURR_TAB == C_TAB_ODF)
                    {
                        //Always referesh the ODF available list box whenever refresh the grid
                        RefreshODFAvailCore();

                        sSql = "SELECT SC.G3E_FNO, SC.G3E_FID, SC.FNO1, SC.FID1,SC.LOW1, SC.HIGH1, SC.FNO2, SC.FID2, SC.LOW2, SC.HIGH2, SC.G3E_CID, sc.g3e_id, SC.WORKORDER, SC.CORE_STATUS, " +
                               " SC.EXC_ABB, SC.CABLE_CODE, SC.FEATURE_STATE, SC.TERM_FNO, SC.TERM_FID, SC.TERM_LOW, SC.TERM_HIGH " +
                               " FROM gc_splice_connect SC " + Condition + " ORDER BY FID2, LOW2";
                    }
                    if (sSql.Trim() == "") return; //Exit the function is not at E or D assignment tab.
                    rs = m_IGTDataContext.OpenRecordset(sSql, ADODB.CursorTypeEnum.adOpenStatic, ADODB.LockTypeEnum.adLockReadOnly, 1);
                    if (rs.RecordCount == 0) return; //Exit the function if no result found.
                    rs.MoveFirst(); // result found
                    do
                    {
                        gFNO1 = rs.Fields["FNO1"].Value.ToString();
                        gFID1 = rs.Fields["FID1"].Value.ToString();

                        gFNO1_Desc = GetFeatName(gFNO1);
                        gFID1_Desc = GetFeatCode(Convert.ToInt16(gFNO1), gFID1) + "(" + gFID1 + ")";

                        gLOW1 = rs.Fields[4].Value.ToString();
                        gHIGH1 = rs.Fields[5].Value.ToString();

                        gFNO2 = rs.Fields[6].Value.ToString();
                        gFID2 = rs.Fields[7].Value.ToString();

                        gFNO2_Desc = GetFeatName(gFNO2);
                        gFID2_Desc = GetFeatCode(Convert.ToInt16(gFNO2), gFID2) + "(" + gFID2 + ")";

                        gLOW2 = rs.Fields["LOW2"].Value.ToString();
                        gHIGH2 = rs.Fields["HIGH2"].Value.ToString();
                        gCID = rs.Fields["G3E_CID"].Value.ToString();
                        gG3E_ID = rs.Fields["G3E_ID"].Value.ToString();
                        ppwo = rs.Fields["WORKORDER"].Value.ToString();
                        Conn_Type = rs.Fields["CORE_STATUS"].Value.ToString();
                        EXC_ABB = rs.Fields["EXC_ABB"].Value.ToString();
                        //CABLE_CODE = rs.Fields["CABLE_CODE"].Value.ToString();
                        FEATURE_STATE = rs.Fields["FEATURE_STATE"].Value.ToString();
                        TERM_FNO = rs.Fields["TERM_FNO"].Value.ToString();
                        TERM_FID = rs.Fields["TERM_FID"].Value.ToString();
                        if (TERM_FNO.Trim() != "") CABLE_CODE = GetFeatName(TERM_FNO) + " - " + GetFeatCode(Convert.ToInt16(TERM_FNO), TERM_FID) + "(" + TERM_FID + ")";
                        TERM_LOW = rs.Fields["TERM_LOW"].Value.ToString();
                        TERM_HIGH = rs.Fields["TERM_HIGH"].Value.ToString();

                        //COLOR = "";
                        //if (gbl_CURR_TAB == C_TAB_ESIDE)
                        //{
                        //    if (cboFrom.Text.Trim() != "" && (gFID1 == GetComboValue(cboFrom, cboFrom.SelectedItem.ToString()) || gFID2 == GetComboValue(cboFrom, cboFrom.SelectedItem.ToString())))
                        //        COLOR = "YES";
                        //    if (cboTo.Text.Trim() != "" && (gFID2 == GetComboValue(cboTo, cboTo.SelectedItem.ToString()) || gFID1 == GetComboValue(cboTo, cboTo.SelectedItem.ToString())))
                        //        COLOR = "YES";
                        //}
                        //else if (gbl_CURR_TAB == C_TAB_DSIDE)
                        //{
                        //    if (cboDFrom.Text.Trim() != "" && (gFID1 == GetComboValue(cboDFrom, cboDFrom.SelectedItem.ToString()) || gFID2 == GetComboValue(cboDFrom, cboDFrom.SelectedItem.ToString())))
                        //        COLOR = "YES";
                        //    if (cboDTo.Text.Trim() != "" && (gFID2 == GetComboValue(cboDTo, cboDTo.SelectedItem.ToString()) || gFID1 == GetComboValue(cboDTo, cboDTo.SelectedItem.ToString())))
                        //        COLOR = "YES";
                        //}
                        //AddtoGrid(gFNO1, gFID1, gFNO1_Desc, gFID1_Desc, gLOW1, gHIGH1, gFNO2, gFID2, gFNO2_Desc, gFID2_Desc, gLOW2, gHIGH2, gCID, gG3E_ID, ppwo, Conn_Type, EXC_ABB, CABLE_CODE, FEATURE_STATE, TERM_FNO, TERM_FID, TERM_LOW, TERM_HIGH, COLOR);
                        AddtoGrid(gFNO1, gFID1, gFNO1_Desc, gFID1_Desc, gLOW1, gHIGH1, gFNO2, gFID2, gFNO2_Desc, gFID2_Desc, gLOW2, gHIGH2, gCID, gG3E_ID, ppwo, Conn_Type, EXC_ABB, CABLE_CODE, FEATURE_STATE, TERM_FNO, TERM_FID, TERM_LOW, TERM_HIGH);

                        rs.MoveNext();
                    }
                    while (!rs.EOF);
                }


            }
            catch (Exception ex)
            {
                //GTClassFactory.Create<IGTApplication>().EndWaitCursor();
                log.WriteLog("");
                log.WriteLog(this.GetType().FullName, "ERROR: " + ex.Message);
                log.WriteLog(ex.StackTrace);
                MessageBox.Show(ex.Message, C_PROG_NAME, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void HighlightGrid()
        {
            string mFID1 = string.Empty;
            string mFID2 = string.Empty;

            //Clear all grid highlight first
            UnHighlightAllGridRow();

            //get selected FID1 & FID2
            string sFID1 = string.Empty;
            string sFID2 = string.Empty;

            if (gbl_CURR_TAB == C_TAB_DSIDE)
            {
                sFID1 = (cboDFrom.SelectedItem == null) ? "" : GetComboValue(cboDFrom, cboDFrom.SelectedItem.ToString());
                sFID2 = (cboDTo.SelectedItem == null) ? "" : GetComboValue(cboDTo, cboDTo.SelectedItem.ToString());
                //sFID1 = cboDFrom.SelectedValue.ToString();
                //sFID2 = cboDFrom.SelectedValue.ToString();
                
            }
            else if (gbl_CURR_TAB == C_TAB_ESIDE)
            {
                sFID1 = (cboFrom.SelectedItem == null) ? "" : GetComboValue(cboFrom, cboFrom.SelectedItem.ToString());
                sFID2 = (cboTo.SelectedItem == null) ? "" : GetComboValue(cboTo, cboTo.SelectedItem.ToString());
            }
            else if (gbl_CURR_TAB == C_TAB_ODF)
            {
                sFID1 = "";
                sFID2 = (cboODFTo.SelectedItem == null) ? "" : GetComboValue(cboODFTo, cboODFTo.SelectedItem.ToString());
            }
            if ((sFID1.Trim() == "") && (sFID2.Trim() == "")) return;
            for (int iRowNum = 0; iRowNum < gv_Route.RowCount; iRowNum++)
            {
                mFID1 = gv_Route.Rows[iRowNum].Cells["FID1"].Value.ToString().Trim();
                mFID2 = gv_Route.Rows[iRowNum].Cells["FID2"].Value.ToString().Trim();
                if ((sFID1 != "") && ((sFID1.Trim() == mFID1) || (sFID1.Trim() == mFID2)))
                {
                    HighlightGridRow(iRowNum);
                }

                if ((sFID2 != "") && ((sFID2.Trim() == mFID1) || (sFID2.Trim() == mFID2)))
                {
                    HighlightGridRow(iRowNum);
                }
            }

        }

        //HighlighGridRow
        private void HighlightGridRow(int iRowNum)
        {
            for (int j = 3; j < gv_Route.ColumnCount; j++)
            {                
                gv_Route.Rows[iRowNum].Cells[j].Style.BackColor = System.Drawing.Color.LemonChiffon;
            }
        }

        //UnHighlighGridRow
        private void UnHighlightAllGridRow()
        {
            for (int iRowNum = 0; iRowNum < gv_Route.RowCount; iRowNum++)
            {
                for (int j = 3; j < gv_Route.ColumnCount; j++)
                {
                    gv_Route.Rows[iRowNum].Cells[j].Style.BackColor = System.Drawing.Color.White;
                }
            }
        }


        //Get the Splice details for selected Feature
        private void Select_SpliceO()
        {
            string gFNO1 = null;
            string gFID1 = null;
            string gFNO1_Desc = null;
            string gFID1_Desc = null;
            string gLOW1 = null;
            string gHIGH1 = null;
            string gFNO2 = null;
            string gFID2 = null;
            string gFNO2_Desc = null;
            string gFID2_Desc = null;
            string gLOW2 = null;
            string gHIGH2 = null;
            string gCID = null;
            string gG3E_ID = null;
            string ppwo = null;
            string sSql = null;
            string Condition = null;
            string Conn_Type = null;
            string EXC_ABB = null;
            string CABLE_CODE = null;
            string FEATURE_STATE = null;
            string TERM_FNO = null;
            string TERM_FID = null;
            string TERM_LOW = null;
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

                    sSql = "SELECT SC.G3E_FNO, SC.G3E_FID, SC.FNO1, SC.FID1,SC.LOW1, SC.HIGH1, SC.FNO2, SC.FID2, SC.LOW2, SC.HIGH2, SC.G3E_CID, sc.g3e_id, SC.WORKORDER, SC.CORE_STATUS, " +
                           " SC.EXC_ABB, SC.CABLE_CODE, SC.FEATURE_STATE, SC.TERM_FNO, SC.TERM_FID, SC.TERM_LOW, SC.TERM_HIGH " +
                           " FROM gc_splice_connect SC " + Condition + " ORDER BY FID2, LOW2";

                    rs = m_IGTDataContext.OpenRecordset(sSql, ADODB.CursorTypeEnum.adOpenStatic, ADODB.LockTypeEnum.adLockReadOnly, 1);
                    if (rs.RecordCount == 0)
                    {
                        //MessageBox.Show("No Records Found.", C_PROG_NAME, MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;

                    }

                    rs.MoveFirst();
                    do
                    {
                        gFNO1 = rs.Fields["FNO1"].Value.ToString();
                        gFID1 = rs.Fields["FID1"].Value.ToString();

                        gFNO1_Desc = GetFeatName(gFNO1);
                        gFID1_Desc = GetFeatCode(Convert.ToInt16(gFNO1), gFID1) + "(" + gFID1 + ")";

                        gLOW1 = rs.Fields[4].Value.ToString();
                        gHIGH1 = rs.Fields[5].Value.ToString();

                        gFNO2 = rs.Fields[6].Value.ToString();
                        gFID2 = rs.Fields[7].Value.ToString();

                        gFNO2_Desc = GetFeatName(gFNO2);
                        gFID2_Desc = GetFeatCode(Convert.ToInt16(gFNO2), gFID2) + "(" + gFID2 + ")";

                        gLOW2 = rs.Fields["LOW2"].Value.ToString();
                        gHIGH2 = rs.Fields["HIGH2"].Value.ToString();
                        gCID = rs.Fields["G3E_CID"].Value.ToString();
                        gG3E_ID = rs.Fields["G3E_ID"].Value.ToString();
                        ppwo = rs.Fields["WORKORDER"].Value.ToString();
                        Conn_Type = rs.Fields["CORE_STATUS"].Value.ToString();
                        EXC_ABB = rs.Fields["EXC_ABB"].Value.ToString();
                        //CABLE_CODE = rs.Fields["CABLE_CODE"].Value.ToString();
                        FEATURE_STATE = rs.Fields["FEATURE_STATE"].Value.ToString();
                        TERM_FNO = rs.Fields["TERM_FNO"].Value.ToString();
                        TERM_FID = rs.Fields["TERM_FID"].Value.ToString();
                        if (TERM_FNO.Trim() != "") CABLE_CODE = GetFeatName(TERM_FNO) + " - " + GetFeatCode(Convert.ToInt16(TERM_FNO), TERM_FID) + "(" + TERM_FID + ")";
                        TERM_LOW = rs.Fields["TERM_LOW"].Value.ToString();
                        TERM_HIGH = rs.Fields["TERM_HIGH"].Value.ToString();

                        COLOR = "";
                        if (gbl_CURR_TAB == C_TAB_ESIDE)
                        {
                            if (cboFrom.Text.Trim() != "" && (gFID1 == GetComboValue(cboFrom, cboFrom.SelectedItem.ToString()) || gFID2 == GetComboValue(cboFrom, cboFrom.SelectedItem.ToString())))
                                COLOR = "YES";
                            if (cboTo.Text.Trim() != "" && (gFID2 == GetComboValue(cboTo, cboTo.SelectedItem.ToString()) || gFID1 == GetComboValue(cboTo, cboTo.SelectedItem.ToString())))
                                COLOR = "YES";
                        } else if (gbl_CURR_TAB == C_TAB_DSIDE)
                        {
                            if (cboDFrom.Text.Trim() != "" && (gFID1 == GetComboValue(cboDFrom, cboDFrom.SelectedItem.ToString()) || gFID2 == GetComboValue(cboDFrom, cboDFrom.SelectedItem.ToString())))
                                COLOR = "YES";
                            if (cboDTo.Text.Trim() != "" && (gFID2 == GetComboValue(cboDTo, cboDTo.SelectedItem.ToString()) || gFID1 == GetComboValue(cboDTo, cboDTo.SelectedItem.ToString())))
                                COLOR = "YES";
                        }
                        AddtoGrid0(gFNO1, gFID1, gFNO1_Desc, gFID1_Desc, gLOW1, gHIGH1, gFNO2, gFID2, gFNO2_Desc, gFID2_Desc, gLOW2, gHIGH2, gCID, gG3E_ID, ppwo, Conn_Type, EXC_ABB, CABLE_CODE, FEATURE_STATE, TERM_FNO, TERM_FID, TERM_LOW, TERM_HIGH, COLOR);

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
                MessageBox.Show(ex.Message, C_PROG_NAME, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private string GetFeatName(string gFNO)
        {
            string g_name = string.Empty;
            g_name = Get_Value("Select g3e_username from g3e_feature where g3e_fno = " + gFNO);
            return g_name;
        }

        private string GetFeatCode(int gFNO, string gFID)
        {
            string g_Desc = string.Empty;
            switch (gFNO)
            {
                case FECBL_FNO:
                    g_Desc = Get_Value("select CABLE_CODE  from GC_fcbl where g3e_fid = " + gFID);
                    break;
                case FDCBL_FNO:
                    g_Desc = Get_Value("select FCABLE_CODE  from GC_FDCBL where g3e_fid = " + gFID);
                    break;
                case TRCBL_FNO:
                    g_Desc = Get_Value("select CABLE_CODE  from GC_FCBL_TRUNK where g3e_fid = " + gFID);
                    break;
                case JNCBL_FNO:
                    g_Desc = Get_Value("select CABLE_CODE  from GC_FCBL_JNT where g3e_fid = " + gFID);
                    break;
                case FDC_FNO:
                    g_Desc = Get_Value("select FDC_CODE  from GC_FDC where g3e_fid = " + gFID);
                    break;
                case UPE_FNO:
                    g_Desc = Get_Value("select UPE_CODE  from GC_UPE where g3e_fid = " + gFID);
                    break;
                case FDP_FNO:
                    g_Desc = Get_Value("select FDP_CODE  from GC_FDP where g3e_fid = " + gFID);
                    break;
                case FTB_FNO:
                    g_Desc = Get_Value("select FDP_CODE  from GC_FTB where g3e_fid = " + gFID);
                    break;
                case TIE_FNO:
                    g_Desc = Get_Value("select FDP_CODE  from GC_TIEFDP where g3e_fid = " + gFID);
                    break;
                case DB_FNO:
                    g_Desc = Get_Value("select DB_CODE  from GC_DB where g3e_fid = " + gFID);
                    break;
                case MSAN_FNO:
                    g_Desc = Get_Value("select RT_CODE  from GC_MSAN where g3e_fid = " + gFID);
                    break;
                case DDN_FNO:
                    g_Desc = Get_Value("select DDN_CODE  from GC_DDN where g3e_fid = " + gFID);
                    break;
                case NDH_FNO:
                    g_Desc = Get_Value("select NDH_CODE  from GC_NDH where g3e_fid = " + gFID);
                    break;
                case MUX_FNO:
                    g_Desc = Get_Value("select MUX_CODE  from GC_MINIMUX where g3e_fid = " + gFID);
                    break;
                case SHELF_FNO:
                    g_Desc = Get_Value("select DECODE(FRAME_NO, NULL, G3E_ID, FRAME_NO)  from GC_FSHELF where g3e_fid = " + gFID);
                    break;
                case EPE_FNO:
                    g_Desc = Get_Value("select EPE_CODE  from GC_EPE where g3e_fid = " + gFID);
                    break;
                case FAN_FNO:
                    g_Desc = Get_Value("select FAN_CODE  from GC_FAN where g3e_fid = " + gFID);
                    break;
                case RT_FNO:
                    g_Desc = Get_Value("select RT_CODE  from GC_RT where g3e_fid = " + gFID);
                    break;
                case ODF_FNO:
                    g_Desc = Get_Value("select ODF_NUM  from GC_ODF where g3e_fid = " + gFID);
                    break;
                case FPATCH_FNO:
                    g_Desc = Get_Value("select PATCH_CODE  from GC_FPATCH where g3e_fid = " + gFID);
                    break;
                case FPP_FNO:
                    g_Desc = Get_Value("select PATCH_CODE  from GC_FPATCHPANEL where g3e_fid = " + gFID);
                    break;
                case FSLICE_FNO:
                    g_Desc = Get_Value("select SPLICE_CODE  from GC_FSPLICE where g3e_fid = " + gFID);
                    break;
                case SPLITTER_FNO:
                    //gFID2_Desc = Get_Value("select SPLITTER_CODE from GC_FSPLITTER where g3e_fid = " + gFID2);
                    //Requst from Mike on 11-Aug-2012
                    //g_Desc = Get_Value("select NO_OF_SPLITTER - from GC_FSPLITTER where g3e_fid = " + gFID);
                    g_Desc = Get_Value("select NO_OF_SPLITTER ||'. Spltr '|| TRIM(DECODE(TRIM(upper(SPLITTER_CLASS)),' ','','NORMAL','',TRIM(SPLITTER_CLASS))) ||'('||Splitter_type||')'  from GC_FSPLITTER where g3e_fid =" + gFID);
                    break;
                default:
                    g_Desc = gFID;
                    break;

            }

            return g_Desc;
        }

        private void ReInitializeComboBox(ComboBox myCbo)
        {
            myCbo.Items.Clear();
            myCbo.Items.Add(new cboItem(" ", " "));
        }

        private bool isInComing(int iIn_fid, int ig3e_fid)
        {
            string sValue = Get_Value("select g3e_fid from gc_nr_connect where g3e_fid = " + ig3e_fid.ToString() + " and in_fid = " + iIn_fid);
            return (sValue != null) ? true : false;
        }

        private bool hasPUUnit(int iFID)
        {
            
            string MIN_MAT = Get_Value("select MIN_MATERIAL from GC_NETELEM where G3E_FID = " + iFID);
            if (MIN_MAT == "")
            {
                return false;
            }
            return true;
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

            string sSqlFrom = null;
            string sSqlTo = null;

            //If ODF, no need load any combo
            if (gbl_CURR_TAB == C_TAB_OVERALL_ODF) return;
            //Clear All Combo.
            if (lbl_FID.Text == "") return;  // Straight away exit if nothing selected
            cboFrom.Items.Clear();
            cboFrom.Items.Add(new cboItem(" ", " "));
            cboTo.Items.Clear();
            cboTo.Items.Add(new cboItem(" ", " "));
       
            cboDFrom.Items.Clear();
            cboDFrom.Items.Add(new cboItem(" ", " "));
            cboDTo.Items.Clear();
            cboDTo.Items.Add(new cboItem(" ", " "));

            ReInitializeComboBox(cboODFTo);

            try
            {
                switch (Convert.ToInt32(lbl_fno.Text))
                {
                    case ODF_FNO:  //ODF
                        //Cables Connected to ODF
                        sSqlFrom = string.Empty;
                        sSqlTo = "SELECT DISTINCT g3e_fid, g3e_fno FROM gc_nr_connect WHERE out_fid = " + lbl_FID.Text + " UNION SELECT DISTINCT g3e_fid, g3e_fno FROM gc_nr_connect WHERE IN_fid = " + lbl_FID.Text + "   ORDER BY g3e_fid";
                        break;
                    case MUX_FNO:  //MUX
                        {
                            sSqlFrom = "SELECT DISTINCT g3e_fid, g3e_fno FROM gc_nr_connect WHERE out_fid = " + lbl_FID.Text + " UNION SELECT DISTINCT g3e_fid, g3e_fno FROM gc_nr_connect WHERE IN_fid = " + lbl_FID.Text + "   ORDER BY g3e_fid";
                            sSqlTo = "select G3E_FID, g3e_fno from gc_ownership where owner1_id = (select G3E_ID from gc_ownership where g3e_fid =" + lbl_FID.Text + " )";
                        }

                        break;
                    case FDC_FNO:  //FDC
                        {
                            sSqlFrom = "SELECT DISTINCT g3e_fid, g3e_fno FROM gc_nr_connect WHERE out_fid = " + lbl_FID.Text + " UNION SELECT DISTINCT g3e_fid, g3e_fno FROM gc_nr_connect WHERE IN_fid = " + lbl_FID.Text + "   ORDER BY g3e_fid";
                            sSqlTo = "select G3E_FID, g3e_fno from gc_ownership where owner1_id = (select G3E_ID from gc_ownership where g3e_fid =" + lbl_FID.Text + " )";
                        }
                        
                        break;
                    case EPE_FNO:  //EPE
                    case DDN_FNO:  //DDN
                    case MSAN_FNO:  //IPMSAN
                    case NDH_FNO:  //NDH
                    case RT_FNO:  //RT
                    case UPE_FNO:  //UPE
                    
                        sSqlFrom = "SELECT DISTINCT g3e_fid, g3e_fno FROM gc_nr_connect WHERE out_fid = " + lbl_FID.Text + " UNION SELECT DISTINCT g3e_fid, g3e_fno FROM gc_nr_connect WHERE IN_fid = " + lbl_FID.Text + "   ORDER BY g3e_fid";
                        sSqlTo = "select G3E_FID, g3e_fno from gc_ownership where owner1_id = (select G3E_ID from gc_ownership where g3e_fid =" + lbl_FID.Text + " )";

                        break;
                        //Dside
                    case FDP_FNO:  //FDP
                        sSqlFrom = "SELECT DISTINCT g3e_fid, g3e_fno FROM gc_nr_connect WHERE out_fid = " + lbl_FID.Text + " UNION SELECT DISTINCT g3e_fid, g3e_fno FROM gc_nr_connect WHERE IN_fid = " + lbl_FID.Text + "   ORDER BY g3e_fid";
                        //if (IsFOMSFDP(lbl_fno.Text.Trim(), lbl_FID.Text.Trim()))
                        //{
                        //    sSqlTo = "select G3E_FID, g3e_fno from gc_ownership where owner1_id = (select G3E_ID from gc_ownership where g3e_fid =" + lbl_FID.Text + " ) UNION SELECT DISTINCT g3e_fid, g3e_fno FROM GC_FDP where g3e_Fid = " + lbl_FID.Text;
                        //}
                        //else
                        //{
                            sSqlTo = "select G3E_FID, g3e_fno from gc_ownership where owner1_id = (select G3E_ID from gc_ownership where g3e_fid =" + lbl_FID.Text + " ) ";
                        //}
                        break;
                    case FTB_FNO:  //FTB
                    case DB_FNO:  //DB
                        //Dside
                        sSqlFrom = "SELECT DISTINCT g3e_fid, g3e_fno FROM gc_nr_connect WHERE out_fid = " + lbl_FID.Text + " UNION SELECT DISTINCT g3e_fid, g3e_fno FROM gc_nr_connect WHERE IN_fid = " + lbl_FID.Text + "   ORDER BY g3e_fid";
                        sSqlTo = "select G3E_FID, g3e_fno from gc_ownership where owner1_id = (select G3E_ID from gc_ownership where g3e_fid =" + lbl_FID.Text + " )";
                        break;
                    case FSLICE_FNO:
                        //Check if Current Selected TAB
                        if (gbl_CURR_TAB == C_TAB_DSIDE)
                        {
                            sSqlFrom = "SELECT DISTINCT g3e_fid, g3e_fno FROM gc_nr_connect WHERE out_fid = " + lbl_FID.Text + " ORDER BY g3e_fid";
                            sSqlTo = "SELECT DISTINCT g3e_fid, g3e_fno FROM gc_nr_connect WHERE in_fid = " + lbl_FID.Text + " ORDER BY g3e_fid";
                        }
                        else
                        {
                            sSqlFrom = "SELECT DISTINCT g3e_fid, g3e_fno FROM gc_nr_connect WHERE out_fid = " + lbl_FID.Text + " UNION SELECT DISTINCT g3e_fid, g3e_fno FROM gc_nr_connect WHERE IN_fid = " + lbl_FID.Text + "   ORDER BY g3e_fid";
                            sSqlTo = "SELECT DISTINCT g3e_fid, g3e_fno FROM gc_nr_connect WHERE out_fid = " + lbl_FID.Text + " UNION SELECT DISTINCT g3e_fid, g3e_fno FROM gc_nr_connect WHERE IN_fid = " + lbl_FID.Text + "   ORDER BY g3e_fid";
                        }
                        
                        break;
                   
                    default:
                        sSqlFrom = "SELECT DISTINCT g3e_fid, g3e_fno FROM gc_nr_connect WHERE out_fid = " + lbl_FID.Text + " UNION SELECT DISTINCT g3e_fid, g3e_fno FROM gc_nr_connect WHERE IN_fid = " + lbl_FID.Text + "   ORDER BY g3e_fid";
                        sSqlTo = "SELECT DISTINCT g3e_fid, g3e_fno FROM gc_nr_connect WHERE out_fid = " + lbl_FID.Text + " UNION SELECT DISTINCT g3e_fid, g3e_fno FROM gc_nr_connect WHERE IN_fid = " + lbl_FID.Text + "   ORDER BY g3e_fid";
                        break;
                }


                
                //Load the FROM ComboBox
                if (sSqlFrom.Trim() != "")
                {
                    ADODB.Recordset rs1 = new ADODB.Recordset();
                    rs1 = m_IGTDataContext.OpenRecordset(sSqlFrom, ADODB.CursorTypeEnum.adOpenStatic, ADODB.LockTypeEnum.adLockReadOnly, 1);

                    if (rs1.RecordCount > 0)
                    {
                        rs1.MoveFirst();
                        do
                        {
                            //get the Connected Cable information
                            gFNO1 = rs1.Fields[1].Value.ToString().Trim();
                            gFID1 = rs1.Fields[0].Value.ToString().Trim();

                            //string MIN_MAT = Get_Value("select MIN_MATERIAL from GC_NETELEM where G3E_FID = " + gFID1);
                            //if (MIN_MAT == "")
                            //{
                            //    MessageBox.Show("No Plant Unit selected for the Cable connected to the Device", C_PROG_NAME, MessageBoxButtons.OK, MessageBoxIcon.Information);
                            //    return;
                            //}

                            gFNO1_Desc = GetFeatName(gFNO1);

                            if (gFNO1.Trim() == FECBL_FNO.ToString())
                                gFID1_Desc = Get_Value("select CABLE_CODE ||' (' || EFF_CORE || ')' from GC_fcbl where g3e_fid = " + gFID1);
                            else if (gFNO1 == FDCBL_FNO.ToString())
                                gFID1_Desc = Get_Value("select FCABLE_CODE ||' (' || CORE_NUM || ')' from GC_FDCBL where g3e_fid = " + gFID1);
                            else if (gFNO1 == TRCBL_FNO.ToString())
                                gFID1_Desc = Get_Value("select CABLE_CODE ||' (' || EFF_CORE || ')' from GC_FCBL_TRUNK where g3e_fid = " + gFID1);
                            else if (gFNO1 == JNCBL_FNO.ToString())
                                gFID1_Desc = Get_Value("select CABLE_CODE ||' (' || EFF_CORE || ')' from GC_FCBL_JNT where g3e_fid = " + gFID1);
                            else
                                gFID1_Desc = gFID1;

                            Desc = gFID1_Desc + " - " + gFID1;


                            //Check Current Screen
                            if (gbl_CURR_TAB == C_TAB_DSIDE)
                            {
                                if (lbl_fno.Text.Trim() == FDC_FNO.ToString())
                                {
                                    if (!IsSourceCable(gFNO1, gFID1)) cboDTo.Items.Add(new cboItem(Desc, gFID1));
                                }
                                else
                                {
                                    cboDFrom.Items.Add(new cboItem(Desc, gFID1));
                                }
                            }
                            else
                            {
                                if (lbl_fno.Text.Trim() == FDC_FNO.ToString())
                                {
                                    if (gFNO1 == FDCBL_FNO.ToString())
                                    {
                                        cboDTo.Items.Add(new cboItem(Desc, gFID1));
                                    }
                                    else
                                    {
                                        cboFrom.Items.Add(new cboItem(Desc, gFID1));
                                    }
                                }
                                else
                                {
                                    if (gbl_CURR_TAB == C_TAB_DSIDE)
                                    {
                                        cboDFrom.Items.Add(new cboItem(Desc, gFID1));
                                    }
                                    else
                                    {
                                        cboFrom.Items.Add(new cboItem(Desc, gFID1));
                                    }
                                }

                            }
                            rs1.MoveNext();
                        }
                        while (!rs1.EOF);

                    }
                    else
                    {
                        MessageBox.Show("There are no related feature.", C_PROG_NAME, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    rs1.Close();

                }

                //Now Load TOComboBox
                //Vinod 09-Aug-2012 for the below 3 devices the out is always the Device.
                if (sSqlTo.Trim() != "")
                {
                    ADODB.Recordset rs2 = new ADODB.Recordset();
                    string sInOutStatus = string.Empty;
                    if (lbl_fno.Text == ODF_FNO.ToString())
                    {
                        RefreshODFAvailCore();
                        rs2 = m_IGTDataContext.OpenRecordset(sSqlTo, ADODB.CursorTypeEnum.adOpenStatic, ADODB.LockTypeEnum.adLockReadOnly, 1);
                        if (rs2.RecordCount > 0)
                        {
                            rs2.MoveFirst();
                            do 
                            {
                                gFNO2 = rs2.Fields[1].Value.ToString();
                                gFID2 = rs2.Fields[0].Value.ToString();
                                gFNO2_Desc = GetFeatName(gFNO2);
                                gFID2_Desc = GetFeatCode(Convert.ToInt16(gFNO2), gFID2);

                                sInOutStatus = isInComing(Convert.ToInt32(lbl_FID.Text.Trim()), Convert.ToInt32(gFID2)) ? C_KEY_FR_ODF  : C_KEY_TO_ODF;
                                // Desc =  gFNO2_Desc + ": " + gFID2_Desc + " - " + gFID2;
                                Desc = gFID2_Desc + " - " + gFID2 + " [" + sInOutStatus + "]";
                                cboODFTo.Items.Add(new cboItem(Desc, gFID2));
                                rs2.MoveNext();
                            }
                            while (!rs2.EOF);
                        }
                        else
                        {
                            MessageBox.Show("There are no related feature.", C_PROG_NAME, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        rs2.Close();
                    } 
                    else if ( lbl_fno.Text == FPATCH_FNO.ToString() || lbl_fno.Text == FPP_FNO.ToString() || lbl_fno.Text == MSAN_FNO.ToString() || lbl_fno.Text == RT_FNO.ToString() || lbl_fno.Text == VDSL_FNO.ToString() || lbl_fno.Text == TIE_FNO.ToString())
                    {
                        gFNO2 = lbl_fno.Text;
                        gFID2 = lbl_FID.Text;
                        //gFNO2_Desc = GetFeatName(gFNO2);
                        gFID2_Desc = GetFeatCode(Convert.ToInt16(gFNO2), gFID2);
                        if (IsFOMSFDP(gFNO2, gFID2)) gFID2_Desc += " " + C_FOMS;
                        Desc = gFID2_Desc + " - " + gFID2;

                        //if Termination is for   FDP, FTB, TIE, DB
                        if ((gFNO2 == FDP_FNO.ToString()) || (gFNO2 == FTB_FNO.ToString()) || (gFNO2 == TIE_FNO.ToString()) || (gFNO2 == DB_FNO.ToString()))
                        {
                            cboDTo.Items.Add(new cboItem(Desc, gFID2));
                        }
                        else
                        {
                            //cboTo.Items.Add(new cboItem(Desc, gFID2));
                            if (gbl_CURR_TAB == C_TAB_ESIDE)
                            {
                                if (!IsSourceCable(gFNO2, gFID2)) cboTo.Items.Add(new cboItem(Desc, gFID2));
                            }
                            else
                            {
                                cboDTo.Items.Add(new cboItem(Desc, gFID2));
                            }
                        }
                    }
                    else
                    {
                        //Other like MINIMUX, FDC
                        rs2 = m_IGTDataContext.OpenRecordset(sSqlTo, ADODB.CursorTypeEnum.adOpenStatic, ADODB.LockTypeEnum.adLockReadOnly, 1);

                        if (rs2.RecordCount > 0)
                        {
                            rs2.MoveFirst();
                            do
                            {
                                gFNO2 = rs2.Fields[1].Value.ToString();
                                gFID2 = rs2.Fields[0].Value.ToString();

                                //string MIN_MAT = Get_Value("select MIN_MATERIAL from GC_NETELEM where G3E_FID = " + gFID2);
                                //if (MIN_MAT == "")
                                //{
                                //    if (!IsFOMSFDP(lbl_fno.Text.Trim(), lbl_FID.Text.Trim()))
                                //    {
                                //        MessageBox.Show("No Plant Unit selected for the Device", C_PROG_NAME, MessageBoxButtons.OK, MessageBoxIcon.Information);
                                //        return;
                                //    }
                                //}

                                //gFNO2_Desc = GetFeatName(gFNO2);
                                gFID2_Desc = GetFeatCode(Convert.ToInt16(gFNO2), gFID2);

                                //Desc =  gFNO2_Desc + ": " + gFID2_Desc + " - " + gFID2;
                                Desc = gFID2_Desc + " - " + gFID2;

                                if ((gFNO2 == FDP_FNO.ToString()) || (gFNO2 == FTB_FNO.ToString()) || (gFNO2 == TIE_FNO.ToString()) || (gFNO2 == DB_FNO.ToString()))
                                {
                                    cboDTo.Items.Add(new cboItem(Desc, gFID2));
                                }
                                else
                                {
                                    //cboTo.Items.Add(new cboItem(Desc, gFID2));
                                    if (gbl_CURR_TAB == C_TAB_ESIDE)
                                    {
                                        if (!IsSourceCable(gFNO2, gFID2)) cboTo.Items.Add(new cboItem(Desc, gFID2));
                                    }
                                    else
                                    {
                                        cboDTo.Items.Add(new cboItem(Desc, gFID2));
                                    }

                                    if ((lbl_fno.Text.Trim() == FDC_FNO.ToString()) && (gFNO2 == SPLITTER_FNO.ToString()))
                                    {
                                        cboDFrom.Items.Add(new cboItem(Desc, gFID2));

                                    }

                                    if ((lbl_fno.Text.Trim() == MUX_FNO.ToString()) && (gFNO2 == SHELF_FNO.ToString()))
                                    {
                                        cboDFrom.Items.Add(new cboItem(Desc, gFID2));

                                    }
                                }

                                rs2.MoveNext();
                            }
                            while (!rs2.EOF);

                        }
                        else
                        {
                            MessageBox.Show("There are no related feature.", C_PROG_NAME, MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                MessageBox.Show(ex.Message, C_PROG_NAME, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        //Add to GRID
        private void AddtoGrid(string gFNO1, string gFID1, string gFNO1D, string gFID1D, string gLOW1, string gHIGH1, string gFNO2, string gFID2, string gFNO2D, string gFID2D, string gLOW2, string gHIGH2, string gCID, string gG3E_ID, string ppwo, string ConnType, string EXC_ABB, string CABLE_CODE, string FEATURE_STATE, string TERM_FNO, string TERM_FID, string TERM_LOW, string TERM_HIGH)
        {
            try
            {
                int n = 0;

                for (int i = 0; i < gv_Route.RowCount; i++)
                {
                    //Ununcheck this row
                    gv_Route.Rows[i].Cells[2].Value = false;
                }

                n = gv_Route.Rows.Add();

                gv_Route.Rows[n].Cells["FNO1"].Value = gFNO1;
                gv_Route.Rows[n].Cells["FID1"].Value = gFID1;
                gv_Route.Rows[n].Cells["FNO1_DESC"].Value = gFNO1D + " [" + gFID1D + "]";
                gv_Route.Rows[n].Cells["LOW1"].Value = gLOW1;
                gv_Route.Rows[n].Cells["HIGH1"].Value = gHIGH1;

                gv_Route.Rows[n].Cells["FNO2"].Value = gFNO2;
                gv_Route.Rows[n].Cells["FID2"].Value = gFID2;
                gv_Route.Rows[n].Cells["FNO2_DESC"].Value = gFNO2D + " [" + gFID2D + "]";
                gv_Route.Rows[n].Cells["LOW2"].Value = gLOW2;
                gv_Route.Rows[n].Cells["HIGH2"].Value = gHIGH2;

                gv_Route.Rows[n].Cells["TERM_FNO"].Value = TERM_FNO;
                gv_Route.Rows[n].Cells["TERM_FID"].Value = TERM_FID;
                gv_Route.Rows[n].Cells["CABLE_CODE"].Value = CABLE_CODE;
                gv_Route.Rows[n].Cells["TERM_LOW"].Value = TERM_LOW;
                gv_Route.Rows[n].Cells["TERM_HIGH"].Value = TERM_HIGH;

                gv_Route.Rows[n].Cells["CID"].Value = gCID;
                gv_Route.Rows[n].Cells["G3E_ID"].Value = gG3E_ID;
                gv_Route.Rows[n].Cells["WORKORDER"].Value = ppwo;
                gv_Route.Rows[n].Cells["Conn_Type"].Value = ConnType;

                gv_Route.Rows[n].Cells["EXC_ABB"].Value = EXC_ABB;
                gv_Route.Rows[n].Cells["Feature_State"].Value = FEATURE_STATE;

                gv_Route.ClearSelection();

            }
            catch (Exception ex)
            {
                log.WriteLog("");
                log.WriteLog(this.GetType().FullName, "ERROR: " + ex.Message);
                log.WriteLog(ex.StackTrace);
                MessageBox.Show(ex.Message, C_PROG_NAME, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void AddtoGrid0(string gFNO1, string gFID1, string gFNO1D, string gFID1D, string gLOW1, string gHIGH1, string gFNO2, string gFID2, string gFNO2D, string gFID2D, string gLOW2, string gHIGH2, string gCID, string gG3E_ID, string ppwo, string ConnType, string EXC_ABB, string CABLE_CODE, string FEATURE_STATE, string TERM_FNO, string TERM_FID, string TERM_LOW, string TERM_HIGH, string COLOR)
        {
            try
            {
                int n = 0, i, j;

                /*
                DataGridViewCheckBoxColumn cbox = new DataGridViewCheckBoxColumn();
                cbox.Name = "chkbox";
                cbox.HeaderText = "Select";
                gv_Route.Columns.Add(cbox);
                gv_Route.AutoSize = true;
                gv_Route.AllowUserToAddRows = false;
                */

                for (i = 0; i < gv_Route.RowCount; i++)
                {
                    gv_Route.Rows[i].Cells[2].Value = false;
                }

                n = gv_Route.Rows.Add();

                for (j = 3; j < gv_Route.ColumnCount; j++)
                {
                    if (COLOR == "YES")
                        gv_Route.Rows[n].Cells[j].Style.BackColor = System.Drawing.Color.LemonChiffon;
                }

                gv_Route.Rows[n].Cells["FNO1"].Value = gFNO1;
                gv_Route.Rows[n].Cells["FID1"].Value = gFID1;
                gv_Route.Rows[n].Cells["FNO1_DESC"].Value = gFNO1D + " [" + gFID1D + "]";
                gv_Route.Rows[n].Cells["LOW1"].Value = gLOW1;
                gv_Route.Rows[n].Cells["HIGH1"].Value = gHIGH1;

                gv_Route.Rows[n].Cells["FNO2"].Value = gFNO2;
                gv_Route.Rows[n].Cells["FID2"].Value = gFID2;
                gv_Route.Rows[n].Cells["FNO2_DESC"].Value = gFNO2D + " [" + gFID2D + "]";
                gv_Route.Rows[n].Cells["LOW2"].Value = gLOW2;
                gv_Route.Rows[n].Cells["HIGH2"].Value = gHIGH2;

                gv_Route.Rows[n].Cells["TERM_FNO"].Value = TERM_FNO;
                gv_Route.Rows[n].Cells["TERM_FID"].Value = TERM_FID;
                gv_Route.Rows[n].Cells["CABLE_CODE"].Value = CABLE_CODE;
                gv_Route.Rows[n].Cells["TERM_LOW"].Value = TERM_LOW;
                gv_Route.Rows[n].Cells["TERM_HIGH"].Value = TERM_HIGH;

                gv_Route.Rows[n].Cells["CID"].Value = gCID;
                gv_Route.Rows[n].Cells["G3E_ID"].Value = gG3E_ID;
                gv_Route.Rows[n].Cells["WORKORDER"].Value = ppwo;
                gv_Route.Rows[n].Cells["Conn_Type"].Value = ConnType;

                gv_Route.Rows[n].Cells["EXC_ABB"].Value = EXC_ABB;
                gv_Route.Rows[n].Cells["Feature_State"].Value = FEATURE_STATE;

                gv_Route.ClearSelection();

            }
            catch (Exception ex)
            {
                log.WriteLog("");
                log.WriteLog(this.GetType().FullName, "ERROR: " + ex.Message);
                log.WriteLog(ex.StackTrace);
                MessageBox.Show(ex.Message, C_PROG_NAME, MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                MessageBox.Show("FOMS/STUMP Record already Exists for the Fiber Splice.", C_PROG_NAME, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
        }

   

        private void btn_Assign_Click(object sender, EventArgs e)
        {
            string sSql = string.Empty;
            ADODB.Recordset rs;
            //Do Check on what to parse in
            //First check the Radio button
            switch (E_ACTION_MODE)
            {
                case C_SPARE:
                    //Catheirne: JAN 2013 Take out checking, allow User to all more record in Spare 
                    //sSql = "Select * from GC_SPLICE_CONNECT where G3E_FNO = " + lbl_fno.Text + " and G3E_FID = " + lbl_FID.Text + " and CORE_STATUS = 'SPARE'";
                    //rs = m_IGTDataContext.OpenRecordset(sSql, ADODB.CursorTypeEnum.adOpenStatic, ADODB.LockTypeEnum.adLockReadOnly, 1);
                    //if (rs.RecordCount == 0)
                    //{
                        Connect(C_SPARE);

                    //}
                    //else
                    //{
                    //    MessageBox.Show("SPARE Record already Exists for the Fiber Splice.", C_PROG_NAME, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    //    return;
                    //}
                    break;
                case C_STUMP:
                    //Catherine: JAN 2013 Take out checking, allow User to all more record in Stump
                    //sSql = "Select * from GC_SPLICE_CONNECT where G3E_FNO = " + lbl_fno.Text + " and G3E_FID = " + lbl_FID.Text + " and CORE_STATUS = 'STUMP'";
                    //rs = m_IGTDataContext.OpenRecordset(sSql, ADODB.CursorTypeEnum.adOpenStatic, ADODB.LockTypeEnum.adLockReadOnly, 1);
                    //if (rs.RecordCount == 0)
                    //{
                        Connect(C_STUMP);

                    //}
                    //else
                    //{
                    //    MessageBox.Show("STUMP Record already Exists for the Fiber Splice.", C_PROG_NAME, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    //    return;
                    //}
                    break;

                case C_DISTRIBUTE:
                    if ((lbl_fno.Text == FDC_FNO.ToString()) || (lbl_fno.Text == RT_FNO.ToString()) || (lbl_fno.Text == MSAN_FNO.ToString()) || (lbl_fno.Text == VDSL_FNO.ToString()) || (lbl_fno.Text == MUX_FNO.ToString()) )
                    {
                        if (rBtnMain.Checked)
                        {
                            Connect(C_MAIN);
                        }
                        if (rbtnProtection.Checked)
                        {
                            Connect(C_PROCTECTION);
                        }
                    }
                    else
                    {
                        Connect("");
                    }
                    break;
                default:
                    break;

            }

        }
       

        private string getFNO(string sFID)
        {
            string sFNO = string.Empty;
            sFNO = Get_Value("Select g3e_fno from gc_netelem where g3e_fid = " + sFID);
            
            return sFNO;
        }

        private bool IsWithinRngList(int iLo, int iHi, string[] arrList)
        {
            bool bStatus = false;

            string[] split;
            int iTLo = -1;
            int iTHi = -1;
            for (int i = 0; i < arrList.Length; i++)
            {
                split = arrList[i].Split(new Char[] { '|', '~' });
                iTLo = (split[0].ToString().Trim() == "") ? -1 : Convert.ToInt16(split[0].ToString().Trim());
                iTHi = (split[0].ToString().Trim() == "") ? -1 : Convert.ToInt16(split[1].ToString().Trim());

                if ((iTLo <= iLo) && (iHi <= iTHi)) 
                {
                    bStatus = true;
                    break;
                }
            }
            
            return bStatus;

        }

        public string[] getListFromListBox(ListBox myListBox)
        {
            string[] myList = new string[myListBox.Items.Count];
            int idx = 0;
            foreach (var item in myListBox.Items)
            {
                myList[idx] = item.ToString();
                idx++;
            }
            return myList;
        }

        private bool IsLoopConnection(string sFID1, string sLo1, string sHi1, string sFID2, string sLo2, string sHi2)
        {
            try
            {
                
                bool bStatus = true;
                ADODB.Recordset rsPP = new ADODB.Recordset();
                string sSQL = "select * from gc_splice_connect where fid1 = " + sFID2 + " and fid2 = " + sFID1 +
                                " and low2 <= " + sLo1 + " and high2 >= " + sHi1;
                
                rsPP = m_IGTDataContext.OpenRecordset(sSQL, ADODB.CursorTypeEnum.adOpenStatic, ADODB.LockTypeEnum.adLockReadOnly, 1);
                if (rsPP.RecordCount > 0)
                {
                    rsPP.MoveFirst();
                    bStatus = true;
                }
                else
                {
                    bStatus = false;
                }
                return bStatus;
            }
            catch (Exception ex)
            {
                log.WriteLog("");
                log.WriteLog(this.GetType().FullName, "ERROR: " + ex.Message);
                log.WriteLog(ex.StackTrace);
                MessageBox.Show(ex.Message, C_PROG_NAME, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return true;
            }

        }
        private bool IsAllInputValueValid(string CORE_STATUS)
        {
            bool bStatus = true;

            string sFID1 = string.Empty;
            string sFNO1 = string.Empty;
            int iSize1 = 0;
            int iLo1 = 0;
            int iHi1 = 0;
            string sFID2 = string.Empty;
            int iSize2 = 0;
            string sFNO2 = string.Empty;
            int iLo2 = 0;
            int iHi2 = 0;
            //string[] RngList1;
            //string[] RngList2;
            bool bIsWithinRng1 = false;
            bool bIsWithinRng2 = false;
            bool bIsLoopConnection = false;

            if (gbl_CURR_TAB == C_TAB_ODF)
            {
                sFID1 = lbl_FID.Text.Trim() ;
                sFNO1 = lbl_fno.Text.Trim();
                iSize1 = (lbl_FromODF.Text.Trim() == "") ? 0 : Convert.ToInt16(lbl_FromODF.Text.Trim());
                iLo1 = (txtODFLOW1.Text.Trim() == "") ? 0 : Convert.ToInt16(txtODFLOW1.Text.Trim());
                iHi1 = (txtODFHIGH1.Text.Trim() == "") ? 0 : Convert.ToInt16(txtODFHIGH1.Text.Trim());

                bIsWithinRng1 = IsWithinRngList(iLo1, iHi1, getListFromListBox(LstInUnAssRngODF));
                sFID2 = GetComboValue(cboODFTo, cboODFTo.SelectedItem.ToString());
                if (sFID2.Trim() == "") return false;
                sFNO2 = getFNO(sFID2);
                iSize2 = (lbl_ToODF.Text.Trim() == "") ? 0 : Convert.ToInt16(lbl_ToODF.Text.Trim()); ;
                iLo2 = (txtODFLOW2.Text.Trim() == "") ? 0 : Convert.ToInt16(txtODFLOW2.Text.Trim()); ;
                iHi2 = (txtODFHIGH2.Text.Trim() == "") ? 0 : Convert.ToInt16(txtODFHIGH2.Text.Trim()); ;
                bIsWithinRng2 = IsWithinRngList(iLo2, iHi2, getListFromListBox(LstOutUnAssRngODF));
                bIsLoopConnection = false;

            }
            else if (gbl_CURR_TAB == C_TAB_ESIDE)
            {
                sFID1 = GetComboValue(cboFrom, cboFrom.SelectedItem.ToString());
                sFNO1 = getFNO(sFID1);
                iSize1 = (lbl_FromCore.Text.Trim() == "") ? 0 : Convert.ToInt16(lbl_FromCore.Text.Trim());
                iLo1 = (txtLOW1.Text.Trim() == "") ? 0 : Convert.ToInt16(txtLOW1.Text.Trim());
                iHi1 = (txtHIGH1.Text.Trim() == "") ? 0 : Convert.ToInt16(txtHIGH1.Text.Trim());

                bIsWithinRng1 = IsWithinRngList(iLo1, iHi1, getListFromListBox(LstInUnAssRngCore));
                if ((CORE_STATUS != C_SPARE) && (CORE_STATUS != C_STUMP))
                {
                    sFID2 = GetComboValue(cboTo, cboTo.SelectedItem.ToString()); ;
                    sFNO2 = getFNO(sFID2);
                    iSize2 = (lbl_ToCore.Text.Trim() == "") ? 0 : Convert.ToInt16(lbl_ToCore.Text.Trim()); ;
                    iLo2 = (txtLOW2.Text.Trim() == "") ? 0 : Convert.ToInt16(txtLOW2.Text.Trim()); ;
                    iHi2 = (txtHIGH2.Text.Trim() == "") ? 0 : Convert.ToInt16(txtHIGH2.Text.Trim()); ;
                    bIsWithinRng2 = IsWithinRngList(iLo2, iHi2, getListFromListBox(LstOutUnAssRngCore));
                    if ((sFID1 != "") && (sFID2 != "") && (iHi2 - iLo2 + 1 > 0))
                    {
                        bIsLoopConnection = IsLoopConnection(sFID1, iLo1.ToString(), iHi1.ToString(), sFID2, iLo2.ToString(), iHi2.ToString());
                    }
                }
                else
                {
                    bIsWithinRng2 = true;
                }

            }
            else if (gbl_CURR_TAB == C_TAB_DSIDE)
            {

                sFID1 = GetComboValue(cboDFrom, cboDFrom.SelectedItem.ToString());
                sFNO1 = getFNO(sFID1);
                iSize1 = (lbl_FromPort.Text.Trim() == "") ? 0 : Convert.ToInt16(lbl_FromPort.Text.Trim());
                iLo1 = (txtDLOW1.Text.Trim() == "") ? 0 : Convert.ToInt16(txtDLOW1.Text.Trim());
                iHi1 = (txtDHIGH1.Text.Trim() == "") ? 0 : Convert.ToInt16(txtDHIGH1.Text.Trim());
                
                bIsWithinRng1 = IsWithinRngList(iLo1, iHi1, getListFromListBox(LstInUnAssRngPort));
                if ((CORE_STATUS != C_SPARE) && (CORE_STATUS != C_STUMP) && (CORE_STATUS != C_FOMS))
                {
                    sFID2 = GetComboValue(cboDTo, cboDTo.SelectedItem.ToString()); ;
                    sFNO2 = getFNO(sFID2);
                    iSize2 = (lbl_ToPort.Text.Trim() == "") ? 0 : Convert.ToInt16(lbl_ToPort.Text.Trim()); ;
                    iLo2 = (txtDLOW2.Text.Trim() == "") ? 0 : Convert.ToInt16(txtDLOW2.Text.Trim()); ;
                    iHi2 = (txtDHIGH2.Text.Trim() == "") ? 0 : Convert.ToInt16(txtDHIGH2.Text.Trim()); ;

                    bIsWithinRng2 = IsWithinRngList(iLo2, iHi2, getListFromListBox(LstOutUnAssRngPort));
                }
                else
                {
                    bIsWithinRng2 = true;
                }
            }

            // 1. Incoming and Outgoing cannot be same Feature
            if (sFID1  != "" && sFID2  != "")
            {
                if (sFID1  == sFID2)
                {
                    MessageBox.Show("Loopback Connection Can't be established ", C_PROG_NAME, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return false;
                }
            }

            // 2. Priminary Checking on Incoming Setting.
            if (sFID1 == "")
            {
                MessageBox.Show("Please select the Cable/Device ", C_PROG_NAME, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return false;
            }
            if (iLo1 <= 0)
            {
                MessageBox.Show("Low Core Cannot be Empty.", C_PROG_NAME, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return false;
            }
            if (iHi1 <= 0)
            {
                MessageBox.Show("High Core Cannot be Empty.", C_PROG_NAME, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return false;
            }

            int count1 = iHi1 - iLo1 + 1;
            if (count1 > iSize1)
            {
                MessageBox.Show("Effective Core (" + iSize1 + ") Exceeds the Pair Count...", C_PROG_NAME, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return false;
                
            }

            if (iLo1 > iHi1)
            {
                MessageBox.Show("Incoming : Low Core is Greater than High Core.", C_PROG_NAME, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return false;
            }

            // 2.1 Further Check on the Source Low Hi Range vs the List
            if (!bIsWithinRng1)
            {
                MessageBox.Show("Incoming : Low Core or High Core Splice Connection Already Exists...", C_PROG_NAME, MessageBoxButtons.OK, MessageBoxIcon.Information);
               return false;
            }
            // 3. Priminary Check on Outoging Setting, Check only if Status not SPARE/STUMP/FOMS
            if ((CORE_STATUS != C_SPARE) && (CORE_STATUS != C_STUMP) && (CORE_STATUS != C_FOMS))
            {

                if (sFID2 == "")
                {
                    MessageBox.Show("Please select the outgoing Device", C_PROG_NAME, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return false;
                }

                
                if (iLo2  <= 0 )
                {
                    MessageBox.Show("Outgoing : Low Core Cannot be Empty.", C_PROG_NAME, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return false;
                }
                if (iHi2 <= 0)
                {
                    MessageBox.Show("Outgoing : High Core Cannot be Empty.", C_PROG_NAME, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return false;
                }
                if (iLo2  > iHi2)
                {
                    MessageBox.Show("Outgoing : Low Core is Greater than High Core.", C_PROG_NAME, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return false;
                }

                //int count = Convert.ToInt32(txtHIGH2.Text) - Convert.ToInt32(txtLOW2.Text) + 1;
                int count2 = iHi2 - iLo2 + 1;

                if (count2 > iSize2)
                {
                    MessageBox.Show("Port/Strand (" + iSize2  + ") Exceeds the Pair Count...", C_PROG_NAME, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return false;
                }

                if (count2 != count1)
                {
                    MessageBox.Show("Core Count and Port Count Don't match. Pair's Count Exceed...", C_PROG_NAME, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return false;
                }

                // 4. Further Check the 

                if (!bIsWithinRng2)
                {
                    MessageBox.Show("Outgoing Low Core & High Core Splice Connection Already Exists...", C_PROG_NAME, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return false;
                }

                if (bIsLoopConnection)
                {
                    MessageBox.Show("Current Selected outgoing Cable will cause Loopback Connection ", C_PROG_NAME, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return false;
                }
                
            }
            return bStatus;
        }


        private bool IsFOMSplitter(string sFNO, string sFID)
        {
            bool sStatus = false;
            if (sFNO == SPLITTER_FNO.ToString()) {
                string sValue = Get_Value("Select Upper(splitter_class) from GC_FSPLITTER where g3e_FID = " + sFID);
                if (sValue.Trim() == C_FOMS) sStatus = true;
            }
            return sStatus;
        }

        private void Connect(string CORE_STATUS)
        {
            try
            {
                
                // Checkings
                if (!IsAllInputValueValid(CORE_STATUS)) return;
                
                IGTKeyObject cblSFeat = null;
                IGTKeyObject cblDFeat = null;
                IGTKeyObject devFeat = null;
                string cblFNO = null;
                string toFID = null;
                string Spltr_cls = string.Empty;
               
                 //string CFNO = string.Empty;
                 int iLow1 = 0;
                 int iHigh1 = 0;
                 int iLow2 = 0;
                 int iHigh2 = 0;

                //For FDC E-Side
                 if (gbl_CURR_TAB == C_TAB_ESIDE)
                 {
                     //CFNO = Get_Value("Select g3e_fno from gc_netelem where g3e_fid = " + GetComboValue(cboFrom, cboFrom.SelectedItem.ToString()));
                     iLow1 = iLow1 + int.Parse(txtLOW1.Text);
                     iHigh1 = iHigh1 + int.Parse(txtHIGH1.Text);
                     cblFNO = Get_Value("select g3e_fno from gc_netelem where g3e_fid = " + GetComboValue(cboFrom, cboFrom.SelectedItem.ToString()));
                     cblSFeat = GTClassFactory.Create<IGTApplication>().DataContext.OpenFeature(short.Parse(cblFNO), int.Parse(GetComboValue(cboFrom, cboFrom.SelectedItem.ToString())));

                     //Catherine: Only stump / spare can have same FID2 & FID1.
                     if ((CORE_STATUS == C_STUMP) || (CORE_STATUS == C_SPARE))
                     {
                         //auto set
                         iLow2 = iLow1 ;
                         iHigh2 = iHigh1 ;
                         cblDFeat = cblSFeat;
                     }
                     else
                     {
                         //Not Spare or Stump, then have to check if connect to FOMS Splitter, 
                         //if yes, set CORE_STATUS to C_FOMS_XXXX

                         iLow2 = iLow2 + int.Parse(txtLOW2.Text);
                         iHigh2 = iHigh2 + int.Parse(txtHIGH2.Text);
                         toFID = GetComboValue(cboTo, cboTo.SelectedItem.ToString());
                         cblFNO = Get_Value("select g3e_fno from gc_netelem where g3e_fid = " + toFID.ToString());
                         if (IsFOMSplitter(cblFNO, toFID ) )
                         {
                             if (CORE_STATUS == C_MAIN) CORE_STATUS = C_FOMS_MAIN;
                             if (CORE_STATUS == C_PROCTECTION) CORE_STATUS = C_FOMS_PROTECTION;
                         }
                         cblDFeat = GTClassFactory.Create<IGTApplication>().DataContext.OpenFeature(short.Parse(cblFNO), int.Parse(GetComboValue(cboTo, cboTo.SelectedItem.ToString())));
                     }
                     cblFNO = Get_Value("select g3e_fno from gc_netelem where g3e_fid = " + lbl_FID.Text);
                     devFeat = GTClassFactory.Create<IGTApplication>().DataContext.OpenFeature(short.Parse(cblFNO), int.Parse(lbl_FID.Text));

                 }
                 else if (gbl_CURR_TAB == C_TAB_DSIDE) //For D-Side
                 {
                     //CFNO = Get_Value("Select g3e_fno from gc_netelem where g3e_fid = " + GetComboValue(cboDFrom, cboDFrom.SelectedItem.ToString()));
                     iLow1 = iLow1 + int.Parse(txtDLOW1.Text);
                     iHigh1 = iHigh1 + int.Parse(txtDHIGH1.Text);
                     
                     cblFNO = Get_Value("select g3e_fno from gc_netelem where g3e_fid = " + GetComboValue(cboDFrom, cboDFrom.SelectedItem.ToString()));
                     cblSFeat = GTClassFactory.Create<IGTApplication>().DataContext.OpenFeature(short.Parse(cblFNO), int.Parse(GetComboValue(cboDFrom, cboDFrom.SelectedItem.ToString())));

                     //Catherine: Only stump / spare can have same FID2 & FID1.
                     //if ((CORE_STATUS == C_STUMP) || (CORE_STATUS == C_SPARE) || (CORE_STATUS == C_FOMS))
                     if ((CORE_STATUS == C_STUMP) || (CORE_STATUS == C_SPARE) )
                     {
                         //auto set
                         iLow2 = iLow1 ;
                         iHigh2 = iHigh1 ;
                         cblDFeat = cblSFeat;
                     }
                     else
                     {
                         iLow2 = iLow2 + int.Parse(txtDLOW2.Text);
                         iHigh2 = iHigh2 + int.Parse(txtDHIGH2.Text);
                         cblFNO = Get_Value("select g3e_fno from gc_netelem where g3e_fid = " + GetComboValue(cboDTo, cboDTo.SelectedItem.ToString()));
                         cblDFeat = GTClassFactory.Create<IGTApplication>().DataContext.OpenFeature(short.Parse(cblFNO), int.Parse(GetComboValue(cboDTo, cboDTo.SelectedItem.ToString())));
                     }
                     cblFNO = Get_Value("select g3e_fno from gc_netelem where g3e_fid = " + lbl_FID.Text);
                 }

                //get Current Symbol/Node FID
                devFeat = GTClassFactory.Create<IGTApplication>().DataContext.OpenFeature(short.Parse(cblFNO), int.Parse(lbl_FID.Text));


                //if (lbl_fno.Text == FDC_FNO.ToString() && CFNO == FDCBL_FNO.ToString())
                //    FiberConnect(cblDFeat, devFeat, cblSFeat, iLow2, iHigh2, iLow1, iHigh1, CORE_STATUS);
                //else
                FiberConnect(cblSFeat, devFeat, cblDFeat, iLow1, iHigh1, iLow2, iHigh2, CORE_STATUS);
               
                All_Clear();

                //Referesh the Grid
                FillDataGrid();
                //Select_Splice();

                lbl_MSG.Text = "Splice Connection Done.";
                GTClassFactory.Create<IGTApplication>().SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Core Count.");

            }
            catch (Exception ex)
            {
                //GTClassFactory.Create<IGTApplication>().EndWaitCursor();
                log.WriteLog("");
                log.WriteLog(this.GetType().FullName, "ERROR: " + ex.Message);
                log.WriteLog(ex.StackTrace);
                MessageBox.Show(ex.Message, C_PROG_NAME, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        public void DrawStump(short iFNO, int iFID, string CORE_TYPE)
        {
            try
            {
                IGTKeyObject oExtFeature;
                IGTTextPointGeometry oTextGeom;
                IGTOrientedPointGeometry oPointGeom;
                IGTVector oVector;
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
                oPointGeom = GTClassFactory.Create<IGTOrientedPointGeometry>();
                oVector = GTClassFactory.Create<IGTVector>();
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
                MessageBox.Show(ex.Message, C_PROG_NAME, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        void All_Clear()
        {
            txtHIGH1.Text = "";
            txtHIGH2.Text = "";
            txtDHIGH1.Text = "";
            txtDHIGH2.Text = "";
            txtDLOW1.Text = "";
            txtDLOW2.Text = "";
            txtLOW1.Text = "";
            txtLOW2.Text = "";
            txtODFLOW1.Text = "";
            txtODFLOW2.Text = "";
            txtODFHIGH1.Text = "";
            txtODFHIGH2.Text = "";

            lbl_MSG.Text = "";
            cboFrom.SelectedIndex = -1;
            cboTo.SelectedIndex = -1;
            cboDFrom.SelectedIndex = -1;
            cboDTo.SelectedIndex = -1;
            cboODFTo.SelectedIndex = -1;

        }


        private bool IsSourceCable(string fno, string FID)
        {
            bool bReturn = false;
            if ((fno != FECBL_FNO.ToString()) && (fno != FDCBL_FNO.ToString())) return bReturn;
            ADODB.Recordset rs1 = new ADODB.Recordset();
            string s_in_fno = string.Empty;
            string s_out_fno = string.Empty;
            string sSql = string.Empty;

            sSql = "SELECT in_fno, out_fno FROM gc_nr_connect WHERE g3e_fid=" + FID;
            rs1 = m_IGTDataContext.OpenRecordset(sSql, ADODB.CursorTypeEnum.adOpenStatic, ADODB.LockTypeEnum.adLockReadOnly, 1);
            if (rs1.RecordCount > 0)
            {
                rs1.MoveFirst();
                while (!rs1.EOF)
                {
                    s_in_fno = rs1.Fields["in_fno"].Value.ToString();
                    s_out_fno = rs1.Fields["out_fno"].Value.ToString();
                    rs1.MoveNext();
                }

            }
            rs1.Close();
            if (((s_in_fno == ODF_FNO.ToString()) || (s_out_fno == ODF_FNO.ToString())) && fno == FECBL_FNO.ToString()) bReturn = true;
            if (((s_in_fno == FPATCH_FNO.ToString()) || (s_out_fno == FPATCH_FNO.ToString())) && fno == FECBL_FNO.ToString()) bReturn = true;
            if (((s_in_fno == FPP_FNO.ToString()) || (s_out_fno == FPP_FNO.ToString())) && fno == FECBL_FNO.ToString()) bReturn = true;
            if (((s_in_fno == SPLITTER_FNO.ToString()) || (s_out_fno == SPLITTER_FNO.ToString())) && fno == FDCBL_FNO.ToString()) bReturn = true;
            return bReturn;
        }

        
        private void Highlight(int[] mFID)
        {
            try
            {
                short myFNO = 0;
                int myFID = 0;
                string sFNO = string.Empty;
                GTClassFactory.Create<IGTApplication>().SelectedObjects.Clear();

                for (int i = 0; i < mFID.Length; i++)
                {
                    myFID = mFID[i];
                    sFNO = getFNO(myFID.ToString()).Trim();
                    if (sFNO != "") myFNO = Convert.ToInt16(sFNO);
                    oGTKeyObjs = (IGTDDCKeyObjects)GTClassFactory.Create<IGTApplication>().DataContext.GetDDCKeyObjects(myFNO, myFID, GTComponentGeometryConstants.gtddcgAllGeographic);
                    
                    for (int K = 0; K < oGTKeyObjs.Count; K++)
                        GTClassFactory.Create<IGTApplication>().SelectedObjects.Add(GTSelectModeConstants.gtsosmAllDisplayedComponentsInActiveLegend, oGTKeyObjs[K]);
                   
                    oGTKeyObjs = null;
                }
                GTClassFactory.Create<IGTApplication>().RefreshWindows();

            }
            catch (Exception ex)
            {
                log.WriteLog("");
                log.WriteLog(this.GetType().FullName, "ERROR: " + ex.Message);
                log.WriteLog(ex.StackTrace);
                //MessageBox.Show(ex.Message, C_PROG_NAME, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
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
                //MessageBox.Show(ex.Message, C_PROG_NAME, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void HighlightAndZoom(short mFNO, int mFID)
        {
            try
            {
                GTClassFactory.Create<IGTApplication>().SelectedObjects.Clear();
                oGTKeyObjs = (IGTDDCKeyObjects)GTClassFactory.Create<IGTApplication>().DataContext.GetDDCKeyObjects(mFNO, mFID, GTComponentGeometryConstants.gtddcgAllGeographic);
                for (int K = 0; K < oGTKeyObjs.Count; K++)
                    GTClassFactory.Create<IGTApplication>().SelectedObjects.Add(GTSelectModeConstants.gtsosmAllDisplayedComponentsInActiveLegend, oGTKeyObjs[K]);


                //if (opt_FitZoom.Checked == true)
                //{
                GTClassFactory.Create<IGTApplication>().ActiveMapWindow.CenterSelectedObjects();
                GTClassFactory.Create<IGTApplication>().ActiveMapWindow.DisplayScale = 500;
                //}
                GTClassFactory.Create<IGTApplication>().RefreshWindows();
                oGTKeyObjs = null;

            }
            catch (Exception ex)
            {
                log.WriteLog("");
                log.WriteLog(this.GetType().FullName, "ERROR: " + ex.Message);
                log.WriteLog(ex.StackTrace);
                //MessageBox.Show(ex.Message, C_PROG_NAME, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void refreshODFUnAssignList(int mFNO, int mFID, ListBox myListBox)
        {
            string itmRng = string.Empty;
            //List<string> _items = new List<string>();
            string sFNO = string.Empty;
            string sFID = string.Empty;
            string sFName = string.Empty;
            if ((mFNO != FECBL_FNO) && (mFNO != TRCBL_FNO ) && (mFNO != JNCBL_FNO ) && (mFNO != ODF_FNO)) return;

            ADODB.Recordset rs1 = new ADODB.Recordset();
            string sSql = "";

            sSql = "Select A.LO, A.HI from table (SELECT AG_GET_UNASS_FODFRNG(" + mFNO + ", " + mFID  + ") from dual ) A";
            rs1 = m_IGTDataContext.OpenRecordset(sSql, ADODB.CursorTypeEnum.adOpenStatic, ADODB.LockTypeEnum.adLockReadOnly, 1);
            if (rs1.RecordCount > 0)
            {
                myListBox.Items.Clear();
                rs1.MoveFirst();
                while (!rs1.EOF)
                {
                    if (rs1.Fields["LO"].Value.ToString() != "-1")
                    {
                        itmRng = rs1.Fields["LO"].Value.ToString() + " ~ " + rs1.Fields["HI"].Value.ToString();
                        //_items.Add(itmRng); // <-- Add these
                        myListBox.Items.Add(itmRng);

                    }
                    rs1.MoveNext();
                }

            }
            rs1.Close();
            
        }


        private void refreshUnAssignList(int mFNO, int mFID, int bCarrFwd, ListBox myListBox)
        {
            string itmRng = string.Empty;
            //List<string> _items = new List<string>();
            string sFNO = string.Empty;
            string sFID = string.Empty;
            string sFName = string.Empty;
            if ((mFNO != FECBL_FNO) && (mFNO != FDCBL_FNO) && (mFNO != SPLITTER_FNO) && (mFNO != RT_FNO) && (mFNO != MSAN_FNO) && (mFNO != VDSL_FNO) && (mFNO != SHELF_FNO)) return;

            ADODB.Recordset rs1 = new ADODB.Recordset();
            string sSql = "";
            if (bCarrFwd < 1) // NOT carry forward
            {
                if (gbl_CURR_TAB == C_TAB_DSIDE)
                {
                    sSql = "Select A.LO, A.HI from table (SELECT AG_GET_D_UNASS_FPORTRNG(" + mFNO + ", " + mFID + ", " + bCarrFwd + ") from dual ) A";

                }
                else
                {
                    sSql = "Select A.LO, A.HI from table (SELECT AG_GET_E_UNASS_FCORERNG(" + mFNO + ", " + mFID + ", " + bCarrFwd + ") from dual ) A";
                }
                rs1 = m_IGTDataContext.OpenRecordset(sSql, ADODB.CursorTypeEnum.adOpenStatic, ADODB.LockTypeEnum.adLockReadOnly, 1);
                if (rs1.RecordCount > 0)
                {
                    myListBox.Items.Clear();
                    rs1.MoveFirst();
                    while (!rs1.EOF)
                    {
                        if (rs1.Fields["LO"].Value.ToString() != "-1")
                        {
                            itmRng = rs1.Fields["LO"].Value.ToString() + " ~ " + rs1.Fields["HI"].Value.ToString();
                            //_items.Add(itmRng); // <-- Add these
                            myListBox.Items.Add(itmRng);

                        }
                        rs1.MoveNext();
                    }

                }
                rs1.Close();
            }
            else
            {
                if (gbl_CURR_TAB == C_TAB_DSIDE)
                {
                    sSql = "Select A.FNO, A.FID, A.LO, A.HI, A.SRC_FNO, A.SRC_FID, A.SRC_LO, A.SRC_HI from table (SELECT AG_GET_D_UNASS_FPORTLIST(" + mFNO + ", " + mFID + ") from dual ) A";

                }
                else
                {
                    sSql = "Select A.FNO, A.FID, A.LO, A.HI, A.SRC_FNO, A.SRC_FID, A.SRC_LO, A.SRC_HI from table (SELECT AG_GET_E_UNASS_FCORELIST(" + mFNO + ", " + mFID + ") from dual ) A";
                }
                rs1 = m_IGTDataContext.OpenRecordset(sSql, ADODB.CursorTypeEnum.adOpenStatic, ADODB.LockTypeEnum.adLockReadOnly, 1);
                if (rs1.RecordCount > 0)
                {
                    myListBox.Items.Clear();
                    rs1.MoveFirst();
                    while (!rs1.EOF)
                    {
                        if (rs1.Fields["LO"].Value.ToString() != "-1")
                        {
                            //get Cable Code
                            itmRng = rs1.Fields["LO"].Value.ToString() + " ~ " + rs1.Fields["HI"].Value.ToString();
                            sFNO = rs1.Fields["SRC_FNO"].Value.ToString();
                            if (sFNO == SPLITTER_FNO.ToString()) sFName = Get_Value("Select g3e_username from g3e_feature where g3e_FNO = " + sFNO) + "#";
                            sFID = rs1.Fields["SRC_FID"].Value.ToString();
                            if (sFID.Trim() != "")
                            {
                                itmRng = itmRng + "   |  " + sFName + GetFeatCode(Convert.ToInt16(sFNO), sFID) + " (" + rs1.Fields["SRC_FID"].Value.ToString() + ") :";
                                itmRng = itmRng + rs1.Fields["SRC_LO"].Value.ToString() + " ~ " + rs1.Fields["SRC_HI"].Value.ToString();
                                //_items.Add(itmRng); // <-- Add these
                                myListBox.Items.Add(itmRng);
                            }
                        }
                        rs1.MoveNext();
                    }

                }
                rs1.Close();
            }

        }


        private bool HasStumpOrStubSymbol(int iFNO, int iFID, string sCoreStatus)
        {
            string sSql = string.Empty;
            ADODB.Recordset rs = new ADODB.Recordset();
            bool bStatus = false;
            sSql = "Select * from GC_SPLICE_CONNECT where G3E_FNO = " + iFNO.ToString() + " and G3E_FID = " + iFID.ToString() + " and CORE_STATUS = '" + sCoreStatus.ToString().Trim() + "'";
            rs = m_IGTDataContext.OpenRecordset(sSql, ADODB.CursorTypeEnum.adOpenStatic, ADODB.LockTypeEnum.adLockReadOnly, 1);
            if (rs.RecordCount > 0)
            {
                bStatus = true ;

            }
            rs.Close();
            return bStatus;
        }

        // Fibre Connection
        private void FiberConnect(IGTKeyObject objFrom, IGTKeyObject objNode, IGTKeyObject objTo, int LOW1, int HIGH1, int LOW2, int HIGH2, string CORE_STATUS)
        {
            try
            {
                string sSql = null;
                string EXC_ABB = null;
                string FEATURE_STATE = null;
                int recordsAffected = 0;
                int lastSEQ = 0;
                string SEQ = null;
                short G_CNO = 0;
                short L_CNO = 0;
                short S_CNO = 0;
                IGTTextPointGeometry oTextGeom;
                IGTOrientedPointGeometry oPointGeom;
                IGTVector oVector;
                IGTPoint oPointS;
                IGTPoint oPointL;
                string TERM_FID = string.Empty;
                string TERM_FNO = string.Empty;
                string TERM_LOW = string.Empty;
                string TERM_HIGH = string.Empty;
                string CABLE_CODE = string.Empty;

                int iRS = 0;

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
                    default:
                        G_CNO = 0;
                        L_CNO = 0;
                        S_CNO = 0;
                        break;
                }


                //save the TERM_xxx on EVERY Joint/FSplice and Device/Enclosure 17-Sep-2012 
                //objNode.FNO != 11800 &&
                //&& !(objFrom.FNO == 12300 && objTo.FNO == 7400)
                //as long as not Detail Feature
                GTCustomCommandModeless.m_oGTTransactionManager.Begin("SpliceConnect");
                oPointS = GTClassFactory.Create<IGTPoint>();
                oPointL = GTClassFactory.Create<IGTPoint>();
                IGTKeyObject OFet = GTClassFactory.Create<IGTApplication>().DataContext.OpenFeature(objNode.FNO, objNode.FID);

                if (objNode.FNO != ODF_FNO && objNode.FNO != FPATCH_FNO && objNode.FNO != FPP_FNO)
                {
                    try
                    {
                        sSql = "Select A.FNO1, A.FID1, A.LOW1, A.HIGH1, A.FNO2, A.FID2, A.LOW2, A.HIGH2, A.TERM_FNO, A.TERM_FID , A.TERM_LOW, A.TERM_HIGH from table (select AG_GET_CORECOUNT2 (" + objNode.FNO + ", " + objNode.FID + ", " + objFrom.FNO + ", " + objFrom.FID + ", " + LOW1 + ", " + HIGH1 + ", " + objTo.FNO + ", " + objTo.FID + ", " + LOW2 + ", " + HIGH2 + ") from dual ) A";
                        Recordset rs = GTClassFactory.Create<IGTApplication>().DataContext.Execute(sSql, out recordsAffected, (int)CommandTypeEnum.adCmdText, null);
                        if (rs.RecordCount > 0)
                        {
                            rs.MoveFirst();
                            
                            iRS = 0;
                            while (!rs.EOF)
                            {
                                // Check if Error
                                if (rs.Fields["TERM_FNO"].Value.ToString() == "-1")
                                {
                                    MessageBox.Show("CABLE or SPLICE connection Error, Possibly at <G3E_FID> returned in another field" + "\n" + sSql, C_PROG_NAME, MessageBoxButtons.OK, MessageBoxIcon.Information);
                                    return;
                                }

                                TERM_FID = rs.Fields["TERM_FID"].Value.ToString().Trim();
                                TERM_FNO = rs.Fields["TERM_FNO"].Value.ToString().Trim();
                                TERM_LOW = rs.Fields["TERM_LOW"].Value.ToString().Trim();
                                TERM_HIGH = rs.Fields["TERM_HIGH"].Value.ToString().Trim();

                                CABLE_CODE = "";
                                // if TERM FNO is Splitter then get the From or TO cable code.
                                if (TERM_FNO == SPLITTER_FNO.ToString())
                                {
                                    if (objFrom.FNO == SPLITTER_FNO)
                                    {
                                        CABLE_CODE = GetFeatCode(objTo.FNO, objTo.FID.ToString());
                                    }
                                    else
                                    {
                                        CABLE_CODE = GetFeatCode(objFrom.FNO, objFrom.FID.ToString());
                                    }
                                }
                                else
                                {
                                    CABLE_CODE = GetFeatCode( Convert.ToInt16(TERM_FNO), TERM_FID);
                                }
                               

                                //Update Splice Connect
                                if (OFet.Components.GetComponent(77).Recordset.EOF)
                                {
                                    lastSEQ = 1;

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
                                }

                                SEQ = lastSEQ.ToString();
                                OFet.Components.GetComponent(77).Recordset.AddNew("G3E_FNO", objNode.FNO);
                                OFet.Components.GetComponent(77).Recordset.Fields["SEQ"].Value = 1;
                                OFet.Components.GetComponent(77).Recordset.Fields["G3E_FNO"].Value = objNode.FNO;
                                OFet.Components.GetComponent(77).Recordset.Fields["G3E_FID"].Value = objNode.FID;
                                OFet.Components.GetComponent(77).Recordset.Fields["G3E_CNO"].Value = 77;
                                OFet.Components.GetComponent(77).Recordset.Fields["G3E_CID"].Value = SEQ;
                                OFet.Components.GetComponent(77).Recordset.Fields["FNO1"].Value = objFrom.FNO;
                                OFet.Components.GetComponent(77).Recordset.Fields["FID1"].Value = objFrom.FID;
                                OFet.Components.GetComponent(77).Recordset.Fields["LOW1"].Value = rs.Fields["LOW1"].Value.ToString().Trim();
                                OFet.Components.GetComponent(77).Recordset.Fields["HIGH1"].Value = rs.Fields["HIGH1"].Value.ToString().Trim();
                                OFet.Components.GetComponent(77).Recordset.Fields["FNO2"].Value = objTo.FNO;
                                OFet.Components.GetComponent(77).Recordset.Fields["FID2"].Value = objTo.FID;
                                OFet.Components.GetComponent(77).Recordset.Fields["LOW2"].Value = rs.Fields["LOW2"].Value.ToString().Trim();
                                OFet.Components.GetComponent(77).Recordset.Fields["HIGH2"].Value = rs.Fields["HIGH2"].Value.ToString().Trim();

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

                                // Temporarily also update this column before from TERM move to SRC Column change to all
                                if (TERM_FID != "") OFet.Components.GetComponent(77).Recordset.Update("SRC_FID", TERM_FID);
                                if (TERM_FNO != "") OFet.Components.GetComponent(77).Recordset.Update("SRC_FNO", TERM_FNO);
                                if (TERM_LOW != "") OFet.Components.GetComponent(77).Recordset.Update("SRC_LOW", TERM_LOW);
                                if (TERM_HIGH != "") OFet.Components.GetComponent(77).Recordset.Update("SRC_HIGH", TERM_HIGH);

                                iRS++;
                                rs.MoveNext();
                            }
                            rs.Close();
                        }


                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message + "\n" + sSql, "AG_GET_CORECOUNT", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }


                //Symbol
                if (objNode.FNO == FSLICE_FNO)
                {   // No Off-set for Symbol on Splice
                    oPointS.X = OFet.Components.GetComponent(S_CNO).Geometry.FirstPoint.X;
                    oPointS.Y = OFet.Components.GetComponent(S_CNO).Geometry.FirstPoint.Y;
                    oPointS.Z = OFet.Components.GetComponent(S_CNO).Geometry.FirstPoint.Z;

                    
                }
                else if (objNode.FNO == FDC_FNO)
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
                if (objNode.FNO == FSLICE_FNO)
                {
                    oPointL.X = OFet.Components.GetComponent(S_CNO).Geometry.FirstPoint.X - 7;
                    oPointL.Y = OFet.Components.GetComponent(S_CNO).Geometry.FirstPoint.Y - 3;
                    oPointL.Z = OFet.Components.GetComponent(S_CNO).Geometry.FirstPoint.Z;
                }
                else if (objNode.FNO == FDC_FNO)
                {
                    oPointL.X = OFet.Components.GetComponent(S_CNO).Geometry.FirstPoint.X - 8;
                    oPointL.Y = OFet.Components.GetComponent(S_CNO).Geometry.FirstPoint.Y - 1;
                    oPointL.Z = OFet.Components.GetComponent(S_CNO).Geometry.FirstPoint.Z;
                }
                else
                {
                    oPointL.X = OFet.Components.GetComponent(S_CNO).Geometry.FirstPoint.X;
                    oPointL.Y = OFet.Components.GetComponent(S_CNO).Geometry.FirstPoint.Y - 1;
                    oPointL.Z = OFet.Components.GetComponent(S_CNO).Geometry.FirstPoint.Z;
                }

                //Now Other Status -- Draw if SPARE or FOMS or STUMP
                //Core Count Symbol
                //Update CORE_STATUS to GC_XXX_S2 Component
                //if ((G_CNO == 11822 && CORE_STATUS == "SPARE") || (G_CNO == 11822 && CORE_STATUS == "FOMS") || (G_CNO == 11822 && CORE_STATUS == "STUMP") || G_CNO == 5622 || G_CNO == 5122)
                //if ((G_CNO == 11822 && CORE_STATUS == "SPARE") || (G_CNO == 11822 && CORE_STATUS == "STUMP") || G_CNO == 5622 || G_CNO == 5122)
                if (((CORE_STATUS == C_SPARE ) || (CORE_STATUS == C_STUMP) ) && ( G_CNO > 0))
                {
                    //If the feature itself already have Spare or Stump, then no need to create duplicate symbol
                    if (!HasStumpOrStubSymbol(objNode.FNO, objNode.FID, CORE_STATUS))
                    {
                        // Only if there is Connection is Spare or Stump and also Symbol CNO is not zero, then create point geometry
                        oPointGeom = GTClassFactory.Create<IGTOrientedPointGeometry>();
                        oPointGeom.Origin = oPointS;

                        oVector = GTClassFactory.Create<IGTVector>();
                        switch (CORE_STATUS)
                        {
                            //case C_FOMS:
                            //    oVector.I = 1;
                            //    oVector.J  = 1;
                            //    break;
                            case C_SPARE:
                                oVector.I = 1;
                                oVector.J = -1;
                                break;
                            case C_STUMP:
                                oVector.I = -1;
                                oVector.J = 1;
                                break;
                        }
                        oPointGeom.Orientation = oVector;

                        SEQ = "1";
                        int iCID = OFet.Components.GetComponent(G_CNO).Recordset.RecordCount;
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
                            iCID++;
                            OFet.Components.GetComponent(G_CNO).Recordset.AddNew("G3E_FID", objNode.FID);
                            OFet.Components.GetComponent(G_CNO).Recordset.Update("G3E_FNO", objNode.FNO);
                            OFet.Components.GetComponent(G_CNO).Recordset.Update("G3E_CID", iCID);
                            OFet.Components.GetComponent(G_CNO).Recordset.Update("CORE_STATUS", CORE_STATUS);
                            OFet.Components.GetComponent(G_CNO).Geometry = oPointGeom;
                        }
                    }
                }

                //Core Count Text
                //FSPLICE, not needed unless there is SPARE or STUMP
                if ((L_CNO != 11834 || (L_CNO == 11834 && CORE_STATUS == C_SPARE ) || (L_CNO == 11834 && CORE_STATUS == C_STUMP ) || (L_CNO == 11834 && CORE_STATUS == C_FOMS)))
                {
                    oTextGeom = GTClassFactory.Create<IGTTextPointGeometry>();
                    oTextGeom.Origin = oPointL;
                    oTextGeom.Rotation = 0;
                    if (SEQ == null) SEQ = "1";
                    int iLCID = OFet.Components.GetComponent(L_CNO).Recordset.RecordCount;
                    iLCID++;
                    if (iLCID <= 1)
                    {
                        OFet.Components.GetComponent(L_CNO).Recordset.AddNew("G3E_FID", objNode.FID);
                        OFet.Components.GetComponent(L_CNO).Recordset.Update("G3E_FNO", objNode.FNO);
                        OFet.Components.GetComponent(L_CNO).Recordset.Update("G3E_CID", iLCID);
                        OFet.Components.GetComponent(L_CNO).Geometry = oTextGeom;
                    }
                    else
                    {
                        OFet.Components.GetComponent(L_CNO).Recordset.MoveLast();
                        OFet.Components.GetComponent(L_CNO).Recordset.Update("G3E_FID", objNode.FID);
                        OFet.Components.GetComponent(L_CNO).Geometry = oTextGeom;
                    }
                }

                GTCustomCommandModeless.m_oGTTransactionManager.Commit();
            }
            catch (Exception ex)
            {
                GTCustomCommandModeless.m_oGTTransactionManager.Rollback();
                MessageBox.Show(ex.Message, C_PROG_NAME, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        //Get the Selected Feature
        private bool  Select_Feature()
        {
            try
            {
                IGTGeometry geom = null;
                short iFNO = 0;
                int iFID = 0;
                string MIN_MAT = null;
                bool bStatus = false;

                if (GTClassFactory.Create<IGTApplication>().SelectedObjects.FeatureCount == 0)
                {                    
                    MessageBox.Show("Please select a Device", C_PROG_NAME, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return bStatus;
                }

                foreach (IGTDDCKeyObject oDDCKeyObject in GTClassFactory.Create<IGTApplication>().SelectedObjects.GetObjects())
                {
                    geom = oDDCKeyObject.Geometry;
                    iFNO = oDDCKeyObject.FNO;
                    iFID = oDDCKeyObject.FID;

                    if (iFNO == FDC_FNO || iFNO == UPE_FNO || iFNO == FDP_FNO || iFNO == FTB_FNO || iFNO == TIE_FNO || iFNO == DB_FNO || iFNO == MSAN_FNO || iFNO == DDN_FNO || iFNO == NDH_FNO || iFNO == MUX_FNO || iFNO == EPE_FNO || iFNO == FAN_FNO || iFNO == RT_FNO || iFNO == VDSL_FNO || iFNO == ODF_FNO || iFNO == FPATCH_FNO || iFNO == FPP_FNO || iFNO == FSLICE_FNO || iFNO == FST_FNO || iFNO == FSJ_FNO)
                    {
                        lbl_FID.Text = iFID.ToString();
                        lbl_fno.Text = iFNO.ToString();

                        //if (!IsFOMSFDP(lbl_fno.Text.Trim(), lbl_FID.Text.Trim()))
                        //{
                        //    MIN_MAT = Get_Value("select MIN_MATERIAL from GC_NETELEM where G3E_FID = " + iFID.ToString());
                        //    if (MIN_MAT == "")
                        //    {
                        //        MessageBox.Show("Selected device does not have Plant Unit information, please reselect another device.", C_PROG_NAME, MessageBoxButtons.OK, MessageBoxIcon.Information);
                        //        return bStatus;
                        //    }
                        //}


                        //Get the Font Name, Letter and Size
                        Font font;
                        ADODB.Recordset rs = new ADODB.Recordset();
                        string iCNO = Get_Value("SELECT G3E_PRIMARYGEOGRAPHICCNO from G3E_FEATURE WHERE G3E_FNO = " + iFNO);
                        string sSql = "SELECT G3E_FONTNAME, G3E_SYMBOL, G3E_SIZE FROM G3E_POINTSTYLE WHERE G3E_SNO IN ( SELECT G3E_SNO FROM G3E_STYLERULE WHERE G3E_SRNO IN ( SELECT L.G3E_SRNO FROM G3E_COMPONENTVIEWDEFINITION CVD, G3E_LEGENDENTRY L WHERE G3E_FNO = " + iFNO + " AND G3E_CNO = " + iCNO + " AND L.G3E_LENO = CVD.G3E_LENO )AND g3e_stylerule.g3e_filter is null )";
                        rs = m_IGTDataContext.OpenRecordset(sSql, ADODB.CursorTypeEnum.adOpenStatic, ADODB.LockTypeEnum.adLockReadOnly, 1);
                        if (rs.RecordCount > 0)
                        {
                            rs.MoveFirst();
                            if (iCNO == "9520") // if Minimux symbol 
                            {
                                font = new Font(rs.Fields[0].Value.ToString(), float.Parse(rs.Fields[2].Value.ToString()) - 100);
                            }
                            else
                            {
                                // font = new Font(rs.Fields[0].Value.ToString(), float.Parse(rs.Fields[2].Value.ToString()));
                                font = new Font(rs.Fields[0].Value.ToString(), 35);
                            }

                            lbl_Symbol.Font = font;
                            lbl_Symbol.Text = rs.Fields[1].Value.ToString();
                        }
                        rs.Close();

                        lbl_Exch.Text = "Exchange : " + Get_Value("select EXC_ABB from gc_netelem where g3e_fid = " + iFID);
                        lbl_DeviceName.Text = GetFeatName(iFNO.ToString());
                        if (IsFOMSFDP(lbl_fno.Text.Trim(), lbl_FID.Text.Trim())) lbl_DeviceName.Text = lbl_DeviceName.Text + "[FOMS]";
                        lbl_Device.Text = "Code : " + GetFeatCode(iFNO, iFID.ToString()) + "[" + iFID.ToString() + "]";
                        
                    }
                    else
                    {
                        
                        MessageBox.Show("This selected feature is not a valid to edit splice connection with.", C_PROG_NAME, MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return bStatus;
                    }

                }

                bStatus = true;
                return bStatus;
            }
            catch (Exception ex)
            {
                log.WriteLog("");
                log.WriteLog(this.GetType().FullName, "ERROR: " + ex.Message);
                log.WriteLog(ex.StackTrace);
                MessageBox.Show(ex.Message, C_PROG_NAME, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

        }

        private void btn_Disconnect_Click(object sender, EventArgs e)
        {
            DialogResult retVal = MessageBox.Show("Are you sure to Disconnect the selected Rows?", "SpliceConnect", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (retVal == DialogResult.Yes)
            {
                Delete_Splice();
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
                            Delete_Splice();
                        }
                        else
                            gv_Route[2, e.RowIndex].Value = false;

                        break;

                    case 0:
                        //gv_Route[5, e.RowIndex].ReadOnly = false;
                        //gv_Route[6, e.RowIndex].ReadOnly = false;

                        //gv_Route[9, e.RowIndex].ReadOnly = false;
                        //gv_Route[10, e.RowIndex].ReadOnly = false; 


                        //SetComboValue(cboFrom, gv_Route[3, e.RowIndex].Value.ToString() + " - " + gv_Route[4, e.RowIndex].Value.ToString());

                        //txtLOW1.Text = gv_Route[5, e.RowIndex].Value.ToString();
                        //txtHIGH1.Text = gv_Route[6, e.RowIndex].Value.ToString();

                        //txtLOW2.Text = gv_Route[9, e.RowIndex].Value.ToString();
                        //txtHIGH2.Text = gv_Route[10, e.RowIndex].Value.ToString();

                        //edit_g3e_id = Convert.ToInt32(gv_Route[16, e.RowIndex].Value);

                        //string Name = gv_Route[7, e.RowIndex].Value.ToString() + " - " + gv_Route[8, e.RowIndex].Value.ToString();
                        //SetComboValue(cboTo, Name);

                        //cboFrom.Enabled = false;
                        //cboTo.Enabled = false;

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

        private string GetSymbolTableName(string FNO)
        {
            string G_CNO = string.Empty;

            //Delete Symbol and Label Component                
                switch (Convert.ToInt16(lbl_fno.Text))
                {
                    case FSLICE_FNO: //Splice
                        G_CNO = "GC_FSPLICE_S2";                        
                        break;
                    case FDC_FNO: //Fiber Distribution Cabinet 
                        G_CNO = "GC_FDC_S2";
                        break;
                    case EPE_FNO: //Edge Provider Edge
                        G_CNO = "GC_EPE_S2";
                        break;
                    case UPE_FNO: //User Provider Edge
                        G_CNO = "GC_UPE_S2";
                        break;
                    case ODF_FNO: //Optical Distribution Frame
                        G_CNO = "GC_ODF_S2";
                        break;
                    case FDP_FNO: //Fiber Distribution Point
                        G_CNO = "GC_FDP_S2";
                        break;
                    case TIE_FNO: //Tie FDP
                        G_CNO = "GC_TIEFDP_S2";
                        break;
                    case FTB_FNO: //FTB
                        G_CNO = "GC_FTB_S2";
                        break;
                    case MSAN_FNO: //MSAN
                        G_CNO = "GC_MSAN_S2";
                        break;
                    case DDN_FNO: //DDN
                        G_CNO = "GC_DDN_S2";
                        break;
                    case NDH_FNO: //NDH
                        G_CNO = "GC_NDH _S2";
                        break;
                    case MUX_FNO: //MINMUX
                        G_CNO = "GC_MINIMUX_S2";
                        break;
                    case RT_FNO: //Remote Terminal
                        G_CNO = "GC_RT_S2";
                        break;
                    case FAN_FNO: //Fiber Access Node
                        G_CNO = "GC_FAN_S2";
                        break;
                    case VDSL_FNO: //VDSL2
                        G_CNO = "GC_VDSL2_S2";
                        break;
                    case DB_FNO: //DB
                        G_CNO = "GC_DB_S2";
                        break;
                }

                return G_CNO;
        }

        private string GetLabelTableName(string FNO)
        {
            string L_CNO = string.Empty;

            //Delete Symbol and Label Component                
            switch (Convert.ToInt16(lbl_fno.Text))
            {
                case FSLICE_FNO: //Splice
                    L_CNO = "GC_FSPLICE_T3";
                    break;
                case FDC_FNO: //Fiber Distribution Cabinet 
                    L_CNO = "GC_FDC_T3";
                    break;
                case EPE_FNO: //Edge Provider Edge
                    L_CNO = "GC_EPE_T3";
                    break;
                case UPE_FNO: //User Provider Edge
                    L_CNO = "GC_UPE_T3";
                    break;
                case ODF_FNO: //Optical Distribution Frame
                    L_CNO = "GC_ODF_T3";
                    break;
                case FDP_FNO: //Fiber Distribution Point
                    L_CNO = "GC_FDP_T3";
                    break;
                case TIE_FNO: //Tie FDP
                    L_CNO = "GC_TIEFDP_T3";
                    break;
                case FTB_FNO: //FTB
                    L_CNO = "GC_FTB_T3";
                    break;
                case MSAN_FNO: //MSAN
                    L_CNO = "GC_MSAN_T3";
                    break;
                case DDN_FNO: //DDN
                    L_CNO = "GC_DDN_T3";
                    break;
                case NDH_FNO: //NDH
                    L_CNO = "GC_NDH_T3";
                    break;
                case MUX_FNO: //MINMUX
                    L_CNO = "GC_MINIMUX_T3";
                    break;
                case RT_FNO: //Remote Terminal
                    L_CNO = "GC_RT_T3";
                    break;
                case FAN_FNO: //Fiber Access Node
                    L_CNO = "GC_FAN_T3";
                    break;
                case VDSL_FNO: //VDSL2
                    L_CNO = "GC_VDSL2_T3";
                    break;
                case DB_FNO: //DB
                    L_CNO = "GC_DB_T3";
                    break;
            }
            return L_CNO;
        }

        private void Delete_Splice()
        {
            try
            {
                Recordset rsDel = null;
                string sSql = "";

                int iRecordNum = 0;
                object[] obj = null;

                string G3E_ID = null;
                int Splice_Cnt = 0;
                string Splice = null;
                string G_CNO = null;
                string L_CNO = null;
                short isL_CNO = 0;
                string sCoreStatus = string.Empty;
                string strActiveJob = GTClassFactory.Create<IGTApplication>().DataContext.ActiveJob;

                string PP_WO = strActiveJob;

                
                  
                for (int i = 0; i <= gv_Route.Rows.Count - 1; i++)
                {
                    if (Convert.ToBoolean(gv_Route.Rows[i].Cells[2].Value) == true)
                    {
                        // if Check delete the splice connect
                        G3E_ID = gv_Route.Rows[i].Cells["G3E_ID"].Value.ToString();
                        sCoreStatus = gv_Route.Rows[i].Cells["Conn_Type"].Value.ToString().Trim();
                        
                        GTCustomCommandModeless.m_oGTTransactionManager.Begin("SpliceDelete");
                        // if Core_status is not null then delete necessary component
                        sSql = "Delete from GC_SPLICE_CONNECT where g3e_id IN (" + G3E_ID + ")";
                        rsDel = m_IGTDataContext.Execute(sSql, out iRecordNum, (int)ADODB.CommandTypeEnum.adCmdText, obj);
                        GTCustomCommandModeless.m_oGTTransactionManager.Commit();

                        //Delete Symbol Component                
                        G_CNO = GetSymbolTableName(lbl_fno.Text.Trim());
                        L_CNO = GetLabelTableName(lbl_fno.Text.Trim());

                        Splice = Get_Value("Select count(*) as cnt from GC_SPLICE_CONNECT where G3E_FNO = " +
                            lbl_fno.Text + " and G3E_FID = " + lbl_FID.Text + " and CORE_STATUS = '" + sCoreStatus.Trim() + "'");
                        if (Splice.Trim() == "0")
                        {
                            if ((sCoreStatus == C_FOMS) || (sCoreStatus == C_STUMP) || (sCoreStatus == C_SPARE))
                            {
                                sSql = "Delete from " + G_CNO + " where CORE_STATUS = '" + sCoreStatus.Trim() + "' and g3e_fid = " + lbl_FID.Text;
                                rsDel = m_IGTDataContext.Execute(sSql, out iRecordNum, (int)ADODB.CommandTypeEnum.adCmdText, obj);
                            }
                        }

                        Splice = "";
                        //Delete Label if now more record left
                        Splice = Get_Value("Select count(*) as cnt from GC_SPLICE_CONNECT where CORE_STATUS IS NOT NULL AND G3E_FNO = " + lbl_fno.Text + " and G3E_FID = " + lbl_FID.Text);
                        if (Splice.Trim() == "0")
                        {
                            sSql = "Delete from " + L_CNO + " where g3e_fid = " + lbl_FID.Text;
                            rsDel = m_IGTDataContext.Execute(sSql, out iRecordNum, (int)ADODB.CommandTypeEnum.adCmdText, obj);
                        }
                        else
                        {
                            IGTTextPointGeometry oTextGeom;
                            IGTPoint oPointL;
                            oPointL = GTClassFactory.Create<IGTPoint>();
                            //refresh the label.                            
                            IGTKeyObject OFet = GTClassFactory.Create<IGTApplication>().DataContext.OpenFeature(short.Parse(lbl_fno.Text), int.Parse(lbl_FID.Text));
                            
                            isL_CNO = getCoreCountLabelCNO(Convert.ToInt16(lbl_fno.Text.Trim()));

                            oPointL.X = OFet.Components.GetComponent(isL_CNO).Geometry.FirstPoint.X;
                            oPointL.Y = OFet.Components.GetComponent(isL_CNO).Geometry.FirstPoint.Y+1;
                            oPointL.Z = OFet.Components.GetComponent(isL_CNO).Geometry.FirstPoint.Z;
                            oTextGeom = GTClassFactory.Create<IGTTextPointGeometry>(); 
                            oTextGeom.Origin = oPointL;
                            oTextGeom.Rotation = 0;

                            //sSql = "Delete from " + L_CNO + " where g3e_fid = " + lbl_FID.Text;
                            //rsDel = m_IGTDataContext.Execute(sSql, out iRecordNum, (int)ADODB.CommandTypeEnum.adCmdText, obj);
                            //GTCustomCommandModeless.m_oGTTransactionManager.Commit();

                            //OFet = GTClassFactory.Create<IGTApplication>().DataContext.OpenFeature(short.Parse(lbl_fno.Text), int.Parse(lbl_FID.Text));
                            int iCID = OFet.Components.GetComponent(isL_CNO).Recordset.RecordCount;
                            //iCID++;
                            if (iCID >= 1)
                            {
                               // OFet.Components.GetComponent(isL_CNO).Recordset.AddNew("G3E_FID", int.Parse(lbl_FID.Text));
                                OFet.Components.GetComponent(isL_CNO).Recordset.MoveLast();
                                OFet.Components.GetComponent(isL_CNO).Recordset.Update("G3E_FNO", short.Parse(lbl_fno.Text));
                                OFet.Components.GetComponent(isL_CNO).Recordset.Update("G3E_CID", iCID);
                                OFet.Components.GetComponent(isL_CNO).Geometry = oTextGeom; 
                            }
                           
                        }

                        GTCustomCommandModeless.m_oGTTransactionManager.Commit();
                        GTCustomCommandModeless.m_oGTTransactionManager.RefreshDatabaseChanges();
                        Splice_Cnt++;
                    }

                }


                if (Splice_Cnt <= 0 )
                {
                    MessageBox.Show("Please Select a Row to Disconnect.", C_PROG_NAME, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }


                lbl_MSG.Text = "Splice Connection Disconnected !";
                GTClassFactory.Create<IGTApplication>().SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Core Count.");

                //refresh the grid
                FillDataGrid();

                

            }
            catch (Exception e)
            {
                GTCustomCommandModeless.m_oGTTransactionManager.Rollback();
                MessageBox.Show(e.Message);
            }
        }

        private short getCoreCountLabelCNO(int iFNO)
        {
            short L_CNO = 0;
            switch (iFNO)
            {
                case 11800: //Stump
                        L_CNO = 11834;
                        break;
                    case 5100: //Fiber Distribution Cabinet 
                        L_CNO = 5134;
                        break;
                    case 5200: //Edge Provider Edge
                        L_CNO = 5234;
                        break;
                    case 5400: //User Provider Edge
                        L_CNO = 5434;
                        break;
                    case 5500: //Optical Distribution Frame
                        L_CNO = 5534;
                        break;
                    case 5600: //Fiber Distribution Point
                        L_CNO = 5634;
                        break;
                    case 5800: //Tie FDP
                        L_CNO = 5834;
                        break;
                    case 5900: //FTB
                        L_CNO = 5934;
                        break;
                    case 9100: //MSAN
                        L_CNO = 9134;
                        break;
                    case 9300: //DDN
                        L_CNO = 9334;
                        break;
                    case 9400: //NDH
                        L_CNO = 9434;
                        break;
                    case 9500: //MINMUX
                        L_CNO = 9534;
                        break;
                    case 9600: //Remote Terminal
                        L_CNO = 9634;
                        break;
                    case 9700: //Fiber Access Node
                        L_CNO = 9734;
                        break;
                    case 9800: //VDSL2
                        L_CNO = 9834;
                        break;
                    case 9900: //DB
                        L_CNO = 9934;
                        break;
                    default:
                        L_CNO = 0;
                        break;
                }
            return L_CNO;
        }
        private void  Delete_Splice0()
        {
            try
            {
                Recordset rsDel = null;
                string sSql = "";

                int iRecordNum = 0;
                object[] obj = null;

                string G3E_ID = null;
                int Splice_Cnt = 0;
                string Splice = null;
                string G_CNO = null;
                string L_CNO = null;

                string strActiveJob = GTClassFactory.Create<IGTApplication>().DataContext.ActiveJob;

                string PP_WO = strActiveJob;

                for (int i = 0; i <= gv_Route.Rows.Count - 1; i++)
                {
                    if (Convert.ToBoolean(gv_Route.Rows[i].Cells[2].Value) == true)
                    {
                        // if Check delete the splice connect

                        // if Core_status is not null then delete necessary component
                        G3E_ID = G3E_ID + gv_Route.Rows[i].Cells["G3E_ID"].Value.ToString() + ",";
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
                    MessageBox.Show("Please Select a Row to Disconnect.", C_PROG_NAME, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                GTCustomCommandModeless.m_oGTTransactionManager.Begin("SpliceConnect");
                sSql = "Delete from GC_SPLICE_CONNECT where g3e_id IN (" + G3E_ID + ")";
                rsDel = m_IGTDataContext.Execute(sSql, out iRecordNum, (int)ADODB.CommandTypeEnum.adCmdText, obj);
                GTCustomCommandModeless.m_oGTTransactionManager.Commit();

                //Delete Symbol and Label Component                
                G_CNO = GetSymbolTableName(lbl_fno.Text.Trim());
                L_CNO = GetLabelTableName(lbl_fno.Text.Trim());
               
                if (Splice_Cnt > 0)
                {
                    GTCustomCommandModeless.m_oGTTransactionManager.Begin("SpliceDelete");

                    Splice = Get_Value("Select count(*) as cnt from GC_SPLICE_CONNECT where G3E_FNO = " + lbl_fno.Text + " and G3E_FID = " + lbl_FID.Text);
                    if (Splice == "0")
                    {
                        sSql = "Delete from " + G_CNO + " where g3e_fid = " + lbl_FID.Text;
                        rsDel = m_IGTDataContext.Execute(sSql, out iRecordNum, (int)ADODB.CommandTypeEnum.adCmdText, obj);
                        sSql = "Delete from " + L_CNO + " where g3e_fid = " + lbl_FID.Text;
                        rsDel = m_IGTDataContext.Execute(sSql, out iRecordNum, (int)ADODB.CommandTypeEnum.adCmdText, obj);
                    }
                    GTCustomCommandModeless.m_oGTTransactionManager.Commit();
                }

                lbl_MSG.Text = "Splice Connection Disconnected !";
                GTClassFactory.Create<IGTApplication>().SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Core Count.");

                //refresh the grid
                FillDataGrid();

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
                MessageBox.Show(ex.Message, C_PROG_NAME, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }

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
        
        private void BtnSelect_Click(object sender, EventArgs e)
        {

            for (int i = 0; i < gv_Route.RowCount; i++)
            {
                gv_Route.Rows[i].Cells[2].Value = !Convert.ToBoolean(gv_Route.Rows[i].Cells[2].Value);
            }
        }

        
        private string GetComboValue(ComboBox cboList, string SelectValue)
        {
            if (SelectValue != null)
            {
                for (int i = 0; i < cboList.Items.Count; i++)
                {
                    if (SelectValue == ((cboItem)cboList.Items[i]).Name)
                    {
                        return ((cboItem)cboList.Items[i]).Value;
                    }
                }
            }
            return "";
        }

        private void SetComboValue(ComboBox cboList, string SelectValue)
        {
            for (int i = 0; i < cboList.Items.Count; i++)
            {
                if (SelectValue == ((cboItem)cboList.Items[i]).Name)
                {
                    cboList.SelectedIndex = i;
                }
                if (SelectValue == DBNull.Value.ToString())
                {
                    cboList.SelectedIndex = -1;

                }
            }

        }

        private int Get_Effective_UsedSize(string fid)
        {
            int recordsAffected = 0;
            string sSize = string.Empty;
            string sSql1 = "SELECT sum(high2-low2+1) FROM GC_SPLICE_CONNECT WHERE g3e_fno not in ("+ ODF_FNO.ToString() +") and fid2 IN (" + fid + ") and fid1 <>fid2";
            Recordset rsComp1 = GTClassFactory.Create<IGTApplication>().DataContext.Execute(sSql1, out recordsAffected, (int)CommandTypeEnum.adCmdText, null);
            if (rsComp1.RecordCount > 0)
            {
                rsComp1.MoveFirst();
                while (!rsComp1.EOF)
                {
                    if (rsComp1.Fields[0].Value.ToString() != "")
                    {
                        sSize = rsComp1.Fields[0].Value.ToString().Trim();
                    }
                    rsComp1.MoveNext();
                }
            }
            rsComp1.Close();
            return (sSize == "") ? 0 : Convert.ToInt16(sSize);

        }

        private int Get_Effective_UsedODFSize(string fid)
        {
            int recordsAffected = 0;
            string sSize = string.Empty;
            string sSql1 = string.Empty;
            string sFNO = getFNO(fid);
            if (sFNO == ODF_FNO.ToString())
            {
                sSql1 = "SELECT sum(high2-low2+1) FROM GC_SPLICE_CONNECT WHERE g3e_fno in (" + ODF_FNO.ToString() + ") and fid1 IN (" + fid + ") and fid1 <>fid2";
            }
            else
            {
                sSql1 = "SELECT sum(high2-low2+1) FROM GC_SPLICE_CONNECT WHERE g3e_fno in (" + ODF_FNO.ToString() + ") and fid2 IN (" + fid + ") and fid1 <>fid2";

            }
            Recordset rsComp1 = GTClassFactory.Create<IGTApplication>().DataContext.Execute(sSql1, out recordsAffected, (int)CommandTypeEnum.adCmdText, null);
            if (rsComp1.RecordCount > 0)
            {
                rsComp1.MoveFirst();
                while (!rsComp1.EOF)
                {
                    if (rsComp1.Fields[0].Value.ToString() != "")
                    {
                        sSize = rsComp1.Fields[0].Value.ToString().Trim();
                    }
                    rsComp1.MoveNext();
                }
            }
            rsComp1.Close();
            return (sSize == "") ? 0 : Convert.ToInt16(sSize);

        }

        private int Get_Effective_Size(string fid)
        {
            int recordsAffected = 0;
            string[] arCore = new string[2];
            char[] splitter = { ':' };
            string sFNO = getFNO(fid).Trim();
            int iFNO = (sFNO == "") ? 0 : Convert.ToInt16(sFNO);
            string sSize = string.Empty;
            
            switch (iFNO)
            {
                case SPLITTER_FNO:
                    string sSql1 = "SELECT SPLITTER_TYPE as Port FROM GC_FSPLITTER WHERE g3e_fid  IN ('" + fid + "')";
                    Recordset rsComp1 = GTClassFactory.Create<IGTApplication>().DataContext.Execute(sSql1, out recordsAffected, (int)CommandTypeEnum.adCmdText, null);
                    if (rsComp1.RecordCount > 0)
                    {
                        rsComp1.MoveFirst();
                        while (!rsComp1.EOF)
                        {
                            if (rsComp1.Fields[0].Value.ToString() != "")
                            {
                                arCore = rsComp1.Fields[0].Value.ToString().Split(splitter);
                                sSize = (gbl_CURR_TAB == C_TAB_DSIDE) ? arCore[1].ToString() : arCore[0].ToString();

                            }
                            rsComp1.MoveNext();
                        }
                    }
                    rsComp1.Close();
                    rsComp1 = null;
                    break;
                case SHELF_FNO:
                    sSize = Get_Value("select decode(NUM_CORE, null, 4, NUM_CORE) from GC_MINIMUX where g3e_fid = (SELECT G3E_FID FROM GC_OWNERSHIP WHERE G3E_ID = (SELECT OWNER1_ID FROM GC_OWNERSHIP WHERE G3E_FID =" + fid.ToString() + ")) ");
                    break;
                
                case RT_FNO:
                    sSize = Get_Value("Select DECODE(NUM_CORE,NULL, 4, NUM_CORE) from GC_RT where g3e_fid = " + fid.ToString());
                    break;
                case MSAN_FNO :
                    sSize = Get_Value("Select DECODE(NUM_CORE,NULL, 4, NUM_CORE) from GC_MSAN where g3e_fid = " + fid.ToString());
                        break;
                case VDSL_FNO:
                    sSize = Get_Value("Select DECODE(NUM_CORE,NULL, 4, NUM_CORE) from GC_VDSL2 where g3e_fid = " + fid.ToString());
                    break;
                case FECBL_FNO :
                    sSize = Get_Value("Select EFF_CORE from GC_FCBL where g3e_fid = " + fid.ToString());
                    break;
                case FDCBL_FNO :
                    sSize = Get_Value("Select CORE_NUM from GC_FDCBL where g3e_fid = " + fid.ToString());
                    break;
                case TRCBL_FNO  :
                    sSize = Get_Value("Select EFF_CORE from GC_FCBL_TRUNK where g3e_fid = " + fid.ToString());
                    break;
                case JNCBL_FNO :
                    sSize = Get_Value("Select EFF_CORE from GC_FCBL_JNT where g3e_fid = " + fid.ToString());
                    break;
                case ODF_FNO:
                    //sSize = Get_Value("Select decode(capacity, 300, 96*4, 600, 96*7, 900, 96*10, -1) result from gc_odf where g3e_fid = " + fid.ToString());
                    sSize = Get_Value("Select (HORZ_NUM * H_PORT_NUM) result from gc_odf where g3e_fid = " + fid.ToString());
                    break;
                default:
                    break; // do nothing
            }


           // get the used range
           return (sSize == "") ? 0 : Convert.ToInt16(sSize);

        }

        private void Get_Port_Size(string fid, Label port)
        {
            port.Text = Get_Effective_Size(fid).ToString();
        }

        private void Display_ODF_summary()
        {
            try
            {
                string src_fno = lbl_fno.Text.Trim();
                string src_fid = lbl_FID.Text.Trim();
                //Clear combobox also 
                lstODF.Items.Clear();
                lstODF_Taken.Items.Clear();

                //refreshODFUnAssignList(Convert.ToInt16(src_fno), Convert.ToInt32(src_fid), lstODF);

                DisplayAvailableODF(src_fno, src_fid, lstODF);
                DisplayUsedODF(src_fno, src_fid, lstODF_Taken);

            }
            catch (Exception ex)
            {
                //GTClassFactory.Create<IGTApplication>().EndWaitCursor();
                log.WriteLog("");
                log.WriteLog(this.GetType().FullName, "ERROR: " + ex.Message);
                log.WriteLog(ex.StackTrace);
                MessageBox.Show(ex.Message, C_PROG_NAME, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
           

        }

        private void Display_Port_Summary2()
        {
            int iCount = 0;
            lst_Port.Items.Clear();
            grpAvail1.Visible = false;
            grpAvail2.Visible = false;
            grpTaken1.Visible = false;
            grpTaken2.Visible = false;

            string src_fno = string.Empty;
            string src_fid = string.Empty;
            
            char[] splitter = { ':' };
            string sPort = string.Empty;
            if (lbl_FID.Text.Trim() == "") return;


            //1. Find Current Network's Source Cable D-Side
            ADODB.Recordset rsPort = new ADODB.Recordset();

            string SSQL = string.Empty;
            //Check if FDC Its, else have to trace to FDC and get the Splitter
            if (lbl_fno.Text.Trim() == FDC_FNO.ToString())
            {
                SSQL = "select G3E_FID, g3e_fno from gc_ownership where owner1_id = (select G3E_ID from gc_ownership where g3e_fid =" + lbl_FID.Text.Trim() + ")";
            }
            else
            {
                SSQL = "select G3E_FID, g3e_fno from gc_ownership where owner1_id = (";
                SSQL = SSQL + "select G3E_ID from gc_ownership where g3e_fid =";
                SSQL = SSQL + "(select distinct in_fid from (select IN_FNO, IN_FID, G3E_FNO, G3E_FID, OUT_FNO, OUT_FID  from ";
                SSQL = SSQL + "GC_NR_CONNECT  start with out_fid = " + lbl_FID.Text.Trim() + " connect by nocycle prior in_fid = out_fid ";
                SSQL = SSQL + "and G3E_FNO in (7400)) where in_fno = 5100))";
            }

            //Clear combobox also 
            lst_Port.Items.Clear();
            lst_Port_Taken.Items.Clear();

            rsPort = m_IGTDataContext.OpenRecordset(SSQL, ADODB.CursorTypeEnum.adOpenStatic, ADODB.LockTypeEnum.adLockReadOnly, 1);
            if (rsPort.RecordCount > 0)
            {
                rsPort.MoveFirst();
                while (!rsPort.EOF)
                {
                    // Load the Splitter into Combo
                    src_fno = rsPort.Fields["g3e_fno"].Value.ToString();
                    src_fid = rsPort.Fields["g3e_fid"].Value.ToString();
                    sPort = Get_Value("select SPLITTER_TYPE from GC_FSPLITTER where g3e_fid = " + src_fid);
                    
                    DisplayAvailablePort(src_fno, src_fid, lst_Port);
                    DisplayUsedPort(src_fno, src_fid, lst_Port_Taken);
                    iCount++;
                    rsPort.MoveNext();
                }
            }
            rsPort.Close();
        }

        private int GetTotalEffectivePort(string sFNO, string sFID)
        {
            string src_fno = string.Empty;
            string src_fid = string.Empty; 
            int iTotalPort = 0;
            string sPort = string.Empty;
            string[] arPort = new string[2];
            char[] splitter = { ':' };
            
            //1. Find Current Network's Source Cable D-Side
            ADODB.Recordset rsPort = new ADODB.Recordset();

            string SSQL = string.Empty;
            //Check if FDC Its, else have to trace to FDC and get the Splitter
            if (sFNO.Trim() == FDC_FNO.ToString())
            {
                SSQL = "select G3E_FID, g3e_fno from gc_ownership where owner1_id = (select G3E_ID from gc_ownership where g3e_fid =" + sFID + ")";
            }
            else
            {
                SSQL = "select G3E_FID, g3e_fno from gc_ownership where owner1_id = (";
                SSQL = SSQL + "select G3E_ID from gc_ownership where g3e_fid =";
                SSQL = SSQL + "(select distinct in_fid from (select IN_FNO, IN_FID, G3E_FNO, G3E_FID, OUT_FNO, OUT_FID  from ";
                SSQL = SSQL + "GC_NR_CONNECT  start with out_fid = " + sFID + " connect by nocycle prior in_fid = out_fid ";
                SSQL = SSQL + "and G3E_FNO in (" + FDCBL_FNO.ToString() + ")) where in_fno = " + FDC_FNO.ToString() + "))";
            }
            rsPort = m_IGTDataContext.OpenRecordset(SSQL, ADODB.CursorTypeEnum.adOpenStatic, ADODB.LockTypeEnum.adLockReadOnly, 1);
            if (rsPort.RecordCount > 0)
            {
                rsPort.MoveFirst();
                while (!rsPort.EOF)
                {
                    // Load the Splitter into Combo
                    src_fno = rsPort.Fields["g3e_fno"].Value.ToString();
                    src_fid = rsPort.Fields["g3e_fid"].Value.ToString();
                    sPort = Get_Value("select SPLITTER_TYPE from GC_FSPLITTER where g3e_fid = " + src_fid);

                    //Get Total Port from all Splitter
                    arPort = sPort.Split(splitter);
                    int iEffPort = (arPort[1].ToString().Trim() == "") ? 0 : Convert.ToInt16(arPort[1].ToString());
                    iTotalPort = iTotalPort + iEffPort;

                    rsPort.MoveNext();
                }
            }
            rsPort.Close();

            return iTotalPort;
        }

        private int GetStartingPortBySplitterNo(string sFNO, string sFID, int iSplitterNo)
        {
            string src_fno = string.Empty;
            string src_fid = string.Empty;
            int iTotalPort = 0;
            string sPort = string.Empty;
            string[] arPort = new string[2];
            char[] splitter = { ':' };
            int iCurrSplitterNo = 1;
            int iStartValue = 1;


            if (iSplitterNo <= 1) return iStartValue;

            //1. Find Current Network's Source Cable D-Side
            ADODB.Recordset rsPort = new ADODB.Recordset();

            string SSQL = string.Empty;
            //Check if FDC Its, else have to trace to FDC and get the Splitter
           SSQL = "select G3E_FID, g3e_fno from gc_ownership where owner1_id = (select owner1_ID from gc_ownership where g3e_fid =" + sFID + ")";
           
            rsPort = m_IGTDataContext.OpenRecordset(SSQL, ADODB.CursorTypeEnum.adOpenStatic, ADODB.LockTypeEnum.adLockReadOnly, 1);
            if (rsPort.RecordCount > 0)
            {
                rsPort.MoveFirst();
                while (!rsPort.EOF)
                {
                    if (iCurrSplitterNo == iSplitterNo) break;
                    // Load the Splitter into Combo
                    src_fno = rsPort.Fields["g3e_fno"].Value.ToString();
                    src_fid = rsPort.Fields["g3e_fid"].Value.ToString();
                    sPort = Get_Value("select SPLITTER_TYPE from GC_FSPLITTER where g3e_fid = " + src_fid);

                    //Get Total Port from all Splitter
                    arPort = sPort.Split(splitter);
                    int iEffPort = (arPort[1].ToString().Trim() == "") ? 0 : Convert.ToInt16(arPort[1].ToString());
                    iTotalPort = iTotalPort + iEffPort;
                    iCurrSplitterNo++;
                    rsPort.MoveNext();
                }
            }
            rsPort.Close();

            return iTotalPort+1;
        }

        private void Display_Port_Summary()
        {
            grpAvail1.Visible = false;
            grpAvail2.Visible = false;
            grpTaken1.Visible = false;
            grpTaken2.Visible = false;
            string src_fno = string.Empty;
            string src_fid = string.Empty;

            int iTotalPort = 0;
            if (lbl_FID.Text.Trim() == "") return;

            //Clear combobox also 
            lst_Port.Items.Clear();
            lst_Port_Taken.Items.Clear();

            //1. Find Current Network's Source Cable D-Side
            ADODB.Recordset rsPort = new ADODB.Recordset();

            string SSQL = string.Empty;
            //Check if FDC Its, else have to trace to FDC and get the Splitter
            if (lbl_fno.Text.Trim() == FDC_FNO.ToString())
            {
                SSQL = "select G3E_FID, g3e_fno from gc_ownership where owner1_id = (select G3E_ID from gc_ownership where g3e_fid =" + lbl_FID.Text.Trim() + ")";
            }
            else
            {
                SSQL = "select G3E_FID, g3e_fno from gc_ownership where owner1_id = (";
                SSQL = SSQL + "select G3E_ID from gc_ownership where g3e_fid =";
                SSQL = SSQL + "(select distinct in_fid from (select IN_FNO, IN_FID, G3E_FNO, G3E_FID, OUT_FNO, OUT_FID  from ";
                SSQL = SSQL + "GC_NR_CONNECT  start with out_fid = " + lbl_FID.Text.Trim() + " connect by nocycle prior in_fid = out_fid ";
                SSQL = SSQL + "and G3E_FNO in (7400)) where in_fno = 5100))";
            }
            rsPort = m_IGTDataContext.OpenRecordset(SSQL, ADODB.CursorTypeEnum.adOpenStatic, ADODB.LockTypeEnum.adLockReadOnly, 1);
            if (rsPort.RecordCount > 0)
            {
                rsPort.MoveFirst();
                while (!rsPort.EOF)
                {
                    // Load the Splitter into Combo
                    src_fno = rsPort.Fields["g3e_fno"].Value.ToString();
                    src_fid = rsPort.Fields["g3e_fid"].Value.ToString();
                    
                    DisplayAvailablePort(src_fno, src_fid, lst_Port);
                    DisplayUsedPort(src_fno, src_fid, lst_Port_Taken);
                    rsPort.MoveNext();
                }
            }
            rsPort.Close();
            
                       
        }

        
        private void Display_Core_Summary()
        {
            int iCount = 0;
            lst_Core1.Items.Clear();
            lst_Core2.Items.Clear();
            lbl_SrcCore1.Text = "";
            lbl_SrcCore2.Text = "";
            grpAvail1.Visible = false;
            grpAvail2.Visible = false;
            grpTaken1.Visible = false;
            grpTaken2.Visible = false;

            string src_fno = string.Empty;
            string src_fid = string.Empty;
            string feat_desc = string.Empty;

            if (lbl_FID.Text.Trim() == "") return;
            //1. Find Current Network's Source Cable E-Side
            ADODB.Recordset rsCore = new ADODB.Recordset();
            string SSQL = string.Empty;

            //If FDC make sure is assignment
            if (lbl_fno.Text.Trim() == FDC_FNO.ToString().Trim())
            {
                SSQL = "select distinct term_FNO as g3e_fno, term_fid as g3e_fid from gc_splice_connect where g3e_fid =" + lbl_FID.Text.Trim();
                SSQL = SSQL + " and term_fno in (7200,7400, 4400,4500)";
            }
            else
            {
                SSQL = "select distinct g3e_fno, g3e_fid from (select IN_FNO, IN_FID, G3E_FNO, G3E_FID, OUT_FNO, OUT_FID  from ";
                SSQL = SSQL + " GC_NR_CONNECT  start with out_fid = " + lbl_FID.Text.Trim() + " connect by nocycle prior in_fid = out_fid ";
                SSQL = SSQL + " and G3E_FNO in (7200,7400, 4400,4500)) where in_fno in (5500,4900,12200) or out_fno in (5500,4900,12200)";
            }
            rsCore = m_IGTDataContext.OpenRecordset(SSQL, ADODB.CursorTypeEnum.adOpenStatic, ADODB.LockTypeEnum.adLockReadOnly, 1);
            //Recordset rsC = GTClassFactory.Create<IGTApplication>().DataContext.Execute(SSQL, out recordsAffected, (int)CommandTypeEnum.adCmdText, null);
            if (rsCore.RecordCount > 0)
            {
                rsCore.MoveFirst();
                while (!rsCore.EOF)
                {
                    src_fno = rsCore.Fields["g3e_fno"].Value.ToString();
                    src_fid = rsCore.Fields["g3e_fid"].Value.ToString();
                    feat_desc = GetFeatCode(Convert.ToInt16(src_fno), src_fid) + " (" + GetEffectiveCore(Convert.ToInt16(src_fno), src_fid) + " ) - " + "[ " + src_fid + " ]";

                    if (iCount == 0)
                    {
                        lbl_SrcCore1.Text = feat_desc;
                        grpAvail1.Visible = true;
                        grpTaken1.Visible = true;
                        //2. List out with Summary in List first.
                        DisplayAvailableCore(src_fno, src_fid, lst_Core1);
                        DisplayUsedCore(src_fno, src_fid, lst_Core1_Taken);
                    }
                    else
                    {
                        lbl_SrcCore2.Text = feat_desc;
                        grpAvail2.Visible = true;
                        grpTaken2.Visible = true;
                        //2. List out with Summary in List first.
                        DisplayAvailableCore(src_fno, src_fid, lst_Core2);
                        DisplayUsedCore(src_fno, src_fid, lst_Core2_Taken);
                    }

                    iCount++;
                    rsCore.MoveNext();
                }
            }
            rsCore.Close();
        }

        private int GetEffectiveCore(int fno, string fid)
        {
            int iNumCore = 0;
            string sCore = string.Empty;

            if (fno == FECBL_FNO)
                sCore = Get_Value("select EFF_CORE from GC_fcbl where g3e_fid = " + fid);
            else if (fno == FDCBL_FNO)
                sCore = Get_Value("select CORE_NUM from GC_FDCBL where g3e_fid = " + fid);
            else if (fno == TRCBL_FNO)
                sCore = Get_Value("select EFF_CORE from GC_FCBL_TRUNK where g3e_fid = " + fid);
            else if (fno == JNCBL_FNO)
                sCore = Get_Value("select EFF_CORE from GC_FCBL_JNT where g3e_fid = " + fid);
            else if (fno == ODF_FNO)
                //sCore = Get_Value("select decode(capacity, 300, 96*4, 600, 96*7, 900, 96*10, -1) result from gc_odf where g3e_fid = " + fid);
                sCore = Get_Value("select (HORZ_NUM * H_PORT_NUM) result from gc_odf where g3e_fid = " + fid);
            else
                sCore = "";

            iNumCore = ((sCore == "") ? 0 : Convert.ToInt16(sCore));
            return iNumCore;
        }

        private void DisplayAvailableODF(string mFNO, string mFID, ListBox myListBox)
        {
            ArrayList LstAvailCore = new ArrayList();

            try {

                string itmRng = string.Empty;
                if (mFNO != ODF_FNO.ToString()) return;

                int iEffCore = GetEffectiveCore(Convert.ToInt16(mFNO), mFID);

                for (int i = 1; i <= iEffCore; i++)
                {
                    LstAvailCore.Add(i);
                }

                ADODB.Recordset rs1 = new ADODB.Recordset();
                string sSql = string.Empty;
                int iTermLo = 0;
                int iTermHi = 0;
                sSql = "select term_low, term_high from gc_splice_connect where term_fid = " + mFID;
                sSql = sSql + " order by term_low";
                rs1 = m_IGTDataContext.OpenRecordset(sSql, ADODB.CursorTypeEnum.adOpenStatic, ADODB.LockTypeEnum.adLockReadOnly, 1);
                if (rs1.RecordCount > 0)
                {
                    //myListBox.Items.Clear();
                    rs1.MoveFirst();
                    while (!rs1.EOF)
                    {
                        iTermLo = (rs1.Fields["term_low"].Value.ToString().Trim() == "") ? 0 : Convert.ToInt16(rs1.Fields["term_low"].Value.ToString());
                        iTermHi = (rs1.Fields["term_high"].Value.ToString().Trim() == "") ? 0 : Convert.ToInt16(rs1.Fields["term_high"].Value.ToString());

                        if ((iTermLo > 0) && (iTermHi > 0))
                        {
                            for (int ind = iTermLo; ind <= iTermHi; ind++)
                            {
                                LstAvailCore.Remove(ind);
                            }
                        }

                        rs1.MoveNext();
                    }

                }
                rs1.Close();

                if (LstAvailCore.Count > 0)
                {
                    myListBox.Items.Clear();
                    foreach (int iCore in LstAvailCore)
                    {
                        myListBox.Items.Add(iCore.ToString());
                    }
                }

            }
            catch (Exception ex)
            {
                //GTClassFactory.Create<IGTApplication>().EndWaitCursor();
                log.WriteLog("");
                log.WriteLog(this.GetType().FullName, "ERROR: " + ex.Message);
                log.WriteLog(ex.StackTrace);
                MessageBox.Show(ex.Message, C_PROG_NAME, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //private void DisplayAvailablePort2(string mFNO, string mFID, ListBox myListBox)
        //{
        //    ArrayList LstAvailPort = new ArrayList();

        //    string itmRng = string.Empty;
        //    string sPort = string.Empty;
        //    string[] arPort = new string[2];
        //    char[] splitter = { ':' };
        //    string SplitterNo = string.Empty;
        //    int iVirtualNo = 0;
        //    int iSplitterNo = 0;

        //    if (mFNO != SPLITTER_FNO.ToString()) return;

        //    //get Port No 
        //    SplitterNo = Get_Value("select NO_OF_SPLITTER from GC_FSPLITTER where g3e_fid = " + mFID);
        //    iSplitterNo = (SplitterNo.Trim() == "") ? 0 : Convert.ToInt16(SplitterNo);
        //    sPort = Get_Value("select SPLITTER_TYPE from GC_FSPLITTER where g3e_fid = " + mFID);
        //    arPort = sPort.Split(splitter);
        //    int iEffPort = (arPort[1].ToString().Trim() == "") ? 0 : Convert.ToInt16(arPort[1].ToString());

        //    for (int i = 1; i <= iEffPort; i++)
        //    {
        //        iVirtualNo = ((iSplitterNo - 1) * iEffPort) + i;
        //        LstAvailPort.Add(iVirtualNo);
        //    }

        //    ADODB.Recordset rs1 = new ADODB.Recordset();
        //    string sSql = string.Empty;
        //    int iTermLo = 0;
        //    int iTermHi = 0;
        //    sSql = "select term_low, term_high, CORE_STATUS from gc_splice_connect where term_fid = " + mFID;
        //    sSql = sSql + " and CORE_STATUS in ('MAIN','PROTECTION','SPARE','STUMP','FOMS') order by term_low";
        //    rs1 = m_IGTDataContext.OpenRecordset(sSql, ADODB.CursorTypeEnum.adOpenStatic, ADODB.LockTypeEnum.adLockReadOnly, 1);
        //    if (rs1.RecordCount > 0)
        //    {
        //        //myListBox.Items.Clear();
        //        rs1.MoveFirst();
        //        while (!rs1.EOF)
        //        {
        //            iTermLo = (rs1.Fields["term_low"].Value.ToString().Trim() == "") ? 0 : Convert.ToInt16(rs1.Fields["term_low"].Value.ToString());
        //            iTermHi = (rs1.Fields["term_high"].Value.ToString().Trim() == "") ? 0 : Convert.ToInt16(rs1.Fields["term_high"].Value.ToString());

        //            if ((iTermLo > 0) && (iTermHi > 0))
        //            {
        //                for (int ind = iTermLo; ind <= iTermHi; ind++)
        //                {
        //                    iVirtualNo = ((iSplitterNo - 1) * iEffPort) + ind;
        //                    LstAvailPort.Remove(iVirtualNo);
        //                }
        //            }

        //            rs1.MoveNext();
        //        }

        //    }
        //    //rs1.Close();

        //    if (LstAvailPort.Count > 0)
        //    {
        //        //myListBox.Items.Clear();
        //        foreach (int iPort in LstAvailPort)
        //        {
        //            myListBox.Items.Add(iPort.ToString());
        //        }
        //    }
        //}

        private void DisplayAvailablePort(string mFNO, string mFID, ListBox myListBox)
        {
            ArrayList LstAvailPort = new ArrayList();

            string itmRng = string.Empty;
            string sPort = string.Empty;
            string[] arPort = new string[2];
            char[] splitter = { ':' };
            string SplitterNo = string.Empty;
            int iVirtualNo = 0;
            int iStartingNo = 0;
            int iSplitterNo = 0;
            
            if (mFNO != SPLITTER_FNO.ToString()) return;

            //get Port No 
            SplitterNo = Get_Value("select NO_OF_SPLITTER from GC_FSPLITTER where g3e_fid = " + mFID);
            iSplitterNo = (SplitterNo.Trim() == "") ? 0 : Convert.ToInt16(SplitterNo);
            iStartingNo = GetStartingPortBySplitterNo(mFNO, mFID, iSplitterNo);
            sPort = Get_Value("select SPLITTER_TYPE from GC_FSPLITTER where g3e_fid = " + mFID);
            arPort = sPort.Split(splitter);
            int iEffPort = (arPort[1].ToString().Trim() == "") ? 0 : Convert.ToInt16(arPort[1].ToString());


            for (int i = iStartingNo; i <= (iStartingNo + iEffPort - 1); i++)
            {
                LstAvailPort.Add(i);
            }

            ADODB.Recordset rs1 = new ADODB.Recordset();
            string sSql = string.Empty;
            int iTermLo = 0;
            int iTermHi = 0;
            sSql = "select term_low, term_high, CORE_STATUS from gc_splice_connect where term_fid = " + mFID;
            sSql = sSql + " and CORE_STATUS in ('MAIN','PROTECTION','SPARE','STUMP','FOMS') order by term_low";
            rs1 = m_IGTDataContext.OpenRecordset(sSql, ADODB.CursorTypeEnum.adOpenStatic, ADODB.LockTypeEnum.adLockReadOnly, 1);
            if (rs1.RecordCount > 0)
            {
                //myListBox.Items.Clear();
                rs1.MoveFirst();
                while (!rs1.EOF)
                {

                    iTermLo = (rs1.Fields["term_low"].Value.ToString().Trim() == "") ? 0 : Convert.ToInt16(rs1.Fields["term_low"].Value.ToString());
                    iTermHi = (rs1.Fields["term_high"].Value.ToString().Trim() == "") ? 0 : Convert.ToInt16(rs1.Fields["term_high"].Value.ToString());
                     if ((iTermLo > 0) && (iTermHi > 0))
                    {
                        for (int ind = iTermLo; ind <= iTermHi; ind++)
                        {
                            iVirtualNo = ind + (iStartingNo - iTermLo);
                            LstAvailPort.Remove(iVirtualNo);
                        }
                    }

                    rs1.MoveNext();
                }

            }
            rs1.Close();

            if (LstAvailPort.Count > 0)
            {
                //myListBox.Items.Clear();
                foreach (int iPort in LstAvailPort)
                {
                    myListBox.Items.Add(iPort.ToString());
                }
            }
        }

        private void DisplayAvailableCore(string mFNO, string mFID, ListBox myListBox)
        {
            ArrayList LstAvailCore = new ArrayList();

            string itmRng = string.Empty;
            if (mFNO != FECBL_FNO.ToString()) return;

            int iEffCore = GetEffectiveCore(Convert.ToInt16(mFNO), mFID);

            for (int i = 1; i <= iEffCore; i++)
            {
                LstAvailCore.Add(i);
            }

            ADODB.Recordset rs1 = new ADODB.Recordset();
            string sSql = string.Empty;
            int iTermLo = 0;
            int iTermHi = 0;
            sSql = "select term_low, term_high, CORE_STATUS from gc_splice_connect where term_fid = " + mFID;
            sSql = sSql + " and CORE_STATUS in ('MAIN','PROTECTION','SPARE','STUMP','FOMS', '" + C_FOMS_MAIN + "', '"+ C_FOMS_PROTECTION +"') order by term_low";
            rs1 = m_IGTDataContext.OpenRecordset(sSql, ADODB.CursorTypeEnum.adOpenStatic, ADODB.LockTypeEnum.adLockReadOnly, 1);
            if (rs1.RecordCount > 0)
            {
                //myListBox.Items.Clear();
                rs1.MoveFirst();
                while (!rs1.EOF)
                {
                    iTermLo = (rs1.Fields["term_low"].Value.ToString().Trim() == "") ? 0 : Convert.ToInt16(rs1.Fields["term_low"].Value.ToString());
                    iTermHi = (rs1.Fields["term_high"].Value.ToString().Trim() == "") ? 0 : Convert.ToInt16(rs1.Fields["term_high"].Value.ToString());

                    if ((iTermLo > 0) && (iTermHi > 0))
                    {
                        for (int ind = iTermLo; ind <= iTermHi; ind++)
                        {
                            LstAvailCore.Remove(ind);
                        }
                    }

                    rs1.MoveNext();
                }

            }
            rs1.Close();

            if (LstAvailCore.Count > 0)
            {
                myListBox.Items.Clear();
                foreach (int iCore in LstAvailCore)
                {
                    myListBox.Items.Add(iCore.ToString());
                }
            }
        }

        private void DisplayUsedPort(string mFNO, string mFID, ListBox myListBox)
        {
            string itmRng = string.Empty;
            string sPort = string.Empty;
            string[] arPort = new string[2];
            char[] splitter = { ':' };
            string SplitterNo = string.Empty;
            int iVirtualNo = 0;
            int iSplitterNo = 0;
            string STermFeatDesc = string.Empty;
            int iNodeFNO = 0;
            int iNodeFID = 0;
            int iStartingNo = 0;
            
            if (mFNO != SPLITTER_FNO.ToString()) return;
            //get Port No 
            SplitterNo = Get_Value("select NO_OF_SPLITTER from GC_FSPLITTER where g3e_fid = " + mFID);
            iSplitterNo = (SplitterNo.Trim() == "") ? 0 : Convert.ToInt16(SplitterNo);
            iStartingNo = GetStartingPortBySplitterNo(mFNO, mFID, iSplitterNo);
             

            ADODB.Recordset rs1 = new ADODB.Recordset();
            string sSql = string.Empty;
            int iTermLo = 0;
            int iTermHi = 0;
            string sStatus = string.Empty;
            //myListBox.Items.Clear();

            sSql = "select distinct term_low, term_high, CORE_STATUS , g3e_fno, g3e_fid from gc_splice_connect where term_fid = " + mFID;
            sSql = sSql + " and CORE_STATUS in ('MAIN','PROTECTION','SPARE','STUMP','FOMS') order by term_low";
            rs1 = m_IGTDataContext.OpenRecordset(sSql, ADODB.CursorTypeEnum.adOpenStatic, ADODB.LockTypeEnum.adLockReadOnly, 1);
            if (rs1.RecordCount > 0)
            {
                //myListBox.Items.Clear();
                rs1.MoveFirst();
                while (!rs1.EOF)
                {
                    iTermLo = (rs1.Fields["term_low"].Value.ToString().Trim() == "") ? 0 : Convert.ToInt16(rs1.Fields["term_low"].Value.ToString());
                    iTermHi = (rs1.Fields["term_high"].Value.ToString().Trim() == "") ? 0 : Convert.ToInt16(rs1.Fields["term_high"].Value.ToString());
                    sStatus = rs1.Fields["CORE_STATUS"].Value.ToString().Trim();
                    iNodeFNO = (rs1.Fields["g3e_fno"].Value.ToString().Trim() == "") ? 0 : Convert.ToInt16(rs1.Fields["g3e_fno"].Value.ToString());
                    iNodeFID = (rs1.Fields["g3e_fid"].Value.ToString().Trim() == "") ? 0 : Convert.ToInt32(rs1.Fields["g3e_fid"].Value.ToString());
                    if ((iTermLo > 0) && (iTermHi > 0))
                    {
                        for (int ind = iTermLo; ind <= iTermHi; ind++)
                        {
                            iVirtualNo = ind + (iStartingNo - iTermLo);
                            //get Feature Code
                            STermFeatDesc = "";
                            if ((sStatus == C_FOMS) || (sStatus == C_MAIN) || (sStatus == C_PROCTECTION))
                            {
                                STermFeatDesc = " [ " + GetFeatCode(iNodeFNO, iNodeFID.ToString()) + " - " + GetFeatName(iNodeFNO.ToString()) + " ]";
                            }
                            myListBox.Items.Add(iVirtualNo + ", " + sStatus + STermFeatDesc);
                        }
                    }

                    rs1.MoveNext();
                }

            }
            rs1.Close();


        }

        //private void DisplayUsedPort0(string mFNO, string mFID, ListBox myListBox)
        //{
        //    string itmRng = string.Empty;
        //    string sPort = string.Empty;
        //    string[] arPort = new string[2];
        //    char[] splitter = { ':' };
        //    string SplitterNo = string.Empty;
        //    int iVirtualNo = 0;
        //    int iSplitterNo = 0;
        //    string STermFeatDesc = string.Empty;
        //    int iNodeFNO = 0;
        //    int iNodeFID = 0;

        //    if (mFNO != SPLITTER_FNO.ToString()) return;
        //    //get Port No 
        //    SplitterNo = Get_Value("select NO_OF_SPLITTER from GC_FSPLITTER where g3e_fid = " + mFID);
        //    iSplitterNo = (SplitterNo.Trim() == "") ? 0 : Convert.ToInt16(SplitterNo);
            
        //    ADODB.Recordset rs1 = new ADODB.Recordset();
        //    string sSql = string.Empty;
        //    int iTermLo = 0;
        //    int iTermHi = 0;
        //    string sStatus = string.Empty;
        //    //myListBox.Items.Clear();

        //    sSql = "select distinct term_low, term_high, CORE_STATUS , g3e_fno, g3e_fid from gc_splice_connect where term_FID = " + mFID;
        //    sSql = sSql + " and CORE_STATUS in ('MAIN','PROTECTION','SPARE','STUMP','FOMS') order by term_low";
        //    rs1 = m_IGTDataContext.OpenRecordset(sSql, ADODB.CursorTypeEnum.adOpenStatic, ADODB.LockTypeEnum.adLockReadOnly, 1);
        //    if (rs1.RecordCount > 0)
        //    {
        //        //myListBox.Items.Clear();
        //        rs1.MoveFirst();
        //        while (!rs1.EOF)
        //        {
        //            iTermLo = (rs1.Fields["term_low"].Value.ToString().Trim() == "") ? 0 : Convert.ToInt16(rs1.Fields["term_low"].Value.ToString());
        //            iTermHi = (rs1.Fields["term_high"].Value.ToString().Trim() == "") ? 0 : Convert.ToInt16(rs1.Fields["term_high"].Value.ToString());
        //            sStatus = rs1.Fields["CORE_STATUS"].Value.ToString().Trim();
        //            iNodeFNO = (rs1.Fields["g3e_fno"].Value.ToString().Trim() == "") ? 0 : Convert.ToInt16(rs1.Fields["g3e_fno"].Value.ToString());
        //            iNodeFID = (rs1.Fields["g3e_fid"].Value.ToString().Trim() == "") ? 0 : Convert.ToInt32(rs1.Fields["g3e_fid"].Value.ToString());
        //            if ((iTermLo > 0) && (iTermHi > 0))
        //            {
        //                for (int ind = iTermLo; ind <= iTermHi; ind++)
        //                {
        //                    iVirtualNo = ((iSplitterNo - 1) * iEffPort) + ind;
        //                    //get Feature Code
        //                    STermFeatDesc = "";
        //                    if ((sStatus == C_FOMS) || (sStatus == C_MAIN) || (sStatus == C_PROCTECTION))
        //                    {
        //                        STermFeatDesc = " [ " + GetFeatCode(iNodeFNO, iNodeFID.ToString()) + " - " + GetFeatName(iNodeFNO.ToString()) + " ]";
        //                    }
        //                    myListBox.Items.Add(iVirtualNo + ", " + sStatus + STermFeatDesc);
        //                }
        //            }

        //            rs1.MoveNext();
        //        }

        //    }
        //    rs1.Close();


        //}

        private void DisplayUsedCore(string mFNO, string mFID, ListBox myListBox)
        {
            string itmRng = string.Empty;
            if (mFNO != FECBL_FNO.ToString()) return;

            int iEffCore = GetEffectiveCore(Convert.ToInt16(mFNO), mFID);

            ADODB.Recordset rs1 = new ADODB.Recordset();
            string sSql = string.Empty;
            int iTermLo = 0;
            int iTermHi = 0;
            string sStatus = string.Empty;
            string STermFeatDesc = string.Empty;
            int iNodeFNO = 0;
            int iNodeFID = 0;
            myListBox.Items.Clear();

            sSql = "select distinct term_low, term_high, CORE_STATUS, g3e_fno, g3e_fid from gc_splice_connect where term_fid = " + mFID;
            sSql = sSql + " and CORE_STATUS in ('MAIN','PROTECTION','SPARE','STUMP','FOMS','" + C_FOMS_MAIN + "', '" + C_FOMS_PROTECTION + "') order by term_low";
            rs1 = m_IGTDataContext.OpenRecordset(sSql, ADODB.CursorTypeEnum.adOpenStatic, ADODB.LockTypeEnum.adLockReadOnly, 1);
            if (rs1.RecordCount > 0)
            {
                //myListBox.Items.Clear();
                rs1.MoveFirst();
                while (!rs1.EOF)
                {
                    iTermLo = (rs1.Fields["term_low"].Value.ToString().Trim() == "") ? 0 : Convert.ToInt16(rs1.Fields["term_low"].Value.ToString());
                    iTermHi = (rs1.Fields["term_high"].Value.ToString().Trim() == "") ? 0 : Convert.ToInt16(rs1.Fields["term_high"].Value.ToString());
                    iNodeFNO = (rs1.Fields["g3e_fno"].Value.ToString().Trim() == "") ? 0 : Convert.ToInt16(rs1.Fields["g3e_fno"].Value.ToString());
                    iNodeFID = (rs1.Fields["g3e_fid"].Value.ToString().Trim() == "") ? 0 : Convert.ToInt32(rs1.Fields["g3e_fid"].Value.ToString());
                    sStatus = rs1.Fields["CORE_STATUS"].Value.ToString().Trim();
                    if ((iTermLo > 0) && (iTermHi > 0))
                    {
                        for (int ind = iTermLo; ind <= iTermHi; ind++)
                        {
                            //get Feature Code
                            STermFeatDesc = "";
                            if ((sStatus == C_FOMS_PROTECTION) || (sStatus == C_FOMS_MAIN) || (sStatus == C_FOMS) || (sStatus == C_MAIN) || (sStatus == C_PROCTECTION))
                            {
                                STermFeatDesc = " [ " + GetFeatCode(iNodeFNO, iNodeFID.ToString()) + " - " + GetFeatName(iNodeFNO.ToString()) + " ]";
                            }
                            myListBox.Items.Add(ind + ", " + sStatus + STermFeatDesc);
                        }
                    }

                    rs1.MoveNext();
                }

            }
            rs1.Close();


        }

        private void DisplayUsedODF(string mFNO, string mFID, ListBox myListBox)
        {
            string itmRng = string.Empty;
            if (mFNO != ODF_FNO.ToString()) return;

            try{

                ADODB.Recordset rs1 = new ADODB.Recordset();
                string sSql = string.Empty;
                int iTermLo = 0;
                int iTermHi = 0;
                string sStatus = string.Empty;
                string STermFeatDesc = string.Empty;
                int iCBLFNO = 0;
                int iCBLFID = 0;
                int iCBLLow = 0;
                int iCBLHigh = 0;
                myListBox.Items.Clear();

                sSql = "select distinct term_low, term_high, fno2, fid2, low2, high2 from gc_splice_connect where term_fid = " + mFID;
                sSql = sSql + " order by term_low";
                rs1 = m_IGTDataContext.OpenRecordset(sSql, ADODB.CursorTypeEnum.adOpenStatic, ADODB.LockTypeEnum.adLockReadOnly, 1);
                if (rs1.RecordCount > 0)
                {
                    //myListBox.Items.Clear();
                    rs1.MoveFirst();
                    while (!rs1.EOF)
                    {
                        iTermLo = (rs1.Fields["term_low"].Value.ToString().Trim() == "") ? 0 : Convert.ToInt16(rs1.Fields["term_low"].Value.ToString());
                        iTermHi = (rs1.Fields["term_high"].Value.ToString().Trim() == "") ? 0 : Convert.ToInt16(rs1.Fields["term_high"].Value.ToString());
                        iCBLFNO = (rs1.Fields["fno2"].Value.ToString().Trim() == "") ? 0 : Convert.ToInt16(rs1.Fields["fno2"].Value.ToString());
                        iCBLFID = (rs1.Fields["fid2"].Value.ToString().Trim() == "") ? 0 : Convert.ToInt32(rs1.Fields["fid2"].Value.ToString());
                        iCBLLow = (rs1.Fields["low2"].Value.ToString().Trim() == "") ? 0 : Convert.ToInt16(rs1.Fields["low2"].Value.ToString());
                        iCBLHigh  = (rs1.Fields["high2"].Value.ToString().Trim() == "") ? 0 : Convert.ToInt32(rs1.Fields["high2"].Value.ToString());
                        sStatus = isInComing(Convert.ToInt32(mFID), iCBLFID) ? C_KEY_FR_ODF : C_KEY_TO_ODF;
                        //sInOutStatus = isInComing(Convert.ToInt32(lbl_FID.Text.Trim()), Convert.ToInt32(gFID2)) ? C_KEY_FR_ODF : C_KEY_TO_ODF;
                                
                        if ((iTermLo > 0) && (iTermHi > 0))
                        {
                            for (int ind = iTermLo; ind <= iTermHi; ind++)
                            {
                                //get Feature Code
                                STermFeatDesc = "";
                                STermFeatDesc = " [ " + GetFeatCode(iCBLFNO, iCBLFID.ToString()) + " - " + GetFeatName(iCBLFNO.ToString()) + " : " + sStatus + " ]";

                                myListBox.Items.Add(ind + " :: " + (iCBLLow + (ind - iTermLo)).ToString() + STermFeatDesc);
                            }
                        }

                        rs1.MoveNext();
                    }

                }
                rs1.Close();

            }
            catch (Exception ex)
            {
                //GTClassFactory.Create<IGTApplication>().EndWaitCursor();
                log.WriteLog("");
                log.WriteLog(this.GetType().FullName, "ERROR: " + ex.Message);
                log.WriteLog(ex.StackTrace);
                MessageBox.Show(ex.Message, C_PROG_NAME, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void cboFrom_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtLOW1.Text = "";
            txtHIGH1.Text = "";
            txtLOW1.Enabled = true;
            txtHIGH1.Enabled = true;
            if (cboFrom.Text.Trim() != "")
            {
                LstInUnAssRngCore.Items.Clear();
                Get_Port_Size(GetComboValue(cboFrom, cboFrom.SelectedItem.ToString()), lbl_FromCore);
                if (cboTo.Text.Trim() != "")
                    Get_Port_Size(GetComboValue(cboTo, cboTo.SelectedItem.ToString()), lbl_ToCore);
                string FID = GetComboValue(cboFrom, cboFrom.SelectedItem.ToString());
                string FNO = Get_Value("Select g3e_fno from gc_netelem where g3e_fid = " + FID);
                //Highlight the graphic featuer
                Highlight(Convert.ToInt16(FNO), Convert.ToInt32(FID));
                HighlightGrid();
                //Select_Splice();
                
                //Add by Catherine -- Load Unassign Range List, set 1 to carry fwd parent range
                refreshUnAssignList(Convert.ToInt16(FNO), Convert.ToInt32(FID), 1, LstInUnAssRngCore);

                if (LstInUnAssRngCore.Items.Count <= 0)
                {
                    txtLOW1.Enabled = false;
                    txtHIGH1.Enabled = false;
                }

            }
            else
            {
                //Clear the list
                LstInUnAssRngCore.Items.Clear();
                HighlightGrid();
                //Select_Splice();
                //Clear_Grid();
                //grp_Core.Visible = false;
            }
        }

        private void cboDFrom_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtDLOW1.Text = "";
            txtDHIGH1.Text = "";
            txtDLOW1.Enabled = true;
            txtDHIGH1.Enabled = true;
            if (cboDFrom.Text.Trim() != "")
            {
                LstInUnAssRngPort.Items.Clear();
                Get_Port_Size(GetComboValue(cboDFrom, cboDFrom.SelectedItem.ToString()), lbl_FromPort);
                if (cboDTo.Text.Trim() != "") Get_Port_Size(GetComboValue(cboDTo, cboDTo.SelectedItem.ToString()), lbl_ToPort);
                //Select_Splice();
                string sFID = GetComboValue(cboDFrom, cboDFrom.SelectedItem.ToString());
                string sFNO = getFNO(sFID);
                Highlight(Convert.ToInt16(sFNO), Convert.ToInt32(sFID));
                HighlightGrid();
                //Add by Catherine -- Load Unassign Range List, set 1 to carry fwd parent range
                refreshUnAssignList(Convert.ToInt16(sFNO), Convert.ToInt32(sFID), 1, LstInUnAssRngPort);

                if (LstInUnAssRngPort.Items.Count <= 0)
                {
                    txtDLOW1.Enabled = false;
                    txtDHIGH1.Enabled = false;
                }

            }
            else
            {
                //Clear the list
                LstInUnAssRngPort.Items.Clear();
                //Select_Splice();
                HighlightGrid();
                
            }
        }

        private void cboTo_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtLOW2.Text = "";
            txtHIGH2.Text = "";
            txtLOW2.Enabled = true;
            txtHIGH2.Enabled = true;
            int intAvalSize = 0;
            if (cboTo.Text.Trim() != "")
            {
                LstOutUnAssRngCore.Items.Clear();
                // Select_Splice();
                string FID = GetComboValue(cboTo, cboTo.SelectedItem.ToString());
                string FNO = getFNO(FID);
                Get_Port_Size(FID, lbl_ToCore);
                intAvalSize = Get_Effective_Size(FID) - Get_Effective_UsedSize(FID);
                lbl_EUnUse.Text = intAvalSize.ToString();
                Highlight(Convert.ToInt16(FNO), Convert.ToInt32(FID));
                HighlightGrid();
                //Add by Catherine -- Load Unassign Range List, set 0 NOT to carry fwd parent range
                refreshUnAssignList(Convert.ToInt16(FNO), Convert.ToInt32(FID), 0, LstOutUnAssRngCore);
                if ((LstOutUnAssRngCore.Items.Count <= 0) || (intAvalSize <= 0))
                {
                    txtLOW2.Enabled = false;
                    txtHIGH2.Enabled = false;
                }
                
            }
            else
            {

                //Clear the list
                LstOutUnAssRngCore.Items.Clear();
                HighlightGrid();
            }
        }

        private bool IsFOMSFDP(string sFNO, string sFID)
        {
            
            bool bStatus = false;

            if (sFNO.Trim() == FDP_FNO.ToString())
            {
                //string sType = Get_Value("Select FDP_TYPE from GC_FDP where G3E_FID = " + sFID);
                string sType = Get_Value("Select FDP_CLASS from GC_FDP where G3E_FID = " + sFID);
                bStatus = (sType.Trim() == C_FOMS_FDP_TYPE) ? true : false;
            }
            return bStatus;
        }

        private void cboDTo_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtDLOW2.Text = "";
            txtDHIGH2.Text = "";
            txtDLOW2.Enabled = true;
            txtDHIGH2.Enabled = true;
            //grpMainFOMS.Enabled = false;
            int intAvalSize = 0;
            if (cboDTo.Text.Trim() != "")
            {
                LstOutUnAssRngPort.Items.Clear();
                string FID = GetComboValue(cboDTo, cboDTo.SelectedItem.ToString());
                string FNO = getFNO(FID);

                //Check if FNO Type is FOMS FDP then do further.
                //grpMainFOMS.Visible = IsFOMSFDP(FNO, FID);
                //grpMainFOMS.Enabled = IsFOMSFDP(FNO, FID);
                Get_Port_Size(FID, lbl_ToPort);
                intAvalSize = Get_Effective_Size(FID) - Get_Effective_UsedSize(FID);
                lbl_DUnUse.Text = intAvalSize.ToString();
                Highlight(Convert.ToInt16(FNO), Convert.ToInt32(FID));
                HighlightGrid();
                //Add by Catherine -- Load Unassign Range List, set 0 NOT to carry fwd parent range
                refreshUnAssignList(Convert.ToInt16(FNO), Convert.ToInt32(FID), 0, LstOutUnAssRngPort);
                if ((LstOutUnAssRngPort.Items.Count <= 0)|| (intAvalSize <= 0))
                {
                    txtDLOW2.Enabled = false;
                    txtDHIGH2.Enabled = false;
                }
            }
            else
            {
                //Clear the list
                LstOutUnAssRngPort.Items.Clear();
                HighlightGrid(); 
                //Select_Splice();
            }
        }
        

        private void btn_DAssign_Click(object sender, EventArgs e)
        {
            string sSql = string.Empty;
            ADODB.Recordset rs;
            //Do Check on what to parse in
            //First check the Radio button
            switch (D_ACTION_MODE)
            {
                case C_SPARE:
                    //sSql = "Select * from GC_SPLICE_CONNECT where G3E_FNO = " + lbl_fno.Text + " and G3E_FID = " + lbl_FID.Text + " and CORE_STATUS = '" + C_SPARE + "'";
                    //rs = m_IGTDataContext.OpenRecordset(sSql, ADODB.CursorTypeEnum.adOpenStatic, ADODB.LockTypeEnum.adLockReadOnly, 1);
                    //if (rs.RecordCount == 0)
                    //{
                        Connect(C_SPARE);

                    //}
                    //else
                    //{
                    //    MessageBox.Show("SPARE Record already Exists for the Fiber Splice.", C_PROG_NAME, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    //    return;
                    //}
                    break;
                case C_STUMP:
                    //sSql = "Select * from GC_SPLICE_CONNECT where G3E_FNO = " + lbl_fno.Text + " and G3E_FID = " + lbl_FID.Text + " and CORE_STATUS = '" + C_STUMP + "'";
                    //rs = m_IGTDataContext.OpenRecordset(sSql, ADODB.CursorTypeEnum.adOpenStatic, ADODB.LockTypeEnum.adLockReadOnly, 1);
                    //if (rs.RecordCount == 0)
                    //{
                        Connect(C_STUMP);

                    //}
                    //else
                    //{
                    //    MessageBox.Show("STUMP Record already Exists for the Fiber Splice.", C_PROG_NAME, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    //    return;
                    //}
                    break;
                case C_FOMS:
                    //sSql = "Select * from GC_SPLICE_CONNECT where G3E_FNO = " + lbl_fno.Text + " and G3E_FID = " + lbl_FID.Text + " and CORE_STATUS = '" +C_FOMS  +"'";
                    //rs = m_IGTDataContext.OpenRecordset(sSql, ADODB.CursorTypeEnum.adOpenStatic, ADODB.LockTypeEnum.adLockReadOnly, 1);
                    //if (rs.RecordCount == 0)
                    //{
                    //    Connect(C_FOMS);

                    //}
                    //else
                    //{
                    //    MessageBox.Show("FOMS Record already Exists for the Fiber Splice.", C_PROG_NAME, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    //    return;
                    //}
                    //break;
                case C_DISTRIBUTE:
                    if ((lbl_fno.Text == FDP_FNO.ToString()) || (lbl_fno.Text == DB_FNO.ToString()) || (lbl_fno.Text == TIE_FNO.ToString()) || (lbl_fno.Text == FTB_FNO.ToString()))
                    {
                        //Check if FOMS FDP
                        if (IsFOMSFDP(lbl_fno.Text.Trim(), lbl_FID.Text.Trim()))
                        {
                            Connect(C_FOMS);
                        } else {
                            Connect(C_MAIN);
                        }
                    }
                    else
                    {
                        Connect("");
                    }
                    break;
                default:
                    break;

            }
        }


        private void rBtnSetSpareE_CheckedChanged(object sender, EventArgs e)
        {
            cboTo.SelectedText = "";
            grpEAssTo.Enabled = !rBtnSetSpareE.Checked;
            grpMainProtection.Enabled = !rBtnSetSpareE.Checked;
            E_ACTION_MODE = C_SPARE;
            EnableOrDisableEAssignButton(E_ACTION_MODE);

        }

        private void rBtnSetStumpE_CheckedChanged(object sender, EventArgs e)
        {
            cboTo.SelectedText = "";
            grpEAssTo.Enabled = !rBtnSetStumpE.Checked;
            grpMainProtection.Enabled = !rBtnSetStumpE.Checked;
            E_ACTION_MODE = C_STUMP;
            EnableOrDisableEAssignButton(E_ACTION_MODE);

        }

        private void rBtnDistributeE_CheckedChanged(object sender, EventArgs e)
        {
            grpEAssTo.Enabled = rBtnDistributeE.Checked;
            grpMainProtection.Enabled = rBtnDistributeE.Checked;
            E_ACTION_MODE = C_DISTRIBUTE;
            EnableOrDisableEAssignButton(E_ACTION_MODE);
        }

        private void rBtnSetSpareD_CheckedChanged(object sender, EventArgs e)
        {
            //grpDAssTo.Enabled = !rBtnSetSpareD.Checked;
            //D_ACTION_MODE = C_SPARE;

            cboDTo.SelectedText = "";
            grpDAssTo.Enabled = !rBtnSetSpareD.Checked;
            D_ACTION_MODE = C_SPARE;
            EnableOrDisableDAssignButton(D_ACTION_MODE);
        }

        private void rBtnSetStumpD_CheckedChanged(object sender, EventArgs e)
        {
            //grpDAssTo.Enabled = !rBtnSetStumpD.Checked;
            //D_ACTION_MODE = C_STUMP;

            cboDTo.SelectedText = "";
            grpDAssTo.Enabled = !rBtnSetStumpD.Checked;
            D_ACTION_MODE = C_STUMP;
            EnableOrDisableDAssignButton(D_ACTION_MODE);
        }

        private void rBtnDistributeD_CheckedChanged(object sender, EventArgs e)
        {           
            grpDAssTo.Enabled = rBtnDistributeD.Checked;
            D_ACTION_MODE = C_DISTRIBUTE;
            EnableOrDisableDAssignButton(D_ACTION_MODE);
        }

        //private void rBtnFOMSD_CheckedChanged(object sender, EventArgs e)
        //{
        //    //cboDTo.SelectedText = "";
        //    grpDAssTo.Enabled = !rBtnFOMSD.Checked;
        //    D_ACTION_MODE = C_FOMS;
        //    EnableOrDisableDAssignButton(D_ACTION_MODE);
        //}

        private void tabCoreCount_SelectedIndexChanged(object sender, EventArgs e)
        {
            gbl_CURR_TAB = tabCoreCount.SelectedTab.Name;

            switch (gbl_CURR_TAB)
            {
                case C_TAB_ESIDE:
                    //filter the grid
                case C_TAB_DSIDE:
                    //filter the grid
                    //EmptyForm();
                    //Load_Combo();
                    FillDataGrid();
                    break;
                case C_TAB_OVERALL_CORE:
                    grp_Core.Visible = true;
                    Display_Core_Summary();
                    break;
                case C_TAB_OVERALL_PORT:
                    grp_Port.Visible = true;
                    Display_Port_Summary();
                    break;
                case C_TAB_OVERALL_ODF:
                    grp_ODF.Visible = true;
                    Display_ODF_summary();
                    break;
                default:
                    break;
            }
        }

        private void EnableOrDisableODFAssignButton()
        {
            bool bStatus = false;

            if ((txtODFLOW1.Text.Trim() != "") && (txtODFHIGH1.Text.Trim() != "") && (txtODFLOW2.Text.Trim() != "") && (txtODFHIGH2.Text.Trim() != "") && (cboODFTo.Text.Trim() != "") ) bStatus = true;
            btn_Assign.Enabled = bStatus;
        }
        
        private void EnableOrDisableEAssignButton(string currMODE)
        {
            bool bStatus = false;

            if ((currMODE == C_SPARE) || (currMODE == C_STUMP))
            {
                if ((txtLOW1.Text.Trim() != "") && (txtHIGH1.Text.Trim() != "")) bStatus = true;

            }
            else
            {
                if ((txtLOW1.Text.Trim() != "") && (txtHIGH1.Text.Trim() != "") && (txtLOW2.Text.Trim() != "") && (txtHIGH2.Text.Trim() != "")) bStatus = true;
            }
            btn_Assign.Enabled = bStatus;
        }

        private void txtLOW1_TextChanged(object sender, EventArgs e)
        {
            EnableOrDisableEAssignButton(E_ACTION_MODE);
            txtLOW2.Text = txtLOW1.Text.Trim();
        }

        private void txtHIGH1_TextChanged(object sender, EventArgs e)
        {
            EnableOrDisableEAssignButton(E_ACTION_MODE);
            int iHi1 = (txtHIGH1.Text.Trim() == "") ? 0 : Convert.ToInt16(txtHIGH1.Text);
            int iLo1 = (txtLOW1.Text.Trim() == "") ? 0 : Convert.ToInt16(txtLOW1.Text);
            int iGap = iHi1 - iLo1;
            int iHigh = (txtLOW2.Text.Trim() == "") ? 0 : Convert.ToInt16(txtLOW2.Text) + iGap;
            txtHIGH2.Text = (iHigh == 0) ? "" : iHigh.ToString();
        }

        private void txtDLOW1_TextChanged(object sender, EventArgs e)
        {
            EnableOrDisableDAssignButton(D_ACTION_MODE);
            txtDLOW2.Text = txtDLOW1.Text.Trim();
        }

        private void txtDHIGH1_TextChanged(object sender, EventArgs e)
        {
            EnableOrDisableDAssignButton(D_ACTION_MODE);
            int iHi1 = (txtDHIGH1.Text.Trim() == "") ? 0 : Convert.ToInt16(txtDHIGH1.Text);
            int iLo1 = (txtDLOW1.Text.Trim() == "") ? 0 : Convert.ToInt16(txtDLOW1.Text);
            int iGap = iHi1 - iLo1;
            int iHigh = (txtDLOW2.Text.Trim() == "") ? 0 : Convert.ToInt16(txtDLOW2.Text) + iGap;
            txtDHIGH2.Text = (iHigh == 0) ? "" : iHigh.ToString();
        }

        private void txtDLOW2_TextChanged(object sender, EventArgs e)
        {
            
            int iHi1 = (txtDHIGH1.Text.Trim() == "") ? 0 : Convert.ToInt16(txtDHIGH1.Text);
            int iLo1 = (txtDLOW1.Text.Trim() == "") ? 0 : Convert.ToInt16(txtDLOW1.Text);
            int iGap = iHi1 - iLo1;
            int iHigh = (txtDLOW2.Text.Trim() == "") ? 0 : Convert.ToInt16(txtDLOW2.Text) + iGap;
            txtDHIGH2.Text = (iHigh == 0) ? "" : iHigh.ToString();
            
            EnableOrDisableDAssignButton(D_ACTION_MODE);
        }

        private void txtLOW2_TextChanged(object sender, EventArgs e)
        {
            if ((E_ACTION_MODE == C_DISTRIBUTE) && (txtLOW1.Text.Trim() != "") && (txtHIGH1.Text.Trim() != "") && (txtLOW2.Text.Trim() != ""))
            {
                //int iGap = Convert.ToInt16(txtHIGH1.Text) - Convert.ToInt16(txtLOW1.Text);
                //int iHigh = Convert.ToInt16(txtLOW2.Text) + iGap;
                //txtHIGH2.Text = iHigh.ToString();

                int iHi1 = (txtHIGH1.Text.Trim() == "") ? 0 : Convert.ToInt16(txtHIGH1.Text);
                int iLo1 = (txtLOW1.Text.Trim() == "") ? 0 : Convert.ToInt16(txtLOW1.Text);
                int iGap = iHi1 - iLo1;
                int iHigh = (txtLOW2.Text.Trim() == "") ? 0 : Convert.ToInt16(txtLOW2.Text) + iGap;
                txtHIGH2.Text = (iHigh == 0) ? "" : iHigh.ToString();
            }
            EnableOrDisableEAssignButton(E_ACTION_MODE);
        }

        private void txtHIGH2_TextChanged(object sender, EventArgs e)
        {
            EnableOrDisableEAssignButton(E_ACTION_MODE);
        }


        private void txtDHIGH2_TextChanged(object sender, EventArgs e)
        {
            EnableOrDisableDAssignButton(D_ACTION_MODE);
        }
        private void EnableOrDisableDAssignButton(string currMODE)
        {
            bool bStatus = false;

            //if ((currMODE == C_SPARE) || (currMODE == C_STUMP) || (currMODE == C_FOMS))
            if ((currMODE == C_SPARE) || (currMODE == C_STUMP) )
            {
                if ((txtDLOW1.Text.Trim() != "") && (txtDHIGH1.Text.Trim() != "")) bStatus = true;

            }
            else
            {
                if ((txtDLOW1.Text.Trim() != "") && (txtDHIGH1.Text.Trim() != "") && (txtDLOW2.Text.Trim() != "") && (txtDHIGH2.Text.Trim() != "")) bStatus = true;
            }
            btn_DAssign.Enabled = bStatus;
        }

        private void cboSplitterList_SelectedIndexChanged(object sender, EventArgs e)
        {
            //Load the port availability summary
            //get selected item from combobox
            //string src_fno = string.Empty;
            //string src_fid = GetComboValue(cboSplitterList, cboSplitterList.SelectedItem.ToString());
            //src_fno = getFNO(src_fid);

            //DisplayAvailablePort(src_fno, src_fid, lst_Port);
            //DisplayUsedPort(src_fno, src_fid, lst_Port_Taken);
                        
        }

        private void gv_Route_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {

            //get selected Row
            int iRow = e.RowIndex;
            int iCol = e.ColumnIndex;

            if (iCol >= 3)
            {
                //get Fid1, fid2 into array
                int[] iArrayFID = new int[2];

                iArrayFID[0] = (gv_Route.Rows[iRow].Cells["FID1"].Value.ToString().Trim() == "") ? 0 : Convert.ToInt32(gv_Route.Rows[iRow].Cells["FID1"].Value);
                iArrayFID[1] = (gv_Route.Rows[iRow].Cells["FID2"].Value.ToString().Trim() == "") ? 0 : Convert.ToInt32(gv_Route.Rows[iRow].Cells["FID2"].Value);

                //Now highlight on the map
                Highlight(iArrayFID);
            }
            else
            {
                //Delete the column
                
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
                    Delete_Splice();
                }
                else
                    gv_Route[2, e.RowIndex].Value = false;

               
            }
        }

        private void gv_Route_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {

            //get selected Row
            int iRow = e.RowIndex;

            //get Fid1, fid2 into array
            int[] iArrayFID = new int[2];

            iArrayFID[0] = (gv_Route.Rows[iRow].Cells["FID1"].Value.ToString().Trim() == "") ? 0 : Convert.ToInt32(gv_Route.Rows[iRow].Cells["FID1"].Value);
            iArrayFID[1] = (gv_Route.Rows[iRow].Cells["FID2"].Value.ToString().Trim() == "") ? 0 : Convert.ToInt32(gv_Route.Rows[iRow].Cells["FID2"].Value);

            //Now highlight on the map
            Highlight(iArrayFID);
        }

        private void LstInUnAssRngPort_SelectedIndexChanged(object sender, EventArgs e)
        {
            //get the selected item
            int iRow = LstInUnAssRngPort.SelectedIndex;
            string SelValue = LstInUnAssRngPort.Items[iRow].ToString();

            string[] split;
            int iTLo = -1;
            int iTHi = -1;
                    
            split = SelValue.Split(new Char[] { '|', '~' });
            iTLo = (split[0].ToString().Trim() == "") ? -1 : Convert.ToInt16(split[0].ToString().Trim());
            iTHi = (split[0].ToString().Trim() == "") ? -1 : Convert.ToInt16(split[1].ToString().Trim());

            txtDLOW1.Text = iTLo.ToString();
            txtDHIGH1.Text = iTHi.ToString();
            txtDLOW2.Text = iTLo.ToString();
            txtDHIGH2.Text = iTHi.ToString();
            //MessageBox.Show(LstInUnAssRngPort.SelectedItem.ToString());
        }

        private void LstInUnAssRngCore_SelectedIndexChanged(object sender, EventArgs e)
        {
           //MessageBox.Show(LstInUnAssRngCore.SelectedItem.ToString());
            //get the selected item
            int iRow = LstInUnAssRngCore.SelectedIndex;
            string SelValue = LstInUnAssRngCore.Items[iRow].ToString();

            string[] split;
            int iTLo = -1;
            int iTHi = -1;

            split = SelValue.Split(new Char[] { '|', '~' });
            iTLo = (split[0].ToString().Trim() == "") ? -1 : Convert.ToInt16(split[0].ToString().Trim());
            iTHi = (split[0].ToString().Trim() == "") ? -1 : Convert.ToInt16(split[1].ToString().Trim());

            txtLOW1.Text = iTLo.ToString();
            txtHIGH1.Text = iTHi.ToString();
            txtLOW2.Text = iTLo.ToString();
            txtHIGH2.Text = iTHi.ToString();
        }

        private void txtODFHIGH1_TextChanged(object sender, EventArgs e)
        {
            int iHi1 = (txtODFHIGH1.Text.Trim() == "") ? 0 : Convert.ToInt16(txtODFHIGH1.Text);
            int iLo1 = (txtODFLOW1.Text.Trim() == "") ? 0 : Convert.ToInt16(txtODFLOW1.Text);
            int iGap = iHi1 - iLo1;
            int iHigh = (txtODFLOW2.Text.Trim() == "") ? 0 : Convert.ToInt16(txtODFLOW2.Text) + iGap;
            txtODFHIGH2.Text = (iHigh == 0) ? "" : iHigh.ToString();
        }

        private void RefreshODFAvailCore()
        {
            txtODFLOW1.Text = "";
            txtODFHIGH1.Text = "";
            txtODFLOW1.Enabled = true;
            txtODFHIGH1.Enabled = true;
            int intAvalSize = 0;
 
            string FID = lbl_FID.Text.Trim();
            string FNO = lbl_fno.Text.Trim();
            if (FNO == ODF_FNO.ToString())
            {

                LstInUnAssRngODF.Items.Clear();
                Get_Port_Size(FID, lbl_FromODF);
                intAvalSize = Get_Effective_Size(FID) - Get_Effective_UsedODFSize(FID);
                lbl_FromODFUnUse.Text = intAvalSize.ToString();
                refreshODFUnAssignList(Convert.ToInt16(FNO), Convert.ToInt32(FID), LstInUnAssRngODF);
                if ((LstInUnAssRngODF.Items.Count <= 0) || (intAvalSize <= 0))
                {
                    txtODFLOW1.Enabled = false;
                    txtODFHIGH1.Enabled = false;
                }
            }
            else
            {
                //Clear the list
                LstInUnAssRngODF.Items.Clear();
                // HighlightGrid();
            }

        }

        private void cboODFTo_SelectedIndexChanged(object sender, EventArgs e)
        {

            txtODFLOW2.Text = "";
            txtODFHIGH2.Text = "";
            txtODFLOW2.Enabled = true;
            txtODFHIGH2.Enabled = true;
            int intAvalSize = 0;
            if (cboODFTo.Text.Trim() != "")
            {
                LstOutUnAssRngODF.Items.Clear();
                // Select_Splice();
                string FID = GetComboValue(cboODFTo, cboODFTo.SelectedItem.ToString());
                string FNO = getFNO(FID);
                Get_Port_Size(FID, lbl_ToODF);
                intAvalSize = Get_Effective_Size(FID) - Get_Effective_UsedODFSize(FID);
                lbl_ToODFUnUse.Text = intAvalSize.ToString();
                Highlight(Convert.ToInt16(FNO), Convert.ToInt32(FID));
                HighlightGrid();
                //Add by Catherine -- Load Unassign Range List, set 0 NOT to carry fwd parent range
                refreshODFUnAssignList(Convert.ToInt16(FNO), Convert.ToInt32(FID),  LstOutUnAssRngODF);
                if ((LstOutUnAssRngODF.Items.Count <= 0) || (intAvalSize <= 0))
                {
                    txtODFLOW2.Enabled = false;
                    txtODFHIGH2.Enabled = false;
                }

            }
            else
            {
                //Clear the list
                LstOutUnAssRngODF.Items.Clear();
                HighlightGrid();
            }

            EnableOrDisableODFAssignButton();
        }

        private void LstOutUnAssRngODF_SelectedIndexChanged(object sender, EventArgs e)
        {
            int iRow = LstOutUnAssRngODF.SelectedIndex;
            string SelValue = LstOutUnAssRngODF.Items[iRow].ToString();

            string[] split;
            int iTLo = -1;
            int iTHi = -1;

            split = SelValue.Split(new Char[] { '|', '~' });
            iTLo = (split[0].ToString().Trim() == "") ? -1 : Convert.ToInt16(split[0].ToString().Trim());
            iTHi = (split[0].ToString().Trim() == "") ? -1 : Convert.ToInt16(split[1].ToString().Trim());

            txtODFLOW1.Text = iTLo.ToString();
            txtODFHIGH1.Text = iTHi.ToString();
            txtODFLOW2.Text = iTLo.ToString();
            txtODFHIGH2.Text = iTHi.ToString();
        }

        private void btn_ODFAssign_Click(object sender, EventArgs e)
        {
            try 
            {
                ConnectODF();

            }
            catch (Exception ex)
            {
                //GTClassFactory.Create<IGTApplication>().EndWaitCursor();
                log.WriteLog("");
                log.WriteLog(this.GetType().FullName, "ERROR: " + ex.Message);
                log.WriteLog(ex.StackTrace);
                MessageBox.Show(ex.Message, C_PROG_NAME, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }



        private void ConnectODF()
        {
            IGTKeyObject cblFeat = null;
            IGTKeyObject ODFFeat = null;
                
            try
            {
                if (cboODFTo.Text.Trim() == "") return;
                if (!IsAllInputValueValid("")) return;
                //if (lbl_fno.Text == FDC_FNO.ToString() && CFNO == FDCBL_FNO.ToString())
                //    FiberConnect(cblDFeat, devFeat, cblSFeat, iLow2, iHigh2, iLow1, iHigh1, CORE_STATUS);
                //else
                string cblFNO = getFNO(GetComboValue(cboODFTo, cboODFTo.SelectedItem.ToString()));
                cblFeat = GTClassFactory.Create<IGTApplication>().DataContext.OpenFeature(short.Parse(cblFNO), int.Parse(GetComboValue(cboODFTo, cboODFTo.SelectedItem.ToString())));
                int iLow1 = int.Parse(txtODFLOW1.Text.Trim());
                int iHigh1 = int.Parse(txtODFHIGH1.Text.Trim());
                int iLow2 = int.Parse(txtODFLOW2.Text.Trim());
                int iHigh2 = int.Parse(txtODFHIGH2.Text.Trim());
                
                ODFFeat = GTClassFactory.Create<IGTApplication>().DataContext.OpenFeature(short.Parse(lbl_fno.Text), int.Parse(lbl_FID.Text));

                ODFConnect(ODFFeat, ODFFeat, cblFeat, iLow1, iHigh1, iLow2, iHigh2);

                All_Clear();

                RefreshODFAvailCore();

                //Referesh the Grid
                FillDataGrid();
                //Select_Splice();

                lbl_MSG.Text = "ODF Connection Done.";
                GTClassFactory.Create<IGTApplication>().SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Core Count.");

            }
            catch (Exception ex)
            {
                //GTClassFactory.Create<IGTApplication>().EndWaitCursor();
                log.WriteLog("");
                log.WriteLog(this.GetType().FullName, "ERROR: " + ex.Message);
                log.WriteLog(ex.StackTrace);
                MessageBox.Show(ex.Message, C_PROG_NAME, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        // Fibre Connection
        private void ODFConnect(IGTKeyObject objFrom, IGTKeyObject objNode, IGTKeyObject objTo, int LOW1, int HIGH1, int LOW2, int HIGH2)
        {
           
               string EXC_ABB = null;
                string FEATURE_STATE = null;
                int lastSEQ = 0;
                string SEQ = null;
                string TERM_FID = string.Empty;
                string TERM_FNO = string.Empty;
                string TERM_LOW = string.Empty;
                string TERM_HIGH = string.Empty;
                string CABLE_CODE = string.Empty;

               try
               {

                GTCustomCommandModeless.m_oGTTransactionManager.Begin("ODFConnect");
                IGTKeyObject OFet = GTClassFactory.Create<IGTApplication>().DataContext.OpenFeature(objNode.FNO, objNode.FID);

                if (objNode.FNO == ODF_FNO )
                {
                    
               
                        //Update Splice Connect
                        if (OFet.Components.GetComponent(77).Recordset.EOF)
                        {
                            lastSEQ = 1;

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
                        }

                        SEQ = lastSEQ.ToString();
                        OFet.Components.GetComponent(77).Recordset.AddNew("G3E_FNO", objNode.FNO);
                        OFet.Components.GetComponent(77).Recordset.Fields["SEQ"].Value = 1;
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

                        OFet.Components.GetComponent(77).Recordset.Fields["NODE_FNO1"].Value = objNode.FNO;
                        OFet.Components.GetComponent(77).Recordset.Fields["NODE_FID1"].Value = objNode.FID;

                        OFet.Components.GetComponent(77).Recordset.Update("G3E_FNO", objNode.FNO);
                        OFet.Components.GetComponent(77).Recordset.Update("CONNECTION_TYPE", "Continuous");
                        OFet.Components.GetComponent(77).Recordset.Update("CORE_STATUS", "");
                        OFet.Components.GetComponent(77).Recordset.Update("LOSS_MEASURED", "0");
                        OFet.Components.GetComponent(77).Recordset.Update("LOSS_TYPICAL", "0");

                        //New Columns
                        EXC_ABB = Get_Value("SELECT EXC_ABB FROM GC_NETELEM WHERE G3E_FID = " + objNode.FID);
                        FEATURE_STATE = Get_Value("SELECT FEATURE_STATE FROM GC_NETELEM WHERE G3E_FID = " + objNode.FID);
                        OFet.Components.GetComponent(77).Recordset.Update("EXC_ABB", EXC_ABB);
                        OFet.Components.GetComponent(77).Recordset.Update("FEATURE_STATE", FEATURE_STATE);

                        //Tempority off the TERM FID and FNO
                        OFet.Components.GetComponent(77).Recordset.Update("TERM_FID", objNode.FID);
                        OFet.Components.GetComponent(77).Recordset.Update("TERM_FNO", objNode.FNO);
                        OFet.Components.GetComponent(77).Recordset.Update("TERM_LOW", LOW1 );
                        OFet.Components.GetComponent(77).Recordset.Update("TERM_HIGH", HIGH1);
                        OFet.Components.GetComponent(77).Recordset.Update("SRC_FID", objNode.FID);
                        OFet.Components.GetComponent(77).Recordset.Update("SRC_FNO", objNode.FNO);
                        OFet.Components.GetComponent(77).Recordset.Update("SRC_LOW", LOW1);
                        OFet.Components.GetComponent(77).Recordset.Update("SRC_HIGH", HIGH1);

                        CABLE_CODE = GetFeatCode(objNode.FNO, objNode.FID.ToString());
                        if (CABLE_CODE != "") OFet.Components.GetComponent(77).Recordset.Update("CABLE_CODE", CABLE_CODE);

                        GTCustomCommandModeless.m_oGTTransactionManager.Commit();
                }

                
            }
            catch (Exception ex)
            {
                GTCustomCommandModeless.m_oGTTransactionManager.Rollback();
                MessageBox.Show(ex.Message, C_PROG_NAME, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LstInUnAssRngODF_SelectedIndexChanged(object sender, EventArgs e)
        {
            int iRow = LstInUnAssRngODF.SelectedIndex;
            string SelValue = LstInUnAssRngODF.Items[iRow].ToString();
            
            string[] split;
            int iTLo = -1;
            int iTHi = -1;

            split = SelValue.Split(new Char[] { '|', '~' });
            iTLo = (split[0].ToString().Trim() == "") ? -1 : Convert.ToInt16(split[0].ToString().Trim());
            iTHi = (split[0].ToString().Trim() == "") ? -1 : Convert.ToInt16(split[1].ToString().Trim());

            txtODFLOW1.Text = iTLo.ToString();
            txtODFHIGH1.Text = iTHi.ToString();
            //get the lowest range from the UnAssList
            //get the Destination lowest range value
            if (LstOutUnAssRngODF.Items.Count <= 0) return;            
            string DesFirstValue = LstOutUnAssRngODF.Items[0].ToString();
            int iTLo2 = -1;
            int iTHi2 = -1;
            split = DesFirstValue.Split(new Char[] { '|', '~' });
            iTLo2 = (split[0].ToString().Trim() == "") ? 0 : Convert.ToInt16(split[0].ToString().Trim());
            iTHi2 = (split[0].ToString().Trim() == "") ? 0 : Convert.ToInt16(split[1].ToString().Trim());
            txtODFLOW2.Text = iTLo2.ToString();
            txtODFHIGH2.Text = (iTLo2 + (iTHi - iTLo)).ToString();
            EnableOrDisableODFAssignButton();
        }

        private void txtODFLOW1_TextChanged(object sender, EventArgs e)
        {
            string[] split;
            //get the Destination lowest range value
            if (LstOutUnAssRngODF.Items.Count <= 0) return;
            string DesFirstValue = LstOutUnAssRngODF.Items[0].ToString();
            int iTLo2 = -1;
            int iTHi2 = -1;
            split = DesFirstValue.Split(new Char[] { '|', '~' });
            iTLo2 = (split[0].ToString().Trim() == "") ? 0 : Convert.ToInt16(split[0].ToString().Trim());
            iTHi2 = (split[0].ToString().Trim() == "") ? 0 : Convert.ToInt16(split[1].ToString().Trim());
            txtODFLOW2.Text = (iTLo2 == 0) ? "" : iTLo2.ToString();
            EnableOrDisableODFAssignButton();

        }

        private void txtODFLOW2_TextChanged(object sender, EventArgs e)
        {

            int iHi1 = (txtODFHIGH1.Text.Trim() == "") ? 0 : Convert.ToInt16(txtODFHIGH1.Text);
            int iLo1 = (txtODFLOW1.Text.Trim() == "") ? 0 : Convert.ToInt16(txtODFLOW1.Text);
            int iGap = iHi1 - iLo1;
            int iHigh = (txtODFLOW2.Text.Trim() == "") ? 0 : Convert.ToInt16(txtODFLOW2.Text) + iGap;
            txtODFHIGH2.Text = (iHigh == 0) ? "" : iHigh.ToString();

            EnableOrDisableODFAssignButton();
        }

        
              

    }
}