# Instructo.Infrastructure - Infrastructure Layer

The Infrastructure layer implements the technical capabilities needed by the application, including data persistence, external service integrations, and infrastructure concerns. This layer provides concrete implementations of domain interfaces and handles all external dependencies.

## Purpose

Instructo.Infrastructure is responsible for:
- Database access and persistence through Entity Framework Core
- External service implementations (email, authentication)
- Data access patterns (Repository, Unit of Work)
- Infrastructure service implementations
- Database migrations and schema management

## Key Components

### [[Data/README|Data Access]]
Database context, repositories, and data persistence layer using Entity Framework Core.

**Main Components:**
- `AppDbContext` - Central database context with comprehensive configuration
- [[Data/Repositories/README|Repositories]] - Domain repository implementations
- [[Data/Configurations/README|Configurations]] - Entity Framework entity configurations
- [[Data/Migrations/README|Migrations]] - Database schema evolution
- `UnitOfWork` - Transaction management implementation

### [[Identity/README|Identity Services]]
Authentication and authorization service implementations.

**Key Services:**
- `IdentityService` - JWT token generation and user management
- Integration with ASP.NET Core Identity
- Custom user and role management

### Services
External service implementations for email, notifications, and other integrations.

**Available Services:**
- `EmailService` - Email notification implementation
- Future: SMS, Push notifications, File storage services

## Technology Stack

### Database Technologies
- **Entity Framework Core 9.0** - Primary ORM
- **SQL Server** - Database engine
- **NetTopologySuite** - Spatial data support for geographical features
- **Code-First Migrations** - Schema management

### Key Dependencies
```xml
<PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" />
<PackageReference Include="Microsoft.EntityFrameworkCore.Tools" />
<PackageReference Include="Microsoft.AspNetCore.Identity.EntityFrameworkCore" />
<PackageReference Include="NetTopologySuite.IO.SqlServerBytes" />
<PackageReference Include="AutoMapper" />
<PackageReference Include="Microsoft.Extensions.Configuration.AzureKeyVault" />
<PackageReference Include="Azure.Identity" />
```

## Database Design

### AppDbContext Configuration
Comprehensive database context setup with:

**Entity Configurations:**
- Fluent API configurations for all entities
- Value object conversions
- Relationship mappings
- Index definitions

**Advanced Features:**
- Audit fields (CreatedAt, UpdatedAt)
- Soft delete support
- Row versioning for optimistic concurrency
- Spatial data types for geographical information

**Performance Optimizations:**
- Query splitting for complex relationships
- Index optimization
- Connection pooling
- Retry policies for resilience

### Entity Framework Configurations

**School Configuration**:
```csharp
public class SchoolConfiguration : IEntityTypeConfiguration<School>
{
    public void Configure(EntityTypeBuilder<School> builder)
    {
        builder.HasKey(s => s.Id);
        builder.Property(s => s.Id).HasConversion(/* SchoolId conversion */);
        builder.Property(s => s.RowVersion).IsRowVersion();
        // ... additional configurations
    }
}
```

**Value Object Conversions**:
- SchoolId, UserId to/from Guid
- Email, PhoneNumber to/from string
- Complex value objects with JSON serialization

**Relationship Mappings**:
- One-to-one: User to School
- One-to-many: County to Cities, School to Images
- Many-to-many: School to Categories (implicit)

## Repository Pattern Implementation

### Generic Repositories
Base repository implementations for common operations:

**IQueryRepository Implementation**:
```csharp
public class QueryRepository<TEntity, TKey> : IQueryRepository<TEntity, TKey>
    where TEntity : BaseEntity<TKey>
{
    public async Task<TEntity?> GetByIdAsync(TKey id, CancellationToken cancellationToken = default)
    public async Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken cancellationToken = default)
    public async Task<PagedResult<TEntity>> GetPagedAsync(int page, int pageSize, CancellationToken cancellationToken = default)
    // ... other operations
}
```

**ICommandRepository Implementation**:
```csharp
public class CommandRepository<TEntity, TKey> : ICommandRepository<TEntity, TKey>
    where TEntity : BaseEntity<TKey>
{
    public async Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken = default)
    public Task UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
    public Task DeleteAsync(TEntity entity, CancellationToken cancellationToken = default)
    // ... other operations
}
```

