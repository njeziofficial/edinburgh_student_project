using Edinburgh_Internation_Students.DTOs.Common;
using Edinburgh_Internation_Students.DTOs.Events;
using Edinburgh_Internation_Students.Extensions;
using Edinburgh_Internation_Students.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Edinburgh_Internation_Students.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class EventsController(IEventService eventService, ILogger<EventsController> logger) : ControllerBase
{

    /// <summary>
    /// Get all events
    /// </summary>
    /// <returns>List of all events</returns>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<List<EventDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<List<EventDto>>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAllEvents()
    {
        try
        {
            var response = await eventService.GetAllEventsAsync();
            return StatusCode(response.StatusCode, response);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while retrieving all events");
            var errorResponse = ApiResponse<List<EventDto>>.ErrorResponse(
                "An error occurred while processing your request",
                [ex.Message],
                500
            );
            return StatusCode(500, errorResponse);
        }
    }

    /// <summary>
    /// Get event by ID
    /// </summary>
    /// <param name="id">Event ID</param>
    /// <returns>Event details</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ApiResponse<EventDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<EventDto>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<EventDto>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetEventById(Guid id)
    {
        try
        {
            var response = await eventService.GetEventByIdAsync(id);
            return StatusCode(response.StatusCode, response);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while retrieving event {EventId}", id);
            var errorResponse = ApiResponse<EventDto>.ErrorResponse(
                "An error occurred while processing your request",
                [ex.Message],
                500
            );
            return StatusCode(500, errorResponse);
        }
    }

    /// <summary>
    /// Create a new event
    /// </summary>
    /// <param name="request">Event details</param>
    /// <returns>Created event</returns>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<EventDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<EventDto>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<EventDto>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateEvent([FromBody] CreateEventRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                var validationResponse = ApiResponse<EventDto>.ErrorResponse(
                    "Validation failed",
                    errors,
                    400
                );
                return BadRequest(validationResponse);
            }

            var userId = User.GetUserId();
            var response = await eventService.CreateEventAsync(userId, request);
            return StatusCode(response.StatusCode, response);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while creating event");
            var errorResponse = ApiResponse<EventDto>.ErrorResponse(
                "An error occurred while processing your request",
                [ex.Message],
                500
            );
            return StatusCode(500, errorResponse);
        }
    }

    /// <summary>
    /// Update an event
    /// </summary>
    /// <param name="id">Event ID</param>
    /// <param name="request">Updated event details</param>
    /// <returns>Updated event</returns>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(ApiResponse<EventDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<EventDto>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<EventDto>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<EventDto>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<EventDto>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UpdateEvent(Guid id, [FromBody] UpdateEventRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                var validationResponse = ApiResponse<EventDto>.ErrorResponse(
                    "Validation failed",
                    errors,
                    400
                );
                return BadRequest(validationResponse);
            }

            var userId = User.GetUserId();
            var response = await eventService.UpdateEventAsync(id, userId, request);
            return StatusCode(response.StatusCode, response);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while updating event {EventId}", id);
            var errorResponse = ApiResponse<EventDto>.ErrorResponse(
                "An error occurred while processing your request",
                new List<string> { ex.Message },
                500
            );
            return StatusCode(500, errorResponse);
        }
    }

    /// <summary>
    /// Delete an event (Admin only)
    /// </summary>
    /// <param name="id">Event ID</param>
    /// <returns>Success status</returns>
    [HttpDelete("{id}")]
    [Authorize(Policy = "AdminOnly")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteEvent(Guid id)
    {
        try
        {
            var userId = User.GetUserId();
            var response = await eventService.DeleteEventAsync(id, userId);
            return StatusCode(response.StatusCode, response);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while deleting event {EventId}", id);
            var errorResponse = ApiResponse<bool>.ErrorResponse(
                "An error occurred while processing your request",
                new List<string> { ex.Message },
                500
            );
            return StatusCode(500, errorResponse);
        }
    }

    /// <summary>
    /// RSVP to an event
    /// </summary>
    /// <param name="id">Event ID</param>
    /// <param name="status">RSVP status (Going, Maybe, NotGoing)</param>
    /// <returns>Attendee details</returns>
    [HttpPost("{id}/rsvp")]
    [ProducesResponseType(typeof(ApiResponse<EventAttendeeDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<EventAttendeeDto>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<EventAttendeeDto>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<EventAttendeeDto>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> RsvpEvent(Guid id, [FromQuery] string status)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(status))
            {
                var validationResponse = ApiResponse<EventAttendeeDto>.ErrorResponse(
                    "Status is required",
                    new List<string> { "Valid statuses: Going, Maybe, NotGoing" },
                    400
                );
                return BadRequest(validationResponse);
            }

            var userId = User.GetUserId();
            var response = await eventService.RsvpEventAsync(id, userId, status);
            return StatusCode(response.StatusCode, response);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while updating RSVP for event {EventId}", id);
            var errorResponse = ApiResponse<EventAttendeeDto>.ErrorResponse(
                "An error occurred while processing your request",
                new List<string> { ex.Message },
                500
            );
            return StatusCode(500, errorResponse);
        }
    }

    /// <summary>
    /// Get all attendees for an event
    /// </summary>
    /// <param name="id">Event ID</param>
    /// <returns>List of attendees</returns>
    [HttpGet("{id}/attendees")]
    [ProducesResponseType(typeof(ApiResponse<List<EventAttendeeDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<List<EventAttendeeDto>>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<List<EventAttendeeDto>>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetEventAttendees(Guid id)
    {
        try
        {
            var response = await eventService.GetEventAttendeesAsync(id);
            return StatusCode(response.StatusCode, response);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while retrieving attendees for event {EventId}", id);
            var errorResponse = ApiResponse<List<EventAttendeeDto>>.ErrorResponse(
                "An error occurred while processing your request",
                new List<string> { ex.Message },
                500
            );
            return StatusCode(500, errorResponse);
        }
    }
}
