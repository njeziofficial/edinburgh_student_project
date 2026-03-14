using System.ComponentModel.DataAnnotations;

namespace Edinburgh_Internation_Students.DTOs.Checklists;

public class CreateChecklistItemRequest
{
    [Required(ErrorMessage = "Title is required")]
    [StringLength(255, MinimumLength = 1, ErrorMessage = "Title must be between 1 and 255 characters")]
    public string Title { get; set; } = string.Empty;
}
