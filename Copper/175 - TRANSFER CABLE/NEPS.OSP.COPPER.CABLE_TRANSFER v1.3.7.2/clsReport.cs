using System;
using System.Collections.Generic;
using System.Text;
using Intergraph.GTechnology.Interfaces;
using Intergraph.GTechnology.API;
using ADODB;

namespace NEPS.OSP.COPPER.CABLE_TRANSFER
{
    class clsReport
    {
        public struct TransferPoint
        {
            public string EXC_ABB;
            public int CAB_FID;
            public string CAB_CODE;
            public string CAB_TYPE;
            public string CABLE_CODE;
            public int m_CABLE_SIZE;
            public int m_EFCT_PAIRS;
        }
        public static TransferPoint rcpnt;
        public static TransferPoint donor;

        private struct Feature
        {
            public string featureName;
            public int featureCount;
            public List<int> featureFID;
            public List<string> featureCode;
        }
        private static Dictionary<string, Feature> otherfeature = new Dictionary<string, Feature>();
        private static Dictionary<string, Feature> termpoint = new Dictionary<string, Feature>();

        private static int transferFID = 0;
        private static string transferType = "";
        private static DateTime transferDate;
        private static int totalTP = 0;
        private static int totalFeature = 0;

        public static Logger log;

        #region Properties
        public static void InitReport(DateTime transferdate, string transfertype)
        {
            otherfeature.Clear();
            termpoint.Clear();
            totalTP = 0;
            totalFeature = 0;
            transferDate = transferdate;
            transferType = transfertype;
        }
        public static int TransferFID
        {
            get { return transferFID; }
            set { transferFID = value; }
        }
        #endregion

        #region Update Transfer Values
        public static void AddTransferFeature(int iFID, string transfertype, DateTime transferdate)
        {
            transferFID = iFID;
            transferType = transfertype;
            transferDate = transferdate;
        }
        private static string GetTermCode(int FID, short FNO)
        {
            string ssql = "SELECT {1} FROM {0} WHERE G3E_FID = " + FID;
            switch (FNO)
            {
                case 10300: ssql = string.Format(ssql, "GC_ITFACE", "ITFACE_CODE"); break;
                case 6200: ssql = string.Format(ssql, "GC_PDDP", "PDDP_CODE"); break;
                case 6300: ssql = string.Format(ssql, "GC_DDP", "DDP_NUM"); break;
                case 13000: ssql = string.Format(ssql, "GC_DP", "DP_NUM"); break;
                case 13100: ssql = string.Format(ssql, "GC_PDP", "PDP_CODE"); break;
                case 13200: ssql = string.Format(ssql, "GC_IDF", "IDF_CODE"); break;
                default: return "";
            }

            ADODB.Recordset rs = new ADODB.Recordset();
            rs = GTTransfer.m_gtapp.DataContext.OpenRecordset(ssql,
            ADODB.CursorTypeEnum.adOpenStatic, ADODB.LockTypeEnum.adLockOptimistic, 1);

            if (!rs.EOF)
            {
                rs.MoveFirst();
                return rs.Fields[0].Name.ToString() + " : " + rs.Fields[0].Value.ToString();

            }
            else
            {
                return FID.ToString();
            }
        }

