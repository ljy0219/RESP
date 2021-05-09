using ExcelDataReader;
using GDRespApproverMatrixMaintenance.DataAccess;
using GDRespApproverMatrixMaintenance.Model;
using OfficeOpenXml;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.Caching;
using System.Security.Permissions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;


namespace GDRespApproverMatrixMaintenance.Business
{
    public class ResponsibilityBusiness
    {

        private static Thread ImportingThread { get; set; }

        public static bool ImportingLocked { get; set; }

        private static MemoryCache _cache = MemoryCache.Default;

        public static async Task<HttpResponseObject<ImportResult>> AsyncImportRespFromExcel(Stream stream)
        {
            var task = await Task.Run(() =>
            {
                return ImportRespFromExcel(stream);
            });

            return task;
        }

        public static HttpResponseObject<ImportResult> ImportRespFromExcel(Stream stream)
        {
            DataSet returnedDS = null;
            DataSet invalidDS = null;
            RespApproverMap map = null;
            ImportResult result = new ImportResult();

            ImportingThread = Thread.CurrentThread;
            ImportingLocked = true;
            try
            {
                IExcelDataReader reader;
                reader = ExcelReaderFactory.CreateReader(stream);

                if (reader != null)
                {
                    DataSet ds = reader.AsDataSet(new ExcelDataSetConfiguration()
                    {
                        ConfigureDataTable = (tablereader) => new ExcelDataTableConfiguration()
                        {
                            UseHeaderRow = true
                        }
                    });

                    string ImportTime = DateTime.Now.ToString("yyyyMMddHHmmss");
                    Utilities.Utilities.SaveDataSetToExcel(ds, "Responsibiities", ImportTime);

                    List<RespApproverMap> respList = new List<RespApproverMap>();
                    #region traverse datatable
                    foreach (DataTable dt in ds.Tables)
                    {
                        string instance = dt.TableName.Trim().ToUpper();
                        if ((instance == "BETSY" || instance == "BILLY" || instance == "BRYAN" || instance == "DONOTUSE") && dt.Rows.Count >0)
                        {
                            //result.TotalCount += dt.Rows.Count;

                            DataTable temp_dt = dt.Copy();
                            dt.Clear();
                            DataTableReader dr = temp_dt.CreateDataReader();

                            DataColumn col = new DataColumn("ID", typeof(Int32));
                            dt.Columns.Add(col);
                            col.AutoIncrement = true;
                            col.AutoIncrementSeed = 1;
                            col.AutoIncrementStep = 1;

                            dt.PrimaryKey = new DataColumn[]{
                                dt.Columns["ID"]
                            };

                            dt.Load(dr);

                            dt.Columns.Add("Errors", type: typeof(string));

                            temp_dt = null;

                            #region Input Validation
                            RespImportValidation(dt);
                            #endregion


                            DataRow[] validRows = dt.Select("Errors is null");

                            foreach (DataRow row in validRows)
                            {
                                map = new RespApproverMap();
                                map.Instance = instance.Equals("DONOTUSE",StringComparison.OrdinalIgnoreCase) ? "BETSY" : instance;
                                map.Ap_Group = string.IsNullOrEmpty(row["Ap Group"] as string) ? row["Ap Group"] as string : (row["Ap Group"] as string).Trim() ;
                                map.Division = string.IsNullOrEmpty(row["Division"] as string) ? row["Division"] as string : (row["Division"] as string).Trim();
                                map.Responsibility_Name = string.IsNullOrEmpty(row["Responsibility Name"] as string) ? row["Responsibility Name"] as string : (row["Responsibility Name"] as string).Trim();
                                map.Application = string.IsNullOrEmpty(row["Application"] as string) ? row["Application"] as string : (row["Application"] as string).Trim();
                                map.Primary_Approver = string.IsNullOrEmpty(row["Primary Approver"] as string) ? row["Primary Approver"] as string : (row["Primary Approver"] as string).Trim();
                                map.Secondary_Approver = string.IsNullOrEmpty(row["Secondary Approver"] as string) ? row["Secondary Approver"] as string : (row["Secondary Approver"] as string).Trim();
                                map.Final_Approver = string.IsNullOrEmpty(row["Final Approver"] as string) ? row["Final Approver"] as string : (row["Final Approver"] as string).Trim();
                                map.Comment = string.IsNullOrEmpty(row["Comment"] as string) ? row["Comment"] as string : (row["Comment"] as string).Trim();
                                map.Sod_Active = string.IsNullOrEmpty(row["Sod Active"] as string) ? row["Sod Active"] as string : (row["Sod Active"] as string).Trim();
                                map.Last_Updated_By = Utilities.Utilities.GetUserName();
                                map.Do_Not_Use = instance.Equals("DONOTUSE", StringComparison.OrdinalIgnoreCase) ? "Yes" : "No";
                                if (string.Compare((row["Default"] as string), "Yes", true) == 0)
                                {
                                    map.Default = "Yes";
                                }
                                else { map.Default = "No"; }

                                respList.Add(map);
                            }

                            if (dt.Select("Errors is not null").Count() > 0)
                            {
                                var ddt = dt.Select("Errors is not null").CopyToDataTable();

                                ddt.TableName = instance;
                                if (ddt != null)
                                {
                                    if (invalidDS == null)
                                    {
                                        invalidDS = new DataSet();

                                    }
                                    invalidDS.Tables.Add(ddt);
                                }
                            }
                            //------------------------------------------------------------------------------------------------------
                        }
                    }
                    #endregion

                    if (respList != null && respList.Count > 0)
                    {
                        //result.ImportedCount = respList.Count;
                        returnedDS = RespAppvMatrixDataAccess.UpdateRespApproverMapInBulk(respList, Utilities.Utilities.GetUserName());
                        //Utilities.Utilities.WriteErrorLog("returnedDS : ", returnedDS ==null ? "null" : returnedDS.Tables.Count.ToString());
                        if (returnedDS != null)
                        {
                            result.ImportedCount = returnedDS.Tables[0].Select("Changes = 'UPDATE'").Length;
                            result.TotalCount = returnedDS.Tables[0].Select("Changes = 'INSERT'").Length;
                        }
                    }

                    List<RespApproverMap> invalidrespList = new List<RespApproverMap>();
                    if (invalidDS != null)
                    {
                        using (ExcelPackage package = new ExcelPackage())
                        {
                            foreach (DataTable tbl in invalidDS.Tables)
                            {
                                tbl.Columns.Remove("ID");
                                ExcelWorksheet sheet = package.Workbook.Worksheets.Add(tbl.TableName);
                                sheet.Cells["A1"].LoadFromDataTable(tbl, true);
                                sheet.Cells.AutoFitColumns();
                                result.FailedCount += tbl.Rows.Count;
                                foreach (DataRow row in tbl.Rows)
                                {
                                    map = new RespApproverMap();
                                    map.Instance = tbl.TableName.Equals("DONOTUSE", StringComparison.OrdinalIgnoreCase) ? "BETSY" : tbl.TableName;
                                    map.Ap_Group = row["Ap Group"] as string;
                                    map.Division = row["Division"] as string;
                                    map.Responsibility_Name = row["Responsibility Name"] as string;
                                    map.Application = row["Application"] as string;
                                    map.Primary_Approver = row["Primary Approver"] as string;
                                    map.Secondary_Approver = row["Secondary Approver"] as string;
                                    map.Final_Approver = row["Final Approver"] as string;
                                    map.Comment = row["Comment"] as string;
                                    map.Sod_Active = row["Sod Active"] as string;
                                    map.Last_Updated_By = Utilities.Utilities.GetUserName();
                                    map.Do_Not_Use = tbl.TableName.Equals("DONOTUSE", StringComparison.OrdinalIgnoreCase) ? "Yes" : "No";
                                    map.Default = row["Default"] as string;
                                    map.Error = row["Errors"] as string;

                                    invalidrespList.Add(map);
                                }
                            }
                            result.FailedPath = Utilities.Utilities.SaveDataSetToExcel(invalidDS, "Exceptional_Responsibilities", ImportTime);
                        }
                    }

                    if (invalidrespList != null && invalidrespList.Count > 0)
                    {
                        RespAppvMatrixDataAccess.BackUpInvalidResp(invalidrespList, Utilities.Utilities.GetUserName());
                    }
                }
            }
            catch (ThreadAbortException ex)
            {
                Utilities.Utilities.WriteErrorLog(ex.Message, ex.StackTrace);
                ImportingLocked = false;
                return new HttpResponseObject<ImportResult>() { IsSuccess = false, Content = null, ErrorMsg = "Process Cancelled" };
            }
            catch (ThreadInterruptedException ex)
            {
                Utilities.Utilities.WriteErrorLog(ex.Message, ex.StackTrace);
                ImportingLocked = false;
                return new HttpResponseObject<ImportResult>() { IsSuccess = false, Content = null, ErrorMsg = "Process Cancelled" };
            }
            catch (Exception ex)
            {
                Utilities.Utilities.WriteErrorLog(ex.Message, ex.StackTrace);
                ImportingLocked = false;
                return new HttpResponseObject<ImportResult>() { IsSuccess = false, Content = result, ErrorMsg = ex.Message };
            }

            ImportingLocked = false;
            return new HttpResponseObject<ImportResult>() { IsSuccess = true, Content = result, ErrorMsg = null };

        }


