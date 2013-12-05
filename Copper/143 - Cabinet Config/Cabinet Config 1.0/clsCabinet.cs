using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

using ADODB;
using Intergraph.GTechnology.Interfaces;
using Intergraph.GTechnology.API;
using System.Diagnostics;
using System.Text.RegularExpressions;


namespace NEPS.GTechnology.Cabinet
{
    class clsCabinet
    {
        public const int FRAC_FNO = 10300;
        public const int FRAC_CNO = 10301;
        public const int FRAU_CNO = 10302;
        
        public struct FRAU
        {
            public int CID;
            public string frau_name; // ESIDE, DSIDE, DSL-IN, DSL-OUT
            public string frau_position;
            public int frau_capacity; 
            public string frau_manufacturer;
            public string frau_status; // default to PLANNED
        }

        public SortedDictionary<int, FRAU> TheBlocks = new SortedDictionary<int, FRAU>();

        private IGTDataContext m_GTDataContext = null;

        
        #region "Delete FRAC"
        public void DeleteFRAC(List<int> deleted_FRAU, int FRAC_FID)
        {
            try
            {
                for (int i = 0; i < deleted_FRAU.Count; i++)
                {
                    if (deleted_FRAU[i] > -1)
                    {
                        DeleteFRAU(deleted_FRAU[i], FRAC_FID);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error while delete slot from database\r\n" + ex.Message);
            }
        }

        /// <summary>
        /// delete record from its main table and from ownership table
        /// we also delete childs of the records
        /// </summary>
        /// <param name="recFID">record FID</param>
        /// <param name="recFNO">record FNO</param>
        public void DeleteFRAU(int CID, int FRAC_FID)
        {
            try
            {
                int iR;
                string ssql = "DELETE FROM GC_ITFACE_BLK WHERE G3E_CID = " + CID.ToString();
                ssql += " AND G3E_FID = " + FRAC_FID.ToString();

                GTClassFactory.Create<IGTApplication>().DataContext.Execute(ssql, out iR, (int)ADODB.CommandTypeEnum.adCmdText);                
                GTClassFactory.Create<IGTApplication>().DataContext.Execute("COMMIT", out iR, (int)ADODB.CommandTypeEnum.adCmdText);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error while delete record from database\r\n" + ex.Message);
            }
        }
        #endregion

        #region "Insert Table"
        public int InsertFRAU(DataGridViewRow f, int FRAC_FID)
        {
            int id = NextSequence("GC_ITFACE_BLK_SEQ");
            int cid = LastCID(FRAC_FID) + 1;

            string ssql = "INSERT INTO GC_ITFACE_BLK (G3E_ID, G3E_FNO, G3E_FID, G3E_CNO, G3E_CID, ";
            ssql += "BLOCK_NAME, BLOCK_POSITION, BLOCK_SIZE, BLOCK_STATUS, BLOCK_MANUFACTURER) VALUES (";

            // add values to be inserted
            ssql += id.ToString() + "," + FRAC_FNO.ToString() + "," + FRAC_FID.ToString() + "," + FRAU_CNO.ToString() + "," + cid.ToString() + ",";
            ssql += "'" + CellValue(f.Cells[0].Value) + "','" 
                        + CellValue(f.Cells[1].Value) + "'," 
                        + ParseInt(CellValue(f.Cells[2].Value)) + ",'" 
                        + CellValue(f.Cells[3].Value) + "','" 
                        + CellValue(f.Cells[4].Value) + "')";

            // runs the SQL
            int iR;
            GTClassFactory.Create<IGTApplication>().DataContext.Execute(ssql, out iR, (int)ADODB.CommandTypeEnum.adCmdText);
            GTClassFactory.Create<IGTApplication>().DataContext.Execute("COMMIT", out iR, (int)ADODB.CommandTypeEnum.adCmdText);
            // return the FID of newly inserted record
            return cid;
        }

        private int NextSequence(string seq_name)
        {
            string ssql = "SELECT " + seq_name + ".NEXTVAL from dual";
            this.m_GTDataContext = GTClassFactory.Create<IGTApplication>().DataContext;
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

        private int LastCID(int FRAC_FID)
        {
            string ssql = "SELECT G3E_CID FROM GC_ITFACE_BLK WHERE G3E_FID = " + FRAC_FID.ToString();
            ssql += " ORDER BY G3E_CID";

            this.m_GTDataContext = GTClassFactory.Create<IGTApplication>().DataContext;
            ADODB.Recordset rs = new ADODB.Recordset();

            rs = GTClassFactory.Create<IGTApplication>().DataContext.OpenRecordset(ssql,
                ADODB.CursorTypeEnum.adOpenStatic, ADODB.LockTypeEnum.adLockOptimistic, 1);

            if (rs.RecordCount > 0)
            {
                rs.MoveLast();
                return (int.Parse(rs.Fields[0].Value.ToString()));
            }
            else
                return 0;
        }

        #endregion

        #region "Update Table"
        public void UpdateFRAU(DataGridViewRow f, int FRAC_FID)
        {
            string ssql = "UPDATE GC_ITFACE_BLK SET ";
            ssql += "BLOCK_NAME = '" + CellValue(f.Cells[0].Value) + "',";
            ssql += "BLOCK_POSITION = '" + CellValue(f.Cells[1].Value) + "',";
            ssql += "BLOCK_SIZE = " + ParseInt(CellValue(f.Cells[2].Value)) + ",";
            ssql += "BLOCK_STATUS = '" + CellValue(f.Cells[3].Value) + "',";
            ssql += "BLOCK_MANUFACTURER = '" + CellValue(f.Cells[4].Value) + "' ";
            ssql += "WHERE G3E_CID = " + f.Tag.ToString() + " AND G3E_FID = " + FRAC_FID.ToString();

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
        private int RecordExist(string tablename, string idname, string keyname, object keyval, string filter)
        {
            string ssql = "SELECT " + idname + " FROM " + tablename + " WHERE " + keyname + " = " +
                ((keyval is string) ? "'" + keyval + "'" : keyval.ToString());
            if (filter.Length > 0)
                ssql += " AND " + filter;

            this.m_GTDataContext = GTClassFactory.Create<IGTApplication>().DataContext;
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


        private int ParseInt(string val)
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

        private string CellValue(object val)
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

        #endregion
    }
}
