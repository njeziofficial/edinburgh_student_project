namespace Edinburgh_Internation_Students.Models;

public class Group
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    public string Name { get; set; } = string.Empty;
    
    public string? Description { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime ExpiresAt { get; set; }
    
    public bool IsActive { get; set; } = true;

    // Navigation properties
    public ICollection<GroupMember> Members { get; set; } = new List<GroupMember>();
    public ICollection<GroupIcebreaker> Icebreakers { get; set; } = new List<GroupIcebreaker>();
    public ICollection<ChecklistItem> ChecklistItems { get; set; } = new List<ChecklistItem>();
    public ICollection<Poll> Polls { get; set; } = new List<Poll>();
    public ICollection<Message> Messages { get; set; } = new List<Message>();
}
