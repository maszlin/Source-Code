using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

using System.Drawing;
using Intergraph.GTechnology.Interfaces;
using Intergraph.GTechnology.API;
using AG.GTechnology.Utilities;
using ADODB;

namespace NEPS.OSP.COPPER.PLOT.MULTI
{
    class clsPlotting
    {
        public static void Plotting(string[] title, int[] frames, string printerName, int totalpage, bool preview)
        {
            try
            {
                #region plot

                if (frames.Length > 0)
                {
                    GTMultiPlot.m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Create/Place plot or Cancel to exit");

                    string sStatusBarTextTemp = string.Empty;
                    IGTMapWindow oMapWindow = GTMultiPlot.activeMapWindow;
                    /*
                    IGTNamedPlots oGTNamePlots = GTMultiPlot.m_gtapp.NamedPlots;                   
                    for (int c = oGTNamePlots.Count; c > 0; )
                    {
                        oGTNamePlots.Remove(--c);
                    }
                    IGTPlotWindows oGTPlotWdws = GTMultiPlot.m_gtapp.GetPlotWindows();
                    for (int w = oGTPlotWdws.Count; w > 0; )
                    {
                        oGTPlotWdws[w].Close(); 
                    }
                    */
                    
                    foreach (int page in frames)
                    {
                        PlotBoundary oPlot = GTMultiPlot.plotFrame.Frames[page];

                        if (oPlot.isHidden) continue;
                        oPlot.Name = "Plotting Page " + oPlot.labelName + " of " + totalpage.ToString(); //oPlot.labelName;
                        oPlot.Legend = oMapWindow.LegendName;
                        oPlot.activemap = oMapWindow;
                        oPlot.ActiveLegendNodes = oMapWindow.DisplayService.GetDisplayControlNodes();


                        IGTNamedPlot oGTNamedPlot;
                        IGTPlotWindow oGTPlotWindow;

                        #region PlotWindow
                        try
                        {
                            #region Check for Open Plot Window
                            IGTPlotWindows oGTPlotWindows = GTMultiPlot.m_gtapp.GetPlotWindows();
                            for (int i = 0; i < oGTPlotWindows.Count; i++)
                            {
                                IGTPlotWindow PlotWindow = oGTPlotWindows[i];
                                if (PlotWindow.NamedPlot.Name == oPlot.Name)
                                    throw new System.Exception("Plow window already exist"); // plot window already exist, no need to create a new one
                            }

                            IGTNamedPlots oGTNamePlots = GTMultiPlot.m_gtapp.NamedPlots;
                            for (int j = oGTNamePlots.Count; j > 0; )
                            {
                                --j;
                                if (oGTNamePlots[j].Name == oPlot.Name)
                                    oGTNamePlots.Remove(j);
                            }
                            #endregion

                            //  Create a new plot window named the same as the PlanID
                            oGTNamedPlot = GTMultiPlot.m_gtapp.NewNamedPlot(oPlot.Name);
                            //  Auto open the new named plot - need to only do this if single named plot created otherwise ask user at the end using dialog FUTURE -Currently need to have PlotWindow open to Insert a Map Window
                            sStatusBarTextTemp = GTMultiPlot.m_gtapp.GetStatusBarText(GTStatusPanelConstants.gtaspcMessage);
                            oGTPlotWindow = GTMultiPlot.m_gtapp.NewPlotWindow(oGTNamedPlot);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(GTMultiPlot.m_CustomForm,ex.Message);
                            continue;
                        }
                        #endregion

                        GTMultiPlot.m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, sStatusBarTextTemp);
                        PPlottingLib.StartDrawingRedlines(oPlot, myUtil.ParseInt(oPlot.labelName), totalpage, title);

                        //preview = true;
                        if (preview) continue; // continue to next without printing & closing current plotwindow

                        oGTPlotWindow.PrintPlot(printerName);
                        oGTPlotWindow.Close();
                        GTMultiPlot.m_gtapp.NamedPlots.Remove(oPlot.Name);

                    }
                }

                #endregion
            }
            catch (Exception ex)
            {

            }
        }

        public static void Plotting_Overall(string[] title, string printerName, int totalpage, bool preview, int count)
        {
            try
            {
                GTMultiPlot.m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Create/Place plot or Cancel to exit");
                string sStatusBarTextTemp = string.Empty;
                IGTMapWindow oMapWindow = GTMultiPlot.activeMapWindow;

                PlotBoundary oPlot = GTMultiPlot.plotFrame.MainFrame;

                oPlot.Name = "Plotting Area Overview [" + count.ToString() + "]";
                oPlot.Legend = oMapWindow.LegendName;
                oPlot.activemap = oMapWindow;
                oPlot.ActiveLegendNodes = oMapWindow.DisplayService.GetDisplayControlNodes();


                IGTNamedPlot oGTNamedPlot;
                IGTPlotWindow oGTPlotWindow;
                IGTPlotWindows oGTPlotWindows;
                try
                {
                    #region Check for Open Plot Window
                    oGTPlotWindows = GTMultiPlot.m_gtapp.GetPlotWindows();
                    for (int i = 0; i < oGTPlotWindows.Count; i++)
                    {
                        IGTPlotWindow PlotWindow = oGTPlotWindows[i];
                        if (PlotWindow.NamedPlot.Name == oPlot.Name)
                            PlotWindow.Close(); // plot window already exist, no need to create a new one
                            //return; 
                    }
                    #endregion

                    //  Create a new plot window named the same as the PlanID
                    oGTNamedPlot = GTMultiPlot.m_gtapp.NewNamedPlot(oPlot.Name );
                    //  Auto open the new named plot - need to only do this if single named plot created otherwise ask user at the end using dialog FUTURE -Currently need to have PlotWindow open to Insert a Map Window
                    sStatusBarTextTemp = GTMultiPlot.m_gtapp.GetStatusBarText(GTStatusPanelConstants.gtaspcMessage);
                    oGTPlotWindow = GTMultiPlot.m_gtapp.NewPlotWindow(oGTNamedPlot);
                }
                catch (Exception ex)
                {
                    //GTMultiPlot.m_gtapp.SetStatusBarText(ex.Message, sStatusBarTextTemp);
                    return;
                }

                GTMultiPlot.m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, sStatusBarTextTemp);
                PPlottingLib.StartDrawingRedlines(oPlot, 0, totalpage, title);

                if (preview) return; // continue to next without printing & closing current plotwindow

                oGTPlotWindow.PrintPlot(printerName);
                oGTPlotWindow.Close();
            }
            catch (Exception ex)
            {

            }
        }
    }
}
