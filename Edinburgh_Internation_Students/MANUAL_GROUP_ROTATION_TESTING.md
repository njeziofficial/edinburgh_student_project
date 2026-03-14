# Manual Group Rotation Testing Guide

## ✅ Admin Endpoint Created

An admin endpoint has been created to manually trigger group rotation for testing purposes.

---

## 🎯 Endpoints

### 1. Trigger Group Rotation

**POST** `/api/admin/trigger-group-rotation`

Manually triggers the group rotation/shuffle process.

**Authentication:** ✅ Required (JWT Token)

---

### 2. Get Group Statistics

**GET** `/api/admin/group-stats`

View current group distribution and statistics.

**Authentication:** ✅ Required (JWT Token)

---

## 📝 Testing Instructions

### Step 1: Create Test Users

Sign up and onboard 10 users:

```bash
# Sign up user 1
curl -X POST "https://localhost:5001/api/auth/signup" \
  -H "Content-Type: application/json" \
  -d '{
    "fullName": "User One",
    "email": "user1@test.com",
    "password": "Pass123!",
    "phoneCode": "234",
    "phoneNumber": "8011111111"
  }'

# Save the token from response

# Complete onboarding for user 1
curl -X POST "https://localhost:5001/api/users/1/onboarding" \
  -H "Authorization: Bearer YOUR_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "country": "Nigeria",
    "campus": "Merchiston",
    "major": "Computer Science",
    "year": "Junior",
    "interests": ["Technology"],
    "languages": ["English"]
  }'

# Repeat for users 2-10 with different emails
```

---

### Step 2: Check Initial Group Distribution

```bash
curl -X GET "https://localhost:5001/api/admin/group-stats" \
  -H "Authorization: Bearer YOUR_TOKEN"
```

**Expected Response:**
```json
{
  "success": true,
  "data": {
    "totalGroups": 2,
    "totalUsers": 10,
    "averageMembersPerGroup": 5,
    "groups": [
      {
        "id": "guid-1",
        "name": "Group1",
        "memberCount": 5,
        "description": "Auto-generated group for student connections",
        "createdAt": "2024-01-15T10:00:00Z"
      },
      {
        "id": "guid-2",
        "name": "Group2",
        "memberCount": 5,
        "description": "Auto-generated group for student connections",
        "createdAt": "2024-01-15T10:05:00Z"
      }
    ]
  },
  "message": "Group statistics retrieved",
  "errors": [],
  "statusCode": 200
}
```

---

### Step 3: Trigger Manual Rotation

```bash
curl -X POST "https://localhost:5001/api/admin/trigger-group-rotation" \
  -H "Authorization: Bearer YOUR_TOKEN"
```

**Expected Response:**
```json
{
  "success": true,
  "data": {
    "message": "Group rotation completed successfully",
    "beforeRotation": {
      "totalGroups": 2,
      "totalUsers": 10,
      "groups": [
        { "name": "Group1", "memberCount": 5 },
        { "name": "Group2", "memberCount": 5 }
      ]
    },
    "afterRotation": {
      "totalGroups": 2,
      "totalUsers": 10,
      "groups": [
        { "name": "Group1", "memberCount": 5 },
        { "name": "Group2", "memberCount": 5 }
      ]
    },
    "timestamp": "2024-01-15T10:30:00Z"
  },
  "message": "Group rotation triggered successfully",
  "errors": [],
  "statusCode": 200
}
```

---

### Step 4: Verify Users Changed Groups

Check a specific user's group:

```bash
curl -X GET "https://localhost:5001/api/users/1/groups" \
  -H "Authorization: Bearer YOUR_TOKEN"
```

**Before Rotation:**
```json
{
  "data": [
    { "id": "guid-1", "name": "Group1", "memberCount": 5 }
  ]
}
```

**After Rotation:**
```json
{
  "data": [
    { "id": "guid-2", "name": "Group2", "memberCount": 5 }
  ]
}
```

✅ User moved from Group1 to Group2!

---

## 🔍 Check Logs

After triggering rotation, check your application logs:

