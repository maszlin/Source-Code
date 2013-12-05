using ADODB;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Intergraph.GTechnology.API;
using System.Windows.Forms.DataVisualization.Charting;

namespace NEPS.GTechnology.NEPSDuctNestPlc
{
    public partial class DuctnestConfigForm : Form
    {
        private Intergraph.GTechnology.API.IGTApplication application;
        private Logger log;
        private int ductwayfree = 0;
        List<DuctFormationCfg> _lstductcfg = new List<DuctFormationCfg>();
       
        public DuctnestConfigForm(IGTCustomCommandHelper customCommandHelper)
        {
            InitializeComponent();
            application = GTClassFactory.Create<IGTApplication>();

            log = Logger.getInstance();

            Shown += new EventHandler(DuctnestConfigForm_Shown);
        }

        private int _ductpath_fid = 0;
        public int DUCTPATH_FID
        {
            set { _ductpath_fid = value; }
            get { return _ductpath_fid; }
        }

        private DuctNest _ductnest = null;
        public DuctNest DUCT_NEST
        {
            set { _ductnest = value; }
            get { return _ductnest; }
        }

        private void EnableControls()
        {
            bool blnDefaultList = true;
            bool btnAddPair = true;
            bool blnPlaceDuctnest = true;
            if (_ductnest == null)
                blnDefaultList = false;
            else
            {
                if ((_ductnest.From_Cols != _ductnest.To_Cols) ||
                    (_ductnest.From_Rows != _ductnest.To_Rows))
                {
                    blnDefaultList = false;
                    btnAddPair = false;
                }
                
                if ((_ductnest.From_Cols * _ductnest.From_Rows != _ductnest.To_Cols * _ductnest.To_Rows)
                    || (_ductnest.DuctMapping.Count != _ductnest.From_Cols * _ductnest.From_Rows)
                    || (_ductnest.DuctMapping.ContainsKey("(none)"))
                    || (_ductnest.DuctMapping.ContainsValue("(none)"))
                    )
                    blnPlaceDuctnest = false;
            }

            if (lvwMapping.Items.Count > 0) blnDefaultList = false;

            btnClearList.Enabled = (lvwMapping.Items.Count > 0);
            btnDefaultList.Enabled = blnDefaultList;
            btnPlaceDuctnest.Enabled = blnPlaceDuctnest;
        }

        private void LoadDuctGraphic(DuctNest _ductnest)
        {
            panelLeft.Controls.Clear();
            for (int col = 0; col < _ductnest.From_Cols; col++)
            {
                for (int row = 0; row < _ductnest.From_Rows; row++)
                {
                    panelLeft.Controls.Add(_ductnest.GetDuctByXY(col, row, 0));
                }
            }
            panelRight.Controls.Clear();
            for (int col = 0; col < _ductnest.To_Cols; col++)
            {
                for (int row = 0; row < _ductnest.To_Rows; row++)
                {
                    panelRight.Controls.Add(_ductnest.GetDuctByXY(col, row, 1));
                }
            }
            _ductnest.DuctClick += new EventHandler(btnDuct_Click);
            EnableControls();
        }

        private void LoadDuctnestLayout(string configName)
        {
            try
            {
                lvwMapping.Items.Clear();
                panelLeft.Controls.Clear();
                panelRight.Controls.Clear();
                foreach (DuctFormationCfg _ductcfg in _lstductcfg)
                {
                    if (configName == _ductcfg.Text)
                    {
                        _ductnest = new DuctNest(_ductcfg.Cols, _ductcfg.Rows);
                        LoadDuctGraphic(_ductnest);
                        nmrFromRow.Minimum = 1;
                        nmrFromCol.Minimum = 1;
                        nmrFromRow.Maximum = ductwayfree+1;//_ductcfg.Cols * _ductcfg.Rows;
                        nmrFromCol.Maximum = ductwayfree+1;//_ductcfg.Cols * _ductcfg.Rows;
                        nmrFromCol.Value = _ductcfg.Cols;
                        nmrFromRow.Value = _ductcfg.Rows;

                        nmrToRow.Minimum = 1;
                        nmrToCol.Minimum = 1;
                        nmrToRow.Maximum = ductwayfree+1;//_ductcfg.Cols * _ductcfg.Rows;
                        nmrToCol.Maximum = ductwayfree+1;//_ductcfg.Cols * _ductcfg.Rows;
                        nmrToCol.Value = _ductcfg.Cols;
                        nmrToRow.Value = _ductcfg.Rows;
                    }
                }
            }
            catch (Exception ex) { }
        }

