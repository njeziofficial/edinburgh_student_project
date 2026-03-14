using Edinburgh_Internation_Students.Data;
using Edinburgh_Internation_Students.DTOs.Common;
using Edinburgh_Internation_Students.DTOs.Events;
using Edinburgh_Internation_Students.DTOs.Groups;
using Edinburgh_Internation_Students.DTOs.Notifications;
using Edinburgh_Internation_Students.DTOs.Users;
using Microsoft.EntityFrameworkCore;

namespace Edinburgh_Internation_Students.Services;

public class UserService(ApplicationDbContext context, IGroupService groupService, ILogger<UserService> logger) : IUserService
{
    public async Task<ApiResponse<List<UserDto>>> GetAllUsersAsync()
    {
        try
        {
            var users = await context.Users
                .Include(u => u.Profile)
                .Select(u => MapToUserDto(u))
                .ToListAsync();

            return ApiResponse<List<UserDto>>.SuccessResponse(users);
        }
        catch (Exception ex)
        {
            return ApiResponse<List<UserDto>>.ErrorResponse(
                "Failed to retrieve users",
                new List<string> { ex.Message },
                500
            );
        }
    }

    public async Task<ApiResponse<UserDto>> GetUserByIdAsync(int userId)
    {
        try
        {
            var user = await context.Users
                .Include(u => u.Profile)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
            {
                return ApiResponse<UserDto>.ErrorResponse("User not found", null, 404);
            }

            var userDto = MapToUserDto(user);
            return ApiResponse<UserDto>.SuccessResponse(userDto);
        }
        catch (Exception ex)
        {
            return ApiResponse<UserDto>.ErrorResponse(
                "Failed to retrieve user",
                new List<string> { ex.Message },
                500
            );
        }
    }

    public async Task<ApiResponse<UserDto>> UpdateUserAsync(int userId, UpdateUserRequest request)
    {
        try
        {
            var user = await context.Users
                .Include(u => u.Profile)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
            {
                return ApiResponse<UserDto>.ErrorResponse("User not found", null, 404);
            }

            // Update user properties
            if (!string.IsNullOrEmpty(request.Name))
            {
                var nameParts = request.Name.Trim().Split(' ', 2);
                user.FirstName = nameParts[0];
                user.LastName = nameParts.Length > 1 ? nameParts[1] : string.Empty;
            }

            user.UpdatedAt = DateTime.UtcNow;

            // Update profile if exists
            if (user.Profile != null)
            {
                if (!string.IsNullOrEmpty(request.Country))
                    user.Profile.HomeCountry = request.Country;

                if (!string.IsNullOrEmpty(request.Campus))
                    user.Profile.Campus = request.Campus;

                if (!string.IsNullOrEmpty(request.Major))
                    user.Profile.MajorFieldOfStudy = request.Major;

                if (request.Bio != null)
                    user.Profile.ShortBio = request.Bio;

                if (request.Interests != null && request.Interests.Count > 0)
                    user.Profile.SetInterests(request.Interests);

                if (request.Languages != null && request.Languages.Count > 0)
                    user.Profile.SetLanguages(request.Languages);

                user.Profile.UpdatedAt = DateTime.UtcNow;
            }

            await context.SaveChangesAsync();

            var userDto = MapToUserDto(user);
            return ApiResponse<UserDto>.SuccessResponse(userDto, "User updated successfully");
        }
        catch (Exception ex)
        {
            return ApiResponse<UserDto>.ErrorResponse(
                "Failed to update user",
                new List<string> { ex.Message },
                500
            );
        }
    }

    public async Task<ApiResponse<bool>> DeleteUserAsync(int userId)
    {
        try
        {
            var user = await context.Users.FindAsync(userId);

            if (user == null)
            {
                return ApiResponse<bool>.ErrorResponse("User not found", null, 404);
            }

            context.Users.Remove(user);
            await context.SaveChangesAsync();

            return ApiResponse<bool>.SuccessResponse(true, "User deleted successfully");
        }
        catch (Exception ex)
        {
            return ApiResponse<bool>.ErrorResponse(
                "Failed to delete user",
                new List<string> { ex.Message },
                500
            );
        }
    }

