using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Moq;
using BridgeCourse.Week4.Api.Data.EfCodeFirst;
using BridgeCourse.Week4.Api.DTOs;
using BridgeCourse.Week4.Api.Models;
using BridgeCourse.Week4.Api.Repositories;
using BridgeCourse.Week4.Api.Services;
using Xunit;

namespace BridgeCourse.Week4.Tests
{
    public class DataAndServiceTests
    {
        #region 1. Mocked Service Tests

        [Fact]
        public void StudentService_GetAll_ReturnsCorrectMappings()
        {
            var mockRepo = new Mock<IRepository<Student>>();
            var mockUow = new Mock<IUnitOfWork>();
            mockRepo.Setup(r => r.GetAll()).Returns(new List<Student>
            {
                new Student { Id = 1, Name = "Alice", Age = 20, Email = "alice@test.com", Grade = 90.0m, InternalNotes = "Secret Notes" }
            });
            mockUow.Setup(u => u.Students).Returns(mockRepo.Object);

            var service = new StudentService(mockUow.Object);
            var result = service.GetAllStudents().ToList();

            Assert.Single(result);
            Assert.Equal("Alice", result[0].Name);
            Assert.Equal(20, result[0].Age);
            // Verify InternalNotes is not mapped to prevent data leaks (security check)
            Assert.Null(typeof(StudentReadDto).GetProperty("InternalNotes"));
        }

        [Fact]
        public void StudentService_GetById_ReturnsNullOnNotFound()
        {
            var mockRepo = new Mock<IRepository<Student>>();
            var mockUow = new Mock<IUnitOfWork>();
            mockRepo.Setup(r => r.GetById(99)).Returns((Student?)null);
            mockUow.Setup(u => u.Students).Returns(mockRepo.Object);

            var service = new StudentService(mockUow.Object);
            var result = service.GetStudentById(99);

            Assert.Null(result);
        }

        [Fact]
        public void StudentService_Create_AddsToRepository()
        {
            var mockRepo = new Mock<IRepository<Student>>();
            var mockUow = new Mock<IUnitOfWork>();
            mockUow.Setup(u => u.Students).Returns(mockRepo.Object);

            var service = new StudentService(mockUow.Object);
            var createDto = new StudentCreateDto
            {
                Name = "Bob",
                Age = 22,
                Email = "bob@test.com",
                Grade = 85.5m,
                InternalNotes = "Confidential"
            };

            var result = service.CreateStudent(createDto);

            Assert.NotNull(result);
            Assert.Equal("Bob", result.Name);
            mockRepo.Verify(r => r.Add(It.Is<Student>(s => s.Name == "Bob" && s.InternalNotes == "Confidential")), Times.Once);
            mockUow.Verify(u => u.Save(), Times.Once);
        }

        [Fact]
        public void StudentService_Update_ReturnsFalseOnNotFound()
        {
            var mockRepo = new Mock<IRepository<Student>>();
            var mockUow = new Mock<IUnitOfWork>();
            mockRepo.Setup(r => r.GetById(5)).Returns((Student?)null);
            mockUow.Setup(u => u.Students).Returns(mockRepo.Object);

            var service = new StudentService(mockUow.Object);
            var updateDto = new StudentCreateDto { Name = "Updated" };

            var result = service.UpdateStudent(5, updateDto);

            Assert.False(result);
        }

        [Fact]
        public void StudentService_Delete_ReturnsTrueOnSuccess()
        {
            var mockRepo = new Mock<IRepository<Student>>();
            var mockUow = new Mock<IUnitOfWork>();
            mockRepo.Setup(r => r.GetById(5)).Returns(new Student { Id = 5 });
            mockUow.Setup(u => u.Students).Returns(mockRepo.Object);

            var service = new StudentService(mockUow.Object);
            var result = service.DeleteStudent(5);

            Assert.True(result);
            mockRepo.Verify(r => r.Delete(5), Times.Once);
            mockUow.Verify(u => u.Save(), Times.Once);
        }

        #endregion

        #region 2. EF Core Code First Integration Tests (InMemory)

        [Fact]
        public void EfRepository_CrudRoundTrip_Succeeds()
        {
            var options = new DbContextOptionsBuilder<EfCodeFirstDbContext>()
                .UseInMemoryDatabase(databaseName: "BridgeCourse_TestDb_" + Guid.NewGuid().ToString())
                .Options;

            using (var context = new EfCodeFirstDbContext(options))
            {
                var repo = new EfStudentRepository(context);
                var student = new Student
                {
                    Name = "Charlie",
                    Age = 21,
                    Email = "charlie@test.com",
                    Grade = 88.0m,
                    InternalNotes = "Integration notes"
                };

                // Create
                repo.Add(student);
                context.SaveChanges();

                Assert.True(student.Id > 0);

                // Read
                var retrieved = repo.GetById(student.Id);
                Assert.NotNull(retrieved);
                Assert.Equal("Charlie", retrieved.Name);

                // Update
                retrieved.Name = "Charlie Updated";
                repo.Update(retrieved);
                context.SaveChanges();

                var updated = repo.GetById(student.Id);
                Assert.Equal("Charlie Updated", updated!.Name);

                // Delete
                repo.Delete(student.Id);
                context.SaveChanges();

                var deleted = repo.GetById(student.Id);
                Assert.Null(deleted);
            }
        }

        #endregion

        #region 3. ADO.NET Injection Parameterization Test

        [Fact]
        public void AdoNet_SqlCommand_ParameterizationSavesInjectionLiterally()
        {
            using (var command = new SqlCommand())
            {
                string rawInjectionInput = "'; DROP TABLE Students;--";
                command.CommandText = "INSERT INTO Students (Name) VALUES (@Name)";
                command.Parameters.AddWithValue("@Name", rawInjectionInput);

                var parameter = command.Parameters["@Name"];
                Assert.NotNull(parameter);
                
                // Assert that the command parameter stores the injection payload verbatim as data (safe)
                Assert.Equal(rawInjectionInput, parameter.Value);
            }
        }

        #endregion
    }
}
