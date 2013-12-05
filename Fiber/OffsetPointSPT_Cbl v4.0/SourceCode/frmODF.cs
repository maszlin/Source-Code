using ADODB;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Intergraph.GTechnology.API;

namespace AG.GTechnology.OffsetPointSPT_Cbl
{
    public partial class frmODF : Form
    {
        private static Intergraph.GTechnology.API.IGTApplication application = GTClassFactory.Create<IGTApplication>();
        private IGTDataContext oDataContext = application.DataContext;

        private string _ExcAbb = string.Empty;
        public string EXC_ABB
        {
            set { _ExcAbb = value; }
        }

        private int _ODF_FID = 0;
        public int ODF_FID
        {
            get { return _ODF_FID; }
        }
        private List<short> _ODF_FNO = new List<short>();
        public List<short> ODF_FNO
        {
            set { _ODF_FNO = value; }
            get { return _ODF_FNO; }
        }

        private short _NR_DIRECTION = 0;
        public short NR_DIRECTION
        {
            get { return _NR_DIRECTION; }
        }

        public frmODF()
        {
            InitializeComponent();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            this.Hide();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (lstODF.SelectedItems.Count > 0)
            {
                _ODF_FID = Convert.ToInt32(lstODF.SelectedItems[0].Tag);
                if (lstODF.SelectedItems[0].Text.Contains("ODF"))
                {
                    _ODF_FNO[0] = 5500;
                }
                if (lstODF.SelectedItems[0].Text.Contains("FTB"))
                {
                    _ODF_FNO[0] = 12200;
                }
            }

            DialogResult = DialogResult.OK;
            this.Hide();
        }

        private void frmODF_Load(object sender, EventArgs e)
        {
            string sSql = null;
            int recordsAffected = 0;

            //mike: set default value.
            chkbox_nr_conn.Checked = true;
            chkbox_nr_conn2.Checked = false;
            //chkbox_nr_conn.Text = "ODF/FTB connects to Cable";
            //this.Text = "ODF/FTB";
            //label1.Text = "Specify the source ODF/FTB in Exchange:";
            //lstODF.Columns[0].Text = "ODF/FTB";
            lstODF.Columns[0].Width = 200;
            lstODF.Columns[1].Width = 50;
            _NR_DIRECTION = 13;
            string OwenerName = "";

            try
            {

                this.Text = "Exchange: " + _ExcAbb;
                for (int j = 0; j < _ODF_FNO.Count; j++)
                {
                    if (_ODF_FNO[j] == 5500)
                    {
                        
                        OwenerName = "ODF";
                        sSql = "select a.* from GC_ODF a, GC_NETELEM b where a.g3e_fid=b.g3e_fid and b.EXC_ABB='" + _ExcAbb + "'";
                        ADODB.Recordset rsODF = application.DataContext.Execute(sSql, out recordsAffected, (int)CommandTypeEnum.adCmdText, null);
                        while (!rsODF.EOF)
                        {
                            string sODF_NUM = string.Empty;
                            string sHORZ_NUM = string.Empty;
                            string sODF_FID = string.Empty;
                            sODF_NUM = rsODF.Fields["ODF_NUM"].Value.ToString();
                            sHORZ_NUM = rsODF.Fields["HORZ_NUM"].Value.ToString();
                            sODF_FID = rsODF.Fields["G3E_FID"].Value.ToString();

                            ListViewItem item = lstODF.Items.Add("ODF #" + sODF_NUM + " (" + sODF_FID + ")");
                            //item.SubItems.Add(sHORZ_NUM);
                            item.Tag = Convert.ToInt32(sODF_FID);
                            rsODF.MoveNext();
                        }
                        rsODF = null;
                        break;
                    }
                }
                for (int j = 0; j < _ODF_FNO.Count; j++)
                {
                    if (_ODF_FNO[j] == 12200)
                    {
                        if (OwenerName == "") OwenerName = "FTB";
                        else OwenerName = "/FTB";
                        sSql = "select a.* from GC_FPATCHPANEL a, GC_NETELEM b where a.g3e_fid=b.g3e_fid and b.EXC_ABB='" + _ExcAbb + "'";
                        ADODB.Recordset rsFTB = application.DataContext.Execute(sSql, out recordsAffected, (int)CommandTypeEnum.adCmdText, null);
                        while (!rsFTB.EOF)
                        {
                            string sFTB_NUM = string.Empty;
                            string sFTB_FID = string.Empty;
                            sFTB_NUM = rsFTB.Fields["PATCH_CODE"].Value.ToString();
                            sFTB_FID = rsFTB.Fields["G3E_FID"].Value.ToString();

                            ListViewItem item = lstODF.Items.Add("FTB #" + sFTB_NUM + " (" + sFTB_FID + ")");
                            item.Tag = Convert.ToInt32(sFTB_FID);
                            rsFTB.MoveNext();
                        }
                        rsFTB = null;
                        break;
                    }
                }

                chkbox_nr_conn.Text = OwenerName+" connects to Cable";
                label1.Text = "Specify the source " + OwenerName + " in Exchange:";
                lstODF.Columns[0].Text = OwenerName;
                chkbox_nr_conn.Text = OwenerName+" connects to Cable";
                chkbox_nr_conn2.Text = "Cable connects to " + OwenerName;

            }
            catch
            {
            }

        }

        private void chkbox_nr_conn_CheckedChanged(object sender, EventArgs e)
        {
            if ( chkbox_nr_conn.Checked )
            {
                _NR_DIRECTION = 13;
                chkbox_nr_conn2.Checked = false;
            }
           
        }

        private void chkbox_nr_conn2_CheckedChanged(object sender, EventArgs e)
        {
            if (chkbox_nr_conn2.Checked)
            {  
                _NR_DIRECTION = 8;
                chkbox_nr_conn.Checked = false;
            }
        }


    }
}