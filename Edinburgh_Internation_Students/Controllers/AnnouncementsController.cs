using Edinburgh_Internation_Students.DTOs.Announcements;
using Edinburgh_Internation_Students.DTOs.Common;
using Edinburgh_Internation_Students.Extensions;
using Edinburgh_Internation_Students.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Edinburgh_Internation_Students.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AnnouncementsController : ControllerBase
{
    private readonly IAnnouncementService _announcementService;
    private readonly ILogger<AnnouncementsController> _logger;

    public AnnouncementsController(
        IAnnouncementService announcementService,
        ILogger<AnnouncementsController> logger)
    {
        _announcementService = announcementService;
        _logger = logger;
    }

    /// <summary>
    /// Get all announcements
    /// </summary>
    /// <returns>List of all announcements</returns>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<List<AnnouncementDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<List<AnnouncementDto>>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAllAnnouncements()
    {
        try
        {
            var response = await _announcementService.GetAllAnnouncementsAsync();
            return StatusCode(response.StatusCode, response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving all announcements");
            var errorResponse = ApiResponse<List<AnnouncementDto>>.ErrorResponse(
                "An error occurred while processing your request",
                new List<string> { ex.Message },
                500
            );
            return StatusCode(500, errorResponse);
        }
    }

    /// <summary>
    /// Get announcement by ID
    /// </summary>
    /// <param name="id">Announcement ID</param>
    /// <returns>Announcement details</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ApiResponse<AnnouncementDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<AnnouncementDto>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<AnnouncementDto>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAnnouncementById(Guid id)
    {
        try
        {
            var response = await _announcementService.GetAnnouncementByIdAsync(id);
            return StatusCode(response.StatusCode, response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving announcement {AnnouncementId}", id);
            var errorResponse = ApiResponse<AnnouncementDto>.ErrorResponse(
                "An error occurred while processing your request",
                new List<string> { ex.Message },
                500
            );
            return StatusCode(500, errorResponse);
        }
    }

    /// <summary>
    /// Create a new announcement
    /// </summary>
    /// <param name="request">Announcement details</param>
    /// <returns>Created announcement</returns>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<AnnouncementDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<AnnouncementDto>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<AnnouncementDto>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateAnnouncement([FromBody] CreateAnnouncementRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                var validationResponse = ApiResponse<AnnouncementDto>.ErrorResponse(
                    "Validation failed",
                    errors,
                    400
                );
                return BadRequest(validationResponse);
            }

            var userId = User.GetUserId();
            var response = await _announcementService.CreateAnnouncementAsync(userId, request);
            return StatusCode(response.StatusCode, response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while creating announcement");
            var errorResponse = ApiResponse<AnnouncementDto>.ErrorResponse(
                "An error occurred while processing your request",
                new List<string> { ex.Message },
                500
            );
            return StatusCode(500, errorResponse);
        }
    }

    /// <summary>
    /// Update an announcement
    /// </summary>
    /// <param name="id">Announcement ID</param>
    /// <param name="request">Updated announcement details</param>
    /// <returns>Updated announcement</returns>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(ApiResponse<AnnouncementDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<AnnouncementDto>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<AnnouncementDto>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<AnnouncementDto>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<AnnouncementDto>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UpdateAnnouncement(Guid id, [FromBody] UpdateAnnouncementRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                var validationResponse = ApiResponse<AnnouncementDto>.ErrorResponse(
                    "Validation failed",
                    errors,
                    400
                );
                return BadRequest(validationResponse);
            }

            var userId = User.GetUserId();
            var response = await _announcementService.UpdateAnnouncementAsync(id, userId, request);
            return StatusCode(response.StatusCode, response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while updating announcement {AnnouncementId}", id);
            var errorResponse = ApiResponse<AnnouncementDto>.ErrorResponse(
                "An error occurred while processing your request",
                new List<string> { ex.Message },
                500
            );
            return StatusCode(500, errorResponse);
        }
    }

    /// <summary>
    /// Delete an announcement (Admin only)
    /// </summary>
    /// <param name="id">Announcement ID</param>
    /// <returns>Success status</returns>
    [HttpDelete("{id}")]
    [Authorize(Policy = "AdminOnly")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteAnnouncement(Guid id)
    {
        try
        {
            var userId = User.GetUserId();
            var response = await _announcementService.DeleteAnnouncementAsync(id, userId);
            return StatusCode(response.StatusCode, response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while deleting announcement {AnnouncementId}", id);
            var errorResponse = ApiResponse<bool>.ErrorResponse(
                "An error occurred while processing your request",
                new List<string> { ex.Message },
                500
            );
            return StatusCode(500, errorResponse);
        }
    }
}
