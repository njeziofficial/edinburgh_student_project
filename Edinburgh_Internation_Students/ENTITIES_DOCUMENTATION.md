# Student Connect - Entity Models Documentation

## Overview

All entities for the Student Connect platform have been successfully created and configured in Entity Framework Core. The system now includes 17 entity models representing the complete database schema.

---

## Entity Summary

| Entity | ID Type | Description |
|--------|---------|-------------|
| `User` | int | User accounts and authentication |
| `Profile` | int | Extended user profile information |
| `Group` | Guid | Student groups with expiration |
| `GroupMember` | Guid | User-Group many-to-many relationship |
| `GroupIcebreaker` | Guid | Icebreaker questions for groups |
| `ChecklistItem` | Guid | Group task checklist items |
| `Poll` | Guid | Group decision-making polls |
| `PollOption` | Guid | Options for each poll |
| `PollVote` | Guid | User votes on poll options |
| `Message` | Guid | Group chat messages |
| `MessageReaction` | Guid | Emoji reactions on messages |
| `Event` | Guid | Campus events |
| `EventAttendee` | Guid | Event RSVP tracking |
| `Notification` | Guid | User notifications |
| `Announcement` | Guid | Admin announcements |
| `RefreshToken` | Guid | JWT refresh token storage |

---

## Detailed Entity Descriptions

### 1. User (Existing - Enhanced)
Primary authentication and user account entity.

**Key Properties:**
- `Id` (int) - Primary key
- `FirstName`, `LastName` - Name components
- `Email` - Unique email address
- `Password` - BCrypt hashed password
- `PhoneCode`, `PhoneNumber` - Contact information
- `CreatedAt`, `UpdatedAt` - Timestamps

**Relationships:**
- One-to-One with `Profile`
- One-to-Many with `GroupMember` (as User)
- Creator/Author of various entities

---

### 2. Profile (Existing - Enhanced)
Extended profile information for matching and preferences.

**Key Properties:**
- `UserId` - Foreign key to User (unique)
- `HomeCountry` - Student's home country
- `Campus` - Campus location
- `MajorFieldOfStudy` - Academic major
- `YearOfStudy` - Academic year (enum)
- `Interests` - Comma-separated interests
- `PreferredGroupSize` - Group size preference (enum)
- `MatchingPreference` - Matching algorithm preference (enum)
- `Languages` - Comma-separated languages

**Relationships:**
- One-to-One with `User`

---

### 3. Group ✨ NEW
Student groups with time-based expiration.

**Key Properties:**
- `Id` (Guid)
- `Name` - Group name
- `Description` - Group description
- `ExpiresAt` - Automatic expiration timestamp
- `IsActive` - Active status flag
- `CreatedAt`

**Relationships:**
- One-to-Many with `GroupMember`
- One-to-Many with `GroupIcebreaker`
- One-to-Many with `ChecklistItem`
- One-to-Many with `Poll`
- One-to-Many with `Message`

**Use Case:** Creating temporary study groups that automatically expire after a set period.

---

### 4. GroupMember ✨ NEW
Junction table connecting users to groups.

**Key Properties:**
- `Id` (Guid)
- `GroupId` - Foreign key to Group
- `UserId` - Foreign key to User
- `JoinedAt` - Join timestamp

**Constraints:**
- Unique combination of `(GroupId, UserId)`

**Use Case:** Managing group membership with join tracking.

---

### 5. GroupIcebreaker ✨ NEW
Icebreaker questions to help group members connect.

**Key Properties:**
- `Id` (Guid)
- `GroupId` - Foreign key to Group
- `Question` - Icebreaker question text
- `OrderIndex` - Display order
- `CreatedAt`

**Use Case:** Facilitating initial conversations in new groups.

---

### 6. ChecklistItem ✨ NEW
Group task/checklist management.

**Key Properties:**
- `Id` (Guid)
- `GroupId` - Foreign key to Group
- `Title` - Task description
- `IsCompleted` - Completion status
- `CompletedBy` - User who completed (nullable)
- `CompletedAt` - Completion timestamp (nullable)
- `CreatedAt`

**Use Case:** Tracking group project tasks and milestones.

---

### 7. Poll ✨ NEW
Group decision-making polls.

**Key Properties:**
- `Id` (Guid)
- `GroupId` - Foreign key to Group
- `Question` - Poll question
- `CreatedBy` - Foreign key to User
- `IsActive` - Poll status
- `ClosesAt` - Optional closing time
- `CreatedAt`

