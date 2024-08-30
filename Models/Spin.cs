using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SlotGameBackend.Models
{
    public class Spin
    {
        [Key]
        public Guid spinResultId {  get; set; }

        [ForeignKey(nameof(gameSession))]
        public Guid sessionId { get; set; }

        public int betAmount { get; set; }

        public int winAmount {  get; set; }

        public string clientSeed { get; set; }

       public string reelsOutcome { get; set; }

        public DateTime spinTime { get; set; }

        public GameSession gameSession { get; set; }

    }
}
