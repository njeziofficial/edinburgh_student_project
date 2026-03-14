using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Edinburgh_Internation_Students.Models;

public class Profile
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int UserId { get; set; }

    [ForeignKey(nameof(UserId))]
    public User User { get; set; } = null!;

    [Required]
    [MaxLength(100)]
    public string HomeCountry { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? ShortBio { get; set; }

    [Required]
    [MaxLength(100)]
    public string Campus { get; set; } = string.Empty;

    [Required]
    [MaxLength(150)]
    public string MajorFieldOfStudy { get; set; } = string.Empty;

    [Required]
    public YearOfStudy YearOfStudy { get; set; }

    [Required]
    [MaxLength(500)]
    public string Interests { get; set; } = string.Empty; // Stored as comma-separated values

    [Required]
    public PreferredGroupSize PreferredGroupSize { get; set; }

    [Required]
    public MatchingPreference MatchingPreference { get; set; }

    [Required]
    [MaxLength(300)]
    public string Languages { get; set; } = string.Empty; // Stored as comma-separated values

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }

    // Helper methods to work with comma-separated values
    public List<string> GetInterests() =>
        [.. Interests.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(i => i.Trim())];

    public void SetInterests(List<string> interests) => 
        Interests = string.Join(", ", interests);

    public List<string> GetLanguages() =>
        [.. Languages.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(l => l.Trim())];

    public void SetLanguages(List<string> languages) => 
        Languages = string.Join(", ", languages);
}
