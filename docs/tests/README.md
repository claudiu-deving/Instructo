# Testing Strategy

The Instructo application follows a comprehensive testing strategy that includes multiple types of tests to ensure code quality, functionality, and reliability. The testing approach emphasizes integration testing with real database interactions while also supporting fast unit testing.

## Testing Philosophy

### Test Pyramid Approach
The testing strategy follows a modified test pyramid:

1. **Unit Tests** - Fast, isolated tests for business logic
2. **Integration Tests** - End-to-end testing with real dependencies
3. **System Tests** - Full application testing scenarios

### Quality Assurance Goals
- **Reliability**: Ensure application functionality works as expected
- **Regression Prevention**: Catch breaking changes early
- **Documentation**: Tests serve as living documentation
- **Confidence**: Enable safe refactoring and feature development

## Test Projects

### [[Instructo.IntegrationTests/README|Instructo.IntegrationTests]]
Primary integration testing project providing end-to-end testing with real database interactions.

**Key Features:**
- Real SQL Server container testing via Testcontainers
- Full application API testing
- Authentication and authorization testing
- Database migration and seeding verification

**Test Categories:**
- API endpoint testing
- Command handler integration tests
- Repository integration tests
- Authentication flow testing

---

### [[Instructo.Tests.Common/README|Instructo.Tests.Common]]
Shared testing infrastructure and utilities used across all test projects.

**Provides:**
- Test fixtures and base classes
- Test data builders and factories
- Authentication helpers
- Common assertions and utilities

**Key Components:**
- `IntegrationTestFixture` - Container-based test setup
- `InMemoryTestFixture` - Fast in-memory testing
- `UserBuilder` - Test user creation patterns
- `AuthenticationHelper` - JWT token generation for tests

---

### Instructo.UnitTests
Unit testing project for isolated component testing (currently minimal implementation).

**Intended for:**
- Domain entity unit tests
- Value object validation tests
- Business logic unit tests
- Handler unit tests with mocked dependencies

## Testing Technologies

### Core Testing Stack
- **xUnit** - Primary test framework with excellent .NET integration
- **FluentAssertions** - Readable and expressive assertions
- **Moq** - Mocking framework for dependency isolation
- **AutoFixture** - Automated test object creation
- **Bogus** - Realistic test data generation

### Specialized Testing Tools
- **Testcontainers** - Real database containers for integration testing
- **Microsoft.AspNetCore.Mvc.Testing** - API testing with TestServer
- **WebApplicationFactory** - Full application testing infrastructure

### Package Dependencies
```xml
<PackageReference Include="Microsoft.NET.Test.Sdk" />
<PackageReference Include="xunit" />
<PackageReference Include="xunit.runner.visualstudio" />
<PackageReference Include="FluentAssertions" />
<PackageReference Include="Moq" />
<PackageReference Include="AutoFixture.AutoMoq" />
<PackageReference Include="Bogus" />
<PackageReference Include="Testcontainers.MsSql" />
<PackageReference Include="Microsoft.AspNetCore.Mvc.Testing" />
```

## Integration Testing Approach

### Container-Based Testing
Using Testcontainers for realistic database testing:

**SQL Server Container Setup**:
```csharp
private static readonly MsSqlContainer Container = new MsSqlBuilder()
    .WithImage("mcr.microsoft.com/mssql/server:2022-latest")
    .WithPassword("YourStrong@Passw0rd")
    .WithCleanUp(true)
    .Build();
```

**Benefits:**
- Real database engine behavior
- Migration testing
- Spatial data support testing
- Realistic performance characteristics

### API Testing Strategy
Full application testing with WebApplicationFactory:

**Test Setup**:
```csharp
public class IntegrationTestBase : IClassFixture<IntegrationTestFixture>
{
    protected readonly HttpClient Client;
    protected readonly IntegrationTestFixture Fixture;
    
    public IntegrationTestBase(IntegrationTestFixture fixture)
    {
        Fixture = fixture;
        Client = fixture.CreateClient();
    }
}
```

**Testing Capabilities:**
- HTTP endpoint testing
- Authentication flow testing
- Authorization policy testing
- Request/response validation
- Error handling verification

## Authentication Testing

### JWT Token Generation
Comprehensive authentication testing support:

```csharp
public class AuthenticationHelper
{
    public static string GenerateJwtToken(ApplicationUser user, IEnumerable<string> roles)
    {
        // JWT token generation for testing
        // Role-based claims setup
        // Test-specific token configuration
    }
}
```

