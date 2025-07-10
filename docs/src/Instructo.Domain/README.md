# Instructo.Domain - Domain Layer

The Domain layer represents the core business logic of the Instructo application. Following Domain-Driven Design (DDD) principles, it contains the essential business entities, value objects, and domain services that model the driving school management domain.

## Purpose

Instructo.Domain is responsible for:
- Defining core business entities with encapsulated business logic
- Implementing value objects for type safety and validation
- Establishing domain interfaces and contracts
- Enforcing business rules and invariants
- Providing domain events for decoupled communication

## Domain-Driven Design Principles

### Rich Domain Model
Entities contain business logic rather than being anemic data containers:
- Business methods on entities
- Encapsulated state changes
- Invariant enforcement
- Domain event publishing

### Ubiquitous Language
Domain concepts use language from the business domain:
- School, Owner, Certificate, Category
- Approval, Registration, Validation
- County, City, Address, BusinessHours

### Bounded Context
The domain represents the driving school management bounded context with clear boundaries and responsibilities.

## Key Components

### [[Entities/README|Entities]]
Core business entities with identity and lifecycle management.

**Primary Entities:**
- [[Entities/School|School]] - Central business entity representing driving schools
- [[Entities/ApplicationUser|ApplicationUser]] - Users in the system (owners, admins)
- [[Entities/Address|Address]] - Geographical location information
- [[Entities/Image|Image]] - File management for school images

### [[ValueObjects/README|Value Objects]]
Immutable objects that represent domain concepts without identity.

**Key Value Objects:**
- [[ValueObjects/SchoolId|SchoolId]], [[ValueObjects/UserId|UserId]] - Strongly-typed identifiers
- [[ValueObjects/Email|Email]], [[ValueObjects/PhoneNumber|PhoneNumber]] - Validated contact information
- [[ValueObjects/BussinessHours|BussinessHours]] - Complex time management
- [[ValueObjects/Slug|Slug]] - URL-friendly identifiers

### Interfaces
Domain service contracts and repository interfaces that define external dependencies without implementation details.

**Repository Interfaces:**
- `IQueryRepository<TEntity, TKey>` - Generic read operations
- `ICommandRepository<TEntity, TKey>` - Generic write operations
- `ISchoolCommandRepository` - School-specific operations
- `IUnitOfWork` - Transaction management

**Service Interfaces:**
- `IIdentityService` - User management abstraction
- `IEmailService` - Email notification abstraction

### Common
Base classes and shared domain concepts used throughout the domain layer.

**Base Classes:**
- `BaseEntity<TKey>` - Base entity with common properties
- `ValueObject` - Base class for value objects
- `BaseEvent` - Domain event base class

**Shared Concepts:**
- `Result<T>` - Consistent error handling pattern
- `Error` - Domain error representation
- `FlexContext<T>` - Operation chaining with error handling

## Core Domain Entities

### School Entity
The central business entity representing a driving school:

**Key Properties:**
- Basic Information: Name, Description, Slogan
- Contact Information: Email, Phone, Website
- Location: Address, City, County
- Business Details: Categories, Certificates, Hours
- System Properties: Approval status, Slug, RowVersion

**Business Methods:**
- Approval workflow management
- Category and certificate management
- Link and image management
- Validation and business rule enforcement

### User Management
User entities integrated with ASP.NET Core Identity:

**ApplicationUser**:
- Extends IdentityUser with domain-specific properties
- One-to-one relationship with School (for owners)
- Role-based functionality

**ApplicationRole**:
- Extends IdentityRole for custom role management
- Support for hierarchical roles

### Geographical Entities
Location management for Romanian context:

**County**: Administrative divisions
**City**: Municipal areas within counties
**Address**: Complete address information with spatial data support

## Value Objects Design

### Strongly-Typed Identifiers
Prevent primitive obsession and provide type safety:

