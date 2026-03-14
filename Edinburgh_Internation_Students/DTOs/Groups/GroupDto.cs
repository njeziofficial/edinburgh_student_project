using Edinburgh_Internation_Students.DTOs.Users;

namespace Edinburgh_Internation_Students.DTOs.Groups;

public class GroupDto
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }
    public string Category { get; set; } = string.Empty;
    public bool IsPrivate { get; set; }
    public int MemberCount { get; set; }
    public int OnlineCount { get; set; }
    public DateTime CreatedAt { get; set; }
}
