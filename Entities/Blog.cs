using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WEB_2023.Entities
{
    [Table("Blogs")]
    public class Blog
    {
        [Key]
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public string? Alias { get; set; }
        public string? Description { get; set; }
        public string? ImageURL { get; set; }
        public string? Detail { get; set; }
        [ForeignKey("Category")]
        public int CategoryId { get; set; }
        public virtual Category Category { get; set; } = null!;
        [ForeignKey("User")]
        public string CreatedById { get; set; } = null!;
        public virtual ApplicationUser CreatedBy { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public int? ViewCount { get; set; }
        public bool IsActive { get; set; }
    }
}