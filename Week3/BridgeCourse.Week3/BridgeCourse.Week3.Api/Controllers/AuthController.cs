using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using BridgeCourse.Week3.Api.Models;
using BridgeCourse.Week3.Api.Services;

namespace BridgeCourse.Week3.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        public static readonly List<User> Users = new List<User>();

        static AuthController()
        {
            var studentCreds = PasswordHasher.HashPassword("student123");
            Users.Add(new User
            {
                Username = "student",
                PasswordHash = studentCreds.Hash,
                PasswordSalt = studentCreds.Salt,
                Role = "Student"
            });

            var teacherCreds = PasswordHasher.HashPassword("teacher123");
            Users.Add(new User
            {
                Username = "teacher",
                PasswordHash = teacherCreds.Hash,
                PasswordSalt = teacherCreds.Salt,
                Role = "Teacher"
            });
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            var user = Users.Find(u => u.Username.Equals(request.Username, StringComparison.OrdinalIgnoreCase));
            if (user == null || !PasswordHasher.VerifyPassword(request.Password, user.PasswordHash, user.PasswordSalt))
            {
                return Unauthorized("Invalid credentials.");
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes("SuperSecretKeyForJWTAuthWeek3MustBeAtLeast32Bytes!");
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim(ClaimTypes.Role, user.Role)
                }),
                Expires = DateTime.UtcNow.AddHours(2),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                Issuer = "Week3Api",
                Audience = "Week3Clients"
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            return Ok(new { Token = tokenString });
        }
    }

    public class LoginRequest
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
