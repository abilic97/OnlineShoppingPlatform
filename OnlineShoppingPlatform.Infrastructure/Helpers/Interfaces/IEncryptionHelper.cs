public interface IEncryptionHelper
{
    string Encrypt(string plainText);
    string Decrypt(string encryptedText);
    bool TryDecrypt(string encryptedText, out string? plainText);
}
