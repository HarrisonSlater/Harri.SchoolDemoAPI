# Copilot Instructions for Harri.SchoolDemoAPI

## Project Overview

This is a demo REST API built with **ASP.NET Core 8.0** and **C# 12** that manages students, schools, and applications. It uses **Dapper** as a micro-ORM against **SQL Server**, follows a Controller → Repository → Database layered architecture, and is containerized with Docker.

## Solution Structure

- `src/Harri.SchoolDemoAPI/` — Main Web API project (controllers, repositories, filters, OpenAPI config)
- `src/Harri.SchoolDemoAPI.Models/` — Shared DTOs, entities, and validation attributes
- `src/Harri.SchoolDemoAPI.Client/` — RestSharp-based HTTP client library (published to NuGet)
- `src/Harri.SchoolDemoAPI.HealthCheckClient/` — Health check client (published to NuGet)
- `src/Harri.SchoolDemoAPI.DatabaseMigrations/` — Database migration executable
- `src/Tests/` — All test projects (Unit, Integration, E2E, Contract, Common)

## Build, Test, and Run

```bash
# Restore and build
dotnet restore
dotnet build

# Run the API
dotnet run --project src/Harri.SchoolDemoAPI/Harri.SchoolDemoAPI.csproj

# Run unit tests
dotnet test src/Tests/Harri.SchoolDemoAPI.Tests.Unit/

# Run integration tests (requires SQL Server)
dotnet test src/Tests/Harri.SchoolDemoAPI.Tests.Integration/

# Run E2E tests (requires running API + SQL Server)
dotnet test src/Tests/Harri.SchoolDemoAPI.Tests.E2E/

# Run contract tests
dotnet test src/Tests/Contract/Harri.SchoolDemoAPI.Tests.Contract.Consumer/
dotnet test src/Tests/Contract/Harri.SchoolDemoAPI.Tests.Contract.Provider/
```

## Coding Conventions

### Naming
- **Namespaces:** `Harri.SchoolDemoAPI`, `Harri.SchoolDemoAPI.Controllers`, `Harri.SchoolDemoAPI.Repository`, `Harri.SchoolDemoAPI.Models`, `Harri.SchoolDemoAPI.Models.Dto`
- **Classes:** PascalCase (e.g., `StudentsApiController`, `StudentRepository`)
- **Interfaces:** `I` prefix (e.g., `IStudentRepository`, `IDbConnectionFactory`)
- **DTOs:** `Dto` suffix (e.g., `NewStudentDto`, `StudentDto`)
- **Methods:** PascalCase (e.g., `AddStudent`, `GetStudentById`)
- **JSON serialization:** camelCase via `JsonNamingPolicy.CamelCase`

### Code Style
- Nullable reference types are enabled — the main API project uses `<Nullable>annotations</Nullable>`, other projects use `<Nullable>enable</Nullable>`
- Controllers use `[ApiController]`, `[Produces]`, `[Consumes]` attributes
- API documentation uses `[SwaggerOperation]` and `[SwaggerResponse]` attributes
- Non-testable code is marked with `[ExcludeFromCodeCoverage]`
- Code style rules are defined in `.editorconfig`

### Architecture Patterns
- **Repository pattern** for data access — controllers depend on repository interfaces, not implementations
- **Dependency injection** configured in `Startup.cs`
- **Dapper** for SQL queries — no Entity Framework
- **Serilog** for structured logging (Console, File, Seq, Application Insights sinks)
- Health checks at `/health` for SQL Server connectivity

## Testing

- **Framework:** NUnit 3 with FluentAssertions and Moq
- **Unit tests:** Mock repository dependencies with Moq; assert with FluentAssertions
- **Integration tests:** Run against a real SQL Server container; tests create and clean up their own data
- **Contract tests:** PactNet for consumer-driven contract testing
- **E2E tests:** Use the RestSharp client library against a running API instance
- **Code coverage:** Collected via Coverlet with `CodeCoverage.runsettings`

## Infrastructure

- **Database:** SQL Server accessed via Dapper; connection string in `appsettings.json`
- **Docker:** Multi-stage Dockerfile in `src/Harri.SchoolDemoAPI/`; exposes port 8080
- **CI/CD:** Azure DevOps pipeline (`pipeline/azure-pipelines.yml`) — builds, tests, publishes Docker images and NuGet packages
