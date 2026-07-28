using System;
using System.Collections.Generic;
using System.Linq;
using BridgeCourse.Week1.Lab.Day5;
using Xunit;

namespace BridgeCourse.Week1.Tests
{
    public class Day5Tests
    {
        #region Task 1.13 MathHelper Tests

        [Theory]
        [InlineData(0, 1)]
        [InlineData(1, 1)]
        [InlineData(2, 2)]
        [InlineData(5, 120)]
        [InlineData(10, 3628800)]
        public void MathHelper_Factorial_ReturnsExpectedValue(int input, long expected)
        {
            // Act
            long result = MathHelper.Factorial(input);

            // Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public void MathHelper_FactorialWithNegative_ThrowsArgumentOutOfRangeException()
        {
            // Act & Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => MathHelper.Factorial(-5));
        }

        [Theory]
        [InlineData(2, true)]
        [InlineData(3, true)]
        [InlineData(4, false)]
        [InlineData(29, true)]
        [InlineData(1, false)]
        [InlineData(0, false)]
        [InlineData(-7, false)]
        public void MathHelper_IsPrime_ReturnsExpectedValue(int input, bool expected)
        {
            // Act
            bool result = MathHelper.IsPrime(input);

            // Assert
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData(54, 24, 6)]
        [InlineData(10, 0, 10)]
        [InlineData(0, 5, 5)]
        [InlineData(-18, 12, 6)]
        [InlineData(17, 13, 1)]
        public void MathHelper_GCD_ReturnsExpectedValue(int a, int b, int expected)
        {
            // Act
            int result = MathHelper.GCD(a, b);

            // Assert
            Assert.Equal(expected, result);
        }

        #endregion

        #region Task 1.14 IComparable & IComparer Employee Sorting Tests

        [Fact]
        public void IComparable_SortsEmployeesBySalaryAscending()
        {
            // Arrange
            List<Employee> employees = EmployeeSorterDemo.GetSampleEmployees();

            // Act: default Sort uses IComparable CompareTo
            employees.Sort();

            // Assert
            decimal previousSalary = decimal.MinValue;
            foreach (var emp in employees)
            {
                Assert.True(emp.Salary >= previousSalary, $"List is not sorted by Salary ascending. {emp.Salary} came after {previousSalary}.");
                previousSalary = emp.Salary;
            }

            // Verify lowest and highest salaries match expected sample data
            Assert.Equal(42000m, employees.First().Salary);      // Bob
            Assert.Equal(120000m, employees.Last().Salary);    // Charlie
        }

        [Fact]
        public void IComparer_SortsEmployeesByNameAlphabetically()
        {
            // Arrange
            List<Employee> employees = EmployeeSorterDemo.GetSampleEmployees();

            // Act: Sort using Custom IComparer
            employees.Sort(new EmployeeNameComparer());

            // Assert
            string previousName = "";
            foreach (var emp in employees)
            {
                Assert.True(string.Compare(emp.Name, previousName, StringComparison.OrdinalIgnoreCase) >= 0, 
                    $"List is not sorted alphabetically. '{emp.Name}' came after '{previousName}'.");
                previousName = emp.Name;
            }

            // Verify first and last names in alphabetical order
            Assert.Equal("Alice", employees.First().Name);
            Assert.Equal("John", employees.Last().Name);
        }

        #endregion

        #region Scenario 1 & 2 & OrderProcessor Tests

        [Fact]
        public void NotificationChannels_Send_ExecutesWithoutException()
        {
            var email = new EmailChannel();
            var sms = new SmsChannel();

            email.Send("Test Email");
            sms.Send("Test SMS");
        }

        [Fact]
        public void SqlServerMigrator_RunsMigrationsSuccessfully()
        {
            var migrator = new SqlServerMigrator("Server=myServerAddress;Database=myDataBase;");
            migrator.RunMigrations();
        }

        [Fact]
        public void DatabaseMigrator_EmptyConnectionString_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => new SqlServerMigrator(""));
            Assert.Throws<ArgumentException>(() => new SqlServerMigrator(null!));
        }

        [Fact]
        public void OrderProcessor_NullChannel_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new OrderProcessor(null!));
        }

        [Fact]
        public void OrderProcessor_ProcessOrder_SendsNotification()
        {
            var email = new EmailChannel();
            var processor = new OrderProcessor(email);

            processor.ProcessOrder(101, 99.99m);
        }

        #endregion
    }
}
