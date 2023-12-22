namespace WEB_2023.Models.Auth
{
    public class ResetPasswordModel
    {
        public string UserId { get; set; } = null!;
        public string Token { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string ConfirmPassword { get; set; } = null!;
    }
}