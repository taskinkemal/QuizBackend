using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BusinessLayer.Context;
using BusinessLayer.Implementations;
using BusinessLayer.Interfaces;
using Common.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Models.DbModels;
using Moq;

namespace BusinessLayer.Test
{
    [TestClass]
    public class QuizManagerTest
    {
        [TestMethod]
        public async Task GetUserQuizList()
        {
            List<Quiz> result;

            using (var context = new QuizContext(ManagerTestHelper.Options))
            {
                var logManager = Mock.Of<ILogManager>();
                var sut = new QuizManager(context, new QuizAttemptManager(context, Mock.Of<IQuestionManager>(), logManager), logManager);
                var quiz = new Quiz
                {
                    Title = "title",
                    Intro = "intro",
                    TimeConstraint = true,
                    TimeLimitInSeconds = 40,
                    AvailableTo = DateTime.Now.AddDays(1)
                };

                var testData = await ManagerTestHelper.CreateAndAssignQuizAsync(context, quiz, true);
                var attempt = await context.QuizAttempts.AddAsync(
                    new QuizAttempt
                    {
                        QuizId = testData.QuizId,
                        UserId = testData.UserId,
                        Status = QuizAttemptStatus.Incomplete,
                        StartDate = DateTime.Now
                    });
                await context.SaveChangesAsync();

                result = await sut.GetUserQuizList(testData.UserId);
            }

            Assert.IsNotNull(result[0].LastAttempt);
        }

        [TestMethod]
        public async Task GetUserQuizListFinish()
        {
            List<Quiz> result;

            using (var context = new QuizContext(ManagerTestHelper.Options))
            {
                var logManager = Mock.Of<ILogManager>();
                var sut = new QuizManager(context, new QuizAttemptManager(context, new QuestionManager(context, logManager), logManager), logManager);
                var quiz = new Quiz
                {
                    Title = "title",
                    Intro = "intro",
                    TimeConstraint = true,
                    TimeLimitInSeconds = 40,
                    AvailableTo = DateTime.Now.AddDays(-1)
                };

                var testData = await ManagerTestHelper.CreateAndAssignQuizAsync(context, quiz, true);
                var attempt = await context.QuizAttempts.AddAsync(
                    new QuizAttempt
                    {
                        QuizId = testData.QuizId,
                        UserId = testData.UserId,
                        Status = QuizAttemptStatus.Incomplete,
                        StartDate = DateTime.Now.AddDays(-2)
                    });
                await context.SaveChangesAsync();

                result = await sut.GetUserQuizList(testData.UserId);
            }

            Assert.IsNotNull(result[0].LastAttempt);
            Assert.AreEqual(QuizAttemptStatus.Completed, result[0].LastAttempt.Status);
        }

        [TestMethod]
        public async Task GetAdminQuizList()
        {
            List<Quiz> result;

            using (var context = new QuizContext(ManagerTestHelper.Options))
            {
                var logManager = Mock.Of<ILogManager>();
                var userManager = ManagerTestHelper.GetUserManager(context, Mock.Of<IAuthManager>(), logManager);
                var sut = new QuizManager(context, new QuizAttemptManager(context, Mock.Of<IQuestionManager>(), logManager), logManager);
                var quiz = new Quiz
                {
                    Title = "title",
                    Intro = "intro",
                    TimeConstraint = true,
                    TimeLimitInSeconds = 40,
                    AvailableTo = DateTime.Now.AddDays(1)
                };

                var userId = await userManager.InsertUserInternalAsync(ManagerTestHelper.CreateUserTo(0), true);
                await sut.InsertQuizInternalAsync(userId, ManagerTestHelper.CreateQuiz(0));
                await sut.InsertQuizInternalAsync(userId, ManagerTestHelper.CreateQuiz(1));

                await context.SaveChangesAsync();

                result = await sut.GetAdminQuizList(userId);
            }

            Assert.AreEqual(2, result.Count);
        }
    }
}
