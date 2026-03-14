# Notifications API Documentation

## ✅ Notifications API Complete

Full Notifications API implementation with service layer and controller matching the TypeScript API specification.

---

## 🔔 API Endpoints

| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| GET | `/api/users/{userId}/notifications` | Get all user notifications | ✅ Yes |
| PATCH | `/api/notifications/{id}` | Mark notification as read | ✅ Yes (own notification) |
| POST | `/api/users/{userId}/notifications/read-all` | Mark all as read | ✅ Yes (own notifications) |
| DELETE | `/api/notifications/{id}` | Delete notification | ✅ Yes (own notification) |

---

## 📦 Implementation Details

### Service Layer
- ✅ `Services/INotificationService.cs` - Service interface
- ✅ `Services/NotificationService.cs` - Complete implementation

### Controllers
- ✅ `Controllers/NotificationsController.cs` - Notification-specific endpoints
- ✅ `Controllers/UsersController.cs` - Updated with mark-all-as-read endpoint

### Features
- ✅ Authorization checks (users can only access their own notifications)
- ✅ Ordered by creation date (newest first)
- ✅ Comprehensive error handling
- ✅ ApiResponse wrapper for consistent responses

---

## 📝 Usage Examples

### 1. Get All Notifications

```http
GET /api/users/1/notifications
Authorization: Bearer {jwt_token}
```

**Response:**
```json
{
  "success": true,
  "data": [
    {
      "id": "guid-here",
      "userId": "1",
      "type": "Group",
      "title": "New Group Invitation",
      "message": "You've been invited to join CS Study Group",
      "isRead": false,
      "readAt": null,
      "data": "group-guid",
      "createdAt": "2024-01-15T10:30:00Z"
    },
    {
      "id": "guid-here-2",
      "userId": "1",
      "type": "Event",
      "title": "Event Reminder",
      "message": "International Food Festival starts in 1 hour",
      "isRead": true,
      "readAt": null,
      "data": "event-guid",
      "createdAt": "2024-01-15T09:00:00Z"
    }
  ],
  "message": "",
  "errors": [],
  "statusCode": 200
}
```

---

### 2. Mark Notification as Read

```http
PATCH /api/notifications/{notification-guid}
Authorization: Bearer {jwt_token}
Content-Type: application/json

{
  "read": true
}
```

**Response:**
```json
{
  "success": true,
  "data": {
    "id": "guid-here",
    "userId": "1",
    "type": "Group",
    "title": "New Group Invitation",
    "message": "You've been invited to join CS Study Group",
    "isRead": true,
    "readAt": null,
    "data": "group-guid",
    "createdAt": "2024-01-15T10:30:00Z"
  },
  "message": "Notification marked as read",
  "errors": [],
  "statusCode": 200
}
```

**Error Response (Not Owner):**
```json
{
  "success": false,
  "data": null,
  "message": "You don't have permission to modify this notification",
  "errors": [],
  "statusCode": 403
}
```

---

### 3. Mark All Notifications as Read

```http
POST /api/users/1/notifications/read-all
Authorization: Bearer {jwt_token}
```

**Response:**
```json
{
  "success": true,
  "data": true,
  "message": "5 notification(s) marked as read",
  "errors": [],
  "statusCode": 200
}
```

**No Unread Notifications:**
```json
{
  "success": true,
  "data": true,
  "message": "No unread notifications to mark",
  "errors": [],
  "statusCode": 200
}
```

---

### 4. Delete Notification

```http
DELETE /api/notifications/{notification-guid}
Authorization: Bearer {jwt_token}
```

**Response:**
```json
{
  "success": true,
  "data": true,
  "message": "Notification deleted successfully",
  "errors": [],
  "statusCode": 200
}
```

**Error Response (Not Found):**
```json
{
  "success": false,
  "data": false,
  "message": "Notification not found",
  "errors": [],
  "statusCode": 404
}
```

---

## 🔐 Security & Authorization

### Authorization Rules
1. **Get All Notifications**: User can only view their own notifications
2. **Mark as Read**: User can only mark their own notifications as read
3. **Mark All as Read**: User can only mark their own notifications
4. **Delete**: User can only delete their own notifications

### Validation
- All endpoints require JWT authentication
- User ID from JWT token is used for authorization
- Notification ownership is verified before any modification

---

## 🎯 Notification Types

The `Type` field in NotificationDto can be:
- `Group` - Group-related notifications
- `Event` - Event-related notifications
- `Message` - Message notifications
- `Announcement` - Announcement notifications
- `System` - System notifications

