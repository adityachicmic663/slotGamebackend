using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualBasic;
using OfficeOpenXml.Style;
using OfficeOpenXml;
using SlotGameBackend.Models;
using System.IO;
using System.Diagnostics.Contracts;
using System.Runtime;
using System.Runtime.CompilerServices;

namespace SlotGameBackend.Services
{
    public class AdminServices:IAdminServices
    {
        private readonly slotDataContext _context;
        private readonly IGameService _gameService;
       
        private readonly string _uploadPath;

      
        public AdminServices(slotDataContext context,IConfiguration configuration,IGameService gameService) {
            _context = context;
            _gameService = gameService;
            _uploadPath = configuration.GetValue<string>("UploadPath");
        }
        public async Task SetMinBetLimit(int minBetLimit)
         {
            var settings = await _context.settings.FirstOrDefaultAsync();

            if (settings == null)
            {
                // Handle the case where the settings are not found
                // You might want to create a new Settings entry if it doesn't exist
                settings = new AdminSettings
                {
                    settingId = Guid.NewGuid(),
                    minimumBetLimit = minBetLimit
                };
                _context.settings.Add(settings);
            }
            else
            {
                settings.minimumBetLimit = minBetLimit;
            }

            await _context.SaveChangesAsync();
        }

        public async Task AddPayLine( List<Tuple<int,int>> positions, int multiplier)
        {
            var payline = new PayLine
            {
                payLineId = Guid.NewGuid(),
                multiplier = multiplier
            };

            foreach( var position in positions )
            {
                if( position.Item1 >2 || position.Item2 > 4)
                {
                    throw new InvalidDataException("index out of 3*5 matrix");
                }
                payline.positions.Add(new payLinePositions
                {
                    positionId = Guid.NewGuid(),
                    X= position.Item1,
                    Y= position.Item2,
                    payLineId=payline.payLineId
                });
            }
            _context.payLines.Add(payline);
            await _context.SaveChangesAsync();
        }

        public async Task RemovePaylineAsync(Guid paylineId)
        {
            
            var payline = await _context.payLines.FirstOrDefaultAsync(x=>x.payLineId==paylineId);

            if (payline == null)
            {
                throw new KeyNotFoundException("PayLine not found.");
            }

             _context.payLines.Remove(payline);
                await _context.SaveChangesAsync();
               
        }

        public void Setmultiplier(int multiplier,Guid paylineId)
        {
            var payline=_context.payLines.FirstOrDefault(x => x.payLineId==paylineId);

            payline.multiplier = multiplier;
            _context.payLines.Update(payline);
            _context.SaveChanges();
        }

        public async Task<bool> AddSymbolAsync(string symbolName,IFormFile image)
        {
            try
            {
                var imagepath = await SaveImageFileAsync(image);
                var symbol = new Symbol
                {
                    symbolName = symbolName,
                    imagePath = imagepath
                };
                _context.symbols.Add(symbol);
                await _context.SaveChangesAsync();
                return true;
            }catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return false;
            }
        }

        public async Task<List<Symbol>> getSymbol()
        {
            var list=await _context.symbols.ToListAsync();

            return list;
        }
        private async Task<string> SaveImageFileAsync(IFormFile image)
        {
                if (!Directory.Exists(_uploadPath))
                {
                    Directory.CreateDirectory(_uploadPath);
                }

                var filePath = Path.Combine(_uploadPath, image.FileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await image.CopyToAsync(stream);
                }
              return filePath;
        }

       public async Task<List<PayLineResponse>> getPayline()
        {
            var list=await _context.payLines.Include(x=>x.positions).ToListAsync();

            var paylines=new List<PayLineResponse>();

            foreach(var line in list)
            {
                var response = new PayLineResponse
                {
                    paylineId = line.payLineId,
                    multiplier = line.multiplier,
                    positions = line.positions.Select(pos => new payLinePositionResponse
                    {
                        positionId = pos.positionId,
                        X = pos.X,
                        Y = pos.Y,
                    }).ToList()
                };
            paylines.Add(response);
            }
            return paylines;
        }

        public async Task<List<UserResponse>> getUsers()
        {
            var list = await _context.users.Where(x=>x.role=="user").ToListAsync();

            var newList=new List<UserResponse>();

            foreach(var user in list)
            {
                var response = new UserResponse
                {
                    UserId = user.userId,
                    UserName = user.userName,
                    UserEmail = user.email,
                    isBlocked = user.isBlocked
                };
                newList.Add(response);
            }
            return newList;
        }

        public async Task<bool> blockUser(Guid userId)
        {
            var user=await _context.users.FirstOrDefaultAsync(x=>x.userId== userId);

            if (user.isBlocked==false)
            {
                user.isBlocked = true;
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<byte[]> GenerateGameHistoryExcelReport(Guid userId,DateTime startDate,DateTime endDate)
        {
            var user =await  _context.users.Include(u => u.sessions)
                               .ThenInclude(gs => gs.spinResults)
                               .FirstOrDefaultAsync(x => x.userId == userId);

            if(user == null)
            {
                throw new Exception("User not found.");
            }

            var spins=user.sessions.SelectMany(gs=>gs.spinResults).Where(spin=>spin.spinTime>=startDate && spin.spinTime<=endDate).OrderByDescending(spin=>spin.spinTime).ToList();

            if (!spins.Any())
            {
                throw new Exception("No spin results found for the specified date range.");
            }


            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("GameHistory");

                worksheet.Cells[1, 1].Value = "spinResultId";
                worksheet.Cells[1, 2].Value = "sessionId";
                worksheet.Cells[1, 3].Value = "betAmount";
                worksheet.Cells[1, 4].Value = "winAmount";
                worksheet.Cells[1, 5].Value = "reelOutcome";
                worksheet.Cells[1, 6].Value = "spinTime";

                using (var range = worksheet.Cells[1, 1, 1, 6])
                {
                    range.Style.Font.Bold = true;
                    range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                }

                int row = 2;
                foreach (var result in spins)
                {
                    worksheet.Cells[row, 1].Value = result.spinResultId.ToString();
                    worksheet.Cells[row, 2].Value = result.sessionId.ToString();
                    worksheet.Cells[row, 3].Value = result.betAmount;
                    worksheet.Cells[row, 4].Value = result.winAmount;
                    worksheet.Cells[row, 5].Value = result.reelsOutcome;
                    worksheet.Cells[row, 6].Value = result.spinTime.ToString("yyyy-MM-dd HH:mm:ss");
                    row++;
                }

                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                var excelReport = package.GetAsByteArray();

                var filePath = Path.Combine(_uploadPath, "GameHistoryReport.xlsx");
                await File.WriteAllBytesAsync(filePath, excelReport);

                return excelReport;
            }
            }
        }
}
