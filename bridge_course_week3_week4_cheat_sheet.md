# Dotnet Bridge Course: Weeks 3 & 4 Comprehensive Guide

This guide is designed to serve as an offline, closed-book preparation reference for your hackathon. It walks through creating an API project from scratch, designing domain entities with Entity Framework Core (Fluent API), implementing swappable data layers (ADO.NET vs. EF Core), managing transactions with the Unit of Work pattern, and writing xUnit integration tests.

---

## 1. Project Initialization & Dependencies (Visual Studio 2022)

When starting a project offline, you can initialize the solution, add projects, and manage packages entirely through the Visual Studio Community 2022 interface.

### Creating the Projects
1. **Create Web API (Solution)**: Open VS 2022 -> Create a new project -> Search for **ASP.NET Core Web API** (C#) -> Name it `BridgeCourse.Api` -> Solution: `BridgeCourse` -> Choose **.NET 8.0 (LTS)** -> Ensure **Use controllers** is checked -> Click **Create**.
2. **Create Data Class Library**: Right-click Solution root -> **Add** -> **New Project...** -> Choose **Class Library** (C#) -> Name it `BridgeCourse.Data` -> Choose **.NET 8.0** -> Click **Create**.
3. **Create Test Project**: Right-click Solution root -> **Add** -> **New Project...** -> Choose **xUnit Test Project** (C#) -> Name it `BridgeCourse.Tests` -> Choose **.NET 8.0** -> Click **Create**.

### Adding Project References
1. Right-click the `Dependencies` node of `BridgeCourse.Api` -> **Add Project Reference...** -> Check `BridgeCourse.Data` -> Click **OK**.
2. Right-click the `Dependencies` node of `BridgeCourse.Tests` -> **Add Project Reference...** -> Check both `BridgeCourse.Data` and `BridgeCourse.Api` -> Click **OK**.

### Installing NuGet Packages via GUI
1. Right-click the Solution node -> **Manage NuGet Packages for Solution...**
2. Go to the **Browse** tab and search for:
   * **`Microsoft.EntityFrameworkCore.SqlServer`** -> Check `BridgeCourse.Data` -> Install.
   * **`Microsoft.EntityFrameworkCore.Design`** -> Check `BridgeCourse.Data` and `BridgeCourse.Api` -> Install.
   * **`Microsoft.Data.SqlClient`** -> Check `BridgeCourse.Data` -> Install.
   * **`Microsoft.EntityFrameworkCore.InMemory`** -> Check `BridgeCourse.Tests` -> Install.
   * **`Moq`** -> Check `BridgeCourse.Tests` -> Install.

---

## 2. Domain Models & Relationships (Fluent API)

We will design a scenario: **Course Enrollment System**.
* **Student** has a 1-to-many relationship with **Enrollment**.
* **Course** has a 1-to-many relationship with **Enrollment**.
* Together, **Student** and **Course** form a many-to-many relationship via the **Enrollment** join entity.

### Entities Definition
Create these inside `BridgeCourse.Data/Entities/`:

```csharp
namespace BridgeCourse.Data.Entities;

public class Student
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string PasswordSalt { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation property
    public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
}

public class Course
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public int Credits { get; set; }

    // Navigation property
    public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
}

public class Enrollment
{
    public int Id { get; set; }
    public int StudentId { get; set; }
    public int CourseId { get; set; }
    public DateTime EnrolledOn { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public Student Student { get; set; } = null!;
    public Course Course { get; set; } = null!;
}
```

---

## 3. EF Core DbContext & Migrations

### Write the DbContext
Create `AppDbContext.cs` inside `BridgeCourse.Data/`:

```csharp
using Microsoft.EntityFrameworkCore;
using BridgeCourse.Data.Entities;

namespace BridgeCourse.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Student> Students => Set<Student>();
    public DbSet<Course> Courses => Set<Course>();
    public DbSet<Enrollment> Enrollments => Set<Enrollment>();

    protected override void OnModelCreating(ModelCreatingBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Student configuration
        modelBuilder.Entity<Student>(entity =>
        {
            entity.HasKey(s => s.Id);
            entity.Property(s => s.Name).IsRequired().HasMaxLength(100);
            entity.Property(s => s.Email).IsRequired().HasMaxLength(150);
            entity.Property(s => s.PasswordHash).IsRequired();
            entity.Property(s => s.PasswordSalt).IsRequired();
        });

        // Course configuration
        modelBuilder.Entity<Course>(entity =>
        {
            entity.HasKey(c => c.Id);
            entity.Property(c => c.Code).IsRequired().HasMaxLength(10);
            entity.Property(c => c.Title).IsRequired().HasMaxLength(150);
        });

        // Enrollment configuration (Join Entity)
        modelBuilder.Entity<Enrollment>(entity =>
        {
            entity.HasKey(e => e.Id);

            // 1-to-Many: Student -> Enrollments
            entity.HasOne(e => e.Student)
                  .WithMany(s => s.Enrollments)
                  .HasForeignKey(e => e.StudentId)
                  .OnDelete(DeleteBehavior.Cascade);

            // 1-to-Many: Course -> Enrollments
            entity.HasOne(e => e.Course)
                  .WithMany(c => c.Enrollments)
                  .HasForeignKey(e => e.CourseId)
                  .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
```

### Suppressing pending warnings & running migrations
In EF Core 9.0+, if you have multiple contexts, suppress design-time warnings in your DI container. 

In `BridgeCourse.Api/Program.cs`:
```csharp
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
           .ConfigureWarnings(w => w.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.PendingModelChangesWarning)));
```

Run migrations from the solution directory:
```powershell
# Scaffold the initial migration
dotnet ef migrations add InitialCreate --project BridgeCourse.Data\BridgeCourse.Data.csproj --startup-project BridgeCourse.Api\BridgeCourse.Api.csproj --context AppDbContext

# Apply changes to the SQL Server database
dotnet ef database update --project BridgeCourse.Data\BridgeCourse.Data.csproj --startup-project BridgeCourse.Api\BridgeCourse.Api.csproj --context AppDbContext
```

---

## 4. The Repository Seam & Unit of Work (EF Core vs. ADO.NET)

To build a swappable data layer, we must isolate model operations behind a generic repository interface and a unit of work interface.

### The Interfaces
Create these inside `BridgeCourse.Data/Repositories/`:

```csharp
namespace BridgeCourse.Data.Repositories;

public interface IRepository<T> where T : class
{
    Task<IEnumerable<T>> GetAllAsync();
    Task<T?> GetByIdAsync(int id);
    Task AddAsync(T entity);
    void Update(T entity);
    void Delete(T entity);
}

public interface IUnitOfWork : IDisposable
{
    IRepository<Student> Students { get; }
    IRepository<Course> Courses { get; }
    Task<int> CompleteAsync(); // Returns rows affected
}
```

### EF Core Implementation
Create `EfRepository.cs` and `EfUnitOfWork.cs` in `BridgeCourse.Data/Repositories/`:

```csharp
using Microsoft.EntityFrameworkCore;

namespace BridgeCourse.Data.Repositories;

public class EfRepository<T> : IRepository<T> where T : class
{
    protected readonly AppDbContext _context;
    private readonly DbSet<T> _dbSet;

    public EfRepository(AppDbContext context)
    {
        _context = context;
        _dbSet = _context.Set<T>();
    }

    public async Task<IEnumerable<T>> GetAllAsync() => await _dbSet.ToListAsync();

    public async Task<T?> GetByIdAsync(int id) => await _dbSet.FindAsync(id);

    public async Task AddAsync(T entity) => await _dbSet.AddAsync(entity);

    public void Update(T entity) => _dbSet.Update(entity);

    public void Delete(T entity) => _dbSet.Remove(entity);
}

public class EfUnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;
    public IRepository<Student> Students { get; }
    public IRepository<Course> Courses { get; }

    public EfUnitOfWork(AppDbContext context)
    {
        _context = context;
        Students = new EfRepository<Student>(_context);
        Courses = new EfRepository<Course>(_context);
    }

    public async Task<int> CompleteAsync() => await _context.SaveChangesAsync();

    public void Dispose() => _context.Dispose();
}
```

### ADO.NET Implementation (Raw SQL)
To demonstrate swapping data layers, we must write standard raw SQL commands using `SqlConnection` and `SqlCommand`.

Create `AdoNetStudentRepository.cs` and `AdoNetUnitOfWork.cs` inside `BridgeCourse.Data/Repositories/`:

```csharp
using System.Data;
using Microsoft.Data.SqlClient;
using BridgeCourse.Data.Entities;

namespace BridgeCourse.Data.Repositories;

public class AdoNetStudentRepository : IRepository<Student>
{
    private readonly SqlConnection _connection;
    private readonly SqlTransaction? _transaction;

    public AdoNetStudentRepository(SqlConnection connection, SqlTransaction? transaction)
    {
        _connection = connection;
        _transaction = transaction;
    }

    public async Task<IEnumerable<Student>> GetAllAsync()
    {
        var students = new List<Student>();
        const string query = "SELECT Id, Name, Email, PasswordHash, PasswordSalt, CreatedAt FROM Students";

        using var command = new SqlCommand(query, _connection, _transaction);
        using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            students.Add(new Student
            {
                Id = reader.GetInt32(0),
                Name = reader.GetString(1),
                Email = reader.GetString(2),
                PasswordHash = reader.GetString(3),
                PasswordSalt = reader.GetString(4),
                CreatedAt = reader.GetDateTime(5)
            });
        }
        return students;
    }

    public async Task<T?> GetByIdAsync(int id) where T : class
    {
        const string query = "SELECT Id, Name, Email, PasswordHash, PasswordSalt, CreatedAt FROM Students WHERE Id = @Id";
        using var command = new SqlCommand(query, _connection, _transaction);
        command.Parameters.AddWithValue("@Id", id);

        using var reader = await command.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return new Student
            {
                Id = reader.GetInt32(0),
                Name = reader.GetString(1),
                Email = reader.GetString(2),
                PasswordHash = reader.GetString(3),
                PasswordSalt = reader.GetString(4),
                CreatedAt = reader.GetDateTime(5)
            } as T;
        }
        return null;
    }

    public async Task AddAsync(Student entity)
    {
        const string query = "INSERT INTO Students (Name, Email, PasswordHash, PasswordSalt, CreatedAt) VALUES (@Name, @Email, @PasswordHash, @PasswordSalt, @CreatedAt); SELECT SCOPE_IDENTITY();";
        using var command = new SqlCommand(query, _connection, _transaction);
        command.Parameters.AddWithValue("@Name", entity.Name);
        command.Parameters.AddWithValue("@Email", entity.Email);
        command.Parameters.AddWithValue("@PasswordHash", entity.PasswordHash);
        command.Parameters.AddWithValue("@PasswordSalt", entity.PasswordSalt);
        command.Parameters.AddWithValue("@CreatedAt", entity.CreatedAt);

        var id = await command.ExecuteScalarAsync();
        if (id != null)
        {
            entity.Id = Convert.ToInt32(id);
        }
    }

    public void Update(Student entity)
    {
        const string query = "UPDATE Students SET Name = @Name, Email = @Email, PasswordHash = @PasswordHash, PasswordSalt = @PasswordSalt WHERE Id = @Id";
        using var command = new SqlCommand(query, _connection, _transaction);
        command.Parameters.AddWithValue("@Id", entity.Id);
        command.Parameters.AddWithValue("@Name", entity.Name);
        command.Parameters.AddWithValue("@Email", entity.Email);
        command.Parameters.AddWithValue("@PasswordHash", entity.PasswordHash);
        command.Parameters.AddWithValue("@PasswordSalt", entity.PasswordSalt);
        command.ExecuteNonQuery();
    }

    public void Delete(Student entity)
    {
        const string query = "DELETE FROM Students WHERE Id = @Id";
        using var command = new SqlCommand(query, _connection, _transaction);
        command.Parameters.AddWithValue("@Id", entity.Id);
        command.ExecuteNonQuery();
    }
}

public class AdoNetUnitOfWork : IUnitOfWork
{
    private readonly SqlConnection _connection;
    private SqlTransaction? _transaction;
    public IRepository<Student> Students { get; }
    public IRepository<Course> Courses => throw new NotImplementedException("Course repository not implemented in ADO.NET");

    public AdoNetUnitOfWork(string connectionString)
    {
        _connection = new SqlConnection(connectionString);
        _connection.Open();
        _transaction = _connection.BeginTransaction();
        Students = new AdoNetStudentRepository(_connection, _transaction);
    }

    public async Task<int> CompleteAsync()
    {
        if (_transaction == null) throw new InvalidOperationException("Transaction has already been completed.");
        try
        {
            await _transaction.CommitAsync();
            return 1;
        }
        catch
        {
            await _transaction.RollbackAsync();
            throw;
        }
        finally
        {
            _transaction.Dispose();
            _transaction = null;
        }
    }

    public void Dispose()
    {
        _transaction?.Dispose();
        _connection.Dispose();
    }
}
```

---

## 5. Swapping Configuration in `Program.cs`

In `appsettings.json`, configure a string configuration switch representing the active data layer:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=BridgeCourseDb;Trusted_Connection=True;TrustServerCertificate=True;"
  },
  "DataLayer": "EfCore" // Or "AdoNet"
}
```

In `Program.cs`, resolve the services using conditional bindings:
```csharp
using BridgeCourse.Data;
using BridgeCourse.Data.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Read variables
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
    ?? throw new InvalidOperationException("Connection string not found.");
var dataLayer = builder.Configuration["DataLayer"] ?? "EfCore";

// Conditional service registration
if (dataLayer.Equals("AdoNet", StringComparison.OrdinalIgnoreCase))
{
    builder.Services.AddScoped<IUnitOfWork>(sp => new AdoNetUnitOfWork(connectionString));
}
else // Default to EF Core
{
    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseSqlServer(connectionString)
               .ConfigureWarnings(w => w.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.PendingModelChangesWarning)));

    builder.Services.AddScoped<IUnitOfWork, EfUnitOfWork>();
}

