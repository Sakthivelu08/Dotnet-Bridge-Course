using System;
using System.Security.Cryptography;
using System.Text;

namespace BridgeCourse.Week3.Vault
{
    public static class Day5Tasks
    {
        public static void RunRsaDemo()
        {
            Console.WriteLine("\n=== Day 5: Asymmetric Encryption (RSA) ===");

            string plaintext = "Small secret payload.";
            byte[] plaintextBytes = Encoding.UTF8.GetBytes(plaintext);

            using (RSA rsa = RSA.Create(2048))
            {
                byte[] ciphertext = rsa.Encrypt(plaintextBytes, RSAEncryptionPadding.OaepSHA256);
                Console.WriteLine($"Plaintext: {plaintext}");
                Console.WriteLine($"Encrypted (Hex): {Convert.ToHexString(ciphertext)}");

                byte[] decrypted = rsa.Decrypt(ciphertext, RSAEncryptionPadding.OaepSHA256);
                Console.WriteLine($"Decrypted: {Encoding.UTF8.GetString(decrypted)}");
            }
        }
    }
}
