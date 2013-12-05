/*
 *  clsTrace.cs
 *  started : m.zam @ 19-SEPT-2012
 * 
 *  TraceUp 
 *  - get start point
 *  - return start FID
 * 
 *  TraceDown 
 *  - get end points 
 *  - return List<object[FNO,FID]>
 * 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using Intergraph.GTechnology.Interfaces;
using Intergraph.GTechnology.API;
using ADODB;

class clsTrace
{
    public static int TraceUp_EGetStartPoint(int iFID)
    {
        // we are looking for IN_FNO 10800 SPLICE_CLASS = MAIN JOINT
        ADODB.Recordset rs = new ADODB.Recordset();
        rs = GTClassFactory.Create<IGTApplication>().DataContext.OpenRecordset(
            "SELECT IN_FNO, IN_FID, G3E_FNO, G3E_FID, OUT_FNO, OUT_FID FROM GC_NR_CONNECT " +
            "START WITH G3E_FID = " + iFID.ToString() + " CONNECT BY NOCYCLE " +
            "PRIOR IN_FID = OUT_FID AND IN_FNO = 10800 AND IN_FID <> 0 AND OUT_FID <> 0 ",
            ADODB.CursorTypeEnum.adOpenStatic, ADODB.LockTypeEnum.adLockOptimistic, 1);

        if (rs.RecordCount > 0)
        {
            rs.MoveLast();
            return Convert.ToInt32(rs.Fields["IN_FID"].Value); // return main joint FID
        }
        return -1;
    }


    public static int TraceUp_DPP(int iFID)
    {
        // we are looking for IN_FNO 6300 => DDP
        ADODB.Recordset rs = new ADODB.Recordset();
        rs = GTClassFactory.Create<IGTApplication>().DataContext.OpenRecordset(
            "SELECT IN_FNO, IN_FID, G3E_FNO, G3E_FID, OUT_FNO, OUT_FID FROM GC_NR_CONNECT " +
            "START WITH G3E_FID = " + iFID.ToString() + " CONNECT BY NOCYCLE " +
            "PRIOR IN_FID = OUT_FID AND IN_FNO <> 6300 AND IN_FID <> 0 AND OUT_FID <> 0 ",
            ADODB.CursorTypeEnum.adOpenStatic, ADODB.LockTypeEnum.adLockOptimistic, 1);

        if (rs.RecordCount > 0)
        {
            rs.MoveLast();
            return Convert.ToInt32(rs.Fields["IN_FID"].Value); // return DDP FID
        }
        return -1;
    }

    public static int[] TraceUp_DGetStartPoint(int iFID)
    {
        // we are looking for IN_FNO 9800(VDSL2), 9600(RT), 9100 (MSAN) or 10300(ITFACE) as start of the network
        ADODB.Recordset rs = new ADODB.Recordset();
        rs = GTClassFactory.Create<IGTApplication>().DataContext.OpenRecordset(
            "SELECT IN_FNO, IN_FID, G3E_FNO, G3E_FID, OUT_FNO, OUT_FID FROM GC_NR_CONNECT " +
            "START WITH G3E_FID = " + iFID.ToString() + " CONNECT BY NOCYCLE " +
            "PRIOR IN_FID = OUT_FID AND OUT_FNO NOT IN (9100, 9600, 9800, 10300) AND IN_FID <> 0 AND OUT_FID <> 0 ",
            ADODB.CursorTypeEnum.adOpenStatic, ADODB.LockTypeEnum.adLockOptimistic, 1);

        if (rs.RecordCount > 0)
        {
            rs.MoveLast();
            while (!rs.BOF)
            {
                int iFNO = Convert.ToInt32(rs.Fields["IN_FNO"].Value);
                if (iFNO == 9100 || iFNO == 9600 || iFNO == 9800 || iFNO == 10300) // RT or MSAN or VDSL2 or ITFACE
                {
                    return new int[] { iFNO, Convert.ToInt32(rs.Fields["IN_FID"].Value), Convert.ToInt32(rs.Fields["G3E_FID"].Value) };
                }
                rs.MovePrevious();
            }
        }
        return null;
    }

    public static Dictionary<int, int> TraceDown_GetEndPoints(int startFID, string startField) // get all end point
    {
        Dictionary<int, int> terminations = new Dictionary<int, int>();

        ADODB.Recordset rs = new ADODB.Recordset();
        string ssql = "SELECT IN_FNO, IN_FID, G3E_FNO, G3E_FID, OUT_FNO, OUT_FID FROM GC_NR_CONNECT " +
            "START WITH " + startField + " = " + startFID.ToString() + " CONNECT BY NOCYCLE " +
            "PRIOR OUT_FID = IN_FID AND IN_FNO IN (10800, 13100, 6200) AND IN_FID <> 0 AND OUT_FID <> 0 ";
        rs = GTClassFactory.Create<IGTApplication>().DataContext.OpenRecordset(
            ssql, ADODB.CursorTypeEnum.adOpenStatic, ADODB.LockTypeEnum.adLockOptimistic, 1);

        if (rs.RecordCount > 0)
        {
            while (!rs.EOF)
            {
                if (!terminations.ContainsKey(Convert.ToInt32(rs.Fields["OUT_FID"].Value)))
                    terminations.Add(Convert.ToInt32(rs.Fields["OUT_FID"].Value), Convert.ToInt32(rs.Fields["OUT_FNO"].Value));

                if (Convert.ToInt32(rs.Fields["IN_FNO"].Value) == 10800) // joint FNO
                {
                    // the joint is not end joint so remove from termination point                    
                    if (terminations.ContainsKey(Convert.ToInt32(rs.Fields["IN_FID"].Value)))
                        terminations.Remove(Convert.ToInt32(rs.Fields["IN_FID"].Value));
                }
                rs.MoveNext();
            }
        }
        return terminations;
    }

}
