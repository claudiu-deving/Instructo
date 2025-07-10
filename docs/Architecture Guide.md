# Architecture Guide

The Instructo application follows Clean Architecture principles combined with Domain-Driven Design (DDD) and CQRS patterns to create a maintainable, testable, and scalable driving school management system.

## Architectural Principles

### Clean Architecture
The application is structured in concentric layers with dependencies pointing inward:

```
┌─────────────────────────────────────┐
│            Infrastructure           │  ← External concerns
├─────────────────────────────────────┤
│              API Layer              │  ← Presentation
├─────────────────────────────────────┤
│           Application Layer         │  ← Business Logic
├─────────────────────────────────────┤
│            Domain Layer             │  ← Core Business
└─────────────────────────────────────┘
```

**Dependency Rules:**
- Inner layers never depend on outer layers
- Domain layer has no external dependencies
- Infrastructure implements domain interfaces
- API layer orchestrates use cases

### Domain-Driven Design (DDD)
The architecture centers around the business domain:

**Core Concepts:**
- **[[src/Instructo.Domain/README|Rich Domain Model]]**: Business logic in entities
- **[[src/Instructo.Domain/ValueObjects/README|Value Objects]]**: Type-safe domain concepts
- **Aggregate Boundaries**: Consistency and transaction boundaries
- **Ubiquitous Language**: Shared vocabulary between technical and business teams

**Domain Modeling:**
```
School Aggregate Root
├── SchoolId (Value Object)
├── Email, Phone (Value Objects)
├── Address (Entity)
├── BusinessHours (Value Object)
└── Images (Collection)

User Aggregate Root
├── UserId (Value Object)  
├── School (Reference)
└── Roles (Collection)
```

### CQRS (Command Query Responsibility Segregation)
Separation of read and write operations:

**Command Side** (Write Operations):
- Modify system state
- Business rule validation
- Domain event publishing
- Optimistic concurrency handling

**Query Side** (Read Operations):
- Data retrieval optimized for specific use cases
- Read-only operations
- Performance-optimized queries
- DTO projections

## Layer Responsibilities

### [[src/Instructo.Domain/README|Domain Layer]]
**Responsibilities:**
- Core business entities and logic
- Domain services and specifications
- Business rule enforcement
- Domain event definitions

**Key Components:**
- `School`, `User`, `Address` entities
- `SchoolId`, `Email`, `PhoneNumber` value objects
- `ISchoolRepository`, `IUnitOfWork` interfaces
- `Result<T>` and error handling patterns

**Dependencies:** None (pure business logic)

### [[src/Instructo.Application/README|Application Layer]]
**Responsibilities:**
- Use case implementation
- Business workflow orchestration
- Cross-cutting concerns (validation, logging)
- DTO definitions and mapping

**Key Components:**
- Command/Query handlers
- MediatR pipeline behaviors
- FluentValidation validators
- Application services

**Dependencies:** Domain layer, MediatR abstractions

### [[src/Instructo.Infrastructure/README|Infrastructure Layer]]
**Responsibilities:**
- Data persistence implementation
- External service integrations
- Infrastructure services
- Technology-specific implementations

**Key Components:**
- Entity Framework context and repositories
- Identity service implementations
- Email service implementations
- Database configurations and migrations

**Dependencies:** Domain, Application layers

### [[src/Instructo.Api/README|API Layer]]
**Responsibilities:**
- HTTP endpoint definitions
- Request/response handling
- Authentication/authorization
- API documentation

**Key Components:**
- Minimal API endpoints
- Middleware pipeline
- Authentication configuration
- OpenAPI/Swagger setup

**Dependencies:** Application, Infrastructure layers

## Architectural Patterns

### Repository Pattern
Data access abstraction with clear contracts:

```csharp
// Generic contracts in Domain
public interface IQueryRepository<TEntity, TKey>
{
    Task<TEntity?> GetByIdAsync(TKey id);
    Task<PagedResult<TEntity>> GetPagedAsync(int page, int pageSize);
}

public interface ICommandRepository<TEntity, TKey>
{
    Task<TEntity> AddAsync(TEntity entity);
    Task UpdateAsync(TEntity entity);
    Task DeleteAsync(TEntity entity);
}

// Specific contracts for complex operations
public interface ISchoolCommandRepository : ICommandRepository<School, SchoolId>
{
    Task<bool> IsSlugUniqueAsync(string slug);
    Task UpdateApprovalStatusAsync(SchoolId id, bool isApproved);
}
```

### Unit of Work Pattern
Transaction coordination and domain event processing:

```csharp
public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken = default);
}
```

