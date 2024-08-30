namespace SlotGameBackend.Models
{
    public class downloadRequest
    {
        public Guid userId { get; set; }

        public DateTime startDate { get; set; }

        public DateTime endDate { get; set; }
    }
}
