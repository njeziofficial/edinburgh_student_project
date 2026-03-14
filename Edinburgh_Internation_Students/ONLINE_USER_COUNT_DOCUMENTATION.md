# Online User Count in Groups

## ✅ Feature: Real-Time Online User Count

The `/api/users/{id}/groups` endpoint now includes the count of online users in each group based on active sessions.

---

## 🎯 What Was Implemented

### 1. **LastActive Tracking** ✅
- Added `LastActive` field to User model
- Tracks when user was last active
- Updated automatically on every authenticated request
- Updated on login

### 2. **Online Status Middleware** ✅
- Middleware updates `LastActive` timestamp
- Runs on every authenticated request
- Silent failure (doesn't break requests)

### 3. **Online Count in Group Response** ✅
- Each group shows count of currently online members
- Based on activity within last 10 minutes (configurable)
- Calculated in real-time on each request

---

## 📦 Files Created/Updated

### Created (3):
1. ✅ `Middleware/LastActiveMiddleware.cs` - Updates LastActive on requests
2. ✅ `Configuration/OnlineStatusSettings.cs` - Configuration model
3. ✅ Migration: `AddLastActiveToUser` - Database schema update

### Updated (6):
1. ✅ `Models/User.cs` - Added `LastActive` field
2. ✅ `DTOs/Groups/GroupDto.cs` - Added `OnlineCount` field
3. ✅ `Services/AuthService.cs` - Updates LastActive on login
4. ✅ `Services/UserService.cs` - Calculates online count in GetUserGroups
5. ✅ `Services/GroupService.cs` - Calculates online count in group methods
6. ✅ `Program.cs` - Registered middleware and settings
7. ✅ `appsettings.json` - Added OnlineStatusSettings

---

## 📝 New Response Format

### GET /api/users/{id}/groups

**Response:**
```json
{
  "success": true,
  "data": {
    "groups": [
      {
        "id": "group-guid-1",
        "name": "Group1",
        "description": "Auto-generated group for student connections",
        "imageUrl": null,
        "category": "General",
        "isPrivate": false,
        "memberCount": 5,
        "onlineCount": 3,
        "createdAt": "2024-01-15T10:00:00Z"
      },
      {
        "id": "group-guid-2",
        "name": "Group2",
        "description": "Auto-generated group for student connections",
        "imageUrl": null,
        "category": "General",
        "isPrivate": false,
        "memberCount": 4,
        "onlineCount": 1,
        "createdAt": "2024-01-15T11:00:00Z"
      }
    ],
    "totalGroups": 2,
    "totalMemberCount": 9
  },
  "message": "",
  "errors": [],
  "statusCode": 200
}
```

---

## ⚙️ Configuration

### appsettings.json

```json
{
  "OnlineStatusSettings": {
    "OnlineThresholdMinutes": 10,
    "UpdateOnEveryRequest": true
  }
}
```

### Settings Explained

| Setting | Type | Default | Description |
|---------|------|---------|-------------|
| `OnlineThresholdMinutes` | int | 10 | Minutes of inactivity before user is considered offline |
| `UpdateOnEveryRequest` | bool | true | Update LastActive on every authenticated request |

---

## 🔄 How It Works

### 1. User Signs In
```
POST /api/auth/login → Sets LastActive = NOW → Returns token
```

### 2. User Makes Authenticated Requests
```
Any API Call → Middleware intercepts → Updates LastActive → Continues request
```

### 3. Get Groups with Online Count
```
GET /api/users/1/groups
   ↓
Query groups
   ↓
For each group:
   Count members where LastActive >= (NOW - 10 minutes)
   ↓
Return groups with onlineCount
```

---

## 📊 Online Status Logic

### Considered Online:
```csharp
var onlineThreshold = DateTime.UtcNow.AddMinutes(-10);
user.LastActive >= onlineThreshold
```

### Example Timeline:
- **Now:** 12:00 PM
- **Online Threshold:** 11:50 AM
- **User A:** LastActive = 11:55 AM → ✅ **Online** (5 min ago)
- **User B:** LastActive = 11:45 AM → ❌ **Offline** (15 min ago)
- **User C:** LastActive = null → ❌ **Offline** (never logged in)

---

## 💡 Use Cases

### Frontend Display

```typescript
const response = await usersApi.getUserGroups(userId);

response.data.groups.forEach(group => {
  console.log(`${group.name}:`);
  console.log(`  Total Members: ${group.memberCount}`);
  console.log(`  Online Now: ${group.onlineCount}`);
  console.log(`  Offline: ${group.memberCount - group.onlineCount}`);
});
```

### React Component

```tsx
function GroupCard({ group }: { group: GroupDto }) {
  return (
    <div className="group-card">
      <h3>{group.name}</h3>
      <p>{group.description}</p>
      <div className="stats">
        <span>{group.memberCount} members</span>
        <span className="online">
          🟢 {group.onlineCount} online
        </span>
      </div>
    </div>
  );
}
```

---

## 🔍 Middleware Details

### LastActiveMiddleware

**Purpose:** Automatically update user's LastActive timestamp on every authenticated request

**Flow:**
1. Request comes in
2. Check if user is authenticated
3. Extract user ID from JWT claims
4. Find user in database
5. Update `LastActive = DateTime.UtcNow`
6. Save changes
7. Continue with request

**Error Handling:**
- Errors are logged but don't fail requests
- Ensures API remains responsive even if update fails
- Silent failure for better UX

---

## 🧪 Testing

### Test Online Status

**Step 1: Sign In User 1**
```bash
curl -X POST "https://localhost:5001/api/auth/login" \
  -H "Content-Type: application/json" \
  -d '{
    "email": "user1@test.com",
    "password": "Pass123!"
  }'
```

**Step 2: Get Groups Immediately**
```bash
curl -X GET "https://localhost:5001/api/users/1/groups" \
  -H "Authorization: Bearer TOKEN"
```

**Expected:** `onlineCount: 1` (User 1 just logged in)

---

**Step 3: Sign In User 2 (same group)**
```bash
curl -X POST "https://localhost:5001/api/auth/login" \
  -H "Content-Type: application/json" \
  -d '{
    "email": "user2@test.com",
    "password": "Pass123!"
  }'
```

**Step 4: Check Groups Again**
```bash
curl -X GET "https://localhost:5001/api/users/1/groups" \
  -H "Authorization: Bearer TOKEN"
```

**Expected:** `onlineCount: 2` (Both User 1 and 2 online)

---

**Step 5: Wait 11 Minutes**

**Step 6: Check Groups Again**
```bash
curl -X GET "https://localhost:5001/api/users/1/groups" \
  -H "Authorization: Bearer TOKEN"
```

**Expected:** `onlineCount: 1` (Only User making request is active)

---

## 📈 Performance Considerations

### Middleware Impact
- **Minimal overhead**: Single database update per request
- **Async operation**: Doesn't block request
- **Error isolated**: Failures don't affect API

### Query Performance
- Uses indexed queries
- Calculated in SQL (not in memory)
- Efficient threshold filtering

### Optimization Options

**Option 1: Update Less Frequently**
```json
{
  "OnlineStatusSettings": {
    "UpdateOnEveryRequest": false
  }
}
```

Then manually update on specific actions:
- Login/Logout
- Message sent
- Group activity
- Event RSVP

**Option 2: Use Caching**
```csharp
// Cache online counts for 1 minute
[ResponseCache(Duration = 60)]
public async Task<IActionResult> GetUserGroups(int id)
```

**Option 3: Redis for High Traffic**
```csharp
// Store LastActive in Redis instead of database
await redis.StringSetAsync($"user:{userId}:lastactive", DateTime.UtcNow);
```

---

## 🔧 Customization

### Change Online Threshold

**5 Minutes:**
```json
{
  "OnlineStatusSettings": {
    "OnlineThresholdMinutes": 5
  }
}
```

**15 Minutes:**
```json
{
  "OnlineStatusSettings": {
    "OnlineThresholdMinutes": 15
  }
}
```

### Disable Auto-Update
```json
{
  "OnlineStatusSettings": {
    "UpdateOnEveryRequest": false
  }
}
```

---

## 📊 Example Scenarios

### Scenario 1: Active Group
```json
{
  "name": "Group1",
  "memberCount": 5,
  "onlineCount": 4
}
```
**80% of members online** ✅

### Scenario 2: Inactive Group
```json
{
  "name": "Group2",
  "memberCount": 5,
  "onlineCount": 0
}
```
**All members offline** ⚠️

### Scenario 3: Mixed Activity
```json
{
  "groups": [
    { "name": "Group1", "memberCount": 5, "onlineCount": 3 },
    { "name": "Group2", "memberCount": 4, "onlineCount": 1 }
  ]
}
```
**Total: 9 members, 4 online** (44% online rate)

---

## 🚀 Additional Benefits

### Real-Time Insights
✅ See which groups are most active  
✅ Know when members are available  
✅ Better timing for group activities  
✅ Identify inactive groups  

### User Experience
✅ Show live online indicators  
✅ Suggest best time to chat  
✅ Display activity heatmaps  
✅ Encourage engagement  

---

## 🔒 Privacy Considerations

### Current Implementation
- Only shows COUNT, not which users are online
- Aggregated data only
- No specific user online status exposed

### Future Enhancements
If you want to show which specific users are online:

```csharp
public class GroupDto
{
    // ... existing fields
    public int OnlineCount { get; set; }
    public List<string> OnlineUserIds { get; set; } // Optional: list of online user IDs
}
```

---

## ✔️ Status

**Migration:** ✅ Created (AddLastActiveToUser)  
**User Model:** ✅ Updated with LastActive field  
**Middleware:** ✅ Created and registered  
**GroupDto:** ✅ Added OnlineCount field  
**Services:** ✅ Updated to calculate online count  
**Configuration:** ✅ Added OnlineStatusSettings  
**Build:** ✅ Successful  

---

## 🎉 Summary

The `/api/users/{id}/groups` endpoint now returns:
- ✅ **memberCount** - Total members in group
- ✅ **onlineCount** - Members active in last 10 minutes
- ✅ Real-time calculation based on `LastActive` timestamp
- ✅ Automatic tracking via middleware

Users are considered online if they made an authenticated request within the last 10 minutes!

---

## 🧪 Quick Test

1. Sign in with 2-3 different users
2. Call GET /api/users/1/groups
3. See `onlineCount` reflecting active users
4. Wait 11 minutes without making requests
5. Call GET /api/users/1/groups again
6. See `onlineCount` decrease to 1 (only the requester)

Your groups now show real-time online status! 🟢✨
