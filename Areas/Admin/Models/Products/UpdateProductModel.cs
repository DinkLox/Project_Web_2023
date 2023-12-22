using Microsoft.AspNetCore.Mvc;

namespace WEB_2023.Areas.Admin.Models
{
    [BindProperties]
    public class UpdateProductModel
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? Detail { get; set; }
        public decimal? Price { get; set; }
        public decimal? Discount { get; set; }
        public int? CategoryId { get; set; }
        public IFormFile? ProductImage { get; set; }
        public bool? IsActive { get; set; }
    }
}