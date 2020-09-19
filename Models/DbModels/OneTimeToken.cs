using System;

namespace Models.DbModels
{
    /// <summary>
    /// 
    /// </summary>
    public class OneTimeToken
    {
        /// <summary>
        /// 
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public byte TokenType { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DateTime ValidUntil { get; set; }
    }
}
