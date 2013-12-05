using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Diagnostics;
using Intergraph.GTechnology.Interfaces;
using Intergraph.GTechnology.API;
using ADODB;

namespace NEPS.OSP.COPPER.COLLECT.COUNT
{
    class clsSaveCount
    {
        internal static void DeletePairCount(clsCableCount cbl)
        {
            try
            {
                int iR;
                string ssql = "DELETE FROM GC_COUNT WHERE G3E_FID = " + cbl.FID.ToString();
                ADODB.Recordset rs = GTClassFactory.Create<IGTApplication>().DataContext.Execute(ssql,
                                       out iR, (int)ADODB.CommandTypeEnum.adCmdText);

                ssql = "UPDATE GC_CBL SET COUNT_ANNOTATION = '' WHERE G3E_FID = " + cbl.FID.ToString();
                GTClassFactory.Create<IGTApplication>().DataContext.Execute(ssql,
                                        out iR, (int)ADODB.CommandTypeEnum.adCmdText);

                GTClassFactory.Create<IGTApplication>().DataContext.Execute("COMMIT",
                                        out iR, (int)ADODB.CommandTypeEnum.adCmdText);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error DeleteCount: FID - " + cbl.FID.ToString() + "\r\n" + ex.Message);
            }
        }

        internal static void SavePairCountDSide(clsDSIDE cbl, string remark)
        {
            if (cbl.CID > 0)
            {
                string anno = "";
                int effective = 0;
                ADODB.Recordset rs;
                IGTKeyObject oFeature = null;

                for (int cid = 0; cid < cbl.CID; cid++)
                {
                    oFeature = GTClassFactory.Create<IGTApplication>().DataContext.OpenFeature(7000, cbl.FID);
                    rs = oFeature.Components.GetComponent(55).Recordset; // 55 is CNO for GC_COUNT

                    if (rs.EOF)
                    {
                        rs.AddNew("G3E_FID", cbl.FID);
                        rs.Update("G3E_FNO", 7000);
                        rs.Update("G3E_CID", 1);
                    }
                    else
                    {
                        rs.MoveLast();
                    }
                    rs.Update("CLASS", "Pair Count");
                    rs.Update("CURRENT_DESIGNATION", cbl.CABLE_CODE);
                    rs.Update("PROPOSED_DESIGNATION", cbl.CABLE_CODE);
                    rs.Update("PROPOSED_FEED_DIR", "F");
                    rs.Update("PROPOSED_HIGH", cbl.Hi(cid));
                    rs.Update("PROPOSED_LOW", cbl.Lo(cid));
                    rs.Update("CURRENT_FEED_DIR", "F");
                    rs.Update("CURRENT_HIGH", cbl.Hi(cid));
                    rs.Update("CURRENT_LOW", cbl.Lo(cid));
                    rs.Update("SEQ", 1);

                    if (cbl.Lo(cid) != 0 && cbl.Hi(cid) != 0)
                    {
                        if (anno.Length > 0)
                            anno += Environment.NewLine;

                        if ((cid == cbl.CID - 1) && remark == "CC")
                            anno += "(CC) ";

                        anno += cbl.Lo(cid).ToString() + "-" + cbl.Hi(cid).ToString();

                        rs.Update("COUNT_ANNOTATION", cbl.Lo(cid).ToString() + "-" + cbl.Hi(cid).ToString());
                        effective += (cbl.Hi(cid) - cbl.Lo(cid) + 1);
                    }
                    else
                        rs.Update("COUNT_ANNOTATION", "");

                }

                rs = oFeature.Components.GetComponent(7001).Recordset;
                if (remark == "ORIG")
                    if (cbl.COUNT_ORI.Length > 0)
                        anno += Environment.NewLine + "(ORIG) " + cbl.COUNT_ORI;
                    else
                        anno += Environment.NewLine + "(ORIG) " + cbl.COUNT_ANNO;

                rs.Update("COUNT_ANNOTATION", anno);
                rs.Update("EFFECTIVE_PAIRS", effective.ToString());

                if (cbl.FEATURE_STATE == "ASB")
                {
                    rs = oFeature.Components.GetComponent(51).Recordset;
                    rs.Update("FEATURE_STATE", "MOD");
                }

            }
            else
            {
                IGTKeyObject oFeature;
                oFeature = GTClassFactory.Create<IGTApplication>().DataContext.OpenFeature(7000, cbl.FID);
                ADODB.Recordset rs = oFeature.Components.GetComponent(7001).Recordset;
                rs.Update("COUNT_ANNOTATION", "");
                rs.Update("EFFECTIVE_PAIRS", cbl.EFFECTIVE_PAIRS.ToString());
            }
        }

