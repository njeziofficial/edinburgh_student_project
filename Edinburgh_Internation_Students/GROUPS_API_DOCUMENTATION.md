# Groups API Documentation

## Overview
The Groups API provides comprehensive CRUD (Create, Read, Update, Delete) operations for managing student groups in the Edinburgh International Students platform.

## Base URL
```
/api/groups
```

## Authentication
All endpoints require JWT authentication. Include the token in the Authorization header:
```
Authorization: Bearer {your_jwt_token}
```

---

## Endpoints

### 1. Get All Groups
Retrieve all active groups in the platform.

**Endpoint:** `GET /api/groups`

**Authentication:** Required

**Response:** `200 OK`
```json
{
  "success": true,
  "data": [
    {
      "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
      "name": "Group1",
      "description": "Auto-generated group for student connections",
      "imageUrl": null,
      "category": "General",
      "isPrivate": false,
      "memberCount": 2,
      "onlineCount": 1,
      "createdAt": "2024-03-21T10:30:00Z"
    }
  ],
  "message": "Success",
  "statusCode": 200
}
```

---

### 2. Get Group By ID
Retrieve details of a specific group.

**Endpoint:** `GET /api/groups/{id}`

**Authentication:** Required

**Path Parameters:**
- `id` (Guid) - Group ID

**Response:** `200 OK`
```json
{
  "success": true,
  "data": {
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "name": "Group1",
    "description": "Auto-generated group for student connections",
    "imageUrl": null,
    "category": "General",
    "isPrivate": false,
    "memberCount": 2,
    "onlineCount": 1,
    "createdAt": "2024-03-21T10:30:00Z"
  },
  "message": "Success",
  "statusCode": 200
}
```

**Error Response:** `404 Not Found`
```json
{
  "success": false,
  "data": null,
  "message": "Group not found",
  "errors": null,
  "statusCode": 404
}
```

---

### 3. Create Group
Create a new group.

**Endpoint:** `POST /api/groups`

**Authentication:** Required

**Request Body:**
```json
{
  "name": "Study Group",
  "description": "Group for Computer Science students",
  "category": "Academic",
  "isPrivate": false,
  "memberIds": []
}
```

**Field Validations:**
- `name` (required): 2-100 characters
- `description` (optional): Any length
- `category` (required): Any string
- `isPrivate` (optional): Boolean, default false
- `memberIds` (optional): Array of user IDs

**Response:** `201 Created`
```json
{
  "success": true,
  "data": {
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "name": "Study Group",
    "description": "Group for Computer Science students",
    "imageUrl": null,
    "category": "General",
    "isPrivate": false,
    "memberCount": 0,
    "onlineCount": 0,
    "createdAt": "2024-03-21T10:30:00Z"
  },
  "message": "Group created successfully",
  "statusCode": 201
}
```

**Error Response:** `400 Bad Request`
```json
{
  "success": false,
  "data": null,
  "message": "Validation failed",
  "errors": [
    "Group name is required",
    "Name must be between 2 and 100 characters"
  ],
  "statusCode": 400
}
```

---

### 4. Update Group
Update an existing group.

**Endpoint:** `PUT /api/groups/{id}`

**Authentication:** Required

**Path Parameters:**
- `id` (Guid) - Group ID

**Request Body:**
```json
{
  "name": "Updated Study Group",
  "description": "Updated description for CS students",
  "category": "Academic",
  "isPrivate": false
}
```

**Note:** All fields are optional. Only provided fields will be updated.

**Response:** `200 OK`
```json
{
  "success": true,
  "data": {
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "name": "Updated Study Group",
    "description": "Updated description for CS students",
    "imageUrl": null,
    "category": "Academic",
    "isPrivate": false,
    "memberCount": 2,
    "onlineCount": 1,
    "createdAt": "2024-03-21T10:30:00Z"
  },
  "message": "Group updated successfully",
  "statusCode": 200
}
```

**Error Response:** `404 Not Found`
```json
{
  "success": false,
  "data": null,
  "message": "Group not found",
  "errors": null,
  "statusCode": 404
}
```

---

### 5. Delete Group (Admin Only)
Delete a group and all its members.

**Endpoint:** `DELETE /api/groups/{id}`

**Authentication:** Required (Admin role only)

**Path Parameters:**
- `id` (Guid) - Group ID

**Response:** `200 OK`
```json
{
  "success": true,
  "data": true,
  "message": "Group deleted successfully",
  "statusCode": 200
}
```

**Error Response:** `403 Forbidden` (Non-admin user)
```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.3",
  "title": "Forbidden",
  "status": 403
}
```

