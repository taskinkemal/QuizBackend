using System;
using Microsoft.AspNetCore.Mvc;
using WebCommon.Attributes;
using System.Net;
using Common;
using System.Linq;
using Models.TransferObjects;

namespace WebCommon.BaseControllers
{
    /// <inheritdoc />
    /// <summary>
    /// </summary>
    [Route("api/[controller]")]
    [TypeFilter(typeof(ExceptionHandlerAttribute))]
    public class BaseController : Controller
    {
        /// <summary>
        /// 
        /// </summary>
        public AuthToken Token = null;

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="result"></param>
        /// <returns></returns>
        protected static JsonResult CreateResponse<T>(T result)
        {
            return IsSimpleType(typeof(T))
                ? new JsonResult(GenericWrapper<T>.Wrap(result))
                {
                    StatusCode = (int)HttpStatusCode.OK
                }
                : new JsonResult(result)
                {
                    StatusCode = (int)HttpStatusCode.OK
                };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="statusCode"></param>
        /// <param name="code"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        protected static JsonResult CreateErrorResponse(HttpStatusCode statusCode, string code, string message)
        {
            return new JsonResult(new HttpErrorMessage(code, message))
            {
                StatusCode = (int)statusCode
            };
        }

        private static bool IsSimpleType(Type type)
        {
            return
                type.IsPrimitive ||
                new[] {
                    typeof(Enum),
                    typeof(string),
                    typeof(decimal),
                    typeof(DateTime),
                    typeof(DateTimeOffset),
                    typeof(TimeSpan),
                    typeof(Guid)
                }.Contains(type) ||
                Convert.GetTypeCode(type) != TypeCode.Object ||
                (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>) && IsSimpleType(type.GetGenericArguments()[0]))
                ;
        }
    }
}
