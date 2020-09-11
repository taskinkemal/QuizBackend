namespace Models.TransferObjects
{
    public class PasswordChangeRequest : OneTimeTokenRequest
    {
        public string Password { get; set; }
    }
}
