# Week 2 Concepts: MCQ & Assessment Prep

This document contains conceptual multiple-choice questions (MCQs), code-based troubleshooting scenarios, and high-frequency technical interview questions mapping to Week 2 of the Dotnet Bridge Course.

---

## Part 1: Conceptual & Output-Based MCQs

### Topic 1: API Routing & Controllers (Day 1)

#### Q1. A developer creates a controller decorated as follows:
```csharp
[ApiController]
[Route("api/[controller]")]
public class StudentsController : ControllerBase
{
    [HttpGet("{id}")]
    public IActionResult GetStudent(int id) { ... }
}
```
If the application is running locally on port 5000, what is the correct URL route to access a student with ID 12?
* **A)** `http://localhost:5000/Students/12`
* **B)** `http://localhost:5000/api/StudentsController/12`
* **C)** `http://localhost:5000/api/Students/12`
* **D)** `http://localhost:5000/api/Students?id=12`

<details>
<summary><b>Answer & Explanation</b></summary>

**Answer: C**

**Explanation:** In ASP.NET Core MVC routing, `[controller]` acts as a token that is automatically replaced with the class name minus the "Controller" suffix. Therefore, `StudentsController` resolves to `students` in the path. The route parameter `{id}` requires the parameter value to be appended in the path itself, resulting in `api/students/12`.
</details>

---

### Topic 2: Dependency Injection Lifetimes (Day 2)

#### Q2. You have registered an in-memory list repository (`InMemoryRepository`) inside `Program.cs`. Which registration lifetime guarantees that additions to the list are retained across subsequent HTTP requests?
* **A)** `builder.Services.AddTransient<IRepository, InMemoryRepository>();`
* **B)** `builder.Services.AddScoped<IRepository, InMemoryRepository>();`
* **C)** `builder.Services.AddSingleton<IRepository, InMemoryRepository>();`
* **D)** Lifetimes do not affect state persistence in memory.

<details>
<summary><b>Answer & Explanation</b></summary>

**Answer: C**

**Explanation:** `AddSingleton` creates only one instance of the registered class that lives for the entire duration of the web server. Therefore, any data written to an in-memory list inside a Singleton persists. `Transient` and `Scoped` dispose of their instances at the end of their respective scopes (per-resolve or per-HTTP request), causing the list to be re-instantiated empty on the next call.
</details>

---

### Topic 3: HTTP Status Codes & REST Conventions (Day 3)

#### Q3. According to REST standards, what is the best status code to return from a `POST /api/students` request when a new student is successfully created?
* **A)** `200 OK`
* **B)** `201 Created`
* **C)** `202 Accepted`
* **D)** `204 No Content`

<details>
<summary><b>Answer & Explanation</b></summary>

**Answer: B**

**Explanation:** `201 Created` indicates that a request has succeeded and led to the creation of a resource. In ASP.NET Core, it is returned using `CreatedAtAction` or `CreatedAtRoute`, which automatically appends a `Location` header to the response, telling the client where they can fetch the newly created resource.
</details>

---

#### Q4. What happens when a client submits an invalid request body (e.g. invalid email format) to an endpoint decorated with `[ApiController]` and having validation attributes?
* **A)** The controller execution fails with a `NullReferenceException`.
* **B)** The framework automatically intercepts the request and returns a `400 Bad Request` before the controller action is executed.
* **C)** The controller code runs, but the parameters are loaded as `null`.
* **D)** The app returns a `500 Internal Server Error`.

<details>
<summary><b>Answer & Explanation</b></summary>

**Answer: B**

**Explanation:** The `[ApiController]` attribute modifies the pipeline behavior of ASP.NET Core by adding an automatic action filter. This filter checks if `ModelState.IsValid` is false. If invalid, it immediately blocks execution and returns a detailed `400 Bad Request` error response describing the validation failure, keeping your controllers thin and clean.
</details>

---

### Topic 4: DTOs & Applied Design Patterns (Day 4)

#### Q5. Why should you map domain Entities to DTOs (Data Transfer Objects) before returning them in API responses?
* **A)** Because entities cannot be serialized to JSON.
* **B)** To prevent internal details (like audit fields, passwords, or internal notes) from leaking over the network, and to shape responses specifically for the client's needs.
* **C)** DTOs run faster than database entities.
* **D)** It is a mandatory requirement enforced by compiler type checks.

<details>
<summary><b>Answer & Explanation</b></summary>

**Answer: B**

**Explanation:** Entities are mapped closely to database storage schemas. Exposing them directly can lead to over-posting vulnerabilities or leak sensitive internal-only details (e.g., system columns or `InternalNotes`). DTOs establish a clear contract separating internal structures from external shapes.
</details>

---

## Part 2: Code Troubleshooting Scenarios

### Scenario A: The Disappearing Data Bug
A developer complains that they add students using `POST /api/students`, receive a `201 Created` success code, but when they run `GET /api/students`, the returned array is empty. 
What is the most likely registration bug in `Program.cs`?

```csharp
builder.Services.AddScoped<IStudentRepository, InMemoryStudentRepository>();
builder.Services.AddScoped<IStudentService, StudentService>();
```

* **Analysis:** Because the repository is registered as `Scoped`, a new instance of `InMemoryStudentRepository` is created at the start of each HTTP request and destroyed at the end. The state of the in-memory list is lost between calls.
* **Fix:** The data store (repository context) must be registered as a `Singleton`:
  ```csharp
  builder.Services.AddSingleton<IStudentRepository, InMemoryStudentRepository>();
  ```

---

### Scenario B: The Location Header Null Exception
When creating a student, a developer gets a server crash with an exception: `No route matches the supplied values`.
```csharp
[HttpPost]
public IActionResult CreateStudent([FromBody] StudentCreateDto dto)
{
    var readDto = _service.Add(dto);
    return CreatedAtAction("GetStudentById", new { id = readDto.Id }, readDto);
}
```
* **Analysis:** The `CreatedAtAction` helper takes the name of the GET action method as its first argument to build the Location header. If the action method is named `GetById` but the string `"GetStudentById"` is passed, routing fails to resolve it at runtime and throws an error.
* **Fix:** Match the exact C# method name of your GET endpoint, or use `nameof`:
  ```csharp
  return CreatedAtAction(nameof(GetById), new { id = readDto.Id }, readDto);
  ```
