using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GDRespApproverMatrixMaintenance.Model
{
    public class RespApproverMap
    {
        public Int32? ID { get; set; }
        public string Instance { get; set; }
        public string Ap_Group { get; set; }
        public string Division { get; set; }
        public string Responsibility_Name { get; set; }
        public string Application { get; set; }
        public string Primary_Approver { get; set; }
        public string Secondary_Approver { get; set; }
        public string Final_Approver { get; set; }
        public string Comment { get; set; }
        public string Sod_Active { get; set; }
        public string Last_Updated_By { get; set; }
        public DateTime Last_Updated_Date { get; set; }

        public string Available_in_Production { get; set; }
        public string Active { get; set; }
        public DateTime? Start_Date { get; set; }
        public DateTime? End_Date { get; set; }
        public string Do_Not_Use { get; set; }
        public string Approvers_Not_Listed { get; set; }

        public string Default { get; set; }

        public string Error { get; set; }

        public int RowNum { get; set; }
        public string PriorPrimaryApprover { get; set; }
        public string PriorSecondaryApprover { get; set; }
        public string PriorFinalApprover { get; set; }
        public DateTime PriorPrimaryApproverUpdatedDate { get; set; }
        public DateTime PriorSecondaryApproverUpdatedDate { get; set; }
        public DateTime PriorFinalApproverUpdatedDate { get; set; }

    }
}
