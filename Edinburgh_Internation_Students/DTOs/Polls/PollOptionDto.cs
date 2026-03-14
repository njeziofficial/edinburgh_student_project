namespace Edinburgh_Internation_Students.DTOs.Polls;

public class PollOptionDto
{
    public string Id { get; set; } = string.Empty;
    public string PollId { get; set; } = string.Empty;
    public string Text { get; set; } = string.Empty;
    public int VoteCount { get; set; }
}
