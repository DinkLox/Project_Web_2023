using Microsoft.AspNetCore.Mvc;

namespace WEB_2023.Areas.Admin.Models
{
    [BindProperties]
    public class UpdateBlogModel
    {
        public string? Title { get; set; } = null!;
        public string? Description { get; set; }
        public string? Content { get; set; }
        public int? CategoryId { get; set; }
        public IFormFile? Image { get; set; }
        public bool? IsActive { get; set; }
    }
}