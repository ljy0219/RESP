using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;

namespace GDRespApproverMatrixMaintenance.API.Utilities
{
    public static class Utility
    {
        public static void WriteLogs(string username, string activity)
        {
            string FolderPath = @"\ResponsibilityLogs\Logs";
            if (!Directory.Exists(FolderPath))
            {
                Directory.CreateDirectory(FolderPath);
            }

            string FileName = FolderPath + "\\log_" + DateTime.Today.ToString("yyyyMMdd") + ".txt";
            if (!File.Exists(FileName))
            {
                File.Create(FileName);
            }

            using (System.IO.StreamWriter file =
              new System.IO.StreamWriter(FileName, true))
            {
                file.WriteLine(username + " " + activity + " at " + DateTime.Now.ToString());
            }

        }
    }
}