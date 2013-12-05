/*
 * 
 * 
 * edited : m.zam @ 12-09-2012
 * issues : connecting copper cable to FTTS RT/MSAN - copy RT_CODE into GC_CBL.ITFACE_CODE
 *          need to add column CAB_TYPE in GC_CBL, GC_SPLICE and AG_CABLE_JOINT
 *          value CAB_TYPE = GC_ITFACE / GC_MSAN / GC_RT : indicate type of cabinet the cable connected to
 * 
 * edited : m.zam @ 19-09-2012
 * issues : cancel using CAB_TYPE, instead just copy RT/MSAN RT_CODE into both GC_CBL.ITFACE_CODE and GC_CBL.RT_CODE
 * 
 *          need to run this script >>> ALTER TABLE AG_CABLE_JOINT ADD RT_CODE VARCHAR2(9);
 *          also need to add RT_CODE column in GC_DP and GC_PDP
 * 
 * edited : m.zam @ 26-09-2012
 * issues : auto assign DP cable code
 * 
 */

using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Intergraph.GTechnology.Interfaces;
using Intergraph.GTechnology.API;
using ADODB;

namespace NEPS.OSP.COPPER.VALIDATE
{
    class GetParentITFaceCode : Intergraph.GTechnology.Interfaces.IGTFunctional
    {
        #region IGTFunctional Members

        private GTArguments m_GTArguments = null;
        private IGTComponents m_GTComponents;
        private IGTDataContext m_GTDataContext = null;
        private Logger log;

        public Intergraph.GTechnology.API.GTArguments Arguments
        {
            get
            {
                throw new System.NotImplementedException();
            }
            set
            {
                m_GTArguments = value;
            }
        }

        #region standard code from sample
        public string ComponentName
        {
            get
            {
                throw new System.NotImplementedException();
            }
            set
            {
            }
        }

        public Intergraph.GTechnology.API.IGTComponents Components
        {
            get
            {
                return m_GTComponents;
            }
            set
            {
                m_GTComponents = value;
            }
        }

        public Intergraph.GTechnology.API.IGTDataContext DataContext
        {
            get
            {
                return m_GTDataContext;
            }
            set
            {
                m_GTDataContext = value;
            }
        }

        public string FieldName
        {
            get
            {
                throw new System.NotImplementedException();
            }
            set
            {
            }
        }

        public Intergraph.GTechnology.API.IGTFieldValue FieldValueBeforeChange
        {
            get
            {
                throw new System.NotImplementedException();
            }
            set
            {
            }
        }

        public string ProcessingMode
        {
            get
            {
                throw new System.NotImplementedException();
            }
            set
            {
            }
        }

        public Intergraph.GTechnology.Interfaces.GTFunctionalTypeConstants Type
        {
            get
            {
                throw new System.NotImplementedException();
            }
            set
            {
            }
        }

        public void Validate(out string[] ErrorPriorityArray, out string[] ErrorMessageArray)
        {
            bool bErrors = false;
            if (bErrors)
            {
                ErrorPriorityArray = new string[2];
                ErrorMessageArray = new string[2];
                ErrorPriorityArray[0] = "P1";
                ErrorMessageArray[0] = "First .NET Sample Error";
                ErrorPriorityArray[1] = "P3";
                ErrorMessageArray[1] = "Second .NET Sample Error";
            }
            else
            {
                ErrorPriorityArray = null;
                ErrorMessageArray = null;
            }

            GTClassFactory.Create<IGTApplication>().Application.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "From .NET Sample: Inside Validate");
        }

