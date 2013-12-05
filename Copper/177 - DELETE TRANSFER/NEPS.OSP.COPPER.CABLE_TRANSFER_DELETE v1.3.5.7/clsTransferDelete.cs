/*
 * NEPS.OSP.COPPER.CABLE_TRANSFER_DELETE
 * class name : clsTransfer
 * 
 * develop by : m.zam 
 * version : 1.0
 * started : 23-JULY-2012
 * done on :
 * 
 */

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;

using Intergraph.GTechnology.Interfaces;
using Intergraph.GTechnology.API;

using ADODB;



namespace NEPS.OSP.COPPER.CABLE_TRANSFER_DELETE
{
    class clsTransferDelete
    {
        public short TR_FNO = 6100;

        public short LINE_FNO = 7000;
        public short LINE_CNO = 7001;
        public short LINE_GEO_CNO = 7010;
        public short LINE_DET_CNO = 7011;

        public short JOINT_FNO = 10800;
        public short JOINT_CNO = 10801;
        public short JOINT_GEO_CNO = 10820;
        public short JOINT_DET_CNO = 10821;

        public string m_transferType;
        public string m_EXC_ABB = "-";
        public string m_ITFACE_CODE = "-";
        public string m_RT_CODE = "-";
        public string m_CABLE_CODE = "";
        public string m_JOB_ID = "";


        /*
         * CNO 6101 --> GC_TRANSFER (Attribute)
         * CNO 6112 --> GC_TRANSFERLDR_L (Leader Line)
         * CNO 6120 --> GC_TRANSFER_S (Transfer Symbol)
         * CNO 6130 --> GC_TRANSFER_T (Transfer Text)
         */

        public IGTDDCKeyObject transferDDC;

        #region INITIAL - recipient and donor selection

        #region Select Transfer
        public bool isTransferSelected(IGTSelectedObjects selectedObj)
        {
            transferDDC = null;
            switch (selectedObj.FeatureCount)
            {
                case 1:     // one feature selected
                    foreach (IGTDDCKeyObject oDDC in selectedObj.GetObjects())
                        if (oDDC.FNO == TR_FNO)
                        {
                            transferDDC = oDDC;
                            return true;
                        }
                    return false;
                default: // more than one features selected
                    return false;
            }//switch
        }

        #endregion

        #endregion

        public void PerformDeleteTransfer()
        {
            try
            {
                GTClassFactory.Create<IGTApplication>().BeginWaitCursor();
                GTTransferDelete.m_oIGTTransactionManager.Begin("Cancel Cable Transfer");

                ADODB.Recordset rs = new ADODB.Recordset();
                rs = myUtil.ADODB_ExecuteQuery("SELECT * FROM GC_TRANSFER WHERE G3E_FID = " + transferDDC.FID);
                if (!rs.EOF)
                {
                    int donorFID = int.Parse(myUtil.rsField(rs, "TERMINATION_FID"));
                    int cableFID = int.Parse(myUtil.rsField(rs, "SRC_CABLE_FID"));
                    // get previous donor properties from upstream cable
                    GetDonorProperties(cableFID);
                    // delete transfer cable + rollback recipient to stub
                    RollbackDonorToRecipient(donorFID);
                    // rollback donor upstream cable from stub to line
                    DeleteDonorStub(cableFID);

                    ReconnectDonor(cableFID, donorFID);

                    RollbackDonors(donorFID); //101366363
                }
                rs.Close();
                rs = null;

                if (DeleteTransferObject(transferDDC.FID))
                {
                    GTTransferDelete.m_oIGTTransactionManager.Commit();
                    GTTransferDelete.m_oIGTTransactionManager.RefreshDatabaseChanges();
                    MessageBox.Show("Transfer already deleted");
                }
                else
                {
                    MessageBox.Show("Transfer canceled with error");
                    GTTransferDelete.m_oIGTTransactionManager.Rollback();
                }
            }
            catch (Exception ex)
            {
                GTTransferDelete.m_oIGTTransactionManager.Rollback();
                MessageBox.Show("Error delete transfer\r\n" + ex.Message);
            }
            GTClassFactory.Create<IGTApplication>().EndWaitCursor();
        }

