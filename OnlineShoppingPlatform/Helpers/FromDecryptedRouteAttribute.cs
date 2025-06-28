using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace OnlineShoppingPlatform.Helpers
{
    [AttributeUsage(AttributeTargets.Parameter)]
    public class FromDecryptedRouteAttribute : Attribute, IBindingSourceMetadata
    {
        public BindingSource BindingSource => BindingSource.Custom;
    }
}
