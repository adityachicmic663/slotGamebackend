namespace SlotGameBackend.Requests
{
    public class AddPayLineRequest
    {
        public List<PositionRequest> positions { get; set; } = new List<PositionRequest>();

        public int multiplier { get; set; } = 1;
    }
}
