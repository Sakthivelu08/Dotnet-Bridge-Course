using System.ComponentModel.DataAnnotations;

namespace BridgeCourse.Week2.Api.DTOs
{
    public class StudentCreateDto
    {
        [Required(ErrorMessage = "Name is required.")]
        [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters.")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Age is required.")]
        [Range(5, 100, ErrorMessage = "Age must be between 5 and 100.")]
        public int Age { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Grade is required.")]
        [Range(0, 100, ErrorMessage = "Grade must be between 0 and 100.")]
        public decimal Grade { get; set; }

        public string InternalNotes { get; set; } = string.Empty;
    }
}
