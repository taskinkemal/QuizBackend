using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BusinessLayer.Context;
using BusinessLayer.Implementations;
using BusinessLayer.Interfaces;
using Common.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Models.DbModels;
using Models.TransferObjects;
using Moq;

namespace BusinessLayer.Test
{
    [TestClass]
    public class QuizAttemptManagerTest
    {
        [TestMethod]
        public async Task UpdateAttemptNull()
        {
            UpdateQuizAttemptResponse result;

            using (var context = new QuizContext(ManagerTestHelper.Options))
            {
                var logManager = Mock.Of<ILogManager>();
                var sut = new QuizAttemptManager(context, logManager);

                result = await sut.UpdateAttempt(2, null);
            }

            Assert.AreEqual(UpdateQuizAttemptResponse.NotAuthorized, result);
        }

        [TestMethod]
        public async Task UpdateAttemptUserNotMatching()
        {
            UpdateQuizAttemptResponse result;
            const int userId = 2;

            using (var context = new QuizContext(ManagerTestHelper.Options))
            {
                var logManager = Mock.Of<ILogManager>();
                var sut = new QuizAttemptManager(context, logManager);

                result = await sut.UpdateAttempt(userId, new QuizAttempt
                {
                    UserId = userId + 1
                });
            }

            Assert.AreEqual(UpdateQuizAttemptResponse.NotAuthorized, result);
        }

        [TestMethod]
        public async Task UpdateAttemptQuizNotFound()
        {
            UpdateQuizAttemptResponse result;
            const int userId = 2;

            using (var context = new QuizContext(ManagerTestHelper.Options))
            {
                var logManager = Mock.Of<ILogManager>();
                var sut = new QuizAttemptManager(context, logManager);

                result = await sut.UpdateAttempt(userId, new QuizAttempt
                {
                    UserId = userId,
                    QuizId = 7
                });
            }

            Assert.AreEqual(UpdateQuizAttemptResponse.NotAuthorized, result);
        }

        [TestMethod]
        public async Task UpdateAttemptAvailableIntervalPassed()
        {
            UpdateQuizAttemptResponse result;
            const int userId = 2;

            using (var context = new QuizContext(ManagerTestHelper.Options))
            {
                var logManager = Mock.Of<ILogManager>();
                var sut = new QuizAttemptManager(context, logManager);
                var quizManager = new QuizManager(context, logManager);
                var userManager = ManagerTestHelper.GetUserManager(context, Mock.Of<IAuthManager>(), logManager);

                var ownerId = await userManager.InsertUserInternalAsync(ManagerTestHelper.CreateUserTo(0), true);
                var quizId = await quizManager.InsertQuizInternalAsync(ownerId, new Quiz
                {
                    Title = "Quiz Title",
                    Intro = "Quiz Intro",
                    AvailableTo = DateTime.Now.AddDays(-1)
                });

                await context.SaveChangesAsync();

                result = await sut.UpdateAttempt(userId, new QuizAttempt
                {
                    UserId = userId,
                    QuizId = quizId
                });
            }

            Assert.AreEqual(UpdateQuizAttemptResponse.DateError, result);
        }

        [TestMethod]
        public async Task UpdateAttemptStatusNotIncomplete()
        {
            UpdateQuizAttemptResponse result;
            const int userId = 2;

            using (var context = new QuizContext(ManagerTestHelper.Options))
            {
                var logManager = Mock.Of<ILogManager>();
                var sut = new QuizAttemptManager(context, logManager);
                var quizManager = new QuizManager(context, logManager);
                var userManager = ManagerTestHelper.GetUserManager(context, Mock.Of<IAuthManager>(), logManager);

                var ownerId = await userManager.InsertUserInternalAsync(ManagerTestHelper.CreateUserTo(0), true);
                var quizId = await quizManager.InsertQuizInternalAsync(ownerId, new Quiz
                {
                    Title = "Quiz Title",
                    Intro = "Quiz Intro",
                    AvailableTo = DateTime.Now.AddDays(1)
                });

                var attempt = await context.QuizAttempts.AddAsync(new QuizAttempt
                {
                    UserId = userId,
                    QuizId = quizId,
                    Correct = 0,
                    Incorrect = 0,
                    StartDate = DateTime.Now,
                    Status = QuizAttemptStatus.Completed,
                    TimeSpent = 0
                });

                var attemptEntity = attempt.Entity;

                await context.SaveChangesAsync();

                attemptEntity.TimeSpent = 10;

                result = await sut.UpdateAttempt(userId, attemptEntity);
            }

            Assert.AreEqual(UpdateQuizAttemptResponse.StatusError, result);
        }
    }
}
