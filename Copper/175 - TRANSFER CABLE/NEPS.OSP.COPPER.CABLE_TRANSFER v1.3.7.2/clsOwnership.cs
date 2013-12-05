using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

using ADODB;
using Intergraph.GTechnology.Interfaces;
using Intergraph.GTechnology.API;
using System.Diagnostics;


class clsOwnership
{
    private int DSLAM_FID = -1;

    private IGTDataContext m_GTDataContext = null;

    #region "Select Table"  

    public void ReadChilds(int parentFID)
    {
        int parentID = RecordExist("GC_OWNERSHIP", "G3E_ID", "G3E_FID", parentFID, "");
        if (parentID > -1)
        {
            string ssql = "SELECT * from GC_OWNERSHIP WHERE OWNER1_ID = " + parentID.ToString() + " ORDER BY G3E_FID";
            this.m_GTDataContext = GTClassFactory.Create<IGTApplication>().DataContext;
            ADODB.Recordset rs = new ADODB.Recordset();
            rs = GTClassFactory.Create<IGTApplication>().DataContext.OpenRecordset(ssql,
                ADODB.CursorTypeEnum.adOpenStatic, ADODB.LockTypeEnum.adLockOptimistic, 1);

            int childFID;
            int childFNO;
            while (!rs.EOF)
            {
                childFID = ParseInt(rs.Fields["G3E_FID"].Value.ToString());
                childFNO = ParseInt(rs.Fields["G3E_FNO"].Value.ToString());

                rs.MoveNext();
            }
            rs.Close();
            rs = null;

        }
    }  

    #endregion

