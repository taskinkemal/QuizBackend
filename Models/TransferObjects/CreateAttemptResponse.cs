using Models.DbModels;

namespace Models.TransferObjects
{
    /// <summary>
    /// 
    /// </summary>
    public class CreateAttemptResponse
    {
        /// <summary>
        /// 
        /// </summary>
        public CreateAttemptResult Result { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public QuizAttempt Attempt { get; set; }
    }
}
