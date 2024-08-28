using System.ComponentModel.DataAnnotations;

namespace SlotGameBackend.Requests
{
    public class forgotPasswordRequest
    {
        [Required(ErrorMessage = "Email address is required")]
        [EmailAddress(ErrorMessage = "email address is required")]
        public string email { get; set; }
    }
}
