# Group Rotation Quick Reference

## 🎯 Quick Overview

**What It Does:**
- Automatically assigns new users to groups (max 5 members)
- Creates groups sequentially (Group1, Group2, Group3...)
- Periodically shuffles members between groups

---

## ⚙️ Configuration

**File:** `appsettings.json`

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

---

## 🔧 Common Configurations

### Daily Rotation
```json
"IntervalInHours": 24, "IntervalInMinutes": 0
```

### Twice Daily
```json
"IntervalInHours": 12, "IntervalInMinutes": 0
```

### Weekly
```json
"IntervalInHours": 168, "IntervalInMinutes": 0
```

### For Testing (Every 5 minutes)
```json
"IntervalInHours": 0, 
"IntervalInMinutes": 5,
"RunOnStartup": true
```

### Disable Rotation
```json
"Enabled": false
```

---

## 📊 How Groups Work

### Group Assignment
- **Max Size**: 5 members per group
- **Naming**: Group1, Group2, Group3... (sequential)
- **Creation**: Automatic when needed
- **Assignment**: Users added to first available group

### Rotation Process
1. Collect all users from all groups
2. Shuffle randomly
3. Redistribute evenly across groups
4. Create new groups if needed

---

## 📝 Key Points

✅ Users assigned to group immediately on signup  
✅ Groups fill up before creating new ones  
✅ Rotation runs in background (doesn't block API)  
✅ Signup succeeds even if group assignment fails  
✅ Minimum 2 groups and 2 users needed for rotation  

---

## 🔍 Checking Logs

**Look for:**
```
info: Group rotation background service started. Running every 24:00:00
info: Created new group: Group1 with ID: {guid}
info: Starting group member rotation at {time}
info: Group shuffle completed successfully. {n} users distributed across {n} groups
```

---

## 🚀 Quick Start

1. ✅ Configuration already in `appsettings.json`
2. ✅ Services registered in `Program.cs`
3. ✅ Background service running
4. ✅ Ready to use!

Just run your application and sign up users - they'll be automatically grouped! 🎉
