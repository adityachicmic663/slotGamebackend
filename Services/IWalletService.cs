using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using SlotGameBackend.Models;
using System.Transactions;

namespace SlotGameBackend.Services
{
    public interface IWalletService
    {
        void CreateTransaction(int amount, TransactionType type);

        IEnumerable<TransactionResponse> GetPendingTransactions();

        void ApproveTransaction(Guid transactionId);

        void RejectTransaction(Guid transactionId);

        IEnumerable<TransactionResponse> GetTransactionHistory(Guid userId);

        IEnumerable<TransactionResponse> SearchTransaction(QueryType type, int pageNumber, int pageSize);

    }
}
