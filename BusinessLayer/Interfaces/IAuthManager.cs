using Common.Interfaces;
using Models.TransferObjects;

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
        System.Threading.Tasks.Task<AuthToken> VerifyAccessToken(string accessToken);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        System.Threading.Tasks.Task<AuthToken> GenerateTokenAsync(TokenRequest request);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="deviceId"></param>
        /// <returns></returns>
        System.Threading.Tasks.Task<AuthToken> GenerateTokenAsync(int userId, string deviceId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="accessToken"></param>
        /// <returns></returns>
        System.Threading.Tasks.Task<Models.DbModels.UserToken> GetAccessToken(string accessToken);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="accessToken"></param>
        /// <returns></returns>
        System.Threading.Tasks.Task<bool> DeleteAccessToken(string accessToken);
    }
}