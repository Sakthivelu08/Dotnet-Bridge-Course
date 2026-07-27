using System;

namespace BridgeCourse.Week1.Lab.Day2
{
    /// <summary>
    /// Thread-safe Singleton Logger using Lazy<T> to guarantee double-checked locking behavior internally.
    /// </summary>
    public sealed class Logger
    {
        // Lazy<T> ensures thread-safe, lazy initialization by default.
        // It guarantees that only a single instance of Logger is created, even if accessed from multiple threads.
        private static readonly Lazy<Logger> _lazyInstance = new Lazy<Logger>(() => new Logger());

        /// <summary>
        /// Public access point to the singleton instance.
        /// </summary>
        public static Logger Instance => _lazyInstance.Value;

        /// <summary>
        /// Private constructor prevents external instantiation.
        /// </summary>
        private Logger()
        {
            Console.WriteLine($"[Logger Constructor] Instance created. HashCode: {GetHashCode()}");
        }

        /// <summary>
        /// Logs a message to the console along with the instance hashcode and thread details.
        /// </summary>
        public void Log(string message)
        {
            int threadId = Environment.CurrentManagedThreadId;
            int? taskId = System.Threading.Tasks.Task.CurrentId;
            string context = taskId.HasValue ? $"Task {taskId.Value}" : $"Thread {threadId}";
            
            Console.WriteLine($"[LOG] [{context}] [Logger HashCode: {GetHashCode()}] - {message}");
        }
    }
}
