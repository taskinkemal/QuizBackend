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
        /// <param name="userId"></param>
        /// <param name="quizId"></param>
        /// <returns></returns>
        public async Task<List<Question>> GetQuizQuestions(int userId, int quizId)
        {
            var quizDb = await Context.Quizes.AsQueryable().AsNoTracking().FirstOrDefaultAsync(q => q.Id == quizId);

            if (quizDb == null)
            {
                return null;
            }

            var isOwner = await base.IsQuizOwner(userId, quizDb.QuizIdentityId);

            var attemptDb = await Context.QuizAttempts.AsQueryable().AsNoTracking()
                .FirstOrDefaultAsync(q => q.QuizId == quizId && q.UserId == userId);

            if (!isOwner && attemptDb == null)
            {
                return null;
            }

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
                 join qq in Context.QuizQuestions on o.QuestionId equals qq.QuestionId
                 where qq.QuizId == quizId
                 select o
                ).ToList();

            foreach (var question in questions)
            {
                question.OptionIds = questionOptions
                    .Where(o => o.QuestionId == question.Id)
                    .OrderBy(o => o.OptionOrder)
                    .Select(o => o.Id)
                    .ToList();
            }

            return await Task.FromResult(questions);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="quizId"></param>
        /// <param name="questionId"></param>
        /// <returns></returns>
        public async Task<List<Option>> GetQuestionOptions(int userId, int quizId, int questionId)
        {
            var quizDb = await Context.Quizes.AsQueryable().AsNoTracking().FirstOrDefaultAsync(q => q.Id == quizId);

            if (quizDb == null)
            {
                return null;
            }

            var isOwner = await base.IsQuizOwner(userId, quizDb.QuizIdentityId);

            if (!isOwner)
            {
                return null;
            }

            var question = await Context.QuizQuestions.AsQueryable().AsNoTracking().FirstOrDefaultAsync(q => q.QuizId == quizId);

            if (question == null)
            {
                return null;
            }

            var questionOptions =
                (from o in Context.Options
                 join qq in Context.QuizQuestions on o.QuestionId equals qq.QuestionId
                 where qq.QuizId == quizId
                 orderby o.OptionOrder
                 select o
                ).ToList();

            return questionOptions;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="quizId"></param>
        /// <param name="questionId"></param>
        /// <param name="question"></param>
        /// <returns></returns>
        public async Task<SaveQuizResult> UpdateQuestion(int userId, int quizId, int questionId, Question question)
        {
            if (question == null)
            {
                return new SaveQuizResult
                {
                    Status = SaveQuizResultStatus.GeneralError
                };
            }

            if (question.Id != questionId)
            {
                return new SaveQuizResult
                {
                    Status = SaveQuizResultStatus.NotAuthorized
                };
            }

            var authorizationResult = AuthorizeQuestionUpdateRequest(userId, quizId, questionId);

            if (authorizationResult != SaveQuizResultStatus.Success)
            {
                return new SaveQuizResult
                {
                    Status = authorizationResult
                };
            }

            if (!IsValid(question))
            {
                return new SaveQuizResult
                {
                    Status = SaveQuizResultStatus.GeneralError
                };
            }

            Context.Questions.Update(question);
            await Context.SaveChangesAsync();

            return new SaveQuizResult
            {
                Status = SaveQuizResultStatus.Success,
                Result = questionId
            };
        }

        internal SaveQuizResultStatus AuthorizeQuestionUpdateRequest(int userId, int quizId, int questionId)
        {
            var quizDb = Context.Quizes.AsQueryable().AsNoTracking().FirstOrDefault(q => q.Id == quizId);
            var quizQuestionDb = Context.QuizQuestions.AsQueryable().AsNoTracking().FirstOrDefault(q => q.QuizId == quizId && q.QuestionId == questionId);

            if (quizDb == null)
            {
                return SaveQuizResultStatus.GeneralError;
            }

            if (quizQuestionDb == null)
            {
                return SaveQuizResultStatus.GeneralError;
            }

            var quizIdentityDb = Context.QuizIdentities.AsQueryable().AsNoTracking().FirstOrDefault(q => q.Id == quizDb.QuizIdentityId);

            if (quizIdentityDb == null)
            {
                return SaveQuizResultStatus.GeneralError;
            }

            if (quizIdentityDb.OwnerId != userId)
            {
                return SaveQuizResultStatus.NotAuthorized;
            }

            return SaveQuizResultStatus.Success;
        }

        internal static bool IsValid(Question question)
        {
            if (question == null)
            {
                return false;
            }

            if (string.IsNullOrWhiteSpace(question.Body))
            {
                return false;
            }

            if (question.Level < 1 || question.Level > 5)
            {
                return false;
            }

            return true;
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
