using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineShoppingPlatform.Domain.DTO;
using OnlineShoppingPlatform.Helpers;
using OnlineShoppingPlatform.Services.Interfaces;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Security.Claims;

namespace OnlineShoppingPlatform.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;
        private readonly IEncryptionHelper _encryptionHelper;

        public CartController(ICartService cartService, IEncryptionHelper encryptionHelper)
        {
            _cartService = cartService;
            _encryptionHelper = encryptionHelper;
        }

        [HttpGet("user")]
        public async Task<ActionResult<CartDto>> GetUserCart()
        {
            var userId = GetUserId();
            if (userId == null) return Unauthorized();

            var cart = await _cartService.GetByUserIdAsync(userId);
            if (cart == null)
            {
                cart = await _cartService.CreateAsync(new CartDto
                {
                    UserId = userId,
                    CartNumber = Guid.NewGuid().ToString(),
                    ExpiresAt = DateTime.UtcNow.AddMinutes(30)
                });
            }
            return Ok(cart);
        }

        [HttpGet("count")]
        public async Task<ActionResult<int>> GetUserCartNumberOfItems()
        {
            var userId = GetUserId();
            if (userId == null) return Unauthorized();
            var cart = await _cartService.GetByUserIdAsync(userId);
            var totalItems = cart?.Items.Count() ?? 0;

            return Ok(totalItems);
        }

        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateCartStatus([FromDecryptedRoute] int id, [FromBody][Required] string newStatus)
        {
            var cart = await _cartService.UpdateStatusAsync(id, newStatus);
            if (cart == null) return NotFound();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCart([FromDecryptedRoute] int id)
        {
            var success = await _cartService.DeleteAsync(id);
            if (!success) return NotFound();
            return NoContent();
        }

        [HttpPost("{id}/recalculate")]
        public async Task<ActionResult<CartDto>> RecalculateTotals([FromDecryptedRoute] int id)
        {
            var cart = await _cartService.RecalculateTotalsAsync(id);
            if (cart == null) return NotFound();
            return Ok(cart);
        }

        [HttpPost("{id}/items")]
        public async Task<ActionResult<CartDto>> AddItemToCart([FromDecryptedRoute] int id, [FromBody][Required] CartItemDto newItem)
        {
            var cart = await _cartService.AddItemAsync(id, newItem);
            if (cart == null) return NotFound();
            return Ok(cart);
        }

        [HttpDelete("{id}/items/{cartItemId}")]
        public async Task<ActionResult<CartDto>> RemoveItemFromCart([FromDecryptedRoute] int id, int cartItemId)
        {
            var cart = await _cartService.RemoveItemAsync(id, cartItemId);
            if (cart == null) return NotFound();
            return Ok(cart);
        }

        private string? GetUserId() => User.FindFirstValue(ClaimTypes.NameIdentifier);
    }
}
