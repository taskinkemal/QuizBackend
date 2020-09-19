using System;
namespace Models.DbModels
{
    /// <summary>
    /// One-Time token types
    /// </summary>
    public enum OneTimeTokenType : byte
    {
        /// <summary>
        /// SSO Token
        /// </summary>
        SSO = 1,
        /// <summary>
        /// Account Verification
        /// </summary>
        AccountVerification = 2,
        /// <summary>
        /// Forgot Password
        /// </summary>
        ForgotPassword = 3
    }
}