    #region "Delete DSLAM"
    /// <summary>
    /// Delete all child base on ownership table.
    /// Recursive calls to DeleteRecord will also delete childs of the child
    /// </summary>
    /// <param name="parentFID">Parent FID</param>
    public void DeleteChild(int parentFID)
    {
        try
        {
            // get parent G3E_ID from ownership table if exist else will return -1
            int parentID = RecordExist("GC_OWNERSHIP", "G3E_ID", "G3E_FID", parentFID, "");
            if (parentID > -1)
            {
                // read all childs of the parent
                string ssql = "SELECT * from GC_OWNERSHIP WHERE OWNER1_ID = " + parentID.ToString();
                this.m_GTDataContext = GTClassFactory.Create<IGTApplication>().DataContext;
                ADODB.Recordset rs = new ADODB.Recordset();
                rs = GTClassFactory.Create<IGTApplication>().DataContext.OpenRecordset(ssql,
                    ADODB.CursorTypeEnum.adOpenStatic, ADODB.LockTypeEnum.adLockOptimistic, 1);

                int childFID;
                int childFNO;
                while (!rs.EOF)
                {
                    childFID = ParseInt(rs.Fields["G3E_FID"].Value.ToString());
                    childFNO = ParseInt(rs.Fields["G3E_FNO"].Value.ToString());
                    // delete all childs (recursive call)
                    DeleteRecord(childFID, childFNO);
                    rs.MoveNext();
                }
                rs.Close();
                rs = null;
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show("Error while delete record child from database\r\n" + ex.Message);
        }
    }

    public void DeleteRecord(int recFID, int recFNO)
    {
        try
        {
            int iR;
            string ssql = "";
           
            GTClassFactory.Create<IGTApplication>().DataContext.Execute(ssql, out iR, (int)ADODB.CommandTypeEnum.adCmdText);

            DeleteChild(recFID);

            ssql = "DELETE FROM GC_OWNERSHIP WHERE G3E_FID = " + recFID.ToString();
            GTClassFactory.Create<IGTApplication>().DataContext.Execute(ssql, out iR, (int)ADODB.CommandTypeEnum.adCmdText);

            GTClassFactory.Create<IGTApplication>().DataContext.Execute("COMMIT", out iR, (int)ADODB.CommandTypeEnum.adCmdText);
        }
        catch (Exception ex)
        {
            MessageBox.Show("Error while delete record from database\r\n" + ex.Message);
        }
    }
    #endregion 

    #region "Insert Ownership"
    public static int ParentOwnershipID(int FID, int FNO)
    {
        int id = RecordExist("GC_OWNERSHIP", "G3E_ID", "G3E_FID", FID.ToString(), "");
        if (id == -1)
        {
            id = NextSequence("GC_OWNERSHIP_SEQ");

            string ssql = "INSERT INTO GC_OWNERSHIP (G3E_ID, G3E_FNO, G3E_FID, G3E_CNO, G3E_CID) ";
            ssql += "VALUES (" + id.ToString() + "," + FNO.ToString() + "," + FID.ToString() + ", 64, 1)";

            int iRecordsAffected;
            GTClassFactory.Create<IGTApplication>().DataContext.Execute(ssql,
                out iRecordsAffected, (int)ADODB.CommandTypeEnum.adCmdText);

            GTClassFactory.Create<IGTApplication>().DataContext.Execute
                ("COMMIT", out iRecordsAffected, (int)ADODB.CommandTypeEnum.adCmdText);
        }
        return id;
    }

    public static void AddOwnership(int childFNO, int childFID, int parentFNO, int parentFID)
    {
        int parentID = GetParentID(parentFID);
        int parentCNO = parentFNO + 1;

        Debug.Write("Child FNO : " + childFNO + ", FID : " + childFID + " - Parent FNO : " + parentFNO + ", FID : " + parentFID + ", ID : " + parentID);
        int id = NextSequence("GC_OWNERSHIP_SEQ");
        string ssql = "INSERT INTO GC_OWNERSHIP (G3E_ID, G3E_FNO, G3E_FID, G3E_CNO, G3E_CID, OWNER1_CNO, OWNER1_ID) ";
        ssql += "VALUES (" + id.ToString() + "," + childFNO.ToString() + "," + childFID.ToString() +
            ",64,1," + parentCNO.ToString() + "," + parentID.ToString() + ")";

        int iR;
        GTClassFactory.Create<IGTApplication>().DataContext.Execute(ssql,
            out iR, (int)ADODB.CommandTypeEnum.adCmdText);
        GTClassFactory.Create<IGTApplication>().DataContext.Execute("COMMIT", out iR, (int)ADODB.CommandTypeEnum.adCmdText);
        Debug.WriteLine(" - ownership added");
    }

    private static int GetParentID(int FID)
    {
        return RecordExist("GC_OWNERSHIP", "G3E_ID", "G3E_FID", FID.ToString(), "");
    }

    private static int NextSequence(string seq_name)
    {
        string ssql = "SELECT " + seq_name + ".NEXTVAL from dual";
        ADODB.Recordset rs = new ADODB.Recordset();
        rs = GTClassFactory.Create<IGTApplication>().DataContext.OpenRecordset(ssql,
            ADODB.CursorTypeEnum.adOpenStatic, ADODB.LockTypeEnum.adLockOptimistic, 1);

        if (rs.RecordCount > 0)
        {
            rs.MoveFirst();
            return (int.Parse(rs.Fields[0].Value.ToString()));
        }
        else
            return -1;
    }


    #endregion

    #region "Update Table"
    private void UpdateRecord(string tablename, string[] col, object[] val, int iFID)
    {
        string ssql = "UPDATE " + tablename + " SET ";
        for (int i = 0; i < col.Length; i++)
        {
            if (val[i] is string)
                ssql += col[i] + " = '" + val[i] + "', ";
            else
                ssql += col[i] + " = " + val[i] + ", ";
        }
        ssql = ssql.Remove(ssql.Length - 2);
        ssql += " WHERE G3E_FID = " + iFID.ToString();

        int iR;
        GTClassFactory.Create<IGTApplication>().DataContext.Execute(ssql, out iR, (int)ADODB.CommandTypeEnum.adCmdText);
        GTClassFactory.Create<IGTApplication>().DataContext.Execute("COMMIT", out iR, (int)ADODB.CommandTypeEnum.adCmdText);
    }
    #endregion

    #region "Helper"   
    /// <summary>
    /// Check if record exist in table:tablename where keyname == keyval
    /// 
    /// </summary>
    /// <param name="tablename"></param>
    /// <param name="idname"></param>
    /// <param name="keyname"></param>
    /// <param name="keyval"></param>
    /// <returns></returns>
    private static int RecordExist(string tablename, string idname, string keyname, object keyval, string filter)
    {
        string ssql = "SELECT " + idname + " FROM " + tablename + " WHERE " + keyname + " = " +
            ((keyval is string) ? "'" + keyval + "'" : keyval.ToString());
        if (filter.Length > 0)
            ssql += " AND " + filter;

        ADODB.Recordset rs = new ADODB.Recordset();

        rs = GTClassFactory.Create<IGTApplication>().DataContext.OpenRecordset(ssql,
            ADODB.CursorTypeEnum.adOpenStatic, ADODB.LockTypeEnum.adLockOptimistic, 1);

        if (rs.RecordCount > 0)
        {
            rs.MoveFirst();
            Debug.WriteLine("SUCCESS : " + ssql);
            return (int.Parse(rs.Fields[0].Value.ToString()));
        }
        else
            return -1;
    }


    private static int ParseInt(string val)
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
    #endregion
}

