using ExcelDataReader;
using GDRespApproverMatrixMaintenance.DataAccess;
using GDRespApproverMatrixMaintenance.Model;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace GDRespApproverMatrixMaintenance.Business
{
    public class HRISContactBusiness
    {
        public static bool ImportingLocked { get; set; }


        //public static HttpResponseObject<MemoryStream> ImportHRISContactFromExcel(Stream stream)
        public static HttpResponseObject<ImportResult> ImportHRISContactFromExcel(HttpPostedFile file)
        {
            DataSet invalidDS = null;
            //MemoryStream mStream = null;
            ImportResult result = new ImportResult();

            try
            {
                string guid = Guid.NewGuid().ToString();
                Utilities.Utilities.FileSaveAs(file, "HRIS_Contact", guid);

                IExcelDataReader reader = ExcelReaderFactory.CreateReader(file.InputStream);
                DataSet ds = reader.AsDataSet(new ExcelDataSetConfiguration()
                {
                    ConfigureDataTable = (tablereader) => new ExcelDataTableConfiguration()
                    {
                        UseHeaderRow = true
                    }
                });

                #region retrive datatables
                foreach (DataTable dt in ds.Tables)
                {
                    if (dt.Rows.Count == 0)
                    {
                        break;
                    }
                    string instance = dt.TableName.Trim().ToUpper();
                    if (instance == "HRIS CONTACTS")
                    {
                        result.TotalCount += dt.Rows.Count;
                        DataTable tempDt = dt.Copy();
                        dt.Clear();

                        DataTableReader dr = tempDt.CreateDataReader();
                        DataColumn col = new DataColumn("ID", typeof(Int32));
                        dt.Columns.Add(col);
                        col.AutoIncrement = true;
                        col.AutoIncrementSeed = 1;
                        col.AutoIncrementStep = 1;

                        dt.PrimaryKey = new DataColumn[]{
                                dt.Columns["ID"]
                            };

                        dt.Load(dr);
                        tempDt.Dispose();


                        dt.Columns.Add("Errors", type: typeof(string));

                        ImportValidation.ValidateRequiredField(dt, "RespID");
                        ImportValidation.ValidateRequiredField(dt, "Responsibility Name");
                        //ImportValidation.ValidateRequiredField(dt, "Org Name");
                        //ImportValidation.ValidateRequiredField(dt, "Business Group");
                        ImportValidation.ValidateRequiredField(dt, "Contact1");
                        //ImportValidation.ValidateRequiredField(dt, "Contact2");

                        // check value format
                        CheckDataFormat(dt, "^[0-9]*$", "RespID");
                        CheckDataFormat(dt, "^[0-9]*$", "OU Org ID");
                        CheckDataFormat(dt, "^[0-9]*$", "BG ID");
                        CheckDataFormat(dt, "^[0-1]{1}$", "Diff"); //Match 0 Or 1

                        List<string> contactList = new List<string>();
                        // check contactors
                        DataTable contactDT = new DataTable();
                        contactDT.Columns.Add("Value");
                        contactDT.CaseSensitive = false;

                        foreach (DataRow row in dt.Rows)
                        {
                            //row["Contact1"] != null && row["Contact1"].ToString().Trim() != ""
                            var contact1 = row["Contact1"] as string;
                            var contact2 = row["Contact2"] as string;
                            if (!string.IsNullOrWhiteSpace(contact1))
                            {
                                var arr = contact1.Split(';');
                                foreach (string cont in arr)
                                {
                                    contactDT.Rows.Add(cont);
                                }
                            }

                            if (!string.IsNullOrWhiteSpace(contact2))
                            {
                                var arr = contact2.Split(';');
                                foreach (string cont in arr)
                                {
                                    contactDT.Rows.Add(cont);
                                }
                            }
                        }

                        contactDT = contactDT.Select(" isnull(Value,'') <> ''").Distinct().CopyToDataTable();
                        StringBuilder sb = new StringBuilder();
                        foreach (DataRow item in contactDT.Rows)
                        {
                            sb.Append(item["Value"].ToString().Replace("'","''"));
                            sb.Append("^");
                        }
                        //DataTable invalidContactDt = RespAppvMatrixDataAccess.CheckApprover(contactDT);
                        DataTable invalidContactDt = RespAppvMatrixDataAccess.CheckApprover(sb.ToString());
                        ValidateContact2(dt, invalidContactDt, "Contact1");
                        ValidateContact2(dt, invalidContactDt, "Contact2");
                        // check duplicated items
                        DuplicatedContact(dt);

                        #region update to DB
                        if (dt.Select("Errors is null").Count() > 0)
                        {
                            DataTable updateDt = dt.Select("Errors is null").CopyToDataTable();
                            result.ImportedCount += updateDt.Rows.Count;

                            updateDt.Columns.Remove("Errors");
                            updateDt.Columns.Remove("ID");

                            List<HRISContact> contact_list = new List<HRISContact>();
                            foreach (DataRow row in updateDt.Rows)
                            {
                                HRISContact contact = new HRISContact();
                                contact.RespID = Convert.ToInt32(row["RespID"]);
                                contact.Responsibility_Name = row["Responsibility Name"] as String;
                                contact.NameOrType = row["Name/Type"] as string;
                                if (row["OU Org ID"] != null && row["OU Org ID"].ToString().Trim() != "")
                                {
                                    contact.OU_Org_ID = Convert.ToInt32(row["OU Org ID"].ToString().Trim());
                                }
                                contact.Org_Name = row["Org Name"] as string;
                                if (row["BG ID"] != null && row["BG ID"].ToString().Trim() != "")
                                {
                                    contact.BG_ID = Convert.ToInt32(row["BG ID"].ToString().Trim());
                                }
                                contact.Business_Group = row["Business Group"] as string;
                                contact.Description = row["Description"] as string;
                                if (row["Diff"] != null && row["Diff"].ToString().Trim() != "")
                                {
                                    contact.Diff = Convert.ToInt32(row["Diff"]);
                                }
                                contact.Contact1 = row["Contact1"] as string;
                                contact.Contact2 = row["Contact2"] as string;
                                contact.Interface_User_Name = row["Interface User Name"] as string;
                                contact.File_Name = row["File Name"] as string;
                                contact.Notes = row["Notes"] as string;
                                contact.Comment = row["Comment"] as string;
                                contact.Attr2 = row["Attr2"] as string;
                                //contact.Last_Updated_By = Utilities.Utilities.GetUserName();

                                contact_list.Add(contact);
                            }
                            HRISContactDataAccessor.UpdateHRISContactInBulk(contact_list, Utilities.Utilities.GetUserName());
                        }

                        #endregion

                        if (dt.Select("Errors is not null").Count() > 0)
                        {
                            var ddt = dt.Select("Errors is not null").CopyToDataTable();
                            ddt.Columns.Remove("ID");
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
                    }
                }
                #endregion

                if (invalidDS != null)
                {
                    using (ExcelPackage package = new ExcelPackage())
                    {
                        foreach (DataTable tbl in invalidDS.Tables)
                        {
                            ExcelWorksheet sheet = package.Workbook.Worksheets.Add(tbl.TableName);
                            sheet.Cells["A1"].LoadFromDataTable(tbl, true);
                            sheet.Cells.AutoFitColumns();
                            result.FailedCount += tbl.Rows.Count;
                        }
                    }
                    result.FailedPath = Utilities.Utilities.SaveDataSetToExcel(invalidDS, "Exceptional_HRIS_Contact", guid);
                }

            }
            catch (Exception ex)
            {
                Utilities.Utilities.WriteErrorLog(ex.Message, ex.StackTrace);
                ImportingLocked = false;
                return new HttpResponseObject<ImportResult>() { IsSuccess = false, Content = null, ErrorMsg = ex.Message };
            }

            ImportingLocked = false;
            return new HttpResponseObject<ImportResult>() { IsSuccess = true, Content = result, ErrorMsg = null };


        }


        private static void ValidateContact(DataTable dt, DataTable invalidDt, string fieldName)
        {
            var results = (from r in dt.AsEnumerable()
                           from a in invalidDt.AsEnumerable()
                           where r.Field<string>(fieldName) != null && r.Field<string>(fieldName).Contains(a.Field<string>("Approver"))
                           select r);

            if (results != null && results.Count() > 0)
            {
                DataTable filter_dt = results.CopyToDataTable();
                foreach (DataRow rr in filter_dt.Rows)
                {
                    if (rr["Errors"] == null || rr["Errors"].ToString().Trim() == "")
                    {
                        rr["Errors"] = "Invalid Contact(s): " + fieldName;
                    }
                    else
                    {
                        if (rr["Errors"].ToString().Contains("Invalid Contact"))
                        {
                            rr["Errors"] += ", " + fieldName;
                        }
                        else
                        {
                            rr["Errors"] += ".\n Invalid Contact(s): " + fieldName;
                        }
                    }
                }
                dt.Merge(filter_dt);
            }
        }

        private static void ValidateContact2(DataTable dt, DataTable invalidDt, string fieldName)
        {
            var results = (from r in dt.AsEnumerable()
                           from a in invalidDt.AsEnumerable()
                           where r.Field<string>(fieldName) != null && r.Field<string>(fieldName).Contains(a.Field<string>("Approver"))
                           select r);

            if (results != null && results.Count() > 0)
            {
                DataTable filter_dt = results.CopyToDataTable();
                foreach (DataRow rr in filter_dt.Rows)
                {

                    var con_arr = rr[fieldName].ToString().Split(';');

                    foreach (string c in con_arr)
                    {
                        var query = (from i in invalidDt.AsEnumerable()
                                     where i.Field<string>(0) != null && i.Field<string>(0).Contains(c)
                                     select i);

                        if (query.Count() > 0)
                        {
                            if (rr["Errors"] == null || rr["Errors"].ToString().Trim() == "")
                            {
                                rr["Errors"] = "Invalid Contact(s): " + c;
                            }
                            else
                            {
                                if (rr["Errors"].ToString().Contains("Invalid Contact") && rr["Errors"].ToString().IndexOf(c) < 0)
                                {
                                    rr["Errors"] += ", " + c;
                                }
                                else
                                {
                                    rr["Errors"] += ".\n Invalid Contact(s): " + c;
                                }
                            }

                        }
                    }


                }
                dt.Merge(filter_dt);
            }
        }


        private static void DuplicatedContact(DataTable dt)
        {
            var results = (from r in dt.AsEnumerable()
                           where r.Field<object>("RespID") != null
                           group r by new
                           {
                               c1 = r.Field<object>("RespID")
                               //,c2 = r.Field<string>("Responsibility Name")
                           } into g

                           where g.Count() > 1
                           select new HRISContact
                           {
                               RespID = Convert.ToInt32(g.Key.c1)
                               //,Responsibility_Name=g.Key.c2
                           });


            if (results != null && results.Count() > 0)
            {
                foreach (var g in results)
                {
                    var query = (from r in dt.AsEnumerable()
                                 where
                                     //r.Field<string>("Responsibility Name") == g.Responsibility_Name &&
                                 Convert.ToInt32(r.Field<object>("RespID")) == g.RespID
                                 select r);

                    if (query != null && query.Count() > 0)
                    {
                        var duplicated_dt = query.CopyToDataTable();
                        foreach (DataRow rr in duplicated_dt.Rows)
                        {
                            if (rr["Errors"] != null && rr["Errors"].ToString().Trim() != "")
                            {
                                rr["Errors"] += ". \n";
                            }

                            rr["Errors"] += "Duplicated Item";
                        }
                        dt.Merge(duplicated_dt);
                    }
                }
            }
        }

        private static void CheckDataFormat(DataTable dt, string expresion, string fieldName)
        {
            Regex reg = new Regex(expresion);
            var results = (from r in dt.AsEnumerable()
                           where r.Field<object>(fieldName) != null && !reg.IsMatch(Convert.ToString(r.Field<object>(fieldName)).Trim())
                           select r
                  );

            if (results != null && results.Count() > 0)
            {
                DataTable filter_dt = results.CopyToDataTable();
                foreach (DataRow rr in filter_dt.Rows)
                {
                    if (rr["Errors"] == null || rr["Errors"].ToString().Trim() == "")
                    {
                        rr["Errors"] = "Invalid Value Format: " + fieldName;
                    }
                    else
                    {
                        if (rr["Errors"].ToString().Contains("Invalid Value Format"))
                        {
                            rr["Errors"] += ", " + fieldName;
                        }
                        else
                        {
                            rr["Errors"] += ".\n Invalid Value Format: " + fieldName;
                        }
                    }
                }
                dt.Merge(filter_dt);
            }
        }


        public static HttpResponseObject<DataCollection<HRISContact>> GetHRISContact(HRISContactQuery query)
        {
            HttpResponseObject<DataCollection<HRISContact>> result = new HttpResponseObject<DataCollection<HRISContact>>();
            DataCollection<HRISContact> contact_dataset = new DataCollection<HRISContact>();

            if (query != null)
            {
                try
                {
                    contact_dataset.Collection = HRISContactDataAccessor.GetHRISContactList(query);
                    contact_dataset.TotalCount = HRISContactDataAccessor.GetHRISContactCount(query);
                    result.Content = contact_dataset;
                    result.IsSuccess = true;
                    result.ErrorMsg = null;
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


        public static MemoryStream ExportHRISContactToExcel(string resp_name = "", string org_name = "", string contact = "")
        {
            try
            {
                HRISContactQuery query = new HRISContactQuery();
                query.Responsibility_Name = resp_name;
                query.Org_Name = org_name;
                query.Contact = contact;
                query.Page_Index = 1;
                query.Page_Size = 0;
                query.GetCountFlag = false;
                DataTable dt = HRISContactDataAccessor.GetHRISContactDT(query);
                if (dt != null && dt.Rows.Count > 0)
                {
                    dt.Columns.Remove("RowNum");

                    foreach (DataColumn col in dt.Columns)
                    {
                        if (col.ColumnName.ToLower() == "nameortype")
                        {
                            col.ColumnName = "Name/Type";
                        }
                        col.ColumnName = col.ColumnName.Replace("_", " ");
                    }

                    using (ExcelPackage package = new ExcelPackage())
                    {
                        ExcelWorksheet sheet = package.Workbook.Worksheets.Add("HRIS Contacts");
                        sheet.Cells["A1"].LoadFromDataTable(dt, true);
                        sheet.Cells.AutoFitColumns();

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


        public static HttpResponseObject<HRISContact> GetSingleHRISContact(int id = 0)
        {
            HttpResponseObject<HRISContact> result = new HttpResponseObject<HRISContact>();
            try
            {
                if (id > 0)
                {
                    result.Content = HRISContactDataAccessor.GetSingleHRISContact(id);
                    result.IsSuccess = true;
                }
                else
                {
                    result.Content = null;
                    result.IsSuccess = false;
                }
            }
            catch (Exception ex)
            {
                result.Content = null;
                result.IsSuccess = false;
                result.ErrorMsg = ex.Message;
                Utilities.Utilities.WriteErrorLog(ex.Message, ex.StackTrace);
            }
            return result;
        }

        public static HttpResponseObject<HRISContact> UpdateSingleHRISContact(HRISContact contact)
        {
            HttpResponseObject<HRISContact> result = new HttpResponseObject<HRISContact>();
            int ResultValue = 0;
            if (contact != null)
            {
                result.IsSuccess = true;
                try
                {
                    contact.Last_Updated_By = Utilities.Utilities.GetUserName();
                    string EditType = contact.ID == 0 ? "Add" : "Edit";

                    if (EditType == "Add")
                    {
                        // check duplicated items
                        //var acount = HRISContactDataAccessor.CheckHRISContactExisting(contact.RespID, contact.Responsibility_Name);
                        //if ((string.IsNullOrWhiteSpace(contact.Last_Updated_By) && acount > 0) || (!string.IsNullOrWhiteSpace(contact.Last_Updated_By) && acount > 1))
                        //{
                        //    result.ErrorMsg = "The RespID has existed already !";
                        //    result.IsSuccess = false;
                        //    return result;
                        //}

                        ResultValue = HRISContactDataAccessor.UpdateHRISContact(contact, EditType);
                        if (ResultValue == -1)
                        {
                            result.ErrorMsg = "The RespID has existed already !";
                            result.IsSuccess = false;
                            return result;
                        }
                        else
                        {
                            result.Content = HRISContactDataAccessor.GetSingleHRISContact(ResultValue);
                        }

                    }
                    else
                    {
                        HRISContactDataAccessor.UpdateHRISContact(contact, EditType);
                        result.Content = HRISContactDataAccessor.GetSingleHRISContact(contact.ID);
                    }

                }
                catch (Exception ex)
                {
                    result.IsSuccess = false;
                    result.ErrorMsg = ex.Message;
                    Utilities.Utilities.WriteErrorLog(ex.Message, ex.StackTrace);
                }

            }
            else
            {
                result.IsSuccess = false;
                result.ErrorMsg = "The Contact object cannot be null";
            }
            return result;
        }

        public static HttpResponseObject<bool> DeleteHRISContact(int id = 0)
        {
            HttpResponseObject<bool> result = new HttpResponseObject<bool>();
            result.IsSuccess = false;
            if (id > 0)
            {
                try
                {
                    HRISContactDataAccessor.DeleteHRISContact(id, Utilities.Utilities.GetUserName());
                    result.IsSuccess = true;
                }
                catch (Exception ex)
                {
                    result.ErrorMsg = ex.Message;
                    Utilities.Utilities.WriteErrorLog(ex.Message, ex.StackTrace);
                }
            }
            return result;
        }

        public static HttpResponseObject<List<KeyValuePair<string, bool>>> CheckHRISContact(string contact1, string contact2 = "")
        {
            HttpResponseObject<List<KeyValuePair<string, bool>>> result = new HttpResponseObject<List<KeyValuePair<string, bool>>>();
            List<KeyValuePair<string, bool>> list = new List<KeyValuePair<string, bool>>();
            try
            {

                if (string.IsNullOrWhiteSpace(contact1))
                {
                    result.IsSuccess = false;
                    result.ErrorMsg = "Contact1 is required";

                    return result;
                }
                else
                {
                    List<KeyValuePair<string, bool>> list1 = ResponsibilityBusiness.ValidateEmail(contact1);
                    if (list1 != null)
                    {
                        list.AddRange(list1);
                    }
                }

                if (!string.IsNullOrWhiteSpace(contact2))
                {
                    List<KeyValuePair<string, bool>> list2 = ResponsibilityBusiness.ValidateEmail(contact1);
                    if (list2 != null)
                    {
                        list.AddRange(list2);
                    }
                }

                result.Content = list;
                result.IsSuccess = true;
                result.ErrorMsg = null;

            }
            catch (Exception ex)
            {
                result.ErrorMsg = ex.Message;
                result.Content = list;
                result.IsSuccess = false;
            }

            return result;
        }
    }
}
