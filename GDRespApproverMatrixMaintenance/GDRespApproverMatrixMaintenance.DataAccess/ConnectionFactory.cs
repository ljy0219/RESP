using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GDRespApproverMatrixMaintenance.DataAccess
{
    public class ConnectionFactory
    {
        private static readonly string sqlConnectionString = ConfigurationManager.ConnectionStrings["DBConnection"].ConnectionString;

        private static readonly string oracleConnectionString =  ConfigurationManager.ConnectionStrings["OracleDBConnection"].ConnectionString;

        public static SqlConnection CreateSqlConnection()
        {
            SqlConnection conn = new SqlConnection(sqlConnectionString);
            conn.Open();
            return conn;
        }




        public static IDbConnection CreateOracleConnection()
        {
            IDbConnection conn = new OracleConnection(sqlConnectionString);
            conn.Open();
            return conn;
        }
    }
}
