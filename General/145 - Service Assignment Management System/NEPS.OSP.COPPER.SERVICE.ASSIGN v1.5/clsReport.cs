using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Intergraph.GTechnology.Interfaces;
using Intergraph.GTechnology.API;
using ADODB;

namespace NEPS.OSP.COPPER.SERVICE.ASSIGN
{
    class clsReport
    {
        public static string segment;
        public static string buildingid;
        public static string buildingname;
        public static string buildingfloor;
        public static string exc_abb;
        public static string service_type;
        public static string service_code;
        public static string address;


        public static Logger log;

        public static string CreateReport(DataGridView dg)
        {
            string filename = "Service Assignment [" + DateTime.Now.ToString("yyMMdd HHmmss") + "].txt";
            log = Logger.getInstance();
            log.OpenFile(filename);
            log.WriteLog("********************************************");
            log.WriteLog("\t\tService Assignment"); // transfer e-side or d-side
            log.WriteLog("********************************************");
            log.WriteLog("Building Information");
            log.WriteLog("\tBuilding Name    : " + buildingname);
            log.WriteLog("\tNumber of Floor  : " + buildingfloor);
            log.WriteLog("\tBuilding Address : " + address);
            log.WriteLog("\tReport Date      : " + DateTime.Now.ToString("dd MMM yyyy"));
            log.WriteLog("");
            log.WriteLog("Service Point");
            log.WriteLog("\t" + "Exchange : " + exc_abb);
            log.WriteLog("\t" + "Service Type : " + service_type );
            log.WriteLog("\t" + "Service Code : " + service_code);
            log.WriteLog("");
            
            log.WriteLog("");
            log.WriteLog("Service Assignment");
            log.WriteLog("");
            foreach (DataGridViewRow r in dg.Rows)
            {
                ReportAssignment(r);
            }
            string tempDir = System.Environment.GetEnvironmentVariable("TEMP");
            return tempDir + "\\" + filename;
        }

        private static void ReportAssignment(DataGridViewRow r)
        {
            string SRV_FID = r.Cells["colSPFID"].Value.ToString();
            string SRV_NUM = r.Cells["colSPCode"].Value.ToString();
            string ssql = "SELECT A.G3E_FID, B.FLOOR_NUM, B.APT_NUM FROM GC_DP_ASSIGNMENT A, GC_ADM_ADDR B ";
            ssql += "WHERE A.G3E_FID = " + SRV_FID + " ";
            ssql += "AND A.OBJECTID = B.OBJECTID AND A.SEGMENT_GDS = B.SEGMENT ";
            ssql += "ORDER BY FLOOR_NUM, APT_NUM";

            ADODB.Recordset rs = myUtil.ADODB_ExecuteQuery(ssql);
            clsServicePoint sp = GTServiceAssign.m_Service;

            if (rs.EOF)
            {
                log.WriteLog("");
                log.WriteLog(sp.SERV_CODE_LBL + " : " + SRV_NUM + "\t- NO UNIT ASSIGNED TO THIS SERVICE");
                return;
            }

            string floor_num = "";
            log.WriteLog("");
            log.WriteLog(sp.SERV_CODE_LBL + "  : " + SRV_NUM);
            while (!rs.EOF)
            {
                if (floor_num != rs.Fields["FLOOR_NUM"].Value.ToString())
                {
                    floor_num = rs.Fields["FLOOR_NUM"].Value.ToString();
                    log.WriteLog("");
                    log.WriteLog("\tFLOOR NUMBER " + floor_num);
                    log.WriteLog("\t------------------");
                }
                log.WriteLog("\tUNIT " + rs.Fields["APT_NUM"].Value.ToString());
                rs.MoveNext();
            }
        }


    }
}