        private void EnableDucts()
        {
            if (_ductnest == null) return;
            _ductnest.EnableDucts();
        }

        private void AddDuctToList(string fromDuct, string toDuct)
        {
            int imgIdx = 0;
            ListViewItem item = null;
            if (lvwMapping.Items.Count == 0)
            {
                item = lvwMapping.Items.Add(fromDuct);
                item.SubItems.Add(toDuct);

                if (fromDuct == "(none)")
                    imgIdx = 0;
                else if (toDuct == "(none)")
                    imgIdx = 1;
                else
                    imgIdx = 2;
                item.ImageIndex = imgIdx;
            }
            else
            {
                item = lvwMapping.Items[lvwMapping.Items.Count - 1];
                if (item.Text == "(none)")
                {
                    item.Text = fromDuct;
                    if (toDuct != "(none)") item.SubItems[1].Text = toDuct;
                }
                else if (item.SubItems[1].Text == "(none)")
                {
                    if (fromDuct != "(none)") item.Text = fromDuct;
                    item.SubItems[1].Text = toDuct;
                }
                else
                {
                    item = lvwMapping.Items.Add(fromDuct);
                    item.SubItems.Add(toDuct);
                }

                fromDuct = item.Text;
                toDuct = item.SubItems[1].Text;

                if (fromDuct == "(none)")
                    imgIdx = 0;
                else if (toDuct == "(none)")
                    imgIdx = 1;
                else
                    imgIdx = 2;
                item.ImageIndex = imgIdx;
            }
            //_ductnest.AddMapping(fromDuct, toDuct);
            _ductnest.DuctMapping.Clear();
            foreach (ListViewItem _item in lvwMapping.Items)
            {
                _ductnest.DuctMapping.Add(_item.Text, _item.SubItems[1].Text);
               // _ductnest.DuctMapping
              //  DuctPosition temp = new DuctPosition();

            }
            _ductnest.EnableDucts();

            EnableControls();
        }

        private void btnDuct_Click(object sender, EventArgs e)
        {
            string ductName = ((Button)sender).Text;

            string fromDuct;
            string toDuct;

            DuctCtrl _duct = (DuctCtrl)((Button)sender).Tag;
            if (_duct.Type == ConnectType.from)
            {
                fromDuct = _duct.Text;
                toDuct = "(none)";
            }
            else
            {
                fromDuct = "(none)";
                toDuct = _duct.Text;
            }
            AddDuctToList(fromDuct, toDuct);
            _duct.Enabled = false;
        }

        private void btnCfg_Click(object sender, EventArgs e)//button like 1x1, click 
        {
            string configName = ((Button)sender).Text;//label of the button like 1x1,
            LoadDuctnestLayout(configName);
        }

