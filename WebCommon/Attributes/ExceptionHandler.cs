using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;
using System;
using System.Net;
using Common;
using Common.Interfaces;
using System.Threading.Tasks;

namespace WebCommon.Attributes
{
    /// <summary>
    /// 
    /// </summary>
    public class ExceptionHandlerAttribute : ExceptionFilterAttribute
    {
        private readonly IOptions<AppSettings> settings;
        private readonly ILogManager logManager;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="settings"></param>
        /// <param name="logManager"></param>
        public ExceptionHandlerAttribute(IOptions<AppSettings> settings, ILogManager logManager)
        {
            this.settings = settings;
            this.logManager = logManager;
        }

        /// <summary>
        /// Captures unhandled exceptions.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override Task OnExceptionAsync(ExceptionContext context)
        {
            logManager.AddLog(context.Exception);

            HttpStatusCode status;
            string code;

            if (context.Exception is NotImplementedException)
            {
                status = HttpStatusCode.NotImplemented;
                code = "MethodNotImplemented";
            }
            else
            {
                status = HttpStatusCode.InternalServerError;
                code = "SystemError";
            }

            context.HttpContext.Response.StatusCode = (int)status;
            context.Result = new JsonResult(new HttpErrorMessage(code));

            return base.OnExceptionAsync(context);
        }
    }
}