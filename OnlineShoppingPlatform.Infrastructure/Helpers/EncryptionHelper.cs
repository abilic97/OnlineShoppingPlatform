using System.Security.Cryptography;
using Microsoft.Extensions.Configuration;

public class EncryptionHelper : IEncryptionHelper
{
    private readonly byte[] _key;
    private readonly byte[] _iv;

    public EncryptionHelper(IConfiguration configuration)
    {
        var keyString = configuration["EncryptionSettings:Key"];
        var ivString = configuration["EncryptionSettings:IV"];

        if (string.IsNullOrWhiteSpace(keyString) || string.IsNullOrWhiteSpace(ivString))
            throw new ArgumentException("Missing encryption configuration.");

        _key = Convert.FromBase64String(keyString);
        _iv = Convert.FromBase64String(ivString);
    }

    public string Encrypt(string plainText)
    {
        using var aes = Aes.Create();
        aes.Key = _key;
        aes.IV = _iv;

        var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
        using var ms = new MemoryStream();
        using var cryptoStream = new CryptoStream(ms, encryptor, CryptoStreamMode.Write);
        using var sw = new StreamWriter(cryptoStream);

        sw.Write(plainText);
        sw.Close();

        var base64 = Convert.ToBase64String(ms.ToArray());

        // URL-safe: replace '+' with '-', '/' with '_', trim '=' padding
        return base64.Replace("+", "-").Replace("/", "_").TrimEnd('=');
    }

    public string Decrypt(string encryptedText)
    {
        // Revert URL-safe characters to original Base64 characters
        string base64 = encryptedText.Replace("-", "+").Replace("_", "/");

        // Restore missing padding if necessary
        int padding = 4 - (base64.Length % 4);
        if (padding < 4)
        {
            base64 = base64.PadRight(base64.Length + padding, '=');
        }

        var buffer = Convert.FromBase64String(base64);

        using var aes = Aes.Create();
        aes.Key = _key;
        aes.IV = _iv;

        var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
        using var ms = new MemoryStream(buffer);
        using var cryptoStream = new CryptoStream(ms, decryptor, CryptoStreamMode.Read);
        using var sr = new StreamReader(cryptoStream);

        return sr.ReadToEnd();
    }
}
