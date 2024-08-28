﻿using SlotGameBackend.Models;

namespace SlotGameBackend.Services
{
    public interface IAdminServices
    {
        Task SetMinBetLimit(int minBetLimit);

         Task AddPayLine(List<Tuple<int, int>> positions, int multiplier);

        Task RemovePaylineAsync(Guid paylineId);

        void Setmultiplier(int multiplier, Guid paylineId);

        Task<bool> AddSymbolAsync(string symbolName, IFormFile image);





    }
}
