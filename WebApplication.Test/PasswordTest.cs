using System;
using System.Threading.Tasks;
using BusinessLayer.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Models.TransferObjects;
using Moq;
using WebApplication.Controllers;

namespace WebApplication.Test
{
    [TestClass]
    public class PasswordTest
    {
        [TestMethod]
        public void GetCriteria()
        {
            var sut = new PasswordController(Mock.Of<IUserManager>());
            var result = sut.Get();

            Assert.AreEqual(6, result.MinimumLength);
            Assert.AreEqual(1, result.MinimumAlphabetic);
            Assert.AreEqual(1, result.MinimumNumeric);
        }

        [DataTestMethod]
        [DataRow(true, "kemal123")]
        [DataRow(false, "kemalkemal")]
        [DataRow(false, "123456")]
        [DataRow(false, "k123")]
        public void PostValidatePassword(bool expected, string password)
        {
            var sut = new PasswordController(Mock.Of<IUserManager>());
            var result = sut.PostValidatePassword(new PasswordChangeRequest
            {
                Password = password,
                DeviceId = "deviceId"
            });

            Assert.AreEqual(expected, result);
        }

        [DataTestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public async Task PostPasswordResetEmail(bool returnValue)
        {
            const string email = "myemail@email.com";
            var userManager = new Mock<IUserManager>();
            userManager.Setup(c => c.SendPasswordResetEmail(email))
                .Returns(Task.FromResult(returnValue));

            var sut = new PasswordController(userManager.Object);
            var result = await sut.PostPasswordResetEmail(new EmailRequest
            {
                Email = email
            });

            Assert.AreEqual(returnValue, result);
        }

        [DataTestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public async Task PostWithToken(bool returnValue)
        {
            const string password = "password123";
            const string token = "token";
            var userManager = new Mock<IUserManager>();
            userManager.Setup(c => c.UpdatePassword(token, password))
                .Returns(Task.FromResult(returnValue));

            var sut = new PasswordController(userManager.Object);
            var result = await sut.PostWithToken(new PasswordChangeRequestWithToken
            {
                Password = password,
                Token = token,
                DeviceId = "deviceId"
            });

            Assert.AreEqual(returnValue, result);
        }

        [DataTestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public async Task Post(bool returnValue)
        {
            const string password = "password123";
            const int userId = 123;
            var userManager = new Mock<IUserManager>();
            userManager.Setup(c => c.UpdatePassword(userId, password))
                .Returns(Task.FromResult(returnValue));

            var sut = new PasswordController(userManager.Object);

            sut.Token = new AuthToken
            {
                Token = "token",
                UserId = userId,
                ValidUntil = DateTime.Now.AddDays(1),
                IsVerified = true
            };

            var result = await sut.Post(new PasswordChangeRequest
            {
                Password = password,
                DeviceId = "deviceId"
            });

            Assert.AreEqual(returnValue, result);
        }

        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public async Task PostException()
        {
            const string password = "password123";
            const int userId = 123;
            var userManager = new Mock<IUserManager>();
            userManager.Setup(c => c.UpdatePassword(userId, password))
                .Returns(Task.FromResult(false));

            var sut = new PasswordController(userManager.Object);

            var result = await sut.Post(new PasswordChangeRequest
            {
                Password = password,
                DeviceId = "deviceId"
            });

            Assert.AreEqual(false, result);
        }
    }
}
