using OnlineShoppingPlatform.Data.Entities;
using OnlineShoppingPlatform.Domain.DTO;

namespace OnlineShoppingPlatform.Domain.EntityMappers
{
    public static class CartMapper
    {
        public static CartDto ToDto(this Cart cart)
        {
            return new CartDto
            {
                CartId = cart.CartId,
                UserId = cart.UserId,
                CartNumber = cart.CartNumber,
                Status = cart.Status,
                Subtotal = cart.Subtotal,
                ShippingCost = cart.ShippingCost,
                Total = cart.Total,
                ExpiresAt = cart.ExpiresAt,
                Notes = cart.Notes,
                Items = cart.Items?.Select(i => i.ToDto()).ToList() ?? new List<CartItemDto>()
            };
        }

        public static Cart ToEntity(this CartDto cartDto)
        {
            return new Cart
            {
                CartId = cartDto.CartId,
                UserId = cartDto.UserId,
                CartNumber = cartDto.CartNumber,
                Status = cartDto.Status,
                Subtotal = cartDto.Subtotal,
                ShippingCost = cartDto.ShippingCost,
                Total = cartDto.Total,
                ExpiresAt = cartDto.ExpiresAt,
                Notes = cartDto.Notes,
                //TODO: Dummy values for now. In future, add auditing DB update mechanism
                // Helps with bookeeping and customer reported bugs if we have good auditing of the system.
                CreatedOn = DateTime.Now,
                UpdatedOn = DateTime.Now,
                UpdatedBy = "SYSTEM",
                CreatedBy = "SYSTEM",
                Items = cartDto.Items?.Select(i => i.ToEntity()).ToList() ?? new List<CartItem>()
            };
        }
    }

    public static class CartItemMapper
    {
        public static CartItemDto ToDto(this CartItem entity)
        {
            if (entity == null) return null!;

            return new CartItemDto
            {
                CartItemId = entity.CartItemId,
                CartId = entity.CartId,
                ProductId = entity.ProductId,
                Quantity = entity.Quantity,
                ProductName = entity.Product?.Name ?? string.Empty
            };
        }

        public static CartItem ToEntity(this CartItemDto dto)
        {
            if (dto == null) return null!;

            return new CartItem
            {
                CartItemId = dto.CartItemId,
                CartId = dto.CartId,
                ProductId = dto.ProductId,
                Quantity = dto.Quantity,
                CreatedBy = "System"
            };
        }
    }
}

