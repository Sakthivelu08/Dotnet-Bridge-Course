using System;

namespace BridgeCourse.Week1.Lab.Day5
{
    /*
     * COMPARISON: INTERFACE vs. ABSTRACT CLASS
     * ----------------------------------------
     * 
     * 1. Inheritance:
     *    - Interface: A class can implement multiple interfaces (multiple inheritance).
     *    - Abstract Class: A class can inherit from only one abstract class (single inheritance).
     * 
     * 2. State:
     *    - Interface: Cannot hold instance fields or state. All fields in an interface are implicitly static and readonly.
     *    - Abstract Class: Can declare instance fields, state, and properties (e.g., protected string connectionString).
     * 
     * 3. Constructors:
     *    - Interface: Cannot have constructors.
     *    - Abstract Class: Can define constructors to enforce initialization patterns in sub-classes.
     * 
     * 4. Versioning:
     *    - Interface: Historically, adding a method to an interface broke all implementing classes. 
     *      Modern C# supports default implementations, but it is still fundamentally a strict contract.
     *    - Abstract Class: Adding a new virtual or non-abstract method is non-breaking. Subclasses automatically inherit it.
     * 
     * ----------------------------------------------------
     * Scenario favoring Interface:
     *   When defining common behaviors (capabilities) across completely unrelated classes. 
     *   For example, ISerializable, IComparable, or INotificationChannel. 
     *   Both an EmailService and an industrial SmartLight might notify, but they share no hierarchy.
     * 
     * Scenario favoring Abstract Class:
     *   When creating a base template for a family of closely related classes that shares state 
     *   and default behavior. For example, a BaseReportGenerator that stores database credentials, 
     *   enforces a specific constructor sequence, and defines template logging methods.
     */

    #region Scenario 1: Interface

    public interface INotificationChannel
    {
        void Send(string message);
    }

    public class EmailChannel : INotificationChannel
    {
        public void Send(string message) => Console.WriteLine($"[EmailChannel] Sending email: {message}");
    }

    public class SmsChannel : INotificationChannel
    {
        public void Send(string message) => Console.WriteLine($"[SmsChannel] Sending SMS: {message}");
    }

    #endregion

    #region Scenario 2: Abstract Class

    public abstract class DatabaseMigrator
    {
        // Holds state (connection string)
        protected string ConnectionString { get; }

        // Defines constructor to enforce initialization
        protected DatabaseMigrator(string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new ArgumentException("Connection string cannot be empty.", nameof(connectionString));
            }
            ConnectionString = connectionString;
        }

        // Shared concrete method (saves duplicating code)
        public void LogMigration(string step)
        {
            Console.WriteLine($"[DatabaseMigrator] [{DateTime.UtcNow:s}] Step: {step}");
        }

        // Abstract method to be overridden by subclasses
        public abstract void RunMigrations();
    }

    public class SqlServerMigrator : DatabaseMigrator
    {
        public SqlServerMigrator(string connectionString) : base(connectionString) { }

        public override void RunMigrations()
        {
            LogMigration("Starting MS SQL migrations...");
            // Execute SQL Server specific DDL script
            LogMigration("Completed MS SQL migrations.");
        }
    }

    #endregion

    #region Static vs Instance

    /// <summary>
    /// MathHelper uses STATIC methods.
    /// RATIONALE: It represents pure utility math functions. It maintains no state, requires no
    /// external dependencies, does not implement interfaces, and behaves identically on every execution
    /// based solely on input arguments. Forcing clients to instantiate MathHelper (new MathHelper())
    /// would add needless memory allocations and syntactic noise.
    /// </summary>
    public static class MathHelper
    {
        public static long Factorial(int n)
        {
            if (n < 0) throw new ArgumentOutOfRangeException(nameof(n), "Factorial is not defined for negative numbers.");
            if (n == 0 || n == 1) return 1;

            long result = 1;
            for (int i = 2; i <= n; i++)
            {
                result *= i;
            }
            return result;
        }

        public static bool IsPrime(int n)
        {
            if (n <= 1) return false;
            if (n == 2) return true;
            if (n % 2 == 0) return false;

            var limit = (int)Math.Floor(Math.Sqrt(n));
            for (int i = 3; i <= limit; i += 2)
            {
                if (n % i == 0) return false;
            }
            return true;
        }

        public static int GCD(int a, int b)
        {
            a = Math.Abs(a);
            b = Math.Abs(b);

            while (b != 0)
            {
                int temp = b;
                b = a % b;
                a = temp;
            }
            return a;
        }
    }

    /// <summary>
    /// OrderProcessor uses INSTANCE methods.
    /// RATIONALE: Unlike pure utility functions, OrderProcessor manages order states and relies
    /// on external service dependencies (like INotificationChannel). It relies on Dependency Injection
    /// to obtain notification and data services at runtime, allowing the codebase to remain loosely coupled,
    /// highly testable, and support mock-based unit tests.
    /// </summary>
    public class OrderProcessor
    {
        private readonly INotificationChannel _notificationChannel;

        // Constructor injection of dependencies
        public OrderProcessor(INotificationChannel notificationChannel)
        {
            _notificationChannel = notificationChannel ?? throw new ArgumentNullException(nameof(notificationChannel));
        }

        public void ProcessOrder(int orderId, decimal amount)
        {
            Console.WriteLine($"[OrderProcessor] Processing Order ID: {orderId} for {amount:C}");
            // Process core business rules here...

            // Send completion message via injected dependency
            _notificationChannel.Send($"Order #{orderId} processed successfully.");
        }
    }

    #endregion
}
