using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Intergraph.GTechnology.API;
using ADODB;
using Intergraph.GTechnology.Diagnostics;
using NEPS.GTechnology.NEPSSpliceOwnership;

namespace NEPS.AssignPlantUnit
{
    public partial class AssignmentForm : Form
    {
        public IGTDataContext dc;
        public IGTComponents componentList;
        public int fspliceFID;
        public Intergraph.GTechnology.API.IGTApplication m_gtapp = null;
        private int CloseStatus = 0;
        MyUserSettings settings;

        public const int CNO_GCNetElem = 51;
        public const int CNO_FSplice = 11801;
        public const int CNO_Ownedby = 64;

        GTDiagnostics m_Diag = new GTDiagnostics(GTDiagSS.IDotNetCustomCmd, GTDiagMaskWord.IDotNetCustomCmd, "AssignmentForm.cs"); 


        public AssignmentForm()
        {
            InitializeComponent();
        }

        private void AssignmentForm_Load(object sender, EventArgs e)
        {
            if (m_Diag.IsEnabled(GTDiagCat.EE)) m_Diag.LogEnter("AssignmentForm_Load"); 

            settings = new MyUserSettings();

            // fill manufacturers
            FillReferenceItems("ref_fsplice_manufacturer", "manufacturer", cbManufacturers);

            // fill Contractors
            FillReferenceItems("REF_FIB_CBLCONTR", "PL_VALUE", cbContractors);
            
            // fill min materials
            FillReferenceItems("REF_FSPLICE", "MIN_MATERIAL", cbMinMaterials);

            FillValuesFromDatabase();

            if (m_Diag.IsEnabled(GTDiagCat.EE)) m_Diag.LogExit("AssignmentForm_Load");

            if (m_gtapp == null) m_gtapp = GTClassFactory.Create<IGTApplication>();
                
        }

        private void FillReferenceItems(string tablename, string fieldname, ComboBox destination)
        {

            if (m_Diag.IsEnabled(GTDiagCat.EE)) m_Diag.LogEnter("FillReferenceItems");

            string sql = "SELECT * FROM " + tablename;
            int count = 0;
            Recordset rs = dc.Execute(sql, out count, 0, null);
            destination.Items.Clear();
            if (!rs.EOF)
            {
                rs.MoveFirst();
                while (!rs.EOF)
                {
                    string fieldValue = rs.Fields[fieldname].Value.ToString();
                    fieldValue = fieldValue.Trim();
                    destination.Items.Add(fieldValue);
                    rs.MoveNext();
                }
                if (destination.Items.Count > 0)
                    destination.SelectedIndex = 0;

            }

            if (m_Diag.IsEnabled(GTDiagCat.EE)) m_Diag.LogExit("FillReferenceItems");
        }

        /// <summary>
        /// Fill values from database. If empty string is specified in the database, use what is stored in the settings (previous values).
        /// </summary>
        private void FillValuesFromDatabase()
        {
            if (m_Diag.IsEnabled(GTDiagCat.EE)) m_Diag.LogEnter("FillValuesFromDatabase");

            // GC_NETELEM TABLE
            IGTComponent comp = componentList.GetComponent(CNO_GCNetElem);
            Recordset rs = comp.Recordset;
            rs.MoveFirst();

            // OWNERSHIP
            try
            {
                if (IsNullOrEmptyString(rs.Fields["OWNERSHIP"]))
                    txtOwnership.Text = settings.Ownership;
                txtOwnership.Text = rs.Fields["OWNERSHIP"].Value.ToString();
                if (txtOwnership.Text == "")
                    txtOwnership.Text = settings.Ownership;

            }
            catch (Exception ex)
            {
                txtOwnership.Text = settings.Ownership;
            }

            // YEAR_PLACED
            try
            {
                if (IsNullOrEmptyString(rs.Fields["YEAR_PLACED"]))
                    txtYearPlaced.Text = DateTime.Today.Year.ToString();
                txtYearPlaced.Text = rs.Fields["YEAR_PLACED"].Value.ToString();
                if (txtYearPlaced.Text == "")
                    txtYearPlaced.Text = DateTime.Today.Year.ToString();
            }
            catch (Exception ex)
            {
                txtYearPlaced.Text = DateTime.Today.Year.ToString();
            }
         
         
            // FSPLICE TABLE
            comp = componentList.GetComponent(CNO_FSplice);
            rs = comp.Recordset;
            rs.MoveFirst();
            
            

            // MANUFACTURER
            try
            {
                if (IsNullOrEmptyString(rs.Fields["MANUFACTURER"]))
                    SelectItem(cbManufacturers, settings.Manufacturer, true);
                else
                    SelectItem(cbManufacturers, rs.Fields["MANUFACTURER"].Value, true);
            }
            catch (Exception ex)
            {

            }

            // CONTRACTOR
            try
            {
                if (IsNullOrEmptyString(rs.Fields["CONTRACTOR"]))
                    SelectItem(cbContractors, settings.Contractor, true);
                else
                    SelectItem(cbContractors, rs.Fields["CONTRACTOR"].Value, true);
            }
            catch (Exception ex)
            {

            }
          

            // MIN MATERIAL
            try
            {
                SelectItem(cbMinMaterials, settings.MinMaterial, false); // have to pass false because we can't simply create a new min material if it does not exist
            }
            catch (Exception ex)
            {
            }

            if (m_Diag.IsEnabled(GTDiagCat.EE)) m_Diag.LogExit("FillValuesFromDatabase");

           
        }

