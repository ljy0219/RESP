using GDRespApproverMatrixMaintenance.Model;
using Microsoft.SqlServer.Server;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GDRespApproverMatrixMaintenance.DataAccess
{
    public class HRISContactCollection : List<HRISContact>, IEnumerable<SqlDataRecord>
    {
        IEnumerator<SqlDataRecord> IEnumerable<SqlDataRecord>.GetEnumerator()
        {
            var sqlRow = new SqlDataRecord(
                  new SqlMetaData("RespID", SqlDbType.Int),
                  new SqlMetaData("Responsibility_Name", SqlDbType.VarChar, 100),
                  new SqlMetaData("Name/Type", SqlDbType.VarChar, 100),
                  new SqlMetaData("OU_Org_ID", SqlDbType.Int),
                  new SqlMetaData("Org_Name", SqlDbType.VarChar, 100),
                  new SqlMetaData("BG_ID", SqlDbType.Int),
                  new SqlMetaData("Business_Group", SqlDbType.VarChar, 100),
                  new SqlMetaData("Description", SqlDbType.VarChar, 100),
                  new SqlMetaData("Diff", SqlDbType.Bit),
                  new SqlMetaData("Contact1", SqlDbType.VarChar, 200),
                  new SqlMetaData("Contact2", SqlDbType.VarChar, 200),
                  new SqlMetaData("Interface_User_Name", SqlDbType.VarChar, 100),
                  new SqlMetaData("File_Name", SqlDbType.VarChar, 200),
                  new SqlMetaData("Comment", SqlDbType.VarChar, 500),
                  new SqlMetaData("Notes", SqlDbType.VarChar, 500),
                  new SqlMetaData("Attr2", SqlDbType.VarChar, 50)

                  );

            foreach (HRISContact cnt in this)
            {
                sqlRow.SetValue(0, cnt.RespID);
                sqlRow.SetValue(1, cnt.Responsibility_Name);
                sqlRow.SetValue(2, cnt.NameOrType);
                sqlRow.SetValue(3, cnt.OU_Org_ID);
                sqlRow.SetValue(4, cnt.Org_Name);
                sqlRow.SetValue(5, cnt.BG_ID);
                sqlRow.SetValue(6, cnt.Business_Group);
                sqlRow.SetValue(7, cnt.Description);
                sqlRow.SetValue(8, Convert.ToBoolean(cnt.Diff == null ? 0 : cnt.Diff));
                sqlRow.SetValue(9, cnt.Contact1);
                sqlRow.SetValue(10, cnt.Contact2);
                sqlRow.SetValue(11, cnt.Interface_User_Name);
                sqlRow.SetValue(12, cnt.File_Name);
                sqlRow.SetValue(13, cnt.Comment);
                sqlRow.SetValue(14, cnt.Notes);
                sqlRow.SetValue(15, cnt.Attr2);

                yield return sqlRow;
            }
        }

    }
}
