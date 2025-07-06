# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Architecture

Instructo is a C# .NET 9 application using Clean Architecture principles with the following layers:

- **Domain** (`src/Instructo.Domain/`): Core business logic, entities, value objects, and domain interfaces
- **Application** (`src/Instructo.Application/`): Business logic implementation using CQRS pattern with MediatR
- **Infrastructure** (`src/Instructo.Infrastructure/`): Data access, external services, and infrastructure concerns
- **API** (`src/Instructo.Api/`): Web API endpoints using ASP.NET Core Minimal APIs
- **Shared** (`src/Instructo.Shared/`): Common utilities and shared types

## Key Technologies

- **.NET 9** with C# implicit usings and nullable reference types enabled
- **Entity Framework Core 9.0** with SQL Server and migrations
- **MediatR** for CQRS pattern implementation
- **FluentValidation** for validation pipeline
- **ASP.NET Core Identity** for authentication and authorization
- **JWT Bearer** authentication
- **AutoMapper** for object mapping
- **Serilog** for logging with Seq integration
- **OpenTelemetry** for observability
- **xUnit** for testing with in-memory database

## Common Development Commands

### Build and Run
```bash
dotnet build
dotnet run --project src/Instructo.Api/Api.csproj
```

### Database Management
```bash
# Add migration
dotnet ef migrations add MigrationName --project src/Instructo.Infrastructure/Infrastructure.csproj --startup-project src/Instructo.Api/Api.csproj

# Update database
dotnet ef database update --project src/Instructo.Infrastructure/Infrastructure.csproj --startup-project src/Instructo.Api/Api.csproj
```

### Testing
```bash
# Run all tests
dotnet test

# Run specific test project
dotnet test tests/Instructo.IntegrationTests/Instructo.IntegrationTests.csproj

# Run with coverage
dotnet test --collect:"XPlat Code Coverage"
```

### Docker
```bash
# Build and run with Docker Compose
docker-compose up --build

# Build image
docker build -t instructo-api .
```

## Architecture Patterns

### CQRS Implementation
- Commands and queries are separated using MediatR
- Commands are in `Application/[Entity]/Commands/` folders
- Queries are in `Application/[Entity]/Queries/` folders
- All handlers implement `ICommandHandler<TCommand, TResult>` or `IQueryHandler<TQuery, TResult>`

### Domain-Driven Design
- Rich domain entities with business logic encapsulated
- Value objects for complex types (e.g., `SchoolId`, `Email`, `PhoneNumber`)
- Domain services for complex business operations
- FlexContext pattern for chaining operations with error handling

### Repository Pattern
- Generic interfaces: `IQueryRepository<TEntity, TKey>` and `ICommandRepository<TEntity, TKey>`
- Specific repositories for complex queries: `ISchoolCommandRepository`, `IUserQueries`
- Entity Framework configurations in `Infrastructure/Data/Configurations/`

### Validation Pipeline
- FluentValidation integrated with MediatR pipeline
- Validation behaviors automatically applied to all commands
- Custom validators in command folders (e.g., `CreateSchoolCommandStaticValidator`)

## Key Domain Entities

### School
- Core entity representing driving schools
- Has approval status (`IsApproved` property)
- Supports concurrency with `RowVersion` timestamp
- Rich business methods for managing vehicle categories, certificates, and links

### User Management
- Uses ASP.NET Core Identity with `ApplicationUser` and `ApplicationRole`
- Role-based authorization with policies:
  - `IronMan`: Super admin access
  - `AdminOnly`: Admin, Owner, and IronMan roles
  - `SchoolOwners`: School owner specific access

### Value Objects
- `SchoolId`, `UserId`, `ImageId` for type safety
- `Email`, `PhoneNumber`, `LegalName` for validation
- `BussinessHours` for complex time management

## Testing Infrastructure

### Integration Tests
- Uses `Microsoft.AspNetCore.Mvc.Testing` for API testing
- In-memory database with Entity Framework Core
- Test fixtures: `IntegrationTestFixture` and `InMemoryTestFixture`
- Authentication helper: `AuthentificationHelper` for test user creation

### Test Structure
- Base class: `IntegrationTestBase` with common setup
- Test collections for parallel execution
- Bogus library for test data generation
- Moq for mocking dependencies

## Configuration

### Required Settings
- `DefaultConnection`: SQL Server connection string
- `JwtSettings`: JWT configuration (Secret, Issuer, Audience)
- `Seq:ServerUrl`: Seq logging server URL
- `OpenTelemetry:OtlpEndpoint`: OpenTelemetry collector endpoint

### Development Setup
- Use User Secrets for sensitive configuration
- Docker Compose provided for local development
- Serilog configured for console and Seq output

## Current Development State

The project is actively developed with recent focus on:
- School approval status functionality
- Concurrency handling with optimistic locking
- Integration test improvements
- Database migration management

## Common Patterns

### Error Handling
- Custom `Result<T>` pattern for operation results
- FlexContext for chaining operations with error propagation
- Validation errors integrated into result pattern

### Logging and Observability
- Correlation ID middleware for request tracing
- OpenTelemetry integration for distributed tracing
- Structured logging with Serilog
- Sensitive data destructuring policy

In this codebase, your job is to be a brainstorming partner. The user will ask general, often high level questions pertaining to architecture.