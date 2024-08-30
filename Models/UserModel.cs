using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography.Xml;

namespace SlotGameBackend.Models
{
    public class UserModel
    {
        [Key]
        public Guid userId {  get; set; }= Guid.NewGuid();

        public string userName { get; set; }

        public string firstName { get; set; }

        public string lastName { get; set; }

        public string role { get; set; }

        public string email { get; set; }

        public string hashPassword { get; set; }

        public bool isBlocked { get; set; } = false;

        public string otpToken { get; set; }

        public DateTime? OtpTokenExpiry { get; set; }
        public string profilePicturePath { get; set; }

        public Wallet wallet { get; set; }  

        public ICollection<GameSession> sessions { get; set; }

        public ICollection<Transaction> transactions { get; set; }

    }
}
