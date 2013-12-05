using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Intergraph.GTechnology.API;
using ADODB;
using NEPS.GTechnology.AssigJob;
using Intergraph.GTechnology.Diagnostics;
using System.Data.OleDb;
using System.IO;

namespace NEPS.GTechnology.AssignJob
{

    public partial class AssignJobForm : Form
    {
        const int FNO_BOUNDARY = 24000;
        private class FeatureFIDFNO
        {
            public int FID;
            public short FNO;
        }

        public delegate void FinishedHandler(FinishedEventarg arg);
        public event FinishedHandler Finished;
        public class FinishedEventarg
        {
            public BindingList<FeatureItem> items;
        }

        public AssignJobForm()
        {
            InitializeComponent();
        }


        public IGTKeyObject PreselectedBoundary = null;
        public BoundaryItem PreselectedBoundaryItem = null;
        public IGTTransactionManager transactionManager = null;
        public string argument;
        public GTDiagnostics m_Diag = null;

        BindingList<FeatureItem> items = new BindingList<FeatureItem>();
        internal void AddItem(FeatureItem item)
        {
            items.Add(item);
            gvFeatures.DataSource = items;
        }

        protected string JobIDToName(string queryJobID)
        {
            if (m_Diag.IsEnabled(GTDiagCat.EE))
                m_Diag.LogEnter("AssignJob.JobIDToName");

            int affected = 0;
            string sql = string.Format("SELECT * FROM G3E_JOB WHERE SCHEME_NAME='{0}'", queryJobID);
            IGTApplication app = GTClassFactory.Create<IGTApplication>();
            Recordset rs = app.DataContext.Execute(sql, out affected, 0, null);
            if (!rs.EOF)
            {
                rs.MoveFirst();
                return rs.Fields["G3E_IDENTIFIER"].Value.ToString();
            }

            if (m_Diag.IsEnabled(GTDiagCat.EE))
                m_Diag.LogExit("AssignJob.JobIDToName");

            return "";
        }

        private void btnApply_Click(object sender, EventArgs e)
        {
            if (m_Diag.IsEnabled(GTDiagCat.EE))
                m_Diag.LogEnter("AssignJobForm.btnApply_Click");

            // get the current job ID
            IGTApplication app = GTClassFactory.Create<IGTApplication>();
            string activeJob = app.DataContext.ActiveJob;
            string activeJobName = JobIDToName(activeJob);

            if (m_Diag.IsEnabled(GTDiagCat.IV1)) m_Diag.LogValue("AssignJobForm.btnApply_Click",
                GTDiagCatDesc.IV1, "activeJob", activeJob);

            if (m_Diag.IsEnabled(GTDiagCat.IV1)) m_Diag.LogValue("AssignJobForm.btnApply_Click",
                GTDiagCatDesc.IV1, "activeJobName", activeJobName);

            // apply the current job ID into the items
            if (m_Diag.IsEnabled(GTDiagCat.LP1)) m_Diag.LogMessage("AssignJobForm.btnApply_Click",
                GTDiagCatDesc.LP1, "foreach (FeatureItem item in items)");

            transactionManager.Begin("Apply");
            foreach (FeatureItem item in items)
            {
                IGTKeyObject feature = app.DataContext.OpenFeature(item.FNO, item.FID);
                IGTComponent compNetElem = feature.Components.GetComponent(CNO_NETELEM);
                if (item.IsChecked)
                {
                    compNetElem.Recordset.Update("JOB_ID", activeJobName);
                }
                else
                {
                    compNetElem.Recordset.Update("JOB_ID", unassignedJobName);
                }
            }
            transactionManager.Commit();

            Close();

            if (Finished != null)
                Finished(null);

            if (m_Diag.IsEnabled(GTDiagCat.EE))
                m_Diag.LogExit("AssignJobForm.btnApply_Click");
        }


        private void AssignJobForm_Load(object sender, EventArgs e)
        {
            // resize to hide regen tools
            Size sz = this.Size;
            sz.Width -= 470;
            this.Size = sz;
            cbSIDs.SelectedIndex = 0;

            if (m_Diag.IsEnabled(GTDiagCat.EE))
                m_Diag.LogEnter("AssignJobForm.AssignJobForm_Load");

            gvFeatures.AutoGenerateColumns = false;

            // set PreselectedBoundaryItem
            if (PreselectedBoundary != null)
                ShowBoundaryInfo(PreselectedBoundary.FID);

            GetFeatureTypes();
            ShowFeatures(PreselectedBoundaryItem);

            // if argument is specified, regenerate automatically
            if (!string.IsNullOrEmpty(argument))
            {
                AutoRegenerate();

            }


            if (m_Diag.IsEnabled(GTDiagCat.EE))
                m_Diag.LogExit("AssignJobForm.AssignJobForm_Load");

        }

