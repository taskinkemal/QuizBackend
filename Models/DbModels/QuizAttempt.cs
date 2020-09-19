using System;

namespace Models.DbModels
{
    /// <summary>
    /// 
    /// </summary>
    public class QuizAttempt
    {
        /// <summary>
        /// 
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int QuizId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DateTime EndDate { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int Correct { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int Incorrect { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public decimal Score { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public QuizResult Result { get; set; }
    }
}
