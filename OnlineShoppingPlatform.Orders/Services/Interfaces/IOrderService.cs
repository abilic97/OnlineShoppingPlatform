namespace OnlineShoppingPlatform.Orders.Services.Interfaces
{
    public interface IOrderService
    {
        Task<int> PlaceOrderAsync(CartDto cartDto);
    }
}
