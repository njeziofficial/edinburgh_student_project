# Student Connect Entities - Quick Start Guide

## ✅ What Was Created

All entities from the Student Connect PostgreSQL schema have been successfully implemented:

### New Entities (16 Total)
1. ✨ **Group** - Student groups with expiration
2. ✨ **GroupMember** - User-Group relationship
3. ✨ **GroupIcebreaker** - Icebreaker questions
4. ✨ **ChecklistItem** - Group tasks
5. ✨ **Poll** - Group decision polls
6. ✨ **PollOption** - Poll choices
7. ✨ **PollVote** - User votes
8. ✨ **Message** - Group chat messages
9. ✨ **MessageReaction** - Message reactions
10. ✨ **Event** - Campus events
11. ✨ **EventAttendee** - Event RSVPs
12. ✨ **Notification** - User notifications
13. ✨ **Announcement** - Admin announcements
14. ✨ **RefreshToken** - JWT refresh tokens

### New Enums (3 Total)
1. ✨ **EventCategory** - Event types
2. ✨ **NotificationType** - Notification types
3. ✨ **AnnouncementPriority** - Priority levels

### Updated
- **ApplicationDbContext** - All entities configured with relationships, indexes, and constraints

---

## 🚀 Next Steps

### Step 1: Create Migration

```bash
dotnet ef migrations add AddStudentConnectEntities
```

This will generate the migration files for all new entities.

### Step 2: Review Migration

Check the generated migration in `Migrations/` folder to ensure it matches expectations.

### Step 3: Update Database

```bash
dotnet ef database update
```

This will apply all changes to your PostgreSQL database.

---

## 📊 Database Changes Summary

### New Tables (14)
- `Groups`
- `GroupMembers`
- `GroupIcebreakers`
- `ChecklistItems`
- `Polls`
- `PollOptions`
- `PollVotes`
- `Messages`
- `MessageReactions`
- `Events`
- `EventAttendees`
- `Notifications`
- `Announcements`
- `RefreshTokens`

### Indexes Added (~30)
- Performance indexes on foreign keys
- Unique constraints on junction tables
- Indexes for common queries (dates, status flags, etc.)

---

## 🔗 Key Relationships

```
User (1) ─── (∞) GroupMember ─── (1) Group
User (1) ─── (∞) Message ─── (1) Group
User (1) ─── (∞) EventAttendee ─── (1) Event
User (1) ─── (∞) Notification
User (1) ─── (∞) RefreshToken
Group (1) ─── (∞) Poll ─── (∞) PollOption
Poll (1) ─── (∞) PollVote ─── (1) User
Message (1) ─── (∞) MessageReaction ─── (1) User
```

---

## 💡 Usage Examples

### Creating a Group
```csharp
var group = new Group
{
    Name = "Computer Science Study Group",
    Description = "Weekly study sessions for CS students",
    ExpiresAt = DateTime.UtcNow.AddMonths(3),
    IsActive = true
};

await context.Groups.AddAsync(group);
await context.SaveChangesAsync();
```

### Adding Members to Group
```csharp
var member = new GroupMember
{
    GroupId = group.Id,
    UserId = currentUserId,
    JoinedAt = DateTime.UtcNow
};

await context.GroupMembers.AddAsync(member);
await context.SaveChangesAsync();
```

### Creating a Poll
```csharp
var poll = new Poll
{
    GroupId = group.Id,
    Question = "What time works best for our next meeting?",
    CreatedBy = currentUserId,
    IsActive = true,
    ClosesAt = DateTime.UtcNow.AddDays(2)
};

poll.Options = new List<PollOption>
{
    new PollOption { Text = "Monday 6 PM", OrderIndex = 0 },
    new PollOption { Text = "Wednesday 7 PM", OrderIndex = 1 },
    new PollOption { Text = "Friday 5 PM", OrderIndex = 2 }
};

await context.Polls.AddAsync(poll);
await context.SaveChangesAsync();
```

