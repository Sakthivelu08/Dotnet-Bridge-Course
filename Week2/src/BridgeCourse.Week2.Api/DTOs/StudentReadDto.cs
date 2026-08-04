namespace BridgeCourse.Week2.Api.DTOs
{
    public class StudentReadDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Age { get; set; }
        public string Email { get; set; } = string.Empty;
        public decimal RawGrade { get; set; }
        public string FormattedGrade { get; set; } = string.Empty;
    }
}
