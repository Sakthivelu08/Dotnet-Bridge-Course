using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace BridgeCourse.Week3.Vault
{
    public static class Day2Tasks
    {
        public static void RunAesCbcDemo()
        {
            Console.WriteLine("\nDay 2: Symmetric Encryption (AES-CBC)");

            string plaintext = "The quick brown fox jumps over the lazy dog.";

            byte[] key = new byte[32];
            RandomNumberGenerator.Fill(key);

            byte[] iv1 = GenerateRandomBytes(16);
            byte[] iv2 = GenerateRandomBytes(16);

            byte[] cipher1 = EncryptCbc(plaintext, key, iv1);
            byte[] cipher2 = EncryptCbc(plaintext, key, iv2);

            Console.WriteLine($"Plaintext: {plaintext}");
            Console.WriteLine($"Ciphertext 1 (Hex): {Convert.ToHexString(cipher1)}");
            Console.WriteLine($"Ciphertext 2 (Hex): {Convert.ToHexString(cipher2)}");
            Console.WriteLine($"Proving IV Rule: Do ciphertexts differ? {Convert.ToHexString(cipher1) != Convert.ToHexString(cipher2)}");

            string decrypted1 = DecryptCbc(cipher1, key, iv1);
            Console.WriteLine($"Decrypted Content: {decrypted1}");

            Console.WriteLine("\n--- Task 3.6: Decrypting with Wrong Key ---");
            byte[] wrongKey = new byte[32];
            RandomNumberGenerator.Fill(wrongKey);

            try
            {
                DecryptCbc(cipher1, wrongKey, iv1);
            }
            catch (CryptographicException ex)
            {
                Console.WriteLine($"[Expected Error Caught] CryptographicException: {ex.Message}");
            }
        }

        private static byte[] EncryptCbc(string plainText, byte[] key, byte[] iv)
        {
            using (Aes aes = Aes.Create())
            {
                aes.Key = key;
                aes.IV = iv;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;

                using (var ms = new MemoryStream())
                {
                    using (var encryptor = aes.CreateEncryptor())
                    using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                    {
                        byte[] inputBytes = Encoding.UTF8.GetBytes(plainText);
                        cs.Write(inputBytes, 0, inputBytes.Length);
                        cs.FlushFinalBlock();
                    }
                    return ms.ToArray();
                }
            }
        }

        private static string DecryptCbc(byte[] cipherText, byte[] key, byte[] iv)
        {
            using (Aes aes = Aes.Create())
            {
                aes.Key = key;
                aes.IV = iv;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;

                using (var ms = new MemoryStream(cipherText))
                {
                    using (var decryptor = aes.CreateDecryptor())
                    using (var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                    using (var reader = new StreamReader(cs, Encoding.UTF8))
                    {
                        return reader.ReadToEnd();
                    }
                }
            }
        }

        private static byte[] GenerateRandomBytes(int size)
        {
            byte[] bytes = new byte[size];
            RandomNumberGenerator.Fill(bytes);
            return bytes;
        }
    }
}
