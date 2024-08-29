using System.ComponentModel.DataAnnotations;

namespace SlotGameBackend.Models
{
    public class Wallet
    {
        [Key]
        public Guid walletId {  get; set; }

        public Guid userId { get; set; }

        public int balance{ get; set; }

        public UserModel user { get; set; }
    }
}
