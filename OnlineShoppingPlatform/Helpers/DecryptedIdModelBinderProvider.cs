using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace OnlineShoppingPlatform.Helpers
{
    public class DecryptedIdModelBinderProvider : IModelBinderProvider
    {
        private readonly IEncryptionHelper _encryptionHelper;

        public DecryptedIdModelBinderProvider(IEncryptionHelper encryptionHelper)
        {
            _encryptionHelper = encryptionHelper;
        }

        public IModelBinder? GetBinder(ModelBinderProviderContext context)
        {
            if (context.Metadata.BindingSource == BindingSource.Custom &&
                context.Metadata.ModelType == typeof(int))
            {
                return new DecryptedIdModelBinder(_encryptionHelper);
            }

            return null;
        }
    }
}
