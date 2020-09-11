namespace Models.DbModels
{
    public class Pool
    {
        public int Id { get; set; }

        public int OwnerId { get; set; }

        public string Name { get; set; }

        public bool IsPublic { get; set; }
    }
}
