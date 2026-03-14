using Edinburgh_Internation_Students.Data;
using Edinburgh_Internation_Students.DTOs;
using Edinburgh_Internation_Students.DTOs.Auth;
using Edinburgh_Internation_Students.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;

namespace Edinburgh_Internation_Students.Services;

public class AuthService(ApplicationDbContext context, IJwtService jwtService, IGroupService groupService, ILogger<AuthService> logger) : IAuthService
{
    public async Task<(bool Success, AuthResponse? Response, string ErrorMessage)> SignUpAsync(SignUpRequest request)
    {
        var existingUser = await context.Users
            .FirstOrDefaultAsync(u => EF.Functions.ILike(u.Email, request.Email));


        if (IsExistingUser(request.Email))
        {
            return (false, null, "A user with this email already exists");
        }

        var user = new User
        {
            FirstName = request.FirstName.Trim(),
            LastName = request.LastName.Trim(),
            Email = request.Email.ToLower(),
            PhoneCode = request.PhoneCode,
            PhoneNumber = request.PhoneNumber,
            CreatedAt = DateTime.UtcNow
        };

        user.SetPassword(request.Password);

        await context.Users.AddAsync(user);
        await context.SaveChangesAsync();

        // Automatically assign user to a group immediately on signup
        try
        {
            var groupId = await groupService.FindOrCreateAvailableGroupAsync();
            var addMemberResult = await groupService.AddMemberToGroupAsync(groupId, user.Id);

            if (addMemberResult.Success)
            {
                logger.LogInformation("User {UserId} automatically assigned to group {GroupId} on signup", 
                    user.Id, groupId);
            }
            else
            {
                logger.LogWarning("Failed to assign user {UserId} to group on signup: {Error}", 
                    user.Id, addMemberResult.Message);
            }
        }
        catch (Exception ex)
        {
            // Log error but don't fail signup if group assignment fails
            logger.LogError(ex, "Error assigning user {UserId} to group on signup", user.Id);
        }

        var token = jwtService.GenerateToken(user.Id, user.Email, user.FirstName, user.LastName);

        var response = new AuthResponse
        {
            UserId = user.Id,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            PhoneCode = user.PhoneCode,
            PhoneNumber = user.PhoneNumber,
            Token = token,
            Message = "User registered successfully"
        };

        return (true, response, string.Empty);
    }

    public async Task<(bool Success, AuthResponse? Response, string ErrorMessage)> SignInAsync(SignInRequest request)
    {
        var user = await context.Users
            .FirstOrDefaultAsync(u => EF.Functions.ILike(u.Email, request.Email));

        if (user == null)
        {
            return (false, null, "Invalid email or password");
        }

        if (!user.VerifyPassword(request.Password))
        {
            return (false, null, "Invalid email or password");
        }

        user.UpdatedAt = DateTime.UtcNow;
        user.LastActive = DateTime.UtcNow;
        await context.SaveChangesAsync();

        var token = jwtService.GenerateToken(user.Id, user.Email, user.FirstName, user.LastName);

        var response = new AuthResponse
        {
            UserId = user.Id,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            PhoneCode = user.PhoneCode,
            PhoneNumber = user.PhoneNumber,
            Token = token,
            Message = "Sign in successful"
        };

        return (true, response, string.Empty);
    }

    public async Task<(bool Success, AuthResponse? Response, string ErrorMessage)> RefreshTokenAsync(RefreshTokenRequest request)
    {
        try
        {
            var refreshToken = await context.RefreshTokens
                .Include(rt => rt.User)
                .FirstOrDefaultAsync(rt => rt.Token == request.RefreshToken);

            if (refreshToken == null)
            {
                return (false, null, "Invalid refresh token");
            }

            if (!refreshToken.IsActive)
            {
                return (false, null, "Refresh token has expired or been revoked");
            }

            // Generate new tokens
            var newAccessToken = jwtService.GenerateToken(
                refreshToken.User.Id,
                refreshToken.User.Email,
                refreshToken.User.FirstName,
                refreshToken.User.LastName
            );

            var newRefreshToken = GenerateRefreshToken();

            // Revoke old refresh token
            refreshToken.RevokedAt = DateTime.UtcNow;

            // Create new refresh token
            var newRefreshTokenEntity = new RefreshToken
            {
                UserId = refreshToken.UserId,
                Token = newRefreshToken,
                ExpiresAt = DateTime.UtcNow.AddDays(7),
                CreatedAt = DateTime.UtcNow
            };

            await context.RefreshTokens.AddAsync(newRefreshTokenEntity);
            await context.SaveChangesAsync();

            var response = new AuthResponse
            {
                UserId = refreshToken.User.Id,
                Email = refreshToken.User.Email,
                FirstName = refreshToken.User.FirstName,
                LastName = refreshToken.User.LastName,
                PhoneCode = refreshToken.User.PhoneCode,
                PhoneNumber = refreshToken.User.PhoneNumber,
                Token = newAccessToken,
                Message = "Token refreshed successfully"
            };

            return (true, response, string.Empty);
        }
        catch (Exception ex)
        {
            return (false, null, $"Token refresh failed: {ex.Message}");
        }
    }

