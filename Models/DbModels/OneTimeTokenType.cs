using System;
namespace Models.DbModels
{
    public enum OneTimeTokenType : byte
    {
        SSO = 1,
        AccountVerification = 2,
        PasswordChange = 3
    }
}
