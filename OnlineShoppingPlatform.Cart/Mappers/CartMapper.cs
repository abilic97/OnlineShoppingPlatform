using OnlineShoppingPlatform.Carts.DTO;
using OnlineShoppingPlatform.Infrastructure.Entities;

namespace OnlineShoppingPlatform.Carts.Mappers
{
    public static class CartMapper
    {
        public static CartDto ToDto(this Cart cart, IEncryptionHelper encryption)
        {
            return new CartDto
            {
                CartId = encryption.Encrypt(cart.CartId.ToString()),
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

        public static Cart ToEntity(this CartDto cartDto, IEncryptionHelper encryption)
        {
            return new Cart
            {
                CartId = cartDto.CartId != null ? (int.Parse(encryption.Decrypt(cartDto.CartId))) : 0,
                UserId = cartDto.UserId,
                CartNumber = cartDto.CartNumber,
                Status = cartDto.Status,
                Subtotal = cartDto.Subtotal,
                ShippingCost = cartDto.ShippingCost,
                Total = cartDto.Total,
                ExpiresAt = cartDto.ExpiresAt,
                Notes = cartDto.Notes,
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
                ProductName = entity.Product.Name,
                Price = entity.Product.Price
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