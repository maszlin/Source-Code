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

namespace NEPS.GTechnology.CopyStreetAddress
{
    public partial class CopyStreetAddress : Form
    {       
        public static IGTApplication m_GeoApp;        
        private Logger log;
       

        public CopyStreetAddress()
        {
            try
            {
                InitializeComponent();               
                m_gtapp =  GTClassFactory.Create<IGTApplication>().Application;
                m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Running Copy Street Address...");

                log = Logger.getInstance();
               
            }
            catch (Exception ex)
            {
                log.WriteLog("");
                log.WriteLog(this.GetType().FullName, "ERROR: " + ex.Message);
                log.WriteLog(ex.StackTrace);
                MessageBox.Show(ex.Message, "Running Copy Street Address...", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }        
        

        private Intergraph.GTechnology.API.IGTApplication m_gtapp = null; 
        

        private void GTWindowsForm_InitRoute_Shown(object sender, EventArgs e)
        {
            m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Running Copy Street Address...");            
        }

        private void GTWindowsForm_InitRoute_Activated(object sender, EventArgs e)
        {
            m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Running Copy Street Address...");            
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
                MessageBox.Show(ex.Message, "Running Copy Street Address...", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }

        }
        
    

        private void GTWindowsForm_InitRoute_Load(object sender, EventArgs e)
        {
           
        }

        int PreFID = 0;
        private void button1_Click(object sender, EventArgs e)
        {
            

                IGTGeometry geom = null;
                short iFNO = 0;
                int iFID = 0;
                string ViewName = string.Empty;


                if (GTClassFactory.Create<IGTApplication>().SelectedObjects.FeatureCount == 0)
                {
                    MessageBox.Show("Please select a Feature", "Copy Street Address", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                foreach (IGTDDCKeyObject oDDCKeyObject in GTClassFactory.Create<IGTApplication>().SelectedObjects.GetObjects())
                {
                    geom = oDDCKeyObject.Geometry;
                    iFNO = oDDCKeyObject.FNO;
                    iFID = oDDCKeyObject.FID;

                    string REF_FNO = Get_Value("SELECT FNO FROM REF_COPYSTREET WHERE fno =" + iFNO);
                    //Vinod Change from Kelvin on 27-Jun-2012
                    //if (iFNO == 5100 || iFNO == 5200 || iFNO == 5400 || iFNO == 9100 || iFNO == 9300 || iFNO == 9400 || iFNO == 9500 || iFNO == 9600 || iFNO == 9700 || iFNO == 5600)
                    if (iFNO.ToString() == REF_FNO)
                    {
                        ViewName = Get_Value("SELECT G3E_TABLE FROM  G3E_COMPONENTINFO_OPTLANG WHERE G3E_CNO = (SELECT G3E_PRIMARYGEOGRAPHICCNO FROM G3E_FEATURES_OPTLANG WHERE G3E_FNO = " + iFNO + ")");

                        if (oDDCKeyObject.ComponentViewName == "V" + ViewName)
                        {
                            Add_Device_Grid(iFNO, iFID);

                            if (iFNO == 5600)
                            {
                                int ChildFID = -1;
                                
                                string FID = Get_Value("SELECT FDC_FID FROM GC_FDP WHERE G3E_FID=" + iFID);
                                if ( FID == "")                                
                                    ChildFID = -1;
                                else
                                    ChildFID = Convert.ToInt32(FID.ToString());

                                if (ChildFID != -1)
                                    Add_Device_Grid(5100, ChildFID);
                            }

                        }
                    }
                    else
                    {
                        MessageBox.Show("Copy address not supported this selected feature", "Copy Street Address", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }
                }
            
        }

        private void GetAddress(int FID, ref AddressDef oAdd)
        {
            try
            {
                ADODB.Recordset rsPP = new ADODB.Recordset();
                string sSql = "SELECT STREET_NAME, STREET_TYPE, POSTAL_CODE, SEC_NAME, CITY, STATE_CODE FROM GC_ADDRESS WHERE G3E_FID =" + FID + "";
                rsPP = GTClassFactory.Create<IGTApplication>().DataContext.OpenRecordset(sSql, ADODB.CursorTypeEnum.adOpenStatic, ADODB.LockTypeEnum.adLockReadOnly, 1);
                if (rsPP.RecordCount > 0)
                {
                    oAdd.STREET_NAME = rsPP.Fields["STREET_NAME"].Value.ToString().TrimEnd();
                    oAdd.STREET_TYPE = rsPP.Fields["STREET_TYPE"].Value.ToString();
                    oAdd.POSTAL_NUM = rsPP.Fields["POSTAL_CODE"].Value.ToString();
                    oAdd.SECTION_NAME = rsPP.Fields["SEC_NAME"].Value.ToString();
                    oAdd.CITY_NAME = rsPP.Fields["CITY"].Value.ToString();
                    oAdd.STATE_CODE = rsPP.Fields["STATE_CODE"].Value.ToString();
                                       
                }

            }
            catch
            {

            }
        }

        IGTDDCKeyObjects oGTKeyObjs;

        private void dvFeat_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (dvFeat.SelectedRows.Count > 0)
            {
                Highlight();
            }
        }

        private void Highlight()
        {
            try
            {
                short mFNO = 0;
                int mFID = 0;

                if (dvFeat.Rows.Count > 0)
                {

                    GTClassFactory.Create<IGTApplication>().SelectedObjects.Clear();
                    foreach (DataGridViewRow row in dvFeat.Rows)
                    {
                        if (row.Selected == true)
                        {
                            mFID = int.Parse(row.Cells[2].Value.ToString());
                            mFNO = short.Parse(row.Cells[0].Value.ToString());

                            oGTKeyObjs = (IGTDDCKeyObjects)GTClassFactory.Create<IGTApplication>().DataContext.GetDDCKeyObjects(mFNO, mFID, GTComponentGeometryConstants.gtddcgAllGeographic);
                            for (int K = 0; K < oGTKeyObjs.Count; K++)
                                GTClassFactory.Create<IGTApplication>().SelectedObjects.Add(GTSelectModeConstants.gtsosmAllDisplayedComponentsInActiveLegend, oGTKeyObjs[K]);
                        }
                    }
                                       
                    GTClassFactory.Create<IGTApplication>().ActiveMapWindow.CenterSelectedObjects();
                    GTClassFactory.Create<IGTApplication>().ActiveMapWindow.DisplayScale = 500;
                    
                    GTClassFactory.Create<IGTApplication>().RefreshWindows();
                    oGTKeyObjs = null;
                }
            }
            catch (Exception ex)
            {
                log.WriteLog("");
                log.WriteLog(this.GetType().FullName, "ERROR: " + ex.Message);
                log.WriteLog(ex.StackTrace);
                MessageBox.Show(ex.Message, "Copy Street Address", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void Add_Device_Grid(short iFNO, int iFID)
        {
            if (PreFID != iFID)
            {
                PreFID = iFID;

                string tablename = string.Empty;
                string feature_name = string.Empty;
                string Code = string.Empty;
                string[] codename = new string[2];
                string Code_Col = string.Empty;

                try
                {
                    tablename = Get_Value("SELECT G3E_TABLE FROM  G3E_COMPONENTINFO_OPTLANG WHERE G3E_CNO = (SELECT G3E_PRIMARYATTRIBUTECNO FROM G3E_FEATURES_OPTLANG WHERE G3E_FNO = " + iFNO + ")");
                    feature_name = Get_Value("SELECT G3E_USERNAME FROM G3E_FEATURES_OPTLANG WHERE G3E_FNO = " + iFNO + "");
                    
                    //23-Sep-2012 New Colum added in REF_COPYSTREET to get the Code Column 
                    Code_Col = Get_Value("SELECT CODE FROM REF_COPYSTREET WHERE fno =" + iFNO);
                    Code = Get_Value("SELECT " + Code_Col + " FROM " + tablename + " WHERE G3E_FID=" + iFID + " ");

                    //codename = tablename.Split('_');
                    //if (tablename == "GC_MSAN")
                    //    Code = "RT_CODE";
                    //else
                    //    Code = Get_Value("SELECT " + codename[1].ToString() + "_CODE FROM " + tablename + " WHERE G3E_FID=" + iFID + " ");                    

                    int iRow = 0;
                    iRow = dvFeat.Rows.Add();
                    dvFeat["G3E_FID", iRow].Value = iFID;
                    dvFeat["G3E_FNO", iRow].Value = iFNO;
                    dvFeat["FEAT_NAME", iRow].Value = feature_name;
                    dvFeat["CODE", iRow].Value = Code;


                    ADODB.Recordset rsPP = new ADODB.Recordset();
                    string sSql = "SELECT STREET_NAME, STREET_TYPE, POSTAL_CODE, SEC_NAME, CITY, STATE_CODE FROM GC_ADDRESS WHERE G3E_FID =" + iFID + "";
                    rsPP = GTClassFactory.Create<IGTApplication>().DataContext.OpenRecordset(sSql, ADODB.CursorTypeEnum.adOpenStatic, ADODB.LockTypeEnum.adLockReadOnly, 1);
                    if (rsPP.RecordCount > 0)
                    {
                        dvFeat["StreetName1", iRow].Value = rsPP.Fields["STREET_NAME"].Value.ToString().TrimEnd();
                        dvFeat["StreetType1", iRow].Value = rsPP.Fields["STREET_TYPE"].Value.ToString();
                        dvFeat["PostalNumber", iRow].Value = rsPP.Fields["POSTAL_CODE"].Value.ToString();
                        dvFeat["SectionName1", iRow].Value = rsPP.Fields["SEC_NAME"].Value.ToString();
                        dvFeat["CityName1", iRow].Value = rsPP.Fields["CITY"].Value.ToString();
                        dvFeat["StateCode1", iRow].Value = rsPP.Fields["STATE_CODE"].Value.ToString();
                    }

                    dvFeat.ClearSelection();
                }
                catch (Exception ex)
                {
                    log.WriteLog("");
                    log.WriteLog(this.GetType().FullName, "ERROR: " + ex.Message);
                    log.WriteLog(ex.StackTrace);
                    MessageBox.Show(ex.Message, "Copy Street Address", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                AddressDef oAdd = new AddressDef();
                int FID;
                short FNO;
                GTCustomCommandModeless.m_oGTTransactionManager.Begin("Update Address");

                int iRow = 0;
                for (iRow = 0; iRow <= dvFeat.Rows.Count - 1; iRow++)
                {
                    FID = Convert.ToInt32(dvFeat["G3E_FID", iRow].Value);
                    FNO = Convert.ToInt16(dvFeat["G3E_FNO", iRow].Value);
                   
                    if(dvFeat["STREET_NAME", iRow].Value != null )
                    {
                        oAdd.STREET_NAME = dvFeat["STREET_NAME", iRow].Value.ToString();
                    }
                    if (dvFeat["STREET_TYPE", iRow].Value != null)
                    {
                        oAdd.STREET_TYPE = dvFeat["STREET_TYPE", iRow].Value.ToString();
                    }
                    if (dvFeat["POSTAL_NUM", iRow].Value != null)
                    {
                        oAdd.POSTAL_NUM = dvFeat["POSTAL_NUM", iRow].Value.ToString();
                    }
                    if (dvFeat["SECTION_NAME", iRow].Value != null)
                    {
                        oAdd.SECTION_NAME = dvFeat["SECTION_NAME", iRow].Value.ToString();
                    }
                    if (dvFeat["CITY_NAME", iRow].Value != null)
                    {
                        oAdd.CITY_NAME = dvFeat["CITY_NAME", iRow].Value.ToString();
                    }
                    if (dvFeat["STATE_CODE", iRow].Value != null)
                    {
                        oAdd.STATE_CODE = dvFeat["STATE_CODE", iRow].Value.ToString();
                    }
                    
                    UpdateAddress(FNO, FID, oAdd);
                }

                GTCustomCommandModeless.m_oGTTransactionManager.Commit();
                GTCustomCommandModeless.m_oGTTransactionManager.RefreshDatabaseChanges();
                MessageBox.Show("Address Copied Succesfully", "Copy Street Address", MessageBoxButtons.OK, MessageBoxIcon.Information );
                this.Close();
            }
            catch (Exception ex)
            {
                log.WriteLog("");
                log.WriteLog(this.GetType().FullName, "ERROR: " + ex.Message);
                log.WriteLog(ex.StackTrace);
                MessageBox.Show(ex.Message, "Copy Street Address", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void UpdateAddress(short iFNO, int iFID, AddressDef oAdd)
        {
            short iCNO;           
            IGTKeyObject oFeature;
            IGTPoint oPointL;
            IGTTextPointGeometry oTextGeom;
            oFeature = GTClassFactory.Create<IGTApplication>().DataContext.OpenFeature(iFNO, iFID);           
            
            //Address
            iCNO = 52;                   

            if (oFeature.Components.GetComponent(iCNO).Recordset.EOF)
            {
                oFeature.Components.GetComponent(iCNO).Recordset.AddNew("G3E_FID", iFID);
                oFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_FNO", iFNO);
                oFeature.Components.GetComponent(iCNO).Recordset.Update("STREET_NAME", oAdd.STREET_NAME );
                oFeature.Components.GetComponent(iCNO).Recordset.Update("STREET_TYPE", oAdd.STREET_TYPE );
                oFeature.Components.GetComponent(iCNO).Recordset.Update("POSTAL_CODE", oAdd.POSTAL_NUM);
                oFeature.Components.GetComponent(iCNO).Recordset.Update("SEC_NAME", oAdd.SECTION_NAME);
                oFeature.Components.GetComponent(iCNO).Recordset.Update("CITY", oAdd.CITY_NAME);
                oFeature.Components.GetComponent(iCNO).Recordset.Update("STATE_CODE", oAdd.STATE_CODE); 
            }
            else
            {
                oFeature.Components.GetComponent(iCNO).Recordset.MoveLast();
                oFeature.Components.GetComponent(iCNO).Recordset.Update("STREET_NAME", oAdd.STREET_NAME);
                oFeature.Components.GetComponent(iCNO).Recordset.Update("STREET_TYPE", oAdd.STREET_TYPE);
                oFeature.Components.GetComponent(iCNO).Recordset.Update("POSTAL_CODE", oAdd.POSTAL_NUM);
                oFeature.Components.GetComponent(iCNO).Recordset.Update("SEC_NAME", oAdd.SECTION_NAME);
                oFeature.Components.GetComponent(iCNO).Recordset.Update("CITY", oAdd.CITY_NAME);
                oFeature.Components.GetComponent(iCNO).Recordset.Update("STATE_CODE", oAdd.STATE_CODE); 
            }

            //16-Oct-2012
            string S_CNO = Get_Value("SELECT G3E_PRIMARYGEOGRAPHICCNO FROM G3E_FEATURES_OPTLANG WHERE G3E_FNO =" + iFNO);
            iCNO = Convert.ToInt16(S_CNO);
            oPointL = GTClassFactory.Create<IGTPoint>();
            oPointL.X = oFeature.Components.GetComponent(iCNO).Geometry.FirstPoint.X + 2;
            oPointL.Y = oFeature.Components.GetComponent(iCNO).Geometry.FirstPoint.Y ;
            oPointL.Z = oFeature.Components.GetComponent(iCNO).Geometry.FirstPoint.Z;

            string Add_ICNO = Get_Value("SELECT CNO FROM REF_COPYSTREET WHERE FNO =" + iFNO);
            if (Add_ICNO != "")
            {
                iCNO = Convert.ToInt16(Add_ICNO);
                oTextGeom = GTClassFactory.Create<IGTTextPointGeometry>();
                oTextGeom.Origin = oPointL;
                oTextGeom.Rotation = 0;

                if (oFeature.Components.GetComponent(iCNO).Recordset.EOF)
                {
                    oFeature.Components.GetComponent(iCNO).Recordset.AddNew("G3E_FID", iFID);
                    oFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_FNO", iFNO);
                    oFeature.Components.GetComponent(iCNO).Geometry = oTextGeom;
                }
                //else
                //{
                //    oFeature.Components.GetComponent(iCNO).Recordset.MoveLast();
                //    oFeature.Components.GetComponent(iCNO).Geometry = oTextGeom;
                //}
            }
        }

        private void Clear()
        {
            label2.Text = "";
            label4.Text = "";
            label3.Text = "";
            label9.Text = "";
            label11.Text = "";
        }

        private void button2_Click(object sender, EventArgs e)
        {
            IGTGeometry geom = null;
            short iFNO = 0;
            int iFID = 0;
            string ViewName = string.Empty;
            bool StreetSel = false;

            Clear();

            if (GTClassFactory.Create<IGTApplication>().SelectedObjects.FeatureCount == 0)
            {
                MessageBox.Show("Please select a Street", "Copy Street Address", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            foreach (IGTDDCKeyObject oDDCKeyObject in GTClassFactory.Create<IGTApplication>().SelectedObjects.GetObjects())
            {
                geom = oDDCKeyObject.Geometry;
                iFNO = oDDCKeyObject.FNO;
                iFID = oDDCKeyObject.FID;

                if (oDDCKeyObject.ComponentViewName == "VGC_ADM_STREET_L")
                {
                    Add_Street(iFNO, iFID);
                    StreetSel = true;
                }                
            }

            if (StreetSel == false)
            {
                MessageBox.Show("Please select a Street Line", "Copy Street Address", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

        }

        private void Add_Street(short iFNO, int iFID)
        {
            AddressDef oAdd = new AddressDef();                 
            
           
            try
            {
                ADODB.Recordset rsPP = new ADODB.Recordset();
                string sSql = "SELECT STREET_NAME, STREET_TYPE, POSTAL_NUM, SECTION_NAME, CITY_NAME, STATE_CODE FROM GC_ADM_STREET WHERE G3E_FID =" + iFID + "";
                rsPP = GTClassFactory.Create<IGTApplication>().DataContext.OpenRecordset(sSql, ADODB.CursorTypeEnum.adOpenStatic, ADODB.LockTypeEnum.adLockReadOnly, 1);
                if (rsPP.RecordCount > 0)
                {
                    oAdd.STREET_NAME = rsPP.Fields["STREET_NAME"].Value.ToString().TrimEnd();
                    oAdd.STREET_TYPE = rsPP.Fields["STREET_TYPE"].Value.ToString();
                    oAdd.POSTAL_NUM = rsPP.Fields["POSTAL_NUM"].Value.ToString();
                    oAdd.SECTION_NAME = rsPP.Fields["SECTION_NAME"].Value.ToString();
                    oAdd.CITY_NAME = rsPP.Fields["CITY_NAME"].Value.ToString();
                    oAdd.STATE_CODE = rsPP.Fields["STATE_CODE"].Value.ToString();

                    txtStreet.Text = oAdd.STREET_NAME.ToString();
                    label2.Text = oAdd.STREET_TYPE;
                    label4.Text = oAdd.POSTAL_NUM;
                    label3.Text = oAdd.SECTION_NAME;
                    label9.Text = oAdd.CITY_NAME;
                    label11.Text = oAdd.STATE_CODE;
                }

                UpdateGrid(oAdd);
                
            }
            catch (Exception ex)
            {
                log.WriteLog("");
                log.WriteLog(this.GetType().FullName, "ERROR: " + ex.Message);
                log.WriteLog(ex.StackTrace);
                MessageBox.Show(ex.Message, "Copy Street Address...", MessageBoxButtons.OK, MessageBoxIcon.Error);
                
            }
        }

        private void UpdateGrid(AddressDef oADD)
        {
            int iRow = 0;

            for (iRow = 0; iRow <= dvFeat.Rows.Count - 1; iRow++)
            {
                dvFeat["STREET_NAME", iRow].Value = oADD.STREET_NAME;
                dvFeat["STREET_TYPE", iRow].Value = oADD.STREET_TYPE;
                dvFeat["POSTAL_NUM", iRow].Value = oADD.POSTAL_NUM;
                dvFeat["SECTION_NAME", iRow].Value = oADD.SECTION_NAME;
                dvFeat["CITY_NAME", iRow].Value = oADD.CITY_NAME;
                dvFeat["STATE_CODE", iRow].Value = oADD.STATE_CODE;
            }

            dvFeat.ClearSelection();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            this.Close();
        }


    }
}