using GDRespApproverMatrixMaintenance.Business;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Configuration;

namespace GDRespApproverMatrixMaintenance.API.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class BaseController : ApiController
    {

        [HttpGet, Route("api/v1/base/GetFile")]
        public HttpResponseMessage GetFile([FromUri] string path)
        {
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK);

            FileStream file = File.Open(HttpContext.Current.Server.MapPath(path), FileMode.Open);
            MemoryStream stream = new MemoryStream();
            file.CopyTo(stream);
            if (stream != null)
            {
                stream.Position = 0;
                response.Content = new StreamContent(stream);
                response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/vnd.ms-excel");
                //response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream");
                //response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
                //response.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment")
                //{
                //    FileName = ".xlsx"
                //};
            }

            return response;
        }


        [HttpGet, Route("api/v1/base/Login")]
        public HttpResponseMessage Login()
        {
            HttpResponseObject<string> result = new HttpResponseObject<string>();

            try
            {
                string LoginAccounts = ConfigurationSettings.AppSettings["LoginAccounts"].ToLower().Replace("emrsn\\", "");
                 if (LoginAccounts.Split(new[] {';'},StringSplitOptions.RemoveEmptyEntries).Contains(HttpContext.Current.User.Identity.Name.ToLower().Replace("emrsn\\", "")))
                {
                    result.Content =  HttpContext.Current.User.Identity.Name;
                    result.IsSuccess = true;
                    Business.Utilities.Utilities.WriteLog(HttpContext.Current.User.Identity.Name, "Login");
                }
                else
                {
                    result.Content = "Sorry, you don't have permission to access.";
                    result.IsSuccess = false;
                    Business.Utilities.Utilities.WriteLog(HttpContext.Current.User.Identity.Name, "Login Failed");
                }

            }
            catch (Exception ex)
            {
                Business.Utilities.Utilities.WriteErrorLog(ex.Message, ex.StackTrace);
                result.IsSuccess = false;
                result.ErrorMsg = ex.Message;
            }
            return Request.CreateResponse(HttpStatusCode.OK, result);
        }
    }
}
