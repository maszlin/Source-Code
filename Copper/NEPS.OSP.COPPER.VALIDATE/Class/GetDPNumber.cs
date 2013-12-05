using System;
using System.Collections.Generic;
using System.Text;
using Intergraph.GTechnology.Interfaces;
using Intergraph.GTechnology.API;
using ADODB;


namespace NEPS.OSP.COPPER.VALIDATE
{
    public class GetDPNumber
    {
        public static string NextCode(string excabb, string itface, string rtcode)
        {
            try
            {
                string ssql = "SELECT MAX (LPAD(REGEXP_REPLACE (A.DP_NUM,'[A-Z]|[a-z]','') ,7,'0'))  " +
                "FROM GC_DP A, GC_NETELEM B WHERE A.G3E_FID = B.G3E_FID AND B.EXC_ABB = '{0}' ";
                
                if (itface.Length == 0)
                    ssql += " AND ITFACE_CODE = '" + itface + "'";
                else
                    ssql += " AND RT_CODE = '" + rtcode + "'";

                ADODB.Recordset rs = new ADODB.Recordset();
                rs = GTClassFactory.Create<IGTApplication>().DataContext.OpenRecordset(string.Format(ssql, excabb),
                    ADODB.CursorTypeEnum.adOpenStatic, ADODB.LockTypeEnum.adLockOptimistic, 1);

                if (!rs.EOF)
                {
                    int val = Convert.ToInt32(rs.Fields[0].Value) + 1;
                    return val.ToString("00");
                }
                else
                    return "01";
            }
            catch (Exception ex)
            {
                return "01";
            }
        }

        public static bool DuplicateCode(string excabb,string itface, string rtcode, string dpnum, int dpfid)
        {
            try
            {
                dpnum = dpnum.PadLeft(7, '0');
                string ssql = "SELECT A.DP_NUM FROM GC_DP A, GC_NETELEM B "
                + "WHERE TRIM(A.DP_NUM) = TRIM('{1}') "
                + "AND A.G3E_FID <> {2} AND B.EXC_ABB = '{0}' AND A.G3E_FID = B.G3E_FID";

                if (itface.Length > 0)
                    ssql += " AND ITFACE_CODE = '" + itface + "'";
                else if (rtcode.Length > 0)
                    ssql += " AND RT_CODE = '" + rtcode + "'";
                else
                    return false;

                ADODB.Recordset rs = new ADODB.Recordset();
                rs = GTClassFactory.Create<IGTApplication>().DataContext.OpenRecordset(string.Format(ssql, excabb, dpnum, dpfid),
                    ADODB.CursorTypeEnum.adOpenStatic, ADODB.LockTypeEnum.adLockOptimistic, 1);

                return (rs.RecordCount > 0);
            }
            catch (Exception ex)
            {
                return false;
            }
        }


    }

}

