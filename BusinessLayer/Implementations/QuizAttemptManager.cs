using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BusinessLayer.Context;
using BusinessLayer.Interfaces;
using Common.Interfaces;
using Models.DbModels;
using Models.TransferObjects;

namespace BusinessLayer.Implementations
{
    /// <summary>
    /// 
    /// </summary>
    public class QuizAttemptManager : ManagerBase, IQuizAttemptManager
    {
        private readonly IQuestionManager questionManager;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="questionManager"></param>
        /// <param name="logManager"></param>
        public QuizAttemptManager(QuizContext context, IQuestionManager questionManager, ILogManager logManager) : base(context, logManager)
        {
            this.questionManager = questionManager;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="quizId"></param>
        /// <returns></returns>
        public async Task<CreateAttemptResponse> CreateAttempt(int userId, int quizId)
        {
            var quiz = await Context.Quizes.FindAsync(quizId);

            if (quiz == null)
            {
                return new CreateAttemptResponse
                {
                    Result = CreateAttemptResult.NotAuthorized
                };
            }

            if (quiz.AvailableTo != null && quiz.AvailableTo < DateTime.Now)
            {
                return new CreateAttemptResponse
                {
                    Result = CreateAttemptResult.DateError
                };
            }

            var isAssigned = (from qas in Context.QuizAssignments
                              join u in Context.Users on qas.Email equals u.Email
                              where qas.QuizIdentityId == quiz.QuizIdentityId && u.Id == userId
                              select 0).Any();

            if (!isAssigned)
            {
                return new CreateAttemptResponse
                {
                    Result = CreateAttemptResult.NotAuthorized
                };
            }

            var attempts = Context.QuizAttempts.Where(a => a.QuizId == quizId && a.UserId == userId).ToList();

            if (attempts.Any())
            {
                if (!quiz.Repeatable)
                {
                    return new CreateAttemptResponse
                    {
                        Result = CreateAttemptResult.NotRepeatable
                    };
                }

                foreach (var a in attempts)
                {
                    await FinishQuizAsync(a, quiz.PassScore, a.TimeSpent);
                }
            }

            var newAttempt = await InsertAttemptInternalAsync(userId, quizId);

            return new CreateAttemptResponse
            {
                Attempt = newAttempt,
                Result = CreateAttemptResult.Success
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="attemptId"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public async Task<UpdateQuizAttemptResponse> UpdateStatus(int userId, int attemptId, UpdateQuizAttemptStatus status)
        {
            var verifyResponse = await VerifyRequest(userId, attemptId, status.TimeSpent);
            var attempt = verifyResponse.attempt;
            var quiz = verifyResponse.quiz;

            if (verifyResponse.response != null)
            {
                return verifyResponse.response;
            }

            if (status.EndQuiz)
            {
                await FinishQuizAsync(attempt, quiz.PassScore, status.TimeSpent);

                return new UpdateQuizAttemptResponse
                {
                    Result = UpdateQuizAttemptStatusResult.Success,
                    Attempt = attempt
                };
            }

            attempt.TimeSpent = status.TimeSpent;

            Context.QuizAttempts.Update(attempt);

            return new UpdateQuizAttemptResponse
            {
                Result = UpdateQuizAttemptStatusResult.Success,
                Attempt = attempt
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="attempt"></param>
        /// <param name="passScore"></param>
        /// <param name="timeSpent"></param>
        /// <returns></returns>
        public async Task<bool> FinishQuizAsync(QuizAttempt attempt, int? passScore, int timeSpent)
        {
            if (attempt.Status != QuizAttemptStatus.Incomplete)
            {
                return false;
            }

            var questions = (await questionManager.GetQuizQuestions(attempt.QuizId)).ToList();

            var options = (from qo in Context.QuestionOptions
                           join qq in Context.QuizQuestions on qo.QuestionId equals qq.QuestionId
                           join q in Context.Questions on qo.QuestionId equals q.Level
                           join o in Context.Options on qo.OptionId equals o.Id
                           join a in Context.Answers on new { AttemptId = attempt.Id, QuestionId = q.Id, OptionId = q.Id } equals new { a.AttemptId, a.QuestionId, a.OptionId } into ad
                           from a in ad.DefaultIfEmpty()
                           where qq.QuizId == attempt.QuizId
                           select new QuestionAnswer { QuestionId = qo.QuestionId, IsCorrect = o.IsCorrect, IsMarked = a != null }
                          ).ToList();

            var evaluationResult = EvaluateQuiz(questions, options);

            attempt.Score = evaluationResult.Score;
            attempt.Correct = evaluationResult.CorrectCount;
            attempt.Incorrect = evaluationResult.IncorrectCount;
            attempt.Status = EvaluateStatus(evaluationResult.Score, passScore);
            attempt.EndDate = DateTime.Now;
            attempt.TimeSpent = timeSpent;

            Context.QuizAttempts.Update(attempt);

            await Context.SaveChangesAsync();

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="attemptId"></param>
        /// <param name="answer"></param>
        /// <returns></returns>
        public async Task<UpdateQuizAttemptStatusResult> InsertAnswerAsync(int userId, int attemptId, Models.TransferObjects.Answer answer)
        {
            var verifyResponse = await VerifyRequest(userId, attemptId, answer.TimeSpent);
            var attempt = verifyResponse.attempt;
            
            if (verifyResponse.response != null)
            {
                return verifyResponse.response.Result;
            }

            Context.Answers.RemoveRange(Context.Answers.Where(a => a.AttemptId == attemptId && a.QuestionId == answer.QuestionId));

            await Context.SaveChangesAsync();

            foreach (var optionId in answer.Options)
            {
                await Context.Answers.AddAsync(new Models.DbModels.Answer
                {
                    AttemptId = attemptId,
                    QuestionId = answer.QuestionId,
                    OptionId = optionId
                });
            }

            attempt.TimeSpent = answer.TimeSpent;

            await Context.SaveChangesAsync();

            return UpdateQuizAttemptStatusResult.Success;
        }

        internal static (int CorrectCount, int IncorrectCount, decimal Score) EvaluateQuiz(List<Question> questions, List<QuestionAnswer> options)
        {
            var totalScore = questions.Sum(q => q.Level);
            var userScore = 0;
            var correctCount = 0;
            var incorrectCount = 0;

            foreach (var question in questions)
            {
                var qOptions = options.Where(o => o.QuestionId == question.Id).ToList();

                var cntCorrect = qOptions.Count(o => o.IsCorrect);
                var cntUserCorrect = qOptions.Count(o => o.IsCorrect && o.IsMarked);
                var cntUserIncorrect = qOptions.Count(o => !o.IsCorrect && o.IsMarked);

                if (cntCorrect == cntUserCorrect && cntUserIncorrect == 0)
                {
                    userScore += question.Level;
                    correctCount++;
                }
                else if (cntUserCorrect > 0 || cntUserIncorrect > 0)
                {
                    incorrectCount++;
                }
            }

            var score = RoundScore(userScore, totalScore);

            return (correctCount, incorrectCount, score);
        }

        internal static decimal RoundScore(int rawScore, int totalScore)
        {
            var score = totalScore == 0 ? 0 : Convert.ToDecimal(Math.Round(rawScore * 100.0 / totalScore, 2));
            if (score > 100)
            {
                score = 100;
            }

            return score;
        }

        internal static QuizAttemptStatus EvaluateStatus(decimal score, int? passScore)
        {
            if (!passScore.HasValue)
            {
                return QuizAttemptStatus.Completed;
            }

            return score >= passScore ? QuizAttemptStatus.Passed : QuizAttemptStatus.Failed;
        }

        internal async Task<QuizAttempt> InsertAttemptInternalAsync(int userId, int quizId)
        {
            var attempt = await Context.QuizAttempts.AddAsync(new QuizAttempt
            {
                UserId = userId,
                QuizId = quizId,
                StartDate = DateTime.Now,
                Status = QuizAttemptStatus.Incomplete,
                TimeSpent = 0
            });

            await Context.SaveChangesAsync();

            return attempt.Entity;
        }

        private async Task<(QuizAttempt attempt, Quiz quiz, UpdateQuizAttemptResponse response)>
            VerifyRequest(int userId, int attemptId, int timeSpent)
        {
            var attempt = await Context.QuizAttempts.FindAsync(attemptId);

            if (attempt == null || attempt.UserId != userId)
            {
                var response = new UpdateQuizAttemptResponse
                {
                    Result = UpdateQuizAttemptStatusResult.NotAuthorized
                };

                return (attempt, null, response);
            }

            var quiz = await Context.Quizes.FindAsync(attempt.QuizId);
            // no need to check for null, because otherwise the attempt cannot be there.

            if (quiz.TimeConstraint && timeSpent >= quiz.TimeLimitInSeconds)
            {
                await FinishQuizAsync(attempt, quiz.PassScore, timeSpent);

                var response = new UpdateQuizAttemptResponse
                {
                    Result = UpdateQuizAttemptStatusResult.TimeUp,
                    Attempt = attempt
                };

                return (attempt, quiz, response);
            }

            if (quiz.AvailableTo != null && quiz.AvailableTo < DateTime.Now)
            {
                await FinishQuizAsync(attempt, quiz.PassScore, timeSpent);

                var response = new UpdateQuizAttemptResponse
                {
                    Result = UpdateQuizAttemptStatusResult.DateError,
                    Attempt = attempt
                };

                return (attempt, quiz, response);
            }

            if (attempt.Status != QuizAttemptStatus.Incomplete)
            {
                var response = new UpdateQuizAttemptResponse
                {
                    Result = UpdateQuizAttemptStatusResult.StatusError
                };

                return (attempt, quiz, response);
            }

            return (attempt, quiz, null);
        }
    }

    internal class QuestionAnswer
    {
        public int QuestionId { get; set; }
        public bool IsCorrect { get; set; }
        public bool IsMarked { get; set; }
    }
}