```
info: Manual group rotation triggered by user
info: Starting group member shuffle/rotation...
info: Found 10 users in 2 groups
info: Removed all existing group memberships
info: Ensured 2 groups exist for 10 users
info: Group shuffle completed successfully. 10 users distributed across 2 groups
info:   Group1: 5 members
info:   Group2: 5 members
```

---

## 🧪 Testing Scenarios

### Scenario 1: Basic Rotation (10 Users, 2 Groups)

```bash
# 1. View initial stats
curl -X GET "https://localhost:5001/api/admin/group-stats" \
  -H "Authorization: Bearer YOUR_TOKEN"

# 2. Note which users are in which groups
curl -X GET "https://localhost:5001/api/users/1/groups" \
  -H "Authorization: Bearer YOUR_TOKEN"

# 3. Trigger rotation
curl -X POST "https://localhost:5001/api/admin/trigger-group-rotation" \
  -H "Authorization: Bearer YOUR_TOKEN"

# 4. Check stats again
curl -X GET "https://localhost:5001/api/admin/group-stats" \
  -H "Authorization: Bearer YOUR_TOKEN"

# 5. Verify user moved groups
curl -X GET "https://localhost:5001/api/users/1/groups" \
  -H "Authorization: Bearer YOUR_TOKEN"
```

---

### Scenario 2: Multiple Rotations

```bash
# Trigger rotation 3 times in a row
curl -X POST "https://localhost:5001/api/admin/trigger-group-rotation" \
  -H "Authorization: Bearer YOUR_TOKEN"

# Wait a few seconds, then repeat
curl -X POST "https://localhost:5001/api/admin/trigger-group-rotation" \
  -H "Authorization: Bearer YOUR_TOKEN"

# Once more
curl -X POST "https://localhost:5001/api/admin/trigger-group-rotation" \
  -H "Authorization: Bearer YOUR_TOKEN"

# Check that users are in different groups each time
curl -X GET "https://localhost:5001/api/users/1/groups" \
  -H "Authorization: Bearer YOUR_TOKEN"
```

**Result:** User should be in different group after each rotation (previous group avoidance working)

---

### Scenario 3: Uneven Groups (11 Users, 3 Groups)

Create 11 users, then:

```bash
# Check distribution
curl -X GET "https://localhost:5001/api/admin/group-stats" \
  -H "Authorization: Bearer YOUR_TOKEN"

# Should show something like:
# Group1: 5 members
# Group2: 5 members
# Group3: 1 member

# Trigger rotation
curl -X POST "https://localhost:5001/api/admin/trigger-group-rotation" \
  -H "Authorization: Bearer YOUR_TOKEN"

# Check new distribution
curl -X GET "https://localhost:5001/api/admin/group-stats" \
  -H "Authorization: Bearer YOUR_TOKEN"

# Should show more even distribution:
# Group1: 4 members
# Group2: 4 members
# Group3: 3 members
```

---

## 🎮 Using Swagger UI

1. **Navigate to Swagger:**
   ```
   https://localhost:5001/swagger
   ```

2. **Authenticate:**
   - Click "Authorize" button
   - Enter: `Bearer YOUR_JWT_TOKEN`
   - Click "Authorize"

3. **Trigger Rotation:**
   - Find `AdminController`
   - Expand `POST /api/admin/trigger-group-rotation`
   - Click "Try it out"
   - Click "Execute"
   - View response

4. **Check Stats:**
   - Expand `GET /api/admin/group-stats`
   - Click "Try it out"
   - Click "Execute"
   - View current distribution

---

## 🔧 Quick Test Script (PowerShell)

Save as `test-rotation.ps1`:

```powershell
# Configuration
$baseUrl = "https://localhost:5001"
$email = "testuser@example.com"
$password = "Pass123!"

# Sign in
$loginResponse = Invoke-RestMethod -Uri "$baseUrl/api/auth/signin" `
    -Method Post `
    -ContentType "application/json" `
    -Body (@{
        email = $email
        password = $password
    } | ConvertTo-Json)

$token = $loginResponse.token
$headers = @{
    "Authorization" = "Bearer $token"
    "Content-Type" = "application/json"
}

