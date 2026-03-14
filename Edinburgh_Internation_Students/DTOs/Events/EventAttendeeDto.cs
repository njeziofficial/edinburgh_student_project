using Edinburgh_Internation_Students.DTOs.Users;

namespace Edinburgh_Internation_Students.DTOs.Events;

public class EventAttendeeDto
{
    public string Id { get; set; } = string.Empty;
    public string EventId { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public string Status { get; set; } = "Going"; // Going, Interested, NotGoing
    public DateTime RegisteredAt { get; set; }
    public UserSummaryDto User { get; set; } = null!;
}
