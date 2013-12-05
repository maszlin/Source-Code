using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;

namespace NEPS.OSP.COPPER.BALANCE.QUANTUM
{
    class clsReport
    {
        Logger log;
        #region Propeties
        string mEXC_ABB;
        string mITFACE_CODE;
        string mCABLE_CODE;
        string mCABLE_CLASS;

        public clsReport(string exc_abb, string itface_code, string cable_code, string cable_class)
        {
            mEXC_ABB = exc_abb;
            mITFACE_CODE = itface_code;
            mCABLE_CLASS = cable_class;
            mCABLE_CODE = cable_code;
        }
        #endregion

        public void CreateReport(ListView lsv, bool isQuantum)
        {
            string filename = "BAL_QUANTUM_REPORT [" + DateTime.Now.ToString("yyMMdd HHmmss") + "].txt";

            log = Logger.getInstance();
            log.OpenFile(filename);

            log.WriteLog("");
            log.WriteLog("*****************************************");
            if (isQuantum)
                log.WriteLog("*       REPORT ON BALANCE QUANTUM       *");
            else
                log.WriteLog("*        REPORT ON BALANCE PAIR         *");

            log.WriteLog("*****************************************");
            log.WriteLog("");
            log.WriteLog("EXCHANGE : " + mEXC_ABB);
            if (mCABLE_CLASS.IndexOf("D") > -1)
            {
                log.WriteLog("CABINET  : " + mITFACE_CODE);
            }
            log.WriteLog("CABLE CODE  : " + mCABLE_CODE);
            log.WriteLog("CABLE CLASS : " + mCABLE_CLASS);
            log.WriteLog("");
            log.WriteLog("****************************************");
            log.WriteLog("");
            string rpt = "";
            rpt = "NO\t";
            rpt += "CABLE FID\t";
            rpt += "EFFECTIVE\t";
            rpt += (isQuantum ? "DOWNSTREAM\t" : "TOTAL\t");
            log.WriteLog(rpt);
            if (isQuantum)
                log.WriteLog("\t\t\t\tPAIRS\t\tPAIRS");
            else
                log.WriteLog("\t\t\t\tPAIRS\t\tSIZE");
            log.WriteLog("");
            log.WriteLog("****************************************");
            log.WriteLog("");
            foreach (ListViewItem l in lsv.Items)
            {
                rpt = l.SubItems[0].Text + "\t";
                rpt += l.SubItems[1].Text + "\t";
                rpt += l.SubItems[2].Text + "\t\t\t";
                rpt += l.SubItems[3].Text + "\t";
                log.WriteLog(rpt);
            }

            string tempDir = System.Environment.GetEnvironmentVariable("TEMP");
            Process.Start(tempDir + "\\" + filename);
        }
    }
}