        private void AutoRegenerate()
        {
            // resize to show regen tools
            Size sz = this.Size;
            sz.Width += 470;
            this.Size = sz;

            char[] delimiter ={ '|' };
            string[] parts = argument.Split(delimiter);

            string boundaryFT = parts[0];
            string SID = parts[1];
            string username = parts[2];
            string password = parts[3];

            Regenerate(boundaryFT, SID, username, password);

            if (Finished != null)
                Finished(null);

            Close();
        }

        public class FeatureType
        {
            public short FNO;
            public string Username;
            public override string ToString()
            {
                return string.Format("{0} ({1})", Username, FNO);
            }
        }

        List<FeatureType> featureTypes = new List<FeatureType>();
        private void GetFeatureTypes()
        {
            if (m_Diag.IsEnabled(GTDiagCat.EE))
                m_Diag.LogEnter("AssignJobForm.ShowFeatureTypes");

            IGTApplication app = GTClassFactory.Create<IGTApplication>();
            string sql = "SELECT * FROM G3E_FEATURE ORDER BY G3E_USERNAME";

            int count = 0;
            Recordset rs = app.DataContext.Execute(sql, out count, 0, null);
            if (!rs.EOF)
            {
                rs.MoveFirst();

                if (m_Diag.IsEnabled(GTDiagCat.LP1)) m_Diag.LogMessage("AssignJobForm.ShowFeatureTypes",
                    GTDiagCatDesc.LP1, "while (!rs.EOF)");

                while (!rs.EOF)
                {
                    FeatureType item = new FeatureType();
                    item.FNO = Convert.ToInt16(rs.Fields["G3E_FNO"].Value);
                    item.Username = rs.Fields["G3E_USERNAME"].Value.ToString();

                    if (m_Diag.IsEnabled(GTDiagCat.IV1)) m_Diag.LogValue("AssignJobForm.ShowFeatureTypes",
                        GTDiagCatDesc.IV1, "item.FNO", item.FNO);

                    rs.MoveNext();
                    featureTypes.Add(item);
                }
            }

            if (m_Diag.IsEnabled(GTDiagCat.EE))
                m_Diag.LogEnter("AssignJobForm.ShowFeatureTypes");
        }

        private void ShowBoundaryInfo(int FID)
        {
            if (m_Diag.IsEnabled(GTDiagCat.EE))
                m_Diag.LogEnter("AssignJobForm.ShowBoundaryInfo");

            IGTApplication app = GTClassFactory.Create<IGTApplication>();
            string sql = "SELECT G3E_FID, EX_EX_NAME, G3E_FNO FROM GC_BND_P WHERE G3E_FID=" + FID;

            int count = 0;
            Recordset rs = app.DataContext.Execute(sql, out count, 0, null);
            if (!rs.EOF)
            {
                rs.MoveFirst();
                while (!rs.EOF)
                {
                    BoundaryItem item = new BoundaryItem();
                    item.FID = Convert.ToInt32(rs.Fields["G3E_FID"].Value);
                    item.FNO = Convert.ToInt16(rs.Fields["G3E_FNO"].Value);
                    item.ExtendedName = rs.Fields["EX_EX_NAME"].Value.ToString();
                    rs.MoveNext();
                    PreselectedBoundaryItem = item;
                }
            }

            gbTitle.Text = string.Format("Boundary: {0} FID: {1}", PreselectedBoundaryItem.ExtendedName, PreselectedBoundaryItem.FID);

            if (m_Diag.IsEnabled(GTDiagCat.EE))
                m_Diag.LogExit("AssignJobForm.ShowBoundaryInfo");
        }

        private void btnSelectAll_Click(object sender, EventArgs e)
        {
            if (m_Diag.IsEnabled(GTDiagCat.EE))
                m_Diag.LogEnter("AssignJobForm.ShowBoundaryInfo");

            foreach (FeatureItem item in items)
            {
                item.IsChecked = true;
            }
            gvFeatures.DataSource = items;
            gvFeatures.Refresh();

            if (m_Diag.IsEnabled(GTDiagCat.EE))
                m_Diag.LogExit("AssignJobForm.ShowBoundaryInfo");

        }

