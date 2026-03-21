using System.ComponentModel.DataAnnotations;
using BCrypt.Net;

namespace Edinburgh_Internation_Students.Models;

public class User
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string LastName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [MaxLength(255)]
    public string Email { get; set; } = string.Empty;

    [Required]
    [MaxLength(60)]
    public string Password { get; set; } = string.Empty;

    [MaxLength(5)]
    public string? PhoneCode { get; set; }

    [MaxLength(15)]
    public string? PhoneNumber { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }

    public DateTime? LastActive { get; set; }

    [Required]
    [MaxLength(20)]
    public string Role { get; set; } = "User";

    // Navigation property
    public Profile? Profile { get; set; }

    /// <summary>
    /// Hashes a plain text password using BCrypt
    /// </summary>
    /// <param name="plainPassword">The plain text password to hash</param>
    public void SetPassword(string plainPassword)
    {
        Password = BCrypt.Net.BCrypt.HashPassword(plainPassword, BCrypt.Net.BCrypt.GenerateSalt(12));
    }

    /// <summary>
    /// Verifies if the provided plain text password matches the stored hash
    /// </summary>
    /// <param name="plainPassword">The plain text password to verify</param>
    /// <returns>True if the password matches, false otherwise</returns>
    public bool VerifyPassword(string plainPassword)
    {
        return BCrypt.Net.BCrypt.Verify(plainPassword, Password);
    }
}
