using System;
using System.Collections.Generic;
using System.Text;
using Intergraph.GTechnology.API;
using Intergraph.GTechnology.Interfaces;
using System.Windows.Forms;
using ADODB;
using Intergraph.GTechnology.Diagnostics;

namespace NEPS.GTechnology.AssignJob
{
    public class GTAssignJob : PlacementBase
    {

        AssignJobForm form = new AssignJobForm();

        string activeJob;
        string activeJobName;

        const int FNO_BOUNDARY = 24000;
        protected override void OnActivate()
        {
            string argument="";
            if (app.Properties["AutoCustomCommandArgument"] != null)
            {
                argument = app.Properties["AutoCustomCommandArgument"].ToString();
                if (argument == "0")
                    argument = "";
            }

            if (!string.IsNullOrEmpty(argument))
                app.ExitRequested = true;

            if (m_Diag.IsEnabled(GTDiagCat.EE))
                m_Diag.LogEnter("AssignJob.OnActivate");

            // if not from a batch job
            if (string.IsNullOrEmpty(argument))
            {
                activeJob = app.DataContext.ActiveJob;
                activeJobName = JobIDToName(activeJob);

                IGTKeyObject obj = ForcePreselectedFeature(FNO_BOUNDARY, "Area Boundary");
                if (obj == null)
                {
                    AllPlacementFinished(true);
                    return;
                }

                form.PreselectedBoundary = obj;
            }

            form.Finished += new AssignJobForm.FinishedHandler(form_Finished);
            form.m_Diag = m_Diag;
            form.transactionManager = this.transactionManager;
            form.argument = argument;
            form.ShowDialog();

            if (m_Diag.IsEnabled(GTDiagCat.EE))
                m_Diag.LogExit("AssignJob.OnActivate");

        }

        protected override void FinishSelectingObject(IGTKeyObject selectedObject)
        {

        }

        void form_Finished(AssignJobForm.FinishedEventarg arg)
        {
            if (m_Diag.IsEnabled(GTDiagCat.EE))
                m_Diag.LogEnter("AssignJob.form_Finished");


            AllPlacementFinished(false);

            if (m_Diag.IsEnabled(GTDiagCat.EE))
                m_Diag.LogEnter("AssignJob.form_Finished");


        }




        protected const int CNO_NETELEM = 51;
        //protected const string unassignedJobName = "Mig_Job";
        //protected const string unassignedFeatureState = "PPF";
        protected override void OnMouseUp(GTMouseEventArgs args)
        {


        }

        public string GetFeatureUsername(int FNO, IGTApplication app)
        {
            if (m_Diag.IsEnabled(GTDiagCat.EE))
                m_Diag.LogEnter("AssignJob.GetFeatureUsername");

            if (m_Diag.IsEnabled(GTDiagCat.IV1))
                m_Diag.LogValue("AssignJob.GetFeatureUsername", GTDiagCatDesc.IV1, "FNO", FNO);

            int affected = 0;
            string sql = string.Format("SELECT * FROM G3E_FEATURE WHERE G3E_FNO='{0}'", FNO);
            Recordset rs = app.DataContext.Execute(sql, out affected, 0, null);
            if (!rs.EOF)
            {
                rs.MoveFirst();
                return rs.Fields["G3E_USERNAME"].Value.ToString();
            }

            if (m_Diag.IsEnabled(GTDiagCat.EE))
                m_Diag.LogEnter("AssignJob.GetFeatureUsername");

            return "";
        }

        protected void UpdateFeatureJob(int FID, string jobName)
        {
            if (m_Diag.IsEnabled(GTDiagCat.EE))
                m_Diag.LogEnter("AssignJob.UpdateFeatureJob");

            int affected = 0;
            string sql = string.Format("UPDATE B$G3E_NETELEM SET JOB_ID='{0}' WHERE G3E_FID={1}", jobName, FID);
            app.DataContext.Execute(sql, out affected, 0, null);

            if (m_Diag.IsEnabled(GTDiagCat.EE))
                m_Diag.LogExit("AssignJob.UpdateFeatureJob");

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





    }
}
