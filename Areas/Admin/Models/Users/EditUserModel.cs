using Microsoft.AspNetCore.Mvc;

namespace WEB_2023.Areas.Admin.Models
{
    [BindProperties]
    public class EditUserModel
    {
        public string? Email { get; set; } = null!;
        public string? FullName { get; set; }
        public string? Phone { get; set; }
        public IFormFile? ImageFile { get; set; }
        public DateOnly? DateOfBirth { get; set; }
        public string? Address { get; set; } = null!;
        public string RoleName { get; set; } = null!;
    }
}