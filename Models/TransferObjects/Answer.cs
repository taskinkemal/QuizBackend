using System.Collections.Generic;

namespace Models.TransferObjects
{
    /// <summary>
    /// 
    /// </summary>
    public class Answer
    {
        /// <summary>
        /// 
        /// </summary>
        public int TimeSpent { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int QuestionId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public IEnumerable<int> Options { get; set; }
    }
}
