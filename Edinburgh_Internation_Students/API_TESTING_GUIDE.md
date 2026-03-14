# API Testing Guide

## Authentication Endpoints

### Base URL
`https://localhost:{port}/api/auth`

## Sign Up

### Endpoint
`POST /api/auth/signup`

### Request Headers
```
Content-Type: application/json
```

### Request Body
```json
{
  "fullName": "John Doe",
  "email": "john.doe@example.com",
  "password": "SecurePassword123!",
  "phoneNumber": "+1234567890"
}
```

### Validation Rules
- **fullName**: Required, 2-200 characters
- **email**: Required, valid email format, max 255 characters
- **password**: Required, minimum 8 characters
- **phoneNumber**: Optional, valid phone format

### Success Response (201 Created)
```json
{
  "userId": 1,
  "email": "john.doe@example.com",
  "firstName": "John",
  "lastName": "Doe",
  "phoneNumber": "+1234567890",
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "message": "User registered successfully"
}
```

> **Note**: The `token` is a JWT (JSON Web Token) that should be stored and included in subsequent authenticated requests.

### Error Response (400 Bad Request)
```json
{
  "message": "A user with this email already exists"
}
```

### cURL Example
```bash
curl -X POST https://localhost:5001/api/auth/signup \
  -H "Content-Type: application/json" \
  -d '{
    "fullName": "John Doe",
    "email": "john.doe@example.com",
    "password": "SecurePassword123!",
    "phoneNumber": "+1234567890"
  }'
```

---

## Sign In

### Endpoint
`POST /api/auth/signin`

### Request Headers
```
Content-Type: application/json
```

### Request Body
```json
{
  "email": "john.doe@example.com",
  "password": "SecurePassword123!"
}
```

### Validation Rules
- **email**: Required, valid email format
- **password**: Required

### Success Response (200 OK)
```json
{
  "userId": 1,
  "email": "john.doe@example.com",
  "firstName": "John",
  "lastName": "Doe",
  "phoneCode": "234",
  "phoneNumber": "8012345678",
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "message": "Sign in successful"
}
```

> **Note**: The `token` is a JWT that contains the user's ID and other claims. Store this token and use it for authenticated API calls.

### Error Response (401 Unauthorized)
```json
{
  "message": "Invalid email or password"
}
```

### cURL Example
```bash
curl -X POST https://localhost:5001/api/auth/signin \
  -H "Content-Type: application/json" \
  -d '{
    "email": "john.doe@example.com",
    "password": "SecurePassword123!"
  }'
```

---

## Making Authenticated Requests

After signing in or signing up, you'll receive a JWT token. Include this token in the `Authorization` header for protected endpoints.

### Header Format
```
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

### Example: Protected Request
```bash
curl -X GET https://localhost:5001/api/profile/user/1 \
  -H "Authorization: Bearer YOUR_JWT_TOKEN_HERE"
```

### JWT Token Claims

The JWT token contains the following claims:
- **sub**: User ID (subject)
- **email**: User's email address
- **given_name**: User's first name
- **family_name**: User's last name
- **jti**: Unique token identifier
- **iat**: Issued at timestamp
- **exp**: Expiration timestamp (24 hours from issue)

### Token Expiry

Tokens expire after **24 hours** (1440 minutes). After expiration, users must sign in again to get a new token.

---

## Testing with Swagger

1. Run the application: `dotnet run`
2. Navigate to: `https://localhost:{port}/swagger`
3. Expand the Auth endpoints
4. Click "Try it out" on any endpoint
5. Fill in the request body
6. Click "Execute"

---

## Common Error Responses

### Validation Error (400 Bad Request)
```json
{
  "message": "Validation failed",
  "errors": {
    "FullName": ["Full name is required"],
    "Email": ["Invalid email address"],
    "Password": ["Password must be at least 8 characters long"]
  }
}
```

### Server Error (500 Internal Server Error)
```json
{
  "message": "An error occurred while processing your request"
}
```

---

## Testing Workflow

1. **Sign Up a new user**
   - Use the signup endpoint with valid data
   - Should receive a 201 Created response

2. **Try signing up with the same email**
   - Should receive a 400 Bad Request with "user already exists" message

3. **Sign In with correct credentials**
   - Should receive a 200 OK response with user details

4. **Sign In with wrong password**
   - Should receive a 401 Unauthorized response

5. **Sign In with non-existent email**
   - Should receive a 401 Unauthorized response
