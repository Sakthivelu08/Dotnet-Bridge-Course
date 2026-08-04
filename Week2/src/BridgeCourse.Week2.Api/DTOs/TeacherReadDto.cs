namespace BridgeCourse.Week2.Api.DTOs
{
    public class TeacherReadDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;
        public decimal Salary { get; set; }
    }
}
