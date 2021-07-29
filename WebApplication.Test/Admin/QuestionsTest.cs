using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using BusinessLayer.Interfaces;
using Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Models.DbModels;
using Models.TransferObjects;
using Moq;
using WebApplication.Controllers.Admin;

namespace WebApplication.Test.Admin
{
    [TestClass]
    public class QuestionsTest
    {
        [DataTestMethod]
        [DataRow(0, SaveQuizResultStatus.NotAuthorized, HttpStatusCode.Unauthorized)]
        [DataRow(0, SaveQuizResultStatus.GeneralError, HttpStatusCode.NotAcceptable)]
        [DataRow(5, SaveQuizResultStatus.Success, HttpStatusCode.OK)]
        public async Task PostUpdate(int saveResult, SaveQuizResultStatus status, HttpStatusCode expectedResponse)
        {
            const int userId = 56;
            const int quizId = 44;
            const int questionId = 154;
            var question = new Question
            {
                Id = questionId,
                Body = "Test",
                Level = 3,
                Type = QuestionType.MultiSelect
            };

            var response = new SaveQuizResult
            {
                Result = saveResult,
                Status = status
            };

            var questionManager = new Mock<IQuestionManager>();

            questionManager
                .Setup(c => c.UpdateQuestion(userId, quizId, questionId, question))
                .Returns(Task.FromResult(response));

            var sut = new QuestionsController(questionManager.Object);
            sut.Token = new AuthToken
            {
                Token = "token",
                UserId = userId,
                ValidUntil = DateTime.Now.AddDays(1),
                IsVerified = true
            };

            var result = await sut.PostUpdateQuestion(quizId, questionId, question);

            Assert.AreEqual(expectedResponse, (HttpStatusCode)result.StatusCode);
            Assert.IsTrue((HttpStatusCode)result.StatusCode != HttpStatusCode.OK || ((GenericWrapper<int>)result.Value).Value == saveResult);
        }

        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public async Task PostUpdateThrowsException()
        {
            const int userId = 56;
            const int quizId = 44;
            const int questionId = 154;
            var question = new Question
            {
                Id = questionId,
                Body = "Test",
                Level = 3,
                Type = QuestionType.MultiSelect
            };

            var response = new SaveQuizResult
            {
                Result = 5,
                Status = SaveQuizResultStatus.Success
            };

            var questionManager = new Mock<IQuestionManager>();

            questionManager
                .Setup(c => c.UpdateQuestion(userId, quizId, questionId, question))
                .Returns(Task.FromResult(response));

            var sut = new QuestionsController(questionManager.Object);

            var result = await sut.PostUpdateQuestion(quizId, questionId, question);

            Assert.AreEqual(HttpStatusCode.OK, (HttpStatusCode)result.StatusCode);
        }
    }
}
