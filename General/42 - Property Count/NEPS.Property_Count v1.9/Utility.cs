using System;
using System.Collections.Generic;
using System.Text;
using Intergraph.GTechnology.API;
using ADODB;

namespace NEPS.GTechnology.Property_Count
{
    class Utility
    {
        public const int FNO_BOUNDARY = 24000;
        public static string CurrentJobName()
        {

            int affected = 0;
            IGTApplication app = GTClassFactory.Create<IGTApplication>();

            string jobID = app.DataContext.ActiveJob;
            string sql = string.Format("SELECT * FROM G3E_JOB WHERE UPPER(SCHEME_NAME)='{0}'", jobID.ToUpper());

            Recordset rs = app.DataContext.Execute(sql, out affected, 0, null);
            if (!rs.EOF)
            {
                rs.MoveFirst();
                return rs.Fields["G3E_IDENTIFIER"].Value.ToString();
            }

            return "";
        }

        /// <summary>
        /// Return all boundary FIDs ubder the active job
        /// </summary>
        /// <returns></returns>
        public static List<int> GetAllBoundaryFIDs()
        {
            IGTApplication app = GTClassFactory.Create<IGTApplication>();

            List<int> output = new List<int>();
            string sql = string.Format("SELECT * FROM gc_netelem WHERE UPPER(JOB_ID)='{0}' AND G3E_FNO={1} ",
                CurrentJobName(), FNO_BOUNDARY);

            int count = 0;
            Recordset rs = app.DataContext.Execute(sql, out count, 0, null);
            if (!rs.EOF)
            {
                rs.MoveFirst();
                while (!rs.EOF)
                {
                    int fid = Convert.ToInt32(rs.Fields["G3E_FID"].Value);
                    output.Add(fid);
                    rs.MoveNext();
                }
            }

            return output;

        }
    }
}
