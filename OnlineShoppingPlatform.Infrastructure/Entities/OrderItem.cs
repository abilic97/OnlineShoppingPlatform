using OnlineShoppingPlatform.Infrastructure.Entities.Base;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineShoppingPlatform.Infrastructure.Entities
{
    public class OrderItem : AuditableEntity
    {
        [Key]
        public int OrderItemId { get; set; }

        [ForeignKey(nameof(Order))]
        public int OrderId { get; set; }

        [ForeignKey(nameof(Product))]
        public int ProductId { get; set; }

        [Range(1, 9999)]
        public int Quantity { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal UnitPrice { get; set; } // Product price at the time of order

        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalPrice => UnitPrice * Quantity;
        public Product Product { get; set; }
        public Order Order { get; set; }
    }
}
