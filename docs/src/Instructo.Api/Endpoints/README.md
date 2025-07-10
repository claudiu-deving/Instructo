# API Endpoints

The Instructo API provides REST endpoints for managing driving schools, users, and authentication. All endpoints follow RESTful conventions and use JSON for request/response payloads.

## Endpoint Groups

### [[SchoolEndpoints|School Management]]
Endpoints for managing driving schools, including CRUD operations and approval workflows.

**Base Route**: `/api/schools`

**Key Endpoints:**
- `GET /api/schools` - List schools with pagination
- `GET /api/schools/{slug}` - Get school by slug
- `POST /api/schools` - Create new school
- `PUT /api/schools/{id}` - Update school information
- `DELETE /api/schools/{id}` - Delete school
- `PATCH /api/schools/{id}/approval` - Update approval status

---

### [[UserEndpoints|User Management]]
Administrative endpoints for user management, restricted to IronMan role.

**Base Route**: `/api/users`

**Key Endpoints:**
- `GET /api/users` - List all users (IronMan only)
- `GET /api/users/{id}` - Get user by ID (IronMan only)
- `PUT /api/users/{id}` - Update user information (IronMan only)
- `DELETE /api/users/{id}` - Delete user (IronMan only)

---

### [[AuthEndpoints|Authentication]]
Public endpoints for user authentication and registration.

**Base Route**: `/api/auth`

**Key Endpoints:**
- `POST /api/auth/register` - Register new user
- `POST /api/auth/login` - User authentication
- `POST /api/auth/change-password` - Change user password
- `POST /api/auth/forgot-password` - Password reset request

## Common Patterns

### Response Format
All endpoints return responses in a consistent format:

```json
{
  "isSuccess": true,
  "data": { /* response data */ },
  "errors": []
}
```

For error responses:
```json
{
  "isSuccess": false,
  "data": null,
  "errors": [
    {
      "code": "VALIDATION_ERROR",
      "message": "Validation failed",
      "details": { /* validation details */ }
    }
  ]
}
```

### Pagination
List endpoints support pagination with query parameters:
- `page` - Page number (default: 1)
- `pageSize` - Items per page (default: 10, max: 100)
- `sortBy` - Sort field
- `sortOrder` - Sort direction (asc/desc)

Example: `GET /api/schools?page=2&pageSize=20&sortBy=name&sortOrder=asc`

### Authentication
Protected endpoints require JWT Bearer token in the Authorization header:
```
Authorization: Bearer <jwt-token>
```

### Authorization Policies

**IronMan**: Super administrator access to all operations
**AdminOnly**: Administrative access (Admin, Owner, IronMan roles)
**SchoolOwners**: School owner specific access

## Error Handling

### HTTP Status Codes
- `200 OK` - Successful operation
- `201 Created` - Resource created successfully
- `400 Bad Request` - Invalid request or validation errors
- `401 Unauthorized` - Authentication required
- `403 Forbidden` - Insufficient permissions
- `404 Not Found` - Resource not found
- `409 Conflict` - Concurrency conflict or business rule violation
- `500 Internal Server Error` - Server error

### Validation Errors
Validation errors include detailed information about field-level issues:

```json
{
  "isSuccess": false,
  "errors": [
    {
      "code": "VALIDATION_ERROR",
      "message": "Validation failed",
      "details": {
        "Name": ["Name is required"],
        "Email": ["Email format is invalid"]
      }
    }
  ]
}
```

### Business Rule Violations
Business logic errors provide context about rule violations:

```json
{
  "isSuccess": false,
  "errors": [
    {
      "code": "BUSINESS_RULE_VIOLATION",
      "message": "School approval failed",
      "details": "School does not meet minimum requirements"
    }
  ]
}
```

## Rate Limiting

### Default Limits
- General endpoints: 100 requests per minute per IP
- Authentication endpoints: 5 requests per minute per IP
- Administrative endpoints: 50 requests per minute per IP

### Rate Limit Headers
Responses include rate limiting information:
```
X-RateLimit-Limit: 100
X-RateLimit-Remaining: 95
X-RateLimit-Reset: 1640995200
```

## CORS Support

### Allowed Origins
- Development: `http://localhost:3000`, `http://localhost:4200`
- Production: Configured based on deployment environment

### Allowed Methods
- `GET`, `POST`, `PUT`, `DELETE`, `PATCH`, `OPTIONS`

### Allowed Headers
- `Content-Type`, `Authorization`, `X-Correlation-ID`

## Content Types

### Request Content Types
- `application/json` - Primary format for all endpoints
- `application/x-www-form-urlencoded` - Alternative for simple forms

### Response Content Types
- `application/json` - All API responses
- `application/problem+json` - Error responses (RFC 7807)

## Versioning

### Current Version
- API Version: v1
- All endpoints are prefixed with `/api/`

### Future Versioning Strategy
- Header-based versioning: `Api-Version: 1.0`
- Backward compatibility maintained for at least one major version

## Documentation

### OpenAPI/Swagger
- Interactive documentation available at `/swagger`
- Complete API specification with examples
- Try-it-out functionality for testing

### Scalar Documentation
- Enhanced documentation UI at `/scalar`
- Improved developer experience
- Better visualization of API structure

For detailed information about specific endpoint groups, follow the links to individual endpoint documentation.