**Responsibilities:**
- Transaction management
- Domain event collection and publishing
- Optimistic concurrency handling
- Audit field management

### Result Pattern
Consistent error handling across all layers:

```csharp
public class Result<T>
{
    public bool IsSuccess { get; }
    public T? Data { get; }
    public IEnumerable<Error> Errors { get; }
    
    // Factory methods
    public static Result<T> Success(T data);
    public static Result<T> Failure(Error error);
}
```

**Benefits:**
- Explicit error handling
- No exception-based control flow
- Composable operations
- Clear success/failure semantics

### MediatR Pattern
Decoupled request/response handling:

```csharp
// Command definition
public record CreateSchoolCommand(string Name, string Email) : ICommand<Result<SchoolId>>;

// Handler implementation
public class CreateSchoolCommandHandler : ICommandHandler<CreateSchoolCommand, Result<SchoolId>>
{
    public async Task<Result<SchoolId>> Handle(CreateSchoolCommand command, CancellationToken cancellationToken)
    {
        // Implementation
    }
}
```

**Pipeline Behaviors:**
- Validation behavior (automatic validation)
- Logging behavior (request/response logging)
- Performance behavior (timing measurements)
- Transaction behavior (automatic transactions)

## Security Architecture

### Authentication Flow
JWT-based authentication with role-based authorization:

```
1. User credentials → API
2. Validate credentials → Identity Service
3. Generate JWT token → Include user claims/roles
4. Return token → Client stores token
5. Subsequent requests → Bearer token in header
6. Validate token → Extract claims
7. Authorize request → Role/policy-based decisions
```

### Authorization Policies
Hierarchical role-based access control:

**IronMan Policy** (Super Admin):
- Full system access
- User management
- System configuration

**AdminOnly Policy** (Admin + IronMan):
- School approval/rejection
- Administrative functions
- Reporting access

**SchoolOwners Policy** (Owner + Admin + IronMan):
- Manage owned school
- Update business information
- View school analytics

### Data Protection
- Password hashing with secure algorithms
- PII data encryption at rest
- Audit logging for sensitive operations
- GDPR compliance considerations

## Performance Architecture

### Database Optimization
- Entity Framework query optimization
- Index strategy for common queries
- Connection pooling
- Read/write separation possibilities

### Caching Strategy
- Application-level caching for static data
- Query result caching where appropriate
- Distributed caching readiness
- Cache invalidation strategies

### Scalability Considerations
- Stateless application design
- Database connection efficiency
- Horizontal scaling readiness
- Load balancing compatibility

## Observability Architecture

### Logging Strategy
Structured logging with correlation tracking:

```csharp
// Correlation ID middleware
app.Use(async (context, next) =>
{
    var correlationId = Guid.NewGuid().ToString();
    context.Response.Headers.Add("X-Correlation-ID", correlationId);
    using (LogContext.PushProperty("CorrelationId", correlationId))
    {
        await next();
    }
});
```

### Monitoring and Tracing
- OpenTelemetry integration
- Distributed tracing across layers
- Performance metric collection
- Health check endpoints

### Error Tracking
- Centralized error logging
- Error aggregation and alerting
- Performance bottleneck identification
- User experience monitoring

## Testing Architecture

### [[tests/README|Testing Strategy]]
Multi-layered testing approach:

**Unit Tests**:
- Domain entity testing
- Value object validation
- Business logic verification
- Handler unit testing with mocks

**Integration Tests**:
- API endpoint testing
- Database integration testing
- Authentication flow testing
- End-to-end scenario testing

**Infrastructure Testing**:
- Repository implementation testing
- External service integration testing
- Configuration validation testing

## Deployment Architecture

### Container Strategy
Docker containerization for consistent deployment:

```dockerfile
# Multi-stage build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
# Build application

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
# Runtime configuration
```

### Cloud-Native Features
- Health check endpoints
- Configuration through environment variables
- Graceful shutdown handling
- Horizontal scaling support

### Database Deployment
- Migration-based schema updates
- Seed data management
- Backup and recovery strategies
- Performance monitoring

## Future Architecture Considerations

### Microservices Evolution
Current monolithic design allows future decomposition:
- Domain boundaries already defined
- Clear service interfaces
- Event-driven communication readiness
- Independent deployment preparation

### Event-Driven Architecture
Domain events provide foundation for:
- Asynchronous processing
- Service decoupling
- Event sourcing possibilities
- Integration with external systems

### API Evolution
Current design supports:
- API versioning strategies
- Backward compatibility
- Client SDK generation
- GraphQL potential integration

This architecture provides a solid foundation for the Instructo application while maintaining flexibility for future growth and evolution.