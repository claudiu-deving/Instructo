# Instructo.Api - Presentation Layer

The API project serves as the presentation layer of the Instructo application, providing HTTP endpoints and handling web-specific concerns. Built using ASP.NET Core Minimal APIs, it offers a clean and efficient way to expose the application's functionality.

## Purpose

Instructo.Api is responsible for:
- HTTP endpoint definitions and routing
- Authentication and authorization setup
- Request/response handling and validation
- Middleware pipeline configuration
- API documentation and OpenAPI specification

## Key Components

### [[Endpoints/README|Endpoints]]
REST API endpoint definitions using ASP.NET Core Minimal APIs for clean, functional endpoint registration.

**Main Endpoint Groups:**
- [[Endpoints/SchoolEndpoints|SchoolEndpoints]] - School management operations
- [[Endpoints/UserEndpoints|UserEndpoints]] - User administration (IronMan only)
- [[Endpoints/AuthEndpoints|AuthEndpoints]] - Authentication and user registration

### [[Middleware/README|Middleware]]
Custom middleware components for cross-cutting concerns and request processing.

**Key Middleware:**
- [[Middleware/CorrelationIdMiddleware|CorrelationIdMiddleware]] - Request correlation tracking
- [[Middleware/RoleBasedAuthorizationMiddleware|RoleBasedAuthorizationMiddleware]] - Custom authorization logic
- [[Middleware/IronmanAccessLoggingMiddleware|IronmanAccessLoggingMiddleware]] - Administrative access logging

### Configuration & Startup

**Program.cs** - Main application configuration including:
- Database context setup with retry policies
- Identity configuration with password policies
- JWT authentication and authorization policies
- MediatR registration and validation pipeline
- Logging and observability setup

## Technology Stack

### Core Technologies
- **ASP.NET Core 9.0** - Web framework
- **Minimal APIs** - Endpoint definition approach
- **OpenAPI/Swagger** - API documentation
- **Scalar** - Enhanced API documentation UI

### Authentication & Security
- **ASP.NET Core Identity** - User management
- **JWT Bearer Authentication** - Token-based authentication
- **Custom Authorization Policies** - Role-based access control
- **Rate Limiting** - API protection

### Observability
- **Serilog** - Structured logging with Seq integration
- **OpenTelemetry** - Distributed tracing and metrics
- **Correlation ID** - Request tracking across services

### Dependencies
```xml
<PackageReference Include="Microsoft.AspNetCore.OpenApi" />
<PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" />
<PackageReference Include="Microsoft.AspNetCore.Identity.EntityFrameworkCore" />
<PackageReference Include="Microsoft.AspNetCore.Authentication.JwtBearer" />
<PackageReference Include="Serilog.AspNetCore" />
<PackageReference Include="OpenTelemetry.Extensions.Hosting" />
<PackageReference Include="AspNetCoreRateLimit" />
<PackageReference Include="Scalar.AspNetCore" />
```

## Authentication & Authorization

### JWT Configuration
The application uses JWT Bearer tokens for authentication with the following setup:
- Token validation parameters
- Issuer and audience validation
- Signature key validation
- Token expiration handling

### Authorization Policies

**IronMan Policy**:
- Super administrator access
- Full system privileges
- User management capabilities

**AdminOnly Policy**:
- Administrative access
- Includes Admin, Owner, and IronMan roles
- School approval and management

**SchoolOwners Policy**:
- School owner specific access
- Manage owned school information
- Limited administrative functions

### Role Hierarchy
```
IronMan (Super Admin)
├── Admin (System Administrator)
├── Owner (School Owner)
└── Student (End User)
```

## API Documentation

### OpenAPI/Swagger Integration
The API includes comprehensive OpenAPI documentation with:
- Endpoint descriptions and examples
- Request/response schemas
- Authentication requirements
- Error response formats

### Scalar UI
Enhanced API documentation interface providing:
- Interactive API testing
- Improved documentation browsing
- Better developer experience

## Logging & Monitoring

### Structured Logging
Using Serilog with:
- Console output for development
- Seq integration for centralized logging
- Correlation ID tracking
- Sensitive data destructuring policies

### Request Tracking
- Correlation ID middleware for request tracing
- Administrative access logging
- Performance monitoring
- Error tracking and alerting

## Error Handling

### Global Error Handling
- Consistent error response format
- Exception logging and tracking
- Client-friendly error messages
- Security consideration for error details

### Validation Pipeline
- FluentValidation integration through MediatR
- Automatic validation for all commands
- Detailed validation error responses
- Model binding validation

## Rate Limiting

### API Protection
- Request rate limiting per client
- Configurable limits per endpoint
- IP-based throttling
- Protection against abuse

## CORS Configuration

### Cross-Origin Support
- Configurable CORS policies
- Support for multiple client applications
- Secure default configurations
- Development vs production settings

## Health Checks

### Monitoring Endpoints
- Database connectivity checks
- External service health monitoring
- System resource monitoring
- Readiness and liveness probes

## Development Features

### Hot Reload Support
- Fast development cycle
- Automatic code reloading
- Configuration change detection
- Debugging support

### Development Tools
- Swagger UI for API testing
- Development-specific logging
- Exception page with details
- Database migration support

## Deployment Considerations

### Production Configuration
- Environment-specific settings
- Security headers
- Performance optimizations
- Monitoring and alerting

### Container Support
- Docker containerization
- Kubernetes deployment ready
- Health check endpoints
- Configuration through environment variables

For detailed information about specific components, explore the linked documentation sections.