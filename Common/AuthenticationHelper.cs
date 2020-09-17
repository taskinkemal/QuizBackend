using System;
using System.Security.Cryptography;
using System.Text;

namespace Common
{
    /// <summary>
    /// 
    /// </summary>
    public static class AuthenticationHelper
    {
        private const string Alphabet = "abcdefghijklmnoqprsqtuwxyz0123456789.";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        public static byte[] EncryptPassword(string password)
        {
            byte[] passwordHash;

            using (var shaM = new SHA512Managed())
            {
                passwordHash = shaM.ComputeHash(Encoding.UTF8.GetBytes(password));
            }

            return passwordHash;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static string GenerateToken(int userId)
        {
            return GenerateRandomString(160) + "_" + userId;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string GenerateRandomString(int length)
        {
            var rand = new Random(DateTime.Now.Millisecond);
            var sb = new StringBuilder();
            for (var i = 0; i < length; i++)
                sb.Append(Alphabet[rand.Next(Alphabet.Length)]);

            return sb.ToString();
        }
    }
}
