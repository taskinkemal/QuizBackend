using System.Collections.Generic;

namespace Models.DbModels
{
    /// <summary>
    /// 
    /// </summary>
    public class Question
    {
        /// <summary>
        /// 
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Body { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int Level { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public IEnumerable<int> PoolIds { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public IEnumerable<string> Tags { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public QuestionType QuestionType { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public IEnumerable<int> OptionIds { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public IEnumerable<int> CorrectOptionIds { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Dictionary<int, int> MatchingOptionIds { get; set; }
    }
}
