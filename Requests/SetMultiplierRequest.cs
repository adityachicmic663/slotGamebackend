namespace SlotGameBackend.Requests
{
    public class SetMultiplierRequest
    {
        public Guid paylineId {  get; set; }

        public int value {  get; set; }
    }
}
