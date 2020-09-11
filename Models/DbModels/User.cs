namespace Models.DbModels
{
    public class User
    {
        public int Id { get; set; }

        public string Email { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string PictureUrl { get; set; }

        public byte[] PasswordHash { get; set; }

        public bool IsVerified { get; set; }
    }
}
