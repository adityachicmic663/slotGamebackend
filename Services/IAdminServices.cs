using SlotGameBackend.Models;

namespace SlotGameBackend.Services
{
    public interface IAdminServices
    {
        Task SetMinBetLimit(int minBetLimit);

         Task AddPayLine(List<Tuple<int, int>> positions, int multiplier);

         Task<List<Symbol>> getSymbol();

        Task RemovePaylineAsync(Guid paylineId);

        void Setmultiplier(int multiplier, Guid paylineId);

        Task<bool> AddSymbolAsync(string symbolName, IFormFile image);

        Task<List<PayLineResponse>> getPayline();

        Task<List<UserResponse>> getUsers();

        Task<bool> blockUser(Guid userId);

        Task<byte[]> GenerateGameHistoryExcelReport(Guid userId, DateTime startDate, DateTime endDate);
    }
}
