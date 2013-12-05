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

namespace NEPS.GTechnology.NEPSDuctPathEdit
{
    public partial class Form_DuctPathEdit : Form
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
        public int ChangeNumberDuctWayPPF = -1;
        public List<DuctPathSect> SectionsChangeNumberDuctWayPPF = null;
        public class DuctPathSect
        {
            public string NumDuctWaysSect;
            public string SectionLength;
            public int SectGraphicLength;
            public string SectDiam;
            public string YearExpanded;
            public string YearExtended;
            public string SectOwner;
            public string SectType;
            public string SectPlc;
            public string SectBillingRate;
            public string Encasement;
            public string SectBackFill;
            public string PUSect;
            public short CID;
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
      
        public bool ConfSelBtnIsEnable
        {
            get { return btnConfirmSectAttr.Enabled; }
            set { btnConfirmSectAttr.Enabled = value; }
        }
        #endregion

        #region init and load form
        public Form_DuctPathEdit()
        {
            try
            {
                InitializeComponent();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Duct Path Edit", MessageBoxButtons.OK, MessageBoxIcon.Error);
                CloseStatus = 1;
                this.Close();
            }
        }



        private void GTWindowsForm_DuctPathPlc_Load(object sender, EventArgs e)
        {
           FillingAttrComboBoxes();
           FillingOriginalAttr();   
        }

        private void GTWindowsForm_DuctPathPlc_Shown(object sender, EventArgs e)
        {
            GTDuctPathEdit.m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "");

        }
       
