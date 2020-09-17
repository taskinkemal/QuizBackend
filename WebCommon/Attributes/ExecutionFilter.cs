using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Linq;
using System.Net;
using System.Threading;
using Microsoft.AspNetCore.Http;
using Common;
using WebCommon.BaseControllers;
using System.Threading.Tasks;
using BusinessLayer.Interfaces;

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
            RetrieveParameters(context, out var accessToken);
            SetCulture();

            var result = ValidateRequest(context, accessToken).Result;

            if (result.isValid || !authenticationRequired)
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

        private async Task<(bool isValid, string errPhrase)> ValidateRequest(ActionExecutingContext context, string accessToken)
        {
            string errPhrase;

            if (accessToken != null)
            {
                if (context.Controller is BaseController apiController)
                {
                    var userToken = await authManager.VerifyAccessToken(accessToken);
                    if (userToken != null)
                    {
                        if (userToken.IsVerified)
                        {
                            apiController.Token = userToken;
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

        private static void RetrieveParameters(ActionContext context, out string accessToken)
        {
            accessToken = GetHeader(context.HttpContext.Request.Headers, "Authorization", "");
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
    }
}
