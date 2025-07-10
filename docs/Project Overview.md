# Project Overview

Instructo is an Enterprise Driving School Management API that provides a comprehensive platform for managing driving schools in Romania. Built with modern .NET technologies and following Clean Architecture principles, it serves as a robust business management solution.

## Business Domain

### Core Purpose
Instructo manages the complete lifecycle of driving schools, from registration and approval to daily operations and regulatory compliance.

### Key Business Entities
- **[[src/Instructo.Domain/Entities/README#School|Schools]]**: The central business entity representing driving schools
- **[[src/Instructo.Domain/Entities/README#ApplicationUser|Users]]**: School owners and system administrators
- **[[src/Instructo.Domain/Entities/README#Address|Geographical Data]]**: Cities, counties, and addresses for school locations
- **[[src/Instructo.Domain/ValueObjects/README|Certificates]]**: ARR certification tracking for regulatory compliance

### Business Workflows
1. **School Registration**: New schools register and await approval
2. **Approval Process**: Administrators review and approve/reject applications
3. **Operations Management**: Approved schools manage their business information
4. **Regulatory Compliance**: Track certifications and compliance requirements

## Technical Architecture

### [[Architecture Guide|Clean Architecture Implementation]]
- **[[src/Instructo.Domain/README|Domain Layer]]**: Core business logic and entities
- **[[src/Instructo.Application/README|Application Layer]]**: Use cases implemented with CQRS
- **[[src/Instructo.Infrastructure/README|Infrastructure Layer]]**: Data access and external services
- **[[src/Instructo.Api/README|Presentation Layer]]**: REST API endpoints

### Key Patterns
- **CQRS**: Command/Query separation using [[src/MediatR/README|MediatR]]
- **Repository Pattern**: Data access abstraction
- **Result Pattern**: Consistent error handling
- **Domain-Driven Design**: Rich domain models with business logic

## Technology Stack

### Core Framework
- **.NET 9** with C# implicit usings and nullable reference types
- **ASP.NET Core** for web API functionality
- **Entity Framework Core 9.0** with SQL Server

### Authentication & Security
- **ASP.NET Core Identity** for user management
- **JWT Bearer** authentication
- **Role-based authorization** with custom policies

### Observability & Monitoring
- **[[src/Instructo.Api/README#Logging|Serilog]]** for structured logging
- **OpenTelemetry** for distributed tracing
- **Correlation ID** middleware for request tracking

### Testing
- **[[tests/README|Comprehensive testing strategy]]** with integration and unit tests
- **Testcontainers** for realistic database testing
- **Bogus** for test data generation

## Development Environment

### Prerequisites
- .NET 9 SDK
- SQL Server (or Docker for development)
- Visual Studio Code or Visual Studio

### Getting Started
```bash
# Clone and build
git clone <repository-url>
cd Instructo
dotnet build

# Run with watch
dotnet watch --project src/Instructo.Api/Api.csproj

# Run tests
dotnet test
```

### Database Setup
```bash
# Update database with migrations
dotnet ef database update --project src/Instructo.Infrastructure/Infrastructure.csproj --startup-project src/Instructo.Api/Api.csproj
```

## Project Structure

```
Instructo/
├── src/                          # Source code projects
│   ├── Instructo.Api/           # [[src/Instructo.Api/README|Web API layer]]
│   ├── Instructo.Application/   # [[src/Instructo.Application/README|Application layer]]
│   ├── Instructo.Domain/        # [[src/Instructo.Domain/README|Domain layer]]
│   ├── Instructo.Infrastructure/# [[src/Instructo.Infrastructure/README|Infrastructure layer]]
│   └── Instructo.Shared/        # [[src/Instructo.Shared/README|Shared utilities]]
├── tests/                       # [[tests/README|Test projects]]
│   ├── Instructo.IntegrationTests/
│   └── Instructo.Tests.Common/
└── docs/                        # This documentation
```

## Current Development Status

### Recently Implemented
- School approval workflow with optimistic concurrency
- Enhanced geographical data support (Romanian counties/cities)
- Slug-based URL routing for schools
- Comprehensive integration testing infrastructure
- Unit of Work pattern implementation

### In Development
- Enhanced user management features
- Additional business validation rules
- Performance optimizations
- Extended API documentation

## Key Stakeholders

### User Roles
- **IronMan**: Super administrator with full system access
- **Admin**: System administrators managing approvals
- **Owner**: School owners managing their institutions
- **Student**: End users of the driving school system

### Technical Roles
- Platform administrators managing infrastructure
- Developers extending functionality
- Quality assurance ensuring system reliability

For detailed information about specific components, navigate to the relevant documentation sections using the links above.