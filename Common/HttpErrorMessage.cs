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
        /// 
        /// </summary>
        /// <param name="code">Error code</param>
        public HttpErrorMessage(string code)
        {
            Code = code;
        }
    }
}
