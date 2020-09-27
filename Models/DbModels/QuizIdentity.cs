using System.Runtime.Serialization;

namespace Models.DbModels
{
    /// <summary>
    /// 
    /// </summary>
    [DataContract]
    public class QuizIdentity
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
        public int OwnerId { get; set; }
    }
}