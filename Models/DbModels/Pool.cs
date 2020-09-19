namespace Models.DbModels
{
    /// <summary>
    /// 
    /// </summary>
    public class Pool
    {
        /// <summary>
        /// 
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int OwnerId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool IsPublic { get; set; }
    }
}
