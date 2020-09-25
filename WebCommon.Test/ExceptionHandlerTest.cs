using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using BusinessLayer.Interfaces;
using Common;
using Common.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using WebCommon.Attributes;

namespace WebCommon.Test
{
    [TestClass]
    public class ExceptionHandlerTest
    {
        [TestMethod]
        public void OnExceptionLog()
        {
            var logManager = new Mock<ILogManager>();
            Exception actual = null;
            Exception exception = new Exception();

            logManager
                .Setup(c => c.AddLog(LogCategory.GeneralError, It.IsAny<Exception>(), It.IsAny<object[]>()))
                .Callback((LogCategory category, Exception e, object[] logArguments) => { actual = e; });

            var sut = new ExceptionHandlerAttribute(Mock.Of<IContextManager>(), logManager.Object);

            var result = sut.ProcessException(exception);

            Assert.AreSame(actual, exception);
            Assert.AreEqual(System.Net.HttpStatusCode.InternalServerError, result.status);
            Assert.AreEqual("SystemError", result.code);
        }

        [TestMethod]
        public void OnExceptionNotImplemented()
        {
            var sut = new ExceptionHandlerAttribute(Mock.Of<IContextManager>(), Mock.Of<ILogManager>());

            var result = sut.ProcessException(new NotImplementedException());
            
            Assert.AreEqual(System.Net.HttpStatusCode.NotImplemented, result.status);
            Assert.AreEqual("MethodNotImplemented", result.code);
        }

        [TestMethod]
        public async Task HandleException()
        {
            var sut = new ExceptionHandlerAttribute(Mock.Of<IContextManager>(), Mock.Of<ILogManager>());
            var actualStatusCode = 0;

            var feature = new Mock<IHttpResponseFeature>();
            feature.SetupSet<int>(c => c.StatusCode = It.IsAny<int>()).Callback(value => actualStatusCode = value);


            var httpContext = new DefaultHttpContext();
            httpContext.Features.Set(feature.Object);

            var exception = new Exception("test");

            var context = new ExceptionContext(new ActionContext(httpContext,
                new RouteData(new RouteValueDictionary()), new ActionDescriptor()), new List<IFilterMetadata>())
            {
                Exception = exception
            };

            await sut.OnExceptionAsync(context);

            Assert.AreEqual((int)HttpStatusCode.InternalServerError, actualStatusCode);
        }
    }
}
