namespace OnlineShoppingPlatform.Exceptions
{
    public class CartNotFoundException : Exception
    {
        public CartNotFoundException(int cartId)
            : base($"Cart with ID {cartId} was not found.") { }

        public CartNotFoundException(string userId)
            : base($"Cart for user '{userId}' was not found.") { }
    }
}
