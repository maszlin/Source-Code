/*
* NEPS.OSP.COPPER.CABLE_TRANSFER_POST
* class name : clsBoundary
* 
* develop by : m.zam 
* started : 20-MAY-2013
*
* to find and update service boundary 
* 
*/

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;

using Intergraph.GTechnology.Interfaces;
using Intergraph.GTechnology.API;

using ADODB;


namespace NEPS.OSP.COPPER.CABLE_TRANSFER_POST
{
    class clsBoundary
    {
        private int parentBoundaryFID;
        public clsBoundary(int cabFID)
        {
            parentBoundaryFID = GetCabinetBoundary(cabFID);
        }

        public void UpdateParentBoundary(short oFNO, int oFID)
        {
            string tablename = GetComponentTable((short)(oFNO + 1));

            int boundaryFID = GetBoundaryFID(tablename, oFID);
            if (boundaryFID > -1)
            {
                myUtil.ADODB_ExecuteNonQuery(
                    "UPDATE GC_BND SET PRT_FID = " + parentBoundaryFID + " WHERE G3E_FID = " + boundaryFID);
            }
        }


        private int GetCabinetBoundary(int cabFID)
        {
            string ssql = "SELECT BND_FID FROM GC_ITFACE WHERE G3E_FID = " + cabFID;
            ssql += " UNION SELECT BND_FID FROM GC_RT WHERE G3E_FID = " + cabFID;
            ssql += " UNION SELECT BND_FID FROM GC_MSAN WHERE G3E_FID = " + cabFID;
            ssql += " UNION SELECT BND_FID FROM GC_VDSL2 WHERE G3E_FID = " + cabFID;

            ADODB.Recordset rs = new ADODB.Recordset();
            rs = myUtil.ADODB_ExecuteQuery(ssql);

            if (rs.RecordCount > 0)
                return int.Parse(rs.Fields[0].Value.ToString());

            else
                return -1;
        }

        private static int GetBoundaryFID(string component_table, int component_FID)
        {
            string ssql = "SELECT BND_FID FROM " + component_table + " WHERE G3E_FID = " + component_FID;

            ADODB.Recordset rs = new ADODB.Recordset();
            rs = myUtil.ADODB_ExecuteQuery(ssql);

            if (rs.RecordCount > 0)
                return int.Parse(rs.Fields[0].Value.ToString());

            else
                return -1;

        }

        private static string GetComponentTable(short CNO)
        {
            ADODB.Recordset rs = new ADODB.Recordset();
            rs = myUtil.ADODB_ExecuteQuery("SELECT G3E_TABLE FROM G3E_COMPONENT WHERE G3E_CNO = " + CNO);

            return rs.Fields[0].Value.ToString();
        }


    }
}
