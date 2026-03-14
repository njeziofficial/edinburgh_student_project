# Messages API Documentation

## ✅ Messages API Complete

Full group messaging implementation with reactions, pagination, and authorization matching the TypeScript API specification.

---

## 💬 API Endpoints

| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| GET | `/api/groups/{groupId}/messages` | Get group messages (paginated) | ✅ Yes (member) |
| POST | `/api/groups/{groupId}/messages` | Send message to group | ✅ Yes (member) |
| POST | `/api/groups/{groupId}/messages/{messageId}/reactions` | Add reaction | ✅ Yes (member) |
| DELETE | `/api/groups/{groupId}/messages/{messageId}/reactions/{emoji}` | Remove reaction | ✅ Yes (owner) |
| DELETE | `/api/groups/{groupId}/messages/{messageId}` | Delete message | ✅ Yes (sender) |

---

## 📦 Implementation Details

### Service Layer
- ✅ `Services/IMessageService.cs` - Service interface
- ✅ `Services/MessageService.cs` - Complete implementation

### Controllers
- ✅ `Controllers/MessagesController.cs` - Nested under groups with 5 endpoints

### Features
- ✅ **Group membership verification** - Only members can view/send messages
- ✅ **Pagination** - Efficient message loading with limit/offset
- ✅ **Reactions** - Emoji reactions with add/remove
- ✅ **Message ownership** - Only sender can delete messages
- ✅ **URL encoding** - Proper emoji encoding in URLs
- ✅ **Comprehensive error handling**
- ✅ **ApiResponse wrapper** for consistent responses

---

## 📝 Usage Examples

### 1. Get Group Messages

```http
GET /api/groups/{group-guid}/messages?limit=50&offset=0
Authorization: Bearer {jwt_token}
```

**Query Parameters:**
- `limit` (optional, default: 50) - Maximum messages to return
- `offset` (optional, default: 0) - Number of messages to skip

**Response:**
```json
{
  "success": true,
  "data": [
    {
      "id": "message-guid",
      "content": "Hey everyone! Looking forward to our study session.",
      "senderId": "1",
      "recipientId": null,
      "groupId": "group-guid",
      "isRead": false,
      "readAt": null,
      "isEdited": false,
      "editedAt": null,
      "isDeleted": false,
      "sender": {
        "id": "1",
        "name": "John Doe",
        "avatarUrl": null,
        "country": null,
        "isOnline": false
      },
      "createdAt": "2024-01-15T10:30:00Z"
    },
    {
      "id": "message-guid-2",
      "content": "Sounds great! What time works best?",
      "senderId": "2",
      "recipientId": null,
      "groupId": "group-guid",
      "isRead": false,
      "readAt": null,
      "isEdited": false,
      "editedAt": null,
      "isDeleted": false,
      "sender": {
        "id": "2",
        "name": "Jane Smith",
        "avatarUrl": null,
        "country": null,
        "isOnline": false
      },
      "createdAt": "2024-01-15T10:32:00Z"
    }
  ],
  "message": "",
  "errors": [],
  "statusCode": 200
}
```

**Error Response (Not a Member):**
```json
{
  "success": false,
  "data": null,
  "message": "You must be a member of this group to view messages",
  "errors": [],
  "statusCode": 403
}
```

---

### 2. Send Message

```http
POST /api/groups/{group-guid}/messages
Authorization: Bearer {jwt_token}
Content-Type: application/json

{
  "content": "I'll be there at 6 PM!"
}
```

**Response:**
```json
{
  "success": true,
  "data": {
    "id": "new-message-guid",
    "content": "I'll be there at 6 PM!",
    "senderId": "1",
    "recipientId": null,
    "groupId": "group-guid",
    "isRead": false,
    "readAt": null,
    "isEdited": false,
    "editedAt": null,
    "isDeleted": false,
    "sender": {
      "id": "1",
      "name": "John Doe",
      "avatarUrl": null,
      "country": null,
      "isOnline": false
    },
    "createdAt": "2024-01-15T10:35:00Z"
  },
  "message": "Message sent successfully",
  "errors": [],
  "statusCode": 201
}
```

**Error Response (Not a Member):**
```json
{
  "success": false,
  "data": null,
  "message": "You must be a member of this group to send messages",
  "errors": [],
  "statusCode": 403
}
```

---

### 3. Add Reaction

```http
POST /api/groups/{group-guid}/messages/{message-guid}/reactions
Authorization: Bearer {jwt_token}
Content-Type: application/json

{
  "emoji": "👍"
}
```

