using System;
using System.Collections.Generic;
using System.Text;

using Intergraph.GTechnology.Interfaces;
using Intergraph.GTechnology.API;
using ADODB;
namespace NEPS.OSP.COPPER.CABLE.CODE
{
    class clsUpdateCode
    {
        short CABLE_FNO = 7000;
        short JOINT_FNO = 10800;
        short JOINT_CNO = 10801;
        short JOINT_GEO_CNO = 10820;
        short JOINT_DET_CNO = 10821;

        string m_NewCode;
        string m_OldCode;
        Dictionary<int, short> downstream_features = new Dictionary<int, short>(); // FID + FNO
        Dictionary<int, short> upstream_features = new Dictionary<int, short>(); // FID + FNO

        public clsUpdateCode(string newcode, string oldcode, int FID)
        {
            m_NewCode = newcode;
            m_OldCode = oldcode;

            downstream_features = clsTrace.TraceDownstream(FID, "G3E_FID");
            upstream_features = clsTrace.TraceUpstream(FID, "G3E_FID");
        }

        public bool UpdateCableCode()
        {
            try
            {
                foreach (int key in downstream_features.Keys)
                    UpdateCableCode(downstream_features[key], key);

                foreach (int key in upstream_features.Keys)
                    UpdateCableCode(upstream_features[key], key);

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        private void UpdateCableCode(short FNO, int FID)
        {
            try
            {
                string ssql_anno = "";
                if (FNO == 7000)
                    ssql_anno = ", COUNT_ANNOTATION = REPLACE(COUNT_ANNOTATION,'" + m_OldCode + "','" + m_NewCode + "') ";

                short CNO = ++FNO; 
                string ssql = 
                    "UPDATE " + TableName(CNO) + " SET CABLE_CODE = '" + m_NewCode + "' " + ssql_anno +
                    "WHERE G3E_FID = " + FID.ToString() + " AND CABLE_CODE = '" + m_OldCode + "'";

                int iR;
                GTClassFactory.Create<IGTApplication>().DataContext.Execute(ssql, out iR, (int)ADODB.CommandTypeEnum.adCmdText);
                GTClassFactory.Create<IGTApplication>().DataContext.Execute("COMMIT", out iR, (int)ADODB.CommandTypeEnum.adCmdText);
                GTManageCableCode.m_CustomForm.UpdateProgressBar();

            }
            catch (Exception ex)
            {
                //Debug.WriteLine("SaveRecord : FNO - " + FNO.ToString() + ", FID - " + FID.ToString() + "\r\n" + ex.Message);
            }
        }

        private string TableName(short CNO)
        {
            string ssql = "SELECT G3E_TABLE FROM G3E_COMPONENT WHERE G3E_CNO = " + CNO.ToString();
            ADODB.Recordset rs = GTClassFactory.Create<IGTApplication>().DataContext.OpenRecordset
                (ssql, ADODB.CursorTypeEnum.adOpenStatic, ADODB.LockTypeEnum.adLockReadOnly, 1);
            if (rs.RecordCount > 0)
                return myUtil.rsField(rs, "G3E_TABLE");
            else
                return "";
        }

    }

}