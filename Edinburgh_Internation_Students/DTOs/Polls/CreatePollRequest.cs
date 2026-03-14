using System.ComponentModel.DataAnnotations;

namespace Edinburgh_Internation_Students.DTOs.Polls;

public class CreatePollRequest
{
    [Required(ErrorMessage = "Question is required")]
    public string Question { get; set; } = string.Empty;

    [Required(ErrorMessage = "At least 2 options are required")]
    [MinLength(2, ErrorMessage = "At least 2 options are required")]
    public List<PollOptionRequest> Options { get; set; } = new();
}

public class PollOptionRequest
{
    [Required(ErrorMessage = "Option text is required")]
    public string Text { get; set; } = string.Empty;
}
