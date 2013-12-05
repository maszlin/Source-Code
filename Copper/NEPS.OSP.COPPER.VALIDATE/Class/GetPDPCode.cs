/*
 * GetPDPCode.cs
 * 
 * created : m.zam @ 02-10-2012
 * function : get next PDP code which is unique by cabinet
 * 
 */ 
using System;
using System.Collections.Generic;
using System.Text;
using Intergraph.GTechnology.Interfaces;
using Intergraph.GTechnology.API;
using ADODB;

namespace NEPS.OSP.COPPER.VALIDATE
{
    public class GetPDPCode
    {
        public static string NextCode(string excabb, string itface, string rtcode)
        {
            try
            {
                string ssql = "SELECT MAX (LPAD(REGEXP_REPLACE (A.PDP_CODE,'[A-Z]|[a-z]','') ,7,'0'))  " +
                "FROM GC_PDP A, GC_NETELEM B WHERE A.G3E_FID = B.G3E_FID AND A.{0} = '{1}' AND B.EXC_ABB = '{2}' ";

                if (rtcode.Length > 0)
                    ssql = string.Format(ssql, "RT_CODE", rtcode, excabb);
                else
                    ssql = string.Format(ssql, "ITFACE_CODE", itface, excabb);

                ADODB.Recordset rs = new ADODB.Recordset();
                rs = GTClassFactory.Create<IGTApplication>().DataContext.OpenRecordset(ssql,
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

    }
}
