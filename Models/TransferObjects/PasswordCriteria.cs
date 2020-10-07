using System.Linq;

namespace Models.TransferObjects
{
    /// <summary>
    /// Password Criteria
    /// </summary>
    public class PasswordCriteria
    {
        /// <summary>
        /// Minimum length required.
        /// </summary>
        public int MinimumLength { get; } = 6;

        /// <summary>
        /// Minimum alphabetic characters required.
        /// </summary>
        public int MinimumAlphabetic { get; } = 1;

        /// <summary>
        /// Minimum numeric characters required.
        /// </summary>
        public int MinimumNumeric { get; } = 1;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        public static bool IsValid(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
            {
                return false;
            }

            var criteria = new PasswordCriteria();

            if (password.Length < criteria.MinimumLength)
            {
                return false;
            }

            if (password.Count(char.IsDigit) < criteria.MinimumNumeric)
            {
                return false;
            }


            if (password.Count(char.IsLetter) < criteria.MinimumAlphabetic)
            {
                return false;
            }

            return true;
        }
    }
}
