// CREATED by M.ZAM @ 04-06-2013 - Service Boundary


using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Intergraph.GTechnology.Interfaces;
using Intergraph.GTechnology.API;
using ADODB;


namespace NEPS.OSP.COPPER.SERVICE.ASSIGN
{
    class clsServBoundary
    {

        private string excABB;
        private int serviceFID;
        private short serviceFNO;
        private string serviceTable;
        private string serviceType;
        private string parentType;

        private int parentFID;

        IGTPolygonGeometry oBoundaryLine;

        public clsServBoundary(clsServicePoint sp)
        {
            this.excABB = sp.EXC_ABB;
            this.serviceFNO = sp.SERV_FNO;
            this.serviceType = sp.SERV_TYPE;
            this.serviceTable = "D" + sp.SERV_TABLE + "_S";
            this.parentFID = sp.PRT_FID;

            GetBoundaryType();
        }


        public int GetServBoundary(int srvFID)
        {
            ADODB.Recordset rs = myUtil.FindFID("GC_BNDTERM", srvFID);//gc_bndterm
            int bndFID = myUtil.ParseInt(myUtil.rsField(rs, "G3E_OWNERFID"));
            System.Diagnostics.Debug.WriteLine("Debug a");
            if (bndFID > 0)
                return bndFID;

            else // autocreate boundary FID form DP
            {
                System.Diagnostics.Debug.WriteLine("Debug b");
                GetParentBoundary(myUtil.rsField(rs, "ITFACE_CODE"), myUtil.rsField(rs, "RT_CODE"));
                System.Diagnostics.Debug.WriteLine("Debug c");
                bndFID = CreateBoundary(srvFID);
            }

            return bndFID;
        }

        private int CreateBoundary(int srvFID)
        {
            short iCNO = 0;
            short iFNO = 24000; // boundary FNO
            int iFID;
            IGTTextPointGeometry oTextGeom;

            IGTKeyObject Boundary;
            Boundary = GTClassFactory.Create<IGTApplication>().DataContext.NewFeature(iFNO);
            iFID = Boundary.FID;

            #region GC_Netelem
           
            iCNO = 51; //Netelem

            if (Boundary.Components.GetComponent(iCNO).Recordset.EOF)
            {
                Boundary.Components.GetComponent(iCNO).Recordset.AddNew("G3E_FID", iFID);
                Boundary.Components.GetComponent(iCNO).Recordset.Update("G3E_FNO", iFNO);
            }
            else
                Boundary.Components.GetComponent(iCNO).Recordset.MoveLast();
   
            Boundary.Components.GetComponent(iCNO).Recordset.Update("EXC_ABB", excABB);
            Boundary.Components.GetComponent(iCNO).Recordset.Update("JOB_STATE", "PROPOSED");
            Boundary.Components.GetComponent(iCNO).Recordset.Update("FEATURE_STATE", "PPF");
            Boundary.Components.GetComponent(iCNO).Recordset.Update("MIN_MATERIAL", "-");


            #endregion
           
            #region Attributes [GC_BND]

            iCNO = 24001;

            Boundary.Components.GetComponent(iCNO).Recordset.MoveLast();
            Boundary.Components.GetComponent(iCNO).Recordset.Update("EXC_ABB", excABB);
            Boundary.Components.GetComponent(iCNO).Recordset.Update("AREA_TYPE", "URBAN");
            Boundary.Components.GetComponent(iCNO).Recordset.Update("BND_TYPE", serviceType);
            Boundary.Components.GetComponent(iCNO).Recordset.Update("PRT_FID", parentFID);
            Boundary.Components.GetComponent(iCNO).Recordset.Update("NAME", "");

            #endregion
          
            #region Boundary Geometry [DGC_BND_P]

            iCNO = 24010;

            if (Boundary.Components.GetComponent(iCNO).Recordset.EOF)
            {
                Boundary.Components.GetComponent(iCNO).Recordset.AddNew("G3E_FID", iFID);
                Boundary.Components.GetComponent(iCNO).Recordset.Update("G3E_FNO", iFNO);
            }
            else
                Boundary.Components.GetComponent(iCNO).Recordset.MoveLast();

            Boundary.Components.GetComponent(iCNO).Recordset.Update("PRT_FID", parentFID);
            Boundary.Components.GetComponent(iCNO).Recordset.Update("FEATURE_TYPE", serviceType);
         
            CreateBoudaryPoints(srvFID);
            
            Boundary.Components.GetComponent(iCNO).Geometry = oBoundaryLine;

            #endregion

            #region TextGeometry [DGC_BND_T]
            iCNO = 24030;
            
            if (Boundary.Components.GetComponent(iCNO).Recordset.EOF)
            {
                Boundary.Components.GetComponent(iCNO).Recordset.AddNew("G3E_FID", iFID);
                Boundary.Components.GetComponent(iCNO).Recordset.Update("G3E_FNO", iFNO);
            }
            else
                Boundary.Components.GetComponent(iCNO).Recordset.MoveLast();
           
            oTextGeom = GTClassFactory.Create<IGTTextPointGeometry>();
            IGTPoint point1 = GTClassFactory.Create<IGTPoint>();
            point1.X = oBoundaryLine.Points[0].X + 2;
            point1.Y = oBoundaryLine.Points[0].Y - 2;
            oTextGeom.Origin = point1;
            Boundary.Components.GetComponent(iCNO).Geometry = oTextGeom;

            #endregion
            System.Diagnostics.Debug.WriteLine("b9 : " + Boundary + ";" + srvFID);
            CreateRelation(Boundary, srvFID);
            System.Diagnostics.Debug.WriteLine("b10");
            return Boundary.FID;
        }



