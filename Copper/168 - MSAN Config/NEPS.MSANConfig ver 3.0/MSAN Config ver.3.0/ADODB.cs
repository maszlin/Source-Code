using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using ADODB;

class adoDB
{
    static internal void SaveDBSetting(string svr, string uid, string pwd, string db)
    {
        MyRegistry.SaveSetting("NEPS", "adoDB", "Server", svr);
        MyRegistry.SaveSetting("NEPS", "OracleDB", "DBName", db);
        MyRegistry.SaveSetting("NEPS", "OracleDB", "UID", uid);
        MyRegistry.SaveSetting("NEPS", "OracleDB", "PWD", pwd);
    }

    static internal string ConnString
    {
        get
        {
            string svr = MyRegistry.GetSetting("NEPS", "OracleDB", "Server", "NOVA");
            string db = MyRegistry.GetSetting("NEPS", "OracleDB", "DBName", "NOVA");
            string uid = MyRegistry.GetSetting("NEPS", "OracleDB", "UID", "NEPS");
            string pwd = MyRegistry.GetSetting("NEPS", "OracleDB", "PWD", "N3PS");
            return "server=" + svr +
                "; uid=" + uid +
                "; password=" + pwd +
                ";";

        }
    }

    static bool ExecuteWriteQuery(string ssql)
    {
        bool flag = false;
        try
        {
            int iRecordsAffected;
            GTClassFactory.Create<IGTApplication>().DataContext.Execute(sSql, out iRecordsAffected, (int)ADODB.CommandTypeEnum.adCmdText);

            flag = true;
        }
        catch (Exception ex)
        {
            System.Windows.Forms.MessageBox.Show(ex.Message, "ExecuteWriteQuery");
        }
        finally
        {
            return flag;
        }
    }

