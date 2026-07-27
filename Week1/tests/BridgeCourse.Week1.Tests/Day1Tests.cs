using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using BridgeCourse.Week1.Lab.Day1;
using Xunit;

namespace BridgeCourse.Week1.Tests
{
    public class Day1Tests
    {
        #region Task 1.1 Exceptions Tests

        [Fact]
        public void Withdraw_UnderBalance_ReducesBalance()
        {
            // Arrange
            var account = new BankAccount(100m);

            // Act
            account.Withdraw(40m);

            // Assert
            Assert.Equal(60m, account.Balance);
        }

        [Fact]
        public void Withdraw_OverBalance_ThrowsInsufficientFundsExceptionAndSetsDeficit()
        {
            // Arrange
            var account = new BankAccount(100m);

            // Act & Assert
            var exception = Assert.Throws<InsufficientFundsException>(() => account.Withdraw(150m));
            Assert.Equal(50m, exception.DeficitAmount);
        }

        [Theory]
        [InlineData("123", "Successfully parsed: 123")]
        [InlineData("abc", "Error: FormatException")]
        [InlineData("9999999999999", "Error: OverflowException")]
        [InlineData(null, "Error: general Exception (ArgumentNullException)")]
        public void ParseAndProcess_WithVariousInputs_ReturnsExpectedResult(string? input, string expected)
        {
            // Act
            string result = ExceptionOrderDemo.ParseAndProcess(input);

            // Assert
            Assert.Equal(expected, result);
        }

        #endregion

        #region Task 1.2 TempFileManager Tests

        [Fact]
        public void TempFileManager_CreatesFileInsideUsing_AndDeletesOutsideUsing()
        {
            string? filePath = null;

            // Act
            using (var manager = new TempFileManager())
            {
                filePath = manager.FilePath;
                
                // Assert file exists inside using block
                Assert.True(File.Exists(filePath), "Temp file should exist inside the using block.");
                Assert.Equal("Temporary file content created by TempFileManager.", manager.ReadContent());
            }

            // Assert file does not exist after using block (disposed)
            Assert.NotNull(filePath);
            Assert.False(File.Exists(filePath), "Temp file should be deleted after the using block.");
        }

        [Fact]
        public void TempFileManager_ReadContentAfterDispose_ThrowsObjectDisposedException()
        {
            // Arrange
            var manager = new TempFileManager();
            manager.Dispose();

            // Act & Assert
            Assert.Throws<ObjectDisposedException>(() => manager.ReadContent());
        }

        #endregion

        #region Task 1.3 Async Performance Tests

        [Fact]
        public async Task UserDataFetcher_Concurrent_IsMeasurablyFasterThanSequential()
        {
            // Arrange
            var fetcher = new UserDataFetcher();
            var ids = new List<int> { 1, 2, 3 };

            // Act
            // We run concurrent first because it's faster.
            var (concurrentData, concurrentTime) = await fetcher.RunConcurrentFetchesAsync(ids);
            
            // Assert fetch outputs are correct
            Assert.Equal(3, concurrentData.Length);
            Assert.Contains("UserData_For_User_1", concurrentData);
            Assert.Contains("UserData_For_User_2", concurrentData);
            Assert.Contains("UserData_For_User_3", concurrentData);

            // Verify elapsed time for concurrent matches expectations (~3000ms delay)
            // It should be roughly 3 seconds (allowing some thread scheduling overhead, e.g., 3000-4500ms).
            // Compare this to sequential fetches which would take 3 * 3 = 9 seconds.
            Assert.True(concurrentTime < 6000, $"Concurrent run took too long: {concurrentTime} ms (Expected < 6000ms)");
        }

        #endregion
    }
}
