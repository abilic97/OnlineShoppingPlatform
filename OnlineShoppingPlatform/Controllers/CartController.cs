using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineShoppingPlatform.Carts.DTO;
using OnlineShoppingPlatform.Carts.Services.Interfaces;
using OnlineShoppingPlatform.Infrastructure.Helpers;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace OnlineShoppingPlatform.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;

        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        [HttpGet("user")]
        public async Task<ActionResult<CartDto>> GetorCreateUserCart()
        {
            var userId = GetUserId();
            if (userId == null) return Unauthorized();

            var cart = await _cartService.GetOrCreateUserCartAsync(userId);
            return Ok(cart);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCart([FromDecryptedRoute] int id)
        {
            var success = await _cartService.DeleteAsync(id);
            if (!success) return NotFound();
            return NoContent();
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
