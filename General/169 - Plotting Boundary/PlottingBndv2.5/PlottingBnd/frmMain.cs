using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
//using System.Drawing .Graphics;

using System.Runtime.InteropServices;
using ADODB;
using Intergraph.GTechnology.API;
using AG.GTechnology.Utilities;

namespace AG.GTechnology.PlottingBnd
{
    public partial class frmMain : Form
    {
        private static Intergraph.GTechnology.API.IGTApplication application = GTClassFactory.Create<IGTApplication>();
        [DllImport("Kernel32.dll")]
        internal static extern int GetUserDefaultLCID();

        bool m_bCancel;
        int m_vListIndex;
        string m_tbMapScaleCustomText;

        public int m_iDRI_ID;
        public string m_sType;
        public string m_sMapScale;
        public string m_sMapScaleCustom;
        public string m_sMapScalePreDefined;
        public int m_lMapScale;
        public string m_sSheetName;
        public string m_sSheetSize;
        public string m_sSheetOrientation;
        public int m_lSheetId;
        public double m_dSheetHeight;
        public double m_dSheetWidth;
        public double m_dSheet_Inset;
        public double m_dCaptionStampTLX;
        public double m_dCaptionStampTLY;

        public double m_dMapTLX;
        public double m_dMapTLY;
        public double m_dMapBRX;
        public double m_dMapBRY;

        public double m_dMapHeight;
        public double m_dMapWidth;

        public IGTPoint m_oMapTLPoint;
        public IGTPoint m_oMapBRPoint;


        private bool m_bPlaceComponment = false;
        private double m_dMapHeightScaled;
        private double m_dMapWidthScaled;
        public bool load_first_time = true;

        public bool m_rbExtPlot = false;
        public bool m_rbNewPlot = false;
        public string m_JobID = null;
        public string m_PlotType = null;

        public double MapHeightScaled
        {
            get
            {
                return m_dMapHeightScaled;
            }
            set
            {
                m_dMapHeightScaled = value;
            }
        }


        public double MapWidthScaled
        {
            get
            {
                return m_dMapWidthScaled;
            }
            set
            {
                m_dMapWidthScaled = value;
            }
        }

        public bool PlaceComponent
        {
            get
            {
                return m_bPlaceComponment;
            }
            set
            {
                m_bPlaceComponment = value;
            }
        }

        public string Desc1
        {
            get
            {
                return txtPoject1.Text;
            }
        }
        public string Desc2
        {
            get
            {
                return txtPoject2.Text;
            }
        }
        public string Desc3
        {
            get
            {
                return txtPoject3.Text;
            }
        }
        public string Desc4
        {
            get
            {
                return txtPoject4.Text;
            }
        }
        public string PrepareBy
        {
            get
            {
                return txtPreparedBy.Text;
            }
        }
        public string RevisedBy
        {
            get
            {
                return txtRevisedBy.Text;
            }
        }
        public string ApprovedBy
        {
            get
            {
                return txtApproved.Text;
            }
        }

        private bool m_bCreatePlot = false;
        public bool CreatePlot
        {
            get
            {
                return m_bCreatePlot;
            }
            set
            {
                m_bCreatePlot = value;
                btnCreatePlotWindows.Enabled = m_bCreatePlot;
            }
        }

        public frmMain()
        {
            InitializeComponent();
            PlaceComponent = false;

        }
                
        public string GetUsername(string sComponentName)
        {
            Recordset rsComp = null;
            int recordsAffected = 0;

            try
            {
                string Sql = " SELECT g3e_username";
                Sql = (Sql + "   FROM g3e_componentinfo_optlang");
                Sql = (Sql + ("  WHERE g3e_name = \'"
                            + (sComponentName + ("\' AND g3e_lcid = \'"
                            + (GetLanguage() + "\'")))));
                rsComp = application.DataContext.Execute(Sql, out recordsAffected, (int)CommandTypeEnum.adCmdText, null);

                if (rsComp != null)
                {
                    if (!rsComp.EOF)
                    {
                        if (rsComp.Fields["g3e_username"].Value != DBNull.Value) return rsComp.Fields["g3e_username"].Value.ToString();
                    }
                }
                rsComp = null;

                return string.Empty;
            }
            catch (Exception ex)
            {
                MessageBox.Show("PlotBoundaryForm.GetUsername:\n" + ex.Message);
                return string.Empty;
            }
        }

