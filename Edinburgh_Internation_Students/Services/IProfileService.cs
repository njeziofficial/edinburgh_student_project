using Edinburgh_Internation_Students.DTOs;

namespace Edinburgh_Internation_Students.Services;

public interface IProfileService
{
    Task<(bool Success, ProfileResponse? Response, string ErrorMessage)> CreateProfileAsync(int userId, CreateProfileRequest request);
    Task<(bool Success, ProfileResponse? Response, string ErrorMessage)> GetProfileByUserIdAsync(int userId);
    Task<(bool Success, ProfileResponse? Response, string ErrorMessage)> UpdateProfileAsync(int userId, UpdateProfileRequest request);
    Task<(bool Success, string ErrorMessage)> DeleteProfileAsync(int userId);
}
