using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Runtime.InteropServices;
using Intergraph.GTechnology.Interfaces;
using Intergraph.GTechnology.API;
using ADODB;

namespace NEPS.OSP.COPPER.VALIDATE
{
    class CableValidation
    {
        public string strMsg = string.Empty;
        public string strLoc = string.Empty;
        //  string mExcAbb = "";

        #region Test Plant Unit

        private bool TestPlantUnit(IGTKeyObject objFeat, string prevPU)
        {
            // Check Attribute
            IGTComponent o_Comp = objFeat.Components.GetComponent(51); // GC_NETELEM

            o_Comp.Recordset.MoveFirst();
            string min_material = o_Comp.Recordset.Fields["MIN_MATERIAL"].Value.ToString();
            if (min_material.Length == 0 || min_material == "-")
            {
                if (prevPU.Length == 0 || prevPU == "-")
                {
                    strMsg = "Plant unit cannot be empty";
                    strLoc = "GC_CBL where G3E_FID:" + o_Comp.Recordset.Fields["G3E_FID"].Value.ToString();
                    return false;
                }
                else
                {
                    o_Comp.Recordset.Update("MIN_MATERIAL", prevPU);
                    min_material = prevPU;
                }
            }

            string[] PU = min_material.Split('|');
            o_Comp = objFeat.Components.GetComponent(7001); // GC_CBL
            o_Comp.Recordset.MoveFirst();
            o_Comp.Recordset.Update("CTYPE", PU[0]);
            o_Comp.Recordset.Update("TOTAL_SIZE", int.Parse(PU[1]));
            if (myUtil.ParseInt(myUtil.rsField(o_Comp.Recordset, "EFFECTIVE_PAIRS")) == 0)
                o_Comp.Recordset.Update("EFFECTIVE_PAIRS", int.Parse(PU[1]));
            o_Comp.Recordset.Update("GAUGE", double.Parse(PU[2]));
            o_Comp.Recordset.Update("PLACEMENT", PU[3]);

            return true;
        }

        #endregion

        #region Get Parent Properties
        private bool GetParentProperties(IGTKeyObject objFeat, int inFNO, int inFID)
        {
            try
            {
                IGTComponent oComp = objFeat.Components.GetComponent(7001);
                //string cblCode = "";
                //string rtCode = "";
                //string itface = "";
                string cblPU = "";
                string ssql;
                ADODB.Recordset rsSQL = new ADODB.Recordset();

                switch (inFNO)
                {
                    case 10300:
                        ssql = "SELECT ITFACE_CODE, '' RT_CODE FROM GC_ITFACE WHERE G3E_FID = " + inFID;
                        break;
                    case 9100:
                        ssql = "SELECT '' ITFACE_CODE, RT_CODE FROM GC_MSAN WHERE G3E_FID = " + inFID;
                        break;
                    case 9600:
                        ssql = "SELECT '' ITFACE_CODE, RT_CODE FROM GC_RT WHERE G3E_FID = " + inFID;
                        break;
                    default:
                        ssql = "SELECT A.ITFACE_CODE, A.RT_CODE, A.CABLE_CODE, B.MIN_MATERIAL FROM GC_CBL A, GC_NETELEM B ";
                        ssql += "WHERE A.G3E_FID = B.G3E_FID AND A.G3E_FID IN ";
                        ssql += "(SELECT AA.G3E_FID FROM GC_NR_CONNECT AA, GC_SPLICE BB WHERE ";
                        ssql += " AA.OUT_FID = BB.G3E_FID AND BB.G3E_FID = " + inFID.ToString() + ")";
                        break;
                }

                rsSQL = GTClassFactory.Create<IGTApplication>().DataContext.OpenRecordset
                    (ssql, ADODB.CursorTypeEnum.adOpenDynamic, ADODB.LockTypeEnum.adLockOptimistic, (int)ADODB.CommandTypeEnum.adCmdText);
                if (rsSQL.RecordCount > 0)
                {
                    rsSQL.MoveFirst();
                    oComp.Recordset.Update("ITFACE_CODE", rsSQL.Fields["ITFACE_CODE"].Value.ToString());
                    oComp.Recordset.Update("RT_CODE", rsSQL.Fields["RT_CODE"].Value.ToString());

                    if (inFNO == 10800) // JOINT 
                    {
                        oComp.Recordset.Update("CABLE_CODE", rsSQL.Fields["CABLE_CODE"].Value.ToString());
                        cblPU = rsSQL.Fields["MIN_MATERIAL"].Value.ToString();
                    }
                    else if (oComp.Recordset.Fields["CABLE_CODE"].Value.ToString().Trim().Length == 0)
                    {
                        string cable_class = oComp.Recordset.Fields["CABLE_CLASS"].Value.ToString().Trim();

                        if (cable_class.IndexOf("D-") > -1)
                        {
                           oComp.Recordset.Update("CABLE_CODE", NewDCableCode(objFeat.FID, inFID));
                        }
                        else
                        {
                            /// *********** eside cable code ????
                        }
                    }
                }

                TestPlantUnit(objFeat, cblPU);
                return true;
            }
            catch (Exception ex)
            {
                strMsg = "Cable Code error : " + ex.Message;
                strLoc = "GC_CBL where G3E_FID:" + objFeat.FID.ToString();
                return false;
            }
        }

