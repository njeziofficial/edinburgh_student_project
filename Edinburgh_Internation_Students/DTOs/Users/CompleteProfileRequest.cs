using System.ComponentModel.DataAnnotations;

namespace Edinburgh_Internation_Students.DTOs.Users;

public class CompleteProfileRequest
{
    [Required(ErrorMessage = "Country is required")]
    public string Country { get; set; } = string.Empty;

    [Required(ErrorMessage = "Campus is required")]
    public string Campus { get; set; } = string.Empty;

    [Required(ErrorMessage = "Major is required")]
    public string Major { get; set; } = string.Empty;

    [Required(ErrorMessage = "Year is required")]
    public string Year { get; set; } = string.Empty;

    public string? Bio { get; set; }

    [Required(ErrorMessage = "At least one interest is required")]
    [MinLength(1, ErrorMessage = "At least one interest is required")]
    public List<string> Interests { get; set; } = new();

    [Required(ErrorMessage = "At least one language is required")]
    [MinLength(1, ErrorMessage = "At least one language is required")]
    public List<string> Languages { get; set; } = new();
}
