using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;
using BridgeCourse.Week2.Api.Controllers;
using BridgeCourse.Week2.Api.DTOs;
using BridgeCourse.Week2.Api.Models;
using BridgeCourse.Week2.Api.Repositories;
using BridgeCourse.Week2.Api.Services;
using BridgeCourse.Week2.Api.Strategies;
using Xunit;

namespace BridgeCourse.Week2.Tests
{
    public class ControllerTests
    {
        private readonly InMemoryUnitOfWork _uow;
        private readonly StudentService _studentService;
        private readonly TeacherService _teacherService;
        private readonly StudentsController _studentsController;
        private readonly TeachersController _teachersController;

        public ControllerTests()
        {
            _uow = new InMemoryUnitOfWork();
            ((InMemoryRepository<Student>)_uow.Students).Clear();
            ((InMemoryRepository<Teacher>)_uow.Teachers).Clear();

            var accessor = new FakeHttpContextAccessor { HttpContext = new DefaultHttpContext() };
            var factory = new GradeStrategyFactory(accessor);

            _studentService = new StudentService(_uow, factory);
            _teacherService = new TeacherService(_uow);

            _studentsController = new StudentsController(_studentService, NullLogger<StudentsController>.Instance);
            _teachersController = new TeachersController(_teacherService, NullLogger<TeachersController>.Instance);
        }

        [Fact]
        public void StudentsController_GetAll_Returns200Ok()
        {
            var result = _studentsController.GetAll();
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(200, okResult.StatusCode);
        }

        [Fact]
        public void StudentsController_GetById_Returns200Ok_Or_404NotFound()
        {
            // 1. Arrange & Act: GET non-existent
            var notFoundResult = _studentsController.GetById(999);
            var notFoundObj = Assert.IsType<NotFoundResult>(notFoundResult.Result);
            Assert.Equal(404, notFoundObj.StatusCode);

            // 2. Arrange & Act: GET existent
            var s = new Student { Name = "Charlie" };
            _uow.Students.Add(s);

            var okResult = _studentsController.GetById(s.Id);
            var okObj = Assert.IsType<OkObjectResult>(okResult.Result);
            Assert.Equal(200, okObj.StatusCode);
        }

        [Fact]
        public void StudentsController_Create_Returns201Created()
        {
            var createDto = new StudentCreateDto
            {
                Name = "Alice",
                Age = 20,
                Email = "alice@example.com",
                Grade = 95m
            };

            var result = _studentsController.Create(createDto);
            var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            Assert.Equal(201, createdResult.StatusCode);

            var readDto = Assert.IsType<StudentReadDto>(createdResult.Value);
            Assert.Equal("Alice", readDto.Name);
            Assert.Equal(1, readDto.Id);
        }

        /*
        [Fact]
        public void StudentsController_Update_Returns204NoContent_Or_404NotFound()
        {
            var dto = new StudentCreateDto { Name = "Bob", Age = 21, Email = "bob@example.com", Grade = 80m };

            // 1. Act: Update non-existent
            var notFoundResult = _studentsController.Update(999, dto);
            Assert.IsType<NotFoundResult>(notFoundResult);

            // 2. Act: Update existent
            var s = new Student { Name = "Bob" };
            _uow.Students.Add(s);

            var noContentResult = _studentsController.Update(s.Id, dto);
            Assert.IsType<NoContentResult>(noContentResult);
        }
        */

        [Fact]
        public void StudentsController_Delete_Returns204NoContent_Or_404NotFound()
        {
            // 1. Act: Delete non-existent
            var notFoundResult = _studentsController.Delete(999);
            Assert.IsType<NotFoundResult>(notFoundResult);

            // 2. Act: Delete existent
            var s = new Student { Name = "Bob" };
            _uow.Students.Add(s);

            var noContentResult = _studentsController.Delete(s.Id);
            Assert.IsType<NoContentResult>(noContentResult);
        }

        [Fact]
        public void TeachersController_CRUD_ReturnsExpectedStatusCodes()
        {
            var createDto = new TeacherCreateDto { Name = "Logan", Email = "wolverine@x.com", Subject = "Combat", Salary = 90000m };

            // POST (201 Created)
            var postResult = _teachersController.Create(createDto);
            var createdResult = Assert.IsType<CreatedAtActionResult>(postResult.Result);
            Assert.Equal(201, createdResult.StatusCode);

            var teacher = Assert.IsType<TeacherReadDto>(createdResult.Value);
            Assert.Equal(1, teacher.Id);

            // GET by ID (200 OK)
            var getResult = _teachersController.GetById(teacher.Id);
            var okGetResult = Assert.IsType<OkObjectResult>(getResult.Result);
            Assert.Equal(200, okGetResult.StatusCode);

            // DELETE (204 No Content)
            var deleteResult = _teachersController.Delete(teacher.Id);
            Assert.IsType<NoContentResult>(deleteResult);

            // GET by ID after delete (404 Not Found)
            var getResultAfterDelete = _teachersController.GetById(teacher.Id);
            Assert.IsType<NotFoundResult>(getResultAfterDelete.Result);
        }
    }
}
