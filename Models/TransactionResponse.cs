namespace SlotGameBackend.Models
{
    public class TransactionResponse
    {
        public Guid transactionId { get; set; } 

        public int amount { get; set; }

        public string type { get; set; }

        public string status { get; set; }

        public DateTime requestedAt { get; set; } 

    }
}
