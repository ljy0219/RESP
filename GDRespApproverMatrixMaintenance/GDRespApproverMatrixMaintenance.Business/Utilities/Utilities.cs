using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Hosting;

namespace GDRespApproverMatrixMaintenance.Business.Utilities
{
    public class Utilities
    {
        public static void FileSaveAs(HttpPostedFile file, string file_type, string guid = "")
        {
            string filePath = GenerateImportedFilePath(file.FileName, file_type, guid);

            file.SaveAs(HttpContext.Current.Server.MapPath(filePath) );

        }

        private static string GenerateImportedFilePath(string file_name = "", string file_type = "", string guid = "")
        {
            string username = GetUserName();
            if (!string.IsNullOrEmpty(username) && username.LastIndexOf('\\') > 0)
            {
                username = username.Substring(username.LastIndexOf('\\') + 1);
            }

            string filePath = "/ImportedFiles";
            if (!Directory.Exists(HostingEnvironment.MapPath(filePath)))
            {
                Directory.CreateDirectory(HostingEnvironment.MapPath(filePath));
            }

            string fileExtension = ".xlsx";
            if (!string.IsNullOrEmpty(file_name))
            {
                fileExtension = file_name.Substring(file_name.LastIndexOf('.'));
            }

            filePath = filePath + "/"+ file_type + "_" + username + "_" + guid + fileExtension;
            int index = 1;
            while (File.Exists(HostingEnvironment.MapPath(filePath) ) )
            {
                filePath = filePath.Substring(0, filePath.LastIndexOf('.')) + "_" + index + fileExtension;
                index++;
            }
            return filePath;
        }

        public static string SaveDataSetToExcel(DataSet ds, string file_type, string guid = "")
        {
            string filePath = null;

            if (ds != null)
            {
                filePath = GenerateImportedFilePath("", file_type, guid);
                using (ExcelPackage package = new ExcelPackage(new FileInfo(HostingEnvironment.MapPath(filePath)  ) ))
                {
                    foreach (DataTable tbl in ds.Tables)
                    {
                        ExcelWorksheet sheet = package.Workbook.Worksheets.Add(tbl.TableName);
                        sheet.Cells["A1"].LoadFromDataTable(tbl, true);
                    }
                    package.Save();
                }
            }

            return filePath;
        }


        public static string GetUserName()
        {
            return HttpContext.Current.User.Identity.Name;
        }

        public static void WriteErrorLog(string err_msg,string stack_msg)
        {
            string filePath = "/ErrorLogs";
            string fileName = "ErrorLog" + System.DateTime.Now.ToString("yyyy_MM_dd") + ".txt";
            if (!System.IO.Directory.Exists(HostingEnvironment.MapPath(filePath)))
            {
                System.IO.Directory.CreateDirectory(HostingEnvironment.MapPath(filePath));
            }

            string fullPath = HostingEnvironment.MapPath("/") + filePath + "/" + fileName;
            if (!System.IO.File.Exists(fullPath))
            {
                System.IO.File.Create(fullPath).Close();
            }
            StreamWriter writer = new StreamWriter(fullPath, true);
            writer.WriteLine("--------Error Occured: "+System.DateTime.Now.ToString()+"----------------------------------");
            writer.WriteLine("Error Message: "+err_msg);
            writer.WriteLine("Stack Message: " + stack_msg);
            writer.WriteLine("-------------------------------------------------------------------------------------------");
            writer.WriteLine();
            writer.Close();

        }

        public static void WriteLog(string username, string activity)
        {
            string filePath = "/RespLogs";
            string fileName = "RespLog" + System.DateTime.Now.ToString("yyyy_MM_dd") + ".txt";
            if (!Directory.Exists(HostingEnvironment.MapPath(filePath)))
            {
                Directory.CreateDirectory(HostingEnvironment.MapPath(filePath));
            }

            string fullPath = HostingEnvironment.MapPath("/") + filePath + "/" + fileName;
            if (!File.Exists(fullPath))
            {
                File.Create(fullPath).Close();
            }

            using (StreamWriter sw =
              new StreamWriter(fullPath, true))
            {
                sw.WriteLine(username + " " + activity + " at " + DateTime.Now.ToString());
            }

        }
    }
}
