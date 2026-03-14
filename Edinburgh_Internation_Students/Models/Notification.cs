namespace Edinburgh_Internation_Students.Models;

public class Notification
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    public int UserId { get; set; }
    
    public NotificationType Type { get; set; }
    
    public string Title { get; set; } = string.Empty;
    
    public string Message { get; set; } = string.Empty;
    
    public Guid? ReferenceId { get; set; }
    
    public bool IsRead { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation property
    public User User { get; set; } = null!;
}
