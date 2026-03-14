namespace Edinburgh_Internation_Students.Models;

public class Poll
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    public Guid GroupId { get; set; }
    
    public string Question { get; set; } = string.Empty;
    
    public int CreatedBy { get; set; }
    
    public bool IsActive { get; set; } = true;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime? ClosesAt { get; set; }

    // Navigation properties
    public Group Group { get; set; } = null!;
    public User CreatedByUser { get; set; } = null!;
    public ICollection<PollOption> Options { get; set; } = new List<PollOption>();
    public ICollection<PollVote> Votes { get; set; } = new List<PollVote>();
}
