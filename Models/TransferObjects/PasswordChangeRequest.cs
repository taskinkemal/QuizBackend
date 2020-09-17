namespace Models.TransferObjects
{
    /// <summary>
    /// 
    /// </summary>
    public class PasswordChangeRequest : OneTimeTokenRequest
    {
        /// <summary>
        /// 
        /// </summary>
        public string Password { get; set; }
    }
}
