using System.ComponentModel.DataAnnotations;

namespace Edinburgh_Internation_Students.DTOs.Auth;

public class VerifyEmailRequest
{
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email address")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Verification code is required")]
    public string Code { get; set; } = string.Empty;
}