        private void btnClearAll_Click(object sender, EventArgs e)
        {
            if (m_Diag.IsEnabled(GTDiagCat.EE))
                m_Diag.LogEnter("AssignJobForm.btnClearAll_Click");

            foreach (FeatureItem item in items)
            {
                item.IsChecked = false;
            }
            gvFeatures.DataSource = items;
            gvFeatures.Refresh();

            if (m_Diag.IsEnabled(GTDiagCat.EE))
                m_Diag.LogExit("AssignJobForm.btnClearAll_Click");
        }

        private const int CNO_NETELEM = 51;
        private const string unassignedJobName = "Mig_Job";
        private const string unassignedFeatureState = "PPF";


        const short CNO_BOUNDARY = 24010;
        private void ShowFeatures(BoundaryItem boundary)
        {
            if (m_Diag.IsEnabled(GTDiagCat.EE))
                m_Diag.LogEnter("AssignJobForm.ShowFeatures");

            if (boundary == null)
                return;

            IGTApplication app = GTClassFactory.Create<IGTApplication>();
            IGTKeyObject objBoundary = app.DataContext.OpenFeature(boundary.FNO, boundary.FID);
            IGTComponent compBoundary = objBoundary.Components.GetComponent(CNO_BOUNDARY);
            IGTGeometry geometryBoundary = compBoundary.Geometry;
            IGTPolygonGeometry polygonBoundary = (IGTPolygonGeometry)geometryBoundary;

            // select all unassigned features
            app.BeginWaitCursor();
            int i = 0;     

            string sql = string.Format("select * from gc_netelem N where " +
                "UPPER(JOB_ID)='{0}' " +
                "AND UPPER(FEATURE_STATE)='{1}' AND " +
                "EXISTS (SELECT * FROM AG_BDY_PAD A WHERE A.fea_fid=n.g3e_fid AND A.BDY_FID={2}) " +
                "ORDER BY G3E_FID",
                unassignedJobName.ToUpper(), unassignedFeatureState.ToUpper(), PreselectedBoundary.FID);

            if (m_Diag.IsEnabled(GTDiagCat.IV1)) m_Diag.LogValue("AssignJobForm.ShowFeatures",
                GTDiagCatDesc.IV1, "SQL", sql);

            List<FeatureFIDFNO> allFeatureFIDFNO = new List<FeatureFIDFNO>();
            int count = 0;
            Recordset rs = app.DataContext.Execute(sql, out count, 0, null);
            if (!rs.EOF)
            {
                rs.MoveFirst();

                if (m_Diag.IsEnabled(GTDiagCat.LP1))
                    m_Diag.LogMessage("AssignJobForm.ShowFeatures", GTDiagCatDesc.LP1, "while (!rs.EOF)");

                while (!rs.EOF)
                {
                    FeatureFIDFNO feature = new FeatureFIDFNO();
                    feature.FID = Convert.ToInt32(rs.Fields["G3E_FID"].Value);
                    feature.FNO = Convert.ToInt16(rs.Fields["G3E_FNO"].Value);
                    allFeatureFIDFNO.Add(feature);
                    rs.MoveNext();
                }
            }

            if (m_Diag.IsEnabled(GTDiagCat.LP1))
                m_Diag.LogMessage("AssignJobForm.ShowFeatures", GTDiagCatDesc.LP1, "foreach (FeatureFIDFNO featureFIDFNO in allFeatureFIDFNO)");

            i = 0;
            foreach (FeatureFIDFNO featureFIDFNO in allFeatureFIDFNO)
            {
                IGTKeyObject featureObj = app.DataContext.OpenFeature(featureFIDFNO.FNO, featureFIDFNO.FID);
                FeatureItem item = new FeatureItem();
                item.originalFeature = featureObj;
                item.Username = GetFeatureUsername(featureFIDFNO.FNO, app);
                AddItem(item);
            }

            app.EndWaitCursor();

            if (m_Diag.IsEnabled(GTDiagCat.EE))
                m_Diag.LogExit("AssignJobForm.ShowFeatures");
        }

        private bool IsInside(IGTKeyObject featureObj, IGTPolygonGeometry poly)
        {
            foreach (IGTComponent comp in featureObj.Components)
            {
                try
                {
                    if (comp.Geometry != null && comp.Geometry.FirstPoint != null)
                    {
                        IGTGeometry geo = comp.Geometry;
                        GTInPolygonType containment = poly.Contains(geo.FirstPoint);
                        if (containment == GTInPolygonType.gtiptInside || containment == GTInPolygonType.gtiptOnPolygon)
                        {
                            m_Diag.LogExit("AssignJobForm.IsInside");
                            return true;
                        }
                    }
                }
                catch (Exception ex)
                {
                    string error = ex.Message;
                    // need to catch exceptions because there could be invalid geometries
                }

            }

            return false;
        }

