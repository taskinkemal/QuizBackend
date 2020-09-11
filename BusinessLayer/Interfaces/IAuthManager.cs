using Common.Interfaces;
using Models.TransferObjects;

namespace BusinessLayer.Interfaces
{
    public interface IAuthManager : IDependency
    {
        System.Threading.Tasks.Task<AuthToken> VerifyAccessToken(string accessToken);

        System.Threading.Tasks.Task<AuthToken> GenerateTokenAsync(int userId, string deviceId);

        System.Threading.Tasks.Task<AuthToken> GenerateTokenAsync(TokenRequest request);

        System.Threading.Tasks.Task<AuthToken> UpsertUserAsync(User user);

        System.Threading.Tasks.Task<bool> DeleteUserAsync(int userId);

        System.Threading.Tasks.Task<bool> SendAccountVerificationEmail(string email);

        System.Threading.Tasks.Task<AuthToken> VerifyAccount(OneTimeTokenRequest request);
    }
}