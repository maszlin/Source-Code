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

namespace NEPS.GTechnology.Prop_Compute_Demand
{
    public partial class GTWindowsForm_Demand : Form
    {
       
        public static IGTApplication m_GeoApp;
       
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

        public GTWindowsForm_Demand()
        {
            try
            {
                InitializeComponent();               
                m_gtapp =  GTClassFactory.Create<IGTApplication>().Application;
                m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Running Propogate/Compute Demand...");

                log = Logger.getInstance();
               
            }
            catch (Exception ex)
            {
                log.WriteLog("");
                log.WriteLog(this.GetType().FullName, "ERROR: " + ex.Message);
                log.WriteLog(ex.StackTrace);            
                MessageBox.Show(ex.Message, "Propogate/Compute Demand", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }        
        

        private Intergraph.GTechnology.API.IGTApplication m_gtapp = null; 
        //IGTApplication m_gtapp = GTClassFactory.Create<IGTApplication>();


        private void GTWindowsForm_InitRoute_Shown(object sender, EventArgs e)
        {
            m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Running Propogate/Compute Demand...");            
        }

        private void GTWindowsForm_InitRoute_Activated(object sender, EventArgs e)
        {
            m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Running Propogate/Compute Demand...");            
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
                MessageBox.Show(ex.Message, "Propagate & Compute Demand", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }

        }
        private IGTDataContext m_GTDataContext = null;

    

        private void GTWindowsForm_InitRoute_Load(object sender, EventArgs e)
        {
           
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
                        FeatType.Text = rs.Fields[0].Value.ToString();
                    }
                    else
                    {
                        txtServiceBndy.Text = "  " + " - " + iFID;
                        MessageBox.Show(" You are Selected Boundry Feature Type is Empty", "Propagate & Compute Demand", MessageBoxButtons.OK, MessageBoxIcon.Information);                        
                    }
                }               

            }
            catch (Exception ex)
            {
                log.WriteLog("");
                log.WriteLog(this.GetType().FullName, "ERROR: " + ex.Message);
                log.WriteLog(ex.StackTrace);
                MessageBox.Show(ex.Message, "Propagate & Compute Demand", MessageBoxButtons.OK, MessageBoxIcon.Error);
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



       

        private void Update_Comp_C(short iFNO, int iFID, User_Demand DMD)
        {
            short iCNO;           
            IGTKeyObject oFeature;                      

            oFeature = GTClassFactory.Create<IGTApplication>().DataContext.OpenFeature(iFNO, iFID);
                        
            #region GC_DMD_TOTAL

            iCNO =24002;

            if (FeatType.Text != "BLOCK" || FeatType.Text != "EXE" ||FeatType.Text != "FDC" || FeatType.Text != "FTTZ" || FeatType.Text != "FTTS" || FeatType.Text !="CAB")
            {
                oFeature.Components.GetComponent(iCNO).Recordset.Update("SERVED_0", DMD.Served0);
                oFeature.Components.GetComponent(iCNO).Recordset.Update("SERVED_1", DMD.Served1);
                oFeature.Components.GetComponent(iCNO).Recordset.Update("SERVED_2", DMD.Served2);
                oFeature.Components.GetComponent(iCNO).Recordset.Update("SERVED_3", DMD.Served3);
                oFeature.Components.GetComponent(iCNO).Recordset.Update("SERVED_4", DMD.Served4);
                oFeature.Components.GetComponent(iCNO).Recordset.Update("SERVED_5", DMD.Served5);
                oFeature.Components.GetComponent(iCNO).Recordset.Update("SERVED_10", DMD.Served10);
                oFeature.Components.GetComponent(iCNO).Recordset.Update("SERVED_20", DMD.Served20);
            }
            
            if (FeatType.Text == "BLOCK")
            {
                oFeature.Components.GetComponent(iCNO).Recordset.Update("UNSERVED_0", DMD.Served0);
                oFeature.Components.GetComponent(iCNO).Recordset.Update("UNSERVED_1", DMD.Served1);
                oFeature.Components.GetComponent(iCNO).Recordset.Update("UNSERVED_2", DMD.Served2);
                oFeature.Components.GetComponent(iCNO).Recordset.Update("UNSERVED_3", DMD.Served3);
                oFeature.Components.GetComponent(iCNO).Recordset.Update("UNSERVED_4", DMD.Served4);
                oFeature.Components.GetComponent(iCNO).Recordset.Update("UNSERVED_5", DMD.Served5);
                oFeature.Components.GetComponent(iCNO).Recordset.Update("UNSERVED_10", DMD.Served10);
                oFeature.Components.GetComponent(iCNO).Recordset.Update("UNSERVED_20", DMD.Served20);
            }
                        
            #endregion


        }

        private void button1_Click(object sender, EventArgs e)
        {

            IGTGeometry geom = null;
            short iFNO = 0;
            int iFID = 0;


            if (GTClassFactory.Create<IGTApplication>().SelectedObjects.FeatureCount == 0)
            {
                MessageBox.Show("Please select a Feature", "Propagate & Compute Demand", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
                       
                    }
                }
            }
        }


