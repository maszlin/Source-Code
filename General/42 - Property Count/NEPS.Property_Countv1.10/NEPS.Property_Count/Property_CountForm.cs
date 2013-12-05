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

namespace NEPS.GTechnology.Property_Count
{
    public partial class GTWindowsForm_Property_Count : Form
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

        public GTWindowsForm_Property_Count()
        {
            try
            {
                InitializeComponent();
                m_gtapp = GTClassFactory.Create<IGTApplication>().Application;
                m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Running Property Count...");

                log = Logger.getInstance();

            }
            catch (Exception ex)
            {
                log.WriteLog("");
                log.WriteLog(this.GetType().FullName, "ERROR: " + ex.Message);
                log.WriteLog(ex.StackTrace);
                MessageBox.Show(ex.Message, "Property Count", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private Intergraph.GTechnology.API.IGTApplication m_gtapp = null;
        //IGTApplication m_gtapp = GTClassFactory.Create<IGTApplication>();


        private void GTWindowsForm_InitRoute_Shown(object sender, EventArgs e)
        {
            m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Running Property Count...");
        }

        private void GTWindowsForm_InitRoute_Activated(object sender, EventArgs e)
        {
            m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Running Property Count...");
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
            rbAllBoundaries.Text = "All boundaries in " + Utility.CurrentJobName();
        }

        bool clicked = false;
        private void button1_Click(object sender, EventArgs e)
        {
            txtServiceBndy.Text = "";

            if (GTClassFactory.Create<IGTApplication>().SelectedObjects.FeatureCount == 0)
            {
                MessageBox.Show("Please select a Feature", "Property Count", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            IGTDDCKeyObjects ddcKeyObjects = GTClassFactory.Create<IGTApplication>().SelectedObjects.GetObjects();
            SetActiveBoundary(ddcKeyObjects);
        }

        /// <summary>
        /// Pick a boundary from ddcKeyObjects and make it the active boundary
        /// </summary>
        /// <param name="ddcKeyObjects"></param>
        public void SetActiveBoundary(IGTDDCKeyObjects ddcKeyObjects)
        {
            // process only in single boundary mode
            if (!rbSingleBoundary.Checked)
            {
                rbSingleBoundary.Checked = true;
            }


            foreach (IGTDDCKeyObject oDDCKeyObject in ddcKeyObjects)
            {
               
                if (oDDCKeyObject.ComponentViewName == "VGC_BND_P")
                {
                    short iFNO = oDDCKeyObject.FNO;
                    int iFID = oDDCKeyObject.FID;

                    if (!exchange_validation(iFID))
                    {

                        if (iFNO == 24000)
                        {
                            if (!SetActiveBoundary(iFNO, iFID, true))
                                return;
                        }
                    }
                    else
                    {

                        MessageBox.Show("Not allowed to update Property Count for Exchange Boundary", "Property Count", MessageBoxButtons.OK, MessageBoxIcon.Information);
             
                    }
                }
            }
        }

        private bool SetActiveBoundary(short iFNO, int iFID, bool promptIfNoProperties)
        {
            Add_Route(iFNO, iFID);
            ServBndyFID.Text = iFID.ToString();
            lblDP_FNO.Text = iFNO.ToString();
            Get_DP_Property(iFID);
            clicked = true;

            if (Property_G3E_FID == "")
            {
                if (promptIfNoProperties)
                {
                    MessageBox.Show("No Admin Property found in the selected Boundary", "Property Count", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                
                txtServiceBndy.Text = "";
                return false;
            }

            return true;
        }

        string Property_G3E_FID = "";

        private void Get_DP_Property(int iFID)
        {
            //string sql = "select b.g3e_fid from gc_bnd_p a, gc_adm_property_s b  where a.g3e_fid = "+ iFID +" and sdo_inside ( b.g3e_geometry, a.g3e_geometry ) = 'TRUE'";

            //this.m_GTDataContext = GTClassFactory.Create<IGTApplication>().DataContext;
            //ADODB.Recordset rs = new ADODB.Recordset();
            //rs = m_GTDataContext.OpenRecordset(sql, ADODB.CursorTypeEnum.adOpenStatic, ADODB.LockTypeEnum.adLockReadOnly, 1);
            //if (rs.RecordCount > 0)
            //{
            //    rs.MoveFirst();
            //    do
            //    {
            //        if (Property_G3E_FID == "")
            //            Property_G3E_FID = rs.Fields[0].Value.ToString();
            //        else
            //            Property_G3E_FID = Property_G3E_FID + "," + rs.Fields[0].Value.ToString();

            //        rs.MoveNext();

            //    }
            //    while (!rs.EOF);
            //}

            //GetFeatureTypes();

            List<RegenItem> featuresContained = GetFeaturesContained(iFID);
            Property_G3E_FID = "";
            bool isFirst = true;
            foreach (RegenItem feature in featuresContained)
            {
                if (!isFirst)
                    Property_G3E_FID += ",";
                Property_G3E_FID += feature.FID.ToString();
                isFirst = false;
            }

        }

        List<FeatureType> featureTypes = new List<FeatureType>();

        private void GetFeatureTypes()
        {
            IGTApplication app = GTClassFactory.Create<IGTApplication>();
            string sql = "SELECT * FROM G3E_FEATURE ORDER BY G3E_USERNAME";

            int count = 0;
            Recordset rs = app.DataContext.Execute(sql, out count, 0, null);
            if (!rs.EOF)
            {
                rs.MoveFirst();
                while (!rs.EOF)
                {
                    FeatureType item = new FeatureType();
                    item.FNO = Convert.ToInt16(rs.Fields["G3E_FNO"].Value);
                    item.Username = rs.Fields["G3E_USERNAME"].Value.ToString();

                    rs.MoveNext();
                    featureTypes.Add(item);
                }
            }
        }

        public class FeatureType
        {
            public short FNO;
            public string Username;
            public override string ToString()
            {
                return string.Format("{0} ({1})", Username, FNO);
            }
        }

        class RegenItem
        {
            public int FID;
            public short FNO;
            public IGTPoint firstPoint;
            public IGTPolygonGeometry poly;
        }


        private List<RegenItem> GetFeaturesContained(int iFID)
        {

            List<RegenItem> output = new List<RegenItem>();
            IGTKeyObject objBoundary = GTClassFactory.Create<IGTApplication>().DataContext.OpenFeature(24000, iFID);
            IGTComponent compBoundary = objBoundary.Components.GetComponent(24010);
            IGTGeometry geometryBoundary = compBoundary.Geometry;

            // initialize spatial service
            IGTSpatialService oSS = GTClassFactory.Create<IGTSpatialService>();
            IGTKeyObjects oKO = GTClassFactory.Create<IGTKeyObjects>();
            IGTKeyObjects oFG = GTClassFactory.Create<IGTKeyObjects>();
            ADODB.Recordset rsAOI = new ADODB.Recordset();
            oSS.DataContext = GTClassFactory.Create<IGTApplication>().DataContext;
            oSS.FilterGeometry = geometryBoundary;
            oSS.Operator = GTSpatialOperatorConstants.gtsoTouches;

            List<short> lstFNO = new List<short>();
            //foreach (FeatureType featureType in featureTypes)
            // {
            //lstFNO.Add(featureType.FNO);
            lstFNO.Add(30200); //GC_ADM_PROPERTY_S
            // }

            rsAOI = oSS.GetResultsByFNO(lstFNO.ToArray());
            if (!rsAOI.EOF)
            {
                rsAOI.MoveFirst();
                while (!rsAOI.EOF)
                {
                    RegenItem item = new RegenItem();
                    item.FNO = Convert.ToInt16(rsAOI.Fields["G3E_FNO"].Value);
                    item.FID = Convert.ToInt32(rsAOI.Fields["G3E_FID"].Value);

                    // skip boundaries within boundaries
                    if (item.FNO == 24000 || item.FNO == 8000)
                    {
                        rsAOI.MoveNext();
                        continue;
                    }
                    output.Add(item);
                    rsAOI.MoveNext();
                }
            }
            return output;
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
                        MessageBox.Show(" You are Selected Boundary Feature Type is Empty", "Property Count", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
                MessageBox.Show(ex.Message, "Service Boundary", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
        }


        #endregion



        private void button3_Click(object sender, EventArgs e)
        {
            button3.Enabled = false;
            button4.Enabled = false;

            if (rbSingleBoundary.Checked)
            {
                lblStatus.Text = "Processing...";
                ProcessActiveBoundary();
            }
            else
            {
                List<int> FIDBoundaries = Utility.GetAllBoundaryFIDs();
                int i = 1;
                foreach (int FID in FIDBoundaries)
                {
                    if (!exchange_validation(FID))
                    {
                        lblStatus.Text = "Progress: " + i + "/" + FIDBoundaries.Count;
                        SetActiveBoundary(Utility.FNO_BOUNDARY, FID, false);
                        ProcessActiveBoundary();
                    }
                    else
                    {
                        MessageBox.Show("Not allowed to update Property Count for Exchange Boundary. This process will skip Exchange Boundary in this Job.", "Property Count", MessageBoxButtons.OK, MessageBoxIcon.Information);
                
                    }
                    i++;                 
                }
            }

            button3.Enabled = true;
            button4.Enabled = true;
            lblStatus.Text = "Finished processing.";
        }

        private void ProcessActiveBoundary()
        {
            short iCNO;
            short iFNO;
            int iFID;
            IGTKeyObject oFeature;

            bool Update = false;

            if (clicked == false)
            {
                MessageBox.Show("No Boundary found.", "Property Count", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            IGTApplication app = GTClassFactory.Create<IGTApplication>();
            app.BeginWaitCursor();

            if (Property_G3E_FID != "")
            {
                iFNO = Convert.ToInt16(lblDP_FNO.Text);
                iFID = Convert.ToInt32(ServBndyFID.Text);

                //Delete Exisiting Prop Count 
                Delete_Prop_Count(Convert.ToInt32(ServBndyFID.Text));

                PropertyCount oPCnt = new PropertyCount();

                //Update 24003  GC_DMD_PROPCNT
                string sql = "SELECT B.PL_VALUE,B.PL_CODE , A.PROPERTY_TYPE,  SUM(A.QUANTITY) AS QTY  FROM GC_ADM_PROPERTY A, REF_ADM_PROPTYPE B ";
                sql = sql + "  WHERE A.PROPERTY_TYPE = B.PL_CODE  AND A.G3E_FID IN (" + Property_G3E_FID + "  ) GROUP BY A.PROPERTY_TYPE, B.PL_VALUE, B.PL_CODE ";

                this.m_GTDataContext = GTClassFactory.Create<IGTApplication>().DataContext;
                ADODB.Recordset rs = new ADODB.Recordset();
                rs = m_GTDataContext.OpenRecordset(sql, ADODB.CursorTypeEnum.adOpenStatic, ADODB.LockTypeEnum.adLockReadOnly, 1);
                if (rs.RecordCount > 0)
                {
                    int i = 1;
                    rs.MoveFirst();
                    do
                    {
                        oPCnt.Property_Type = rs.Fields["PL_CODE"].Value.ToString();
                        //oPCnt.Property_Type = rs.Fields["PROPERTY_TYPE"].Value.ToString();
                        oPCnt.Property_Type_Desc = rs.Fields["PL_VALUE"].Value.ToString();
                        //oPCnt.Property_Type_No = rs.Fields["PL_CODE"].Value;
                        oPCnt.Property_Type_Qty = Convert.ToInt32(rs.Fields["QTY"].Value);

                        if (oPCnt.Property_Type_Qty < 0) // because we see strange -9999s
                            oPCnt.Property_Type_Qty = 0;

                        UpdateDP_Comp(iFNO, iFID, oPCnt, i);

                        i++; // For CID increment.
                        rs.MoveNext();
                    }
                    while (!rs.EOF);
                    Update = true;
                }

                //Update 24004 GC_SERVICEADDRESS
                sql = "SELECT P.OBJECTID, P.LOT_NUMBER, P.HOUSE_NUMBER, P.PROPERTY_TYPE, S.STREET_NAME, S.STREET_TYPE, S.SECTION_NAME, S.POSTAL_NUM, S.CITY_NAME, S.STATE_CODE  FROM GC_ADM_PROPERTY P, GC_ADM_STREET S WHERE  P.STREET_ID = S.OBJECTID AND P.G3E_FID IN (" + Property_G3E_FID + "  )and P.SEGMENT = S.SEGMENT";
                System.Diagnostics.Debug.WriteLine(sql);
                
                rs = m_GTDataContext.OpenRecordset(sql, ADODB.CursorTypeEnum.adOpenStatic, ADODB.LockTypeEnum.adLockReadOnly, 1);
                bool hasRecords = false;
                if (rs.RecordCount > 0)
                {
                    int i = 1;
                    GTCustomCommandModeless.m_oGTTransactionManager.Begin("AddressUpdate");

                    oFeature = GTClassFactory.Create<IGTApplication>().DataContext.OpenFeature(iFNO, iFID);
                    iCNO = 24004;

                    rs.MoveFirst();
                    do
                    {
                        hasRecords = true;
                        oFeature.Components.GetComponent(iCNO).Recordset.AddNew("G3E_FID", iFID);
                        oFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_FNO", iFNO);
                        oFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_CID", i);
                        oFeature.Components.GetComponent(iCNO).Recordset.Update("OBJECTID", rs.Fields["OBJECTID"].Value.ToString());
                        oFeature.Components.GetComponent(iCNO).Recordset.Update("LOT_NUMBER", rs.Fields["LOT_NUMBER"].Value.ToString());
                        oFeature.Components.GetComponent(iCNO).Recordset.Update("HOUSE_NUMBER", rs.Fields["HOUSE_NUMBER"].Value.ToString());
                        oFeature.Components.GetComponent(iCNO).Recordset.Update("PROPERTY_TYPE", rs.Fields["PROPERTY_TYPE"].Value.ToString());
                        oFeature.Components.GetComponent(iCNO).Recordset.Update("STREET_NAME", rs.Fields["STREET_NAME"].Value.ToString());
                        oFeature.Components.GetComponent(iCNO).Recordset.Update("STREET_TYPE", rs.Fields["STREET_TYPE"].Value.ToString());
                        oFeature.Components.GetComponent(iCNO).Recordset.Update("SEC_NAME", rs.Fields["SECTION_NAME"].Value.ToString());
                        oFeature.Components.GetComponent(iCNO).Recordset.Update("POSTAL_CODE", rs.Fields["POSTAL_NUM"].Value.ToString());
                        oFeature.Components.GetComponent(iCNO).Recordset.Update("CITY", rs.Fields["CITY_NAME"].Value.ToString());
                        oFeature.Components.GetComponent(iCNO).Recordset.Update("STATE_CODE", rs.Fields["STATE_CODE"].Value.ToString());

                        i++; // For CID increment.
                        rs.MoveNext();
                    }
                    while (!rs.EOF);
                    Update = true;
                    GTCustomCommandModeless.m_oGTTransactionManager.Commit();
                }

            }
            else
            {
                MessageBox.Show("No Property Found.", "Property Count", MessageBoxButtons.OK, MessageBoxIcon.Information);
                app.EndWaitCursor();
                return;
            }
            app.EndWaitCursor();
        }

        private void Delete_Prop_Count(int iFID)
        {
            string strSQL = null;
            int iRecordNum = 0;
            object[] obj = null;

            GTCustomCommandModeless.m_oGTTransactionManager.Begin("DeletePropertyCount");
            strSQL = "Delete from GC_DMD_PROPCNT where g3e_fid = " + iFID + "";
            GTClassFactory.Create<IGTApplication>().DataContext.Execute(strSQL, out iRecordNum, (int)ADODB.CommandTypeEnum.adCmdText, obj);
            GTCustomCommandModeless.m_oGTTransactionManager.Commit();

            GTCustomCommandModeless.m_oGTTransactionManager.Begin("DeleteAddress");
            strSQL = "Delete from GC_SERVICEADDRESS where g3e_fid = " + iFID + "";
            GTClassFactory.Create<IGTApplication>().DataContext.Execute(strSQL, out iRecordNum, (int)ADODB.CommandTypeEnum.adCmdText, obj);
            GTCustomCommandModeless.m_oGTTransactionManager.Commit();

        }

        private void UpdateDP_Comp(short iFNO, int iFID, PropertyCount oPCnt, int cid)
        {


            int Qty = 0;
            try
            {
                short iCNO;
                IGTKeyObject oFeature;

                GTCustomCommandModeless.m_oGTTransactionManager.Begin("PropertyCount");
                oFeature = GTClassFactory.Create<IGTApplication>().DataContext.OpenFeature(iFNO, iFID);

                iCNO = 24003; //GC_DMD_PROPCNT
                Qty = oPCnt.Property_Type_Qty;


                oFeature.Components.GetComponent(iCNO).Recordset.AddNew("G3E_FID", iFID);
                oFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_FNO", iFNO);
                oFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_CID", cid);
                oFeature.Components.GetComponent(iCNO).Recordset.Update("PROP_TYPE", oPCnt.Property_Type);
                oFeature.Components.GetComponent(iCNO).Recordset.Update("BASEYEAR_0", Qty);
                oFeature.Components.GetComponent(iCNO).Recordset.Update("BASEYEAR_1", Qty);
                oFeature.Components.GetComponent(iCNO).Recordset.Update("BASEYEAR_2", Qty);
                oFeature.Components.GetComponent(iCNO).Recordset.Update("BASEYEAR_3", Qty);
                oFeature.Components.GetComponent(iCNO).Recordset.Update("BASEYEAR_4", Qty);
                oFeature.Components.GetComponent(iCNO).Recordset.Update("BASEYEAR_5", Qty);
                oFeature.Components.GetComponent(iCNO).Recordset.Update("BASEYEAR_10", Qty);
                oFeature.Components.GetComponent(iCNO).Recordset.Update("BASEYEAR_20", Qty);
                GTCustomCommandModeless.m_oGTTransactionManager.Commit();
            }
            catch (Exception ex)
            {
                string errorMessage = "Error while updating: " + ex.Message +
                    string.Format(". iFID {0}, iFNO {1}, cid {2}, prop_type {3}, qty {4}",
                    iFID, iFNO, oPCnt.Property_Type, Qty);
                MessageBox.Show(errorMessage, "Exception in UpdateDP_Comp");
            }






        }

        private void button4_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void rbSingleBoundary_CheckedChanged(object sender, EventArgs e)
        {
            groupBox2.Enabled = rbSingleBoundary.Checked;
        }

        private void btnPickSelected_Click(object sender, EventArgs e)
        {
            IGTApplication app = GTClassFactory.Create<IGTApplication>();
            SetActiveBoundary(app.SelectedObjects.GetObjects());
        }

        public Boolean exchange_validation(int iFID)
        {
            IGTApplication app = GTClassFactory.Create<IGTApplication>();
            string sql = "SELECT G3E_FNO FROM GC_BNDTERM WHERE G3E_OWNERFID = " + iFID + " and G3E_FNO = 6000";

            int count = 0;
            Recordset rs = app.DataContext.Execute(sql, out count, 0, null);
            if (rs.RecordCount > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

    }
}