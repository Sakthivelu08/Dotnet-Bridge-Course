# Design Patterns - Future Course Mapping

This document maps all the design patterns introduced in Week 1 to where they appear in future modules of this .NET application development course.

---

| Design Pattern | Week / Area of Appearance | Context & Real-world Usage in ASP.NET Core |
| :--- | :--- | :--- |
| **Repository + Unit of Work** | **Week 4 — Data Layer (EF Core)** | Creates the abstract seam separating database transactions from business logic. Implemented via generic `Repository<T>` and `UnitOfWork` wrapping `DbContext.SaveChanges()`. |
| **Strategy** | **Week 2 & 3 — Web APIs & Controllers** | Selects payment channels (Card, UPI) or parsing engines at runtime. Also used for custom authentication handlers based on headers. |
| **Observer** | **Week 5 — Messaging & Events** | Pub/Sub event-driven triggers. Used when integrating RabbitMQ / Kafka, or with SignalR to notify clients in real-time. |
| **Factory / Factory Method** | **Week 2 — Dependency Injection (DI)** | Underpins the ASP.NET Core DI Container. Instantiates transient, scoped, and singleton service instances behind the scenes. |
| **Adapter** | **Week 3 — Integrations** | Translates internal model representations to third-party integration contracts (e.g., converting modern JSON requests to legacy SOAP XML payloads). |
| **Facade** | **Week 2 & 3 — Controllers / Services** | Coordinates underlying micro-services (e.g., checking stock, processing charge, and scheduling shipping) under a clean API endpoint. |
| **Singleton** | **Week 2 — Configurations & Logging** | Keeps single shared state instances across the application lifetime (e.g., appsettings config cache, thread-safe memory logging). |
| **Builder** | **Week 2 — Application Setup** | Used inside `Program.cs` to configure service containers and HTTP pipelines (e.g., `WebApplication.CreateBuilder(args)`). |
| **Prototype** | **Week 4 — Object Copying / EF Tracking** | Clones entity record configurations or mocks system configurations without triggering heavy database fetches. |
| **Decorator** | **Week 3 — Middleware & Filters** | Extends controllers with cross-cutting concerns dynamically (e.g., wrapping request flows with logging, caching, or performance checks). |
| **Command** | **Week 3 — CQRS (MediatR)** | Separates read/write operations (CQRS) where operations are encapsulated as standalone Command objects (e.g., `CreateOrderCommand`). |
| **Template Method** | **Week 3 — Base Services / Controllers** | Defines pipeline skeleton structures (e.g., abstract `BaseApiController` standardizing response formatting while subclasses define actions). |
| **Mediator** | **Week 3 — CQRS Architecture** | Decouples Web API Controllers from the service handlers via MediatR, routing commands and queries centrally. |
| **Chain of Responsibility** | **Week 2 — HTTP Request Pipeline** | Forms the core ASP.NET Core Middleware pipeline. HTTP requests traverse a chain of middlewares (Auth -> Logging -> CORS -> Routing). |
| **State** | **Week 5 — Saga Orchestration** | Manages states in distributed transactions (e.g., Order transitioning from `Pending` -> `Paid` -> `Shipped` -> `Delivered`). |
