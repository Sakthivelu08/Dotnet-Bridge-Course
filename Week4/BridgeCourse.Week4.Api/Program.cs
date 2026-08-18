using System;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using BridgeCourse.Week4.Api.Data.AdoNet;
using BridgeCourse.Week4.Api.Data.EfCodeFirst;
using BridgeCourse.Week4.Api.Data.EfDbFirst;
using BridgeCourse.Week4.Api.Models;
using BridgeCourse.Week4.Api.Repositories;
using BridgeCourse.Week4.Api.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter JWT Bearer token: Bearer {token}"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// Swappable registration of repositories (Task 4.11)
var dataLayer = builder.Configuration["DataLayer"] ?? "EfCodeFirst";

if (dataLayer == "AdoNet")
{
    builder.Services.AddScoped<IRepository<Student>, AdoNetStudentRepository>();
    builder.Services.AddScoped<IRepository<Teacher>, AdoNetTeacherRepository>();
    builder.Services.AddScoped<IUserRepository, AdoNetUserRepository>();
    builder.Services.AddScoped<IUnitOfWork, AdoNetUnitOfWork>();
}
else if (dataLayer == "EfDbFirst")
{
    builder.Services.AddDbContext<EfDbFirstDbContext>(options =>
        options.UseSqlServer(connectionString));

    builder.Services.AddScoped<IRepository<Student>, DbFirstStudentRepository>();
    builder.Services.AddScoped<IRepository<Teacher>, DbFirstTeacherRepository>();
    builder.Services.AddScoped<IUserRepository, DbFirstUserRepository>();
    builder.Services.AddScoped<IUnitOfWork, DbFirstUnitOfWork>();
}
else // Default is EfCodeFirst
{
    builder.Services.AddDbContext<EfCodeFirstDbContext>(options =>
        options.UseSqlServer(connectionString));

    builder.Services.AddScoped<IRepository<Student>, EfStudentRepository>();
    builder.Services.AddScoped<IRepository<Teacher>, EfTeacherRepository>();
    builder.Services.AddScoped<IUserRepository, EfUserRepository>();
    builder.Services.AddScoped<IUnitOfWork, EfCodeFirstUnitOfWork>();
}

// Service registrations
builder.Services.AddScoped<StudentService>();
builder.Services.AddScoped<TeacherService>();

// JWT Authentication Setup
var jwtKey = Encoding.UTF8.GetBytes("SuperSecretKeyForJWTAuthWeek4MustBeAtLeast32Bytes!");
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = "Week4Api",
        ValidAudience = "Week4Clients",
        IssuerSigningKey = new SymmetricSecurityKey(jwtKey),
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddAuthorization();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
