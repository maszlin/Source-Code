using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

using ADODB;
using Intergraph.GTechnology.Interfaces;
using Intergraph.GTechnology.API;
using System.Diagnostics;
using System.Text.RegularExpressions;


namespace NEPS.GTechnology.MSANConfig
{
    class clsMSAN
    {
      

        private int MSAN_FID = -1;
        private int MSAN_FNO= -1;
        public string JOB_ID = "";
        public string EXC_ABB = "";
        private string g_RackNo = "00";
        private string g_FrameNIS = "00";
        private MSANSlot g_slot = new MSANSlot();

        public struct MSANSlot
        {
            public int slotFID;
            public int cardFID;
            public bool inuse;
            public string slot_no;
            public string NIS;
            public string type; // card type = card name : ref - meeting with azman sulong on 13-Apr-2012
            public string model;
            public string service; // service card - added base on template given by azman sulong on 11-May-2012
            public string status;
            public int port_num;
            public string port_lo;
            public string port_hi;
            public string plant_unit;
            public string manufacturer;
        }

        public struct MSANFrame
        {
            public string rack_no;
            public string frame_no;
            public string frame_NIS;
            public string manufacturer;
            public SortedDictionary<string, MSANSlot> frame_slot;
        }

        public SortedDictionary<string, MSANFrame> MSANCabinet = new SortedDictionary<string, MSANFrame>();

        private IGTDataContext m_GTDataContext = null;

        #region "Select Table"
        public bool SelectMSAN(int MSANFID, int MSANFNO,string rack_no, string jobID, string exc_abb)
        {
            MSANCabinet.Clear();
            MSAN_FID = MSANFID;
            MSAN_FNO = MSANFNO;
            JOB_ID = jobID;
            EXC_ABB = exc_abb;
            g_RackNo = rack_no;

            try
            {
                ReadChilds(MSANFID); // read all shelfs own by MSAN                                       
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        private void ReadChilds(int parentFID)
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
                    Debug.WriteLine("parentFID : " + parentFID + " - childFNO : " + childFNO + " childFID : " + childFID);
                    switch (childFNO)
                    {
                        case Utilities.SHELF_FNO: ReadFrame(childFID); break;
                        case Utilities.SLOT_FNO: ReadSlot(childFID); break;
                        case Utilities.CARD_FNO: ReadCard(childFID); break;
                    }
                    rs.MoveNext();
                }
                rs.Close();
                rs = null;

            }
        }

        private void ReadFrame(int recFID)
        {
            try
            {
                string ssql = "SELECT * FROM GC_FSHELF WHERE G3E_FID = " + recFID.ToString();
                this.m_GTDataContext = GTClassFactory.Create<IGTApplication>().DataContext;
                ADODB.Recordset rs = new ADODB.Recordset();
                rs = GTClassFactory.Create<IGTApplication>().DataContext.OpenRecordset(ssql,
                    ADODB.CursorTypeEnum.adOpenStatic, ADODB.LockTypeEnum.adLockOptimistic, 1);

                if (rs.RecordCount == 0)
                {
                    ShowNoRecordWarning(ssql);
                }
                else
                {
                    rs.MoveFirst();

                    MSANFrame f = new MSANFrame();
                    f.frame_NIS = rs.Fields["FRAME_NIS"].Value.ToString();
                    f.frame_no = rs.Fields["FRAME_NO"].Value.ToString();
                    f.manufacturer = rs.Fields["MANUFACTURER"].Value.ToString();
                    f.rack_no = g_RackNo;
                    f.frame_slot = new SortedDictionary<string, MSANSlot>(StringComparer.CurrentCultureIgnoreCase);
                    //new clsSlotSorter());

                    if (!MSANCabinet.ContainsKey(f.frame_NIS))
                        MSANCabinet.Add(f.frame_NIS, f);

                    else
                        MSANCabinet[f.frame_NIS] = f;

                    g_FrameNIS = f.frame_NIS;
                    ReadChilds(recFID); // read all slots own by the shelf
                }            
                    
                rs.Close();
                rs = null;
            }
            catch { }
        }

