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

namespace NEPS.GTechnology.User_Demand
{
    public partial class GTWindowsForm_User_Demand : Form
    {
       
        public static IGTApplication m_GeoApp;
        //IGTApplication m_GeoApp = GTClassFactory.Create<IGTApplication>();
        private Logger log;
        private string _XPoint;
        public string XPointGeom
        {
            get
            {
                return _XPoint;
            }
            set
            {
                _XPoint = value;
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
                _YPoint = value;
            }
        }

        public GTWindowsForm_User_Demand()
        {
            try
            {
                InitializeComponent();               
                m_gtapp =  GTClassFactory.Create<IGTApplication>().Application;
                m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Running Init Route...");

                log = Logger.getInstance();
               
            }
            catch (Exception ex)
            {
                log.WriteLog("");
                log.WriteLog(this.GetType().FullName, "ERROR: " + ex.Message);
                log.WriteLog(ex.StackTrace);            
                MessageBox.Show(ex.Message, "Init Route", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }        
        

        private Intergraph.GTechnology.API.IGTApplication m_gtapp = null; 
        //IGTApplication m_gtapp = GTClassFactory.Create<IGTApplication>();


        private void GTWindowsForm_InitRoute_Shown(object sender, EventArgs e)
        {
            m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Running Init Route...");            
        }

        private void GTWindowsForm_InitRoute_Activated(object sender, EventArgs e)
        {
            m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Running Init Route...");            
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
                MessageBox.Show(ex.Message, "Property Count", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }

        }
        private IGTDataContext m_GTDataContext = null;

    

        private void GTWindowsForm_InitRoute_Load(object sender, EventArgs e)
        {
           
        }

        private void button1_Click(object sender, EventArgs e)
        {
            IGTGeometry geom = null;
            short iFNO = 0;
            int iFID = 0;

            txtServiceBndy.Text = "";

            if (GTClassFactory.Create<IGTApplication>().SelectedObjects.FeatureCount == 0)
            {
                MessageBox.Show("Please select a Feature", "Property Count", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            foreach (IGTDDCKeyObject oDDCKeyObject in GTClassFactory.Create<IGTApplication>().SelectedObjects.GetObjects())
            {
                geom = oDDCKeyObject.Geometry;
                iFNO = oDDCKeyObject.FNO;
                iFID = oDDCKeyObject.FID;

                if (oDDCKeyObject.ComponentViewName == "VGC_BND_P")
                {
                    if (iFNO == 24000)
                    {
                        Add_Route(iFNO, iFID);                                                
                        ServBndyFID.Text = iFID.ToString();
                        lblDP_FNO.Text  = iFNO.ToString();
                        Get_DP_Property(iFID);
                    }
                }
            }
        }

        string Property_G3E_FID = "";

        private void Get_DP_Property(int iFID)
        {
            //	 select b.g3e_fid   from b$gc_bnd_p a, gc_adm_property_s b  where a.g3e_fid = 1981906 and sdo_inside ( b.g3e_geometry, a.g3e_geometry ) = 'TRUE'; 
            string sql = "select b.g3e_fid from gc_bnd_p a, gc_adm_property_s b  where a.g3e_fid = "+ iFID +" and sdo_inside ( b.g3e_geometry, a.g3e_geometry ) = 'TRUE'";

            this.m_GTDataContext = GTClassFactory.Create<IGTApplication>().DataContext;
            ADODB.Recordset rs = new ADODB.Recordset();
            rs = m_GTDataContext.OpenRecordset(sql, ADODB.CursorTypeEnum.adOpenStatic, ADODB.LockTypeEnum.adLockReadOnly, 1);
            if (rs.RecordCount > 0)
            {
                if (Property_G3E_FID == "")
                    Property_G3E_FID = rs.Fields[0].Value.ToString();
                else
                    Property_G3E_FID = Property_G3E_FID + "," + rs.Fields[0].Value.ToString();
            }
        }


        private void Add_Route(short iFNO, int iFID)
        {

            try
            {
                string sql = "SELECT FEATURE_TYPE FROM GC_BND_P WHERE G3E_FID =" + iFID;

                this.m_GTDataContext = GTClassFactory.Create<IGTApplication>().DataContext;
                ADODB.Recordset rs = new ADODB.Recordset();
                rs = m_GTDataContext.OpenRecordset(sql, ADODB.CursorTypeEnum.adOpenStatic, ADODB.LockTypeEnum.adLockReadOnly, 1);
                if (rs.RecordCount > 0)
                {
                    if (rs.Fields[0].Value != DBNull.Value)
                    {
                        txtServiceBndy.Text = rs.Fields[0].Value.ToString() + " - " + iFID;
                    }
                    else
                    {
                        txtServiceBndy.Text = "  " + " - " + iFID;
                        MessageBox.Show(" You are Selected Boundry Feature Type is Empty", "Property Count", MessageBoxButtons.OK, MessageBoxIcon.Information);                        
                    }
                }               

            }
            catch (Exception ex)
            {
                log.WriteLog("");
                log.WriteLog(this.GetType().FullName, "ERROR: " + ex.Message);
                log.WriteLog(ex.StackTrace);
                MessageBox.Show(ex.Message, "Property Count", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }
        

        #region Generate Property Point

        private void button2_Click(object sender, EventArgs e)
        {
            IGTPoint oPoint = GTClassFactory.Create<IGTPoint>();
            oPoint.X = Convert.ToDouble(_XPoint);
            oPoint.Y = Convert.ToDouble(_YPoint);
            GTCustomCommandModeless.m_oGTTransactionManager.Begin("Property");
            DrawPropertyPoint(30200, oPoint);
            GTCustomCommandModeless.m_oGTTransactionManager.Commit();
            GTCustomCommandModeless.m_oGTTransactionManager.RefreshDatabaseChanges();
        }

        public void DrawPropertyPoint(short iFNO, IGTPoint PointCol)
        {
            try
            {
                short iCNO;
                int iFID = 0;
                IGTKeyObject oNewFeature;
            
                IGTTextPointGeometry oTextGeom;                
                IGTOrientedPointGeometry oBoundryLine;

                oNewFeature = GTClassFactory.Create<IGTApplication>().DataContext.NewFeature(iFNO);

                iFID = oNewFeature.FID;

                #region Attributes

                iCNO = 30201;

                oNewFeature.Components.GetComponent(iCNO).Recordset.Update("PROPERTY_TYPE", 12);
                oNewFeature.Components.GetComponent(iCNO).Recordset.Update("YEAR_INSTALL", 2011);
                oNewFeature.Components.GetComponent(iCNO).Recordset.Update("QUANTITY", 5);

                #endregion

                #region Property Geometry

                //Geometry
                iCNO = 30220;

                oBoundryLine = GTClassFactory.Create<IGTOrientedPointGeometry>();
                oBoundryLine.Origin.X = PointCol.X;
                oBoundryLine.Origin.Y = PointCol.Y;
                oBoundryLine.Origin.Z = PointCol.Z;

                oBoundryLine.Origin = PointCol;

                if (oNewFeature.Components.GetComponent(iCNO).Recordset.EOF)
                {
                    oNewFeature.Components.GetComponent(iCNO).Recordset.AddNew("G3E_FID", iFID);
                    oNewFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_FNO", iFNO);
                    
                }
                else
                {
                    oNewFeature.Components.GetComponent(iCNO).Recordset.MoveLast();                    
                }


                oNewFeature.Components.GetComponent(iCNO).Geometry = oBoundryLine;

                #endregion

                #region TextGeometry


                //Text Geometry

                oTextGeom = GTClassFactory.Create<IGTTextPointGeometry>();
                IGTPoint point1 = GTClassFactory.Create<IGTPoint>();
                point1.X = PointCol.X + 2;
                point1.Y = PointCol.Y - 2;
                oTextGeom.Origin = point1;

                iCNO = 30230;
                if (oNewFeature.Components.GetComponent(iCNO).Recordset.EOF)
                {
                    oNewFeature.Components.GetComponent(iCNO).Recordset.AddNew("G3E_FID", iFID);
                    oNewFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_FNO", iFNO);
                }
                else
                {
                    oNewFeature.Components.GetComponent(iCNO).Recordset.MoveLast();
                }
                oNewFeature.Components.GetComponent(iCNO).Geometry = oTextGeom;

                #endregion

             
            }
            catch (Exception ex)
            {
                log.WriteLog("");
                log.WriteLog(this.GetType().FullName, "ERROR: " + ex.Message);
                log.WriteLog(ex.StackTrace);
                GTCustomCommandModeless.m_oGTTransactionManager.Rollback();
                MessageBox.Show(ex.Message, "Service Boundry", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
        }


        #endregion



        private void button3_Click(object sender, EventArgs e)
        {
            if (Property_G3E_FID != "")
            {
                User_Demand oPCnt = new User_Demand();
                
                string sql = "SELECT B.PL_VALUE,B.PL_CODE , A.PROPERTY_TYPE,  SUM(A.QUANTITY) FROM GC_ADM_PROPERTY A, REF_ADM_PROPTYPE B ";
                sql = sql + "  WHERE A.PROPERTY_TYPE = B.PL_NUM  AND A.G3E_FID IN ("+  Property_G3E_FID +"  ) GROUP BY A.PROPERTY_TYPE, B.PL_VALUE, B.PL_CODE ";

                this.m_GTDataContext = GTClassFactory.Create<IGTApplication>().DataContext;
                ADODB.Recordset rs = new ADODB.Recordset();
                rs = m_GTDataContext.OpenRecordset(sql, ADODB.CursorTypeEnum.adOpenStatic, ADODB.LockTypeEnum.adLockReadOnly, 1);
                if (rs.RecordCount > 0)
                {
                    rs.MoveLast();
                    do
                    {
                        oPCnt.Property_Type = rs.Fields["PROPERTY_TYPE"].Value.ToString();
                        oPCnt.Property_Type_Desc = rs.Fields["PL_VALUE"].Value.ToString();
                        oPCnt.Property_Type_No = Convert.ToInt32(rs.Fields["PL_CODE"].Value);
                        oPCnt.Property_Type_Qty = Convert.ToInt32(rs.Fields["QUANTITY"].Value);
                        
                        UpdateDP_Comp(Convert.ToInt16(lblDP_FNO.Text), Convert.ToInt32(ServBndyFID.Text), oPCnt);


                        rs.MoveNext();

                    }
                    while (!rs.EOF);
                }
            }
            else
            {
                MessageBox.Show("No Property found in DP", "Property Count", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
        }

        private void UpdateDP_Comp(short iFNO, int iFID, User_Demand oPCnt)
        {
            short iCNO;           
            IGTKeyObject oFeature;                      

            oFeature = GTClassFactory.Create<IGTApplication>().DataContext.OpenFeature(iFNO,iFID);
                        
            #region Attributes

            iCNO =0; //

            oFeature.Components.GetComponent(iCNO).Recordset.Update("","" );
            oFeature.Components.GetComponent(iCNO).Recordset.Update("","" );
            oFeature.Components.GetComponent(iCNO).Recordset.Update("","");

            #endregion


        }
    

    

    }
}