        public static void AddTermPoint(string name, int FID, short FNO)
        {
            string CODE = GetTermCode(FID, FNO);
            Feature tp = new Feature();
            totalTP++;
            totalFeature++;
            if (termpoint.ContainsKey(name))
            {
                tp = termpoint[name];
                if (!tp.featureFID.Contains(FID))
                {
                    tp.featureCount++;
                    tp.featureFID.Add(FID);
                    tp.featureCode.Add(CODE);
                    termpoint[name] = tp;
                }
                else
                {
                    totalTP--;
                    totalFeature--;
                }
            }
            else
            {
                tp.featureName = name;
                tp.featureCount = 1;
                tp.featureFID = new List<int>();
                tp.featureCode = new List<string>();
                tp.featureFID.Add(FID);
                tp.featureCode.Add(CODE);
                termpoint.Add(name, tp);
            }
        }
        public static void AddFeature(string name, int FID)
        {
            Feature tp = new Feature();
            string code = "";
            if (name == "CABLE")
            {
                string ssql = "SELECT TEXT_VALUE, ITFACE_CODE, CABLE_CODE, COUNT_TRANSFER FROM GC_CBL WHERE G3E_FID = " + FID.ToString();

                ADODB.Recordset rs = new ADODB.Recordset();
                rs = GTTransfer.m_gtapp.DataContext.OpenRecordset(ssql,
                ADODB.CursorTypeEnum.adOpenStatic, ADODB.LockTypeEnum.adLockOptimistic, 1);

                if (!rs.EOF)
                {
                    code = rs.Fields["TEXT_VALUE"].Value.ToString();

                }
            }
            totalFeature++;
            if (otherfeature.ContainsKey(name))
            {
                tp = otherfeature[name];
                if (!tp.featureFID.Contains(FID))
                {
                    tp.featureCount++;
                    tp.featureFID.Add(FID);
                    tp.featureCode.Add(code);
                    otherfeature[name] = tp;
                }
                else
                    totalFeature--;
            }
            else
            {
                tp.featureName = name;
                tp.featureCount = 1;
                tp.featureCode = new List<string>();
                tp.featureCode.Add(code);
                tp.featureFID = new List<int>();
                tp.featureFID.Add(FID);
                otherfeature.Add(name, tp);
            }
        }
        #endregion

        public static string CreateReport()
        {
            string filename = transferType + " [" + DateTime.Now.ToString("yyMMdd HHmm") + "].txt";
            log = Logger.getInstance();
            log.OpenFile(filename);
            log.WriteLog("********************************************");
            log.WriteLog("\t\t" + transferType); // transfer e-side or d-side
            log.WriteLog("********************************************");
            log.WriteLog("");
            log.WriteLog("Transfer Identification     : " + transferFID.ToString());
            log.WriteLog("Number of TP affected       : " + totalTP.ToString());
            log.WriteLog("Number of features affected : " + (totalFeature).ToString());
            log.WriteLog("Proposed Transfer Date      : " + transferDate.ToString("dd MMM yyyy"));
            log.WriteLog("");
            log.WriteLog("Recipient Network:");
            log.WriteLog("\t" + "Exchange : " + rcpnt.EXC_ABB);
            log.WriteLog("\t" + rcpnt.CAB_TYPE + " : " + rcpnt.CAB_CODE);
            log.WriteLog("\t" + "Cable    : " + rcpnt.CABLE_CODE);
            log.WriteLog("");
            log.WriteLog("Donor Network:");
            log.WriteLog("\t" + "Exchange : " + donor.EXC_ABB);
            log.WriteLog("\t" + donor.CAB_TYPE + " : " + donor.CAB_CODE);
            log.WriteLog("\t" + "Cable Code : " + donor.CABLE_CODE);

            foreach (Feature f in termpoint.Values)
            {
                log.WriteLog("");
                log.WriteLog("Affected " + f.featureName + " : " + f.featureCount.ToString());
                for (int i = 0; i < f.featureFID.Count; i++)
                {
                    log.WriteLog("\t" + f.featureCode[i].ToString() + "\t\t" + f.featureFID[i].ToString());
                }
            }
            foreach (Feature f in otherfeature.Values) // cables and joints
            {
                log.WriteLog("");
                log.WriteLog("Affected " + f.featureName + " : " + f.featureCount.ToString());

                for (int i = 0; i < f.featureFID.Count; i++)
                {
                    //                    log.WriteLog("\t" + f.featureCode[i].ToString() + "(" + f.featureFID[i].ToString() + ")");
                    log.WriteLog("\t" + f.featureFID[i].ToString());
                }
            }
            string tempDir = System.Environment.GetEnvironmentVariable("TEMP");
            return tempDir + "\\" + filename;
        }
    }
}
