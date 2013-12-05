using ADODB;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Text;
using Intergraph.GTechnology.API;
using AG.GTechnology.PlottingBnd;

namespace AG.GTechnology.Utilities
{
    public class PlotBoundary
    {
        public string Name = "Plot 1";
        public string Type = "Engineering";
        public string PaperSize = "17 x 22";
        public string PaperOrientation = "Landscape";
        public int SheetID = 2;
        public int SheetInset = 7;
        public int SheetWidth = 420;
        public int SheetHeigh = 297;
        public int SheetStyleNo = 5011;
        public int DRI_ID = 70003;
        public List<FieldAttribute> Attributes;
        public int FNO = 0;
        public IGTPoint TopRight;
        public IGTPoint BottomLeft;
        public IGTGeometry Geometry;
        public string MapScale = "1000";
        public string Legend = "Geobase Engineering";
        public IGTDisplayControlNodes ActiveLegendNodes;
        public string MapFrameName;
        public bool Adhoc = false;
        public bool InsertActiveMapWindow = true;

        public string ActiveMapWindow_LegendName;
        public int ActiveMapWindow_DetailID;
        public IGTWorldRange ActiveMapWindow_Range;
        public IGTMapWindow activemap = null;
    }

    public class FieldAttribute
    {
        public string G3E_NAME = "";
        public string G3E_FIELD = "";
        public string VALUE = "";
        public string DATA_DEFAULT = "";
        public int FNO = 0;
    }

    class PPlottingLib
    {
        private const double dSCALE = 100;
        private static List<object> arrPlotStyle;
        private static List<IGTGeometry> arrPlotGeom;
        private static Intergraph.GTechnology.API.IGTApplication application = GTClassFactory.Create<IGTApplication>();

        public static List<IGTGeometry> PlotGeom
        {
            get
            {
                return arrPlotGeom;
            }
        }

        public static List<object> PlotStyle
        {
            get
            {
                return arrPlotStyle;
            }
        }

