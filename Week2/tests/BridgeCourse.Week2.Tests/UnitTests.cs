using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using BridgeCourse.Week2.Api.Models;
using BridgeCourse.Week2.Api.Repositories;
using BridgeCourse.Week2.Api.Strategies;
using Xunit;

namespace BridgeCourse.Week2.Tests
{
    public class FakeHttpContextAccessor : IHttpContextAccessor
    {
        public HttpContext? HttpContext { get; set; }
    }

    public class UnitTests
    {
        [Fact]
        public void InMemoryRepository_AddAndGet_WorksCorrectly()
        {
            // Arrange
            var repo = new InMemoryRepository<Student>();
            var s = new Student { Id = 0, Name = "Alice", Age = 20, Email = "alice@example.com", Grade = 85.5m };

            // Act
            repo.Add(s);
            var fetched = repo.GetById(s.Id);

            // Assert
            Assert.NotNull(fetched);
            Assert.Equal("Alice", fetched.Name);
            Assert.Equal(1, fetched.Id); // Auto-incremented ID starts at 1
        }

        [Fact]
        public void InMemoryRepository_DuplicateId_ThrowsException()
        {
            var repo = new InMemoryRepository<Student>();
            var s1 = new Student { Id = 10, Name = "Alice" };
            var s2 = new Student { Id = 10, Name = "Bob" };

            repo.Add(s1);
            Assert.Throws<InvalidOperationException>(() => repo.Add(s2));
        }

        [Fact]
        public void InMemoryRepository_UpdateNonExistent_ThrowsException()
        {
            var repo = new InMemoryRepository<Student>();
            var s = new Student { Id = 999, Name = "Alice" };

            Assert.Throws<KeyNotFoundException>(() => repo.Update(s));
        }

        [Fact]
        public void InMemoryRepository_Delete_WorksCorrectly()
        {
            var repo = new InMemoryRepository<Student>();
            var s = new Student { Id = 0, Name = "Alice" };
            repo.Add(s);

            repo.Delete(s.Id);
            Assert.Null(repo.GetById(s.Id));
        }

        [Fact]
        public void PercentageGradeStrategy_FormatsCorrectly()
        {
            var strategy = new PercentageGradeStrategy();
            var formatted = strategy.FormatGrade(85.54m);
            Assert.Equal("85.5%", formatted);
        }

        [Fact]
        public void GpaGradeStrategy_FormatsCorrectly()
        {
            var strategy = new GpaGradeStrategy();
            var formatted = strategy.FormatGrade(90m);
            Assert.Equal("3.60 GPA", formatted); // (90 / 100) * 4 = 3.60
        }

        [Fact]
        public void GradeStrategyFactory_ResolvesPercentage_ByDefault()
        {
            var accessor = new FakeHttpContextAccessor { HttpContext = null };
            var factory = new GradeStrategyFactory(accessor);

            var strategy = factory.GetStrategy();
            Assert.IsType<PercentageGradeStrategy>(strategy);
        }

        [Fact]
        public void GradeStrategyFactory_ResolvesGpa_WhenHeaderIsProvided()
        {
            var context = new DefaultHttpContext();
            context.Request.Headers["X-Grade-Format"] = "GPA";
            var accessor = new FakeHttpContextAccessor { HttpContext = context };
            var factory = new GradeStrategyFactory(accessor);

            var strategy = factory.GetStrategy();
            Assert.IsType<GpaGradeStrategy>(strategy);
        }

        [Fact]
        public void GradeStrategyFactory_ResolvesPercentage_WhenInvalidHeaderIsProvided()
        {
            var context = new DefaultHttpContext();
            context.Request.Headers["X-Grade-Format"] = "INVALID";
            var accessor = new FakeHttpContextAccessor { HttpContext = context };
            var factory = new GradeStrategyFactory(accessor);

            var strategy = factory.GetStrategy();
            Assert.IsType<PercentageGradeStrategy>(strategy);
        }
    }
}
