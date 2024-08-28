﻿using System.ComponentModel.DataAnnotations;

namespace SlotGameBackend.Requests
{
    public class ResetPasswordRequest
    {
        public string token { get; set; }


        [Required(ErrorMessage = "password is required")]
        [RegularExpression("^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)(?=.*[#$^+=!*()@%&]).{8,}$", ErrorMessage = "Passwords must be at least 8 characters and contain each of the following: upper case (A-Z), lower case (a-z), number (0-9) and special character (e.g. !@#$%^&*)")]
        public string newPassword { get; set; }
    }
}
