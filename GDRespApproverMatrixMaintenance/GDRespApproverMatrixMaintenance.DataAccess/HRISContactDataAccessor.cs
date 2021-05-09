using Dapper;
using GDRespApproverMatrixMaintenance.Model;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GDRespApproverMatrixMaintenance.DataAccess
{
    public class HRISContactDataAccessor
    {
        private static string connectingstring = ConfigurationManager.ConnectionStrings["DBConnection"].ConnectionString;

        public static int UpdateHRISContact(HRISContact contact,string EditType)
        {
            using (SqlConnection conn = new SqlConnection(connectingstring))
            {
                try
                {
                    DynamicParameters param = new DynamicParameters();
                    param.Add("@EditType", EditType, DbType.String);
                    param.Add("@ID", contact.ID, DbType.Int32);
                    param.Add("@RespID", contact.RespID, DbType.Int32);
                    param.Add("@Responsibility_Name", contact.Responsibility_Name, DbType.String);
                    param.Add("@NameOrType", contact.NameOrType, DbType.String);
                    param.Add("@OU_Org_ID", contact.OU_Org_ID, DbType.Int32);
                    param.Add("@Org_Name", contact.Org_Name, DbType.String);
                    param.Add("@BG_ID", contact.BG_ID, DbType.Int32);
                    param.Add("@Business_Group", contact.Business_Group, DbType.String);
                    param.Add("@Description", contact.Description, DbType.String);
                    param.Add("@Diff", contact.Diff, DbType.Boolean);
                    param.Add("@Contact1", contact.Contact1, DbType.String);
                    param.Add("@Contact2", contact.@Contact2, DbType.String);
                    param.Add("@Interface_User_Name", contact.Interface_User_Name, DbType.String);
                    param.Add("@File_Name", contact.File_Name, DbType.String);
                    param.Add("@Comment", contact.Comment, DbType.String);
                    param.Add("@Notes", contact.Notes, DbType.String);
                    param.Add("@Attr2", contact.Attr2, DbType.String);
                    param.Add("@Last_Updated_By", contact.Last_Updated_By, DbType.String);
                    param.Add("@Return_ID", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);
                    conn.Query("usp_RAMM_UpdateHRISContact", param, commandType: CommandType.StoredProcedure);
                    return param.Get<int>("@Return_ID");
                }
                catch (Exception ex)
                {

                    throw ex;
                }
            }
        }



        public static int UpdateHRISContactInBulk(List<HRISContact> list, string updated_By)
        {
            string str = "truncate table [Tbl_RAMM_HRIS_Contact_Upload];";
            StringBuilder sb = new StringBuilder();
            foreach (HRISContact item in list)
            {
                sb.Append("INSERT INTO [dbo].[Tbl_RAMM_HRIS_Contact_Upload]([RespID],[Responsibility_Name],[Name/Type],[OU_Org_ID],[Org_Name],[BG_ID],[Business_Group],[Description],[Diff],[Contact1],[Contact2],[Interface_User_Name],[File_Name],[Comment],[Notes],[Attr2],[DelFlag],[Updated_Date],[Last_Updated_By])VALUES('");
                sb.Append(item.RespID);
                sb.Append("','");
                sb.Append(item.Responsibility_Name);
                sb.Append("','");
                sb.Append(item.NameOrType);
                sb.Append("','");
                sb.Append(item.OU_Org_ID);
                sb.Append("','");
                sb.Append(item.Org_Name);
                sb.Append("','");
                sb.Append(item.BG_ID);
                sb.Append("','");
                sb.Append(item.Business_Group);
                sb.Append("','");
                sb.Append(string.IsNullOrWhiteSpace(item.Description) ? "" : item.Description.Replace("'", "''"));
                sb.Append("','");
                sb.Append(item.Diff);
                sb.Append("','");
                sb.Append(string.IsNullOrWhiteSpace(item.Contact1) ? "" : item.Contact1.Replace("'","''"));
                sb.Append("','");
                sb.Append(string.IsNullOrWhiteSpace(item.Contact2) ? "" : item.Contact2.Replace("'", "''"));
                sb.Append("','");
                sb.Append(item.Interface_User_Name);
                sb.Append("','");
                sb.Append(item.File_Name);
                sb.Append("','");
                sb.Append(string.IsNullOrWhiteSpace(item.Comment) ? "" : item.Comment.Replace("'", "''"));
                sb.Append("','");
                sb.Append(string.IsNullOrWhiteSpace(item.Notes) ? "" : item.Notes.Replace("'", "''"));
                sb.Append("','");
                sb.Append(item.Attr2);
                sb.Append("',null,GETDATE(),'");
                sb.Append(updated_By);
                sb.Append("');");
            }
            using (SqlConnection conn = new SqlConnection(connectingstring))
            {
                conn.Open();

                SqlCommand comm_firststep = new SqlCommand(str, conn);
                comm_firststep.ExecuteNonQuery();

                SqlCommand comm_secondstep = new SqlCommand(sb.ToString(), conn);
                comm_secondstep.CommandTimeout = 0;
                comm_secondstep.ExecuteNonQuery();

                SqlCommand cmd_thirdstep = new SqlCommand("usp_RAMM_UpdateHRISContactInBulk", conn);
                cmd_thirdstep.CommandType = CommandType.StoredProcedure;
                cmd_thirdstep.CommandTimeout = 0;

                return cmd_thirdstep.ExecuteNonQuery();

            }
        }

        public static IEnumerable<HRISContact> GetHRISContactList(HRISContactQuery query)
        {
            using (SqlConnection conn = new SqlConnection(connectingstring))
            {
                try
                {
                    DynamicParameters param = new DynamicParameters();
                    param.Add("@Responsibility_Name", query.Responsibility_Name, DbType.String, size: 200);
                    param.Add("@Contact", query.Contact, DbType.String,size: 200);
                    param.Add("@Org_Name", query.Org_Name, DbType.String, size: 200);
                    param.Add("@PageIndex", query.Page_Index, DbType.Int32);
                    param.Add("@PageSize", query.Page_Size, DbType.String);
                    param.Add("@GetCountFlag", false, DbType.String);
                    return conn.Query<HRISContact>("usp_RAMM_GetHRISContactList", param, commandType: CommandType.StoredProcedure);
                }
                catch (Exception ex)
                {
                    return null;
                }
            }
        }


        public static int GetHRISContactCount(HRISContactQuery query)
        {
            using (SqlConnection conn = new SqlConnection(connectingstring))
            {
                try
                {
                    DynamicParameters param = new DynamicParameters();
                    param.Add("@Responsibility_Name", query.Responsibility_Name, DbType.String, size: 200);
                    param.Add("@Contact", query.Contact, DbType.String, size: 200);
                    param.Add("@Org_Name", query.Org_Name, DbType.String, size: 200);
                    param.Add("@PageIndex", query.Page_Index, DbType.Int32);
                    param.Add("@PageSize", query.Page_Size, DbType.String);
                    param.Add("@GetCountFlag", true, DbType.String);
                    return conn.Query<int>("usp_RAMM_GetHRISContactList", param, commandType: CommandType.StoredProcedure).FirstOrDefault();
                }
                catch (Exception ex)
                {
                    return 0;
                }
            }
        }

        public static DataTable GetHRISContactDT(HRISContactQuery query)
        {
            using (SqlConnection conn = new SqlConnection(connectingstring))
            {
                DataSet ds = new DataSet();
                SqlParameter[] param = new SqlParameter[]{
                new SqlParameter("@Responsibility_Name",query.Responsibility_Name==null?"":query.Responsibility_Name),
                new SqlParameter("@Contact",query.Contact==null?"":query.Contact),
                new SqlParameter("@Org_Name",query.Org_Name==null?"":query.Org_Name),
                new SqlParameter("@PageIndex",query.Page_Index),
                new SqlParameter("@PageSize",query.Page_Size),
                new SqlParameter("@GetCountFlag",query.GetCountFlag)
                };

                SqlCommand cmd = new SqlCommand("usp_RAMM_GetHRISContactList", conn);
                cmd.Parameters.AddRange(param);
                cmd.CommandType = CommandType.StoredProcedure;
                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                conn.Open();
                sda.Fill(ds);

                if (ds.Tables.Count > 0)
                {
                    return ds.Tables[0];
                }
                return null;
            }
        }


        public static HRISContact GetSingleHRISContact(int id)
        {
            using (SqlConnection conn = new SqlConnection(connectingstring))
            {
                try
                {
                    DynamicParameters param = new DynamicParameters();
                    param.Add("@ID", id, DbType.Int32);
                    return conn.Query<HRISContact>("usp_RAMM_GetSingleHRISContact", param, commandType: CommandType.StoredProcedure).FirstOrDefault();
                }
                catch (Exception ex)
                {
                    return null;
                }
            }
        }

        public static int DeleteHRISContact(int id,string UserName)
        {
            using (SqlConnection conn = new SqlConnection(connectingstring))
            {
                try
                {
                    DynamicParameters param = new DynamicParameters();
                    param.Add("@ID", id, DbType.Int32);
                    param.Add("@UserName", UserName, DbType.String);
                    return conn.Execute("usp_RAMM_DeleteHRISContact", param, commandType: CommandType.StoredProcedure);
                }
                catch (Exception ex)
                {
                    return 0;
                }
            }
        }

        public static int CheckHRISContactExisting(int respID, string resp_name)
        {

            using (SqlConnection conn = new SqlConnection(connectingstring))
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("@RespID", respID, DbType.Int32);
                param.Add("@Responsibility_Name", resp_name, DbType.String);
                return conn.ExecuteScalar<int>("usp_RAMM_CheckHRISContactExisting", param, commandType: CommandType.StoredProcedure);
            }

        }
    }
}
