---
name: dotnet-feature-planner
description: 'Analyze .NET feature requests before implementation. Use when planning API endpoints, Clean Architecture changes, database modifications, or refactoring.'
---

# .NET Feature Planner

## Workflow
1. Inspect the solution, project references, existing patterns, and relevant tests.
2. Restate the requirement and identify missing information.
3. Trace the affected layers: API, Application, Domain, and Infrastructure, based on the architecture actually present.
4. Identify DTOs, validation, authorization, database changes, and error handling.
5. Create an ordered implementation plan with files to create or modify.
6. Identify risks and test scenarios.

## Constraints
- Do not modify files during planning.
- Do not assume project names or architecture that do not exist.
- Do not propose a database migration unless the schema needs to change.
- Ask questions if a critical requirement is ambiguous.

## Output
Provide the requirement summary, proposed file changes, implementation steps, test plan, and risks.