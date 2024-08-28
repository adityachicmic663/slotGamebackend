
using Microsoft.EntityFrameworkCore;
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
            var transaction = new Transaction
            {
                walletId = user.wallet.walletId,
                amount = amount,
                type = type,
                transactionStatus = TransactionStatus.Pending
            };
           _context.transactions.Add(transaction);
            _context.SaveChanges();
        }

        public IEnumerable<TransactionResponse> GetPendingTransactions()
        {
           var list = _context.transactions.Include(t=>t.wallet).ThenInclude(x=>x.user).Where(t=>t.transactionStatus==TransactionStatus.Pending).OrderByDescending(x=>x.requestedAt).ToList();

            var responseList= new List<TransactionResponse>();

            foreach(var transaction in list)
            {
                var response = new TransactionResponse
                {
                    transactionId = transaction.transactionId,
                    requestedAt = transaction.requestedAt,
                    type = transaction.type,
                    amount = transaction.amount,
                    status=TransactionStatus.Pending
                };
                responseList.Add(response);
            }
            return responseList;
        }

        public void ApproveTransaction(Guid transactionId)
        {
            var transaction=_context.transactions.Include(t=>t.wallet).ThenInclude(x=>x.user).FirstOrDefault(t=>t.transactionId==transactionId);
            if (transaction!=null || transaction.transactionStatus==TransactionStatus.Pending){
                transaction.transactionStatus = TransactionStatus.Approved;
                transaction.amount = transaction.amount;

                if (transaction.type == TransactionType.Deposit)
                {
                    transaction.wallet.balance += transaction.amount;
                }
                else if(transaction.type==TransactionType.Deposit)
                {
                    transaction.wallet.balance -= transaction.amount;
                }

                _context.transactions.Update(transaction);
                _context.SaveChanges();
            }

        }

        public void RejectTransaction(Guid transactionId)
        {
            var transaction = _context.transactions.Include(t => t.wallet).ThenInclude(x => x.user).FirstOrDefault(t => t.transactionId == transactionId);

            if (transaction != null)
            {
                transaction.transactionStatus = TransactionStatus.Rejected;

                _context.transactions.Update(transaction) ;
                _context.SaveChanges();
            }
        }

        public IEnumerable<TransactionResponse> GetTransactionHistory(Guid userId)
        {
            var user = _context.users.FirstOrDefault(x=>x.userId==userId);
            if (user == null)
            {
                throw new InvalidDataException("user is not found");
            }
            var transactionList = _context.transactions.Where(x => x.walletId == user.wallet.walletId).OrderByDescending(x => x.requestedAt).ThenByDescending(x => x.ApprovedAt).ToList();
           

            List<TransactionResponse> result = new List<TransactionResponse>();

            foreach(var transaction in transactionList)
            {
                var response = new TransactionResponse()
                {
                    transactionId = transaction.transactionId,
                    requestedAt = transaction.requestedAt,
                    type = transaction.type,
                    amount = transaction.amount,
                    status=transaction.transactionStatus
                };
                result.Add(response);
               
            };
             
            return result;

        }

        public IEnumerable<TransactionResponse> SearchTransaction(QueryType type,int pageNumber,int pageSize) {
        {
                DateTime startDate = new DateTime();
                DateTime endDate = new DateTime();
                if (type == QueryType.today)
                {
                    startDate = DateTime.Today;
                    endDate = startDate.AddDays(1).AddTicks(-1);
                }
                else if(type == QueryType.lastday) 
                {
                    endDate=DateTime.Now;
                    startDate = endDate.AddDays(-1);
                }else if(type == QueryType.lastmonth){
                    endDate=DateTime.Now;
                    startDate = endDate.AddMonths(-1);
                }

                var transactions=_context.transactions.Where(t=>t.ApprovedAt>=startDate && t.ApprovedAt<=endDate).OrderByDescending(t=>t.ApprovedAt).Skip((pageNumber-1)*pageSize).Take(pageSize).ToList();

                var responseList=new List<TransactionResponse>();
                foreach (var transaction in transactions)
                {
                    var response = new TransactionResponse()
                    {
                        transactionId = transaction.transactionId,
                        status = transaction.transactionStatus,
                        amount = transaction.amount,
                        requestedAt = transaction.requestedAt,
                        type = transaction.type
                    };
                    responseList.Add(response);
                }
                return responseList;
        }
        }
       
    }
}
