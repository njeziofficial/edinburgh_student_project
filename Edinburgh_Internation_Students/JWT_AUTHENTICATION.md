# JWT Authentication Documentation

## Overview

The Edinburgh International Students API uses JWT (JSON Web Tokens) for authentication. Upon successful sign up or sign in, users receive a JWT token that must be included in subsequent authenticated requests.

## JWT Configuration

### Settings (appsettings.json)

```json
{
  "JwtSettings": {
    "SecretKey": "YourSuperSecretKeyThatIsAtLeast32CharactersLongForSecurity!",
    "Issuer": "EdinburghStudentAPI",
    "Audience": "EdinburghStudentApp",
    "ExpiryInMinutes": 1440
  }
}
```

### Configuration Details

- **SecretKey**: Secret key used to sign and verify tokens (minimum 32 characters)
- **Issuer**: The entity that issues the token (API identifier)
- **Audience**: The intended recipient of the token (App identifier)
- **ExpiryInMinutes**: Token lifetime (1440 minutes = 24 hours)

> ⚠️ **Security Note**: Change the `SecretKey` to a strong, unique value in production and store it securely (e.g., Azure Key Vault, AWS Secrets Manager).

## Token Generation

Tokens are automatically generated when users:
1. **Sign Up** - `POST /api/auth/signup`
2. **Sign In** - `POST /api/auth/signin`

### Example Response

```json
{
  "userId": 1,
  "email": "john.doe@example.com",
  "firstName": "John",
  "lastName": "Doe",
  "phoneCode": "234",
  "phoneNumber": "8012345678",
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxIiwiZW1haWwiOiJqb2huLmRvZUBleGFtcGxlLmNvbSIsImdpdmVuX25hbWUiOiJKb2huIiwiZmFtaWx5X25hbWUiOiJEb2UiLCJqdGkiOiJhYmMxMjMiLCJpYXQiOiIxNzA5NTY3ODkwIiwibmFtZWlkIjoiMSIsImVtYWlsIjoiam9obi5kb2VAZXhhbXBsZS5jb20iLCJ1bmlxdWVfbmFtZSI6IkpvaG4gRG9lIiwiZXhwIjoxNzA5NjU0MjkwLCJpc3MiOiJFZGluYnVyZ2hTdHVkZW50QVBJIiwiYXVkIjoiRWRpbmJ1cmdoU3R1ZGVudEFwcCJ9.signature",
  "message": "Sign in successful"
}
```

## JWT Claims

The token contains the following claims:

| Claim | Type | Description | Example |
|-------|------|-------------|---------|
| **sub** | string | User ID (Subject) | "1" |
| **email** | string | User's email | "john.doe@example.com" |
| **given_name** | string | User's first name | "John" |
| **family_name** | string | User's last name | "Doe" |
| **jti** | string | JWT ID (unique identifier) | "abc123..." |
| **iat** | string | Issued at (Unix timestamp) | "1709567890" |
| **nameid** | string | User ID (Name Identifier) | "1" |
| **name** | string | Full name | "John Doe" |
| **exp** | number | Expiration (Unix timestamp) | 1709654290 |
| **iss** | string | Issuer | "EdinburghStudentAPI" |
| **aud** | string | Audience | "EdinburghStudentApp" |

## Using JWT Tokens

### 1. Store the Token

After receiving a token, store it securely:

**Web Applications:**
```javascript
// Store in localStorage or sessionStorage
localStorage.setItem('jwt_token', response.token);
```

**Mobile Applications:**
- Use secure storage mechanisms (Keychain on iOS, Keystore on Android)

### 2. Include Token in Requests

Add the token to the `Authorization` header with the `Bearer` scheme:

```
Authorization: Bearer YOUR_JWT_TOKEN_HERE
```

### Example Requests

#### cURL
```bash
curl -X GET https://localhost:5001/api/profile/user/1 \
  -H "Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
```

#### JavaScript (Fetch API)
```javascript
const token = localStorage.getItem('jwt_token');

fetch('https://localhost:5001/api/profile/user/1', {
  method: 'GET',
  headers: {
    'Authorization': `Bearer ${token}`,
    'Content-Type': 'application/json'
  }
})
.then(response => response.json())
.then(data => console.log(data));
```

#### C# (HttpClient)
```csharp
var client = new HttpClient();
client.DefaultRequestHeaders.Authorization = 
    new AuthenticationHeaderValue("Bearer", token);

var response = await client.GetAsync("https://localhost:5001/api/profile/user/1");
```

## Extracting User Information

### From Token Claims (Server-Side)

```csharp
// In a controller
var userId = User.GetUserId(); // Extension method
var email = User.GetUserEmail();
var name = User.GetUserName();
```

### Decoding Token (Client-Side)

You can decode the JWT to read claims without verifying the signature:

