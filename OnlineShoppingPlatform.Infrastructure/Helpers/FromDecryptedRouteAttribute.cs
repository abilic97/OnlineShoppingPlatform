using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace OnlineShoppingPlatform.Infrastructure.Helpers
{
    [AttributeUsage(AttributeTargets.Parameter)]
    public class FromDecryptedRouteAttribute : Attribute, IBindingSourceMetadata
    {
        public BindingSource BindingSource => BindingSource.Custom;
    }
}