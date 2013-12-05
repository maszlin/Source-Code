/*
 *  class name : usrPlotList
 *  develop by : m.zam
 *  created on : 29-Mac-2013
 * 
 *  functions :
 *  1. list plot frame as per user selection
 *  2. allow user to remove selected frame
 *  3. allow user to re-number the other plot frame
 *  4. allow user to re-build all the frame (cancel remove)
 *  5. allow user to plot/print selected frame (1 .. all frames)
 * 
 */ 

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

using System.Drawing.Printing;

namespace NEPS.OSP.COPPER.PLOT.MULTI
{
    public partial class usrPlotList : UserControl
    {
        #region Declaration  - check list box
        
        clsCheckedListBox chkList;
        struct ChkItem
        {
            int key;
            string text;
            bool hidden;

            public ChkItem(int key, string text)
            {
                this.key = key;
                this.text = text;
                this.hidden = false;
            }

            public int Key
            { get { return this.key; } }

            public string Text
            {
                get { return this.text; }
                set { this.text = value; }
            }

            public bool Hidden
            {
                get { return this.hidden; }
                set { this.hidden = value; }
            }

            public override string ToString()
            {
                return this.text;
            }
        }

        #endregion

        #region Initialization
        public usrPlotList()
        {
            InitializeComponent();
            chkList = new clsCheckedListBox();
            chkList.Parent = this.panel1;
            chkList.Location = chkListFrame.Location;
            chkList.Size = chkListFrame.Size;
            chkList.Dock = DockStyle.Fill;
            chkList.MultiColumn = true;
            chkList.ColumnWidth = 100;
            //chkList.ItemCheck += chkList_ItemCheck;
            chkListFrame.Visible = false;
            chkList.IndeterminateColor = Color.LightGray;
            this.panel1.Controls.Add(chkList);
            chkList.BringToFront();

            LoadFrameList();

        }

        private void LoadFrameList()
        {
            chkList.Items.Clear();
            chkList.IndeterminateCount = 0;
            for (int i = 0; i < GTMultiPlot.plotFrame.Frames.Count; i++)
            {
                GTMultiPlot.plotFrame.Frames[i].isHidden = false;
                chkList.Items.Add(new ChkItem(i, "Frame " + (i + 1)));
            }
        }
        private void usrPlotList_Load(object sender, EventArgs e)
        {
            PrinterSettings settings = new PrinterSettings();
            foreach (string prt in PrinterSettings.InstalledPrinters)
            {
                cmbPrinter.Items.Add(prt);
                settings.PrinterName = prt;
                if (settings.IsDefaultPrinter)
                    cmbPrinter.Text = prt;
            }           
        }
        #endregion

        #region Check or Uncheck all items in check list
        private void btnSelect_Click(object sender, EventArgs e)
        {
            SetItemChecked(true);
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            SetItemChecked(false);
        }

        private void SetItemChecked(bool flag)
        {
            for (int i = 0; i < chkList.Items.Count; i++)
                chkList.SetItemChecked(i, flag);
        }
        #endregion

        #region Remove - Reset - Renumber
        private void btnRemove_Click(object sender, EventArgs e)
        {
            for (int i = chkList.CheckedIndices.Count - 1; i > -1; i--)
            {
                int del_index = chkList.CheckedIndices[i];
                ChkItem f = (ChkItem)chkList.Items[del_index];
                GTMultiPlot.plotFrame.RemoveFrameLabel(GTMultiPlot.mobjPlotFrame, del_index + 2, f.Key);
                chkList.Items.RemoveAt(del_index);
            }
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            LoadFrameList();
            GTMultiPlot.plotFrame.DrawFrame(GTMultiPlot.mobjPlotFrame);
        }


        private void btnReNumber_Click(object sender, EventArgs e)
        {
            for (int i = 0, key = 0; i < chkList.Items.Count; i++)
            {
                ChkItem itm = (ChkItem)chkList.Items[i];
                itm.Text = "Frame " + (++key).ToString();
                chkList.Items[i] = itm;
            }

            GTMultiPlot.plotFrame.UpdateFrameIndex(GTMultiPlot.mobjPlotFrame);
        }
        #endregion

        #region Plotting -preview or autoprint selected frame or all frame 
        private void btnPlot_Click(object sender, EventArgs e)
        {
            int totalpage = chkList.Items.Count; //-chkList.IndeterminateCount;

            PrinterSettings printerSettings = new PrinterSettings();
            printerSettings.PrinterName = cmbPrinter.Text;
            string[] papersize = clsPlotProperties.m_sSheetSize.Split(',');
            printerSettings.DefaultPageSettings.PaperSize
            = new PaperSize(clsPlotProperties.m_sSheetName, (int)clsPlotProperties.m_dSheetWidth, (int)clsPlotProperties.m_dSheetHeight);

            printerSettings.DefaultPageSettings.Landscape = true;

            string[] plotTitle = {
                txtPoject1.Text, txtPoject2.Text,txtPoject3.Text,txtPoject4.Text, 
                txtPreparedBy.Text,txtRevisedBy.Text,txtApproved.Text
            };

            int[] plotIndex = new int[chkList.CheckedIndices.Count];
            for (int i = 0; i < chkList.CheckedIndices.Count; i++)
            {
                ChkItem itm = (ChkItem)chkList.Items[chkList.CheckedIndices[i]];
                plotIndex[i] = itm.Key;
            }
            if (plotIndex.Length > 0)
                clsPlotting.Plotting(plotTitle, plotIndex, cmbPrinter.Text, chkList.Items.Count, optPreview.Checked);
            else
                MessageBox.Show(GTMultiPlot.m_CustomForm, "Please select frame(s) to plot");
        }
        #endregion

        #region Other Events
        private void btnClose_Click(object sender, EventArgs e)
        {
            this.ParentForm.Close();
        }

        private void btnSplitter_Click(object sender, EventArgs e)
        {
            if (btnSplitter.ImageIndex == 1)
            {
                grpPlotProp.Height = btnSplitter.Height;
                btnSplitter.ImageIndex = 0;
            }
            else
            {
                grpPlotProp.Height = 160;
                btnSplitter.ImageIndex = 1;
            }
        }
        #endregion

        int print_all_count = 0;
        private void btnPlotOverview_Click(object sender, EventArgs e)
        {
            int totalpage = chkList.Items.Count; //-chkList.IndeterminateCount;
            
            PrinterSettings printerSettings = new PrinterSettings();
            printerSettings.PrinterName = cmbPrinter.Text;
            string[] papersize = clsPlotProperties.m_sSheetSize.Split(',');
            printerSettings.DefaultPageSettings.PaperSize
            = new PaperSize(clsPlotProperties.m_sSheetName, (int)clsPlotProperties.m_dSheetWidth, (int)clsPlotProperties.m_dSheetHeight);

            printerSettings.DefaultPageSettings.Landscape = true;

            string[] plotTitle = {
                txtPoject1.Text, txtPoject2.Text,txtPoject3.Text,txtPoject4.Text, 
                txtPreparedBy.Text,txtRevisedBy.Text,txtApproved.Text
            };

            int[] plotIndex = new int[chkList.CheckedIndices.Count];
            for (int i = 0; i < chkList.CheckedIndices.Count; i++)
            {
                ChkItem itm = (ChkItem)chkList.Items[chkList.CheckedIndices[i]];
                plotIndex[i] = itm.Key;
            }
            clsPlotting.Plotting_Overall(plotTitle, cmbPrinter.Text, chkList.Items.Count, true, ++print_all_count);
        }     

    }
}
