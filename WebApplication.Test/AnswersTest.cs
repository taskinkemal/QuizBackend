using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using BusinessLayer.Interfaces;
using Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using WebApplication.Controllers;

namespace WebApplication.Test
{
    [TestClass]
    public class AnswersTest
    {
        [TestMethod]
        public async Task Post()
        {
            var result = await ExecutePost(Models.TransferObjects.UpdateQuizAttemptStatusResult.Success);

            var resultObject = (Common.GenericWrapper<Models.TransferObjects.UpdateQuizAttemptStatusResult>)result.Value;

            Assert.AreEqual(Models.TransferObjects.UpdateQuizAttemptStatusResult.Success, resultObject.Value);
        }

        [TestMethod]
        public async Task PostUnauthorized()
        {
            var result = await ExecutePost(Models.TransferObjects.UpdateQuizAttemptStatusResult.NotAuthorized);
            Assert.AreEqual(HttpStatusCode.Unauthorized, (HttpStatusCode)result.StatusCode);
        }

        [TestMethod]
        public async Task PostStatusError()
        {
            var result = await ExecutePost(Models.TransferObjects.UpdateQuizAttemptStatusResult.StatusError);
            Assert.AreEqual(HttpStatusCode.Conflict, (HttpStatusCode)result.StatusCode);
        }

        [TestMethod]
        public async Task PostTimeUp()
        {
            var result = await ExecutePost(Models.TransferObjects.UpdateQuizAttemptStatusResult.TimeUp);
            Assert.AreEqual(HttpStatusCode.ExpectationFailed, (HttpStatusCode)result.StatusCode);
        }

        [TestMethod]
        public async Task PostDateError()
        {
            var result = await ExecutePost(Models.TransferObjects.UpdateQuizAttemptStatusResult.DateError);
            Assert.AreEqual(HttpStatusCode.NotAcceptable, (HttpStatusCode)result.StatusCode);
        }

        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public async Task PostThrowsError()
        {
            var quizAttemptsManager = new Mock<IQuizAttemptManager>();
            var controller = new AnswersController(quizAttemptsManager.Object);

            var result = await controller.Post(5,
                new Models.TransferObjects.Answer
                {
                    TimeSpent = 10,
                    QuestionId = 3,
                    Options = new List<int> { 8, 9 }
                });

            Assert.IsNotNull(result);
        }

        private async Task<JsonResult> ExecutePost(Models.TransferObjects.UpdateQuizAttemptStatusResult status)
        {
            const int attemptId = 5;
            const int userId = 2;
            
            var quizAttemptsManager = new Mock<IQuizAttemptManager>();
            quizAttemptsManager.Setup(c => c.InsertAnswerAsync(userId, attemptId, It.IsAny<Models.TransferObjects.Answer>()))
                .Returns<int, int, Models.TransferObjects.Answer>((u, a, answer) =>
                Task.FromResult(status));
            var controller = new AnswersController(quizAttemptsManager.Object);
            controller.Token = new Models.TransferObjects.AuthToken
            {
                Token = "token",
                UserId = userId,
                ValidUntil = DateTime.Now.AddDays(1),
                IsVerified = true
            };
            var result = await controller.Post(5,
                new Models.TransferObjects.Answer
                {
                    TimeSpent = 10,
                    QuestionId = 3,
                    Options = new List<int> { 8, 9 }
                });

            return result;
        }
    }
}
