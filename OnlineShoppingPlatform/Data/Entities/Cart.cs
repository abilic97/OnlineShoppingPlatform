using OnlineShoppingPlatform.Data.Entities.Base;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineShoppingPlatform.Data.Entities
{
    public class Cart : AuditableEntity
    {
        [Key]
        public int CartId { get; set; }

        [Required]
        [StringLength(100)]
        public required string  UserId { get; set; }

        // A human-friendly reference identifier (e.g., "CART-12345")
        [StringLength(50)]
        public string CartNumber { get; set; }

        // Possible values: "Open", "Completed", "Canceled", etc.
        [Required]
        [StringLength(50)]
        public string Status { get; set; } = "Open";

        // The sum of item prices (before shipping)
        [Column(TypeName = "decimal(18,2)")]
        public decimal Subtotal { get; set; }

        // Shipping cost (optional or determined at checkout)
        [Column(TypeName = "decimal(18,2)")]
        public decimal ShippingCost { get; set; }

        // The final price (subtotal + shipping, tax already included in product prices)
        [Column(TypeName = "decimal(18,2)")]
        public decimal Total { get; set; }

        // Optional: if you want the cart to expire or auto-clean
        public DateTime? ExpiresAt { get; set; }

        // Free-form notes or instructions
        [StringLength(1000)]
        public string? Notes { get; set; }

        // Navigation property: each cart can have multiple items
        public ICollection<CartItem> Items { get; set; } = new List<CartItem>();
    }
}