        private void GetDonorProperties(int upstreamFID)
        {
            string ssql = "SELECT EXC_ABB, JOB_ID FROM GC_NETELEM WHERE G3E_FID = " + upstreamFID.ToString();

            ADODB.Recordset rs = new ADODB.Recordset();
            rs = GTTransferDelete.m_gtapp.DataContext.OpenRecordset(ssql,
            ADODB.CursorTypeEnum.adOpenStatic, ADODB.LockTypeEnum.adLockOptimistic, 1);

            m_EXC_ABB = myUtil.rsField(rs, "EXC_ABB");
            m_JOB_ID = myUtil.rsField(rs, "JOB_ID");

            rs = myUtil.ADODB_ExecuteQuery("SELECT CABLE_CLASS, CABLE_CODE, ITFACE_CODE, RT_CODE " +
                "FROM GC_CBL WHERE G3E_FID = " + upstreamFID.ToString());

            m_CABLE_CODE = myUtil.rsField(rs, "CABLE_CODE");
            if (myUtil.rsField(rs, "CABLE_CLASS").IndexOf("D-SIDE") > -1)
            {
                m_transferType = "TRANSFER D-SIDE";
                m_ITFACE_CODE = myUtil.rsField(rs, "ITFACE_CODE");
                m_RT_CODE = myUtil.rsField(rs, "RT_CODE");
            }
            else
            {
                m_transferType = "TRANSFER E-SIDE";
                m_ITFACE_CODE = "";
                m_RT_CODE = "";

            }
        }


        #region 2 : delete transfer cable and rollback recipient

        /// <summary>
        /// Get FID for upstream cable from selected cable IN joint
        /// </summary>
        /// <returns>cable FID</returns>
        private bool RollbackDonorToRecipient(int donorFID)
        {
            int iFID = 0;

            ADODB.Recordset rs = new ADODB.Recordset();
            rs = myUtil.ADODB_ExecuteQuery("SELECT * FROM GC_NR_CONNECT WHERE OUT_FID = " + donorFID.ToString());

            if (rs.EOF)
                return false;
            else
            {
                DeleteTransferCable(int.Parse(myUtil.rsField(rs, "G3E_FID")));
            }
            rs.Close();
            rs = null;

            return true;

        }

        private void DeleteTransferCable(int iFID)
        {
            IGTKeyObject cblFeature = GTTransferDelete.m_gtapp.DataContext.OpenFeature(LINE_FNO, iFID);
            DeleteFeatureFNO(cblFeature.FID, cblFeature.FNO);
        }

        private void RollbackRecipient(int jointFID)
        {

            #region Get Recipient Cable
            int cableFID = -1;
            ADODB.Recordset rs = new ADODB.Recordset();
            rs = myUtil.ADODB_ExecuteQuery("SELECT G3E_FID FROM GC_NR_CONNECT WHERE OUT_FID = " + jointFID.ToString());
            if (!rs.EOF)
                cableFID = int.Parse(myUtil.rsField(rs, "G3E_FID"));

            rs.Close();
            rs = null;
            #endregion

            #region Update Recipient Cable Class
            short iCNO = 7001;
            IGTKeyObject cblFeature;
            cblFeature = GTTransferDelete.m_gtapp.DataContext.OpenFeature(LINE_FNO, cableFID);
            cblFeature.Components.GetComponent(iCNO).Recordset.MoveLast();
            cblFeature.Components.GetComponent(iCNO).Recordset.Update("CUSAGE", "STUB");
            if (m_transferType == "TRANSFER D-SIDE")
                cblFeature.Components.GetComponent(iCNO).Recordset.Update("CABLE_CLASS", "STUB D-CABLE");
            else
                cblFeature.Components.GetComponent(iCNO).Recordset.Update("CABLE_CLASS", "STUB E-CABLE");
            #endregion

            #region Update Recipient Cable Joint
            iCNO = 10801;
            IGTKeyObject oJntFeature;
            oJntFeature = GTTransferDelete.m_gtapp.DataContext.OpenFeature(JOINT_FNO, jointFID);
            oJntFeature.Components.GetComponent(iCNO).Recordset.MoveLast();
            oJntFeature.Components.GetComponent(iCNO).Recordset.Update("JOINT_TYPE", "STRAIGHT");
            oJntFeature.Components.GetComponent(iCNO).Recordset.Update("SPLICE_CLASS",
                (m_transferType == "TRANSFER D-SIDE" ? "STUB D-SIDE" : "STUB E-SIDE"));

            #endregion

        }

