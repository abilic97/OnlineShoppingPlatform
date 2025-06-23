using OnlineShoppingPlatform.Data.Entities.Base;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineShoppingPlatform.Data.Entities
{
    public class Product : AuditableEntity
    {
        [Key]
        public int ProductId { get; set; }

        [Required]
        [StringLength(200)]
        public string Name { get; set; }

        [StringLength(1000)]
        public string Description { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        // Tracks how many items are in stock
        public int StockQuantity { get; set; }

        // Boolean to easily mark if the product is in stock
        public bool IsInStock { get; set; }
    }
}