**Relationships:**
- One-to-Many with `PollOption`
- One-to-Many with `PollVote`

**Use Case:** Democratic decision-making within groups.

---

### 8. PollOption ✨ NEW
Options for each poll.

**Key Properties:**
- `Id` (Guid)
- `PollId` - Foreign key to Poll
- `Text` - Option text
- `OrderIndex` - Display order

**Relationships:**
- Many-to-One with `Poll`
- One-to-Many with `PollVote`

---

### 9. PollVote ✨ NEW
Tracks user votes on poll options.

**Key Properties:**
- `Id` (Guid)
- `PollId` - Foreign key to Poll
- `OptionId` - Foreign key to PollOption
- `UserId` - Foreign key to User
- `VotedAt`

**Constraints:**
- Unique combination of `(PollId, UserId)` - One vote per user per poll

**Use Case:** Ensuring fair voting with one vote per user.

---

### 10. Message ✨ NEW
Group chat messages.

**Key Properties:**
- `Id` (Guid)
- `GroupId` - Foreign key to Group
- `SenderId` - Foreign key to User
- `Content` - Message text
- `IsEdited` - Edit tracking flag
- `CreatedAt`, `UpdatedAt`

**Relationships:**
- Many-to-One with `Group`
- Many-to-One with `User` (Sender)
- One-to-Many with `MessageReaction`

**Indexes:**
- GroupId (for efficient chat loading)
- SenderId
- CreatedAt (for chronological ordering)

**Use Case:** Real-time group communication.

---

### 11. MessageReaction ✨ NEW
Emoji reactions on messages.

**Key Properties:**
- `Id` (Guid)
- `MessageId` - Foreign key to Message
- `UserId` - Foreign key to User
- `Emoji` - Emoji character(s)
- `CreatedAt`

**Constraints:**
- Unique combination of `(MessageId, UserId, Emoji)`

**Use Case:** Expressing quick responses without sending messages.

---

### 12. Event ✨ NEW
Campus events and activities.

**Key Properties:**
- `Id` (Guid)
- `Title` - Event name
- `Description` - Event details
- `Date`, `Time` - Event scheduling
- `Location` - Event venue
- `Category` - EventCategory enum
- `OrganizerId` - Foreign key to User
- `MaxAttendees` - Capacity limit (nullable)
- `ImageUrl` - Event image (nullable)
- `IsCancelled` - Cancellation flag
- `CreatedAt`, `UpdatedAt`

**Relationships:**
- Many-to-One with `User` (Organizer)
- One-to-Many with `EventAttendee`

**Enums:**
- `EventCategory`: Cultural, Academic, Sports, Networking, Social, Other

**Use Case:** Event discovery and RSVP management.

---

### 13. EventAttendee ✨ NEW
Event RSVP tracking.

**Key Properties:**
- `Id` (Guid)
- `EventId` - Foreign key to Event
- `UserId` - Foreign key to User
- `RsvpAt` - RSVP timestamp

**Constraints:**
- Unique combination of `(EventId, UserId)`

**Use Case:** Managing event attendance and capacity.

---

### 14. Notification ✨ NEW
User notification system.

**Key Properties:**
- `Id` (Guid)
- `UserId` - Foreign key to User
- `Type` - NotificationType enum
- `Title` - Notification heading
- `Message` - Notification content
- `ReferenceId` - Related entity ID (nullable)
- `IsRead` - Read status
- `CreatedAt`

**Enums:**
- `NotificationType`: Group, Event, Message, Announcement, System

