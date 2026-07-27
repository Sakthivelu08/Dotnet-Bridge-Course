using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BridgeCourse.Week1.Lab.Day4;
using Xunit;

namespace BridgeCourse.Week1.Tests
{
    public class Day4Tests
    {
        #region Task 1.10 TPL Tests

        [Fact]
        public void TplComparison_ParallelRuns_ExecuteFasterThanSequential()
        {
            // Act
            long parallelTime = TplComparison.RunParallelForEach();
            long taskTime = TplComparison.RunThreadPoolTasks();

            // Assert: Confirm parallel execution runs concurrently.
            // Spawning 100 operations of 100ms in parallel should take way less than 5000ms, 
            // whereas sequential takes at least 10,000ms.
            Assert.True(parallelTime < 5000, $"Parallel.ForEach took too long: {parallelTime} ms (Expected < 5000ms)");
            Assert.True(taskTime < 5000, $"Task.Run took too long: {taskTime} ms (Expected < 5000ms)");
        }

        #endregion

        #region Task 1.11 Reflection Tests

        [Fact]
        public void ReflectAndInspect_ReturnsCorrectMetadataStructure()
        {
            // Act
            string metadata = InvoiceReflector.ReflectAndInspect();

            // Assert
            Assert.Contains("Class Name: BridgeCourse.Week1.Lab.Day4.Invoice", metadata);
            Assert.Contains("Properties:", metadata);
            Assert.Contains("Name: CustomerName, Type: String", metadata);
            Assert.Contains("Name: CalculateTax, Return Type: Decimal", metadata);
            Assert.Contains("Constructor: Invoice(Int32 id, String customerName, Decimal amount)", metadata);
        }

        [Fact]
        public void CreateAndModifyViaReflection_CreatesCorrectInstance_AndUpdatesProperty()
        {
            // Act
            Invoice invoice = InvoiceReflector.CreateAndModifyViaReflection(123, "Alice", 450.50m, "Bob");

            // Assert
            Assert.NotNull(invoice);
            Assert.Equal(123, invoice.Id);
            Assert.Equal("Bob", invoice.CustomerName); // Property was successfully set entirely via reflection
            Assert.Equal(450.50m, invoice.Amount);
        }

        #endregion

        #region Task 1.12 Custom Attribute Validation Tests

        [Fact]
        public void ValidationEngine_ValidObject_ReturnsTrueAndNoWarnings()
        {
            // Arrange (Length of name is 6 <= 10)
            var user = new User { Name = "Sakthi", Email = "sakthi@example.com" };

            // Act
            var (isValid, warnings) = ValidationEngine.Validate(user);

            // Assert
            Assert.True(isValid);
            Assert.Empty(warnings);
        }

        [Fact]
        public void ValidationEngine_InvalidObjectLength_ReturnsFalseAndWarnings()
        {
            // Arrange (Length of name is 17 > 10)
            var user = new User { Name = "Sakthivelu Selvam", Email = "sakthivelu@example.com" };

            // Act
            var (isValid, warnings) = ValidationEngine.Validate(user);

            // Assert
            Assert.False(isValid);
            Assert.Single(warnings);
            Assert.Contains("Warning: Property 'Name'", warnings[0]);
            Assert.Contains("exceeds the maximum limit of 10 characters", warnings[0]);
        }

        #endregion
    }
}
