namespace Common
{
    /// <summary>
    /// 
    /// </summary>
    public class HttpErrorMessage
    {
        /// <summary>
        /// Error code
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// Error message
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="code">Error code</param>
        /// <param name="message">Error message</param>
        public HttpErrorMessage(string code, string message)
        {
            Code = code;
            Message = message;
        }
    }
}