        private void ReadSlot(int recFID)
        {
            try
            {
                string ssql = "SELECT * FROM GC_FSLOT WHERE G3E_FID = " + recFID.ToString() + " ORDER BY SLOT_NIS";
                this.m_GTDataContext = GTClassFactory.Create<IGTApplication>().DataContext;
                ADODB.Recordset rs = new ADODB.Recordset();
                rs = GTClassFactory.Create<IGTApplication>().DataContext.OpenRecordset(ssql,
                    ADODB.CursorTypeEnum.adOpenStatic, ADODB.LockTypeEnum.adLockOptimistic, 1);

                if (rs.RecordCount == 0)
                {
                    ShowNoRecordWarning(ssql);
                }
                else
                {
                    rs.MoveFirst();

                    g_slot = new MSANSlot();
                    g_slot.slot_no = rs.Fields["SLOT_NO"].Value.ToString();
                    g_slot.NIS = rs.Fields["SLOT_NIS"].Value.ToString();
                    g_slot.slotFID = ParseInt(rs.Fields["G3E_FID"].Value.ToString());
                    if (rs.Fields["SLOT_USE"].Value.ToString() == "1")
                        g_slot.inuse = true;
                    else
                        g_slot.inuse = false;

                    ReadChilds(recFID);
                }
                rs.Close();
                rs = null;
            }
            catch { }

        }


        private void ReadCard(int recFID)
        {
            try
            {
                string ssql = "SELECT fc.*,net.min_material FROM GC_FCARD fc, GC_NETELEM net  WHERE fc.G3E_FID=net.g3e_fid and  fc.G3E_FID= "
                    + recFID.ToString();
                this.m_GTDataContext = GTClassFactory.Create<IGTApplication>().DataContext;
                ADODB.Recordset rs = new ADODB.Recordset();
                rs = GTClassFactory.Create<IGTApplication>().DataContext.OpenRecordset(ssql,
                    ADODB.CursorTypeEnum.adOpenStatic, ADODB.LockTypeEnum.adLockOptimistic, 1);

                if (rs.RecordCount == 0)
                {
                    ShowNoRecordWarning(ssql);
                }
                else
                {
                    rs.MoveFirst();

                    g_slot.inuse = true;
                    g_slot.cardFID = recFID;
                    g_slot.model = rs.Fields["MODEL"].Value.ToString();
                    g_slot.type = rs.Fields["CARD_TYPE"].Value.ToString();
                    g_slot.plant_unit = rs.Fields["MIN_MATERIAL"].Value.ToString();
                    g_slot.status = rs.Fields["CARD_STATUS"].Value.ToString();
                    g_slot.port_num = 0; // ParseInt(rs.Fields["NUM_PORTS"].Value.ToString());
                    g_slot.port_hi = rs.Fields["PORT_HI"].Value.ToString();
                    g_slot.port_lo = rs.Fields["PORT_LO"].Value.ToString();
                    g_slot.manufacturer = rs.Fields["MANUFACTURER"].Value.ToString();
                    if (MSANCabinet[g_FrameNIS].frame_slot.ContainsKey(g_slot.NIS))
                        MSANCabinet[g_FrameNIS].frame_slot[g_slot.NIS] = g_slot;

                    else
                        MSANCabinet[g_FrameNIS].frame_slot.Add(g_slot.NIS, g_slot);
                }
                    

                rs.Close();
                rs = null;
            }
            catch { }

        }

        private static void ShowNoRecordWarning(string sql)
        {
            MessageBox.Show("Caution: no records found. " + sql);
        }


        #endregion

