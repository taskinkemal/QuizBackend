using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BusinessLayer.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Models.DbModels;
using Moq;
using WebApplication.Controllers;

namespace WebApplication.Test
{
    [TestClass]
    public class QuizTest
    {
        [TestMethod]
        public async Task Get()
        {
            const int userId = 56;
            var quizzes = new List<Quiz>
            {
                new Quiz(),
                new Quiz()
            };

            var quizManager = new Mock<IQuizManager>();

            quizManager.Setup(c => c.GetUserQuizList(userId))
                .Returns(Task.FromResult(quizzes));

            var sut = new QuizController(quizManager.Object);
            sut.Token = new Models.TransferObjects.AuthToken
            {
                Token = "token",
                UserId = userId,
                ValidUntil = DateTime.Now.AddDays(1),
                IsVerified = true
            };

            var result = await sut.Get();

            Assert.AreEqual(quizzes.Count, result.ToList().Count);
        }

        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public async Task GetThrowsException()
        {
            const int userId = 56;
            var quizzes = new List<Quiz>
            {
                new Quiz(),
                new Quiz()
            };

            var quizManager = new Mock<IQuizManager>();

            quizManager.Setup(c => c.GetUserQuizList(userId))
                .Returns(Task.FromResult(quizzes));

            var sut = new QuizController(quizManager.Object);

            var result = await sut.Get();

            Assert.AreEqual(quizzes.Count, result.ToList().Count);
        }
    }
}
