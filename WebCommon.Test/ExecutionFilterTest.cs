using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using BusinessLayer.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Primitives;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Models.TransferObjects;
using Moq;
using WebCommon.Attributes;
using WebCommon.BaseControllers;
using WebCommon.Interfaces;

namespace WebCommon.Test
{
    [TestClass]
    public class ExecutionFilterTest
    {
        [TestMethod]
        public async Task ValidateRequest()
        {
            const string tokenString = "sampletoken";

            var controller = new Mock<IBaseController>();

            var token = GetAuthToken(tokenString, true);
            var authManager = GetAuthManager(token);

            AuthToken actual = null;

            controller
                .SetupSet(p => p.Token = It.IsAny<AuthToken>())
                .Callback<AuthToken>(value => actual = value);

            var sut = new ExecutionFilterAttribute(authManager, Mock.Of<IContextManager>(), true);

            var result = await sut.ValidateRequest(controller.Object, tokenString);

            Assert.AreSame(token, actual);
            Assert.IsTrue(result.isValid);
            Assert.AreEqual("", result.errPhrase);
        }

        [TestMethod]
        public async Task ValidateRequestNotVerified()
        {
            const string tokenString = "sampletoken";

            var token = GetAuthToken(tokenString, false);
            var authManager = GetAuthManager(token);

            var sut = new ExecutionFilterAttribute(authManager, Mock.Of<IContextManager>(), true);

            var result = await sut.ValidateRequest(Mock.Of<IBaseController>(), tokenString);

            Assert.IsFalse(result.isValid);
            Assert.AreEqual("AccountNotVerified", result.errPhrase);
        }

        [TestMethod]
        public async Task ValidateRequestTokenNotValid()
        {
            const string tokenString = "sampletoken";

            var authManager = GetAuthManager(null);

            var sut = new ExecutionFilterAttribute(authManager, Mock.Of<IContextManager>(), true);

            var result = await sut.ValidateRequest(Mock.Of<IBaseController>(), tokenString);

            Assert.IsFalse(result.isValid);
            Assert.AreEqual("InvalidToken", result.errPhrase);
        }

        [TestMethod]
        public async Task ValidateRequestNoToken()
        {
            var sut = new ExecutionFilterAttribute(Mock.Of<IAuthManager>(), Mock.Of<IContextManager>(), true);

            var result = await sut.ValidateRequest(Mock.Of<IBaseController>(), null);

            Assert.IsFalse(result.isValid);
            Assert.AreEqual("InvalidToken", result.errPhrase);
        }

        [TestMethod]
        public async Task ValidateRequestInvalidController()
        {
            var sut = new ExecutionFilterAttribute(Mock.Of<IAuthManager>(), Mock.Of<IContextManager>(), true);

            var result = await sut.ValidateRequest(null, "token");

            Assert.IsFalse(result.isValid);
            Assert.AreEqual("InvalidController", result.errPhrase);
        }

        [TestMethod]
        public void RetrieveParameters()
        {
            const string token = "sampletoken";

            var headers = new HeaderDictionary();
            headers.Add("Authorization", "Bearer " + token);

            string actual;
            ExecutionFilterAttribute.RetrieveParameters(headers, out actual);

            Assert.AreEqual(actual, token);
        }

        [TestMethod]
        public void RetrieveParametersNoToken()
        {
            var headers = new HeaderDictionary();

            string actual;
            ExecutionFilterAttribute.RetrieveParameters(headers, out actual);

            Assert.IsNull(actual);
        }

        [TestMethod]
        public void RetrieveParametersFormatMismatch1()
        {
            const string token = "sampletoken";

            var headers = new HeaderDictionary();
            headers.Add("Authorization", "Bearer" + token);

            string actual;
            ExecutionFilterAttribute.RetrieveParameters(headers, out actual);

            Assert.IsNull(actual);
        }

        [TestMethod]
        public void RetrieveParametersFormatMismatch2()
        {
            const string token = "sampletoken";

            var headers = new HeaderDictionary();
            headers.Add("Authorization", token);

            string actual;
            ExecutionFilterAttribute.RetrieveParameters(headers, out actual);

            Assert.IsNull(actual);
        }

        [TestMethod]
        public void RetrieveParametersFormatMismatch3()
        {
            var headers = new HeaderDictionary();
            headers.Add("Authorization", "");

            string actual;
            ExecutionFilterAttribute.RetrieveParameters(headers, out actual);

            Assert.IsNull(actual);
        }