    public async Task<(bool Success, string ErrorMessage)> ChangePasswordAsync(int userId, ChangePasswordRequest request)
    {
        try
        {
            var user = await context.Users.FindAsync(userId);

            if (user == null)
            {
                return (false, "User not found");
            }

            if (!user.VerifyPassword(request.CurrentPassword))
            {
                return (false, "Current password is incorrect");
            }

            user.SetPassword(request.NewPassword);
            user.UpdatedAt = DateTime.UtcNow;

            await context.SaveChangesAsync();

            return (true, string.Empty);
        }
        catch (Exception ex)
        {
            return (false, $"Password change failed: {ex.Message}");
        }
    }

    public async Task<(bool Success, string ErrorMessage)> ForgotPasswordAsync(ForgotPasswordRequest request)
    {
        try
        {
            var user = await context.Users
                .FirstOrDefaultAsync(u => EF.Functions.ILike(u.Email, request.Email));

            if (user == null)
            {
                // For security, don't reveal if email exists
                return (true, string.Empty);
            }

            // Generate reset token (in production, store this in database)
            var resetToken = GenerateSecureToken();

            // TODO: Send email with reset token
            // In production, you would:
            // 1. Store resetToken in database with expiry (e.g., 1 hour)
            // 2. Send email with reset link containing the token
            // Example: https://yourapp.com/reset-password?token={resetToken}&email={email}

            // For now, log it (remove in production)
            Console.WriteLine($"Password reset token for {user.Email}: {resetToken}");

            return (true, string.Empty);
        }
        catch (Exception ex)
        {
            return (false, $"Failed to process password reset: {ex.Message}");
        }
    }

    public async Task<(bool Success, string ErrorMessage)> ResetPasswordAsync(ResetPasswordRequest request)
    {
        try
        {
            var user = await context.Users
                .FirstOrDefaultAsync(u => EF.Functions.ILike(u.Email, request.Email));

            if (user == null)
            {
                return (false, "Invalid reset token or email");
            }

            // TODO: Verify reset token from database
            // In production, you would:
            // 1. Look up the token in database
            // 2. Check if it's expired
            // 3. Verify it matches the user
            // For now, we'll just reset the password

            user.SetPassword(request.NewPassword);
            user.UpdatedAt = DateTime.UtcNow;

            await context.SaveChangesAsync();

            // TODO: Delete used reset token from database
            // TODO: Optionally revoke all refresh tokens for security

            return (true, string.Empty);
        }
        catch (Exception ex)
        {
            return (false, $"Password reset failed: {ex.Message}");
        }
    }

    public async Task<(bool Success, string ErrorMessage)> LogoutAsync(int userId)
    {
        try
        {
            // Revoke all active refresh tokens for the user
            var activeTokens = await context.RefreshTokens
                .Where(rt => rt.UserId == userId && rt.RevokedAt == null)
                .ToListAsync();

            foreach (var token in activeTokens)
            {
                token.RevokedAt = DateTime.UtcNow;
            }

            await context.SaveChangesAsync();

            return (true, string.Empty);
        }
        catch (Exception ex)
        {
            return (false, $"Logout failed: {ex.Message}");
        }
    }

    public async Task<(bool Success, string ErrorMessage)> VerifyEmailAsync(VerifyEmailRequest request)
    {
        try
        {
            var user = await context.Users
                .FirstOrDefaultAsync(u => EF.Functions.ILike(u.Email, request.Email));

            if (user == null)
            {
                return (false, "Invalid verification code");
            }

            // TODO: Verify code from database
            // In production, you would:
            // 1. Look up the verification code in database
            // 2. Check if it's expired
            // 3. Mark email as verified in User table

            // For now, just return success
            return (true, string.Empty);
        }
        catch (Exception ex)
        {
            return (false, $"Email verification failed: {ex.Message}");
        }
    }

    public async Task<(bool Success, string ErrorMessage)> ResendVerificationCodeAsync(string email)
    {
        try
        {
            var user = await context.Users
                .FirstOrDefaultAsync(u => EF.Functions.ILike(u.Email, email));

            if (user == null)
            {
                // For security, don't reveal if email exists
                return (true, string.Empty);
            }

            // Generate new verification code
            var verificationCode = GenerateVerificationCode();

            // TODO: Send email with verification code
            // In production, you would:
            // 1. Store code in database with expiry
            // 2. Send email with the code

            // For now, log it (remove in production)
            Console.WriteLine($"Verification code for {user.Email}: {verificationCode}");

            return (true, string.Empty);
        }
        catch (Exception ex)
        {
            return (false, $"Failed to resend verification code: {ex.Message}");
        }
    }

    // Helper methods
    private bool IsExistingUser(string email) =>
        context.Users.Any(u => EF.Functions.ILike(u.Email, email));

    private static string GenerateRefreshToken()
    {
        var randomBytes = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomBytes);
        return Convert.ToBase64String(randomBytes);
    }

    private static string GenerateSecureToken()
    {
        var randomBytes = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomBytes);
        return Convert.ToBase64String(randomBytes).Replace("+", "-").Replace("/", "_").Replace("=", "");
    }

    private static string GenerateVerificationCode()
    {
        return new Random().Next(100000, 999999).ToString();
    }
}
