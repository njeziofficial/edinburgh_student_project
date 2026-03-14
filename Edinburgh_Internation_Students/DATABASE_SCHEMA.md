# Database Schema Overview

## Entity Relationship Diagram

```
┌─────────────────────┐
│       Users         │
├─────────────────────┤
│ Id (PK)            │
│ FirstName          │
│ LastName           │
│ Email (Unique)     │
│ Password           │
│ PhoneCode          │
│ PhoneNumber        │
│ CreatedAt          │
│ UpdatedAt          │
└─────────────────────┘
         │
         │ 1:1 Relationship
         │ (One User has One Profile)
         ▼
┌─────────────────────┐
│      Profiles       │
├─────────────────────┤
│ Id (PK)            │
│ UserId (FK, Unique)│
│ HomeCountry        │
│ ShortBio           │
│ Campus             │
│ MajorFieldOfStudy  │
│ YearOfStudy        │
│ Interests          │
│ PreferredGroupSize │
│ MatchingPreference │
│ Languages          │
│ CreatedAt          │
│ UpdatedAt          │
└─────────────────────┘
```

## Table: Users

| Column        | Type          | Constraints           | Description                    |
|--------------|---------------|----------------------|--------------------------------|
| Id           | INT           | PRIMARY KEY, IDENTITY| Auto-incrementing user ID      |
| FirstName    | VARCHAR(100)  | NOT NULL             | User's first name              |
| LastName     | VARCHAR(100)  | NOT NULL             | User's last name               |
| Email        | VARCHAR(255)  | NOT NULL, UNIQUE     | User's email (lowercase)       |
| Password     | VARCHAR(60)   | NOT NULL             | BCrypt hashed password         |
| PhoneCode    | VARCHAR(5)    | NULL                 | Country phone code (e.g., 234) |
| PhoneNumber  | VARCHAR(15)   | NULL                 | Phone number without code      |
| CreatedAt    | TIMESTAMP     | DEFAULT CURRENT_TS   | Account creation timestamp     |
| UpdatedAt    | TIMESTAMP     | NULL                 | Last update timestamp          |

### Indexes
- **Unique Index**: Email

## Table: Profiles

| Column             | Type          | Constraints           | Description                           |
|-------------------|---------------|----------------------|---------------------------------------|
| Id                | INT           | PRIMARY KEY, IDENTITY| Auto-incrementing profile ID          |
| UserId            | INT           | FOREIGN KEY, UNIQUE  | Reference to Users table              |
| HomeCountry       | VARCHAR(100)  | NOT NULL             | Student's home country                |
| ShortBio          | VARCHAR(500)  | NULL                 | Brief biography                       |
| Campus            | VARCHAR(100)  | NOT NULL             | Campus location                       |
| MajorFieldOfStudy | VARCHAR(150)  | NOT NULL             | Academic major                        |
| YearOfStudy       | INT           | NOT NULL             | Year level (1-4)                      |
| Interests         | VARCHAR(500)  | NOT NULL             | Comma-separated interests             |
| PreferredGroupSize| INT           | NOT NULL             | Preferred group size (1-3)            |
| MatchingPreference| INT           | NOT NULL             | Matching preference (1-3)             |
| Languages         | VARCHAR(300)  | NOT NULL             | Comma-separated languages             |
| CreatedAt         | TIMESTAMP     | DEFAULT CURRENT_TS   | Profile creation timestamp            |
| UpdatedAt         | TIMESTAMP     | NULL                 | Last update timestamp                 |

### Indexes
- **Unique Index**: UserId

### Foreign Keys
- **UserId**: References Users(Id) with CASCADE DELETE

## Enum Mappings

### YearOfStudy
```sql
1 = Freshman
2 = Sophomore
3 = Junior
4 = Senior
```

### PreferredGroupSize
```sql
1 = Small (3-4 members)
2 = Medium (5-6 members)
3 = Large (7-8 members)
```

### MatchingPreference
```sql
1 = Same Country
2 = Different Countries
3 = No Preference
```

## Relationships

### User ↔ Profile (One-to-One)
- **Type**: One-to-One
- **Foreign Key**: Profiles.UserId → Users.Id
- **Delete Behavior**: CASCADE (deleting a user deletes their profile)
- **Constraint**: Each user can have only one profile

## Sample Data

### Users Table
```sql
INSERT INTO Users (FirstName, LastName, Email, Password, PhoneCode, PhoneNumber, CreatedAt)
VALUES 
  ('John', 'Doe', 'john.doe@example.com', '$2a$12$...', '234', '8012345678', CURRENT_TIMESTAMP),
  ('Jane', 'Smith', 'jane.smith@example.com', '$2a$12$...', '1', '2025551234', CURRENT_TIMESTAMP);
```

### Profiles Table
```sql
INSERT INTO Profiles (UserId, HomeCountry, ShortBio, Campus, MajorFieldOfStudy, 
                      YearOfStudy, Interests, PreferredGroupSize, MatchingPreference, 
                      Languages, CreatedAt)
VALUES 
  (1, 'Nigeria', 'Tech enthusiast', 'Edinburgh Central', 'Computer Science', 
   3, 'Technology, Music, Photography', 2, 2, 'English, Yoruba', CURRENT_TIMESTAMP),
  (2, 'United States', 'Music lover', 'Edinburgh Central', 'Music Production', 
   2, 'Music, Art, Travel', 1, 3, 'English, Spanish', CURRENT_TIMESTAMP);
```

## Migration Commands

### Create Initial Migration
```bash
dotnet ef migrations add InitialCreate
```

### Add Profile Migration
```bash
dotnet ef migrations add AddProfileEntity
```

### Apply Migrations
```bash
dotnet ef database update
```

### Rollback Migration
```bash
dotnet ef database update PreviousMigrationName
```

### Remove Last Migration (if not applied)
```bash
dotnet ef migrations remove
```

## Database Connection String

Located in `appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=edinburgh_students_db;Username=postgres;Password=postgres"
  }
}
```

## Notes

1. **Password Security**: Passwords are hashed using BCrypt with work factor 12
2. **Email Uniqueness**: Email addresses are stored in lowercase and must be unique
3. **Cascade Delete**: Deleting a user automatically deletes their profile
4. **Timestamp Defaults**: CreatedAt fields use database-level default (CURRENT_TIMESTAMP)
5. **Collections Storage**: Interests and languages are stored as comma-separated values
6. **Enum Storage**: Enums are stored as integers but displayed as descriptive strings in API responses
