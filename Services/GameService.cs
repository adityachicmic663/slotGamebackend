using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using SlotGameBackend.Models;
using System.Security.Claims;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using Microsoft.EntityFrameworkCore.Internal;

namespace SlotGameBackend.Services
{
    public class GameService:IGameService
    {
        private readonly slotDataContext _context;
        private  Random _random;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private AdminSettings _settings;
        
        private readonly IProvablyFairService _provablyFairService;

        public GameService(slotDataContext context, IHttpContextAccessor httpContextAccessor,IProvablyFairService provablyFairService)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _random = new Random();
            _provablyFairService = provablyFairService;
        }

        public gameSessionResponse StartSession()
        {
            var UserEmailClaim = _httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Email)?.Value;
       
            var user = _context.users.SingleOrDefault(x => x.email == UserEmailClaim);
            
            var existingSession = _context.gameSessions.SingleOrDefault(x => x.userId == user.userId && x.isActive);

            var session = new GameSession
            {
                sessionId = Guid.NewGuid(),
                userId = user.userId,
                sessionStartTime = DateTime.Now,
                serverSeed=Guid.NewGuid().ToString(),
                isActive = true
            };
            var serverSeedHash = _provablyFairService.HashServerSeed(session.serverSeed);

            var isWallet = _context.wallets.Any(x => x.userId == user.userId);
            if (!isWallet)
            {
                var newWallet = new Wallet()
                {
                    walletId = Guid.NewGuid(),
                    userId = user.userId,
                    balance = 1000
                };
                _context.wallets.Add(newWallet);
                _context.SaveChanges();
            }
            var wallet = _context.wallets.FirstOrDefault(x => x.userId == user.userId);
            if (existingSession != null)
            {
                var response1 = new gameSessionResponse
                {
                    sessionId = existingSession.sessionId,
                    userId = existingSession.userId,
                    sessionStartTime = existingSession.sessionStartTime,
                    serverSeedHash = serverSeedHash,
                    isActive = existingSession.isActive,
                    balance=wallet.balance
                };
                return response1;
            }

            var response = new gameSessionResponse
            {
                sessionId=session.sessionId,
                userId=session.userId,
                sessionStartTime=session.sessionStartTime,
                serverSeedHash = serverSeedHash,
                isActive =session.isActive,
                balance=wallet.balance
            };

            _context.gameSessions.Add(session);
            _context.SaveChanges();

