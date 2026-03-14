using System.ComponentModel.DataAnnotations;
using Edinburgh_Internation_Students.Models;

namespace Edinburgh_Internation_Students.DTOs;

public class CreateProfileRequest
{
    [Required(ErrorMessage = "Home country is required")]
    [StringLength(100, ErrorMessage = "Home country must not exceed 100 characters")]
    public string HomeCountry { get; set; } = string.Empty;

    [StringLength(500, ErrorMessage = "Bio must not exceed 500 characters")]
    public string? ShortBio { get; set; }

    [Required(ErrorMessage = "Campus is required")]
    [StringLength(100, ErrorMessage = "Campus must not exceed 100 characters")]
    public string Campus { get; set; } = string.Empty;

    [Required(ErrorMessage = "Major/Field of study is required")]
    [StringLength(150, ErrorMessage = "Major/Field of study must not exceed 150 characters")]
    public string MajorFieldOfStudy { get; set; } = string.Empty;

    [Required(ErrorMessage = "Year of study is required")]
    public YearOfStudy YearOfStudy { get; set; }

    [Required(ErrorMessage = "At least one interest is required")]
    [MinLength(1, ErrorMessage = "At least one interest is required")]
    public List<string> Interests { get; set; } = [];

    [Required(ErrorMessage = "Preferred group size is required")]
    public PreferredGroupSize PreferredGroupSize { get; set; }

    [Required(ErrorMessage = "Matching preference is required")]
    public MatchingPreference MatchingPreference { get; set; }

    [Required(ErrorMessage = "At least one language is required")]
    [MinLength(1, ErrorMessage = "At least one language is required")]
    public List<string> Languages { get; set; } = new();
}
