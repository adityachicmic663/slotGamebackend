namespace SlotGameBackend.Models
{
    public class gameSessionResponse
    {
        public Guid sessionId { get; set; }

        public Guid userId { get; set; }

        public DateTime sessionStartTime { get; set; }

        public string clientSeed { get; set; }

        public string serverSeedHash { get; set; }

        public bool isActive {  get; set; } 
    }
}
