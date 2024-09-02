namespace SlotGameBackend.Models
{
    public class downloadRequest
    {
        public Guid userId { get; set; }

        public DateTime startDate { get; set; }

        public DateTime endDate { get; set; }

        public int pageNumber { get; set; }

        public int pageSize { get; set; }
    }
}
