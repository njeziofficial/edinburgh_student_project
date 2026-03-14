using System.ComponentModel.DataAnnotations;

namespace Edinburgh_Internation_Students.DTOs.Announcements;

public class CreateAnnouncementRequest
{
    [Required(ErrorMessage = "Title is required")]
    [StringLength(200, MinimumLength = 2, ErrorMessage = "Title must be between 2 and 200 characters")]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "Content is required")]
    public string Content { get; set; } = string.Empty;

    public string? Priority { get; set; }
}
