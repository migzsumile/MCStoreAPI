---
name: dotnet-clean-architecture
description: Build and review .NET APIs using Clean Architecture, dependency injection, EF Core, DTOs, and secure coding practices. Use when creating or modifying .NET backend code.
---

# .NET Clean Architecture Skill

When implementing .NET APIs:

1. Keep the Domain layer independent of infrastructure.
2. Keep business rules in the appropriate application or business layer.
3. Keep database access and EF Core configurations in the Data Access or Infrastructure layer.
4. Use DTOs for API requests and responses; do not expose database entities directly.
5. Use dependency injection and constructor injection.
6. Use async/await for asynchronous I/O operations.
7. Validate inputs and handle errors consistently.
8. Use parameterized database operations and avoid SQL injection.
9. Keep secrets out of source control.
10. Write unit tests for important business rules.

Before making changes, inspect the existing solution structure and follow its established conventions. Avoid unnecessary architectural changes.