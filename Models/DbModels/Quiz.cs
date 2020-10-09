using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

[assembly: InternalsVisibleTo("Models.Test")]
namespace Models.DbModels
{
    /// <summary>
    /// 
    /// </summary>
    [DataContract]
    public class Quiz
    {
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public int Id { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public int QuizIdentityId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public QuizStatus Status { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public int Version { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public string Title { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public string Intro { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        [NotMapped]
        public IEnumerable<int> QuestionIds { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public int TimeLimitInSeconds { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public bool TimeConstraint { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public bool ShuffleQuestions { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public bool ShuffleOptions { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember(IsRequired = false)]
        public int? PassScore { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public bool Repeatable { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember(IsRequired = false)]
        public DateTime? ShowResults { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember(IsRequired = false)]
        public DateTime? AvailableFrom { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember(IsRequired = false)]
        public DateTime? AvailableTo { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember(IsRequired = false)]
        [NotMapped]
        public QuizAttempt LastAttempt { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember(IsRequired = false)]
        [NotMapped]
        public bool CanStart =>
            IsAvailable(this) &&
            (LastAttempt == null || (LastAttempt.Status != QuizAttemptStatus.Incomplete && Repeatable));

        /// <summary>
        /// 
        /// </summary>
        [DataMember(IsRequired = false)]
        [NotMapped]
        public bool CanResume =>
            IsAvailable(this) &&
            LastAttempt != null &&
            LastAttempt.Status == QuizAttemptStatus.Incomplete;

        internal static bool IsAvailable(Quiz quiz)
        {
            return
                quiz != null &&
                (quiz.AvailableFrom == null || DateTime.Now >= quiz.AvailableFrom) &&
                (quiz.AvailableTo == null || DateTime.Now < quiz.AvailableTo);
        }
    }
}
