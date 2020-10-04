using System;
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
