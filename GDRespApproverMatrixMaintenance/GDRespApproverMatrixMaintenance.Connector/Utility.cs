using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace GDRespApproverMatrixMaintenance.Connector
{
   public class Utility
    {

       public static bool SendEmail(string toList, string subject, string body)
       {
           try
           {
               MailMessage objMail = new MailMessage();

               string[] arrTo = toList.Split(';');
               for (int i = 0; i < arrTo.Length; i++)
               {
                   if (arrTo[i] != null && arrTo[i].Trim().Length > 0)
                   {
                       objMail.To.Add(arrTo[i].Trim());
                   }
               }
               SmtpClient objSMTP = new SmtpClient(System.Configuration.ConfigurationManager.AppSettings["SmtpHost"]);
               string emailFrom = ConfigurationManager.AppSettings["EmailFrom"];
               string fromDisplayName = ConfigurationManager.AppSettings["EmailDisplayName"];
               MailAddress maFrom = new MailAddress(emailFrom, fromDisplayName);
               objMail.From = maFrom;

               objMail.Subject = subject;
               objMail.Body = body;
               objMail.IsBodyHtml = true;
               objMail.Priority = MailPriority.High;
               objSMTP.Send(objMail);

               maFrom = null;
               objMail = null;
               objSMTP = null;

           }
           catch (Exception ex)
           {
               return false;
           }
           return true;
       }


       public static void WriteErrorLog(string err_msg, string stack_msg)
       {
           string LogDir = ConfigurationManager.AppSettings["ErrorLogDir"];
           string filePath = Path.Combine(Environment.CurrentDirectory, LogDir);
           string fileName = "ErrorLog_" + System.DateTime.Now.ToString("yyyy_MM_dd") + ".txt";
           if (!System.IO.Directory.Exists(filePath))
           {
               System.IO.Directory.CreateDirectory(filePath);
           }

           string fullPath = filePath + "/" + fileName;
           if (!System.IO.File.Exists(fullPath))
           {
               System.IO.File.Create(fullPath).Close();
           }
           StreamWriter writer = new StreamWriter(fullPath, true);
           writer.WriteLine("--------Error Occured: " + System.DateTime.Now.ToString() + "----------------------------------");
           writer.WriteLine("Error Message: " + err_msg);
           writer.WriteLine("Stack Message: " + stack_msg);
           writer.WriteLine("-------------------------------------------------------------------------------------------");
           writer.WriteLine();
           writer.Close();

       }



    }
}
