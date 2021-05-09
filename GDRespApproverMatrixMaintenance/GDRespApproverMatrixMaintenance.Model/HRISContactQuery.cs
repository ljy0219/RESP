using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GDRespApproverMatrixMaintenance.Model
{
   public class HRISContactQuery
    {
        public string Responsibility_Name { get; set; }
        public string Contact { get; set; }
        public string Org_Name { get; set; }
        public int Page_Index { get; set; }
        public int Page_Size { get; set; }
        public bool GetCountFlag { get; set; }

    }
}
