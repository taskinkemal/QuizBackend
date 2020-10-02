using System.Threading.Tasks;
using Common.Interfaces;
using Models.DbModels;
using Models.TransferObjects;

namespace BusinessLayer.Interfaces
{
    /// <summary>
    /// 
    /// </summary>
    public interface IQuizAttemptManager : IDependency
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="quizId"></param>
        /// <returns></returns>
        Task<CreateAttemptResponse> CreateAttempt(int userId, int quizId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="attemptId"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        Task<UpdateQuizAttemptResponse> UpdateStatus(int userId, int attemptId, UpdateQuizAttemptStatus status);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="attempt"></param>
        /// <param name="passScore"></param>
        /// <param name="timeSpent"></param>
        /// <returns></returns>
        Task<bool> FinishQuizAsync(QuizAttempt attempt, int? passScore, int timeSpent);
    }
}
