using Edinburgh_Internation_Students.DTOs.Announcements;
using Edinburgh_Internation_Students.DTOs.Common;

namespace Edinburgh_Internation_Students.Services;

public interface IAnnouncementService
{
    Task<ApiResponse<List<AnnouncementDto>>> GetAllAnnouncementsAsync();
    Task<ApiResponse<AnnouncementDto>> GetAnnouncementByIdAsync(Guid announcementId);
    Task<ApiResponse<AnnouncementDto>> CreateAnnouncementAsync(int authorId, CreateAnnouncementRequest request);
    Task<ApiResponse<AnnouncementDto>> UpdateAnnouncementAsync(Guid announcementId, int userId, UpdateAnnouncementRequest request);
    Task<ApiResponse<bool>> DeleteAnnouncementAsync(Guid announcementId, int userId);
}
