using System;

namespace BridgeCourse.Week4.Api.Models
{
    public class Student : IEntity
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Age { get; set; }
        public string Email { get; set; } = string.Empty;
        public decimal Grade { get; set; }
        public string? InternalNotes { get; set; }
        public DateTime EnrolledOn { get; set; } = DateTime.UtcNow;
    }
}
