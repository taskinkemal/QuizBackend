namespace Models.TransferObjects
{
    /// <summary>
    /// 
    /// </summary>
    public enum UpdateQuizAttemptStatusResult
    {
        /// <summary>
        /// 
        /// </summary>
        Success,
        /// <summary>
        /// 
        /// </summary>
        NotAuthorized,
        /// <summary>
        /// 
        /// </summary>
        StatusError,
        /// <summary>
        /// 
        /// </summary>
        DateError,
        /// <summary>
        /// As a sideeffect, this completes the quiz.
        /// </summary>
        TimeUp
    }
}
