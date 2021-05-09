using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GDRespApproverMatrixMaintenance.DataAccess;
using Oracle.ManagedDataAccess.Client;


namespace GDRespApproverMatrixMaintenance.Business
{
    public class ConnectorBusiness
    {

        public static bool SyncResponsibilities()
        {
            bool result = false;
            string app_name,resp_name, start_date, end_date;
            try
            {

                OracleDataReader dr = OracleDataAccessor.PullResponsibilities();
                if (dr != null && dr.HasRows)
                {
                    while (dr.Read())
                    {
                        app_name = Convert.IsDBNull(dr["Application_Name"]) ? null : dr["Application_Name"].ToString();

                        resp_name = Convert.IsDBNull(dr["Responsibility_Name"]) ? null : dr["Responsibility_Name"].ToString();

                        start_date = Convert.IsDBNull(dr["Start_date"]) ? null : dr["Start_date"].ToString();


                        end_date = Convert.IsDBNull(dr["End_Date"]) ? null : dr["End_Date"].ToString();
                        if (start_date == null || end_date == null)
                        {
                            start_date = "";
                        }

                        RespAccessor.SyncResp(resp_name, app_name, dr["Start_date"], dr["End_Date"]);

                    }
                }
            }
            catch (Exception ex)
            {
                Utilities.Utilities.WriteErrorLog(ex.Message, ex.StackTrace);
            }
            return result;

        }
    }
}
