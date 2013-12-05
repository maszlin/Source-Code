/*
 * Balance Pair and Balance Quantum
 * - Balance Quantum : compare cable effective with immidiate downstream effective pairs
 * - Balance Pair
 * >>> Balance Quantum
 * >>> Check for redundant Pair Count
 * >>> Check for unassigned Pair Count
 * 
 * Notes on Pair Count 
 * - D-SIDE
 * > total pair count for each cable code is 200
 * > pair count annotation shows high and low pair use for each DP
 * > each pair must be terminated either to DP or STUB/STUMP
 * > cable count follow sequence of cable code >> D1 : 1-200, D2 : 201-400 .....
 * 
 * develop by : m.zam
 * started on : 01-08-2012
 * 
 *
 */

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.Drawing;

using Intergraph.GTechnology.Interfaces;
using Intergraph.GTechnology.API;

using ADODB;

namespace NEPS.OSP.COPPER.BALANCE.QUANTUM
{
    class clsBalance
    {
        short LINE_FNO = 7000;
        short LINE_GEO_CNO = 7010;
        short LINE_DET_CNO = 7011;

        short JOINT_FNO = 10800;
        short JOINT_CNO = 10801;
        short JOINT_GEO_CNO = 10820;
        short JOINT_DET_CNO = 10821;

        #region Properties
        string mEXC_ABB;
        string mITFACE_CODE;
        string mRT_CODE;
        string mCABLE_CODE;
        string mCABLE_CLASS;
        int mCABLE_FID;

        public string EXC_ABB
        {
            get { return mEXC_ABB; }
        }
        public string ITFACE_CODE
        {
            get { return mITFACE_CODE; }
        }
        public string RT_CODE
        {
            get { return mRT_CODE; }
        }
        public string CABLE_CODE
        {
            get { return mCABLE_CODE; }
        }
        public string CABLE_CLASS
        {
            get { return mCABLE_CLASS; }
        }
        public int CABLE_FID
        {
            get { return mCABLE_FID; }
        }
        #endregion

        #region Initialization
        public bool isCableSelected(IGTSelectedObjects selectedObj)
        {
            bool flag = false;
            if (selectedObj.FeatureCount == 1)
            {
                foreach (IGTDDCKeyObject oDDC in selectedObj.GetObjects())
                    if (oDDC.FNO == LINE_FNO)
                    {
                        GetExcAbb(oDDC.FID);
                        GetCableProperties(oDDC.FID);
                        mCABLE_FID = oDDC.FID;
                        flag = true;
                    }
            }
            return flag;
        }

        private void GetExcAbb(int iFID)
        {
            mEXC_ABB = GetValue("SELECT EXC_ABB FROM GC_NETELEM WHERE G3E_FID = " + iFID, "EXC_ABB");
        }

        private void GetCableProperties(int iFID)
        {
            Dictionary<string, string> props = new Dictionary<string, string>();
            props = GetValues("SELECT * FROM GC_CBL WHERE G3E_FID = " + iFID,
                new string[] { "CABLE_CODE", "CABLE_CLASS", "ITFACE_CODE", "RT_CODE" });

            mCABLE_CODE = props["CABLE_CODE"];
            mCABLE_CLASS = props["CABLE_CLASS"];
            mITFACE_CODE = props["ITFACE_CODE"];
            mRT_CODE = props["RT_CODE"];
        }
        #endregion

        #region Declaration
        public struct CablePairs
        {
            public int iFID;
            public int iEffectivePairs;
            public int newEffectivePairs;
            public int childEffectivePairs;
            public int iTotalPairs;
            public int parentFID;
            public int hi;
            public int lo;

            public CablePairs(int iFID, int effectivepair, int totalpair, int parentFID)
            {
                this.iFID = iFID;
                this.parentFID = parentFID;
                this.iEffectivePairs = effectivepair;
                this.iTotalPairs = totalpair;
                newEffectivePairs = effectivepair;
                childEffectivePairs = 0;
                hi = 0;
                lo = 0;
            }
            public override string ToString()
            {
                string txt = iFID.ToString() + " - " + iEffectivePairs.ToString();
                if (iEffectivePairs != childEffectivePairs)
                    txt += " (" + childEffectivePairs.ToString() + ")";
                return txt;
            }
            public bool isConflict
            {
                get { return (newEffectivePairs > iTotalPairs); }
            }
            public bool isBalance
            {
                get { return (iEffectivePairs == childEffectivePairs); }
            }
        }
        private Dictionary<int, CablePairs> cables = new Dictionary<int, CablePairs>();
        public CablePairs cable(int FID)
        {
            if (cables.ContainsKey(FID))
                return cables[FID];
            else
                return new CablePairs();
        }
        public void cable(CablePairs c)
        {
            if (cables.ContainsKey(c.iFID))
                cables[c.iFID] = c;
        }

