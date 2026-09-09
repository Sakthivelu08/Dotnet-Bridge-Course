# Week 1: Comprehensive Walkthrough, Edge Cases, and Assessment Guide

This document provides a highly detailed walkthrough of all concepts covered in the Week 1 syllabus, the daily tasks, implementation mechanics, edge cases, and typical multiple-choice question (MCQ) assessment angles. Use this guide to deeply study the mechanics of C# and .NET.

---

## Table of Contents
1. [Day 1: Exceptions, Disposal & Async](#day-1-exceptions-disposal--async)
2. [Day 2: Singleton, Factory & Observer](#day-2-singleton-factory--observer)
3. [Day 3: Strategy, Repository+UoW, Adapter & Facade](#day-3-strategy-repositoryuow-adapter--facade)
4. [Day 4: TPL, Reflection & Attributes](#day-4-tpl-reflection--attributes)
5. [Day 5: Abstraction, Lifecycle & Sorting](#day-5-abstraction-lifecycle--sorting)
6. [Summary of the 8 Additional Design Patterns](#summary-of-the-8-additional-design-patterns)
7. [Edge Cases & "Gotcha" Cheat Sheet for Assessments](#edge-cases--gotcha-cheat-sheet-for-assessments)

---

## Day 1: Exceptions, Disposal & Async

### 1. Exception Handling & Catch Block Ordering
* **Core Concept:** C# exception catching is evaluated **sequentially (top-to-bottom)**. More specific exceptions (subclasses) must be caught before more general exceptions (base classes).
* **The Compiler Error (CS0160):** If a base class exception (like `Exception` or `SystemException`) is placed above a specific exception (like `FormatException` or `OverflowException`), the compiler will throw error `CS0160`: *"A previous catch clause already catches all exceptions of this or of a super type."*
* **The `finally` Block Mechanics:** The `finally` block is guaranteed to execute regardless of whether an exception is thrown, caught, or if the `try`/`catch` blocks return a value. 
  * *Exception to the rule:* `finally` will not execute if `Environment.Exit()` is called, or if an infinite loop/stack overflow terminates the process.
* **Edge Cases & MCQ Targets:**
  * **Nested Exceptions:** If an exception is thrown inside a `catch` or `finally` block, it overrides the original exception unless wrapped as an `InnerException`.
  * **Re-throwing Exceptions:** Using `throw;` preserves the original stack trace. Using `throw ex;` resets the stack trace to the current line, destroying debugging context.

---

### 2. Resource Disposal & IDisposable Pattern
* **Core Concept:** Managed memory is cleaned up by the Garbage Collector (GC). However, unmanaged resources (file handles, database connections, sockets, GDI+ handles) must be released immediately and deterministically via `IDisposable`.
* **The Standard Dispose Pattern Structure:**
  ```csharp
  public class TempFileManager : IDisposable
  {
      private bool _disposed = false;

      // Constructor creates unmanaged/managed resources
      public TempFileManager() { ... }

      public void Dispose()
      {
          Dispose(true);
          GC.SuppressFinalize(this); // Prevents finalizer from running
      }

      protected virtual void Dispose(bool disposing)
      {
          if (!_disposed)
          {
              if (disposing)
              {
                  // Clean up managed resources here
              }
              // Clean up unmanaged resources here
              _disposed = true;
          }
      }

      ~TempFileManager() // Finalizer (Destructor)
      {
          Dispose(false); // Only clean up unmanaged resources
      }
  }
  ```
* **Why do we need a Finalizer (~Class)?** The finalizer acts as a backup safety net. If a developer forgets to call `.Dispose()`, the GC will eventually invoke the finalizer during garbage collection.
* **Why do we call `GC.SuppressFinalize(this)`?** If `Dispose()` is called manually/deterministically, the resources are already freed. Suppressing finalization instructs the GC that the object does not need to go to the finalizer queue, saving performance overhead and preventing the object from being promoted to a higher GC generation.
* **Edge Cases & MCQ Targets:**
  * Double Disposal: Implementations of `.Dispose()` must be safe to call multiple times without throwing exceptions (idempotent).
  * Accessing Disposed Objects: Methods inside a disposed class should throw `ObjectDisposedException` if invoked after disposal.

---

### 3. Async & Await (Non-blocking I/O)
* **Core Concept:** `async` and `await` enable asynchronous execution by transforming methods into state machines behind the scenes. They prevent the calling thread (e.g., UI main thread) from blocking during long-running I/O operations.
* **Task.Delay vs. Thread.Sleep:**
  * `Thread.Sleep(ms)` is a **blocking synchronous** call. It freezes the current OS thread, rendering it completely idle and unusable for other tasks.
  * `Task.Delay(ms)` is a **non-blocking asynchronous** task. It registers a timer and yields the thread back to the thread pool to execute other work. When the timer expires, an available thread resumes the continuation.
* **Task.WhenAll vs. Sequential Awaiting:**
  * *Sequential:* `await FetchA(); await FetchB();` $\rightarrow$ Total execution time is $T_A + T_B$.
  * *Concurrent:* `var tA = FetchA(); var tB = FetchB(); await Task.WhenAll(tA, tB);` $\rightarrow$ Total execution time is $Max(T_A, T_B)$.
* **Edge Cases & MCQ Targets:**
  * **`async void` vs. `async Task`:** `async void` should **only** be used for event handlers. You cannot `await` an `async void` method, and any unhandled exception inside it will crash the entire application process.
  * **SynchronizationContext:** In UI applications (WPF/WinForms), continuation resumes on the UI thread. Calling `.Result` or `.Wait()` on an async task from the UI thread causes a **deadlock** because the UI thread is waiting for the task to complete, while the task is waiting for the UI thread to become free to run the continuation.

---

## Day 2: Singleton, Factory & Observer

### 4. Thread-Safe Singleton
* **Core Concept:** Guarantees that a class has only one instance throughout the application lifecycle and provides a global access point.
* **Implementation via `Lazy<T>`:**
  ```csharp
  public class Logger
  {
      private static readonly Lazy<Logger> _instance = 
          new Lazy<Logger>(() => new Logger(), LazyThreadSafetyMode.ExecutionAndPublication);

      public static Logger Instance => _instance.Value;

      private Logger() { } // Private constructor prevents direct instantiation
  }
  ```
* **Thread-Safety Mechanics:** `Lazy<T>` handles thread synchronization internally. Under the hood, it uses double-check locking to make sure that only one thread can execute the constructor even if multiple threads access `Instance` concurrently.
* **Edge Cases & MCQ Targets:**
  * Reflection Attack: Private constructors can still be instantiated via reflection unless guarded.
  * Lazy vs Eager Initialization: Static variables without `Lazy<T>` are initialized when the class is loaded (eager). `Lazy<T>` defers initialization until `.Value` is accessed.

---

### 5. Factory & Factory Method
* **Core Concept:** Decouples the client code from concrete object creation, eliminating direct calls to the `new` keyword.
* **Factory vs. Factory Method:**
  * **Simple Factory:** A single class with a static method (e.g., `VehicleFactory.CreateVehicle(type)`) containing a `switch` statement to return a concrete instance. Violates the **Open/Closed Principle (OCP)** because adding a new vehicle type requires modifying the factory class.
  * **Factory Method:** Relies on inheritance. An abstract creator class (e.g., `VehicleCreator`) defines an abstract factory method that subclasses (e.g., `CarFactory`, `BikeFactory`) override to instantiate and return specific products. Adheres to OCP because you can support new products by creating new factory subclasses without editing existing code.
* **Edge Cases & MCQ Targets:**
  * Factory Method parameter signatures must remain consistent across subclasses.
  * If a factory returns an interface, callers are completely unaware of the underlying concrete class implementation.

---

### 6. Observer Pattern (Interfaces vs. Events)
* **Core Concept:** Establishes a one-to-many dependency where state changes in a subject automatically notify all registered observers.
* **Comparison: Custom Interface vs. C# Events**
  | Feature | Custom Interface (`IObserver`/`ISubject`) | C# native Events (`event Action`) |
  | :--- | :--- | :--- |
  | **Coupling** | Explicitly couples subject to the observer interfaces. | Completely decoupled; subject only knows delegate signatures. |
  | **Invocation** | Managed manually by looping through a list of interface instances. | Managed by the compiler via multicast delegate chains. |
  | **Encapsulation** | Observers have to implement formal interface methods. | Any matching method (instance, static, lambda) can subscribe. |
* **The "Lapsed Listener" Memory Leak:** When an observer subscribes to a subject's event (`subject.Changed += observer.OnChanged`), the subject holds a strong reference to the observer. If the observer goes out of scope but fails to unsubscribe (`-=`), the Garbage Collector cannot collect the observer, resulting in a memory leak.
* **Edge Cases & MCQ Targets:**
  * Standard events are synchronous. If one observer blocks inside its event handler, it delays notifications for all subsequent observers in the multicast delegate chain.

---

## Day 3: Strategy, Repository+UoW, Adapter & Facade

### 7. Strategy Pattern
* **Core Concept:** Defines a family of algorithms, encapsulates each one, and makes them interchangeable at runtime. Represents **Composition over Inheritance**.
* **Implementation:** The context class (e.g., `ShoppingCart`) references an interface (`IPaymentStrategy`). The client can swap the concrete strategy (`CreditCardPaymentStrategy`, `UpiPaymentStrategy`) at runtime via a setter method.
* **Edge Cases & MCQ Targets:**
  * Switching strategy in a multi-threaded context can cause race conditions if the strategy reference is not thread-safe.
  * Strategy is a behavioral pattern; it does not construct objects (unlike Factory).

---

### 8. Repository & Unit of Work (The Data Seam)
* **Core Concept:** Decouples business logic from persistence (data access) frameworks.
  * **Repository:** Mimics an in-memory collection of domain objects (CRUD operations: `Add`, `Get`, `Delete`).
  * **Unit of Work:** Coordinates transactions. It maintains a list of business objects affected by a transaction and coordinates the writing out of changes (e.g., exposing a `Save()` method that maps to `DbContext.SaveChanges()`).
* **Why it matters for ASP.NET Core (EF Core):** This architecture decouples controllers from direct DB queries, allowing databases to be swapped, cached, or mocked easily inside unit tests.
* **Edge Cases & MCQ Targets:**
  * Shared context: Multiple repositories must share the same Unit of Work database context to run under a single transaction.
  * Generic vs Specific Repositories: Generic repositories (`IRepository<T>`) reduce code duplication, but specific repositories are required for custom queries (like complex SQL joins).

---

### 9. Adapter vs. Facade (Structural Comparison)
* **Adapter Pattern:** *Translates* one interface to another. Used when integrating third-party code or legacy components whose interfaces do not match the system's contract (e.g., adapting JSON to XML).
* **Facade Pattern:** *Simplifies* a complex subsystem. It provides a unified, higher-level interface that coordinates multiple sub-services under a single entry point (e.g., `OrderFacade` calling `Inventory`, `Payment`, and `Shipping` services).
* **Edge Cases & MCQ Targets:**
  * **Adapter** changes the interface structure to resolve incompatibility.
  * **Facade** simplifies the subsystem but does not restrict advanced clients from bypassing the facade to call internal services directly if needed.

---

## Day 4: TPL, Reflection & Attributes

### 10. Task Parallel Library (TPL)
* **Core Concept:** Simplifies parallel and concurrent code execution by abstracting threads into Tasks.
* **OS Threads vs. Task.Run vs. Parallel.ForEach:**
  * **Raw OS Threads:** High instantiation cost (~1MB stack memory per thread). Unsuited for spawning hundreds of small operations.
  * **Task.Run:** Queues work to the managed **ThreadPool**, which automatically manages thread recycling and scheduling.
  * **Parallel.ForEach:** Automatically partitions source collections and executes work across multiple threads concurrently. It blocks the calling thread until the loop completes.
* **Edge Cases & MCQ Targets:**
  * Spawning I/O-blocking tasks inside a `Parallel.ForEach` causes thread pool starvation. It should only be used for **CPU-bound** parallel calculations.
  * Race conditions: Shared mutable state inside parallel loops must be synchronized using thread-safe primitives (like `lock`, `Interlocked`, or thread-safe collections).

---

### 11. Reflection & Metaprogramming
* **Core Concept:** The ability to inspect assemblies, classes, methods, properties, and constructors metadata at runtime, and dynamically instantiate objects or invoke members.
* **Dynamic Instantiation:** `Activator.CreateInstance(type, args)` dynamically locates the constructor matching the parameter list and instantiates the object at runtime.
* **Property Manipulation:** `PropertyInfo.SetValue(obj, value)` updates a property value bypassing compile-time static type checks.
* **Edge Cases & MCQ Targets:**
  * Performance: Reflection operations are significantly slower than standard compile-time operations because type checks are deferred to runtime.
  * Security/Access: Reflection can bypass accessibility access modifiers (e.g., reading or writing private fields), which can fail in environments with strict sandbox trust permissions.

---

### 12. Custom Attributes & Validation Engines
* **Core Concept:** Attributes append declarative metadata to code structures. By themselves, attributes are inert metadata tags; they require a reflection-based validation engine to read and enforce constraints at runtime.
* **How it works:**
  1. Define a class inheriting from `Attribute` (e.g., `MaxLengthNoAttribute`).
  2. Decorate properties with `[MaxLengthNo(10)]`.
  3. The validation engine calls `propertyInfo.GetCustomAttributes(typeof(MaxLengthNoAttribute), true)` to locate constraints and inspect string values.
* **Edge Cases & MCQ Targets:**
  * Attributes can be restricted to specific targets (classes, methods, properties) using the `[AttributeUsage]` attribute.
  * Attribute parameters must be compile-time constants (literals, enums, `typeof` expressions). You cannot pass dynamic runtime values as attribute arguments.

---

## Day 5: Abstraction, Lifecycle & Sorting

### 13. Interface vs. Abstract Class
* **Structural Comparison:**
  | Property | Interface | Abstract Class |
  | :--- | :--- | :--- |
  | **Inheritance** | A class can implement **multiple** interfaces. | A class can inherit from **only one** abstract class. |
  | **State** | **Cannot** contain instance fields or variables. | **Can** contain instance fields and state. |
  | **Constructors** | **Cannot** define constructors. | **Can** define constructors to enforce base setup. |
  | **Versioning** | Historically breaks implementations (mitigated by modern default implementations). | Adding non-abstract methods is non-breaking. |
* **Design Guideline:** Use an **Interface** to define common capabilities across unrelated classes (e.g., `INotificationChannel`). Use an **Abstract Class** when modeling a close family of related classes sharing common code and initialization states (e.g., `DatabaseMigrator`).

---

### 14. Static vs. Instance Lifecycle
* **Static Classes/Methods:**
  * Class cannot be instantiated (`new`) and is sealed.
  * Methods are stateless and depend entirely on their inputs.
  * *Use case:* Utility math libraries (`MathHelper.Factorial`), extensions, or constants.
* **Instance Classes/Methods:**
  * Requires instantiation.
  * Can implement interfaces, maintain mutable state, and participate in polymorphism.
  * *Use case:* Component layers (`OrderProcessor`) requiring Dependency Injection (DI) to swap implementation drivers (like notification engines or DB contexts) for modularity and testing.

---

### 15. Sorting: IComparable vs. IComparer
* **IComparable`<T>`:**
  * Implemented directly inside the class (e.g., `Employee : IComparable<Employee>`).
  * Establishes the **default** sorting criteria (intrinsic comparison) by implementing `CompareTo(T other)`.
* **IComparer`<T>`:**
  * Implemented in a separate helper class (e.g., `EmployeeNameComparer : IComparer<Employee>`).
  * Establishes **alternative** sorting strategies (extrinsic comparison) by implementing `Compare(T x, T y)`.
* **Edge Cases & MCQ Targets:**
  * Both interfaces should return:
    * `< 0` if the current/first object is less than the compared object.
    * `0` if both objects are equal.
    * `> 0` if the current/first object is greater than the compared object.
  * Sorting list containing `null`: Comparers must handle null references defensively to avoid `NullReferenceException`. By convention, `null` is considered less than any non-null reference.

---

## Summary of the 8 Additional Design Patterns

These 8 patterns are introduced in Week 1 for conceptual mapping:

1. **Builder Pattern**
   * *Definition:* Separates the construction of a complex object from its representation, allowing step-by-step construction.
   * *Real-world Use Case:* Setting up application configurations in ASP.NET Core: `WebApplication.CreateBuilder(args)`.
2. **Prototype Pattern**
   * *Definition:* Creates new instances by cloning an existing instance (prototype) instead of creating them from scratch.
   * *Real-world Use Case:* Entity Framework Core cloning entity configurations or state tracking mock-ups.
3. **Decorator Pattern**
   * *Definition:* Dynamically attaches additional responsibilities to an object without modifying its structure.
   * *Real-world Use Case:* ASP.NET Core Middleware or Action Filters wrapping controller endpoints with logging/caching.
4. **Command Pattern**
   * *Definition:* Encapsulates a request as an object, allowing parameterization, queueing, and undo operations.
   * *Real-world Use Case:* CQRS command objects (e.g., `CreateOrderCommand`) routed via MediatR handlers.
5. **Template Method Pattern**
   * *Definition:* Defines the skeleton of an algorithm in an abstract method, deferring specific steps to subclasses without altering the algorithm structure.
   * *Real-world Use Case:* Base controllers standardizing error formatting while subclasses define specific endpoint routes.
6. **Mediator Pattern**
   * *Definition:* Restricts direct communication between objects, forcing them to communicate through a central mediator object to reduce coupling.
   * *Real-world Use Case:* The `MediatR` library routing web requests directly to service handlers.
7. **Chain of Responsibility Pattern**
   * *Definition:* Passes requests along a chain of handlers, where each handler decides either to process the request or pass it to the next handler.
   * *Real-world Use Case:* The ASP.NET Core HTTP Request Middleware pipeline.
8. **State Pattern**
   * *Definition:* Allows an object to alter its behavior when its internal state changes, making it appear as if the class changed.
   * *Real-world Use Case:* Saga distributed transactions managing order workflows (`Pending` $\rightarrow$ `Paid` $\rightarrow$ `Shipped`).

---

## Edge Cases & "Gotcha" Cheat Sheet for Assessments

Here are specific scenarios frequently used in C# assessments to test depth of knowledge:

### 1. The `finally` variable update trap
What does this method return?
```csharp
public static int GetNumber()
{
    int x = 10;
    try
    {
        return x; // returns 10
    }
    finally
    {
        x = 20; // x is modified to 20, but does it change the return value?
    }
}
```
* **Gotcha:** You might think it returns 20 because `finally` is guaranteed to run.
* **Reality:** It returns **10**. The return value is evaluated and copied to a temporary stack slot *before* the `finally` block executes. Modifying `x` inside `finally` has no effect on the value that was already queued for return.

### 2. Async exceptions inside `Task.WhenAll`
What happens when multiple tasks throw exceptions concurrently in `Task.WhenAll`?
```csharp
try
{
    var t1 = Task.FromException(new ArgumentException("Err1"));
    var t2 = Task.FromException(new InvalidOperationException("Err2"));
    await Task.WhenAll(t1, t2);
}
catch (Exception ex)
{
    Console.WriteLine(ex.Message);
}
```
* **Gotcha:** `Task.WhenAll` captures all exceptions inside an `AggregateException`.
* **Reality:** When you `await` the result, the state machine will only throw the **first** exception (e.g. `Err1`). To inspect all failures, you must check the `.Exception.InnerExceptions` collection of the task returned by `Task.WhenAll`.

### 3. Mutating values in a read-only Singleton
Does a `readonly` static Singleton instance guarantee state immutability?
```csharp
public class MySingleton
{
    public static readonly MySingleton Instance = new MySingleton();
    public int Counter { get; set; } = 0;
}
```
* **Gotcha:** Believing the singleton data cannot change because the field is marked `readonly`.
* **Reality:** The reference to `Instance` is readonly (you cannot execute `Instance = null`). However, the internal properties of the singleton (`Counter`) can be mutated freely by any thread, which requires synchronization logic to prevent data corruption.
