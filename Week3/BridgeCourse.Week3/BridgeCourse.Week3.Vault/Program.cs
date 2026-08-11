using System;

namespace BridgeCourse.Week3.Vault
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=================================================");
            Console.WriteLine("     Dotnet Bridge Course - Week 3: Vault        ");
            Console.WriteLine("=================================================");

            Console.WriteLine("\nExecuting Day 1 Tasks...");
            Day1Tasks.RunFileOperationsDemo();
            Day1Tasks.RunEncodingVsHashingDemo("SecurePassword123!");

            Console.WriteLine("\nExecuting Day 2 Tasks...");
            Day2Tasks.RunAesCbcDemo();

            Console.WriteLine("\nExecuting Day 3 Tasks...");
            Day3Tasks.RunAesGcmDemo();

            Console.WriteLine("\nExecuting Day 4 Tasks...");
            Day4Tasks.RunHashAndHmacDemo();
            Day4Tasks.RunConstantTimeCompareDemo();
            Day4Tasks.RunLargeFileStreamingCbcDemo();

            Console.WriteLine("\nExecuting Day 5 Tasks...");
            Day5Tasks.RunRsaDemo();

            Console.WriteLine("\nAll Vault execution tasks completed successfully.");
        }
    }
}
