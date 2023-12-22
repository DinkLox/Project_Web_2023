using Microsoft.AspNetCore.Identity;

namespace WEB_2023.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; } = null!;
        public DateOnly? DateOfBirth { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? Address { get; set; }
        public bool? IsDeleted { get; set; }
        public string? Avatar { get; set; }
    }
}