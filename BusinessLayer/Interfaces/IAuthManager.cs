using Common.Interfaces;
using Models;

namespace BusinessLayer.Interfaces
{
    /// <summary>
    /// 
    /// </summary>
    public interface IAuthManager : IDependency
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="accessToken"></param>
        /// <returns></returns>
        System.Threading.Tasks.Task<UserToken> VerifyAccessToken(string accessToken);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="deviceId"></param>
        /// <returns></returns>
        System.Threading.Tasks.Task<UserToken> GenerateTokenAsync(int userId, string deviceId);
    }
}