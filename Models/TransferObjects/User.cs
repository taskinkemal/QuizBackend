namespace Models.TransferObjects
{
    public class User
    {
        public string Email { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string PictureUrl { get; set; }

        public string Password { get; set; }

        public bool IsVerified { get; set; }

        public string DeviceId { get; set; }
    }
}
