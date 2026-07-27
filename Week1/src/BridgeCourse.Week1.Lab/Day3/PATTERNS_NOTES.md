# Design Patterns - Short Notes & Use Cases

This file compiles short descriptions (3-5 lines) and one realistic software engineering use case for the 8 requested design patterns.

---

### 1. Builder (Creational)
- **Short Note**: Separatess the construction of a complex object from its representation so that the same construction process can create different representations. It provides step-by-step construction using a fluent API and is highly useful when an object has many optional properties, preventing constructor bloating.
- **Use Case**: Constructing a SQL query string dynamically (`QueryBuilder`) with optional clauses like `.Select()`, `.Where()`, `.OrderBy()`, and `.Limit()` before executing `.Build()`.

### 2. Prototype (Creational)
- **Short Note**: Specifies the kinds of objects to create using a prototypical instance, and creates new objects by copying this prototype. In C#, this is typically achieved via `ICloneable` or binary/JSON deep/shallow copying, avoiding the performance hit of repeating costly database fetches or file initializations.
- **Use Case**: Creating clones of heavy game characters (like a template monster with fixed base stats, graphics, and mesh) instead of loading assets repeatedly from disk.

### 3. Decorator (Structural)
- **Short Note**: Attaches additional responsibilities to an object dynamically. Decorators provide a flexible alternative to subclassing for extending functionality without altering existing source code. It wraps the core object while implementing the same interface.
- **Use Case**: Adding encryption, compression, or caching wrapper classes over a standard file or stream reader (e.g., `CryptoStream` wrapping `FileStream`).

### 4. Command (Behavioral)
- **Short Note**: Encapsulates a request as an object, thereby letting you parameterize clients with different requests, queue or log requests, and support undoable operations. It decouples the sender of the request from the receiver that executes it.
- **Use Case**: Implementing an undo/redo manager in a text editor where actions like `InsertTextCommand` or `DeleteTextCommand` are pushed onto a stack.

### 5. Template Method (Behavioral)
- **Short Note**: Defines the skeleton of an algorithm in a method, deferring some steps to subclasses. Subclasses can redefine certain steps of an algorithm without changing the algorithm's structure. It uses inheritance and hook methods.
- **Use Case**: A data parsing framework that implements a `ParseData()` template containing: `OpenConnection()`, `ReadSource()`, `ExtractRecords()`, and `CloseConnection()`, where subclasses override `ExtractRecords()` for CSV vs. JSON.

### 6. Mediator (Behavioral)
- **Short Note**: Defines an object that encapsulates how a set of objects interact. It promotes loose coupling by keeping objects from referring to each other explicitly, and lets you vary their interaction independently. Communication is centralized.
- **Use Case**: A chat room application or a workflow library like MediatR where UI components or handlers send requests to a central broker instead of communicating directly with each other.

### 7. Chain of Responsibility (Behavioral)
- **Short Note**: Avoids coupling the sender of a request to its receiver by giving more than one object a chance to handle the request. Receivers are chained, and the request passes along the chain until an object handles it.
- **Use Case**: Middleware pipeline processing in ASP.NET Core where an incoming HTTP request is sequentially evaluated by logging middleware, auth middleware, and caching middleware.

### 8. State (Behavioral)
- **Short Note**: Allows an object to alter its behavior when its internal state changes. The object will appear to change its class. It replaces massive `switch` or `if-else` blocks with concrete state classes that implement the transitions.
- **Use Case**: A vending machine controller transition state diagram containing: `NoCoinState`, `HasCoinState`, `DispensingState`, and `OutOfStockState`.