        public static HttpResponseObject<DataCollection<RespApproverMap>> GetResponsibilityApproverMatrix(RespApproverMatrixQuery query)
        {
            HttpResponseObject<DataCollection<RespApproverMap>> result = new HttpResponseObject<DataCollection<RespApproverMap>>();
            DataCollection<RespApproverMap> resp_dataset = new DataCollection<RespApproverMap>();

            if (query != null)
            {
                try
                {
                    List<RespApproverMap> list_resp = RespAppvMatrixDataAccess.GetRespApproverMatrix(query) as List<RespApproverMap>;
                    int stratindex = (query.Page_Index - 1) * query.Page_Size;
                    int endindex = query.Page_Index * query.Page_Size;
                    resp_dataset.Collection = list_resp.Where(x => x.RowNum > stratindex && x.RowNum <= endindex).ToList();
                    resp_dataset.TotalCount = list_resp.Count();

                    result.Content = resp_dataset;
                    result.IsSuccess = true;
                    result.ErrorMsg = null;

                    if (_cache.Contains("dtInCache"))
                    {
                        _cache.Remove("dtInCache");
                    }
                    _cache.Add("dtInCache", list_resp, DateTime.Now.AddMinutes(30));


                }
                catch (Exception ex)
                {
                    Utilities.Utilities.WriteErrorLog(ex.Message, ex.StackTrace);
                    result.Content = null;
                    result.IsSuccess = false;
                    result.ErrorMsg = ex.Message;
                }
            }

            return result;
        }

