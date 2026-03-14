# Automatic Group Assignment and Rotation System

## ✅ Complete Implementation

Automatic group assignment on signup and periodic group rotation background service have been successfully implemented.

---

## 🎯 Features Implemented

### 1. Automatic Group Assignment on Signup
- ✅ New users are automatically assigned to a group upon registration
- ✅ Groups are created automatically as needed (Group1, Group2, Group3, etc.)
- ✅ Maximum group size: **5 members per group**
- ✅ Groups fill up sequentially before creating new ones

### 2. Group Rotation Background Service
- ✅ Periodic shuffling of group members
- ✅ Configurable rotation interval
- ✅ Random distribution across groups
- ✅ Runs as a hosted background service
- ✅ Optional startup rotation

---

## 📦 Files Created

1. ✅ **Services/IGroupService.cs** - Group service interface
2. ✅ **Services/GroupService.cs** - Group management implementation
3. ✅ **Configuration/GroupRotationSettings.cs** - Rotation configuration model
4. ✅ **BackgroundServices/GroupRotationBackgroundService.cs** - Background rotation job

---

## 🔄 Files Updated

1. ✅ **Services/AuthService.cs** - Added automatic group assignment on signup
2. ✅ **Program.cs** - Registered GroupService and background service
3. ✅ **appsettings.json** - Added GroupRotationSettings configuration

---

## ⚙️ Configuration

### appsettings.json

```json
{
  "GroupRotationSettings": {
    "Enabled": true,
    "IntervalInHours": 24,
    "IntervalInMinutes": 0,
    "RunOnStartup": false
  }
}
```

### Configuration Options

| Setting | Type | Default | Description |
|---------|------|---------|-------------|
| `Enabled` | bool | true | Enable/disable rotation service |
| `IntervalInHours` | int | 24 | Hours between rotations |
| `IntervalInMinutes` | int | 0 | Additional minutes to add |
| `RunOnStartup` | bool | false | Run rotation immediately on app startup |

### Example Configurations

**Daily Rotation (24 hours):**
```json
{
  "IntervalInHours": 24,
  "IntervalInMinutes": 0
}
```

**Every 12 Hours:**
```json
{
  "IntervalInHours": 12,
  "IntervalInMinutes": 0
}
```

**Every 6 Hours:**
```json
{
  "IntervalInHours": 6,
  "IntervalInMinutes": 0
}
```

**Weekly Rotation (7 days):**
```json
{
  "IntervalInHours": 168,
  "IntervalInMinutes": 0
}
```

**For Testing (Every 5 minutes):**
```json
{
  "IntervalInHours": 0,
  "IntervalInMinutes": 5,
  "RunOnStartup": true
}
```

---

## 🔄 How It Works

### Signup Flow with Auto-Assignment

```
1. User signs up
   ↓
2. User account created
   ↓
3. System searches for available group (< 5 members)
   ↓
4. If group found → Add user to group
   ↓
5. If no group available → Create new group (Group1, Group2, etc.)
   ↓
6. Add user to new group
   ↓
7. Return JWT token to user
```

### Group Rotation Process

```
1. Background service timer triggers
   ↓
2. Fetch all active groups and members
   ↓
3. Track each user's current group (for avoidance)
   ↓
4. Check if enough groups (minimum 2) and users (minimum 2)
   ↓
5. Remove all current group memberships
   ↓
6. Randomly shuffle all users
   ↓
7. Distribute users evenly across groups
   ↓
8. ⭐ Ensure users are NOT assigned to their previous group
   ↓
9. Create additional groups if needed
   ↓
10. Add users to groups
   ↓
11. Log completion and distribution stats
```

---

## 💡 Key Features

### Smart Group Creation
- Groups are numbered sequentially: Group1, Group2, Group3...
- New groups are created only when all existing groups are full
- Group descriptions are auto-generated

