# Week 2 Mastery Guide: ASP.NET Core Web API & Design Patterns

This guide is designed to help you master all the core concepts of ASP.NET Core Web APIs and modern backend design patterns. Each section contains a real-world analogy, production-ready code examples, deep-dive explanations, and a curated list of "twisted" cohort/interview questions to ensure you cannot be caught off guard.

---

## 1. Web API Project Anatomy & Middleware

### The Real-World Analogy: The Theme Park Entrance
Imagine entering a secure theme park:
1. **Checkpoint 1 (Exception Handler):** A paramedic team sits at the gate. If anyone collapses anywhere in the park, they handle it.
2. **Checkpoint 2 (HTTPS Redirection):** Security forces everyone to walk through a well-lit, paved secure pathway rather than the dark, muddy side alley.
3. **Checkpoint 3 (Routing):** A signpost tells you where different rides are.
4. **Checkpoint 4 (Authorization):** A ticket scanner checks if you have a valid pass before letting you onto the rides.
5. **Checkpoint 5 (Endpoint Execution):** You actually get on the roller coaster (the Controller).

If you swap the order—for example, putting the ticket scanner (Authorization) *before* the signpost (Routing)—security won't know which ride you are trying to access, and the system collapses!

---

### Production-Ready Code Example (`Program.cs`)
```csharp
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

// --- 1. Service Registration (Dependency Injection Container) ---
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// --- 2. Middleware Pipeline (Order of Execution Matters!) ---
if (app.Environment.IsDevelopment())
{
    // Exception boundary for development details
    app.UseDeveloperExceptionPage();
    
    // API documentation endpoints
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    // Global production exception filter
    app.UseExceptionHandler("/error");
}

app.UseHttpsRedirection(); // Redirect HTTP -> HTTPS

app.UseRouting(); // Matches request path to a controller endpoint

app.UseAuthorization(); // Checks access policies

app.MapControllers(); // Dispatches request to the matched controller action

app.Run();
```

---

### Deep-Dive Mechanics
* **Composition Root:** `Program.cs` is the entry point of the app. It registers all services in the DI container and maps the HTTP middleware pipeline.
* **Middleware Pipeline:** A chain of request delegates executed sequentially. Each middleware can either pass the request to the next block (`_next(context)`) or short-circuit the request (e.g., return `401 Unauthorized` directly).

---

### ⚠️ Twisted Cohort/Interview Questions

#### Q1. What happens if you swap `app.UseRouting()` and `app.UseAuthorization()` in the pipeline?
* **Answer:** The application will fail or throw an exception at startup. `UseAuthorization` requires endpoint metadata to know which access policies apply to the requested route. Because `UseRouting` is responsible for matching the URL to endpoint metadata, running authorization *before* routing means the endpoint context is null, preventing authorization from running correctly.

#### Q2. What is the difference between a Service and a Middleware in ASP.NET Core?
* **Answer:** 
  * A **Service** is a class registered in the DI container to perform specific business operations (e.g., database access, DTO mapping). Services are injected and called on demand.
  * A **Middleware** is a component built into the request-response pipeline. It intercept every single HTTP request and response passing through the application.

---

## 2. Controllers & Routing (REST CRUD Conventions)

### The Real-World Analogy: The Corporate Mailroom
Think of a mailroom handling packages:
* The address label `api/students/5` is read by the mailroom worker.
* The wrapper attribute `[Route("api/[controller]")]` directs it to the **Students** department.
* The stamp `GET` or `POST` determines what they do with it: `GET` means "fetch student file #5", while `POST` means "log this new student document."

---

### Production-Ready Code Example
```csharp
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

[ApiController]
[Route("api/[controller]")] // Route matches: api/students
public class StudentsController : ControllerBase
{
    // GET: api/students
    [HttpGet]
    public ActionResult<IEnumerable<string>> GetAll()
    {
        return Ok(new[] { "Alice", "Bob" });
    }

    // GET: api/students/5
    [HttpGet("{id:int}")] // Constraint enforces integer id
    public ActionResult<string> GetById(int id)
    {
        if (id != 5) return NotFound();
        return Ok("Alice");
    }

    // POST: api/students
    [HttpPost]
    public IActionResult Create([FromBody] string name)
    {
        // 201 Created returns location header matching GET url: api/students/6
        return CreatedAtAction(nameof(GetById), new { id = 6 }, name);
    }
}
```

---

