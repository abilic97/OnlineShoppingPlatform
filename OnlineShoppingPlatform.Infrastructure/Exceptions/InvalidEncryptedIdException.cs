namespace OnlineShoppingPlatform.Infrastructure.Exceptions
{
    public class InvalidEncryptedIdException : Exception
    {
        public InvalidEncryptedIdException(string message) : base(message) { }
    }
}
