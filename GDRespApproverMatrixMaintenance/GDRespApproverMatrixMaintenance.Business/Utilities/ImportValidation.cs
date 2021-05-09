using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GDRespApproverMatrixMaintenance.Business
{
   public class ImportValidation
    {
       public static void ValidateRequiredField(DataTable dt, string fieldName)
       {
           var result = (from r in dt.AsEnumerable()
                         where r.Field<object>(fieldName) == null || Convert.ToString( r.Field<object>(fieldName)).Trim() == ""
                         select r);
           if (result != null && result.Count() > 0)
           {
               DataTable filter_dt = result.CopyToDataTable();
               foreach (DataRow row in filter_dt.Rows)
               {
                   if (row["Errors"] != null && row["Errors"].ToString().Length > 0)
                   {
                       row["Errors"] += ", " + fieldName.Replace("_"," ");
                   }
                   else
                   {
                       row["Errors"] = "Missing Field(s): " + fieldName.Replace("_", " ");
                   }
               }
               dt.Merge(filter_dt);
           }
       }

    }
}