builder.Services.AddControllers();
var app = builder.Build();

app.MapControllers();
app.Run();
```

---

## 6. Middlewares & Global Exception Handling

A key hackathon task is mapping specific domain/database exceptions to appropriate HTTP status codes (e.g., throwing a `StudentNotFoundException` mapping to a `404 Not Found`).

### Define the Exception
```csharp
namespace BridgeCourse.Data.Exceptions;

public class StudentNotFoundException : Exception
{
    public int StudentId { get; }
    public StudentNotFoundException(int id) : base($"Student with ID {id} was not found.")
    {
        StudentId = id;
    }
}
```

### Write global handling middleware
Create `ExceptionHandlingMiddleware.cs` in `BridgeCourse.Api/Middlewares/`:
```csharp
using System.Net;
using System.Text.Json;
using BridgeCourse.Data.Exceptions;

namespace BridgeCourse.Api.Middlewares;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (StudentNotFoundException ex)
        {
            _logger.LogWarning(ex, "Entity lookup failed.");
            await WriteResponseAsync(context, HttpStatusCode.NotFound, ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception occurred.");
            await WriteResponseAsync(context, HttpStatusCode.InternalServerError, "An internal server error occurred.");
        }
    }

    private static async Task WriteResponseAsync(HttpContext context, HttpStatusCode code, string message)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)code;

        var result = JsonSerializer.Serialize(new { error = message, status = (int)code });
        await context.Response.WriteAsync(result);
    }
}
```

Register this middleware in `Program.cs` before controllers are mapped:
```csharp
app.UseMiddleware<ExceptionHandlingMiddleware>();
```

---

## 7. xUnit Integration Testing

During exams, write automated tests targeting your SLA strategies, exception handlers, and repository operations.

Create `StudentServiceTests.cs` in `BridgeCourse.Tests/`:
```csharp
using Microsoft.EntityFrameworkCore;
using BridgeCourse.Data;
using BridgeCourse.Data.Entities;
using BridgeCourse.Data.Repositories;
using BridgeCourse.Data.Exceptions;
using Xunit;