---

## 💡 Service Methods

### INotificationService

```csharp
public interface INotificationService
{
    // Get all notifications for a user (ordered by newest first)
    Task<ApiResponse<List<NotificationDto>>> GetAllNotificationsAsync(int userId);
    
    // Mark a single notification as read
    Task<ApiResponse<NotificationDto>> MarkAsReadAsync(Guid notificationId, int userId);
    
    // Mark all user notifications as read
    Task<ApiResponse<bool>> MarkAllAsReadAsync(int userId);
    
    // Delete a notification
    Task<ApiResponse<bool>> DeleteNotificationAsync(Guid notificationId, int userId);
}
```

---

## 🔄 Integration Examples

### React/TypeScript Client

```typescript
import { notificationsApi } from './api/notifications';

// Get all notifications
const response = await notificationsApi.getAll(userId);
if (response.success) {
  console.log('Notifications:', response.data);
}

// Mark as read
await notificationsApi.markAsRead(notificationId);

// Mark all as read
await notificationsApi.markAllAsRead(userId);

// Delete notification
await notificationsApi.delete(notificationId);
```

### C# Client

```csharp
// Get all notifications
var notificationsResponse = await userService.GetUserNotificationsAsync(userId);

// Mark as read
var markReadResponse = await notificationService.MarkAsReadAsync(notificationId, userId);

// Mark all as read
var markAllResponse = await notificationService.MarkAllAsReadAsync(userId);

// Delete notification
var deleteResponse = await notificationService.DeleteNotificationAsync(notificationId, userId);
```

---

## 📊 Database Queries

### Get All Notifications
- Filters by `userId`
- Orders by `CreatedAt` descending (newest first)
- Returns all fields as `NotificationDto`

### Mark as Read
- Finds notification by ID
- Verifies ownership
- Sets `IsRead = true`
- Saves changes

### Mark All as Read
- Finds all unread notifications for user
- Batch updates all to `IsRead = true`
- Returns count of updated notifications

### Delete Notification
- Finds notification by ID
- Verifies ownership
- Removes from database

---

## 🧪 Testing Examples

### cURL Examples

**Get Notifications:**
```bash
curl -X GET "https://localhost:5001/api/users/1/notifications" \
  -H "Authorization: Bearer YOUR_JWT_TOKEN"
```

**Mark as Read:**
```bash
curl -X PATCH "https://localhost:5001/api/notifications/{guid}" \
  -H "Authorization: Bearer YOUR_JWT_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{"read": true}'
```

**Mark All as Read:**
```bash
curl -X POST "https://localhost:5001/api/users/1/notifications/read-all" \
  -H "Authorization: Bearer YOUR_JWT_TOKEN"
```

**Delete Notification:**
```bash
curl -X DELETE "https://localhost:5001/api/notifications/{guid}" \
  -H "Authorization: Bearer YOUR_JWT_TOKEN"
```

---

## 🚨 Error Handling

### Common Error Responses

**401 Unauthorized:**
```json
{
  "success": false,
  "data": null,
  "message": "Unauthorized",
  "errors": [],
  "statusCode": 401
}
```

**403 Forbidden:**
```json
{
  "success": false,
  "data": null,
  "message": "You don't have permission to modify this notification",
  "errors": [],
  "statusCode": 403
}
```

**404 Not Found:**
```json
{
  "success": false,
  "data": null,
  "message": "Notification not found",
  "errors": [],
  "statusCode": 404
}
```

**500 Internal Server Error:**
```json
{
  "success": false,
  "data": null,
  "message": "Failed to retrieve notifications",
  "errors": ["Detailed error message"],
  "statusCode": 500
}
```

---

## 📈 Performance Considerations

1. **Indexing**: Notifications table has indexes on:
   - `UserId` (for filtering user notifications)
   - `IsRead` (for filtering unread notifications)
   - `CreatedAt` (for ordering)

2. **Query Optimization**: 
   - Uses efficient `Where` clauses
   - Selects only required fields
   - Orders in database, not in memory

3. **Batch Operations**: 
   - Mark all as read uses batch update
   - More efficient than individual updates

---

## ✔️ Status

**Build:** ✅ Successful  
**Service:** ✅ Implemented  
**Controller:** ✅ Complete  
**Authorization:** ✅ Configured  
**Error Handling:** ✅ Complete  
**Documentation:** ✅ Done  

All Notifications API endpoints are fully functional and ready to use! 🎉
