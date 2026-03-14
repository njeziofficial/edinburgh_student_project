namespace Edinburgh_Internation_Students.Models;

public class PollOption
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    public Guid PollId { get; set; }
    
    public string Text { get; set; } = string.Empty;
    
    public int OrderIndex { get; set; }

    // Navigation properties
    public Poll Poll { get; set; } = null!;
    public ICollection<PollVote> Votes { get; set; } = new List<PollVote>();
}
