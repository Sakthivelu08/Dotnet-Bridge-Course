using System.ComponentModel.DataAnnotations;

namespace BridgeCourse.Week2.Api.DTOs
{
    public class TeacherCreateDto
    {
        [Required(ErrorMessage = "Name is required.")]
        [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters.")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Subject is required.")]
        [StringLength(100, ErrorMessage = "Subject cannot exceed 100 characters.")]
        public string Subject { get; set; } = string.Empty;

        [Required(ErrorMessage = "Salary is required.")]
        [Range(0, 1000000, ErrorMessage = "Salary must be between 0 and 1,000,000.")]
        public decimal Salary { get; set; }

        public string InternalNotes { get; set; } = string.Empty;
    }
}