        private static void RespImportValidation(DataTable dt)
        {
            // Step1. validate Required Fields
            //ValidateRequiredField(dt, "Ap Group");
            //ValidateRequiredField(dt, "Division");
            ValidateRequiredField(dt, "Responsibility Name");
            ValidateRequiredField(dt, "Application");
            if (dt.TableName.ToUpper() != "DONOTUSE")
            {
                ValidateRequiredField(dt, "Primary Approver");
                ValidateRequiredField(dt, "Secondary Approver");
            }



            // Step2. validate approvers
            #region validate approvers
            if (dt.TableName.ToUpper() != "DONOTUSE")
            {
                List<string> p_Approvers = new List<string>();
                List<string> s_Approvers = new List<string>();
                List<string> f_Approvers = new List<string>();
                p_Approvers = (from r in dt.AsEnumerable()
                               select r.Field<string>("Primary Approver")
                              ).Distinct().ToList();
                s_Approvers = (from r in dt.AsEnumerable()
                               select r.Field<string>("Secondary Approver")
                                          ).Distinct().ToList();
                f_Approvers = (from r in dt.AsEnumerable()
                               select r.Field<string>("Final Approver")
                                          ).Distinct().ToList();

                p_Approvers.AddRange(s_Approvers);
                p_Approvers.AddRange(f_Approvers);
                p_Approvers = p_Approvers.Distinct().ToList();

                StringBuilder sb = new StringBuilder();
                foreach (string v in p_Approvers)
                {
                    if (!string.IsNullOrEmpty(v))
                    {
                        sb.Append(v.Replace("'","''"));
                        sb.Append("^");
                    }
                }
                DataTable invalidAppvDt = new DataTable();
                if (dt.TableName.ToUpper() != "DONOTUSE")
                {
                    invalidAppvDt = RespAppvMatrixDataAccess.CheckApprover(sb.ToString());
                }

                if (invalidAppvDt != null)
                {
                    // check Primary Approver
                    ValidateApprover(dt, invalidAppvDt, "Primary Approver");

                    // check Secondary Approver 
                    ValidateApprover(dt, invalidAppvDt, "Secondary Approver");

                    // check Final Approver
                    ValidateApprover(dt, invalidAppvDt, "Final Approver");
                }
            }
            #endregion



            // Step3: check duplicated items
            var result = from r in dt.AsEnumerable()
                         group r by new
                         {
                             r1 = r.Field<string>("Responsibility Name"),
                             r2 = r.Field<string>("Application")
                         }
                             into g
                             where g.Count() > 1
                             select new RespApproverMap
                             {
                                 Responsibility_Name = g.Key.r1,
                                 Application = g.Key.r2

                             };

            if (result != null && result.Count() > 0)
            {
                foreach (var g in result)
                {
                    var query = (from r in dt.AsEnumerable()
                                 where r.Field<string>("Responsibility Name") == g.Responsibility_Name
                                 & r.Field<string>("Application") == g.Application
                                 select r);

                    if (query != null && query.Count() > 0)
                    {
                        var duplicated_dt = query.CopyToDataTable();
                        foreach (DataRow rr in duplicated_dt.Rows)
                        {
                            if (rr["Errors"] != null && rr["Errors"].ToString().Trim() != "")
                            {
                                rr["Errors"] += " \n";
                            }

                            rr["Errors"] += "Duplicated Item";
                        }

                        dt.Merge(duplicated_dt);
                    }
                }
            }


        }

