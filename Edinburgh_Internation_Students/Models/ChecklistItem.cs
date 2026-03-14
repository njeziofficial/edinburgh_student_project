namespace Edinburgh_Internation_Students.Models;

public class ChecklistItem
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    public Guid GroupId { get; set; }
    
    public string Title { get; set; } = string.Empty;
    
    public bool IsCompleted { get; set; }
    
    public int? CompletedBy { get; set; }
    
    public DateTime? CompletedAt { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public Group Group { get; set; } = null!;
    public User? CompletedByUser { get; set; }
}