        public struct TerminationPoint
        {
            public int termFID;
            public int termFNO;
            public int parentFID;
            public string termName;
            public TerminationPoint(int iFID, int iFNO, int parentFID)
            {
                termFID = iFID;
                termFNO = iFNO;
                this.parentFID = parentFID;
                termName = "";
                termName = terminationName(iFNO);

            }
            private string terminationName(int FNO)
            {
                ADODB.Recordset rs = new ADODB.Recordset();
                rs = GTBalanceQuantum.m_gtapp.DataContext.OpenRecordset
                    ("SELECT G3E_USERNAME FROM G3E_FEATURE WHERE G3E_FNO = " + FNO,
                    ADODB.CursorTypeEnum.adOpenStatic, ADODB.LockTypeEnum.adLockOptimistic, 1);

                return (myUtil.rsField(rs, "G3E_USERNAME"));
            }
            public override string ToString()
            {
                string txt = termFID.ToString() + " [" + termFNO.ToString() + "] " + termName;
                return txt;
            }
        }
        public Dictionary<int, TerminationPoint> termpoints = new Dictionary<int, TerminationPoint>();
        #endregion

        #region E-SIDE Balance Quantum and Balance Pair
        public void BalanceQuantum_ESIDE()
        {
            // tracedown from cab to end
            int[] mainjoint = clsTrace.TraceUp_EGetStartPoint(mCABLE_FID); // inFNO, inFID, cblFID
            if (mainjoint == null)
            {
                throw new System.Exception("Unable to trace main joint for selected E-SIDE cable");
            }
            //trace down cable + get pair information
            int maxpair = LoopNRConnect(mainjoint[2]);

        }
        #endregion

        #region D-SIDE Balance Quantum and Balance Pair

        clsDSIDE DSIDE = new clsDSIDE();

        public void BalanceQuantum_DSIDE()
        {
            // tracedown from cab to end
            int[] startcable = clsTrace.TraceUp_DGetStartPoint(mCABLE_FID); // inFNO, inFID, cblFID
            if (startcable == null)
            {
                throw new System.Exception("Unable to trace termination point for selected cable");
            }
            //trace down cable + get pair information
            int maxpair = LoopNRConnect(startcable[2]);
        }

        #endregion

        #region Trace Downstream
        private int LoopNRConnect(int startFID) // inFNO, inFID, cblFID
        {
            int cFID = -1;
            int oFID = -1;
            int oFNO = -1;
            int maxpair = 0;
            int effpair = 0;
            ADODB.Recordset rs = new ADODB.Recordset();

            rs = GTBalanceQuantum.m_gtapp.DataContext.OpenRecordset
                    ("SELECT A.* FROM GC_NR_CONNECT A WHERE A.G3E_FID = " + startFID,
                     ADODB.CursorTypeEnum.adOpenStatic, ADODB.LockTypeEnum.adLockOptimistic, 1);

            cables.Clear();
            termpoints.Clear();

            while (!rs.EOF)
            {
                cFID = int.Parse(myUtil.rsField(rs, "G3E_FID"));
                oFID = int.Parse(myUtil.rsField(rs, "OUT_FID"));
                oFNO = int.Parse(myUtil.rsField(rs, "OUT_FNO"));
                Debug.WriteLine("oFID : " + oFID.ToString() + " - oFNO : " + oFNO.ToString());
                // read total size + effective pair
                Dictionary<string, string> pairs = GetPairSize(cFID);
                if (oFNO != JOINT_FNO && !termpoints.ContainsKey(oFID))
                    termpoints.Add(cFID, new TerminationPoint(oFID, oFNO, cFID));

                if (!cables.ContainsKey(cFID))
                {
                    effpair = myUtil.ParseInt(pairs["EFFECTIVE_PAIRS"]);
                    cables.Add(cFID, new CablePairs(cFID, effpair, myUtil.ParseInt(pairs["TOTAL_SIZE"]), -1));
                    maxpair += effpair;
                    // loop to down stream
                    LoopNRConnect(cFID, pairs, oFID);
                }

                rs.MoveNext();
            }
            rs.Close();
            rs = null;

            return maxpair;
        }

