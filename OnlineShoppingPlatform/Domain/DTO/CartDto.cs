namespace OnlineShoppingPlatform.Domain.DTO
{
    public class CartDto
    {
        public string CartId { get; set; }
        public required string UserId { get; set; }
        public required string CartNumber { get; set; }
        public string Status { get; set; } = "Open";
        public decimal Subtotal { get; set; }
        public decimal ShippingCost { get; set; }
        public decimal Total { get; set; }
        public DateTime? ExpiresAt { get; set; }
        public string? Notes { get; set; }
        public IEnumerable<CartItemDto> Items { get; set; } = new List<CartItemDto>();
    }
}
