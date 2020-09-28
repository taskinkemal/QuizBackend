using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using BusinessLayer.Context;
using BusinessLayer.Interfaces;
using Common.Interfaces;
using Models.DbModels;

namespace BusinessLayer.Implementations
{
    /// <summary>
    /// 
    /// </summary>
    public class OptionManager : ManagerBase, IOptionManager
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="logManager"></param>
        public OptionManager(QuizContext context, ILogManager logManager) : base(context, logManager)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="quizId"></param>
        /// <returns></returns>
        public async Task<List<Option>> GetQuizOptions(int quizId)
        {
            var options =
                (from o in Context.Options
                 join qo in Context.QuestionOptions on o.Id equals qo.OptionId
                 join qq in Context.QuizQuestions on qo.QuestionId equals qq.QuestionId
                 where qq.QuizId == quizId
                 select o
                ).ToList();

            return await Task.FromResult(options);
        }
    }
}
