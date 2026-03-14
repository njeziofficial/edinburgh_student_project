namespace Edinburgh_Internation_Students.Models;

public class Announcement
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    public string Title { get; set; } = string.Empty;
    
    public string Content { get; set; } = string.Empty;
    
    public int AuthorId { get; set; }
    
    public AnnouncementPriority Priority { get; set; } = AnnouncementPriority.Normal;
    
    public bool IsActive { get; set; } = true;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation property
    public User Author { get; set; } = null!;
}
