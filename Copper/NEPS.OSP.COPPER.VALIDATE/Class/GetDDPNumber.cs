using System;
using System.Collections.Generic;
using System.Text;
using Intergraph.GTechnology.Interfaces;
using Intergraph.GTechnology.API;
using ADODB;

namespace NEPS.OSP.COPPER.VALIDATE
{
    public class GetDDPNumber
    {
        public static string NextCode(string excabb)
        {
            try
            {
                string ssql = "SELECT MAX (LPAD(REGEXP_REPLACE (A.DDP_NUM,'[A-Z]|[a-z]','') ,7,'0'))  " +
                "FROM GC_DDP A, GC_NETELEM B WHERE A.G3E_FID = B.G3E_FID AND B.EXC_ABB = '{0}' ";

                ADODB.Recordset rs = new ADODB.Recordset();
                rs = GTClassFactory.Create<IGTApplication>().DataContext.OpenRecordset(string.Format(ssql, excabb),
                    ADODB.CursorTypeEnum.adOpenStatic, ADODB.LockTypeEnum.adLockOptimistic, 1);

                if (!rs.EOF)
                {
                    int val = Convert.ToInt32(rs.Fields[0].Value) + 1;
                    return val.ToString("0000");
                }
                else
                    return "0001";
            }
            catch (Exception ex)
            {
                return "0001";
            }
        }

        public static bool DuplicateCode(string excabb, string ddpnum, int ddpfid)
        {
            try
            {
                ddpnum = ddpnum.PadLeft(4, '0');
                string ssql = "SELECT A.DDP_NUM FROM GC_DDP A, GC_NETELEM B WHERE A.DDP_NUM = '{1}' AND A.G3E_FID <> {2} AND B.EXC_ABB = '{0}' AND A.G3E_FID = B.G3E_FID";

                ADODB.Recordset rs = new ADODB.Recordset();
                rs = GTClassFactory.Create<IGTApplication>().DataContext.OpenRecordset(string.Format(ssql, excabb, ddpnum, ddpfid),
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
