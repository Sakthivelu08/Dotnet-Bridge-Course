using System;

namespace BridgeCourse.Week2.Api.Strategies
{
    public class PercentageGradeStrategy : IGradeStrategy
    {
        public string Name => "Percentage";

        public string FormatGrade(decimal rawScore)
        {
            // E.g. 85.5 -> "85.5%"
            return $"{rawScore:F1}%";
        }
    }
}
