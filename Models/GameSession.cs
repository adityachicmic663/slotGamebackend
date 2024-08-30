using System.ComponentModel.DataAnnotations;

namespace SlotGameBackend.Models
{
    public class GameSession
    {
        [Key]
        public Guid sessionId { get; set; } 
        public Guid userId { get; set; }

        public DateTime sessionStartTime { get; set; }= DateTime.Now;

        public DateTime? sessionEndTime { get; set;}

        public bool isActive {  get; set; }

        public string serverSeed {  get; set; }

        public DateTime lastActivityTime {  get; set; }= DateTime.Now;  

        public UserModel user { get; set; }

        public ICollection<Spin> spinResults {  get; set; }= new List<Spin>();
       

    }
}
