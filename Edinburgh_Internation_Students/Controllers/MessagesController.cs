using Edinburgh_Internation_Students.DTOs.Common;
using Edinburgh_Internation_Students.DTOs.Messages;
using Edinburgh_Internation_Students.Extensions;
using Edinburgh_Internation_Students.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Edinburgh_Internation_Students.Controllers;

[ApiController]
[Route("api/groups/{groupId}/messages")]
[Authorize]
public class MessagesController(IMessageService messageService, ILogger<MessagesController> logger) : ControllerBase
{

    /// <summary>
    /// Get messages for a group with pagination
    /// </summary>
    /// <param name="groupId">Group ID</param>
    /// <param name="limit">Maximum number of messages to return (default: 50)</param>
    /// <param name="offset">Number of messages to skip (default: 0)</param>
    /// <returns>List of messages</returns>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<List<MessageDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<List<MessageDto>>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<List<MessageDto>>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<List<MessageDto>>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetGroupMessages(
        Guid groupId,
        [FromQuery] int limit = 50,
        [FromQuery] int offset = 0)
    {
        try
        {
            var userId = User.GetUserId();
            var response = await messageService.GetGroupMessagesAsync(groupId, userId, limit, offset);
            return StatusCode(response.StatusCode, response);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while retrieving messages for group {GroupId}", groupId);
            var errorResponse = ApiResponse<List<MessageDto>>.ErrorResponse(
                "An error occurred while processing your request",
                [ex.Message],
                500
            );
            return StatusCode(500, errorResponse);
        }
    }

    /// <summary>
    /// Send a message to a group
    /// </summary>
    /// <param name="groupId">Group ID</param>
    /// <param name="request">Message content</param>
    /// <returns>Created message</returns>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<MessageDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<MessageDto>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<MessageDto>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<MessageDto>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<MessageDto>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> SendMessage(Guid groupId, [FromBody] SendMessageRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                var validationResponse = ApiResponse<MessageDto>.ErrorResponse(
                    "Validation failed",
                    errors,
                    400
                );
                return BadRequest(validationResponse);
            }

            var userId = User.GetUserId();
            var response = await messageService.SendMessageAsync(groupId, userId, request);
            return StatusCode(response.StatusCode, response);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while sending message to group {GroupId}", groupId);
            var errorResponse = ApiResponse<MessageDto>.ErrorResponse(
                "An error occurred while processing your request",
                [ex.Message],
                500
            );
            return StatusCode(500, errorResponse);
        }
    }

    /// <summary>
    /// Add a reaction to a message
    /// </summary>
    /// <param name="groupId">Group ID</param>
    /// <param name="messageId">Message ID</param>
    /// <param name="request">Reaction emoji</param>
    /// <returns>Success status</returns>
    [HttpPost("{messageId}/reactions")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> AddReaction(
        Guid groupId,
        Guid messageId,
        [FromBody] ReactionRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                var validationResponse = ApiResponse<bool>.ErrorResponse(
                    "Validation failed",
                    errors,
                    400
                );
                return BadRequest(validationResponse);
            }

            var userId = User.GetUserId();
            var response = await messageService.AddReactionAsync(groupId, messageId, userId, request.Emoji);
            return StatusCode(response.StatusCode, response);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while adding reaction to message {MessageId}", messageId);
            var errorResponse = ApiResponse<bool>.ErrorResponse(
                "An error occurred while processing your request",
                [ex.Message],
                500
            );
            return StatusCode(500, errorResponse);
        }
    }

    /// <summary>
    /// Remove a reaction from a message
    /// </summary>
    /// <param name="groupId">Group ID</param>
    /// <param name="messageId">Message ID</param>
    /// <param name="emoji">Emoji to remove (URL encoded)</param>
    /// <returns>Success status</returns>
    [HttpDelete("{messageId}/reactions/{emoji}")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> RemoveReaction(
        Guid groupId,
        Guid messageId,
        string emoji)
    {
        try
        {
            // Decode the emoji parameter
            var decodedEmoji = Uri.UnescapeDataString(emoji);

            var userId = User.GetUserId();
            var response = await messageService.RemoveReactionAsync(groupId, messageId, userId, decodedEmoji);
            return StatusCode(response.StatusCode, response);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while removing reaction from message {MessageId}", messageId);
            var errorResponse = ApiResponse<bool>.ErrorResponse(
                "An error occurred while processing your request",
                [ex.Message],
                500
            );
            return StatusCode(500, errorResponse);
        }
    }

    /// <summary>
    /// Delete a message
    /// </summary>
    /// <param name="groupId">Group ID</param>
    /// <param name="messageId">Message ID</param>
    /// <returns>Success status</returns>
    [HttpDelete("{messageId}")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteMessage(Guid groupId, Guid messageId)
    {
        try
        {
            var userId = User.GetUserId();
            var response = await messageService.DeleteMessageAsync(groupId, messageId, userId);
            return StatusCode(response.StatusCode, response);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while deleting message {MessageId}", messageId);
            var errorResponse = ApiResponse<bool>.ErrorResponse(
                "An error occurred while processing your request",
                [ex.Message],
                500
            );
            return StatusCode(500, errorResponse);
        }
    }
}
