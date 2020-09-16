namespace Common.Interfaces
{
    /// <summary>
    /// 
    /// </summary>
    public interface IEmailManager
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="email"></param>
        /// <param name="subject"></param>
        /// <param name="body"></param>
        void Send(string email, string subject, string body);
    }
}
