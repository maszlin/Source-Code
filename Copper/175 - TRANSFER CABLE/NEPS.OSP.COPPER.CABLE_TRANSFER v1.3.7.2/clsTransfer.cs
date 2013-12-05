/*
 * NEPS.OSP.COPPER.CABLE_TRANSFER
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
 * UPDATE : CREATE REPORT - 20-JULY-2012
 * 
 * update : 30-Oct-2012
 * - E-SIDE transfer
 */

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;

using Intergraph.GTechnology.Interfaces;
using Intergraph.GTechnology.API;

using ADODB;



namespace NEPS.OSP.COPPER.CABLE_TRANSFER
{
    class clsTransfer
    {
        public short LINE_FNO = 7000;
        public short LINE_GEO_CNO = 7010;
        public short LINE_DET_CNO = 7011;

        public short JOINT_FNO = 10800;
        public short JOINT_CNO = 10801;
        public short JOINT_GEO_CNO = 10820;
        public short JOINT_DET_CNO = 10821;

        private int srcInFID = 0;
        private short srcInFNO = 0;
        private int dstJointFID = 0;
        private int dstCableFID = 0;

        public IGTDDCKeyObject srcSelectedCable;
        public IGTDDCKeyObject dstSelectedCable;
        public IGTDDCKeyObject dstSelectedJoint;
        private IGTKeyObject srcFeature;
        private IGTKeyObject dstFeature;

        public string m_transferType;
        public string m_EXC_ABB = "-";
        public string m_ITFACE_CODE = "-";
        public string m_RT_CODE = "-";
        public string m_CABLE_CODE = "";

        public IGTPoint m_LeaderPoint;

        #region INITIAL - recipient and donor selection

        #region Select Recipient (source)
        public bool isSrceSelected(IGTSelectedObjects selectedObj)
        {
            srcSelectedCable = null;
            switch (selectedObj.FeatureCount)
            {
                case 1:     // one feature selected
                    foreach (IGTDDCKeyObject oDDC in selectedObj.GetObjects())
                        if (oDDC.FNO == LINE_FNO)
                        {
                            srcSelectedCable = oDDC;
                            if (isStubOrStump(oDDC.FID))
                            {
                                GetExcAbb(oDDC.FID, 0);
                                if (m_transferType == "TRANSFER D-SIDE")
                                    GetCableCode_D(oDDC.FID, "GC_CBL", 0);
                                else
                                    GetCableCode_E(oDDC.FID, "GC_CBL", 0);

                                return GetNRConnect(oDDC, "IN");
                            }
                            else
                                return false;
                        }
                    return false;
                default: // more than one features selected
                    return false;
            }//switch
        }

