using Edinburgh_Internation_Students.DTOs.Common;
using Edinburgh_Internation_Students.DTOs.Notifications;

namespace Edinburgh_Internation_Students.Services;

public interface INotificationService
{
    Task<ApiResponse<List<NotificationDto>>> GetAllNotificationsAsync(int userId);
    Task<ApiResponse<NotificationDto>> MarkAsReadAsync(Guid notificationId, int userId);
    Task<ApiResponse<bool>> MarkAllAsReadAsync(int userId);
    Task<ApiResponse<bool>> DeleteNotificationAsync(Guid notificationId, int userId);
}
