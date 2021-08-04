using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BusinessLayer.Context;
using BusinessLayer.Interfaces;
using Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using Models.DbModels;
using Models.TransferObjects;

namespace BusinessLayer.Implementations
{
    /// <summary>
    /// 
    /// </summary>
    public class QuizManager : ManagerBase, IQuizManager
    {
        private readonly IQuizAttemptManager quizAttemptManager;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="quizAttemptManager"></param>
        /// <param name="logManager"></param>
        public QuizManager(QuizContext context, IQuizAttemptManager quizAttemptManager, ILogManager logManager) : base(context, logManager)
        {
            this.quizAttemptManager = quizAttemptManager;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<List<Quiz>> GetUserQuizList(int userId)
        {
            await CompleteExpiredQuizzes(userId);

            var attempts = Context.QuizAttempts
                .Where(a => a.UserId == userId)
                .ToList()
                .GroupBy(a => a.QuizId, (key, g) => g.OrderByDescending(p => p.Id).FirstOrDefault())
                .ToList();

            var quizes = (
                from qi in Context.QuizIdentities
                join q in Context.Quizes on qi.Id equals q.QuizIdentityId
                join ua in Context.QuizAssignments on qi.Id equals ua.QuizIdentityId
                join u in Context.Users on new { F1 = ua.Email, F2 = userId } equals new { F1 = u.Email, F2 = u.Id }
                where q.Status == QuizStatus.Current
                select q).ToList();


            foreach (var quiz in quizes)
            {
                quiz.LastAttempt = attempts.FirstOrDefault(a => a.QuizId == quiz.Id);
            }

            return await Task.FromResult(quizes);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<List<Quiz>> GetAdminQuizList(int userId)
        {
            var quizes = (
                from qi in Context.QuizIdentities
                join q in Context.Quizes on qi.Id equals q.QuizIdentityId
                where qi.OwnerId == userId
                orderby qi.Id, q.Id
                select q).ToList();

            return await Task.FromResult(quizes);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="quiz"></param>
        /// <returns></returns>
        public async Task<SaveQuizResult> InsertQuiz(int userId, Quiz quiz)
        {
            if (quiz == null)
            {
                return new SaveQuizResult
                {
                    Status = SaveQuizResultStatus.GeneralError
                };
            }

            if (quiz.Id != default || quiz.QuizIdentityId != default)
            {
                return new SaveQuizResult
                {
                    Status = SaveQuizResultStatus.NotAuthorized
                };
            }

            if (!IsValid(quiz))
            {
                return new SaveQuizResult
                {
                    Status = SaveQuizResultStatus.GeneralError
                };
            }

            var result = await InsertQuizInternalAsync(userId, quiz);

            return new SaveQuizResult
            {
                Status = SaveQuizResultStatus.Success,
                Result = result.QuizId
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="quizId"></param>
        /// <param name="quiz"></param>
        /// <returns></returns>
        public async Task<SaveQuizResult> UpdateQuiz(int userId, int quizId, Quiz quiz)
        {
            if (quiz == null)
            {
                return new SaveQuizResult
                {
                    Status = SaveQuizResultStatus.GeneralError
                };
            }

            if (quiz.Id != quizId)
            {
                return new SaveQuizResult
                {
                    Status = SaveQuizResultStatus.NotAuthorized
                };
            }

            var authorizationResult = await AuthorizeQuizUpdateRequest(userId, quiz);

            if (authorizationResult != SaveQuizResultStatus.Success)
            {
                return new SaveQuizResult
                {
                    Status = authorizationResult
                };
            }

            if (!IsValid(quiz))
            {
                return new SaveQuizResult
                {
                    Status = SaveQuizResultStatus.GeneralError
                };
            }

            Context.Quizes.Update(quiz);
            await Context.SaveChangesAsync();

            return new SaveQuizResult
            {
                Status = SaveQuizResultStatus.Success,
                Result = quizId
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="quizId"></param>
        /// <returns></returns>
        public async Task<bool> DeleteQuiz(int userId, int quizId)
        {
            var quiz = await Context.Quizes.FindAsync(quizId);

            if (quiz == null)
            {
                return false;
            }

            if (!await IsOwner(userId, quiz.QuizIdentityId))
            {
                return false;
            }

            //TODO: should we delete the quiz identity as well?
            return true;
        }

        internal async Task<SaveQuizResultStatus> AuthorizeQuizUpdateRequest(int userId, Quiz quiz)
        {
            if (quiz == null)
            {
                return SaveQuizResultStatus.GeneralError;
            }

            var quizDb = Context.Quizes.AsQueryable().AsNoTracking().FirstOrDefault(q => q.Id == quiz.Id);

            if (quizDb == null || quiz.QuizIdentityId != quizDb.QuizIdentityId)
            {
                return SaveQuizResultStatus.NotAuthorized;
            }

            if (quiz.Version != quizDb.Version || quiz.Status != quizDb.Status)
            {
                return SaveQuizResultStatus.GeneralError;
            }

            if (!await IsOwner(userId, quiz.QuizIdentityId))
            {
                return SaveQuizResultStatus.NotAuthorized;
            }

            return SaveQuizResultStatus.Success;
        }

        internal async Task<bool> IsOwner(int userId, int quizIdentityId)
        {
            return await base.IsQuizOwner(userId, quizIdentityId);
        }

        internal static bool IsValid(Quiz quiz)
        {
            if (quiz == null)
            {
                return false;
            }

            if (quiz.TimeConstraint && quiz.TimeLimitInSeconds <= 0)
            {
                return false;
            }

            if (quiz.PassScore.HasValue && (quiz.PassScore < 0 || quiz.PassScore > 100))
            {
                return false;
            }

            if (quiz.AvailableFrom.HasValue && quiz.AvailableTo.HasValue && quiz.AvailableFrom > quiz.AvailableTo)
            {
                return false;
            }

            return true;
        }

        private async Task CompleteExpiredQuizzes(int userId)
        {
            var expiredAttempts = (from qa in Context.QuizAttempts
                                   join q in Context.Quizes on qa.QuizId equals q.Id
                                   where qa.UserId == userId && q.AvailableTo != null
                                   && q.AvailableTo < DateTime.Now && qa.Status == QuizAttemptStatus.Incomplete
                                   select new { Attempt = qa, Quiz = q }
                                  ).ToList();

            foreach (var item in expiredAttempts)
            {
                await quizAttemptManager.FinishQuizAsync(item.Attempt, item.Quiz.PassScore, item.Attempt.TimeSpent);
            }
        }

        internal async Task<(int QuizIdentityId, int QuizId)> InsertQuizInternalAsync(int userId, Quiz quiz)
        {
            var quizIdentity = await Context.QuizIdentities.AddAsync(
                new QuizIdentity
                {
                    OwnerId = userId
                });

            await Context.SaveChangesAsync();

            quiz.QuizIdentityId = quizIdentity.Entity.Id;
            quiz.Version = 1;
            quiz.Status = QuizStatus.Current;

            var insertedQuiz = await Context.Quizes.AddAsync(quiz);

            await Context.SaveChangesAsync();

            return (QuizIdentityId: quiz.QuizIdentityId, QuizId: insertedQuiz.Entity.Id);
        }
    }
}
