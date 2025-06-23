using OnlineShoppingPlatform.Data.Entities.Base;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace OnlineShoppingPlatform.Data.Entities
{
    public class CartItem : AuditableEntity
    {
        [Key]
        public int CartItemId { get; set; }

        // Foreign key to Cart
        [ForeignKey(nameof(Cart))]
        public int CartId { get; set; }

        // Foreign key to Product
        [ForeignKey(nameof(Product))]
        public int ProductId { get; set; }

        [Range(1, 9999)]
        public int Quantity { get; set; }

        // Navigation properties
        [JsonIgnore]
        public Cart Cart { get; set; }
        [JsonIgnore]
        public Product Product { get; set; }
    }
}
