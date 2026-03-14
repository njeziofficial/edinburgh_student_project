using System.ComponentModel.DataAnnotations;

namespace Edinburgh_Internation_Students.DTOs;

public class SignUpRequest
{
    [Required(ErrorMessage = "First name is required")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "First name must be between 2 and 100 characters")]
    public string FirstName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Last name is required")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Last name must be between 2 and 100 characters")]
    public string LastName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email address")]
    [StringLength(255)]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password is required")]
    [StringLength(100, MinimumLength = 8, ErrorMessage = "Password must be at least 8 characters long")]
    public string Password { get; set; } = string.Empty;

    [StringLength(5, ErrorMessage = "Phone code must not exceed 5 characters")]
    [RegularExpression(@"^\d+$", ErrorMessage = "Phone code must contain only digits")]
    public string? PhoneCode { get; set; }

    [StringLength(15, ErrorMessage = "Phone number must not exceed 15 characters")]
    [RegularExpression(@"^\d+$", ErrorMessage = "Phone number must contain only digits")]
    public string? PhoneNumber { get; set; }
}
