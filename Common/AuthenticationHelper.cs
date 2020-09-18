using System;
using System.Security.Cryptography;
using System.Text;

namespace Common
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class AuthenticationHelper
    {
        private const string Alphabet = "abcdefghijklmnoqprsqtuwxyz0123456789.";

        private static Random random;

        static AuthenticationHelper()
        {
            random = new Random(DateTime.Now.Millisecond);
        }

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
            var sb = new StringBuilder();
            for (var i = 0; i < length; i++)
                sb.Append(Alphabet[random.Next(Alphabet.Length)]);

            return sb.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="arr1"></param>
        /// <param name="arr2"></param>
        /// <returns></returns>
        public static bool CompareByteArrays(byte[] arr1, byte[] arr2)
        {
            if (arr1 == null || arr2 == null || arr1.Length != arr2.Length)
            {
                return false;
            }

            var isSame = true;

            for (var i = 0; i < arr1.Length; i++)
            {
                isSame = arr1[i] == arr2[i];

                if (!isSame)
                {
                    break;
                }
            }

            return isSame;
        }
    }
}
