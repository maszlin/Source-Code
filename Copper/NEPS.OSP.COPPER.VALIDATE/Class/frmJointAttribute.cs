/*
 * frmJointAttribute.cs
 * - this form allow user to select Joint and Closure Type for edited Joint
 * - together with selected values from connected cable (read from AG_CABLE_JOINT) update attribute values of the JOINT
 * 
 * edited : m.zam @ 12-09-2012
 * issues : handle copper cable connected to fiber cabinet
 * copy rt_code by default
 * 
 * 
 * edited : m.zam @ 26-09-2012
 * issues : 
 * 1.selected plant unit for joint must be available in ref_cop_mm_splice table
 * if the PU not available we prompt user to change selection or accept selection with error 
 * 
 * 2. autopopulate cable usage 
 * d-side : distribution
 * e-side : main
 * 
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Intergraph.GTechnology.Interfaces;
using Intergraph.GTechnology.API;
using ADODB;

namespace NEPS.OSP.COPPER.VALIDATE
{
    public partial class frmJointAttribute : Form
    {
        private IGTComponents jComps;
        private int cFID;
        private int jFID;
        private string itfCode;
        private string rtCode;
        private string cCode;
        private string cType;
        private int cSize;
        private double cGauge;
        private Logger log;

        public frmJointAttribute(int jointFID, string[] cableAttribute)
        {
            InitializeComponent();
            this.Text = "Joint Attributes [FID:" + jointFID.ToString() + "]";
            jFID = jointFID;
            cFID = int.Parse(cableAttribute[0]);
            itfCode = cableAttribute[1];
            rtCode = cableAttribute[2];
            cCode = cableAttribute[3];
            cType = cableAttribute[4];
            cSize = int.Parse(cableAttribute[5]);
            cGauge = double.Parse(cableAttribute[6]);
        }

        private void frmJointAttribute_Load(object sender, EventArgs e)
        {
            try
            {

                sendFormToBottomLeft();

                if (cFID < 1)
                {
                    this.Close();
                    return;
                }
                LoadPlanUnit("|" + cType + "|" + cSize.ToString() + "|" + cGauge.ToString() + "|");
                lblPlantUnit.Text = cmbJoint.Text + "|" + cType +
                    "|" + cSize.ToString() + "|" + cGauge.ToString() + "|" + cmbClosure.Items[0].ToString();
                lblCableCode.Text = "Cable Code : " + cCode;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading joint attribute form\r\n" + ex.Message);
            }
            finally
            {
                Application.DoEvents();
            }
        }

        private void sendFormToBottomLeft()
        {
            Rectangle r = Screen.PrimaryScreen.WorkingArea;
            this.StartPosition = FormStartPosition.Manual;
            this.Location = new Point(Screen.PrimaryScreen.WorkingArea.Width - (this.Width * 2),
                Screen.PrimaryScreen.WorkingArea.Height - (this.Height * 2));
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (!ValidatePlanUnit(lblPlantUnit.Text))
                if (MessageBox.Show(this,
                    "Selected Plan Unit not available in the database.\r\nProceed to save current setting and close dialog?", 
                    "Saving Joint", MessageBoxButtons.OKCancel) == DialogResult.Cancel)
                    return;

            UpdateJointAttribute();
            this.Close();
        }

        private void UpdateJointAttribute()
        {
            IGTComponent jComp;

            jComp = GetJointAttribute.m_GTComponents.GetComponent(10801); // gc_splice
            jComp.Recordset.MoveFirst();

            jComp.Recordset.Update("JOINT_TYPE", cmbJoint.Text);
            jComp.Recordset.Update("CLOSURE_TYPE", cmbClosure.Text);
            jComp.Recordset.Update("ITFACE_CODE", itfCode);
            jComp.Recordset.Update("RT_CODE", rtCode);
            jComp.Recordset.Update("CABLE_CODE", cCode);

            jComp = GetJointAttribute.m_GTComponents.GetComponent(51); // gc_netelem
            jComp.Recordset.MoveFirst();
            jComp.Recordset.Update("MIN_MATERIAL", lblPlantUnit.Text);
        }

        private void cmbJoint_SelectedIndexChanged(object sender, EventArgs e)
        {
            lblPlantUnit.Text = cmbJoint.Text + "|" + cType + "|" + cSize.ToString() + "|" + cGauge.ToString() + "|" + cmbClosure.Text;
        }

        private bool ValidatePlanUnit(string PU)
        {
            string ssql = "SELECT * FROM REF_COP_MM_SPLICE WHERE MIN_MATERIAL LIKE '" + PU + "'";
            ADODB.Recordset rsSQL = new ADODB.Recordset();

            rsSQL = GTClassFactory.Create<IGTApplication>().DataContext.OpenRecordset
                (ssql, ADODB.CursorTypeEnum.adOpenDynamic, ADODB.LockTypeEnum.adLockOptimistic,
                (int)ADODB.CommandTypeEnum.adCmdText);

            return (rsSQL.RecordCount > 0 ? true : false);
        }

        private void LoadPlanUnit(string PU)
        {
            try
            {
                string ssql = "SELECT * FROM REF_COP_MM_SPLICE WHERE MIN_MATERIAL LIKE '%" + PU + "%'";
                ADODB.Recordset rsSQL = new ADODB.Recordset();
                rsSQL = GTClassFactory.Create<IGTApplication>().DataContext.OpenRecordset
                    (ssql, ADODB.CursorTypeEnum.adOpenDynamic, ADODB.LockTypeEnum.adLockOptimistic,
                    (int)ADODB.CommandTypeEnum.adCmdText);


                if (rsSQL.RecordCount > 0)
                {
                    cmbJoint.Items.Clear();
                    cmbClosure.Items.Clear();
                    while (!rsSQL.EOF)
                    {
                        string joint = rsSQL.Fields["JOINT_TYPE"].Value.ToString();
                        string closure = rsSQL.Fields["CLOSURE_TYPE"].Value.ToString();
                        if (!cmbJoint.Items.Contains(joint)) cmbJoint.Items.Add(joint);
                        if (!cmbClosure.Items.Contains(closure)) cmbClosure.Items.Add(closure);
                        rsSQL.MoveNext();
                    }
                }
                if (cmbJoint.Items.Count == 0) cmbJoint.Items.Add("***");
                cmbJoint.SelectedIndex = 0;
                if (cmbClosure.Items.Count == 0) cmbClosure.Items.Add("***");
                cmbClosure.SelectedIndex = 0;


            }
            catch (Exception ex)
            {
                log.WriteErr(ex);
            }
        }

    }
}