namespace Models.DbModels
{
    /// <summary>
    /// 
    /// </summary>
    public class User
    {
        /// <summary>
        /// 
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string PictureUrl { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public byte[] PasswordHash { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool IsVerified { get; set; }
    }
}
