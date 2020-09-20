namespace Models.TransferObjects
{
    /// <summary>
    /// 
    /// </summary>
    public class PasswordChangeRequestWithToken : OneTimeTokenRequest
    {
        /// <summary>
        /// 
        /// </summary>
        public string Password { get; set; }
    }
}
