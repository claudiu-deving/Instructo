# Instructo.Application - Application Layer

The Application layer implements business use cases through the CQRS (Command Query Responsibility Segregation) pattern using MediatR. This layer orchestrates business logic, coordinates between the domain and infrastructure layers, and ensures proper validation and error handling.

## Purpose

Instructo.Application is responsible for:
- Implementing business use cases as commands and queries
- Orchestrating business logic flow
- Providing validation pipeline through FluentValidation
- Managing cross-cutting concerns via behaviors
- Coordinating between domain entities and external services

## Architecture Pattern

### CQRS Implementation
The application follows strict CQRS separation:

**Commands** - Write operations that modify system state:
- Create, Update, Delete operations
- Business workflow operations
- State transition commands

**Queries** - Read operations that return data:
- Data retrieval operations
- Reporting and analytics
- Search and filtering operations

## Key Components

### [[Schools/README|Schools]]
School domain operations including registration, management, and approval workflows.

**Main Operations:**
- [[Schools/Commands/README|Commands]] - School creation, updates, and approval management
- [[Schools/Queries/README|Queries]] - School retrieval, search, and listing operations

### [[Users/README|Users]]
User management operations for authentication, authorization, and profile management.

**Main Operations:**
- [[Users/Commands/README|Commands]] - User registration, updates, and authentication
- [[Users/Queries/README|Queries]] - User retrieval and profile operations

### Abstractions/Messaging
Base interfaces and contracts for CQRS implementation:

**Core Interfaces:**
- `ICommand<TResult>` - Command without return value
- `ICommand<TCommand, TResult>` - Command with return value
- `IQuery<TResult>` - Query interface
- `ICommandHandler<TCommand, TResult>` - Command handler contract
- `IQueryHandler<TQuery, TResult>` - Query handler contract

### Behaviors
Cross-cutting concerns implemented as MediatR pipeline behaviors:

**ValidationPipelineBehavior** - Automatic validation for all commands:
- FluentValidation integration
- Validation error aggregation
- Early validation failure return
- Consistent validation response format

## Technology Stack

### Core Dependencies
- **Custom MediatR Implementation** - CQRS pattern implementation
- **FluentValidation** - Validation pipeline
- **ASP.NET Core Identity** - User management integration
- **AutoMapper** - Object mapping (where needed)

### Package References
```xml
<PackageReference Include="FluentValidation.DependencyInjectionExtensions" />
<PackageReference Include="Microsoft.AspNetCore.Identity" />
<PackageReference Include="Microsoft.Extensions.DependencyInjection.Abstractions" />
```

## CQRS Pattern Details

### Command Pattern
Commands represent write operations that change system state:

```csharp
public record CreateSchoolCommand(
    string Name,
    string Email,
    string PhoneNumber,
    // ... other properties
) : ICommand<CreateSchoolCommand, Result<SchoolId>>;
```

**Command Characteristics:**
- Immutable record types
- Clear intent and purpose
- Validation attributes where appropriate
- Result<T> return type for error handling

### Query Pattern
Queries represent read operations that return data:

```csharp
public record GetSchoolBySlugQuery(string Slug) : IQuery<Result<SchoolDto>>;
```

**Query Characteristics:**
- Immutable record types
- Specific data retrieval purpose
- DTO return types
- No side effects

### Handler Pattern
Handlers implement the actual business logic:

```csharp
public class CreateSchoolCommandHandler : ICommandHandler<CreateSchoolCommand, Result<SchoolId>>
{
    public async Task<Result<SchoolId>> Handle(CreateSchoolCommand command, CancellationToken cancellationToken)
    {
        // Implementation
    }
}
```

## Validation Strategy

### FluentValidation Integration
Every command includes validation through FluentValidation:

**Static Validation** - Basic property validation:
```csharp
public class CreateSchoolCommandStaticValidator : AbstractValidator<CreateSchoolCommand>
{
    public CreateSchoolCommandStaticValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        // ... other rules
    }
}
```

**Dynamic Validation** - Business rule validation:
- Database constraint checking
- Business rule enforcement
- External service validation

### Validation Pipeline
The `ValidationPipelineBehavior` automatically:
1. Discovers all validators for the command
2. Executes validation rules
3. Aggregates validation errors
4. Returns validation failure if any errors exist
5. Continues to handler if validation passes

## Error Handling

### Result Pattern
All operations return `Result<T>` for consistent error handling:

```csharp
public class Result<T>
{
    public bool IsSuccess { get; }
    public T? Data { get; }
    public IEnumerable<Error> Errors { get; }
}
```

**Error Types:**
- Validation errors
- Business rule violations
- Infrastructure failures
- External service errors

### Error Propagation
Errors are propagated up through the layers:
1. Domain layer validation
2. Application layer business rules
3. Infrastructure layer technical issues
4. Presentation layer user-friendly messages

## Dependency Injection

### Service Registration
The application layer registers services in the DI container:

```csharp
services.AddMediatR(Assembly.GetExecutingAssembly());
services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationPipelineBehavior<,>));
```

### Handler Discovery
MediatR automatically discovers and registers:
- Command handlers
- Query handlers
- Validation pipeline behaviors
- Other pipeline behaviors

## Testing Strategy

### Unit Testing
Each handler is unit tested in isolation:
- Mock external dependencies
- Test business logic
- Verify error scenarios
- Test validation rules

### Integration Testing
Commands and queries are tested end-to-end:
- Real database interactions
- Complete validation pipeline
- Full error handling flow
- Authentication and authorization

## Performance Considerations

### Async/Await Pattern
All handlers use async/await for:
- Database operations
- External service calls
- CPU-intensive operations
- Scalability improvements

### Caching Strategy
Where appropriate:
- Query result caching
- Computed value caching
- Configuration caching
- External service response caching

## Security Considerations

### Authorization
Commands and queries include authorization checks:
- Role-based authorization
- Resource-based authorization
- Business rule authorization
- Data access authorization

### Data Protection
Sensitive data handling:
- Password hashing
- PII protection
- Audit logging
- Data encryption where required

For detailed information about specific domain operations, explore the Schools and Users documentation sections.