using SlotGameBackend.Models;

namespace SlotGameBackend.Requests
{
    public class SearchRequest
    {
        public QueryType type { get; set; }

        public int pageNumber {  get; set; }

        public int pageSize { get; set; }
    }
}
