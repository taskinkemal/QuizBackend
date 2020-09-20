using System;
using System.Threading.Tasks;
using BusinessLayer.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using WebCommon.Attributes;
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

            Models.TransferObjects.AuthToken actual = null;

            controller
                .SetupSet(p => p.Token = It.IsAny<Models.TransferObjects.AuthToken>())
                .Callback<Models.TransferObjects.AuthToken>(value => actual = value);

            var sut = new ExecutionFilterAttribute(authManager, true);

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

            var sut = new ExecutionFilterAttribute(authManager, true);

            var result = await sut.ValidateRequest(Mock.Of<IBaseController>(), tokenString);

            Assert.IsFalse(result.isValid);
            Assert.AreEqual("AccountNotVerified", result.errPhrase);
        }

        [TestMethod]
        public async Task ValidateRequestTokenNotValid()
        {
            const string tokenString = "sampletoken";

            var token = GetAuthToken(tokenString, false);
            var authManager = GetAuthManager(null);

            var sut = new ExecutionFilterAttribute(authManager, true);

            var result = await sut.ValidateRequest(Mock.Of<IBaseController>(), tokenString);

            Assert.IsFalse(result.isValid);
            Assert.AreEqual("InvalidToken", result.errPhrase);
        }

        [TestMethod]
        public async Task ValidateRequestNoToken()
        {
            var sut = new ExecutionFilterAttribute(Mock.Of<IAuthManager>(), true);

            var result = await sut.ValidateRequest(Mock.Of<IBaseController>(), null);

            Assert.IsFalse(result.isValid);
            Assert.AreEqual("InvalidToken", result.errPhrase);
        }

        [TestMethod]
        public async Task ValidateRequestInvalidController()
        {
            var sut = new ExecutionFilterAttribute(Mock.Of<IAuthManager>(), true);

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
