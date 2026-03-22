# Admin User Implementation Summary

## Overview
This document summarizes the implementation of admin user functionality with role-based access control for delete operations.

## Changes Made

### 1. Added Role Property to User Model
- **File**: `Models\User.cs`
- Added `Role` property with default value "User"
- Max length: 20 characters

### 2. Created AdminSettings Configuration
- **File**: `Configuration\AdminSettings.cs`
- Configuration class to hold admin credentials from appsettings.json

### 3. Updated appsettings.json
- **File**: `appsettings.json`
- Added `AdminSettings` section with:
  - Email: `admin@live.napier.ac.uk`
  - Password: `admin123`

### 4. Updated Database Configuration
- **File**: `Data\ApplicationDbContext.cs`
- Added Role property configuration with default value "User"

### 5. Updated JWT Service
- **File**: `Services\JwtService.cs`
- Modified `GenerateToken` method to accept and include `role` parameter
- Added `ClaimTypes.Role` claim to JWT token

### 6. Updated Auth Service
- **File**: `Services\AuthService.cs`
- Updated all `GenerateToken` calls to include user role:
  - `SignUpAsync`
  - `SignInAsync`
  - `RefreshTokenAsync`

### 7. Configured Admin Authorization Policy
- **File**: `Program.cs`
- Registered AdminSettings configuration
- Added "AdminOnly" authorization policy that requires "Admin" role
- Implemented admin user seeding on application startup

### 8. Admin User Seeding
- **File**: `Program.cs`
- Added `SeedAdminUser` method that:
  - Checks if admin user already exists
  - Creates admin user with credentials from appsettings.json
  - Sets Role to "Admin"
  - Hashes password using BCrypt
  - Logs seeding status

### 9. Protected Delete Endpoints with Admin Authorization
All delete operations now require Admin role:

#### Controllers Updated:
1. **UsersController.cs** - Delete user endpoint
   - Removed "own account only" restriction
   - Added `[Authorize(Policy = "AdminOnly")]`

2. **EventsController.cs** - Delete event endpoint
   - Added `[Authorize(Policy = "AdminOnly")]`

3. **MessagesController.cs** - Delete message endpoint
   - Added `[Authorize(Policy = "AdminOnly")]`

4. **AnnouncementsController.cs** - Delete announcement endpoint
   - Added `[Authorize(Policy = "AdminOnly")]`

5. **NotificationsController.cs** - Delete notification endpoint
   - Added `[Authorize(Policy = "AdminOnly")]`

6. **AdminController.cs** - All admin operations
   - Changed from `[Authorize]` to `[Authorize(Policy = "AdminOnly")]`

### 10. Database Migration
- **Migration**: `AddRoleToUser`
- Created migration to add Role column to Users table with default value "User"

## Admin User Details

### Credentials (from appsettings.json)
- **Email**: admin@live.napier.ac.uk
- **Password**: admin123
- **Role**: Admin
- **FirstName**: System
- **LastName**: Administrator

### Seeding Behavior
- Admin user is automatically created on application startup
- If admin user already exists, it skips creation
- Password is hashed using BCrypt with salt rounds = 12

## Authorization Flow

### How It Works:
1. Admin logs in with `admin@platform.com` / `admin123`
2. JWT token is generated with `ClaimTypes.Role = "Admin"`
3. Delete endpoints check for "AdminOnly" policy
4. Policy validates user has "Admin" role claim
5. Only admin users can access delete operations

### Regular Users:
- Can no longer delete anything (users, events, messages, announcements, notifications)
- Will receive 403 Forbidden when attempting to access delete endpoints

### Admin Users:
- Can delete any resource across the application
- Can trigger group rotations
- Can access all admin endpoints

## Security Considerations

### ⚠️ Important Notes:
1. **Change Admin Password**: The default password `admin123` should be changed in production
2. **Secure appsettings.json**: Use environment variables or Azure Key Vault for sensitive data in production
3. **HTTPS Only**: Ensure admin operations are only accessible via HTTPS
4. **Audit Logging**: Consider adding audit logs for admin delete operations

## Testing the Implementation

### 1. Login as Admin
```bash
POST /api/auth/signin
{
  "email": "admin@live.napier.ac.uk",
  "password": "admin123"
}
```

### 2. Use Token for Delete Operations
Include the JWT token in Authorization header:
```
Authorization: Bearer <jwt_token>
```

### 3. Test Delete Endpoints
- DELETE /api/users/{id}
- DELETE /api/events/{id}
- DELETE /api/groups/{groupId}/messages/{messageId}
- DELETE /api/announcements/{id}
- DELETE /api/notifications/{id}

### 4. Verify Non-Admin Cannot Delete
- Login as regular user
- Attempt delete operation
- Should receive 403 Forbidden response

## Next Steps

### Recommended Enhancements:
1. Add audit logging for admin operations
2. Implement password reset for admin user
3. Add ability to create multiple admin users
4. Implement role management endpoints
5. Add admin dashboard with statistics
6. Use environment variables for admin credentials
7. Implement two-factor authentication for admin users

## Migration Commands

### Apply Migration:
```bash
dotnet ef database update
```

### Rollback Migration (if needed):
```bash
dotnet ef migrations remove
```

## Build Status
✅ Build Successful - All changes compile without errors
