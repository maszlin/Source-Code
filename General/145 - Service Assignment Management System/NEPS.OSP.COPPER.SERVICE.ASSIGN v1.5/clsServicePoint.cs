using System;
using System.Collections.Generic;
using System.Text;
using Intergraph.GTechnology.Interfaces;
using Intergraph.GTechnology.API;
using ADODB;
using System.Windows.Forms;

namespace NEPS.OSP.COPPER.SERVICE.ASSIGN
{
    class clsServicePoint
    {
        private string reftable = "AG_REF_SERV_ASSIGNMENT";
        private string service_type;
        private string lbl_servcode;
        private string lbl_servname;
        private short srv_FNO = 13000;
        private short srv_CNO = 13002;
        private string srv_table;

        private string prt_type;
        private int prt_FID;
        private short prt_FNO;
        private string prt_table; //GC_ITFACE
        private string prt_code_col;
        private string prt_code;

        private string exc_abb;

        public string SERV_TABLE
        {
            get { return srv_table; }
        }
        public string SERV_TYPE
        {
            get { return service_type; }
        }
        public string SERV_CODE_LBL
        {
            get { return lbl_servcode; }
        }
        public string SERV_NAME_LBL
        {
            get { return lbl_servname; }
        }
        public short SERV_FNO
        {
            get { return srv_FNO; }
        }
        public short SERV_CNO
        {
            get { return srv_CNO; }
        }
        public int PRT_FID
        {
            get { return prt_FID; }
        }
        public string PRT_TYPE
        {
            get { return prt_type; }
        }
        public string PRT_CODE
        {
            get { return prt_code; }
        }
        public string EXC_ABB
        {
            get { return exc_abb; }
        }

        public bool ValidServicePoint(short FNO, int FID)
        {
            //if (oDDCKeyObject.FNO == 13100 || oDDCKeyObject.FNO == 10300 || oDDCKeyObject.FNO == 9800)

            /*if (FNO == 10300)//pcab
            {
                prt_FNO = FNO;
                prt_FID = FID;
                prt_code_col = "ITFACE_CODE";
                prt_table = "GC_ITFACE";
                //prt_code = myUtil.rsField(rs, "PARENT_CODE");

                return true;
            }*/
            ADODB.Recordset rs = myUtil.FindRecord(reftable, "PARENT_FNO = " + FNO);
            if (rs.RecordCount == 0)
                return false;
            else
            {
                prt_type = myUtil.rsField(rs, "PARENT_TYPE");
                prt_code_col = myUtil.rsField(rs, "PARENT_CODE_COL");
                prt_table = myUtil.rsField(rs, "PARENT_TABLE");

                service_type = myUtil.rsField(rs, "SERV_TYPE");
                lbl_servcode = myUtil.rsField(rs, "SERV_CODE_LBL");
                lbl_servname = myUtil.rsField(rs, "SERV_NAME_LBL");

                srv_FNO = short.Parse(myUtil.rsField(rs, "SERV_FNO"));
                srv_CNO = short.Parse(myUtil.rsField(rs, "SERV_CNO"));
                srv_table = myUtil.rsField(rs, "SERV_TABLE");

                prt_FNO = FNO;
                prt_FID = FID;               
                return true;
            }
        }

        public bool GetParentCode()
        {
            string strSQL = "SELECT A.{2} SERVICE_CODE, " + (prt_FNO == 10300 ? "A.ITFACE_CLASS" : prt_type) +
                " SERVICE_TYPE, B.EXC_ABB FROM {1} A, GC_NETELEM B" +
                " WHERE A.G3E_FID = B.G3E_FID AND A.G3E_FID = {0}";


            ADODB.Recordset rs = myUtil.ADODB_ExecuteQuery(
                string.Format(strSQL, prt_FID, prt_table, prt_code_col));

            if (!rs.EOF)
            {
                prt_code = myUtil.rsField(rs, "SERVICE_CODE");
                prt_type = myUtil.rsField(rs, "SERVICE_TYPE");
                exc_abb = myUtil.rsField(rs, "EXC_ABB");
                return true;
            }
            else
                return false;
        }

        public void GetServices(DataGridView dg)
        {
            System.Diagnostics.Debug.WriteLine("service_type : " + service_type);
            System.Diagnostics.Debug.WriteLine("lbl_servcode : " + lbl_servcode);
            System.Diagnostics.Debug.WriteLine("srv_table : " + srv_table);
            System.Diagnostics.Debug.WriteLine("prt_FID : " + prt_FID);
            string strSQL = "";
            Int32 i = 0;
            //
            // Load DP
            dg.Rows.Clear();
            i = 0;
            strSQL = "SELECT A.G3E_FID, '" + service_type + " '||A." + lbl_servcode + " SRV_CODE, D.FLOOR_NO, ";
            strSQL += "(SELECT COUNT(*) FROM GC_DP_ASSIGNMENT C WHERE C.G3E_FID = A.G3E_FID) UNIT_COUNT, ";
            strSQL += "(SELECT G3E_OWNERFID FROM GC_BNDTERM E WHERE E.G3E_FID = A.G3E_FID) BND_FID ";
            strSQL += "FROM " + srv_table + " A, D" + srv_table + "_S B, GC_ADDRESS D ";
            strSQL += "WHERE A.G3E_FID = B.G3E_FID AND A.G3E_FID = D.G3E_FID AND B.G3E_DETAILID = ";
            strSQL += "(SELECT G3E_DETAILID FROM GC_DETAIL WHERE G3E_FID = " + prt_FID + " AND ROWNUM = 1) ";
            strSQL += "ORDER BY G3E_FID";

            System.Diagnostics.Debug.WriteLine("SQL : " + strSQL);
            
            ADODB.Recordset rs = myUtil.ADODB_ExecuteQuery(strSQL);
            if (rs.RecordCount == 0)
                throw new System.Exception("Selected service point is empty");

            do
            {
                dg.Rows.Add();
                dg.Rows[i].Cells["colSPFID"].Value = myUtil.rsField(rs, "G3E_FID");
                dg.Rows[i].Cells["colSPCode"].Value = myUtil.rsField(rs, "SRV_CODE");
                dg.Rows[i].Cells["colSPUnitCnt"].Value = myUtil.rsField(rs, "UNIT_COUNT");
                dg.Rows[i].Cells["colSPBndFID"].Value = myUtil.rsField(rs, "BND_FID");
                dg.Rows[i].Cells["colSPFloor"].Value = myUtil.rsField(rs, "FLOOR_NO");
                dg.Rows[i].Cells["colSPUpdate"].Value = 1; // already updated - read from db
                i++;
                rs.MoveNext();
            }
            while (!rs.EOF);

            dg.Sort(new DataGridColumnSorter(1, SortOrder.Ascending));
            dg.Columns[1].HeaderCell.SortGlyphDirection = SortOrder.Ascending;
            dg.Rows[0].Selected = true;
        }

    }
}