        private bool IsNullOrEmptyString(Field field)
        {
            if (field.Value == DBNull.Value)
                return true;

            if (string.IsNullOrEmpty(field.Value.ToString()))
                return true;

            return false;
        }

       

        private void SelectItem(ComboBox combobox, object valueToSelect, bool createIfNoExist)
        {

            // null value specified, default to the first item
            // also for empty strings
            if (combobox.Items.Count > 0)
                combobox.SelectedIndex = 0;
            if (valueToSelect == DBNull.Value)
                return;
            if (string.IsNullOrEmpty(valueToSelect.ToString()))
                return;


            // select the item if it exists.
            int index=0;
            foreach (string item in combobox.Items)
            {
                if (item == valueToSelect.ToString())
                {
                    combobox.SelectedIndex = index;
                    return;
                }
                index++;
            }

            // the item does not exist, create new one
            if (createIfNoExist)
            {
                int newItemIndex = combobox.Items.Add(valueToSelect);
                combobox.SelectedIndex = newItemIndex;
            }

        }

        private void SaveSpliceAttributes()
        {
             // save previously selected values
            SaveValuesForFuture();

           

            string minMaterial = cbMinMaterials.SelectedItem.ToString();
            string branchType = "";
            int closureSize = 0;
            ParseMinMaterial(minMaterial, ref branchType, ref closureSize);

            
            // update components

            // FSPLICE TABLE
            Recordset rs = componentList.GetComponent(CNO_FSplice).Recordset;
            rs.MoveFirst();
            rs.Update("CLOSURE_SIZE", closureSize);
            rs.Update("BRANCH_TYPE", branchType);
            if (cbManufacturers.SelectedIndex != -1)
                rs.Update("MANUFACTURER", cbManufacturers.SelectedItem.ToString());
            if (cbContractors.SelectedIndex != -1)
                rs.Update("CONTRACTOR", cbContractors.SelectedItem.ToString());

            // GC_NETELEM TABLE
            rs = componentList.GetComponent(CNO_GCNetElem).Recordset;
            rs.MoveFirst();
            rs.Update("MIN_MATERIAL", minMaterial);
            if (!string.IsNullOrEmpty(txtOwnership.Text)) 
                rs.Update("OWNERSHIP", txtOwnership.Text);
            rs.Update("YEAR_PLACED", txtYearPlaced.Text);

            //GC_OWNERSHIP table
            //string owner_id = Get_Value("select g3e_id from gc_ownership where g3e_fid=" + txtOwnerFID.Text);

            //if (owner_id != "")
            //{
            //    rs = componentList.GetComponent(CNO_Ownedby).Recordset;
            //    rs.MoveFirst();
            //    rs.Update("OWNER1_ID", owner_id);
            //}

            //

            txtOwnerFID.Text = "";
            lbOwnerFID.Text = "FID:";
            txtManhlID.Text = "";
            CloseStatus = 1;
            Close();

        }

