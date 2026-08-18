using System.ComponentModel.DataAnnotations;

namespace BridgeCourse.Week4.Api.DTOs
{
    public class StudentCreateDto
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Range(5, 100)]
        public int Age { get; set; }

        [Required]
        [EmailAddress]
        [MaxLength(100)]
        public string Email { get; set; } = string.Empty;

        [Range(0, 100)]
        public decimal Grade { get; set; }

        public string? InternalNotes { get; set; }
    }
}
