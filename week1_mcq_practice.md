# Week 1 Concepts: MCQ & Assessment Prep

This document contains conceptual multiple-choice questions (MCQs), code-based troubleshooting scenarios, and high-frequency technical interview questions mapping to Week 1 of the Dotnet Bridge Course. 

Use this to test your understanding of Exceptions, Disposal, Async, Design Patterns, TPL, Reflection, and C# Core.

---

## Part 1: Conceptual & Output-Based MCQs

### Topic 1: Exceptions & Resource Disposal (Day 1)

#### Q1. Consider the following catch block ordering. What is the result?
```csharp
try 
{
    int value = int.Parse("abc");
}
catch (Exception ex)
{
    Console.WriteLine("General Exception Caught");
}
catch (FormatException ex)
{
    Console.WriteLine("Format Exception Caught");
}
```
* **A)** The code compiles and prints "General Exception Caught" at runtime.
* **B)** The code compiles and prints "Format Exception Caught" at runtime.
* **C)** The code fails to compile with error `CS0160` (A previous catch clause already catches all exceptions of this or of a super type).
* **D)** The compiler automatically reorders the catch blocks to catch `FormatException` first.

<details>
<summary><b>Answer & Explanation</b></summary>

**Answer: C**

**Explanation:** In C#, exception filters/catches are evaluated from top to bottom. Since `Exception` is the base class of all exceptions, catching it first makes all subsequent specific catches (like `FormatException`) unreachable code. The C# compiler enforces specific-to-general catch ordering and throws a compiler error (`CS0160`) to prevent this.
</details>

---

#### Q2. Why do we invoke `GC.SuppressFinalize(this)` inside the `Dispose()` method of the `IDisposable` pattern?
* **A)** To immediately free managed memory without waiting for the Garbage Collector.
* **B)** To instruct the GC that the object's resources have already been cleaned up and it does not need to run its finalizer (destructor).
* **C)** To prevent the operating system from deleting files or releasing sockets.
* **D)** To suppress any exceptions thrown during resource cleanup.

<details>
<summary><b>Answer & Explanation</b></summary>

**Answer: B**

**Explanation:** Finalizers are executed by a special background thread during Garbage Collection, which adds overhead and delays memory reclamation (objects with finalizers are promoted to the next GC generation first). If `Dispose()` is called deterministically, all resources are already cleaned up. `GC.SuppressFinalize(this)` tells the GC that finalization is redundant, optimizing resource collection.
</details>

---

### Topic 2: Async Programming & TPL (Days 1 & 4)

#### Q3. Look at this async method. What happens when `FetchDataAsync` is called?
```csharp
public async Task ProcessData()
{
    Console.WriteLine("Step 1");
    await Task.Delay(1000);
    Console.WriteLine("Step 2");
}
```
* **A)** The thread calling `ProcessData` is blocked (frozen) for 1000ms.
* **B)** `Step 1` is printed, the executing thread is released back to the thread pool/caller, and when the 1000ms timer completes, a thread resumes execution to print `Step 2`.
* **C)** `Step 1` and `Step 2` are printed immediately; `Task.Delay` runs on a separate thread in parallel.
* **D)** This method throws a compilation error because it does not return a value.

<details>
<summary><b>Answer & Explanation</b></summary>

**Answer: B**

**Explanation:** `await` yields control of the executing thread back to the caller (or UI thread loop) instead of blocking. `Task.Delay` is a non-blocking timer. Once the delay completes, the system schedules the remainder of the method (the continuation) to run on an available thread.
</details>

---

#### Q4. Which of the following is the most efficient way to fetch data from three independent external APIs asynchronously?
* **A)**
  ```csharp
  var data1 = await FetchApi1();
  var data2 = await FetchApi2();
  var data3 = await FetchApi3();
  ```
* **B)**
  ```csharp
  var t1 = FetchApi1();
  var t2 = FetchApi2();
  var t3 = FetchApi3();
  await Task.WhenAll(t1, t2, t3);
  ```
* **C)**
  ```csharp
  Task.Run(() => FetchApi1());
  Task.Run(() => FetchApi2());
  Task.Run(() => FetchApi3());
  ```
* **D)**
  ```csharp
  Parallel.Invoke(
      async () => await FetchApi1(),
      async () => await FetchApi2(),
      async () => await FetchApi3()
  );
  ```

<details>
<summary><b>Answer & Explanation</b></summary>

**Answer: B**

**Explanation:** In option **A**, each request awaits sequentially (taking $T1 + T2 + T3$ time). Option **B** kicks off all three non-blocking asynchronous operations simultaneously, then awaits their collective completion (taking only $Max(T1, T2, T3)$ time). Parallel loops/invokes are designed for CPU-bound tasks, not I/O-bound async tasks.
</details>

---

