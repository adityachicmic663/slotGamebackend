namespace SlotGameBackend.Requests
{
    public class verifyRequest
    {
        public string serverSeed { get; set; }

        public string providedHash { get; set; }
    }
}
