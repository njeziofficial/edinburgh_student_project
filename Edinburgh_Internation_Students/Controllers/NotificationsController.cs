using Edinburgh_Internation_Students.DTOs.Common;
using Edinburgh_Internation_Students.DTOs.Notifications;
using Edinburgh_Internation_Students.Extensions;
using Edinburgh_Internation_Students.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Edinburgh_Internation_Students.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class NotificationsController(
    INotificationService notificationService,
    ILogger<NotificationsController> logger) : ControllerBase
{

    /// <summary>
    /// Mark a notification as read
    /// </summary>
    /// <param name="id">Notification ID</param>
    /// <returns>Updated notification</returns>
    [HttpPatch("{id}")]
    [ProducesResponseType(typeof(ApiResponse<NotificationDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<NotificationDto>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<NotificationDto>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<NotificationDto>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> MarkAsRead(Guid id)
    {
        try
        {
            var userId = User.GetUserId();
            var response = await notificationService.MarkAsReadAsync(id, userId);
            return StatusCode(response.StatusCode, response);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while marking notification {NotificationId} as read", id);
            var errorResponse = ApiResponse<NotificationDto>.ErrorResponse(
                "An error occurred while processing your request",
                [ex.Message],
                500
            );
            return StatusCode(500, errorResponse);
        }
    }

    /// <summary>
    /// Delete a notification (Admin only)
    /// </summary>
    /// <param name="id">Notification ID</param>
    /// <returns>Success status</returns>
    [HttpDelete("{id}")]
    [Authorize(Policy = "AdminOnly")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            var userId = User.GetUserId();
            var response = await notificationService.DeleteNotificationAsync(id, userId);
            return StatusCode(response.StatusCode, response);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while deleting notification {NotificationId}", id);
            var errorResponse = ApiResponse<bool>.ErrorResponse(
                "An error occurred while processing your request",
                [ex.Message],
                500
            );
            return StatusCode(500, errorResponse);
        }
    }
}
