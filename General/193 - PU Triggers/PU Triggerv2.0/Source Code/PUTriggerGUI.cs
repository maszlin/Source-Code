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
using System.Reflection;

namespace NEPS.GTechnology.PUTrigger
{
    public partial class frmPUTrigger : Form
    {
        #region variables
        private IGTDataContext m_GTDataContext = null;
        private Intergraph.GTechnology.API.IGTApplication m_gtapp = null;

        public int PlaceValue = 0;

        //related feature info
        public string vFeature1 = null;
        public string vFeature2 = null;
        private short featFNO1 = 0;
        private short featFNO2 = 0;
        private int featFID1 = 0;
        private int featFID2 = 0;
        //
        //possible relat features
        public class vFeat
        {
            public string name;
            public short FNO;
            public string feat_state;
        };
        private List<vFeat> FeatList1 = null;
        private List<vFeat> FeatList2 = null;
        //

        public int StyleId = 16711935;

        IGTKeyObject oNewFeature = null;

        private const short PUTriggerFNO = 5000;
        private const short PUTriggerAttribCNO = 5001;
        private const short PUTriggerNetElemCNO = 51;
        private const short PUTriggerPriGeoCNO = 5020;

       
        #endregion

        #region Form Init and Load

        public frmPUTrigger()
        {
            try
            {
                               
                InitializeComponent();
                m_gtapp = GTPUTrigger.application;
                m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Running PU Trigger Placement....");
                m_GTDataContext = m_gtapp.DataContext;


               
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "PU Trigger Placement", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Frm_PUTrigger_Load(object sender, EventArgs e)
        {
            this.DesktopLocation = new Point(24, 100);
            m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Running PU Trigger Placement....");
            LoadPUFeature();
        }
#endregion      

        #region Get Value from database
        private string Get_Value(string sSql)
        {
            try
            {
                ADODB.Recordset rsPP = new ADODB.Recordset();
                rsPP = m_GTDataContext.OpenRecordset(sSql, ADODB.CursorTypeEnum.adOpenStatic, ADODB.LockTypeEnum.adLockReadOnly, 1);
                if (rsPP.RecordCount > 0)
                {
                    rsPP.MoveFirst();
                    return (rsPP.Fields[0].Value.ToString());
                }
                return null;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }
        #endregion

        #region Update Attr based on selected features

        private void LoadAttrFeat1()
        {
            if (cbPuTrFeature.Text == "IB_T | FIB" || cbPuTrFeature.Text == "IB_T" || cbPuTrFeature.Text == "IB_CON")
            {
                pole_attr_populate(featFID1.ToString(),featFNO1);
            }
            else if (cbPuTrFeature.Text == "PLAT_A")
            {
                txtAttr4.Visible = true;
                txtAttr4.ReadOnly = true;
                txtAttr4.Text = "A";
            }
            else if (cbPuTrFeature.Text == "PLAT_B")
            {
                txtAttr4.Visible = true;
                txtAttr4.ReadOnly = true;
                txtAttr4.Text = "B";
            }
            else if (cbPuTrFeature.Text == "TRANS_POLE 100KM" || cbPuTrFeature.Text == "TRANS_POLE 150KM"
                    || cbPuTrFeature.Text == "TRANS_POLE 200KM" || cbPuTrFeature.Text == "TRANS_POLE 50KM"
                    || cbPuTrFeature.Text == "TRANS_POLE 201KM" || cbPuTrFeature.Text == "CABLE_BEARER_NO:2"
                    || cbPuTrFeature.Text == "CABLE_BEARER_NO:1" || cbPuTrFeature.Text == "CABLE_BRACKET_NO:12"
                    || cbPuTrFeature.Text == "CABLE_BRACKET_NO:18" || cbPuTrFeature.Text == "CABLE_BRACKET_NO:24"
                    || cbPuTrFeature.Text == "CABLE_BRACKET_NO:8" || cbPuTrFeature.Text == "LOCKING_PIN")
            {
              
                txtAttr2.Visible = true;
                txtAttr2.ReadOnly = true;
                txtAttr2.Text = cbPuTrFeature.Text;

            }
            else if (cbPuTrFeature.Text == "OFCW")
            {

                txtAttr2.Text = "OFCW";
                txtAttr2.Visible = true;
                txtAttr2.ReadOnly = true;
                fiber_cable_attr_populate(featFID1.ToString(), featFNO1);
            }            
            else if (cbPuTrFeature.Text == "LD_IN")
            {
                manhole_attr_populate(featFID1.ToString(), featFNO1);
            }
        }

        private void LoadAttrFeat2()
        {
            if (cbPuTrFeature.Text == "IB_CON" || cbPuTrFeature.Text == "IB_E" || cbPuTrFeature.Text == "IB_T")
            {
                cop_cable_attr_populate(featFID2.ToString(), featFNO2);
            }
            else if (cbPuTrFeature.Text == "IB_T | FIB" )
            {
                fiber_cable_attr_populate(featFID2.ToString(), featFNO2);
            }


        }

        #region autopopulation from related features

        private void fiber_cable_attr_populate( string FID, short FNO)
        {

            txtAttr6.Text = "FIB";
            txtAttr6.Visible = true;
            txtAttr6.ReadOnly = true;

            string att3 = "-";
            string att4 = "-";
            string att5 = "-";
            string att7 = "-";
            string att8 = "-";
            string att10 = "-";
            string att11 = "-";

            if (cbPuTrFeature.Text == "IB_T | FIB")
            {
                if (FNO == 7200)///Fiber E side
                {
                    att4 = Get_Value("SELECT CABLE_SIZE FROM GC_FCBL WHERE G3E_FID = " + FID);
                }
                else if (FNO == 7400)///Fiber D side
                {
                    att4 = Get_Value("SELECT CABLE_SIZE FROM GC_FDCBL WHERE G3E_FID = " + FID);
                }
            }
            else
            {

                if (FNO == 7200)///Fiber E side
                {
                    att3 = Get_Value("SELECT CUST_NETWORK FROM GC_FCBL WHERE G3E_FID = " + FID);
                    att4 = Get_Value("SELECT CABLE_SIZE FROM GC_FCBL WHERE G3E_FID = " + FID);
                    att5 = Get_Value("SELECT SECT_NUM FROM GC_FCBL WHERE G3E_FID = " + FID);
                    att7 = Get_Value("SELECT CABLE_TYPE FROM GC_FCBL WHERE G3E_FID = " + FID);
                    att8 = Get_Value("SELECT CORE_CONSTYPE FROM GC_FCBL WHERE G3E_FID = " + FID);
                    att10 = Get_Value("SELECT CABLE_LENGTH FROM GC_FCBL WHERE G3E_FID = " + FID);
                    att11 = Get_Value("SELECT CONTRACTOR FROM GC_FCBL WHERE G3E_FID = " + FID);
                }
                else if (FNO == 7400)///Fiber D side
                {
                    att3 = Get_Value("SELECT CUST_NETWORK FROM GC_FDCBL WHERE G3E_FID = " + FID);
                    att4 = Get_Value("SELECT CABLE_SIZE FROM GC_FDCBL WHERE G3E_FID = " + FID);
                    att5 = Get_Value("SELECT SECT_NUM FROM GC_FDCBL WHERE G3E_FID = " + FID);
                    att7 = Get_Value("SELECT CABLE_TYPE FROM GC_FDCBL WHERE G3E_FID = " + FID);
                    att8 = Get_Value("SELECT CORE_CONSTYPE FROM GC_FDCBL WHERE G3E_FID = " + FID);
                    att10 = Get_Value("SELECT CABLE_LENGTH FROM GC_FDCBL WHERE G3E_FID = " + FID);
                }
               
                txtAttr3.Text = att3;
                txtAttr3.Visible = true;
                txtAttr3.ReadOnly = true;                

                txtAttr5.Text = att5;
                txtAttr5.Visible = true;
                txtAttr5.ReadOnly = true;

                txtAttr7.Text = att7;
                txtAttr7.Visible = true;
                txtAttr7.ReadOnly = true;

                txtAttr8.Text = att8;
                txtAttr8.Visible = true;
                txtAttr8.ReadOnly = true;

                txtAttr10.Text = att10;
                txtAttr10.Visible = true;
                txtAttr10.ReadOnly = true;               
            }
            txtAttr4.Text = att4;
            txtAttr4.Visible = true;
            txtAttr4.ReadOnly = true;

            txtAttr11.Text = att11;
            txtAttr11.Visible = true;
            txtAttr11.ReadOnly = true;
        }


        private void pole_attr_populate(string FID, short FNO)
        {
            if (FNO == 3000)///pole
            {
                string pole_type = Get_Value("SELECT UPPER(POLE_TYPE) FROM GC_POLE WHERE G3E_FID = " + FID);
                txtAttr3.Visible = true;
                txtAttr3.ReadOnly = true;
                if (pole_type != null)
                {
                    if (pole_type == "CONCRETE")
                    {
                        txtAttr3.Text = "C/P";
                    }
                    else if (pole_type == "BESI")
                    {
                        txtAttr3.Text = "I/P";
                    }
                    else if (pole_type == "KAYU")
                    {
                        txtAttr3.Text = "W/P";
                    }
                    else
                    {
                        txtAttr3.Text = "WALL";
                    }
                }
                else txtAttr3.Text = "-";
            }
            else txtAttr3.Text = "-";
        }

        private void manhole_attr_populate(string FID, short FNO)
        {
            if (FNO == 2700)///manhole
            {
                string mh_type = Get_Value("SELECT TRIM(FEATURE_TYPE) FEATURE_TYPE FROM GC_MANHL WHERE G3E_FID = " + FID);
                switch (mh_type)
                {
                    case "18X18":
                    case "18X30":
                    case "JB1":
                    case "JB22":
                    case "JB30":
                    case "JRC7":
                    case "JC9":
                    case "JC9(M)":
                    case "JC9C":
                    case "JC9C(M)":
                        {
                            txtAttr2.Text = "JB";
                            break;
                        }
                    default:
                        {
                            txtAttr2.Text = "MH";
                            break;
                        }
                }
                txtAttr2.Visible = true;
                txtAttr2.ReadOnly = true;
            }


        }


        private void cop_cable_attr_populate(string FID, short FNO)
        {

            string att4 = "";
            string att5 = "";

            if (FNO == 7000)//cop cable
            {
                att4 = Get_Value("SELECT TOTAL_SIZE FROM GC_CBL WHERE G3E_FID = " + FID);
                att5 = Get_Value("SELECT GAUGE FROM GC_CBL WHERE G3E_FID = " + FID);
            }

            txtAttr4.Text = att4;
            txtAttr4.Visible = true;
            txtAttr4.ReadOnly = true;

            txtAttr5.Text = att5;
            txtAttr5.Visible = true;
            txtAttr5.ReadOnly = true;

        }
        #endregion

        #endregion

        #region Load Combo Boxes
        private void LoadPUFeature()
        {
            Load_Combo(cbPuTrFeature, "select DISTINCT pu_feature from ref_pu_trigger order by pu_feature asc");
            Load_Combo(cbBillingRate, "select pl_value from REF_COM_BILLRATE order by pl_num asc");
            cbBillingRate.SelectedIndex = 0;
        }

       

        private void LoadAttr()
        {
            //string sSQL = "select DISTINCT min_material from ref_pu_trigger where pu_feature='" + cbPuTrFeature.SelectedItem.ToString() + "' ";
            //if (featFNO1 != 0)
            //    sSQL += " and fno_1=" + featFNO1;
            //if (featFNO2 != 0)
            //    sSQL += " and fno_2=" + featFNO2;
            //if (txtAttr1.Text != "" && txtAttr1.Text != "-")
            //    sSQL += " and ((instr(fields, 'ATT_1') > 0  and att_1='" + txtAttr1.Text + "') or 1=1)";
            //if (txtAttr2.Text != "" && txtAttr2.Text != "-")
            //    sSQL += "  and ((instr(fields, 'ATT_2') > 0 and att_2='" + txtAttr2.Text + "') or 1=1)";
            //if (txtAttr3.Text != "" && txtAttr3.Text != "-")
            //    sSQL += "  and ((instr(fields, 'ATT_3') > 0 and att_3='" + txtAttr3.Text + "') or 1=1)";
            //if (txtAttr4.Text != "" && txtAttr4.Text != "-")
            //    sSQL += "  and ((instr(fields, 'ATT_4') > 0 and att_4='" + txtAttr4.Text + "') or 1=1)";
            //if (txtAttr5.Text != "" && txtAttr5.Text != "-")
            //    sSQL += "  and ((instr(fields, 'ATT_5') > 0 and att_5='" + txtAttr5.Text + "') or 1=1)";
            //if (txtAttr6.Text != "" && txtAttr6.Text != "-")
            //    sSQL += "  and ((instr(fields, 'ATT_6') > 0 and att_6='" + txtAttr6.Text + "') or 1=1)";
            //if (txtAttr7.Text != "" && txtAttr7.Text != "-")
            //    sSQL += "  and ((instr(fields, 'ATT_7') > 0 and att_7='" + txtAttr7.Text + "') or 1=1)";
            //if (txtAttr8.Text != "" && txtAttr8.Text != "-")
            //    sSQL += "  and ((instr(fields, 'ATT_8') > 0 and att_8='" + txtAttr8.Text + "') or 1=1)";
            //if (txtAttr9.Text != "" && txtAttr9.Text != "-")
            //    sSQL += "  and ((instr(fields, 'ATT_9') > 0 and att_9='" + txtAttr9.Text + "') or 1=1)";
            //if (txtAttr10.Text != "" && txtAttr10.Text != "-")
            //    sSQL += "  and ((instr(fields, 'ATT_10') > 0 and att_10='" + txtAttr10.Text + "') or 1=1)";
            //if (txtAttr11.Text != "" && txtAttr11.Text != "-")
            //    sSQL += "  and ((instr(fields, 'ATT_11') > 0 and att_11='" + txtAttr11.Text + "') or 1=1)";
            //if (txtAttr12.Text != "" && txtAttr12.Text != "-")
            //    sSQL += "  and ((instr(fields, 'ATT_12') > 0 and att_12='" + txtAttr12.Text + "') or 1=1)";
            //sSQL += "  order by min_material asc";

            string sSQL = "select AG_REF_PU_TRIGGER('" + cbPuTrFeature.SelectedItem.ToString() + "'," +
                featFNO1 + "," + featFNO2 + ",'"+txtAttr1.Text+"','" +txtAttr2.Text+"','"+txtAttr3.Text+"','"
            +txtAttr4.Text+"','"+txtAttr5.Text+"','"+txtAttr6.Text+"','"+txtAttr7.Text+"','"+txtAttr8.Text+"','"
            +txtAttr9.Text+"','"+txtAttr10.Text+"','"+txtAttr11.Text+"','"+txtAttr12.Text+"') from dual";

            string Min_mat_sql = Get_Value(sSQL);


            Load_Combo(cbPlantUnit, Min_mat_sql);


            if(cbPlantUnit.Items.Count ==0 )
                MessageBox.Show("Plant Unit is not available for choosen parameter!", "PU Trigger Placement", MessageBoxButtons.OK, MessageBoxIcon.Information);
            else if (cbPlantUnit.Items.Count == 1)
            {
                cbPlantUnit.SelectedIndex = 0;
                btnPlace.Enabled = true;
                gbAttrs.Enabled = true;
            }
            else
            {
                btnPlace.Enabled = true;
                gbAttrs.Enabled = true;
                gbPlantUnit.Enabled = true;
                LoadAttrComboAll(sSQL);
            }

          
        }

        private void UpdatePUlist()
        {
            string sSQL = "select AG_REF_PU_TRIGGER('" + cbPuTrFeature.SelectedItem.ToString() + "'," +
                featFNO1 + "," + featFNO2 + ",'" + txtAttr1.Text + "','" + txtAttr2.Text + "','" + txtAttr3.Text + "','"
            + txtAttr4.Text + "','" + txtAttr5.Text + "','" + txtAttr6.Text + "','" + txtAttr7.Text + "','" + txtAttr8.Text + "','"
            + txtAttr9.Text + "','" + txtAttr10.Text + "','" + txtAttr11.Text + "','" + txtAttr12.Text + "') from dual";

            string Min_mat_sql = Get_Value(sSQL);


            Load_Combo(cbPlantUnit, Min_mat_sql);

            if (cbPlantUnit.Items.Count == 0)
                MessageBox.Show("Plant Unit is not available for choosen parameter!", "PU Trigger Placement", MessageBoxButtons.OK, MessageBoxIcon.Information);
            else if (cbPlantUnit.Items.Count == 1)
            {
                cbPlantUnit.SelectedIndex = 0;
                btnPlace.Enabled = true;
                gbAttrs.Enabled = true;
            }
            else
            {
                btnPlace.Enabled = true;
                gbAttrs.Enabled = true;
                gbPlantUnit.Enabled = true;
            }
        }

        private void LoadAttrComboAll(string sSQLmin_mat)
        {
            
            if (txtAttr1.Text == "" || txtAttr1.Text == "-")
            {
                Load_Combo(cbAttr1, LoadAttrCombo("att_1"));
                if (cbAttr1.Items.Count <= 0)
                {
                    cbAttr1.Visible = false;
                    txtAttr1.Visible = true;
                }
                else if (cbAttr1.Items.Count == 1)
                {
                    cbAttr1.Visible = false;
                    txtAttr1.Visible = true;
                    txtAttr1.Text = cbAttr1.Items[0].ToString();
                    txtAttr1.ReadOnly = true;
                }
                else
                {
                    cbAttr1.Visible = true;
                    txtAttr1.Visible = false;
                }
            }
            if (txtAttr2.Text == "" || txtAttr2.Text == "-")
            {
                Load_Combo(cbAttr2, LoadAttrCombo("att_2"));
                if (cbAttr2.Items.Count <= 0)
                {
                    cbAttr2.Visible = false;
                    txtAttr2.Visible = true;
                }
                else if (cbAttr2.Items.Count == 1)
                {
                    cbAttr2.Visible = false;
                    txtAttr2.Visible = true;
                    txtAttr2.Text = cbAttr2.Items[0].ToString();
                    txtAttr2.ReadOnly = true;
                }
                else
                {
                    cbAttr2.Visible = true;
                    txtAttr2.Visible = false;
                }
            }
            if (txtAttr3.Text == "" || txtAttr3.Text == "-")
            {
                Load_Combo(cbAttr3, LoadAttrCombo("att_3"));
                if (cbAttr3.Items.Count <= 0)
                {
                    cbAttr3.Visible = false;
                    txtAttr3.Visible = true;
                }
                else if (cbAttr3.Items.Count == 1)
                {
                    cbAttr3.Visible = false;
                    txtAttr3.Visible = true;
                    txtAttr3.Text = cbAttr3.Items[0].ToString();
                    txtAttr3.ReadOnly = true;
                }
                else
                {
                    cbAttr3.Visible = true;
                    txtAttr3.Visible = false;
                }
            }
            if (txtAttr4.Text == "" || txtAttr4.Text == "-")
            {
                Load_Combo(cbAttr4, LoadAttrCombo("att_4"));
                if (cbAttr4.Items.Count <= 0)
                {
                    cbAttr4.Visible = false;
                    txtAttr4.Visible = true;
                }
                else if (cbAttr4.Items.Count == 1)
                {
                    cbAttr4.Visible = false;
                    txtAttr4.Visible = true;
                    txtAttr4.Text = cbAttr4.Items[0].ToString();
                    txtAttr4.ReadOnly = true;
                }
                else
                {
                    cbAttr4.Visible = true;
                    txtAttr4.Visible = false;
                }
            }
            if (txtAttr5.Text == "" || txtAttr5.Text == "-")
            {
                Load_Combo(cbAttr5, LoadAttrCombo("att_5"));
                if (cbAttr5.Items.Count <= 0)
                {
                    cbAttr5.Visible = false;
                    txtAttr5.Visible = true;
                }
                else if (cbAttr5.Items.Count == 1)
                {
                    cbAttr5.Visible = false;
                    txtAttr5.Visible = true;
                    txtAttr5.Text = cbAttr5.Items[0].ToString();
                    txtAttr5.ReadOnly = true;
                }
                else
                {
                    cbAttr5.Visible = true;
                    txtAttr5.Visible = false;
                }
            }
            if (txtAttr6.Text == "" || txtAttr6.Text == "-")
            {
                Load_Combo(cbAttr6, LoadAttrCombo("att_6"));
                if (cbAttr6.Items.Count <= 0)
                {
                    cbAttr6.Visible = false;
                    txtAttr6.Visible = true;
                }
                else if (cbAttr6.Items.Count == 1)
                {
                    cbAttr6.Visible = false;
                    txtAttr6.Visible = true;
                    txtAttr6.Text = cbAttr6.Items[0].ToString();
                    txtAttr6.ReadOnly = true;
                }
                else
                {
                    cbAttr6.Visible = true;
                    txtAttr6.Visible = false;
                }
            }
            if (txtAttr7.Text == "" || txtAttr7.Text == "-")
            {
                Load_Combo(cbAttr7, LoadAttrCombo("att_7"));
                if (cbAttr7.Items.Count <= 0)
                {
                    cbAttr7.Visible = false;
                    txtAttr7.Visible = true;
                }
                else if (cbAttr7.Items.Count == 1)
                {
                    cbAttr7.Visible = false;
                    txtAttr7.Visible = true;
                    txtAttr7.Text = cbAttr7.Items[0].ToString();
                    txtAttr7.ReadOnly = true;
                }
                else
                {
                    cbAttr7.Visible = true;
                    txtAttr7.Visible = false;
                }
            }
            if (txtAttr8.Text == "" || txtAttr8.Text == "-")
            {

                Load_Combo(cbAttr8, LoadAttrCombo("att_8"));
                if (cbAttr8.Items.Count <= 0)
                {
                    cbAttr8.Visible = false;
                    txtAttr8.Visible = true;
                }
                else if (cbAttr8.Items.Count == 1)
                {
                    cbAttr8.Visible = false;
                    txtAttr8.Visible = true;
                    txtAttr8.Text = cbAttr8.Items[0].ToString();
                    txtAttr8.ReadOnly = true;
                }
                else
                {
                    cbAttr8.Visible = true;
                    txtAttr8.Visible = false;
                }
            }

            if (txtAttr9.Text == "" || txtAttr9.Text == "-")
            {

                Load_Combo(cbAttr9, LoadAttrCombo("att_9"));
                if (cbAttr9.Items.Count <= 0)
                {
                    cbAttr9.Visible = false;
                    txtAttr9.Visible = true;
                }
                else if (cbAttr9.Items.Count == 1)
                {
                    cbAttr9.Visible = false;
                    txtAttr9.Visible = true;
                    txtAttr9.Text = cbAttr9.Items[0].ToString();
                    txtAttr9.ReadOnly = true;
                }
                else
                {
                    cbAttr9.Visible = true;
                    txtAttr9.Visible = false;
                }
            }
            if (txtAttr10.Text == "" || txtAttr10.Text == "-")
            {
                Load_Combo(cbAttr10, LoadAttrCombo("att_10"));
                if (cbAttr10.Items.Count <= 0)
                {
                    cbAttr10.Visible = false;
                    txtAttr10.Visible = true;
                }
                else if (cbAttr10.Items.Count == 1)
                {
                    cbAttr10.Visible = false;
                    txtAttr10.Visible = true;
                    txtAttr10.Text = cbAttr10.Items[0].ToString();
                    txtAttr10.ReadOnly = true;
                }
                else
                {
                    cbAttr10.Visible = true;
                    txtAttr10.Visible = false;
                }
            }
            if (txtAttr11.Text == "" || txtAttr11.Text == "-")
            {

                Load_Combo(cbAttr11, LoadAttrCombo("att_11"));
                if (cbAttr11.Items.Count <= 0)
                {
                    cbAttr11.Visible = false;
                    txtAttr11.Visible = true;
                }
                else if (cbAttr11.Items.Count == 1)
                {
                    cbAttr11.Visible = false;
                    txtAttr11.Visible = true;
                    txtAttr11.Text = cbAttr11.Items[0].ToString();
                    txtAttr11.ReadOnly = true;
                }
                else
                {
                    cbAttr11.Visible = true;
                    txtAttr11.Visible = false;
                }
            }
            if (txtAttr12.Text == "" || txtAttr12.Text == "-")
            {

                Load_Combo(cbAttr12, LoadAttrCombo("att_12"));
                if (cbAttr12.Items.Count <= 0)
                {
                    cbAttr12.Visible = false;
                    txtAttr12.Visible = true;
                }
                else if (cbAttr12.Items.Count == 1)
                {
                    cbAttr12.Visible = false;
                    txtAttr12.Visible = true;
                    txtAttr12.Text = cbAttr12.Items[0].ToString();
                    txtAttr12.ReadOnly = true;
                }
                else
                {
                    cbAttr12.Visible = true;
                    txtAttr12.Visible = false;
                }
            }

        }

        private string LoadAttrCombo(string attr)
        {
            string sSQL = "select DISTINCT " + attr + " from ref_pu_trigger where pu_feature='" + cbPuTrFeature.SelectedItem.ToString() + "' ";
            if (featFNO1 != 0)
                sSQL += " and fno_1=" + featFNO1;
            if (featFNO2 != 0)
                sSQL += " and fno_2=" + featFNO2;

            if (txtAttr1.Text != "" && txtAttr1.Text != "-" && attr != "att_1")
                sSQL += " and ((instr(fields, 'ATT_1') > 0  and att_1='" + txtAttr1.Text + "') or 1=1)";
            if (txtAttr2.Text != "" && txtAttr2.Text != "-" && attr != "att_2")
                sSQL += "  and ((instr(fields, 'ATT_2') > 0 and att_2='" + txtAttr2.Text + "') or 1=1)";
            if (txtAttr3.Text != "" && txtAttr3.Text != "-" && attr != "att_3")
                sSQL += "  and ((instr(fields, 'ATT_3') > 0 and att_3='" + txtAttr3.Text + "') or 1=1)";
            if (txtAttr4.Text != "" && txtAttr4.Text != "-" && attr != "att_4")
                sSQL += "  and ((instr(fields, 'ATT_4') > 0 and att_4='" + txtAttr4.Text + "') or 1=1)";
            if (txtAttr5.Text != "" && txtAttr5.Text != "-" && attr != "att_5")
                sSQL += "  and ((instr(fields, 'ATT_5') > 0 and att_5='" + txtAttr5.Text + "') or 1=1)";
            if (txtAttr6.Text != "" && txtAttr6.Text != "-" && attr != "att_6")
                sSQL += "  and ((instr(fields, 'ATT_6') > 0 and att_6='" + txtAttr6.Text + "') or 1=1)";
            if (txtAttr7.Text != "" && txtAttr7.Text != "-" && attr != "att_7")
                sSQL += "  and ((instr(fields, 'ATT_7') > 0 and att_7='" + txtAttr7.Text + "') or 1=1)";
            if (txtAttr8.Text != "" && txtAttr8.Text != "-" && attr != "att_8")
                sSQL += "  and ((instr(fields, 'ATT_8') > 0 and att_8='" + txtAttr8.Text + "') or 1=1)";
            if (txtAttr9.Text != "" && txtAttr9.Text != "-" && attr != "att_9")
                sSQL += "  and ((instr(fields, 'ATT_9') > 0 and att_9='" + txtAttr9.Text + "') or 1=1)";
            if (txtAttr10.Text != "" && txtAttr10.Text != "-" && attr != "att_10")
                sSQL += "  and ((instr(fields, 'ATT_10') > 0 and att_10='" + txtAttr10.Text + "') or 1=1)";
            if (txtAttr11.Text != "" && txtAttr11.Text != "-" && attr != "att_11")
                sSQL += "  and ((instr(fields, 'ATT_11') > 0 and att_11='" + txtAttr11.Text + "') or 1=1)";
            if (txtAttr12.Text != "" && txtAttr12.Text != "-" && attr != "att_12")
                sSQL += "  and ((instr(fields, 'ATT_12') > 0 and att_12='" + txtAttr12.Text + "') or 1=1)";

            sSQL += "  order by "+attr+" asc";

            return sSQL;
        }

        private void UpdateAttrBasedOnPlantUnit()
        {
             string sSQL = "select ATT_1,ATT_2,ATT_3,ATT_4,ATT_5,ATT_6,ATT_7,ATT_8,ATT_9,ATT_10,ATT_11,ATT_12 from ref_pu_trigger where"+
                 " pu_feature='" + cbPuTrFeature.SelectedItem.ToString() + "' " +
                 " and  min_material='"+cbPlantUnit.SelectedItem.ToString()+"' ";
            if (featFNO1 != 0)
                sSQL += " and fno_1=" + featFNO1;
            if (featFNO2 != 0)
                sSQL += " and fno_2=" + featFNO2;

            int recordsAffected = 0;
            Recordset rsComp = m_GTDataContext.Execute(sSQL, out recordsAffected, (int)CommandTypeEnum.adCmdText, null);
            if (rsComp != null && rsComp.RecordCount > 0)
            {
                rsComp.MoveFirst();
                do
                {
                    if (rsComp.Fields[0].Value.ToString() != "-")
                    {
                        txtAttr1.Text = rsComp.Fields[0].Value.ToString();
                        if (cbAttr1.Visible && cbAttr1.Items.Count > 0)
                        {
                            for (int i = 0; i < cbAttr1.Items.Count; i++)
                            {
                                if (cbAttr1.Items[i].ToString() == rsComp.Fields[0].Value.ToString())
                                {
                                    cbAttr1.SelectedIndex = i;
                                    break;
                                }
                            }
                        }
                    }

                    if (rsComp.Fields[1].Value.ToString() != "-")
                    {
                        txtAttr2.Text = rsComp.Fields[1].Value.ToString();
                        if (cbAttr2.Visible && cbAttr2.Items.Count > 0)
                        {
                            for (int i = 0; i < cbAttr2.Items.Count; i++)
                            {
                                if (cbAttr2.Items[i].ToString() == rsComp.Fields[1].Value.ToString())
                                {
                                    cbAttr2.SelectedIndex = i;
                                    break;
                                }
                            }
                        }
                    }

                    if (rsComp.Fields[2].Value.ToString() != "-")
                    {
                        txtAttr3.Text = rsComp.Fields[2].Value.ToString();
                        if (cbAttr3.Visible && cbAttr3.Items.Count > 0)
                        {
                            for (int i = 0; i < cbAttr3.Items.Count; i++)
                            {
                                if (cbAttr3.Items[i].ToString() == rsComp.Fields[2].Value.ToString())
                                {
                                    cbAttr3.SelectedIndex = i;
                                    break;
                                }
                            }
                        }
                    }

                    if (rsComp.Fields[3].Value.ToString() != "-")
                    {
                        txtAttr4.Text = rsComp.Fields[3].Value.ToString();
                        if (cbAttr4.Visible && cbAttr4.Items.Count > 0)
                        {
                            for (int i = 0; i < cbAttr4.Items.Count; i++)
                            {
                                if (cbAttr4.Items[i].ToString() == rsComp.Fields[3].Value.ToString())
                                {
                                    cbAttr4.SelectedIndex = i;
                                    break;
                                }
                            }
                        }
                    }

                    if (rsComp.Fields[4].Value.ToString() != "-")
                    {
                        txtAttr5.Text = rsComp.Fields[4].Value.ToString();
                        if (cbAttr5.Visible && cbAttr5.Items.Count > 0)
                        {
                            for (int i = 0; i < cbAttr5.Items.Count; i++)
                            {
                                if (cbAttr5.Items[i].ToString() == rsComp.Fields[4].Value.ToString())
                                {
                                    cbAttr5.SelectedIndex = i;
                                    break;
                                }
                            }
                        }
                    }

                    if (rsComp.Fields[5].Value.ToString() != "-")
                    {
                        txtAttr6.Text = rsComp.Fields[5].Value.ToString();
                        if (cbAttr6.Visible && cbAttr6.Items.Count > 0)
                        {
                            for (int i = 0; i < cbAttr6.Items.Count; i++)
                            {
                                if (cbAttr6.Items[i].ToString() == rsComp.Fields[5].Value.ToString())
                                {
                                    cbAttr6.SelectedIndex = i;
                                    break;
                                }
                            }
                        }
                    }

                    if (rsComp.Fields[6].Value.ToString() != "-")
                    {
                        txtAttr7.Text = rsComp.Fields[6].Value.ToString();
                        if (cbAttr7.Visible && cbAttr7.Items.Count > 0)
                        {
                            for (int i = 0; i < cbAttr7.Items.Count; i++)
                            {
                                if (cbAttr7.Items[i].ToString() == rsComp.Fields[6].Value.ToString())
                                {
                                    cbAttr7.SelectedIndex = i;
                                    break;
                                }
                            }
                        }
                    }

                    if (rsComp.Fields[7].Value.ToString() != "-")
                    {
                        txtAttr8.Text = rsComp.Fields[7].Value.ToString();
                        if (cbAttr8.Visible && cbAttr8.Items.Count > 0)
                        {
                            for (int i = 0; i < cbAttr8.Items.Count; i++)
                            {
                                if (cbAttr8.Items[i].ToString() == rsComp.Fields[7].Value.ToString())
                                {
                                    cbAttr8.SelectedIndex = i;
                                    break;
                                }
                            }
                        }
                    }

                    if (rsComp.Fields[8].Value.ToString() != "-")
                    {
                        txtAttr9.Text = rsComp.Fields[8].Value.ToString();
                        if (cbAttr9.Visible && cbAttr9.Items.Count > 0)
                        {
                            for (int i = 0; i < cbAttr9.Items.Count; i++)
                            {
                                if (cbAttr9.Items[i].ToString() == rsComp.Fields[8].Value.ToString())
                                {
                                    cbAttr9.SelectedIndex = i;
                                    break;
                                }
                            }
                        }
                    }

                    if (rsComp.Fields[9].Value.ToString() != "-")
                    {
                        txtAttr10.Text = rsComp.Fields[9].Value.ToString();
                        if (cbAttr10.Visible && cbAttr10.Items.Count > 0)
                        {
                            for (int i = 0; i < cbAttr10.Items.Count; i++)
                            {
                                if (cbAttr10.Items[i].ToString() == rsComp.Fields[9].Value.ToString())
                                {
                                    cbAttr10.SelectedIndex = i;
                                    break;
                                }
                            }
                        }
                    }

                    if (rsComp.Fields[10].Value.ToString() != "-")
                    {
                        txtAttr11.Text = rsComp.Fields[10].Value.ToString();
                        if (cbAttr11.Visible && cbAttr11.Items.Count > 0)
                        {
                            for (int i = 0; i < cbAttr11.Items.Count; i++)
                            {
                                if (cbAttr11.Items[i].ToString() == rsComp.Fields[10].Value.ToString())
                                {
                                    cbAttr11.SelectedIndex = i;
                                    break;
                                }
                            }
                        }
                    }

                    if (rsComp.Fields[11].Value.ToString() != "-")
                    {
                        txtAttr12.Text = rsComp.Fields[11].Value.ToString();
                        if (cbAttr12.Visible && cbAttr12.Items.Count > 0)
                        {
                            for (int i = 0; i < cbAttr12.Items.Count; i++)
                            {
                                if (cbAttr12.Items[i].ToString() == rsComp.Fields[11].Value.ToString())
                                {
                                    cbAttr12.SelectedIndex = i;
                                    break;
                                }
                            }
                        }
                    }




                    rsComp.MoveNext();
                }
                while (!rsComp.EOF);
            }
            rsComp = null;

            if(cbPuTrFeature.Text == "PILE")
            {
                if (cbPlantUnit.SelectedItem.ToString().Contains("EXTRA"))
                { txtQuantity3.ReadOnly = false; }
                else
                { txtQuantity3.ReadOnly = true; 
                  txtQuantity3.Text = ""; }
            }
        }

        private void Load_Combo(ComboBox cmb, string sSql)
        {
            int recordsAffected = 0;
            Recordset rsComp = m_GTDataContext.Execute(sSql, out recordsAffected, (int)CommandTypeEnum.adCmdText, null);
            cmb.Items.Clear();

            if (rsComp != null && rsComp.RecordCount > 0)
            {
                rsComp.MoveFirst();
                do
                {
                    cmb.Items.Add(rsComp.Fields[0].Value.ToString());
                    rsComp.MoveNext();
                }
                while (!rsComp.EOF);
            }
            rsComp = null;
        }
        #endregion
   

        #region Check selected feature state
        private bool CheckState(short iFNO, int iFID,string name, string feaSTATE)
        {
            try
            {                
                string selectedFeatState =Get_Value( "SELECT FEATURE_STATE FROM GC_NETELEM WHERE G3E_FID = " + iFID + " AND G3E_FNO = " + iFNO);
                if (feaSTATE.Contains(selectedFeatState) || feaSTATE=="")
                    return true;
                else MessageBox.Show("Please select only " + feaSTATE+" "+name, "PU Trigger Placement", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "PU Trigger Placement", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        #endregion

        #region pick related features btn

        private void btn_Pick_Click(object sender, EventArgs e)
        {
            this.Hide();
            m_gtapp.SelectedObjects.Clear();
            m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Point to select " + vFeature1 + "!Right click to cancel selection");
            PlaceValue = 10;
        }

        private void btn_Pick2_Click(object sender, EventArgs e)
        {
            this.Hide();
            m_gtapp.SelectedObjects.Clear();
            m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Point to select " + vFeature2 + "!Right click to cancel selection");
            PlaceValue = 20;
        }

        public void PickParent(int parent_num)
        {
            short iFNO = 0;
            int iFID = 0;
            string name = "";
           
            if (parent_num == 1)
            {
                foreach (IGTDDCKeyObject oDDCKeyObject in m_gtapp.SelectedObjects.GetObjects())
                {
                                       
                    foreach (vFeat temp in FeatList1)
                    {
                        if (oDDCKeyObject.FNO == temp.FNO)
                        {
                            iFNO = oDDCKeyObject.FNO;
                            iFID = oDDCKeyObject.FID;
                            name = temp.name;
                            if (!CheckState(iFNO, iFID, name, temp.feat_state))
                                iFNO = 0;
                            break;
                        }
                    }

                    if (iFNO !=0)
                    {
                        featFID1 = iFID;
                        featFNO1 = iFNO;
                        txtFeatName1.Text = name;
                        txtFeatFID1.Text = featFID1.ToString();
                        vFeature2 = "";
                        gbFeature2.Enabled = false;
                        gbFeature2.Text = "Feature 2:";
                        txtFeatFID2.Text = "";
                        txtFeatName2.Text = "";
                        cleanupattr_values();
                        gbAttrs.Enabled = false;
                        LoadAttrFeat1();
                        getrelatedfeatures2();
                        if (!gbFeature2.Enabled)
                        {
                            LoadAttr();
                        }
                        this.Show();
                        PlaceValue = 0;
                        return;
                    }
                    else  
                    {
                    MessageBox.Show("Please select a " + vFeature1, "PU Trigger Placement", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    m_gtapp.SelectedObjects.Clear();
                    return;
                    }
                }  
            }
            else if (parent_num == 2)
            {
                foreach (IGTDDCKeyObject oDDCKeyObject in m_gtapp.SelectedObjects.GetObjects())
                {

                    foreach (vFeat temp in FeatList2)
                    {
                        if (oDDCKeyObject.FNO == temp.FNO)
                        {
                            iFNO = oDDCKeyObject.FNO;
                            iFID = oDDCKeyObject.FID;
                            name = temp.name;
                            if (!CheckState(iFNO, iFID, name, temp.feat_state))
                                iFNO = 0;
                            break;
                        }
                    }

                    if (iFNO != 0)
                    {
                        featFID2 = iFID;
                        featFNO2 = iFNO;
                        txtFeatName2.Text = name;
                        txtFeatFID2.Text = featFID2.ToString();
                        
                        LoadAttrFeat2();
                        LoadAttr();
                        this.Show();
                        PlaceValue = 0;
                        return;
                    }
                    else
                    {
                        MessageBox.Show("Please select a " + vFeature2, "PU Trigger Placement", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        m_gtapp.SelectedObjects.Clear();
                        return;
                    }
                }
            }
            
        }
        #endregion

        #region Change PU Feature type
        private void cbPuTrFeature_SelectedValueChanged(object sender, EventArgs e)
        {
            cleanupform();
            getrelatedfeatures1();
            SetAttrConfigPUTr();
        }
        #endregion

        #region clean up form
        private void cleanupform()
        {
            gbFeature1.Enabled = false;
            gbFeature1.Text = "Feature 1:";
            vFeature1 = "";
            vFeature2 = "";
            txtFeatFID1.Text = "";
            txtFeatName1.Text = "";
            gbFeature2.Enabled = false;
            gbFeature2.Text = "Feature 2:";
            txtFeatFID2.Text = "";
            txtFeatName2.Text = "";
            cleanupattr_captions();
            cleanupattr_values();
            gbAttrs.Enabled = false;
        }

        private void cleanupattr_captions()
        {
            lbAttr1.Text = "Attr1";
            lbAttr2.Text = "Attr2";
            lbAttr3.Text = "Attr3";
            lbAttr4.Text = "Attr4";
            lbAttr5.Text = "Attr5";
            lbAttr6.Text = "Attr6";
            lbAttr7.Text = "Attr7";
            lbAttr8.Text = "Attr8";
            lbAttr9.Text = "Attr9";
            lbAttr10.Text = "Attr10";
            lbAttr11.Text = "Attr11";
            lbAttr12.Text = "Attr12";
            lbQuantity1.Text = "Quantity1";
            lbQuantity2.Text = "Quantity2";
            lbQuantity3.Text = "Quantity3";
            lbQuantity4.Text = "Quantity4";
        }

        private void cleanupattr_values()
        {

            gbPlantUnit.Enabled = false;
            cbPlantUnit.SelectedIndex = -1;

            txtAttr1.Visible = true;
            txtAttr2.Visible = true;
            txtAttr3.Visible = true;
            txtAttr4.Visible = true;
            txtAttr5.Visible = true;
            txtAttr6.Visible = true;
            txtAttr7.Visible = true;
            txtAttr8.Visible = true;
            txtAttr9.Visible = true;
            txtAttr10.Visible = true;
            txtAttr11.Visible = true;
            txtAttr12.Visible = true;

            txtAttr1.ReadOnly = true;
            txtAttr2.ReadOnly = true;
            txtAttr3.ReadOnly = true;
            txtAttr4.ReadOnly = true;
            txtAttr5.ReadOnly = true;
            txtAttr6.ReadOnly = true;
            txtAttr7.ReadOnly = true;
            txtAttr8.ReadOnly = true;
            txtAttr9.ReadOnly = true;
            txtAttr10.ReadOnly = true;
            txtAttr11.ReadOnly = true;
            txtAttr12.ReadOnly = true;

            cbAttr1.Visible = false;
            cbAttr2.Visible = false;
            cbAttr3.Visible = false;
            cbAttr4.Visible = false;
            cbAttr5.Visible = false;
            cbAttr6.Visible = false;
            cbAttr7.Visible = false;
            cbAttr8.Visible = false;
            cbAttr9.Visible = false;
            cbAttr10.Visible = false;
            cbAttr11.Visible = false;
            cbAttr12.Visible = false;
            txtQuantity1.Visible = true;
            txtQuantity2.Visible = true;
            txtQuantity3.Visible = true;
            txtQuantity4.Visible = true;
            txtQuantity1.ReadOnly = true;
            txtQuantity2.ReadOnly = true;
            txtQuantity3.ReadOnly = true;
            txtQuantity4.ReadOnly = true;
            if (lbQuantity1.Text != "-")
                txtQuantity1.ReadOnly = false;
            if (lbQuantity2.Text != "-")
                txtQuantity2.ReadOnly = false;
            if (lbQuantity3.Text != "-")
                txtQuantity3.ReadOnly = false;
            if (lbQuantity4.Text != "-")
                txtQuantity4.ReadOnly = false;
            txtAttr1.Text = "-";
            txtAttr2.Text = "-";
            txtAttr3.Text = "-";
            txtAttr4.Text = "-";
            txtAttr5.Text = "-";
            txtAttr6.Text = "-";
            txtAttr7.Text = "-";
            txtAttr8.Text = "-";
            txtAttr9.Text = "-";
            txtAttr10.Text = "-";
            txtAttr11.Text = "-";
            txtAttr11.Text = "-";
            cbAttr1.SelectedIndex = -1;
            cbAttr2.SelectedIndex = -1;
            cbAttr3.SelectedIndex = -1;
            cbAttr4.SelectedIndex = -1;
            cbAttr5.SelectedIndex = -1;
            cbAttr6.SelectedIndex = -1;
            cbAttr7.SelectedIndex = -1;
            cbAttr8.SelectedIndex = -1;
            cbAttr9.SelectedIndex = -1;
            cbAttr10.SelectedIndex = -1;
            cbAttr11.SelectedIndex = -1;
            cbAttr11.SelectedIndex = -1;
            txtQuantity1.Text = "0";
            txtQuantity1.Text = "1";
            txtQuantity2.Text = "";
            txtQuantity3.Text = "";
            txtQuantity4.Text = "";
           
        }
        #endregion

        #region Get Possible Related features
        private void getrelatedfeatures1()
        {
            if (FeatList1 == null)
                FeatList1 = new List<vFeat>();
            FeatList1.Clear();
            vFeature1 = "";

            featFID1 = 0;
            featFNO1 = 0;
            txtFeatName1.Text = "";
            txtFeatFID1.Text = "";
            gbFeature1.Text = "Feature 1:";

            try
            {
                string sSql = "select DISTINCT feature_1, fno_1, FEATURE_STATE1 from ref_pu_trigger where pu_feature='" + cbPuTrFeature.Text + "' order by feature_1 asc ";
                int recordsAffected = 0;
                Recordset rsComp = m_GTDataContext.Execute(sSql, out recordsAffected, (int)CommandTypeEnum.adCmdText, null);


                if (rsComp != null && rsComp.RecordCount > 0)
                {
                    rsComp.MoveFirst();
                    do
                    {
                        vFeat temp = new vFeat();
                        temp.name = rsComp.Fields[0].Value.ToString();
                        temp.FNO = short.Parse(rsComp.Fields[1].Value.ToString());
                        temp.feat_state = rsComp.Fields[2].Value.ToString();                        
                        if (temp.FNO != 0)
                        {
                            FeatList1.Add(temp);
                            vFeature1 += rsComp.Fields[0].Value.ToString() + "/ ";
                        }
                        rsComp.MoveNext();
                    }
                    while (!rsComp.EOF);
                }
                rsComp = null;

                if (vFeature1 != "")
                {
                    gbFeature1.Enabled = true;
                    gbFeature1.Text = vFeature1;
                }
            }
            catch (Exception e)
            { throw e; }
        }

        private void getrelatedfeatures2()
        {
            if (FeatList2 == null)
                FeatList2 = new List<vFeat>();
            FeatList2.Clear();
            vFeature2 = "";

            featFID2 = 0;
            featFNO2 = 0;
            txtFeatName2.Text = "";
            txtFeatFID2.Text = "";
            gbFeature2.Text = "Feature 2:";

            try
            {
                string sSql = "select DISTINCT feature_2, fno_2, FEATURE_STATE2 from ref_pu_trigger where pu_feature='" + cbPuTrFeature.Text + "' and fno_1=" + featFNO1 + " and fno_2 is not null order by feature_2 asc ";
                int recordsAffected = 0;
                Recordset rsComp = m_GTDataContext.Execute(sSql, out recordsAffected, (int)CommandTypeEnum.adCmdText, null);


                if (rsComp != null && rsComp.RecordCount > 0)
                {
                    rsComp.MoveFirst();
                    do
                    {
                        vFeat temp = new vFeat();
                        temp.name = rsComp.Fields[0].Value.ToString();
                        temp.FNO = short.Parse(rsComp.Fields[1].Value.ToString());
                        temp.feat_state = rsComp.Fields[2].Value.ToString();
                        if (temp.FNO != 0)
                        {
                            FeatList2.Add(temp);
                            vFeature2 += rsComp.Fields[0].Value.ToString() + "/ ";
                        }

                        rsComp.MoveNext();
                    }
                    while (!rsComp.EOF);
                }
                rsComp = null;
                if (vFeature2 != "")
                {
                    gbFeature2.Enabled = true;
                    gbFeature2.Text = vFeature2;
                }
            }
            catch (Exception e) { throw e; }
        }
        #endregion

        #region Set Attributes for PU TR
        private void SetAttrConfigPUTr()
        {
            try
            {
                string sSql = "select ATT_1,ATT_2,ATT_3,ATT_4,ATT_5,ATT_6,ATT_7,ATT_8,ATT_9,ATT_10,ATT_11,ATT_12,QUANTITY1,QUANTITY2,QUANTITY3,QUANTITY4 " +
                    " from REF_PU_TRIGGER_ATTR where PU_FEATURE='" + cbPuTrFeature.Text + "'";
                int recordsAffected = 0;
                Recordset rsComp = m_GTDataContext.Execute(sSql, out recordsAffected, (int)CommandTypeEnum.adCmdText, null);


                if (rsComp != null && rsComp.RecordCount > 0)
                {
                    rsComp.MoveFirst();
                    do
                    {
                        lbAttr1.Text = rsComp.Fields[0].Value.ToString();
                        lbAttr2.Text = rsComp.Fields[1].Value.ToString();
                        lbAttr3.Text = rsComp.Fields[2].Value.ToString();
                        lbAttr4.Text = rsComp.Fields[3].Value.ToString();
                        lbAttr5.Text = rsComp.Fields[4].Value.ToString();
                        lbAttr6.Text = rsComp.Fields[5].Value.ToString();
                        lbAttr7.Text = rsComp.Fields[6].Value.ToString();
                        lbAttr8.Text = rsComp.Fields[7].Value.ToString();
                        lbAttr9.Text = rsComp.Fields[8].Value.ToString();
                        lbAttr10.Text = rsComp.Fields[9].Value.ToString();
                        lbAttr11.Text = rsComp.Fields[10].Value.ToString();
                        lbAttr12.Text = rsComp.Fields[11].Value.ToString();
                        lbQuantity1.Text = rsComp.Fields[12].Value.ToString();
                        lbQuantity2.Text = rsComp.Fields[13].Value.ToString();
                        lbQuantity3.Text = rsComp.Fields[14].Value.ToString();
                        lbQuantity4.Text = rsComp.Fields[15].Value.ToString();

                        if (lbQuantity1.Text != "-")
                            txtQuantity1.ReadOnly = false;
                        if (lbQuantity2.Text != "-")
                            txtQuantity2.ReadOnly = false;
                        if (lbQuantity3.Text != "-")
                            txtQuantity3.ReadOnly = false;
                        if (lbQuantity4.Text != "-")
                            txtQuantity4.ReadOnly = false;

                        rsComp.MoveNext();
                    }
                    while (!rsComp.EOF);
                }
                rsComp = null;
            }
            catch (Exception e)
            { MessageBox.Show(e.Message, "Error.", MessageBoxButtons.OK, MessageBoxIcon.Error);  }

        }
        #endregion

        #region Plant Unit DropDwon List valie change
        private void cbPlantUnit_SelectedValueChanged(object sender, EventArgs e)
        {
            if(cbPlantUnit.SelectedIndex != -1)
                UpdateAttrBasedOnPlantUnit();
        }

        #region Plant Unit DropDown update list
        private void cbPlantUnit_DropDown(object sender, EventArgs e)
        {
            UpdatePUlist();
        }
        #endregion
        #endregion
        
        #region Attr Combo Index Change
        private void cbAttr1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbAttr1.SelectedIndex != -1)
                txtAttr1.Text = cbAttr1.SelectedItem.ToString();
          //  UpdatePUlist();
        }

        private void cbAttr2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbAttr2.SelectedIndex != -1)
                txtAttr2.Text = cbAttr2.SelectedItem.ToString();
          //  UpdatePUlist();
        }

        private void cbAttr3_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbAttr3.SelectedIndex != -1)
                txtAttr3.Text = cbAttr3.SelectedItem.ToString();
          //  UpdatePUlist();
        }

        private void cbAttr4_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbAttr4.SelectedIndex != -1)
                txtAttr4.Text = cbAttr4.SelectedItem.ToString();
           // UpdatePUlist();
        }