        private string GetFeatureUsername(int FNO, IGTApplication app)
        {
            //int affected = 0;
            //string sql = string.Format("SELECT * FROM G3E_FEATURE WHERE G3E_FNO='{0}'", FNO);
            //Recordset rs = app.DataContext.Execute(sql, out affected, 0, null);
            //if (!rs.EOF)
            //{
            //    rs.MoveFirst();
            //    return rs.Fields["G3E_USERNAME"].Value.ToString();
            //}

            //return "";


            if (m_Diag.IsEnabled(GTDiagCat.EE))
                m_Diag.LogEnter("AssignJobForm.GetFeatureUsername");

            foreach (FeatureType featureType in featureTypes)
            {
                if (featureType.FNO == FNO)
                {
                    if (m_Diag.IsEnabled(GTDiagCat.EE))
                        m_Diag.LogExit("AssignJobForm.GetFeatureUsername");
                    return featureType.Username;
                }
            }

            if (m_Diag.IsEnabled(GTDiagCat.EE))
                m_Diag.LogExit("AssignJobForm.GetFeatureUsername");

            return "";
        }


        private void gvFeatures_SelectionChanged(object sender, EventArgs e)
        {
            if (gvFeatures.SelectedRows.Count == 0)
                return;

            FeatureItem featureItem = (FeatureItem)gvFeatures.SelectedRows[0].DataBoundItem;
            IGTApplication app = GTClassFactory.Create<IGTApplication>();
            IGTKeyObject feature = app.DataContext.OpenFeature(featureItem.FNO, featureItem.FID);

            IGTWorldRange featureRange = null;
            foreach (IGTComponent comp in feature.Components)
            {
                if (comp.Geometry != null)
                {
                    if (comp.Geometry.Range != null)
                    {
                        featureRange = comp.Geometry.Range;
                        break;
                    }
                }
            }

            if (featureRange != null)
            {
                app.ActiveMapWindow.ZoomArea(featureRange);
            }
        }

        private void ShowRegenerationStatus(string status, string logFilename)
        {
            if (m_Diag.IsEnabled(GTDiagCat.LP1))
                m_Diag.LogMessage("AssignJobForm.ShowRegenerationStatus", GTDiagCatDesc.LP1, status);
            string statusWithTime = DateTime.Now.ToString("dd/MM/yyyy - hh:mm:ss") + " - " +
                status;
            txtRegenerateStatus.Text += "\r\n" + statusWithTime + "\r\n";
            txtRegenerateStatus.SelectionStart = txtRegenerateStatus.Text.Length;
            txtRegenerateStatus.ScrollToCaret();

            try
            {
                using (TextWriter writer = System.IO.File.AppendText(logFilename))
                {
                    writer.WriteLine(statusWithTime);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + ": " + logFilename);
                throw ex;
            }


            Refresh();
        }

        private string GetCombinationString(string boundaryFeatureType)
        {
            return boundaryFeatureType;
        }

        private void btnRegenerate_Click(object sender, EventArgs e)
        {

            if (m_Diag.IsEnabled(GTDiagCat.EE))
                m_Diag.LogEnter("AssignJobForm.btnRegenerate_Click");

            // recognize the selected feature types
            FeatureType selectedFeatureFT = (FeatureType)cbRegenFeatureFT.SelectedItem;
            short selectedFeatureFT_FNO = selectedFeatureFT.FNO;
            string selectedBoundaryFT = cbRegenBoundaryFT.SelectedItem.ToString();

            string SID = cbSIDs.SelectedItem.ToString();
            string username = txtUsername.Text;
            string password = txtPassword.Text;

            Regenerate(selectedBoundaryFT, SID, username, password);

            if (m_Diag.IsEnabled(GTDiagCat.EE))
                m_Diag.LogExit("AssignJobForm.btnRegenerate_Click");


        }

