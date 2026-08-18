namespace BridgeCourse.Week4.Api.Models
{
    public class Teacher : IEntity
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;
        public decimal Salary { get; set; }
        public string? InternalNotes { get; set; }
    }
}