```csharp
public sealed class SchoolId : ValueObject
{
    public Guid Value { get; }
    
    private SchoolId(Guid value) => Value = value;
    
    public static SchoolId New() => new(Guid.NewGuid());
    public static SchoolId From(Guid value) => new(value);
}
```

### Validated Value Types
Encapsulate validation logic within value objects:

```csharp
public sealed class Email : ValueObject
{
    public string Value { get; }
    
    private Email(string value) => Value = value;
    
    public static Result<Email> Create(string email)
    {
        // Validation logic
        return email.IsValidEmail() ? 
            Result.Success(new Email(email)) : 
            Result.Failure<Email>(Error.Validation("Invalid email format"));
    }
}
```

### Complex Business Concepts
Model complex domain concepts as value objects:

**BussinessHours**: Represents operating hours with complex rules
**Slug**: URL-friendly identifiers with generation rules
**LegalName**: Business names with validation requirements

## Domain Events

### Event-Driven Architecture
Domain entities can publish events for decoupled communication:

```csharp
public abstract class BaseEntity<TKey>
{
    private readonly List<BaseEvent> _domainEvents = new();
    
    protected void AddDomainEvent(BaseEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }
    
    public void ClearDomainEvents() => _domainEvents.Clear();
    public IReadOnlyCollection<BaseEvent> GetDomainEvents() => _domainEvents.AsReadOnly();
}
```

### Common Events
- School approval status changes
- User registration events
- Business information updates
- System state transitions

## Business Rules and Invariants

### Entity Invariants
Business rules enforced at the entity level:
- School must have valid contact information
- Approved schools must meet minimum requirements
- Users can only own one school
- Business hours must be valid time ranges

### Validation Strategy
Multi-layered validation approach:
1. **Value Object Validation**: Basic format and range validation
2. **Entity Invariants**: Business rule enforcement
3. **Domain Service Validation**: Complex cross-entity rules
4. **Application Layer Validation**: Use case specific rules

## Error Handling

### Result Pattern
Consistent error handling throughout the domain:

```csharp
public class Result<T>
{
    public bool IsSuccess { get; }
    public T? Data { get; }
    public IEnumerable<Error> Errors { get; }
    
    // Factory methods for success/failure
    public static Result<T> Success(T data);
    public static Result<T> Failure(Error error);
    public static Result<T> Failure(IEnumerable<Error> errors);
}
```

### Error Types
Domain-specific error classifications:
- Validation errors
- Business rule violations
- Concurrency conflicts
- Resource not found errors

### FlexContext Pattern
Chainable operations with error propagation:

```csharp
var result = FlexContext<School>.Start()
    .Then(ValidateSchoolData)
    .Then(CheckApprovalRequirements)
    .Then(SaveSchool)
    .GetResult();
```

## Concurrency Handling

### Optimistic Concurrency
Using row versioning for conflict detection:
- `RowVersion` timestamp on entities
- Automatic conflict detection by Entity Framework
- Business-friendly concurrency error messages

### Aggregate Consistency
Ensuring consistency within aggregate boundaries:
- Transaction scope management
- Invariant preservation
- State transition validation

## Technology Dependencies

### Minimal Dependencies
The domain layer has minimal external dependencies:
- **ASP.NET Core Identity** - For user entities only
- **NetTopologySuite** - For spatial data support
- **System libraries** - Basic .NET functionality

### Dependency Inversion
External concerns are abstracted through interfaces:
- Repository pattern for data access
- Service interfaces for external operations
- Event handling abstractions

## Testing Approach

### Unit Testing
Domain entities and value objects are easily unit testable:
- No external dependencies
- Pure business logic testing
- Validation rule verification
- Business rule enforcement testing

### Domain Service Testing
Testing complex business operations:
- Mock repository dependencies
- Test business rule combinations
- Verify domain event publication
- Error scenario validation

For detailed information about specific domain components, explore the Entities and ValueObjects documentation sections.