        /// <summary>
        /// This is a recursive loop which tracedown from start till end
        /// While tracing it collects the effective pair count of the cable and its downstream
        /// </summary>
        /// <param name="iFID">current cable FID</param>
        /// <param name="iEffectivePairs">current cable effective count</param>
        /// <param name="jointFID">outer joint of current cable</param>
        private void LoopNRConnect(int iFID, Dictionary<string, string> pairCount, int jointFID)
        {
            int cFID = -1;
            int oFID = -1;
            int oFNO = -1;

            ADODB.Recordset rs = new ADODB.Recordset();
            rs = GTBalanceQuantum.m_gtapp.DataContext.OpenRecordset
                    ("SELECT * FROM GC_NR_CONNECT WHERE IN_FID = " + jointFID +
                    " AND IN_FNO IN (7000, 10800)",
                     ADODB.CursorTypeEnum.adOpenStatic, ADODB.LockTypeEnum.adLockOptimistic, 1);

            if (rs.EOF)
            {
                CablePairs c = cables[iFID];
                c.childEffectivePairs = myUtil.ParseInt(pairCount["EFFECTIVE_PAIRS"]);
                cables[iFID] = c;
            }
            else
            {
                while (!rs.EOF)
                {
                    cFID = int.Parse(myUtil.rsField(rs, "G3E_FID"));
                    oFID = int.Parse(myUtil.rsField(rs, "OUT_FID"));
                    oFNO = int.Parse(myUtil.rsField(rs, "OUT_FNO"));

                    if ((cFID == iFID) || (jointFID == oFID)) break;

                    Dictionary<string, string> pairs = GetPairSize(cFID); // read total + effective pairs

                    GTBalanceQuantum.m_CustomForm.IncreaseProgressBar(1);
                    if (oFNO != JOINT_FNO & !termpoints.ContainsKey(cFID))
                        termpoints.Add(cFID, new TerminationPoint(oFID, oFNO, cFID));

                    if (!cables.ContainsKey(cFID))
                    {
                        CablePairs c = cables[iFID];
                        c.childEffectivePairs += myUtil.ParseInt(pairs["EFFECTIVE_PAIRS"]);
                        cables[iFID] = c;
                        cables.Add(cFID, new CablePairs(cFID,
                            myUtil.ParseInt(pairs["EFFECTIVE_PAIRS"]), myUtil.ParseInt(pairs["TOTAL_SIZE"]), iFID));
                    }
                    LoopNRConnect(cFID, pairs, oFID);
                    rs.MoveNext();
                }
            }
            rs.Close();
            rs = null;
        }


        private int GetEffectivePairs(int iFID)
        {
            string paircount =
                GetValue("SELECT EFFECTIVE_PAIRS FROM GC_CBL WHERE G3E_FID = '" + iFID.ToString() + "'", "EFFECTIVE_PAIRS");

            return int.Parse(paircount);
        }

        private Dictionary<string, string> GetPairSize(int iFID)
        {
            return GetValues("SELECT EFFECTIVE_PAIRS, TOTAL_SIZE FROM GC_CBL WHERE G3E_FID = '" + iFID.ToString() + "'",
                new string[] { "EFFECTIVE_PAIRS", "TOTAL_SIZE" });
        }
        #endregion

        #region GetValue
        private string GetValue(string ssql, string keyname)
        {
            ADODB.Recordset rs = new ADODB.Recordset();
            rs = GTBalanceQuantum.m_gtapp.DataContext.OpenRecordset
                (ssql, ADODB.CursorTypeEnum.adOpenStatic, ADODB.LockTypeEnum.adLockOptimistic, 1);
            return (myUtil.rsField(rs, keyname));
        }

        private Dictionary<string, string> GetValues(string ssql, string[] keyname)
        {
            ADODB.Recordset rs = new ADODB.Recordset();
            rs = GTBalanceQuantum.m_gtapp.DataContext.OpenRecordset
                (ssql, ADODB.CursorTypeEnum.adOpenStatic, ADODB.LockTypeEnum.adLockOptimistic, 1);

            if (rs.EOF) return null;

            Dictionary<string, string> val = new Dictionary<string, string>();
            for (int i = 0; i < keyname.Length; i++)
            {
                val.Add(keyname[i], myUtil.rsField(rs, keyname[i]));
            }
            return val;
        }
        #endregion

        #region Network Tree 2013-02-01
        public bool EffectiveTree(TreeView trv)
        {
            bool balance = true;
            TreeNode nd = new TreeNode();
            trv.Nodes.Clear();
            if (cables.Count > 0)
            {
                foreach (CablePairs c in cables.Values)
                {
                    if (c.parentFID == -1)
                        nd = trv.Nodes.Add(c.iFID.ToString(), c.ToString());
                    else if (nd.Name == c.parentFID.ToString())
                        nd = nd.Nodes.Add(c.iFID.ToString(), c.ToString());
                    else
                    {
                        if (!termpoints.ContainsKey(myUtil.ParseInt(nd.Name)))
                        {
                            nd.Nodes.Add(c.iFID.ToString(), "STUB/STUMP");
                        }

                        while (nd.Parent != null)
                        {
                            if (nd.Parent.Name == c.parentFID.ToString())
                            {
                                nd = nd.Parent.Nodes.Add(c.iFID.ToString(), c.ToString());
                                break;
                            }
                            else
                                nd = nd.Parent;
                        }
                        if (nd.Parent == null)
                            nd = trv.Nodes.Add(c.iFID.ToString(), c.ToString());
                    }

                    int keyFID = myUtil.ParseInt(nd.Name);
                    if (termpoints.ContainsKey(keyFID))
                    {
                        TerminationPoint t = termpoints[keyFID];
                        nd.Nodes.Add(t.termFID.ToString(), t.ToString());
                    }

                    if (c.childEffectivePairs != c.iEffectivePairs)
                    {
                        nd.ForeColor = Color.Red;
                        balance = false;
                        TreeNode n = nd.Parent;
                        while (n != null)
                        {
                            if (n.IsExpanded) break;
                            n.Expand();
                            n = n.Parent;
                        }

                    }
                }
            }
            return balance;
        }



        #endregion
    }
}
