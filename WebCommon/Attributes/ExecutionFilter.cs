using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Linq;
using System.Net;
using System.Threading;
using Microsoft.AspNetCore.Http;
using Common;
using System.Threading.Tasks;
using BusinessLayer.Interfaces;
using Microsoft.AspNetCore.Mvc.Controllers;
using WebCommon.Interfaces;
using System.Runtime.CompilerServices;
using WebCommon.BaseControllers;

[assembly: InternalsVisibleTo("WebCommon.Test")]
namespace WebCommon.Attributes
{
    /// <summary>
    /// 
    /// </summary>
    public class ExecutionFilterAttribute : ActionFilterAttribute
    {
        private readonly IAuthManager authManager;
        private readonly IContextManager contextManager;
        private readonly bool authenticationRequired;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="authManager"></param>
        /// <param name="contextManager"></param>
        /// <param name="authenticationRequired"></param>
        public ExecutionFilterAttribute(IAuthManager authManager, IContextManager contextManager, bool authenticationRequired)
        {
            this.authManager = authManager;
            this.contextManager = contextManager;
            this.authenticationRequired = authenticationRequired;
        }

        /// <summary>
        /// Policy injection for all endpoints.
        /// </summary>
        /// <param name="context"></param>
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            RetrieveParameters(context.HttpContext.Request.Headers, out var accessToken);
            SetCulture(Thread.CurrentThread);

            contextManager.BeginTransaction();

            var result = ValidateRequest(context.Controller as IBaseController, accessToken).Result;

            if (ProceedWithExecution(result.isValid, authenticationRequired, HasAuthenticateAttribute(context.ActionDescriptor as ControllerActionDescriptor)))
            {
                base.OnActionExecuting(context);
            }
            else
            {
                context.Result = ControllerHelper.CreateErrorResponse(HttpStatusCode.Unauthorized, result.errPhrase);
            }
        }

        internal async Task<(bool isValid, string errPhrase)> ValidateRequest(IBaseController controller, string accessToken)
        {
            string errPhrase;

            if (accessToken != null)
            {
                if (controller != null)
                {
                    var userToken = await authManager.VerifyAccessToken(accessToken);
                    if (userToken != null)
                    {
                        if (userToken.IsVerified)
                        {
                            controller.Token = userToken;
                        }
                        else
                        {
                            errPhrase = "AccountNotVerified";
                            return (false, errPhrase);
                        }
                    }
                    else
                    {
                        errPhrase = "InvalidToken";
                        return (false, errPhrase);
                    }
                }
                else
                {
                    errPhrase = "InvalidController";
                    return (false, errPhrase);
                }
            }
            else
            {
                errPhrase = "InvalidToken";
                return (false, errPhrase);
            }

            return (true, "");
        }

        internal static void RetrieveParameters(IHeaderDictionary headers, out string accessToken)
        {
            accessToken = GetHeader(headers, "Authorization", "");
            if (!string.IsNullOrWhiteSpace(accessToken) && accessToken.StartsWith("Bearer ", StringComparison.InvariantCulture))
            {
                accessToken = accessToken.Substring("Bearer ".Length);
            }
            else
            {
                accessToken = null;
            }
        }

        private static string GetHeader(IHeaderDictionary headers, string key, string defaultValue)
        {
            return headers.ContainsKey(key) ? Convertor.ToString(headers[key].First(), defaultValue) : defaultValue;
        }

        internal static void SetCulture(Thread t)
        {
            t.CurrentCulture = new System.Globalization.CultureInfo("en-US");
            t.CurrentUICulture = new System.Globalization.CultureInfo("en-US");
        }

        internal static bool ProceedWithExecution(bool isValid, bool authenticationRequired, bool hasAuthenticateAttribute)
        {
            return isValid || !(authenticationRequired || hasAuthenticateAttribute);
        }

        internal static bool HasAuthenticateAttribute(ControllerActionDescriptor descriptor)
        {
            return descriptor?.MethodInfo?.GetCustomAttributes(true).Any(a => a is AuthenticateAttribute) ?? false;
        }
    }
}