        private void cbAttr5_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbAttr5.SelectedIndex != -1)
                txtAttr5.Text = cbAttr5.SelectedItem.ToString();
           // UpdatePUlist();
        }

        private void cbAttr6_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbAttr6.SelectedIndex != -1)
                txtAttr6.Text = cbAttr6.SelectedItem.ToString();
           // UpdatePUlist();
        }

        private void cbAttr7_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbAttr7.SelectedIndex != -1)
                txtAttr7.Text = cbAttr7.SelectedItem.ToString();
          //  UpdatePUlist();
        }

        private void cbAttr8_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbAttr8.SelectedIndex != -1)
                txtAttr8.Text = cbAttr8.SelectedItem.ToString();
           // UpdatePUlist();
        }

        private void cbAttr9_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbAttr9.SelectedIndex != -1)
                txtAttr9.Text = cbAttr9.SelectedItem.ToString();
           // UpdatePUlist();
        }

        private void cbAttr10_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbAttr10.SelectedIndex != -1)
                txtAttr10.Text = cbAttr10.SelectedItem.ToString();
          //  UpdatePUlist();
        }

        private void cbAttr11_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbAttr11.SelectedIndex != -1)
                txtAttr11.Text = cbAttr11.SelectedItem.ToString();
           // UpdatePUlist();
        }

        private void cbAttr12_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbAttr12.SelectedIndex != -1)
                txtAttr12.Text = cbAttr12.SelectedItem.ToString();
          // UpdatePUlist();
        }
        #endregion

        #region change value of quantity 1
        private void txtQuantity1_TextChanged(object sender, EventArgs e)
        {
            if (cbPuTrFeature.Text == "LD_IN" )
            {
                int num=0;
                if(int.TryParse(txtQuantity1.Text,out num))
                {
                    if (num > 4)
                        txtAttr3.Text = "Y";
                    else txtAttr3.Text = "N";
                    txtAttr3.Visible = true;
                    txtAttr3.ReadOnly = true;
                    cbAttr3.Visible = false;
                    if(cbPlantUnit.Items.Count>0)
                        UpdatePUlist();
                   
                }
                else MessageBox.Show("Please enter numeric value for <"+lbQuantity1.Text+">!", "PU Trigger Placement", MessageBoxButtons.OK, MessageBoxIcon.Information);
           
            }
        }
        #endregion

        #region Place button
        private void btnPlace_Click(object sender, EventArgs e)
        {
            if (Validate())
            {
                this.Hide();
                m_gtapp.SelectedObjects.Clear();
                m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Point to place PU Trigger!Right click to cancel placement");
                PlaceValue = 30;
                CreatePuTrFeature();
            }


        }

        #endregion

        #region Validation
        private bool Validate()
        {
           
            if (cbPlantUnit.SelectedIndex == -1)
            {
                MessageBox.Show("Please select Plant Unit!", "PU Trigger Placement", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return false;
            }
            UpdateAttrBasedOnPlantUnit();

            int num = 0;

            if (!txtQuantity1.ReadOnly && !int.TryParse(txtQuantity1.Text, out num))
            {
                MessageBox.Show("Please enter numeric value for <" + lbQuantity1.Text + ">!", "PU Trigger Placement", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return false;
            }
            if (!txtQuantity2.ReadOnly && !int.TryParse(txtQuantity2.Text, out num))
            {
                MessageBox.Show("Please enter numeric value for <" + lbQuantity2.Text + ">!", "PU Trigger Placement", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return false;
            }
            if (!txtQuantity3.ReadOnly && !int.TryParse(txtQuantity3.Text, out num))
            {
                MessageBox.Show("Please enter numeric value for <" + lbQuantity3.Text + ">!", "PU Trigger Placement", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return false;
            }
            if (!txtQuantity4.ReadOnly && !int.TryParse(txtQuantity4.Text, out num))
            {
                MessageBox.Show("Please enter numeric value for <" + lbQuantity4.Text + ">!", "PU Trigger Placement", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return false;
            }
            return true;
        }
        #endregion

        #region Place PU Trigger /create feature in database
        public void CommitToDBnewPUTr(IGTPoint Point1)
        {
            try
            {
                short iCNO = 0;

                short iFNO = PUTriggerFNO;               
                int iFID = oNewFeature.FID;
                iCNO = PUTriggerPriGeoCNO;
                if (oNewFeature.Components.GetComponent(iCNO).Recordset.EOF)
                {
                    oNewFeature.Components.GetComponent(iCNO).Recordset.AddNew("G3E_FID", iFID);
                    oNewFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_FNO", iFNO);
                }
                else
                {
                    oNewFeature.Components.GetComponent(iCNO).Recordset.MoveLast();
                }


                IGTOrientedPointGeometry oOrPointGeom = GTClassFactory.Create<IGTOrientedPointGeometry>();
                int dRotation = 0;
                oOrPointGeom.Origin = Point1;
                // Radians
                oOrPointGeom.Orientation = GTClassFactory.Create<IGTVector>();
                oOrPointGeom.Orientation.I = Math.Cos(dRotation);
                oOrPointGeom.Orientation.J = Math.Sin(dRotation);
                oOrPointGeom.Orientation.K = 0.0;
                oNewFeature.Components.GetComponent(iCNO).Geometry = oOrPointGeom;

                GTPUTrigger.m_oGTTransactionManager.Commit();
                GTPUTrigger.m_oGTTransactionManager.RefreshDatabaseChanges();
                MessageBox.Show("PU Trigger were successfully placed!", "PU Trigger Placement", MessageBoxButtons.OK, MessageBoxIcon.Information);

            }
            catch (Exception e)
            {
                if (GTPUTrigger.m_oGTTransactionManager.TransactionInProgress)
                    GTPUTrigger.m_oGTTransactionManager.Rollback();

                PlaceValue = 0;
                MessageBox.Show(e.Message, "PU Trigger Placement", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
        }
        public void CancelPlacement()
        {
            if (GTPUTrigger.m_oGTTransactionManager.TransactionInProgress)
                GTPUTrigger.m_oGTTransactionManager.Rollback();

            oNewFeature = null;
            MessageBox.Show("PU Trigger placement were canceled!", "PU Trigger Placement", MessageBoxButtons.OK, MessageBoxIcon.Information);


        }

        private void CreatePuTrFeature()
        {
            try
            {
                short iFNO = PUTriggerFNO;
                short iCNO = 0;
                GTPUTrigger.m_oGTTransactionManager.Begin("Place PU Trigger 1");
                oNewFeature = null;
                 oNewFeature = m_GTDataContext.NewFeature(iFNO);
                int iFID = oNewFeature.FID;
                // netelem component for common detail window
                iCNO = PUTriggerNetElemCNO;
                if (oNewFeature.Components.GetComponent(iCNO).Recordset.EOF)
                {
                    oNewFeature.Components.GetComponent(iCNO).Recordset.AddNew("G3E_FID", iFID);
                }
                else
                {
                    oNewFeature.Components.GetComponent(iCNO).Recordset.MoveLast();
                }

                oNewFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_FNO", iFNO);
                oNewFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_CID", 1);
                oNewFeature.Components.GetComponent(iCNO).Recordset.Update("MIN_MATERIAL", cbPlantUnit.SelectedItem.ToString());
                oNewFeature.Components.GetComponent(iCNO).Recordset.Update("FEATURE_STATE", "PPF");
                oNewFeature.Components.GetComponent(iCNO).Recordset.Update("BILLING_RATE", cbBillingRate.SelectedItem.ToString());

                iCNO = PUTriggerAttribCNO;
                if (oNewFeature.Components.GetComponent(iCNO).Recordset.EOF)
                {
                    oNewFeature.Components.GetComponent(iCNO).Recordset.AddNew("G3E_FID", iFID);
                   
                }
                else
                {
                    oNewFeature.Components.GetComponent(iCNO).Recordset.MoveLast();
                 }

                 oNewFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_FNO", iFNO);
                 oNewFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_CID", 1);
                 if (txtQuantity1.Text != "")
                     oNewFeature.Components.GetComponent(iCNO).Recordset.Update("QUANTITY1", txtQuantity1.Text);
                 if (txtQuantity2.Text != "")
                     oNewFeature.Components.GetComponent(iCNO).Recordset.Update("QUANTITY2", txtQuantity2.Text);
                 if (txtQuantity3.Text != "")
                     oNewFeature.Components.GetComponent(iCNO).Recordset.Update("QUANTITY3", txtQuantity3.Text);
                 if (txtQuantity4.Text != "")
                     oNewFeature.Components.GetComponent(iCNO).Recordset.Update("QUANTITY4", txtQuantity4.Text);
                 oNewFeature.Components.GetComponent(iCNO).Recordset.Update("ATT_1", txtAttr1.Text);
                 oNewFeature.Components.GetComponent(iCNO).Recordset.Update("ATT_2", txtAttr2.Text);
                 oNewFeature.Components.GetComponent(iCNO).Recordset.Update("ATT_3", txtAttr3.Text);
                 oNewFeature.Components.GetComponent(iCNO).Recordset.Update("ATT_4", txtAttr4.Text);
                 oNewFeature.Components.GetComponent(iCNO).Recordset.Update("ATT_5", txtAttr5.Text);
                 oNewFeature.Components.GetComponent(iCNO).Recordset.Update("ATT_6", txtAttr6.Text);
                 oNewFeature.Components.GetComponent(iCNO).Recordset.Update("ATT_7", txtAttr7.Text);
                 oNewFeature.Components.GetComponent(iCNO).Recordset.Update("ATT_8", txtAttr8.Text);
                 oNewFeature.Components.GetComponent(iCNO).Recordset.Update("ATT_9", txtAttr9.Text);
                 oNewFeature.Components.GetComponent(iCNO).Recordset.Update("ATT_10", txtAttr10.Text);
                 oNewFeature.Components.GetComponent(iCNO).Recordset.Update("ATT_11", txtAttr11.Text);
                 oNewFeature.Components.GetComponent(iCNO).Recordset.Update("ATT_12", txtAttr12.Text);
                 oNewFeature.Components.GetComponent(iCNO).Recordset.Update("SOURCE_FID1", featFID1);
                 oNewFeature.Components.GetComponent(iCNO).Recordset.Update("SOURCE_USERNAME1", txtFeatName1.Text);
                 oNewFeature.Components.GetComponent(iCNO).Recordset.Update("SOURCE_FID2", featFID2);
                 oNewFeature.Components.GetComponent(iCNO).Recordset.Update("SOURCE_USERNAME2", txtFeatName2.Text);
                 oNewFeature.Components.GetComponent(iCNO).Recordset.Update("PU_FEATURE", cbPuTrFeature.SelectedItem.ToString());
                 if (cbPuTrFeature.SelectedItem.ToString() == "IB_T | FIB")
                     oNewFeature.Components.GetComponent(iCNO).Recordset.Update("PU_FEATURE", "IB_T");


                IGTHelperService helper = GTClassFactory.Create<IGTHelperService>();
                helper.DataContext = m_GTDataContext;
                StyleId = helper.GetComponentStyleID(oNewFeature, PUTriggerPriGeoCNO);

            }
            catch (Exception e)
            {
                if (GTPUTrigger.m_oGTTransactionManager.TransactionInProgress)
                    GTPUTrigger.m_oGTTransactionManager.Rollback();
                PlaceValue = 0;
                oNewFeature = null;
                MessageBox.Show(e.Message, "PU Trigger Placement", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Show();
            }

        }
        #endregion

        private void btnAttr_Click(object sender, EventArgs e)
        {
            if (btnAttr.Text == "Expand Attributes") // expand
            {
                btnAttr.Text = "Suspend Attributes";
                panAttr.Height = 406;
                this.Height = 734;
            }
            else
            {
                btnAttr.Text = "Expand Attributes";
                panAttr.Height = 34;
                this.Height = 362;
            }
        }

    
    }       
}