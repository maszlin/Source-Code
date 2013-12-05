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

namespace NEPS.GTechnology.UpdateFDC
{
    public partial class GTWindowsForm_UpdateFDC : Form
    {
       
        public static IGTApplication m_GeoApp;
        //IGTApplication m_GeoApp = GTClassFactory.Create<IGTApplication>();
        private Logger log;
        private IGTDataContext m_GTDataContext = null;
        public bool flag = false;

        string ParentXY = "";
        string[] ParantBND;
        int ParentFID = 0;

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



        public GTWindowsForm_UpdateFDC()
        {
            try
            {
                InitializeComponent();
                //m_gtapp = GTApplication.Application;
                m_gtapp =  GTClassFactory.Create<IGTApplication>().Application;
                m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Running Update FDC...");

                log = Logger.getInstance();
              
            }
            catch (Exception ex)
            {
                log.WriteLog("");
                log.WriteLog(this.GetType().FullName, "ERROR: " + ex.Message);
                log.WriteLog(ex.StackTrace);            
                MessageBox.Show(ex.Message, "Update FDC", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }        
        

        private Intergraph.GTechnology.API.IGTApplication m_gtapp = null; 
        //IGTApplication m_gtapp = GTClassFactory.Create<IGTApplication>();


        private void GTWindowsForm_InitRoute_Shown(object sender, EventArgs e)
        {
            m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Running Update FDC...");            
        }

        private void GTWindowsForm_InitRoute_Activated(object sender, EventArgs e)
        {
            m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Running Update FDC...");            
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
                MessageBox.Show(ex.Message, "Update FDC", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }

        }

        //Load all OLT ID from ISP_PLACEMENT Table
        private void Load_FDP()
        {
            try
            {       
            string sSql;
            int recordsAffected = 0;           
            this.m_GTDataContext = GTClassFactory.Create<IGTApplication>().DataContext;
            //Vinod 27-May-2012 Request from Kelvin
            sSql = "select NAME  from ISP_CUSTOM.ISP_PLACEMENT where TYPE like 'OLT%' and LOCATION = '" + txt_Location.Text.Trim() + "'";
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
                MessageBox.Show(ex.Message, "Update FDC", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return ;
            }
        }

        private void GTWindowsForm_InitRoute_Load(object sender, EventArgs e)
        {
            
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
           
        //Pick a FDC
        private void btn_Pick_Click(object sender, EventArgs e)
        { try
            {
            IGTGeometry geom = null;
            short iFNO = 0;
            int iFID = 0;

            if (GTClassFactory.Create<IGTApplication>().SelectedObjects.FeatureCount == 0)
            {
                MessageBox.Show("Please select a FDC", "Update FDC", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            foreach (IGTDDCKeyObject oDDCKeyObject in GTClassFactory.Create<IGTApplication>().SelectedObjects.GetObjects())
            {
                geom = oDDCKeyObject.Geometry;
                iFNO = oDDCKeyObject.FNO;
                iFID = oDDCKeyObject.FID;

                if (oDDCKeyObject.ComponentViewName == "VGC_FDC_S")
                {
                    if (iFNO == 5100)
                    {
                        txtFDC.Text = iFID.ToString();
                        lblFID.Text = iFID.ToString();
                        txtFDCcode.Text = Get_Value("Select FDC_CODE from gc_fdc where g3e_fid = " + iFID.ToString());
                        txt_Location.Text = Get_Value("Select EXC_ABB from GC_NETELEM where g3e_fid = " + iFID.ToString());
                        Load_FDP();
                    }
                }
            } }
            catch (Exception ex)
            {
                log.WriteLog("");
                log.WriteLog(this.GetType().FullName, "ERROR: " + ex.Message);
                log.WriteLog(ex.StackTrace);
                MessageBox.Show(ex.Message, "Update FDC", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return ;
            }
        }
        //Update OLT_ID for FDC
        private void btn_Update_Click(object sender, EventArgs e)
        { try
            {
            string strSQL = null;
            int iRecordNum = 0;
            object[] obj = null;
            
            if (txtFDC.Text == "")
            {
                MessageBox.Show("Please Select a FDC", "Update FDC", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (cboType.Text == "")
            {
                MessageBox.Show("Please Select an OLT ID", "Update FDC", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            GTCustomCommandModeless.m_oGTTransactionManager.Begin("UpdateFDC");
            IGTKeyObject oExtFeature;
            oExtFeature = GTClassFactory.Create<IGTApplication>().DataContext.OpenFeature(5100, Convert.ToInt32(lblFID.Text));
            oExtFeature.Components.GetComponent(5101).Recordset.Update("OLT_ID", cboType.Text);
            
            //Vinod 27-May-2012
            //commented by Anna 07 Aug 2012
            //FID - should be for OLT, updating is wrong, comment till next clarification

         //   strSQL = "UPDATE ISP_CUSTOM.ISP_PLACEMENT SET FID = " + lblFID.Text + " where where TYPE = 'OLT' and LOCATION = '" + txt_Location.Text.Trim() + "' and NAME = '" + cboType.Text + "'";
          //  m_gtapp.DataContext.Execute(strSQL, out iRecordNum, (int)ADODB.CommandTypeEnum.adCmdText, obj);

            GTCustomCommandModeless.m_oGTTransactionManager.Commit();
            GTCustomCommandModeless.m_oGTTransactionManager.RefreshDatabaseChanges();

            MessageBox.Show("OLT ID  is successfully updated!", "Update FDC", MessageBoxButtons.OK, MessageBoxIcon.Information);
            this.Close();
        }
        catch (Exception ex)
        {
            log.WriteLog("");
            log.WriteLog(this.GetType().FullName, "ERROR: " + ex.Message);
            log.WriteLog(ex.StackTrace);
            MessageBox.Show(ex.Message, "Update FDC", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }
        }

       
        
    }
}