namespace SlotGameBackend.Models
{
    public class PayLineResponse
    {
        public Guid paylineId { get; set; }  
        
        public int multiplier {  get; set; }

        public ICollection<payLinePositionResponse> positions { get; set; }
    }
}
