using System.Collections.Generic;

namespace BusinessLayer
{
    public class Question
    {
        public int Id { get; set; }

        public string Body { get; set; }

        public int Level { get; set; }

        public IEnumerable<int> PoolIds { get; set; }

        public IEnumerable<string> Tags { get; set; }

        public QuestionType QuestionType { get; set; }

        public IEnumerable<int> OptionIds { get; set; }

        public IEnumerable<int> CorrectOptionIds { get; set; }

        public Dictionary<int, int> MatchingOptionIds { get; set; }
    }
}
