namespace Edinburgh_Internation_Students.DTOs.Groups;

public class UpdateGroupRequest
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? Category { get; set; }
    public bool? IsPrivate { get; set; }
}