        #endregion

        private void DeleteFeatureFNO(int FID, short FNO)
        {
            // addded 08-May-2013 : just to make sure it really delete from the table .. but sadly it still not...
            ADODB.Recordset rs = new ADODB.Recordset();
            string tables = " select co.g3e_table from g3e_component co, g3e_featurecomponent fc " +
                "where fc.g3e_cno=co.g3e_cno and fc.g3e_fno=" + FNO.ToString() + "  order by co.g3e_cno";
            rs = myUtil.ADODB_ExecuteQuery(tables);
            if (rs.RecordCount > 0)
            {
                rs.MoveFirst();
                while (!rs.EOF)
                {
                    myUtil.ADODB_ExecuteNonQuery("DELETE FROM " + rs.Fields[0].Value.ToString() + " WHERE G3E_FID = " + FID.ToString());
                    rs.MoveNext();
                }
            }
        }

        #region 3 : rollback donor
        private void ReconnectDonor(int cableFID, int jointFID)
        {
            IGTPolylineGeometry cblPoints = GTClassFactory.Create<IGTPolylineGeometry>();
            IGTPoint jntPoint = GTClassFactory.Create<IGTPoint>();

            #region  Read All Points From Upstream Cable
            string ssql =
            "SELECT GC_OSP_COP_VAL.EXTRACT_LINE_TO_VECTOR(G3E_GEOMETRY) AS XY FROM GC_CBL_L WHERE G3E_FID = " + cableFID;

            ADODB.Recordset rs = GTClassFactory.Create<IGTApplication>().DataContext.OpenRecordset(ssql,
                ADODB.CursorTypeEnum.adOpenStatic, ADODB.LockTypeEnum.adLockOptimistic, 1);

            if (!rs.EOF)
            {
                // get XY position
                string val = myUtil.CellValue(rs.Fields[0].Value).Trim();
                if (val.Length > 0)
                {
                    string[] XYs = val.Split('|');
                    for (int i = 0; i < XYs.GetUpperBound(0); i += 2)
                    {
                        IGTPoint p = GTClassFactory.Create<IGTPoint>();
                        p.X = double.Parse(XYs[i]);
                        p.Y = double.Parse(XYs[i + 1]);
                        cblPoints.Points.Add(p);
                    }
                }
            }
            rs.Close();
            rs = null;
            #endregion

            #region Move Upsream Last Point to Donor Joint
            int lastpoint = cblPoints.Points.Count - 1;
            cblPoints.Points[lastpoint] = GetDonorPosition(jointFID);
            #endregion

            #region Update Upstream Cable Geometry
            IGTKeyObject upstreamCable = GTTransferDelete.m_gtapp.DataContext.OpenFeature(LINE_FNO, cableFID);
            upstreamCable.Components.GetComponent(LINE_GEO_CNO).Recordset.MoveLast();
            upstreamCable.Components.GetComponent(LINE_GEO_CNO).Geometry = cblPoints;

            upstreamCable.Components.GetComponent(LINE_CNO).Recordset.MoveLast();
            if (m_transferType == "TRANSFER D-SIDE")
            {
                upstreamCable.Components.GetComponent(LINE_CNO).Recordset.Update("CUSAGE", "DISTRIBUTION");
                upstreamCable.Components.GetComponent(LINE_CNO).Recordset.Update("CABLE_CLASS", "D-CABLE");
            }
            else
            {
                upstreamCable.Components.GetComponent(LINE_CNO).Recordset.Update("CUSAGE", "MAIN");
                upstreamCable.Components.GetComponent(LINE_CNO).Recordset.Update("CABLE_CLASS", "E-CABLE");
            }

            upstreamCable.Components.GetComponent(51).Recordset.MoveLast();
            upstreamCable.Components.GetComponent(51).Recordset.Update("FEATURE_STATE", "ASB");

            #endregion

            UpdateNRConnectUpstream(cableFID, jointFID);
        }