         #region filling first page
        #region filling combobox
        private void FillingAttrComboBoxes()
        {

            if (cbConstructed.DataSource == null)
            {
                ADODB.Recordset rsPP = new ADODB.Recordset();
                string sSql = "SELECT PL_VALUE FROM REF_COM_CONSTRUCTION";
                rsPP = GTDuctPathEdit.m_IGTDataContext.OpenRecordset(sSql, ADODB.CursorTypeEnum.adOpenStatic, ADODB.LockTypeEnum.adLockReadOnly, 1);
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
            if (cbBillingRate.DataSource == null)
            {
                ADODB.Recordset rsPP = new ADODB.Recordset();
                string sSql = "select pl_value, pl_num from  REF_COM_BILLRATE";
                rsPP = GTDuctPathEdit.m_IGTDataContext.OpenRecordset(sSql, ADODB.CursorTypeEnum.adOpenStatic, ADODB.LockTypeEnum.adLockReadOnly, 1);
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
            if (cbNumDuctWays.DataSource == null)
            {
                cbNumDuctWays.Items.Clear();
                string sSql = " select distinct DT_S_WAYS from ref_civ_ductpath  order by to_number(DT_S_WAYS) asc ";
                ADODB.Recordset rsPP = GTDuctPathEdit.m_IGTDataContext.OpenRecordset(sSql, ADODB.CursorTypeEnum.adOpenStatic, ADODB.LockTypeEnum.adLockReadOnly, 1);

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
        }
         #endregion

        private void FillingOriginalAttr()
        {
            txtDuctPathFID.Text = GTDuctPathEdit.DuctPathOrigin.FID.ToString();
            txtDuctSourceFID.Text =GTDuctPathEdit.DuctPathOrigin.sourceFID.ToString();
            txtDuctSourceType.Text = GTDuctPathEdit.DuctPathOrigin.sourceType;
            txtDuctSourceWall.Text = GTDuctPathEdit.DuctPathOrigin.sourceWall.ToString();
            txtDuctTermFID.Text = GTDuctPathEdit.DuctPathOrigin.termFID.ToString();
            txtDuctTermType.Text = GTDuctPathEdit.DuctPathOrigin.termType;
            txtDuctTermWall.Text = GTDuctPathEdit.DuctPathOrigin.termWall.ToString();
            txtYearIns.Text = GTDuctPathEdit.DuctPathOrigin.InstallYear;
            txtExcAbb.Text = GTDuctPathEdit.DuctPathOrigin.EXC_ABB;
            txtFlag.Text = GTDuctPathEdit.DuctPathOrigin.DBFlag;
            txtTotalLength.Text = GTDuctPathEdit.DuctPathOrigin.Length.ToString();
            if (cbNumDuctWays.DataSource != null)
            {
                int i = 0;
                for (; i < cbNumDuctWays.Items.Count; i++)
                {
                    cbNumDuctWays.SelectedItem = cbNumDuctWays.Items[i];
                    if (cbNumDuctWays.SelectedValue.ToString() == GTDuctPathEdit.DuctPathOrigin.DuctWay.ToString())
                        break;
                }
                if (i == cbNumDuctWays.Items.Count)
                    cbNumDuctWays.SelectedIndex = 0;
            }
            if (GTDuctPathEdit.DuctPathOrigin.Feature_state == "ASB")
                cbNumDuctWays.Enabled = false;
            if (GTDuctPathEdit.DuctPathOrigin.Feature_state == "PPF")
                cbNumDuctWays.Enabled = true;
            if (cbConstructed.DataSource != null)
            {
                int i = 0;
                for (; i < cbConstructed.Items.Count; i++)
                {
                    cbConstructed.SelectedItem = cbConstructed.Items[i];
                    if (cbConstructed.SelectedValue.ToString().Contains(GTDuctPathEdit.DuctPathOrigin.ConstructBy) )
                        break;
                }
                if (i == cbConstructed.Items.Count)
                    cbConstructed.SelectedIndex = 0;
            }

            if (cbBillingRate.DataSource != null)
            {
                int i = 0;
                for (; i < cbBillingRate.Items.Count; i++)
                {
                    cbBillingRate.SelectedItem = cbBillingRate.Items[i];
                    if (cbBillingRate.SelectedValue.ToString().Contains(GTDuctPathEdit.DuctPathOrigin.BillingRate))
                        break;
                }
                if (i == cbBillingRate.Items.Count)
                    cbBillingRate.SelectedIndex = 0;
            }           
        }
         #endregion
        #endregion

        #region Closing Form
        private void GTWindowsForm_DuctPathPlc_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (CloseStatus == 0)
            {
                DialogResult retVal = MessageBox.Show("Are you sure that you want to exit?", "Duct Path Edit", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (retVal == DialogResult.Yes)
                {
                    statusPage = 0;
                    if (GTDuctPathEdit.mobjEditService != null)
                        GTDuctPathEdit.mobjEditService.RemoveAllGeometries();
                    GTDuctPathEdit.m_gtapp.SelectedObjects.Clear();
                  //  GTDuctPathEdit.startdraw = 0;
                   // tabDuctPlc.SelectedTab = tabPagePLC;
                    LastSection = 0;
                    if(SectSlashes!=null)
                        SectSlashes.Clear();
                    if(SectLabels!=null)
                        SectLabels.Clear();
                    if(Sections!=null)
                        Sections.Clear();
                    SourceDevicePoint = null;
                    TermDevicePoint = null;
                   // GTDuctPathEdit.StartDrawPoint = null;
                   // GTDuctPathEdit.EndDrawPoint = null;
                    if (DuctPathLineGeom != null)
                    {
                        DuctPathLineGeom.Points.Clear();
                        DuctPathLineGeom = null;
                    }
                   // btnGetSelSource.Enabled = true;
                   // txtFIDSource.Text = "";
                    //txtFIDTerm.Text = "";
                    //txtTypeSource.Text = "";
                    //txtTypeTerm.Text = "";
                    //txtWallSource.Text = "";
                    //txtWallTerm.Text = "";
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
                if (GTDuctPathEdit.mobjEditService != null)
                    GTDuctPathEdit.mobjEditService.RemoveAllGeometries();
                GTDuctPathEdit.m_gtapp.SelectedObjects.Clear();
               // GTDuctPathEdit.startdraw = 0;
                //tabDuctPlc.SelectedTab = tabPagePLC;
                LastSection = 0;
                if (SectSlashes != null)
                    SectSlashes.Clear();
                if (SectLabels != null)
                    SectLabels.Clear();
                if (Sections != null)
                    Sections.Clear();
                SourceDevicePoint = null;
                TermDevicePoint = null;
               // GTDuctPathEdit.StartDrawPoint = null;
               // GTDuctPathEdit.EndDrawPoint = null;
                if (DuctPathLineGeom != null)
                {
                    DuctPathLineGeom.Points.Clear();
                    DuctPathLineGeom = null;
                }
                //btnGetSelSource.Enabled = true;
                //txtFIDSource.Text = "";
                //txtFIDTerm.Text = "";
                //txtTypeSource.Text = "";
                //txtTypeTerm.Text = "";
                //txtWallSource.Text = "";
                //txtWallTerm.Text = "";
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
                string strActiveJob = GTDuctPathEdit.m_IGTDataContext.ActiveJob;
                sSql = "select work_order_id,g3e_description,g3e_identifier from g3e_job where g3e_identifier ='" + strActiveJob + "'";
                rsWorkOrder = GTDuctPathEdit.m_IGTDataContext.OpenRecordset(sSql, ADODB.CursorTypeEnum.adOpenStatic, ADODB.LockTypeEnum.adLockReadOnly, 1);
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
                MessageBox.Show(ex.Message, "Duct Path Edit", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                rsPP = GTDuctPathEdit.m_IGTDataContext.OpenRecordset(sSql, ADODB.CursorTypeEnum.adOpenStatic, ADODB.LockTypeEnum.adLockReadOnly, 1);
                if (rsPP.RecordCount > 0)
                {
                    rsPP.MoveFirst();
                    return (rsPP.Fields[0].Value.ToString());
                }
                return "";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Duct Path Edit", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                tabDuctEdit.SelectedTab = tabPageAttr;
            }
            if (statusPage == 1)
            {
                tabDuctEdit.SelectedTab = tabPageListSect;
            }
            if (statusPage == 2)
            {
                tabDuctEdit.SelectedTab = tabPageSect;
            }
        }
        #endregion

        #region Page 1(4 steps to draw conduit)

        #region Re-Own Duct Path

        #region Select Wall source
        private void btnWallSource_Click(object sender, EventArgs e)
        {
            if (txtDuctSourceType.Text == "Civil Node")
            {
                MessageBox.Show("Please select Manhole, Chamber or Tunnel first!", "Duct Path Edit", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                LocateFeature(1, GTDuctPathEdit.m_gtapp.ActiveMapWindow);
                this.Hide();
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

            //if (flag)
            //{
            //    FeatureType = txtTypeSource.Text;
            //    iFIDSelected = int.Parse(txtFIDSource.Text);
            //}
            //else
            //{
            //    FeatureType = txtTypeTerm.Text;
            //    iFIDSelected = int.Parse(txtFIDTerm.Text);
            //}

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
            if (GTDuctPathEdit.m_gtapp.SelectedObjects.FeatureCount == 0)
            {
                MessageBox.Show("Please select first Wall of selected " + FeatureType + " with FID = " + iFIDSelected.ToString() + " !", "Duct Path Edit", MessageBoxButtons.OK, MessageBoxIcon.Information);
                GTDuctPathEdit.m_gtapp.SelectedObjects.Clear();
                return false;
            }

            if (GTDuctPathEdit.m_gtapp.SelectedObjects.FeatureCount > 1)
            {
                MessageBox.Show("Please select first Wall of selected " + FeatureType + " with FID = " + iFIDSelected.ToString() + " !", "Duct Path Edit", MessageBoxButtons.OK, MessageBoxIcon.Information);
                GTDuctPathEdit.m_gtapp.SelectedObjects.Clear();
                return false;
            }


            foreach (IGTDDCKeyObject oDDCKeyObject in GTDuctPathEdit.m_gtapp.SelectedObjects.GetObjects())
            {
                geom = oDDCKeyObject.Geometry;
                iFNO = oDDCKeyObject.FNO;
                iFID = oDDCKeyObject.FID;
               
                if(iFNOSelected != iFNO)
                {
                    MessageBox.Show("Please select first Wall of selected " + FeatureType + " with FID = " + iFIDSelected.ToString() + " !", "Duct Path Edit", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    GTDuctPathEdit.m_gtapp.SelectedObjects.Clear();
                    return false;
                }
                if (iFIDSelected != iFID)
                {
                    MessageBox.Show("Please select first Wall of selected " + FeatureType + " with FID = " + iFIDSelected.ToString() + " !", "Duct Path Edit", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    GTDuctPathEdit.m_gtapp.SelectedObjects.Clear();
                    return false;
                }

                //manhole
                if (iFNO == 2700)
                {
                    if (oDDCKeyObject.ComponentViewName == "VGC_MANHLW_T")
                    {
                        WallNum = oDDCKeyObject.Recordset.Fields[4].Value.ToString();
                        break;
                    }
                }
                //chamber
                else if (iFNO == 3800)
                {
                    if (oDDCKeyObject.ComponentViewName == "VGC_CHAMBERWALL_T")
                    {
                        WallNum = oDDCKeyObject.Recordset.Fields[4].Value.ToString();
                        break;
                    }
                }
                //tunnel/trench
                else if (iFNO == 3300)
                {
                    if (oDDCKeyObject.ComponentViewName == "VGC_TUNNELWALL_T")
                    {
                        WallNum = oDDCKeyObject.Recordset.Fields[4].Value.ToString();
                        break;
                    }
                }

                MessageBox.Show("Please select first Wall of selected " + FeatureType + " with FID = " + iFIDSelected.ToString() + " !", "Duct Path Edit", MessageBoxButtons.OK, MessageBoxIcon.Information);
                GTDuctPathEdit.m_gtapp.SelectedObjects.Clear();
                return false;

            }
            #endregion
            if (flag)
            {
                txtDuctSourceWall.Text = WallNum;
                if (SourceDevicePoint == null)
                    SourceDevicePoint = GTClassFactory.Create<IGTPoint>();
                SourceDevicePoint.X = geom.FirstPoint.X;
                SourceDevicePoint.Y = geom.FirstPoint.Y;
                SourceDevicePoint.Z = geom.FirstPoint.Z;

                //if (GTDuctPathEdit.StartDrawPoint == null)
                //    GTDuctPathEdit.StartDrawPoint = GTClassFactory.Create<IGTPoint>();
                //GTDuctPathEdit.StartDrawPoint.X = SourceDevicePoint.X;
                //GTDuctPathEdit.StartDrawPoint.Y = SourceDevicePoint.Y;
                //GTDuctPathEdit.StartDrawPoint.Z = SourceDevicePoint.Z;

                #region redraw first section of temporary geometry

                if (GTDuctPathEdit.mobjEditService != null)
                {
                    if (GTDuctPathEdit.mobjEditService.GeometryCount > 0)
                    {
                        GTDuctPathEdit.m_gtapp.SelectedObjects.Clear();

                        IGTPolylineGeometry oLineGeom = GTClassFactory.Create<IGTPolylineGeometry>();

                        oLineGeom.Points.Add(SourceDevicePoint);
                        for (int i = 1; i <= GTDuctPathEdit.mobjEditService.GeometryCount; i++)
                            oLineGeom.Points.Add(GTDuctPathEdit.mobjEditService.GetGeometry(i).LastPoint);

                        //GTDuctPathEdit.StartDrawPoint.X = oLineGeom.Points[oLineGeom.Points.Count - 2].X;
                        //GTDuctPathEdit.StartDrawPoint.Y = oLineGeom.Points[oLineGeom.Points.Count - 2].Y;
                        //GTDuctPathEdit.StartDrawPoint.Z = oLineGeom.Points[oLineGeom.Points.Count - 2].Z;

                        GTDuctPathEdit.mobjEditService.RemoveAllGeometries();

                        for (int i = 0; i < oLineGeom.Points.Count - 1; i++)
                        {
                            IGTPolylineGeometry oLineGeom1 = GTClassFactory.Create<IGTPolylineGeometry>();
                            oLineGeom1.Points.Add(oLineGeom.Points[i]);
                            oLineGeom1.Points.Add(oLineGeom.Points[i + 1]);
                            GTDuctPathEdit.mobjEditService.AddGeometry(oLineGeom1, 14500);
                        }


                    }
                    else
                    {
                        //if (GTDuctPathEdit.EndDrawPoint != null)
                        //{
                        //    IGTPolylineGeometry oLineGeom = GTClassFactory.Create<IGTPolylineGeometry>();
                        //    oLineGeom.Points.Add(GTDuctPathEdit.StartDrawPoint);
                        //    oLineGeom.Points.Add(GTDuctPathEdit.EndDrawPoint);
                        //    GTDuctPathEdit.mobjEditService.AddGeometry(oLineGeom, 14500);
                        //}
                    }
                }
                #endregion
            }
            else
            {
                txtDuctTermWall.Text = WallNum;
                if (TermDevicePoint == null)
                    TermDevicePoint = GTClassFactory.Create<IGTPoint>();
                TermDevicePoint.X = geom.FirstPoint.X;
                TermDevicePoint.Y = geom.FirstPoint.Y;
                TermDevicePoint.Z = geom.FirstPoint.Z;


                //if (GTDuctPathEdit.EndDrawPoint == null)
                //    GTDuctPathEdit.EndDrawPoint = GTClassFactory.Create<IGTPoint>();
                //GTDuctPathEdit.EndDrawPoint = TermDevicePoint;
                //GTDuctPathEdit.EndDrawPoint.X = TermDevicePoint.X;
                //GTDuctPathEdit.EndDrawPoint.Y = TermDevicePoint.Y;
                //GTDuctPathEdit.EndDrawPoint.Z = TermDevicePoint.Z;
               
                if (GTDuctPathEdit.mobjEditService != null)
                {
               //     if (GTDuctPathEdit.StartDrawPoint!=null)//GTDuctPathEdit.mobjEditService.GeometryCount > 0)
                    {
                        GTDuctPathEdit.m_gtapp.SelectedObjects.Clear();
                        IGTPolylineGeometry oLineGeom = GTClassFactory.Create<IGTPolylineGeometry>();

                     //   oLineGeom.Points.Add(GTDuctPathEdit.StartDrawPoint);
                     //   oLineGeom.Points.Add(GTDuctPathEdit.EndDrawPoint);
                        if (GTDuctPathEdit.mobjEditService.GeometryCount > 0)
                            GTDuctPathEdit.mobjEditService.RemoveGeometry(GTDuctPathEdit.mobjEditService.GeometryCount);

                        GTDuctPathEdit.mobjEditService.AddGeometry(oLineGeom, 14500);
                    }
                }
            }
            GTDuctPathEdit.m_gtapp.SelectedObjects.Clear();
            return true;
        }
        #endregion

        #region Select Wall term
        private void btnWallTerm_Click(object sender, EventArgs e)
        {
            if (txtDuctTermType.Text == "Civil Node")
            {
                MessageBox.Show("Please select Manhole, Chamber or Tunnel first!", "Duct Path Edit", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                LocateFeature(3, GTDuctPathEdit.m_gtapp.ActiveMapWindow);
                this.Hide();
            }
        }
        #endregion
       
        #endregion
       
        #region Button close2 click
        private void btnClose2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
          #endregion 

       
        #region button Confirm for attr click
        private void btnConfirmDuctAttr_Click(object sender, EventArgs e)
        {
            //  if (ValidateDuctPathAttr())
            // {
            GTDuctPathEdit.m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Wait, updating in process...");
            gbDuctPathAttrValues.Enabled = false;
            btnSaveDuctAttr.Enabled = false;
            btnClose2.Enabled = false;
            btnNext.Enabled = false;
            lbWait.Visible = true;
            if(!ChangeNumberDuctWay())
            {


                if (!UpdateDuctPathAttributes(GTDuctPathEdit.DuctPathOrigin.FID))
                 MessageBox.Show("Error during update Duct Path attributes", "Duct Path Edit", MessageBoxButtons.OK, MessageBoxIcon.Error); 
                else
                {
                    ChangeNumberDuctWayPPF = -1;
                    if(SectionsChangeNumberDuctWayPPF!=null)
                        SectionsChangeNumberDuctWayPPF.Clear();
                }
                
               
            }
            gbDuctPathAttrValues.Enabled = true;
            btnSaveDuctAttr.Enabled = true;
            btnClose2.Enabled = true;
            lbWait.Visible = false;
            btnNext.Enabled = true;
            GTDuctPathEdit.m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "");
            //  }
        }
        private bool ChangeNumberDuctWay()
        {
            if (GTDuctPathEdit.DuctPathOrigin.DuctWay == int.Parse(cbNumDuctWays.SelectedValue.ToString()))
            {
                ChangeNumberDuctWayPPF = -1;
                return false;
            }
            ChangeNumberDuctWayPPF=0;
            if (SectionsChangeNumberDuctWayPPF == null)
                SectionsChangeNumberDuctWayPPF = new List<DuctPathSect>();
            if (GTDuctPathEdit.DuctPathOrigin.Sections.Count > 0)
            {
                for (int i = 0; i < GTDuctPathEdit.DuctPathOrigin.Sections.Count; i++)
                {
                    DuctPathSect temp = new DuctPathSect();
                    temp.CID = GTDuctPathEdit.DuctPathOrigin.Sections[i].CID;
                    temp.Encasement = GTDuctPathEdit.DuctPathOrigin.Sections[i].Encasement;
                    temp.NumDuctWaysSect = GTDuctPathEdit.DuctPathOrigin.Sections[i].NumDuctWaysSect;
                    temp.PUSect = GTDuctPathEdit.DuctPathOrigin.Sections[i].PUSect;
                    temp.SectBackFill = GTDuctPathEdit.DuctPathOrigin.Sections[i].SectBackFill;
                    temp.SectBillingRate = GTDuctPathEdit.DuctPathOrigin.Sections[i].SectBillingRate;
                    temp.SectDiam = GTDuctPathEdit.DuctPathOrigin.Sections[i].SectDiam;
                    if(GTDuctPathEdit.DuctPathOrigin.Sections[i].SectGraphicLength!=null)
                        temp.SectGraphicLength = GTDuctPathEdit.DuctPathOrigin.Sections[i].SectGraphicLength;
                    temp.SectionLength = GTDuctPathEdit.DuctPathOrigin.Sections[i].SectionLength;
                    temp.SectOwner = GTDuctPathEdit.DuctPathOrigin.Sections[i].SectOwner;
                    temp.SectPlc = GTDuctPathEdit.DuctPathOrigin.Sections[i].SectPlc;
                    temp.SectType = GTDuctPathEdit.DuctPathOrigin.Sections[i].SectType;
                    SectionsChangeNumberDuctWayPPF.Add(temp);
                }
                for (int i = 0; i < SectionsChangeNumberDuctWayPPF.Count; i++)
                {
                  string  minmat = Get_Value("select MIN_MATERIAL from ref_civ_ductpath " +
                   " where DT_S_TYPE='" + SectionsChangeNumberDuctWayPPF[i].SectType + 
                   "' and  DT_S_WAYS=" + cbNumDuctWays.SelectedValue.ToString() +
                   " and DT_S_PLACMNT='" + SectionsChangeNumberDuctWayPPF[i].SectPlc +
                   "' and DT_S_ENCASE='" + SectionsChangeNumberDuctWayPPF[i].Encasement +
                    "' and DT_S_BACKFILL='" + SectionsChangeNumberDuctWayPPF[i].SectBackFill + 
                    "' and DT_S_DIAMETER =" + SectionsChangeNumberDuctWayPPF[i].SectDiam);
                  SectionsChangeNumberDuctWayPPF[i].PUSect = minmat;
                  SectionsChangeNumberDuctWayPPF[i].NumDuctWaysSect = cbNumDuctWays.SelectedValue.ToString();
                  if (minmat != "")
                      ChangeNumberDuctWayPPF++;
                }
                if (SectionsChangeNumberDuctWayPPF.Count == ChangeNumberDuctWayPPF)
                    return false;

                DialogResult closetype =MessageBox.Show("Sections' attributes need to be updated \nbecause no matching plant unit with new number of ductways found!"+
                    "\nDo you want to continue?", "Duct Path Placement", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (closetype == DialogResult.No)
                {
                    ChangeNumberDuctWayPPF = -1;
                    if (SectionsChangeNumberDuctWayPPF != null)
                        SectionsChangeNumberDuctWayPPF.Clear();
                    if (cbNumDuctWays.DataSource != null)
                    {
                        int i = 0;
                        for (; i < cbNumDuctWays.Items.Count; i++)
                        {
                            cbNumDuctWays.SelectedItem = cbNumDuctWays.Items[i];
                            if (cbNumDuctWays.SelectedValue.ToString() == GTDuctPathEdit.DuctPathOrigin.DuctWay.ToString())
                                break;
                        }
                        if (i == cbNumDuctWays.Items.Count)
                            cbNumDuctWays.SelectedIndex = 0;
                    }
                }
                else
                {
                    short cid_to_change = 0;
                    for (int i = 0; i < SectionsChangeNumberDuctWayPPF.Count; i++)
                    {
                        if (SectionsChangeNumberDuctWayPPF[i].PUSect == "")
                        {
                            cid_to_change = SectionsChangeNumberDuctWayPPF[i].CID;
                            break;
                        }
                    }
                    FillingPage3ComboBoxes();
                    FillingPage3Sect(cid_to_change);
                    statusPage = 2;
                    tabDuctEdit.SelectedTab = tabPageSect;
                }
                return true;
                
                // minmat = Get_Value("select MIN_MATERIAL from ref_civ_ductpath " +
                //" where DT_S_TYPE='" + cbSectType.SelectedValue.ToString() + "' and  DT_S_WAYS=" + txtNumDuctWaysSect.Text +
                //" and DT_S_PLACMNT='" + cbSectPlc.SelectedValue.ToString() + "' and DT_S_ENCASE='" + cbEncasement.SelectedValue.ToString() +
                // "' and DT_S_BACKFILL='" + cbSectBackFill.SelectedValue.ToString() + "' and DT_S_DIAMETER =" + cbSectDiam.SelectedValue.ToString());

                // if (minmat == "")
                // {
                //     MessageBox.Show("Min_Material type are not matching with the rest of attributes!", "Duct Path Placement", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                //     return false;
                // }
            }
            return false;
        }
        private bool UpdateDuctPathAttributes(int iFID)
        {
            short iFNO = 2200;
            short iCNO;
            if (GTDuctPathEdit.DuctPathOrigin.ConstructBy == cbConstructed.SelectedValue.ToString()
                   //&& (txtFlag.Text == "" && GTDuctPathEdit.DuctPathOrigin.DBFlag != txtFlag.Text.Substring(0, 1))
                   && GTDuctPathEdit.DuctPathOrigin.DBFlag == txtFlag.Text
                   && GTDuctPathEdit.DuctPathOrigin.DuctWay == int.Parse(cbNumDuctWays.SelectedValue.ToString())
                   && GTDuctPathEdit.DuctPathOrigin.InstallYear == txtYearIns.Text
                   && GTDuctPathEdit.DuctPathOrigin.BillingRate == cbBillingRate.SelectedValue.ToString())
                return true;
            try
            {

                GTDuctPathEdit.m_oIGTTransactionManager.Begin("DuctPathAttrUpdate");
                
                IGTKeyObject oNewFeature = GTDuctPathEdit.m_IGTDataContext.OpenFeature(iFNO, iFID);

                if (GTDuctPathEdit.DuctPathOrigin.ConstructBy != cbConstructed.SelectedValue.ToString()
                    || (txtFlag.Text!="" && GTDuctPathEdit.DuctPathOrigin.DBFlag != txtFlag.Text.Substring(0, 1))
                    || GTDuctPathEdit.DuctPathOrigin.DBFlag != txtFlag.Text
                    || GTDuctPathEdit.DuctPathOrigin.DuctWay != int.Parse(cbNumDuctWays.SelectedValue.ToString()))
                {
                    #region Attributes
                    iCNO = 2201;
                    
                        oNewFeature.Components.GetComponent(iCNO).Recordset.MoveLast();
                        oNewFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_FNO", iFNO);
                        oNewFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_CID", 1);
                        if (txtFlag.Text != "")
                            oNewFeature.Components.GetComponent(iCNO).Recordset.Update("CENTRALDB_FLAG", txtFlag.Text);
                        else oNewFeature.Components.GetComponent(iCNO).Recordset.Update("CENTRALDB_FLAG", null);
                        oNewFeature.Components.GetComponent(iCNO).Recordset.Update("DT_CONSTRUCTION", cbConstructed.SelectedValue);
                        oNewFeature.Components.GetComponent(iCNO).Recordset.Update("DT_WAYS",cbNumDuctWays.SelectedValue);

                    #endregion
                        if (GTDuctPathEdit.DuctPathOrigin.DuctWay != int.Parse(cbNumDuctWays.SelectedValue.ToString()))
                        {
                            #region Section Attributes upt
                            iCNO = 2202;

                                if (!oNewFeature.Components.GetComponent(iCNO).Recordset.EOF)
                                    {
                                        oNewFeature.Components.GetComponent(iCNO).Recordset.MoveFirst();
                                        for (int k = 0; k < oNewFeature.Components.GetComponent(iCNO).Recordset.RecordCount; k++)
                                        {
                                            for (int k1 = 0; k1 < oNewFeature.Components.GetComponent(iCNO).Recordset.Fields.Count; k1++)
                                            {
                                                if (oNewFeature.Components.GetComponent(iCNO).Recordset.Fields[k1].Name == "G3E_CID")
                                                {
                                                    int k3 = 0;
                                                    if (SectionsChangeNumberDuctWayPPF.Count > 0)
                                                    {
                                                        for (int i = 0; i < SectionsChangeNumberDuctWayPPF.Count; i++)
                                                        {
                                                            if (oNewFeature.Components.GetComponent(iCNO).Recordset.Fields[k1].Value.ToString() == SectionsChangeNumberDuctWayPPF[i].CID.ToString())
                                                            {
                                                                k3 = i;
                                                                break;
                                                            }
                                                        }
                                                    }
                                                            
                                                                oNewFeature.Components.GetComponent(iCNO).Recordset.Update("DT_S_LENGTH", SectionsChangeNumberDuctWayPPF[k3].SectionLength);
                                                                oNewFeature.Components.GetComponent(iCNO).Recordset.Update("DT_S_TYPE", SectionsChangeNumberDuctWayPPF[k3].SectType);
                                                                oNewFeature.Components.GetComponent(iCNO).Recordset.Update("DT_S_PLACMNT", SectionsChangeNumberDuctWayPPF[k3].SectPlc);
                                                                oNewFeature.Components.GetComponent(iCNO).Recordset.Update("DT_S_ENCASE", SectionsChangeNumberDuctWayPPF[k3].Encasement);
                                                                oNewFeature.Components.GetComponent(iCNO).Recordset.Update("DT_S_DIAMETER", int.Parse(SectionsChangeNumberDuctWayPPF[k3].SectDiam));
                                                                oNewFeature.Components.GetComponent(iCNO).Recordset.Update("DT_S_BACKFILL", SectionsChangeNumberDuctWayPPF[k3].SectBackFill);
                                                                oNewFeature.Components.GetComponent(iCNO).Recordset.Update("COMMON_TRENCH", SectionsChangeNumberDuctWayPPF[k3].SectOwner);
                                                                oNewFeature.Components.GetComponent(iCNO).Recordset.Update("BILLING_RATE", SectionsChangeNumberDuctWayPPF[k3].SectBillingRate);
                                                                oNewFeature.Components.GetComponent(iCNO).Recordset.Update("DT_S_WAYS", SectionsChangeNumberDuctWayPPF[k3].NumDuctWaysSect);
                                                                oNewFeature.Components.GetComponent(iCNO).Recordset.Update("MIN_MATERIAL", SectionsChangeNumberDuctWayPPF[k3].PUSect);
                                                                break;
                                                            
                                                        
                                                    
                                                }

                                            }
                                            oNewFeature.Components.GetComponent(iCNO).Recordset.MoveNext();
                                        }
                                    }
                                
                            
                            progressBar1.Value = 20;
                            #endregion
                        }
                    }
                if (GTDuctPathEdit.DuctPathOrigin.InstallYear != txtYearIns.Text
                    || GTDuctPathEdit.DuctPathOrigin.BillingRate != cbBillingRate.SelectedValue.ToString())
                {
                    #region Netelem
                    iCNO = 51;
                   
                        oNewFeature.Components.GetComponent(iCNO).Recordset.MoveLast();
                        oNewFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_FNO", iFNO);
                        oNewFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_CID", 1);
                        oNewFeature.Components.GetComponent(iCNO).Recordset.Update("YEAR_PLACED", txtYearIns.Text);
                        oNewFeature.Components.GetComponent(iCNO).Recordset.Update("BILLING_RATE", cbBillingRate.SelectedValue);
                   
                    
                    #endregion
                }
                GTDuctPathEdit.m_oIGTTransactionManager.Commit();
             
                GTDuctPathEdit.m_oIGTTransactionManager.RefreshDatabaseChanges();
                if(GTDuctPathEdit.DuctPathOrigin.DuctWay != int.Parse(cbNumDuctWays.SelectedValue.ToString()))
                {
                    for (int SectNum = 0; SectNum < GTDuctPathEdit.DuctPathOrigin.Sections.Count; SectNum++)
                    {
                        GTDuctPathEdit.DuctPathOrigin.Sections[SectNum].SectionLength = SectionsChangeNumberDuctWayPPF[SectNum].SectionLength;
                        GTDuctPathEdit.DuctPathOrigin.Sections[SectNum].SectType = SectionsChangeNumberDuctWayPPF[SectNum].SectType;
                        GTDuctPathEdit.DuctPathOrigin.Sections[SectNum].SectPlc = SectionsChangeNumberDuctWayPPF[SectNum].SectPlc;
                        GTDuctPathEdit.DuctPathOrigin.Sections[SectNum].Encasement = SectionsChangeNumberDuctWayPPF[SectNum].Encasement;
                        GTDuctPathEdit.DuctPathOrigin.Sections[SectNum].SectDiam = SectionsChangeNumberDuctWayPPF[SectNum].SectDiam;
                        GTDuctPathEdit.DuctPathOrigin.Sections[SectNum].SectBackFill = SectionsChangeNumberDuctWayPPF[SectNum].SectBackFill;
                        GTDuctPathEdit.DuctPathOrigin.Sections[SectNum].SectOwner = SectionsChangeNumberDuctWayPPF[SectNum].SectOwner;
                        GTDuctPathEdit.DuctPathOrigin.Sections[SectNum].SectBillingRate = SectionsChangeNumberDuctWayPPF[SectNum].SectBillingRate;
                        GTDuctPathEdit.DuctPathOrigin.Sections[SectNum].NumDuctWaysSect = SectionsChangeNumberDuctWayPPF[SectNum].NumDuctWaysSect;
                      GTDuctPathEdit.DuctPathOrigin.Sections[SectNum].PUSect = SectionsChangeNumberDuctWayPPF[SectNum].PUSect;
               
                    }
                }
                GTDuctPathEdit.DuctPathOrigin.InstallYear = txtYearIns.Text;
                GTDuctPathEdit.DuctPathOrigin.BillingRate = cbBillingRate.SelectedValue.ToString();
                GTDuctPathEdit.DuctPathOrigin.ConstructBy = cbConstructed.SelectedValue.ToString();
                GTDuctPathEdit.DuctPathOrigin.DuctWay = int.Parse(cbNumDuctWays.SelectedValue.ToString());
                if (txtFlag.Text != "")
                GTDuctPathEdit.DuctPathOrigin.DBFlag = txtFlag.Text;
                else  GTDuctPathEdit.DuctPathOrigin.DBFlag = "";



            }
            catch (Exception ex)
            {
                GTDuctPathEdit.m_oIGTTransactionManager.Rollback();
                MessageBox.Show(ex.Message, "Duct Path Edit", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            return true;
        }
        #endregion




        #region Button Next click
        private void btnNext_Click(object sender, EventArgs e)
        {
            FillingSectList();
            statusPage = 1;
            tabDuctEdit.SelectedTab = tabPageListSect;
        }
        #endregion
        #endregion

        #region Page 2

        #region filling list
        private void FillingSectList()
        {
            lvDPSections.Clear();
            lvDPSections.Columns.Add("SectionNo", 100, HorizontalAlignment.Left);
            lvDPSections.Columns.Add("Length", 98, HorizontalAlignment.Left);
            lvDPSections.View = View.Details;
            lvDPSections.FullRowSelect = true;
            lvDPSections.MultiSelect = false;
            if (GTDuctPathEdit.DuctPathOrigin.Sections.Count > 0)
            {
                for (int i = 0; i < GTDuctPathEdit.DuctPathOrigin.Sections.Count; i++)
                {

                    // Define the list items
                    ListViewItem lvi = new ListViewItem(GTDuctPathEdit.DuctPathOrigin.Sections[i].CID.ToString());
                    lvi.SubItems.Add(GTDuctPathEdit.DuctPathOrigin.Sections[i].SectionLength + " m");
                    // Add the list items to the ListView
                    lvDPSections.Items.Add(lvi);
                }
                lvDPSections.Sort();
                lvDPSections.TopItem.Selected = true;
            //    private void BubbleSort(long[] inputArray)
//{
//    for (int iterator = 0; iterator < inputArray.Length; iterator++)
//    {
//        for (int index = 0; index < inputArray.Length - 1; index++)
//        {
//            if (inputArray[index] > inputArray[index + 1])
//            {
//                Swap(ref inputArray[index], ref inputArray[index+1]);
//            }
//        }
//    }
//}
            }
        }
        #endregion
        
        #region Button Edit click
        private void btnEditSect_Click(object sender, EventArgs e)
        {
            try
            {
                if (lvDPSections.SelectedItems.Count > 0)
                {
                    FillingPage3ComboBoxes();
                    FillingPage3Sect(short.Parse(lvDPSections.SelectedItems[0].Text));
                   
                    statusPage = 2;
                    tabDuctEdit.SelectedTab = tabPageSect;
                }
                else
                {
                    MessageBox.Show("No section selected!", "Duct Path Edit", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
            }
        }
        #endregion 

        #region Button Delete click
        private void btnDelSect_Click(object sender, EventArgs e)
        {
            if (GTDuctPathEdit.DuctPathOrigin.Sections.Count == 1)
            {
                MessageBox.Show("Duct Path must have at least one section! Deleting is not allowed.", "Duct Path Edit", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                if (lvDPSections.SelectedItems.Count > 0)
                {
                    gbButtonsFotSEcts.Enabled = false;
                    DeleteDuctPathSection(GTDuctPathEdit.DuctPathOrigin.FID,short.Parse(lvDPSections.SelectedItems[0].Text));
                    FillingSectList();
                    gbButtonsFotSEcts.Enabled = true;
                }
                else
                {
                    MessageBox.Show("No section selected!", "Duct Path Edit", MessageBoxButtons.OK, MessageBoxIcon.Information);
                } 
            }
            // this.Close();
        }
        #region Change checkbox for length Delete section
       private void cbRightDel_CheckedChanged(object sender, EventArgs e)
        {
            if (cbRightDel.Checked)
            {
                cbBothDel.Checked = false;
                cbLeftDel.Checked = false;
            }
            
        }

        private void cbLeftDel_CheckedChanged(object sender, EventArgs e)
        {
            if (cbLeftDel.Checked)
            {
                cbBothDel.Checked = false;
                cbRightDel.Checked = false;
            }
        }

        private void cbBothDel_CheckedChanged(object sender, EventArgs e)
        {
            if (cbBothDel.Checked)
            {
                cbLeftDel.Checked = false;
                cbRightDel.Checked = false;
            }
        }

        #endregion

        #region delete section function
        private bool DeleteDuctPathSection(int iFID, short CID_Del)
        {
            short iFNO = 2200;
            short iCNO;
            int SectNum = 0;
            for (int i = 0; i < GTDuctPathEdit.DuctPathOrigin.Sections.Count; i++)
            {
                if (GTDuctPathEdit.DuctPathOrigin.Sections[i].CID == CID_Del)
                {
                    SectNum = i;
                    break;
                }
            }
            progressBar1.Value = 10;
            try
            {
                GTDuctPathEdit.m_oIGTTransactionManager.Begin("DuctPathSectionDelete");
                progressBar1.Value = 20;
                IGTKeyObject oNewFeature = GTDuctPathEdit.m_IGTDataContext.OpenFeature(iFNO, iFID);
                progressBar1.Value = 30;
        
                #region update length for neighbor section
                iCNO = 2202;
                short CID_R = 0;
                short CID_L = 0;
                if (cbBothDel.Checked || cbRightDel.Checked)
                {
                    CID_R = ++CID_Del;
                    --CID_Del;
                }
                if (cbBothDel.Checked || cbLeftDel.Checked)
                {
                    CID_L = --CID_Del;
                    ++CID_Del;
                }

                int SectNumR = -1;
                int SectNumL = -1;
                for (int i = 0; i < GTDuctPathEdit.DuctPathOrigin.Sections.Count; i++)
                {
                    if (GTDuctPathEdit.DuctPathOrigin.Sections[i].CID == CID_R)
                    {
                        SectNumR = i;
                    }
                    if (GTDuctPathEdit.DuctPathOrigin.Sections[i].CID == CID_L)
                    {
                        SectNumL = i;
                    }
                }

                int length_diff_R = int.Parse(GTDuctPathEdit.DuctPathOrigin.Sections[SectNum].SectionLength);
                int length_diff_L = length_diff_R;
                if (length_diff_R == 1)
                    length_diff_L = 0;
                else
                    if (cbBothDel.Checked)
                    {
                        double len_dif_2 = (double)length_diff_R / 2;
                        int len_dif_3 = length_diff_R / 2;

                        len_dif_2 = len_dif_2 - len_dif_3;
                        if (len_dif_2 > 0)
                        {
                            length_diff_R /= 2;
                            length_diff_L = length_diff_R - 1;
                        }
                        else
                        {
                            length_diff_R /= 2;
                            length_diff_L = length_diff_R;
                        }
                    }
                int LenR = 0;
                if (SectNumR >= 0)
                    LenR = int.Parse(GTDuctPathEdit.DuctPathOrigin.Sections[SectNumR].SectionLength) + length_diff_R;
                int LenL = 0;
                if (SectNumL >= 0)
                    LenL = int.Parse(GTDuctPathEdit.DuctPathOrigin.Sections[SectNumL].SectionLength) + length_diff_R;

                if (!oNewFeature.Components.GetComponent(iCNO).Recordset.EOF)
                {
                    oNewFeature.Components.GetComponent(iCNO).Recordset.MoveFirst();
                    for (int k = 0; k < oNewFeature.Components.GetComponent(iCNO).Recordset.RecordCount; k++)
                    {
                        for (int k1 = 0; k1 < oNewFeature.Components.GetComponent(iCNO).Recordset.Fields.Count; k1++)
                        {
                            if (oNewFeature.Components.GetComponent(iCNO).Recordset.Fields[k1].Name == "G3E_CID")
                            {
                                if (oNewFeature.Components.GetComponent(iCNO).Recordset.Fields[k1].Value.ToString() == CID_R.ToString())
                                    oNewFeature.Components.GetComponent(iCNO).Recordset.Update("DT_S_LENGTH", LenR);
                                if (oNewFeature.Components.GetComponent(iCNO).Recordset.Fields[k1].Value.ToString() == CID_L.ToString())
                                    oNewFeature.Components.GetComponent(iCNO).Recordset.Update("DT_S_LENGTH", LenL);

                            }


                        }
                        oNewFeature.Components.GetComponent(iCNO).Recordset.MoveNext();
                    }
                }
                #endregion
                #region delete sect
                int roweff = 0;
                //attr
                string Sql = "delete from GC_CONDST where g3e_fno=2200 and g3e_fid=" + iFID.ToString()+ " and g3e_cid=" +CID_Del.ToString();
                GTDuctPathEdit.m_IGTDataContext.Execute(Sql, out roweff, -1);
                //label
                Sql = "update GC_COND_T set ST_CHECK=5 where g3e_fno=2200 and g3e_fid=" + iFID.ToString() + " and g3e_cid=" + CID_Del.ToString();
                GTDuctPathEdit.m_IGTDataContext.Execute(Sql, out roweff, -1);
                Sql = "delete from GC_COND_T where g3e_fno=2200 and g3e_fid=" + iFID.ToString() + " and g3e_cid=" + CID_Del.ToString();
                GTDuctPathEdit.m_IGTDataContext.Execute(Sql, out roweff, -1);
                short CIDDel2 = --CID_Del;
                ++CID_Del;
                Sql = "update GC_CONDSLASH_S set ST_CHECK=5 where g3e_fno=2200 and g3e_fid=" + iFID.ToString() +
                      " and g3e_cid in (" + CID_Del.ToString() + "," + CIDDel2.ToString() + ")";
                GTDuctPathEdit.m_IGTDataContext.Execute(Sql, out roweff, -1);
                Sql = "delete from GC_CONDSLASH_S where g3e_fno=2200 and g3e_fid=" + iFID.ToString() +
                      " and g3e_cid in (" + CID_Del.ToString() + "," + CIDDel2.ToString()+")";
                GTDuctPathEdit.m_IGTDataContext.Execute(Sql, out roweff, -1);

                #endregion
                #region update sect
                Sql = "update GC_COND_T set g3e_cid=(g3e_cid-1) where g3e_fno=2200 and g3e_fid=" + iFID.ToString() + " and g3e_cid>" + CID_Del.ToString();
                GTDuctPathEdit.m_IGTDataContext.Execute(Sql, out roweff, -1);

                Sql = "update GC_CONDST set g3e_cid=(g3e_cid-1) where g3e_fno=2200 and g3e_fid=" + iFID.ToString() + " and g3e_cid>" + CID_Del.ToString();
                GTDuctPathEdit.m_IGTDataContext.Execute(Sql, out roweff, -1);

                #endregion

                #region add new slash
                
                int lengthfornewSlash=0;
                if (SectNumR > -1)
                {
                    for (int i = 0; i < GTDuctPathEdit.DuctPathOrigin.Sections.Count; i++)
                    {
                        if (GTDuctPathEdit.DuctPathOrigin.Sections[i].CID <= CID_R)
                        {
                            lengthfornewSlash += int.Parse(GTDuctPathEdit.DuctPathOrigin.Sections[i].SectionLength);
                        }                       
                       
                    }
                    lengthfornewSlash -= LenR;
                    
                    
                }
                else
                {
                    for (int i = 0; i < GTDuctPathEdit.DuctPathOrigin.Sections.Count; i++)
                    {
                        if (GTDuctPathEdit.DuctPathOrigin.Sections[i].CID < CID_L)
                        {
                            lengthfornewSlash += int.Parse(GTDuctPathEdit.DuctPathOrigin.Sections[i].SectionLength);
                        }
                        
                    }
                    lengthfornewSlash += LenL;
                    
                }
                GTDuctPathEdit.SectSlash newslash = null;
                if (lengthfornewSlash < GTDuctPathEdit.DuctPathOrigin.Length && lengthfornewSlash !=0 )
                {
                    IGTOrientedPointGeometry temp = GTClassFactory.Create<IGTOrientedPointGeometry>();
                    IGTPoint tempp = PointForSlashBasedOnEnteredLength(lengthfornewSlash);
                    temp.Origin = tempp;
                    temp.Orientation = OrientationForPointOnConduit(tempp.X, tempp.Y, lengthfornewSlash);
                    iCNO = 2220;

                    oNewFeature.Components.GetComponent(iCNO).Recordset.AddNew("G3E_FID", iFID);
                    oNewFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_FNO", iFNO);
                    oNewFeature.Components.GetComponent(iCNO).Recordset.Update("ST_CHECK", 5);
                    oNewFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_CID", CIDDel2);
                    oNewFeature.Components.GetComponent(iCNO).Geometry = temp;
                    newslash = new GTDuctPathEdit.SectSlash();
                    newslash.CID = CIDDel2;
                    newslash.length = lengthfornewSlash;
                    newslash.Slash = temp;
                }

                Sql = "update GC_CONDSLASH_S set g3e_cid=(g3e_cid-1) where g3e_fno=2200 and g3e_fid=" + iFID.ToString() + " and g3e_cid>" + CIDDel2.ToString();
            
  
                GTDuctPathEdit.m_IGTDataContext.Execute(Sql, out roweff, -1);
                #endregion

                if (!oNewFeature.Components.GetComponent(51).Recordset.EOF)
                {
                    oNewFeature.Components.GetComponent(51).Recordset.MoveLast();
                    oNewFeature.Components.GetComponent(51).Recordset.Update("FEATURE_STATE", "");
                   // oNewFeature.Components.GetComponent(51).Recordset.Update("FEATURE_STATE", GTDuctPathEdit.DuctPathOrigin.Feature_state);
                }
                GTDuctPathEdit.m_oIGTTransactionManager.Commit();
                progressBar1.Value = 70;
                GTDuctPathEdit.m_oIGTTransactionManager.RefreshDatabaseChanges();
                progressBar1.Value = 80;

                GTDuctPathEdit.m_gtapp.RefreshWindows();

                GTDuctPathEdit.m_oIGTTransactionManager.Begin("DuctPathSectionDelete2");
                oNewFeature = GTDuctPathEdit.m_IGTDataContext.OpenFeature(iFNO, iFID);

                if (!oNewFeature.Components.GetComponent(51).Recordset.EOF)
                {
                    oNewFeature.Components.GetComponent(51).Recordset.MoveLast();
                    oNewFeature.Components.GetComponent(51).Recordset.Update("FEATURE_STATE", GTDuctPathEdit.DuctPathOrigin.Feature_state);
                }
                 GTDuctPathEdit.m_oIGTTransactionManager.Commit();
                progressBar1.Value = 85;
                GTDuctPathEdit.m_oIGTTransactionManager.RefreshDatabaseChanges();
                progressBar1.Value = 90;
                GTDuctPathEdit.m_gtapp.RefreshWindows();
                GTDuctPathEdit.DuctPathOrigin.Sections[SectNum].SectionLength = txtSectionLength.Text;
                if (SectNumL >= 0)
                    GTDuctPathEdit.DuctPathOrigin.Sections[SectNumL].SectionLength = LenL.ToString();
                if (SectNumR >= 0)
                    GTDuctPathEdit.DuctPathOrigin.Sections[SectNumR].SectionLength = LenR.ToString();
                //GTDuctPathEdit.DuctPathOrigin.Sections[SectNum].PU
                GTDuctPathEdit.DuctPathOrigin.Sections.RemoveAt(SectNum);
               // GTDuctPathEdit.DuctPathOrigin.SectLabels.RemoveAt(SectNum);
                for (int i = 0; i < GTDuctPathEdit.DuctPathOrigin.Sections.Count; i++)
                {
                    if (GTDuctPathEdit.DuctPathOrigin.Sections[i].CID > CID_Del)
                    {
                        GTDuctPathEdit.DuctPathOrigin.Sections[i].CID--;
                    }
                }

                for (int i = 0; i < GTDuctPathEdit.DuctPathOrigin.SectLabels.Count; i++)
                {
                    if (GTDuctPathEdit.DuctPathOrigin.SectLabels[i].CID == CID_Del)
                    {
                        GTDuctPathEdit.DuctPathOrigin.SectLabels.RemoveAt(i);
                        break;
                    }
                }

                for (int i = 0; i < GTDuctPathEdit.DuctPathOrigin.SectLabels.Count; i++)
                {
                    if (GTDuctPathEdit.DuctPathOrigin.SectLabels[i].CID > CID_Del)
                    {
                        GTDuctPathEdit.DuctPathOrigin.SectLabels[i].CID--;
                    }
                }

                for (int i = 0; i < GTDuctPathEdit.DuctPathOrigin.SectSlashes.Count; i++)
                {
                    if (GTDuctPathEdit.DuctPathOrigin.SectSlashes[i].CID == CID_Del)
                    {
                        GTDuctPathEdit.DuctPathOrigin.SectSlashes.RemoveAt(i);
                        i--;
                    } else
                    if (GTDuctPathEdit.DuctPathOrigin.SectSlashes[i].CID == CIDDel2)
                    {
                        GTDuctPathEdit.DuctPathOrigin.SectSlashes.RemoveAt(i);
                        i--;
                    }                   
                    
                }
                for (int i = 0; i < GTDuctPathEdit.DuctPathOrigin.SectSlashes.Count; i++)
                {
                    if (GTDuctPathEdit.DuctPathOrigin.SectSlashes[i].CID > CIDDel2)
                    {
                        GTDuctPathEdit.DuctPathOrigin.SectSlashes[i].CID--;
                    }
                }
                if (newslash != null)
                {
                    GTDuctPathEdit.DuctPathOrigin.SectSlashes.Add(newslash);
                }
                progressBar1.Value = 100;
               
            }
            catch (Exception ex)
            {
                GTDuctPathEdit.m_oIGTTransactionManager.Rollback();
                MessageBox.Show(ex.Message, "Duct Path Edit", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            return true;
        }
        #endregion
        #endregion

        #region Button Add New click
        private void btnAddNewSect_Click(object sender, EventArgs e)
        {
            if (GTDuctPathEdit.DuctPathOrigin.Sections.Count == 1)
                GTDuctPathEdit.step = 102;
            else  GTDuctPathEdit.step = 100;
            this.Hide();

        }
        #endregion

        #region Button close1 click
        private void btnClose1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        #endregion 
       
        #region Button Back click
        private void btnBack_Click(object sender, EventArgs e)
        {
            statusPage = 0;
            tabDuctEdit.SelectedTab = tabPageAttr;
        }
        #endregion 

        #region index change of selected item
        private void lvDPSections_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (GTDuctPathEdit.DuctPathOrigin.Sections.Count == 1)
            {
                gbDelete.Enabled = false;
            }
            else
            {
                gbDelete.Enabled = true;
                if (lvDPSections.SelectedItems.Count > 0)
                {
                    if (int.Parse(lvDPSections.SelectedItems[0].Text) == 1)
                    {
                        cbRightDel.Checked = true;
                        cbBothDel.Enabled = false;
                        cbLeftDel.Enabled = false;
                        cbRightDel.Enabled = false;
                    }
                    else
                        if (int.Parse(lvDPSections.SelectedItems[0].Text) == GTDuctPathEdit.DuctPathOrigin.Sections.Count)
                        {
                            cbLeftDel.Checked = true;
                            cbBothDel.Enabled = false;
                            cbLeftDel.Enabled = false;
                            cbRightDel.Enabled = false;
                        }
                        else
                        {
                            cbBothDel.Checked = true;
                            cbBothDel.Enabled = true;
                            cbLeftDel.Enabled = true;
                            cbRightDel.Enabled = true;
                        }
                }
            }
        }
        #endregion
        #endregion

        #region Page 3 Section

        #region Button close3 click
        private void btnClose3_Click_1(object sender, EventArgs e)
        {
            this.Close();
        }        
        #endregion 
        
        #region Button Back click
        private void btnBackToSectList_Click(object sender, EventArgs e)
        {
            if (ChangeNumberDuctWayPPF == -1)
            {
                FillingSectList();
                statusPage = 1;
                tabDuctEdit.SelectedTab = tabPageListSect;
            }
            else
            {
                 DialogResult closetype =MessageBox.Show("Do you want to cancel changing Number of DuctWay?", "Duct Path Placement", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                 if (closetype == DialogResult.Yes)
                 {
                     ChangeNumberDuctWayPPF = -1; 
                     if (SectionsChangeNumberDuctWayPPF != null)
                         SectionsChangeNumberDuctWayPPF.Clear();
                     if (cbNumDuctWays.DataSource != null)
                     {
                         int i = 0;
                         for (; i < cbNumDuctWays.Items.Count; i++)
                         {
                             cbNumDuctWays.SelectedItem = cbNumDuctWays.Items[i];
                             if (cbNumDuctWays.SelectedValue.ToString() == GTDuctPathEdit.DuctPathOrigin.DuctWay.ToString())
                                 break;
                         }
                         if (i == cbNumDuctWays.Items.Count)
                             cbNumDuctWays.SelectedIndex = 0;
                     }

                     statusPage = 0;
                     tabDuctEdit.SelectedTab = tabPageAttr;
                 }
            }
        }
        #endregion 

        #region Change checkbox for length modi
        private void cbBoth_CheckedChanged(object sender, EventArgs e)
        {
            if (cbBoth.Checked)
            {
                cbLeft.Checked = false;
                cbRight.Checked = false;
            }

        }

        private void cbLeft_CheckedChanged(object sender, EventArgs e)
        {
            if (cbLeft.Checked)
            {
                cbBoth.Checked = false;
                cbRight.Checked = false;
            }
        }

        private void cbRight_CheckedChanged(object sender, EventArgs e)
        {
            if (cbRight.Checked)
            {
                cbBoth.Checked = false;
                cbLeft.Checked = false;
            }
        }

        #endregion

        #region filling text and combo boxes for section attr
        private void FillingPage3Sect(short SectCID)
        {
            if (GTDuctPathEdit.DuctPathOrigin.Sections.Count == 1)
            {
                gbSectLength.Enabled = false;
            }
            else gbSectLength.Enabled = true;

            cbBoth.Checked = true;
            cbBoth.Enabled = true;
            cbLeft.Enabled = true;
            cbRight.Enabled = true;
            if (SectCID==1)
            {
                cbRight.Checked = true;
                cbBoth.Enabled = false;
                cbLeft.Enabled = false;
                cbRight.Enabled = false;
            }
            if (SectCID == GTDuctPathEdit.DuctPathOrigin.Sections.Count)
            {
                cbLeft.Checked = true;
                cbBoth.Enabled = false;
                cbLeft.Enabled = false;
                cbRight.Enabled = false;
            }
            int SectNum = 0;
            for (int i = 0; i < GTDuctPathEdit.DuctPathOrigin.Sections.Count; i++)
            {
                if (GTDuctPathEdit.DuctPathOrigin.Sections[i].CID == SectCID)
                {
                    SectNum = i;
                    break;
                }
            }
            mcbMinMaterial.Text = GTDuctPathEdit.DuctPathOrigin.Sections[SectNum].PUSect;
            txtSectionLength.Text = GTDuctPathEdit.DuctPathOrigin.Sections[SectNum].SectionLength;
            gbSectValues.Enabled = true;
            txtYearExpanded.Text = GTDuctPathEdit.DuctPathOrigin.Sections[SectNum].YearExpanded;
            txtYearExtended.Text = GTDuctPathEdit.DuctPathOrigin.Sections[SectNum].YearExtended;
            txtNumDuctWaysSect.Text = cbNumDuctWays.SelectedValue.ToString();
            
        //    mcbMinMaterial.Text = "PVC_6_GRASSVERGE_N_Y_106";
            if (cbSectDiam.DataSource != null)
            {
                // cbConstructed.SelectedText = "TM";
                int i = 0;
                for (; i < cbSectDiam.Items.Count; i++)
                {
                    cbSectDiam.SelectedItem = cbSectDiam.Items[i];
                    if (cbSectDiam.SelectedValue.ToString() == GTDuctPathEdit.DuctPathOrigin.Sections[SectNum].SectDiam)
                        break;
                }
                if (i == cbSectDiam.Items.Count)
                    cbSectDiam.SelectedIndex = 0;
            }
            if (cbSectOwner.DataSource != null)
            {
                // cbConstructed.SelectedText = "TM";
                int i = 0;
                for (; i < cbSectOwner.Items.Count; i++)
                {
                    cbSectOwner.SelectedItem = cbSectOwner.Items[i];
                    if (cbSectOwner.SelectedValue.ToString().ToUpper().Contains(GTDuctPathEdit.DuctPathOrigin.Sections[SectNum].SectOwner))
                        break;
                }
                if (i == cbSectOwner.Items.Count)
                    cbSectOwner.SelectedIndex = 0;
            }

            if (cbSectType.DataSource != null)
            {
                // cbConstructed.SelectedText = "TM";
                int i = 0;
                for (; i < cbSectType.Items.Count; i++)
                {
                    cbSectType.SelectedItem = cbSectType.Items[i];
                    if (cbSectType.SelectedValue.ToString().ToUpper().Contains(GTDuctPathEdit.DuctPathOrigin.Sections[SectNum].SectType))
                        break;
                }
                if (i == cbSectType.Items.Count)
                    cbSectType.SelectedIndex = 0;
            }

            if (cbSectPlc.DataSource != null)
            {
                // cbConstructed.SelectedText = "TM";
                int i = 0;
                for (; i < cbSectPlc.Items.Count; i++)
                {
                    cbSectPlc.SelectedItem = cbSectPlc.Items[i];
                    if (cbSectPlc.SelectedValue.ToString().ToUpper().Contains(GTDuctPathEdit.DuctPathOrigin.Sections[SectNum].SectPlc))
                        break;
                }
                if (i == cbSectPlc.Items.Count)
                    cbSectPlc.SelectedIndex = 0;
            }

            if (cbSectBillingRate.DataSource != null)
            {
                // cbConstructed.SelectedText = "TM";
                int i = 0;
                for (; i < cbSectBillingRate.Items.Count; i++)
                {
                    cbSectBillingRate.SelectedItem = cbSectBillingRate.Items[i];
                    if (cbSectBillingRate.SelectedValue.ToString().ToUpper().Contains(GTDuctPathEdit.DuctPathOrigin.Sections[SectNum].SectBillingRate))
                        break;
                }
                if (i == cbSectBillingRate.Items.Count)
                    cbSectBillingRate.SelectedIndex = 0;
            }

            if (cbEncasement.DataSource != null)
            {
                // cbConstructed.SelectedText = "TM";
                int i = 0;
                for (; i < cbEncasement.Items.Count; i++)
                {
                    cbEncasement.SelectedItem = cbEncasement.Items[i];
                    if (cbEncasement.SelectedValue.ToString().ToUpper().Contains(GTDuctPathEdit.DuctPathOrigin.Sections[SectNum].Encasement))
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
                    if (cbSectBackFill.SelectedValue.ToString().ToUpper().Contains(GTDuctPathEdit.DuctPathOrigin.Sections[SectNum].SectBackFill))
                        break;
                }
                if (i == cbSectBackFill.Items.Count)
                    cbSectBackFill.SelectedIndex = 0;
            }


        }

        private void FillingPage3ComboBoxes()
        {
            ADODB.Recordset rsPP = new ADODB.Recordset();
            string sSql = "";

            if (cbSectOwner.DataSource == null)
            {
                cbSectOwner.Items.Clear();
                sSql = "SELECT PL_VALUE FROM REF_COM_CONSTRUCTION";
                rsPP = GTDuctPathEdit.m_IGTDataContext.OpenRecordset(sSql, ADODB.CursorTypeEnum.adOpenStatic, ADODB.LockTypeEnum.adLockReadOnly, 1);

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
            rsPP = GTDuctPathEdit.m_IGTDataContext.OpenRecordset(sSql, ADODB.CursorTypeEnum.adOpenStatic, ADODB.LockTypeEnum.adLockReadOnly, 1);

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
                rsPP = GTDuctPathEdit.m_IGTDataContext.OpenRecordset(sSql, ADODB.CursorTypeEnum.adOpenStatic, ADODB.LockTypeEnum.adLockReadOnly, 1);

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
                rsPP = GTDuctPathEdit.m_IGTDataContext.OpenRecordset(sSql, ADODB.CursorTypeEnum.adOpenStatic, ADODB.LockTypeEnum.adLockReadOnly, 1);

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
                rsPP = GTDuctPathEdit.m_IGTDataContext.OpenRecordset(sSql, ADODB.CursorTypeEnum.adOpenStatic, ADODB.LockTypeEnum.adLockReadOnly, 1);

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
                rsPP = GTDuctPathEdit.m_IGTDataContext.OpenRecordset(sSql, ADODB.CursorTypeEnum.adOpenStatic, ADODB.LockTypeEnum.adLockReadOnly, 1);

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
                rsPP = GTDuctPathEdit.m_IGTDataContext.OpenRecordset(sSql, ADODB.CursorTypeEnum.adOpenStatic, ADODB.LockTypeEnum.adLockReadOnly, 1);

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
                rsPP = GTDuctPathEdit.m_IGTDataContext.OpenRecordset(sSql, ADODB.CursorTypeEnum.adOpenStatic, ADODB.LockTypeEnum.adLockReadOnly, 1);

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

        #region button Update for attr for Section clicl
        private void btnConfirmSectAttr_Click_1(object sender, EventArgs e)
        {
            if (ValidateDuctPathSect())
            {
                GTDuctPathEdit.m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Wait, updating in process...");
                btnBackToSectList.Enabled = false;
                btnClose3.Enabled = false;
                btnConfirmSectAttr.Enabled = false;
                progressBar1.Value = 0;
                progressBar1.Visible = true;
                gbSectValues.Enabled = false;

                if (ChangeNumberDuctWayPPF == -1)
                {
                  if (!UpdateDuctPathSectionAttr(GTDuctPathEdit.DuctPathOrigin.FID, short.Parse(lvDPSections.SelectedItems[0].Text)))
                        MessageBox.Show("Error during update Duct Path Section attributes", "Duct Path Edit", MessageBoxButtons.OK, MessageBoxIcon.Error);
                  
                }
                else
                {
                    UpdateDuctPathSectionAttrForChangeNumDuctWay();
                }

                #region add new
                //btnConfirmSectAttr.Enabled = false;
                //gbSectValues.Enabled = false;
                //if(Sections==null)
                //    Sections = new List<DuctPathSect>();
                //if(SectSlashes==null)
                //    SectSlashes = new List<SectSlash>();
                //DuctPathSect s = FillSection();
                //Sections.Add(s);
                
              
                //if(SectSlashes.Count>0 && LastSection==0)
                //{
                //    int graphlen = 0;
                //    int newgraphlen = 0;
                //    if (SectSlashes.Count == 1)
                //    {
                //        graphlen = SectSlashes[SectSlashes.Count - 1].length;
                //        newgraphlen = Sections[Sections.Count - 1].SectGraphicLength;
                    
                //    }
                //    else
                //    {
                //        graphlen = SectSlashes[SectSlashes.Count - 1].length;
                //        newgraphlen = SectSlashes[SectSlashes.Count - 2].length + Sections[Sections.Count - 1].SectGraphicLength;
                //    }

                //    if (newgraphlen != graphlen)
                //    {
                //      PlaceSlashSection(newgraphlen);
                //    }
                //    PlaceLabelSect();
                //    return;
                //}

                //if (SectSlashes.Count >= 0 && LastSection!=0)
                //{
                    
                //    int lengthRest = TotalEnteredLength;
                   
                //        if (Sections.Count > 0)
                //        {
                //            for (int i = 0; i < Sections.Count; i++)
                //            {
                //                lengthRest -= int.Parse(Sections[i].SectionLength);
                //            }
                //        }
                //        if (lengthRest != 0)
                //        {
                //            int LenLastSection = int.Parse(Sections[Sections.Count - 1].SectionLength) + Math.Abs(lengthRest);
                //            DialogResult closetype = MessageBox.Show("Summary length of Section's lengths should be equal to \n Total Duct Path Length = " + txtTotalLength.Text +
                //                " !\n Do you want to changed length of last section to " + LenLastSection.ToString() + 
                //                "\n to complete summary length of sections?\n If 'NO', place one more section!", "Duct Path Edit", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                //            if (closetype == DialogResult.Yes)
                //            {
                //                txtSectionLength.Text = LenLastSection.ToString();
                //                int c = Sections.Count;
                //                DuctPathSect Stem = new DuctPathSect();
                //                Stem = Sections[c - 1];
                //                Stem.SectionLength = LenLastSection.ToString();
                //                if (TotalGraphicLength != TotalEnteredLength)
                //                {
                //                    if (TotalEnteredLength == 0) TotalEnteredLength = 1;
                //                    Stem.SectGraphicLength = TotalGraphicLength * LenLastSection / TotalEnteredLength;
                //                }
                //                else Stem.SectGraphicLength = LenLastSection;
                //                Sections.RemoveAt(c - 1);
                //                Sections.Add(Stem);

                //                if (Sections.Count == SectSlashes.Count && Sections.Count != 0)
                //                {
                //                    SectSlashes.RemoveAt(SectSlashes.Count - 1);
                //                    GTDuctPathEdit.mobjEditService.RemoveGeometry(GTDuctPathEdit.mobjEditService.GeometryCount - 2);
                //                }

                //                PlaceLabelSect();
                //                btnFinished.Enabled = true;
                //                return;
                //            }
                //            else
                //            {
                //                btnFinished.Enabled = false;
                //                LastSection = 2;
                //                if(SectSlashes.Count>0)
                //                    PlaceSlashSection(SectSlashes[SectSlashes.Count - 1].length + Sections[Sections.Count - 1].SectGraphicLength);
                //                if (SectSlashes.Count ==0)
                //                    PlaceSlashSection(Sections[Sections.Count - 1].SectGraphicLength);
                //                PlaceLabelSect();
                //                return;
                //            }
                //        }
                //        else LastSection = 1;
                    

                //    PlaceLabelSect();
                //    btnFinished.Enabled = true;
                //    return;
                //}
#endregion
                btnBackToSectList.Enabled = true;
                btnClose3.Enabled = true;
                btnConfirmSectAttr.Enabled = true;
                progressBar1.Visible = false;

                gbSectValues.Enabled = true;
                GTDuctPathEdit.m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "");
            }
        }
        #region UpdateDuctPathSectionAttr

        private void UpdateDuctPathSectionAttrForChangeNumDuctWay()
        {
            progressBar1.Value = 30;
            int SectNum = -1;
            short CID = 0;
            for (int i = 0; i < SectionsChangeNumberDuctWayPPF.Count; i++)
            {
                if (SectionsChangeNumberDuctWayPPF[i].PUSect == "")
                {
                    SectionsChangeNumberDuctWayPPF[i].SectionLength= txtSectionLength.Text;
                    SectionsChangeNumberDuctWayPPF[i].SectType= cbSectType.SelectedValue.ToString();
                    SectionsChangeNumberDuctWayPPF[i].SectPlc= cbSectPlc.SelectedValue.ToString();
                    SectionsChangeNumberDuctWayPPF[i].Encasement = cbEncasement.SelectedValue.ToString() ;
                    SectionsChangeNumberDuctWayPPF[i].SectDiam =cbSectDiam.SelectedValue.ToString();
                    SectionsChangeNumberDuctWayPPF[i].SectBackFill= cbSectBackFill.SelectedValue.ToString();
                    SectionsChangeNumberDuctWayPPF[i].SectOwner= cbSectOwner.SelectedValue.ToString();
                    SectionsChangeNumberDuctWayPPF[i].SectBillingRate= cbSectBillingRate.SelectedValue.ToString();
                    SectionsChangeNumberDuctWayPPF[i].PUSect = mcbMinMaterial.Text;
                    SectNum=i;
                    CID = SectionsChangeNumberDuctWayPPF[i].CID;
                    break;
                }
            }
            progressBar1.Value = 60;
              #region update length for neighbor section c num
                short CID_R = 0;
                short CID_L = 0;
                if (cbBoth.Checked || cbRight.Checked)
                {
                    CID_R = ++CID;
                    --CID;
                }
                if (cbBoth.Checked || cbLeft.Checked)
                {
                    CID_L = --CID;
                    ++CID;
                }

                int SectNumR = -1;
                int SectNumL = -1;
                for (int i = 0; i < SectionsChangeNumberDuctWayPPF.Count; i++)
                {
                    if (SectionsChangeNumberDuctWayPPF[i].CID == CID_R)
                    {
                        SectNumR = i;
                    }
                    if (SectionsChangeNumberDuctWayPPF[i].CID == CID_L)
                    {
                        SectNumL = i;
                    }
                }

                int length_diff_R = int.Parse(SectionsChangeNumberDuctWayPPF[SectNum].SectionLength) - int.Parse(txtSectionLength.Text);
                int length_diff_L = length_diff_R;
                if (length_diff_R == 1)
                    length_diff_L = 0;
                else
                if (cbBoth.Checked)
                {
                    double len_dif_2 = (double)length_diff_R / 2;
                    int len_dif_3 = length_diff_R / 2;

                    len_dif_2 = len_dif_2 - len_dif_3;
                    if (len_dif_2 > 0)
                    {
                        length_diff_R /= 2;
                        length_diff_L = length_diff_R - 1;
                    }
                    else
                    {
                        length_diff_R /= 2;
                        length_diff_L = length_diff_R;
                    }
                }
                int LenR = 0;
                if (SectNumR >= 0)
                    LenR = int.Parse(SectionsChangeNumberDuctWayPPF[SectNumR].SectionLength) + length_diff_R;
                int LenL = 0;
                if (SectNumL >= 0)
                    LenL = int.Parse(SectionsChangeNumberDuctWayPPF[SectNumL].SectionLength) + length_diff_L;
            
                if(SectNumL>=0)
                   SectionsChangeNumberDuctWayPPF[SectNumL].SectionLength = LenL.ToString();
                if (SectNumR >= 0)
                    SectionsChangeNumberDuctWayPPF[SectNumR].SectionLength = LenR.ToString();
                
                
                #endregion
                progressBar1.Value = 90;
                ChangeNumberDuctWayPPF++;
                if (SectionsChangeNumberDuctWayPPF.Count != ChangeNumberDuctWayPPF)
                {

                    short cid_to_change = 0;
                    for (int i = 0; i < SectionsChangeNumberDuctWayPPF.Count; i++)
                    {
                        if (SectionsChangeNumberDuctWayPPF[i].PUSect == "")
                        {
                            cid_to_change = SectionsChangeNumberDuctWayPPF[i].CID;
                            break;
                        }
                    }
                    FillingPage3ComboBoxes();
                    FillingPage3Sect(cid_to_change);
                    progressBar1.Value = 100;
                }
                else
                {
                    progressBar1.Value = 10;
                    GTDuctPathEdit.m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Wait, updating in process...");
                    progressBar1.Value = 50;
                    if (!UpdateDuctPathAttributes(GTDuctPathEdit.DuctPathOrigin.FID))
                        MessageBox.Show("Error during update Duct Path attributes", "Duct Path Edit", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    else
                    {

                        progressBar1.Value = 100;
                        MessageBox.Show("Duct Path attributes are updated successfully", "Duct Path Edit", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        ChangeNumberDuctWayPPF = -1;
                        if (SectionsChangeNumberDuctWayPPF != null)
                            SectionsChangeNumberDuctWayPPF.Clear(); 
                    }
                    GTDuctPathEdit.m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "");
                    statusPage = 0;

                    tabDuctEdit.SelectedTab = tabPageAttr;
                }
        }
        #endregion

        #region UpdateDuctPathSectionAttr

        private bool UpdateDuctPathSectionAttr(int iFID, short CID)
        {
            short iFNO = 2200;
            short iCNO;
            int SectNum = 0;
            for (int i = 0; i < GTDuctPathEdit.DuctPathOrigin.Sections.Count; i++)
            {
                if (GTDuctPathEdit.DuctPathOrigin.Sections[i].CID == CID)
                {
                    SectNum = i;
                    break;
                }
            }

            progressBar1.Value = 10;
            if (      GTDuctPathEdit.DuctPathOrigin.Sections[SectNum].SectionLength == txtSectionLength.Text
                   && GTDuctPathEdit.DuctPathOrigin.Sections[SectNum].PUSect == mcbMinMaterial.Text
                    && GTDuctPathEdit.DuctPathOrigin.Sections[SectNum].SectType == cbSectType.SelectedValue.ToString()
                    && GTDuctPathEdit.DuctPathOrigin.Sections[SectNum].SectPlc == cbSectPlc.SelectedValue.ToString()
                    && GTDuctPathEdit.DuctPathOrigin.Sections[SectNum].Encasement == cbEncasement.SelectedValue.ToString()
                    && GTDuctPathEdit.DuctPathOrigin.Sections[SectNum].SectBackFill == cbSectBackFill.SelectedValue.ToString()
                    && GTDuctPathEdit.DuctPathOrigin.Sections[SectNum].SectDiam == cbSectDiam.SelectedValue.ToString()
                    && GTDuctPathEdit.DuctPathOrigin.Sections[SectNum].SectOwner == cbSectOwner.SelectedValue.ToString()
                    && GTDuctPathEdit.DuctPathOrigin.Sections[SectNum].SectBillingRate == cbSectBillingRate.SelectedValue.ToString())
            {
                progressBar1.Value = 90;
                return true;
            }
            try
            {
                GTDuctPathEdit.m_oIGTTransactionManager.Begin("DuctPathSectionAttrUpdate");
                progressBar1.Value = 20;
                IGTKeyObject oNewFeature = GTDuctPathEdit.m_IGTDataContext.OpenFeature(iFNO, iFID);
                progressBar1.Value = 30;
                if (
                    GTDuctPathEdit.DuctPathOrigin.Sections[SectNum].SectionLength != txtSectionLength.Text
                   || GTDuctPathEdit.DuctPathOrigin.Sections[SectNum].PUSect != mcbMinMaterial.Text
                    || GTDuctPathEdit.DuctPathOrigin.Sections[SectNum].SectType != cbSectType.SelectedValue.ToString()
                    || GTDuctPathEdit.DuctPathOrigin.Sections[SectNum].SectPlc != cbSectPlc.SelectedValue.ToString()
                    || GTDuctPathEdit.DuctPathOrigin.Sections[SectNum].Encasement != cbEncasement.SelectedValue.ToString()
                    || GTDuctPathEdit.DuctPathOrigin.Sections[SectNum].SectBackFill != cbSectBackFill.SelectedValue.ToString()
                    || GTDuctPathEdit.DuctPathOrigin.Sections[SectNum].SectDiam != cbSectDiam.SelectedValue.ToString()
                    || GTDuctPathEdit.DuctPathOrigin.Sections[SectNum].SectOwner != cbSectOwner.SelectedValue.ToString()
                    || GTDuctPathEdit.DuctPathOrigin.Sections[SectNum].SectBillingRate != cbSectBillingRate.SelectedValue.ToString())
                {
                  
                    #region Section Attributes
                    iCNO = 2202;

                    if (!oNewFeature.Components.GetComponent(iCNO).Recordset.EOF)
                    {
                        oNewFeature.Components.GetComponent(iCNO).Recordset.MoveFirst();
                        for (int k = 0; k < oNewFeature.Components.GetComponent(iCNO).Recordset.RecordCount; k++)
                        {
                            for (int k1 = 0; k1 < oNewFeature.Components.GetComponent(iCNO).Recordset.Fields.Count; k1++)
                            {
                                if(oNewFeature.Components.GetComponent(iCNO).Recordset.Fields[k1].Name=="G3E_CID")
                                    if (oNewFeature.Components.GetComponent(iCNO).Recordset.Fields[k1].Value.ToString() == CID.ToString())
                                    {
                                        oNewFeature.Components.GetComponent(iCNO).Recordset.Update("DT_S_LENGTH", int.Parse(txtSectionLength.Text));
                                        oNewFeature.Components.GetComponent(iCNO).Recordset.Update("DT_S_TYPE", cbSectType.SelectedValue);
                                        oNewFeature.Components.GetComponent(iCNO).Recordset.Update("DT_S_PLACMNT", cbSectPlc.SelectedValue);
                                        oNewFeature.Components.GetComponent(iCNO).Recordset.Update("DT_S_ENCASE", cbEncasement.SelectedValue);
                                        oNewFeature.Components.GetComponent(iCNO).Recordset.Update("DT_S_DIAMETER", int.Parse(cbSectDiam.SelectedValue.ToString()));
                                        oNewFeature.Components.GetComponent(iCNO).Recordset.Update("DT_S_BACKFILL", cbSectBackFill.SelectedValue);
                                        oNewFeature.Components.GetComponent(iCNO).Recordset.Update("COMMON_TRENCH", cbSectOwner.SelectedValue);
                                        oNewFeature.Components.GetComponent(iCNO).Recordset.Update("BILLING_RATE", cbSectBillingRate.SelectedValue);
                                        oNewFeature.Components.GetComponent(iCNO).Recordset.Update("MIN_MATERIAL", mcbMinMaterial.Text);
                                        k = oNewFeature.Components.GetComponent(iCNO).Recordset.RecordCount - 1;
                                        break;
                                    }

                            }
                            oNewFeature.Components.GetComponent(iCNO).Recordset.MoveNext();
                        }                       
                    }
                    progressBar1.Value = 20;
                    #endregion

                    progressBar1.Value = 60;
                }

                if (!oNewFeature.Components.GetComponent(51).Recordset.EOF)
                {
                    oNewFeature.Components.GetComponent(51).Recordset.MoveLast();
                    oNewFeature.Components.GetComponent(51).Recordset.Update("FEATURE_STATE", "");
                   // oNewFeature.Components.GetComponent(51).Recordset.Update("FEATURE_STATE", GTDuctPathEdit.DuctPathOrigin.Feature_state);
                }
                GTDuctPathEdit.m_oIGTTransactionManager.Commit();
                progressBar1.Value = 80;
                GTDuctPathEdit.m_oIGTTransactionManager.RefreshDatabaseChanges();
                progressBar1.Value = 90;

                GTDuctPathEdit.m_oIGTTransactionManager.Begin("DuctPathSectionAttrUpdate2");
                oNewFeature = GTDuctPathEdit.m_IGTDataContext.OpenFeature(iFNO, iFID);

                if (!oNewFeature.Components.GetComponent(51).Recordset.EOF)
                {
                    oNewFeature.Components.GetComponent(51).Recordset.MoveLast();
                    oNewFeature.Components.GetComponent(51).Recordset.Update("FEATURE_STATE", GTDuctPathEdit.DuctPathOrigin.Feature_state);
                }
               
                #region update length for neighbor section
                iCNO = 2202;
                short CID_R = 0;
                short CID_L = 0;
                if (cbBoth.Checked || cbRight.Checked)
                {
                    CID_R = ++CID;
                    --CID;
                }
                if (cbBoth.Checked || cbLeft.Checked)
                {
                    CID_L = --CID;
                    ++CID;
                }

                int SectNumR = -1;
                int SectNumL = -1;
                for (int i = 0; i < GTDuctPathEdit.DuctPathOrigin.Sections.Count; i++)
                {
                    if (GTDuctPathEdit.DuctPathOrigin.Sections[i].CID == CID_R)
                    {
                        SectNumR = i;
                    }
                    if (GTDuctPathEdit.DuctPathOrigin.Sections[i].CID == CID_L)
                    {
                        SectNumL = i;
                    }
                }
                
                int length_diff_R = int.Parse(GTDuctPathEdit.DuctPathOrigin.Sections[SectNum].SectionLength)-int.Parse(txtSectionLength.Text);
                int length_diff_L = length_diff_R;
                if (length_diff_R == 1)
                    length_diff_L = 0;
                else
                if (cbBoth.Checked)
                {
                    double len_dif_2 = (double)length_diff_R / 2;
                    int len_dif_3 = length_diff_R / 2;

                    len_dif_2 = len_dif_2 - len_dif_3;
                    if (len_dif_2 > 0)
                    {
                        length_diff_R /= 2;
                        length_diff_L = length_diff_R - 1;
                    }
                    else
                    {
                        length_diff_R /= 2;
                        length_diff_L = length_diff_R;
                    }
                }
                int LenR = 0;
                if (SectNumR >= 0)
                    LenR = int.Parse(GTDuctPathEdit.DuctPathOrigin.Sections[SectNumR].SectionLength) + length_diff_R;
                int LenL = 0;
                if (SectNumL >= 0)
                    LenL = int.Parse(GTDuctPathEdit.DuctPathOrigin.Sections[SectNumL].SectionLength) + length_diff_L;

                if (!oNewFeature.Components.GetComponent(iCNO).Recordset.EOF)
                {
                    oNewFeature.Components.GetComponent(iCNO).Recordset.MoveFirst();
                    for (int k = 0; k < oNewFeature.Components.GetComponent(iCNO).Recordset.RecordCount; k++)
                    {
                        for (int k1 = 0; k1 < oNewFeature.Components.GetComponent(iCNO).Recordset.Fields.Count; k1++)
                        {
                            if (oNewFeature.Components.GetComponent(iCNO).Recordset.Fields[k1].Name == "G3E_CID")
                            {
                                if (oNewFeature.Components.GetComponent(iCNO).Recordset.Fields[k1].Value.ToString() == CID_R.ToString())
                                    oNewFeature.Components.GetComponent(iCNO).Recordset.Update("DT_S_LENGTH", LenR);
                                if (oNewFeature.Components.GetComponent(iCNO).Recordset.Fields[k1].Value.ToString() == CID_L.ToString())
                                    oNewFeature.Components.GetComponent(iCNO).Recordset.Update("DT_S_LENGTH", LenL);
                            }


                        }
                        oNewFeature.Components.GetComponent(iCNO).Recordset.MoveNext();
                    }
                }
                #endregion
                GTDuctPathEdit.m_oIGTTransactionManager.Commit();
                GTDuctPathEdit.m_oIGTTransactionManager.RefreshDatabaseChanges();
                GTDuctPathEdit.m_gtapp.RefreshWindows();
               GTDuctPathEdit.DuctPathOrigin.Sections[SectNum].SectionLength = txtSectionLength.Text;
                if(SectNumL>=0)
                    GTDuctPathEdit.DuctPathOrigin.Sections[SectNumL].SectionLength = LenL.ToString();
                if (SectNumR >= 0)
                    GTDuctPathEdit.DuctPathOrigin.Sections[SectNumR].SectionLength = LenR.ToString();
                GTDuctPathEdit.DuctPathOrigin.Sections[SectNum].PUSect = mcbMinMaterial.Text;
               GTDuctPathEdit.DuctPathOrigin.Sections[SectNum].SectType = cbSectType.SelectedValue.ToString();
               GTDuctPathEdit.DuctPathOrigin.Sections[SectNum].SectPlc = cbSectPlc.SelectedValue.ToString();
               GTDuctPathEdit.DuctPathOrigin.Sections[SectNum].Encasement = cbEncasement.SelectedValue.ToString();
               GTDuctPathEdit.DuctPathOrigin.Sections[SectNum].SectBackFill = cbSectBackFill.SelectedValue.ToString();
               GTDuctPathEdit.DuctPathOrigin.Sections[SectNum].SectDiam = cbSectDiam.SelectedValue.ToString();
               GTDuctPathEdit.DuctPathOrigin.Sections[SectNum].SectOwner = cbSectOwner.SelectedValue.ToString();
               GTDuctPathEdit.DuctPathOrigin.Sections[SectNum].SectBillingRate = cbSectBillingRate.SelectedValue.ToString();
                
                progressBar1.Value = 100;

            }
            catch (Exception ex)
            {
                GTDuctPathEdit.m_oIGTTransactionManager.Rollback();
                MessageBox.Show(ex.Message, "Duct Path Edit", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            return true;
        }

        #endregion
        #endregion

        #region validate enter values for attr for section
        private bool ValidateDuctPathSect()
        {
            int sectLen=0;
            int lengthRest = int.Parse(txtTotalLength.Text);
            int SectNum = 0;
            int lengthAllowed = 0;
            short CID_up = 0;
            if (ChangeNumberDuctWayPPF != -1)
            {
                for (int i = 0; i < SectionsChangeNumberDuctWayPPF.Count; i++)
                {
                    if (SectionsChangeNumberDuctWayPPF[i].PUSect == "")
                    {
                        SectNum = i;
                        CID_up = SectionsChangeNumberDuctWayPPF[i].CID;
                        break;
                    }
                }
            }else CID_up=short.Parse(lvDPSections.SelectedItems[0].Text);
                    for (int i = 0; i < GTDuctPathEdit.DuctPathOrigin.Sections.Count; i++)
                    {
                        if (GTDuctPathEdit.DuctPathOrigin.Sections[i].CID == CID_up)
                        {
                            SectNum = i;
                            lengthAllowed += int.Parse(GTDuctPathEdit.DuctPathOrigin.Sections[i].SectionLength);
                            if (cbBoth.Checked || cbRight.Checked)
                            {
                                short tempCid = GTDuctPathEdit.DuctPathOrigin.Sections[i].CID;
                                tempCid++;
                                for (int m = 0; m < GTDuctPathEdit.DuctPathOrigin.Sections.Count; m++)
                                {
                                    if (GTDuctPathEdit.DuctPathOrigin.Sections[m].CID == tempCid)
                                    {
                                        lengthAllowed += int.Parse(GTDuctPathEdit.DuctPathOrigin.Sections[m].SectionLength) - 1;
                                        break;
                                    }
                                }
                                //if ((i + 1) < GTDuctPathEdit.DuctPathOrigin.Sections.Count)
                                //    lengthAllowed += int.Parse(GTDuctPathEdit.DuctPathOrigin.Sections[i + 1].SectionLength) - 1;
                            }
                            if (cbBoth.Checked || cbLeft.Checked)
                            {
                                short tempCid = GTDuctPathEdit.DuctPathOrigin.Sections[i].CID;
                                tempCid--;
                                for (int m = 0; m < GTDuctPathEdit.DuctPathOrigin.Sections.Count; m++)
                                {
                                    if (GTDuctPathEdit.DuctPathOrigin.Sections[m].CID == tempCid)
                                    {
                                        lengthAllowed += int.Parse(GTDuctPathEdit.DuctPathOrigin.Sections[m].SectionLength) - 1;
                                        break;
                                    }
                                }
                            }
                            break;
                        }
                    }
            if (int.TryParse(txtSectionLength.Text, out sectLen))
            {
                if (sectLen == 0)
                {
                    MessageBox.Show("Length of Section can not be equal 0!", "Duct Path Edit", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }
                if (sectLen > lengthAllowed)
                {
                    MessageBox.Show("New Length of Section can not be overlap neighbor section\nMaximum is "+lengthAllowed.ToString()+"m allowed!", "Duct Path Edit", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }
 
 
            }
            else
            {
                MessageBox.Show("Section's length should be integer number!", "Duct Path Edit", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            #region add new
            //if (int.TryParse(txtSectionLength.Text, out sectLen))
            //{
            //    if (sectLen == 0)
            //    {
            //        MessageBox.Show("Length of Section can not be equal 0!", "Duct Path Edit", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //        return false;
            //    }
            //    if (Sections.Count > 0)
            //    {                   
            //        for (int i = 0; i < Sections.Count; i++)
            //        {
            //            lengthRest -= int.Parse(Sections[i].SectionLength);
            //        }
            //    }
            //    if (lengthRest < sectLen)
            //    {
            //        MessageBox.Show("Summary length of Section's lengths can not be greater than Total Conduit Length = " + txtTotalLength.Text + " !", "Duct Path Edit", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //        return false;
            //    }
            //}
            //else
            //{
            //    MessageBox.Show("Section's length should be integer number!", "Duct Path Edit", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //    return false;
            //}
#endregion
            if (!int.TryParse(txtNumDuctWaysSect.Text, out sectLen))
            {
                MessageBox.Show("Section's number of Ducts (Ductways) should be integer number!", "Duct Path Edit", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            //else  if (int.Parse(txtNumDuctWays.Text) < sectLen)
            //{
            //    MessageBox.Show("Section's number of duct ways can not be greater than Conduit's = "+txtNumDuctWays.Text+" !", "Duct Path Edit", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //    return false;
            //}
            else if (sectLen == 0)
            {
                MessageBox.Show("Number of duct ways can not be equal 0!", "Duct Path Edit", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            if (!int.TryParse(cbSectDiam.SelectedValue.ToString(), out sectLen))
            {
                MessageBox.Show("Section's diameter should be integer number!", "Duct Path Edit", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            string minmat = Get_Value("select MIN_MATERIAL from ref_civ_ductpath " +
" where DT_S_TYPE='" + cbSectType.SelectedValue.ToString() + "' and  DT_S_WAYS=" + txtNumDuctWaysSect.Text +
" and DT_S_PLACMNT='" + cbSectPlc.SelectedValue.ToString() + "' and DT_S_ENCASE='" + cbEncasement.SelectedValue.ToString() +
               "' and DT_S_BACKFILL='" + cbSectBackFill.SelectedValue.ToString() + "' and DT_S_DIAMETER =" + cbSectDiam.SelectedValue.ToString());

            if (minmat == "")
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
                    MessageBox.Show("Value of Expanded Year is not correct!", "Duct Path Edit", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }
                else if(sectLen < 1900)
                {
                    MessageBox.Show("Value of Expanded Year is not correct!", "Duct Path Edit", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
                    MessageBox.Show("Value of Extended Year is not correct!", "Duct Path Edit", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }
                else if (sectLen < 1900)
                {
                    MessageBox.Show("Value of Extended Year is not correct!", "Duct Path Edit", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
                btnBackToSectList.Enabled = false;
                btnClose3.Enabled = false;
                //placing Conduit(Duct Path)
                if (DuctPathPlacement())
                {
                    //if success - placing Sections (attributes, slash points, labels)
                    if (SectionPlacemet())
                    {
                        
                        progressBar1.Visible = false;
                        progressBar1.Value = 0;
                        DialogResult closeval = MessageBox.Show("Do you want place next Duct Path?", "Duct Path Edit", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (closeval == DialogResult.Yes)
                        {
                            btnBackToSectList.Enabled = true;
                            btnClose3.Enabled = true;
                            if (GTDuctPathEdit.mobjEditService != null)
                                GTDuctPathEdit.mobjEditService.RemoveAllGeometries();
                            GTDuctPathEdit.m_gtapp.SelectedObjects.Clear();
                         //   tabDuctEdit.SelectedTab = tabPagePLC;
                            LastSection = 0;
                            if (SectSlashes != null)
                                SectSlashes.Clear();
                            if (SectLabels != null)
                                SectLabels.Clear();
                            if (Sections != null)
                                Sections.Clear();
                            SourceDevicePoint = null;
                            TermDevicePoint = null;
                           // GTDuctPathEdit.StartDrawPoint = null;
                           // GTDuctPathEdit.EndDrawPoint = null; 
                            if (DuctPathLineGeom != null)
                            {
                                DuctPathLineGeom.Points.Clear();
                                DuctPathLineGeom = null;
                            }
                            //txtFIDSource.Text = "";
                            //txtFIDTerm.Text = "";
                            //txtTypeSource.Text = "";
                            //txtTypeTerm.Text = "";
                            //txtWallSource.Text = "";
                            //txtWallTerm.Text = "";
                            gbSectValues.Enabled = true;
                            gbDuctPathAttrValues.Enabled = true;
                            txtSectionLength.Enabled = true;
                            this.Hide();
                            NumberOfConduit++;
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
                    CloseStatus = 1;
                    Close();
                    Dispose();
                }
            }
            else
            {
                MessageBox.Show("Placement for last section is not finished yet!", "Duct Path Edit", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

        }
        #endregion
    
        #region Change Enable status for Button Confirm Section Attr
        private void btnConfirmSectAttr_EnabledChanged(object sender, EventArgs e)
        {
           // if (btnConfirmSectAttr.Enabled == true)
            //    FillingPage3Sect();
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
            }
            if (btnFinished.Enabled == false)
            {
                btnConfirmSectAttr.Visible = true;
                btnFinished.Visible = false;
            }

        }
        #endregion

        #region filtering
        public void FilteringMinMaterial()
        {
            string sql = "select MIN_MATERIAL from ref_civ_ductpath where " +
                " DT_S_TYPE ='" + cbSectType.SelectedValue.ToString() + "' and " +
                " DT_S_WAYS='" + txtNumDuctWaysSect.Text + "' and " +
                " DT_S_PLACMNT='" + cbSectPlc.SelectedValue.ToString() + "' and " +
                " DT_S_ENCASE='" + cbEncasement.SelectedValue.ToString() + "' and " +
                " DT_S_BACKFILL='" + cbSectBackFill.SelectedValue.ToString() + "' and " +
                " DT_S_DIAMETER='" + cbSectDiam.SelectedValue.ToString() + "' ";
            //     mcbMinMaterial.Text = Get_Value(sql);
        }

        public void FilteringAttributes(string minmat)
        {
            string sSql = "select DT_S_TYPE, DT_S_WAYS, DT_S_PLACMNT, DT_S_ENCASE, DT_S_BACKFILL, DT_S_DIAMETER  from ref_civ_ductpath " +
                        " where MIN_MATERIAL='" + minmat + "'";
            ADODB.Recordset rsPP = new ADODB.Recordset();
            rsPP = GTDuctPathEdit.m_IGTDataContext.OpenRecordset(sSql, ADODB.CursorTypeEnum.adOpenStatic, ADODB.LockTypeEnum.adLockReadOnly, 1);

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
                    if (cbSectDiam.SelectedValue.ToString().ToUpper() == rsPP.Fields[5].Value.ToString())
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

        #endregion

        #region Duct Path Placement
        private bool DuctPathPlacement()
        {
            short iFNO=2200;
            short iCNO;
            int iFID;
            try
            {
                GTDuctPathEdit.m_oIGTTransactionManager.Begin("DuctPathPlc");
                progressBar1.Visible = true;
                progressBar1.Value = 2;
                IGTKeyObject oNewFeature = GTDuctPathEdit.m_IGTDataContext.NewFeature(iFNO);
                iFID = oNewFeature.FID;
                NewPathFID = iFID;
                
                #region Attributes
                iCNO = 2201;
                    if (oNewFeature.Components.GetComponent(iCNO).Recordset.EOF)
                    {
                        oNewFeature.Components.GetComponent(iCNO).Recordset.AddNew("G3E_FID", iFID);
                        oNewFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_FNO", iFNO);
                        oNewFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_CID", 1);
                        oNewFeature.Components.GetComponent(iCNO).Recordset.Update("CENTRALDB_FLAG", txtFlag.Text.Substring(0,1));
                        oNewFeature.Components.GetComponent(iCNO).Recordset.Update("DT_WAYS", cbNumDuctWays.SelectedValue.ToString());
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
                        oNewFeature.Components.GetComponent(iCNO).Recordset.Update("CENTRALDB_FLAG", txtFlag.Text.Substring(0, 1));
                        oNewFeature.Components.GetComponent(iCNO).Recordset.Update("DT_WAYS", cbNumDuctWays.SelectedValue.ToString());
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

                #region Section Attributes
                iCNO = 2202;

                if (oNewFeature.Components.GetComponent(iCNO).Recordset.EOF)
                {
                    oNewFeature.Components.GetComponent(iCNO).Recordset.AddNew("G3E_FID", iFID);
                    oNewFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_FNO", iFNO);
                    oNewFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_CID", 1);
                    oNewFeature.Components.GetComponent(iCNO).Recordset.Update("DT_S_WAYS", int.Parse(txtNumDuctWaysSect.Text));
                    oNewFeature.Components.GetComponent(iCNO).Recordset.Update("DT_S_LENGTH", int.Parse(txtTotalLength.Text));
                    oNewFeature.Components.GetComponent(iCNO).Recordset.Update("DT_S_TYPE", cbSectType.SelectedValue);
                    oNewFeature.Components.GetComponent(iCNO).Recordset.Update("DT_S_PLACMNT", cbSectPlc.SelectedValue);
                    oNewFeature.Components.GetComponent(iCNO).Recordset.Update("DT_S_ENCASE", cbEncasement.SelectedValue);
                    oNewFeature.Components.GetComponent(iCNO).Recordset.Update("DT_S_DIAMETER", int.Parse(cbSectDiam.SelectedValue.ToString()));
                    oNewFeature.Components.GetComponent(iCNO).Recordset.Update("DT_S_BACKFILL", cbSectBackFill.SelectedValue);
                    oNewFeature.Components.GetComponent(iCNO).Recordset.Update("COMMON_TRENCH", cbSectOwner.SelectedValue);                    
                    oNewFeature.Components.GetComponent(iCNO).Recordset.Update("BILLING_RATE", cbSectBillingRate.SelectedValue);
                }
                else
                {
                    oNewFeature.Components.GetComponent(iCNO).Recordset.MoveLast();
                    oNewFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_FNO", iFNO);
                    oNewFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_CID", 1);
                    oNewFeature.Components.GetComponent(iCNO).Recordset.Update("DT_S_WAYS", int.Parse(txtNumDuctWaysSect.Text));
                    oNewFeature.Components.GetComponent(iCNO).Recordset.Update("DT_S_LENGTH", int.Parse(txtTotalLength.Text));
                    oNewFeature.Components.GetComponent(iCNO).Recordset.Update("DT_S_TYPE", cbSectType.SelectedValue);
                    oNewFeature.Components.GetComponent(iCNO).Recordset.Update("DT_S_PLACMNT", cbSectPlc.SelectedValue);
                    oNewFeature.Components.GetComponent(iCNO).Recordset.Update("DT_S_ENCASE", cbEncasement.SelectedValue);
                    oNewFeature.Components.GetComponent(iCNO).Recordset.Update("DT_S_DIAMETER", int.Parse(cbSectDiam.SelectedValue.ToString()));
                    oNewFeature.Components.GetComponent(iCNO).Recordset.Update("DT_S_BACKFILL", cbSectBackFill.SelectedValue);
                    oNewFeature.Components.GetComponent(iCNO).Recordset.Update("COMMON_TRENCH", cbSectOwner.SelectedValue);                    
                    oNewFeature.Components.GetComponent(iCNO).Recordset.Update("BILLING_RATE", cbSectBillingRate.SelectedValue);
                }
                progressBar1.Value = 20;
                #endregion

                #region Section Label
                iCNO = 2232;
                IGTTextPointGeometry oTextGeom = GTClassFactory.Create<IGTTextPointGeometry>();
                IGTPoint temp = PointForSlashBasedOnEnteredLength(TotalGraphicLength / 2);
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
                oTextGeom = GTClassFactory.Create<IGTTextPointGeometry>();
                IGTPoint oPointText = GTClassFactory.Create<IGTPoint>();
                oPointText.X = temp.X;
                oPointText.Y = temp.Y;
                oPointText.Z = 0;
                oTextGeom.Origin = oPointText;
                oTextGeom.Rotation = TakeRotationOfSegmentPolyline(TotalGraphicLength / 2);
                oNewFeature.Components.GetComponent(iCNO).Geometry = oTextGeom;
                progressBar1.Value = 25;
                #endregion
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


                IGTKeyObject oSource = GTDuctPathEdit.m_IGTDataContext.OpenFeature(SourceFNO, SourceFID);
                IGTKeyObject oTerm = GTDuctPathEdit.m_IGTDataContext.OpenFeature(TermFNO, TermFID);
             //   IGTKeyObject oDuctPath = m_IGTDataContext.OpenFeature(2200, iFID);

                GTDuctPathEdit.mobjRelationshipService.ActiveFeature = oSource;

                if (GTDuctPathEdit.mobjRelationshipService.AllowSilentEstablish(oNewFeature))
                    GTDuctPathEdit.mobjRelationshipService.SilentEstablish(1, oNewFeature, GTRelationshipOrdinalConstants.gtrelRelationshipOrdinal1);
                else
                {
                    GTDuctPathEdit.m_oIGTTransactionManager.Rollback();
                    MessageBox.Show("Error during trying reestablish relationship!", "Duct Path Edit", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }

                GTDuctPathEdit.mobjRelationshipService.ActiveFeature = oNewFeature;
                if (GTDuctPathEdit.mobjRelationshipService.AllowSilentEstablish(oTerm))
                    GTDuctPathEdit.mobjRelationshipService.SilentEstablish(1, oTerm, GTRelationshipOrdinalConstants.gtrelRelationshipOrdinal2);
                else
                {
                    GTDuctPathEdit.m_oIGTTransactionManager.Rollback();
                    MessageBox.Show("Error during trying reestablish relationship!", "Duct Path Edit", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
                progressBar1.Value = 30;
                #endregion

                GTDuctPathEdit.m_oIGTTransactionManager.Commit();
                progressBar1.Value = 40;
                GTDuctPathEdit.m_oIGTTransactionManager.RefreshDatabaseChanges();
                progressBar1.Value = 42;


                
                
            }
            catch (Exception ex)
            {
                GTDuctPathEdit.m_oIGTTransactionManager.Rollback();
                MessageBox.Show(ex.Message, "Duct Path Edit", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            return true;

        }

        #region Make NE connection
        private bool CreateNEConnectionDuctPath(int iFID)
        {
            try
            {
                GTDuctPathEdit.m_oIGTTransactionManager.Begin("CreateNEConnection");

                IGTKeyObject oSource = GTDuctPathEdit.m_IGTDataContext.OpenFeature(SourceFNO, SourceFID);
                IGTKeyObject oTerm = GTDuctPathEdit.m_IGTDataContext.OpenFeature(TermFNO, TermFID);
                IGTKeyObject oDuctPath = GTDuctPathEdit.m_IGTDataContext.OpenFeature(2200, iFID);

                GTDuctPathEdit.mobjRelationshipService.ActiveFeature = oSource;

                if (GTDuctPathEdit.mobjRelationshipService.AllowSilentEstablish(oDuctPath))
                    GTDuctPathEdit.mobjRelationshipService.SilentEstablish(1, oDuctPath, GTRelationshipOrdinalConstants.gtrelRelationshipOrdinal1);
                else
                {
                    GTDuctPathEdit.m_oIGTTransactionManager.Rollback();
                    MessageBox.Show("Error during trying reestablish relationship!", "Duct Path Edit", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }

                GTDuctPathEdit.mobjRelationshipService.ActiveFeature = oDuctPath;
                if (GTDuctPathEdit.mobjRelationshipService.AllowSilentEstablish(oTerm))
                    GTDuctPathEdit.mobjRelationshipService.SilentEstablish(1, oTerm, GTRelationshipOrdinalConstants.gtrelRelationshipOrdinal2);
                else
                {
                    GTDuctPathEdit.m_oIGTTransactionManager.Rollback();
                    MessageBox.Show("Error during trying reestablish relationship!", "Duct Path Edit", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
                GTDuctPathEdit.m_oIGTTransactionManager.Commit();
                GTDuctPathEdit.m_oIGTTransactionManager.RefreshDatabaseChanges();
                return true; ;
            }
            catch (Exception ex)
            {
                GTDuctPathEdit.m_oIGTTransactionManager.Rollback();
                MessageBox.Show(ex.Message, "Duct Path Edit", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                GTDuctPathEdit.mobjEditService.RemoveGeometry(GTDuctPathEdit.mobjEditService.GeometryCount);
                SectSlashes.RemoveAt(SectSlashes.Count - 1);
            }
            temp.Orientation = OrientationForPointOnConduit(tempp.X, tempp.Y, len);
            GTDuctPathEdit.mobjEditService.AddGeometry(temp, 23000);
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

                GTDuctPathEdit.m_oIGTTransactionManager.Begin("DuctPathPlc");
                progressBar1.Value = indexProgress;
                IGTKeyObject oNewFeature = GTDuctPathEdit.m_IGTDataContext.OpenFeature(iFNO, iFID);
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
                        iCNO = 2232;
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
                        iCNO = 2232;

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

                GTDuctPathEdit.m_oIGTTransactionManager.Commit();
                progressBar1.Value = 95;
                GTDuctPathEdit.m_oIGTTransactionManager.RefreshDatabaseChanges();
                progressBar1.Value = 100;
                GTDuctPathEdit.mobjEditService.RemoveAllGeometries();
            }
            catch (Exception ex)
            {
                GTDuctPathEdit.m_oIGTTransactionManager.Rollback();
                MessageBox.Show(ex.Message, "Duct Path Edit", MessageBoxButtons.OK, MessageBoxIcon.Error);                
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


            for (int i = 0; i < GTDuctPathEdit.DuctPathOrigin.DuctPathLineGeom.Points.Count - 1; i++)
            {

                int temp = LegthBtwTwoPoints(GTDuctPathEdit.DuctPathOrigin.DuctPathLineGeom.Points[i].X,
                    GTDuctPathEdit.DuctPathOrigin.DuctPathLineGeom.Points[i].Y,
                    GTDuctPathEdit.DuctPathOrigin.DuctPathLineGeom.Points[i + 1].X,
                    GTDuctPathEdit.DuctPathOrigin.DuctPathLineGeom.Points[i + 1].Y);
                lengthTemp += temp;

                if (lengthTemp >= lengthSlash)
                {
                    return Orientation.BuildVector(GTDuctPathEdit.DuctPathOrigin.DuctPathLineGeom.Points[i], slashPoint);
                }
            }

            return Orientation.BuildVector(GTDuctPathEdit.DuctPathOrigin.DuctPathLineGeom.Points[0], GTDuctPathEdit.DuctPathOrigin.DuctPathLineGeom.Points[GTDuctPathEdit.DuctPathOrigin.DuctPathLineGeom.Points.Count - 1]);

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
            SlashPoint.X = GTDuctPathEdit.DuctPathOrigin.DuctPathLineGeom.Points[GTDuctPathEdit.DuctPathOrigin.DuctPathLineGeom.Points.Count - 1].X;
            SlashPoint.Y = GTDuctPathEdit.DuctPathOrigin.DuctPathLineGeom.Points[GTDuctPathEdit.DuctPathOrigin.DuctPathLineGeom.Points.Count - 1].Y;
            SlashPoint.Z = 0.0;

            for (int i = 0; i < GTDuctPathEdit.DuctPathOrigin.DuctPathLineGeom.Points.Count - 1; i++)
            {

                int temp = LegthBtwTwoPoints(GTDuctPathEdit.DuctPathOrigin.DuctPathLineGeom.Points[i].X,
                    GTDuctPathEdit.DuctPathOrigin.DuctPathLineGeom.Points[i].Y,
                    GTDuctPathEdit.DuctPathOrigin.DuctPathLineGeom.Points[i + 1].X,
                    GTDuctPathEdit.DuctPathOrigin.DuctPathLineGeom.Points[i + 1].Y);
                lengthTemp += temp;
                if (lengthTemp >= GraphicLength)
                {
                    length = temp - lengthTemp + GraphicLength;
                    if (temp == 0) temp = 1;
                    SlashPoint.X = length * (GTDuctPathEdit.DuctPathOrigin.DuctPathLineGeom.Points[i + 1].X - GTDuctPathEdit.DuctPathOrigin.DuctPathLineGeom.Points[i].X) / temp + GTDuctPathEdit.DuctPathOrigin.DuctPathLineGeom.Points[i].X;
                    SlashPoint.Y = length * (GTDuctPathEdit.DuctPathOrigin.DuctPathLineGeom.Points[i + 1].Y - GTDuctPathEdit.DuctPathOrigin.DuctPathLineGeom.Points[i].Y) / temp + GTDuctPathEdit.DuctPathOrigin.DuctPathLineGeom.Points[i].Y;
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
          //  GTDuctPathEdit.m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "PLACING LABEL FOR SECTION");
            GTDuctPathEdit.m_gtapp.SelectedObjects.Clear();
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
            //if (GTDuctPathEdit.LabelAlongLine == null)
            //    GTDuctPathEdit.LabelAlongLine = GTClassFactory.Create<IGTTextPointGeometry>();
            //if (GTDuctPathEdit.oTextGeomLabel == null)
            //    GTDuctPathEdit.oTextGeomLabel = GTClassFactory.Create<IGTTextPointGeometry>();
            tempp = PointForSlashBasedOnEnteredLength(len);

            //GTDuctPathEdit.LabelAlongLine.Origin = tempp;
            //GTDuctPathEdit.LabelAlongLine.Text = SectLabel;
            //GTDuctPathEdit.LabelAlongLine.Rotation = TakeRotationOfSegmentPolyline(len);
            //GTDuctPathEdit.LabelAlongLine.Alignment = 0;
            //GTDuctPathEdit.mobjEditService.AddGeometry(GTDuctPathEdit.LabelAlongLine, 32400);

            //GTDuctPathEdit.oTextGeomLabel.Origin = tempp;
            //GTDuctPathEdit.oTextGeomLabel.Text = SectLabel;
            //GTDuctPathEdit.oTextGeomLabel.Alignment = 0;
            //GTDuctPathEdit.oTextGeomLabel.Rotation = GTDuctPathEdit.LabelAlongLine.Rotation;
            this.Hide();
                    GTDuctPathEdit.m_gtapp.SelectedObjects.Clear();
               //     GTDuctPathEdit.startdraw = 30;
                   // GTDuctPathEdit.m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "LEFT MOUSE CLICK TO PLACE TEMPORARY LABEL ON MAP, DOUBLE MOUSE CLICK TO CONFIRM LOCATION OF LABEL");
             
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
            if (TotalGraphicLength != TotalEnteredLength)
            {
                if (TotalEnteredLength == 0) TotalEnteredLength = 1;
                S.SectGraphicLength = TotalGraphicLength * int.Parse(txtSectionLength.Text) / TotalEnteredLength;
            }
            else S.SectGraphicLength = int.Parse(txtSectionLength.Text);


          //  S.YearExpanded = txtYearExpanded.Text;
           // S.YearExtended = txtYearExtended.Text;
            int year1;

            //if (txtYearExpanded.Text.Trim() != "0" &&
            //                    txtYearExpanded.Text.Trim() != "00" &&
            //                    txtYearExpanded.Text.Trim() != "000" &&
            //                    txtYearExpanded.Text.Trim() != "0000" &&
            //                    txtYearExpanded.Text.Trim() != "")
            //{
            //    if (int.TryParse(txtYearExpanded.Text.Trim(), out year1))
            //        S.YearExpanded = new DateTime(year1, 1, 1);
            //    else S.YearExpanded = null;
            //}
            //else S.YearExpanded = null;

            //if (txtYearExtended.Text.Trim() != "0" &&
            //                    txtYearExtended.Text.Trim() != "00" &&
            //                    txtYearExtended.Text.Trim() != "000" &&
            //                    txtYearExtended.Text.Trim() != "0000" &&
            //                    txtYearExtended.Text.Trim() != "")
            //{
            //    if (int.TryParse(txtYearExtended.Text.Trim(), out year1))
            //        S.YearExtended = new DateTime(year1, 1, 1);
            //    else S.YearExtended = null;
            //}
            //else S.YearExtended = null;

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
            
             GTDuctPathEdit.m_gtapp.SelectedObjects.Clear();
             
            if (flag == 1)//fit for source feature
             {
                 feat = GTDuctPathEdit.m_gtapp.DataContext.GetDDCKeyObjects(iFNO, lFID, GTComponentGeometryConstants.gtddcgAllGeographic);
                

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
                         GTDuctPathEdit.m_gtapp.RefreshWindows();
                         GTDuctPathEdit.m_gtapp.SelectedObjects.Clear();
                         return;
                     }

                     if (feat[K].ComponentViewName == "VGC_CHAMBER_P" ||
                         feat[K].ComponentViewName == "VGC_TUNNEL_P" )
                     {
                         //3800 VGC_CHAMBER_P
                         //3300 VGC_TUNNEL_P
                         for (int K2 = 0; K2 < feat.Count; K2++)
                             GTDuctPathEdit.m_gtapp.SelectedObjects.Add(GTSelectModeConstants.gtsosmAllDisplayedComponentsInActiveLegend, feat[K2]);
                         window.FitSelectedObjects();
                         window.CenterSelectedObjects();
                         GTDuctPathEdit.m_gtapp.RefreshWindows();
                         GTDuctPathEdit.m_gtapp.SelectedObjects.Clear();
                         return;
                     }
                 }
                 
             }
             if (flag == 2)//copy source feature to selected obj to fit both source and term
             {
                 feat = GTDuctPathEdit.m_gtapp.DataContext.GetDDCKeyObjects(iFNO, lFID, GTComponentGeometryConstants.gtddcgAllGeographic);
                 for (int K = 0; K < feat.Count; K++)
                     GTDuctPathEdit.m_gtapp.SelectedObjects.Add(GTSelectModeConstants.gtsosmAllDisplayedComponentsInActiveLegend, feat[K]);
             }
             iFNO = TermFNO;
             lFID = TermFID;// int.Parse(txtFIDTerm.Text);
           
            if (flag == 3)//fit for term feature
            {
                GTDuctPathEdit.m_gtapp.SelectedObjects.Clear();           
                feat = GTDuctPathEdit.m_gtapp.DataContext.GetDDCKeyObjects(iFNO, lFID, GTComponentGeometryConstants.gtddcgAllGeographic);
               
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
                        GTDuctPathEdit.m_gtapp.RefreshWindows();
                        GTDuctPathEdit.m_gtapp.SelectedObjects.Clear();
                        return;
                    }

                    if (feat[K].ComponentViewName == "VGC_CHAMBER_P" ||
                        feat[K].ComponentViewName == "VGC_TUNNEL_P")
                    {
                        //3800 VGC_CHAMBER_P
                        //3300 VGC_TUNNEL_P
                        for (int K2 = 0; K2 < feat.Count; K2++)
                            GTDuctPathEdit.m_gtapp.SelectedObjects.Add(GTSelectModeConstants.gtsosmAllDisplayedComponentsInActiveLegend, feat[K2]);
                        window.FitSelectedObjects();
                        window.CenterSelectedObjects();
                        GTDuctPathEdit.m_gtapp.RefreshWindows();
                        GTDuctPathEdit.m_gtapp.SelectedObjects.Clear();
                        return;
                    }
                }
                 
            }
           //copy term feature to selected obj to fit both source and term
            feat = GTDuctPathEdit.m_gtapp.DataContext.GetDDCKeyObjects(iFNO, lFID, GTComponentGeometryConstants.gtddcgAllGeographic);
            for (int K = 0; K < feat.Count; K++)
                GTDuctPathEdit.m_gtapp.SelectedObjects.Add(GTSelectModeConstants.gtsosmAllDisplayedComponentsInActiveLegend, feat[K]);

            window.CenterSelectedObjects();
            window.FitSelectedObjects();
            GTDuctPathEdit.m_gtapp.RefreshWindows();
           // GTDuctPathEdit.m_gtapp.SelectedObjects.Clear();
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

            Content += " ";

            if (Sections[SectNum].SectOwner == "TELEKOM MALAYSIA, TM")
                Content += "(CT:TM)";
            else if (Sections[SectNum].SectOwner == "DEVELOPER, DEV")
                Content += "(CT:DEV)";
            else if (Sections[SectNum].SectOwner == "BINARIANG, BIN")
                Content += "(CT:BIN)";
            else if (Sections[SectNum].SectOwner == "CELCOM, CEL")
                Content += "(CT:CEL)";
            else if (Sections[SectNum].SectOwner == "FIBRERAIL, FIB")
                Content += "(CT:FIB)";
            else if (Sections[SectNum].SectOwner == "MOBIKOM, MOB")
                Content += "(CT:MOB)";
            else if (Sections[SectNum].SectOwner == "MUTIARA, MUT")
                Content += "(CT:MUT)";
            else if (Sections[SectNum].SectOwner == "STW, STW")
                Content += "(CT:STW)";
            else if (Sections[SectNum].SectOwner == "TIME, TIM")
                Content += "(CT:TIM)";

            Content += "\n" +Sections[SectNum].SectionLength+ " m";
            return Content;
        }
        #endregion

        #region Expand Duct Path
        //private void btnExpand_Click(object sender, EventArgs e)
        //{
        //    if (btnExpand.Text == "Expand")
        //    {
        //        if (!(GTDuctPathEdit.DuctPathOrigin.Feature_state == "ASB"))
        //        {
        //            MessageBox.Show("Expandion is only avaliable for ASB Duct Path", "Duct Path Edit", MessageBoxButtons.OK, MessageBoxIcon.Information);
        //        }
        //        else
        //        {
        //            if (ExpandDuctPath())
        //            {
        //                MessageBox.Show("Expandion is successed", "Duct Path Edit", MessageBoxButtons.OK, MessageBoxIcon.Information);

        //            }
        //        }
        //    }
        //    else if (btnExpand.Text == "Cancel Expand")
        //    {
        //        if (ExpandDuctPath())
        //        {
        //            MessageBox.Show("Cancellation of Expandion is successed", "Duct Path Edit", MessageBoxButtons.OK, MessageBoxIcon.Information);
        //            btnExpand.Text = "Expand";
        //        }
        //    }
        //}
        //private bool ExpandDuctPath()
        //{
        //    if (btnExpand.Text == "Expand")
        //    {
        //        try
        //        {
        //            int ExpandCount = int.Parse(txtNumDuctWays.Text);
        //            GTDuctPathEdit.m_oIGTTransactionManager.Begin("Expand Duct Path");
        //            IGTKeyObject oDuctPathFeature = GTDuctPathEdit.m_IGTDataContext.OpenFeature(GTDuctPathEdit.DuctPathOrigin.FNO, GTDuctPathEdit.DuctPathOrigin.FID);
        //            if (!oDuctPathFeature.Components.GetComponent(51).Recordset.EOF)
        //            {
        //                oDuctPathFeature.Components.GetComponent(51).Recordset.MoveLast();
        //                oDuctPathFeature.Components.GetComponent(51).Recordset.Update("FEATURE_STATE", "MOD");
        //            }
        //            if (!oDuctPathFeature.Components.GetComponent(2202).Recordset.EOF)
        //            {
        //                oDuctPathFeature.Components.GetComponent(2202).Recordset.MoveFirst();
        //                for (int j = 0; j < oDuctPathFeature.Components.GetComponent(2202).Recordset.RecordCount; j++)
        //                {
        //                    oDuctPathFeature.Components.GetComponent(2202).Recordset.Update("DT_S_PP_WAYS", ExpandCount);
        //                    oDuctPathFeature.Components.GetComponent(2202).Recordset.Update("YEAR_EXPANDED", new DateTime(DateTime.Now.Year, 1, 1));
        //                    oDuctPathFeature.Components.GetComponent(2202).Recordset.MoveNext();
        //                }
        //            }
        //            GTDuctPathEdit.m_oIGTTransactionManager.Commit();
        //            GTDuctPathEdit.m_oIGTTransactionManager.RefreshDatabaseChanges();
        //            return true;
        //        }
        //        catch (Exception ex)
        //        {
        //            if (GTDuctPathEdit.m_oIGTTransactionManager.TransactionInProgress)
        //                GTDuctPathEdit.m_oIGTTransactionManager.Rollback();
        //            MessageBox.Show(ex.Message, "Duct Path Edit", MessageBoxButtons.OK, MessageBoxIcon.Error);
        //            CloseStatus = 1;
        //            this.Close();
        //        }
        //    }
        //    else if (btnExpand.Text == "Cancel Expand")
        //    {
        //        try
        //        {
        //            int ExpandCount = int.Parse(txtNumDuctWays.Text);
        //            GTDuctPathEdit.m_oIGTTransactionManager.Begin("Cancel Expand Duct Path");
        //            IGTKeyObject oDuctPathFeature = GTDuctPathEdit.m_IGTDataContext.OpenFeature(GTDuctPathEdit.DuctPathOrigin.FNO, GTDuctPathEdit.DuctPathOrigin.FID);
        //            if (!oDuctPathFeature.Components.GetComponent(51).Recordset.EOF)
        //            {
        //                oDuctPathFeature.Components.GetComponent(51).Recordset.MoveLast();
        //                oDuctPathFeature.Components.GetComponent(51).Recordset.Update("FEATURE_STATE", "ASB");
        //            }
        //            if (!oDuctPathFeature.Components.GetComponent(2202).Recordset.EOF)
        //            {
        //                oDuctPathFeature.Components.GetComponent(2202).Recordset.MoveFirst();
        //                for (int j = 0; j < oDuctPathFeature.Components.GetComponent(2202).Recordset.RecordCount; j++)
        //                {
        //                    oDuctPathFeature.Components.GetComponent(2202).Recordset.Update("DT_S_PP_WAYS", 0);
        //                    oDuctPathFeature.Components.GetComponent(2202).Recordset.Update("YEAR_EXPANDED", null);
        //                    oDuctPathFeature.Components.GetComponent(2202).Recordset.MoveNext();
        //                }
        //            }
        //            GTDuctPathEdit.m_oIGTTransactionManager.Commit();
        //            GTDuctPathEdit.m_oIGTTransactionManager.RefreshDatabaseChanges();
        //            return true;
        //        }
        //        catch (Exception ex)
        //        {
        //            if (GTDuctPathEdit.m_oIGTTransactionManager.TransactionInProgress)
        //                GTDuctPathEdit.m_oIGTTransactionManager.Rollback();
        //            MessageBox.Show(ex.Message, "Duct Path Edit", MessageBoxButtons.OK, MessageBoxIcon.Error);
        //            CloseStatus = 1;
        //            this.Close();
        //        }
        //    }
        //    return false;
        //}
        #endregion
        
       #region Extend Duct Path (for stub/stump only)
        private void btnExtend_Click(object sender, EventArgs e)
        {
            if (!(GTDuctPathEdit.DuctPathOrigin.Feature_state == "ASB" &&
                (GTDuctPathEdit.DuctPathOrigin.termType=="STUMP" || GTDuctPathEdit.DuctPathOrigin.termType=="STUB")))
            {
                MessageBox.Show("Extention is only avaliable for ASB Stump/Stub Duct Path", "Duct Path Edit", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        #endregion

       

       

       

       






    }
      
}