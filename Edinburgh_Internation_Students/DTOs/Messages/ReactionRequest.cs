using System.ComponentModel.DataAnnotations;

namespace Edinburgh_Internation_Students.DTOs.Messages;

public class ReactionRequest
{
    [Required(ErrorMessage = "Emoji is required")]
    [StringLength(10, MinimumLength = 1, ErrorMessage = "Emoji must be between 1 and 10 characters")]
    public string Emoji { get; set; } = string.Empty;
}
