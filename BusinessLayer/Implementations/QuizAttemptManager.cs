using System;
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
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="logManager"></param>
        public QuizAttemptManager(QuizContext context, ILogManager logManager) : base(context, logManager)
        {
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

            var attempts = Context.QuizAttempts.Where(a => a.QuizId == quizId && a.UserId == userId);

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

                await Context.SaveChangesAsync();
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
            var attempt = await Context.QuizAttempts.FindAsync(attemptId);

            if (attempt == null || attempt.UserId != userId)
            {
                return new UpdateQuizAttemptResponse
                {
                    Result = UpdateQuizAttemptStatusResult.NotAuthorized
                };
            }

            var quiz = await Context.Quizes.FindAsync(attempt.QuizId);
            // no need to check for null, because otherwise the attempt cannot be there.

            var isAssigned = (from qas in Context.QuizAssignments
                              join u in Context.Users on qas.Email equals u.Email
                              where qas.QuizIdentityId == quiz.QuizIdentityId && u.Id == userId
                              select 0).Any();

            if (!isAssigned)
            {
                return new UpdateQuizAttemptResponse
                {
                    Result = UpdateQuizAttemptStatusResult.NotAuthorized
                };
            }

            if (quiz.TimeConstraint && status.TimeSpent >= quiz.TimeLimitInSeconds)
            {
                attempt = await FinishQuizAsync(attempt, quiz.PassScore, status.TimeSpent);

                return new UpdateQuizAttemptResponse
                {
                    Result = UpdateQuizAttemptStatusResult.TimeUp,
                    Attempt = attempt
                };
            }

            if (quiz.AvailableTo != null && quiz.AvailableTo < DateTime.Now)
            {
                attempt = await FinishQuizAsync(attempt, quiz.PassScore, status.TimeSpent);

                return new UpdateQuizAttemptResponse
                {
                    Result = UpdateQuizAttemptStatusResult.DateError,
                    Attempt = attempt
                };
            }

            if (attempt.Status != QuizAttemptStatus.Incomplete)
            {
                return new UpdateQuizAttemptResponse
                {
                    Result = UpdateQuizAttemptStatusResult.StatusError
                };
            }

            if (status.EndQuiz)
            {
                attempt = await FinishQuizAsync(attempt, quiz.PassScore, status.TimeSpent);

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

        internal async Task<QuizAttempt> FinishQuizAsync(QuizAttempt attempt, int? passScore, int timeSpent)
        {
            if (attempt.Status != QuizAttemptStatus.Incomplete)
            {
                return attempt;
            }

            var questions = (from qq in Context.QuizQuestions
                             join q in Context.Questions on qq.QuestionId equals q.Id
                             where qq.QuizId == attempt.QuizId
                             select q
                            ).ToList();

            var options = (from qo in Context.QuestionOptions
                           join qq in Context.QuizQuestions on qo.QuestionId equals qq.QuestionId
                           join q in Context.Questions on qo.QuestionId equals q.Level
                           join o in Context.Options on qo.OptionId equals o.Id
                           join a in Context.Answers on new { AttemptId = attempt.Id, QuestionId = q.Id, OptionId = q.Id } equals new { a.AttemptId, a.QuestionId, a.OptionId } into ad
                           from a in ad.DefaultIfEmpty()
                           where qq.QuizId == attempt.QuizId
                           select new { qo.QuestionId, q.Level, o.Id, o.IsCorrect, IsMarked = a != null ? (int?)a.OptionId : null }
                          ).ToList();

            var totalScore = questions.Sum(q => q.Level);
            var userScore = 0;
            var correctCount = 0;
            var incorrectCount = 0;

            foreach (var question in questions)
            {
                var qOptions = options.Where(o => o.QuestionId == question.Id);

                var cntCorrect = qOptions.Count(o => o.IsCorrect);
                var cntUserCorrect = qOptions.Count(o => o.IsCorrect && o.IsMarked.HasValue);
                var cntUserIncorrect = qOptions.Count(o => !o.IsCorrect && o.IsMarked.HasValue);

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

            var score = totalScore == 0 ? 0 : Convert.ToDecimal(Math.Round(userScore * 100.0 / totalScore, 2));
            if (score > 100)
            {
                score = 100;
            }

            var status = passScore.HasValue ? score >= passScore ? QuizAttemptStatus.Passed : QuizAttemptStatus.Failed : QuizAttemptStatus.Completed;

            attempt.Score = score;
            attempt.Status = status;
            attempt.EndDate = DateTime.Now;
            attempt.TimeSpent = timeSpent;

            Context.Update(attempt);

            return await Task.FromResult(attempt);
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
    }
}
