using System;

namespace BridgeCourse.Week2.Api.Strategies
{
    public class GpaGradeStrategy : IGradeStrategy
    {
        public string Name => "GPA";

        public string FormatGrade(decimal rawScore)
        {
            // Convert raw score (0-100) to GPA (0-4.0 scale)
            decimal gpa = (rawScore / 100m) * 4.0m;
            return $"{gpa:F2} GPA";
        }
    }
}