### Specialized Repositories
Domain-specific repository implementations:

**SchoolCommandRepository**:
- School-specific create/update operations
- Approval workflow support
- Complex validation scenarios

**SchoolQueriesRepository**:
- Advanced school querying capabilities
- Search and filtering operations
- Performance-optimized read operations

**UserQueryRepository**:
- User management queries
- Role-based filtering
- Authentication support queries

## Unit of Work Pattern

### Transaction Management
Coordinating multiple repository operations:

```csharp
public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;
    
    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // Domain event processing
        // Audit field updates
        // Transaction coordination
        return await _context.SaveChangesAsync(cancellationToken);
    }
    
    public async Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken = default)
    {
        // Business logic validation
        // Concurrency conflict handling
        // Domain event publishing
    }
}
```

### Domain Event Handling
Processing domain events during save operations:
- Event collection from entities
- Event publishing through mediator
- Transactional event processing

## Data Seeding

### Initial Data Setup
Seeding essential data for application functionality:

**County and City Data**:
- Romanian counties (județe)
- Major cities within counties
- Geographical coordinate data

**Role Setup**:
- System roles (IronMan, Admin, Owner, Student)
- Default permissions and policies

**Test Data** (Development):
- Sample schools for development
- Test users with different roles
- Sample geographical data

### Migration Strategy
Database schema evolution through migrations:

**Migration Naming**:
- Descriptive names indicating changes
- Date-based ordering
- Feature-specific grouping

**Data Migration**:
- Backward compatibility considerations
- Data transformation scripts
- Rollback strategies

## External Service Integration

### Email Service
Email notification implementation:

```csharp
public class EmailService : IEmailService
{
    public async Task SendEmailAsync(string to, string subject, string body)
    {
        // SMTP configuration
        // Template processing
        // Delivery tracking
        // Error handling
    }
}
```

**Email Features**:
- HTML and text email support
- Template-based email generation
- Attachment support
- Delivery confirmation

### Identity Service
JWT token management and user operations:

```csharp
public class IdentityService : IIdentityService
{
    public async Task<string> GenerateJwtTokenAsync(ApplicationUser user)
    {
        // Token generation logic
        // Claims setup
        // Expiration management
        // Security considerations
    }
}
```

**Authentication Features**:
- JWT token generation and validation
- Password hashing and verification
- Role and claims management
- Token refresh capabilities

## Configuration Management

### Connection Strings
Database connection configuration:
- Development: Local SQL Server
- Testing: In-memory or container databases
- Production: Azure SQL Database

### External Service Configuration
Configuration for external dependencies:
- Email server settings
- JWT signing keys
- Azure services configuration
- Third-party API keys

### Security Configuration
Secure handling of sensitive configuration:
- Azure Key Vault integration
- Environment variable configuration
- Encrypted connection strings
- Certificate management

## Performance Considerations

### Query Optimization
Database query performance:
- Index optimization
- Query compilation
- Connection pooling
- Result caching where appropriate

### Bulk Operations
Efficient handling of large data sets:
- Bulk insert operations
- Batch updates
- Streaming data processing
- Memory-efficient queries

### Monitoring and Diagnostics
Performance monitoring capabilities:
- EF Core logging and diagnostics
- SQL query profiling
- Connection pool monitoring
- Performance counter integration

## Testing Infrastructure

### In-Memory Testing
Support for unit and integration testing:
- In-memory database provider
- Test data seeding
- Isolated test environments
- Fast test execution

### Container Testing
Real database testing with Testcontainers:
- SQL Server container setup
- Migration execution
- Realistic test scenarios
- Data cleanup strategies

## Security Considerations

### Data Protection
Protecting sensitive information:
- Password hashing with secure algorithms
- PII data encryption
- Connection string security
- Audit trail maintenance

### SQL Injection Prevention
Secure data access practices:
- Parameterized queries
- Input validation
- Stored procedure usage where appropriate
- Query plan caching

### Access Control
Database-level security:
- Principle of least privilege
- Connection string security
- Database user permissions
- Network security considerations

For detailed information about specific infrastructure components, explore the Data, Identity, and Services documentation sections.