        [TestMethod]
        public void HasAuthenticateAttributeReturnsTrue()
        {
            var descriptor = new ControllerActionDescriptor
            {
                MethodInfo = GetMethodInfo<ExecutionFilterTest>(x => x.TestControllerWithAuthenticateAttribute())
            };

            var result = ExecutionFilterAttribute.HasAuthenticateAttribute(descriptor);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void HasAuthenticateAttributeReturnsFalse()
        {
            var descriptor = new ControllerActionDescriptor
            {
                MethodInfo = GetMethodInfo<ExecutionFilterTest>(x => x.TestControllerWithoutAuthenticateAttribute())
            };

            var result = ExecutionFilterAttribute.HasAuthenticateAttribute(descriptor);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void HasAuthenticateAttributeMethodIsNull()
        {
            var result = ExecutionFilterAttribute.HasAuthenticateAttribute(new ControllerActionDescriptor());

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void HasAuthenticateAttributeDescriptorIsNull()
        {
            var result = ExecutionFilterAttribute.HasAuthenticateAttribute(null);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void ProceedWithExecutionReturnsTrueWhenValid()
        {
            var result = ExecutionFilterAttribute.ProceedWithExecution(true, true, true);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void ProceedWithExecutionReturnsTrueWhenAuthenticationIsNotRequiredAndWithNoAttribute()
        {
            var result = ExecutionFilterAttribute.ProceedWithExecution(false, false, false);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void ProceedWithExecutionReturnsFalseWhenAuthenticationIsNotRequiredAndWithAttribute()
        {
            var result = ExecutionFilterAttribute.ProceedWithExecution(false, false, true);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void ProceedWithExecutionReturnsFalseWhenAuthenticationIsRequired()
        {
            var result = ExecutionFilterAttribute.ProceedWithExecution(false, true, false);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void SetCulture()
        {
            var actualCultureName = "";
            var actualUiCultureName = "";

            var thread = new Thread(() =>
            {
                ExecutionFilterAttribute.SetCulture(Thread.CurrentThread);

                actualCultureName = Thread.CurrentThread.CurrentCulture.Name;
                actualUiCultureName = Thread.CurrentThread.CurrentUICulture.Name;
            });

            thread.Start();
            thread.Join();

            Assert.AreEqual("en-US", actualCultureName);
            Assert.AreEqual("en-US", actualUiCultureName);
        }

        [TestMethod]
        public void OnActionExecuting()
        {
            const bool authenticationRequired = true;
            const string token = "MyToken";

            var authManager = new Mock<IAuthManager>();
            authManager.Setup(c => c.VerifyAccessToken(It.IsAny<string>())).Returns<string>(value =>
            {
                if (value == token)
                {
                    return Task.FromResult(new AuthToken
                    {
                        IsVerified = true,
                        Token = value,
                        UserId = 1,
                        ValidUntil = DateTime.Now.AddDays(1)
                    });
                }

                return Task.FromResult(default(AuthToken));
            });

            var sut = new ExecutionFilterAttribute(authManager.Object, Mock.Of<IContextManager>(), authenticationRequired);

            var dictionary =
                new HeaderDictionary(new Dictionary<string, StringValues> {{"Authorization", "Bearer " + token}});

            var feature = new Mock<IHttpRequestFeature>();
            feature.Setup(c => c.Headers).Returns(dictionary);

            var controller = new BaseController();

            var httpContext = new DefaultHttpContext();
            httpContext.Features.Set(feature.Object);
            var actionContext = new ActionContext(httpContext,
                new RouteData(new RouteValueDictionary()), new ControllerActionDescriptor());
            var context = new ActionExecutingContext(new ControllerContext(actionContext), new List<IFilterMetadata>(),
                new ConcurrentDictionary<string, object>(), controller);

            var thread = new Thread(() =>
            {
                sut.OnActionExecuting(context);
            });

            thread.Start();
            thread.Join();

            Assert.AreEqual(token, controller.Token.Token);
        }

        [TestMethod]
        public void OnActionExecutingTokenInvalid()
        {
            const bool authenticationRequired = true;
            const string token = "MyToken";

            var authManager = new Mock<IAuthManager>();
            authManager.Setup(c => c.VerifyAccessToken(It.IsAny<string>())).Returns<string>(value =>
            {
                if (value == token)
                {
                    return Task.FromResult(new AuthToken
                    {
                        IsVerified = true,
                        Token = value,
                        UserId = 1,
                        ValidUntil = DateTime.Now.AddDays(1)
                    });
                }

                return Task.FromResult(default(AuthToken));
            });

            var sut = new ExecutionFilterAttribute(authManager.Object, Mock.Of<IContextManager>(), authenticationRequired);

            var dictionary =
                new HeaderDictionary(new Dictionary<string, StringValues> { { "Authorization", "Bearer " + token + "somesuffix" } });

            var feature = new Mock<IHttpRequestFeature>();
            feature.Setup(c => c.Headers).Returns(dictionary);

            var controller = new BaseController();

            var httpContext = new DefaultHttpContext();
            httpContext.Features.Set(feature.Object);
            var actionContext = new ActionContext(httpContext,
                new RouteData(new RouteValueDictionary()), new ControllerActionDescriptor());
            var context = new ActionExecutingContext(new ControllerContext(actionContext), new List<IFilterMetadata>(),
                new ConcurrentDictionary<string, object>(), controller);

            var thread = new Thread(() =>
            {
                sut.OnActionExecuting(context);
            });

            thread.Start();
            thread.Join();

            Assert.IsNull(controller.Token);
        }

        private static MethodInfo GetMethodInfo<T>(Expression<Action<T>> expression)
        {
            if (expression.Body is MethodCallExpression member)
                return member.Method;

            throw new ArgumentException("Expression is not a method", "expression");
        }

        [Authenticate]
        private void TestControllerWithAuthenticateAttribute()
        {
            // Method intentionally left empty.
        }

        private void TestControllerWithoutAuthenticateAttribute()
        {
            // Method intentionally left empty.
        }

        private Models.TransferObjects.AuthToken GetAuthToken(string tokenString, bool isVerified)
        {
            return new Models.TransferObjects.AuthToken
            {
                Token = tokenString,
                UserId = 1,
                ValidUntil = DateTime.Now.AddYears(1),
                IsVerified = isVerified
            };
        }

        private IAuthManager GetAuthManager(Models.TransferObjects.AuthToken token)
        {
            var authManager = new Mock<IAuthManager>();

            authManager
                .Setup(c => c.VerifyAccessToken(It.IsAny<string>()))
                .Returns((string s) => s == token?.Token ?
                    Task.FromResult(token) :
                    Task.FromResult(default(Models.TransferObjects.AuthToken)));

            return authManager.Object;
        }
    }
}
