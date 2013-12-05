using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

using Intergraph.GTechnology.Interfaces;
using Intergraph.GTechnology.API;
using AG.GTechnology.Utilities;


namespace NEPS.OSP.COPPER.PLOT.MULTI
{
    public partial class usrPlotProperties : UserControl
    {       
        clsGetPlotConfig getPlot = new clsGetPlotConfig();
        //Intergraph.GTechnology.API.IGTApplication application = GTClassFactory.Create<IGTApplication>();

        #region Initialize

        public usrPlotProperties()
        {
            InitializeComponent();
            InitPlotProperties();
        }

        private void InitPlotProperties()
        {
            ADODB.Recordset rs;
            try
            {
                if (GTMultiPlot.m_iPlotTask == GTMultiPlot.PlotTask.init) // first time
                {
                    GTMultiPlot.m_iPlotTask = GTMultiPlot.PlotTask.set_plot_prop;

                    rs = getPlot.GetTypes(); // get plotting type
                    lbType.Items.Clear();
                    while (!rs.EOF)
                    {
                        lbType.Items.Add(rs.Fields["DRI_TYPE"].Value);
                        rs.MoveNext();
                    }

                    // Populate PaperSize listbox
                    rs = getPlot.GetPaperSizes();
                    lbPaperSize.Items.Clear();
                    while (!rs.EOF)
                    {
                        lbPaperSize.Items.Add("(" + rs.Fields["SHEET_NAME"].Value + ") " + rs.Fields["SHEET_SIZE"].Value);
                        rs.MoveNext();
                    }

                    rs = getPlot.GetScales();
                    lbMapScalePreDefined.Items.Clear();
                    int i = 1;
                    while (!rs.EOF)
                    {
                        lbMapScalePreDefined.Items.Add("1:" + rs.Fields["SCALE"].Value);
                        rs.MoveNext();
                    }
                    rs.Close();

                    // Populate MapScaleCustom textbox
                    tbMapScaleCustom.Text = "1:1000";
                    if (lbPaperSize.Items.Count > 0) lbPaperSize.SelectedIndex = 0;
                    clsPlotProperties.m_sSheetSize = lbPaperSize.Text.Substring(lbPaperSize.Text.IndexOf(')') + 1, 
                        lbPaperSize.Text.Length - (lbPaperSize.Text.IndexOf(')') + 1)).Trim();
                    clsPlotProperties.m_sMapScaleCustom = "1:250";
                    clsPlotProperties.m_sMapScale = clsPlotProperties.m_sMapScaleCustom;
                    clsPlotProperties.m_sSheetOrientation = "Landscape";
                    optPaperOrientationLandbase.Enabled = true;
                    lbType.SetSelected(0, true);
                    lbPaperSize.SetSelected(0, true);
                    lbMapScalePreDefined.SetSelected(0, true);
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

        #endregion

        #region Template
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

                if (clsPlotProperties.m_iDRI_ID == 0)
                {
                    g.DrawString("NO TEMPLATE", new Font("Arial", 20, FontStyle.Bold),
                        Brushes.White, new PointF(imgPageContainer.Width / 4, imgPageContainer.Height / 2));
                    btnPlace.Enabled = false;
                    return;
                }

                double dPgContainerScale;
                if ((clsPlotProperties.m_sSheetOrientation == "Landscape"))
                    dPgContainerScale = (imgPageContainer.Width / clsPlotProperties.m_dSheetWidth) * 0.9;

                else
                    dPgContainerScale = (imgPageContainer.Height / clsPlotProperties.m_dSheetHeight) * 0.9;

                // g.PageScale = dPgContainerScale
                System.Drawing.Rectangle oRectPaper;
                System.Drawing.Rectangle oRectBorder;
                System.Drawing.Rectangle oRectMapWindow;
                //  Draw Page
                oRectPaper = new System.Drawing.Rectangle();
                oRectPaper.X = (int)((imgPageContainer.Width - (clsPlotProperties.m_dSheetWidth * dPgContainerScale)) / 2);
                oRectPaper.Y = (int)((imgPageContainer.Height - (clsPlotProperties.m_dSheetHeight * dPgContainerScale)) / 2);
                oRectPaper.Height = (int)(clsPlotProperties.m_dSheetHeight * dPgContainerScale);
                oRectPaper.Width = (int)(clsPlotProperties.m_dSheetWidth * dPgContainerScale);
                g.DrawRectangle(Pens.Black, oRectPaper);
                g.FillRectangle(Brushes.White, oRectPaper);
                //  Draw Border
                oRectBorder = new System.Drawing.Rectangle();
                oRectBorder.X = (int)(oRectPaper.X + (clsPlotProperties.m_dSheet_Inset * dPgContainerScale));
                oRectBorder.Y = (int)(oRectPaper.Y + (clsPlotProperties.m_dSheet_Inset * dPgContainerScale));
                oRectBorder.Height = (int)(oRectPaper.Height - (clsPlotProperties.m_dSheet_Inset * 2 * dPgContainerScale));
                oRectBorder.Width = (int)(oRectPaper.Width - (clsPlotProperties.m_dSheet_Inset * 2 * dPgContainerScale));
                g.DrawRectangle(Pens.Black, oRectBorder);
                //  Draw MapWindow
                oRectMapWindow = new System.Drawing.Rectangle();
                //  Old - hard coded to start map window at TL _ inset... no good
                // oRectMapWindow.X = oRectPaper.X + (clsPlotProperties.m_dSheet_Inset * dPgContainerScale)
                // oRectMapWindow.Y = oRectPaper.Y + (clsPlotProperties.m_dSheet_Inset * dPgContainerScale)
                // oRectMapWindow.Height = clsPlotProperties.m_dMapHeight * dPgContainerScale
                // oRectMapWindow.Width = clsPlotProperties.m_dMapWidth * dPgContainerScale
                oRectMapWindow.X = (int)(oRectPaper.X
                            + (clsPlotProperties.m_oMapTLPoint.X * dPgContainerScale));
                oRectMapWindow.Y = (int)(oRectPaper.Y
                            + (clsPlotProperties.m_oMapTLPoint.Y * dPgContainerScale));
                oRectMapWindow.Height = (int)((clsPlotProperties.m_oMapBRPoint.Y - clsPlotProperties.m_oMapTLPoint.Y)
                            * dPgContainerScale);
                oRectMapWindow.Width = (int)((clsPlotProperties.m_oMapBRPoint.X - clsPlotProperties.m_oMapTLPoint.X)
                            * dPgContainerScale);
                g.DrawImage(ImageList1.Images[0], oRectMapWindow);
                g.DrawRectangle(Pens.Black, oRectMapWindow);
                //  Draw the border a little thicker
                oRectBorder.Inflate((int)(-0.25 * dPgContainerScale), (int)(-0.25 * dPgContainerScale));
                g.DrawRectangle(Pens.Black, oRectBorder);
                oRectBorder.Inflate((int)(0.5 * dPgContainerScale), (int)(0.5 * dPgContainerScale));
                g.DrawRectangle(Pens.Black, oRectBorder);
                ADODB.Recordset rsRedlines;
                rsRedlines = getPlot.GetRedlines(clsPlotProperties.m_iDRI_ID);
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
                MessageBox.Show(this, "PlotBoundaryForm.imgPageContainer_Paint:\r\n" + ex.Message, ex.Source, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        #endregion

        #region Settings
        private void optMapScalePreDefined_CheckedChanged(object sender, System.EventArgs e)
        {
            lbMapScalePreDefined.Enabled = true;
            tbMapScaleCustom.Enabled = false;
            if ((optMapScalePreDefined.Checked == false))
            {
                clsPlotProperties.m_sMapScalePreDefined = lbMapScalePreDefined.Text;
                // m_vListIndex = lbMapScalePreDefined.ListIndex
            }
            else
            {
                // lbMapScalePreDefined.Selected(m_vListIndex) = False
                // lbMapScalePreDefined.Selected(m_vListIndex) = True
            }
            clsPlotProperties.m_sMapScale = lbMapScalePreDefined.Text;
            WPB_Change();
        }

        private void optMapScaleCustom_CheckedChanged(object sender, System.EventArgs e)
        {
            lbMapScalePreDefined.Enabled = false;
            tbMapScaleCustom.Enabled = true;
            clsPlotProperties.m_sMapScaleCustom = tbMapScaleCustom.Text;
            clsPlotProperties.m_sMapScale = clsPlotProperties.m_sMapScaleCustom;
            WPB_Change();
        }

        private void optPaperOrientationLandbase_CheckedChanged(object sender, System.EventArgs e)
        {
            clsPlotProperties.m_sSheetOrientation = "Landscape";
            WPB_Change();
        }

        private void optPaperOrientationPortrait_CheckedChanged(object sender, System.EventArgs e)
        {
            clsPlotProperties.m_sSheetOrientation = "Portrait";
            WPB_Change();
        }

        private void tbMapScaleCustom_TextChanged(object sender, System.EventArgs e)
        {
            lblSetMapScaleValue.Text = tbMapScaleCustom.Text;
            clsPlotProperties.m_sMapScaleCustom = tbMapScaleCustom.Text;
            clsPlotProperties.m_sMapScale = clsPlotProperties.m_sMapScaleCustom;
            WPB_Change();
        }

        private void cbType_CheckedChanged(object sender, System.EventArgs e)
        {
            lbType.Enabled = cbType.Checked;
            if (cbType.Checked)
            {
                clsPlotProperties.m_sType = lbType.Text;
            }
            else
            {
                clsPlotProperties.m_sType = "";
            }
            WPB_Change();
        }

        private void lbPaperSize_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            clsPlotProperties.m_sSheetName = lbPaperSize.Text.Substring(1, lbPaperSize.Text.IndexOf(')') - 1);
            clsPlotProperties.m_sSheetSize = lbPaperSize.Text.Substring(lbPaperSize.Text.IndexOf(')') + 1, lbPaperSize.Text.Length - (lbPaperSize.Text.IndexOf(')') + 1)).Trim();
            WPB_Change();
        }

        private void lbType_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            clsPlotProperties.m_sType = lbType.Text;
            WPB_Change();
        }

        private void lbMapScalePreDefined_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            clsPlotProperties.m_sMapScalePreDefined = lbMapScalePreDefined.Text;
            clsPlotProperties.m_sMapScale = clsPlotProperties.m_sMapScalePreDefined;
            WPB_Change();
        }
        /*
         *         private void btnCancel_Click(object sender, EventArgs e)
        {
            clsPlotProperties.m_bCreatePlot = false;
            clsPlotProperties.m_bPlaceComponment = false;
            load_first_time = true;
            this.Hide();
        }

        private void optMapScalePreDefined_CheckedChanged(object sender, System.EventArgs e)
        {
            lbMapScalePreDefined.Enabled = true;
            tbMapScaleCustom.Enabled = false;
            if ((optMapScalePreDefined.Checked == false))
            {
                clsPlotProperties.m_sMapScalePreDefined = lbMapScalePreDefined.Text;
                // m_vListIndex = lbMapScalePreDefined.ListIndex
            }
            else
            {
                // lbMapScalePreDefined.Selected(m_vListIndex) = False
                // lbMapScalePreDefined.Selected(m_vListIndex) = True
            }
            clsPlotProperties.m_sMapScale = lbMapScalePreDefined.Text;
            WPB_Change();
        }

        private void optMapScaleCustom_CheckedChanged(object sender, System.EventArgs e)
        {
            lbMapScalePreDefined.Enabled = false;
            tbMapScaleCustom.Enabled = true;
            clsPlotProperties.m_sMapScaleCustom = tbMapScaleCustom.Text;
            clsPlotProperties.m_sMapScale = clsPlotProperties.m_sMapScaleCustom;
            WPB_Change();
        }

        private void optPaperOrientationLandbase_CheckedChanged(object sender, System.EventArgs e)
        {
            clsPlotProperties.m_sSheetOrientation = "Landscape";
            WPB_Change();
        }

        private void optPaperOrientationPortrait_CheckedChanged(object sender, System.EventArgs e)
        {
            clsPlotProperties.m_sSheetOrientation = "Portrait";
            WPB_Change();
        }

        private void tbMapScaleCustom_TextChanged(object sender, System.EventArgs e)
        {
            lblSetMapScaleValue.Text = tbMapScaleCustom.Text;
            clsPlotProperties.m_sMapScaleCustom = tbMapScaleCustom.Text;
            clsPlotProperties.m_sMapScale = clsPlotProperties.m_sMapScaleCustom;
            WPB_Change();
        }

        private void cbType_CheckedChanged(object sender, System.EventArgs e)
        {
            lbType.Enabled = cbType.Checked;
            if (cbType.Checked)
            {
                clsPlotProperties.m_sType = lbType.Text;
            }
            else
            {
                clsPlotProperties.m_sType = "";
            }
            WPB_Change();
        }

        private void lbPaperSize_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            clsPlotProperties.m_sSheetName = lbPaperSize.Text.Substring(1, lbPaperSize.Text.IndexOf(')') - 1);
            clsPlotProperties.m_sSheetSize = lbPaperSize.Text.Substring(lbPaperSize.Text.IndexOf(')') + 1, lbPaperSize.Text.Length - (lbPaperSize.Text.IndexOf(')') + 1)).Trim();
            WPB_Change();
        }

        private void lbType_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            clsPlotProperties.m_sType = lbType.Text;
            WPB_Change();
        }

        private void lbMapScalePreDefined_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            clsPlotProperties.m_sMapScalePreDefined = lbMapScalePreDefined.Text;
            clsPlotProperties.m_sMapScale = clsPlotProperties.m_sMapScalePreDefined;
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

            application.Properties.Add("PlotBoundary.Type", clsPlotProperties.m_sType);
            application.Properties.Add("PlotBoundary.PaperSize", clsPlotProperties.m_sSheetSize);
            application.Properties.Add("PlotBoundary.SheetOrientation", clsPlotProperties.m_sSheetOrientation);
            if (optMapScalePreDefined.Checked)
                application.Properties.Add("PlotBoundary.MapScale", clsPlotProperties.m_sMapScale);
            else
                application.Properties.Add("PlotBoundary.MapScale", clsPlotProperties.m_sMapScaleCustom);

            clsPlotProperties.m_bPlaceComponment = true;
            clsPlotProperties.m_bCreatePlot = false;
         
            this.Hide();
        }


        */
        // 
        //  When the WPB changes update the image page
        // 
        private void WPB_Change()
        {
            try
            {
                // If the SheetName or SheetOrientation is not set exit
                if ((string.IsNullOrEmpty(clsPlotProperties.m_sSheetName)
                            || (string.IsNullOrEmpty(clsPlotProperties.m_sSheetOrientation)
                            || string.IsNullOrEmpty(clsPlotProperties.m_sMapScale))))
                {
                    return;
                }
                if (!ValidScale(clsPlotProperties.m_sMapScale))
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
                getPlot.GetSheetSize(clsPlotProperties.m_sType, clsPlotProperties.m_sSheetName, clsPlotProperties.m_sSheetOrientation, ref clsPlotProperties.m_lSheetId, ref clsPlotProperties.m_dSheetHeight, ref clsPlotProperties.m_dSheetWidth, ref clsPlotProperties.m_dSheet_Inset, ref clsPlotProperties.m_iDRI_ID);
                // Get the Map Size from Map Frame table
                getPlot.GetMapSize(clsPlotProperties.m_iDRI_ID, ref clsPlotProperties.m_dMapHeight, ref clsPlotProperties.m_dMapWidth);
                // Get the Map Location from Map Frame table
                getPlot.GetMapLocation(clsPlotProperties.m_iDRI_ID, ref clsPlotProperties.m_oMapTLPoint, ref clsPlotProperties.m_oMapBRPoint);
                //  Get Map Scale & Scaled Map Size
                clsPlotProperties.m_lMapScale = int.Parse(clsPlotProperties.m_sMapScale.Substring(2, clsPlotProperties.m_sMapScale.Length - 2));
                clsPlotProperties.m_dMapHeightScaled = (clsPlotProperties.m_dMapHeight
                            * ((double)clsPlotProperties.m_lMapScale / 1000));
                clsPlotProperties.m_dMapWidthScaled = (clsPlotProperties.m_dMapWidth
                            * ((double)clsPlotProperties.m_lMapScale / 1000));
                if ((optPaperOrientationLandbase.Checked == true))
                {
                    lblSetPaperValue.Text = (clsPlotProperties.m_sSheetSize + ("  ("
                                + (optPaperOrientationLandbase.Text + ")")));
                }
                else
                {
                    lblSetPaperValue.Text = (clsPlotProperties.m_sSheetSize + ("  ("
                                + (optPaperOrientationPortrait.Text + ")")));
                }
                lblSetBorderInsetValue.Text = (clsPlotProperties.m_dSheet_Inset + "mm");
                lblSetMapSizeValue.Text = (Convert.ToString(System.Math.Round(clsPlotProperties.m_dMapWidthScaled, 1)) + ("m x "
                            + (Convert.ToString(System.Math.Round(clsPlotProperties.m_dMapHeightScaled, 1)) + "m"))).Trim();
                lblSetMapScaleValue.Text = clsPlotProperties.m_sMapScale;
                btnPlace.Enabled = true;
                imgPageContainer.Refresh();
            }
            catch (Exception ex)
            {
                MessageBox.Show(("PlotBoundaryForm.WPB_Change:" + ("\r\n" + ex.Message)), ex.Source, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private bool ValidScale(string sScale)
        {
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


        #endregion

        #region Plotting
        private void btnPlace_Click(object sender, EventArgs e)
        {
            try
            {
                GTMultiPlot.m_gtapp.Properties.Remove("PlotBoundary.Type");
                GTMultiPlot.m_gtapp.Properties.Remove("PlotBoundary.PaperSize");
                GTMultiPlot.m_gtapp.Properties.Remove("PlotBoundary.SheetOrientation");
                GTMultiPlot.m_gtapp.Properties.Remove("PlotBoundary.MapScale");
            }
            catch { }

            GTMultiPlot.m_gtapp.Properties.Add("PlotBoundary.Type", clsPlotProperties.m_sType);
            GTMultiPlot.m_gtapp.Properties.Add("PlotBoundary.PaperSize", clsPlotProperties.m_sSheetSize);
            GTMultiPlot.m_gtapp.Properties.Add("PlotBoundary.SheetOrientation", clsPlotProperties.m_sSheetOrientation);
            if (optMapScalePreDefined.Checked)
                GTMultiPlot.m_gtapp.Properties.Add("PlotBoundary.MapScale", clsPlotProperties.m_sMapScale);
            else
                GTMultiPlot.m_gtapp.Properties.Add("PlotBoundary.MapScale", clsPlotProperties.m_sMapScaleCustom);

            GTMultiPlot.m_iPlotTask = GTMultiPlot.PlotTask.set_plot_area;
            this.ParentForm.Hide();
        }
        #endregion

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.ParentForm.Close();
        }
    }
}
