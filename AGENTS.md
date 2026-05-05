# Agent Instructions

This repository contains the Northwind Traders Order Management System.

Before creating or modifying code, always read:

- docs/northwind_traders_vs_code_agent_context.md
- .github/copilot-instructions.md
- README.md

## Project Rules

- Use .NET 10 and target framework net10.0.
- Use ASP.NET Core Web API with controllers.
- Use Clean Architecture.
- Use SQL Server with the Northwind database.
- Use Entity Framework Core Database First.
- Use Repository Pattern and Unit of Work.
- Use FluentValidation for backend validation.
- Use xUnit, Moq, and FluentAssertions for backend tests.
- Use Vue 3 + Quasar + TypeScript for the frontend.
- Do not implement authentication unless explicitly requested.
- Treat Docker as a final bonus.
- Keep all source code, file names, folders, variables, classes, methods, comments, UI labels, and documentation in English.

## Architecture Rules

- Domain must not depend on any other project.
- Application may depend only on Domain.
- Infrastructure may depend on Application and Domain.
- Api may depend on Application and Infrastructure.
- Tests may reference the projects they test.

## Do Not Do

- Do not put business logic in controllers.
- Do not access DbContext directly from controllers.
- Do not hardcode API keys or connection strings.
- Do not call real Google Maps APIs in unit tests.
- Do not use Spanish names in code.
- Do not create unrelated features.

## Working Style

- Make small, focused changes.
- Build after structural/backend changes.
- Run tests when test projects are affected.
- Explain assumptions before changing architecture, database schema, or external service behavior.