        private void Regenerate(string boundaryFT,
            string SID, string username, string password)
        {
            int totalBoundaries = 0;
            int dotCounter = 0;
            int boundaryCounter = 1;

            this.TopMost = false;

            if (m_Diag.IsEnabled(GTDiagCat.EE))
                m_Diag.LogEnter("AssignJobForm.Regenerate");

            // create CSV file and Log file
            string foldername = "c:\\NEPS.AssignJob Logs";
            if (!System.IO.Directory.Exists(foldername))
                System.IO.Directory.CreateDirectory(foldername);
            bool csvFileHasContent = false;
            string filenameSafeCombinationString = GetCombinationString(boundaryFT);
            string filename = foldername + "\\NEPS.AssignJob " + filenameSafeCombinationString;
            string csvFilename = filename + ".csv";
            string logFilename = filename + "_log.txt";
            // delete existing files
            if (System.IO.File.Exists(logFilename))
                System.IO.File.Delete(logFilename);
            if (System.IO.File.Exists(csvFilename))
                System.IO.File.Delete(csvFilename);

            IGTApplication app = GTClassFactory.Create<IGTApplication>();

            try
            {


                // first, delete all records
                int count = 0;
                string sql = "DELETE FROM AG_BDY_PAD WHERE COMBINATION='" +
                    GetCombinationString(boundaryFT) + "'";

                //DELETION STATEMENT REMOVED DUE TO "insufficient privilege error"
                //ShowRegenerationStatus("Deleting existing data: " + sql, logFilename);
                //app.DataContext.Execute(sql, out count, 0, null);

                // clear status
                txtRegenerateStatus.Text = "";

                // INITIALIZE LISTS
                List<RegenItem> boundaryItems = new List<RegenItem>();
                List<RegenItem> featureItems = new List<RegenItem>();

                using (System.IO.TextWriter csvFile = System.IO.File.CreateText(csvFilename))
                {
                    csvFile.WriteLine("\"BDY_FID\",\"FEA_FNO\",\"FEA_FID\"\"COMBINATION\"");
                }

                // OLEDB OPERATIONS
                string countSql;

                ShowRegenerationStatus("Beginning OleDB data operation. ", logFilename);
                using (OleDbConnection conn = UtilityDb.GetConnection(SID, username, password))
                {
                    /// BOUNDARIES

                    // if boundaryFT is an empty string also include null in the where clause
                    string whereFeatureType = " FEATURE_TYPE='" + boundaryFT + "' ";
                    if (string.IsNullOrEmpty(boundaryFT))
                        whereFeatureType = " (FEATURE_TYPE='' OR FEATURE_TYPE IS NULL) ";

                    string where = "from GC_BND_P WHERE G3E_FNO=" + FNO_BOUNDARY + " AND " + 
                       // " EX_EX_NAME like 'LTS%' AND " + // temporarily restrict to LTS only
                        whereFeatureType + " ORDER BY G3E_ID";

                    sql = "select G3E_FID, G3E_FNO " + where;
                    countSql = "SELECT COUNT(*) " + where;
                    ShowRegenerationStatus("Counting boundaries.", logFilename);
                    totalBoundaries = Convert.ToInt32(UtilityDb.ExecuteScalar(countSql, conn));

                    // get boundaries
                    boundaryCounter = 1;
                    dotCounter = 0;
                    ShowRegenerationStatus("Fetching " + totalBoundaries + " boundaries: " + sql, logFilename);
                    using (OleDbDataReader dr = UtilityDb.GetDataReader(sql, conn))
                    {
                        while (dr.Read())
                        {
                            RegenItem item = new RegenItem();
                            item.FID = Convert.ToInt32(dr["G3E_FID"]);
                            item.FNO = Convert.ToInt16(dr["G3E_FNO"]);
                            boundaryItems.Add(item);
                            RegenOutputDot(boundaryCounter, ref dotCounter);
                            boundaryCounter++;
                        }
                    }

                    // find output the features inside the boundaries
                    boundaryCounter = 1;
                    dotCounter = 0;
                    ShowRegenerationStatus("Determining contained features for " + totalBoundaries + " boundaries.", logFilename);
                    foreach (RegenItem boundaryItem in boundaryItems)
                    {
                        try
                        {
                            List<RegenItem> featuresContained = GetFeaturesContained(boundaryItem, app);
                            foreach (RegenItem feature in featuresContained)
                            {
                                using (System.IO.TextWriter csvFile = System.IO.File.AppendText(csvFilename))
                                {
                                    csvFile.WriteLine(string.Format("\"{0}\",\"{1}\",\"{2}\",\"{3}\"", boundaryItem.FID,
                                        feature.FNO, feature.FID,
                                        GetCombinationString(boundaryFT)));
                                    csvFileHasContent = true;
                                }

                                sql = string.Format(
                                    "INSERT INTO AG_BDY_PAD (BDY_FID, FEA_FNO, FEA_FID, COMBINATION) values ({0}, {1}, {2}, '{3}')",
                                    boundaryItem.FID, feature.FNO, feature.FID,
                                    GetCombinationString(boundaryFT));

                                UtilityDb.ExecuteSql(sql, conn);
                            }

                            RegenOutputDot(boundaryCounter, ref dotCounter);
                            boundaryCounter++;


                        }
                        catch (Exception ex)
                        {
                            boundaryCounter++;
                            ShowRegenerationStatus("Exception: " + ex.Message, logFilename);
                        }
                    }
                }

                // delete blank CSV file
                if (!csvFileHasContent)
                    System.IO.File.Delete(csvFilename);

                ShowRegenerationStatus("Regeneration is complete. A CSV file " + csvFilename + " has also been written.", logFilename);

            }
            catch (Exception functionException)
            {
                ShowRegenerationStatus("Regeneration terminated with exception: " + functionException.Message, logFilename);
            }

            this.TopMost = true;
        }

