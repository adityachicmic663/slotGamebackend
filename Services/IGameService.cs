using SlotGameBackend.Models;

namespace SlotGameBackend.Services
{
    public interface IGameService
    {
        gameSessionResponse StartSession();

        void EndSession();

        Spin SpinReels(int betAmount,string clientSeed);

        IEnumerable<Spin> gamehistory(Guid userId);
    }
}
