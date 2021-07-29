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
    public class QuizzesTest
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

            quizManager.Setup(c => c.GetAdminQuizList(userId))
                .Returns(Task.FromResult(quizzes));

            var sut = new QuizzesController(quizManager.Object);
            sut.Token = new AuthToken
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

            quizManager.Setup(c => c.GetAdminQuizList(userId))
                .Returns(Task.FromResult(quizzes));

            var sut = new QuizzesController(quizManager.Object);

            var result = await sut.Get();

            Assert.AreEqual(quizzes.Count, result.ToList().Count);
        }

        [DataTestMethod]
        [DataRow(0, SaveQuizResultStatus.NotAuthorized, HttpStatusCode.Unauthorized)]
        [DataRow(0, SaveQuizResultStatus.GeneralError, HttpStatusCode.NotAcceptable)]
        [DataRow(5, SaveQuizResultStatus.Success, HttpStatusCode.OK)]
        public async Task PutInsert(int saveResult, SaveQuizResultStatus status, HttpStatusCode expectedResponse)
        {
            const int userId = 56;
            var quiz = new Quiz
            {
                Title = "Test",
                Intro = "Intro"
            };

            var response = new SaveQuizResult
            {
                Result = saveResult,
                Status = status
            };

            var quizManager = new Mock<IQuizManager>();

            quizManager
                .Setup(c => c.InsertQuiz(userId, quiz))
                .Returns(Task.FromResult(response));

            var sut = new QuizzesController(quizManager.Object);
            sut.Token = new AuthToken
            {
                Token = "token",
                UserId = userId,
                ValidUntil = DateTime.Now.AddDays(1),
                IsVerified = true
            };

            var result = await sut.PutInsertQuiz(quiz);

            Assert.AreEqual(expectedResponse, (HttpStatusCode)result.StatusCode);
            Assert.IsTrue((HttpStatusCode)result.StatusCode != HttpStatusCode.OK || ((GenericWrapper<int>)result.Value).Value == saveResult);
        }

        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public async Task PutInsertThrowsException()
        {
            const int userId = 56;
            var response = new SaveQuizResult
            {
                Result = 5,
                Status = SaveQuizResultStatus.Success
            };

            var quizManager = new Mock<IQuizManager>();

            quizManager.Setup(c => c.InsertQuiz(userId, It.IsAny<Quiz>()))
                .Returns(Task.FromResult(response));

            var sut = new QuizzesController(quizManager.Object);

            var result = await sut.PutInsertQuiz(new Quiz());

            Assert.AreEqual(HttpStatusCode.OK, (HttpStatusCode)result.StatusCode);
        }

        [DataTestMethod]
        [DataRow(0, SaveQuizResultStatus.NotAuthorized, HttpStatusCode.Unauthorized)]
        [DataRow(0, SaveQuizResultStatus.GeneralError, HttpStatusCode.NotAcceptable)]
        [DataRow(5, SaveQuizResultStatus.Success, HttpStatusCode.OK)]
        public async Task PostUpdate(int saveResult, SaveQuizResultStatus status, HttpStatusCode expectedResponse)
        {
            const int userId = 56;
            const int quizId = 154;
            var quiz = new Quiz
            {
                Id = quizId,
                Title = "Test",
                Intro = "Intro"
            };

            var response = new SaveQuizResult
            {
                Result = saveResult,
                Status = status
            };

            var quizManager = new Mock<IQuizManager>();

            quizManager
                .Setup(c => c.UpdateQuiz(userId, quizId, quiz))
                .Returns(Task.FromResult(response));

            var sut = new QuizzesController(quizManager.Object);
            sut.Token = new AuthToken
            {
                Token = "token",
                UserId = userId,
                ValidUntil = DateTime.Now.AddDays(1),
                IsVerified = true
            };

            var result = await sut.PostUpdateQuiz(quizId, quiz);

            Assert.AreEqual(expectedResponse, (HttpStatusCode)result.StatusCode);
            Assert.IsTrue((HttpStatusCode)result.StatusCode != HttpStatusCode.OK || ((GenericWrapper<int>)result.Value).Value == saveResult);
        }

        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public async Task PostUpdateThrowsException()
        {
            const int userId = 56;
            const int quizId = 125;
            var response = new SaveQuizResult
            {
                Result = 5,
                Status = SaveQuizResultStatus.Success
            };

            var quizManager = new Mock<IQuizManager>();

            quizManager.Setup(c => c.UpdateQuiz(userId, quizId, It.IsAny<Quiz>()))
                .Returns(Task.FromResult(response));

            var sut = new QuizzesController(quizManager.Object);

            var result = await sut.PostUpdateQuiz(quizId, new Quiz { Id = quizId });

            Assert.AreEqual(HttpStatusCode.OK, (HttpStatusCode)result.StatusCode);
        }

        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public async Task DeleteThrowsException()
        {
            const int userId = 56;
            const int quizId = 56;

            var quizManager = new Mock<IQuizManager>();

            quizManager.Setup(c => c.DeleteQuiz(userId, quizId))
                .Returns(Task.FromResult(true));

            var sut = new QuizzesController(quizManager.Object);

            var result = await sut.Delete(quizId);

            Assert.AreEqual(HttpStatusCode.OK, (HttpStatusCode)result.StatusCode);
        }

        [DataTestMethod]
        [DataRow(HttpStatusCode.OK, true)]
        [DataRow(HttpStatusCode.Unauthorized, false)]
        public async Task DeleteReturnsOk(HttpStatusCode expected, bool response)
        {
            const int userId = 56;
            const int quizId = 56;

            var quizManager = new Mock<IQuizManager>();

            quizManager.Setup(c => c.DeleteQuiz(userId, quizId))
                .Returns(Task.FromResult(response));

            var sut = new QuizzesController(quizManager.Object);
            sut.Token = new AuthToken
            {
                Token = "token",
                UserId = userId,
                ValidUntil = DateTime.Now.AddDays(1),
                IsVerified = true
            };

            var result = await sut.Delete(quizId);

            Assert.AreEqual(expected, (HttpStatusCode)result.StatusCode);
        }
    }
}