**Error Response:** `404 Not Found`
```json
{
  "success": false,
  "data": false,
  "message": "Group not found",
  "errors": null,
  "statusCode": 404
}
```

---

### 6. Get Group Members
Retrieve all members of a specific group.

**Endpoint:** `GET /api/groups/{id}/members`

**Authentication:** Required

**Path Parameters:**
- `id` (Guid) - Group ID

**Response:** `200 OK`
```json
{
  "success": true,
  "data": [
    {
      "id": "member-guid",
      "groupId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
      "userId": "1",
      "role": "Member",
      "joinedAt": "2024-03-21T10:30:00Z",
      "user": {
        "id": "1",
        "name": "John Doe",
        "avatarUrl": null,
        "country": "United Kingdom",
        "isOnline": true
      }
    }
  ],
  "message": "Success",
  "statusCode": 200
}
```

**Error Response:** `404 Not Found`
```json
{
  "success": false,
  "data": null,
  "message": "Group not found",
  "errors": null,
  "statusCode": 404
}
```

---

### 7. Add Member to Group
Add a user to a group.

**Endpoint:** `POST /api/groups/{id}/members/{userId}`

**Authentication:** Required

**Path Parameters:**
- `id` (Guid) - Group ID
- `userId` (int) - User ID to add

**Response:** `201 Created`
```json
{
  "success": true,
  "data": {
    "id": "member-guid",
    "groupId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "userId": "1",
    "role": "Member",
    "joinedAt": "2024-03-21T10:30:00Z",
    "user": {
      "id": "1",
      "name": "John Doe",
      "avatarUrl": null,
      "country": "United Kingdom",
      "isOnline": true
    }
  },
  "message": "User added to group successfully",
  "statusCode": 201
}
```

**Error Response:** `400 Bad Request` (Group full)
```json
{
  "success": false,
  "data": null,
  "message": "Group is full",
  "errors": [
    "Maximum group size is 2"
  ],
  "statusCode": 400
}
```

**Error Response:** `400 Bad Request` (Already member)
```json
{
  "success": false,
  "data": null,
  "message": "User is already a member of this group",
  "errors": null,
  "statusCode": 400
}
```

**Error Response:** `404 Not Found`
```json
{
  "success": false,
  "data": null,
  "message": "Group not found",
  "errors": null,
  "statusCode": 404
}
```

---

### 8. Remove Member from Group
Remove a user from a group.

**Endpoint:** `DELETE /api/groups/{id}/members/{userId}`

**Authentication:** Required

**Authorization:** 
- Users can only remove themselves from a group
- Admins can remove any user

**Path Parameters:**
- `id` (Guid) - Group ID
- `userId` (int) - User ID to remove

**Response:** `200 OK`
```json
{
  "success": true,
  "data": true,
  "message": "Member removed from group successfully",
  "statusCode": 200
}
```

**Error Response:** `403 Forbidden` (Unauthorized)
```json
{
  "success": false,
  "data": false,
  "message": "You can only remove yourself from a group",
  "errors": null,
  "statusCode": 403
}
```

**Error Response:** `404 Not Found`
```json
{
  "success": false,
  "data": null,
  "message": "Member not found in group",
  "errors": null,
  "statusCode": 404
}
```

---

### 9. Get My Groups
Get all groups for the currently authenticated user.

**Endpoint:** `GET /api/groups/my-groups`

**Authentication:** Required

**Response:** `200 OK`
```json
{
  "success": true,
  "data": {
    "groups": [
      {
        "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
        "name": "Group1",
        "description": "Auto-generated group for student connections",
        "imageUrl": null,
        "category": "General",
        "isPrivate": false,
        "memberCount": 2,
        "onlineCount": 1,
        "createdAt": "2024-03-21T10:30:00Z"
      }
    ],
    "totalGroups": 1,
    "totalMemberCount": 2
  },
  "message": "Success",
  "statusCode": 200
}
```

---

### 10. Get User Groups
Get all groups for a specific user.

**Endpoint:** `GET /api/groups/users/{userId}/groups`

**Authentication:** Required

**Path Parameters:**
- `userId` (int) - User ID

**Response:** `200 OK`
```json
{
  "success": true,
  "data": {
    "groups": [
      {
        "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
        "name": "Group1",
        "description": "Auto-generated group for student connections",
        "imageUrl": null,
        "category": "General",
        "isPrivate": false,
        "memberCount": 2,
        "onlineCount": 1,
        "createdAt": "2024-03-21T10:30:00Z"
      }
    ],
    "totalGroups": 1,
    "totalMemberCount": 2
  },
  "message": "Success",
  "statusCode": 200
}
```