            return response;
        }

       
        public void EndSession()
        {
            var UserEmailClaim = _httpContextAccessor.HttpContext.User.Claims
                                  .FirstOrDefault(x => x.Type == ClaimTypes.Email)?.Value;
            var user = _context.users.SingleOrDefault(x => x.email == UserEmailClaim);

            var session = _context.gameSessions.SingleOrDefault(x => x.userId == user.userId && x.isActive);

            if(session != null)
            {
                session.isActive = false;
                session.sessionEndTime = DateTime.Now;
                session.lastActivityTime= DateTime.Now;

                _context.gameSessions.Update(session);
                _context.SaveChanges();
            }
        }
        

        public SpinResponse SpinReels(int betAmount,string clientSeed)
        {
            var UserEmailClaim = _httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Email)?.Value;
            var user = _context.users.SingleOrDefault(x => x.email == UserEmailClaim);

            string nounce=Guid.NewGuid().ToString();

            if (user == null)
            {
                throw new UnauthorizedAccessException("User is not authenticated.");
            }

            var session = _context.gameSessions.FirstOrDefault(x => x.userId == user.userId);

            if (session == null)
            {
                throw new InvalidOperationException("No active session found.");
            }

            var isWallet=_context.wallets.Any(x=>x.userId == user.userId);
            if (!isWallet)
            {
                var newWallet = new Wallet()
                {
                    walletId = Guid.NewGuid(),
                    userId = user.userId,
                    balance = 1000
                };
                _context.wallets.Add(newWallet);
                _context.SaveChanges();
            }
            var wallet = _context.wallets.FirstOrDefault(x => x.userId == user.userId);

            if(wallet.balance < betAmount) {

                throw new InvalidOperationException("Insufficient balance.");
            }

            var settings =  _context.settings.FirstOrDefault();
            if (settings == null)
            {
                throw new InvalidOperationException("minimum amount is not set yet");
            }
            if (betAmount < settings.minimumBetLimit)
            {
                throw new InvalidOperationException("bet amount is less than minimum limit");
            }
            wallet.balance -= betAmount;
           
            session.lastActivityTime = DateTime.Now;

            int outcomeSeed = _provablyFairService.DetermineOutcome(session.serverSeed, clientSeed,nounce);

            Symbol[,] reelresults = GenerateReelResults(outcomeSeed);

            string reelsOutComeJson=SerializeReelresult(reelresults);

            int winnings = CalculateWinnings(reelresults, betAmount);

            wallet.balance += winnings;

            var spinResult = new Spin
            {
                spinResultId = Guid.NewGuid(),
                sessionId=session.sessionId,
                betAmount = betAmount,
                winAmount = winnings,
                clientSeed = clientSeed,
                spinTime=DateTime.Now,
                reelsOutcome=reelsOutComeJson
            };
            session.spinResults.Add(spinResult);
            _context.spinResults.Add(spinResult);
            _context.SaveChanges();

            string combinedSeed = session.serverSeed + clientSeed+nounce;

            var combinedSeedHash = _provablyFairService.HashServerSeed(combinedSeed);

            var response = new SpinResponse
            {
                spinResultId =spinResult.spinResultId,
                sessionId = session.sessionId,
                betAmount = betAmount,
                winAmount = winnings,
                balance=wallet.balance,
                nounce=nounce,
                spinTime = DateTime.Now,
                serverSeed=session.serverSeed,
                combinedSeedHash = combinedSeedHash,
                reelsOutcome = reelsOutComeJson
            };

            return response;
        }

        private string SerializeReelresult(Symbol[,] reelResults)
        {
            var rows=new List<List<string>>();

            for(int i=0;i<reelResults.GetLength(0);i++)
            {
                var row = new List<string>();
                for(int j = 0; j < reelResults.GetLength(1); j++)
                {
                    row.Add(reelResults[i, j].symbolName);
                }
                rows.Add(row);
            }
            return JsonConvert.SerializeObject(rows);
        }

        private Symbol[,] GenerateReelResults(int seed)
        {
             _random = new Random(seed);

            Symbol[,] reelResults = new Symbol[3, 5];

            var symbols=_context.symbols.ToList();

            if (symbols == null || symbols.Count == 0)
            {
                throw new InvalidOperationException("No symbols available for generating reel results.");
            }


            for (int row = 0; row < 3; row++)
            {
                for(int col = 0; col < 5; col++)
                {
                    int randomIndex = _random.Next(0, symbols.Count);
                    reelResults[row, col] = symbols[randomIndex];
                }
            }

            return reelResults;
        }
     
        private int CalculateWinnings(Symbol[,] reelResults, int betAmount)
        {
            int winnings = 0;
            var paylines=_context.payLines.Include(p=>p.positions).ToList();

            foreach(var payline in paylines)
            {
                bool match = true;
                var firstPosition=payline.positions.First();
                Symbol firstSymbol = reelResults[firstPosition.X, firstPosition.Y];

                foreach(var position in payline.positions)
                {
                    if (reelResults[position.X, position.Y].symbolId != firstSymbol.symbolId) { 
                    match = false;
                        break;
                    }
                }
                if (match)
                {
                    winnings += betAmount * payline.multiplier;
                }
            }
            return winnings;
        }
        public IEnumerable<gameHistoryResponse> gamehistory(Guid? userId, int pageNumber, int pageSize)
        {
            List<Spin> spins= new List<Spin>();

            if(userId.HasValue && userId.Value!=Guid.Empty)
            {
                var user = _context.users.Include(u => u.sessions)
                             .ThenInclude(gs => gs.spinResults)
                             .FirstOrDefault(x => x.userId == userId);

                 spins = user.sessions.SelectMany(gs => gs.spinResults).OrderByDescending(gs => gs.spinTime).Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
            }
            else
            {
                spins = _context.spinResults.OrderByDescending(x=>x.spinTime).Skip((pageNumber-1)*pageSize).Take(pageSize).ToList();
            }

            var list = new List<gameHistoryResponse>();
            foreach (var spin in spins)
            {
                var response = new gameHistoryResponse
                {
                    spinResultId = spin.spinResultId,
                    sessionId = spin.sessionId,
                    betAmount = spin.betAmount,
                    winAmount = spin.winAmount,
                    reelsOutcome = spin.reelsOutcome,
                    spinTime = spin.spinTime,
                };
                list.Add(response);
            }
            // Return the list of spins (game history)
            return list;
        }
    }
}

