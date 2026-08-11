using System;
using System.Security.Cryptography;
using System.Text;

namespace BridgeCourse.Week3.Vault
{
    public static class Day3Tasks
    {
        public static void RunAesGcmDemo()
        {
            Console.WriteLine("\n=== Day 3: PBKDF2 & AES-GCM ===");

            string password = "StrongUserPassword123!";
            string plaintext = "Confidential student grade records.";

            byte[] salt = new byte[16];
            RandomNumberGenerator.Fill(salt);

            byte[] derivedKey = Rfc2898DeriveBytes.Pbkdf2(
                password,
                salt,
                100000,
                HashAlgorithmName.SHA256,
                32
            );

            Console.WriteLine($"Password: {password}");
            Console.WriteLine($"Derived Key (Hex): {Convert.ToHexString(derivedKey)}");

            byte[] nonce = new byte[12];
            RandomNumberGenerator.Fill(nonce);
            byte[] tag = new byte[16];

            byte[] plaintextBytes = Encoding.UTF8.GetBytes(plaintext);
            byte[] ciphertext = new byte[plaintextBytes.Length];

            using (var aesGcm = new AesGcm(derivedKey, 16))
            {
                aesGcm.Encrypt(nonce, plaintextBytes, ciphertext, tag);
            }

            Console.WriteLine($"Ciphertext (Hex): {Convert.ToHexString(ciphertext)}");
            Console.WriteLine($"Tag (Hex): {Convert.ToHexString(tag)}");

            byte[] decryptedBytes = new byte[ciphertext.Length];
            using (var aesGcm = new AesGcm(derivedKey, 16))
            {
                aesGcm.Decrypt(nonce, ciphertext, tag, decryptedBytes);
            }
            Console.WriteLine($"Decrypted: {Encoding.UTF8.GetString(decryptedBytes)}");

            Console.WriteLine("\n--- Testing GCM Rejection ---");
            byte[] tampered = (byte[])ciphertext.Clone();
            tampered[0] ^= 0x01;

            try
            {
                byte[] failBuffer = new byte[tampered.Length];
                using (var aesGcm = new AesGcm(derivedKey, 16))
                {
                    aesGcm.Decrypt(nonce, tampered, tag, failBuffer);
                }
            }
            catch (CryptographicException ex)
            {
                Console.WriteLine($"[Expected Catch] CryptographicException: {ex.Message}");
            }
        }
    }
}