### Deep-Dive Mechanics
* `ControllerBase` vs `Controller`: Web APIs inherit from `ControllerBase`. The base `Controller` class includes support for views and webpages (Razor), which is unnecessary overhead for REST endpoints that return JSON.
* `[ApiController]`: Enables REST-specific behaviors:
  1. Enforces Attribute Routing (conventional routes in `Program.cs` like `{controller}/{action}` are ignored).
  2. Automatic `400 Bad Request` validation returns (no need to manually write `if (!ModelState.IsValid)`).
  3. Automatic binding inference (`[FromBody]` for complex types, `[FromQuery]` for primitives).

---

### ⚠️ Twisted Cohort/Interview Questions

#### Q1. When should you return `204 No Content` vs `200 OK` from a PUT or DELETE request?
* **Answer:** 
  * Return `204 No Content` when the resource has been successfully updated or deleted, and the client does not need any updated payload back to save network bandwidth.
  * Return `200 OK` (with the updated resource in the body) if the client needs to see the final server-side computed state of the resource after updates.

#### Q2. Why is using `CreatedAtAction` preferred over returning a basic `200 OK` on resource creation?
* **Answer:** `CreatedAtAction` adheres strictly to the HTTP/1.1 specification for a `201 Created` status code. It automatically inserts a `Location` header in the HTTP response pointing to the GET URL of the newly created resource, enabling clients to immediately query the resource state without hardcoding URLs.

---

## 3. Dependency Injection (DI) Lifetimes

### The Real-World Analogy: The Hotel Amenities
1. **Transient (Single-use Paper Cup):** Every time you want a drink, you grab a brand new cup, drink, and throw it away. (Created on every request/resolution).
2. **Scoped (Your Table Waiter):** The waiter serves you for your entire dinner. Once you finish your meal and pay (the HTTP request ends), the waiter is assigned to a new table. (Created once per HTTP request).
3. **Singleton (The Hotel Lobby):** There is only one lobby. Every guest shares the exact same lobby throughout their stay. (Created once when the app starts and shared globally).

---

### Production-Ready Lifetimes Registration
```csharp
// Program.cs
builder.Services.AddTransient<ITransientService, TransientService>();
builder.Services.AddScoped<IScopedService, ScopedService>();
builder.Services.AddSingleton<ISingletonService, SingletonService>();
```

---

### Deep-Dive Mechanics
* **Transient:** Best for lightweight, stateless services (e.g., helpers, mathematical calculation classes).
* **Scoped:** Best for services that hold state during a single request (e.g., Entity Framework database contexts, business services).
* **Singleton:** Best for long-lived application cache, config systems, or thread-safe shared resources.

---

### ⚠️ Twisted Cohort/Interview Questions

#### Q1. What is a "Captive Dependency" and why is it dangerous?
* **Answer:** A **Captive Dependency** occurs when a service with a longer lifetime injects a service with a shorter lifetime.
  * *Example:* A **Singleton** service injects a **Scoped** service.
  * *Why it's dangerous:* Because the Singleton instance lives forever, the Scoped service injected into its constructor is kept alive ("captured") forever inside the Singleton. This bypasses the Scoped service's intended disposal boundary, leading to database connection pool exhaustion or memory leaks.

#### Q2. How does ASP.NET Core protect you from Captive Dependencies?
* **Answer:** In `Development` environments, the ASP.NET Core DI container automatically performs scope validation check at startup. If a Singleton tries to resolve a Scoped dependency, it immediately throws an `InvalidOperationException` stating: *"Cannot resolve scoped service from root provider."*

---

## 4. Model Validation & ApiController

### The Real-World Analogy: Airport Check-in Gate
Before boarding, a gate agent checks your passport, bags, and ticket:
* If your name is blank (`[Required]`), or your passport is expired (`Range` validation), you are turned back immediately.
* The pilot (the Controller action) doesn't have to leave the cockpit to check your papers; the gate agent (`[ApiController]`) intercepts you before you can step onto the plane.

---

### Production-Ready Code Example
```csharp
using System.ComponentModel.DataAnnotations;

public class StudentCreateDto
{
    [Required(ErrorMessage = "Name is mandatory.")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "Name must be between 2 and 50 characters.")]
    public string Name { get; set; } = string.Empty;

    [Range(5, 100, ErrorMessage = "Age must be between 5 and 100.")]
    public int Age { get; set; }

    [Required]
    [EmailAddress(ErrorMessage = "Invalid email format.")]
    public string Email { get; set; } = string.Empty;
}
```

---

### Deep-Dive Mechanics
* **Automatic Validation:** When `[ApiController]` is present on a controller, the framework automatically checks `ModelState.IsValid`. If invalid, it short-circuits the request and returns a `400 Bad Request` containing a structured `ValidationProblemDetails` JSON body.
* **Manual Check (Fallback):** If `[ApiController]` is missing, validation attributes do NOT throw automatically. You must write:
  ```csharp
  if (!ModelState.IsValid)
  {
      return BadRequest(ModelState);
  }
  ```

