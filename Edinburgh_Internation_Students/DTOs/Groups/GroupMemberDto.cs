using Edinburgh_Internation_Students.DTOs.Users;

namespace Edinburgh_Internation_Students.DTOs.Groups;

public class GroupMemberDto
{
    public string Id { get; set; } = string.Empty;
    public string GroupId { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public string Role { get; set; } = "Member"; // Member, Moderator, Admin, Owner
    public DateTime JoinedAt { get; set; }
    public UserSummaryDto User { get; set; } = null!;
}