**Response:**
```json
{
  "success": true,
  "data": true,
  "message": "Reaction added successfully",
  "errors": [],
  "statusCode": 200
}
```

**Already Exists:**
```json
{
  "success": true,
  "data": true,
  "message": "Reaction already exists",
  "errors": [],
  "statusCode": 200
}
```

---

### 4. Remove Reaction

```http
DELETE /api/groups/{group-guid}/messages/{message-guid}/reactions/%F0%9F%91%8D
Authorization: Bearer {jwt_token}
```

**Note:** Emoji must be URL-encoded. `👍` becomes `%F0%9F%91%8D`

**Response:**
```json
{
  "success": true,
  "data": true,
  "message": "Reaction removed successfully",
  "errors": [],
  "statusCode": 200
}
```

**Error Response (Not Found):**
```json
{
  "success": false,
  "data": false,
  "message": "Reaction not found",
  "errors": [],
  "statusCode": 404
}
```

---

### 5. Delete Message

```http
DELETE /api/groups/{group-guid}/messages/{message-guid}
Authorization: Bearer {jwt_token}
```

**Response:**
```json
{
  "success": true,
  "data": true,
  "message": "Message deleted successfully",
  "errors": [],
  "statusCode": 200
}
```

**Error Response (Not Sender):**
```json
{
  "success": false,
  "data": false,
  "message": "You can only delete your own messages",
  "errors": [],
  "statusCode": 403
}
```

---

## 🔐 Security & Authorization

### Authorization Rules
1. **Get Messages**: User must be a group member
2. **Send Message**: User must be a group member
3. **Add Reaction**: User must be a group member
4. **Remove Reaction**: User can only remove their own reactions
5. **Delete Message**: User can only delete their own messages

### Group Verification
- Group must exist and be active
- User membership is verified for all operations
- Inactive groups cannot receive new messages

---

## 📊 Pagination

### Parameters
- **limit**: Maximum number of messages (default: 50, recommended: 20-100)
- **offset**: Number of messages to skip (default: 0)

### Example Pagination
```http
# Get first 20 messages
GET /api/groups/{groupId}/messages?limit=20&offset=0

# Get next 20 messages
GET /api/groups/{groupId}/messages?limit=20&offset=20

# Get messages 40-60
GET /api/groups/{groupId}/messages?limit=20&offset=40
```

### Order
Messages are returned in **descending order by creation date** (newest first).

---

## 😊 Emoji Reactions

### Supported Emojis
Any valid Unicode emoji can be used, including:
- 👍 👎 ❤️ 😂 😮 😢 😡
- 🎉 🔥 ⭐ ✅ ❌
- And many more...

### URL Encoding
When removing reactions via DELETE, emojis must be URL-encoded:
- `👍` → `%F0%9F%91%8D`
- `❤️` → `%E2%9D%A4%EF%B8%8F`
- `😂` → `%F0%9F%98%82`

JavaScript automatically encodes:
```javascript
const emoji = "👍";
const encoded = encodeURIComponent(emoji);
// Use in URL: /reactions/${encoded}
```

### Reaction Constraints
- One user can only add the same emoji once per message
- Duplicate reactions are prevented at database level
- Users can have multiple different emojis on same message

---

## 💡 Service Methods

### IMessageService

```csharp
public interface IMessageService
{
    // Get messages with pagination
    Task<ApiResponse<List<MessageDto>>> GetGroupMessagesAsync(
        Guid groupId, 
        int userId, 
        int limit = 50, 
        int offset = 0
    );
    
    // Send a message
    Task<ApiResponse<MessageDto>> SendMessageAsync(
        Guid groupId, 
        int userId, 
        SendMessageRequest request
    );
    
    // Add emoji reaction
    Task<ApiResponse<bool>> AddReactionAsync(
        Guid groupId, 
        Guid messageId, 
        int userId, 
        string emoji
    );
    
    // Remove emoji reaction
    Task<ApiResponse<bool>> RemoveReactionAsync(
        Guid groupId, 
        Guid messageId, 
        int userId, 
        string emoji
    );
    
    // Delete message
    Task<ApiResponse<bool>> DeleteMessageAsync(
        Guid groupId, 
        Guid messageId, 
        int userId
    );
}
```

---

## 🔄 Integration Examples

### React/TypeScript Client

```typescript
import { messagesApi } from './api/messages';

// Get messages with pagination
const messages = await messagesApi.getGroupMessages(groupId, 50, 0);

// Load more messages (infinite scroll)
const moreMessages = await messagesApi.getGroupMessages(groupId, 20, currentOffset);

// Send message
const newMessage = await messagesApi.send(groupId, {
  content: "Hello everyone!"
});

// Add reaction
await messagesApi.addReaction(groupId, messageId, "👍");

// Remove reaction
await messagesApi.removeReaction(groupId, messageId, "👍");

// Delete message
await messagesApi.delete(groupId, messageId);
```

