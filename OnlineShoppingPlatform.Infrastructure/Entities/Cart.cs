using OnlineShoppingPlatform.Infrastructure.Entities.Base;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineShoppingPlatform.Infrastructure.Entities
{
    public class Cart : AuditableEntity
    {
        [Key]
        public int CartId { get; set; }

        [Required]
        [StringLength(100)]
        public required string UserId { get; set; }

        [StringLength(50)]
        public string CartNumber { get; set; }

        [Required]
        [StringLength(50)]
        public string Status { get; set; } = "Open";

        [Column(TypeName = "decimal(18,2)")]
        public decimal Subtotal { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal ShippingCost { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Total { get; set; }

        public DateTime? ExpiresAt { get; set; }

        [StringLength(1000)]
        public string? Notes { get; set; }

        public ICollection<CartItem> Items { get; set; } = new List<CartItem>();
    }
}
