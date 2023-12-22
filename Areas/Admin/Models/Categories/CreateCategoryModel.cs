using Microsoft.AspNetCore.Mvc;

namespace WEB_2023.Areas.Admin.Models
{
    [BindProperties]
    public class CreateCategoryModel
    {
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public bool IsActive { get; set; }
    }
}