using System.ComponentModel.DataAnnotations;

namespace SlotGameBackend.Models
{
    public class PayLine
    {
        [Key]
        [Required]
        public Guid payLineId {  get; set; }

        public int multiplier { get; set; } = 1;

        public ICollection<payLinePositions> positions { get; set; } = new List<payLinePositions>();


    }
}
