using ADODB;
using System;
using System.Collections.Generic;
using System.Text;
using Intergraph.GTechnology.API;

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
        public IGTPoint TopLeft;
        public IGTPoint BottomRight;
        public string MapScale = "1000";
        public string Legend = "Engineering";
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
            string strText="";
            string strText2="";
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
                //MessageBox.Show(ex.Message);
            }
            return oAttributes;
        }

        private static string ReplaceFields(string strText, ref ADODB.Recordset rsAttributeQuery)
        {
            string strNewText = null;

            if (strText.IndexOf("[") < 0)
            {
                return strText;
            }
            if (rsAttributeQuery.RecordCount == 0)
            {
                return strText;
            }

            foreach (ADODB.Field objField in rsAttributeQuery.Fields)
            {
                {
                    if (strText.IndexOf("[" + objField.Name + "]") != 0)
                    {

                        if (!(objField.Value == System.DBNull.Value))
                        {
                            //If objField.Value <> vbNullString Then
                            strText = strText.Replace("[" + objField.Name + "]", objField.Value.ToString());
                        }
                        else
                        {
                            strText = strText.Replace("[" + objField.Name + "]", "");
                        }
                    }
                }
            }

            return strText;
        }

        private static string GetField(string strText, ref ADODB.Recordset rsAttributeQuery)
        {
            string functionReturnValue = null;
            string strNewText = null;

            functionReturnValue = "";

            if (strText.IndexOf("[") == 0)
            {
                return "";
            }
            if (rsAttributeQuery.RecordCount == 0)
            {
                return "";
            }

            foreach (Field objField in rsAttributeQuery.Fields)
            {
                {
                    if (strText.IndexOf("[" + objField.Name + "]") >= 0)
                    {
                        functionReturnValue = objField.Name;
                    }
                }
            }

            return functionReturnValue;
        }

        private static void PopulateRedlineGroupInfo(ref ADODB.Recordset rsAttributeQuery, ref ADODB.Recordset rsRedlines, ref ADODB.Recordset rsGroupsDRI, PlotBoundary oPlotBoundary)
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

                        oPolygonGeometry = GTClassFactory.Create <IGTPolygonGeometry>();

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

                        oOrientedPointGeometry = GTClassFactory.Create < IGTOrientedPointGeometry>();
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

                        if (!(strText == string.Empty))
                        {

                            // Replace database fields with values from Query.
                            strFieldName = GetField(strText, ref rsAttributeQuery);
                            strText = ReplaceFields(strText, ref rsAttributeQuery);

                            //Added by Sharon - 4 Jan 2011
                            if (rsRedlines.Fields["RL_NAME"].Value.ToString().IndexOf("ADDR") >= 0)
                            {
                                if (rsRedlines.Fields["RL_NAME"].Value.ToString().IndexOf("BAU") < 0)
                                {
                                    strText = "";
                                }
                            }

                            oGTPoint = GTClassFactory.Create<IGTPoint>();
                            oTextPointGeometry = GTClassFactory.Create < IGTTextPointGeometry>();
                            oGTVector = GTClassFactory.Create < IGTVector>();

                            oGTPoint.X = Convert.ToDouble(Convert.ToDouble(rsRedlines.Fields["RL_COORDINATE_X1"].Value)) * dSCALE + Convert.ToDouble(rsGroupsDRI.Fields["GROUP_OFFSET_X"].Value) * dSCALE;
                            oGTPoint.Y = Convert.ToDouble(rsRedlines.Fields["RL_COORDINATE_Y1"].Value) * dSCALE + Convert.ToDouble(rsGroupsDRI.Fields["GROUP_OFFSET_Y"].Value) * dSCALE;
                            oGTPoint.Z = 0;

                            {
                                oTextPointGeometry.Alignment = (GTAlignmentConstants) Convert.ToInt32(rsRedlines.Fields["RL_TEXT_ALIGNMENT"].Value);
                                // TODO: Fix Text Rotation
                                oTextPointGeometry.Normal = oGTVector;
                                oTextPointGeometry.Rotation = Convert.ToDouble(rsRedlines.Fields["RL_ROTATION"].Value);
                                oTextPointGeometry.Origin = oGTPoint;
                                //If Not strFieldName = "" And strText = "" Then
                                //  .Text = "[" & strFieldName & "]"
                                //Else
                                oTextPointGeometry.Text = strText;
                            }
                            //End If
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

                        oPolylineGeometry = GTClassFactory.Create < IGTPolylineGeometry>();

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


        private void LoadLegend(IGTPlotMap oGTPlotMap, ref ADODB.Recordset rsAttributeQuery, PlotBoundary oPlotBoundary)
        {
            long lngDetailID;
            // 
            //  Load the legend
            // 
            oGTPlotMap.DisplayService.ReplaceLegend(oPlotBoundary.Legend);
            // If oPlotBoundary.DetailID = 0 Then 'Plot Boundary is in Geo
            //   oGTPlotMap.DisplayService.ReplaceLegend("Design Legend") 'Geobase Default
            //   'oGTPlotMap.DisplayService.ReplaceLegend("Construction Legend") 'Geobase Default
            // Else
            //   lngDetailID = rsAttributeQuery.Fields["G3E_DETAILID").Value
            //   oGTPlotMap.DisplayService.ReplaceLegend("Detail Default")
            // End If
            return;
        }


        private IGTPlotMap InsertMapFrame(ref ADODB.Recordset rsGroupsDRI, ref ADODB.Recordset rsRedlines)
        {
            IGTPoint oGTPoint;
            IGTPaperRange oGTPaperRange;
            try
            {
                oGTPaperRange = GTClassFactory.Create < IGTPaperRange>();
                oGTPoint = GTClassFactory.Create<IGTPoint>();
                oGTPoint.X = ((Convert.ToDouble(rsRedlines.Fields["MF_COORDINATE_X1"].Value) * dSCALE)
                            + (Convert.ToDouble(rsGroupsDRI.Fields["GROUP_OFFSET_X"].Value) * dSCALE));
                oGTPoint.Y = ((Convert.ToDouble(rsRedlines.Fields["MF_COORDINATE_Y1"].Value) * dSCALE)
                            + (Convert.ToDouble(rsGroupsDRI.Fields["GROUP_OFFSET_Y"].Value) * dSCALE));
                oGTPoint.Z = 0;
                oGTPaperRange.TopLeft = oGTPoint;
                oGTPoint = GTClassFactory.Create<IGTPoint>();
                oGTPoint.X = ((Convert.ToDouble(rsRedlines.Fields["MF_COORDINATE_X2"].Value) * dSCALE)
                            + (Convert.ToDouble(rsGroupsDRI.Fields["GROUP_OFFSET_X"].Value) * dSCALE));
                oGTPoint.Y = ((Convert.ToDouble(rsRedlines.Fields["MF_COORDINATE_Y2"].Value) * dSCALE)
                            + (Convert.ToDouble(rsGroupsDRI.Fields["GROUP_OFFSET_Y"].Value) * dSCALE));
                oGTPoint.Z = 0;
                oGTPaperRange.BottomRight = oGTPoint;
                return application.ActivePlotWindow.InsertMap(oGTPaperRange);
            }
            catch (Exception ex)
            {
                //MsgBox(("NewPlotWindowForm.InsertMapFrame:" + ("\r\n" + ex.Message)), (vbOKOnly + vbExclamation), ex.Source);
                return null;
            }
        }

        private static void InsertMapFrames(ref ADODB.Recordset rsAttributeQuery, ref ADODB.Recordset rsMapFrames, ref ADODB.Recordset rsGroupsDRI, PlotBoundary oPlotBoundary)
        {

            string strDatatype = "";
            IGTPlotMap oGTPlotMap = GTClassFactory.Create <IGTPlotMap>();
            IGTNamedPlot oGTNamedPlot = GTClassFactory.Create <IGTNamedPlot>();

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
                            break;
                    }
                    rsMapFrames.MoveNext();
                }

            }
            catch (Exception ex)
            {
            }
            finally
            {
                application.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, sStatusBarText);
            }
        }

        public static void DrawInsetBorder(PlotBoundary oPlotBoundary)
        {

            IGTPoint oPoint = GTClassFactory.Create<IGTPoint>();
            IGTPolygonGeometry oGTPolygonGeometry = GTClassFactory.Create <IGTPolygonGeometry>();
            IGTPlotRedline oGTPlotRedline = GTClassFactory.Create<IGTPlotRedline>();

            string sStatusBarText = application.GetStatusBarText(GTStatusPanelConstants.gtaspcMessage);

            try
            {

                oGTPolygonGeometry = GTClassFactory.Create < IGTPolygonGeometry>();

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

                arrPlotGeom.Add(oGTPolygonGeometry);
                arrPlotStyle.Add(oPlotBoundary.SheetStyleNo);
            }
            catch (Exception ex)
            {
            }
            finally
            {

            }
        }

        public static void ProcessDrawingInfoGroups(ref ADODB.Recordset rsAttributeQuery, PlotBoundary oPlotBoundary)
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
                sSql = sSql + " AND MF_DATATYPE != 'Map Frame'";

                rsMapFrames = application.DataContext.Execute(sSql, out recordsAffected, (int)CommandTypeEnum.adCmdText, null);
                if (rsMapFrames.RecordCount > 0)
                {
                    InsertMapFrames(ref rsAttributeQuery, ref rsMapFrames, ref rsGroupsDRI, oPlotBoundary);
                }
                rsMapFrames = null;

                // Draw Redlines
                sSql = "SELECT * FROM APL_REDLINES WHERE GROUP_NO = " + rsGroupsDRI.Fields["GROUP_NO"].Value;
                rsRedlines = application.DataContext.Execute(sSql, out recordsAffected, (int)CommandTypeEnum.adCmdText, null);
                if (rsRedlines.RecordCount > 0)
                {
                    PopulateRedlineGroupInfo(ref rsAttributeQuery, ref rsRedlines, ref rsGroupsDRI, oPlotBoundary);
                }
                rsRedlines = null;

                rsGroupsDRI.MoveNext();
            }

        }

        public static void StartDrawingRedlines(PlotBoundary oPlotBoundary)
        {

            ADODB.Recordset rsSheets = default(ADODB.Recordset);
            ADODB.Recordset rsDRIs = default(ADODB.Recordset);
            ADODB.Recordset rsAttributeQuery = default(ADODB.Recordset);

            string strSql = null;
            int recordsAffected = 0;

            string sStatusBarText = application.GetStatusBarText(GTStatusPanelConstants.gtaspcMessage);

            try
            {

                //oPlotWindow = New PlotWindow
                strSql="SELECT * FROM APL_SHEETS WHERE SHEET_SIZE = '" + oPlotBoundary.PaperSize + "' AND SHEET_ORIENTATION = '" + oPlotBoundary.PaperOrientation + "'";
                rsSheets = application.DataContext.Execute(strSql, out recordsAffected, (int)CommandTypeEnum.adCmdText, null);

                // Get Sheet Info
                if (rsSheets.RecordCount > 0)
                {
                    {
                        oPlotBoundary.SheetHeigh = (int) (Convert.ToInt32(rsSheets.Fields["SHEET_HEIGHT"].Value) * dSCALE);
                        oPlotBoundary.SheetWidth = (int) (Convert.ToInt32(rsSheets.Fields["SHEET_WIDTH"].Value) * dSCALE);
                        oPlotBoundary.SheetID = Convert.ToInt32(rsSheets.Fields["SHEET_ID"].Value);
                        oPlotBoundary.SheetInset = (int) (Convert.ToInt32(rsSheets.Fields["SHEET_INSET"].Value) * dSCALE);
                        oPlotBoundary.SheetStyleNo = (int) (Convert.ToInt32(rsSheets.Fields["SHEET_INSET_STYLE_NO"].Value));
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


                //
                // Original querry
                //

                // ADHOC Template vs generated from Plot Boundary
                if (oPlotBoundary.FNO == 0)
                {
                    oPlotBoundary.Attributes = new List<FieldAttribute>();
                    //oPlotBoundary.FNO = 1500;
                    //oPlotBoundary.Attributes = GetAttributes(oPlotBoundary);
                    strSql = "SELECT ";
                    foreach (FieldAttribute oAttribute in oPlotBoundary.Attributes)
                    {
                        if ("G3E_FNO,G3E_FID,G3E_CNO,G3E_CID,G3E_DETAILID".IndexOf(oAttribute.G3E_FIELD) < 0)
                        strSql = strSql + (string.IsNullOrEmpty(oAttribute.VALUE.Trim()) ? "NULL" : "'" + oAttribute.VALUE + "'") + " " + oAttribute.G3E_FIELD + ", ";
                    }

                    strSql = strSql + "NULL G3E_FNO, ";
                    strSql = strSql + "NULL G3E_FID, ";
                    strSql = strSql + "NULL G3E_CNO, ";
                    strSql = strSql + "NULL G3E_CID, ";
                    strSql = strSql + "NULL G3E_DETAILID ";
                    //strSql = strSql & IIf(Trim(cmbPaperSize2) = "", ", NULL", ", '" + oPlotBoundary.PaperSize + "'") & " PLOT_SIZE "
                    //strSql = strSql & IIf(Trim(cmbMapScale2) = "", ", NULL", ", '" + oPlotBoundary.MapScale + "'") & " PLOT_SCALE "
                    //strSql = strSql & IIf(Trim(cmbPaperOrientation2) = "", ", NULL", ", '" + oPlotBoundary.PaperOrientation + "'") & " PLOT_ORIENTATION "
                    strSql = strSql + "FROM DUAL";
                }
                else
                {
                    // TODO - Build select base on the components that make up the feature identified in the G3E_FEATURECOMPONENT.
                    //to_char(sysdate, 'Dy DD-Mon-YYYY HH24:MI:SS')
                    //strSql = "SELECT w.*, n.*, j.*, r.*, to_char(sysdate, 'YYYY/MM/DD') \"SYSDATE\" " + 
                    //    " FROM " + strPLOT_BOUNDARY_TABLE + " w, " + strPLOT_BOUNDARY_COMMON_TABLE + " n, G3E_JOB j, " + strREF_EXCH_TABLE + " r, dual " + 
                    //    " WHERE j." + strG3E_JOB_CAPITAL_WORK_ORDER_NUMBER_COLUMN + " = '" + cbPlotBoundaryFilterCapitalWorkOrderNumber.Text + "'" + 
                    //    " AND w." + strPLOT_BOUNDARY_NAME_COLUMN + " = '" + oPlotBoundary.Name + "'" + 
                    //    " AND n.G3E_FNO=" + lPLOT_BOUNDARY_G3E_FNO + 
                    //    " AND n." + strPLOT_BOUNDARY_COMMON_JOB_ID_COLUMN + " = j.G3E_IDENTIFIER" + 
                    //    " AND w.G3E_FID = n.G3E_FID" + 
                    //    " AND n." + strPLOT_BOUNDARY_COMMON_SWITCH_CENTRE_CLLI + " = r." + strREF_EXCH_SWITCH_CENTRE_CLLI;
                }
                rsAttributeQuery = application.DataContext.Execute(strSql, out recordsAffected, (int)CommandTypeEnum.adCmdText, null);
                application.NamedPlots[oPlotBoundary.Name].FieldsQuery = rsAttributeQuery.Source.ToString();


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
                ProcessDrawingInfoGroups(ref rsAttributeQuery, oPlotBoundary);
            }
            catch (Exception ex)
            {

            }
            finally
            {

            }
        }
    }
}
