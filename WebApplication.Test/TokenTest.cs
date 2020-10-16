using System;
using System.Net;
using System.Threading.Tasks;
using BusinessLayer.Interfaces;
using Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using WebApplication.Controllers;

namespace WebApplication.Test
{
    [TestClass]
    public class TokenTest
    {
        [TestMethod]
        public async Task Post()
        {
            var validUntil = DateTime.Now.AddYears(1);

            var response = new Models.TransferObjects.AuthToken
            {
                ValidUntil = validUntil
            };
            var authManager = new Mock<IAuthManager>();
            authManager.Setup(c => c.GenerateTokenAsync(It.IsAny<Models.TransferObjects.TokenRequest>()))
                .Returns<Models.TransferObjects.TokenRequest>(r => Task.FromResult(response));

            var sut = new TokenController(authManager.Object);

            var result = await sut.Post(new Models.TransferObjects.TokenRequest());

            var resultObject = (Models.TransferObjects.AuthToken)result.Value;

            Assert.AreSame(response, resultObject);
            Assert.AreEqual(validUntil, resultObject.ValidUntil);
        }

        [TestMethod]
        public async Task PostNull()
        {
            var response = new Models.TransferObjects.AuthToken();
            var authManager = new Mock<IAuthManager>();
            authManager.Setup(c => c.GenerateTokenAsync(It.IsAny<Models.TransferObjects.TokenRequest>()))
                .Returns<Models.TransferObjects.TokenRequest>(r => Task.FromResult(default(Models.TransferObjects.AuthToken)));
            var sut = new TokenController(authManager.Object);

            var result = await sut.Post(new Models.TransferObjects.TokenRequest());

            Assert.AreEqual(HttpStatusCode.Unauthorized, (HttpStatusCode)result.StatusCode);
        }

        [TestMethod]
        public async Task GetValid()
        {
            var validUntil = DateTime.Now.AddYears(1);

            var response = new Models.TransferObjects.AuthToken
            {
                ValidUntil = validUntil
            };
            var authManager = new Mock<IAuthManager>();
            authManager.Setup(c => c.VerifyAccessToken(It.IsAny<string>()))
                .Returns<string>(r => Task.FromResult(response));

            var sut = new TokenController(authManager.Object);

            var result = await sut.Get("testtoken");

            var resultObject = (GenericWrapper<bool>)result.Value;

            Assert.IsTrue(resultObject.Value);
        }

        [TestMethod]
        public async Task GetExpired()
        {
            var validUntil = DateTime.Now.AddYears(-1);

            var response = new Models.TransferObjects.AuthToken
            {
                ValidUntil = validUntil
            };
            var authManager = new Mock<IAuthManager>();
            authManager.Setup(c => c.VerifyAccessToken(It.IsAny<string>()))
                .Returns<string>(r => Task.FromResult(response));

            var sut = new TokenController(authManager.Object);

            var result = await sut.Get("testtoken");

            var resultObject = (GenericWrapper<bool>)result.Value;

            Assert.IsFalse(resultObject.Value);
        }

        [TestMethod]
        public async Task GetNotFound()
        {
            var authManager = new Mock<IAuthManager>();
            authManager.Setup(c => c.VerifyAccessToken(It.IsAny<string>()))
                .Returns<string>(r => Task.FromResult(default(Models.TransferObjects.AuthToken)));

            var sut = new TokenController(authManager.Object);

            var result = await sut.Get("testtoken");

            var resultObject = (GenericWrapper<bool>)result.Value;

            Assert.IsFalse(resultObject.Value);
        }
    }
}
