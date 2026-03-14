namespace Edinburgh_Internation_Students.DTOs.Groups;

public class UserGroupsResponse
{
    public List<GroupDto> Groups { get; set; } = new();
    public int TotalGroups { get; set; }
    public int TotalMemberCount { get; set; }
}
