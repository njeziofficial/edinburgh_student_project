using Edinburgh_Internation_Students.DTOs.Common;
using Edinburgh_Internation_Students.DTOs.Events;
using Edinburgh_Internation_Students.DTOs.Groups;
using Edinburgh_Internation_Students.DTOs.Notifications;
using Edinburgh_Internation_Students.DTOs.Users;
using Edinburgh_Internation_Students.Extensions;
using Edinburgh_Internation_Students.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Edinburgh_Internation_Students.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UsersController(
    IUserService userService,
    INotificationService notificationService,
    ILogger<UsersController> logger) : ControllerBase
{

    /// <summary>
    /// Get all users
    /// </summary>
    /// <returns>List of all users</returns>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<List<UserDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<List<UserDto>>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var response = await userService.GetAllUsersAsync();
            return StatusCode(response.StatusCode, response);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while retrieving all users");
            var errorResponse = ApiResponse<List<UserDto>>.ErrorResponse(
                "An error occurred while processing your request",
                [ex.Message],
                500
            );
            return StatusCode(500, errorResponse);
        }
    }

    /// <summary>
    /// Get user by ID
    /// </summary>
    /// <param name="id">User ID</param>
    /// <returns>User details</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ApiResponse<UserDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<UserDto>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<UserDto>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetById(int id)
    {
        try
        {
            var response = await userService.GetUserByIdAsync(id);
            return StatusCode(response.StatusCode, response);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while retrieving user {UserId}", id);
            var errorResponse = ApiResponse<UserDto>.ErrorResponse(
                "An error occurred while processing your request",
                new List<string> { ex.Message },
                500
            );
            return StatusCode(500, errorResponse);
        }
    }

    /// <summary>
    /// Update user profile
    /// </summary>
    /// <param name="id">User ID</param>
    /// <param name="request">Update data</param>
    /// <returns>Updated user details</returns>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(ApiResponse<UserDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<UserDto>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<UserDto>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<UserDto>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateUserRequest request)
    {
        try
        {
            // Verify the user is updating their own profile
            var currentUserId = User.GetUserId();
            if (currentUserId != id)
            {
                var forbiddenResponse = ApiResponse<UserDto>.ErrorResponse(
                    "You can only update your own profile",
                    null,
                    403
                );
                return StatusCode(403, forbiddenResponse);
            }

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                var validationResponse = ApiResponse<UserDto>.ErrorResponse(
                    "Validation failed",
                    errors,
                    400
                );
                return BadRequest(validationResponse);
            }

            var response = await userService.UpdateUserAsync(id, request);
            return StatusCode(response.StatusCode, response);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while updating user {UserId}", id);
            var errorResponse = ApiResponse<UserDto>.ErrorResponse(
                "An error occurred while processing your request",
                [ex.Message],
                500
            );
            return StatusCode(500, errorResponse);
        }
    }

    /// <summary>
    /// Delete user account
    /// </summary>
    /// <param name="id">User ID</param>
    /// <returns>Success status</returns>
    /// <summary>
    /// Delete a user account (Admin only)
    /// </summary>
    /// <param name="id">User ID to delete</param>
    /// <returns>Deletion confirmation</returns>
    [HttpDelete("{id}")]
    [Authorize(Policy = "AdminOnly")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var response = await userService.DeleteUserAsync(id);
            return StatusCode(response.StatusCode, response);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while deleting user {UserId}", id);
            var errorResponse = ApiResponse<bool>.ErrorResponse(
                "An error occurred while processing your request",
                new List<string> { ex.Message },
                500
            );
            return StatusCode(500, errorResponse);
        }
    }

    /// <summary>
    /// Complete user onboarding/profile setup
    /// </summary>
    /// <param name="id">User ID</param>
    /// <param name="request">Profile completion data</param>
    /// <returns>Updated user details</returns>
    [HttpPost("{id}/onboarding")]
    [ProducesResponseType(typeof(ApiResponse<UserDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<UserDto>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<UserDto>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<UserDto>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CompleteOnboarding(int id, [FromBody] CompleteProfileRequest request)
    {
        try
        {
            // Verify the user is completing their own onboarding
            var currentUserId = User.GetUserId();
            if (currentUserId != id)
            {
                var forbiddenResponse = ApiResponse<UserDto>.ErrorResponse(
                    "You can only complete your own onboarding",
                    null,
                    403
                );
                return StatusCode(403, forbiddenResponse);
            }

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                var validationResponse = ApiResponse<UserDto>.ErrorResponse(
                    "Validation failed",
                    errors,
                    400
                );
                return BadRequest(validationResponse);
            }

            var response = await userService.CompleteOnboardingAsync(id, request);
            return StatusCode(response.StatusCode, response);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while completing onboarding for user {UserId}", id);
            var errorResponse = ApiResponse<UserDto>.ErrorResponse(
                "An error occurred while processing your request",
                new List<string> { ex.Message },
                500
            );
            return StatusCode(500, errorResponse);
        }
    }

    /// <summary>
    /// Get user's groups
    /// </summary>
    /// <param name="id">User ID</param>
    /// <returns>List of groups the user belongs to with total member count</returns>
    [HttpGet("{id}/groups")]
    [ProducesResponseType(typeof(ApiResponse<UserGroupsResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<UserGroupsResponse>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetUserGroups(int id)
    {
        try
        {
            var response = await userService.GetUserGroupsAsync(id);
            return StatusCode(response.StatusCode, response);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while retrieving groups for user {UserId}", id);
            var errorResponse = ApiResponse<UserGroupsResponse>.ErrorResponse(
                "An error occurred while processing your request",
                new List<string> { ex.Message },
                500
            );
            return StatusCode(500, errorResponse);
        }
    }

    /// <summary>
    /// Get user's events
    /// </summary>
    /// <param name="id">User ID</param>
    /// <returns>List of events the user is attending</returns>
    [HttpGet("{id}/events")]
    [ProducesResponseType(typeof(ApiResponse<List<EventDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<List<EventDto>>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetUserEvents(int id)
    {
        try
        {
            var response = await userService.GetUserEventsAsync(id);
            return StatusCode(response.StatusCode, response);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while retrieving events for user {UserId}", id);
            var errorResponse = ApiResponse<List<EventDto>>.ErrorResponse(
                "An error occurred while processing your request",
                new List<string> { ex.Message },
                500
            );
            return StatusCode(500, errorResponse);
        }
    }

    /// <summary>
    /// Get user's notifications
    /// </summary>
    /// <param name="id">User ID</param>
    /// <returns>List of user notifications</returns>
    [HttpGet("{id}/notifications")]
    [ProducesResponseType(typeof(ApiResponse<List<NotificationDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<List<NotificationDto>>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetUserNotifications(int id)
    {
        try
        {
            var response = await userService.GetUserNotificationsAsync(id);
            return StatusCode(response.StatusCode, response);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while retrieving notifications for user {UserId}", id);
            var errorResponse = ApiResponse<List<NotificationDto>>.ErrorResponse(
                "An error occurred while processing your request",
                [ex.Message],
                500
            );
            return StatusCode(500, errorResponse);
        }
    }

    /// <summary>
    /// Mark all notifications as read for a user
    /// </summary>
    /// <param name="id">User ID</param>
    /// <returns>Success status</returns>
    [HttpPost("{id}/notifications/read-all")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> MarkAllNotificationsAsRead(int id)
    {
        try
        {
            // Verify the user is marking their own notifications
            var currentUserId = User.GetUserId();
            if (currentUserId != id)
            {
                var forbiddenResponse = ApiResponse<bool>.ErrorResponse(
                    "You can only mark your own notifications as read",
                    null,
                    403
                );
                return StatusCode(403, forbiddenResponse);
            }

            var response = await notificationService.MarkAllAsReadAsync(id);
            return StatusCode(response.StatusCode, response);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while marking all notifications as read for user {UserId}", id);
            var errorResponse = ApiResponse<bool>.ErrorResponse(
                "An error occurred while processing your request",
                new List<string> { ex.Message },
                500
            );
            return StatusCode(500, errorResponse);
        }
    }
}
