using System;

namespace Models.TransferObjects
{
    /// <summary>
    /// Authentication token
    /// </summary>
    public class AuthToken
    {
        /// <summary>
        /// Token string for authentication.
        /// </summary>
        public string Token { get; set; }

        public bool IsVerified { get; set; }

        public DateTime ValidUntil { get; set; }
    }
}