        private static void ValidateRequiredField(DataTable dt, string fieldName)
        {
            StringBuilder condition = new StringBuilder();
            StringBuilder error = new StringBuilder();

            condition.Append(" isnull([" + fieldName+ "],'') ='' ");
            error.Append(fieldName);

            if (fieldName.Contains("Primary Approver") || fieldName.Contains("Secondary Approver"))
            {
                //check approver when Default <> Yes
                condition.Append(" and isnull([Default],'') <>'Yes'");
            }

            foreach (DataRow dr in dt.Select(condition.ToString()))
            {
                dr["Errors"] += "Missing Field(s): " + error.ToString() + ";";
            }
        }

        private static void ValidateApprover(DataTable dt, DataTable invalidAppvDt, string fieldName)
        {
            var results = (from r in dt.AsEnumerable()
                                join a in invalidAppvDt.AsEnumerable()
                               on r.Field<string>(fieldName) equals a.Field<string>("Approver")
                               where string.Compare(r.Field<string>("Default"),"Yes",true) !=0
                               select r);

            if (results != null && results.Count() > 0)
            {
                DataTable filter_dt = results.CopyToDataTable();
                foreach (DataRow rr in filter_dt.Rows)
                {
                    if (rr["Errors"] == null || rr["Errors"].ToString().Trim() == "")
                    {
                        rr["Errors"] = "Invalid Approver(s): " + fieldName;
                    }
                    else
                    {
                        if (rr["Errors"].ToString().Contains("Invalid Approver"))
                        {
                            rr["Errors"] += ", " + fieldName;
                        }
                        else
                        {
                            rr["Errors"] += "\n Invalid Approver(s): " + fieldName;
                        }
                    }
                }
                dt.Merge(filter_dt);
            }
        }

