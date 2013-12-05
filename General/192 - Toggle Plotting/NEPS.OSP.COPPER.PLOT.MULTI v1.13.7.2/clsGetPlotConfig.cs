using System;
using System.Collections.Generic;
using System.Text;

using System.Data;
using System.Text;

//using System.Drawing .Graphics;

using System.Runtime.InteropServices;


using System.Windows.Forms;
using System.Diagnostics;
using System.ComponentModel;

using Intergraph.GTechnology.Interfaces;
using Intergraph.GTechnology.API;
//using AG.GTechnology.Utilities;

using ADODB;

namespace NEPS.OSP.COPPER.PLOT.MULTI
{
    class clsGetPlotConfig
    {

        [DllImport("Kernel32.dll")]
        internal static extern int GetUserDefaultLCID();

        private static Intergraph.GTechnology.API.IGTApplication GTApps = GTClassFactory.Create<IGTApplication>();

        public string GetUsername(string sComponentName)
        {
            Recordset rsComp = null;
            int recordsAffected = 0;
            try
            {
                string ssql = "SELECT g3e_username FROM g3e_componentinfo_optlang ";
                ssql += "WHERE g3e_name = '" + sComponentName + "' ";
                ssql += "AND g3e_lcid = '" + GetLanguage() + "'";

                rsComp = GTApps.DataContext.Execute(ssql, out recordsAffected, (int)CommandTypeEnum.adCmdText, null);

                if (rsComp != null && !rsComp.EOF)
                    if (rsComp.Fields["g3e_username"].Value != DBNull.Value)
                        return rsComp.Fields["g3e_username"].Value.ToString();

            }
            catch (Exception ex)
            {
                MessageBox.Show(GTMultiPlot.m_CustomForm,"PlotBoundaryForm.GetUsername:\n" + ex.Message);
            }
            return string.Empty;
        }