        public ADODB.Recordset GetRedlines(int iDRI_ID)
        {
            string Sql;
            object vParam = null;
            int recordsAffected = 0;
            try
            {
                Sql = " SELECT g.GROUP_NO, g.GROUP_NAME, DRI_ID, USERPLACE, GROUP_OFFSET_X, GROUP_OFFSET_Y, RL_NO, RL_DATATYPE, RL_COORDINATE_X1, RL_COORDINATE_Y1, RL_COORDINATE_X2, RL_COORDINATE_Y2, RL_STYLE_NUMBER, RL_TEXT_ALIGNMENT, RL_ROTATION,    RL_TEXT, RL_USERINPUT, RL_NAME, RL_TEXT_FR ";
                Sql = (Sql + "     FROM apl_groups_dri g, apl_redlines r");
                Sql = (Sql + ("    WHERE g.dri_id = \'"
                            + (iDRI_ID + "\'")));
                Sql = (Sql + "      AND g.group_no = r.group_no");
                Sql = (Sql + "      AND rl_datatype = \'Redline Lines\'");
                return application.DataContext.Execute(Sql, out recordsAffected, (int)CommandTypeEnum.adCmdText, vParam);
                //Repos(GetRedlines);
            }
            catch (Exception ex)
            {
                MessageBox.Show(("PlotBoundaryForm.GetRedlines:" + ("\r\n" + ex.Message)), ex.Source, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return null;
            }
        }

        public ADODB.Recordset GetTypes()
        {
            string strSql;
            ADODB.Recordset rsTypes;
            object vParam = null;
            int recordsAffected = 0;
            try
            {
                strSql = " SELECT DISTINCT dri_type ";
                strSql = (strSql + "           FROM apl_drawinginfo ");
                strSql = (strSql + "          WHERE dri_type IS NOT NULL ");
                strSql = (strSql + "       ORDER BY dri_type ");
                return application.DataContext.Execute(strSql, out recordsAffected, (int)CommandTypeEnum.adCmdText, vParam);
                //Repos(rsTypes);
            }
            catch (Exception ex)
            {
                MessageBox.Show(("PlotBoundaryForm.GetTypes:" + ("\r\n" + ex.Message)), ex.Source, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return null;
            }
        }

        public ADODB.Recordset GetScales()
        {
            string strSql;
            ADODB.Recordset rsTypes;
            object vParam = null;
            int recordsAffected = 0;
            try
            {
                strSql = " SELECT DISTINCT scale ";
                strSql = (strSql + "           FROM APL_PLOT_SCALE ");
                strSql = (strSql + "          WHERE scale IS NOT NULL ");
                strSql = (strSql + "       ORDER BY scale ");
                return application.DataContext.Execute(strSql, out recordsAffected, (int)CommandTypeEnum.adCmdText, vParam);
                //Repos(rsTypes);
            }
            catch (Exception ex)
            {
                MessageBox.Show(("PlotBoundaryForm.GetScales:" + ("\r\n" + ex.Message)), ex.Source, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return null;
            }
        }

        public void GetMapLocation(int iDRI_ID, ref IGTPoint oMapTLPoint, ref IGTPoint oMapBRPoint)
        {
            string Sql;
            ADODB.Recordset rsMapSize;
            object vParam = null;
            int recordsAffected = 0;
            try
            {
                Sql = " SELECT   g.dri_id, g.group_no, g.group_offset_x, g.group_offset_y, mf_coordinate_x1, mf_coordinate_y1, mf_coordinate_x2, mf_coordinate_y" +
                "2";
                Sql = (Sql + "     FROM apl_groups_dri g, apl_mapframe r");
                Sql = (Sql + ("    WHERE g.dri_id = \'"
                            + (iDRI_ID + "\'")));
                Sql = (Sql + "      AND g.group_no = r.group_no");
                Sql = (Sql + "      AND mf_datatype = \'Map Frame\'");
                Sql = (Sql + " ORDER BY group_no");
                rsMapSize = application.DataContext.Execute(Sql, out recordsAffected, (int)CommandTypeEnum.adCmdText, vParam);
                //Repos(rsMapSize);

                if (rsMapSize.EOF) return;
                //oMapTLPoint = GTClassFactory.Create<IGTPoint>();
                //oMapTLPoint.X = Convert.ToDouble(rsMapSize.Fields["MF_COORDINATE_X1"].Value);
                //oMapTLPoint.Y = Convert.ToDouble(rsMapSize.Fields["MF_COORDINATE_Y1"].Value);
                //oMapBRPoint = GTClassFactory.Create<IGTPoint>();
                //oMapBRPoint.X = Convert.ToDouble(rsMapSize.Fields["MF_COORDINATE_X2"].Value);
                //oMapBRPoint.Y = Convert.ToDouble(rsMapSize.Fields["MF_COORDINATE_Y2"].Value);
                
                // Change using group_offset - Phuong
                oMapTLPoint = GTClassFactory.Create<IGTPoint>();
                oMapTLPoint.X = Convert.ToDouble(rsMapSize.Fields["MF_COORDINATE_X1"].Value) + Convert.ToDouble(rsMapSize.Fields["GROUP_OFFSET_X"].Value);
                oMapTLPoint.Y = Convert.ToDouble(rsMapSize.Fields["MF_COORDINATE_Y1"].Value) + Convert.ToDouble(rsMapSize.Fields["GROUP_OFFSET_Y"].Value);
                oMapBRPoint = GTClassFactory.Create<IGTPoint>();
                oMapBRPoint.X = Convert.ToDouble(rsMapSize.Fields["MF_COORDINATE_X2"].Value) + Convert.ToDouble(rsMapSize.Fields["GROUP_OFFSET_X"].Value);
                oMapBRPoint.Y = Convert.ToDouble(rsMapSize.Fields["MF_COORDINATE_Y2"].Value) + Convert.ToDouble(rsMapSize.Fields["GROUP_OFFSET_Y"].Value);
                rsMapSize.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(("PlotBoundaryForm.GetMapLocation:" + ("\r\n" + ex.Message)), ex.Source, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
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
                MessageBox.Show(("PlotBoundaryForm.GetMapSize:" + ("\r\n" + ex.Message)), ex.Source, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        public void GetSheetSize(string sType, string sSheetName, string sSheetOrientation, ref int lSheetId, ref double dSheetHeight, ref double dSheetWidth, ref double dSheetInset, ref int iDRI_ID)
        {
            string strSql;
            Recordset rsSheetSize;
            object vParam = null;
            int recordsAffected = 0;
            try
            {
                strSql = "SELECT s.SHEET_ID, s.SHEET_NAME, s.SHEET_HEIGHT, s.SHEET_WIDTH, s.SHEET_SIZE, s.SHEET_ORIENTATION, s.SHEET_INSET, s.SHEET_INSET_STYLE_NO, d.DRI_ID, d.DRI_TYPE, d.DRI_NAME, d.MODULE ";
                strSql = (strSql + "  FROM apl_sheets s, apl_drawinginfo d ");
                strSql = (strSql + " WHERE s.sheet_id = d.sheet_id ");
                if ((sType == ""))
                {
                    strSql = (strSql + "   AND d.dri_type is null");
                }
                else
                {
                    strSql = (strSql + ("   AND d.dri_type = \'"
                                + (sType + "\'")));
                }
                strSql = (strSql + ("   AND s.sheet_name = \'"
                            + (sSheetName + "\'")));
                strSql = (strSql + ("   AND s.sheet_orientation = \'"
                            + (sSheetOrientation + "\'")));

                rsSheetSize = application.DataContext.Execute(strSql, out recordsAffected, (int)CommandTypeEnum.adCmdText, vParam);
                //Repos(rsSheetSize);
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
                MessageBox.Show(("PlotBoundaryForm.GetSheetSize:" + ("\r\n" + ex.Message)), ex.Source, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        public ADODB.Recordset GetPaperSizes()
        {
            string strSql;
            ADODB.Recordset rsPaperSizes;
            object vParam = null;
            int recordsAffected = 0;
            try
            {
                strSql = "select distinct sheet_size, sheet_name from apl_sheets order by to_number(substr(sheet_size,1,2))";
                rsPaperSizes = application.DataContext.Execute(strSql, out recordsAffected, (int)CommandTypeEnum.adCmdText, vParam);
                return rsPaperSizes;
                //Repos(rsPaperSizes);
                //GetPaperSizes = rsPaperSizes;
            }
            catch (Exception ex)
            {
                MessageBox.Show(("PlotBoundaryForm.GetPaperSizes:" + ("\r\n" + ex.Message)), ex.Source, MessageBoxButtons.OK, MessageBoxIcon.Exclamation); 
                return null;
            }
        }

        public void Repos(ref ADODB.Recordset rs)
        {
            // TODO: On Error Resume Next Warning!!!: The statement is not translatable 
            rs.MoveLast();
            rs.MoveFirst();
            // TODO: On Error GoTo 0 Warning!!!: The statement is not translatable 
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
                MessageBox.Show(("PlotBoundaryForm.GetLanguage:" + ("\r\n" + ex.Message)), ex.Source, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return string.Empty;
            }
        }

        private bool ValidScale(string sScale)
        {
            // Dim m_sMapScale As Object
            try
            {
                if ((sScale.Length < 3)) return false;
                if (!(sScale.Substring(0, 1) == "1")) return false;
                if (!(sScale.Substring(1, 1) == ":")) return false;
                if ((int.Parse(sScale.Substring(2, sScale.Length - 2)) == 0)) return false;
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(("PlotBoundaryForm.ValidScale:" + ("\r\n" + ex.Message)), ex.Source, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return false;
            }
        }

        // 
        //  When the WPB changes update the image page
        // 
        private void WPB_Change()
        {
            try
            {
                // If the SheetName or SheetOrientation is not set exit
                if ((string.IsNullOrEmpty(m_sSheetName)
                            || (string.IsNullOrEmpty(m_sSheetOrientation)
                            || string.IsNullOrEmpty(m_sMapScale))))
                {
                    return;
                }
                if (!ValidScale(m_sMapScale))
                {
                    // tbMapScaleCustom.Font.Bold = True
                    tbMapScaleCustom.ForeColor = Color.Red;
                    //  System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Red)
                    return;
                }
                // tbMapScaleCustom.Font.Bold = False
                tbMapScaleCustom.ForeColor = System.Drawing.SystemColors.WindowText;
                //  System.Drawing.ColorTranslator.ToOle(System.Drawing.SystemColors.WindowText)
                // 
                //  Get info
                // 
                // Get the Sheet info
                GetSheetSize(m_sType, m_sSheetName, m_sSheetOrientation, ref m_lSheetId, ref m_dSheetHeight, ref m_dSheetWidth, ref m_dSheet_Inset, ref m_iDRI_ID);
                // Get the Map Size from Map Frame table
                GetMapSize(m_iDRI_ID, ref m_dMapHeight, ref m_dMapWidth);
                // Get the Map Location from Map Frame table
                GetMapLocation(m_iDRI_ID, ref m_oMapTLPoint, ref m_oMapBRPoint);
                //  Get Map Scale & Scaled Map Size
                m_lMapScale = int.Parse(m_sMapScale.Substring(2, m_sMapScale.Length - 2));
                m_dMapHeightScaled = (m_dMapHeight
                            * ((double)m_lMapScale / 1000));
                m_dMapWidthScaled = (m_dMapWidth
                            * ((double)m_lMapScale / 1000));
                if ((optPaperOrientationLandbase.Checked == true))
                {
                    lblSetPaperValue.Text = (m_sSheetSize + ("  ("
                                + (optPaperOrientationLandbase.Text + ")")));
                }
                else
                {
                    lblSetPaperValue.Text = (m_sSheetSize + ("  ("
                                + (optPaperOrientationPortrait.Text + ")")));
                }
                lblSetBorderInsetValue.Text = (m_dSheet_Inset + "mm");
                lblSetMapSizeValue.Text = (Convert.ToString(System.Math.Round(m_dMapWidthScaled, 1)) + ("m x "
                            + (Convert.ToString(System.Math.Round(m_dMapHeightScaled, 1)) + "m"))).Trim();
                lblSetMapScaleValue.Text = m_sMapScale;
                // 
                //  Update the images (   N O   L O N G E R   U S E D   )
                //  imgPageContainer_Paint used instead
                // 
                // Dim dPgContainerScale As Double
                // If m_sSheetOrientation = "Landscape" Then
                //   dPgContainerScale = (imgPageContainer.Width / m_dSheetWidth) * 0.8
                // Else
                //   dPgContainerScale = (imgPageContainer.Height / m_dSheetHeight) * 0.9
                // End If
                // 'dPgContainerScale = 1
                // imgPaper.Hide()
                // imgPaper.Height = m_dSheetHeight * dPgContainerScale
                // imgPaper.Width = m_dSheetWidth * dPgContainerScale
                // imgPaper.Top = imgPageContainer.Top + ((imgPageContainer.Height - imgPaper.Height) / 2)
                // imgPaper.Left = imgPageContainer.Left + ((imgPageContainer.Width - imgPaper.Width) / 2)
                // imgMapWindow.Hide()
                // imgMapWindow.Height = m_dMapHeight * dPgContainerScale
                // imgMapWindow.Width = m_dMapWidth * dPgContainerScale
                // imgMapWindow.Top = imgPaper.Top + (m_dSheet_Inset * dPgContainerScale)
                // imgMapWindow.Left = imgPaper.Left + (m_dSheet_Inset * dPgContainerScale)
                // ' TODO change to use actual map BL/TR from APL_MapFrame table
                // imgBorder.Hide()
                // 'imgBorder.Height = imgPaper.Height - ((m_dSheet_Inset * 2) * dPgContainerScale)
                // 'imgBorder.Width = imgPaper.Width - ((m_dSheet_Inset * 2) * dPgContainerScale)
                // 'imgBorder.Top = imgPaper.Top + (m_dSheet_Inset * dPgContainerScale)
                // 'imgBorder.Left = imgPaper.Left + (m_dSheet_Inset * dPgContainerScale)
                //  '' NO LONGER USED
                //  ''Removed 5% padding now that we can automate the fitting the WPB.
                // imgWPB.Hide()
                // 'imgWPB.Visible = False
                //  ''imgWPB.Height = imgMapWindow.Height '* 0.95 'Removed 5% padding now that we can automate the fitting the WPB.
                //  ''imgWPB.Width = imgMapWindow.Width '* 0.95
                //  ''imgWPB.Top = imgMapWindow.Top + ((imgMapWindow.Height - imgWPBold.Height) / 2)
                //  ''imgWPB.Left = imgMapWindow.Left + ((imgMapWindow.Width - imgWPBold.Width) / 2)
                //  '' Check if [drawing info/caption stamp/title block] is to be used
                // imgCaption.Hide()
                // 'imgCaption.Visible = cbType.Checked
                // 'imgCaption.Top = imgPaper.Top + (m_dCaptionStampTLY * dPgContainerScale)
                // 'imgCaption.Left = imgPaper.Left + (m_dCaptionStampTLX * dPgContainerScale)
                // 'imgCaption.Height = (imgBorder.Top + imgBorder.Height) - imgCaption.Top
                // 'imgCaption.Width = (imgBorder.Left + imgBorder.Width) - imgCaption.Left
                btnPlace.Enabled = true;
                imgPageContainer.Refresh();
            }
            catch (Exception ex)
            {
                MessageBox.Show(("PlotBoundaryForm.WPB_Change:" + ("\r\n" + ex.Message)), ex.Source, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void imgPageContainer_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
        {
            try
            {
                // ' Create a local version of the graphics object for the PictureBox.
                Graphics g = e.Graphics;
                System.Drawing.PointF oPoint1 = new PointF();
                System.Drawing.PointF oPoint2 = new PointF();
                g.Clear(Color.FromKnownColor(KnownColor.Highlight));
                //  Was using RGB=0,192,192 or DeepSkyBlue or CadetBlue
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                g.PageUnit = GraphicsUnit.Pixel;

                if (m_iDRI_ID == 0)
                {
                    g.DrawString("NO TEMPLATE", new Font("Arial", 20, FontStyle.Bold),
                        Brushes.White, new PointF(imgPageContainer.Width / 4, imgPageContainer.Height / 2));
                    btnPlace.Enabled = false;
                    return;
                }

                double dPgContainerScale;
                if ((m_sSheetOrientation == "Landscape"))
                {
                    dPgContainerScale = ((imgPageContainer.Width / m_dSheetWidth)
                                * 0.9);
                }
                else
                {
                    dPgContainerScale = ((imgPageContainer.Height / m_dSheetHeight)
                                * 0.9);
                }
                // g.PageScale = dPgContainerScale
                System.Drawing.Rectangle oRectPaper;
                System.Drawing.Rectangle oRectBorder;
                System.Drawing.Rectangle oRectMapWindow;
                //  Draw Page
                oRectPaper = new System.Drawing.Rectangle();
                oRectPaper.X = (int)((imgPageContainer.Width
                            - (m_dSheetWidth * dPgContainerScale))
                            / 2);
                oRectPaper.Y = (int)((imgPageContainer.Height
                            - (m_dSheetHeight * dPgContainerScale))
                            / 2);
                oRectPaper.Height = (int)(m_dSheetHeight * dPgContainerScale);
                oRectPaper.Width = (int)(m_dSheetWidth * dPgContainerScale);
                g.DrawRectangle(Pens.Black, oRectPaper);
                g.FillRectangle(Brushes.White, oRectPaper);
                //  Draw Border
                oRectBorder = new System.Drawing.Rectangle();
                oRectBorder.X = (int)(oRectPaper.X
                            + (m_dSheet_Inset * dPgContainerScale));
                oRectBorder.Y = (int)(oRectPaper.Y
                            + (m_dSheet_Inset * dPgContainerScale));
                oRectBorder.Height = (int)(oRectPaper.Height
                            - ((m_dSheet_Inset * 2)
                            * dPgContainerScale));
                oRectBorder.Width = (int)(oRectPaper.Width
                            - ((m_dSheet_Inset * 2)
                            * dPgContainerScale));
                g.DrawRectangle(Pens.Black, oRectBorder);
                //  Draw MapWindow
                oRectMapWindow = new System.Drawing.Rectangle();
                //  Old - hard coded to start map window at TL _ inset... no good
                // oRectMapWindow.X = oRectPaper.X + (m_dSheet_Inset * dPgContainerScale)
                // oRectMapWindow.Y = oRectPaper.Y + (m_dSheet_Inset * dPgContainerScale)
                // oRectMapWindow.Height = m_dMapHeight * dPgContainerScale
                // oRectMapWindow.Width = m_dMapWidth * dPgContainerScale
                oRectMapWindow.X = (int)(oRectPaper.X
                            + (m_oMapTLPoint.X * dPgContainerScale));
                oRectMapWindow.Y = (int)(oRectPaper.Y
                            + (m_oMapTLPoint.Y * dPgContainerScale));
                oRectMapWindow.Height = (int)((m_oMapBRPoint.Y - m_oMapTLPoint.Y)
                            * dPgContainerScale);
                oRectMapWindow.Width = (int)((m_oMapBRPoint.X - m_oMapTLPoint.X)
                            * dPgContainerScale);
                g.DrawImage(ImageList1.Images[0], oRectMapWindow);
                g.DrawRectangle(Pens.Black, oRectMapWindow);
                //  Draw the border a little thicker
                oRectBorder.Inflate((int)((0.25 * dPgContainerScale)
                                * -1), (int)((0.25 * dPgContainerScale)
                                * -1));
                g.DrawRectangle(Pens.Black, oRectBorder);
                oRectBorder.Inflate((int)(0.5 * dPgContainerScale), (int)(0.5 * dPgContainerScale));
                g.DrawRectangle(Pens.Black, oRectBorder);
                ADODB.Recordset rsRedlines;
                rsRedlines = GetRedlines(m_iDRI_ID);
                while (!rsRedlines.EOF)
                {
                    oPoint1.X = (float)(oRectPaper.X
                                + ((Convert.ToInt32(rsRedlines.Fields["GROUP_OFFSET_X"].Value) + Convert.ToInt32(rsRedlines.Fields["RL_COORDINATE_X1"].Value))
                                * dPgContainerScale));
                    oPoint1.Y = (float)(oRectPaper.Y
                                + ((Convert.ToInt32(rsRedlines.Fields["GROUP_OFFSET_Y"].Value) + Convert.ToInt32(rsRedlines.Fields["RL_COORDINATE_Y1"].Value))
                                * dPgContainerScale));
                    oPoint2.X = (float)(oRectPaper.X
                                + ((Convert.ToInt32(rsRedlines.Fields["GROUP_OFFSET_X"].Value) + Convert.ToInt32(rsRedlines.Fields["RL_COORDINATE_X2"].Value))
                                * dPgContainerScale));
                    oPoint2.Y = (float)(oRectPaper.Y
                                + ((Convert.ToInt32(rsRedlines.Fields["GROUP_OFFSET_Y"].Value) + Convert.ToInt32(rsRedlines.Fields["RL_COORDINATE_Y2"].Value))
                                * dPgContainerScale));
                    g.DrawLine(Pens.Black, oPoint1, oPoint2);
                    rsRedlines.MoveNext();
                }
                rsRedlines.Close();
                g.Flush();
                // ' FUTURE
                // ' Draw a string on the PictureBox.
                // g.DrawString("This is a diagonal line drawn on the control", _
                //     New Font("Arial", 10), Brushes.Red, New PointF(0.0F, 0.0F))
                // ' Draw a line in the PictureBox.
                // g.DrawLine(System.Drawing.Pens.Red, imgPaper.Left, _
                //     imgPaper.Top, imgPaper.Right, imgPaper.Bottom)
            }
            catch (Exception ex)
            {
                MessageBox.Show(("PlotBoundaryForm.imgPageContainer_Paint:" + ("\r\n" + ex.Message)), ex.Source, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            ADODB.Recordset rsPaperSizes;
            ADODB.Recordset rsTypes;
            ADODB.Recordset rsScales;
            try
            {
                if (load_first_time)
                {
                    //Vinod Apr-2013
                    lblPPWO.Text = application.DataContext.ActiveJob;

                    cmb_PlotType.Items.Add("Civil");
                    cmb_PlotType.Items.Add("Copper ESIDE");
                    cmb_PlotType.Items.Add("Copper DSIDE");
                    cmb_PlotType.Items.Add("Fibre");
                    cmb_PlotType.Items.Add("HSBB DSIDE");

                    load_first_time = false;
                    if (tabPlotTemplate.TabPages.Count > 1) tabPlotTemplate.TabPages.RemoveAt(1);
                    //       AddHandler imgPaper.Paint, AddressOf imgPaper_Paint
                    // Populate Type listbox
                    rsTypes = GetTypes();
                    lbType.Items.Clear();
                    while (!rsTypes.EOF)
                    {
                        lbType.Items.Add(rsTypes.Fields["DRI_TYPE"].Value);
                        rsTypes.MoveNext();
                    }
                    rsTypes.Close();
                    // Populate PaperSize listbox
                    rsPaperSizes = GetPaperSizes();
                    lbPaperSize.Items.Clear();
                    while (!rsPaperSizes.EOF)
                    {
                        lbPaperSize.Items.Add(("("
                                        + (rsPaperSizes.Fields["SHEET_NAME"].Value + (") " + rsPaperSizes.Fields["SHEET_SIZE"].Value))));
                        rsPaperSizes.MoveNext();
                    }
                    rsPaperSizes.Close();
                    // Populate MapScalePreDefined listbox
                    //lbMapScalePreDefined.Items.Clear();
                    //lbMapScalePreDefined.Items.Add("1:250");
                    //lbMapScalePreDefined.Items.Add("1:350");
                    //lbMapScalePreDefined.Items.Add("1:500");

                    rsScales = GetScales();
                    lbMapScalePreDefined.Items.Clear();
                    while (!rsScales.EOF)
                    {
                        lbMapScalePreDefined.Items.Add(("1:"
                                        + (rsScales.Fields["SCALE"].Value)));
                        rsScales.MoveNext();
                    }
                    rsScales.Close();
                    // Populate MapScaleCustom textbox
                    tbMapScaleCustom.Text = "1:1000";
                    if (lbPaperSize.Items.Count > 0) lbPaperSize.SelectedIndex = 0;
                    m_sSheetSize = lbPaperSize.Text.Substring(lbPaperSize.Text.IndexOf(')') + 1, lbPaperSize.Text.Length - (lbPaperSize.Text.IndexOf(')') + 1)).Trim();
                    m_sMapScaleCustom = "1:250";
                    m_sMapScale = m_sMapScaleCustom;
                    m_sSheetOrientation = "Landscape";
                    optPaperOrientationLandbase.Enabled = true;
                    lbType.SetSelected(0, true);
                    lbPaperSize.SetSelected(0, true);
                    lbMapScalePreDefined.SetSelected(0, true);
                }
                if (btnCreatePlotWindows.Enabled)
                {
                    fillingTemplate();
                }
                
            }
            catch (Exception ex)
            {
                MessageBox.Show(("PlotBoundaryForm.New:" + ("\r\n" + ex.Message)), ex.Source, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            finally
            {
                // cleaning
            }
        }

        private void fillingTemplate()
        {
            if( application.DataContext.ActiveJob!=null)
            {
            //txtExchange.Text="";

                txtPoject1.Text = PPlottingLib.GetDesc("desc1");
                txtPoject2.Text = PPlottingLib.GetDesc("desc2");
                txtPoject3.Text="";
                txtPoject4.Text="";
                txtPreparedBy.Text = PPlottingLib.GetFullName() ;
            //txtScale.Text="";
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            m_bCreatePlot = false;
            m_bPlaceComponment = false;
            load_first_time = true;
            this.Hide();
        }

        private void optMapScalePreDefined_CheckedChanged(object sender, System.EventArgs e)
        {
            lbMapScalePreDefined.Enabled = true;
            tbMapScaleCustom.Enabled = false;
            if ((optMapScalePreDefined.Checked == false))
            {
                m_sMapScalePreDefined = lbMapScalePreDefined.Text;
                // m_vListIndex = lbMapScalePreDefined.ListIndex
            }
            else
            {
                // lbMapScalePreDefined.Selected(m_vListIndex) = False
                // lbMapScalePreDefined.Selected(m_vListIndex) = True
            }
            m_sMapScale = lbMapScalePreDefined.Text;
            WPB_Change();
        }

        private void optMapScaleCustom_CheckedChanged(object sender, System.EventArgs e)
        {
            lbMapScalePreDefined.Enabled = false;
            tbMapScaleCustom.Enabled = true;
            m_sMapScaleCustom = tbMapScaleCustom.Text;
            m_sMapScale = m_sMapScaleCustom;
            WPB_Change();
        }

        private void optPaperOrientationLandbase_CheckedChanged(object sender, System.EventArgs e)
        {
            m_sSheetOrientation = "Landscape";
            WPB_Change();
        }

        private void optPaperOrientationPortrait_CheckedChanged(object sender, System.EventArgs e)
        {
            m_sSheetOrientation = "Portrait";
            WPB_Change();
        }

        private void tbMapScaleCustom_TextChanged(object sender, System.EventArgs e)
        {
            lblSetMapScaleValue.Text = tbMapScaleCustom.Text;
            m_sMapScaleCustom = tbMapScaleCustom.Text;
            m_sMapScale = m_sMapScaleCustom;
            WPB_Change();
        }

        private void cbType_CheckedChanged(object sender, System.EventArgs e)
        {
            lbType.Enabled = cbType.Checked;
            if (cbType.Checked)
            {
                m_sType = lbType.Text;
            }
            else
            {
                m_sType = "";
            }
            WPB_Change();
        }

        private void lbPaperSize_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            m_sSheetName = lbPaperSize.Text.Substring(1, lbPaperSize.Text.IndexOf(')') - 1);
            m_sSheetSize = lbPaperSize.Text.Substring(lbPaperSize.Text.IndexOf(')') + 1, lbPaperSize.Text.Length - (lbPaperSize.Text.IndexOf(')') + 1)).Trim();
            WPB_Change();
        }

        private void lbType_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            m_sType = lbType.Text;
            WPB_Change();
            
            if (lbType.Text == "Junction" || lbType.Text == "Trunk")
            {
                cmb_PlotType.Text = "HSBB DSIDE";
            }
            else
            {
                cmb_PlotType.Text = lbType.Text;
            }
        }

        private void lbMapScalePreDefined_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            m_sMapScalePreDefined = lbMapScalePreDefined.Text;
            m_sMapScale = m_sMapScalePreDefined;
            WPB_Change();
        }

        private void btnPlace_Click(object sender, EventArgs e)
        {
            try
            {
                application.Properties.Remove("PlotBoundary.Type");
                application.Properties.Remove("PlotBoundary.PaperSize");
                application.Properties.Remove("PlotBoundary.SheetOrientation");
                application.Properties.Remove("PlotBoundary.MapScale");
            }
            catch { }

            application.Properties.Add("PlotBoundary.Type", m_sType);
            application.Properties.Add("PlotBoundary.PaperSize", m_sSheetSize);
            application.Properties.Add("PlotBoundary.SheetOrientation", m_sSheetOrientation);
            if (optMapScalePreDefined.Checked)
                application.Properties.Add("PlotBoundary.MapScale", m_sMapScale);
            else
                application.Properties.Add("PlotBoundary.MapScale", m_sMapScaleCustom);

            m_bPlaceComponment = true;
            m_bCreatePlot = false;
            this.Hide();
        }

        private void btnCreatePlotWindows_Click(object sender, EventArgs e)
        {
            //Vinod Apr-2013
            if (cmb_PlotType.Text == "")
            {
                MessageBox.Show("Please selcct a Plot Type", "Plotting", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            m_bCreatePlot = true;
            
            //Vinod Apr-2013
            m_rbNewPlot = rbNewPlot.Checked;
            m_rbExtPlot = rbExtPlot.Checked;
            m_JobID = lblPPWO.Text;
            m_PlotType = cmb_PlotType.Text;

            this.Hide();
        }

    }
}