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

namespace NEPS.GTechnology.SDFplacement
{
    public partial class SDF_Plc_Form : Form
    {
      

         #region variables
          
        private IGTDataContext m_GTDataContext = null;
        private Intergraph.GTechnology.API.IGTApplication m_gtapp = null;

        #region class to fill combo box
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

        public int PlaceValue = 0;

        class SDF
        {
            public int number;
            public int row;
            public string Code;
        };

        List<SDF> SDFgroup = null;

        int VDSL2fid = 0;
        string VDSL2Code = "";

        class DetailWindowCoor
        {
            public double X;
            public double Y;
            public int detailID;
        };
        

        #endregion

        #region initial form
        public SDF_Plc_Form()
        {
            try
            {
                InitializeComponent();
                m_gtapp = GTSDFplacement.application;

                m_GTDataContext = m_gtapp.DataContext;
                m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Running SDF Placement..");
                FillingGraniteTabPicklist();
              
            }
            catch (Exception ex)
            {    
                MessageBox.Show(ex.Message, "SDF Placement", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }        
        
               
        #endregion

        #region Filling GraniteTab Picklists
        private void FillingGraniteTabPicklist()
        {

            if (cbGrTemplate.DataSource == null)
            {
                ADODB.Recordset rsPP = new ADODB.Recordset();
                string sSql = "SELECT DISTINCT GRANITE_TEMPLATE FROM REF_GRANITE_TEMPLATESDF order by GRANITE_TEMPLATE";
                rsPP = m_GTDataContext.OpenRecordset(sSql, ADODB.CursorTypeEnum.adOpenStatic, ADODB.LockTypeEnum.adLockReadOnly, 1);
                cbGrTemplate.Items.Clear();
                if (rsPP.RecordCount > 0)
                {
                    List<ComboBoxItems> item = new List<ComboBoxItems>();

                    rsPP.MoveFirst();
                    for (int i = 0; i < rsPP.RecordCount; i++)
                    {
                        item.Add(new ComboBoxItems(rsPP.Fields[0].Value.ToString(), rsPP.Fields[0].Value.ToString()));
                        rsPP.MoveNext();
                    }
                    cbGrTemplate.DataSource = item;
                    cbGrTemplate.DisplayMember = "Text";
                    cbGrTemplate.ValueMember = "Value";
                }
                cbGrTemplate.SelectedIndex = -1;
            }

            if (cbContractor.DataSource == null)
            {
                ADODB.Recordset rsPP = new ADODB.Recordset();
                string sSql = "SELECT DISTINCT CONTRACTOR FROM REF_GRANITE_TEMPLATESDF  order by CONTRACTOR";
                rsPP = m_GTDataContext.OpenRecordset(sSql, ADODB.CursorTypeEnum.adOpenStatic, ADODB.LockTypeEnum.adLockReadOnly, 1);
                cbContractor.Items.Clear();
                if (rsPP.RecordCount > 0)
                {
                    List<ComboBoxItems> item = new List<ComboBoxItems>();

                    rsPP.MoveFirst();
                    for (int i = 0; i < rsPP.RecordCount; i++)
                    {
                        item.Add(new ComboBoxItems(rsPP.Fields[0].Value.ToString(), rsPP.Fields[0].Value.ToString()));
                        rsPP.MoveNext();
                    }
                    cbContractor.DataSource = item;
                    cbContractor.DisplayMember = "Text";
                    cbContractor.ValueMember = "Value";
                }
                cbContractor.SelectedIndex = -1;
            }

            if (cbManufacturer.DataSource == null)
            {
                ADODB.Recordset rsPP = new ADODB.Recordset();
                string sSql = "SELECT DISTINCT MANUFACTURER FROM REF_GRANITE_TEMPLATESDF  order by MANUFACTURER";
                rsPP = m_GTDataContext.OpenRecordset(sSql, ADODB.CursorTypeEnum.adOpenStatic, ADODB.LockTypeEnum.adLockReadOnly, 1);
                cbManufacturer.Items.Clear();
                if (rsPP.RecordCount > 0)
                {
                    List<ComboBoxItems> item = new List<ComboBoxItems>();

                    rsPP.MoveFirst();
                    for (int i = 0; i < rsPP.RecordCount; i++)
                    {
                        item.Add(new ComboBoxItems(rsPP.Fields[0].Value.ToString(), rsPP.Fields[0].Value.ToString()));
                        rsPP.MoveNext();
                    }
                    cbManufacturer.DataSource = item;
                    cbManufacturer.DisplayMember = "Text";
                    cbManufacturer.ValueMember = "Value";
                }
                cbManufacturer.SelectedIndex = -1;
            }

            if (cbAccessRestr.DataSource == null)
            {
                ADODB.Recordset rsPP = new ADODB.Recordset();
                string sSql = "SELECT DISTINCT ACCESS_RESTRICTION FROM REF_GRANITE_ACCESSRESTRICTION  order by ACCESS_RESTRICTION";
                rsPP = m_GTDataContext.OpenRecordset(sSql, ADODB.CursorTypeEnum.adOpenStatic, ADODB.LockTypeEnum.adLockReadOnly, 1);
                cbAccessRestr.Items.Clear();
                if (rsPP.RecordCount > 0)
                {
                    List<ComboBoxItems> item = new List<ComboBoxItems>();

                    rsPP.MoveFirst();
                    for (int i = 0; i < rsPP.RecordCount; i++)
                    {
                        item.Add(new ComboBoxItems(rsPP.Fields[0].Value.ToString(), rsPP.Fields[0].Value.ToString()));
                        rsPP.MoveNext();
                    }
                    cbAccessRestr.DataSource = item;
                    cbAccessRestr.DisplayMember = "Text";
                    cbAccessRestr.ValueMember = "Value";
                }
                cbAccessRestr.SelectedIndex = -1;
            }

            if (cbEquipLoc.DataSource == null)
            {
                ADODB.Recordset rsPP = new ADODB.Recordset();
                string sSql = "SELECT DISTINCT EQUIP_LOCATION FROM REF_GRANITE_EQUIPLOCATION  order by EQUIP_LOCATION";
                rsPP = m_GTDataContext.OpenRecordset(sSql, ADODB.CursorTypeEnum.adOpenStatic, ADODB.LockTypeEnum.adLockReadOnly, 1);
                cbEquipLoc.Items.Clear();
                if (rsPP.RecordCount > 0)
                {
                    List<ComboBoxItems> item = new List<ComboBoxItems>();

                    rsPP.MoveFirst();
                    for (int i = 0; i < rsPP.RecordCount; i++)
                    {
                        item.Add(new ComboBoxItems(rsPP.Fields[0].Value.ToString(), rsPP.Fields[0].Value.ToString()));
                        rsPP.MoveNext();
                    }
                    cbEquipLoc.DataSource = item;
                    cbEquipLoc.DisplayMember = "Text";
                    cbEquipLoc.ValueMember = "Value";
                }
                cbEquipLoc.SelectedIndex = -1;
            }

            if (cbCopperOwnTM.DataSource == null)
            {
                ADODB.Recordset rsPP = new ADODB.Recordset();
                string sSql = "SELECT DISTINCT PICKLIST FROM REF_PREMISEEXIST  order by PICKLIST";
                rsPP = m_GTDataContext.OpenRecordset(sSql, ADODB.CursorTypeEnum.adOpenStatic, ADODB.LockTypeEnum.adLockReadOnly, 1);
                cbCopperOwnTM.Items.Clear();
                if (rsPP.RecordCount > 0)
                {
                    List<ComboBoxItems> item = new List<ComboBoxItems>();

                    rsPP.MoveFirst();
                    for (int i = 0; i < rsPP.RecordCount; i++)
                    {
                        item.Add(new ComboBoxItems(rsPP.Fields[0].Value.ToString(), rsPP.Fields[0].Value.ToString()));
                        rsPP.MoveNext();
                    }
                    cbCopperOwnTM.DataSource = item;
                    cbCopperOwnTM.DisplayMember = "Text";
                    cbCopperOwnTM.ValueMember = "Value";
                }
                cbCopperOwnTM.SelectedIndex = -1;
            }

            if (cbFiberToPremise.DataSource == null)
            {
                ADODB.Recordset rsPP = new ADODB.Recordset();
                string sSql = "SELECT DISTINCT PICKLIST FROM REF_PREMISEEXIST order by PICKLIST";
                rsPP = m_GTDataContext.OpenRecordset(sSql, ADODB.CursorTypeEnum.adOpenStatic, ADODB.LockTypeEnum.adLockReadOnly, 1);
                cbFiberToPremise.Items.Clear();
                if (rsPP.RecordCount > 0)
                {
                    List<ComboBoxItems> item = new List<ComboBoxItems>();

                    rsPP.MoveFirst();
                    for (int i = 0; i < rsPP.RecordCount; i++)
                    {
                        item.Add(new ComboBoxItems(rsPP.Fields[0].Value.ToString(), rsPP.Fields[0].Value.ToString()));
                        rsPP.MoveNext();
                    }
                    cbFiberToPremise.DataSource = item;
                    cbFiberToPremise.DisplayMember = "Text";
                    cbFiberToPremise.ValueMember = "Value";
                }
                cbFiberToPremise.SelectedIndex = -1;
            }

            if (cbCableType.DataSource == null)
            {
                ADODB.Recordset rsPP = new ADODB.Recordset();
                string sSql = "SELECT DISTINCT PICKLIST FROM REF_CABLINGTYPE order by PICKLIST";
                rsPP = m_GTDataContext.OpenRecordset(sSql, ADODB.CursorTypeEnum.adOpenStatic, ADODB.LockTypeEnum.adLockReadOnly, 1);
                cbCableType.Items.Clear();
                if (rsPP.RecordCount > 0)
                {
                    List<ComboBoxItems> item = new List<ComboBoxItems>();

                    rsPP.MoveFirst();
                    for (int i = 0; i < rsPP.RecordCount; i++)
                    {
                        item.Add(new ComboBoxItems(rsPP.Fields[0].Value.ToString(), rsPP.Fields[0].Value.ToString()));
                        rsPP.MoveNext();
                    }
                    cbCableType.DataSource = item;
                    cbCableType.DisplayMember = "Text";
                    cbCableType.ValueMember = "Value";
                }
                cbCableType.SelectedIndex = -1;
            }

            if (cbDeveloper.DataSource == null)
            {
                ADODB.Recordset rsPP = new ADODB.Recordset();
                string sSql = "SELECT DISTINCT PICKLIST FROM REF_PREMISEEXIST order by PICKLIST";
                rsPP = m_GTDataContext.OpenRecordset(sSql, ADODB.CursorTypeEnum.adOpenStatic, ADODB.LockTypeEnum.adLockReadOnly, 1);
                cbDeveloper.Items.Clear();
                if (rsPP.RecordCount > 0)
                {
                    List<ComboBoxItems> item = new List<ComboBoxItems>();

                    rsPP.MoveFirst();
                    for (int i = 0; i < rsPP.RecordCount; i++)
                    {
                        item.Add(new ComboBoxItems(rsPP.Fields[0].Value.ToString(), rsPP.Fields[0].Value.ToString()));
                        rsPP.MoveNext();
                    }
                    cbDeveloper.DataSource = item;
                    cbDeveloper.DisplayMember = "Text";
                    cbDeveloper.ValueMember = "Value";
                }
                cbDeveloper.SelectedIndex = -1;
            }

            cbServiceDate.Checked = false;
                // hide date value since it's not set
                cbServiceDate.CustomFormat = " ";
                cbServiceDate.Format = DateTimePickerFormat.Custom;

        }
        #endregion

        #region Get Value from Database
        public string Get_Value(string sSql)
        {
            try
            {
                ADODB.Recordset rsPP = new ADODB.Recordset();
                rsPP = m_gtapp.DataContext.OpenRecordset(sSql, ADODB.CursorTypeEnum.adOpenStatic, ADODB.LockTypeEnum.adLockReadOnly, 1);
                if (rsPP.RecordCount > 0)
                {
                    rsPP.MoveFirst();
                    return (rsPP.Fields[0].Value.ToString());
                }
                return "";
            }
            catch (Exception ex)
            {
               
                MessageBox.Show(ex.Message, "SDF Placement", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return "";
            }

        }

#endregion

        #region BTN pick parent 
        private void btn_Pick_Click(object sender, EventArgs e)
        {
            this.Hide();
            m_gtapp.SelectedObjects.Clear();
            m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Point to select VDSL2. Right click to cancel selection");
            PlaceValue = 100;

            txtParantCode.Text = "";
            txtParant.Text = "";
        }

        public void PickVDSL2()
        {
            short iFNO = 0;
            int iFID = 0;

            
            if (m_gtapp.SelectedObjects.FeatureCount == 0)
            {
                MessageBox.Show("Please select a Feature", "SDF Placement", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            foreach (IGTDDCKeyObject oDDCKeyObject in m_gtapp.SelectedObjects.GetObjects())
            {

                iFNO = oDDCKeyObject.FNO;
                iFID = oDDCKeyObject.FID;

                if (iFNO == 9800)
                {
                    txtParantCode.Text = Get_Value("Select RT_CODE from gc_vdsl2 where g3e_fid = " + iFID.ToString());
                    VDSL2Code = txtParantCode.Text.Trim();
                    txtParant.Text = iFID.ToString();
                    VDSL2fid = iFID;
                    break;
                }
                else break;                

               

            }

            if (txtParant.Text == "")
            {
                MessageBox.Show("Please select a VDSL2", "SDF Placement", MessageBoxButtons.OK, MessageBoxIcon.Information);
                m_gtapp.SelectedObjects.Clear();
                return;
            }
            this.Show();
            PlaceValue = 0;
        }
        #endregion

        #region BTN Generate SDF
        private void btnGenerateSDF_Click(object sender, EventArgs e)
        {
            if (MissingAttribute())
                return;

            int SDFCount = int.Parse(txtSDFNum.Text.Trim());
            if (CreateSDFlist(SDFCount))
            {
                CreateSDFs();
            }
            tbcSDFplc.SelectedTab = tabSDFAttr;

           // SDFgroup
        }
        #endregion
        
        #region Check all attribute is filled up

        bool MissingAttribute()
        {
            
            if (txtParantCode.Text.Trim() == "" || txtParant.Text.Trim() == "")
            {
                tbcSDFplc.SelectedTab = tabSDFAttr;
                MessageBox.Show("Please select a VDSL2 first!", "SDF Placement", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return true;
            }
            int SDFCount = 0;
            if (!int.TryParse(txtSDFNum.Text.Trim(), out SDFCount))
            {
                tbcSDFplc.SelectedTab = tabSDFAttr;
                MessageBox.Show("Please enter correct value for SDF Number!", "SDF Placement", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return true;
            }
            if (SDFCount <= 0)
            {
                tbcSDFplc.SelectedTab = tabSDFAttr;
                MessageBox.Show("Please enter value greater than 0 for SDF Number!", "SDF Placement", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return true;
            }
            if (SDFCount > 10)
            {
                tbcSDFplc.SelectedTab = tabSDFAttr;
                MessageBox.Show("Please place maximum 10 SDF at once!", "SDF Placement", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return true;
            }

            if (cbGrTemplate.SelectedValue == null)
            {
                tbcSDFplc.SelectedTab = tabGranite;
                MessageBox.Show("Please filled up Granite Template!", "SDF Placement", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return true;
            }
            
            if (cbContractor.SelectedValue == null)
            {
                tbcSDFplc.SelectedTab = tabGranite;
                MessageBox.Show("Please filled up Contractor!", "SDF Placement", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return true;
            }

            if (cbManufacturer.SelectedValue == null)
            {
                tbcSDFplc.SelectedTab = tabGranite;
                MessageBox.Show("Please filled up Manufacturer!", "SDF Placement", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return true;
            }

            if (cbEquipLoc.SelectedValue == null)
            {
                tbcSDFplc.SelectedTab = tabGranite;
                MessageBox.Show("Please filled up Equip Location!", "SDF Placement", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return true;
            }
            
            if (cbCopperOwnTM.SelectedValue == null)
            {
                tbcSDFplc.SelectedTab = tabGranite;
                MessageBox.Show("Please filled up Copper Own By TM!", "SDF Placement", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return true;
            }
            
            if (cbFiberToPremise.SelectedValue == null)
            {
                tbcSDFplc.SelectedTab = tabGranite;
                MessageBox.Show("Please filled up Fiber To Premise Exist!", "SDF Placement", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return true;
            }

            return false;
        }
        #endregion
  
        #region Create SDF Temp List
        bool CreateSDFlist(int count)
        {
            
            List<SDF> tempList = new List<SDF>();
            int i = 0;

            if (count <= 10)
            {
                if (count >=1 && txt1.Text.Trim() != "")
                {
                    SDF temp = new SDF();
                    temp.number = 1;
                    temp.row = 1;
                    temp.Code = txt1.Text.Trim();
                    tempList.Add(temp);
                    i++;
                }
                if (count >= 2 && txt2.Text.Trim() != "")
                {
                    SDF temp = new SDF();
                    temp.number = 2;
                    temp.row = 1;
                    temp.Code = txt2.Text.Trim();
                    tempList.Add(temp);
                    i++;
                }
                if (count >=3 && txt3.Text.Trim() != "")
                {
                    SDF temp = new SDF();
                    temp.number = 3;
                    temp.row = 1;
                    temp.Code = txt3.Text.Trim();
                    tempList.Add(temp);
                    i++;
                }
                if (count >=4 && txt4.Text.Trim() != "")
                {
                    SDF temp = new SDF();
                    temp.number = 4;
                    temp.row = 1;
                    temp.Code = txt4.Text.Trim();
                    tempList.Add(temp);
                    i++;
                }
                if (count >=5 && txt5.Text.Trim() != "")
                {
                    SDF temp = new SDF();
                    temp.number = 5;
                    temp.row = 1;
                    temp.Code = txt5.Text.Trim();
                    tempList.Add(temp);
                    i++;
                }
                if (count >=6 && txt6.Text.Trim() != "")
                {
                    SDF temp = new SDF();
                    temp.number = 6;
                    temp.row = 1;
                    temp.Code = txt6.Text.Trim();
                    tempList.Add(temp);
                    i++;
                }
                if (count >=7 && txt7.Text.Trim() != "")
                {
                    SDF temp = new SDF();
                    temp.number = 7;
                    temp.row = 1;
                    temp.Code = txt7.Text.Trim();
                    tempList.Add(temp);
                    i++;
                }
                if (count >=8 && txt8.Text.Trim() != "")
                {
                    SDF temp = new SDF();
                    temp.number = 8;
                    temp.row = 1;
                    temp.Code = txt8.Text.Trim();
                    tempList.Add(temp);
                    i++;
                }
                if (count >=9 && txt9.Text.Trim() != "")
                {
                    SDF temp = new SDF();
                    temp.number = 9;
                    temp.row = 1;
                    temp.Code = txt9.Text.Trim();
                    tempList.Add(temp);
                    i++;
                }
                if (count >=10 && txt10.Text.Trim() != "")
                {
                    SDF temp = new SDF();
                    temp.number = 10;
                    temp.row = 1;
                    temp.Code = txt10.Text.Trim();
                    tempList.Add(temp);
                    i++;
                }

                if (count != i)
                {
                    MessageBox.Show("Please fill up all Codes!", "SDF Placement", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return false;
                }
                if (SDFgroup == null)
                    SDFgroup = new List<SDF>();
                else
                {
                    SDFgroup.Clear();
                }

                //validate code on repeating inside list
                foreach (SDF elem1 in tempList)
                {
                    foreach (SDF elem2 in tempList)
                    {
                        if (elem1.Code == elem2.Code && elem1.number != elem2.number)
                        {
                            MessageBox.Show("SDF Code can not be duplicated\nSDF No " + elem1.number.ToString()
                                            + " and SDF No " + elem2.number.ToString(), "SDF Placement", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return false;

                        }
                    }
                }

                //validate code on already exist code in database
                foreach (SDF elem in tempList)
                {
                    if (!ValidationCode(elem.Code, elem.number))
                    {
                        return false;
                    }
                }

                //ValidationCode(string code_to_check, int sdf_num)
                foreach( SDF elem in tempList)
                {
                    SDFgroup.Add(elem);
                }
                
                return true;

            }
            else {
                MessageBox.Show("Place maximum 10 SDF at once!", "SDF Placement", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    
                return false; }
        }

        #endregion

        #region Create SDFs Features

        void CreateSDFs()
        {
            try
            {
                progressBar1.Visible = true;
                progressBar1.Value = 10;
                tbcSDFplc.Enabled = false;
                btnGenerateSDF.Enabled = false;
               
                
                int step = 60 / SDFgroup.Count;
               

                GTSDFplacement.m_oIGTTransactionManager.Begin("CreateSDF");
                DetailWindowCoor DetWin=GetDetailWindowForVDSL2();
                progressBar1.Value = 20;
                foreach (SDF elem in SDFgroup)
                {
                    IGTPoint PointDP = GTClassFactory.Create<IGTPoint>();
                    PointDP.Z = 0.0;
                    PointDP.X = DetWin.X + 15 ;
                    PointDP.Y = DetWin.Y + 15 * (elem.number);
                    //PointDP.X = nMinX + 50;
                    //PointDP.Y = nMinY + 1 + 14;
                    CreateSDFfeature(PointDP, DetWin.detailID, elem.Code);
                    progressBar1.Value += step;
                }


                GTSDFplacement.m_oIGTTransactionManager.Commit();
                progressBar1.Value = 90;
                GTSDFplacement.m_oIGTTransactionManager.RefreshDatabaseChanges();
                progressBar1.Value = 100;
                MessageBox.Show("SDF were successfully placed!", "SDF Placement", MessageBoxButtons.OK, MessageBoxIcon.Information);


                //int test = DetWin.detailID;
                ////Detail Engineering
                //IGTMapWindow DetailWindowToOpen = m_gtapp.OpenDetailWindow("Detail Engineering", test);
                //DetailWindowToOpen.FitAll();
                //m_gtapp.RefreshWindows();


                //ToWindow = application.ActiveMapWindow;
                //FromWindow = application.NewMapWindow(application.ActiveMapWindow.LegendName);
                //application.ArrangeWindows(GTWindowActionConstants.gtapwaTileHorizontal);
                //FromWindow.Activate();

                //mobjEditServiceFrom.TargetMapWindow = FromWindow;
                //mobjEditServiceTo.TargetMapWindow = ToWindow;
                //FromWindow.Caption = "From " + from_sourcename;
                //ToWindow.Caption = "To " + to_sourcename;

                progressBar1.Visible = false;
                tbcSDFplc.Enabled = true;
                btnGenerateSDF.Enabled = true;

            }
            catch (Exception ex)
            {
                if (GTSDFplacement.m_oIGTTransactionManager.TransactionInProgress)
                {
                    GTSDFplacement.m_oIGTTransactionManager.Rollback();
                    MessageBox.Show(ex.Message, "SDF Placement", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                progressBar1.Visible = false;
                tbcSDFplc.Enabled = true;
                btnGenerateSDF.Enabled = true;
            }
        }

        IGTKeyObject CreateSDFfeature(IGTPoint Point1, int p_DetailID, string SDFCode)
        {
           
            short iFNO;
            short iCNO;
            int iFID;
            double dRotation;
            IGTKeyObject oNewFeature;


            IGTOrientedPointGeometry oOrPointGeom;
            //IGTPoint oPointGeom;
            IGTTextPointGeometry oOrTextGeom;
            iFNO = 9200;

            oNewFeature = GTClassFactory.Create<IGTApplication>().DataContext.NewFeature(iFNO);
            iFID = oNewFeature.FID;
            // netelem component for common detail window
            iCNO = 51;
            if (oNewFeature.Components.GetComponent(iCNO).Recordset.EOF)
            {
                oNewFeature.Components.GetComponent(iCNO).Recordset.AddNew("G3E_FID", iFID);
                oNewFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_FNO", iFNO);
                oNewFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_CID", 1);
                oNewFeature.Components.GetComponent(iCNO).Recordset.Update("MIN_MATERIAL", "0");
            }
            else
            {
                oNewFeature.Components.GetComponent(iCNO).Recordset.MoveLast();
                oNewFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_FNO", iFNO);
                oNewFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_CID", 1);
                oNewFeature.Components.GetComponent(iCNO).Recordset.Update("MIN_MATERIAL", "0");
            }


            iCNO = 9201;
            if (oNewFeature.Components.GetComponent(iCNO).Recordset.EOF)
            {
                oNewFeature.Components.GetComponent(iCNO).Recordset.AddNew("G3E_FID", iFID);
                oNewFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_FNO", iFNO);
                oNewFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_CID", 1);
                oNewFeature.Components.GetComponent(iCNO).Recordset.Update("SDF_CODE", SDFCode);
                oNewFeature.Components.GetComponent(iCNO).Recordset.Update("VDSL2_CODE", VDSL2Code);
                oNewFeature.Components.GetComponent(iCNO).Recordset.Update("VDSL2_FID", VDSL2fid);
                oNewFeature.Components.GetComponent(iCNO).Recordset.Update("CONTRACTOR", cbManufacturer.SelectedValue);
                oNewFeature.Components.GetComponent(iCNO).Recordset.Update("MANUFACTURER", cbContractor.SelectedValue);
            }
            else
            {
                oNewFeature.Components.GetComponent(iCNO).Recordset.MoveLast();
                oNewFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_FNO", iFNO);
                oNewFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_CID", 1);
                oNewFeature.Components.GetComponent(iCNO).Recordset.Update("SDF_CODE", SDFCode);
                oNewFeature.Components.GetComponent(iCNO).Recordset.Update("VDSL2_CODE", VDSL2Code);
                oNewFeature.Components.GetComponent(iCNO).Recordset.Update("VDSL2_FID", VDSL2fid);
                oNewFeature.Components.GetComponent(iCNO).Recordset.Update("CONTRACTOR", cbManufacturer.SelectedValue);
                oNewFeature.Components.GetComponent(iCNO).Recordset.Update("MANUFACTURER", cbContractor.SelectedValue);

            }

            iCNO = 91;
            if (oNewFeature.Components.GetComponent(iCNO).Recordset.EOF)
            {
                oNewFeature.Components.GetComponent(iCNO).Recordset.AddNew("G3E_FID", iFID);
                oNewFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_FNO", iFNO);
                oNewFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_CID", 1);
                oNewFeature.Components.GetComponent(iCNO).Recordset.Update("GRANITE_TEMPLATE", cbGrTemplate.SelectedValue);
                oNewFeature.Components.GetComponent(iCNO).Recordset.Update("ACCESS_RESTRICTION", cbAccessRestr.SelectedValue);
                oNewFeature.Components.GetComponent(iCNO).Recordset.Update("EQUIP_LOCATION", cbEquipLoc.SelectedValue);
                oNewFeature.Components.GetComponent(iCNO).Recordset.Update("COPPER_OWNBYTM", cbCopperOwnTM.SelectedValue);
                oNewFeature.Components.GetComponent(iCNO).Recordset.Update("GRANITE_FIBERTOPREMISEEXIST", cbFiberToPremise.SelectedValue);
                oNewFeature.Components.GetComponent(iCNO).Recordset.Update("CABLINGTYPE", cbCableType.SelectedValue);
                oNewFeature.Components.GetComponent(iCNO).Recordset.Update("GRANITE_CONTACT", txtContact.Text.Trim());
                oNewFeature.Components.GetComponent(iCNO).Recordset.Update("GRANITE_COMMENT", txtComment.Text.Trim());
                oNewFeature.Components.GetComponent(iCNO).Recordset.Update("SMART_DEVELOPER", cbDeveloper.SelectedValue);
                oNewFeature.Components.GetComponent(iCNO).Recordset.Update("SMART_PROJECTID", txtProjectID.Text.Trim());
                if (cbServiceDate.Checked)
                {                    
                    oNewFeature.Components.GetComponent(iCNO).Recordset.Update("INSERVICE_DATE", cbServiceDate.Value);
                }    
            }
            else
            {
                oNewFeature.Components.GetComponent(iCNO).Recordset.MoveLast();
                oNewFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_FNO", iFNO);
                oNewFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_CID", 1);
                oNewFeature.Components.GetComponent(iCNO).Recordset.Update("GRANITE_TEMPLATE", cbGrTemplate.SelectedValue);
                oNewFeature.Components.GetComponent(iCNO).Recordset.Update("ACCESS_RESTRICTION", cbAccessRestr.SelectedValue);
                oNewFeature.Components.GetComponent(iCNO).Recordset.Update("EQUIP_LOCATION", cbEquipLoc.SelectedValue);
                oNewFeature.Components.GetComponent(iCNO).Recordset.Update("COPPER_OWNBYTM", cbCopperOwnTM.SelectedValue);
                oNewFeature.Components.GetComponent(iCNO).Recordset.Update("GRANITE_FIBERTOPREMISEEXIST", cbFiberToPremise.SelectedValue);
                oNewFeature.Components.GetComponent(iCNO).Recordset.Update("CABLINGTYPE", cbCableType.SelectedValue);
                oNewFeature.Components.GetComponent(iCNO).Recordset.Update("GRANITE_CONTACT", txtContact.Text.Trim());
                oNewFeature.Components.GetComponent(iCNO).Recordset.Update("GRANITE_COMMENT", txtComment.Text.Trim());
                oNewFeature.Components.GetComponent(iCNO).Recordset.Update("SMART_DEVELOPER", cbDeveloper.SelectedValue);
                oNewFeature.Components.GetComponent(iCNO).Recordset.Update("SMART_PROJECTID", txtProjectID.Text.Trim());
                if (cbServiceDate.Checked)
                {
                    oNewFeature.Components.GetComponent(iCNO).Recordset.Update("INSERVICE_DATE", cbServiceDate.Value);
                }  
            }

            iCNO = 9221;
            if (oNewFeature.Components.GetComponent(iCNO).Recordset.EOF)
            {
                oNewFeature.Components.GetComponent(iCNO).Recordset.AddNew("G3E_FID", iFID);
                oNewFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_FNO", iFNO);
                oNewFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_DETAILID", p_DetailID);
            }
            else
            {
                oNewFeature.Components.GetComponent(iCNO).Recordset.MoveLast();
                oNewFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_DETAILID", p_DetailID);
            }


            oOrPointGeom = GTClassFactory.Create<IGTOrientedPointGeometry>();
            dRotation = 0;
            oOrPointGeom.Origin = Point1;
            //  Radians
            //oOrPointGeom.Orientation = new IGTVector(Math.Cos(dRotation), Math.Sin(dRotation), 0.0);
            oOrPointGeom.Orientation = GTClassFactory.Create<IGTVector>();
            oOrPointGeom.Orientation.I = Math.Cos(dRotation);
            oOrPointGeom.Orientation.J = Math.Sin(dRotation);
            oOrPointGeom.Orientation.K = 0.0;
            oNewFeature.Components.GetComponent(iCNO).Geometry = oOrPointGeom;

            iCNO = 9231;
            if (oNewFeature.Components.GetComponent(iCNO).Recordset.EOF)
            {
                oNewFeature.Components.GetComponent(iCNO).Recordset.AddNew("G3E_FID", iFID);
                oNewFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_FNO", iFNO);
                oNewFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_DETAILID", p_DetailID);
            }
            else
            {
                oNewFeature.Components.GetComponent(iCNO).Recordset.MoveLast();
                oNewFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_DETAILID", p_DetailID);
            }


            oOrTextGeom = GTClassFactory.Create<IGTTextPointGeometry>();
            oOrTextGeom.Origin = Point1;
            oOrTextGeom.Rotation = 0;
            oNewFeature.Components.GetComponent(iCNO).Geometry = oOrTextGeom;

            return oNewFeature;
        }
        #endregion

        #region Get Detail Window For VDSL2
        private DetailWindowCoor GetDetailWindowForVDSL2()
        {
            DetailWindowCoor DetWinVDSL2 = new DetailWindowCoor();
                         
                string g3e_detailid = Get_Value("select g3e_detailid from gc_detail where g3e_fid=" + VDSL2fid);
               
                if(g3e_detailid != "" )
                {
                string sSql = "SELECT coor.X as X, coor.Y as Y   FROM dgc_vdsl2_s s,   TABLE(SDO_UTIL.GETVERTICES(s.g3e_geometry)) coor where s.g3e_detailid=" + g3e_detailid +
                              " union all " +
                              "SELECT coor.X as X, coor.Y as Y   FROM dgc_dp_s s,   TABLE(SDO_UTIL.GETVERTICES(s.g3e_geometry)) coor where s.g3e_detailid=" + g3e_detailid +
                              " union all " +
                              "SELECT coor.X as X, coor.Y as Y   FROM dgc_sdf_s s,   TABLE(SDO_UTIL.GETVERTICES(s.g3e_geometry)) coor where s.g3e_detailid=" + g3e_detailid ;
                           
                ADODB.Recordset rsComp = new ADODB.Recordset();
                rsComp = GTClassFactory.Create<IGTApplication>().DataContext.OpenRecordset(sSql, ADODB.CursorTypeEnum.adOpenStatic, ADODB.LockTypeEnum.adLockReadOnly, 1);
                double nMinX = 0;
                double nMaxY = 0;
                double coortemp = 0;
                if (rsComp != null)
                {
                    if (rsComp.RecordCount >= 1)
                    {
                        rsComp.MoveFirst();
                        for (int h = 0; h < rsComp.RecordCount; h++)
                        {
                            if (rsComp.Fields["X"].Value != DBNull.Value)
                                coortemp = Convert.ToInt32(rsComp.Fields["X"].Value);
                            else
                                coortemp = 0;

                            if (nMinX > coortemp || nMinX==0)
                                nMinX = coortemp;

                            if (rsComp.Fields["Y"].Value != DBNull.Value)
                                coortemp = Convert.ToInt32(rsComp.Fields["Y"].Value);
                            else
                                coortemp = 0;

                            if (nMaxY < coortemp)
                                nMaxY = coortemp;

                            rsComp.MoveNext();
                        }
                    }
                }
                DetWinVDSL2.detailID = int.Parse(g3e_detailid);
                DetWinVDSL2.X = nMinX;
                DetWinVDSL2.Y = nMaxY;
                return DetWinVDSL2;
                }


                if (g3e_detailid == "")
                    g3e_detailid = Get_Value("select max(g3e_detailid)+1 from gc_detail");
                if (g3e_detailid == "")
                    g3e_detailid = "1";

            ////BGI VDSL2 FTTO V1084
                string detwindowname = Get_Value("select net.exc_abb || ' VDSL2 ' || vdsl2.rt_type || ' ' || vdsl2.rt_code from GC_VDSL2 vdsl2, gc_netelem net where vdsl2.g3e_fid=net.g3e_fid and vdsl2.g3e_fid=" + VDSL2fid);

                IGTKeyObject oVDSL2 = GTClassFactory.Create<IGTApplication>().DataContext.OpenFeature(9800, VDSL2fid);
                double Vdsl2X = oVDSL2.Components.GetComponent(9820).Geometry.FirstPoint.X;
                 double Vdsl2Y = oVDSL2.Components.GetComponent(9820).Geometry.FirstPoint.Y;

                short iCNO = 60;
                if (oVDSL2.Components.GetComponent(iCNO).Recordset.EOF)
                {
                    oVDSL2.Components.GetComponent(iCNO).Recordset.AddNew("G3E_FID", VDSL2fid);
                    oVDSL2.Components.GetComponent(iCNO).Recordset.Update("G3E_FNO", 9800);
                    oVDSL2.Components.GetComponent(iCNO).Recordset.Update("G3E_CID", 1);
                    oVDSL2.Components.GetComponent(iCNO).Recordset.Update("DETAIL_MBRXHI", Vdsl2X + 5);
                    oVDSL2.Components.GetComponent(iCNO).Recordset.Update("DETAIL_MBRXLO", Vdsl2X - 5);
                    oVDSL2.Components.GetComponent(iCNO).Recordset.Update("DETAIL_MBRYHI", Vdsl2Y + 5);
                    oVDSL2.Components.GetComponent(iCNO).Recordset.Update("DETAIL_MBRYLO", Vdsl2Y - 5);
                    oVDSL2.Components.GetComponent(iCNO).Recordset.Update("DETAIL_USERNAME", detwindowname); 
                    oVDSL2.Components.GetComponent(iCNO).Recordset.Update("G3E_DETAILID", int.Parse(g3e_detailid));
                    oVDSL2.Components.GetComponent(iCNO).Recordset.Update("DETAIL_LEGENDNUMBER", 2);
                    oVDSL2.Components.GetComponent(iCNO).Recordset.Update("DETAIL_MBRXOFFSET", 10);
                    oVDSL2.Components.GetComponent(iCNO).Recordset.Update("DETAIL_MBRYOFFSET", 10);
                    //oNewFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_ALIGNMENT", Get_Value("select G3E_ALIGNMENT from gc_adm_bldg_s where g3e_fid=" + iFID));//"");//
                }
                else
                {
                    oVDSL2.Components.GetComponent(iCNO).Recordset.MoveLast();
                    //oVDSL2.Components.GetComponent(iCNO).Recordset.Update("G3E_FNO", 9800);
                    // oVDSL2.Components.GetComponent(iCNO).Recordset.Update("G3E_CID", 1);
                    oVDSL2.Components.GetComponent(iCNO).Recordset.Update("DETAIL_MBRXHI", Vdsl2X + 5);
                    oVDSL2.Components.GetComponent(iCNO).Recordset.Update("DETAIL_MBRXLO", Vdsl2X - 5);
                    oVDSL2.Components.GetComponent(iCNO).Recordset.Update("DETAIL_MBRYHI", Vdsl2Y + 5);
                    oVDSL2.Components.GetComponent(iCNO).Recordset.Update("DETAIL_MBRYLO", Vdsl2Y - 5);
                    oVDSL2.Components.GetComponent(iCNO).Recordset.Update("DETAIL_USERNAME", detwindowname); oVDSL2.Components.GetComponent(iCNO).Recordset.Update("G3E_DETAILID", int.Parse(g3e_detailid));
                    oVDSL2.Components.GetComponent(iCNO).Recordset.Update("DETAIL_LEGENDNUMBER", 2);
                    oVDSL2.Components.GetComponent(iCNO).Recordset.Update("DETAIL_MBRXOFFSET", 10);
                    oVDSL2.Components.GetComponent(iCNO).Recordset.Update("DETAIL_MBRYOFFSET", 10);
                    //oNewFeature.Components.GetComponent(iCNO).Recordset.Update("G3E_ALIGNMENT", Get_Value("select G3E_ALIGNMENT from gc_adm_bldg_s where g3e_fid=" + iFID));//"");//
                }

             //get cooordinate for detail symbol
                IGTPoint Point1 = GTClassFactory.Create<IGTPoint>();
                Point1.X = Vdsl2X;
                Point1.Y = Vdsl2Y;
                Point1.Z = 0.0;

                    //GC_DETAILIND_S	61
                    iCNO = 61;
                    int cid = 0;
                    string cidst = Get_Value("select max(g3e_cid) from GC_DETAILIND_S where g3e_fno=9800 and g3e_fid=" + VDSL2fid);
                    if (cidst == "")
                        cid = 1;
                    else cid = cid + 1;

                    if (oVDSL2.Components.GetComponent(iCNO).Recordset.EOF)
                    {
                        oVDSL2.Components.GetComponent(iCNO).Recordset.AddNew("G3E_FID", VDSL2fid);
                        oVDSL2.Components.GetComponent(iCNO).Recordset.Update("G3E_FNO", 9800);
                        oVDSL2.Components.GetComponent(iCNO).Recordset.Update("G3E_CID", cid);
                    }
                    else
                    {
                        oVDSL2.Components.GetComponent(iCNO).Recordset.MoveLast();
                        oVDSL2.Components.GetComponent(iCNO).Recordset.Update("G3E_FNO", 9800);
                        oVDSL2.Components.GetComponent(iCNO).Recordset.Update("G3E_CID", cid);
                    }



                    IGTOrientedPointGeometry oOrPointGeom = GTClassFactory.Create<IGTOrientedPointGeometry>();
                    oOrPointGeom.Origin = Point1;
                    //  Radians
                    //oOrPointGeom.Orientation = new IGTVector(Math.Cos(dRotation), Math.Sin(dRotation), 0.0);
                    oOrPointGeom.Orientation = GTClassFactory.Create<IGTVector>();
                    oOrPointGeom.Orientation.I = Math.Cos(0);
                    oOrPointGeom.Orientation.J = Math.Sin(0);
                    oOrPointGeom.Orientation.K = 0.0;
                    oVDSL2.Components.GetComponent(iCNO).Geometry = oOrPointGeom;

                iCNO = 9821;
                 cid = 0;                
               // string sSql = "select max g3e_cid from dgc_vdsl2_s where g3e_fno=9800 and g3e_fid=" + VDSL2fid;
                 cidst = Get_Value("select max(g3e_cid) from dgc_vdsl2_s where g3e_fno=9800 and g3e_fid=" + VDSL2fid);
                if (cidst == "")
                    cid = 1;
                else cid = cid + 1;

                if (oVDSL2.Components.GetComponent(iCNO).Recordset.EOF)
                    {
                        oVDSL2.Components.GetComponent(iCNO).Recordset.AddNew("G3E_FID", VDSL2fid);
                        oVDSL2.Components.GetComponent(iCNO).Recordset.Update("G3E_FNO", 9800);
                        oVDSL2.Components.GetComponent(iCNO).Recordset.Update("G3E_CID", cid);
                        oVDSL2.Components.GetComponent(iCNO).Recordset.Update("G3E_DETAILID", int.Parse(g3e_detailid));
                    }
                    else
                    {
                        oVDSL2.Components.GetComponent(iCNO).Recordset.MoveLast();
                        oVDSL2.Components.GetComponent(iCNO).Recordset.Update("G3E_FNO", 9800);
                        oVDSL2.Components.GetComponent(iCNO).Recordset.Update("G3E_CID", cid);
                        oVDSL2.Components.GetComponent(iCNO).Recordset.Update("G3E_DETAILID", int.Parse(g3e_detailid));
                    }



                     oOrPointGeom = GTClassFactory.Create<IGTOrientedPointGeometry>();
                    oOrPointGeom.Origin = Point1;
                    //  Radians
                    //oOrPointGeom.Orientation = new IGTVector(Math.Cos(dRotation), Math.Sin(dRotation), 0.0);
                    oOrPointGeom.Orientation = GTClassFactory.Create<IGTVector>();
                    oOrPointGeom.Orientation.I = Math.Cos(0);
                    oOrPointGeom.Orientation.J = Math.Sin(0);
                    oOrPointGeom.Orientation.K = 0.0;
                    oVDSL2.Components.GetComponent(iCNO).Geometry = oOrPointGeom;
                

                    //get cooordinate for detail label
                    if (!oVDSL2.Components.GetComponent(9830).Recordset.EOF)
                    {
                        IGTPoint Point2 = GTClassFactory.Create<IGTPoint>();
                        Point2.X = oVDSL2.Components.GetComponent(9830).Geometry.FirstPoint.X;
                        Point2.Y = oVDSL2.Components.GetComponent(9830).Geometry.FirstPoint.Y;
                        Point2.Z = 0.0;
                        iCNO = 9831;
                      
                         cid = 0;
                         cidst = Get_Value("select max(g3e_cid) from DGC_VDSL2_T where g3e_fno=9800 and g3e_fid=" + VDSL2fid);
                        if (cidst == "")
                            cid = 1;
                        else cid = cid + 1;
                        if (oVDSL2.Components.GetComponent(iCNO).Recordset.EOF)
                        {
                            oVDSL2.Components.GetComponent(iCNO).Recordset.AddNew("G3E_FID", VDSL2fid);
                            oVDSL2.Components.GetComponent(iCNO).Recordset.Update("G3E_FNO", 9800);
                            oVDSL2.Components.GetComponent(iCNO).Recordset.Update("G3E_CID", cid);
                            oVDSL2.Components.GetComponent(iCNO).Recordset.Update("G3E_DETAILID", int.Parse(g3e_detailid));
                        }
                        else
                        {
                            oVDSL2.Components.GetComponent(iCNO).Recordset.MoveLast();
                            oVDSL2.Components.GetComponent(iCNO).Recordset.Update("G3E_FNO", 9800);
                            oVDSL2.Components.GetComponent(iCNO).Recordset.Update("G3E_CID", cid);
                            oVDSL2.Components.GetComponent(iCNO).Recordset.Update("G3E_DETAILID", int.Parse(g3e_detailid));
                        }


                        IGTTextPointGeometry oOrTextGeom = GTClassFactory.Create<IGTTextPointGeometry>();
                        oOrTextGeom.Origin = Point2;
                        oOrTextGeom.Rotation = 0;
                        oVDSL2.Components.GetComponent(iCNO).Geometry = oOrTextGeom;
                    }



                DetWinVDSL2.detailID = int.Parse(g3e_detailid);
                DetWinVDSL2.X = oVDSL2.Components.GetComponent(9820).Geometry.FirstPoint.X;
                DetWinVDSL2.Y = oVDSL2.Components.GetComponent(9820).Geometry.FirstPoint.Y;
                return DetWinVDSL2;
           
        }
        #endregion

        #region BTN Cancel
        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        #endregion

        #region BTN Next Group of SDF
        private void btnNextGroup_Click(object sender, EventArgs e)
        {

        }

        #endregion

        #region closing Form
        private void SDF_Plc_Form_FormClosing(object sender, FormClosingEventArgs e)
        {

        }
         
        #endregion

        #region Date Pick check/unchecked
        private void cbServiceDate_ValueChanged(object sender, EventArgs e)
        {
            //  cbServiceDate.Checked

            if (!cbServiceDate.Checked)
            {
                // hide date value since it's not set
                cbServiceDate.CustomFormat = " ";
                cbServiceDate.Format = DateTimePickerFormat.Custom;
            }
            else
            {
                cbServiceDate.CustomFormat = null;
                cbServiceDate.Format = DateTimePickerFormat.Short;
            }

        }
        #endregion

        #region validate code
        private bool ValidationCode(string code_to_check, int sdf_num)
        {
            if (code_to_check == "")
            {
                MessageBox.Show("SDF Code can not be empty!\nSDF No "+sdf_num.ToString()+"", "SDF Placement", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            ADODB.Recordset rsPP = new ADODB.Recordset();
            string Exch = Get_Value("Select EXC_ABB from G3E_JOB where G3E_IDENTIFIER = '" + m_gtapp.DataContext.ActiveJob.ToString() + "'");

            string sSql = "select SDF_CODE from GC_SDF A, GC_NETELEM B where A.G3E_FID = B.G3E_FID and B.EXC_ABB = '" + Exch + "' and SDF_CODE = '" + code_to_check + "' and VDSL2_CODE = '" + VDSL2Code + "'";
                
                rsPP = m_GTDataContext.OpenRecordset(sSql, ADODB.CursorTypeEnum.adOpenDynamic, ADODB.LockTypeEnum.adLockOptimistic, (int)ADODB.CommandTypeEnum.adCmdText);
                if (rsPP.RecordCount > 0)
                {
                    MessageBox.Show("SDF Code already exists\nSDF No " + sdf_num.ToString() + "", "SDF Placement", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;

                }

            

            return true;
        }

        #endregion

        #region Autopopulate Manuf and Contractor from template

        private void cbGrTemplate_SelectedValueChanged(object sender, EventArgs e)
        {
            if (cbGrTemplate.SelectedValue != null)
                FilteringAttributes(cbGrTemplate.SelectedValue.ToString(), "","");
        }

        private void cbManufacturer_SelectedValueChanged(object sender, EventArgs e)
        {
            if (cbManufacturer.SelectedValue != null)
                FilteringAttributes("","",cbManufacturer.SelectedValue.ToString());
        }

        private void cbContractor_SelectedValueChanged(object sender, EventArgs e)
        {
            if (cbContractor.SelectedValue != null)
                FilteringAttributes("",cbContractor.SelectedValue.ToString(), "");
        }

         #region filtering

        public void FilteringAttributes(string template, string contractor, string manufacturer)
        {
            ADODB.Recordset rsPP = new ADODB.Recordset();
            string sSql = "";

            #region template
            if (template != "")
            {
             sSql="select GRANITE_TEMPLATE, MANUFACTURER, CONTRACTOR from REF_GRANITE_TEMPLATESDF "+
                        " where GRANITE_TEMPLATE='" + template+"'";
            rsPP = m_GTDataContext.OpenRecordset(sSql, ADODB.CursorTypeEnum.adOpenStatic, ADODB.LockTypeEnum.adLockReadOnly, 1);

             if (rsPP.RecordCount <= 0)
             {
                 return;
             }

             rsPP.MoveFirst();
                
            if (cbManufacturer.DataSource != null)
            {
                int i = 0;
                for (; i < cbManufacturer.Items.Count; i++)
                {
                    cbManufacturer.SelectedItem = cbManufacturer.Items[i];
                    if (cbManufacturer.SelectedValue.ToString().ToUpper().Contains(rsPP.Fields[1].Value.ToString()))
                        break;
                }
                if (i == cbManufacturer.Items.Count)
                    cbManufacturer.SelectedIndex = -1;
            }           
                

               if (cbContractor.DataSource != null)
            {
                int i = 0;
                for (; i < cbContractor.Items.Count; i++)
                {
                    cbContractor.SelectedItem = cbContractor.Items[i];
                    if (cbContractor.SelectedValue.ToString().ToUpper().Contains(rsPP.Fields[2].Value.ToString()))
                        break;
                }
                if (i == cbContractor.Items.Count)
                    cbContractor.SelectedIndex = -1;
            }

        }
            #endregion

        #region manuf or contractor
        if (contractor != "" || manufacturer != "")
        {
            //sSql = "SELECT GRANITE_TEMPLATE FROM REF_GRANITE_TEMPLATESDF where 1=1 ";
            //if (cbContractor.SelectedValue != null)
            //    sSql += " and CONTRACTOR='" + cbContractor.SelectedValue.ToString() + "'";

            //if (cbManufacturer.SelectedValue != null)
            //    sSql += " and MANUFACTURER='" + cbManufacturer.SelectedValue.ToString() + "'";
            
                
            //    rsPP = m_GTDataContext.OpenRecordset(sSql, ADODB.CursorTypeEnum.adOpenStatic, ADODB.LockTypeEnum.adLockReadOnly, 1);
               
            //    if (rsPP.RecordCount > 0)
            //    {
            //        cbGrTemplate.DataSource = null;
            //        cbGrTemplate.Items.Clear();
            //        List<ComboBoxItems> item = new List<ComboBoxItems>();

            //        rsPP.MoveFirst();
            //        for (int i = 0; i < rsPP.RecordCount; i++)
            //        {
            //            item.Add(new ComboBoxItems(rsPP.Fields[0].Value.ToString(), rsPP.Fields[0].Value.ToString()));
            //            rsPP.MoveNext();
            //        }
            //        cbGrTemplate.DataSource = item;
            //        cbGrTemplate.DisplayMember = "Text";
            //        cbGrTemplate.ValueMember = "Value";
            //    }
            //    cbGrTemplate.SelectedIndex = -1;
            
        }
        #endregion

        #region contractor
        #endregion

    }

        #endregion

        
        
        #endregion

    }

       
}