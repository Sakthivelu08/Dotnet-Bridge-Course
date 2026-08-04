using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using BridgeCourse.Week2.Api.DTOs;
using BridgeCourse.Week2.Api.Models;
using BridgeCourse.Week2.Api.Repositories;
using BridgeCourse.Week2.Api.Services;
using BridgeCourse.Week2.Api.Strategies;
using Xunit;

namespace BridgeCourse.Week2.Tests
{
    public class ServiceTests
    {
        private readonly InMemoryUnitOfWork _uow;
        private readonly GradeStrategyFactory _factory;
        private readonly StudentService _studentService;
        private readonly TeacherService _teacherService;

        public ServiceTests()
        {
            _uow = new InMemoryUnitOfWork();
            // Clear lists in case of static instances (in-memory repositories are instance-based in UoW constructor)
            ((InMemoryRepository<Student>)_uow.Students).Clear();
            ((InMemoryRepository<Teacher>)_uow.Teachers).Clear();

            var accessor = new FakeHttpContextAccessor { HttpContext = new DefaultHttpContext() };
            _factory = new GradeStrategyFactory(accessor);

            _studentService = new StudentService(_uow, _factory);
            _teacherService = new TeacherService(_uow);
        }

        [Fact]
        public void StudentService_CreateAndGet_WorksCorrectly()
        {
            // Arrange
            var createDto = new StudentCreateDto
            {
                Name = "John Doe",
                Age = 18,
                Email = "john@example.com",
                Grade = 90.0m,
                InternalNotes = "Private details"
            };

            // Act: Create
            var readDto = _studentService.Create(createDto);

            // Assert: Verify creation and that InternalNotes are not exposed in readDto
            Assert.NotNull(readDto);
            Assert.Equal(1, readDto.Id);
            Assert.Equal("John Doe", readDto.Name);
            Assert.Equal("90.0%", readDto.FormattedGrade);
            Assert.Equal(90.0m, readDto.RawGrade);
            
            // Verify InternalNotes does NOT exist in StudentReadDto (does not compile if we check since it doesn't exist, which confirms safety!)
            // To verify via reflection:
            var readProperties = typeof(StudentReadDto).GetProperties().Select(p => p.Name);
            Assert.DoesNotContain("InternalNotes", readProperties);
        }

        /*
        [Fact]
        public void StudentService_Update_WorksCorrectly()
        {
            var s = new Student { Name = "John", Age = 18, Email = "john@example.com", Grade = 80m };
            _uow.Students.Add(s);

            var updateDto = new StudentCreateDto
            {
                Name = "John Updated",
                Age = 19,
                Email = "john.updated@example.com",
                Grade = 85m,
                InternalNotes = "Secret update"
            };

            var readDto = _studentService.Update(s.Id, updateDto);

            Assert.NotNull(readDto);
            Assert.Equal("John Updated", readDto.Name);
            Assert.Equal(19, readDto.Age);
            Assert.Equal(85m, readDto.RawGrade);
        }
        */

        [Fact]
        public void StudentService_Delete_WorksCorrectly()
        {
            var s = new Student { Name = "John" };
            _uow.Students.Add(s);

            var result = _studentService.Delete(s.Id);
            Assert.True(result);
            Assert.Null(_studentService.GetById(s.Id));

            // Delete non-existent
            var result2 = _studentService.Delete(999);
            Assert.False(result2);
        }

        [Fact]
        public void StudentService_SearchByName_Scenarios()
        {
            _uow.Students.Add(new Student { Name = "Alice Smith" });
            _uow.Students.Add(new Student { Name = "Bob Jones" });
            _uow.Students.Add(new Student { Name = "Charlie Smith" });

            // 1. Search Hit (Smith) - Case-insensitive check
            var hits = _studentService.SearchByName("smith").ToList();
            Assert.Equal(2, hits.Count);
            Assert.Contains(hits, h => h.Name == "Alice Smith");
            Assert.Contains(hits, h => h.Name == "Charlie Smith");

            // 2. Search Miss
            var misses = _studentService.SearchByName("Zach");
            Assert.Empty(misses);

            // 3. Empty Query (returns all)
            var all = _studentService.SearchByName("").ToList();
            Assert.Equal(3, all.Count);
        }

        [Fact]
        public void TeacherService_CreateAndGet_WorksCorrectly()
        {
            var createDto = new TeacherCreateDto
            {
                Name = "Professor X",
                Email = "x@xavier.edu",
                Subject = "Mutant Studies",
                Salary = 80000m,
                InternalNotes = "Telepathic"
            };

            var readDto = _teacherService.Create(createDto);

            Assert.NotNull(readDto);
            Assert.Equal(1, readDto.Id);
            Assert.Equal("Professor X", readDto.Name);
            Assert.Equal(80000m, readDto.Salary);

            var readProperties = typeof(TeacherReadDto).GetProperties().Select(p => p.Name);
            Assert.DoesNotContain("InternalNotes", readProperties);
        }

        [Fact]
        public void TeacherService_Search_WorksCorrectly()
        {
            var t = new Teacher { Name = "Hank McCoy", Email = "hank@beast.com", Subject = "Biochemistry", Salary = 70000m };
            _uow.Teachers.Add(t);

            var searchResults = _teacherService.SearchByName("mccoy").ToList();
            Assert.Single(searchResults);
            Assert.Equal("Hank McCoy", searchResults[0].Name);
        }
    }
}
