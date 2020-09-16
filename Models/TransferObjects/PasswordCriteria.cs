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
        public int MinimumLength { get; set; } = 6;

        /// <summary>
        /// Minimum alpabetic characters required.
        /// </summary>
        public int MinimumAlphabetic { get; set; } = 1;

        /// <summary>
        /// Minimum numeric characters required.
        /// </summary>
        public int MinimumNumeric { get; set; } = 1;
    }
}
