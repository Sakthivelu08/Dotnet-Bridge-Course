using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BridgeCourse.Week3.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SecureController : ControllerBase
    {
        [Authorize(Roles = "Teacher")]
        [HttpGet("teacher-only")]
        public IActionResult GetTeacherData()
        {
            return Ok("Welcome, Teacher! Secure payload retrieved successfully.");
        }

        [Authorize(Roles = "Student")]
        [HttpGet("student-only")]
        public IActionResult GetStudentData()
        {
            return Ok("Welcome, Student! Secure payload retrieved successfully.");
        }
    }
}
