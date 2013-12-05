using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

using System.Drawing;
using Intergraph.GTechnology.Interfaces;
using Intergraph.GTechnology.API;
using ADODB;

namespace NEPS.OSP.COPPER.BALANCE.QUANTUM
{
    class clsAutoBalance
    {
        private clsBalance Cable;
        private bool flag = true;
        public bool AutoBalanceTree(TreeView t1, TreeView t2, clsBalance cable)
        {
            t2.Nodes.Clear();
            Cable = cable;
            foreach (TreeNode n1 in t1.Nodes)
            {
                TreeNode n2 = t2.Nodes.Add(n1.Name, n1.Text);
                int count = BalanceTree(n1, n2);
                AssignPair(n1, n2, count);
            }
            return flag;
        }

        private int BalanceTree(TreeNode oldnode, TreeNode newnode)
        {
            int total = 0;
            if (oldnode.Nodes.Count > 0)
                foreach (TreeNode n1 in oldnode.Nodes)
                {
                    TreeNode n2 = newnode.Nodes.Add(n1.Name, n1.Text);
                    if (n1.Text.IndexOf("ST") == -1)
                    {
                        int count = BalanceTree(n1, n2);
                        if (count == 0) count = GetEffectivePair(n1.Name);
                        AssignPair(n1, n2, count);
                        total += count;
                    }
                }
            return total;
        }

        private int GetEffectivePair(string FID)
        {
            System.Diagnostics.Debug.WriteLine(FID.ToString());
            string ssql = "SELECT EFFECTIVE_PAIRS FROM GC_CBL WHERE G3E_FID = " + FID;
            ADODB.Recordset rs = new ADODB.Recordset();
            rs = GTBalanceQuantum.m_gtapp.DataContext.OpenRecordset
                (ssql, ADODB.CursorTypeEnum.adOpenStatic, ADODB.LockTypeEnum.adLockOptimistic, 1);

            if (rs.EOF)
                return 0;
            else
                return myUtil.ParseInt(rs.Fields["EFFECTIVE_PAIRS"].Value.ToString());
        }

        private void AssignPair(TreeNode n1, TreeNode n2, int pair)
        {
            try
            {
                int FID = int.Parse(n1.Name);
                clsBalance.CablePairs c = Cable.cable(FID);

                if (pair == c.iEffectivePairs)
                {
                    if (c.iEffectivePairs > 0)
                        n2.Text = FID + " - " + c.iEffectivePairs;
                    else
                        n2.Text = FID.ToString();
                }
                else
                {
                    n2.Text = FID + " - " + c.iEffectivePairs +
                        " -> " + pair.ToString();
                    TreeNode n = n2.Parent;
                    while (n != null)
                    {
                        if (n.IsExpanded) break;
                        n.Expand();
                        n = n.Parent;
                    }
                    n2.Expand();

                    if (pair > c.iTotalPairs)
                    {
                        n2.BackColor = Color.Red;
                        n2.ForeColor = Color.White;
                        flag = false;
                    }
                    else
                        n2.ForeColor = Color.Red;

                    c.newEffectivePairs = pair;
                    Cable.cable(c);
                }
            }
            catch { }
        }
    }
}
