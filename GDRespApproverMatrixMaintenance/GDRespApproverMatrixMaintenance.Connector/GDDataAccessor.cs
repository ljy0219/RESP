using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace GDRespApproverMatrixMaintenance.Connector
{
    public class GDDataAccessor
    {
        public static bool SyncResp(string instance, object resp_name, object app_name, object start_date, object end_date)
        {

            if (resp_name != null)
            {
                try
                {
                    using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["DBConnection"].ConnectionString))
                    {

                        SqlCommand cmd = new SqlCommand("usp_RAMM_SyncResponsibility", conn);
                        cmd.CommandType = CommandType.StoredProcedure;

                        SqlParameter[] param = new SqlParameter[]{
                          new SqlParameter("@Instance", instance),
                          new SqlParameter("@App_Name",  Convert.ToString(app_name)),
                          new SqlParameter("@Resp_Name", Convert.ToString(resp_name)),
                          new SqlParameter("@Start_Date", start_date==null? DBNull.Value:start_date),
                          new SqlParameter("@End_Date", end_date==null? DBNull.Value: end_date),
                          new SqlParameter("@Updated_By", Assembly.GetCallingAssembly().GetName().Name),
                        };

                        cmd.Parameters.AddRange(param);

                        conn.Open();

                       int n= cmd.ExecuteNonQuery();
                    }

                }
                catch (Exception ex)
                {
                    Utility.WriteErrorLog(ex.Message, ex.StackTrace);
                    Utility.SendEmail(ConfigurationManager.AppSettings["ErrorEmailRecipients"].ToString(), "Error Occurred in " + Assembly.GetCallingAssembly().GetName().Name, ex.Message);
                    return false;
                }
            }

            return true;

        }

    }
}
