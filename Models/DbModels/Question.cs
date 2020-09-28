using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

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
        public byte Level { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public QuestionType Type { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [NotMapped]
        public IEnumerable<int> OptionIds { get; set; }
    }
}
