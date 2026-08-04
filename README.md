# Student & Teacher Management API

This repository contains a RESTful Web API for managing Student and Teacher domains, built with ASP.NET Core.

## Features
- **Student CRUD:** Full create, read, update, delete, and case-insensitive search endpoints.
- **Teacher CRUD:** Full CRUD operations and domain validation.
- **Strategy Pattern:** Swappable grade formatting (Percentage vs GPA) resolved dynamically at runtime using custom request headers.
- **Persistence:** Thread-safe generic in-memory repository pattern coordinated by a Unit of Work context.

## Running the Web API
To start the REST API locally:
```powershell
dotnet run --project Week2\src\BridgeCourse.Week2.Api\BridgeCourse.Week2.Api.csproj
```

## Swagger UI Documentation
Once running, the API documentation and endpoint testing console are accessible at:
* 🌐 **Swagger UI:** `http://localhost:5025/swagger/index.html`

## Running Tests
To execute the xUnit test suite and check code coverage (Target: >=80%):
```powershell
dotnet test Week2\tests\BridgeCourse.Week2.Tests\BridgeCourse.Week2.Tests.csproj --collect:"XPlat Code Coverage" -- DataCollectionRunSettings.DataCollectors.DataCollector.Configuration.Exclude="[*]Program"
```
