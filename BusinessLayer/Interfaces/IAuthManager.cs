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
        /// <param name="user"></param>
        /// <returns></returns>
        System.Threading.Tasks.Task<AuthToken> UpsertUserAsync(User user);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        System.Threading.Tasks.Task<bool> DeleteUserAsync(int userId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        System.Threading.Tasks.Task<bool> SendAccountVerificationEmail(string email);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        System.Threading.Tasks.Task<AuthToken> VerifyAccount(OneTimeTokenRequest request);
    }
}