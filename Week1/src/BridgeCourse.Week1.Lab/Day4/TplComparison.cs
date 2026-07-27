using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BridgeCourse.Week1.Lab.Day2;

namespace BridgeCourse.Week1.Lab.Day4
{
    /// <summary>
    /// Performs benchmarks comparing raw Thread creation, Task.Run (ThreadPool), Parallel.ForEach, and Sequential execution
    /// over 100 simulated 100ms operations.
    /// </summary>
    public static class TplComparison
    {
        private const int TotalOperations = 100;
        private const int OperationDelayMs = 100;

        /// <summary>
        /// Benchmark: Spawning 100 raw OS Threads.
        /// </summary>
        public static long RunRawThreads()
        {
            var stopwatch = Stopwatch.StartNew();
            var threads = new List<Thread>();

            for (int i = 0; i < TotalOperations; i++)
            {
                var thread = new Thread(() =>
                {
                    // Simulating a blocking operation
                    Thread.Sleep(OperationDelayMs);
                });
                threads.Add(thread);
            }

            // Start all
            foreach (var thread in threads)
            {
                thread.Start();
            }

            // Join all
            foreach (var thread in threads)
            {
                thread.Join();
            }

            stopwatch.Stop();
            Logger.Instance.Log($"[TPL Benchmark] Raw OS Threads finished in {stopwatch.ElapsedMilliseconds} ms.");
            return stopwatch.ElapsedMilliseconds;
        }

        /// <summary>
        /// Benchmark: Queuing 100 tasks onto the ThreadPool via Task.Run().
        /// </summary>
        public static long RunThreadPoolTasks()
        {
            var stopwatch = Stopwatch.StartNew();
            var tasks = new List<Task>();

            for (int i = 0; i < TotalOperations; i++)
            {
                tasks.Add(Task.Run(() =>
                {
                    Thread.Sleep(OperationDelayMs);
                }));
            }

            Task.WaitAll(tasks.ToArray());
            stopwatch.Stop();
            Logger.Instance.Log($"[TPL Benchmark] Task.Run (ThreadPool) finished in {stopwatch.ElapsedMilliseconds} ms.");
            return stopwatch.ElapsedMilliseconds;
        }

        /// <summary>
        /// Benchmark: Processing 100 items using Parallel.ForEach.
        /// </summary>
        public static long RunParallelForEach()
        {
            var stopwatch = Stopwatch.StartNew();
            var items = Enumerable.Range(0, TotalOperations).ToList();

            // Parallel.ForEach automatically partitions the collections and manages thread coordination.
            Parallel.ForEach(items, item =>
            {
                Thread.Sleep(OperationDelayMs);
            });

            stopwatch.Stop();
            Logger.Instance.Log($"[TPL Benchmark] Parallel.ForEach finished in {stopwatch.ElapsedMilliseconds} ms.");
            return stopwatch.ElapsedMilliseconds;
        }

        /// <summary>
        /// Benchmark: Running 100 items sequentially (one after another).
        /// </summary>
        public static long RunSequential()
        {
            var stopwatch = Stopwatch.StartNew();

            for (int i = 0; i < TotalOperations; i++)
            {
                Thread.Sleep(OperationDelayMs);
            }

            stopwatch.Stop();
            Logger.Instance.Log($"[TPL Benchmark] Sequential loop finished in {stopwatch.ElapsedMilliseconds} ms.");
            return stopwatch.ElapsedMilliseconds;
        }
    }
}
