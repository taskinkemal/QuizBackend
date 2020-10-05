using System;
using System.Net;
using System.Threading.Tasks;
using BusinessLayer.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Models.DbModels;
using Models.TransferObjects;
using Moq;
using WebApplication.Controllers;

namespace WebApplication.Test
{
    [TestClass]
    public class QuizAttemptsTest
    {
        [TestMethod]
        public async Task Put()
        {
            var attempt = new QuizAttempt();

            var result = await ExecutePut(2, 5, new CreateAttemptResponse
            {
                Attempt = attempt,
                Result = CreateAttemptResult.Success
            });

            var resultObject = (QuizAttempt)result.Value;

            Assert.AreSame(attempt, resultObject);
        }

        [DataTestMethod]
        [DataRow(HttpStatusCode.Unauthorized, CreateAttemptResult.NotAuthorized)]
        [DataRow(HttpStatusCode.Conflict, CreateAttemptResult.NotRepeatable)]
        [DataRow(HttpStatusCode.NotAcceptable, CreateAttemptResult.DateError)]
        public async Task PutError(HttpStatusCode expected, CreateAttemptResult result)
        {
            var attempt = new QuizAttempt();

            var actual = await ExecutePut(2, 5, new CreateAttemptResponse
            {
                Attempt = attempt,
                Result = result
            });

            Assert.AreEqual(expected, (HttpStatusCode)actual.StatusCode);
        }

        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public async Task PutThrowsError()
        {
            var quizAttemptsManager = new Mock<IQuizAttemptManager>();
            quizAttemptsManager.Setup(c => c.CreateAttempt(2, 5))
                .Returns<int, int>((u, q) =>
                Task.FromResult(new CreateAttemptResponse()));
            var controller = new QuizAttemptsController(quizAttemptsManager.Object);

            var result = await controller.Put(5);

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public async Task Post()
        {
            var attempt = new QuizAttempt();

            var actual = await ExecutePost(2, 5, new UpdateQuizAttemptStatus(),
                new UpdateQuizAttemptResponse
                {
                    Attempt = attempt,
                    Result = UpdateQuizAttemptStatusResult.Success
                });

            var resultObject = (QuizAttempt)actual.Value;

            Assert.AreSame(attempt, resultObject);
        }

        [DataTestMethod]
        [DataRow(HttpStatusCode.Unauthorized, UpdateQuizAttemptStatusResult.NotAuthorized)]
        [DataRow(HttpStatusCode.Conflict, UpdateQuizAttemptStatusResult.StatusError)]
        [DataRow(HttpStatusCode.ExpectationFailed, UpdateQuizAttemptStatusResult.TimeUp)]
        [DataRow(HttpStatusCode.NotAcceptable, UpdateQuizAttemptStatusResult.DateError)]
        public async Task PostError(HttpStatusCode expected, UpdateQuizAttemptStatusResult result)
        {
            var attempt = new QuizAttempt();

            var actual = await ExecutePost(2, 5, new UpdateQuizAttemptStatus(),
                new UpdateQuizAttemptResponse
            {
                Attempt = attempt,
                Result = result
            });

            Assert.AreEqual(expected, (HttpStatusCode)actual.StatusCode);
        }

        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public async Task PostThrowsError()
        {
            var quizAttemptsManager = new Mock<IQuizAttemptManager>();
            quizAttemptsManager.Setup(c => c.UpdateStatus(2, 5, It.IsAny<UpdateQuizAttemptStatus>()))
                .Returns<int, int, UpdateQuizAttemptStatus>((u, q, s) =>
                Task.FromResult(new UpdateQuizAttemptResponse()));
            var controller = new QuizAttemptsController(quizAttemptsManager.Object);

            var result = await controller.Post(5, new UpdateQuizAttemptStatus());

            Assert.IsNotNull(result);
        }

        private async Task<JsonResult> ExecutePost(int userId, int attemptId, UpdateQuizAttemptStatus data, UpdateQuizAttemptResponse response)
        {
            var quizAttemptsManager = new Mock<IQuizAttemptManager>();
            quizAttemptsManager.Setup(c => c.UpdateStatus(userId, attemptId, It.IsAny<UpdateQuizAttemptStatus>()))
                .Returns<int, int, UpdateQuizAttemptStatus>((u, q, s) =>
                Task.FromResult(response));
            var controller = new QuizAttemptsController(quizAttemptsManager.Object);
            controller.Token = new AuthToken
            {
                Token = "token",
                UserId = userId,
                ValidUntil = DateTime.Now.AddDays(1),
                IsVerified = true
            };
            var result = await controller.Post(attemptId, data);

            return result;
        }

        private async Task<JsonResult> ExecutePut(int userId, int quizId, CreateAttemptResponse response)
        {
            var quizAttemptsManager = new Mock<IQuizAttemptManager>();
            quizAttemptsManager.Setup(c => c.CreateAttempt(userId, quizId))
                .Returns<int, int>((u, q) =>
                Task.FromResult(response));
            var controller = new QuizAttemptsController(quizAttemptsManager.Object);
            controller.Token = new AuthToken
            {
                Token = "token",
                UserId = userId,
                ValidUntil = DateTime.Now.AddDays(1),
                IsVerified = true
            };
            var result = await controller.Put(quizId);

            return result;
        }
    }
}
