using Microsoft.EntityFrameworkCore;
using BridgeCourse.Week4.Api.Models;
using BridgeCourse.Week4.Api.Services;

namespace BridgeCourse.Week4.Api.Data.EfCodeFirst
{
    public class EfCodeFirstDbContext : DbContext
    {
        public DbSet<Student> Students { get; set; }
        public DbSet<Teacher> Teachers { get; set; }
        public DbSet<User> Users { get; set; }

        public EfCodeFirstDbContext(DbContextOptions<EfCodeFirstDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Student>(entity =>
            {
                entity.HasKey(s => s.Id);
                entity.Property(s => s.Name).IsRequired().HasMaxLength(100);
                entity.Property(s => s.Email).IsRequired().HasMaxLength(100);
                entity.HasIndex(s => s.Email).IsUnique();
                entity.Property(s => s.Grade).HasPrecision(5, 2);
                entity.Property(s => s.EnrolledOn).HasDefaultValueSql("GETUTCDATE()");
            });

            modelBuilder.Entity<Teacher>(entity =>
            {
                entity.HasKey(t => t.Id);
                entity.Property(t => t.Name).IsRequired().HasMaxLength(100);
                entity.Property(t => t.Email).IsRequired().HasMaxLength(100);
                entity.HasIndex(t => t.Email).IsUnique();
                entity.Property(t => t.Subject).IsRequired().HasMaxLength(100);
                entity.Property(t => t.Salary).HasPrecision(18, 2);
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(u => u.Username);
                entity.Property(u => u.Username).HasMaxLength(50);
                entity.Property(u => u.Role).IsRequired().HasMaxLength(20);
            });

            var studentCreds = PasswordHasher.HashPassword("student123");
            var teacherCreds = PasswordHasher.HashPassword("teacher123");

            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Username = "student",
                    PasswordHash = studentCreds.Hash,
                    PasswordSalt = studentCreds.Salt,
                    Role = "Student"
                },
                new User
                {
                    Username = "teacher",
                    PasswordHash = teacherCreds.Hash,
                    PasswordSalt = teacherCreds.Salt,
                    Role = "Teacher"
                }
            );
        }
    }
}