        [SecurityPermissionAttribute(SecurityAction.Demand, ControlThread = true)]
        public static bool TerminateImporting()
        {
            if (ImportingThread != null && ImportingThread.IsAlive)
            {
                try
                {
                    ThreadState state = ImportingThread.ThreadState;
                    ImportingThread.Interrupt();
                    state = ImportingThread.ThreadState;
                    if (!ImportingThread.Join(2000))
                    {
                        ImportingThread.Abort();
                    }

                    while (ImportingLocked)
                    {
                        state = ImportingThread.ThreadState;
                    }
                }
                catch (Exception ex)
                {
                    Utilities.Utilities.WriteErrorLog(ex.Message, ex.StackTrace);
                }
            }

            return !ImportingLocked;
        }


        public static HttpResponseObject<RespApproverMap> GetSingleRespApproverMap(int id)
        {
            HttpResponseObject<RespApproverMap> result = new HttpResponseObject<RespApproverMap>();

            try
            {
                result.Content = RespAppvMatrixDataAccess.GetSingleRespApproverMap(id);
                result.ErrorMsg = null;
                result.IsSuccess = true;
            }
            catch (Exception ex)
            {
                Utilities.Utilities.WriteErrorLog(ex.Message, ex.StackTrace);
                result.Content = null;
                result.IsSuccess = false;
                result.ErrorMsg = ex.Message;
            }

            return result;
        }

        public static HttpResponseObject<RespApproverMap> UpdateRespApproverMap(RespApproverMap map)
        {
            HttpResponseObject<RespApproverMap> result = new HttpResponseObject<RespApproverMap>();
            if (map != null)
            {
                try
                {
                    // update or add
                    int id = RespAppvMatrixDataAccess.UpdateRespApproverMap(map, Utilities.Utilities.GetUserName());
                    if (id == -1)
                    {
                        result.ErrorMsg = "Primary Approver is invalid in Global Directory.";
                        result.IsSuccess = false;
                    }
                    else if (id == -2)
                    {
                        result.ErrorMsg = "Secondary Approver is invalid in Global Directory.";
                        result.IsSuccess = false;
                    }
                    else if (id == -3)
                    {
                        result.ErrorMsg = "This Instance, Responsibility Name and Application has existed.";
                        result.IsSuccess = false;
                    }
                    else
                    {
                        result.Content = RespAppvMatrixDataAccess.GetSingleRespApproverMap(id);
                        result.IsSuccess = true;
                    }

                    //if (map.Default.ToUpper() == "YES")
                    //{

                    //}
                    //else
                    //{
                    //    if (RespAppvMatrixDataAccess.CheckApprover(map.Primary_Approver) != null && RespAppvMatrixDataAccess.CheckApprover(map.Primary_Approver).Rows.Count > 0)
                    //    {
                    //        result.ErrorMsg = "Invalid Primary Approver.";
                    //        result.IsSuccess = false;
                    //    }
                    //    else if (RespAppvMatrixDataAccess.CheckApprover(map.Secondary_Approver) != null && RespAppvMatrixDataAccess.CheckApprover(map.Secondary_Approver).Rows.Count > 0)
                    //    {
                    //        result.ErrorMsg = "Invalid Secodary Approver.";
                    //        result.IsSuccess = false;
                    //    }
                    //    else
                    //    {
                    //        int id = RespAppvMatrixDataAccess.UpdateRespApproverMap(map, Utilities.Utilities.GetUserName());
                    //        result.Content = RespAppvMatrixDataAccess.GetSingleRespApproverMap(id);
                    //        result.IsSuccess = true;
                    //    }

                    //}
                }
                catch (Exception ex)
                {
                    Utilities.Utilities.WriteErrorLog(ex.Message, ex.StackTrace);           
                }
            }
            else
            {
                result.IsSuccess = false;
                result.ErrorMsg = "The parameter is null";
            }
            return result;
        }