        private void DeleteDonorStub(int cableFID)
        {
            int iFID = -1;

            ADODB.Recordset rs = new ADODB.Recordset();
            rs = myUtil.ADODB_ExecuteQuery("SELECT * FROM GC_NR_CONNECT WHERE G3E_FID = " + cableFID.ToString());

            if (!rs.EOF)
            {
                iFID = int.Parse(myUtil.rsField(rs, "OUT_FID"));
                IGTKeyObject jntFeature = GTTransferDelete.m_gtapp.DataContext.OpenFeature(JOINT_FNO, iFID);
                DeleteFeatureFNO(jntFeature.FID, jntFeature.FNO);
            }
        }

        private IGTPoint GetDonorPosition(int iFID)
        {
            IGTKeyObject donorJoint = GTTransferDelete.m_gtapp.DataContext.OpenFeature(JOINT_FNO, iFID);
            donorJoint.Components.GetComponent(JOINT_GEO_CNO).Recordset.MoveLast();
            return donorJoint.Components.GetComponent(JOINT_GEO_CNO).Geometry.FirstPoint;
        }

        private void UpdateNRConnectUpstream(int cableFID, int jointFID)
        {
            short iCNO = 53; // CNO for NR_CONNECT
            IGTKeyObject upstreamCable = GTTransferDelete.m_gtapp.DataContext.OpenFeature(LINE_FNO, cableFID);
            upstreamCable.Components.GetComponent(iCNO).Recordset.MoveLast();
            upstreamCable.Components.GetComponent(iCNO).Recordset.Update("OUT_FNO", JOINT_FNO);
            upstreamCable.Components.GetComponent(iCNO).Recordset.Update("OUT_FID", jointFID);
        }

        #endregion

        #region 4 : Update Downstream

        /// <summary>
        /// Update all downstreams cable and features connected to the donor joint
        /// </summary>
        /// <param name="jointFID">FID of donor joint</param>
        private void RollbackDonors(int jointFID)
        {
            try
            {
                IGTKeyObject jointDonor = GTTransferDelete.m_gtapp.DataContext.OpenFeature(JOINT_FNO, jointFID);
                jointDonor.Components.GetComponent(51).Recordset.Update("FEATURE_STATE", "ASB");

                int iFID = 0;

                string ssql = "SELECT G3E_FID FROM GC_NR_CONNECT WHERE IN_FID = " + jointFID.ToString();

                ADODB.Recordset rs = new ADODB.Recordset();
                rs = GTTransferDelete.m_gtapp.DataContext.OpenRecordset(ssql,
                ADODB.CursorTypeEnum.adOpenStatic, ADODB.LockTypeEnum.adLockOptimistic, 1);

                while (!rs.EOF)
                {
                    RollbackDonor(int.Parse(myUtil.rsField(rs, "G3E_FID")));
                    rs.MoveNext();
                }
                rs.Close();
                rs = null;


            }
            catch (Exception ex)
            {

            }
        }

