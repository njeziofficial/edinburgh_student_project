using Edinburgh_Internation_Students.Data;
using Edinburgh_Internation_Students.DTOs.Announcements;
using Edinburgh_Internation_Students.DTOs.Common;
using Edinburgh_Internation_Students.DTOs.Users;
using Edinburgh_Internation_Students.Models;
using Microsoft.EntityFrameworkCore;

namespace Edinburgh_Internation_Students.Services;

public class AnnouncementService(ApplicationDbContext context, ILogger<AnnouncementService> logger) : IAnnouncementService
{
    public async Task<ApiResponse<List<AnnouncementDto>>> GetAllAnnouncementsAsync()
    {
        try
        {
            var announcements = await context.Announcements
                .Include(a => a.Author)
                .OrderByDescending(a => a.CreatedAt)
                .Select(a => new AnnouncementDto
                {
                    Id = a.Id.ToString(),
                    GroupId = "",
                    Title = a.Title,
                    Content = a.Content,
                    IsPinned = false,
                    AuthorId = a.AuthorId.ToString(),
                    Author = new UserSummaryDto
                    {
                        Id = a.Author.Id.ToString(),
                        Name = $"{a.Author.FirstName} {a.Author.LastName}".Trim(),
                        AvatarUrl = null,
                        Country = null,
                        IsOnline = false
                    },
                    CreatedAt = a.CreatedAt
                })
                .ToListAsync();

            return ApiResponse<List<AnnouncementDto>>.SuccessResponse(announcements);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error retrieving all announcements");
            return ApiResponse<List<AnnouncementDto>>.ErrorResponse(
                "Failed to retrieve announcements",
                new List<string> { ex.Message },
                500
            );
        }
    }

    public async Task<ApiResponse<AnnouncementDto>> GetAnnouncementByIdAsync(Guid announcementId)
    {
        try
        {
            var announcement = await context.Announcements
                .Include(a => a.Author)
                .FirstOrDefaultAsync(a => a.Id == announcementId);

            if (announcement == null)
            {
                return ApiResponse<AnnouncementDto>.ErrorResponse("Announcement not found", null, 404);
            }

            var announcementDto = new AnnouncementDto
            {
                Id = announcement.Id.ToString(),
                GroupId = "",
                Title = announcement.Title,
                Content = announcement.Content,
                IsPinned = false,
                AuthorId = announcement.AuthorId.ToString(),
                Author = new UserSummaryDto
                {
                    Id = announcement.Author.Id.ToString(),
                    Name = $"{announcement.Author.FirstName} {announcement.Author.LastName}".Trim(),
                    AvatarUrl = null,
                    Country = null,
                    IsOnline = false
                },
                CreatedAt = announcement.CreatedAt
            };

            return ApiResponse<AnnouncementDto>.SuccessResponse(announcementDto);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error retrieving announcement {AnnouncementId}", announcementId);
            return ApiResponse<AnnouncementDto>.ErrorResponse(
                "Failed to retrieve announcement",
                new List<string> { ex.Message },
                500
            );
        }
    }

    public async Task<ApiResponse<AnnouncementDto>> CreateAnnouncementAsync(int authorId, CreateAnnouncementRequest request)
    {
        try
        {
            var announcement = new Announcement
            {
                Title = request.Title,
                Content = request.Content,
                AuthorId = authorId,
                CreatedAt = DateTime.UtcNow
            };

            await context.Announcements.AddAsync(announcement);
            await context.SaveChangesAsync();

            var author = await context.Users.FindAsync(authorId);

            var announcementDto = new AnnouncementDto
            {
                Id = announcement.Id.ToString(),
                GroupId = "",
                Title = announcement.Title,
                Content = announcement.Content,
                IsPinned = false,
                AuthorId = announcement.AuthorId.ToString(),
                Author = new UserSummaryDto
                {
                    Id = author!.Id.ToString(),
                    Name = $"{author.FirstName} {author.LastName}".Trim(),
                    AvatarUrl = null,
                    Country = null,
                    IsOnline = false
                },
                CreatedAt = announcement.CreatedAt
            };

            return ApiResponse<AnnouncementDto>.SuccessResponse(
                announcementDto,
                "Announcement created successfully",
                201
            );
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error creating announcement");
            return ApiResponse<AnnouncementDto>.ErrorResponse(
                "Failed to create announcement",
                new List<string> { ex.Message },
                500
            );
        }
    }

    public async Task<ApiResponse<AnnouncementDto>> UpdateAnnouncementAsync(Guid announcementId, int userId, UpdateAnnouncementRequest request)
    {
        try
        {
            var announcement = await context.Announcements
                .Include(a => a.Author)
                .FirstOrDefaultAsync(a => a.Id == announcementId);

            if (announcement == null)
            {
                return ApiResponse<AnnouncementDto>.ErrorResponse("Announcement not found", null, 404);
            }

            if (announcement.AuthorId != userId)
            {
                return ApiResponse<AnnouncementDto>.ErrorResponse(
                    "You don't have permission to update this announcement",
                    null,
                    403
                );
            }

            announcement.Title = request.Title;
            announcement.Content = request.Content;
            announcement.UpdatedAt = DateTime.UtcNow;

            await context.SaveChangesAsync();

            var announcementDto = new AnnouncementDto
            {
                Id = announcement.Id.ToString(),
                GroupId = "",
                Title = announcement.Title,
                Content = announcement.Content,
                IsPinned = false,
                AuthorId = announcement.AuthorId.ToString(),
                Author = new UserSummaryDto
                {
                    Id = announcement.Author.Id.ToString(),
                    Name = $"{announcement.Author.FirstName} {announcement.Author.LastName}".Trim(),
                    AvatarUrl = null,
                    Country = null,
                    IsOnline = false
                },
                CreatedAt = announcement.CreatedAt
            };

            return ApiResponse<AnnouncementDto>.SuccessResponse(
                announcementDto,
                "Announcement updated successfully"
            );
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error updating announcement {AnnouncementId}", announcementId);
            return ApiResponse<AnnouncementDto>.ErrorResponse(
                "Failed to update announcement",
                new List<string> { ex.Message },
                500
            );
        }
    }

    public async Task<ApiResponse<bool>> DeleteAnnouncementAsync(Guid announcementId, int userId)
    {
        try
        {
            var announcement = await context.Announcements
                .FirstOrDefaultAsync(a => a.Id == announcementId);

            if (announcement == null)
            {
                return ApiResponse<bool>.ErrorResponse("Announcement not found", null, 404);
            }

            if (announcement.AuthorId != userId)
            {
                return ApiResponse<bool>.ErrorResponse(
                    "You don't have permission to delete this announcement",
                    null,
                    403
                );
            }

            context.Announcements.Remove(announcement);
            await context.SaveChangesAsync();

            return ApiResponse<bool>.SuccessResponse(true, "Announcement deleted successfully");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error deleting announcement {AnnouncementId}", announcementId);
            return ApiResponse<bool>.ErrorResponse(
                "Failed to delete announcement",
                new List<string> { ex.Message },
                500
            );
        }
    }
}
