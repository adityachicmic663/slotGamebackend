namespace SlotGameBackend.Models
{
    public class gameHistoryResponse
    {
        public Guid spinResultId { get; set; }

        public Guid sessionId { get; set; }

        public int betAmount { get; set; }

        public int winAmount { get; set; }

        public string reelsOutcome { get; set; }

        public DateTime spinTime { get; set; }
    }
}
