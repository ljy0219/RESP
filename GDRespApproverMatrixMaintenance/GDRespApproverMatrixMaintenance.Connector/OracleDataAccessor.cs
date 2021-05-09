using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GDRespApproverMatrixMaintenance.Connector
{
    public class OracleDataAccessor
    {
        private string _conn_key { get; set; }
        public  string connection_key { get{return _conn_key;} set{ _conn_key=value;}}


        public  DataTable PullResponsibilities()
        {
            string plSql = ConfigurationManager.AppSettings.Get("PLSQL");
            DataSet dsResp = new DataSet();
            using (OracleConnection conn = new OracleConnection(ConfigurationManager.ConnectionStrings[connection_key].ConnectionString))
            {
                if (!string.IsNullOrEmpty(plSql))
                {
                    OracleCommand cmd = new OracleCommand(plSql, conn);
                    conn.Open();
                    OracleDataAdapter oda = new OracleDataAdapter(cmd);
                    oda.Fill(dsResp);
                    
                }
            }

            return dsResp == null && dsResp.Tables.Count > 0 ? null : dsResp.Tables[0];
        }

    }
}
