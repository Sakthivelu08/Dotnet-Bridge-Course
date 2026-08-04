using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using BridgeCourse.Week2.Api.DTOs;
using BridgeCourse.Week2.Api.Services;

namespace BridgeCourse.Week2.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TeachersController : ControllerBase
    {
        private readonly ITeacherService _teacherService;
        private readonly ILogger<TeachersController> _logger;

        public TeachersController(ITeacherService teacherService, ILogger<TeachersController> logger)
        {
            _teacherService = teacherService;
            _logger = logger;
        }

        [HttpGet]
        public ActionResult<IEnumerable<TeacherReadDto>> GetAll()
        {
            return Ok(_teacherService.GetAll());
        }

        [HttpGet("{id}")]
        public ActionResult<TeacherReadDto> GetById(int id)
        {
            var teacher = _teacherService.GetById(id);
            if (teacher == null)
            {
                _logger.LogWarning("Teacher with ID {Id} was not found.", id);
                return NotFound();
            }
            return Ok(teacher);
        }

        [HttpGet("search")]
        public ActionResult<IEnumerable<TeacherReadDto>> Search([FromQuery] string name)
        {
            return Ok(_teacherService.SearchByName(name));
        }

        [HttpPost]
        public ActionResult<TeacherReadDto> Create([FromBody] TeacherCreateDto createDto)
        {
            var teacher = _teacherService.Create(createDto);
            _logger.LogInformation("Successfully created teacher with ID {Id}.", teacher.Id);
            return CreatedAtAction(nameof(GetById), new { id = teacher.Id }, teacher);
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] TeacherCreateDto updateDto)
        {
            var updated = _teacherService.Update(id, updateDto);
            if (updated == null)
            {
                _logger.LogWarning("Update failed. Teacher with ID {Id} was not found.", id);
                return NotFound();
            }
            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var deleted = _teacherService.Delete(id);
            if (!deleted)
            {
                _logger.LogWarning("Delete failed. Teacher with ID {Id} was not found.", id);
                return NotFound();
            }
            return NoContent();
        }
    }
}
