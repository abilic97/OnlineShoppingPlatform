using Microsoft.AspNetCore.Mvc;
using OnlineShoppingPlatform.Data.Entities;
using OnlineShoppingPlatform.Domain.DTO;
using OnlineShoppingPlatform.Services.Interfaces;

namespace OnlineShoppingPlatform.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;
        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Cart>> GetCart(int id)
        {
            var cart = await _cartService.GetByIdAsync(id);
            if (cart == null) return NotFound();

            return Ok(cart);
        }

        [HttpPost]
        public async Task<ActionResult<Cart>> CreateCart(CartDto newCart)
        {
            var cart = await _cartService.CreateAsync(newCart);
            return CreatedAtAction(nameof(GetCart), new { id = cart.CartId }, cart);
        }

        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateCartStatus(int id, [FromBody] string newStatus)
        {
            var cart = await _cartService.UpdateStatusAsync(id, newStatus);
            if (cart == null) return NotFound();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCart(int id)
        {
            var success = await _cartService.DeleteAsync(id);
            if (!success) return NotFound();

            return NoContent();
        }

        // Example endpoint to recalc totals
        [HttpPost("{id}/recalculate")]
        public async Task<ActionResult<Cart>> RecalculateTotals(int id)
        {
            var cart = await _cartService.RecalculateTotalsAsync(id);
            if (cart == null) return NotFound();

            return Ok(cart);
        }
    }
}
