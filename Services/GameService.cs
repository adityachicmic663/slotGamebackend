using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using SlotGameBackend.Models;
using System.Security.Claims;

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
                clientSeed = Guid.NewGuid().ToString(),
                serverSeed = Guid.NewGuid().ToString(),
                isActive = true
            };
            var serverSeedHash = _provablyFairService.HashServerSeed(session.serverSeed);
            if (existingSession != null)
            {
                var response1 = new gameSessionResponse
                {
                    sessionId = existingSession.sessionId,
                    userId = existingSession.userId,
                    sessionStartTime = existingSession.sessionStartTime,
                    clientSeed = existingSession.clientSeed,
                    serverSeedHash = serverSeedHash,
                    isActive = existingSession.isActive
                };
                return response1;
            }

            var response = new gameSessionResponse
            {
                sessionId=session.sessionId,
                userId=session.userId,
                sessionStartTime=session.sessionStartTime,
                clientSeed=session.clientSeed,
                serverSeedHash = serverSeedHash,
                isActive =session.isActive
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
                session.lastActivityTime= DateTime.Now;

                _context.gameSessions.Update(session);
                _context.SaveChanges();
            }
        }
        

        public Spin SpinReels(int betAmount,string clientSeed)
        {
            var UserEmailClaim = _httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Email)?.Value;
            var user = _context.users.SingleOrDefault(x => x.email == UserEmailClaim);

            if (user == null)
            {
                throw new UnauthorizedAccessException("User is not authenticated.");
            }

            var session = _context.gameSessions.FirstOrDefault(x => x.userId == user.userId);

            if (session == null)
            {
                throw new InvalidOperationException("No active session found.");
            }

            var wallet=_context.wallets.FirstOrDefault(x=>x.userId == user.userId);

            if(wallet.balance < betAmount) {

                throw new InvalidOperationException("Insufficient balance.");
            }

            if (betAmount < _settings.minimumBetLimit)
            {
                throw new InvalidOperationException("bet amount is more than minimum limit");
            }
            wallet.balance -= betAmount;
           
            session.lastActivityTime = DateTime.Now;

            int outcomeSeed = _provablyFairService.DetermineOutcome(session.serverSeed, clientSeed);

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
                spinTime=DateTime.Now,
                serverSeed=session.serverSeed,
                reelsOutcome=reelsOutComeJson
            };

            session.spinResults.Add(spinResult);
            return spinResult;
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

            for(int row = 0; row < 3; row++)
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
        public IEnumerable<Spin> gamehistory(Guid userId)
        {
            var user = _context.users.Include(u => u.sessions)
                               .ThenInclude(gs => gs.spinResults)
                               .FirstOrDefault(x => x.userId == userId);

            // If the user is not found, return an empty list (or handle it as appropriate)
            if (user == null)
            {
                return new List<Spin>();
            }

            // Get all spins from the user's game sessions
            var spins = user.sessions.SelectMany(gs => gs.spinResults).ToList();

            // Return the list of spins (game history)
            return spins;
        }
    }
}

