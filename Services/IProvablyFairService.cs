namespace SlotGameBackend.Services
{
    public interface IProvablyFairService
    {
        string GenerateServerSeed();
        string HashServerSeed(string serverSeed);
        int DetermineOutcome(string serverSeed, string clientSeed);
        bool VerifyOutcome(string serverSeed, string providedHash);
    }
}