namespace BridgeCourse.Tests;

public class StudentServiceTests
{
    private AppDbContext GetInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        return new AppDbContext(options);
    }

    [Fact]
    public async Task AddStudent_ShouldPersistStudentToDatabase()
    {
        // Arrange
        using var context = GetInMemoryDbContext();
        using var unitOfWork = new EfUnitOfWork(context);
        var student = new Student { Name = "John Doe", Email = "john@example.com" };

        // Act
        await unitOfWork.Students.AddAsync(student);
        await unitOfWork.CompleteAsync();

        // Assert
        var dbStudent = await context.Students.FirstOrDefaultAsync(s => s.Name == "John Doe");
        Assert.NotNull(dbStudent);
        Assert.Equal("john@example.com", dbStudent.Email);
    }

    [Fact]
    public async Task GetById_OnMissingStudent_ShouldThrowStudentNotFoundException()
    {
        // Arrange
        using var context = GetInMemoryDbContext();
        using var unitOfWork = new EfUnitOfWork(context);

        // Act & Assert
        await Assert.ThrowsAsync<StudentNotFoundException>(async () =>
        {
            var student = await unitOfWork.Students.GetByIdAsync(999);
            if (student == null)
            {
                throw new StudentNotFoundException(999);
            }
        });
    }
}
```

---

## 8. File Handling Utilities (Read/Write/Upload)

Exams often ask you to save uploaded assets or parse text/CSV logs asynchronously.

```csharp
using Microsoft.AspNetCore.Http;