### Topic 3: Creational & Behavioral Patterns (Day 2)

#### Q5. What is the primary benefit of implementing a Singleton pattern using C# `Lazy<T>`?
```csharp
private static readonly Lazy<Logger> _instance = 
    new Lazy<Logger>(() => new Logger());
```
* **A)** It guarantees the class can only be instantiated inside unit tests.
* **B)** It ensures the instance is initialized only when first accessed, and it is automatically thread-safe without manual locks.
* **C)** It makes the Singleton behave as a transient service.
* **D)** It forces the garbage collector to never collect the logger instance.

<details>
<summary><b>Answer & Explanation</b></summary>

**Answer: B**

**Explanation:** `Lazy<T>` is lazy-evaluating (instantiates the object only when the `.Value` property is read) and is thread-safe by default, using double-check locking internally to handle race conditions where multiple threads access it at the exact same moment.
</details>

---

#### Q6. What is a key risk of implementing the Observer pattern using native C# events?
* **A)** Subscribers cannot receive events on background threads.
* **B)** Events can only notify a single subscriber at a time.
* **C)** A memory leak occurs if a subscriber (Observer) fails to unsubscribe (`-=`) from the publisher before it goes out of scope.
* **D)** C# events cannot compile inside console applications.

<details>
<summary><b>Answer & Explanation</b></summary>

**Answer: C**

**Explanation:** A publisher event holds a strong reference to the target method and object of its subscribers. If the subscriber object goes out of scope but does not unsubscribe, the publisher still references it. This prevents the garbage collector from cleaning up the subscriber, creating a memory leak (often called the *lapsed listener* problem).
</details>

---

### Topic 4: Structural & Architecture Patterns (Day 3)

#### Q7. In standard architectural terms, what are the roles of the **Repository** and **Unit of Work** patterns?
* **A)** Repository formats UI JSON data; Unit of Work maps database queries.
* **B)** Repository creates a collection-like abstraction for database queries; Unit of Work coordinates multiple repository actions under a single database transaction.
* **C)** Repository handles HTTP requests; Unit of Work runs middleware threads.
* **D)** Repository clones template objects; Unit of Work dynamically injects dependencies.

<details>
<summary><b>Answer & Explanation</b></summary>

**Answer: B**

**Explanation:** The **Repository** hides the query logic (EF core context calls, SQL, etc.) behind a clean collection interface (`IRepository<T>`). The **Unit of Work** groups multiple repositories together and exposes a method (`Save()`) to guarantee that all changes succeed or fail together as a transaction block.
</details>

---

#### Q8. What is the fundamental difference between the **Adapter** and **Facade** patterns?
* **A)** Adapter switches behaviors at runtime; Facade instantiates objects dynamically.
* **B)** Adapter creates a unified entry point to a complex subsystem; Facade translates one interface to another.
* **C)** Adapter translates one interface to another (resolving incompatibility); Facade provides a simplified, higher-level interface to a complex subsystem.
* **D)** Adapter is a creational pattern; Facade is a behavioral pattern.

<details>
<summary><b>Answer & Explanation</b></summary>

**Answer: C**

**Explanation:** Both are structural patterns, but their intent is different. **Adapter** makes two incompatible interfaces work together (e.g., converting system XML output to JSON for an external API). **Facade** simplifies a complex network of internal systems (e.g., `OrderFacade` grouping `PaymentService`, `InventoryService`, and `ShippingService` under one clean API call).
</details>

---

### Topic 5: Reflection & Attributes (Day 4)

#### Q9. Consider the following code. How does Reflection instantiate this object?
```csharp
Type type = typeof(Invoice);
object instance = Activator.CreateInstance(type, 101, "Alice", 250m);
```
* **A)** It bypasses all constructors and creates a blank memory instance.
* **B)** It queries the type metadata at runtime, locates the public constructor matching the signature `(int, string, decimal)`, and executes it with the provided arguments.
* **C)** It compiles a new class at runtime that inherits from `Invoice`.
* **D)** It generates a compile-time warning because new is not used.

<details>
<summary><b>Answer & Explanation</b></summary>

**Answer: B**

**Explanation:** `Activator.CreateInstance` uses reflection to dynamically inspect metadata at runtime, match parameters, and invoke the constructor. While powerful (used by DI containers), it has a small performance overhead compared to compiling with `new Invoice()`.
</details>

---

#### Q10. What is the role of an Attribute in C#?
* **A)** To represent standard class properties like strings or integers.
* **B)** To attach declarative metadata to code elements (classes, methods, properties) that can be inspected at runtime using reflection.
* **C)** To act as a garbage collector hook.
* **D)** To establish inheritance relationships between interfaces.

<details>
<summary><b>Answer & Explanation</b></summary>

**Answer: B**

