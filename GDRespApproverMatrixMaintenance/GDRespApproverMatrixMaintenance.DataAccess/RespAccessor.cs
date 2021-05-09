using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GDRespApproverMatrixMaintenance.DataAccess
{
    public class RespAccessor
    {
        public static bool SyncResp(string resp_name, string app_name, object start_date, object end_date)
        {

            if (!string.IsNullOrEmpty(resp_name))
            {
                try
                {
                    using (IDbConnection conn = ConnectionFactory.CreateSqlConnection())
                    {
                        DynamicParameters param = new DynamicParameters();
                        param.Add("@App_Name", app_name, DbType.String);
                        param.Add("@Resp_Name", resp_name, DbType.String);
                        param.Add("@Start_Date", Convert.ToDateTime(start_date), DbType.DateTime);
                        param.Add("@End_Date", Convert.ToDateTime(end_date), DbType.DateTime);
                        param.Add("@Updated_By", "RAMM_Connector", DbType.String);
                        conn.Query("usp_RAMM_SyncResponsibility", param, commandType: CommandType.StoredProcedure);

                    }

                }
                catch (Exception ex)
                {

                    return false;
                }
            }

            return true;

        }
   
    
    
    
    }
}
