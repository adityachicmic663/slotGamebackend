using System.ComponentModel.DataAnnotations;

namespace SlotGameBackend.Models
{
    public class AdminSettings
    {
        [Key]
        public Guid settingId {  get; set; }    

        public int minimumBetLimit { get; set; } 



    }
}