        private static List<FieldAttribute> GetAttributes(PlotBoundary oPlotBoundary)
        {
            bool bFound;
            FieldAttribute oAttribute = null;
            List<FieldAttribute> oAttributes = null;
            Recordset rsRL_TEXT = null;
            Recordset rsAttributes = null;
            string strSql = "";
            string strText = "";
            string strText2 = "";
            Intergraph.GTechnology.API.IGTApplication application = GTClassFactory.Create<IGTApplication>();
            IGTDataContext m_GTDataContext = application.DataContext;

            try
            {

                //
                // Get all the attributes used by the selected Type & Sheet values
                //

                strSql = " SELECT rl_text ";
                strSql = strSql + "  FROM apl_redlines ";
                strSql = strSql + " WHERE rl_datatype = 'Redline Text' ";
                strSql = strSql + "   AND rl_text LIKE '%[%]%' ";
                strSql = strSql + "   AND group_no IN ( ";
                strSql = strSql + "          SELECT group_no ";
                strSql = strSql + "            FROM apl_groups_dri ";
                strSql = strSql + "           WHERE dri_id IN ( ";
                strSql = strSql + "                    SELECT dri_id ";
                strSql = strSql + "                      FROM apl_drawinginfo ";
                strSql = strSql + "                     WHERE dri_type " + (string.IsNullOrEmpty(oPlotBoundary.Type) ? "IS NULL" : "='" + oPlotBoundary.Type + "'");
                strSql = strSql + "                       AND sheet_id IN ( ";
                strSql = strSql + "                              SELECT sheet_id ";
                strSql = strSql + "                                FROM apl_sheets ";
                strSql = strSql + "                               WHERE sheet_size = '" + oPlotBoundary.PaperSize + "' ";
                strSql = strSql + "                                 AND sheet_orientation = '" + (oPlotBoundary.PaperOrientation) + "'))) ";
                rsRL_TEXT = application.DataContext.OpenRecordset(strSql, ADODB.CursorTypeEnum.adOpenStatic, ADODB.LockTypeEnum.adLockReadOnly, 1);


                // Get Attribute info from metadata
                strSql = " SELECT * ";
                strSql = strSql + "  FROM g3e_attributeinfo_optlang ";
                strSql = strSql + " WHERE g3e_cno IN (SELECT g3e_cno ";
                strSql = strSql + "                     FROM g3e_featurecomps_optable ";
                strSql = strSql + "                    WHERE g3e_fno = " + oPlotBoundary.FNO.ToString() + ") ";
                strSql = strSql + " ORDER BY G3E_REQUIRED DESC";

                rsAttributes = application.DataContext.OpenRecordset(strSql, ADODB.CursorTypeEnum.adOpenStatic, ADODB.LockTypeEnum.adLockReadOnly, 1);

                oAttributes = new List<FieldAttribute>();

                while (!rsAttributes.EOF)
                {
                    if (rsRL_TEXT.RecordCount > 0) rsRL_TEXT.MoveFirst();
                    while (!rsRL_TEXT.EOF)
                    {

                        strText = rsRL_TEXT.Fields["RL_TEXT"].Value.ToString();
                        strText2 = rsAttributes.Fields["G3E_FIELD"].Value.ToString();

                        if (strText.IndexOf("[" + rsAttributes.Fields["G3E_FIELD"].Value + "]") != 0)
                        {
                            oAttribute = new FieldAttribute();
                            {
                                oAttribute.G3E_FIELD = (rsAttributes.Fields["G3E_FIELD"].Value == System.DBNull.Value ? "" : rsAttributes.Fields["G3E_FIELD"].Value.ToString());
                                //oAttribute.G3E_USERNAME = (rsAttributes.Fields["G3E_USERNAME"].Value == System.DBNull.Value ? "" : rsAttributes.Fields["G3E_USERNAME"].Value);
                                //oAttribute.G3E_FORMAT = (rsAttributes.Fields["G3E_FORMAT"].Value == System.DBNull.Value ? "" : rsAttributes.Fields["G3E_FORMAT"].Value);

                                //oAttribute.G3E_PRECISION = (rsAttributes.Fields["G3E_PRECISION"].Value == System.DBNull.Value ? 0 : rsAttributes.Fields["G3E_PRECISION"].Value);

                                //if (!(rsAttributes.Fields["G3E_PNO"].Value == System.DBNull.Value)) oAttribute.G3E_PRECISION = rsAttributes.Fields["G3E_PNO"].Value;

                                //oAttribute.G3E_ADDITIONALREFFIELDS = (rsAttributes.Fields["G3E_ADDITIONALREFFIELDS"].Value == System.DBNull.Value ? "" : rsAttributes.Fields["G3E_ADDITIONALREFFIELDS"].Value);
                                //oAttribute.G3E_PICKLISTTABLE = (rsAttributes.Fields["G3E_PICKLISTTABLE"].Value == System.DBNull.Value ? "" : rsAttributes.Fields["G3E_PICKLISTTABLE"].Value);
                                //oAttribute.G3E_KEYFIELD = (rsAttributes.Fields["G3E_KEYFIELD"].Value == System.DBNull.Value ? "" : rsAttributes.Fields["G3E_KEYFIELD"].Value);
                                //oAttribute.G3E_VALUEFIELD = (rsAttributes.Fields["G3E_VALUEFIELD"].Value == System.DBNull.Value ? "" : rsAttributes.Fields["G3E_VALUEFIELD"].Value);
                                //oAttribute.G3E_FILTER = (rsAttributes.Fields["G3E_FILTER"].Value == System.DBNull.Value ? "" : rsAttributes.Fields["G3E_FILTER"].Value);
                                //oAttribute.G3E_ADDITIONALFIELDS = (rsAttributes.Fields["G3E_ADDITIONALFIELDS"].Value == System.DBNull.Value ? "" : rsAttributes.Fields["G3E_ADDITIONALFIELDS"].Value);
                                oAttribute.G3E_NAME = (rsAttributes.Fields["G3E_NAME"].Value == System.DBNull.Value ? "" : rsAttributes.Fields["G3E_NAME"].Value.ToString());
                                oAttribute.DATA_DEFAULT = (rsAttributes.Fields["DATA_DEFAULT"].Value == System.DBNull.Value ? "" : rsAttributes.Fields["DATA_DEFAULT"].Value.ToString());
                            }

                            // Check if attribute already defined - Used to filter out duplicate attributes that may exist in common components of the feature
                            bFound = false;
                            foreach (FieldAttribute oAttributeExists in oAttributes)
                            {
                                if (oAttributeExists.G3E_FIELD == oAttribute.G3E_FIELD) bFound = true;
                            }

                            if (bFound == false) oAttributes.Add(oAttribute);
                        }
                        rsRL_TEXT.MoveNext();
                    }
                    rsAttributes.MoveNext();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Plotting Boundary", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return oAttributes;
        }

        private static void PopulateRedlineGroupInfo(ref ADODB.Recordset rsRedlines, ref ADODB.Recordset rsGroupsDRI, PlotBoundary oPlotBoundary, int plotCount, int totalPlot, string[] formData)
        {
            try
            {
                string strDatatype = null;
                string strText = null;
                string strFieldName = "";

                IGTPoint oGTPoint = GTClassFactory.Create<IGTPoint>();
                IGTPolygonGeometry oPolygonGeometry = GTClassFactory.Create<IGTPolygonGeometry>();
                IGTPolylineGeometry oPolylineGeometry = GTClassFactory.Create<IGTPolylineGeometry>();
                IGTOrientedPointGeometry oOrientedPointGeometry = GTClassFactory.Create<IGTOrientedPointGeometry>();
                IGTTextPointGeometry oTextPointGeometry = GTClassFactory.Create<IGTTextPointGeometry>();
                IGTVector oGTVector = GTClassFactory.Create<IGTVector>();

                IGTKeyObject oGTKeyObject = GTClassFactory.Create<IGTKeyObject>();
                IGTComponent oGTComponent = GTClassFactory.Create<IGTComponent>();
                IGTComponents oGTComponents = GTClassFactory.Create<IGTComponents>();
                IGTWorldRange oGTWorldRange = GTClassFactory.Create<IGTWorldRange>();
                IGTPaperRange oGTPaperRange = GTClassFactory.Create<IGTPaperRange>();
                IGTPlotMap oGTPlotMap = GTClassFactory.Create<IGTPlotMap>();
                IGTPlotRedline oGTPlotRedline = GTClassFactory.Create<IGTPlotRedline>();
                IGTNamedPlot oGTNamedPlot = GTClassFactory.Create<IGTNamedPlot>();

                int iGroupNumber = -1;

                string sStatusBarText = application.GetStatusBarText(GTStatusPanelConstants.gtaspcMessage);

                arrPlotGeom = new List<IGTGeometry>();
                arrPlotStyle = new List<object>();

                oGTNamedPlot = application.NamedPlots[oPlotBoundary.Name];

                while (!rsRedlines.EOF)
                {

                    strDatatype = rsRedlines.Fields["RL_DATATYPE"].Value.ToString();

                    switch (strDatatype)
                    {
                        //
                        // Area redline
                        //
                        case GTRedlineGroupDataTypeConstants.gtrgdtRedlineAreas:

                            oPolygonGeometry = GTClassFactory.Create<IGTPolygonGeometry>();

                            oGTPoint = GTClassFactory.Create<IGTPoint>();
                            oGTPoint.X = Convert.ToDouble(rsRedlines.Fields["RL_COORDINATE_X1"].Value) * dSCALE + Convert.ToDouble(rsGroupsDRI.Fields["GROUP_OFFSET_X"].Value) * dSCALE;
                            oGTPoint.Y = Convert.ToDouble(rsRedlines.Fields["RL_COORDINATE_Y1"].Value) * dSCALE + Convert.ToDouble(rsGroupsDRI.Fields["GROUP_OFFSET_Y"].Value) * dSCALE;
                            oGTPoint.Z = 0;
                            oPolygonGeometry.Points.Add(oGTPoint);

                            oGTPoint.X = Convert.ToDouble(rsRedlines.Fields["RL_COORDINATE_X2"].Value) * dSCALE + Convert.ToDouble(rsGroupsDRI.Fields["GROUP_OFFSET_X"].Value) * dSCALE;
                            oGTPoint.Y = Convert.ToDouble(rsRedlines.Fields["RL_COORDINATE_Y2"].Value) * dSCALE + Convert.ToDouble(rsGroupsDRI.Fields["GROUP_OFFSET_Y"].Value) * dSCALE;
                            oGTPoint.Z = 0;
                            oPolygonGeometry.Points.Add(oGTPoint);

                            oGTPlotRedline = oGTNamedPlot.NewRedline(oPolygonGeometry, Convert.ToInt32(rsRedlines.Fields["RL_STYLE_NUMBER"].Value), iGroupNumber);
                            iGroupNumber = oGTPlotRedline.GroupNumber;

                            arrPlotGeom.Add(oPolygonGeometry);
                            arrPlotStyle.Add(Convert.ToInt32(rsRedlines.Fields["RL_STYLE_NUMBER"].Value));

                            break;

                        //
                        // Point redline
                        //
                        case GTRedlineGroupDataTypeConstants.gtrgdtRedlinePoints:

                            oGTPoint = GTClassFactory.Create<IGTPoint>();
                            oGTPoint.X = Convert.ToDouble(rsRedlines.Fields["RL_COORDINATE_X1"].Value) * dSCALE + Convert.ToDouble(rsGroupsDRI.Fields["GROUP_OFFSET_X"].Value) * dSCALE;
                            oGTPoint.Y = Convert.ToDouble(rsRedlines.Fields["RL_COORDINATE_Y1"].Value) * dSCALE + Convert.ToDouble(rsGroupsDRI.Fields["GROUP_OFFSET_Y"].Value) * dSCALE;
                            oGTPoint.Z = 0;

                            oOrientedPointGeometry = GTClassFactory.Create<IGTOrientedPointGeometry>();
                            oOrientedPointGeometry.Origin = oGTPoint;

                            oGTPlotRedline = oGTNamedPlot.NewRedline(oOrientedPointGeometry, Convert.ToInt32(rsRedlines.Fields["RL_STYLE_NUMBER"].Value), iGroupNumber);
                            iGroupNumber = oGTPlotRedline.GroupNumber;

                            arrPlotGeom.Add(oOrientedPointGeometry);
                            arrPlotStyle.Add(Convert.ToInt32(rsRedlines.Fields["RL_STYLE_NUMBER"].Value));

                            break;
                        //
                        // Text redline
                        //
                        case GTRedlineGroupDataTypeConstants.gtrgdtRedlineText:

                            strText = rsRedlines.Fields["RL_TEXT"].Value.ToString();

                            //Hardcode for special keywords
                            if (strText.ToLower().IndexOf("[pl_scale]") >= 0)
                            {
                                strText = strText.Replace("[pl_scale]", oPlotBoundary.MapScale);
                                strText = strText.Replace("[PL_SCALE]", oPlotBoundary.MapScale);
                            }
                            if (strText.ToLower().IndexOf("[cr_by]") >= 0)
                            {
                                //string sFullName = GetFullName();

                                strText = strText.Replace("[cr_by]", formData[4]);
                                strText = strText.Replace("[CR_BY]", formData[4]);
                            }
                            if (strText.ToLower().IndexOf("[cr_date]") >= 0)
                            {
                                strText = strText.Replace("[cr_date]", string.Format("{0:yyyyMMdd}", DateTime.Today));
                                strText = strText.Replace("[CR_DATE]", string.Format("{0:yyyyMMdd}", DateTime.Today));
                            }
                            if (strText.ToLower().IndexOf("[plot_title]") >= 0)
                            {
                                //strText = strText.Replace("[plot_type]", oPlotBoundary.Legend+" "+ oPlotBoundary.Type);
                                //strText = strText.Replace("[PLOT_TYPE]", oPlotBoundary.Legend + " " + oPlotBoundary.Type);
                                string strJobId = application.DataContext.ActiveJob;
                                
                                strText = strText.Replace("[plot_title]", strJobId + " " + oPlotBoundary.Type);
                                strText = strText.Replace("[PLOT_title]", strJobId + " " + oPlotBoundary.Type);
                            }
                            if (strText.ToLower().IndexOf("[plot_type]") >= 0)
                            {
                                //strText = strText.Replace("[plot_type]", oPlotBoundary.Legend+" "+ oPlotBoundary.Type);
                                //strText = strText.Replace("[PLOT_TYPE]", oPlotBoundary.Legend + " " + oPlotBoundary.Type);
                                string strJobId = application.DataContext.ActiveJob;

                                strText = strText.Replace("[plot_type]", oPlotBoundary.Type);
                                strText = strText.Replace("[PLOT_type]", oPlotBoundary.Type);
                            }
                            if (strText.ToLower().IndexOf("[exc_abb]") >= 0)
                            {
                                string sExc_abb = GetExcAbb();

                                strText = strText.Replace("[exc_abb]", sExc_abb);
                                strText = strText.Replace("[EXC_ABB]", sExc_abb);
                            }
                            if (strText.ToLower().IndexOf("[app_by]") >= 0)
                            {
                                //string sExc_abb = GetExcAbb();

                                strText = strText.Replace("[app_by]", formData[6]);
                                strText = strText.Replace("[APP_BY]", formData[6]);
                            }
                            if (strText.ToLower().IndexOf("[check_by]") >= 0)
                            {
                                //string sExc_abb = GetExcAbb();

                                strText = strText.Replace("[check_by]", formData[5]);
                                strText = strText.Replace("[CHECK_BY]", formData[5]);
                            }
                            if (strText.ToLower().IndexOf("[desc1]") >= 0)
                            {
                                //string sExc_abb = GetExcAbb();
                               
                                strText = strText.Replace("[desc1]", formData[0]);
                                strText = strText.Replace("[DESC1]", formData[0]);
                            }
                            if (strText.ToLower().IndexOf("[desc2]") >= 0)
                            {
                                //string sExc_abb = GetExcAbb();

                                strText = strText.Replace("[desc2]", formData[1]);
                                strText = strText.Replace("[DESC2]", formData[1]);
                            }
                            if (strText.ToLower().IndexOf("[desc3]") >= 0)
                            {
                                //string sExc_abb = GetExcAbb();

                                strText = strText.Replace("[desc3]", formData[2]);
                                strText = strText.Replace("[DESC3]", formData[2]);
                            }
                            if (strText.ToLower().IndexOf("[desc4]") >= 0)
                            {
                                //string sExc_abb = GetExcAbb();

                                strText = strText.Replace("[desc4]", formData[3]);
                                strText = strText.Replace("[DESC4]", formData[3]);
                            }
                            if (strText.ToLower().IndexOf("[curr_page]") >= 0)
                            {
                                //string sExc_abb = GetExcAbb();

                                strText = strText.Replace("[curr_page]", Convert.ToString(plotCount));
                                strText = strText.Replace("[CURR_PAGE]", Convert.ToString(plotCount));
                            }
                            if (strText.ToLower().IndexOf("[total_page]") >= 0)
                            {
                                //string sExc_abb = GetExcAbb();

                                strText = strText.Replace("[total_page]", Convert.ToString(totalPlot));
                                strText = strText.Replace("[TOTAL_PAGE]", Convert.ToString(totalPlot));
                            }
                            if (strText.ToLower().StartsWith("[img]|"))
                            {
                                strText = strText.Replace("[img]|", "");
                                strText = strText.Replace("[IMG]|", "");
                                string sPath = System.AppDomain.CurrentDomain.BaseDirectory + strText;
                                oGTPoint = GTClassFactory.Create<IGTPoint>();
                                oTextPointGeometry = GTClassFactory.Create<IGTTextPointGeometry>();
                                oGTVector = GTClassFactory.Create<IGTVector>();

                                oGTPoint.X = Convert.ToDouble(Convert.ToDouble(rsRedlines.Fields["RL_COORDINATE_X1"].Value)) * dSCALE + Convert.ToDouble(rsGroupsDRI.Fields["GROUP_OFFSET_X"].Value) * dSCALE;
                                oGTPoint.Y = Convert.ToDouble(rsRedlines.Fields["RL_COORDINATE_Y1"].Value) * dSCALE + Convert.ToDouble(rsGroupsDRI.Fields["GROUP_OFFSET_Y"].Value) * dSCALE;
                                oGTPoint.Z = 0;

                                if (System.IO.File.Exists(sPath))
                                {
                                    IGTPlotFrame oPlotFrame = application.ActivePlotWindow.InsertObjectFromFile(sPath, false, oGTPoint);
                                    IGTPlotRedline oRedline = oPlotFrame.BorderRedline;
                                    //IGTSymbology oSym = oRedline.Symbology;
                                    //oSym.Color = GTStyleColorConstants.gtcolTransparent;
                                    //oRedline.Symbology = oSym;
                                }
                                strText = string.Empty;
                            }

                            if (!(strText == string.Empty))
                            {
                                oGTPoint = GTClassFactory.Create<IGTPoint>();
                                oTextPointGeometry = GTClassFactory.Create<IGTTextPointGeometry>();
                                oGTVector = GTClassFactory.Create<IGTVector>();

                                oGTPoint.X = Convert.ToDouble(Convert.ToDouble(rsRedlines.Fields["RL_COORDINATE_X1"].Value)) * dSCALE + Convert.ToDouble(rsGroupsDRI.Fields["GROUP_OFFSET_X"].Value) * dSCALE;
                                oGTPoint.Y = Convert.ToDouble(rsRedlines.Fields["RL_COORDINATE_Y1"].Value) * dSCALE + Convert.ToDouble(rsGroupsDRI.Fields["GROUP_OFFSET_Y"].Value) * dSCALE;
                                oGTPoint.Z = 0;

                                {
                                    oTextPointGeometry.Alignment = (GTAlignmentConstants)Convert.ToInt32(rsRedlines.Fields["RL_TEXT_ALIGNMENT"].Value);
                                    // TODO: Fix Text Rotation
                                    oTextPointGeometry.Normal = oGTVector;
                                    oTextPointGeometry.Rotation = Convert.ToDouble(rsRedlines.Fields["RL_ROTATION"].Value);
                                    oTextPointGeometry.Origin = oGTPoint;
                                    //If Not strFieldName = "" And strText = "" Then
                                    //  .Text = "[" & strFieldName & "]"
                                    //Else
                                    oTextPointGeometry.Text = strText;
                                    //End If
                                }
                                oGTPlotRedline = oGTNamedPlot.NewRedline(oTextPointGeometry, Convert.ToInt32(rsRedlines.Fields["RL_STYLE_NUMBER"].Value), iGroupNumber);

                                // Turn readline text that are attributes into AutomaticTextFields
                                if (!string.IsNullOrEmpty(strFieldName))
                                {
                                    oGTPlotRedline.AutomaticTextField = strFieldName;
                                    oGTPlotRedline.AutomaticTextSource = GTPlotAutomaticTextSourceConstants.gtpatPlotByQuery;
                                }


                                arrPlotGeom.Add(oTextPointGeometry);
                                arrPlotStyle.Add(Convert.ToInt32(rsRedlines.Fields["RL_STYLE_NUMBER"].Value));

                                iGroupNumber = oGTPlotRedline.GroupNumber;
                            }


                            break;

                        //
                        // Line redlines
                        //
                        case GTRedlineGroupDataTypeConstants.gtrgdtRedlineLines:

                            oPolylineGeometry = GTClassFactory.Create<IGTPolylineGeometry>();

                            oGTPoint = GTClassFactory.Create<IGTPoint>();
                            oGTPoint.X = Convert.ToDouble(rsRedlines.Fields["RL_COORDINATE_X1"].Value) * dSCALE + Convert.ToDouble(rsGroupsDRI.Fields["GROUP_OFFSET_X"].Value) * dSCALE;
                            oGTPoint.Y = Convert.ToDouble(rsRedlines.Fields["RL_COORDINATE_Y1"].Value) * dSCALE + Convert.ToDouble(rsGroupsDRI.Fields["GROUP_OFFSET_Y"].Value) * dSCALE;
                            oGTPoint.Z = 0;
                            oPolylineGeometry.Points.Add(oGTPoint);

                            oGTPoint = GTClassFactory.Create<IGTPoint>();
                            oGTPoint.X = Convert.ToDouble(rsRedlines.Fields["RL_COORDINATE_X2"].Value) * dSCALE + Convert.ToDouble(rsGroupsDRI.Fields["GROUP_OFFSET_X"].Value) * dSCALE;
                            oGTPoint.Y = Convert.ToDouble(rsRedlines.Fields["RL_COORDINATE_Y2"].Value) * dSCALE + Convert.ToDouble(rsGroupsDRI.Fields["GROUP_OFFSET_Y"].Value) * dSCALE;
                            oGTPoint.Z = 0;
                            oPolylineGeometry.Points.Add(oGTPoint);

                            oGTPlotRedline = oGTNamedPlot.NewRedline(oPolylineGeometry, Convert.ToInt32(rsRedlines.Fields["RL_STYLE_NUMBER"].Value), iGroupNumber);
                            iGroupNumber = oGTPlotRedline.GroupNumber;

                            arrPlotGeom.Add(oPolylineGeometry);
                            arrPlotStyle.Add(Convert.ToInt32(rsRedlines.Fields["RL_STYLE_NUMBER"].Value));

                            break;
                    }


                    rsRedlines.MoveNext();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Plotting Boundary", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public static int GetTypesDefaultLegend(string sType, string sSheetSize, string sSheetOrientation, bool bDetailLegend)
        {
            string strSql;
            ADODB.Recordset rs;
            object vParam = null;
            int recordsAffected = 0;
            try
            {
                if (bDetailLegend)
                {
                    strSql = " SELECT mf_det_lno ";
                }
                else
                {
                    strSql = " SELECT mf_geo_lno ";
                }
                strSql = (strSql + "  FROM apl_mapframe ");
                strSql = (strSql + " WHERE group_no IN ( ");
                strSql = (strSql + "          SELECT group_no ");
                strSql = (strSql + "            FROM apl_groups_dri ");
                strSql = (strSql + "           WHERE dri_id IN ( ");
                strSql = (strSql + "                    SELECT dri_id ");
                strSql = (strSql + "                      FROM apl_drawinginfo ");
                strSql = (strSql + ("                     WHERE dri_type " + ((sType == "") ? "IS NULL" : ("=\'"
                            + (sType + "\'")))));
                strSql = (strSql + "                       AND sheet_id IN ( ");
                strSql = (strSql + "                              SELECT sheet_id ");
                strSql = (strSql + "                                FROM apl_sheets ");
                strSql = (strSql + ("                               WHERE sheet_size = \'"
                            + (sSheetSize + "\' ")));
                strSql = (strSql + ("                                 AND sheet_orientation = \'"
                            + (sSheetOrientation + "\'))) ")));
                rs = application.DataContext.Execute(strSql, out recordsAffected, (int)CommandTypeEnum.adCmdText, vParam);
                //Repos(rs);
                if (bDetailLegend)
                {
                    return Convert.ToInt32(rs.Fields["mf_det_lno"].Value);
                }
                else
                {
                    return Convert.ToInt32(rs.Fields["mf_geo_lno"].Value);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Plotting Boundary", MessageBoxButtons.OK, MessageBoxIcon.Error); return 0;
            }
        }

        public static string GetExcAbb()
        {
            string strSql;
            ADODB.Recordset rs;
            object vParam = null;
            int recordsAffected = 0;
            try
            {
                string strJobId = application.DataContext.ActiveJob;

                if (string.IsNullOrEmpty(strJobId)) return string.Empty;

                strSql = " SELECT exc_abb ";
                strSql = (strSql + "    FROM g3e_job ");
                strSql = (strSql + ("   WHERE G3E_IDENTIFIER='" + strJobId + "'"));
                rs = application.DataContext.Execute(strSql, out recordsAffected, (int)CommandTypeEnum.adCmdText, vParam);
                //Repos(rs);
                if (rs.EOF) return string.Empty;
                return rs.Fields["exc_abb"].Value.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Plotting Boundary", MessageBoxButtons.OK, MessageBoxIcon.Error); return string.Empty;
            }
        }

        public static string GetFullName()
        {
            string strSql;
            ADODB.Recordset rsfn;
            object vParam = null;
            int recordsAffected = 0;
            try
            {
                string strJobId = application.DataContext.ActiveJob;

                if (string.IsNullOrEmpty(strJobId)) return string.Empty;

                strSql = " SELECT u.full_name ";
                strSql = (strSql + "    FROM g3e_job j, wv_user u ");
                strSql = (strSql + ("   WHERE j.g3e_owner = u.username and j.G3E_IDENTIFIER='" + strJobId + "'"));
                rsfn = application.DataContext.Execute(strSql, out recordsAffected, (int)CommandTypeEnum.adCmdText, vParam);
                //Repos(rs);
                if (rsfn.EOF) return string.Empty;
                return rsfn.Fields["full_name"].Value.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Plotting Boundary", MessageBoxButtons.OK, MessageBoxIcon.Error); return string.Empty;
            }
        }

        public static string GetDesc(string descType)
        {
            string strSql;
            ADODB.Recordset rs;
            object vParam = null;
            int recordsAffected = 0;
            try
            {
                string strJobId = application.DataContext.ActiveJob;

                if (string.IsNullOrEmpty(strJobId)) return string.Empty;

                if (descType == "desc1")
                {
                    strSql = " SELECT sch_desc1";
                    strSql = (strSql + " FROM g3e_job");
                    strSql = (strSql + (" WHERE G3E_IDENTIFIER='" + strJobId + "'"));
                }
                else
                {
                    strSql = " SELECT sch_desc2";
                    strSql = (strSql + " FROM g3e_job");
                    strSql = (strSql + (" WHERE G3E_IDENTIFIER='" + strJobId + "'"));
                }
                rs = application.DataContext.Execute(strSql, out recordsAffected, (int)CommandTypeEnum.adCmdText, vParam);
                //Repos(rs);
                if (rs.EOF) return string.Empty;

                if (descType == "desc1")
                    return rs.Fields["sch_desc1"].Value.ToString();
                else
                    return rs.Fields["sch_desc2"].Value.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Plotting Boundary", MessageBoxButtons.OK, MessageBoxIcon.Error); return string.Empty;
            }
        }

        public static string GetLegendName(int iLNO)
        {
            string strSql;
            ADODB.Recordset rs;
            object vParam = null;
            int recordsAffected = 0;
            try
            {
                strSql = " SELECT   g3e_username ";
                strSql = (strSql + "    FROM g3e_legend ");
                strSql = (strSql + ("   WHERE g3e_lno = " + iLNO));
                rs = application.DataContext.Execute(strSql, out recordsAffected, (int)CommandTypeEnum.adCmdText, vParam);
                //Repos(rs);
                if (rs.EOF) return string.Empty;
                return rs.Fields["g3e_username"].Value.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Plotting Boundary", MessageBoxButtons.OK, MessageBoxIcon.Error); return string.Empty;
            }
        }

        private static void LoadLegend(IGTPlotMap oGTPlotMap, PlotBoundary oPlotBoundary)
        {
            long lngDetailID;
            // 
            //  Load the legend
            // 
            try
            {
                oGTPlotMap.DisplayService.ReplaceLegendByMapWindow(oPlotBoundary.activemap);
            }
            catch { oGTPlotMap.DisplayService.ReplaceLegend(oPlotBoundary.Legend); }
           
          //  oGTPlotMap.DisplayService.ReplaceLegend("Traces");
            // If oPlotBoundary.DetailID = 0 Then 'Plot Boundary is in Geo
            //   oGTPlotMap.DisplayService.ReplaceLegend("Design Legend") 'Geobase Default
            //   'oGTPlotMap.DisplayService.ReplaceLegend("Construction Legend") 'Geobase Default
            // Else
            //   lngDetailID = rsAttributeQuery.Fields["G3E_DETAILID"].Value
            //   oGTPlotMap.DisplayService.ReplaceLegend("Detail Default")
            // End If
            return;
        }

        private static void ApplyActiveLegendOverrides(IGTPlotMap oGTPlotMap, PlotBoundary oPlotBoundary)
        {
            try
            {
                IGTDisplayControlNodes fromNodes = oPlotBoundary.ActiveLegendNodes;
                IGTDisplayControlNodes toNodes = oGTPlotMap.DisplayService.GetDisplayControlNodes();
                for (int i = 0; i < fromNodes.Count; i++)
                {
                    IGTDisplayControlNode fromNode = fromNodes[i];
                    string NodeName = fromNode.DisplayPathName;

                    IGTDisplayControlNode toNode = toNodes[i];
                    if (toNode.DisplayPathName == NodeName)
                    {
                        if (toNode.LegendEntry != null)
                        {
                            if (toNode.LegendEntry.Displayable != fromNode.LegendEntry.Displayable)
                                toNode.LegendEntry.Displayable = fromNode.LegendEntry.Displayable;

                            //if (toNode.LegendEntry.DisplayScaleMode != fromNode.LegendEntry.DisplayScaleMode)
                            //    toNode.LegendEntry.DisplayScaleMode = fromNode.LegendEntry.DisplayScaleMode;

                            if (toNode.LegendEntry.Locatable != fromNode.LegendEntry.Locatable)
                                toNode.LegendEntry.Locatable = fromNode.LegendEntry.Locatable;

                            if (toNode.LegendEntry.Filter != fromNode.LegendEntry.Filter)
                                toNode.LegendEntry.Filter = fromNode.LegendEntry.Filter;
                        }
                    }
                }

                return;
            }
            catch( Exception ex)
            {
            }
        }

        public static void ZoomToFitPage(string sPlotWindowCaption)
        {
            string sStatusBarText = application.GetStatusBarText(GTStatusPanelConstants.gtaspcMessage);
            IGTPlotWindow oPlotWindow = null;
            try
            {
                foreach (IGTPlotWindow oWindow in application.GetPlotWindows())
                {
                    if ((oWindow.Caption == sPlotWindowCaption))
                    {
                        oPlotWindow = oWindow;
                        break;
                    }
                }
                //oPlotWindow.ZoomToFit();
                // Fit Page
                // oPlotWindow.ZoomOut()
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Plotting Boundary", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                oPlotWindow = null;
            }
        }

        private static IGTWorldRange GetFeatureRange(PlotBoundary oPlotBoundary)
        {
            IGTWorldRange oGTWorldRange = GTClassFactory.Create <IGTWorldRange>();

            oGTWorldRange.BottomLeft = oPlotBoundary.BottomLeft;
            oGTWorldRange.TopRight = oPlotBoundary.TopRight;
            return oGTWorldRange;
        }

        // 
        //  Position Map in Frame
        // 
        private static void ZoomToPlotBoundary(IGTPlotMap oGTPlotMap, PlotBoundary oPlotBoundary, bool blnUpdateScale)
        {
            IGTWorldRange oGTWorldRange;

            oGTWorldRange = GetFeatureRange(oPlotBoundary);
            oGTPlotMap.Frame.Activate();
            if (blnUpdateScale)
            {
                switch (oPlotBoundary.MapScale)
                {
                    case "Fit Work Plan Boundary":
                        oGTPlotMap.ZoomArea(oGTWorldRange);
                        break;
                    case "Active Map Window Scale":
                        oGTPlotMap.CenterSelectedObjects();
                        // .ZoomArea oGTWorldRange
                        // .DisplayScale = Application.ActiveMapWindow.DisplayScale
                        break;
                    default:
                        oGTPlotMap.Frame.Activate();
                        oGTPlotMap.ZoomArea(oGTWorldRange);
                        oGTPlotMap.DisplayScale = Convert.ToDouble(oPlotBoundary.MapScale.Substring(2));
                        break;
                }
            }
            oGTPlotMap.Frame.Deactivate();
            return;
        }

        public static void RotateMapView(PlotBoundary oPlotBoundary, double dAngle)
        {
            IGTPlotFrames oPlotFrames = null;
            IGTPlotFrame oPlotFrame = null;
            IGTPlotMap oPlotMapFrame = null;
            IGTNamedPlot oNamedPlot = null;
            IGTNamedPlots oNamedPlots = null;
            string sStatusBarText = application.GetStatusBarText(GTStatusPanelConstants.gtaspcMessage);
            try
            {
                foreach (IGTPlotWindow oPlotWindow in application.GetPlotWindows())
                {
                    if ((oPlotWindow.Caption == oPlotBoundary.Name))
                    {
                        oNamedPlot = oPlotWindow.NamedPlot;
                        oPlotFrames = oNamedPlot.Frames;
                        oPlotFrame = oPlotFrames[oPlotBoundary.MapFrameName];
                        if ((oPlotFrame.Type == GTPlotFrameTypeConstants.gtpftMap))
                        {
                            oPlotMapFrame = oPlotFrame.PlotMap;
                            break;
                        }
                    }
                }
                if (!(oPlotMapFrame == null))
                {
                    oPlotFrame.Activate();
                    oPlotMapFrame.Rotation = (dAngle);
                    oPlotFrame.Deactivate();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Plotting Boundary", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                // GTApplication.Application.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, sStatusBarText)
                oPlotFrames = null;
                oPlotFrame = null;
                oPlotMapFrame = null;
                oNamedPlot = null;
                oNamedPlots = null;
            }
        }

        private static void RotateMapView(PlotBoundary oPlotBoundary)
        {
            IGTKeyObject oGTKeyObject;
            IGTComponent oGTComponent;
            IGTComponents oGTComponents;
            IGTGeometry oGTGeometry;
            double dSlope;
            double dAngle;
            double dAngleDeg;
            
            try
            {
                oGTGeometry = oPlotBoundary.Geometry;
                dSlope = ((oGTGeometry.GetKeypointPosition(0).Y - oGTGeometry.GetKeypointPosition(1).Y)
                            / (oGTGeometry.GetKeypointPosition(0).X - oGTGeometry.GetKeypointPosition(1).X));
                dAngle = System.Math.Atan(dSlope);
                // m_dAngle = Atan2(m_pRotateAnchor.Y - UserPoint.Y, m_pRotateAnchor.X - UserPoint.X) ' Using the dSlope and Atan() instead of Atan2() keeps the polygon right reading.
                dAngleDeg = (dAngle * (180 / Math.PI));

                RotateMapView(oPlotBoundary, (dAngle * -1));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Plotting Boundary", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                
            }
        }

        private static IGTPlotMap InsertMapFrame(ADODB.Recordset rsGroupsDRI, ADODB.Recordset rsRedlines)
        {
            IGTPoint oGTPoint;
            IGTPaperRange oGTPaperRange;
            try
            {
                oGTPaperRange = GTClassFactory.Create<IGTPaperRange>();
                oGTPoint = GTClassFactory.Create<IGTPoint>();
                oGTPoint.X = ((Convert.ToDouble(rsRedlines.Fields["MF_COORDINATE_X1"].Value) * dSCALE)
                            + (Convert.ToDouble(rsGroupsDRI.Fields["GROUP_OFFSET_X"].Value) * dSCALE));
                oGTPoint.Y = ((Convert.ToDouble(rsRedlines.Fields["MF_COORDINATE_Y1"].Value) * dSCALE)
                            + (Convert.ToDouble(rsGroupsDRI.Fields["GROUP_OFFSET_Y"].Value) * dSCALE));
                //oGTPoint.X = ((Convert.ToDouble(rsRedlines.Fields["MF_COORDINATE_X1"].Value) * dSCALE));
                //oGTPoint.Y = ((Convert.ToDouble(rsRedlines.Fields["MF_COORDINATE_Y1"].Value) * dSCALE));
                oGTPoint.Z = 0;
                oGTPaperRange.TopLeft = oGTPoint;
                oGTPoint = GTClassFactory.Create<IGTPoint>();
                oGTPoint.X = ((Convert.ToDouble(rsRedlines.Fields["MF_COORDINATE_X2"].Value) * dSCALE)
                            + (Convert.ToDouble(rsGroupsDRI.Fields["GROUP_OFFSET_X"].Value) * dSCALE));
                oGTPoint.Y = ((Convert.ToDouble(rsRedlines.Fields["MF_COORDINATE_Y2"].Value) * dSCALE)
                            + (Convert.ToDouble(rsGroupsDRI.Fields["GROUP_OFFSET_Y"].Value) * dSCALE));
                //oGTPoint.X = ((Convert.ToDouble(rsRedlines.Fields["MF_COORDINATE_X2"].Value) * dSCALE));
                //oGTPoint.Y = ((Convert.ToDouble(rsRedlines.Fields["MF_COORDINATE_Y2"].Value) * dSCALE));
                oGTPoint.Z = 0;
                oGTPaperRange.BottomRight = oGTPoint;
                return application.ActivePlotWindow.InsertMap(oGTPaperRange);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Plotting Boundary", MessageBoxButtons.OK, MessageBoxIcon.Error); return null;
            }
        }

        private static void InsertMapFrames(ref ADODB.Recordset rsMapFrames, ref ADODB.Recordset rsGroupsDRI, PlotBoundary oPlotBoundary)
        {

            string strDatatype = "";
            IGTPlotMap oGTPlotMap = GTClassFactory.Create<IGTPlotMap>();
            IGTNamedPlot oGTNamedPlot = GTClassFactory.Create<IGTNamedPlot>();

            string sStatusBarText = application.GetStatusBarText(GTStatusPanelConstants.gtaspcMessage);
            string sStatusBarTextTemp = null;

            try
            {
                oGTNamedPlot = application.NamedPlots[oPlotBoundary.Name];

                while (!rsMapFrames.EOF)
                {
                    strDatatype = rsMapFrames.Fields["MF_DATATYPE"].Value.ToString();
                    application.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, (sStatusBarText + (" - Inserting: " + strDatatype)));
                    switch (strDatatype)
                    {
                        case "Map Frame":
                            ZoomToFitPage(oPlotBoundary.Name);
                            oGTPlotMap = InsertMapFrame(rsGroupsDRI, rsMapFrames);
                            oGTPlotMap.Frame.Name = rsMapFrames.Fields["MF_DATATYPE"].Value.ToString();
                            oPlotBoundary.MapFrameName = oGTPlotMap.Frame.Name;
                            //oGTNamedPlot.StyleMap = oPlotBoundary.StyleSubstitution;
                            if (!oPlotBoundary.Adhoc)
                            {
                                //  Load legend and zoom to Plot Boundary
                               // string LegendName = GetLegendName(GetTypesDefaultLegend(oPlotBoundary.Type, oPlotBoundary.PaperSize, oPlotBoundary.PaperOrientation, false));
                              //  LegendName = "NEPS G ";
                                //if (!string.IsNullOrEmpty(LegendName))
                                //{
                                //    oPlotBoundary.Legend = LegendName;
                                //    LoadLegend(oGTPlotMap, oPlotBoundary);
                                //}
                                //else
                                //{
                                    LoadLegend(oGTPlotMap, oPlotBoundary);
                                   // ApplyActiveLegendOverrides(oGTPlotMap, oPlotBoundary);
                               // }
                                sStatusBarTextTemp = application.GetStatusBarText(GTStatusPanelConstants.gtaspcMessage);
                                ZoomToPlotBoundary(oGTPlotMap, oPlotBoundary, true);
                                oGTPlotMap.Frame.Activate();
                                application.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, sStatusBarTextTemp);
                                ////  Legend Overrides
                                //LegendOverrides oLegendOverrides = new LegendOverrides();
                                //oLegendOverrides.ApplyLegendOverrides(oPlotBoundary);
                                oGTPlotMap.Frame.Deactivate();
                                ////  RotateMapView
                                RotateMapView(oPlotBoundary);
                                ////  InsertNorthArrow
                                if ((rsMapFrames.Fields["NORTHARROW_SYMBOLFILE"].Value != null))
                                {
                                    //oGTPlotMap.Frame.Activate();                                    
                                    //oGFramme.InsertNorthArrow(oPlotBoundary, rsMapFrames.Fields["NORTHARROW_SIZE"].Value, rsMapFrames.Fields["NORTHARROW_SYMBOLFILE"].Value);
                                    //oGTPlotMap.Frame.Deactivate();
                                }
                            }
                            else
                            {
                                // User Defined placement use active window legend if exists
                                if (oPlotBoundary.InsertActiveMapWindow)
                                {
                                    oPlotBoundary.ActiveMapWindow_LegendName = application.ActiveMapWindow.LegendName;
                                    oPlotBoundary.ActiveMapWindow_DetailID = application.ActiveMapWindow.DetailID;
                                    oPlotBoundary.ActiveMapWindow_Range = application.ActiveMapWindow.GetRange();

                                    oPlotBoundary.Legend = GetLegendName(GetTypesDefaultLegend(oPlotBoundary.Type, oPlotBoundary.PaperSize, oPlotBoundary.PaperOrientation, false));
                                    oGTPlotMap.DisplayService.ReplaceLegend(oPlotBoundary.Legend, oPlotBoundary.ActiveMapWindow_DetailID);
                                    // .DisplayService.ReplaceLegend(oPlotBoundary.ActiveMapWindow_LegendName, oPlotBoundary.ActiveMapWindow_DetailID)
                                    application.GetStatusBarText(GTStatusPanelConstants.gtaspcMessage);
                                    oGTPlotMap.Frame.Activate();
                                    oGTPlotMap.ZoomArea(oPlotBoundary.ActiveMapWindow_Range);
                                    oGTPlotMap.DisplayScale = Convert.ToDouble(oPlotBoundary.MapScale.Substring(2));
                                    oGTPlotMap.Frame.Deactivate();
                                    sStatusBarTextTemp = oPlotBoundary.MapScale.Substring(2);
                                    oGTPlotMap.Frame.Deactivate();
                                    application.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, sStatusBarTextTemp);
                                }
                            }
                            ZoomToFitPage(oPlotBoundary.Name);
                            application.RefreshWindows();
                            break;
                    }
                    rsMapFrames.MoveNext();
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Plotting Boundary", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                application.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, sStatusBarText);
            }
        }

        public static void DrawInsetBorder(PlotBoundary oPlotBoundary)
        {

            IGTPoint oPoint = GTClassFactory.Create<IGTPoint>();
            IGTPolygonGeometry oGTPolygonGeometry = GTClassFactory.Create<IGTPolygonGeometry>();
            IGTPlotRedline oGTPlotRedline = GTClassFactory.Create<IGTPlotRedline>();

            string sStatusBarText = application.GetStatusBarText(GTStatusPanelConstants.gtaspcMessage);

            try
            {

                oGTPolygonGeometry = GTClassFactory.Create<IGTPolygonGeometry>();

                {
                    oPoint.X = oPlotBoundary.SheetInset;
                    oPoint.Y = oPlotBoundary.SheetInset;
                    oPoint.Z = 0;
                }
                oGTPolygonGeometry.Points.Add(oPoint);

                {
                    oPoint.X = oPlotBoundary.SheetWidth - oPlotBoundary.SheetInset;
                    oPoint.Y = oPlotBoundary.SheetInset;
                    oPoint.Z = 0;
                }
                oGTPolygonGeometry.Points.Add(oPoint);

                {
                    oPoint.X = oPlotBoundary.SheetWidth - oPlotBoundary.SheetInset;
                    oPoint.Y = oPlotBoundary.SheetHeigh - oPlotBoundary.SheetInset;
                    oPoint.Z = 0;
                }
                oGTPolygonGeometry.Points.Add(oPoint);

                {
                    oPoint.X = oPlotBoundary.SheetInset;
                    oPoint.Y = oPlotBoundary.SheetHeigh - oPlotBoundary.SheetInset;
                    oPoint.Z = 0;
                }
                oGTPolygonGeometry.Points.Add(oPoint);



                oGTPlotRedline = application.NamedPlots[oPlotBoundary.Name].NewRedline(oGTPolygonGeometry, oPlotBoundary.SheetStyleNo);
                if( arrPlotGeom==null)
                    arrPlotGeom = new List<IGTGeometry>();

                arrPlotGeom.Add(oGTPolygonGeometry);
                if (arrPlotStyle == null)
                    arrPlotStyle = new List<object>();
                arrPlotStyle.Add(oPlotBoundary.SheetStyleNo);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Plotting Boundary", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {

            }
        }

        public static void ProcessDrawingInfoGroups(PlotBoundary oPlotBoundary, int plotCount, int totalPlot, string[] formData)
        {
            ADODB.Recordset rsGroupsDRI = null;
            ADODB.Recordset rsRedlines = null;
            ADODB.Recordset rsMapFrames = null;
            string sSql = null;
            int recordsAffected = 0;

            string sStatusBarText = application.GetStatusBarText(GTStatusPanelConstants.gtaspcMessage);

            DrawInsetBorder(oPlotBoundary);

            sSql = "SELECT * FROM APL_GROUPS_DRI WHERE DRI_ID = " + oPlotBoundary.DRI_ID + " ORDER BY GROUP_NO";
            rsGroupsDRI = application.DataContext.Execute(sSql, out recordsAffected, (int)CommandTypeEnum.adCmdText, null);
            while (!rsGroupsDRI.EOF)
            {
                sSql = "SELECT * FROM APL_MAPFRAME WHERE GROUP_NO = " + rsGroupsDRI.Fields["GROUP_NO"].Value;
                sSql = sSql + " AND MF_DATATYPE = 'Map Frame'";

                rsMapFrames = application.DataContext.Execute(sSql, out recordsAffected, (int)CommandTypeEnum.adCmdText, null);
                if (rsMapFrames.RecordCount > 0)
                {
                    InsertMapFrames(ref rsMapFrames, ref rsGroupsDRI, oPlotBoundary);
                }
                rsMapFrames = null;

                // Draw Redlines
                sSql = "SELECT * FROM APL_REDLINES WHERE GROUP_NO = " + rsGroupsDRI.Fields["GROUP_NO"].Value;
                rsRedlines = application.DataContext.Execute(sSql, out recordsAffected, (int)CommandTypeEnum.adCmdText, null);
                if (rsRedlines.RecordCount > 0)
                {
                    PopulateRedlineGroupInfo(ref rsRedlines, ref rsGroupsDRI, oPlotBoundary, plotCount, totalPlot, formData);
                }
                rsRedlines = null;

                rsGroupsDRI.MoveNext();
            }

        }

        public static void StartDrawingRedlines(PlotBoundary oPlotBoundary, int plotCount, int totalPlot, string[] formData)
        {

            ADODB.Recordset rsSheets = default(ADODB.Recordset);
            ADODB.Recordset rsDRIs = default(ADODB.Recordset);

            string strSql = null;
            int recordsAffected = 0;

            string sStatusBarText = application.GetStatusBarText(GTStatusPanelConstants.gtaspcMessage);

            try
            {

                //oPlotWindow = New PlotWindow
                strSql = "SELECT * FROM APL_SHEETS WHERE SHEET_SIZE = '" + oPlotBoundary.PaperSize + "' AND SHEET_ORIENTATION = '" + oPlotBoundary.PaperOrientation + "'";
                rsSheets = application.DataContext.Execute(strSql, out recordsAffected, (int)CommandTypeEnum.adCmdText, null);

                // Get Sheet Info
                if (rsSheets.RecordCount > 0)
                {
                    {
                        oPlotBoundary.SheetHeigh = (int)(Convert.ToInt32(rsSheets.Fields["SHEET_HEIGHT"].Value) * dSCALE);
                        oPlotBoundary.SheetWidth = (int)(Convert.ToInt32(rsSheets.Fields["SHEET_WIDTH"].Value) * dSCALE);
                        oPlotBoundary.SheetID = Convert.ToInt32(rsSheets.Fields["SHEET_ID"].Value);
                        oPlotBoundary.SheetInset = (int)(Convert.ToInt32(rsSheets.Fields["SHEET_INSET"].Value) * dSCALE);
                        oPlotBoundary.SheetStyleNo = (int)(Convert.ToInt32(rsSheets.Fields["SHEET_INSET_STYLE_NO"].Value));
                    }
                }
                else
                {
                    return;
                }
                rsSheets.Close();


                // Set Plot Window Page Size
                application.NamedPlots[oPlotBoundary.Name].PaperHeight = oPlotBoundary.SheetHeigh;
                application.NamedPlots[oPlotBoundary.Name].PaperWidth = oPlotBoundary.SheetWidth;


                // Get Title Block Insertion
                strSql = "SELECT *";
                strSql = strSql + "  FROM APL_DRAWINGINFO";
                strSql = strSql + " WHERE SHEET_ID = " + oPlotBoundary.SheetID;
                if (string.IsNullOrEmpty(oPlotBoundary.Type))
                {
                    strSql = strSql + "   AND DRI_TYPE is null";
                }
                else
                {
                    strSql = strSql + "   AND DRI_TYPE = '" + oPlotBoundary.Type + "'";
                }
                rsDRIs = application.DataContext.Execute(strSql, out recordsAffected, (int)CommandTypeEnum.adCmdText, null);

                if (rsDRIs.RecordCount > 0)
                {
                    {
                        oPlotBoundary.DRI_ID = Convert.ToInt32(rsDRIs.Fields["DRI_ID"].Value);
                    }
                }
                else
                {
                    //["No Title Block Redlines defined for selected paper size. Please ask the administrator to check the database"];
                    return;
                }
                rsDRIs.Close();



                //
                // Generate redlines
                //
                ProcessDrawingInfoGroups(oPlotBoundary, plotCount, totalPlot, formData);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Plotting Boundary", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {

            }
        }
    }
}
