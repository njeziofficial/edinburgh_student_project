namespace Edinburgh_Internation_Students.Models;

public class Message
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    public Guid GroupId { get; set; }
    
    public int SenderId { get; set; }
    
    public string Content { get; set; } = string.Empty;
    
    public bool IsEdited { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public Group Group { get; set; } = null!;
    public User Sender { get; set; } = null!;
    public ICollection<MessageReaction> Reactions { get; set; } = new List<MessageReaction>();
}
