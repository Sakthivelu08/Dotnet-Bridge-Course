using Microsoft.AspNetCore.Http;
using System;

namespace BridgeCourse.Week2.Api.Strategies
{
    public class GradeStrategyFactory
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public GradeStrategyFactory(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public IGradeStrategy GetStrategy()
        {
            var context = _httpContextAccessor.HttpContext;
            if (context != null && context.Request.Headers.TryGetValue("X-Grade-Format", out var headerValue))
            {
                var format = headerValue.ToString().Trim().ToUpper();
                if (format == "GPA")
                {
                    return new GpaGradeStrategy();
                }
            }

            return new PercentageGradeStrategy();
        }
    }
}
