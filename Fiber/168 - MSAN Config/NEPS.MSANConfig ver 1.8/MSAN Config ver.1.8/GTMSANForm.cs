/*
 * GTMSAN - MSAN configuration window
 * develop by : m.zam - azza solution (m.zam@azza4u.com)
 * start date : 01-MAR-2012
 * 
 * latest update : 10-MAY-2012
 * 
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


namespace NEPS.GTechnology.MSANConfig
{
    public partial class GTMSANForm : Form
    {

        #region variables

        private bool hasBeenSaved = false;
        private IGTApplication m_gtapp = null;
        private IGTDataContext m_IGTDataContext = null;

        public static int appsmode = 0;
        /* appsmode 
         * 0 : start
         * 1 : monitor mouse click to select MSAN
         * 2 : confirm selection user on edit mode
         * 3 : 
         * 4 : insert/update data to database
         * 5 : done
         */
        private clsMSAN cMSAN = new clsMSAN();

        

        private int SELECTED_FNO = 0;
        private int SELECTED_FID = -1;
        private string SELECTED_TBL = "";


        List<string> g_delframe = new List<string>(); // hold id fo deleted row
        List<int> g_delrow = new List<int>(); // hold id fo deleted row
        List<object> g_copiedcell = new List<object>(); // hold values of copied cell
        int g_copiedcol = 0; // user copying a single col
        int g_copiedrow = 0; // user copying a single row

        private IGTDataContext m_GTDataContext = null;
        private bool isLoadingMSAN = false;

        #endregion

        #region " Local Events "
        private void btnHeader_Click(object sender, EventArgs e)
        {
            if (btnHeader.ImageIndex == 0) // down arrow
            {
                btnHeader.ImageIndex = 1;
                pnlTop.Height = txtStatus.Top + (int)(txtStatus.Height * 1.5);
            }
            else
            {
                btnHeader.ImageIndex = 0;
                pnlTop.Height = btnTemplate.Height;
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

        #region " 2. Show MSAN dialog window "

        #region " 2.0 Initialize and Load Form "
        public GTMSANForm()
        {
            try
            {
                InitializeComponent();

                m_IGTDataContext = GTClassFactory.Create<IGTApplication>().Application.DataContext;
                m_gtapp = GTClassFactory.Create<IGTApplication>().Application;
                m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Running MSAN Config...");
                txtMSANRack.Text = "";
                Init_Combo_DSALMRefTable(); // header
                appsmode = 0;
                //TODO : InitTreeView();

                InitTemplate();
                // LoadXLFile();
                InitiateMSAN();
                if (appsmode != 1)
                {
                    InitComboManually();
                    if (appsmode != 1)
                        InitBOM();
                }

                DisableApplyButtons_ForMinumux();


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "MSAN Config", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// for minimux, disable the apply buttons.
        private void DisableApplyButtons_ForMinumux()
        {
            bool isOneTemplateSelected =
                cmbMSANTemplate.SelectedIndex != -1 &&
                cmbMSANTemplate.Items.Count == 1;
            if (SELECTED_FNO == Utilities.IMUX_FNO && isOneTemplateSelected)
            {
                // disable the apply buttons
                btnApply.Enabled = false;
                btnApplySelected.Enabled = false;

                // auto click the apply selected
                OnClickApplyTemplate(false);
            }
        }

        #endregion

        #region " 2.1.0 Load Properties Selection from ReferenceTable"

        private void Load_MSANReferenceTable(ComboBox cmb, string reftable)
        {
            try
            {
                string ssql = "SELECT PL_NUM, PL_VALUE FROM " + reftable;
                Load_MSANCombo(ssql, cmb, "PL_NUM", "PL_VALUE");
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR reading " + reftable + " : " + ex.Message);
            }
        }

        private void Load_MSANCombo(string ssql, ComboBox cmb, string colID, string colValue)
        {
            int rec_count = 0;
            this.m_IGTDataContext = GTClassFactory.Create<IGTApplication>().DataContext;
            ADODB.Recordset rs = m_IGTDataContext.Execute(ssql, out rec_count, (int)CommandTypeEnum.adCmdText, null);
            myCombo.AddItems(cmb, rs, colID, colValue, "");
        }

        private void Init_Combo_DSALMRefTable()
        {
            //Load_MSANReferenceTable(cmbMSANType, "REF_COP_MSANTYPE");
            //Load_MSANReferenceTable(cmbMSANLoc, "REF_COP_MSANLOC");
            //Load_MSANReferenceTable(cmbMSANFac, "REF_COP_MSANFAC");
            //Load_MSANReferenceTable(cmbMSANContr, "REF_COP_MSANCONTR");
        }

        #endregion

        #region " 2.1.1 Init Combobox Value - hardcoded "

        private void InitComboManually()
        {
            //colCardType.Items.Clear();
            //colCardType.Items.Add("");
            //colCardType.Items.Add("ADSL");
            //colCardType.Items.Add("TSS");
            //colCardType.Items.Add("POTS");
            //colCardType.Items.Add("CCB");
            //colCardType.Items.Add("POWER CARD");
            //colCardType.Items.Add("CONTROL CARD");

            colPlantUnit.Items.Clear();

            // determing whether the current feature is a Minimux
            bool isMux = SELECTED_FNO == Utilities.IMUX_FNO;
            string ssql = "";

            // fill the plant units
            if (isMux)
            {
                ssql = string.Format("SELECT MIN_MATERIAL FROM REF_MUX where NO_E1='{0}' and MANUFACTURER='{1}'",
                    txtType.Text, txtManufacturer.Text);
            }
            else
            {
                ssql = string.Format("SELECT MIN_MATERIAL FROM REF_FCARD_MSAN where MSAN_FNO={0} and MANUFACTURER='{1}'",
                    SELECTED_FNO, txtManufacturer.Text);// and CARD_MODEL='" + txtModel.Text + "'";
            }
            if (!Utilities.FillComboBox(ssql, colPlantUnit, "MIN_MATERIAL"))
            {
                MessageBox.Show("MANUFACTURER or MODEL of selected feature is not available! " +
                    ssql, "MSAN Config", MessageBoxButtons.OK, MessageBoxIcon.Information);
                appsmode = 1;
                return;
            }

            // fill the card types
            ssql = string.Format("select distinct(card_type) from ag_msan_template where card_type is not null order by card_type");
            Utilities.FillComboBox(ssql, colCardTypeCombo, "card_type");

            // fill the card models
            ssql = "select distinct(card_model) from ag_msan_template where card_model is not null order by card_model";
            Utilities.FillComboBox(ssql, colCardModelCombo, "card_model");

            colCardStatus.Items.Clear();
            colCardStatus.Items.Add("");
            colCardStatus.Items.Add("PLANNED");
            colCardStatus.Items.Add("IN SERVICE");
            colCardStatus.Items.Add("DECOMMISSIONED");

            //colCardModel.Items.Clear();
            //colCardModel.Items.Add("");
            //colCardModel.Items.Add("GAGL");
            //colCardModel.Items.Add("ADRB");
            //colCardModel.Items.Add("TSSB");
            //colCardModel.Items.Add("A32");
            //colCardModel.Items.Add("ADSL");
            //colCardModel.Items.Add("ALC");
            //colCardModel.Items.Add("RALC");
        }


        #endregion

        #region " 2.2 Initialize Application for MSAN Configuration "

        /// <summary>
        /// Initiate application to start MSAN configuration procedure. 
        /// Monitor mouse click for selected object. 
        /// Display MSAN information on the window if selected.
        /// 
        /// created : m.zam @ 01-04-2012
        /// </summary>
        private void InitiateMSAN()
        {
            GTClassFactory.Create<IGTApplication>().SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Initiate MSAN Configuration Procedure ....");

            // skip this step if MSAN is already selected
            if (!isMSANSelected())
            {
                // set application mode to 1 : monitoring mouse click to select RT feature
                appsmode = 1;
                // initialize G-Tech for convertion
                MessageBox.Show("Select MSAN/RT/VDSL2/MINIMUX feature first", "MSAN Config", MessageBoxButtons.OK, MessageBoxIcon.Information);
                //  m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Select one MSAN feature to configure or Press Esc to cancel");
                m_gtapp.Application.ActiveMapWindow.MousePointer = GTMousePointerConstants.gtmwmpNWArrow;
            }
            else
            {
            }
        }

        #endregion

        #endregion

        #region " 3. User Selected an RT Feauture by Mouse Click "
        public Boolean _mouseDown
        {
            set
            {
                if (isMSANSelected())
                {

                }
            }
        }

        private bool isMSANSelected()
        {
            // make sure only one RT object is selected
            //GTClassFactory.Create<IGTApplication>().BeginWaitCursor();
            SELECTED_FID = -1;
            SELECTED_FNO = -1;
            grdMSAN.Rows.Clear();
            try
            {

                IGTSelectedObjects SelectedObjects = m_gtapp.SelectedObjects;
                if (SelectedObjects.FeatureCount == 1)
                {
                    foreach (IGTDDCKeyObject oDDCKeyObject in SelectedObjects.GetObjects())
                    // notes for me : in one feature can have more than one object
                    {

                        switch (oDDCKeyObject.FNO)
                        {
                            case Utilities.MSAN_FNO: SetLabel("MSAN", Utilities.MSAN_FNO, "GC_MSAN"); SELECTED_FNO = Utilities.MSAN_FNO; break;
                            case Utilities.RT_FNO: SetLabel("RT", Utilities.RT_FNO, "GC_RT"); SELECTED_FNO = Utilities.RT_FNO; break;
                            case Utilities.VDSL_FNO: SetLabel("VDSL2", Utilities.VDSL_FNO, "GC_VDSL2"); SELECTED_FNO = Utilities.VDSL_FNO; break;
                            case Utilities.IMUX_FNO: SetLabel("MUX", Utilities.IMUX_FNO, "GC_MINIMUX"); SELECTED_FNO = Utilities.IMUX_FNO; break;
                            default: return false;
                        }

                        // reset this flag since something has been selected
                        hasBeenSaved = false;
                        SELECTED_FID = oDDCKeyObject.FID;
                        // LoadSelectedObj();
                        LoadHeaderInfo();
                        LoadNetelement();

                        string jobID = GTClassFactory.Create<IGTApplication>().DataContext.ActiveJob.ToString();
                        cMSAN.SelectMSAN(SELECTED_FID,SELECTED_FNO, "00", jobID, txtExchange.Text);

                        LoadMSAN_Shelf();

                        Application.DoEvents();
                        InitTreeView();
                        LoadXLFile();
                        return true;
                    }
                    return true;
                }
                else
                {
                    GTClassFactory.Create<IGTApplication>().SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Please select an MSAN/RT/VDSL2/MINIMUX only");
                    GTClassFactory.Create<IGTApplication>().Application.ActiveMapWindow.MousePointer = GTMousePointerConstants.gtmwmpNWArrow;
                    return false;
                }//switch
            }
            catch (Exception ex)
            {
                return false;
            }
            finally
            {
                GTClassFactory.Create<IGTApplication>().EndWaitCursor();
            }
        }// isRTSelected


        private void SetLabel(string rt_type, int iFNO, string tblname)
        {
            this.lblCode.Text = rt_type + " Code";
            this.lblType.Text = rt_type + " Type";
            this.btnHeader.Text = rt_type + " General Information";
            this.btnTemplate.Text = rt_type + " Template";
            this.btnDetail.Text = rt_type + " Configuration";

            this.trvCard.Nodes.Clear();
            TreeNode nd = this.trvCard.Nodes.Add(rt_type);
            nd.Tag = rt_type;

            SELECTED_FNO = iFNO;
            SELECTED_TBL = tblname;
        }

        #endregion

        #region " 5. Shows Details "

        private void LoadSelectedObj()
        {
            //GTClassFactory.Create<IGTApplication>().BeginWaitCursor();
            IGTSelectedObjects SelectedObjects =
                GTClassFactory.Create<IGTApplication>().SelectedObjects;

            // get selected object properties
            foreach (IGTDDCKeyObject oDDCKeyObject in SelectedObjects.GetObjects())
            {
                //ShowMSAN_Feature(oDDCKeyObject);

                ADODB.Recordset rs = new ADODB.Recordset();
                rs = oDDCKeyObject.Recordset;

                while (!rs.EOF) // notes for me : read all component under the RT features
                {
                    //ShowMSAN_Component(rs);
                    rs.MoveNext();
                }
                rs.Close();
                rs = null;
            }

        }


        private void LoadHeaderInfo()
        {
            try
            {
                ADODB.Recordset rs = new ADODB.Recordset();
                string ssql = "SELECT * FROM " + SELECTED_TBL + " WHERE G3E_FID = " + SELECTED_FID;

                rs = GTClassFactory.Create<IGTApplication>().DataContext.OpenRecordset(ssql, ADODB.CursorTypeEnum.adOpenStatic, ADODB.LockTypeEnum.adLockReadOnly, 1);

                bool isMinimux = SELECTED_TBL.ToUpper() == "GC_MINIMUX";
                txtCode.Text = isMinimux ? Utilities.rsField(rs, "MUX_CODE") : Utilities.rsField(rs, "RT_CODE");
                txtType.Text = isMinimux ? Utilities.rsField(rs, "NO_E1") : Utilities.rsField(rs, "RT_TYPE") + Utilities.rsField(rs, "MSAN_TYPE");
                txtModel.Text = Utilities.rsField(rs, "MODEL");
                txtManufacturer.Text = Utilities.rsField(rs, "MANUFACTURER");

            }
            catch (Exception ex)
            {
            }
        }


        private void LoadNetelement()
        {
            try
            {
                ADODB.Recordset rs = new ADODB.Recordset();
                string ssql = "SELECT * FROM GC_NETELEM WHERE G3E_FID = " + SELECTED_FID;

                rs = GTClassFactory.Create<IGTApplication>().DataContext.OpenRecordset(ssql, ADODB.CursorTypeEnum.adOpenStatic, ADODB.LockTypeEnum.adLockReadOnly, 1);
                this.txtExchange.Text = Utilities.rsField(rs, "EXC_ABB");
                this.txtStatus.Text = Utilities.rsField(rs, "FEATURE_STATE");
            }
            catch (Exception ex)
            {
                this.txtExchange.Text = "-not define-";
            }
        }

        private void LoadMSAN_Detail(int iFID, int iFNO)
        {
            string jobID = GTClassFactory.Create<IGTApplication>().DataContext.ActiveJob.ToString();
            cMSAN.SelectMSAN(iFID,iFNO, txtMSANRack.Text, jobID, txtExchange.Text);
        }

        private void LoadMSAN_Shelf()
        {
            try
            {
                cmbShelf.Items.Clear();
                foreach (clsMSAN.MSANFrame f in cMSAN.MSANCabinet.Values)
                {
                    cmbShelf.Items.Add(f.frame_NIS);
                }
                if (cmbShelf.Items.Count > 0) cmbShelf.SelectedIndex = 0;
            }
            catch { }
        }

        #endregion

        #region " 7. User edit MSAN Header Information "
        // user are not allowed to edit MSAN header information (attributes)
        // use may edit these attributes directly from GTECH interface
        #endregion

        #region " 8. template manager : user select template "

        /// <summary>
        /// Initialize MSAN template.
        /// Load available template name from database and add to cmbTemplate
        /// </summary>
        private void InitTemplate()
        {
            btnTemplate.ImageIndex = 0;
            pnlTemplate.Height = btnTemplate.Height;
            cmbMSANTemplate.Items.Clear();
        }

        private void LoadTemplateName()
        {
            try
            {
                string ssql = "SELECT * FROM AG_MSAN_XLTEMPLATE";

                int rec_count;

                this.m_IGTDataContext = GTClassFactory.Create<IGTApplication>().DataContext;
                ADODB.Recordset rs = m_IGTDataContext.Execute(ssql, out rec_count, (int)CommandTypeEnum.adCmdText, null);
                cmbMSANTemplate.Items.Clear();

                if (rs != null)
                {
                    rs.MoveFirst();
                    do
                    {
                        myCombo.AddItems(cmbMSANTemplate, rs, "MODEL_ID", "XL_FILE", "");
                        rs.MoveNext();
                    }
                    while (!rs.EOF);
                    rs = null;
                }
            }
            catch { }
        }

        private void cmbMSANTemplate_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbMSANTemplate.SelectedIndex > -1)
            {
                Debug.WriteLine("cmbMSAN (selected) : " + cmbMSANTemplate.SelectedIndex);
                LoadTemplate();
            }
        }

        private void LoadXLFile()
        {
            try
            {
                this.TopMost = false;
                cmbMSANTemplate.DataSource = null;
                cmbMSANTemplate.Items.Clear();

                string ssql = "select * from AG_MSAN_XLTEMPLATE where MSAN_FNO=" + SELECTED_FNO;
                bool isMux = SELECTED_FNO == Utilities.IMUX_FNO;
                if (isMux) // for Mux, just query the filename instead
                {
                    // special handling for Mux. Because the filename does not match the actual NO_E1.
                    // NO_E1 = 4E1, 8E1, etc. Filename = "....4XE1", ".....8XE1".
                    // This is just a temporary condition, the data and the filename should eventually be changed
                    // to match, but for the purpose of my testing, I will manually insert an X there
                    string modifiedNO_E1 = AddXToNO_E1(txtType.Text);
                    ssql += string.Format(" and XL_FILE LIKE '%{0}%' OR XL_FILE LIKE '%{1}%'", txtType.Text,
                        modifiedNO_E1);
                }
                else
                    ssql += " and MANUFACTURER='" + txtManufacturer.Text + "' and MODEL='" + txtModel.Text + "'";


                int rec_count = 0;
                this.m_IGTDataContext = GTClassFactory.Create<IGTApplication>().DataContext;
                ADODB.Recordset rs = m_IGTDataContext.Execute(ssql, out rec_count, (int)CommandTypeEnum.adCmdText, null);
                if (rs != null && !rs.BOF)
                {

                    rs.MoveFirst();
                    myCombo.AddItems(cmbMSANTemplate, rs, "MODEL_ID", "XL_FILE", "");
                    rs = null;
                }
                else
                {
                    MessageBox.Show("MANUFACTURER or MODEL of selected feature is not available! " + ssql, "MSAN Config", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    appsmode = 1;
                    return;
                }


            }
            catch (Exception ex)
            {
                MessageBox.Show("Fail to get list of template file\r\n" + ex.Message);
            }
            this.TopMost = true;
        }

        /// <summary>
        /// Add an X to the NO_E1, e.g. 4E1 -> 4XE1; 8E1 -> 8XE1; 16E1 -> 16XE1;
        /// This is just a temporary solution.
        /// </summary>
        /// <param name="NO_E1"></param>
        /// <returns></returns>
        private string AddXToNO_E1(string NO_E1)
        {
            if (NO_E1 == "4E1")
                return "4XE1";
            else if (NO_E1 == "8E1")
                return "8XE1";
            else if (NO_E1 == "16E1")
                return "16XE1";
            return NO_E1;

        }

        private void LoadTemplate()
        {
            try
            {
                string ssql = "SELECT * FROM AG_MSAN_TEMPLATE";
                int id = (int)cmbMSANTemplate.SelectedValue;
                if (id > -1)
                    ssql += " WHERE MODEL_ID = " + id.ToString();
                ssql += " ORDER BY to_number(FRAME_NO), to_number(SLOT_NO) asc ";

                int rec_count = 0;
                this.m_IGTDataContext = GTClassFactory.Create<IGTApplication>().DataContext;
                ADODB.Recordset rs = m_IGTDataContext.Execute(ssql, out rec_count, (int)CommandTypeEnum.adCmdText, null);

                lsvTemplate.Items.Clear();

                if (rs != null)
                {
                    if (rs.EOF || rs.BOF)
                    {
                        if (this.Visible)
                        {
                            //MessageBox.Show("Sorry - Fail to read template file\r\nThe file might be empty");
                            ListViewItem l = lsvTemplate.Items.Add("ERROR");
                            l.SubItems.Add("OPENING");
                            l.SubItems.Add("TEMPLATE");
                            l.SubItems.Add(": THE FILE");
                            l.SubItems.Add("IS EMPTY");
                            l.BackColor = Color.Red;
                        }
                        return;
                    }
                    rs.MoveFirst();
                    do
                    {
                        ListViewItem l = lsvTemplate.Items.Add(Utilities.rsField(rs, "RACK_NO"));
                        l.SubItems.Add(Utilities.rsField(rs, "FRAME_NO"));
                        l.SubItems.Add(Utilities.rsField(rs, "SLOT_NO"));
                        l.SubItems.Add(Utilities.rsField(rs, "SLOT_NIS"));
                        l.SubItems.Add(Utilities.rsField(rs, "CARD_TYPE").Trim().ToUpper());
                        l.SubItems.Add(Utilities.rsField(rs, "CARD_MODEL").Trim().ToUpper());
                        // l.SubItems.Add(Utilities.rsField(rs, "CARD_SERVICE").Trim().ToUpper());
                        l.SubItems.Add("PLANNED");
                        l.SubItems.Add(Utilities.rsField(rs, "PORT_LO"));
                        l.SubItems.Add(Utilities.rsField(rs, "PORT_HI"));
                        l.SubItems.Add(Utilities.rsField(rs, "MIN_MATERIAL"));
                        l.Tag = Utilities.rsField(rs, "MODEL_ID");
                        rs.MoveNext();
                    }
                    while (!rs.EOF);
                    rs = null;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Fail to load template file\r\n" + ex.Message);
            }
        }

        private void cmbTemplate_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbMSANTemplate.SelectedIndex > 0)
            {
                // TODO : load selected template details from database
            }
            else
            {
                lsvTemplate.Items.Clear();
            }
        }

        private void btnTemplate_Click(object sender, EventArgs e)
        {
            if (btnTemplate.ImageIndex == 0) // down arrow
            {
                btnTemplate.ImageIndex = 1;
                pnlTemplate.Height = 175;
            }
            else
            {
                btnTemplate.ImageIndex = 0;
                pnlTemplate.Height = btnTemplate.Height;
            }
        }

        private void btnApply_Click(object sender, EventArgs e)
        {
            OnClickApplyTemplate(false);
        }

        private void OnClickApplyTemplate(bool selectedOnly)
        {
            this.TopMost = false;

            List<ListViewItem> templateItems = new List<ListViewItem>();

            if (selectedOnly)
            {
                foreach (ListViewItem templateItem in lsvTemplate.SelectedItems)
                    templateItems.Add(templateItem);
            }
            else
            {
                foreach (ListViewItem templateItem in lsvTemplate.Items)
                    templateItems.Add(templateItem);
            }

            ApplyTemplate(templateItems);

            this.TopMost = true;
        }

        private void btnApplySelected_Click(object sender, EventArgs e)
        {
            OnClickApplyTemplate(true);
        }

        /// <summary>
        /// Obsolete now, Kamal does not want existing data to be erased when applying tempalte
        /// </summary>
        /// <returns></returns>
        private bool CheckExisting()
        {
            if (grdMSAN.Rows.Count > 1 || cMSAN.MSANCabinet.Count > 0)
                switch (MessageBox.Show("Delete existing record and replace data from template", "Template", MessageBoxButtons.YesNoCancel))
                {
                    case DialogResult.Yes:
                        grdMSAN.Rows.Clear();
                        cmbShelf.Items.Clear();
                        cMSAN.MSANCabinet.Clear();

                        return true;
                    case DialogResult.No:
                        return true;
                    default:
                        return false;
                }
            else
                return true;

        }
        /// <summary>
        /// Apply template config to MSAN config
        /// </summary>
        private void ApplyTemplate(List<ListViewItem> listitems)
        {
            try
            {
                string frame_nis = "";
                clsMSAN.MSANFrame frame = new clsMSAN.MSANFrame();
                SortedDictionary<string, clsMSAN.MSANSlot> slot = new SortedDictionary<string, clsMSAN.MSANSlot>();

                for (int i = 0; i < listitems.Count; i++)
                {
                    ListViewItem l = listitems[i];
                    frame_nis = l.SubItems[1].Text; // RACK_NO / FRAME_NO

                    #region "set MSAN frame"
                    if (frame.frame_NIS != frame_nis)
                    {
                        if (frame.frame_NIS != null)
                            cMSAN.MSANCabinet[frame.frame_NIS] = frame; // keep changes
                        if (!cMSAN.MSANCabinet.ContainsKey(frame_nis))
                            NewFrame(frame_nis, l.SubItems[0].Text, l.SubItems[1].Text);

                        frame = cMSAN.MSANCabinet[frame_nis];
                        slot = frame.frame_slot;
                    }
                    #endregion

                    #region "copy card detail"
                    clsMSAN.MSANSlot c = new clsMSAN.MSANSlot();
                    c.inuse = true;
                    c.slot_no = l.SubItems[2].Text.Trim();
                    c.NIS = l.SubItems[3].Text.Trim();
                    c.type = l.SubItems[4].Text.Trim();
                    c.model = l.SubItems[5].Text.Trim();
                    // c.service = l.SubItems[6].Text.Trim();
                    c.status = l.SubItems[6].Text.Trim();
                    c.port_lo = l.SubItems[7].Text.Trim();
                    c.port_hi = l.SubItems[8].Text.Trim();
                    c.plant_unit = l.SubItems[9].Text.Trim();
                    c.manufacturer = txtManufacturer.Text.Trim();
                    #endregion

                    // ask whether to replace the current card.
                    // obsolete.. Kamal wants these existing data not to be damaged
                    // instead, add only when it does not exist.
                    //if (slot.ContainsKey(c.NIS))
                    //{
                    //    if (MessageBox.Show("Card already define for slot " + c.slot_no +
                    //        "\r\nOverwrite existing card with template card detail", "Duplicate NIS",
                    //        MessageBoxButtons.OKCancel) == DialogResult.OK)
                    //        slot[c.NIS] = c;
                    //}
                    //else
                    //    slot.Add(c.NIS, c);
                    if (!slot.ContainsKey(c.NIS))
                        slot.Add(c.NIS, c);

                    frame.frame_slot = slot;
                    frame.manufacturer = txtManufacturer.Text.Trim();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error Applying the Template\r\n" + ex.Message);
            }
            finally
            {
                InitTreeView();
                LoadShelfCombo();
            }
        }

        #endregion

        #region " 9. User edit MSAN Shelf / Slots / Card "

        #region " 9.1 Edit MSAN Shelf - edited : m.zam @ 13-05-2012 "

        private void btnShelfAdd_Click(object sender, EventArgs e)
        {
            this.TopMost = false;
            try
            {
                string frame_no = cmbShelf.Items.Count.ToString("00");
                string frame_NIS = (txtMSANRack.Text.Length == 0 ? "" : txtMSANRack.Text + "/");


                frame_NIS = InputBox.Show(
                    "Please enter NIS for the new shelf [" + frame_no + "] :", "Adding New Shelf",
                    frame_NIS + frame_no, -1, -1);

                if (frame_NIS.Length < 2)
                    MessageBox.Show("Invalid NIS format\r\nNIS must between 2 o 10 character");

                else if (cMSAN.MSANCabinet.ContainsKey(frame_NIS))
                    MessageBox.Show("The NIS entered is already define for other shelf");

                else
                {
                    NewFrame(frame_NIS, txtMSANRack.Text, frame_no);
                    cmbShelf.Items.Add(frame_NIS);
                    cmbShelf.SelectedIndex = cmbShelf.Items.Count - 1;

                    TreeNode n = trvCard.Nodes[0].Nodes.Add("SHELF " + frame_no
                        + " [" + frame_NIS + "]");
                    n.Tag = frame_NIS;
                }
            }
            catch
            {
            }
            this.TopMost = true;
        }

        /// <summary>
        /// only last shelf will be remove from the rack. 
        /// user confirmation is required
        /// </summary>
        private void btnShelfDel_Click(object sender, EventArgs e)
        {
            try
            {
                int i = cmbShelf.Items.Count - 1;
                if (i == -1)
                {
                    MessageBox.Show("The rack is already empty");
                }
                else if (!cMSAN.MSANCabinet.ContainsKey(cmbShelf.Text))
                {
                    cmbShelf.Items.RemoveAt(i);
                    grdMSAN.Rows.Clear();
                }
                else
                {
                    string msg = "Are your sure to delete shelf " + cmbShelf.Items[i].ToString();
                    if (MessageBox.Show(msg, "Delete Shelf", MessageBoxButtons.OKCancel) == DialogResult.OK)
                    {
                        cMSAN.MSANCabinet.Remove(cmbShelf.Items[i].ToString());
                        g_delframe.Add(cmbShelf.Items[i].ToString());
                        cmbShelf.Items.RemoveAt(i);
                        grdMSAN.Rows.Clear();
                        InitTreeView();
                    }
                }
            }
            catch { }
        }

        private void cmbShelf_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                //txtSlot.Text = "0";
                grdMSAN.Rows.Clear();
                for (int i = grdMSAN.SelectedCells.Count - 1; i > 0; i--)
                    grdMSAN.SelectedCells[i].Selected = false;
                Application.DoEvents();
                isLoadingMSAN = true;

                if (cmbShelf.SelectedIndex > -1)
                {
                    if (cMSAN.MSANCabinet.ContainsKey(cmbShelf.Text))
                    {
                        clsMSAN.MSANFrame f = cMSAN.MSANCabinet[cmbShelf.Text];

                        int row = 0;
                        foreach (clsMSAN.MSANSlot c in f.frame_slot.Values)
                        {
                            grdMSAN.Rows.Add();
                            grdMSAN.Rows[row].Cells[0].Value = c.slot_no.Trim();
                            grdMSAN.Rows[row].Cells[1].Value = c.NIS.Trim();
                            grdMSAN.Rows[row].Cells[2].Value = c.plant_unit.Trim();
                            grdMSAN.Rows[row].Cells[3].Value = c.type.Trim();
                            grdMSAN.Rows[row].Cells[4].Value = c.model.Trim();
                            grdMSAN.Rows[row].Cells[5].Value = c.status.Trim();
                            grdMSAN.Rows[row].Cells[6].Value = c.port_lo.Trim();
                            grdMSAN.Rows[row].Cells[7].Value = c.port_hi.Trim();

                            grdMSAN.Rows[row].Tag = c.NIS.Trim();
                            row++;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            finally
            {
                isLoadingMSAN = false;
            }
        }

        private void NewFrame(string frame_nis, string rack_no, string frame_no)
        {
            clsMSAN.MSANFrame frame = new clsMSAN.MSANFrame();
            frame.frame_NIS = frame_nis;
            frame.rack_no = rack_no;
            frame.frame_no = frame_no;
            frame.manufacturer = txtManufacturer.Text.Trim();
            frame.frame_slot = new SortedDictionary<string, clsMSAN.MSANSlot>();
            cMSAN.MSANCabinet.Add(frame_nis, frame);
        }

        private void LoadShelfCombo()
        {
            try
            {
                cmbShelf.Items.Clear();
                foreach (string frame_nis in cMSAN.MSANCabinet.Keys)
                    cmbShelf.Items.Add(frame_nis);

                if (cmbShelf.Items.Count > 0) cmbShelf.SelectedIndex = 0;
            }
            catch
            {
            }
        }

        #endregion

        #region " 9.1 Context Menu - 07-MAY-2012 "

        private void ctxCopy_Click(object sender, EventArgs e)
        {
            try
            {
                int count = grdMSAN.SelectedCells.Count;
                g_copiedcell.Clear();
                if (count > 0)
                {
                    g_copiedcol = (grdMSAN.SelectedCells[0].ColumnIndex - grdMSAN.SelectedCells[count - 1].ColumnIndex) + 1;
                    g_copiedrow = (grdMSAN.SelectedCells[0].RowIndex - grdMSAN.SelectedCells[count - 1].RowIndex) + 1;

                    for (int i = 0; i < grdMSAN.SelectedCells.Count; i++)
                    {
                        g_copiedcell.Insert(i, grdMSAN.SelectedCells[i].Value);
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
                int maxrow_index = grdMSAN.RowCount - 1;
                //int totalselect = grdMSAN.SelectedCells.Count;

                if (grdMSAN.SelectedCells[0].RowIndex == maxrow_index)
                {
                    MessageBox.Show("Please add new row before pasting as new record");
                }
                else if (g_copiedcell.Count > 0)
                {
                    if (g_copiedcell.Count == 1)
                    {
                        // fill up selected column with copied value
                        for (int i = 0; i < grdMSAN.SelectedCells.Count; i++)
                            grdMSAN.SelectedCells[i].Value = g_copiedcell[0];
                    }
                    else
                    {
                        int i = g_copiedcell.Count - 1;
                        int c = grdMSAN.SelectedCells[grdMSAN.SelectedCells.Count - 1].ColumnIndex;


                        for (int col = 0; col < g_copiedcol; col++, c++)
                        {
                            int r = grdMSAN.SelectedCells[grdMSAN.SelectedCells.Count - 1].RowIndex;
                            for (int row = 0; row < g_copiedrow; row++, r++, i--)
                            {
                                if (r < maxrow_index)
                                {
                                    grdMSAN.Rows[r].Cells[c].Value = g_copiedcell[i];
                                }
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
                if (grdMSAN.SelectedCells.Count > 0)
                {
                    for (int i = 0; i < grdMSAN.SelectedCells.Count; i++)
                    {
                        if (grdMSAN.SelectedCells[i].ValueType == typeof(int))
                            grdMSAN.SelectedCells[i].Value = 0;

                        else
                            grdMSAN.SelectedCells[i].Value = "";
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
                if (grdMSAN.SelectedCells.Count > 0)
                {
                    string val = InputBox.Show("Please enter value into selected column", "Fill Column", "", -1, -1);
                    for (int i = 0; i < grdMSAN.SelectedCells.Count; i++)
                    {
                        if (grdMSAN.SelectedCells[i].ValueType == typeof(int))
                            grdMSAN.SelectedCells[i].Value = ParseInt(val);

                        else
                            grdMSAN.SelectedCells[i].Value = val;

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
                if (cmbShelf.Text.Length == 0)
                {
                    MessageBox.Show("Please select/add a shelf before adding new row");
                }
                else
                {
                    isLoadingMSAN = true;
                    clsMSAN.MSANSlot s = AddingNewRow();
                    if (s.NIS != "error")
                    {
                        grdMSAN.Rows.Add();
                        int r = grdMSAN.NewRowIndex - 1;

                        grdMSAN.Rows[r].Tag = -1;
                        grdMSAN.Rows[r].Cells[0].Value = s.slot_no;
                        grdMSAN.Rows[r].Cells[1].Value = s.NIS;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Fail to add new record\r\n" + ex.Message);
            }
            finally
            {
                this.TopMost = true;
            }
            isLoadingMSAN = false;
        }

        private void ctxDelRow_Click(object sender, EventArgs e)
        {
            try
            {
                this.TopMost = false;
                if (grdMSAN.SelectedRows.Count == 0)
                {
                    MessageBox.Show("Please select rows to be deleted");
                }
                else if (MessageBox.Show("Please confirm to delete selected row",
                    "Delete Row", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    foreach (DataGridViewRow r in grdMSAN.SelectedRows)
                    {
                        string slotNIS = Utilities.CellValue(r.Cells[1].Value);
                        //int slotFID = (int)r.Tag;
                        grdMSAN.Rows.Remove(r);

                        clsMSAN.MSANFrame frame = cMSAN.MSANCabinet[cmbShelf.Text];
                        if (frame.frame_slot[slotNIS].slotFID > 0)
                        {
                            g_delrow.Add(frame.frame_slot[slotNIS].slotFID);
                        }
                        frame.frame_slot.Remove(slotNIS);
                        InitTreeView();
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

        #endregion

        #region " 9.2 edit MSAN Slot - m.zam @ 13-05-2012 "

        private void grdMSAN_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            try
            {

                int r = grdMSAN.Rows.Count - 2;
                if (r < 0) r = 0;
                DataGridViewRow row = grdMSAN.Rows[r];
                row.DefaultCellStyle.BackColor = (r % 2 == 0 ? Color.White : Color.LightGray);
                Utilities.SelectFirstItem(colCardStatus, row.Cells["colCardStatus"]);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Adding new row : " + ex.Message);
            }
        }

        private clsMSAN.MSANSlot AddingNewRow()
        {
            clsMSAN.MSANFrame f = cMSAN.MSANCabinet[grdMSAN.Tag.ToString()];
            clsMSAN.MSANSlot s = new clsMSAN.MSANSlot();
            s.NIS = "error";

            string slotno = InputBox.Show("1. Plaese enter slot number for new row", "Adding New Record", "00", -1, -1);
            string slotnis = InputBox.Show("2. Enter slot NIS for the new slot " + slotno, "Adding New Record", "00", -1, -1);

            if (slotnis.Length < 2)
                MessageBox.Show("Slot NIS must at least 2 character length");
            else if (f.frame_slot.ContainsKey(slotnis))
                MessageBox.Show("Slot NIS entered already used by other slot");
            else
            {
                s.NIS = slotnis;
                s.slot_no = slotno;
                s.slotFID = -1;
                s.cardFID = -1;

                f.frame_slot.Add(s.NIS, s);
                cMSAN.MSANCabinet[cmbShelf.Text] = f;

            }
            return s;
        }

        private clsMSAN.MSANSlot ApplyRow(DataGridViewRow r)
        {
            clsMSAN.MSANSlot c = new clsMSAN.MSANSlot();
            c.inuse = true;
            c.slot_no = Utilities.CellValue(r.Cells[colSlotNo.Index].Value);
            c.NIS = Utilities.CellValue(r.Cells[colSlotNIS.Index].Value);
            c.type = Utilities.CellValue(r.Cells[colCardTypeCombo.Name].Value);
            c.model = Utilities.CellValue(r.Cells[colCardModelCombo.Name].Value);
            c.status = Utilities.CellValue(r.Cells[colCardStatus.Name].Value);
            c.plant_unit = Utilities.CellValue(r.Cells[colPlantUnit.Name].Value);
            //            c.service = Utilities.CellValue(r.Cells[colCardService.Name].Value);
            c.port_lo = Utilities.CellValue(r.Cells[colPortLo.Name].Value);
            c.port_hi = Utilities.CellValue(r.Cells[colPortHi.Name].Value);
            c.port_num = ParseInt(c.port_hi) - ParseInt(c.port_lo) + 1;
            c.manufacturer = txtManufacturer.Text.Trim();
            return c;
        }




        #endregion

        #region " 9.3 edit MSAN Card "


        #endregion

        #region " 9.4 edit MSAN Port "

        private void InitTreeView()
        {
            string rt_type = trvCard.Nodes[0].Text;
            trvCard.Nodes.Clear();
            if (!rt_type.Contains(" [" + txtExchange.Text.Trim() + "_" + txtCode.Text.Trim() + "]"))
                rt_type += " [" + txtExchange.Text.Trim() + "_" + txtCode.Text.Trim() + "]";
            TreeNode root = trvCard.Nodes.Add(rt_type);// + " [" + txtExchange.Text.Trim() + "_" + txtCode.Text.Trim() + "]");
            foreach (clsMSAN.MSANFrame frame in cMSAN.MSANCabinet.Values)
            {
                TreeNode n = root.Nodes.Add("SHELF " + frame.frame_no
                    + " [" + frame.frame_NIS + "]");
                n.Tag = frame.frame_NIS;

                foreach (clsMSAN.MSANSlot card in cMSAN.MSANCabinet[frame.frame_NIS].frame_slot.Values)
                {
                    string txt = "SLOT " + card.slot_no;
                    if (card.NIS != null)
                        txt += " [" + card.type + "]";
                    else
                        txt += " [empty]";

                    TreeNode nd = n.Nodes.Add(txt);
                    nd.Tag = card.NIS;
                }
            }
            trvCard.ExpandAll();

            //trvCard.TreeViewNodeSorter = new clsTreeSorter();
            // trvCard.Sort();

            if (trvCard.Nodes.Count > 0)
            {
                if (trvCard.Nodes[0].GetNodeCount(false) > 0)
                    trvCard.SelectedNode = trvCard.Nodes[0].Nodes[0];
                else
                    trvCard.SelectedNode = trvCard.Nodes[0];
            }
        }

        private void trvCard_AfterSelect(object sender, TreeViewEventArgs e)
        {
            try
            {
                if (trvCard.SelectedNode != null)
                {
                    ShowCardList(trvCard.SelectedNode);
                    if (trvCard.SelectedNode.Text.IndexOf("SLOT") > -1) // it's a slot

                        for (int i = 0; i < grdMSAN.SelectedRows.Count; i++)
                            grdMSAN.SelectedRows[i].Selected = false;

                    foreach (DataGridViewRow r in grdMSAN.Rows)
                    {
                        if (r.Tag == trvCard.SelectedNode.Tag)
                        {
                            r.Selected = true;
                            return;
                        }
                    }

                }
            }
            catch (Exception ex)
            {
            }
        }

        private void ShowCardList(TreeNode nd)
        {
            string frame_NIS;

            if (nd.Text.IndexOf("SLOT") > -1)
            { // it's a slot
                frame_NIS = nd.Parent.Tag.ToString();
            }
            else
            {
                frame_NIS = nd.Tag.ToString();
            }
            if (grdMSAN.Tag == null) grdMSAN.Tag = -1;

            if (frame_NIS != grdMSAN.Tag.ToString())
            {
                int row = 0;
                isLoadingMSAN = true;
                grdMSAN.Rows.Clear();
                grdMSAN.Tag = frame_NIS;
                cmbShelf.Text = frame_NIS;

                foreach (clsMSAN.MSANSlot c in cMSAN.MSANCabinet[frame_NIS].frame_slot.Values)
                {
                    grdMSAN.Rows.Add();
                    grdMSAN.Rows[row].Cells[0].Value = Utilities.CellValue(c.slot_no);
                    grdMSAN.Rows[row].Cells[1].Value = Utilities.CellValue(c.NIS);
                    grdMSAN.Rows[row].Cells[2].Value = Utilities.CellValue(c.plant_unit);
                    grdMSAN.Rows[row].Cells[3].Value = Utilities.CellValue(c.type);
                    grdMSAN.Rows[row].Cells[4].Value = Utilities.CellValue(c.model);
                    grdMSAN.Rows[row].Cells[5].Value = Utilities.CellValue(c.status);
                    grdMSAN.Rows[row].Cells[6].Value = Utilities.CellValue(c.port_lo);
                    grdMSAN.Rows[row].Cells[7].Value = Utilities.CellValue(c.port_hi);

                    grdMSAN.Rows[row].Tag = Utilities.CellValue(c.NIS);
                    //Application.DoEvents();
                    row++;
                }
                isLoadingMSAN = false;

            }
        }

        #endregion

        #region "DataGrid Cell / ComboBox Event"
        Dictionary<int, DataGridViewRow> chgRow = new Dictionary<int, DataGridViewRow>();

        DataGridViewCell gCell;
        private void grdMSAN_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            gCell = grdMSAN.Rows[e.RowIndex].Cells[e.ColumnIndex];
        }

        private void grdMSAN_KeyDown(object sender, KeyEventArgs e)
        {
            if (grdMSAN.Columns[gCell.ColumnIndex] is DataGridViewComboBoxColumn)
            {
                e.SuppressKeyPress = true;
                m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Please select value from the dropdown list");
            }
        }

        private void grdMSAN_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                this.TopMost = false;
                if (e.RowIndex == -1) return;

                if (grdMSAN.Columns[e.ColumnIndex] is DataGridViewComboBoxColumn)
                {
                    string val = grdMSAN.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString();
                    DataGridViewComboBoxColumn cmbCol = (DataGridViewComboBoxColumn)grdMSAN.Columns[e.ColumnIndex];

                    if (!cmbCol.Items.Contains(val)) //check if item is already in drop down, if not, add it to all
                    {
                        cmbCol.Items.Add(val);
                        this.grdMSAN.CurrentCell.Value = val;
                    }

                    if (cmbCol.Name == "colPlantUnit")
                    {
                        string ssql = "SELECT CARD_TYPE,CARD_MODEL FROM REF_FCARD_MSAN WHERE MIN_MATERIAL='" + val + "' and MSAN_FNO=" + SELECTED_FNO + " and MANUFACTURER='" + txtManufacturer.Text + "'";


                        int rec_count = 0;
                        this.m_IGTDataContext = GTClassFactory.Create<IGTApplication>().DataContext;
                        ADODB.Recordset rs = m_IGTDataContext.Execute(ssql, out rec_count, (int)CommandTypeEnum.adCmdText, null);

                        if (rs != null)
                        {

                            rs.MoveFirst();
                            grdMSAN.Rows[e.RowIndex].Cells[e.ColumnIndex + 1].Value = Utilities.rsField(rs, "CARD_TYPE");
                            grdMSAN.Rows[e.RowIndex].Cells[e.ColumnIndex + 2].Value = Utilities.rsField(rs, "CARD_MODEL");
                            grdMSAN.Rows[e.RowIndex].Cells[e.ColumnIndex + 4].Value = "";
                            grdMSAN.Rows[e.RowIndex].Cells[e.ColumnIndex + 5].Value = "";
                            rs = null;
                        }
                    }
                }
                if (e.ColumnIndex == 2) LoadBOM();

                if (!isLoadingMSAN)
                {
                    clsMSAN.MSANSlot s = ApplyRow(grdMSAN.Rows[e.RowIndex]);
                    clsMSAN.MSANFrame f = cMSAN.MSANCabinet[cmbShelf.Text];
                    if (s.NIS.Length > 0)
                    {
                        if (f.frame_slot.ContainsKey(s.NIS))
                            f.frame_slot[s.NIS] = s;
                        else
                            f.frame_slot.Add(s.NIS, s);
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
                    if (gCell == null || !gCell.IsInEditMode)
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
                DataGridViewComboBoxColumn comboColumn = this.grdMSAN.Columns[this.grdMSAN.CurrentCell.ColumnIndex] as DataGridViewComboBoxColumn;
                combo.Items.Add(combo.Text);
                comboColumn.Items.Add(combo.Text);
                this.grdMSAN.CurrentCell.Value = combo.Text;
            }
        }

        #endregion

        #region BOM
        private struct BOM
        {
            public string ComponentID;
            public int GEMS;
            public string Description;

            public BOM(int itemNo, string componentID, string description)
            {
                this.ComponentID = componentID;
                this.GEMS = itemNo;
                this.Description = description;
            }
            // Override the ToString method:
            public override string ToString()
            {
                return (String.Format("{0}({1},{2})", Description, GEMS.ToString(), ComponentID));
            }
        }


        Dictionary<string, BOM> CardBOM = new Dictionary<string, BOM>();

        private void InitBOM()
        {
            CardBOM.Clear();

            //Dust Cover (Front, Sides and Rear)
            CardBOM.Add("BLANK_415", new BOM(415, "21133383", "Blank panel for HW transfer board/EFTF board slot (Shield)"));
            CardBOM.Add("BLANK_416", new BOM(416, "21133384", "Blank panel for transfer board/HLSF/HLEF/HLSF/E1TF/HABD transfer board slot (Shield)"));
            CardBOM.Add("BLANK_417", new BOM(417, "21133505", "Blank panel for BB main control board and power board slot"));
            CardBOM.Add("BLANK_418", new BOM(418, "21131422", "All-purpose blank panel (For UA5000)"));

            //Power Card
            CardBOM.Add("POWER", new BOM(435, "03037641", "Secondary Power Board"));

            //Control Card
            CardBOM.Add("CONTROL_8E1", new BOM(440, "03039850", "Remote Subscriber Processing Board(8E1)"));
            CardBOM.Add("CONTROL_V5", new BOM(441, "03036767", "V5 Protocol Processing Unit"));
            CardBOM.Add("CONTROL_IP", new BOM(443, "03020EQJ", "IP Service Process Board of Master Frame"));
            CardBOM.Add("CONTROL_TRANSCEIVER", new BOM(497, "34060286", "Optical Transceiver,Enhanced SFP,850nm,2.125G Series ,-2.5~-9.5dBm,-17dBm,LC,0.5Km"));

            //Communication Card
            CardBOM.Add("POTS", new BOM(460, "03033734", "32-Channel, 48V Analog Subscriber Board"));
            CardBOM.Add("POTS (Non-V5)", new BOM(525, "03031337", "Direct Dial In Subscriber Interface Board"));
            CardBOM.Add("ADSL2+", new BOM(476, "03030DGQ", "32-port ADSL2+ Board"));
        }

        private void LoadBOM()
        {
            DataGridViewRow r = grdMSAN.Rows[gCell.RowIndex];
            BOM b;
            switch (r.Cells["colCardType"].Value.ToString())
            {
                case "POWER CARD":
                    b = CardBOM["POWER"];
                    break;
                case "CONTROL CARD":
                    b = CardBOM["CONTROL_V5"];
                    break;
                case "ADSL":
                    b = CardBOM["ADSL2+"];
                    break;
                case "POTS":
                    b = CardBOM["POTS"];
                    break;
                default:
                    b = CardBOM["BLANK_418"];
                    break;
            }
            r.Cells["colBOM"].Value = b.ComponentID;
            r.Cells["colGEMS"].Value = b.GEMS;
        }

        #endregion

        #endregion

        #region Database Menu
        private void btnCommit_Click(object sender, EventArgs e)
        {
            try
            {
                this.TopMost = false;
                if (MessageBox.Show("Current changes will be saved to database ?", "Commit", MessageBoxButtons.OKCancel) != DialogResult.OK)
                {
                    return;
                }
                this.Cursor = Cursors.WaitCursor;

                if (gCell.IsInEditMode)
                {
                    DataGridViewDataErrorContexts c = new DataGridViewDataErrorContexts();
                    gCell.DataGridView.CommitEdit(c);
                }

                cMSAN.DeleteMSAN_Frame(g_delframe, SELECTED_FID);
                cMSAN.DeleteMSAN_Slot(g_delrow);

                int iRecordsAffected;
                if (cMSAN.SaveMSAN())
                {
                    GTClassFactory.Create<IGTApplication>().DataContext.Execute(
                         "COMMIT", out iRecordsAffected, (int)ADODB.CommandTypeEnum.adCmdText);

                    MessageBox.Show("Success : Changes has been saved to database");

                    hasBeenSaved = true; // update the flag
                }
                else
                {
                    GTClassFactory.Create<IGTApplication>().DataContext.Execute(
                        "ROLLBACK", out iRecordsAffected, (int)ADODB.CommandTypeEnum.adCmdText);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error updating changes to database\r\n" + ex.Message, "MSAN COnfiguration", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                this.Cursor = Cursors.Default;
                this.TopMost = true;
            }
        }

        private void btnRollback_Click(object sender, EventArgs e)
        {
            this.TopMost = false;
            if (MessageBox.Show("All changes will be cancel. Reload data from database", "Rollback", MessageBoxButtons.OKCancel) != DialogResult.OK)
            {
                this.Cursor = Cursors.WaitCursor;
                cmbShelf_SelectedIndexChanged(sender, e);
                return;
            }
            this.Cursor = Cursors.Default;
            this.TopMost = true;
        }

        private void GTMSANForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!hasBeenSaved)
            {
                if (MessageBox.Show("Are you sure you want to close?",
                    "Confirmation", MessageBoxButtons.YesNo) == DialogResult.No)
                {
                    e.Cancel = true;
                    return;
                }
            }

            GTClassFactory.Create<IGTApplication>().EndWaitCursor();

        }

        private void mnuReload_Click(object sender, EventArgs e)
        {
            InitTreeView();
        }

        #endregion








    }
}