using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GDRespApproverMatrixMaintenance.Model;
using System.Data;
using Dapper;
using System.Data.SqlClient;
using System.Configuration;
using GDRespApproverMatrixMaintenance.DataAccess.Utilities;

namespace GDRespApproverMatrixMaintenance.DataAccess
{
    public class RespAppvMatrixDataAccess
    {

        private static string connectingstring = ConfigurationManager.ConnectionStrings["DBConnection"].ConnectionString;

        /// <summary>
        /// update single RespApproverMap, if ID exists then update, if ID not exists, then add.
        /// </summary>
        /// <param name="map"></param>
        /// <returns></returns>
        public static int UpdateRespApproverMap(RespApproverMap map,string UserName)
        {

            using (SqlConnection conn = new SqlConnection(connectingstring))
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("@EditType", "Manual", DbType.String);
                param.Add("@ID", map.ID, DbType.String);
                param.Add("@Instance", map.Instance, DbType.String);
                param.Add("@Ap_Group", string.IsNullOrEmpty(map.Ap_Group) ? map.Ap_Group : map.Ap_Group.Trim(), DbType.String);
                param.Add("@Division", string.IsNullOrEmpty(map.Division) ? map.Division : map.Division.Trim(), DbType.String);
                param.Add("@Responsibility_Name", string.IsNullOrEmpty(map.Responsibility_Name) ? map.Responsibility_Name : map.Responsibility_Name.Trim(), DbType.String);
                param.Add("@Application", string.IsNullOrEmpty(map.Application) ? map.Application : map.Application.Trim(), DbType.String);
                param.Add("@Primary_Approver", string.IsNullOrEmpty(map.Primary_Approver) ? map.Primary_Approver : map.Primary_Approver.Trim(), DbType.String);
                param.Add("@Secondary_Approver", string.IsNullOrEmpty(map.Secondary_Approver) ? map.Secondary_Approver : map.Secondary_Approver.Trim(), DbType.String);
                param.Add("@Final_Approver", string.IsNullOrEmpty(map.Final_Approver) ? map.Final_Approver : map.Final_Approver.Trim(), DbType.String);
                param.Add("@Comment", string.IsNullOrEmpty(map.Comment) ? map.Comment : map.Comment.Trim(), DbType.String);
                param.Add("@Sod_Active", string.IsNullOrEmpty(map.Sod_Active) ? map.Sod_Active : map.Sod_Active.Trim(), DbType.String);
                param.Add("@Last_Updated_By", map.Last_Updated_By, DbType.String);
                param.Add("@Return_ID", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);
                param.Add("@Do_Not_Use", map.Do_Not_Use, DbType.String);
                param.Add("@Approvers_Not_Listed", map.Approvers_Not_Listed, DbType.String);
                param.Add("@Default", map.Default, DbType.String);
                param.Add("@Updated_By", UserName, DbType.String);

                conn.Query("usp_RAMM_UpdateRespApproverMap", param, commandType: CommandType.StoredProcedure);

                return param.Get<int>("@Return_ID");
            }

        }

        public static DataSet UpdateRespApproverMapInBulk(List<RespApproverMap> list,string UserName)
        {
            string str = " truncate table Tbl_RAMM_Resp_Approver_Matrix_Upload ";
            StringBuilder sb = new StringBuilder();
            foreach (RespApproverMap item in list)
            {
                sb.Append("INSERT INTO [dbo].[Tbl_RAMM_Resp_Approver_Matrix_Upload]([Instance],[Ap_Group],[Division],[Responsibility_Name],[Application],[Primary_Approver],[Secondary_Approver],[Final_Approver],[Comment],[Sod_Active],[DelFlag],[Last_Updated_By],[Last_Updated_Date],[Do_Not_Use],[Approvers_Not_Listed],[Default])VALUES('" + item.Instance + "','" + item.Ap_Group + "','" + item.Division + "','" + item.Responsibility_Name + "','" + item.Application + "','" +(string.IsNullOrEmpty(item.Primary_Approver) ? "": item.Primary_Approver.Replace("'", "''")) + "','" + (string.IsNullOrEmpty(item.Secondary_Approver) ? "" : item.Secondary_Approver.Replace("'", "''")) + "','" + (string.IsNullOrEmpty(item.Final_Approver) ? "" : item.Final_Approver.Replace("'", "''")) + "','" + (string.IsNullOrEmpty(item.Comment) ? "" : item.Comment.Replace("'", "''")) + "','" + item.Sod_Active + "',null,'" + item.Last_Updated_By + "',GETDATE(),'" + item.Do_Not_Use + "',null,'" + item.Default + "');");
            }

            SqlConnection conn = new SqlConnection(connectingstring);
            conn.Open();
            SqlCommand comm_firststep = new SqlCommand(str, conn);
            SqlCommand comm_secondstep = new SqlCommand(sb.ToString(), conn);
            comm_secondstep.CommandTimeout = 0;
            SqlCommand cmd_thirdstep= new SqlCommand("usp_RAMM_UpdateRespApproverMapInBulk", conn);
            cmd_thirdstep.CommandTimeout = 0;
            cmd_thirdstep.CommandType = CommandType.StoredProcedure;
            cmd_thirdstep.Parameters.Add(new SqlParameter()
            {
                ParameterName = "@Updated_By",
                SqlDbType = SqlDbType.VarChar,
                Value = UserName
            });

            DataSet ds = new DataSet();

            try
            {
                comm_firststep.ExecuteNonQuery();
                comm_secondstep.ExecuteNonQuery();
                SqlDataAdapter adapter = new SqlDataAdapter(cmd_thirdstep);

                adapter.Fill(ds);
            }
            catch (Exception)
            {
            }
            finally
            {              
                conn.Close();
            }
            return ds;
        }

