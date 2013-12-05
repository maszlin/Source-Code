/*
 * 
 * Service Boundary 1.8.0.0
 * edited : m.zam @ 27-March-2012
 * issues : 
 * 1. sort type of Boundary : combobox.sorted = true 
 * 2. change button label -> "Place Boundary"
 * 3. add exit button (remove close button)
 * - form will only close when user press exit button
 * - edit PlaceGeometry - 
 * -> for all 'IGTGeometryEditService' only clear geometry but still pointing to the activewindow
 * 
 * Service Boundary 1.7.0.0
 * edited : m.zam @ 14-March-2012
 * issues : 
 * 1. change temporary line to polygon while user mousemove while generating Boundary
 * 2. enable zooming and pan while editing Boundary
 * 3. clean the code for ESC (cancel all) and BACKSPACE (cancel one point)
 * 4. double-click (or F10) to accept and place Boundary
 * 
 */
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


namespace NEPS.OSP.SERVICE.BND
{
    public partial class GTWindowsForm_Service_Boundary : Form
    {

        public static IGTApplication m_GeoApp;
        //IGTApplication m_GeoApp = GTClassFactory.Create<IGTApplication>();
        private Logger log;

        #region



        //GLobals
        //private short Manhole_FNO = 2700;
        //private short Manhole_GCNO = 2720;

        //private short Trench_FNO = 2200;
        //private short Trench_GCNO = 2210;

        //private short Duct_FNO = 2300;
        //private short IDuct_FNO = 2100; 
        //private short Fibre_FNO = 7200;
        //private short Copper_FNO = 7000;

        //private short Cab_FNO = 2900;
        //private short Cab_GCNO = 2920;

        //private short FDH_FNO = 12400;
        //private short FDH_GCNO = 12420;

        //private short FTP_FNO = 11900;
        //private short FTP_GCNO = 11920;

        //private short FSE_FNO = 11800;
        //private short FSE_GCNO = 11820;
        //private string FSE_TYPE = "Fibre Splice Box";

        //private short FBR_FNO = 15700;
        //private short FBR_GCNO = 15720;

        //private int iEFID = 0;
        //private short iEFNO = 0;

        //private short PrevFNO = 0;
        //private int PrevFID = 0;

        //private IGTDataContext m_GTDataContext = null;
        //IGTApplication m_GTDataContext = GTClassFactory.Create<IGTDataContext>();

        //private static string sExch = null;
        //private static string sCO = null;
        //private static string sPID = null;

        //private int selRow = 0;
        //private string ownership = null;    

        #endregion

        public bool EditFlag = false;
        public bool CopyFlag = false;

        public IGTGeometry oCopyBoundary;

        string ParentXY = "";
        string[] ParentBND;
        int ParentFID = 0;
        int CopyFID = 0;

        public bool CopyBND = false;
        public bool DrawBND = false;

        private string _XPoint = "";
        public string XPointGeom
        {
            get
            {
                return _XPoint;
            }
            set
            {
                if (EditFlag == true)
                {
                    listBox1.Items.Add(value);
                }

            }
        }

        private string _YPoint = "";
        public string YPointGeom
        {
            get
            {
                return _YPoint;
            }
            set
            {
                // listBox2.Items.Add(value);              
            }
        }



