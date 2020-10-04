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
            const int quizId = 56;
            var questions = new List<Question>
            {
                new Question(),
                new Question()
            };

            var questionManager = new Mock<IQuestionManager>();

            questionManager.Setup(c => c.GetQuizQuestions(quizId))
                .Returns(Task.FromResult(questions));

            var sut = new QuestionsController(questionManager.Object);

            var result = await sut.Get(quizId);

            Assert.AreEqual(questions.Count, result.ToList().Count);
        }
    }
}
