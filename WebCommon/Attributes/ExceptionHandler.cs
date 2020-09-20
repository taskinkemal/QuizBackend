using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Net;
using Common;
using Common.Interfaces;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("WebCommon.Test")]
namespace WebCommon.Attributes
{
    /// <summary>
    /// 
    /// </summary>
    public class ExceptionHandlerAttribute : ExceptionFilterAttribute
    {
        private readonly ILogManager logManager;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="logManager"></param>
        public ExceptionHandlerAttribute(ILogManager logManager)
        {
            this.logManager = logManager;
        }

        /// <summary>
        /// Captures unhandled exceptions.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override Task OnExceptionAsync(ExceptionContext context)
        {
            var result = ProcessException(context.Exception);

            context.HttpContext.Response.StatusCode = (int)result.status;
            context.Result = new JsonResult(new HttpErrorMessage(result.code));

            return base.OnExceptionAsync(context);
        }

        internal (HttpStatusCode status, string code) ProcessException(Exception exception)
        {
            logManager.AddLog(exception);

            HttpStatusCode status;
            string code;

            if (exception is NotImplementedException)
            {
                status = HttpStatusCode.NotImplemented;
                code = "MethodNotImplemented";
            }
            else
            {
                status = HttpStatusCode.InternalServerError;
                code = "SystemError";
            }

            return (status, code);
        }
    }
}