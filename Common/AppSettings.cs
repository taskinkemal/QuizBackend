namespace Common
{
    /// <summary>
    /// 
    /// </summary>
    public class AppSettings
    {
        /// <summary>
        /// 
        /// </summary>
        public string ConnectionString { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string ApiKey { get; set; }

        public EmailSettings Email { get; set; }
    }
}