        public GTWindowsForm_Service_Boundary()
        {
            try
            {
                InitializeComponent();
                //m_gtapp = GTApplication.Application;
                m_gtapp = GTClassFactory.Create<IGTApplication>().Application;
                m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Running Service Boundary...");

                log = Logger.getInstance();
                //  Assigns the private member variables their default values.

            }
            catch (Exception ex)
            {
                log.WriteLog("");
                log.WriteLog(this.GetType().FullName, "ERROR: " + ex.Message);
                log.WriteLog(ex.StackTrace);
                MessageBox.Show(ex.Message, "Service Boundary", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private Intergraph.GTechnology.API.IGTApplication m_gtapp = null;
        //IGTApplication m_gtapp = GTClassFactory.Create<IGTApplication>();


        private void GTWindowsForm_InitRoute_Shown(object sender, EventArgs e)
        {
            m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Running Service Boundary...");
        }

        private void GTWindowsForm_InitRoute_Activated(object sender, EventArgs e)
        {
            m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Running Service Boundary...");
        }


        //Get Value from Database
        private string Get_Value(string sSql)
        {
            try
            {
                ADODB.Recordset rsPP = new ADODB.Recordset();
                rsPP = GTClassFactory.Create<IGTApplication>().DataContext.OpenRecordset(sSql, ADODB.CursorTypeEnum.adOpenStatic, ADODB.LockTypeEnum.adLockReadOnly, 1);
                if (rsPP.RecordCount > 0)
                {
                    rsPP.MoveFirst();
                    return (rsPP.Fields[0].Value.ToString());
                }
                return null;
            }
            catch (Exception ex)
            {
                log.WriteLog("");
                log.WriteLog(this.GetType().FullName, "ERROR: " + ex.Message);
                log.WriteLog(ex.StackTrace);
                MessageBox.Show(ex.Message, "Service Boundary", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }

        }
        private IGTDataContext m_GTDataContext = null;

        private void Load_Boundary()
        {
            string sSql = "";
            int recordsAffected = 0;
            this.m_GTDataContext = GTClassFactory.Create<IGTApplication>().DataContext;
            sSql = "SELECT BND_TYPE FROM REF_BND_TYPE";
            Recordset rsComp = m_GTDataContext.Execute(sSql, out recordsAffected, (int)CommandTypeEnum.adCmdText, null);

            if (rsComp != null)
            {
                rsComp.MoveFirst();
                do
                {
                    string val = rsComp.Fields[0].Value.ToString();
                    if (val.Length > 0 && val != "***")
                        cboTypeBoundary.Items.Add(val);
                    rsComp.MoveNext();
                }
                while (!rsComp.EOF);
                if (cboTypeBoundary.Items.Count > 0) cboTypeBoundary.SelectedIndex = 0;
            }
            rsComp = null;
        }

        private void Load_Exch_Abb()
        {
            string sSql = "";
            int recordsAffected = 0;
            this.m_GTDataContext = GTClassFactory.Create<IGTApplication>().DataContext;
            sSql = "SELECT PL_NUM, PL_VALUE FROM REF_COM_EXCHABB";
            Recordset rsComp = m_GTDataContext.Execute(sSql, out recordsAffected, (int)CommandTypeEnum.adCmdText, null);

            if (rsComp != null)
            {
                rsComp.MoveFirst();
                do
                {
                    string val = rsComp.Fields[1].Value.ToString();
                    if (val.Length > 0 && val != "***")
                        cboExchAbb.Items.Add(val);
                    rsComp.MoveNext();
                }
                while (!rsComp.EOF);
                if (cboExchAbb.Items.Count > 0) cboExchAbb.SelectedIndex = 0;
            }
            rsComp = null;
        }

        private void Load_Area_Type()
        {
            string sSql = "";
            int recordsAffected = 0;
            this.m_GTDataContext = GTClassFactory.Create<IGTApplication>().DataContext;
            sSql = "SELECT PL_NUM, PL_VALUE FROM REF_BND_BLK_AREATYPE";
            Recordset rsComp = m_GTDataContext.Execute(sSql, out recordsAffected, (int)CommandTypeEnum.adCmdText, null);

            if (rsComp != null)
            {
                rsComp.MoveFirst();
                do
                {
                    string val = rsComp.Fields[1].Value.ToString();
                    if (val.Length > 0 && val != "***")
                        cboAreaType.Items.Add(val);
                    rsComp.MoveNext();
                }
                while (!rsComp.EOF);
                if (cboAreaType.Items.Count > 0) cboAreaType.SelectedIndex = 0;
            }
            rsComp = null;
        }

        private void GTWindowsForm_InitRoute_Load(object sender, EventArgs e)
        {
            Load_Boundary();
            Load_Exch_Abb();
            Load_Area_Type();

            lblExchAbb.Visible = false;
            cboExchAbb.Visible = false;
            lblExch_Abb.Visible = false;

        }

        private bool CheckPoints(string parent, string child)
        {

            string sSql;
            int recordsAffected = 0;
            this.m_GTDataContext = GTClassFactory.Create<IGTApplication>().DataContext;

            //Query For 
            sSql = "select  sdo_geom.relate (a.g3e_geometry ,'determine', ";
            sSql = sSql + "  sdo_geometry(2003,NULL,NULL,sdo_elem_info_array(1,1003,1),";
            sSql = sSql + " sdo_ordinate_array(" + child + ") ),0.0004)";
            sSql = sSql + " from gc_bnd_p a where a.g3e_fid =" + ParentFID + "";


            Recordset rsComp = m_GTDataContext.Execute(sSql, out recordsAffected, (int)CommandTypeEnum.adCmdText, null);

            if (rsComp != null)
            {
                if (rsComp.Fields[0].Value.ToString() == "CONTAINS")
                    return true;
                else
                    return true; // false;
            }
            else
                return true;

        }

        private void btnCopyBND_Click(object sender, EventArgs e)
        {
            if (cboTypeBoundary.Text == "")
            {
                MessageBox.Show(this, "Please Select the Type of Boundary", "Service Boundary", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (cboAreaType.Text == "")
            {
                MessageBox.Show(this, "Please Select the Area Type of Boundary", "Service Boundary", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (cboTypeBoundary.Text == "EXC")
            {
                if (cboExchAbb.Text == "")
                {
                    MessageBox.Show(this, "Please Select the Exch ABB", "Service Boundary", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            }

            if (CopyFID == 0 || txt_CopyBND.Text == "")
            {
                MessageBox.Show(this, "Please Select a Boundary to Copy", "Service Boundary", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            List<IGTPoint> PointCol = new List<IGTPoint>();
            DrawBND = false;
            CopyBND = true;
            PointCol = null;

            IGTKeyObject oExtFeature;
            oExtFeature = null;
            oExtFeature = GTClassFactory.Create<IGTApplication>().DataContext.OpenFeature(24000, CopyFID);
            oCopyBoundary = oExtFeature.Components.GetComponent(24010).Geometry.Copy();
            oCopyBoundary = oExtFeature.Components.GetComponent(24010).Geometry.ExtractGeometry(oCopyBoundary.FirstPoint, oCopyBoundary.LastPoint, false);
            CopyFlag = true;
            btnExit.Enabled = false;

            //DrawAreaBoundary(24000, PointCol);            
        }

        private void btnGenBND_Click(object sender, EventArgs e)
        {
            if (!ValidatePreCondition()) return;

            EditFlag = true;
            btnGenBND.Enabled = false;
            btnCopyBND.Enabled = false;
            btnExit.Enabled = false;

            GTServiceBoundary.InitBoundry();
            /*
            if (GTServiceBoundary.mobjEditService != null) GTServiceBoundary.mobjEditService.RemoveAllGeometries();            
            if (GTServiceBoundary.mobjEditServiceLine != null) GTServiceBoundary.mobjEditServiceLine.RemoveAllGeometries();            
            if (GTServiceBoundary.mobjEditServiceTemp != null) GTServiceBoundary.mobjEditServiceTemp.RemoveAllGeometries();
            */
            m_gtapp.SetStatusBarText(GTStatusPanelConstants.gtaspcMessage, "Click to Place Point. Right Click to Generate Boundary.`");
            GTClassFactory.Create<IGTApplication>().Application.ActiveMapWindow.MousePointer = GTMousePointerConstants.gtmwmpCrossHair;

        }

        public void DrawAreaBoundary(short iFNO, List<IGTPoint> PointCol)
        {
            short CNO = 0;
            try
            {
                this.TopMost = false;
                if (ParentXY != "" && DrawBND == true)
                {
                    string NewXY = "";
                    for (int i = 0; i <= PointCol.Count - 1; i++)
                    {
                        if (NewXY == "")
                        {
                            NewXY = PointCol[i].X + "," + PointCol[i].Y;
                        }
                        else
                        {
                            NewXY = NewXY + "," + PointCol[i].X + "," + PointCol[i].Y;
                        }
                    }
                    // close the boundary by adding back the first point to last.
                    NewXY = NewXY + "," + PointCol[0].X + "," + PointCol[0].Y;

                    if (CheckPoints(ParentXY, NewXY) == false)
                    {
                        MessageBox.Show(this, "Boundary Points does not contains inside the Parent Boundary", "Service Boundary", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        if (GTServiceBoundary.mobjEditService.GeometryCount > 0) GTServiceBoundary.mobjEditService.RemoveAllGeometries();
                        if (GTServiceBoundary.mobjEditServiceTemp.GeometryCount > 0) GTServiceBoundary.mobjEditServiceTemp.RemoveAllGeometries();
                        if (GTServiceBoundary.mobjEditServiceLine.GeometryCount > 0) GTServiceBoundary.mobjEditServiceLine.RemoveAllGeometries();
                        return;
                    }
                }

                GTServiceBoundary.m_oGTTransactionManager.Begin("DrawArea");

                IGTKeyObject oNewFeature;

                oNewFeature = GTClassFactory.Create<IGTApplication>().DataContext.NewFeature(iFNO);
                SaveBoundary(oNewFeature, PointCol);


                if (CopyBND == true)
                {
                    CopyFlag = false;
                    if (GTServiceBoundary.mobjEditServiceBound != null)
                    {
                        GTServiceBoundary.mobjEditServiceBound.RemoveAllGeometries();
                    }
                }
                else
                {
                    //Remove Temporary Geometry
                    if (GTServiceBoundary.mobjEditService != null)
                    {
                        GTServiceBoundary.mobjEditService.RemoveAllGeometries();
                    }
                    if (GTServiceBoundary.mobjEditServiceLine != null)
                    {
                        GTServiceBoundary.mobjEditServiceLine.RemoveAllGeometries();
                    }
                    if (GTServiceBoundary.mobjEditServiceTemp != null)
                    {
                        GTServiceBoundary.mobjEditServiceTemp.RemoveAllGeometries();
                    }
                    EditFlag = false;
                }

                btnGenBND.Enabled = true;
                btnCopyBND.Enabled = true;
                btnExit.Enabled = true;

            }
            catch (Exception ex)
            {
                log.WriteLog("Error");
                log.WriteLog(this.GetType().FullName, "ERROR: " + ex.Message);
                log.WriteLog(ex.StackTrace);
                GTServiceBoundary.m_oGTTransactionManager.Rollback();
                MessageBox.Show(ex.Message, "Service Boundary", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
            finally
            {
                this.TopMost = true;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            EditFlag = true;
        }

        private void button6_Click(object sender, EventArgs e)
        {
            EditFlag = false;
            if (listBox1.Items.Count < 3)
            {
                MessageBox.Show(this, "Please Select Minimum 3 Points.!", "Service Boundary", MessageBoxButtons.OK, MessageBoxIcon.Information);
                EditFlag = true;
                return;
            }
        }

        private void btnPickParent_Click(object sender, EventArgs e)
        {
            IGTGeometry geom = null;
            short iFNO = 0;
            int iFID = 0;

            if (GTClassFactory.Create<IGTApplication>().SelectedObjects.FeatureCount == 0)
            {
                MessageBox.Show(this, "Please select a Feature", "Service Boundary", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            foreach (IGTDDCKeyObject oDDCKeyObject in GTClassFactory.Create<IGTApplication>().SelectedObjects.GetObjects())
            {
                geom = oDDCKeyObject.Geometry;
                iFNO = oDDCKeyObject.FNO;
                iFID = oDDCKeyObject.FID;

                if (oDDCKeyObject.ComponentViewName == "VGC_BND_P")
                {
                    if (iFNO == 24000)
                    {
                        if (!Add_Route(iFNO, iFID)) return;
                        Get_X_Y_Feature(iFID);
                        ParentFID = iFID;
                        lblExch_Abb.Text = Get_Value("SELECT EXC_ABB FROM GC_BND WHERE G3E_FID=" + iFID);
                    }
                }
            }
        }

        private void Get_X_Y_Feature(int FID)
        {
            string sSql = "SELECT T.X, T.Y FROM GC_BND_P A, TABLE(SDO_UTIL.GETVERTICES(A.G3E_GEOMETRY)) T WHERE A.G3E_FID =" + FID;
            ADODB.Recordset rs = new ADODB.Recordset();
            rs = GTClassFactory.Create<IGTApplication>().DataContext.OpenRecordset(sSql, ADODB.CursorTypeEnum.adOpenStatic, ADODB.LockTypeEnum.adLockReadOnly, 1);

            ParentXY = "";
            if (rs.RecordCount > 0)
            {
                rs.MoveFirst();
                do
                {
                    if (ParentXY == "")
                    {
                        ParentXY = rs.Fields[0].Value.ToString() + "," + rs.Fields[1].Value.ToString() + ",0";
                    }
                    else
                    {
                        ParentXY = ParentXY + "," + rs.Fields[0].Value.ToString() + "," + rs.Fields[1].Value.ToString() + ",0";
                    }
                    rs.MoveNext();
                }
                while (!rs.EOF);
            }
            rs = null;
        }

        private void Check_Parent_Boundary(string BndType)
        {
            lblParent.Text = "";
            txtParent.Text = "";
            cboParent.Items.Clear();
            //SELECT * FROM REF_BND_TYPE
            string sSql = "SELECT PRT_BND FROM REF_BND_TYPE WHERE BND_TYPE = '" + BndType + "'";
            int i = 0;
            this.m_GTDataContext = GTClassFactory.Create<IGTApplication>().DataContext;
            ADODB.Recordset rs = new ADODB.Recordset();
            rs = m_GTDataContext.OpenRecordset(sSql, ADODB.CursorTypeEnum.adOpenStatic, ADODB.LockTypeEnum.adLockReadOnly, 1);
            if (rs.RecordCount > 0)
            {
                rs.MoveFirst();
                do
                {
                    lblParent.Text = rs.Fields[0].Value.ToString();
                    rs.MoveNext();
                }
                while (!rs.EOF);

                string[] bndtype = lblParent.Text.ToString().Split(',');

                for (i = 0; i < bndtype.Length; i++)
                {
                    cboParent.Items.Add(bndtype[i].ToString().Trim());
                }
            }
        }

        private bool Add_Route(short iFNO, int iFID)
        {
            try
            {
                string sql = "SELECT FEATURE_TYPE FROM GC_BND_P WHERE G3E_FID =" + iFID;
                txtParent.Text = "--";

                this.m_GTDataContext = GTClassFactory.Create<IGTApplication>().DataContext;
                ADODB.Recordset rs = new ADODB.Recordset();
                rs = m_GTDataContext.OpenRecordset(sql, ADODB.CursorTypeEnum.adOpenStatic, ADODB.LockTypeEnum.adLockReadOnly, 1);
                if (rs.RecordCount > 0)
                {
                    if (rs.Fields[0].Value != DBNull.Value)
                    {
                        if (cboParent.FindStringExact(rs.Fields[0].Value.ToString().Trim()) == -1)
                        {
                            MessageBox.Show(this, "It is not Parent Boundary of " + cboTypeBoundary.Text + "", "Service Boundary", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            return false;
                        }
                        else
                        {
                            txtParent.Text = rs.Fields[0].Value.ToString().Trim();
                            return true;
                        }
                    }
                }

                MessageBox.Show(this, "Parent Boundary Feature Type is Empty", "Service Boundary", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return false;
            }
            catch (Exception ex)
            {
                log.WriteLog("");
                log.WriteLog(this.GetType().FullName, "ERROR: " + ex.Message);
                log.WriteLog(ex.StackTrace);
                MessageBox.Show(ex.Message, "Service Boundary", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

        }

        #region "Copy Boundary - by others"
        private void btnCancel_Click(object sender, EventArgs e)
        {
            cboTypeBoundary.SelectedIndex = -1;
            txtParent.Text = "";
            txt_CopyBND.Text = "";
            ParentXY = "";
            listBox1.Items.Clear();
            EditFlag = false;
            CopyFlag = false;

            //Remove Temporary Geometry
            GTServiceBoundary.InitBoundry();
            /*
            if (GTServiceBoundary.mobjEditService.GeometryCount > 0) GTServiceBoundary.mobjEditService.RemoveAllGeometries();
            if (GTServiceBoundary.mobjEditServiceLine.GeometryCount > 0) GTServiceBoundary.mobjEditServiceLine.RemoveAllGeometries();
            if (GTServiceBoundary.mobjEditServiceTemp.GeometryCount > 0) GTServiceBoundary.mobjEditServiceTemp.RemoveAllGeometries();
            */
            btnGenBND.Enabled = true;
            btnCopyBND.Enabled = true;
            btnCancel.Enabled = false;
            btnExit.Enabled = true;

        }

        private void cboTypeBoundary_SelectedIndexChanged(object sender, EventArgs e)
        {

            if (cboTypeBoundary.Text == "EXC")
            {
                Check_Termination_Point(cboTypeBoundary.Text);
                txtParent.Enabled = false;
                btnPickParent.Enabled = false;
                lblParent.Text = "--";
                lblExchAbb.Visible = true;
                cboExchAbb.Visible = true;
                lblExch_Abb.Visible = false;
            }
            else
            {
                Check_Parent_Boundary(cboTypeBoundary.Text);
                Check_Termination_Point(cboTypeBoundary.Text);
                txtParent.Enabled = true;
                txtParent.Text = "";
                btnPickParent.Enabled = true;
                lblExchAbb.Visible = true;
                cboExchAbb.Visible = false;
                lblExch_Abb.Visible = true;
                lblExch_Abb.Text = "--";
            }
        }

        private void btnPickCopy_Click(object sender, EventArgs e)
        {
            IGTGeometry geom = null;
            short iFNO = 0;
            int iFID = 0;

            if (GTClassFactory.Create<IGTApplication>().SelectedObjects.FeatureCount == 0)
            {
                MessageBox.Show(this, "Please select a Feature", "Service Boundary", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            foreach (IGTDDCKeyObject oDDCKeyObject in GTClassFactory.Create<IGTApplication>().SelectedObjects.GetObjects())
            {
                geom = oDDCKeyObject.Geometry;
                iFNO = oDDCKeyObject.FNO;
                iFID = oDDCKeyObject.FID;

                if (oDDCKeyObject.ComponentViewName == "VGC_BND_P")
                {
                    if (iFNO == 24000)
                    {
                        txt_CopyBND.Text = Get_Value("SELECT FEATURE_TYPE FROM GC_BND_P WHERE G3E_FID =" + iFID);
                        CopyFID = iFID;
                    }
                }
            }
        }
        #endregion

        private void btnExit_Click(object sender, EventArgs e)
        {
            //Remove Temporary Geometry
            if (GTServiceBoundary.mobjEditService != null)
            {
                GTServiceBoundary.mobjEditService.RemoveAllGeometries();
                GTServiceBoundary.mobjEditService = null;
            }
            if (GTServiceBoundary.mobjEditServiceLine != null)
            {
                GTServiceBoundary.mobjEditServiceLine.RemoveAllGeometries();
                GTServiceBoundary.mobjEditServiceLine = null;
            }
            if (GTServiceBoundary.mobjEditServiceTemp != null)
            {
                GTServiceBoundary.mobjEditServiceTemp.RemoveAllGeometries();
                GTServiceBoundary.mobjEditServiceTemp = null;
            }
            // close this form
            this.Close();
        }


        #region Update 1.8.2 Termination Point - m.zam @ 18-apr-2012

        private void Check_Termination_Point(string BndType)
        {
            lblTermination.Text = "";
            txtTermination.Text = "";
            cboTermination.Items.Clear();
            try
            {
                string sSql = "SELECT VGC_TABLE FROM REF_BND_TYPE WHERE BND_TYPE = '" + BndType + "'";
                int i = 0;
                this.m_GTDataContext = GTClassFactory.Create<IGTApplication>().DataContext;
                ADODB.Recordset rs = new ADODB.Recordset();
                rs = m_GTDataContext.OpenRecordset(sSql, ADODB.CursorTypeEnum.adOpenStatic, ADODB.LockTypeEnum.adLockReadOnly, 1);
                if (rs.RecordCount > 0)
                {
                    string termination;
                    rs.MoveFirst();
                    do
                    {
                        termination = rs.Fields[0].Value.ToString();
                        rs.MoveNext();
                    }
                    while (!rs.EOF);

                    string[] t = termination.Split(',');

                    for (i = 0; i < t.Length; i++)
                    {
                        try
                        {
                            if (i > 0) lblTermination.Text += ", ";
                            t[i] = t[i].Trim();
                            string termPoint = t[i].Substring(4, t[i].Length - 6);
                            if (termPoint == "ITFACE")
                                termPoint = cboTypeBoundary.Text;
                            lblTermination.Text += termPoint;
                            cboTermination.Items.Add(t[i]);
                        }
                        catch
                        {
                        }
                    }
                }
                else
                {
                    lblTermination.Text = "--";
                }
            }
            catch { }
        }

        short TermPointFNO = 0;
        int TermPointFID = 0;
        private void btnPickTermination_Click(object sender, EventArgs e)
        {
            IGTGeometry geom = null;
            short iFNO = 0;
            int iFID = 0;

            if (GTClassFactory.Create<IGTApplication>().SelectedObjects.FeatureCount == 0)
            {
                MessageBox.Show(this, "Please select a Feature", "Service Boundary", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            bool PickFlag = false;
            foreach (IGTDDCKeyObject oDDCKeyObject in GTClassFactory.Create<IGTApplication>().SelectedObjects.GetObjects())
            {
                if (cboTermination.FindStringExact(oDDCKeyObject.ComponentViewName) > -1)
                {
                    if (ValidateTerminationPoint(oDDCKeyObject.FID))
                    {
                        PickFlag = true;
                        txtTermination.Text = oDDCKeyObject.ComponentViewName;
                        txtTermination.Tag = oDDCKeyObject.FID;
                        TermPointFNO = oDDCKeyObject.FNO;
                        TermPointFID = oDDCKeyObject.FID;
                    }
                    else
                    {
                        MessageBox.Show(this, "Pick termination point fail.\r\nBoundary already define for selected feature", "Service Boundary");
                        return;
                    }
                    break;
                }
            }

            if (!PickFlag)
            {
                if (txtTermination.Text.Length > 0 & txtTermination.Text != "--")
                    MessageBox.Show(this, txtTermination.Text + " is not termination point of " + cboTypeBoundary.Text + "", "Service Boundary", MessageBoxButtons.OK, MessageBoxIcon.Information);
                else
                    MessageBox.Show(this, "This is not termination point of " + cboTypeBoundary.Text + "", "Service Boundary", MessageBoxButtons.OK, MessageBoxIcon.Information);
                txtTermination.Text = "";
                txtTermination.Tag = -1;
                return;
            }
            else if (!ValidateFiberTermination(TermPointFID))
            {
                txtTermination.Text = "";
                txtTermination.Tag = -1;
                return;
            }
            else if (txtTermination.Text.Length > 6)
            {
                string t = txtTermination.Text;
                txtTermination.Text = t.Substring(4, t.Length - 6);
                //if (txtTermination.Text == "ITFACE") txtTermination.Text = "CABINET";
                txtTermination.Text = GetBoundaryName(TermPointFID);
                //GetTerminationDetail(TermPointFID, TermPointFNO);
            }
        }

        private bool ValidateFiberTermination(int terminationFID)
        {
            string rttype;
            if (cboTypeBoundary.Text.IndexOf("RT") > -1)
                rttype = GetFieldValue("GC_RT", "RT_TYPE", terminationFID);
            else if (cboTypeBoundary.Text.IndexOf("MSAN") > -1)
                rttype = GetFieldValue("GC_MSAN", "RT_TYPE", terminationFID);
            else if (cboTypeBoundary.Text.IndexOf("VDSL2") > -1)
                rttype = GetFieldValue("GC_VDSL2", "RT_TYPE", terminationFID);
            else if (cboTypeBoundary.Text == "SDF" || cboTypeBoundary.Text == "PCAB" || cboTypeBoundary.Text == "CAB")
                rttype = GetFieldValue("GC_ITFACE", "ITFACE_CLASS", terminationFID);
            else
                return true;

            if (cboTypeBoundary.Text.IndexOf(rttype) > -1)
                return true;
            else if (cboTypeBoundary.Text == "PCAB" && rttype == "PHANTOM CABINET")
                return true;
            else if (cboTypeBoundary.Text == "CAB" && rttype == "CABINET")
                return true;
            else
            {
                MessageBox.Show(this, rttype + " is not termination point of " + cboTypeBoundary.Text, "Service Boundary", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return false;
            }

        }

        private void GetTerminationDetail(int iFID, short iFNO)
        {
            try
            {
                string ssql = "SELECT {1} FROM {0} WHERE G3E_FID = " + iFID.ToString();
                switch (iFNO)
                {
                    case 10300: ssql = string.Format(ssql, "GC_ITFACE", "ITFACE_CODE, ITFACE_CLASS"); break;
                    case 6200: ssql = string.Format(ssql, "GC_PDDP", "PDDP_CODE"); break;
                    case 6300: ssql = string.Format(ssql, "GC_DDP", "DDP_NUM"); break;
                    case 13000: ssql = string.Format(ssql, "GC_DP", "DP_NUM"); break;
                    case 13100: ssql = string.Format(ssql, "GC_PDP", "PDP_CODE"); break;
                    case 13200: ssql = string.Format(ssql, "GC_IDF", "IDF_CODE"); break;
                }

                ADODB.Recordset rs = new ADODB.Recordset();
                rs = GTClassFactory.Create<IGTApplication>().DataContext.OpenRecordset(ssql, ADODB.CursorTypeEnum.adOpenStatic, ADODB.LockTypeEnum.adLockOptimistic, 1);

                if (rs.RecordCount > 0)
                {
                    rs.MoveFirst();
                    if (iFNO == 10300)
                        txtTermination.Text = rs.Fields[1].Value.ToString() + " CODE - " + rs.Fields[0].Value.ToString();
                    else
                        txtTermination.Text = rs.Fields[0].Name.ToString() + " - " + rs.Fields[0].Value.ToString();
                }

            }
            catch { }
        }

        private void UpdateTerminationPoint(int boundryFID, int terminationFID)
        {
            try
            {
                string tablename;
                string sSql = "SELECT TABLE_NAME FROM REF_BND_TYPE WHERE BND_TYPE = '"
                    + cboTypeBoundary.Text + "'";

                this.m_GTDataContext = GTClassFactory.Create<IGTApplication>().DataContext;
                ADODB.Recordset rs = new ADODB.Recordset();
                rs = GTClassFactory.Create<IGTApplication>().DataContext.OpenRecordset(sSql, ADODB.CursorTypeEnum.adOpenStatic, ADODB.LockTypeEnum.adLockOptimistic, 1);

                if (rs.RecordCount > 0)
                {
                    rs.MoveFirst();
                    tablename = rs.Fields[0].Value.ToString();
                }
                else
                    return; // exit if record not found

                //bantuan diperlukan BSN -14175-41-00001619-2
                if (tablename == "") return;

                sSql = "UPDATE " + tablename + " SET BND_FID = " + boundryFID +
                   " WHERE G3E_FID = " + terminationFID;

                int iRecordsAffected;

                GTClassFactory.Create<IGTApplication>().DataContext.Execute(sSql, out iRecordsAffected, (int)ADODB.CommandTypeEnum.adCmdText);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, "Error updating termination point\r\n" + ex.Message, "Service Boundary", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool ValidateTerminationPoint(int terminationFID)
        {
            try
            {
                string tablename;
                string sSql = "SELECT TABLE_NAME FROM REF_BND_TYPE WHERE BND_TYPE = '"
                    + cboTypeBoundary.Text + "'";

                this.m_GTDataContext = GTClassFactory.Create<IGTApplication>().DataContext;
                ADODB.Recordset rs = new ADODB.Recordset();
                rs = GTClassFactory.Create<IGTApplication>().DataContext.OpenRecordset
                    (sSql, ADODB.CursorTypeEnum.adOpenStatic, ADODB.LockTypeEnum.adLockOptimistic, 1);

                if (rs.RecordCount > 0)
                {
                    rs.MoveFirst();
                    tablename = rs.Fields[0].Value.ToString();
                }
                else
                    return false; // exit if record not found

                //bantuan diperlukan BSN -14175-41-00001619-2
                if (tablename == "") return true;

                sSql = "SELECT BND_FID FROM " + tablename + " WHERE G3E_FID = " + terminationFID;
                rs = GTClassFactory.Create<IGTApplication>().DataContext.OpenRecordset
                    (sSql, ADODB.CursorTypeEnum.adOpenStatic, ADODB.LockTypeEnum.adLockOptimistic, 1);

                string bndFID = "";
                if (!rs.EOF)
                    bndFID = rs.Fields["BND_FID"].Value.ToString();

                Debug.WriteLine("TABLE : " + tablename + " - FID : " + terminationFID + " - BND FID : " + bndFID);

                if (bndFID.Length == 0)
                    return true;
                else
                {
                    sSql = "SELECT * FROM GC_BND WHERE G3E_FID = " + bndFID;
                    rs = GTClassFactory.Create<IGTApplication>().DataContext.OpenRecordset
                        (sSql, ADODB.CursorTypeEnum.adOpenStatic, ADODB.LockTypeEnum.adLockOptimistic, 1);
                    if (!rs.EOF)
                        return true;
                    else if (MessageBox.Show(this,
                      "Selected feature already have a boundary\r\nDo you want to continue with new boundary ?",
                      "Service Boundary", MessageBoxButtons.OKCancel) == DialogResult.OK)
                        return true;
                    else
                        return false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, "Error updating termination point\r\n" + ex.Message, "Service Boundary", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }


        #endregion

        #region Update 1.8.3 - m.zam @ 10-jul-2012
        /* 
         * update on : 02-JULY-2012
         * update by : m.zam
         * description : add boundary name to GC_BND base on termination >> type + exc_abb + itface
         */
        private bool ValidatePreCondition()
        {
            if (cboTypeBoundary.Text == "")
            {
                MessageBox.Show(this, "Please Select the Type of Boundary", "Service Boundary", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return false;
            }

            if (cboAreaType.Text == "")
            {
                MessageBox.Show(this, "Please Select the Area Type of Boundary", "Service Boundary", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return false;
            }

            if (cboTypeBoundary.Text == "EXC")
            {
                if (cboExchAbb.Text == "")
                {
                    MessageBox.Show(this, "Please Select the Exch ABB", "Service Boundary", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return false;
                }
            }

            if (txtParent.Text == "")
            {
                if (lblParent.Text != "--" && lblParent.Text.Length > 0)
                {
                    MessageBox.Show(this, "Please Select the Parent Boundary", "Service Boundary", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return false;
                }
            }
            if (txtTermination.Text == "")
            {
                if (cboTypeBoundary.Text != "BLOCK" && lblTermination.Text != "--" && lblTermination.Text.Length > 0)
                {
                    MessageBox.Show(this, "Please Select the Termination Point", "Service Boundary", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return false;
                }
            }
            return true;
        }

        private string GetFieldValue(string tablename, string fieldname, int iFID)
        {
            string sql = "SELECT " + fieldname + " FROM " + tablename + " WHERE G3E_FID =" + iFID;
            this.m_GTDataContext = GTClassFactory.Create<IGTApplication>().DataContext;
            ADODB.Recordset rs = new ADODB.Recordset();
            rs = GTClassFactory.Create<IGTApplication>().DataContext.OpenRecordset
                (sql, ADODB.CursorTypeEnum.adOpenStatic, ADODB.LockTypeEnum.adLockReadOnly, 1);
            if (rs.RecordCount > 0)
                return rs.Fields[0].Value.ToString();
            else
                return "---";
        }
        private string GetFieldValues(string tablename, string[] fieldname, int iFID)
        {
            string sql = "SELECT " + fieldname[0];
            for (int i = 1; i < fieldname.Length; i++) sql += "," + fieldname[i];
            sql += " FROM " + tablename + " WHERE G3E_FID =" + iFID;

            this.m_GTDataContext = GTClassFactory.Create<IGTApplication>().DataContext;
            ADODB.Recordset rs = new ADODB.Recordset();
            rs = GTClassFactory.Create<IGTApplication>().DataContext.OpenRecordset
                (sql, ADODB.CursorTypeEnum.adOpenStatic, ADODB.LockTypeEnum.adLockReadOnly, 1);
            if (rs.RecordCount > 0)
            {
                string val = "";
                for (int i = 0; i < fieldname.Length; i++)
                {
                    val += rs.Fields[i].Value.ToString() + " ";
                }
                return val;
            }
            else
                return "---";
        }

        private string GetBoundaryName(int iFID)
        {

            string bndname = cboTypeBoundary.Text + " " + GetFieldValue("GC_NETELEM", "EXC_ABB", iFID) + " "; // GetFieldValue("GC_BND_P", "FEATURE_TYPE", iFID);
            switch (cboTypeBoundary.Text)
            {
                // as requested by Ageng - 02-July-2012
                case "CAB":
                case "PCAB":
                    bndname += GetFieldValue("GC_ITFACE", "ITFACE_CODE", iFID);
                    break;
                case "SDF":
                    bndname += "999-" + GetFieldValue("GC_ITFACE", "ITFACE_CODE", iFID);
                    break;
                case "DDP":
                    bndname += GetFieldValue("GC_DDP", "DDP_NUM", iFID);
                    break;
                case "PDDP":
                    bndname += GetFieldValue("GC_NETELEM", "EXC_ABB", iFID)
                        + " " + GetFieldValue("GC_DDP", "DDP_NUM", iFID)
                        + " " + GetFieldValue("GC_PDDP", "PDDP_CODE", iFID);
                    break;
                case "DP":
                    bndname += GetFieldValues("GC_DP", new string[] { "ITFACE_CODE", "DP_NUM" }, iFID);
                    break;
                case "IDF":
                    bndname += GetFieldValues("GC_IDF", new string[] { "ITFACE_CODE", "IDF_CODE" }, iFID);
                    break;
                case "PDP":
                    bndname += GetFieldValues("GC_PDP", new string[] { "ITFACE_CODE", "PDP_CODE" }, iFID);
                    break;

                // as requested by Kelvin - 04-July-2012
                case "EXC":
                    bndname = bndname.Trim(); break;
                case "MSAN-FTTS":
                case "MSAN-FTTO":
                case "MSAN-PFTTS":
                case "MSAN-FTTZ":
                    bndname += GetFieldValue("GC_MSAN", "RT_CODE", iFID);
                    break;
                case "RT-FTTS":
                case "RT-FTTO":
                case "RT-PFTTS":
                case "RT-FTTZ":
                    bndname += GetFieldValue("GC_RT", "RT_CODE", iFID);
                    break;
                case "FDC":
                    bndname += GetFieldValue("GC_FDC", "FDC_CODE", iFID);
                    break;
                case "FDP":
                    bndname += GetFieldValues("GC_FDP", new string[] { "FDC_CODE", "FDP_CODE" }, iFID);
                    break;
                case "Tie FDP":
                    bndname += GetFieldValues("GC_TIEFDP", new string[] { "FDC_CODE", "MAINFDP_CODE", "FDP_CODE" }, iFID);
                    break;
                case "FTB":
                    bndname += GetFieldValues("GC_FTB", new string[] { "FDC_CODE", "MAINFDP_CODE", "FDP_CODE" }, iFID);
                    break;
                case "DB":
                    // as by kelvin - 2012-07-04
                    // bndname += GetFieldValues("GC_DB", new string[] { "FDC_CODE", "MAINFDP_CODE", "FDP_CODE" }, iFID);
                    // as by mike - 2012-10-17
                    bndname += GetFieldValues("GC_DB", new string[] { "FDC_CODE", "MAINFDP_CODE", "DB_CODE" }, iFID);
                    break;
                case "EPE":
                    bndname += GetFieldValue("GC_EPE", "EPE_CODE", iFID);
                    break;
                case "UPE":
                    bndname += GetFieldValue("GC_UPE", "UPE_CODE", iFID);
                    break;
                case "DDN":
                    bndname += GetFieldValue("GC_DDN", "DDN_CODE", iFID);
                    break;
                case "NDH":
                    bndname += GetFieldValue("GC_NDH", "NDH_CODE", iFID);
                    break;
                case "MINIMUX":
                    bndname += GetFieldValue("GC_MINIMUX", "MUX_CODE", iFID);
                    break;
                case "VDSL2-FTTS":
                case "VDSL2-FTTO":
                    bndname += GetFieldValue("GC_VDSL2", "RT_CODE", iFID);
                    break;
            }
            return bndname.Trim();
        }

        /* 
         * update on : 02-JULY-2012
         * update by : m.zam
         * function 1 : add boundary name to GC_BND base on termination >> type + exc_abb + itface
         * function 2 : pick boundary
         */


        private void btnPickBND_Click(object sender, EventArgs e)
        {
            if (!ValidatePreCondition()) return;
            DrawBND = false;
            CopyBND = false;

            this.TopMost = false;
            try
            {
                if (GTClassFactory.Create<IGTApplication>().SelectedObjects.FeatureCount == 0)
                {
                    MessageBox.Show(this, "No feature is selected. Please select a boundary feature", "Service Boundary", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                IGTSelectedObjects selFeature;  // selected Feature (any object on the map)
                selFeature = GTClassFactory.Create<IGTApplication>().SelectedObjects;

                if (selFeature.FeatureCount == 1)
                {
                    foreach (IGTDDCKeyObject oDDCKeyObject in selFeature.GetObjects())
                    {
                        if (oDDCKeyObject.FNO != 24000) //BoundaryFNO = 24000. Check whether the selected object is DP
                        {
                            MessageBox.Show(this, "Please select only boundary feature", "Service Boundary", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            return;
                        }
                        else if (!ValidatePickBoundary(oDDCKeyObject.FID))
                        {
                            //MessageBox.Show(this, "Sorry. This boundary already define to a termination point.\r\nPlease select an empty boundary", "Service Boundary");
                            return;
                        }
                        else if (MessageBox.Show(this, "Add termination point to the selected boundary", "Service Boundary", MessageBoxButtons.OKCancel) == DialogResult.OK)
                        {
                            GTServiceBoundary.m_oGTTransactionManager.Begin("SelectBoundary");

                            IGTKeyObject oBoundary;
                            oBoundary = GTClassFactory.Create<IGTApplication>().DataContext.OpenFeature(24000, oDDCKeyObject.FID);

                            oCopyBoundary = oBoundary.Components.GetComponent(24010).Geometry.Copy();
                            oCopyBoundary = oBoundary.Components.GetComponent(24010).Geometry.ExtractGeometry(oCopyBoundary.FirstPoint, oCopyBoundary.LastPoint, false);

                            List<IGTPoint> PointCol = new List<IGTPoint>();

                            for (int i = 0; i < oCopyBoundary.KeypointCount; i++)
                                PointCol.Add(oCopyBoundary.GetKeypointPosition(i));
                            
                            SaveBoundary(oBoundary, PointCol);
                            break;
                        } //end if else
                    }//end foreach
                }//end if (selFeature.FeatureCount == 1)

            }
            catch
            {
            }
            finally
            {
                this.TopMost = true;
            }
        }

        private bool ValidatePickBoundary(int iFID)
        {
            return true;

            // return true if the boundary is still available
            try
            {
                string tablename;
                string sSql = "SELECT NAME FROM GC_BND WHERE G3E_FID = " + iFID;

                this.m_GTDataContext = GTClassFactory.Create<IGTApplication>().DataContext;
                ADODB.Recordset rs = new ADODB.Recordset();
                rs = GTClassFactory.Create<IGTApplication>().DataContext.OpenRecordset
                    (sSql, ADODB.CursorTypeEnum.adOpenStatic, ADODB.LockTypeEnum.adLockOptimistic, 1);

                if (rs.EOF)
                    return true;
                else
                {
                    string name = rs.Fields["Name"].Value.ToString().Trim();
                    if (name.Length > 0)
                    {
                        if (iFID == GetTerminationBndFID(name))
                            if (MessageBox.Show(this, "This boundary already has termination point\r\n" +
                                "Do you want to continue or cancel and select another boundary", "Service Boundary", MessageBoxButtons.OKCancel) == DialogResult.Cancel)
                                return false;
                    }
                    return true;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        private void SaveBoundary(IGTKeyObject Boundary, List<IGTPoint> PointCol)
        {
            short iCNO = 0;
            short iFNO = Boundary.FNO; // 24000;
            int iFID = Boundary.FID;
            IGTTextPointGeometry oTextGeom;
            IGTPolygonGeometry oBoundaryLine;

            #region GC_Netelem

            iCNO = 51; //Netelem

            if (Boundary.Components.GetComponent(iCNO).Recordset.EOF)
            {
                Boundary.Components.GetComponent(iCNO).Recordset.AddNew("G3E_FID", iFID);
                Boundary.Components.GetComponent(iCNO).Recordset.Update("G3E_FNO", iFNO);
            }
            else
                Boundary.Components.GetComponent(iCNO).Recordset.MoveLast();

            string exc_abb = (cboTypeBoundary.Text == "EXC" ? cboExchAbb.Text : lblExch_Abb.Text);
            Boundary.Components.GetComponent(iCNO).Recordset.Update("EXC_ABB", exc_abb);
            Boundary.Components.GetComponent(iCNO).Recordset.Update("JOB_STATE", "PROPOSED");
            Boundary.Components.GetComponent(iCNO).Recordset.Update("FEATURE_STATE", "PPF");
            Boundary.Components.GetComponent(iCNO).Recordset.Update("MIN_MATERIAL", "-");


            #endregion

            #region Attributes [GC_BND]

            iCNO = 24001;

            Boundary.Components.GetComponent(iCNO).Recordset.MoveLast();
            Boundary.Components.GetComponent(iCNO).Recordset.Update("EXC_ABB", exc_abb);
            Boundary.Components.GetComponent(iCNO).Recordset.Update("AREA_TYPE", cboAreaType.Text);
            Boundary.Components.GetComponent(iCNO).Recordset.Update("BND_TYPE", cboTypeBoundary.Text);
            Boundary.Components.GetComponent(iCNO).Recordset.Update("PRT_FID", ParentFID);
            Boundary.Components.GetComponent(iCNO).Recordset.Update("NAME", "");

            log.WriteLog("Error CNO :" + iCNO.ToString());
            log.WriteLog("Completed Attributes");

            #endregion

            #region Boundary Geometry

            iCNO = 24010;

            if (Boundary.Components.GetComponent(iCNO).Recordset.EOF)
            {
                Boundary.Components.GetComponent(iCNO).Recordset.AddNew("G3E_FID", iFID);
                Boundary.Components.GetComponent(iCNO).Recordset.Update("G3E_FNO", iFNO);
            }
            else
                Boundary.Components.GetComponent(iCNO).Recordset.MoveLast();

            Boundary.Components.GetComponent(iCNO).Recordset.Update("PRT_FID", ParentFID);
            Boundary.Components.GetComponent(iCNO).Recordset.Update("FEATURE_TYPE", cboTypeBoundary.Text);

            int i = 0;
            oBoundaryLine = GTClassFactory.Create<IGTPolygonGeometry>();
            for (; i < PointCol.Count; i++) oBoundaryLine.Points.Add(PointCol[i]);
            i = PointCol.Count - 1;
            if (PointCol[i].X != PointCol[0].X && PointCol[i].Y != PointCol[0].Y)
                oBoundaryLine.Points.Add(PointCol[0]); // closing the boundary

            Boundary.Components.GetComponent(iCNO).Geometry = oBoundaryLine;

            log.WriteLog("Error CNO :" + iCNO.ToString());
            log.WriteLog("Completed Geometry");

            #endregion

            #region TextGeometry
            iCNO = 24030;

            if (Boundary.Components.GetComponent(iCNO).Recordset.EOF)
            {
                Boundary.Components.GetComponent(iCNO).Recordset.AddNew("G3E_FID", iFID);
                Boundary.Components.GetComponent(iCNO).Recordset.Update("G3E_FNO", iFNO);
            }
            else
                Boundary.Components.GetComponent(iCNO).Recordset.MoveLast();

            oTextGeom = GTClassFactory.Create<IGTTextPointGeometry>();
            IGTPoint point1 = GTClassFactory.Create<IGTPoint>();
            point1.X = PointCol[0].X + 2;
            point1.Y = PointCol[0].Y - 2;
            oTextGeom.Origin = point1;
            Boundary.Components.GetComponent(iCNO).Geometry = oTextGeom;

            log.WriteLog("Error CNO :" + iCNO.ToString());
            log.WriteLog("Completed Text Geometry");

            #endregion

            #region    Demand Total  GC_DMD_TOTAL

            //iCNO = 24002;

            //if (Boundary.Components.GetComponent(iCNO).Recordset.EOF)
            //{
            //    Boundary.Components.GetComponent(iCNO).Recordset.AddNew("G3E_FID", iFID);
            //    Boundary.Components.GetComponent(iCNO).Recordset.Update("G3E_FNO", iFNO);
            //}
            //else
            //{
            //    Boundary.Components.GetComponent(iCNO).Recordset.MoveLast();
            //}


            #endregion

            UpdateTerminationPoint(iFID, (int)txtTermination.Tag);

            GTServiceBoundary.m_oGTTransactionManager.Commit();
            GTServiceBoundary.m_oGTTransactionManager.RefreshDatabaseChanges();
            //GTClassFactory.Create<IGTApplication>().Application.ActiveMapWindow.MousePointer 
            this.EditFlag = false;
            this.CopyFlag = false;
        }
        #endregion

        #region Update 2012-11-20

        private int GetBoundaryFID(string tablename, string exc_abb, string filter)
        {
            string sql = "SELECT A.BND_FID, A.G3E_FID FROM " + tablename + " A, GC_NETELEM B WHERE "
                + filter + " AND B.EXC_ABB = '" + exc_abb + "' AND A.G3E_FID = B.G3E_FID";
            this.m_GTDataContext = GTClassFactory.Create<IGTApplication>().DataContext;
            ADODB.Recordset rs = new ADODB.Recordset();
            rs = GTClassFactory.Create<IGTApplication>().DataContext.OpenRecordset
                (sql, ADODB.CursorTypeEnum.adOpenStatic, ADODB.LockTypeEnum.adLockReadOnly, 1);
            if (rs.RecordCount > 0)
            {
                if (int.Parse(rs.Fields[1].Value.ToString()) == TermPointFID)
                {
                    MessageBox.Show(this, "Selected boundary already assign to the termination point", "Service Boundary");
                    throw new System.Exception("Cancel");
                }
                return int.Parse(rs.Fields[0].Value.ToString());
            }
            else
                return 0;

        }
        private int GetTerminationBndFID(string bndname)
        {
            string[] name = bndname.Split(' ');
            int bFID = 0;
            switch (name[0])
            {
                // as requested by Ageng - 02-July-2012
                case "CAB":
                case "PCAB":
                    bFID = GetBoundaryFID("GC_ITFACE", name[1], "ITFACE_CODE = '" + name[2] + "'");
                    break;
                case "SDF":
                    name[2] = name[2].Substring(4);
                    bFID = GetBoundaryFID("GC_ITFACE", name[1], "ITFACE_CODE = '" + name[2] + "'");
                    break;
                case "DDP":
                    bFID = GetBoundaryFID("GC_DDP", name[1], "DDP_NUM = '" + name[2] + "'");
                    break;
                case "PDDP":
                    bFID = 0;
                    break;
                case "DP":
                    bFID = GetBoundaryFID("GC_DP", name[1], "ITFACE_CODE = '" + name[2]
                        + "' AND DP_NUM = '" + name[3] + "'");
                    break;
                case "IDF":
                    bFID = GetBoundaryFID("GC_IDF", name[1], "ITFACE_CODE = '" + name[2] + "' AND IDF_CODE = '" + name[3] + "'");
                    break;
                case "PDP":
                    bFID = GetBoundaryFID("GC_PDP", name[1], "ITFACE_CODE = '" + name[2] + "' AND PDP_CODE = '" + name[3] + "'");
                    break;

                // as requested by Kelvin - 04-July-2012
                case "EXC":
                    bndname = bndname.Trim(); break;
                case "MSAN-FTTS":
                case "MSAN-FTTO":
                case "MSAN-PFTTS":
                case "MSAN-FTTZ":
                    bFID = GetBoundaryFID("GC_MSAN", name[1], "RT_CODE = '" + name[2] + "'");
                    break;
                case "RT-FTTS":
                case "RT-FTTO":
                case "RT-PFTTS":
                case "RT-FTTZ":
                    bFID = GetBoundaryFID("GC_RT", name[1], "RT_CODE = '" + name[2] + "'");
                    break;
                case "FDC":
                    bFID = GetBoundaryFID("GC_FDC", name[1], "FDC_CODE = '" + name[2] + "'");
                    break;
                case "FDP":
                    bFID = GetBoundaryFID("GC_FDP", name[1], "FDC_CODE = '" + name[2] + "' AND FDP_CODE = '" + name[3] + "'");
                    break;
                case "Tie FDP":
                    bFID = GetBoundaryFID("GC_TIEFDP", name[1], "FDC_CODE = '" + name[2] + "' AND MAINFDP_CODE = '" + name[3] + "' AND FDP_CODE = '" + name[4] + "'");
                    break;
                case "FTB":
                    bFID = GetBoundaryFID("GC_FTB", name[1], "FDC_CODE = '" + name[2] + "' AND MAINFDP_CODE = '" + name[3] + "' AND FDP_CODE = '" + name[4] + "'");
                    break;
                case "DB":
                    bFID = GetBoundaryFID("GC_DB", name[1], "FDC_CODE = '" + name[2] + "' AND MAINFDP_CODE = '" + name[3] + "' AND DB_CODE = '" + name[4] + "'");
                    break;
                case "EPE":
                    bFID = GetBoundaryFID("GC_EPE", name[1], "EPE_CODE = '" + name[2] + "'");
                    break;
                case "UPE":
                    bFID = GetBoundaryFID("GC_UPE", name[1], "UPE_CODE = '" + name[2] + "'");
                    break;
                case "DDN":
                    bFID = GetBoundaryFID("GC_DDN", name[1], "DDN_CODE = '" + name[2] + "'");
                    break;
                case "NDH":
                    bFID = GetBoundaryFID("GC_NDH", name[1], "NDH_CODE = '" + name[2] + "'");
                    break;
                case "MINIMUX":
                    bFID = GetBoundaryFID("GC_MINIMUX", name[1], "MUX_CODE = '" + name[2] + "'");
                    break;
                case "VDSL2-FTTS":
                case "VDSL2-FTTO":
                    bFID = GetBoundaryFID("GC_VDSL2", name[1], "RT_CODE = '" + name[2] + "'");
                    break;
            }

            return (bFID);
        }




        #endregion
    }
}