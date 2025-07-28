using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace OnlineShoppingPlatform.Infrastructure.Helpers
{
    public class DecryptedIdModelBinder : IModelBinder
    {
        private readonly IEncryptionHelper _encryptionHelper;

        public DecryptedIdModelBinder(IEncryptionHelper encryptionHelper)
        {
            _encryptionHelper = encryptionHelper;
        }

        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            var modelName = bindingContext.ModelName;
            var valueProviderResult = bindingContext.ValueProvider.GetValue(modelName);

            if (valueProviderResult == ValueProviderResult.None)
            {
                bindingContext.Result = ModelBindingResult.Failed();
                return Task.CompletedTask;
            }

            string encryptedValue = valueProviderResult.FirstValue;

            if (string.IsNullOrWhiteSpace(encryptedValue))
            {
                bindingContext.Result = ModelBindingResult.Failed();
                return Task.CompletedTask;
            }

            try
            {
                string urlDecoded = System.Net.WebUtility.UrlDecode(encryptedValue);
                var decrypted = _encryptionHelper.Decrypt(urlDecoded);

                var modelType = bindingContext.ModelMetadata.ModelType;

                object result = Convert.ChangeType(decrypted, modelType);
                bindingContext.Result = ModelBindingResult.Success(result);
            }
            catch
            {
                bindingContext.ModelState.AddModelError(modelName, "Invalid encrypted ID.");
                bindingContext.Result = ModelBindingResult.Failed();
            }

            return Task.CompletedTask;
        }
    }
}