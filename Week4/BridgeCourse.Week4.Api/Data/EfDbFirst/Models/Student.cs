using System;

namespace BridgeCourse.Week4.Api.Data.EfDbFirst.Models
{
    public class Student
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Age { get; set; }
        public string Email { get; set; } = string.Empty;
        public decimal Grade { get; set; }
        public string? InternalNotes { get; set; }
        public DateTime EnrolledOn { get; set; }
    }
}
