namespace Edinburgh_Internation_Students.Models;

public class Event
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    public string Title { get; set; } = string.Empty;
    
    public string? Description { get; set; }
    
    public DateTime Date { get; set; }
    
    public TimeSpan Time { get; set; }
    
    public string Location { get; set; } = string.Empty;
    
    public EventCategory Category { get; set; }
    
    public int OrganizerId { get; set; }
    
    public int? MaxAttendees { get; set; }
    
    public string? ImageUrl { get; set; }
    
    public bool IsCancelled { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public User Organizer { get; set; } = null!;
    public ICollection<EventAttendee> Attendees { get; set; } = new List<EventAttendee>();
}
