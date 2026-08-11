using System;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace BridgeCourse.Week3.Vault
{
    public static class Day4Tasks
    {
        public static void RunHashAndHmacDemo()
        {
            Console.WriteLine("\n=== Day 4: Hashing & HMAC ===");

            string tempFile = "hash_demo.txt";
            File.WriteAllText(tempFile, "Data to check integrity.");

            byte[] fileBytes = File.ReadAllBytes(tempFile);
            byte[] sha256 = SHA256.HashData(fileBytes);
            byte[] sha512 = SHA512.HashData(fileBytes);

            Console.WriteLine($"SHA-256: {Convert.ToHexString(sha256)}");
            Console.WriteLine($"SHA-512: {Convert.ToHexString(sha512)}");

            byte[] hmacKey = new byte[32];
            RandomNumberGenerator.Fill(hmacKey);

            using (var hmac = new HMACSHA256(hmacKey))
            {
                byte[] mac = hmac.ComputeHash(fileBytes);
                Console.WriteLine($"HMAC-SHA256: {Convert.ToHexString(mac)}");
            }

            if (File.Exists(tempFile))
            {
                File.Delete(tempFile);
            }
        }

        public static void RunConstantTimeCompareDemo()
        {
            Console.WriteLine("\n--- Task 3.11: Constant Time Comparison ---");

            byte[] digestA = Encoding.UTF8.GetBytes("HashSignatureA12345");
            byte[] digestB = Encoding.UTF8.GetBytes("HashSignatureA12345");
            byte[] digestC = Encoding.UTF8.GetBytes("HashSignatureDifferent");

            bool matchAB = CryptographicOperations.FixedTimeEquals(digestA, digestB);
            bool matchAC = CryptographicOperations.FixedTimeEquals(digestA, digestC);

            Console.WriteLine($"Digest A == Digest B: {matchAB}");
            Console.WriteLine($"Digest A == Digest C: {matchAC}");
        }

        public static void RunLargeFileStreamingCbcDemo()
        {
            Console.WriteLine("\n--- Task 3.12: Streaming Large File (100MB+) via CBC ---");

            string largeSource = "large_source.dat";
            string largeEncrypted = "large_encrypted.dat";
            string largeDecrypted = "large_decrypted.dat";

            Console.WriteLine("Generating 100MB file...");
            GenerateLargeFile(largeSource, 100);

            byte[] key = new byte[32];
            byte[] iv = new byte[16];
            RandomNumberGenerator.Fill(key);
            RandomNumberGenerator.Fill(iv);

            var sw = Stopwatch.StartNew();
            EncryptFileCbc(largeSource, largeEncrypted, key, iv);
            sw.Stop();
            Console.WriteLine($"Encryption took: {sw.ElapsedMilliseconds} ms");

            sw.Restart();
            DecryptFileCbc(largeEncrypted, largeDecrypted, key, iv);
            sw.Stop();
            Console.WriteLine($"Decryption took: {sw.ElapsedMilliseconds} ms");

            bool identical = CompareFiles(largeSource, largeDecrypted);
            Console.WriteLine($"Result: Decrypted file is byte-identical? {identical}");

            CleanUpFiles(largeSource, largeEncrypted, largeDecrypted);
        }

        public static void EncryptFileGcm(string inputPath, string outputPath, byte[] key)
        {
            byte[] buffer = new byte[64 * 1024]; // 64 KB chunks
            using (var fsIn = new FileStream(inputPath, FileMode.Open, FileAccess.Read))
            using (var fsOut = new FileStream(outputPath, FileMode.Create, FileAccess.Write))
            using (var aesGcm = new AesGcm(key, 16))
            {
                int bytesRead;
                byte[] nonce = new byte[12];
                byte[] tag = new byte[16];

                while ((bytesRead = fsIn.Read(buffer, 0, buffer.Length)) > 0)
                {
                    RandomNumberGenerator.Fill(nonce);
                    byte[] chunk = new byte[bytesRead];
                    Array.Copy(buffer, 0, chunk, 0, bytesRead);
                    byte[] ciphertext = new byte[bytesRead];

                    aesGcm.Encrypt(nonce, chunk, ciphertext, tag);

                    fsOut.Write(BitConverter.GetBytes(bytesRead), 0, 4);
                    fsOut.Write(nonce, 0, 12);
                    fsOut.Write(tag, 0, 16);
                    fsOut.Write(ciphertext, 0, bytesRead);
                }
            }
        }

        public static void DecryptFileGcm(string inputPath, string outputPath, byte[] key)
        {
            using (var fsIn = new FileStream(inputPath, FileMode.Open, FileAccess.Read))
            using (var fsOut = new FileStream(outputPath, FileMode.Create, FileAccess.Write))
            using (var aesGcm = new AesGcm(key, 16))
            {
                byte[] lengthBuffer = new byte[4];
                byte[] nonce = new byte[12];
                byte[] tag = new byte[16];

                while (fsIn.Read(lengthBuffer, 0, 4) == 4)
                {
                    int chunkSize = BitConverter.ToInt32(lengthBuffer, 0);
                    byte[] ciphertext = new byte[chunkSize];

                    if (fsIn.Read(nonce, 0, 12) != 12 ||
                        fsIn.Read(tag, 0, 16) != 16 ||
                        fsIn.Read(ciphertext, 0, chunkSize) != chunkSize)
                    {
                        throw new CryptographicException("Malformed GCM encrypted file.");
                    }

                    byte[] plaintext = new byte[chunkSize];
                    aesGcm.Decrypt(nonce, ciphertext, tag, plaintext);

                    fsOut.Write(plaintext, 0, chunkSize);
                }
            }
        }

        private static void EncryptFileCbc(string inputPath, string outputPath, byte[] key, byte[] iv)
        {
            using (Aes aes = Aes.Create())
            {
                aes.Key = key;
                aes.IV = iv;
                aes.Mode = CipherMode.CBC;

                using (var fsIn = new FileStream(inputPath, FileMode.Open, FileAccess.Read))
                using (var fsOut = new FileStream(outputPath, FileMode.Create, FileAccess.Write))
                using (var encryptor = aes.CreateEncryptor())
                using (var cs = new CryptoStream(fsOut, encryptor, CryptoStreamMode.Write))
                {
                    byte[] buffer = new byte[4096];
                    int bytesRead;
                    while ((bytesRead = fsIn.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        cs.Write(buffer, 0, bytesRead);
                    }
                }
            }
        }

        private static void DecryptFileCbc(string inputPath, string outputPath, byte[] key, byte[] iv)
        {
            using (Aes aes = Aes.Create())
            {
                aes.Key = key;
                aes.IV = iv;
                aes.Mode = CipherMode.CBC;

                using (var fsIn = new FileStream(inputPath, FileMode.Open, FileAccess.Read))
                using (var fsOut = new FileStream(outputPath, FileMode.Create, FileAccess.Write))
                using (var decryptor = aes.CreateDecryptor())
                using (var cs = new CryptoStream(fsIn, decryptor, CryptoStreamMode.Read))
                {
                    byte[] buffer = new byte[4096];
                    int bytesRead;
                    while ((bytesRead = cs.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        fsOut.Write(buffer, 0, bytesRead);
                    }
                }
            }
        }

        public static void GenerateLargeFile(string path, int sizeInMb)
        {
            byte[] data = new byte[1024 * 1024]; // 1 MB buffer
            RandomNumberGenerator.Fill(data);

            using (var fs = new FileStream(path, FileMode.Create, FileAccess.Write))
            {
                for (int i = 0; i < sizeInMb; i++)
                {
                    fs.Write(data, 0, data.Length);
                }
            }
        }

        public static bool CompareFiles(string path1, string path2)
        {
            using (var fs1 = new FileStream(path1, FileMode.Open, FileAccess.Read))
            using (var fs2 = new FileStream(path2, FileMode.Open, FileAccess.Read))
            {
                if (fs1.Length != fs2.Length) return false;

                int b1, b2;
                while ((b1 = fs1.ReadByte()) != -1)
                {
                    b2 = fs2.ReadByte();
                    if (b1 != b2) return false;
                }
                return true;
            }
        }

        public static void CleanUpFiles(params string[] paths)
        {
            foreach (var path in paths)
            {
                if (File.Exists(path))
                {
                    File.Delete(path);
                }
            }
        }
    }
}
