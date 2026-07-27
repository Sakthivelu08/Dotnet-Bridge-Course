using System;

namespace BridgeCourse.Week1.Lab.Day1
{
    /// <summary>
    /// Custom exception representing an insufficient funds error in a withdrawal scenario.
    /// </summary>
    public class InsufficientFundsException : Exception
    {
        public decimal DeficitAmount { get; }

        public InsufficientFundsException(decimal deficitAmount)
            : base($"Insufficient funds. Deficit amount: {deficitAmount:C}")
        {
            DeficitAmount = deficitAmount;
        }

        public InsufficientFundsException(decimal deficitAmount, string message)
            : base(message)
        {
            DeficitAmount = deficitAmount;
        }

        public InsufficientFundsException(decimal deficitAmount, string message, Exception innerException)
            : base(message, innerException)
        {
            DeficitAmount = deficitAmount;
        }
    }

    /// <summary>
    /// A simple bank account class to demonstrate try/catch/finally with custom exception.
    /// </summary>
    public class BankAccount
    {
        public decimal Balance { get; private set; }

        public BankAccount(decimal initialBalance)
        {
            Balance = initialBalance;
        }

        /// <summary>
        /// Withdraws an amount from the account balance.
        /// Throws InsufficientFundsException if the balance is too low.
        /// </summary>
        public void Withdraw(decimal amount)
        {
            Console.WriteLine($"[Attempt] Withdrawing {amount:C} from balance {Balance:C}.");
            try
            {
                if (amount < 0)
                {
                    throw new ArgumentException("Withdrawal amount cannot be negative.", nameof(amount));
                }

                if (amount > Balance)
                {
                    decimal deficit = amount - Balance;
                    throw new InsufficientFundsException(deficit);
                }

                Balance -= amount;
                Console.WriteLine($"[Success] Withdrawal of {amount:C} completed. New Balance: {Balance:C}.");
            }
            catch (InsufficientFundsException ex)
            {
                Console.WriteLine($"[Caught Exception] InsufficientFundsException: {ex.Message}");
                throw;
            }
            finally
            {
                // finally block logs the attempt
                Console.WriteLine($"[Finally] Logged withdrawal attempt of {amount:C}.");
            }
        }
    }

    /// <summary>
    /// Class showing exception catching order and validation.
    /// </summary>
    public static class ExceptionOrderDemo
    {
        /// <summary>
        /// Demonstrates catching FormatException, OverflowException, then general Exception in the correct order.
        /// </summary>
        /// <param name="input">The input string to parse as an integer.</param>
        /// <returns>A status description of the parsing attempt.</returns>
        public static string ParseAndProcess(string? input)
        {
            try
            {
                // This line can throw:
                // - ArgumentNullException (handled by general Exception)
                // - FormatException (if input is not a number)
                // - OverflowException (if input exceeds Int32 limits)
                int value = int.Parse(input!);
                return $"Successfully parsed: {value}";
            }
            catch (FormatException ex)
            {
                Console.WriteLine($"[Catch FormatException] {ex.Message}");
                return "Error: FormatException";
            }
            catch (OverflowException ex)
            {
                Console.WriteLine($"[Catch OverflowException] {ex.Message}");
                return "Error: OverflowException";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Catch general Exception] {ex.Message}");
                return $"Error: general Exception ({ex.GetType().Name})";
            }

            /*
             * WHY REORDERING CAUSES A COMPILE ERROR:
             * --------------------------------------
             * If we place the catch (Exception ex) block first, like this:
             * 
             *      catch (Exception ex) { ... }
             *      catch (FormatException ex) { ... }
             *      catch (OverflowException ex) { ... }
             * 
             * It will cause a compile-time error CS0160: 
             * "A previous catch clause already catches all exceptions of this or of a super type."
             * 
             * This happens because catch clauses are evaluated from top to bottom. Since Exception 
             * is the base class for both FormatException and OverflowException, the first catch block
             * would intercept every exception. The subsequent blocks for FormatException and 
             * OverflowException would be completely unreachable. The C# compiler enforces exception
             * catching ordering from most specific to least specific to prevent unreachable code.
             */
        }
    }
}
