using Microsoft.AspNetCore.Mvc;
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

[assembly: InternalsVisibleTo("WebCommon.Test")]
namespace WebCommon.Attributes
{
    /// <summary>
    /// 
    /// </summary>
    public class ExecutionFilterAttribute : ActionFilterAttribute
    {
        private readonly IAuthManager authManager;
        private readonly bool authenticationRequired;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="authManager"></param>
        /// <param name="authenticationRequired"></param>
        public ExecutionFilterAttribute(IAuthManager authManager, bool authenticationRequired)
        {
            this.authManager = authManager;
            this.authenticationRequired = authenticationRequired;
        }

        /// <summary>
        /// Policy injection for all endpoints.
        /// </summary>
        /// <param name="context"></param>
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            RetrieveParameters(context.HttpContext.Request.Headers, out var accessToken);
            SetCulture();

            var result = ValidateRequest(context.Controller as IBaseController, accessToken).Result;

            if (result.isValid || !(authenticationRequired || HasAuthenticateAttribute(context)))
            {
                base.OnActionExecuting(context);
            }
            else
            {
                context.Result = new JsonResult(new HttpErrorMessage(result.errPhrase))
                {
                    StatusCode = (int)HttpStatusCode.Unauthorized
                };
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

        private static void SetCulture()
        {
            Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");
            Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("en-US");
        }

        private static bool HasAuthenticateAttribute(ActionExecutingContext context)
        {
            var controllerActionDescriptor = context.ActionDescriptor as ControllerActionDescriptor;

            if (controllerActionDescriptor != null)
            {
                // Check if the attribute exists on the action method
                if (controllerActionDescriptor.MethodInfo?.GetCustomAttributes(inherit: true)?.Any(a => a.GetType().Equals(typeof(AuthenticateAttribute))) ?? false)
                    return true;
            }

            return false;
        }
    }
}
