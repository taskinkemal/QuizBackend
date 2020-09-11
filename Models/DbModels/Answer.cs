using System.Collections.Generic;

namespace Models.DbModels
{
    public class Answer
    {
        public int UserId { get; set; }

        public int QuizId { get; set; }

        public int QuestionId { get; set; }

        public IEnumerable<int> Options { get; set; }

        public Dictionary<int, int> MatchingOptions { get; set; }
    }
}
