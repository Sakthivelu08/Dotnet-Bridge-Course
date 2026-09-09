# Week 4 Concepts Walkthrough: Database Access & Swappable Layers

This document provides a comprehensive guide to database access in C# and .NET 9.0, focusing on ADO.NET, Entity Framework Core Code First, Database First, and configurations-driven swappable data layers.

---

## 1. Database Design & Normalization (3NF)

### The Concept
A database design should be normalized to at least Third Normal Form (3NF) to eliminate data redundancy and anomalies:
* **1NF (First Normal Form):** Every cell contains a single, atomic value; no repeating groups.
* **2NF (Second Normal Form):** Meets 1NF, and all non-key columns depend entirely on the primary key (no partial dependencies).
* **3NF (Third Normal Form):** Meets 2NF, and no non-key columns depend transitively on another non-key column (no transitive dependencies).

### Production Example (Schema Mapping)
For our Student management system, the schema contains primary keys, foreign constraints (optional when linking courses), and indexing on frequently queried columns (such as Student `Email`).

---

## 2. ADO.NET: Raw SQL, Stored Procedures, and Injection Defense

### Core Objects
* **SqlConnection:** Represents an active connection session to a SQL Server database. Must always be closed/disposed immediately to return the connection back to the OS connection pool.
* **SqlCommand:** Represents an executable SQL statement or Stored Procedure name.
* **SqlDataReader:** A high-speed, forward-only stream of data rows read from the database. Low-level, read-only, and memory-efficient.
* **DataAdapter & DataSet:** Provides disconnected data representation (reads all rows into a in-memory cache/DataSet). Uses more RAM but supports offline updates.

### SQL Injection Defenses (Parameterization)
* **Vulnerable SQL (String Concatenation):**
  ```csharp
  // VULNERABLE: If input is: '; DROP TABLE Students;--
  string query = "SELECT * FROM Students WHERE Name = '" + inputName + "'";
  ```
* **Secure SQL (Parameterized Query):**
  ```csharp
  string query = "SELECT * FROM Students WHERE Name = @Name";
  command.Parameters.AddWithValue("@Name", inputName);
  ```
  * **How it works:** SQL Server treats the parameter value strictly as **data** (literal text), compile-checking the SQL command structure *before* applying parameter values, completely neutralizing SQL injections.

### Stored Procedures
Precompiled sets of SQL statements stored on the database server. They reduce network traffic and improve execution speeds.
```csharp
using (var command = new SqlCommand("usp_GetStudentById", connection))
{
    command.CommandType = CommandType.StoredProcedure;
    command.Parameters.AddWithValue("@Id", id);
    // Execute...
}
```

---

## 3. Entity Framework Core: Code First & Migrations

### DbContext & Fluent API
The `DbContext` represents the session with the database. While we can use attributes (Data Annotations like `[Required]`, `[MaxLength]`) directly on our models, the **Fluent API** configuration inside `OnModelCreating` is preferred in enterprise projects because it keeps entities clean of data-access details.

### Migrations Workflow
Migrations keep your C# model definitions in sync with the database tables.
1. `dotnet ef migrations add <MigrationName>`: Compares your C# entities with the model snapshot and generates the C# migration files.
2. `dotnet ef database update`: Applies any pending migrations to the physical database server, updating the schema.

---

## 4. Entity Framework Core: Database First & The Adapter Seam

### Scaffolding (Scaffold-DbContext)
DB First reverses Code First: it reads the metadata of an existing database and generates C# entities and a `DbContext` matching the schema.
```bash
dotnet ef dbcontext scaffold "Server=(localdb)\MSSQLLocalDB;Database=BridgeCourseDb;Trusted_Connection=True;" Microsoft.EntityFrameworkCore.SqlServer -o Data/EfDbFirst/Models
```

### The Adapter Pattern Seam
Scaffolded entities contain database-specific properties and structure. If we expose scaffolded classes directly to our API controllers, we break the "seam" — making our controllers depend on database details.
* **Solution:** We map scaffolded entities to our shared clean domain models inside the repository layer (using the **Adapter Pattern**). The API controllers only interact with clean domain models.

---

## 5. Swappable Configurations & Dependency Injection (DI)

### How it works
By defining a configuration key in `appsettings.json` (e.g. `"DataLayer": "EfCodeFirst"`), we can select which repository registration runs inside `Program.cs` conditionally:
```csharp
var dataLayer = builder.Configuration["DataLayer"];
if (dataLayer == "AdoNet")
{
    builder.Services.AddScoped<IRepository<Student>, AdoNetStudentRepository>();
}
else if (dataLayer == "EfDbFirst")
{
    builder.Services.AddScoped<IRepository<Student>, DbFirstStudentRepository>();
}
else
{
    builder.Services.AddScoped<IRepository<Student>, EfStudentRepository>();
}
```
Changing a single configuration string swaps the entire database access pipeline without touching a single line of controller code!

---

## ⚠️ Twisted Cohort Questions (Q&A Prep)

1. **Why do we call `GC.SuppressFinalize(this)` inside IDisposable.Dispose()?**
   * *Answer:* If we manually dispose unmanaged resources (like closing SQL connections), running the class's finalizer (~destructor) is redundant. Calling `SuppressFinalize` informs the Garbage Collector to skip the finalization queue, allowing the memory to be reclaimed instantly in one cycle.
2. **What is the difference between `SqlDataReader` and `SqlDataAdapter`?**
   * *Answer:* `SqlDataReader` is a high-speed, forward-only, read-only stream (connected mode). It requires keeping the connection open while reading. `SqlDataAdapter` reads all rows into an in-memory `DataSet` (disconnected mode), closing the connection immediately but consuming more RAM.
3. **If a database table is updated externally, what happens to EF Code First vs. DB First?**
   * *Answer:* 
     * In **Code First**, external schema changes break synchronization. You must create migrations to sync them or risk runtime errors.
     * In **DB First**, you simply re-scaffold the DbContext using `Scaffold-DbContext` to regenerate code models instantly.
