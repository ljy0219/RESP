using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GDRespApproverMatrixMaintenance.API
{
    public class CustomAuthorizeAttribute : System.Web.Mvc.AuthorizeAttribute
    {

        private readonly string[] allowedroles;
        public CustomAuthorizeAttribute()
        { }
        public CustomAuthorizeAttribute(params string[] roles)
        {
            this.allowedroles = roles;
        }  
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            if (httpContext == null)
            {
                throw new ArgumentNullException("HttpContext");
            }
            if (!httpContext.User.Identity.IsAuthenticated)
            {
                return false;
            }
            return true;
        }


        public override void OnAuthorization(System.Web.Mvc.AuthorizationContext filterContext)
        {
            base.OnAuthorization(filterContext);
        }
    }
}