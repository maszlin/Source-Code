using System;
using System.Collections.Generic;
using System.Web;
using System.Configuration;
using System.Data;
using System.Data.OleDb;

namespace NEPS.GTechnology.AssigJob
{

   
    public class UtilityDb : IDisposable
    {


        // Data insertion and update mode
        public OleDbConnection connection;
        public OleDbDataAdapter dataAdapter;
        public DataSet dataSet;
        public OleDbTransaction transaction;

        public void OpenConnection(string SID, string username, string password)
        {
            string connStr = GetConnectionString(SID, username, password);
            connection = new OleDbConnection(connStr);
            connection.Open();
        }

        public void CreateTransaction()
        {
            transaction = connection.BeginTransaction();
        }

        public void Commit()
        {
            if (transaction != null)
                transaction.Commit();
        }

        public void Rollback()
        {
            if (transaction != null)
                transaction.Rollback();
        }

        public void Close()
        {
            if (connection == null || connection.State != ConnectionState.Open)
                return;

            connection.Close();
        }

        public static void ResetTable(string tableName, OleDbConnection conn)
        {
            UtilityDb.ExecuteSql("DELETE FROM " + tableName, conn);

            try
            {
                UtilityDb.ExecuteSql("DBCC CHECKIDENT ('" + tableName + "', reseed, 0)", conn);
            }
            catch (Exception)
            {


            }

        }

        public void PrepareInsert(string tableName)
        {
            string sql = "SELECT * FROM " + tableName;
            dataAdapter = new OleDbDataAdapter(sql, connection);
            OleDbCommandBuilder cb = new OleDbCommandBuilder(dataAdapter);

            
            dataSet = new DataSet();

            dataAdapter.SelectCommand.Transaction = transaction;

            dataAdapter.Fill(dataSet);

         
        }

        public DataRow Insert(DataRow row)
        {
            DataTable table = dataSet.Tables[0];
            if (row != null)
                table.Rows.Add(row);

            DataRow output = table.NewRow();
            return output;
        }

        public void EndInsert()
        {
            dataAdapter.Update(dataSet);
        }

        public void EndUpdate()
        {
            dataAdapter.Update(dataSet);
        }


        // static functions
        public static string GetConnectionString(string SID, string username, string password)
        {
            string connectionString= string.Format("Provider=MSDAORA;Data Source={0};Persist Security Info=True;Password={1};User ID={2}",
                SID, password, username);

           return connectionString;
        }

   
        public static OleDbConnection GetConnection(string SID, string username, string password)
        {
            try
            {
                int i = 4;
                OleDbConnection output = new OleDbConnection(GetConnectionString(SID, username, password));
                output.Open();
                return output;
            }
            catch (Exception ex)
            {
                throw;
            }
          
        }

    
        public static OleDbDataReader GetDataReader(string sqlQuery, OleDbConnection connection)
        {
            if (connection == null)
                return null;

            if (connection.State != System.Data.ConnectionState.Open)
                connection.Open();

            OleDbCommand command = new OleDbCommand(sqlQuery, connection);
            OleDbDataReader output = command.ExecuteReader();
            return output;
        }

        public static void ExecuteSql(string sql, OleDbConnection conn)
        {
            OleDbCommand command = new OleDbCommand(sql, conn);
            command.ExecuteNonQuery();
        }

        public static object ExecuteScalar(string sql, OleDbConnection conn)
        {
            OleDbCommand command = new OleDbCommand(sql, conn);
            return command.ExecuteScalar();
        }

        public static string FixQuotes(string original)
        {
            string fixedString = original;
            fixedString =  fixedString.Replace("'", "''");
            return fixedString;

        }

        #region IDisposable Members

        public void Dispose()
        {
            Close();
        }

        #endregion



        public static string InClause(List<int> ids)
        {
            string output = "";
            for (int i = 0; i < ids.Count; i++)
            {
                int id = ids[i];
                if (i > 0)
                    output += ",";
                output += id.ToString();
            }
            output = " (" + output + ") ";
            return output;
        }

        public void PrepareUpdate(string sql)
        {
            dataAdapter = new OleDbDataAdapter(sql, connection);
            OleDbCommandBuilder cb = new OleDbCommandBuilder(dataAdapter);
            dataSet = new DataSet();
            dataAdapter.Fill(dataSet);
        }

        public DataRow FindRowFromID(int id, string idField)
        {
            DataTable table = dataSet.Tables[0];
            foreach (DataRow row in table.Rows)
            {
                if (Convert.ToInt32(row[idField]) == id)
                    return row;
            }
            return null;

        }

        public static string InClause(List<string> items)
        {
            string output = "";
            for (int i = 0; i < items.Count; i++)
            {
                string item = items[i];
                if (i > 0)
                    output += ",";
                output += "'" + item + "'";
            }
            output = " (" + output + ") ";
            return output;
        }


        internal static void SetNullableRow(DataRow row, string fieldName, object value)
        {
            if (value == null)
                row[fieldName] = DBNull.Value;
            else
                row[fieldName] = value;
        }

    


     
    }
}
