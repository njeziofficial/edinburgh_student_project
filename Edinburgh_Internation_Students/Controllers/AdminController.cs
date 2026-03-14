using Edinburgh_Internation_Students.DTOs.Common;
using Edinburgh_Internation_Students.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Edinburgh_Internation_Students.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AdminController : ControllerBase
{
    private readonly IGroupService _groupService;
    private readonly ILogger<AdminController> _logger;

    public AdminController(IGroupService groupService, ILogger<AdminController> logger)
    {
        _groupService = groupService;
        _logger = logger;
    }

    /// <summary>
    /// Manually trigger group rotation/shuffle (for testing)
    /// </summary>
    /// <returns>Success message with rotation details</returns>
    [HttpPost("trigger-group-rotation")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> TriggerGroupRotation()
    {
        try
        {
            _logger.LogInformation("Manual group rotation triggered by user");

            // Get groups before rotation
            var groupsBefore = await _groupService.GetActiveGroupsWithMembersAsync();
            var totalUsersBefore = groupsBefore.Sum(g => g.MemberCount);

            // Perform rotation
            await _groupService.ShuffleGroupMembersAsync();

            // Get groups after rotation
            var groupsAfter = await _groupService.GetActiveGroupsWithMembersAsync();
            var totalUsersAfter = groupsAfter.Sum(g => g.MemberCount);

            var result = new
            {
                message = "Group rotation completed successfully",
                beforeRotation = new
                {
                    totalGroups = groupsBefore.Count,
                    totalUsers = totalUsersBefore,
                    groups = groupsBefore.Select(g => new { g.Name, g.MemberCount })
                },
                afterRotation = new
                {
                    totalGroups = groupsAfter.Count,
                    totalUsers = totalUsersAfter,
                    groups = groupsAfter.Select(g => new { g.Name, g.MemberCount })
                },
                timestamp = DateTime.UtcNow
            };

            return Ok(ApiResponse<object>.SuccessResponse(result, "Group rotation triggered successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred during manual group rotation");
            return StatusCode(500, ApiResponse<object>.ErrorResponse(
                "Failed to trigger group rotation",
                new List<string> { ex.Message },
                500
            ));
        }
    }

    /// <summary>
    /// Get current group distribution statistics
    /// </summary>
    /// <returns>Group statistics</returns>
    [HttpGet("group-stats")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetGroupStats()
    {
        try
        {
            var groups = await _groupService.GetActiveGroupsWithMembersAsync();

            var stats = new
            {
                totalGroups = groups.Count,
                totalUsers = groups.Sum(g => g.MemberCount),
                averageMembersPerGroup = groups.Count > 0 ? groups.Average(g => g.MemberCount) : 0,
                groups = groups.Select(g => new
                {
                    g.Id,
                    g.Name,
                    g.MemberCount,
                    g.Description,
                    g.CreatedAt
                }).OrderBy(g => g.Name)
            };

            return Ok(ApiResponse<object>.SuccessResponse(stats, "Group statistics retrieved"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving group stats");
            return StatusCode(500, ApiResponse<object>.ErrorResponse(
                "Failed to retrieve group statistics",
                new List<string> { ex.Message },
                500
            ));
        }
    }
}
