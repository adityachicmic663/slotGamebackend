namespace SlotGameBackend.Models
{
    public class TransactionResponse
    {
        public Guid transactionId { get; set; } 

        public int amount { get; set; }

        public TransactionType type { get; set; }

        public TransactionStatus status { get; set; }


        public DateTime requestedAt { get; set; } 

    }
}
