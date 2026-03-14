# Quick Reference: JWT Authentication

## 🚀 Quick Start

### 1. Sign Up / Sign In
Both endpoints return a JWT token in the response:

```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "userId": 1,
  ...
}
```

### 2. Use Token for Protected Requests
Add to Authorization header:
```
Authorization: Bearer YOUR_TOKEN_HERE
```

## 📋 Token Information

- **Expires**: 24 hours from issue
- **Contains**: User ID in `sub` claim
- **Format**: Standard JWT (Header.Payload.Signature)

## 🔧 Common Operations

### Get Token (Sign In)
```bash
curl -X POST /api/auth/signin \
  -H "Content-Type: application/json" \
  -d '{"email":"user@example.com","password":"pass"}'
```

### Use Token
```bash
curl -X GET /api/profile/user/1 \
  -H "Authorization: Bearer YOUR_TOKEN"
```

### Extract User ID (Server-Side)
```csharp
var userId = User.GetUserId();
```

## 🎯 Quick Tips

✅ Store token securely on client  
✅ Include in all authenticated requests  
✅ Handle 401 errors (expired/invalid token)  
✅ Token contains user ID - no need to pass separately  

## 📖 Full Documentation

See `JWT_AUTHENTICATION.md` for complete details.
