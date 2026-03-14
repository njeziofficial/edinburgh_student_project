namespace Edinburgh_Internation_Students.Models;

public class MessageReaction
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    public Guid MessageId { get; set; }
    
    public int UserId { get; set; }
    
    public string Emoji { get; set; } = string.Empty;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public Message Message { get; set; } = null!;
    public User User { get; set; } = null!;
}