**Indexes:**
- UserId (for user's notifications)
- IsRead (for unread filtering)
- CreatedAt (for chronological display)

**Use Case:** Keeping users informed of activity and updates.

---

### 15. Announcement ✨ NEW
Admin announcements and updates.

**Key Properties:**
- `Id` (Guid)
- `Title` - Announcement heading
- `Content` - Announcement body
- `AuthorId` - Foreign key to User (admin)
- `Priority` - AnnouncementPriority enum
- `IsActive` - Visibility flag
- `CreatedAt`, `UpdatedAt`

**Enums:**
- `AnnouncementPriority`: Low, Normal, High

**Use Case:** Broadcasting important information to all users.

---

### 16. RefreshToken ✨ NEW
JWT refresh token management.

**Key Properties:**
- `Id` (Guid)
- `UserId` - Foreign key to User
- `Token` - Refresh token string
- `ExpiresAt` - Token expiration
- `CreatedAt`
- `RevokedAt` - Revocation timestamp (nullable)

**Helper Properties:**
- `IsExpired` - Computed property
- `IsRevoked` - Computed property
- `IsActive` - Computed property

**Constraints:**
- Unique `Token`

**Use Case:** Secure token refresh mechanism for authentication.

---

## Enumerations

### YearOfStudy (Existing)
```csharp
Freshman = 1
Sophomore = 2
Junior = 3
Senior = 4
```

### PreferredGroupSize (Existing)
```csharp
Small = 1    // 3-4 members
Medium = 2   // 5-6 members
Large = 3    // 7-8 members
```

### MatchingPreference (Existing)
```csharp
SameCountry = 1
DifferentCountries = 2
NoPreference = 3
```

### EventCategory ✨ NEW
```csharp
Cultural = 1
Academic = 2
Sports = 3
Networking = 4
Social = 5
Other = 6
```

### NotificationType ✨ NEW
```csharp
Group = 1
Event = 2
Message = 3
Announcement = 4
System = 5
```

### AnnouncementPriority ✨ NEW
```csharp
Low = 1
Normal = 2
High = 3
```

---

## Database Indexes

For optimal query performance, the following indexes have been configured:

### User Table
- `Email` (Unique)

### Profile Table
- `UserId` (Unique)

### Group Table
- `ExpiresAt`
- `IsActive`

### GroupMember Table
- `GroupId`
- `UserId`
- `(GroupId, UserId)` (Unique)

### ChecklistItem Table
- `GroupId`

### Poll Table
- `GroupId`

### PollVote Table
- `(PollId, UserId)` (Unique)

### Message Table
- `GroupId`
- `SenderId`
- `CreatedAt`

### MessageReaction Table
- `(MessageId, UserId, Emoji)` (Unique)

### Event Table
- `Date`
- `Category`
- `OrganizerId`

### EventAttendee Table
- `(EventId, UserId)` (Unique)

### Notification Table
- `UserId`
- `IsRead`
- `CreatedAt`

### Announcement Table
- `Priority`
- `CreatedAt`

### RefreshToken Table
- `UserId`
- `Token` (Unique)

---

## Cascade Delete Behavior

The following cascade delete rules are configured:

| Parent Entity | Child Entity | Behavior |
|---------------|--------------|----------|
| User | Profile | CASCADE |
| Group | GroupMember | CASCADE |
| Group | GroupIcebreaker | CASCADE |
| Group | ChecklistItem | CASCADE |
| Group | Poll | CASCADE |
| Group | Message | CASCADE |
| Poll | PollOption | CASCADE |
| Poll | PollVote | CASCADE |
| Message | MessageReaction | CASCADE |
| Event | EventAttendee | CASCADE |
| User | Notification | CASCADE |
| User | Announcement | CASCADE |
| User | RefreshToken | CASCADE |

**SET NULL Behavior:**
- ChecklistItem.CompletedBy → User (SET NULL on user delete)

---

## Next Steps

### 1. Create Migration
```bash
dotnet ef migrations add AddStudentConnectEntities
```

### 2. Update Database
```bash
dotnet ef database update
```

### 3. Implement Services
Create service layers for:
- Group management
- Messaging
- Events
- Notifications
- Polls
- Announcements

### 4. Create Controllers
Implement API endpoints for all entities

### 5. Add SignalR
For real-time features:
- Group chat
- Notifications
- Online presence

---

## File Structure

```
Models/
├── User.cs (existing)
├── Profile.cs (existing)
├── YearOfStudy.cs (existing)
├── PreferredGroupSize.cs (existing)
├── MatchingPreference.cs (existing)
├── JwtSettings.cs (existing)
├── Group.cs ✨
├── GroupMember.cs ✨
├── GroupIcebreaker.cs ✨
├── ChecklistItem.cs ✨
├── Poll.cs ✨
├── PollOption.cs ✨
├── PollVote.cs ✨
├── Message.cs ✨
├── MessageReaction.cs ✨
├── Event.cs ✨
├── EventCategory.cs ✨
├── EventAttendee.cs ✨
├── Notification.cs ✨
├── NotificationType.cs ✨
├── Announcement.cs ✨
├── AnnouncementPriority.cs ✨
└── RefreshToken.cs ✨

Data/
└── ApplicationDbContext.cs (updated)
```

---

## Build Status

✅ **Build Successful** - All entities created and configured

All 17 entities have been successfully implemented with:
- Complete property definitions
- Navigation properties
- Database constraints
- Indexes for performance
- Cascade delete rules
- Enum conversions

Ready for migration and implementation!
