using System;
using System.Collections.Generic;

namespace BusinessLayer
{
    public class Quiz
    {
        public int Id { get; set; }

        public int Version { get; set; }

        public string Title { get; set; }

        public string Intro { get; set; }

        public IEnumerable<int> QuestionIds { get; set; }

        public int TimeLimitInSeconds { get; set; }

        public bool TimeConstraint { get; set; }

        public IEnumerable<int> PoolIds { get; set; }

        public bool ShuffleQuestions { get; set; }

        public bool ShuffleOptions { get; set; }

        public int PassScore { get; set; }

        public bool Repeatable { get; set; }

        public DateTime ShowResults { get; set; }

        public DateTime AvailableFrom { get; set; }

        public DateTime AvailableTo { get; set; }
    }
}
