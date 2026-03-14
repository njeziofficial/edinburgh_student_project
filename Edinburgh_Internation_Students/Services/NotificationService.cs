using Edinburgh_Internation_Students.Data;
using Edinburgh_Internation_Students.DTOs.Common;
using Edinburgh_Internation_Students.DTOs.Notifications;
using Microsoft.EntityFrameworkCore;

namespace Edinburgh_Internation_Students.Services;

public class NotificationService(ApplicationDbContext context) : INotificationService
{
    public async Task<ApiResponse<List<NotificationDto>>> GetAllNotificationsAsync(int userId)
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
                    ReadAt = null,
                    Data = n.ReferenceId.HasValue ? n.ReferenceId.Value.ToString() : null,
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

    public async Task<ApiResponse<NotificationDto>> MarkAsReadAsync(Guid notificationId, int userId)
    {
        try
        {
            var notification = await context.Notifications
                .FirstOrDefaultAsync(n => n.Id == notificationId);

            if (notification == null)
            {
                return ApiResponse<NotificationDto>.ErrorResponse(
                    "Notification not found",
                    null,
                    404
                );
            }

            // Verify the notification belongs to the user
            if (notification.UserId != userId)
            {
                return ApiResponse<NotificationDto>.ErrorResponse(
                    "You don't have permission to modify this notification",
                    null,
                    403
                );
            }

            notification.IsRead = true;
            await context.SaveChangesAsync();

            var notificationDto = new NotificationDto
            {
                Id = notification.Id.ToString(),
                UserId = notification.UserId.ToString(),
                Type = notification.Type.ToString(),
                Title = notification.Title,
                Message = notification.Message,
                IsRead = notification.IsRead,
                ReadAt = null,
                Data = notification.ReferenceId.HasValue ? notification.ReferenceId.Value.ToString() : null,
                CreatedAt = notification.CreatedAt
            };

            return ApiResponse<NotificationDto>.SuccessResponse(
                notificationDto,
                "Notification marked as read"
            );
        }
        catch (Exception ex)
        {
            return ApiResponse<NotificationDto>.ErrorResponse(
                "Failed to mark notification as read",
                new List<string> { ex.Message },
                500
            );
        }
    }

    public async Task<ApiResponse<bool>> MarkAllAsReadAsync(int userId)
    {
        try
        {
            var unreadNotifications = await context.Notifications
                .Where(n => n.UserId == userId && !n.IsRead)
                .ToListAsync();

            if (unreadNotifications.Count == 0)
            {
                return ApiResponse<bool>.SuccessResponse(
                    true,
                    "No unread notifications to mark"
                );
            }

            foreach (var notification in unreadNotifications)
            {
                notification.IsRead = true;
            }

            await context.SaveChangesAsync();

            return ApiResponse<bool>.SuccessResponse(
                true,
                $"{unreadNotifications.Count} notification(s) marked as read"
            );
        }
        catch (Exception ex)
        {
            return ApiResponse<bool>.ErrorResponse(
                "Failed to mark all notifications as read",
                new List<string> { ex.Message },
                500
            );
        }
    }

    public async Task<ApiResponse<bool>> DeleteNotificationAsync(Guid notificationId, int userId)
    {
        try
        {
            var notification = await context.Notifications
                .FirstOrDefaultAsync(n => n.Id == notificationId);

            if (notification == null)
            {
                return ApiResponse<bool>.ErrorResponse(
                    "Notification not found",
                    null,
                    404
                );
            }

            // Verify the notification belongs to the user
            if (notification.UserId != userId)
            {
                return ApiResponse<bool>.ErrorResponse(
                    "You don't have permission to delete this notification",
                    null,
                    403
                );
            }

            context.Notifications.Remove(notification);
            await context.SaveChangesAsync();

            return ApiResponse<bool>.SuccessResponse(
                true,
                "Notification deleted successfully"
            );
        }
        catch (Exception ex)
        {
            return ApiResponse<bool>.ErrorResponse(
                "Failed to delete notification",
                new List<string> { ex.Message },
                500
            );
        }
    }
}
