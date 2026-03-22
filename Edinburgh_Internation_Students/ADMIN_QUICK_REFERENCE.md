# Admin User Quick Reference Guide

## Admin Login Credentials
```
Email: admin@live.napier.ac.uk
Password: admin123
```

## How to Login as Admin

### Using cURL:
```bash
curl -X POST http://localhost:5000/api/auth/signin \
  -H "Content-Type: application/json" \
  -d '{
    "email": "admin@live.napier.ac.uk",
    "password": "admin123"
  }'
```

### Response:
```json
{
  "success": true,
  "data": {
    "userId": 1,
    "email": "admin@live.napier.ac.uk",
    "firstName": "System",
    "lastName": "Administrator",
    "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
    "message": "Sign in successful"
  }
}
```

## Admin-Only Endpoints

All delete operations require admin authentication:

### 1. Delete User
```bash
DELETE /api/users/{id}
Authorization: Bearer {admin_token}
```

### 2. Delete Event
```bash
DELETE /api/events/{id}
Authorization: Bearer {admin_token}
```

### 3. Delete Message
```bash
DELETE /api/groups/{groupId}/messages/{messageId}
Authorization: Bearer {admin_token}
```

### 4. Delete Announcement
```bash
DELETE /api/announcements/{id}
Authorization: Bearer {admin_token}
```

### 5. Delete Notification
```bash
DELETE /api/notifications/{id}
Authorization: Bearer {admin_token}
```

### 6. Admin Operations
```bash
POST /api/admin/trigger-group-rotation
GET /api/admin/group-stats
Authorization: Bearer {admin_token}
```

## Error Responses

### Non-Admin Attempting Delete (403 Forbidden):
```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.3",
  "title": "Forbidden",
  "status": 403
}
```

### Unauthorized Request (401):
```json
{
  "type": "https://tools.ietf.org/html/rfc7235#section-3.1",
  "title": "Unauthorized",
  "status": 401
}
```

## JWT Token Claims

Admin tokens include the following role claim:
```json
{
  "role": "Admin",
  "sub": "1",
  "email": "admin@live.napier.ac.uk",
  "given_name": "System",
  "family_name": "Administrator"
}
```

## Testing Steps

### 1. Start the Application
```bash
dotnet run
```

### 2. Admin User is Auto-Created
Check logs for:
```
Admin user created successfully with email: admin@live.napier.ac.uk
```
OR
```
Admin user already exists with email: admin@live.napier.ac.uk
```

### 3. Login as Admin
Use the credentials above

### 4. Test Delete Permission
- As admin: Delete operations should succeed
- As regular user: Delete operations should return 403 Forbidden

## Swagger/OpenAPI

Access Swagger UI at: `http://localhost:5000/swagger`

1. Click "Authorize" button
2. Login as admin
3. Copy the JWT token from response
4. Paste token in format: `{token}` (without "Bearer" prefix)
5. Test delete endpoints

## Database Verification

Check admin user in database:
```sql
SELECT * FROM "Users" WHERE "Email" = 'admin@live.napier.ac.uk';
```

Expected result:
```
Id | FirstName | LastName      | Email               | Role  | CreatedAt
1  | System    | Administrator | admin@live.napier.ac.uk| Admin | 2024-...
```

## Troubleshooting

### Admin User Not Created
Check application logs for errors during startup

### 403 Forbidden Even as Admin
- Verify token includes role claim
- Check token hasn't expired
- Ensure using correct Authorization header format

### Can't Login
- Verify database is running
- Check admin credentials in appsettings.json
- Ensure database migrations are applied

## Production Recommendations

1. **Change Password**: Update admin password in appsettings.json
2. **Use Environment Variables**:
   ```bash
   export AdminSettings__Email="admin@yourcompany.com"
   export AdminSettings__Password="SecurePassword123!"
   ```
3. **Enable HTTPS**: Require HTTPS for all admin operations
4. **Add Rate Limiting**: Protect admin endpoints from brute force
5. **Audit Logging**: Log all admin delete operations
6. **Two-Factor Auth**: Implement 2FA for admin accounts