        #region "Delete MSAN"
        public void DeleteMSAN_Frame(List<string> deleted_frame, int MSANFID)
        {
            try
            {
                int shelfFID;
                for (int i = 0; i < deleted_frame.Count; i++)
                {
                    shelfFID = RecordExist("GC_FSHELF", "G3E_FID", "FRAME_NIS", 
                        deleted_frame[i], "MSAN_FID = " + MSANFID);
                    if (shelfFID > -1)
                        DeleteRecord(shelfFID, Utilities.SHELF_FNO);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error while delete shelf from database\r\n" + ex.Message);
            }
        }

        public void DeleteMSAN_Slot(List<int> deleted_slot)
        {
            try
            {
                for (int i = 0; i < deleted_slot.Count; i++)
                    DeleteRecord(deleted_slot[i], Utilities.SLOT_FNO);
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
        public void DeleteRecord(int recFID, int recFNO)
        {
            try
            {


                int iR;
                string sSql = "select g3e_table from g3e_component co, g3e_featurecomponent fc " +
                              "where fc.g3e_cno=co.g3e_cno and fc.g3e_fno=" + recFNO.ToString();

                Recordset rsComp = GTClassFactory.Create<IGTApplication>().DataContext.Execute(sSql, out iR, (int)CommandTypeEnum.adCmdText, null);
                if (rsComp != null)
                {
                    while (!rsComp.EOF)
                    {
                        if (rsComp.Fields[0].Value.ToString() != "GC_OWNERSHIP")
                        {
                            sSql = "DELETE FROM " + rsComp.Fields[0].Value.ToString() + " WHERE G3E_FID = " + recFID.ToString();
                            GTClassFactory.Create<IGTApplication>().DataContext.Execute(sSql, out iR, (int)ADODB.CommandTypeEnum.adCmdText);

                        }
                        rsComp.MoveNext();
                    }

                }
                rsComp = null;

                DeleteChild(recFID);

                sSql = "DELETE FROM GC_OWNERSHIP WHERE G3E_FID = " + recFID.ToString();
                GTClassFactory.Create<IGTApplication>().DataContext.Execute(sSql, out iR, (int)ADODB.CommandTypeEnum.adCmdText);

                GTClassFactory.Create<IGTApplication>().DataContext.Execute("COMMIT", out iR, (int)ADODB.CommandTypeEnum.adCmdText);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error while delete record from database\r\n" + ex.Message);
            }
        }

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
        #endregion

        #region "Save MSAN"
        public bool SaveMSAN()
        {
            try
            {
                int MSANID = MSAN_OwnershipID(MSAN_FID, MSAN_FNO);

                int frameFID;
                int slotFID;
                int cardFID;

                string[] col_card = { "SLOT_NIS", "CARD_TYPE", "MODEL", "CARD_STATUS", "PORT_LO", "PORT_HI", "MANUFACTURER" };  

                string[] col = { };
                object[] val = { };

                foreach (MSANFrame frame in MSANCabinet.Values)
                {
                    col = new string[] { "FRAME_NO", "FRAME_NIS" ,"MANUFACTURER"};
                    
                    val = new object[] { frame.frame_no, frame.frame_NIS,frame.manufacturer};
                    frameFID = SaveRecords("GC_FSHELF", col, val, Utilities.SHELF_FNO, MSAN_FID,MSAN_FNO , "-");

                    foreach (MSANSlot slot in frame.frame_slot.Values)
                    {
                        col = new string[] { "SLOT_NIS", "SLOT_NO", "SLOT_USE" };
                        val = new object[] { slot.NIS, slot.slot_no, (slot.inuse ? 1 : 0) };
                        slotFID = SaveRecords("GC_FSLOT", col, val, Utilities.SLOT_FNO, frameFID, Utilities.SHELF_FNO, "-");

                        val = new object[] { slot.NIS, slot.type, slot.model, slot.status, slot.port_lo, slot.port_hi,slot.manufacturer };
                        SaveRecords("GC_FCARD", col_card, val, Utilities.CARD_FNO, slotFID, Utilities.SLOT_FNO, slot.plant_unit);
                       
                        
                    }
                }
                return true;
            }
            catch (Exception ex)
            {

                MessageBox.Show("Error saving record to database\r\n" + ex.Message);
                return false;
            }
        }

        private int SaveRecord(string tablename, string recName, object recValue, int recFNO, int parentFID, int parentFNO, string min_mat)
        {
            string[] col = { recName };
            object[] val = { recValue };
            return SaveRecords(tablename, col, val, recFNO, parentFID, parentFNO, min_mat);
        }

        private int SaveRecords(string tablename, string[] col, object[] val, int recFNO, int parentFID, int parentFNO, string min_mat)
        {
            int recCNO = recFNO + 1;

            // read recFID if record already exist
            int recFID = RecordExist(tablename, "G3E_FID", col[0], val[0].ToString(), "MSAN_FID = " + MSAN_FID.ToString());

            if (recFID == -1) // insert new record and add ownership if not exist
            {
                recFID = InsertRecord(tablename, col, val, recFNO, recCNO, parentFID);
                AddOwnership(recFNO, recFID, parentFNO, parentFID);
                AddNetelem(recFNO, recFID, min_mat);
            }
            else
            {
                UpdateRecord(tablename, col, val, recFID);
                UpdateNetelem(recFNO, recFID, min_mat);
            }
            return recFID;
        }
        #endregion

        #region "Insert Table"
        private int InsertRecord(string tablename, string[] col, object[] val, int FNO, int CNO, int parentFID)
        {
            int id = NextSequence(tablename.ToUpper() + "_SEQ");
            int fid = NextSequence("G3E_FID_SEQ");

            string ssql = "INSERT INTO " + tablename + " (G3E_ID, G3E_FNO, G3E_FID, G3E_CNO, G3E_CID, MSAN_FID, ";
            // insert columns name to be inserted
            for (int i = 0; i < col.Length; i++) ssql += col[i] + ", ";

            // add values to be inserted
            ssql = ssql.Remove(ssql.Length - 2) + ") VALUES (" +
                id.ToString() + "," + FNO.ToString() + "," + fid.ToString() + "," + CNO.ToString() + ", 1, " + MSAN_FID.ToString() + ", ";

            for (int i = 0; i < val.Length; i++)
            {
                if (val[i] is string)
                    ssql += "'" + val[i] + "', ";
                else
                    ssql += val[i] + ", ";
            }
            // replace last ',' with ')'
            ssql = ssql.Remove(ssql.Length - 2) + ")";
            // runs the SQL
            int iR;
            GTClassFactory.Create<IGTApplication>().DataContext.Execute(ssql, out iR, (int)ADODB.CommandTypeEnum.adCmdText);
            GTClassFactory.Create<IGTApplication>().DataContext.Execute("COMMIT", out iR, (int)ADODB.CommandTypeEnum.adCmdText);
            // return the FID of newly inserted record
            return fid;
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

        private void AddOwnership(int childFNO, int childFID, int parentFNO, int parentFID)
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

        private int GetParentID(int FID)
        {
            return RecordExist("GC_OWNERSHIP", "G3E_ID", "G3E_FID", FID.ToString(), "");
        }

        private void AddNetelem(int recFNO, int recFID, string min_material)
        {
            int id = NextSequence("GC_NETELEM_SEQ");           
            string ssql = "INSERT INTO GC_NETELEM (G3E_ID, G3E_FNO, G3E_FID, G3E_CNO, G3E_CID, ";
            ssql += "JOB_ID, JOB_STATE, EXC_ABB, FEATURE_STATE, OWNERSHIP, YEAR_PLACED, MIN_MATERIAL) ";
            ssql += "VALUES (" + id.ToString() + "," + recFNO.ToString() + "," + recFID.ToString() + ",51,1," +
                 "'" + JOB_ID + "','PROPOSED','" + EXC_ABB.Trim() + "','PPF','TM','" + DateTime.Now.Year.ToString() + "','"+min_material+"')";

            int iR;
            GTClassFactory.Create<IGTApplication>().DataContext.Execute(ssql, out iR, (int)ADODB.CommandTypeEnum.adCmdText);
            GTClassFactory.Create<IGTApplication>().DataContext.Execute("COMMIT", out iR, (int)ADODB.CommandTypeEnum.adCmdText);
        }

        private void UpdateNetelem(int recFNO, int recFID, string min_material)
        {
            if (min_material != "-")
            {
                string ssql = "UPDATE GC_NETELEM set MIN_MATERIAL='" + min_material + "'" +
                " where g3e_fno=" + recFNO.ToString() + " and g3e_fid=" + recFID.ToString();
                int iR;
                GTClassFactory.Create<IGTApplication>().DataContext.Execute(ssql, out iR, (int)ADODB.CommandTypeEnum.adCmdText);
                GTClassFactory.Create<IGTApplication>().DataContext.Execute("COMMIT", out iR, (int)ADODB.CommandTypeEnum.adCmdText);
            }
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
        /// read MSAN ownership id from GC_OWNERSHIP
        /// if not yet define, add MSAN to GC_OWNERSHIP
        /// </summary>
        /// <param name="FID">MSAN FID</param>
        /// <returns>MSAN ownership ID</returns>
        private int MSAN_OwnershipID(int FID, int FNO)
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
            }
            return id;
        }

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
                Debug.WriteLine("FOUND : " + ssql);
                return (int.Parse(rs.Fields[0].Value.ToString()));
            }
            else
            {
                Debug.WriteLine("NOT FOUND : " + ssql);
                return -1;
            }
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
        #endregion
    }

    public class clsSlotSorter : IComparer
    {
        // Initialize the CaseInsensitiveComparer object
        private CaseInsensitiveComparer ObjectCompare;
        public clsSlotSorter()
        {
            ObjectCompare = new CaseInsensitiveComparer();
        }
        public int Compare(object x, object y)
        {
            clsMSAN.MSANSlot slotx = (clsMSAN.MSANSlot)x;
            clsMSAN.MSANSlot sloty = (clsMSAN.MSANSlot)y;

            int compareResult;
            compareResult = ObjectCompare.Compare(slotx.NIS, sloty.NIS);
            return compareResult;
        }
    }

}
