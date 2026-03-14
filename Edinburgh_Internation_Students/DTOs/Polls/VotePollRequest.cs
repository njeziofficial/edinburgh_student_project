using System.ComponentModel.DataAnnotations;

namespace Edinburgh_Internation_Students.DTOs.Polls;

public class VotePollRequest
{
    [Required(ErrorMessage = "Option ID is required")]
    public string OptionId { get; set; } = string.Empty;
}
