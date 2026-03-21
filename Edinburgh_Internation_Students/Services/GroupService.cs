using Edinburgh_Internation_Students.Data;
using Edinburgh_Internation_Students.DTOs.Common;
using Edinburgh_Internation_Students.DTOs.Groups;
using Edinburgh_Internation_Students.Models;
using Microsoft.EntityFrameworkCore;

namespace Edinburgh_Internation_Students.Services;

public class GroupService(ApplicationDbContext context, ILogger<GroupService> logger, IConfiguration configuration) : IGroupService
{
    private readonly int _maxGroupSize = configuration.GetValue<int>("GroupRotationSettings:MaxGroupSize");
    public async Task<ApiResponse<GroupDto>> CreateGroupAsync(string name, string? description = null)
    {
        try
        {
            var group = new Group
            {
                Name = name,
                Description = description,
                ExpiresAt = DateTime.UtcNow.AddMonths(3),
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            await context.Groups.AddAsync(group);
            await context.SaveChangesAsync();

            var groupDto = new GroupDto
            {
                Id = group.Id.ToString(),
                Name = group.Name,
                Description = group.Description,
                Category = "General",
                IsPrivate = false,
                MemberCount = 0,
                OnlineCount = 0,
                CreatedAt = group.CreatedAt
            };

            return ApiResponse<GroupDto>.SuccessResponse(groupDto, "Group created successfully", 201);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error creating group {GroupName}", name);
            return ApiResponse<GroupDto>.ErrorResponse(
                "Failed to create group",
                new List<string> { ex.Message },
                500
            );
        }
    }

    public async Task<ApiResponse<GroupDto>> GetGroupByIdAsync(Guid groupId)
    {
        try
        {
            var onlineThreshold = DateTime.UtcNow.AddMinutes(-10);

            var group = await context.Groups
                .Include(g => g.Members)
                    .ThenInclude(m => m.User)
                .FirstOrDefaultAsync(g => g.Id == groupId);

            if (group == null)
            {
                return ApiResponse<GroupDto>.ErrorResponse("Group not found", null, 404);
            }

            var groupDto = new GroupDto
            {
                Id = group.Id.ToString(),
                Name = group.Name,
                Description = group.Description,
                Category = "General",
                IsPrivate = false,
                MemberCount = group.Members.Count,
                OnlineCount = group.Members.Count(m => m.User.LastActive.HasValue && m.User.LastActive >= onlineThreshold),
                CreatedAt = group.CreatedAt
            };

            return ApiResponse<GroupDto>.SuccessResponse(groupDto);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error retrieving group {GroupId}", groupId);
            return ApiResponse<GroupDto>.ErrorResponse(
                "Failed to retrieve group",
                new List<string> { ex.Message },
                500
            );
        }
    }

    public async Task<ApiResponse<List<GroupDto>>> GetAllGroupsAsync()
    {
        try
        {
            var onlineThreshold = DateTime.UtcNow.AddMinutes(-10);

            var groups = await context.Groups
                .Include(g => g.Members)
                    .ThenInclude(m => m.User)
                .Where(g => g.IsActive)
                .Select(g => new GroupDto
                {
                    Id = g.Id.ToString(),
                    Name = g.Name,
                    Description = g.Description,
                    Category = "General",
                    IsPrivate = false,
                    MemberCount = g.Members.Count,
                    OnlineCount = g.Members.Count(m => m.User.LastActive.HasValue && m.User.LastActive >= onlineThreshold),
                    CreatedAt = g.CreatedAt
                })
                .ToListAsync();

            return ApiResponse<List<GroupDto>>.SuccessResponse(groups);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error retrieving all groups");
            return ApiResponse<List<GroupDto>>.ErrorResponse(
                "Failed to retrieve groups",
                [ex.Message],
                500
            );
        }
    }

    public async Task<ApiResponse<GroupDto>> UpdateGroupAsync(Guid groupId, UpdateGroupRequest request)
    {
        try
        {
            var group = await context.Groups.FindAsync(groupId);

            if (group == null)
            {
                return ApiResponse<GroupDto>.ErrorResponse("Group not found", null, 404);
            }

            // Update only provided fields
            if (!string.IsNullOrWhiteSpace(request.Name))
            {
                group.Name = request.Name;
            }

            if (request.Description != null)
            {
                group.Description = request.Description;
            }

            await context.SaveChangesAsync();

            var onlineThreshold = DateTime.UtcNow.AddMinutes(-10);
            var memberCount = await context.GroupMembers.CountAsync(gm => gm.GroupId == groupId);
            var onlineCount = await context.GroupMembers
                .Include(gm => gm.User)
                .Where(gm => gm.GroupId == groupId && gm.User.LastActive.HasValue && gm.User.LastActive >= onlineThreshold)
                .CountAsync();

            var groupDto = new GroupDto
            {
                Id = group.Id.ToString(),
                Name = group.Name,
                Description = group.Description,
                Category = request.Category ?? "General",
                IsPrivate = request.IsPrivate ?? false,
                MemberCount = memberCount,
                OnlineCount = onlineCount,
                CreatedAt = group.CreatedAt
            };

            return ApiResponse<GroupDto>.SuccessResponse(groupDto, "Group updated successfully");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error updating group {GroupId}", groupId);
            return ApiResponse<GroupDto>.ErrorResponse(
                "Failed to update group",
                new List<string> { ex.Message },
                500
            );
        }
    }

    public async Task<ApiResponse<bool>> DeleteGroupAsync(Guid groupId)
    {
        try
        {
            var group = await context.Groups
                .Include(g => g.Members)
                .FirstOrDefaultAsync(g => g.Id == groupId);

            if (group == null)
            {
                return ApiResponse<bool>.ErrorResponse("Group not found", null, 404);
            }

            // Remove all members first
            context.GroupMembers.RemoveRange(group.Members);

            // Remove the group
            context.Groups.Remove(group);

            await context.SaveChangesAsync();

            logger.LogInformation("Group {GroupId} deleted successfully", groupId);
            return ApiResponse<bool>.SuccessResponse(true, "Group deleted successfully");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error deleting group {GroupId}", groupId);
            return ApiResponse<bool>.ErrorResponse(
                "Failed to delete group",
                new List<string> { ex.Message },
                500
            );
        }
    }

    public async Task<ApiResponse<List<GroupMemberDto>>> GetGroupMembersAsync(Guid groupId)
    {
        try
        {
            var group = await context.Groups.FindAsync(groupId);

            if (group == null)
            {
                return ApiResponse<List<GroupMemberDto>>.ErrorResponse("Group not found", null, 404);
            }

            var onlineThreshold = DateTime.UtcNow.AddMinutes(-10);

            var members = await context.GroupMembers
                .Include(gm => gm.User)
                    .ThenInclude(u => u.Profile)
                .Where(gm => gm.GroupId == groupId)
                .Select(gm => new GroupMemberDto
                {
                    Id = gm.Id.ToString(),
                    GroupId = groupId.ToString(),
                    UserId = gm.UserId.ToString(),
                    Role = "Member",
                    JoinedAt = gm.JoinedAt,
                    User = new DTOs.Users.UserSummaryDto
                    {
                        Id = gm.UserId.ToString(),
                        Name = $"{gm.User.FirstName} {gm.User.LastName}".Trim(),
                        AvatarUrl = null,
                        Country = gm.User.Profile != null ? gm.User.Profile.HomeCountry : null,
                        IsOnline = gm.User.LastActive.HasValue && gm.User.LastActive >= onlineThreshold
                    }
                })
                .OrderBy(gm => gm.JoinedAt)
                .ToListAsync();

            return ApiResponse<List<GroupMemberDto>>.SuccessResponse(members);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error retrieving members for group {GroupId}", groupId);
            return ApiResponse<List<GroupMemberDto>>.ErrorResponse(
                "Failed to retrieve group members",
                new List<string> { ex.Message },
                500
            );
        }
    }

    public async Task<ApiResponse<UserGroupsResponse>> GetUserGroupsAsync(int userId)
    {
        try
        {
            var user = await context.Users.FindAsync(userId);

            if (user == null)
            {
                return ApiResponse<UserGroupsResponse>.ErrorResponse("User not found", null, 404);
            }

            var onlineThreshold = DateTime.UtcNow.AddMinutes(-10);

            var userGroups = await context.GroupMembers
                .Include(gm => gm.Group)
                    .ThenInclude(g => g.Members)
                        .ThenInclude(m => m.User)
                .Where(gm => gm.UserId == userId && gm.Group.IsActive)
                .Select(gm => new GroupDto
                {
                    Id = gm.Group.Id.ToString(),
                    Name = gm.Group.Name,
                    Description = gm.Group.Description,
                    Category = "General",
                    IsPrivate = false,
                    MemberCount = gm.Group.Members.Count,
                    OnlineCount = gm.Group.Members.Count(m => m.User.LastActive.HasValue && m.User.LastActive >= onlineThreshold),
                    CreatedAt = gm.Group.CreatedAt
                })
                .ToListAsync();

            var response = new UserGroupsResponse
            {
                Groups = userGroups,
                TotalGroups = userGroups.Count,
                TotalMemberCount = userGroups.Sum(g => g.MemberCount)
            };

            return ApiResponse<UserGroupsResponse>.SuccessResponse(response);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error retrieving groups for user {UserId}", userId);
            return ApiResponse<UserGroupsResponse>.ErrorResponse(
                "Failed to retrieve user groups",
                new List<string> { ex.Message },
                500
            );
        }
    }

    public async Task<ApiResponse<GroupMemberDto>> AddMemberToGroupAsync(Guid groupId, int userId)
    {
        try
        {
            var group = await context.Groups
                .Include(g => g.Members)
                .FirstOrDefaultAsync(g => g.Id == groupId);

            if (group == null)
            {
                return ApiResponse<GroupMemberDto>.ErrorResponse("Group not found", null, 404);
            }

            if (group.Members.Count >= _maxGroupSize)
            {
                return ApiResponse<GroupMemberDto>.ErrorResponse(
                    "Group is full",
                    new List<string> { $"Maximum group size is {_maxGroupSize}" },
                    400
                );
            }

            var existingMember = await context.GroupMembers
                .FirstOrDefaultAsync(gm => gm.GroupId == groupId && gm.UserId == userId);

            if (existingMember != null)
            {
                return ApiResponse<GroupMemberDto>.ErrorResponse(
                    "User is already a member of this group",
                    null,
                    400
                );
            }

            var member = new GroupMember
            {
                GroupId = groupId,
                UserId = userId,
                JoinedAt = DateTime.UtcNow
            };

            await context.GroupMembers.AddAsync(member);
            await context.SaveChangesAsync();

            var user = await context.Users.FindAsync(userId);

            var memberDto = new GroupMemberDto
            {
                Id = member.Id.ToString(),
                GroupId = groupId.ToString(),
                UserId = userId.ToString(),
                Role = "Member",
                JoinedAt = member.JoinedAt,
                User = new DTOs.Users.UserSummaryDto
                {
                    Id = user!.Id.ToString(),
                    Name = $"{user.FirstName} {user.LastName}".Trim(),
                    AvatarUrl = null,
                    Country = null,
                    IsOnline = false
                }
            };

            return ApiResponse<GroupMemberDto>.SuccessResponse(memberDto, "User added to group successfully", 201);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error adding user {UserId} to group {GroupId}", userId, groupId);
            return ApiResponse<GroupMemberDto>.ErrorResponse(
                "Failed to add member to group",
                new List<string> { ex.Message },
                500
            );
        }
    }

    public async Task<ApiResponse<bool>> RemoveMemberFromGroupAsync(Guid groupId, int userId)
    {
        try
        {
            var member = await context.GroupMembers
                .FirstOrDefaultAsync(gm => gm.GroupId == groupId && gm.UserId == userId);

            if (member == null)
            {
                return ApiResponse<bool>.ErrorResponse("Member not found in group", null, 404);
            }

            context.GroupMembers.Remove(member);
            await context.SaveChangesAsync();

            return ApiResponse<bool>.SuccessResponse(true, "Member removed from group successfully");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error removing user {UserId} from group {GroupId}", userId, groupId);
            return ApiResponse<bool>.ErrorResponse(
                "Failed to remove member from group",
                [ex.Message],
                500
            );
        }
    }

    public async Task<Guid> FindOrCreateAvailableGroupAsync()
    {
        try
        {
            // Find an active group that has less than MaxGroupSize members
            var availableGroup = await context.Groups
                .Include(g => g.Members)
                .Where(g => g.IsActive && g.Members.Count < _maxGroupSize)
                .OrderBy(g => g.CreatedAt)
                .FirstOrDefaultAsync();

            if (availableGroup != null)
            {
                return availableGroup.Id;
            }

            // No available group found, create a new one
            var groupCount = await context.Groups.CountAsync();
            var newGroupName = $"Group{groupCount + 1}";

            var newGroup = new Group
            {
                Name = newGroupName,
                Description = $"Auto-generated group for student connections",
                ExpiresAt = DateTime.UtcNow.AddMonths(3),
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            await context.Groups.AddAsync(newGroup);
            await context.SaveChangesAsync();

            logger.LogInformation("Created new group: {GroupName} with ID: {GroupId}", newGroupName, newGroup.Id);

            return newGroup.Id;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error finding or creating available group");
            throw;
        }
    }

    public async Task<int> GetGroupMemberCountAsync(Guid groupId)
    {
        return await context.GroupMembers.CountAsync(gm => gm.GroupId == groupId);
    }

    public async Task<List<GroupDto>> GetActiveGroupsWithMembersAsync()
    {
        var onlineThreshold = DateTime.UtcNow.AddMinutes(-10);

        return await context.Groups
            .Include(g => g.Members)
                .ThenInclude(m => m.User)
            .Where(g => g.IsActive && g.Members.Count > 0)
            .Select(g => new GroupDto
            {
                Id = g.Id.ToString(),
                Name = g.Name,
                Description = g.Description,
                Category = "General",
                IsPrivate = false,
                MemberCount = g.Members.Count,
                OnlineCount = g.Members.Count(m => m.User.LastActive.HasValue && m.User.LastActive >= onlineThreshold),
                CreatedAt = g.CreatedAt
            })
            .ToListAsync();
    }

    public async Task ShuffleGroupMembersAsync()
    {
        try
        {
            logger.LogInformation("Starting group member shuffle/rotation...");

            // Get all active groups with their members
            var groups = await context.Groups
                .Include(g => g.Members)
                .Where(g => g.IsActive)
                .ToListAsync();

            if (groups.Count < 2)
            {
                logger.LogInformation("Not enough groups for rotation. Need at least 2 groups.");
                return;
            }

            // Get all user IDs from all groups and track their current groups
            var userPreviousGroups = new Dictionary<int, Guid>();
            foreach (var group in groups)
            {
                foreach (var member in group.Members)
                {
                    userPreviousGroups[member.UserId] = group.Id;
                }
            }

            var allUserIds = userPreviousGroups.Keys.ToList();

            if (allUserIds.Count < 2)
            {
                logger.LogInformation("Not enough users for rotation. Need at least 2 users.");
                return;
            }

            logger.LogInformation("Found {UserCount} users in {GroupCount} groups", allUserIds.Count, groups.Count);

            // Remove all current group memberships
            var allMemberships = await context.GroupMembers.ToListAsync();
            context.GroupMembers.RemoveRange(allMemberships);
            await context.SaveChangesAsync();

            logger.LogInformation("Removed all existing group memberships");

            // Shuffle users randomly
            var random = new Random();
            var shuffledUsers = allUserIds.OrderBy(x => random.Next()).ToList();

            // Calculate how many groups we need
            var numberOfGroups = (int)Math.Ceiling((double)allUserIds.Count / _maxGroupSize);

            // Ensure we have enough groups
            var existingGroupCount = groups.Count;
            for (int i = existingGroupCount; i < numberOfGroups; i++)
            {
                var newGroup = new Group
                {
                    Name = $"Group{i + 1}",
                    Description = "Auto-generated group for student connections",
                    ExpiresAt = DateTime.UtcNow.AddMonths(3),
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                };

                await context.Groups.AddAsync(newGroup);
                groups.Add(newGroup);
            }

            await context.SaveChangesAsync();

            logger.LogInformation("Ensured {GroupCount} groups exist for {UserCount} users", numberOfGroups, allUserIds.Count);

            // Track group fill count for even distribution
            var groupFillCount = new Dictionary<Guid, int>();
            foreach (var group in groups.Take(numberOfGroups))
            {
                groupFillCount[group.Id] = 0;
            }

            // Distribute users, avoiding their previous groups
            var newMemberships = new List<GroupMember>();
            var usersReassignedToPreviousGroup = 0;

            foreach (var userId in shuffledUsers)
            {
                var previousGroupId = userPreviousGroups.ContainsKey(userId) ? userPreviousGroups[userId] : Guid.Empty;
                var availableGroups = groups.Take(numberOfGroups).ToList();

                // Try to find a group that is not the user's previous group
                Group? targetGroup = null;

                // First attempt: Find a non-previous group that's not full
                var nonPreviousGroups = availableGroups
                    .Where(g => g.Id != previousGroupId && groupFillCount[g.Id] < _maxGroupSize)
                    .OrderBy(g => groupFillCount[g.Id]) // Prefer groups with fewer members
                    .ToList();

                if (nonPreviousGroups.Any())
                {
                    targetGroup = nonPreviousGroups.First();
                }
                else
                {
                    // Fallback: If no other option (user must return to previous group)
                    // This can happen with very few users or specific group configurations
                    targetGroup = availableGroups
                        .Where(g => groupFillCount[g.Id] < _maxGroupSize)
                        .OrderBy(g => groupFillCount[g.Id])
                        .FirstOrDefault();

                    if (targetGroup != null && targetGroup.Id == previousGroupId)
                    {
                        usersReassignedToPreviousGroup++;
                        logger.LogWarning("User {UserId} had to be reassigned to their previous group {GroupId} due to constraints", 
                            userId, targetGroup.Id);
                    }
                }

                if (targetGroup == null)
                {
                    logger.LogError("Failed to find target group for user {UserId}", userId);
                    continue;
                }

                var membership = new GroupMember
                {
                    GroupId = targetGroup.Id,
                    UserId = userId,
                    JoinedAt = DateTime.UtcNow
                };

                newMemberships.Add(membership);
                groupFillCount[targetGroup.Id]++;
            }

            await context.GroupMembers.AddRangeAsync(newMemberships);
            await context.SaveChangesAsync();

            if (usersReassignedToPreviousGroup > 0)
            {
                logger.LogWarning("{Count} user(s) were reassigned to their previous group due to constraints", 
                    usersReassignedToPreviousGroup);
            }

            logger.LogInformation("Group shuffle completed successfully. {UserCount} users distributed across {GroupCount} groups", 
                shuffledUsers.Count, numberOfGroups);

            // Log group distribution
            foreach (var group in groups.Take(numberOfGroups))
            {
                var count = groupFillCount[group.Id];
                logger.LogInformation("  {GroupName}: {MemberCount} members", group.Name, count);
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred during group member shuffle");
            throw;
        }
    }
}