        #region Boundary Points
        private void CreateBoudaryPoints(int srvFID)
        {
            string ssql = "SELECT GC_OSP_COP_VAL.EXTRACT_LINE_TO_VECTOR(G3E_GEOMETRY) AS SRV_XY FROM " +
                serviceTable + " WHERE G3E_FID = " + srvFID;

            ADODB.Recordset rs = myUtil.ADODB_ExecuteQuery(ssql);

            if (rs.EOF) throw new System.Exception("Unable to read service geometry location");

            GetGeoPoint(myUtil.rsField(rs, "SRV_XY"));

        }

        private void GetGeoPoint(string vector)
        {
            IGTPoint geopoint = GTClassFactory.Create<IGTPoint>();
            oBoundaryLine = GTClassFactory.Create<IGTPolygonGeometry>();

            string[] points = vector.Split('|');
            geopoint.X = double.Parse(points[0]) - 3;
            geopoint.Y = double.Parse(points[1]) - 3;
            oBoundaryLine.Points.Add(geopoint);

            geopoint.X = double.Parse(points[0]) - 3;
            geopoint.Y = double.Parse(points[1]) + 3;
            oBoundaryLine.Points.Add(geopoint);

            geopoint.X = double.Parse(points[0]) + 3;
            geopoint.Y = double.Parse(points[1]) + 3;
            oBoundaryLine.Points.Add(geopoint);

            geopoint.X = double.Parse(points[0]) + 3;
            geopoint.Y = double.Parse(points[1]) - 3;
            oBoundaryLine.Points.Add(geopoint);

            geopoint.X = double.Parse(points[0]) - 3;
            geopoint.Y = double.Parse(points[1]) - 3;
            oBoundaryLine.Points.Add(geopoint);

        }
        #endregion

        #region Parent Boundary

        private void GetParentBoundary(string ITFACE_CODE, string RT_CODE)
        {
            System.Diagnostics.Debug.WriteLine(ITFACE_CODE+" : "+RT_CODE);
            if (this.parentType.IndexOf("EXC") == -1)
            {
                if (ITFACE_CODE.Length > 0)
                {
                    System.Diagnostics.Debug.WriteLine("a1");
                    if (GetCabinetBoundary(ITFACE_CODE, "GC_ITFACE")) return;
                }
                else if (RT_CODE.Length > 0)
                {
                    System.Diagnostics.Debug.WriteLine("a2");
                    if (GetCabinetBoundary(RT_CODE, "GC_RT")) return;

                    if (GetCabinetBoundary(RT_CODE, "GC_MSAN")) return;

                }
            }

            GetEXCBoundary();
            System.Diagnostics.Debug.WriteLine("a3");
        }




        private bool GetCabinetBoundary(string cabCode, string cabTable)
        {
            string ssql = "SELECT G3E_OWNERFID FROM " + cabTable + " A, GC_BNDTERM B WHERE ITFACE_CODE = '" + cabCode +
                "' AND EXC_ABB = '" + excABB + "' AND A.G3E_FID = B.G3E_FID";

            ADODB.Recordset rs = myUtil.ADODB_ExecuteQuery(ssql);
            if (rs.EOF) return false;

            parentFID = myUtil.ParseInt(rs.Fields["G3E_OWNERFID"].Value.ToString());
            return true;
        }

        private bool GetEXCBoundary()
        {
            string ssql = "SELECT G3E_FID FROM GC_BND WHERE BND_TYPE = 'EXC' AND EXC_ABB = '" + excABB + "'";

            ADODB.Recordset rs = myUtil.ADODB_ExecuteQuery(ssql);
            if (rs.EOF) return false;

            parentFID = myUtil.ParseInt(rs.Fields["G3E_FID"].Value.ToString());
            return true;
        }

        #endregion

        #region Create Relationship
        private IGTApplication m_gtapp = null;

        private void CreateRelation(IGTKeyObject BNDfeature, int srvFID)
        {
            m_gtapp = GTClassFactory.Create<IGTApplication>().Application;
            
            //relationship btw bnd and dp
            IGTRelationshipService mobjRelationshipService;
            mobjRelationshipService = GTClassFactory.Create<IGTRelationshipService>(GTServiceAssign.m_oIGTCustomCommandHelper);
            mobjRelationshipService.DataContext = m_gtapp.DataContext;
            
            short mRelationshipNumber = 12;//number of relationship for dp-> boundary
            
            IGTKeyObject ServFeature = m_gtapp.DataContext.OpenFeature(serviceFNO, srvFID); // dpFNO = 13000
            mobjRelationshipService.ActiveFeature = ServFeature;
            
            if (mobjRelationshipService.AllowSilentEstablish(BNDfeature))
            {
                mobjRelationshipService.SilentEstablish(mRelationshipNumber, BNDfeature);
            }
            
            mRelationshipNumber = 16; //number of relationship for boundary ->dp                       
            mobjRelationshipService.ActiveFeature = BNDfeature;

            if (mobjRelationshipService.AllowSilentEstablish(ServFeature))
            {
                mobjRelationshipService.SilentEstablish(mRelationshipNumber, ServFeature);
            }


        }
        #endregion

        private void GetBoundaryType()
        {
            string ssql = "SELECT * FROM REF_BND_TYPE WHERE TABLE_FNO = " + this.serviceFNO.ToString();
            ADODB.Recordset rs = myUtil.ADODB_ExecuteQuery(ssql);
            if (rs.RecordCount == 1)
            {
                this.serviceType = myUtil.rsField(rs, "BND_TYPE");
                this.parentType = myUtil.rsField(rs, "PRT_BND");
            }
            else
            {
                this.parentType = "EXC";
            }
        }

    }
}
