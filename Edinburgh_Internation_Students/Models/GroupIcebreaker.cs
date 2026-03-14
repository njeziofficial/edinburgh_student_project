namespace Edinburgh_Internation_Students.Models;

public class GroupIcebreaker
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    public Guid GroupId { get; set; }
    
    public string Question { get; set; } = string.Empty;
    
    public int OrderIndex { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation property
    public Group Group { get; set; } = null!;
}
