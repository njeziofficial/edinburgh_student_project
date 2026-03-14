using Edinburgh_Internation_Students.DTOs;
using Edinburgh_Internation_Students.Services;
using Microsoft.AspNetCore.Mvc;

namespace Edinburgh_Internation_Students.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProfileController(IProfileService profileService, ILogger<ProfileController> logger) : ControllerBase
{

    /// <summary>
    /// Create a profile for a user
    /// </summary>
    /// <param name="userId">The user ID</param>
    /// <param name="request">Profile details</param>
    /// <returns>Created profile</returns>
    [HttpPost("user/{userId}")]
    [ProducesResponseType(typeof(ProfileResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateProfile(int userId, [FromBody] CreateProfileRequest request)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState
                .Where(x => x.Value?.Errors.Count > 0)
                .ToDictionary(
                    kvp => kvp.Key,
                    kvp => kvp.Value?.Errors.Select(e => e.ErrorMessage).ToArray() ?? Array.Empty<string>()
                );

            return BadRequest(new ErrorResponse
            {
                Message = "Validation failed",
                Errors = errors
            });
        }

        try
        {
            var (success, response, errorMessage) = await profileService.CreateProfileAsync(userId, request);

            if (!success)
            {
                if (errorMessage == "User not found")
                    return NotFound(new ErrorResponse { Message = errorMessage });

                return BadRequest(new ErrorResponse { Message = errorMessage });
            }

            return CreatedAtAction(nameof(GetProfileByUserId), new { userId }, response);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while creating profile for user: {UserId}", userId);
            return StatusCode(500, new ErrorResponse
            {
                Message = "An error occurred while processing your request"
            });
        }
    }

    /// <summary>
    /// Get a user's profile
    /// </summary>
    /// <param name="userId">The user ID</param>
    /// <returns>User profile</returns>
    [HttpGet("user/{userId}")]
    [ProducesResponseType(typeof(ProfileResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetProfileByUserId(int userId)
    {
        try
        {
            var (success, response, errorMessage) = await profileService.GetProfileByUserIdAsync(userId);

            if (!success)
            {
                return NotFound(new ErrorResponse { Message = errorMessage });
            }

            return Ok(response);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while retrieving profile for user: {UserId}", userId);
            return StatusCode(500, new ErrorResponse
            {
                Message = "An error occurred while processing your request"
            });
        }
    }

    /// <summary>
    /// Update a user's profile
    /// </summary>
    /// <param name="userId">The user ID</param>
    /// <param name="request">Updated profile details</param>
    /// <returns>Updated profile</returns>
    [HttpPut("user/{userId}")]
    [ProducesResponseType(typeof(ProfileResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UpdateProfile(int userId, [FromBody] UpdateProfileRequest request)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState
                .Where(x => x.Value?.Errors.Count > 0)
                .ToDictionary(
                    kvp => kvp.Key,
                    kvp => kvp.Value?.Errors.Select(e => e.ErrorMessage).ToArray() ?? Array.Empty<string>()
                );

            return BadRequest(new ErrorResponse
            {
                Message = "Validation failed",
                Errors = errors
            });
        }

        try
        {
            var (success, response, errorMessage) = await profileService.UpdateProfileAsync(userId, request);

            if (!success)
            {
                return NotFound(new ErrorResponse { Message = errorMessage });
            }

            return Ok(response);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while updating profile for user: {UserId}", userId);
            return StatusCode(500, new ErrorResponse
            {
                Message = "An error occurred while processing your request"
            });
        }
    }

    /// <summary>
    /// Delete a user's profile
    /// </summary>
    /// <param name="userId">The user ID</param>
    /// <returns>Success or error message</returns>
    [HttpDelete("user/{userId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteProfile(int userId)
    {
        try
        {
            var (success, errorMessage) = await profileService.DeleteProfileAsync(userId);

            if (!success)
            {
                return NotFound(new ErrorResponse { Message = errorMessage });
            }

            return NoContent();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while deleting profile for user: {UserId}", userId);
            return StatusCode(500, new ErrorResponse
            {
                Message = "An error occurred while processing your request"
            });
        }
    }
}
