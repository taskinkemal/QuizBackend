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
            var quizIdentities = Context.QuizIdentities.ToList();

            var list = new List<Quiz>();

            foreach (var quizIdentity in quizIdentities)
            {
                if (quizIdentity.Id == 1)
                {
                    list.Add(new Quiz
                    {
                        Id = 1,
                        Title = "Sample Quiz",
                        Intro = "This is a sample quiz",
                        Version = 1,
                        TimeConstraint = true,
                        TimeLimitInSeconds = 300,
                        ShuffleQuestions = true,
                        ShuffleOptions = true,
                        PassScore = 60,
                        Repeatable = true,
                        QuestionIds = new List<int> { 3, 4, 5 }
                    });
                }
                else if (quizIdentity.Id == 2)
                {
                    list.Add(new Quiz
                    {
                        Id = 2,
                        Title = "Driving License Test",
                        Intro = "This is a sample quiz for the driving license.",
                        Version = 1,
                        TimeConstraint = true,
                        TimeLimitInSeconds = 600,
                        ShuffleQuestions = true,
                        ShuffleOptions = true,
                        PassScore = 90,
                        Repeatable = true,
                        QuestionIds = new List<int> { 6, 7, 8, 9 }
                    });
                }
            }

            return await Task.FromResult(list);
        }
    }
}
