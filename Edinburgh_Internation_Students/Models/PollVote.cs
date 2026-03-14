namespace Edinburgh_Internation_Students.Models;

public class PollVote
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    public Guid PollId { get; set; }
    
    public Guid OptionId { get; set; }
    
    public int UserId { get; set; }
    
    public DateTime VotedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public Poll Poll { get; set; } = null!;
    public PollOption Option { get; set; } = null!;
    public User User { get; set; } = null!;
}
