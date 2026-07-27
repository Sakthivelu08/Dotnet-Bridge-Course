using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace BridgeCourse.Week1.Lab.Day1
{
    /// <summary>
    /// UserDataFetcher simulates asynchronous data fetching with delays to show sequential vs concurrent execution times.
    /// </summary>
    public class UserDataFetcher
    {
        /// <summary>
        /// Simulates fetching user data from a remote service with a 3-second delay.
        /// Prints messages before and after the await.
        /// </summary>
        public async Task<string> FetchUserDataAsync(int userId)
        {
            Console.WriteLine($"[Fetcher - User {userId}] Before await: Starting fetch...");
            
            // Simulating a 3-second network latency / I/O work
            await Task.Delay(3000);
            
            Console.WriteLine($"[Fetcher - User {userId}] After await: Completed fetch.");
            return $"UserData_For_User_{userId}";
        }

        /// <summary>
        /// Runs three user fetches sequentially (one after another).
        /// </summary>
        public async Task<(string[] Data, long ElapsedMilliseconds)> RunSequentialFetchesAsync(List<int> userIds)
        {
            Console.WriteLine("\n--- Starting Sequential Fetches ---");
            Stopwatch stopwatch = Stopwatch.StartNew();
            
            var results = new List<string>();
            foreach (var id in userIds)
            {
                string data = await FetchUserDataAsync(id);
                results.Add(data);
            }
            
            stopwatch.Stop();
            Console.WriteLine($"--- Sequential Fetches Completed in {stopwatch.ElapsedMilliseconds} ms ---");
            return (results.ToArray(), stopwatch.ElapsedMilliseconds);
        }

        /// <summary>
        /// Runs three user fetches concurrently (at the same time) using Task.WhenAll().
        /// </summary>
        public async Task<(string[] Data, long ElapsedMilliseconds)> RunConcurrentFetchesAsync(List<int> userIds)
        {
            Console.WriteLine("\n--- Starting Concurrent Fetches (Task.WhenAll) ---");
            Stopwatch stopwatch = Stopwatch.StartNew();
            
            var tasks = new List<Task<string>>();
            foreach (var id in userIds)
            {
                // We call the async method but do NOT await it here.
                // This starts the asynchronous operation immediately.
                Task<string> fetchTask = FetchUserDataAsync(id);
                tasks.Add(fetchTask);
            }
            
            // We await all tasks in parallel using Task.WhenAll.
            // Execution resumes once all tasks are complete.
            string[] results = await Task.WhenAll(tasks);
            
            stopwatch.Stop();
            Console.WriteLine($"--- Concurrent Fetches Completed in {stopwatch.ElapsedMilliseconds} ms ---");
            return (results, stopwatch.ElapsedMilliseconds);
        }
    }
}
