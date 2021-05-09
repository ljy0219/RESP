using GDRespApproverMatrixMaintenance.Model;
using Microsoft.SqlServer.Server;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GDRespApproverMatrixMaintenance.DataAccess.Utilities
{
    public class RespApproverMapCollection : List<RespApproverMap>, IEnumerable<SqlDataRecord>
    {

        IEnumerator<SqlDataRecord> IEnumerable<SqlDataRecord>.GetEnumerator()
        {
            var sqlRow = new SqlDataRecord(
                  new SqlMetaData("Instance", SqlDbType.VarChar, 200),
                  new SqlMetaData("Ap_Group", SqlDbType.VarChar, 200),
                  new SqlMetaData("Division", SqlDbType.VarChar, 200),
                  new SqlMetaData("Responsibility_Name", SqlDbType.VarChar, 200),
                  new SqlMetaData("Application", SqlDbType.VarChar, 200),
                  new SqlMetaData("Primary_Approver", SqlDbType.VarChar, 1000),
                  new SqlMetaData("Secondary_Approver", SqlDbType.VarChar, 1000),
                  new SqlMetaData("Final_Approver", SqlDbType.VarChar, 1000),
                  new SqlMetaData("Comment", SqlDbType.VarChar, 1000),
                  new SqlMetaData("Sod_Active", SqlDbType.VarChar, 20),
                  new SqlMetaData("Last_Updated_By", SqlDbType.VarChar, 50),
                  new SqlMetaData("Do_Not_Use", SqlDbType.VarChar, 10),
                  new SqlMetaData("Default", SqlDbType.VarChar, 10),
                  new SqlMetaData("Error", SqlDbType.VarChar, 1000)
                  );

            foreach (RespApproverMap map in this)
            {
                sqlRow.SetValue(0, map.Instance);
                sqlRow.SetValue(1, map.Ap_Group);
                sqlRow.SetValue(2, map.Division);
                sqlRow.SetValue(3, map.Responsibility_Name);
                sqlRow.SetValue(4, map.Application);
                sqlRow.SetValue(5, map.Primary_Approver);
                sqlRow.SetValue(6, map.Secondary_Approver);
                sqlRow.SetValue(7, map.Final_Approver);
                sqlRow.SetValue(8, map.Comment);
                sqlRow.SetValue(9, map.Sod_Active);
                sqlRow.SetValue(10, map.Last_Updated_By);
                sqlRow.SetValue(11, map.Do_Not_Use);
                sqlRow.SetValue(12, map.Default);
                sqlRow.SetValue(13, map.Error);
                yield return sqlRow;
            }
        }

    }
}
