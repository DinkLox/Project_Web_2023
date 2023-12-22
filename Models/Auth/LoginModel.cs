using Microsoft.AspNetCore.Mvc;

namespace WEB_2023.Models.Auth
{
    [BindProperties]
    public class LoginModel
    {
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
        public bool RememberMe { get; set; }
        public string? ReturnUrl { get; set; } = null!;
    }
}