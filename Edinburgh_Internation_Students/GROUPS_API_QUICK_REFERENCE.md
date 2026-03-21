# Groups API Quick Reference

## Endpoints Summary

| Method | Endpoint | Auth | Description |
|--------|----------|------|-------------|
| GET | `/api/groups` | ✅ | Get all groups |
| GET | `/api/groups/{id}` | ✅ | Get group by ID |
| POST | `/api/groups` | ✅ | Create group |
| PUT | `/api/groups/{id}` | ✅ | Update group |
| DELETE | `/api/groups/{id}` | 🔒 Admin | Delete group |
| GET | `/api/groups/{id}/members` | ✅ | Get group members |
| POST | `/api/groups/{id}/members/{userId}` | ✅ | Add member |
| DELETE | `/api/groups/{id}/members/{userId}` | ✅ | Remove member |
| GET | `/api/groups/my-groups` | ✅ | Get current user's groups |
| GET | `/api/groups/users/{userId}/groups` | ✅ | Get user's groups |

## Request Examples

### Create Group
```json
POST /api/groups
{
  "name": "Study Group",
  "description": "CS students",
  "category": "Academic",
  "isPrivate": false
}
```

### Update Group
```json
PUT /api/groups/{id}
{
  "name": "New Name",
  "description": "New description"
}
```

### Add Member
```
POST /api/groups/{groupId}/members/{userId}
```

### Remove Member
```
DELETE /api/groups/{groupId}/members/{userId}
```

## Response Format

### Success (200/201)
```json
{
  "success": true,
  "data": { /* response data */ },
  "message": "Success message",
  "statusCode": 200
}
```

### Error (400/404/500)
```json
{
  "success": false,
  "data": null,
  "message": "Error message",
  "errors": ["Error details"],
  "statusCode": 400
}
```

## Authorization

### Regular User Actions:
- ✅ View all groups
- ✅ Create groups
- ✅ Update groups
- ✅ Add members to groups
- ✅ Remove themselves from groups
- ❌ Delete groups (Admin only)
- ❌ Remove other users (Admin only)

### Admin Actions:
- ✅ All regular user actions
- ✅ Delete any group
- ✅ Remove any user from any group

## Common Status Codes

| Code | Meaning |
|------|---------|
| 200 | Success |
| 201 | Created |
| 400 | Bad Request / Validation Failed |
| 401 | Unauthorized (No token) |
| 403 | Forbidden (Insufficient permissions) |
| 404 | Not Found |
| 500 | Server Error |

## Business Rules

### Groups
- Max group size: **2 members** (configurable)
- Group names: **2-100 characters**
- Default expiry: **3 months**
- Only admins can delete groups

### Members
- Users auto-join group on signup
- Can't join same group twice
- Can remove themselves anytime
- Admins can remove any user

## Testing Tips

### 1. Get Auth Token
```bash
POST /api/auth/signin
{
  "email": "user@email.com",
  "password": "password"
}
```

### 2. Test Endpoints
```bash
# Add token to all requests
Authorization: Bearer {token}
```

### 3. Admin Testing
```bash
# Login as admin
POST /api/auth/signin
{
  "email": "admin@platform.com",
  "password": "admin123"
}

# Test delete operations
DELETE /api/groups/{id}
```

## cURL Examples

### Get All Groups
```bash
curl -X GET "http://localhost:5000/api/groups" \
  -H "Authorization: Bearer {token}"
```

### Create Group
```bash
curl -X POST "http://localhost:5000/api/groups" \
  -H "Authorization: Bearer {token}" \
  -H "Content-Type: application/json" \
  -d '{
    "name": "Study Group",
    "description": "CS students",
    "category": "Academic",
    "isPrivate": false
  }'
```

### Get My Groups
```bash
curl -X GET "http://localhost:5000/api/groups/my-groups" \
  -H "Authorization: Bearer {token}"
```

### Add Member
```bash
curl -X POST "http://localhost:5000/api/groups/{groupId}/members/{userId}" \
  -H "Authorization: Bearer {token}"
```

### Leave Group (Remove Self)
```bash
curl -X DELETE "http://localhost:5000/api/groups/{groupId}/members/{myUserId}" \
  -H "Authorization: Bearer {token}"
```

## Swagger Testing

1. Navigate to: `http://localhost:5000/swagger`
2. Click **Authorize** button (top right)
3. Enter JWT token
4. Test endpoints interactively

## Common Errors & Solutions

### "Group is full"
- Max group size reached (default: 2)
- Create new group or wait for rotation

### "User is already a member"
- User already in the group
- Check membership before adding

### "You can only remove yourself"
- Regular users can't remove others
- Use admin account or remove self only

### "Group not found"
- Invalid group ID
- Group may have been deleted
- Check group exists first

### "Forbidden" (403)
- Missing admin role for admin-only operations
- Login as admin: `admin@platform.com` / `admin123`

## Related Features

### Automatic Group Rotation
Groups automatically shuffle based on schedule:
```json
"GroupRotationSettings": {
  "IntervalInHours": 24,
  "MaxGroupSize": 2,
  "Enabled": true
}
```

### Manual Rotation (Admin)
```bash
POST /api/admin/trigger-group-rotation
Authorization: Bearer {admin_token}
```

### Group Statistics (Admin)
```bash
GET /api/admin/group-stats
Authorization: Bearer {admin_token}
```

## Database Schema

### Groups Table
- Id (Guid)
- Name (string, 100)
- Description (string, nullable)
- CreatedAt (DateTime)
- ExpiresAt (DateTime)
- IsActive (bool)

### GroupMembers Table
- Id (Guid)
- GroupId (Guid, FK)
- UserId (int, FK)
- JoinedAt (DateTime)

## Performance Tips

- Use pagination for large group lists (future feature)
- Cache frequently accessed groups
- Batch member operations when possible
- Monitor group rotation performance

## Support

For issues or questions:
- Check logs: Application logs contain detailed error info
- Test in Swagger: Interactive testing and schema validation
- Review docs: Full documentation in GROUPS_API_DOCUMENTATION.md