        public void Delete()
        {
            GTClassFactory.Create<IGTApplication>().Application.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "From .NET Sample: Inside Delete");
        }

        #endregion

        public void Execute()
        {
            try
            {
                // Declaration
                string strJobID = string.Empty;
                string strSql = string.Empty;
                string excABB = string.Empty;
                string cabType = "";

                string filename = "COPPER_PLACE_CABLE";
                log = Logger.getInstance();
                log.OpenFile(filename);

                log.WriteLog("Start Functional Interface for Cable Placement");

                //Get Exchange
                IGTComponent oComp = m_GTComponents.GetComponent(51); // GC_NETELEM
                log.WriteLog("OPEN GC_NETELEM");
                excABB = oComp.Recordset.Fields["EXC_ABB"].Value.ToString();
                if (excABB.Length == 0)
                {
                    ADODB.Recordset rsE = new ADODB.Recordset();
                    rsE = m_GTDataContext.OpenRecordset("Select EXC_ABB from G3E_JOB where G3E_IDENTIFIER = '" + m_GTDataContext.ActiveJob.ToString() + "'", ADODB.CursorTypeEnum.adOpenDynamic, ADODB.LockTypeEnum.adLockOptimistic, (int)ADODB.CommandTypeEnum.adCmdText);
                    if (rsE.RecordCount > 0)
                    {
                        excABB = rsE.Fields[0].Value.ToString();
                    }
                    rsE.Close();
                    log.WriteLog("EXC ABB (JOB) = " + excABB);
                }

                oComp = m_GTComponents.GetComponent(53); // NR_CONNECT
                log.WriteLog("OPEN NR_CONNECT");
                oComp.Recordset.MoveFirst();

                // Check Ownership to Joint
                int cFID = int.Parse(oComp.Recordset.Fields["G3E_FID"].Value.ToString());
                int InFID = int.Parse(oComp.Recordset.Fields["IN_FID"].Value.ToString());
                int InFNO = int.Parse(oComp.Recordset.Fields["IN_FNO"].Value.ToString());
                int OutFID = int.Parse(oComp.Recordset.Fields["OUT_FID"].Value.ToString());
                int OutFNO = int.Parse(oComp.Recordset.Fields["OUT_FNO"].Value.ToString());
                log.WriteLog("CABLE FID :" + cFID + " - IN : " + InFNO + "," + InFID + " - OUT : " + OutFNO + "," + OutFID);

                // Component for cable
                oComp = m_GTComponents.GetComponent(7001);
                // 12-09-2012 Get CAB_TYPE
                ADODB.Recordset rsSQL = new ADODB.Recordset();
                switch (InFNO)
                {
                    case 10300:
                        strSql = "SELECT ITFACE_CODE, '' RT_CODE FROM GC_ITFACE WHERE G3E_FID = " + InFID;
                        break;
                    case 9600:
                        strSql = "SELECT '' ITFACE_CODE, RT_CODE FROM GC_RT WHERE G3E_FID = " + InFID;
                        break;
                    case 9100:
                        strSql = "SELECT '' ITFACE_CODE, RT_CODE FROM GC_MSAN WHERE G3E_FID = " + InFID;
                        break;
                    default:
                        strSql = "SELECT ITFACE_CODE, RT_CODE, CABLE_CODE FROM GC_SPLICE WHERE G3E_FID = " + InFID;
                        break;
                }
                log.WriteLog(strSql);
                rsSQL = m_GTDataContext.OpenRecordset(strSql, ADODB.CursorTypeEnum.adOpenDynamic, ADODB.LockTypeEnum.adLockOptimistic, (int)ADODB.CommandTypeEnum.adCmdText);
                if (rsSQL.RecordCount > 0)
                {
                    rsSQL.MoveFirst();

                    oComp.Recordset.Update("ITFACE_CODE", rsSQL.Fields["ITFACE_CODE"].Value.ToString());
                    oComp.Recordset.Update("RT_CODE", rsSQL.Fields["RT_CODE"].Value.ToString());

                    if (InFNO == 10800)
                        oComp.Recordset.Update("CABLE_CODE", rsSQL.Fields["CABLE_CODE"].Value.ToString());
                }
                UpdateOutITFaceCode(OutFID, OutFNO, InFID, cFID, oComp, excABB);

                rsSQL = null;
                strSql = null;
            }
            catch (Exception ex) { log.WriteErr(ex); }
            finally { log.CloseFile(); }
        }

        private void UpdateOutITFaceCode(int OutFID, int OutFNO, int InFID, int cableFID, IGTComponent oComp, string excabb)
        {
            try
            {
                switch (OutFNO)
                {
                    case 13000: // DP 
                        UpdateDP_ITFaceCode(OutFID, InFID, cableFID, oComp, excabb); break;
                    case 13100: // PDP
                        UpdatePDP_ITFaceCode(OutFID, InFID, cableFID, oComp, excabb); break;
                    case 10800: // joint
                        UpdateJoint_ITFaceCode(OutFID, InFID, cableFID, oComp); break;
                }
            }
            catch (Exception ex) { log.WriteErr(ex); }
        }

        #region PDP - update 29-09-2012

        private void UpdatePDP_ITFaceCode(int pdpFID, int inFID, int cblFID, IGTComponent oComp, string excabb)
        {
            string itface = myUtil.rsField(oComp.Recordset, "ITFACE_CODE");
            string rtcode = myUtil.rsField(oComp.Recordset, "RT_CODE");
            string cblcode = myUtil.rsField(oComp.Recordset, "CABLE_CODE");

            string pdpcode = myUtil.GetFieldValue("GC_PDP", "PDP_CODE", pdpFID);
            if (pdpcode.Length == 0) pdpcode = GetPDPCode.NextCode(excabb, itface, rtcode);

            string route_dist = clsTrace.DSIDE_RouteDistance(inFID, cblFID);
            string cbllen = myUtil.rsField(oComp.Recordset, "TOTAL_LENGTH");

            route_dist = Convert.ToString(double.Parse(route_dist) + double.Parse(cbllen));

            
            string ssql = "UPDATE GC_PDP SET ITFACE_CODE = '" + itface + "', RT_CODE = '" + rtcode +
                "', PDP_CODE = '" + pdpcode + "', CABLE_CODE = '" + cblcode + 
                "', DIST_FROM_EXC = " + route_dist + " WHERE G3E_FID = " + pdpFID.ToString();

            ADODB.Recordset rsSQL = new ADODB.Recordset();

            int iR;
            GTClassFactory.Create<IGTApplication>().DataContext.Execute(ssql, out iR, (int)ADODB.CommandTypeEnum.adCmdText);
            GTClassFactory.Create<IGTApplication>().DataContext.Execute("COMMIT", out iR, (int)ADODB.CommandTypeEnum.adCmdText);
        }
        #endregion

        #region DP - update 28-09-2012

        private void UpdateDP_ITFaceCode(int dpFID, int inFID, int cblFID, IGTComponent oComp, string excabb)
        {
            string itface = myUtil.rsField(oComp.Recordset, "ITFACE_CODE");
            string rtcode = myUtil.rsField(oComp.Recordset, "RT_CODE");
            string cblcode = myUtil.rsField(oComp.Recordset, "CABLE_CODE");

            string curr_dpnum = myUtil.GetFieldValue("GC_DP", "DP_NUM", dpFID).Trim();
            string next_dpnum = GetNextDPNumber(excabb, itface, rtcode);

            string route_dist = clsTrace.DSIDE_RouteDistance(inFID,cblFID);
            string cbllen = myUtil.rsField(oComp.Recordset, "TOTAL_LENGTH");

            route_dist = Convert.ToString(double.Parse(route_dist) + double.Parse(cbllen));

            if (curr_dpnum.Length > 0)
            {
                #region test for duplicate DP number
                try
                {
                    string dpnum = curr_dpnum.PadLeft(7, '0');
                    string ssql_dp = "SELECT A.DP_NUM FROM GC_DP A, GC_NETELEM B " +
                        "WHERE LPAD(REGEXP_REPLACE (A.DP_NUM,'[A-Z]|[a-z]','') ,7,'0') = '{1}' " +
                        "AND A.G3E_FID <> {2} AND B.EXC_ABB = '{0}' AND A.G3E_FID = B.G3E_FID";

                    ADODB.Recordset rs1 = new ADODB.Recordset();
                    if (itface.Length > 0)
                        ssql_dp += " AND A.ITFACE_CODE = '" + itface + "'";
                    else
                        ssql_dp += " AND A.RT_CODE = '" + rtcode + "'";
                    ssql_dp = string.Format(ssql_dp, excabb, dpnum, dpFID);

                    rs1 = m_GTDataContext.OpenRecordset(ssql_dp, ADODB.CursorTypeEnum.adOpenDynamic, ADODB.LockTypeEnum.adLockOptimistic, (int)ADODB.CommandTypeEnum.adCmdText);

                    if (rs1.RecordCount == 0)
                        next_dpnum = curr_dpnum;
                    else
                        MessageBox.Show("DP number already use by others\r\nConnected DP will be assign with new number");
                }
                catch (Exception ex)
                {
                }
                #endregion
            }


            string ssql = "UPDATE GC_DP SET ITFACE_CODE = '" + itface + "', RT_CODE = '" + rtcode +
                "', CABLE_CODE = '" + cblcode + "', DP_NUM = '" + next_dpnum + "', DIST_FROM_EXC = " + route_dist +
                " WHERE G3E_FID = " + dpFID.ToString();

            ADODB.Recordset rsSQL = new ADODB.Recordset();

            int iR;
            GTClassFactory.Create<IGTApplication>().DataContext.Execute(ssql, out iR, (int)ADODB.CommandTypeEnum.adCmdText);
            GTClassFactory.Create<IGTApplication>().DataContext.Execute("COMMIT", out iR, (int)ADODB.CommandTypeEnum.adCmdText);
        }

        private string GetDPNumber(int dpFID)
        {
            try
            {
                string DP_NUM = myUtil.GetFieldValue("GC_DP", "DP_NUM", dpFID);
                return DP_NUM;
            }
            catch (Exception ex)
            {
                log.WriteErr(ex);
                return "";
            }
        }
        private string GetNextDPNumber(string excabb, string itface, string rtcode)
        {
            try
            {
                string ssql = "SELECT MAX (LPAD(REGEXP_REPLACE (A.DP_NUM,'[A-Z]|[a-z]','') ,7,'0'))  " +
                "FROM GC_DP A, GC_NETELEM B WHERE A.G3E_FID = B.G3E_FID AND A.{0} = '{1}' AND B.EXC_ABB = '{2}' ";

                if (rtcode.Length > 0)
                    ssql = string.Format(ssql, "RT_CODE", rtcode, excabb);
                else
                    ssql = string.Format(ssql, "ITFACE_CODE", itface, excabb);

                ADODB.Recordset rs = new ADODB.Recordset();
                rs = GTClassFactory.Create<IGTApplication>().DataContext.OpenRecordset(ssql,
                    ADODB.CursorTypeEnum.adOpenStatic, ADODB.LockTypeEnum.adLockOptimistic, 1);

                if (!rs.EOF)
                {
                    int val = Convert.ToInt32(rs.Fields[0].Value) + 1;
                    return val.ToString("00");
                }
                else
                {
                    return "01";
                }
            }
            catch (Exception ex)
            {
                log.WriteErr(ex);
                return "01";
            }
        }


        #endregion


        private void UpdateJoint_ITFaceCode(int jFID, int inFID, int cFID, IGTComponent cComp)
        {
            try
            {
                string ssql = "SELECT * FROM AG_CABLE_JOINT WHERE JOINT_FID = " + jFID.ToString();
                ADODB.Recordset rsSQL = new ADODB.Recordset();
                log.WriteLog("UpdateJoint : " + ssql);

                rsSQL = m_GTDataContext.OpenRecordset(ssql, ADODB.CursorTypeEnum.adOpenDynamic, ADODB.LockTypeEnum.adLockOptimistic, (int)ADODB.CommandTypeEnum.adCmdText);
                if (rsSQL.RecordCount == 0)
                {
                    ssql = "INSERT INTO AG_CABLE_JOINT (ID, ACTIVE_JOB, CABLE_FID, JOINT_FID, ITFACE_CODE, RT_CODE, CABLE_CODE, CTYPE, TOTAL_SIZE, GAUGE) " +
                    "VALUES (AG_CABLE_JOINT_SEQ.NEXTVAL,'" + m_GTDataContext.ActiveJob.ToString() + "'," + cFID.ToString() + "," + jFID.ToString() + ",'" +
                    myUtil.rsField(cComp.Recordset, "ITFACE_CODE") + "','" +
                    myUtil.rsField(cComp.Recordset, "RT_CODE") + "','" +
                    myUtil.rsField(cComp.Recordset, "CABLE_CODE") + "','" +
                    myUtil.rsField(cComp.Recordset, "CTYPE") + "'," +
                    myUtil.rsField(cComp.Recordset, "TOTAL_SIZE") + "," +
                    myUtil.rsField(cComp.Recordset, "GAUGE") + ")";

                    log.WriteLog("UpdateJoint : " + ssql);

                    int iR;
                    GTClassFactory.Create<IGTApplication>().DataContext.Execute(ssql, out iR, (int)ADODB.CommandTypeEnum.adCmdText);
                    GTClassFactory.Create<IGTApplication>().DataContext.Execute("COMMIT", out iR, (int)ADODB.CommandTypeEnum.adCmdText);

                    m_GTDataContext.OpenRecordset(ssql, ADODB.CursorTypeEnum.adOpenDynamic, ADODB.LockTypeEnum.adLockOptimistic, (int)ADODB.CommandTypeEnum.adCmdText);
                }
            }
            catch (Exception ex)
            {
                log.WriteErr(ex);
            }
        }


        #endregion
    }
}
