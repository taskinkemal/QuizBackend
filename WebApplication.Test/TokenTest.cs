using System;
using System.Net;
using System.Threading.Tasks;
using BusinessLayer.Interfaces;
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
    }
}
