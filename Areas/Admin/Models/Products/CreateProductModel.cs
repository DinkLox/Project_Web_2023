
using Microsoft.AspNetCore.Mvc;

namespace WEB_2023.Areas.Admin.Models
{
    [BindProperties]
    public class CreateProductModel
    {
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        /*public string? Alias { get; set; } = null!;*/
        public string? Detail { get; set; } = null!;
        public decimal Price { get; set; }
        public decimal? Discount { get; set; }
        public int CategoryId { get; set; }
        public IFormFile ProductImage { get; set; } = null!;
        public bool IsActive { get; set; }
    }
}