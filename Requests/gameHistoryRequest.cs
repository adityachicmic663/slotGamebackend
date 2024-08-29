namespace SlotGameBackend.Requests
{
    public class gameHistoryRequest
    {
        public Guid userId {  get; set; }

        public int pageNumber {  get; set; }

        public int pageSize { get; set; }
    }
}