        /// <summary>
        /// Update downstream cable and all features connected to it
        /// </summary>
        /// <param name="cableFID">Cable FID</param>
        private void RollbackDonor(int cableFID)
        {

            // create downstream trace table
            string trace_file = clsTrace.TraceDownstream(cableFID); //101731509
            // query the trace table
            string ssql =
            "SELECT B.G3E_FID, B.G3E_FNO, C.IN_FID, C.IN_FNO, C.OUT_FID, C.OUT_FNO  FROM TRACEID A, TRACERESULT B, GC_NR_CONNECT C " +
            "WHERE A.G3E_ID = B.G3E_TNO AND B.G3E_FID = C.G3E_FID AND A.G3E_NAME = '" + trace_file + "'";

            ADODB.Recordset rs = new ADODB.Recordset();
            rs = GTClassFactory.Create<IGTApplication>().DataContext.OpenRecordset(ssql,
                ADODB.CursorTypeEnum.adOpenStatic, ADODB.LockTypeEnum.adLockOptimistic, 1);

            int iFID = 0;
            short iFNO = 0;
            int cFID = 0;
            short cFNO = 0;


            while (!rs.EOF)
            {
                iFNO = short.Parse(myUtil.rsField(rs, "OUT_FNO"));
                iFID = int.Parse(myUtil.rsField(rs, "OUT_FID"));
                cFNO = short.Parse(myUtil.rsField(rs, "G3E_FNO"));
                cFID = int.Parse(myUtil.rsField(rs, "G3E_FID"));

                ChangeEXCHANGE(cFID, cFNO);
                ChangeEXCHANGE(iFID, iFNO);

                if (m_transferType == "TRANSFER D-SIDE")
                    Rollback_D_CABLE(cFID);
                else
                    Rollback_E_CABLE(cFID);


                switch (iFNO)
                {
                    case 6200: ChangeCableCode(iFID, iFNO, "GC_PDDP"); break;
                    case 6300: ChangeCableCode(iFID, iFNO, "GC_DDP"); break;
                    // attachment
                    // to be explore ???
                    case 6400: ChangeCableCode(iFID, iFNO, "GC_CONTGAUGE"); break;
                    case 6500: ChangeCableCode(iFID, iFNO, "GC_CONTALARM"); break;
                    case 6600: ChangeCableCode(iFID, iFNO, "GC_GASSEAL"); break;
                    case 6700: ChangeCableCode(iFID, iFNO, "GC_TESTPNT"); break;
                    case 6800: ChangeCableCode(iFID, iFNO, "GC_LDCOIL"); break;
                    case 6900: ChangeCableCode(iFID, iFNO, "GC_TRNSDCR"); break;

                    case 10300: ChangeCableCode(iFID, iFNO, "GC_ITFACE"); break;
                    case 10800: ChangeITFACE(iFID, iFNO, "GC_SPLICE"); break;

                    case 13000: ChangeITFACE(iFID, iFNO, "GC_DP"); break;
                    case 13100: ChangeITFACE(iFID, iFNO, "GC_PDP"); break;
                    case 13200: ChangeITFACE(iFID, iFNO, "GC_IDF"); break;
                }
                rs.MoveNext();
            }
            rs.Close();
            rs = null;
            // delete the trace table after done
            clsTrace.DeleteTrace(trace_file);
        }

        private void ChangeEXCHANGE(int iFID, short iFNO)
        {
            try
            {
                myUtil.ADODB_ExecuteNonQuery
                    ("UPDATE GC_NETELEM SET EXC_ABB = '" + m_EXC_ABB + "', JOB_ID = '" + m_JOB_ID +
                    "', FEATURE_STATE = 'ASB' WHERE G3E_FID = " + iFID.ToString());
            }
            catch (Exception ex)
            {
                Debug.WriteLine("ERROR on CHG EXC_ABB [" + iFNO.ToString() + "][" + iFID.ToString() + "] : " + ex.Message);
            }
        }
        private void ChangeITFACE(int iFID, short iFNO, string tablename)
        {
            if (m_ITFACE_CODE != null && m_ITFACE_CODE.Length > 0)
                myUtil.ADODB_ExecuteNonQuery("UPDATE " + tablename + " SET ITFACE_CODE = '" + m_ITFACE_CODE +
                    "', RT_CODE = '', CABLE_CODE = '" + m_CABLE_CODE + "' WHERE G3E_FID = " + iFID.ToString());
            else
                myUtil.ADODB_ExecuteNonQuery("UPDATE " + tablename + " SET RT_CODE = '" + m_RT_CODE +
                    "', ITFACE_CODE = '', CABLE_CODE = '" + m_CABLE_CODE + "' WHERE G3E_FID = " + iFID.ToString());
        }
        private void ChangeCableCode(int iFID, short iFNO, string tablename)
        {
            myUtil.ADODB_ExecuteNonQuery("UPDATE " + tablename + " SET CABLE_CODE = '" + m_CABLE_CODE + "'" +
                " WHERE G3E_FID = " + iFID.ToString());
        }


