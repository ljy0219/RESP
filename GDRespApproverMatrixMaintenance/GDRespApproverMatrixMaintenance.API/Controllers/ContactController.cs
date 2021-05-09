using GDRespApproverMatrixMaintenance.Business;
using GDRespApproverMatrixMaintenance.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;

namespace GDRespApproverMatrixMaintenance.API.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class ContactController : ApiController
    {
        [HttpPost, Route("api/v1/contact/ImportFromExcel")]
        public async Task<HttpResponseMessage> ImportFromExcel([FromUri]string userName)
        {
            HttpRequest httpRequest = HttpContext.Current.Request;

            HttpResponseObject<ImportResult> result = null;
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK);

            if (HRISContactBusiness.ImportingLocked)
            {
                response = Request.CreateResponse(HttpStatusCode.Conflict);
            }
            else
            {
                if (httpRequest.Files.Count > 0)
                {
                    foreach (string name in httpRequest.Files)
                    {
                        HttpPostedFile file = httpRequest.Files[name];

                        //result = await Task.Run(() =>
                        //{
                        //    return HRISContactBusiness.ImportHRISContactFromExcel(file);
                        //});

                        result = HRISContactBusiness.ImportHRISContactFromExcel(file);


                        if (result != null )
                        {
                            //obj.Content.Position = 0;
                            //response.Content = new StreamContent(obj.Content);
                            //response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/vnd.ms-excel");
                            //response.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment")
                            //{
                            //    FileName = "Exceptional Contacts.xlsx"
                            //};

                            response = Request.CreateResponse(HttpStatusCode.OK, result);
                            Business.Utilities.Utilities.WriteLog(userName, "ImportFromExcel successfully");
                        }
                    }
                }
            }

            return response;
        }

        
        [HttpPost, Route("api/v1/contact/GetHRISContact")]
        public HttpResponseMessage GetHRISContact([FromBody]HRISContactQuery query)
        {
            return Request.CreateResponse(HttpStatusCode.OK, HRISContactBusiness.GetHRISContact(query));
        }


        [HttpGet, Route("api/v1/contact/ExportToExcel")]
        public HttpResponseMessage ExportToExcel([FromUri] string resp = "", [FromUri]string org = "",[FromUri] string contact="")
        {
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK);
            MemoryStream stream = HRISContactBusiness.ExportHRISContactToExcel(resp, org,contact);
            if (stream != null)
            {
                stream.Position = 0;
                response.Content = new StreamContent(stream);
                response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/vnd.ms-excel");
                response.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment")
                {
                    FileName = " HRIS Contacts.xlsx"
                };
            }

            return response;
        }

        [HttpGet, Route("api/v1/contact/GetSingleHRISContact")]
        public HttpResponseMessage GetSingleHRISContact([FromUri]int id = 0)
        {
            return Request.CreateResponse(HttpStatusCode.OK, HRISContactBusiness.GetSingleHRISContact(id));
        }


        [HttpPost, Route("api/v1/contact/UpdateHRISContact")]
        public HttpResponseMessage UpdateHRISContact([FromBody] HRISContact contact)
        {
            return Request.CreateResponse(HttpStatusCode.OK, HRISContactBusiness.UpdateSingleHRISContact(contact));
        }

        [HttpDelete, Route("api/v1/contact/DeleteHRISContact")]
        public HttpResponseMessage DeleteHRISContact([FromUri] int id = 0)
        {
            return Request.CreateResponse(HttpStatusCode.OK, HRISContactBusiness.DeleteHRISContact(id));
        }

        [HttpGet, Route("api/v1/contact/CheckHRISContact")]
        public HttpResponseMessage CheckHRISContact([FromUri] string c1, [FromUri]string c2 = "")
        {
            return Request.CreateResponse(HttpStatusCode.OK, HRISContactBusiness.CheckHRISContact(c1, c2));
        }

    }
}
