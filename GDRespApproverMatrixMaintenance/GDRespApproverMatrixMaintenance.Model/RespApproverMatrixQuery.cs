using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GDRespApproverMatrixMaintenance.Model
{
  public  class RespApproverMatrixQuery
    {
      public string Instance { get; set; }
      public string Application { get; set; }
      public string Responsibility_Name { get; set; }
      public string Division { get; set; }
      public string Approver { get; set; }
      public int Page_Index { get; set; }
      public int Page_Size { get; set; }
      public bool GetCountFlag { get; set; }
      public string DoNotUse { get; set; }
      public string Default { get; set; }
    }
}
