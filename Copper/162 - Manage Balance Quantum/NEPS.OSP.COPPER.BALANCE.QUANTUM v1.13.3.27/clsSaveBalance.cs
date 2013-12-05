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
    class clsSaveBalance
    {
        private static clsBalance Cable;
        public static void SaveBalance(TreeView t, clsBalance cable)
        {
                Cable = cable;
                foreach (TreeNode n in t.Nodes)
                    SaveBalance(n);
        }

        private static void SaveBalance(TreeNode nd)
        {            
            clsBalance.CablePairs c = Cable.cable(int.Parse(nd.Name));
            if (c.iFID != 0)
            {
                if (c.newEffectivePairs != c.iEffectivePairs)
                    SaveRecord(c.iFID, c.newEffectivePairs);
            }
            foreach (TreeNode n in nd.Nodes)
                SaveBalance(n);
        }

        private static void SaveRecord(int FID, int ePairs)
        {
            try
            {
                IGTKeyObject oFeature;
                oFeature = GTClassFactory.Create<IGTApplication>().DataContext.OpenFeature(7000, FID);
                ADODB.Recordset rs = oFeature.Components.GetComponent(7001).Recordset;
                rs.Update("EFFECTIVE_PAIRS", ePairs.ToString());
            }
            catch (Exception ex)
            {
                Debug.WriteLine("SaveRecord : FID - " + FID.ToString() + "\r\n" + ex.Message);
            }
        }

    }
}