namespace BridgeCourse.Data.Utilities;

public static class FileUtility
{
    // 1. Asynchronously save an uploaded file (IFormFile) to a folder
    public static async Task<string> SaveUploadedFileAsync(IFormFile file, string targetFolder)
    {
        if (file == null || file.Length == 0)
            throw new ArgumentException("Invalid file upload.");

        if (!Directory.Exists(targetFolder))
            Directory.CreateDirectory(targetFolder);

        var uniqueFileName = $"{Guid.NewGuid()}_{Path.GetFileName(file.FileName)}";
        var filePath = Path.Combine(targetFolder, uniqueFileName);

        using var stream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None, 4096, useAsync: true);
        await file.CopyToAsync(stream);

        return filePath;
    }

    // 2. Read contents of a text file asynchronously
    public static async Task<string> ReadTextFileAsync(string filePath)
    {
        if (!File.Exists(filePath))
            throw new FileNotFoundException("Target file not found.", filePath);

        using var reader = new StreamReader(filePath);
        return await reader.ReadToEndAsync();
    }

    // 3. Write contents to a file asynchronously
    public static async Task WriteTextFileAsync(string filePath, string content)
    {
        var folder = Path.GetDirectoryName(filePath);
        if (!string.IsNullOrEmpty(folder) && !Directory.Exists(folder))
            Directory.CreateDirectory(folder);

        using var writer = new StreamWriter(filePath, append: false);
        await writer.WriteAsync(content);
    }
}
```

---

## 9. Offline Password Hashing & Salting (PBKDF2)

For closed-book, offline exams where you cannot install NuGet packages like `BCrypt`, use the built-in **`Rfc2898DeriveBytes`** class from `System.Security.Cryptography`.

Create `PasswordHasher.cs` inside `BridgeCourse.Data.Utilities`:

```csharp
using System.Security.Cryptography;

