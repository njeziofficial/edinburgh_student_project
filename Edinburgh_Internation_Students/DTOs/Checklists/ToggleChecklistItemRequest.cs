using System.ComponentModel.DataAnnotations;

namespace Edinburgh_Internation_Students.DTOs.Checklists;

public class ToggleChecklistItemRequest
{
    [Required]
    public bool Completed { get; set; }
}
