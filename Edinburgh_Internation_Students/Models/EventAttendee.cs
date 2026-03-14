namespace Edinburgh_Internation_Students.Models;

public class EventAttendee
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    public Guid EventId { get; set; }
    
    public int UserId { get; set; }
    
    public DateTime RsvpAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public Event Event { get; set; } = null!;
    public User User { get; set; } = null!;
}
