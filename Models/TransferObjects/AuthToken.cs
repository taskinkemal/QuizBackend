using System;
using System.Runtime.Serialization;

namespace Models.TransferObjects
{
    /// <summary>
    /// Authentication token
    /// </summary>
    [DataContract]
    public class AuthToken
    {
        /// <summary>
        /// Token string for authentication.
        /// </summary>
        [DataMember]
        public string Token { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool IsVerified { get; set; }

        /// <summary>
        /// The date until this token is valid.
        /// </summary>
        [DataMember]
        public DateTime ValidUntil { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int UserId { get; set; }
    }
}