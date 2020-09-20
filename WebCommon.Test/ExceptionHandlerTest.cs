using System;
using System.Collections.Generic;
using Common.Interfaces;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Serilog.Events;
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
                .Setup(c => c.AddLog(It.IsAny<Exception>(), It.IsAny<string>(), It.IsAny<LogEventLevel>()))
                .Callback((Exception e, string s, LogEventLevel l) => { actual = e; });

            var sut = new ExceptionHandlerAttribute(logManager.Object);
            var context = new ExceptionContext(
                new Microsoft.AspNetCore.Mvc.ActionContext(Mock.Of<Microsoft.AspNetCore.Http.HttpContext>(),
                Mock.Of<RouteData>(), Mock.Of<ActionDescriptor>()), new List<IFilterMetadata>());

            var result = sut.ProcessException(exception);

            Assert.AreSame(actual, exception);
            Assert.AreEqual(result.status, System.Net.HttpStatusCode.InternalServerError);
            Assert.AreEqual(result.code, "SystemError");
        }

        [TestMethod]
        public void OnExceptionNotImplemented()
        {
            var sut = new ExceptionHandlerAttribute(Mock.Of<ILogManager>());
            var context = new ExceptionContext(
                new Microsoft.AspNetCore.Mvc.ActionContext(Mock.Of<Microsoft.AspNetCore.Http.HttpContext>(),
                Mock.Of<RouteData>(), Mock.Of<ActionDescriptor>()), new List<IFilterMetadata>());

            var result = sut.ProcessException(new NotImplementedException());
            
            Assert.AreEqual(result.status, System.Net.HttpStatusCode.NotImplemented);
            Assert.AreEqual(result.code, "MethodNotImplemented");
        }
    }
}
