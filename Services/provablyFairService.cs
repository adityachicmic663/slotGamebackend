using System.Runtime.Intrinsics.Arm;
using System.Security.Cryptography;
using System.Text;

namespace SlotGameBackend.Services
{
    public class provablyFairService:IProvablyFairService
    {
        public string GenerateServerSeed()
        {
            return Guid.NewGuid().ToString();
        }

        public string HashServerSeed(string serverSeed)
        {
            using (var sha256 = SHA256.Create())
            {
                var bytes = Encoding.UTF8.GetBytes(serverSeed);
                var hash = sha256.ComputeHash(bytes);
                return BitConverter.ToString(hash).Replace("-", "").ToLower();
            }
        }
        public  int DetermineOutcome(string serverSeed, string clientSeed)
        {
            string combinedSeed=serverSeed+clientSeed;

            using (var sha256 = SHA256.Create())
            {
                byte[] hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(combinedSeed));

                if (hash.Length < 4)
                {
                    throw new InvalidOperationException("Hash length is insufficient to convert to an integer.");
                }


                int outcome =BitConverter.ToInt32(hash, 0);

                outcome = Math.Abs(outcome);
                return outcome;
            }
        }


        public bool VerifyOutcome(string serverSeed, string providedHash)
        {
            var calculatedHash = HashServerSeed(serverSeed);
            return calculatedHash.Equals(providedHash, StringComparison.OrdinalIgnoreCase);
        }

    }
}
