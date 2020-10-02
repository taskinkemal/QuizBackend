using Models.DbModels;

namespace Models.TransferObjects
{
    /// <summary>
    /// 
    /// </summary>
    public class UpdateQuizAttemptStatus
    {
        /// <summary>
        /// 
        /// </summary>
        public int TimeSpent { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool EndQuiz { get; set; }
    }
}
