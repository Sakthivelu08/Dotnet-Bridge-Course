# Week 4 MCQ Practice & Edge Cases Preparation

This practice document contains tricky multiple-choice questions, database design edge cases, and architectural scenarios related to database access in C# (ADO.NET, EF Core Code First & DB First).

---

## Part 1: Multiple-Choice Questions (Self-Assessment)

### Q1: What database anomaly is eliminated by transitioning from 2NF to 3NF?
1. Partial functional dependency (non-key attributes depending on only part of a composite key).
2. Transitive dependency (non-key attributes depending on other non-key attributes).
3. Repeating values (non-atomic cells).
4. Duplicate tables.
* **Correct Answer: 2**
* **Explanation:** 3NF specifically removes transitive dependencies. For example, if a table has `StudentId -> MajorId -> MajorName`, the `MajorName` depends transitively on `StudentId` through `MajorId`. Moving `MajorId` and `MajorName` to a separate table satisfies 3NF.

---

### Q2: Why is it critical to wrap ADO.NET connections (`SqlConnection`) in `using` statements?
1. Because the compiler will refuse to build the project without it.
2. It encrypts the connection stream automatically.
3. It guarantees that the connection is closed and returned to the ADO.NET connection pool even if an exception is thrown.
4. It speeds up the raw SQL queries.
* **Correct Answer: 3**
* **Explanation:** ADO.NET connection pooling maintains a pool of active database connections. If connections are not closed/disposed, they remain open ("leak"), eventually exhausting the pool and causing subsequent database connection attempts to fail or timeout.

---

### Q3: When using ADO.NET, which method should you call on `SqlCommand` to execute a SELECT query that returns multiple rows?
1. `ExecuteNonQuery()`
2. `ExecuteScalar()`
3. `ExecuteReader()`
4. `ExecuteDataSet()`
* **Correct Answer: 3**
* **Explanation:** `ExecuteReader()` returns a `SqlDataReader` for streaming rows. `ExecuteNonQuery()` returns the number of affected rows (used for INSERT/UPDATE/DELETE). `ExecuteScalar()` returns the first column of the first row (used for COUNT or ID lookups).

---

### Q4: In Entity Framework Core Code First, how does the `HasData` method seed data into the database?
1. It reads from a CSV file during startup.
2. It executes direct SQL commands during the first database query.
3. It embeds the seed data directly into the generated C# migration files, applying it during `database update`.
4. It calls a stored procedure automatically.
* **Correct Answer: 3**
* **Explanation:** Seeding with Fluent API's `HasData` links the seed data to the DbContext model. When you generate a migration, the CLI generates standard INSERT statements for those entities, applying them in sequence when updating the database.

---

### Q5: What is the main design advantage of using the Adapter Pattern in an EF DB First project?
1. It speeds up database queries by 20%.
2. It isolates the scaffolded entities (owned by database schema) from the clean, core domain models used by the API.
3. It automatically updates the database schema when C# classes change.
4. It encrypts the connection strings in appsettings.json.
* **Correct Answer: 2**
* **Explanation:** Scaffolding databases generated from existing schemas might include database-specific columns (like audit logs or legacy names). Mapping them to clean domain models inside the repository keeps controllers and services independent of schema structural details (decoupling).

---

### Q6: If your database context (`DbContext`) is registered with the default "Scoped" lifetime in ASP.NET Core, what happens if two concurrent HTTP requests call the API?
1. They share the same DbContext instance, causing thread-safety crashes.
2. Each HTTP request gets its own isolated DbContext instance.
3. The API blocks the second request until the first one is completed.
4. One DbContext instance is used for the entire application lifetime.
* **Correct Answer: 2**
* **Explanation:** Scoped lifetime means ASP.NET Core creates a new instance of the service once per client HTTP request. This ensures that concurrent requests run in isolation, preventing multi-threading conflicts inside EF Core's non-thread-safe DbContext change tracker.

---

### Q7: What EF Core CLI command applies all pending migrations to the SQL Server database?
1. `dotnet ef migrations add`
2. `dotnet ef database update`
3. `dotnet ef dbcontext scaffold`
4. `dotnet ef database migration apply`
* **Correct Answer: 2**
* **Explanation:** `database update` compiles the project, checks the `__EFMigrationsHistory` table in SQL Server, and runs the `Up` methods of any migration files that have not yet been applied.

---

## Part 2: Critical Edge Cases & Troubleshooting

### 1. The Captive Dependency Trap (DI Lifetimes)
* **Edge Case:** Injecting a Scoped dependency (like `DbContext`) into a Singleton service.
* **The Problem:** The Singleton outlives the scope. The injected DbContext will never be disposed, causing memory leaks, thread safety issues, and database handle exhaustion.
* **Rule:** If a Singleton needs a DbContext, it should resolve it dynamically using a scoped factory (`IServiceScopeFactory`) inside its execution methods, disposing it immediately after.

### 2. Transaction Scope and Unit of Work
* **Edge Case:** Running multiple repository `Add`/`Update` calls in a single HTTP request, but calling `SaveChanges()` inside each repository instead of coordinating it.
* **The Problem:** If the third operation fails, the first two are already committed, leaving the database in an inconsistent state (partial transaction updates).
* **Rule:** Always call `SaveChanges()` (or `UnitOfWork.Save()`) **once** at the very end of the service execution block to ensure all changes succeed or fail together as a single unit (Atomicity).

### 3. AsNoTracking() for Read-only Paths
* **Edge Case:** Querying thousands of rows for list reports.
* **Optimization:** Always chain `.AsNoTracking()` to read-only queries. This tells EF Core to skip tracking changes on these entities, reducing RAM consumption and query execution times significantly.
