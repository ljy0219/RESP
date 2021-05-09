using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Oracle.ManagedDataAccess.Client;
using System.Configuration;
using System.Reflection;
using System.Data;

namespace GDRespApproverMatrixMaintenance.Connector
{
   public class RespConnector
    {

       public static bool SyncResponsibilities()
       {
           bool result = true;

           try
           {
               if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings.Get("OracleInstances")))
               {
                   var instanceArr = ConfigurationManager.AppSettings.Get("OracleInstances").Split('|');

                   for (int i = 0; i < instanceArr.Length; i++)
                   {
                       if (instanceArr[0].Contains(":"))
                       {
                           SyncByInstance(instanceArr[0].Split(':')[0], instanceArr[0].Split(':')[1]);
                       }
                   }
               }

           }
           catch (Exception ex)
           {
               Utility.WriteErrorLog(ex.Message, ex.StackTrace);
               Utility.SendEmail(ConfigurationManager.AppSettings["ErrorEmailRecipients"].ToString(), "Error Occurred in " + Assembly.GetCallingAssembly().GetName().Name, ex.Message);
               result = false;
           } 
           
           return result;

       }

       private static void SyncByInstance(string instance, string connkey)
       {
           string app_name, resp_name, start_date, end_date;
           OracleDataAccessor oda = new OracleDataAccessor();
           oda.connection_key = connkey;
           DataTable dtResp = oda.PullResponsibilities();

           
           
           if (dtResp !=null)
           {
               //int i = 0;
               foreach(DataRow  row in dtResp.Rows)
               {
                   //app_name =row["Application_Name"]==null ? null : row["Application_Name"].ToString();

                   //resp_name = Convert.IsDBNull(row["Responsibility_Name"]) ? null : row["Responsibility_Name"].ToString();

                   //start_date = Convert.IsDBNull(row["Start_date"]) ? null : row["Start_date"].ToString();


                   //end_date = Convert.IsDBNull(row["End_Date"]) ? null : row["End_Date"].ToString();
                   //if (start_date == null || end_date == null)
                   //{
                   //    start_date = "";
                   //}

                   //GDDataAccessor.SyncResp(instance, resp_name, app_name, row["Start_date"], row["End_Date"]);
                   GDDataAccessor.SyncResp(instance, row["Responsibility_Name"], row["Application_Name"], row["Start_date"], row["End_Date"]);

               }
           }
       }
    }
}