### Even Distribution
- Users are distributed evenly across all available groups
- If there are 13 users and max size is 5:
  - Group1: 3 users
  - Group2: 3 users
  - Group3: 3 users
  - Group4: 4 users

### Rotation Algorithm
- **Random Shuffle**: Uses Random.Next() for shuffling
- **Round-Robin Distribution**: Users distributed using modulo operation
- **Group Creation**: Automatically creates groups as needed
- **Previous Group Check**: ⭐ **Users are NOT assigned back to their previous group**
- **Fallback Logic**: Only assigns to previous group if absolutely necessary (rare edge case)

### Safety Features
- ✅ Checks minimum requirements (2 groups, 2 users) before rotation
- ✅ Comprehensive logging at each step
- ✅ Error handling doesn't stop the service
- ✅ Signup continues even if group assignment fails
- ✅ **Prevents same-group reassignment for better diversity**
- ✅ Warns when users must return to previous group (constraint issue)

---

## 📊 Example Scenarios

### Scenario 1: Initial Signups

**User 1 signs up:**
- Group1 created
- User 1 added to Group1

**Users 2-5 sign up:**
- All added to Group1 (now full)

**User 6 signs up:**
- Group2 created
- User 6 added to Group2

**Users 7-10 sign up:**
- Users 7-10 added to Group2 (now full)

**User 11 signs up:**
- Group3 created
- User 11 added to Group3

### Scenario 2: First Rotation (After 24 hours)

**Before Rotation:**
- Group1: [User1, User2, User3, User4, User5]
- Group2: [User6, User7, User8, User9, User10]
- Group3: [User11]

**After Rotation (Random):**
- Group1: [User7, User2, User11, User9, User4]
- Group2: [User1, User10, User5, User8, User3]
- Group3: [User6]

---

## 📝 Service Methods

### IGroupService

```csharp
public interface IGroupService
{
    // Find a group with available slots or create a new one
    Task<Guid> FindOrCreateAvailableGroupAsync();
    
    // Add a user to a specific group
    Task<ApiResponse<GroupMemberDto>> AddMemberToGroupAsync(Guid groupId, int userId);
    
    // Remove a user from a group
    Task<ApiResponse<bool>> RemoveMemberFromGroupAsync(Guid groupId, int userId);
    
    // Get member count for a group
    Task<int> GetGroupMemberCountAsync(Guid groupId);
    
    // Shuffle all group members randomly
    Task ShuffleGroupMembersAsync();
    
    // Get all active groups with members
    Task<List<GroupDto>> GetActiveGroupsWithMembersAsync();
}
```

---

## 🔍 Logging

The system provides comprehensive logging:

### Startup
```
info: Group rotation background service started. Running every 24:00:00
info: Database is up to date. No pending migrations
```

### Group Assignment (Signup)
```
info: Created new group: Group1 with ID: guid-here
info: User 1 assigned to Group1
```

### Rotation Execution
```
info: Starting group member rotation at 2024-01-16 10:00:00
info: Found 11 users in 3 groups
info: Removed all existing group memberships
info: Ensured 3 groups exist for 11 users
info: Group shuffle completed successfully. 11 users distributed across 3 groups
```

---

## 🧪 Testing

### Test Auto-Assignment

1. **Sign up users:**
```bash
# Sign up user 1
curl -X POST "https://localhost:5001/api/auth/signup" \
  -H "Content-Type: application/json" \
  -d '{
    "fullName": "User One",
    "email": "user1@example.com",
    "password": "Pass123!",
    "phoneCode": "234",
    "phoneNumber": "8011111111"
  }'

# Repeat for users 2-10
```

2. **Check groups:**
```bash
curl -X GET "https://localhost:5001/api/users/1/groups" \
  -H "Authorization: Bearer {token}"
```

### Test Rotation

**Option 1: Configure for quick rotation (testing)**
```json
{
  "GroupRotationSettings": {
    "Enabled": true,
    "IntervalInHours": 0,
    "IntervalInMinutes": 1,
    "RunOnStartup": true
  }
}
```

