using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BridgeCourse.Week4.Api.DTOs;
using BridgeCourse.Week4.Api.Services;

namespace BridgeCourse.Week4.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class StudentsController : ControllerBase
    {
        private readonly StudentService _studentService;

        public StudentsController(StudentService studentService)
        {
            _studentService = studentService;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            return Ok(_studentService.GetAllStudents());
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var student = _studentService.GetStudentById(id);
            if (student == null) return NotFound($"Student with ID {id} not found.");
            return Ok(student);
        }

        [Authorize(Roles = "Teacher")]
        [HttpPost]
        public IActionResult Create([FromBody] StudentCreateDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var created = _studentService.CreateStudent(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [Authorize(Roles = "Teacher")]
        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] StudentCreateDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var success = _studentService.UpdateStudent(id, dto);
            if (!success) return NotFound($"Student with ID {id} not found.");
            return NoContent();
        }

        [Authorize(Roles = "Teacher")]
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var success = _studentService.DeleteStudent(id);
            if (!success) return NotFound($"Student with ID {id} not found.");
            return NoContent();
        }
    }
}
