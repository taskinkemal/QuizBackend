using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BusinessLayer.Context;
using BusinessLayer.Interfaces;
using Common.Interfaces;
using Models.DbModels;

namespace BusinessLayer.Implementations
{
    /// <summary>
    /// 
    /// </summary>
    public class QuizManager : ManagerBase, IQuizManager
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="logManager"></param>
        public QuizManager(QuizContext context, ILogManager logManager) : base(context, logManager)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<List<Quiz>> GetUserQuizList(int userId)
        {
            //TODO: If the availableTo is passed, or time is up, fix the quizattemptstatus.

            var attempts = Context.QuizAttempts
                .Where(a => a.UserId == userId)
                .GroupBy(a => a.QuizId, (key, g) => g.OrderByDescending(p => p.Id).FirstOrDefault())
                .ToList();

            var quizes = (
                from qi in Context.QuizIdentities
                join q in Context.Quizes on qi.Id equals q.QuizId
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

        internal async Task<int> InsertQuizInternalAsync(int userId, Quiz quiz)
        {
            var quizIdentity = await Context.QuizIdentities.AddAsync(
                new QuizIdentity
                {
                    OwnerId = userId
                });

            await Context.SaveChangesAsync();

            quiz.QuizId = quizIdentity.Entity.Id;
            quiz.Version = 1;
            quiz.Status = QuizStatus.Current;

            var insertedQuiz = await Context.Quizes.AddAsync(quiz);

            await Context.SaveChangesAsync();

            return insertedQuiz.Entity.Id;
        }
    }
}
