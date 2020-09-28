using System.Collections.Generic;
using System.Threading.Tasks;
using Common.Interfaces;
using Models.DbModels;

namespace BusinessLayer.Interfaces
{
    /// <summary>
    /// 
    /// </summary>
    public interface IOptionManager : IDependency
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="quizId"></param>
        /// <returns></returns>
        Task<List<Option>> GetQuizOptions(int quizId);
    }
}
