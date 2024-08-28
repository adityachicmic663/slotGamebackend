using System.ComponentModel.DataAnnotations;

namespace SlotGameBackend.Models
{
    public class payLinePositions
    {
        [Key]
        public Guid positionId { get; set; }

        public int X {  get; set; }

        public int Y { get; set; }

        public Guid payLineId { get; set; } 

        public PayLine PayLine { get; set; }
    }
}
