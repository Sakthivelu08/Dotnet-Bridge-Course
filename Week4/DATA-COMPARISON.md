# Data Access Layer Comparison: ADO.NET vs. EF Core Code First vs. EF Core DB First

This document analyzes the three data access approaches implemented in Week 4, outlining their strengths, trade-offs, and optimal use cases based on real-world engineering constraints.

---

## 1. stable security table (e.g. Users / Roles)
### Optimal Choice: **ADO.NET (Raw SQL Parameterized Queries)**
* **Justification:** Security components like authentication stores (e.g., verifying user credentials, password hash matches, or OAuth metadata) are highly critical, static in schema, and require absolute protection against performance side-channels (like timing attacks) or query overhead.
* **Why it fits:** 
  * **Zero Overhead:** ADO.NET bypasses the Entity Framework Core change tracker, query execution plan generation, and model metadata inspection, guaranteeing the fastest possible authentication checks.
  * **Precision:** Parameterized queries or stored procedures allow you to control the exact execution plan and parameters on SQL Server, ensuring optimal index usage.
  * **Minimal Footprint:** A simple user validation query does not require loading an entire ORM model framework, preventing dependency-related vulnerabilities in low-level security middleware.

---

## 2. an evolving domain (e.g. Student Registration / LMS Core)
### Optimal Choice: **EF Core Code First (Migrations-Driven)**
* **Justification:** During rapid product development, the domain model changes frequently (e.g., adding properties like `EnrolledOn`, modifying relationships, or updating validation constraints).
* **Why it fits:**
  * **Single Source of Truth:** You write standard C# classes, and EF Core handles the SQL schema synchronization. There is no need to write SQL script files manually.
  * **Migrations Versioning:** Schema changes are checked into Git as C# migration files, providing a clear history of how the database evolved from v1 to v2. This enables automated, zero-downtime database upgrades during CI/CD deployments.
  * **Fluent API Power:** Allows you to declare complex database constraints, cascading delete behaviors, and seed data directly in C# using strongly-typed APIs.

---

## 3. a reporting layer over a database you don't own (e.g. ERP / Legacy Systems)
### Optimal Choice: **EF Core Database First (Scaffold-DbContext)**
* **Justification:** When writing reports or analytical queries against a database maintained by a separate DBA or third-party vendor team, the schema is read-only for your team, and you cannot make modifications.
* **Why it fits:**
  * **Instant Modeling:** Running `Scaffold-DbContext` reads the existing database metadata and automatically generates the matching entities and context class. This saves weeks of manual mapping effort.
  * **Separation of Concerns:** Keep the third-party schema models inside an isolated namespace (e.g. `Data.EfDbFirst.Models`) without letting legacy schema fields leak into your clean core domain logic.
  * **Synchronization:** If the external team adds a column or table, you simply re-run the scaffolding command to sync your code models instantly.

---

## Architectural Summary Matrix

| Metric | ADO.NET | EF Core Code First | EF Core DB First |
| :--- | :--- | :--- | :--- |
| **Control** | Maximum (Raw SQL, Execution plans) | Medium (ORM abstracts database) | Medium (ORM abstracts database) |
| **Development Speed** | Low (Heavy boilerplate mapping) | High (Auto schema creation) | High (Scaffold-generated) |
| **Migration Handling** | Manual SQL scripts | Automatic C# migrations | Database-driven schema updates |
| **Performance** | High (Direct data reader pipelines) | Good (Has minor change tracking overhead) | Good (Has minor change tracking overhead) |
| **Best For** | Ultra-high performance, Stored Procs | New projects, Evolving domain schemas | Legacy systems, External read-only databases |
