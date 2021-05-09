using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GDRespApproverMatrixMaintenance.Model
{
   public class HRISContact
    {
      public int RespID {get;set;}
      public string  Responsibility_Name {get;set;}
      public string NameOrType {get;set;}
      public int? OU_Org_ID {get;set;}
      public string Org_Name {get;set;}
      public int? BG_ID {get;set;}
      public string  Business_Group {get;set;}
      public string Description {get;set;}
      public int? Diff {get;set;}
      public string Contact1 {get;set;}
      public string Contact2 {get;set;}
      public string Interface_User_Name {get;set;}
      public string File_Name { get; set; }
      public string Comment { get; set; }
      public string  Notes {get;set;}
      public string  Attr2 {get;set;}
      public DateTime Updated_Date {get;set;}
      public string Last_Updated_By { get; set; }

      public int ID { get; set; }
    }
}
