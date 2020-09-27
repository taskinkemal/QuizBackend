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
        /// <param name="attempt"></param>
        /// <returns></returns>
        Task<UpdateQuizAttemptResponse> UpdateAttempt(int userId, QuizAttempt attempt);
    }
}