**JavaScript:**
```javascript
function parseJwt(token) {
    const base64Url = token.split('.')[1];
    const base64 = base64Url.replace(/-/g, '+').replace(/_/g, '/');
    const jsonPayload = decodeURIComponent(
        atob(base64).split('').map(c => {
            return '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2);
        }).join('')
    );
    return JSON.parse(jsonPayload);
}

const token = localStorage.getItem('jwt_token');
const decoded = parseJwt(token);
console.log('User ID:', decoded.sub);
console.log('Email:', decoded.email);
```

## Token Validation

### Automatic Validation

The API automatically validates tokens on protected endpoints:
- ✅ Signature verification
- ✅ Expiration check
- ✅ Issuer validation
- ✅ Audience validation

### Validation Errors

| Status Code | Error | Reason |
|-------------|-------|--------|
| **401 Unauthorized** | Missing token | No Authorization header provided |
| **401 Unauthorized** | Invalid token | Token signature invalid or malformed |
| **401 Unauthorized** | Expired token | Token has passed expiration time |
| **401 Unauthorized** | Invalid issuer/audience | Token not issued by this API |

## Token Lifecycle

```
┌─────────────┐
│  Sign Up /  │
│   Sign In   │
└──────┬──────┘
       │
       ▼
┌─────────────────┐
│  Token Issued   │
│  (Valid 24hrs)  │
└──────┬──────────┘
       │
       ▼
┌─────────────────┐
│ Make Requests   │
│ (with token)    │
└──────┬──────────┘
       │
       ▼
    ┌──────┐
    │Valid?│
    └──┬───┘
       │
   ┌───┴────┐
   │        │
 Yes       No
   │        │
   ▼        ▼
┌─────┐  ┌──────────┐
│Allow│  │  Reject  │
└─────┘  │  (401)   │
         └──────────┘
```

## Token Expiry and Refresh

### Token Expiry
- **Duration**: 24 hours (1440 minutes)
- **Behavior**: After expiration, API returns 401 Unauthorized

### Handling Expiration

**Option 1: Re-authenticate**
```javascript
// Detect 401 and redirect to login
if (response.status === 401) {
    localStorage.removeItem('jwt_token');
    window.location.href = '/login';
}
```

**Option 2: Check expiry before request**
```javascript
function isTokenExpired(token) {
    const decoded = parseJwt(token);
    return decoded.exp * 1000 < Date.now();
}

if (isTokenExpired(token)) {
    // Prompt user to sign in again
    window.location.href = '/login';
}
```

## Security Best Practices

### ✅ Do's
- ✅ Use HTTPS in production
- ✅ Store tokens securely (avoid localStorage for sensitive apps)
- ✅ Validate tokens on every protected request
- ✅ Use strong secret keys (minimum 32 characters)
- ✅ Set reasonable expiration times
- ✅ Implement token refresh mechanism for better UX

### ❌ Don'ts
- ❌ Don't store tokens in cookies without proper security flags
- ❌ Don't expose secret keys in client-side code
- ❌ Don't use weak secret keys
- ❌ Don't send tokens in URL parameters
- ❌ Don't store tokens in plain text on the server

## Testing JWT Authentication

### 1. Sign Up and Get Token
```bash
curl -X POST https://localhost:5001/api/auth/signup \
  -H "Content-Type: application/json" \
  -d '{
    "fullName": "Test User",
    "email": "test@example.com",
    "password": "TestPass123!",
    "phoneCode": "234",
    "phoneNumber": "8012345678"
  }'
```

### 2. Copy the Token from Response
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
}
```

### 3. Use Token for Protected Requests
```bash
TOKEN="eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."

curl -X GET https://localhost:5001/api/profile/user/1 \
  -H "Authorization: Bearer $TOKEN"
```

## Troubleshooting

### Issue: "401 Unauthorized"
**Causes:**
- Token not included in request
- Token expired
- Token invalid or malformed
- Wrong secret key used

**Solution:**
- Verify token is included in Authorization header
- Check token expiration
- Sign in again to get a new token

### Issue: "Token validation failed"
**Causes:**
- Secret key mismatch
- Token issued by different system
- Token tampered with

**Solution:**
- Verify JWT settings match between client and server
- Ensure using latest token from API

### Issue: "User ID not found in token"
**Causes:**
- Token doesn't contain `sub` or `nameid` claim
- Token from different authentication system

**Solution:**
- Ensure token generated by this API
- Check JWT claims structure

## Environment-Specific Configuration

### Development
```json
{
  "JwtSettings": {
    "SecretKey": "DevSecretKey32CharactersMinimum!",
    "Issuer": "EdinburghStudentAPI-Dev",
    "Audience": "EdinburghStudentApp-Dev",
    "ExpiryInMinutes": 1440
  }
}
```

### Production
```json
{
  "JwtSettings": {
    "SecretKey": "${JWT_SECRET_KEY}",
    "Issuer": "EdinburghStudentAPI",
    "Audience": "EdinburghStudentApp",
    "ExpiryInMinutes": 720
  }
}
```

> Use environment variables or secret management services for production secrets.
