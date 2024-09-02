
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using Pomelo.EntityFrameworkCore.MySql.Query.ExpressionTranslators.Internal;
using SlotGameBackend.Models;
using System.Security.Claims;

namespace SlotGameBackend.Services
{
    public class WalletServices:IWalletService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly slotDataContext _context;
        public WalletServices(IHttpContextAccessor httpContextAccessor,slotDataContext context)
        {
            _httpContextAccessor=httpContextAccessor;
            _context=context;
        }

        public void CreateTransaction(int amount,TransactionType type)
        {
            var userEmail= _httpContextAccessor.HttpContext.User.Claims
                                  .FirstOrDefault(x => x.Type == ClaimTypes.Email)?.Value;
            var user = _context.users.FirstOrDefault(x => x.email == userEmail);
            if (user == null)
            {
                throw new InvalidOperationException("User not found.");
            }

            var isWallet = _context.wallets.Any(x => x.userId == user.userId);

            if (!isWallet)
            {
                var newWallet = new Wallet
                {
                    walletId=Guid.NewGuid(),
                    userId=user.userId,
                    balance=1000
                };
            }
            var wallet = _context.wallets.FirstOrDefault(x => x.userId == user.userId);
           
            var transaction = new Transaction
            {
                walletId = wallet.walletId,
                amount = amount,
                type = type,
                transactionStatus = TransactionStatus.Pending
            };
           _context.transactions.Add(transaction);
            _context.SaveChanges();
        }

        public IEnumerable<TransactionResponse> GetPendingTransactions(int pageNumber,int pageSize)
        {
           var list = _context.transactions.Include(t=>t.wallet).ThenInclude(x=>x.user).Where(t=>t.transactionStatus==TransactionStatus.Pending).OrderByDescending(x=>x.requestedAt).Skip((pageNumber-1)*pageSize).Take(pageSize).ToList();

            var responseList= new List<TransactionResponse>();

            foreach(var transaction in list)
            {
                var response = new TransactionResponse
                {
                    transactionId = transaction.transactionId,
                    requestedAt = transaction.requestedAt,
                    type = transaction.type.ToString(),
                    amount = transaction.amount,
                    status=TransactionStatus.Pending.ToString()
                };
                responseList.Add(response);
            }
            return responseList;
        }

        public async Task<bool> ApproveTransaction(Guid transactionId)
        {
            var transaction=_context.transactions.Include(t=>t.wallet).ThenInclude(x=>x.user).FirstOrDefault(t=>t.transactionId==transactionId);
            if (transaction!=null && transaction.transactionStatus==TransactionStatus.Pending){
                transaction.transactionStatus = TransactionStatus.Approved;
                transaction.amount = transaction.amount;
                transaction.ApprovedAt=DateTime.Now;

                if (transaction.type == TransactionType.Deposit)
                {
                    transaction.wallet.balance += transaction.amount;
                }
                else if(transaction.type==TransactionType.Deposit)
                {
                    transaction.wallet.balance -= transaction.amount;
                }

                 _context.transactions.Update(transaction);
                await _context.SaveChangesAsync();
                return true;
            }

            return false;

        }

        public async Task<bool> RejectTransaction(Guid transactionId)
        {
            var transaction = _context.transactions.Include(t => t.wallet).ThenInclude(x => x.user).FirstOrDefault(t => t.transactionId == transactionId);

            if (transaction != null && transaction.transactionStatus!=TransactionStatus.Approved)
            {
                transaction.transactionStatus = TransactionStatus.Rejected;

                _context.transactions.Update(transaction) ;
               await  _context.SaveChangesAsync();
                return true;    
            }
            return false;
        }

        public IEnumerable<TransactionResponse> GetTransactionHistory(Guid? userId,int pageNumber,int pageSize)
        {
            List<Transaction> transactionList=new List<Transaction>();
            if (userId.HasValue && userId.Value!=Guid.Empty)
            {
                var user = _context.users.Include(u => u.wallet).FirstOrDefault(x => x.userId == userId);
                if (user == null)
                {
                    throw new InvalidDataException("user is not found");
                }

                var wallet = user.wallet;

                if (wallet == null)
                {
                    throw new InvalidDataException("Wallet not found for the user.");
                }

                 transactionList = _context.transactions.Where(x => x.walletId == wallet.walletId).OrderByDescending(x => x.requestedAt).ThenByDescending(x => x.ApprovedAt).Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
            }
            else
            {
                transactionList=_context.transactions.OrderByDescending(x=>x.requestedAt).ThenByDescending(x=>x.ApprovedAt).Skip((pageNumber-1)*pageSize).Take(pageSize).ToList();
            }
     
            List<TransactionResponse> result = new List<TransactionResponse>();

            foreach(var transaction in transactionList)
            {
                var response = new TransactionResponse
                {
                    transactionId = transaction.transactionId,
                    requestedAt = transaction.requestedAt,
                    type = transaction.type.ToString(),
                    amount = transaction.amount,
                    status=transaction.transactionStatus.ToString()
                };
                result.Add(response);
               
            };
             
            return result;

        }

        public IEnumerable<TransactionResponse> SearchTransaction(QueryType type,int pageNumber,int pageSize) {
        
                DateTime startDate = DateTime.MinValue;
                DateTime endDate = DateTime.MaxValue;

                switch (type)
                {
                    case QueryType.today:
                        startDate = DateTime.Today;
                        endDate = startDate.AddDays(1).AddTicks(-1);
                        break;
                    case QueryType.lastday:
                        endDate = DateTime.Today.AddTicks(-1);
                        startDate = endDate.AddDays(-1);
                        break;
                    case QueryType.lastmonth:
                        endDate = DateTime.Now;
                        startDate = endDate.AddMonths(-1);
                        break;
                }

                var transactions=_context.transactions.Where(t=>t.ApprovedAt>=startDate && t.ApprovedAt<=endDate).OrderByDescending(t=>t.ApprovedAt).Skip((pageNumber-1)*pageSize).Take(pageSize).ToList();

                var responseList=new List<TransactionResponse>();
                foreach (var transaction in transactions)
                {
                    var response = new TransactionResponse
                    {
                        transactionId = transaction.transactionId,
                        status = transaction.transactionStatus.ToString(),
                        amount = transaction.amount,
                        requestedAt = transaction.requestedAt,
                        type = transaction.type.ToString()
                    };
                    responseList.Add(response);
                }
                return responseList;
        }

        public async Task<int> checkBalance()
        {
            var userEmail = _httpContextAccessor.HttpContext.User.Claims
                                 .FirstOrDefault(x => x.Type == ClaimTypes.Email)?.Value;
            var user = await _context.users.FirstOrDefaultAsync(x => x.email == userEmail);

            var wallet = await _context.wallets.FirstOrDefaultAsync(x => x.userId == user.userId);

            return wallet.balance;
        }
       
    }
}