        public static HttpResponseObject<List<KeyValuePair<string, bool>>> CheckIfUserExistsInGD(string p_email, string s_email, string f_email)
        {
            HttpResponseObject<List<KeyValuePair<string, bool>>> result = new HttpResponseObject<List<KeyValuePair<string, bool>>>();
            List<KeyValuePair<string, bool>> list = new List<KeyValuePair<string, bool>>();
            try
            {
               var p_list = ValidateEmail(p_email);
               if (p_list != null)
               {
                   list.AddRange(p_list);
               }

               var s_list = ValidateEmail(s_email);
               if (s_list != null)
               {
                   list.AddRange(s_list);
               }

               var f_list = ValidateEmail(f_email);
               if (f_list != null)
               {
                   list.AddRange(f_list);
               }

                result.Content = list;
                result.IsSuccess = true;
            }
            catch (Exception ex)
            {
                Utilities.Utilities.WriteErrorLog(ex.Message, ex.StackTrace);
                result.IsSuccess = false;
                result.ErrorMsg = ex.Message;
            }

            return result;
        }

        public static List<KeyValuePair<string, bool>> ValidateEmail(string emails)
        {
            List<KeyValuePair<string, bool>> list = null;
            if (!string.IsNullOrWhiteSpace(emails))
            {
                list = new List<KeyValuePair<string, bool>>();
                var e_Arr = emails.Split(';');
                foreach (var email in e_Arr)
                {
                    if (!string.IsNullOrWhiteSpace(email))
                    {
                        list.Add(new KeyValuePair<string, bool>(email.Trim(), CheckUserByEmail(email.Trim())));
                    }
                }
            }

            return list;
        }

        public static HttpResponseObject<bool> DeleteRespApproverMap(int id)
        {
            HttpResponseObject<bool> resullt = new HttpResponseObject<bool>();

            try
            {
                resullt.Content = RespAppvMatrixDataAccess.DeleteRespApproverMap(id, Utilities.Utilities.GetUserName()) > 0 ? true : false;
                resullt.IsSuccess = true;
            }
            catch (Exception ex)
            {
                Utilities.Utilities.WriteErrorLog(ex.Message, ex.StackTrace);
                resullt.IsSuccess = false;
                resullt.Content = false;
                resullt.ErrorMsg = ex.Message;
            }

            return resullt;
        }

