using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Intergraph.GTechnology.Interfaces;
using Intergraph.GTechnology.API;
using ADODB;

namespace NEPS.OSP.COPPER.SERVICE.ASSIGN
{
    class clsServAssignment
    {
        private string segmentGDS;
        private string buildingID;
        private int srvFNO;
        private short srvCNO;

        public clsServAssignment(string building_id, string segment_gds, int FNO, short CNO)
        {
            segmentGDS = segment_gds;
            buildingID = building_id;
            srvFNO = FNO;
            srvCNO = CNO;
        }

        public void SaveServiceAssignment(DataGridViewRow r)
        {
            // get next G3E_CID
            int vCID = 1;
            int srvFID = myUtil.ParseInt(r.Cells["colSvrFID"].Value.ToString());
            int objID = myUtil.ParseInt(r.Cells["colSvrAptID"].Value.ToString());
            string objAddr = r.Cells["colSvrAptNum"].Value.ToString();

            if (RecordExist(objID, srvFID, objAddr)) return;

            string ssql = "SELECT DECODE(MAX(G3E_CID), NULL, 1, MAX(G3E_CID) + 1) LAST_CID FROM GC_DP_ASSIGNMENT WHERE G3E_FID = " + srvFID;
            ADODB.Recordset rsChkCID = myUtil.ADODB_ExecuteQuery(ssql);
            if (rsChkCID.RecordCount > 0)
            {
                vCID = myUtil.ParseInt(rsChkCID.Fields[0].Value.ToString());
            }

            ssql = "INSERT INTO GC_DP_ASSIGNMENT (G3E_ID, G3E_FID, G3E_FNO, G3E_CNO, G3E_CID, OBJECTID, SEGMENT_GDS) " +
                    "VALUES (GC_DP_ASSIGNMENT_SEQ.NEXTVAL," + srvFID + "," + srvFNO + "," + srvCNO + "," + vCID + ","
                    + objID + ", '" + segmentGDS + "')"; 
            myUtil.ADODB_ExecuteNonQuery(ssql);

        }

        private bool RecordExist(int aptID, int srvFID, string aptADDR)
        {
            ADODB.Recordset rs = myUtil.FindRecord("GC_DP_ASSIGNMENT", "OBJECTID = " + aptID);
            if (rs.RecordCount == 0)
                return false;

            else if (myUtil.rsField(rs, "G3E_FID") != srvFID.ToString())
            {
                string msg = "Apartment " + aptADDR + " has already been assigned to another service " + Environment.NewLine +
                    "Do you want to replace with current new assignment ?";

                if (MessageBox.Show(GTServiceAssign.m_CustomForm, msg,
                    "Service Assignment", MessageBoxButtons.OKCancel) == DialogResult.OK)
                {
                    DeleteRecord(aptID, srvFID);
                    return false;
                }
                else
                    throw new System.Exception("Cancel saving for apartment " + aptADDR + " as conflict found");
            }
            else
                return true;
        }

        private void DeleteRecord(int aptID, int srvFID)
        {
            string ssql = "DELETE FROM GC_DP_ASSIGNMENT WHERE G3E_FID = " + srvFID + " AND OBJECTID = " + aptID;
            myUtil.ADODB_ExecuteNonQuery(ssql);
        }

    }
}