        public void LoadDuctnestConfig(string fromsourcename, string tosourcename)
        {
            Recordset rsComp = null;
            int recordsAffected = 0;
            int numberofDucts = 0;
            int numberofDuctsInNest = 0;
            //rsComp = application.DataContext.Execute("QUERY_DUCTNEST", out recordsAffected, (int)CommandTypeEnum.adCmdStoredProc + (int)ADODB.ExecuteOptionEnum.adExecuteNoRecords, PathFid);

            if (_ductpath_fid == 0) return;
            string sSql = "select nvl(b.DT_WAYS,0) DT_WAYS, nvl(b.DT_NEST_WAYS,0) DT_NEST_WAYS from GC_COND b where b.G3E_FID=" + _ductpath_fid.ToString();
            rsComp = application.DataContext.Execute(sSql, out recordsAffected, (int)CommandTypeEnum.adCmdText, null);

            if (rsComp != null)
            {
                if (!rsComp.EOF)
                {
                    if (rsComp.Fields["DT_WAYS"].Value != DBNull.Value) numberofDucts = Convert.ToInt32(rsComp.Fields["DT_WAYS"].Value);
                    if (rsComp.Fields["DT_NEST_WAYS"].Value != DBNull.Value) numberofDuctsInNest = Convert.ToInt32(rsComp.Fields["DT_NEST_WAYS"].Value);

                }
            }
            rsComp = null;

            numberofDucts = numberofDucts - numberofDuctsInNest;

            if (numberofDucts <= 0) numberofDucts = 10;
            ductwayfree = numberofDucts-1;
            // Load duct configuration
            _lstductcfg.Clear();
            sSql = "select b.* from G3E_DUCTFORMATION b where b.G3E_DFNO=2300 and b.G3E_NUMROWS*b.G3E_NUMCOLS <= " + numberofDucts.ToString() + " ORDER BY b.G3E_NUMROWS*b.G3E_NUMCOLS";
            rsComp = application.DataContext.Execute(sSql, out recordsAffected, (int)CommandTypeEnum.adCmdText, null);

            int row = 0;
            if (rsComp != null)
            {
                while (!rsComp.EOF)
                {
                    DuctFormationCfg _ductcfg = new DuctFormationCfg();
                    if (rsComp.Fields["G3E_USERNAME"].Value != DBNull.Value) _ductcfg.Text = rsComp.Fields["G3E_USERNAME"].Value.ToString();
                    if (rsComp.Fields["G3E_NUMROWS"].Value != DBNull.Value) _ductcfg.Rows = Convert.ToInt32(rsComp.Fields["G3E_NUMROWS"].Value);
                    if (rsComp.Fields["G3E_NUMCOLS"].Value != DBNull.Value) _ductcfg.Cols = Convert.ToInt32(rsComp.Fields["G3E_NUMCOLS"].Value);
                    _lstductcfg.Add(_ductcfg);
                    Button btnCfg = new Button();
                    btnCfg.Text = _ductcfg.Text;
                    btnCfg.Top = row * (btnCfg.Height + 5);
                    btnCfg.Left = 10;
                    btnCfg.Tag = _ductcfg;
                    btnCfg.Click += new EventHandler(btnCfg_Click);
                    panelLayout.Controls.Add(btnCfg);

                    row++;
                    rsComp.MoveNext();
                }
            }
            rsComp = null;
            lblFromManhole.Text = "Nest From " + fromsourcename;
            lblToManhole.Text = "Nest To " + tosourcename;
        }

        /// <summary>
        /// Sets the focus to the valueTextBox when this form is shown.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DuctnestConfigForm_Shown(object sender, EventArgs e)
        {
            application.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Loading DuctNest Placement Configuration form...");
        }

        #region Getters/Setters

        public Logger Logger
        {
            set
            { log = value; }
        }

        #endregion

        public new event EventHandler PlaceClick
        {
            add
            {
                this.btnPlaceDuctnest.Click += value;
            }
            remove
            {
                this.btnPlaceDuctnest.Click -= value;
            }
        }

        public string Msg
        {
            set
            {
                lblMsg.Text = value;
            }
        }

        private void btnAddPair_Click(object sender, EventArgs e)
        {
            if (_ductnest == null) return;
            if (_ductnest.Total_duct == 0) return;
            if ((_ductnest.From_Cols == _ductnest.To_Cols) &&
                (_ductnest.From_Rows == _ductnest.To_Rows))
            {
            string pair = txtDuctPair.Text;
            string[] pairs = pair.Split(' ');
            if (pairs.Length < 2) return;
            string fromDuct = pairs[0];
            string toDuct = pairs[1];

            txtDuctPair.Text = string.Empty;
            foreach (ListViewItem _item in lvwMapping.Items)
            {
                if (_item.Text == fromDuct) return;
                if (_item.SubItems[1].Text == toDuct) return;
            }
            AddDuctToList(fromDuct, toDuct);
        }
        else
            btnAddPair.Enabled = false;
        }

        private void btnDefaultList_Click(object sender, EventArgs e)
        {
            if (_ductnest == null)return;
            if (_ductnest.Total_duct == 0) return;
            if ((_ductnest.From_Cols == _ductnest.To_Cols) &&
                (_ductnest.From_Rows == _ductnest.To_Rows))
            {
                lvwMapping.Items.Clear();
                _ductnest.DuctMapping.Clear();
                foreach (DuctCtrl _duct in _ductnest.FromDucts)
                {
                    AddDuctToList(_duct.Text, _duct.Text);
                }
            }
            else
                btnDefaultList.Enabled = false;
        }

        private void btnClearList_Click(object sender, EventArgs e)
        {
            lvwMapping.Items.Clear();
            if (_ductnest != null) _ductnest.DuctMapping.Clear();
            if ((_ductnest.From_Cols == _ductnest.To_Cols) &&
                (_ductnest.From_Rows == _ductnest.To_Rows))
            {
                btnDefaultList.Enabled = true;
                btnAddPair.Enabled = true;
            }
            else
            {
                btnDefaultList.Enabled = false;
                btnAddPair.Enabled = false;
            }
            _ductnest.EnableDucts();
        }

