using GDRespApproverMatrixMaintenance.Business;
using GDRespApproverMatrixMaintenance.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;

namespace GDRespApproverMatrixMaintenance.API.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class ResponsibilityController : ApiController
    {
        //[Transaction]
        [HttpPost, Route("api/v1/Resp/ImportFromExcel")]
        //public async Task<HttpResponseMessage> ImportFromExcel([FromUri]string userName)
        public HttpResponseMessage ImportFromExcel([FromUri]string userName)
        {
            HttpRequest httpRequest = HttpContext.Current.Request;

            HttpResponseObject<ImportResult> obj = null;
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK);

            if (ResponsibilityBusiness.ImportingLocked)
            {
                //obj = new HttpResponseObject<MemoryStream>() { IsSuccess = false, Content = null, ErrorMsg = "System is busy now, please try again later." };
                response = Request.CreateResponse(HttpStatusCode.Conflict);
            }
            else
            {
                if (httpRequest.Files.Count > 0)
                {
                    foreach (string name in httpRequest.Files)
                    {
                        HttpPostedFile file = httpRequest.Files[name];

                        var threadID = Thread.CurrentThread.ManagedThreadId;

                        // multi-thread
                        // obj = await Task.Run(() =>
                        //{
                        //    return ResponsibilityBusiness.ImportRespFromExcel(file.InputStream);
                        //});

                        // Single thread
                        obj = ResponsibilityBusiness.ImportRespFromExcel(file.InputStream);


                        if (obj != null)
                        {
                            response = Request.CreateResponse(HttpStatusCode.OK, obj);
                            Business.Utilities.Utilities.WriteLog(userName, "ImportFromExcel successfully");
                        }
                    }
                }
            }

            return response;
        }


        [HttpGet, Route("api/v1/Resp/CancelImportFromExcel")]
        public HttpResponseMessage CancelImportFromExcel()
        {
            return Request.CreateResponse(HttpStatusCode.OK, ResponsibilityBusiness.TerminateImporting());
        }

        [HttpPost, Route("api/v1/Resp/GetRespApproverMatrix")]
        public HttpResponseMessage GetResponsibilityApproverMatrix([FromBody] RespApproverMatrixQuery query)
        {
            return Request.CreateResponse(HttpStatusCode.OK, ResponsibilityBusiness.GetResponsibilityApproverMatrix(query));
        }

        [HttpGet, Route("api/v1/Resp/GetSingleRespApproverMap")]
        public HttpResponseMessage GetSingleRespApproverMap([FromUri]int id)
        {
            return Request.CreateResponse(HttpStatusCode.OK, ResponsibilityBusiness.GetSingleRespApproverMap(id));
        }

        [HttpPost, Route("api/v1/Resp/UpdateRespApproverMap")]
        public HttpResponseMessage UpdateRespApproverMap([FromBody]RespApproverMap map)
        {
            return Request.CreateResponse(HttpStatusCode.OK, ResponsibilityBusiness.UpdateRespApproverMap(map));
        }

        [HttpGet, Route("api/v1/Resp/CheckIfUserExistsInGD")]
        public HttpResponseMessage CheckIfUserExistsInGD([FromUri]string p_email, [FromUri]string s_email, [FromUri]string f_email)
        {
            return Request.CreateResponse(HttpStatusCode.OK, ResponsibilityBusiness.CheckIfUserExistsInGD(p_email, s_email, f_email));
        }

        [HttpPost, Route("api/v1/Resp/DeleteRespApproverMap")]
        public HttpResponseMessage DeleteRespApproverMap([FromUri]int id = 0)
        {
            return Request.CreateResponse(HttpStatusCode.OK, ResponsibilityBusiness.DeleteRespApproverMap(id));
        }

        [HttpPost, Route("api/v1/Resp/ExportToExcel")]
        public HttpResponseMessage ExportToExcel(RespApproverMatrixQuery query)
        {
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK);

            if (query != null)
            {
                MemoryStream stream = ResponsibilityBusiness.ExportRespAppvMatrixToExcel(query);
                if (stream != null)
                {
                    stream.Position = 0;
                    response.Content = new StreamContent(stream);
                    response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/vnd.ms-excel");
                    response.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment")
                    {
                        FileName = query.Instance + " Responsibility Approver Matrix.xlsx"
                    };
                }
            }

            return response;
        }

    }
}
