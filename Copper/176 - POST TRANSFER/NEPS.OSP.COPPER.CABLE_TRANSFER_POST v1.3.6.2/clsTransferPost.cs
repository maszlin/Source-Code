/*
 * NEPS.OSP.COPPER.CABLE_TRANSFER_POST
 * class name : clsTransfer
 * 
 * develop by : m.zam 
 * version : 1.0
 * started : 27-JUNE-2012
 * done on :
 * 
 * 
 * assumption & conditions 
 * - user draws cable from exchange to cabinet
 * - selected source cable must have joint at OUT side of NR-CONNECT
 * - selected destination cable must have joint at IN side of NR-CONNECT
 * 
 * UAT 03-JULY-2012
 * - cable count
 * - confirmation from user to maintain pair count or update new if cable code is equal
 * - compare : effective pairs and copper size
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



namespace NEPS.OSP.COPPER.CABLE_TRANSFER_POST
{
    class clsTransferPost
    {

        /*
         * CNO 6101 --> GC_TRANSFER (Attribute)
         * CNO 6112 --> GC_TRANSFERLDR_L (Leader Line)
         * CNO 6120 --> GC_TRANSFER_S (Transfer Symbol)
         * CNO 6130 --> GC_TRANSFER_T (Transfer Text)
         */
        public short TR_FNO = 6100;
        public short LINE_FNO = 7000;
        public short JOINT_FNO = 10800;

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

        public bool PerformPostTransfer(Logger log)
        {
            try
            {
                GTTransferPost.m_oIGTTransactionManager.Begin("Complete Transfer");
                GTClassFactory.Create<IGTApplication>().BeginWaitCursor();

                ADODB.Recordset rs = new ADODB.Recordset();
                log.WriteLog("SELECT * FROM GC_TRANSFER WHERE G3E_FID = " + transferDDC.FID);

                GTTransferPost.m_CustomForm.IncreaseProgressBar(1);
                rs = myUtil.ADODB_ExecuteQuery("SELECT * FROM GC_TRANSFER WHERE G3E_FID = " + transferDDC.FID);
                if (!rs.EOF)
                {
                    int donorJointFID = int.Parse(myUtil.rsField(rs, "TERMINATION_FID"));
                    int donorCableFID = int.Parse(myUtil.rsField(rs, "SRC_CABLE_FID"));
                    int cabinetFID = int.Parse(myUtil.rsField(rs, "CABINET_FID"));
                    int recipientStubFID = int.Parse(myUtil.rsField(rs, "STUB_FID"));

                    log.WriteLog("donorJointFID " + donorJointFID);
                    log.WriteLog("donorCableFID " + donorCableFID);

                    log.WriteLog("PostDownstream");
                    PostDownstream(donorJointFID, cabinetFID); //donor joint FID
                    log.WriteLog("PostTransferCable");
                    PostTransferCable(donorJointFID);
                    log.WriteLog("DeleteTransferStub");
                    DeleteTransferStub(recipientStubFID);
                    log.WriteLog("UpdateDonorUpstream");
                    UpdateDonorUpstream(donorCableFID);
                    log.WriteLog("Post Transfer Completed");
                    DeleteTransferObject(transferDDC.FID);
                    log.WriteLog("Transfer Feature Deleted");
                }

                rs.Close();
                rs = null;

                GTTransferPost.m_oIGTTransactionManager.Commit();
                GTTransferPost.m_oIGTTransactionManager.RefreshDatabaseChanges();
                return true;

            }
            catch (Exception ex)
            {
                log.WriteErr(ex);
                GTTransferPost.m_oIGTTransactionManager.Rollback();
                return false;
            }
            finally
            {
                GTClassFactory.Create<IGTApplication>().EndWaitCursor();
            }
        }


        #region 4 : Update Downstream
        /// <summary>
        /// Update downstream cable and all features connected to it
        /// </summary>
        /// <param name="cableFID">Cable FID</param>
        private void PostDownstream(int donorFID, int cabFID)
        {
            int oFID = 0;
            short oFNO = 0;
            int cFID = 0;
            short cFNO = 0;

            ADODB.Recordset rs = new ADODB.Recordset();
            string ssql = "SELECT IN_FNO, IN_FID, G3E_FNO, G3E_FID, OUT_FNO, OUT_FID FROM GC_NR_CONNECT " +
                "START WITH IN_FID = " + donorFID.ToString() + " CONNECT BY NOCYCLE " +
                "PRIOR OUT_FID = IN_FID AND IN_FNO IN (10800, 13100, 6200) AND IN_FID <> 0 AND OUT_FID <> 0 ";
            rs = GTClassFactory.Create<IGTApplication>().DataContext.OpenRecordset(
                ssql, ADODB.CursorTypeEnum.adOpenStatic, ADODB.LockTypeEnum.adLockOptimistic, 1);

            ChangeFeatureState(donorFID, 10800, "ASB");

            clsBoundary cabBoundary = new clsBoundary(cabFID);

            while (!rs.EOF)
            {
                GTTransferPost.m_CustomForm.IncreaseProgressBar(1);

                oFNO = short.Parse(myUtil.rsField(rs, "OUT_FNO"));
                oFID = int.Parse(myUtil.rsField(rs, "OUT_FID"));
                cFNO = short.Parse(myUtil.rsField(rs, "G3E_FNO"));
                cFID = int.Parse(myUtil.rsField(rs, "G3E_FID"));

                ChangeFeatureState(oFID, oFNO, "ASB");
                ChangeFeatureState(cFID, cFNO, "ASB");

                // edited by m.zam @ 2013-05-20 - update parent boundary
                if (oFNO != JOINT_FNO)
                    cabBoundary.UpdateParentBoundary(oFNO, oFID);

                myUtil.ADODB_ExecuteNonQuery("UPDATE GC_CBL SET COUNT_TRANSFER = '' WHERE G3E_FID = " + cFID.ToString());
                DeleteCountBL_Geo(cFID);
                DeleteCountBL_Detail(cFID);

                rs.MoveNext();
            }
            rs.Close();
            rs = null;
        }

        #endregion

        #region 5 : Transfer Rollback Feature 20-07-2012 _ last update 10-05-2013 - delete by FNO

        /*
         * CNO 6101 --> GC_TRANSFER (Attribute)
         * CNO 6112 --> GC_TRANSFERLDR_L (Leader Line)
         * CNO 6120 --> GC_TRANSFER_S (Transfer Symbol)
         * CNO 6130 --> GC_TRANSFER_T (Transfer Text)
         */
        public bool DeleteTransferObject(int iFID)
        {
            IGTKeyObject TR_Feature = GTTransferPost.m_gtapp.DataContext.OpenFeature(TR_FNO, iFID);
            DeleteFeatureFNO(TR_Feature.FID, TR_Feature.FNO);
            return true;
        }

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
        #endregion

        #region 6 : Update Upstream 27-03-2013
        int mi_srcFID = 0;

        private void PostTransferCable(int donorFID)
        {
            int iFID = 0;
            short iFNO = 0;
            int cFID = 0;
            short cFNO = 0;

            ADODB.Recordset rs = new ADODB.Recordset();
            string ssql = "SELECT IN_FNO, IN_FID, G3E_FNO, G3E_FID, OUT_FNO, OUT_FID FROM GC_NR_CONNECT " +
                "WHERE OUT_FID = " + donorFID.ToString();
            rs = GTClassFactory.Create<IGTApplication>().DataContext.OpenRecordset(
                ssql, ADODB.CursorTypeEnum.adOpenStatic, ADODB.LockTypeEnum.adLockOptimistic, 1);


            if (!rs.EOF)
            {
                GTTransferPost.m_CustomForm.IncreaseProgressBar(1);

                iFNO = short.Parse(myUtil.rsField(rs, "IN_FNO"));
                iFID = int.Parse(myUtil.rsField(rs, "IN_FID"));
                cFNO = short.Parse(myUtil.rsField(rs, "G3E_FNO"));
                cFID = int.Parse(myUtil.rsField(rs, "G3E_FID"));

                ChangeFeatureState(iFID, iFNO, "ASB");
                ChangeFeatureState(cFID, cFNO, "ASB");
                mi_srcFID = iFID;
            }
        }

        private void ChangeFeatureState(int iFID, short iFNO, string status)
        {
            try
            {
                myUtil.ADODB_ExecuteNonQuery
                    ("UPDATE GC_NETELEM SET FEATURE_STATE = '" + status + "' WHERE G3E_FID = " + iFID.ToString());
            }
            catch (Exception ex)
            {
                Debug.WriteLine("ERROR on CHG Feature State [" + iFNO.ToString() + "][" + iFID.ToString() + "] : " + ex.Message);
            }
        }

        #endregion

        #region DeleteStub 27-03-2013
        private void DeleteTransferStub(int stubFID)
        {

            string CableClass = myUtil.GetFieldValue("GC_CBL", "CABLE_CLASS", stubFID);
            if (CableClass.Contains("STUB"))
            {
                short cblFNO = 7000;
                short jntFNO = 10800;
                int jntFID = 0;

                jntFID = myUtil.ParseInt(myUtil.GetFieldValue("GC_NR_CONNECT", "OUT_FID", stubFID));

                ChangeFeatureState(stubFID, cblFNO, "MOD");
                DeleteFeatureFNO(stubFID, cblFNO);   // delete stub cable

                if (jntFID > 0)
                {
                    ChangeFeatureState(jntFID, jntFNO, "MOD");
                    DeleteFeatureFNO(jntFID, jntFNO);   // delete stub joint
                }
            }
        }


        #endregion

        #region COUNT - 20-07-2012

        private void DeleteCountBL_Geo(int iFID)
        {
            try
            {
                myUtil.ADODB_ExecuteNonQuery("DELETE FROM GC_CBLCNT_BL_T WHERE G3E_FID = " + iFID);
                GTTransferPost.m_CustomForm.IncreaseProgressBar(1);
            }
            catch (Exception ex)
            {
            }
        }
        private void DeleteCountBL_Detail(int iFID)
        {
            try
            {
                myUtil.ADODB_ExecuteNonQuery("DELETE FROM DGC_CBLCNT_BL_T WHERE G3E_FID = " + iFID);
                GTTransferPost.m_CustomForm.IncreaseProgressBar(1);
            }
            catch (Exception ex)
            {
            }
        }
        #endregion

        #region Update Upstream Cable Feature State 13-05-2013

        private void UpdateDonorUpstream(int cableFID)
        {
            IGTKeyObject upstreamCable = GTTransferPost.m_gtapp.DataContext.OpenFeature(LINE_FNO, cableFID);
            upstreamCable.Components.GetComponent(51).Recordset.MoveLast();
            upstreamCable.Components.GetComponent(51).Recordset.Update("FEATURE_STATE", "ASB");

            ADODB.Recordset rs = new ADODB.Recordset();
            rs = myUtil.ADODB_ExecuteQuery("SELECT * FROM GC_NR_CONNECT WHERE G3E_FID = " + cableFID.ToString());
            if (!rs.EOF)
            {
                int jointFID = int.Parse(myUtil.rsField(rs, "OUT_FID"));
                IGTKeyObject jntFeature = GTTransferPost.m_gtapp.DataContext.OpenFeature(JOINT_FNO, jointFID);
                jntFeature.Components.GetComponent(51).Recordset.MoveLast();
                jntFeature.Components.GetComponent(51).Recordset.Update("FEATURE_STATE", "ASB");
            }
        }
        #endregion
    }
}