    public async Task<ApiResponse<UserDto>> CompleteOnboardingAsync(int userId, CompleteProfileRequest request)
    {
        try
        {
            var user = await context.Users
                .Include(u => u.Profile)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
            {
                return ApiResponse<UserDto>.ErrorResponse("User not found", null, 404);
            }

            if (user.Profile == null)
            {
                // Create new profile
                var profile = new Models.Profile
                {
                    UserId = userId,
                    HomeCountry = request.Country,
                    Campus = request.Campus,
                    MajorFieldOfStudy = request.Major,
                    ShortBio = request.Bio,
                    CreatedAt = DateTime.UtcNow
                };

                profile.SetInterests(request.Interests);
                profile.SetLanguages(request.Languages);

                await context.Profiles.AddAsync(profile);
            }
            else
            {
                // Update existing profile
                user.Profile.HomeCountry = request.Country;
                user.Profile.Campus = request.Campus;
                user.Profile.MajorFieldOfStudy = request.Major;
                user.Profile.ShortBio = request.Bio;
                user.Profile.SetInterests(request.Interests);
                user.Profile.SetLanguages(request.Languages);
                user.Profile.UpdatedAt = DateTime.UtcNow;
            }

            await context.SaveChangesAsync();

            // Automatically assign user to a group after onboarding completion
            try
            {
                // Check if user is already in a group
                var existingMembership = await context.GroupMembers
                    .AnyAsync(gm => gm.UserId == userId);

                if (!existingMembership)
                {
                    var groupId = await groupService.FindOrCreateAvailableGroupAsync();
                    var addMemberResult = await groupService.AddMemberToGroupAsync(groupId, userId);

                    if (addMemberResult.Success)
                    {
                        logger.LogInformation("User {UserId} automatically assigned to group {GroupId} after onboarding", 
                            userId, groupId);
                    }
                    else
                    {
                        logger.LogWarning("Failed to assign user {UserId} to group after onboarding: {Error}", 
                            userId, addMemberResult.Message);
                    }
                }
                else
                {
                    logger.LogInformation("User {UserId} already has a group assignment", userId);
                }
            }
            catch (Exception ex)
            {
                // Log error but don't fail onboarding if group assignment fails
                logger.LogError(ex, "Error assigning user {UserId} to group after onboarding", userId);
            }

            var userDto = MapToUserDto(user);
            return ApiResponse<UserDto>.SuccessResponse(userDto, "Onboarding completed successfully");
        }
        catch (Exception ex)
        {
            return ApiResponse<UserDto>.ErrorResponse(
                "Failed to complete onboarding",
                new List<string> { ex.Message },
                500
            );
        }
    }

    public async Task<ApiResponse<UserGroupsResponse>> GetUserGroupsAsync(int userId)
    {
        try
        {
            var onlineThreshold = DateTime.UtcNow.AddMinutes(-10); // Users active in last 10 minutes are considered online

            var groups = await context.GroupMembers
                .Where(gm => gm.UserId == userId)
                .Include(gm => gm.Group)
                    .ThenInclude(g => g.Members)
                        .ThenInclude(m => m.User)
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
                Groups = groups,
                TotalGroups = groups.Count,
                TotalMemberCount = groups.Sum(g => g.MemberCount)
            };

            return ApiResponse<UserGroupsResponse>.SuccessResponse(response);
        }
        catch (Exception ex)
        {
            return ApiResponse<UserGroupsResponse>.ErrorResponse(
                "Failed to retrieve user groups",
                new List<string> { ex.Message },
                500
            );
        }
    }

    public async Task<ApiResponse<List<EventDto>>> GetUserEventsAsync(int userId)
    {
        try
        {
            var events = await context.EventAttendees
                .Where(ea => ea.UserId == userId)
                .Include(ea => ea.Event)
                .Select(ea => new EventDto
                {
                    Id = ea.Event.Id.ToString(),
                    Title = ea.Event.Title,
                    Description = ea.Event.Description,
                    ImageUrl = ea.Event.ImageUrl,
                    StartDate = ea.Event.Date.Add(ea.Event.Time),
                    Location = ea.Event.Location,
                    IsVirtual = false,
                    MaxAttendees = ea.Event.MaxAttendees,
                    Status = ea.Event.IsCancelled ? "Cancelled" : "Upcoming",
                    OrganizerId = ea.Event.OrganizerId.ToString(),
                    AttendeeCount = ea.Event.Attendees.Count,
                    CreatedAt = ea.Event.CreatedAt
                })
                .ToListAsync();

            return ApiResponse<List<EventDto>>.SuccessResponse(events);
        }
        catch (Exception ex)
        {
            return ApiResponse<List<EventDto>>.ErrorResponse(
                "Failed to retrieve user events",
                new List<string> { ex.Message },
                500
            );
        }
    }

    public async Task<ApiResponse<List<NotificationDto>>> GetUserNotificationsAsync(int userId)
    {
        try
        {
            var notifications = await context.Notifications
                .Where(n => n.UserId == userId)
                .OrderByDescending(n => n.CreatedAt)
                .Select(n => new NotificationDto
                {
                    Id = n.Id.ToString(),
                    UserId = n.UserId.ToString(),
                    Type = n.Type.ToString(),
                    Title = n.Title,
                    Message = n.Message,
                    IsRead = n.IsRead,
                    Data = n.ReferenceId.ToString(),
                    CreatedAt = n.CreatedAt
                })
                .ToListAsync();

            return ApiResponse<List<NotificationDto>>.SuccessResponse(notifications);
        }
        catch (Exception ex)
        {
            return ApiResponse<List<NotificationDto>>.ErrorResponse(
                "Failed to retrieve notifications",
                new List<string> { ex.Message },
                500
            );
        }
    }

    private static UserDto MapToUserDto(Models.User user)
    {
        return new UserDto
        {
            Id = user.Id.ToString(),
            Email = user.Email,
            Name = $"{user.FirstName} {user.LastName}".Trim(),
            Country = user.Profile?.HomeCountry,
            Campus = user.Profile?.Campus,
            Major = user.Profile?.MajorFieldOfStudy,
            Year = user.Profile?.YearOfStudy.ToString(),
            Bio = user.Profile?.ShortBio,
            Interests = user.Profile?.GetInterests() ?? new List<string>(),
            Languages = user.Profile?.GetLanguages() ?? new List<string>(),
            IsOnline = false,
            CreatedAt = user.CreatedAt
        };
    }
}
