using Edinburgh_Internation_Students.DTOs;
using Edinburgh_Internation_Students.DTOs.Auth;

namespace Edinburgh_Internation_Students.Services;

public interface IAuthService
{
    Task<(bool Success, AuthResponse? Response, string ErrorMessage)> SignUpAsync(SignUpRequest request);
    Task<(bool Success, AuthResponse? Response, string ErrorMessage)> SignInAsync(SignInRequest request);
    Task<(bool Success, AuthResponse? Response, string ErrorMessage)> SignInAdminAsync(SignInRequest request);
    Task<(bool Success, AuthResponse? Response, string ErrorMessage)> RefreshTokenAsync(RefreshTokenRequest request);
    Task<(bool Success, string ErrorMessage)> ChangePasswordAsync(int userId, ChangePasswordRequest request);
    Task<(bool Success, string ErrorMessage)> ForgotPasswordAsync(ForgotPasswordRequest request);
    Task<(bool Success, string ErrorMessage)> ResetPasswordAsync(ResetPasswordRequest request);
    Task<(bool Success, string ErrorMessage)> LogoutAsync(int userId);
    Task<(bool Success, string ErrorMessage)> VerifyEmailAsync(VerifyEmailRequest request);
    Task<(bool Success, string ErrorMessage)> ResendVerificationCodeAsync(string email);
}
