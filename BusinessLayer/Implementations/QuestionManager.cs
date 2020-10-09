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
    public class QuestionManager : ManagerBase, IQuestionManager
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="logManager"></param>
        public QuestionManager(QuizContext context, ILogManager logManager) : base(context, logManager)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="quizId"></param>
        /// <returns></returns>
        public async Task<List<Question>> GetQuizQuestions(int quizId)
        {
            var questions =
                (from q in Context.Quizes
                 join qq in Context.QuizQuestions on q.Id equals qq.QuizId
                 join qs in Context.Questions on qq.QuestionId equals qs.Id
                 where q.Id == quizId
                 orderby qq.QuestionOrder
                 select qs
                ).ToList();

            var questionOptions =
                (from o in Context.Options
                 join qo in Context.QuestionOptions on o.Id equals qo.OptionId
                 join qq in Context.QuizQuestions on qo.QuestionId equals qq.QuestionId
                 where qq.QuizId == quizId
                 select qo
                ).ToList();

            foreach (var question in questions)
            {
                question.OptionIds = questionOptions
                    .Where(o => o.QuestionId == question.Id)
                    .OrderBy(o => o.OptionOrder)
                    .Select(o => o.OptionId)
                    .ToList();
            }

            return await Task.FromResult(questions);
        }

        internal async Task<int> InsertQuestionInternalAsync(Question question)
        {
            var insertedQuestion = await Context.Questions.AddAsync(question);

            await Context.SaveChangesAsync();

            return insertedQuestion.Entity.Id;
        }

        internal async Task<QuizQuestion> AssignQuestionInternalAsync(int quizId, int questionId)
        {
            var list = Context.QuizQuestions
                .Where(q => q.QuizId == quizId)
                .Select(q => (int) q.QuestionOrder)
                .ToList();

            var questionOrder = list.Any() ? list.Max() : 0;

            var insertedAssignment = await Context.QuizQuestions.AddAsync(new QuizQuestion
            {
                QuizId = quizId,
                QuestionId = questionId,
                QuestionOrder = (byte)(questionOrder + 1)
            });

            await Context.SaveChangesAsync();

            return insertedAssignment.Entity;
        }
    }
}
