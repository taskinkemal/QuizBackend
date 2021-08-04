using System.Collections.Generic;
using System.Threading.Tasks;
using Common.Interfaces;
using Models.DbModels;
using Models.TransferObjects;

namespace BusinessLayer.Interfaces
{
    /// <summary>
    /// 
    /// </summary>
    public interface IQuestionManager : IDependency
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="quizId"></param>
        /// <returns></returns>
        Task<List<Question>> GetQuizQuestions(int userId, int quizId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="quizId"></param>
        /// <param name="questionId"></param>
        /// <returns></returns>
        Task<List<Option>> GetQuestionOptions(int userId, int quizId, int questionId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="quizId"></param>
        /// <param name="questionId"></param>
        /// <param name="question"></param>
        /// <returns></returns>
        Task<SaveQuizResult> UpdateQuestion(int userId, int quizId, int questionId, Question question);
    }
}
