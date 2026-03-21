using Edinburgh_Internation_Students.DTOs.Common;
using Edinburgh_Internation_Students.DTOs.Groups;
using Edinburgh_Internation_Students.Extensions;
using Edinburgh_Internation_Students.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Edinburgh_Internation_Students.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class GroupsController(IGroupService groupService, ILogger<GroupsController> logger) : ControllerBase
{

    /// <summary>
    /// Get all groups
    /// </summary>
    /// <returns>List of all active groups</returns>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<List<GroupDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<List<GroupDto>>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAllGroups()
    {
        try
        {
            var response = await groupService.GetAllGroupsAsync();
            return StatusCode(response.StatusCode, response);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while retrieving all groups");
            var errorResponse = ApiResponse<List<GroupDto>>.ErrorResponse(
                "An error occurred while processing your request",
                new List<string> { ex.Message },
                500
            );
            return StatusCode(500, errorResponse);
        }
    }

    /// <summary>
    /// Get a specific group by ID
    /// </summary>
    /// <param name="id">Group ID</param>
    /// <returns>Group details</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ApiResponse<GroupDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<GroupDto>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<GroupDto>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetGroupById(Guid id)
    {
        try
        {
            var response = await groupService.GetGroupByIdAsync(id);
            return StatusCode(response.StatusCode, response);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while retrieving group {GroupId}", id);
            var errorResponse = ApiResponse<GroupDto>.ErrorResponse(
                "An error occurred while processing your request",
                new List<string> { ex.Message },
                500
            );
            return StatusCode(500, errorResponse);
        }
    }

    /// <summary>
    /// Create a new group
    /// </summary>
    /// <param name="request">Group creation data</param>
    /// <returns>Created group details</returns>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<GroupDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<GroupDto>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<GroupDto>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateGroup([FromBody] CreateGroupRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                var validationResponse = ApiResponse<GroupDto>.ErrorResponse(
                    "Validation failed",
                    errors,
                    400
                );
                return BadRequest(validationResponse);
            }

            var response = await groupService.CreateGroupAsync(request.Name, request.Description);

            if (response.Success)
            {
                return StatusCode(201, response);
            }

            return StatusCode(response.StatusCode, response);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while creating group");
            var errorResponse = ApiResponse<GroupDto>.ErrorResponse(
                "An error occurred while processing your request",
                new List<string> { ex.Message },
                500
            );
            return StatusCode(500, errorResponse);
        }
    }

    /// <summary>
    /// Update a group
    /// </summary>
    /// <param name="id">Group ID</param>
    /// <param name="request">Group update data</param>
    /// <returns>Updated group details</returns>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(ApiResponse<GroupDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<GroupDto>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<GroupDto>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<GroupDto>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UpdateGroup(Guid id, [FromBody] UpdateGroupRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                var validationResponse = ApiResponse<GroupDto>.ErrorResponse(
                    "Validation failed",
                    errors,
                    400
                );
                return BadRequest(validationResponse);
            }

            var response = await groupService.UpdateGroupAsync(id, request);
            return StatusCode(response.StatusCode, response);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while updating group {GroupId}", id);
            var errorResponse = ApiResponse<GroupDto>.ErrorResponse(
                "An error occurred while processing your request",
                new List<string> { ex.Message },
                500
            );
            return StatusCode(500, errorResponse);
        }
    }

    /// <summary>
    /// Delete a group (Admin only)
    /// </summary>
    /// <param name="id">Group ID</param>
    /// <returns>Deletion confirmation</returns>
    [HttpDelete("{id}")]
    [Authorize(Policy = "AdminOnly")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteGroup(Guid id)
    {
        try
        {
            var response = await groupService.DeleteGroupAsync(id);
            return StatusCode(response.StatusCode, response);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while deleting group {GroupId}", id);
            var errorResponse = ApiResponse<bool>.ErrorResponse(
                "An error occurred while processing your request",
                new List<string> { ex.Message },
                500
            );
            return StatusCode(500, errorResponse);
        }
    }

    /// <summary>
    /// Get all members of a group
    /// </summary>
    /// <param name="id">Group ID</param>
    /// <returns>List of group members</returns>
    [HttpGet("{id}/members")]
    [ProducesResponseType(typeof(ApiResponse<List<GroupMemberDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<List<GroupMemberDto>>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<List<GroupMemberDto>>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetGroupMembers(Guid id)
    {
        try
        {
            var response = await groupService.GetGroupMembersAsync(id);
            return StatusCode(response.StatusCode, response);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while retrieving members for group {GroupId}", id);
            var errorResponse = ApiResponse<List<GroupMemberDto>>.ErrorResponse(
                "An error occurred while processing your request",
                new List<string> { ex.Message },
                500
            );
            return StatusCode(500, errorResponse);
        }
    }

    /// <summary>
    /// Add a member to a group
    /// </summary>
    /// <param name="id">Group ID</param>
    /// <param name="userId">User ID to add</param>
    /// <returns>Added member details</returns>
    [HttpPost("{id}/members/{userId}")]
    [ProducesResponseType(typeof(ApiResponse<GroupMemberDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<GroupMemberDto>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<GroupMemberDto>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<GroupMemberDto>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> AddMemberToGroup(Guid id, int userId)
    {
        try
        {
            var response = await groupService.AddMemberToGroupAsync(id, userId);
            return StatusCode(response.StatusCode, response);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while adding user {UserId} to group {GroupId}", userId, id);
            var errorResponse = ApiResponse<GroupMemberDto>.ErrorResponse(
                "An error occurred while processing your request",
                new List<string> { ex.Message },
                500
            );
            return StatusCode(500, errorResponse);
        }
    }

    /// <summary>
    /// Remove a member from a group
    /// </summary>
    /// <param name="id">Group ID</param>
    /// <param name="userId">User ID to remove</param>
    /// <returns>Removal confirmation</returns>
    [HttpDelete("{id}/members/{userId}")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> RemoveMemberFromGroup(Guid id, int userId)
    {
        try
        {
            // Verify the user is removing themselves or is an admin
            var currentUserId = User.GetUserId();
            var isAdmin = User.IsInRole("Admin");

            if (currentUserId != userId && !isAdmin)
            {
                var forbiddenResponse = ApiResponse<bool>.ErrorResponse(
                    "You can only remove yourself from a group",
                    null,
                    403
                );
                return StatusCode(403, forbiddenResponse);
            }

            var response = await groupService.RemoveMemberFromGroupAsync(id, userId);
            return StatusCode(response.StatusCode, response);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while removing user {UserId} from group {GroupId}", userId, id);
            var errorResponse = ApiResponse<bool>.ErrorResponse(
                "An error occurred while processing your request",
                new List<string> { ex.Message },
                500
            );
            return StatusCode(500, errorResponse);
        }
    }

    /// <summary>
    /// Get all groups for the current user
    /// </summary>
    /// <returns>User's groups</returns>
    [HttpGet("my-groups")]
    [ProducesResponseType(typeof(ApiResponse<UserGroupsResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<UserGroupsResponse>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<UserGroupsResponse>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetMyGroups()
    {
        try
        {
            var userId = User.GetUserId();
            var response = await groupService.GetUserGroupsAsync(userId);
            return StatusCode(response.StatusCode, response);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while retrieving groups for current user");
            var errorResponse = ApiResponse<UserGroupsResponse>.ErrorResponse(
                "An error occurred while processing your request",
                new List<string> { ex.Message },
                500
            );
            return StatusCode(500, errorResponse);
        }
    }

    /// <summary>
    /// Get all groups for a specific user
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <returns>User's groups</returns>
    [HttpGet("users/{userId}/groups")]
    [ProducesResponseType(typeof(ApiResponse<UserGroupsResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<UserGroupsResponse>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<UserGroupsResponse>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetUserGroups(int userId)
    {
        try
        {
            var response = await groupService.GetUserGroupsAsync(userId);
            return StatusCode(response.StatusCode, response);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while retrieving groups for user {UserId}", userId);
            var errorResponse = ApiResponse<UserGroupsResponse>.ErrorResponse(
                "An error occurred while processing your request",
                new List<string> { ex.Message },
                500
            );
            return StatusCode(500, errorResponse);
        }
    }
}
