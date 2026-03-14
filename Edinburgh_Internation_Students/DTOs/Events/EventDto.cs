namespace Edinburgh_Internation_Students.DTOs.Events;

public class EventDto
{
    public string Id { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string? Location { get; set; }
    public bool IsVirtual { get; set; }
    public string? VirtualLink { get; set; }
    public int? MaxAttendees { get; set; }
    public string Status { get; set; } = "Upcoming"; // Draft, Upcoming, Ongoing, Completed, Cancelled
    public string? GroupId { get; set; }
    public string OrganizerId { get; set; } = string.Empty;
    public int AttendeeCount { get; set; }
    public DateTime CreatedAt { get; set; }
}
