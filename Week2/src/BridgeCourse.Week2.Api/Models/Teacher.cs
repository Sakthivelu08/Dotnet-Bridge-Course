using System.ComponentModel.DataAnnotations;

namespace BridgeCourse.Week2.Api.Models
{
    public class Teacher : IEntity
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Subject { get; set; } = string.Empty;

        [Required]
        [Range(0, 1000000)]
        public decimal Salary { get; set; }

        public string InternalNotes { get; set; } = string.Empty;
    }
}
