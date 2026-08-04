using System.ComponentModel.DataAnnotations;

namespace BridgeCourse.Week2.Api.Models
{
    public class Student : IEntity
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [Range(5, 100)]
        public int Age { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [Range(0, 100)]
        public decimal Grade { get; set; }

        public string InternalNotes { get; set; } = string.Empty;
    }
}