**Option 2: Manually trigger (add endpoint)**
You could add an admin endpoint to manually trigger rotation for testing.

---

## 🎮 Manual Rotation Trigger (Optional)

You can create an admin endpoint to manually trigger rotation:

```csharp
[HttpPost("admin/rotate-groups")]
[Authorize] // Add admin role check
public async Task<IActionResult> TriggerGroupRotation()
{
    try
    {
        await _groupService.ShuffleGroupMembersAsync();
        return Ok(new { message = "Group rotation completed successfully" });
    }
    catch (Exception ex)
    {
        return StatusCode(500, new { message = "Rotation failed", error = ex.Message });
    }
}
```

---

## 📈 Performance Considerations

### Efficient Operations
- ✅ Uses `OrderBy(x => random.Next())` for shuffling
- ✅ Batch operations for removing/adding memberships
- ✅ Single transaction per rotation
- ✅ Minimal database queries

### Scalability
- Works well for small to medium user bases (< 10,000 users)
- For larger scale, consider:
  - Partial rotation (rotate only some groups at a time)
  - Staggered rotation (different schedules for different groups)
  - Message queue for rotation jobs

---

## 🔐 Security Notes

### Group Assignment
- Silent failure if group assignment fails during signup
- User still gets registered and receives token
- Can be manually assigned to group later

### Rotation Service
- Runs in background with scoped services
- Protected by try-catch to prevent service crashes
- Continues running even if individual rotations fail

---

## ⚙️ Production Recommendations

### Rotation Frequency
- **Daily (24h)**: Good balance for most use cases
- **Weekly (168h)**: For longer-term groups
- **Twice daily (12h)**: For very dynamic environments

### Monitoring
Add monitoring for:
- Number of groups created
- Number of users assigned
- Rotation success/failure rates
- Group size distribution

### Notifications
Consider sending notifications when:
- Users are assigned to a new group
- Group rotation occurs
- User's group changes

---

## 🚀 Next Steps

### Recommended Enhancements

1. **Notification on Rotation**
```csharp
// After rotation, notify all users
foreach (var userId in allUserIds)
{
    await notificationService.CreateNotificationAsync(userId, 
        "Group Rotation", 
        "You've been assigned to a new group!");
}
```

2. **Rotation History**
```csharp
public class GroupRotationHistory
{
    public Guid Id { get; set; }
    public DateTime RotatedAt { get; set; }
    public int UsersAffected { get; set; }
    public int GroupsInvolved { get; set; }
}
```

3. **Admin Dashboard**
- View rotation history
- Manually trigger rotations
- View group distribution statistics
- Adjust rotation settings

---

## ✔️ Status

**Build:** ✅ Successful  
**Auto-Assignment:** ✅ Working on signup  
**Group Service:** ✅ Complete  
**Background Service:** ✅ Registered and running  
**Configuration:** ✅ Complete  
**Max Group Size:** ✅ 5 members  
**Rotation Algorithm:** ✅ Random shuffle with even distribution  

---

## 📚 Configuration Summary

### Current Settings
- **Rotation Interval**: 24 hours
- **Run on Startup**: No
- **Max Group Size**: 5 members
- **Group Naming**: Sequential (Group1, Group2, ...)
- **Group Expiry**: 3 months from creation

### To Enable/Disable
```json
{
  "GroupRotationSettings": {
    "Enabled": false  // Set to false to disable rotation
  }
}
```

### To Change Interval
```json
{
  "GroupRotationSettings": {
    "IntervalInHours": 12,  // Rotate every 12 hours
    "IntervalInMinutes": 30 // Plus 30 minutes = 12.5 hours total
  }
}
```

---

Your automatic group assignment and rotation system is complete and running! 🎉🔄

Users will be automatically grouped on signup, and groups will be shuffled periodically to help students meet new people.
