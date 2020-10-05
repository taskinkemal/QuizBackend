using System;
using System.Net;
using System.Threading.Tasks;
using BusinessLayer.Interfaces;
using Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Models.TransferObjects;
using Moq;
using WebApplication.Controllers;

namespace WebApplication.Test
{
    [TestClass]
    public class UsersTest
    {
        [TestMethod]
        public async Task Put()
        {
            var result = await ExecutePut(InsertUserResponse.Success);

            var resultObject = (AuthToken)result.Value;

            Assert.AreEqual(HttpStatusCode.OK, (HttpStatusCode)result.StatusCode);
            Assert.IsNotNull(resultObject);
        }

        [DataTestMethod]
        [DataRow(HttpStatusCode.Conflict, InsertUserResponse.EmailExists)]
        [DataRow(HttpStatusCode.NotAcceptable, InsertUserResponse.PasswordCriteriaNotSatisfied)]
        public async Task PutError(HttpStatusCode expected, InsertUserResponse result)
        {
            var actual = await ExecutePut(result);

            Assert.AreEqual(expected, (HttpStatusCode)actual.StatusCode);
        }

        [TestMethod]
        public async Task Post()
        {
            var result = await ExecutePost(true, true);

            var resultObject = (GenericWrapper<bool>)result.Value;

            Assert.IsTrue(resultObject.Value);
        }

        [TestMethod]
        public async Task PostError()
        {
            var result = await ExecutePost(true, false);

            Assert.AreEqual(HttpStatusCode.Unauthorized, (HttpStatusCode)result.StatusCode);
        }


        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public async Task PostThrowsError()
        {
            var result = await ExecutePost(false, false);

            Assert.IsNotNull(result);
        }

        [DataTestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public async Task PostSendVerificationEmail(bool expected)
        {
            var userManager = new Mock<IUserManager>();
            userManager.Setup(c => c.SendAccountVerificationEmail(It.IsAny<string>()))
                .Returns<string>(email =>
                Task.FromResult(expected));

            var controller = new UsersController(userManager.Object);

            var result = await controller.PostSendVerificationEmail(new EmailRequest { Email = "email" });

            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public async Task PostVerifyAccount()
        {
            var expected = new AuthToken();
            var userManager = new Mock<IUserManager>();
            userManager.Setup(c => c.VerifyAccount(It.IsAny<OneTimeTokenRequest>()))
                .Returns<OneTimeTokenRequest>(r =>
                Task.FromResult(expected));

            var controller = new UsersController(userManager.Object);

            var result = await controller.PostVerifyAccount(new OneTimeTokenRequest { Token = "token" });

            Assert.AreEqual(expected, result);
        }

        [DataTestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public async Task Delete(bool expected)
        {
            var userManager = new Mock<IUserManager>();
            userManager.Setup(c => c.DeleteUserAsync(It.IsAny<int>()))
                .Returns<int>(u =>
                Task.FromResult(expected));

            var controller = new UsersController(userManager.Object);

            var result = await controller.Delete(5);

            Assert.AreEqual(expected, result);
        }

        private async Task<JsonResult> ExecutePut(InsertUserResponse response)
        {
            var methodResponse = new GenericManagerResponse<AuthToken, InsertUserResponse>(response, new AuthToken());
            var userManager = new Mock<IUserManager>();
            userManager.Setup(c => c.InsertUserAsync(It.IsAny<User>()))
                .Returns<User>(u =>
                Task.FromResult(methodResponse));

            var controller = new UsersController(userManager.Object);

            var result = await controller.Put(new User());

            return result;
        }

        private async Task<JsonResult> ExecutePost(bool isAuthenticated, bool response)
        {
            const int userId = 55;
            var userManager = new Mock<IUserManager>();
            userManager.Setup(c => c.UpdateUserAsync(userId, It.IsAny<User>()))
                .Returns<int, User>((uid, u) =>
                Task.FromResult(response));

            var controller = new UsersController(userManager.Object);
            if (isAuthenticated)
            {
                controller.Token = new Models.TransferObjects.AuthToken
                {
                    Token = "token",
                    UserId = userId,
                    ValidUntil = DateTime.Now.AddDays(1),
                    IsVerified = true
                };
            }
            var result = await controller.Post(new User());

            return result;
        }
    }
}
