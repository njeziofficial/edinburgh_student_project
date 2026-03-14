namespace Edinburgh_Internation_Students.Models;

public class GroupMember
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    public Guid GroupId { get; set; }
    
    public int UserId { get; set; }
    
    public DateTime JoinedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public Group Group { get; set; } = null!;
    public User User { get; set; } = null!;
}
