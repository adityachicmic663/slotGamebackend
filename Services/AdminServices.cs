using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualBasic;
using SlotGameBackend.Models;
using System.Diagnostics.Contracts;
using System.Runtime;
using System.Runtime.CompilerServices;

namespace SlotGameBackend.Services
{
    public class AdminServices:IAdminServices
    {
        private readonly slotDataContext _context;
       
        private readonly string _uploadPath;

      
        public AdminServices(slotDataContext context,IConfiguration configuration) {
            _context = context;
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
    }
}
