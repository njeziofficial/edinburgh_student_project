# Using Bearer Token in Swagger

## ✅ JWT Bearer Authentication Configured in Swagger

Swagger UI now supports JWT Bearer token authentication with an "Authorize" button.

---

## 🚀 How to Use

### Step 1: Start Your Application

```bash
dotnet run
```

Navigate to: `https://localhost:5001/swagger` (or your configured port)

---

### Step 2: Get Your JWT Token

#### Option A: Use Swagger to Sign In

1. Find **AuthController**
2. Expand **POST /api/auth/login** (or `/api/auth/register`)
3. Click **"Try it out"**
4. Enter credentials:
   ```json
   {
     "email": "your@email.com",
     "password": "YourPassword"
   }
   ```
5. Click **"Execute"**
6. Copy the `token` value from the response

#### Option B: Use cURL

```bash
curl -X POST "https://localhost:5001/api/auth/login" \
  -H "Content-Type: application/json" \
  -d '{
    "email": "your@email.com",
    "password": "YourPassword"
  }'
```

Copy the token from the response.

---

### Step 3: Authorize in Swagger

1. Click the **🔓 Authorize** button at the top right of Swagger UI
2. In the "Value" field, enter your token in this format:
   ```
   Bearer YOUR_JWT_TOKEN_HERE
   ```
   
   **OR** just paste the token without "Bearer " prefix:
   ```
   YOUR_JWT_TOKEN_HERE
   ```
   
   Swagger will automatically add "Bearer " prefix.

3. Click **"Authorize"**
4. Click **"Close"**

---

### Step 4: Test Protected Endpoints

Now you can test any endpoint that requires authentication:

1. Find **UsersController**
2. Expand **GET /api/users**
3. Click **"Try it out"**
4. Click **"Execute"**
5. ✅ You should get a successful response!

The 🔓 icon will change to 🔒, indicating you're authenticated.

---

## 📝 Visual Guide

### Before Authorization
```
🔓 Authorize  ← Click this button

[UsersController]
  GET /api/users  🔓  ← Unlocked (will fail 401)
```

### After Authorization
```
🔒 Authorize  ← Shows you're authenticated

[UsersController]
  GET /api/users  🔒  ← Locked (will succeed with your identity)
```

---

## 🎯 Testing Workflow

### 1. Register a New User
```
POST /api/auth/register
{
  "firstName": "John",
  "lastName": "Doe",
  "email": "john@test.com",
  "password": "Pass123!",
  "phoneCode": "234",
  "phoneNumber": "8012345678"
}
```

**Copy the token from response**

---

### 2. Authorize in Swagger
- Click 🔓 Authorize
- Paste token
- Click Authorize

---

### 3. Complete Onboarding
```
POST /api/users/1/onboarding
{
  "country": "Nigeria",
  "campus": "Merchiston",
  "major": "Computer Science",
  "year": "Junior",
  "interests": ["Technology", "Music"],
  "languages": ["English"]
}
```

---

### 4. Test Protected Endpoints
- GET /api/users - View all users
- GET /api/users/1/groups - View your groups
- POST /api/admin/trigger-group-rotation - Trigger rotation
- POST /api/auth/logout - Logout

All should work with your token! ✅

---

## 🔒 What Was Configured

### Swagger Security Definition
```csharp
options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
{
    Name = "Authorization",
    Type = SecuritySchemeType.Http,
    Scheme = "bearer",
    BearerFormat = "JWT",
    In = ParameterLocation.Header,
    Description = "JWT Authorization header using the Bearer scheme."
});
```

### Security Requirement
```csharp
options.AddSecurityRequirement(new OpenApiSecurityRequirement
{
    {
        new OpenApiSecurityScheme
        {
            Reference = new OpenApiReference
            {
                Type = ReferenceType.SecurityScheme,
                Id = "Bearer"
            }
        },
        Array.Empty<string>()
    }
});
```

---

## 🎨 Swagger UI Features

### Enhanced API Documentation
- ✅ API Title: "Edinburgh International Students API"
- ✅ Version: v1
- ✅ Description
- ✅ Contact information
- ✅ Bearer token support

### Authentication Indicators
- 🔓 Unlocked padlock = Not authenticated
- 🔒 Locked padlock = Authenticated
- Green "Authorize" button when authenticated

---

## 🧪 Quick Test

### Full Test Flow in Swagger:

1. **Register** (POST /api/auth/register)
   ```json
   {
     "firstName": "Test",
     "lastName": "User",
     "email": "test@example.com",
     "password": "TestPass123!",
     "phoneCode": "234",
     "phoneNumber": "8012345678"
   }
   ```

2. **Copy token** from response

3. **Authorize** (🔓 button)
   - Paste token
   - Click Authorize

4. **Complete Onboarding** (POST /api/users/{id}/onboarding)
   ```json
   {
     "country": "Nigeria",
     "campus": "Merchiston",
     "major": "Computer Science",
     "year": "Junior",
     "interests": ["Technology"],
     "languages": ["English"]
   }
   ```

5. **Check Groups** (GET /api/users/{id}/groups)
   - Should see Group1

6. **Trigger Rotation** (POST /api/admin/trigger-group-rotation)
   - View rotation results

7. **Check Groups Again** (GET /api/users/{id}/groups)
   - Should see different group

All in Swagger UI! 🎉

---

## 💡 Tips

### Token Expiry
- Tokens expire after 24 hours (1440 minutes)
- If you get 401 errors, get a new token and re-authorize
- Use refresh token endpoint to get new token without re-login

### Logout
- After logout, your refresh tokens are revoked
- You'll need to login again to get a new token
- Remember to authorize again in Swagger

### Multiple Sessions
- You can have multiple browser tabs with different tokens
- Each tab maintains its own authorization state
- Useful for testing different users

---

## 🔍 Troubleshooting

### 401 Unauthorized Error
- ✅ Click Authorize and enter your token
- ✅ Ensure token is not expired
- ✅ Format should be: `Bearer {token}` or just `{token}`

### 403 Forbidden Error
- ✅ You're authenticated but don't have permission
- ✅ Some endpoints require specific user (e.g., update own profile)

### Token Not Working
- ✅ Re-login to get fresh token
- ✅ Check token hasn't expired
- ✅ Verify JWT settings in appsettings.json

---

## ✔️ Status

**Swagger Auth:** ✅ Configured  
**Authorize Button:** ✅ Visible  
**Bearer Token:** ✅ Supported  
**Build:** ✅ Successful  

You can now use the 🔓 Authorize button in Swagger to test all authenticated endpoints! 🎉🔒
