using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using BridgeCourse.Week2.Api.Repositories;
using BridgeCourse.Week2.Api.Services;
using BridgeCourse.Week2.Api.Strategies;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddHttpContextAccessor();

builder.Services.AddSingleton<IUnitOfWork, InMemoryUnitOfWork>();
builder.Services.AddScoped<IStudentService, StudentService>();
builder.Services.AddScoped<ITeacherService, TeacherService>();
builder.Services.AddTransient<GradeStrategyFactory>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

/*
 * Default middleware order:
 * 1. ExceptionHandler
 * 2. Swagger / SwaggerUI
 * 3. HttpsRedirection
 * 4. Routing
 * 5. Authorization
 * 6. MapControllers
 */

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Week 2 Student API v1");
    });
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