        public static MemoryStream ExportRespAppvMatrixToExcel(RespApproverMatrixQuery query)
        {
            try
            {
                //DataTable dt = RespAppvMatrixDataAccess.GetRespApproverMatrixDT(query);
                DataTable dt = new DataTable();
                dt.Columns.Add("Ap_Group");
                dt.Columns.Add("Division");
                dt.Columns.Add("Responsibility_Name");
                dt.Columns.Add("Application");
                dt.Columns.Add("Primary_Approver");
                dt.Columns.Add("Secondary_Approver");
                dt.Columns.Add("Final_Approver");
                dt.Columns.Add("Comment");
                dt.Columns.Add("Sod_Active");
                dt.Columns.Add("Last_Updated_By");
                dt.Columns.Add("Last_Updated_Date");
                dt.Columns.Add("Available_in_Production");
                dt.Columns.Add("Active");
                dt.Columns.Add("Start_Date");
                dt.Columns.Add("End_Date");
                dt.Columns.Add("Do_Not_Use");
                dt.Columns.Add("Default");

                DataTable dt_main = new DataTable();
                DataTable dt_donotuse = new DataTable();
                DataTable dt_default = new DataTable();

                if (_cache.Contains("dtInCache"))
                {
                    foreach (RespApproverMap item in _cache.Get("dtInCache") as List<RespApproverMap>)
                    {
                        DataRow row = dt.NewRow();
                        row["Ap_Group"] = item.Ap_Group;
                        row["Division"] = item.Division;
                        row["Responsibility_Name"] = item.Responsibility_Name;
                        row["Application"] = item.Application;
                        row["Primary_Approver"] = item.Primary_Approver;
                        row["Secondary_Approver"] = item.Secondary_Approver;
                        row["Final_Approver"] = item.Final_Approver;
                        row["Comment"] = item.Comment;
                        row["Sod_Active"] = item.Sod_Active;
                        row["Last_Updated_By"] = item.Last_Updated_By;
                        row["Last_Updated_Date"] = item.Last_Updated_Date;
                        row["Available_in_Production"] = item.Available_in_Production;
                        row["Active"] = item.Active;
                        row["Start_Date"] = item.Start_Date;
                        row["End_Date"] = item.End_Date;
                        row["Do_Not_Use"] = item.Do_Not_Use;
                        row["Default"] = item.Default;

                        dt.Rows.Add(row);
                    }
                }
                else
                {
                    dt = RespAppvMatrixDataAccess.GetRespApproverMatrixDT(query);
                }

                if (dt != null && dt.Rows.Count > 0)
                {
                    if (dt.Select("[Do_Not_Use] <> 'Yes' and isnull([Default],'') <>'Yes' ").Count() > 0)
                    {
                        dt_main = dt.Select("[Do_Not_Use] <> 'Yes' and isnull([Default],'') <>'Yes' ").CopyToDataTable();
                    }
                    if (dt.Select("[Do_Not_Use] <> 'Yes' AND [Default] = 'Yes'").Count() > 0)
                    {
                        dt_default = dt.Select("[Do_Not_Use] <> 'Yes' and isnull([Default],'') = 'Yes'").CopyToDataTable();
                    }
                    if (dt.Select("[Do_Not_Use] = 'Yes'").Count() > 0)
                    {
                        dt_donotuse = dt.Select("[Do_Not_Use] = 'Yes'").CopyToDataTable();
                    }

                    using (ExcelPackage package = new ExcelPackage())
                    {
                        ExcelWorksheet sheet = null;
                        if (dt_main != null && dt_main.Rows.Count > 0)
                        {
                            sheet = package.Workbook.Worksheets.Add(query.Instance);
                            sheet.Cells["A1"].LoadFromDataTable(dt_main, true);
                            sheet.Cells.AutoFitColumns();
                        }

                        if (dt_donotuse != null && dt_donotuse.Rows.Count > 0)
                        {
                            sheet = package.Workbook.Worksheets.Add("DoNotUse");
                            sheet.Cells["A1"].LoadFromDataTable(dt_donotuse, true);
                            sheet.Cells.AutoFitColumns();
                        }

                        if (dt_default != null && dt_default.Rows.Count > 0)
                        {
                            sheet = package.Workbook.Worksheets.Add("Default");
                            sheet.Cells["A1"].LoadFromDataTable(dt_default, true);
                            sheet.Cells.AutoFitColumns();
                        }

                        MemoryStream mStream = new MemoryStream();
                        package.SaveAs(mStream);

                        return mStream;
                    }
                }

                return null;
            }
            catch (Exception ex)
            {
                Utilities.Utilities.WriteErrorLog(ex.Message, ex.StackTrace);
                return null;
            }
        }

        private static bool CheckUserByEmail(string email_address)
        {
            if (!string.IsNullOrWhiteSpace(email_address))
            {
                var res = RespAppvMatrixDataAccess.CheckIfUserExistsInGD(email_address.Trim());
                if (res != null && res.Count() > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            return false;
        }

    }
}
