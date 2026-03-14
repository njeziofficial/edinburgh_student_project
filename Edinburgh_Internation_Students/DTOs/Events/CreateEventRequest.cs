using System.ComponentModel.DataAnnotations;

namespace Edinburgh_Internation_Students.DTOs.Events;

public class CreateEventRequest
{
    [Required(ErrorMessage = "Title is required")]
    [StringLength(200, MinimumLength = 2, ErrorMessage = "Title must be between 2 and 200 characters")]
    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }
    public string? ImageUrl { get; set; }

    [Required(ErrorMessage = "Start date is required")]
    public DateTime StartDate { get; set; }

    public DateTime? EndDate { get; set; }
    public string? Location { get; set; }
    public bool IsVirtual { get; set; }
    public string? VirtualLink { get; set; }
    public int? MaxAttendees { get; set; }
    public string? Category { get; set; }
    public string? GroupId { get; set; }
}
