# Instructo - Documentation

Instructo is an Enterprise Driving School Management API built with .NET 9 using Clean Architecture principles. This documentation serves as a comprehensive guide to understanding the project structure, architecture, and implementation details.

## Quick Navigation

### Core Architecture
- [[Project Overview]]
- [[Architecture Guide]]
- [[Technology Stack]]

### Source Code Documentation
- [[src/README|Source Projects]]
  - [[src/Instructo.Api/README|API Layer]]
  - [[src/Instructo.Application/README|Application Layer]]
  - [[src/Instructo.Domain/README|Domain Layer]]
  - [[src/Instructo.Infrastructure/README|Infrastructure Layer]]
  - [[src/Instructo.Shared/README|Shared Layer]]

### Testing Documentation
- [[tests/README|Testing Overview]]
  - [[tests/Instructo.IntegrationTests/README|Integration Tests]]
  - [[tests/Instructo.Tests.Common/README|Test Infrastructure]]

## What is Instructo?

Instructo is a comprehensive business management system specifically designed for driving schools. The application manages:

- **Driving Schools**: Registration, approval workflows, and detailed profiles
- **School Owners**: User management with role-based authorization
- **Certificates**: ARR (Romanian driving authority) certification tracking
- **Vehicle Categories**: Different types of driving instruction
- **Geographical Data**: City and county information for school locations
- **Business Operations**: Hours, contact information, and website links

## Key Features

- Clean Architecture with Domain-Driven Design
- CQRS pattern implementation using MediatR
- JWT-based authentication with role-based authorization
- Entity Framework Core with SQL Server
- Comprehensive observability with OpenTelemetry
- Container-based integration testing
- .NET Aspire support for cloud-native development

## Getting Started

1. Review the [[Project Overview]] for high-level understanding
2. Explore the [[Architecture Guide]] for design patterns and principles
3. Check [[src/Instructo.Api/README|API Documentation]] for endpoint details
4. Review [[tests/README|Testing Strategy]] for development practices

## Documentation Structure

This documentation mirrors the project structure to provide easy navigation and comprehensive coverage of all components. Each section contains:

- Purpose and responsibilities
- Key components and their roles
- Dependencies and relationships
- Implementation patterns and best practices