using Edinburgh_Internation_Students.DTOs.Users;

namespace Edinburgh_Internation_Students.DTOs.Messages;

public class MessageDto
{
    public string Id { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string SenderId { get; set; } = string.Empty;
    public string? RecipientId { get; set; }
    public string? GroupId { get; set; }
    public bool IsRead { get; set; }
    public DateTime? ReadAt { get; set; }
    public bool IsEdited { get; set; }
    public DateTime? EditedAt { get; set; }
    public bool IsDeleted { get; set; }
    public UserSummaryDto Sender { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
}
