using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using System.Text;

namespace Crypto.Utils;

public static class EncryptionHelper
{
    public static string DoEncryptAES(string plainText, string key)
    {
        ValidateData(plainText, key);
        return Convert.ToBase64String(Encrypt(plainText, key));
    }

    public static string DoDecryptAES(string encryptedText, string key)
    {
        ValidateData(encryptedText, key);
        return Encoding.UTF8.GetString(Decrypt(encryptedText, key));
    }

    private static byte[] Encrypt(string data, string key)
    {
        byte[] inputBytes = Encoding.UTF8.GetBytes(data);
        byte[] keyBytes = Encoding.UTF8.GetBytes(key);

        Random random = new();
        var ivArray = new byte[16];
        random.NextBytes(ivArray);

        IBufferedCipher cipher = CipherUtilities.GetCipher("AES/CTR/NoPadding");
        cipher.Init(true, new ParametersWithIV(ParameterUtilities.CreateKeyParameter("AES", keyBytes), ivArray, 0, 16));

        byte[] encryptedBytes = cipher.DoFinal(inputBytes);
        byte[] finalByte = new byte[ivArray.Length + encryptedBytes.Length];
        Array.Copy(ivArray, 0, finalByte, 0, 16);
        Array.Copy(encryptedBytes, 0, finalByte, ivArray.Length, encryptedBytes.Length);

        return finalByte;
    }

    private static byte[] Decrypt(string data, string key)
    {
        byte[] inputBytes = Convert.FromBase64String(data);
        byte[] keyBytes = Encoding.UTF8.GetBytes(key);
        byte[] ivBytes = inputBytes.Take(16).ToArray();
        byte[] textBytes = inputBytes.Skip(ivBytes.Length).ToArray();

        IBufferedCipher cipher = CipherUtilities.GetCipher("AES/CTR/NoPadding");
        cipher.Init(true, new ParametersWithIV(ParameterUtilities.CreateKeyParameter("AES", keyBytes), ivBytes, 0, 16));

        byte[] encryptedBytes = cipher.DoFinal(textBytes);

        return encryptedBytes;
    }

    private static void ValidateData(string data, string key)
    {
        if (string.IsNullOrEmpty(data))
            throw new ArgumentException("Input cannot be null or empty.");

        if (key.Length != 16 && key.Length != 24 && key.Length != 32)
            throw new ArgumentException("SecretKey must be 16, 24, or 32 characters.");
    }
}
