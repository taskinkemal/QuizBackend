using Common.Interfaces;
using Models.TransferObjects;

namespace BusinessLayer.Interfaces
{
    /// <summary>
    /// 
    /// </summary>
    public interface IUserManager : IDependency
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        System.Threading.Tasks.Task<Models.DbModels.User> GetUserAsync(int userId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        System.Threading.Tasks.Task<bool> SendPasswordResetEmail(string email);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="oneTimeToken"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        System.Threading.Tasks.Task<bool> UpdatePassword(string oneTimeToken, string password);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        System.Threading.Tasks.Task<bool> UpdatePassword(int userId, string password);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        System.Threading.Tasks.Task<bool> DeleteUserAsync(int userId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        System.Threading.Tasks.Task<GenericManagerResponse<AuthToken, InsertUserResponse>> InsertUserAsync(User user);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        System.Threading.Tasks.Task<bool> UpdateUserAsync(int userId, User user);

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
