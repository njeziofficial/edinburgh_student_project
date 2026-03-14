using System.ComponentModel.DataAnnotations;
using Edinburgh_Internation_Students.Models;

namespace Edinburgh_Internation_Students.DTOs;

public class UpdateProfileRequest
{
    [StringLength(100, ErrorMessage = "Home country must not exceed 100 characters")]
    public string? HomeCountry { get; set; }

    [StringLength(500, ErrorMessage = "Bio must not exceed 500 characters")]
    public string? ShortBio { get; set; }

    [StringLength(100, ErrorMessage = "Campus must not exceed 100 characters")]
    public string? Campus { get; set; }

    [StringLength(150, ErrorMessage = "Major/Field of study must not exceed 150 characters")]
    public string? MajorFieldOfStudy { get; set; }

    public YearOfStudy? YearOfStudy { get; set; }

    [MinLength(1, ErrorMessage = "At least one interest is required")]
    public List<string>? Interests { get; set; }

    public PreferredGroupSize? PreferredGroupSize { get; set; }

    public MatchingPreference? MatchingPreference { get; set; }

    [MinLength(1, ErrorMessage = "At least one language is required")]
    public List<string>? Languages { get; set; }
}
