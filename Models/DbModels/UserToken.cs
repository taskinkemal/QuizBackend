using System;

namespace Models.DbModels
{
    /// <summary>
    /// Authentication token
    /// </summary>
    public class UserToken
    {
        /// <summary>
        /// Authenticated user id.
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// Token string for authentication.
        /// </summary>
        public string Token { get; set; }

        public string DeviceId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DateTime ValidUntil { get; set; }
    }
}