        public static void BackUpInvalidResp(List<RespApproverMap> list,string username)
        {
            StringBuilder sb = new StringBuilder();
            foreach (RespApproverMap item in list)
            {
                sb.Append("INSERT INTO [dbo].[Tbl_RAMM_Invalid_Resp]([Instance],[Ap_Group],[Division],[Responsibility_Name],[Application],[Primary_Approver],[Secondary_Approver],[Final_Approver],[Comment],[Sod_Active],[DelFlag],[Last_Updated_By],[Last_Updated_Date],[Do_Not_Use],[Approvers_Not_Listed],[Default],[Error])VALUES('" + item.Instance + "','" + item.Ap_Group + "','" + item.Division + "','" + item.Responsibility_Name + "','" + item.Application + "','" + (string.IsNullOrEmpty(item.Primary_Approver) ? "" : item.Primary_Approver.Replace("'", "''")) + "','" + (string.IsNullOrEmpty(item.Secondary_Approver) ? "" : item.Secondary_Approver.Replace("'", "''")) + "','" + (string.IsNullOrEmpty(item.Final_Approver) ? "" : item.Final_Approver.Replace("'", "''")) + "','" + (string.IsNullOrEmpty(item.Comment) ? "" : item.Comment.Replace("'", "''")) + "','" + item.Sod_Active + "',null,'" + username + "',GETDATE(),'" + item.Do_Not_Use + "',null,'" + item.Default + "','" + item.Error + "');");
            }
            using (SqlConnection conn = new SqlConnection(connectingstring))
            {
                RespApproverMapCollection collection = new RespApproverMapCollection();
                collection.AddRange(list);

                SqlCommand cmd = new SqlCommand(sb.ToString(), conn);
                cmd.CommandTimeout = 0;

                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }



        /// <summary>
        /// Before updating responsibility info to DB, the SP will check if Primary_Approver and Secondary_Approver are valid in GD. 
        /// </summary>
        /// <param name="primary_approver">Primary Approver email address</param>
        /// <param name="secondary_approver">secondary approver email address</param>
        /// <returns>If Primary_Approver is invalid, the return value will be 1. If Secondary_Approver is invalid, the return value will be 2. If both are invalid, the return value will be 3. If everything is fine, the return value will be 0.</returns>
        public static int CheckApprovers(string primary_approver, string secondary_approver)
        {
            using (SqlConnection conn = new SqlConnection(connectingstring))
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("Primary_Approver", primary_approver, DbType.String);
                param.Add("Secondary_Approver", secondary_approver, DbType.String);
                param.Add("Return", dbType: DbType.Int32, direction: ParameterDirection.Output, size: 1000);
                conn.Query("usp_RAMM_CheckApprovers", param, commandType: CommandType.StoredProcedure);
                return param.Get<Int32>("Return");
            }
        }


        public static DataTable CheckApprover(string str)
        {
            using (SqlConnection conn = new SqlConnection(connectingstring))
            {
                DataSet ds = new DataSet();

                SqlCommand cmd = new SqlCommand("usp_RAMM_CheckApprover", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter()
                {
                    ParameterName = "@Approvers",
                    SqlDbType = SqlDbType.NVarChar,
                    Value = str
                });


                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                conn.Open();
                sda.Fill(ds);
                sda.Dispose();
                cmd.Dispose();

                if (ds.Tables.Count > 0)
                {
                    return ds.Tables[0];
                }
                else
                {
                    return null;
                }
            }
        }


