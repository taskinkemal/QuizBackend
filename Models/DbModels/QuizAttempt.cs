using System;
using System.Runtime.Serialization;

namespace Models.DbModels
{
    /// <summary>
    /// 
    /// </summary>
    [DataContract]
    public class QuizAttempt
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
        public int UserId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public int QuizId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public QuizAttemptStatus Status { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public DateTime StartDate { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember(IsRequired = false)]
        public DateTime? EndDate { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember(IsRequired = false)]
        public int? Correct { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember(IsRequired = false)]
        public int? Incorrect { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember(IsRequired = false)]
        public decimal? Score { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public int TimeSpent { get; set; }
    }
}