        private bool Validate()
        {

            if (cbMinMaterials.SelectedItem == null)
            {
                MessageBox.Show("Please choose Plant Unit first!", "Splice Attributes", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return false;
            }

            //if (txtOwnerFID.Text == "")
            //{
            //    MessageBox.Show("Please select a Owner Feature : Pole or Manhole !", "Splice Attributes", MessageBoxButtons.OK, MessageBoxIcon.Information);
            //    m_gtapp.SelectedObjects.Clear();
            //    return false;
            //}

            return true;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (Validate())
            {
                if (m_Diag.IsEnabled(GTDiagCat.EE)) m_Diag.LogEnter("btnOK_Click");
                SaveSpliceAttributes();
                if (m_Diag.IsEnabled(GTDiagCat.EE)) m_Diag.LogExit("btnOK_Click");
            }
            return;
        }

        private void SaveValuesForFuture()
        {
            if (m_Diag.IsEnabled(GTDiagCat.EE)) m_Diag.LogEnter("SaveValuesForFuture"); 

            if (cbManufacturers.SelectedIndex!= -1)
                settings.Manufacturer = cbManufacturers.SelectedItem.ToString();

            if (cbMinMaterials.SelectedIndex != -1)
                settings.MinMaterial = cbMinMaterials.SelectedItem.ToString();

            if (!string.IsNullOrEmpty(txtOwnership.Text))
                settings.Ownership = txtOwnership.Text;

            if (cbContractors.SelectedIndex != -1)
                settings.Contractor = cbContractors.SelectedItem.ToString();

            if (!string.IsNullOrEmpty(txtYearPlaced.Text))
                settings.YearPlaced = txtYearPlaced.Text;

            settings.Save();

            if (m_Diag.IsEnabled(GTDiagCat.EE)) m_Diag.LogExit("SaveValuesForFuture"); 
        }


        private void ParseMinMaterial(string minMaterial, ref string branchType, ref int closureSize)
        {
            if (m_Diag.IsEnabled(GTDiagCat.EE)) m_Diag.LogEnter("ParseMinMaterial"); 

            char[] delim={' '};
            string[] parts = minMaterial.Split(delim);
            if (parts.Length == 2)
            {
                closureSize = Convert.ToInt32(parts[0]);
                branchType = parts[1];
            }

            if (m_Diag.IsEnabled(GTDiagCat.EE)) m_Diag.LogExit("ParseMinMaterial"); 
        }

      
        private void btn_Pick_Click(object sender, EventArgs e)
        {
          //  GetOwner SpliceCivilOwner = new GetOwner();
          // SpliceCivilOwner.GetFNOFIDOwner();

            if (m_gtapp.SelectedObjects.FeatureCount == 0)
            {
                MessageBox.Show("Please select a Owner Feature : Pole or Manhole !", "Splice Attributes", MessageBoxButtons.OK, MessageBoxIcon.Information);
             //   m_gtapp.SelectedObjects.Clear();
                return;
            }
            if (m_gtapp.SelectedObjects.FeatureCount > 0)
            {
                foreach (IGTDDCKeyObject oDDCKeyObject in m_gtapp.SelectedObjects.GetObjects())
                {
                    if (oDDCKeyObject.FNO == 3000 || oDDCKeyObject.FNO == 2700)
                    {
                        txtOwnerFID.Text = oDDCKeyObject.FID.ToString();

                        if (oDDCKeyObject.FNO == 2700)
                        {
                            lbOwnerFID.Text = "Manhole FID:";
                            txtManhlID.Text = Get_Value("select MANHOLE_ID from  gc_manhl where g3e_fid=" + oDDCKeyObject.FID.ToString());
                        }
                        else
                        {
                            lbOwnerFID.Text = "Pole FID:";
                            txtManhlID.Text = "pole";
                        }
                        return;
                    }
                    else
                    {
                        MessageBox.Show("Please select a Owner Feature : Pole or Manhole !", "Splice Attributes", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        m_gtapp.SelectedObjects.Clear();
                        return;
                    }
                }
            }
        }

        //Get Value from Database
        public string Get_Value(string sSql)
        {
            try
            {
                ADODB.Recordset rsPP = new ADODB.Recordset();
                rsPP = dc.OpenRecordset(sSql, ADODB.CursorTypeEnum.adOpenStatic, ADODB.LockTypeEnum.adLockReadOnly, 1);
                if (rsPP.RecordCount > 0)
                {
                    rsPP.MoveFirst();
                    return (rsPP.Fields[0].Value.ToString());
                }
                return null;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Splice Attributes", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }

        }

        private void AssignmentForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (CloseStatus == 0)
            {
                if (Validate())
                {
                   SaveSpliceAttributes();
                 }
                 else e.Cancel = true;
            }
        }

       
    }
}