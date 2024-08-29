namespace SlotGameBackend.Requests
{
    public class transHistoryRequest
    {
        public Guid userId {  get; set; }   

        public int pageNumber { get; set; }

        public int pageSize { get; set; }
    }
}
