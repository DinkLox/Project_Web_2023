using Microsoft.AspNetCore.Mvc;

namespace WEB_2023.Models.Auth
{
    [BindProperties]
    public class SignUpModel
    {
        public string FullName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string ConfirmPassword { get; set; } = null!;
    }
}