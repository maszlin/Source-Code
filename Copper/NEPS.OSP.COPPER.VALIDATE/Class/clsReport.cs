using System;
using System.Collections.Generic;
using System.Text;

namespace NEPS.OSP.COPPER.VALIDATE
{
    class clsReport
    {
        public struct TransferPoint
        {
            public string EXC_ABB;
            public int CAB_FID;
            public string CABLE_CODE;
        }
        public static TransferPoint recipient;
        public static TransferPoint donor;

        private struct Feature
        {
            public string featureName;
            public int featureCount;
            public List<int> featureFID;
        }
        private static Dictionary<string, Feature> otherfeature = new Dictionary<string, Feature>();
        private static Dictionary<string, Feature> termpoint = new Dictionary<string, Feature>();

        private static int transferFID = 0;
        private static string transferType = "";
        private static DateTime transferDate;
        private static int totalTP = 0;
        private static int totalFeature = 0;

        public static Logger log;

        #region Propeties
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
        public static void AddRecipient(string EXC_ABB, int CAB_FID, string CableCode)
        {
            recipient.EXC_ABB = EXC_ABB;
            recipient.CAB_FID = CAB_FID;
            recipient.CABLE_CODE = CableCode;
        }
        public static void AddDonor(string EXC_ABB, int CAB_FID, string CableCode)
        {
            donor.EXC_ABB = EXC_ABB;
            donor.CAB_FID = CAB_FID;
            donor.CABLE_CODE = CableCode;
        }
        public static void AddTransferFeature(int iFID, string transfertype, DateTime transferdate)
        {
            transferFID = iFID;
            transferType = transfertype;
            transferDate = transferdate;
        }
        public static void AddTermPoint(string name, int FID)
        {
            Feature tp = new Feature();
            totalTP++;
            totalFeature++;
            if (termpoint.ContainsKey(name))
            {
                tp = termpoint[name];
                tp.featureCount++;
                tp.featureFID.Add(FID);
                termpoint[name] = tp;
            }
            else
            {
                tp.featureName = name;
                tp.featureCount = 1;
                tp.featureFID = new List<int>();
                tp.featureFID.Add(FID);
                termpoint.Add(name, tp);
            }
        }
        public static void AddFeature(string name, int FID)
        {
            Feature tp = new Feature();
            totalFeature++;
            if (otherfeature.ContainsKey(name))
            {
                tp = otherfeature[name];
                tp.featureCount++;
                tp.featureFID.Add(FID);
                otherfeature[name] = tp;
            }
            else
            {
                tp.featureName = name;
                tp.featureCount = 1;
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
            log.WriteLog("\t" + "Exchange    : " + recipient.EXC_ABB);
            log.WriteLog("\t" + "Cabinet FID : " + recipient.CAB_FID.ToString());
            log.WriteLog("\t" + "Cable Code  : " + recipient.CABLE_CODE);
            log.WriteLog("");
            log.WriteLog("Donor Network:");
            log.WriteLog("\t" + "Exchange    : " + donor.EXC_ABB);
            log.WriteLog("\t" + "Cabinet FID : " + donor.CAB_FID.ToString());
            log.WriteLog("\t" + "Cable Code  : " + donor.CABLE_CODE);

            foreach (Feature f in termpoint.Values)
            {
                log.WriteLog("");
                log.WriteLog("Affected " + f.featureName + " : " + f.featureCount.ToString());
                for (int i = 0; i < f.featureFID.Count; i++)
                {
                    log.WriteLog("\tFID : " + f.featureFID[i].ToString());
                }
            }
            foreach (Feature f in otherfeature.Values)
            {
                log.WriteLog("");
                log.WriteLog("Affected " + f.featureName + " : " + f.featureCount.ToString());
                for (int i = 0; i < f.featureFID.Count; i++)
                {
                    log.WriteLog("\tFID : " + f.featureFID[i].ToString());
                }
            }
            string tempDir = System.Environment.GetEnvironmentVariable("TEMP");
            return tempDir + "\\" + filename;
        }
    }
}
