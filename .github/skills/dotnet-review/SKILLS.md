---
name: dotnet-review
description: 'Review C# and ASP.NET Core changes for correctness, security, performance, maintainability, and architecture regressions before merging.'
---

# .NET Code Review

Review the changed code and its relevant context.

Check:
- Incorrect business logic and unhandled edge cases
- Broken Clean Architecture boundaries and dependency direction
- Missing validation or authorization
- SQL injection risks and unsafe dynamic queries
- Exposed secrets or sensitive logging
- Incorrect async usage and resource disposal
- EF Core query efficiency and transaction handling
- Error handling, HTTP status codes, and API contract compatibility
- Missing or ineffective automated tests

Prioritize findings by severity. For each finding, provide the affected file and location, explain the impact, and recommend a concrete fix.

Do not invent vulnerabilities. Distinguish confirmed issues from potential risks. Do not modify files during review unless explicitly asked.