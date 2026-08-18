using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BridgeCourse.Week4.Api.DTOs;
using BridgeCourse.Week4.Api.Services;

namespace BridgeCourse.Week4.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class TeachersController : ControllerBase
    {
        private readonly TeacherService _teacherService;

        public TeachersController(TeacherService teacherService)
        {
            _teacherService = teacherService;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            return Ok(_teacherService.GetAllTeachers());
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var teacher = _teacherService.GetTeacherById(id);
            if (teacher == null) return NotFound($"Teacher with ID {id} not found.");
            return Ok(teacher);
        }

        [Authorize(Roles = "Teacher")]
        [HttpPost]
        public IActionResult Create([FromBody] TeacherCreateDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var created = _teacherService.CreateTeacher(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [Authorize(Roles = "Teacher")]
        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] TeacherCreateDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var success = _teacherService.UpdateTeacher(id, dto);
            if (!success) return NotFound($"Teacher with ID {id} not found.");
            return NoContent();
        }

        [Authorize(Roles = "Teacher")]
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var success = _teacherService.DeleteTeacher(id);
            if (!success) return NotFound($"Teacher with ID {id} not found.");
            return NoContent();
        }
    }
}
