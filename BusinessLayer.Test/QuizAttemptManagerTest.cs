﻿using System;
using System.Collections.Generic;
using System.Linq;
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
                var sut = new QuizAttemptManager(context, new QuestionManager(context, logManager), logManager);

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
        public async Task UpdateStatusTimeUp()
        {
            UpdateQuizAttemptResponse result;

            using (var context = new QuizContext(ManagerTestHelper.Options))
            {
                var logManager = Mock.Of<ILogManager>();
                var sut = new QuizAttemptManager(context, new QuestionManager(context, logManager), logManager);
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
                var sut = new QuizAttemptManager(context, new QuestionManager(context, logManager), logManager);
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
        public async Task UpdateStatusAvailableIntervalNotPassed()
        {
            UpdateQuizAttemptResponse result;

            using (var context = new QuizContext(ManagerTestHelper.Options))
            {
                var logManager = Mock.Of<ILogManager>();
                var sut = new QuizAttemptManager(context, new QuestionManager(context, logManager), logManager);
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

        [TestMethod]
        public async Task UpdateStatusStatusNotIncomplete()
        {
            UpdateQuizAttemptResponse result;

            using (var context = new QuizContext(ManagerTestHelper.Options))
            {
                var logManager = Mock.Of<ILogManager>();
                var sut = new QuizAttemptManager(context, new QuestionManager(context, logManager), logManager);
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
                var sut = new QuizAttemptManager(context, new QuestionManager(context, logManager), logManager);
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
                var sut = new QuizAttemptManager(context, new QuestionManager(context, logManager), logManager);
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

        [TestMethod]
        public async Task CreateAttemptQuizNotFound()
        {
            CreateAttemptResponse result;

            using (var context = new QuizContext(ManagerTestHelper.Options))
            {
                var logManager = Mock.Of<ILogManager>();
                var sut = new QuizAttemptManager(context, new QuestionManager(context, logManager), logManager);
                result = await sut.CreateAttempt(3, 5);
            }

            Assert.AreEqual(CreateAttemptResult.NotAuthorized, result.Result);
            Assert.IsNull(result.Attempt);
        }

        [TestMethod]
        public async Task CreateAttemptQuizNotAvailable()
        {
            CreateAttemptResponse result;

            var quiz = new Quiz
            {
                Title = "title",
                Intro = "intro",
                AvailableTo = DateTime.Now.AddDays(-1)
            };

            using (var context = new QuizContext(ManagerTestHelper.Options))
            {
                var logManager = Mock.Of<ILogManager>();
                var testData = await ManagerTestHelper.CreateAndAssignQuizAsync(context, quiz, true);
                var sut = new QuizAttemptManager(context, new QuestionManager(context, logManager), logManager);
                result = await sut.CreateAttempt(testData.UserId, testData.QuizId);
            }

            Assert.AreEqual(CreateAttemptResult.DateError, result.Result);
            Assert.IsNull(result.Attempt);
        }

        [TestMethod]
        public async Task CreateAttemptQuizAvailable()
        {
            CreateAttemptResponse result;

            var quiz = new Quiz
            {
                Title = "title",
                Intro = "intro",
                AvailableTo = DateTime.Now.AddDays(1)
            };

            using (var context = new QuizContext(ManagerTestHelper.Options))
            {
                var logManager = Mock.Of<ILogManager>();
                var testData = await ManagerTestHelper.CreateAndAssignQuizAsync(context, quiz, true);
                var sut = new QuizAttemptManager(context, new QuestionManager(context, logManager), logManager);
                result = await sut.CreateAttempt(testData.UserId, testData.QuizId);
            }

            Assert.AreEqual(CreateAttemptResult.Success, result.Result);
            Assert.IsNotNull(result.Attempt);
        }

        [TestMethod]
        public async Task CreateAttemptQuizNotAssigned()
        {
            CreateAttemptResponse result;

            var quiz = new Quiz
            {
                Title = "title",
                Intro = "intro"
            };

            using (var context = new QuizContext(ManagerTestHelper.Options))
            {
                var logManager = Mock.Of<ILogManager>();
                var testData = await ManagerTestHelper.CreateAndAssignQuizAsync(context, quiz, false);
                var sut = new QuizAttemptManager(context, new QuestionManager(context, logManager), logManager);
                result = await sut.CreateAttempt(testData.UserId, testData.QuizId);
            }

            Assert.AreEqual(CreateAttemptResult.NotAuthorized, result.Result);
            Assert.IsNull(result.Attempt);
        }

        [TestMethod]
        public async Task CreateAttemptQuizNotRepeatable()
        {
            CreateAttemptResponse result;
            QuizAttempt previousAttempt;

            var quiz = new Quiz
            {
                Title = "title",
                Intro = "intro",
                Repeatable = false
            };

            using (var context = new QuizContext(ManagerTestHelper.Options))
            {
                var logManager = Mock.Of<ILogManager>();
                var testData = await ManagerTestHelper.CreateAndAssignQuizAsync(context, quiz, true);
                var attempt = await context.QuizAttempts.AddAsync(
                    new QuizAttempt
                    {
                        QuizId = testData.QuizId,
                        UserId = testData.UserId,
                        StartDate = DateTime.Now,
                        Status = QuizAttemptStatus.Incomplete
                    });
                await context.SaveChangesAsync();

                var sut = new QuizAttemptManager(context, new QuestionManager(context, logManager), logManager);
                result = await sut.CreateAttempt(testData.UserId, testData.QuizId);

                previousAttempt = await context.QuizAttempts.FindAsync(attempt.Entity.Id);
            }

            Assert.AreEqual(CreateAttemptResult.NotRepeatable, result.Result);
            Assert.AreEqual(QuizAttemptStatus.Incomplete, previousAttempt.Status);
            Assert.IsNull(result.Attempt);
        }

        [TestMethod]
        public async Task CreateAttemptQuizRepeatable()
        {
            CreateAttemptResponse result;
            QuizAttempt previousAttempt;

            var quiz = new Quiz
            {
                Title = "title",
                Intro = "intro",
                Repeatable = true
            };

            using (var context = new QuizContext(ManagerTestHelper.Options))
            {
                var logManager = Mock.Of<ILogManager>();
                var testData = await ManagerTestHelper.CreateAndAssignQuizAsync(context, quiz, true);
                var attempt = await context.QuizAttempts.AddAsync(
                    new QuizAttempt
                    {
                        QuizId = testData.QuizId,
                        UserId = testData.UserId,
                        StartDate = DateTime.Now,
                        Status = QuizAttemptStatus.Incomplete,
                        TimeSpent = 10
                    });
                await context.SaveChangesAsync();

                var sut = new QuizAttemptManager(context, new QuestionManager(context, logManager), logManager);
                result = await sut.CreateAttempt(testData.UserId, testData.QuizId);

                previousAttempt = await context.QuizAttempts.FindAsync(attempt.Entity.Id);
            }

            Assert.AreEqual(CreateAttemptResult.Success, result.Result);
            Assert.AreEqual(QuizAttemptStatus.Completed, previousAttempt.Status);
            Assert.AreEqual(QuizAttemptStatus.Incomplete, result.Attempt.Status);
        }

        [TestMethod]
        public async Task FinishQuizNull()
        {
            var sut = new QuizAttemptManager(null, null, Mock.Of<ILogManager>());
            var result = await sut.FinishQuizAsync(new QuizAttempt
            {
                Status = QuizAttemptStatus.Completed
            }, null, 0);

            Assert.IsFalse(result);
        }


        [TestMethod]
        public async Task FinishQuiz()
        {
            CreateAttemptResponse result;

            using (var context = new QuizContext(ManagerTestHelper.Options))
            {
                var logManager = Mock.Of<ILogManager>();
                var quizData = await ManagerTestHelper.CreateQuizAsync(context, 3, 5);
                var quiz = await context.Quizes.FindAsync(quizData.QuizId);
                var userId = await ManagerTestHelper.AssignQuizAsync(context, quiz.QuizIdentityId);

                var sut = new QuizAttemptManager(context, new QuestionManager(context, logManager), logManager);
                result = await sut.CreateAttempt(userId, quiz.Id);
                await sut.FinishQuizAsync(result.Attempt, quiz.PassScore, 30);
            }

            Assert.AreEqual(CreateAttemptResult.Success, result.Result);
            Assert.AreEqual(QuizAttemptStatus.Completed, result.Attempt.Status);
            Assert.IsNotNull(result.Attempt.EndDate);
        }

        [DataTestMethod]
        [DataRow(24, 20, "100.00")]
        [DataRow(0, 0, "0.00")]
        [DataRow(17, 31, "54.84")]
        public void RoundScore(int userScore, int totalScore, string expected)
        {
            var actual = QuizAttemptManager.RoundScore(userScore, totalScore);
            Assert.AreEqual(Convert.ToDecimal(expected, new System.Globalization.CultureInfo("en-US")), actual);
        }

        [TestMethod]
        public void EvaluateQuizSuccess()
        {
            var options = new List<QuestionAnswer>();
            options.Add(new QuestionAnswer
            {
                QuestionId = 1,
                IsCorrect = false,
                IsMarked = false
            });
            options.Add(new QuestionAnswer
            {
                QuestionId = 1,
                IsCorrect = true,
                IsMarked = false
            });
            options.Add(new QuestionAnswer
            {
                QuestionId = 2,
                IsCorrect = false,
                IsMarked = false
            });
            options.Add(new QuestionAnswer
            {
                QuestionId = 2,
                IsCorrect = true,
                IsMarked = true
            });
            options.Add(new QuestionAnswer
            {
                QuestionId = 2,
                IsCorrect = true,
                IsMarked = true
            });
            options.Add(new QuestionAnswer
            {
                QuestionId = 2,
                IsCorrect = false,
                IsMarked = false
            });
            options.Add(new QuestionAnswer
            {
                QuestionId = 3,
                IsCorrect = false,
                IsMarked = false
            });
            options.Add(new QuestionAnswer
            {
                QuestionId = 3,
                IsCorrect = false,
                IsMarked = false
            });
            options.Add(new QuestionAnswer
            {
                QuestionId = 3,
                IsCorrect = true,
                IsMarked = true
            });
            options.Add(new QuestionAnswer
            {
                QuestionId = 4,
                IsCorrect = false,
                IsMarked = true
            });
            options.Add(new QuestionAnswer
            {
                QuestionId = 4,
                IsCorrect = true,
                IsMarked = false
            });
            options.Add(new QuestionAnswer
            {
                QuestionId = 5,
                IsCorrect = true,
                IsMarked = true
            });
            options.Add(new QuestionAnswer
            {
                QuestionId = 5,
                IsCorrect = true,
                IsMarked = true
            });
            options.Add(new QuestionAnswer
            {
                QuestionId = 5,
                IsCorrect = false,
                IsMarked = true
            });

            var questions = new List<Question>();
            questions.Add(new Question
            {
                Id = 1,
                Level = 4
            });
            questions.Add(new Question
            {
                Id = 2,
                Level = 2
            });
            questions.Add(new Question
            {
                Id = 3,
                Level = 5
            });
            questions.Add(new Question
            {
                Id = 4,
                Level = 2
            });
            questions.Add(new Question
            {
                Id = 5,
                Level = 5
            });

            var result = QuizAttemptManager.EvaluateQuiz(questions, options);

            Assert.AreEqual(2, result.CorrectCount);
            Assert.AreEqual(2, result.IncorrectCount);
            Assert.AreEqual(38.89M, result.Score);
        }

        [DataTestMethod]
        [DataRow(QuizAttemptStatus.Passed, 80, 70)]
        [DataRow(QuizAttemptStatus.Failed, 60, 65)]
        [DataRow(QuizAttemptStatus.Passed, 90, 90)]
        [DataRow(QuizAttemptStatus.Completed, 80, null)]
        public void EvaluateStatus(QuizAttemptStatus expected, int userScore, int? passScore)
        {
            var actual = QuizAttemptManager.EvaluateStatus(userScore, passScore);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public async Task InsertAnswer()
        {
            UpdateQuizAttemptStatusResult result;
            QuizAttempt attempt;
            List<int> optionIds;
            List<int> answers;
            List<int> answersOtherQuestion;

            using (var context = new QuizContext(ManagerTestHelper.Options))
            {
                var logManager = Mock.Of<ILogManager>();
                var quizData = await ManagerTestHelper.CreateQuizAsync(context, 3, 5);
                var quiz = await context.Quizes.FindAsync(quizData.QuizId);
                var userId = await ManagerTestHelper.AssignQuizAsync(context, quiz.QuizIdentityId);

                var sut = new QuizAttemptManager(context, new QuestionManager(context, logManager), logManager);
                var createAttemptResult = await sut.CreateAttempt(userId, quiz.Id);
                attempt = createAttemptResult.Attempt;
                optionIds = quizData.OptionIds.Skip(2).Take(2).ToList();

                await sut.InsertAnswerAsync(userId, attempt.Id,
                    new Models.TransferObjects.Answer
                    {
                        QuestionId = quizData.QuestionIds[0],
                        Options = quizData.OptionIds.Take(2),
                        TimeSpent = 40
                    });

                result = await sut.InsertAnswerAsync(userId, attempt.Id,
                    new Models.TransferObjects.Answer
                    {
                        QuestionId = quizData.QuestionIds[0],
                        Options = optionIds,
                        TimeSpent = 40
                    });

                answers = context.Answers
                    .Where(a => a.AttemptId == attempt.Id && a.QuestionId == quizData.QuestionIds[0])
                    .Select(a => a.OptionId)
                    .ToList();

                answersOtherQuestion = context.Answers
                    .Where(a => a.AttemptId == attempt.Id && a.QuestionId == quizData.QuestionIds[1])
                    .Select(a => a.OptionId)
                    .ToList();
            }

            Assert.AreEqual(UpdateQuizAttemptStatusResult.Success, result);
            Assert.AreEqual(0, optionIds.Except(answers).ToList().Count);
            Assert.AreEqual(0, answers.Except(optionIds).ToList().Count);
            Assert.AreEqual(0, answersOtherQuestion.Count);
            Assert.AreEqual(40, attempt.TimeSpent);
        }

        [TestMethod]
        public async Task InsertAnswerUnauthorized()
        {
            UpdateQuizAttemptStatusResult result;
            
            using (var context = new QuizContext(ManagerTestHelper.Options))
            {
                var logManager = Mock.Of<ILogManager>();
                var quizData = await ManagerTestHelper.CreateQuizAsync(context, 3, 5);
                var quiz = await context.Quizes.FindAsync(quizData.QuizId);
                var userId = await ManagerTestHelper.AssignQuizAsync(context, quiz.QuizIdentityId);

                var sut = new QuizAttemptManager(context, new QuestionManager(context, logManager), logManager);
                
                result = await sut.InsertAnswerAsync(userId, 8,
                    new Models.TransferObjects.Answer
                    {
                        QuestionId = quizData.QuestionIds[0],
                        Options = quizData.OptionIds.Take(2),
                        TimeSpent = 40
                    });
            }

            Assert.AreEqual(UpdateQuizAttemptStatusResult.NotAuthorized, result);
        }
    }
}
