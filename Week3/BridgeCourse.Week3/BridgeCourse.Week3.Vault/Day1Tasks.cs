using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace BridgeCourse.Week3.Vault
{
    public static class Day1Tasks
    {
        public static void RunFileOperationsDemo()
        {
            string sourcePath = "source_file.txt";
            string copyPath = "copied_file.txt";

            using (var ws = new FileStream(sourcePath, FileMode.Create, FileAccess.Write))
            using (var writer = new StreamWriter(ws, Encoding.UTF8))
            {
                writer.WriteLine("Line 1: Dotnet Bridge Course File I/O.");
                writer.WriteLine("Line 2: Understanding streaming data in chunks.");
            }

            using (var ws = new FileStream(sourcePath, FileMode.Append, FileAccess.Write))
            using (var writer = new StreamWriter(ws, Encoding.UTF8))
            {
                writer.WriteLine("Line 3: Appended data using FileAccess.Write.");
            }

            byte[] buffer = new byte[4096];
            using (var rs = new FileStream(sourcePath, FileMode.Open, FileAccess.Read))
            using (var ws = new FileStream(copyPath, FileMode.Create, FileAccess.Write))
            {
                int bytesRead;
                while ((bytesRead = rs.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ws.Write(buffer, 0, bytesRead);
                }
            }

            Console.WriteLine($"[Day 1] Chunked copy completed. Copy exists? {File.Exists(copyPath)}");
        }

        public static void RunEncodingVsHashingDemo(string plaintext)
        {
            byte[] textBytes = Encoding.UTF8.GetBytes(plaintext);
            string base64Encoded = Convert.ToBase64String(textBytes);
            
            byte[] decodedBytes = Convert.FromBase64String(base64Encoded);
            string decodedText = Encoding.UTF8.GetString(decodedBytes);

            byte[] hashBytes = SHA256.HashData(textBytes);
            string sha256Hex = Convert.ToHexString(hashBytes);

            Console.WriteLine($"\n--- Day 1: Encoding vs Hashing ---");
            Console.WriteLine($"Plaintext: {plaintext}");
            Console.WriteLine($"Base64 Encoded: {base64Encoded}");
            Console.WriteLine($"Base64 Decoded: {decodedText}");
            Console.WriteLine($"SHA-256 Hash: {sha256Hex}");
        }
    }
}
