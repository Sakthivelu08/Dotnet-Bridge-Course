namespace BridgeCourse.Week4.Api.Data.EfDbFirst.Models
{
    public class Teacher
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;
        public decimal Salary { get; set; }
        public string? InternalNotes { get; set; }
    }
}
