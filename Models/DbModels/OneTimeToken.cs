using System;

namespace Models.DbModels
{
    public class OneTimeToken
    {
        public int Id { get; set; }

        public string Email { get; set; }

        public byte TokenType { get; set; }

        public string Token { get; set; }

        public DateTime ValidUntil { get; set; }
    }
}
