# Dotnet Bridge Course & React Integration

This repository contains the laboratories, assignments, and full-stack integration practice solutions completed as part of the **Dotnet Bridge Course**. It serves as a comprehensive training codebase covering C# OOP, RESTful API design, cryptography, databases (ADO.NET & EF Core), React web fundamentals, and Selenium E2E portal testing.

---

## Learning Milestones

### 1. Week 1 — Object Oriented Programming Fundamentals
* **Description**: Covers fundamental OOP concepts (Encapsulation, Inheritance, Polymorphism, and Abstraction) using C# classes, records, properties, and constructors.
* **Run Command**:
  ```powershell
  dotnet run --project Week1\src\BridgeCourse.Week1.Lab\BridgeCourse.Week1.Lab.csproj
  ```

### 2. Week 2 — RESTful API Development & Strategy Pattern
* **Description**: Built an ASP.NET Core Web API with generic repositories. Implemented a dynamic Strategy Pattern resolved via request headers to toggle student grade formatting between percentage-based and GPA-based styles.
* **Run Command**:
  ```powershell
  dotnet run --project Week2\src\BridgeCourse.Week2.Api\BridgeCourse.Week2.Api.csproj
  ```
  * Swagger UI access: `http://localhost:5025/swagger/index.html` (after running).

### 3. Week 3 — Cryptography & Signed JWT Authentication
* **Description**: Implemented password security using PBKDF2 salting/hashing, AES-GCM encryption layouts for secure database persistence, and custom JWT Bearer signing middleware.
* **Run Command**:
  ```powershell
  dotnet run --project Week3\BridgeCourse.Week3\BridgeCourse.Week3.Api\BridgeCourse.Week3.Api.csproj
  ```

### 4. Week 4 — Database Persistence Swapping (ADO.NET vs EF Core)
* **Description**: Configured swappable persistence layers. Compares ADO.NET parameterized queries (SQL injection defense) and Stored Procedures with EF Core (Code First migrations and Database First adapter seams).
* **Run Command**:
  ```powershell
  dotnet run --project Week4\BridgeCourse.Week4.Api\BridgeCourse.Week4.Api.csproj
  ```

### 5. Week 5 — React Client SPA & Full-Stack Integration (Part 1)
* **Description**: Single Page Application built on React/Vite containing login/registration forms, custom JWT request interceptors, and role-based UI conditional rendering. Includes a Vitest + RTL testing suite with an 80% coverage threshold.
* **Run Command**:
  ```powershell
  cd Week5\student-portal
  npm install
  npm run dev
  ```
  * Local host access: `http://localhost:5173`. Make sure the target port in your client `.env` matches the running port of your active backend API.

### 6. Week 6 — Full-Stack Integration (Part 2) & Portal-Wide E2E Testing
* **Description**: Complete end-to-end integration of the portal with graceful error handling (401 auto-relogin, 403 access denied, 400 validation, 500 server degrade). Includes a Selenium.WebDriver E2E test project executing browser flows and server rejection validation.
* **Run Command (Selenium E2E Tests)**:
  ```powershell
  dotnet test Week6\tests\BridgeCourse.Week6.E2E\BridgeCourse.Week6.E2E.csproj
  ```

---

## Running Test Suites Across All Layers

### 1. Backend xUnit Unit Tests (Weeks 1 - 4)
From the workspace root, run:
```powershell
dotnet test Week4\BridgeCourse.Week4.Tests\BridgeCourse.Week4.Tests.csproj
```

### 2. Frontend Vitest / RTL Unit Tests (Week 5)
From the `Week5\student-portal` directory:
```powershell
npm run test:coverage
```

### 3. Selenium E2E Browser Suite (Week 6)
Make sure both the .NET Web API and Vite React server are running, then execute:
```powershell
dotnet test Week6\tests\BridgeCourse.Week6.E2E\BridgeCourse.Week6.E2E.csproj
```
