using SlotGameBackend.Models;

namespace SlotGameBackend.Services
{
    public interface IGameService
    {
        gameSessionResponse StartSession();

        void EndSession();

        SpinResponse SpinReels(int betAmount,string clientSeed);

        IEnumerable<gameHistoryResponse> gamehistory(Guid userId,int pageNumber,int pageSize);
    }
}