        internal static void SavePairCountESide(clsESIDE cbl, string remark)
        {
            if (cbl.CID > 0)
            {
                string anno = "";
                int effective = 0;
                ADODB.Recordset rs;
                IGTKeyObject oFeature = null;

                for (int cid = 0; cid < cbl.CID; cid++)
                {
                    oFeature = GTClassFactory.Create<IGTApplication>().DataContext.OpenFeature(7000, cbl.FID);
                    rs = oFeature.Components.GetComponent(55).Recordset; // 55 is CNO for GC_COUNT

                    if (rs.EOF)
                    {
                        rs.AddNew("G3E_FID", cbl.FID);
                        rs.Update("G3E_FNO", 7000);
                        rs.Update("G3E_CID", 1);
                    }
                    else
                    {
                        rs.MoveLast();
                    }

                    #region Update Recordset - GC_COUNT
                    rs.Update("CLASS", "Pair Count");
                    rs.Update("CURRENT_DESIGNATION", cbl.CABLE_CODE);
                    rs.Update("PROPOSED_DESIGNATION", cbl.CABLE_CODE);
                    rs.Update("PROPOSED_FEED_DIR", "F");
                    rs.Update("PROPOSED_HIGH", cbl.Hi(cid));
                    rs.Update("PROPOSED_LOW", cbl.Lo(cid));
                    rs.Update("CURRENT_FEED_DIR", "F");
                    rs.Update("CURRENT_HIGH", cbl.Hi(cid));
                    rs.Update("CURRENT_LOW", cbl.Lo(cid));

                    rs.Update("EHI_PR", cbl.Hi(cid));
                    rs.Update("ELO_PR", cbl.Lo(cid));
                    try
                    {
                        rs.Update("HIGH_PORT", cbl.Vert_Hi(cid));
                        rs.Update("LOW_PORT", cbl.Vert_Lo(cid));
                    }
                    catch { }
                    rs.Update("MDF_UNIT", cbl.MDF(cid));
                    rs.Update("VERTICAL_UNIT", cbl.Vertical(cid));
                    rs.Update("SEQ", cid + 1);
                    #endregion

                    if (cbl.Lo(cid) != 0 && cbl.Hi(cid) != 0)
                    {
                        if (anno.Length > 0) anno += Environment.NewLine;

                        int psg = (cbl.Hi(cid) - cbl.Lo(cid)) + 1;

                        string a = cbl.EXC_ABB + "/" + cbl.MDF(cid).ToString() + "/" + cbl.Vertical(cid).ToString() + "/"
                            + (cbl.Vert_Lo(cid) == 0 ? "" : cbl.Vert_Lo(cid).ToString()) + "-"
                            + (cbl.Vert_Hi(cid) == 0 ? "" : cbl.Vert_Hi(cid).ToString()) + "/"
                            + (cbl.Lo(cid) == 0 ? "" : cbl.Lo(cid).ToString()) + "-"
                            + (cbl.Hi(cid) == 0 ? "" : cbl.Hi(cid).ToString()) + "/"
                            + (psg == 0 ? "" : psg.ToString() + " psg");

                        anno += a;
                        rs.Update("COUNT_ANNOTATION", a);
                    }
                    else
                        rs.Update("COUNT_ANNOTATION", "");

                }

                rs = oFeature.Components.GetComponent(7001).Recordset;
                if (remark == "ORIG")
                    if (cbl.COUNT_ORI.Length > 0)
                        anno += Environment.NewLine + "(ORIG) " + cbl.COUNT_ORI;
                    else
                        anno += Environment.NewLine + "(ORIG) " + cbl.COUNT_ANNO;
                else
                    anno = "(CC) " + anno;

                rs.Update("COUNT_ANNOTATION", anno);
                rs.Update("EFFECTIVE_PAIRS", effective.ToString());
            }
            else
            {
                IGTKeyObject oFeature;
                oFeature = GTClassFactory.Create<IGTApplication>().DataContext.OpenFeature(7000, cbl.FID);
                ADODB.Recordset rs = oFeature.Components.GetComponent(7001).Recordset;
                rs.Update("COUNT_ANNOTATION", "");
                rs.Update("EFFECTIVE_PAIRS", cbl.EFFECTIVE_PAIRS.ToString());
            }
        }

        internal static void PlacePairCountLabel(clsCableCount cbl)
        {
            #region AUTO PLACEMENT OF COUNT ANNOTATION
            IGTTextPointGeometry oCableTextTL;
            oCableTextTL = GTClassFactory.Create<IGTTextPointGeometry>();

            oCableTextTL.Origin = cbl.END_POINT;
            oCableTextTL.Rotation = 0;

            // notes : gc_cblcnt_tl_t = 7034 : dgc_cblcnt_tl_t = 7035 : gc_cblcnt_bl_t = 7032
            short CNO = (short)(cbl.DETAIL_ID > 0 ? 7035 : 7034);

            IGTKeyObject oFeature;
            oFeature = GTClassFactory.Create<IGTApplication>().DataContext.OpenFeature(7000, cbl.FID);
            ADODB.Recordset rs = oFeature.Components.GetComponent(CNO).Recordset;

            if (rs.EOF)
            {
                rs.AddNew("G3E_FID", cbl.FID);
                rs.Update("G3E_FNO", 7000);
                rs.Update("G3E_CID", 1);
            }
            else
                rs.MoveLast();

            oFeature.Components.GetComponent(CNO).Geometry = oCableTextTL;
            if (cbl.DETAIL_ID > 0)
                oFeature.Components.GetComponent(CNO).Recordset.Update("G3E_DETAILID", cbl.DETAIL_ID);
            #endregion
        }
    }

}