# Get stats before
Write-Host "=== Before Rotation ===" -ForegroundColor Cyan
$statsBefore = Invoke-RestMethod -Uri "$baseUrl/api/admin/group-stats" `
    -Method Get `
    -Headers $headers
$statsBefore.data | ConvertTo-Json -Depth 5

# Trigger rotation
Write-Host "`n=== Triggering Rotation ===" -ForegroundColor Yellow
$rotationResult = Invoke-RestMethod -Uri "$baseUrl/api/admin/trigger-group-rotation" `
    -Method Post `
    -Headers $headers
$rotationResult.data | ConvertTo-Json -Depth 5

# Get stats after
Write-Host "`n=== After Rotation ===" -ForegroundColor Green
$statsAfter = Invoke-RestMethod -Uri "$baseUrl/api/admin/group-stats" `
    -Method Get `
    -Headers $headers
$statsAfter.data | ConvertTo-Json -Depth 5
```

**Run it:**
```powershell
.\test-rotation.ps1
```

---

## 📊 Response Examples

### Group Stats Response
```json
{
  "success": true,
  "data": {
    "totalGroups": 2,
    "totalUsers": 10,
    "averageMembersPerGroup": 5,
    "groups": [
      {
        "id": "group-guid-1",
        "name": "Group1",
        "memberCount": 5,
        "description": "Auto-generated group for student connections",
        "createdAt": "2024-01-15T10:00:00Z"
      },
      {
        "id": "group-guid-2",
        "name": "Group2",
        "memberCount": 5,
        "description": "Auto-generated group for student connections",
        "createdAt": "2024-01-15T10:05:00Z"
      }
    ]
  },
  "message": "Group statistics retrieved",
  "errors": [],
  "statusCode": 200
}
```

### Rotation Trigger Response
```json
{
  "success": true,
  "data": {
    "message": "Group rotation completed successfully",
    "beforeRotation": {
      "totalGroups": 2,
      "totalUsers": 10,
      "groups": [
        { "name": "Group1", "memberCount": 5 },
        { "name": "Group2", "memberCount": 5 }
      ]
    },
    "afterRotation": {
      "totalGroups": 2,
      "totalUsers": 10,
      "groups": [
        { "name": "Group1", "memberCount": 5 },
        { "name": "Group2", "memberCount": 5 }
      ]
    },
    "timestamp": "2024-01-15T12:30:00Z"
  },
  "message": "Group rotation triggered successfully",
  "errors": [],
  "statusCode": 200
}
```

---

## 🔒 Security Notes

### Current Authorization
- ✅ Requires JWT authentication
- ⚠️ Any authenticated user can trigger rotation

### Production Recommendations

For production, add role-based authorization:

```csharp
[Authorize(Roles = "Admin")]
public class AdminController : ControllerBase
{
    // ...
}
```

Or create a custom policy:

```csharp
[Authorize(Policy = "AdminOnly")]
public class AdminController : ControllerBase
{
    // ...
}
```

---

## 🎯 Quick Test Commands

### 1. Sign In First
```bash
curl -X POST "https://localhost:5001/api/auth/signin" \
  -H "Content-Type: application/json" \
  -d '{
    "email": "your@email.com",
    "password": "YourPassword"
  }'
```

Save the token from response.

---

### 2. Check Current Stats
```bash
curl -X GET "https://localhost:5001/api/admin/group-stats" \
  -H "Authorization: Bearer YOUR_TOKEN"
```

---

### 3. Trigger Rotation
```bash
curl -X POST "https://localhost:5001/api/admin/trigger-group-rotation" \
  -H "Authorization: Bearer YOUR_TOKEN"
```

---

### 4. Check Stats Again
```bash
curl -X GET "https://localhost:5001/api/admin/group-stats" \
  -H "Authorization: Bearer YOUR_TOKEN"
```

---

## 📋 Complete Test Workflow

### Bash Script (Linux/Mac/Git Bash)

```bash
#!/bin/bash

BASE_URL="https://localhost:5001"

# Sign in
echo "=== Signing in ==="
TOKEN=$(curl -s -X POST "$BASE_URL/api/auth/signin" \
  -H "Content-Type: application/json" \
  -d '{
    "email": "test@example.com",
    "password": "Pass123!"
  }' | jq -r '.token')