    static internal void Commit()
    {
        // Open the connection using the connection string.
        using (OracleConnection con = new OracleConnection((OracleDB.ConnString)))
        {
            try
            {
                con.Open();

                OracleCommand cmd = new OracleCommand();
                //setting the connection of the command
                cmd.Connection = con;
                //setting the sql of the command
                cmd.CommandText = "COMMIT";
                //executing the query and writing to the database
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message, "ExecuteWriteQuery");
            }
            finally
            {
                con.Close();
                con.Dispose();
            }
        }
    }


    #region "SELECT"
    static internal Recordset SelectTable(string tablename, string colname, string colvalue)
    {
        string ssql = "SELECT * FROM " + tablename + " WHERE " + colname + " = '" + colvalue + "'";
        return SelectTable(tablename, ssql);
    }

        //string tablename;
        //string sSql = "SELECT TABLE_NAME FROM REF_BND_TYPE WHERE BND_TYPE = '"
        //    + cboTypeBoundary.Text + "'";

    static internal Recordset SelectTable(string tablename, string ssql)
    {
        try {
            this.m_GTDataContext = GTClassFactory.Create<IGTApplication>().DataContext;
            ADODB.Recordset rs = new ADODB.Recordset();
            rs = GTClassFactory.Create<IGTApplication>().DataContext.OpenRecordset(sSql, ADODB.CursorTypeEnum.adOpenStatic, ADODB.LockTypeEnum.adLockOptimistic, 1);

            if (rs.RecordCount > 0)
            {
                return rs;
            }
            else
                return null; // exit if record not found
        }
        catch (Exception ex)
        {
            System.Windows.Forms.MessageBox.Show(ex.Message, "SelectTable");
            return null;
        }

    }

    static internal bool RecordFound(string tablename, string colname, string colvalue)
    {
        string ssql = "SELECT * FROM " + tablename + " WHERE " + colname + " = '" + colvalue + "'";
        if (SelectTable(tablename, ssql) == null)
            return false;
        else
            return true;
    }

    static internal int LatestID(string tablename, string colname)
    {
        string ssql = "SELECT " + colname + " FROM " + tablename + " ORDER BY " + colname + " DESC";
        Recordset ds = SelectTable(tablename, ssql);
        if (ds.Tables[0].Rows.Count > 0)
            return int.Parse(ds.Tables[0].Rows[0][colname].ToString());
        else
            return -1;
    }

    #endregion

    #region "INSERT"
    /// <summary>
    /// Insert record to table
    /// </summary>
    /// <param name="tablename">table name for the record</param>
    /// <param name="columnlist">column name to insert</param>
    /// <param name="valuelist">value to insert</param>
    /// <returns>true if insert success</returns>
    static internal bool InsertTable(string tablename, List<string> columnlist, List<object> valuelist) //, string idname = "", string sseq = "")
    {
        //creating sql command
        string ssql = "INSERT INTO " + tablename + " (";
        if (idname.Length > 0) ssql += idname + ", ";

        string ssqlval = ") VALUES (";
        if (sseq.Length > 0) ssqlval += sseq + ", ";

        for (int i = 0; i < columnlist.Count; i++)
        {
            ssql += columnlist[i] + ", ";
            if (valuelist[i].GetType() == typeof(int))
                ssqlval += valuelist[i] + ", ";
            else
                ssqlval += " '" + valuelist[i] + "', ";
        }
        ssql = ssql.Remove(ssql.Length - 2);// remove the last comma from ssql
        ssqlval = ssqlval.Remove(ssqlval.Length - 2) + ")"; // remove from ssqlval the last two character : "', " + add ")" to end the sql query

        if (ExecuteWriteQuery(ssql + ssqlval))
            return true;
        else
            return false;
    }

    static internal bool InsertTableSQL(string tablename, string ssql)
    {
        // Open the connection using the connection string.
        using (OracleConnection con = new OracleConnection(OracleDB.ConnString))
        {
            try
            {
                con.Open();
                OracleCommand insertCommand = new OracleCommand(ssql, con);
                insertCommand.ExecuteNonQuery();
                con.Close();
                return true;
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message, "InsertTable");
                return false;
            }
            finally
            {
                con.Close();
                con.Dispose();
            }
        }
    }

    #endregion

    #region "UPDATE"

    static internal bool UpdateTable(string tablename, string colname, string colvalue, string idname, string idvalue)
    {
        string ssql = "UPDATE " + tablename + " SET " + colname + " = '" + colvalue + "' ";
        ssql += "WHERE " + idname + " = '" + idvalue + "'";
        return UpdateTableSQL(tablename, ssql);
    }

    static internal bool UpdateTable(string tablename, List<string> columnlist, List<string> valuelist, string idname, string idvalue)
    {
        string ssql = "UPDATE " + tablename + " SET ";
        for (int i = 0; i < columnlist.Count; i++)
        {
            ssql += columnlist[i] + " = '" + FormatString2DB(valuelist[i]) + "', ";
        }
        ssql = ssql.Remove(ssql.Length - 2);
        ssql += " WHERE " + idname + " = '" + idvalue + "'";

        List<object> emptyval = new List<object>();
        if (ExecuteWriteQuery(ssql, emptyval))
        {
            return true;
        }
        else
            return false;
    }

    static internal bool UpdateTableSQL(string tablename, string ssql)
    {
        using (OracleConnection con = new OracleConnection((OracleDB.ConnString)))
        {
            try
            {
                con.Open();
                OracleCommand updateCommand = new OracleCommand(ssql, con);
                updateCommand.ExecuteNonQuery();
                con.Close();
                return true;
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message, "Update Table");
                return false;
            }
            finally
            {
                con.Close();
                con.Dispose();
            }
        }
    }

    static internal string FormatString2DB(string val)
    {
        val = val.Replace("\\", "\\\\");
        val = val.Replace("\'", "\\'");
        return val;
    }
    #endregion

    #region "DELETE"
    static internal bool DeleteTable(string tablename, string columnname, string keyvalue)
    {
        // Open the connection using the connection string.
        using (OracleConnection con = new OracleConnection(OracleDB.ConnString))
        {
            try
            {
                string ssql = "DELETE FROM " + tablename + " WHERE " + columnname + " = '" + keyvalue + "'";

                con.Open();
                OracleCommand deleteCommand = new OracleCommand(ssql, con);
                deleteCommand.ExecuteNonQuery();
                con.Close();
                return true;
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message, "DeleteTable");
                return false;
            }
            finally
            {
                con.Close();
                con.Dispose();
            }
        }


    }
    #endregion
}