        private void nmrFromRow_ValueChanged(object sender, EventArgs e)
        {
            if (_ductnest == null) return;
            //int max = ductwayfree - (int)nmrFromRow.Value;
            //if (max < 0)
            //    nmrFromCol.Maximum = max * (-1);
            //else if (max == 0)
            //    nmrFromCol.Maximum = ductwayfree + 1;
            //else nmrFromCol.Maximum = max;

            _ductnest.From_Rows = (int)nmrFromRow.Value;
            if (_ductnest.Total_duct == 0)
            {
                nmrFromCol.BackColor = Color.Red;
                nmrFromRow.BackColor = Color.Red;
                nmrToCol.BackColor = Color.Red;
                nmrToRow.BackColor = Color.Red;
            }
            else
            {
                nmrFromCol.BackColor = Color.White;
                nmrFromRow.BackColor = Color.White;
                nmrToCol.BackColor = Color.White;
                nmrToRow.BackColor = Color.White;
                _ductnest.InitLayout();
                LoadDuctGraphic(_ductnest);
            }

           
        }

        private void nmrFromCol_ValueChanged(object sender, EventArgs e)
        {
            if (_ductnest == null) return;
            _ductnest.From_Cols = (int)nmrFromCol.Value;
            if (_ductnest.Total_duct == 0)
            {
                nmrFromCol.BackColor = Color.Red;
                nmrFromRow.BackColor = Color.Red;
            }
            else
            {
                nmrFromCol.BackColor = Color.White;
                nmrFromRow.BackColor = Color.White;
                nmrToCol.BackColor = Color.White;
                nmrToRow.BackColor = Color.White;
                _ductnest.InitLayout();
                LoadDuctGraphic(_ductnest);
            }
            //int max = ductwayfree - (int)nmrFromCol.Value;
            //if (max < 0)
            //    nmrFromRow.Maximum = max * (-1);
            //else if (max == 0)
            //    nmrFromRow.Maximum = ductwayfree + 1;
            //else nmrFromRow.Maximum = max;
        }

        private void nmrToRow_ValueChanged(object sender, EventArgs e)
        {
            if (_ductnest == null) return;
            _ductnest.To_Rows = (int)nmrToRow.Value;
            if (_ductnest.Total_duct == 0)
            {
                nmrToCol.BackColor = Color.Red;
                nmrToRow.BackColor = Color.Red;
            }
            else
            {
                nmrFromCol.BackColor = Color.White;
                nmrFromRow.BackColor = Color.White;
                nmrToCol.BackColor = Color.White;
                nmrToRow.BackColor = Color.White;
                _ductnest.InitLayout();
                LoadDuctGraphic(_ductnest);
            }
           //int max = ductwayfree - (int)nmrToRow.Value;
           // if (max < 0)
           //     nmrToCol.Maximum = max * (-1);
           // else if (max == 0)
           //     nmrToCol.Maximum = ductwayfree + 1;
           // else nmrToCol.Maximum = max;
        }

        private void nmrToCol_ValueChanged(object sender, EventArgs e)
        {
            if (_ductnest == null) return;
            _ductnest.To_Cols = (int)nmrToCol.Value;
            if (_ductnest.Total_duct == 0)
            {
                nmrToCol.BackColor = Color.Red;
                nmrToRow.BackColor = Color.Red;
            }
            else
            {
                nmrFromCol.BackColor = Color.White;
                nmrFromRow.BackColor = Color.White;
                nmrToCol.BackColor = Color.White;
                nmrToRow.BackColor = Color.White;
                _ductnest.InitLayout();
                LoadDuctGraphic(_ductnest);
            }
            //int max = ductwayfree - (int)nmrToCol.Value;
            //if (max < 0)
            //    nmrToRow.Maximum = max * (-1);
            //else if (max == 0)
            //    nmrToRow.Maximum = ductwayfree + 1;
            //else nmrToRow.Maximum = max;
        }

        private void DuctnestConfigForm_Load(object sender, EventArgs e)
        {
            //lblFromManhole.Text = GTDuctnestPlac

            //string name = FeatureName(toFNO, toFID);

            //if (name == "Manhole")
            //    name += " ID=" + Get_Value("select MANHOLE_ID from GC_MANHL where g3e_fid = " + fromFID.ToString());
            //else name += " FID=" + fromFID.ToString();

            //lblToManhole.Text = "To " + name;
        }

    }
}