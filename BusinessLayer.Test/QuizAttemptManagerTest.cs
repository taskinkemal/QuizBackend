using System;
using System.Threading.Tasks;
using BusinessLayer.Context;
using BusinessLayer.Implementations;
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
        public async Task UpdateStatusNull()
        {
            UpdateQuizAttemptResponse result;

            using (var context = new QuizContext(ManagerTestHelper.Options))
            {
                var logManager = Mock.Of<ILogManager>();
                var sut = new QuizAttemptManager(context, logManager);

                result = await sut.UpdateStatus(2, 0, new UpdateQuizAttemptStatus
                {
                    EndQuiz = false,
                    TimeSpent = 10
                });
            }

            Assert.AreEqual(UpdateQuizAttemptStatusResult.NotAuthorized, result.Result);
            Assert.IsNull(result.Attempt);
        }

        [TestMethod]
        public async Task UpdateStatusUserNotAssigned()
        {
            UpdateQuizAttemptResponse result;

            using (var context = new QuizContext(ManagerTestHelper.Options))
            {
                var logManager = Mock.Of<ILogManager>();
                var sut = new QuizAttemptManager(context, logManager);
                var testData = await ManagerTestHelper.CreateAndAssignQuizAsync(context, false);
                result = await sut.UpdateStatus(testData.UserId, 5, new UpdateQuizAttemptStatus
                {
                    TimeSpent = 10,
                    EndQuiz = false
                });
            }

            Assert.AreEqual(UpdateQuizAttemptStatusResult.NotAuthorized, result.Result);
            Assert.IsNull(result.Attempt);
        }

        [TestMethod]
        public async Task UpdateStatusTimeUp()
        {
            UpdateQuizAttemptResponse result;

            using (var context = new QuizContext(ManagerTestHelper.Options))
            {
                var logManager = Mock.Of<ILogManager>();
                var sut = new QuizAttemptManager(context, logManager);
                var quiz = new Quiz
                {
                    Title = "title",
                    Intro = "intro",
                    TimeConstraint = true,
                    TimeLimitInSeconds = 40
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

                result = await sut.UpdateStatus(testData.UserId, attempt.Entity.Id, new UpdateQuizAttemptStatus
                {
                    TimeSpent = 50,
                    EndQuiz = false
                });
            }

            Assert.AreEqual(UpdateQuizAttemptStatusResult.TimeUp, result.Result);
            Assert.AreNotEqual(QuizAttemptStatus.Incomplete, result.Attempt.Status);
        }

        [TestMethod]
        public async Task UpdateStatusAvailableIntervalPassed()
        {
            UpdateQuizAttemptResponse result;

            using (var context = new QuizContext(ManagerTestHelper.Options))
            {
                var logManager = Mock.Of<ILogManager>();
                var sut = new QuizAttemptManager(context, logManager);
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
                        StartDate = DateTime.Now
                    });
                await context.SaveChangesAsync();

                result = await sut.UpdateStatus(testData.UserId, attempt.Entity.Id, new UpdateQuizAttemptStatus
                {
                    TimeSpent = 30,
                    EndQuiz = false
                });
            }

            Assert.AreEqual(UpdateQuizAttemptStatusResult.DateError, result.Result);
            Assert.AreNotEqual(QuizAttemptStatus.Incomplete, result.Attempt.Status);
        }

        [TestMethod]
        public async Task UpdateStatusStatusNotIncomplete()
        {
            UpdateQuizAttemptResponse result;

            using (var context = new QuizContext(ManagerTestHelper.Options))
            {
                var logManager = Mock.Of<ILogManager>();
                var sut = new QuizAttemptManager(context, logManager);
                var quiz = new Quiz
                {
                    Title = "title",
                    Intro = "intro",
                    TimeConstraint = true,
                    TimeLimitInSeconds = 40
                };

                var testData = await ManagerTestHelper.CreateAndAssignQuizAsync(context, quiz, true);
                var attempt = await context.QuizAttempts.AddAsync(
                    new QuizAttempt
                    {
                        QuizId = testData.QuizId,
                        UserId = testData.UserId,
                        Status = QuizAttemptStatus.Completed,
                        StartDate = DateTime.Now
                    });
                await context.SaveChangesAsync();

                result = await sut.UpdateStatus(testData.UserId, attempt.Entity.Id, new UpdateQuizAttemptStatus
                {
                    TimeSpent = 30,
                    EndQuiz = false
                });
            }

            Assert.AreEqual(UpdateQuizAttemptStatusResult.StatusError, result.Result);
            Assert.IsNull(result.Attempt);
        }

        [TestMethod]
        public async Task UpdateStatusFinishQuiz()
        {
            UpdateQuizAttemptResponse result;

            using (var context = new QuizContext(ManagerTestHelper.Options))
            {
                var logManager = Mock.Of<ILogManager>();
                var sut = new QuizAttemptManager(context, logManager);
                var quiz = new Quiz
                {
                    Title = "title",
                    Intro = "intro",
                    TimeConstraint = true,
                    TimeLimitInSeconds = 40
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

                result = await sut.UpdateStatus(testData.UserId, attempt.Entity.Id, new UpdateQuizAttemptStatus
                {
                    TimeSpent = 30,
                    EndQuiz = true
                });
            }

            Assert.AreEqual(UpdateQuizAttemptStatusResult.Success, result.Result);
            Assert.AreNotEqual(QuizAttemptStatus.Incomplete, result.Attempt.Status);
            Assert.AreEqual(30, result.Attempt.TimeSpent);
        }

        [TestMethod]
        public async Task UpdateStatusSuccess()
        {
            UpdateQuizAttemptResponse result;

            using (var context = new QuizContext(ManagerTestHelper.Options))
            {
                var logManager = Mock.Of<ILogManager>();
                var sut = new QuizAttemptManager(context, logManager);
                var quiz = new Quiz
                {
                    Title = "title",
                    Intro = "intro",
                    TimeConstraint = true,
                    TimeLimitInSeconds = 40
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

                result = await sut.UpdateStatus(testData.UserId, attempt.Entity.Id, new UpdateQuizAttemptStatus
                {
                    TimeSpent = 30,
                    EndQuiz = false
                });
            }

            Assert.AreEqual(UpdateQuizAttemptStatusResult.Success, result.Result);
            Assert.AreEqual(QuizAttemptStatus.Incomplete, result.Attempt.Status);
            Assert.AreEqual(30, result.Attempt.TimeSpent);
        }
    }
}