        public Recordset GetRedlines(int iDRI_ID)
        {
            string ssql;
            object vParam = null;
            int recordsAffected = 0;
            try
            {
                ssql = "SELECT g.GROUP_NO, g.GROUP_NAME, DRI_ID, USERPLACE, GROUP_OFFSET_X, GROUP_OFFSET_Y, RL_NO, RL_DATATYPE, RL_COORDINATE_X1, RL_COORDINATE_Y1, RL_COORDINATE_X2, RL_COORDINATE_Y2, RL_STYLE_NUMBER, RL_TEXT_ALIGNMENT, RL_ROTATION,    RL_TEXT, RL_USERINPUT, RL_NAME, RL_TEXT_FR ";
                ssql += "FROM apl_groups_dri g, apl_redlines r ";
                ssql += "WHERE g.dri_id = '" + iDRI_ID + "' ";
                ssql += "AND g.group_no = r.group_no AND rl_datatype = 'Redline Lines'";
                return GTApps.DataContext.Execute(ssql, out recordsAffected, (int)CommandTypeEnum.adCmdText, vParam);
            }
            catch (Exception ex)
            {
                MessageBox.Show(GTMultiPlot.m_CustomForm,"PlotBoundaryForm.GetRedlines :\r\n" + ex.Message, ex.Source, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return null;
            }
        }

        public Recordset GetTypes()
        {
            string ssql;
            object vParam = null;
            int recordsAffected = 0;
            try
            {
                ssql = "SELECT DISTINCT dri_type FROM apl_drawinginfo ";
                ssql += "WHERE dri_type IS NOT NULL ORDER BY dri_type";
                return GTApps.DataContext.Execute(ssql, out recordsAffected, (int)CommandTypeEnum.adCmdText, vParam);
            }
            catch (Exception ex)
            {
                MessageBox.Show(GTMultiPlot.m_CustomForm,"PlotBoundaryForm.GetTypes :\r\n" + ex.Message,ex.Source, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return null;
            }
        }

        public Recordset GetScales()
        {
            string ssql;
            object vParam = null;
            int recordsAffected = 0;
            try
            {
                ssql = "SELECT DISTINCT scale FROM APL_PLOT_SCALE ";
                ssql += "WHERE scale IS NOT NULL ORDER BY scale";
                return GTApps.DataContext.Execute(ssql, out recordsAffected, (int)CommandTypeEnum.adCmdText, vParam);
            }
            catch (Exception ex)
            {
                MessageBox.Show(GTMultiPlot.m_CustomForm,"PlotBoundaryForm.GetScales :\r\n" + ex.Message, ex.Source, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return null;
            }
        }

        public void GetMapLocation(int iDRI_ID, ref IGTPoint oMapTLPoint, ref IGTPoint oMapBRPoint)
        {
            string ssql;
            ADODB.Recordset rsMapSize;
            object vParam = null;
            int recordsAffected = 0;
            try
            {
                ssql = "SELECT g.dri_id, g.group_no, g.group_offset_x, g.group_offset_y, ";
                ssql += "mf_coordinate_x1, mf_coordinate_y1, mf_coordinate_x2, mf_coordinate_y2 ";
                ssql += "FROM apl_groups_dri g, apl_mapframe r ";
                ssql += "WHERE g.dri_id = '" + iDRI_ID + "' AND g.group_no = r.group_no ";
                ssql += "AND mf_datatype = 'Map Frame' ORDER BY group_no";

                rsMapSize = GTApps.DataContext.Execute(ssql, out recordsAffected, (int)CommandTypeEnum.adCmdText, vParam);
                if (rsMapSize.EOF) return;

                // Change using group_offset - Phuong
                oMapTLPoint = GTClassFactory.Create<IGTPoint>();
                oMapTLPoint.X = Convert.ToDouble(rsMapSize.Fields["MF_COORDINATE_X1"].Value) + Convert.ToDouble(rsMapSize.Fields["GROUP_OFFSET_X"].Value);
                oMapTLPoint.Y = Convert.ToDouble(rsMapSize.Fields["MF_COORDINATE_Y1"].Value) + Convert.ToDouble(rsMapSize.Fields["GROUP_OFFSET_Y"].Value);
                oMapBRPoint = GTClassFactory.Create<IGTPoint>();
                oMapBRPoint.X = Convert.ToDouble(rsMapSize.Fields["MF_COORDINATE_X2"].Value) + Convert.ToDouble(rsMapSize.Fields["GROUP_OFFSET_X"].Value);
                oMapBRPoint.Y = Convert.ToDouble(rsMapSize.Fields["MF_COORDINATE_Y2"].Value) + Convert.ToDouble(rsMapSize.Fields["GROUP_OFFSET_Y"].Value);

            }
            catch (Exception ex)
            {
                MessageBox.Show(GTMultiPlot.m_CustomForm,"PlotBoundaryForm.GetMapLocation :\r\n" + ex.Message, ex.Source, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        public void GetMapSize(int iDRI_ID, ref double dMapHeight, ref double dMapWidth)
        {
            IGTPoint oMapTLPoint = GTClassFactory.Create<IGTPoint>();
            IGTPoint oMapBRPoint = GTClassFactory.Create<IGTPoint>();
            try
            {
                GetMapLocation(iDRI_ID, ref oMapTLPoint, ref oMapBRPoint);
                dMapHeight = (oMapBRPoint.Y - oMapTLPoint.Y);
                dMapWidth = (oMapBRPoint.X - oMapTLPoint.X);
            }
            catch (Exception ex)
            {
                MessageBox.Show(GTMultiPlot.m_CustomForm,"PlotBoundaryForm.GetMapSize :\r\n" + ex.Message, ex.Source, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        public void GetSheetSize(string sType, string sSheetName, string sSheetOrientation, ref int lSheetId, ref double dSheetHeight, ref double dSheetWidth, ref double dSheetInset, ref int iDRI_ID)
        {
            string ssql;
            Recordset rsSheetSize;
            object vParam = null;
            int recordsAffected = 0;
            try
            {
                ssql = "SELECT s.SHEET_ID, s.SHEET_NAME, s.SHEET_HEIGHT, s.SHEET_WIDTH, s.SHEET_SIZE, s.SHEET_ORIENTATION, s.SHEET_INSET, s.SHEET_INSET_STYLE_NO, d.DRI_ID, d.DRI_TYPE, d.DRI_NAME, d.MODULE ";
                ssql += "FROM apl_sheets s, apl_drawinginfo d WHERE s.sheet_id = d.sheet_id ";
                if (sType == "")
                    ssql += "AND d.dri_type is null ";
                else
                    ssql += "AND d.dri_type = '" + sType + "' ";

                ssql += "AND s.sheet_name = '" + sSheetName + "' ";
                ssql += "AND s.sheet_orientation = '" + sSheetOrientation + "'";

                rsSheetSize = GTApps.DataContext.Execute(ssql, out recordsAffected, (int)CommandTypeEnum.adCmdText, vParam);
                if (rsSheetSize.EOF)
                {
                    iDRI_ID = 0;
                    return;
                }
                lSheetId = Convert.ToInt32(rsSheetSize.Fields["SHEET_ID"].Value);
                dSheetHeight = Convert.ToDouble(rsSheetSize.Fields["SHEET_HEIGHT"].Value);
                dSheetWidth = Convert.ToDouble(rsSheetSize.Fields["SHEET_WIDTH"].Value);
                dSheetInset = Convert.ToDouble(rsSheetSize.Fields["SHEET_INSET"].Value);
                iDRI_ID = Convert.ToInt32(rsSheetSize.Fields["DRI_ID"].Value);
                rsSheetSize.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(GTMultiPlot.m_CustomForm, "PlotBoundaryForm.GetSheetSize :\r\n" + ex.Message, ex.Source, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        public Recordset GetPaperSizes()
        {
            string ssql;
            Recordset rsPaperSizes;
            object vParam = null;
            int recordsAffected = 0;
            try
            {
                ssql = "SELECT DISTINCT sheet_size, sheet_name FROM apl_sheets ORDER BY TO_NUMBER(SUBSTR(sheet_size,1,2))";
                return GTApps.DataContext.Execute(ssql, out recordsAffected, (int)CommandTypeEnum.adCmdText, vParam);
            }
            catch (Exception ex)
            {
                MessageBox.Show(GTMultiPlot.m_CustomForm,"PlotBoundaryForm.GetPaperSizes: \r\n" + ex.Message, ex.Source, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return null;
            }
        }

        public string GetLanguage()
        {
            int LCID;
            int PLangId;
            int sLangId;
            string slang;
            try
            {
                // Get current user LCID
                LCID = GetUserDefaultLCID();
                // LCID's Primary language ID
                PLangId = (LCID & 1023);
                sLangId = (LCID / (2 | 10));
                // TODO: Warning!!! The operator should be an XOR ^ instead of an OR, but not available in CodeDOM
                if ((PLangId == 9))
                {
                    slang = ("000" + PLangId);
                }
                else if ((PLangId == 12))
                {
                    slang = "000C";
                }
                else
                {
                    slang = "0009";
                }
                return slang;
            }
            catch (Exception ex)
            {
                MessageBox.Show(GTMultiPlot.m_CustomForm,"PlotBoundaryForm.GetLanguage :\r\n" + ex.Message, ex.Source, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return string.Empty;
            }
        }

    }
}
