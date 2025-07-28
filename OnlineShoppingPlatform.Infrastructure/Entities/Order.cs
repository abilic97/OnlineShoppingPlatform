using OnlineShoppingPlatform.Infrastructure.Entities.Base;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineShoppingPlatform.Infrastructure.Entities
{
    public class Order : AuditableEntity
    {
        [Key]
        public int OrderId { get; set; }

        [Required]
        [StringLength(100)]
        public string UserId { get; set; }

        [Required]
        [StringLength(20)]
        public string OrderNumber { get; set; }

        [Required]
        [StringLength(50)]
        public string Status { get; set; } = "Pending";

        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalAmount { get; set; }
        public DateTime OrderedAt { get; set; } = DateTime.UtcNow;
        public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();

        [StringLength(1000)]
        public string? Notes { get; set; }
        public int? CartId { get; set; }
        public Cart? Cart { get; set; }
    }
}
