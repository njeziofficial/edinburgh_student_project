namespace Edinburgh_Internation_Students.DTOs.Polls;

public class PollDto
{
    public string Id { get; set; } = string.Empty;
    public string Question { get; set; } = string.Empty;
    public string GroupId { get; set; } = string.Empty;
    public string CreatorId { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public bool AllowMultipleVotes { get; set; }
    public List<PollOptionDto> Options { get; set; } = new();
    public DateTime CreatedAt { get; set; }
}
