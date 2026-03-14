using Edinburgh_Internation_Students.DTOs.Common;
using Edinburgh_Internation_Students.DTOs.Events;
using Edinburgh_Internation_Students.DTOs.Groups;
using Edinburgh_Internation_Students.DTOs.Notifications;
using Edinburgh_Internation_Students.DTOs.Users;

namespace Edinburgh_Internation_Students.Services;

public interface IUserService
{
    Task<ApiResponse<List<UserDto>>> GetAllUsersAsync();
    Task<ApiResponse<UserDto>> GetUserByIdAsync(int userId);
    Task<ApiResponse<UserDto>> UpdateUserAsync(int userId, UpdateUserRequest request);
    Task<ApiResponse<bool>> DeleteUserAsync(int userId);
    Task<ApiResponse<UserDto>> CompleteOnboardingAsync(int userId, CompleteProfileRequest request);
    Task<ApiResponse<UserGroupsResponse>> GetUserGroupsAsync(int userId);
    Task<ApiResponse<List<EventDto>>> GetUserEventsAsync(int userId);
    Task<ApiResponse<List<NotificationDto>>> GetUserNotificationsAsync(int userId);
}
