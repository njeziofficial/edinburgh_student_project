using Edinburgh_Internation_Students.Models;

namespace Edinburgh_Internation_Students.DTOs;

public class ProfileResponse
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string HomeCountry { get; set; } = string.Empty;
    public string? ShortBio { get; set; }
    public string Campus { get; set; } = string.Empty;
    public string MajorFieldOfStudy { get; set; } = string.Empty;
    public string YearOfStudy { get; set; } = string.Empty;
    public List<string> Interests { get; set; } = new();
    public string PreferredGroupSize { get; set; } = string.Empty;
    public string MatchingPreference { get; set; } = string.Empty;
    public List<string> Languages { get; set; } = new();
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
