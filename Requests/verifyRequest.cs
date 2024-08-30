namespace SlotGameBackend.Requests
{
    public class verifyRequest
    {
        public string serverSeed { get; set; }

        public string clientSeed { get; set; }

        public string nounce {  get; set; }

        public string providedHash { get; set; }
    }
}
