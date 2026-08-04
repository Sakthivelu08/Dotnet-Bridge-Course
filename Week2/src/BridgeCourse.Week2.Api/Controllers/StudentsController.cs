using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using BridgeCourse.Week2.Api.DTOs;
using BridgeCourse.Week2.Api.Services;

namespace BridgeCourse.Week2.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StudentsController : ControllerBase
    {
        private readonly IStudentService _studentService;
        private readonly ILogger<StudentsController> _logger;

        public StudentsController(IStudentService studentService, ILogger<StudentsController> logger)
        {
            _studentService = studentService;
            _logger = logger;
        }

        [HttpGet]
        public ActionResult<IEnumerable<StudentReadDto>> GetAll()
        {
            return Ok(_studentService.GetAll());
        }

        [HttpGet("{id}")]
        public ActionResult<StudentReadDto> GetById(int id)
        {
            var student = _studentService.GetById(id);
            if (student == null)
            {
                _logger.LogWarning("Student with ID {Id} was not found.", id);
                return NotFound();
            }
            return Ok(student);
        }

        [HttpGet("search")]
        public ActionResult<IEnumerable<StudentReadDto>> Search([FromQuery] string name)
        {
            return Ok(_studentService.SearchByName(name));
        }

        [HttpPost]
        public ActionResult<StudentReadDto> Create([FromBody] StudentCreateDto createDto)
        {
            var student = _studentService.Create(createDto);
            _logger.LogInformation("Successfully created student with ID {Id}.", student.Id);
            return CreatedAtAction(nameof(GetById), new { id = student.Id }, student);
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] StudentCreateDto updateDto)
        {
            var updated = _studentService.Update(id, updateDto);
            if (updated == null)
            {
                _logger.LogWarning("Update failed. Student with ID {Id} was not found.", id);
                return NotFound();
            }
            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var deleted = _studentService.Delete(id);
            if (!deleted)
            {
                _logger.LogWarning("Delete failed. Student with ID {Id} was not found.", id);
                return NotFound();
            }
            return NoContent();
        }
    }
}
