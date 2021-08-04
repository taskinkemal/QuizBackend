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
                 join qq in Context.QuizQuestions on o.QuestionId equals qq.QuestionId
                 where qq.QuizId == quizId
                 select o
                ).ToList();

            return await Task.FromResult(options);
        }

        internal async Task<int> InsertOptionInternalAsync(Option option)
        {
            var insertedOption = await Context.Options.AddAsync(option);

            await Context.SaveChangesAsync();

            return insertedOption.Entity.Id;
        }
    }
}