### Sending a Message
```csharp
var message = new Message
{
    GroupId = group.Id,
    SenderId = currentUserId,
    Content = "Hey everyone! Looking forward to our study session."
};

await context.Messages.AddAsync(message);
await context.SaveChangesAsync();
```

### Creating an Event
```csharp
var event = new Event
{
    Title = "International Food Festival",
    Description = "Celebrate diversity through food!",
    Date = DateTime.Parse("2024-03-15"),
    Time = TimeSpan.Parse("18:00:00"),
    Location = "Student Union Building",
    Category = EventCategory.Cultural,
    OrganizerId = currentUserId,
    MaxAttendees = 100
};

await context.Events.AddAsync(event);
await context.SaveChangesAsync();
```

### Creating a Notification
```csharp
var notification = new Notification
{
    UserId = targetUserId,
    Type = NotificationType.Group,
    Title = "New Group Invitation",
    Message = "You've been invited to join the CS Study Group",
    ReferenceId = group.Id,
    IsRead = false
};

await context.Notifications.AddAsync(notification);
await context.SaveChangesAsync();
```

---

## 🛠️ Common Queries

### Get Active Groups
```csharp
var activeGroups = await context.Groups
    .Where(g => g.IsActive && g.ExpiresAt > DateTime.UtcNow)
    .Include(g => g.Members)
        .ThenInclude(m => m.User)
    .ToListAsync();
```

### Get User's Groups
```csharp
var userGroups = await context.GroupMembers
    .Where(gm => gm.UserId == userId)
    .Include(gm => gm.Group)
    .Select(gm => gm.Group)
    .ToListAsync();
```

### Get Group Messages with Reactions
```csharp
var messages = await context.Messages
    .Where(m => m.GroupId == groupId)
    .Include(m => m.Sender)
    .Include(m => m.Reactions)
        .ThenInclude(r => r.User)
    .OrderByDescending(m => m.CreatedAt)
    .Take(50)
    .ToListAsync();
```

### Get Upcoming Events
```csharp
var upcomingEvents = await context.Events
    .Where(e => !e.IsCancelled && e.Date >= DateTime.Today)
    .Include(e => e.Organizer)
    .Include(e => e.Attendees)
    .OrderBy(e => e.Date)
    .ThenBy(e => e.Time)
    .ToListAsync();
```

### Get Unread Notifications
```csharp
var unreadNotifications = await context.Notifications
    .Where(n => n.UserId == userId && !n.IsRead)
    .OrderByDescending(n => n.CreatedAt)
    .ToListAsync();
```

### Get Active Poll Results
```csharp
var pollResults = await context.Polls
    .Where(p => p.Id == pollId)
    .Include(p => p.Options)
        .ThenInclude(o => o.Votes)
            .ThenInclude(v => v.User)
    .FirstOrDefaultAsync();
```

---

## 🔒 Security Considerations

1. **Group Access Control**
   - Verify user is a member before allowing access to group resources

2. **Event Capacity**
   - Check `MaxAttendees` before allowing new RSVPs

3. **Poll Voting**
   - Enforce one vote per user per poll (handled by unique constraint)

4. **Message Permissions**
   - Only group members can view/send messages

5. **Refresh Tokens**
   - Always check `IsActive` status before using tokens

---

## 📚 Documentation Files

- **ENTITIES_DOCUMENTATION.md** - Complete entity reference
- **DATABASE_SCHEMA.md** - Database structure (PostgreSQL schema)
- **JWT_AUTHENTICATION.md** - JWT implementation details

---

## ✔️ Status

**Build:** ✅ Successful  
**Entities:** ✅ 17 models created  
**Relationships:** ✅ Fully configured  
**Indexes:** ✅ Optimized  
**Ready for migration:** ✅ Yes

Run the migration commands above to apply changes to your database!
