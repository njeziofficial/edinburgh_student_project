# Profile API Documentation

## Overview

The Profile entity stores detailed information about students including their academic background, preferences, interests, and matching criteria for group formation.

## Profile Properties

### Basic Information
- **Home Country**: Student's country of origin (Required, max 100 chars)
- **Short Bio**: Brief description about the student (Optional, max 500 chars)
- **Campus**: Student's campus location (Required, max 100 chars)
- **Major/Field of Study**: Student's academic major (Required, max 150 chars)

### Academic Details
- **Year of Study**: Current academic year
  - `1` - Freshman
  - `2` - Sophomore
  - `3` - Junior
  - `4` - Senior

### Preferences
- **Preferred Group Size**: Ideal group size for collaboration
  - `1` - Small (3-4 members)
  - `2` - Medium (5-6 members)
  - `3` - Large (7-8 members)

- **Matching Preference**: Preference for group diversity
  - `1` - Same Country
  - `2` - Different Countries
  - `3` - No Preference

### Collections
- **Interests**: List of interests (e.g., Technology, Music, Photography, Sports, Art, Travel)
- **Languages**: List of languages spoken (e.g., English, Spanish, Mandarin, French)

## API Endpoints

### 1. Create Profile

**POST** `/api/profile/user/{userId}`

Creates a new profile for a user.

#### Request Body
```json
{
  "homeCountry": "Nigeria",
  "shortBio": "Passionate about technology and innovation. Love connecting with people from diverse backgrounds.",
  "campus": "Edinburgh Central Campus",
  "majorFieldOfStudy": "Computer Science",
  "yearOfStudy": 3,
  "interests": ["Technology", "Music", "Photography", "Travel"],
  "preferredGroupSize": 2,
  "matchingPreference": 2,
  "languages": ["English", "Yoruba", "Spanish"]
}
```

#### Success Response (201 Created)
```json
{
  "id": 1,
  "userId": 1,
  "homeCountry": "Nigeria",
  "shortBio": "Passionate about technology and innovation. Love connecting with people from diverse backgrounds.",
  "campus": "Edinburgh Central Campus",
  "majorFieldOfStudy": "Computer Science",
  "yearOfStudy": "Junior",
  "interests": ["Technology", "Music", "Photography", "Travel"],
  "preferredGroupSize": "Medium (5-6 members)",
  "matchingPreference": "Different Countries",
  "languages": ["English", "Yoruba", "Spanish"],
  "createdAt": "2024-01-15T10:30:00Z",
  "updatedAt": null
}
```

#### Error Responses
- **400 Bad Request**: Validation error or profile already exists
- **404 Not Found**: User not found
- **500 Internal Server Error**: Server error

---

### 2. Get Profile by User ID

**GET** `/api/profile/user/{userId}`

Retrieves a user's profile.

#### Success Response (200 OK)
```json
{
  "id": 1,
  "userId": 1,
  "homeCountry": "Nigeria",
  "shortBio": "Passionate about technology and innovation.",
  "campus": "Edinburgh Central Campus",
  "majorFieldOfStudy": "Computer Science",
  "yearOfStudy": "Junior",
  "interests": ["Technology", "Music", "Photography", "Travel"],
  "preferredGroupSize": "Medium (5-6 members)",
  "matchingPreference": "Different Countries",
  "languages": ["English", "Yoruba", "Spanish"],
  "createdAt": "2024-01-15T10:30:00Z",
  "updatedAt": null
}
```

#### Error Responses
- **404 Not Found**: Profile not found
- **500 Internal Server Error**: Server error

---

### 3. Update Profile

**PUT** `/api/profile/user/{userId}`

Updates an existing profile. All fields are optional - only provided fields will be updated.

#### Request Body (All fields optional)
```json
{
  "shortBio": "Updated bio with new interests and goals.",
  "yearOfStudy": 4,
  "interests": ["Technology", "Music", "Sports", "Reading"],
  "matchingPreference": 3
}
```

#### Success Response (200 OK)
```json
{
  "id": 1,
  "userId": 1,
  "homeCountry": "Nigeria",
  "shortBio": "Updated bio with new interests and goals.",
  "campus": "Edinburgh Central Campus",
  "majorFieldOfStudy": "Computer Science",
  "yearOfStudy": "Senior",
  "interests": ["Technology", "Music", "Sports", "Reading"],
  "preferredGroupSize": "Medium (5-6 members)",
  "matchingPreference": "No Preference",
  "languages": ["English", "Yoruba", "Spanish"],
  "createdAt": "2024-01-15T10:30:00Z",
  "updatedAt": "2024-02-20T14:45:00Z"
}
```

#### Error Responses
- **400 Bad Request**: Validation error
- **404 Not Found**: Profile not found
- **500 Internal Server Error**: Server error

---

### 4. Delete Profile

**DELETE** `/api/profile/user/{userId}`

Deletes a user's profile.

#### Success Response (204 No Content)
No content returned on successful deletion.

#### Error Responses
- **404 Not Found**: Profile not found
- **500 Internal Server Error**: Server error

---

## Enum Values Reference

### Year of Study
```
1 = Freshman
2 = Sophomore
3 = Junior
4 = Senior
```

### Preferred Group Size
```
1 = Small (3-4 members)
2 = Medium (5-6 members)
3 = Large (7-8 members)
```

### Matching Preference
```
1 = Same Country
2 = Different Countries
3 = No Preference
```

## Common Interests Examples
- Technology
- Music
- Photography
- Sports
- Art
- Travel
- Reading
- Cooking
- Gaming
- Fitness
- Movies
- Fashion

## Common Languages Examples
- English
- Spanish
- Mandarin
- French
- Arabic
- German
- Portuguese
- Japanese
- Russian
- Italian
- Korean
- Dutch

## cURL Examples

### Create Profile
```bash
curl -X POST https://localhost:5001/api/profile/user/1 \
  -H "Content-Type: application/json" \
  -d '{
    "homeCountry": "Nigeria",
    "shortBio": "Tech enthusiast and music lover",
    "campus": "Edinburgh Central Campus",
    "majorFieldOfStudy": "Computer Science",
    "yearOfStudy": 3,
    "interests": ["Technology", "Music", "Photography"],
    "preferredGroupSize": 2,
    "matchingPreference": 2,
    "languages": ["English", "Yoruba"]
  }'
```

### Get Profile
```bash
curl -X GET https://localhost:5001/api/profile/user/1
```

### Update Profile
```bash
curl -X PUT https://localhost:5001/api/profile/user/1 \
  -H "Content-Type: application/json" \
  -d '{
    "yearOfStudy": 4,
    "interests": ["Technology", "Music", "Sports"]
  }'
```

### Delete Profile
```bash
curl -X DELETE https://localhost:5001/api/profile/user/1
```

## Complete User Registration Flow

1. **Sign Up**: Create user account
   ```
   POST /api/auth/signup
   ```

2. **Create Profile**: Add student profile details
   ```
   POST /api/profile/user/{userId}
   ```

3. **Update Profile**: Modify profile as needed
   ```
   PUT /api/profile/user/{userId}
   ```

4. **Get Profile**: View profile information
   ```
   GET /api/profile/user/{userId}
   ```

## Notes

- A user can only have one profile
- Profile is automatically deleted when user is deleted (cascade delete)
- Interests and languages are stored as comma-separated values
- All enum values are stored as integers in the database but returned as descriptive strings in responses
