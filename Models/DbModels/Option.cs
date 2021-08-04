namespace Models.DbModels
{
    /// <summary>
    /// 
    /// </summary>
    public class Option
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
        public bool IsCorrect { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int QuestionId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public byte OptionOrder { get; set; }
    }
}
