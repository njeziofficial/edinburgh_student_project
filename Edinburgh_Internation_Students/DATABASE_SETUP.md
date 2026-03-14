# Database Setup Guide

## PostgreSQL Database Configuration

This project uses PostgreSQL running in Docker with Entity Framework Core.

### Files Created:
- **Models/User.cs** - User entity with validation attributes
- **Data/ApplicationDbContext.cs** - EF Core DbContext for PostgreSQL
- **docker-compose.yml** - Docker configuration for PostgreSQL
- **appsettings.json** - Updated with connection string

### Getting Started:

1. **Start PostgreSQL in Docker:**
   ```bash
   docker-compose up -d
   ```

2. **Create the initial migration:**
   ```bash
   dotnet ef migrations add InitialCreate
   ```

3. **Apply migrations to create the database schema:**
   ```bash
   dotnet ef database update
   ```

4. **Run the application:**
   ```bash
   dotnet run
   ```

### Connection String Details:
- **Host:** localhost
- **Port:** 5432
- **Database:** edinburgh_students_db
- **Username:** postgres
- **Password:** postgres

### User Entity Properties:
- `Id` (Primary Key)
- `FirstName` (Required, Max 100 chars)
- `LastName` (Required, Max 100 chars)
- `Email` (Required, Unique, Email format, Max 255 chars)
- `Password` (Required, Hashed using BCrypt, Max 60 chars)
- `PhoneCode` (Optional, Max 5 chars, digits only - e.g., "234", "1")
- `PhoneNumber` (Optional, Max 15 chars, digits only)
- `CreatedAt` (Auto-set to current UTC time)
- `UpdatedAt` (Nullable)

### API Endpoints:

#### Sign Up
**POST** `/api/auth/signup`

Request Body:
```json
{
  "fullName": "John Doe",
  "email": "john.doe@example.com",
  "password": "SecurePassword123!",
  "phoneCode": "234",
  "phoneNumber": "8012345678"
}
```

Response (201 Created):
```json
{
  "userId": 1,
  "email": "john.doe@example.com",
  "firstName": "John",
  "lastName": "Doe",
  "phoneCode": "234",
  "phoneNumber": "8012345678",
  "message": "User registered successfully"
}
```

#### Sign In
**POST** `/api/auth/signin`

Request Body:
```json
{
  "email": "john.doe@example.com",
  "password": "SecurePassword123!"
}
```

Response (200 OK):
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

> **Note**: The `token` field contains a JWT (JSON Web Token) that should be included in the Authorization header for protected endpoints. See `JWT_AUTHENTICATION.md` for details.

### Useful Commands:

Stop PostgreSQL:
```bash
docker-compose down
```

Stop and remove data:
```bash
docker-compose down -v
```

View logs:
```bash
docker-compose logs -f postgres
```

5. **Split PhoneNumber into PhoneCode and PhoneNumber:**
   ```bash
   dotnet ef migrations add SplitPhoneNumber
   dotnet ef database update
   ```