        private bool isStubOrStump(int iFID)
        {
            string ssql = "SELECT CABLE_CLASS, CABLE_CODE, TOTAL_SIZE, EFFECTIVE_PAIRS FROM GC_CBL WHERE G3E_FID = " + iFID;

            ADODB.Recordset rs = new ADODB.Recordset();
            rs = GTTransfer.m_gtapp.DataContext.OpenRecordset(ssql,
            ADODB.CursorTypeEnum.adOpenStatic, ADODB.LockTypeEnum.adLockOptimistic, 1);

            if (!rs.EOF)
            {
                string cableclass = myUtil.rsField(rs, "CABLE_CLASS");
                if (cableclass.IndexOf("D-SIDE") > -1)
                    m_transferType = "TRANSFER D-SIDE";
                else
                    m_transferType = "TRANSFER E-SIDE";

                m_CABLE_CODE = myUtil.rsField(rs, "CABLE_CODE");
                System.Diagnostics.Debug.WriteLine("CABLE_CODE :" + m_CABLE_CODE);
                clsReport.rcpnt.m_CABLE_SIZE = myUtil.ParseInt(myUtil.rsField(rs, "TOTAL_SIZE"));
                clsReport.rcpnt.m_EFCT_PAIRS = myUtil.ParseInt(myUtil.rsField(rs, "EFFECTIVE_PAIRS"));

                if ((cableclass.IndexOf("STUB") > -1) || (cableclass.IndexOf("STUMP") > -1))
                    return true;
                else
                    return false;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Get the joint connected to the selected cable. 
        /// For source cable, we are looking for joint at the OUT side
        /// while for destination cable, we want join at the IN side
        /// </summary>
        /// <param name="cable">selected cable</param>
        /// <param name="jointType">OUT (source) or IN (destination)</param>
        /// <returns></returns>
        private bool GetNRConnect(IGTDDCKeyObject cable, string jointType)
        {
            try
            {
                int jointFID = -1;

                string ssql = "SELECT * FROM GC_NR_CONNECT ";
                ssql += "WHERE G3E_FID = " + cable.FID;

                ADODB.Recordset rs = new ADODB.Recordset();

                rs = GTTransfer.m_gtapp.DataContext.OpenRecordset(ssql,
                ADODB.CursorTypeEnum.adOpenStatic, ADODB.LockTypeEnum.adLockOptimistic, 1);

                if (rs.EOF)
                {
                    GTTransfer.m_CustomForm.ProgressMessage("NR connect not define for this cable");
                    return false;
                }
                else
                {
                    srcInFNO = short.Parse(myUtil.rsField(rs, jointType + "_FNO"));
                    srcInFID = int.Parse(myUtil.rsField(rs, jointType + "_FID"));
                }
                return true;
            }
            catch (Exception ex)
            {
                GTTransfer.m_CustomForm.ProgressMessage("Fail to read connected cable from this joint\r\n" + ex.Message);
                return false;
            }
        }
        #endregion

        #region Select Donor (destination)
        public bool isDestSelected(IGTSelectedObjects selectedObj)
        {
            dstSelectedJoint = null;
            switch (selectedObj.FeatureCount)
            {
                case 1:     // one feature selected
                    foreach (IGTDDCKeyObject oDDC in selectedObj.GetObjects())
                        if (oDDC.FNO == JOINT_FNO)
                        {
                            dstSelectedJoint = oDDC;
                            dstJointFID = dstSelectedJoint.FID;

                            GetExcAbb(oDDC.FID, 1);
                            if (m_transferType == "TRANSFER D-SIDE")
                                GetCableCode_D(oDDC.FID, "GC_SPLICE", 1);
                            else
                                GetCableCode_E(oDDC.FID, "GC_SPLICE", 1);

                            if (ValidateDonorClass(dstJointFID) && ValidateDonorStatus(dstJointFID))
                                return ValidateDonorCable(dstJointFID);
                            else
                                return false;
                        }
                    return false;
                default: // more than one features selected
                    return false;
            }//switch
        }

        private bool ValidateDonorClass(int iFID)
        {
            //m_transferType
            string ssql = "SELECT SPLICE_CLASS FROM GC_SPLICE WHERE G3E_FID = " + iFID;

            ADODB.Recordset rs = new ADODB.Recordset();
            rs = GTTransfer.m_gtapp.DataContext.OpenRecordset(ssql,
            ADODB.CursorTypeEnum.adOpenStatic, ADODB.LockTypeEnum.adLockOptimistic, 1);

            if (rs.EOF)
                return false;
            else
            {
                string status = (m_transferType == "TRANSFER E-SIDE" ? "JOINT E-SIDE" : "JOINT D-SIDE");
                if (myUtil.rsField(rs, "SPLICE_CLASS") == status)
                    return true;
                else
                {
                    GTTransfer.m_CustomForm.ProgressMessage("CLASS ERROR : The donor must be a " + status);
                    return false;
                }
            }
        }

        private bool ValidateDonorStatus(int iFID)
        {
            // as per discussion with HJ Zuki, TM on 25-July-2013 
            // base on user feedback, we are not going to test Donor feature state 
            return true; 

            string ssql = "SELECT FEATURE_STATE FROM GC_NETELEM WHERE G3E_FID = " + iFID;

            ADODB.Recordset rs = new ADODB.Recordset();
            rs = GTTransfer.m_gtapp.DataContext.OpenRecordset(ssql,
            ADODB.CursorTypeEnum.adOpenStatic, ADODB.LockTypeEnum.adLockOptimistic, 1);

            if (rs.EOF)
                return false;
            else
            {
                string status = myUtil.rsField(rs, "FEATURE_STATE");
                if (status == "ASB" || status == "MOD" || status == "PPF")
                    return true;
                else
                {
                    GTTransfer.m_CustomForm.ProgressMessage("STATUS ERROR : The donor must be either ASB, MOD or PPF only");
                    return false;
                }
            }

        }

        private bool ValidateDonorCable(int iFID)
        {
            dstCableFID = UpstreamCableFID(dstJointFID);
            // compare cable between recipient and donor
            if (dstCableFID == 1)
            {
                GTTransfer.m_CustomForm.ProgressMessage("RECORD ERROR : Donor record not found");
                return false;
            }
            else if (!CompareCable(dstCableFID))
                return false;

            return true;
        }
        #endregion


        private string GetExcAbb(int iFID, int featuretype)
        {
            string ssql = "SELECT EXC_ABB FROM GC_NETELEM WHERE G3E_FID = " + iFID;

            ADODB.Recordset rs = new ADODB.Recordset();
            rs = GTTransfer.m_gtapp.DataContext.OpenRecordset(ssql,
            ADODB.CursorTypeEnum.adOpenStatic, ADODB.LockTypeEnum.adLockOptimistic, 1);

            if (!rs.EOF)
            {
                if (featuretype == 0)
                    clsReport.rcpnt.EXC_ABB = myUtil.rsField(rs, "EXC_ABB");
                else
                    clsReport.donor.EXC_ABB = myUtil.rsField(rs, "EXC_ABB");
                return myUtil.rsField(rs, "EXC_ABB");
            }
            else
                return "";
        }

        public string GetCableCode_D(int iFID, string tablename, int featuretype)
        {
            string ssql = "SELECT CABLE_CODE, ITFACE_CODE, RT_CODE FROM " + tablename + " WHERE G3E_FID = " + iFID;

            ADODB.Recordset rs = new ADODB.Recordset();
            rs = GTTransfer.m_gtapp.DataContext.OpenRecordset(ssql,
            ADODB.CursorTypeEnum.adOpenStatic, ADODB.LockTypeEnum.adLockOptimistic, 1);

            if (!rs.EOF)
            {
                string cabcode = myUtil.rsField(rs, "ITFACE_CODE");
                string rtcode = myUtil.rsField(rs, "RT_CODE");
                if (featuretype == 0) // it's a recipient cable
                {
                    if (cabcode.Length > 0)
                    {
                        clsReport.rcpnt = GetCabFID(clsReport.rcpnt, cabcode);
                        //clsReport.rcpnt.CAB_CODE = cabcode;
                    }
                    else
                    {
                        clsReport.rcpnt = GetRTFID(clsReport.rcpnt, rtcode);
                        clsReport.rcpnt.CAB_CODE = rtcode;
                    }
                    clsReport.rcpnt.CABLE_CODE = myUtil.rsField(rs, "CABLE_CODE");
                }
                else // it's a donor joint
                {
                    if (cabcode.Length > 0)
                        clsReport.donor = GetCabFID(clsReport.donor, cabcode);
                    else
                        clsReport.donor = GetRTFID(clsReport.donor, rtcode);

                    clsReport.donor.CABLE_CODE = myUtil.rsField(rs, "CABLE_CODE");
                }
                return (myUtil.rsField(rs, "CABLE_CODE"));
            }
            else
                return "-";
        }

        private clsReport.TransferPoint GetCabFID(clsReport.TransferPoint f, string itface_code)
        {
            try
            {
                string ssql = "SELECT A.G3E_FID, A.ITFACE_CODE, A.ITFACE_CLASS FROM GC_ITFACE A, GC_NETELEM B " +
                    "WHERE A.G3E_FID = B.G3E_FID AND A.ITFACE_CODE = '" + itface_code + "' " +
                    "AND B.EXC_ABB = '" + f.EXC_ABB + "'";

                ADODB.Recordset rs = new ADODB.Recordset();
                rs = GTClassFactory.Create<IGTApplication>().DataContext.OpenRecordset(ssql,
                    ADODB.CursorTypeEnum.adOpenStatic, ADODB.LockTypeEnum.adLockOptimistic, 1);

                if (!rs.EOF)
                {
                    rs.MoveLast();
                    f.CAB_FID = int.Parse(myUtil.rsField(rs, "G3E_FID"));
                    f.CAB_CODE = myUtil.rsField(rs, "ITFACE_CODE");
                    f.CAB_TYPE = myUtil.rsField(rs, "ITFACE_CLASS");
                }
                else
                {
                    f.CAB_FID = -1;
                    f.CAB_CODE = itface_code;
                    f.CAB_TYPE = "NOT DEFINE";
                }

            }
            catch
            {
                f.CAB_FID = -1;
                f.CAB_CODE = itface_code;
                f.CAB_TYPE = "NOT DEFINE";
            }
            return f;
        }

        private clsReport.TransferPoint GetRTFID(clsReport.TransferPoint f, string rt_code)
        {
            try
            {
                string ssql = "SELECT A.G3E_FID, A.RT_CODE, A.RT_TYPE FROM GC_RT A, GC_NETELEM B " +
                    "WHERE A.G3E_FID = B.G3E_FID AND A.RT_CODE = '" + rt_code + "' " +
                    "AND B.EXC_ABB = '" + f.EXC_ABB + "' ";

                ssql += "UNION SELECT A.G3E_FID, A.RT_CODE, A.RT_TYPE FROM GC_MSAN A, GC_NETELEM B " +
                "WHERE A.G3E_FID = B.G3E_FID AND A.RT_CODE = '" + rt_code + "' " +
                "AND B.EXC_ABB = '" + f.EXC_ABB + "' ";

                ssql = "UNION SELECT A.G3E_FID, A.RT_CODE, A.RT_TYPE FROM GC_VDSL2 A, GC_NETELEM B " +
                "WHERE A.G3E_FID = B.G3E_FID AND A.RT_CODE = '" + rt_code + "' " +
                "AND B.EXC_ABB = '" + f.EXC_ABB + "'";



                ADODB.Recordset rs = new ADODB.Recordset();
                rs = GTClassFactory.Create<IGTApplication>().DataContext.OpenRecordset(ssql,
                    ADODB.CursorTypeEnum.adOpenStatic, ADODB.LockTypeEnum.adLockOptimistic, 1);

                if (!rs.EOF)
                {
                    rs.MoveLast();
                    f.CAB_FID = int.Parse(myUtil.rsField(rs, "G3E_FID"));
                    f.CAB_CODE = myUtil.rsField(rs, "RT_CODE");
                    f.CAB_TYPE = myUtil.rsField(rs, "RT_TYPE");
                }                
                else
                {
                    f.CAB_FID = -1;
                    f.CAB_CODE = rt_code;
                    f.CAB_TYPE = "NOT DEFINE";
                }

            }
            catch
            {
                f.CAB_FID = -1;
                f.CAB_CODE = rt_code;
                f.CAB_TYPE = "NOT DEFINE";
            }
            return f;
        }
        #endregion

        public int PerformCableTransfer(DateTime transferDate)
        {
            int cableFID = -1;
            try
            {
                GTTransfer.m_CustomForm.IncreaseProgressBar(3);

                GTClassFactory.Create<IGTApplication>().BeginWaitCursor();
                GTTransfer.m_oIGTTransactionManager.Begin("Perform Cable Transfer");
                /*** notes : Commit or Rollback is call from 'CreateTransferObject' ***/

                clsReport.InitReport(transferDate, m_transferType);
                GTTransfer.m_CustomForm.ProgressMessage("Cable Transfer starting ...");

                // break the cable from destination cable
                GTTransfer.m_CustomForm.AddReport("Breaking upstream cable from donor");
                GTTransfer.m_CustomForm.IncreaseProgressBar(3);
                IGTPoint upstreamJointPoint = BreakDestinationUpstreamCable(dstCableFID);

                // create new splice stub for the upstream cable (move back by 1 point)
                GTTransfer.m_CustomForm.AddReport("Set upstream cable as stub");
                GTTransfer.m_CustomForm.IncreaseProgressBar(3);
                int upstreamJointFID = CreateUpstreamSplice(upstreamJointPoint);

                // update NR connect for upstream cable to the splice stub
                GTTransfer.m_CustomForm.AddReport("Update upstream cable NR connect");
                GTTransfer.m_CustomForm.IncreaseProgressBar(3);
                UpdateNRConnectUpstream(dstCableFID, upstreamJointFID);

                // create transfer cable connecting source cable to destination cable
                GTTransfer.m_CustomForm.AddReport("Create transfer cable between donor to recipient");
                GTTransfer.m_CustomForm.IncreaseProgressBar(3);
                cableFID = CreateTransferCable();

                // update recipient class - cable & joint
                GTTransfer.m_CustomForm.AddReport("Update recipient cable and splice class");
                GTTransfer.m_CustomForm.IncreaseProgressBar(3);
                UpdateRecipientClass(srcSelectedCable.FID, srcInFID);
                // update EXC_ABB or ITFACE_CODE for all component
                GTTransfer.m_CustomForm.AddReport("Update donor downstream");
                GTTransfer.m_CustomForm.IncreaseProgressBar(3);
                UpdateDownstreams(dstJointFID);
                // create transfer object
                GTTransfer.m_CustomForm.AddReport("Creating transfer object ... please wait");

                return CreateTransferObject(dstCableFID, transferDate);

            }
            catch (Exception ex)
            {
                GTTransfer.m_oIGTTransactionManager.Rollback();
                throw ex;
            }
            finally
            {
                GTClassFactory.Create<IGTApplication>().EndWaitCursor();
                // center view to tranfer cable
                if (cableFID > -1) hiliteFeature(cableFID, LINE_FNO);
            }
        }

        #region 2 : break destination cable from exchange - create new transfer joint for upstream cable

        /// <summary>
        /// Get FID for upstream cable from selected cable IN joint
        /// </summary>
        /// <returns>cable FID</returns>
        private int UpstreamCableFID(int jointFID)
        {
            try
            {
                int iFID = -1;

                string ssql = "SELECT * FROM GC_NR_CONNECT WHERE OUT_FID = " + jointFID.ToString();

                ADODB.Recordset rs = new ADODB.Recordset();
                rs = GTTransfer.m_gtapp.DataContext.OpenRecordset(ssql,
                ADODB.CursorTypeEnum.adOpenStatic, ADODB.LockTypeEnum.adLockOptimistic, 1);

                if (rs.EOF)
                    GTTransfer.m_CustomForm.ProgressMessage("No cable define as OUT to the donor joint");
                else if (rs.RecordCount > 1)
                    GTTransfer.m_CustomForm.ProgressMessage("More than two cables define as OUT to the donor joint");
                else
                    iFID = int.Parse(myUtil.rsField(rs, "G3E_FID"));
                rs.Close();
                rs = null;

                return iFID;

            }
            catch (Exception ex)
            {
                GTTransfer.m_CustomForm.ProgressMessage("Fail to read upstream cable from this joint\r\n" + ex.Message);
                return -1;
            }
        }

        private bool CompareCable(int dstCableFID)
        {
            try
            {
                string[] keys = { "TOTAL_SIZE", "EFFECTIVE_PAIRS" };

                ADODB.Recordset rsDest = new ADODB.Recordset();

                string ssql = "SELECT * FROM GC_CBL WHERE G3E_FID = ";

                rsDest = GTTransfer.m_gtapp.DataContext.OpenRecordset(ssql + dstCableFID,
                ADODB.CursorTypeEnum.adOpenStatic, ADODB.LockTypeEnum.adLockOptimistic, 1);

                clsReport.donor.m_CABLE_SIZE = myUtil.ParseInt(rsDest.Fields["TOTAL_SIZE"].Value.ToString());
                clsReport.donor.m_EFCT_PAIRS = myUtil.ParseInt(rsDest.Fields["EFFECTIVE_PAIRS"].Value.ToString());

                if (clsReport.rcpnt.m_CABLE_SIZE != clsReport.donor.m_CABLE_SIZE)
                    throw new System.Exception("Cable size is not equal");
                else if (clsReport.rcpnt.m_EFCT_PAIRS != clsReport.donor.m_EFCT_PAIRS)
                    throw new System.Exception("Effective pairs is not equal");
                else
                    return true;
            }
            catch (Exception ex)
            {
                GTTransfer.m_CustomForm.ProgressMessage("Donor not compatible to recipient\r\n" + ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Break destination cable from current exchange by removing the upstream cable
        /// Create new end point for the upstream cable by move back 5 point
        /// </summary>
        /// <returns>new end point of the upstream cable</returns>
        private IGTPoint BreakDestinationUpstreamCable(int LINE_FID)
        {
            IGTPolylineGeometry cblPoints = GTClassFactory.Create<IGTPolylineGeometry>();
            IGTPoint jntPoint = GTClassFactory.Create<IGTPoint>();

            #region  Read All Points From Upstream Cable
            string ssql =
            "SELECT GC_OSP_COP_VAL.EXTRACT_LINE_TO_VECTOR(G3E_GEOMETRY) AS XY FROM GC_CBL_L WHERE G3E_FID = " + LINE_FID;

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

            GTTransfer.m_CustomForm.IncreaseProgressBar(3);
            #region Move Back Upsream Last Point by 5%
            int lastpoint = cblPoints.Points.Count - 1;
            jntPoint = MoveBack2Point(cblPoints, lastpoint - 1);
            cblPoints.Points[lastpoint] = jntPoint;
            // copy the endpoint for feature second leaderline
            m_LeaderPoint = jntPoint;
            #endregion

            GTTransfer.m_CustomForm.IncreaseProgressBar(3);
            #region Update Upstream Cable Geometry

            IGTKeyObject upstreamCable = GTTransfer.m_gtapp.DataContext.OpenFeature(LINE_FNO, LINE_FID);
            upstreamCable.Components.GetComponent(LINE_GEO_CNO).Recordset.MoveLast();
            upstreamCable.Components.GetComponent(LINE_GEO_CNO).Geometry = cblPoints;

            upstreamCable.Components.GetComponent(7001).Recordset.MoveLast();
            upstreamCable.Components.GetComponent(7001).Recordset.Update("CUSAGE", "STUB");
            upstreamCable.Components.GetComponent(7001).Recordset.Update("CABLE_CLASS",
                (m_transferType == "TRANSFER D-SIDE" ? "STUB D-SIDE" : "STUB E-SIDE"));

            upstreamCable.Components.GetComponent(51).Recordset.MoveLast();
            upstreamCable.Components.GetComponent(51).Recordset.Update("FEATURE_STATE", "MOD");

            #endregion

            GTTransfer.m_CustomForm.IncreaseProgressBar(3);
            return jntPoint;
        }

        private int CreateUpstreamSplice(IGTPoint jointPoint)
        {
            ADODB.Recordset rs;
            short iCNO = JOINT_GEO_CNO;
            int iFID = -1;


            IGTPointGeometry jointGeo = GTClassFactory.Create<IGTPointGeometry>();
            jointGeo.Origin = jointPoint;

            #region Open Donor Joint
            IGTKeyObject donorJoint;
            donorJoint = GTTransfer.m_gtapp.DataContext.OpenFeature(JOINT_FNO, dstJointFID);
            #endregion

            #region Insert Joint Upstream

            IGTKeyObject upstreamJoint;
            upstreamJoint = GTClassFactory.Create<IGTApplication>().DataContext.NewFeature(JOINT_FNO);
            iFID = upstreamJoint.FID;

            if (upstreamJoint.Components.GetComponent(iCNO).Recordset.EOF)
            {
                upstreamJoint.Components.GetComponent(iCNO).Recordset.AddNew("G3E_FID", upstreamJoint.FID);
                upstreamJoint.Components.GetComponent(iCNO).Recordset.Update("G3E_FNO", upstreamJoint.FNO);
                upstreamJoint.Components.GetComponent(iCNO).Recordset.Update("G3E_CID", 1);
            }
            else
                upstreamJoint.Components.GetComponent(iCNO).Recordset.MoveLast();

            upstreamJoint.Components.GetComponent(iCNO).Geometry = jointGeo;

            #endregion

            #region Joint Attributes
            iCNO = JOINT_CNO;
            if (upstreamJoint.Components.GetComponent(iCNO).Recordset.EOF)
            {
                upstreamJoint.Components.GetComponent(iCNO).Recordset.AddNew("G3E_FID", upstreamJoint.FID);
                upstreamJoint.Components.GetComponent(iCNO).Recordset.Update("G3E_FNO", upstreamJoint.FNO);
                upstreamJoint.Components.GetComponent(iCNO).Recordset.Update("G3E_CID", 1);
            }
            else
                upstreamJoint.Components.GetComponent(iCNO).Recordset.MoveLast();

            // copy min material from donor joint
            rs = donorJoint.Components.GetComponent(iCNO).Recordset;

            myUtil.CopyFields(upstreamJoint, rs, iCNO, "CABLE_CODE");
            myUtil.CopyFields(upstreamJoint, rs, iCNO, "CASE_TYPE");
            myUtil.CopyFields(upstreamJoint, rs, iCNO, "CLOSURE_TYPE");
            myUtil.CopyFields(upstreamJoint, rs, iCNO, "DBLOSS");
            myUtil.CopyFields(upstreamJoint, rs, iCNO, "DCR");
            myUtil.CopyFields(upstreamJoint, rs, iCNO, "DIST_FROM_EXC");
            myUtil.CopyFields(upstreamJoint, rs, iCNO, "ID");
            myUtil.CopyFields(upstreamJoint, rs, iCNO, "ITFACE_CODE");
            myUtil.CopyFields(upstreamJoint, rs, iCNO, "MODEL");
            myUtil.CopyFields(upstreamJoint, rs, iCNO, "ORIGINAL_USER");
            myUtil.CopyFields(upstreamJoint, rs, iCNO, "RT_CODE");
            myUtil.CopyFields(upstreamJoint, rs, iCNO, "SPLICE_ADMIN_TYPE");
            myUtil.CopyFields(upstreamJoint, rs, iCNO, "SPLICE_CONNECTION_TYPE");
            myUtil.CopyFields(upstreamJoint, rs, iCNO, "SPLICE_LOSS");
            myUtil.CopyFields(upstreamJoint, rs, iCNO, "SPLICE_NOTE");
            myUtil.CopyFields(upstreamJoint, rs, iCNO, "SPLICE_PHYSICAL_TYPE");

            upstreamJoint.Components.GetComponent(iCNO).Recordset.Update("JOINT_TYPE", "STRAIGHT");
            if (m_transferType == "TRANSFER D-SIDE")
                upstreamJoint.Components.GetComponent(iCNO).Recordset.Update("SPLICE_CLASS", "STUB D-SIDE");
            else
                upstreamJoint.Components.GetComponent(iCNO).Recordset.Update("SPLICE_CLASS", "STUB E-SIDE");
            #endregion

            #region Joint Netelem
            iCNO = 51;
            if (upstreamJoint.Components.GetComponent(iCNO).Recordset.EOF)
            {
                upstreamJoint.Components.GetComponent(iCNO).Recordset.AddNew("G3E_FID", upstreamJoint.FID);
                upstreamJoint.Components.GetComponent(iCNO).Recordset.Update("G3E_FNO", upstreamJoint.FNO);
                upstreamJoint.Components.GetComponent(iCNO).Recordset.Update("G3E_CID", 1);
            }
            else
                upstreamJoint.Components.GetComponent(iCNO).Recordset.MoveLast();

            upstreamJoint.Components.GetComponent(iCNO).Recordset.Update("JOB_STATE", "PROPOSED");
            upstreamJoint.Components.GetComponent(iCNO).Recordset.Update("FEATURE_STATE", "PPF");
            upstreamJoint.Components.GetComponent(iCNO).Recordset.Update("YEAR_PLACED", DateTime.Now.ToString("yyyy"));

            // copy min material from donor joint
            rs = donorJoint.Components.GetComponent(iCNO).Recordset;
            rs.MoveLast();
            myUtil.CopyFields(upstreamJoint, rs, iCNO, "EXC_ABB");
            myUtil.CopyFields(upstreamJoint, rs, iCNO, "MIN_MATERIAL");

            donorJoint.Components.GetComponent(iCNO).Recordset.MoveLast();
            donorJoint.Components.GetComponent(iCNO).Recordset.Update("FEATURE_STATE", "MOD");

            #endregion

            return iFID;
        }

        private void UpdateNRConnectUpstream(int cableFID, int jointFID)
        {
            short iCNO = 53; // CNO for NR_CONNECT

            IGTKeyObject upstreamCable = GTTransfer.m_gtapp.DataContext.OpenFeature(LINE_FNO, cableFID);

            upstreamCable.Components.GetComponent(iCNO).Recordset.MoveLast();
            upstreamCable.Components.GetComponent(iCNO).Recordset.Update("OUT_FNO", JOINT_FNO);
            upstreamCable.Components.GetComponent(iCNO).Recordset.Update("OUT_FID", jointFID);
        }

        #endregion

        #region 3 : Create Transfer Cable from Source (OutJoint) to Destination (InJoint)

        /// <summary>
        /// Create new transfer cable to connect between the source and destination cable
        /// This function also holds source EXC_ABB and ITFACE_CODE into global variable
        /// </summary>
        private int CreateTransferCable()
        {
            IGTKeyObject scrJointFeature;
            IGTKeyObject dstJointFeature;
            IGTKeyObject cableFeature;

            IGTPolylineGeometry cableJoints = GTClassFactory.Create<IGTPolylineGeometry>();
            IGTPointGeometry geoPoint = GTClassFactory.Create<IGTPointGeometry>();

            ADODB.Recordset rs1;
            short iCNO;
            int iFID = 0;

            //*** GTTransfer.m_oIGTTransactionManager.Begin("Create Transfer Cable");

            #region Get Cable Points
            iCNO = (short)(srcInFNO + 20);
            scrJointFeature = GTTransfer.m_gtapp.DataContext.OpenFeature(srcInFNO, srcInFID);
            scrJointFeature.Components.GetComponent(iCNO).Recordset.MoveLast();
            geoPoint = (IGTPointGeometry)scrJointFeature.Components.GetComponent(iCNO).Geometry;
            cableJoints.Points.Add(geoPoint.Origin);

            iCNO = JOINT_GEO_CNO;
            dstJointFeature = GTTransfer.m_gtapp.DataContext.OpenFeature(JOINT_FNO, dstJointFID);
            dstJointFeature.Components.GetComponent(iCNO).Recordset.MoveLast();
            geoPoint = (IGTPointGeometry)dstJointFeature.Components.GetComponent(iCNO).Geometry;
            cableJoints.Points.Add(geoPoint.Origin);

            #endregion

            #region Create New Cable
            iCNO = LINE_GEO_CNO;
            cableFeature = GTClassFactory.Create<IGTApplication>().DataContext.NewFeature(LINE_FNO);
            iFID = cableFeature.FID;

            if (cableFeature.Components.GetComponent(iCNO).Recordset.EOF)
            {
                cableFeature.Components.GetComponent(iCNO).Recordset.AddNew("G3E_FID", cableFeature.FID);
                cableFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_FNO", cableFeature.FNO);
                cableFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_CID", 1);
            }
            else
            {
                cableFeature.Components.GetComponent(iCNO).Recordset.MoveLast();
            }
            cableFeature.Components.GetComponent(iCNO).Geometry = cableJoints;

            #endregion

            #region Positioning Text Label

            iCNO = 7030;

            if (cableFeature.Components.GetComponent(iCNO).Recordset.EOF)
            {
                cableFeature.Components.GetComponent(iCNO).Recordset.AddNew("G3E_FID", cableFeature.FID);
                cableFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_FNO", cableFeature.FNO);
                cableFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_CID", 1);
            }
            else
            {
                cableFeature.Components.GetComponent(iCNO).Recordset.MoveLast();
            }
            IGTOrientedPointGeometry cblLabel = GTClassFactory.Create<IGTOrientedPointGeometry>();
            cblLabel.Origin = myUtil.GetPointInBetween(cableJoints, 0, 0.5);
            cableFeature.Components.GetComponent(iCNO).Geometry = cblLabel;
            #endregion

            #region GC_Netelem
            iCNO = 51; //Netelem

            if (cableFeature.Components.GetComponent(iCNO).Recordset.EOF)
            {
                cableFeature.Components.GetComponent(iCNO).Recordset.AddNew("G3E_FID", cableFeature.FID);
                cableFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_FNO", cableFeature.FNO);
                cableFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_CID", 1);
            }
            else
            {
                cableFeature.Components.GetComponent(iCNO).Recordset.MoveLast();
            }


            cableFeature.Components.GetComponent(iCNO).Recordset.Update("JOB_STATE", "PROPOSED");
            cableFeature.Components.GetComponent(iCNO).Recordset.Update("FEATURE_STATE", "PPF");

            // copy netelem from source cable
            srcFeature = GTTransfer.m_gtapp.DataContext.OpenFeature(srcSelectedCable.FNO, srcSelectedCable.FID);
            rs1 = srcFeature.Components.GetComponent(iCNO).Recordset;

            myUtil.CopyFields(cableFeature, rs1, iCNO, "EXC_ABB");
            myUtil.CopyFields(cableFeature, rs1, iCNO, "BILLING_RATE");
            myUtil.CopyFields(cableFeature, rs1, iCNO, "ID");
            myUtil.CopyFields(cableFeature, rs1, iCNO, "IMAP_FEATURE_ID");
            myUtil.CopyFields(cableFeature, rs1, iCNO, "JOB_ID");
            myUtil.CopyFields(cableFeature, rs1, iCNO, "JOB_STATE");
            myUtil.CopyFields(cableFeature, rs1, iCNO, "MIC");
            myUtil.CopyFields(cableFeature, rs1, iCNO, "MIN_MATERIAL");
            myUtil.CopyFields(cableFeature, rs1, iCNO, "OWNERSHIP");
            myUtil.CopyFields(cableFeature, rs1, iCNO, "PLAN_ID");
            myUtil.CopyFields(cableFeature, rs1, iCNO, "SAP_WRK_ID");
            myUtil.CopyFields(cableFeature, rs1, iCNO, "SCHEME_NAME");
            myUtil.CopyFields(cableFeature, rs1, iCNO, "SEGMENT");
            myUtil.CopyFields(cableFeature, rs1, iCNO, "SERVICE_CODE");
            myUtil.CopyFields(cableFeature, rs1, iCNO, "SWITCH_CENTRE_CLLI");
            myUtil.CopyFields(cableFeature, rs1, iCNO, "YEAR_PLACED");
            #endregion

            m_EXC_ABB = myUtil.rsField(rs1, "EXC_ABB");

            #region Attributes

            iCNO = 7001;
            if (cableFeature.Components.GetComponent(iCNO).Recordset.EOF)
            {
                cableFeature.Components.GetComponent(iCNO).Recordset.AddNew("G3E_FID", cableFeature.FID);
                cableFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_FNO", cableFeature.FNO);
                cableFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_CID", 1);
            }
            else
            {
                cableFeature.Components.GetComponent(iCNO).Recordset.MoveLast();
            }

            cableFeature.Components.GetComponent(iCNO).Recordset.Update("TOTAL_LENGTH", CableLength(cableJoints));

            // copy source attribute from source cable
            rs1 = srcFeature.Components.GetComponent(iCNO).Recordset;

            myUtil.CopyFields(cableFeature, rs1, iCNO, "ALPHA_CODE");
            myUtil.CopyFields(cableFeature, rs1, iCNO, "ARMOUR");
            myUtil.CopyFields(cableFeature, rs1, iCNO, "BLOCK_NUM");
            myUtil.CopyFields(cableFeature, rs1, iCNO, "CABLE_CODE");
            myUtil.CopyFields(cableFeature, rs1, iCNO, "CMP_NUMBER_OF_COAX_TUBES");
            myUtil.CopyFields(cableFeature, rs1, iCNO, "COMPOSITION");
            myUtil.CopyFields(cableFeature, rs1, iCNO, "COPPER_SIZE");
            myUtil.CopyFields(cableFeature, rs1, iCNO, "COUNT_ANNOTATION");
            myUtil.CopyFields(cableFeature, rs1, iCNO, "CTYPE");
            myUtil.CopyFields(cableFeature, rs1, iCNO, "DESIGN_TYPE");
            myUtil.CopyFields(cableFeature, rs1, iCNO, "DIAMETER");
            myUtil.CopyFields(cableFeature, rs1, iCNO, "EFFECTIVE_PAIRS");
            myUtil.CopyFields(cableFeature, rs1, iCNO, "FIBER_MODE");
            myUtil.CopyFields(cableFeature, rs1, iCNO, "FIBER_SIZE");
            myUtil.CopyFields(cableFeature, rs1, iCNO, "FIBER_TAG_ID");
            myUtil.CopyFields(cableFeature, rs1, iCNO, "G3E_PAIRCOUNTPREFIX");
            myUtil.CopyFields(cableFeature, rs1, iCNO, "GAUGE");
            myUtil.CopyFields(cableFeature, rs1, iCNO, "HI_PR");
            myUtil.CopyFields(cableFeature, rs1, iCNO, "ID");
            myUtil.CopyFields(cableFeature, rs1, iCNO, "ITFACE_CODE");
            myUtil.CopyFields(cableFeature, rs1, iCNO, "LO_PR");
            myUtil.CopyFields(cableFeature, rs1, iCNO, "MDF_NUM");
            myUtil.CopyFields(cableFeature, rs1, iCNO, "NUMBER_OF_COAX_TUBES");
            myUtil.CopyFields(cableFeature, rs1, iCNO, "NUMBER_OF_VIDEO_PAIRS");
            myUtil.CopyFields(cableFeature, rs1, iCNO, "NUMCABLES");
            myUtil.CopyFields(cableFeature, rs1, iCNO, "ORIGINAL_USER");
            myUtil.CopyFields(cableFeature, rs1, iCNO, "OTHER_ID");
            myUtil.CopyFields(cableFeature, rs1, iCNO, "PERCENT_AERIAL");
            myUtil.CopyFields(cableFeature, rs1, iCNO, "PERCENT_BURIED");
            myUtil.CopyFields(cableFeature, rs1, iCNO, "PLACEMENT");
            myUtil.CopyFields(cableFeature, rs1, iCNO, "ROUTE_DETAIL");
            myUtil.CopyFields(cableFeature, rs1, iCNO, "RT_CODE");
            myUtil.CopyFields(cableFeature, rs1, iCNO, "SHEATH");
            myUtil.CopyFields(cableFeature, rs1, iCNO, "SOURCE_ID");
            myUtil.CopyFields(cableFeature, rs1, iCNO, "SOURCE_TYPE");
            myUtil.CopyFields(cableFeature, rs1, iCNO, "STUB_LABEL");
            myUtil.CopyFields(cableFeature, rs1, iCNO, "SUB_TERMCODE");
            myUtil.CopyFields(cableFeature, rs1, iCNO, "TERMINATION_ID");
            myUtil.CopyFields(cableFeature, rs1, iCNO, "TERMINATION_TYPE");
            myUtil.CopyFields(cableFeature, rs1, iCNO, "TEXT_FORMAT");
            myUtil.CopyFields(cableFeature, rs1, iCNO, "TEXT_VALUE");
            myUtil.CopyFields(cableFeature, rs1, iCNO, "TOTAL_SIZE");
            myUtil.CopyFields(cableFeature, rs1, iCNO, "USAGE");
            myUtil.CopyFields(cableFeature, rs1, iCNO, "VERT_BLOCK_HI");
            myUtil.CopyFields(cableFeature, rs1, iCNO, "VERT_BLOCK_LO");
            myUtil.CopyFields(cableFeature, rs1, iCNO, "VERT_NUM");
            myUtil.CopyFields(cableFeature, rs1, iCNO, "VERT_TYPE");

            if (m_transferType == "TRANSFER D-SIDE")
            {
                cableFeature.Components.GetComponent(7001).Recordset.Update("CUSAGE", "DISTRIBUTION");
                cableFeature.Components.GetComponent(7001).Recordset.Update("CABLE_CLASS", "D-CABLE");
                m_ITFACE_CODE = myUtil.rsField(rs1, "ITFACE_CODE");
                m_RT_CODE = myUtil.rsField(rs1, "RT_CODE");
            }
            else
            {
                cableFeature.Components.GetComponent(7001).Recordset.Update("CUSAGE", "MAIN");
                cableFeature.Components.GetComponent(7001).Recordset.Update("CABLE_CLASS", "E-CABLE");
                m_ITFACE_CODE = "";
                m_RT_CODE = "";
            }

            #endregion

            #region NR_Connect
            iCNO = 53;

            rs1 = cableFeature.Components.GetComponent(iCNO).Recordset;

            if (rs1.EOF)
            {
                rs1.AddNew("G3E_FID", cableFeature.FID);
                rs1.Update("G3E_FNO", cableFeature.FNO);
                rs1.Update("G3E_CID", 1);
            }
            else
            {
                rs1.MoveLast();
            }
            rs1.Update("IN_FNO", srcInFNO);
            rs1.Update("IN_FID", srcInFID);
            rs1.Update("OUT_FNO", JOINT_FNO);
            rs1.Update("OUT_FID", dstJointFID);

            #endregion

            //*** GTTransfer.m_oIGTTransactionManager.Commit();
            //*** GTTransfer.m_oIGTTransactionManager.RefreshDatabaseChanges();

            return iFID;
        }

        private double CableLength(IGTPolylineGeometry linepoints)
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

        #endregion

        #region 3.5 : Change Source Cable & Joint Class - stub -> normal
        //private void UpdateRecord(string tablename, string[] col, object[] val, int iFID)
        //{
        //    string ssql = "UPDATE " + tablename + " SET ";
        //    for (int i = 0; i < col.Length; i++)
        //    {
        //        if (val[i] is string)
        //            ssql += col[i] + " = '" + val[i] + "', ";
        //        else
        //            ssql += col[i] + " = " + val[i] + ", ";
        //    }
        //    ssql = ssql.Remove(ssql.Length - 2);
        //    ssql += " WHERE G3E_FID = " + iFID.ToString();

        //    int iR;
        //    GTClassFactory.Create<IGTApplication>().DataContext.Execute(ssql, out iR, (int)ADODB.CommandTypeEnum.adCmdText);
        //    GTClassFactory.Create<IGTApplication>().DataContext.Execute("COMMIT", out iR, (int)ADODB.CommandTypeEnum.adCmdText);
        //}

        private void UpdateRecipientClass(int cableFID, int jointFID)
        {
            //*** GTTransfer.m_oIGTTransactionManager.Begin("Update Recipient Class");

         return; // for we don't need to change the recipient class

            #region Update Recipient Cable Class
            short iCNO = 7001;
            srcFeature = GTTransfer.m_gtapp.DataContext.OpenFeature(srcFeature.FNO, srcFeature.FID);
            srcFeature.Components.GetComponent(iCNO).Recordset.MoveLast();
            if (m_transferType == "TRANSFER D-SIDE")
            {
                srcFeature.Components.GetComponent(iCNO).Recordset.Update("CABLE_CLASS", "D-CABLE");
                srcFeature.Components.GetComponent(iCNO).Recordset.Update("CUSAGE", "DISTRIBUTION");
            }
            else
            {
                srcFeature.Components.GetComponent(iCNO).Recordset.Update("CABLE_CLASS", "E-CABLE");
                srcFeature.Components.GetComponent(iCNO).Recordset.Update("CUSAGE", "MAIN");
            }

            #endregion

            #region Update Recipient Cable Joint
            iCNO = 10801;
            IGTKeyObject oJntFeature;
            oJntFeature = GTTransfer.m_gtapp.DataContext.OpenFeature(JOINT_FNO, jointFID);
            oJntFeature.Components.GetComponent(iCNO).Recordset.MoveLast();
            oJntFeature.Components.GetComponent(iCNO).Recordset.Update("JOINT_TYPE", "STRAIGHT");
            oJntFeature.Components.GetComponent(iCNO).Recordset.Update("SPLICE_CLASS",
                (m_transferType == "TRANSFER D-SIDE" ? "JOINT D-SIDE" : "JOINT E-SIDE"));

            CopyJointIn2Out(oJntFeature, srcFeature.Components.GetComponent(53).Recordset);

            #endregion

            //*** GTTransfer.m_oIGTTransactionManager.Commit();
            //*** GTTransfer.m_oIGTTransactionManager.RefreshDatabaseChanges();

        }

        private static void CopyJointIn2Out(IGTKeyObject outJoint, ADODB.Recordset rsCableNRConnect)
        {
            try
            {
                rsCableNRConnect.MoveLast();
                int iFID = int.Parse(rsCableNRConnect.Fields["IN_FID"].Value.ToString());
                short iFNO = short.Parse(rsCableNRConnect.Fields["IN_FNO"].Value.ToString());

                IGTKeyObject inJoint = GTTransfer.m_gtapp.DataContext.OpenFeature(iFNO, iFID);

                // copy min material from existing joint
                short iCNO = 51; // netelem
                inJoint.Components.GetComponent(iCNO).Recordset.MoveLast();
                myUtil.CopyFields(outJoint, inJoint.Components.GetComponent(iCNO).Recordset, iCNO, "MIN_MATERIAL");

                // copy attribute from inJoint
                iCNO = 10801;
                ADODB.Recordset rs = inJoint.Components.GetComponent(iCNO).Recordset;
                rs.MoveLast();

                myUtil.CopyFields(outJoint, rs, iCNO, "CABLE_CODE");
                myUtil.CopyFields(outJoint, rs, iCNO, "CASE_TYPE");
                myUtil.CopyFields(outJoint, rs, iCNO, "CLOSURE_TYPE");
                myUtil.CopyFields(outJoint, rs, iCNO, "DBLOSS");
                myUtil.CopyFields(outJoint, rs, iCNO, "DCR");
                myUtil.CopyFields(outJoint, rs, iCNO, "ID");
                myUtil.CopyFields(outJoint, rs, iCNO, "ITFACE_CODE");
                myUtil.CopyFields(outJoint, rs, iCNO, "MODEL");
                myUtil.CopyFields(outJoint, rs, iCNO, "ORIGINAL_USER");
                myUtil.CopyFields(outJoint, rs, iCNO, "RT_CODE");
                myUtil.CopyFields(outJoint, rs, iCNO, "SPLICE_ADMIN_TYPE");
                myUtil.CopyFields(outJoint, rs, iCNO, "SPLICE_CONNECTION_TYPE");
                myUtil.CopyFields(outJoint, rs, iCNO, "SPLICE_LOSS");
                myUtil.CopyFields(outJoint, rs, iCNO, "SPLICE_NOTE");
                myUtil.CopyFields(outJoint, rs, iCNO, "SPLICE_PHYSICAL_TYPE");

                outJoint.Components.GetComponent(iCNO).Recordset.Update("DIST_FROM_EXC",
                    short.Parse(myUtil.rsField(outJoint.Components.GetComponent(iCNO).Recordset, "DIST_FROM_EXC")) +
                    short.Parse(myUtil.rsField(rs, "DIST_FROM_EXC")));

            }
            catch
            { }
        }

        #endregion

        #region 4 : Update Downstream - EXC_ABB or ITFACE_CODE
        /// <summary>
        /// Update all downstreams cable and features connected to the donor joint
        /// </summary>
        /// <param name="jointFID">FID of donor joint</param>
        private void UpdateDownstreams(int jointFID)
        {
            try
            {
                UpdateDownstream(jointFID);
                return;

                int iFID = 0;

                string ssql = "SELECT G3E_FID FROM GC_NR_CONNECT WHERE IN_FID = " + jointFID.ToString();

                ADODB.Recordset rs = new ADODB.Recordset();
                rs = GTTransfer.m_gtapp.DataContext.OpenRecordset(ssql,
                ADODB.CursorTypeEnum.adOpenStatic, ADODB.LockTypeEnum.adLockOptimistic, 1);

                if (m_transferType == "TRANSFER E-SIDE")
                    while (!rs.EOF)
                    {
                        UpdateDownstream(int.Parse(myUtil.rsField(rs, "G3E_FID")));
                        UpdateDownstreamAttachment(int.Parse(myUtil.rsField(rs, "G3E_FID")));
                        rs.MoveNext();
                    }
                else
                    while (!rs.EOF)
                    {
                        UpdateDownstream(int.Parse(myUtil.rsField(rs, "G3E_FID")));
                        rs.MoveNext();
                    }

                rs.Close();
                rs = null;


            }
            catch (Exception ex)
            {
                GTTransfer.m_CustomForm.ProgressMessage("Fail to read downstream cable from this joint\r\n" + ex.Message);
            }
        }

        /// <summary>
        /// Update downstream cable and all features connected to it
        /// </summary>
        /// <param name="cableFID">Cable FID</param>
        private void UpdateDownstream(int donorFID)
        {
            ChangeSpliceCableCode(donorFID, 10800);

            ADODB.Recordset rs = new ADODB.Recordset();
            string ssql = "SELECT IN_FNO, IN_FID, G3E_FNO, G3E_FID, OUT_FNO, OUT_FID FROM GC_NR_CONNECT " +
                "START WITH IN_FID = " + donorFID.ToString() + " CONNECT BY NOCYCLE " +
                "PRIOR OUT_FID = IN_FID AND IN_FNO IN (10800, 13100, 6200) AND IN_FID <> 0 AND OUT_FID <> 0 ";
            rs = GTClassFactory.Create<IGTApplication>().DataContext.OpenRecordset(
                ssql, ADODB.CursorTypeEnum.adOpenStatic, ADODB.LockTypeEnum.adLockOptimistic, 1);

            int iFID = 0;
            short iFNO = 0;
            int oFID = 0;
            short oFNO = 0;
            int cFID = 0;
            short cFNO = 0;

            if (m_transferType == "TRANSFER E-SIDE")
                GTTransfer.m_CustomForm.AddReport("Update Donor with new exchange abbreviation : " + m_EXC_ABB);
            else
                GTTransfer.m_CustomForm.AddReport("Update Donor with new itface code : " + m_ITFACE_CODE);

            while (!rs.EOF)
            {
                try
                {
                    iFNO = short.Parse(myUtil.rsField(rs, "IN_FNO"));
                    iFID = int.Parse(myUtil.rsField(rs, "IN_FID"));
                    oFNO = short.Parse(myUtil.rsField(rs, "OUT_FNO"));
                    oFID = int.Parse(myUtil.rsField(rs, "OUT_FID"));
                    cFNO = short.Parse(myUtil.rsField(rs, "G3E_FNO"));
                    cFID = int.Parse(myUtil.rsField(rs, "G3E_FID"));

                    GTTransfer.m_CustomForm.IncreaseProgressBar(1);

                    UpdateNetelem(cFID, cFNO);
                    UpdateNetelem(oFID, oFNO);

                    if (oFNO == 10800) // OUT_FNO is a splice
                    {
                        #region Update Cable and Splice

                        UpdateCableAttribute(cFID, 7000);
                        ChangeSpliceCableCode(oFID, 10800);
                        #endregion
                    }
                    else
                    {

                        if (!OffsetCableCount(cFID, ""))
                            OffsetCableCount(cFID, "D"); // GEO or DETAIL

                        string tablename = GetTableName(oFNO);
                        switch (oFNO)
                        {
                            #region TRANSER E-SIDE
                            case 10300:
                                UpdateEndCableAttribute_E(cFID, cFNO, oFID, oFNO);
                                break;
                            case 6300:
                                UpdateTermAttribute_E(oFID, oFNO, tablename);
                                UpdateEndCableAttribute_E(cFID, cFNO, oFID, oFNO);
                                break;
                            #endregion

                            #region TRANSFER D-SIDE
                            case 13000:
                            case 13100:
                                UpdateTermAttribute(oFID, oFNO, tablename);
                                UpdateEndCableAttribute(cFID, cFNO, oFID, oFNO);
                                break;
                            #endregion

                            default:
                                if (m_transferType == "TRANSFER E-SIDE")
                                    UpdateTermAttribute_E(oFID, oFNO, tablename);
                                else
                                    UpdateTermAttribute(oFID, oFNO, tablename);
                                break;
                        }
                    }
                }
                catch { }
                rs.MoveNext();
            }
            rs.Close();
            rs = null;
        }

        private void UpdateDownstreamAttachment(int cableFID)
        {
            // create downstream trace table
            string trace_file = clsTrace.TraceDownstream(cableFID);
            // query the trace table
            string ssql =
                "SELECT G3E_FID, G3E_FNO FROM GC_OWNERSHIP WHERE OWNER1_ID IN (" +
                "SELECT G3E_ID FROM GC_OWNERSHIP WHERE G3E_FID IN (SELECT DISTINCT G3E_FID FROM (" +
                "SELECT A.G3E_FID FROM TRACERESULT A, TRACEID B WHERE B.G3E_ID = A.G3E_TNO AND B.G3E_NAME = '" +
                trace_file + "' UNION ALL " +
                "SELECT A.G3E_NODE1 FROM TRACERESULT A, TRACEID B WHERE B.G3E_ID = A.G3E_TNO AND B.G3E_NAME = '" +
                trace_file + "' UNION ALL " +
                "SELECT A.G3E_NODE2 FROM TRACERESULT A, TRACEID B WHERE B.G3E_ID = A.G3E_TNO AND B.G3E_NAME = '" +
                trace_file + "')))";

            ADODB.Recordset rs = new ADODB.Recordset();
            rs = GTClassFactory.Create<IGTApplication>().DataContext.OpenRecordset(ssql,
                ADODB.CursorTypeEnum.adOpenStatic, ADODB.LockTypeEnum.adLockOptimistic, 1);

            int iFID = 0;
            short iFNO = 0;

            if (m_transferType == "TRANSFER E-SIDE")
                GTTransfer.m_CustomForm.AddReport("Update Donor with new exchange abbreviation : " + m_EXC_ABB);
            else
                return; // attachment only available at E-SIDE

            while (!rs.EOF)
            {
                try
                {
                    iFNO = short.Parse(myUtil.rsField(rs, "G3E_FNO"));
                    iFID = int.Parse(myUtil.rsField(rs, "G3E_FID"));

                    GTTransfer.m_CustomForm.IncreaseProgressBar(1);

                    UpdateNetelem(iFID, iFNO);

                    switch (iFNO)
                    {
                        // attachment to be explore ???
                        case 6400: UpdateTermCableCode(iFID, iFNO, "GC_CONTGAUGE"); break;
                        case 6500: UpdateTermCableCode(iFID, iFNO, "GC_CONTALARM"); break;
                        case 6600: UpdateTermCableCode(iFID, iFNO, "GC_GASSEAL"); break;
                        case 6700: UpdateTermCableCode(iFID, iFNO, "GC_TESTPNT"); break;
                        case 6800: UpdateTermCableCode(iFID, iFNO, "GC_LDCOIL"); break;
                        case 6900: UpdateTermCableCode(iFID, iFNO, "GC_TRNSDCR"); break;
                    }

                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Error Updating Downstream : " + ex.Message);
                }
                rs.MoveNext();
            }
            rs.Close();
            rs = null;
            // delete the trace table after done
            clsTrace.DeleteTrace(trace_file);
        }

        private void UpdateNetelem(int iFID, short iFNO)
        {
            myUtil.ADODB_ExecuteNonQuery
                ("UPDATE GC_NETELEM SET EXC_ABB = '" + m_EXC_ABB + "', JOB_ID = '" + GTTransfer.JOB_ID() +
                "', FEATURE_STATE = 'MOD' WHERE G3E_FID = " + iFID.ToString());

            GTTransfer.m_CustomForm.AddReport("Update GC_NETELEM - FNO : " + iFNO + ", FID : " + iFID);
        }
        private void UpdateTermCableCode(int iFID, short iFNO, string tablename)
        {
            myUtil.ADODB_ExecuteNonQuery("UPDATE " + tablename + " SET CABLE_CODE = '" + m_CABLE_CODE + "' " +
                "WHERE G3E_FID = " + iFID.ToString());
            clsReport.AddTermPoint(tablename.Substring(3), iFID, iFNO);
        }
        private void UpdateTermAttribute(int iFID, short iFNO, string tablename)
        {
            myUtil.ADODB_ExecuteNonQuery("UPDATE " + tablename + " SET ITFACE_CODE = '" + m_ITFACE_CODE +
                "',RT_CODE = '" + m_RT_CODE +
                "', CABLE_CODE = '" + m_CABLE_CODE + "' WHERE G3E_FID = " + iFID.ToString());
            clsReport.AddTermPoint(tablename.Substring(3), iFID, iFNO);
        }

        #region Update Cable - m.zam @ 2012-10-16
        private void UpdateCableAttribute(int iFID, short iFNO)
        {
            string ssql = "UPDATE GC_CBL SET ITFACE_CODE = '" + m_ITFACE_CODE + "',RT_CODE = '" + m_RT_CODE +
                    "', CABLE_CODE = '" + m_CABLE_CODE + "'" +
                    ", COUNT_TRANSFER = '(T)' WHERE G3E_FID = " + iFID.ToString();
            myUtil.ADODB_ExecuteNonQuery(ssql);
            clsReport.AddFeature("CABLE", iFID);
        }
        private void UpdateEndCableAttribute(int iFID, short iFNO, int oFID, short oFNO)
        {
            string prefix = GetCountPrefix(oFID, oFNO);

            string ssql = "UPDATE GC_CBL SET ITFACE_CODE = '" + m_ITFACE_CODE + "', RT_CODE = '" + m_RT_CODE +
                    "', CABLE_CODE = '" + m_CABLE_CODE + "', " +
                    "COUNT_TRANSFER = '" + prefix + "'||COUNT_ANNOTATION WHERE G3E_FID = " + iFID.ToString();

            myUtil.ADODB_ExecuteNonQuery(ssql);
            clsReport.AddFeature("CABLE", iFID);
        }
        private string GetCountPrefix(int iFID, short iFNO)
        {
            string ssql = "SELECT {1} FROM {0} WHERE G3E_FID = " + iFID;
            switch (iFNO)
            {
                case 6300: ssql = string.Format(ssql, "GC_DDP", "DDP_NUM"); break;
                case 13000: ssql = string.Format(ssql, "GC_DP", "DP_NUM"); break;
                default: return "T, ";
            }

            ADODB.Recordset rs = new ADODB.Recordset();
            rs = GTTransfer.m_gtapp.DataContext.OpenRecordset(ssql,
            ADODB.CursorTypeEnum.adOpenStatic, ADODB.LockTypeEnum.adLockOptimistic, 1);

            if (!rs.EOF)
            {
                rs.MoveFirst();
                return "T [" + rs.Fields[0].Value.ToString() + "], ";
            }
            else
            {
                return "T, ";
            }
        }
        #endregion

        private void ChangeSpliceCableCode(int iFID, short iFNO)
        {
            myUtil.ADODB_ExecuteNonQuery("UPDATE GC_SPLICE SET ITFACE_CODE = '" + m_ITFACE_CODE +
                "',RT_CODE = '" + m_RT_CODE + "', CABLE_CODE = '" + m_CABLE_CODE + 
                "' WHERE G3E_FID = " + iFID.ToString());
            clsReport.AddFeature("SPLICE", iFID);
        }

        #endregion

        #region 5 : Add Transfer Feature 14-07-2012

        /*
         * CNO 6101 --> GC_TRANSFER (Attribute)
         * CNO 6112 --> GC_TRANSFERLDR_L (Leader Line)
         * CNO 6120 --> GC_TRANSFER_S (Transfer Symbol)
         * CNO 6130 --> GC_TRANSFER_T (Transfer Text)
         */
        public void UpdateTransferObject(int TR_FID, List<IGTPoint> arrPoint)
        {
            GTClassFactory.Create<IGTApplication>().BeginWaitCursor();
            try
            {
                short TR_CNO = 0;
                short TR_FNO = 6100;

                #region Transfer Object : Preparing Transfer Symbol and Transfer Text according to X-coordinate, Y-coordinate
                IGTPoint ePoint = arrPoint[arrPoint.Count - 1];

                #region Transfer Object - Transfer symbol geometry
                IGTPointGeometry TR_Point;
                TR_Point = GTClassFactory.Create<IGTPointGeometry>();
                TR_Point.Origin = ePoint;
                #endregion

                #region Transfer Object - Transfer leader line geometry

                IGTPolylineGeometry TR_Line = GTClassFactory.Create<IGTPolylineGeometry>();
                IGTPolylineGeometry TR_Line2 = GTClassFactory.Create<IGTPolylineGeometry>();

                for (int i = 0; i < arrPoint.Count; i++)
                {
                    TR_Line.Points.Add(arrPoint[i]);
                    TR_Line2.Points.Add(arrPoint[i]);
                }
                TR_Line2.Points[0] = m_LeaderPoint;
                TR_Line2.Points[1] = m_LeaderPoint;

                #endregion

                #region Transfer Object - Transfer text geometry
                IGTTextPointGeometry TR_Text;
                TR_Text = GTClassFactory.Create<IGTTextPointGeometry>();
                ePoint.Y -= 7;
                TR_Text.Origin = ePoint;
                TR_Text.Rotation = 0;
                #endregion

                #endregion

                #region Transfer Object : Open transfer feature

                GTTransfer.m_oIGTTransactionManager.Begin("Update Transfer Object");
                IGTKeyObject TR_Feature = GTTransfer.m_gtapp.DataContext.OpenFeature(TR_FNO, TR_FID);

                #endregion

                #region Transfer Object : Create Leader Line

                TR_CNO = 6112; // GC_TRANSFERLDR_L
                if (TR_Feature.Components.GetComponent(TR_CNO).Recordset.EOF)
                {
                    TR_Feature.Components.GetComponent(TR_CNO).Recordset.AddNew("G3E_FID", TR_FID);
                    TR_Feature.Components.GetComponent(TR_CNO).Recordset.Update("G3E_FNO", TR_FNO);
                    TR_Feature.Components.GetComponent(TR_CNO).Recordset.Update("G3E_CID", 1);
                }
                else
                    TR_Feature.Components.GetComponent(TR_CNO).Recordset.MoveLast();

                TR_Feature.Components.GetComponent(TR_CNO).Geometry = TR_Line;

                TR_Feature.Components.GetComponent(TR_CNO).Recordset.AddNew("G3E_FID", TR_FID);
                TR_Feature.Components.GetComponent(TR_CNO).Recordset.Update("G3E_FNO", TR_FNO);
                TR_Feature.Components.GetComponent(TR_CNO).Recordset.Update("G3E_CID", 2);
                TR_Feature.Components.GetComponent(TR_CNO).Recordset.MoveLast();
                TR_Feature.Components.GetComponent(TR_CNO).Geometry = TR_Line2;

                #endregion

                #region Transfer Object : Create Transfer Symbol

                TR_CNO = 6120; // GC_TRANSFER_S

                if (TR_Feature.Components.GetComponent(TR_CNO).Recordset.EOF)
                {
                    TR_Feature.Components.GetComponent(TR_CNO).Recordset.AddNew("G3E_FID", TR_FID);
                    TR_Feature.Components.GetComponent(TR_CNO).Recordset.Update("G3E_FNO", TR_FNO);
                    TR_Feature.Components.GetComponent(TR_CNO).Recordset.Update("G3E_CID", 1);
                }
                else
                    TR_Feature.Components.GetComponent(TR_CNO).Recordset.MoveLast();

                TR_Feature.Components.GetComponent(TR_CNO).Geometry = TR_Point;

                #endregion

                #region Transfer Object : Create Transfer Text
                TR_CNO = 6130; // GC_TRANSFER_T

                //if (TR_Feature.Components.GetComponent(TR_CNO).Recordset.EOF)
                //{
                //    TR_Feature.Components.GetComponent(TR_CNO).Recordset.AddNew("G3E_FID", TR_FID);
                //    TR_Feature.Components.GetComponent(TR_CNO).Recordset.Update("G3E_FNO", TR_FNO);
                //    TR_Feature.Components.GetComponent(TR_CNO).Recordset.Update("G3E_CID", 1);
                //}
                //else
                //{
                //    TR_Feature.Components.GetComponent(TR_CNO).Recordset.MoveLast();
                //}

                //TR_Feature.Components.GetComponent(TR_CNO).Geometry = TR_Text;
                #endregion

                #region Transfer Object - Initial End Drawing & Refresh Metadata Changing
                GTTransfer.m_oIGTTransactionManager.Commit();
                GTTransfer.m_oIGTTransactionManager.RefreshDatabaseChanges();
                #endregion

            }
            catch (Exception ex)
            {
                MessageBox.Show("Transfer Cable canceled. Error detected\r\n" + ex.Message, "Update Transfer Object");
                GTTransfer.m_oIGTTransactionManager.Rollback();
            }
            finally
            {
                GTClassFactory.Create<IGTApplication>().EndWaitCursor();
            }
        }

        public int CreateTransferObject(int upstreamFID, DateTime transferDate)
        {
            try
            {
                int TR_FID = -1;
                short TR_CNO = 0;
                short TR_FNO = 6100;

                #region Transfer Object : Initiate placement/drawing process

                //*** GTTransfer.m_oIGTTransactionManager.Begin("Create Transfer Object");
                IGTKeyObject oNewFeature;
                oNewFeature = GTClassFactory.Create<IGTApplication>().DataContext.NewFeature(TR_FNO);
                TR_FID = oNewFeature.FID;

                #endregion

                #region Transfer Object - Registering Transfer Attributes into the database
                TR_CNO = 6101; // GC_TRANSFER

                if (oNewFeature.Components.GetComponent(TR_CNO).Recordset.EOF)
                {
                    oNewFeature.Components.GetComponent(TR_CNO).Recordset.AddNew("G3E_FID", TR_FID);
                    oNewFeature.Components.GetComponent(TR_CNO).Recordset.Update("G3E_FNO", TR_FNO);
                    oNewFeature.Components.GetComponent(TR_CNO).Recordset.Update("G3E_CNO", TR_CNO);
                    oNewFeature.Components.GetComponent(TR_CNO).Recordset.Update("G3E_CID", 1);
                }
                else
                    oNewFeature.Components.GetComponent(TR_CNO).Recordset.MoveLast();

                oNewFeature.Components.GetComponent(TR_CNO).Recordset.Update("CABINET_FID", CabinetFID());
                oNewFeature.Components.GetComponent(TR_CNO).Recordset.Update("CABLE_CODE", m_CABLE_CODE);
                oNewFeature.Components.GetComponent(TR_CNO).Recordset.Update("DATE_TRANSFER", transferDate);
                oNewFeature.Components.GetComponent(TR_CNO).Recordset.Update("ITFACE_CODE", m_ITFACE_CODE);
                oNewFeature.Components.GetComponent(TR_CNO).Recordset.Update("RT_CODE", m_RT_CODE);
                oNewFeature.Components.GetComponent(TR_CNO).Recordset.Update("STUB_FID", srcSelectedCable.FID );
                oNewFeature.Components.GetComponent(TR_CNO).Recordset.Update("SRC_CABLE_FID", upstreamFID);
                oNewFeature.Components.GetComponent(TR_CNO).Recordset.Update("TERMINATION_FID", dstJointFID);
                oNewFeature.Components.GetComponent(TR_CNO).Recordset.Update("TERMINATION_TYPE", "INTERMEDIATE");
                oNewFeature.Components.GetComponent(TR_CNO).Recordset.Update("TRANS_TYPE", "AREA");
                oNewFeature.Components.GetComponent(TR_CNO).Recordset.Update("TRANSFER_CLASS",
                    (m_transferType == "TRANSFER E-SIDE" ? "E-SIDE" : "D-SIDE"));

                #endregion

                #region Transfer Object : Registering Transfer Netelem into the database
                TR_CNO = 51;  //GC_NETELEM            

                if (oNewFeature.Components.GetComponent(TR_CNO).Recordset.EOF)
                {
                    oNewFeature.Components.GetComponent(TR_CNO).Recordset.AddNew("G3E_FID", TR_FID);
                    oNewFeature.Components.GetComponent(TR_CNO).Recordset.Update("G3E_FNO", TR_FNO);
                    oNewFeature.Components.GetComponent(TR_CNO).Recordset.Update("G3E_CID", 1);
                }
                else
                {
                    oNewFeature.Components.GetComponent(TR_CNO).Recordset.MoveLast();
                }
                oNewFeature.Components.GetComponent(TR_CNO).Recordset.Update("MIN_MATERIAL", "-");
                oNewFeature.Components.GetComponent(TR_CNO).Recordset.Update("FEATURE_STATE", "PPF");
                oNewFeature.Components.GetComponent(TR_CNO).Recordset.Update("JOB_STATE", "PROPOSED");
                oNewFeature.Components.GetComponent(TR_CNO).Recordset.Update("EXC_ABB", m_EXC_ABB); // ???
                oNewFeature.Components.GetComponent(TR_CNO).Recordset.Update("YEAR_PLACED", DateTime.Now.ToString("yyyy"));

                #endregion

        
                clsOwnership.AddOwnership(TR_FNO, TR_FID, JOINT_FNO, dstJointFID);

                #region Transfer Object - Initial End Drawing & Refresh Metadata Changing
                //*** BeginTransaction was called at the begining of PerformCableTransfer

                //throw new System.Exception();
                GTTransfer.m_oIGTTransactionManager.Commit();
                GTTransfer.m_oIGTTransactionManager.RefreshDatabaseChanges();
                #endregion


                return TR_FID;

            }
            catch (Exception ex)
            {
                MessageBox.Show("Transfer Cable canceled. Error detected\r\n" + ex.Message, "Insert Transfer Object");
                GTTransfer.m_oIGTTransactionManager.Rollback();
                return -1;
            }

        }

        private int CabinetFID()
        {
            try
            {
                string ssql;

                if (m_ITFACE_CODE.Length > 0 && m_ITFACE_CODE != "***")
                {
                    ssql = "SELECT A.G3E_FID FROM GC_ITFACE A, GC_NETELEM B " +
                        "WHERE A.G3E_FID = B.G3E_FID AND A.ITFACE_CODE = '" + m_ITFACE_CODE + "' " +
                        "AND B.EXC_ABB = '" + m_EXC_ABB + "'";
                }
                else
                {
                    ssql = 
                    "SELECT A.G3E_FID FROM GC_RT A, GC_NETELEM B " +
                    "WHERE A.G3E_FID = B.G3E_FID AND A.RT_CODE = '" + m_RT_CODE + "' " +
                    "AND B.EXC_ABB = '" + m_EXC_ABB + "' ";
                    
                    ssql += "UNION " + 
                    "SELECT A.G3E_FID FROM GC_MSAN A, GC_NETELEM B " +
                    "WHERE A.G3E_FID = B.G3E_FID AND A.RT_CODE = '" + m_RT_CODE + "' " +
                    "AND B.EXC_ABB = '" + m_EXC_ABB + "' ";

                    ssql += "UNION " +
                    "SELECT A.G3E_FID FROM GC_VDSL2 A, GC_NETELEM B " +
                    "WHERE A.G3E_FID = B.G3E_FID AND A.RT_CODE = '" + m_RT_CODE + "' " +
                    "AND B.EXC_ABB = '" + m_EXC_ABB + "'";

                }

                ADODB.Recordset rs = new ADODB.Recordset();
                rs = GTClassFactory.Create<IGTApplication>().DataContext.OpenRecordset(ssql,
                    ADODB.CursorTypeEnum.adOpenStatic, ADODB.LockTypeEnum.adLockOptimistic, 1);

                if (!rs.EOF)
                {
                    rs.MoveFirst();
                    return int.Parse(myUtil.rsField(rs, "G3E_FID"));
                }
                else
                {
                    return -1;
                }

            }
            catch
            {
                return -1;
            }
        }
        #endregion

        /*************************************************************************
         * private void hiliteFeature(int isFID, short isFNO)
         * private IGTPoint MoveBack2Point(IGTPolylineGeometry geoPoint, int startPoint)
         **************************************************************************/
        #region "Local General Method"
        private IGTPoint MoveBack2Point(IGTPolylineGeometry geoPoint, int startPoint)
        {
            IGTPoint newPoint = GTClassFactory.Create<IGTPoint>();

            double x1 = geoPoint.Points[startPoint].X;
            double y1 = geoPoint.Points[startPoint].Y;

            double x2 = geoPoint.Points[startPoint + 1].X;
            double y2 = geoPoint.Points[startPoint + 1].Y;

            double diffx = Math.Abs(x1 - x2);
            double diffy = Math.Abs(y1 - y2);

            if (diffx < 1) newPoint.X = x2;

            if (diffx > diffy)
            {
                newPoint.X = (x1 > x2 ? x2 + 3 : x2 - 3);
                if (diffy < 1)
                    newPoint.Y = y2;
                else
                {
                    diffy = (3 / diffx) * diffy;
                    newPoint.Y = (y1 > y2 ? y2 + diffy : y2 - diffy);
                }
            }
            else
            {
                newPoint.Y = (y1 > y2 ? y2 + 3 : y2 - 3);
                if (diffx < 1)
                    newPoint.X = x2;
                else
                {
                    diffx = (3 / diffy) * diffx;
                    newPoint.X = (x1 > x2 ? x2 + diffx : x2 - diffx);

                }
            }
            return newPoint;
        }

        private void hiliteFeature(int isFID, short isFNO)
        {
            try
            {
                IGTDDCKeyObjects oGTKeyObjs;
                GTTransfer.m_gtapp.SelectedObjects.Clear();

                oGTKeyObjs = (IGTDDCKeyObjects)GTTransfer.m_gtapp.DataContext.GetDDCKeyObjects(isFNO, isFID, GTComponentGeometryConstants.gtddcgAllGeographic);
                for (int i = 0; i < oGTKeyObjs.Count; i++)
                {
                    GTTransfer.m_gtapp.SelectedObjects.Add(GTSelectModeConstants.gtsosmAllDisplayedComponentsInActiveLegend, oGTKeyObjs[i]);
                }
                GTTransfer.m_gtapp.ActiveMapWindow.CenterSelectedObjects();
                GTTransfer.m_gtapp.ActiveMapWindow.DisplayScale = 500;
                GTTransfer.m_gtapp.RefreshWindows();
                oGTKeyObjs = null;

            }
            catch (Exception ex)
            {
                GTTransfer.m_CustomForm.ProgressMessage(ex.Message);
            }
        }
        #endregion

        #region COUNT - 16-07-2012

        private void TransferCount(int iFID)
        {
            try
            /*
            COUNT_TRANSFER = COUNT_ANNOTATION
            */
            {
                string ssql = "UPDATE GC_CBL SET COUNT_TRANSFER = COUNT_ANNOTATION WHERE G3E_FID = " + iFID.ToString();

                int iR;
                GTClassFactory.Create<IGTApplication>().DataContext.Execute(ssql, out iR, (int)ADODB.CommandTypeEnum.adCmdText);
                GTClassFactory.Create<IGTApplication>().DataContext.Execute("COMMIT", out iR, (int)ADODB.CommandTypeEnum.adCmdText);

                ADODB.Recordset rs = new ADODB.Recordset();
                rs = GTClassFactory.Create<IGTApplication>().DataContext.OpenRecordset(ssql,
                    ADODB.CursorTypeEnum.adOpenStatic, ADODB.LockTypeEnum.adLockOptimistic, 1);

            }
            catch
            {
                // some of the table doesnot have itface_code
            }
        }

        private bool OffsetCableCount(int iFID, string tableprefix)
        {
            string ssql = "INSERT INTO " + tableprefix + "GC_CBLCNT_BL_T (G3E_ID, G3E_FNO, G3E_FID, G3E_CNO, G3E_CID, G3E_GEOMETRY) ";
            ssql += "(SELECT  " + tableprefix + "GC_CBLCNT_BL_T_SEQ.NEXTVAL, 7000, G3E_FID, 7032, 1, ";
            ssql += tableprefix + "GC_OSP_COP_VAL.OFSET_GEOM_T(G3E_GEOMETRY, -1) FROM " + tableprefix + "GC_CBLCNT_TL_T WHERE G3E_FID = " + iFID + ")";

            try
            {
                if (!myUtil.FIDFound(tableprefix + "GC_CBLCNT_BL_T", iFID))
                {
                    int iR;
                    GTClassFactory.Create<IGTApplication>().DataContext.Execute(ssql, out iR, (int)ADODB.CommandTypeEnum.adCmdText);
                    GTClassFactory.Create<IGTApplication>().DataContext.Execute("COMMIT", out iR, (int)ADODB.CommandTypeEnum.adCmdText);
                }
                GTTransfer.m_CustomForm.AddReport("Offset cable count - FID:" + iFID.ToString());
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error Offset : FID - " + iFID.ToString() + "\r\n" + ssql + "\r\n" + ex.Message);
                return false;
            }
        }
        #endregion

        #region TRANSFER E-SIDE 2012-10-30

        private void UpdateTermAttribute_E(int iFID, short iFNO, string tablename)
        {
            myUtil.ADODB_ExecuteNonQuery("UPDATE " + tablename + " SET CABLE_CODE = '" + m_CABLE_CODE +
                "' WHERE G3E_FID = " + iFID.ToString());
            clsReport.AddTermPoint(tablename.Substring(3), iFID, iFNO);
        }

        public string GetCableCode_E(int iFID, string tablename, int featuretype)
        {
            string ssql = "SELECT CABLE_CODE FROM " + tablename + " WHERE G3E_FID = " + iFID;

            ADODB.Recordset rs = new ADODB.Recordset();
            rs = GTTransfer.m_gtapp.DataContext.OpenRecordset(ssql,
            ADODB.CursorTypeEnum.adOpenStatic, ADODB.LockTypeEnum.adLockOptimistic, 1);

            if (!rs.EOF)
            {
                if (featuretype == 0) // it's a recipient cable
                    clsReport.rcpnt.CABLE_CODE = myUtil.rsField(rs, "CABLE_CODE");
                else // it's a donor joint
                    clsReport.donor.CABLE_CODE = myUtil.rsField(rs, "CABLE_CODE");
                return (myUtil.rsField(rs, "CABLE_CODE"));
            }
            else
                return "-";
        }
        private void UpdateEndCableAttribute_E(int iFID, short iFNO, int oFID, short oFNO)
        {
            string prefix = GetCountPrefix(oFID, oFNO);

            string ssql = "UPDATE GC_CBL SET CABLE_CODE = '" + m_CABLE_CODE + "', " +
                    "COUNT_TRANSFER = '" + prefix + "'||COUNT_ANNOTATION WHERE G3E_FID = " + iFID.ToString();

            myUtil.ADODB_ExecuteNonQuery(ssql);
            clsReport.AddFeature("CABLE", iFID);
        }

        private string GetTableName(short iFNO)
        {
            short CNO = (short)(iFNO + 1);
            string ssql = "SELECT G3E_TABLE FROM G3E_COMPONENT WHERE G3E_CNO = " + CNO.ToString();

            ADODB.Recordset rs = new ADODB.Recordset();
            rs = GTTransfer.m_gtapp.DataContext.OpenRecordset(ssql,
            ADODB.CursorTypeEnum.adOpenStatic, ADODB.LockTypeEnum.adLockOptimistic, 1);

            if (!rs.EOF)
                return (myUtil.rsField(rs, "G3E_TABLE"));
            else
                return "NOT DEFINE";

        }
        #endregion
    }
}

