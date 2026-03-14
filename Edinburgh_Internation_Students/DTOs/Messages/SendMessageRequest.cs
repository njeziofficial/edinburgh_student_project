using System.ComponentModel.DataAnnotations;

namespace Edinburgh_Internation_Students.DTOs.Messages;

public class SendMessageRequest
{
    [Required(ErrorMessage = "Content is required")]
    [StringLength(2000, MinimumLength = 1, ErrorMessage = "Message must be between 1 and 2000 characters")]
    public string Content { get; set; } = string.Empty;
}
