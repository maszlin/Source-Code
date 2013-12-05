using System;
using System.Collections.Generic;
using System.Text;

using Intergraph.GTechnology.Interfaces;
using Intergraph.GTechnology.API;
using ADODB;


class clsTrace
{
    public static string TraceDownstream(int cableFID)
    {
        ADODB.Recordset rsSrcCntD = new ADODB.Recordset();
        string trace_name = "TRACEDOWN_" + DateTime.Now.ToString("yyMMdd_HHmmss");

        try
        {
            object[] objs = { trace_name, cableFID };

            int iRecordsAffected;
            rsSrcCntD = GTClassFactory.Create<IGTApplication>().DataContext.Execute("GC_OSP_COP_VAL.TRACE_DOWNSTREAM",
                out iRecordsAffected, (int)ADODB.CommandTypeEnum.adCmdStoredProc, objs);
        }
        catch (Exception ex)
        {
            return "";
        }

        return trace_name;
    }

    public static string TraceUpstream(int cableFID)
    {
        ADODB.Recordset rsSrcCntD = new ADODB.Recordset();
        string trace_name = "TRACEUP_" + DateTime.Now.ToString("yyMMdd_HHmmss");

        try
        {
            object[] objs = { trace_name, cableFID };

            int iRecordsAffected;
            rsSrcCntD = GTClassFactory.Create<IGTApplication>().DataContext.Execute("GC_OSP_COP_VAL.TRACE_UPSTREAM",
                out iRecordsAffected, (int)ADODB.CommandTypeEnum.adCmdStoredProc, objs);
        }
        catch (Exception ex)
        {
            
        }

        return trace_name;
    }

    public static void DeleteTrace(string trace_name)
    {
        ADODB.Recordset rs = new ADODB.Recordset();
        string strSQL = "TRACE.DELETE(" + trace_name + ")";

        try
        {
            int iRecordsAffected;
            rs = GTClassFactory.Create<IGTApplication>().DataContext.Execute(strSQL,
                out iRecordsAffected, (int)ADODB.CommandTypeEnum.adCmdStoredProc, 1);
            rs = null;
        }
        catch (Exception ex)
        { 
            
        }
    }
}

