using System.Collections.Generic;

namespace Models.DbModels
{
    /// <summary>
    /// 
    /// </summary>
    public class Answer
    {
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
        public int QuestionId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public IEnumerable<int> Options { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Dictionary<int, int> MatchingOptions { get; set; }
    }
}