---

### ⚠️ Twisted Cohort/Interview Questions

#### Q1. If a client submits a payload containing a property type mismatch (e.g. string "abc" for an integer `Age` field), does it reach the controller code?
* **Answer:** No. Model binding occurs before validation. When the binder fails to parse "abc" to an integer, it registers a binding failure error in `ModelState`. The `[ApiController]` filter detects this failure, blocks execution, and returns a `400 Bad Request` outlining the parsing error.

---

## 5. DTOs (Data Transfer Objects) vs. Domain Entities

### The Real-World Analogy: Security Clearance Resume
* **Domain Entity (Full Profile):** Contains social security numbers, private background check logs, and payroll keys.
* **DTO (The Resume):** Contains only your name, email, and skills. You show the resume to external clients, keeping your private profile hidden.

---

### Production-Ready DTO Mapping
```csharp
// The Database Entity
public class Student
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string InternalNotes { get; set; } = string.Empty; // Private field!
}

// The Read DTO (Response Payload)
public class StudentReadDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}

// Inside the Service:
public StudentReadDto MapToReadDto(Student entity)
{
    return new StudentReadDto
    {
        Id = entity.Id,
        Name = entity.Name,
        Email = entity.Email
        // InternalNotes is left out, preventing leaks!
    };
}
```

---

### Deep-Dive Mechanics
* **Overposting Attack (Mass Assignment):** If you bind input payloads directly to a Domain Entity in a POST/PUT endpoint (e.g., `public IActionResult Create(Student student)`), a malicious client can add extra fields in the JSON payload (such as `IsAdmin: true` or `Salary: 10000000`). If mapped directly to the database context, the database updates these fields without warning.
* **Mitigation:** Always use a specific DTO (e.g. `StudentCreateDto`) that only exposes properties safe for the client to change.

---

### ⚠️ Twisted Cohort/Interview Questions

#### Q1. Can overposting occur on a GET request? Why or why not?
* **Answer:** No. Overposting is a write vulnerability where clients send unsolicited properties to modify state. Since GET requests are read-only and do not accept a request body to mutate the database, overposting cannot occur on GET requests. (However, DTOs are still required on GET to prevent database information leakage).

---

## 6. Swappable Patterns: Strategy & Factory

### The Real-World Analogy: The Smart Payment Terminal
When you checkout at a store, the cash register doesn’t care whether you swipe a Visa card, scan a UPI QR code, or use Apple Pay. The register presents a standard payment interface (Strategy Interface). Once you choose your payment method (The Factory), the terminal changes its behavior to run the chosen algorithm.

---

### Production-Ready Code Example

#### 1. Define Strategy & Implementations
```csharp
public interface IGradeStrategy
{
    string FormatGrade(decimal score);
}

public class PercentageGradeStrategy : IGradeStrategy
{
    public string FormatGrade(decimal score) => $"{score:F1}%";
}

public class GpaGradeStrategy : IGradeStrategy
{
    public string FormatGrade(decimal score) => $"{ (score / 100m) * 4.0m :F2} GPA";
}
```

#### 2. The Factory (Resolving Strategy dynamically from Request Context)
```csharp
using Microsoft.AspNetCore.Http;

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
        if (context != null && context.Request.Headers.TryGetValue("X-Grade-Format", out var headerVal))
        {
            if (headerVal.ToString().Equals("GPA", System.StringComparison.OrdinalIgnoreCase))
            {
                return new GpaGradeStrategy();
            }
        }
        return new PercentageGradeStrategy(); // Default fallback
    }
}
```

---

### Deep-Dive Mechanics
* **Decoupling controllers:** The controller simply injects the service, and the service resolves the strategy dynamically.
* **Unit Testability:** Because the strategies are decoupled, you can write isolated test assertions checking that `GpaGradeStrategy` converts `90` to `3.60 GPA` and `PercentageGradeStrategy` formats it to `90.0%`, without needing to run an HTTP server or a web browser.

---

### ⚠️ Twisted Cohort/Interview Questions

#### Q1. Why use a Strategy Factory instead of passing the strategy name as a string parameter in our business service method (e.g. `service.GetAll("GPA")`)?
* **Answer:** Passing the strategy name as a string violates the Open/Closed Principle (OCP) and leads to code duplication across services. By isolating strategy resolution to a centralized `GradeStrategyFactory` that accesses the `HttpContext` directly, business services stay completely stateless, cohesive, and unaware of raw HTTP headers, making the architecture modular.
