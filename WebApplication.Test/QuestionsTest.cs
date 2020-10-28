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
    public class QuestionsTest
    {
        [TestMethod]
        public async Task Get()
        {
            const int userId = 32;
            const int quizId = 56;
            var questions = new List<Question>
            {
                new Question(),
                new Question()
            };

            var questionManager = new Mock<IQuestionManager>();

            questionManager.Setup(c => c.GetQuizQuestions(userId, quizId))
                .Returns(Task.FromResult(questions));

            var sut = new QuestionsController(questionManager.Object);
            sut.Token = new Models.TransferObjects.AuthToken
            {
                Token = "token",
                UserId = userId,
                ValidUntil = DateTime.Now.AddDays(1),
                IsVerified = true
            };

            var result = await sut.Get(quizId);

            Assert.AreEqual(questions.Count, result.ToList().Count);
        }

        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public async Task GetThrowsException()
        {
            const int userId = 32;
            const int quizId = 56;
            var questions = new List<Question>
            {
                new Question(),
                new Question()
            };

            var questionManager = new Mock<IQuestionManager>();

            questionManager.Setup(c => c.GetQuizQuestions(userId, quizId))
                .Returns(Task.FromResult(questions));

            var sut = new QuestionsController(questionManager.Object);

            var result = await sut.Get(quizId);

            Assert.AreEqual(questions.Count, result.ToList().Count);
        }
    }
}