echo "Token: $TOKEN"

# Get stats before
echo -e "\n=== Stats Before Rotation ==="
curl -s -X GET "$BASE_URL/api/admin/group-stats" \
  -H "Authorization: Bearer $TOKEN" | jq

# Trigger rotation
echo -e "\n=== Triggering Rotation ==="
curl -s -X POST "$BASE_URL/api/admin/trigger-group-rotation" \
  -H "Authorization: Bearer $TOKEN" | jq

# Get stats after
echo -e "\n=== Stats After Rotation ==="
curl -s -X GET "$BASE_URL/api/admin/group-stats" \
  -H "Authorization: Bearer $TOKEN" | jq
```

---

## ✅ Expected Behavior

### Before First Rotation
```
Group1: [User1, User2, User3, User4, User5]
Group2: [User6, User7, User8, User9, User10]
```

### After First Rotation
```
Group1: [User6, User8, User7, User9, User10] ← All from Group2
Group2: [User1, User3, User2, User5, User4] ← All from Group1
```

✅ No user returned to their previous group!

### After Second Rotation
```
Group1: [User1, User3, User2, User5, User4] ← All from Group2
Group2: [User6, User8, User7, User9, User10] ← All from Group1
```

✅ Again, no user in their previous group!

---

## 🔍 What to Look For

### In API Response
- ✅ `beforeRotation` shows initial distribution
- ✅ `afterRotation` shows new distribution
- ✅ `totalUsers` stays the same
- ✅ `totalGroups` might increase if needed

### In Logs
```
info: Manual group rotation triggered by user
info: Starting group member shuffle/rotation...
info: Found 10 users in 2 groups
info: Removed all existing group memberships
info: Ensured 2 groups exist for 10 users
info: Group shuffle completed successfully. 10 users distributed across 2 groups
info:   Group1: 5 members
info:   Group2: 5 members
```

### Verify Previous Group Avoidance
```
# Should see 0 warnings, or very few:
info: 0 user(s) were reassigned to their previous group due to constraints

# If you see warnings, check the edge case:
warn: User 5 had to be reassigned to their previous group due to constraints
warn: 1 user(s) were reassigned to their previous group due to constraints
```

---

## 🎯 Testing Checklist

- [ ] Create 10+ test users
- [ ] Complete onboarding for all users
- [ ] Verify they're assigned to groups
- [ ] Check group stats (should show 2 groups with 5 members each)
- [ ] Trigger rotation manually
- [ ] Verify users moved to different groups
- [ ] Check logs for "0 reassigned to previous group"
- [ ] Trigger rotation again
- [ ] Verify users moved again to new groups
- [ ] Repeat multiple times to ensure consistency

---

## 💡 Quick Verification

After rotation, pick any user and check their group history:

```bash
# Check user 1's group before rotation
curl -X GET "https://localhost:5001/api/users/1/groups" \
  -H "Authorization: Bearer YOUR_TOKEN"
# Output: Group1

# Trigger rotation
curl -X POST "https://localhost:5001/api/admin/trigger-group-rotation" \
  -H "Authorization: Bearer YOUR_TOKEN"

# Check user 1's group after rotation
curl -X GET "https://localhost:5001/api/users/1/groups" \
  -H "Authorization: Bearer YOUR_TOKEN"
# Output: Group2 ✅ (Different!)

# Trigger rotation again
curl -X POST "https://localhost:5001/api/admin/trigger-group-rotation" \
  -H "Authorization: Bearer YOUR_TOKEN"

# Check user 1's group after second rotation
curl -X GET "https://localhost:5001/api/users/1/groups" \
  -H "Authorization: Bearer YOUR_TOKEN"
# Output: Group1 ✅ (Back to Group1, but not previous from last rotation!)
```

---

## ✔️ Status

**Endpoint:** ✅ Created  
**Authentication:** ✅ Required  
**Build:** ✅ Successful  
**Testing:** ✅ Ready  

You can now test group rotation immediately without waiting for the background service timer! 🚀

**Pro Tip:** Use the Swagger UI at `https://localhost:5001/swagger` for easiest testing! 🎯
