using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Models.DbModels
{
    [DataContract]
    public class Quiz
    {
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public int Version { get; set; }

        [DataMember]
        public string Title { get; set; }

        [DataMember]
        public string Intro { get; set; }

        [DataMember]
        public IEnumerable<int> QuestionIds { get; set; }

        [DataMember]
        public int TimeLimitInSeconds { get; set; }

        [DataMember]
        public bool TimeConstraint { get; set; }

        [DataMember]
        public IEnumerable<int> PoolIds { get; set; }

        [DataMember]
        public bool ShuffleQuestions { get; set; }

        [DataMember]
        public bool ShuffleOptions { get; set; }

        [DataMember]
        public int PassScore { get; set; }

        [DataMember]
        public bool Repeatable { get; set; }

        [DataMember(IsRequired = false)]
        public DateTime ShowResults { get; set; }

        [DataMember(IsRequired = false)]
        public DateTime AvailableFrom { get; set; }

        [DataMember(IsRequired=false)]
        public DateTime AvailableTo { get; set; }
    }
}
