using System.ComponentModel.DataAnnotations;

namespace SlotGameBackend.Models
{
    public class Transaction
    {
        [Key] 
        public Guid transactionId {  get; set; }=Guid.NewGuid();

        public Guid walletId { get; set; }

        public int  amount { get; set; }

        public TransactionType type{ get; set; }

        public DateTime requestedAt{ get; set; }=DateTime.Now;

        public TransactionStatus transactionStatus { get; set; }

        public DateTime?  ApprovedAt { get; set; }

        public Wallet wallet { get; set; }
    }
}