        //Propogate Demand
        private void button2_Click_1(object sender, EventArgs e)
        {
            //Propogate Demand
            if (FeatType.Text == "EXC" || FeatType.Text == "FDC" || FeatType.Text  == "FTTZ" || FeatType.Text  == "FTTS" || FeatType.Text == "CAB")
            {

                Propogate_Demand_Calculation(Convert.ToInt32(ServBndyFID.Text));

            }
            else
            {
                MessageBox.Show("The Selected Boundry not support Propagate Demand", "Propagate & Compute Demand", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
        }


        //Compute Demand
        private void button3_Click(object sender, EventArgs e)
        {

            if (FeatType.Text  != "EXC" || FeatType.Text  != "FDC" || FeatType.Text  != "FTTZ" || FeatType.Text  != "FTTS" || FeatType.Text  != "CAB")
            {
                Compute_Demand_Calculation(Convert.ToInt32(ServBndyFID.Text));

            }
            else
            {
                MessageBox.Show("The Selected Boundry not support Compute Demand", "Propagate & Compute Demand", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
        }

        private void Compute_Demand_Calculation(int Bnd_Fid)
        {
            try
            {
                string Sql = "";
                double Served0 = 0;
                double Served1 = 0;
                double Served2 = 0;
                double Served3 = 0;
                double Served4 = 0;
                double Served5 = 0;
                double Served10 = 0;
                double Served20 = 0;

                double YEAR0 = 0;
                double YEAR1 = 0;
                double YEAR2 = 0;
                double YEAR3 = 0;
                double YEAR4 = 0;
                double YEAR5 = 0;
                double YEAR10 = 0;
                double YEAR20 = 0;

                Sql = "SELECT ";
                Sql = Sql + " PF.YEAR0, PF.YEAR1, PF.YEAR2, PF.YEAR3, PF.YEAR4, PF.YEAR5, PF.YEAR10, PF.YEAR20";
                Sql = Sql + " FROM REF_ADM_PROPTYPE REF, GC_DMD_PROPCNT B, PF_TABLE PF, GC_BND BD";
                Sql = Sql + " WHERE REF.PL_CODE = B.PROP_TYPE AND REF.PL_CODE = PF.PROP_TYPE AND PF.AREA_TYPE =BD.AREA_TYPE AND BD.G3E_FID= B.G3E_FID ";
                Sql = Sql + " AND B.G3E_FID =" + Bnd_Fid + " AND PF.EXC_ABB=BD.EXC_ABB";
                ADODB.Recordset rsPP = new ADODB.Recordset();
                rsPP = GTClassFactory.Create<IGTApplication>().DataContext.OpenRecordset(Sql, ADODB.CursorTypeEnum.adOpenStatic, ADODB.LockTypeEnum.adLockReadOnly, 1);
                if (rsPP.RecordCount > 0)
                {
                    rsPP.MoveFirst();
                    YEAR0 = Convert.ToDouble(rsPP.Fields["YEAR0"].Value);
                    YEAR1 = Convert.ToDouble(rsPP.Fields["YEAR1"].Value);
                    YEAR2 = Convert.ToDouble(rsPP.Fields["YEAR2"].Value);
                    YEAR3 = Convert.ToDouble(rsPP.Fields["YEAR3"].Value);
                    YEAR4 = Convert.ToDouble(rsPP.Fields["YEAR4"].Value);
                    YEAR5 = Convert.ToDouble(rsPP.Fields["YEAR5"].Value);
                    YEAR10 = Convert.ToDouble(rsPP.Fields["YEAR10"].Value);
                    YEAR20 = Convert.ToDouble(rsPP.Fields["YEAR20"].Value);

                }
                else
                {
                    rsPP = null;
                    Sql = "";
                    Sql = "SELECT ";
                    Sql = Sql + " PF.YEAR0, PF.YEAR1, PF.YEAR2, PF.YEAR3, PF.YEAR4, PF.YEAR5, PF.YEAR10, PF.YEAR20";
                    Sql = Sql + " FROM REF_ADM_PROPTYPE REF, GC_DMD_PROPCNT B, PF_TABLE PF, GC_BND BD";
                    Sql = Sql + " WHERE REF.PL_CODE = B.PROP_TYPE AND REF.PL_CODE = PF.PROP_TYPE AND PF.AREA_TYPE =BD.AREA_TYPE AND BD.G3E_FID= B.G3E_FID ";
                    Sql = Sql + " AND B.G3E_FID =" + Bnd_Fid + " AND PF.EXC_ABB IS NULL";
                    rsPP = GTClassFactory.Create<IGTApplication>().DataContext.OpenRecordset(Sql, ADODB.CursorTypeEnum.adOpenStatic, ADODB.LockTypeEnum.adLockReadOnly, 1);
                    if (rsPP.RecordCount > 0)
                    {
                        rsPP.MoveFirst();
                        YEAR0 = Convert.ToDouble(rsPP.Fields["YEAR0"].Value);
                        YEAR1 = Convert.ToDouble(rsPP.Fields["YEAR1"].Value);
                        YEAR2 = Convert.ToDouble(rsPP.Fields["YEAR2"].Value);
                        YEAR3 = Convert.ToDouble(rsPP.Fields["YEAR3"].Value);
                        YEAR4 = Convert.ToDouble(rsPP.Fields["YEAR4"].Value);
                        YEAR5 = Convert.ToDouble(rsPP.Fields["YEAR5"].Value);
                        YEAR10 = Convert.ToDouble(rsPP.Fields["YEAR10"].Value);
                        YEAR20 = Convert.ToDouble(rsPP.Fields["YEAR20"].Value);
                    }
                }

                Sql = "SELECT REF.PL_CODE,B.PROP_TYPE, B.BASEYEAR_0,B.BASEYEAR_1, B.BASEYEAR_2, B.BASEYEAR_3, B.BASEYEAR_4, B.BASEYEAR_5, B.BASEYEAR_10,B.BASEYEAR_20 ";
                Sql = Sql + " FROM REF_ADM_PROPTYPE REF, GC_DMD_PROPCNT B, GC_BND BD";
                Sql = Sql + " WHERE REF.PL_CODE = B.PROP_TYPE AND BD.G3E_FID= B.G3E_FID ";
                Sql = Sql + " AND B.G3E_FID =" + Bnd_Fid + " ";
                rsPP = GTClassFactory.Create<IGTApplication>().DataContext.OpenRecordset(Sql, ADODB.CursorTypeEnum.adOpenStatic, ADODB.LockTypeEnum.adLockReadOnly, 1);
                if (rsPP.RecordCount > 0)
                {
                    rsPP.MoveFirst();
                    do
                    {
                        Served0 = Served0 + Convert.ToDouble(rsPP.Fields["BASEYEAR_0"].Value) * YEAR0;
                        Served1 = Served1 + Convert.ToDouble(rsPP.Fields["BASEYEAR_1"].Value) * YEAR1;
                        Served2 = Served2 + Convert.ToDouble(rsPP.Fields["BASEYEAR_2"].Value) * YEAR2;
                        Served3 = Served3 + Convert.ToDouble(rsPP.Fields["BASEYEAR_3"].Value) * YEAR3;
                        Served4 = Served4 + Convert.ToDouble(rsPP.Fields["BASEYEAR_4"].Value) * YEAR4;
                        Served5 = Served5 + Convert.ToDouble(rsPP.Fields["BASEYEAR_5"].Value) * YEAR5;
                        Served10 = Served10 + Convert.ToDouble(rsPP.Fields["BASEYEAR_10"].Value) * YEAR10;
                        Served20 = Served20 + Convert.ToDouble(rsPP.Fields["BASEYEAR_20"].Value) * YEAR20;

                        rsPP.MoveNext();
                    } while (!rsPP.EOF);

                }
                
                
                User_Demand dmd = new User_Demand();

                dmd.Served0 = Served0;
                dmd.Served1 = Served1;
                dmd.Served2 = Served2;
                dmd.Served3 = Served3;
                dmd.Served4 = Served4;
                dmd.Served5 = Served5;
                dmd.Served10 = Served10;
                dmd.Served20 = Served20;

                GTCustomCommandModeless.m_oGTTransactionManager.Begin("Update Demand");
                Update_Comp_C(24000, Bnd_Fid, dmd);
                GTCustomCommandModeless.m_oGTTransactionManager.Commit();
                GTCustomCommandModeless.m_oGTTransactionManager.RefreshDatabaseChanges();

                MessageBox.Show("Compute Demand Total Updated..!", "Propogate/Compute Demand", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                MessageBox.Show("Error: Compute Demand Can not Calculated", "Propogate/Compute Demand", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

        }


        private void Propogate_Demand_Calculation(int bnd_fid)
        {
            try
            {
                string Sql = "";
                double Served0 = 0;
                double Served1 = 0;
                double Served2 = 0;
                double Served3 = 0;
                double Served4 = 0;
                double Served5 = 0;
                double Served10 = 0;
                double Served20 = 0;


                double UnServed0 = 0;
                double UnServed1 = 0;
                double UnServed2 = 0;
                double UnServed3 = 0;
                double UnServed4 = 0;
                double UnServed5 = 0;
                double UnServed10 = 0;
                double UnServed20 = 0;

                //string DMD_Val = Get_Value("select g3e_fid from GC_DMD_TOTAL where ");
                
                Sql = "SELECT T.G3E_FID,T.SERVED_0,T.SERVED_1,T.SERVED_2, T.SERVED_3, T.SERVED_4, T.SERVED_5, T.SERVED_10, T.SERVED_20,";
                Sql = Sql + " T.UNSERVED_0, T.UNSERVED_1, T.UNSERVED_2, T.UNSERVED_3, T.UNSERVED_4, T.UNSERVED_5, T.UNSERVED_10, T.UNSERVED_20";
                Sql = Sql + " FROM GC_DMD_TOTAL T, GC_BND_P B WHERE T.G3E_FID = B.G3E_FID AND B.PRT_FID =" + bnd_fid + "";

                ADODB.Recordset rsPP = new ADODB.Recordset();
                rsPP = GTClassFactory.Create<IGTApplication>().DataContext.OpenRecordset(Sql, ADODB.CursorTypeEnum.adOpenStatic, ADODB.LockTypeEnum.adLockReadOnly, 1);
                if (rsPP.RecordCount > 0)
                {
                    rsPP.MoveFirst();
                    do
                    {
                        if (rsPP.Fields["SERVED_0"].Value != DBNull.Value)
                            Served0 = Served0 + Convert.ToDouble(rsPP.Fields["SERVED_0"].Value);

                        if (rsPP.Fields["SERVED_1"].Value != DBNull.Value)
                            Served1 = Served1 + Convert.ToDouble(rsPP.Fields["SERVED_1"].Value);

                        if (rsPP.Fields["SERVED_2"].Value != DBNull.Value)
                            Served2 = Served2 + Convert.ToDouble(rsPP.Fields["SERVED_2"].Value);

                        if (rsPP.Fields["SERVED_3"].Value != DBNull.Value)
                            Served3 = Served3 + Convert.ToDouble(rsPP.Fields["SERVED_3"].Value);

                        if (rsPP.Fields["SERVED_4"].Value != DBNull.Value)
                            Served4 = Served4 + Convert.ToDouble(rsPP.Fields["SERVED_4"].Value);

                        if (rsPP.Fields["SERVED_5"].Value != DBNull.Value)
                            Served5 = Served5 + Convert.ToDouble(rsPP.Fields["SERVED_5"].Value);

                        if (rsPP.Fields["SERVED_10"].Value != DBNull.Value)
                            Served10 = Served10 + Convert.ToDouble(rsPP.Fields["SERVED_10"].Value);

                        if (rsPP.Fields["SERVED_20"].Value != DBNull.Value)
                            Served20 = Served20 + Convert.ToDouble(rsPP.Fields["SERVED_20"].Value);



                        if (rsPP.Fields["UNSERVED_0"].Value != DBNull.Value)
                            UnServed0 = UnServed0 + Convert.ToDouble(rsPP.Fields["UNSERVED_0"].Value);

                        if (rsPP.Fields["UNSERVED_1"].Value != DBNull.Value)
                            UnServed1 = UnServed1 + Convert.ToDouble(rsPP.Fields["UNSERVED_1"].Value);

                        if (rsPP.Fields["UNSERVED_2"].Value != DBNull.Value)
                            UnServed2 = UnServed2 + Convert.ToDouble(rsPP.Fields["UNSERVED_2"].Value);

                        if (rsPP.Fields["UNSERVED_3"].Value != DBNull.Value)
                            UnServed3 = UnServed3 + Convert.ToDouble(rsPP.Fields["UNSERVED_3"].Value);

                        if (rsPP.Fields["UNSERVED_4"].Value != DBNull.Value)
                            UnServed4 = UnServed4 + Convert.ToDouble(rsPP.Fields["UNSERVED_4"].Value);

                        if (rsPP.Fields["UNSERVED_5"].Value != DBNull.Value)
                            UnServed5 = UnServed5 + Convert.ToDouble(rsPP.Fields["UNSERVED_5"].Value);

                        if (rsPP.Fields["UNSERVED_10"].Value != DBNull.Value)
                            UnServed10 = UnServed10 + Convert.ToDouble(rsPP.Fields["UNSERVED_10"].Value);

                        if (rsPP.Fields["UNSERVED_20"].Value != DBNull.Value)
                            UnServed20 = UnServed20 + Convert.ToDouble(rsPP.Fields["UNSERVED_20"].Value);


                        rsPP.MoveNext();
                    }
                    while (!rsPP.EOF);
                }

                User_Demand dmd = new User_Demand();

                dmd.Served0 = Served0;
                dmd.Served1 = Served1;
                dmd.Served2 = Served2;
                dmd.Served3 = Served3;
                dmd.Served4 = Served4;
                dmd.Served5 = Served5;
                dmd.Served10 = Served10;
                dmd.Served20 = Served20;

                dmd.UnServed0 = UnServed0;
                dmd.UnServed1 = UnServed1;
                dmd.UnServed2 = UnServed2;
                dmd.UnServed3 = UnServed3;
                dmd.UnServed4 = UnServed4;
                dmd.UnServed5 = UnServed5;
                dmd.UnServed10 = UnServed10;
                dmd.UnServed20 = UnServed20;


                GTCustomCommandModeless.m_oGTTransactionManager.Begin("Update Demand");
                Update_Comp_P(24000, bnd_fid, dmd);
                GTCustomCommandModeless.m_oGTTransactionManager.Commit();
                GTCustomCommandModeless.m_oGTTransactionManager.RefreshDatabaseChanges();

                MessageBox.Show("Propogate Demand Total Updated..!", "Propogate/Compute Demand", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                MessageBox.Show("Error: Propogate Demand Can not Calculated", "Propogate/Compute Demand", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;

            }
            
        }

        private void Update_Comp_P(short iFNO, int iFID, User_Demand DMD)
        {
            short iCNO;
            IGTKeyObject oFeature;

            oFeature = GTClassFactory.Create<IGTApplication>().DataContext.OpenFeature(iFNO, iFID);

            #region GC_DMD_TOTAL

            iCNO = 24002;

            if (FeatType.Text == "EXC" || FeatType.Text == "FDC" || FeatType.Text == "FTTZ" || FeatType.Text == "FTTS" || FeatType.Text == "CAB")
            {
                oFeature.Components.GetComponent(iCNO).Recordset.Update("SERVED_0", DMD.Served0);
                oFeature.Components.GetComponent(iCNO).Recordset.Update("SERVED_1", DMD.Served1);
                oFeature.Components.GetComponent(iCNO).Recordset.Update("SERVED_2", DMD.Served2);
                oFeature.Components.GetComponent(iCNO).Recordset.Update("SERVED_3", DMD.Served3);
                oFeature.Components.GetComponent(iCNO).Recordset.Update("SERVED_4", DMD.Served4);
                oFeature.Components.GetComponent(iCNO).Recordset.Update("SERVED_5", DMD.Served5);
                oFeature.Components.GetComponent(iCNO).Recordset.Update("SERVED_10", DMD.Served10);
                oFeature.Components.GetComponent(iCNO).Recordset.Update("SERVED_20", DMD.Served20);  
            
                oFeature.Components.GetComponent(iCNO).Recordset.Update("UNSERVED_0", DMD.UnServed0);
                oFeature.Components.GetComponent(iCNO).Recordset.Update("UNSERVED_1", DMD.UnServed1);
                oFeature.Components.GetComponent(iCNO).Recordset.Update("UNSERVED_2", DMD.UnServed2);
                oFeature.Components.GetComponent(iCNO).Recordset.Update("UNSERVED_3", DMD.UnServed3);
                oFeature.Components.GetComponent(iCNO).Recordset.Update("UNSERVED_4", DMD.UnServed4);
                oFeature.Components.GetComponent(iCNO).Recordset.Update("UNSERVED_5", DMD.UnServed5);
                oFeature.Components.GetComponent(iCNO).Recordset.Update("UNSERVED_10", DMD.UnServed10);
                oFeature.Components.GetComponent(iCNO).Recordset.Update("UNSERVED_20", DMD.UnServed20);
            }



            #endregion


        }

        private void button4_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}