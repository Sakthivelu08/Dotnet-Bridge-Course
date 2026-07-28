using System;
using System.Collections.Generic;
using System.Linq;
using BridgeCourse.Week1.Lab.Day3;
using Xunit;

namespace BridgeCourse.Week1.Tests
{
    public class Day3Tests
    {
        #region Task 1.7 Strategy Tests

        [Fact]
        public void ShoppingCart_CheckoutWithStrategy_ExecutesSuccessfully()
        {
            // Arrange
            var cart = new ShoppingCart(100.00m, new CreditCardPaymentStrategy("1234567812345678", "Alice"));

            // Act & Assert (Should not throw)
            cart.Checkout();
        }

        [Fact]
        public void ShoppingCart_StrategySwapAtRuntime_ExecutesSuccessfully()
        {
            // Arrange
            var cart = new ShoppingCart(100.00m, new CreditCardPaymentStrategy("1234567812345678", "Alice"));
            
            // Act: Swap to UPI
            cart.SetPaymentStrategy(new UpiPaymentStrategy("alice@okaxis"));
            cart.Checkout();

            // Act: Swap to NetBanking
            cart.SetPaymentStrategy(new NetBankingPaymentStrategy("ICICI Bank"));
            cart.Checkout();
        }

        [Fact]
        public void ShoppingCart_CheckoutWithZeroOrNegativeAmount_ThrowsInvalidOperationException()
        {
            // Arrange
            var cart = new ShoppingCart(0m, new UpiPaymentStrategy("alice@upi"));
            var negativeCart = new ShoppingCart(-10.50m, new UpiPaymentStrategy("alice@upi"));

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => cart.Checkout());
            Assert.Throws<InvalidOperationException>(() => negativeCart.Checkout());
        }

        #endregion

        #region Task 1.8 Repository and Unit of Work Tests

        [Fact]
        public void UnitOfWork_CRUDOperations_PerformCorrectly()
        {
            // Arrange: Clean static repository lists
            StudentRepository.Clear();
            CourseRepository.Clear();

            using (var uow = new UnitOfWork())
            {
                var student = new Student { Name = "Sakthi", Email = "sakthi@example.com" };
                var course = new Course { Title = "C# Core", Code = "CS-101" };

                // Act: Add
                uow.Students.Add(student);
                uow.Courses.Add(course);

                // Assert: Retrieve
                var allStudents = uow.Students.GetAll().ToList();
                var allCourses = uow.Courses.GetAll().ToList();

                Assert.Single(allStudents);
                Assert.Single(allCourses);
                Assert.Equal("Sakthi", allStudents[0].Name);
                Assert.Equal("CS-101", allCourses[0].Code);

                // Act: Update
                student.Name = "Sakthi Selvam";
                uow.Students.Update(student);

                // Assert: Updated value is fetched
                var fetchedStudent = uow.Students.GetById(student.Id);
                Assert.NotNull(fetchedStudent);
                Assert.Equal("Sakthi Selvam", fetchedStudent.Name);

                // Act: Delete
                uow.Courses.Delete(course.Id);

                // Assert: Deleted
                Assert.Empty(uow.Courses.GetAll());

                // Act: Save (should compile and execute successfully)
                uow.Save();
            }
        }

        [Fact]
        public void UnitOfWork_SaveAfterDispose_ThrowsObjectDisposedException()
        {
            // Arrange
            var uow = new UnitOfWork();
            uow.Dispose();

            // Act & Assert
            Assert.Throws<ObjectDisposedException>(() => uow.Save());
        }

        #endregion

        #region Task 1.9 Adapter & Facade Tests

        [Fact]
        public void OrderFacade_PlaceOrder_RunsAllSubsystemsSuccessfully()
        {
            // Arrange
            var facade = new OrderFacade();

            // Act
            bool result = facade.PlaceOrder(9001, 101, 2, 350.00m, "creditcard", "4321432143214321");

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void XmlReportAdapter_AdaptsJsonToXmlCorrectly_AndRunsGenerator()
        {
            // Arrange
            var legacyGenerator = new XmlReportGenerator();
            var adapter = new XmlReportAdapter(legacyGenerator);
            var jsonReport = new JsonReportData { OrderId = 9001, TotalAmount = 350.50m };

            // Act
            string result = adapter.ConvertAndGenerateReport(jsonReport);

            // Assert
            Assert.Contains("[SUCCESS] Legacy report processed successfully", result);
            Assert.Contains("Payload length", result);
        }

        #endregion

        #region Repository Exception Tests

        [Fact]
        public void StudentRepository_AddDuplicateId_ThrowsInvalidOperationException()
        {
            StudentRepository.Clear();
            var repo = new StudentRepository();
            var s1 = new Student { Id = 1, Name = "Alice" };
            var s2 = new Student { Id = 1, Name = "Bob" };

            repo.Add(s1);
            Assert.Throws<InvalidOperationException>(() => repo.Add(s2));
        }

        [Fact]
        public void StudentRepository_UpdateNonExistent_ThrowsKeyNotFoundException()
        {
            StudentRepository.Clear();
            var repo = new StudentRepository();
            var s = new Student { Id = 999, Name = "Nobody" };

            Assert.Throws<KeyNotFoundException>(() => repo.Update(s));
        }

        [Fact]
        public void CourseRepository_AddDuplicateId_ThrowsInvalidOperationException()
        {
            CourseRepository.Clear();
            var repo = new CourseRepository();
            var c1 = new Course { Id = 10, Title = "Math" };
            var c2 = new Course { Id = 10, Title = "Science" };

            repo.Add(c1);
            Assert.Throws<InvalidOperationException>(() => repo.Add(c2));
        }

        [Fact]
        public void CourseRepository_UpdateNonExistent_ThrowsKeyNotFoundException()
        {
            CourseRepository.Clear();
            var repo = new CourseRepository();
            var c = new Course { Id = 999, Title = "Nobody" };

            Assert.Throws<KeyNotFoundException>(() => repo.Update(c));
        }

        #endregion
    }
}
