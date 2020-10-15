using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebCommon.BaseControllers;
using WebCommon.Attributes;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using Models.TransferObjects;
using Common;

namespace WebCommon.Test
{
    [TestClass]
    public class BaseControllerTest
    {
        [TestMethod]
        public void NoAuthControllerAttribute()
        {
            var result = RequiresAuthentication<NoAuthController>();

            Assert.AreEqual(AuthenticationLevel.NoAuthentication, result);
        }

        [TestMethod]
        public void AuthControllerAttribute()
        {
            var result = RequiresAuthentication<AuthController>();

            Assert.AreEqual(AuthenticationLevel.User, result);
        }

        [TestMethod]
        public void ManagementAuthControllerAttribute()
        {
            var result = RequiresAuthentication<ManagementController>();

            Assert.AreEqual(AuthenticationLevel.Admin, result);
        }

        [TestMethod]
        public void BaseControllerAttribute()
        {
            var attribute = GetTypeFilterAttribute<ExceptionHandlerAttribute>(typeof(BaseController));

            Assert.IsNotNull(attribute);
        }

        [TestMethod]
        public void BaseControllerConstructorTokenIsNull()
        {
            var sut = new BaseController();
            
            Assert.IsNull(sut.Token);
        }

        [TestMethod]
        public void BaseControllerConstructorTokenIsSet()
        {
            var sut = new BaseController();
            var token = new AuthToken();

            sut.Token = token;

            Assert.AreSame(token, sut.Token);
        }

        private TypeFilterAttribute GetTypeFilterAttribute<T>(Type type)
        {
            return Attribute.GetCustomAttributes(type, typeof(TypeFilterAttribute))
                .OfType<TypeFilterAttribute>()
                .First(a => a.ImplementationType == typeof(T));
        }


        private AuthenticationLevel RequiresAuthentication<T>() where T: BaseController
        {
            var attribute = GetTypeFilterAttribute<ExecutionFilterAttribute>(typeof(T));

            return (AuthenticationLevel)attribute.Arguments[0];
        }
    }
}
