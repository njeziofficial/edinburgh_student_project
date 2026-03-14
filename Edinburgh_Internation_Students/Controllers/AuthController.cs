using Edinburgh_Internation_Students.DTOs;
using Edinburgh_Internation_Students.DTOs.Auth;
using Edinburgh_Internation_Students.Extensions;
using Edinburgh_Internation_Students.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Edinburgh_Internation_Students.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(IAuthService authService, ILogger<AuthController> logger) : ControllerBase
{
    /// <summary>
    /// Register a new user
    /// </summary>
    /// <param name="request">Sign up details including full name, email, and password</param>
    /// <returns>User details if successful</returns>
    [HttpPost("register")]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> SignUp([FromBody] SignUpRequest request)
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
            var (success, response, errorMessage) = await authService.SignUpAsync(request);

            if (!success)
            {
                return BadRequest(new ErrorResponse { Message = errorMessage });
            }

            return CreatedAtAction(nameof(SignUp), new { id = response!.UserId }, response);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred during sign up for email: {Email}", request.Email);
            return StatusCode(500, new ErrorResponse
            {
                Message = "An error occurred while processing your request"
            });
        }
    }

    /// <summary>
    /// Sign in an existing user
    /// </summary>
    /// <param name="request">Sign in credentials (email and password)</param>
    /// <returns>User details if successful</returns>
    [HttpPost("login")]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> SignIn([FromBody] SignInRequest request)
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
            var (success, response, errorMessage) = await authService.SignInAsync(request);

            if (!success)
            {
                return Unauthorized(new ErrorResponse { Message = errorMessage });
            }

            return Ok(response);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred during sign in for email: {Email}", request.Email);
            return StatusCode(500, new ErrorResponse
            {
                Message = "An error occurred while processing your request"
            });
        }
    }

    /// <summary>
    /// Refresh access token using refresh token
    /// </summary>
    /// <param name="request">Refresh token</param>
    /// <returns>New access token and refresh token</returns>
    [HttpPost("refresh")]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
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
            var (success, response, errorMessage) = await authService.RefreshTokenAsync(request);

            if (!success)
            {
                return Unauthorized(new ErrorResponse { Message = errorMessage });
            }

            return Ok(response);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred during token refresh");
            return StatusCode(500, new ErrorResponse
            {
                Message = "An error occurred while processing your request"
            });
        }
    }

    /// <summary>
    /// Change user password (requires authentication)
    /// </summary>
    /// <param name="request">Current and new password</param>
    /// <returns>Success message</returns>
    [HttpPost("change-password")]
    [Authorize]
    [ProducesResponseType(typeof(MessageResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
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
            var userId = User.GetUserId();
            var (success, errorMessage) = await authService.ChangePasswordAsync(userId, request);

            if (!success)
            {
                return BadRequest(new ErrorResponse { Message = errorMessage });
            }

            return Ok(new MessageResponse
            {
                Success = true,
                Message = "Password changed successfully"
            });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred during password change");
            return StatusCode(500, new ErrorResponse
            {
                Message = "An error occurred while processing your request"
            });
        }
    }

    /// <summary>
    /// Request password reset (send reset email)
    /// </summary>
    /// <param name="request">User email</param>
    /// <returns>Success message</returns>
    [HttpPost("forgot-password")]
    [ProducesResponseType(typeof(MessageResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request)
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
            var (success, errorMessage) = await authService.ForgotPasswordAsync(request);

            if (!success)
            {
                return BadRequest(new ErrorResponse { Message = errorMessage });
            }

            return Ok(new MessageResponse
            {
                Success = true,
                Message = "If an account exists with this email, a password reset link has been sent"
            });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred during forgot password request");
            return StatusCode(500, new ErrorResponse
            {
                Message = "An error occurred while processing your request"
            });
        }
    }

    /// <summary>
    /// Reset password using reset token
    /// </summary>
    /// <param name="request">Email, token, and new password</param>
    /// <returns>Success message</returns>
    [HttpPost("reset-password")]
    [ProducesResponseType(typeof(MessageResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
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
            var (success, errorMessage) = await authService.ResetPasswordAsync(request);

            if (!success)
            {
                return BadRequest(new ErrorResponse { Message = errorMessage });
            }

            return Ok(new MessageResponse
            {
                Success = true,
                Message = "Password has been reset successfully"
            });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred during password reset");
            return StatusCode(500, new ErrorResponse
            {
                Message = "An error occurred while processing your request"
            });
        }
    }

    /// <summary>
    /// Logout user (revoke refresh tokens)
    /// </summary>
    /// <returns>Success message</returns>
    [HttpPost("logout")]
    [Authorize]
    [ProducesResponseType(typeof(MessageResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Logout()
    {
        try
        {
            var userId = User.GetUserId();
            var (success, errorMessage) = await authService.LogoutAsync(userId);

            if (!success)
            {
                return BadRequest(new ErrorResponse { Message = errorMessage });
            }

            return Ok(new MessageResponse
            {
                Success = true,
                Message = "Logged out successfully"
            });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred during logout");
            return StatusCode(500, new ErrorResponse
            {
                Message = "An error occurred while processing your request"
            });
        }
    }

    /// <summary>
    /// Verify email address with verification code
    /// </summary>
    /// <param name="request">Email and verification code</param>
    /// <returns>Success message</returns>
    [HttpPost("verify-email")]
    [ProducesResponseType(typeof(MessageResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> VerifyEmail([FromBody] VerifyEmailRequest request)
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
            var (success, errorMessage) = await authService.VerifyEmailAsync(request);

            if (!success)
            {
                return BadRequest(new ErrorResponse { Message = errorMessage });
            }

            return Ok(new MessageResponse
            {
                Success = true,
                Message = "Email verified successfully"
            });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred during email verification");
            return StatusCode(500, new ErrorResponse
            {
                Message = "An error occurred while processing your request"
            });
        }
    }

    /// <summary>
    /// Resend email verification code
    /// </summary>
    /// <param name="email">User email</param>
    /// <returns>Success message</returns>
    [HttpPost("resend-verification")]
    [ProducesResponseType(typeof(MessageResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ResendVerification([FromQuery] string email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            return BadRequest(new ErrorResponse { Message = "Email is required" });
        }

        try
        {
            var (success, errorMessage) = await authService.ResendVerificationCodeAsync(email);

            if (!success)
            {
                return BadRequest(new ErrorResponse { Message = errorMessage });
            }

            return Ok(new MessageResponse
            {
                Success = true,
                Message = "If an account exists with this email, a verification code has been sent"
            });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while resending verification code");
            return StatusCode(500, new ErrorResponse
            {
                Message = "An error occurred while processing your request"
            });
        }
    }
}
