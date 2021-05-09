using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
//using System.Web.Mvc;

namespace GDRespApproverMatrixMaintenance.API
{
    public class TransactionAttribute :ActionFilterAttribute
    {
        private const string transactionId = "TransactionToken";

        //public override void OnActionExecuting(ActionExecutingContext filterContext)
        //{
        //    base.OnActionExecuting(filterContext);

        //        var values = filterContext.HttpContext.Request.Headers.GetValues(transactionId);
        //        if (values != null && values.Any())
        //        {
        //            byte[] transactionToken = Convert.FromBase64String(values.FirstOrDefault());
        //            var transaction = TransactionInterop.GetTransactionFromTransmitterPropagationToken(transactionToken);
        //            var transactionScope = new TransactionScope(transaction);
        //        }
        //}

        public override void OnActionExecuting(HttpActionContext filterContext)
        {
            //base.OnActionExecuting(filterContext);

            if (filterContext.Request.Headers.Contains(transactionId))
            {
                var values = filterContext.Request.Headers.GetValues(transactionId);
                if (values != null && values.Any())
                {
                    byte[] transactionToken = Convert.FromBase64String(values.FirstOrDefault());
                    var transaction = TransactionInterop.GetTransactionFromTransmitterPropagationToken(transactionToken);
                    var transactionScope = new TransactionScope(transaction);
                    filterContext.Request.Properties.Add(transactionId, transactionScope);
                }
            }
        }

        //public override void OnActionExecuted(ActionExecutedContext filterContext)
        public override void OnActionExecuted(HttpActionExecutedContext executedContext)
        {
            if (executedContext.Request.Properties.Keys.Contains(transactionId))
            {
                var transactionScope = executedContext.Request.Properties[transactionId] as TransactionScope;

                if (transactionScope != null)
                {
                    if (executedContext.Exception != null)
                    {
                        Transaction.Current.Rollback();
                    }
                    else {
                        transactionScope.Complete();
                    }

                    transactionScope.Dispose();
                    executedContext.Request.Properties[transactionId] = null;
                }

            }

        }

    }
}