**Explanation:** Attributes (e.g., `[MaxLengthNo]` or `[Required]`) add structural metadata to class members. The code itself doesn't execute anything upon declaration, but a reflection engine (like a validation validator) can read these attributes at runtime and apply logic.
</details>

---

### Topic 6: Abstraction & Lifecycles (Day 5)

#### Q11. Which of the following is true regarding **Interface vs. Abstract Class** in modern C#?
* **A)** Abstract classes support multiple inheritance; interfaces do not.
* **B)** Interfaces cannot define default method implementations.
* **C)** Abstract classes can contain instance state (fields) and enforce constructor initialization; interfaces cannot contain instance fields.
* **D)** Interfaces cannot be unit-tested.

<details>
<summary><b>Answer & Explanation</b></summary>

**Answer: C**

**Explanation:** Abstract classes represent a class template and can hold instance state (`protected string _connString`). Interfaces represent a behavioral contract. Even though C# 8+ allows default implementations in interfaces, interfaces still cannot hold instance variables/states, and they do not have constructors.
</details>

---

#### Q12. Under what scenario should you design a utility class to be `static` rather than `instance`-based?
* **A)** When the class has external dependencies that need mocking during unit testing.
* **B)** When the class functions represent stateless utility calculations (e.g., `Math.Abs`) and do not hold any mutable state.
* **C)** When the class needs to inherit from a base class.
* **D)** When you want to manage concurrent user sessions.

<details>
<summary><b>Answer & Explanation</b></summary>

**Answer: B**

**Explanation:** Static classes are stateless, do not need instantiation, and cannot implement interfaces. They are perfect for math helper utilities (`MathHelper`). If a class needs dependencies injected (like a logger or database gateway), it *must* be instance-based to allow dependency injection and modular unit testing.
</details>

---

## Part 2: Code Troubleshooting & Review Scenarios

### Scenario A: The Memory Leak in Stock Ticker
A developer submits this code for an Event-driven Stock ticker. What is the bug?
```csharp
public class Subscriber
{
    public Subscriber(StockTicker ticker)
    {
        ticker.OnPriceChanged += HandlePriceUpdate;
    }

    private void HandlePriceUpdate(decimal price)
    {
        Console.WriteLine($"Price update received: {price}");
    }
}
```
* **Analysis:** If the `Subscriber` instance goes out of scope elsewhere in the app, the garbage collector **cannot** collect it. The `StockTicker` instance holds a delegate chain reference pointing to `Subscriber.HandlePriceUpdate`.
* **Fix:** The subscriber must implement `IDisposable` or have a mechanism to unsubscribe:
  ```csharp
  public void Unsubscribe(StockTicker ticker)
  {
      ticker.OnPriceChanged -= HandlePriceUpdate;
  }
  ```

---

### Scenario B: The Blocking Parallel Loop
Why is this parallel code not yielding the expected performance benefits?
```csharp
Parallel.ForEach(items, item =>
{
    // Awaiting an async operation synchronously
    FetchDataFromApiAsync(item).GetAwaiter().GetResult(); 
});
```
* **Analysis:** `Parallel.ForEach` is designed for CPU-bound computation tasks (e.g. image processing). Using it for network I/O with `.GetResult()` forces threadpool threads to lock/block synchronously, causing thread pool starvation.
* **Fix:** For asynchronous I/O loops, use `Task.WhenAll` with concurrency limits instead:
  ```csharp
  var tasks = items.Select(item => FetchDataFromApiAsync(item));
  await Task.WhenAll(tasks);
  ```

---

## Part 3: High-Frequency Interview Questions ("Gotchas")

Here are the key "Gotchas" interviewers look for when validating your C# knowledge:

1. **"What is the difference between `Task.Delay` and `Thread.Sleep`?"**
   * *Gotcha Answer:* Both pause execution.
   * *Correct Answer:* `Thread.Sleep` is a synchronous block that freezes the current operating system thread, preventing it from doing other work. `Task.Delay` is an asynchronous, non-blocking timer that releases the executing thread back to the thread pool/loop.

2. **"Does a class finalizer guarantee resource cleanup?"**
   * *Gotcha Answer:* Yes, the finalizer is called when the object is destroyed.
   * *Correct Answer:* No. Finalizers are non-deterministic; they execute only when the Garbage Collector decides to reclaim memory. If the app shuts down abruptly, finalizers might not run. Therefore, always use `IDisposable.Dispose()` for guaranteed resource cleanup, wrapping usage inside a `using` block.

3. **"Can we make a Singleton thread-safe without locking?"**
   * *Correct Answer:* Yes, by using `Lazy<T>` which is thread-safe by default, or using static initialization (`private static readonly MySingleton _instance = new MySingleton();`), which C# guarantees is executed thread-safe by the class loader before the class is used.
