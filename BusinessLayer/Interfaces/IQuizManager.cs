using System.Collections.Generic;
using System.Threading.Tasks;
using Common.Interfaces;
using Models.DbModels;

namespace BusinessLayer.Interfaces
{
    /// <summary>
    /// 
    /// </summary>
    public interface IQuizManager : IDependency
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<List<Quiz>> GetUserQuizList(int userId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<List<Quiz>> GetAdminQuizList(int userId);
    }
}
