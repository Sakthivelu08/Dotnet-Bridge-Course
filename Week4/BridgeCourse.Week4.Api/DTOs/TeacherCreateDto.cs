using System.ComponentModel.DataAnnotations;

namespace BridgeCourse.Week4.Api.DTOs
{
    public class TeacherCreateDto
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [MaxLength(100)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string Subject { get; set; } = string.Empty;

        [Range(0, 1000000)]
        public decimal Salary { get; set; }

        public string? InternalNotes { get; set; }
    }
}
