using System;
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
        /// <param name="attempt"></param>
        /// <returns></returns>
        public async Task<UpdateQuizAttemptResponse> UpdateAttempt(int userId, QuizAttempt attempt)
        {
            if (attempt == null || userId != attempt.UserId)
            {
                return UpdateQuizAttemptResponse.NotAuthorized;
            }

            var quiz = await Context.Quizes.FindAsync(attempt.QuizId);

            if (quiz == null)
            {
                return UpdateQuizAttemptResponse.NotAuthorized;
            }

            if (quiz.AvailableTo != null && quiz.AvailableTo < DateTime.Now)
            {
                return UpdateQuizAttemptResponse.DateError;
            }

            //TODO: is the quiz assigned to the user?

            var currentAttempt = await Context.QuizAttempts.FindAsync(attempt.Id);

            if (currentAttempt != null && currentAttempt.Status != QuizAttemptStatus.Incomplete)
            {
                return UpdateQuizAttemptResponse.StatusError;
            }

            //TODO: if quiz has no pass score, status must be checked.

            //TODO: if the time is up (DateTime.Now - StartDate > TimeLimit)

            if (currentAttempt != null)
            {
                //TODO:
            }
            else
            {
                //TODO:
            }

            return UpdateQuizAttemptResponse.Success;
        }

        internal async Task<QuizAttempt> InsertAttemptInternalAsync(int userId, int quizId)
        {
            var attempt = await Context.QuizAttempts.AddAsync(new QuizAttempt
            {
                UserId = userId,
                QuizId = quizId,
                Correct = 0,
                Incorrect = 0,
                StartDate = DateTime.Now,
                Status = QuizAttemptStatus.Incomplete,
                TimeSpent = 0
            });

            await Context.SaveChangesAsync();

            return attempt.Entity;
        }
    }
}