        private string NewDCableCode(int cblFID, int cabFID)
        {
            string ssql = "SELECT GC_OSP_COP_VAL.CNO_7000_GET_DSIDE_CBLCODE(" + cblFID + "," + cabFID + ") AS DCODE FROM DUAL";

            ADODB.Recordset rsSQL = new ADODB.Recordset();
            rsSQL = GTClassFactory.Create<IGTApplication>().DataContext.OpenRecordset
                (ssql, ADODB.CursorTypeEnum.adOpenDynamic, ADODB.LockTypeEnum.adLockOptimistic, (int)ADODB.CommandTypeEnum.adCmdText);
            if (rsSQL.RecordCount > 0)
            {
                rsSQL.MoveFirst();
                return rsSQL.Fields[0].Value.ToString();
            }
            else
                return "";
        }
        #endregion

        public bool ValidateCableCode(IGTKeyObject objFeat, string excabb)
        {
            IGTComponent Comp = objFeat.Components.GetComponent(53);

            Comp.Recordset.MoveFirst();
            int cblFID = myUtil.ParseInt(myUtil.rsField(Comp.Recordset, "G3E_FID"));
            int inFID = myUtil.ParseInt(myUtil.rsField(Comp.Recordset, "IN_FID"));
            int inFNO = myUtil.ParseInt(myUtil.rsField(Comp.Recordset, "IN_FNO"));

            if (!GetParentProperties(objFeat, inFNO, inFID))
                if (!GetParentProperties(objFeat, inFNO, inFID)) return false;

            Comp = objFeat.Components.GetComponent(7001); //gc_cbl

            string cblcode = Comp.Recordset.Fields["CABLE_CODE"].Value.ToString();
            string cblusage = Comp.Recordset.Fields["CUSAGE"].Value.ToString();

            if (Comp.Recordset.Fields["CABLE_CLASS"].Value.ToString() == "D-CABLE")
            {
                #region Validation D-CABLE

                int total_pair = 0;
                string ssql = "";

                if (inFNO == 10300 || inFNO == 9600 || inFNO == 9100)
                {
                    ssql = "SELECT G3E_FID FROM GC_NR_CONNECT WHERE IN_FID = " + inFID.ToString();
                    ssql = "SELECT * FROM GC_CBL WHERE G3E_FID IN (" + ssql + ")";
                    ssql += " AND G3E_FID <> " + objFeat.FID.ToString();
                    ssql += " AND CABLE_CODE = '" + cblcode + "'";

                    Recordset rsChk = new RecordsetClass();

                    rsChk = GTClassFactory.Create<IGTApplication>().DataContext.OpenRecordset
                        (ssql, CursorTypeEnum.adOpenDynamic, LockTypeEnum.adLockOptimistic, 1, new object[0]);

                    if (rsChk.RecordCount > 0)
                    {
                        rsChk.MoveFirst();
                        while (!rsChk.EOF)
                        {
                            int epair = myUtil.ParseInt(myUtil.rsField(rsChk, "EFFECTIVE_PAIRS"));
                            int tpair = myUtil.ParseInt(myUtil.rsField(rsChk, "TOTAL_SIZE"));
                            total_pair += (epair == 0 ? tpair : epair);
                            rsChk.MoveNext();
                        }
                    }
                }
                else if (inFNO == 13100) // PDP
                {
                    string pdpCblCode = myUtil.GetFieldValue("GC_PDP", "CABLE_CODE", inFID);
                   if (cblcode != pdpCblCode)
                       Comp.Recordset.Update("CABLE_CODE", pdpCblCode);
                }

                int efct_pair = myUtil.ParseInt(myUtil.rsField(Comp.Recordset, "EFFECTIVE_PAIRS"));
                int new_tpair = myUtil.ParseInt(myUtil.rsField(Comp.Recordset, "TOTAL_SIZE"));
                if (efct_pair == 0)
                {
                    efct_pair = new_tpair;
                    if ((inFNO == 10300) && (efct_pair > 200 - total_pair))
                        efct_pair = 200 - total_pair;
                    Comp.Recordset.Update("EFFECTIVE_PAIRS", efct_pair);
                }
                else if (efct_pair > new_tpair)
                {
                    strLoc = "GC_CBL where G3E_FID:" + objFeat.FID.ToString();
                    strMsg = "Effective pairs for " + cblcode + " exceed total size";
                    return false;
                }
                else if (efct_pair > 200)
                {
                    strLoc = "GC_CBL where G3E_FID:" + objFeat.FID.ToString();
                    strMsg = "Effective pairs for " + cblcode + " exceed 200 pair";
                    return false;
                }
                else if (efct_pair + total_pair > 200)
                {
                    strLoc = "GC_CBL where G3E_FID:" + objFeat.FID.ToString();
                    strMsg = "Total effective pairs for cable code " + cblcode + " exceed 200 pair";
                    return false;
                }
                else if (cblusage.Length == 0)
                {
                    Comp.Recordset.Update("CUSAGE", "DISTRIBUTION");
                }

                #endregion
                return true;
            }
            else if (cblusage.Length == 0) // E-SIDE cable
            {

                Comp.Recordset.Update("CUSAGE", "MAIN");
            }
            return true;
        }
    }
}
