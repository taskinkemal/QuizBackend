namespace Models.DbModels
{
    /// <summary>
    /// 
    /// </summary>
    public enum QuestionType : byte
    {
        /// <summary>
        /// Multiple options, one correct option
        /// </summary>
        MultiSelect = 1,
        /// <summary>
        /// Multiple options, multiple correct options
        /// </summary>
        MultiSelectMultiOptions = 2
    }
}
