using SlotGameBackend.Models;
using SlotGameBackend.Requests;

namespace SlotGameBackend.Services
{
    public interface IAuthService
    {

        Task<string> Login(LoginRequest request);

        Task<string> Register(RegisterRequest request);

        Task<bool> Forgotpassword(string email);

        Task<bool> resetPassword(ResetPasswordRequest request);

        Task<string> uploadProfile(IFormFile file);

        Task<UserProfileResponse> changeProfile(string? userName, string? firstName, string? lastName);

        Task<bool> changePassword(string oldPassword, string newPassword);

        Task<UserProfileResponse> getProfile();
    }
}
