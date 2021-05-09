using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace GDRespApproverMatrixMaintenance.DataAccess
{
    public class OracleDataAccessor
    {
        private static string oracleConnection = ConfigurationManager.ConnectionStrings["OracleDBConnection"].ConnectionString;
        
        public static OracleDataReader PullResponsibilities()
        {
            string plSql = ConfigurationManager.AppSettings.Get("PLSQL");
            OracleDataReader odr = null;
            using (OracleConnection conn = new OracleConnection(oracleConnection))
            {
                if (!string.IsNullOrEmpty(plSql))
                {
                    OracleCommand cmd = new OracleCommand(plSql, conn);
                    odr = cmd.ExecuteReader();
                    cmd.Dispose();
                }
            }

            return odr;
        }

    }
}