**Error Response:** `404 Not Found`
```json
{
  "success": false,
  "data": null,
  "message": "User not found",
  "errors": null,
  "statusCode": 404
}
```

---

## Usage Examples

### Using cURL

#### Get All Groups
```bash
curl -X GET "http://localhost:5000/api/groups" \
  -H "Authorization: Bearer {your_jwt_token}"
```

#### Create Group
```bash
curl -X POST "http://localhost:5000/api/groups" \
  -H "Authorization: Bearer {your_jwt_token}" \
  -H "Content-Type: application/json" \
  -d '{
    "name": "Study Group",
    "description": "Computer Science study group",
    "category": "Academic",
    "isPrivate": false
  }'
```

#### Update Group
```bash
curl -X PUT "http://localhost:5000/api/groups/{group-id}" \
  -H "Authorization: Bearer {your_jwt_token}" \
  -H "Content-Type: application/json" \
  -d '{
    "name": "Updated Study Group",
    "description": "Updated description"
  }'
```

#### Delete Group (Admin Only)
```bash
curl -X DELETE "http://localhost:5000/api/groups/{group-id}" \
  -H "Authorization: Bearer {admin_jwt_token}"
```

#### Add Member to Group
```bash
curl -X POST "http://localhost:5000/api/groups/{group-id}/members/{user-id}" \
  -H "Authorization: Bearer {your_jwt_token}"
```

#### Remove Member from Group
```bash
curl -X DELETE "http://localhost:5000/api/groups/{group-id}/members/{user-id}" \
  -H "Authorization: Bearer {your_jwt_token}"
```

#### Get My Groups
```bash
curl -X GET "http://localhost:5000/api/groups/my-groups" \
  -H "Authorization: Bearer {your_jwt_token}"
```

---

## Business Rules

### Group Creation
- Any authenticated user can create a group
- Group names must be 2-100 characters
- Groups expire after 3 months by default
- Groups are active by default

### Group Membership
- Maximum group size is configurable (default: 2)
- Users automatically join a group on signup
- Users cannot join a group they're already in
- Users can be in multiple groups

### Group Updates
- Any authenticated user can update a group
- Only provided fields are updated
- Group ID cannot be changed

### Group Deletion
- **Only admins** can delete groups
- Deleting a group removes all members
- Cascade delete removes associated:
  - Members
  - Messages
  - Polls
  - Checklist items
  - Icebreakers

### Member Removal
- Users can remove themselves from any group
- Admins can remove any user from any group
- Regular users cannot remove other users

---

## Error Handling

### Common Error Codes
- `400 Bad Request` - Validation failed or business rule violation
- `401 Unauthorized` - Missing or invalid JWT token
- `403 Forbidden` - Insufficient permissions (not admin)
- `404 Not Found` - Group or user not found
- `500 Internal Server Error` - Server error

### Error Response Format
```json
{
  "success": false,
  "data": null,
  "message": "Error description",
  "errors": [
    "Detailed error 1",
    "Detailed error 2"
  ],
  "statusCode": 400
}
```

---

## Configuration

### Group Settings (appsettings.json)
```json
{
  "GroupRotationSettings": {
    "MaxGroupSize": 2,
    "IntervalInHours": 24,
    "Enabled": true,
    "RunOnStartup": false
  }
}
```

### Settings Description
- `MaxGroupSize`: Maximum number of members per group
- `IntervalInHours`: Hours between automatic group rotations
- `Enabled`: Enable/disable automatic group rotation
- `RunOnStartup`: Trigger rotation on application startup

---

## Related Endpoints

### Admin Operations
- `POST /api/admin/trigger-group-rotation` - Manually trigger group shuffle
- `GET /api/admin/group-stats` - Get group statistics

### User Operations
- `GET /api/users/{id}/groups` - Get groups for a specific user (via UsersController)

---

## Testing with Swagger

Access Swagger UI at: `http://localhost:5000/swagger`

1. Click "Authorize" button
2. Enter your JWT token
3. Test all endpoints interactively
4. View request/response schemas

---

## Notes

- All timestamps are in UTC
- GUIDs are used for group IDs
- Integer IDs are used for user IDs
- Online status threshold is 10 minutes
- Groups are soft-deleted (IsActive flag)
- Member count includes all members
- Online count only includes recently active users

---

## Future Enhancements

Potential features for future development:
- Group categories and filtering
- Private/public group types
- Group invitations
- Group roles (admin, moderator, member)
- Group search functionality
- Group avatars/images
- Group tags and interests
- Group activity feed
- Group analytics