### C# Client

```csharp
// Get messages
var messages = await messageService.GetGroupMessagesAsync(groupId, userId, 50, 0);

// Send message
var request = new SendMessageRequest { Content = "Hello everyone!" };
var message = await messageService.SendMessageAsync(groupId, userId, request);

// Add reaction
await messageService.AddReactionAsync(groupId, messageId, userId, "👍");

// Remove reaction
await messageService.RemoveReactionAsync(groupId, messageId, userId, "👍");

// Delete message
await messageService.DeleteMessageAsync(groupId, messageId, userId);
```

---

## 📈 Performance Considerations

### Database Queries
1. **Get Messages**:
   - Uses efficient WHERE clause on GroupId
   - Orders in database (not in memory)
   - Includes related entities (Sender, Reactions) in one query
   - Pagination reduces data transfer

2. **Send Message**:
   - Validates group membership before insert
   - Single database transaction

3. **Reactions**:
   - Unique constraint prevents duplicates
   - Efficient lookup by message + user + emoji

### Indexes
Messages table has indexes on:
- `GroupId` - Fast filtering by group
- `SenderId` - Quick sender lookup
- `CreatedAt` - Efficient ordering

MessageReactions table has unique index on:
- `(MessageId, UserId, Emoji)` - Prevents duplicates

---

## 🧪 Testing Examples

### cURL Examples

**Get Messages:**
```bash
curl -X GET "https://localhost:5001/api/groups/{groupId}/messages?limit=50&offset=0" \
  -H "Authorization: Bearer YOUR_JWT_TOKEN"
```

**Send Message:**
```bash
curl -X POST "https://localhost:5001/api/groups/{groupId}/messages" \
  -H "Authorization: Bearer YOUR_JWT_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{"content":"Hello everyone!"}'
```

**Add Reaction:**
```bash
curl -X POST "https://localhost:5001/api/groups/{groupId}/messages/{messageId}/reactions" \
  -H "Authorization: Bearer YOUR_JWT_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{"emoji":"👍"}'
```

**Remove Reaction:**
```bash
# URL encode the emoji first
curl -X DELETE "https://localhost:5001/api/groups/{groupId}/messages/{messageId}/reactions/%F0%9F%91%8D" \
  -H "Authorization: Bearer YOUR_JWT_TOKEN"
```

**Delete Message:**
```bash
curl -X DELETE "https://localhost:5001/api/groups/{groupId}/messages/{messageId}" \
  -H "Authorization: Bearer YOUR_JWT_TOKEN"
```

---

## 🚨 Error Handling

### Common Error Responses

**403 Forbidden (Not a Member):**
```json
{
  "success": false,
  "data": null,
  "message": "You must be a member of this group to view messages",
  "errors": [],
  "statusCode": 403
}
```

**403 Forbidden (Delete Permission):**
```json
{
  "success": false,
  "data": false,
  "message": "You can only delete your own messages",
  "errors": [],
  "statusCode": 403
}
```

**404 Not Found (Group):**
```json
{
  "success": false,
  "data": null,
  "message": "Group not found",
  "errors": [],
  "statusCode": 404
}
```

**404 Not Found (Message):**
```json
{
  "success": false,
  "data": false,
  "message": "Message not found",
  "errors": [],
  "statusCode": 404
}
```

**400 Bad Request (Validation):**
```json
{
  "success": false,
  "data": null,
  "message": "Validation failed",
  "errors": [
    "Message must be between 1 and 2000 characters"
  ],
  "statusCode": 400
}
```

---

## 🎯 Real-Time Considerations

This REST API provides the foundation for group messaging. For real-time features, consider:

1. **SignalR Integration**: Add real-time message broadcasting
2. **WebSockets**: Push new messages to connected clients
3. **Notification Service**: Alert users of new messages
4. **Typing Indicators**: Show when users are typing
5. **Read Receipts**: Track message read status

---

## ✔️ Status

**Build:** ✅ Successful  
**Service:** ✅ Implemented  
**Controller:** ✅ Complete  
**Authorization:** ✅ Group membership verified  
**Pagination:** ✅ Implemented  
**Reactions:** ✅ Full support  
**Error Handling:** ✅ Complete  
**Documentation:** ✅ Done  

All Messages API endpoints are fully functional and ready for group communication! 💬🎉
