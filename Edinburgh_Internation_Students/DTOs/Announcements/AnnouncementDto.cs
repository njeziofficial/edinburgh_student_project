using Edinburgh_Internation_Students.DTOs.Users;

namespace Edinburgh_Internation_Students.DTOs.Announcements;

public class AnnouncementDto
{
    public string Id { get; set; } = string.Empty;
    public string GroupId { get; set; } = string.Empty;
    public string AuthorId { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public bool IsPinned { get; set; }
    public UserSummaryDto? Author { get; set; }
    public DateTime CreatedAt { get; set; }
}
