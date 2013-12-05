using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Windows.Forms;

using Intergraph.GTechnology.Interfaces;
using Intergraph.GTechnology.API;

using ADODB;

class myUtil
{
    public static IGTPoint GetPointInBetween(IGTPolylineGeometry geoPoint, int startPoint, double percentage)
    {
        IGTPoint newPoint = GTClassFactory.Create<IGTPoint>();

        double x1 = geoPoint.Points[startPoint].X;
        double y1 = geoPoint.Points[startPoint].Y;

        double x2 = geoPoint.Points[startPoint + 1].X;
        double y2 = geoPoint.Points[startPoint + 1].Y;

        newPoint.X = x1 + ((x2 - x1) * percentage);
        newPoint.Y = y1 + ((y2 - y1) * percentage);

        return newPoint;
    }

    public static string CellValue(object val)
    {
        try
        {
            if (val == null)
                return "";
            else
                return val.ToString().Trim();
        }
        catch { return ""; }
    }

    public static int ParseInt(string val)
    {
        try
        {
            return int.Parse(val);
        }
        catch
        {
            return 0;
        }
    }

    public static double CableLength(IGTPolylineGeometry linepoints)
    {
        double cablelen = 0;
        for (int i = 0; i < linepoints.Points.Count - 1; i++)
        {
            double x = (linepoints.Points[i + 1].X - linepoints.Points[i].X);
            double y = (linepoints.Points[i + 1].Y - linepoints.Points[i].Y);

            cablelen += Math.Sqrt((x * x) + (y * y));
        }
        string l = cablelen.ToString("0.00");
        return double.Parse(l);
    }

    //public static ADODB.Recordset ADODB_ExecuteQuery(string ssql)
    //{

    //}
    public static void ADODB_ExecuteNonQuery(string ssql)
    {        
        int iR;
        GTClassFactory.Create<IGTApplication>().DataContext.Execute(ssql, out iR, (int)ADODB.CommandTypeEnum.adCmdText);
        GTClassFactory.Create<IGTApplication>().DataContext.Execute("COMMIT", out iR, (int)ADODB.CommandTypeEnum.adCmdText);               
    }

    public static string GetFieldValue(string tablename, string fieldname, int iFID)
    {
        string sql = "SELECT " + fieldname + " FROM " + tablename + " WHERE G3E_FID =" + iFID;
        ADODB.Recordset rs = new ADODB.Recordset();
        rs = GTClassFactory.Create<IGTApplication>().DataContext.OpenRecordset
            (sql, ADODB.CursorTypeEnum.adOpenStatic, ADODB.LockTypeEnum.adLockReadOnly, 1);
        if (rs.RecordCount > 0)
            return rs.Fields[0].Value.ToString();
        else
            return "-";
    }

    public static string GetFieldValues(string tablename, string[] fieldname, int iFID)
    {
        string sql = "SELECT " + fieldname[0];
        for (int i = 1; i < fieldname.Length; i++) sql += "," + fieldname[i];
        sql += " FROM " + tablename + " WHERE G3E_FID =" + iFID;

        ADODB.Recordset rs = new ADODB.Recordset();
        rs = GTClassFactory.Create<IGTApplication>().DataContext.OpenRecordset
            (sql, ADODB.CursorTypeEnum.adOpenStatic, ADODB.LockTypeEnum.adLockReadOnly, 1);
        if (rs.RecordCount > 0)
            return rs.Fields[0].Value.ToString();
        else
            return "-";
    }

    public static string rsField(ADODB.Recordset rs, string fieldname)
    {
        try
        {
            object val = rs.Fields[fieldname].Value.ToString();
            if (val == null)
                return "";
            else
                return val.ToString().Trim();
        }
        catch { return ""; }
    }
    
    public static void CopyFields(IGTKeyObject dest, ADODB.Recordset srce, short iCNO, string fieldname)
    {
        try
        {
            Debug.Write(fieldname + " = ");
            if (srce.Fields[fieldname].Value is DBNull)
            {
                Debug.WriteLine("NULL");
                return;
            }
            else if (srce.Fields[fieldname].Type.ToString().IndexOf("VarChar") > -1)
            {
                dest.Components.GetComponent(iCNO).Recordset.Update(fieldname, srce.Fields[fieldname].Value.ToString());
                Debug.WriteLine(srce.Fields[fieldname].Value.ToString());
            }
            else
            {
                dest.Components.GetComponent(iCNO).Recordset.Update(fieldname, srce.Fields[fieldname].Value);
                Debug.WriteLine(srce.Fields[fieldname].Value.ToString());
            }

        }
        catch (Exception ex)
        {
            throw ex;
        }
    }


}