        #endregion

        #region 5 : Transfer Rollback Feature 20-07-2012

        /*
         * CNO 6101 --> GC_TRANSFER (Attribute)
         * CNO 6112 --> GC_TRANSFERLDR_L (Leader Line)
         * CNO 6120 --> GC_TRANSFER_S (Transfer Symbol)
         * CNO 6130 --> GC_TRANSFER_T (Transfer Text)
         */
        public bool DeleteTransferObject(int iFID)
        {
            try
            {
                IGTKeyObject TR_Feature = GTTransferDelete.m_gtapp.DataContext.OpenFeature(TR_FNO, iFID);
                DeleteFeatureFNO(TR_Feature.FID, TR_Feature.FNO);
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error detected\r\n" + ex.Message, "Transfer Rollback Object");
                return false;
            }
            finally
            {
            }
        }

        #endregion

        #region COUNT - 20-07-2012

        private bool Rollback_E_CABLE(int iFID)
        {
            // count_transfer = T,count_annotation
            try
            {
                string anno = myUtil.GetFieldValue("GC_CBL", "COUNT_TRANSFER", iFID);

                myUtil.ADODB_ExecuteNonQuery("DELETE FROM GC_CBLCNT_BL_T WHERE G3E_FID = " + iFID);
                myUtil.ADODB_ExecuteNonQuery("DELETE FROM DGC_CBLCNT_BL_T WHERE G3E_FID = " + iFID);

                if (anno.Length > 0 && anno != "(T)")
                {
                    string paircount = anno.Substring(anno.LastIndexOf(',') + 1).Trim(); // remove the 'T'
                    paircount = paircount.Replace(Environment.NewLine, "#");
                    string[] counts = paircount.Split('#');

                    myUtil.ADODB_ExecuteNonQuery("UPDATE GC_CBL SET CABLE_CODE = '" + m_CABLE_CODE + "', " +
                     "COUNT_ANNOTATION = '" + paircount + "', COUNT_TRANSFER = '' WHERE G3E_FID = " + iFID.ToString());

                    for (int CID = 0; CID < counts.Length; CID++)
                    {
                        string[] lbl = counts[CID].Split('/');
                        string vhi = lbl[3].Substring(lbl[3].IndexOf('-') + 1);
                        string vlo = lbl[3].Remove(lbl[3].IndexOf('-'));

                        string ehi = lbl[4].Substring(lbl[4].IndexOf('-') + 1);
                        string elo = lbl[4].Remove(lbl[4].IndexOf('-'));

                        myUtil.ADODB_ExecuteNonQuery("UPDATE GC_COUNT SET CURRENT_DESIGNATION = '" + m_CABLE_CODE + "'" +
                            ",COUNT_ANNOTATION = '" + counts[CID].Trim() + "'" +
                            ",MDF_UNIT = " + lbl[1] +
                            ",VERTICAL_UNIT = " + lbl[2] +
                            ",PROPOSED_DESIGNATION = '" + m_CABLE_CODE + "'" +
                            ",PROPOSED_HIGH = " + ehi + ", PROPOSED_LOW = " + elo +
                            ",CURRENT_HIGH = " + ehi + ", CURRENT_LOW = " + elo +
                            ",EHI_PR = " + ehi + ", ELO_PR = " + elo +
                            ",HIGH_PORT = " + vhi + ", LOW_PORT = " + vlo +
                            "WHERE G3E_FID = " + iFID.ToString() + " AND G3E_CID = " + (CID + 1));
                    }
                    return true;
                }
                else
                {
                    myUtil.ADODB_ExecuteNonQuery("UPDATE GC_CBL SET CABLE_CODE = '" + m_CABLE_CODE + "', " +
                     "COUNT_TRANSFER = '' WHERE G3E_FID = " + iFID.ToString());
                    return true;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return false;
            }

        }
        private string Rollback_D_CABLE(int iFID)
        {
            // count_transfer = T [dp_num],count_annotation
            try
            {
                string anno = myUtil.GetFieldValue("GC_CBL", "COUNT_TRANSFER", iFID);

                myUtil.ADODB_ExecuteNonQuery("DELETE FROM GC_CBLCNT_BL_T WHERE G3E_FID = " + iFID);
                myUtil.ADODB_ExecuteNonQuery("DELETE FROM DGC_CBLCNT_BL_T WHERE G3E_FID = " + iFID);

                if (anno.Length > 0 && anno != "(T)")
                {
                    string prefix = anno.Remove(anno.IndexOf(','));
                    string dp_num = prefix.Substring(3, prefix.Length - 4);
                    string paircount = anno.Substring(anno.LastIndexOf(',') + 1);
                    string hi = paircount.Substring(paircount.IndexOf('-') + 1);
                    string lo = paircount.Remove(paircount.IndexOf('-'));
                    if (m_ITFACE_CODE != null && m_ITFACE_CODE.Length > 0)
                        myUtil.ADODB_ExecuteNonQuery("UPDATE GC_CBL SET ITFACE_CODE = '" + m_ITFACE_CODE + "', RT_CODE = '', CABLE_CODE = '" + m_CABLE_CODE + "', " +
                            "COUNT_ANNOTATION = '" + paircount + "', COUNT_TRANSFER = '' WHERE G3E_FID = " + iFID.ToString());
                    else
                        myUtil.ADODB_ExecuteNonQuery("UPDATE GC_CBL SET ITFACE_CODE = '', RT_CODE = '" + m_RT_CODE + "', CABLE_CODE = '" + m_CABLE_CODE + "', " +
                         "COUNT_ANNOTATION = '" + paircount + "', COUNT_TRANSFER = '' WHERE G3E_FID = " + iFID.ToString());

                    myUtil.ADODB_ExecuteNonQuery("UPDATE GC_COUNT SET CURRENT_DESIGNATION = '" + m_CABLE_CODE + "'," +
                        "PROPOSED_DESIGNATION = '" + m_CABLE_CODE + "'," +
                        "PROPOSED_HIGH = '" + hi + "', PROPOSED_LOW = '" + lo + "'," +
                        "CURRENT_HIGH = '" + hi + "', CURRENT_LOW = '" + lo + "' " +
                        "WHERE G3E_FID = " + iFID.ToString());

                    return dp_num;
                }
                else
                {
                    if (m_ITFACE_CODE != null && m_ITFACE_CODE.Length > 0)
                        myUtil.ADODB_ExecuteNonQuery("UPDATE GC_CBL SET ITFACE_CODE = '" + m_ITFACE_CODE + "',RT_CODE = '', CABLE_CODE = '" + m_CABLE_CODE + "', " +
                        "COUNT_ANNOTATION = '', COUNT_TRANSFER = '' WHERE G3E_FID = " + iFID.ToString());
                    else
                        myUtil.ADODB_ExecuteNonQuery("UPDATE GC_CBL SET ITFACE_CODE = '', RT_CODE = '" + m_RT_CODE + "', CABLE_CODE = '" + m_CABLE_CODE + "', " +
                        "COUNT_ANNOTATION = '', COUNT_TRANSFER = '' WHERE G3E_FID = " + iFID.ToString());
                    return "";
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return "";
            }
        }
        #endregion


    }
}