        private List<RegenItem> GetFeaturesContained(RegenItem boundaryItem, IGTApplication app)
        {
            List<RegenItem> output = new List<RegenItem>();
            IGTKeyObject objBoundary = app.DataContext.OpenFeature(boundaryItem.FNO, boundaryItem.FID);
            IGTComponent compBoundary = objBoundary.Components.GetComponent(CNO_BOUNDARY);
            IGTGeometry geometryBoundary = compBoundary.Geometry;

            // initialize spatial service
            IGTSpatialService oSS = GTClassFactory.Create<IGTSpatialService>();
            IGTKeyObjects oKO = GTClassFactory.Create<IGTKeyObjects>();
            IGTKeyObjects oFG = GTClassFactory.Create<IGTKeyObjects>();
            ADODB.Recordset rsAOI = new ADODB.Recordset();
            oSS.DataContext = app.DataContext;
            oSS.FilterGeometry = geometryBoundary;
            oSS.Operator = GTSpatialOperatorConstants.gtsoTouches;

            List<short> lstFNO = new List<short>();
            foreach (FeatureType featureType in featureTypes)
            {
                lstFNO.Add(featureType.FNO);
            }

            rsAOI = oSS.GetResultsByFNO(lstFNO.ToArray());
            if (!rsAOI.EOF)
            {
                rsAOI.MoveFirst();
                while (!rsAOI.EOF)
                {
                    RegenItem item = new RegenItem();
                    item.FNO = Convert.ToInt16(rsAOI.Fields["G3E_FNO"].Value);
                    item.FID = Convert.ToInt32(rsAOI.Fields["G3E_FID"].Value);

                    // skip boundaries within boundaries
                    if (item.FNO == 24000 || item.FNO == 8000)
                    {
                        rsAOI.MoveNext();
                        continue;
                    }

                    output.Add(item);
                    rsAOI.MoveNext();
                }
            }

            return output;
        }


        private void RegenOutputDot(int itemCounter, ref int dotCounter)
        {
            if (itemCounter % 100 == 0)
            {
                txtRegenerateStatus.Text += ".";
                if (dotCounter >= 100)
                {
                    dotCounter = 0;
                    txtRegenerateStatus.Text += "\r\n";
                }
                dotCounter++;
                Refresh();
            }
        }

        private void btnAdmin_Click(object sender, EventArgs e)
        {
            // resize to show regen tools
            Size sz = this.Size;
            sz.Width += 470;
            this.Size = sz;

            // show boundary feature types

            using (OleDbConnection conn = UtilityDb.GetConnection(cbSIDs.SelectedItem.ToString(), txtUsername.Text, txtPassword.Text))
            {
                string sql = "SELECT DISTINCT feature_type FROM GC_BND_P order by feature_type";
                using (OleDbDataReader dr = UtilityDb.GetDataReader(sql, conn))
                {
                    cbRegenBoundaryFT.Items.Clear();
                    while (dr.Read())
                    {
                        cbRegenBoundaryFT.Items.Add(dr["feature_type"].ToString());
                    }
                    cbRegenBoundaryFT.SelectedIndex = 0;
                }

                cbRegenFeatureFT.DataSource = featureTypes;
                cbRegenFeatureFT.SelectedIndex = 0;
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();

            if (Finished != null)
                Finished(null);
        }

    }


}