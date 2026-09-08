using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using BridgeCourse.Week4.Api.Repositories;
using BridgeCourse.Week4.Api.Services;

namespace BridgeCourse.Week4.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public AuthController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            var username = !string.IsNullOrEmpty(request.Username) ? request.Username : request.Email;
            var user = _unitOfWork.Users.GetByUsername(username);
            if (user == null || !PasswordHasher.VerifyPassword(request.Password, user.PasswordHash, user.PasswordSalt))
            {
                return Unauthorized("Invalid credentials.");
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes("SuperSecretKeyForJWTAuthWeek4MustBeAtLeast32Bytes!");
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim(ClaimTypes.Role, user.Role)
                }),
                Expires = DateTime.UtcNow.AddHours(2),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                Issuer = "Week4Api",
                Audience = "Week4Clients"
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            return Ok(new { Token = tokenString, Role = user.Role });
        }

        [HttpPost("register")]
        public IActionResult Register([FromBody] RegisterRequest request)
        {
            var username = !string.IsNullOrEmpty(request.Email) ? request.Email : request.Username;
            var existing = _unitOfWork.Users.GetByUsername(username);
            if (existing == null)
            {
                var hashResult = PasswordHasher.HashPassword(request.Password);
                var roleStr = request.Role == "1" || request.Role == "Teacher" ? "Teacher" : "Student";

                var user = new User
                {
                    Username = username,
                    PasswordHash = hashResult.Hash,
                    PasswordSalt = hashResult.Salt,
                    Role = roleStr
                };

                _unitOfWork.Users.Add(user);
                _unitOfWork.Complete();
            }

            return Ok(new { Message = "Registration successful" });
        }
    }

    public class LoginRequest
    {
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class RegisterRequest
    {
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Role { get; set; } = "Student";
    }
}
