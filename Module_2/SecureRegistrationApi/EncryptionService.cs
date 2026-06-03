using System.Security.Cryptography;
using System.Text;

public class EncryptionService
{
    private readonly byte[] _key;

    public EncryptionService(IConfiguration configuration)
    {
        string? keyHex = configuration["EncryptionKey"];
        if (string.IsNullOrEmpty(keyHex))
            throw new Exception("EncryptionKey is not configured.");

        _key = HexStringToByteArray(keyHex) ?? throw new Exception("Failed to convert EncryptionKey to byte array.");
        if (_key.Length != 32)
            throw new Exception("EncryptionKey must be 32 bytes (64 hex characters).");
    }

    public string Encrypt(string plainText)
    {
        using Aes aes = Aes.Create();
        aes.Key = _key;
        aes.Mode = CipherMode.CBC;
        aes.Padding = PaddingMode.PKCS7;
        
        // Generate a random IV.
        aes.GenerateIV();
        byte[] iv = aes.IV;

        using var encryptor = aes.CreateEncryptor(aes.Key, iv);
        byte[] plainBytes = Encoding.UTF8.GetBytes(plainText);
        byte[] encryptedBytes = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);

        return $"{ByteArrayToHex(iv)}:{ByteArrayToHex(encryptedBytes)}";
    }

    private byte[] HexStringToByteArray(string hex)
    {
        int numberChars = hex.Length;
        byte[] bytes = new byte[numberChars / 2];
        for (int i = 0; i < numberChars; i += 2)
            bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
        return bytes;
    }

    // Helper: Convert a byte array to a hex string.
    private string ByteArrayToHex(byte[] bytes)
    {
        StringBuilder hex = new StringBuilder(bytes.Length * 2);
        foreach (var b in bytes)
            hex.AppendFormat("{0:x2}", b);
        return hex.ToString();
    }
}