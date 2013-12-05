using System;
using System.Collections.Generic;
using System.Text;
using Intergraph.GTechnology.API;
using ADODB;

namespace NEPS.GTechnology.WallNumberReorder
{
    class Utility
    {
        public const short FNO_DuctPath = 2200;

        internal static void GetDuctpaths(IGTDataContext dataContext, short SourceFeatureFNO, int SourceFeatureFID, out List<int> startingDuctPaths, out List<int> terminatingDuctPaths)
        {
            startingDuctPaths = new List<int>();
            terminatingDuctPaths = new List<int>();

            // find the node ID of the manhole first
            int recordsAffected=0;
            string sql = "SELECT * FROM GC_NE_CONNECT WHERE G3E_FID=" + SourceFeatureFID + " AND G3E_FNO=" + SourceFeatureFNO;
            Recordset rs = dataContext.Execute(sql, out recordsAffected, 0, null);

            int SourceFeatureID = 0;
            if (!rs.EOF)
                SourceFeatureID = Convert.ToInt32(rs.Fields["NODE1_ID"].Value);
            rs.Close();
            if (SourceFeatureID == 0)
                return;
            // starting duct paths
            sql = "SELECT * FROM GC_NE_CONNECT WHERE NODE1_ID=" + SourceFeatureID + " AND G3E_FNO=" + FNO_DuctPath;
            rs = dataContext.Execute(sql, out recordsAffected, 0, null);
            while (!rs.EOF)
            {
                startingDuctPaths.Add(Convert.ToInt32(rs.Fields["G3E_FID"].Value));
                rs.MoveNext();
            }
            rs.Close();

            // terminating duct paths
            sql = "SELECT * FROM GC_NE_CONNECT WHERE NODE2_ID=" + SourceFeatureID + " AND G3E_FNO=" + FNO_DuctPath;
            rs = dataContext.Execute(sql, out recordsAffected, 0, null);
            while (!rs.EOF)
            {
                terminatingDuctPaths.Add(Convert.ToInt32(rs.Fields["G3E_FID"].Value));
                rs.MoveNext();
            }
            rs.Close();

        }
    }
}
