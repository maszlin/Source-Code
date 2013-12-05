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

namespace NEPS.GTechnology.UpdateRTMSAN
{
    public partial class GTWindowsForm_UpdateRTMSAN : Form
    {
       
        public static IGTApplication m_GeoApp;
        //IGTApplication m_GeoApp = GTClassFactory.Create<IGTApplication>();
        private Logger log;
        private IGTDataContext m_GTDataContext = null;
        public bool flag = false;

        string ParentXY = "";
        string[] ParantBND;
        int ParentFID = 0;
        string type = "";

        private string _XPoint;
        public string XPointGeom
        {
            get
            {
                return _XPoint;
            }
            set
            {
                                
            }
        }

        private string _YPoint;
        public string YPointGeom
        {
            get
            {
                return _YPoint;
            }
            set
            {
               // listBox2.Items.Add(value);              
            }
        }



        public GTWindowsForm_UpdateRTMSAN()
        {
            try
            {
                InitializeComponent();
                //m_gtapp = GTApplication.Application;
                m_gtapp =  GTClassFactory.Create<IGTApplication>().Application;
                m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Running Update MSAN/RT/MINIMUX...");

                log = Logger.getInstance();              
            }
            catch (Exception ex)
            {
                log.WriteLog("");
                log.WriteLog(this.GetType().FullName, "ERROR: " + ex.Message);
                log.WriteLog(ex.StackTrace);            
                MessageBox.Show(ex.Message, "Update MSAN/RT/MINIMUX", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }        
        

        private Intergraph.GTechnology.API.IGTApplication m_gtapp = null; 
        //IGTApplication m_gtapp = GTClassFactory.Create<IGTApplication>();


        private void GTWindowsForm_InitRoute_Shown(object sender, EventArgs e)
        {
            m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Running Update MSAN/RT/MINIMUX...");            
        }

        private void GTWindowsForm_InitRoute_Activated(object sender, EventArgs e)
        {
            m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Running Update MSAN/RT/MINIMUX...");            
        }
        
        //Get Value from Database
        private string Get_Value(string sSql)
        {
            try
            {
                ADODB.Recordset rsPP = new ADODB.Recordset();
                rsPP = GTClassFactory.Create<IGTApplication>().DataContext.OpenRecordset(sSql, ADODB.CursorTypeEnum.adOpenStatic, ADODB.LockTypeEnum.adLockReadOnly, 1);
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
                MessageBox.Show(ex.Message, "Update MSAN/RT/MINIMUX", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }

        }

        private void Load_FDP()
        {      try
            {       
            string sSql;
            int recordsAffected = 0;           
            this.m_GTDataContext = GTClassFactory.Create<IGTApplication>().DataContext;
            
            string type_code = "";
            if (type == "MSAN")
                type_code = "COT_IPMSAN%";
            if (type == "RT")
                type_code = "COT_RT%";
            if (type == "MINIMUX")
                type_code = "COT_MINIMUX%";

            sSql = "select NAME  from ISP_CUSTOM.ISP_PLACEMENT where TYPE like '"+type_code+"' and LOCATION = '" + txt_Location.Text.Trim() + "'";

            Recordset rsComp = m_GTDataContext.Execute(sSql, out recordsAffected, (int)CommandTypeEnum.adCmdText, null);
            cboType.Items.Clear();

            if (rsComp.RecordCount > 0)
            {
                rsComp.MoveFirst();             
                do
                {
                    cboType.Items.Add(rsComp.Fields[0].Value.ToString());
                    rsComp.MoveNext();
                }
                while (!rsComp.EOF);
            }
            rsComp = null;
        }
        catch (Exception ex)
        {
            log.WriteLog("");
            log.WriteLog(this.GetType().FullName, "ERROR: " + ex.Message);
            log.WriteLog(ex.StackTrace);
            MessageBox.Show(ex.Message, "Update MSAN/RT/MINIMUX", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }
        }

        private void GTWindowsForm_InitRoute_Load(object sender, EventArgs e)
        {
            
        }
        
        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }   
           
        private void btn_Pick_Click(object sender, EventArgs e)
        {
            IGTGeometry geom = null;
            short iFNO = 0;
            int iFID = 0;
            try
            {
                if (GTClassFactory.Create<IGTApplication>().SelectedObjects.FeatureCount == 0)
                {
                    MessageBox.Show("Please select a MSAN/RT/MINIMUX Feature", "Update MSAN/RT/MINIMUX", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                foreach (IGTDDCKeyObject oDDCKeyObject in GTClassFactory.Create<IGTApplication>().SelectedObjects.GetObjects())
                {
                    iFNO = oDDCKeyObject.FNO;
                    iFID = oDDCKeyObject.FID;
                    if (iFNO == 9600 || iFNO == 9100 || iFNO == 9500)
                        {
                            txtDevice.Text = iFID.ToString();
                            lblFID.Text = iFID.ToString();
                            if (iFNO == 9600)
                            {
                                type = "RT";
                                txtDeviceCode.Text = Get_Value("Select RT_CODE from GC_RT where g3e_fid = " + iFID.ToString());
                            
                            }
                            if (iFNO == 9100)
                            {
                                type = "MSAN";
                                txtDeviceCode.Text = Get_Value("Select RT_CODE from gc_msan where g3e_fid = " + iFID.ToString());
                            
                            }
                            if (iFNO == 9500)
                            {
                                type = "MINIMUX";
                                txtDeviceCode.Text = Get_Value("Select MUX_CODE from gc_minimux where g3e_fid = " + iFID.ToString());
                            }

                            gbFeatureInfo.Text = type;
                            txt_Location.Text = Get_Value("Select EXC_ABB from GC_NETELEM where g3e_fid = " + iFID.ToString());
                            Load_FDP();
                            return;
                        }
                        else
                        {
                            MessageBox.Show("Please select a MSAN/RT/MINIMUX Feature", "Update MSAN/RT/MINIMUX", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            return;
                        }
                    
                }
            }
            catch (Exception ex)
            {
                log.WriteLog("");
                log.WriteLog(this.GetType().FullName, "ERROR: " + ex.Message);
                log.WriteLog(ex.StackTrace);
                MessageBox.Show(ex.Message, "Update MSAN/RT/MINIMUX", MessageBoxButtons.OK, MessageBoxIcon.Error);
               
            }
        }

        private void btn_Update_Click(object sender, EventArgs e)
        {
            try
            {
            if (txtDevice.Text == "")
            {
                MessageBox.Show("Please Select a MSAN/RT/MINIMUX Feature", "Update MSAN/RT/MINIMUX", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (cboType.Text == "")
            {
                MessageBox.Show("Please, Select a COT Code", "Update MSAN/RT/MINIMUX", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            GTCustomCommandModeless.m_oGTTransactionManager.Begin("UpdateRTMSANMINIMUX");
            IGTKeyObject oExtFeature;
            short sFNO = 0;
            string field = "";
            short sCNO = 0;
            if (type == "MSAN")
            {
                sFNO = 9100;
                sCNO = 9101;
                field = "MSAN_COT_CODE";
            }
            if (type == "RT")
            {
                sFNO = 9600;
                sCNO = 9601;
                field = "COT_CODE";
            }
            if (type == "MINIMUX")
            {
                sFNO = 9500;
                sCNO = 9501;
                field = "COT_CODE";
            }
            oExtFeature = GTClassFactory.Create<IGTApplication>().DataContext.OpenFeature(sFNO, Convert.ToInt32(lblFID.Text));
            oExtFeature.Components.GetComponent(sCNO).Recordset.Update(field, cboType.Text);
            GTCustomCommandModeless.m_oGTTransactionManager.Commit();
            GTCustomCommandModeless.m_oGTTransactionManager.RefreshDatabaseChanges();

            MessageBox.Show("COT Code is successfully updated!", "Update MSAN/RT/MINIMUX", MessageBoxButtons.OK, MessageBoxIcon.Information);
            this.Close();   }
            catch (Exception ex)
            {
                log.WriteLog("");
                log.WriteLog(this.GetType().FullName, "ERROR: " + ex.Message);
                log.WriteLog(ex.StackTrace);
                MessageBox.Show(ex.Message, "Update MSAN/RT/MINIMUX", MessageBoxButtons.OK, MessageBoxIcon.Error);
               
            }
        }
        
    }
}