### Role-Based Testing
Testing different user roles and permissions:
- **IronMan**: Super administrator testing
- **Admin**: Administrative function testing
- **Owner**: School owner specific testing
- **Student**: End user functionality testing

### Test User Management
Pre-configured test users for different scenarios:
- Standard users with various roles
- Users with specific permissions
- Edge case user configurations
- Invalid user scenarios

## Test Data Generation

### Builder Pattern Implementation
Fluent builders for complex test object creation:

```csharp
public class UserBuilder
{
    public UserBuilder WithEmail(string email) => this;
    public UserBuilder WithRole(string role) => this;
    public UserBuilder InSchool(SchoolId schoolId) => this;
    
    public ApplicationUser Build() => /* construct user */;
    public async Task<ApplicationUser> BuildAndSaveAsync() => /* build and persist */;
}
```

### Bogus Integration
Realistic test data with Romanian locale:

```csharp
var faker = new Faker<School>("ro")
    .RuleFor(s => s.Name, f => f.Company.CompanyName())
    .RuleFor(s => s.Email, f => f.Internet.Email())
    .RuleFor(s => s.PhoneNumber, f => f.Phone.PhoneNumber());
```

### Test Factories
Factory pattern for common test scenarios:

```csharp
public static class TestCommandFactory
{
    public static CreateSchoolCommand ValidCreateSchoolCommand() => new(/* valid data */);
    public static CreateSchoolCommand InvalidCreateSchoolCommand() => new(/* invalid data */);
}
```

## Test Organization

### Test Structure
Tests are organized to mirror the application structure:
```
tests/
├── Instructo.IntegrationTests/
│   ├── Application/
│   │   ├── Schools/Commands/
│   │   └── Users/Commands/
│   ├── Infrastructure/
│   └── Api/
├── Instructo.Tests.Common/
│   ├── Fixtures/
│   ├── Builders/
│   └── Helpers/
└── Instructo.UnitTests/
    ├── Domain/
    ├── Application/
    └── Infrastructure/
```

### Test Categories
Tests are categorized by type and scope:

**Integration Tests**:
- `[IntegrationTest]` - Full application tests
- `[DatabaseTest]` - Database-specific tests
- `[ApiTest]` - HTTP endpoint tests

**Unit Tests**:
- `[UnitTest]` - Pure unit tests
- `[DomainTest]` - Domain logic tests
- `[HandlerTest]` - Handler unit tests

## Testing Best Practices

### Test Naming Conventions
Clear, descriptive test names following patterns:
- `Should_ReturnSuccess_When_ValidDataProvided`
- `Should_ThrowException_When_InvalidInput`
- `Should_ReturnNotFound_When_EntityDoesNotExist`

### Test Data Management
Consistent test data handling:
- Use builders for complex objects
- Prefer factories for common scenarios
- Clean up test data between tests
- Use realistic but anonymized data

### Assertion Patterns
Consistent assertion style with FluentAssertions:
```csharp
result.Should().NotBeNull();
result.IsSuccess.Should().BeTrue();
result.Data.Should().BeOfType<SchoolDto>();
result.Data.Name.Should().Be(expectedName);
```

### Test Isolation
Ensuring tests don't affect each other:
- Database cleanup between tests
- Independent test data creation
- Isolated HTTP clients
- Proper resource disposal

## Performance Testing

### Test Execution Speed
Optimization for fast feedback loops:
- Parallel test execution where safe
- Efficient database setup/teardown
- Connection pooling for database tests
- In-memory alternatives for unit tests

### Load Testing Considerations
Future load testing capabilities:
- Performance benchmarking
- Concurrent user simulation
- Database performance under load
- API endpoint performance testing

## Continuous Integration

### CI/CD Integration
Tests designed for automated execution:
- Reliable container startup
- Deterministic test execution
- Proper error reporting
- Test result artifacts

### Coverage Reporting
Code coverage analysis:
- Line coverage measurement
- Branch coverage analysis
- Coverage trend tracking
- Coverage quality gates

## Future Testing Enhancements

### Planned Improvements
- **Unit Test Expansion**: More comprehensive unit test coverage
- **Performance Testing**: Automated performance regression testing
- **End-to-End Testing**: Browser-based UI testing
- **Chaos Testing**: Resilience and fault tolerance testing

### Testing Tools Evaluation
Potential additional testing tools:
- **NBomber** for load testing
- **Playwright** for end-to-end testing
- **Stryker.NET** for mutation testing
- **BenchmarkDotNet** for performance benchmarking

The testing strategy provides a solid foundation for ensuring code quality and enabling confident development and deployment of the Instructo application.