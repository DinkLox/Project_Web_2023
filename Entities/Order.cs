using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WEB_2023.Entities
{
    [Table("Orders")]
    public class Order
    {
        [Key]
        public int Id { get; set; }
        [ForeignKey("User")]
        public string CustomerId { get; set; } = null!;
        public virtual ApplicationUser Customer { get; set; } = null!;
        [Column(TypeName = "Money")]
        public decimal TotalPayment { get; set; }
        [Column(TypeName = "Money")]
        public decimal? TotalAmount { get; set; }
        public int OrderStatus { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}