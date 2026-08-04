namespace BridgeCourse.Week2.Api.Strategies
{
    public interface IGradeStrategy
    {
        string Name { get; }
        string FormatGrade(decimal rawScore);
    }
}