        public static IEnumerable<RespApproverMap> GetRespApproverMatrix(RespApproverMatrixQuery query)
        {
            using (SqlConnection conn = new SqlConnection(connectingstring))
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("@Instance", query.Instance, DbType.String);
                //param.Add("@Application", query.Application, DbType.String);
                param.Add("@Responsibility_Name", query.Responsibility_Name, DbType.String);
                param.Add("@Division", query.Division, DbType.String);
                param.Add("@Approver", query.Approver, DbType.String);
                param.Add("@PageIndex", query.Page_Index, DbType.Int32);
                param.Add("@PageSize", query.Page_Size, DbType.Int32);
                param.Add("@GetCountFlag", false, DbType.Boolean);
                param.Add("@DoNotUse", query.DoNotUse, DbType.String);
                param.Add("@Default", query.Default, DbType.String);
                return conn.Query<RespApproverMap>("usp_RAMM_GetResponsibilityMatrix", param, commandType: CommandType.StoredProcedure);
            }

        }

        public static Int32 GetRespApproverMatrixCount(RespApproverMatrixQuery query)
        {
            using (SqlConnection conn = new SqlConnection(connectingstring))
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("@Instance", query.Instance, DbType.String);
                //param.Add("@Application", query.Application, DbType.String);
                param.Add("@Responsibility_Name", query.Responsibility_Name, DbType.String);
                param.Add("@Division", query.Division, DbType.String);
                param.Add("@Approver", query.Approver, DbType.String);
                param.Add("@PageIndex", query.Page_Index, DbType.Int32);
                param.Add("@PageSize", query.Page_Size, DbType.Int32);
                param.Add("@GetCountFlag", true, DbType.Boolean);
                param.Add("@DoNotUse", query.DoNotUse, DbType.String);
                return conn.Query<Int32>("usp_RAMM_GetResponsibilityMatrix", param, commandType: CommandType.StoredProcedure).FirstOrDefault();
            }

        }

        public static RespApproverMap GetSingleRespApproverMap(Int32 id)
        {
            using (SqlConnection conn = new SqlConnection(connectingstring))
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("@id", id, DbType.Int32);
                return conn.Query<RespApproverMap>("usp_RAMM_GetSingleRespApproverMap", param, commandType: CommandType.StoredProcedure).FirstOrDefault();
            }
        }


        public static bool CheckRespApproverMapExists(RespApproverMap map)
        {
            using (SqlConnection conn = new SqlConnection(connectingstring))
            {
                var exists = false;
                DynamicParameters param = new DynamicParameters();
                param.Add("@Instance", map.Instance, DbType.String);
                //param.Add("@Ap_Group", map.Ap_Group, DbType.String);
                //param.Add("@Division", map.Division, DbType.String);
                param.Add("@Responsibility_Name", map.Responsibility_Name, DbType.String);
                param.Add("@Application", map.Application, DbType.String);

                IEnumerable<Int32?> idList = conn.Query<Int32?>("usp_RAMM_CheckRespApproverMapExists", param, commandType: CommandType.StoredProcedure);
                if (idList != null && idList.Count() > 0)
                {
                    if (map.ID == null || map.ID < 1)
                    {
                        exists = true;
                    }
                    else if (idList.Count() > 1)
                    {
                        exists = true;
                    }
                }

                return exists;
            }

        }

        /// <summary>
        /// If exists active user, then return user id list
        /// </summary>
        /// <param name="email_address"></param>
        /// <returns></returns>
        public static IEnumerable<Int32?> CheckIfUserExistsInGD(string email_address)
        {
            using (SqlConnection conn = new SqlConnection(connectingstring))
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("@Email_Address", email_address, DbType.String);

                return conn.Query<Int32?>("usp_CheckGDUserExistingByEmail", param, commandType: CommandType.StoredProcedure);
            }
        }


        public static Int32 DeleteRespApproverMap(int id,string UserName)
        {
            using (SqlConnection conn = new SqlConnection(connectingstring))
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("@id", id, DbType.Int32);
                param.Add("@Updated_By", UserName, DbType.String);
                return conn.Execute("usp_RAMM_DeleteRespApproverMap", param, commandType: CommandType.StoredProcedure);
            }
        }

        public static DataTable GetRespApproverMatrixDT(RespApproverMatrixQuery query)
        {
            using (SqlConnection conn = new SqlConnection(connectingstring))
            {
                DataSet ds = new DataSet();
                SqlParameter[] param = new SqlParameter[]{
                new SqlParameter("@Instance",query.Instance==null?"":query.Instance),
                new SqlParameter("@Responsibility_Name",query.Responsibility_Name==null?"":query.Responsibility_Name),
                new SqlParameter("@Division",query.Division==null?"":query.Division),
                new SqlParameter("@Approver",query.Approver==null?"":query.Approver),
                new SqlParameter("@PageIndex",query.Page_Index),
                new SqlParameter("@PageSize",query.Page_Size),
                new SqlParameter("@GetCountFlag",query.GetCountFlag),
                new SqlParameter("@DoNotUse",query.DoNotUse)
                };

                SqlCommand cmd = new SqlCommand("usp_RAMM_GetResponsibilityMatrix", conn);
                cmd.Parameters.AddRange(param);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 0;
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
    }
}
