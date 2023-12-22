
namespace WEB_2023.Models
{
    public class ProductViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Alias { get; set; }
        public string? Description { get; set; }
        public string? Detail { get; set; }
        public string? ImageURL { get; set; }
        public decimal Price { get; set; }
        public decimal? DiscountPrice { get; set; }
        public int? CategoryId { get; set; }
        public string? CategoryName { get; set; }
        public string? CreatedByName { get; set; }
        public DateTime? CreatedAt { get; set; }
        public bool? IsActive { get; set; }
        public bool? IsHot { get; set; }
        public int GetDiscountPercent()
        {
            if (DiscountPrice == null)
            {
                return 0;
            }
            return (int)((Price - DiscountPrice) / Price * 100);
        }
    }
}