namespace BridgeCourse.Data.Utilities;

public static class PasswordHasher
{
    private const int SaltSize = 16; // 128-bit
    private const int KeySize = 32;  // 256-bit
    private const int Iterations = 100000; // PBKDF2 iteration count
    private static readonly HashAlgorithmName HashAlgorithm = HashAlgorithmName.SHA256;

    // Hashes a plain-text password and returns the Hash and Salt as Base64 strings
    public static (string hash, string salt) HashPassword(string password)
    {
        // 1. Generate a cryptographically secure random salt
        byte[] saltBytes = RandomNumberGenerator.GetBytes(SaltSize);

        // 2. Hash the password with the salt using PBKDF2
        using var pbkdf2 = new Rfc2898DeriveBytes(password, saltBytes, Iterations, HashAlgorithm);
        byte[] hashBytes = pbkdf2.GetBytes(KeySize);

        // 3. Return both as base64-encoded strings for database storage
        return (Convert.ToBase64String(hashBytes), Convert.ToBase64String(saltBytes));
    }

    // Verifies a plain-text password against a stored Hash and Salt
    public static bool VerifyPassword(string password, string storedHash, string storedSalt)
    {
        byte[] saltBytes = Convert.FromBase64String(storedSalt);
        byte[] hashBytes = Convert.FromBase64String(storedHash);

        // 1. Re-hash the incoming password using the original salt
        using var pbkdf2 = new Rfc2898DeriveBytes(password, saltBytes, Iterations, HashAlgorithm);
        byte[] testHashBytes = pbkdf2.GetBytes(KeySize);

        // 2. Compare the resulting hash bytes (Cryptographic safe compare)
        return CryptographicOperations.FixedTimeEquals(hashBytes, testHashBytes);
    }
}
```
