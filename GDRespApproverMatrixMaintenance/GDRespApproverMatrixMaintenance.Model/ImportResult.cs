using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GDRespApproverMatrixMaintenance.Model
{
   public class ImportResult
    {
       public int TotalCount { get; set; }
       public int ImportedCount { get; set; }
       public int FailedCount { get; set; }
       public string FailedPath { get; set; }
    }
}
