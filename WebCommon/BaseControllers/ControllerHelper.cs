using System;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Common;
using System.Linq;

namespace WebCommon.BaseControllers
{
    /// <summary>
    /// 
    /// </summary>
    public static class ControllerHelper
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="result"></param>
        /// <returns></returns>
        public static JsonResult CreateResponse<T>(T result)
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
        /// <returns></returns>
        public static JsonResult CreateErrorResponse(HttpStatusCode statusCode, string code)
        {
            return new JsonResult(new HttpErrorMessage(code))
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
