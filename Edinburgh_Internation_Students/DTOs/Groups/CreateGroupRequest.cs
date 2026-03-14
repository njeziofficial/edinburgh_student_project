using System.ComponentModel.DataAnnotations;

namespace Edinburgh_Internation_Students.DTOs.Groups;

public class CreateGroupRequest
{
    [Required(ErrorMessage = "Group name is required")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Name must be between 2 and 100 characters")]
    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    [Required(ErrorMessage = "Category is required")]
    public string Category { get; set; } = string.Empty;

    public bool IsPrivate { get; set; }

    public List<string>? MemberIds { get; set; }
}
