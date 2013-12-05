using System;
using System.Collections.Generic;
using System.Text;
using Intergraph.GTechnology.Interfaces;
using Intergraph.GTechnology.API;
using ADODB;

namespace NEPS.OSP.COPPER.VALIDATE
{
    public class GetPDDPCode
    {
        public static bool AssignCode(string excabb, int pddpFID)
        {
            try
            {
                string next_code = NextCode(excabb);
                string route_dist = clsTrace.ESIDE_RouteDistance(pddpFID);

                string ssql = "UPDATE GC_PDDP SET PDDP_CODE = '" + next_code + "', DIST_FROM_EXC = " + route_dist +
                " WHERE G3E_FID = " + pddpFID.ToString();

                ADODB.Recordset rsSQL = new ADODB.Recordset();
                int iR;
                GTClassFactory.Create<IGTApplication>().DataContext.Execute(ssql, out iR, (int)ADODB.CommandTypeEnum.adCmdText);
                GTClassFactory.Create<IGTApplication>().DataContext.Execute("COMMIT", out iR, (int)ADODB.CommandTypeEnum.adCmdText);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static string NextCode(string excabb)
        {
            try
            {
                string ssql = "SELECT MAX (LPAD(REGEXP_REPLACE (A.PDDP_CODE,'[A-Z]|[a-z]','') ,7,'0'))  " +
                "FROM GC_PDDP A, GC_NETELEM B WHERE A.G3E_FID = B.G3E_FID AND B.EXC_ABB = '{0}'";

                ADODB.Recordset rs = new ADODB.Recordset();
                rs = GTClassFactory.Create<IGTApplication>().DataContext.OpenRecordset(string.Format(ssql, excabb),
                    ADODB.CursorTypeEnum.adOpenStatic, ADODB.LockTypeEnum.adLockOptimistic, 1);

                if (!rs.EOF)
                {
                    int val = Convert.ToInt32(rs.Fields[0].Value) + 1;
                    return val.ToString("000");
                }
                else
                    return "001";
            }
            catch (Exception ex)
            {
                return "001";
            }
        }

        public static bool DuplicateCode(string excabb, string pddpcode, int pddpfid)
        {
            try
            {
                pddpcode = pddpcode.PadLeft(3, '0');
                string ssql = "SELECT PDDP_CODE FROM GC_PDDP A, GC_NETELEM B WHERE A.PDDP_CODE = '{1}' AND A.G3E_FID <> {2} AND B.EXC_ABB = '{0}' AND A.G3E_FID = B.G3E_FID";

                ADODB.Recordset rs = new ADODB.Recordset();
                rs = GTClassFactory.Create<IGTApplication>().DataContext.OpenRecordset(string.Format(ssql, excabb, pddpcode, pddpfid),
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
