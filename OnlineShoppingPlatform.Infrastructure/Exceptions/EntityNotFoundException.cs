namespace OnlineShoppingPlatform.Infrastructure.Exceptions
{
    public class EntityNotFoundException : Exception
    {
        public EntityNotFoundException(string entity, object id)
            : base($"{entity} with ID '{id}' was not found.") { }
    }

}
