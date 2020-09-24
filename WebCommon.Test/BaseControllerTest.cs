using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebCommon.BaseControllers;
using WebCommon.Attributes;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace WebCommon.Test
{
    [TestClass]
    public class BaseControllerTest
    {
        [TestMethod]
        public void NoAuthControllerAttribute()
        {
            var result = RequiresAuthentication<NoAuthController>();

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void AuthControllerAttribute()
        {
            var result = RequiresAuthentication<AuthController>();

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void BaseControllerAttribute()
        {
            var attribute = GetTypeFilterAttribute<ExceptionHandlerAttribute>(typeof(BaseController));

            Assert.IsNotNull(attribute);
        }

        private TypeFilterAttribute GetTypeFilterAttribute<T>(Type type)
        {
            return Attribute.GetCustomAttributes(type, typeof(TypeFilterAttribute))
                .OfType<TypeFilterAttribute>()
                .First(a => a.ImplementationType == typeof(T));
        }


        private bool RequiresAuthentication<T>() where T: BaseController
        {
            var attribute = GetTypeFilterAttribute<ExecutionFilterAttribute>(typeof(T));

            return (bool)attribute.Arguments[0];
        }
    }
}
