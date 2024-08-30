using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SlotGameBackend.Models;
using SlotGameBackend.Requests;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SlotGameBackend.Services
{
    public class AuthService:IAuthService
    {
        private readonly slotDataContext _context;
        private readonly string _secretkey;
        private readonly IEmailSender _emailSender;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly string _uploadPath;
        private readonly string[] _permittedExtensions = { ".jpg", ".jpeg", ".png", ".gif" };
        private readonly string[] _permittedMimeTypes = { "image/jpeg", "image/png", "image/gif" };

        public AuthService(slotDataContext context, IHttpContextAccessor httpContextAccessor, IEmailSender emailSender, IOptions<JwtSettings> jwtsettings, IConfiguration configuration)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _emailSender = emailSender;
            _secretkey = jwtsettings.Value.SecretKey;
            _uploadPath = configuration.GetValue<string>("UploadPath");

        }

        public async Task<string> Register(RegisterRequest request)
        {
            
                if ( await _context.users.AnyAsync(x => x.email == request.email))
                {
                    return null;
                }
                var newUser = new UserModel
                {
                    userName = request.userName,
                    firstName = request.firstName,
                    lastName = request.lastName,
                    email = request.email,
                    hashPassword = BCrypt.Net.BCrypt.HashPassword(request.password),
                    role = "user"
                };

                await _context.users.AddAsync(newUser);
               await  _context.SaveChangesAsync();
                return await Login(new LoginRequest
                {   email = request.email,
                    password = request.password,
                });
        }

        public   async Task<string> Login(LoginRequest request)
        {
            var user = await  _context.users.SingleOrDefaultAsync(x => x.email == request.email);

            if (user.isBlocked)
            {
                throw new InvalidOperationException("sorry you are blocked by the admin");
            }
            if (user == null)
            {
                return null;
            }
            if ( !BCrypt.Net.BCrypt.Verify(request.password, user.hashPassword))
            {
                return null;
            }

            return await GenerateJwtToken(user);
        }

        private async Task<string> GenerateJwtToken(UserModel user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_secretkey);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.userId.ToString()),
                    new Claim(ClaimTypes.Email, user.email),
                    new Claim(ClaimTypes.Role, user.role)
                }),
                Expires = DateTime.UtcNow.AddHours(5),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }


        public async Task<bool> Forgotpassword(string email)
        {
            var user = await  _context.users.SingleOrDefaultAsync(x => x.email == email);

            if (user.isBlocked)
            {
                throw new InvalidOperationException("sorry you are blocked by the user");
            }

            if (user == null)
            {
                return false;
            }

            var token =  GeneratePasswordResetToken();
            user.otpToken = token;
            user.OtpTokenExpiry = DateTime.UtcNow.AddMinutes(15);
            _context.SaveChanges();

            await  _emailSender.SendEmailAsync(user.email, "Reset password", $"your otp is {token}");

            return true;
        }

        private string GeneratePasswordResetToken()
        {
            return  Guid.NewGuid().ToString();
        }

        public async Task<bool> ValidateOtpToken( string token)
        {
            var user = await _context.users.SingleOrDefaultAsync(x => x.otpToken == token);
            if (user == null || user.OtpTokenExpiry < DateTime.UtcNow)
            {
                return false;
            }

            return true;
        }
        public  async Task<bool> resetPassword(ResetPasswordRequest request)
        {
            if (! await ValidateOtpToken( request.token))
            {
                return false;
            }
            
            var user = await _context.users.SingleOrDefaultAsync(x => x.otpToken == request.token);

            if (user.isBlocked)
            {
                throw new InvalidOperationException("sorry you are blocked by the user");
            }

            if (user == null )
            {
                return false;
            }

            user.hashPassword = BCrypt.Net.BCrypt.HashPassword(request.newPassword);
            user.otpToken = null;
            user.OtpTokenExpiry = null;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<string> uploadProfile(IFormFile file)
        {

            try
            {
                var UserEmailClaim = _httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Email)?.Value;
                var user = _context.users.SingleOrDefault(x => x.email == UserEmailClaim);

                if (user == null)
                {
                    Console.WriteLine("User not found.");
                    return null;
                }

                if (user.isBlocked)
                {
                    throw new InvalidOperationException("sorry you are blocked by the user");
                }
                if (file.Length > 0 && IsImageFile(file))
                {
                    if (!Directory.Exists(_uploadPath))
                    {
                        Directory.CreateDirectory(_uploadPath);
                    }
                    var filePath = Path.Combine(_uploadPath, file.FileName);

                    using (var filestream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(filestream);
                    }
                    user.profilePicturePath = filePath;
                    await _context.SaveChangesAsync();
                    return filePath;
                }
                Console.WriteLine("Invalid file format or upload failed.");
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
                return null;
            }
        }
        private bool IsImageFile(IFormFile file)
        {
            var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
            var mimeType = file.ContentType.ToLowerInvariant();

            Console.WriteLine($"File Extension: {fileExtension}");
            Console.WriteLine($"MIME Type: {mimeType}");

            return _permittedExtensions.Contains(fileExtension) && _permittedMimeTypes.Contains(mimeType);
        }
    }
}
