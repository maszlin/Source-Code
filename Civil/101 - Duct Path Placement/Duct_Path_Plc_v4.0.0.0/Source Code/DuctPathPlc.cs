using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ADODB;
using Intergraph.GTechnology.Interfaces;
using Intergraph.GTechnology.API;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace NEPS.GTechnology.NEPSDuctPathPlc
{
    public partial class GTWindowsForm_DuctPathPlc : Form
    {
        #region variables
        private short SourceFNO = 0;
        private short TermFNO = 0; 
        private int SourceFID = 0;
        private int TermFID = 0;
        private int statusPage=0;
        private IGTPoint SourceDevicePoint = null;       
        private IGTPoint TermDevicePoint = null;
        private IGTPolylineGeometry DuctPathLineGeom = null;
        private int NumberOfConduit = 1;
        public int NewPathFID = 0;
        private int CloseStatus = 0;
        public int TotalGraphicLength = 0;
        public int TotalEnteredLength = 0;
        public int LastSection = 0;
        private struct DuctPathSect
        {
            public string NumDuctWaysSect;
            public string SectionLength;
            public int SectGraphicLength;
            public string SectDiam;
            public Nullable<DateTime> YearExpanded;
            public Nullable<DateTime> YearExtended;
            public string SectOwner;
            public string SectType;
            public string SectPlc;
            public string SectBillingRate;
            public string Encasement;
            public string SectBackFill;
            public string PUSect;
        };
        private List<DuctPathSect> Sections = null;
        
        public struct SectSlash
        {
            public double X;
            public double Y;
            public int length;
            public IGTVector Orient;
        };

        public struct SectLabelLeaderLine
        {
            public IGTTextPointGeometry Label;
            public IGTPolylineGeometry LeaderLine;
        };

        public List<SectSlash> SectSlashes = null;
        public List<SectLabelLeaderLine> SectLabels = null;
        public string SectLabel = "";
        public class ComboBoxItems
        {
            string _text;
            string _value;

            public ComboBoxItems() { _text = string.Empty; _value = string.Empty; }
            public ComboBoxItems(string text, string value)
            {
                _text = text;
                _value = value;
            }
            public string Text
            {
                get
                {
                    return _text;
                }
                set
                {
                    _text = value;
                }
            }
            public string Value
            {
                get { return _value; }
                set { _value = value; }
            }
            public override string ToString()
            {
                return _value;
            }
        };
#endregion

        #region function to change enability of buttons from other class
        public bool DrawBtnIsEnable
        {
            get { return btnGetSelTerm.Enabled; }
            set { btnGetSelTerm.Enabled = value; }
        }
        public bool ConfSelBtnIsEnable
        {
            get { return btnConfirmSectAttr.Enabled; }
            set { btnConfirmSectAttr.Enabled = value; }
        }
        #endregion

        #region init and load form
        public GTWindowsForm_DuctPathPlc()
        {
            try
            {
                InitializeComponent();
                DataTable dtable = new DataTable();
               
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Duct Path Placement", MessageBoxButtons.OK, MessageBoxIcon.Error);
                CloseStatus = 1;
                this.Close();
            }
        }



        private void GTWindowsForm_DuctPathPlc_Load(object sender, EventArgs e)
        {
            //getPPWO();
            //this.Hide();
            //GTDuctPathPlc.startdraw = 100;
            //GTDuctPathPlc.m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Select Sourse Device!");
          
                
        }

        private void GTWindowsForm_DuctPathPlc_Shown(object sender, EventArgs e)
        {
            GTDuctPathPlc.m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "");
       
        }
        #endregion

        #region Close Form
        private void GTWindowsForm_DuctPathPlc_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (CloseStatus == 0)
            {
                DialogResult retVal = MessageBox.Show("Are you sure that you want to exit?", "Duct Path Placement", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (retVal == DialogResult.Yes)
                {
                    statusPage = 0;
                    if (GTDuctPathPlc.mobjEditService != null)
                        GTDuctPathPlc.mobjEditService.RemoveAllGeometries();
                    GTDuctPathPlc.m_gtapp.SelectedObjects.Clear();
                    GTDuctPathPlc.startdraw = 0;
                    tabDuctPlc.SelectedTab = tabPagePLC;
                    LastSection = 0;
                    if(SectSlashes!=null)
                        SectSlashes.Clear();
                    if(SectLabels!=null)
                        SectLabels.Clear();
                    if(Sections!=null)
                        Sections.Clear();
                    SourceDevicePoint = null;
                    TermDevicePoint = null;
                    GTDuctPathPlc.StartDrawPoint = null;
                    GTDuctPathPlc.EndDrawPoint = null;
                    if (DuctPathLineGeom != null)
                    {
                        DuctPathLineGeom.Points.Clear();
                        DuctPathLineGeom = null;
                    }
                    btnGetSelSource.Enabled = true;
                    txtFIDSource.Text = "";
                    txtFIDTerm.Text = "";
                    txtTypeSource.Text = "";
                    txtTypeTerm.Text = "";
                    txtWallSource.Text = "";
                    txtWallTerm.Text = "";
                    NumberOfConduit=0;
                    SourceFID = 0;
                    SourceFNO = 0;
                    TermFID = 0;
                    TermFNO = 0;
                    gbSectValues.Enabled = true;
                    gbDuctPathAttrValues.Enabled = true;
                    txtSectionLength.Enabled = true;
                    e.Cancel = false;
                }
                else { e.Cancel = true; }
            }
            else
            {
                statusPage = 0;
                if (GTDuctPathPlc.mobjEditService != null)
                    GTDuctPathPlc.mobjEditService.RemoveAllGeometries();
                GTDuctPathPlc.m_gtapp.SelectedObjects.Clear();
                GTDuctPathPlc.startdraw = 0;
                tabDuctPlc.SelectedTab = tabPagePLC;
                LastSection = 0;
                if (SectSlashes != null)
                    SectSlashes.Clear();
                if (SectLabels != null)
                    SectLabels.Clear();
                if (Sections != null)
                    Sections.Clear();
                SourceDevicePoint = null;
                TermDevicePoint = null;
                GTDuctPathPlc.StartDrawPoint = null;
                GTDuctPathPlc.EndDrawPoint = null;
                if (DuctPathLineGeom != null)
                {
                    DuctPathLineGeom.Points.Clear();
                    DuctPathLineGeom = null;
                }
                btnGetSelSource.Enabled = true;
                txtFIDSource.Text = "";
                txtFIDTerm.Text = "";
                txtTypeSource.Text = "";
                txtTypeTerm.Text = "";
                txtWallSource.Text = "";
                txtWallTerm.Text = "";
                NumberOfConduit=0;
                SourceFID = 0;
                SourceFNO = 0;
                TermFID = 0;
                TermFNO = 0;
                gbSectValues.Enabled = true;
                gbDuctPathAttrValues.Enabled = true;
                txtSectionLength.Enabled = true;
            }
        }
        #endregion
       
        #region Get PPWO No
        void getPPWO()
        {
            try
            {
                string sSql = string.Empty;
                ADODB.Recordset rsWorkOrder = new ADODB.Recordset();
                string strActiveJob = GTDuctPathPlc.m_IGTDataContext.ActiveJob;
                sSql = "select work_order_id,g3e_description,g3e_identifier from g3e_job where g3e_identifier ='" + strActiveJob + "'";
                rsWorkOrder = GTDuctPathPlc.m_IGTDataContext.OpenRecordset(sSql, ADODB.CursorTypeEnum.adOpenStatic, ADODB.LockTypeEnum.adLockReadOnly, 1);
                if (!rsWorkOrder.EOF || !rsWorkOrder.BOF)
                {
                    if (rsWorkOrder.Fields[0] != null)
                    {
                        lbl_PPWO.Text = "PPWO No : " + rsWorkOrder.Fields[0].Value.ToString();                        
                    }
                    rsWorkOrder.Close();
                }
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Duct Path Placement", MessageBoxButtons.OK, MessageBoxIcon.Error);
                CloseStatus = 1;
                this.Close();
            }
        }
        #endregion
        
        #region Get Value from Database
        private string Get_Value(string sSql)
        {
            try
            {
                ADODB.Recordset rsPP = new ADODB.Recordset();
                rsPP = GTDuctPathPlc.m_IGTDataContext.OpenRecordset(sSql, ADODB.CursorTypeEnum.adOpenStatic, ADODB.LockTypeEnum.adLockReadOnly, 1);
                if (rsPP.RecordCount > 0)
                {
                    rsPP.MoveFirst();
                    return (rsPP.Fields[0].Value.ToString());
                }
               // MessageBox.Show("No data found!", "Duct Path Placement", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return "";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Duct Path Placement", MessageBoxButtons.OK, MessageBoxIcon.Error);
                CloseStatus = 1;
                this.Close();
                return "";
            }

        }
        #endregion

        #region Tab selection change
        private void tabDuctPlc_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (statusPage == 0)
            {
                tabDuctPlc.SelectedTab = tabPagePLC;
            }
            if (statusPage == 1)
            {
                tabDuctPlc.SelectedTab = tabPageAttr;
            }
            if (statusPage == 2)
            {
                tabDuctPlc.SelectedTab = tabPageSect;
            }
        }
        #endregion

        #region Page 1(4 steps to draw conduit)

        #region Step 1

        #region Get Selected for Source click
        private void btnGatSelSource_Click(object sender, EventArgs e)
        {
            this.Hide();
            GTDuctPathPlc.m_gtapp.SelectedObjects.Clear();
            GTDuctPathPlc.startdraw = 500;
           
        }
        public bool GetSourceDevice(short iFNO, int iFID, IGTGeometry geom)
        {
            
            #region check if selected allowed feature and if successshow detail
            
               if (iFNO != 2700 && iFNO != 3800 && iFNO != 3300 && iFNO != 2800)
                {
                    MessageBox.Show("Please select only allowed Civil Feature: Manhole,Chamber,Tunnel or Civil Node!", "Duct Path Placement", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    GTDuctPathPlc.m_gtapp.SelectedObjects.Clear();
                    return false;
                }

                if (iFID.ToString() == txtFIDTerm.Text)
                {
                    btnGetSelTerm.Enabled = true;
                    MessageBox.Show("Source and Terminate Civil Features should be different!", "Duct Path Placement", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    GTDuctPathPlc.m_gtapp.SelectedObjects.Clear();
                    return false;
                }

                //manhole
                if (iFNO == 2700)
                    {
                        txtWallSource.Text = "";
                        txtFIDSource.Text = iFID.ToString();
                        txtTypeSource.Text = "Manhole";
                        txtMHIDSource.Text = Get_Value("select MANHOLE_ID from GC_MANHL where g3e_fid = " + iFID.ToString());
                        lbMHIDSource.Visible = true;
                        txtMHIDSource.Visible = true;
                        btnWallSource.Enabled = true;
                    }
                //chamber
                    else if (iFNO == 3800)
                    {
                        txtWallSource.Text = "";
                        txtFIDSource.Text = iFID.ToString();
                        txtTypeSource.Text = "Chamber";
                        lbMHIDSource.Visible = false;
                        txtMHIDSource.Visible = false;
                        txtMHIDSource.Text = "";
                        btnWallSource.Enabled = true;
                     }
                //tunnel/trench
                     else if (iFNO == 3300)
                    {
                        txtWallSource.Text = "";
                        txtFIDSource.Text = iFID.ToString();
                        string type = Get_Value("select TRENCH from GC_TUNNEL where g3e_fid = " + iFID.ToString());
                        if (type == "N")
                            txtTypeSource.Text = "Tunnel";
                        else txtTypeSource.Text = "Trench";
                        lbMHIDSource.Visible = false;
                        txtMHIDSource.Visible = false;
                        txtMHIDSource.Text = "";
                        btnWallSource.Enabled = true;
                      }
                //civil node
                      else if (iFNO == 2800)
                {
                    txtWallSource.Text = "0";
                    txtFIDSource.Text = iFID.ToString();
                    txtTypeSource.Text = "Civil Node";
                    lbMHIDSource.Visible = false;
                    txtMHIDSource.Visible = false;
                    txtMHIDSource.Text = "";
                    btnWallSource.Enabled = false;
                    if (SourceDevicePoint == null)
                        SourceDevicePoint = GTClassFactory.Create<IGTPoint>();
                    SourceDevicePoint.X = geom.FirstPoint.X;
                    SourceDevicePoint.Y = geom.FirstPoint.Y;
                    SourceDevicePoint.Z = geom.FirstPoint.Z;

                    if (GTDuctPathPlc.StartDrawPoint == null)
                        GTDuctPathPlc.StartDrawPoint = GTClassFactory.Create<IGTPoint>();
                    GTDuctPathPlc.StartDrawPoint.X = SourceDevicePoint.X;
                    GTDuctPathPlc.StartDrawPoint.Y = SourceDevicePoint.Y;
                    GTDuctPathPlc.StartDrawPoint.Z = SourceDevicePoint.Z;
                }
            
            #endregion
                SourceFID = iFID;
                SourceFNO = iFNO;
            GTDuctPathPlc.m_gtapp.SelectedObjects.Clear();
            return true;
        }
        #endregion

        #region check if source selected
         private bool ConfirmSource()
        {
            if (txtTypeSource.Text == "" || txtFIDSource.Text == "" || txtWallSource.Text == "")
            {
                btnGetSelSource.Enabled = true;
                MessageBox.Show("Please select first Source Civil Feature!", "Duct Path Placement", MessageBoxButtons.OK, MessageBoxIcon.Information);
                GTDuctPathPlc.m_gtapp.SelectedObjects.Clear();
                //GTDuctPathPlc.startdraw = 7;
                return false;
            }
            if (txtTypeTerm.Text == "" || txtFIDTerm.Text == "" || txtWallTerm.Text == "")
            {
                btnGetSelTerm.Enabled = true;
                MessageBox.Show("Please select first Terminated Civil Feature!", "Duct Path Placement", MessageBoxButtons.OK, MessageBoxIcon.Information);
                GTDuctPathPlc.m_gtapp.SelectedObjects.Clear();
                //GTDuctPathPlc.startdraw = 7;
                return false;
            }
           // btnGetSelSource.Enabled = false;
            btnStartDraw.Enabled = true;
            return true;
        }
        #endregion

        #region Select Wall source
        private void btnWallSource_Click(object sender, EventArgs e)
        {
            if (txtTypeSource.Text == "Civil Node")
            {
                MessageBox.Show("Please select Manhole, Chamber or Tunnel first!", "Duct Path Placement", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                LocateFeature(1, GTDuctPathPlc.m_gtapp.ActiveMapWindow);
                this.Hide();
                GTDuctPathPlc.startdraw = 300;
            }
        }

        public bool GetDeviceWithWall( bool flag)
        {
            IGTGeometry geom = null;
            short iFNO = 0;
            int iFID = 0;
            string WallNum = "";
            short iFNOSelected = 0;
            int iFIDSelected = 0;
            string FeatureType = "";

            if (flag)
            {
                FeatureType = txtTypeSource.Text;
                iFIDSelected = int.Parse(txtFIDSource.Text);
            }
            else
            {
                FeatureType = txtTypeTerm.Text;
                iFIDSelected = int.Parse(txtFIDTerm.Text);
            }

            if (FeatureType == "Manhole")
                iFNOSelected = 2700;
            else if (FeatureType == "Chamber")
                iFNOSelected = 3800;
            else if (FeatureType == "Tunnel")
                iFNOSelected = 3300;
            else if (FeatureType == "Trench")
                iFNOSelected = 3300;
            else if (FeatureType == "Civil Node")
                iFNOSelected = 2800;

            #region check if selected allowed feature and if successshow detail
            if (GTDuctPathPlc.m_gtapp.SelectedObjects.FeatureCount == 0)
            {
                MessageBox.Show("Please select first Wall of selected " + FeatureType + " with FID = " + iFIDSelected.ToString() + " !", "Duct Path Placement", MessageBoxButtons.OK, MessageBoxIcon.Information);
                GTDuctPathPlc.m_gtapp.SelectedObjects.Clear();
                return false;
            }

            if (GTDuctPathPlc.m_gtapp.SelectedObjects.FeatureCount > 1)
            {
                MessageBox.Show("Please select first Wall of selected " + FeatureType + " with FID = " + iFIDSelected.ToString() + " !", "Duct Path Placement", MessageBoxButtons.OK, MessageBoxIcon.Information);
                GTDuctPathPlc.m_gtapp.SelectedObjects.Clear();
                return false;
            }


            foreach (IGTDDCKeyObject oDDCKeyObject in GTDuctPathPlc.m_gtapp.SelectedObjects.GetObjects())
            {
                geom = oDDCKeyObject.Geometry;
                iFNO = oDDCKeyObject.FNO;
                iFID = oDDCKeyObject.FID;
               
                if(iFNOSelected != iFNO)
                {
                    MessageBox.Show("Please select first Wall of selected " + FeatureType + " with FID = " + iFIDSelected.ToString() + " !", "Duct Path Placement", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    GTDuctPathPlc.m_gtapp.SelectedObjects.Clear();
                    return false;
                }
                if (iFIDSelected != iFID)
                {
                    MessageBox.Show("Please select first Wall of selected " + FeatureType + " with FID = " + iFIDSelected.ToString() + " !", "Duct Path Placement", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    GTDuctPathPlc.m_gtapp.SelectedObjects.Clear();
                    return false;
                }

                //manhole
                if (iFNO == 2700)
                {
                    if (oDDCKeyObject.ComponentViewName == "VGC_MANHLW_T")
                    {
                        for (int i = 0; i < oDDCKeyObject.Recordset.Fields.Count; i++)
                        {
                            if (oDDCKeyObject.Recordset.Fields[i].Name == "WALL_NUM")
                            {
                                WallNum = oDDCKeyObject.Recordset.Fields[i].Value.ToString();
                            }
                        }
                        break;
                    }
                }
                //chamber
                else if (iFNO == 3800)
                {
                    if (oDDCKeyObject.ComponentViewName == "VGC_CHAMBERWALL_T")
                    {
                        for (int i = 0; i < oDDCKeyObject.Recordset.Fields.Count; i++)
                        {
                            if (oDDCKeyObject.Recordset.Fields[i].Name == "WALL_NUM")
                            {
                                WallNum = oDDCKeyObject.Recordset.Fields[i].Value.ToString();
                            }
                        }
                        break;
                    }
                }
                //tunnel/trench
                else if (iFNO == 3300)
                {
                    if (oDDCKeyObject.ComponentViewName == "VGC_TUNNELWALL_T")
                    {
                        for (int i = 0; i < oDDCKeyObject.Recordset.Fields.Count; i++)
                        {
                            if (oDDCKeyObject.Recordset.Fields[i].Name == "WALL_NUM")
                            {
                                WallNum = oDDCKeyObject.Recordset.Fields[i].Value.ToString();
                            }
                        }
                        break;
                    }
                }

                MessageBox.Show("Please select first Wall of selected " + FeatureType + " with FID = " + iFIDSelected.ToString() + " !", "Duct Path Placement", MessageBoxButtons.OK, MessageBoxIcon.Information);
                GTDuctPathPlc.m_gtapp.SelectedObjects.Clear();
                return false;

            }
            #endregion
            if (flag)
            {
                txtWallSource.Text = WallNum;
                if (SourceDevicePoint == null)
                    SourceDevicePoint = GTClassFactory.Create<IGTPoint>();
                SourceDevicePoint.X = geom.FirstPoint.X;
                SourceDevicePoint.Y = geom.FirstPoint.Y;
                SourceDevicePoint.Z = geom.FirstPoint.Z;

                if (GTDuctPathPlc.StartDrawPoint == null)
                    GTDuctPathPlc.StartDrawPoint = GTClassFactory.Create<IGTPoint>();
                GTDuctPathPlc.StartDrawPoint.X = SourceDevicePoint.X;
                GTDuctPathPlc.StartDrawPoint.Y = SourceDevicePoint.Y;
                GTDuctPathPlc.StartDrawPoint.Z = SourceDevicePoint.Z;

                #region redraw first section of temporary geometry

                if (GTDuctPathPlc.mobjEditService != null)
                {
                    if (GTDuctPathPlc.mobjEditService.GeometryCount > 0)
                    {
                        GTDuctPathPlc.m_gtapp.SelectedObjects.Clear();

                        IGTPolylineGeometry oLineGeom = GTClassFactory.Create<IGTPolylineGeometry>();

                        oLineGeom.Points.Add(SourceDevicePoint);
                        for (int i = 1; i <= GTDuctPathPlc.mobjEditService.GeometryCount; i++)
                            oLineGeom.Points.Add(GTDuctPathPlc.mobjEditService.GetGeometry(i).LastPoint);

                        GTDuctPathPlc.StartDrawPoint.X = oLineGeom.Points[oLineGeom.Points.Count - 2].X;
                        GTDuctPathPlc.StartDrawPoint.Y = oLineGeom.Points[oLineGeom.Points.Count - 2].Y;
                        GTDuctPathPlc.StartDrawPoint.Z = oLineGeom.Points[oLineGeom.Points.Count - 2].Z;

                        GTDuctPathPlc.mobjEditService.RemoveAllGeometries();

                        for (int i = 0; i < oLineGeom.Points.Count - 1; i++)
                        {
                            IGTPolylineGeometry oLineGeom1 = GTClassFactory.Create<IGTPolylineGeometry>();
                            oLineGeom1.Points.Add(oLineGeom.Points[i]);
                            oLineGeom1.Points.Add(oLineGeom.Points[i + 1]);
                            GTDuctPathPlc.mobjEditService.AddGeometry(oLineGeom1, 14500);
                        }


                    }
                    else
                    {
                        if (GTDuctPathPlc.EndDrawPoint != null)
                        {
                            IGTPolylineGeometry oLineGeom = GTClassFactory.Create<IGTPolylineGeometry>();
                            oLineGeom.Points.Add(GTDuctPathPlc.StartDrawPoint);
                            oLineGeom.Points.Add(GTDuctPathPlc.EndDrawPoint);
                            GTDuctPathPlc.mobjEditService.AddGeometry(oLineGeom, 14500);
                        }
                    }
                }
                #endregion
            }
            else
            {
                txtWallTerm.Text = WallNum;
                if (TermDevicePoint == null)
                    TermDevicePoint = GTClassFactory.Create<IGTPoint>();
                TermDevicePoint.X = geom.FirstPoint.X;
                TermDevicePoint.Y = geom.FirstPoint.Y;
                TermDevicePoint.Z = geom.FirstPoint.Z;


                if (GTDuctPathPlc.EndDrawPoint == null)
                    GTDuctPathPlc.EndDrawPoint = GTClassFactory.Create<IGTPoint>();
                GTDuctPathPlc.EndDrawPoint = TermDevicePoint;
                GTDuctPathPlc.EndDrawPoint.X = TermDevicePoint.X;
                GTDuctPathPlc.EndDrawPoint.Y = TermDevicePoint.Y;
                GTDuctPathPlc.EndDrawPoint.Z = TermDevicePoint.Z;
               
                if (GTDuctPathPlc.mobjEditService != null)
                {
                    if (GTDuctPathPlc.StartDrawPoint!=null)//GTDuctPathPlc.mobjEditService.GeometryCount > 0)
                    {
                        GTDuctPathPlc.m_gtapp.SelectedObjects.Clear();
                        IGTPolylineGeometry oLineGeom = GTClassFactory.Create<IGTPolylineGeometry>();

                        oLineGeom.Points.Add(GTDuctPathPlc.StartDrawPoint);
                        oLineGeom.Points.Add(GTDuctPathPlc.EndDrawPoint);
                        if (GTDuctPathPlc.mobjEditService.GeometryCount > 0)
                            GTDuctPathPlc.mobjEditService.RemoveGeometry(GTDuctPathPlc.mobjEditService.GeometryCount);

                        GTDuctPathPlc.mobjEditService.AddGeometry(oLineGeom, 14500);
                    }
                }
            }
            GTDuctPathPlc.m_gtapp.SelectedObjects.Clear();
            return true;
        }
        #endregion
        #endregion

        #region Step 3

        #region Get Selected for Term device click
        private void btnGetSelTerm_Click(object sender, EventArgs e)
        {
            this.Hide();
            GTDuctPathPlc.m_gtapp.SelectedObjects.Clear();
            GTDuctPathPlc.startdraw = 600;        
           
        }
        public bool GetTermDevice(short iFNO, int iFID, IGTGeometry geom)
        {
               if (iFNO != 2700 && iFNO != 3800 && iFNO != 3300 && iFNO != 2800)
                {
                    MessageBox.Show("Please select only allowed Civil Feature: Manhole,Chamber,Tunnel or Civil Node!", "Duct Path Placement", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    GTDuctPathPlc.m_gtapp.SelectedObjects.Clear();
                    return false;
                }

                if (iFID.ToString() == txtFIDSource.Text)
                {
                    btnGetSelTerm.Enabled = true;
                    MessageBox.Show("Source and Terminate Civil Features should be different!", "Duct Path Placement", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    GTDuctPathPlc.m_gtapp.SelectedObjects.Clear();
                    return false;
                }

                //manhole
                if (iFNO == 2700)
                    {
                        txtWallTerm.Text = "";
                        txtFIDTerm.Text = iFID.ToString();
                        txtTypeTerm.Text = "Manhole";
                        txtMHIDTerm.Text = Get_Value("select MANHOLE_ID from GC_MANHL where g3e_fid = " + iFID.ToString());
                        lbMHIDTerm.Visible = true;
                        txtMHIDTerm.Visible = true;
                        btnWallTerm.Enabled = true;
                    }
                //chamber
                else if (iFNO == 3800)
                    {
                        txtWallTerm.Text = "";
                        txtFIDTerm.Text = iFID.ToString();
                        txtTypeTerm.Text = "Chamber";
                        lbMHIDTerm.Visible = false;
                        txtMHIDTerm.Visible = false;
                        txtMHIDTerm.Text = "";
                        btnWallTerm.Enabled = true;
                    }
                //tunnel
                    else if (iFNO == 3300)
                    {
                        txtWallTerm.Text = "";
                        txtFIDTerm.Text = iFID.ToString();
                        txtTypeTerm.Text = "Tunnel";
                        string type = Get_Value("select TRENCH from GC_TUNNEL where g3e_fid = " + iFID.ToString());
                        if (type == "N")
                            txtTypeTerm.Text = "Tunnel";
                        else txtTypeTerm.Text = "Trench";
                        lbMHIDTerm.Visible = false;
                        txtMHIDTerm.Visible = false;
                        txtMHIDTerm.Text = "";
                        btnWallTerm.Enabled = true;
                    }
                //civil node
                    else if (iFNO == 2800)
                {
                    txtWallTerm.Text = "0";
                    txtFIDTerm.Text = iFID.ToString();
                    txtTypeTerm.Text = "Civil Node";
                    lbMHIDTerm.Visible = false;
                    txtMHIDTerm.Visible = false;
                    txtMHIDTerm.Text = "";
                    btnWallTerm.Enabled = false; 

                    if (TermDevicePoint == null)
                        TermDevicePoint = GTClassFactory.Create<IGTPoint>();
                    TermDevicePoint.X = geom.FirstPoint.X;
                    TermDevicePoint.Y = geom.FirstPoint.Y;
                    TermDevicePoint.Z = geom.FirstPoint.Z;


                    if (GTDuctPathPlc.EndDrawPoint == null)
                        GTDuctPathPlc.EndDrawPoint = GTClassFactory.Create<IGTPoint>();
                    GTDuctPathPlc.EndDrawPoint = TermDevicePoint;
                    GTDuctPathPlc.EndDrawPoint.X = TermDevicePoint.X;
                    GTDuctPathPlc.EndDrawPoint.Y = TermDevicePoint.Y;
                    GTDuctPathPlc.EndDrawPoint.Z = TermDevicePoint.Z;
                }

                TermFID = iFID;
                TermFNO = iFNO;
            GTDuctPathPlc.m_gtapp.SelectedObjects.Clear();
            return true;
        }
        #endregion
        #region Select Wall term
        private void btnWallTerm_Click(object sender, EventArgs e)
        {
         if (txtTypeTerm.Text == "Civil Node")
            {
                MessageBox.Show("Please select Manhole, Chamber or Tunnel first!", "Duct Path Placement", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                LocateFeature(3, GTDuctPathPlc.m_gtapp.ActiveMapWindow);
                this.Hide();
                GTDuctPathPlc.startdraw = 400;
            }
        }
        #endregion
        #endregion

        #region Step 4 (confirmation or restarting)

        #region Confirm button for term device and whole line for conduit click
        private void btnConfirmTerm_Click(object sender, EventArgs e)
        {
            if (txtTypeTerm.Text == "" || txtFIDTerm.Text == "" || txtWallTerm.Text == "" ||
                txtTypeSource.Text == "" || txtFIDSource.Text == "" || txtWallSource.Text == "")
            {
                MessageBox.Show("Please select first Source and Terminated Civil Features!", "Duct Path Placement", MessageBoxButtons.OK, MessageBoxIcon.Information);
                GTDuctPathPlc.m_gtapp.SelectedObjects.Clear();
                return;
            }
            if (txtFIDTerm.Text == txtFIDSource.Text)
            {
                MessageBox.Show("Source and Terminate Civil Features should be different!", "Duct Path Placement", MessageBoxButtons.OK, MessageBoxIcon.Information);
                GTDuctPathPlc.m_gtapp.SelectedObjects.Clear();
                return;
            }
            if (GTDuctPathPlc.mobjEditService == null)
            {
                MessageBox.Show("Draw fist graphic geometry!", "Duct Path Placement", MessageBoxButtons.OK, MessageBoxIcon.Information);
                GTDuctPathPlc.m_gtapp.SelectedObjects.Clear();
                return;
            }
            else if(GTDuctPathPlc.mobjEditService.GeometryCount==0) 
            {
                MessageBox.Show("Draw fist graphic geometry!", "Duct Path Placement", MessageBoxButtons.OK, MessageBoxIcon.Information);
                GTDuctPathPlc.m_gtapp.SelectedObjects.Clear();
                return;
            }

            DuctPathLineGeom = GTClassFactory.Create<IGTPolylineGeometry>();
            IGTGeometry geom = GTClassFactory.Create<IGTGeometry>();
            for (int i = 1; i <= GTDuctPathPlc.mobjEditService.GeometryCount; i++)
            {
                geom = GTDuctPathPlc.mobjEditService.GetGeometry(i);
                DuctPathLineGeom.Points.Add(geom.FirstPoint);
            }
            DuctPathLineGeom.Points.Add(GTDuctPathPlc.mobjEditService.GetGeometry(GTDuctPathPlc.mobjEditService.GeometryCount).LastPoint);
            GTDuctPathPlc.mobjEditService.RemoveAllGeometries();
            GTDuctPathPlc.mobjEditService.AddGeometry(DuctPathLineGeom, 12000);

             //filling Page 2
            if (NumberOfConduit == 1)
                FillingPage2ComboBoxes();
            FillingPage2Attr();
            statusPage = 1;
            LocateFeature(2, GTDuctPathPlc.m_gtapp.ActiveMapWindow);
            btnConfirmDuctAttr.Enabled = true;
            tabDuctPlc.SelectedTab = tabPageAttr;
        }
        #endregion
        
        #region Button Close1 click
        private void btnClose1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        #endregion
 
        #region Button Re-Start click
        private void btnReStart_Click(object sender, EventArgs e)
        {
            DialogResult retVal = MessageBox.Show(" Are you sure that you want to start from beginning \n and discard all changes?", "Duct Path Placement", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (retVal == DialogResult.Yes)
            {
                if (GTDuctPathPlc.mobjEditService != null)
                    GTDuctPathPlc.mobjEditService.RemoveAllGeometries();
                GTDuctPathPlc.m_gtapp.SelectedObjects.Clear();
                tabDuctPlc.SelectedTab = tabPagePLC;
                LastSection = 0;
                if(SectSlashes!=null)
                    SectSlashes.Clear();
                if (SectLabels != null)
                    SectLabels.Clear();
                if (Sections != null)
                    Sections.Clear();
                SourceDevicePoint = null;
                TermDevicePoint = null;
                GTDuctPathPlc.StartDrawPoint = null;
                GTDuctPathPlc.EndDrawPoint = null;
                if (DuctPathLineGeom != null)
                {
                    DuctPathLineGeom.Points.Clear();
                    DuctPathLineGeom = null;
                }
                btnGetSelSource.Enabled = true;
                btnGetSelTerm.Enabled = true;
                btnStartDraw.Enabled = true;
                txtFIDSource.Text = "";
                txtFIDTerm.Text = "";
                txtTypeSource.Text = "";
                txtTypeTerm.Text = "";
                txtWallSource.Text = "";
                txtWallTerm.Text = "";
                gbSectValues.Enabled = true;
                gbDuctPathAttrValues.Enabled = true;
                txtSectionLength.Enabled = true;
                this.Hide();
                GTDuctPathPlc.startdraw = 100;
            }
        
        }
        #endregion

        #endregion

        #region Step 2

        #region start draw button
        private void btnStartDraw_Click(object sender, EventArgs e)
        {
            if (ConfirmSource())
            {
                if (GTDuctPathPlc.StartDrawPoint == null)
                    GTDuctPathPlc.StartDrawPoint = GTClassFactory.Create<IGTPoint>();
                GTDuctPathPlc.StartDrawPoint.X = SourceDevicePoint.X;
                GTDuctPathPlc.StartDrawPoint.Y = SourceDevicePoint.Y;
                GTDuctPathPlc.StartDrawPoint.Z = SourceDevicePoint.Z;

                IGTPolylineGeometry oLineGeom = GTClassFactory.Create<IGTPolylineGeometry>();
                oLineGeom.Points.Add(GTDuctPathPlc.StartDrawPoint);
                oLineGeom.Points.Add(GTDuctPathPlc.EndDrawPoint);
                if (GTDuctPathPlc.mobjEditService.GeometryCount > 0)
                    GTDuctPathPlc.mobjEditService.RemoveAllGeometries();
                GTDuctPathPlc.mobjEditService.AddGeometry(oLineGeom, 14500);
                LocateFeature(2, GTDuctPathPlc.m_gtapp.ActiveMapWindow);
                this.Hide();
               // GTDuctPathPlc.m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "LEFT MOUSE CLICK TO CONFIRM POINT ON MAP, DOUBLE MOUSE CLICK TO FINISHED DRAWING");
                GTDuctPathPlc.m_gtapp.SelectedObjects.Clear();
                GTDuctPathPlc.startdraw = 101;
            }
        }
        #endregion

        #endregion

        #endregion
       
        #region Page2 Attricutes for Duct Path
        
        #region Filling text and list boxes for conduit attr
        private void FillingPage2Attr()
        {
            if (cbNumDuctWays.DataSource != null)
            {
                int i = 0;
                for (; i < cbNumDuctWays.Items.Count; i++)
                {
                    cbNumDuctWays.SelectedItem = cbNumDuctWays.Items[i];
                    if (cbNumDuctWays.SelectedValue.ToString() == "6")
                        break;
                }
                if (i == cbNumDuctWays.Items.Count)
                    cbNumDuctWays.SelectedIndex = 0;
            }

            txtDuctSourceFID.Text = txtFIDSource.Text;
            txtDuctSourceType.Text = txtTypeSource.Text;
            txtDuctSourceWall.Text = txtWallSource.Text;
            txtDuctTermFID.Text = txtFIDTerm.Text;
            txtDuctTermType.Text = txtTypeTerm.Text;
            txtDuctTermWall.Text = txtWallTerm.Text;
            txtYearIns.Text = DateTime.Now.Year.ToString();

            string EXC_ABB = Get_Value("select EXC_ABB from GC_NETELEM where G3E_FNO= " + SourceFNO.ToString() +
                " AND G3E_FID = " + SourceFID.ToString());
            if (EXC_ABB=="")
                EXC_ABB = Get_Value("select EXC_ABB from GC_NETELEM where G3E_FNO= " +TermFNO.ToString()+
                " AND G3E_FID = " + TermFID.ToString());
            txtExcAbb.Text = EXC_ABB;//"***";
            txtFlag.Text = "0";
            TotalGraphicLength = LengthConduit();
            txtTotalLength.Text = TotalGraphicLength.ToString();
            if (cbConstructed.DataSource != null)
            {
               // cbConstructed.SelectedText = "TM";
                int i = 0;
                for (; i < cbConstructed.Items.Count; i++)
                {
                    cbConstructed.SelectedItem = cbConstructed.Items[i];
                    if (cbConstructed.SelectedValue.ToString().ToUpper().Contains("TM") || cbConstructed.SelectedValue.ToString().ToUpper().Contains("TELEKOM"))
                        break;
                }
                if (i==cbConstructed.Items.Count)
                    cbConstructed.SelectedIndex = 0;
            }

            if (cbBillingRate.DataSource != null)
            {
                // cbConstructed.SelectedText = "TM";
                int i = 0;
                for (; i < cbBillingRate.Items.Count; i++)
                {
                    cbBillingRate.SelectedItem = cbBillingRate.Items[i];
                    if (cbBillingRate.SelectedValue.ToString().ToUpper().Contains("DAY"))
                        break;
                }
                if (i == cbBillingRate.Items.Count)
                    cbBillingRate.SelectedIndex = 0;
            }
            
            GTDuctPathPlc.startdraw = 11;
         //   GTDuctPathPlc.m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "PRESS 'Confirm' BUTTON FOR CONFIRMATION OF ENTERED ATTRIBUTES FOR CONDUIT");
               
        }

        private void FillingPage2ComboBoxes()
        {

            if (cbConstructed.DataSource == null)
            {
                ADODB.Recordset rsPP = new ADODB.Recordset();
                string sSql = "SELECT PL_VALUE FROM REF_COM_CONSTRUCTION";
                rsPP = GTDuctPathPlc.m_IGTDataContext.OpenRecordset(sSql, ADODB.CursorTypeEnum.adOpenStatic, ADODB.LockTypeEnum.adLockReadOnly, 1);
                cbConstructed.Items.Clear();
                if (rsPP.RecordCount > 0)
                {
                    List<ComboBoxItems> item = new List<ComboBoxItems>();

                    rsPP.MoveFirst();
                    for (int i = 0; i < rsPP.RecordCount; i++)
                    {
                        item.Add(new ComboBoxItems(rsPP.Fields[0].Value.ToString(), rsPP.Fields[0].Value.ToString()));
                        rsPP.MoveNext();
                    }
                    cbConstructed.DataSource = item;
                    cbConstructed.DisplayMember = "Text";
                    cbConstructed.ValueMember = "Value";
                }
            }

            if (cbNumDuctWays.DataSource == null)
            {
                cbNumDuctWays.Items.Clear();
                string sSql = " select distinct DT_S_WAYS from ref_civ_ductpath  order by to_number(DT_S_WAYS) asc ";
                ADODB.Recordset rsPP = GTDuctPathPlc.m_IGTDataContext.OpenRecordset(sSql, ADODB.CursorTypeEnum.adOpenStatic, ADODB.LockTypeEnum.adLockReadOnly, 1);

                if (rsPP.RecordCount > 0)
                {
                    List<ComboBoxItems> item1 = new List<ComboBoxItems>();

                    rsPP.MoveFirst();
                    for (int i = 0; i < rsPP.RecordCount; i++)
                    {
                        item1.Add(new ComboBoxItems(rsPP.Fields[0].Value.ToString(), rsPP.Fields[0].Value.ToString()));
                        rsPP.MoveNext();
                    }
                    cbNumDuctWays.DataSource = item1;
                    cbNumDuctWays.DisplayMember = "Text";
                    cbNumDuctWays.ValueMember = "Value";
                }
            }

            if (cbBillingRate.DataSource == null)
            {
                ADODB.Recordset rsPP = new ADODB.Recordset();
                string sSql = "select pl_value, pl_num from  REF_COM_BILLRATE";
                rsPP = GTDuctPathPlc.m_IGTDataContext.OpenRecordset(sSql, ADODB.CursorTypeEnum.adOpenStatic, ADODB.LockTypeEnum.adLockReadOnly, 1);
                cbBillingRate.Items.Clear();
                if (rsPP.RecordCount > 0)
                {
                    List<ComboBoxItems> item = new List<ComboBoxItems>();

                    rsPP.MoveFirst();
                    for (int i = 0; i < rsPP.RecordCount; i++)
                    {
                        item.Add(new ComboBoxItems(rsPP.Fields[0].Value.ToString(), rsPP.Fields[0].Value.ToString()));
                        rsPP.MoveNext();
                    }
                    cbBillingRate.DataSource = item;
                    cbBillingRate.DisplayMember = "Text";
                    cbBillingRate.ValueMember = "Value";
                }
            }
        }

        #endregion

        #region button Confirm for attr click
        private void btnConfirmDuctAttr_Click(object sender, EventArgs e)
        {
            if (ValidateDuctPathAttr())
            {
                gbDuctPathAttrValues.Enabled = false;
                btnConfirmDuctAttr.Enabled = false;

               // if(NumberOfConduit==1)
                    FillingPage3ComboBoxes();
                   // FillingPage3Sect();
                   //gbSectValues.Enabled = false;
                btnConfirmSectAttr.Enabled = false;   
                if (Sections == null)
                    Sections = new List<DuctPathSect>();
                if (SectSlashes == null)
                    SectSlashes = new List<SectSlash>();
                //GTDuctPathPlc.m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "LEFT MOUSE CLICK TO PLACE TEMPORARY SLASH POINT ON MAP, DOUBLE MOUSE CLICK TO CONFIRM LOCATION OF SLASH POINT, Right click to cancel placement of slash!");
                GTDuctPathPlc.m_gtapp.SelectedObjects.Clear();
                this.Hide();
                GTDuctPathPlc.startdraw = 2;
                gbDuctPathAttrValues.Enabled = true;
               // btnConfirmSectAttr.Enabled = true;
                statusPage = 2;
                tabDuctPlc.SelectedTab = tabPageSect;
            }
        }
        #endregion

        #region validation for enter values
        private bool ValidateDuctPathAttr()
        {
            int Len = 0;
            if (!int.TryParse(cbNumDuctWays.SelectedValue.ToString(), out Len))
            {
                MessageBox.Show("Number of Ducts (DuctWays) should be integer number!", "Duct Path Placement", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            else if (Len == 0)
            {
                MessageBox.Show("Number of duct ways can not be equal 0!", "Duct Path Placement", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            
            if (!int.TryParse(txtTotalLength.Text, out Len))
            {
                MessageBox.Show("Total Length should be integer number!", "Duct Path Placement", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            } if (Len == 0)
            {
                MessageBox.Show("Total Length can not be equal 0!", "Duct Path Placement", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            if (Sections != null)
            {
                if (Sections.Count > 0)
                {
                    if (TotalEnteredLength != Len)
                    {
                        DialogResult retVal = MessageBox.Show("Duct Path has Sections!\nTo modify Total Length of Duct Path all sections will be deleted!\nContinue?", "Duct Path Placement", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (retVal == DialogResult.Yes)
                        {
                            Sections.Clear();
                            if (SectSlashes != null)
                                SectSlashes.Clear();
                            if (SectLabels != null)
                                SectLabels.Clear();
                            GTDuctPathPlc.mobjEditService.RemoveAllGeometries();
                            GTDuctPathPlc.mobjEditService.AddGeometry(DuctPathLineGeom, 12000);
                        }
                        else
                        {
                            txtTotalLength.Text = TotalEnteredLength.ToString();
                            return false;
                        }
                    }

                    if (Sections[0].NumDuctWaysSect != cbNumDuctWays.SelectedValue.ToString())
                    {
                        DialogResult retVal = MessageBox.Show("Duct Path has Sections!\nTo modify Number of Duct Way of Duct Path all sections will be deleted!\nContinue?", "Duct Path Placement", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (retVal == DialogResult.Yes)
                        {
                            Sections.Clear();
                            if (SectSlashes != null)
                                SectSlashes.Clear();
                            if (SectLabels != null)
                                SectLabels.Clear();
                            GTDuctPathPlc.mobjEditService.RemoveAllGeometries();
                            GTDuctPathPlc.mobjEditService.AddGeometry(DuctPathLineGeom, 12000);
                        }
                        else
                        {
                            int i = 0;
                            for (; i < cbNumDuctWays.Items.Count; i++)
                            {
                                cbNumDuctWays.SelectedItem = cbNumDuctWays.Items[i];
                                if (cbNumDuctWays.SelectedValue.ToString() == Sections[0].NumDuctWaysSect)
                                    break;
                            }
                            if (i == cbNumDuctWays.Items.Count)
                                cbNumDuctWays.SelectedIndex = 0;
                            return false;
                        }
                    }
                }

            }
            if (Len != TotalGraphicLength)
            {
                DialogResult closetype = MessageBox.Show("Entered Length of conduit is different from graphic length!\nContinue with entered length?", "Duct Path Placement", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (closetype == DialogResult.No)
                        {
                            txtTotalLength.Text = TotalGraphicLength.ToString();
                            return false;
                        }
                        else
                        {
                            TotalEnteredLength = Len;
                            return true;
                        }
            }
            
            TotalEnteredLength = TotalGraphicLength;
            return true;
        }
        #endregion
        
        #region Button close2 click
        private void btnClose2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        #endregion 
        
        #region Button back to graphic
        private void btnBackToGraph_Click(object sender, EventArgs e)
        {
            if (Sections != null)
            {
                if (Sections.Count > 0)
                {
                    DialogResult retVal = MessageBox.Show("Placed Sections for Duct Path will be deleted!\nContinue?", "Duct Path Placement", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (retVal == DialogResult.Yes)
                    {
                        Sections.Clear();
                        if (SectSlashes != null)
                            SectSlashes.Clear();
                        if (SectLabels != null)
                            SectLabels.Clear();
                    }
                    else return;
                }
                
            }
            #region redraw first section of temporary geometry

            if (GTDuctPathPlc.mobjEditService != null)
            {
                if (GTDuctPathPlc.mobjEditService.GeometryCount > 0)
                {
                    GTDuctPathPlc.m_gtapp.SelectedObjects.Clear();

                    GTDuctPathPlc.StartDrawPoint.X = DuctPathLineGeom.Points[DuctPathLineGeom.Points.Count - 2].X;
                    GTDuctPathPlc.StartDrawPoint.Y = DuctPathLineGeom.Points[DuctPathLineGeom.Points.Count - 2].Y;
                    GTDuctPathPlc.StartDrawPoint.Z = DuctPathLineGeom.Points[DuctPathLineGeom.Points.Count - 2].Z;

                    GTDuctPathPlc.mobjEditService.RemoveAllGeometries();

                    for (int i = 0; i < DuctPathLineGeom.Points.Count - 1; i++)
                    {
                        IGTPolylineGeometry oLineGeom1 = GTClassFactory.Create<IGTPolylineGeometry>();
                        oLineGeom1.Points.Add(DuctPathLineGeom.Points[i]);
                        oLineGeom1.Points.Add(DuctPathLineGeom.Points[i + 1]);
                        GTDuctPathPlc.mobjEditService.AddGeometry(oLineGeom1, 14500);
                    }
                    DuctPathLineGeom.Points.Clear();
                    DuctPathLineGeom = null;

                }
            }
            #endregion
            
            LocateFeature(2, GTDuctPathPlc.m_gtapp.ActiveMapWindow);
            btnConfirmDuctAttr.Enabled = false;
            statusPage = 0;
            tabDuctPlc.SelectedTab = tabPagePLC;

        }
        #endregion  
        #endregion     
        
        #region Page 3 Section
  
        #region Button close3 click
        private void btnClose3_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        #endregion 

        #region Back button from sections to attributes
        private void btnBack2_Click(object sender, EventArgs e)
        {
            if (btnFinished.Enabled == true)
            {
                DialogResult retVal = MessageBox.Show("Last Sections for Duct Path will be deleted!\nContinue?", "Duct Path Placement", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (retVal == DialogResult.Yes)
                {
                    Sections.RemoveAt(Sections.Count - 1);
                    if(SectLabels[SectLabels.Count - 1].LeaderLine!=null)
                        GTDuctPathPlc.mobjEditService.RemoveGeometry(GTDuctPathPlc.mobjEditService.GeometryCount);
                    GTDuctPathPlc.mobjEditService.RemoveGeometry(GTDuctPathPlc.mobjEditService.GeometryCount);
                    SectLabels.RemoveAt(SectLabels.Count - 1);
                    btnFinished.Enabled = false;
                }
                else return;

            }
            if (SectSlashes != null)
            {
                if (SectSlashes.Count > 0 && LastSection == 0)
                {
                    DialogResult retVal = MessageBox.Show("Last placed Slash Sections for Duct Path will be deleted!\nContinue?", "Duct Path Placement", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (retVal == DialogResult.Yes)
                    {
                        SectSlashes.RemoveAt(SectSlashes.Count - 1);
                        GTDuctPathPlc.mobjEditService.RemoveGeometry(GTDuctPathPlc.mobjEditService.GeometryCount);
                    }
                    else return;
                }

            }
            LastSection = 0;
            btnConfirmSectAttr.Enabled = false;
            btnConfirmDuctAttr.Enabled = true;
            gbSectValues.Enabled = true; 
            statusPage = 1;
            tabDuctPlc.SelectedTab = tabPageAttr;
        }
        #endregion

        #region filling tex and combo boxes for section attr
        private void FillingPage3Sect()
        {
            
            if (Sections == null)
                Sections = new List<DuctPathSect>();
            if (SectSlashes == null)
                SectSlashes = new List<SectSlash>();
            if (SectSlashes.Count == 0)
                txtSectionLength.Text = txtTotalLength.Text;
            else
            {
                int length = TotalGraphicLength;
                if (LastSection == 1 || LastSection == 2)
                {
                    length -= SectSlashes[SectSlashes.Count - 1].length;
                }
                else
                {
                    int len1 = 0;
                    int len2 = 0;
                    if (SectSlashes.Count == 1)
                        len2 = SectSlashes[SectSlashes.Count - 1].length;
                    else if (SectSlashes.Count > 1)
                    {
                        len2 = SectSlashes[SectSlashes.Count - 1].length;
                        len1 = SectSlashes[SectSlashes.Count - 2].length;
                    }
                    length = len2 - len1;
                }
                if (TotalGraphicLength != TotalEnteredLength)
                {
                    if (TotalGraphicLength == 0) TotalGraphicLength = 1;
                    length = TotalEnteredLength * length / TotalGraphicLength;
                }
              txtSectionLength.Text = length.ToString();
            }
            gbSectValues.Enabled = true;
            txtYearExpanded.Text = "0";
            txtYearExtended.Text = "0";
            txtNumDuctWaysSect.Text=cbNumDuctWays.SelectedValue.ToString();
             
            if (cbSectOwner.DataSource != null)
            {
                // cbConstructed.SelectedText = "TM";
                int i = 0;
                for (; i < cbSectOwner.Items.Count; i++)
                {
                    cbSectOwner.SelectedItem = cbSectOwner.Items[i];
                    if (cbSectOwner.SelectedValue.ToString().ToUpper().Contains("TM"))
                        break;
                }
                if (i == cbSectOwner.Items.Count)
                    cbSectOwner.SelectedIndex = 0;
            }
            if (cbSectBillingRate.DataSource != null)
            {
                // cbConstructed.SelectedText = "TM";
                int i = 0;
                for (; i < cbSectBillingRate.Items.Count; i++)
                {
                    cbSectBillingRate.SelectedItem = cbSectBillingRate.Items[i];
                    if (cbSectBillingRate.SelectedValue.ToString().ToUpper().Contains("DAY"))
                        break;
                }
                if (i == cbSectBillingRate.Items.Count)
                    cbSectBillingRate.SelectedIndex = 0;
            }
            mcbMinMaterial.Text = "";// "PVC_6_GRASSVERGE_N_Y_106";
            //if (cbSectDiam.DataSource != null)
            //{
            //    // cbConstructed.SelectedText = "TM";
            //    int i = 0;
            //    for (; i < cbSectDiam.Items.Count; i++)
            //    {
            //        cbSectDiam.SelectedItem = cbSectDiam.Items[i];
            //        if (cbSectDiam.SelectedValue.ToString()=="106")
            //            break;
            //    }
            //    if (i == cbSectDiam.Items.Count)
            //        cbSectDiam.SelectedIndex = 0;
            //}  
            //if (cbSectType.DataSource != null)
            //{
            //    // cbConstructed.SelectedText = "TM";
            //    int i = 0;
            //    for (; i < cbSectType.Items.Count; i++)
            //    {
            //        cbSectType.SelectedItem = cbSectType.Items[i];
            //        if (cbSectType.SelectedValue.ToString().ToUpper().Contains("PVC"))
            //            break;
            //    }
            //    if (i == cbSectType.Items.Count)
            //        cbSectType.SelectedIndex = 0;
            //}

            //if (cbSectPlc.DataSource != null)
            //{
            //    // cbConstructed.SelectedText = "TM";
            //    int i = 0;
            //    for (; i < cbSectPlc.Items.Count; i++)
            //    {
            //        cbSectPlc.SelectedItem = cbSectPlc.Items[i];
            //        if (cbSectPlc.SelectedValue.ToString().ToUpper().Contains("GRASSVERGE"))
            //            break;
            //    }
            //    if (i == cbSectPlc.Items.Count)
            //        cbSectPlc.SelectedIndex = 0;
            //}           

            //if (cbEncasement.DataSource != null)
            //{
            //    // cbConstructed.SelectedText = "TM";
            //    int i = 0;
            //    for (; i < cbEncasement.Items.Count; i++)
            //    {
            //        cbEncasement.SelectedItem = cbEncasement.Items[i];
            //        if (cbEncasement.SelectedValue.ToString().ToUpper().Contains("N"))
            //            break;
            //    }
            //    if (i == cbEncasement.Items.Count)
            //        cbEncasement.SelectedIndex = 0;
            //}

            //if (cbSectBackFill.DataSource != null)
            //{
            //    // cbConstructed.SelectedText = "TM";
            //    int i = 0;
            //    for (; i < cbSectBackFill.Items.Count; i++)
            //    {
            //        cbSectBackFill.SelectedItem = cbSectBackFill.Items[i];
            //        if (cbSectBackFill.SelectedValue.ToString().ToUpper().Contains("Y"))
            //            break;
            //    }
            //    if (i == cbSectBackFill.Items.Count)
            //        cbSectBackFill.SelectedIndex = 0;
            //}


        }
        #region filtering
        public void FilteringMinMaterial()
        {
            string sql = "select MIN_MATERIAL from ref_civ_ductpath where " +
                " DT_S_TYPE ='" + cbSectType.SelectedValue.ToString() + "' and " +
                " DT_S_WAYS='" + txtNumDuctWaysSect.Text + "' and " +
                " DT_S_PLACMNT='" + cbSectPlc.SelectedValue.ToString() + "' and " +
                " DT_S_ENCASE='" + cbEncasement.SelectedValue.ToString() + "' and " +
                " DT_S_BACKFILL='" + cbSectBackFill.SelectedValue.ToString() + "' and " +
                " DT_S_DIAMETER='"+cbSectDiam.SelectedValue.ToString()+"' ";
       //     mcbMinMaterial.Text = Get_Value(sql);
        }

        public void FilteringAttributes(string minmat)
        {
            string sSql="select DT_S_TYPE, DT_S_WAYS, DT_S_PLACMNT, DT_S_ENCASE, DT_S_BACKFILL, DT_S_DIAMETER  from ref_civ_ductpath "+
                        " where MIN_MATERIAL='" + minmat+"'";
            ADODB.Recordset rsPP = new ADODB.Recordset();
             rsPP = GTDuctPathPlc.m_IGTDataContext.OpenRecordset(sSql, ADODB.CursorTypeEnum.adOpenStatic, ADODB.LockTypeEnum.adLockReadOnly, 1);

             if (rsPP.RecordCount <= 0)
             {
                 return;
             }

             rsPP.MoveFirst();
            if (cbSectType.DataSource != null)
            {
                // cbConstructed.SelectedText = "TM";
                int i = 0;
                for (; i < cbSectType.Items.Count; i++)
                {
                    cbSectType.SelectedItem = cbSectType.Items[i];
                    if (cbSectType.SelectedValue.ToString().ToUpper().Contains(rsPP.Fields[0].Value.ToString()))
                        break;
                }
                if (i == cbSectType.Items.Count)
                    cbSectType.SelectedIndex = 0;
            }
                        //rsPP.Fields[0].Value.ToString()
            

            if (cbSectPlc.DataSource != null)
            {
                // cbConstructed.SelectedText = "TM";
                int i = 0;
                for (; i < cbSectPlc.Items.Count; i++)
                {
                    cbSectPlc.SelectedItem = cbSectPlc.Items[i];
                    if (cbSectPlc.SelectedValue.ToString().ToUpper().Contains(rsPP.Fields[2].Value.ToString()))
                        break;
                }
                if (i == cbSectPlc.Items.Count)
                    cbSectPlc.SelectedIndex = 0;
            }

            if (cbEncasement.DataSource != null)
            {
                // cbConstructed.SelectedText = "TM";
                int i = 0;
                for (; i < cbEncasement.Items.Count; i++)
                {
                    cbEncasement.SelectedItem = cbEncasement.Items[i];
                    if (cbEncasement.SelectedValue.ToString().ToUpper().Contains(rsPP.Fields[3].Value.ToString()))
                        break;
                }
                if (i == cbEncasement.Items.Count)
                    cbEncasement.SelectedIndex = 0;
            }

            if (cbSectBackFill.DataSource != null)
            {
                // cbConstructed.SelectedText = "TM";
                int i = 0;
                for (; i < cbSectBackFill.Items.Count; i++)
                {
                    cbSectBackFill.SelectedItem = cbSectBackFill.Items[i];
                    if (cbSectBackFill.SelectedValue.ToString().ToUpper().Contains(rsPP.Fields[4].Value.ToString()))
                        break;
                }
                if (i == cbSectBackFill.Items.Count)
                    cbSectBackFill.SelectedIndex = 0;
            }
            if (cbSectDiam.DataSource != null)
            {
                // cbConstructed.SelectedText = "TM";
                int i = 0;
                for (; i < cbSectDiam.Items.Count; i++)
                {
                    cbSectDiam.SelectedItem = cbSectDiam.Items[i];
                    if (cbSectDiam.SelectedValue.ToString().ToUpper()==rsPP.Fields[5].Value.ToString())
                        break;
                }
                if (i == cbSectDiam.Items.Count)
                    cbSectDiam.SelectedIndex = 0;
            }

        }

        #endregion
        private void mcbMinMaterial_TextChanged(object sender, EventArgs e)
        {
            if (mcbMinMaterial.Text != "")
                FilteringAttributes(mcbMinMaterial.Text);
        }

        private void FillingPage3ComboBoxes()
        {
            ADODB.Recordset rsPP = new ADODB.Recordset();
            string sSql = "";

            if (cbSectOwner.DataSource == null)
            {
                cbSectOwner.Items.Clear();
                sSql = "SELECT PL_VALUE FROM REF_COM_CONSTRUCTION";
                rsPP = GTDuctPathPlc.m_IGTDataContext.OpenRecordset(sSql, ADODB.CursorTypeEnum.adOpenStatic, ADODB.LockTypeEnum.adLockReadOnly, 1);

                if (rsPP.RecordCount > 0)
                {
                    List<ComboBoxItems> item = new List<ComboBoxItems>();

                    rsPP.MoveFirst();
                    for (int i = 0; i < rsPP.RecordCount; i++)
                    {
                        item.Add(new ComboBoxItems(rsPP.Fields[0].Value.ToString(), rsPP.Fields[0].Value.ToString()));
                        rsPP.MoveNext();
                    }
                    cbSectOwner.DataSource = item;
                    cbSectOwner.DisplayMember = "Text";
                    cbSectOwner.ValueMember = "Value";

                }
            }

            
          
                mcbMinMaterial.Items.Clear();
                sSql = "select MIN_MATERIAL, DT_S_TYPE, DT_S_WAYS, DT_S_PLACMNT, DT_S_ENCASE, DT_S_BACKFILL, DT_S_DIAMETER  from ref_civ_ductpath where DT_S_WAYS='" + cbNumDuctWays.SelectedValue.ToString() + "' order by DT_S_TYPE";
                rsPP = GTDuctPathPlc.m_IGTDataContext.OpenRecordset(sSql, ADODB.CursorTypeEnum.adOpenStatic, ADODB.LockTypeEnum.adLockReadOnly, 1);

                if (rsPP.RecordCount > 0)
                {
                    DataTable dtable = new DataTable();
                    //set columns names
                   dtable.Columns.Add("MIN_MATERIAL", typeof(System.String));
                    dtable.Columns.Add("TYPE", typeof(System.String));
                    dtable.Columns.Add("DUCTWAYS", typeof(System.String));
                    dtable.Columns.Add("PLACEMENT", typeof(System.String));
                    dtable.Columns.Add("ENCASEMENT", typeof(System.String));
                    dtable.Columns.Add("BACKFILL", typeof(System.String));
                    dtable.Columns.Add("DIAMETER", typeof(System.String));


                   
                    rsPP.MoveFirst();
                    for (int i = 0; i < rsPP.RecordCount; i++)
                    {
                        //Add Rows
                        DataRow drow = dtable.NewRow();
                        drow["MIN_MATERIAL"] = rsPP.Fields[0].Value.ToString();
                        drow["TYPE"] = rsPP.Fields[1].Value.ToString();
                        drow["DUCTWAYS"] = rsPP.Fields[2].Value.ToString();
                        drow["PLACEMENT"] = rsPP.Fields[3].Value.ToString();
                        drow["ENCASEMENT"] = rsPP.Fields[4].Value.ToString();
                        drow["BACKFILL"] = rsPP.Fields[5].Value.ToString();
                        drow["DIAMETER"] = rsPP.Fields[6].Value.ToString();
                        dtable.Rows.Add(drow);
                        rsPP.MoveNext();
                    }
                    mcbMinMaterial.Table = dtable;
                    mcbMinMaterial.DisplayMember = "MIN_MATERIAL";
                    mcbMinMaterial.ColumnsToDisplay = new string[] { "MIN_MATERIAL", "TYPE", "DUCTWAYS", "PLACEMENT", "ENCASEMENT", "BACKFILL", "DIAMETER" };

                
            }
         
            if (cbSectType.DataSource == null)
            {
                cbSectType.Items.Clear();
                sSql = " select distinct DT_S_TYPE from ref_civ_ductpath where DT_S_WAYS='" + cbNumDuctWays.SelectedValue.ToString() + "'";
                rsPP = GTDuctPathPlc.m_IGTDataContext.OpenRecordset(sSql, ADODB.CursorTypeEnum.adOpenStatic, ADODB.LockTypeEnum.adLockReadOnly, 1);

                if (rsPP.RecordCount > 0)
                {
                    List<ComboBoxItems> item = new List<ComboBoxItems>();

                    rsPP.MoveFirst();
                    for (int i = 0; i < rsPP.RecordCount; i++)
                    {
                        item.Add(new ComboBoxItems(rsPP.Fields[0].Value.ToString(), rsPP.Fields[0].Value.ToString()));
                        rsPP.MoveNext();
                    }
                    cbSectType.DataSource = item;
                    cbSectType.DisplayMember = "Text";
                    cbSectType.ValueMember = "Value";
                }
            }

            if (cbSectPlc.DataSource == null)
            {
                cbSectPlc.Items.Clear();
                sSql = " select distinct DT_S_PLACMNT from ref_civ_ductpath where DT_S_WAYS='" + cbNumDuctWays.SelectedValue.ToString() + "'";
                rsPP = GTDuctPathPlc.m_IGTDataContext.OpenRecordset(sSql, ADODB.CursorTypeEnum.adOpenStatic, ADODB.LockTypeEnum.adLockReadOnly, 1);

                if (rsPP.RecordCount > 0)
                {
                    List<ComboBoxItems> item = new List<ComboBoxItems>();

                    rsPP.MoveFirst();
                    for (int i = 0; i < rsPP.RecordCount; i++)
                    {
                        item.Add(new ComboBoxItems(rsPP.Fields[0].Value.ToString(), rsPP.Fields[0].Value.ToString()));
                        rsPP.MoveNext();
                    }
                    cbSectPlc.DataSource = item;
                    cbSectPlc.DisplayMember = "Text";
                    cbSectPlc.ValueMember = "Value";
                }
            }

            if (cbSectBillingRate.DataSource == null)
            {
                cbSectBillingRate.Items.Clear();
                sSql = "select pl_value, pl_num from  REF_COM_BILLRATE";
                rsPP = GTDuctPathPlc.m_IGTDataContext.OpenRecordset(sSql, ADODB.CursorTypeEnum.adOpenStatic, ADODB.LockTypeEnum.adLockReadOnly, 1);

                if (rsPP.RecordCount > 0)
                {
                    List<ComboBoxItems> item = new List<ComboBoxItems>();

                    rsPP.MoveFirst();
                    for (int i = 0; i < rsPP.RecordCount; i++)
                    {
                        item.Add(new ComboBoxItems(rsPP.Fields[0].Value.ToString(), rsPP.Fields[0].Value.ToString()));
                        rsPP.MoveNext();
                    }
                    cbSectBillingRate.DataSource = item;
                    cbSectBillingRate.DisplayMember = "Text";
                    cbSectBillingRate.ValueMember = "Value";
                }
            }

            if (cbEncasement.DataSource == null)
            {
                cbEncasement.Items.Clear();
                sSql = " select distinct DT_S_ENCASE from ref_civ_ductpath where DT_S_WAYS='" + cbNumDuctWays.SelectedValue.ToString() + "'";
                rsPP = GTDuctPathPlc.m_IGTDataContext.OpenRecordset(sSql, ADODB.CursorTypeEnum.adOpenStatic, ADODB.LockTypeEnum.adLockReadOnly, 1);

                if (rsPP.RecordCount > 0)
                {
                    List<ComboBoxItems> item = new List<ComboBoxItems>();

                    rsPP.MoveFirst();
                    for (int i = 0; i < rsPP.RecordCount; i++)
                    {
                        item.Add(new ComboBoxItems(rsPP.Fields[0].Value.ToString(), rsPP.Fields[0].Value.ToString()));
                        rsPP.MoveNext();
                    }
                    cbEncasement.DataSource = item;
                    cbEncasement.DisplayMember = "Text";
                    cbEncasement.ValueMember = "Value";
                }
            }

            if (cbSectBackFill.DataSource == null)
            {
                cbSectBackFill.Items.Clear();
                sSql = " select distinct DT_S_BACKFILL from ref_civ_ductpath where DT_S_WAYS='" + cbNumDuctWays.SelectedValue.ToString() + "'";
                rsPP = GTDuctPathPlc.m_IGTDataContext.OpenRecordset(sSql, ADODB.CursorTypeEnum.adOpenStatic, ADODB.LockTypeEnum.adLockReadOnly, 1);

                if (rsPP.RecordCount > 0)
                {
                    List<ComboBoxItems> item1 = new List<ComboBoxItems>();

                    rsPP.MoveFirst();
                    for (int i = 0; i < rsPP.RecordCount; i++)
                    {
                        item1.Add(new ComboBoxItems(rsPP.Fields[0].Value.ToString(), rsPP.Fields[0].Value.ToString()));
                        rsPP.MoveNext();
                    }
                    cbSectBackFill.DataSource = item1;
                    cbSectBackFill.DisplayMember = "Text";
                    cbSectBackFill.ValueMember = "Value";
                }
            }

           
            if (cbSectDiam.DataSource == null)
            {
                cbSectDiam.Items.Clear();
                sSql = " select distinct DT_S_DIAMETER from ref_civ_ductpath  where DT_S_WAYS='" + cbNumDuctWays.SelectedValue.ToString() + "' order by to_number(DT_S_DIAMETER) asc ";
                rsPP = GTDuctPathPlc.m_IGTDataContext.OpenRecordset(sSql, ADODB.CursorTypeEnum.adOpenStatic, ADODB.LockTypeEnum.adLockReadOnly, 1);

                if (rsPP.RecordCount > 0)
                {
                    List<ComboBoxItems> item1 = new List<ComboBoxItems>();

                    rsPP.MoveFirst();
                    for (int i = 0; i < rsPP.RecordCount; i++)
                    {
                        item1.Add(new ComboBoxItems(rsPP.Fields[0].Value.ToString(), rsPP.Fields[0].Value.ToString()));
                        rsPP.MoveNext();
                    }
                    cbSectDiam.DataSource = item1;
                    cbSectDiam.DisplayMember = "Text";
                    cbSectDiam.ValueMember = "Value";
                }
            }

        }
        #endregion

        #region button Confirm for attr for Section clicl
        private void btnConfirmSectAttr_Click(object sender, EventArgs e)
        {
            if (ValidateDuctPathSect())
            {
                btnConfirmSectAttr.Enabled = false;
                gbSectValues.Enabled = false;
                if(Sections==null)
                    Sections = new List<DuctPathSect>();
                if(SectSlashes==null)
                    SectSlashes = new List<SectSlash>();
                DuctPathSect s = FillSection();
                Sections.Add(s);
                
              
                if(SectSlashes.Count>0 && LastSection==0)
                {
                    int graphlen = 0;
                    int newgraphlen = 0;
                    if (SectSlashes.Count == 1)
                    {
                        graphlen = SectSlashes[SectSlashes.Count - 1].length;
                        newgraphlen = Sections[Sections.Count - 1].SectGraphicLength;
                    
                    }
                    else
                    {
                        graphlen = SectSlashes[SectSlashes.Count - 1].length;
                        newgraphlen = SectSlashes[SectSlashes.Count - 2].length + Sections[Sections.Count - 1].SectGraphicLength;
                    }

                    if (newgraphlen != graphlen)
                    {
                      PlaceSlashSection(newgraphlen);
                    }
                    PlaceLabelSect();
                    return;
                }

                if (SectSlashes.Count >= 0 && LastSection!=0)
                {
                    
                    int lengthRest = TotalEnteredLength;
                   
                        if (Sections.Count > 0)
                        {
                            for (int i = 0; i < Sections.Count; i++)
                            {
                                lengthRest -= int.Parse(Sections[i].SectionLength);
                            }
                        }
                        if (lengthRest != 0)
                        {
                            int LenLastSection = int.Parse(Sections[Sections.Count - 1].SectionLength) + Math.Abs(lengthRest);
                            DialogResult closetype = MessageBox.Show("Summary length of Section's lengths should be equal to \n Total Duct Path Length = " + txtTotalLength.Text +
                                " !\n Do you want to change length of last section to " + LenLastSection.ToString() + 
                                "\n to complete summary length of sections?\n If 'NO', place one more section!", "Duct Path Placement", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                            if (closetype == DialogResult.Yes)
                            {
                                txtSectionLength.Text = LenLastSection.ToString();
                                int c = Sections.Count;
                                DuctPathSect Stem = new DuctPathSect();
                                Stem = Sections[c - 1];
                                Stem.SectionLength = LenLastSection.ToString();
                                if (TotalGraphicLength != TotalEnteredLength)
                                {
                                    if (TotalEnteredLength == 0) TotalEnteredLength = 1;
                                    Stem.SectGraphicLength = TotalGraphicLength * LenLastSection / TotalEnteredLength;
                                }
                                else Stem.SectGraphicLength = LenLastSection;
                                Sections.RemoveAt(c - 1);
                                Sections.Add(Stem);

                                if (Sections.Count == SectSlashes.Count && Sections.Count != 0)
                                {
                                    SectSlashes.RemoveAt(SectSlashes.Count - 1);
                                    GTDuctPathPlc.mobjEditService.RemoveGeometry(GTDuctPathPlc.mobjEditService.GeometryCount - 2);
                                }

                                PlaceLabelSect();
                                btnFinished.Enabled = true;
                                return;
                            }
                            else
                            {
                                btnFinished.Enabled = false;
                                LastSection = 2;
                                if(SectSlashes.Count>0)
                                    PlaceSlashSection(SectSlashes[SectSlashes.Count - 1].length + Sections[Sections.Count - 1].SectGraphicLength);
                                if (SectSlashes.Count ==0)
                                    PlaceSlashSection(Sections[Sections.Count - 1].SectGraphicLength);
                                PlaceLabelSect();
                                return;
                            }
                        }
                        else LastSection = 1;
                    

                    PlaceLabelSect();
                    btnFinished.Enabled = true;
                    return;
                }

            }
        }
        #endregion
              
        #region validate enter values for attr for section
        private bool ValidateDuctPathSect()
        {
            int sectLen=0;
            int lengthRest = int.Parse(txtTotalLength.Text);
            if (int.TryParse(txtSectionLength.Text, out sectLen))
            {
                if (sectLen == 0)
                {
                    int lengthRest1 = TotalEnteredLength;
                    int LenLastSection = 0;
                   
                        if (Sections.Count > 0)
                        {
                            for (int i = 0; i < Sections.Count; i++)
                            {
                                lengthRest1 -= int.Parse(Sections[i].SectionLength);
                            }
                        }
                        if (lengthRest1 != 0)
                        {
                            LenLastSection = int.Parse(Sections[Sections.Count - 1].SectionLength) + Math.Abs(lengthRest1);
                        }
                    txtSectionLength.Text = LenLastSection.ToString();
                    MessageBox.Show("Length of Section can not be equal 0!", "Duct Path Placement", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }
                if (Sections.Count > 0)
                {                   
                    for (int i = 0; i < Sections.Count; i++)
                    {
                        lengthRest -= int.Parse(Sections[i].SectionLength);
                    }
                }
                if (lengthRest < sectLen)
                {
                    int LenLastSection = 0;
                    if (lengthRest != 0)
                    {
                        LenLastSection = int.Parse(Sections[Sections.Count - 1].SectionLength) + Math.Abs(lengthRest);
                    }
                    txtSectionLength.Text = LenLastSection.ToString();
                    MessageBox.Show("Summary length of Section's lengths can not be greater than Total Conduit Length = " + txtTotalLength.Text + " !", "Duct Path Placement", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }
            }
            else
            {
                MessageBox.Show("Section's length should be integer number!", "Duct Path Placement", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            if (!int.TryParse(txtNumDuctWaysSect.Text, out sectLen))
            {
                MessageBox.Show("Section's number of Ducts (Ductways) should be integer number!", "Duct Path Placement", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            //else  if (int.Parse(txtNumDuctWays.Text) < sectLen)
            //{
            //    MessageBox.Show("Section's number of duct ways can not be greater than Conduit's = "+txtNumDuctWays.Text+" !", "Duct Path Placement", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //    return false;
            //}
            else if (sectLen == 0)
            {
                MessageBox.Show("Number of duct ways can not be equal 0!", "Duct Path Placement", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            if (!int.TryParse(cbSectDiam.SelectedValue.ToString(), out sectLen))
            {
                MessageBox.Show("Section's diameter should be integer number!", "Duct Path Placement", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
         //   string st=
            string minmat = Get_Value("select MIN_MATERIAL from ref_civ_ductpath "+
" where DT_S_TYPE='" + cbSectType.SelectedValue.ToString() + "' and  DT_S_WAYS=" + txtNumDuctWaysSect.Text +
" and DT_S_PLACMNT='" + cbSectPlc.SelectedValue.ToString() + "' and DT_S_ENCASE='" + cbEncasement.SelectedValue.ToString() +
                "' and DT_S_BACKFILL='" + cbSectBackFill.SelectedValue.ToString() + "' and DT_S_DIAMETER =" + cbSectDiam.SelectedValue.ToString());

            if(minmat=="")
            {
                MessageBox.Show("Min_Material type are not matching with the rest of attributes!", "Duct Path Placement", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            mcbMinMaterial.Text = minmat;

            if (txtYearExpanded.Text != "0" &&
                            txtYearExpanded.Text != "00" &&
                            txtYearExpanded.Text != "000" &&
                            txtYearExpanded.Text != "0000" &&
                            txtYearExpanded.Text != "")
            { 
                if (!int.TryParse(txtYearExpanded.Text, out sectLen))
                {
                    MessageBox.Show("Value of Expanded Year is not correct!", "Duct Path Placement", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }
                else if(sectLen < 1900)
                {
                    MessageBox.Show("Value of Expanded Year is not correct!", "Duct Path Placement", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }
            }

            if (txtYearExtended.Text != "0" &&
                            txtYearExtended.Text != "00" &&
                            txtYearExtended.Text != "000" &&
                            txtYearExtended.Text != "0000" &&
                            txtYearExtended.Text != "")
            {
                if (!int.TryParse(txtYearExtended.Text, out sectLen))
                {
                    MessageBox.Show("Value of Extended Year is not correct!", "Duct Path Placement", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }
                else if (sectLen < 1900)
                {
                    MessageBox.Show("Value of Extended Year is not correct!", "Duct Path Placement", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }
            }

            return true;
        }
        #endregion

        #region button Finished click
        private void btnFinished_Click(object sender, EventArgs e)
        {
            if (SectLabels.Count == Sections.Count && SectSlashes.Count == SectLabels.Count -1)
            {

                btnFinished.Enabled = false;
                btnClearAllSections.Enabled = false;
                btnBack2.Enabled = false;
                btnClose3.Enabled = false;
                //placing Conduit(Duct Path)
                if (DuctPathPlacement())
                {
                    //if success - placing Sections (attributes, slash points, labels)
                    //if (SectionPlacemet())
                    //{
                        
                        progressBar1.Visible = false;
                        progressBar1.Value = 0;
                        DialogResult closeval = MessageBox.Show("Do you want place next Duct Path?", "Duct Path Placement", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (closeval == DialogResult.Yes)
                        {
                            btnClearAllSections.Enabled = true;
                            btnBack2.Enabled = true;
                            btnClose3.Enabled = true;
                            if (GTDuctPathPlc.mobjEditService != null)
                                GTDuctPathPlc.mobjEditService.RemoveAllGeometries();
                            GTDuctPathPlc.m_gtapp.SelectedObjects.Clear();
                            tabDuctPlc.SelectedTab = tabPagePLC;
                            LastSection = 0;
                            if (SectSlashes != null)
                                SectSlashes.Clear();
                            if (SectLabels != null)
                                SectLabels.Clear();
                            if (Sections != null)
                                Sections.Clear();
                            SourceDevicePoint = null;
                            TermDevicePoint = null;
                            GTDuctPathPlc.StartDrawPoint = null;
                            GTDuctPathPlc.EndDrawPoint = null; 
                            if (DuctPathLineGeom != null)
                            {
                                DuctPathLineGeom.Points.Clear();
                                DuctPathLineGeom = null;
                            }
                            txtFIDSource.Text = "";
                            txtFIDTerm.Text = "";
                            txtTypeSource.Text = "";
                            txtTypeTerm.Text = "";
                            txtWallSource.Text = "";
                            txtWallTerm.Text = "";
                            gbSectValues.Enabled = true;
                            gbDuctPathAttrValues.Enabled = true;
                            txtSectionLength.Enabled = true;
                            this.Hide();
                            GTDuctPathPlc.startdraw = 100;
                            NumberOfConduit++;
                        //}
                        //else
                        //{
                        //    CloseStatus = 1;
                        //    Close();
                        //    Dispose();
                        //}

                    }
                    else
                    {
                        CloseStatus = 1;
                        Close();
                        Dispose();
                    }
                }
                else
                {
                    CloseStatus = 1;
                    Close();
                    Dispose();
                }
            }
            else
            {
                MessageBox.Show("Placement for last section is not finished yet!", "Duct Path Placement", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

        }
        #endregion
    
        #region Change Enable status for Button Confirm Section Attr
        private void btnConfirmSectAttr_EnabledChanged(object sender, EventArgs e)
        {
            if (btnConfirmSectAttr.Enabled == true)
                FillingPage3Sect();
        }
        #endregion

        #region Change Enable status for Finished button
        private void btnFinished_EnabledChanged(object sender, EventArgs e)
        {
            if (btnFinished.Enabled == true)
            {
                btnConfirmSectAttr.Enabled = false;
                btnConfirmSectAttr.Visible = false;
                btnFinished.Visible = true;
                gbFinishedMessage.Visible = true;
            }

            if (btnFinished.Enabled == false)
            {
                btnConfirmSectAttr.Visible = true;
                btnFinished.Visible = false;
                gbFinishedMessage.Visible = false;
            }

        }
        #endregion

        #region Clear All Sections
        private void btnClearAllSections_Click(object sender, EventArgs e)
        {
            DialogResult retVal = MessageBox.Show("All Sections for Duct Path will be deleted!\nContinue?", "Duct Path Placement", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (retVal == DialogResult.No)
                return;
            
            if (Sections != null)
                Sections.Clear();
            if (SectSlashes != null)
                SectSlashes.Clear();
            if (SectLabels != null)
                SectLabels.Clear();
            GTDuctPathPlc.mobjEditService.RemoveAllGeometries();
            GTDuctPathPlc.mobjEditService.AddGeometry(DuctPathLineGeom, 12000);
            btnConfirmSectAttr.Enabled = false;
            LastSection = 0;
            LocateFeature(2, GTDuctPathPlc.m_gtapp.ActiveMapWindow);
            this.Hide();
            GTDuctPathPlc.startdraw = 2;
            gbSectValues.Enabled = true;
            btnFinished.Enabled = false;
                   
        }
        #endregion  

        #endregion

        #region Duct Path Placement
        private bool DuctPathPlacement()
        {
            short iFNO=2200;
            short iCNO;
            int iFID;
            try
            {
                GTDuctPathPlc.m_oIGTTransactionManager.Begin("DuctPathPlc");
                progressBar1.Visible = true;
                progressBar1.Value = 2;
                IGTKeyObject oNewFeature = GTDuctPathPlc.m_IGTDataContext.NewFeature(iFNO);
                iFID = oNewFeature.FID;
                NewPathFID = iFID;
                
                #region Attributes
                iCNO = 2201;
                    if (oNewFeature.Components.GetComponent(iCNO).Recordset.EOF)
                    {
                        oNewFeature.Components.GetComponent(iCNO).Recordset.AddNew("G3E_FID", iFID);
                        oNewFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_FNO", iFNO);
                        oNewFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_CID", 1);
                        if (txtFlag.Text != "")
                            oNewFeature.Components.GetComponent(iCNO).Recordset.Update("CENTRALDB_FLAG", txtFlag.Text);
                        else oNewFeature.Components.GetComponent(iCNO).Recordset.Update("CENTRALDB_FLAG", null);
                        oNewFeature.Components.GetComponent(iCNO).Recordset.Update("DT_WAYS", cbNumDuctWays.SelectedValue);
                        oNewFeature.Components.GetComponent(iCNO).Recordset.Update("DT_ND_FRM_ID", txtDuctSourceFID.Text);
                        oNewFeature.Components.GetComponent(iCNO).Recordset.Update("DT_ND_FRM_TY", txtDuctSourceType.Text);
                        oNewFeature.Components.GetComponent(iCNO).Recordset.Update("DT_MH_FRM_WALL", txtDuctSourceWall.Text);
                        oNewFeature.Components.GetComponent(iCNO).Recordset.Update("DT_ND_TO_ID", txtDuctTermFID.Text);
                        oNewFeature.Components.GetComponent(iCNO).Recordset.Update("DT_ND_TO_TY", txtDuctTermType.Text);
                        oNewFeature.Components.GetComponent(iCNO).Recordset.Update("DT_MH_TO_WALL", txtDuctTermWall.Text);
                        oNewFeature.Components.GetComponent(iCNO).Recordset.Update("DT_CONSTRUCTION", cbConstructed.SelectedValue);
                        oNewFeature.Components.GetComponent(iCNO).Recordset.Update("TOTAL_LENGTH", txtTotalLength.Text);
                        oNewFeature.Components.GetComponent(iCNO).Recordset.Update("DT_NEST_WAYS", 0);
                        oNewFeature.Components.GetComponent(iCNO).Recordset.Update("DT_ASB_SUBDUCT", 0);
                        oNewFeature.Components.GetComponent(iCNO).Recordset.Update("DT_PP_SUBDUCT", 0);
                        oNewFeature.Components.GetComponent(iCNO).Recordset.Update("DT_PP_INNDUCT", 0);
                        oNewFeature.Components.GetComponent(iCNO).Recordset.Update("DESCRIPTION", txtTotalLength.Text + "m/DW[" + cbNumDuctWays.SelectedValue.ToString() + "]");
                        
                    }
                    else
                    {
                        oNewFeature.Components.GetComponent(iCNO).Recordset.MoveLast();
                        oNewFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_FNO", iFNO);
                        oNewFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_CID", 1);
                        if (txtFlag.Text != "")
                            oNewFeature.Components.GetComponent(iCNO).Recordset.Update("CENTRALDB_FLAG", txtFlag.Text);
                        else oNewFeature.Components.GetComponent(iCNO).Recordset.Update("CENTRALDB_FLAG", null);
                        oNewFeature.Components.GetComponent(iCNO).Recordset.Update("DT_WAYS", cbNumDuctWays.SelectedValue);
                        oNewFeature.Components.GetComponent(iCNO).Recordset.Update("DT_ND_FRM_ID", txtDuctSourceFID.Text);
                        oNewFeature.Components.GetComponent(iCNO).Recordset.Update("DT_ND_FRM_TY", txtDuctSourceType.Text);
                        oNewFeature.Components.GetComponent(iCNO).Recordset.Update("DT_MH_FRM_WALL", txtDuctSourceWall.Text);
                        oNewFeature.Components.GetComponent(iCNO).Recordset.Update("DT_ND_TO_ID", txtDuctTermFID.Text);
                        oNewFeature.Components.GetComponent(iCNO).Recordset.Update("DT_ND_TO_TY", txtDuctTermType.Text);
                        oNewFeature.Components.GetComponent(iCNO).Recordset.Update("DT_MH_TO_WALL", txtDuctTermWall.Text);
                        oNewFeature.Components.GetComponent(iCNO).Recordset.Update("DT_CONSTRUCTION", cbConstructed.SelectedValue);
                        oNewFeature.Components.GetComponent(iCNO).Recordset.Update("TOTAL_LENGTH", txtTotalLength.Text);
                        oNewFeature.Components.GetComponent(iCNO).Recordset.Update("DT_NEST_WAYS", 0);
                        oNewFeature.Components.GetComponent(iCNO).Recordset.Update("DT_ASB_SUBDUCT", 0);
                        oNewFeature.Components.GetComponent(iCNO).Recordset.Update("DT_PP_SUBDUCT", 0);
                        oNewFeature.Components.GetComponent(iCNO).Recordset.Update("DT_PP_INNDUCT", 0);
                        oNewFeature.Components.GetComponent(iCNO).Recordset.Update("DESCRIPTION", txtTotalLength.Text + "m/DW[" + cbNumDuctWays.SelectedValue.ToString() + "]");
                        
                    }
                    progressBar1.Value = 7;

                    #endregion

                #region Netelem
                    iCNO = 51;
                    if (oNewFeature.Components.GetComponent(iCNO).Recordset.EOF)
                    {
                        oNewFeature.Components.GetComponent(iCNO).Recordset.AddNew("G3E_FID", iFID);
                        oNewFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_FNO", iFNO);
                        oNewFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_CID", 1);
                        oNewFeature.Components.GetComponent(iCNO).Recordset.Update("YEAR_PLACED", txtYearIns.Text);
                        oNewFeature.Components.GetComponent(iCNO).Recordset.Update("SWITCH_CENTRE_CLLI", "0");
                        oNewFeature.Components.GetComponent(iCNO).Recordset.Update("EXC_ABB", txtExcAbb.Text);
                        oNewFeature.Components.GetComponent(iCNO).Recordset.Update("FEATURE_STATE", "PPF");
                        oNewFeature.Components.GetComponent(iCNO).Recordset.Update("BILLING_RATE", cbBillingRate.SelectedValue);
                        oNewFeature.Components.GetComponent(iCNO).Recordset.Update("MIN_MATERIAL", mcbMinMaterial.Text);
                    }
                    else
                    {
                        oNewFeature.Components.GetComponent(iCNO).Recordset.MoveLast();
                        oNewFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_FNO", iFNO);
                        oNewFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_CID", 1);
                        oNewFeature.Components.GetComponent(iCNO).Recordset.Update("YEAR_PLACED", txtYearIns.Text);
                        oNewFeature.Components.GetComponent(iCNO).Recordset.Update("SWITCH_CENTRE_CLLI", "0");
                        oNewFeature.Components.GetComponent(iCNO).Recordset.Update("EXC_ABB", txtExcAbb.Text);
                        oNewFeature.Components.GetComponent(iCNO).Recordset.Update("FEATURE_STATE", "PPF");
                        oNewFeature.Components.GetComponent(iCNO).Recordset.Update("BILLING_RATE", cbBillingRate.SelectedValue);
                        oNewFeature.Components.GetComponent(iCNO).Recordset.Update("MIN_MATERIAL", mcbMinMaterial.Text);
                    }
                    progressBar1.Value = 13;
                    #endregion
                
                #region Line Graphic
                    iCNO = 2210;
                    if (oNewFeature.Components.GetComponent(iCNO).Recordset.EOF)
                    {
                        oNewFeature.Components.GetComponent(iCNO).Recordset.AddNew("G3E_FID", iFID);
                        oNewFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_FNO", iFNO);
                        oNewFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_CID", 1);
                        
                    }
                    else
                    {
                        oNewFeature.Components.GetComponent(iCNO).Recordset.MoveLast();
                        oNewFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_FNO", iFNO);
                        oNewFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_CID", 1);
                    }
                    oNewFeature.Components.GetComponent(iCNO).Geometry = DuctPathLineGeom;
                progressBar1.Value = 17;
                #endregion

                //#region Section Attributes
                //iCNO = 2202;

                //if (oNewFeature.Components.GetComponent(iCNO).Recordset.EOF)
                //{
                //    oNewFeature.Components.GetComponent(iCNO).Recordset.AddNew("G3E_FID", iFID);
                //    oNewFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_FNO", iFNO);
                //    oNewFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_CID", 1);
                //    oNewFeature.Components.GetComponent(iCNO).Recordset.Update("DT_S_WAYS", int.Parse(txtNumDuctWaysSect.Text));
                //    oNewFeature.Components.GetComponent(iCNO).Recordset.Update("DT_S_LENGTH", int.Parse(txtTotalLength.Text));
                //    oNewFeature.Components.GetComponent(iCNO).Recordset.Update("DT_S_TYPE", cbSectType.SelectedValue);
                //    oNewFeature.Components.GetComponent(iCNO).Recordset.Update("DT_S_PLACMNT", cbSectPlc.SelectedValue);
                //    oNewFeature.Components.GetComponent(iCNO).Recordset.Update("DT_S_ENCASE", cbEncasement.SelectedValue);
                //    oNewFeature.Components.GetComponent(iCNO).Recordset.Update("DT_S_DIAMETER", int.Parse(txtSectDiam.Text));
                //    oNewFeature.Components.GetComponent(iCNO).Recordset.Update("DT_S_BACKFILL", cbSectBackFill.SelectedValue);
                //    oNewFeature.Components.GetComponent(iCNO).Recordset.Update("COMMON_TRENCH", cbSectOwner.SelectedValue);                    
                //    oNewFeature.Components.GetComponent(iCNO).Recordset.Update("BILLING_RATE", cbSectBillingRate.SelectedValue);
                //}
                //else
                //{
                //    oNewFeature.Components.GetComponent(iCNO).Recordset.MoveLast();
                //    oNewFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_FNO", iFNO);
                //    oNewFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_CID", 1);
                //    oNewFeature.Components.GetComponent(iCNO).Recordset.Update("DT_S_WAYS", int.Parse(txtNumDuctWaysSect.Text));
                //    oNewFeature.Components.GetComponent(iCNO).Recordset.Update("DT_S_LENGTH", int.Parse(txtTotalLength.Text));
                //    oNewFeature.Components.GetComponent(iCNO).Recordset.Update("DT_S_TYPE", cbSectType.SelectedValue);
                //    oNewFeature.Components.GetComponent(iCNO).Recordset.Update("DT_S_PLACMNT", cbSectPlc.SelectedValue);
                //    oNewFeature.Components.GetComponent(iCNO).Recordset.Update("DT_S_ENCASE", cbEncasement.SelectedValue);
                //    oNewFeature.Components.GetComponent(iCNO).Recordset.Update("DT_S_DIAMETER", int.Parse(txtSectDiam.Text));
                //    oNewFeature.Components.GetComponent(iCNO).Recordset.Update("DT_S_BACKFILL", cbSectBackFill.SelectedValue);
                //    oNewFeature.Components.GetComponent(iCNO).Recordset.Update("COMMON_TRENCH", cbSectOwner.SelectedValue);                    
                //    oNewFeature.Components.GetComponent(iCNO).Recordset.Update("BILLING_RATE", cbSectBillingRate.SelectedValue);
                //}
                //progressBar1.Value = 20;
                //#endregion

                //#region Section Label
                //iCNO = 2230;
                //IGTTextPointGeometry oTextGeom = GTClassFactory.Create<IGTTextPointGeometry>();
                //IGTPoint temp = PointForSlashBasedOnEnteredLength(TotalGraphicLength / 2);
                //if (oNewFeature.Components.GetComponent(iCNO).Recordset.EOF)
                //{
                //    oNewFeature.Components.GetComponent(iCNO).Recordset.AddNew("G3E_FID", iFID);
                //    oNewFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_FNO", iFNO);
                //    oNewFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_CID", 1);
                //}
                //else
                //{
                //    oNewFeature.Components.GetComponent(iCNO).Recordset.MoveLast();
                //    oNewFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_FNO", iFNO);
                //    oNewFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_CID", 1);
                //}
                //oTextGeom = GTClassFactory.Create<IGTTextPointGeometry>();
                //IGTPoint oPointText = GTClassFactory.Create<IGTPoint>();
                //oPointText.X = temp.X;
                //oPointText.Y = temp.Y;
                //oPointText.Z = 0;
                //oTextGeom.Origin = oPointText;
                //oTextGeom.Rotation = TakeRotationOfSegmentPolyline(TotalGraphicLength / 2);
                //oNewFeature.Components.GetComponent(iCNO).Geometry = oTextGeom;
                //progressBar1.Value = 25;
                //#endregion

                #region NE Connection to Source or Term devices
                //iCNO = 54;
                //if (oNewFeature.Components.GetComponent(iCNO).Recordset.EOF)
                //{
                //    oNewFeature.Components.GetComponent(iCNO).Recordset.AddNew("G3E_FID", iFID);
                //    oNewFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_FNO", iFNO);
                //    oNewFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_CID", 1);
                //    oNewFeature.Components.GetComponent(iCNO).Recordset.Update("NODE1_ID", Get_Value("select max(NODE1_ID) from GC_NE_CONNECT where g3e_fid = " + txtDuctSourceFID.Text));
                //    oNewFeature.Components.GetComponent(iCNO).Recordset.Update("NODE2_ID", Get_Value("select max(NODE1_ID) from GC_NE_CONNECT where g3e_fid = " + txtDuctTermFID.Text));
                //}
                //else
                //{
                //    oNewFeature.Components.GetComponent(iCNO).Recordset.MoveLast();
                //    oNewFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_FNO", iFNO);
                //    oNewFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_CID", 1);
                //    oNewFeature.Components.GetComponent(iCNO).Recordset.Update("NODE1_ID", Get_Value("select max(NODE1_ID) from GC_NE_CONNECT where g3e_fid = " + txtDuctSourceFID.Text));
                //    oNewFeature.Components.GetComponent(iCNO).Recordset.Update("NODE2_ID", Get_Value("select max(NODE1_ID) from GC_NE_CONNECT where g3e_fid = " + txtDuctTermFID.Text));
                //}
                //progressBar1.Value = 25;
                // if (CreateNEConnectionDuctPath(iFID))
                //   kk;
                //int sourceFID = int.Parse(txtDuctSourceFID.Text);
                //int termFID = int.Parse(txtDuctTermFID.Text);
                //short sourceFNO = 0;
                //short termFNO = 0;

                //if (txtDuctSourceType.Text == "Manhole")
                //    sourceFNO = 2700;
                //else if (txtDuctSourceType.Text == "Chamber")
                //    sourceFNO = 3800;
                //else if (txtDuctSourceType.Text == "Tunnel")
                //    sourceFNO = 3300;
                //else if (txtDuctSourceType.Text == "Trench")
                //    sourceFNO = 3300;
                //else if (txtDuctSourceType.Text == "Civil Node")
                //    sourceFNO = 2800;

                //if (txtDuctTermType.Text == "Manhole")
                //    termFNO = 2700;
                //else if (txtDuctTermType.Text == "Chamber")
                //    termFNO = 3800;
                //else if (txtDuctTermType.Text == "Tunnel")
                //    termFNO = 3300;
                //else if (txtDuctTermType.Text == "Trench")
                //    termFNO = 3300;
                //else if (txtDuctTermType.Text == "Civil Node")
                //    termFNO = 2800;


                IGTKeyObject oSource = GTDuctPathPlc.m_IGTDataContext.OpenFeature(SourceFNO, SourceFID);
                IGTKeyObject oTerm = GTDuctPathPlc.m_IGTDataContext.OpenFeature(TermFNO, TermFID);
             //   IGTKeyObject oDuctPath = m_IGTDataContext.OpenFeature(2200, iFID);

                GTDuctPathPlc.mobjRelationshipService.ActiveFeature = oSource;

                if (GTDuctPathPlc.mobjRelationshipService.AllowSilentEstablish(oNewFeature))
                    GTDuctPathPlc.mobjRelationshipService.SilentEstablish(1, oNewFeature, GTRelationshipOrdinalConstants.gtrelRelationshipOrdinal1);
                else
                {
                    GTDuctPathPlc.m_oIGTTransactionManager.Rollback();
                    MessageBox.Show("Error during trying reestablish relationship!", "Duct Path Placement", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }

                GTDuctPathPlc.mobjRelationshipService.ActiveFeature = oNewFeature;
                if (GTDuctPathPlc.mobjRelationshipService.AllowSilentEstablish(oTerm))
                    GTDuctPathPlc.mobjRelationshipService.SilentEstablish(1, oTerm, GTRelationshipOrdinalConstants.gtrelRelationshipOrdinal2);
                else
                {
                    GTDuctPathPlc.m_oIGTTransactionManager.Rollback();
                    MessageBox.Show("Error during trying reestablish relationship!", "Duct Path Placement", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
                progressBar1.Value = 30;
                #endregion

                #region Sects
                int indexProgress = 45;

                progressBar1.Value = indexProgress;
                if (Sections.Count + SectLabels.Count + SectSlashes.Count != 0)
                    indexProgress = 50 / (Sections.Count + SectLabels.Count + SectSlashes.Count);
                else indexProgress = 10;

                #region Section Attributes
                for (int i = 0; i < Sections.Count; i++)
                {
                    if (i != 0)
                    {

                        // Section Attributes
                        iCNO = 2202;
                        oNewFeature.Components.GetComponent(iCNO).Recordset.AddNew("G3E_FID", iFID);
                        oNewFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_FNO", iFNO);
                        oNewFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_CID", i + 1);
                        oNewFeature.Components.GetComponent(iCNO).Recordset.Update("DT_S_WAYS", Sections[i].NumDuctWaysSect);
                        oNewFeature.Components.GetComponent(iCNO).Recordset.Update("DT_S_LENGTH", Sections[i].SectionLength);
                        oNewFeature.Components.GetComponent(iCNO).Recordset.Update("DT_S_TYPE", Sections[i].SectType);
                        oNewFeature.Components.GetComponent(iCNO).Recordset.Update("DT_S_PLACMNT", Sections[i].SectPlc);
                        oNewFeature.Components.GetComponent(iCNO).Recordset.Update("DT_S_ENCASE", Sections[i].Encasement);
                        oNewFeature.Components.GetComponent(iCNO).Recordset.Update("DT_S_DIAMETER", Sections[i].SectDiam);
                        oNewFeature.Components.GetComponent(iCNO).Recordset.Update("DT_S_BACKFILL", Sections[i].SectBackFill);
                        oNewFeature.Components.GetComponent(iCNO).Recordset.Update("COMMON_TRENCH", Sections[i].SectOwner);
                        oNewFeature.Components.GetComponent(iCNO).Recordset.Update("BILLING_RATE", Sections[i].SectBillingRate);
                        oNewFeature.Components.GetComponent(iCNO).Recordset.Update("MIN_MATERIAL", Sections[i].PUSect);

                        if (Sections[i].YearExpanded.HasValue)
                            oNewFeature.Components.GetComponent(iCNO).Recordset.Update("YEAR_EXPANDED", Sections[i].YearExpanded);

                        if (Sections[i].YearExtended.HasValue)
                            oNewFeature.Components.GetComponent(iCNO).Recordset.Update("YEAR_EXTENDED", Sections[i].YearExtended);

                        progressBar1.Value += indexProgress;
                    }
                    else
                    {

                        // Section Attributes
                        iCNO = 2202;
                        if (oNewFeature.Components.GetComponent(iCNO).Recordset.EOF)
                        {
                            oNewFeature.Components.GetComponent(iCNO).Recordset.AddNew("G3E_FID", iFID);
                            oNewFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_FNO", iFNO);
                            oNewFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_CID", i + 1);
                            oNewFeature.Components.GetComponent(iCNO).Recordset.Update("DT_S_WAYS", Sections[i].NumDuctWaysSect);
                            oNewFeature.Components.GetComponent(iCNO).Recordset.Update("DT_S_LENGTH", Sections[i].SectionLength);
                            oNewFeature.Components.GetComponent(iCNO).Recordset.Update("DT_S_TYPE", Sections[i].SectType);
                            oNewFeature.Components.GetComponent(iCNO).Recordset.Update("DT_S_PLACMNT", Sections[i].SectPlc);
                            oNewFeature.Components.GetComponent(iCNO).Recordset.Update("DT_S_ENCASE", Sections[i].Encasement);
                            oNewFeature.Components.GetComponent(iCNO).Recordset.Update("DT_S_DIAMETER", Sections[i].SectDiam);
                            oNewFeature.Components.GetComponent(iCNO).Recordset.Update("DT_S_BACKFILL", Sections[i].SectBackFill);
                            oNewFeature.Components.GetComponent(iCNO).Recordset.Update("COMMON_TRENCH", Sections[i].SectOwner);
                            oNewFeature.Components.GetComponent(iCNO).Recordset.Update("BILLING_RATE", Sections[i].SectBillingRate);
                            oNewFeature.Components.GetComponent(iCNO).Recordset.Update("MIN_MATERIAL", Sections[i].PUSect);

                            if (Sections[i].YearExpanded != null)
                                oNewFeature.Components.GetComponent(iCNO).Recordset.Update("YEAR_EXPANDED", Sections[i].YearExpanded);

                            if (Sections[i].YearExtended != null)
                                oNewFeature.Components.GetComponent(iCNO).Recordset.Update("YEAR_EXTENDED", Sections[i].YearExtended);

                        }
                        else
                        {
                            oNewFeature.Components.GetComponent(iCNO).Recordset.MoveLast();
                            oNewFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_FNO", iFNO);
                            oNewFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_CID", i + 1);
                            oNewFeature.Components.GetComponent(iCNO).Recordset.Update("DT_S_WAYS", Sections[i].NumDuctWaysSect);
                            oNewFeature.Components.GetComponent(iCNO).Recordset.Update("DT_S_LENGTH", Sections[i].SectionLength);
                            oNewFeature.Components.GetComponent(iCNO).Recordset.Update("DT_S_TYPE", Sections[i].SectType);
                            oNewFeature.Components.GetComponent(iCNO).Recordset.Update("DT_S_PLACMNT", Sections[i].SectPlc);
                            oNewFeature.Components.GetComponent(iCNO).Recordset.Update("DT_S_ENCASE", Sections[i].Encasement);
                            oNewFeature.Components.GetComponent(iCNO).Recordset.Update("DT_S_DIAMETER", Sections[i].SectDiam);
                            oNewFeature.Components.GetComponent(iCNO).Recordset.Update("DT_S_BACKFILL", Sections[i].SectBackFill);
                            oNewFeature.Components.GetComponent(iCNO).Recordset.Update("COMMON_TRENCH", Sections[i].SectOwner);
                            oNewFeature.Components.GetComponent(iCNO).Recordset.Update("BILLING_RATE", Sections[i].SectBillingRate);
                            oNewFeature.Components.GetComponent(iCNO).Recordset.Update("MIN_MATERIAL", Sections[i].PUSect);

                            if (Sections[i].YearExpanded != null)
                                oNewFeature.Components.GetComponent(iCNO).Recordset.Update("YEAR_EXPANDED", Sections[i].YearExpanded);

                            if (Sections[i].YearExtended != null)
                                oNewFeature.Components.GetComponent(iCNO).Recordset.Update("YEAR_EXTENDED", Sections[i].YearExtended);

                        }

                        progressBar1.Value += indexProgress;
                    }
                }
                #endregion

                #region Lables for Sections and Leader Lines
                if (SectLabels.Count > 0)
                    for (int i = 0; i < SectLabels.Count; i++)
                    {
                        if (i != 0)
                        {
                            iCNO = 2230;
                            oNewFeature.Components.GetComponent(iCNO).Recordset.AddNew("G3E_FID", iFID);
                            oNewFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_FNO", iFNO);
                            oNewFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_CID", i + 1);
                            oNewFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_ALIGNMENT", 0);
                            oNewFeature.Components.GetComponent(iCNO).Geometry = SectLabels[i].Label;

                            if (SectLabels[i].LeaderLine != null)
                            {
                                iCNO = 2212;
                                oNewFeature.Components.GetComponent(iCNO).Recordset.AddNew("G3E_FID", iFID);
                                oNewFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_FNO", iFNO);
                                oNewFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_CID", i + 1);
                                oNewFeature.Components.GetComponent(iCNO).Geometry = SectLabels[i].LeaderLine;

                            }
                            progressBar1.Value += indexProgress;
                        }
                        else
                        {
                            iCNO = 2230;

                            if (oNewFeature.Components.GetComponent(iCNO).Recordset.EOF)
                            {
                                oNewFeature.Components.GetComponent(iCNO).Recordset.AddNew("G3E_FID", iFID);
                                oNewFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_FNO", iFNO);
                                oNewFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_CID", i + 1);
                                oNewFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_ALIGNMENT", 0);
                            }
                            else
                            {
                                oNewFeature.Components.GetComponent(iCNO).Recordset.MoveLast();
                                oNewFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_FNO", iFNO);
                                oNewFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_CID", i + 1);
                                oNewFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_ALIGNMENT", 0);
                            }
                            oNewFeature.Components.GetComponent(iCNO).Geometry = SectLabels[i].Label;

                            if (SectLabels[i].LeaderLine != null)
                            {
                                iCNO = 2212;
                                if (oNewFeature.Components.GetComponent(iCNO).Recordset.EOF)
                                {
                                    oNewFeature.Components.GetComponent(iCNO).Recordset.AddNew("G3E_FID", iFID);
                                    oNewFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_FNO", iFNO);
                                    oNewFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_CID", i + 1);
                                }
                                else
                                {
                                    oNewFeature.Components.GetComponent(iCNO).Recordset.MoveLast();
                                    oNewFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_FNO", iFNO);
                                    oNewFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_CID", i + 1);
                                }
                                oNewFeature.Components.GetComponent(iCNO).Geometry = SectLabels[i].LeaderLine;

                            }
                            progressBar1.Value += indexProgress;
                        }

                    }
                #endregion

                #region Slashes for sections
                if (Sections.Count == SectSlashes.Count && Sections.Count != 0)
                    SectSlashes.RemoveAt(SectSlashes.Count - 1);

                if (SectSlashes.Count > 0)
                    for (int i = 0; i < SectSlashes.Count; i++)
                    {

                        iCNO = 2220;
                        if (i != 0)
                        {

                            oNewFeature.Components.GetComponent(iCNO).Recordset.AddNew("G3E_FID", iFID);
                            oNewFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_FNO", iFNO);
                            oNewFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_CID", i + 1);
                        }
                        else
                        {
                            if (oNewFeature.Components.GetComponent(iCNO).Recordset.EOF)
                            {
                                oNewFeature.Components.GetComponent(iCNO).Recordset.AddNew("G3E_FID", iFID);
                                oNewFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_FNO", iFNO);
                                oNewFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_CID", i + 1);
                            }
                            else
                            {
                                oNewFeature.Components.GetComponent(iCNO).Recordset.MoveLast();
                                oNewFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_FNO", iFNO);
                                oNewFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_CID", i + 1);
                            }
                        }
                        IGTOrientedPointGeometry oPointGeom = GTClassFactory.Create<IGTOrientedPointGeometry>();
                        IGTPoint tpoint = GTClassFactory.Create<IGTPoint>();
                        tpoint.X = SectSlashes[i].X;
                        tpoint.Y = SectSlashes[i].Y;
                        tpoint.Z = 0.0;
                        oPointGeom.Origin = tpoint;
                        if (SectSlashes[i].Orient != null)
                            oPointGeom.Orientation = SectSlashes[i].Orient;
                        oNewFeature.Components.GetComponent(iCNO).Geometry = oPointGeom;
                        progressBar1.Value += indexProgress;
                    }
                #endregion
                #endregion
                GTDuctPathPlc.m_oIGTTransactionManager.Commit();
                progressBar1.Value = 98;
                GTDuctPathPlc.m_oIGTTransactionManager.RefreshDatabaseChanges();
                progressBar1.Value = 100;


                
                
            }
            catch (Exception ex)
            {
                GTDuctPathPlc.m_oIGTTransactionManager.Rollback();
                MessageBox.Show(ex.Message, "Duct Path Placement", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            return true;

        }

        #region Make NE connection
        private bool CreateNEConnectionDuctPath(int iFID)
        {
            try
            {
                GTDuctPathPlc.m_oIGTTransactionManager.Begin("CreateNEConnection");

                IGTKeyObject oSource = GTDuctPathPlc.m_IGTDataContext.OpenFeature(SourceFNO, SourceFID);
                IGTKeyObject oTerm = GTDuctPathPlc.m_IGTDataContext.OpenFeature(TermFNO, TermFID);
                IGTKeyObject oDuctPath = GTDuctPathPlc.m_IGTDataContext.OpenFeature(2200, iFID);

                GTDuctPathPlc.mobjRelationshipService.ActiveFeature = oSource;

                if (GTDuctPathPlc.mobjRelationshipService.AllowSilentEstablish(oDuctPath))
                    GTDuctPathPlc.mobjRelationshipService.SilentEstablish(1, oDuctPath, GTRelationshipOrdinalConstants.gtrelRelationshipOrdinal1);
                else
                {
                    GTDuctPathPlc.m_oIGTTransactionManager.Rollback();
                    MessageBox.Show("Error during trying reestablish relationship!", "Duct Path Placement", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }

                GTDuctPathPlc.mobjRelationshipService.ActiveFeature = oDuctPath;
                if (GTDuctPathPlc.mobjRelationshipService.AllowSilentEstablish(oTerm))
                    GTDuctPathPlc.mobjRelationshipService.SilentEstablish(1, oTerm, GTRelationshipOrdinalConstants.gtrelRelationshipOrdinal2);
                else
                {
                    GTDuctPathPlc.m_oIGTTransactionManager.Rollback();
                    MessageBox.Show("Error during trying reestablish relationship!", "Duct Path Placement", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
                GTDuctPathPlc.m_oIGTTransactionManager.Commit();
                GTDuctPathPlc.m_oIGTTransactionManager.RefreshDatabaseChanges();
                return true; ;
            }
            catch (Exception ex)
            {
                GTDuctPathPlc.m_oIGTTransactionManager.Rollback();
                MessageBox.Show(ex.Message, "Duct Path Placement", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }
        #endregion
        #endregion

        #region Place New Sections

        #region Place Slash for sect

        private void PlaceSlashSection(int len)
        {
            
            IGTOrientedPointGeometry temp = GTClassFactory.Create<IGTOrientedPointGeometry>();            
            IGTPoint tempp = PointForSlashBasedOnEnteredLength(len);
            temp.Origin = tempp;
            if (LastSection==0)
            {
                GTDuctPathPlc.mobjEditService.RemoveGeometry(GTDuctPathPlc.mobjEditService.GeometryCount);
                SectSlashes.RemoveAt(SectSlashes.Count - 1);
            }
            temp.Orientation = OrientationForPointOnConduit(tempp.X, tempp.Y, len);
            GTDuctPathPlc.mobjEditService.AddGeometry(temp, 23000);
            AddSectSlash(tempp.X, tempp.Y, len, temp.Orientation);
           }
        
        #endregion

        #region Sections to database
        private bool SectionPlacemet()
        {
            short iFNO = 2200;
            short iCNO;
            int iFID = NewPathFID;
            int indexProgress = 45;
            try
            {

                GTDuctPathPlc.m_oIGTTransactionManager.Begin("DuctPathPlc");
                progressBar1.Value = indexProgress;
                IGTKeyObject oNewFeature = GTDuctPathPlc.m_IGTDataContext.OpenFeature(iFNO, iFID);
                if (Sections.Count + SectLabels.Count + SectSlashes.Count != 0)
                    indexProgress = 50 / (Sections.Count + SectLabels.Count + SectSlashes.Count);
                else indexProgress = 10;
                
                #region Section Attributes
                for (int i = 0; i < Sections.Count; i++)
                {
                    if (i != 0)
                    {

                        // Section Attributes
                        iCNO = 2202;
                        oNewFeature.Components.GetComponent(iCNO).Recordset.AddNew("G3E_FID", iFID);
                        oNewFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_FNO", iFNO);
                        oNewFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_CID", i + 1);
                        oNewFeature.Components.GetComponent(iCNO).Recordset.Update("DT_S_WAYS", Sections[i].NumDuctWaysSect);
                        oNewFeature.Components.GetComponent(iCNO).Recordset.Update("DT_S_LENGTH", Sections[i].SectionLength);
                        oNewFeature.Components.GetComponent(iCNO).Recordset.Update("DT_S_TYPE", Sections[i].SectType);
                        oNewFeature.Components.GetComponent(iCNO).Recordset.Update("DT_S_PLACMNT", Sections[i].SectPlc);
                        oNewFeature.Components.GetComponent(iCNO).Recordset.Update("DT_S_ENCASE", Sections[i].Encasement);
                        oNewFeature.Components.GetComponent(iCNO).Recordset.Update("DT_S_DIAMETER", Sections[i].SectDiam);
                        oNewFeature.Components.GetComponent(iCNO).Recordset.Update("DT_S_BACKFILL", Sections[i].SectBackFill);
                        oNewFeature.Components.GetComponent(iCNO).Recordset.Update("COMMON_TRENCH", Sections[i].SectOwner);
                        oNewFeature.Components.GetComponent(iCNO).Recordset.Update("BILLING_RATE", Sections[i].SectBillingRate);

                        if (Sections[i].YearExpanded.HasValue)
                            oNewFeature.Components.GetComponent(iCNO).Recordset.Update("YEAR_EXPANDED", Sections[i].YearExpanded);

                        if (Sections[i].YearExtended.HasValue)
                            oNewFeature.Components.GetComponent(iCNO).Recordset.Update("YEAR_EXTENDED", Sections[i].YearExtended);

                        progressBar1.Value += indexProgress;
                    }
                    else
                    {

                        // Section Attributes
                        iCNO = 2202;
                        if (oNewFeature.Components.GetComponent(iCNO).Recordset.EOF)
                        {
                            oNewFeature.Components.GetComponent(iCNO).Recordset.AddNew("G3E_FID", iFID);
                            oNewFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_FNO", iFNO);
                            oNewFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_CID", i + 1);
                            oNewFeature.Components.GetComponent(iCNO).Recordset.Update("DT_S_WAYS", Sections[i].NumDuctWaysSect);
                            oNewFeature.Components.GetComponent(iCNO).Recordset.Update("DT_S_LENGTH", Sections[i].SectionLength);
                            oNewFeature.Components.GetComponent(iCNO).Recordset.Update("DT_S_TYPE", Sections[i].SectType);
                            oNewFeature.Components.GetComponent(iCNO).Recordset.Update("DT_S_PLACMNT", Sections[i].SectPlc);
                            oNewFeature.Components.GetComponent(iCNO).Recordset.Update("DT_S_ENCASE", Sections[i].Encasement);
                            oNewFeature.Components.GetComponent(iCNO).Recordset.Update("DT_S_DIAMETER", Sections[i].SectDiam);
                            oNewFeature.Components.GetComponent(iCNO).Recordset.Update("DT_S_BACKFILL", Sections[i].SectBackFill);
                            oNewFeature.Components.GetComponent(iCNO).Recordset.Update("COMMON_TRENCH", Sections[i].SectOwner);
                            oNewFeature.Components.GetComponent(iCNO).Recordset.Update("BILLING_RATE", Sections[i].SectBillingRate);

                            if (Sections[i].YearExpanded != null)
                                oNewFeature.Components.GetComponent(iCNO).Recordset.Update("YEAR_EXPANDED", Sections[i].YearExpanded);

                            if (Sections[i].YearExtended != null)
                                oNewFeature.Components.GetComponent(iCNO).Recordset.Update("YEAR_EXTENDED", Sections[i].YearExtended);

                        }
                        else
                        {
                            oNewFeature.Components.GetComponent(iCNO).Recordset.MoveLast();
                            oNewFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_FNO", iFNO);
                            oNewFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_CID", i + 1);
                            oNewFeature.Components.GetComponent(iCNO).Recordset.Update("DT_S_WAYS", Sections[i].NumDuctWaysSect);
                            oNewFeature.Components.GetComponent(iCNO).Recordset.Update("DT_S_LENGTH", Sections[i].SectionLength);
                            oNewFeature.Components.GetComponent(iCNO).Recordset.Update("DT_S_TYPE", Sections[i].SectType);
                            oNewFeature.Components.GetComponent(iCNO).Recordset.Update("DT_S_PLACMNT", Sections[i].SectPlc);
                            oNewFeature.Components.GetComponent(iCNO).Recordset.Update("DT_S_ENCASE", Sections[i].Encasement);
                            oNewFeature.Components.GetComponent(iCNO).Recordset.Update("DT_S_DIAMETER", Sections[i].SectDiam);
                            oNewFeature.Components.GetComponent(iCNO).Recordset.Update("DT_S_BACKFILL", Sections[i].SectBackFill);
                            oNewFeature.Components.GetComponent(iCNO).Recordset.Update("COMMON_TRENCH", Sections[i].SectOwner);
                            oNewFeature.Components.GetComponent(iCNO).Recordset.Update("BILLING_RATE", Sections[i].SectBillingRate);

                            if (Sections[i].YearExpanded != null)
                                oNewFeature.Components.GetComponent(iCNO).Recordset.Update("YEAR_EXPANDED", Sections[i].YearExpanded);

                            if (Sections[i].YearExtended != null)
                                oNewFeature.Components.GetComponent(iCNO).Recordset.Update("YEAR_EXTENDED", Sections[i].YearExtended);

                        }

                        progressBar1.Value += indexProgress;
                    }
                }
                 #endregion

                #region Lables for Sections and Leader Lines
                if (SectLabels.Count > 0)
                for (int i = 0; i < SectLabels.Count; i++)
                {
                    if (i != 0)
                    {
                        iCNO = 2230;
                        oNewFeature.Components.GetComponent(iCNO).Recordset.AddNew("G3E_FID", iFID);
                        oNewFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_FNO", iFNO);
                        oNewFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_CID", i + 1);
                        oNewFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_ALIGNMENT", 0);
                        oNewFeature.Components.GetComponent(iCNO).Geometry = SectLabels[i].Label;

                        if (SectLabels[i].LeaderLine != null)
                        {
                            if (SectLabels[i].LeaderLine.Points.Count != 0)
                            {
                                iCNO = 2212;
                                oNewFeature.Components.GetComponent(iCNO).Recordset.AddNew("G3E_FID", iFID);
                                oNewFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_FNO", iFNO);
                                oNewFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_CID", i + 1);
                                oNewFeature.Components.GetComponent(iCNO).Geometry = SectLabels[i].LeaderLine;
                            }
                        }
                        progressBar1.Value += indexProgress;
                    }
                    else
                    {
                        iCNO = 2230;

                        if (oNewFeature.Components.GetComponent(iCNO).Recordset.EOF)
                        {
                            oNewFeature.Components.GetComponent(iCNO).Recordset.AddNew("G3E_FID", iFID);
                            oNewFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_FNO", iFNO);
                            oNewFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_CID", i + 1);
                            oNewFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_ALIGNMENT", 0);
                        }
                        else
                        {
                            oNewFeature.Components.GetComponent(iCNO).Recordset.MoveLast();
                            oNewFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_FNO", iFNO);
                            oNewFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_CID", i + 1);
                            oNewFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_ALIGNMENT", 0);
                        }
                        oNewFeature.Components.GetComponent(iCNO).Geometry = SectLabels[i].Label;

                        if (SectLabels[i].LeaderLine != null)
                        {
                            if (SectLabels[i].LeaderLine.Points.Count!=0)
                            {
                                iCNO = 2212;
                                if (oNewFeature.Components.GetComponent(iCNO).Recordset.EOF)
                                {
                                    oNewFeature.Components.GetComponent(iCNO).Recordset.AddNew("G3E_FID", iFID);
                                    oNewFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_FNO", iFNO);
                                    oNewFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_CID", i + 1);
                                }
                                else
                                {
                                    oNewFeature.Components.GetComponent(iCNO).Recordset.MoveLast();
                                    oNewFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_FNO", iFNO);
                                    oNewFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_CID", i + 1);
                                }
                                oNewFeature.Components.GetComponent(iCNO).Geometry = SectLabels[i].LeaderLine;
                            }
                        }
                        progressBar1.Value += indexProgress;
                    }

                }
                 #endregion

                #region Slashes for sections
            if (Sections.Count == SectSlashes.Count && Sections.Count != 0)
                SectSlashes.RemoveAt(SectSlashes.Count - 1);

                if(SectSlashes.Count>0)
                for (int i = 0; i < SectSlashes.Count; i++)
                {

                    iCNO = 2220;
                    if (i != 0)
                    {

                        oNewFeature.Components.GetComponent(iCNO).Recordset.AddNew("G3E_FID", iFID);
                        oNewFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_FNO", iFNO);
                        oNewFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_CID", i + 1);
                    }
                    else
                    {
                        if (oNewFeature.Components.GetComponent(iCNO).Recordset.EOF)
                        {
                            oNewFeature.Components.GetComponent(iCNO).Recordset.AddNew("G3E_FID", iFID);
                            oNewFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_FNO", iFNO);
                            oNewFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_CID", i + 1);
                        }
                        else
                        {
                            oNewFeature.Components.GetComponent(iCNO).Recordset.MoveLast();
                            oNewFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_FNO", iFNO);
                            oNewFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_CID", i + 1);
                        }
                    }
                    IGTOrientedPointGeometry oPointGeom = GTClassFactory.Create<IGTOrientedPointGeometry>();
                    IGTPoint tpoint = GTClassFactory.Create<IGTPoint>();
                    tpoint.X = SectSlashes[i].X;
                    tpoint.Y = SectSlashes[i].Y;
                    tpoint.Z = 0.0;
                    oPointGeom.Origin = tpoint;
                    if(SectSlashes[i].Orient!=null)
                        oPointGeom.Orientation = SectSlashes[i].Orient;
                    oNewFeature.Components.GetComponent(iCNO).Geometry = oPointGeom;
                    progressBar1.Value += indexProgress;
                }
                #endregion

                GTDuctPathPlc.m_oIGTTransactionManager.Commit();
                progressBar1.Value = 95;
                GTDuctPathPlc.m_oIGTTransactionManager.RefreshDatabaseChanges();
                progressBar1.Value = 100;
                GTDuctPathPlc.mobjEditService.RemoveAllGeometries();
            }
            catch (Exception ex)
            {
                GTDuctPathPlc.m_oIGTTransactionManager.Rollback();
                MessageBox.Show(ex.Message, "Duct Path Placement", MessageBoxButtons.OK, MessageBoxIcon.Error);                
                return false;
            }
            return true;

        }
        #endregion

        #region Add New Slash to Slash list
        public void AddSectSlash(double x, double y, int len, IGTVector vec)
        {
            SectSlash temp = new SectSlash();
            temp.X = x;
            temp.Y = y;
            temp.length = len;
            temp.Orient = vec;
            SectSlashes.Add(temp);

        }
        #endregion

        #endregion
        
        #region Math Calculation
       
        #region Calculate Length's
        
        #region Between Two points on sumple line
        private int LegthBtwTwoPoints(double startPointX, double startPointY, double endPointX, double endPointY)
        {
            return Convert.ToInt32(Math.Round(Math.Sqrt(Math.Pow((endPointX - startPointX), 2) + Math.Pow((endPointY - startPointY), 2)), 0));
        }
        #endregion

        #region For whole conduit
        private int LengthConduit()
        {
            int length = 0;
            for (int i = 0; i < DuctPathLineGeom.Points.Count - 1; i++)
            {
                length += LegthBtwTwoPoints(DuctPathLineGeom.Points[i].X,
                        DuctPathLineGeom.Points[i].Y,
                        DuctPathLineGeom.Points[i + 1].X,
                        DuctPathLineGeom.Points[i + 1].Y);
            }

            return length;
        }
        #endregion
        #endregion

        #region Angle between segment and OX by start and end's points on segment 
        public double AngleBtwPoint(double stX, double stY, double endX, double endY)
        {
            double t1 = endY - stY;
            double t2 = endX - stX;
           
            if (t1 == 0 && t2 == 0) return 0;
            if (t2 == 0)
            {
                if (t1 > 0)
                    return  90;
                if (t1 < 0)
                    return  -90;
                if (t1 == 0)
                    return 0;
            }

            double grad = Math.Atan(Math.Abs(t1 / t2)) * 180 / Math.PI;

            if (t2 > 0)
            {
                if (t1 > 0)
                    return grad;
                if (t1 < 0)
                    return -grad;
                if (t1 == 0)
                    return 0;
            }
            if (t2 < 0)
            {
                if (t1 > 0)
                    return 180 - grad;
                if (t1 < 0)
                    return 180 + grad;
                if (t1 == 0)
                    return 180;
            }
            return 0;      
       
        }
         #endregion

        #region Orientation for Projected Point on Conduit line
        public IGTVector OrientationForPointOnConduit(double Xslash, double Yslash, int lengthSlash)
        {
            IGTVector Orientation = GTClassFactory.Create<IGTVector>();
            IGTPoint slashPoint = GTClassFactory.Create<IGTPoint>();
            slashPoint.X = Xslash;
            slashPoint.Y = Yslash;
            slashPoint.Z = 0;

            int lengthTemp = 0;
           

            for (int i = 0; i < DuctPathLineGeom.Points.Count - 1; i++)
            {

                int temp = LegthBtwTwoPoints(DuctPathLineGeom.Points[i].X,
                    DuctPathLineGeom.Points[i].Y,
                    DuctPathLineGeom.Points[i + 1].X,
                    DuctPathLineGeom.Points[i + 1].Y);
                lengthTemp += temp;

                if (lengthTemp >= lengthSlash)
                {
                    return Orientation.BuildVector(DuctPathLineGeom.Points[i], slashPoint);
                }
            }
            
            return Orientation.BuildVector(DuctPathLineGeom.Points[0], DuctPathLineGeom.Points[DuctPathLineGeom.Points.Count - 1]);

        }
        #endregion
        
        #region Coordinate for Projected Point on Conduit line
        public IGTPoint PointOnConduit(double Xslash, double Yslash, bool conf)
        {

            IGTPoint projectPoint = GTClassFactory.Create<IGTPoint>();
            projectPoint.X = 0;
            projectPoint.Y = 0;
            projectPoint.Z = 0;
            List<IGTPoint> ProjectedPoints = new List<IGTPoint>();
            for (int i = 0; i < DuctPathLineGeom.Points.Count - 1; i++)
            {
                    ProjectedPoints.Add(ProjectedPointOnConduit(DuctPathLineGeom.Points[i].X,
                    DuctPathLineGeom.Points[i].Y,
                    DuctPathLineGeom.Points[i + 1].X,
                    DuctPathLineGeom.Points[i + 1].Y,
                    Xslash, Yslash));
            }
            double min = 0;
            int sectnum = 0;
            for (int i = 0; i < ProjectedPoints.Count; i++)
            {
                double difX = ProjectedPoints[i].X - Xslash;
                double difY = ProjectedPoints[i].Y - Yslash;

                if (difX < 0) difX *= -1;
                if (difY < 0) difY *= -1;
                double disMin = difY + difX;
                if (min == 0) min = disMin+1;
                if (min > disMin)
                {
                    projectPoint.X = ProjectedPoints[i].X;
                    projectPoint.Y = ProjectedPoints[i].Y;
                    projectPoint.Z = 0;
                    min = disMin;
                    sectnum = i+1;
                }

            }
            //checking if mouse click too far from conduit
            if (min > 3)
            {
                projectPoint.X = 0;
                projectPoint.Y = 0;
                projectPoint.Z = 0;
                sectnum = 0;
            }

            //if point on conduit confirm
            if (conf)
            {
                if (sectnum > 0)
                {

                    int length = 0;
                    for (int i = 0; i < DuctPathLineGeom.Points.Count - 1; i++)
                    {
                        if (sectnum == i + 1)
                        {
                            length += LegthBtwTwoPoints(DuctPathLineGeom.Points[i].X,
                            DuctPathLineGeom.Points[i].Y, Xslash, Yslash);
                            break;
                        }
                        else
                            length += LegthBtwTwoPoints(DuctPathLineGeom.Points[i].X,
                                DuctPathLineGeom.Points[i].Y,
                                DuctPathLineGeom.Points[i + 1].X,
                                DuctPathLineGeom.Points[i + 1].Y);
                    }
                  projectPoint.Z = double.Parse(length.ToString());
                }
            }
            return projectPoint;

        }
        #endregion
        
        #region Coordinate for Projected Point on SEGMENT of Conduit line
        public IGTPoint ProjectedPointOnConduit(double stX, double stY, double endX, double endY, double slashX, double slashY)
        {
            IGTSegment sectT = GTClassFactory.Create<IGTSegment>();
            IGTSegmentPoint slashT = GTClassFactory.Create<IGTSegmentPoint>();
            IGTPoint clickT = GTClassFactory.Create<IGTPoint>();
            IGTPoint projectPoint = GTClassFactory.Create<IGTPoint>();
            sectT.Point1.X = stX;
            sectT.Point1.Y = stY;
            sectT.Point1.Z = 0;
            sectT.Point2.X = endX;
            sectT.Point2.Y = endY;
            sectT.Point2.Z = 0;

            clickT.X = slashX;
            clickT.Y = slashY;
            clickT.Z = 0;

            slashT = clickT.ProjectTo(sectT);
            
            projectPoint.X = slashT.Point.X;
            projectPoint.Y = slashT.Point.Y;
            projectPoint.Z = 0;

            return projectPoint;
        }
        #endregion

        #region Calculate Coord for point pn confuit base on entered length by user
        public IGTPoint PointForSlashBasedOnEnteredLength(int GraphicLength)
        {
            int length = 0;
            int lengthTemp = 0;
            IGTPoint SlashPoint = GTClassFactory.Create<IGTPoint>();
            SlashPoint.X = DuctPathLineGeom.Points[DuctPathLineGeom.Points.Count - 1].X;
            SlashPoint.Y = DuctPathLineGeom.Points[DuctPathLineGeom.Points.Count - 1].Y;
            SlashPoint.Z = 0.0;

            for (int i = 0; i < DuctPathLineGeom.Points.Count - 1; i++)
            {

                int temp = LegthBtwTwoPoints(DuctPathLineGeom.Points[i].X,
                    DuctPathLineGeom.Points[i].Y,
                    DuctPathLineGeom.Points[i + 1].X,
                    DuctPathLineGeom.Points[i + 1].Y);
                lengthTemp += temp;
                if (lengthTemp >= GraphicLength)
                {
                    length = temp - lengthTemp + GraphicLength;
                    if (temp == 0) temp = 1;
                    SlashPoint.X = length * (DuctPathLineGeom.Points[i + 1].X - DuctPathLineGeom.Points[i].X) / temp + DuctPathLineGeom.Points[i].X;
                    SlashPoint.Y = length * (DuctPathLineGeom.Points[i + 1].Y - DuctPathLineGeom.Points[i].Y) / temp + DuctPathLineGeom.Points[i].Y;
                    SlashPoint.Z = 0.0;
                    break;
                }
            }
            return SlashPoint;
        }
        #endregion

        #region Calculate Rotation for Along plc lable
        public double TakeRotationOfSegmentPolyline(int GraphicLength)
        {
            int lengthTemp = 0;
            double Rotat = 0.0;
            for (int i = 0; i < DuctPathLineGeom.Points.Count - 1; i++)
            {

                int temp = LegthBtwTwoPoints(DuctPathLineGeom.Points[i].X,
                    DuctPathLineGeom.Points[i].Y,
                    DuctPathLineGeom.Points[i + 1].X,
                    DuctPathLineGeom.Points[i + 1].Y);
                lengthTemp += temp;
                if (lengthTemp >= GraphicLength)
                {
                    if (DuctPathLineGeom.Points[i + 1].Y < DuctPathLineGeom.Points[i].Y &&
                        DuctPathLineGeom.Points[i + 1].X <= DuctPathLineGeom.Points[i].X
                        || DuctPathLineGeom.Points[i + 1].Y > DuctPathLineGeom.Points[i].Y &&
                        DuctPathLineGeom.Points[i + 1].X <= DuctPathLineGeom.Points[i].X)
                        Rotat = AngleBtwPoint(DuctPathLineGeom.Points[i + 1].X, DuctPathLineGeom.Points[i + 1].Y,
                            DuctPathLineGeom.Points[i].X, DuctPathLineGeom.Points[i].Y);
                    else
                        Rotat = AngleBtwPoint(DuctPathLineGeom.Points[i].X, DuctPathLineGeom.Points[i].Y,
                                          DuctPathLineGeom.Points[i + 1].X, DuctPathLineGeom.Points[i + 1].Y);
                    break;
                }
            }
            return Rotat;
        }
        #endregion

        #endregion

        #region Section's info filling
       
        #region Place label for section
        private void PlaceLabelSect()
        {
          //  GTDuctPathPlc.m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "PLACING LABEL FOR SECTION");
            GTDuctPathPlc.m_gtapp.SelectedObjects.Clear();
            if (SectLabels == null)
                SectLabels = new List<SectLabelLeaderLine>();
            int len = 0;
            if (Sections.Count > 0)
            {
                if (Sections.Count == 1)
                    len = Sections[0].SectGraphicLength / 2;
                else
                {
                    if (LastSection == 1)
                        len = SectSlashes[SectSlashes.Count - 1].length + Sections[Sections.Count - 1].SectGraphicLength / 2;
                    else len = SectSlashes[SectSlashes.Count - 1].length - Sections[Sections.Count - 1].SectGraphicLength / 2;
                }           
            }

            SectLabel = SectLabelContent(Sections.Count-1);//"[" + txtNumDuctWaysSect.Text + "] /" + txtSectionLength.Text + " m";

            IGTPoint tempp = GTClassFactory.Create<IGTPoint>();
            if (GTDuctPathPlc.LabelAlongLine == null)
                GTDuctPathPlc.LabelAlongLine = GTClassFactory.Create<IGTTextPointGeometry>();
            if (GTDuctPathPlc.oTextGeomLabel == null)
                GTDuctPathPlc.oTextGeomLabel = GTClassFactory.Create<IGTTextPointGeometry>();
            tempp = PointForSlashBasedOnEnteredLength(len);

            GTDuctPathPlc.LabelAlongLine.Origin = tempp;
            GTDuctPathPlc.LabelAlongLine.Text = SectLabel;
            GTDuctPathPlc.LabelAlongLine.Rotation = TakeRotationOfSegmentPolyline(len);
            GTDuctPathPlc.LabelAlongLine.Alignment = 0;
            GTDuctPathPlc.mobjEditService.AddGeometry(GTDuctPathPlc.LabelAlongLine, 32400);

            GTDuctPathPlc.oTextGeomLabel.Origin = tempp;
            GTDuctPathPlc.oTextGeomLabel.Text = SectLabel;
            GTDuctPathPlc.oTextGeomLabel.Alignment = 0;
            GTDuctPathPlc.oTextGeomLabel.Rotation = GTDuctPathPlc.LabelAlongLine.Rotation;
            this.Hide();
                    GTDuctPathPlc.m_gtapp.SelectedObjects.Clear();
                    GTDuctPathPlc.startdraw = 30;
                   // GTDuctPathPlc.m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "LEFT MOUSE CLICK TO PLACE TEMPORARY LABEL ON MAP, DOUBLE MOUSE CLICK TO CONFIRM LOCATION OF LABEL");
             
        }
        #endregion

        #region Add label for section
        public void SectionLabelAdd(double X, double Y, double Rotation, IGTPolylineGeometry LeaderLine)
        {
            IGTPoint tempp = GTClassFactory.Create<IGTPoint>();
            tempp.X = X;
            tempp.Y = Y;
            tempp.Z = 0.0;
            IGTTextPointGeometry oTextGeom = GTClassFactory.Create<IGTTextPointGeometry>();
            oTextGeom.Origin = tempp;
            oTextGeom.Rotation = Rotation;

            IGTPolylineGeometry LeaderLine2 = GTClassFactory.Create<IGTPolylineGeometry>();


            if (LeaderLine != null)
            {
                for (int i = LeaderLine.Points.Count - 1; i >= 0; i--)
                {
                    LeaderLine2.Points.Add(LeaderLine.Points[i]);
                }
            }
            SectLabelLeaderLine temp = new SectLabelLeaderLine();
            temp.Label = oTextGeom;
            temp.LeaderLine = LeaderLine2;
            SectLabels.Add(temp);

            int lengthRest = int.Parse(txtTotalLength.Text);
            if (Sections.Count > 0)
            {
                for (int i = 0; i < Sections.Count; i++)
                {
                    lengthRest -= int.Parse(Sections[i].SectionLength);
                }
            }
           

        }
        #endregion

        #region Filling element of section's list
        private DuctPathSect FillSection()
        {
            DuctPathSect S = new DuctPathSect();
            S.NumDuctWaysSect = txtNumDuctWaysSect.Text;
            S.SectionLength = txtSectionLength.Text;
            S.SectDiam = cbSectDiam.SelectedValue.ToString();
            S.SectOwner = cbSectOwner.SelectedValue.ToString();
            S.SectType = cbSectType.SelectedValue.ToString();
            S.SectPlc = cbSectPlc.SelectedValue.ToString();
            S.SectBillingRate = cbSectBillingRate.SelectedValue.ToString();
            S.Encasement = cbEncasement.SelectedValue.ToString();
            S.SectBackFill = cbSectBackFill.SelectedValue.ToString();
            S.PUSect = mcbMinMaterial.Text;
            if (TotalGraphicLength != TotalEnteredLength)
            {
                if (TotalEnteredLength == 0) TotalEnteredLength = 1;
                S.SectGraphicLength = TotalGraphicLength * int.Parse(txtSectionLength.Text) / TotalEnteredLength;
            }
            else S.SectGraphicLength = int.Parse(txtSectionLength.Text);


          //  S.YearExpanded = txtYearExpanded.Text;
           // S.YearExtended = txtYearExtended.Text;
            int year1;

            if (txtYearExpanded.Text.Trim() != "0" &&
                                txtYearExpanded.Text.Trim() != "00" &&
                                txtYearExpanded.Text.Trim() != "000" &&
                                txtYearExpanded.Text.Trim() != "0000" &&
                                txtYearExpanded.Text.Trim() != "")
            {
                if (int.TryParse(txtYearExpanded.Text.Trim(), out year1))
                    S.YearExpanded = new DateTime(year1, 1, 1);
                else S.YearExpanded = null;
            }
            else S.YearExpanded = null;

            if (txtYearExtended.Text.Trim() != "0" &&
                                txtYearExtended.Text.Trim() != "00" &&
                                txtYearExtended.Text.Trim() != "000" &&
                                txtYearExtended.Text.Trim() != "0000" &&
                                txtYearExtended.Text.Trim() != "")
            {
                if (int.TryParse(txtYearExtended.Text.Trim(), out year1))
                    S.YearExtended = new DateTime(year1, 1, 1);
                else S.YearExtended = null;
            }
            else S.YearExtended = null;

            return S;
        }
        #endregion

       
     
        #endregion

        #region ZoomIn/ZoomOut

        public void LocateFeature(int flag, IGTMapWindow window)
        {
            if (window == null) return;
            IGTDDCKeyObjects feat =null;
            short iFNO = SourceFNO;
            int lFID = SourceFID;
            
             GTDuctPathPlc.m_gtapp.SelectedObjects.Clear();
             
            if (flag == 1)//fit for source feature
             {
                 feat = GTDuctPathPlc.m_gtapp.DataContext.GetDDCKeyObjects(iFNO, lFID, GTComponentGeometryConstants.gtddcgAllGeographic);
                

                 for (int K = 0; K < feat.Count; K++)
                 {
                     string t = feat[K].ComponentViewName.ToString();
                     if (feat[K].ComponentViewName == "VGC_MANHL_S" ||
                         feat[K].ComponentViewName == "VGC_PSGCON_S")
                     {
                         //2700 VGC_MANHL_S  
                         //2800 VGC_PSGCON_S
                         IGTWorldRange range = GTClassFactory.Create<IGTWorldRange>();
                         IGTPoint point1 = GTClassFactory.Create<IGTPoint>();
                         IGTPoint point2 = GTClassFactory.Create<IGTPoint>();
                         point1.X = feat[K].Geometry.FirstPoint.X - 3;
                         point1.Y = feat[K].Geometry.FirstPoint.Y - 3;
                         range.BottomLeft = point1;
                         point2.X = feat[K].Geometry.FirstPoint.X + 3;
                         point2.Y = feat[K].Geometry.FirstPoint.Y + 3;
                         range.TopRight = point2;
                         window.ZoomArea(range);
                         GTDuctPathPlc.m_gtapp.RefreshWindows();
                         GTDuctPathPlc.m_gtapp.SelectedObjects.Clear();
                         return;
                     }

                     if (feat[K].ComponentViewName == "VGC_CHAMBER_P" ||
                         feat[K].ComponentViewName == "VGC_TUNNEL_P" )
                     {
                         //3800 VGC_CHAMBER_P
                         //3300 VGC_TUNNEL_P
                         for (int K2 = 0; K2 < feat.Count; K2++)
                             GTDuctPathPlc.m_gtapp.SelectedObjects.Add(GTSelectModeConstants.gtsosmAllDisplayedComponentsInActiveLegend, feat[K2]);
                         window.FitSelectedObjects();
                         window.CenterSelectedObjects();
                         GTDuctPathPlc.m_gtapp.RefreshWindows();
                         GTDuctPathPlc.m_gtapp.SelectedObjects.Clear();
                         return;
                     }
                 }
                 
             }
             if (flag == 2)//copy source feature to selected obj to fit both source and term
             {
                 feat = GTDuctPathPlc.m_gtapp.DataContext.GetDDCKeyObjects(iFNO, lFID, GTComponentGeometryConstants.gtddcgAllGeographic);
                 for (int K = 0; K < feat.Count; K++)
                     GTDuctPathPlc.m_gtapp.SelectedObjects.Add(GTSelectModeConstants.gtsosmAllDisplayedComponentsInActiveLegend, feat[K]);
             }
             iFNO = TermFNO;
             lFID = TermFID;// int.Parse(txtFIDTerm.Text);
           
            if (flag == 3)//fit for term feature
            {
                GTDuctPathPlc.m_gtapp.SelectedObjects.Clear();           
                feat = GTDuctPathPlc.m_gtapp.DataContext.GetDDCKeyObjects(iFNO, lFID, GTComponentGeometryConstants.gtddcgAllGeographic);
               
                for (int K = 0; K < feat.Count; K++)
                {
                    string t = feat[K].ComponentViewName.ToString();
                    if (feat[K].ComponentViewName == "VGC_MANHL_S" ||
                        feat[K].ComponentViewName == "VGC_PSGCON_S")
                    {
                        //2700 VGC_MANHL_S  
                        //2800 VGC_PSGCON_S
                        IGTWorldRange range = GTClassFactory.Create<IGTWorldRange>();
                        IGTPoint point1 = GTClassFactory.Create<IGTPoint>();
                        IGTPoint point2 = GTClassFactory.Create<IGTPoint>();
                        point1.X = feat[K].Geometry.FirstPoint.X - 3;
                        point1.Y = feat[K].Geometry.FirstPoint.Y - 3;
                        range.BottomLeft = point1;
                        point2.X = feat[K].Geometry.FirstPoint.X + 3;
                        point2.Y = feat[K].Geometry.FirstPoint.Y + 3;
                        range.TopRight = point2;
                        window.ZoomArea(range);
                        GTDuctPathPlc.m_gtapp.RefreshWindows();
                        GTDuctPathPlc.m_gtapp.SelectedObjects.Clear();
                        return;
                    }

                    if (feat[K].ComponentViewName == "VGC_CHAMBER_P" ||
                        feat[K].ComponentViewName == "VGC_TUNNEL_P")
                    {
                        //3800 VGC_CHAMBER_P
                        //3300 VGC_TUNNEL_P
                        for (int K2 = 0; K2 < feat.Count; K2++)
                            GTDuctPathPlc.m_gtapp.SelectedObjects.Add(GTSelectModeConstants.gtsosmAllDisplayedComponentsInActiveLegend, feat[K2]);
                        window.FitSelectedObjects();
                        window.CenterSelectedObjects();
                        GTDuctPathPlc.m_gtapp.RefreshWindows();
                        GTDuctPathPlc.m_gtapp.SelectedObjects.Clear();
                        return;
                    }
                }
                 
            }
           //copy term feature to selected obj to fit both source and term
            feat = GTDuctPathPlc.m_gtapp.DataContext.GetDDCKeyObjects(iFNO, lFID, GTComponentGeometryConstants.gtddcgAllGeographic);
            for (int K = 0; K < feat.Count; K++)
                GTDuctPathPlc.m_gtapp.SelectedObjects.Add(GTSelectModeConstants.gtsosmAllDisplayedComponentsInActiveLegend, feat[K]);

            window.CenterSelectedObjects();
            window.FitSelectedObjects();
            GTDuctPathPlc.m_gtapp.RefreshWindows();
           // GTDuctPathPlc.m_gtapp.SelectedObjects.Clear();
        }
       
        #endregion

        #region SectionLabelContent
        //IF RECORD_2.DT_S_ENCASE	= "Y" THEN 
        //    "[" 
        //ENDIF
        
        //RECORD_2.DT_S_WAYS
		
		
        
        //IF RECORD_2.DT_S_ENCASE	= "Y" THEN 
        //    "]" 
        //ENDIF
		
        //" "
		
	
        //IF RECORD_2.DT_S_TYPE != "PVC"  then
        //    RECORD_2.DT_S_TYPE ","
        //ENDIF
                
        //IF RECORD_2.DT_S_PLACMNT = "GRASSVERGE"	THEN
        //    "GV"
        //ENDIF
        //IF RECORD_2.DT_S_PLACMNT = "CONCRETE" THEN
        //    "CO"
        //ENDIF
        //IF RECORD_2.DT_S_PLACMNT = "BITUMIN" THEN
        //    "BI"
        //ENDIF
        //IF RECORD_2.DT_S_PLACMNT = "PIPE JACK" THEN
        //    "PJ"
        //ENDIF
        //IF RECORD_2.DT_S_PLACMNT = "THRUST BORE" THEN
        //    "TB"
        //ENDIF
        //IF RECORD_2.DT_S_PLACMNT = "PIER" THEN
        //    "PIER"
        //ENDIF
        //IF RECORD_2.DT_S_PLACMNT = "CARRIAGEWAY" THEN
        //    "CW"
        //ENDIF
        //IF RECORD_2.DT_S_PLACMNT = "BRIDGE ATTACHMENT" THEN
        //            "BA"
        //ENDIF
        //IF RECORD_2.DT_S_PLACMNT = "HORIZONTAL DIRECT DRILLING" THEN
        //            "HDD"
        //ENDIF
        //IF RECORD_2.DT_S_PLACMNT = "DRAIN" THEN
        //    "DR"
        //ENDIF
        //IF RECORD_2.DT_S_PLACMNT = "ROCKY TERRAIN" THEN
        //"RT"
        //ENDIF
        
        //IF RECORD_1.EXPAND_FLAG = "1" THEN
        //    "+"
        //ENDIF
        		
        //" "
		
        //IF RECORD_2.YEAR_EXPANDED != "0"	THEN 
        //    ",EXP(" RECORD_2.YEAR_EXPANDED ")"
        //ENDIF
        //IF RECORD_2.YEAR_EXTENDED != "0"	THEN 
        //    ",EXT(" RECORD_2.YEAR_EXTENDED ")"
        //ENDIF
       
        //IF RECORD_2.COMMON_TRENCH = "TELEKOM MALAYSIA, TM" THEN
        //    "(CT:TM)"
        //ENDIF
        //IF RECORD_2.COMMON_TRENCH = "DEVELOPER, DEV" THEN
        //    "(CT:DEV)"
        //ENDIF
        //IF RECORD_2.COMMON_TRENCH = "BINARIANG, BIN" THEN
        //    "(CT:BIN)"
        //ENDIF
        //IF RECORD_2.COMMON_TRENCH = "CELCOM, CEL" THEN
        //    "(CT:CEL)"
        //ENDIF
        //IF RECORD_2.COMMON_TRENCH = "FIBRERAIL, FIB" THEN
        //    "(CT:FIB)"
        //ENDIF
        //IF RECORD_2.COMMON_TRENCH = "MOBIKOM, MOB" THEN
        //    "(CT:MOB)"
        //ENDIF
        //IF RECORD_2.COMMON_TRENCH = "MUTIARA, MUT" THEN
        //    "(CT:MUT)"
        //ENDIF
        //IF RECORD_2.COMMON_TRENCH = "STW, STW" THEN
        //    "(CT:STW)"
        //ENDIF
        //IF RECORD_2.COMMON_TRENCH = "TIME, TIM" THEN
        //    "(CT:TIM)"
        //ENDIF
		
		
        
        
        //"\n" RECORD_2.DT_S_LENGTH "m"
        public string SectLabelContent(int SectNum)
        {
            string Content = "";

            if (Sections[SectNum].Encasement == "Y")
                Content += "[";
            
            Content += Sections[SectNum].NumDuctWaysSect;
            
            if (Sections[SectNum].Encasement == "Y")
                Content += "]";

            Content += " ";

            if (Sections[SectNum].SectType != "PVC")
                Content += Sections[SectNum].SectType + ",";

            if (Sections[SectNum].SectPlc == "GRASSVERGE")
                Content += "GV";
            else if (Sections[SectNum].SectPlc == "CONCRETE")
                Content += "CO";
            else if (Sections[SectNum].SectPlc == "BITUMIN")
                Content += "BI";
            else if (Sections[SectNum].SectPlc == "PIPE JACK")
                Content += "PJ";
            else if (Sections[SectNum].SectPlc == "THRUST BORE")
                Content += "TB";
            else if (Sections[SectNum].SectPlc == "PIER")
                Content += "PIER";
            else if (Sections[SectNum].SectPlc == "BRIDGE ATTACHMENT")
                Content += "BA";
            else if (Sections[SectNum].SectPlc == "HORIZONTAL DIRECT DRILLING")
                Content += "HDD";
            else if (Sections[SectNum].SectPlc == "DRAIN")
                Content += "DR";
            else if (Sections[SectNum].SectPlc == "ROCKY TERRAIN")
                Content += "RT";

            //Content += " ";

            //if (Sections[SectNum].SectOwner == "TELEKOM MALAYSIA, TM")
            //    Content += "(CT:TM)";
            //else if (Sections[SectNum].SectOwner == "DEVELOPER, DEV")
            //    Content += "(CT:DEV)";
            //else if (Sections[SectNum].SectOwner == "BINARIANG, BIN")
            //    Content += "(CT:BIN)";
            //else if (Sections[SectNum].SectOwner == "CELCOM, CEL")
            //    Content += "(CT:CEL)";
            //else if (Sections[SectNum].SectOwner == "FIBRERAIL, FIB")
            //    Content += "(CT:FIB)";
            //else if (Sections[SectNum].SectOwner == "MOBIKOM, MOB")
            //    Content += "(CT:MOB)";
            //else if (Sections[SectNum].SectOwner == "MUTIARA, MUT")
            //    Content += "(CT:MUT)";
            //else if (Sections[SectNum].SectOwner == "STW, STW")
            //    Content += "(CT:STW)";
            //else if (Sections[SectNum].SectOwner == "TIME, TIM")
            //    Content += "(CT:TIM)";

            Content += "\n" +Sections[SectNum].SectionLength+ " m";
            return Content;
        }
        #endregion

     

      


    }
      
}